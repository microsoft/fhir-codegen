using System;
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

    object ICloneable.Clone() => this with { };
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

    object ICloneable.Clone() => this with { };
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


    public List<DbSdCell?[]> Project()
    {
        DbSdCell?[] row = new DbSdCell?[_packages.Count];
        int startCol = _packages.FindIndex((fp) => fp.Key == _keySd.FhirPackageKey);
        row[startCol] = new()
        {
            FhirPackage = _packages[startCol],
            Sd = _keySd,
            Elements = DbElement.SelectList(_db, StructureKey: _keySd.Key),
        };

        _keyCol = startCol;

        List<DbSdCell?[]> right = [];

        // project right
        if (startCol < _packages.Count)
        {
            right = projectSd(row, startCol, true);
        }

        // if we started at the first definition, we are done
        if (startCol == 0)
        {
            return right;
        }

        List<DbSdCell?[]> results = [];

        // project left
        foreach (DbSdCell?[] r in right)
        {
            results.AddRange(projectSd(r, startCol, false));
        }

        return results;
    }

    private List<DbSdCell?[]> projectSd(
        DbSdCell?[] incomingRow,
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
        List<DbStructureComparison> edges = DbStructureComparison.SelectList(
            _db,
            SourceStructureKey: incomingRow[column]!.Sd.Key,
            TargetFhirPackageKey: _packages[nextCol].Key,
            orderByProperties: [nameof(DbStructureComparison.TargetStructureKey)]);

        if (edges.Count == 0)
        {
            return [incomingRow];
        }

        List<DbSdCell?[]> results = [];

        // iterate over our neighbors
        foreach (DbStructureComparison edge in edges)
        {
            DbStructureComparison? inverseEdge = edge.InverseComparisonKey == null
                ? null
                : DbStructureComparison.SelectSingle(_db, Key: edge.InverseComparisonKey);

            DbSdCell?[] row = edges.Count == 1 ? incomingRow : (DbSdCell?[])incomingRow.Clone();
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
            List<DbSdCell?[]> next = projectSd(row, nextCol, projectRight);

            // combine results
            results.AddRange(next);
        }

        return results;
    }

    public List<DbElementCell?[]> ProjectElements(DbSdCell?[] sdRow)
    {
        if (_keyCol == -1)
        {
            throw new Exception("Key column not set!");
        }

        if (sdRow[_keyCol] == null)
        {
            return [];
        }

        List<DbElementCell?[]> results = [];

        // iterate over the concepts for this value set
        foreach (DbElement element in sdRow[_keyCol]!.Elements)
        {
            DbElementCell?[] row = new DbElementCell?[_packages.Count];

            int startCol = _keyCol;
            row[startCol] = new()
            {
                SdCell = sdRow[startCol]!,
                Element = element,
            };

            List<DbElementCell?[]> right = [];

            // project right
            if (startCol < _packages.Count)
            {
                right = projectElement(sdRow, row, startCol, true);
            }

            // if we started at the first definition, we are done
            if (startCol == 0)
            {
                results.AddRange(right);
                continue;
            }

            // project left and add as we go
            foreach (DbElementCell?[] r in right)
            {
                results.AddRange(projectElement(sdRow, r, startCol, false));
            }
        }

        return results;
    }

    private List<DbElementCell?[]> projectElement(
        DbSdCell?[] sdRow,
        DbElementCell?[] incomingRow,
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

        List<DbElementCell?[]> results = [];

        // iterate over our neighbors
        foreach (DbElementComparison edge in edges)
        {
            // resolve the concept
            DbElement element = DbElement.SelectSingle(_db, Key: edge.TargetElementKey) ?? throw new Exception($"Failed to resolve compared element: {edge.TargetElementKey}!");

            DbElementCell?[] row = edges.Count == 1 ? incomingRow : (DbElementCell?[])incomingRow.Clone();
            if (projectRight == true)
            {
                row[nextCol] = new()
                {
                    SdCell = sdRow[nextCol]!,
                    Element = element,
                    LeftCell = incomingRow[column]!,
                    LeftElement = incomingRow[column]!.Element,
                    LeftComparison = edge,
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
                    RightComparison = edge,
                };

                row[column]!.LeftCell = row[nextCol];
                row[column]!.RightElement = row[nextCol]!.Element;
                row[column]!.LeftComparison = edge;
            }

            // recurse
            List<DbElementCell?[]> next = projectElement(sdRow, row, nextCol, projectRight);

            // combine results
            results.AddRange(next);
        }

        return results;
    }
}
