// <copyright file="LanguageConfiguration.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Manager
{
    /// <summary>A language configuration.</summary>
    public class LanguageConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageConfiguration"/> class.
        /// </summary>
        /// <param name="languageName">               Name of the language export module.</param>
        /// <param name="supportsModelInheritance">   True to supports model inheritance.</param>
        /// <param name="primitiveStyles">            The primitive styles.</param>
        /// <param name="primitiveNamingConvention">  The primitive naming convention.</param>
        /// <param name="fhirPrimitiveTypeMap">       The primitive type map.</param>
        /// <param name="complexTypeStyles">          The complex type styles.</param>
        /// <param name="complexTypeNamingConvention">The complex type naming convention.</param>
        /// <param name="resourceStyles">             The resource styles.</param>
        /// <param name="resourceNamingConvention">   The resource naming convention.</param>
        /// <param name="interactionStyles">          The interaction styles.</param>
        /// <param name="interactionNamingConvention">The preferred naming convention.</param>
        /// <param name="handlebarExportTemplate">    The Handlebar export template.</param>
        public LanguageConfiguration(
            string languageName,
            bool supportsModelInheritance,
            HashSet<FeatureExportStyle> primitiveStyles,
            NamingConvention primitiveNamingConvention,
            Dictionary<string, string> fhirPrimitiveTypeMap,
            HashSet<FeatureExportStyle> complexTypeStyles,
            NamingConvention complexTypeNamingConvention,
            HashSet<FeatureExportStyle> resourceStyles,
            NamingConvention resourceNamingConvention,
            HashSet<FeatureExportStyle> interactionStyles,
            NamingConvention interactionNamingConvention,
            string handlebarExportTemplate)
        {
            LanguageName = languageName;
            SupportsModelInheritance = supportsModelInheritance;
            PrimitiveStyles = primitiveStyles;
            PrimitiveNamingConvention = primitiveNamingConvention;
            FhirPrimitiveTypeMap = fhirPrimitiveTypeMap;
            ComplexTypeStyles = complexTypeStyles;
            ComplexTypeNamingConvention = complexTypeNamingConvention;
            ResourceStyles = resourceStyles;
            ResourceNamingConvention = resourceNamingConvention;
            InteractionStyles = interactionStyles;
            InteractionNamingConvention = interactionNamingConvention;
            HandlebarExportTemplate = handlebarExportTemplate;
        }

        /// <summary>Values that represent export file structures.</summary>
        public enum FeatureExportStyle
        {
            /// <summary>This feature is not supported for export.</summary>
            NotSupported,

            /// <summary>Export everything into a single file.</summary>
            SingleFile,

            /// <summary>Export this type of feature grouped into a file (e.g., all Primitives, Resources).</summary>
            Grouped,

            /// <summary>Export each record individually (e.g., string, Patient).</summary>
            FilePerRecord,
        }

        /// <summary>Values that represent naming conventions for item types.</summary>
        public enum NamingConvention
        {
            /// <summary>This feature is not supported.</summary>
            NotSupported,

            /// <summary>Names are standard FHIR dot notation (e.g., path).</summary>
            FhirDotNotation,

            /// <summary>Names are dot notation, with each first letter capitalized.</summary>
            PascalDotNotation,

            /// <summary>Names are Pascal Case (first letter capitalized).</summary>
            PascalCase,

            /// <summary>Names are Camel Case (first letter lower case).</summary>
            CamelCase,

            /// <summary>Names are all upper case.</summary>
            UpperCase,

            /// <summary>Names are all lower case.</summary>
            LowerCase,
        }

        /// <summary>Gets the name of the language.</summary>
        /// <value>The name of the language.</value>
        public string LanguageName { get; }

        /// <summary>Gets a value indicating whether the language supports model inheritance.</summary>
        /// <value>True if the language supports model inheritance, false if not.</value>
        public bool SupportsModelInheritance { get; }

        /// <summary>Gets the primitive styles.</summary>
        /// <value>The primitive styles.</value>
        public HashSet<FeatureExportStyle> PrimitiveStyles { get; }

        /// <summary>Gets the primitive naming convention.</summary>
        /// <value>The primitive naming convention.</value>
        public NamingConvention PrimitiveNamingConvention { get; }

        /// <summary>Gets the FHIR primitive type map.</summary>
        /// <value>The FHIR primitive type map.</value>
        public Dictionary<string, string> FhirPrimitiveTypeMap { get; }

        /// <summary>Gets the complex type styles.</summary>
        /// <value>The complex type styles.</value>
        public HashSet<FeatureExportStyle> ComplexTypeStyles { get; }

        /// <summary>Gets the complex type naming convention.</summary>
        /// <value>The complex type naming convention.</value>
        public NamingConvention ComplexTypeNamingConvention { get; }

        /// <summary>Gets the resource styles.</summary>
        /// <value>The resource styles.</value>
        public HashSet<FeatureExportStyle> ResourceStyles { get; }

        /// <summary>Gets the resource naming convention.</summary>
        /// <value>The resource naming convention.</value>
        public NamingConvention ResourceNamingConvention { get; }

        /// <summary>Gets the interaction styles.</summary>
        /// <value>The interaction styles.</value>
        public HashSet<FeatureExportStyle> InteractionStyles { get; }

        /// <summary>Gets the interaction naming convention.</summary>
        /// <value>The interaction naming convention.</value>
        public NamingConvention InteractionNamingConvention { get; }

        /// <summary>Gets the Handlebar export template.</summary>
        /// <value>The Handlebar export template.</value>
        public string HandlebarExportTemplate { get; }
    }
}
