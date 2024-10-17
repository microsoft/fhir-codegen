// <copyright file="XVerProcessor.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;
using Microsoft.Health.Fhir.CodeGen.CompareTool;
using Microsoft.Health.Fhir.CodeGen.Configuration;
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
            compareValueSets(dc1, dc2, cvMapUp, ComparisonDirection.Up);
            compareValueSets(dc2, dc1, cvMapDown, ComparisonDirection.Down);
        }

        Console.WriteLine("done!");
    }

    private void compareValueSets(
        DefinitionCollection dcSource,
        DefinitionCollection dcTarget,
        CrossVersionMapCollection cvMap,
        ComparisonDirection direction)
    {
        // iterate over the value sets in the first definition collection
        foreach ((string unversionedUrl, string[] versions) in dcSource.ValueSetVersions.OrderBy(kvp => kvp.Key))
        {
            // skip value sets we know we do not care about
            if (_exclusionSet.Contains(unversionedUrl))
            {
                continue;
            }

            // only compare on the highest version in this package
            string vsVersion = versions.OrderDescending().First();
            string versionedUrl = unversionedUrl + "|" + vsVersion;

            // we only need to process value sets that have a required binding
            if (!hasRequiredBinding(dcSource, versionedUrl, unversionedUrl))
            {
                continue;
            }

            // only process value sets we can expand
            if (!dcSource.TryExpandVs(versionedUrl, out ValueSet? vs))
            {
                continue;
            }

            // TODO(ginoc): need to use annotations on the base VS, not the expansion!!!

            // get or create the comparison annotation for this VS
            if ((!vs.TryGetAnnotation(out ValueSetComparisonAnnotation? comparisonAnnotation)) ||
                (comparisonAnnotation == null))
            {
                comparisonAnnotation = new();
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

    private ValueSetComparisonDetails compareValueSet(
        ValueSet sourceVs,
        ValueSet targetVs,
        ConceptMap? cm)
    {
        // build our concept comparison dictionary
        Dictionary<string, ValueSetConceptComparisonDetails[]>? vsConceptComparisons = compareValueSetConcepts(sourceVs, targetVs, cm);

        return new()
        {
            Target = targetVs,
            ExplicitMappingSource = cm?.Url,
            ConceptDomain = new()
            {
                Relationship = ConceptDomainRelationshipCodes.Related,
            },
            ValueSetConcepts = vsConceptComparisons,
        };
    }

    private Dictionary<string, ValueSetConceptComparisonDetails[]>? compareValueSetConcepts(
        ValueSet sourceVs,
        ValueSet targetVs,
        ConceptMap? cm)
    {
        Dictionary<string, ValueSetConceptComparisonDetails[]> retVal = [];

        // build a dictionary of target keys so that we can determine if something exists
        Dictionary<string, ValueSet.ContainsComponent> targetContainsDict = targetVs.Expansion.Contains.ToDictionary(c => c.System + "#" + c.Code);

        HashSet<string> noMaps;
        Dictionary<string, Dictionary<string, ConceptMap.TargetElementComponent>> mapTargetsByKeyBySourceKey;

        (noMaps, mapTargetsByKeyBySourceKey) = processValueSetConceptMap(sourceVs.Url, targetVs.Url, cm);

        // iterate over the source expansion and build our comparisons
        foreach (ValueSet.ContainsComponent source in sourceVs.Expansion.Contains)
        {
            // skip non-selectable entries in value sets
            if (string.IsNullOrEmpty(source.Code))
            {
                continue;
            }

            string sourceKey = source.System + "#" + source.Code;
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

            retVal.Add(sourceKey, vscDetails.ToArray());
        }

        return retVal;
    }

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

    private CrossVersionMapCollection getMapCollection(DefinitionCollection dc1, DefinitionCollection dc2)
    {
        string cvMapKey = $"{dc1.MainPackageId}@{dc1.MainPackageVersion}-{dc2.MainPackageId}@{dc2.MainPackageVersion}";

        if (_crossVersionMaps.TryGetValue(cvMapKey, out CrossVersionMapCollection? cvMap))
        {
            return cvMap;
        }

        cvMap = new(dc1, dc2);
        _crossVersionMaps.Add(cvMapKey, cvMap);

        return cvMap;
    }
}
