// <copyright file="CSharp2.cs" company="Microsoft Corporation">
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
    public sealed class CSharp2 : ILanguage
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

        /// <summary>The base namespace to use for all exported classes.</summary>
        private string _namespace;

        /// <summary>The namespace models.</summary>
        private string _namespaceModels;

        /// <summary>Sets the namespace value belongs to.</summary>
        private string _namespaceValueSets;

        /// <summary>The serialization namespace.</summary>
        private string _namespaceSerialization;

        /// <summary>True to include, false to exclude the summaries.</summary>
        private bool _includeSummaries = true;

        /// <summary>The access modifier.</summary>
        private string _accessModifier = "public";

        /// <summary>Pathname of the model directory.</summary>
        private string _directoryModels;

        /// <summary>Pathname of the value set directory.</summary>
        private string _directoryValueSets;

        /// <summary>Pathname of the serialization directory.</summary>
        private string _directorySerialization;

        /// <summary>List of types of the exported resource names and types.</summary>
        private Dictionary<string, string> _exportedResourceNamesAndTypes = new Dictionary<string, string>();

        /// <summary>The exported codes.</summary>
        private HashSet<string> _exportedCodes = new HashSet<string>();

        /// <summary>The currently in-use text writer.</summary>
        private ExportStreamWriter _writer;

        /// <summary>Name of the language.</summary>
        private const string _languageName = "CSharp2";

        /// <summary>The single file export extension - requires directory export.</summary>
        private const string _singleFileExportExtension = null;

        /// <summary>Dictionary mapping FHIR primitive types to language equivalents.</summary>
        private static readonly Dictionary<string, string> _primitiveTypeMap = new Dictionary<string, string>()
        {
            { "base", "Object" },
            { "base64Binary", "byte[]" },
            { "bool", "bool" },
            { "bool?", "bool" },
            { "boolean", "bool" },
            { "canonical", "string" },
            { "code", "string" },
            { "date", "string" },
            { "dateTime", "string" },           // Cannot use "DateTime" because of Partial Dates... may want to consider defining a new type, but not today
            { "decimal", "decimal" },
            { "id", "string" },
            { "instant", "string" },
            { "int", "int" },
            { "int?", "int" },
            { "integer", "int" },
            { "integer64", "long" },
            { "markdown", "string" },
            { "oid", "string" },
            { "positiveInt", "uint" },
            { "string", "string" },
            { "string?", "string" },
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
            { "namespace", "Base namespace for C# classes (default: Fhir.R{VersionNumber})." },
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

            if (options.OptionalClassTypesToExport.Contains(ExporterOptions.FhirExportClassType.Enum))
            {
                _exportEnums = true;
            }
            else
            {
                _exportEnums = false;
            }

            _namespace = options.GetParam("namespace", $"Fhir.{info.FhirSequence}");
            _namespaceModels = _namespace + ".Models";
            _namespaceValueSets = _namespace + ".ValueSets";
            _namespaceSerialization = _namespace + ".Serialization";

            _includeSummaries = options.GetParam("include-summaries", true);
            _accessModifier = options.GetParam("access-modifier", "public");

            _exportedResourceNamesAndTypes = new Dictionary<string, string>();
            _exportedCodes = new HashSet<string>();

            _directoryModels = Path.Combine(exportDirectory, "Models");
            if (!Directory.Exists(_directoryModels))
            {
                Directory.CreateDirectory(_directoryModels);
            }

            _directoryValueSets = Path.Combine(exportDirectory, "ValueSets");
            if (_exportEnums)
            {
                if (!Directory.Exists(_directoryValueSets))
                {
                    Directory.CreateDirectory(_directoryValueSets);
                }
            }

            _directorySerialization = Path.Combine(exportDirectory, "Serialization");
            if (!Directory.Exists(_directorySerialization))
            {
                Directory.CreateDirectory(_directorySerialization);
            }

            WriteComplexes(_info.ComplexTypes.Values, false);
            WriteComplexes(_info.Resources.Values, true);

            if (_exportEnums)
            {
                WriteValueSets(_info.ValueSetsByUrl.Values);
            }

            WriteJsonSerializationHelpers();
            WriteXmlSerializationHelpers();
        }

        /// <summary>Writes the XML serialization helpers.</summary>
        private void WriteXmlSerializationHelpers()
        {
            WriteXmlSerializerInterface();
            WriteXmlSerializerOptions();
        }

        /// <summary>Writes the JSON serializer options.</summary>
        private void WriteXmlSerializerOptions()
        {
            // create a filename for writing
            string filename = Path.Combine(_directorySerialization, "FhirXmlSerializerOptions.cs");

            using (FileStream stream = new FileStream(filename, FileMode.Create))
            using (ExportStreamWriter writer = new ExportStreamWriter(stream))
            {
                _writer = writer;

                WriteHeader(false, false, true);

                // open namespace
                _writer.WriteLineIndented($"namespace {_namespaceSerialization}");
                _writer.OpenScope();

                // open class
                WriteIndentedComment("Default XmlSerializerOptions to format XML serialization as expected.");
                _writer.WriteLineIndented($"{_accessModifier} static class FhirXmlSerializerOptions");
                _writer.OpenScope();

                _writer.WriteLine("#pragma warning disable CA1810 // Initialize reference type static fields inline");
                _writer.WriteLine();

                WriteIndentedComment("Compact format internal variable.");
                _writer.WriteLineIndented("private static readonly XmlWriterSettings _compactFormat;");
                _writer.WriteLine();

                WriteIndentedComment("Pretty print format internal variable.");
                _writer.WriteLineIndented("private static readonly XmlWriterSettings _prettyFormat;");
                _writer.WriteLine();

                WriteIndentedComment("Parser settings internal variable.");
                _writer.WriteLineIndented("private static readonly XmlReaderSettings _readerSettings;");
                _writer.WriteLine();

                WriteIndentedComment("Initializes static members of the <see cref=\"FhirXmlSerializerOptions\"/> class.");
                _writer.WriteLineIndented("static FhirXmlSerializerOptions()");
                _writer.OpenScope();
                _writer.WriteLineIndented("_prettyFormat = new XmlWriterSettings()");
                _writer.OpenScope();
                _writer.WriteLineIndented("Indent = true,");
                _writer.WriteLineIndented("OmitXmlDeclaration = false,");
                _writer.WriteLineIndented("NewLineOnAttributes = true,");
                _writer.CloseScope("};");
                _writer.WriteLine();
                _writer.WriteLineIndented("_compactFormat = new XmlWriterSettings()");
                _writer.OpenScope();
                _writer.WriteLineIndented("Indent = true,");
                _writer.WriteLineIndented("OmitXmlDeclaration = false,");
                _writer.WriteLineIndented("NewLineOnAttributes = true,");
                _writer.CloseScope("};");
                _writer.WriteLine();
                _writer.WriteLineIndented("_readerSettings = new XmlReaderSettings()");
                _writer.OpenScope();
                _writer.WriteLineIndented("CheckCharacters = true,");
                _writer.WriteLineIndented("DtdProcessing = DtdProcessing.Ignore,");
                _writer.CloseScope("};");

                _writer.CloseScope();
                _writer.WriteLine();

                _writer.WriteLine("#pragma warning restore CA1810 // Initialize reference type static fields inline");
                _writer.WriteLine();

                WriteIndentedComment("Compact (no extra whitespace) format.");
                _writer.WriteLineIndented("public static XmlWriterSettings Compact => _compactFormat;");
                _writer.WriteLine();

                WriteIndentedComment("Pretty-printed (newlines and indentation) format.");
                _writer.WriteLineIndented("public static XmlWriterSettings Pretty => _prettyFormat;");

                WriteIndentedComment("Default Parser settings.");
                _writer.WriteLineIndented("public static XmlReaderSettings ParseSettings => _readerSettings;");

                // close class
                _writer.CloseScope();

                // close namespace
                _writer.CloseScope();

                WriteFooter();
            }
        }

        /// <summary>Writes the XML serializer interface.</summary>
        private void WriteXmlSerializerInterface()
        {
            // create a filename for writing
            string filename = Path.Combine(_directorySerialization, "IFhirXmlSerializable.cs");

            using (FileStream stream = new FileStream(filename, FileMode.Create))
            using (ExportStreamWriter writer = new ExportStreamWriter(stream))
            {
                _writer = writer;

                WriteHeader(false, false, true);

                // open namespace
                _writer.WriteLineIndented($"namespace {_namespaceSerialization}");
                _writer.OpenScope();

                // open interface
                WriteIndentedComment("Interface for indicating FHIR XML Serialization is possible.");
                _writer.WriteLineIndented($"{_accessModifier} interface IFhirXmlSerializable");
                _writer.OpenScope();

                // function SerializeJson
                WriteIndentedComment("Serialize to an XML object");
                _writer.WriteLineIndented($"void SerializeXml(" +
                    "XmlWriter writer, " +
                    "XmlWriterSettings options, " +
                    "bool includeStartObject);");

                // function DeserializeXml
                WriteIndentedComment("Parse an open XML object into the current object.");
                _writer.WriteLineIndented($"void DeserializeXml(" +
                    $"ref XmlReader reader, " +
                    $"XmlReaderSettings options);");

                // function DeserializeXmlProperty
                WriteIndentedComment("Parse a specific property from an open XML object into a field in the current object.");
                _writer.WriteLineIndented($"void DeserializeXmlProperty(" +
                    $"ref XmlReader reader, " +
                    $"XmlReaderSettings options, " +
                    $"string propertyName);");

                // close interface
                _writer.CloseScope();

                // close namespace
                _writer.CloseScope();

                WriteFooter();
            }
        }

        /// <summary>Writes the JSON serialization helper files.</summary>
        private void WriteJsonSerializationHelpers()
        {
            WriteJsonSerializerInterface();
            WriteJsonSerializerOptions();

            WriteJsonStreamResourceConverter();
            WriteJsonStreamComponentConverter();
        }

        /// <summary>Writes the JSON serializer options.</summary>
        private void WriteJsonSerializerOptions()
        {
            // create a filename for writing
            string filename = Path.Combine(_directorySerialization, "FhirJsonSerializerOptions.cs");

            using (FileStream stream = new FileStream(filename, FileMode.Create))
            using (ExportStreamWriter writer = new ExportStreamWriter(stream))
            {
                _writer = writer;

                WriteHeader(false, false, true);

                // open namespace
                _writer.WriteLineIndented($"namespace {_namespaceSerialization}");
                _writer.OpenScope();

                // open class
                WriteIndentedComment("Default JsonSerializerOptions to format JSON serialization as expected.");
                _writer.WriteLineIndented($"{_accessModifier} static class FhirJsonSerializerOptions");
                _writer.OpenScope();

                _writer.WriteLine("#pragma warning disable CA1810 // Initialize reference type static fields inline");
                _writer.WriteLine();

                WriteIndentedComment("Compact format internal variable.");
                _writer.WriteLineIndented("private static readonly JsonSerializerOptions _compactFormat;");
                _writer.WriteLine();

                WriteIndentedComment("Pretty print format internal variable.");
                _writer.WriteLineIndented("private static readonly JsonSerializerOptions _prettyFormat;");
                _writer.WriteLine();

                WriteIndentedComment("Initializes static members of the <see cref=\"FhirJsonSerializerOptions\"/> class.");
                _writer.WriteLineIndented("static FhirJsonSerializerOptions()");
                _writer.OpenScope();
                _writer.WriteLineIndented("_prettyFormat = new JsonSerializerOptions();");
                _writer.WriteLineIndented("_prettyFormat.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;");
                _writer.WriteLineIndented("_prettyFormat.WriteIndented = true;");
                _writer.WriteLine();
                _writer.WriteLineIndented("_compactFormat = new JsonSerializerOptions();");
                _writer.WriteLineIndented("_compactFormat.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;");
                _writer.WriteLineIndented("_compactFormat.WriteIndented = true;");
                _writer.CloseScope();
                _writer.WriteLine();

                _writer.WriteLine("#pragma warning restore CA1810 // Initialize reference type static fields inline");
                _writer.WriteLine();

                WriteIndentedComment("Compact (no extra whitespace) format.");
                _writer.WriteLineIndented("public static JsonSerializerOptions Compact => _compactFormat;");
                _writer.WriteLine();

                WriteIndentedComment("Pretty-printed (newlines and indentation) format.");
                _writer.WriteLineIndented("public static JsonSerializerOptions Pretty => _prettyFormat;");

                // close class
                _writer.CloseScope();

                // close namespace
                _writer.CloseScope();

                WriteFooter();
            }
        }

        /// <summary>Writes the JSON serializer resource converter.</summary>
        /// <exception cref="JsonException">Thrown when a JSON error condition occurs.</exception>
        private void WriteJsonSerializerInterface()
        {
            // create a filename for writing
            string filename = Path.Combine(_directorySerialization, "IFhirJsonSerializable.cs");

            using (FileStream stream = new FileStream(filename, FileMode.Create))
            using (ExportStreamWriter writer = new ExportStreamWriter(stream))
            {
                _writer = writer;

                WriteHeader(false, false, true);

                // open namespace
                _writer.WriteLineIndented($"namespace {_namespaceSerialization}");
                _writer.OpenScope();

                // open interface
                WriteIndentedComment("Interface for indicating FHIR Json Serialization is possible.");
                _writer.WriteLineIndented($"{_accessModifier} interface IFhirJsonSerializable");
                _writer.OpenScope();

                // function SerializeJson
                WriteIndentedComment("Serialize to a JSON object");
                _writer.WriteLineIndented($"void SerializeJson(" +
                    "Utf8JsonWriter writer, " +
                    "JsonSerializerOptions options, " +
                    "bool includeStartObject);");

                // function DeserializeJson
                WriteIndentedComment("Parse an open JSON object into the current object.");
                _writer.WriteLineIndented($"void DeserializeJson(" +
                    $"ref Utf8JsonReader reader, " +
                    $"JsonSerializerOptions options);");

                // function DeserializeJsonProperty
                WriteIndentedComment("Parse a specific property from an open JSON object into a field in the current object.");
                _writer.WriteLineIndented($"void DeserializeJsonProperty(" +
                    $"ref Utf8JsonReader reader, " +
                    $"JsonSerializerOptions options, " +
                    $"string propertyName);");

                // close interface
                _writer.CloseScope();

                // close namespace
                _writer.CloseScope();

                WriteFooter();
            }
        }

        /// <summary>Writes the JSON serializer component converter.</summary>
        /// <exception cref="JsonException">Thrown when a JSON error condition occurs.</exception>
        private void WriteJsonStreamComponentConverter()
        {
            // create a filename for writing
            string filename = Path.Combine(_directorySerialization, "JsonStreamComponentConverter.cs");

            using (FileStream stream = new FileStream(filename, FileMode.Create))
            using (ExportStreamWriter writer = new ExportStreamWriter(stream))
            {
                _writer = writer;

                WriteHeader(false, false, true);

                // open namespace
                _writer.WriteLineIndented($"namespace {_namespaceSerialization}");
                _writer.OpenScope();

                // open class
                WriteIndentedComment("Common converter to support deserialization of non-resource FHIR components.");
                _writer.WriteLineIndented($"{_accessModifier} class JsonStreamComponentConverter<T> : JsonConverter<T>");
                _writer.WriteLineIndented($"  where T : IFhirJsonSerializable, new()");
                _writer.OpenScope();

                // function CanConvert
                WriteIndentedComment("Determines whether the specified type can be converted.");
                _writer.WriteLineIndented($"{_accessModifier} override bool CanConvert(Type objectType) =>");
                _writer.IncreaseIndent();
                _writer.WriteLineIndented("typeof(T).IsAssignableFrom(objectType);");
                _writer.DecreaseIndent();
                _writer.WriteLine();

                WriteIndentedComment("Writes a specified value as JSON.");
                _writer.WriteLineIndented($"{_accessModifier} override void Write(" +
                    "Utf8JsonWriter writer, " +
                    "T component, " +
                    "JsonSerializerOptions options)");

                // open Write
                _writer.OpenScope();

                _writer.WriteLineIndented($"component.SerializeJson(writer, options, true);");
                _writer.WriteLineIndented("writer.Flush();");

                // close Write
                _writer.CloseScope();

                WriteIndentedComment("Reads and converts the JSON to a typed object.");
                _writer.WriteLineIndented($"{_accessModifier} override T Read(" +
                    "ref Utf8JsonReader reader, " +
                    "Type typeToConvert, " +
                    "JsonSerializerOptions options)");

                // open Read
                _writer.OpenScope();

                _writer.WriteLineIndented("return ComponentRead(ref reader, typeToConvert, options);");

                // close Read
                _writer.CloseScope();

                WriteIndentedComment("Read override to handle reading of components (to allow for open reader).");
                _writer.WriteLineIndented($"{_accessModifier} static T ComponentRead(" +
                    "ref Utf8JsonReader reader, " +
                    "Type typeToConvert, " +
                    "JsonSerializerOptions options)");

                // open Read
                _writer.OpenScope();

                _writer.WriteLineIndented("if (reader.TokenType != JsonTokenType.StartObject)");
                _writer.OpenScope();
                _writer.WriteLineIndented("throw new JsonException();");
                _writer.CloseScope();
                _writer.WriteLine();

                _writer.WriteLineIndented("IFhirJsonSerializable target = new T();");
                _writer.WriteLineIndented("target.DeserializeJson(ref reader, options);");
                _writer.WriteLine();
                _writer.WriteLineIndented("return (T)target;");

                // close Read
                _writer.CloseScope();

                // close class
                _writer.CloseScope();

                // close namespace
                _writer.CloseScope();

                WriteFooter();
            }
        }

        /// <summary>Writes the JSON serializer resource converter.</summary>
        /// <exception cref="JsonException">Thrown when a JSON error condition occurs.</exception>
        private void WriteJsonStreamResourceConverter()
        {
            // create a filename for writing
            string filename = Path.Combine(_directorySerialization, "JsonStreamResourceConverter.cs");

            using (FileStream stream = new FileStream(filename, FileMode.Create))
            using (ExportStreamWriter writer = new ExportStreamWriter(stream))
            {
                _writer = writer;

                WriteHeader(false, false, true);

                // open namespace
                _writer.WriteLineIndented($"namespace {_namespaceSerialization}");
                _writer.OpenScope();

                // open class
                WriteIndentedComment("Common resource converter to support polymorphic deserialization.");
                _writer.WriteLineIndented($"{_accessModifier} class JsonStreamResourceConverter : JsonConverter<Resource>");
                _writer.OpenScope();

                _writer.WriteLineIndented("private static readonly byte[] _startObject = Encoding.UTF8.GetBytes(\"{\");");
                _writer.WriteLineIndented("private static readonly byte[] _endObject = Encoding.UTF8.GetBytes(\"}\");");
                _writer.WriteLineIndented("private static readonly byte[] _startArray = Encoding.UTF8.GetBytes(\"[\");");
                _writer.WriteLineIndented("private static readonly byte[] _endArray = Encoding.UTF8.GetBytes(\"]\");");
                _writer.WriteLineIndented("private static readonly byte[] _comma = Encoding.UTF8.GetBytes(\",\");");
                _writer.WriteLineIndented("private static readonly byte[] _propertySep = Encoding.UTF8.GetBytes(\":\");");
                _writer.WriteLineIndented("private static readonly byte[] _quote = Encoding.UTF8.GetBytes(\"\\\"\");");
                _writer.WriteLine();

                // function CanConvert
                WriteIndentedComment("Determines whether the specified type can be converted.");
                _writer.WriteLineIndented($"{_accessModifier} override bool CanConvert(Type objectType) =>");
                _writer.IncreaseIndent();
                _writer.WriteLineIndented("typeof(Resource).IsAssignableFrom(objectType);");
                _writer.DecreaseIndent();
                _writer.WriteLine();

                WriteIndentedComment("Writes a specified value as JSON.");
                _writer.WriteLineIndented($"{_accessModifier} override void Write(" +
                    "Utf8JsonWriter writer, " +
                    "Resource resource, " +
                    "JsonSerializerOptions options)");

                // open Write
                _writer.OpenScope();

                _writer.WriteLineIndented($"switch (resource)");

                // open switch
                _writer.OpenScope();

                // loop through our types
                foreach (KeyValuePair<string, string> kvp in _exportedResourceNamesAndTypes)
                {
                    _writer.WriteLineIndented($"case {_namespaceModels}.{kvp.Value} typed{kvp.Value}:");
                    _writer.IncreaseIndent();
                    _writer.WriteLineIndented($"typed{kvp.Value}.SerializeJson(writer, options, true);");
                    _writer.WriteLineIndented("break;");
                    _writer.DecreaseIndent();
                }

                // close switch
                _writer.CloseScope();

                _writer.WriteLine();
                _writer.WriteLineIndented("writer.Flush();");

                // close Write
                _writer.CloseScope();
                _writer.WriteLine();

                WriteIndentedComment("Reads and converts the JSON to a typed object.");
                _writer.WriteLineIndented($"{_accessModifier} override Resource Read(" +
                    "ref Utf8JsonReader reader, " +
                    "Type typeToConvert, " +
                    "JsonSerializerOptions options)");

                // open Read
                _writer.OpenScope();

                _writer.WriteLineIndented("return PolymorphicRead(ref reader, typeToConvert, options);");

                // close Read
                _writer.CloseScope();
                _writer.WriteLine();

                // function WriteReaderValueBytes
                WriteIndentedComment("Copy raw data from a Utf8JsonReader to a MemoryStream.");
                _writer.WriteLineIndented("private static void WriteReaderValueBytes(ref MemoryStream ms, ref Utf8JsonReader reader)");

                // open WriteReaderValueBytes
                _writer.OpenScope();

                _writer.WriteLineIndented("if (reader.HasValueSequence)");
                _writer.OpenScope();
                _writer.WriteLineIndented("byte[] data = new byte[reader.ValueSequence.Length];");
                _writer.WriteLineIndented("reader.ValueSequence.CopyTo(data);");
                _writer.WriteLineIndented("ms.Write(data);");
                _writer.WriteLineIndented("return;");
                _writer.CloseScope();
                _writer.WriteLine();
                _writer.WriteLineIndented("ms.Write(reader.ValueSpan);");

                // close WriteReaderValueBytes
                _writer.CloseScope();
                _writer.WriteLine();

                // function AddSeperatorIfNeeded
                WriteIndentedComment("Add a JSON seperator token, if necessary.");
                _writer.WriteLineIndented("private static void AddSeperatorIfNeeded(ref MemoryStream ms, ref Utf8JsonReader reader, JsonTokenType last)");

                // open AddSeperatorIfNeeded
                _writer.OpenScope();

                _writer.WriteLineIndented("switch (last)");
                _writer.OpenScope();
                _writer.WriteLineIndented("case JsonTokenType.StartObject:");
                _writer.WriteLineIndented("case JsonTokenType.StartArray:");
                _writer.IncreaseIndent();
                _writer.WriteLineIndented("// do nothing");
                _writer.WriteLineIndented("break;");
                _writer.DecreaseIndent();
                _writer.WriteLineIndented("case JsonTokenType.PropertyName:");
                _writer.IncreaseIndent();
                _writer.WriteLineIndented("ms.Write(_propertySep);");
                _writer.WriteLineIndented("break;");
                _writer.DecreaseIndent();
                _writer.WriteLineIndented("default:");
                _writer.IncreaseIndent();
                _writer.WriteLineIndented("ms.Write(_comma);");
                _writer.WriteLineIndented("break;");
                _writer.DecreaseIndent();
                _writer.CloseScope();

                // close AddSeperatorIfNeeded
                _writer.CloseScope();
                _writer.WriteLine();

                // function PolymorphicRead
                WriteIndentedComment("Read override to handle polymorphic reading of resources.");
                _writer.WriteLineIndented($"{_accessModifier} static Resource PolymorphicRead(" +
                    "ref Utf8JsonReader reader, " +
                    "Type typeToConvert, " +
                    "JsonSerializerOptions options)");

                // open PolymorphicRead
                _writer.OpenScope();

                _writer.WriteLineIndented("string propertyName = null;");
                _writer.WriteLineIndented("string resourceType = null;");
                _writer.WriteLine();
                _writer.WriteLineIndented("if (reader.TokenType != JsonTokenType.StartObject)");
                _writer.OpenScope();
                _writer.WriteLineIndented("throw new JsonException();");
                _writer.CloseScope();
                _writer.WriteLine();
                _writer.WriteLineIndented("reader.Read();");
                _writer.WriteLineIndented("if (reader.TokenType != JsonTokenType.PropertyName)");
                _writer.OpenScope();
                _writer.WriteLineIndented("throw new JsonException();");
                _writer.CloseScope();
                _writer.WriteLine();

                _writer.WriteLineIndented("propertyName = reader.GetString();");
                _writer.WriteLineIndented("if (propertyName == \"resourceType\")");
                _writer.OpenScope();
                _writer.WriteLineIndented("reader.Read();");
                _writer.WriteLineIndented("if (reader.TokenType != JsonTokenType.String)");
                _writer.OpenScope();
                _writer.WriteLineIndented("throw new JsonException();");
                _writer.CloseScope();
                _writer.WriteLine();
                _writer.WriteLineIndented("resourceType = reader.GetString();");
                _writer.WriteLine();
                _writer.WriteLineIndented("return DoPolymorphicRead(ref reader, options, resourceType);");
                _writer.CloseScope();
                _writer.WriteLine();

                _writer.WriteLineIndented("MemoryStream ms = new MemoryStream(4096);");
                _writer.WriteLine();
                _writer.WriteLineIndented("ms.Write(Encoding.UTF8.GetBytes($\"{{\\\"{propertyName}\\\"\"));");
                _writer.WriteLineIndented("propertyName = string.Empty;");
                _writer.WriteLine();
                _writer.WriteLineIndented("int depth = reader.CurrentDepth;");
                _writer.WriteLineIndented("bool done = false;");
                _writer.WriteLineIndented("bool nextValueIsResourceType = false;");
                _writer.WriteLineIndented("JsonTokenType lastToken = JsonTokenType.PropertyName;");
                _writer.WriteLine();
                _writer.WriteLineIndented("while ((!done) && reader.Read())");
                _writer.OpenScope();
                _writer.WriteLineIndented("switch (reader.TokenType)");
                _writer.OpenScope();
                _writer.WriteLineIndented("case JsonTokenType.StartObject:");
                _writer.IncreaseIndent();
                _writer.WriteLineIndented("AddSeperatorIfNeeded(ref ms, ref reader, lastToken);");
                _writer.WriteLineIndented("ms.Write(_startObject);");
                _writer.WriteLineIndented("break;");
                _writer.DecreaseIndent();
                _writer.WriteLine();
                _writer.WriteLineIndented("case JsonTokenType.EndObject:");
                _writer.IncreaseIndent();
                _writer.WriteLineIndented("ms.Write(_endObject);");
                _writer.WriteLineIndented("if (reader.CurrentDepth == (depth - 1))");
                _writer.OpenScope();
                _writer.WriteLineIndented("done = true;");
                _writer.CloseScope();
                _writer.WriteLineIndented("break;");
                _writer.DecreaseIndent();
                _writer.WriteLine();
                _writer.WriteLineIndented("case JsonTokenType.StartArray:");
                _writer.IncreaseIndent();
                _writer.WriteLineIndented("AddSeperatorIfNeeded(ref ms, ref reader, lastToken);");
                _writer.WriteLineIndented("ms.Write(_startArray);");
                _writer.WriteLineIndented("break;");
                _writer.DecreaseIndent();
                _writer.WriteLine();
                _writer.WriteLineIndented("case JsonTokenType.EndArray:");
                _writer.IncreaseIndent();
                _writer.WriteLineIndented("ms.Write(_endArray);");
                _writer.WriteLineIndented("break;");
                _writer.DecreaseIndent();
                _writer.WriteLine();
                _writer.WriteLineIndented("case JsonTokenType.PropertyName:");
                _writer.IncreaseIndent();
                _writer.WriteLineIndented("AddSeperatorIfNeeded(ref ms, ref reader, lastToken);");
                _writer.WriteLineIndented("if (reader.CurrentDepth == depth)");
                _writer.OpenScope();
                _writer.WriteLineIndented("if (reader.ValueTextEquals(\"resourceType\"))");
                _writer.OpenScope();
                _writer.WriteLineIndented("nextValueIsResourceType = true;");
                _writer.CloseScope();
                _writer.CloseScope();
                _writer.WriteLine();
                _writer.WriteLineIndented("ms.Write(_quote);");
                _writer.WriteLineIndented("WriteReaderValueBytes(ref ms, ref reader);");
                _writer.WriteLineIndented("ms.Write(_quote);");
                _writer.WriteLineIndented("break;");
                _writer.DecreaseIndent();
                _writer.WriteLine();
                _writer.WriteLineIndented("case JsonTokenType.Comment:");
                _writer.IncreaseIndent();
                _writer.WriteLineIndented("break;");
                _writer.DecreaseIndent();
                _writer.WriteLine();
                _writer.WriteLineIndented("case JsonTokenType.String:");
                _writer.IncreaseIndent();
                _writer.WriteLineIndented("AddSeperatorIfNeeded(ref ms, ref reader, lastToken);");
                _writer.WriteLineIndented("if (nextValueIsResourceType)");
                _writer.OpenScope();
                _writer.WriteLineIndented("resourceType = reader.GetString();");
                _writer.WriteLineIndented("nextValueIsResourceType = false;");
                _writer.CloseScope();
                _writer.WriteLine();
                _writer.WriteLineIndented("ms.Write(_quote);");
                _writer.WriteLineIndented("WriteReaderValueBytes(ref ms, ref reader);");
                _writer.WriteLineIndented("ms.Write(_quote);");
                _writer.WriteLineIndented("break;");
                _writer.DecreaseIndent();
                _writer.WriteLine();
                _writer.WriteLineIndented("case JsonTokenType.Number:");
                _writer.WriteLineIndented("case JsonTokenType.True:");
                _writer.WriteLineIndented("case JsonTokenType.False:");
                _writer.WriteLineIndented("case JsonTokenType.Null:");
                _writer.WriteLineIndented("default:");
                _writer.IncreaseIndent();
                _writer.WriteLineIndented("AddSeperatorIfNeeded(ref ms, ref reader, lastToken);");
                _writer.WriteLineIndented("WriteReaderValueBytes(ref ms, ref reader);");
                _writer.WriteLineIndented("break;");
                _writer.DecreaseIndent();
                _writer.CloseScope();
                _writer.WriteLine();
                _writer.WriteLineIndented("lastToken = reader.TokenType;");
                _writer.CloseScope();
                _writer.WriteLine();
                _writer.WriteLineIndented("ms.Flush();");
                _writer.WriteLineIndented("Utf8JsonReader secondary = new Utf8JsonReader(ms.GetBuffer());");
                _writer.WriteLine();
                _writer.WriteLineIndented("return DoPolymorphicRead(ref secondary, options, resourceType);");

                // close PolymorphicRead
                _writer.CloseScope();

                // function DoPolymorphicRead
                WriteIndentedComment("Sub-function for simpler handling of reader switching.");
                _writer.WriteLineIndented($"{_accessModifier} static Resource DoPolymorphicRead(" +
                    "ref Utf8JsonReader reader, " +
                    "JsonSerializerOptions options, " +
                    "string resourceType)");

                // open DoPolymorphicRead
                _writer.OpenScope();

                _writer.WriteLineIndented("IFhirJsonSerializable target = null;");

                _writer.WriteLineIndented("switch (resourceType)");

                // open switch
                _writer.OpenScope();

                // loop through our types
                foreach (KeyValuePair<string, string> kvp in _exportedResourceNamesAndTypes)
                {
                    _writer.WriteLineIndented($"case \"{kvp.Key}\":");
                    _writer.IncreaseIndent();
                    _writer.WriteLineIndented($"target = new {_namespaceModels}.{kvp.Value}();");
                    _writer.WriteLineIndented($"target.DeserializeJson(ref reader, options);");
                    _writer.WriteLineIndented("break;");
                    _writer.DecreaseIndent();
                }

                // default case returns a Resource object
                _writer.WriteLineIndented("default:");
                _writer.IncreaseIndent();
                _writer.WriteLineIndented($"target = new {_namespaceModels}.Resource();");
                _writer.WriteLineIndented($"target.DeserializeJson(ref reader, options);");
                _writer.WriteLineIndented("break;");
                _writer.DecreaseIndent();

                // close switch
                _writer.CloseScope();
                _writer.WriteLine();

                _writer.WriteLineIndented("return (Resource)target;");

                // close DoPolymorphicRead
                _writer.CloseScope();

                // close class
                _writer.CloseScope();

                // close namespace
                _writer.CloseScope();

                WriteFooter();
            }
        }

        /// <summary>Writes a value sets.</summary>
        /// <param name="valueSets">List of valueSetCollections.</param>
        private void WriteValueSets(
            IEnumerable<FhirValueSetCollection> valueSets)
        {
            HashSet<string> exportedNames = new HashSet<string>();

            foreach (FhirValueSetCollection collection in valueSets)
            {
                FhirValueSet vs = collection.ValueSetsByVersion.Values.OrderBy((vs) => vs.Version).Last();

                //foreach (FhirValueSet vs in collection.ValueSetsByVersion.Values)
                //{
                    string vsName = FhirUtils.SanitizeForProperty(vs.Id ?? vs.Name, _reservedWords);

                    vsName = FhirUtils.SanitizedToConvention(vsName, FhirTypeBase.NamingConvention.PascalCase);

                    if ((!string.IsNullOrEmpty(vs.Version)) &&
                        (vs.Version != _info.VersionString))
                    {
                        vsName += "_" + vs.Version.Replace('.', '_');
                    }

                    if (exportedNames.Contains(vsName))
                    {
                        Console.WriteLine($"Duplicate export name: {vsName} ({vs.Key})");
                        continue;
                    }

                    exportedNames.Add(vsName);

                    // create a filename for writing
                    string filename = Path.Combine(_directoryValueSets, $"{vsName}.cs");

                    using (FileStream stream = new FileStream(filename, FileMode.Create))
                    using (ExportStreamWriter writer = new ExportStreamWriter(stream))
                    {
                        _writer = writer;

                        WriteHeader(true, false, false);

                        // open namespace
                        _writer.WriteLineIndented($"namespace {_namespaceValueSets}");
                        _writer.OpenScope();

                        WriteValueSet(vs);

                        // close namespace
                        _writer.CloseScope();

                        WriteFooter();
                    }
                //}
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
                _writer.WriteLineIndented($"{_accessModifier} static class {vsName.Substring(0, vsName.Length - 8)}Codes");
            }
            else
            {
                _writer.WriteLineIndented($"{_accessModifier} static class {vsName}Codes");
            }

            // class
            _writer.OpenScope();

            bool prefixWithSystem = vs.ReferencedCodeSystems.Count > 1;
            HashSet<string> usedValues = new HashSet<string>();

            Dictionary<string, string> literals = new ();
            Dictionary<string, string> literalsWithSystemLookup = new ();

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

                string codeSystem = FhirUtils.SanitizeForQuoted(concept.System);
                string codeSystemNameSanitized = FhirUtils.SanitizeForProperty(concept.SystemLocalName, _reservedWords);
                string codeName = FhirUtils.SanitizeForProperty(input, _reservedWords);
                string codeValue = FhirUtils.SanitizeForValue(concept.Code);

                codeName = FhirUtils.SanitizedToConvention(codeName, FhirTypeBase.NamingConvention.PascalCase);
                codeSystemNameSanitized = FhirUtils.SanitizedToConvention(codeSystemNameSanitized, FhirTypeBase.NamingConvention.PascalCase);

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

                literals.Add(name, codeValue);
                literals.Add(codeSystemNameSanitized + name, codeSystem + "#" + codeValue);
                literalsWithSystemLookup.Add(codeSystem + "#" + codeValue, name);

                if (!string.IsNullOrEmpty(concept.Definition))
                {
                    WriteIndentedComment(concept.Definition);
                }
                else
                {
                    WriteIndentedComment(concept.Display);
                }

                if (_namesRequiringKeywordNew.Contains(name))
                {
                    _writer.WriteLineIndented($"{_accessModifier} static readonly new Coding {name} = new Coding");
                }
                else
                {
                    _writer.WriteLineIndented($"{_accessModifier} static readonly Coding {name} = new Coding");
                }

                _writer.OpenScope();
                _writer.WriteLineIndented($"Code = \"{codeValue}\",");

                if (!string.IsNullOrEmpty(concept.Display))
                {
                    _writer.WriteLineIndented($"Display = \"{FhirUtils.SanitizeForQuoted(concept.Display)}\",");
                }

                _writer.WriteLineIndented($"System = \"{concept.System}\"");

                _writer.DecreaseIndent();
                _writer.WriteLineIndented("};");
            }

            foreach (KeyValuePair<string, string> kvp in literals)
            {
                _writer.WriteLine();
                WriteIndentedComment($"Literal for code: {kvp.Key}");
                _writer.WriteLineIndented($"public const string Literal{kvp.Key} = \"{kvp.Value}\";");
            }

            // values
            _writer.WriteLine();
            WriteIndentedComment($"Dictionary for looking up {vsName} Codings based on Codes");
            _writer.OpenScope("public static Dictionary<string, Coding> Values = new Dictionary<string, Coding>() {");
            foreach (KeyValuePair<string, string> kvp in literals)
            {
                if (literalsWithSystemLookup.ContainsKey(kvp.Value))
                {
                    _writer.WriteLineIndented($"{{ \"{kvp.Value}\", {literalsWithSystemLookup[kvp.Value]} }}, ");
                }
                else
                {
                    _writer.WriteLineIndented($"{{ \"{kvp.Value}\", {kvp.Key} }}, ");
                }
            }

            // Values
            _writer.CloseScope("};");

            // class
            _writer.CloseScope("};");
        }

        /// <summary>Writes the complexes.</summary>
        /// <param name="complexes"> The complexes.</param>
        /// <param name="isResource">(Optional) True if is resource, false if not.</param>
        private void WriteComplexes(
            IEnumerable<FhirComplex> complexes,
            bool isResource = false)
        {
            foreach (FhirComplex complex in complexes)
            {
                // create a filename for writing
                string filename = Path.Combine(_directoryModels, $"{complex.NameCapitalized}.cs");

                using (FileStream stream = new FileStream(filename, FileMode.Create))
                using (ExportStreamWriter writer = new ExportStreamWriter(stream))
                {
                    _writer = writer;

                    WriteHeader(false, true, false);

                    // open namespace
                    _writer.WriteLineIndented($"namespace {_namespaceModels}");
                    _writer.OpenScope();

                    WriteComplex(complex, isResource);

                    // close namespace
                    _writer.CloseScope();

                    WriteFooter();
                }
            }
        }

        /// <summary>Writes a JSON converter attribute.</summary>
        /// <param name="isResource">   True if is resource, false if not.</param>
        /// <param name="nameForExport">The name for export.</param>
        private void WriteJsonConverterAttribute(bool isResource, string nameForExport)
        {
            switch (nameForExport)
            {
                case "Resource":
                case "DomainResource":
                case "MetadataResource":
                case "CanonicalResource":
                    _writer.WriteLineIndented($"[JsonConverter(typeof({_namespaceSerialization}.JsonStreamResourceConverter))]");
                    break;

                default:
                    _writer.WriteLineIndented($"[JsonConverter(typeof({_namespaceSerialization}.JsonStreamComponentConverter<{nameForExport}>))]");
                    break;
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

                WriteJsonConverterAttribute(isResource, nameForExport);
                _writer.WriteLineIndented($"{_accessModifier} class {nameForExport} : IFhirJsonSerializable {{");
            }
            else if (complex.Name.Equals(complex.BaseTypeName, StringComparison.Ordinal))
            {
                nameForExport = complex.NameForExport(FhirTypeBase.NamingConvention.PascalCase, true);
                hasBaseClass = false;
                baseClassName = string.Empty;

                WriteJsonConverterAttribute(isResource, nameForExport);
                _writer.WriteLineIndented(
                    $"{_accessModifier} class" +
                        $" {nameForExport}" +
                        $" :" +
                        $" IFhirJsonSerializable {{");
            }
            else if ((complex.Components != null) && complex.Components.ContainsKey(complex.Path))
            {
                nameForExport = complex.NameForExport(FhirTypeBase.NamingConvention.PascalCase, true);
                hasBaseClass = true;
                baseClassName = complex.TypeForExport(FhirTypeBase.NamingConvention.PascalCase, _primitiveTypeMap, false);

                WriteJsonConverterAttribute(isResource, nameForExport);
                _writer.WriteLineIndented(
                    $"{_accessModifier} class" +
                        $" {nameForExport}" +
                        $" :" +
                        $" {baseClassName}, " +
                        $" IFhirJsonSerializable {{");
            }
            else
            {
                nameForExport = complex.NameForExport(FhirTypeBase.NamingConvention.PascalCase, true);
                hasBaseClass = true;
                baseClassName = complex.TypeForExport(FhirTypeBase.NamingConvention.PascalCase, _primitiveTypeMap);

                WriteJsonConverterAttribute(isResource, nameForExport);
                _writer.WriteLineIndented(
                    $"{_accessModifier} class" +
                        $" {nameForExport}" +
                        $" :" +
                        $" {baseClassName}, " +
                        $" IFhirJsonSerializable {{");
            }

            _writer.IncreaseIndent();

            if (isResource)
            {
                if (nameForExport == "Resource")
                {
                    WriteIndentedComment("Resource Type Name");
                    _writer.WriteLineIndented($"{_accessModifier} virtual string ResourceType => string.Empty;");
                }
                else if (ShouldWriteResourceName(nameForExport))
                {
                    _exportedResourceNamesAndTypes.Add(complex.Name, complex.Name);

                    WriteIndentedComment("Resource Type Name");
                    _writer.WriteLineIndented($"{_accessModifier} override string ResourceType => \"{complex.Name}\";");
                }
            }

            // write elements
            WriteElements(complex, out List<FhirElement> elementsWithCodes);

            WriteComplexJsonSerialization(complex, nameForExport, hasBaseClass, baseClassName, isResource);

            WriteComplexJsonDeserialization(complex, nameForExport, hasBaseClass, baseClassName);

            // close interface (type)
            _writer.CloseScope();

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

            if (codeName.Contains("[x]", StringComparison.OrdinalIgnoreCase))
            {
                codeName = codeName.Replace("[x]", string.Empty, StringComparison.OrdinalIgnoreCase);
            }

            if (_exportedCodes.Contains(codeName))
            {
                return;
            }

            _exportedCodes.Add(codeName);

            _writer.WriteLineIndented($"/// <summary>");
            _writer.WriteLineIndented($"/// Code Values for the {element.Path} field");
            _writer.WriteLineIndented($"/// </summary>");

            if (codeName.EndsWith("Codes", StringComparison.Ordinal))
            {
                _writer.WriteLineIndented($"public static class {codeName} {{");
            }
            else
            {
                _writer.WriteLineIndented($"public static class {codeName}Codes {{");
            }

            _writer.IncreaseIndent();

            List<string> sanitizedValues = new ();

            if (_info.TryGetValueSet(element.ValueSet, out FhirValueSet vs))
            {
                foreach (FhirConcept concept in vs.Concepts)
                {
                    FhirUtils.SanitizeForCode(concept.Code, _reservedWords, out string name, out string value);

                    _writer.WriteLineIndented($"public const string {name.ToUpperInvariant()} = \"{value}\";");
                    sanitizedValues.Add(value);
                }
            }
            else
            {
                foreach (string code in element.Codes)
                {
                    FhirUtils.SanitizeForCode(code, _reservedWords, out string name, out string value);

                    _writer.WriteLineIndented($"public const string {name.ToUpperInvariant()} = \"{value}\";");
                    sanitizedValues.Add(value);
                }
            }

            if (sanitizedValues.Count > 0)
            {
                _writer.OpenScope("public static HashSet<string> Values = new HashSet<string>() {");
                foreach (string value in sanitizedValues)
                {
                    _writer.WriteLineIndented($"\"{value}\",");
                }

                _writer.CloseScope("};");
            }

            _writer.DecreaseIndent();
            _writer.WriteLineIndented("}");
        }

        /// <summary>Writes serialization functions for a complex object.</summary>
        /// <exception cref="JsonException">Thrown when a JSON error condition occurs.</exception>
        /// <param name="complex">      The complex.</param>
        /// <param name="nameForExport">The name for export.</param>
        /// <param name="hasBaseClass"> True if has base class, false if not.</param>
        /// <param name="baseClassName">Name of the base class.</param>
        private void WriteComplexJsonSerialization(
            FhirComplex complex,
            string nameForExport,
            bool hasBaseClass,
            string baseClassName,
            bool isResource)
        {
            string keywordNew = hasBaseClass ? "new " : string.Empty;

            WriteIndentedComment("Serialize to a JSON object");

            _writer.WriteLineIndented($"{_accessModifier} {keywordNew}void SerializeJson(" +
                "Utf8JsonWriter writer, " +
                "JsonSerializerOptions options, " +
                "bool includeStartObject = true)");

            // open SerializeJson
            _writer.OpenScope();

            _writer.WriteLineIndented($"if (includeStartObject)");
            _writer.OpenScope();
            _writer.WriteLineIndented($"writer.WriteStartObject();");
            _writer.CloseScope();

            if (isResource && ShouldWriteResourceName(nameForExport))
            {
                WriteJsonSerializeElement("ResourceType", "resourceType", "string", false, "WriteString");
                _writer.WriteLine();
            }

            if (hasBaseClass)
            {
                _writer.WriteLineIndented($"(({_namespaceModels}.{baseClassName})this).SerializeJson(writer, options, false);");
                _writer.WriteLine();
            }

            WriteSerializeJsonElements(complex);

            _writer.WriteLineIndented($"if (includeStartObject)");
            _writer.OpenScope();
            _writer.WriteLineIndented($"writer.WriteEndObject();");
            _writer.CloseScope();

            // close SerializeJson
            _writer.CloseScope();
        }

        /// <summary>Writes a parse JSON property elements.</summary>
        /// <param name="complex">The complex.</param>
        private void WriteSerializeJsonElements(FhirComplex complex)
        {
            foreach (FhirElement element in complex.Elements.Values)
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

                bool isMultiTyped = values.Count > 1;

                foreach (KeyValuePair<string, string> kvp in values)
                {
                    bool isOptional = RequiresNullTest(kvp.Value, element.IsOptional || isMultiTyped);

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
                            WriteJsonSerializeElement(elementName, camel, kvp.Value, isOptional, "WriteBoolean");
                            break;
                        case "List<bool>":
                            WriteJsonSerializeListElement(elementName, camel, kvp.Value, "WriteBooleanValue");
                            break;
                        case "byte[]":
                            WriteJsonSerializeElement(elementName, camel, kvp.Value, isOptional, "WriteBase64String");
                            break;
                        case "List<byte[]>":
                            WriteJsonSerializeListElement(elementName, camel, kvp.Value, "WriteBase64StringValue");
                            break;
                        case "decimal":
                            WriteJsonSerializeElement(elementName, camel, kvp.Value, isOptional, "WriteNumber");
                            break;
                        case "List<decimal>":
                            WriteJsonSerializeListElement(elementName, camel, kvp.Value, "WriteNumberValue");
                            break;
                        case "Guid":
                            WriteJsonSerializeElement(elementName, camel, kvp.Value, isOptional, "WriteString");
                            break;
                        case "List<Guid>":
                            WriteJsonSerializeListElement(elementName, camel, kvp.Value, "WriteStringValue");
                            break;
                        case "int":
                            WriteJsonSerializeElement(elementName, camel, kvp.Value, isOptional, "WriteNumber");
                            break;
                        case "List<int>":
                            WriteJsonSerializeListElement(elementName, camel, kvp.Value, "WriteNumberValue");
                            break;
                        case "long":
                            WriteJsonSerializeElement(elementName, camel, kvp.Value, isOptional, "WriteString");
                            break;
                        case "List<long>":
                            WriteJsonSerializeListElement(elementName, camel, kvp.Value, "WriteStringValue");
                            break;
                        case "string":
                            WriteJsonSerializeElement(elementName, camel, kvp.Value, isOptional, "WriteString");
                            break;
                        case "List<string>":
                            WriteJsonSerializeListElement(elementName, camel, kvp.Value, "WriteStringValue");
                            break;
                        case "uint":
                            WriteJsonSerializeElement(elementName, camel, kvp.Value, isOptional, "WriteNumber");
                            break;
                        case "List<uint>":
                            WriteJsonSerializeListElement(elementName, camel, kvp.Value, "WriteNumberValue");
                            break;
                        default:
                            if (element.IsArray)
                            {
                                WriteJsonSerializeListElement(elementName, camel, kvp.Value, string.Empty);
                            }
                            else
                            {
                                WriteJsonSerializeElement(elementName, camel, kvp.Value, isOptional, string.Empty);
                            }

                            break;
                    }

                    if (RequiresExtension(kvp.Value))
                    {
                        if (element.IsArray)
                        {
                            WriteJsonSerializeListElement("_" + elementName, "_" + camel, "Element", string.Empty);
                        }
                        else
                        {
                            WriteJsonSerializeElement("_" + elementName, "_" + camel, "Element", true, string.Empty);
                        }
                    }
                }
            }
        }

        /// <summary>Writes the JSON serialization for an element.</summary>
        /// <param name="elementName">       Name of the element.</param>
        /// <param name="camel">             The camel.</param>
        /// <param name="elementType">       Type of the element.</param>
        /// <param name="isOptional">        True if is optional, false if not.</param>
        /// <param name="writerFunctionName">Name of the getter function.</param>
        private void WriteJsonSerializeElement(
            string elementName,
            string camel,
            string elementType,
            bool isOptional,
            string writerFunctionName)
        {
            switch (elementType)
            {
                case "Resource":
                case "DomainResource":
                case "MetadataResource":
                case "CanonicalResource":
                    if (isOptional)
                    {
                        _writer.WriteLineIndented($"if ({elementName} != null)");
                        _writer.OpenScope();
                        _writer.WriteLineIndented($"writer.WritePropertyName(\"{camel}\");");
                        _writer.WriteLineIndented($"JsonSerializer.Serialize<{_namespaceModels}.Resource>(" +
                            $"writer, " +
                            $"({_namespaceModels}.Resource){elementName}, " +
                            $"options);");
                        _writer.CloseScope();
                    }
                    else
                    {
                        _writer.WriteLineIndented($"writer.WritePropertyName(\"{camel}\");");
                        _writer.WriteLineIndented($"JsonSerializer.Serialize<{_namespaceModels}.Resource>(" +
                            $"writer, " +
                            $"({_namespaceModels}.Resource){elementName}, " +
                            $"options);");
                    }

                    break;

                case "byte[]":
                    if (isOptional)
                    {
                        _writer.WriteLineIndented($"if ({elementName} != null)");
                        _writer.OpenScope();
                        _writer.WriteLineIndented($"writer.WriteString(\"{camel}\", System.Convert.ToBase64String({elementName}));");
                        _writer.CloseScope();
                    }
                    else
                    {
                        _writer.WriteLineIndented($"writer.WriteString(\"{camel}\", System.Convert.ToBase64String({elementName}));");
                    }

                    break;

                // non-string types that are serialized as strings
                case "guid":
                case "integer64":
                case "int64":
                case "long":
                    if (isOptional)
                    {
                        _writer.WriteLineIndented($"if ({elementName} != null)");
                        _writer.OpenScope();
                        _writer.WriteLineIndented($"writer.WriteString(\"{camel}\", {elementName}.ToString());");
                        _writer.CloseScope();
                    }
                    else
                    {
                        _writer.WriteLineIndented($"writer.WriteString(\"{camel}\", {elementName}.ToString());");
                    }

                    break;

                default:
                    if (string.IsNullOrEmpty(writerFunctionName))
                    {
                        if (isOptional)
                        {
                            _writer.WriteLineIndented($"if ({elementName} != null)");
                            _writer.OpenScope();
                            _writer.WriteLineIndented($"writer.WritePropertyName(\"{camel}\");");
                            _writer.WriteLineIndented($"{elementName}.SerializeJson(writer, options);");
                            _writer.CloseScope();
                        }
                        else
                        {
                            _writer.WriteLineIndented($"writer.WritePropertyName(\"{camel}\");");
                            _writer.WriteLineIndented($"{elementName}.SerializeJson(writer, options);");
                        }
                    }
                    else
                    {
                        if (isOptional)
                        {
                            _writer.WriteLineIndented($"if ({elementName} != null)");
                            _writer.OpenScope();
                            _writer.WriteLineIndented($"writer.{writerFunctionName}(\"{camel}\", ({elementType}){elementName}!);");
                            _writer.CloseScope();
                        }
                        else if (elementType == "string")
                        {
                            _writer.WriteLineIndented($"if (!string.IsNullOrEmpty({elementName}))");
                            _writer.OpenScope();
                            _writer.WriteLineIndented($"writer.{writerFunctionName}(\"{camel}\", ({elementType}){elementName}!);");
                            _writer.CloseScope();
                        }
                        else
                        {
                            _writer.WriteLineIndented($"writer.{writerFunctionName}(\"{camel}\", {elementName});");
                        }
                    }

                    break;
            }

            _writer.WriteLine();
        }

        /// <summary>Writes the JSON serialization for a list of elements.</summary>
        /// <exception cref="JsonException">Thrown when a JSON error condition occurs.</exception>
        /// <param name="elementName">       Name of the element.</param>
        /// <param name="camel">             The camel.</param>
        /// <param name="elementType">       Type of the element.</param>
        /// <param name="writerFunctionName">Name of the getter function.</param>
        private void WriteJsonSerializeListElement(
            string elementName,
            string camel,
            string elementType,
            string writerFunctionName)
        {
            _writer.WriteLineIndented($"if (({elementName} != null) && ({elementName}.Count != 0))");
            _writer.OpenScope();
            _writer.WriteLineIndented($"writer.WritePropertyName(\"{camel}\");");
            _writer.WriteLineIndented($"writer.WriteStartArray();");
            _writer.WriteLine();

            switch (elementType)
            {
                case "Resource":
                case "DomainResource":
                case "MetadataResource":
                case "CanonicalResource":
                    _writer.WriteLineIndented($"foreach (dynamic resource in {elementName})");
                    _writer.OpenScope();
                    _writer.WriteLineIndented($"resource.SerializeJson(writer, options, true);");
                    _writer.CloseScope();

                    // _writer.WriteLineIndented($"foreach (Resource resource in {elementName})");
                    // _writer.WriteLineIndented($"((Resource)this).SerializeJson(writer, options, true);");
                    break;

                case "byte[]":
                    _writer.WriteLineIndented($"foreach (byte[] byteArr{elementName} in {elementName})");
                    _writer.OpenScope();
                    _writer.WriteLineIndented($"writer.WriteStringValue(System.Convert.ToBase64String(byteArr{elementName}));");
                    _writer.CloseScope();

                    break;

                case "guid":
                case "integer64":
                case "int64":
                case "long":
                    _writer.WriteLineIndented($"foreach (long longVal{elementName} in {elementName})");
                    _writer.OpenScope();
                    _writer.WriteLineIndented($"writer.WriteStringValue(longVal{elementName}.ToString());");
                    _writer.CloseScope();

                    break;

                default:
                    if (string.IsNullOrEmpty(writerFunctionName))
                    {
                        _writer.WriteLineIndented($"foreach ({elementType} val{elementName} in {elementName})");
                        _writer.OpenScope();
                        _writer.WriteLineIndented($"val{elementName}.SerializeJson(writer, options, true);");
                        _writer.CloseScope();
                    }
                    else
                    {
                        _writer.WriteLineIndented($"foreach ({elementType} val{elementName} in {elementName})");
                        _writer.OpenScope();
                        _writer.WriteLineIndented($"writer.{writerFunctionName}(val{elementName});");
                        _writer.CloseScope();
                    }

                    break;
            }

            _writer.WriteLine();
            _writer.WriteLineIndented($"writer.WriteEndArray();");
            _writer.CloseScope();
            _writer.WriteLine();
        }

        /// <summary>Writes the JSON serialization for an element.</summary>
        /// <param name="elementName">       Name of the element.</param>
        /// <param name="camel">             The camel.</param>
        /// <param name="elementType">       Type of the element.</param>
        /// <param name="isOptional">        True if is optional, false if not.</param>
        /// <param name="getterFunctionName">Name of the getter function.</param>
        private void WriteJsonElementLoadSimple(
            string elementName,
            string camel,
            string elementType,
            bool isOptional,
            string getterFunctionName)
        {
            switch (elementType)
            {
                case "Resource":
                case "DomainResource":
                case "MetadataResource":
                case "CanonicalResource":
                    _writer.WriteLineIndented($"if (root.TryGetProperty(\"{camel}\", out JsonElement je{elementName}))");
                    _writer.OpenScope();
                    _writer.WriteLineIndented($"{elementName} = new {_namespaceModels}.Resource();");
                    _writer.WriteLineIndented($"{elementName}.LoadFromJsonElements(je{elementName});");
                    _writer.CloseScope();

                    break;

                case "integer64":
                case "int64":
                case "long":
                    _writer.WriteLineIndented($"if (root.TryGetProperty(\"{camel}\", out JsonElement je{elementName}))");
                    _writer.OpenScope();
                    _writer.WriteLineIndented($"string strVal{elementName} = je{elementName}.GetString();");
                    _writer.WriteLineIndented($"if (long.TryParse(strVal{elementName}, out long longVal{elementName}))");
                    _writer.OpenScope();
                    _writer.WriteLineIndented($"{elementName} = longVal{elementName};");
                    _writer.CloseScope();
                    _writer.CloseScope();

                    break;

                default:
                    if (string.IsNullOrEmpty(getterFunctionName))
                    {
                        _writer.WriteLineIndented($"if (root.TryGetProperty(\"{camel}\", out JsonElement je{elementName}))");
                        _writer.OpenScope();
                        _writer.WriteLineIndented($"{elementName} = new {_namespaceModels}.{elementType}();");
                        _writer.WriteLineIndented($"{elementName}.LoadFromJsonElements(je{elementName});");
                        _writer.CloseScope();
                    }
                    else
                    {
                        _writer.WriteLineIndented($"if (root.TryGetProperty(\"{camel}\", out JsonElement je{elementName}))");
                        _writer.OpenScope();
                        _writer.WriteLineIndented($"{elementName} = je{elementName}.{getterFunctionName}();");
                        _writer.CloseScope();
                    }

                    break;
            }

            _writer.WriteLine();
        }

        /// <summary>Writes the JSON serialization for a list of elements.</summary>
        /// <exception cref="JsonException">Thrown when a JSON error condition occurs.</exception>
        /// <param name="elementName">       Name of the element.</param>
        /// <param name="camel">             The camel.</param>
        /// <param name="elementType">       Type of the element.</param>
        /// <param name="getterFunctionName">Name of the getter function.</param>
        private void WriteJsonElementLoadList(
            string elementName,
            string camel,
            string elementType,
            string getterFunctionName)
        {
            _writer.WriteLineIndented($"if (root.TryGetProperty(\"{camel}\", out JsonElement jList{elementName}))");
            _writer.OpenScope();
            _writer.WriteLineIndented($"int len = jList{elementName}.GetArrayLength();");

            _writer.WriteLineIndented($"if (len > 0)");
            _writer.OpenScope();
            _writer.WriteLineIndented($"{elementName} = new List<{elementType}>();");
            _writer.WriteLine();
            _writer.WriteLineIndented($"for (int i = 0; i < len; i++)");
            _writer.OpenScope();

            switch (elementType)
            {
                case "Resource":
                case "DomainResource":
                case "MetadataResource":
                case "CanonicalResource":
                    _writer.WriteLineIndented($"{_namespaceModels}.Resource item{elementName} = new {_namespaceModels}.Resource();");
                    _writer.WriteLineIndented($"item{elementName}.LoadFromJsonElements(jList{elementName}[i]);");
                    _writer.WriteLineIndented($"{elementName}.Add(item{elementName});");

                    break;

                case "integer64":
                case "int64":
                case "long":
                    _writer.WriteLineIndented($"string strVal{elementName} = jList{elementName}[i].GetString();");
                    _writer.WriteLineIndented($"if (long.TryParse(strVal{elementName}, out long longVal{elementName}))");
                    _writer.OpenScope();
                    _writer.WriteLineIndented($"{elementName}.Add(longVal{elementName});");
                    _writer.CloseScope();

                    break;

                default:
                    if (string.IsNullOrEmpty(getterFunctionName))
                    {
                        _writer.WriteLineIndented($"{_namespaceModels}.{elementType} item{elementName} = new {_namespaceModels}.{elementType}();");
                        _writer.WriteLineIndented($"item{elementName}.LoadFromJsonElements(jList{elementName}[i]);");
                        _writer.WriteLineIndented($"{elementName}.Add(item{elementName});");
                    }
                    else
                    {
                        _writer.WriteLineIndented($"{elementName}.Add(jList{elementName}[i].{getterFunctionName}());");
                    }

                    break;
            }

            _writer.CloseScope();
            _writer.CloseScope();
            _writer.CloseScope();
            _writer.WriteLine();
        }

        /// <summary>Writes deserialization functions for a a complex object.</summary>
        /// <exception cref="JsonException">Thrown when a JSON error condition occurs.</exception>
        /// <param name="complex">      The complex.</param>
        /// <param name="nameForExport">The name for export.</param>
        /// <param name="hasBaseClass"> True if has base class, false if not.</param>
        /// <param name="baseClassName">Name of the base class.</param>
        private void WriteComplexJsonDeserialization(
            FhirComplex complex,
            string nameForExport,
            bool hasBaseClass,
            string baseClassName)
        {
            WriteIndentedComment("Deserialize a JSON property");

            string keywordNew = hasBaseClass ? "new " : string.Empty;

            _writer.WriteLineIndented($"{_accessModifier} {keywordNew}void DeserializeJsonProperty(" +
                "ref Utf8JsonReader reader, " +
                "JsonSerializerOptions options, " +
                "string propertyName)");

            // open DeserializeJsonProperty
            _writer.OpenScope();

            _writer.WriteLineIndented("switch (propertyName)");

            // open switch
            _writer.OpenScope();

            WriteDeserializeJsonPropertyElements(complex);

            if (hasBaseClass)
            {
                _writer.WriteLineIndented($"default:");
                _writer.IncreaseIndent();
                _writer.WriteLineIndented($"(({_namespaceModels}.{baseClassName})this).DeserializeJsonProperty(ref reader, options, propertyName);");
                _writer.WriteLineIndented("break;");
                _writer.DecreaseIndent();
            }

            // close switch
            _writer.CloseScope();

            // close DeserializeJsonProperty
            _writer.CloseScope();
            _writer.WriteLine();

            WriteIndentedComment("Deserialize a JSON object");

            _writer.WriteLineIndented($"{_accessModifier} {keywordNew}void DeserializeJson(" +
                "ref Utf8JsonReader reader, " +
                "JsonSerializerOptions options)");

            // open DeserializeJson
            _writer.OpenScope();

            _writer.WriteLineIndented("string propertyName;");
            _writer.WriteLine();
            _writer.WriteLineIndented("while (reader.Read())");
            _writer.OpenScope();
            _writer.WriteLineIndented("if (reader.TokenType == JsonTokenType.EndObject)");
            _writer.OpenScope();
            _writer.WriteLineIndented("return;");
            _writer.CloseScope();
            _writer.WriteLine();
            _writer.WriteLineIndented("if (reader.TokenType == JsonTokenType.PropertyName)");
            _writer.OpenScope();
            _writer.WriteLineIndented("propertyName = reader.GetString();");

            // TODO: this is for debugging ONLY!
            // _writer.WriteLineIndented($"Console.WriteLine($\"Reading {nameForExport}.{{propertyName}}\");");
            _writer.WriteLineIndented("reader.Read();");
            _writer.WriteLineIndented("this.DeserializeJsonProperty(ref reader, options, propertyName);");
            _writer.CloseScope();
            _writer.CloseScope();
            _writer.WriteLine();

            _writer.WriteLineIndented("throw new JsonException();");

            // close DeserializeJsonProperty
            _writer.CloseScope();
        }

        /// <summary>Writes a parse JSON property elements.</summary>
        /// <param name="complex">The complex.</param>
        private void WriteDeserializeJsonPropertyElements(FhirComplex complex)
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
                            WriteJsonPropertyParseCase(elementName, camel, kvp.Value, "GetBoolean");
                            break;
                        case "List<bool>":
                            WriteJsonPropertyParseListCase(elementName, camel, kvp.Value, "GetBoolean");
                            break;
                        case "byte[]":
                            WriteJsonPropertyParseCase(elementName, camel, kvp.Value, "GetBytesFromBase64");
                            break;
                        case "List<byte[]>":
                            WriteJsonPropertyParseListCase(elementName, camel, kvp.Value, "GetBytesFromBase64");
                            break;
                        case "decimal":
                            WriteJsonPropertyParseCase(elementName, camel, kvp.Value, "GetDecimal");
                            break;
                        case "List<decimal>":
                            WriteJsonPropertyParseListCase(elementName, camel, kvp.Value, "GetDecimal");
                            break;
                        case "Guid":
                            WriteJsonPropertyParseCase(elementName, camel, kvp.Value, "GetGuid");
                            break;
                        case "List<Guid>":
                            WriteJsonPropertyParseListCase(elementName, camel, kvp.Value, "GetGuid");
                            break;
                        case "int":
                            WriteJsonPropertyParseCase(elementName, camel, kvp.Value, "GetInt32");
                            break;
                        case "List<int>":
                            WriteJsonPropertyParseListCase(elementName, camel, kvp.Value, "GetInt32");
                            break;
                        case "long":
                            WriteJsonPropertyParseCase(elementName, camel, kvp.Value, "GetInt64");
                            break;
                        case "List<long>":
                            WriteJsonPropertyParseListCase(elementName, camel, kvp.Value, "GetInt64");
                            break;
                        case "string":
                            WriteJsonPropertyParseCase(elementName, camel, kvp.Value, "GetString");
                            break;
                        case "List<string>":
                            WriteJsonPropertyParseListCase(elementName, camel, kvp.Value, "GetString");
                            break;
                        case "uint":
                            WriteJsonPropertyParseCase(elementName, camel, kvp.Value, "GetUInt32");
                            break;
                        case "List<uint>":
                            WriteJsonPropertyParseListCase(elementName, camel, kvp.Value, "GetUInt32");
                            break;
                        default:
                            if (element.IsArray)
                            {
                                WriteJsonPropertyParseListCase(elementName, camel, kvp.Value, string.Empty);
                            }
                            else
                            {
                                WriteJsonPropertyParseCase(elementName, camel, kvp.Value, string.Empty);
                            }

                            break;
                    }

                    if (RequiresExtension(kvp.Value))
                    {
                        if (element.IsArray)
                        {
                            WriteJsonPropertyParseListCase("_" + elementName, "_" + camel, "Element", string.Empty);
                        }
                        else
                        {
                            WriteJsonPropertyParseCase("_" + elementName, "_" + camel, "Element", string.Empty);
                        }
                    }
                }
            }
        }

        /// <summary>Writes a JSON property case simple.</summary>
        /// <param name="elementName">       Name of the element.</param>
        /// <param name="camel">             The camel.</param>
        /// <param name="elementType">       Type of the element.</param>
        /// <param name="getterFunctionName">Name of the getter function.</param>
        private void WriteJsonPropertyParseCase(
            string elementName,
            string camel,
            string elementType,
            string getterFunctionName)
        {
            _writer.WriteLineIndented($"case \"{camel}\":");
            _writer.IncreaseIndent();

            switch (elementType)
            {
                case "Resource":
                case "DomainResource":
                case "MetadataResource":
                case "CanonicalResource":
                    _writer.WriteLineIndented($"{elementName} = " +
                        $"JsonSerializer.Deserialize" +
                        $"<{_namespaceModels}.Resource>(ref reader, options);");
                    break;

                case "byte[]":
                    _writer.WriteLineIndented($"{elementName} = System.Convert.FromBase64String(reader.GetString());");
                    break;

                case "integer64":
                case "int64":
                case "long":
                    _writer.WriteLineIndented($"string strVal{elementName} = reader.GetString();");
                    _writer.WriteLineIndented($"if (long.TryParse(strVal{elementName}, out long longVal{elementName}))");
                    _writer.OpenScope();
                    _writer.WriteLineIndented($"{elementName} = longVal{elementName};");
                    _writer.CloseScope();
                    _writer.WriteLine();
                    break;

                default:
                    if (string.IsNullOrEmpty(getterFunctionName))
                    {
                        _writer.WriteLineIndented($"{elementName} = new {_namespaceModels}.{elementType}();");
                        _writer.WriteLineIndented($"{elementName}.DeserializeJson(ref reader, options);");
                    }
                    else
                    {
                        _writer.WriteLineIndented($"{elementName} = reader.{getterFunctionName}();");
                    }

                    break;
            }

            _writer.WriteLineIndented("break;");
            _writer.DecreaseIndent();
            _writer.WriteLine();
        }

        /// <summary>Writes a JSON property list case.</summary>
        /// <exception cref="JsonException">Thrown when a JSON error condition occurs.</exception>
        /// <param name="elementName">       Name of the element.</param>
        /// <param name="camel">             The camel.</param>
        /// <param name="elementType">       Type of the element.</param>
        /// <param name="getterFunctionName">Name of the getter function.</param>
        private void WriteJsonPropertyParseListCase(
            string elementName,
            string camel,
            string elementType,
            string getterFunctionName)
        {
            _writer.WriteLineIndented($"case \"{camel}\":");
            _writer.IncreaseIndent();
            _writer.WriteLineIndented($"if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))");
            _writer.OpenScope();
            _writer.WriteLineIndented("throw new JsonException();");
            _writer.CloseScope();
            _writer.WriteLine();
            _writer.WriteLineIndented($"{elementName} = new List<{elementType}>();");
            _writer.WriteLine();
            _writer.WriteLineIndented($"while (reader.TokenType != JsonTokenType.EndArray)");
            _writer.OpenScope();

            switch (elementType)
            {
                case "Resource":
                case "DomainResource":
                case "MetadataResource":
                case "CanonicalResource":
                    _writer.WriteLineIndented($"{_namespaceModels}.{elementType} resource = " +
                        $"JsonSerializer.Deserialize" +
                        $"<{_namespaceModels}.{elementType}>(ref reader, options);");
                    _writer.WriteLineIndented($"{elementName}.Add(resource);");
                    break;

                case "byte[]":
                    _writer.WriteLineIndented($"{elementName}.Add(System.Convert.FromBase64String(reader.GetString()));");
                    break;

                case "integer64":
                case "int64":
                case "long":
                    _writer.WriteLineIndented($"string strVal{elementName} = reader.GetString();");
                    _writer.WriteLineIndented($"if (long.TryParse(strVal{elementName}, out long longVal{elementName}))");
                    _writer.OpenScope();
                    _writer.WriteLineIndented($"{elementName}.Add(longVal{elementName});");
                    _writer.CloseScope();
                    break;

                default:
                    if (string.IsNullOrEmpty(getterFunctionName))
                    {
                        _writer.WriteLineIndented($"{_namespaceModels}.{elementType} obj{elementName} = new {_namespaceModels}.{elementType}();");
                        _writer.WriteLineIndented($"obj{elementName}.DeserializeJson(ref reader, options);");
                        _writer.WriteLineIndented($"{elementName}.Add(obj{elementName});");
                    }
                    else
                    {
                        _writer.WriteLineIndented($"{elementName}.Add(reader.{getterFunctionName}());");
                    }

                    break;
            }

            _writer.WriteLine();
            _writer.WriteLineIndented($"if (!reader.Read())");
            _writer.OpenScope();
            _writer.WriteLineIndented("throw new JsonException();");
            _writer.CloseScope();
            _writer.CloseScope();
            _writer.WriteLine();
            _writer.WriteLineIndented($"if ({elementName}.Count == 0)");
            _writer.OpenScope();
            _writer.WriteLineIndented($"{elementName} = null;");
            _writer.CloseScope();
            _writer.WriteLine();
            _writer.WriteLineIndented("break;");
            _writer.DecreaseIndent();
            _writer.WriteLine();
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

            bool isMultiTyped = values.Count > 1;

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
                    ((element.IsOptional || isMultiTyped) && (!element.IsArray) && IsNullable(kvp.Value))
                        ? "?"
                        : string.Empty;

                if (!string.IsNullOrEmpty(element.Comment))
                {
                    WriteIndentedComment(element.Comment);
                }

                string elementType = element.IsArray
                    ? $"List<{kvp.Value}{optionalFlagString}>"
                    : $"{kvp.Value}{optionalFlagString}";

                _writer.WriteLineIndented($"{_accessModifier} {elementType} {elementName} {{ get; set; }}");

                if (RequiresExtension(kvp.Value))
                {
                    WriteIndentedComment($"Extension container element for {kvp.Key}");

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
                case "long":
                case "Guid":
                    return true;
            }

            return false;
        }

        /// <summary>Tests requires null.</summary>
        /// <param name="typeName">         Name of the type.</param>
        /// <param name="flaggedAsOptional">True to flagged as optional.</param>
        /// <returns>True if the test passes, false if the test fails.</returns>
        private static bool RequiresNullTest(string typeName, bool flaggedAsOptional)
        {
            // nullable reference types are not allowed in current C#
            switch (typeName)
            {
                case "string":
                    return false;

                case "bool":
                case "byte[]":
                case "decimal":
                case "DateTime":
                case "int":
                case "uint":
                case "long":
                case "Guid":
                    return flaggedAsOptional;
            }

            return true;
        }

        /// <summary>Writes a header.</summary>
        /// <param name="isValueSet">     True if this is the header for a ValueSet.</param>
        /// <param name="isModel">        True if this is the header for a Model.</param>
        /// <param name="isSerialization">True if is serialization, false if not.</param>
        private void WriteHeader(bool isValueSet, bool isModel, bool isSerialization)
        {
            _writer.WriteLineIndented("// <auto-generated />");
            _writer.WriteLineIndented($"// Built from: {_info.PackageName} version: {_info.VersionString}");

            if ((_options.ExportList != null) && _options.ExportList.Any())
            {
                string restrictions = string.Join("|", _options.ExportList);
                _writer.WriteLineIndented($"  // Restricted to: {restrictions}");
            }

            if ((_options.LanguageOptions != null) && (_options.LanguageOptions.Count > 0))
            {
                foreach (KeyValuePair<string, string> kvp in _options.LanguageOptions)
                {
                    _writer.WriteLineIndented($"  // Option: \"{kvp.Key}\" = \"{kvp.Value}\"");
                }
            }

            _writer.WriteLine();

            if (isModel)
            {
                _writer.WriteLineIndented("using System;");
                _writer.WriteLineIndented("using System.Collections.Generic;");

                _writer.WriteLineIndented("using System.Text.Json;");
                _writer.WriteLineIndented("using System.Text.Json.Serialization;");

                _writer.WriteLineIndented("using System.Xml;");

                _writer.WriteLineIndented($"using {_namespaceSerialization};");
            }

            if (isValueSet)
            {
                _writer.WriteLineIndented($"using {_namespaceModels};");
            }

            if (isSerialization)
            {
                _writer.WriteLineIndented("using System;");
                _writer.WriteLineIndented("using System.Buffers;");
                _writer.WriteLineIndented("using System.Collections.Generic;");
                _writer.WriteLineIndented("using System.IO;");
                _writer.WriteLineIndented("using System.Text;");
                _writer.WriteLineIndented("using System.Text.Json;");
                _writer.WriteLineIndented("using System.Text.Json.Serialization;");
                _writer.WriteLineIndented("using System.Xml;");

                _writer.WriteLineIndented($"using {_namespaceModels};");
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
        }

        /// <summary>Writes an indented comment.</summary>
        /// <param name="value">    The value.</param>
        /// <param name="isSummary">(Optional) True if is summary, false if not.</param>
        private void WriteIndentedComment(string value, bool isSummary = true)
        {
            string comment;
            string[] lines;

            if (isSummary)
            {
                if (!_includeSummaries)
                {
                    return;
                }

                _writer.WriteLineIndented("/// <summary>");

                comment = value
                    .Replace('\r', '\n')
                    .Replace("\r\n", "\n", StringComparison.Ordinal)
                    .Replace("\n\n", "\n", StringComparison.Ordinal)
                    .Replace("&", "&amp;", StringComparison.Ordinal)
                    .Replace("<", "&lt;", StringComparison.Ordinal)
                    .Replace(">", "&gt;", StringComparison.Ordinal);

                lines = comment.Split('\n');
                foreach (string line in lines)
                {
                    _writer.WriteIndented("/// ");
                    _writer.WriteLine(line);
                }

                _writer.WriteLineIndented("/// </summary>");

                return;
            }

            comment = value
                .Replace('\r', '\n')
                .Replace("\r\n", "\n", StringComparison.Ordinal)
                .Replace("\n\n", "\n", StringComparison.Ordinal);

            lines = comment.Split('\n');
            foreach (string line in lines)
            {
                _writer.WriteIndented("// ");
                _writer.WriteLine(line);
            }
        }
    }
}
