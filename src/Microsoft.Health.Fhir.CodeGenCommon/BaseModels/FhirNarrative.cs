// <copyright file="FhirNarrative.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;

namespace Microsoft.Health.Fhir.CodeGenCommon.BaseModels;

/// <summary>A FHIR narrative.</summary>
public record class FhirNarrative : FhirBase, ICloneable
{
    /// <summary>Values that represent narrative status codes.</summary>
    public enum NarrativeStatusCodes
    {
        /// <summary>The contents of the narrative are entirely generated from the core elements in the content.</summary>
        [FhirLiteral("generated")]
        Generated,

        /// <summary>
        /// The contents of the narrative are entirely generated from the core elements in the content and some of
        /// the content is generated from extensions. The narrative SHALL reflect the impact of all modifier
        /// extensions.
        /// </summary>
        [FhirLiteral("extensions")]
        Extensions,

        /// <summary>
        /// The contents of the narrative may contain additional information not found in the structured data.
        /// Note that there is no computable way to determine what the extra information is, other than by human
        /// inspection.
        /// </summary>
        [FhirLiteral("additional")]
        Additional,

        /// <summary>The contents of the narrative are some equivalent of "No human-readable text provided in this case".</summary>
        [FhirLiteral("empty")]
        Empty,
    }

    private NarrativeStatusCodes _status = NarrativeStatusCodes.Empty;
    private string _statusLiteral = string.Empty;

    /// <summary>Initializes a new instance of the FhirNarrative class.</summary>
    public FhirNarrative() : base() { }

    /// <summary>Initializes a new instance of the FhirNarrative class.</summary>
    /// <param name="other">The other.</param>
    [SetsRequiredMembers]
    public FhirNarrative(FhirNarrative other)
        : base(other)
    {
        StatusLiteral = other.StatusLiteral;
        Div = other.Div;
    }

    /// <summary>Gets the status.</summary>
    public NarrativeStatusCodes Status { get => _status; }

    /// <summary>Gets or initializes the status literal.</summary>
    public required string StatusLiteral
    {
        get => _statusLiteral;
        init
        {
            _statusLiteral = value;
            _status = value.ToEnum<NarrativeStatusCodes>() ?? NarrativeStatusCodes.Empty;
        }
    }

    /// <summary>Gets or initializes the div.</summary>
    public required string Div { get; init; }

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    object ICloneable.Clone() => this with { };
}
