// <copyright file="TypeScript.cs" company="Microsoft Corporation">
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
    public sealed class TypeScript : ILanguage
    {
        /// <summary>The systems named by display.</summary>
        private static HashSet<string> _systemsNamedByDisplay = new HashSet<string>()
        {
            /// <summary>Units of Measure have incomprehensible codes after naming substitutions.</summary>
            "http://unitsofmeasure.org",
        };

        private static HashSet<string> _systemsNamedByCode = new HashSet<string>()
        {
            /// <summary>Operation Outcomes include c-style string formats in display.</summary>
            "http://terminology.hl7.org/CodeSystem/operation-outcome",

            /// <summary>Descriptions have quoted values.</summary>
            "http://terminology.hl7.org/CodeSystem/smart-capabilities",

            /// <summary>Descriptions have quoted values.</summary>
            "http://hl7.org/fhir/v2/0301",

            /// <summary>Display values are too long to be useful.</summary>
            "http://terminology.hl7.org/CodeSystem/v2-0178",

            /// <summary>Display values are too long to be useful.</summary>
            "http://terminology.hl7.org/CodeSystem/v2-0277",

            /// <summary>Display values are too long to be useful.</summary>
            "http://terminology.hl7.org/CodeSystem/v3-VaccineManufacturer",

            /// <summary>Display values are too long to be useful.</summary>
            "http://hl7.org/fhir/v2/0278",

            /// <summary>Display includes operation symbols: $.</summary>
            "http://terminology.hl7.org/CodeSystem/testscript-operation-codes",

            /// <summary>Display are often just symbols.</summary>
            "http://hl7.org/fhir/v2/0290",

            /// <summary>Display includes too many Unicode characters (invalid export names).</summary>
            "http://hl7.org/fhir/v2/0255",

            /// <summary>Display includes too many Unicode characters (invalid export names).</summary>
            "http://hl7.org/fhir/v2/0256",
        };

        /// <summary>FHIR information we are exporting.</summary>
        private FhirVersionInfo _info;

        /// <summary>Options for controlling the export.</summary>
        private ExporterOptions _options;

        /// <summary>True to export enums.</summary>
        private bool _exportEnums;

        /// <summary>The exported codes.</summary>
        private HashSet<string> _exportedCodes = new HashSet<string>();

        /// <summary>The exported resources.</summary>
        private List<string> _exportedResources = new List<string>();

        /// <summary>The currently in-use text writer.</summary>
        private ExportStreamWriter _writer;

        /// <summary>Name of the language.</summary>
        private const string _languageName = "TypeScript";

        /// <summary>The single file export extension.</summary>
        private const string _singleFileExportExtension = ".ts";

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
        private static readonly HashSet<string> _reservedWords = new HashSet<string>()
        {
            "const",
            "enum",
            "export",
            "interface",
        };

        /// <summary>Gets the name of the language.</summary>
        /// <value>The name of the language.</value>
        string ILanguage.LanguageName => _languageName;

        /// <summary>
        /// Gets the single file extension for this language - null or empty indicates a multi-file
        /// export (exporter should copy the contents of the directory).
        /// </summary>
        string ILanguage.SingleFileExportExtension => _singleFileExportExtension;

        /// <summary>Gets the FHIR primitive type map.</summary>
        /// <value>The FHIR primitive type map.</value>
        Dictionary<string, string> ILanguage.FhirPrimitiveTypeMap => _primitiveTypeMap;

        /// <summary>Gets the reserved words.</summary>
        /// <value>The reserved words.</value>
        HashSet<string> ILanguage.ReservedWords => _reservedWords;

        /// <summary>
        /// Gets a list of FHIR class types that the language WILL export, regardless of user choices.
        /// Used to provide information to users.
        /// </summary>
        List<ExporterOptions.FhirExportClassType> ILanguage.RequiredExportClassTypes => new List<ExporterOptions.FhirExportClassType>()
        {
            ExporterOptions.FhirExportClassType.ComplexType,
            ExporterOptions.FhirExportClassType.Resource,
        };

        /// <summary>
        /// Gets a list of FHIR class types that the language CAN export, depending on user choices.
        /// </summary>
        List<ExporterOptions.FhirExportClassType> ILanguage.OptionalExportClassTypes => new List<ExporterOptions.FhirExportClassType>()
        {
            ExporterOptions.FhirExportClassType.Enum,
        };

        /// <summary>Gets language-specific options and their descriptions.</summary>
        Dictionary<string, string> ILanguage.LanguageOptions => new Dictionary<string, string>();

        /// <summary>Export the passed FHIR version into the specified directory.</summary>
        /// <param name="info">           The information.</param>
        /// <param name="serverInfo">     Information describing the server.</param>
        /// <param name="options">        Options for controlling the operation.</param>
        /// <param name="exportDirectory">Directory to write files.</param>
        void ILanguage.Export(
            FhirVersionInfo info,
            FhirServerInfo serverInfo,
            ExporterOptions options,
            string exportDirectory)
        {
            // set internal vars so we don't pass them to every function
            // this is ugly, but the interface patterns get bad quickly because we need the type map to copy the FHIR info
            _info = info;
            _options = options;

            _exportedCodes = new HashSet<string>();
            _exportedResources = new List<string>();

            if (options.OptionalClassTypesToExport.Contains(ExporterOptions.FhirExportClassType.Enum))
            {
                _exportEnums = true;
            }
            else
            {
                _exportEnums = false;
            }

            // create a filename for writing (single file for now)
            string filename = Path.Combine(exportDirectory, $"R{info.MajorVersion}.ts");

            using (FileStream stream = new FileStream(filename, FileMode.Create))
            using (ExportStreamWriter writer = new ExportStreamWriter(stream))
            {
                _writer = writer;

                WriteHeader();

                WriteComplexes(_info.ComplexTypes.Values, false);
                WriteComplexes(_info.Resources.Values, true);

                if (_exportEnums)
                {
                    WriteValueSets(_info.ValueSetsByUrl.Values);
                }

                WriteExpandedResourceInterfaceBinding();

                WriteFooter();
            }
        }

        /// <summary>Writes the expanded resource interface binding.</summary>
        private void WriteExpandedResourceInterfaceBinding()
        {
            if (_exportedResources.Count == 0)
            {
                return;
            }

            _exportedResources.Sort();

            WriteIndentedComment("Resource binding for generic use.");

            if (_exportedResources.Count == 1)
            {
                _writer.WriteLineIndented($"export type FhirResource = {_exportedResources[0]};");
                return;
            }

            _writer.WriteLineIndented("export type FhirResource = ");

            _writer.IncreaseIndent();

            int index = 0;
            int last = _exportedResources.Count - 1;
            foreach (string exportedName in _exportedResources)
            {
                if (index == 0)
                {
                    _writer.WriteLineIndented(exportedName);
                }
                else if (index == last)
                {
                    _writer.WriteLineIndented("|" + exportedName + ";");
                }
                else
                {
                    _writer.WriteLineIndented("|" + exportedName);
                }

                index++;
            }

            _writer.DecreaseIndent();
        }

        /// <summary>Writes a value sets.</summary>
        /// <param name="valueSets">List of valueSetCollections.</param>
        private void WriteValueSets(
            IEnumerable<FhirValueSetCollection> valueSets)
        {
            Dictionary<string, WrittenCodeInfo> writtenCodesAndNames = new Dictionary<string, WrittenCodeInfo>();
            HashSet<string> writtenNames = new HashSet<string>();

            foreach (FhirValueSetCollection collection in valueSets.OrderBy(c => c.URL))
            {
                foreach (FhirValueSet vs in collection.ValueSetsByVersion.Values.OrderBy(v => v.Version))
                {
                    WriteValueSet(
                        vs,
                        ref writtenCodesAndNames,
                        ref writtenNames);
                }
            }
        }

        /// <summary>Writes a value set.</summary>
        /// <param name="vs">                  The value set.</param>
        /// <param name="writtenCodesAndNames">[in,out] The written codes, to prevent duplication
        ///  without writing all code systems.</param>
        /// <param name="writtenNames">        [in,out] List of names of the writtens.</param>
        private void WriteValueSet(
            FhirValueSet vs,
            ref Dictionary<string, WrittenCodeInfo> writtenCodesAndNames,
            ref HashSet<string> writtenNames)
        {
            string vsName = FhirUtils.SanitizeForProperty(vs.Id ?? vs.Name, _reservedWords);

            vsName = FhirUtils.SanitizedToConvention(vsName, FhirTypeBase.NamingConvention.PascalCase);

            foreach (FhirConcept concept in vs.Concepts.OrderBy(c => c.Code))
            {
                if (writtenCodesAndNames.ContainsKey(concept.Key()))
                {
                    continue;
                }

                string input = concept.Display;
                if (_systemsNamedByDisplay.Contains(concept.System))
                {
                    input = concept.Display;
                }
                else if (_systemsNamedByCode.Contains(concept.System))
                {
                    input = concept.Code;
                }
                else if (string.IsNullOrEmpty(input))
                {
                    input = concept.Code;
                }

                string codeName = FhirUtils.SanitizeForProperty(input, _reservedWords);
                string codeValue = FhirUtils.SanitizeForValue(concept.Code);

                codeName = FhirUtils.SanitizedToConvention(codeName, FhirTypeBase.NamingConvention.PascalCase);

                string constName;
                if (!string.IsNullOrEmpty(concept.SystemLocalName))
                {
                    constName = $"{concept.SystemLocalName}_{codeName}";
                }
                else
                {
                    constName = $"{vsName}_{codeName}";
                }

                if (writtenNames.Contains(constName))
                {
                    // start at 2 so that the unadorned version makes sense as v1
                    for (int i = 2; i < 1000; i++)
                    {
                        if (writtenNames.Contains($"{constName}_{i}"))
                        {
                            continue;
                        }

                        constName = $"{constName}_{i}";
                        break;
                    }
                }

                writtenCodesAndNames.Add(
                    concept.Key(),
                    new WrittenCodeInfo() { Name = codeName, ConstName = constName });
                writtenNames.Add(constName);

                _writer.WriteLineIndented($"const {constName}: Coding = {{");
                _writer.IncreaseIndent();

                _writer.WriteLineIndented($"code: \"{codeValue}\",");

                if (!string.IsNullOrEmpty(concept.Display))
                {
                    _writer.WriteLineIndented($"display: \"{FhirUtils.SanitizeForQuoted(concept.Display)}\",");
                }

                _writer.WriteLineIndented($"system: \"{concept.System}\"");

                _writer.DecreaseIndent();

                _writer.WriteLineIndented("};");
            }

            if (!string.IsNullOrEmpty(vs.Description))
            {
                WriteIndentedComment(vs.Description);
            }
            else
            {
                WriteIndentedComment($"Value Set: {vs.URL}|{vs.Version}");
            }

            _writer.WriteLineIndented($"export const {vsName} = {{");
            _writer.IncreaseIndent();

            bool prefixWithSystem = vs.ReferencedCodeSystems.Count > 1;
            HashSet<string> usedValues = new HashSet<string>();

            // TODO: shouldn't loop over this twice, but writer functions don't allow writing in two places at once yet
            foreach (FhirConcept concept in vs.Concepts.OrderBy(c => c.Code))
            {
                string codeKey = concept.Key();

                if (!string.IsNullOrEmpty(concept.Definition))
                {
                    WriteIndentedComment(concept.Definition);
                }

                string name;

                if (prefixWithSystem)
                {
                    name = $"{writtenCodesAndNames[codeKey].Name}_{concept.SystemLocalName}";
                }
                else
                {
                    name = writtenCodesAndNames[codeKey].Name;
                }

                if (usedValues.Contains(name))
                {
                    // start at 2 so that the unadorned version makes sense as v1
                    for (int i = 2; i < 1000; i++)
                    {
                        if (usedValues.Contains($"{name}_{i}"))
                        {
                            continue;
                        }

                        name = $"{name}_{i}";
                        break;
                    }
                }

                usedValues.Add(name);

                _writer.WriteLineIndented($"{name}: {writtenCodesAndNames[codeKey].ConstName},");
            }

            _writer.DecreaseIndent();

            _writer.WriteLineIndented("};");
        }

        /// <summary>Writes the complexes.</summary>
        /// <param name="complexes"> The complexes.</param>
        /// <param name="isResource">(Optional) True if is resource, false if not.</param>
        private void WriteComplexes(
            IEnumerable<FhirComplex> complexes,
            bool isResource = false)
        {
            foreach (FhirComplex complex in complexes.OrderBy(c => c.Name))
            {
                WriteComplex(complex, isResource);
            }
        }

        /// <summary>Writes a complex.</summary>
        /// <param name="complex">   The complex.</param>
        /// <param name="isResource">True if is resource, false if not.</param>
        private void WriteComplex(
            FhirComplex complex,
            bool isResource)
        {
            // check for nested components
            if (complex.Components != null)
            {
                foreach (FhirComplex component in complex.Components.Values)
                {
                    WriteComplex(component, false);
                }
            }

            if (!string.IsNullOrEmpty(complex.Comment))
            {
                WriteIndentedComment(complex.Comment);
            }

            string exportName;

            if (string.IsNullOrEmpty(complex.BaseTypeName) ||
                complex.Name.Equals("Element", StringComparison.Ordinal))
            {
                exportName = complex.NameForExport(FhirTypeBase.NamingConvention.PascalCase);
                _writer.WriteLineIndented($"export interface {exportName} {{");
            }
            else if (complex.Name.Equals(complex.BaseTypeName, StringComparison.Ordinal))
            {
                exportName = complex.NameForExport(FhirTypeBase.NamingConvention.PascalCase, true);
                _writer.WriteLineIndented($"export interface {exportName} {{");
            }
            else
            {
                exportName = complex.NameForExport(FhirTypeBase.NamingConvention.PascalCase, true);
                string typeName = complex.TypeForExport(FhirTypeBase.NamingConvention.PascalCase, _primitiveTypeMap, false);

                _writer.WriteLineIndented($"export interface {exportName} extends {typeName} {{");
            }

            _writer.IncreaseIndent();

            if (isResource)
            {
                if (ShouldWriteResourceName(complex.Name))
                {
                    _exportedResources.Add(exportName);

                    _writer.WriteLineIndented("/** Resource Type Name (for serialization) */");
                    _writer.WriteLineIndented($"readonly resourceType: '{complex.Name}';");
                }
                else
                {
                    _writer.WriteLineIndented("/** Resource Type Name (for serialization) */");
                    _writer.WriteLineIndented($"readonly resourceType: string;");
                }
            }

            // write elements
            WriteElements(complex, out List<FhirElement> elementsWithCodes);

            _writer.DecreaseIndent();

            // close interface (type)
            _writer.WriteLineIndented("}");

            if (_exportEnums)
            {
                foreach (FhirElement element in elementsWithCodes)
                {
                    WriteCode(element);
                }
            }
        }

        /// <summary>Writes a code.</summary>
        /// <param name="element">The element.</param>
        private void WriteCode(
            FhirElement element)
        {
            string codeName = FhirUtils.ToConvention(
                $"{element.Path}.Codes",
                string.Empty,
                FhirTypeBase.NamingConvention.PascalCase);

            if (codeName.Contains("[x]"))
            {
                codeName = codeName.Replace("[x]", string.Empty);
            }

            if (_exportedCodes.Contains(codeName))
            {
                return;
            }

            _exportedCodes.Add(codeName);

            _writer.WriteLineIndented($"/**");
            _writer.WriteLineIndented($" * Code Values for the {element.Path} field");
            _writer.WriteLineIndented($" */");

            _writer.WriteLineIndented($"export enum {codeName} {{");

            _writer.IncreaseIndent();

            if (_info.TryGetValueSet(element.ValueSet, out FhirValueSet vs))
            {
                foreach (FhirConcept concept in vs.Concepts)
                {
                    FhirUtils.SanitizeForCode(concept.Code, _reservedWords, out string name, out string value);

                    _writer.WriteLineIndented($"{name.ToUpperInvariant()} = \"{value}\",");
                }
            }
            else
            {
                foreach (string code in element.Codes)
                {
                    FhirUtils.SanitizeForCode(code, _reservedWords, out string name, out string value);

                    _writer.WriteLineIndented($"{name.ToUpperInvariant()} = \"{value}\",");
                }
            }

            _writer.DecreaseIndent();

            _writer.WriteLineIndented("}");
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
        /// <param name="complex">          The complex.</param>
        /// <param name="elementsWithCodes">[out] The elements with codes.</param>
        private void WriteElements(
            FhirComplex complex,
            out List<FhirElement> elementsWithCodes)
        {
            elementsWithCodes = new List<FhirElement>();

            foreach (FhirElement element in complex.Elements.Values.OrderBy(s => s.Name))
            {
                if (element.IsInherited)
                {
                    continue;
                }

                WriteElement(complex, element);

                if ((element.Codes != null) && (element.Codes.Count > 0))
                {
                    elementsWithCodes.Add(element);
                }
            }
        }

        /// <summary>Writes an element.</summary>
        /// <param name="complex">The complex.</param>
        /// <param name="element">The element.</param>
        private void WriteElement(
            FhirComplex complex,
            FhirElement element)
        {
            string optionalFlagString = element.IsOptional ? "?" : string.Empty;
            string arrayFlagString = element.IsArray ? "[]" : string.Empty;

            Dictionary<string, string> values = element.NamesAndTypesForExport(
                FhirTypeBase.NamingConvention.CamelCase,
                FhirTypeBase.NamingConvention.PascalCase,
                false,
                string.Empty,
                complex.Components.ContainsKey(element.Path));

            if ((values.Count > 1) &&
                (!element.IsOptional) &&
                string.IsNullOrEmpty(optionalFlagString))
            {
                optionalFlagString = "?";
            }

            foreach (KeyValuePair<string, string> kvp in values)
            {
                if (!string.IsNullOrEmpty(element.Comment))
                {
                    WriteIndentedComment(element.Comment);
                }

                // Use generated enum for codes when required strength
                // EXCLUDE the MIME type value set - those should be bound to strings
                if (element.Codes != null
                        && element.Codes.Any()
                        && !string.IsNullOrEmpty(element.ValueSet)
                        && !string.IsNullOrEmpty(element.BindingStrength)
                        && string.Equals(element.BindingStrength, "required", StringComparison.Ordinal)
                        && (element.ValueSet != "http://www.rfc-editor.org/bcp/bcp13.txt")
                        && (!element.ValueSet.StartsWith("http://hl7.org/fhir/ValueSet/mimetypes", StringComparison.Ordinal)))
                {
                    if (_exportEnums)
                    {
                        // If we are building enum, reference
                        string codeName = FhirUtils.ToConvention(
                            $"{element.Path}.Codes",
                            string.Empty,
                            FhirTypeBase.NamingConvention.PascalCase);

                        _writer.WriteLineIndented($"{kvp.Key}{optionalFlagString}: {codeName}{arrayFlagString};");
                    }
                    else if (_info.TryGetValueSet(element.ValueSet, out FhirValueSet vs))
                    {
                        // use the full expansion
                        _writer.WriteLineIndented($"{kvp.Key}{optionalFlagString}: ({string.Join("|", vs.Concepts.Select(c => $"'{c.Code}'"))}){arrayFlagString};");
                    }
                    else
                    {
                        // otherwise, inline the required codes
                        _writer.WriteLineIndented($"{kvp.Key}{optionalFlagString}: ({string.Join("|", element.Codes.Select(c => $"'{c}'"))}){arrayFlagString};");
                    }
                }
                else if (kvp.Value.Equals("Resource", StringComparison.Ordinal))
                {
                    _writer.WriteLineIndented($"{kvp.Key}{optionalFlagString}: FhirResource{arrayFlagString};");
                }
                else
                {
                    _writer.WriteLineIndented($"{kvp.Key}{optionalFlagString}: {kvp.Value}{arrayFlagString};");
                }

                if (RequiresExtension(kvp.Value))
                {
                    _writer.WriteLineIndented($"_{kvp.Key}?: Element{arrayFlagString};");
                }
            }
        }

        /// <summary>Requires extension.</summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        private static bool RequiresExtension(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return false;
            }

            if (_primitiveTypeMap.ContainsKey(typeName))
            {
                return true;
            }

            return false;
        }

        /// <summary>Writes a header.</summary>
        private void WriteHeader()
        {
            _writer.WriteLineIndented("// <auto-generated/>");
            _writer.WriteLineIndented($"// Contents of: {_info.PackageName} version: {_info.VersionString}");
            _writer.WriteLineIndented($"  // Primitive Naming Style: {FhirTypeBase.NamingConvention.None}");
            _writer.WriteLineIndented($"  // Complex Type / Resource Naming Style: {FhirTypeBase.NamingConvention.PascalCase}");
            _writer.WriteLineIndented($"  // Interaction Naming Style: {FhirTypeBase.NamingConvention.None}");
            _writer.WriteLineIndented($"  // Extension Support: {_options.ExtensionSupport}");

            if ((_options.ExportList != null) && _options.ExportList.Any())
            {
                string restrictions = string.Join("|", _options.ExportList);
                _writer.WriteLineIndented($"  // Restricted to: {restrictions}");
            }

            if ((_options.LanguageOptions != null) && (_options.LanguageOptions.Count > 0))
            {
                foreach (KeyValuePair<string, string> kvp in _options.LanguageOptions)
                {
                    _writer.WriteLineIndented($"  // Language option: \"{kvp.Key}\" = \"{kvp.Value}\"");
                }
            }
        }

        /// <summary>Writes a footer.</summary>
        private void WriteFooter()
        {
            return;
        }

        /// <summary>Writes an indented comment.</summary>
        /// <param name="value">The value.</param>
        private void WriteIndentedComment(string value)
        {
            _writer.WriteLineIndented($"/**");

            string comment = value.Replace('\r', '\n').Replace("\r\n", "\n", StringComparison.Ordinal).Replace("\n\n", "\n", StringComparison.Ordinal);

            string[] lines = comment.Split('\n');
            foreach (string line in lines)
            {
                _writer.WriteIndented(" * ");
                _writer.WriteLine(line);
            }

            _writer.WriteLineIndented($" */");
        }

        /// <summary>Information about written codes.</summary>
        private struct WrittenCodeInfo
        {
            internal string Name;
            internal string ConstName;
        }
    }
}
