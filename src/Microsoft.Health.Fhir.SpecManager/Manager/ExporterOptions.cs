// <copyright file="ExporterOptions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Health.Fhir.SpecManager.Models;
using static Microsoft.Health.Fhir.SpecManager.Models.FhirTypeBase;

namespace Microsoft.Health.Fhir.SpecManager.Manager
{
    /// <summary>An exporter options.</summary>
    public class ExporterOptions
    {
        private readonly HashSet<string> _extensionUrls;
        private readonly HashSet<string> _extensionElementPaths;
        private readonly Dictionary<string, string> _languageOptions;

        /// <summary>Initializes a new instance of the <see cref="ExporterOptions"/> class.</summary>
        /// <param name="languageName">           Name of the language.</param>
        /// <param name="exportList">             List of exports.</param>
        /// <param name="optionalClassesToExport">Language optional class types to export (e.g., Enums).</param>
        /// <param name="extensionSupport">       The extension support.</param>
        /// <param name="extensionUrls">          Manually supported extension URLs that should be added.</param>
        /// <param name="extensionElementPaths">  Manually supported element paths that should have
        ///  extensions.</param>
        /// <param name="languageOptions">        Options for controlling the language.</param>
        /// <param name="fhirServerUrl">          FHIR Server URL to pull a CapabilityStatement (or
        ///  Conformance) from.  Requires application/fhir+json.</param>
        /// <param name="serverInfo">             Information describing the server (if specified).</param>
        /// <param name="includeExperimental">    A value indicating whether structures marked experimental
        ///  should be included.</param>
        public ExporterOptions(
            string languageName,
            IEnumerable<string> exportList,
            List<FhirExportClassType> optionalClassesToExport,
            ExtensionSupportLevel extensionSupport,
            IEnumerable<string> extensionUrls,
            IEnumerable<string> extensionElementPaths,
            Dictionary<string, string> languageOptions,
            string fhirServerUrl,
            bool includeExperimental)
        {
            LanguageName = languageName;
            ExportList = exportList;
            ExtensionSupport = extensionSupport;

            OptionalClassTypesToExport = optionalClassesToExport;

            _extensionUrls = new HashSet<string>();
            if (extensionUrls != null)
            {
                foreach (string url in extensionUrls)
                {
                    if (!_extensionUrls.Contains(url))
                    {
                        _extensionUrls.Add(url);
                    }
                }
            }

            _extensionElementPaths = new HashSet<string>();
            if (extensionElementPaths != null)
            {
                foreach (string path in extensionElementPaths)
                {
                    if (!_extensionElementPaths.Contains(path))
                    {
                        _extensionElementPaths.Add(path);
                    }
                }
            }

            _languageOptions = languageOptions;
            ServerUrl = fhirServerUrl;
            IncludeExperimental = includeExperimental;
        }

        /// <summary>Values that represent FHIR export class types.</summary>
        public enum FhirExportClassType
        {
            /// <summary>
            /// Primitive Types (e.g., string, uri).
            /// See http://hl7.org/fhir/datatypes.html#primitive
            /// </summary>
            PrimitiveType,

            /// <summary>
            /// Complex Types (e.g., Address, Coding).
            /// See http://hl7.org/fhir/datatypes.html#complex
            /// </summary>
            ComplexType,

            /// <summary>
            /// Resources (e.g., Patient, Bundle).
            /// See http://hl7.org/fhir/resourcelist.html
            /// </summary>
            Resource,

            /// <summary>
            /// Interactions (e.g., read, create).
            /// See http://hl7.org/fhir/http.html#3.1.0
            /// </summary>
            Interaction,

            /// <summary>
            /// Enumerations (e.g., instances of Codes and Value Sets).
            /// </summary>
            Enum,
        }

        /// <summary>Values that represent extension support requests.</summary>
        public enum ExtensionSupportLevel
        {
            /// <summary>No extensions should be included.</summary>
            NoExtensions,

            /// <summary>Official (core) extensions should be included.</summary>
            OfficialExtensions,

            /// <summary>An enum constant representing the official non primitive option.</summary>
            OfficialNonPrimitive,

            /// <summary>Every field should have a mockup for extensions.</summary>
            EveryField,

            /// <summary>Non-primitive type fields should have extensions.</summary>
            NonPrimitives,

            /// <summary>Only extensions with a URL in the provided list should be included.</summary>
            IncludeByExtensionUrlLookup,

            /// <summary>Only elements with a path in the provided list should have extensions.</summary>
            IncludeByElementPathLookup,
        }

        /// <summary>Gets the name of the language.</summary>
        /// <value>The name of the language.</value>
        public string LanguageName { get; }

        /// <summary>Gets a list of exports.</summary>
        /// <value>A list of exports.</value>
        public IEnumerable<string> ExportList { get; }

        /// <summary>Gets the optional class types to export.</summary>
        public List<FhirExportClassType> OptionalClassTypesToExport { get; }

        /// <summary>Gets the extension support.</summary>
        /// <value>The extension support.</value>
        public ExtensionSupportLevel ExtensionSupport { get; }

        /// <summary>Gets the extension urls.</summary>
        /// <value>The extension urls.</value>
        public HashSet<string> ExtensionUrls => _extensionUrls;

        /// <summary>Gets the extension element paths.</summary>
        /// <value>The extension element paths.</value>
        public HashSet<string> ExtensionElementPaths => _extensionElementPaths;

        /// <summary>Gets options for controlling the language.</summary>
        public Dictionary<string, string> LanguageOptions => _languageOptions;

        /// <summary>Gets URL of the FHIR server (if specified).</summary>
        public string ServerUrl { get; }

        /// <summary>Gets a value indicating whether structures marked experimental should be included.</summary>
        public bool IncludeExperimental { get; }
    }
}
