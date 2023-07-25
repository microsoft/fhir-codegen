// <copyright file="IWithExpectations.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.CodeGenCommon.Resource;

namespace Microsoft.Health.Fhir.CodeGenCommon.Structural;

/// <summary>Interface for classes annotated with expectations.</summary>
public interface IWithExpectations
{
    /// <summary>Gets the obligations by actor.</summary>
    public Dictionary<string, IEnumerable<FhirObligation>> ObligationsByActor { get; init; }

    ///// <summary>Initializes obligations.</summary>
    //public IEnumerable<FhirObligation> Obligations { init; }
    ////get => _obligations.SelectMany(kvp => kvp.Value);
}
