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

    /// <summary>Attempts to get export language an ILanguage? from the given string.</summary>
    /// <param name="name"> The name.</param>
    /// <param name="iLang">[out] Language interface or null.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    bool TryGetExportLanguage(string name, out ILanguage? iLang);

    /// <summary>Request export.</summary>
    /// <param name="info">          The loaded FHIR package information.</param>
    /// <param name="exportLanguage">The export language.</param>
    /// <param name="options">       Options for controlling the operation.</param>
    /// <param name="outputPath">    Full pathname of the output file.</param>
    /// <returns>An asynchronous result.</returns>
    Task RequestExport(FhirVersionInfo info, ILanguage exportLanguage, ExporterOptions options, string outputPath);
}
