// <copyright file="FhirCapSearchParamCombination.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using Microsoft.Health.Fhir.CodeGenCommon.Extensions;

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>
/// A FHIR capability search parameter combination, used to express conformance expectations.
/// See: http://hl7.org/fhir/StructureDefinition/capabilitystatement-search-parameter-combination
/// </summary>
public class FhirCapSearchParamCombination : ICloneable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FhirCapSearchParamCombination"/> class.
    /// </summary>
    /// <param name="requiredParams">The required parameters for this combination definition.</param>
    /// <param name="optionalParams">The optional parameters for this combination definition.</param>
    /// <param name="expectation">   The expectation literal.</param>
    public FhirCapSearchParamCombination(
        IEnumerable<string> requiredParams,
        IEnumerable<string> optionalParams,
        string expectation)
    {
        RequiredParams = requiredParams ?? Array.Empty<string>();
        OptionalParams = optionalParams ?? Array.Empty<string>();
        ExpectationLiteral = expectation;
        if (expectation.TryFhirEnum<FhirCapabiltyStatement.ExpectationCodes>(out object expect))
        {
            Expectation = (FhirCapabiltyStatement.ExpectationCodes)expect;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirCapSearchParamCombination"/> class.
    /// </summary>
    /// <param name="source">Source to deep copy.</param>
    public FhirCapSearchParamCombination(FhirCapSearchParamCombination source)
    {
        RequiredParams = source.RequiredParams.Select(s => s);
        OptionalParams = source.OptionalParams.Select(s => s);
        ExpectationLiteral = source.ExpectationLiteral;
        Expectation = source.Expectation;
    }

    /// <summary>Gets the required parameters for this combination definition.</summary>
    public IEnumerable<string> RequiredParams { get; }

    /// <summary>Gets the optional parameters for this combination definition.</summary>
    public IEnumerable<string> OptionalParams { get; }

    /// <summary>Gets the expectation literal.</summary>
    public string ExpectationLiteral { get; }

    /// <summary>Gets the expectation.</summary>
    public FhirCapabiltyStatement.ExpectationCodes? Expectation { get; }

    /// <summary>Creates a new object that is a copy of the current instance.</summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone() => new FhirCapSearchParamCombination(this);
}
