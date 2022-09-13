// <copyright file="FhirExpando.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq.Expressions;
using Microsoft.Scripting.Debugging;
using Microsoft.Scripting.Utils;

namespace Microsoft.Health.Fhir.SpecManager.Models;

/// <summary>
/// Represents an object with members that can be dynamically added and removed at runtime.
/// This implementation has modifications to support FHIR.
/// Based off the DotNet ExpandoObject:
///   https://github.com/Microsoft/referencesource/blob/master/System.Core/Microsoft/Scripting/Actions/ExpandoObject.cs
/// </summary>
[System.Text.Json.Serialization.JsonConverter(typeof(FhirExpandoJsonConverter))]
public class FhirExpando : IDynamicMetaObjectProvider, IDictionary<string, object>, INotifyPropertyChanged
{
    /// <summary>(Immutable) the readonly field is used for locking the FhirExpando object.</summary>
    internal readonly object _lockObject;

    /// <summary>The data currently being held by the FhirExpando object.</summary>
    private FhirExpandoData _data;

    /// <summary>The count of available members.</summary>
    private int _count;

    /// <summary>
    /// (Immutable) A marker object used to identify that a value is uninitialized.
    /// </summary>
    internal static readonly object Uninitialized = new object();

    /// <summary>
    /// (Immutable) The value is used to indicate there exists ambiguous match in the FhirExpando object.
    /// </summary>
    internal const int AmbiguousMatchFound = -2;

    /// <summary>(Immutable) The value is used to indicate there is no matching member.</summary>
    internal const int NoMatch = -1;

    /// <summary>The property changed.</summary>
    private PropertyChangedEventHandler _propertyChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirExpando"/> class with no members.
    /// </summary>
    public FhirExpando()
    {
        _data = FhirExpandoData.Empty;
        _lockObject = new object();
    }

    /// <summary>Gets a string.</summary>
    /// <param name="path">A variable-length parameters list containing path.</param>
    /// <returns>The string.</returns>
    public object GetObject(params string[] path)
    {
        object current = this;

        foreach (string p in path)
        {
            if (current == null)
            {
                return null;
            }

            if (current is FhirExpando)
            {
                if (!((FhirExpando)current).TryGetValue(p, out current))
                {
                    return null;
                }

                continue;
            }

            if (current is Array)
            {
                if (!int.TryParse(p, out int index))
                {
                    return null;
                }

                if (((Array)current).Length >= index)
                {
                    current = ((Array)current).GetValue(index);
                }
                else
                {
                    return null;
                }

                continue;
            }

            if (current is IEnumerable)
            {
                if (!int.TryParse(p, out int index))
                {
                    return null;
                }

                current = ((IEnumerable<object>)current).ElementAtOrDefault(index);

                if (current == default(object))
                {
                    return null;
                }

                continue;
            }

            return null;
        }

        return current;
    }

    /// <summary>Gets a string.</summary>
    /// <param name="path">A variable-length parameters list containing path.</param>
    /// <returns>The string.</returns>
    public string GetString(params string[] path) => (string)GetObject(path);

    /// <summary>Gets an int.</summary>
    /// <param name="path">A variable-length parameters list containing path.</param>
    /// <returns>The int.</returns>
    public int? GetInt(params string[] path)
    {
        object o = GetObject(path);

        if (o is int)
        {
            return (int)o;
        }

        if (int.TryParse(o?.ToString(), out int i))
        {
            return i;
        }

        return null;
    }

    /// <summary>Gets a long.</summary>
    /// <param name="path">A variable-length parameters list containing path.</param>
    /// <returns>The long.</returns>
    public long? GetLong(params string[] path)
    {
        object o = GetObject(path);

        if (o is long)
        {
            return (long)o;
        }

        if (long.TryParse(o?.ToString(), out long l))
        {
            return l;
        }

        return null;
    }

    /// <summary>Gets a decimal.</summary>
    /// <param name="path">A variable-length parameters list containing path.</param>
    /// <returns>The decimal.</returns>
    public decimal? GetDecimal(params string[] path)
    {
        object o = GetObject(path);

        if (o is decimal)
        {
            return (decimal)o;
        }

        if (decimal.TryParse(o?.ToString(), out decimal d))
        {
            return d;
        }

        return null;
    }

    /// <summary>Gets a bool.</summary>
    /// <param name="path">A variable-length parameters list containing path.</param>
    /// <returns>The bool.</returns>
    public bool? GetBool(params string[] path)
    {
        object o = GetObject(path);

        if (o is bool)
        {
            return (bool)o;
        }

        if (bool.TryParse(o?.ToString(), out bool b))
        {
            return b;
        }

        return null;
    }

    /// <summary>Gets byte array.</summary>
    /// <param name="path">A variable-length parameters list containing path.</param>
    /// <returns>An array of byte.</returns>
    public byte[] GetByteArray(params string[] path)
    {
        object o = GetObject(path);

        if (o is byte[])
        {
            return (byte[])o;
        }

        if (o is string)
        {
            return Encoding.UTF8.GetBytes((string)o);
        }

        return null;
    }

    /// <summary>Gets string array.</summary>
    /// <param name="path">A variable-length parameters list containing path.</param>
    /// <returns>An array of byte.</returns>
    public string[] GetStringArray(params string[] path)
    {
        object o = GetObject(path);

        switch (o)
        {
            case string[] oSA:
                return oSA;

            case IEnumerable<string> oES:
                return oES.ToArray();

            case IEnumerable<object> oEO:
                return oEO.Select(oEOo => oEOo.ToString()).ToArray();

            case object oO:
                return new string[] { oO.ToString() }; 
        }

        return null;
    }

    /// <summary>Gets string list.</summary>
    /// <param name="path">A variable-length parameters list containing path.</param>
    /// <returns>The string list.</returns>
    public List<string> GetStringList(params string[] path)
    {
        object o = GetObject(path);

        switch (o)
        {
            case string[] oSA:
                return oSA.ToList();

            case List<string> oLS:
                return oLS;

            case IEnumerable<string> oES:
                return oES.ToList();

            case IEnumerable<object> oEO:
                return oEO.Select(oEOo => oEOo.ToString()).ToList();

            case object oO:
                return new List<string> { oO.ToString() };
        }

        return null;
    }

    /// <summary>Gets an expando.</summary>
    /// <param name="path">A variable-length parameters list containing path.</param>
    /// <returns>The expando.</returns>
    public FhirExpando GetExpando(params string[] path)
    {
        object o = GetObject(path);

        if (o is FhirExpando)
        {
            return (FhirExpando)o;
        }

        return null;
    }

    /// <summary>Gets the expando enumerables in this collection.</summary>
    /// <param name="path">A variable-length parameters list containing path.</param>
    /// <returns>
    /// An enumerator that allows foreach to be used to process the expando enumerables in this
    /// collection.
    /// </returns>
    public IEnumerable<FhirExpando> GetExpandoEnumerable(params string[] path)
    {
        object o = GetObject(path);

        switch (o)
        {
            case List<FhirExpando> lf:
                return lf;

            case IEnumerable<FhirExpando> ef:
                return ef;

            case List<object> lo:
                return lo.Select(loo => (FhirExpando)loo).ToList();

            case IEnumerable<object> eo:
                return (IEnumerable<FhirExpando>)eo;

            case FhirExpando f:
                return new FhirExpando[] { f };
        }

        return new FhirExpando[] { (FhirExpando)o };
    }

    /// <summary>Gets the first extension.</summary>
    /// <param name="url"> URL of the resource.</param>
    /// <param name="path">A variable-length parameters list containing path.</param>
    /// <returns>The first extension.</returns>
    public FhirExpando GetFirstExtension(string url, params string[] path)
    {
        FhirExpando o;

        if ((path == null) || (path.Length == 0))
        {
            o = this;
        }
        else
        {
            o = GetExpando(path);

            if (o == null)
            {
                return null;
            }
        }

        IEnumerable<FhirExpando> ext = o.GetExpandoEnumerable("extension");

        if ((ext == null) || (!ext.Any()))
        {
            return null;
        }

        IEnumerable<FhirExpando> filtered = ext.Where(e => url.Equals(e?.GetString("url")));

        if ((filtered == null) || (!filtered.Any()))
        {
            return null;
        }

        return filtered.First();
    }

    /// <summary>Gets the extensions in this collection.</summary>
    /// <param name="url"> URL of the resource.</param>
    /// <param name="path">A variable-length parameters list containing path.</param>
    /// <returns>
    /// An enumerator that allows foreach to be used to process the extensions in this collection.
    /// </returns>
    public IEnumerable<FhirExpando> GetExtensions(string url, params string[] path)
    {
        FhirExpando o;

        if ((path == null) || (path.Length == 0))
        {
            o = this;
        }
        else
        {
            o = GetExpando(path);

            if (o == null)
            {
                return null;
            }
        }

        IEnumerable<FhirExpando> ext = o.GetExpandoEnumerable("extension");

        if ((ext == null) || (!ext.Any()))
        {
            return null;
        }

        return ext.Where(e => url.Equals(e?.GetString("url")));
    }

    /// <summary>
    /// Try to get the data stored for the specified class at the specified index.  If the class has
    /// changed a full lookup for the slot will be performed and the correct value will be retrieved.
    /// </summary>
    /// <exception cref="AmbiguousMatchException">Thrown when the Ambiguous Match error condition
    ///  occurs.</exception>
    /// <param name="indexClass">The index class.</param>
    /// <param name="index">     Zero-based index of the.</param>
    /// <param name="name">      The name.</param>
    /// <param name="ignoreCase">True to ignore case.</param>
    /// <param name="value">     [out] The value.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    internal bool TryGetValue(object indexClass, int index, string name, bool ignoreCase, out object value)
    {
        // read the data now.  The data is immutable so we get a consistent view.
        // If there's a concurrent writer they will replace data and it just appears
        // that we won the ----
        FhirExpandoData data = _data;
        if (data.Class != indexClass || ignoreCase)
        {
            /* Re-search for the index matching the name here if
             *  1) the class has changed, we need to get the correct index and return
             *  the value there.
             *  2) the search is case insensitive:
             *      a. the member specified by index may be deleted, but there might be other
             *      members matching the name if the binder is case insensitive.
             *      b. the member that exactly matches the name didn't exist before and exists now,
             *      need to find the exact match.
             */
            index = data.Class.GetValueIndex(name, ignoreCase, this);
            if (index == FhirExpando.AmbiguousMatchFound)
            {
                throw new System.Reflection.AmbiguousMatchException(name);
            }
        }

        if (index == FhirExpando.NoMatch)
        {
            value = null;
            return false;
        }

        // Capture the value into a temp, so it doesn't get mutated after we check
        // for Uninitialized.
        object temp = data[index];
        if (temp == Uninitialized)
        {
            value = null;
            return false;
        }

        // index is now known to be correct
        value = temp;
        return true;
    }

    /// <summary>
    /// Sets the data for the specified class at the specified index.  If the class has changed then
    /// a full look for the slot will be performed.  If the new class does not have the provided slot
    /// then the FhirExpando's class will change. Only case sensitive setter is supported in FhirExpando.
    /// </summary>
    /// <exception cref="AmbiguousMatchException">Thrown when the Ambiguous Match error condition
    ///  occurs.</exception>
    /// <exception cref="ArgumentException">      Thrown when one or more arguments have unsupported or
    ///  illegal values.</exception>
    /// <param name="indexClass">The index class.</param>
    /// <param name="index">     Zero-based index of the.</param>
    /// <param name="value">     The value.</param>
    /// <param name="name">      The name.</param>
    /// <param name="ignoreCase">True to ignore case.</param>
    /// <param name="add">       True to add.</param>
    internal void TrySetValue(object indexClass, int index, object value, string name, bool ignoreCase, bool add)
    {
        FhirExpandoData data;
        object oldValue;

        if (value == null)
        {
            _ = TryDeleteValue(indexClass, index, name, ignoreCase, Uninitialized);
            return;
        }

        lock (_lockObject)
        {
            data = _data;

            if (data.Class != indexClass || ignoreCase)
            {
                // The class has changed or we are doing a case-insensitive search,
                // we need to get the correct index and set the value there.  If we
                // don't have the value then we need to promote the class - that
                // should only happen when we have multiple concurrent writers.
                index = data.Class.GetValueIndex(name, ignoreCase, this);
                if (index == FhirExpando.AmbiguousMatchFound)
                {
                    throw new System.Reflection.AmbiguousMatchException(name);
                }
                if (index == FhirExpando.NoMatch)
                {
                    // Before creating a new class with the new member, need to check
                    // if there is the exact same member but is deleted. We should reuse
                    // the class if there is such a member.
                    int exactMatch = ignoreCase ?
                        data.Class.GetValueIndexCaseSensitive(name) :
                        index;
                    if (exactMatch != FhirExpando.NoMatch)
                    {
                        Debug.Assert(data[exactMatch] == Uninitialized);
                        index = exactMatch;
                    }
                    else
                    {
                        FhirExpandoClass newClass = data.Class.FindNewClass(name);
                        data = PromoteClassCore(data.Class, newClass);

                        // After the class promotion, there must be an exact match,
                        // so we can do case-sensitive search here.
                        index = data.Class.GetValueIndexCaseSensitive(name);
                        Debug.Assert(index != FhirExpando.NoMatch);
                    }
                }
            }

            // Setting an uninitialized member increases the count of available members
            oldValue = data[index];
            if (oldValue == Uninitialized)
            {
                _count++;
            }
            else if (add)
            {
                throw new ArgumentException("Key already exists", name);
            }

            data[index] = value;
        }

        // Notify property changed, outside of the lock.
        var propertyChanged = _propertyChanged;
        if (propertyChanged != null && value != oldValue)
        {
            // Use the canonical case for the key.
            propertyChanged(this, new PropertyChangedEventArgs(data.Class.Keys[index]));
        }
    }

    /// <summary>Deletes the data stored for the specified class at the specified index.</summary>
    /// <exception cref="AmbiguousMatchException">Thrown when the Ambiguous Match error condition
    ///  occurs.</exception>
    /// <param name="indexClass"> The index class.</param>
    /// <param name="index">      Zero-based index of the.</param>
    /// <param name="name">       The name.</param>
    /// <param name="ignoreCase"> True to ignore case.</param>
    /// <param name="deleteValue">The delete value.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    internal bool TryDeleteValue(object indexClass, int index, string name, bool ignoreCase, object deleteValue)
    {
        FhirExpandoData data;
        lock (_lockObject)
        {
            data = _data;

            if (data.Class != indexClass || ignoreCase)
            {
                // the class has changed or we are doing a case-insensitive search,
                // we need to get the correct index.  If there is no associated index
                // we simply can't have the value and we return false.
                index = data.Class.GetValueIndex(name, ignoreCase, this);
                if (index == FhirExpando.AmbiguousMatchFound)
                {
                    throw new System.Reflection.AmbiguousMatchException(name);
                }
            }
            if (index == FhirExpando.NoMatch)
            {
                return false;
            }

            object oldValue = data[index];
            if (oldValue == Uninitialized)
            {
                return false;
            }

            // Make sure the value matches, if requested.
            //
            // It's a shame we have to call Equals with the lock held but
            // there doesn't seem to be a good way around that, and
            // ConcurrentDictionary in mscorlib does the same thing.
            if (deleteValue != Uninitialized && !object.Equals(oldValue, deleteValue))
            {
                return false;
            }

            data[index] = Uninitialized;

            // Deleting an available member decreases the count of available members
            _count--;
        }

        // Notify property changed, outside of the lock.
        var propertyChanged = _propertyChanged;
        if (propertyChanged != null)
        {
            // Use the canonical case for the key.
            propertyChanged(this, new PropertyChangedEventArgs(data.Class.Keys[index]));
        }

        return true;
    }

    /// <summary>
    /// Returns true if the member at the specified index has been deleted, otherwise false. Call
    /// this function holding the lock.
    /// </summary>
    /// <param name="index">Zero-based index of the.</param>
    /// <returns>True if deleted member, false if not.</returns>
    internal bool IsDeletedMember(int index)
    {
        Debug.Assert(index >= 0 && index <= _data.Length, "Index out of bounds");

        if (index == _data.Length)
        {
            // The member is a newly added by SetMemberBinder and not in data yet
            return false;
        }

        return _data[index] == FhirExpando.Uninitialized;
    }

    /// <summary>
    /// Gets the FhirExpandoClass which we've associated with this FhirExpando object.  Used for
    /// type checks in rules.
    /// </summary>
    internal FhirExpandoClass Class
    {
        get
        {
            return _data.Class;
        }
    }

    /// <summary>
    /// Promotes the class from the old type to the new type and returns the new FhirExpandoData object.
    /// </summary>
    /// <param name="oldClass">The old class.</param>
    /// <param name="newClass">The new class.</param>
    /// <returns>A FhirExpandoData.</returns>
    private FhirExpandoData PromoteClassCore(FhirExpandoClass oldClass, FhirExpandoClass newClass)
    {
        Debug.Assert(oldClass != newClass, "Incorrect class found during promotion");

        lock (_lockObject)
        {
            if (_data.Class == oldClass)
            {
                _data = _data.UpdateClass(newClass);
            }

            return _data;
        }
    }

    /// <summary>
    /// Internal helper to promote a class.  Called from our RuntimeOps helper.  This version simply
    /// doesn't expose the FhirExpandoData object which is a private data structure.
    /// </summary>
    /// <param name="oldClass">The old class.</param>
    /// <param name="newClass">The new class.</param>
    internal void PromoteClass(object oldClass, object newClass)
    {
        PromoteClassCore((FhirExpandoClass)oldClass, (FhirExpandoClass)newClass);
    }

    /// <summary>
    /// Returns the <see cref="T:System.Dynamic.DynamicMetaObject" /> responsible for binding
    /// operations performed on this object.
    /// </summary>
    /// <param name="parameter">The expression tree representation of the runtime value.</param>
    /// <returns>The <see cref="T:System.Dynamic.DynamicMetaObject" /> to bind this object.</returns>
    DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
    {
        return new MetaFhirExpando(parameter, this);
    }

    /// <summary>Try add member.</summary>
    /// <param name="key">  The key.</param>
    /// <param name="value">The value.</param>
    private void TryAddMember(string key, object value)
    {
        ContractUtils.RequiresNotNull(key, "key");

        // Pass null to the class, which forces lookup.
        TrySetValue(null, -1, value, key, false, true);
    }

    /// <summary>Attempts to get value for key an object from the given string.</summary>
    /// <param name="key">  The key.</param>
    /// <param name="value">[out] The value.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool TryGetValueForKey(string key, out object value)
    {
        // Pass null to the class, which forces lookup.
        return TryGetValue(null, -1, key, false, out value);
    }

    /// <summary>FhirExpando contains key.</summary>
    /// <param name="key">The key.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool FhirExpandoContainsKey(string key)
    {
        return _data.Class.GetValueIndexCaseSensitive(key) >= 0;
    }

    /// <summary>
    /// We create a non-generic type for the debug view for each different collection type that uses
    /// DebuggerTypeProxy, instead of defining a generic debug view type and using different
    /// instantiations. The reason for this is that support for generics with using DebuggerTypeProxy
    /// is limited. For C#, DebuggerTypeProxy supports only open types (from MSDN
    /// http://msdn.microsoft.com/en-us/library/d8eyd8zc.aspx).
    /// </summary>
    private sealed class KeyCollectionDebugView
    {
        /// <summary>The collection.</summary>
        private ICollection<string> collection;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyCollectionDebugView"/> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public KeyCollectionDebugView(ICollection<string> collection)
        {
            Debug.Assert(collection != null, "Cannot enumerate keys of null");
            this.collection = collection;
        }

        /// <summary>Gets the items.</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public string[] Items
        {
            get
            {
                string[] items = new string[collection.Count];
                collection.CopyTo(items, 0);
                return items;
            }
        }
    }

    /// <summary>Collection of keys.</summary>
    [DebuggerTypeProxy(typeof(KeyCollectionDebugView))]
    [DebuggerDisplay("Count = {Count}")]
    private class KeyCollection : ICollection<string>
    {
        /// <summary>(Immutable) The expando.</summary>
        private readonly FhirExpando _expando;

        /// <summary>(Immutable) The expando version.</summary>
        private readonly int _expandoVersion;

        /// <summary>(Immutable) Number of expandoes.</summary>
        private readonly int _expandoCount;

        /// <summary>(Immutable) Information describing the expando.</summary>
        private readonly FhirExpandoData _expandoData;

        /// <summary>Initializes a new instance of the <see cref="KeyCollection"/> class.</summary>
        /// <param name="expando">The expando.</param>
        internal KeyCollection(FhirExpando expando)
        {
            lock (expando._lockObject)
            {
                _expando = expando;
                _expandoVersion = expando._data.Version;
                _expandoCount = expando._count;
                _expandoData = expando._data;
            }
        }

        /// <summary>Check version.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        private void CheckVersion()
        {
            if (_expando._data.Version != _expandoVersion || _expandoData != _expando._data)
            {
                // the underlying expando object has changed
                throw new InvalidOperationException("Collection was modified while enumerating");
            }
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />
        ///  .</param>
        public void Add(string item)
        {
            throw new NotSupportedException("Collection is read only");
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        public void Clear()
        {
            throw new NotSupportedException("Collection is read only");
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a
        /// specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />
        ///  .</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />
        /// ; otherwise, <see langword="false" />.
        /// </returns>
        public bool Contains(string item)
        {
            lock (_expando._lockObject)
            {
                CheckVersion();
                return _expando.FhirExpandoContainsKey(item);
            }
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />
        /// , starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        /// <param name="array">     The one-dimensional <see cref="T:System.Array" /> that is the
        ///  destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />
        ///  . The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying
        ///  begins.</param>
        public void CopyTo(string[] array, int arrayIndex)
        {
            ContractUtils.RequiresNotNull(array, "array");
            ContractUtils.RequiresArrayRange(array, arrayIndex, _expandoCount, "arrayIndex", "Count");
            lock (_expando._lockObject)
            {
                CheckVersion();
                FhirExpandoData data = _expando._data;
                for (int i = 0; i < data.Class.Keys.Length; i++)
                {
                    if (data[i] != Uninitialized)
                    {
                        array[arrayIndex++] = data.Class.Keys[i];
                    }
                }
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />
        /// .
        /// </summary>
        public int Count
        {
            get
            {
                CheckVersion();
                return _expandoCount;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" />
        /// is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />
        /// .
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />
        ///  .</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />
        /// ; otherwise, <see langword="false" />. This method also returns <see langword="false" /> if <paramref name="item" />
        /// is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </returns>
        public bool Remove(string item)
        {
            throw new NotSupportedException("Collection is read only");
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<string> GetEnumerator()
        {
            for (int i = 0, n = _expandoData.Class.Keys.Length; i < n; i++)
            {
                CheckVersion();
                if (_expandoData[i] != Uninitialized)
                {
                    yield return _expandoData.Class.Keys[i];
                }
            }
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through
        /// the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }

    /// <summary>
    /// We create a non-generic type for the debug view for each different collection type that uses
    /// DebuggerTypeProxy, instead of defining a generic debug view type and using different
    /// instantiations. The reason for this is that support for generics with using DebuggerTypeProxy
    /// is limited. For C#, DebuggerTypeProxy supports only open types (from MSDN
    /// http://msdn.microsoft.com/en-us/library/d8eyd8zc.aspx).
    /// </summary>
    private sealed class ValueCollectionDebugView
    {
        /// <summary>The collection.</summary>
        private ICollection<object> collection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueCollectionDebugView"/> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public ValueCollectionDebugView(ICollection<object> collection)
        {
            Debug.Assert(collection != null, "Cannot enumerate values of null");
            this.collection = collection;
        }

        /// <summary>Gets the items.</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public object[] Items
        {
            get
            {
                object[] items = new object[collection.Count];
                collection.CopyTo(items, 0);
                return items;
            }
        }
    }

    /// <summary>Collection of values.</summary>
    [DebuggerTypeProxy(typeof(ValueCollectionDebugView))]
    [DebuggerDisplay("Count = {Count}")]
    private class ValueCollection : ICollection<object>
    {
        /// <summary>(Immutable) The expando.</summary>
        private readonly FhirExpando _expando;

        /// <summary>(Immutable) The expando version.</summary>
        private readonly int _expandoVersion;

        /// <summary>(Immutable) Number of expandoes.</summary>
        private readonly int _expandoCount;

        /// <summary>(Immutable) Information describing the expando.</summary>
        private readonly FhirExpandoData _expandoData;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueCollection"/> class.
        /// </summary>
        /// <param name="expando">The expando.</param>
        internal ValueCollection(FhirExpando expando)
        {
            lock (expando._lockObject)
            {
                _expando = expando;
                _expandoVersion = expando._data.Version;
                _expandoCount = expando._count;
                _expandoData = expando._data;
            }
        }

        /// <summary>Check version.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        private void CheckVersion()
        {
            if (_expando._data.Version != _expandoVersion || _expandoData != _expando._data)
            {
                // the underlying expando object has changed
                throw new InvalidOperationException("Collection was modified while enumerating");
            }
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />
        ///  .</param>
        public void Add(object item)
        {
            throw new NotSupportedException("Collection is read only");
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        public void Clear()
        {
            throw new NotSupportedException("Collection is read only");
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a
        /// specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />
        ///  .</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />
        /// ; otherwise, <see langword="false" />.
        /// </returns>
        public bool Contains(object item)
        {
            lock (_expando._lockObject)
            {
                CheckVersion();

                FhirExpandoData data = _expando._data;
                for (int i = 0; i < data.Class.Keys.Length; i++)
                {
                    // See comment in TryDeleteValue; it's okay to call
                    // object.Equals with the lock held.
                    if (object.Equals(data[i], item))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />
        /// , starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        /// <param name="array">     The one-dimensional <see cref="T:System.Array" /> that is the
        ///  destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />
        ///  . The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying
        ///  begins.</param>
        public void CopyTo(object[] array, int arrayIndex)
        {
            ContractUtils.RequiresNotNull(array, "array");
            ContractUtils.RequiresArrayRange(array, arrayIndex, _expandoCount, "arrayIndex", "Count");
            lock (_expando._lockObject)
            {
                CheckVersion();
                FhirExpandoData data = _expando._data;
                for (int i = 0; i < data.Class.Keys.Length; i++)
                {
                    if (data[i] != Uninitialized)
                    {
                        array[arrayIndex++] = data[i];
                    }
                }
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />
        /// .
        /// </summary>
        public int Count
        {
            get
            {
                CheckVersion();
                return _expandoCount;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" />
        /// is read-only.
        /// </summary>
        public bool IsReadOnly => true;

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />
        /// .
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />
        ///  .</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />
        /// ; otherwise, <see langword="false" />. This method also returns <see langword="false" /> if <paramref name="item" />
        /// is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </returns>
        public bool Remove(object item)
        {
            throw new NotSupportedException("Collection is read only");
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<object> GetEnumerator()
        {
            FhirExpandoData data = _expando._data;
            for (int i = 0; i < data.Class.Keys.Length; i++)
            {
                CheckVersion();

                // Capture the value into a temp so we don't inadvertently
                // return Uninitialized.
                object temp = data[i];
                if (temp != Uninitialized)
                {
                    yield return temp;
                }
            }
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through
        /// the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>
    /// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2" />
    /// .
    /// </summary>
    public ICollection<string> Keys => new KeyCollection(this);

    /// <summary>
    /// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in
    /// the <see cref="T:System.Collections.Generic.IDictionary`2" />.
    /// </summary>
    /// <typeparam name="string">Type of the string.</typeparam>
    /// <typeparam name="object">Type of the object.</typeparam>
    public ICollection<object> Values => new ValueCollection(this);

    /// <summary>Gets or sets the element with the specified key.</summary>
    /// <exception cref="KeyNotFoundException">Thrown when a Key Not Found error condition occurs.</exception>
    /// <typeparam name="string">Type of the string.</typeparam>
    /// <typeparam name="object">Type of the object.</typeparam>
    /// <param name="key">The key of the element to get or set.</param>
    /// <returns>The element with the specified key.</returns>
    public object this[string key]
    {
        get
        {
            if (!TryGetValueForKey(key, out object value))
            {
                return null;
            }

            return value;
        }

        set
        {
            ContractUtils.RequiresNotNull(key, "key");

            // Pass null to the class, which forces lookup.
            TrySetValue(null, -1, value, key, false, false);
        }
    }

    /// <summary>Gets or sets the element with the specified key.</summary>
    /// <param name="key1">The key of the element to get or set.</param>
    /// <param name="key2">The second key.</param>
    /// <returns>The element with the specified key.</returns>
    public object this[string key1, string key2]
    {
        get
        {
            if (!TryGetValueForKey(key1, out object v1))
            {
                return null;
            }

            if ((v1 is not FhirExpando) ||
                (!((FhirExpando)v1).TryGetValueForKey(key2, out object value)))
            {
                return null;
            }

            return value;
        }

        set
        {
            ContractUtils.RequiresNotNull(key1, "key1");
            ContractUtils.RequiresNotNull(key2, "key2");

            if ((!TryGetValueForKey(key1, out object v1)) ||
                (v1 is not FhirExpando))
            {
                return;
            }

            // Pass null to the class, which forces lookup.
            ((FhirExpando)v1).TrySetValue(null, -1, value, key2, false, false);
        }
    }

    /// <summary>
    /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2" />
    /// .
    /// </summary>
    /// <typeparam name="string">Type of the string.</typeparam>
    /// <typeparam name="object">Type of the object.</typeparam>
    /// <param name="key">  The object to use as the key of the element to add.</param>
    /// <param name="value">The object to use as the value of the element to add.</param>
    public void Add(string key, object value)
    {
        this.TryAddMember(key, value);
    }

    /// <summary>
    /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an
    /// element with the specified key.
    /// </summary>
    /// <typeparam name="string">Type of the string.</typeparam>
    /// <typeparam name="object">Type of the object.</typeparam>
    /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2" />
    ///  .</param>
    /// <returns>
    /// <see langword="true" /> if the <see cref="T:System.Collections.Generic.IDictionary`2" />
    /// contains an element with the key; otherwise, <see langword="false" />.
    /// </returns>
    public bool ContainsKey(string key)
    {
        ContractUtils.RequiresNotNull(key, "key");

        FhirExpandoData data = _data;
        int index = data.Class.GetValueIndexCaseSensitive(key);
        return index >= 0 && data[index] != Uninitialized;
    }

    /// <summary>
    /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2" />
    /// .
    /// </summary>
    /// <typeparam name="string">Type of the string.</typeparam>
    /// <typeparam name="object">Type of the object.</typeparam>
    /// <param name="key">The key of the element to remove.</param>
    /// <returns>
    /// <see langword="true" /> if the element is successfully removed; otherwise, <see langword="false" />
    /// .  This method also returns <see langword="false" /> if <paramref name="key" /> was not found
    /// in the original <see cref="T:System.Collections.Generic.IDictionary`2" />.
    /// </returns>
    public bool Remove(string key)
    {
        ContractUtils.RequiresNotNull(key, "key");

        // Pass null to the class, which forces lookup.
        return TryDeleteValue(null, -1, key, false, Uninitialized);
    }

    /// <summary>Gets the value associated with the specified key.</summary>
    /// <typeparam name="string">Type of the string.</typeparam>
    /// <typeparam name="object">Type of the object.</typeparam>
    /// <param name="key">  The key whose value to get.</param>
    /// <param name="value">[out] When this method returns, the value associated with the specified
    ///  key, if the key is found; otherwise, the default value for the type of the <paramref name="value" />
    ///  parameter. This parameter is passed uninitialized.</param>
    /// <returns>
    /// <see langword="true" /> if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" />
    /// contains an element with the specified key; otherwise, <see langword="false" />.
    /// </returns>
    public bool TryGetValue(string key, out object value)
    {
        return TryGetValueForKey(key, out value);
    }

    /// <summary>
    /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />
    /// .
    /// </summary>
    /// <typeparam name="string"> Type of the string.</typeparam>
    /// <typeparam name="object>">Type of the object></typeparam>
    public int Count => _count;

    /// <summary>
    /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" />
    /// is read-only.
    /// </summary>
    /// <typeparam name="string"> Type of the string.</typeparam>
    /// <typeparam name="object>">Type of the object></typeparam>
    public bool IsReadOnly => false;

    /// <summary>
    /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </summary>
    /// <typeparam name="string"> Type of the string.</typeparam>
    /// <typeparam name="object>">Type of the object></typeparam>
    /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />
    ///  .</param>
    public void Add(KeyValuePair<string, object> item)
    {
        TryAddMember(item.Key, item.Value);
    }

    /// <summary>
    /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </summary>
    /// <typeparam name="string"> Type of the string.</typeparam>
    /// <typeparam name="object>">Type of the object></typeparam>
    public void Clear()
    {
        // We remove both class and data!
        FhirExpandoData data;
        lock (_lockObject)
        {
            data = _data;
            _data = FhirExpandoData.Empty;
            _count = 0;
        }

        // Notify property changed for all properties.
        var propertyChanged = _propertyChanged;
        if (propertyChanged != null)
        {
            for (int i = 0, n = data.Class.Keys.Length; i < n; i++)
            {
                if (data[i] != Uninitialized)
                {
                    propertyChanged(this, new PropertyChangedEventArgs(data.Class.Keys[i]));
                }
            }
        }
    }

    /// <summary>
    /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a
    /// specific value.
    /// </summary>
    /// <typeparam name="string"> Type of the string.</typeparam>
    /// <typeparam name="object>">Type of the object></typeparam>
    /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />
    ///  .</param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />
    /// ; otherwise, <see langword="false" />.
    /// </returns>
    public bool Contains(KeyValuePair<string, object> item)
    {
        if (!TryGetValueForKey(item.Key, out object value))
        {
            return false;
        }

        return object.Equals(value, item.Value);
    }

    /// <summary>
    /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />
    /// , starting at a particular <see cref="T:System.Array" /> index.
    /// </summary>
    /// <typeparam name="string"> Type of the string.</typeparam>
    /// <typeparam name="object>">Type of the object></typeparam>
    /// <param name="array">     The one-dimensional <see cref="T:System.Array" /> that is the
    ///  destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />
    ///  . The <see cref="T:System.Array" /> must have zero-based indexing.</param>
    /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying
    ///  begins.</param>
    public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
    {
        ContractUtils.RequiresNotNull(array, "array");
        ContractUtils.RequiresArrayRange(array, arrayIndex, _count, "arrayIndex", "Count");

        // We want this to be atomic and not throw
        lock (_lockObject)
        {
            foreach (KeyValuePair<string, object> item in this)
            {
                array[arrayIndex++] = item;
            }
        }
    }

    /// <summary>
    /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />
    /// .
    /// </summary>
    /// <typeparam name="string"> Type of the string.</typeparam>
    /// <typeparam name="object>">Type of the object></typeparam>
    /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />
    ///  .</param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />
    /// ; otherwise, <see langword="false" />. This method also returns <see langword="false" /> if <paramref name="item" />
    /// is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </returns>
    public bool Remove(KeyValuePair<string, object> item)
    {
        return TryDeleteValue(null, -1, item.Key, false, item.Value ?? Uninitialized);
    }

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    /// <typeparam name="string"> Type of the string.</typeparam>
    /// <typeparam name="object>">Type of the object></typeparam>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        FhirExpandoData data = _data;
        return GetFhirExpandoEnumerator(data, data.Version);
    }

    /// <summary>Returns an enumerator that iterates through a collection.</summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through
    /// the collection.
    /// </returns>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        FhirExpandoData data = _data;
        return GetFhirExpandoEnumerator(data, data.Version);
    }

    /// <summary>
    /// Note: takes the data and version as parameters so they will be captured before the first call
    /// to MoveNext().
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="data">   The data.</param>
    /// <param name="version">The version.</param>
    /// <returns>The expando enumerator.</returns>
    private IEnumerator<KeyValuePair<string, object>> GetFhirExpandoEnumerator(FhirExpandoData data, int version)
    {
        for (int i = 0; i < data.Class.Keys.Length; i++)
        {
            if (_data.Version != version || data != _data)
            {
                // The underlying expando object has changed:
                // 1) the version of the expando data changed
                // 2) the data object is changed 
                throw new InvalidOperationException("Collection was modified while enumerating");
            }

            // Capture the value into a temp so we don't inadvertently
            // return Uninitialized.
            object temp = data[i];
            if (temp != Uninitialized)
            {
                yield return new KeyValuePair<string, object>(data.Class.Keys[i], temp);
            }
        }
    }

    /// <summary>A meta expando.</summary>
    private class MetaFhirExpando : DynamicMetaObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetaFhirExpando"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="value">     The value.</param>
        public MetaFhirExpando(Expression expression, FhirExpando value)
            : base(expression, BindingRestrictions.Empty, value)
        {
        }

        /// <summary>Bind get or invoke member.</summary>
        /// <param name="binder">        The binder.</param>
        /// <param name="name">          The name.</param>
        /// <param name="ignoreCase">    True to ignore case.</param>
        /// <param name="fallback">      The fallback.</param>
        /// <param name="fallbackInvoke">The fallback invoke.</param>
        /// <returns>A DynamicMetaObject.</returns>
        private DynamicMetaObject BindGetOrInvokeMember(
            DynamicMetaObjectBinder binder,
            string name,
            bool ignoreCase,
            DynamicMetaObject fallback,
            Func<DynamicMetaObject, DynamicMetaObject> fallbackInvoke)
        {
            FhirExpandoClass klass = Value.Class;

            // try to find the member, including the deleted members
            int index = klass.GetValueIndex(name, ignoreCase, Value);

            ParameterExpression value = Expression.Parameter(typeof(object), "value");

            Expression tryGetValue = Expression.Call(
                typeof(RuntimeOps).GetMethod("FhirExpandoTryGetValue"),
                GetLimitedSelf(),
                Expression.Constant(klass, typeof(object)),
                Expression.Constant(index),
                Expression.Constant(name),
                Expression.Constant(ignoreCase),
                value);

            var result = new DynamicMetaObject(value, BindingRestrictions.Empty);
            if (fallbackInvoke != null)
            {
                result = fallbackInvoke(result);
            }

            result = new DynamicMetaObject(
                Expression.Block(
                    new[] { value },
                    Expression.Condition(
                        tryGetValue,
                        result.Expression,
                        fallback.Expression,
                        typeof(object))),
                result.Restrictions.Merge(fallback.Restrictions));

            return AddDynamicTestAndDefer(binder, Value.Class, null, result);
        }

        /// <summary>Performs the binding of the dynamic get member operation.</summary>
        /// <param name="binder">An instance of the <see cref="T:System.Dynamic.GetMemberBinder" /> that
        ///  represents the details of the dynamic operation.</param>
        /// <returns>
        /// The new <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the
        /// binding.
        /// </returns>
        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
            ContractUtils.RequiresNotNull(binder, "binder");
            return BindGetOrInvokeMember(
                binder,
                binder.Name,
                binder.IgnoreCase,
                binder.FallbackGetMember(this),
                null);
        }

        /// <summary>Performs the binding of the dynamic invoke member operation.</summary>
        /// <param name="binder">An instance of the <see cref="T:System.Dynamic.InvokeMemberBinder" />
        ///  that represents the details of the dynamic operation.</param>
        /// <param name="args">  An array of <see cref="T:System.Dynamic.DynamicMetaObject" /> instances -
        ///  arguments to the invoke member operation.</param>
        /// <returns>
        /// The new <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the
        /// binding.
        /// </returns>
        public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
        {
            ContractUtils.RequiresNotNull(binder, "binder");
            return BindGetOrInvokeMember(
                binder,
                binder.Name,
                binder.IgnoreCase,
                binder.FallbackInvokeMember(this, args),
                value => binder.FallbackInvoke(value, args, null));
        }

        /// <summary>Performs the binding of the dynamic set member operation.</summary>
        /// <param name="binder">An instance of the <see cref="T:System.Dynamic.SetMemberBinder" /> that
        ///  represents the details of the dynamic operation.</param>
        /// <param name="value"> The <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the
        ///  value for the set member operation.</param>
        /// <returns>
        /// The new <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the
        /// binding.
        /// </returns>
        public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
        {
            ContractUtils.RequiresNotNull(binder, nameof(binder));
            ContractUtils.RequiresNotNull(value, nameof(value));

            FhirExpandoClass klass;
            int index;

            FhirExpandoClass originalClass = GetClassEnsureIndex(binder.Name, binder.IgnoreCase, Value, out klass, out index);

            return AddDynamicTestAndDefer(
                binder,
                klass,
                originalClass,
                new DynamicMetaObject(
                    Expression.Call(
                        typeof(RuntimeOps).GetMethod("FhirExpandoTrySetValue")!,
                        GetLimitedSelf(),
                        Expression.Constant(klass, typeof(object)),
                        Expression.Constant(index),
                        Expression.Convert(value.Expression, typeof(object)),
                        Expression.Constant(binder.Name),
                        Expression.Constant(binder.IgnoreCase)),
                    BindingRestrictions.Empty));
        }

        /// <summary>Performs the binding of the dynamic delete member operation.</summary>
        /// <param name="binder">An instance of the <see cref="T:System.Dynamic.DeleteMemberBinder" />
        ///  that represents the details of the dynamic operation.</param>
        /// <returns>
        /// The new <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the
        /// binding.
        /// </returns>
        public override DynamicMetaObject BindDeleteMember(DeleteMemberBinder binder)
        {
            ContractUtils.RequiresNotNull(binder, nameof(binder));

            int index = Value.Class.GetValueIndex(binder.Name, binder.IgnoreCase, Value);

            Expression tryDelete = Expression.Call(
                typeof(RuntimeOps).GetMethod("FhirExpandoTryDeleteValue")!,
                GetLimitedSelf(),
                Expression.Constant(Value.Class, typeof(object)),
                Expression.Constant(index),
                Expression.Constant(binder.Name),
                Expression.Constant(binder.IgnoreCase));

            DynamicMetaObject fallback = binder.FallbackDeleteMember(this);

            DynamicMetaObject target = new DynamicMetaObject(
                Expression.IfThen(Expression.Not(tryDelete), fallback.Expression),
                fallback.Restrictions);

            return AddDynamicTestAndDefer(binder, Value.Class, null, target);
        }

        /// <summary>Returns the enumeration of all dynamic member names.</summary>
        /// <returns>The list of dynamic member names.</returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            var expandoData = Value._data;
            var klass = expandoData.Class;
            for (int i = 0; i < klass.Keys.Length; i++)
            {
                object val = expandoData[i];
                if (val != FhirExpando.Uninitialized)
                {
                    yield return klass.Keys[i];
                }
            }
        }

        /// <summary>
        /// Adds a dynamic test which checks if the version has changed.  The test is only necessary for
        /// performance as the methods will do the correct thing if called with an incorrect version.
        /// </summary>
        /// <param name="binder">       The binder.</param>
        /// <param name="klass">        [out] The klass.</param>
        /// <param name="originalClass">The original class.</param>
        /// <param name="succeeds">     The succeeds.</param>
        /// <returns>A DynamicMetaObject.</returns>
        private DynamicMetaObject AddDynamicTestAndDefer(
            DynamicMetaObjectBinder binder,
            FhirExpandoClass klass,
            FhirExpandoClass originalClass,
            DynamicMetaObject succeeds)
        {
            Expression ifTestSucceeds = succeeds.Expression;
            if (originalClass != null)
            {
                // we are accessing a member which has not yet been defined on this class.
                // We force a class promotion after the type check.  If the class changes the
                // promotion will fail and the set/delete will do a full lookup using the new
                // class to discover the name.
                Debug.Assert(originalClass != klass, "Cannot access undefined member");

                ifTestSucceeds = Expression.Block(
                    Expression.Call(
                        null,
                        typeof(RuntimeOps).GetMethod("FhirExpandoPromoteClass"),
                        GetLimitedSelf(),
                        Expression.Constant(originalClass, typeof(object)),
                        Expression.Constant(klass, typeof(object))),
                    succeeds.Expression);
            }

            return new DynamicMetaObject(
                Expression.Condition(
                    Expression.Call(
                        null,
                        typeof(RuntimeOps).GetMethod("FhirExpandoCheckVersion"),
                        GetLimitedSelf(),
                        Expression.Constant(originalClass ?? klass, typeof(object))),
                    ifTestSucceeds,
                    binder.GetUpdateExpression(ifTestSucceeds.Type)),
                GetRestrictions().Merge(succeeds.Restrictions));
        }

        /// <summary>
        /// Gets the class and the index associated with the given name.  Does not update the expando
        /// object.  Instead this returns both the original and desired new class.  A rule is created
        /// which includes the test for the original class, the promotion to the new class, and the
        /// set/delete based on the class post-promotion.
        /// </summary>
        /// <param name="name">           The name.</param>
        /// <param name="caseInsensitive">True to case insensitive.</param>
        /// <param name="obj">            The object.</param>
        /// <param name="klass">          [out] The klass.</param>
        /// <param name="index">          [out] Zero-based index of the.</param>
        /// <returns>The class ensure index.</returns>
        private FhirExpandoClass GetClassEnsureIndex(
            string name,
            bool caseInsensitive,
            FhirExpando obj,
            out FhirExpandoClass klass,
            out int index)
        {
            FhirExpandoClass originalClass = Value.Class;

            index = originalClass.GetValueIndex(name, caseInsensitive, obj);
            if (index == FhirExpando.AmbiguousMatchFound)
            {
                klass = originalClass;
                return null;
            }

            if (index == FhirExpando.NoMatch)
            {
                // go ahead and find a new class now...
                FhirExpandoClass newClass = originalClass.FindNewClass(name);

                klass = newClass;
                index = newClass.GetValueIndexCaseSensitive(name);

                Debug.Assert(index != FhirExpando.NoMatch, "Referenced index does not exist!");
                return originalClass;
            }
            else
            {
                klass = originalClass;
                return null;
            }
        }

        /// <summary>Returns our Expression converted to our known LimitType.</summary>
        /// <returns>The limited self.</returns>
        private Expression GetLimitedSelf()
        {
            if ((Expression.Type == LimitType) || Expression.Type.IsEquivalentTo(LimitType))
            {
                return Expression;
            }

            return Expression.Convert(Expression, LimitType);
        }

        /// <summary>
        /// Returns a Restrictions object which includes our current restrictions merged with a
        /// restriction limiting our type.
        /// </summary>
        /// <returns>The restrictions.</returns>
        private BindingRestrictions GetRestrictions()
        {
            Debug.Assert(Restrictions == BindingRestrictions.Empty, "We don't merge, restrictions are always empty");

            return GetTypeRestriction(this);
        }

        /// <summary>Gets type restriction.</summary>
        /// <param name="obj">The object.</param>
        /// <returns>The type restriction.</returns>
        private static BindingRestrictions GetTypeRestriction(DynamicMetaObject obj)
        {
            if (obj.Value == null && obj.HasValue)
            {
                return BindingRestrictions.GetInstanceRestriction(obj.Expression, null);
            }
            else
            {
                return BindingRestrictions.GetTypeRestriction(obj.Expression, obj.LimitType);
            }
        }

        /// <summary>Gets the value.</summary>
        public new FhirExpando Value => (FhirExpando)base.Value!;
    }

    /// <summary>
    /// Stores the class and the data associated with the class as one atomic pair.  This enables us
    /// to do a class check in a thread safe manner w/o requiring locks.
    /// </summary>
    private class FhirExpandoData
    {
        /// <summary>The empty.</summary>
        internal static FhirExpandoData Empty = new FhirExpandoData();

        /// <summary>
        /// (Immutable)
        /// the dynamically assigned class associated with the FhirExpando object.
        /// </summary>
        internal readonly FhirExpandoClass Class;

        /// <summary>
        /// (Immutable)
        /// data stored in the expando object, key names are stored in the class.
        /// FhirExpando._data must be locked when mutating the value.  Otherwise a copy of it could be
        /// made and lose values.
        /// </summary>
        private readonly object[] _dataArray;

        /// <summary>Indexer for getting/setting the data.</summary>
        /// <param name="index">Zero-based index of the entry to access.</param>
        /// <returns>The indexed item.</returns>
        internal object this[int index]
        {
            get
            {
                return _dataArray[index];
            }

            set
            {
                // when the array is updated, version increases, even the new value is the same
                // as previous. Dictionary type has the same behavior.
                _version++;
                _dataArray[index] = value;
            }
        }

        /// <summary>Gets the version.</summary>
        internal int Version => _version;

        /// <summary>Gets the length.</summary>
        internal int Length
        {
            get { return _dataArray.Length; }
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="FhirExpandoData"/> class from being
        /// created.
        /// </summary>
        private FhirExpandoData()
        {
            Class = FhirExpandoClass.Empty;
            _dataArray = new object[0];
        }

        /// <summary>The version of the FhirExpando that tracks set and delete operations.</summary>
        private int _version;

        /// <summary>
        /// Initializes a new instance of the <see cref="FhirExpandoData"/> class
        /// with the specified class and data.</summary>
        /// <param name="klass">  The klass.</param>
        /// <param name="data">   The data.</param>
        /// <param name="version">the version of the FhirExpando that tracks set and delete operations.</param>
        internal FhirExpandoData(FhirExpandoClass klass, object[] data, int version)
        {
            Class = klass;
            _dataArray = data;
            _version = version;
        }

        /// <summary>
        /// Update the associated class and increases the storage for the data array if needed.
        /// </summary>
        /// <param name="newClass">The new class.</param>
        /// <returns>A FhirExpandoData.</returns>
        internal FhirExpandoData UpdateClass(FhirExpandoClass newClass)
        {
            if (_dataArray.Length >= newClass.Keys.Length)
            {
                // we have extra space in our buffer, just initialize it to Uninitialized.
                this[newClass.Keys.Length - 1] = FhirExpando.Uninitialized;
                return new FhirExpandoData(newClass, this._dataArray, this._version);
            }
            else
            {
                // we've grown too much - we need a new object array
                int oldLength = _dataArray.Length;
                object[] arr = new object[GetAlignedSize(newClass.Keys.Length)];
                Array.Copy(_dataArray, arr, _dataArray.Length);
                FhirExpandoData newData = new FhirExpandoData(newClass, arr, this._version);
                newData[oldLength] = FhirExpando.Uninitialized;
                return newData;
            }
        }

        /// <summary>Gets aligned size.</summary>
        /// <param name="len">The length.</param>
        /// <returns>The aligned size.</returns>
        private static int GetAlignedSize(int len)
        {
            // the alignment of the array for storage of values (must be a power of two)
            const int DataArrayAlignment = 8;

            // round up and then mask off lower bits
            return (len + (DataArrayAlignment - 1)) & (~(DataArrayAlignment - 1));
        }
    }

    /// <summary>Occurs when a property value changes.</summary>
    event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
    {
        add { _propertyChanged += value; }
        remove { _propertyChanged -= value; }
    }
}
