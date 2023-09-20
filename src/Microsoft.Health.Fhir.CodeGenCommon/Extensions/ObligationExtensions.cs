// <copyright file="ObligationExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Runtime.CompilerServices;
using Microsoft.Health.Fhir.CodeGenCommon.Expectations;

namespace Microsoft.Health.Fhir.CodeGenCommon.Extensions;

/// <summary>An obligation extensions.</summary>
public static class ObligationExtensions
{
    /// <summary>
    /// A WithObsVal&lt;string&gt; extension method that converts a val to an enum.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="val">The val to act on.</param>
    /// <returns>Val as a WithObsEnum&lt;T&gt;?</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConformanceEnum<T>? ToEnum<T>(
        this ConformanceVal<string>? val)
        where T : struct, System.Enum
    {
        if (val?.Value.TryFhirEnum(out T v) ?? false)
        {
            return new ConformanceEnum<T>()
            {
                Value = v,
                ObligationsByActor = val?.ObligationsByActor.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Select(kv => kv with { })) ?? new(),
            };
        }

        return null;
    }

    /// <summary>
    /// A WithObsVal&lt;string&gt; extension method that converts a val to an enum.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="val">The val to act on.</param>
    /// <returns>Val as a WithObsEnum&lt;T&gt;?</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConformanceEnum<T> ToEnum<T>(
        this ConformanceVal<string>? val,
        T defaultVal)
        where T : struct, System.Enum
    {
        if (val?.Value.TryFhirEnum(out T v) ?? false)
        {
            return new ConformanceEnum<T>()
            {
                Value = v,
                ObligationsByActor = val.ObligationsByActor.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Select(kv => kv with { })),
            };
        }

        return new ConformanceEnum<T>()
        {
            Value = (T)defaultVal,
            ObligationsByActor = val?.ObligationsByActor.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Select(kv => kv with { })) ?? new(),
        };
    }

    /// <summary>
    /// A WithObsVal&lt;string&gt; extension method that converts a val to an enum.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="val">The val to act on.</param>
    /// <returns>Val as a WithObsEnum&lt;T&gt;?</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<ConformanceEnum<T>> ToEnum<T>(this IEnumerable<ConformanceVal<string>> source)
        where T : struct, System.Enum
    {
        List<ConformanceEnum<T>> list = new();

        foreach (ConformanceVal<string> val in source)
        {
            if (val.Value.TryFhirEnum(out T v))
            {
                list.Add(new ConformanceEnum<T>()
                {
                    Value = v,
                    ObligationsByActor = val.ObligationsByActor.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Select(kv => kv with { })),
                });
            }
        }

        return list.AsEnumerable();
    }
}
