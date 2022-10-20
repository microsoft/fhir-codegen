// <copyright file="ILanguage.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.SpecManager.Manager;
using Microsoft.Health.Fhir.SpecManager.Models;

namespace Microsoft.Health.Fhir.SpecManager.Language;

/// <summary>An export language.</summary>
public interface ILanguage
{
    /// <summary>Gets the name of the language.</summary>
    /// <value>The name of the language.</value>
    string LanguageName { get; }

    /// <summary>
    /// Gets the single file extension for this language - null or empty indicates a multi-file
    /// export (exporter should copy the contents of the directory).
    /// </summary>
    string SingleFileExportExtension { get; }

    /// <summary>Gets the FHIR primitive type map.</summary>
    Dictionary<string, string> FhirPrimitiveTypeMap { get; }

    /// <summary>Gets the reserved words.</summary>
    HashSet<string> ReservedWords { get; }

    /// <summary>
    /// Gets a list of FHIR class types that the language WILL export, regardless of user choices.
    /// Used to provide information to users.
    /// </summary>
    List<ExporterOptions.FhirExportClassType> RequiredExportClassTypes { get; }

    /// <summary>
    /// Gets a list of FHIR class types that the language CAN export, depending on user choices.
    /// </summary>
    List<ExporterOptions.FhirExportClassType> OptionalExportClassTypes { get; }

    /// <summary>Gets language-specific options and their descriptions.</summary>
    Dictionary<string, string> LanguageOptions { get; }

    /// <summary>Gets the export.</summary>
    /// <param name="info">           The information.</param>
    /// <param name="serverInfo">     Information describing the server.</param>
    /// <param name="options">        Options for controlling the operation.</param>
    /// <param name="exportDirectory">Directory to write files.</param>
    void Export(
        FhirVersionInfo info,
        FhirCapabiltyStatement serverInfo,
        ExporterOptions options,
        string exportDirectory);
}
