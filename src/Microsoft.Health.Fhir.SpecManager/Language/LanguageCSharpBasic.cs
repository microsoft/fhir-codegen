// <copyright file="LanguageCSharpBasic.cs" company="Microsoft Corporation">
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
    /// <summary>A language C# prototype.</summary>
    public sealed class LanguageCSharpBasic : ILanguage
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

            /// <summary>Names are often just symbols.</summary>
            "http://hl7.org/fhir/v2/0290",

            /// <summary>Display includes too many Unicode characters (invalid export names).</summary>
            "http://hl7.org/fhir/v2/0255",

            /// <summary>Display includes too many Unicode characters (invalid export names).</summary>
            "http://hl7.org/fhir/v2/0256",
        };

        /// <summary>The names requiring keyword new.</summary>
        private static HashSet<string> _namesRequiringKeywordNew = new HashSet<string>()
        {
            "Equals",
        };

        /// <summary>FHIR information we are exporting.</summary>
        private FhirVersionInfo _info;

        /// <summary>Options for controlling the export.</summary>
        private ExporterOptions _options;

        /// <summary>True to export enums.</summary>
        private bool _exportEnums;

        /// <summary>The namespace, default to 'fhir'.</summary>
        private string _namespace = "fhir";

        /// <summary>List of types of the exported resource names ands.</summary>
        private Dictionary<string, string> _exportedResourceNamesAndTypes = new Dictionary<string, string>();

        /// <summary>The exported codes.</summary>
        private HashSet<string> _exportedCodes = new HashSet<string>();

        /// <summary>The currently in-use text writer.</summary>
        private ExportStreamWriter _writer;

        /// <summary>Name of the language.</summary>
        private const string _languageName = "CSharpBasic";

        /// <summary>Dictionary mapping FHIR primitive types to language equivalents.</summary>
        private static readonly Dictionary<string, string> _primitiveTypeMap = new Dictionary<string, string>()
        {
            { "base", "Object" },
            { "base64Binary", "string" },
            { "boolean", "bool" },
            { "canonical", "string" },
            { "code", "string" },
            { "date", "string" },
            { "dateTime", "string" },           // Cannot use "DateTime" because of Partial Dates... may want to consider defining a new type, but not today
            { "decimal", "decimal" },
            { "id", "string" },
            { "instant", "string" },
            { "integer", "int" },
            { "integer64", "string" },          // int64 serializes as string, need to add custom handling here
            { "markdown", "string" },
            { "oid", "string" },
            { "positiveInt", "uint" },
            { "string", "string" },
            { "time", "string" },
            { "unsignedInt", "uint" },
            { "uri", "string" },
            { "url", "string" },
            { "uuid", "Guid" },
            { "xhtml", "string" },
        };

        /// <summary>Gets the reserved words.</summary>
        /// <value>The reserved words.</value>
        private static readonly HashSet<string> _reservedWords = new HashSet<string>()
        {
            "abstract",
            "as",
            "base",
            "bool",
            "break",
            "byte",
            "case",
            "catch",
            "char",
            "checked",
            "class",
            "const",
            "continue",
            "decimal",
            "default",
            "delegate",
            "do",
            "double",
            "else",
            "enum",
            "event",
            "explicit",
            "extern",
            "false",
            "finally",
            "fixed",
            "float",
            "for",
            "foreach",
            "goto",
            "if",
            "implicit",
            "in",
            "int",
            "interface",
            "internal",
            "is",
            "lock",
            "long",
            "namespace",
            "new",
            "null",
            "object",
            "operator",
            "out",
            "override",
            "params",
            "private",
            "protected",
            "public",
            "readonly",
            "ref",
            "return",
            "sbyte",
            "sealed",
            "short",
            "sizeof",
            "stackalloc",
            "static",
            "string",
            "struct",
            "switch",
            "this",
            "throw",
            "true",
            "try",
            "typeof",
            "uint",
            "ulong",
            "unchecked",
            "unsafe",
            "ushort",
            "using",
            "static",
            "virtual",
            "void",
            "volatile",
            "while",
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
        Dictionary<string, string> ILanguage.LanguageOptions => new Dictionary<string, string>()
        {
            { "namespace", "Namespace to use when exporting C# files (default: fhir)." },
        };

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

            if (options.OptionalClassTypesToExport.Contains(ExporterOptions.FhirExportClassType.Enum))
            {
                _exportEnums = true;
            }
            else
            {
                _exportEnums = false;
            }

            if ((options.LanguageOptions != null) && (options.LanguageOptions.Count > 0))
            {
                foreach (KeyValuePair<string, string> kvp in options.LanguageOptions)
                {
                    string key = kvp.Key.ToUpperInvariant();

                    switch (key)
                    {
                        case "NAMESPACE":
                            _namespace = kvp.Value;
                            break;
                    }
                }
            }

            _exportedResourceNamesAndTypes = new Dictionary<string, string>();
            _exportedCodes = new HashSet<string>();

            // create a filename for writing (single file for now)
            string filename = Path.Combine(exportDirectory, $"R{info.MajorVersion}.cs");

            using (FileStream stream = new FileStream(filename, FileMode.Create))
            using (ExportStreamWriter writer = new ExportStreamWriter(stream))
            {
                _writer = writer;

                WriteHeader();

                // open namespace
                _writer.WriteLineI($"namespace {_namespace}");
                _writer.WriteLineI("{");
                _writer.IncreaseIndent();

                WriteComplexes(_info.ComplexTypes.Values, false);
                WriteComplexes(_info.Resources.Values, true);

                if (_exportEnums)
                {
                    WriteValueSets(_info.ValueSetsByUrl.Values);
                }

                WritePolymorphicHelpers();

                // close namespace
                _writer.DecreaseIndent();
                _writer.WriteLineI("}");

                WriteFooter();
            }
        }

        /// <summary>Writes a polymorphic helpers.</summary>
        private void WritePolymorphicHelpers()
        {
            _writer.WriteLineI("public class ResourceConverter : JsonConverter");
            _writer.WriteLineI("{");
            _writer.IncreaseIndent();

            // function CanConvert
            _writer.WriteLineI("public override bool CanConvert(Type objectType)");
            _writer.WriteLineI("{");
            _writer.IncreaseIndent();
            _writer.WriteLineI("return typeof(Resource).IsAssignableFrom(objectType);");
            _writer.DecreaseIndent();
            _writer.WriteLineI("}");

            // property CanWrite
            _writer.WriteLineI("public override bool CanWrite");
            _writer.WriteLineI("{");
            _writer.IncreaseIndent();
            _writer.WriteLineI("get { return false; }");
            _writer.DecreaseIndent();
            _writer.WriteLineI("}");

            // function WriteJson
            _writer.WriteLineI("public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)");
            _writer.WriteLineI("{");
            _writer.IncreaseIndent();
            _writer.WriteLineI("throw new NotImplementedException();");
            _writer.DecreaseIndent();
            _writer.WriteLineI("}");

            // property CanRead
            _writer.WriteLineI("public override bool CanRead");
            _writer.WriteLineI("{");
            _writer.IncreaseIndent();
            _writer.WriteLineI("get { return true; }");
            _writer.DecreaseIndent();
            _writer.WriteLineI("}");

            // function ReadJson
            _writer.WriteLineI("public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)");
            _writer.WriteLineI("{");
            _writer.IncreaseIndent();
            _writer.WriteLineI("JObject jObject = JObject.Load(reader);");
            _writer.WriteLineI("string resourceType = jObject[\"resourceType\"].Value<string>();");
            _writer.WriteLineI("object target = null;");
            _writer.WriteLineI("switch (resourceType)");
            _writer.WriteLineI("{");
            _writer.IncreaseIndent();

            // loop through our types
            foreach (KeyValuePair<string, string> kvp in _exportedResourceNamesAndTypes)
            {
                _writer.WriteLineI($"case \"{kvp.Key}\":");
                _writer.IncreaseIndent();
                _writer.WriteLineI($"target = new {kvp.Value}();");
                _writer.WriteLineI("break;");
                _writer.DecreaseIndent();
            }

            // default case returns a Resource object
            _writer.WriteLineI("default:");
            _writer.IncreaseIndent();
            _writer.WriteLineI("target = new Resource();");
            _writer.WriteLineI("break;");
            _writer.DecreaseIndent();

            // close switch
            _writer.DecreaseIndent();
            _writer.WriteLineI("}");

            // populate
            _writer.WriteLineI("serializer.Populate(jObject.CreateReader(), target);");

            // return/close ReadJson
            _writer.WriteLineI("return target;");
            _writer.DecreaseIndent();
            _writer.WriteLineI("}");

            // close class
            _writer.DecreaseIndent();
            _writer.WriteLineI("}");
        }

        /// <summary>Writes a value sets.</summary>
        /// <param name="valueSets">List of valueSetCollections.</param>
        private void WriteValueSets(
            IEnumerable<FhirValueSetCollection> valueSets)
        {
            foreach (FhirValueSetCollection collection in valueSets.OrderBy(c => c.URL))
            {
                foreach (FhirValueSet vs in collection.ValueSetsByVersion.Values.OrderBy(v => v.Version))
                {
                    WriteValueSet(
                        vs);
                }
            }
        }

        /// <summary>Writes a value set.</summary>
        /// <param name="vs">         The value set.</param>
        /// <param name="indentation">The indentation.</param>
        private void WriteValueSet(
            FhirValueSet vs)
        {
            string vsName = FhirUtils.SanitizeForProperty(vs.Id ?? vs.Name, _reservedWords);

            vsName = FhirUtils.SanitizedToConvention(vsName, FhirTypeBase.NamingConvention.PascalCase);

            if (!string.IsNullOrEmpty(vs.Description))
            {
                WriteIndentedComment(vs.Description);
            }
            else
            {
                WriteIndentedComment($"Value Set: {vs.URL}|{vs.Version}");
            }

            if (vsName.EndsWith("ValueSet", StringComparison.Ordinal))
            {
                _writer.WriteLineI($"public static class {vsName}");
            }
            else
            {
                _writer.WriteLineI($"public static class {vsName}ValueSet");
            }

            _writer.WriteLineI("{");
            _writer.IncreaseIndent();

            bool prefixWithSystem = vs.ReferencedCodeSystems.Count > 1;
            HashSet<string> usedValues = new HashSet<string>();

            foreach (FhirConcept concept in vs.Concepts.OrderBy(c => c.Code))
            {
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

                string name;

                if (prefixWithSystem)
                {
                    name = $"{codeName}_{concept.SystemLocalName}";
                }
                else
                {
                    name = codeName;
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

                if (_namesRequiringKeywordNew.Contains(name))
                {
                    _writer.WriteLineI($"public static readonly new Coding {name} = new Coding");
                }
                else
                {
                    _writer.WriteLineI($"public static readonly Coding {name} = new Coding");
                }

                _writer.WriteLineI("{");
                _writer.IncreaseIndent();

                _writer.WriteLineI($"Code = \"{codeValue}\",");

                if (!string.IsNullOrEmpty(concept.Display))
                {
                    _writer.WriteLineI($"Display = \"{FhirUtils.SanitizeForQuoted(concept.Display)}\",");
                }

                _writer.WriteLineI($"System = \"{concept.System}\"");

                _writer.DecreaseIndent();
                _writer.WriteLineI("};");
            }

            _writer.DecreaseIndent();
            _writer.WriteLineI("};");
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

            if (string.IsNullOrEmpty(complex.BaseTypeName) ||
                complex.Name.Equals("Element", StringComparison.Ordinal))
            {
                _writer.WriteLineI($"public class {complex.NameForExport(FhirTypeBase.NamingConvention.PascalCase)} {{");
            }
            else if (complex.Name.Equals(complex.BaseTypeName, StringComparison.Ordinal))
            {
                _writer.WriteLineI(
                    $"public class" +
                        $" {complex.NameForExport(FhirTypeBase.NamingConvention.PascalCase, true)}" +
                        $" {{");
            }
            else if ((complex.Components != null) && complex.Components.ContainsKey(complex.Path))
            {
                _writer.WriteLineI(
                    $"public class" +
                        $" {complex.NameForExport(FhirTypeBase.NamingConvention.PascalCase, true)}" +
                        $" :" +
                        $" {complex.TypeForExport(FhirTypeBase.NamingConvention.PascalCase, _primitiveTypeMap, false)} {{");
            }
            else
            {
                _writer.WriteLineI(
                    $"public class" +
                        $" {complex.NameForExport(FhirTypeBase.NamingConvention.PascalCase, true)}" +
                        $" :" +
                        $" {complex.TypeForExport(FhirTypeBase.NamingConvention.PascalCase, _primitiveTypeMap)} {{");
            }

            _writer.IncreaseIndent();

            if (isResource && ShouldWriteResourceName(complex.Name))
            {
                _exportedResourceNamesAndTypes.Add(complex.Name, complex.Name);

                _writer.WriteLineI("/** Resource Type Name (for serialization) */");
                _writer.WriteLineI("[JsonProperty(\"resourceType\")]");
                _writer.WriteLineI($"public string ResourceType => \"{complex.Name}\";");
            }

            // write elements
            WriteElements(complex, out List<FhirElement> elementsWithCodes);

            // close interface (type)
            _writer.DecreaseIndent();
            _writer.WriteLineI("}");

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

            if (codeName.Contains("[X]"))
            {
                codeName = codeName.Replace("[X]", string.Empty);
            }

            if (_exportedCodes.Contains(codeName))
            {
                return;
            }

            _exportedCodes.Add(codeName);

            _writer.WriteLineI($"/// <summary>");
            _writer.WriteLineI($"/// Code Values for the {element.Path} field");
            _writer.WriteLineI($"/// </summary>");

            if (codeName.EndsWith("Codes", StringComparison.Ordinal))
            {
                _writer.WriteLineI($"public static class {codeName} {{");
            }
            else
            {
                _writer.WriteLineI($"public static class {codeName}Codes {{");
            }

            _writer.IncreaseIndent();

            foreach (string code in element.Codes)
            {
                FhirUtils.SanitizeForCode(code, _reservedWords, out string name, out string value);

                _writer.WriteLineI($"public const string {name.ToUpperInvariant()} = \"{value}\";");
            }

            _writer.DecreaseIndent();
            _writer.WriteLineI("}");
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
                if (_options.UseModelInheritance && element.IsInherited)
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
            string arrayFlagString = element.IsArray ? "[]" : string.Empty;

            Dictionary<string, string> values = element.NamesAndTypesForExport(
                FhirTypeBase.NamingConvention.PascalCase,
                FhirTypeBase.NamingConvention.PascalCase,
                false,
                string.Empty,
                complex.Components.ContainsKey(element.Path));

            foreach (KeyValuePair<string, string> kvp in values)
            {
                string elementName;
                if (kvp.Key == complex.Name)
                {
                    elementName = $"{kvp.Key}Field";
                }
                else
                {
                    elementName = kvp.Key;
                }

                string optionalFlagString = (element.IsOptional && IsNullable(kvp.Value)) ? "?" : string.Empty;

                if (!string.IsNullOrEmpty(element.Comment))
                {
                    WriteIndentedComment(element.Comment);
                }

                string camel = FhirUtils.ToConvention(kvp.Key, string.Empty, FhirTypeBase.NamingConvention.CamelCase);

                _writer.WriteLineI($"[JsonProperty(\"{camel}\")]");

                _writer.WriteLineI($"public {kvp.Value}{optionalFlagString}{arrayFlagString} {elementName} {{ get; set; }}");

                if (RequiresExtension(kvp.Value))
                {
                    _writer.WriteLineI($"[JsonProperty(\"_{camel}\")]");
                    _writer.WriteLineI($"public Element{arrayFlagString} _{elementName} {{ get; set; }}");
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

        /// <summary>Query if 'typeName' is nullable.</summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>True if nullable, false if not.</returns>
        private static bool IsNullable(string typeName)
        {
            // nullable reference types are not allowed in current C#
            switch (typeName)
            {
                case "bool":
                case "decimal":
                case "DateTime":
                case "int":
                case "uint":
                case "Guid":
                    return true;
            }

            return false;
        }

        /// <summary>Writes a header.</summary>
        private void WriteHeader()
        {
            _writer.WriteLineI("// <auto-generated/>");
            _writer.WriteLineI($"// Contents of: {_info.PackageName} version: {_info.VersionString}");
            _writer.WriteLineI($"  // Using Model Inheritance: {_options.UseModelInheritance}");
            _writer.WriteLineI($"  // Hiding Removed Parent Fields: {_options.HideRemovedParentFields}");
            _writer.WriteLineI($"  // Nesting Type Definitions: {_options.NestTypeDefinitions}");
            _writer.WriteLineI($"  // Primitive Naming Style: {FhirTypeBase.NamingConvention.None}");
            _writer.WriteLineI($"  // Complex Type / Resource Naming Style: {FhirTypeBase.NamingConvention.PascalCase}");
            _writer.WriteLineI($"  // Element Naming Style: {FhirTypeBase.NamingConvention.PascalCase}");
            _writer.WriteLineI($"  // Enum Naming Style: {FhirTypeBase.NamingConvention.PascalCase}");
            _writer.WriteLineI($"  // Interaction Naming Style: {FhirTypeBase.NamingConvention.None}");
            _writer.WriteLineI($"  // Extension Support: {_options.ExtensionSupport}");

            if ((_options.ExportList != null) && _options.ExportList.Any())
            {
                string restrictions = string.Join("|", _options.ExportList);
                _writer.WriteLineI($"  // Restricted to: {restrictions}");
            }

            if ((_options.LanguageOptions != null) && (_options.LanguageOptions.Count > 0))
            {
                foreach (KeyValuePair<string, string> kvp in _options.LanguageOptions)
                {
                    _writer.WriteLineI($"  // Language option: \"{kvp.Key}\" = \"{kvp.Value}\"");
                }
            }

            _writer.WriteLine(string.Empty);

            _writer.WriteLineI("using System;");
            _writer.WriteLineI("using System.Collections.Generic;");
            _writer.WriteLineI("using Newtonsoft.Json;");
            _writer.WriteLineI("using Newtonsoft.Json.Linq;");
            _writer.WriteLine(string.Empty);
        }

        /// <summary>Writes a footer.</summary>
        private void WriteFooter()
        {
            WriteIndentedComment("end of file", false);
        }

        /// <summary>Writes an indented comment.</summary>
        /// <param name="value">    The value.</param>
        /// <param name="isSummary">(Optional) True if is summary, false if not.</param>
        private void WriteIndentedComment(string value, bool isSummary = true)
        {
            if (isSummary)
            {
                _writer.WriteLineI("/// <summary>");
            }

            string comment = value.Replace('\r', '\n').Replace("\r\n", "\n").Replace("\n\n", "\n");

            string[] lines = comment.Split('\n');
            foreach (string line in lines)
            {
                _writer.WriteI("/// ");
                _writer.WriteLine(line);
            }

            if (isSummary)
            {
                _writer.WriteLineI("/// </summary>");
            }
        }
    }
}
