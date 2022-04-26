// <copyright file= "CSharpFirely2IG.cs" company= "Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
using System.IO;
using Microsoft.Health.Fhir.SpecManager.Manager;
using Microsoft.Health.Fhir.SpecManager.Models;

namespace Microsoft.Health.Fhir.SpecManager.Language
{
    /// <summary>A language exporter for Firely-compliant C# FHIR JSON Serialization Extension output.</summary>
    public sealed class CSharpIGExtensions : ILanguage
    {
        /// <summary>The namespace to use during export.</summary>
        private const string _modelNamespace = "Hl7.Fhir.Model";

        /// <summary>The serialization namespace.</summary>
        private const string _serializationNamespace = "Hl7.Fhir.Serialization";

        /// <summary>Name of the header user.</summary>
        private string _headerUserName;

        /// <summary>The header generation date time.</summary>
        private string _headerGenerationDateTime;

        /// <summary>FHIR information we are exporting.</summary>
        private FhirVersionInfo _info;

        /// <summary>Options for controlling the export.</summary>
        private ExporterOptions _options;

        /// <summary>Keep track of information about written value sets.</summary>
        private Dictionary<string, WrittenValueSetInfo> _writtenValueSets = new Dictionary<string, WrittenValueSetInfo>();

        /// <summary>The written resources.</summary>
        private Dictionary<string, WrittenModelInfo> _writtenModels = new Dictionary<string, WrittenModelInfo>();

        /// <summary>The written converters.</summary>
        private List<string> _writtenConverters = new List<string>();

        /// <summary>The split characters.</summary>
        private static readonly char[] _splitChars = { '|', ' ' };

        /// <summary>The currently in-use text writer.</summary>
        private ExportStreamWriter _writer;

        /// <summary>Pathname of the export directory.</summary>
        private string _exportDirectory;

        /// <summary>Name of the language.</summary>
        private const string _languageName = "CSharpIGExtensions";

        /// <summary>The single file export extension (uses directory export).</summary>
        private const string _singleFileExportExtension = "cs";

        /// <summary>Structures to skip generating.</summary>
        private static HashSet<string> _exclusionSet = new HashSet<string>()
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
        /// List of complex datatype classes that are part of the 'common' subset. See <see cref= "GenSubset"/>.
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
        /// List of resource classes that are part of the 'common' subset. See <see cref= "GenSubset"/>.
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

        private static string[] excludeFromCsToFhir =
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

        private static Dictionary<string, string> _elementNameReplacementsByPath = new Dictionary<string, string>()
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

        List<FhirComplex> _duplicateNames;

        /// <summary>Export the passed FHIR version into the specified directory.</summary>
        /// <param name= "info">           The information.</param>
        /// <param name= "serverInfo">     Information describing the server.</param>
        /// <param name= "options">        Options for controlling the operation.</param>
        /// <param name= "exportDirectory">Directory to write files.</param>
        void ILanguage.Export(
            FhirVersionInfo info,
            FhirServerInfo serverInfo,
            ExporterOptions options,
            string exportDirectory)
        {
            // set internal vars so we don't pass them to every function
            _info = info;
            info.FhirSequence = FhirPackageCommon.MajorReleaseForVersion(info.ReleaseName); // no idea why this is defaulting in DSTU2
            _options = options;
            _exportDirectory = exportDirectory;
            _writtenValueSets = new Dictionary<string, WrittenValueSetInfo>();

            if (!Directory.Exists(exportDirectory))
            {
                Directory.CreateDirectory(exportDirectory);
            }

            _headerUserName = Environment.UserName;
            _headerGenerationDateTime = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss", null);

            string filename = Path.Combine(_exportDirectory, $"{info.PackageName}.cs");

            using (FileStream stream = new FileStream(filename, FileMode.Create))
            using (ExportStreamWriter writer = new ExportStreamWriter(stream))
            {
                _writer = writer;

                WriteHeaderJsonExt();

                WriteNamespaceExtensionsOpen();

                WriteIndentedComment($"Implementation Guide Extension getter/setter extension methods for {info.PackageName}");
                _writer.WriteLineIndented($"public static class {info.PackageName.Replace(".", "_")}_Extensions");

                // Before processing any of these items, ensure that all extensions have a unique name
                Dictionary<string, int> nameCounts = new Dictionary<string, int>();
                foreach (var val in _info.ExtensionsByUrl.Values)
                {
                    if (nameCounts.ContainsKey(val.Name))
                    {
                        nameCounts[val.Name]++;
                    }
                    else
                    {
                        nameCounts.Add(val.Name, 1);
                    }
                }

                // this set of names should be de-duplicated (except the name property is read-only)
                _duplicateNames = _info.ExtensionsByUrl.Values.Where(v => nameCounts.Any(n => v.Name == n.Key && n.Value > 1)).ToList();

                // open class
                OpenScope();

                foreach (FhirComplex model in _info.ExtensionsByUrl.Values)
                {
                    WriteExtension(model);
                }

                CloseScope();
                WriteNamespaceClose();
                WriteFooter();
            }
        }

        private string ConvertName(FhirComplex complex)
        {
            if (complex.Name == "namespace")
            {
                return "namespace_";
            }

            if (_duplicateNames.Contains(complex))
            {
                return complex.Name.Replace("-", "_").Replace(" ", "_") + $"_{_duplicateNames.Where(v => v.Name == complex.Name).ToList().IndexOf(complex)}";
            }

            return complex.Name.Replace("-", "_").Replace(" ", "_");
        }

        /// <summary>Writes an Extension</summary>
        /// <param name= "complex">       The complex data type.</param>
        private void WriteExtension(
            FhirComplex complex)
        {
            string exportName = complex.NameForExport(FhirTypeBase.NamingConvention.PascalCase);

            bool isAbstract = complex.IsAbstract;

            // open class
            // OpenExtensionClass(complex.Name, exportName);
            _writer.WriteLineIndented($"#region << {complex.Name} >>");

            WriteIndentedComment($"<summary>", false, false, false);
            WriteIndentedComment($"{complex.ShortDescription}", false, false, false);
            WriteIndentedComment($"</summary>", false, false, false);
            _writer.WriteLineIndented($"public const string exturl_{ConvertName(complex)} = \"{complex.URL}\";");
            _writer.WriteLine();

            // Contexts for this extension:
            string context = "IExtendable";

            if (complex.ContextElements.Count == 1)
            {
                var simpleContext = complex.ContextElements[0];
                if (!simpleContext.Contains("."))
                {
                    context = simpleContext;
                }
                else
                {
                    System.Diagnostics.Trace.WriteLine($"Unknown C# type {simpleContext}");
                }
            }

            var elementTypes = complex.Elements.FirstOrDefault(e => e.Key == "Extension.value[x]").Value?.ElementTypes;

            // Output the getter function(s)
            if (elementTypes?.Count == 1)
            {
                WriteAccessorForDataType(complex, context, elementTypes.First().Value.Name, false);
            }
            else
            {
                WriteAccessorForDataType(complex, context, "DataType", false);
                if (elementTypes != null)
                {
                    foreach (var et in elementTypes)
                    {
                        WriteAccessorForDataType(complex, context, et.Value.Name, true);
                    }
                }
            }

            _writer.WriteLineIndented($"#endregion");
            _writer.WriteLine();

            // close class
            // CloseScope();
        }

        private void WriteAccessorForDataType(FhirComplex complex, string context, string valueDataType, bool usingCast)
        {
            // remap the type name
            if (CSharpFirelyCommon.TypeNameMappings.ContainsKey(valueDataType))
            {
                valueDataType = CSharpFirelyCommon.TypeNameMappings[valueDataType];
            }

            if (valueDataType == "code")
            {
                valueDataType = "Code";
            }

            if (CSharpFirelyCommon.TypeNameMappings.ContainsKey(context))
            {
                context = CSharpFirelyCommon.TypeNameMappings[context];
            }

            if (context == "code")
            {
                context = "Code";
            }
            if (context == "Resource")
            {
                context = "DomainResource"; // If you define Resource as the context, it doesn't support that, so substitue to DomainResource
            }

            WriteIndentedComment($"<summary>", false, false, false);
            if (complex.RootElement.CardinalityMax != 1)
            {
                WriteIndentedComment($"(first only) {complex.ShortDescription}", false);
            }
            else
            {
                WriteIndentedComment($"{complex.ShortDescription}", false);
            }

            WriteIndentedComment($"<br/><code>{complex.URL}</code>", false, false, false);
            WriteIndentedComment($"</summary>", false, false, false);
            _writer.WriteLineIndented($"public static {valueDataType} {ConvertName(complex)}{(usingCast ? $"As{valueDataType}" : null)}(this {context} me)");
            OpenScope();
            _writer.WriteLineIndented($"return me.GetExtensionValue<{valueDataType}>(exturl_{ConvertName(complex)});");
            CloseScope();

            // If this is a multi-cardinality extension, then we should also output the collection based version of this
            if (complex.RootElement.CardinalityMax != 1)
            {
                WriteIndentedComment($"<summary>", false, false, false);
                WriteIndentedComment($"(collection) {complex.ShortDescription}", false);
                WriteIndentedComment($"<br/><code>{complex.URL}</code>", false, false, false);
                WriteIndentedComment($"</summary>", false, false, false);
                _writer.WriteLineIndented($"public static IEnumerable<{valueDataType}> {ConvertName(complex)}s{(usingCast ? $"As{valueDataType}" : null)}(this {context} me)");
                OpenScope();
                _writer.WriteLineIndented($"return me.GetExtensions(exturl_{ConvertName(complex)}).Select(e => e.Value{(valueDataType != "DataType" ? $" as {valueDataType}" : null)});");
                CloseScope();

            }
        }


        /// <summary>Writes the enums.</summary>
        /// <param name= "complex">      The complex data type.</param>
        /// <param name= "className">    Name of the class this enum is being written in.</param>
        /// <param name= "usedEnumNames">(Optional) List of names of the used enums.</param>
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
        /// <param name= "vs">       The vs.</param>
        /// <param name= "className">Name of the class this enum is being written in.</param>
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

            string name = (vs.Name ?? vs.Id).Replace("", string.Empty, StringComparison.Ordinal).Replace("_", string.Empty, StringComparison.Ordinal);
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


        /// <summary>Builds type from path.</summary>
        /// <param name= "type">The type.</param>
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


        /// <summary>Opens extension class.</summary>
        /// <param name= "fhirName">  Name of the FHIR.</param>
        /// <param name= "exportName">Name of the export.</param>
        private void OpenExtensionClass(string fhirName, string exportName)
        {
            WriteIndentedComment($"JSON Serialization Extensions for {fhirName}");
            _writer.WriteLineIndented($"public static class {exportName}JsonExtensions");

            // open class
            OpenScope();
        }


        /// <summary>Writes the namespace open.</summary>
        private void WriteNamespaceOpen()
        {
            _writer.WriteLineIndented($"namespace {_modelNamespace}.JsonExtensions");
            OpenScope();
        }

        /// <summary>Writes the namespace open.</summary>
        private void WriteNamespaceExtensionsOpen()
        {
            _writer.WriteLineIndented($"namespace {_modelNamespace}.test");
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
            _writer.WriteLineIndented("using System.Linq;");
            // _writer.WriteLineIndented("using System.Text.Json.Serialization;");
            _writer.WriteLineIndented($"using {_modelNamespace};");
            // _writer.WriteLineIndented($"using {_modelNamespace}.JsonExtensions;");
            // _writer.WriteLineIndented($"using {_serializationNamespace};");
            _writer.WriteLine(string.Empty);

            WriteCopyright();
        }

        /// <summary>Writes the generation comment.</summary>
        /// <param name= "writer">(Optional) The currently in-use text writer.</param>
        private void WriteGenerationComment(ExportStreamWriter writer = null)
        {
            if (writer == null)
            {
                writer = _writer;
            }

            writer.WriteLineIndented("// <auto-generated/>");
            writer.WriteLineIndented($"// Contents of: {_info.PackageName} version: {_info.VersionString}");
#if !DEBUG
            writer.WriteLineIndented($"// Generated by {_headerUserName} on {_headerGenerationDateTime}");
#endif
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
            _writer.WriteLine();
            _writer.WriteLineIndented("  Redistribution and use in source and binary forms, with or without modification,");
            _writer.WriteLineIndented("  are permitted provided that the following conditions are met:");
            _writer.WriteLine();
            _writer.WriteLineIndented("   * Redistributions of source code must retain the above copyright notice, this");
            _writer.WriteLineIndented("     list of conditions and the following disclaimer.");
            _writer.WriteLineIndented("   * Redistributions in binary form must reproduce the above copyright notice,");
            _writer.WriteLineIndented("     this list of conditions and the following disclaimer in the documentation");
            _writer.WriteLineIndented("     and/or other materials provided with the distribution.");
            _writer.WriteLineIndented("   * Neither the name of HL7 nor the names of its contributors may be used to");
            _writer.WriteLineIndented("     endorse or promote products derived from this software without specific");
            _writer.WriteLineIndented("     prior written permission.");
            _writer.WriteLine();
            _writer.WriteLineIndented("  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS \"AS IS\" AND");
            _writer.WriteLineIndented("  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED");
            _writer.WriteLineIndented("  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.");
            _writer.WriteLineIndented("  IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,");
            _writer.WriteLineIndented("  INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT");
            _writer.WriteLineIndented("  NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR");
            _writer.WriteLineIndented("  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,");
            _writer.WriteLineIndented("  WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)");
            _writer.WriteLineIndented("  ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE");
            _writer.WriteLineIndented("  POSSIBILITY OF SUCH DAMAGE.");
            _writer.WriteLine();
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
        /// <param name= "value">    The value.</param>
        /// <param name= "isSummary">(Optional) True if is summary, false if not.</param>
        private void WriteIndentedComment(string value, bool isSummary = true, bool singleLine = false, bool encodeHtml = true)
            => CSharpFirelyCommon.WriteIndentedComment(_writer, value, isSummary, singleLine, encodeHtml);

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
