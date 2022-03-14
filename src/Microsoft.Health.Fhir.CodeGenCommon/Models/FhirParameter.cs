// <copyright file="FhirParameter.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>A fhir parameter.</summary>
public class FhirParameter
{
    /// <summary>Initializes a new instance of the <see cref="FhirParameter"/> class.</summary>
    /// <param name="name">         The name.</param>
    /// <param name="use">          The use.</param>
    /// <param name="min">          The minimum value.</param>
    /// <param name="max">          The maximum value.</param>
    /// <param name="documentation">The documentation.</param>
    /// <param name="valueType">    The type.</param>
    /// <param name="fieldOrder">   The field order.</param>
    public FhirParameter(
        string name,
        string use,
        int min,
        string max,
        string documentation,
        string valueType,
        int fieldOrder)
    {
        Name = name;
        Use = use;
        Min = min;
        Documentation = documentation;
        ValueType = valueType;
        FieldOrder = fieldOrder;

        if (string.IsNullOrEmpty(max) || (max == "*"))
        {
            Max = null;
        }
        else
        {
            if (int.TryParse(max, out int parsed))
            {
                Max = parsed;
            }
        }
    }

    /// <summary>Initializes a new instance of the <see cref="FhirParameter"/> class.</summary>
    /// <param name="name">         The name.</param>
    /// <param name="use">          The use.</param>
    /// <param name="min">          The minimum value.</param>
    /// <param name="max">          The maximum value.</param>
    /// <param name="documentation">The documentation.</param>
    /// <param name="valueType">    The type.</param>
    /// <param name="fieldOrder">   The field order.</param>
    [System.Text.Json.Serialization.JsonConstructor]
    public FhirParameter(
        string name,
        string use,
        int min,
        int? max,
        string documentation,
        string valueType,
        int fieldOrder)
    {
        Name = name;
        Use = use;
        Min = min;
        Documentation = documentation;
        ValueType = valueType;
        FieldOrder = fieldOrder;
        Max = max;
    }

    /// <summary>Gets the name.</summary>
    /// <value>The name.</value>
    public string Name { get; }

    /// <summary>Gets the use.</summary>
    /// <value>The use.</value>
    public string Use { get; }

    /// <summary>Gets the minimum.</summary>
    /// <value>The minimum value.</value>
    public int Min { get; }

    /// <summary>Gets the maximum.</summary>
    /// <value>The maximum value.</value>
    public int? Max { get; }

    /// <summary>Gets the cardinality maximum string.</summary>
    /// <value>The cardinality maximum string.</value>
    public string MaxString
    {
        get
        {
            if ((Max == null) || (Max == -1))
            {
                return "*";
            }

            return Max.ToString();
        }
    }

    /// <summary>Gets the FHIR cardinality string: min..max.</summary>
    /// <value>The FHIR cardinality.</value>
    public string FhirCardinality => $"{Min}..{MaxString}";

    /// <summary>Gets the documentation.</summary>
    /// <value>The documentation.</value>
    public string Documentation { get; }

    /// <summary>Gets the value type.</summary>
    /// <value>The value type.</value>
    public string ValueType { get; }

    /// <summary>Gets the field order.</summary>
    /// <value>The field order.</value>
    public int FieldOrder { get; }

    /// <summary>Deep copy.</summary>
    /// <returns>A FhirParameter.</returns>
    public FhirParameter DeepCopy()
    {
        return new FhirParameter(
            Name,
            Use,
            Min,
            (Max == null) ? "*" : Max.ToString()!,
            Documentation,
            ValueType,
            FieldOrder);
    }
}
