using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
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

public class DbVsRow : IEnumerable<DbVsCell?>
{
    private readonly int _rowNumber;
    private readonly int _keyCol;
    private readonly DbVsCell?[] _cells;

    public DbVsRow(DbVsCell?[] cells, int rowNumber)
    {
        _cells = cells;
        _keyCol = Array.FindIndex(cells, cell => cell != null);
        _rowNumber = rowNumber;
    }
    public DbVsRow(DbVsCell?[] cells, int rowNumber, int keyCol = -1)
    {
        _cells = cells;
        _keyCol = keyCol;
        _rowNumber = rowNumber;
    }
    public DbVsRow(int size, int keyCol, int rowNumber)
    {
        _cells = new DbVsCell?[size];
        _keyCol = keyCol;
        _rowNumber = rowNumber;
    }

    public int RowNumber => _rowNumber;
    public int KeyCol => _keyCol;
    public DbVsCell? KeyCell => _keyCol >= 0 ? _cells[_keyCol] : null;
    public DbVsCell?[] Cells => _cells;

    public int Length => _cells.Length;

    // Add indexer to access cells directly
    public DbVsCell? this[int index]
    {
        get
        {
            if (index >= 0 && index < _cells.Length)
            {
                return _cells[index];
            }

            return null;
        }
        set
        {
            if (index >= 0 && index < _cells.Length)
            {
                _cells[index] = value;
            }
        }
    }

    // Indexer to find a cell by package
    public DbVsCell? this[DbFhirPackage package]
    {
        get
        {
            if (package == null)
                return null;

            return _cells.FirstOrDefault(cell =>
                cell?.FhirPackage != null && cell.FhirPackage.Key == package.Key);
        }
    }

    public DbVsRow DeepCopy(int? newRowNumber = null)
    {
        DbVsCell?[] cells = new DbVsCell?[_cells.Length];
        for (int i = 0; i < _cells.Length; i++)
        {
            cells[i] = _cells[i] == null ? null : _cells[i]! with { };
        }
        return new DbVsRow(cells, newRowNumber ?? _rowNumber, KeyCol);
    }

    public DbVsRow ShallowCopy(int? newRowNumber = null)
    {
        DbVsCell?[] cells = new DbVsCell?[_cells.Length];
        for (int i = 0; i < _cells.Length; i++)
        {
            cells[i] = _cells[i];
        }
        return new DbVsRow(cells, newRowNumber ?? _rowNumber, KeyCol);
    }

    public DbVsRow ShallowCopy(int? newRowNumber, params int[] onlyCopyRows)
    {
        DbVsCell?[] cells = new DbVsCell?[_cells.Length];
        for (int i = 0; i < _cells.Length; i++)
        {
            if (onlyCopyRows.Contains(i))
                cells[i] = _cells[i];
            else
                cells[i] = null;
        }
        return new DbVsRow(cells, newRowNumber ?? _rowNumber, KeyCol);
    }

    public IEnumerator<DbVsCell?> GetEnumerator() => ((IEnumerable<DbVsCell?>)_cells).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _cells.GetEnumerator();
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

public class DbVsConceptRow : IEnumerable<DbVsConceptCell?>
{
    private readonly int _rowNumber;
    private readonly int _keyCol;
    private readonly DbVsConceptCell?[] _cells;

    public DbVsConceptRow(DbVsConceptCell?[] cells, int rowNumber)
    {
        _cells = cells;
        _keyCol = Array.FindIndex(cells, cell => cell != null);
        _rowNumber = rowNumber;
    }
    public DbVsConceptRow(DbVsConceptCell?[] cells, int rowNumber, int keyCol = -1)
    {
        _cells = cells;
        _keyCol = keyCol;
        _rowNumber = rowNumber;
    }
    public DbVsConceptRow(int size, int keyCol, int rowNumber)
    {
        _cells = new DbVsConceptCell?[size];
        _keyCol = keyCol;
        _rowNumber = rowNumber;
    }

    public int RowNumber => _rowNumber;
    public int KeyCol => _keyCol;
    public DbVsConceptCell? KeyCell => _keyCol >= 0 ? _cells[_keyCol] : null;
    public DbVsConceptCell?[] Cells => _cells;
    public int Length => _cells.Length;

    // Add indexer to access cells directly
    public DbVsConceptCell? this[int index]
    {
        get
        {
            if (index >= 0 && index < _cells.Length)
                return _cells[index];
            return null;
        }
        set
        {
            if (index >= 0 && index < _cells.Length)
                _cells[index] = value;
        }
    }
    // Indexer to find a cell by package
    public DbVsConceptCell? this[DbFhirPackage package]
    {
        get
        {
            if (package == null)
                return null;
            return _cells.FirstOrDefault(cell =>
                cell?.VsCell.FhirPackage != null && cell.VsCell.FhirPackage.Key == package.Key);
        }
    }

    public DbVsConceptRow DeepCopy(int? newRowNumber = null)
    {
        DbVsConceptCell?[] cells = new DbVsConceptCell?[_cells.Length];
        for (int i = 0; i < _cells.Length; i++)
        {
            cells[i] = _cells[i] == null ? null : _cells[i]! with { };
        }
        return new DbVsConceptRow(cells, newRowNumber ?? _rowNumber, KeyCol);
    }

    public DbVsConceptRow ShallowCopy(int? newRowNumber = null)
    {
        DbVsConceptCell?[] cells = new DbVsConceptCell?[_cells.Length];
        for (int i = 0; i < _cells.Length; i++)
        {
            cells[i] = _cells[i];
        }
        return new DbVsConceptRow(cells, newRowNumber ?? _rowNumber, KeyCol);
    }

    public DbVsConceptRow ShallowCopy(int? newRowNumber, params int[] onlyCopyRows)
    {
        DbVsConceptCell?[] cells = new DbVsConceptCell?[_cells.Length];
        for (int i = 0; i < _cells.Length; i++)
        {
            if (onlyCopyRows.Contains(i))
                cells[i] = _cells[i];
            else
                cells[i] = null;
        }
        return new DbVsConceptRow(cells, newRowNumber ?? _rowNumber, KeyCol);
    }

    public IEnumerator<DbVsConceptCell?> GetEnumerator() => ((IEnumerable<DbVsConceptCell?>)_cells).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _cells.GetEnumerator();
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

    public List<DbVsRow> Project()
    {
        int rowNumber = 0;
        int startCol = _packages.FindIndex((fp) => fp.Key == _keyVs.FhirPackageKey);
        _keyCol = startCol;

        DbVsRow row = new DbVsRow(_packages.Count, _keyCol, rowNumber++);
        row[startCol] = new()
        {
            FhirPackage = _packages[startCol],
            Vs = _keyVs,
            Concepts = DbValueSetConcept.SelectList(_db, ValueSetKey: _keyVs.Key),
        };

        List<DbVsRow> right = [];

        // project right
        if (startCol < _packages.Count)
        {
            right = projectVs(row, startCol, true, ref rowNumber);
        }

        // if we started at the first definition, we are done
        if (startCol == 0)
        {
            return right;
        }

        List<DbVsRow> results = [];

        // project left
        foreach (DbVsRow r in right)
        {
            results.AddRange(projectVs(r, startCol, false, ref rowNumber));
        }

        return results;
    }

    private List<DbVsRow> projectVs(
        DbVsRow incomingRow,
        int column,
        bool projectRight,
        ref int rowNumber)
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

        List<DbVsRow> results = [];

        // iterate over our neighbors
        foreach (DbValueSetComparison edge in edges)
        {
            DbValueSetComparison? inverseEdge = edge.InverseComparisonKey == null
                ? null
                : DbValueSetComparison.SelectSingle(_db, Key: edge.InverseComparisonKey);

            DbVsRow row = edges.Count == 1 ? incomingRow : incomingRow.DeepCopy(rowNumber++);
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
            List<DbVsRow> next = projectVs(row, nextCol, projectRight, ref rowNumber);

            // combine results
            results.AddRange(next);
        }

        return results;
    }

    public List<DbVsConceptRow> ProjectConcepts(DbVsRow vsRow, int? keyColumnIndex = null)
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

        int rowIndex = 0;
        List<DbVsConceptRow> results = [];

        // iterate over the concepts for this value set
        foreach (DbValueSetConcept concept in vsRow[keyColIndex]!.Concepts)
        {
            DbVsConceptRow row = new(_packages.Count, _keyCol, rowIndex++);

            int startCol = keyColIndex;
            row[startCol] = new()
            {
                VsCell = vsRow[startCol]!,
                Concept = concept,
            };

            List<DbVsConceptRow> right = [];

            // project right
            if (startCol < _packages.Count)
            {
                right = projectConcept(vsRow, row, startCol, true, ref rowIndex);
            }

            // if we started at the first definition, we are done
            if (startCol == 0)
            {
                results.AddRange(right);
                continue;
            }

            // project left and add as we go
            foreach (DbVsConceptRow r in right)
            {
                results.AddRange(projectConcept(vsRow, r, startCol, false, ref rowIndex));
            }
        }

        return results;
    }

    private List<DbVsConceptRow> projectConcept(
        DbVsRow vsRow,
        DbVsConceptRow incomingRow,
        int column,
        bool projectRight,
        ref int rowNumber)
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

        List<DbVsConceptRow> results = [];

        // iterate over our neighbors
        foreach (DbValueSetConceptComparison edge in edges)
        {
            // resolve the concept
            DbValueSetConcept concept = DbValueSetConcept.SelectSingle(_db, Key: edge.TargetConceptKey)
                ?? throw new Exception($"Failed to resolve compared concept: {edge.TargetConceptKey}!");

            DbValueSetConceptComparison? inverseEdge = edge.InverseComparisonKey == null
                ? null
                : DbValueSetConceptComparison.SelectSingle(_db, Key: edge.InverseComparisonKey);

            DbVsConceptRow row = edges.Count == 1 ? incomingRow : incomingRow.DeepCopy(rowNumber++);
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
            List<DbVsConceptRow> next = projectConcept(vsRow, row, nextCol, projectRight, ref rowNumber);

            // combine results
            results.AddRange(next);
        }

        return results;
    }
}
