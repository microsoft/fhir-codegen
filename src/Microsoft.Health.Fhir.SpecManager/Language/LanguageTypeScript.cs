// <copyright file="LanguageTypeScript.cs" company="Microsoft Corporation">
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
    /// <summary>Export to TypeScript - serializable to/from JSON.</summary>
    public sealed class LanguageTypeScript : ILanguage
    {
        /// <summary>FHIR information we are exporting.</summary>
        private FhirVersionInfo _info;

        /// <summary>Options for controlling the export.</summary>
        private ExporterOptions _options;

        /// <summary>The currently in-use text writer.</summary>
        private TextWriter _writer;

        /// <summary>Name of the language.</summary>
        private const string _languageName = "TypeScript";

        /// <summary>Dictionary mapping FHIR primitive types to language equivalents.</summary>
        private static readonly Dictionary<string, string> _primitiveTypeMap = new Dictionary<string, string>()
        {
            { "base", "Object" },
            { "base64Binary", "string" },
            { "boolean", "boolean" },
            { "canonical", "string" },
            { "code", "string" },
            { "date", "string" },
            { "dateTime", "string" },
            { "decimal", "number" },
            { "id", "string" },
            { "instant", "string" },
            { "integer", "number" },
            { "integer64", "string" },       // int64 serializes as string, need to add custom handling here
            { "markdown", "string" },
            { "oid", "string" },
            { "positiveInt", "number" },
            { "string", "string" },
            { "time", "string" },
            { "unsignedInt", "number" },
            { "uri", "string" },
            { "url", "string" },
            { "uuid", "string" },
            { "xhtml", "string" },
        };

        /// <summary>Gets the reserved words.</summary>
        /// <value>The reserved words.</value>
        private static HashSet<string> _reservedWords => new HashSet<string>()
        {
            "const",
            "enum",
            "export",
            "interface",
        };

        /// <summary>This option supports only Pascal case.</summary>
        private static readonly HashSet<FhirTypeBase.NamingConvention> _pascalStyle = new HashSet<FhirTypeBase.NamingConvention>()
        {
            FhirTypeBase.NamingConvention.PascalCase,
        };

        /// <summary>This option supports only Pascal case.</summary>
        private static readonly HashSet<FhirTypeBase.NamingConvention> _camelStyle = new HashSet<FhirTypeBase.NamingConvention>()
        {
            FhirTypeBase.NamingConvention.CamelCase,
        };

        /// <summary>The not supported style.</summary>
        private static readonly HashSet<FhirTypeBase.NamingConvention> _notSupportedStyle = new HashSet<FhirTypeBase.NamingConvention>()
        {
            FhirTypeBase.NamingConvention.None,
        };

        /// <summary>Gets the name of the language.</summary>
        /// <value>The name of the language.</value>
        string ILanguage.LanguageName => _languageName;

        /// <summary>Gets a value indicating whether the language supports model inheritance.</summary>
        /// <value>True if the language supports model inheritance, false if not.</value>
        bool ILanguage.SupportsModelInheritance => true;

        /// <summary>Gets a value indicating whether the supports hiding parent field.</summary>
        /// <value>True if the language supports hiding parent field, false if not.</value>
        bool ILanguage.SupportsHidingParentField => false;

        /// <summary>
        /// Gets a value indicating whether the language supports nested type definitions.
        /// </summary>
        /// <value>True if the language supports nested type definitions, false if not.</value>
        bool ILanguage.SupportsNestedTypeDefinitions => false;

        /// <summary>Gets a value indicating whether the supports slicing.</summary>
        /// <value>True if supports slicing, false if not.</value>
        bool ILanguage.SupportsSlicing => false;

        /// <summary>Gets the FHIR primitive type map.</summary>
        /// <value>The FHIR primitive type map.</value>
        Dictionary<string, string> ILanguage.FhirPrimitiveTypeMap => _primitiveTypeMap;

        /// <summary>Gets the reserved words.</summary>
        /// <value>The reserved words.</value>
        HashSet<string> ILanguage.ReservedWords => _reservedWords;

        /// <summary>Gets the primitive configuration.</summary>
        /// <value>The primitive configuration.</value>
        HashSet<FhirTypeBase.NamingConvention> ILanguage.SupportedPrimitiveNameStyles => _notSupportedStyle;

        /// <summary>Gets the complex type configuration.</summary>
        /// <value>The complex type configuration.</value>
        HashSet<FhirTypeBase.NamingConvention> ILanguage.SupportedComplexTypeNameStyles => _pascalStyle;

        /// <summary>Gets the supported element name styles.</summary>
        /// <value>The supported element name styles.</value>
        HashSet<FhirTypeBase.NamingConvention> ILanguage.SupportedElementNameStyles => _camelStyle;

        /// <summary>Gets the resource configuration.</summary>
        /// <value>The resource configuration.</value>
        HashSet<FhirTypeBase.NamingConvention> ILanguage.SupportedResourceNameStyles => _pascalStyle;

        /// <summary>Gets the interaction configuration.</summary>
        /// <value>The interaction configuration.</value>
        HashSet<FhirTypeBase.NamingConvention> ILanguage.SupportedInteractionNameStyles => _notSupportedStyle;

        /// <summary>Gets the supported enum styles.</summary>
        /// <value>The supported enum styles.</value>
        HashSet<FhirTypeBase.NamingConvention> ILanguage.SupportedEnumStyles => _pascalStyle;

        /// <summary>Export the passed FHIR version into the specified directory.</summary>
        /// <param name="info">           The information.</param>
        /// <param name="options">        Options for controlling the operation.</param>
        /// <param name="exportDirectory">Directory to write files.</param>
        void ILanguage.Export(
            FhirVersionInfo info,
            ExporterOptions options,
            string exportDirectory)
        {
            // set internal vars so we don't pass them to every function
            // this is ugly, but the interface patterns get bad quickly because we need the type map to copy the FHIR info
            _info = info;
            _options = options;

            // create a filename for writing (single file for now)
            string filename = Path.Combine(exportDirectory, $"R{info.MajorVersion}.ts");

            using (StreamWriter writer = new StreamWriter(new FileStream(filename, FileMode.Create)))
            {
                _writer = writer;

                WriteHeader();

                if (options.PrimitiveNameStyle != FhirTypeBase.NamingConvention.None)
                {
                    WritePrimitiveTypes(_info.PrimitiveTypes.Values, 0);
                }

                if (options.ComplexTypeNameStyle != FhirTypeBase.NamingConvention.None)
                {
                    WriteComplexes(_info.ComplexTypes.Values, 0, false);
                    WriteComplexes(_info.Resources.Values, 0, true);
                }

                WriteFooter();
            }
        }

        /// <summary>Writes the complexes.</summary>
        /// <param name="complexes">  The complexes.</param>
        /// <param name="indentation">The indentation.</param>
        /// <param name="headerHint"> (Optional) The header hint.</param>
        /// <param name="isResource"> (Optional) True if is resource, false if not.</param>
        private void WriteComplexes(
            IEnumerable<FhirComplex> complexes,
            int indentation,
            bool isResource = false)
        {
            foreach (FhirComplex complex in complexes.OrderBy(c => c.Name))
            {
                WriteComplex(complex, indentation, isResource);
            }
        }

        /// <summary>Writes a complex.</summary>
        /// <param name="complex">    The complex.</param>
        /// <param name="indentation">The indentation.</param>
        private void WriteComplex(
            FhirComplex complex,
            int indentation,
            bool isResource)
        {
            // check for nested components
            if (complex.Components != null)
            {
                foreach (FhirComplex component in complex.Components.Values)
                {
                    WriteComplex(component, indentation, false);
                }
            }

            if (!string.IsNullOrEmpty(complex.Comment))
            {
                WriteIndentedComment(indentation, complex.Comment);
            }

            if (string.IsNullOrEmpty(complex.BaseTypeName) ||
                complex.Name.Equals("Element", StringComparison.Ordinal))
            {
                WriteIndented(
                    indentation,
                    $"export interface {complex.NameForExport(_options.ComplexTypeNameStyle)} {{");
            }
            else if (complex.Name.Equals(complex.BaseTypeName, StringComparison.Ordinal))
            {
                WriteIndented(
                    indentation,
                    $"export interface" +
                        $" {complex.NameForExport(_options.ComplexTypeNameStyle, true)}" +
                        $" extends Element {{");
            }
            else if ((complex.Components != null) && complex.Components.ContainsKey(complex.Path))
            {
                WriteIndented(
                    indentation,
                    $"export interface" +
                        $" {complex.NameForExport(_options.ComplexTypeNameStyle, true)}" +
                        $" extends" +
                        $" {complex.TypeForExport(_options.ComplexTypeNameStyle, _primitiveTypeMap, false)} {{");
            }
            else
            {
                WriteIndented(
                    indentation,
                    $"export interface" +
                        $" {complex.NameForExport(_options.ComplexTypeNameStyle, true)}" +
                        $" extends" +
                        $" {complex.TypeForExport(_options.ComplexTypeNameStyle, _primitiveTypeMap)} {{");
            }

            if (isResource && ShouldWriteResourceName(complex.Name))
            {
                WriteIndented(indentation + 1, "/** Resource Type Name (for serialization) */");
                WriteIndented(indentation + 1, $"resourceType: '{complex.Name}'");
            }

            // write elements
            WriteElements(complex, indentation + 1, out List<FhirElement> elementsWithCodes);

            // close interface (type)
            WriteIndented(indentation, "}");

            foreach (FhirElement element in elementsWithCodes)
            {
                WriteCode(element, indentation);
            }
        }

        /// <summary>Writes a code.</summary>
        /// <param name="element">    The element.</param>
        /// <param name="indentation">The indentation.</param>
        private void WriteCode(
            FhirElement element,
            int indentation)
        {
            WriteIndented(indentation, $"/**");
            WriteIndented(indentation, $" * Code Values for the {element.Path} field");
            WriteIndented(indentation, $" */");

            string codeName = FhirUtils.ToConvention(
                $"{element.Path}.Codes",
                string.Empty,
                _options.EnumStyle);

            if (codeName.Contains("[x]"))
            {
                codeName = codeName.Replace("[x]", string.Empty);
            }

            WriteIndented(indentation, $"export enum {codeName} {{");

            foreach (string code in element.Codes)
            {
                FhirUtils.SanitizeForCode(code, _reservedWords, out string name, out string value);

                WriteIndented(indentation + 1, $"{name.ToUpperInvariant()} = \"{value}\",");
            }

            WriteIndented(indentation, "}");
        }

        /// <summary>Determine if we should write resource name.</summary>
        /// <param name="name">The name.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        private static bool ShouldWriteResourceName(string name)
        {
            switch (name)
            {
                case "Resource":
                case "DomainResource":
                case "MetadataResource":
                case "CanonicalResource":
                    return false;
            }

            return true;
        }

        /// <summary>Writes the elements.</summary>
        /// <param name="complex">    The complex.</param>
        /// <param name="indentation">The indentation.</param>
        private void WriteElements(
            FhirComplex complex,
            int indentation,
            out List<FhirElement> elementsWithCodes)
        {
            elementsWithCodes = new List<FhirElement>();

            foreach (FhirElement element in complex.Elements.Values.OrderBy(s => s.Name))
            {
                if (_options.UseModelInheritance && element.IsInherited)
                {
                    continue;
                }

                WriteElement(complex, element, indentation);

                if ((element.Codes != null) && (element.Codes.Count > 0))
                {
                    elementsWithCodes.Add(element);
                }
            }
        }

        /// <summary>Writes an element.</summary>
        /// <param name="complex">    The complex.</param>
        /// <param name="element">    The element.</param>
        /// <param name="indentation">The indentation.</param>
        private void WriteElement(
            FhirComplex complex,
            FhirElement element,
            int indentation)
        {
            string optionalFlagString = element.IsOptional ? "?" : string.Empty;
            string arrayFlagString = element.IsArray ? "[]" : string.Empty;

            Dictionary<string, string> values = element.NamesAndTypesForExport(
                _options.ElementNameStyle,
                _options.ComplexTypeNameStyle,
                false,
                string.Empty,
                complex.Components.ContainsKey(element.Path));

            foreach (KeyValuePair<string, string> kvp in values)
            {
                if (!string.IsNullOrEmpty(element.Comment))
                {
                    WriteIndentedComment(indentation, element.Comment);
                }

                WriteIndented(
                    indentation,
                    $"{kvp.Key}{optionalFlagString}: {kvp.Value}{arrayFlagString};");

                WriteIndented(indentation, $"_{kvp.Key}?: Element;");
            }
        }

        /// <summary>Writes a primitive types.</summary>
        /// <param name="primitives"> The primitives.</param>
        /// <param name="indentation">The indentation.</param>
        private void WritePrimitiveTypes(
            IEnumerable<FhirPrimitive> primitives,
            int indentation)
        {
            foreach (FhirPrimitive primitive in primitives)
            {
                WritePrimitiveType(primitive, indentation);
            }
        }

        /// <summary>Writes a primitive type.</summary>
        /// <param name="primitive">  The primitive.</param>
        /// <param name="indentation">The indentation.</param>
        private void WritePrimitiveType(
            FhirPrimitive primitive,
            int indentation)
        {
            if (!string.IsNullOrEmpty(primitive.Comment))
            {
                WriteIndentedComment(indentation, primitive.Comment);
            }

            WriteIndented(
                indentation,
                $"export type" +
                    $" {primitive.NameForExport(_options.PrimitiveNameStyle)}" +
                    $" =" +
                    $" {primitive.TypeForExport(_options.PrimitiveNameStyle, _primitiveTypeMap)};");
        }

        /// <summary>Writes a header.</summary>
        private void WriteHeader()
        {
            WriteIndented(0, "// <auto-generated/>");
            WriteIndented(0, $"// Contents of: {_info.PackageName} version: {_info.VersionString}");
            WriteIndented(1, $"// Using Model Inheritance: {_options.UseModelInheritance}");
            WriteIndented(1, $"// Hiding Removed Parent Fields: {_options.HideRemovedParentFields}");
            WriteIndented(1, $"// Nesting Type Definitions: {_options.NestTypeDefinitions}");
            WriteIndented(1, $"// Primitive Naming Sylte: {_options.PrimitiveNameStyle}");
            WriteIndented(1, $"// Complex Type Naming Sylte: {_options.ComplexTypeNameStyle}");
            WriteIndented(1, $"// Resource Naming Sylte: {_options.ResourceNameStyle}");
            WriteIndented(1, $"// Interaction Naming Sylte: {_options.InteractionNameStyle}");
            WriteIndented(1, $"// Extension Support: {_options.ExtensionSupport}");

            if ((_options.ExportList != null) && _options.ExportList.Any())
            {
                string restrictions = string.Join("|", _options.ExportList);
                WriteIndented(1, $"// Restricted to: {restrictions}");
            }
        }

        /// <summary>Writes a footer.</summary>
        private void WriteFooter()
        {
            return;
        }

        /// <summary>Writes an indented comment.</summary>
        /// <param name="indentation">The indentation.</param>
        /// <param name="value">      The value.</param>
        private void WriteIndentedComment(int indentation, string value)
        {
            string prefix = $"{new string(' ', indentation * 2)} * ";

            WriteIndented(indentation, $"/**");
            _writer.Write(prefix);

            prefix = $"\n{prefix}";

            _writer.WriteLine(value.Replace("\n", prefix).Replace("\r", string.Empty));
            WriteIndented(indentation, $" */");
        }

        /// <summary>
        /// Writes a line indented, convenience function for clarity in this language output.
        /// </summary>
        /// <param name="indentation">The indentation.</param>
        /// <param name="value">      The value.</param>
        private void WriteIndented(int indentation, string value)
        {
            switch (indentation)
            {
                case 0:
                    _writer.WriteLine(value);
                    break;

                case 1:
                    _writer.WriteLine($"  {value}");
                    break;

                case 2:
                    _writer.WriteLine($"    {value}");
                    break;

                case 3:
                    _writer.WriteLine($"      {value}");
                    break;

                case 4:
                    _writer.WriteLine($"        {value}");
                    break;

                case 5:
                    _writer.WriteLine($"          {value}");
                    break;

                case 6:
                    _writer.WriteLine($"            {value}");
                    break;

                case 7:
                    _writer.WriteLine($"              {value}");
                    break;

                case 8:
                    _writer.WriteLine($"                {value}");
                    break;

                case 9:
                    _writer.WriteLine($"                  {value}");
                    break;

                case 10:
                    _writer.WriteLine($"                      {value}");
                    break;

                default:
                    _writer.WriteLine($"{new string(' ', indentation * 2)}{value}");
                    break;
            }
        }

    }
}
