// <copyright file="LanguageInfo.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Health.Fhir.SpecManager.Manager;
using Microsoft.Health.Fhir.SpecManager.Models;

namespace Microsoft.Health.Fhir.SpecManager.Language
{
    /// <summary>Information about the language.</summary>
    public sealed class LanguageInfo : ILanguage
    {
        /// <summary>FHIR information we are exporting.</summary>
        private FhirVersionInfo _info;

        /// <summary>Options for controlling the export.</summary>
        private ExporterOptions _options;

        /// <summary>Name of the language.</summary>
        private const string _languageName = "Info";

        /// <summary>Dictionary mapping FHIR primitive types to language equivalents.</summary>
        private static readonly Dictionary<string, string> _primitiveTypeMap = new Dictionary<string, string>()
        {
            { "base64Binary", "base64Binary" },
            { "boolean", "boolean" },
            { "canonical", "canonical" },
            { "code", "code" },
            { "date", "date" },
            { "dateTime", "dateTime" },
            { "decimal", "decimal" },
            { "id", "id" },
            { "instant", "instant" },
            { "integer", "integer" },
            { "integer64", "integer64" },
            { "markdown", "markdown" },
            { "oid", "oid" },
            { "positiveInt", "positiveInt" },
            { "string", "string" },
            { "time", "time" },
            { "unsignedInt", "unsignedInt" },
            { "uri", "uri" },
            { "url", "url" },
            { "uuid", "uuid" },
            { "xhtml", "xhtml" },
        };

        /// <summary>This language supports all naming styles on all object types.</summary>
        private static readonly HashSet<FhirTypeBase.NamingConvention> _allNamingStyles = new HashSet<FhirTypeBase.NamingConvention>()
        {
            FhirTypeBase.NamingConvention.None,
            FhirTypeBase.NamingConvention.FhirDotNotation,
            FhirTypeBase.NamingConvention.PascalDotNotation,
            FhirTypeBase.NamingConvention.PascalCase,
            FhirTypeBase.NamingConvention.CamelCase,
            FhirTypeBase.NamingConvention.UpperCase,
            FhirTypeBase.NamingConvention.LowerCase,
        };

        /// <summary>Gets the name of the language.</summary>
        /// <value>The name of the language.</value>
        string ILanguage.LanguageName => _languageName;

        /// <summary>Gets a value indicating whether the language supports model inheritance.</summary>
        /// <value>True if the language supports model inheritance, false if not.</value>
        bool ILanguage.SupportsModelInheritance => true;

        /// <summary>Gets a value indicating whether the supports hiding parent field.</summary>
        /// <value>True if the language supports hiding parent field, false if not.</value>
        bool ILanguage.SupportsHidingParentField => true;

        /// <summary>
        /// Gets a value indicating whether the language supports nested type definitions.
        /// </summary>
        /// <value>True if the language supports nested type definitions, false if not.</value>
        bool ILanguage.SupportsNestedTypeDefinitions => true;

        /// <summary>Gets a value indicating whether the supports slicing.</summary>
        /// <value>True if supports slicing, false if not.</value>
        bool ILanguage.SupportsSlicing => true;

        /// <summary>Gets the FHIR primitive type map.</summary>
        /// <value>The FHIR primitive type map.</value>
        Dictionary<string, string> ILanguage.FhirPrimitiveTypeMap => _primitiveTypeMap;

        /// <summary>Gets the primitive configuration.</summary>
        /// <value>The primitive configuration.</value>
        HashSet<FhirTypeBase.NamingConvention> ILanguage.SupportedPrimitiveNameStyles => _allNamingStyles;

        /// <summary>Gets the complex type configuration.</summary>
        /// <value>The complex type configuration.</value>
        HashSet<FhirTypeBase.NamingConvention> ILanguage.SupportedComplexTypeNameStyles => _allNamingStyles;

        /// <summary>Gets the resource configuration.</summary>
        /// <value>The resource configuration.</value>
        HashSet<FhirTypeBase.NamingConvention> ILanguage.SupportedResourceNameStyles => _allNamingStyles;

        /// <summary>Gets the interaction configuration.</summary>
        /// <value>The interaction configuration.</value>
        HashSet<FhirTypeBase.NamingConvention> ILanguage.SupportedInteractionNameStyles => _allNamingStyles;

        /// <summary>Gets the export.</summary>
        /// <param name="info">   The information.</param>
        /// <param name="options">Options for controlling the operation.</param>
        /// <param name="stream"> [in,out] The stream.</param>
        void ILanguage.Export(
            FhirVersionInfo info,
            ExporterOptions options,
            ref MemoryStream stream)
        {
            // set internal vars so we don't pass them to every function
            // this is ugly, but the interface patterns get bad quickly because we need the type map to copy the FHIR info
            _info = info;
            _options = options;

            // fill in our file
#pragma warning disable CA2000 // Dispose objects before losing scope
            StreamWriter writer = new StreamWriter(stream);
#pragma warning restore CA2000 // Dispose objects before losing scope

            WriteHeader(writer);

            WritePrimitiveTypes(writer, _info.PrimitiveTypes.Values, 0);
            WriteComplexes(writer, _info.ComplexTypes.Values, 0, "Complex Types");
            WriteComplexes(writer, _info.Resources.Values, 0, "Resources");

            WriteOperations(writer, _info.SystemOperations.Values, 0, true);
            WriteSearchParameters(writer, _info.AllResourceParameters.Values, 0);
            WriteSearchParameters(writer, _info.SearchResultParameters.Values, 0);
            WriteSearchParameters(writer, _info.AllInteractionParameters.Values, 0);

            WriteFooter(writer);
        }

        /// <summary>Writes the complexes.</summary>
        /// <param name="writer">     The writer.</param>
        /// <param name="complexes">  The complexes.</param>
        /// <param name="indentation">The indentation.</param>
        private void WriteComplexes(
            TextWriter writer,
            IEnumerable<FhirComplex> complexes,
            int indentation,
            string headerHint = null)
        {
            if (!string.IsNullOrEmpty(headerHint))
            {
                WriteIndented(writer, indentation, $"{headerHint}: {complexes.Count()}");
            }

            foreach (FhirComplex complex in complexes)
            {
                WriteComplex(writer, complex, indentation);
            }
        }

        /// <summary>Writes a primitive types.</summary>
        /// <param name="writer">     The writer.</param>
        /// <param name="primitives"> The primitives.</param>
        /// <param name="indentation">The indentation.</param>
        private void WritePrimitiveTypes(
            TextWriter writer,
            IEnumerable<FhirPrimitive> primitives,
            int indentation)
        {
            WriteIndented(writer, indentation, $"Primitive Types: {primitives.Count()}");

            foreach (FhirPrimitive primitive in primitives)
            {
                WritePrimitiveType(writer, primitive, indentation);
            }
        }

        /// <summary>Writes a primitive type.</summary>
        /// <param name="writer">   The writer.</param>
        /// <param name="primitive">The primitive.</param>
        private void WritePrimitiveType(
            TextWriter writer,
            FhirPrimitive primitive,
            int indentation)
        {
            WriteIndented(writer, indentation, $"- {primitive.Name}: {primitive.NameForExport(_options.PrimitiveNameStyle)}");

            if (_info.ExtensionsByPath.ContainsKey(primitive.Path))
            {
                WriteExtensions(writer, _info.ExtensionsByPath[primitive.Name].Values, indentation + 1);
            }
        }

        /// <summary>Writes the extensions.</summary>
        /// <param name="writer">     The writer.</param>
        /// <param name="extensions"> The extensions.</param>
        /// <param name="indentation">The indentation.</param>
        private void WriteExtensions(
            TextWriter writer,
            IEnumerable<FhirComplex> extensions,
            int indentation)
        {
            WriteIndented(writer, indentation, $"Extensions: {extensions.Count()}");

            foreach (FhirComplex extension in extensions)
            {
                WriteExtension(writer, extension, indentation);
            }
        }

        /// <summary>Writes an extension.</summary>
        /// <param name="writer">     The writer.</param>
        /// <param name="extension">  The extension.</param>
        /// <param name="indentation">The indentation.</param>
        private void WriteExtension(
            TextWriter writer,
            FhirComplex extension,
            int indentation)
        {
            WriteIndented(writer, indentation, $"+{extension.URL}");

            if (extension.Elements.Count > 0)
            {
                WriteComplex(writer, extension, indentation + 1);
            }
        }

        /// <summary>Writes a complex.</summary>
        /// <param name="writer">     The writer.</param>
        /// <param name="complex">    The complex.</param>
        /// <param name="indentation">The indentation.</param>
        private void WriteComplex(
            TextWriter writer,
            FhirComplex complex,
            int indentation)
        {
            // write this type's line, if it's a root element
            // (sub-properties are written with cardinality in the prior loop)
            if (indentation == 0)
            {
                WriteIndented(writer, indentation, $"- {complex.Name}: {complex.BaseTypeName}");
            }

            // write elements
            WriteElements(writer, complex, indentation + 1);

            // check for extensions
            if (_info.ExtensionsByPath.ContainsKey(complex.Path))
            {
                WriteExtensions(writer, _info.ExtensionsByPath[complex.Path].Values, indentation + 1);
            }

            // check for search parameters on this object
            if (complex.SearchParameters != null)
            {
                WriteSearchParameters(writer, complex.SearchParameters.Values, indentation + 1);
            }

            // check for type operations
            if (complex.TypeOperations != null)
            {
                WriteOperations(writer, complex.TypeOperations.Values, indentation + 1, true);
            }

            // check for instance operations
            if (complex.InstanceOperations != null)
            {
                WriteOperations(writer, complex.TypeOperations.Values, indentation + 1, false);
            }
        }

        /// <summary>Writes the operations.</summary>
        /// <param name="writer">     The writer.</param>
        /// <param name="operations"> The operations.</param>
        /// <param name="indentation">The indentation.</param>
        /// <param name="isTypeLevel">True if is type level, false if not.</param>
        private void WriteOperations(
            TextWriter writer,
            IEnumerable<FhirOperation> operations,
            int indentation,
            bool isTypeLevel)
        {
            foreach (FhirOperation operation in operations)
            {
                if (isTypeLevel)
                {
                    WriteIndented(writer, indentation, $"${operation.Code}");
                }
                else
                {
                    WriteIndented(writer, indentation, $"/{{id}}${operation.Code}");
                }

                if (operation.Parameters != null)
                {
                    // write operation parameters inline
                    foreach (FhirParameter parameter in operation.Parameters.OrderBy(p => p.FieldOrder))
                    {
                        WriteIndented(writer, indentation, $"{parameter.Use}: {parameter.Name} ({parameter.FhirCardinality})");
                    }
                }
            }
        }

        /// <summary>Writes search parameters.</summary>
        /// <param name="writer">          The writer.</param>
        /// <param name="searchParameters">Options for controlling the search.</param>
        /// <param name="indentation">     The indentation.</param>
        private void WriteSearchParameters(
            TextWriter writer,
            IEnumerable<FhirSearchParam> searchParameters,
            int indentation)
        {
            foreach (FhirSearchParam searchParam in searchParameters)
            {
                WriteIndented(
                    writer,
                    indentation,
                    $"?{searchParam.Code} {searchParam.ValueType} ({searchParam.Name})");
            }
        }

        /// <summary>Writes the elements.</summary>
        /// <param name="writer">     The writer.</param>
        /// <param name="complex">    The complex.</param>
        /// <param name="indentation">The indentation.</param>
        private void WriteElements(
            TextWriter writer,
            FhirComplex complex,
            int indentation)
        {
            foreach (FhirElement element in complex.Elements.Values.OrderBy(s => s.FieldOrder))
            {
                WriteElement(writer, complex, element, indentation);
            }
        }

        /// <summary>Writes an element.</summary>
        /// <param name="writer">     The writer.</param>
        /// <param name="element">    The element.</param>
        /// <param name="indentation">The indentation.</param>
        private void WriteElement(
            TextWriter writer,
            FhirComplex complex,
            FhirElement element,
            int indentation)
        {
            string propertyType = string.Empty;

            if (element.ElementTypes != null)
            {
                foreach (FhirElementType elementType in element.ElementTypes.Values)
                {
                    string joiner = string.IsNullOrEmpty(propertyType) ? string.Empty : "|";

                    string profiles = string.Empty;
                    if ((elementType.Profiles != null) && (elementType.Profiles.Count > 0))
                    {
                        profiles = "(" + string.Join("|", elementType.Profiles.Values) + ")";
                    }

                    propertyType = $"{propertyType}{joiner}{elementType.Name}{profiles}";
                }
            }

            if (string.IsNullOrEmpty(propertyType))
            {
                propertyType = element.BaseTypeName;
            }

            WriteIndented(writer, indentation, $"- {element.NameForExport(_options.ElementNameStyle)}[{element.FhirCardinality}]: {propertyType}");

            // check for default value
            if (!string.IsNullOrEmpty(element.DefaultFieldName))
            {
                WriteIndented(writer, indentation + 1, $".{element.DefaultFieldName} = {element.DefaultFieldValue}");
            }

            // check for fixed value
            if (!string.IsNullOrEmpty(element.FixedFieldName))
            {
                WriteIndented(writer, indentation + 1, $".{element.FixedFieldName} = {element.FixedFieldValue}");
            }

            // either step into backbone definition OR extensions, don't write both
            if (complex.Components.ContainsKey(element.Path))
            {
                WriteComplex(writer, complex.Components[element.Path], indentation + 1);
            }
            else if (_info.ExtensionsByPath.ContainsKey(element.Path))
            {
                WriteExtensions(writer, _info.ExtensionsByPath[element.Path].Values, indentation + 1);
            }

            // check for slicing information
            if (element.Slicing != null)
            {
                WriteSlicings(writer, element.Slicing.Values, indentation + 1);
            }
        }

        /// <summary>Writes the slicings.</summary>
        /// <param name="writer">     The writer.</param>
        /// <param name="slicings">   The slicings.</param>
        /// <param name="indentation">The indentation.</param>
        private void WriteSlicings(
            TextWriter writer,
            IEnumerable<FhirSlicing> slicings,
            int indentation)
        {
            foreach (FhirSlicing slicing in slicings)
            {
                if (slicing.Slices.Count == 0)
                {
                    continue;
                }

                WriteSlicing(writer, slicing, indentation);
            }
        }

        /// <summary>Writes a slicing.</summary>
        /// <param name="writer">     The writer.</param>
        /// <param name="slicing">    The slicing.</param>
        /// <param name="indentation">The indentation.</param>
        private void WriteSlicing(
            TextWriter writer,
            FhirSlicing slicing,
            int indentation)
        {
            string rules = string.Empty;

            foreach (FhirSliceDiscriminatorRule rule in slicing.DiscriminatorRules.Values)
            {
                if (!string.IsNullOrEmpty(rules))
                {
                    rules += ", ";
                }

                rules += $"{rule.DiscriminatorTypeName}@{rule.Path}";
            }

            WriteIndented(
                writer,
                indentation,
                $": {slicing.DefinedByUrl} - {slicing.SlicingRules} ({rules})");

            // write slices inline
            int sliceNumber = 0;
            foreach (FhirComplex slice in slicing.Slices)
            {
                WriteIndented(writer, indentation + 1, $": Slice {sliceNumber++}:{slice.Name}");

                // recurse into this slice
                WriteComplex(writer, slice, indentation + 2);
            }
        }

        /// <summary>Writes a header.</summary>
        /// <param name="writer">The writer.</param>
        private void WriteHeader(TextWriter writer)
        {
            WriteIndented(writer, 0, $"Contents of: {_info.PackageName} version: {_info.VersionString}");
            WriteIndented(writer, 1, $"  Using Model Inheritance: {_options.UseModelInheritance}");
            WriteIndented(writer, 1, $"  Hiding Removed Parent Fields: {_options.HideRemovedParentFields}");
            WriteIndented(writer, 1, $"  Nesting Type Definitions: {_options.NestTypeDefinitions}");
            WriteIndented(writer, 1, $"  Primitive Naming Sylte: {_options.PrimitiveNameStyle}");
            WriteIndented(writer, 1, $"  Complex Type Naming Sylte: {_options.ComplexTypeNameStyle}");
            WriteIndented(writer, 1, $"  Resource Naming Sylte: {_options.ResourceNameStyle}");
            WriteIndented(writer, 1, $"  Interaction Naming Sylte: {_options.InteractionNameStyle}");
            WriteIndented(writer, 1, $"  Extension Support: {_options.ExtensionSupport}");

            if ((_options.ExportList != null) && _options.ExportList.Any())
            {
                string restrictions = string.Join("|", _options.ExportList);
                WriteIndented(writer, 1, $"  Restricted to: {restrictions}");
            }
        }

        /// <summary>Writes a footer.</summary>
        /// <param name="writer">The writer.</param>
        private void WriteFooter(TextWriter writer)
        {
            return;
        }

        /// <summary>
        /// Writes a line indented, convenience function for clarity in this language output.
        /// </summary>
        /// <param name="writer">     The writer.</param>
        /// <param name="indentation">The indentation.</param>
        /// <param name="value">      The value.</param>
        private static void WriteIndented(TextWriter writer, int indentation, string value)
        {
            switch (indentation)
            {
                case 0:
                    writer.WriteLine(value);
                    break;

                case 1:
                    writer.WriteLine($"  {value}");
                    break;

                case 2:
                    writer.WriteLine($"    {value}");
                    break;

                case 3:
                    writer.WriteLine($"      {value}");
                    break;

                case 4:
                    writer.WriteLine($"        {value}");
                    break;

                case 5:
                    writer.WriteLine($"          {value}");
                    break;

                case 6:
                    writer.WriteLine($"            {value}");
                    break;

                case 7:
                    writer.WriteLine($"              {value}");
                    break;

                case 8:
                    writer.WriteLine($"                {value}");
                    break;

                case 9:
                    writer.WriteLine($"                  {value}");
                    break;

                case 10:
                    writer.WriteLine($"                      {value}");
                    break;

                default:
                    writer.WriteLine($"{new string(' ', indentation * 2)}{value}");
                    break;
            }
        }
    }
}
