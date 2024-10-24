// <copyright file="XVerProcessor.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Text;
using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;
using Microsoft.Health.Fhir.CodeGen.CompareTool;
using Microsoft.Health.Fhir.CodeGen.Configuration;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;

#if NETSTANDARD2_0
using Microsoft.Health.Fhir.CodeGenCommon.Polyfill;
#endif


namespace Microsoft.Health.Fhir.CodeGen.XVer;

public class XVerProcessor
{
    private enum ComparisonDirection
    {
        Up,
        Down
    }

    internal static readonly HashSet<string> _exclusionSet =
    [
        "http://hl7.org/fhir/ValueSet/ucum-units",
        "http://hl7.org/fhir/ValueSet/all-languages",
        "http://hl7.org/fhir/ValueSet/mimetypes",
    ];

    internal static readonly HashSet<string> _escapeValveCodes = [
        "OTHER",
        "Other",
        "other",
        "OTH",      // v3 Null Flavor of other
        "UNKNOWN",
        "Unknown",
        "unknown",
        "UNK",      // v3 Null Flavor of Unknown
        //"NI",       // v3 Null Flavor of No Information
        ];

    private ConfigXVer _config;
    private DefinitionCollection[] _definitions;
    private Dictionary<string, CrossVersionMapCollection> _crossVersionMaps = [];

    public XVerProcessor(ConfigXVer config, IEnumerable<DefinitionCollection> definitions)
    {
        _config = config;
        _definitions = [.. definitions];
    }

    public void Compare()
    {
        if (_definitions.Length < 2)
        {
            throw new InvalidOperationException("At least two definitions are required to compare.");
        }

        HashSet<string> vsUrlsToInclude = [];

        // we need to discover which value sets have a required binding in any set of definitions
        foreach (DefinitionCollection dc in _definitions)
        {
            // iterate over the value sets in the first definition collection
            foreach ((string unversionedUrl, string[] versions) in dc.ValueSetVersions.OrderBy(kvp => kvp.Key))
            {
                // skip value sets we know we will not process
                if (_exclusionSet.Contains(unversionedUrl))
                {
                    continue;
                }

                // only compare on the highest version in this package

                string vsVersion = versions.OrderDescending().First();
                string versionedUrl = unversionedUrl + "|" + vsVersion;

                // TODO(ginoc): We should add a flag to process all expandable value sets for use in mapping, but do not need right now

                // we only need to process value sets that have a required binding
                if (hasRequiredBinding(dc, versionedUrl, unversionedUrl))
                {
                    vsUrlsToInclude.Add(unversionedUrl);
                }
            }
        }

        // walk the definitions to compare versions next to each other
        for (int definitionIndex = 1; definitionIndex < _definitions.Length; definitionIndex++)
        {
            // grab our definition collection pair
            DefinitionCollection dc1 = _definitions[definitionIndex - 1];
            DefinitionCollection dc2 = _definitions[definitionIndex];

            // get cross version maps for each direction
            CrossVersionMapCollection cvMapUp = getMapCollection(dc1, dc2);
            CrossVersionMapCollection cvMapDown = getMapCollection(dc2, dc1);

            // compare value sets in each direction
            compareValueSets(dc1, dc2, vsUrlsToInclude, cvMapUp, ComparisonDirection.Up);
            compareValueSets(dc2, dc1, vsUrlsToInclude, cvMapDown, ComparisonDirection.Down);
        }

        Console.WriteLine("done!");
    }

    /// <summary>
    /// Compares the value sets between the source and target definition collections.
    /// </summary>
    /// <param name="dcSource">The source definition collection.</param>
    /// <param name="dcTarget">The target definition collection.</param>
    /// <param name="vsUrlsToInclude">The set of value set URLs to include in the comparison.</param>
    /// <param name="cvMap">The cross-version map collection.</param>
    /// <param name="direction">The direction of the comparison.</param>
    private void compareValueSets(
            DefinitionCollection dcSource,
            DefinitionCollection dcTarget,
            HashSet<string> vsUrlsToInclude,
            CrossVersionMapCollection cvMap,
            ComparisonDirection direction)
    {
        // iterate over the value sets in the first definition collection
        foreach ((string unversionedUrl, string[] versions) in dcSource.ValueSetVersions.OrderBy(kvp => kvp.Key))
        {
            // only process value sets we have already determined should be compared
            if (!vsUrlsToInclude.Contains(unversionedUrl))
            {
                continue;
            }

            // only compare on the highest version in this package
            string vsVersion = versions.OrderDescending().First();
            string versionedUrl = unversionedUrl + "|" + vsVersion;

            // we can only process value sets we can expand
            if (!dcSource.TryExpandVs(versionedUrl, out ValueSet? vs, out string? expandMessage))
            {
                // get the unexpanded value set object
                if (dcSource.ValueSetsByVersionedUrl.TryGetValue(versionedUrl, out vs))
                {
                    if ((!vs.TryGetAnnotation(out ValueSetComparisonAnnotation? ca)) ||
                        (ca == null))
                    {
                        ca = new();
                        vs.AddAnnotation(ca);
                    }

                    ca.FailureCode = ComparisonFailureCodes.CannotExpand;
                    ca.FailureMessage = $"Failed to expand value set {versionedUrl} for comparison: {expandMessage}.";
                }

                Console.WriteLine($"Error: compareValueSets cannot process {versionedUrl} since it cannot be expanded: {expandMessage}");
                continue;
            }

            // get or create the comparison annotation for this VS
            if ((!vs.TryGetAnnotation(out ValueSetComparisonAnnotation? comparisonAnnotation)) ||
                (comparisonAnnotation == null))
            {
                comparisonAnnotation = new()
                {
                    EscapeValveCodes = getEscapeValveCodes(vs),
                };
                vs.AddAnnotation(comparisonAnnotation);
            }

            List<ValueSetComparisonDetails> detailsList = direction == ComparisonDirection.Up
                ? comparisonAnnotation.ToNext
                : comparisonAnnotation.ToPrev;

            HashSet<string> processedTargets = [];

            // get any mappings for this value set (use the versioned URL to get the versioned and unversioned maps)
            List<ConceptMap> vsConceptMaps = cvMap.GetMapsForSource(versionedUrl);
            foreach (ConceptMap cm in vsConceptMaps)
            {
                string cmTarget = cm.TargetScope is Canonical targetCanonical
                    ? targetCanonical.Value ?? targetCanonical.Uri ?? string.Empty
                    : cm.TargetScope is FhirUri targetUri
                    ? targetUri.Value ?? string.Empty
                    : string.Empty;

                if (string.IsNullOrEmpty(cmTarget))
                {
                    continue;
                }

                // check for already being processed
                if (processedTargets.Contains(cmTarget))
                {
                    continue;
                }

                // check to see if we have an expandable target value set
                if (!dcTarget.TryExpandVs(cmTarget, out ValueSet? mappedTargetVs))
                {
                    detailsList.Add(new()
                    {
                        Target = null,
                        FailureCode = ComparisonFailureCodes.UnresolvedTarget,
                        FailureMessage = $"Failed to resolve target scope for value set {versionedUrl} from {cm.Url}.",
                        ExplicitMappingSource = cm.Url,
                        ConceptDomain = null,
                        ValueSetConcepts = null,
                    });

                    continue;
                }

                // run this comparison and add our results
                detailsList.Add(compareValueSet(vs, mappedTargetVs, cm));
            }

            // check for this valueset exactly in the target collection
            if (!processedTargets.Contains(unversionedUrl) &&
                dcTarget.TryExpandVs(unversionedUrl, out ValueSet? unversionedVs))
            {
                processedTargets.Add(unversionedUrl);
                detailsList.Add(compareValueSet(vs, unversionedVs, null));
            }
        }
    }

    /// <summary>
    /// Applies the relationship between existing and change concept domain relationship codes.
    /// </summary>
    /// <param name="existing">The existing concept domain relationship code.</param>
    /// <param name="change">The change concept domain relationship code.</param>
    /// <returns>The resulting concept domain relationship code.</returns>
    private ConceptDomainRelationshipCodes applyRelationship(ConceptDomainRelationshipCodes? existing, ConceptDomainRelationshipCodes? change) => existing switch
    {
        ConceptDomainRelationshipCodes.Unknown => change ?? ConceptDomainRelationshipCodes.Unknown,
        ConceptDomainRelationshipCodes.Equivalent => cdrCodeIsBroader(change)
            ? ConceptDomainRelationshipCodes.SourceIsBroaderThanTarget
            : cdrCodeIsNarrower(change)
            ? ConceptDomainRelationshipCodes.SourceIsNarrowerThanTarget
            : change ?? ConceptDomainRelationshipCodes.Equivalent,
        ConceptDomainRelationshipCodes.SourceIsNew => cdrCodeIsBroader(change)
            ? ConceptDomainRelationshipCodes.SourceIsBroaderThanTarget
            : ConceptDomainRelationshipCodes.Related,
        ConceptDomainRelationshipCodes.SourceIsDeprecated => cdrCodeIsNarrower(change)
            ? ConceptDomainRelationshipCodes.SourceIsNarrowerThanTarget
            : ConceptDomainRelationshipCodes.Related,
        ConceptDomainRelationshipCodes.NotMapped => cdrCodeIsBroader(change)
            ? ConceptDomainRelationshipCodes.SourceIsBroaderThanTarget
            : ConceptDomainRelationshipCodes.Related,
        ConceptDomainRelationshipCodes.SourceIsNarrowerThanTarget => cdrCodeIsNarrower(change)
            ? ConceptDomainRelationshipCodes.SourceIsNarrowerThanTarget
            : ConceptDomainRelationshipCodes.Related,
        ConceptDomainRelationshipCodes.SourceIsBroaderThanTarget => cdrCodeIsBroader(change)
            ? ConceptDomainRelationshipCodes.SourceIsBroaderThanTarget
            : ConceptDomainRelationshipCodes.Related,
        ConceptDomainRelationshipCodes.Related => (change == ConceptDomainRelationshipCodes.NotRelated)
            ? ConceptDomainRelationshipCodes.NotRelated
            : ConceptDomainRelationshipCodes.Related,
        ConceptDomainRelationshipCodes.NotRelated => change ?? ConceptDomainRelationshipCodes.NotRelated,
        _ => change ?? existing ?? ConceptDomainRelationshipCodes.Unknown,
    };

    /// <summary>
    /// Determines if the given ConceptDomainRelationshipCodes is narrower.
    /// </summary>
    /// <param name="cdr">The ConceptDomainRelationshipCodes to check.</param>
    /// <returns>True if the ConceptDomainRelationshipCodes is narrower; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool cdrCodeIsNarrower(ConceptDomainRelationshipCodes? cdr) =>
        cdr == ConceptDomainRelationshipCodes.SourceIsNarrowerThanTarget ||
        cdr == ConceptDomainRelationshipCodes.SourceIsDeprecated;

    /// <summary>
    /// Determines if the given ConceptDomainRelationshipCodes is broader.
    /// </summary>
    /// <param name="cdr">The ConceptDomainRelationshipCodes to check.</param>
    /// <returns>True if the ConceptDomainRelationshipCodes is broader; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool cdrCodeIsBroader(ConceptDomainRelationshipCodes? cdr) =>
        cdr == ConceptDomainRelationshipCodes.SourceIsBroaderThanTarget ||
        cdr == ConceptDomainRelationshipCodes.SourceIsNew ||
        cdr == ConceptDomainRelationshipCodes.NotMapped;

    /// <summary>
    /// Compares two ValueSets and returns the comparison details.
    /// </summary>
    /// <param name="sourceVs">The source ValueSet.</param>
    /// <param name="targetVs">The target ValueSet.</param>
    /// <param name="cm">The ConceptMap for mapping concepts between the ValueSets.</param>
    /// <returns>The comparison details of the ValueSets.</returns>
    private ValueSetComparisonDetails compareValueSet(
        ValueSet sourceVs,
        ValueSet targetVs,
        ConceptMap? cm)
    {
        // build our concept comparison dictionary
        Dictionary<string, ValueSetConceptComparisonDetails[]>? vsConceptComparisons = compareValueSetConcepts(sourceVs, targetVs, cm);

        // start optimistically
        ConceptDomainRelationshipCodes vsRelationship = ConceptDomainRelationshipCodes.Equivalent;

        // iterate over our concept comparisons to determine the overall relationship
        foreach (ValueSetConceptComparisonDetails vscDetails in vsConceptComparisons?.Values.SelectMany(v => v) ?? [])
        {
            vsRelationship = applyRelationship(vsRelationship, vscDetails.ConceptDomain?.Relationship);
        }

        return new()
        {
            Target = targetVs,
            ExplicitMappingSource = cm?.Url,
            ConceptDomain = new()
            {
                Relationship = vsRelationship,
            },
            ValueSetConcepts = vsConceptComparisons,
        };
    }

    /// <summary>
    /// Retrieves the escape valve codes from the specified ValueSet.
    /// </summary>
    /// <param name="vs">The ValueSet to retrieve the escape valve codes from.</param>
    /// <returns>An array of escape valve codes.</returns>
    private List<string> getEscapeValveCodes(
        ValueSet vs)
    {
        List<string> assumedEscapeValveCodes = [];

        // check all our codes to see if there is an 'escape valve' code
        foreach (ValueSet.ContainsComponent source in vs.cgGetFlatContains())
        {
            if (!_escapeValveCodes.Contains(source.Code))
            {
                continue;
            }

            // add this code to our assumed set
            assumedEscapeValveCodes.Add(source.cgKey());
        }

        return assumedEscapeValveCodes;
    }

    /// <summary>
    /// Compares the concepts of two value sets and generates a dictionary of comparison details.
    /// </summary>
    /// <param name="sourceVs">The source value set.</param>
    /// <param name="targetVs">The target value set.</param>
    /// <param name="cm">The concept map.</param>
    /// <returns>A dictionary containing the comparison details for each concept in the source value set.</returns>
    private Dictionary<string, ValueSetConceptComparisonDetails[]>? compareValueSetConcepts(
        ValueSet sourceVs,
        ValueSet targetVs,
        ConceptMap? cm)
    {
        HashSet<string> escapeValveKeys = sourceVs.TryGetAnnotation(typeof(ValueSetComparisonAnnotation), out object? annotation)
            ? new HashSet<string>(((ValueSetComparisonAnnotation)annotation).EscapeValveCodes ?? [])
            : [];

        Dictionary<string, ValueSetConceptComparisonDetails[]> retVal = [];

        // build a dictionary of target keys so that we can determine if something exists
        Dictionary<string, ValueSet.ContainsComponent> targetContainsDict = targetVs.cgGetFlatContains().ToDictionary(c => c.System + "#" + c.Code);

        HashSet<string> noMaps;
        Dictionary<string, Dictionary<string, ConceptMap.TargetElementComponent>> mapTargetsByKeyBySourceKey;

        (noMaps, mapTargetsByKeyBySourceKey) = processValueSetConceptMap(sourceVs.Url, targetVs.Url, cm);

        ValueSet.ContainsComponent[] sourceFlat = sourceVs.cgGetFlatContains().ToArray();

        // iterate over the source expansion and build our comparisons
        foreach (ValueSet.ContainsComponent source in sourceFlat)
        {
            string sourceKey = source.cgKey();
            List<ValueSetConceptComparisonDetails> vscDetails = [];

            // if we have a no-map, use that first
            if (noMaps.Contains(sourceKey))
            {
                vscDetails.Add(new()
                {
                    Source = source,
                    Target = null,
                    ExplicitMappingSource = cm?.Url,
                    ConceptDomain = new()
                    {
                        Relationship = ConceptDomainRelationshipCodes.NotMapped,
                    },
                    ValueDomain = new()
                    {
                        ConceptRelationship = ValueSetConceptRelationshipFlags.Removed,
                        Messages = [$"{sourceKey} explicitly not mapped in {cm?.Url}"],
                    },
                });
            }

            // if we have mappings, use those
            if (mapTargetsByKeyBySourceKey.TryGetValue(sourceKey, out Dictionary<string, ConceptMap.TargetElementComponent>? mapTargetsByKey))
            {
                // iterate over the targets for this source
                foreach ((string targetKey, ConceptMap.TargetElementComponent cmTarget) in mapTargetsByKey)
                {
                    // check for the target in the target value set
                    if (!targetContainsDict.TryGetValue(targetKey, out ValueSet.ContainsComponent? mappedTarget))
                    {
                        vscDetails.Add(new()
                        {
                            Source = source,
                            Target = null,
                            FailureCode = ComparisonFailureCodes.UnresolvedTarget,
                            FailureMessage = $"Failed to resolve target scope for value set { sourceVs.Url} from { cm!.Url} - expected relationship of {cmTarget.Relationship}.",
                            ExplicitMappingSource = cm?.Url,
                            ConceptDomain = null,
                            ValueDomain = null,
                        });

                        continue;
                    }

                    // start with whatever was mapped
                    ConceptDomainRelationshipCodes conceptDomain = cmTarget.Relationship.ToDomainRelationship();

                    vscDetails.Add(new()
                    {
                        Source = source,
                        Target = mappedTarget,
                        ExplicitMappingSource = cm?.Url,
                        ConceptDomain = new()
                        {
                            Relationship = conceptDomain,
                        },
                        ValueDomain = new()
                        {
                            ConceptRelationship = valueDomainForVsConcept(source.System, source.Code, targetKey),
                            Messages = [
                                $"{sourceKey} mapped with relationship {cmTarget.Relationship} to {targetKey} via {cm?.Url}"
                                ],
                        },
                    });
                }
            }

            // if we have nothing by this point, try to compare literals
            if ((vscDetails.Count == 0) &&
                targetContainsDict.TryGetValue(sourceKey, out ValueSet.ContainsComponent? matchedTarget))
            {
                vscDetails.Add(new()
                {
                    Source = source,
                    Target = matchedTarget,
                    ExplicitMappingSource = cm?.Url,
                    ConceptDomain = new()
                    {
                        Relationship = ConceptDomainRelationshipCodes.Equivalent,
                    },
                    ValueDomain = new()
                    {
                        ConceptRelationship = ValueSetConceptRelationshipFlags.Equivalent,
                        Messages = [
                            $"{sourceKey} found exact match to literal with no map - assumed equivalent in {targetVs.Url}"
                            ],
                    },
                });
            }

            // finally, if we have not found anything, it is an implicit no map
            if (vscDetails.Count == 0)
            {
                vscDetails.Add(new()
                {
                    Source = source,
                    Target = null,
                    ExplicitMappingSource = cm?.Url,
                    ConceptDomain = new()
                    {
                        Relationship = ConceptDomainRelationshipCodes.NotMapped,
                    },
                    ValueDomain = new()
                    {
                        ConceptRelationship = ValueSetConceptRelationshipFlags.Removed,
                        Messages = [$"{sourceKey} not mapped - no mapping found and a matching literal was not found in {targetVs.Url}"],
                    },
                });
            }

            // if this is an escape-valve code, we want to check equivalency
            if (escapeValveKeys.Contains(sourceKey))
            {
                List<KeyValuePair<ValueSetConceptComparisonDetails, ValueSetConceptComparisonDetails>> toReplace = [];

                // loop over the existing details and check the relationships
                foreach (ValueSetConceptComparisonDetails vscDetail in vscDetails)
                {
                    if (vscDetail.ConceptDomain?.Relationship != ConceptDomainRelationshipCodes.Equivalent)
                    {
                        continue;
                    }

                    // check the number of codes in the source and target value sets
                    if (sourceFlat.Length != targetContainsDict.Count)
                    {
                        // this should not be equivalent
                        ConceptDomainRelationshipCodes r = sourceFlat.Length > targetContainsDict.Count
                            ? ConceptDomainRelationshipCodes.SourceIsNarrowerThanTarget     // more source codes means that other is a narrower concept
                            : ConceptDomainRelationshipCodes.SourceIsBroaderThanTarget;     // more target codes means that other is a broader concept

                        List<string> messages = vscDetail.ConceptDomain.Messages;
                        messages.Add(
                            $"Modified escape-type relationship based on concept domains covered:" +
                            $" source ({sourceKey}) has {sourceFlat.Length} concepts and" +
                            $" target ({targetVs.Url}|{targetVs.Version}) has {targetContainsDict.Count}. ");

                        toReplace.Add(new(
                            vscDetail,
                            vscDetail with
                            {
                                ConceptDomain = vscDetail.ConceptDomain with
                                {
                                    Relationship = r,
                                    Messages = messages,
                                },
                            }));
                    }
                }

                foreach ((ValueSetConceptComparisonDetails original, ValueSetConceptComparisonDetails updated) in toReplace)
                {
                    vscDetails.Remove(original);
                    vscDetails.Add(updated);
                }
            }

            retVal.Add(sourceKey, vscDetails.ToArray());
        }

        return retVal;
    }

    /// <summary>
    /// Determines the relationship between a source value set concept and a target value set concept.
    /// </summary>
    /// <param name="sourceSystem">The system of the source value set concept.</param>
    /// <param name="sourceCode">The code of the source value set concept.</param>
    /// <param name="targetKey">The key of the target value set concept.</param>
    /// <returns>The relationship between the source and target value set concepts.</returns>
    private ValueSetConceptRelationshipFlags valueDomainForVsConcept(
        string sourceSystem,
        string sourceCode,
        string? targetKey)
    {
        if (string.IsNullOrEmpty(targetKey) || (targetKey == "#"))
        {
            return ValueSetConceptRelationshipFlags.Removed;
        }

        ValueSetConceptRelationshipFlags retVal = ValueSetConceptRelationshipFlags.None;

        string[] targetComponents = targetKey!.Split('#');
        string targetSystem = targetComponents[0];
        string targetCode = targetComponents.Length > 1 ? targetComponents[1] : string.Empty;

        if (sourceSystem != targetSystem)
        {
            retVal |= ValueSetConceptRelationshipFlags.SystemChanged;
        }

        if (sourceCode != targetCode)
        {
            retVal |= ValueSetConceptRelationshipFlags.Renamed;
        }

        return retVal;
    }

    /// <summary>
    /// Extracts the unversioned URL from the given URL.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <returns>The unversioned URL.</returns>
    private string getUnversionedUrl(string url) => url.Contains('|') ? url.Split('|')[0] : url;

    /// <summary>
    /// Processes the concept map for the value set.
    /// </summary>
    /// <param name="sourceVsUrl">The URL of the source value set.</param>
    /// <param name="targetVsUrl">The URL of the target value set.</param>
    /// <param name="cm">The concept map.</param>
    /// <returns>A tuple containing the mappings between source and target value set concepts.</returns>
    private (HashSet<string> noMaps, Dictionary<string, Dictionary<string, ConceptMap.TargetElementComponent>> mapTargetsByKeyBySourceKey) processValueSetConceptMap(
        string sourceVsUrl,
        string targetVsUrl,
        ConceptMap? cm)
    {
        if (cm == null)
        {
            return ([], []);
        }

        HashSet<string> noMaps = [];

        // build a map of our concept map to simplify lookups
        Dictionary<string, Dictionary<string, ConceptMap.TargetElementComponent>> mapTargetsByKeyBySourceKey = [];

        // traverse the groups in our map - each group represents a system
        foreach (ConceptMap.GroupComponent cmGroup in cm.Group)
        {
            string groupSourceSystem = cmGroup.Source ?? getUnversionedUrl(sourceVsUrl);
            string groupTargetSystem = cmGroup.Target ?? getUnversionedUrl(targetVsUrl);

            // add all the elements from this group to our lookup
            foreach (ConceptMap.SourceElementComponent cmElement in cmGroup.Element)
            {
                string sourceKey = $"{groupSourceSystem}#{cmElement.Code}";

                // check for sources without targets
                if ((cmElement.NoMap == true) || (cmElement.Target.Count == 0))
                {
                    if (!noMaps.Contains(sourceKey))
                    {
                        noMaps.Add(sourceKey);
                    }

                    continue;
                }

                // grab the targets for this source
                if (!mapTargetsByKeyBySourceKey.TryGetValue(sourceKey, out Dictionary<string, ConceptMap.TargetElementComponent>? mapTargets))
                {
                    mapTargets = [];
                    mapTargetsByKeyBySourceKey.Add(sourceKey, mapTargets);
                }

                // add our targets
                foreach (ConceptMap.TargetElementComponent cmTarget in cmElement.Target)
                {
                    string targetKey = $"{groupTargetSystem}#{cmTarget.Code}";
                    mapTargets.Add(targetKey, cmTarget);
                }
            }
        }

        return (noMaps, mapTargetsByKeyBySourceKey);
    }

    /// <summary>
    /// Checks if the specified value set has a required binding in the given definition collection.
    /// </summary>
    /// <param name="dc">The definition collection.</param>
    /// <param name="versionedUrl">The versioned URL of the value set.</param>
    /// <param name="unversionedUrl">The unversioned URL of the value set.</param>
    /// <returns>True if the value set has a required binding, false otherwise.</returns>
    private bool hasRequiredBinding(
        DefinitionCollection dc,
        string versionedUrl,
        string unversionedUrl)
    {
        IEnumerable<StructureElementCollection> coreBindingsUnversioned = dc.CoreBindingsForVs(unversionedUrl);
        if (dc.StrongestBinding(coreBindingsUnversioned) == BindingStrength.Required)
        {
            return true;
        }

        IEnumerable<StructureElementCollection> coreBindingsVersioned = dc.CoreBindingsForVs(versionedUrl);
        if (dc.StrongestBinding(coreBindingsVersioned) == BindingStrength.Required)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Retrieves the cross-version map collection for the given definition collections.
    /// </summary>
    /// <param name="dc1">The first definition collection.</param>
    /// <param name="dc2">The second definition collection.</param>
    /// <returns>The cross-version map collection.</returns>
    private CrossVersionMapCollection getMapCollection(DefinitionCollection dc1, DefinitionCollection dc2)
    {
        string cvMapKey = $"{dc1.MainPackageId}@{dc1.MainPackageVersion}-{dc2.MainPackageId}@{dc2.MainPackageVersion}";

        if (_crossVersionMaps.TryGetValue(cvMapKey, out CrossVersionMapCollection? cvMap))
        {
            return cvMap;
        }

        cvMap = new(dc1, dc2);

        if (!cvMap.TryLoadCrossVersionMaps(_config.CrossVersionMapSourcePath))
        {
            Console.WriteLine($"Failed to load requested cross-version maps for {cvMapKey}! Processing will be only algorithmic!");
        }

        _crossVersionMaps.Add(cvMapKey, cvMap);

        return cvMap;
    }
}
