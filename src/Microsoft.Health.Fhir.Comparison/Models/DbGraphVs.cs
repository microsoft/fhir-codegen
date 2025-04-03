using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.ElementModel.Types;

namespace Microsoft.Health.Fhir.Comparison.Models;

public record class DbVsCell : ICloneable
{
    private readonly List<DbValueSetConcept> _concepts = null!;
    private readonly ILookup<int, DbValueSetConcept> _conceptsByKey = null!;

    public required DbFhirPackage FhirPackage { get; init; }
    public required DbValueSet Vs { get; init; }
    public required List<DbValueSetConcept> Concepts
    {
        get => _concepts;
        init
        {
            _concepts = value;
            _conceptsByKey = value.ToLookup((c) => c.Key);
        }
    }

    public DbVsCell? LeftCell { get; set; } = null;
    public DbValueSet? LeftVs { get; set; } = null;
    public DbValueSetComparison? LeftComparison { get; set; } = null;

    public DbVsCell? RightCell { get; set; } = null;
    public DbValueSet? RightVs { get; set; } = null;
    public DbValueSetComparison? RightComparison { get; set; } = null;

    object ICloneable.Clone() => this with { };
}

public record class DbVsConceptCell : ICloneable
{
    public required DbVsCell VsCell { get; set; }
    public required DbValueSetConcept Concept { get; set; }

    public DbVsConceptCell? LeftCell { get; set; } = null;
    public DbValueSetConcept? LeftConcept { get; set; } = null;
    public DbValueSetConceptComparison? LeftComparison { get; set; } = null;

    public DbVsConceptCell? RightCell { get; set; } = null;
    public DbValueSetConcept? RightConcept { get; set; } = null;
    public DbValueSetConceptComparison? RightComparison { get; set; } = null;

    object ICloneable.Clone() => this with { };
}

public class DbGraphVs
{
    private readonly IDbConnection _db = null!;
    private readonly List<DbFhirPackage> _packages = null!;
    private readonly DbValueSet _keyVs = null!;
    private int _keyCol = -1;

    public required IDbConnection DB { get => _db; init => _db = value; }
    public required List<DbFhirPackage> Packages { get => _packages; init => _packages = value; }
    public required DbValueSet KeyVs { get => _keyVs; init => _keyVs = value; }

    public List<DbVsCell?[]> Project()
    {
        DbVsCell?[] row = new DbVsCell?[_packages.Count];
        int startCol = _packages.FindIndex((fp) => fp.Key == _keyVs.FhirPackageKey);
        row[startCol] = new()
        {
            FhirPackage = _packages[startCol],
            Vs = _keyVs,
            Concepts = DbValueSetConcept.SelectList(_db, ValueSetKey: _keyVs.Key),
        };

        _keyCol = startCol;

        List<DbVsCell?[]> right = [];

        // project right
        if (startCol < _packages.Count)
        {
            right = projectVs(row, startCol, true);
        }

        // if we started at the first definition, we are done
        if (startCol == 0)
        {
            return right;
        }

        List<DbVsCell?[]> results = [];

        // project left
        foreach (DbVsCell?[] r in right)
        {
            results.AddRange(projectVs(r, startCol, false));
        }

        return results;
    }

    private List<DbVsCell?[]> projectVs(
        DbVsCell?[] incomingRow,
        int column,
        bool projectRight)
    {
        if (incomingRow[column] == null)
        {
            return [incomingRow];
        }

        int nextCol = projectRight ? column + 1 : column - 1;
        if ((nextCol < 0) || (nextCol >= incomingRow.Length))
        {
            return [incomingRow];
        }

        // look for comparisons from this value set to the next package
        List<DbValueSetComparison> edges = DbValueSetComparison.SelectList(
            _db,
            SourceValueSetKey: incomingRow[column]!.Vs.Key,
            TargetFhirPackageKey: _packages[nextCol].Key,
            orderByProperties: [nameof(DbValueSetComparison.TargetValueSetKey)]);

        if (edges.Count == 0)
        {
            return [incomingRow];
        }

        List<DbVsCell?[]> results = [];

        // iterate over our neighbors
        foreach (DbValueSetComparison edge in edges)
        {
            DbValueSetComparison? inverseEdge = edge.InverseComparisonKey == null
                ? null
                : DbValueSetComparison.SelectSingle(_db, Key: edge.InverseComparisonKey);

            DbVsCell?[] row = edges.Count == 1 ? incomingRow : (DbVsCell?[])incomingRow.Clone();
            if (projectRight == true)
            {
                row[nextCol] = new()
                {
                    FhirPackage = _packages[nextCol],
                    Vs = DbValueSet.SelectSingle(_db, Key: edge.TargetValueSetKey) ?? throw new Exception($"Failed to resolve compared ValueSet: {edge.TargetValueSetKey}!"),
                    Concepts = DbValueSetConcept.SelectList(_db, ValueSetKey: edge.TargetValueSetKey, orderByProperties: [nameof(DbValueSetConcept.System), nameof(DbValueSetConcept.Code)]),
                    LeftCell = incomingRow[column]!,
                    LeftVs = incomingRow[column]!.Vs,
                    LeftComparison = inverseEdge,
                };

                row[column]!.RightCell = row[nextCol];
                row[column]!.RightVs = row[nextCol]!.Vs;
                row[column]!.RightComparison = edge;
            }
            else
            {
                row[nextCol] = new()
                {
                    FhirPackage = _packages[nextCol],
                    Vs = DbValueSet.SelectSingle(_db, Key: edge.TargetValueSetKey) ?? throw new Exception($"Failed to resolve compared ValueSet: {edge.TargetValueSetKey}!"),
                    Concepts = DbValueSetConcept.SelectList(_db, ValueSetKey: edge.TargetValueSetKey, orderByProperties: [nameof(DbValueSetConcept.System), nameof(DbValueSetConcept.Code)]),
                    RightCell = incomingRow[column]!,
                    RightVs = incomingRow[column]!.Vs,
                    RightComparison = inverseEdge,
                };

                row[column]!.LeftCell = row[nextCol];
                row[column]!.LeftVs = row[nextCol]!.Vs;
                row[column]!.LeftComparison = edge;
            }

            // recurse
            List<DbVsCell?[]> next = projectVs(row, nextCol, projectRight);

            // combine results
            results.AddRange(next);
        }

        return results;
    }

    public List<DbVsConceptCell?[]> ProjectConcepts(DbVsCell?[] vsRow, int? keyColumnIndex = null)
    {
        int keyColIndex = keyColumnIndex ??= _keyCol;

        if (keyColIndex == -1)
        {
            throw new Exception("Key column not set!");
        }

        if (vsRow[keyColIndex] == null)
        {
            return [];
        }

        List<DbVsConceptCell?[]> results = [];

        // iterate over the concepts for this value set
        foreach (DbValueSetConcept concept in vsRow[keyColIndex]!.Concepts)
        {
            DbVsConceptCell?[] row = new DbVsConceptCell?[_packages.Count];

            int startCol = keyColIndex;
            row[startCol] = new()
            {
                VsCell = vsRow[startCol]!,
                Concept = concept,
            };

            List<DbVsConceptCell?[]> right = [];

            // project right
            if (startCol < _packages.Count)
            {
                right = projectConcept(vsRow, row, startCol, true);
            }

            // if we started at the first definition, we are done
            if (startCol == 0)
            {
                results.AddRange(right);
                continue;
            }

            // project left and add as we go
            foreach (DbVsConceptCell?[] r in right)
            {
                results.AddRange(projectConcept(vsRow, r, startCol, false));
            }
        }

        return results;
    }

    private List<DbVsConceptCell?[]> projectConcept(
        DbVsCell?[] vsRow,
        DbVsConceptCell?[] incomingRow,
        int column,
        bool projectRight)
    {
        if ((incomingRow[column] == null) ||
            (vsRow[column] == null))
        {
            return [incomingRow];
        }

        int nextCol = projectRight ? column + 1 : column - 1;
        if ((nextCol < 0) || (nextCol >= incomingRow.Length))
        {
            return [incomingRow];
        }

        if (projectRight &&
            ((vsRow[column]!.RightCell == null) || (vsRow[column]!.RightComparison == null)))
        {
            return [incomingRow];
        }

        if ((!projectRight) &&
            ((vsRow[column]!.LeftCell == null) || (vsRow[column]!.LeftComparison == null)))
        {
            return [incomingRow];
        }

        int comparisonKey = projectRight
            ? vsRow[column]!.RightComparison!.Key
            : vsRow[column]!.LeftComparison!.Key;

        // look for the concept comparisons for this ValueSet comparison and concept
        List<DbValueSetConceptComparison> edges = DbValueSetConceptComparison.SelectList(
            _db,
            ValueSetComparisonKey: comparisonKey,
            SourceConceptKey: incomingRow[column]!.Concept.Key,
            orderByProperties: [nameof(DbValueSetConceptComparison.TargetConceptKey)]);

        if (edges.Count == 0)
        {
            return [incomingRow];
        }

        if ((edges.Count == 1) &&
            (edges[0].NoMap == true))
        {
            if (projectRight)
            {
                incomingRow[column]!.RightComparison = edges[0];
            }
            else
            {
                incomingRow[column]!.LeftComparison = edges[0];
            }

            return [incomingRow];
        }

        List<DbVsConceptCell?[]> results = [];

        // iterate over our neighbors
        foreach (DbValueSetConceptComparison edge in edges)
        {
            // resolve the concept
            DbValueSetConcept concept = DbValueSetConcept.SelectSingle(_db, Key: edge.TargetConceptKey)
                ?? throw new Exception($"Failed to resolve compared concept: {edge.TargetConceptKey}!");

            DbValueSetConceptComparison? inverseEdge = edge.InverseComparisonKey == null
                ? null
                : DbValueSetConceptComparison.SelectSingle(_db, Key: edge.InverseComparisonKey);

            DbVsConceptCell?[] row = edges.Count == 1 ? incomingRow : (DbVsConceptCell?[])incomingRow.Clone();
            if (projectRight == true)
            {
                row[nextCol] = new()
                {
                    VsCell = vsRow[nextCol]!,
                    Concept = concept,
                    LeftCell = incomingRow[column]!,
                    LeftConcept = incomingRow[column]!.Concept,
                    LeftComparison = inverseEdge,
                };

                row[column]!.RightCell = row[nextCol];
                row[column]!.RightConcept = row[nextCol]!.Concept;
                row[column]!.RightComparison = edge;
            }
            else
            {
                row[nextCol] = new()
                {
                    VsCell = vsRow[nextCol]!,
                    Concept = concept,
                    RightCell = incomingRow[column]!,
                    RightConcept = incomingRow[column]!.Concept,
                    RightComparison = inverseEdge,
                };

                row[column]!.LeftCell = row[nextCol];
                row[column]!.RightConcept = row[nextCol]!.Concept;
                row[column]!.LeftComparison = edge;
            }

            // recurse
            List<DbVsConceptCell?[]> next = projectConcept(vsRow, row, nextCol, projectRight);

            // combine results
            results.AddRange(next);
        }

        return results;
    }
}
