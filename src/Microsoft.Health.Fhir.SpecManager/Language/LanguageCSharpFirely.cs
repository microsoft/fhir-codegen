// <copyright file="LanguageCSharpFirely.cs" company="Microsoft Corporation">
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
    /// <summary>A language exporter for Firely-compliant C# FHIR output.</summary>
    public sealed class LanguageCSharpFirely : ILanguage
    {
        /// <summary>The namespace to use during export.</summary>
        private const string _namespace = "Hl7.Fhir.Model";

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

        /// <summary>FHIR information we are exporting.</summary>
        private FhirVersionInfo _info;

        /// <summary>Options for controlling the export.</summary>
        private ExporterOptions _options;

        private Dictionary<string, string> _exportedResourceNamesAndTypes = new Dictionary<string, string>();

        /// <summary>The currently in-use text writer.</summary>
        private TextWriter _writer;

        /// <summary>Pathname of the export directory.</summary>
        private string _exportDirectory;

        /// <summary>Name of the language.</summary>
        private const string _languageName = "CSharpFirely";

        /// <summary>Dictionary mapping FHIR primitive types to language equivalents.</summary>
        private static readonly Dictionary<string, string> _primitiveTypeMap = new Dictionary<string, string>()
        {
            { "base", "Object" },
        };

        /// <summary>Types that have non-standard names or formatting.</summary>
        private static Dictionary<string, string> _typeMappings = new Dictionary<string, string>()
        {
            { "boolean", "FhirBoolean" },
            { "dateTime", "FhirDateTime" },
            { "decimal", "FhirDecimal" },
            { "string", "FhirString" },
            { "uri", "FhirUri" },
            { "url", "FhirUrl" },
            { "xhtml", "XHtml" },
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

        /// <summary>This option supports only Pascal case.</summary>
        private static readonly HashSet<FhirTypeBase.NamingConvention> _pascalStyle = new HashSet<FhirTypeBase.NamingConvention>()
        {
            FhirTypeBase.NamingConvention.PascalCase,
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
        HashSet<FhirTypeBase.NamingConvention> ILanguage.SupportedElementNameStyles => _pascalStyle;

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
            _exportDirectory = exportDirectory;

            if (!((ILanguage)this).SupportedPrimitiveNameStyles.Contains(options.PrimitiveNameStyle))
            {
                throw new NotSupportedException($"Unsupported primitive naming style: {options.PrimitiveNameStyle}");
            }

            if (!((ILanguage)this).SupportedComplexTypeNameStyles.Contains(options.ComplexTypeNameStyle))
            {
                throw new NotSupportedException($"Unsupported complex naming style: {options.ComplexTypeNameStyle}");
            }

            if (!((ILanguage)this).SupportedElementNameStyles.Contains(options.ElementNameStyle))
            {
                throw new NotSupportedException($"Unsupported element naming style: {options.ElementNameStyle}");
            }

            if (!((ILanguage)this).SupportedInteractionNameStyles.Contains(options.InteractionNameStyle))
            {
                throw new NotSupportedException($"Unsupported interaction naming style: {options.InteractionNameStyle}");
            }

            if (!((ILanguage)this).SupportedEnumStyles.Contains(options.EnumStyle))
            {
                throw new NotSupportedException($"Unsupported enum naming style: {options.EnumStyle}");
            }

            if (options.PrimitiveNameStyle != FhirTypeBase.NamingConvention.None)
            {
                WritePrimitiveTypes(_info.PrimitiveTypes.Values, 1);
            }

            #if DISABLED
            // create a filename for writing (single file for now)
            string filename = Path.Combine(exportDirectory, $"R{info.MajorVersion}.cs");

            using (StreamWriter writer = new StreamWriter(new FileStream(filename, FileMode.Create)))
            {
                _writer = writer;

                WriteHeader();

                WriteNamespaceOpen();

                if (options.ComplexTypeNameStyle != FhirTypeBase.NamingConvention.None)
                {
                    WriteComplexes(_info.ComplexTypes.Values, 1, false);
                    WriteComplexes(_info.Resources.Values, 1, true);
                }

                if (options.EnumStyle != FhirTypeBase.NamingConvention.None)
                {
                    WriteValueSets(_info.ValueSetsByUrl.Values, 1);
                }

                WritePolymorphicHelpers(1);

                WriteNamespaceClose();

                WriteFooter();
            }
            #endif
        }

        /// <summary>Writes a polymorphic helpers.</summary>
        /// <param name="indentation">The indentation.</param>
        private void WritePolymorphicHelpers(int indentation)
        {
            WriteIndented(indentation, "public class ResourceConverter : JsonConverter");
            WriteIndented(indentation, "{");

            // function CanConvert
            WriteIndented(indentation + 1, "public override bool CanConvert(Type objectType)");
            WriteIndented(indentation + 1, "{");
            WriteIndented(indentation + 2, "return typeof(Resource).IsAssignableFrom(objectType);");
            WriteIndented(indentation + 1, "}");

            // property CanWrite
            WriteIndented(indentation + 1, "public override bool CanWrite");
            WriteIndented(indentation + 1, "{");
            WriteIndented(indentation + 2, "get { return false; }");
            WriteIndented(indentation + 1, "}");

            // function WriteJson
            WriteIndented(indentation + 1, "public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)");
            WriteIndented(indentation + 1, "{");
            WriteIndented(indentation + 2, "throw new NotImplementedException();");
            WriteIndented(indentation + 1, "}");

            // property CanRead
            WriteIndented(indentation + 1, "public override bool CanRead");
            WriteIndented(indentation + 1, "{");
            WriteIndented(indentation + 2, "get { return true; }");
            WriteIndented(indentation + 1, "}");

            // function ReadJson
            WriteIndented(indentation + 1, "public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)");
            WriteIndented(indentation + 1, "{");
            WriteIndented(indentation + 2, "JObject jObject = JObject.Load(reader);");
            WriteIndented(indentation + 2, "string resourceType = jObject[\"resourceType\"].Value<string>();");
            WriteIndented(indentation + 2, "object target = null;");
            WriteIndented(indentation + 2, "switch (resourceType)");
            WriteIndented(indentation + 2, "{");

            // loop through our types
            foreach (KeyValuePair<string, string> kvp in _exportedResourceNamesAndTypes)
            {
                WriteIndented(indentation + 3, $"case \"{kvp.Key}\":");
                WriteIndented(indentation + 4, $"target = new {kvp.Value}();");
                WriteIndented(indentation + 4, "break;");
            }

            // default case returns a Resource object
            WriteIndented(indentation + 3, "default:");
            WriteIndented(indentation + 4, "target = new Resource();");
            WriteIndented(indentation + 4, "break;");

            // close switch
            WriteIndented(indentation + 2, "}");

            // populate
            WriteIndented(indentation + 2, "serializer.Populate(jObject.CreateReader(), target);");

            // return/close ReadJson
            WriteIndented(indentation + 2, "return target;");
            WriteIndented(indentation + 1, "}");

            // close class
            WriteIndented(indentation, "}");
        }

        /// <summary>Writes a value sets.</summary>
        /// <param name="valueSets">  List of valueSetCollections.</param>
        /// <param name="indentation">The indentation.</param>
        private void WriteValueSets(
            IEnumerable<FhirValueSetCollection> valueSets,
            int indentation)
        {
            foreach (FhirValueSetCollection collection in valueSets.OrderBy(c => c.URL))
            {
                foreach (FhirValueSet vs in collection.ValueSetsByVersion.Values.OrderBy(v => v.Version))
                {
                    WriteValueSet(
                        vs,
                        indentation);
                }
            }
        }

        /// <summary>Writes a value set.</summary>
        /// <param name="vs">         The value set.</param>
        /// <param name="indentation">The indentation.</param>
        private void WriteValueSet(
            FhirValueSet vs,
            int indentation)
        {
            string vsName = FhirUtils.SanitizeForProperty(vs.Id ?? vs.Name, _reservedWords);

            vsName = FhirUtils.SanitizedToConvention(vsName, FhirTypeBase.NamingConvention.PascalCase);

            if (!string.IsNullOrEmpty(vs.Description))
            {
                WriteIndentedComment(indentation, vs.Description);
            }
            else
            {
                WriteIndentedComment(indentation, $"Value Set: {vs.URL}|{vs.Version}");
            }

            WriteIndented(
                indentation,
                $"public abstract class {vsName}");

            WriteIndented(
                indentation,
                "{");

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

                WriteIndented(
                    indentation + 1,
                    $"public static readonly Coding {name} = new Coding");

                WriteIndented(
                    indentation + 1,
                    "{");

                WriteIndented(
                    indentation + 2,
                    $"Code = \"{codeValue}\",");

                if (!string.IsNullOrEmpty(concept.Display))
                {
                    WriteIndented(
                        indentation + 2,
                        $"Display = \"{FhirUtils.SanitizeForQuoted(concept.Display)}\",");
                }

                WriteIndented(
                    indentation + 2,
                    $"System = \"{concept.System}\"");

                WriteIndented(
                    indentation + 1,
                    "};");
            }

            WriteIndented(
                indentation,
                "};");
        }

        /// <summary>Writes the complexes.</summary>
        /// <param name="complexes">  The complexes.</param>
        /// <param name="indentation">The indentation.</param>
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
                    $"public class {complex.NameForExport(_options.ComplexTypeNameStyle)} {{");
            }
            else if (complex.Name.Equals(complex.BaseTypeName, StringComparison.Ordinal))
            {
                WriteIndented(
                    indentation,
                    $"public class" +
                        $" {complex.NameForExport(_options.ComplexTypeNameStyle, true)}" +
                        $" : Element {{");
            }
            else if ((complex.Components != null) && complex.Components.ContainsKey(complex.Path))
            {
                WriteIndented(
                    indentation,
                    $"public class" +
                        $" {complex.NameForExport(_options.ComplexTypeNameStyle, true)}" +
                        $" :" +
                        $" {complex.TypeForExport(_options.ComplexTypeNameStyle, _primitiveTypeMap, false)} {{");
            }
            else
            {
                WriteIndented(
                    indentation,
                    $"public class" +
                        $" {complex.NameForExport(_options.ComplexTypeNameStyle, true)}" +
                        $" :" +
                        $" {complex.TypeForExport(_options.ComplexTypeNameStyle, _primitiveTypeMap)} {{");
            }

            if (isResource && ShouldWriteResourceName(complex.Name))
            {
                _exportedResourceNamesAndTypes.Add(complex.Name, complex.Name);

                WriteIndented(indentation + 1, "/** Resource Type Name (for serialization) */");
                WriteIndented(indentation + 1, "[JsonPropertyName(\"resourceType\")]");
                WriteIndented(indentation + 1, $"public string ResourceType => \"{complex.Name}\";");
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
            WriteIndented(indentation, $"/// <summary>");
            WriteIndented(indentation, $"/// Code Values for the {element.Path} field");
            WriteIndented(indentation, $"/// </summary>");

            string codeName = FhirUtils.ToConvention(
                $"{element.Path}.Codes",
                string.Empty,
                _options.EnumStyle);

            if (codeName.Contains("[x]"))
            {
                codeName = codeName.Replace("[x]", string.Empty);
            }

            if (codeName.Contains("[X]"))
            {
                codeName = codeName.Replace("[X]", string.Empty);
            }

            WriteIndented(indentation, $"public sealed class {codeName} {{");

            foreach (string code in element.Codes)
            {
                FhirUtils.SanitizeForCode(code, _reservedWords, out string name, out string value);

                WriteIndented(indentation + 1, $"public const string {name.ToUpperInvariant()} = \"{value}\";");
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

                string camel = FhirUtils.ToConvention(kvp.Key, string.Empty, FhirTypeBase.NamingConvention.CamelCase);

                WriteIndented(indentation, $"[JsonPropertyName(\"{camel}\")]");

                WriteIndented(
                    indentation,
                    $"public {kvp.Value}{optionalFlagString}{arrayFlagString} {kvp.Key} {{ get; set; }}");

                WriteIndented(indentation, $"[JsonPropertyName(\"_{camel}\")]");
                WriteIndented(indentation, $"public Element{arrayFlagString} _{kvp.Key} {{ get; set; }}");
            }
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
            string exportName;
            string typeName = primitive.TypeForExport(_options.PrimitiveNameStyle, _primitiveTypeMap);

            if (_typeMappings.ContainsKey(primitive.Name))
            {
                exportName = _typeMappings[primitive.Name];
            }
            else
            {
                exportName = primitive.NameForExport(_options.PrimitiveNameStyle);
            }

            string filename = Path.Combine(_exportDirectory, $"{exportName}.cs");

            using (StreamWriter writer = new StreamWriter(new FileStream(filename, FileMode.Create)))
            {
                _writer = writer;

                WriteHeaderPrimitive();

                WriteNamespaceOpen();

                if (!string.IsNullOrEmpty(primitive.Comment))
                {
                    WriteIndentedComment(indentation + 1, primitive.Comment);
                }
                else
                {
                    WriteIndentedComment(indentation + 1, $"Primitive Type {primitive.Name}");
                }

                WriteIndented(indentation + 1, $"[FhirType(\"{primitive.Name}\")]");
                WriteIndented(indentation + 1, "[DataContract]");

                WriteIndented(
                    indentation + 1,
                    $"public partial class" +
                        $" {exportName}" +
                        $" : {_namespace}.Primitive<{typeName}>," +
                        $" System.ComponentModel.INotifyPropertyChanged");

                // open class
                WriteIndented(
                    indentation + 1,
                    "{");

                WriteIndented(
                    indentation + 2,
                    "[NotMapped]");

                WriteIndented(
                    indentation + 2,
                    $"public override string TypeName {{ get {{ return \"{typeName}\"; }} }}");

                WriteIndented(0, string.Empty);

                if (!string.IsNullOrEmpty(primitive.ValidationRegEx))
                {
                    WriteIndentedComment(
                        indentation + 2,
                        $"Must conform to pattern \"{primitive.ValidationRegEx}\"",
                        false);

                    WriteIndented(
                        indentation + 2,
                        $"public const string PATTERN = @\"{primitive.ValidationRegEx}\";");

                    WriteIndented(0, string.Empty);
                }

                WriteIndented(
                    indentation + 2,
                    $"public {exportName}({typeName} value)");

                WriteIndented(
                    indentation + 2,
                    "{");

                WriteIndented(
                    indentation + 3,
                    "Value = value;");

                WriteIndented(
                    indentation + 2,
                    "}");

                WriteIndented(0, string.Empty);

                WriteIndented(
                    indentation + 2,
                    $"public {exportName}(): this(({typeName})null) {{}}");

                WriteIndented(0, string.Empty);

                WriteIndentedComment(
                    indentation + 2,
                    "Primitive value of the element");

                WriteIndented(
                    indentation + 2,
                    "[FhirElement(\"value\", IsPrimitiveValue=true, XmlSerialization=XmlRepresentation.XmlAttr, InSummary=true, Order=30)]");

                WriteIndented(
                    indentation + 2,
                    "[DataMemeber]");

                WriteIndented(
                    indentation + 2,
                    $"public {typeName} Value");

                WriteIndented(
                    indentation + 2,
                    "{");

                WriteIndented(
                    indentation + 3,
                    $"get {{ return ({typeName})ObjectValue; }}");

                WriteIndented(
                    indentation + 3,
                    "set { ObjectValue = value; OnPropertyChanged(\"Value\"); }");

                WriteIndented(
                    indentation + 2,
                    "}");

                // close class
                WriteIndented(
                    indentation + 1,
                    "}");

                WriteNamespaceClose();

                WriteFooter();
            }
        }

        /// <summary>Writes a header.</summary>
        private void WriteHeader()
        {
            WriteIndented(0, "// <auto-generated/>");
            WriteIndented(0, $"// Contents of: {_info.PackageName} version: {_info.VersionString}");
            WriteIndented(1, $"// Using Model Inheritance: {_options.UseModelInheritance}");
            WriteIndented(1, $"// Hiding Removed Parent Fields: {_options.HideRemovedParentFields}");
            WriteIndented(1, $"// Nesting Type Definitions: {_options.NestTypeDefinitions}");
            WriteIndented(1, $"// Primitive Naming Style: {_options.PrimitiveNameStyle}");
            WriteIndented(1, $"// Complex Type / Resource Naming Style: {_options.ComplexTypeNameStyle}");
            WriteIndented(1, $"// Interaction Naming Style: {_options.InteractionNameStyle}");
            WriteIndented(1, $"// Extension Support: {_options.ExtensionSupport}");

            if ((_options.ExportList != null) && _options.ExportList.Any())
            {
                string restrictions = string.Join("|", _options.ExportList);
                WriteIndented(1, $"// Restricted to: {restrictions}");
            }

            WriteIndented(0, string.Empty);

            WriteIndented(0, "using System;");
            WriteIndented(0, "using System.Collections.Generic;");
            WriteIndented(0, "using Newtonsoft.Json;");
            WriteIndented(0, "using Newtonsoft.Json.Linq;");
            WriteIndented(0, string.Empty);
        }

        /// <summary>Writes the namespace open.</summary>
        private void WriteNamespaceOpen()
        {
            WriteIndented(0, $"namespace {_namespace}");
            WriteIndented(0, "{");
        }

        /// <summary>Writes the namespace close.</summary>
        private void WriteNamespaceClose()
        {
            WriteIndented(0, "}");
        }

        /// <summary>Writes a header.</summary>
        private void WriteHeaderPrimitive()
        {
            WriteIndented(0, "// <auto-generated/>");
            WriteIndented(0, $"// Contents of: {_info.PackageName} version: {_info.VersionString}");
            WriteIndented(1, $"// Using Model Inheritance: {_options.UseModelInheritance}");
            WriteIndented(1, $"// Hiding Removed Parent Fields: {_options.HideRemovedParentFields}");
            WriteIndented(1, $"// Nesting Type Definitions: {_options.NestTypeDefinitions}");
            WriteIndented(1, $"// Primitive Naming Style: {_options.PrimitiveNameStyle}");
            WriteIndented(1, $"// Complex Type / Resource Naming Style: {_options.ComplexTypeNameStyle}");
            WriteIndented(1, $"// Interaction Naming Style: {_options.InteractionNameStyle}");
            WriteIndented(1, $"// Extension Support: {_options.ExtensionSupport}");

            if ((_options.ExportList != null) && _options.ExportList.Any())
            {
                string restrictions = string.Join("|", _options.ExportList);
                WriteIndented(1, $"// Restricted to: {restrictions}");
            }

            WriteIndented(0, string.Empty);
            WriteIndented(0, "using System;");
            WriteIndented(0, "using System.Collections.Generic;");
            WriteIndented(0, "using Hl7.Fhir.Introspection;");
            WriteIndented(0, "using Hl7.Fhir.Validation;");
            WriteIndented(0, "using System.Linq;");
            WriteIndented(0, "using System.Runtime.Serialization;");
            WriteIndented(0, "using Hl7.Fhir.Serialization;");
            WriteIndented(0, "using Hl7.Fhir.Utility;");
            WriteIndented(0, "using Hl7.Fhir.Specification;");
            WriteIndented(0, string.Empty);
            WriteIndented(0, "/*");
            WriteIndented(0, "  Copyright (c) 2011+, HL7, Inc.");
            WriteIndented(0, "  All rights reserved.");
            WriteIndented(0, "  ");
            WriteIndented(0, "  Redistribution and use in source and binary forms, with or without modification, ");
            WriteIndented(0, "  are permitted provided that the following conditions are met:");
            WriteIndented(0, "  ");
            WriteIndented(0, "   * Redistributions of source code must retain the above copyright notice, this ");
            WriteIndented(0, "     list of conditions and the following disclaimer.");
            WriteIndented(0, "   * Redistributions in binary form must reproduce the above copyright notice, ");
            WriteIndented(0, "     this list of conditions and the following disclaimer in the documentation ");
            WriteIndented(0, "     and/or other materials provided with the distribution.");
            WriteIndented(0, "   * Neither the name of HL7 nor the names of its contributors may be used to ");
            WriteIndented(0, "     endorse or promote products derived from this software without specific ");
            WriteIndented(0, "     prior written permission.");
            WriteIndented(0, "  ");
            WriteIndented(0, "  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS \"AS IS\" AND ");
            WriteIndented(0, "  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED ");
            WriteIndented(0, "  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. ");
            WriteIndented(0, "  IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, ");
            WriteIndented(0, "  INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT ");
            WriteIndented(0, "  NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR ");
            WriteIndented(0, "  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, ");
            WriteIndented(0, "  WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ");
            WriteIndented(0, "  ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE ");
            WriteIndented(0, "  POSSIBILITY OF SUCH DAMAGE.");
            WriteIndented(0, "  ");
            WriteIndented(0, string.Empty);
            WriteIndented(0, "*/");
            WriteIndented(0, string.Empty);
        }

        /// <summary>Writes a footer.</summary>
        private void WriteFooter()
        {
            WriteIndentedComment(0, "end of file");
        }

        /// <summary>Writes an indented comment.</summary>
        /// <param name="indentation">The indentation.</param>
        /// <param name="value">      The value.</param>
        /// <param name="isSummary">  (Optional) True if is summary, false if not.</param>
        private void WriteIndentedComment(int indentation, string value, bool isSummary = true)
        {
            string prefix = $"{new string(' ', indentation * 2)}/// ";

            if (isSummary)
            {
                WriteIndented(indentation, "/// <summary>");
            }

            _writer.Write(prefix);
            prefix = $"\n{prefix}";

            _writer.WriteLine(value.Replace("\n", prefix).Replace("\r", string.Empty));

            if (isSummary)
            {
                WriteIndented(indentation, "/// </summary>");
            }
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
