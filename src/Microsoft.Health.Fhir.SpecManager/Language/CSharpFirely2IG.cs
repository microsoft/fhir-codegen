﻿// <copyright file="CSharpFirely2IG.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
using System.Diagnostics;
using System.IO;
using Microsoft.Health.Fhir.SpecManager.Manager;

namespace Microsoft.Health.Fhir.SpecManager.Language
{
    /// <summary>A language exporter for Firely-compliant C# FHIR JSON Serialization Extension output.</summary>
    public sealed class CSharpFirely2IG : ILanguage
    {
        /// <summary>The namespace to use during export.</summary>
        private const string _modelNamespace = "Hl7.Fhir.Model";

        /// <summary>The serialization namespace.</summary>
        private const string _serializationNamespace = "Hl7.Fhir.Serialization";

        /// <summary>FHIR information we are exporting.</summary>
        private FhirVersionInfo _info;

        /// <summary>Options for controlling the export.</summary>
        private ExporterOptions _options;

        /// <summary>Keep track of information about written value sets.</summary>
        private Dictionary<string, WrittenValueSetInfo> _writtenValueSets = new Dictionary<string, WrittenValueSetInfo>();

        /// <summary>The written resources.</summary>
        private readonly Dictionary<string, WrittenModelInfo> _writtenModels = new Dictionary<string, WrittenModelInfo>();

        /// <summary>The written converters.</summary>
        private readonly List<string> _writtenConverters = new List<string>();

        /// <summary>The split characters.</summary>
        private static readonly char[] _splitChars = { '|', ' ' };

        /// <summary>The currently in-use text writer.</summary>
        private ExportStreamWriter _writer;

        /// <summary>Pathname of the export directory.</summary>
        private string _exportDirectory;

        /// <summary>Name of the language.</summary>
        private const string _languageName = "CSharpFirely2IG";

        /// <summary>The single file export extension (uses directory export).</summary>
        private const string _singleFileExportExtension = null;

        /// <summary>Structures to skip generating.</summary>
        private static readonly HashSet<string> _exclusionSet = new HashSet<string>()
        {
            /* Since Base defines its methods abstractly, the pattern for generating it
             * is sufficiently different from derived classes that it makes sense not
             * to generate the methods (it's pretty empty too - no members on this abstract class) */
            // "Base",

            /* PrimitiveType defines the magic `ObjectValue` member used by all derived
             * primitives to store their value. This makes the CopyTo(), IsExact() methods
             * different enough that it does not make sense to generate them. */
            // "PrimitiveType",

            /* Element has the special `id` element, that is both an attribute in the
             * XML serialization and is not using a FHIR primitive for representation. Consequently,
             * the generated CopyTo() and IsExact() methods diverge too much to be useful. */
            // "Element",

            /* Extension has the special `url` element, that is both an attribute in the
             * XML serialization and is not using a FHIR primitive for representation. Consequently,
             * the generated CopyTo() and IsExact() methods diverge too much to be useful.
             * Also, it uses the special `IsOpen` argument to `AllowedTypes` to account for open
             * types *not* defined in common. */
            // "Extension",

            /* Narrative has a special `div` element, serialized as an element frm the
             * XHTML namespace, not using a normal FHIR primitive. This makes this class
             * deviate in ways we cannot achieve with the generator. */
            // "Narrative",

            /* These two types are interfaces rather than classes (at least, for now)
             * so we're not generating them. Also, all types deriving from these
             * are generated to derive from DomainResource instead */
            "CanonicalResource",
            "MetadataResource",

            /* Citation somehow generates incorrect code - there must be something new
             * going on with this resource type. For now, it has been disabled so we can
             * take a look at it later, before R5 ships. */
            "Citation",

            /* UCUM is used as a required binding in a codeable concept. Since we do not
             * use enums in this situation, it is not useful to generate this valueset
             */
            "http://hl7.org/fhir/ValueSet/ucum-units",
        };

        /// <summary>
        /// List of types introduced in R5 that are retrospectively introduced in R3 and R4.
        /// </summary>
        private static readonly List<WrittenModelInfo> _commonR5DataTypes = new List<WrittenModelInfo>
        {
            new WrittenModelInfo { CsName = "BackboneType", FhirName = "BackboneType", IsAbstract = true, IsResource = false },
            new WrittenModelInfo { CsName = "Base", FhirName = "Base", IsAbstract = true, IsResource = false },
            new WrittenModelInfo { CsName = "DataType", FhirName = "DataType", IsAbstract = true, IsResource = false },
            new WrittenModelInfo { CsName = "PrimitiveType", FhirName = "PrimitiveType", IsAbstract = true, IsResource = false },
        };

        /// <summary>
        /// List of complex datatype classes that are part of the 'common' subset. See <see cref="GenSubset"/>.
        /// </summary>
        private static readonly List<string> _commmonComplexTypes = new List<string>()
        {
            "BackboneElement",
            "BackboneType",
            "Base",
            "Coding",
            "DataType",
            "Element",
            "Extension",
            "Meta",
            "PrimitiveType",
            "Narrative",
        };

        /// <summary>
        /// List of resource classes that are part of the 'common' subset. See <see cref="GenSubset"/>.
        /// </summary>
        private static readonly List<string> _commmonResourceTypes = new List<string>()
        {
            "Resource",
        };

        /// <summary>Gets the reserved words.</summary>
        /// <value>The reserved words.</value>
        private static readonly HashSet<string> _reservedWords = new HashSet<string>();

        private static readonly Func<WrittenModelInfo, bool> SupportedResourcesFilter = wmi => !wmi.IsAbstract;
        private static readonly Func<WrittenModelInfo, bool> FhirToCsFilter = wmi => !excludeFromCsToFhir.Contains(wmi.FhirName);
        private static readonly Func<WrittenModelInfo, bool> CsToStringFilter = FhirToCsFilter;

        private static readonly string[] excludeFromCsToFhir =
        {
            "CanonicalResource",
            "MetadataResource",
            "Citation",
        };

        /// <summary>The elements for 'Text' style serialization.</summary>
        private static readonly HashSet<string> _elementsForText = new HashSet<string>()
        {
            "text",
            "id",
            "meta",
        };

        /// <summary>The elements for 'Count' style serialization.</summary>
        private static readonly HashSet<string> _elementsForCount = new HashSet<string>()
        {
            "id",
            "total",
        };

        private static readonly Dictionary<string, string> _elementNameReplacementsByPath = new Dictionary<string, string>()
        {
            { "Element.id", "ElementId" },
            { "Extension.url", "Url" },
            { "Narrative.div", "Div" },
        };

        /// <summary>
        /// Determines the subset of code to generate.
        /// </summary>
        [Flags]
        private enum GenSubset
        {
            // Subset of generated output for the 'fhir-net-common' repo
            Common = 1,

            // Subset of generated output for the 'fhir-net-api' repo
            Main = 2,

            // No subsetting, generate all
            All = Common | Main,
        }

        /// <summary>Values that represent null check types.</summary>
        private enum NullCheckType
        {
            // do not perform a null check
            None,

            // perform string-based null check
            String,

            // perform equality-style null check
            Equality,
        }

        /// <summary>Gets the name of the language.</summary>
        /// <value>The name of the language.</value>
        string ILanguage.LanguageName => _languageName;

        string ILanguage.Namespace
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the single file extension for this language - null or empty indicates a multi-file
        /// export (exporter should copy the contents of the directory).
        /// </summary>
        string ILanguage.SingleFileExportExtension => _singleFileExportExtension;

        /// <summary>Gets the FHIR primitive type map.</summary>
        /// <value>The FHIR primitive type map.</value>
        Dictionary<string, string> ILanguage.FhirPrimitiveTypeMap => CSharpFirelyCommon.PrimitiveTypeMap;

        /// <summary>Gets the reserved words.</summary>
        /// <value>The reserved words.</value>
        HashSet<string> ILanguage.ReservedWords => _reservedWords;

        /// <summary>
        /// Gets a list of FHIR class types that the language WILL export, regardless of user choices.
        /// Used to provide information to users.
        /// </summary>
        List<ExporterOptions.FhirExportClassType> ILanguage.RequiredExportClassTypes => new List<ExporterOptions.FhirExportClassType>();

        /// <summary>
        /// Gets a list of FHIR class types that the language CAN export, depending on user choices.
        /// </summary>
        List<ExporterOptions.FhirExportClassType> ILanguage.OptionalExportClassTypes => new List<ExporterOptions.FhirExportClassType>()
        {
            ExporterOptions.FhirExportClassType.PrimitiveType,
            ExporterOptions.FhirExportClassType.ComplexType,
            ExporterOptions.FhirExportClassType.Resource,
            ExporterOptions.FhirExportClassType.Enum,
        };

        /// <summary>Gets language-specific options and their descriptions.</summary>
        Dictionary<string, string> ILanguage.LanguageOptions => new Dictionary<string, string>();


        void ILanguage.Export(
            FhirVersionInfo info,
            FhirComplex complex,
            Stream outputStream)
            => throw new NotImplementedException();

        /// <summary>Export the passed FHIR version into the specified directory.</summary>
        /// <param name="info">           The information.</param>
        /// <param name="serverInfo">     Information describing the server.</param>
        /// <param name="options">        Options for controlling the operation.</param>
        /// <param name="exportDirectory">Directory to write files.</param>
        void ILanguage.Export(
            FhirVersionInfo info,
            FhirCapabiltyStatement serverInfo,
            ExporterOptions options,
            string exportDirectory)
        {
            // set internal vars so we don't pass them to every function
            _info = info;
            _options = options;
            _exportDirectory = exportDirectory;
            _writtenValueSets = new Dictionary<string, WrittenValueSetInfo>();

            if (!Directory.Exists(exportDirectory))
            {
                Directory.CreateDirectory(exportDirectory);
            }

            //foreach (FhirPrimitive model in _info.PrimitiveTypes.Values)
            //{
            //    WritePrimitiveType(model);
            //}

            foreach (FhirComplex model in _info.ComplexTypes.Values)
            {
                WriteComplexDataType(model);
            }

            foreach (FhirComplex model in _info.Resources.Values)
            {
                WriteResource(model);
            }

            WriteJsonSerializationHelpers();
        }

        /// <summary>Writes the JSON serialization helper files.</summary>
        private void WriteJsonSerializationHelpers()
        {
            WriteJsonSerializerOptions();

            WriteJsonStreamResourceConverter();

            WriteJsonStreamUtilities();
        }

        /// <summary>Writes the JSON serializer options.</summary>
        private void WriteJsonSerializerOptions()
        {
            // create a filename for writing
            string filename = Path.Combine(_exportDirectory, "Serialization", "FhirSerializerOptions.cs");

            using (FileStream stream = new FileStream(filename, FileMode.Create))
            using (ExportStreamWriter writer = new ExportStreamWriter(stream))
            {
                _writer = writer;

                WriteHeaderJsonExt();

                // open namespace
                _writer.WriteLineIndented($"namespace {_serializationNamespace}");
                _writer.OpenScope();

                // open class
                WriteIndentedComment("Default JsonSerializerOptions to format JSON serialization as expected.");
                _writer.WriteLineIndented($"public static class FhirSerializerOptions");
                _writer.OpenScope();

                // TODO(ginoc): For debugging ONLY
                WriteIndentedComment("Debug option for serialiation / parsing.");
                _writer.WriteLine("public static bool Debug { get; set; } = false;");

                _writer.WriteLine("#pragma warning disable CA1810 // Initialize reference type static fields inline");
                _writer.WriteLine();

                WriteIndentedComment("Converters list for use in options.");
                _writer.WriteLineIndented("private static readonly List<JsonConverter> _converters;");
                _writer.WriteLine();

                WriteIndentedComment("Serializer options for deserialization, or serializing in Compact format.");
                _writer.WriteLineIndented("private static readonly JsonSerializerOptions _serializerCompact;");
                _writer.WriteLine();

                WriteIndentedComment("Serializer options for deserialization, or serializing in Pretty format.");
                _writer.WriteLineIndented("private static readonly JsonSerializerOptions _serializerPretty;");
                _writer.WriteLine();

                WriteIndentedComment("Writer options for Compact format writing.");
                _writer.WriteLineIndented("private static readonly JsonWriterOptions _writerCompact;");
                _writer.WriteLine();

                WriteIndentedComment("Writer options for Pretty format writing.");
                _writer.WriteLineIndented("private static readonly JsonWriterOptions _writerPretty;");
                _writer.WriteLine();

                // open FhirSerializerOptions (static constructor)
                WriteIndentedComment("Initializes static members of the <see cref=\"FhirSerializerOptions\"/> class.");
                _writer.WriteLineIndented("static FhirSerializerOptions()");
                _writer.OpenScope();

                // open converters
                _writer.WriteLineIndented("_converters = new List<JsonConverter>()");
                _writer.OpenScope();

                // first converter is the generic resource converter
                _writer.WriteLineIndented("new JsonStreamResourceConverter(),");

                foreach (string converter in _writtenConverters)
                {
                    _writer.WriteLineIndented($"new {converter}(),");
                }

                // close Converters
                _writer.CloseScope("};");
                _writer.WriteLine();

                // _serializerCompact
                _writer.WriteLineIndented("_serializerCompact = new JsonSerializerOptions()");
                _writer.OpenScope();
                _writer.WriteLineIndented("Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,");
                _writer.WriteLineIndented("WriteIndented = false,");
                _writer.CloseScope("};");
                _writer.WriteLine();

                // _serializerPretty
                _writer.WriteLineIndented("_serializerPretty = new JsonSerializerOptions()");
                _writer.OpenScope();
                _writer.WriteLineIndented("Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,");
                _writer.WriteLineIndented("WriteIndented = true,");
                _writer.CloseScope("};");
                _writer.WriteLine();

                // add converters to Serializer options
                _writer.WriteLineIndented("foreach (JsonConverter converter in _converters)");
                _writer.OpenScope();
                _writer.WriteLineIndented("_serializerCompact.Converters.Add(converter);");
                _writer.WriteLineIndented("_serializerPretty.Converters.Add(converter);");
                _writer.CloseScope();

                // _writerCompact
                _writer.WriteLineIndented("_writerCompact = new JsonWriterOptions()");
                _writer.OpenScope();
                _writer.WriteLineIndented("Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,");
                _writer.WriteLineIndented("Indented = false,");
                _writer.CloseScope("};");
                _writer.WriteLine();

                // _writerPretty
                _writer.WriteLineIndented("_writerPretty = new JsonWriterOptions()");
                _writer.OpenScope();
                _writer.WriteLineIndented("Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,");
                _writer.WriteLineIndented("Indented = true,");
                _writer.CloseScope("};");
                _writer.WriteLine();

                // close FhirSerializerOptions (static constructor)
                _writer.CloseScope();

                _writer.WriteLine("#pragma warning restore CA1810 // Initialize reference type static fields inline");
                _writer.WriteLine();

                WriteIndentedComment("Converters list for polymorphic deserialization (use in options).");
                _writer.WriteLineIndented("public static List<JsonConverter> FhirConverters => _converters;");
                _writer.WriteLine();

                WriteIndentedComment("Serializer options for deserialization, or serializing in Compact format.");
                _writer.WriteLineIndented("public static JsonSerializerOptions SerializerCompact => _serializerCompact;");
                _writer.WriteLine();

                WriteIndentedComment("Serializer options for deserialization, or serializing in Pretty format.");
                _writer.WriteLineIndented("public static JsonSerializerOptions SerializerPretty => _serializerPretty;");
                _writer.WriteLine();

                WriteIndentedComment("Writer options for Compact format writing.");
                _writer.WriteLineIndented("public static JsonWriterOptions WriterCompact => _writerCompact;");
                _writer.WriteLine();

                WriteIndentedComment("Writer options for Pretty format writing.");
                _writer.WriteLineIndented("public static JsonWriterOptions WriterPretty => _writerPretty;");

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
            string filename = Path.Combine(_exportDirectory, "Serialization", "JsonStreamResourceConverter.cs");

            using (FileStream stream = new FileStream(filename, FileMode.Create))
            using (ExportStreamWriter writer = new ExportStreamWriter(stream))
            {
                _writer = writer;

                WriteHeaderJsonExt();

                // open namespace
                _writer.WriteLineIndented($"namespace {_serializationNamespace}");
                _writer.OpenScope();

                // open class
                WriteIndentedComment("Common resource converter to support polymorphic deserialization.");
                _writer.WriteLineIndented($"public class JsonStreamResourceConverter : JsonConverter<Resource>");
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
                _writer.WriteLineIndented($"public override bool CanConvert(Type objectType) =>");
                _writer.IncreaseIndent();
                _writer.WriteLineIndented("typeof(Resource).IsAssignableFrom(objectType);");
                _writer.DecreaseIndent();
                _writer.WriteLine();

                WriteIndentedComment("Writes a specified value as JSON.");
                _writer.WriteLineIndented($"public override void Write(" +
                    "Utf8JsonWriter writer, " +
                    "Resource resource, " +
                    "JsonSerializerOptions options)");

                // open Write
                _writer.OpenScope();

                _writer.WriteLineIndented("WriteResource(writer, resource, options);");

                // close Write
                _writer.CloseScope();

                WriteIndentedComment("Writes a specified Resource as JSON.");
                _writer.WriteLineIndented($"public static void WriteResource(" +
                    "Utf8JsonWriter writer, " +
                    "Resource resource, " +
                    "JsonSerializerOptions options)");

                // open WriteResource
                _writer.OpenScope();

                _writer.WriteLineIndented($"switch (resource)");

                // open switch
                _writer.OpenScope();

                // loop through our types
                foreach (KeyValuePair<string, WrittenModelInfo> kvp in _writtenModels)
                {
                    if ((!kvp.Value.IsResource) || kvp.Value.IsAbstract)
                    {
                        continue;
                    }

                    _writer.WriteLineIndented($"case {kvp.Value.CsName} typed{kvp.Value.CsName}:");
                    _writer.IncreaseIndent();
                    _writer.WriteLineIndented($"typed{kvp.Value.CsName}.SerializeJson(writer, options, true);");
                    _writer.WriteLineIndented("break;");
                    _writer.DecreaseIndent();
                }

                // close switch
                _writer.CloseScope();

                _writer.WriteLine();
                _writer.WriteLineIndented("writer.Flush();");

                // close WriteResource
                _writer.CloseScope();
                _writer.WriteLine();

                WriteIndentedComment("Reads and converts the JSON to a typed object.");
                _writer.WriteLineIndented($"public override Resource Read(" +
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
                _writer.WriteLineIndented($"public static Resource PolymorphicRead(" +
                    "ref Utf8JsonReader reader, " +
                    "Type typeToConvert, " +
                    "JsonSerializerOptions options)");

                // open PolymorphicRead
                _writer.OpenScope();

                _writer.WriteLineIndented("string propertyName = null;");
                _writer.WriteLineIndented("string resourceType = null;");
                _writer.WriteLine();
                _writer.WriteLineIndented("if (reader.TokenType == JsonTokenType.None)");
                _writer.OpenScope();
                _writer.WriteLineIndented("reader.Read();");
                _writer.CloseScope();
                _writer.WriteLine();
                _writer.WriteLineIndented("if (reader.TokenType != JsonTokenType.StartObject)");
                _writer.OpenScope();
                _writer.WriteLineIndented($"throw new JsonException($\"PolymorphicRead <<< error reading: expected StartObject, found {{reader.TokenType}}! depth: {{reader.CurrentDepth}}, pos: {{reader.BytesConsumed}}\");");
                _writer.CloseScope();
                _writer.WriteLine();
                _writer.WriteLineIndented("reader.Read();");
                _writer.WriteLineIndented("if (reader.TokenType != JsonTokenType.PropertyName)");
                _writer.OpenScope();
                _writer.WriteLineIndented($"throw new JsonException($\"PolymorphicRead <<< error reading: expected PropertyName, found {{reader.TokenType}}! depth: {{reader.CurrentDepth}}, pos: {{reader.BytesConsumed}}\");");
                _writer.CloseScope();
                _writer.WriteLine();

                _writer.WriteLineIndented("propertyName = reader.GetString();");
                _writer.WriteLineIndented("if (propertyName == \"resourceType\")");
                _writer.OpenScope();
                _writer.WriteLineIndented("reader.Read();");
                _writer.WriteLineIndented("if (reader.TokenType != JsonTokenType.String)");
                _writer.OpenScope();
                _writer.WriteLineIndented($"throw new JsonException($\"PolymorphicRead <<< error reading 'resourceType': expected String, found {{reader.TokenType}}! depth: {{reader.CurrentDepth}}, pos: {{reader.BytesConsumed}}\");");
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
                _writer.WriteLineIndented($"public static Resource DoPolymorphicRead(" +
                    "ref Utf8JsonReader reader, " +
                    "JsonSerializerOptions options, " +
                    "string resourceType)");

                // open DoPolymorphicRead
                _writer.OpenScope();

                _writer.WriteLineIndented("dynamic target;");
                _writer.WriteLineIndented("switch (resourceType)");

                // open switch
                _writer.OpenScope();

                // loop through our types
                foreach (KeyValuePair<string, WrittenModelInfo> kvp in _writtenModels)
                {
                    if ((!kvp.Value.IsResource) || kvp.Value.IsAbstract)
                    {
                        continue;
                    }

                    _writer.WriteLineIndented($"case \"{kvp.Key}\":");
                    _writer.IncreaseIndent();
                    _writer.WriteLineIndented($"target = new {_modelNamespace}.{kvp.Value.CsName}();");
                    _writer.WriteLineIndented($"(({_modelNamespace}.{kvp.Value.CsName})target).DeserializeJson(ref reader, options);");
                    _writer.WriteLineIndented("break;");
                    _writer.DecreaseIndent();
                }

                // default case returns a Resource object
                _writer.WriteLineIndented("default:");
                _writer.IncreaseIndent();
                _writer.WriteLineIndented($"throw new Exception($\"Cannot parse resource type: {{resourceType}}\");");
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

        /// <summary>Writes the JSON stream utilities.</summary>
        private void WriteJsonStreamUtilities()
        {
            // create a filename for writing
            string filename = Path.Combine(_exportDirectory, "Serialization", "JsonStreamUtilities.cs");

            using (FileStream stream = new FileStream(filename, FileMode.Create))
            using (ExportStreamWriter writer = new ExportStreamWriter(stream))
            {
                _writer = writer;

                WriteHeaderJsonExt();

                // open namespace
                _writer.WriteLineIndented($"namespace {_serializationNamespace}");
                _writer.OpenScope();

                // open class
                WriteIndentedComment("Common utilties for JSON Stream functionality.");
                _writer.WriteLineIndented($"public static class JsonStreamUtilities");
                _writer.OpenScope();

                // function SerializeExtensionList
                _writer.WriteLineIndented("/// <summary>");
                _writer.WriteLineIndented("/// Serialize an extension list.");
                _writer.WriteLineIndented("/// </summary>");
                _writer.WriteLineIndented("/// <param name=\"writer\">      The writer.</param>");
                _writer.WriteLineIndented("/// <param name=\"options\">     Options for controlling the operation.</param>");
                _writer.WriteLineIndented("/// <param name=\"propertyName\">Name of the property.</param>");
                _writer.WriteLineIndented("/// <param name=\"isArray\">     True if is array, false if not.</param>");
                _writer.WriteLineIndented("/// <param name=\"extensions\">  The extensions.</param>");
                _writer.WriteLineIndented("/// <param name=\"elementId\">   The element id.</param>");
                _writer.WriteLineIndented("public static void SerializeExtensionList(");
                _writer.IncreaseIndent();
                _writer.WriteLineIndented("Utf8JsonWriter writer,");
                _writer.WriteLineIndented("JsonSerializerOptions options,");
                _writer.WriteLineIndented("string propertyName,");
                _writer.WriteLineIndented("bool isArray,");
                _writer.WriteLineIndented("List<Extension> extensions,");
                _writer.WriteLineIndented("string elementId)");
                _writer.DecreaseIndent();
                _writer.OpenScope();

                _writer.WriteLineIndented("if (((extensions == null) || (extensions.Count == 0)) && (string.IsNullOrEmpty(elementId)))");
                _writer.OpenScope();
                _writer.WriteLineIndented("return;");
                _writer.CloseScope();
                _writer.WriteLine();

                _writer.WriteLineIndented("if (!isArray) { writer.WritePropertyName(propertyName); }");
                _writer.WriteLineIndented("writer.WriteStartObject();");

                _writer.WriteLineIndented("if (!string.IsNullOrEmpty(elementId)) { writer.WriteString(\"id\",elementId); }");

                _writer.WriteLineIndented("if ((extensions != null) && (extensions.Count > 0))");
                _writer.OpenScope();
                _writer.WriteLineIndented("writer.WritePropertyName(\"extension\");");
                _writer.WriteLineIndented("writer.WriteStartArray();");
                _writer.WriteLine();
                _writer.WriteLineIndented("foreach (Extension ext in extensions)");
                _writer.OpenScope();
                _writer.WriteLineIndented("ext.SerializeJson(writer, options, true);");
                _writer.CloseScope();
                _writer.WriteLine();
                _writer.WriteLineIndented("writer.WriteEndArray();");
                _writer.CloseScope();

                _writer.WriteLineIndented("writer.WriteEndObject();");

                // close SerializeExtensionList
                _writer.CloseScope();

                // close class
                _writer.CloseScope();

                // close namespace
                _writer.CloseScope();

                WriteFooter();
            }
        }

        /// <summary>Writes a complex data type.</summary>
        /// <param name="complex">      The complex data type.</param>
        private void WriteResource(
            FhirComplex complex)
        {
            string exportName = complex.NameForExport(FhirTypeBase.NamingConvention.PascalCase);

            string filename = Path.Combine(_exportDirectory, "Model", $"{exportName}.cs");

            _writtenModels.Add(complex.Name, new WrittenModelInfo()
            {
                CsName = exportName,
                FhirName = complex.Name,
                IsAbstract = complex.IsAbstract,
                IsResource = true,
            });

            using (FileStream stream = new FileStream(filename, FileMode.Create))
            using (ExportStreamWriter writer = new ExportStreamWriter(stream))
            {
                _writer = writer;

                WriteHeaderJsonExt();

                WriteNamespaceOpen();

                WriteComponent(
                    complex,
                    exportName,
                    DetermineExportedBaseTypeName(complex.Path, complex.Name, complex.BaseTypeName, false),
                    true,
                    0);

                WriteNamespaceClose();

                WriteFooter();
            }
        }

        /// <summary>Writes a complex data type.</summary>
        /// <param name="complex">      The complex data type.</param>
        private void WriteComplexDataType(
            FhirComplex complex)
        {
            string exportName = complex.NameForExport(FhirTypeBase.NamingConvention.PascalCase);

            if (CSharpFirelyCommon.TypeNameMappings.ContainsKey(exportName))
            {
                exportName = CSharpFirelyCommon.TypeNameMappings[exportName];
            }

            string baseExportName = DetermineExportedBaseTypeName(complex.Path, complex.Name, complex.BaseTypeName, true);

            string filename = Path.Combine(_exportDirectory, "Model", $"{exportName}.cs");

            _writtenModels.Add(complex.Name, new WrittenModelInfo()
            {
                CsName = exportName,
                FhirName = complex.Name,
                IsAbstract = complex.IsAbstract,
                IsResource = false,
            });

            using (FileStream stream = new FileStream(filename, FileMode.Create))
            using (ExportStreamWriter writer = new ExportStreamWriter(stream))
            {
                _writer = writer;

                WriteHeaderJsonExt();

                WriteNamespaceOpen();

                WriteComponent(complex, exportName, baseExportName, false, 0);

                WriteNamespaceClose();

                WriteFooter();
            }
        }

        /// <summary>Writes a component.</summary>
        /// <param name="complex">       The complex data type.</param>
        /// <param name="exportName">    Name of the export.</param>
        /// <param name="baseExportName">Name of the base export.</param>
        /// <param name="isResource">    True if is resource, false if not.</param>
        /// <param name="depth">         The depth.</param>
        private void WriteComponent(
            FhirComplex complex,
            string exportName,
            string baseExportName,
            bool isResource,
            int depth)
        {
            bool isAbstract = complex.IsAbstract;

            // process enums for this type
            ProcessEnums(complex, exportName);

            // open class
            OpenExtensionClass(complex.Name, exportName);

            // open SerializeJson
            OpenSerializeJson(complex.Name, exportName);

            _writer.WriteLineIndented("if (includeStartObject) { writer.WriteStartObject(); }");

            if (isResource && (!complex.IsAbstract))
            {
                _writer.WriteLineIndented($"writer.WriteString(\"resourceType\",\"{complex.Name}\");");
            }

            if ((!string.IsNullOrEmpty(baseExportName)) &&
                (baseExportName != exportName) &&
                (baseExportName != "DataType"))
            {
                _writer.WriteLineIndented($"// Complex: {complex.Name}, Export: {exportName}, Base: {complex.BaseTypeName} ({baseExportName})");

                _writer.WriteLineIndented($"(({_modelNamespace}.{baseExportName})current).SerializeJson(writer, options, false);");
                _writer.WriteLine();
            }

            WriteJsonSerializeElements(complex, exportName);

            _writer.WriteLineIndented(
                $"if (includeStartObject)" +
                $" {{" +
                $" writer.WriteEndObject();" +
                $" }}");

            // close SerializeJson
            CloseScope();

            // Write DeserializeJson
            WriteDeserializeJson(complex.Name, exportName);

            // open DeserializeJsonProperty
            OpenDeserializeJsonProperty(complex.Name, exportName);

            // open switch
            _writer.WriteLineIndented("switch (propertyName)");
            _writer.OpenScope();

            WriteDeserializeJsonElements(complex, exportName);

            if ((!string.IsNullOrEmpty(baseExportName)) &&
                 (baseExportName != exportName) &&
                 (baseExportName != "DataType"))
            {
                _writer.WriteLineIndented($"// Complex: {complex.Name}, Export: {exportName}, Base: {complex.BaseTypeName}");
                _writer.WriteLineIndented($"default:");
                _writer.IncreaseIndent();
                _writer.WriteLineIndented($"(({_modelNamespace}.{baseExportName})current).DeserializeJsonProperty(ref reader, options, propertyName);");
                _writer.WriteLineIndented("break;");
                _writer.DecreaseIndent();
            }

            // close switch
            _writer.CloseScope();

            // close DeserializeJsonProperty
            CloseScope();

            // check for nested components
            if (complex.Components != null)
            {
                foreach (FhirComplex component in complex.Components.Values)
                {
                    string componentExportName;

                    if (string.IsNullOrEmpty(component.ExplicitName))
                    {
                        componentExportName =
                            $"{component.NameForExport(FhirTypeBase.NamingConvention.PascalCase)}Component";
                    }
                    else
                    {
                        // Consent.provisionActorComponent is explicit lower case...
                        componentExportName =
                            $"{component.ExplicitName}" +
                            $"Component";
                    }

                    WriteBackboneComponent(
                        component,
                        componentExportName,
                        exportName,
                        isResource,
                        depth + 1);
                }
            }

            if (!complex.IsAbstract)
            {
                // write our component converter
                WriteComponentConverter(exportName);
            }

            // close class
            CloseScope();
        }

        /// <summary>Writes the JSON serializer component converter.</summary>
        /// <exception cref="JsonException">Thrown when a JSON error condition occurs.</exception>
        private void WriteComponentConverter(
            string exportName)
        {
            _writtenConverters.Add($"{exportName}JsonExtensions.{exportName}JsonConverter");

            // open class
            WriteIndentedComment("Resource converter to support System.Text.Json interop.");
            _writer.WriteLineIndented($"public class {exportName}JsonConverter : JsonConverter<{exportName}>");
            _writer.OpenScope();

            // function CanConvert
            //WriteIndentedComment("Determines whether the specified type can be converted.");
            //_writer.WriteLineIndented($"public override bool CanConvert(Type objectType) =>");
            //_writer.IncreaseIndent();
            //_writer.WriteLineIndented($"typeof({exportName}).IsAssignableFrom(objectType);");
            //_writer.DecreaseIndent();
            //_writer.WriteLine();

            // open Write
            WriteIndentedComment("Writes a specified value as JSON.");
            _writer.WriteLineIndented($"public override void Write(" +
                $"Utf8JsonWriter writer, " +
                $"{exportName} value, " +
                $"JsonSerializerOptions options)");
            _writer.OpenScope();

            _writer.WriteLineIndented($"value.SerializeJson(writer, options, true);");
            _writer.WriteLineIndented("writer.Flush();");

            // close Write
            _writer.CloseScope();

            // open Read
            WriteIndentedComment("Reads and converts the JSON to a typed object.");
            _writer.WriteLineIndented($"public override {exportName} Read(" +
                $"ref Utf8JsonReader reader, " +
                $"Type typeToConvert, " +
                $"JsonSerializerOptions options)");
            _writer.OpenScope();

            _writer.WriteLineIndented($"{exportName} target = new {exportName}();");
            _writer.WriteLineIndented("target.DeserializeJson(ref reader, options);");
            _writer.WriteLineIndented("return target;");

            // close Read
            _writer.CloseScope();

            // close class
            _writer.CloseScope();
        }

        /// <summary>Determine exported base type name.</summary>
        /// <param name="path">        Full pathname of the file.</param>
        /// <param name="name">        The name.</param>
        /// <param name="baseTypeName">Name of the base type.</param>
        /// <param name="isDataType">  True if is data type, false if not.</param>
        /// <returns>A string.</returns>
        private string DetermineExportedBaseTypeName(string path, string name, string baseTypeName, bool isDataType)
        {
            if (name == "Extension")
            {
                return "Element";
            }

            // These two classes are more like interfaces, we treat their subclasses
            // as subclasses of DomainResource instead.
            if (baseTypeName == "MetadataResource" || baseTypeName == "CanonicalResource")
            {
                return "DomainResource";
            }

            if (_info.FhirSequence < FhirPackageCommon.FhirSequenceEnum.R5)
            {
                // Promote R4 datatypes (all derived from Element/BackboneElement) to the right new subclass
                if ((baseTypeName == "BackboneElement") && (_info.FhirSequence == FhirPackageCommon.FhirSequenceEnum.R4) && isDataType)
                {
                    return "BackboneType";
                }

                //if (baseTypeName == "Element")
                //{
                //    if (path.StartsWith("ElementDefinition", StringComparison.Ordinal))
                //    {
                //        return "Element";
                //    }

                //    return "DataType";
                //}
            }

            return baseTypeName;
        }

        /// <summary>Writes a component.</summary>
        /// <param name="complex">              The complex data type.</param>
        /// <param name="exportName">           Name of the export.</param>
        /// <param name="parentExportName">     Name of the parent export.</param>
        /// <param name="isResource">           True if is resource, false if not.</param>
        /// <param name="depth">                The depth.</param>
        private void WriteBackboneComponent(
            FhirComplex complex,
            string exportName,
            string parentExportName,
            bool isResource,
            int depth)
        {
            string componentName = parentExportName + "#" + (string.IsNullOrEmpty(complex.ExplicitName) ?
                complex.NameForExport(FhirTypeBase.NamingConvention.PascalCase) :
                complex.ExplicitName);

            string baseExportName = DetermineExportedBaseTypeName(complex.Path, complex.Name, complex.BaseTypeName, false);

            // open SerializeJson
            OpenSerializeJson(componentName, exportName, parentExportName);

            _writer.WriteLineIndented(
                $"if (includeStartObject)" +
                $" {{" +
                $" writer.WriteStartObject();" +
                $" }}");

            if ((!string.IsNullOrEmpty(baseExportName)) &&
                (baseExportName != exportName) &&
                (baseExportName != "DataType"))
            {
                _writer.WriteLineIndented($"// Component: {componentName}, Export: {exportName}, Base: {complex.BaseTypeName} ({baseExportName})");
                _writer.WriteLineIndented($"(({_modelNamespace}.{baseExportName})current).SerializeJson(writer, options, false);");
                _writer.WriteLine();
            }

            WriteJsonSerializeElements(complex, exportName);

            _writer.WriteLineIndented(
                $"if (includeStartObject)" +
                $" {{" +
                $" writer.WriteEndObject();" +
                $" }}");

            // close SerializeJson
            CloseScope();

            // Write DeserializeJson
            WriteDeserializeJson(componentName, exportName, parentExportName, true);

            // open DeserializeJsonProperty
            OpenDeserializeJsonProperty(componentName, exportName, parentExportName, true);

            // open switch
            _writer.WriteLineIndented("switch (propertyName)");
            _writer.OpenScope();

            WriteDeserializeJsonElements(complex, exportName);

            if ((!string.IsNullOrEmpty(baseExportName)) &&
                 (baseExportName != exportName) &&
                 (baseExportName != "DataType"))
            {
                _writer.WriteLineIndented($"// Complex: {complex.Name}, Export: {exportName}, Base: {complex.BaseTypeName}");
                _writer.WriteLineIndented($"default:");
                _writer.IncreaseIndent();
                _writer.WriteLineIndented($"(({_modelNamespace}.{baseExportName})current).DeserializeJsonProperty(ref reader, options, propertyName);");
                _writer.WriteLineIndented("break;");
                _writer.DecreaseIndent();
            }

            // close switch
            _writer.CloseScope();

            // close DeserializeJsonProperty
            CloseScope();

            // check for nested components
            if (complex.Components != null)
            {
                foreach (FhirComplex component in complex.Components.Values)
                {
                    string componentExportName;

                    if (string.IsNullOrEmpty(component.ExplicitName))
                    {
                        componentExportName =
                            $"{component.NameForExport(FhirTypeBase.NamingConvention.PascalCase)}" +
                            $"Component";
                    }
                    else
                    {
                        componentExportName =
                            $"{component.ExplicitName}" +
                            $"Component";
                    }

                    WriteBackboneComponent(
                        component,
                        componentExportName,
                        parentExportName,
                        isResource,
                        depth + 1);
                }
            }
        }

        /// <summary>Writes the enums.</summary>
        /// <param name="complex">      The complex data type.</param>
        /// <param name="className">    Name of the class this enum is being written in.</param>
        /// <param name="usedEnumNames">(Optional) List of names of the used enums.</param>
        private void ProcessEnums(
            FhirComplex complex,
            string className,
            HashSet<string> usedEnumNames = null)
        {
            if (usedEnumNames == null)
            {
                usedEnumNames = new HashSet<string>();
            }

            if (complex.Elements != null)
            {
                foreach (FhirElement element in complex.Elements.Values)
                {
                    if ((!string.IsNullOrEmpty(element.ValueSet)) &&
                        (element.BindingStrength == "required") &&
                        _info.TryGetValueSet(element.ValueSet, out FhirValueSet vs))
                    {
                        ProcessEnum(vs, className, usedEnumNames);

                        continue;
                    }
                }
            }

            if (complex.Components != null)
            {
                foreach (FhirComplex component in complex.Components.Values)
                {
                    ProcessEnums(component, className, usedEnumNames);
                }
            }
        }

        /// <summary>Writes a value set as an enum.</summary>
        /// <param name="vs">       The vs.</param>
        /// <param name="className">Name of the class this enum is being written in.</param>
        private void ProcessEnum(
            FhirValueSet vs,
            string className,
            HashSet<string> usedEnumNames)
        {
            if (_writtenValueSets.ContainsKey(vs.URL))
            {
                return;
            }

            if (_exclusionSet.Contains(vs.URL))
            {
                return;
            }

            if (vs.ReferencedByComplexes.Count > 1)
            {
                // common value set
                className = string.Empty;
            }

            if (vs.StrongestBinding != FhirElement.ElementDefinitionBindingStrength.Required)
            {
                /* Since required bindings cannot be extended, those are the only bindings that
                   can be represented using enums in the POCO classes (using <c>Code&lt;T&gt;</c>). All other coded members
                   use <c>Code</c>, <c>Coding</c> or <c>CodeableConcept</c>.
                   Consequently, we only need to generate enums for valuesets that are used as
                   required bindings anywhere in the datamodel. */
                return;
            }

            string name = (vs.Name ?? vs.Id).Replace(" ", string.Empty, StringComparison.Ordinal).Replace("_", string.Empty, StringComparison.Ordinal);
            string nameSanitized = FhirUtils.SanitizeForProperty(name, _reservedWords);

            if (usedEnumNames.Contains(nameSanitized))
            {
                return;
            }

            usedEnumNames.Add(nameSanitized);

            _writtenValueSets.Add(
                vs.URL,
                new WrittenValueSetInfo()
                {
                    ClassName = className,
                    ValueSetName = nameSanitized,
                });
        }

        /// <summary>Writes a deserialize JSON elements.</summary>
        /// <param name="complex">            The complex data type.</param>
        /// <param name="exportedComplexName">Name of the exported complex parent.</param>
        private void WriteDeserializeJsonElements(
            FhirComplex complex,
            string exportedComplexName)
        {
            foreach (FhirElement element in complex.Elements.Values.OrderBy(e => e.FieldOrder))
            {
                if (element.IsInherited)
                {
                    continue;
                }

                //if (element.Path == "Timing.event")
                //{
                //    Console.Write("");
                //}

                string typeName = element.BaseTypeName;

                if (string.IsNullOrEmpty(typeName) &&
                    (element.ElementTypes.Count == 1))
                {
                    typeName = element.ElementTypes.Values.First().Name;
                }

                WrittenElementInfo elementInfo;

                if (typeName == "code")
                {
                    elementInfo = BuildCodedElementInfo(element);
                }
                else
                {
                    elementInfo = BuildElementInfo(exportedComplexName, element);
                }

                string currentName = "current." + elementInfo.ExportedName;

                if (elementInfo.IsChoice)
                {
                    WriteJsonDeserializeChoiceElement(currentName, exportedComplexName, element, elementInfo);
                }
                else
                {
                    string csType;

                    if (elementInfo.IsList)
                    {
                        csType = elementInfo.ExportedListSubType.StartsWith(_modelNamespace, StringComparison.Ordinal)
                            ? elementInfo.ExportedListSubType.Substring(_modelNamespace.Length + 1)
                            : elementInfo.ExportedListSubType;
                    }
                    else
                    {
                        csType = elementInfo.ExportedType.StartsWith(_modelNamespace, StringComparison.Ordinal)
                            ? elementInfo.ExportedType.Substring(_modelNamespace.Length + 1)
                            : elementInfo.ExportedType;
                    }

                    switch (csType)
                    {
                        case "Resource":
                        case "DomainResource":
                        case "MetadataResource":
                        case "CanonicalResource":

                            _writer.WriteLineIndented($"case \"{elementInfo.FhirElementName}\":");
                            _writer.IncreaseIndent();

                            if (elementInfo.IsList)
                            {
                                _writer.WriteLineIndented($"if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))");
                                _writer.OpenScope();
                                _writer.WriteLineIndented($"throw new JsonException($\"{exportedComplexName} error reading '{elementInfo.FhirElementName}' expected StartArray, found {{reader.TokenType}}! depth: {{reader.CurrentDepth}}, pos: {{reader.BytesConsumed}}\");");
                                _writer.CloseScope();
                                _writer.WriteLine();
                                _writer.WriteLineIndented($"{currentName} = new List<{csType}>();");
                                _writer.WriteLine();
                                _writer.WriteLineIndented($"while (reader.TokenType != JsonTokenType.EndArray)");
                                _writer.OpenScope();

                                // _writer.WriteLineIndented($"{currentName}.Add(JsonSerializer.Deserialize<{_modelNamespace}.Resource>(ref reader, options));");
                                _writer.WriteLineIndented(
                                    $"{currentName}.Add(" +
                                        "JsonStreamResourceConverter.PolymorphicRead(" +
                                            "ref reader, typeof(Resource)," +
                                            "options));");

                                _writer.WriteLine();
                                _writer.WriteLineIndented($"if (!reader.Read())");
                                _writer.OpenScope();
                                _writer.WriteLineIndented($"throw new JsonException($\"{exportedComplexName} error reading '{elementInfo.FhirElementName}' array, read failed! depth: {{reader.CurrentDepth}}, pos: {{reader.BytesConsumed}}\");");
                                _writer.CloseScope();

                                _writer.WriteLineIndented($"if (reader.TokenType == JsonTokenType.EndObject) {{ reader.Read(); }}");

                                _writer.CloseScope();
                                _writer.WriteLine();
                                _writer.WriteLineIndented($"if ({currentName}.Count == 0)");
                                _writer.OpenScope();
                                _writer.WriteLineIndented($"{currentName} = null;");
                                _writer.CloseScope();
                            }
                            else
                            {
                                // _writer.WriteLineIndented($"{currentName} = JsonSerializer.Deserialize<{_modelNamespace}.Resource>(ref reader, options);");
                                _writer.WriteLineIndented(
                                    $"{currentName} = " +
                                    "JsonStreamResourceConverter.PolymorphicRead(" +
                                        "ref reader," +
                                        "typeof(Resource)," +
                                        "options);");
                            }

                            _writer.WriteLineIndented("break;");
                            _writer.DecreaseIndent();
                            _writer.WriteLine();

                            break;

                        case "Base64Binary":
                            WriteJsonDeserializeElementPrimitive(
                                csType,
                                currentName,
                                elementInfo.FhirElementName,
                                exportedComplexName,
                                elementInfo.IsList,
                                "String",
                                "System.Convert.FromBase64String");

                            break;

                        case "Canonical":
                        case "Code":
                        case "Date":
                        case "DateTime":
                        case "FhirDateTime":
                        case "FhirString":
                        case "FhirUri":
                        case "FhirUrl":
                        case "Id":
                        case "Markdown":
                        case "Oid":
                        case "Uuid":
                        case "XHtml":
                            WriteJsonDeserializeElementPrimitive(
                                csType,
                                currentName,
                                elementInfo.FhirElementName,
                                exportedComplexName,
                                elementInfo.IsList,
                                "String",
                                string.Empty);

                            break;

                        // special case for Element.id, Extension.url, and Narrative.div
                        case "string":
                            _writer.WriteLineIndented($"case \"{elementInfo.FhirElementName}\":");
                            _writer.IncreaseIndent();

                            if (elementInfo.IsList)
                            {
                                _writer.WriteLineIndented($"{currentName}.Add(reader.GetString());");
                            }
                            else
                            {
                                _writer.WriteLineIndented($"{currentName} = reader.GetString();");
                            }

                            _writer.WriteLineIndented("break;");
                            _writer.DecreaseIndent();
                            _writer.WriteLine();

                            break;

                        case "FhirBoolean":
                            WriteJsonDeserializeElementPrimitive(
                                csType,
                                currentName,
                                elementInfo.FhirElementName,
                                exportedComplexName,
                                elementInfo.IsList,
                                "Boolean",
                                string.Empty);

                            break;

                        case "FhirDecimal":
                            WriteJsonDeserializeElementPrimitive(
                                csType,
                                currentName,
                                elementInfo.FhirElementName,
                                exportedComplexName,
                                elementInfo.IsList,
                                "Decimal",
                                string.Empty);

                            break;

                        case "Integer":
                        case "PositiveInt":
                        case "UnsignedInt":
                            WriteJsonDeserializeElementPrimitive(
                                csType,
                                currentName,
                                elementInfo.FhirElementName,
                                exportedComplexName,
                                elementInfo.IsList,
                                "Int32",
                                string.Empty);

                            break;

                        case "Instant":
                            WriteJsonDeserializeElementPrimitive(
                                csType,
                                currentName,
                                elementInfo.FhirElementName,
                                exportedComplexName,
                                elementInfo.IsList,
                                "String",
                                "DateTimeOffset.Parse");

                            break;

                        case "Integer64":
                            WriteJsonDeserializeElementPrimitive(
                                csType,
                                currentName,
                                elementInfo.FhirElementName,
                                exportedComplexName,
                                elementInfo.IsList,
                                "String",
                                "long.Parse");

                            break;

                        default:
                            // check for enum-typed codes (non-enum codes are handled above)
                            if (csType.StartsWith("Code<", StringComparison.Ordinal))
                            {
                                string codeType = csType.Substring(5, csType.Length - 6);

                                WriteJsonDeserializeElementPrimitive(
                                    csType,
                                    currentName,
                                    elementInfo.FhirElementName,
                                    exportedComplexName,
                                    elementInfo.IsList,
                                    "String",
                                    $"Hl7.Fhir.Utility.EnumUtility.ParseLiteral<{codeType}>");
                            }
                            else
                            {
                                _writer.WriteLineIndented($"case \"{elementInfo.FhirElementName}\":");
                                _writer.IncreaseIndent();

                                if (elementInfo.IsList)
                                {
                                    _writer.WriteLineIndented($"if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))");
                                    _writer.OpenScope();
                                    _writer.WriteLineIndented($"throw new JsonException($\"{exportedComplexName} error reading '{elementInfo.FhirElementName}' expected StartArray, found {{reader.TokenType}}! depth: {{reader.CurrentDepth}}, pos: {{reader.BytesConsumed}}\");");
                                    _writer.CloseScope();
                                    _writer.WriteLine();
                                    _writer.WriteLineIndented($"{currentName} = new List<{csType}>();");
                                    _writer.WriteLine();
                                    _writer.WriteLineIndented($"while (reader.TokenType != JsonTokenType.EndArray)");
                                    _writer.OpenScope();

                                    //_writer.WriteLineIndented($"{currentName}.Add(" +
                                    //    $"JsonSerializer.Deserialize<{_modelNamespace}.{csType}>(ref reader, options));");

                                    _writer.WriteLineIndented($"{_modelNamespace}.{csType} v_{elementInfo.ExportedName} = new {_modelNamespace}.{csType}();");
                                    _writer.WriteLineIndented($"v_{elementInfo.ExportedName}.DeserializeJson(ref reader, options);");
                                    _writer.WriteLineIndented($"{currentName}.Add(v_{elementInfo.ExportedName});");

                                    _writer.WriteLine();
                                    _writer.WriteLineIndented($"if (!reader.Read())");
                                    _writer.OpenScope();
                                    _writer.WriteLineIndented($"throw new JsonException($\"{exportedComplexName} error reading '{elementInfo.FhirElementName}' array, read failed! depth: {{reader.CurrentDepth}}, pos: {{reader.BytesConsumed}}\");");
                                    _writer.CloseScope();

                                    _writer.WriteLineIndented($"if (reader.TokenType == JsonTokenType.EndObject) {{ reader.Read(); }}");

                                    _writer.CloseScope();
                                    _writer.WriteLine();
                                    _writer.WriteLineIndented($"if ({currentName}.Count == 0)");
                                    _writer.OpenScope();
                                    _writer.WriteLineIndented($"{currentName} = null;");
                                    _writer.CloseScope();
                                }
                                else
                                {
                                    //_writer.WriteLineIndented($"{currentName} = " +
                                    //    $"JsonSerializer.Deserialize<{_modelNamespace}.{csType}>(ref reader, options);");

                                    _writer.WriteLineIndented($"{currentName} = new {_modelNamespace}.{csType}();");
                                    _writer.WriteLineIndented($"(({_modelNamespace}.{csType}){currentName}).DeserializeJson(ref reader, options);");
                                }

                                _writer.WriteLineIndented("break;");
                                _writer.DecreaseIndent();
                                _writer.WriteLine();
                            }

                            break;
                    }
                }
            }
        }

        /// <summary>Writes a JSON deserialize element primitive.</summary>
        /// <exception cref="JsonException">Thrown when a JSON error condition occurs.</exception>
        /// <param name="csType">             Type of the create structure.</param>
        /// <param name="currentName">        The current name.</param>
        /// <param name="fhirElementName">    Name of the FHIR element.</param>
        /// <param name="exportedComplexName">Name of the exported complex parent.</param>
        /// <param name="isList">             True if is list, false if not.</param>
        /// <param name="readerFunctionClass">The reader function class.</param>
        /// <param name="conversionFunction"> The conversion function required to write the value.</param>
        /// <param name="isChoiceType">       True if is choice type, false if not.</param>
        private void WriteJsonDeserializeElementPrimitive(
            string csType,
            string currentName,
            string fhirElementName,
            string exportedComplexName,
            bool isList,
            string readerFunctionClass,
            string conversionFunction)
        {
            _writer.WriteLineIndented($"case \"{fhirElementName}\":");
            _writer.IncreaseIndent();

            string readerString = string.IsNullOrEmpty(conversionFunction)
                ? $"reader.Get{readerFunctionClass}()"
                : $"{conversionFunction}(reader.Get{readerFunctionClass}())";

            if (isList)
            {
                // StartArray block (open and close)
                _writer.WriteLineIndented($"if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))");
                _writer.OpenScope();
                _writer.WriteLineIndented($"throw new JsonException($\"{exportedComplexName} error reading '{fhirElementName}' expected StartArray, found {{reader.TokenType}}! depth: {{reader.CurrentDepth}}, pos: {{reader.BytesConsumed}}\");");
                _writer.CloseScope();

                _writer.WriteLine();
                _writer.WriteLineIndented($"{currentName} = new List<{csType}>();");
                _writer.WriteLine();

                // open while - !EndArray
                _writer.WriteLineIndented($"while (reader.TokenType != JsonTokenType.EndArray)");
                _writer.OpenScope();

                // null check block (open and close)
                _writer.WriteLineIndented($"if (reader.TokenType == JsonTokenType.Null)");
                _writer.OpenScope();
                _writer.WriteLineIndented($"{currentName}.Add(new {csType}());");
                _writer.WriteLineIndented("reader.Skip();");
                _writer.CloseScope();
                _writer.WriteLineIndented("else");
                _writer.OpenScope();
                _writer.WriteLineIndented($"{currentName}.Add(new {csType}({readerString}));");
                _writer.CloseScope();

                // read block (open and close)
                _writer.WriteLine();
                _writer.WriteLineIndented($"if (!reader.Read())");
                _writer.OpenScope();
                _writer.WriteLineIndented($"throw new JsonException($\"{exportedComplexName} error reading '{fhirElementName}' array, read failed! depth: {{reader.CurrentDepth}}, pos: {{reader.BytesConsumed}}\");");
                _writer.CloseScope();

                _writer.WriteLineIndented($"if (reader.TokenType == JsonTokenType.EndObject) {{ reader.Read(); }}");

                // close while !EndArray
                _writer.CloseScope();

                _writer.WriteLine();

                // null check block (open and close)
                _writer.WriteLineIndented($"if ({currentName}.Count == 0)");
                _writer.OpenScope();
                _writer.WriteLineIndented($"{currentName} = null;");
                _writer.CloseScope();
            }
            else
            {
                // null check block (open and close)
                _writer.WriteLineIndented($"if (reader.TokenType == JsonTokenType.Null)");
                _writer.OpenScope();
                _writer.WriteLineIndented($"{currentName} = new {csType}();");
                _writer.WriteLineIndented("reader.Skip();");
                _writer.CloseScope();
                _writer.WriteLineIndented("else");
                _writer.OpenScope();
                _writer.WriteLineIndented($"{currentName} = new {csType}({readerString});");
                _writer.CloseScope();
            }

            _writer.WriteLineIndented("break;");
            _writer.DecreaseIndent();
            _writer.WriteLine();

            //string elementName = currentName.EndsWith("Element", StringComparison.Ordinal)
            //    ? currentName
            //    : currentName + "Element";

            string elementName = currentName;
            //if (isChoiceType || currentName.EndsWith("Element", StringComparison.Ordinal))
            //{
            //    elementName = currentName;
            //}
            //else
            //{
            //    elementName = currentName + "Element";
            //}

            _writer.WriteLineIndented($"case \"_{fhirElementName}\":");
            _writer.IncreaseIndent();

            if (isList)
            {
                // StartArray block (open and close)
                _writer.WriteLineIndented($"if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))");
                _writer.OpenScope();
                _writer.WriteLineIndented($"throw new JsonException($\"{exportedComplexName} error reading '{fhirElementName}' expected StartArray, found {{reader.TokenType}}! depth: {{reader.CurrentDepth}}, pos: {{reader.BytesConsumed}}\");");
                _writer.CloseScope();
                _writer.WriteLine();

                _writer.WriteLineIndented($"int i_{fhirElementName} = 0;");
                _writer.WriteLine();

                // open while - !EndArray
                _writer.WriteLineIndented($"while (reader.TokenType != JsonTokenType.EndArray)");
                _writer.OpenScope();

                // count block (open and close)
                _writer.WriteLineIndented($"if (i_{fhirElementName} >= {elementName}.Count)");
                _writer.OpenScope();
                _writer.WriteLineIndented($"{elementName}.Add(new {csType}());");
                _writer.CloseScope();

                // null check block (open and close)
                _writer.WriteLineIndented($"if (reader.TokenType == JsonTokenType.Null)");
                _writer.OpenScope();
                _writer.WriteLineIndented("reader.Skip();");
                _writer.CloseScope();
                _writer.WriteLineIndented("else");
                _writer.OpenScope();
                _writer.WriteLineIndented($"(({_modelNamespace}.Element){elementName}[i_{fhirElementName}++]).DeserializeJson(ref reader, options);");
                _writer.CloseScope();
                _writer.WriteLine();

                // read block (open and close)
                _writer.WriteLineIndented($"if (!reader.Read())");
                _writer.OpenScope();
                _writer.WriteLineIndented($"throw new JsonException($\"{exportedComplexName} error reading '{fhirElementName}' array, read failed! depth: {{reader.CurrentDepth}}, pos: {{reader.BytesConsumed}}\");");
                _writer.CloseScope();

                _writer.WriteLineIndented($"if (reader.TokenType == JsonTokenType.EndObject) {{ reader.Read(); }}");

                // close while - !EndArray
                _writer.CloseScope();
            }
            else
            {
                _writer.WriteLineIndented($"if ({currentName} == null) {{ {currentName} = new {csType}(); }}");
                _writer.WriteLineIndented($"(({_modelNamespace}.Element){elementName}).DeserializeJson(ref reader, options);");
            }

            _writer.WriteLineIndented("break;");
            _writer.DecreaseIndent();
            _writer.WriteLine();
        }

        /// <summary>Writes a JSON deserialize choice element.</summary>
        /// <param name="currentName">The current name.</param>
        /// <param name="element">    The element.</param>
        /// <param name="elementInfo">Information describing the element.</param>
        private void WriteJsonDeserializeChoiceElement(
            string currentName,
            string exportedComplexName,
            FhirElement element,
            WrittenElementInfo elementInfo)
        {
            foreach (KeyValuePair<string, string> kvp in elementInfo.FhirAndCsTypes)
            {
                string fhirCombinedName = elementInfo.FhirElementName + char.ToUpperInvariant(kvp.Key[0]) + kvp.Key.Substring(1);

                switch (kvp.Value)
                {
                    case "Resource":
                    case "DomainResource":
                    case "MetadataResource":
                    case "CanonicalResource":

                        _writer.WriteLineIndented($"case \"{fhirCombinedName}\":");
                        _writer.IncreaseIndent();

                        // _writer.WriteLineIndented($"{currentName} = JsonSerializer.Deserialize<{_modelNamespace}.Resource>(ref reader, options);");
                        _writer.WriteLineIndented($"{currentName} = " +
                            $"{_serializationNamespace}.JsonStreamResourceConverter.PolymorphicRead(" +
                                $"ref reader, typeof({_modelNamespace}.Resource), options);");

                        _writer.WriteLineIndented("break;");
                        _writer.WriteLine();
                        _writer.DecreaseIndent();
                        break;

                    case "Base64Binary":
                        WriteJsonDeserializeElementPrimitive(
                            kvp.Value,
                            currentName,
                            fhirCombinedName,
                            exportedComplexName,
                            elementInfo.IsList,
                            "String",
                            "System.Convert.FromBase64String");

                        break;

                    case "Canonical":
                    case "Code":
                    case "Date":
                    case "DateTime":
                    case "FhirDateTime":
                    case "FhirString":
                    case "FhirUri":
                    case "FhirUrl":
                    case "Id":
                    case "Markdown":
                    case "Oid":
                    case "Uuid":
                    case "XHtml":
                        WriteJsonDeserializeElementPrimitive(
                            kvp.Value,
                            currentName,
                            fhirCombinedName,
                            exportedComplexName,
                            elementInfo.IsList,
                            "String",
                            string.Empty);

                        break;

                    // special case for Element.id, Extension.url, and Narrative.div
                    case "string":
                        _writer.WriteLineIndented($"case \"{fhirCombinedName}\":");
                        _writer.IncreaseIndent();

                        _writer.WriteLineIndented($"{currentName} = reader.GetString();");

                        _writer.WriteLineIndented("break;");
                        _writer.WriteLine();
                        _writer.DecreaseIndent();
                        break;

                    case "FhirBoolean":
                        WriteJsonDeserializeElementPrimitive(
                            kvp.Value,
                            currentName,
                            fhirCombinedName,
                            exportedComplexName,
                            elementInfo.IsList,
                            "Boolean",
                            string.Empty);

                        break;

                    case "FhirDecimal":
                        WriteJsonDeserializeElementPrimitive(
                            kvp.Value,
                            currentName,
                            fhirCombinedName,
                            exportedComplexName,
                            elementInfo.IsList,
                            "Decimal",
                            string.Empty);

                        break;

                    case "Integer":
                    case "PositiveInt":
                    case "UnsignedInt":
                        WriteJsonDeserializeElementPrimitive(
                            kvp.Value,
                            currentName,
                            fhirCombinedName,
                            exportedComplexName,
                            elementInfo.IsList,
                            "Int32",
                            string.Empty);

                        break;

                    case "Instant":
                        WriteJsonDeserializeElementPrimitive(
                            kvp.Value,
                            currentName,
                            fhirCombinedName,
                            exportedComplexName,
                            elementInfo.IsList,
                            "String",
                            "DateTimeOffset.Parse");

                        break;

                    case "Integer64":
                        WriteJsonDeserializeElementPrimitive(
                            kvp.Value,
                            currentName,
                            fhirCombinedName,
                            exportedComplexName,
                            elementInfo.IsList,
                            "String",
                            "long.Parse");

                        break;

                    default:
                        // check for enum-typed codes (non-enum codes are handled above)
                        if (kvp.Value.StartsWith("Code<", StringComparison.Ordinal))
                        {
                            string codeType = kvp.Value.Substring(5, kvp.Value.Length - 6);

                            WriteJsonDeserializeElementPrimitive(
                                kvp.Value,
                                currentName,
                                fhirCombinedName,
                                exportedComplexName,
                                elementInfo.IsList,
                                "String",
                                $"Hl7.Fhir.Utility.EnumUtility.ParseLiteral<{codeType}>");
                        }
                        else
                        {
                            _writer.WriteLineIndented($"case \"{fhirCombinedName}\":");
                            _writer.IncreaseIndent();

                            //_writer.WriteLineIndented($"{currentName} = " +
                            //    $"JsonSerializer.Deserialize<{_modelNamespace}.{kvp.Value}>(ref reader, options);");

                            _writer.WriteLineIndented($"{currentName} = new {_modelNamespace}.{kvp.Value}();");
                            _writer.WriteLineIndented($"(({_modelNamespace}.{kvp.Value}){currentName}).DeserializeJson(ref reader, options);");

                            _writer.WriteLineIndented("break;");
                            _writer.WriteLine();
                            _writer.DecreaseIndent();
                        }

                        break;
                }
            }
        }

        /// <summary>Writes the elements.</summary>
        /// <param name="complex">              The complex data type.</param>
        /// <param name="exportedComplexName">  Name of the exported complex parent.</param>
        private void WriteJsonSerializeElements(
            FhirComplex complex,
            string exportedComplexName)
        {
            foreach (FhirElement element in complex.Elements.Values.OrderBy(e => e.FieldOrder))
            {
                if (element.IsInherited)
                {
                    continue;
                }

                string typeName = element.BaseTypeName;

                if (string.IsNullOrEmpty(typeName) &&
                    (element.ElementTypes.Count == 1))
                {
                    typeName = element.ElementTypes.Values.First().Name;
                }

                WrittenElementInfo elementInfo;

                if (typeName == "code")
                {
                    elementInfo = BuildCodedElementInfo(element);
                }
                else
                {
                    elementInfo = BuildElementInfo(exportedComplexName, element);
                }

                string currentName = "current." + elementInfo.ExportedName;

                if (elementInfo.IsChoice)
                {
                    WriteJsonSerializeChoiceElement(currentName, element, elementInfo);
                }
                else
                {
                    string csType;
                    if (elementInfo.IsList)
                    {
                        csType = elementInfo.ExportedListSubType.StartsWith(_modelNamespace, StringComparison.Ordinal)
                            ? elementInfo.ExportedListSubType.Substring(_modelNamespace.Length + 1)
                            : elementInfo.ExportedListSubType;
                    }
                    else
                    {
                        csType = elementInfo.ExportedType.StartsWith(_modelNamespace, StringComparison.Ordinal)
                            ? elementInfo.ExportedType.Substring(_modelNamespace.Length + 1)
                            : elementInfo.ExportedType;
                    }

                    switch (csType)
                    {
                        case "Resource":
                        case "DomainResource":
                        case "MetadataResource":
                        case "CanonicalResource":
                            if (elementInfo.IsList)
                            {
                                _writer.WriteLineIndented($"if (({currentName} != null) && ({currentName}.Count != 0))");
                                _writer.OpenScope();
                                _writer.WriteLineIndented($"writer.WritePropertyName(\"{elementInfo.FhirElementName}\");");
                                _writer.WriteLineIndented($"writer.WriteStartArray();");

                                _writer.WriteLineIndented($"foreach (dynamic resource in {currentName})");
                                _writer.OpenScope();
                                _writer.WriteLineIndented($"JsonStreamResourceConverter.WriteResource(writer, resource, options);");
                                _writer.CloseScope();

                                _writer.WriteLineIndented($"writer.WriteEndArray();");
                                _writer.CloseScope();
                            }
                            else
                            {
                                if (elementInfo.IsMandatory)
                                {
                                    _writer.WriteLineIndented($"writer.WritePropertyName(\"{elementInfo.FhirElementName}\");");

                                    // _writer.WriteLineIndented($"JsonSerializer.Serialize<{_modelNamespace}.Resource>(" +
                                    // _writer.WriteLineIndented($"JsonStreamResourceConverter.WriteResource(" +
                                    _writer.WriteLineIndented($"JsonSerializer.Serialize<object>(" +
                                        $"writer, " +
                                        $"({_modelNamespace}.Resource){currentName}, " +
                                        $"options);");
                                }
                                else
                                {
                                    _writer.WriteLineIndented($"if ({currentName} != null)");
                                    _writer.OpenScope();
                                    _writer.WriteLineIndented($"writer.WritePropertyName(\"{elementInfo.FhirElementName}\");");

                                    // _writer.WriteLineIndented($"JsonSerializer.Serialize<{_modelNamespace}.Resource>(" +
                                    // _writer.WriteLineIndented($"JsonStreamResourceConverter.WriteResource(" +
                                    _writer.WriteLineIndented($"JsonSerializer.Serialize<object>(" +
                                        $"writer, " +
                                        $"({_modelNamespace}.Resource){currentName}, " +
                                        $"options);");
                                    _writer.CloseScope();
                                }
                            }

                            break;

                        case "Base64Binary":

                            WriteJsonSerializeElementPrimitive(
                                csType,
                                currentName,
                                elementInfo.FhirElementName,
                                elementInfo.IsList,
                                elementInfo.IsMandatory,
                                NullCheckType.Equality,
                                "String",
                                "System.Convert.ToBase64String",
                                string.Empty,
                                false,
                                string.Empty,
                                false);

                            break;

                        case "Code":
                            WriteJsonSerializeElementPrimitive(
                                csType,
                                currentName,
                                elementInfo.FhirElementName,
                                elementInfo.IsList,
                                elementInfo.IsMandatory,
                                NullCheckType.String,
                                "String",
                                string.Empty,
                                string.Empty,
                                false,
                                string.Empty,
                                true);
                            break;

                        case "Canonical":
                        case "Date":
                        case "DateTime":
                        case "FhirDateTime":
                        case "FhirString":
                        case "FhirUri":
                        case "FhirUrl":
                        case "Id":
                        case "Markdown":
                        case "Oid":
                        case "Uuid":
                        case "XHtml":
                            WriteJsonSerializeElementPrimitive(
                                csType,
                                currentName,
                                elementInfo.FhirElementName,
                                elementInfo.IsList,
                                elementInfo.IsMandatory,
                                NullCheckType.String,
                                "String",
                                string.Empty,
                                string.Empty,
                                false,
                                string.Empty,
                                false);
                            break;

                        // special case for Element.id, Extension.url, and Narrative.div
                        case "string":
                            if (elementInfo.IsList)
                            {
                                _writer.WriteLineIndented($"if (({currentName} != null) && ({currentName}.Count != 0))");
                                _writer.OpenScope();
                                _writer.WriteLineIndented($"writer.WritePropertyName(\"{elementInfo.FhirElementName}\");");
                                _writer.WriteLineIndented($"writer.WriteStartArray();");

                                _writer.WriteLineIndented($"foreach (string val in {currentName})");
                                _writer.OpenScope();
                                _writer.WriteLineIndented($"writer.WriteStringValue(val.Trim());");
                                _writer.CloseScope();

                                _writer.WriteLineIndented($"writer.WriteEndArray();");
                                _writer.CloseScope();
                            }
                            else
                            {
                                if (elementInfo.IsMandatory)
                                {
                                    _writer.WriteLineIndented($"writer.WriteString(\"{elementInfo.FhirElementName}\",{currentName}.Trim());");
                                }
                                else
                                {
                                    _writer.WriteLineIndented($"if (!string.IsNullOrEmpty({currentName}))");
                                    _writer.OpenScope();
                                    _writer.WriteLineIndented($"writer.WriteString(\"{elementInfo.FhirElementName}\",{currentName}.Trim());");
                                    _writer.CloseScope();
                                }
                            }

                            break;

                        case "FhirBoolean":
                            WriteJsonSerializeElementPrimitive(
                                csType,
                                currentName,
                                elementInfo.FhirElementName,
                                elementInfo.IsList,
                                elementInfo.IsMandatory,
                                NullCheckType.Equality,
                                "Boolean",
                                string.Empty,
                                "bool",
                                false,
                                string.Empty,
                                false);

                            break;

                        case "FhirDecimal":
                            WriteJsonSerializeElementPrimitive(
                                csType,
                                currentName,
                                elementInfo.FhirElementName,
                                elementInfo.IsList,
                                elementInfo.IsMandatory,
                                NullCheckType.Equality,
                                "Number",
                                string.Empty,
                                "decimal",
                                false,
                                string.Empty,
                                false);

                            break;

                        case "Integer":
                        case "PositiveInt":
                        case "UnsignedInt":
                            WriteJsonSerializeElementPrimitive(
                                csType,
                                currentName,
                                elementInfo.FhirElementName,
                                elementInfo.IsList,
                                elementInfo.IsMandatory,
                                NullCheckType.Equality,
                                "Number",
                                string.Empty,
                                "int",
                                false,
                                string.Empty,
                                false);

                            break;

                        case "Instant":
                            WriteJsonSerializeElementPrimitive(
                                csType,
                                currentName,
                                elementInfo.FhirElementName,
                                elementInfo.IsList,
                                elementInfo.IsMandatory,
                                NullCheckType.Equality,
                                "String",
                                string.Empty,
                                "DateTimeOffset",
                                true,
                                "yyyy-MM-dd'T'HH:mm:ss.FFFFFFFK",
                                false);

                            break;

                        case "Integer64":
                            WriteJsonSerializeElementPrimitive(
                                csType,
                                currentName,
                                elementInfo.FhirElementName,
                                elementInfo.IsList,
                                elementInfo.IsMandatory,
                                NullCheckType.Equality,
                                "String",
                                string.Empty,
                                string.Empty,
                                true,
                                string.Empty,
                                false);

                            break;

                        default:
                            // check for enum-typed codes (non-enum codes are handled above)
                            if (csType.StartsWith("Code<", StringComparison.Ordinal))
                            {
                                WriteJsonSerializeElementPrimitive(
                                    csType,
                                    currentName,
                                    elementInfo.FhirElementName,
                                    elementInfo.IsList,
                                    elementInfo.IsMandatory,
                                    NullCheckType.Equality,
                                    "String",
                                    "Hl7.Fhir.Utility.EnumUtility.GetLiteral",
                                    string.Empty,
                                    false,
                                    string.Empty,
                                    false);
                            }
                            else
                            {
                                if (elementInfo.IsList)
                                {
                                    _writer.WriteLineIndented($"if (({currentName} != null) && ({currentName}.Count != 0))");
                                    _writer.OpenScope();
                                    _writer.WriteLineIndented($"writer.WritePropertyName(\"{elementInfo.FhirElementName}\");");
                                    _writer.WriteLineIndented($"writer.WriteStartArray();");

                                    _writer.WriteLineIndented($"foreach ({csType} val in {currentName})");
                                    _writer.OpenScope();
                                    _writer.WriteLineIndented($"val.SerializeJson(writer, options, true);");
                                    _writer.CloseScope();

                                    _writer.WriteLineIndented($"writer.WriteEndArray();");
                                    _writer.CloseScope();
                                }
                                else
                                {
                                    if (elementInfo.IsMandatory)
                                    {
                                        _writer.WriteLineIndented($"writer.WritePropertyName(\"{elementInfo.FhirElementName}\");");
                                        _writer.WriteLineIndented($"{currentName}.SerializeJson(writer, options);");
                                    }
                                    else
                                    {
                                        _writer.WriteLineIndented($"if ({currentName} != null)");
                                        _writer.OpenScope();
                                        _writer.WriteLineIndented($"writer.WritePropertyName(\"{elementInfo.FhirElementName}\");");
                                        _writer.WriteLineIndented($"{currentName}.SerializeJson(writer, options);");
                                        _writer.CloseScope();
                                    }
                                }
                            }

                            break;
                    }

                    _writer.WriteLine();
                }
            }
        }

        /// <summary>Writes a serialize JSON element internal.</summary>
        /// <param name="csType">             Type of the create structure.</param>
        /// <param name="currentName">        The current name.</param>
        /// <param name="fhirElementName">    Name of the FHIR element.</param>
        /// <param name="isList">             True if is list, false if not.</param>
        /// <param name="isMandatory">        True if is mandatory, false if not.</param>
        /// <param name="nullCheckType">      Type of the null check.</param>
        /// <param name="writerFunctionClass">The writer function class.</param>
        /// <param name="conversionFunction"> The conversion function required to write the value.</param>
        /// <param name="castType">           Type of the cast.</param>
        /// <param name="requiresToString">   True to requires to string.</param>
        /// <param name="stringFormat">       The string format.</param>
        /// <param name="requiresTrim">       True to requires trim.</param>
        private void WriteJsonSerializeElementPrimitive(
            string csType,
            string currentName,
            string fhirElementName,
            bool isList,
            bool isMandatory,
            NullCheckType nullCheckType,
            string writerFunctionClass,
            string conversionFunction,
            string castType,
            bool requiresToString,
            string stringFormat,
            bool requiresTrim)
        {
            if (isList)
            {
                string valueString = string.IsNullOrEmpty(conversionFunction)
                    ? "val.Value"
                    : $"{conversionFunction}(val.Value)";

                if (!string.IsNullOrEmpty(castType))
                {
                    valueString = $"({castType}){valueString}";
                }

                if (requiresToString)
                {
                    if (string.IsNullOrEmpty(stringFormat))
                    {
                        valueString = $"{valueString}.ToString()";
                    }
                    else
                    {
                        valueString = $"({valueString}).ToString(\"{stringFormat}\",System.Globalization.CultureInfo.InvariantCulture)";
                    }
                }

                if (requiresTrim)
                {
                    valueString = $"{valueString}.Trim()";
                }

                // open if - element exists
                _writer.WriteLineIndented($"if (({currentName} != null) && ({currentName}.Count != 0))");
                _writer.OpenScope();

                _writer.WriteLineIndented("int valueCount = 0;");
                _writer.WriteLineIndented("int extensionCount = 0;");

                // open foreach - array element check
                _writer.WriteLineIndented($"foreach ({csType} val in {currentName})");
                _writer.OpenScope();

                switch (nullCheckType)
                {
                    case NullCheckType.String:
                        _writer.WriteLineIndented("if (!string.IsNullOrEmpty(val.Value)) { valueCount++; }");
                        break;
                    case NullCheckType.Equality:
                        _writer.WriteLineIndented("if (val.Value != null) { valueCount++; }");
                        break;
                    case NullCheckType.None:
                    default:
                        _writer.WriteLineIndented("valueCount++;");
                        break;
                }

                _writer.WriteLineIndented("if (val.HasExtensions()) { extensionCount++; }");

                // close foreach - array element check
                _writer.CloseScope();
                _writer.WriteLine();

                // open if - valueCount > 0
                _writer.WriteLineIndented("if (valueCount > 0)");
                _writer.OpenScope();

                _writer.WriteLineIndented($"writer.WritePropertyName(\"{fhirElementName}\");");
                _writer.WriteLineIndented($"writer.WriteStartArray();");

                // open foreach - element writing
                _writer.WriteLineIndented($"foreach ({csType} val in {currentName})");
                _writer.OpenScope();

                switch (nullCheckType)
                {
                    case NullCheckType.String:
                        _writer.WriteLineIndented("if (string.IsNullOrEmpty(val.Value))");
                        _writer.OpenScope();
                        _writer.WriteLineIndented("writer.WriteNullValue();");
                        _writer.CloseScope();
                        _writer.WriteLineIndented("else");
                        _writer.OpenScope();
                        _writer.WriteLineIndented($"writer.Write{writerFunctionClass}Value({valueString});");
                        _writer.CloseScope();
                        break;
                    case NullCheckType.Equality:
                        _writer.WriteLineIndented("if (val.Value == null)");
                        _writer.OpenScope();
                        _writer.WriteLineIndented("writer.WriteNullValue();");
                        _writer.CloseScope();
                        _writer.WriteLineIndented("else");
                        _writer.OpenScope();
                        _writer.WriteLineIndented($"writer.Write{writerFunctionClass}Value({valueString});");
                        _writer.CloseScope();
                        break;
                    case NullCheckType.None:
                    default:
                        _writer.WriteLineIndented($"writer.Write{writerFunctionClass}Value({valueString});");
                        break;
                }

                // close foreach - element writing
                _writer.CloseScope();

                _writer.WriteLine();
                _writer.WriteLineIndented("writer.WriteEndArray();");

                // close if - valueCount > 0
                _writer.CloseScope();

                _writer.WriteLine();

                // open if - extensionCount > 0
                _writer.WriteLineIndented("if (extensionCount > 0)");
                _writer.OpenScope();

                // end the main property array
                _writer.WriteLineIndented($"writer.WritePropertyName(\"_{fhirElementName}\");");
                _writer.WriteLineIndented("writer.WriteStartArray();");

                // open foreach - extension writing
                _writer.WriteLineIndented($"foreach ({csType} val in {currentName})");
                _writer.OpenScope();

                _writer.WriteLineIndented("if (val.HasExtensions() || (!string.IsNullOrEmpty(val.ElementId)))");
                _writer.OpenScope();
                _writer.WriteLineIndented("JsonStreamUtilities.SerializeExtensionList(" +
                    "writer," +
                    "options," +
                    "string.Empty," +
                    "true," +
                    "val.Extension," +
                    "val.ElementId);");
                _writer.CloseScope();
                _writer.WriteLineIndented("else");
                _writer.OpenScope();
                _writer.WriteLineIndented($"writer.WriteNullValue();");
                _writer.CloseScope();
                _writer.WriteLine();

                // close foreach - extension writing
                _writer.CloseScope();

                _writer.WriteLine();
                _writer.WriteLineIndented("writer.WriteEndArray();");

                // close if - extensionCount > 0
                _writer.CloseScope();

                // close if - element exists
                _writer.CloseScope();
            }
            else
            {
                string valueString = string.IsNullOrEmpty(conversionFunction)
                    ? $"{currentName}.Value"
                    : $"{conversionFunction}({currentName}.Value)";

                if (!string.IsNullOrEmpty(castType))
                {
                    valueString = $"({castType}){valueString}";
                }

                if (requiresToString)
                {
                    if (string.IsNullOrEmpty(stringFormat))
                    {
                        valueString = $"{valueString}.ToString()";
                    }
                    else
                    {
                        valueString = $"({valueString}).ToString(\"{stringFormat}\",System.Globalization.CultureInfo.InvariantCulture)";
                    }
                }

                if (requiresTrim)
                {
                    valueString = $"{valueString}.Trim()";
                }

                if (isMandatory)
                {
                    _writer.WriteLineIndented($"writer.Write{writerFunctionClass}(\"{fhirElementName}\",{valueString});");
                }
                else
                {
                    if (nullCheckType != NullCheckType.None)
                    {
                        // open element null check
                        _writer.WriteLineIndented($"if ({currentName} != null)");
                        _writer.OpenScope();
                    }

                    switch (nullCheckType)
                    {
                        case NullCheckType.String:
                            _writer.WriteLineIndented($"if (!string.IsNullOrEmpty({currentName}.Value))");
                            _writer.OpenScope();
                            _writer.WriteLineIndented($"writer.Write{writerFunctionClass}(\"{fhirElementName}\",{valueString});");
                            _writer.CloseScope();
                            break;

                        case NullCheckType.Equality:
                            _writer.WriteLineIndented($"if ({currentName}.Value != null)");
                            _writer.OpenScope();
                            _writer.WriteLineIndented($"writer.Write{writerFunctionClass}(\"{fhirElementName}\",{valueString});");
                            _writer.CloseScope();
                            break;

                        case NullCheckType.None:
                        default:
                            _writer.WriteLineIndented($"writer.Write{writerFunctionClass}(\"{fhirElementName}\",{valueString});");
                            break;
                    }

                    _writer.WriteLineIndented($"if ({currentName}.HasExtensions() || (!string.IsNullOrEmpty({currentName}.ElementId)))");
                    _writer.OpenScope();
                    _writer.WriteLineIndented($"JsonStreamUtilities.SerializeExtensionList(" +
                        $"writer," +
                        $"options," +
                        $"\"_{fhirElementName}\"," +
                        $"false," +
                        $"{currentName}.Extension," +
                        $"{currentName}.ElementId);");
                    _writer.CloseScope();

                    if (nullCheckType != NullCheckType.None)
                    {
                        // close element null check
                        _writer.CloseScope();
                    }
                }
            }
        }

        /// <summary>Writes a JSON serialize choice element.</summary>
        /// <param name="currentName">The current name.</param>
        /// <param name="element">    The element.</param>
        /// <param name="elementInfo">Information describing the element.</param>
        private void WriteJsonSerializeChoiceElement(
            string currentName,
            FhirElement element,
            WrittenElementInfo elementInfo)
        {
            // open null check
            _writer.WriteLineIndented($"if ({currentName} != null)");
            _writer.OpenScope();

            // open type switch
            _writer.WriteLineIndented($"switch ({currentName})");
            _writer.OpenScope();

            if (elementInfo.IsList)
            {
                Console.Write("");
            }

            foreach (KeyValuePair<string, string> kvp in elementInfo.FhirAndCsTypes)
            {
                string fhirCombinedName = elementInfo.FhirElementName + char.ToUpperInvariant(kvp.Key[0]) + kvp.Key.Substring(1);

                string caseVarName = "v_" + kvp.Value;

                _writer.WriteLineIndented($"case {kvp.Value} {caseVarName}:");
                _writer.IncreaseIndent();

                switch (kvp.Value)
                {
                    case "Resource":
                    case "DomainResource":
                    case "MetadataResource":
                    case "CanonicalResource":
                        _writer.WriteLineIndented($"writer.WritePropertyName(\"{fhirCombinedName}\");");

                        // _writer.WriteLineIndented($"JsonSerializer.Serialize<{_modelNamespace}.Resource>(writer, ({_modelNamespace}.Resource){caseVarName}, options);");
                        // _writer.WriteLineIndented($"JsonStreamResourceConverter.WriteResource(writer, ({_modelNamespace}.Resource){caseVarName}, options);");
                        _writer.WriteLineIndented($"JsonSerializer.Serialize<object>(writer, ({_modelNamespace}.Resource){caseVarName}, options);");

                        break;

                    case "Base64Binary":
                        WriteJsonSerializeElementPrimitive(
                            kvp.Value,
                            caseVarName,
                            fhirCombinedName,
                            elementInfo.IsList,
                            elementInfo.IsMandatory,
                            NullCheckType.Equality,
                            "String",
                            "System.Convert.ToBase64String",
                            string.Empty,
                            false,
                            string.Empty,
                            false);

                        break;

                    case "Code":
                        WriteJsonSerializeElementPrimitive(
                            kvp.Value,
                            caseVarName,
                            fhirCombinedName,
                            elementInfo.IsList,
                            elementInfo.IsMandatory,
                            NullCheckType.String,
                            "String",
                            string.Empty,
                            string.Empty,
                            false,
                            string.Empty,
                            true);

                        break;

                    case "Canonical":
                    case "Date":
                    case "DateTime":
                    case "FhirDateTime":
                    case "FhirString":
                    case "FhirUri":
                    case "FhirUrl":
                    case "Id":
                    case "Markdown":
                    case "Oid":
                    case "Uuid":
                    case "XHtml":
                        WriteJsonSerializeElementPrimitive(
                            kvp.Value,
                            caseVarName,
                            fhirCombinedName,
                            elementInfo.IsList,
                            elementInfo.IsMandatory,
                            NullCheckType.String,
                            "String",
                            string.Empty,
                            string.Empty,
                            false,
                            string.Empty,
                            false);

                        break;

                    // special case for Element.id, Extension.url, and Narrative.div
                    case "string":
                        _writer.WriteLineIndented($"writer.WriteString(\"{fhirCombinedName}\",{caseVarName}.Trim());");
                        break;

                    case "FhirBoolean":
                        WriteJsonSerializeElementPrimitive(
                            kvp.Value,
                            caseVarName,
                            fhirCombinedName,
                            elementInfo.IsList,
                            elementInfo.IsMandatory,
                            NullCheckType.Equality,
                            "Boolean",
                            string.Empty,
                            "bool",
                            false,
                            string.Empty,
                            false);

                        break;

                    case "FhirDecimal":
                        WriteJsonSerializeElementPrimitive(
                            kvp.Value,
                            caseVarName,
                            fhirCombinedName,
                            elementInfo.IsList,
                            elementInfo.IsMandatory,
                            NullCheckType.Equality,
                            "Number",
                            string.Empty,
                            "decimal",
                            false,
                            string.Empty,
                            false);

                        break;

                    case "Integer":
                    case "PositiveInt":
                    case "UnsignedInt":
                        WriteJsonSerializeElementPrimitive(
                            kvp.Value,
                            caseVarName,
                            fhirCombinedName,
                            elementInfo.IsList,
                            elementInfo.IsMandatory,
                            NullCheckType.Equality,
                            "Number",
                            string.Empty,
                            "int",
                            false,
                            string.Empty,
                            false);

                        break;

                    case "Instant":
                        WriteJsonSerializeElementPrimitive(
                            kvp.Value,
                            caseVarName,
                            fhirCombinedName,
                            elementInfo.IsList,
                            elementInfo.IsMandatory,
                            NullCheckType.Equality,
                            "String",
                            string.Empty,
                            "DateTimeOffset",
                            true,
                            "yyyy-MM-dd'T'HH:mm:ss.FFFFFFFK",
                            false);

                        break;

                    case "Integer64":
                        WriteJsonSerializeElementPrimitive(
                            kvp.Value,
                            caseVarName,
                            fhirCombinedName,
                            elementInfo.IsList,
                            elementInfo.IsMandatory,
                            NullCheckType.Equality,
                            "String",
                            string.Empty,
                            string.Empty,
                            true,
                            string.Empty,
                            false);

                        break;

                    default:
                        // check for enum-typed codes (non-enum codes are handled above)
                        if (kvp.Value.StartsWith("Code<", StringComparison.Ordinal))
                        {
                            WriteJsonSerializeElementPrimitive(
                                kvp.Value,
                                caseVarName,
                                fhirCombinedName,
                                elementInfo.IsList,
                                elementInfo.IsMandatory,
                                NullCheckType.Equality,
                                "String",
                                "Hl7.Fhir.Utility.EnumUtility.GetLiteral",
                                string.Empty,
                                false,
                                string.Empty,
                                false);
                        }
                        else
                        {
                            _writer.WriteLineIndented($"writer.WritePropertyName(\"{fhirCombinedName}\");");
                            _writer.WriteLineIndented($"{caseVarName}.SerializeJson(writer, options);");
                        }

                        break;
                }

                _writer.WriteLineIndented("break;");
                _writer.DecreaseIndent();
            }

            // close type switch
            _writer.CloseScope();

            // close null check
            _writer.CloseScope();
        }

        /// <summary>Writes an element.</summary>
        /// <param name="element">            The element.</param>
        private WrittenElementInfo BuildCodedElementInfo(
            FhirElement element)
        {
            bool hasDefinedEnum = true;
            if ((element.BindingStrength != "required") ||
                (!_info.TryGetValueSet(element.ValueSet, out FhirValueSet vs)) ||
                _exclusionSet.Contains(vs.URL))
            {
                hasDefinedEnum = false;
                vs = null;
            }

            string pascal = FhirUtils.ToConvention(element.Name, string.Empty, FhirTypeBase.NamingConvention.PascalCase);

            BuildElementOptionals(
                element,
                out string summary,
                out string choice,
                out string allowedTypes,
                out string resourceReferences,
                out Dictionary<string, string> fhirAndCsTypes);

            string codeLiteral;
            string enumClass;
            string matchTrailer = string.Empty;

            if (hasDefinedEnum)
            {
                string vsClass = _writtenValueSets[vs.URL].ClassName;
                string vsName = _writtenValueSets[vs.URL].ValueSetName;

                if (string.IsNullOrEmpty(vsClass))
                {
                    codeLiteral = $"Code<{_modelNamespace}.{vsName}>";
                    enumClass = $"{_modelNamespace}.{vsName}";
                }
                else
                {
                    codeLiteral = $"Code<{_modelNamespace}.{vsClass}.{vsName}>";
                    enumClass = $"{_modelNamespace}.{vsClass}.{vsName}";

                    if (vsName.ToUpperInvariant() == pascal.ToUpperInvariant())
                    {
                        matchTrailer = "_";
                    }
                }
            }
            else
            {
                codeLiteral = $"{_modelNamespace}.Code";
                enumClass = "string";
            }

            if (element.CardinalityMax == 1)
            {
                return new WrittenElementInfo()
                {
                    FhirElementName = element.Name.Replace("[x]", string.Empty, StringComparison.Ordinal),
                    ExportedName = $"{pascal}{matchTrailer}Element",
                    ExportedType = codeLiteral,
                    ExportedListSubType = null,
                    ExportedEnumType = enumClass,
                    IsList = false,
                    InSummary = element.IsSummary,
                    IsMandatory = element.CardinalityMin > 0,
                    IsChoice = element.Name.Contains("[x]", StringComparison.Ordinal),
                    FhirAndCsTypes = fhirAndCsTypes,
                };
            }

            return new WrittenElementInfo()
            {
                FhirElementName = element.Name.Replace("[x]", string.Empty, StringComparison.Ordinal),
                ExportedName = $"{pascal}{matchTrailer}Element",
                ExportedType = $"List<{codeLiteral}>",
                ExportedListSubType = codeLiteral,
                ExportedEnumType = enumClass,
                IsList = true,
                InSummary = element.IsSummary,
                IsMandatory = element.CardinalityMin > 0,
                IsChoice = element.Name.Contains("[x]", StringComparison.Ordinal),
                FhirAndCsTypes = fhirAndCsTypes,
            };
        }

        /// <summary>Writes an element.</summary>
        /// <param name="exportedComplexName">Name of the exported complex parent.</param>
        /// <param name="element">            The element.</param>
        private WrittenElementInfo BuildElementInfo(
            string exportedComplexName,
            FhirElement element)
        {
            List<WrittenElementInfo> elementInfo = new List<WrittenElementInfo>();
            bool isChoice = false;

            string name = element.Name;

            if (name.Contains("[x]", StringComparison.Ordinal))
            {
                name = name.Replace("[x]", string.Empty, StringComparison.Ordinal);
                isChoice = true;
            }

            string pascal = FhirUtils.ToConvention(name, string.Empty, FhirTypeBase.NamingConvention.PascalCase);

            BuildElementOptionals(
                element,
                out string summary,
                out string choice,
                out string allowedTypes,
                out string resourceReferences,
                out Dictionary<string, string> fhirAndCsTypes);

            string type;

            if (!string.IsNullOrEmpty(element.BaseTypeName))
            {
                type = element.BaseTypeName;
            }
            else if (element.ElementTypes.Count == 1)
            {
                type = element.ElementTypes.First().Value.Name;
            }
            else if (!string.IsNullOrEmpty(choice))
            {
                type = "DataType";
            }
            else
            {
                Debug.Fail($"No type can be derived for element {element.Name}");
                type = "object";
            }

            /* This is an exception - we want to share Meta across different FHIR versions
             * in the common library, so we use the "most common" type to the versions, which
             * is uri rather than the more specific canonical. */
            if (element.Path == "Meta.profile")
            {
                type = "uri";
            }

            bool noElement = true;

            if (type.Contains('.', StringComparison.Ordinal))
            {
                type = BuildTypeFromPath(type);
            }

            if (CSharpFirelyCommon.PrimitiveTypeMap.ContainsKey(type))
            {
                noElement = false;
            }

            if ((_info.FhirSequence < FhirPackageCommon.FhirSequenceEnum.R4) &&
                _info.ComplexTypes.ContainsKey(exportedComplexName) &&
                (type == "markdown"))
            {
                noElement = false;
            }

            if (CSharpFirelyCommon.TypeNameMappings.ContainsKey(type))
            {
                type = CSharpFirelyCommon.TypeNameMappings[type];
            }
            else
            {
                type = FhirUtils.SanitizedToConvention(type, FhirTypeBase.NamingConvention.PascalCase);
            }

            string elementTag = noElement ? string.Empty : "Element";

            if (_elementNameReplacementsByPath.ContainsKey(element.Path))
            {
                return new WrittenElementInfo()
                {
                    FhirElementName = element.Name.Replace("[x]", string.Empty, StringComparison.Ordinal),
                    ExportedName = _elementNameReplacementsByPath[element.Path],
                    ExportedType = $"string",
                    ExportedListSubType = null,
                    ExportedEnumType = null,
                    IsList = false,
                    InSummary = element.IsSummary,
                    IsMandatory = element.CardinalityMin > 0,
                    IsChoice = isChoice,
                    FhirAndCsTypes = fhirAndCsTypes,
                };
            }

            string exportName = $"{pascal}{elementTag}";

            if (element.CardinalityMax == 1)
            {
                return new WrittenElementInfo()
                {
                    FhirElementName = element.Name.Replace("[x]", string.Empty, StringComparison.Ordinal),
                    ExportedName = exportName,
                    ExportedType = $"{_modelNamespace}.{type}",
                    ExportedListSubType = null,
                    ExportedEnumType = null,
                    IsList = false,
                    InSummary = element.IsSummary,
                    IsMandatory = element.CardinalityMin > 0,
                    IsChoice = isChoice,
                    FhirAndCsTypes = fhirAndCsTypes,
                };
            }

            return new WrittenElementInfo()
            {
                FhirElementName = element.Name.Replace("[x]", string.Empty, StringComparison.Ordinal),
                ExportedName = exportName,
                ExportedType = $"List<{_modelNamespace}.{type}>",
                ExportedListSubType = $"{_modelNamespace}.{type}",
                ExportedEnumType = null,
                IsList = true,
                InSummary = element.IsSummary,
                IsMandatory = element.CardinalityMin > 0,
                IsChoice = isChoice,
                FhirAndCsTypes = fhirAndCsTypes,
            };
        }

        /// <summary>Builds type from path.</summary>
        /// <param name="type">The type.</param>
        /// <returns>A string.</returns>
        private string BuildTypeFromPath(string type)
        {
            if (_info.TryGetExplicitName(type, out string explicitTypeName))
            {
                string parentName = type.Substring(0, type.IndexOf('.', StringComparison.Ordinal));
                type = $"{parentName}" +
                    $".{explicitTypeName}" +
                    $"Component";
            }
            else
            {
                StringBuilder sb = new StringBuilder();

                string[] components = type.Split('.');

                for (int i = 0; i < components.Length; i++)
                {
                    if (i == 0)
                    {
                        sb.Append(components[i]);
                        continue;
                    }

                    if (i == 1)
                    {
                        sb.Append(".");
                    }

                    // AdverseEvent.suspectEntity.Causality does not prefix?
                    if (i == components.Length - 1)
                    {
                        sb.Append(FhirUtils.SanitizedToConvention(components[i], FhirTypeBase.NamingConvention.PascalCase));
                    }
                }

                sb.Append("Component");
                type = sb.ToString();
            }

            return type;
        }

        /// <summary>Builds element optional flags.</summary>
        /// <param name="element">           The element.</param>
        /// <param name="summary">           [out] The summary.</param>
        /// <param name="choice">            [out] The choice.</param>
        /// <param name="allowedTypes">      [out] List of types of the allowed.</param>
        /// <param name="resourceReferences">[out] The resource references.</param>
        /// <param name="fhirAndCsTypes">    [out] List of types of the FHIR and create structures.</param>
        private void BuildElementOptionals(
            FhirElement element,
            out string summary,
            out string choice,
            out string allowedTypes,
            out string resourceReferences,
            out Dictionary<string, string> fhirAndCsTypes)
        {
            choice = string.Empty;
            allowedTypes = string.Empty;
            resourceReferences = string.Empty;
            fhirAndCsTypes = new Dictionary<string, string>();

            summary = element.IsSummary ? ", InSummary=true" : string.Empty;

            if (element.ElementTypes != null)
            {
                if (element.ElementTypes.Count == 1)
                {
                    string elementType = element.ElementTypes.First().Value.Name;

                    if (CSharpFirelyCommon.TypeNameMappings.ContainsKey(elementType))
                    {
                        fhirAndCsTypes.Add(elementType, CSharpFirelyCommon.TypeNameMappings[elementType]);
                    }
                    else
                    {
                        fhirAndCsTypes.Add(elementType, elementType);
                    }

                    if (elementType == "Resource")
                    {
                        choice = ", Choice=ChoiceType.ResourceChoice";
                        allowedTypes = $"[AllowedTypes(typeof({_modelNamespace}.Resource))]";
                    }
                }
                else
                {
                    string firstType = element.ElementTypes.First().Key;

                    if (_info.PrimitiveTypes.ContainsKey(firstType) ||
                        _info.ComplexTypes.ContainsKey(firstType))
                    {
                        choice = ", Choice=ChoiceType.DatatypeChoice";
                    }

                    if (_info.Resources.ContainsKey(firstType))
                    {
                        choice = ", Choice=ChoiceType.ResourceChoice";
                    }

                    StringBuilder sb = new StringBuilder();
                    sb.Append("[AllowedTypes(");

                    bool needsSep = false;
                    foreach (FhirElementType elementType in element.ElementTypes.Values)
                    {
                        if (needsSep)
                        {
                            sb.Append(",");
                        }

                        needsSep = true;

                        sb.Append("typeof(");
                        sb.Append(_modelNamespace);
                        sb.Append(".");

                        if (CSharpFirelyCommon.TypeNameMappings.ContainsKey(elementType.Name))
                        {
                            string csType = CSharpFirelyCommon.TypeNameMappings[elementType.Name];
                            sb.Append(csType);
                            fhirAndCsTypes.Add(elementType.Name, csType);
                        }
                        else
                        {
                            string csType = FhirUtils.SanitizedToConvention(elementType.Name, FhirTypeBase.NamingConvention.PascalCase);
                            sb.Append(csType);
                            fhirAndCsTypes.Add(elementType.Name, csType);
                        }

                        sb.Append(")");
                    }

                    sb.Append(")]");
                    allowedTypes = sb.ToString();
                }
            }

            if (element.ElementTypes != null)
            {
                foreach (FhirElementType elementType in element.ElementTypes.Values)
                {
                    if (elementType.Name == "Reference" && elementType.Profiles.Values.Any())
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("[References(");

                        bool needsSep = false;
                        foreach (FhirElementProfile profile in elementType.Profiles.Values)
                        {
                            if (needsSep)
                            {
                                sb.Append(",");
                            }

                            needsSep = true;

                            sb.Append("\"");
                            sb.Append(profile.Name);
                            sb.Append("\"");
                        }

                        sb.Append(")]");
                        resourceReferences = sb.ToString();

                        break;
                    }
                }
            }
        }

        /// <summary>Opens extension class.</summary>
        /// <param name="fhirName">  Name of the FHIR.</param>
        /// <param name="exportName">Name of the export.</param>
        private void OpenExtensionClass(string fhirName, string exportName)
        {
            WriteIndentedComment($"JSON Serialization Extensions for {fhirName}");
            _writer.WriteLineIndented($"public static class {exportName}JsonExtensions");

            // open class
            OpenScope();
        }

        /// <summary>Opens serialize JSON.</summary>
        /// <param name="fhirName">        Name of the FHIR.</param>
        /// <param name="exportName">      Name of the export.</param>
        /// <param name="parentExportName">(Optional) Name of the parent export.</param>
        /// <param name="hasBaseClass">    (Optional) True if has base class, false if not.</param>
        private void OpenSerializeJson(
            string fhirName,
            string exportName,
            string parentExportName = "",
            bool hasBaseClass = false)
        {
            string className = string.IsNullOrEmpty(parentExportName)
                ? exportName
                : $"{parentExportName}.{exportName}";

            WriteIndentedComment($"Serialize a FHIR {fhirName} into JSON");
            _writer.WriteLineIndented(
                $"public static void SerializeJson(" +
                    $"this {className} current," +
                    $" Utf8JsonWriter writer," +
                    $" JsonSerializerOptions options," +
                    $" bool includeStartObject = true)");
            OpenScope();
        }

        /// <summary>Opens deserialize JSON.</summary>
        /// <param name="fhirName">        Name of the FHIR.</param>
        /// <param name="exportName">      Name of the export.</param>
        /// <param name="parentExportName">(Optional) Name of the parent export.</param>
        /// <param name="hasBaseClass">    (Optional) True if has base class, false if not.</param>
        private void OpenDeserializeJsonProperty(
            string fhirName,
            string exportName,
            string parentExportName = "",
            bool hasBaseClass = false)
        {
            string className = string.IsNullOrEmpty(parentExportName)
                ? exportName
                : $"{parentExportName}.{exportName}";

            WriteIndentedComment($"Deserialize JSON into a FHIR {fhirName}");
            _writer.WriteLineIndented(
                $"public static void DeserializeJsonProperty(" +
                    $"this {className} current," +
                    $" ref Utf8JsonReader reader," +
                    $" JsonSerializerOptions options," +
                    $" string propertyName)");
            OpenScope();
        }

        /// <summary>Opens deserialize JSON.</summary>
        /// <exception cref="JsonException">Thrown when a JSON error condition occurs.</exception>
        /// <param name="fhirName">        Name of the FHIR.</param>
        /// <param name="exportName">      Name of the export.</param>
        /// <param name="parentExportName">(Optional) Name of the parent export.</param>
        /// <param name="hasBaseClass">    (Optional) True if has base class, false if not.</param>
        private void WriteDeserializeJson(
            string fhirName,
            string exportName,
            string parentExportName = "",
            bool hasBaseClass = false)
        {
            string className = string.IsNullOrEmpty(parentExportName)
                ? exportName
                : $"{parentExportName}.{exportName}";

            WriteIndentedComment($"Deserialize JSON into a FHIR {fhirName}");
            _writer.WriteLineIndented(
                $"public static void DeserializeJson(" +
                    $"this {className} current," +
                    $" ref Utf8JsonReader reader," +
                    $" JsonSerializerOptions options)");
            OpenScope();

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

            // TODO(ginoc): DEBUG ONLY
            _writer.WriteLineIndented("if (Hl7.Fhir.Serialization.FhirSerializerOptions.Debug)" +
                $" {{ Console.WriteLine($\"{className} >>> {fhirName}.{{propertyName}}, depth: {{reader.CurrentDepth}}, pos: {{reader.BytesConsumed}}\"); }}");

            _writer.WriteLineIndented("reader.Read();");
            _writer.WriteLineIndented("current.DeserializeJsonProperty(ref reader, options, propertyName);");
            _writer.CloseScope();
            _writer.CloseScope();
            _writer.WriteLine();

            _writer.WriteLineIndented($"throw new JsonException($\"{className}: invalid state! depth: {{reader.CurrentDepth}}, pos: {{reader.BytesConsumed}}\");");

            // Close DeserializeJson
            CloseScope();
        }

        /// <summary>Writes the namespace open.</summary>
        private void WriteNamespaceOpen()
        {
            _writer.WriteLineIndented($"namespace {_modelNamespace}.JsonExtensions");
            OpenScope();
        }

        /// <summary>Writes the namespace close.</summary>
        private void WriteNamespaceClose()
        {
            CloseScope();
        }

        /// <summary>Writes the header JSON extent.</summary>
        private void WriteHeaderJsonExt()
        {
            WriteGenerationComment();

            _writer.WriteLineIndented("using System;");
            _writer.WriteLineIndented("using System.Buffers;");
            _writer.WriteLineIndented("using System.Collections.Generic;");
            _writer.WriteLineIndented("using System.IO;");
            _writer.WriteLineIndented("using System.Text;");
            _writer.WriteLineIndented("using System.Text.Json;");
            _writer.WriteLineIndented("using System.Text.Json.Serialization;");
            _writer.WriteLineIndented($"using {_modelNamespace};");
            _writer.WriteLineIndented($"using {_modelNamespace}.JsonExtensions;");
            _writer.WriteLineIndented($"using {_serializationNamespace};");
            _writer.WriteLine(string.Empty);

            WriteCopyright();
        }

        /// <summary>Writes the generation comment.</summary>
        /// <param name="writer">(Optional) The currently in-use text writer.</param>
        private void WriteGenerationComment(ExportStreamWriter writer = null)
        {
            if (writer == null)
            {
                writer = _writer;
            }

            writer.WriteLineIndented("// <auto-generated/>");
            writer.WriteLineIndented($"// Contents of: {_info.PackageName} version: {_info.VersionString}");
            if ((_options.ExportList != null) && _options.ExportList.Any())
            {
                string restrictions = string.Join("|", _options.ExportList);
                writer.WriteLineIndented($"  // Restricted to: {restrictions}");
            }

            writer.WriteLine(string.Empty);
        }

        /// <summary>Writes the copyright.</summary>
        private void WriteCopyright()
        {
            _writer.WriteLineIndented("/*");
            _writer.WriteLineIndented("  Copyright (c) 2011+, HL7, Inc.");
            _writer.WriteLineIndented("  All rights reserved.");
            _writer.WriteLineIndented("  ");
            _writer.WriteLineIndented("  Redistribution and use in source and binary forms, with or without modification, ");
            _writer.WriteLineIndented("  are permitted provided that the following conditions are met:");
            _writer.WriteLineIndented("  ");
            _writer.WriteLineIndented("   * Redistributions of source code must retain the above copyright notice, this ");
            _writer.WriteLineIndented("     list of conditions and the following disclaimer.");
            _writer.WriteLineIndented("   * Redistributions in binary form must reproduce the above copyright notice, ");
            _writer.WriteLineIndented("     this list of conditions and the following disclaimer in the documentation ");
            _writer.WriteLineIndented("     and/or other materials provided with the distribution.");
            _writer.WriteLineIndented("   * Neither the name of HL7 nor the names of its contributors may be used to ");
            _writer.WriteLineIndented("     endorse or promote products derived from this software without specific ");
            _writer.WriteLineIndented("     prior written permission.");
            _writer.WriteLineIndented("  ");
            _writer.WriteLineIndented("  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS \"AS IS\" AND ");
            _writer.WriteLineIndented("  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED ");
            _writer.WriteLineIndented("  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. ");
            _writer.WriteLineIndented("  IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, ");
            _writer.WriteLineIndented("  INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT ");
            _writer.WriteLineIndented("  NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR ");
            _writer.WriteLineIndented("  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, ");
            _writer.WriteLineIndented("  WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ");
            _writer.WriteLineIndented("  ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE ");
            _writer.WriteLineIndented("  POSSIBILITY OF SUCH DAMAGE.");
            _writer.WriteLineIndented("  ");
            _writer.WriteLineIndented("*/");
            _writer.WriteLine(string.Empty);
        }

        /// <summary>Writes a footer.</summary>
        private void WriteFooter()
        {
            WriteIndentedComment("end of file", singleLine: true);
        }

        /// <summary>Opens the scope.</summary>
        private void OpenScope()
            => CSharpFirelyCommon.OpenScope(_writer);

        /// <summary>Closes the scope.</summary>
        private void CloseScope(bool includeSemicolon = false, bool suppressNewline = false)
            => CSharpFirelyCommon.CloseScope(_writer, includeSemicolon, suppressNewline);

        /// <summary>Writes an indented comment.</summary>
        /// <param name="value">    The value.</param>
        /// <param name="isSummary">(Optional) True if is summary, false if not.</param>
        private void WriteIndentedComment(string value, bool isSummary = true, bool singleLine = false)
            => CSharpFirelyCommon.WriteIndentedComment(_writer, value, isSummary, singleLine);

        /// <summary>Adds a set of FhirTypes to a total set of exportable WrittenModelInfos.</summary>
        private static void AddModels(
           Dictionary<string, WrittenModelInfo> total,
           IEnumerable<WrittenModelInfo> typesToAdd)
        {
            foreach (WrittenModelInfo type in typesToAdd)
            {
                if (total.ContainsKey(type.FhirName))
                {
                    continue;
                }

                total.Add(type.FhirName, type);
            }
        }

        /// <summary>Information about a written value set.</summary>
        private struct WrittenValueSetInfo
        {
            internal string ClassName;
            internal string ValueSetName;
        }

        /// <summary>Information about the written element.</summary>
        private struct WrittenElementInfo
        {
            internal string FhirElementName;
            internal string ExportedName;
            internal string ExportedType;
            internal string ExportedListSubType;
            internal string ExportedEnumType;
            internal bool IsList;
            internal bool InSummary;
            internal bool IsMandatory;
            internal bool IsChoice;
            internal Dictionary<string, string> FhirAndCsTypes;
        }

        /// <summary>Information about the written model.</summary>
        private struct WrittenModelInfo
        {
            internal string FhirName;
            internal string CsName;
            internal bool IsAbstract;
            internal bool IsResource;
        }
    }
}
