// <copyright file="FhirSearchParamComponent.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>A fhir search parameter.</summary>
public class FhirSearchParamComponent : ICloneable
{

    /// <summary>Initializes a new instance of the <see cref="FhirSearchParamComponent"/> class.</summary>
    /// <param name="definition">       The definition.</param>
    /// <param name="expression">       The expression.</param>
    [System.Text.Json.Serialization.JsonConstructor]
    public FhirSearchParamComponent(
        string definition,
        string expression)
    {
        Definition = definition ?? string.Empty;
        Expression = expression ?? string.Empty;
    }

    /// <summary>Gets the expression.</summary>
    public string Expression { get; }

    /// <summary>Gets the definition.</summary>
    public string Definition { get; }

    public FhirSearchParam DefinitionParam { get; set; }

    /// <summary>Deep copy.</summary>
    /// <returns>A FhirSearchParam.</returns>
    public object Clone()
    {
        return new FhirSearchParamComponent(
            Definition,
            Expression);
    }
}
