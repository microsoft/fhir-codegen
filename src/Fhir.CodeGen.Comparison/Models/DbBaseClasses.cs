using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fhir.CodeGen.SQLiteGenerator;

namespace Fhir.CodeGen.Comparison.Models;

[CgSQLiteBaseClass]
public abstract class DbRecordBase
{
    [CgSQLiteKey]
    public int Key { get; set; } = -1;

    //internal static int _indexValue = 0;
    //public static int GetIndex() => Interlocked.Increment(ref _indexValue);
}


[CgSQLiteBaseClass]
public abstract class DbPackageContent : DbRecordBase
{
    [CgSQLiteForeignKey(referenceTable: "FhirPackages", referenceColumn: nameof(DbFhirPackage.Key))]
    public required int FhirPackageKey { get; set; }
}

[CgSQLiteBaseClass]
public abstract class DbResourceRelatedContentBase : DbPackageContent
{
    public required int ResourceKey { get; set; }
    public required int Order { get; set; }
}


public class DbRecordCache<T>
        where T : DbRecordBase
{
    private readonly Dictionary<int, T> _byKey = [];

    private readonly Dictionary<int, T> _toAdd = [];
    private readonly Dictionary<int, T> _toUpdate = [];
    private readonly Dictionary<int, T> _toDelete = [];

    public bool TryGet(int key, [NotNullWhen(true)] out T? value) => _byKey.TryGetValue(key, out value);

    public T? Get(int key) => _byKey.TryGetValue(key, out T? value) ? value : default(T);

    public IEnumerable<T> Values => _byKey.Values;
    public int ValueCount => _byKey.Count;

    public IEnumerable<T> ToAdd => _toAdd.Values;
    public int ToAddCount => _toAdd.Count;
    public IEnumerable<T> ToUpdate => _toUpdate.Values;
    public int ToUpdateCount => _toUpdate.Count;
    public IEnumerable<T> ToDelete => _toDelete.Values;
    public int ToDeleteCount => _toDelete.Count;

    public void Clear()
    {
        _byKey.Clear();
        _toAdd.Clear();
        _toUpdate.Clear();
        _toDelete.Clear();
    }

    public void CacheAdd(T item)
    {
        _byKey[item.Key] = item;
        _toAdd[item.Key] = item;
    }

    public void CacheUpdate(T item)
    {
        _byKey[item.Key] = item;
        _toUpdate[item.Key] = item;
    }

    public void CacheDelete(T item)
    {
        _byKey[item.Key] = item;
        _toDelete[item.Key] = item;
    }

    public void Changed(T item)
    {
        if (!_byKey.ContainsKey(item.Key))
        {
            _byKey[item.Key] = item;
        }

        if (_toDelete.ContainsKey(item.Key))
        {
            _toDelete.Remove(item.Key);
        }

        if (_toUpdate.ContainsKey(item.Key))
        {
            _toUpdate[item.Key] = item;
            return;
        }

        _toAdd[item.Key] = item;
    }

    public int Count => _byKey.Count;
}
