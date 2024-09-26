// <copyright file="ICrossVersionExtractor.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion;

public interface ICrossVersionExtractor<T> where T : Hl7.Fhir.Model.Base, new()
{
#if NET8_0_OR_GREATER
    /// <summary>Gets the instance.</summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    static ICrossVersionExtractor<T> Instance { get; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#endif

    /// <summary>Extracts the given source node into a resource.</summary>
    /// <param name="node">Source content.</param>
    /// <returns>A T.</returns>
    T Extract(ISourceNode node);
}
