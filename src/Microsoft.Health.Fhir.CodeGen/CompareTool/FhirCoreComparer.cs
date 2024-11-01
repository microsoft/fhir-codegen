// <copyright file="FhirCoreComparer.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.CodeGen.CompareTool;
using Microsoft.Health.Fhir.CodeGen.Configuration;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;
using Microsoft.Health.Fhir.CodeGen.Language;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using Microsoft.Health.Fhir.CodeGenCommon.Utils;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;
using Microsoft.Health.Fhir.CodeGenCommon.FhirExtensions;
using System.Text.RegularExpressions;




#if NETSTANDARD2_0
using Microsoft.Health.Fhir.CodeGenCommon.Polyfill;
#endif


namespace Microsoft.Health.Fhir.CodeGen.CompareTool;


internal static partial class FhirCoreComparerLogMessages
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Comparing {leftKey} and {rightKey}.")]
    internal static partial void LogComparisonStart(this ILogger logger, string leftKey, string rightKey);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to find maps for {cvMapKey}! Processing will be only algorithmic!")]
    internal static partial void LogMapsNotFound(this ILogger logger, string cvMapKey);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to load maps for {aKey} and {bKey}, processing will be only algorithmic!")]
    internal static partial void LogMapsNotLoaded(this ILogger logger, string aKey, string bKey);

    [LoggerMessage(Level = LogLevel.Warning, Message = "ValueSet {url} not compared because it is in the manual exclusion list.")]
    internal static partial void LogValueSetExcluded(this ILogger logger, string url);

    [LoggerMessage(Level = LogLevel.Warning, Message = "ValueSet {url} not compared because it has no maps and does not exist in the target.")]
    internal static partial void LogValueSetNoTarget(this ILogger logger, string url);

    [LoggerMessage(Level = LogLevel.Warning, Message = "ValueSet {vsUrl} not compared because the map {mapUrl} does not have a valid target.")]
    internal static partial void LogValueSetInvalidTarget(this ILogger logger, string vsUrl, string mapUrl);

    [LoggerMessage(Level = LogLevel.Warning, Message = "ValueSet {url} not compared because this ValueSet has no discovered required bindings.")]
    internal static partial void LogValueSetNoRequiredBindings(this ILogger logger, string url);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to expand ValueSet {url} for comparison: {details}")]
    internal static partial void LogValueSetNotExpanded(this ILogger logger, string url, string? details);
}

public class FhirCoreComparer
{
    internal static readonly HashSet<string> _exclusionSet = [
        /* UCUM is used as a required binding in a codeable concept. Since we do not
         * use enums in this situation, it is not useful to generate this valueset
         */
        "http://hl7.org/fhir/ValueSet/ucum-units",

        /* R5 made Resource.language a required binding to all-languages, which contains
         * all of bcp:47 and is listed as infinite. This is not useful to generate.
         * Note that in R5, many elements that are required to all-languages also have bound
         * starter value sets.  TODO: consider if we want to generate constants for those.
         */
        "http://hl7.org/fhir/ValueSet/all-languages",

        /* MIME types are infinite, so we do not want to generate these.
         * Note that in R5, many elements that are required to MIME type also have bound
         * starter value sets.  TODO: consider if we want to generate constants for those.
         */
        "http://hl7.org/fhir/ValueSet/mimetypes",

        /* ISO 3166 is large and has not changed since used in this context - do not map it
         * This should not need to be in the list - it is a system not a value set
         */
        //"urn:iso:std:iso:3166",
        //"urn:iso:std:iso:3166:-2",
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
        ];


    private ConfigCompare _config;
    private ILogger _logger;

    private DefinitionCollection _leftDc;
    private string _leftShortVersion;
    private string _leftRLiteral;
    private string _leftKey;

    private DefinitionCollection _rightDc;
    private string _rightShortVersion;
    private string _rightRLiteral;
    private string _rightKey;

    private CrossVersionMapCollection? _cvLeftToRight = null;
    private CrossVersionMapCollection? _cvRightToLeft = null;

    private Dictionary<string, List<PairComparison<ValueSet>>> _valueSetComparisons = [];

    public FhirCoreComparer(ConfigCompare config, DefinitionCollection left, DefinitionCollection right)
    {
        _config = config;
        _logger = config.LogFactory.CreateLogger<FhirCoreComparer>();

        _leftDc = left;
        _leftShortVersion = left.FhirSequence.ToShortVersion();
        _leftRLiteral = left.FhirSequence.ToRLiteral();
        _leftKey = $"{_leftDc.MainPackageId}@{_leftDc.MainPackageVersion}";

        _rightDc = right;
        _rightShortVersion = right.FhirSequence.ToShortVersion();
        _rightRLiteral = right.FhirSequence.ToRLiteral();
        _rightKey = $"{_rightDc.MainPackageId}@{_rightDc.MainPackageVersion}";
    }

    public void Compare()
    {
        _logger.LogComparisonStart(_leftKey, _rightKey);

        // load cross-version maps in both directions
        (_cvLeftToRight, _cvRightToLeft) = getInitialMaps();

        // first, process value sets
        compareAllValueSets();

        if (_config.SaveComparisonResult)
        {
            saveValueSetMaps();
        }
    }

    private void saveValueSetMaps()
    {
        string dir = string.IsNullOrEmpty(_config.CrossVersionMapSourcePath)
            ? Path.Combine(_config.OutputDirectory, "input", "codes_v2")
            : Path.Combine(_config.CrossVersionMapSourcePath, $"codes_v2");

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        _cvLeftToRight?.SaveValueSetConceptMaps(dir);
        _cvRightToLeft?.SaveValueSetConceptMaps(dir);
    }

    private void compareAllValueSets()
    {
        // iterate over the pairs in both directions
        foreach (
            (DefinitionCollection left, DefinitionCollection right, CrossVersionMapCollection cvMap) in
            ((DefinitionCollection, DefinitionCollection, CrossVersionMapCollection)[])[(_leftDc, _rightDc, _cvLeftToRight!), (_rightDc, _leftDc, _cvRightToLeft!)])
        {
            // iterate over the value sets in the definition collection
            foreach ((string unversionedUrl, string[] versions) in left.ValueSetVersions.OrderBy(kvp => kvp.Key))
            {
                // only use the highest version in the package
                string vsVersion = versions.OrderDescending().First();
                string versionedUrl = unversionedUrl + "|" + vsVersion;

                // skip value sets we know we will not process
                if (_exclusionSet.Contains(unversionedUrl))
                {
                    if (left.ValueSetsByVersionedUrl.TryGetValue(versionedUrl, out ValueSet? unexpandedVs))
                    {
                        // flag that we are not comparing this because of manual exclusion
                        _valueSetComparisons.AddToValue(versionedUrl, new()
                        {
                            Source = unexpandedVs,
                            Target = null,
                            Relationship = null,
                            IssueCode = ComparisonIssueCode.ManuallyExcluded,
                            Message = "Not compared because this ValueSet is in the manual exclusion list.",
                            Map = null,
                        });
                    }

                    _logger.LogValueSetExcluded(versionedUrl);
                    continue;
                }

                // we can only process value sets we can expand
                if (!left.TryExpandVs(versionedUrl, out ValueSet? vs, out string? expandMessage))
                {
                    // get the unexpanded value set object
                    if (left.ValueSetsByVersionedUrl.TryGetValue(versionedUrl, out ValueSet? unexpandedVs))
                    {
                        // flag that we are not comparing this because we could not expand
                        _valueSetComparisons.AddToValue(versionedUrl, new()
                        {
                            Source = unexpandedVs,
                            Target = null,
                            Relationship = null,
                            IssueCode = ComparisonIssueCode.CannotExpandSource,
                            Message = $"Not compared because this ValueSet failed to expand: {expandMessage}.",
                            Map = null,
                        });
                    }

                    _logger.LogValueSetNotExpanded(versionedUrl, expandMessage);
                    continue;
                }

                // TODO(ginoc): We should add a flag to process all expandable value sets for use in mapping, but do not need right now

                // we only need to process value sets that have a required binding
                if (!hasRequiredBinding(left, versionedUrl, unversionedUrl))
                {
                    // flag that we are not comparing this because we could not expand
                    _valueSetComparisons.AddToValue(versionedUrl,  new()
                    {
                        Source = vs,
                        Target = null,
                        Relationship = null,
                        IssueCode = ComparisonIssueCode.NoRequiredBindings,
                        Message = "Not compared because this ValueSet has no discovered required bindings.",
                        Map = null,
                    });

                    _logger.LogValueSetNoRequiredBindings(versionedUrl);
                    continue;
                }

                // perform all comparisons against what we can find in the target
                compareValueSet(left, vs, versionedUrl, unversionedUrl, right, cvMap);
            }
        }

        // check maps against their inverses (do after all maps are processed in each collection)
        buildMissingInverseMaps(_cvLeftToRight!, _cvRightToLeft!);
    }

    private void buildMissingInverseMaps(CrossVersionMapCollection cvLeft, CrossVersionMapCollection cvRight)
    {
        Dictionary<(string sourceUrl, string targetUrl), ConceptMap> mapsLeft = cvLeft.GetValueSetMaps()
            .ToDictionary(cm => (cm.SourceScope is Canonical s ? s.Value : string.Empty, cm.TargetScope is Canonical t ? t.Value : string.Empty));

        Dictionary<(string sourceUrl, string targetUrl), ConceptMap> mapsRight = cvRight.GetValueSetMaps()
            .ToDictionary(cm => (cm.SourceScope is Canonical s ? s.Value : string.Empty, cm.TargetScope is Canonical t ? t.Value : string.Empty));

        // iterate over the concept maps in the left collection
        foreach (((string leftUrl, string rightUrl), ConceptMap cm) in mapsLeft)
        {
            // check to see if there is an inverse map
            if (mapsRight.ContainsKey((rightUrl, leftUrl)))
            {
                continue;
            }

            // resolve value sets
            if (_leftDc.TryExpandVs(leftUrl, out ValueSet? leftVs) &&
                _rightDc.TryExpandVs(rightUrl, out ValueSet? rightVs))
            {
                // build an inverse map in the right collection
                ConceptMap inverted = buildInverseMap(cm, leftVs, rightVs, cvRight);

                // add to the right map set
                mapsRight.Add((rightUrl, leftUrl), inverted);
            }
        }

        // iterate over the concept maps in the right collection
        foreach (((string rightUrl, string leftUrl), ConceptMap cm) in mapsRight)
        {
            // check to see if there is an inverse map
            if (mapsLeft.ContainsKey((leftUrl, rightUrl)))
            {
                continue;
            }

            // resolve value sets
            if (_leftDc.TryExpandVs(leftUrl, out ValueSet? leftVs) &&
                _rightDc.TryExpandVs(rightUrl, out ValueSet? rightVs))
            {
                // build an inverse map in the left collection
                ConceptMap inverted = buildInverseMap(cm, rightVs, leftVs, cvLeft);

                // add to the left map set
                mapsLeft.Add((leftUrl, rightUrl), inverted);
            }
        }
    }

    private ConceptMap buildInverseMap(
        ConceptMap existing,
        ValueSet existingSourceVs,
        ValueSet existingTargetVs,
        CrossVersionMapCollection targetCv)
    {
        // alias our value sets for sanity (invert from existing)
        ValueSet sourceVs = existingTargetVs;
        ValueSet targetVs = existingSourceVs;

        // build our initial concept map
        ConceptMap cm = targetCv.BuildBaseMap(sourceVs, targetVs);

        Dictionary<(string system, string code), Dictionary<(string system, string? code), ValueSetCodeComparisonRec>> exMapByTarget = [];

        // unroll the existing concept map
        foreach (ConceptMap.GroupComponent exGroup in existing.Group)
        {
            foreach (ConceptMap.SourceElementComponent exSourceElement in exGroup.Element)
            {
                // check for no-map
                if (exSourceElement.NoMap == true)
                {
                    // skip
                    continue;
                }

                // iterate over our concept map targets
                foreach (ConceptMap.TargetElementComponent exTargetElement in exSourceElement.Target)
                {
                    ValueSetCodeComparisonRec mapRec = new()
                    {
                        SourceSystem = exGroup.Source,
                        SourceCode = exSourceElement.Code,
                        SourceDisplay = exSourceElement.Display,
                        TargetSystem = exGroup.Target,
                        TargetCode = exTargetElement.Code,
                        TargetDisplay = exTargetElement.Display,
                        Relationship = exTargetElement.Relationship,
                        Comment = exTargetElement.Comment,
                        IsGenerated = mappingIsGenerated(exTargetElement),
                    };

                    // add to the target-based map
                    if (!exMapByTarget.TryGetValue((exGroup.Target, exTargetElement.Code), out Dictionary<(string system, string? code), ValueSetCodeComparisonRec>? mapRecsBySource))
                    {
                        mapRecsBySource = [];
                        exMapByTarget.Add((exGroup.Target, exTargetElement.Code), mapRecsBySource);
                    }

                    mapRecsBySource[(exGroup.Source, exSourceElement.Code)] = mapRec;
                }
            }
        }

        Dictionary<(string system, string code), Dictionary<(string system, string? code), ValueSetCodeComparisonRec>> mapBySource = [];

        // invert the existing records
        foreach(((string exTargetSystem, string exTargetCode), Dictionary<(string system, string? code), ValueSetCodeComparisonRec> exMapRecsBySource) in exMapByTarget)
        {
            if (!mapBySource.TryGetValue((exTargetSystem, exTargetCode), out Dictionary<(string system, string? code), ValueSetCodeComparisonRec>? mapRecsByTarget))
            {
                mapRecsByTarget = [];
                mapBySource.Add((exTargetSystem, exTargetCode), mapRecsByTarget);
            }

            // iterate over our records and invert them
            foreach (((string exSourceSystem, string? exSourceCode), ValueSetCodeComparisonRec exMapRec) in exMapRecsBySource)
            {
                ValueSetCodeComparisonRec mapRec = new()
                {
                    SourceSystem = exMapRec.TargetSystem!,
                    SourceCode = exMapRec.TargetCode!,
                    SourceDisplay = exMapRec.TargetDisplay,
                    TargetSystem = exMapRec.SourceSystem,
                    TargetCode = exMapRec.SourceCode,
                    TargetDisplay = exMapRec.SourceDisplay,
                    Relationship = invert(exMapRec.Relationship),
                    Comment = "Generated by inverting an opposite map",
                    IsGenerated = true,
                };

                mapRecsByTarget[(exMapRec.SourceSystem, exMapRec.SourceCode)] = mapRec;
            }
        }

        // group our map by source/target system pair
        IEnumerable<IGrouping<(string, string), ValueSetCodeComparisonRec>> results =
            from rec in mapBySource.SelectMany(kvp => kvp.Value.Values)
            group rec by (rec.SourceSystem, rec.TargetSystem);

        // rebuild the ConceptMap from our grouped records
        cm.Group.Clear();
        foreach (IGrouping<(string, string), ValueSetCodeComparisonRec> systemPairGroup in results)
        {
            (string sourceSystem, string targetSystem) = systemPairGroup.Key;
            string groupKey = sourceSystem + "-" + targetSystem;

            ConceptMap.GroupComponent cmGroup = new()
            {
                Source = sourceSystem,
                Target = targetSystem,
                Element = [],
            };

            IEnumerable<IGrouping<string, ValueSetCodeComparisonRec>> sourceCodeGroups = systemPairGroup.GroupBy(rec => rec.SourceCode);

            foreach (IGrouping<string, ValueSetCodeComparisonRec> sourceCodeGroup in sourceCodeGroups)
            {
                string sourceCode = sourceCodeGroup.Key;

                ValueSetCodeComparisonRec firstRec = sourceCodeGroup.First();

                ConceptMap.SourceElementComponent element = new()
                {
                    Code = firstRec.SourceCode,
                    Display = firstRec.SourceDisplay,
                };

                foreach (ValueSetCodeComparisonRec rec in sourceCodeGroup)
                {
                    // check for no map
                    if (rec.NoMap == true)
                    {
                        element.NoMap = rec.NoMap;
                        continue;
                    }

                    element.Target.Add(new()
                    {
                        Code = rec.TargetCode,
                        Display = rec.TargetDisplay,
                        Relationship = rec.Relationship,
                        Comment = rec.Comment,
                        Property = rec.IsGenerated == null
                            ? []
                            : [new() { Code = CommonDefinitions.ConceptMapPropertyGeneratedCode, Value = new FhirBoolean(rec.IsGenerated) }],
                    });
                }

                cmGroup.Element.Add(element);
            }

            cm.Group.Add(cmGroup);
        }

        // update our concept map aggregate relationships
        aggregateValueSetRelationships(cm);

        // ensure this map has our usage context
        setValueSetComparisonUseContext(cm);

        // ensure this map has our properties
        addConceptMapProperties(cm);

        return cm;
    }

    private CMR? invert(CMR? existing) => existing switch
    {
        CMR.RelatedTo => CMR.RelatedTo,
        CMR.Equivalent => CMR.Equivalent,
        CMR.SourceIsNarrowerThanTarget => CMR.SourceIsBroaderThanTarget,
        CMR.SourceIsBroaderThanTarget => CMR.SourceIsNarrowerThanTarget,
        CMR.NotRelatedTo => CMR.NotRelatedTo,
        _ => null,
    };

    private void compareValueSet(
        DefinitionCollection dc,
        ValueSet vs,
        string versionedUrl,
        string unversionedUrl,
        DefinitionCollection targetDc,
        CrossVersionMapCollection cv)
    {
        // get all the maps for this source (versioned URL will get versioned and unversioned)
        List<ConceptMap> maps = cv.GetMapsForSource(versionedUrl);

        // check for no maps
        if (maps.Count == 0)
        {
            // if we cannot find a matching VS in the target, we are done
            if (!targetDc.TryGetValueSet(unversionedUrl, out ValueSet? tVs))
            {
                // flag that we are not comparing this because we could not expand
                _valueSetComparisons.AddToValue(versionedUrl, new()
                {
                    Source = vs,
                    Target = null,
                    Relationship = null,
                    IssueCode = ComparisonIssueCode.NoTarget,
                    Message = "Not compared because this ValueSet has no maps and does not exist in the target.",
                    Map = null,
                });

                _logger.LogValueSetNoTarget(versionedUrl);
                return;
            }

            // add a shell map for this target
            maps.Add(cv.BuildBaseMap(vs, tVs));
        }

        // iterate over our maps
        foreach (ConceptMap cm in maps)
        {
            // check for invalid target in the map
            if ((cm.TargetScope == null) ||
                (cm.TargetScope is not Canonical targetCanonical))
            {
                // flag that we are not comparing this because we could not expand
                _valueSetComparisons.AddToValue(versionedUrl, new()
                {
                    Source = vs,
                    Target = null,
                    Relationship = null,
                    IssueCode = ComparisonIssueCode.InvalidMap,
                    Message = $"Not compared because map {cm.Id} ({cm.Url}) does not have a valid target.",
                    Map = cm,
                });

                _logger.LogValueSetInvalidTarget(versionedUrl, cm.Url);
                continue;
            }

            string targetVersioned = targetCanonical.Value;
            string targetUnversioned = targetVersioned.Split('|')[0];

            // we can only process value sets we can expand
            if (!targetDc.TryExpandVs(targetVersioned, out ValueSet? targetVs, out string? expandMessage))
            {
                // get the unexpanded value set object
                if (targetDc.ValueSetsByVersionedUrl.TryGetValue(targetVersioned, out ValueSet? unexpandedVs))
                {
                    // flag that we are not comparing this because we could not expand
                    _valueSetComparisons.AddToValue(versionedUrl, new()
                    {
                        Source = vs,
                        Target = unexpandedVs,
                        Relationship = null,
                        IssueCode = ComparisonIssueCode.CannotExpandTarget,
                        Message = $"Not compared because the target ValueSet failed to expand: {expandMessage}.",
                        Map = cm,
                    });
                }

                _logger.LogValueSetNotExpanded(versionedUrl, expandMessage);
                continue;
            }

            // ensure this map has our usage context
            setValueSetComparisonUseContext(cm);

            // ensure this map has our properties
            addConceptMapProperties(cm);

            // actually compare the two value sets, using the map
            compareValueSet(vs, targetVs, cm);
        }
    }

    private void setValueSetComparisonUseContext(ConceptMap cm)
    {
        if (cm.UseContext.Any(uc => uc.Code.System == CommonDefinitions.ConceptMapUsageContextSystem))
        {
            return;
        }

        cm.UseContext.Add(new()
        {
            Code = new(CommonDefinitions.ConceptMapUsageContextSystem, CommonDefinitions.ConceptMapUsageContextTarget),
            Value = new CodeableConcept(CommonDefinitions.ConceptMapUsageContextSystem, CommonDefinitions.ConceptMapUsageContextValueSet),
        });
    }

    private void addConceptMapProperties(ConceptMap cm)
    {
        if (!cm.Property.Any(p => p.Uri == CommonDefinitions.ConceptMapPropertyGeneratedUri))
        {
            cm.Property.Add(new()
            {
                Code = CommonDefinitions.ConceptMapPropertyGeneratedCode,
                Uri = CommonDefinitions.ConceptMapPropertyGeneratedUri,
                Description = "Generated by the FHIR Cross-Version Mapping Tool",
                Type = ConceptMap.ConceptMapPropertyType.Boolean,
            });
        }
    }

    private bool? mappingIsGenerated(ConceptMap.TargetElementComponent te)
    {
        ConceptMap.MappingPropertyComponent? mpc = te.Property.FirstOrDefault(p => p.Code == CommonDefinitions.ConceptMapPropertyGeneratedCode);
        return mpc?.Value is FhirBoolean fb ? fb.Value : null;
    }

    private void compareValueSet(
        ValueSet source,
        ValueSet target,
        ConceptMap cm)
    {
        Dictionary<string, Dictionary<string, ValueSet.ContainsComponent>> targetCodesDict = [];
        Dictionary<(string system, string code), Dictionary<(string system, string? code), ValueSetCodeComparisonRec>> mapBySource = [];
        Dictionary<(string system, string code), Dictionary<(string system, string? code), ValueSetCodeComparisonRec>> mapByTarget = [];

        // unroll the target system so we can lookup codes
        foreach (ValueSet.ContainsComponent tc in target.cgGetFlatContains())
        {
            // check for this code
            if (!targetCodesDict.TryGetValue(tc.Code, out Dictionary<string, ValueSet.ContainsComponent>? targetsBySystem))
            {
                targetsBySystem = [];
                targetCodesDict[tc.Code] = targetsBySystem;
            }

            // add this system
            targetsBySystem[tc.System] = tc;

            // add to our target map dictionary
            mapByTarget.Add((tc.System, tc.Code), []);
        }

        // unroll the existing concept map
        foreach (ConceptMap.GroupComponent cmGroup in cm.Group)
        {
            foreach (ConceptMap.SourceElementComponent cmSourceElement in cmGroup.Element)
            {
                if (!mapBySource.TryGetValue((cmGroup.Source, cmSourceElement.Code), out Dictionary<(string system, string? code), ValueSetCodeComparisonRec>? mapRecsByTarget))
                {
                    mapRecsByTarget = [];
                    mapBySource.Add((cmGroup.Source, cmSourceElement.Code), mapRecsByTarget);
                }

                // check for no-map
                if (cmSourceElement.NoMap == true)
                {
                    // add a no-map entry
                    mapRecsByTarget[(cmGroup.Target, null)] = new()
                    {
                        SourceSystem = cmGroup.Source,
                        SourceCode = cmSourceElement.Code,
                        SourceDisplay = cmSourceElement.Display,
                        NoMap = cmSourceElement.NoMap,
                        Relationship = null,
                        Comment = $"Concept is listed in {cm.Url} as not mapped.",
                        IsGenerated = false,
                    };
                }

                // iterate over our concept map targets
                foreach (ConceptMap.TargetElementComponent cmTargetElement in cmSourceElement.Target)
                {
                    ValueSetCodeComparisonRec mapRec = new()
                    {
                        SourceSystem = cmGroup.Source,
                        SourceCode = cmSourceElement.Code,
                        SourceDisplay = cmSourceElement.Display,
                        TargetSystem = cmGroup.Target,
                        TargetCode = cmTargetElement.Code,
                        TargetDisplay = cmTargetElement.Display,
                        Relationship = cmTargetElement.Relationship,
                        Comment = cmTargetElement.Comment,
                        IsGenerated = mappingIsGenerated(cmTargetElement),
                    };

                    // add to the source-based map
                    mapRecsByTarget[(cmGroup.Target, cmTargetElement.Code)] = mapRec;

                    // add to the target-based map
                    if (!mapByTarget.TryGetValue((cmGroup.Target, cmTargetElement.Code), out Dictionary<(string system, string? code), ValueSetCodeComparisonRec>? mapRecsBySource))
                    {
                        mapRecsBySource = [];
                        mapByTarget.Add((cmGroup.Target, cmTargetElement.Code), mapRecsBySource);
                    }

                    mapRecsBySource[(cmGroup.Source, cmSourceElement.Code)] = mapRec;
                }
            }
        }

        // iterate over the source value set to check and update the map
        foreach (ValueSet.ContainsComponent sourceConcept in source.cgGetFlatContains().ToArray())
        {
            // get or create a map dictionary for this value set system+code
            if (!mapBySource.TryGetValue((sourceConcept.System, sourceConcept.Code), out Dictionary<(string system, string? code), ValueSetCodeComparisonRec>? mapRecsByTarget))
            {
                mapRecsByTarget = [];
                mapBySource.Add((sourceConcept.System, sourceConcept.Code), mapRecsByTarget);
            }

            // if there are no known targets, see if we can find a match by literal
            if ((mapRecsByTarget.Count == 0) &&
                targetCodesDict.TryGetValue(sourceConcept.Code, out Dictionary<string, ValueSet.ContainsComponent>? targetsBySystem))
            {
                // check for the same system
                if (targetsBySystem.TryGetValue(sourceConcept.System, out ValueSet.ContainsComponent? matchedContains))
                {
                    ValueSetCodeComparisonRec mapRec = new()
                    {
                        SourceSystem = sourceConcept.System,
                        SourceCode = sourceConcept.Code,
                        SourceDisplay = sourceConcept.Display,
                        TargetSystem = matchedContains.System,
                        TargetCode = matchedContains.Code,
                        TargetDisplay = matchedContains.Display,
                        Relationship = CMR.Equivalent,
                        Comment = $"`{sourceConcept.Code}` does not have a map, but found a literal match of `{matchedContains.Code}` in code and system.",
                        IsGenerated = true,
                    };

                    // add to the source-based map
                    mapRecsByTarget.Add((matchedContains.System, matchedContains.Code), mapRec);

                    // add to the target-based map
                    if (!mapByTarget.TryGetValue((matchedContains.System, matchedContains.Code), out Dictionary<(string system, string? code), ValueSetCodeComparisonRec>? mapRecsBySource))
                    {
                        mapRecsBySource = [];
                        mapByTarget.Add((matchedContains.System, matchedContains.Code), mapRecsBySource);
                    }

                    mapRecsBySource[(sourceConcept.System, sourceConcept.Code)] = mapRec;
                }
                else
                {
                    // add a map for each matching target literal
                    foreach (ValueSet.ContainsComponent targetContains in targetsBySystem.Values)
                    {
                        ValueSetCodeComparisonRec mapRec = new()
                        {
                            SourceSystem = sourceConcept.System,
                            SourceCode = sourceConcept.Code,
                            SourceDisplay = sourceConcept.Display,
                            TargetSystem = targetContains.System,
                            TargetCode = targetContains.Code,
                            TargetDisplay = targetContains.Display,
                            Relationship = targetsBySystem.Count == 1 ? CMR.Equivalent : CMR.SourceIsBroaderThanTarget,
                            Comment = targetsBySystem.Count == 1
                                ? $"`{sourceConcept.Code}` does not have a map, but found a literal match of `{targetContains.Code}` with a different system."
                                : $"`{sourceConcept.Code}` does not have a map, but found literal matches of `{targetContains.Code}` in multiple systems.",
                            IsGenerated = true,
                        };

                        // add to the source-based map
                        mapRecsByTarget.Add((targetContains.System, targetContains.Code), mapRec);

                        // add to the target-based map
                        if (!mapByTarget.TryGetValue((targetContains.System, targetContains.Code), out Dictionary<(string system, string? code), ValueSetCodeComparisonRec>? mapRecsBySource))
                        {
                            mapRecsBySource = [];
                            mapByTarget.Add((targetContains.System, targetContains.Code), mapRecsBySource);
                        }

                        mapRecsBySource[(sourceConcept.System, sourceConcept.Code)] = mapRec;
                    }
                }
            }       // if: no known targets

            // iterate over our existing records to check content elements (e.g., display)
            foreach (ValueSetCodeComparisonRec rec in mapRecsByTarget.Values)
            {
                // check for no source display
                if (string.IsNullOrEmpty(rec.SourceDisplay))
                {
                    rec.SourceDisplay = sourceConcept.Display;
                }

                // check for having a target and not having a display
                if (string.IsNullOrEmpty(rec.TargetDisplay) &&
                    (rec.TargetCode != null) &&
                    (rec.TargetSystem != null) &&
                    targetCodesDict.TryGetValue(rec.TargetCode, out Dictionary<string, ValueSet.ContainsComponent>? matchingTargets) &&
                    matchingTargets.TryGetValue(rec.TargetSystem, out ValueSet.ContainsComponent? matchingContains))
                {
                    rec.TargetDisplay = matchingContains.Display;
                }
            }       // foreach: iterate over map targets to check contents
        }           // foreach: iterate over source valueset

        // traverse the source map to check relationships
        foreach (((string sourceSystem, string sourceCode), Dictionary<(string system, string? code), ValueSetCodeComparisonRec> mapRecsByTarget) in mapBySource)
        {
            // check for the source being an escape-valve code
            if (_escapeValveCodes.Contains(sourceCode))
            {
                // iterate over our map targets
                foreach (((string targetSystem, string? targetCode), ValueSetCodeComparisonRec rec) in mapRecsByTarget)
                {
                    // skip no-maps
                    if (targetCode == null)
                    {
                        continue;
                    }

                    // skip reviewed records
                    if (rec.IsGenerated == false)
                    {
                        continue;
                    }

                    // skip anything not equivalent
                    if (rec.Relationship == CMR.Equivalent)
                    {
                        continue;
                    }

                    // check for mapping to another escape-valve code
                    if (_escapeValveCodes.Contains(targetCode))
                    {
                        // check to see if the sides have a different number of concepts
                        if (mapBySource.Count != mapByTarget.Count)
                        {
                            // this should not be equivalent and should be reviewed
                            rec.Relationship = mapBySource.Count > mapByTarget.Count ? CMR.SourceIsNarrowerThanTarget : CMR.SourceIsBroaderThanTarget;
                            rec.IsGenerated = true;
                            rec.Comment = rec.Comment + $" Escape-valve code `{sourceCode}` maps to `{targetCode}`, but represent different concept domains (different number of codes).";
                        }
                    }
                }
            }

            // check for a single source with multiple targets and any that map as equivalent
            if ((mapRecsByTarget.Count > 1) &&
                mapRecsByTarget.Any(kvp => kvp.Value.Relationship == CMR.Equivalent))
            {
                foreach (((string tSystem, string? tCode), ValueSetCodeComparisonRec rec) in mapRecsByTarget)
                {
                    // skip any that have been reviewed
                    if (rec.IsGenerated == false)
                    {
                        continue;
                    }

                    // skip any that look correct
                    if (rec.Relationship != CMR.Equivalent)
                    {
                        continue;
                    }

                    // mark as not equivalent and flag for review
                    rec.Relationship = CMR.SourceIsBroaderThanTarget;
                    rec.IsGenerated = true;
                    rec.Comment = $" `{rec.SourceCode}` maps to multiple codes ({string.Join(", ", mapRecsByTarget.Values.Select(r => "`" + r.TargetCode + "`"))}) and cannot be equivalent.";
                }
            }
        }

        // traverse the target map to check relationships
        foreach (((string targetSystem, string targetCode), Dictionary<(string system, string? code), ValueSetCodeComparisonRec> mapRecsBySource) in mapByTarget)
        {
            // iterate over the map recs
            foreach (((string sourceSystem, string? sourceCode), ValueSetCodeComparisonRec rec) in mapRecsBySource)
            {
                // check for an unreviewed comparison that is equivalent from multiple sources
                if ((rec.Relationship == CMR.Equivalent) &&
                    (rec.IsGenerated == null) &&
                    (mapRecsBySource.Count > 1))
                {
                    // mark as not equivalent and flag for review
                    rec.Relationship = CMR.SourceIsNarrowerThanTarget;
                    rec.IsGenerated = true;
                    rec.Comment = $" {string.Join(", ", mapRecsBySource.Values.Select(r => "`" + r.SourceCode + "`"))} all map to `{rec.TargetCode}` and cannot be equivalent.";
                }
            }
        }

        // group our map by source/target system pair
        IEnumerable<IGrouping<(string, string), ValueSetCodeComparisonRec>> results =
            from rec in mapBySource.SelectMany(kvp => kvp.Value.Values)
            group rec by (rec.SourceSystem, rec.TargetSystem);

        // rebuild the ConceptMap from our grouped records
        cm.Group.Clear();
        foreach (IGrouping<(string, string), ValueSetCodeComparisonRec> systemPairGroup in results)
        {
            (string sourceSystem, string targetSystem) = systemPairGroup.Key;
            string groupKey = sourceSystem + "-" + targetSystem;

            ConceptMap.GroupComponent cmGroup = new()
            {
                Source = sourceSystem,
                Target = targetSystem,
                Element = [],
            };

            IEnumerable<IGrouping<string, ValueSetCodeComparisonRec>> sourceCodeGroups = systemPairGroup.GroupBy(rec => rec.SourceCode);

            foreach (IGrouping<string, ValueSetCodeComparisonRec> sourceCodeGroup in sourceCodeGroups)
            {
                string sourceCode = sourceCodeGroup.Key;

                ValueSetCodeComparisonRec firstRec = sourceCodeGroup.First();

                ConceptMap.SourceElementComponent element = new()
                {
                    Code = firstRec.SourceCode,
                    Display = firstRec.SourceDisplay,
                };

                foreach (ValueSetCodeComparisonRec rec in sourceCodeGroup)
                {
                    // check for no map
                    if (rec.NoMap == true)
                    {
                        element.NoMap = rec.NoMap;
                        continue;
                    }

                    element.Target.Add(new()
                    {
                        Code = rec.TargetCode,
                        Display = rec.TargetDisplay,
                        Relationship = rec.Relationship,
                        Comment = rec.Comment,
                        Property = rec.IsGenerated == null
                            ? []
                            : [ new() { Code = CommonDefinitions.ConceptMapPropertyGeneratedCode, Value = new FhirBoolean(rec.IsGenerated) } ],
                    });
                }

                cmGroup.Element.Add(element);
            }

            cm.Group.Add(cmGroup);
        }

        // update our concept map aggregate relationships
        aggregateValueSetRelationships(cm);
    }

    /// <summary>
    /// Aggregates the relationships of a given ConceptMap by iterating over its groups and elements.
    /// </summary>
    /// <param name="cm">The ConceptMap to aggregate relationships for.</param>
    /// <returns>The aggregated ConceptMapRelationship for the entire ConceptMap.</returns>
    /// <remarks>
    /// This method starts with an optimistic assumption that the relationship is equivalent.
    /// It iterates over each group and each element within the group to apply the relationships.
    /// The aggregated relationship is stored as an extension in both the group and the ConceptMap.
    /// </remarks>
    private CMR aggregateValueSetRelationships(ConceptMap cm)
    {
        // start optimistic
        CMR vsRelationship = CMR.Equivalent;

        // iterate over groups
        foreach (ConceptMap.GroupComponent group in cm.Group)
        {
            // start optimistic
            CMR groupRelationship = CMR.Equivalent;

            // iterate over the elements (individual concept maps)
            foreach (ConceptMap.SourceElementComponent sourceElement in group.Element)
            {
                // check for no map
                if (sourceElement.NoMap == true)
                {
                    // unmapped element means the group is broader than the target
                    groupRelationship = applyRelationship(groupRelationship, CMR.SourceIsBroaderThanTarget);
                }

                // iterate over the targets
                foreach (ConceptMap.TargetElementComponent targetElement in sourceElement.Target)
                {
                    // apply the current relationship
                    groupRelationship = applyRelationship(groupRelationship, targetElement.Relationship);
                }
            }

            // add an extension to the group to store the relationship
            group.SetExtension(CommonDefinitions.ExtUrlConceptMapAggregateRelationship, new Code<ConceptMap.ConceptMapRelationship>(groupRelationship));

            // apply the group relationship to the value set relationship
            vsRelationship = applyRelationship(vsRelationship, groupRelationship);
        }

        // add an extension to the concept map to store the relationship
        cm.SetExtension(CommonDefinitions.ExtUrlConceptMapAggregateRelationship, new Code<ConceptMap.ConceptMapRelationship>(vsRelationship));

        // return the relationship
        return vsRelationship;
    }

    private CMR applyRelationship(CMR? existing, CMR? change) => existing switch
    {
        CMR.Equivalent => change ?? CMR.Equivalent,
        CMR.RelatedTo => (change == CMR.NotRelatedTo) ? CMR.NotRelatedTo : CMR.RelatedTo,
        CMR.SourceIsNarrowerThanTarget => (change == CMR.SourceIsNarrowerThanTarget || change == CMR.Equivalent)
            ? CMR.SourceIsNarrowerThanTarget : CMR.RelatedTo,
        CMR.SourceIsBroaderThanTarget => (change == CMR.SourceIsBroaderThanTarget || change == CMR.Equivalent)
            ? CMR.SourceIsBroaderThanTarget : CMR.RelatedTo,
        CMR.NotRelatedTo => change ?? CMR.NotRelatedTo,
        _ => change ?? existing ?? CMR.NotRelatedTo,
    };



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
    /// Gets the initial cross-version maps between two definition collections.
    /// </summary>
    /// <returns>A collection of cross-version maps.</returns>
    private (CrossVersionMapCollection lToR, CrossVersionMapCollection rToL) getInitialMaps()
    {
        CrossVersionMapCollection lToR = new(_leftDc, _rightDc);
        CrossVersionMapCollection rToL = new(_rightDc, _leftDc);

        // check for creating new maps
        if (string.IsNullOrEmpty(_config.CrossVersionMapSourcePath))
        {
            return (lToR, rToL);
        }

        if (!lToR.TryLoadCrossVersionMaps(_config.CrossVersionMapSourcePath))
        {
            _logger.LogMapsNotLoaded(_leftKey, _rightKey);
        }

        if (!rToL.TryLoadCrossVersionMaps(_config.CrossVersionMapSourcePath))
        {
            _logger.LogMapsNotLoaded(_rightKey, _leftKey);
        }

        return (lToR, rToL);
    }

}
