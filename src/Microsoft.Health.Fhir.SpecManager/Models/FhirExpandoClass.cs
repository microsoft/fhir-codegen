// <copyright file="FhirExpandoClass.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.SpecManager.Models;

/// <summary>A FHIR expando class.</summary>
public class FhirExpandoClass
{
    private readonly string[] _keys;                            // list of names associated with each element in the data array, sorted
    private readonly int _hashCode;                             // pre-calculated hash code of all the keys the class contains
    private Dictionary<int, List<WeakReference>> _transitions;  // cached transitions

    private const int EmptyHashCode = 6551;                     // hash code of the empty FhirExpandoClass.

    /// <summary>
    /// The empty FhirExpando class - all FhirExpando objects start off w/ this class.
    /// </summary>
    public static FhirExpandoClass Empty = new FhirExpandoClass();

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirExpandoClass"/> class. This is the class used
    /// when an empty FhirExpando is initially constructed.
    /// </summary>
    public FhirExpandoClass()
    {
        _hashCode = EmptyHashCode;
        _keys = new string[0];
        _transitions = new();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirExpandoClass"/> class. Constructs an instance
    /// that can hold onto the specified keys.  The keys must be sorted ordinally.  The hash code
    /// must be precalculated for the keys.
    /// </summary>
    /// <param name="keys">    Gets the names of the keys that can be stored in the FhirExpando
    ///  class.  The list is sorted ordinally.</param>
    /// <param name="hashCode">The hash code.</param>
    public FhirExpandoClass(string[] keys, int hashCode)
    {
        _hashCode = hashCode;
        _keys = keys;
        _transitions = new();
    }

    /// <summary>
    /// Finds or creates a new FhirExpandoClass given the existing set of keys in this
    /// FhirExpandoClass plus the new key to be added. Members in an FhirExpandoClass are always
    /// stored case sensitively.
    /// </summary>
    /// <param name="newKey">The new key.</param>
    /// <returns>The found new class.</returns>
    public FhirExpandoClass FindNewClass(string newKey)
    {
        // just XOR the newKey hash code 
        int hashCode = _hashCode ^ newKey.GetHashCode();

        lock (this)
        {
            List<WeakReference> infos = GetTransitionList(hashCode);

            for (int i = 0; i < infos.Count; i++)
            {
                FhirExpandoClass? klass = infos[i].Target as FhirExpandoClass;
                if (klass == null)
                {
                    infos.RemoveAt(i);
                    i--;
                    continue;
                }

                if (string.Equals(klass._keys[klass._keys.Length - 1], newKey, StringComparison.Ordinal))
                {
                    // the new key is the key we added in this transition
                    return klass;
                }
            }

            // no applicable transition, create a new one
            string[] keys = new string[_keys.Length + 1];
            Array.Copy(_keys, keys, _keys.Length);
            keys[_keys.Length] = newKey;
            FhirExpandoClass ec = new FhirExpandoClass(keys, hashCode);

            infos.Add(new WeakReference(ec));
            return ec;
        }
    }

    /// <summary>
    /// Gets the lists of transitions that are valid from this FhirExpandoClass
    /// to an FhirExpandoClass whos keys hash to the apporopriate hash code.
    /// </summary>
    private List<WeakReference> GetTransitionList(int hashCode)
    {
        if (_transitions == null)
        {
            _transitions = new Dictionary<int, List<WeakReference>>();
        }

        List<WeakReference>? infos;
        if (!_transitions.TryGetValue(hashCode, out infos))
        {
            _transitions[hashCode] = infos = new List<WeakReference>();
        }

        return infos!;
    }

    /// <summary>
    /// Gets the index at which the value should be stored for the specified name.
    /// </summary>
    public int GetValueIndex(string name, bool caseInsensitive, FhirExpando obj)
    {
        if (caseInsensitive)
        {
            return GetValueIndexCaseInsensitive(name, obj);
        }
        else
        {
            return GetValueIndexCaseSensitive(name);
        }
    }

    /// <summary>
    /// Gets the index at which the value should be stored for the specified name
    /// case sensitively. Returns the index even if the member is marked as deleted.
    /// </summary>
    public int GetValueIndexCaseSensitive(string name)
    {
        for (int i = 0; i < _keys.Length; i++)
        {
            if (string.Equals(
                _keys[i],
                name,
                StringComparison.Ordinal))
            {
                return i;
            }
        }
        return FhirExpando.NoMatch;
    }

    /// <summary>
    /// Gets the index at which the value should be stored for the specified name,
    /// the method is only used in the case-insensitive case.
    /// </summary>
    /// <param name="name">the name of the member</param>
    /// <param name="obj">The FhirExpando associated with the class
    /// that is used to check if a member has been deleted.</param>
    /// <returns>
    /// the exact match if there is one
    /// if there is exactly one member with case insensitive match, return it
    /// otherwise we throw AmbiguousMatchException.
    /// </returns>
    private int GetValueIndexCaseInsensitive(string name, FhirExpando obj)
    {
        int caseInsensitiveMatch = FhirExpando.NoMatch; //the location of the case-insensitive matching member
        lock (obj._lockObject)
        {
            for (int i = _keys.Length - 1; i >= 0; i--)
            {
                if (string.Equals(
                    _keys[i],
                    name,
                    StringComparison.OrdinalIgnoreCase))
                {
                    //if the matching member is deleted, continue searching
                    if (!obj.IsDeletedMember(i))
                    {
                        if (caseInsensitiveMatch == FhirExpando.NoMatch)
                        {
                            caseInsensitiveMatch = i;
                        }
                        else
                        {
                            //Ambigous match, stop searching
                            return FhirExpando.AmbiguousMatchFound;
                        }
                    }
                }
            }
        }
        //There is exactly one member with case insensitive match.
        return caseInsensitiveMatch;
    }

    /// <summary>
    /// Gets the names of the keys that can be stored in the FhirExpando class.  The
    /// list is sorted ordinally.
    /// </summary>
    public string[] Keys
    {
        get
        {
            return _keys;
        }
    }
}
