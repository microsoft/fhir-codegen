// <copyright file="ExporterOptions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.Health.Fhir.SpecManager.Models.FhirTypeBase;

namespace Microsoft.Health.Fhir.SpecManager.Manager
{
    /// <summary>An exporter options.</summary>
    public class ExporterOptions
    {
        private readonly HashSet<string> _extensionUrls;
        private readonly HashSet<string> _extensionElementPaths;

        /// <summary>Initializes a new instance of the <see cref="ExporterOptions"/> class.</summary>
        /// <param name="languageName">           Name of the language.</param>
        /// <param name="exportList">             List of exports.</param>
        /// <param name="useModelInheritance">    True to use model inheritance.</param>
        /// <param name="hideRemovedParentFields">True to hide, false to show the parent fields.</param>
        /// <param name="nestTypeDefinitions">    True to nest definitions.</param>
        /// <param name="primitiveNameStyle">     The primitive name style.</param>
        /// <param name="complexNameStyle">       The complex name style (complex types, resources, etc.).</param>
        /// <param name="elementNameStyle">       The element name style.</param>
        /// <param name="interactionNameStyle">   The interaction name style.</param>
        /// <param name="enumStyle">              The enum style.</param>
        /// <param name="extensionSupport">       The extension support.</param>
        /// <param name="extensionUrls">          Manually supported extension URLs that should be added.</param>
        /// <param name="extensionElementPaths">  Manually supported element paths that should have
        ///  extensions.</param>
        public ExporterOptions(
            string languageName,
            IEnumerable<string> exportList,
            bool useModelInheritance,
            bool hideRemovedParentFields,
            bool nestTypeDefinitions,
            NamingConvention primitiveNameStyle,
            NamingConvention complexNameStyle,
            NamingConvention elementNameStyle,
            NamingConvention interactionNameStyle,
            NamingConvention enumStyle,
            ExtensionSupportLevel extensionSupport,
            IEnumerable<string> extensionUrls,
            IEnumerable<string> extensionElementPaths)
        {
            LanguageName = languageName;
            ExportList = exportList;
            UseModelInheritance = useModelInheritance;
            HideRemovedParentFields = hideRemovedParentFields;
            NestTypeDefinitions = nestTypeDefinitions;
            PrimitiveNameStyle = primitiveNameStyle;
            ElementNameStyle = elementNameStyle;
            ComplexTypeNameStyle = complexNameStyle;
            InteractionNameStyle = interactionNameStyle;
            EnumStyle = enumStyle;
            ExtensionSupport = extensionSupport;

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

        /// <summary>Gets a value indicating whether this object use model inheritance.</summary>
        /// <value>True if use model inheritance, false if not.</value>
        public bool UseModelInheritance { get; }

        /// <summary>Gets a value indicating whether the parent fields is hidden.</summary>
        /// <value>True if hide parent fields, false if not.</value>
        public bool HideRemovedParentFields { get; }

        /// <summary>Gets a value indicating whether the nest definitions.</summary>
        /// <value>True if nest definitions, false if not.</value>
        public bool NestTypeDefinitions { get; }

        /// <summary>Gets the primitive name style.</summary>
        /// <value>The primitive name style.</value>
        public NamingConvention PrimitiveNameStyle { get; }

        /// <summary>Gets the element name style.</summary>
        /// <value>The element name style.</value>
        public NamingConvention ElementNameStyle { get; }

        /// <summary>Gets the complex type name style.</summary>
        /// <value>The complex type name style.</value>
        public NamingConvention ComplexTypeNameStyle { get; }

        /// <summary>Gets the interaction name style.</summary>
        /// <value>The interaction name style.</value>
        public NamingConvention InteractionNameStyle { get; }

        /// <summary>Gets the enum style.</summary>
        /// <value>The enum style.</value>
        public NamingConvention EnumStyle { get; }

        /// <summary>Gets the extension support.</summary>
        /// <value>The extension support.</value>
        public ExtensionSupportLevel ExtensionSupport { get; }

        /// <summary>Gets the extension urls.</summary>
        /// <value>The extension urls.</value>
        public HashSet<string> ExtensionUrls => _extensionUrls;

        /// <summary>Gets the extension element paths.</summary>
        /// <value>The extension element paths.</value>
        public HashSet<string> ExtensionElementPaths => _extensionElementPaths;
    }
}
