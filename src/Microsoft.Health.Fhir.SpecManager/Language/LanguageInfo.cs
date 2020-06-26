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
    /// <summary>Export to an Information format - used to check parsing and dump FHIR version info.</summary>
    public sealed class LanguageInfo : ILanguage
    {
        /// <summary>FHIR information we are exporting.</summary>
        private FhirVersionInfo _info;

        /// <summary>Options for controlling the export.</summary>
        private ExporterOptions _options;

        /// <summary>The currently in-use text writer.</summary>
        private TextWriter _writer;

        /// <summary>Name of the language.</summary>
        private const string _languageName = "Info";

        /// <summary>Dictionary mapping FHIR primitive types to language equivalents.</summary>
        private static readonly Dictionary<string, string> _primitiveTypeMap = new Dictionary<string, string>()
        {
            { "base", "base" },
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

        /// <summary>Gets the reserved words.</summary>
        /// <value>The reserved words.</value>
        private static HashSet<string> _reservedWords => new HashSet<string>();

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

        /// <summary>Gets the reserved words.</summary>
        /// <value>The reserved words.</value>
        HashSet<string> ILanguage.ReservedWords => _reservedWords;

        /// <summary>Gets the primitive configuration.</summary>
        /// <value>The primitive configuration.</value>
        HashSet<FhirTypeBase.NamingConvention> ILanguage.SupportedPrimitiveNameStyles => _allNamingStyles;

        /// <summary>Gets the complex type configuration.</summary>
        /// <value>The complex type configuration.</value>
        HashSet<FhirTypeBase.NamingConvention> ILanguage.SupportedComplexTypeNameStyles => _allNamingStyles;

        /// <summary>Gets the supported element name styles.</summary>
        /// <value>The supported element name styles.</value>
        HashSet<FhirTypeBase.NamingConvention> ILanguage.SupportedElementNameStyles => _allNamingStyles;

        /// <summary>Gets the resource configuration.</summary>
        /// <value>The resource configuration.</value>
        HashSet<FhirTypeBase.NamingConvention> ILanguage.SupportedResourceNameStyles => _allNamingStyles;

        /// <summary>Gets the interaction configuration.</summary>
        /// <value>The interaction configuration.</value>
        HashSet<FhirTypeBase.NamingConvention> ILanguage.SupportedInteractionNameStyles => _allNamingStyles;

        /// <summary>Gets the supported enum styles.</summary>
        /// <value>The supported enum styles.</value>
        HashSet<FhirTypeBase.NamingConvention> ILanguage.SupportedEnumStyles => _allNamingStyles;

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
            string filename = Path.Combine(exportDirectory, $"R{info.MajorVersion}.txt");

            using (StreamWriter writer = new StreamWriter(new FileStream(filename, FileMode.Create)))
            {
                _writer = writer;

                WriteHeader();

                WritePrimitiveTypes(_info.PrimitiveTypes.Values, 0);
                WriteComplexes(_info.ComplexTypes.Values, 0, "Complex Types");
                WriteComplexes(_info.Resources.Values, 0, "Resources");

                WriteOperations(_info.SystemOperations.Values, 0, true, "System Operations");
                WriteSearchParameters(_info.AllResourceParameters.Values, 0, "All Resource Parameters");
                WriteSearchParameters(_info.SearchResultParameters.Values, 0, "Search Result Parameters");
                WriteSearchParameters(_info.AllInteractionParameters.Values, 0, "All Interaction Parameters");

                WriteValueSets(_info.ValueSetsByUrl.Values, 0, "Value Sets");

                WriteFooter();
            }
        }

        /// <summary>Writes a value sets.</summary>
        /// <param name="valueSets">  Sets the value belongs to.</param>
        /// <param name="indentation">The indentation.</param>
        /// <param name="headerHint"> (Optional) The header hint.</param>
        private void WriteValueSets(
            IEnumerable<FhirValueSetCollection> valueSets,
            int indentation,
            string headerHint = null)
        {
            if (!string.IsNullOrEmpty(headerHint))
            {
                WriteIndented(indentation, $"{headerHint}: {valueSets.Count()} (unversioned)");
            }

            foreach (FhirValueSetCollection collection in valueSets.OrderBy(c => c.URL))
            {
                foreach (FhirValueSet vs in collection.ValueSetsByVersion.Values.OrderBy(v => v.Version))
                {
                    WriteIndented(
                        indentation,
                        $"- ValueSet: {vs.URL}|{vs.Version}");

                    foreach (FhirConcept value in vs.Concepts)
                    {
                        WriteIndented(
                            indentation + 1,
                            $"- #{value.Code}: {value.Display}");
                    }
                }
            }
        }

        /// <summary>Writes a value set.</summary>
        /// <param name="valueSet">   Set the value belongs to.</param>
        /// <param name="indentation">The indendation.</param>
        private void WriteValueSet(
            FhirValueSet valueSet,
            int indentation)
        {
            WriteIndented(
                indentation,
                $"- {valueSet.URL}|{valueSet.Version} ({valueSet.Name})");

        }

        /// <summary>Writes the complexes.</summary>
        /// <param name="complexes">  The complexes.</param>
        /// <param name="indentation">The indentation.</param>
        /// <param name="headerHint"> (Optional) The header hint.</param>
        private void WriteComplexes(
            IEnumerable<FhirComplex> complexes,
            int indentation,
            string headerHint = null)
        {
            if (!string.IsNullOrEmpty(headerHint))
            {
                WriteIndented(indentation, $"{headerHint}: {complexes.Count()}");
            }

            foreach (FhirComplex complex in complexes)
            {
                WriteComplex(complex, indentation);
            }
        }

        /// <summary>Writes a primitive types.</summary>
        /// <param name="primitives"> The primitives.</param>
        /// <param name="indentation">The indentation.</param>
        private void WritePrimitiveTypes(
            IEnumerable<FhirPrimitive> primitives,
            int indentation)
        {
            WriteIndented(indentation, $"Primitive Types: {primitives.Count()}");

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
            WriteIndented(
                indentation,
                $"- {primitive.Name}:" +
                    $" {primitive.NameForExport(_options.PrimitiveNameStyle)}" +
                    $"::{primitive.TypeForExport(_options.PrimitiveNameStyle, _primitiveTypeMap)}");

            // check for regex
            if (!string.IsNullOrEmpty(primitive.ValidationRegEx))
            {
                WriteIndented(indentation + 1, $"[{primitive.ValidationRegEx}]");
            }

            if (_info.ExtensionsByPath.ContainsKey(primitive.Path))
            {
                WriteExtensions(_info.ExtensionsByPath[primitive.Name].Values, indentation + 1);
            }
        }

        /// <summary>Writes the extensions.</summary>
        /// <param name="extensions"> The extensions.</param>
        /// <param name="indentation">The indentation.</param>
        private void WriteExtensions(
            IEnumerable<FhirComplex> extensions,
            int indentation)
        {
            WriteIndented(indentation, $"Extensions: {extensions.Count()}");

            foreach (FhirComplex extension in extensions)
            {
                WriteExtension(extension, indentation);
            }
        }

        /// <summary>Writes an extension.</summary>
        /// <param name="extension">  The extension.</param>
        /// <param name="indentation">The indentation.</param>
        private void WriteExtension(
            FhirComplex extension,
            int indentation)
        {
            WriteIndented(indentation, $"+{extension.URL}");

            if (extension.Elements.Count > 0)
            {
                WriteComplex(extension, indentation + 1);
            }
        }

        /// <summary>Writes a complex.</summary>
        /// <param name="complex">    The complex.</param>
        /// <param name="indentation">The indentation.</param>
        private void WriteComplex(
            FhirComplex complex,
            int indentation)
        {
            // write this type's line, if it's a root element
            // (sub-properties are written with cardinality in the prior loop)
            if (indentation == 0)
            {
                WriteIndented(indentation, $"- {complex.Name}: {complex.BaseTypeName}");
            }

            // write elements
            WriteElements(complex, indentation + 1);

            // check for extensions
            if (_info.ExtensionsByPath.ContainsKey(complex.Path))
            {
                WriteExtensions(_info.ExtensionsByPath[complex.Path].Values, indentation + 1);
            }

            // check for search parameters on this object
            if (complex.SearchParameters != null)
            {
                WriteSearchParameters(complex.SearchParameters.Values, indentation + 1);
            }

            // check for type operations
            if (complex.TypeOperations != null)
            {
                WriteOperations(complex.TypeOperations.Values, indentation + 1, true);
            }

            // check for instance operations
            if (complex.InstanceOperations != null)
            {
                WriteOperations(complex.TypeOperations.Values, indentation + 1, false);
            }
        }

        /// <summary>Writes the operations.</summary>
        /// <param name="operations"> The operations.</param>
        /// <param name="indentation">The indentation.</param>
        /// <param name="isTypeLevel">True if is type level, false if not.</param>
        /// <param name="headerHint"> (Optional) The header hint.</param>
        private void WriteOperations(
            IEnumerable<FhirOperation> operations,
            int indentation,
            bool isTypeLevel,
            string headerHint = null)
        {
            if (!string.IsNullOrEmpty(headerHint))
            {
                WriteIndented(indentation, $"{headerHint}: {operations.Count()}");
                indentation++;
            }

            foreach (FhirOperation operation in operations)
            {
                if (isTypeLevel)
                {
                    WriteIndented(indentation, $"${operation.Code}");
                }
                else
                {
                    WriteIndented(indentation, $"/{{id}}${operation.Code}");
                }

                if (operation.Parameters != null)
                {
                    // write operation parameters inline
                    foreach (FhirParameter parameter in operation.Parameters.OrderBy(p => p.FieldOrder))
                    {
                        WriteIndented(indentation + 1, $"{parameter.Use}: {parameter.Name} ({parameter.FhirCardinality})");
                    }
                }
            }
        }

        /// <summary>Writes search parameters.</summary>
        /// <param name="searchParameters">Options for controlling the search.</param>
        /// <param name="indentation">     The indentation.</param>
        /// <param name="headerHint">      (Optional) The header hint.</param>
        private void WriteSearchParameters(
            IEnumerable<FhirSearchParam> searchParameters,
            int indentation,
            string headerHint = null)
        {
            if (!string.IsNullOrEmpty(headerHint))
            {
                WriteIndented(indentation, $"{headerHint}: {searchParameters.Count()}");
                indentation++;
            }

            foreach (FhirSearchParam searchParam in searchParameters)
            {
                WriteIndented(
                    indentation,
                    $"?{searchParam.Code}={searchParam.ValueType} ({searchParam.Name})");
            }
        }

        /// <summary>Writes the elements.</summary>
        /// <param name="complex">    The complex.</param>
        /// <param name="indentation">The indentation.</param>
        private void WriteElements(
            FhirComplex complex,
            int indentation)
        {
            foreach (FhirElement element in complex.Elements.Values.OrderBy(s => s.FieldOrder))
            {
                WriteElement(complex, element, indentation);
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

            WriteIndented(indentation, $"- {element.NameForExport(_options.ElementNameStyle)}[{element.FhirCardinality}]: {propertyType}");

            // check for regex
            if (!string.IsNullOrEmpty(element.ValidationRegEx))
            {
                WriteIndented(indentation + 1, $"[{element.ValidationRegEx}]");
            }

            // check for default value
            if (!string.IsNullOrEmpty(element.DefaultFieldName))
            {
                WriteIndented(indentation + 1, $".{element.DefaultFieldName} = {element.DefaultFieldValue}");
            }

            // check for fixed value
            if (!string.IsNullOrEmpty(element.FixedFieldName))
            {
                WriteIndented(indentation + 1, $".{element.FixedFieldName} = {element.FixedFieldValue}");
            }

            if ((element.Codes != null) && (element.Codes.Count > 0))
            {
                string codes = string.Join("|", element.Codes);
                WriteIndented(indentation + 1, $"{{{codes}}}");
            }

            // either step into backbone definition OR extensions, don't write both
            if (complex.Components.ContainsKey(element.Path))
            {
                WriteComplex(complex.Components[element.Path], indentation + 1);
            }
            else if (_info.ExtensionsByPath.ContainsKey(element.Path))
            {
                WriteExtensions(_info.ExtensionsByPath[element.Path].Values, indentation + 1);
            }

            // check for slicing information
            if (element.Slicing != null)
            {
                WriteSlicings(element.Slicing.Values, indentation + 1);
            }
        }

        /// <summary>Writes the slicings.</summary>
        /// <param name="slicings">   The slicings.</param>
        /// <param name="indentation">The indentation.</param>
        private void WriteSlicings(
            IEnumerable<FhirSlicing> slicings,
            int indentation)
        {
            foreach (FhirSlicing slicing in slicings)
            {
                if (slicing.Slices.Count == 0)
                {
                    continue;
                }

                WriteSlicing(slicing, indentation);
            }
        }

        /// <summary>Writes a slicing.</summary>
        /// <param name="slicing">    The slicing.</param>
        /// <param name="indentation">The indentation.</param>
        private void WriteSlicing(
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
                indentation,
                $": {slicing.DefinedByUrl} - {slicing.SlicingRules} ({rules})");

            // write slices inline
            int sliceNumber = 0;
            foreach (FhirComplex slice in slicing.Slices)
            {
                WriteIndented(indentation + 1, $": Slice {sliceNumber++}:{slice.SliceName} - on {slice.Name}");

                // recurse into this slice
                WriteComplex(slice, indentation + 2);
            }
        }

        /// <summary>Writes a header.</summary>
        private void WriteHeader()
        {
            WriteIndented(0, $"Contents of: {_info.PackageName} version: {_info.VersionString}");
            WriteIndented(1, $"  Using Model Inheritance: {_options.UseModelInheritance}");
            WriteIndented(1, $"  Hiding Removed Parent Fields: {_options.HideRemovedParentFields}");
            WriteIndented(1, $"  Nesting Type Definitions: {_options.NestTypeDefinitions}");
            WriteIndented(1, $"  Primitive Naming Sylte: {_options.PrimitiveNameStyle}");
            WriteIndented(1, $"  Complex Type Naming Sylte: {_options.ComplexTypeNameStyle}");
            WriteIndented(1, $"  Resource Naming Sylte: {_options.ResourceNameStyle}");
            WriteIndented(1, $"  Interaction Naming Sylte: {_options.InteractionNameStyle}");
            WriteIndented(1, $"  Extension Support: {_options.ExtensionSupport}");

            if ((_options.ExportList != null) && _options.ExportList.Any())
            {
                string restrictions = string.Join("|", _options.ExportList);
                WriteIndented(1, $"  Restricted to: {restrictions}");
            }
        }

        /// <summary>Writes a footer.</summary>
        private void WriteFooter()
        {
            return;
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
