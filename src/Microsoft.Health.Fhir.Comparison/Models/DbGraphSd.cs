using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.ElementModel.Types;
using static Microsoft.Health.Fhir.Comparison.Models.DbGraphVs;

namespace Microsoft.Health.Fhir.Comparison.Models;


public class DbGraphSd
{
    protected readonly IDbConnection _db = null!;
    protected readonly List<DbFhirPackage> _packages = null!;
    protected readonly DbStructureDefinition _keySd = null!;
    protected int _keyCol = -1;

    public required IDbConnection DB { get => _db; init => _db = value; }
    public required List<DbFhirPackage> Packages { get => _packages; init => _packages = value; }
    public required DbStructureDefinition KeySd { get => _keySd; init => _keySd = value; }

    private List<DbSdRow>? _projection = null;
    public List<DbSdRow> Projection => _projection ?? BuildProjection();


    public record class DbSdCell : IDbComparisonCell, ICloneable
    {
        private readonly List<DbElement> _elements = null!;

        public required DbSdRow Row { get; init; }
        public required int FhirPackageIndex { get; init; }
        public required DbStructureDefinition Sd { get; init; }
        public required List<DbElement> Elements
        {
            get => _elements;
            init
            {
                _elements = value;
            }
        }

        public DbSdCell? LeftCell { get; set; } = null;
        public DbStructureDefinition? LeftSd { get; set; } = null;
        public DbStructureComparison? LeftComparison { get; set; } = null;

        public DbSdCell? RightCell { get; set; } = null;
        public DbStructureDefinition? RightSd { get; set; } = null;
        public DbStructureComparison? RightComparison { get; set; } = null;
        IDbComparisonCell? IDbComparisonCell.LeftCell => LeftCell;
        DbPackageComparisonContent? IDbComparisonCell.LeftComparison => LeftComparison;
        IDbComparisonCell? IDbComparisonCell.RightCell => RightCell;
        DbPackageComparisonContent? IDbComparisonCell.RightComparison => RightComparison;

        object ICloneable.Clone() => this with { };

        public override string ToString() => Row._graph._packages[FhirPackageIndex].ShortName + ":" + Sd.Name;
    }


    public class DbSdRow : IEnumerable<DbSdCell?>
    {
        internal DbGraphSd _graph;
        protected readonly Guid _uuid;
        private readonly DbSdCell?[] _cells;

        public DbSdRow(
            DbGraphSd graph,
            DbSdCell?[]? cells = null,
            Guid? uuid = null)
        {
            _uuid = uuid ?? Guid.NewGuid();
            _graph = graph;
            _cells = cells ?? new DbSdCell?[_graph._packages.Count];
        }


        public Guid RowId => _uuid;
        public DbSdCell? KeyCell => _graph._keyCol >= 0 ? _cells[_graph._keyCol] : null;
        public DbSdCell?[] Cells => _cells;
        public int Length => _cells.Length;

        private List<DbElementRow>? _projection = null;
        public List<DbElementRow> Projection => _projection ??= BuildProjection(_graph._keyCol);

        public override string ToString() => string.Join(" - ", _cells.Select(c => c?.ToString() ?? string.Empty));

        public DbSdCell? CellAt(int index)
        {
            if (index >= 0 && index < _cells.Length)
            {
                return _cells[index];
            }
            return null;
        }

        public bool HasNeigbor(int index)
        {
            if (index == 0)
            {
                return _cells[1] != null;
            }

            if (index == _cells.Length - 1)
            {
                return _cells[_cells.Length - 2] != null;
            }

            return (_cells[index - 1] != null) || (_cells[index + 1] != null);
        }

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
                {
                    return null;
                }

                return _cells.FirstOrDefault(cell => cell?.Sd.FhirPackageKey == package.Key);
            }
        }


        public List<DbElementRow> BuildProjection(int? keyColumnIndex = null, bool fullJoin = false)
        {
            int keyColIndex = keyColumnIndex ??= _graph._keyCol;

            if (keyColIndex == -1)
            {
                throw new Exception("Key column not set!");
            }

            if (_cells[keyColIndex] == null)
            {
                return [];
            }

            HashSet<int> usedElementKeys = [];
            List<DbElementRow> results = [];

            // iterate over the elements for this structure
            foreach (DbElement element in _cells[keyColIndex]!.Elements)
            {
                DbElementRow row = new DbElementRow(_graph, this);

                int startCol = keyColIndex;
                row[startCol] = new()
                {
                    Row = row,
                    SdCell = _cells[startCol]!,
                    Element = element,
                };

                usedElementKeys.Add(element.Key);

                List<DbElementRow> right = [];

                // project right
                if (startCol < _graph._packages.Count)
                {
                    right = projectElement(row, startCol, true, usedElementKeys);
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
                    results.AddRange(projectElement(r, startCol, false, usedElementKeys));
                }
            }


            // if we are doing a full join, check for unused SD mappings
            if (fullJoin)
            {
                if (keyColIndex < (_graph._packages.Count - 1))
                {
                    for (int colIndex = keyColIndex + 1; colIndex < _graph._packages.Count; colIndex++)
                    {
                        if (_cells[colIndex] == null)
                        {
                            continue;
                        }

                        foreach (DbElement element in _cells[colIndex]!.Elements)
                        {
                            if (usedElementKeys.Contains(element.Key))
                            {
                                continue;
                            }

                            // add a new row
                            DbElementRow row = new DbElementRow(_graph, this);

                            row[colIndex] = new()
                            {
                                Row = row,
                                SdCell = _cells[colIndex]!,
                                Element = element,
                            };

                            usedElementKeys.Add(element.Key);

                            List<DbElementRow> right = [];

                            // project right
                            if (colIndex < _graph._packages.Count)
                            {
                                right = projectElement(row, colIndex, true, usedElementKeys);
                            }

                            // if we started at the first definition, we are done
                            if (colIndex == 0)
                            {
                                results.AddRange(right);
                                continue;
                            }

                            // project left and add as we go
                            foreach (DbElementRow r in right)
                            {
                                results.AddRange(projectElement(r, colIndex, false, usedElementKeys));
                            }
                        }
                    }
                }

                if (keyColIndex > 0)
                {
                    for (int colIndex = keyColIndex - 1; colIndex >= 0; colIndex--)
                    {
                        if (_cells[colIndex] == null)
                        {
                            continue;
                        }

                        foreach (DbElement element in _cells[colIndex]!.Elements)
                        {
                            if (usedElementKeys.Contains(element.Key))
                            {
                                continue;
                            }

                            // add a new row
                            DbElementRow row = new DbElementRow(_graph, this);

                            row[colIndex] = new()
                            {
                                Row = row,
                                SdCell = _cells[colIndex]!,
                                Element = element,
                            };

                            usedElementKeys.Add(element.Key);

                            List<DbElementRow> right = [];

                            // project right
                            if (colIndex < _graph._packages.Count)
                            {
                                right = projectElement(row, colIndex, true, usedElementKeys);
                            }

                            // if we started at the first definition, we are done
                            if (colIndex == 0)
                            {
                                results.AddRange(right);
                                continue;
                            }

                            // project left and add as we go
                            foreach (DbElementRow r in right)
                            {
                                results.AddRange(projectElement(r, colIndex, false, usedElementKeys));
                            }
                        }
                    }
                }
            }

            _projection = results;

            return results;
        }

        private List<DbElementRow> projectElement(
            DbElementRow incomingRow,
            int column,
            bool projectRight,
            HashSet<int> usedElementKeys)
        {
            if ((incomingRow[column] == null) ||
                (_cells[column] == null))
            {
                return [incomingRow];
            }

            int nextCol = projectRight ? column + 1 : column - 1;
            if ((nextCol < 0) || (nextCol >= incomingRow.Length))
            {
                return [incomingRow];
            }

            if (projectRight &&
                ((_cells[column]!.RightCell == null) || (_cells[column]!.RightComparison == null)))
            {
                return [incomingRow];
            }

            if ((!projectRight) &&
                ((_cells[column]!.LeftCell == null) || (_cells[column]!.LeftComparison == null)))
            {
                return [incomingRow];
            }

            int comparisonKey = projectRight
                ? _cells[column]!.RightComparison!.Key
                : _cells[column]!.LeftComparison!.Key;

            // look for the concept comparisons for this ValueSet comparison and concept
            List<DbElementComparison> edges = DbElementComparison.SelectList(
                _graph._db,
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
                DbElement element = DbElement.SelectSingle(_graph._db, Key: edge.TargetElementKey)
                    ?? throw new Exception($"Failed to resolve compared element: {edge.TargetElementKey}!");

                DbElementComparison? inverseEdge = edge.InverseComparisonKey == null
                    ? null
                    : DbElementComparison.SelectSingle(_graph._db, Key: edge.InverseComparisonKey);

                DbElementRow row = edges.Count == 1 ? incomingRow : incomingRow.DeepCopy(false);
                if (projectRight == true)
                {
                    row[nextCol] = new()
                    {
                        Row = row,
                        SdCell = _cells[nextCol]!,
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
                        Row = row,
                        SdCell = _cells[nextCol]!,
                        Element = element,
                        RightCell = incomingRow[column]!,
                        RightElement = incomingRow[column]!.Element,
                        RightComparison = inverseEdge,
                    };

                    row[column]!.LeftCell = row[nextCol];
                    row[column]!.RightElement = row[nextCol]!.Element;
                    row[column]!.LeftComparison = edge;
                }

                usedElementKeys.Add(element.Key);

                // recurse
                List<DbElementRow> next = projectElement(row, nextCol, projectRight, usedElementKeys);

                // combine results
                results.AddRange(next);
            }

            return results;
        }

        public DbSdRow DeepCopy(bool copyGuid)
        {
            DbSdCell?[] cells = new DbSdCell?[_cells.Length];
            for (int i = 0; i < _cells.Length; i++)
            {
                cells[i] = _cells[i] == null ? null : _cells[i]! with { };
            }
            return new DbSdRow(_graph, cells, copyGuid ? _uuid : null);
        }

        public DbSdRow ShallowCopy(bool copyGuid)
        {
            DbSdCell?[] cells = _cells.Select(v => v).ToArray();
            return new DbSdRow(_graph, cells, copyGuid ? _uuid : null);
        }

        public DbSdRow ShallowCopy(bool copyGuid, params int[] onlyCopyRows)
        {
            DbSdCell?[] cells = new DbSdCell?[_cells.Length];
            for (int i = 0; i < _cells.Length; i++)
            {
                if (onlyCopyRows.Contains(i))
                {
                    cells[i] = _cells[i];
                }
                else
                {
                    cells[i] = null;
                }
            }
            return new DbSdRow(_graph, cells, copyGuid ? _uuid : null);
        }

        public IEnumerator<DbSdCell?> GetEnumerator() => ((IEnumerable<DbSdCell?>)_cells).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _cells.GetEnumerator();
    }


    public record class DbElementCell : IDbComparisonCell, ICloneable
    {
        public required DbElementRow Row { get; init; }
        public required DbSdCell SdCell { get; set; }
        public required DbElement Element { get; set; }

        public DbElementCell? LeftCell { get; set; } = null;
        public DbElement? LeftElement { get; set; } = null;
        public DbElementComparison? LeftComparison { get; set; } = null;

        public DbElementCell? RightCell { get; set; } = null;
        public DbElement? RightElement { get; set; } = null;
        public DbElementComparison? RightComparison { get; set; } = null;

        IDbComparisonCell? IDbComparisonCell.LeftCell => LeftCell;
        DbPackageComparisonContent? IDbComparisonCell.LeftComparison => LeftComparison;
        IDbComparisonCell? IDbComparisonCell.RightCell => RightCell;
        DbPackageComparisonContent? IDbComparisonCell.RightComparison => RightComparison;

        object ICloneable.Clone() => this with { };
    }


    public class DbElementRow : IEnumerable<DbElementCell?>
    {
        private readonly Guid _uuid;
        private readonly DbGraphSd _graph;
        private readonly DbSdRow _sdRow;
        private readonly DbElementCell?[] _cells;

        public DbElementRow(
            DbGraphSd graph,
            DbSdRow sdRow,
            DbElementCell?[]? cells = null,
            Guid? uuid = null)
        {
            _uuid = uuid ?? Guid.NewGuid();
            _graph = graph;
            _sdRow = sdRow;
            _cells = cells ?? new DbElementCell?[_graph._packages.Count];
        }

        public Guid RowId => _uuid;
        public int KeyCol => _graph._keyCol;
        public DbElementCell? KeyCell => _graph._keyCol >= 0 ? _cells[_graph._keyCol] : null;
        public DbElementCell?[] Cells => _cells;
        public int Length => _cells.Length;

        public (bool, bool) HasNeigbors(int index)
        {
            if (index == 0)
            {
                return (false, _cells[1] != null);
            }

            if (index == _cells.Length - 1)
            {
                return (_cells[_cells.Length - 2] != null, false);
            }

            return (_cells[index - 1] != null, _cells[index + 1] != null);
        }

        // Add indexer to access cells directly
        public DbElementCell? this[int index]
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
        public DbElementCell? this[DbFhirPackage package]
        {
            get
            {
                if (package == null)
                {
                    return null;
                }

                return _cells.FirstOrDefault(cell => cell?.SdCell.Sd.FhirPackageKey == package.Key);
            }
        }

        public DbElementRow DeepCopy(bool copyGuid)
        {
            DbElementCell?[] cells = new DbElementCell?[_cells.Length];
            for (int i = 0; i < _cells.Length; i++)
            {
                cells[i] = _cells[i] == null ? null : _cells[i]! with { };
            }
            return new DbElementRow(_graph, _sdRow, cells, copyGuid ? _uuid : null);
        }

        public DbElementRow ShallowCopy(bool copyGuid)
        {
            DbElementCell?[] cells = new DbElementCell?[_cells.Length];
            for (int i = 0; i < _cells.Length; i++)
            {
                cells[i] = _cells[i];
            }
            return new DbElementRow(_graph, _sdRow, cells, copyGuid ? _uuid : null);
        }

        public DbElementRow ShallowCopy(bool copyGuid, params int[] onlyCopyRows)
        {
            DbElementCell?[] cells = new DbElementCell?[_cells.Length];
            for (int i = 0; i < _cells.Length; i++)
            {
                if (onlyCopyRows.Contains(i))
                    cells[i] = _cells[i];
                else
                    cells[i] = null;
            }
            return new DbElementRow(_graph, _sdRow, cells, copyGuid ? _uuid : null);
        }

        public IEnumerator<DbElementCell?> GetEnumerator() => ((IEnumerable<DbElementCell?>)_cells).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _cells.GetEnumerator();
    }


    public List<DbSdRow> BuildProjection()
    {
        int startCol = _packages.FindIndex((fp) => fp.Key == _keySd.FhirPackageKey);
        _keyCol = startCol;

        DbSdRow row = new DbSdRow(this);
        row[startCol] = new()
        {
            Row = row,
            Sd = _keySd,
            FhirPackageIndex = startCol,
            Elements = DbElement.SelectList(_db, StructureKey: _keySd.Key),
        };

        List<DbSdRow> right = [];

        // project right
        if (startCol < _packages.Count)
        {
            right = projectSd(row, startCol, projectRight: true);
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
            results.AddRange(projectSd(r, startCol, projectRight: false));
        }

        return results;
    }

    private List<DbSdRow> projectSd(
        DbSdRow incomingRow,
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

        List<DbSdRow> results = [];

        // iterate over our neighbors
        foreach (DbStructureComparison edge in edges)
        {
            DbStructureComparison? inverseEdge = edge.InverseComparisonKey == null
                ? null
                : DbStructureComparison.SelectSingle(_db, Key: edge.InverseComparisonKey);

            DbSdRow row = edges.Count == 1 ? incomingRow : incomingRow.DeepCopy(false);
            if (projectRight == true)
            {
                row[nextCol] = new DbSdCell()
                {
                    Row = row,
                    FhirPackageIndex = nextCol,
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
                    Row = row,
                    FhirPackageIndex = nextCol,
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
            List<DbSdRow> next = projectSd(row, nextCol, projectRight);

            // combine results
            results.AddRange(next);
        }

        return results;
    }
}
