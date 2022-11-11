// <copyright file="FhirValueSetFilter.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>A fhir value set filter.</summary>
public class FhirValueSetFilter : ICloneable
{
    /// <summary>Operation equality test.</summary>
    public const string FilterOperationEquals = "=";

    /// <summary>The filter operation is a.</summary>
    public const string FilterOperationIsA = "is-a";

    /// <summary>The filter operation descendent of.</summary>
    public const string FilterOperationDescendentOf = "descendent-of";

    /// <summary>The filter operation is not a.</summary>
    public const string FilterOperationIsNotA = "is-not-a";

    /// <summary>The filter operation RegEx.</summary>
    public const string FilterOperationRegex = "regex";

    /// <summary>The filter operation in.</summary>
    public const string FilterOperationIn = "in";

    /// <summary>The filter operation not in.</summary>
    public const string FilterOperationNotIn = "not-in";

    /// <summary>The filter operation generalizes.</summary>
    public const string FilterOperationGeneralizes = "generalizes";

    /// <summary>The filter operation exists.</summary>
    public const string FilterOperationExists = "exists";

    /// <summary>The filter operations.</summary>
    public static readonly string[] FilterOperations =
    {
        FilterOperationDescendentOf,
        FilterOperationEquals,
        FilterOperationExists,
        FilterOperationGeneralizes,
        FilterOperationIn,
        FilterOperationIsA,
        FilterOperationIsNotA,
        FilterOperationNotIn,
        FilterOperationRegex,
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirValueSetFilter"/> class.
    /// </summary>
    /// <param name="property"> The property.</param>
    /// <param name="operation">The operation.</param>
    /// <param name="value">    The value.</param>
    public FhirValueSetFilter(
        string property,
        string operation,
        string value)
    {
        Property = property;
        Operation = operation;
        Value = value;
    }

    /// <summary>Initializes a new instance of the <see cref="FhirValueSetFilter"/> class.</summary>
    /// <param name="source">Source to copy.</param>
    public FhirValueSetFilter(FhirValueSetFilter source)
    {
        Property = source.Property;
        Operation = source.Operation;
        Value = source.Value;
    }

    /// <summary>Gets the property.</summary>
    /// <value>The property.</value>
    public string Property { get; }

    /// <summary>Gets the operation.</summary>
    /// <value>The operation.</value>
    public string Operation { get; }

    /// <summary>Gets the value.</summary>
    /// <value>The value.</value>
    public string Value { get; }

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    public object Clone()
    {
        return new FhirValueSetFilter(this);
    }
}
