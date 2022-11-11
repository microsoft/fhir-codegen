// <copyright file="LinqExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


namespace Microsoft.Health.Fhir.CodeGenCommon.Extensions;

/// <summary>A linq extensions.</summary>
public static class LinqExtensions
{
    /// <summary>
    /// An IEnumerable&lt;T&gt; extension method that applies an operation to all items in this
    /// collection.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="ie">    The IE to act on.</param>
    /// <param name="action">The action.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public static bool ForEach<T>(this IEnumerable<T> ie, Func<T, int, bool> action)
    {
        int i = 0;
        foreach (T e in ie)
        {
            if (!action(e, i++))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// A Dictionary&lt;KT,VT&gt; extension method that deep copies the dictionary.
    /// </summary>
    /// <typeparam name="KT">Key Type.</typeparam>
    /// <typeparam name="VT">Value Type.</typeparam>
    /// <param name="source">The source dictionary to copy.</param>
    /// <returns>A Dictionary&lt;KT,VT&gt;</returns>
    public static Dictionary<KT,VT> DeepCopy<KT, VT>(this Dictionary<KT, VT> source)
        where VT : ICloneable
    {
        Dictionary<KT, VT> dest = new();

        foreach ((KT key, VT value) in source)
        {
            dest.Add(key, (VT)value.Clone());
        }

        return dest;
    }

    /// <summary>
    /// A Dictionary&lt;KT,VT&gt; extension method that shallow copies the given source.
    /// </summary>
    /// <typeparam name="KT">Key Type.</typeparam>
    /// <typeparam name="VT">Value Type.</typeparam>
    /// <param name="source">The source dictionary to copy.</param>
    /// <returns>A Dictionary&lt;KT,VT&gt;</returns>
    public static Dictionary<KT, VT> ShallowCopy<KT, VT>(this Dictionary<KT, VT> source)
    {
        Dictionary<KT, VT> dest = new();

        foreach ((KT key, VT value) in source)
        {
            dest.Add(key, value);
        }

        return dest;
    }
}
