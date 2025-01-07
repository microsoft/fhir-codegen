// <copyright file="StructureDefinitionGraph.cs" company="Microsoft Corporation">
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
using Microsoft.Health.Fhir.CodeGenCommon.Models;


#if NETSTANDARD2_0
using Microsoft.Health.Fhir.CodeGenCommon.Polyfill;
#endif

namespace Microsoft.Health.Fhir.CodeGen.XVer;

/// <summary>
/// Represents a cell in a graph of Structure Definitions and their relationships through ConceptMaps.
/// </summary>
public record class StructureDefinitionGraphCell : ICloneable
{
    public required DefinitionCollection DC { get; init; }
    public required StructureDefinition Resource { get; init; }

    public int UniqueElementCount { get; set; } = 0;

    public StructureDefinitionGraphCell? LeftCell { get; set; } = null;
    public StructureDefinitionGraphEdge? LeftEdge { get; set; } = null;

    public StructureDefinitionGraphCell? RightCell { get; set; } = null;
    public StructureDefinitionGraphEdge? RightEdge { get; set; } = null;

    object ICloneable.Clone() => this with { };
}


/// <summary>
/// Represents an edge in a graph of Structure Definitions and their relationships through ConceptMaps.
/// </summary>
public record class StructureDefinitionGraphEdge
{
    public required StructureDefinition Source { get; init; }
    public required StructureDefinition Target { get; init; }

    public required ComparisonDirection Direction { get; init; }

    public required ConceptMap? OverviewUp { get; init; }
    public required ConceptMap.SourceElementComponent? OverviewUpSource { get; init; }
    public required ConceptMap.TargetElementComponent? OverviewUpTarget { get; init; }

    public required ConceptMap? OverviewDown { get; init; }
    public required ConceptMap.SourceElementComponent? OverviewDownSource { get; init; }
    public required ConceptMap.TargetElementComponent? OverviewDownTarget { get; init; }

    public required ConceptMap? Up { get; init; }
    public required ConceptMap? Down { get; init; }
}


/// <summary>
/// Represents a graph of Structure Definitions and their relationships through ConceptMaps.
/// </summary>
public class StructureDefinitionGraph
{
    private Dictionary<StructureDefinition, List<StructureDefinitionGraphEdge>> _edges = [];

    /// <summary>
    /// The collection of definitions used in the graph.
    /// </summary>
    public required DefinitionCollection[] Definitions { get; init; }

    /// <summary>
    /// The type of artifact represented by the graph.
    /// </summary>
    public required FhirArtifactClassEnum ArtifactType { get; init; }

    /// <summary>
    /// Builds the graph based on the provided comparisons.
    /// </summary>
    /// <param name="comparisons"></param>
    public void Build(IEnumerable<FhirCoreComparer> comparisons)
    {
        switch (ArtifactType)
        {
            case FhirArtifactClassEnum.PrimitiveType:
                buildPrimitiveTypeEdges(comparisons);
                break;
            case FhirArtifactClassEnum.ComplexType:
                buildComplexTypeEdges(comparisons);
                break;
            case FhirArtifactClassEnum.Resource:
                buildResourceEdges(comparisons);
                break;
        }

    }

    /// <summary>
    /// Builds the graph based on the provided comparisons.
    /// </summary>
    /// <param name="comparisons"></param>
    public void BuildResourceEdges(IEnumerable<FhirCoreComparer> comparisons)
    {
        buildResourceEdges(comparisons);
    }

    /// <summary>
    /// Builds the edges of the graph based on the provided comparisons.
    /// <param name="comparisons">The enumerable of FhirCoreComparers used to build the edges of the graph.</param>
    /// </summary>
    private void buildPrimitiveTypeEdges(IEnumerable<FhirCoreComparer> comparisons)
    {
        // iterate across the comparisons
        foreach (FhirCoreComparer coreComparer in comparisons)
        {
            // grab the type comparison overview maps
            (ConceptMap overviewUp, ConceptMap overviewDown) = coreComparer.GetStructureOverviewMaps(ArtifactType);

            // build a dictionary of the sources and targets for each element in all groups of the up direction
            Dictionary<(string source, string target), (ConceptMap.SourceElementComponent sourceElement, ConceptMap.TargetElementComponent targetElement)> pairsUp =
                overviewUp.Group
                .SelectMany(cmg => cmg.Element)
                .Where(se => se.Code != null)
                .SelectMany(se => se.Target.Where(te => te.Code != null), (se, te) => (se, te))
                .ToDictionary(v => (v.se.Code, v.te.Code), v => v);

            // build a dictionary of the sources and targets for each element in all groups of the down direction
            Dictionary<(string source, string target), (ConceptMap.SourceElementComponent sourceElement, ConceptMap.TargetElementComponent targetElement)> pairsDown =
                overviewDown.Group
                .SelectMany(cmg => cmg.Element)
                .Where(se => se.Code != null)
                .SelectMany(se => se.Target.Where(te => te.Code != null), (se, te) => (se, te))
                .ToDictionary(v => (v.se.Code, v.te.Code), v => v);

            // iterate over the upward mapping pairs
            foreach (((string source, string target), (ConceptMap.SourceElementComponent sourceElement, ConceptMap.TargetElementComponent targetElement)) in pairsUp)
            {
                // try to resolve the source and target stucture definitions
                if (!coreComparer.LeftDC.TryGetStructure(source, out StructureDefinition? leftSd) ||
                    !coreComparer.RightDC.TryGetStructure(target, out StructureDefinition? rightSd))
                {
                    continue;
                }

                // try to resolve the reverse mapping
                bool hasReverse = pairsDown.TryGetValue((target, source), out var reverseMapping);

                // add this edge
                _edges.AddToValue(leftSd, new()
                {
                    Direction = ComparisonDirection.Up,
                    Source = leftSd,
                    Target = rightSd,
                    OverviewUp = overviewUp,
                    OverviewUpSource = sourceElement,
                    OverviewUpTarget = targetElement,
                    OverviewDown = overviewDown,
                    OverviewDownSource = hasReverse ? reverseMapping.sourceElement : null,
                    OverviewDownTarget = hasReverse ? reverseMapping.targetElement : null,
                    Up = null,
                    Down = null,
                });
            }

            // iterate over the downward mapping pairs
            foreach (((string source, string target), (ConceptMap.SourceElementComponent sourceElement, ConceptMap.TargetElementComponent targetElement)) in pairsDown)
            {
                // try to resolve the source and target stucture definitions
                if (!coreComparer.RightDC.TryGetStructure(source, out StructureDefinition? rightSd) ||
                    !coreComparer.LeftDC.TryGetStructure(target, out StructureDefinition? leftSd))
                {
                    continue;
                }

                // try to resolve the reverse mapping
                bool hasReverse = pairsUp.TryGetValue((target, source), out var reverseMapping);

                // add this edge
                _edges.AddToValue(rightSd, new()
                {
                    Direction = ComparisonDirection.Down,
                    Source = rightSd,
                    Target = leftSd,
                    OverviewUp = overviewUp,
                    OverviewUpSource = hasReverse ? reverseMapping.sourceElement : null,
                    OverviewUpTarget = hasReverse? reverseMapping.targetElement : null,
                    OverviewDown = overviewDown,
                    OverviewDownSource = sourceElement,
                    OverviewDownTarget = targetElement,
                    Up = null,
                    Down = null,
                });
            }
        }
    }

    /// <summary>
    /// Builds the edges of the graph based on the provided comparisons.
    /// <param name="comparisons">The enumerable of FhirCoreComparers used to build the edges of the graph.</param>
    /// </summary>
    private void buildComplexTypeEdges(IEnumerable<FhirCoreComparer> comparisons)
    {
        // iterate across the comparisons
        foreach (FhirCoreComparer coreComparer in comparisons)
        {
            // grab the type comparison overview maps
            (ConceptMap overviewUp, ConceptMap overviewDown) = coreComparer.GetStructureOverviewMaps(ArtifactType);

            // build a dictionary of the sources and targets for each element in all groups of the up direction
            Dictionary<(string source, string target), (ConceptMap.SourceElementComponent sourceElement, ConceptMap.TargetElementComponent targetElement)> pairsUp =
                overviewUp.Group
                .SelectMany(cmg => cmg.Element)
                .Where(se => se.Code != null)
                .SelectMany(se => se.Target.Where(te => te.Code != null), (se, te) => (se, te))
                .ToDictionary(v => (v.se.Code, v.te.Code), v => v);

            // build a dictionary of the sources and targets for each element in all groups of the down direction
            Dictionary<(string source, string target), (ConceptMap.SourceElementComponent sourceElement, ConceptMap.TargetElementComponent targetElement)> pairsDown =
                overviewDown.Group
                .SelectMany(cmg => cmg.Element)
                .Where(se => se.Code != null)
                .SelectMany(se => se.Target.Where(te => te.Code != null), (se, te) => (se, te))
                .ToDictionary(v => (v.se.Code, v.te.Code), v => v);

            // iterate over the upward mapping pairs
            foreach (((string source, string target), (ConceptMap.SourceElementComponent sourceElement, ConceptMap.TargetElementComponent targetElement)) in pairsUp)
            {
                // try to resolve the source and target stucture definitions
                if (!coreComparer.LeftDC.TryGetStructure(source, out StructureDefinition? leftSd) ||
                    !coreComparer.RightDC.TryGetStructure(target, out StructureDefinition? rightSd))
                {
                    continue;
                }

                // try to resolve the reverse mapping
                bool hasReverse = pairsDown.TryGetValue((target, source), out var reverseMapping);

                // add this edge
                _edges.AddToValue(leftSd, new()
                {
                    Direction = ComparisonDirection.Up,
                    Source = leftSd,
                    Target = rightSd,
                    OverviewUp = overviewUp,
                    OverviewUpSource = sourceElement,
                    OverviewUpTarget = targetElement,
                    OverviewDown = overviewDown,
                    OverviewDownSource = hasReverse ? reverseMapping.sourceElement : null,
                    OverviewDownTarget = hasReverse ? reverseMapping.targetElement : null,
                    Up = null,
                    Down = null,
                });
            }

            // iterate over the downward mapping pairs
            foreach (((string source, string target), (ConceptMap.SourceElementComponent sourceElement, ConceptMap.TargetElementComponent targetElement)) in pairsDown)
            {
                // try to resolve the source and target stucture definitions
                if (!coreComparer.RightDC.TryGetStructure(source, out StructureDefinition? rightSd) ||
                    !coreComparer.LeftDC.TryGetStructure(target, out StructureDefinition? leftSd))
                {
                    continue;
                }

                // try to resolve the reverse mapping
                bool hasReverse = pairsUp.TryGetValue((target, source), out var reverseMapping);

                // add this edge
                _edges.AddToValue(rightSd, new()
                {
                    Direction = ComparisonDirection.Down,
                    Source = rightSd,
                    Target = leftSd,
                    OverviewUp = overviewUp,
                    OverviewUpSource = hasReverse ? reverseMapping.sourceElement : null,
                    OverviewUpTarget = hasReverse ? reverseMapping.targetElement : null,
                    OverviewDown = overviewDown,
                    OverviewDownSource = sourceElement,
                    OverviewDownTarget = targetElement,
                    Up = null,
                    Down = null,
                });
            }
        }
    }

    private void buildResourceEdges(IEnumerable<FhirCoreComparer> comparisons)
    {

    }


    /// <summary>
    /// Gets the neighboring StructureDefinitions and the edges connecting them in the specified direction.
    /// </summary>
    /// <param name="source">The source StructureDefinition.</param>
    /// <param name="direction">The direction of the comparison.</param>
    /// <returns>An enumerable of tuples containing the target StructureDefinition and the connecting edge.</returns>
    private IEnumerable<(StructureDefinition target, StructureDefinitionGraphEdge via)> getNeighbors(StructureDefinition source, ComparisonDirection direction) =>
        from edge in _edges.TryGetValue(source, out List<StructureDefinitionGraphEdge>? edges) ? edges : []
        where edge.Direction == direction
        select (edge.Target, edge);

    /// <summary>
    /// Projects the graph starting from the specified key definition collection and key resource.
    /// </summary>
    /// <param name="keyDc">The key definition collection.</param>
    /// <param name="keyResource">The key StructureDefinition resource.</param>
    /// <returns>A list of arrays representing the projected graph cells.</returns>
    public List<StructureDefinitionGraphCell?[]> Project(DefinitionCollection keyDc, StructureDefinition keyResource)
    {
        StructureDefinitionGraphCell?[] row = new StructureDefinitionGraphCell?[Definitions.Length];
        int startCol = Array.IndexOf(Definitions, keyDc);
        row[startCol] = new()
        {
            DC = keyDc,
            Resource = keyResource,
        };

        List<StructureDefinitionGraphCell?[]> upwards = [];

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

        List<StructureDefinitionGraphCell?[]> results = [];

        // project down
        if (startCol > 0)
        {
            foreach (StructureDefinitionGraphCell?[] partial in upwards)
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
    private List<StructureDefinitionGraphCell?[]> project(
        StructureDefinitionGraphCell?[] incomingRow,
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

        IReadOnlyList<(StructureDefinition target, StructureDefinitionGraphEdge via)> neighbors = getNeighbors(incomingRow[column]!.Resource, direction).ToList();

        if (neighbors.Count == 0)
        {
            return [incomingRow];
        }

        List<StructureDefinitionGraphCell?[]> results = [];

        // iterate over neighbors in this direction
        foreach ((StructureDefinition target, StructureDefinitionGraphEdge via) in neighbors)
        {
            StructureDefinitionGraphCell?[] row = neighbors.Count == 1 ? incomingRow : incomingRow.DeepClone().ToArray();

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
            List<StructureDefinitionGraphCell?[]> completedRows = completedRows = project(row, nextCol, direction);

            // add to our current set of results
            results.AddRange(completedRows);
        }

        return results;
    }
}



public record class StructureDefinitionComponentGraphCell : ICloneable
{
    public required StructureDefinitionGraphCell StructureDefinitionCell { get; init; }

    public required ElementDefinition Component { get; init; }

    public StructureDefinitionComponentGraphCell? LeftCell { get; set; } = null;
    public StructureDefinitionElementGraphEdge? LeftEdge { get; set; } = null;

    public StructureDefinitionComponentGraphCell? RightCell { get; set; } = null;
    public StructureDefinitionElementGraphEdge? RightEdge { get; set; } = null;

    object ICloneable.Clone() => this with { };

}

public record class StructureDefinitionElementGraphEdge
{
    public required ElementDefinition Source { get; init; }
    public required ElementDefinition Target { get; init; }

    public required ComparisonDirection Direction { get; init; }

    public required ConceptMap.SourceElementComponent? UpSource { get; init; }
    public required ConceptMap.TargetElementComponent? UpTarget { get; init; }

    public required ConceptMap.SourceElementComponent? DownSource { get; init; }
    public required ConceptMap.TargetElementComponent? DownTarget { get; init; }
}


public class StructureDefinitionComponentGraph
{
    private Dictionary<ElementDefinition, List<StructureDefinitionElementGraphEdge>> _edges = [];

    private StructureDefinitionGraphCell?[] _sourceRow = null!;
    public required StructureDefinitionGraphCell?[] SourceRow
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
        //// build the contains dictionaries for every cell
        //Dictionary<string, ElementDefinition>[] cellContains = _sourceRow.Select(cell => (cell?.Resource.cgGetFlatContains() ?? []).ToDictionary(c => c.cgKey())).ToArray();

        //// update code counts for each value set
        //for (int column = 0; column < cellContains.Length; column++)
        //{
        //    if (_sourceRow[column]?.UniqueElementCount == 0)
        //    {
        //        _sourceRow[column]!.UniqueElementCount = cellContains[column].Count;
        //    }
        //}

        //// iterate over the cells upwards
        //for (int column = 0; column < (_sourceRow.Length - 1); column++)
        //{
        //    StructureDefinitionGraphCell? currentCell = _sourceRow[column];

        //    if (currentCell == null)
        //    {
        //        continue;
        //    }

        //    StructureDefinitionGraphCell? nextCell = _sourceRow[column + 1];

        //    if ((nextCell == null) ||
        //        (currentCell.RightEdge == null))
        //    {
        //        continue;
        //    }

        //    Dictionary<string, ElementDefinition> leftContains = cellContains[column];
        //    Dictionary<string, ElementDefinition> rightContains = cellContains[column + 1];

        //    Dictionary<(string leftKey, string? rightKey), ConceptMapExtensions.ConceptMapElementMapping> upMappings =
        //        (currentCell.RightEdge.Up?.cgGetMappings() ?? []).ToDictionary(m => (m.SourceKey, m.TargetKey));

        //    Dictionary<(string rightKey, string? leftKey), ConceptMapExtensions.ConceptMapElementMapping> downMappings =
        //        (currentCell.RightEdge.Down?.cgGetMappings() ?? []).ToDictionary(m => (m.SourceKey, m.TargetKey));

        //    // iterate over the upwards mappings
        //    foreach (((string leftKey, string? rightKey), ConceptMapExtensions.ConceptMapElementMapping mapping) in upMappings)
        //    {
        //        // any maps we use must have both directions
        //        if (rightKey == null)
        //        {
        //            continue;
        //        }

        //        if (!leftContains.TryGetValue(leftKey, out ElementDefinition? leftComponent) ||
        //            !rightContains.TryGetValue(rightKey, out ElementDefinition? rightComponent))
        //        {
        //            continue;
        //        }

        //        _ = downMappings.TryGetValue((rightKey, leftKey), out ConceptMapExtensions.ConceptMapElementMapping? reverseMapping);

        //        _edges.AddToValue(leftComponent, new()
        //        {
        //            Source = leftComponent,
        //            Target = rightComponent,
        //            Direction = ComparisonDirection.Up,
        //            UpSource = mapping.SourceElement,
        //            UpTarget = mapping.TargetElement,
        //            DownSource = reverseMapping?.SourceElement,
        //            DownTarget = reverseMapping?.TargetElement,
        //        });
        //    }

        //    // iterate over the downwards mappings
        //    foreach (((string rightKey, string? leftKey), ConceptMapExtensions.ConceptMapElementMapping mapping) in downMappings)
        //    {
        //        // any maps we use must have both directions
        //        if (leftKey == null)
        //        {
        //            continue;
        //        }

        //        if (!rightContains.TryGetValue(rightKey, out ElementDefinition? sourceComponent) ||
        //            !leftContains.TryGetValue(leftKey, out ElementDefinition? targetComponent))
        //        {
        //            continue;
        //        }

        //        _ = upMappings.TryGetValue((leftKey, rightKey), out ConceptMapExtensions.ConceptMapElementMapping? reverseMapping);

        //        _edges.AddToValue(sourceComponent, new()
        //        {
        //            Source = sourceComponent,
        //            Target = targetComponent,
        //            Direction = ComparisonDirection.Down,
        //            UpSource = reverseMapping?.SourceElement,
        //            UpTarget = reverseMapping?.TargetElement,
        //            DownSource = mapping.SourceElement,
        //            DownTarget = mapping.TargetElement,
        //        });
        //    }
        //}
    }


    private IEnumerable<(ElementDefinition target, StructureDefinitionElementGraphEdge via)> getNeighbors(ElementDefinition source, ComparisonDirection direction) =>
        from edge in _edges.TryGetValue(source, out List<StructureDefinitionElementGraphEdge>? edges) ? edges : []
        where edge.Direction == direction
        select (edge.Target, edge);

    public List<StructureDefinitionComponentGraphCell?[]> Project(StructureDefinitionGraphCell keyCell, ElementDefinition keyComponent)
    {
        StructureDefinitionComponentGraphCell?[] row = new StructureDefinitionComponentGraphCell?[_sourceRow.Length];
        int startCol = Array.IndexOf(_sourceRow, keyCell);
        row[startCol] = new()
        {
            StructureDefinitionCell = keyCell,
            Component = keyComponent,
        };

        List<StructureDefinitionComponentGraphCell?[]> upwards = [];

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

        List<StructureDefinitionComponentGraphCell?[]> results = [];

        // project down
        if (startCol > 0)
        {
            foreach (StructureDefinitionComponentGraphCell?[] partial in upwards)
            {
                results.AddRange(project(partial, startCol, ComparisonDirection.Down));
            }
        }

        return results;
    }

    private List<StructureDefinitionComponentGraphCell?[]> project(
        StructureDefinitionComponentGraphCell?[] incomingRow,
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

        IReadOnlyList<(ElementDefinition target, StructureDefinitionElementGraphEdge via)> neighbors = getNeighbors(incomingRow[column]!.Component, direction).ToList();

        if (neighbors.Count == 0)
        {
            return [incomingRow];
        }

        List<StructureDefinitionComponentGraphCell?[]> results = [];

        // iterate over neighbors in this direction
        foreach ((ElementDefinition target, StructureDefinitionElementGraphEdge via) in neighbors)
        {
            StructureDefinitionComponentGraphCell?[] row = neighbors.Count == 1 ? incomingRow : incomingRow.DeepClone().ToArray();

            if (direction == ComparisonDirection.Up)
            {
                row[nextCol] = new()
                {
                    StructureDefinitionCell = _sourceRow[nextCol]!,
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
                    StructureDefinitionCell = _sourceRow[nextCol]!,
                    Component = target,
                    RightCell = row[column],
                    RightEdge = via,
                };
                row[column]!.LeftCell = row[nextCol];
                row[column]!.LeftEdge = via;
            }

            // recurse
            List<StructureDefinitionComponentGraphCell?[]> completedRows = completedRows = project(row, nextCol, direction);

            // add to our current set of results
            results.AddRange(completedRows);
        }

        return results;
    }
}


