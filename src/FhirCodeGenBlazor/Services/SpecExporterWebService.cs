// <copyright file="SpecExporterWebService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.SpecManager.Language;
using Microsoft.Health.Fhir.SpecManager.Manager;

namespace FhirCodeGenBlazor.Services;

/// <summary>A service for accessing specifier exporter webs information.</summary>
public class SpecExporterWebService : IDisposable, IHostedService, ISpecExporterWebService
{
    /// <summary>True if has disposed, false if not.</summary>
    private bool _hasDisposed;


    /// <summary>
    /// Initializes a new instance of the <see cref="SpecExporterWebService"/> class.
    /// </summary>
    public SpecExporterWebService()
    {
        _hasDisposed = false;
    }

    /// <summary>Initializes this object.</summary>
    public void Init()
    {
    }

    /// <summary>Request export.</summary>
    /// <param name="info">          The information.</param>
    /// <param name="exportLanguage">The export language.</param>
    /// <param name="options">       Options for controlling the operation.</param>
    /// <param name="outputPath">    Full pathname of the output file.</param>
    /// <returns>An asynchronous result.</returns>
    public Task RequestExport(FhirVersionInfo info, ILanguage exportLanguage, ExporterOptions options, string outputPath)
    {
        Task exportTask = new Task(() => Exporter.Export(
            info,
            null,
            exportLanguage,
            options,
            outputPath,
            false));

        return exportTask;
    }


    /// <summary>Gets languages by name.</summary>
    /// <returns>The languages by name.</returns>
    public Dictionary<string, ILanguage> GetExportLanguages()
    {
        Dictionary<string, ILanguage> languages = new();

        foreach (ILanguage lang in LanguageHelper.GetLanguages("*"))
        {
            languages.Add(lang.LanguageName, lang);
        }

        return languages;
    }

    /// <summary>Attempts to get export language an ILanguage? from the given string.</summary>
    /// <param name="name"> The name.</param>
    /// <param name="iLang">[out] Language interface or null.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool TryGetExportLanguage(string name, out ILanguage? iLang)
    {
        List<ILanguage> languages = LanguageHelper.GetLanguages(name);

        if (languages.Any())
        {
            iLang = languages[0];
            return true;
        }

        iLang = null;
        return false;
    }


    /// <summary>Triggered when the application host is ready to start the service.</summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    /// <returns>An asynchronous result.</returns>
    Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <summary>Triggered when the application host is performing a graceful shutdown.</summary>
    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be
    ///  graceful.</param>
    /// <returns>An asynchronous result.</returns>
    Task IHostedService.StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Releases the unmanaged resources used by the
    /// FhirModelComparer.Server.Services.FhirManagerService and optionally releases the managed
    /// resources.
    /// </summary>
    /// <param name="disposing">True to release both managed and unmanaged resources; false to
    ///  release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_hasDisposed)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _hasDisposed = true;
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
    /// resources.
    /// </summary>
    void IDisposable.Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
