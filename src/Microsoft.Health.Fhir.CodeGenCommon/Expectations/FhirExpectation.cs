// <copyright file="FhirExpectation.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;

namespace Microsoft.Health.Fhir.CodeGenCommon.Expectations;

/// <summary>A FHIR expectation.</summary>
public record class FhirExpectation : ICloneable
{
    /// <summary>Values that represent expectation codes.</summary>
    public enum ExpectationCodes
    {
        /// <summary>No obligation or conformance expectation has been specified.</summary>
        [FhirLiteral("")]
        NotSpecified,

        /// <summary>
        /// The functional requirement is mandatory. Applications that do not implement this functional behavior are considered
        /// non-conformant.
        /// </summary>
        [FhirLiteral("SHALL")]
        Shall,

        /// <summary>The functional requirement is a recommendation.</summary>
        [FhirLiteral("SHOULD")]
        Should,

        /// <summary>
        /// The functional requirement is presented as an option for applications to consider. Note that this is usually used to
        /// indicate a choice is still valid for an application to make.
        /// </summary>
        [FhirLiteral("MAY")]
        May,
    }

    /// <summary>Initializes a new instance of the FhirExpectation class.</summary>
    public FhirExpectation() { }

    [SetsRequiredMembers]
    protected FhirExpectation(FhirExpectation other)
    {
        ExpectationLiteral = other.ExpectationLiteral;
    }

    private readonly ExpectationCodes _expectation = ExpectationCodes.NotSpecified;
    private string _expectationLiteral = string.Empty;

    /// <summary>Gets the expectation.</summary>
    public ExpectationCodes Expectation => _expectation;

    /// <summary>Gets or initializes the expectation literal.</summary>
    public string ExpectationLiteral
    {
        get => _expectationLiteral;
        init
        {
            _expectationLiteral = value;
            _expectation = _expectationLiteral.ToEnum<ExpectationCodes>() ?? ExpectationCodes.NotSpecified;
        }
    }

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    object ICloneable.Clone() => this with { };
}
