// <copyright file="FhirCoreComparerValueSets.cs" company="Microsoft Corporation">
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
using System.Collections;
using static Microsoft.Health.Fhir.CodeGen.CompareTool.FhirCoreComparerLogMessages;


#if NETSTANDARD2_0
using Microsoft.Health.Fhir.CodeGenCommon.Polyfill;
#endif


namespace Microsoft.Health.Fhir.CodeGen.CompareTool;

public partial class FhirCoreComparer
{
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

    private HashSet<string>? _leftValueSetUrls = null;
    private HashSet<string>? _rightValueSetUrls = null;

    public void RegisterValueSetFilters(HashSet<string> leftValueSetUrls, HashSet<string> rightValueSetUrls)
    {
        _leftValueSetUrls = leftValueSetUrls;
        _rightValueSetUrls = rightValueSetUrls;
    }

    public IEnumerable<(ValueSet left, ValueSet right, ConceptMap? up, ConceptMap? down)> GetPairedValueSetMaps()
    {
        Dictionary<(string? source, string? target), ConceptMap> mapsUp =
            (_cvLeftToRight?.GetValueSetMaps() ?? []).ToDictionary(cm => (cm.cgSourceScope(), cm.cgTargetScope()));
        Dictionary<(string? source, string? target), ConceptMap> mapsDown =
            (_cvRightToLeft?.GetValueSetMaps() ?? []).ToDictionary(cm => (cm.cgSourceScope(), cm.cgTargetScope()));

        // iterate over the forward maps (up)
        foreach (((string? source, string? target), ConceptMap cmUp) in mapsUp)
        {
            mapsDown.TryGetValue((target, source), out ConceptMap? cmDown);

            ValueSet? leftVs = null;
            ValueSet? rightVs = null;

            if ((source == null) ||
                (!_leftDc.TryExpandVs(source, out leftVs) && !_leftDc.TryGetValueSet(source, out leftVs)))
            {
                continue;
            }

            if ((target == null) ||
                (!_rightDc.TryExpandVs(target, out rightVs) && !_rightDc.TryGetValueSet(target, out rightVs)))
            {
                continue;
            }

            yield return (leftVs, rightVs, cmUp, cmDown);
        }

        // iterate over the reverse maps looking for orphans
        foreach (((string? source, string? target), ConceptMap cmDown) in mapsDown)
        {
            if (mapsUp.ContainsKey((target, source)))
            {
                continue;
            }

            ValueSet? leftVs = null;
            ValueSet? rightVs = null;

            if ((source == null) ||
                (!_rightDc.TryExpandVs(source, out rightVs) && !_rightDc.TryGetValueSet(source, out rightVs)))
            {
                continue;
            }

            if ((target == null) ||
                (!_leftDc.TryExpandVs(target, out leftVs) && !_leftDc.TryGetValueSet(target, out leftVs)))
            {
                continue;
            }

            yield return (leftVs, rightVs, null, cmDown);
        }
    }

    private void compareAllValueSets()
    {
        // iterate over the pairs in both directions
        foreach (
            (DefinitionCollection left, DefinitionCollection right, CrossVersionMapCollection cvMap, HashSet<string>? vsFilter) in
            ((DefinitionCollection, DefinitionCollection, CrossVersionMapCollection, HashSet<string>?)[])[
                (_leftDc, _rightDc, _cvLeftToRight!, _leftValueSetUrls),
                (_rightDc, _leftDc, _cvRightToLeft!, _rightValueSetUrls)
                ])
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
                        // flag why we are not including this
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

                // check for precomputed filter
                if (vsFilter != null)
                {
                    if (!vsFilter.Contains(versionedUrl) &&
                        !vsFilter.Contains(unversionedUrl))
                    {
                        continue;
                    }
                }
                // we only need to process value sets that have a required binding
                else if (!left.cgHasRequiredBinding(versionedUrl, unversionedUrl))
                {
                    // flag why we are not including this
                    _valueSetComparisons.AddToValue(versionedUrl, new()
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
        buildMissingInverseMapsForValueSets(_cvLeftToRight!, _cvRightToLeft!);
    }

    private void buildMissingInverseMapsForValueSets(CrossVersionMapCollection cvLeft, CrossVersionMapCollection cvRight)
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
                        IsGenerated = exTargetElement.cgIsGenerated(),
                        NeedsReview = exTargetElement.cgNeedsReview(),
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
        foreach (((string exTargetSystem, string exTargetCode), Dictionary<(string system, string? code), ValueSetCodeComparisonRec> exMapRecsBySource) in exMapByTarget)
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
                    NeedsReview = true,
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
                        Property = getMappingProperties(rec),
                    });
                }

                cmGroup.Element.Add(element);
            }

            cm.Group.Add(cmGroup);
        }

        // update our concept map aggregate relationships
        aggregateValueSetRelationships(cm);

        // ensure this map has our usage context
        setUseContext(cm, CommonDefinitions.ConceptMapUsageContextValueSet);

        // ensure this map has our properties
        addConceptMapPropertyDefinitions(cm);

        return cm;
    }


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

                _logger.LogNoTarget(versionedUrl);
                return;
            }

            // add a shell map for this target
            maps.Add(cv.BuildBaseMap(vs, tVs));
        }

        // iterate over our maps
        foreach (ConceptMap cm in maps)
        {
            // ensure this map has our usage context
            setUseContext(cm, CommonDefinitions.ConceptMapUsageContextValueSet);

            // ensure this map has our properties
            addConceptMapPropertyDefinitions(cm);

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

                _logger.LogInvalidMapTarget(versionedUrl, cm.Url);
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

            // actually compare the two value sets, using the map
            compareValueSet(vs, targetVs, cm);
        }
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
            if (!mapByTarget.ContainsKey((tc.System, tc.Code)))
            {
                mapByTarget.Add((tc.System, tc.Code), []);
            }
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
                        NeedsReview = false,
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
                        IsGenerated = cmTargetElement.cgIsGenerated(),
                        NeedsReview = cmTargetElement.cgNeedsReview(),
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
                        NeedsReview = true,
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
                            NeedsReview = true,
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
                            : [new() { Code = CommonDefinitions.ConceptMapPropertyGenerated, Value = new FhirBoolean(rec.IsGenerated) }],
                    });
                }

                cmGroup.Element.Add(element);
            }

            cm.Group.Add(cmGroup);
        }

        // update our concept map aggregate relationships
        aggregateValueSetRelationships(cm);
    }

}
