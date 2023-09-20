// <copyright file="FhirResourceBase.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Structure;
using static Microsoft.Health.Fhir.CodeGenCommon.Extensions.FhirNameConventionExtensions;
using Microsoft.Health.Fhir.CodeGenCommon.BaseModels;

namespace Microsoft.Health.Fhir.CodeGenCommon.Instance;

/// <summary>A FHIR resource base.</summary>
public abstract record class FhirResourceBase : FhirArtifactBase, ICloneable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FhirResourceBase"/> class.
    /// </summary>
    public FhirResourceBase() : base() { }

    /// <summary>Initializes a new instance of the FhirDefinitionBase class.</summary>
    /// <param name="other">The other.</param>
    [SetsRequiredMembers]
    protected FhirResourceBase(FhirResourceBase other)
        : base(other)
    {
        Meta = other.Meta == null ? null : other.Meta with { };
        Text = other.Text == null ? null : other.Text with { };
        Language = other.Language;
    }

    /// <summary>Gets or initializes the FHIR metadata about this resource.</summary>
    public FhirMeta? Meta { get; init; } = null;

    /// <summary>Gets or initializes the text summary of the resource.</summary>
    public FhirNarrative? Text { get; init; } = null;

    /// <summary>Gets or initializes the language of the resource content.</summary>
    public string Language { get; init; } = string.Empty;

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    object ICloneable.Clone() => this with { };
}
