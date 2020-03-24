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
        /// <value>True if the language supports model inheritance, false if not.</value>
        bool SupportsModelInheritance { get; }

        /// <summary>Gets a value indicating whether the supports hiding parent field.</summary>
        /// <value>True if the language supports hiding parent field, false if not.</value>
        bool SupportsHidingParentField { get; }

        /// <summary>Gets a value indicating whether the language supports nested type definitions.</summary>
        /// <value>True if the language supports nested type definitions, false if not.</value>
        bool SupportsNestedTypeDefinitions { get; }

        /// <summary>Gets a value indicating whether the supports slicing.</summary>
        /// <value>True if supports slicing, false if not.</value>
        bool SupportsSlicing { get; }

        /// <summary>Gets the FHIR primitive type map.</summary>
        /// <value>The FHIR primitive type map.</value>
        Dictionary<string, string> FhirPrimitiveTypeMap { get; }

        /// <summary>Gets the primitive configuration.</summary>
        /// <value>The primitive configuration.</value>
        HashSet<NamingConvention> SupportedPrimitiveNameStyles { get; }

        /// <summary>Gets the complex type configuration.</summary>
        /// <value>The complex type configuration.</value>
        HashSet<NamingConvention> SupportedComplexTypeNameStyles { get; }

        /// <summary>Gets the resource configuration.</summary>
        /// <value>The resource configuration.</value>
        HashSet<NamingConvention> SupportedResourceNameStyles { get; }

        /// <summary>Gets the interaction configuration.</summary>
        /// <value>The interaction configuration.</value>
        HashSet<NamingConvention> SupportedInteractionNameStyles { get; }

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
