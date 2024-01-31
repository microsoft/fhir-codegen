// <copyright file="IWithExpectations.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.CodeGenCommon.Expectations;

/// <summary>Interface for classes annotated with expectations/obligations/etc..</summary>
public interface IConformanceAnnotated
{
    /// <summary>Gets or initializes the conformance expectation.</summary>
    FhirExpectation ConformanceExpectation { get; init; }
}
