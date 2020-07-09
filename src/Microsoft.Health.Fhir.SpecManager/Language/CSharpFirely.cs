// <copyright file="CSharpFirely.cs" company="Microsoft Corporation">
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
    public sealed class CSharpFirely : ILanguage
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

        /// <summary>Keep track of information about written value sets.</summary>
        private Dictionary<string, WrittenValueSetInfo> _writtenValueSets = new Dictionary<string, WrittenValueSetInfo>();

        /// <summary>The currently in-use text writer.</summary>
        private ExportStreamWriter _writer;

        /// <summary>The information writer (Template-Model.cs).</summary>
        private ExportStreamWriter _modelWriter;

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
            _info = info;
            _options = options;
            _exportDirectory = exportDirectory;
            _writtenValueSets = new Dictionary<string, WrittenValueSetInfo>();

            _headerUserName = Environment.UserName;
            _headerGenerationDateTime = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss", null);

            string infoFilename = Path.Combine(_exportDirectory, "Template-Model.cs");

            using (FileStream infoStream = new FileStream(infoFilename, FileMode.Create))
            using (ExportStreamWriter infoWriter = new ExportStreamWriter(infoStream))
            {
                _modelWriter = infoWriter;

                WriteGenerationComment(infoWriter);

                WriteCommonValueSets();

                _modelWriter.WriteLineIndented("// Generated items");

                WritePrimitiveTypes(_info.PrimitiveTypes.Values);

                WriteComplexDataTypes(_info.ComplexTypes.Values);

                WriteResources(_info.Resources.Values);
            }
        }

        /// <summary>Writes the common enums.</summary>
        private void WriteCommonValueSets()
        {
            string filename = Path.Combine(_exportDirectory, "Template-Bindings.cs");

            using (FileStream stream = new FileStream(filename, FileMode.Create))
            using (ExportStreamWriter writer = new ExportStreamWriter(stream))
            {
                _writer = writer;

                WriteHeaderBasic();
                WriteNamespaceOpen();

                foreach (FhirValueSetCollection collection in _info.ValueSetsByUrl.Values)
                {
                    // traverse value sets starting with highest version
                    foreach (FhirValueSet vs in collection.ValueSetsByVersion.Values.OrderByDescending(s => s.Version))
                    {
                        if (vs.ReferencedByPaths.Count < 2)
                        {
                            continue;
                        }

                        WriteEnum(vs, string.Empty);

                        _modelWriter.WriteLineIndented($"// Generated Shared Enumeration: {_writtenValueSets[vs.URL].ValueSetName} ({vs.URL})");
                        _modelWriter.IncreaseIndent();

                        foreach (string path in vs.ReferencedByPaths)
                        {
                            string name = path.Split('.')[0];

                            if (_info.ComplexTypes.ContainsKey(name))
                            {
                                _modelWriter.WriteLineIndented($"// Used in model class (type): {path}");
                                continue;
                            }

                            _modelWriter.WriteLineIndented($"// Used in model class (resource): {path}");
                        }

                        _modelWriter.DecreaseIndent();
                        _modelWriter.WriteLine(string.Empty);
                    }
                }

                WriteNamespaceClose();
            }
        }

        /// <summary>Writes the complex data types.</summary>
        /// <param name="complexes">The complex data types.</param>
        private void WriteResources(
            IEnumerable<FhirComplex> complexes)
        {
            foreach (FhirComplex complex in complexes.OrderBy(c => c.Name))
            {
                if (_exclusionSet.Contains(complex.Name))
                {
                    continue;
                }

                WriteResource(complex);
            }
        }

        /// <summary>Writes a complex data type.</summary>
        /// <param name="complex">The complex data type.</param>
        private void WriteResource(
            FhirComplex complex)
        {
            string exportName = complex.NameForExport(FhirTypeBase.NamingConvention.PascalCase);

            string filename = Path.Combine(_exportDirectory, $"{exportName}.cs");

            _modelWriter.WriteLineIndented($"// {exportName}.cs");

            using (FileStream stream = new FileStream(filename, FileMode.Create))
            using (ExportStreamWriter writer = new ExportStreamWriter(stream))
            {
                _writer = writer;

                WriteHeaderComplexDataType();

                WriteNamespaceOpen();

                WriteComponent(complex, exportName, true, 0);

                WriteNamespaceClose();

                WriteFooter();
            }
        }

        /// <summary>Writes the complex data types.</summary>
                 /// <param name="complexes">The complex data types.</param>
        private void WriteComplexDataTypes(
            IEnumerable<FhirComplex> complexes)
        {
            foreach (FhirComplex complex in complexes.OrderBy(c => c.Name))
            {
                if (_exclusionSet.Contains(complex.Name))
                {
                    continue;
                }

                WriteComplexDataType(complex);
            }
        }

        /// <summary>Writes a complex data type.</summary>
        /// <param name="complex">The complex data type.</param>
        private void WriteComplexDataType(
            FhirComplex complex)
        {
            string exportName = complex.NameForExport(FhirTypeBase.NamingConvention.PascalCase);

            string filename = Path.Combine(_exportDirectory, $"{exportName}.cs");

            _modelWriter.WriteLineIndented($"// {exportName}.cs");

            using (FileStream stream = new FileStream(filename, FileMode.Create))
            using (ExportStreamWriter writer = new ExportStreamWriter(stream))
            {
                _writer = writer;

                WriteHeaderComplexDataType();

                WriteNamespaceOpen();

                WriteComponent(complex, exportName, false, 0);

                WriteNamespaceClose();

                WriteFooter();
            }
        }

        /// <summary>Writes a component.</summary>
        /// <param name="complex">   The complex data type.</param>
        /// <param name="exportName">Name of the export.</param>
        /// <param name="isResource">True if is resource, false if not.</param>
        /// <param name="depth">     The depth.</param>
        private void WriteComponent(
            FhirComplex complex,
            string exportName,
            bool isResource,
            int depth)
        {
            WriteIndentedComment($"{complex.ShortDescription}");

            if (!complex.IsAbstract)
            {
                if (isResource)
                {
                    _writer.WriteLineIndented($"[FhirType(\"{complex.Name}\", IsResource=true)]");
                }
                else
                {
                    _writer.WriteLineIndented($"[FhirType(\"{complex.Name}\")]");
                }
            }

            string abstractFlag = complex.IsAbstract ? " abstract" : string.Empty;

            switch (complex.BaseTypeName)
            {
                case "Quantity":
                    WriteConstrainedQuantity(complex, exportName, depth);
                    return;

                case "BackboneElement":
                case "BackboneType":
                    _writer.WriteLineIndented("[DataContract]");
                    _writer.WriteLineIndented(
                        $"public{abstractFlag} partial class" +
                            $" {exportName}" +
                            $" : {_namespace}.BackboneElement," +
                            $" System.ComponentModel.INotifyPropertyChanged");
                    break;

                case "DataType":
                    _writer.WriteLineIndented("[DataContract]");
                    _writer.WriteLineIndented(
                        $"public{abstractFlag} partial class" +
                            $" {exportName}" +
                            $" : {_namespace}.Element," +
                            $" System.ComponentModel.INotifyPropertyChanged");
                    break;

                case "Resource":
                case "DomainResource":
                    _writer.WriteLineIndented("[DataContract]");
                    _writer.WriteLineIndented(
                        $"public{abstractFlag} partial class" +
                            $" {exportName}" +
                            $" : {_namespace}.{complex.BaseTypeName}," +
                            $" System.ComponentModel.INotifyPropertyChanged");
                    break;

                default:
                    _writer.WriteLineIndented("[DataContract]");
                    if (_info.HasComplex(complex.BaseTypeName))
                    {
                        _writer.WriteLineIndented(
                            $"public{abstractFlag} partial class" +
                                $" {exportName}" +
                                $" : {_namespace}.{complex.BaseTypeName}," +
                                $" System.ComponentModel.INotifyPropertyChanged");
                    }
                    else
                    {
                        _writer.WriteLineIndented(
                            $"public{abstractFlag} partial class" +
                                $" {exportName}" +
                                $" : {_namespace}.Element," +
                                $" System.ComponentModel.INotifyPropertyChanged");
                    }

                    break;
            }

            // open class
            OpenScope();

            WritePropertyTypeName(complex.Name, isResource, depth);

            if (!string.IsNullOrEmpty(complex.ValidationRegEx))
            {
                WriteIndentedComment(
                    $"Must conform to pattern \"{complex.ValidationRegEx}\"",
                    false);

                _writer.WriteLineIndented($"public const string PATTERN = @\"{complex.ValidationRegEx}\";");

                _writer.WriteLine(string.Empty);
            }

            WriteEnums(complex, exportName);

            // check for nested components
            if (complex.Components != null)
            {
                foreach (FhirComplex component in complex.Components.Values)
                {
                    string componentName = $"{component.NameForExport(FhirTypeBase.NamingConvention.PascalCase)}Component";
                    WriteBackboneComponent(component, componentName, isResource, depth + 1);
                }
            }

            WriteElements(complex, isResource);

            // close class
            CloseScope();
        }

        /// <summary>Writes a constrained quantity.</summary>
        /// <param name="complex">   The complex data type.</param>
        /// <param name="exportName">Name of the export.</param>
        /// <param name="depth">     The depth.</param>
        private void WriteConstrainedQuantity(
            FhirComplex complex,
            string exportName,
            int depth)
        {
            _writer.WriteLineIndented(
                $"public partial class" +
                    $" {exportName}" +
                    $" : Quantity");

            // open class
            OpenScope();

            WritePropertyTypeName(complex.Name, false, depth);

            _writer.WriteLineIndented("public override IDeepCopyable DeepCopy()");
            OpenScope();
            _writer.WriteLineIndented($"return CopyTo(new {exportName}());");
            CloseScope();

            _writer.WriteLineIndented("// TODO: Add code to enforce these constraints:");
            WriteIndentedComment(complex.Purpose, false);

            // close class
            CloseScope();
        }

        /// <summary>Writes a component.</summary>
        /// <param name="complex">   The complex data type.</param>
        /// <param name="exportName">Name of the export.</param>
        /// <param name="isResource">True if is resource, false if not.</param>
        /// <param name="depth">     The depth.</param>
        private void WriteBackboneComponent(
            FhirComplex complex,
            string exportName,
            bool isResource,
            int depth)
        {
            WriteIndentedComment($"{complex.ShortDescription}");

            _writer.WriteLineIndented($"[FhirType(\"{exportName}\", NamedBackboneElement=true)]");
            _writer.WriteLineIndented("[DataContract]");

            _writer.WriteLineIndented(
                $"public partial class" +
                    $" {exportName}" +
                    $" : {_namespace}.BackboneElement," +
                    $" System.ComponentModel.INotifyPropertyChanged," +
                    $" IBackboneElement");

            // open class
            OpenScope();

            WritePropertyTypeName(exportName, false, depth);

            // check for nested components
            if (complex.Components != null)
            {
                foreach (FhirComplex component in complex.Components.Values)
                {
                    string componentName = $"{component.NameForExport(FhirTypeBase.NamingConvention.PascalCase)}Component";
                    WriteBackboneComponent(component, componentName, isResource, depth + 1);
                }
            }

            WriteElements(complex, isResource);

            // close class
            CloseScope();
        }

        /// <summary>Writes the enums.</summary>
        /// <param name="complex">  The complex data type.</param>
        /// <param name="className">Name of the class this enum is being written in.</param>
        private void WriteEnums(
            FhirComplex complex,
            string className)
        {
            if (complex.Elements != null)
            {
                foreach (FhirElement element in complex.Elements.Values)
                {
                    if ((!string.IsNullOrEmpty(element.ValueSet)) &&
                        _info.TryGetValueSet(element.ValueSet, out FhirValueSet vs))
                    {
                        WriteEnum(vs, className);

                        continue;
                    }
                }
            }

            if (complex.Components != null)
            {
                foreach (FhirComplex component in complex.Components.Values)
                {
                    WriteEnums(component, className);
                }
            }
        }

        /// <summary>Writes a value set as an enum.</summary>
        /// <param name="vs">       The vs.</param>
        /// <param name="className">Name of the class this enum is being written in.</param>
        private void WriteEnum(
            FhirValueSet vs,
            string className)
        {
            if (_writtenValueSets.ContainsKey(vs.URL))
            {
                return;
            }

            string name = vs.Name ?? vs.Id;
            string nameSanitized = FhirUtils.SanitizeForProperty(name, _reservedWords);

            nameSanitized = FhirUtils.SanitizedToConvention(nameSanitized, FhirTypeBase.NamingConvention.PascalCase);

            if (vs.ReferencedCodeSystems.Count == 1)
            {
                WriteIndentedComment($"{vs.Description}\n(system: {vs.ReferencedCodeSystems.First()})");
            }
            else
            {
                WriteIndentedComment($"{vs.Description}\n(systems: {vs.ReferencedCodeSystems.Count})");
            }

            _writer.WriteLineIndented($"[FhirEnumeration(\"{name}\")]");

            _writer.WriteLineIndented($"public enum {nameSanitized}");

            OpenScope();

            HashSet<string> usedValues = new HashSet<string>();

            foreach (FhirConcept concept in vs.Concepts)
            {
                string codeName = FhirUtils.SanitizeForProperty(concept.Code, _reservedWords);
                string pascal = FhirUtils.SanitizedToConvention(codeName, FhirTypeBase.NamingConvention.PascalCase);
                string codeValue = FhirUtils.SanitizeForValue(concept.Code);

                string comment = concept.Definition;
                if (string.IsNullOrEmpty(comment))
                {
                    comment = "MISSING DESCRIPTION";
                }

                string display = FhirUtils.SanitizeForValue(concept.Display);

                WriteIndentedComment(comment);

                _writer.WriteLineIndented($"[EnumLiteral(\"{codeValue}\", \"{concept.System}\"), Description(\"{display}\")]");
                _writer.WriteLineIndented($"{pascal},");
            }

            _writtenValueSets.Add(
                vs.URL,
                new WrittenValueSetInfo()
                {
                    ClassName = className,
                    ValueSetName = nameSanitized,
                });

            CloseScope();
        }

        /// <summary>Gets an order.</summary>
        /// <param name="element">The element.</param>
        private static int GetOrder(FhirElement element)
        {
            return (element.FieldOrder * 10) + 10;
        }

        /// <summary>Writes the elements.</summary>
        /// <param name="complex">   The complex data type.</param>
        /// <param name="isResource">True if is resource, false if not.</param>
        private void WriteElements(
            FhirComplex complex,
            bool isResource)
        {
            foreach (FhirElement element in complex.Elements.Values.OrderBy(e => e.FieldOrder))
            {
                if (element.IsInherited)
                {
                    continue;
                }

                string typeName = element.BaseTypeName;

                if (string.IsNullOrEmpty(typeName))
                {
                    typeName = element.ElementTypes.Values.First().Name;
                }

                // if (!string.IsNullOrEmpty(element.ValueSet))
                if (typeName == "code")
                {
                    WriteCodedElement(element, isResource);
                    continue;
                }

                WriteElement(complex, element, isResource);
            }
        }

        /// <summary>Writes an element.</summary>
        /// <param name="element">   The element.</param>
        /// <param name="isResource">True if is resource, false if not.</param>
        private void WriteCodedElement(
            FhirElement element,
            bool isResource)
        {
            bool hasDefinedEnum = true;
            if (!_info.TryGetValueSet(element.ValueSet, out FhirValueSet vs))
            {
                hasDefinedEnum = false;
            }

            string pascal = FhirUtils.ToConvention(element.Name, string.Empty, FhirTypeBase.NamingConvention.PascalCase);

            WriteIndentedComment(element.ShortDescription);

            BuildElementOptionals(
                element,
                isResource,
                out string summary,
                out string choice,
                out string allowedTypes,
                out string resourceReferences);

            _writer.WriteLineIndented($"[FhirElement(\"{element.Name}\"{summary}, Order={GetOrder(element)}{choice})]");

            if (!string.IsNullOrEmpty(resourceReferences))
            {
                _writer.WriteLineIndented("[CLSCompliant(false)]");
                _writer.WriteLineIndented(resourceReferences);
            }

            if (!string.IsNullOrEmpty(allowedTypes))
            {
                _writer.WriteLineIndented("[CLSCompliant(false)]");
                _writer.WriteLineIndented(allowedTypes);
            }

            if ((element.CardinalityMin != 0) ||
                (element.CardinalityMax != 1))
            {
                _writer.WriteLineIndented($"[Cardinality(Min={element.CardinalityMin},Max={element.CardinalityMax})]");
            }

            _writer.WriteLineIndented("[DataMember]");

            string codeLiteral;
            string enumClass;
            string optional;

            if (hasDefinedEnum)
            {
                string vsClass = _writtenValueSets[vs.URL].ClassName;
                string vsName = _writtenValueSets[vs.URL].ValueSetName;

                if (string.IsNullOrEmpty(vsClass))
                {
                    codeLiteral = $"Code<{_namespace}.{vsName}>";
                    enumClass = $"{_namespace}.{vsName}";
                }
                else
                {
                    codeLiteral = $"Code<{_namespace}.{vsClass}.{vsName}>";
                    enumClass = $"{_namespace}.{vsClass}.{vsName}";
                }

                optional = "?";
            }
            else
            {
                codeLiteral = $"{_namespace}.Code";
                enumClass = "string";
                optional = string.Empty;
            }

            if (element.CardinalityMax == 1)
            {
                _writer.WriteLineIndented($"public {codeLiteral} {pascal}Element");

                OpenScope();
                _writer.WriteLineIndented($"get {{ return _{pascal}Element; }}");
                _writer.WriteLineIndented($"set {{ _{pascal}Element = value; OnPropertyChanged(\"{pascal}Element\"); }}");
                CloseScope();

                _writer.WriteLineIndented($"private {codeLiteral} _{pascal}Element;");
                _writer.WriteLine(string.Empty);
            }
            else
            {
                _writer.WriteLineIndented($"public List<{codeLiteral}> {pascal}Element");

                OpenScope();
                _writer.WriteLineIndented($"get {{ if (_{pascal}Element==null) _{pascal}Element = new List<{codeLiteral}>(); return _{pascal}Element; }}");
                _writer.WriteLineIndented($"set {{ _{pascal}Element = value; OnPropertyChanged(\"{pascal}Element\"); }}");
                CloseScope();

                _writer.WriteLineIndented($"private List<{codeLiteral}> _{pascal}Element;");
                _writer.WriteLine(string.Empty);
            }

            WriteIndentedComment(element.ShortDescription);
            _writer.WriteLineIndented($"/// <remarks>This uses the native .NET datatype, rather than the FHIR equivalent</remarks>");

            _writer.WriteLineIndented("[NotMapped]");
            _writer.WriteLineIndented("[IgnoreDataMemberAttribute]");

            if (element.CardinalityMax == 1)
            {
                _writer.WriteLineIndented($"public {enumClass}{optional} {pascal}");

                OpenScope();
                _writer.WriteLineIndented($"get {{ return {pascal}Element != null ? {pascal}Element.Value : null; }}");
                _writer.WriteLineIndented("set");
                OpenScope();

                if (string.IsNullOrEmpty(optional))
                {
                    _writer.WriteLineIndented($"if (value == null)");
                }
                else
                {
                    _writer.WriteLineIndented($"if (!value.HasValue)");
                }

                _writer.IncreaseIndent();
                _writer.WriteLineIndented($"{pascal}Element = null;");
                _writer.DecreaseIndent();
                _writer.WriteLineIndented("else");
                _writer.IncreaseIndent();
                _writer.WriteLineIndented($"{pascal}Element = new {codeLiteral}(value);");
                _writer.DecreaseIndent();
                _writer.WriteLineIndented($"OnPropertyChanged(\"{pascal}\");");
                CloseScope();
                CloseScope();
            }
            else
            {
                _writer.WriteLineIndented($"public IEnumerable<{enumClass}{optional}> {pascal}");

                OpenScope();
                _writer.WriteLineIndented($"get {{ return {pascal}Element != null ? {pascal}Element.Select(elem => elem.Value) : null; }}");
                _writer.WriteLineIndented("set");
                OpenScope();

                if (element.CardinalityMin == 0)
                {
                    _writer.WriteLineIndented($"if (!value.HasValue)");
                    _writer.IncreaseIndent();
                    _writer.WriteLineIndented($"{pascal}Element = null;");
                    _writer.DecreaseIndent();
                    _writer.WriteLineIndented("else");
                    _writer.IncreaseIndent();
                    _writer.WriteLineIndented($"{pascal}Element = new List<{enumClass}>(value.Select(elem => new {enumClass}(elem)));");
                    _writer.DecreaseIndent();
                }
                else
                {
                    _writer.WriteLineIndented($"if (value == null)");
                    _writer.IncreaseIndent();
                    _writer.WriteLineIndented($"{pascal}Element = null;");
                    _writer.DecreaseIndent();
                    _writer.WriteLineIndented("else");
                    _writer.IncreaseIndent();
                    _writer.WriteLineIndented($"{pascal}Element = new {codeLiteral}(value);");
                    _writer.DecreaseIndent();
                }

                _writer.WriteLineIndented($"OnPropertyChanged(\"{pascal}\");");
                CloseScope();
                CloseScope();
            }
        }

        /// <summary>Writes an element.</summary>
        /// <param name="element">   The element.</param>
        /// <param name="isResource">True if is resource, false if not.</param>
        private void WriteElement(
            FhirComplex complex,
            FhirElement element,
            bool isResource)
        {
            string name = element.Name;

            if (name.Contains("[x]"))
            {
                name = name.Replace("[x]", string.Empty);
            }

            string pascal = FhirUtils.ToConvention(name, string.Empty, FhirTypeBase.NamingConvention.PascalCase);

            WriteIndentedComment(element.ShortDescription);

            BuildElementOptionals(
                element,
                isResource,
                out string summary,
                out string choice,
                out string allowedTypes,
                out string resourceReferences);

            _writer.WriteLineIndented($"[FhirElement(\"{name}\"{summary}, Order={GetOrder(element)}{choice})]");

            if ((!string.IsNullOrEmpty(resourceReferences)) && string.IsNullOrEmpty(allowedTypes))
            {
                _writer.WriteLineIndented("[CLSCompliant(false)]");
                _writer.WriteLineIndented(resourceReferences);
            }

            if (!string.IsNullOrEmpty(allowedTypes))
            {
                _writer.WriteLineIndented("[CLSCompliant(false)]");
                _writer.WriteLineIndented(allowedTypes);
            }

            if ((element.CardinalityMin != 0) ||
                (element.CardinalityMax != 1))
            {
                _writer.WriteLineIndented($"[Cardinality(Min={element.CardinalityMin},Max={element.CardinalityMax})]");
            }

            _writer.WriteLineIndented("[DataMember]");

            string type = string.Empty;

            if (!string.IsNullOrEmpty(element.BaseTypeName))
            {
                type = element.BaseTypeName;
            }
            else if (!string.IsNullOrEmpty(choice))
            {
                type = "Element";
            }
            else if (element.ElementTypes.Count == 1)
            {
                type = element.ElementTypes.First().Value.Name;
            }
            else
            {
                type = "object";
            }

            bool noElement = true;

            if (complex.Components.ContainsKey(type))
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

                    sb.Append(".");
                    sb.Append(FhirUtils.SanitizedToConvention(components[i], FhirTypeBase.NamingConvention.PascalCase));
                    sb.Append("Component");
                }

                type = sb.ToString();
            }

            string nativeType = type;
            string optional = string.Empty;

            if (_primitiveTypeMap.ContainsKey(nativeType))
            {
                nativeType = _primitiveTypeMap[nativeType];

                if (IsNullable(nativeType))
                {
                    optional = "?";
                }

                noElement = false;
            }
            else
            {
                nativeType = $"{_namespace}.{type}";
            }

            if (_typeNameMappings.ContainsKey(type))
            {
                type = _typeNameMappings[type];
            }

            string elementTag = noElement ? string.Empty : "Element";

            if (element.CardinalityMax == 1)
            {
                _writer.WriteLineIndented($"public {_namespace}.{type} {pascal}{elementTag}");

                OpenScope();
                _writer.WriteLineIndented($"get {{ return _{pascal}{elementTag}; }}");
                _writer.WriteLineIndented($"set {{ _{pascal}{elementTag} = value; OnPropertyChanged(\"{pascal}{elementTag}\"); }}");
                CloseScope();

                _writer.WriteLineIndented($"private {_namespace}.{type} _{pascal}{elementTag};");
                _writer.WriteLine(string.Empty);
            }
            else
            {
                _writer.WriteLineIndented($"public List<{_namespace}.{type}> {pascal}{elementTag}");

                OpenScope();
                _writer.WriteLineIndented($"get {{ if (_{pascal}{elementTag}==null) _{pascal}{elementTag} = new List<{_namespace}.{type}>(); return _{pascal}{elementTag}; }}");
                _writer.WriteLineIndented($"set {{ _{pascal}{elementTag} = value; OnPropertyChanged(\"{pascal}{elementTag}\"); }}");
                CloseScope();

                _writer.WriteLineIndented($"private List<{_namespace}.{type}> _{pascal}{elementTag};");
                _writer.WriteLine(string.Empty);
            }

            if (noElement)
            {
                // only write the one field
                return;
            }

            WriteIndentedComment(element.ShortDescription);
            _writer.WriteLineIndented($"/// <remarks>This uses the native .NET datatype, rather than the FHIR equivalent</remarks>");

            _writer.WriteLineIndented("[NotMapped]");
            _writer.WriteLineIndented("[IgnoreDataMemberAttribute]");

            if (element.CardinalityMax == 1)
            {
                _writer.WriteLineIndented($"public {nativeType}{optional} {pascal}");

                OpenScope();
                _writer.WriteLineIndented($"get {{ return {pascal}Element != null ? {pascal}Element.Value : null; }}");
                _writer.WriteLineIndented("set");
                OpenScope();

                if (string.IsNullOrEmpty(optional))
                {
                    _writer.WriteLineIndented($"if (value == null)");
                }
                else
                {
                    _writer.WriteLineIndented($"if (!value.HasValue)");
                }

                _writer.IncreaseIndent();
                _writer.WriteLineIndented($"{pascal}Element = null;");
                _writer.DecreaseIndent();
                _writer.WriteLineIndented("else");
                _writer.IncreaseIndent();
                _writer.WriteLineIndented($"{pascal}Element = new {nativeType}(value);");
                _writer.DecreaseIndent();
                _writer.WriteLineIndented($"OnPropertyChanged(\"{pascal}\");");
                CloseScope();
                CloseScope();
            }
            else
            {
                _writer.WriteLineIndented($"public IEnumerable<{nativeType}{optional}> {pascal}");

                OpenScope();
                _writer.WriteLineIndented($"get {{ return {pascal}Element != null ? {pascal}Element.Select(elem => elem.Value) : null; }}");
                _writer.WriteLineIndented("set");
                OpenScope();

                if (element.CardinalityMin == 0)
                {
                    if (string.IsNullOrEmpty(optional))
                    {
                        _writer.WriteLineIndented($"if (value == null)");
                    }
                    else
                    {
                        _writer.WriteLineIndented($"if (!value.HasValue)");
                    }

                    _writer.IncreaseIndent();
                    _writer.WriteLineIndented($"{pascal}Element = null;");
                    _writer.DecreaseIndent();
                    _writer.WriteLineIndented("else");
                    _writer.IncreaseIndent();
                    _writer.WriteLineIndented($"{pascal}Element = new List<{_namespace}.{type}>(value.Select(elem => new {_namespace}{type}(elem)));");
                    _writer.DecreaseIndent();
                }
                else
                {
                    if (string.IsNullOrEmpty(optional))
                    {
                        _writer.WriteLineIndented($"if (value == null)");
                    }
                    else
                    {
                        _writer.WriteLineIndented($"if (!value.HasValue)");
                    }

                    _writer.IncreaseIndent();
                    _writer.WriteLineIndented($"{pascal}Element = null;");
                    _writer.DecreaseIndent();
                    _writer.WriteLineIndented("else");
                    _writer.IncreaseIndent();
                    _writer.WriteLineIndented($"{pascal}Element = new {_namespace}{type}(value);");
                    _writer.DecreaseIndent();
                }

                _writer.WriteLineIndented($"OnPropertyChanged(\"{pascal}\");");
                CloseScope();
                CloseScope();
            }
        }

        /// <summary>Builds element optional flags.</summary>
        /// <param name="element">           The element.</param>
        /// <param name="isResource">        True if is resource, false if not.</param>
        /// <param name="summary">           [out] The summary.</param>
        /// <param name="choice">            [out] The choice.</param>
        /// <param name="allowedTypes">      [out] List of types of the allowed.</param>
        /// <param name="resourceReferences">[out] The resource references.</param>
        private static void BuildElementOptionals(
            FhirElement element,
            bool isResource,
            out string summary,
            out string choice,
            out string allowedTypes,
            out string resourceReferences)
        {
            choice = string.Empty;
            allowedTypes = string.Empty;
            resourceReferences = string.Empty;

            summary = element.IsSummary ? ", InSummary=true" : string.Empty;

            if ((element.ElementTypes != null) &&
                (element.ElementTypes.Count > 1))
            {
                choice = isResource
                    ? ", Choice=ChoiceType.ResourceChoice"
                    : ", Choice=ChoiceType.DatatypeChoice";

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
                    sb.Append(_namespace);
                    sb.Append(".");

                    if (elementType.Name == "Reference")
                    {
                        sb.Append("ResourceReference");
                    }
                    else
                    {
                        sb.Append(elementType.Name);
                    }

                    sb.Append(")");
                }

                sb.Append(")]");
                allowedTypes = sb.ToString();
            }

            if (element.ElementTypes != null)
            {
                foreach (FhirElementType elementType in element.ElementTypes.Values)
                {
                    if (elementType.Name == "Reference")
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

        /// <summary>Writes a property type name.</summary>
        /// <param name="name">      The name.</param>
        /// <param name="isResource">True if is resource, false if not.</param>
        private void WritePropertyTypeName(string name, bool isResource, int depth)
        {
            if (isResource && (depth == 0))
            {
                WriteIndentedComment("FHIR Resource Type");
                _writer.WriteLineIndented("[NotMapped]");
                _writer.WriteLineIndented($"public override ResourceType ResourceType {{ get {{ return ResourceType.{name}; }} }}");
                _writer.WriteLine(string.Empty);
            }

            WriteIndentedComment("FHIR Type Name");

            _writer.WriteLineIndented("[NotMapped]");
            _writer.WriteLineIndented($"public override string TypeName {{ get {{ return \"{name}\"; }} }}");

            _writer.WriteLine(string.Empty);
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
        /// <param name="primitives">The primitives.</param>
        private void WritePrimitiveTypes(
            IEnumerable<FhirPrimitive> primitives)
        {
            foreach (FhirPrimitive primitive in primitives)
            {
                if (_exclusionSet.Contains(primitive.Name))
                {
                    continue;
                }

                WritePrimitiveType(primitive);
            }
        }

        /// <summary>Writes a primitive type.</summary>
        /// <param name="primitive">The primitive.</param>
        private void WritePrimitiveType(
            FhirPrimitive primitive)
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

            _modelWriter.WriteLineIndented($"// {exportName}.cs");

            using (FileStream stream = new FileStream(filename, FileMode.Create))
            using (ExportStreamWriter writer = new ExportStreamWriter(stream))
            {
                _writer = writer;

                WriteHeaderPrimitive();

                WriteNamespaceOpen();

                if (!string.IsNullOrEmpty(primitive.Comment))
                {
                    WriteIndentedComment($"Primitive Type {primitive.Name}\n{primitive.Comment}");
                }
                else
                {
                    WriteIndentedComment($"Primitive Type {primitive.Name}");
                }

                _writer.WriteLineIndented($"[FhirType(\"{primitive.Name}\")]");
                _writer.WriteLineIndented("[DataContract]");

                _writer.WriteLineIndented(
                    $"public partial class" +
                        $" {exportName}" +
                        $" : {_namespace}.Primitive<{typeName}>," +
                        $" System.ComponentModel.INotifyPropertyChanged");

                // open class
                OpenScope();

                WritePropertyTypeName(primitive.Name, false, 0);

                _writer.WriteLine(string.Empty);

                if (!string.IsNullOrEmpty(primitive.ValidationRegEx))
                {
                    WriteIndentedComment(
                        $"Must conform to pattern \"{primitive.ValidationRegEx}\"",
                        false);

                    _writer.WriteLineIndented($"public const string PATTERN = @\"{primitive.ValidationRegEx}\";");

                    _writer.WriteLine(string.Empty);
                }

                _writer.WriteLineIndented($"public {exportName}({typeName} value)");
                OpenScope();
                _writer.WriteLineIndented("Value = value;");
                CloseScope();
                _writer.WriteLine(string.Empty);

                _writer.WriteLineIndented($"public {exportName}(): this(({typeName})null) {{}}");
                _writer.WriteLine(string.Empty);

                WriteIndentedComment("Primitive value of the element");

                _writer.WriteLineIndented("[FhirElement(\"value\", IsPrimitiveValue=true, XmlSerialization=XmlRepresentation.XmlAttr, InSummary=true, Order=30)]");
                _writer.WriteLineIndented("[DataMemeber]");
                _writer.WriteLineIndented($"public {typeName} Value");
                OpenScope();
                _writer.WriteLineIndented($"get {{ return ({typeName})ObjectValue; }}");
                _writer.WriteLineIndented("set { ObjectValue = value; OnPropertyChanged(\"Value\"); }");
                CloseScope();

                // close class
                CloseScope();

                WriteNamespaceClose();

                WriteFooter();
            }
        }

        /// <summary>Writes the namespace open.</summary>
        private void WriteNamespaceOpen()
        {
            _writer.WriteLineIndented($"namespace {_namespace}");
            OpenScope();
        }

        /// <summary>Writes the namespace close.</summary>
        private void WriteNamespaceClose()
        {
            CloseScope();
        }

        /// <summary>Writes a header.</summary>
        private void WriteHeaderBasic()
        {
            WriteGenerationComment();

            _writer.WriteLineIndented("using Hl7.Fhir.Utility;");
            _writer.WriteLine(string.Empty);

            WriteCopyright();
        }

        /// <summary>Writes a header.</summary>
        private void WriteHeaderComplexDataType()
        {
            WriteGenerationComment();

            _writer.WriteLineIndented("using System;");
            _writer.WriteLineIndented("using System.Collections.Generic;");
            _writer.WriteLineIndented("using System.Linq;");
            _writer.WriteLineIndented("using System.Runtime.Serialization;");
            _writer.WriteLineIndented("using Hl7.Fhir.Introspection;");
            _writer.WriteLineIndented("using Hl7.Fhir.Serialization;");
            _writer.WriteLineIndented("using Hl7.Fhir.Specification;");
            _writer.WriteLineIndented("using Hl7.Fhir.Utility;");
            _writer.WriteLineIndented("using Hl7.Fhir.Validation;");
            _writer.WriteLine(string.Empty);

            WriteCopyright();

#if DISABLED    // 2020.07.01 - should be exporting everything with necessary summary tags
            _writer.WriteLineI("#pragma warning disable 1591 // suppress XML summary warnings ");
            _writer.WriteLine(string.Empty);
#endif
        }

        /// <summary>Writes a header.</summary>
        private void WriteHeaderPrimitive()
        {
            WriteGenerationComment();

            _writer.WriteLineIndented("using System;");
            _writer.WriteLineIndented("using System.Collections.Generic;");
            _writer.WriteLineIndented("using System.Linq;");
            _writer.WriteLineIndented("using System.Runtime.Serialization;");
            _writer.WriteLineIndented("using Hl7.Fhir.Introspection;");
            _writer.WriteLineIndented("using Hl7.Fhir.Serialization;");
            _writer.WriteLineIndented("using Hl7.Fhir.Specification;");
            _writer.WriteLineIndented("using Hl7.Fhir.Utility;");
            _writer.WriteLineIndented("using Hl7.Fhir.Validation;");
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
            writer.WriteLineIndented($"// Generated by {_headerUserName} on {_headerGenerationDateTime}");

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
            WriteIndentedComment("end of file");
        }

        /// <summary>Opens the scope.</summary>
        private void OpenScope()
        {
            _writer.WriteLineIndented("{");
            _writer.IncreaseIndent();
        }

        /// <summary>Closes the scope.</summary>
        private void CloseScope()
        {
            _writer.DecreaseIndent();
            _writer.WriteLineIndented("}");
            _writer.WriteLine(string.Empty);
        }

        /// <summary>Writes an indented comment.</summary>
        /// <param name="value">    The value.</param>
        /// <param name="isSummary">(Optional) True if is summary, false if not.</param>
        private void WriteIndentedComment(string value, bool isSummary = true)
        {
            if (isSummary)
            {
                _writer.WriteLineIndented("/// <summary>");
            }

            string comment = value.Replace('\r', '\n').Replace("\r\n", "\n").Replace("\n\n", "\n").Replace("&", "&amp;");

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

        /// <summary>Information about a written value set.</summary>
        private struct WrittenValueSetInfo
        {
            internal string ClassName;
            internal string ValueSetName;
        }
    }
}
