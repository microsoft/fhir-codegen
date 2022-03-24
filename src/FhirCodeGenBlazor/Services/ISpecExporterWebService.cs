// <copyright file="ISpecExporterWebService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.SpecManager.Language;
using Microsoft.Health.Fhir.SpecManager.Manager;

namespace FhirCodeGenBlazor.Services;

/// <summary>Interface for specifier exporter web service.</summary>
public interface ISpecExporterWebService
{
    /// <summary>Initializes this object.</summary>
    void Init();

    /// <summary>Gets languages by name.</summary>
    /// <returns>The languages by name.</returns>
    Dictionary<string, ILanguage> GetExportLanguages();

    /// <summary>Gets export language.</summary>
    /// <param name="name">The name.</param>
    /// <returns>The export language.</returns>
    ILanguage? GetExportLanguage(string name);

    /// <summary>Request export.</summary>
    /// <param name="info">          The loaded FHIR package information.</param>
    /// <param name="exportLanguage">The export language.</param>
    /// <param name="options">       Options for controlling the operation.</param>
    /// <param name="outputPath">    Full pathname of the output file.</param>
    /// <returns>An asynchronous result.</returns>
    Task RequestExport(FhirVersionInfo info, ILanguage exportLanguage, ExporterOptions options, string outputPath);
}
