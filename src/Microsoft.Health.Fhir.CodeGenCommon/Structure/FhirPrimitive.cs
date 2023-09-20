// <copyright file="FhirPrimitive.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Health.Fhir.CodeGenCommon.Structure;

/// <summary>A FHIR primitive type.</summary>
public record class FhirPrimitive : FhirStructureBase, ICloneable
{
    /// <summary>Initializes a new instance of the FhirPrimitive class.</summary>
    public FhirPrimitive() : base() { }

    /// <summary>Initializes a new instance of the FhirPrimitive class.</summary>
    /// <param name="other">The other.</param>
    [SetsRequiredMembers]
    protected FhirPrimitive(FhirPrimitive other)
        : base(other)
    {
    }

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    object ICloneable.Clone() => this with { };
}
