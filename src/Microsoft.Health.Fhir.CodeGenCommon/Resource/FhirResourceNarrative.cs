// <copyright file="FhirResourceNarrative.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;

namespace Microsoft.Health.Fhir.CodeGenCommon.Resource;

/// <summary>A FHIR resource narrative.</summary>
public class FhirResourceNarrative : ICloneable
{
    private string _fhirStatus = string.Empty;
    private NarrativeStatusEnum _status = NarrativeStatusEnum.Empty;

    /// <summary>Values that represent narrative status enums.</summary>
    public enum NarrativeStatusEnum
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

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirResourceNarrative"/> class.
    /// </summary>
    public FhirResourceNarrative() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirResourceNarrative"/> class.
    /// </summary>
    /// <param name="other">The other.</param>
    [SetsRequiredMembers]
    public FhirResourceNarrative(FhirResourceNarrative other)
    {
        FhirStatus = other._fhirStatus;
        Div = other.Div;
    }

    /// <summary>Gets the status.</summary>
    public NarrativeStatusEnum Status { get => _status; }

    /// <summary>Gets or initializes the status literal.</summary>
    public required string FhirStatus
    {
        get => _fhirStatus;
        init
        {
            _fhirStatus = value;
            if (_fhirStatus.TryFhirEnum(out NarrativeStatusEnum v))
            {
                _status = v;
            }
        }
    }

    /// <summary>Gets the div.</summary>
    public required string Div { get; init; }

    /// <summary>Creates a new object that is a copy of the current instance.</summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    object ICloneable.Clone() => new FhirResourceNarrative(this);
}
