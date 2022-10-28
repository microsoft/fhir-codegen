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
}
