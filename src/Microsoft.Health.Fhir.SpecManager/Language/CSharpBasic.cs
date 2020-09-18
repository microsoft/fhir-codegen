// <copyright file="CSharpBasic.cs" company="Microsoft Corporation">
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
    /// <summary>A basic C# language - useful for testing, simple projects, and prototypes.</summary>
    public sealed class CSharpBasic : ILanguage
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

        /// <summary>True to use system JSON.</summary>
        private bool _jsonSystemText = true;

        /// <summary>True to JSON newton soft.</summary>
        private bool _jsonNewtonSoft = false;

        /// <summary>True to include, false to exclude the summaries.</summary>
        private bool _includeSummaries = true;

        /// <summary>The access modifier.</summary>
        private string _accessModifier = "public";

        /// <summary>List of types of the exported resource names and types.</summary>
        private Dictionary<string, string> _exportedResourceNamesAndTypes = new Dictionary<string, string>();

        /// <summary>The exported codes.</summary>
        private HashSet<string> _exportedCodes = new HashSet<string>();

        /// <summary>The currently in-use text writer.</summary>
        private ExportStreamWriter _writer;

        /// <summary>Name of the language.</summary>
        private const string _languageName = "CSharpBasic";

        /// <summary>The single file export extension.</summary>
        private const string _singleFileExportExtension = ".cs";

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
        Dictionary<string, string> ILanguage.LanguageOptions => new Dictionary<string, string>()
        {
            { "namespace", "Namespace to use when exporting C# files (default: fhir)." },
            { "json-library", "Which JSON library to use (system, newtonsoft)." },
            { "include-summaries", "If export should include Summary XML comments (true|false)." },
            { "access-modifier", "Access modifier for exported elements (public|internal|private)." },
        };

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

            if (options.GetParam("json-library", "system").Equals("system", StringComparison.OrdinalIgnoreCase))
            {
                _jsonSystemText = true;
                _jsonNewtonSoft = false;
            }
            else
            {
                _jsonSystemText = false;
                _jsonNewtonSoft = true;
            }

            if (options.OptionalClassTypesToExport.Contains(ExporterOptions.FhirExportClassType.Enum))
            {
                _exportEnums = true;
            }
            else
            {
                _exportEnums = false;
            }

            _namespace = options.GetParam("namespace", "fhir");
            _includeSummaries = options.GetParam("include-summaries", true);
            _accessModifier = options.GetParam("access-modifier", "public");

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
                _writer.WriteLineIndented($"namespace {_namespace}");
                _writer.WriteLineIndented("{");
                _writer.IncreaseIndent();

                WriteComplexes(_info.ComplexTypes.Values, false);
                WriteComplexes(_info.Resources.Values, true);

                if (_exportEnums)
                {
                    WriteValueSets(_info.ValueSetsByUrl.Values);
                }

                if (_jsonSystemText)
                {
                    WritePolymorphicHelpersSystemText();
                }
                else
                {
                    WritePolymorphicHelpersNewtonsoft();
                }

                // close namespace
                _writer.DecreaseIndent();
                _writer.WriteLineIndented("}");

                WriteFooter();
            }
        }

        /// <summary>Writes the polymorphic helpers for System.Text.Json.</summary>
        private void WritePolymorphicHelpersSystemText()
        {
            // open class
            _writer.WriteLineIndented($"{_accessModifier} class ResourceConverter : JsonConverter<Resource>");
            _writer.WriteLineIndented("{");
            _writer.IncreaseIndent();

            // function CanConvert
            _writer.WriteLineIndented($"{_accessModifier} override bool CanConvert(Type objectType) =>");
            _writer.IncreaseIndent();
            _writer.WriteLineIndented("typeof(Resource).IsAssignableFrom(objectType);");
            _writer.DecreaseIndent();
            _writer.WriteLine();

            _writer.WriteLineIndented($"{_accessModifier} override void Write(" +
                "Utf8JsonWriter writer, " +
                "Resource resource, " +
                "JsonSerializerOptions options)");

            // open Write
            _writer.WriteLineIndented("{");
            _writer.IncreaseIndent();

            // close Write
            _writer.DecreaseIndent();
            _writer.WriteLineIndented("}");

            _writer.WriteLineIndented($"{_accessModifier} override Resource Read(" +
                "ref Utf8JsonReader reader, " +
                "Type typeToConvert, " +
                "JsonSerializerOptions options)");

            // open Read
            _writer.WriteLineIndented("{");
            _writer.IncreaseIndent();

            _writer.WriteLineIndented("return PolymorphicRead(ref reader, typeToConvert, options);");

            // close Read
            _writer.DecreaseIndent();
            _writer.WriteLineIndented("}");

            _writer.WriteLineIndented($"{_accessModifier} static Resource PolymorphicRead(" +
                "ref Utf8JsonReader reader, " +
                "Type typeToConvert, " +
                "JsonSerializerOptions options)");

            // open Read
            _writer.WriteLineIndented("{");
            _writer.IncreaseIndent();

            _writer.WriteLineIndented("if (reader.TokenType != JsonTokenType.StartObject)");
            _writer.WriteLineIndented("{");
            _writer.IncreaseIndent();
            _writer.WriteLineIndented("throw new JsonException();");
            _writer.DecreaseIndent();
            _writer.WriteLineIndented("}");
            _writer.WriteLine();

            _writer.WriteLineIndented("reader.Read();");
            _writer.WriteLineIndented("if (reader.TokenType != JsonTokenType.PropertyName)");
            _writer.WriteLineIndented("{");
            _writer.IncreaseIndent();
            _writer.WriteLineIndented("throw new JsonException();");
            _writer.DecreaseIndent();
            _writer.WriteLineIndented("}");
            _writer.WriteLine();

            _writer.WriteLineIndented("string propertyName = reader.GetString();");
            _writer.WriteLineIndented("if (propertyName != \"resourceType\")");
            _writer.WriteLineIndented("{");
            _writer.IncreaseIndent();
            _writer.WriteLineIndented("throw new JsonException();");
            _writer.DecreaseIndent();
            _writer.WriteLineIndented("}");
            _writer.WriteLine();

            _writer.WriteLineIndented("reader.Read();");
            _writer.WriteLineIndented("if (reader.TokenType != JsonTokenType.String)");
            _writer.WriteLineIndented("{");
            _writer.IncreaseIndent();
            _writer.WriteLineIndented("throw new JsonException();");
            _writer.DecreaseIndent();

            _writer.WriteLineIndented("}");
            _writer.WriteLine();

            _writer.WriteLineIndented("string resourceType = reader.GetString();");
            _writer.WriteLineIndented("object target = null;");

            _writer.WriteLineIndented("switch (resourceType)");

            // open switch
            _writer.WriteLineIndented("{");
            _writer.IncreaseIndent();

            // loop through our types
            foreach (KeyValuePair<string, string> kvp in _exportedResourceNamesAndTypes)
            {
                _writer.WriteLineIndented($"case \"{kvp.Key}\":");
                _writer.IncreaseIndent();
                _writer.WriteLineIndented($"{kvp.Value}.ParseJson(ref reader, options, ref target);");
                _writer.WriteLineIndented("break;");
                _writer.DecreaseIndent();
            }

            // default case returns a Resource object
            _writer.WriteLineIndented("default:");
            _writer.IncreaseIndent();
            _writer.WriteLineIndented("Resource.ParseJson(ref reader, options, ref target);");
            _writer.WriteLineIndented("break;");
            _writer.DecreaseIndent();

            // close switch
            _writer.DecreaseIndent();
            _writer.WriteLineIndented("}");
            _writer.WriteLine();

            _writer.WriteLineIndented("return (Resource)target;");

            // close Read
            _writer.DecreaseIndent();
            _writer.WriteLineIndented("}");

            // close class
            _writer.DecreaseIndent();
            _writer.WriteLineIndented("}");
        }

        /// <summary>Writes a polymorphic helpers for NewtonSoft.Json.</summary>
        private void WritePolymorphicHelpersNewtonsoft()
        {
            _writer.WriteLineIndented($"{_accessModifier} class ResourceConverter : JsonConverter");
            _writer.WriteLineIndented("{");
            _writer.IncreaseIndent();

            // function CanConvert
            _writer.WriteLineIndented($"{_accessModifier} override bool CanConvert(Type objectType)");
            _writer.WriteLineIndented("{");
            _writer.IncreaseIndent();
            _writer.WriteLineIndented("return typeof(Resource).IsAssignableFrom(objectType);");
            _writer.DecreaseIndent();
            _writer.WriteLineIndented("}");

            // property CanWrite
            _writer.WriteLineIndented($"{_accessModifier} override bool CanWrite");
            _writer.WriteLineIndented("{");
            _writer.IncreaseIndent();
            _writer.WriteLineIndented("get { return false; }");
            _writer.DecreaseIndent();
            _writer.WriteLineIndented("}");

            // function WriteJson
            _writer.WriteLineIndented($"{_accessModifier} override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)");
            _writer.WriteLineIndented("{");
            _writer.IncreaseIndent();
            _writer.WriteLineIndented("throw new NotImplementedException();");
            _writer.DecreaseIndent();
            _writer.WriteLineIndented("}");

            // property CanRead
            _writer.WriteLineIndented($"{_accessModifier} override bool CanRead");
            _writer.WriteLineIndented("{");
            _writer.IncreaseIndent();
            _writer.WriteLineIndented("get { return true; }");
            _writer.DecreaseIndent();
            _writer.WriteLineIndented("}");

            // function ReadJson
            _writer.WriteLineIndented($"{_accessModifier} override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)");
            _writer.WriteLineIndented("{");
            _writer.IncreaseIndent();
            _writer.WriteLineIndented("JObject jObject = JObject.Load(reader);");
            _writer.WriteLineIndented("string resourceType = jObject[\"resourceType\"].Value<string>();");
            _writer.WriteLineIndented("object target = null;");
            _writer.WriteLineIndented("switch (resourceType)");
            _writer.WriteLineIndented("{");
            _writer.IncreaseIndent();

            // loop through our types
            foreach (KeyValuePair<string, string> kvp in _exportedResourceNamesAndTypes)
            {
                _writer.WriteLineIndented($"case \"{kvp.Key}\":");
                _writer.IncreaseIndent();
                _writer.WriteLineIndented($"target = new {kvp.Value}();");
                _writer.WriteLineIndented("break;");
                _writer.DecreaseIndent();
            }

            // default case returns a Resource object
            _writer.WriteLineIndented("default:");
            _writer.IncreaseIndent();
            _writer.WriteLineIndented("target = new Resource();");
            _writer.WriteLineIndented("break;");
            _writer.DecreaseIndent();

            // close switch
            _writer.DecreaseIndent();
            _writer.WriteLineIndented("}");

            // populate
            _writer.WriteLineIndented("serializer.Populate(jObject.CreateReader(), target);");

            // return/close ReadJson
            _writer.WriteLineIndented("return target;");
            _writer.DecreaseIndent();
            _writer.WriteLineIndented("}");

            // close class
            _writer.DecreaseIndent();
            _writer.WriteLineIndented("}");
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
        /// <param name="vs">The value set.</param>
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
                _writer.WriteLineIndented($"{_accessModifier} static class {vsName}");
            }
            else
            {
                _writer.WriteLineIndented($"{_accessModifier} static class {vsName}ValueSet");
            }

            _writer.WriteLineIndented("{");
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
                    _writer.WriteLineIndented($"{_accessModifier} static readonly new Coding {name} = new Coding");
                }
                else
                {
                    _writer.WriteLineIndented($"{_accessModifier} static readonly Coding {name} = new Coding");
                }

                _writer.WriteLineIndented("{");
                _writer.IncreaseIndent();

                _writer.WriteLineIndented($"Code = \"{codeValue}\",");

                if (!string.IsNullOrEmpty(concept.Display))
                {
                    _writer.WriteLineIndented($"Display = \"{FhirUtils.SanitizeForQuoted(concept.Display)}\",");
                }

                _writer.WriteLineIndented($"System = \"{concept.System}\"");

                _writer.DecreaseIndent();
                _writer.WriteLineIndented("};");
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

            string nameForExport;
            bool hasBaseClass;
            string baseClassName;

            if (string.IsNullOrEmpty(complex.BaseTypeName) ||
                complex.Name.Equals("Element", StringComparison.Ordinal))
            {
                nameForExport = complex.NameForExport(FhirTypeBase.NamingConvention.PascalCase);
                hasBaseClass = false;
                baseClassName = string.Empty;

                _writer.WriteLineIndented($"{_accessModifier} class {nameForExport} {{");
            }
            else if (complex.Name.Equals(complex.BaseTypeName, StringComparison.Ordinal))
            {
                nameForExport = complex.NameForExport(FhirTypeBase.NamingConvention.PascalCase, true);
                hasBaseClass = false;
                baseClassName = string.Empty;

                _writer.WriteLineIndented(
                    $"{_accessModifier} class" +
                        $" {nameForExport}" +
                        $" {{");
            }
            else if ((complex.Components != null) && complex.Components.ContainsKey(complex.Path))
            {
                nameForExport = complex.NameForExport(FhirTypeBase.NamingConvention.PascalCase, true);
                hasBaseClass = true;
                baseClassName = complex.TypeForExport(FhirTypeBase.NamingConvention.PascalCase, _primitiveTypeMap, false);

                _writer.WriteLineIndented(
                    $"{_accessModifier} class" +
                        $" {nameForExport}" +
                        $" :" +
                        $" {baseClassName} {{");
            }
            else
            {
                nameForExport = complex.NameForExport(FhirTypeBase.NamingConvention.PascalCase, true);
                hasBaseClass = true;
                baseClassName = complex.TypeForExport(FhirTypeBase.NamingConvention.PascalCase, _primitiveTypeMap);

                _writer.WriteLineIndented(
                    $"{_accessModifier} class" +
                        $" {nameForExport}" +
                        $" :" +
                        $" {baseClassName} {{");
            }

            _writer.IncreaseIndent();

            if (isResource && ShouldWriteResourceName(complex.Name))
            {
                _exportedResourceNamesAndTypes.Add(complex.Name, complex.Name);

                WriteIndentedComment("Resource Type Name");

                if (_jsonNewtonSoft)
                {
                    _writer.WriteLineIndented("[JsonProperty(\"resourceType\")]");
                }

                _writer.WriteLineIndented($"{_accessModifier} string ResourceType => \"{complex.Name}\";");
            }

            // write elements
            WriteElements(complex, out List<FhirElement> elementsWithCodes);

            if (_jsonSystemText)
            {
                string keywordNew = hasBaseClass ? "new " : string.Empty;

                _writer.WriteLineIndented($"{_accessModifier} static {keywordNew}void ParseJsonProperty(" +
                    "ref Utf8JsonReader reader, " +
                    "JsonSerializerOptions options, " +
                    "ref object obj, " +
                    "string propertyName)");

                // open ParseJsonProperty
                _writer.WriteLineIndented("{");
                _writer.IncreaseIndent();

                _writer.WriteLineIndented("switch (propertyName)");

                // open switch
                _writer.WriteLineIndented("{");
                _writer.IncreaseIndent();

                WriteParseJsonPropertyElements(complex, nameForExport);

                if (hasBaseClass)
                {
                    _writer.WriteLineIndented($"default:");
                    _writer.IncreaseIndent();
                    _writer.WriteLineIndented($"{_namespace}.{baseClassName}.ParseJsonProperty(ref reader, options, ref obj, propertyName);");
                    _writer.WriteLineIndented("break;");
                    _writer.DecreaseIndent();
                }

                // close switch
                _writer.DecreaseIndent();
                _writer.WriteLineIndented("}");

                // close ParseJsonProperty
                _writer.DecreaseIndent();
                _writer.WriteLineIndented("}");
                _writer.WriteLine();

                _writer.WriteLineIndented($"{_accessModifier} static {keywordNew}void ParseJson(" +
                    "ref Utf8JsonReader reader, " +
                    "JsonSerializerOptions options, " +
                    "ref object obj)");

                // open ParseJson
                _writer.WriteLineIndented("{");
                _writer.IncreaseIndent();

                _writer.WriteLineIndented("if (obj == null)");
                _writer.WriteLineIndented("{");
                _writer.IncreaseIndent();
                _writer.WriteLineIndented($"obj = new {nameForExport}();");
                _writer.DecreaseIndent();
                _writer.WriteLineIndented("}");
                _writer.WriteLine();

                _writer.WriteLineIndented("string propertyName;");
                _writer.WriteLine();
                _writer.WriteLineIndented("while (reader.Read())");
                _writer.WriteLineIndented("{");
                _writer.IncreaseIndent();
                _writer.WriteLineIndented("if (reader.TokenType == JsonTokenType.EndObject)");
                _writer.WriteLineIndented("{");
                _writer.IncreaseIndent();
                _writer.WriteLineIndented("return;");
                _writer.DecreaseIndent();
                _writer.WriteLineIndented("}");
                _writer.WriteLine();
                _writer.WriteLineIndented("if (reader.TokenType == JsonTokenType.PropertyName)");
                _writer.WriteLineIndented("{");
                _writer.IncreaseIndent();
                _writer.WriteLineIndented("propertyName = reader.GetString();");

                // TODO: this is for debugging ONLY!
                // _writer.WriteLineIndented($"Console.WriteLine($\"Reading {nameForExport}.{{propertyName}}\");");
                _writer.WriteLineIndented("reader.Read();");
                _writer.WriteLineIndented("ParseJsonProperty(ref reader, options, ref obj, propertyName);");
                _writer.DecreaseIndent();
                _writer.WriteLineIndented("}");
                _writer.DecreaseIndent();
                _writer.WriteLineIndented("}");
                _writer.WriteLine();

                _writer.WriteLineIndented("throw new JsonException();");

                // close ParseJson
                _writer.DecreaseIndent();
                _writer.WriteLineIndented("}");
            }

            // close interface (type)
            _writer.DecreaseIndent();
            _writer.WriteLineIndented("}");

            if (_exportEnums)
            {
                foreach (FhirElement element in elementsWithCodes)
                {
                    WriteCode(element);
                }
            }
        }

        /// <summary>Writes a parse JSON property elements.</summary>
        /// <param name="complex">The complex.</param>
        private void WriteParseJsonPropertyElements(FhirComplex complex, string className)
        {
            foreach (FhirElement element in complex.Elements.Values.OrderBy(s => s.Name))
            {
                if (element.IsInherited)
                {
                    continue;
                }

                Dictionary<string, string> values = element.NamesAndTypesForExport(
                    FhirTypeBase.NamingConvention.PascalCase,
                    FhirTypeBase.NamingConvention.PascalCase,
                    false,
                    string.Empty,
                    complex.Components.ContainsKey(element.Path));

                foreach (KeyValuePair<string, string> kvp in values)
                {
                    string elementName;
                    if ((kvp.Key == complex.Name) && (!element.IsInherited))
                    {
                        elementName = $"{kvp.Key}Field";
                    }
                    else
                    {
                        elementName = kvp.Key;
                    }

                    string camel = FhirUtils.ToConvention(kvp.Key, string.Empty, FhirTypeBase.NamingConvention.CamelCase);

                    string elementType = element.IsArray
                        ? $"List<{kvp.Value}>"
                        : $"{kvp.Value}";

                    switch (elementType)
                    {
                        case "bool":
                            WriteJsonPropertyCaseSimple(className, elementName, camel, "GetBoolean");
                            break;
                        case "List<bool>":
                            WriteJsonPropertyListCaseSimple(className, elementName, camel, kvp.Value, "GetBoolean");
                            break;
                        case "decimal":
                            WriteJsonPropertyCaseSimple(className, elementName, camel, "GetDecimal");
                            break;
                        case "List<decimal>":
                            WriteJsonPropertyListCaseSimple(className, elementName, camel, kvp.Value, "GetDecimal");
                            break;
                        case "Guid":
                            WriteJsonPropertyCaseSimple(className, elementName, camel, "GetGuid");
                            break;
                        case "List<Guid>":
                            WriteJsonPropertyListCaseSimple(className, elementName, camel, kvp.Value, "GetGuid");
                            break;
                        case "int":
                            WriteJsonPropertyCaseSimple(className, elementName, camel, "GetInt32");
                            break;
                        case "List<int>":
                            WriteJsonPropertyListCaseSimple(className, elementName, camel, kvp.Value, "GetInt32");
                            break;
                        case "string":
                            WriteJsonPropertyCaseSimple(className, elementName, camel, "GetString");
                            break;
                        case "List<string>":
                            WriteJsonPropertyListCaseSimple(className, elementName, camel, kvp.Value, "GetString");
                            break;
                        case "uint":
                            WriteJsonPropertyCaseSimple(className, elementName, camel, "GetUInt32");
                            break;
                        case "List<uint>":
                            WriteJsonPropertyListCaseSimple(className, elementName, camel, kvp.Value, "GetUInt32");
                            break;
                        default:
                            if (element.IsArray)
                            {
                                WriteJsonPropertyListCaseComplex(className, elementName, camel, kvp.Value);
                            }
                            else
                            {
                                WriteJsonPropertyCaseComplex(className, elementName, camel, elementType);
                            }

                            break;
                    }

                    if (RequiresExtension(kvp.Value))
                    {
                        if (element.IsArray)
                        {
                            WriteJsonPropertyListCaseComplex(className, "_" + elementName, "_" + camel, "Element");
                        }
                        else
                        {
                            WriteJsonPropertyCaseComplex(className, "_" + elementName, "_" + camel, "Element");

                            //_writer.WriteLineIndented($"case \"_{camel}\":");
                            //_writer.IncreaseIndent();
                            //// _writer.WriteLineIndented($"{_namespace}.Element.ParseJsonProperty(reader, options, ref obj, propertyName);");

                            //_writer.WriteLineIndented($"object obj{elementName}Ext = null;");
                            //_writer.WriteLineIndented($"{_namespace}.Element.ParseJson(ref reader, options, ref obj{elementName}Ext);");
                            //_writer.WriteLineIndented($"(({className})obj)._{elementName} = ((Element)obj{elementName}Ext);");

                            //_writer.WriteLineIndented("break;");
                            //_writer.DecreaseIndent();
                        }
                    }
                }
            }
        }

        /// <summary>Writes a JSON property case complex.</summary>
        /// <param name="className">  Name of the class.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="camel">      The camel.</param>
        /// <param name="elementType">Type of the element.</param>
        private void WriteJsonPropertyCaseComplex(
            string className,
            string elementName,
            string camel,
            string elementType)
        {
            _writer.WriteLineIndented($"case \"{camel}\":");
            _writer.IncreaseIndent();

            // _writer.WriteLineIndented($"{_namespace}.{elementType}.ParseJsonProperty(reader, options, ref obj, propertyName);");

            if (elementType == "Resource")
            {
                _writer.WriteLineIndented($"{_namespace}.{elementType} res{elementName} = " +
                    $"{_namespace}.ResourceConverter.PolymorphicRead(ref reader, typeof(Resource), options);");
                _writer.WriteLineIndented($"(({className})obj).{elementName} = ({_namespace}.{elementType})res{elementName};");
            }
            else
            {
                _writer.WriteLineIndented($"object obj{elementName} = null;");
                _writer.WriteLineIndented($"{_namespace}.{elementType}.ParseJson(ref reader, options, ref obj{elementName});");
                _writer.WriteLineIndented($"(({className})obj).{elementName} = ({_namespace}.{elementType})obj{elementName};");
            }

            _writer.WriteLineIndented("break;");
            _writer.DecreaseIndent();
        }

        /// <summary>Writes a JSON property case simple.</summary>
        /// <param name="elementName">       Name of the element.</param>
        /// <param name="camel">             The camel.</param>
        /// <param name="getterFunctionName">Name of the getter function.</param>
        private void WriteJsonPropertyCaseSimple(
            string className,
            string elementName,
            string camel,
            string getterFunctionName)
        {
            _writer.WriteLineIndented($"case \"{camel}\":");
            _writer.IncreaseIndent();
            _writer.WriteLineIndented($"(({className})obj).{elementName} = reader.{getterFunctionName}();");
            _writer.WriteLineIndented("break;");
            _writer.DecreaseIndent();
        }

        /// <summary>Writes a JSON property list case complex.</summary>
        /// <exception cref="JsonException">Thrown when a JSON error condition occurs.</exception>
        /// <param name="className">  Name of the class.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="camel">      The camel.</param>
        /// <param name="elementType">Type of the element.</param>
        private void WriteJsonPropertyListCaseComplex(
            string className,
            string elementName,
            string camel,
            string elementType)
        {
            _writer.WriteLineIndented($"case \"{camel}\":");
            _writer.IncreaseIndent();
            _writer.WriteLineIndented($"if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))");
            _writer.WriteLineIndented("{");
            _writer.IncreaseIndent();
            _writer.WriteLineIndented("throw new JsonException();");
            _writer.DecreaseIndent();
            _writer.WriteLineIndented("}");
            _writer.WriteLine();
            _writer.WriteLineIndented($"(({className})obj).{elementName} = new List<{elementType}>();");
            _writer.WriteLine();
            _writer.WriteLineIndented($"while (reader.TokenType != JsonTokenType.EndArray)");
            _writer.WriteLineIndented("{");
            _writer.IncreaseIndent();

            if (elementType == "Resource")
            {
                _writer.WriteLineIndented($"{_namespace}.{elementType} resource = " +
                    $"{_namespace}.ResourceConverter.PolymorphicRead(ref reader, typeof(Resource), options);");
                _writer.WriteLineIndented($"(({className})obj).{elementName}.Add(resource);");
            }
            else
            {
                _writer.WriteLineIndented($"object listObj = null;");
                _writer.WriteLineIndented($"{_namespace}.{elementType}.ParseJson(ref reader, options, ref listObj);");
                _writer.WriteLineIndented($"(({className})obj).{elementName}.Add(({elementType})listObj);");
            }

            _writer.WriteLine();
            _writer.WriteLineIndented($"if (!reader.Read())");
            _writer.WriteLineIndented("{");
            _writer.IncreaseIndent();
            _writer.WriteLineIndented("throw new JsonException();");
            _writer.DecreaseIndent();
            _writer.WriteLineIndented("}");
            _writer.DecreaseIndent();
            _writer.WriteLineIndented("}");
            _writer.WriteLine();
            _writer.WriteLineIndented($"if ((({className})obj).{elementName}.Count == 0)");
            _writer.WriteLineIndented("{");
            _writer.IncreaseIndent();
            _writer.WriteLineIndented($"(({className})obj).{elementName} = null;");
            _writer.DecreaseIndent();
            _writer.WriteLineIndented("}");
            _writer.WriteLineIndented("break;");
            _writer.DecreaseIndent();
        }

        /// <summary>Writes a JSON property list case.</summary>
        /// <exception cref="JsonException">Thrown when a JSON error condition occurs.</exception>
        /// <param name="className">         Name of the class.</param>
        /// <param name="elementName">       Name of the element.</param>
        /// <param name="camel">             The camel.</param>
        /// <param name="elementType">       Type of the element.</param>
        /// <param name="getterFunctionName">Name of the getter function.</param>
        private void WriteJsonPropertyListCaseSimple(
            string className,
            string elementName,
            string camel,
            string elementType,
            string getterFunctionName)
        {
            _writer.WriteLineIndented($"case \"{camel}\":");
            _writer.IncreaseIndent();
            _writer.WriteLineIndented($"if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))");
            _writer.WriteLineIndented("{");
            _writer.IncreaseIndent();
            _writer.WriteLineIndented("throw new JsonException();");
            _writer.DecreaseIndent();
            _writer.WriteLineIndented("}");
            _writer.WriteLine();
            _writer.WriteLineIndented($"(({className})obj).{elementName} = new List<{elementType}>();");
            _writer.WriteLine();
            _writer.WriteLineIndented($"while (reader.TokenType != JsonTokenType.EndArray)");
            _writer.WriteLineIndented("{");
            _writer.IncreaseIndent();
            _writer.WriteLineIndented($"(({className})obj).{elementName}.Add(reader.{getterFunctionName}());");
            _writer.WriteLine();
            _writer.WriteLineIndented($"if (!reader.Read())");
            _writer.WriteLineIndented("{");
            _writer.IncreaseIndent();
            _writer.WriteLineIndented("throw new JsonException();");
            _writer.DecreaseIndent();
            _writer.WriteLineIndented("}");
            _writer.DecreaseIndent();
            _writer.WriteLineIndented("}");
            _writer.WriteLine();
            _writer.WriteLineIndented($"if ((({className})obj).{elementName}.Count == 0)");
            _writer.WriteLineIndented("{");
            _writer.IncreaseIndent();
            _writer.WriteLineIndented($"(({className})obj).{elementName} = null;");
            _writer.DecreaseIndent();
            _writer.WriteLineIndented("}");
            _writer.WriteLineIndented("break;");
            _writer.DecreaseIndent();
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

            WriteIndentedComment($"Code Values for the {element.Path} field");

            if (codeName.EndsWith("Codes", StringComparison.Ordinal))
            {
                _writer.WriteLineIndented($"{_accessModifier} static class {codeName} {{");
            }
            else
            {
                _writer.WriteLineIndented($"{_accessModifier} static class {codeName}Codes {{");
            }

            _writer.IncreaseIndent();

            foreach (string code in element.Codes)
            {
                FhirUtils.SanitizeForCode(code, _reservedWords, out string name, out string value);

                _writer.WriteLineIndented($"{_accessModifier} const string {name.ToUpperInvariant()} = \"{value}\";");
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
            Dictionary<string, string> values = element.NamesAndTypesForExport(
                FhirTypeBase.NamingConvention.PascalCase,
                FhirTypeBase.NamingConvention.PascalCase,
                false,
                string.Empty,
                complex.Components.ContainsKey(element.Path));

            foreach (KeyValuePair<string, string> kvp in values)
            {
                string elementName;
                if ((kvp.Key == complex.Name) && (!element.IsInherited))
                {
                    elementName = $"{kvp.Key}Field";
                }
                else
                {
                    elementName = kvp.Key;
                }

                string optionalFlagString =
                    (element.IsOptional && (!element.IsArray) && IsNullable(kvp.Value))
                        ? "?"
                        : string.Empty;

                if (!string.IsNullOrEmpty(element.Comment))
                {
                    WriteIndentedComment(element.Comment);
                }

                string elementType = element.IsArray
                    ? $"List<{kvp.Value}{optionalFlagString}>"
                    : $"{kvp.Value}{optionalFlagString}";

                string camel = FhirUtils.ToConvention(kvp.Key, string.Empty, FhirTypeBase.NamingConvention.CamelCase);

                if (_jsonNewtonSoft)
                {
                    _writer.WriteLineIndented($"[JsonProperty(\"{camel}\")]");
                }

                _writer.WriteLineIndented($"{_accessModifier} {elementType} {elementName} {{ get; set; }}");

                if (RequiresExtension(kvp.Value))
                {
                    if (_jsonNewtonSoft)
                    {
                        _writer.WriteLineIndented($"[JsonProperty(\"_{camel}\")]");
                    }

                    if (element.IsArray)
                    {
                        _writer.WriteLineIndented($"{_accessModifier} List<Element> _{elementName} {{ get; set; }}");
                    }
                    else
                    {
                        _writer.WriteLineIndented($"{_accessModifier} Element _{elementName} {{ get; set; }}");
                    }
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
            _writer.WriteLineIndented("// <auto-generated />");
            _writer.WriteLineIndented($"// Contents of: {_info.PackageName} version: {_info.VersionString}");
            _writer.WriteLineIndented($"  // Primitive Naming Style: {FhirTypeBase.NamingConvention.None}");
            _writer.WriteLineIndented($"  // Complex Type / Resource Naming Style: {FhirTypeBase.NamingConvention.PascalCase}");
            _writer.WriteLineIndented($"  // Element Naming Style: {FhirTypeBase.NamingConvention.PascalCase}");
            _writer.WriteLineIndented($"  // Enum Naming Style: {FhirTypeBase.NamingConvention.PascalCase}");
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

            _writer.WriteLine();

            _writer.WriteLineIndented("using System;");
            _writer.WriteLineIndented("using System.Collections.Generic;");

            if (_jsonSystemText)
            {
                _writer.WriteLineIndented("using System.Text.Json;");
                _writer.WriteLineIndented("using System.Text.Json.Serialization;");
            }

            if (_jsonNewtonSoft)
            {
                _writer.WriteLineIndented("using Newtonsoft.Json;");
                _writer.WriteLineIndented("using Newtonsoft.Json.Linq;");
            }

            _writer.WriteLine();

            if (!_includeSummaries)
            {
                _writer.WriteLineIndented("#pragma warning disable 1591");
                _writer.WriteLine();
            }
        }

        /// <summary>Writes a footer.</summary>
        private void WriteFooter()
        {
            if (!_includeSummaries)
            {
                _writer.WriteLine();
                _writer.WriteLineIndented("#pragma warning restore 1591");
                _writer.WriteLine();
            }

            WriteIndentedComment("end of file", false);
        }

        /// <summary>Writes an indented comment.</summary>
        /// <param name="value">    The value.</param>
        /// <param name="isSummary">(Optional) True if is summary, false if not.</param>
        private void WriteIndentedComment(string value, bool isSummary = true)
        {
            if (isSummary)
            {
                if (!_includeSummaries)
                {
                    return;
                }

                _writer.WriteLineIndented("/// <summary>");
            }

            string comment = value
                .Replace('\r', '\n')
                .Replace("\r\n", "\n")
                .Replace("\n\n", "\n")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;");

            string[] lines = comment.Split('\n');
            foreach (string line in lines)
            {
                _writer.WriteIndented("/// ");
                _writer.WriteLine(line);
            }

            if (isSummary)
            {
                _writer.WriteLineIndented("/// </summary>");
            }
        }
    }
}
