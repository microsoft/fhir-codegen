// <copyright file="FhirCapabilityStatement.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.CodeGenCommon.Extensions;

namespace Microsoft.Health.Fhir.CodeGenCommon.Resource;

/// <summary>A FHIR capability statement.</summary>
public class FhirCapabilityStatement : FhirCanonicalBase
{
    /// <summary>Values that represent conformance expectation codes.</summary>
    public enum ExpectationCodes
    {
        /// <summary>An enum constant representing the may option.</summary>
        [FhirLiteral("MAY")]
        MAY,

        /// <summary>An enum constant representing the should option.</summary>
        [FhirLiteral("SHOULD")]
        SHOULD,

        /// <summary>An enum constant representing the shall option.</summary>
        [FhirLiteral("SHALL")]
        SHALL,

        /// <summary>No conformance expectation has been specified.</summary>
        NotSpecified,
    }

    /// <summary>A value with a conformance expectation.</summary>
    /// <param name="Value">             The value.</param>
    /// <param name="ExpectationLiteral">The expectation literal.</param>
    /// <param name="ExpectationCode">   The expectation code.</param>
    public record WithExpectation<T>(
        T Value,
        string FhirExpectation,
        ExpectationCodes ExpectationCode);


}
