// <copyright file="RecordExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using Microsoft.Health.Fhir.CodeGenCommon.Models;

namespace Microsoft.Health.Fhir.CodeGenCommon.Extensions;

/// <summary>A record extensions.</summary>
public static class RecordExtensions
{

    /// <summary>
    /// A ValWithExpectation&lt;T&gt; extension method that convert this object into a string
    /// representation.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="v">The v to act on.</param>
    /// <returns>V as a string.</returns>
    public static string ToStringWithExpectation<T>(this FhirCapabiltyStatement.ValWithExpectation<T> v)
    {
        if (string.IsNullOrEmpty(v.ExpectationLiteral))
        {
            return v.Value?.ToString() ?? string.Empty;
        }

        return $"{v.Value} ({v.ExpectationLiteral})";
    }

}
