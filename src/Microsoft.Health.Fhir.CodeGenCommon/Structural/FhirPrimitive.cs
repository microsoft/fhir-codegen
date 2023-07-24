// <copyright file="FhirPrimitive.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Data;

namespace Microsoft.Health.Fhir.CodeGenCommon.Structural;

/// <summary>A FHIR primitive type.</summary>
public record class FhirPrimitive : FhirModelBase
{
    /// <summary>Initializes a new instance of the FhirPrimitive class.</summary>
    /// <param name="other">The other.</param>
    protected FhirPrimitive(FhirPrimitive other)
        : base(other)
    {
    }
}
