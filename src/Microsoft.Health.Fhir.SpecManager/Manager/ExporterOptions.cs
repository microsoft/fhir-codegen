// <copyright file="ExporterOptions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Manager
{
    /// <summary>An exporter options.</summary>
    public class ExporterOptions
    {
        /// <summary>Initializes a new instance of the <see cref="ExporterOptions"/> class.</summary>
        /// <param name="languageName">    Name of the language.</param>
        /// <param name="primitiveStyle">  The primitive style.</param>
        /// <param name="complexTypeStyle">The complex type style.</param>
        /// <param name="resourceStyle">   The resource style.</param>
        /// <param name="interactionStyle">The interaction style.</param>
        /// <param name="extensionSupport">The extension support.</param>
        /// <param name="extensionLookup"> Extension look up set (Extension URL, Element Path, or null).</param>
        public ExporterOptions(
            string languageName,
            LanguageConfiguration.FeatureExportStyle primitiveStyle,
            LanguageConfiguration.FeatureExportStyle complexTypeStyle,
            LanguageConfiguration.FeatureExportStyle resourceStyle,
            LanguageConfiguration.FeatureExportStyle interactionStyle,
            ExtensionSupport extensionSupport,
            HashSet<string> extensionLookup)
        {
            LanguageName = languageName;
            PrimitiveStyle = primitiveStyle;
            ComplexTypeStyle = complexTypeStyle;
            ResourceStyle = resourceStyle;
            InteractionStyle = interactionStyle;
        }

        /// <summary>Values that represent extension support requests.</summary>
        public enum ExtensionSupport
        {
            /// <summary>No extensions should be included.</summary>
            NoExtensions,

            /// <summary>Official (core) extensions should be included.</summary>
            OfficialExtensions,

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

        /// <summary>Gets the primitive style.</summary>
        /// <value>The primitive style.</value>
        public LanguageConfiguration.FeatureExportStyle PrimitiveStyle { get; }

        /// <summary>Gets the complex type style.</summary>
        /// <value>The complex type style.</value>
        public LanguageConfiguration.FeatureExportStyle ComplexTypeStyle { get; }

        /// <summary>Gets the resource style.</summary>
        /// <value>The resource style.</value>
        public LanguageConfiguration.FeatureExportStyle ResourceStyle { get; }

        /// <summary>Gets the interaction style.</summary>
        /// <value>The interaction style.</value>
        public LanguageConfiguration.FeatureExportStyle InteractionStyle { get; }
    }
}
