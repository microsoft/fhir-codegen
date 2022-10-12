// <copyright file="FhirNarrative.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>A FHIR text element (Narrative).</summary>
public class FhirNarrative
{
    /// <summary>Values that represent narrative status enums.</summary>
    public enum NarrativeStatusEnum
    {
        generated,
        extensions,
        additional,
        empty,
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirNarrative"/> class.
    /// </summary>
    /// <param name="status">The status.</param>
    /// <param name="div">   The div.</param>
    public FhirNarrative(string status, string div)
    {
        Status = (NarrativeStatusEnum)Enum.Parse(typeof(NarrativeStatusEnum), status, true);
        Div = div;
    }

    /// <summary>Initializes a new instance of the <see cref="FhirNarrative"/> class.</summary>
    /// <param name="status">The status.</param>
    /// <param name="div">   The div.</param>
    public FhirNarrative(NarrativeStatusEnum status, string div)
    {
        Status = status;
        Div = div;
    }

    /// <summary>Gets the status.</summary>
    public NarrativeStatusEnum Status { get; set; }

    /// <summary>Gets the div.</summary>
    public string Div { get; set; }
}
