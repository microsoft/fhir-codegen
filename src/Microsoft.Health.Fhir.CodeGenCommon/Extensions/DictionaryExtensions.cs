// <copyright file="DictionaryExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.CodeGenCommon.Extensions;

public static class DictionaryExtensions
{
    public static T AddToValue<T>(this Dictionary<string, List<T>> d, string key, T value)
    {
        if (!d.TryGetValue(key, out List<T>? l))
        {
            l = new List<T>();
            d[key] = l;
        }

        l.Add(value);
        return value;
    }
}
