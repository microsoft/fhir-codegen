// <copyright file="ValueSetGraph.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGen.CompareTool;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;

#if NETSTANDARD2_0
using Microsoft.Health.Fhir.CodeGenCommon.Polyfill;
using Newtonsoft.Json.Linq;
#endif

namespace Microsoft.Health.Fhir.CodeGen.XVer;



public record class ValueSetGraphCell : ICloneable
{
    public required DefinitionCollection DC { get; init; }
    public required ValueSet Resource { get; init; }

    public int UniqueCodeCount { get; set; } = 0;

    public ValueSetGraphCell? LeftCell { get; set; } = null;
    public ValueSetGraphEdge? LeftEdge { get; set; } = null;

    public ValueSetGraphCell? RightCell { get; set; } = null;
    public ValueSetGraphEdge? RightEdge { get; set; } = null;

    object ICloneable.Clone() => this with { };
}

public record class ValueSetGraphEdge
{
    public required ValueSet Source { get; init; }
    public required ValueSet Target { get; init; }

    public required ComparisonDirection Direction { get; init; }

    public required ConceptMap? Up { get; init; }
    public required ConceptMap? Down { get; init; }
}

/// <summary>
/// Represents a graph of ValueSets and their relationships through ConceptMaps.
/// </summary>
public class ValueSetGraph
{
    private Dictionary<ValueSet, List<ValueSetGraphEdge>> _edges = [];

    /// <summary>
    /// The collection of definitions used in the graph.
    /// </summary>
    public required DefinitionCollection[] Definitions { get; init; }

    /// <summary>
    /// Builds the graph based on the provided comparisons.
    /// </summary>
    /// <param name="comparisons"></param>
    public void Build(IEnumerable<FhirCoreComparer> comparisons)
    {
        buildEdges(comparisons);
    }

    /// <summary>
    /// Builds the edges of the graph based on the provided comparisons.
    /// </summary>
    /// <param name="comparisons">The enumerable of FhirCoreComparers used to build the edges of the graph.</param>
    private void buildEdges(IEnumerable<FhirCoreComparer> comparisons)
    {
        foreach (FhirCoreComparer coreComparer in comparisons)
        {
            // iterate over the paired maps in this comparison
            foreach ((ValueSet leftVs, ValueSet rightVs, ConceptMap? up, ConceptMap? down) in coreComparer.GetPairedValueSetMaps())
            {
                if (up != null)
                {
                    _edges.AddToValue(leftVs, new()
                    {
                        Direction = ComparisonDirection.Up,
                        Source = leftVs,
                        Target = rightVs,
                        Up = up,
                        Down = down,
                    });
                }

                if (down != null)
                {
                    _edges.AddToValue(rightVs, new()
                    {
                        Direction = ComparisonDirection.Down,
                        Source = rightVs,
                        Target = leftVs,
                        Up = up,
                        Down = down,
                    });
                }
            }
        }
    }

    /// <summary>
    /// Gets the neighboring ValueSets and the edges connecting them in the specified direction.
    /// </summary>
    /// <param name="source">The source ValueSet.</param>
    /// <param name="direction">The direction of the comparison.</param>
    /// <returns>An enumerable of tuples containing the target ValueSet and the connecting edge.</returns>
    private IEnumerable<(ValueSet target, ValueSetGraphEdge via)> getNeighbors(ValueSet source, ComparisonDirection direction) =>
        from edge in _edges.TryGetValue(source, out List<ValueSetGraphEdge>? edges) ? edges : []
        where edge.Direction == direction
        select (edge.Target, edge);

    /// <summary>
    /// Projects the graph starting from the specified key definition collection and key resource.
    /// </summary>
    /// <param name="keyDc">The key definition collection.</param>
    /// <param name="keyResource">The key ValueSet resource.</param>
    /// <returns>A list of arrays representing the projected graph cells.</returns>
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

    /// <summary>
    /// Recursively projects the graph in the specified direction.
    /// </summary>
    /// <param name="incomingRow">The incoming row of graph cells.</param>
    /// <param name="column">The current column index.</param>
    /// <param name="direction">The direction of the projection.</param>
    /// <returns>A list of arrays representing the projected graph cells.</returns>
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

        IReadOnlyList<(ValueSet target, ValueSetGraphEdge via)> neighbors = getNeighbors(incomingRow[column]!.Resource, direction).ToList();

        if (neighbors.Count == 0)
        {
            return [incomingRow];
        }

        List<ValueSetGraphCell?[]> results = [];

        // iterate over neighbors in this direction
        foreach ((ValueSet target, ValueSetGraphEdge via) in neighbors)
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
    public ValueSetContainsGraphEdge? LeftEdge { get; set; } = null;

    public ValueSetComponentGraphCell? RightCell { get; set; } = null;
    public ValueSetContainsGraphEdge? RightEdge { get; set; } = null;

    object ICloneable.Clone() => this with { };

}

public record class ValueSetContainsGraphEdge
{
    public required ValueSet.ContainsComponent Source { get; init; }
    public required ValueSet.ContainsComponent Target { get; init; }

    public required ComparisonDirection Direction { get; init; }

    public required ConceptMap.SourceElementComponent? UpSource { get; init; }
    public required ConceptMap.TargetElementComponent? UpTarget { get; init; }

    public required ConceptMap.SourceElementComponent? DownSource { get; init; }
    public required ConceptMap.TargetElementComponent? DownTarget { get; init; }
}


public class ValueSetComponentGraph
{
    private Dictionary<ValueSet.ContainsComponent, List<ValueSetContainsGraphEdge>> _edges = [];

    private ValueSetGraphCell?[] _sourceRow = null!;
    public required ValueSetGraphCell?[] SourceRow
    {
        get => _sourceRow;
        init
        {
            _sourceRow = value;
            buildEdges();
        }
    }

    private void buildEdges()
    {
        // build the contains dictionaries for every cell
        Dictionary<string, ValueSet.ContainsComponent>[] cellContains = _sourceRow.Select(cell => (cell?.Resource.cgGetFlatContains() ?? []).ToDictionary(c => c.cgKey())).ToArray();

        // update code counts for each value set
        for (int column = 0; column < cellContains.Length; column++)
        {
            if (_sourceRow[column]?.UniqueCodeCount == 0)
            {
                _sourceRow[column]!.UniqueCodeCount = cellContains[column].Count;
            }
        }

        // iterate over the cells upwards
        for (int column = 0; column < (_sourceRow.Length - 1); column++)
        {
            ValueSetGraphCell? currentCell = _sourceRow[column];

            if (currentCell == null)
            {
                continue;
            }

            ValueSetGraphCell? nextCell = _sourceRow[column + 1];

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

                _edges.AddToValue(leftComponent, new()
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

                _edges.AddToValue(sourceComponent, new()
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
    }


    private IEnumerable<(ValueSet.ContainsComponent target, ValueSetContainsGraphEdge via)> getNeighbors(ValueSet.ContainsComponent source, ComparisonDirection direction) =>
        from edge in _edges.TryGetValue(source, out List<ValueSetContainsGraphEdge>? edges) ? edges : []
        where edge.Direction == direction
        select (edge.Target, edge);

    public List<ValueSetComponentGraphCell?[]> Project(ValueSetGraphCell keyCell, ValueSet.ContainsComponent keyComponent)
    {
        ValueSetComponentGraphCell?[] row = new ValueSetComponentGraphCell?[_sourceRow.Length];
        int startCol = Array.IndexOf(_sourceRow, keyCell);
        row[startCol] = new()
        {
            ValueSetCell = keyCell,
            Component = keyComponent,
        };

        List<ValueSetComponentGraphCell?[]> upwards = [];

        // project up
        if (startCol < _sourceRow.Length)
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
        if (nextCol < 0 || nextCol >= SourceRow.Length)
        {
            return [incomingRow];
        }

        IReadOnlyList<(ValueSet.ContainsComponent target, ValueSetContainsGraphEdge via)> neighbors = getNeighbors(incomingRow[column]!.Component, direction).ToList();

        if (neighbors.Count == 0)
        {
            return [incomingRow];
        }

        List<ValueSetComponentGraphCell?[]> results = [];

        // iterate over neighbors in this direction
        foreach ((ValueSet.ContainsComponent target, ValueSetContainsGraphEdge via) in neighbors)
        {
            ValueSetComponentGraphCell?[] row = neighbors.Count == 1 ? incomingRow : incomingRow.DeepClone().ToArray();

            if (direction == ComparisonDirection.Up)
            {
                row[nextCol] = new()
                {
                    ValueSetCell = _sourceRow[nextCol]!,
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
                    ValueSetCell = _sourceRow[nextCol]!,
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


