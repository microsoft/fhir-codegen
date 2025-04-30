using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Health.Fhir.Comparison.Models;

public record class DbSdCell : ICloneable
{
    private readonly List<DbElement> _elements = null!;
    private readonly ILookup<int, DbElement> _elementsByKey = null!;

    public required DbFhirPackage FhirPackage { get; init; }
    public required DbStructureDefinition Sd { get; init; }
    public required List<DbElement> Elements
    {
        get => _elements;
        init
        {
            _elements = value;
            _elementsByKey = value.ToLookup((c) => c.Key);
        }
    }

    public DbSdCell? LeftCell { get; set; } = null;
    public DbStructureDefinition? LeftSd { get; set; } = null;
    public DbStructureComparison? LeftComparison { get; set; } = null;

    public DbSdCell? RightCell { get; set; } = null;
    public DbStructureDefinition? RightSd { get; set; } = null;
    public DbStructureComparison? RightComparison { get; set; } = null;

    public BidirectionalRelationshipCodes? BidirectionalRight
    {
        get
        {
            if ((RightComparison == null) || (RightCell?.LeftComparison == null))
            {
                return null;
            }

            Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? up = RightComparison.Relationship;
            Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? down = RightCell.LeftComparison.Relationship;

            if ((up == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.Equivalent) &&
                (down == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.Equivalent))
            {
                return BidirectionalRelationshipCodes.Equivalent;
            }

            if ((up == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.SourceIsNarrowerThanTarget) &&
                (down == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.SourceIsBroaderThanTarget))
            {
                return BidirectionalRelationshipCodes.NewerNarrows;
            }

            if ((up == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.SourceIsBroaderThanTarget) &&
                (down == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.SourceIsNarrowerThanTarget))
            {
                return BidirectionalRelationshipCodes.NewerBroadens;
            }

            if ((up == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.RelatedTo) &&
                (down == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.RelatedTo))
            {
                return BidirectionalRelationshipCodes.Related;
            }

            if ((up == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.NotRelatedTo) &&
                (down == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.NotRelatedTo))
            {
                return BidirectionalRelationshipCodes.NotRelated;
            }

            return BidirectionalRelationshipCodes.Mismatched;
        }
    }

    public BidirectionalRelationshipCodes? BidirectionalLeft
    {
        get
        {
            if ((LeftComparison == null) || (LeftCell?.RightComparison == null))
            {
                return null;
            }

            Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? up = LeftCell.RightComparison.Relationship;
            Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? down = LeftComparison.Relationship;

            if ((up == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.Equivalent) &&
                (down == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.Equivalent))
            {
                return BidirectionalRelationshipCodes.Equivalent;
            }

            if ((up == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.SourceIsNarrowerThanTarget) &&
                (down == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.SourceIsBroaderThanTarget))
            {
                return BidirectionalRelationshipCodes.NewerBroadens;
            }

            if ((up == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.SourceIsBroaderThanTarget) &&
                (down == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.SourceIsNarrowerThanTarget))
            {
                return BidirectionalRelationshipCodes.NewerNarrows;
            }

            if ((up == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.RelatedTo) &&
                (down == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.RelatedTo))
            {
                return BidirectionalRelationshipCodes.Related;
            }

            if ((up == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.NotRelatedTo) &&
                (down == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.NotRelatedTo))
            {
                return BidirectionalRelationshipCodes.NotRelated;
            }

            return BidirectionalRelationshipCodes.Mismatched;
        }
    }

    object ICloneable.Clone() => this with { };

    public override string ToString() => FhirPackage.ShortName + ":" + Sd.Name;
}


public class DbSdRow : IEnumerable<DbSdCell?>
{
    private readonly int _rowNumber;
    private readonly int _keyCol;
    private readonly DbSdCell?[] _cells;

    public DbSdRow(DbSdCell?[] cells, int rowNumber)
    {
        _cells = cells;
        _keyCol = Array.FindIndex(cells, cell => cell != null);
        _rowNumber = rowNumber;
    }
    public DbSdRow(DbSdCell?[] cells, int rowNumber, int keyCol = -1)
    {
        _cells = cells;
        _keyCol = keyCol;
        _rowNumber = rowNumber;
    }
    public DbSdRow(int size, int keyCol, int rowNumber)
    {
        _cells = new DbSdCell?[size];
        _keyCol = keyCol;
        _rowNumber = rowNumber;
    }

    public int RowNumber => _rowNumber;
    public int KeyCol => _keyCol;
    public DbSdCell? KeyCell => _keyCol >= 0 ? _cells[_keyCol] : null;
    public DbSdCell?[] Cells => _cells;

    public int Length => _cells.Length;

    public override string ToString() => string.Join(" - ", _cells.Select(c => c?.ToString() ?? string.Empty));

    // Add indexer to access cells directly
    public DbSdCell? this[int index]
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
    public DbSdCell? this[DbFhirPackage package]
    {
        get
        {
            if (package == null)
                return null;

            return _cells.FirstOrDefault(cell =>
                cell?.FhirPackage != null && cell.FhirPackage.Key == package.Key);
        }
    }

    public DbSdRow DeepCopy(int? newRowNumber = null)
    {
        DbSdCell?[] cells = new DbSdCell?[_cells.Length];
        for (int i = 0; i < _cells.Length; i++)
        {
            cells[i] = _cells[i] == null ? null : _cells[i]! with { };
        }
        return new DbSdRow(cells, newRowNumber ?? _rowNumber, KeyCol);
    }

    public DbSdRow ShallowCopy(int? newRowNumber = null)
    {
        DbSdCell?[] cells = new DbSdCell?[_cells.Length];
        for (int i = 0; i < _cells.Length; i++)
        {
            cells[i] = _cells[i];
        }
        return new DbSdRow(cells, newRowNumber ?? _rowNumber, KeyCol);
    }

    public DbSdRow ShallowCopy(int? newRowNumber, params int[] onlyCopyRows)
    {
        DbSdCell?[] cells = new DbSdCell?[_cells.Length];
        for (int i = 0; i < _cells.Length; i++)
        {
            if (onlyCopyRows.Contains(i))
                cells[i] = _cells[i];
            else
                cells[i] = null;
        }
        return new DbSdRow(cells, newRowNumber ?? _rowNumber, KeyCol);
    }

    public IEnumerator<DbSdCell?> GetEnumerator() => ((IEnumerable<DbSdCell?>)_cells).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _cells.GetEnumerator();
}


public record class DbElementCell : ICloneable
{
    public required DbSdCell SdCell { get; set; }
    public required DbElement Element { get; set; }

    public DbElementCell? LeftCell { get; set; } = null;
    public DbElement? LeftElement { get; set; } = null;
    public DbElementComparison? LeftComparison { get; set; } = null;

    public DbElementCell? RightCell { get; set; } = null;
    public DbElement? RightElement { get; set; } = null;
    public DbElementComparison? RightComparison { get; set; } = null;

    public string ToRightMessage => (RightComparison == null)
        ? string.Empty
        : $"{RightComparison.Relationship}: {RightComparison.Message}";

    public string FromRightMessage => (RightCell?.LeftComparison == null)
        ? string.Empty
        : $"{RightCell.LeftComparison.Relationship}: {RightCell.LeftComparison.Message}";
    
    public string ToLeftMessage => (LeftComparison == null)
        ? string.Empty
        : $"{LeftComparison.Relationship}: {LeftComparison.Message}";
    public string FromLeftMessage => (LeftCell?.RightComparison == null)
        ? string.Empty
        : $"{LeftCell.RightComparison.Relationship}: {LeftCell.RightComparison.Message}";

    public BidirectionalRelationshipCodes? BidirectionalRight
    {
        get
        {
            if ((RightComparison == null) || (RightCell?.LeftComparison == null))
            {
                return null;
            }

            Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? up = RightComparison.Relationship;
            Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? down = RightCell.LeftComparison.Relationship;

            if ((up == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.Equivalent) &&
                (down == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.Equivalent))
            {
                return BidirectionalRelationshipCodes.Equivalent;
            }

            if ((up == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.SourceIsNarrowerThanTarget) &&
                (down == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.SourceIsBroaderThanTarget))
            {
                return BidirectionalRelationshipCodes.NewerBroadens;
            }

            if ((up == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.SourceIsBroaderThanTarget) &&
                (down == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.SourceIsNarrowerThanTarget))
            {
                return BidirectionalRelationshipCodes.NewerNarrows;
            }

            if ((up == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.RelatedTo) &&
                (down == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.RelatedTo))
            {
                return BidirectionalRelationshipCodes.Related;
            }

            if ((up == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.NotRelatedTo) &&
                (down == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.NotRelatedTo))
            {
                return BidirectionalRelationshipCodes.NotRelated;
            }

            return BidirectionalRelationshipCodes.Mismatched;
        }
    }

    public BidirectionalRelationshipCodes? BidirectionalLeft
    {
        get
        {
            if ((LeftComparison == null) || (LeftCell?.RightComparison == null))
            {
                return null;
            }

            Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? up = LeftCell.RightComparison.Relationship;
            Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? down = LeftComparison.Relationship;

            if ((up == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.Equivalent) &&
                (down == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.Equivalent))
            {
                return BidirectionalRelationshipCodes.Equivalent;
            }

            if ((up == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.SourceIsNarrowerThanTarget) &&
                (down == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.SourceIsBroaderThanTarget))
            {
                return BidirectionalRelationshipCodes.NewerBroadens;
            }

            if ((up == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.SourceIsBroaderThanTarget) &&
                (down == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.SourceIsNarrowerThanTarget))
            {
                return BidirectionalRelationshipCodes.NewerNarrows;
            }

            if ((up == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.RelatedTo) &&
                (down == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.RelatedTo))
            {
                return BidirectionalRelationshipCodes.Related;
            }

            if ((up == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.NotRelatedTo) &&
                (down == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.NotRelatedTo))
            {
                return BidirectionalRelationshipCodes.NotRelated;
            }


            return BidirectionalRelationshipCodes.Mismatched;
        }
    }

    object ICloneable.Clone() => this with { };
}


public class DbElementRow : IEnumerable<DbElementCell?>
{
    private readonly int _rowNumber;
    private readonly int _keyCol;
    private readonly DbElementCell?[] _cells;

    public DbElementRow(DbElementCell?[] cells, int rowNumber)
    {
        _cells = cells;
        _keyCol = Array.FindIndex(cells, cell => cell != null);
        _rowNumber = rowNumber;
    }
    public DbElementRow(DbElementCell?[] cells, int rowNumber, int keyCol = -1)
    {
        _cells = cells;
        _keyCol = keyCol;
        _rowNumber = rowNumber;
    }
    public DbElementRow(int size, int keyCol, int rowNumber)
    {
        _cells = new DbElementCell?[size];
        _keyCol = keyCol;
        _rowNumber = rowNumber;
    }

    public int RowNumber => _rowNumber;
    public int KeyCol => _keyCol;
    public DbElementCell? KeyCell => _keyCol >= 0 ? _cells[_keyCol] : null;
    public DbElementCell?[] Cells => _cells;
    public int Length => _cells.Length;

    // Add indexer to access cells directly
    public DbElementCell? this[int index]
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
    public DbElementCell? this[DbFhirPackage package]
    {
        get
        {
            if (package == null)
                return null;
            return _cells.FirstOrDefault(cell =>
                cell?.SdCell.FhirPackage != null && cell.SdCell.FhirPackage.Key == package.Key);
        }
    }

    public DbElementRow DeepCopy(int? newRowNumber = null)
    {
        DbElementCell?[] cells = new DbElementCell?[_cells.Length];
        for (int i = 0; i < _cells.Length; i++)
        {
            cells[i] = _cells[i] == null ? null : _cells[i]! with { };
        }
        return new DbElementRow(cells, newRowNumber ?? _rowNumber, KeyCol);
    }

    public DbElementRow ShallowCopy(int? newRowNumber = null)
    {
        DbElementCell?[] cells = new DbElementCell?[_cells.Length];
        for (int i = 0; i < _cells.Length; i++)
        {
            cells[i] = _cells[i];
        }
        return new DbElementRow(cells, newRowNumber ?? _rowNumber, KeyCol);
    }

    public DbElementRow ShallowCopy(int? newRowNumber, params int[] onlyCopyRows)
    {
        DbElementCell?[] cells = new DbElementCell?[_cells.Length];
        for (int i = 0; i < _cells.Length; i++)
        {
            if (onlyCopyRows.Contains(i))
                cells[i] = _cells[i];
            else
                cells[i] = null;
        }
        return new DbElementRow(cells, newRowNumber ?? _rowNumber, KeyCol);
    }

    public IEnumerator<DbElementCell?> GetEnumerator() => ((IEnumerable<DbElementCell?>)_cells).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _cells.GetEnumerator();
}

public class DbGraphSd
{
    private readonly IDbConnection _db = null!;
    private readonly List<DbFhirPackage> _packages = null!;
    private readonly DbStructureDefinition _keySd = null!;
    private int _keyCol = -1;

    public required IDbConnection DB { get => _db; init => _db = value; }
    public required List<DbFhirPackage> Packages { get => _packages; init => _packages = value; }
    public required DbStructureDefinition KeySd { get => _keySd; init => _keySd = value; }


    public List<DbSdRow> Project()
    {
        int rowNumber = 0;
        int startCol = _packages.FindIndex((fp) => fp.Key == _keySd.FhirPackageKey);
        _keyCol = startCol;

        DbSdRow row = new DbSdRow(_packages.Count, _keyCol, rowNumber++);
        row[startCol] = new()
        {
            FhirPackage = _packages[startCol],
            Sd = _keySd,
            Elements = DbElement.SelectList(_db, StructureKey: _keySd.Key),
        };

        List<DbSdRow> right = [];

        // project right
        if (startCol < _packages.Count)
        {
            right = projectSd(row, startCol, true, ref rowNumber);
        }

        // if we started at the first definition, we are done
        if (startCol == 0)
        {
            return right;
        }

        List<DbSdRow> results = [];

        // project left
        foreach (DbSdRow r in right)
        {
            results.AddRange(projectSd(r, startCol, false, ref rowNumber));
        }

        return results;
    }

    private List<DbSdRow> projectSd(
        DbSdRow incomingRow,
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
        List<DbStructureComparison> edges = DbStructureComparison.SelectList(
            _db,
            SourceStructureKey: incomingRow[column]!.Sd.Key,
            TargetFhirPackageKey: _packages[nextCol].Key,
            orderByProperties: [nameof(DbStructureComparison.TargetStructureKey)]);

        if (edges.Count == 0)
        {
            return [incomingRow];
        }

        List<DbSdRow> results = [];

        // iterate over our neighbors
        foreach (DbStructureComparison edge in edges)
        {
            DbStructureComparison? inverseEdge = edge.InverseComparisonKey == null
                ? null
                : DbStructureComparison.SelectSingle(_db, Key: edge.InverseComparisonKey);

            DbSdRow row = edges.Count == 1 ? incomingRow : incomingRow.DeepCopy(rowNumber++);
            if (projectRight == true)
            {
                row[nextCol] = new()
                {
                    FhirPackage = _packages[nextCol],
                    Sd = DbStructureDefinition.SelectSingle(_db, Key: edge.TargetStructureKey) ?? throw new Exception($"Failed to resolve compared Structure: {edge.TargetStructureKey}!"),
                    Elements = DbElement.SelectList(_db, StructureKey: edge.TargetStructureKey, orderByProperties: [nameof(DbElement.Id)]),
                    LeftCell = incomingRow[column]!,
                    LeftSd = incomingRow[column]!.Sd,
                    LeftComparison = inverseEdge,
                };

                row[column]!.RightCell = row[nextCol];
                row[column]!.RightSd = row[nextCol]!.Sd;
                row[column]!.RightComparison = edge;
            }
            else
            {
                row[nextCol] = new()
                {
                    FhirPackage = _packages[nextCol],
                    Sd = DbStructureDefinition.SelectSingle(_db, Key: edge.TargetStructureKey) ?? throw new Exception($"Failed to resolve compared Structure: {edge.TargetStructureKey}!"),
                    Elements = DbElement.SelectList(_db, StructureKey: edge.TargetStructureKey, orderByProperties: [nameof(DbElement.Id)]),
                    RightCell = incomingRow[column]!,
                    RightSd = incomingRow[column]!.Sd,
                    RightComparison = inverseEdge,
                };

                row[column]!.LeftCell = row[nextCol];
                row[column]!.LeftSd = row[nextCol]!.Sd;
                row[column]!.LeftComparison = edge;
            }

            // recurse
            List<DbSdRow> next = projectSd(row, nextCol, projectRight, ref rowNumber);

            // combine results
            results.AddRange(next);
        }

        return results;
    }

    public List<DbElementRow> ProjectElements(DbSdRow sdRow, int? keyColumnIndex = null)
    {
        int keyColIndex = keyColumnIndex ??= _keyCol;

        if (keyColIndex == -1)
        {
            throw new Exception("Key column not set!");
        }

        if (sdRow[keyColIndex] == null)
        {
            return [];
        }

        int rowIndex = 0;
        List<DbElementRow> results = [];

        // iterate over the concepts for this value set
        foreach (DbElement element in sdRow[keyColIndex]!.Elements)
        {
            DbElementRow row = new DbElementRow(_packages.Count, keyColIndex, rowIndex++);

            int startCol = keyColIndex;
            row[startCol] = new()
            {
                SdCell = sdRow[startCol]!,
                Element = element,
            };

            List<DbElementRow> right = [];

            // project right
            if (startCol < _packages.Count)
            {
                right = projectElement(sdRow, row, startCol, true, ref rowIndex);
            }

            // if we started at the first definition, we are done
            if (startCol == 0)
            {
                results.AddRange(right);
                continue;
            }

            // project left and add as we go
            foreach (DbElementRow r in right)
            {
                results.AddRange(projectElement(sdRow, r, startCol, false, ref rowIndex));
            }
        }

        return results;
    }

    private List<DbElementRow> projectElement(
        DbSdRow sdRow,
        DbElementRow incomingRow,
        int column,
        bool projectRight,
        ref int rowNumber)
    {
        if ((incomingRow[column] == null) ||
            (sdRow[column] == null))
        {
            return [incomingRow];
        }

        int nextCol = projectRight ? column + 1 : column - 1;
        if ((nextCol < 0) || (nextCol >= incomingRow.Length))
        {
            return [incomingRow];
        }

        if (projectRight &&
            ((sdRow[column]!.RightCell == null) || (sdRow[column]!.RightComparison == null)))
        {
            return [incomingRow];
        }

        if ((!projectRight) &&
            ((sdRow[column]!.LeftCell == null) || (sdRow[column]!.LeftComparison == null)))
        {
            return [incomingRow];
        }

        int comparisonKey = projectRight
            ? sdRow[column]!.RightComparison!.Key
            : sdRow[column]!.LeftComparison!.Key;

        // look for the concept comparisons for this ValueSet comparison and concept
        List<DbElementComparison> edges = DbElementComparison.SelectList(
            _db,
            StructureComparisonKey: comparisonKey,
            SourceElementKey: incomingRow[column]!.Element.Key,
            orderByProperties: [nameof(DbElementComparison.TargetElementKey)]);

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

        List<DbElementRow> results = [];

        // iterate over our neighbors
        foreach (DbElementComparison edge in edges)
        {
            // resolve the concept
            DbElement element = DbElement.SelectSingle(_db, Key: edge.TargetElementKey)
                ?? throw new Exception($"Failed to resolve compared element: {edge.TargetElementKey}!");

            DbElementComparison? inverseEdge = edge.InverseComparisonKey == null
                ? null
                : DbElementComparison.SelectSingle(_db, Key: edge.InverseComparisonKey);

            DbElementRow row = edges.Count == 1 ? incomingRow : incomingRow.DeepCopy(rowNumber++);
            if (projectRight == true)
            {
                row[nextCol] = new()
                {
                    SdCell = sdRow[nextCol]!,
                    Element = element,
                    LeftCell = incomingRow[column]!,
                    LeftElement = incomingRow[column]!.Element,
                    LeftComparison = inverseEdge,
                };

                row[column]!.RightCell = row[nextCol];
                row[column]!.RightElement = row[nextCol]!.Element;
                row[column]!.RightComparison = edge;
            }
            else
            {
                row[nextCol] = new()
                {
                    SdCell = sdRow[nextCol]!,
                    Element = element,
                    RightCell = incomingRow[column]!,
                    RightElement = incomingRow[column]!.Element,
                    RightComparison = inverseEdge,
                };

                row[column]!.LeftCell = row[nextCol];
                row[column]!.RightElement = row[nextCol]!.Element;
                row[column]!.LeftComparison = edge;
            }

            // recurse
            List<DbElementRow> next = projectElement(sdRow, row, nextCol, projectRight, ref rowNumber);

            // combine results
            results.AddRange(next);
        }

        return results;
    }
}
