// <copyright file="ICrossVersionProcessor.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion;

public interface ICrossVersionProcessor<T> where T : Hl7.Fhir.Model.Base
{
#if NET8_0_OR_GREATER
    /// <summary>Gets the instance.</summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    static ICrossVersionProcessor<T> Instance { get; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#endif

    /// <summary>Process the source node values into the current object.</summary>
    /// <param name="node"> Source for the.</param>
    /// <param name="current">The current.</param>
    void Process(ISourceNode node, T current);
}
