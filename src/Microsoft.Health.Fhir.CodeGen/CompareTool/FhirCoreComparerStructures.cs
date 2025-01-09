// <copyright file="FhirCoreComparerStructures.cs" company="Microsoft Corporation">
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
using Microsoft.Health.Fhir.CodeGenCommon.Models;
using Newtonsoft.Json.Linq;
using System.Diagnostics.CodeAnalysis;



#if NETSTANDARD2_0
using Microsoft.Health.Fhir.CodeGenCommon.Polyfill;
#endif


namespace Microsoft.Health.Fhir.CodeGen.CompareTool;

public partial class FhirCoreComparer
{
    /// <summary>
    /// Gets the overview type maps between two definition collections (bi-directional).
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public (ConceptMap up, ConceptMap down) GetStructureOverviewMaps(FhirArtifactClassEnum artifactType)
    {
        if ((_cvLeftToRight == null) || (_cvRightToLeft == null))
        {
            // nothing to check if we do not have maps
            throw new InvalidOperationException("Cannot get overview maps without cross-version maps.");
        }

        // make any maps that are missing
        ConceptMap up;
        ConceptMap down;

        switch (artifactType)
        {
            case FhirArtifactClassEnum.PrimitiveType:
            case FhirArtifactClassEnum.ComplexType:
                up = _cvLeftToRight.DataTypeMap ?? _cvLeftToRight.BuildBaseTypeMap();
                down = _cvRightToLeft.DataTypeMap ?? _cvRightToLeft.BuildBaseTypeMap();

                // ensure this map has our usage context
                setUseContext(up, CommonDefinitions.ConceptMapUsageContextTypeOverview);
                setUseContext(down, CommonDefinitions.ConceptMapUsageContextTypeOverview);
                break;

            case FhirArtifactClassEnum.Resource:
                up = _cvLeftToRight.ResourceTypeMap ?? _cvLeftToRight.BuildBaseResourceMap();
                down = _cvRightToLeft.ResourceTypeMap ?? _cvRightToLeft.BuildBaseResourceMap();

                // ensure this map has our usage context
                setUseContext(up, CommonDefinitions.ConceptMapUsageContextResourceOverview);
                setUseContext(down, CommonDefinitions.ConceptMapUsageContextResourceOverview);
                break;

            default:
                throw new InvalidOperationException($"Cannot get overview maps for this artifact type: {artifactType}.");
        }

        // ensure these maps have our properties
        addConceptMapPropertyDefinitions(up);
        addConceptMapPropertyDefinitions(down);

        return (up, down);
    }


    public ConceptMap GetOrCreateElementMap(
        DefinitionCollection sourceDc,
        StructureDefinition sourceSd,
        DefinitionCollection targetDc,
        StructureDefinition targetSd,
        FhirArtifactClassEnum artifactClass)
    {
        CrossVersionMapCollection? cv = null;

        // check for a left-to-right mapping
        if ((_leftDc == sourceDc) && (_rightDc == targetDc))
        {
            cv = _cvLeftToRight;
        }
        //check for a right-to-left mapping
        else if ((_rightDc == sourceDc) && (_leftDc == targetDc))
        {
            cv = _cvRightToLeft;
        }

        if (cv == null)
        {
            // not a mapping we have
            throw new Exception($"Invalid collections: source {sourceDc.FhirVersionLiteral} and target {targetDc.FhirVersionLiteral} are not in this comparison");
        }

        // check for an existing map
        ConceptMap? cm = cv.GetMap(sourceSd.Url, targetSd.Url);

        if (cm != null)
        {
            return cm;
        }

        // build a new map
        cm = cv.BuildBaseElementMap(sourceSd, targetSd);

        // set the correct usage context
        setUseContext(
            cm,
            artifactClass == FhirArtifactClassEnum.ComplexType
            ? CommonDefinitions.ConceptMapUsageContextDataType
            : CommonDefinitions.ConceptMapUsageContextResource);
        addConceptMapPropertyDefinitions(cm, includeDomainProps: true);

        return cm;
    }

    private void checkPrimitiveOverviewMaps()
    {
        if ((_cvLeftToRight == null) || (_cvRightToLeft == null))
        {
            // nothing to check if we do not have maps
            return;
        }

        // make any maps that are missing
        (ConceptMap up, ConceptMap down) = GetStructureOverviewMaps(FhirArtifactClassEnum.PrimitiveType);

        // ensure these maps have our properties
        addConceptMapPropertyDefinitions(up, includeDomainProps: true);
        addConceptMapPropertyDefinitions(down, includeDomainProps: true);

        // perform the checks in both directions
        foreach (
            (ConceptMap map,
                IReadOnlyDictionary<string, StructureDefinition> sourceTypes,
                IReadOnlyDictionary<string, StructureDefinition> targetTypes,
                ILookup<string, FhirTypeMappings.CodeGenTypeMapping> mappings)
            in getMapAndPrimitiveMappings())
        {
            HashSet<string> existingSources = new(map.Group.SelectMany(g => g.Element).Select(e => e.Code));

            foreach (string sourceType in sourceTypes.Keys)
            {
                if (existingSources.Contains(sourceType))
                {
                    continue;
                }

                // add an entry
                ConceptMap.SourceElementComponent sourceElement = new()
                {
                    Code = sourceType,
                };
                map.Group[0].Element.Add(sourceElement);
            }

            // iterate over the sources in the map
            foreach (ConceptMap.SourceElementComponent source in map.Group.SelectMany(g => g.Element))
            {
                // skip non-primitives (map contains complex types)
                if (!sourceTypes.TryGetValue(source.Code, out StructureDefinition? sourceSd))
                {
                    // this should not be possible
                    continue;
                }

                // build a lookup of the targets for this source
                ILookup<string, FhirTypeMappings.CodeGenTypeMapping> targetMappings = mappings[source.Code].ToLookup(m => m.TargetType);

                // track mapped targets so we can add ones we are missing
                HashSet<string> mappedTargetTypes = [];

                // iterate over the targets in the map
                foreach (ConceptMap.TargetElementComponent target in source.Target)
                {
                    // check to see if the target mapping exists
                    if (!targetMappings.Contains(target.Code))
                    {
                        // this should not be possible
                        throw new Exception($"{map.Name} has an unknown mapping for {source.Code} to {target.Code}!");
                    }

                    if (!targetTypes.TryGetValue(target.Code, out StructureDefinition? targetSd))
                    {
                        // this should not be possible
                        throw new Exception($"{map.Name} has an unknown target type {target.Code}!");
                    }

                    // this is a type we have covered
                    mappedTargetTypes.Add(target.Code);

                    FhirTypeMappings.CodeGenTypeMapping mapping = targetMappings[target.Code].First();

                    // add the properties for this mapping
                    addPrimitiveMapProps(target, mapping, false);

                    // update target properties
                    target.Display = targetSd.cgpDefinition();
                    target.Relationship = mapping.Relationship;

                    // check to see if there is a comment
                    if (string.IsNullOrEmpty(target.Comment))
                    {
                        target.Comment = mapping.Comment;
                    }
                }

                // add any missing targets
                foreach (FhirTypeMappings.CodeGenTypeMapping mapping in targetMappings.SelectMany(g => g))
                {
                    if (!targetTypes.TryGetValue(mapping.TargetType, out StructureDefinition? targetSd))
                    {
                        // this should not be possible
                        throw new Exception($"{map.Name} has an unknown target type {mapping.TargetType}!");
                    }

                    if (!mappedTargetTypes.Contains(mapping.TargetType))
                    {
                        ConceptMap.TargetElementComponent target = new()
                        {
                            Code = mapping.TargetType,
                            Display = targetSd.cgpDefinition(),
                            Relationship = mapping.Relationship,
                            Comment = mapping.Comment,
                        };
                        addPrimitiveMapProps(target, mapping, true);
                        source.Target.Add(target);
                    }
                }
            }
        }

        return;

        void addPrimitiveMapProps(ConceptMap.TargetElementComponent target, FhirTypeMappings.CodeGenTypeMapping mapping, bool isGenerated)
        {
            if (!target.Property.Any(p => p.Code == CommonDefinitions.ConceptMapPropertyConceptDomainRelationship))
            {
                target.Property.Add(new()
                {
                    Code = CommonDefinitions.ConceptMapPropertyConceptDomainRelationship,
                    Value = new Code<ConceptMap.ConceptMapRelationship>(mapping.ConceptDomainRelationship),
                });
            }

            if (!target.Property.Any(p => p.Code == CommonDefinitions.ConceptMapPropertyValueDomainRelationship))
            {
                target.Property.Add(new()
                {
                    Code = CommonDefinitions.ConceptMapPropertyValueDomainRelationship,
                    Value = new Code<ConceptMap.ConceptMapRelationship>(mapping.ValueDomainRelationship),
                });
            }

            if (isGenerated && !target.Property.Any(p => p.Code == CommonDefinitions.ConceptMapPropertyGenerated))
            {
                target.Property.Add(new()
                {
                    Code = CommonDefinitions.ConceptMapPropertyGenerated,
                    Value = new FhirBoolean(true),
                });
            }
        }

        (ConceptMap map, IReadOnlyDictionary<string, StructureDefinition> sourceTypes, IReadOnlyDictionary<string, StructureDefinition> targetTypes, ILookup<string, FhirTypeMappings.CodeGenTypeMapping> mappings)[] getMapAndPrimitiveMappings () => [
            (up, _leftDc.PrimitiveTypesByName, _rightDc.PrimitiveTypesByName, FhirTypeMappings.PrimitiveMappings
                .Where(pm => _leftDc.PrimitiveTypesByName.ContainsKey(pm.SourceType) && _rightDc.PrimitiveTypesByName.ContainsKey(pm.TargetType))
                .ToLookup(pm => pm.SourceType)),
            (down, _rightDc.PrimitiveTypesByName, _leftDc.PrimitiveTypesByName, FhirTypeMappings.PrimitiveMappings
                .Where(pm => _rightDc.PrimitiveTypesByName.ContainsKey(pm.SourceType) && _leftDc.PrimitiveTypesByName.ContainsKey(pm.TargetType))
                .ToLookup(pm => pm.SourceType))
            ];
    }

    /// <summary>
    /// Checks the overview maps for the specified artifact type.
    /// Ensures that all types exist in the map and adds no-map entries if necessary.
    /// Also traverses the maps looking for reversible inversions that don't exist and adds them.
    /// </summary>
    /// <param name="artifactType">The type of the artifact to check.</param>
    private void checkOverviewMaps(FhirArtifactClassEnum artifactType)
    {
        if ((_cvLeftToRight == null) || (_cvRightToLeft == null))
        {
            // nothing to check if we do not have maps
            return;
        }

        if (artifactType == FhirArtifactClassEnum.PrimitiveType)
        {
            throw new Exception("Use checkPrimitiveOverviewMaps!");
        }

        // make any maps that are missing
        (ConceptMap up, ConceptMap down) = GetStructureOverviewMaps(artifactType);

        // ensure these maps have our properties
        addConceptMapPropertyDefinitions(up);
        addConceptMapPropertyDefinitions(down);

        // check for the overview links in the correct sources
        foreach ((ConceptMap map, IEnumerable<StructureDefinition> types, DefinitionCollection sourceDc, DefinitionCollection targetDc) in getMapAndTypesArray())
        {
            // create a source lookup
            ILookup<string, ConceptMap.SourceElementComponent> sources = map.Group[0].Element.ToLookup(e => e.Code);

            // check to see if all the types exist in the map
            foreach (StructureDefinition sd in types)
            {
                if (!sources.Contains(sd.Name))
                {
                    // check to see if the same structure name exists in the target
                    if (targetDc.TryGetStructure(sd.Name, out _))
                    {
                        // add a map
                        ConceptMap.SourceElementComponent sourceElement = new()
                        {
                            Code = sd.Name,
                            Target = [
                                new()
                                {
                                    Code = sd.Name,
                                    Display = sd.cgDefinition(),
                                    Relationship = ConceptMap.ConceptMapRelationship.Equivalent,
                                    Property = getMappingProperties(generated: true, needsReview: true),
                                }],
                        };

                        map.Group[0].Element.Add(sourceElement);
                    }
                    else
                    {
                        // add a no-map entry
                        ConceptMap.SourceElementComponent sourceElement = new()
                        {
                            Code = sd.Name,
                            NoMap = true,
                        };
                        map.Group[0].Element.Add(sourceElement);
                    }
                }
            }
        }

        // traverse the map to ensure all the listed sources and targets exist
        foreach ((ConceptMap map, DefinitionCollection sourceDc, DefinitionCollection targetDc) in
            ((ConceptMap, DefinitionCollection, DefinitionCollection)[])[(up, _leftDc, _rightDc), (down, _rightDc, _leftDc)])
        {
            ConceptMap.SourceElementComponent? sourceBackboneElement = null;
            List<ConceptMap.SourceElementComponent> backboneSources = [];

            // iterate over all the sources in the map
            foreach (ConceptMap.SourceElementComponent mapSourceElement in map.Group[0].Element)
            {
                // grab our backbone element while iterating for later use
                if (mapSourceElement.Code == "BackboneElement")
                {
                    sourceBackboneElement = mapSourceElement;
                }

                if (sourceDc.TryGetStructure(mapSourceElement.Code, out _))
                {
                    // structure exists, we are good
                }
                // check for incorrectly flagged Backbone <-> type mappings
                else if ((sourceDc.FhirSequence == FhirReleases.FhirSequenceCodes.STU3) &&
                    (mapSourceElement.Code == "Expression"))
                {
                    backboneSources.Add(mapSourceElement);
                }
                else
                {
                    // this should not be possible
                    throw new Exception($"Cannot find target type {mapSourceElement.Code} in the source structures ({sourceDc.FhirVersionLiteral})!");
                }

                // iterate over all the targets for this source
                foreach (ConceptMap.TargetElementComponent mapTargetElement in mapSourceElement.Target)
                {
                    // ensure we have a matching type
                    if (targetDc.TryGetStructure(mapTargetElement.Code, out _))
                    {
                        // structure exists, we are good
                    }
                    // check for incorrectly flagged Backbone <-> type mappings
                    else if ((targetDc.FhirSequence == FhirReleases.FhirSequenceCodes.STU3) &&
                        (mapTargetElement.Code == "Expression"))
                    {
                        // targets can be fixed inline since duplication is not an issue
                        mapTargetElement.Code = "BackboneElement";
                        mapTargetElement.Comment = "In R4 the Metadata Type Expression was added - the STU3 equivalent data was represented as elements within a BackboneElement.";
                        mapTargetElement.Relationship = CMR.RelatedTo;
                        mapTargetElement.Property = getMappingProperties(needsReview: true, conceptRelationship: CMR.Equivalent, valueRelationship: CMR.RelatedTo);
                    }
                    else
                    {
                        // this should not be possible
                        throw new Exception($"Cannot find target type {mapTargetElement.Code} in the target types (mapped from source type {mapSourceElement.Code})!");
                    }
                }
            }

            if (sourceBackboneElement == null)
            {
                // add a no-map entry for the BackboneElement
                sourceBackboneElement = new()
                {
                    Code = "BackboneElement",
                    NoMap = true,
                };
                map.Group[0].Element.Add(sourceBackboneElement);
            }

            // fix elements that should map *from* BackboneElements
            foreach (ConceptMap.SourceElementComponent incorrectSource in backboneSources)
            {
                // if our backbone element was no-map, it is not any more
                if (sourceBackboneElement.NoMap == true)
                {
                    sourceBackboneElement.NoMap = false;
                    sourceBackboneElement.Target = [];
                }

                // copy the targets from our incorrect source
                foreach (ConceptMap.TargetElementComponent incorrectTarget in incorrectSource.Target)
                {
                    ConceptMap.TargetElementComponent te = (ConceptMap.TargetElementComponent)incorrectTarget.DeepCopy();
                    te.Comment = "In R4 the Metadata Type Expression was added - the STU3 equivalent data was represented as elements within a BackboneElement.";
                    te.Relationship = CMR.RelatedTo;
                    te.Property = getMappingProperties(needsReview: true, conceptRelationship: CMR.Equivalent, valueRelationship: CMR.RelatedTo);
                    sourceBackboneElement.Target.Add(te);
                }

                // remove the incorrect source element
                map.Group[0].Element.Remove(incorrectSource);
            }
        }

        // traverse the maps looking for reversible inversions that don't exist
        foreach ((ConceptMap map, ConceptMap inverseMap) in ((ConceptMap, ConceptMap)[])[(up, down), (down, up)])
        {
            ILookup<string, (ConceptMap.SourceElementComponent, ConceptMap.TargetElementComponent)> inverseTargets = inverseMap.Group[0].Element
                .SelectMany(se => se.Target.Select(te => (te.Code, (se, te))))
                .ToLookup(kvp => kvp.Code, kvp => kvp.Item2);

            // iterate over the souces in the forward map to check the source type
            foreach (ConceptMap.SourceElementComponent forwardSource in map.Group[0].Element)
            {
                // iterate over the inverse targets that match this type
                foreach ((ConceptMap.SourceElementComponent inverseSource, ConceptMap.TargetElementComponent inverseTarget) in inverseTargets[forwardSource.Code])
                {
                    // check to see if the forward map has the inverse source
                    if (forwardSource.Target.Any(sourceTarget => sourceTarget.Code == inverseSource.Code))
                    {
                        // we have a match
                        continue;
                    }

                    // add the inverse source to the forward map
                    forwardSource.Target.Add(new()
                    {
                        Code = inverseSource.Code,
                        Display = inverseSource.Display,
                        Relationship = invert(inverseTarget.Relationship),
                        Comment = "Generated by inverting an opposite map",
                        Property = getInvertedProperties(),
                    });

                    if (forwardSource.NoMap == true)
                    {
                        forwardSource.NoMap = false;
                    }
                }
            }
        }

        return;

        (ConceptMap, IEnumerable<StructureDefinition>, DefinitionCollection, DefinitionCollection)[] getMapAndTypesArray() => artifactType switch
        {
            FhirArtifactClassEnum.PrimitiveType => [(up, _leftDc.PrimitiveTypesByName.Values, _leftDc, _rightDc), (down, _rightDc.PrimitiveTypesByName.Values, _rightDc, _leftDc)],
            FhirArtifactClassEnum.ComplexType => [(up, _leftDc.ComplexTypesByName.Values, _leftDc, _rightDc), (down, _rightDc.ComplexTypesByName.Values, _rightDc, _leftDc)],
            FhirArtifactClassEnum.Resource => [(up, _leftDc.ResourcesByName.Values, _leftDc, _rightDc), (down, _rightDc.ResourcesByName.Values, _rightDc, _leftDc)],
            _ => [],
        };
    }

    private void checkElementMaps(FhirArtifactClassEnum artifactType)
    {
        if ((_cvLeftToRight == null) || (_cvRightToLeft == null))
        {
            // nothing to check if we do not have maps
            return;
        }

        if (artifactType == FhirArtifactClassEnum.PrimitiveType)
        {
            // primitives do not have element mappings
            return;
        }

        // get the overview maps (already updated) as the basis for doing element comparisons
        (ConceptMap up, ConceptMap down) = GetStructureOverviewMaps(artifactType);

        // check for the overview links in the correct sources
        foreach ((ConceptMap overviewMap, CrossVersionMapCollection cv, DefinitionCollection sourceDc, DefinitionCollection targetDc) in getMapAndTypesArray())
        {
            // iterate over the mapped types
            foreach (ConceptMap.SourceElementComponent mapSourceElement in overviewMap.Group[0].Element)
            {
                // no-maps have nothing to do
                if (mapSourceElement.NoMap == true)
                {
                    continue;
                }

                // ensure we have a matching type
                if (!sourceDc.TryGetStructure(mapSourceElement.Code, out StructureDefinition? sourceSd))
                {
                    // this should not be possible
                    throw new Exception($"Cannot find source structure {mapSourceElement.Code} in {sourceDc.FhirVersionLiteral}!");
                }

                // skip primitive sources
                if (sourceDc.PrimitiveTypesByName.ContainsKey(mapSourceElement.Code))
                {
                    continue;
                }

                // iterate over the mapped targets
                foreach (ConceptMap.TargetElementComponent mapTargetElement in mapSourceElement.Target)
                {
                    // ensure we have a matching type
                    if (!targetDc.TryGetStructure(mapTargetElement.Code, out StructureDefinition? targetSd))
                    {
                        // this should not be possible
                        throw new Exception($"Cannot find target structure {mapTargetElement.Code} in {targetDc.FhirVersionLiteral} (mapped from source type {mapSourceElement.Code})!");
                    }

                    ConceptMap elementMap = GetOrCreateElementMap(sourceDc, sourceSd, targetDc, targetSd, artifactType);

                    // process all the elements for this source to this target
                    checkStructureElements(elementMap, cv, sourceDc, sourceSd, targetDc, targetSd);
                }
            }

        }

        return;

        (ConceptMap, CrossVersionMapCollection, DefinitionCollection, DefinitionCollection)[] getMapAndTypesArray() => artifactType switch
        {
            FhirArtifactClassEnum.ComplexType => [
                (up, _cvLeftToRight, _leftDc, _rightDc),
                (down, _cvRightToLeft, _rightDc, _leftDc)
                ],
            FhirArtifactClassEnum.Resource => [
                (up, _cvLeftToRight, _leftDc, _rightDc),
                (down, _cvRightToLeft, _rightDc, _leftDc)
                ],
            _ => [],
        };
    }

    private record class ElementMapTrackingRec
    {
        public required ElementDefinition SourceElement { get; set; }
        public required ConceptMap.SourceElementComponent SourceMapElement { get; set; }

        public required ElementDefinition? TargetElement { get; set; }
        public required ConceptMap.TargetElementComponent? TargetMapElement { get; set; }
    }

    private void checkStructureElements(
        ConceptMap cm,
        CrossVersionMapCollection cv,
        DefinitionCollection sourceDc,
        StructureDefinition sourceSd,
        DefinitionCollection targetDc,
        StructureDefinition targetSd)
    {
        string? idPrefix = sourceSd.Name == targetSd.Name ? null : targetSd.Name;

        // build lookups of elements based on id for the source and target structures
        ILookup<string, ElementDefinition> sourceElements = sourceSd.cgElements(includeRoot: false).ToLookup(e => e.ElementId);
        ILookup<string, ElementDefinition> targetElements = targetSd.cgElements(includeRoot: false).ToLookup(e => e.ElementId);

        // build the dictionary to represent our mappings in a processible way
        Dictionary<string, List<ElementMapTrackingRec>> mappings = [];

        // load the current map into our mapping dictionary
        foreach (ConceptMap.SourceElementComponent mapSource in cm.Group[0].Element)
        {
            ElementDefinition? sourceElement = sourceElements[mapSource.Code].FirstOrDefault();

            if (sourceElement == null)
            {
                // this should not be possible
                throw new Exception($"Cannot find source element {mapSource.Code} in {sourceSd.Name}!");
            }

            // no-maps have a basic element
            if (mapSource.NoMap == true)
            {
                mappings.AddToValue(
                    mapSource.Code,
                    new()
                    {
                        SourceElement = sourceElement,
                        SourceMapElement = mapSource,
                        TargetElement = null,
                        TargetMapElement = null,
                    });

                continue;
            }

            // iterate over the targets for this source
            foreach (ConceptMap.TargetElementComponent mapTarget in mapSource.Target)
            {
                ElementDefinition? targetElement = targetElements[mapTarget.Code].FirstOrDefault();

                // ensure we have a matching target element
                if (targetElement == null)
                {
                    // this should not be possible
                    throw new Exception($"Cannot find target element {mapTarget.Code} in {targetSd.Name} (mapped from source element {mapSource.Code})!");
                }

                // add the mapping to the dictionary
                mappings.AddToValue(
                    mapSource.Code,
                    new()
                    {
                        SourceElement = sourceElement,
                        SourceMapElement = mapSource,
                        TargetElement = targetElement,
                        TargetMapElement = mapTarget,
                    });
            }
        }

        // iterate over source elements to look for elements which have not been added
        foreach (ElementDefinition sourceElement in sourceElements.SelectMany(g => g))
        {
            // skip elements that are already in the map
            if (mappings.ContainsKey(sourceElement.ElementId))
            {
                continue;
            }

            bool added = false;

            // grab the id components so we can see if something in the path has already been mapped
            string[] sourceComponents = sourceElement.ElementId.Split('.');

            // need to iterate over every path component to see if there was a renamed backbone somewhere in the id
            for (int i = sourceComponents.Length; i > 0; i--)
            {
                // TODO: This needs to check if the element exists too!

                if (!mappings.TryGetValue(string.Join(".", sourceComponents[..i]), out List<ElementMapTrackingRec>? existingMappings))
                {
                    continue;
                }

                // iterate over the any targets to see if we can use the mapped prefix to find an element
                foreach (ElementMapTrackingRec existingMapping in existingMappings)
                {
                    // skip maps with no targets
                    if (existingMapping.TargetElement == null)
                    {
                        continue;
                    }

                    string[] existingTargetComponents = existingMapping.TargetElement.ElementId.Split('.');

                    // build a path that adds the current path to the existing mapped path
                    string testPath = string.Join(".", [..existingTargetComponents, ..sourceComponents[^i]]);

                    ElementDefinition? targetElement = targetElements[testPath].FirstOrDefault();

                    if (targetElement == null)
                    {
                        continue;
                    }

                    // add a mapping for this element
                    ConceptMap.SourceElementComponent sourceMapElement = new()
                    {
                        Code = sourceElement.ElementId,
                        Target = [
                            new()
                            {
                                Code = targetElement.ElementId,
                                Display = targetElement.cgShort(),
                                Comment = $"Generated by matching the source element path based on {existingMapping.SourceElement.ElementId}",
                                Relationship = ConceptMap.ConceptMapRelationship.Equivalent,
                                Property = getMappingProperties(generated: true, needsReview: true),
                            }],
                    };

                    // add the mapping to the dictionary
                    mappings.AddToValue(
                        sourceElement.ElementId,
                        new()
                        {
                            SourceElement = sourceElement,
                            SourceMapElement = sourceMapElement,
                            TargetElement = targetElement,
                            TargetMapElement = sourceMapElement.Target[0],
                        });

                    // once we have found a mapping, stop looking
                    added = true;
                    break;
                }
            }

            // check to see if we need to add a no-map entry
            if (!added)
            {
                // add a no-map entry to the concept map
                ConceptMap.SourceElementComponent noMapElement = new()
                {
                    Code = sourceElement.ElementId,
                    NoMap = true,
                };
                cm.Group[0].Element.Add(noMapElement);

                // add to our dictionary
                mappings.AddToValue(
                    sourceElement.ElementId,
                    new()
                    {
                        SourceElement = sourceElement,
                        SourceMapElement = noMapElement,
                        TargetElement = null,
                        TargetMapElement = null,
                    });
            }
        }

        return;
    }


    /// <summary>
    /// Perform the primitive type comparisons.
    /// </summary>
    private void compareAllPrimitiveTypes()
    {
        checkPrimitiveOverviewMaps();
    }

    private void compareAllComplexTypes()
    {
        checkOverviewMaps(FhirArtifactClassEnum.ComplexType);
        checkElementMaps(FhirArtifactClassEnum.ComplexType);
    }
}
