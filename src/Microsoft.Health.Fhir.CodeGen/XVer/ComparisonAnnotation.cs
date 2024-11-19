// <copyright file="ComparisonAnnotation.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;

#if NETSTANDARD2_0
using Microsoft.Health.Fhir.CodeGenCommon.Polyfill;
#endif

namespace Microsoft.Health.Fhir.CodeGen.XVer;

public enum ComparisonDirection
{
    Up,
    Down,
}


public record class ValueSetGraphCell : ICloneable
{
    public required DefinitionCollection DC { get; init; }
    public required ValueSet Resource { get; init; }

    public int UniqueCodeCount { get; set; } = 0;

    public ValueSetGraphCell? LeftCell { get; set; } = null;
    public ResourceGraphEdge<ValueSet>? LeftEdge { get; set; } = null;

    public ValueSetGraphCell? RightCell { get; set; } = null;
    public ResourceGraphEdge<ValueSet>? RightEdge { get; set; } = null;

    object ICloneable.Clone() => this with { };
}

public record class ResourceGraphEdge<T>
    where T : Hl7.Fhir.Model.DomainResource, IConformanceResource
{
    public required T Source { get; init; }
    public required T Target { get; init; }

    public required ComparisonDirection Direction { get; init; }

    public required ConceptMap? Up { get; init; }
    public required ConceptMap? Down { get; init; }
}

public class ValueSetGraph
{
    public required DefinitionCollection[] Definitions { get; init; }
    public required HashSet<ValueSet> Resources { get; init; }
    public required Dictionary<ValueSet, List<ResourceGraphEdge<ValueSet>>> Edges { get; init; }

    private IEnumerable<(ValueSet target, ResourceGraphEdge<ValueSet> via)> getNeighbors(ValueSet source, ComparisonDirection direction) =>
        from edge in Edges.TryGetValue(source, out List<ResourceGraphEdge<ValueSet>>? edges) ? edges : []
        where edge.Direction == direction
        select (edge.Target, edge);

    public List<ValueSetGraphCell?[]> Project(DefinitionCollection keyDc, ValueSet keyResource)
    {
        ValueSetGraphCell?[] row = new ValueSetGraphCell?[Definitions.Length];
        int startCol = Array.IndexOf(Definitions, keyDc);
        row[startCol] = new()
        {
            DC = keyDc,
            Resource = keyResource,
        };

        List<ValueSetGraphCell?[]> upwards = [];

        // project up
        if (startCol < Definitions.Length)
        {
            upwards = project(row, startCol, ComparisonDirection.Up);
        }

        // if we started at the first definition, we are done
        if (startCol == 0)
        {
            return upwards;
        }

        List<ValueSetGraphCell?[]> results = [];

        // project down
        if (startCol > 0)
        {
            foreach (ValueSetGraphCell?[] partial in upwards)
            {
                results.AddRange(project(partial, startCol, ComparisonDirection.Down));
            }
        }

        return results;
    }

    private List<ValueSetGraphCell?[]> project(
        ValueSetGraphCell?[] incomingRow,
        int column,
        ComparisonDirection direction)
    {
        if (incomingRow[column] == null)
        {
            return [incomingRow];
        }

        int nextCol = direction == ComparisonDirection.Up ? column + 1 : column - 1;
        if (nextCol < 0 || nextCol >= Definitions.Length)
        {
            return [incomingRow];
        }

        IReadOnlyList<(ValueSet target, ResourceGraphEdge<ValueSet> via)> neighbors = getNeighbors(incomingRow[column]!.Resource, direction).ToList();

        if (neighbors.Count == 0)
        {
            return [incomingRow];
        }

        List<ValueSetGraphCell?[]> results = [];

        // iterate over neighbors in this direction
        foreach ((ValueSet target, ResourceGraphEdge<ValueSet> via) in neighbors)
        {
            ValueSetGraphCell?[] row = neighbors.Count == 1 ? incomingRow : incomingRow.DeepClone().ToArray();

            if (direction == ComparisonDirection.Up)
            {
                row[nextCol] = new()
                {
                    DC = Definitions[nextCol],
                    Resource = target,
                    LeftCell = row[column],
                    LeftEdge = via,
                };

                row[column]!.RightCell = row[nextCol];
                row[column]!.RightEdge = via;
            }
            else
            {
                row[nextCol] = new()
                {
                    DC = Definitions[nextCol],
                    Resource = target,
                    RightCell = row[column],
                    RightEdge = via,
                };
                row[column]!.LeftCell = row[nextCol];
                row[column]!.LeftEdge = via;
            }

            // recurse
            List<ValueSetGraphCell?[]> completedRows = completedRows = project(row, nextCol, direction);

            // add to our current set of results
            results.AddRange(completedRows);
        }

        return results;
    }
}



public record class ValueSetComponentGraphCell : ICloneable
{
    public required ValueSetGraphCell ValueSetCell { get; init; }

    public required ValueSet.ContainsComponent Component { get; init; }

    public ValueSetComponentGraphCell? LeftCell { get; set; } = null;
    public ResourceComponentGraphEdge<ValueSet.ContainsComponent>? LeftEdge { get; set; } = null;

    public ValueSetComponentGraphCell? RightCell { get; set; } = null;
    public ResourceComponentGraphEdge<ValueSet.ContainsComponent>? RightEdge { get; set; } = null;

    object ICloneable.Clone() => this with { };

}

public record class ResourceComponentGraphEdge<T>
    where T : Hl7.Fhir.Model.Base
{
    public required T Source { get; init; }
    public required T Target { get; init; }

    public required ComparisonDirection Direction { get; init; }

    public required ConceptMap.SourceElementComponent? UpSource { get; init; }
    public required ConceptMap.TargetElementComponent? UpTarget { get; init; }

    public required ConceptMap.SourceElementComponent? DownSource { get; init; }
    public required ConceptMap.TargetElementComponent? DownTarget { get; init; }
}


public class ValueSetComponentGraph
{
    private Dictionary<ValueSet.ContainsComponent, List<ResourceComponentGraphEdge<ValueSet.ContainsComponent>>> _edges = [];

    private ValueSetGraphCell?[] _valueSetRow = null!;
    public required ValueSetGraphCell?[] ValueSetRow
    {
        get => _valueSetRow;
        init
        {
            _valueSetRow = value;
            _edges = buildEdges();
        }
    }

    private Dictionary<ValueSet.ContainsComponent, List<ResourceComponentGraphEdge<ValueSet.ContainsComponent>>> buildEdges()
    {
        Dictionary<ValueSet.ContainsComponent, List<ResourceComponentGraphEdge<ValueSet.ContainsComponent>>> edges = [];

        // build the contains dictionaries for every cell
        Dictionary<string, ValueSet.ContainsComponent>[] cellContains = _valueSetRow.Select(cell => (cell?.Resource.cgGetFlatContains() ?? []).ToDictionary(c => c.cgKey())).ToArray();

        // update code counts for each value set
        for (int column = 0; column < cellContains.Length; column++)
        {
            if (_valueSetRow[column]?.UniqueCodeCount == 0)
            {
                _valueSetRow[column]!.UniqueCodeCount = cellContains[column].Count;
            }
        }

        // iterate over the cells upwards
        for (int column = 0; column < (_valueSetRow.Length - 1); column++)
        {
            ValueSetGraphCell? currentCell = _valueSetRow[column];

            if (currentCell == null)
            {
                continue;
            }

            ValueSetGraphCell? nextCell = _valueSetRow[column + 1];

            if ((nextCell == null) ||
                (currentCell.RightEdge == null))
            {
                continue;
            }

            Dictionary<string, ValueSet.ContainsComponent> leftContains = cellContains[column];
            Dictionary<string, ValueSet.ContainsComponent> rightContains = cellContains[column + 1];

            Dictionary<(string leftKey, string? rightKey), ConceptMapExtensions.ConceptMapElementMapping> upMappings =
                (currentCell.RightEdge.Up?.cgGetMappings() ?? []).ToDictionary(m => (m.SourceKey, m.TargetKey));

            Dictionary<(string rightKey, string? leftKey), ConceptMapExtensions.ConceptMapElementMapping> downMappings =
                (currentCell.RightEdge.Down?.cgGetMappings() ?? []).ToDictionary(m => (m.SourceKey, m.TargetKey));

            // iterate over the upwards mappings
            foreach (((string leftKey, string? rightKey), ConceptMapExtensions.ConceptMapElementMapping mapping) in upMappings)
            {
                // any maps we use must have both directions
                if (rightKey == null)
                {
                    continue;
                }

                if (!leftContains.TryGetValue(leftKey, out ValueSet.ContainsComponent? leftComponent) ||
                    !rightContains.TryGetValue(rightKey, out ValueSet.ContainsComponent? rightComponent))
                {
                    continue;
                }

                _ = downMappings.TryGetValue((rightKey, leftKey), out ConceptMapExtensions.ConceptMapElementMapping? reverseMapping);

                edges.AddToValue(leftComponent, new()
                {
                    Source = leftComponent,
                    Target = rightComponent,
                    Direction = ComparisonDirection.Up,
                    UpSource = mapping.SourceElement,
                    UpTarget = mapping.TargetElement,
                    DownSource = reverseMapping?.SourceElement,
                    DownTarget = reverseMapping?.TargetElement,
                });
            }

            // iterate over the downwards mappings
            foreach (((string rightKey, string? leftKey), ConceptMapExtensions.ConceptMapElementMapping mapping) in downMappings)
            {
                // any maps we use must have both directions
                if (leftKey == null)
                {
                    continue;
                }

                if (!rightContains.TryGetValue(rightKey, out ValueSet.ContainsComponent? sourceComponent) ||
                    !leftContains.TryGetValue(leftKey, out ValueSet.ContainsComponent? targetComponent))
                {
                    continue;
                }

                _ = upMappings.TryGetValue((leftKey, rightKey), out ConceptMapExtensions.ConceptMapElementMapping? reverseMapping);

                edges.AddToValue(sourceComponent, new()
                {
                    Source = sourceComponent,
                    Target = targetComponent,
                    Direction = ComparisonDirection.Down,
                    UpSource = reverseMapping?.SourceElement,
                    UpTarget = reverseMapping?.TargetElement,
                    DownSource = mapping.SourceElement,
                    DownTarget = mapping.TargetElement,
                });
            }
        }

        return edges;
    }


    private IEnumerable<(ValueSet.ContainsComponent target, ResourceComponentGraphEdge<ValueSet.ContainsComponent> via)> getNeighbors(ValueSet.ContainsComponent source, ComparisonDirection direction) =>
        from edge in _edges.TryGetValue(source, out List<ResourceComponentGraphEdge<ValueSet.ContainsComponent>>? edges) ? edges : []
        where edge.Direction == direction
        select (edge.Target, edge);

    public List<ValueSetComponentGraphCell?[]> Project(ValueSetGraphCell keyCell, ValueSet.ContainsComponent keyComponent)
    {
        ValueSetComponentGraphCell?[] row = new ValueSetComponentGraphCell?[_valueSetRow.Length];
        int startCol = Array.IndexOf(_valueSetRow, keyCell);
        row[startCol] = new()
        {
            ValueSetCell = keyCell,
            Component = keyComponent,
        };

        List<ValueSetComponentGraphCell?[]> upwards = [];

        // project up
        if (startCol < _valueSetRow.Length)
        {
            upwards = project(row, startCol, ComparisonDirection.Up);
        }

        // if we started at the first definition, we are done
        if (startCol == 0)
        {
            return upwards;
        }

        List<ValueSetComponentGraphCell?[]> results = [];

        // project down
        if (startCol > 0)
        {
            foreach (ValueSetComponentGraphCell?[] partial in upwards)
            {
                results.AddRange(project(partial, startCol, ComparisonDirection.Down));
            }
        }

        return results;
    }

    private List<ValueSetComponentGraphCell?[]> project(
        ValueSetComponentGraphCell?[] incomingRow,
        int column,
        ComparisonDirection direction)
    {
        if (incomingRow[column] == null)
        {
            return [incomingRow];
        }

        int nextCol = direction == ComparisonDirection.Up ? column + 1 : column - 1;
        if (nextCol < 0 || nextCol >= ValueSetRow.Length)
        {
            return [incomingRow];
        }

        IReadOnlyList<(ValueSet.ContainsComponent target, ResourceComponentGraphEdge<ValueSet.ContainsComponent> via)> neighbors = getNeighbors(incomingRow[column]!.Component, direction).ToList();

        if (neighbors.Count == 0)
        {
            return [incomingRow];
        }

        List<ValueSetComponentGraphCell?[]> results = [];

        // iterate over neighbors in this direction
        foreach ((ValueSet.ContainsComponent target, ResourceComponentGraphEdge<ValueSet.ContainsComponent> via) in neighbors)
        {
            ValueSetComponentGraphCell?[] row = neighbors.Count == 1 ? incomingRow : incomingRow.DeepClone().ToArray();

            if (direction == ComparisonDirection.Up)
            {
                row[nextCol] = new()
                {
                    ValueSetCell = _valueSetRow[nextCol]!,
                    Component = target,
                    LeftCell = row[column],
                    LeftEdge = via,
                };

                row[column]!.RightCell = row[nextCol];
                row[column]!.RightEdge = via;
            }
            else
            {
                row[nextCol] = new()
                {
                    ValueSetCell = _valueSetRow[nextCol]!,
                    Component = target,
                    RightCell = row[column],
                    RightEdge = via,
                };
                row[column]!.LeftCell = row[nextCol];
                row[column]!.LeftEdge = via;
            }

            // recurse
            List<ValueSetComponentGraphCell?[]> completedRows = completedRows = project(row, nextCol, direction);

            // add to our current set of results
            results.AddRange(completedRows);
        }

        return results;
    }
}





public record class ValueSetMappingCell : ICloneable
{
    public required DefinitionCollection DC { get; init; }
    public required ValueSet VS { get; init; }

    public ValueSetMappingCell? LeftCell { get; set; } = null;
    public ConceptMap? ToLeftCell { get; set; } = null;
    public ConceptMappingTable? ToLeftCellCodeMap { get; set; } = null;
    public ConceptMap? FromLeftCell { get; set; } = null;
    public ConceptMappingTable? FromLeftCellCodeMap { get; set; } = null;

    public ValueSetMappingCell? RightCell { get; set; } = null;
    public ConceptMap? ToRightCell { get; set; } = null;
    public ConceptMappingTable? ToRightCellCodeMap { get; set; } = null;
    public ConceptMap? FromRightCell { get; set; } = null;
    public ConceptMappingTable? FromRightCellCodeMap { get; set; } = null;

    object ICloneable.Clone() => this with { };
}

public class ConceptMappingTable
    : Dictionary<
        (string system, string code),
        Dictionary<(string? system, string? code), ConceptMapExtensions.ConceptMapElementMapping>>
{
    public ConceptMappingTable(ConceptMap cm)
    {
        // iterate across groups
        foreach (ConceptMap.GroupComponent group in cm.Group)
        {
            // grab the source and target
            string? sourceSystem = group.Source;
            string? targetSystem = group.Target;

            // iterate across each element in the group
            foreach (ConceptMap.SourceElementComponent sourceElement in group.Element)
            {
                // add if this is a no map
                if (sourceElement.NoMap == true)
                {
                    if (!this.TryGetValue(
                        (sourceSystem, sourceElement.Code),
                        out Dictionary<(string? system, string? code), ConceptMapExtensions.ConceptMapElementMapping>? targets))
                    {
                        targets = [];
                        this.Add((sourceSystem, sourceElement.Code), targets);
                    }

                    targets[(null, null)] = new()
                    {
                        SourceSystem = sourceSystem,
                        TargetSystem = targetSystem,
                        SourceElement = sourceElement,
                        TargetElement = null,
                    };

                    continue;
                }

                // iterate across each target element
                foreach (ConceptMap.TargetElementComponent targetElement in sourceElement.Target)
                {
                    if (!this.TryGetValue(
                        (sourceSystem, sourceElement.Code),
                        out Dictionary<(string? system, string? code), ConceptMapExtensions.ConceptMapElementMapping>? targets))
                    {
                        targets = [];
                        this.Add((sourceSystem, sourceElement.Code), targets);
                    }

                    targets.Add((targetSystem, targetElement.Code), new()
                    {
                        SourceSystem = sourceSystem,
                        TargetSystem = targetSystem,
                        SourceElement = sourceElement,
                        TargetElement = targetElement,
                    });
                }
            }
        }
    }
}




/// <summary>
/// Represents the failure codes for the comparison.
/// </summary>
public enum ComparisonFailureCodes
{
    UnresolvedTarget,
    CannotExpand,
}

/// <summary>
/// Represents the relationship codes for the concept domain.
/// </summary>
public enum ConceptDomainRelationshipCodes
{
    Unknown,
    Equivalent,
    SourceIsNew,
    SourceIsDeprecated,
    NotMapped,
    SourceIsNarrowerThanTarget,
    SourceIsBroaderThanTarget,
    Related,
    NotRelated,
}

/// <summary>
/// Represents the flags for value set concept relationships.
/// </summary>
[Flags]
public enum ValueSetConceptRelationshipFlags : long
{
    None = 0,
    Equivalent = 1,
    Added = 2,
    Removed = 4,
    Renamed = 8,
    SystemChanged = 16,
}

/// <summary>
/// Represents the flags for structural relationships.
/// </summary>
[Flags]
public enum StructuralRelationshipFlags : long
{
    None = 0,
    Added = 1,
    Removed = 2,
    Renamed = 4,
    Moved = 8,
    ConvertedToBackbone = 16,
    ConvertedToSimpleProperty = 32,
    ConvertedToComplexProperty = 64,
    ConvertedToChoiceProperty = 128,
}

/// <summary>
/// Represents the flags for type relationships.
/// </summary>
[Flags]
public enum TypeRelationshipFlags : long
{
    None = 0,
    AddedType = 1,
    RemovedType = 2,
    ReplacedType = 4,
    AddedProfile = 8,
    RemovedProfile = 16,
    ReplacedProfile = 32,
}

/// <summary>
/// Represents the flags for cardinality relationships.
/// </summary>
[Flags]
public enum CardinalityRelationshipFlags : long
{
    None = 0,
    MadeRequired = 1,
    MadeOptional = 2,
    MadeArray = 4,
    MadeScalar = 8,
    MadeProhibited = 16,
    MadeAllowed = 32,
    ArrayLenReduced = 64,
    ArrayLenIncreased = 128,
}

/// <summary>
/// Represents the flags for binding relationships.
/// </summary>
[Flags]
public enum BindingRelationshipFlags : long
{
    None = 0,
    TargetContentsIncreased = 1,
    TargetContentsDecreased = 2,
    TargetContentsChanged = 4,
    StrengthIncreased = 8,
    StrengthDecreased = 16,
    UnresolvedChange = 32,
}

public static class ComparisonExtensions
{
    public static ConceptDomainRelationshipCodes ToDomainRelationship(this Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? cmr) => cmr switch
    {
        Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.RelatedTo => ConceptDomainRelationshipCodes.Related,
        Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.Equivalent => ConceptDomainRelationshipCodes.Equivalent,
        Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.SourceIsNarrowerThanTarget => ConceptDomainRelationshipCodes.SourceIsNarrowerThanTarget,
        Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.SourceIsBroaderThanTarget => ConceptDomainRelationshipCodes.SourceIsBroaderThanTarget,
        Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.NotRelatedTo => ConceptDomainRelationshipCodes.Unknown,
        _ => ConceptDomainRelationshipCodes.Unknown
    };
}
