using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.ElementModel.Types;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.Comparison.Models;


public class DbGraphVs
{
    protected readonly IDbConnection _db = null!;
    protected readonly List<DbFhirPackage> _packages = null!;
    protected readonly DbValueSet _keyVs = null!;
    protected int _keyCol = -1;

    public required IDbConnection DB { get => _db; init => _db = value; }
    public required List<DbFhirPackage> Packages { get => _packages; init => _packages = value; }
    public required DbValueSet KeyVs { get => _keyVs; init => _keyVs = value; }
    public bool IncludeInactive { get; set; } = false;
    public bool IncludeAbstract { get; set; } = false;

    private List<DbVsRow>? _projection = null;
    public List<DbVsRow> Projection => _projection ?? BuildProjection();

    public record class DbVsCell : IDbComparisonCell, ICloneable
    {
        private readonly List<DbValueSetConcept> _concepts = null!;

        public required DbVsRow Row { get; init; }
        public required int FhirPackageIndex { get; init; }
        public required DbValueSet Vs { get; init; }
        public required List<DbValueSetConcept> Concepts
        {
            get => _concepts;
            init
            {
                _concepts = value;
            }
        }

        public DbVsCell? LeftCell { get; set; } = null;
        public DbValueSet? LeftVs { get; set; } = null;
        public DbValueSetComparison? LeftComparison { get; set; } = null;

        public DbVsCell? RightCell { get; set; } = null;
        public DbValueSet? RightVs { get; set; } = null;
        public DbValueSetComparison? RightComparison { get; set; } = null;

        IDbComparisonCell? IDbComparisonCell.LeftCell => LeftCell;
        DbPackageComparisonContent? IDbComparisonCell.LeftComparison => LeftComparison;
        IDbComparisonCell? IDbComparisonCell.RightCell => RightCell;
        DbPackageComparisonContent? IDbComparisonCell.RightComparison => RightComparison;

        object ICloneable.Clone() => this with { };
    }


    public class DbVsRow : IEnumerable<DbVsCell?>
    {
        private DbGraphVs _graph;
        private readonly Guid _uuid;
        private readonly DbVsCell?[] _cells;

        private List<DbVsConceptRow>? _projection = null;
        public List<DbVsConceptRow> Projection => _projection ?? BuildConceptProjection();

        public DbVsRow(
            DbGraphVs graph,
            DbVsCell?[] cells,
            Guid? uuid = null)
        {
            _uuid = uuid ?? Guid.NewGuid();
            _graph = graph;
            _cells = cells;
        }

        public DbVsRow(
            DbGraphVs graph,
            Guid? uuid = null)
        {
            _uuid = uuid ?? Guid.NewGuid();
            _graph = graph;
            _cells = new DbVsCell?[_graph.Packages.Count];
        }

        public Guid RowId => _uuid;
        public DbVsCell? KeyCell => _graph._keyCol >= 0 ? _cells[_graph._keyCol] : null;
        public DbVsCell?[] Cells => _cells;

        public int Length => _cells.Length;

        public DbVsCell? CellAt(int index)
        {
            if (index >= 0 && index < _cells.Length)
            {
                return _cells[index];
            }
            return null;
        }

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
                {
                    return null;
                }

                return _cells.FirstOrDefault(cell => cell?.Vs.FhirPackageKey == package.Key);
            }
        }

        public List<DbVsConceptRow> BuildConceptProjection(int? keyColumnIndex = null, bool fullJoin = false)
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

            HashSet<int> usedConceptKeys = [];
            List<DbVsConceptRow> results = [];

            // iterate over the concepts for this value set
            foreach (DbValueSetConcept concept in _cells[keyColIndex]!.Concepts)
            {
                DbVsConceptRow row = new(_graph, this);

                int startCol = keyColIndex;
                row[startCol] = new()
                {
                    Row = row,
                    VsCell = _cells[startCol]!,
                    Concept = concept,
                };

                usedConceptKeys.Add(concept.Key);

                List<DbVsConceptRow> right = [];

                // project right
                if (startCol < _graph._packages.Count)
                {
                    right = projectConcept(row, startCol, true, usedConceptKeys);
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
                    results.AddRange(projectConcept(r, startCol, false, usedConceptKeys));
                }
            }

            // if we are doing a full join, check for unused VS mappings
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

                        foreach (DbValueSetConcept concept in _cells[colIndex]!.Concepts)
                        {
                            if (usedConceptKeys.Contains(concept.Key))
                            {
                                continue;
                            }

                            // add a new row
                            DbVsConceptRow row = new(_graph, this);

                            row[colIndex] = new()
                            {
                                Row = row,
                                VsCell = _cells[colIndex]!,
                                Concept = concept,
                            };

                            usedConceptKeys.Add(concept.Key);

                            List<DbVsConceptRow> right = [];

                            // project right
                            if (colIndex < _graph._packages.Count)
                            {
                                right = projectConcept(row, colIndex, true, usedConceptKeys);
                            }

                            // if we started at the first definition, we are done
                            if (colIndex == 0)
                            {
                                results.AddRange(right);
                                continue;
                            }

                            // project left and add as we go
                            foreach (DbVsConceptRow r in right)
                            {
                                results.AddRange(projectConcept(r, colIndex, false, usedConceptKeys));
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

                        foreach (DbValueSetConcept concept in _cells[colIndex]!.Concepts)
                        {
                            if (usedConceptKeys.Contains(concept.Key))
                            {
                                continue;
                            }

                            // add a new row
                            DbVsConceptRow row = new(_graph, this);

                            row[colIndex] = new()
                            {
                                Row = row,
                                VsCell = _cells[colIndex]!,
                                Concept = concept,
                            };

                            usedConceptKeys.Add(concept.Key);

                            List<DbVsConceptRow> right = [];

                            // project right
                            if (colIndex < _graph._packages.Count)
                            {
                                right = projectConcept(row, colIndex, true, usedConceptKeys);
                            }

                            // if we started at the first definition, we are done
                            if (colIndex == 0)
                            {
                                results.AddRange(right);
                                continue;
                            }

                            // project left and add as we go
                            foreach (DbVsConceptRow r in right)
                            {
                                results.AddRange(projectConcept(r, colIndex, false, usedConceptKeys));
                            }
                        }
                    }
                }
            }

            // update our local projection copy
            _projection = results;

            return _projection;
        }

        private List<DbVsConceptRow> projectConcept(
            DbVsConceptRow incomingRow,
            int column,
            bool projectRight,
            HashSet<int> usedConceptKeys)
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
            List<DbValueSetConceptComparison> edges = DbValueSetConceptComparison.SelectList(
                _graph._db,
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
                DbValueSetConcept concept = DbValueSetConcept.SelectSingle(_graph._db, Key: edge.TargetConceptKey)
                    ?? throw new Exception($"Failed to resolve compared concept: {edge.TargetConceptKey}!");

                DbValueSetConceptComparison? inverseEdge = edge.InverseComparisonKey == null
                    ? null
                    : DbValueSetConceptComparison.SelectSingle(_graph._db, Key: edge.InverseComparisonKey);

                DbVsConceptRow row = edges.Count == 1 ? incomingRow : incomingRow.DeepCopy(false);
                if (projectRight == true)
                {
                    row[nextCol] = new()
                    {
                        Row = row,
                        VsCell = _cells[nextCol]!,
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
                        Row = row,
                        VsCell = _cells[nextCol]!,
                        Concept = concept,
                        RightCell = incomingRow[column]!,
                        RightConcept = incomingRow[column]!.Concept,
                        RightComparison = inverseEdge,
                    };

                    row[column]!.LeftCell = row[nextCol];
                    row[column]!.RightConcept = row[nextCol]!.Concept;
                    row[column]!.LeftComparison = edge;
                }

                usedConceptKeys.Add(concept.Key);

                // recurse
                List<DbVsConceptRow> next = projectConcept(row, nextCol, projectRight, usedConceptKeys);

                // combine results
                results.AddRange(next);
            }

            return results;
        }


        public DbVsRow DeepCopy(bool copyGuid)
        {
            DbVsCell?[] cells = new DbVsCell?[_cells.Length];
            for (int i = 0; i < _cells.Length; i++)
            {
                cells[i] = _cells[i] == null ? null : _cells[i]! with { };
            }
            return new DbVsRow(_graph, cells, copyGuid ? _uuid : null);
        }

        public DbVsRow ShallowCopy(bool copyGuid)
        {
            DbVsCell?[] cells = new DbVsCell?[_cells.Length];
            for (int i = 0; i < _cells.Length; i++)
            {
                cells[i] = _cells[i];
            }
            return new DbVsRow(_graph, cells, copyGuid ? _uuid : null);
        }

        public DbVsRow ShallowCopy(bool copyGuid, params int[] onlyCopyRows)
        {
            DbVsCell?[] cells = new DbVsCell?[_cells.Length];
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
            return new DbVsRow(_graph, cells, copyGuid ? _uuid : null);
        }

        public IEnumerator<DbVsCell?> GetEnumerator() => ((IEnumerable<DbVsCell?>)_cells).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _cells.GetEnumerator();
    }


    public record class DbVsConceptCell : IDbComparisonCell, ICloneable
    {
        public required DbVsConceptRow Row { get; init; }
        public required DbVsCell VsCell { get; set; }
        public required DbValueSetConcept Concept { get; set; }

        public DbVsConceptCell? LeftCell { get; set; } = null;
        public DbValueSetConcept? LeftConcept { get; set; } = null;
        public DbValueSetConceptComparison? LeftComparison { get; set; } = null;

        public DbVsConceptCell? RightCell { get; set; } = null;
        public DbValueSetConcept? RightConcept { get; set; } = null;
        public DbValueSetConceptComparison? RightComparison { get; set; } = null;

        IDbComparisonCell? IDbComparisonCell.LeftCell => LeftCell;
        DbPackageComparisonContent? IDbComparisonCell.LeftComparison => LeftComparison;
        IDbComparisonCell? IDbComparisonCell.RightCell => RightCell;
        DbPackageComparisonContent? IDbComparisonCell.RightComparison => RightComparison;

        object ICloneable.Clone() => this with { };
    }


    public class DbVsConceptRow : IEnumerable<DbVsConceptCell?>
    {
        private readonly Guid _uuid;
        private readonly DbGraphVs _graph;
        private readonly DbVsRow _vsRow;
        private readonly DbVsConceptCell?[] _cells;

        public DbVsConceptRow(
            DbGraphVs graph,
            DbVsRow vsRow,
            DbVsConceptCell?[] cells,
            Guid? uuid = null)
        {
            _uuid = uuid ?? Guid.NewGuid();
            _graph = graph;
            _vsRow = vsRow;
            _cells = cells;
        }

        public DbVsConceptRow(
            DbGraphVs graph,
            DbVsRow vsRow,
            Guid? uuid = null)
        {
            _uuid = uuid ?? Guid.NewGuid();
            _graph = graph;
            _vsRow = vsRow;
            _cells = new DbVsConceptCell?[_graph._packages.Count];
        }

        public Guid RowId => _uuid;
        public int KeyCol => _graph._keyCol;
        public DbVsConceptCell? KeyCell => _graph._keyCol >= 0 ? _cells[_graph._keyCol] : null;
        public DbVsConceptCell?[] Cells => _cells;
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
                {
                    return null;
                }

                return _cells.FirstOrDefault(cell => cell?.VsCell.Vs.FhirPackageKey == package.Key);
            }
        }

        public DbVsConceptRow DeepCopy(bool copyGuid)
        {
            DbVsConceptCell?[] cells = new DbVsConceptCell?[_cells.Length];
            for (int i = 0; i < _cells.Length; i++)
            {
                cells[i] = _cells[i] == null ? null : _cells[i]! with { };
            }
            return new DbVsConceptRow(_graph, _vsRow, cells, copyGuid ? _uuid : null);
        }

        public DbVsConceptRow ShallowCopy(bool copyGuid)
        {
            DbVsConceptCell?[] cells = new DbVsConceptCell?[_cells.Length];
            for (int i = 0; i < _cells.Length; i++)
            {
                cells[i] = _cells[i];
            }
            return new DbVsConceptRow(_graph, _vsRow, cells, copyGuid ? _uuid : null);
        }

        public DbVsConceptRow ShallowCopy(bool copyGuid, params int[] onlyCopyRows)
        {
            DbVsConceptCell?[] cells = new DbVsConceptCell?[_cells.Length];
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
            return new DbVsConceptRow(_graph, _vsRow, cells, copyGuid ? _uuid : null);
        }

        public IEnumerator<DbVsConceptCell?> GetEnumerator() => ((IEnumerable<DbVsConceptCell?>)_cells).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _cells.GetEnumerator();
    }

    public List<DbVsRow> BuildProjection()
    {
        int startCol = _packages.FindIndex((fp) => fp.Key == _keyVs.FhirPackageKey);
        _keyCol = startCol;

        DbVsRow row = new DbVsRow(this);
        row[startCol] = new()
        {
            Row = row,
            FhirPackageIndex = startCol,
            Vs = _keyVs,
            Concepts = DbValueSetConcept.SelectList(
                _db,
                ValueSetKey: _keyVs.Key,
                Inactive: IncludeInactive,
                Abstract: IncludeAbstract),
        };

        List<DbVsRow> right = [];

        // project right
        if (startCol < _packages.Count)
        {
            right = projectVs(row, startCol, projectRight: true);
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
            results.AddRange(projectVs(r, startCol, projectRight: false));
        }

        _projection = results;

        return _projection;
    }

    private List<DbVsRow> projectVs(
        DbVsRow incomingRow,
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

        List<DbVsRow> results = [];

        // iterate over our neighbors
        foreach (DbValueSetComparison edge in edges)
        {
            DbValueSetComparison? inverseEdge = edge.InverseComparisonKey == null
                ? null
                : DbValueSetComparison.SelectSingle(_db, Key: edge.InverseComparisonKey);

            DbVsRow row = edges.Count == 1 ? incomingRow : incomingRow.DeepCopy(false);
            if (projectRight == true)
            {
                row[nextCol] = new()
                {
                    Row = row,
                    FhirPackageIndex = nextCol,
                    Vs = DbValueSet.SelectSingle(_db, Key: edge.TargetValueSetKey) ?? throw new Exception($"Failed to resolve compared ValueSet: {edge.TargetValueSetKey}!"),
                    Concepts = DbValueSetConcept.SelectList(
                        _db,
                        ValueSetKey: edge.TargetValueSetKey,
                        Inactive: false,
                        Abstract: false,
                        orderByProperties: [nameof(DbValueSetConcept.System), nameof(DbValueSetConcept.Code)]),
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
                    Row = row,
                    FhirPackageIndex = nextCol,
                    Vs = DbValueSet.SelectSingle(_db, Key: edge.TargetValueSetKey) ?? throw new Exception($"Failed to resolve compared ValueSet: {edge.TargetValueSetKey}!"),
                    Concepts = DbValueSetConcept.SelectList(
                        _db,
                        ValueSetKey: edge.TargetValueSetKey,
                        Inactive: false,
                        Abstract: false,
                        orderByProperties: [nameof(DbValueSetConcept.System), nameof(DbValueSetConcept.Code)]),
                    RightCell = incomingRow[column]!,
                    RightVs = incomingRow[column]!.Vs,
                    RightComparison = inverseEdge,
                };

                row[column]!.LeftCell = row[nextCol];
                row[column]!.LeftVs = row[nextCol]!.Vs;
                row[column]!.LeftComparison = edge;
            }

            // recurse
            List<DbVsRow> next = projectVs(row, nextCol, projectRight);

            // combine results
            results.AddRange(next);
        }

        return results;
    }

}
