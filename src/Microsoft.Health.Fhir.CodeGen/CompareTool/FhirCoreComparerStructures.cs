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

    /// <summary>
    /// Retrieves the paired data type maps from the cross-version map collections.
    /// Note that there will always be at most one (and should always be one), but this method
    /// returns an enumerable for consistency with other map retrieval methods.
    /// </summary>
    /// <returns>
    /// An enumerable of tuples containing the ConceptMap for the upward and downward directions.
    /// Each tuple contains:
    /// <list type="bullet">
    /// <item>
    /// <description>The ConceptMap for the upward direction, or null if not available.</description>
    /// </item>
    /// <item>
    /// <description>The ConceptMap for the downward direction, or null if not available.</description>
    /// </item>
    /// </list>
    /// </returns>
    public IEnumerable<(StructureDefinition left, StructureDefinition right, ConceptMap? up, ConceptMap? down)> GetPairedStructureConceptMaps()
    {
        Dictionary<(string? source, string? target), ConceptMap> mapsUp =
            (_cvLeftToRight?.GetDataTypeMaps() ?? []).ToDictionary(cm => (cm.cgSourceScope(), cm.cgTargetScope()));
        Dictionary<(string? source, string? target), ConceptMap> mapsDown =
            (_cvRightToLeft?.GetDataTypeMaps() ?? []).ToDictionary(cm => (cm.cgSourceScope(), cm.cgTargetScope()));

        // iterate over the forward maps (up)
        foreach (((string? source, string? target), ConceptMap cmUp) in mapsUp)
        {
            mapsDown.TryGetValue((target, source), out ConceptMap? cmDown);

            StructureDefinition? leftSd = null;
            StructureDefinition? rightSd = null;

            if ((source == null) ||
                !_leftDc.TryGetStructure(source, out leftSd))
            {
                continue;
            }

            if ((target == null) ||
                !_rightDc.TryGetStructure(target, out rightSd))
            {
                continue;
            }

            yield return (leftSd, rightSd, cmUp, cmDown);
        }

        // iterate over the reverse maps looking for orphans
        foreach (((string? source, string? target), ConceptMap cmDown) in mapsDown)
        {
            if (mapsUp.ContainsKey((target, source)))
            {
                continue;
            }

            StructureDefinition? leftSd = null;
            StructureDefinition? rightSd = null;

            if ((source == null) ||
                !_rightDc.TryGetStructure(source, out rightSd))
            {
                continue;
            }

            if ((target == null) ||
                !_leftDc.TryGetStructure(target, out leftSd))
            {
                continue;
            }

            yield return (leftSd, rightSd, null, cmDown);
        }
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
        foreach ((ConceptMap map, IEnumerable<StructureDefinition> types) in getMapAndTypesArray())
        {
            // create a source lookup
            ILookup<string, ConceptMap.SourceElementComponent> sources = map.Group[0].Element.ToLookup(e => e.Code);

            // check to see if all the types exist in the map
            foreach (StructureDefinition sd in types)
            {
                if (!sources.Contains(sd.Name))
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

        (ConceptMap, IEnumerable<StructureDefinition>)[] getMapAndTypesArray() => artifactType switch
        {
            FhirArtifactClassEnum.PrimitiveType => [(up, _leftDc.PrimitiveTypesByName.Values), (down, _rightDc.PrimitiveTypesByName.Values)],
            FhirArtifactClassEnum.ComplexType => [(up, _leftDc.ComplexTypesByName.Values), (down, _rightDc.ComplexTypesByName.Values)],
            FhirArtifactClassEnum.Resource => [(up, _leftDc.ResourcesByName.Values), (down, _rightDc.ResourcesByName.Values)],
            _ => [],
        };
    }

    /// <summary>
    /// Perform the primitive type comparisons.
    /// </summary>
    private void compareAllPrimitiveTypes()
    {
        checkPrimitiveOverviewMaps();
    }


}
