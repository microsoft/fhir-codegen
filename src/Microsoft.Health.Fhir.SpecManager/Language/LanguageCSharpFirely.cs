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

        /// <summary>Name of the header user.</summary>
        private string _headerUserName;

        /// <summary>The header generation date time.</summary>
        private string _headerGenerationDateTime;

        /// <summary>FHIR information we are exporting.</summary>
        private FhirVersionInfo _info;

        /// <summary>Options for controlling the export.</summary>
        private ExporterOptions _options;

        /// <summary>List of types of the exported resource names ands.</summary>
        private Dictionary<string, string> _exportedResourceNamesAndTypes = new Dictionary<string, string>();

        /// <summary>The currently in-use text writer.</summary>
        private TextWriter _writer;

        /// <summary>Pathname of the export directory.</summary>
        private string _exportDirectory;

        /// <summary>Name of the language.</summary>
        private const string _languageName = "CSharpFirely";

        /// <summary>Dictionary mapping FHIR primitive types to language equivalents (see Template-Model.tt#1252).</summary>
        private static readonly Dictionary<string, string> _primitiveTypeMap = new Dictionary<string, string>()
        {
            { "base64Binary", "byte[]" },
            { "boolean", "bool?" },
            { "canonical", "string" },
            { "code", "string" },
            { "date", "string" },
            { "dateTime", "string" },
            { "decimal", "decimal?" },
            { "id", "string" },
            { "instant", "DateTimeOffset?" },
            { "integer", "int?" },
            { "integer64", "long" },
            { "markdown", "string" },
            { "narrative", "string" },
            { "oid", "string" },
            { "positiveInt", "int?" },
            { "string", "string" },
            { "time", "string" },
            { "unsignedInt", "int?" },
            { "uri", "string" },
            { "url", "string" },
            { "xhtml", "string" },
        };

        /// <summary>Types that have non-standard names or formatting (see Template-Model.tt#1252).</summary>
        private static Dictionary<string, string> _typeNameMappings = new Dictionary<string, string>()
        {
            { "boolean", "FhirBoolean" },
            { "dateTime", "FhirDateTime" },
            { "decimal", "FhirDecimal" },
            { "Reference", "ResourceReference" },
            { "string", "FhirString" },
            { "uri", "FhirUri" },
            { "url", "FhirUrl" },
            { "xhtml", "XHtml" },
        };

        /// <summary>Types or resources to skip writing (Template-Model.tt#1334).</summary>
        private static HashSet<string> _exclusionSet = new HashSet<string>()
        {
            "Base",
            "CanonicalResource",
            "DomainResource",
            "Element",
            "Extension",
            "MetadataResource",
            "Narrative",
            "Resource",
            "xhtml",
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

        /// <summary>This language does not allow configuration of name styles.</summary>
        private static readonly HashSet<FhirTypeBase.NamingConvention> _supportedStyles = new HashSet<FhirTypeBase.NamingConvention>()
        {
            FhirTypeBase.NamingConvention.LanguageControlled,
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
        bool ILanguage.SupportsNestedTypeDefinitions => true;

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
            ExporterOptions.FhirExportClassType.PrimitiveType,
            ExporterOptions.FhirExportClassType.ComplexType,
            ExporterOptions.FhirExportClassType.Resource,
            ExporterOptions.FhirExportClassType.Enum,
        };

        /// <summary>
        /// Gets a list of FHIR class types that the language CAN export, depending on user choices.
        /// </summary>
        List<ExporterOptions.FhirExportClassType> ILanguage.OptionalExportClassTypes => new List<ExporterOptions.FhirExportClassType>();

        /// <summary>Gets language-specific options and their descriptions.</summary>
        Dictionary<string, string> ILanguage.LanguageOptions => new Dictionary<string, string>();

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

            _headerUserName = Environment.UserName;
            _headerGenerationDateTime = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss", null);

            WritePrimitiveTypes(_info.PrimitiveTypes.Values, 1);

            WriteComplexDataTypes(_info.ComplexTypes.Values, 1);
        }

        /// <summary>Writes the complex data types.</summary>
        /// <param name="complexes">  The complex data types.</param>
        /// <param name="indentation">The indentation.</param>
        private void WriteComplexDataTypes(
            IEnumerable<FhirComplex> complexes,
            int indentation)
        {
            foreach (FhirComplex complex in complexes.OrderBy(c => c.Name))
            {
                if (_exclusionSet.Contains(complex.Name))
                {
                    continue;
                }

                WriteComplexDataType(complex, indentation);
            }
        }

        /// <summary>Writes a complex data type.</summary>
        /// <param name="complex">    The complex data type.</param>
        /// <param name="indentation">The indentation.</param>
        private void WriteComplexDataType(
            FhirComplex complex,
            int indentation)
        {
            string exportName = complex.NameForExport(FhirTypeBase.NamingConvention.PascalCase);

            string filename = Path.Combine(_exportDirectory, $"{exportName}.cs");

            using (StreamWriter writer = new StreamWriter(new FileStream(filename, FileMode.Create)))
            {
                _writer = writer;

                WriteHeaderComplexDataType();

                WriteNamespaceOpen();

                WriteComponent(complex, indentation, exportName);

                WriteNamespaceClose();

                WriteFooter();
            }
        }

        /// <summary>Writes a component.</summary>
        /// <param name="complex">    The complex data type.</param>
        /// <param name="indentation">The indentation.</param>
        /// <param name="exportName"> Name of the export.</param>
        private void WriteComponent(
            FhirComplex complex,
            int indentation,
            string exportName)
        {
            WriteIndentedComment(indentation, $"{complex.ShortDescription}");

            WriteIndented(indentation, $"[FhirType(\"{complex.Name}\")]");
            WriteIndented(indentation, "[DataContract]");

            switch (complex.BaseTypeName)
            {
                case "BackboneType":
                    WriteIndented(
                        indentation,
                        $"public partial class" +
                            $" {exportName}" +
                            $" : {_namespace}.BackboneElement," +
                            $" System.ComponentModel.INotifyPropertyChanged");
                    break;

                default:
                    if (_info.HasComplex(complex.BaseTypeName))
                    {
                        WriteIndented(
                            indentation,
                            $"public partial class" +
                                $" {exportName}" +
                                $" : {_namespace}.{complex.BaseTypeName}," +
                                $" System.ComponentModel.INotifyPropertyChanged");
                    }
                    else
                    {
                        WriteIndented(
                            indentation,
                            $"public partial class" +
                                $" {exportName}" +
                                $" : {_namespace}.Element," +
                                $" System.ComponentModel.INotifyPropertyChanged");
                    }

                    break;
            }

            // open class
            WriteIndented(
                indentation,
                "{");

            WritePropertyTypeName(indentation + 1, complex.Name);

            // check for nested components
            if (complex.Components != null)
            {
                foreach (FhirComplex component in complex.Components.Values)
                {
                    string componentName = $"{component.NameForExport(FhirTypeBase.NamingConvention.PascalCase)}Component";
                    WriteBackboneComponent(component, indentation + 1, componentName);
                }
            }

            WriteIndented(0, string.Empty);

            WriteEnums(complex, indentation + 1);
            WriteElements(complex, indentation);

            // close class
            WriteIndented(
                indentation,
                "}");
        }

        /// <summary>Writes a component.</summary>
        /// <param name="complex">    The complex data type.</param>
        /// <param name="indentation">The indentation.</param>
        /// <param name="exportName"> Name of the export.</param>
        private void WriteBackboneComponent(
            FhirComplex complex,
            int indentation,
            string exportName)
        {
            WriteIndentedComment(indentation, $"{complex.ShortDescription}");

            WriteIndented(indentation, $"[FhirType(\"{exportName}\", NamedBackboneElement=true)]");
            WriteIndented(indentation, "[DataContract]");

            WriteIndented(
                indentation,
                $"public partial class" +
                    $" {exportName}" +
                    $" : {_namespace}.Element," +
                    $" System.ComponentModel.INotifyPropertyChanged," +
                    $" IBackboneElement");

            // open class
            WriteIndented(
                indentation,
                "{");

            WritePropertyTypeName(indentation + 1, exportName);

            // check for nested components
            if (complex.Components != null)
            {
                foreach (FhirComplex component in complex.Components.Values)
                {
                    string componentName = $"{component.NameForExport(FhirTypeBase.NamingConvention.PascalCase)}Component";
                    WriteBackboneComponent(component, indentation + 1, componentName);
                }
            }

            WriteEnums(complex, indentation + 1);
            WriteElements(complex, indentation + 1);

            // close class
            WriteIndented(
                indentation,
                "}");
        }

        private void WriteEnums(
            FhirComplex complex,
            int indentation)
        {
            foreach (FhirElement element in complex.Elements.Values)
            {
                if ((!string.IsNullOrEmpty(element.ValueSet)) &&
                    _info.TryGetValueSet(element.ValueSet, out FhirValueSet vs))
                {
                    WriteValueSet(vs, indentation);

                    continue;
                }
            }
        }

        /// <summary>Writes a value set.</summary>
        /// <param name="vs">         The vs.</param>
        /// <param name="indentation">The indentation.</param>
        private void WriteValueSet(
            FhirValueSet vs,
            int indentation)
        {
            string name = vs.Name ?? vs.Id;
            string nameSanitized = FhirUtils.SanitizeForProperty(name, _reservedWords);

            nameSanitized = FhirUtils.SanitizedToConvention(nameSanitized, FhirTypeBase.NamingConvention.PascalCase);

            if (vs.ReferencedCodeSystems.Count == 1)
            {
                WriteIndentedComment(indentation, $"{vs.Description}\n(system: {vs.ReferencedCodeSystems.First()})");
            }
            else
            {
                WriteIndentedComment(indentation, $"{vs.Description}\n(systems: {vs.ReferencedCodeSystems.Count})");
            }

            WriteIndented(
                indentation,
                $"[FhirEnumeration(\"{name}\")]");

            WriteIndented(
                indentation,
                $"public enum {nameSanitized}");

            WriteIndented(
                indentation,
                "{");

            HashSet<string> usedValues = new HashSet<string>();

            foreach (FhirConcept concept in vs.Concepts.OrderBy(c => c.Code))
            {
                string codeName = FhirUtils.SanitizeForProperty(concept.Code, _reservedWords);
                string pascal = FhirUtils.ToConvention(codeName, string.Empty, FhirTypeBase.NamingConvention.PascalCase);
                string codeValue = FhirUtils.SanitizeForValue(concept.Code);

                string comment = concept.Definition ?? "MISSING DESCRIPTION";

                string display = FhirUtils.SanitizeForValue(concept.Display);

                WriteIndentedComment(
                    indentation + 1,
                    comment);

                WriteIndented(
                    indentation + 1,
                    $"[EnumLiteral(\"{codeValue}\", \"{concept.System}\"), Description(\"{display}\")]");

                WriteIndented(
                    indentation + 1,
                    $"{pascal},");
            }

            WriteIndented(
                indentation,
                "}");
        }

        /// <summary>Writes the elements.</summary>
        /// <param name="complex">    The complex data type.</param>
        /// <param name="indentation">The indentation.</param>
        private void WriteElements(
            FhirComplex complex,
            int indentation)
        {
            int order = 30;

            foreach (FhirElement element in complex.Elements.Values.OrderBy(e => e.FieldOrder))
            {
                order += 10;
                WriteElement(complex, element, indentation, order);
            }
        }

        private void WriteElement(
            FhirComplex complex,
            FhirElement element,
            int indentation,
            int order)
        {
            return;
            string pascal = FhirUtils.ToConvention(element.Name, string.Empty, FhirTypeBase.NamingConvention.PascalCase);

            Dictionary<string, string> values = element.NamesAndTypesForExport(
                FhirTypeBase.NamingConvention.PascalCase,
                FhirTypeBase.NamingConvention.PascalCase,
                false,
                string.Empty,
                complex.Components.ContainsKey(element.Path));

            WriteIndentedComment(indentation, $"{element.ShortDescription}");

            string inSummary = element.IsSummary ? ", InSummary=true" : string.Empty;

            WriteIndented(
                indentation,
                $"[FhirElement(\"{element.Name}\"{inSummary}, Order={order})]");

            WriteIndented(
                indentation,
                "[DataMember]");

            WriteIndented(
                indentation,
                $"public {_namespace}.{element.BaseTypeName} {pascal}");

            WriteIndented(
                indentation,
                "{");

            WriteIndented(
                indentation + 1,
                $"get {{ return _{pascal}; }}");

            WriteIndented(
                indentation + 1,
                $"set {{ _{pascal} = value; OnPropertyChanged(\"{pascal}\"); }}");

            WriteIndented(
                indentation,
                "}");
        }

        /// <summary>Writes a property type name.</summary>
        /// <param name="indentation">The indentation.</param>
        /// <param name="name">       The name.</param>
        private void WritePropertyTypeName(int indentation, string name)
        {
            WriteIndentedComment(
                indentation,
                "FHIR Type Name");

            WriteIndented(
                indentation,
                "[NotMapped]");

            WriteIndented(
                indentation,
                $"public override string TypeName {{ get {{ return \"{name}\"; }} }}");

            WriteIndented(0, string.Empty);
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
                case "long":
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
                if (_exclusionSet.Contains(primitive.Name))
                {
                    continue;
                }

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
            string typeName = primitive.TypeForExport(FhirTypeBase.NamingConvention.PascalCase, _primitiveTypeMap);

            if (_typeNameMappings.ContainsKey(primitive.Name))
            {
                exportName = _typeNameMappings[primitive.Name];
            }
            else
            {
                exportName = primitive.NameForExport(FhirTypeBase.NamingConvention.PascalCase);
            }

            string filename = Path.Combine(_exportDirectory, $"{exportName}.cs");

            using (StreamWriter writer = new StreamWriter(new FileStream(filename, FileMode.Create)))
            {
                _writer = writer;

                WriteHeaderPrimitive();

                WriteNamespaceOpen();

                if (!string.IsNullOrEmpty(primitive.Comment))
                {
                    WriteIndentedComment(indentation + 1, $"Primitive Type {primitive.Name}\n{primitive.Comment}");
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

                WritePropertyTypeName(indentation + 2, primitive.Name);

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
        private void WriteHeaderComplexDataType()
        {
            WriteGenerationComment();

            WriteIndented(0, "using System;");
            WriteIndented(0, "using System.Collections.Generic;");
            WriteIndented(0, "using System.Linq;");
            WriteIndented(0, "using System.Runtime.Serialization;");
            WriteIndented(0, "using Hl7.Fhir.Introspection;");
            WriteIndented(0, "using Hl7.Fhir.Serialization;");
            WriteIndented(0, "using Hl7.Fhir.Specification;");
            WriteIndented(0, "using Hl7.Fhir.Utility;");
            WriteIndented(0, "using Hl7.Fhir.Validation;");
            WriteIndented(0, string.Empty);

            WriteCopyright();

            #if DISABLED    // 2020.07.01 - should be exporting everything with necessary summary tags
            WriteIndented(0, "#pragma warning disable 1591 // suppress XML summary warnings ");
            WriteIndented(0, string.Empty);
            #endif
        }

        /// <summary>Writes a header.</summary>
        private void WriteHeaderPrimitive()
        {
            WriteGenerationComment();

            WriteIndented(0, "using System;");
            WriteIndented(0, "using System.Collections.Generic;");
            WriteIndented(0, "using System.Linq;");
            WriteIndented(0, "using System.Runtime.Serialization;");
            WriteIndented(0, "using Hl7.Fhir.Introspection;");
            WriteIndented(0, "using Hl7.Fhir.Serialization;");
            WriteIndented(0, "using Hl7.Fhir.Specification;");
            WriteIndented(0, "using Hl7.Fhir.Utility;");
            WriteIndented(0, "using Hl7.Fhir.Validation;");
            WriteIndented(0, string.Empty);

            WriteCopyright();
        }

        /// <summary>Writes the generation comment.</summary>
        private void WriteGenerationComment()
        {
            WriteIndented(0, "// <auto-generated/>");
            WriteIndented(0, $"// Contents of: {_info.PackageName} version: {_info.VersionString}");
            WriteIndented(0, $"// Generated by {_headerUserName} on {_headerGenerationDateTime}");

            if ((_options.ExportList != null) && _options.ExportList.Any())
            {
                string restrictions = string.Join("|", _options.ExportList);
                WriteIndented(1, $"// Restricted to: {restrictions}");
            }

            WriteIndented(0, string.Empty);
        }

        /// <summary>Writes the copyright.</summary>
        private void WriteCopyright()
        {
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
