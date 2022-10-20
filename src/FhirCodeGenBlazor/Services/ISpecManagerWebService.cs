// <copyright file="ISpecManagerWebService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.CodeGenCommon.Models;
using Microsoft.Health.Fhir.SpecManager.Manager;

namespace FhirCodeGenBlazor.Services;

/// <summary>A service for accessing specifier manager webs information.</summary>
public interface ISpecManagerWebService : IReadOnlyDictionary<string, FhirVersionInfo>
{
    /// <summary>Initializes this object.</summary>
    void Init();
}
