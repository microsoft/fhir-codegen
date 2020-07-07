// <copyright file="ILanguage.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Health.Fhir.SpecManager.Manager;
using static Microsoft.Health.Fhir.SpecManager.Models.FhirTypeBase;

namespace Microsoft.Health.Fhir.SpecManager.Language
{
    /// <summary>An export language.</summary>
    public interface ILanguage
    {
        /// <summary>Gets the name of the language.</summary>
        /// <value>The name of the language.</value>
        string LanguageName { get; }

        /// <summary>Gets a value indicating whether the language supports model inheritance.</summary>
        bool SupportsModelInheritance { get; }

        /// <summary>Gets a value indicating whether the supports hiding parent field.</summary>
        bool SupportsHidingParentField { get; }

        /// <summary>Gets a value indicating whether the language supports nested type definitions.</summary>
        /// <value>True if the language supports nested type definitions, false if not.</value>
        bool SupportsNestedTypeDefinitions { get; }

        /// <summary>Gets a value indicating whether the supports slicing.</summary>
        bool SupportsSlicing { get; }

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
        /// <param name="options">        Options for controlling the operation.</param>
        /// <param name="exportDirectory">Directory to write files.</param>
        void Export(
            FhirVersionInfo info,
            ExporterOptions options,
            string exportDirectory);
    }
}
