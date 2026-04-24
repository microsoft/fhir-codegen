// <copyright file="DictionaryExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Fhir.CodeGen.Common.Extensions;

public static class DictionaryExtensions
{
    public static V AddToValue<K, V>(this Dictionary<K, List<V>> d, K key, V value)
        where K : notnull
    {
        if (!d.TryGetValue(key, out List<V>? l))
        {
            l = new List<V>();
            d[key] = l;
        }

        l.Add(value);
        return value;
    }

    public static V AddToValue<K, V>(this Dictionary<K, HashSet<V>> d, K key, V value)
        where K : notnull
    {
        if (!d.TryGetValue(key, out HashSet<V>? l))
        {
            l = new HashSet<V>();
            d[key] = l;
        }

        l.Add(value);
        return value;
    }
}
