// <copyright file="CSharpFirely2.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Health.Fhir.SpecManager.Manager;
using Microsoft.Health.Fhir.SpecManager.Models;

namespace Microsoft.Health.Fhir.SpecManager.Language
{
    /// <summary>A language exporter for Firely-compliant C# FHIR output.</summary>
    public sealed class CSharpFirely2 : ILanguage
    {
        /// <summary>The namespace to use during export.</summary>
        private const string Namespace = "Hl7.Fhir.Model";

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

        /// <summary>The split characters.</summary>
        private static readonly char[] _splitChars = { '|', ' ' };

        /// <summary>The currently in-use text writer.</summary>
        private ExportStreamWriter _writer;

        /// <summary>The model writer.</summary>
        private ExportStreamWriter _modelWriter;

        /// <summary>Pathname of the export directory.</summary>
        private string _exportDirectory;

        /// <summary>Name of the language.</summary>
        private const string _languageName = "CSharpFirely2";

        /// <summary>The single file export extension (uses directory export).</summary>
        private const string _singleFileExportExtension = null;

        /// <summary>Structures to skip generating.</summary>
        private static readonly HashSet<string> _exclusionSet = new HashSet<string>()
        {
            /* Since Base defines its methods abstractly, the pattern for generating it
             * is sufficiently different from derived classes that it makes sense not
             * to generate the methods (it's pretty empty too - no members on this abstract class) */
            "Base",

            /* PrimitiveType defines the magic `ObjectValue` member used by all derived
             * primitives to store their value. This makes the CopyTo(), IsExact() methods
             * different enough that it does not make sense to generate them. */
            "PrimitiveType",

            /* Element has the special `id` element, that is both an attribute in the
             * XML serialization and is not using a FHIR primitive for representation. Consequently,
             * the generated CopyTo() and IsExact() methods diverge too much to be useful. */
            "Element",

            /* Extension has the special `url` element, that is both an attribute in the
             * XML serialization and is not using a FHIR primitive for representation. Consequently,
             * the generated CopyTo() and IsExact() methods diverge too much to be useful. */
            "Extension",

            /* Narrative has a special `div` element, serialized as an element frm the
             * XHTML namespace, not using a normal FHIR primitive. This makes this class
             * deviate in ways we cannot achieve with the generator. */
            "Narrative",

            /* These two types are interfaces rather than classes (at least, for now)
             * so we're not generating them. Also, all types deriving from these
             * are generated to derive from DomainResource instead */
            "CanonicalResource",
            "MetadataResource",

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
            new WrittenModelInfo { CsName = "BackboneType", FhirName = "BackboneType", IsAbstract = true },
            new WrittenModelInfo { CsName = "Base", FhirName = "Base", IsAbstract = true },
            new WrittenModelInfo { CsName = "DataType", FhirName = "DataType", IsAbstract = true },
            new WrittenModelInfo { CsName = "PrimitiveType", FhirName = "PrimitiveType", IsAbstract = true },
        };

        /// <summary>
        /// List of complex datatype classes that are part of the 'common' subset. See <see cref="GenSubset"/>.
        /// </summary>
        private static readonly List<string> _commmonComplexTypes = new List<string>()
        {
            "BackboneElement",
            "BackboneType",
            "Base",
            "CodeableConcept",
            "Coding",
            "ContactPoint",
            "ContactDetail",
            "DataType",
            "Element",
            "Extension",
            "Identifier",
            "Meta",
            "Narrative",
            "Period",
            "PrimitiveType",
            "Quantity",
            "Range",
            "Reference",
            "UsageContext",
        };

        /// <summary>
        /// List of resource classes that are part of the 'common' subset. See <see cref="GenSubset"/>.
        /// </summary>
        private static readonly List<string> _commmonResourceTypes = new List<string>()
        {
            "DomainResource",
            "OperationOutcome",
            "Parameters",
            "Resource",
         /*   "CodeSystem",
            "ValueSet", */
        };

        private static readonly string[] _allCommonTypes = _commmonComplexTypes
            .Concat(_commmonResourceTypes)
            .Concat(_commonR5DataTypes.Select(r5dt => r5dt.FhirName)).ToArray();

        private static readonly List<string> _commonValueSets = new List<string>()
        {
            "http://hl7.org/fhir/ValueSet/filter-operator",
            "http://hl7.org/fhir/ValueSet/publication-status",
        };

        /// <summary>Gets the reserved words.</summary>
        /// <value>The reserved words.</value>
        private static readonly HashSet<string> _reservedWords = new HashSet<string>();

        private static readonly Func<WrittenModelInfo, bool> SupportedResourcesFilter = wmi => !wmi.IsAbstract;
        private static readonly Func<WrittenModelInfo, bool> FhirToCsFilter = wmi => !ExcludeFromCsToFhir.Contains(wmi.FhirName);
        private static readonly Func<WrittenModelInfo, bool> CsToStringFilter = FhirToCsFilter;

        private static readonly string[] ExcludeFromCsToFhir =
        {
            "CanonicalResource",
            "MetadataResource",
            "Citation",
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
            /* For the ExporterOptions.LanguageOptions we accept:
             *    subset=common|main        'common' will generate code for 'common' repo of the library,
             *                              'main' will genreate code for the version-specific 'api' (main) repo of the library
             */
            GenSubset subset = GenSubset.Main;

            if (options.LanguageOptions.TryGetValue("subset", out string ss))
            {
                subset = ss == "common" ? GenSubset.Common : ss == "main" ? GenSubset.Main : GenSubset.All;
            }

            if (subset.HasFlag(GenSubset.Common) && info.MajorVersion != 5)
            {
                Console.WriteLine($"Aborting {_languageName} for {info.MajorVersion}: code generation for the 'common' subset should be run on r5 only.");
                return;
            }

            // set internal vars so we don't pass them to every function
            _info = info;
            _options = options;
            _exportDirectory = exportDirectory;
            _writtenValueSets = new Dictionary<string, WrittenValueSetInfo>();

            if (!Directory.Exists(exportDirectory))
            {
                Directory.CreateDirectory(exportDirectory);
            }

            if (!Directory.Exists(Path.Combine(exportDirectory, "Generated")))
            {
                Directory.CreateDirectory(Path.Combine(exportDirectory, "Generated"));
            }

            _headerUserName = Environment.UserName;
            _headerGenerationDateTime = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss", null);

            Dictionary<string, WrittenModelInfo> allPrimitives = new Dictionary<string, WrittenModelInfo>();
            Dictionary<string, WrittenModelInfo> allComplexTypes = new Dictionary<string, WrittenModelInfo>();
            Dictionary<string, WrittenModelInfo> allResources = new Dictionary<string, WrittenModelInfo>();
            Dictionary<string, WrittenModelInfo> dummy = new Dictionary<string, WrittenModelInfo>();

            string infoFilename = Path.Combine(_exportDirectory, "Generated", "_GeneratorLog.cs");

            using (FileStream infoStream = new FileStream(infoFilename, FileMode.Create))
            using (ExportStreamWriter infoWriter = new ExportStreamWriter(infoStream))
            {
                _modelWriter = infoWriter;

                WriteGenerationComment(infoWriter);

                if (options.OptionalClassTypesToExport.Contains(ExporterOptions.FhirExportClassType.Enum))
                {
                    WriteSharedValueSets(subset);
                }

                _modelWriter.WriteLineIndented("// Generated items");

                if (options.OptionalClassTypesToExport.Contains(ExporterOptions.FhirExportClassType.PrimitiveType) && subset.HasFlag(GenSubset.Common))
                {
                    WritePrimitiveTypes(_info.PrimitiveTypes.Values, ref dummy);
                }

                AddModels(allPrimitives, _info.PrimitiveTypes.Values);

                if (options.OptionalClassTypesToExport.Contains(ExporterOptions.FhirExportClassType.ComplexType))
                {
                    WriteComplexDataTypes(_info.ComplexTypes.Values, ref dummy, subset);
                }

                AddModels(allComplexTypes, _info.ComplexTypes.Values);
                AddModels(allComplexTypes, _commonR5DataTypes);

                if (options.OptionalClassTypesToExport.Contains(ExporterOptions.FhirExportClassType.Resource))
                {
                    WriteResources(_info.Resources.Values, ref dummy, subset);
                }

                AddModels(allResources, _info.Resources.Values);

                if (subset.HasFlag(GenSubset.Main))
                {
                    WriteModelInfo(allPrimitives, allComplexTypes, allResources);
                }
            }
        }

        /// <summary>Writes a model information.</summary>
        /// <param name="writtenPrimitives">   The written primitives.</param>
        /// <param name="writtenComplexTypes">List of types of the written complexes.</param>
        /// <param name="writtenResources">   The written resources.</param>
        private void WriteModelInfo(
            Dictionary<string, WrittenModelInfo> writtenPrimitives,
            Dictionary<string, WrittenModelInfo> writtenComplexTypes,
            Dictionary<string, WrittenModelInfo> writtenResources)
        {
            string filename = Path.Combine(_exportDirectory, "Generated", "Template-ModelInfo.cs");

            using (FileStream stream = new FileStream(filename, FileMode.Create))
            using (ExportStreamWriter writer = new ExportStreamWriter(stream))
            {
                _writer = writer;

                WriteGenerationComment();

                _writer.WriteLineIndented("using System;");
                _writer.WriteLineIndented("using System.Collections.Generic;");
                _writer.WriteLineIndented("using Hl7.Fhir.Introspection;");
                _writer.WriteLineIndented("using Hl7.Fhir.Validation;");
                _writer.WriteLineIndented("using System.Linq;");
                _writer.WriteLineIndented("using System.Runtime.Serialization;");
                _writer.WriteLine(string.Empty);

                WriteCopyright();

                WriteNamespaceOpen();

                WriteIndentedComment(
                    "A class with methods to retrieve information about the\n" +
                    "FHIR definitions based on which this assembly was generated.");

                _writer.WriteLineIndented("public static partial class ModelInfo");

                // open class
                OpenScope();

                WriteSupportedResources(writtenResources.Values.Where(SupportedResourcesFilter));

                WriteFhirVersion();

                WriteFhirToCs(writtenPrimitives.Values.Where(FhirToCsFilter), writtenComplexTypes.Values.Where(FhirToCsFilter), writtenResources.Values.Where(FhirToCsFilter));
                WriteCsToString(writtenPrimitives.Values.Where(CsToStringFilter), writtenComplexTypes.Values.Where(CsToStringFilter), writtenResources.Values.Where(CsToStringFilter));

                WriteSearchParameters();

                // close class
                CloseScope();

                WriteNamespaceClose();
            }
        }

        /// <summary>Writes the search parameters.</summary>
        private void WriteSearchParameters()
        {
            _writer.WriteLineIndented("public static List<SearchParamDefinition> SearchParameters = new List<SearchParamDefinition>()");
            OpenScope();

            foreach (FhirComplex complex in _info.Resources.Values)
            {
                if (complex.SearchParameters == null)
                {
                    continue;
                }

                foreach (FhirSearchParam sp in complex.SearchParameters.Values.OrderBy(s => s.Name))
                {
                    if (sp.IsExperimental)
                    {
                        continue;
                    }

                    string description;

                    if ((!string.IsNullOrEmpty(sp.Description)) &&
                        sp.Description.StartsWith("Multiple", StringComparison.Ordinal))
                    {
                        description = string.Empty;
                    }
                    else
                    {
                        description = sp.Description;
                    }

                    string searchType = FhirUtils.SanitizedToConvention(sp.ValueType, FhirTypeBase.NamingConvention.PascalCase);
                    string path = string.Empty;

                    if (!string.IsNullOrEmpty(sp.XPath))
                    {
#pragma warning disable CA1307 // Specify StringComparison
                        string temp = sp.XPath.Replace("f:", string.Empty).Replace('/', '.').Replace('(', '[').Replace(')', ']');
#pragma warning restore CA1307 // Specify StringComparison

                        IEnumerable<string> split = temp
                            .Split(_splitChars, StringSplitOptions.RemoveEmptyEntries)
                            .Where(s => s.StartsWith(complex.Name + ".", StringComparison.Ordinal));

                        path = "\"" + string.Join("\", \"", split) + "\", ";
                    }

                    string target;

                    if ((sp.Targets == null) || (sp.Targets.Count == 0))
                    {
                        target = string.Empty;
                    }
                    else
                    {
                        SortedSet<string> sc = new SortedSet<string>();

                        foreach (string t in sp.Targets)
                        {
                            sc.Add("ResourceType." + t);
                        }

                        // HACK: for http://hl7.org/fhir/SearchParameter/clinical-encounter,
                        // none of the base resources have EpisodeOfCare as target, except
                        // Procedure and DeviceRequest. There is no way you can see this from the
                        // source data we generate this from, afaik, so we need to make
                        // a special case here.
                        // Brian P reported that there are many such exceptions - but this one
                        // was reported as a bug. Again, there is no way to know this from our
                        // inputs, so this will remain manually maintained input.
                        if (sp.Id == "clinical-encounter")
                        {
                            if (_info.MajorVersion == 3)
                            {
                                if (complex.Name != "Procedure" && complex.Name != "DeviceRequest")
                                {
                                    sc.Remove("ResourceType.EpisodeOfCare");
                                }
                            }
                            else
                            {
                                if (complex.Name != "DocumentReference")
                                {
                                    sc.Remove("ResourceType.EpisodeOfCare");
                                }
                            }
                        }

                        target = ", Target = new ResourceType[] { " + string.Join(", ", sc) + ", }";
                    }

                    string xpath = string.IsNullOrEmpty(sp.XPath) ? xpath = string.Empty : ", XPath = \"" + sp.XPath + "\"";
                    string expression = string.IsNullOrEmpty(sp.Expression) ? string.Empty : ", Expression = \"" + sp.Expression + "\"";
                    string urlComponent = $", Url = \"{sp.URL}\"";

                    _writer.WriteLineIndented(
                        $"new SearchParamDefinition() " +
                            $"{{" +
                            $" Resource = \"{complex.Name}\"," +
                            $" Name = \"{sp.Name}\"," +
                            (_info.MajorVersion == 3 ?
                                $" Description = @\"{SanitizeForMarkdown(description)}\"," :
                                $" Description = new Markdown(@\"{SanitizeForMarkdown(description)}\"),") +
                            $" Type = SearchParamType.{searchType}," +
                            $" Path = new string[] {{ {path}}}" +
                            target +
                            xpath +
                            expression +
                            urlComponent +
                            $" }},");
                }
            }

            CloseScope(true);
        }

        /// <summary>Sanitize for markdown.</summary>
        /// <param name="value">The value.</param>
        private static string SanitizeForMarkdown(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

#pragma warning disable CA1307 // Specify StringComparison
            return value.Replace("\"", "\"\"").Replace("\r", @"\r").Replace("\n", @"\n");
#pragma warning restore CA1307 // Specify StringComparison
        }

        /// <summary>Writes the C# to FHIR map dictionary.</summary>
        /// <param name="writtenPrimitives">  The written primitives.</param>
        /// <param name="writtenComplexTypes">List of types of the written complexes.</param>
        /// <param name="writtenResources">   The written resources.</param>
        private void WriteCsToString(
            IEnumerable<WrittenModelInfo> writtenPrimitives,
            IEnumerable<WrittenModelInfo> writtenComplexTypes,
            IEnumerable<WrittenModelInfo> writtenResources)
        {
            _writer.WriteLineIndented("public static Dictionary<Type,string> FhirCsTypeToString = new Dictionary<Type,string>()");
            OpenScope();

            foreach (WrittenModelInfo type in writtenPrimitives.Concat(writtenComplexTypes).OrderBy(t => t.FhirName))
            {
                _writer.WriteLineIndented($"{{ typeof({type.CsName}), \"{type.FhirName}\" }},");
            }

            _writer.WriteLine(string.Empty);

            foreach (WrittenModelInfo type in writtenResources.OrderBy(t => t.FhirName))
            {
                _writer.WriteLineIndented($"{{ typeof({type.CsName}), \"{type.FhirName}\" }},");
            }

            CloseScope(true);
        }

        /// <summary>Writes the FHIR to C# map dictionary.</summary>
        /// <param name="writtenPrimitives">  The written primitives.</param>
        /// <param name="writtenComplexTypes">List of types of the written complexes.</param>
        /// <param name="writtenResources">   The written resources.</param>
        private void WriteFhirToCs(
            IEnumerable<WrittenModelInfo> writtenPrimitives,
            IEnumerable<WrittenModelInfo> writtenComplexTypes,
            IEnumerable<WrittenModelInfo> writtenResources)
        {
            _writer.WriteLineIndented("public static Dictionary<string,Type> FhirTypeToCsType = new Dictionary<string,Type>()");
            OpenScope();

            foreach (WrittenModelInfo type in writtenPrimitives.Concat(writtenComplexTypes).OrderBy(t => t.FhirName))
            {
                _writer.WriteLineIndented($"{{ \"{type.FhirName}\", typeof({type.CsName}) }},");
            }

            _writer.WriteLine(string.Empty);

            foreach (WrittenModelInfo type in writtenResources.OrderBy(t => t.FhirName))
            {
                _writer.WriteLineIndented($"{{ \"{type.FhirName}\", typeof({type.CsName}) }},");
            }

            CloseScope(true);
        }

        /// <summary>Writes the FHIR version.</summary>
        private void WriteFhirVersion()
        {
            _writer.WriteLineIndented("public static string Version");
            OpenScope();
            _writer.WriteLineIndented($"get {{ return \"{_info.VersionString}\"; }}");
            CloseScope();
        }

        /// <summary>Writes the supported resources dictionary.</summary>
        /// <param name="resources">The written resources.</param>
        private void WriteSupportedResources(IEnumerable<WrittenModelInfo> resources)
        {
            _writer.WriteLineIndented("public static List<string> SupportedResources = new List<string>()");
            OpenScope();

            foreach (WrittenModelInfo wmi in resources.OrderBy(s => s.FhirName))
            {
                _writer.WriteLineIndented($"\"{wmi.FhirName}\",");
            }

            CloseScope(true);
        }

        /// <summary>Writes the common enums.</summary>
        private void WriteSharedValueSets(GenSubset subset)
        {
            HashSet<string> usedEnumNames = new HashSet<string>();

            string filename = Path.Combine(_exportDirectory, "Generated", "Template-Bindings.cs");

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
                        if (vs.ReferencedByComplexes.Count < 2)
                        {
                            /* ValueSets that are used in a single POCO are generated as a nested enum inside that
                             * POCO, not here in the shared valuesets */
                            continue;
                        }

                        if (vs.StrongestBinding != FhirElement.ElementDefinitionBindingStrength.Required)
                        {
                            /* Since required bindings cannot be extended, those are the only bindings that
                               can be represented using enums in the POCO classes (using <c>Code&lt;T&gt;</c>). All other coded members
                               use <c>Code</c>, <c>Coding</c> or <c>CodeableConcept</c>.
                               Consequently, we only need to generate enums for valuesets that are used as
                               required bindings anywhere in the datamodel. */
                            continue;
                        }

                        if (_exclusionSet.Contains(vs.URL))
                        {
                            continue;
                        }

                        if (subset.HasFlag(GenSubset.Common) && !_commonValueSets.Contains(vs.URL))
                        {
                            continue;
                        }

                        // If this is a shared valueset that will be generated in the common library,
                        // don't also generate it here.
                        bool suppressWriteValueSet = subset.HasFlag(GenSubset.Main) && _commonValueSets.Contains(vs.URL);
                        WriteEnum(vs, string.Empty, usedEnumNames, silent: suppressWriteValueSet);

                        if (!suppressWriteValueSet)
                        {
                            _modelWriter.WriteLineIndented($"// Generated Shared Enumeration: {_writtenValueSets[vs.URL].ValueSetName} ({vs.URL})");
                        }
                        else
                        {
                            _modelWriter.WriteLineIndented($"// Deferred generation of Shared Enumeration (will be generated in common): {_writtenValueSets[vs.URL].ValueSetName} ({vs.URL})");
                        }

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
        /// <param name="complexes">    The complex data types.</param>
        /// <param name="writtenModels">[in,out] The written models.</param>
        private void WriteResources(
            IEnumerable<FhirComplex> complexes,
            ref Dictionary<string, WrittenModelInfo> writtenModels,
            GenSubset subset)
        {
            foreach (FhirComplex complex in complexes.OrderBy(c => c.Name))
            {
                if (_exclusionSet.Contains(complex.Name))
                {
                    continue;
                }

                if ((subset.HasFlag(GenSubset.Common) && _commmonResourceTypes.Contains(complex.Name)) ||
                    (subset.HasFlag(GenSubset.Main) && !_commmonResourceTypes.Contains(complex.Name)))
                {
                    WriteResource(complex, ref writtenModels, subset);
                }
            }
        }

        /// <summary>Writes a complex data type.</summary>
        /// <param name="complex">      The complex data type.</param>
        /// <param name="writtenModels">[in,out] The written models.</param>
        private void WriteResource(
            FhirComplex complex,
            ref Dictionary<string, WrittenModelInfo> writtenModels,
            GenSubset subset)
        {
            string exportName = complex.NameForExport(FhirTypeBase.NamingConvention.PascalCase);

            writtenModels.Add(
                complex.Name,
                new WrittenModelInfo()
                {
                    FhirName = complex.Name,
                    CsName = $"{Namespace}.{exportName}",
                    IsAbstract = complex.IsAbstract,
                });

            string filename = Path.Combine(_exportDirectory, "Generated", $"{exportName}.cs");

            _modelWriter.WriteLineIndented($"// {exportName}.cs");

            using (FileStream stream = new FileStream(filename, FileMode.Create))
            using (ExportStreamWriter writer = new ExportStreamWriter(stream))
            {
                _writer = writer;

                WriteHeaderComplexDataType();

                WriteNamespaceOpen();

                WriteComponent(complex, exportName, true, 0, subset);

                WriteNamespaceClose();

                WriteFooter();
            }
        }

        /// <summary>Writes the complex data types.</summary>
        /// <param name="complexes">    The complex data types.</param>
        /// <param name="writtenModels">[in,out] The written models.</param>
        private void WriteComplexDataTypes(
            IEnumerable<FhirComplex> complexes,
            ref Dictionary<string, WrittenModelInfo> writtenModels,
            GenSubset subset)
        {
            foreach (FhirComplex complex in complexes.OrderBy(c => c.Name))
            {
                if (_exclusionSet.Contains(complex.Name))
                {
                    continue;
                }

                if ((subset.HasFlag(GenSubset.Common) && _commmonComplexTypes.Contains(complex.Name)) ||
                    (subset.HasFlag(GenSubset.Main) && !_commmonComplexTypes.Contains(complex.Name)))
                {
                    WriteComplexDataType(complex, ref writtenModels, subset);
                }
            }
        }

        /// <summary>Writes a complex data type.</summary>
        /// <param name="complex">      The complex data type.</param>
        /// <param name="writtenModels">[in,out] The written models.</param>
        private void WriteComplexDataType(
            FhirComplex complex,
            ref Dictionary<string, WrittenModelInfo> writtenModels,
            GenSubset subset)
        {
            string exportName = complex.NameForExport(FhirTypeBase.NamingConvention.PascalCase);

            if (CSharpFirelyCommon.TypeNameMappings.ContainsKey(exportName))
            {
                exportName = CSharpFirelyCommon.TypeNameMappings[exportName];
            }

            writtenModels.Add(
                complex.Name,
                new WrittenModelInfo()
                {
                    FhirName = complex.Name,
                    CsName = $"{Namespace}.{exportName}",
                    IsAbstract = complex.IsAbstract,
                });

            string filename = Path.Combine(_exportDirectory, "Generated", $"{exportName}.cs");

            _modelWriter.WriteLineIndented($"// {exportName}.cs");

            using (FileStream stream = new FileStream(filename, FileMode.Create))
            using (ExportStreamWriter writer = new ExportStreamWriter(stream))
            {
                _writer = writer;

                WriteHeaderComplexDataType();

                WriteNamespaceOpen();

                WriteComponent(complex, exportName, false, 0, subset);

                WriteNamespaceClose();

                WriteFooter();
            }
        }

        /// <summary>Writes a component.</summary>
        /// <param name="complex">              The complex data type.</param>
        /// <param name="exportName">           Name of the export.</param>
        /// <param name="isResource">           True if is resource, false if not.</param>
        /// <param name="depth">                The depth.</param>
        private void WriteComponent(
            FhirComplex complex,
            string exportName,
            bool isResource,
            int depth,
            GenSubset subset)
        {
            bool isAbstract = complex.IsAbstract;

            List<WrittenElementInfo> exportedElements = new List<WrittenElementInfo>();

            WriteIndentedComment($"{complex.ShortDescription}");

            WriteSerializable();

            string fhirTypeConstructor = $"\"{complex.Name}\",\"{complex.URL}\"";

            if (isResource)
            {
                _writer.WriteLineIndented($"[FhirType({fhirTypeConstructor}, IsResource=true)]");
            }
            else
            {
                _writer.WriteLineIndented($"[FhirType({fhirTypeConstructor})]");
            }

            if (complex.BaseTypeName == "Quantity")
            {
                // Constrained quantities are handled differently
                WriteConstrainedQuantity(complex, exportName);
                return;
            }

            string abstractFlag = isAbstract ? " abstract" : string.Empty;

            List<string> interfaces = new List<string>();

            string modifierElementName = complex.Elements.Keys.SingleOrDefault(k => k.EndsWith(".modifierExtension", StringComparison.InvariantCulture));
            if (modifierElementName != null)
            {
                FhirElement modifierElement = complex.Elements[modifierElementName];
                if (!modifierElement.IsInherited)
                {
                    interfaces.Add($"{Namespace}.IModifierExtendable");
                }
            }

            string interfacesSuffix = interfaces.Any() ? $", {string.Join(", ", interfaces)}" : string.Empty;

            _writer.WriteLineIndented(
                $"public{abstractFlag} partial class" +
                    $" {exportName}" +
                    $" : {Namespace}.{DetermineExportedBaseTypeName(complex.BaseTypeName)}{interfacesSuffix}");

            // open class
            OpenScope();

            WritePropertyTypeName(complex.Name);

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
                        depth + 1,
                        subset);
                }
            }

            WriteElements(complex, exportName, ref exportedElements, subset);

            WriteCopyTo(exportName, exportedElements);

            if (!isAbstract)
            {
                WriteDeepCopy(exportName);
            }

            WriteMatches(exportName, exportedElements);
            WriteIsExactly(exportName, exportedElements);
            WriteChildren(exportedElements);
            WriteNamedChildren(exportedElements);

            WriteIDictionarySupport(exportedElements);

            // close class
            CloseScope();
        }

        private string DetermineExportedBaseTypeName(string baseTypeName)
        {
            // These two classes are more like interfaces, we treat their subclasses
            // as subclasses of DomainResource instead.
            if (baseTypeName == "MetadataResource" || baseTypeName == "CanonicalResource")
            {
                return "DomainResource";
            }

            if (_info.MajorVersion < 5)
            {
                // Promote R4 datatypes (all derived from Element/BackboneElement) to the right new subclass
                if (baseTypeName == "BackboneElement" && _info.MajorVersion == 4)
                {
                    return "BackboneType";
                }

                if (baseTypeName == "Element")
                {
                    return "DataType";
                }
            }

            return baseTypeName;
        }

        private void WriteIDictionarySupport(IEnumerable<WrittenElementInfo> exportedElements)
        {
            WriteDictionaryTryGetValue(exportedElements);
            WriteDictionaryPairs(exportedElements);
        }


        private void WriteDictionaryPairs(IEnumerable<WrittenElementInfo> exportedElements)
        {
            if (!exportedElements.Any()) return;

            _writer.WriteLineIndented("protected override IEnumerable<KeyValuePair<string, object>> GetElementPairs()");
            OpenScope();

            _writer.WriteLineIndented("foreach (var kvp in base.GetElementPairs()) yield return kvp;");

            foreach (var info in exportedElements)
            {
                var nullcheck = !info.IsList ? " is not null" : "?.Any() == true";
                _writer.WriteLineIndented($"if ({info.ExportedName}{nullcheck}) yield return new KeyValuePair<string,object>(\"{info.FhirElementName}\",{info.ExportedName});");
            }

            CloseScope();
        }

        private void WriteDictionaryTryGetValue(IEnumerable<WrittenElementInfo> exportedElements)
        {
            // Don't override anything if there are no additional elements.
            if (!exportedElements.Any()) return;

            _writer.WriteLineIndented("public override bool TryGetValue(string key, out object value)");
            OpenScope();
            _writer.WriteLineIndented("value = key switch");
            OpenScope();

            foreach (WrittenElementInfo info in exportedElements)
            {
                _writer.WriteIndented($"\"{info.FhirElementName}\" => ");

                if (!info.IsList)
                    _writer.WriteLine($"{info.ExportedName},");
                else
                    _writer.WriteLine($"{info.ExportedName}?.Any() == true ? {info.ExportedName} : null,");
            }

            _writer.WriteLineIndented("_ => default");
            CloseScope(includeSemicolon: true);

            _writer.WriteLineIndented("return value is not null || base.TryGetValue(key, out value);");
            CloseScope();
        }

        /// <summary>Writes the children of this item.</summary>
        /// <param name="exportedElements">The exported elements.</param>
        private void WriteNamedChildren(
            List<WrittenElementInfo> exportedElements)
        {
            _writer.WriteLineIndented("[IgnoreDataMember]");
            _writer.WriteLineIndented("public override IEnumerable<ElementValue> NamedChildren");
            OpenScope();
            _writer.WriteLineIndented("get");
            OpenScope();
            _writer.WriteLineIndented($"foreach (var item in base.NamedChildren) yield return item;");

            foreach (WrittenElementInfo info in exportedElements)
            {
                if (info.IsList)
                {
                    _writer.WriteLineIndented(
                        $"foreach (var elem in {info.ExportedName})" +
                            $" {{ if (elem != null)" +
                            $" yield return new ElementValue(\"{info.FhirElementName}\", elem);" +
                            $" }}");
                }
                else
                {
                    _writer.WriteLineIndented(
                        $"if ({info.ExportedName} != null)" +
                            $" yield return new ElementValue(\"{info.FhirElementName}\", {info.ExportedName});");
                }
            }

            CloseScope(suppressNewline: true);
            CloseScope();
        }

        /// <summary>Writes the children of this item.</summary>
        /// <param name="exportedElements">The exported elements.</param>
        private void WriteChildren(
            List<WrittenElementInfo> exportedElements)
        {
            _writer.WriteLineIndented("[IgnoreDataMember]");
            _writer.WriteLineIndented("public override IEnumerable<Base> Children");
            OpenScope();
            _writer.WriteLineIndented("get");
            OpenScope();
            _writer.WriteLineIndented($"foreach (var item in base.Children) yield return item;");

            foreach (WrittenElementInfo info in exportedElements)
            {
                if (info.IsList)
                {
                    _writer.WriteLineIndented(
                        $"foreach (var elem in {info.ExportedName})" +
                            $" {{ if (elem != null) yield return elem; }}");
                }
                else
                {
                    _writer.WriteLineIndented(
                        $"if ({info.ExportedName} != null)" +
                            $" yield return {info.ExportedName};");
                }
            }

            CloseScope(suppressNewline: true);
            CloseScope();
        }

        /// <summary>Writes the matches.</summary>
        /// <param name="exportName">      Name of the export.</param>
        /// <param name="exportedElements">The exported elements.</param>
        private void WriteMatches(
            string exportName,
            List<WrittenElementInfo> exportedElements)
        {
            _writer.WriteLineIndented("public override bool Matches(IDeepComparable other)");
            OpenScope();
            _writer.WriteLineIndented($"var otherT = other as {exportName};");
            _writer.WriteLineIndented("if(otherT == null) return false;");
            _writer.WriteLine(string.Empty);
            _writer.WriteLineIndented("if(!base.Matches(otherT)) return false;");

            foreach (WrittenElementInfo info in exportedElements)
            {
                _writer.WriteLineIndented(
                    $"if( !DeepComparable.Matches({info.ExportedName}, otherT.{info.ExportedName}))" +
                        $" return false;");
            }

            _writer.WriteLine(string.Empty);
            _writer.WriteLineIndented("return true;");

            CloseScope();
        }

        /// <summary>Writes the is exactly.</summary>
        /// <param name="exportName">      Name of the export.</param>
        /// <param name="exportedElements">The exported elements.</param>
        private void WriteIsExactly(
            string exportName,
            List<WrittenElementInfo> exportedElements)
        {
            _writer.WriteLineIndented("public override bool IsExactly(IDeepComparable other)");
            OpenScope();
            _writer.WriteLineIndented($"var otherT = other as {exportName};");
            _writer.WriteLineIndented("if(otherT == null) return false;");
            _writer.WriteLine(string.Empty);
            _writer.WriteLineIndented("if(!base.IsExactly(otherT)) return false;");

            foreach (WrittenElementInfo info in exportedElements)
            {
                _writer.WriteLineIndented(
                    $"if( !DeepComparable.IsExactly({info.ExportedName}, otherT.{info.ExportedName}))" +
                        $" return false;");
            }

            _writer.WriteLine(string.Empty);
            _writer.WriteLineIndented("return true;");

            CloseScope();
        }

        /// <summary>Writes a copy to.</summary>
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or
        ///  illegal values.</exception>
        /// <param name="exportName">      Name of the export.</param>
        /// <param name="exportedElements">The exported elements.</param>
        private void WriteCopyTo(
            string exportName,
            List<WrittenElementInfo> exportedElements)
        {
            _writer.WriteLineIndented("public override IDeepCopyable CopyTo(IDeepCopyable other)");
            OpenScope();
            _writer.WriteLineIndented($"var dest = other as {exportName};");
            _writer.WriteLine(string.Empty);

            _writer.WriteLineIndented("if (dest == null)");
            OpenScope();
            _writer.WriteLineIndented("throw new ArgumentException(\"Can only copy to an object of the same type\", \"other\");");
            CloseScope();

            _writer.WriteLineIndented("base.CopyTo(dest);");

            foreach (WrittenElementInfo info in exportedElements)
            {
                if (info.IsList)
                {
                    _writer.WriteLineIndented(
                        $"if({info.ExportedName} != null)" +
                            $" dest.{info.ExportedName} = new {info.ExportedType}({info.ExportedName}.DeepCopy());");
                }
                else
                {
                    _writer.WriteLineIndented(
                        $"if({info.ExportedName} != null)" +
                            $" dest.{info.ExportedName} = ({info.ExportedType}){info.ExportedName}.DeepCopy();");
                }
            }

            _writer.WriteLineIndented("return dest;");

            CloseScope();
        }

        /// <summary>Writes a deep copy.</summary>
        /// <param name="exportName">Name of the export.</param>
        private void WriteDeepCopy(
            string exportName)
        {
            _writer.WriteLineIndented("public override IDeepCopyable DeepCopy()");
            OpenScope();
            _writer.WriteLineIndented($"return CopyTo(new {exportName}());");
            CloseScope();
        }

        /// <summary>Sanitize for quoting.</summary>
        /// <param name="value">The value.</param>
        /// <returns>A string.</returns>
        private static string SanitizeForQuote(string value)
        {
#pragma warning disable CA1307 // Specify StringComparison
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
#pragma warning restore CA1307 // Specify StringComparison
        }

        /// <summary>Writes a constrained quantity.</summary>
        /// <param name="complex">   The complex data type.</param>
        /// <param name="exportName">Name of the export.</param>
        private void WriteConstrainedQuantity(
            FhirComplex complex,
            string exportName)
        {
            _writer.WriteLineIndented(
                $"public partial class" +
                    $" {exportName}" +
                    $" : Quantity");

            // open class
            OpenScope();

            WritePropertyTypeName(complex.Name);

            _writer.WriteLineIndented("public override IDeepCopyable DeepCopy()");
            OpenScope();
            _writer.WriteLineIndented($"return CopyTo(new {exportName}());");
            CloseScope();

            _writer.WriteLineIndented("// TODO: Add code to enforce these constraints:");
            WriteIndentedComment(complex.Purpose, isSummary: false, singleLine: true);

            // close class
            CloseScope();
        }

        private string capitalizeThoseSillyBackboneNames(string path) =>
            path.Length == 1 ? path :
                   path.StartsWith(".") ?
                    char.ToUpper(path[1]) + capitalizeThoseSillyBackboneNames(path.Substring(2))
                    : path[0] + capitalizeThoseSillyBackboneNames(path.Substring(1));

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
            int depth,
            GenSubset subset)
        {
            List<WrittenElementInfo> exportedElements = new List<WrittenElementInfo>();

            WriteIndentedComment($"{complex.ShortDescription}");

            string explicitName = complex.ExplicitName;

            // TODO: the following renames (repairs) should be removed when release 4B is official and there is an
            //   explicitname in the definition for attributes:
            //   - Statistic.attributeEstimate.attributeEstimate
            //   - Citation.contributorship.summary

            if (complex.Id.StartsWith("Citation") || complex.Id.StartsWith("Statistic") || complex.Id.StartsWith("DeviceDefinition"))
            {
                string parentName = complex.Id.Substring(0, complex.Id.IndexOf('.'));
                var sillyBackboneName = complex.Id.Substring(parentName.Length);
                explicitName = capitalizeThoseSillyBackboneNames(sillyBackboneName);
                exportName = explicitName + "Component";
            }
            // end of repair

            string componentName = parentExportName + "#" + (string.IsNullOrEmpty(explicitName) ?
                complex.NameForExport(FhirTypeBase.NamingConvention.PascalCase) :
                explicitName);

            Debug.Assert(!string.IsNullOrEmpty(componentName), $"Found a type at element {complex.Path} without a name or explicit name.");

            WriteSerializable();
            _writer.WriteLineIndented($"[FhirType(\"{componentName}\", IsNestedType=true)]");
            _writer.WriteLineIndented(
                $"public partial class" +
                    $" {exportName}" +
                    $" : {Namespace}.{complex.BaseTypeName}");

            // open class
            OpenScope();

            WritePropertyTypeName(componentName);

            WriteElements(complex, exportName, ref exportedElements, subset);

            if (exportedElements.Count > 0)
            {
                WriteCopyTo(exportName, exportedElements);
            }

            WriteDeepCopy(exportName);

            if (exportedElements.Count > 0)
            {
                WriteMatches(exportName, exportedElements);
                WriteIsExactly(exportName, exportedElements);
                WriteChildren(exportedElements);
                WriteNamedChildren(exportedElements);
                WriteIDictionarySupport(exportedElements);
            }

            // close class
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
                        depth + 1,
                        subset);
                }
            }
        }

        /// <summary>Writes the enums.</summary>
        /// <param name="complex">      The complex data type.</param>
        /// <param name="className">    Name of the class this enum is being written in.</param>
        /// <param name="usedEnumNames">(Optional) List of names of the used enums.</param>
        private void WriteEnums(
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
                        WriteEnum(vs, className, usedEnumNames);

                        continue;
                    }
                }
            }

            if (complex.Components != null)
            {
                foreach (FhirComplex component in complex.Components.Values)
                {
                    WriteEnums(component, className, usedEnumNames);
                }
            }
        }

        /// <summary>Writes a value set as an enum.</summary>
        /// <param name="vs">       The vs.</param>
        /// <param name="className">Name of the class this enum is being written in.</param>
        /// <param name="silent">Do not actually write parameter to file, just add it in memory.</param>
        private void WriteEnum(
            FhirValueSet vs,
            string className,
            HashSet<string> usedEnumNames,
            bool silent = false)
        {
            if (_writtenValueSets.ContainsKey(vs.URL))
            {
                return;
            }

            if (_exclusionSet.Contains(vs.URL))
            {
                return;
            }

#pragma warning disable CA1307 // Specify StringComparison
            string name = (vs.Name ?? vs.Id).Replace(" ", string.Empty).Replace("_", string.Empty);
#pragma warning restore CA1307 // Specify StringComparison
            string nameSanitized = FhirUtils.SanitizeForProperty(name, _reservedWords);

            if (usedEnumNames.Contains(nameSanitized))
            {
                return;
            }

            usedEnumNames.Add(nameSanitized);

            if (!silent)
            {
                if (vs.ReferencedCodeSystems.Count == 1)
                {
                    WriteIndentedComment(
                        $"{vs.Description}\n" +
                        $"(url: {vs.URL})\n" +
                        $"(system: {vs.ReferencedCodeSystems.First()})");
                }
                else
                {
                    WriteIndentedComment(
                        $"{vs.Description}\n" +
                        $"(url: {vs.URL})\n" +
                        $"(systems: {vs.ReferencedCodeSystems.Count})");
                }

                _writer.WriteLineIndented($"[FhirEnumeration(\"{name}\")]");

                _writer.WriteLineIndented($"public enum {nameSanitized}");

                OpenScope();

                HashSet<string> usedLiterals = new HashSet<string>();

                foreach (FhirConcept concept in vs.Concepts)
                {
                    string codeName = ConvertEnumValue(concept.Code);
                    string codeValue = FhirUtils.SanitizeForValue(concept.Code);

                    if (string.IsNullOrEmpty(concept.Definition))
                    {
                        WriteIndentedComment($"MISSING DESCRIPTION\n(system: {concept.System})");
                    }
                    else
                    {
                        WriteIndentedComment($"{concept.Definition}\n(system: {concept.System})");
                    }

                    string display = FhirUtils.SanitizeForValue(concept.Display);

                    _writer.WriteLineIndented($"[EnumLiteral(\"{codeValue}\", \"{concept.System}\"), Description(\"{display}\")]");

                    if (usedLiterals.Contains(codeName))
                    {
                        // start at 2 so that the unadorned version makes sense as v1
                        for (int i = 2; i < 1000; i++)
                        {
                            if (usedLiterals.Contains($"{codeName}_{i}"))
                            {
                                continue;
                            }

                            codeName = $"{codeName}_{i}";
                            break;
                        }
                    }

                    usedLiterals.Add(codeName);

                    _writer.WriteLineIndented($"{codeName},");
                }

                CloseScope();
            }

            _writtenValueSets.Add(
                vs.URL,
                new WrittenValueSetInfo()
                {
                    ClassName = className,
                    ValueSetName = nameSanitized,
                });
        }

        /// <summary>Convert enum value - see Template-Model.tt#2061.</summary>
        /// <param name="name">The name.</param>
        /// <returns>The enum converted value.</returns>
        private static string ConvertEnumValue(string name) => CSharpFirelyCommon.ConvertEnumValue(name);

        /// <summary>Gets an order.</summary>
        /// <param name="element">The element.</param>
        private static int GetOrder(FhirElement element) => CSharpFirelyCommon.GetOrder(element);

        /// <summary>Writes the elements.</summary>
        /// <param name="complex">              The complex data type.</param>
        /// <param name="exportedComplexName">  Name of the exported complex parent.</param>
        /// <param name="exportedElements">     [in,out] The exported elements.</param>
        private void WriteElements(
            FhirComplex complex,
            string exportedComplexName,
            ref List<WrittenElementInfo> exportedElements,
            GenSubset subset)
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

                // if (!string.IsNullOrEmpty(element.ValueSet))
                if (typeName == "code")
                {
                    WriteCodedElement(
                        element,
                        ref exportedElements,
                        subset);
                    continue;
                }

                WriteElement(
                    exportedComplexName,
                    element,
                    ref exportedElements,
                    subset);
            }
        }

        /// <summary>Writes an element.</summary>
        /// <param name="element">            The element.</param>
        /// <param name="exportedElements">   [in,out] The exported elements.</param>
        private void WriteCodedElement(
            FhirElement element,
            ref List<WrittenElementInfo> exportedElements,
            GenSubset subset)
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

            WriteIndentedComment(element.ShortDescription);

            BuildElementOptionals(
                element,
                subset,
                out string summary,
                out string choice,
                out string allowedTypes,
                out string resourceReferences);

            _writer.WriteLineIndented($"[FhirElement(\"{element.Name}\"{summary}, Order={GetOrder(element)}{choice})]");

            if (hasDefinedEnum)
            {
                _writer.WriteLineIndented("[DeclaredType(Type = typeof(Code))]");
            }

            if (!string.IsNullOrEmpty(resourceReferences))
            {
                _writer.WriteLineIndented("[CLSCompliant(false)]");
                _writer.WriteLineIndented(resourceReferences);
            }

            // Generate the [AllowedTypes] attribute, except when we are generating an element for the
            // open datatypes in Common, since this list contains classes that we have not yet moved to common.
            bool isOpenTypeInCommon = subset.HasFlag(GenSubset.Common) && element.ElementTypes.Count > 25;
            if (!string.IsNullOrEmpty(allowedTypes) && !isOpenTypeInCommon)
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

            string namespacedCodeLiteral;
            string codeLiteral;
            string enumClass;
            string optional;

            string matchTrailer = string.Empty;

            if (hasDefinedEnum)
            {
                string vsClass = _writtenValueSets[vs.URL].ClassName;
                string vsName = _writtenValueSets[vs.URL].ValueSetName;

                if (string.IsNullOrEmpty(vsClass))
                {
                    codeLiteral = $"Code<{Namespace}.{vsName}>";
                    namespacedCodeLiteral = $"{Namespace}.Code<{Namespace}.{vsName}>";
                    enumClass = $"{Namespace}.{vsName}";
                }
                else
                {
                    codeLiteral = $"Code<{Namespace}.{vsClass}.{vsName}>";
                    namespacedCodeLiteral = $"{Namespace}.Code<{Namespace}.{vsClass}.{vsName}>";
                    enumClass = $"{Namespace}.{vsClass}.{vsName}";

                    if (vsName.ToUpperInvariant() == pascal.ToUpperInvariant())
                    {
                        matchTrailer = "_";
                    }
                }

                optional = "?";
            }
            else
            {
                codeLiteral = $"{Namespace}.Code";
                namespacedCodeLiteral = $"{Namespace}.Code";
                enumClass = "string";
                optional = string.Empty;
            }

            if (element.CardinalityMax == 1)
            {
                exportedElements.Add(
                    new WrittenElementInfo()
                    {
                        FhirElementName = element.Name.Replace("[x]", string.Empty),
                        ExportedName = $"{pascal}{matchTrailer}Element",
                        ExportedType = codeLiteral,
                        IsList = false,
                    });

                _writer.WriteLineIndented($"public {codeLiteral} {pascal}{matchTrailer}Element");

                OpenScope();
                _writer.WriteLineIndented($"get {{ return _{pascal}{matchTrailer}Element; }}");
                _writer.WriteLineIndented($"set {{ _{pascal}{matchTrailer}Element = value; OnPropertyChanged(\"{pascal}{matchTrailer}Element\"); }}");
                CloseScope();

                _writer.WriteLineIndented($"private {codeLiteral} _{pascal}{matchTrailer}Element;");
                _writer.WriteLine(string.Empty);
            }
            else
            {
                exportedElements.Add(
                    new WrittenElementInfo()
                    {
                        FhirElementName = element.Name.Replace("[x]", string.Empty),
                        ExportedName = $"{pascal}{matchTrailer}Element",
                        ExportedType = $"List<{codeLiteral}>",
                        IsList = true,
                    });

                _writer.WriteLineIndented($"public List<{codeLiteral}> {pascal}{matchTrailer}Element");

                OpenScope();
                _writer.WriteLineIndented($"get {{ if(_{pascal}{matchTrailer}Element==null) _{pascal}{matchTrailer}Element = new List<{namespacedCodeLiteral}>(); return _{pascal}{matchTrailer}Element; }}");
                _writer.WriteLineIndented($"set {{ _{pascal}{matchTrailer}Element = value; OnPropertyChanged(\"{pascal}{matchTrailer}Element\"); }}");
                CloseScope();

                _writer.WriteLineIndented($"private List<{codeLiteral}> _{pascal}{matchTrailer}Element;");
                _writer.WriteLine(string.Empty);
            }

            WriteIndentedComment(element.ShortDescription);
            _writer.WriteLineIndented($"/// <remarks>This uses the native .NET datatype, rather than the FHIR equivalent</remarks>");

            _writer.WriteLineIndented("[IgnoreDataMember]");

            if (element.CardinalityMax == 1)
            {
                _writer.WriteLineIndented($"public {enumClass}{optional} {pascal}{matchTrailer}");

                OpenScope();
                _writer.WriteLineIndented($"get {{ return {pascal}{matchTrailer}Element != null ? {pascal}{matchTrailer}Element.Value : null; }}");
                _writer.WriteLineIndented("set");
                OpenScope();

                // T4 template has some HasValue checks - compiler will replace and this is simpler
                _writer.WriteLineIndented($"if (value == null)");

                _writer.IncreaseIndent();
                _writer.WriteLineIndented($"{pascal}{matchTrailer}Element = null;");
                _writer.DecreaseIndent();
                _writer.WriteLineIndented("else");
                _writer.IncreaseIndent();
                _writer.WriteLineIndented($"{pascal}{matchTrailer}Element = new {codeLiteral}(value);");
                _writer.DecreaseIndent();
                _writer.WriteLineIndented($"OnPropertyChanged(\"{pascal}{matchTrailer}\");");
                CloseScope(suppressNewline: true);
                CloseScope();
            }
            else
            {
                _writer.WriteLineIndented($"public IEnumerable<{enumClass}{optional}> {pascal}{matchTrailer}");

                OpenScope();
                _writer.WriteLineIndented($"get {{ return {pascal}{matchTrailer}Element != null ? {pascal}{matchTrailer}Element.Select(elem => elem.Value) : null; }}");
                _writer.WriteLineIndented("set");
                OpenScope();

                _writer.WriteLineIndented($"if (value == null)");
                _writer.IncreaseIndent();
                _writer.WriteLineIndented($"{pascal}{matchTrailer}Element = null;");
                _writer.DecreaseIndent();
                _writer.WriteLineIndented("else");
                _writer.IncreaseIndent();
                _writer.WriteLineIndented($"{pascal}{matchTrailer}Element = new List<{namespacedCodeLiteral}>(value.Select(elem=>new {namespacedCodeLiteral}(elem)));");
                _writer.DecreaseIndent();

                _writer.WriteLineIndented($"OnPropertyChanged(\"{pascal}{matchTrailer}\");");
                CloseScope(suppressNewline: true);
                CloseScope();
            }
        }

        /// <summary>Writes an element.</summary>
        /// <param name="exportedComplexName">Name of the exported complex parent.</param>
        /// <param name="element">            The element.</param>
        /// <param name="exportedElements">   [in,out] The exported elements.</param>
        private void WriteElement(
            string exportedComplexName,
            FhirElement element,
            ref List<WrittenElementInfo> exportedElements,
            GenSubset subset)
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
                subset,
                out string summary,
                out string choice,
                out string allowedTypes,
                out string resourceReferences);

            /* Exceptions:
             *  o Meta.source only exists since R5, it is still present in the common version.
             *  o Meta.profile has changed types from `uri` to `canonical`, but we stick to Uri for the common version
             *
             * If we start to include more classes like this, we might need to
             * automate this, by scanning differences between 3/4/5/6/7 etc.. */
            if (element.Path == "Meta.source")
            {
                _writer.WriteLineIndented($"[FhirElement(\"{name}\"{summary}, Order={GetOrder(element)}{choice}, Since=FhirRelease.R4)]");
            }
            else if (element.Path == "Reference.type")
            {
                _writer.WriteLineIndented($"[FhirElement(\"{name}\"{summary}, Order={GetOrder(element)}{choice}, Since=FhirRelease.R4)]");
            }
            else
            {
                _writer.WriteLineIndented($"[FhirElement(\"{name}\"{summary}, Order={GetOrder(element)}{choice})]");
            }

            if (element.Path == "Meta.profile")
            {
                _writer.WriteLineIndented($"[DeclaredType(Type = typeof(Canonical), Since = FhirRelease.R4)]");
            }

            // Generate the [AllowedTypes] and [ResourceReference] attributes, except when we are
            // generating datatypes and resources in Common, since this list probably contains
            // classes that we have not yet moved to common.
            bool notClsCompliant = !string.IsNullOrEmpty(allowedTypes) ||
                !string.IsNullOrEmpty(resourceReferences);

            if (notClsCompliant)
            {
                _writer.WriteLineIndented("[CLSCompliant(false)]");
            }

            if (!string.IsNullOrEmpty(resourceReferences))
            {
                _writer.WriteLineIndented(resourceReferences);
            }

            if (!string.IsNullOrEmpty(allowedTypes))
            {
                _writer.WriteLineIndented(allowedTypes);
            }

            if ((element.CardinalityMin != 0) ||
                (element.CardinalityMax != 1))
            {
                _writer.WriteLineIndented($"[Cardinality(Min={element.CardinalityMin},Max={element.CardinalityMax})]");
            }

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

            _writer.WriteLineIndented("[DataMember]");

            bool noElement = true;

            if (type.Contains('.'))
            {
                type = BuildTypeFromPath(type);
            }

            string nativeType = type;
            string optional = string.Empty;

            if (CSharpFirelyCommon.PrimitiveTypeMap.ContainsKey(nativeType))
            {
                nativeType = CSharpFirelyCommon.PrimitiveTypeMap[nativeType];

                if (IsNullable(nativeType))
                {
                    optional = "?";
                }

                noElement = false;
            }
            else
            {
                nativeType = $"{Namespace}.{type}";
            }

            if ((_info.MajorVersion < 4) &&
                _info.ComplexTypes.ContainsKey(exportedComplexName) &&
                (type == "markdown"))
            {
                nativeType = "string";
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

            if (element.CardinalityMax == 1)
            {
                exportedElements.Add(
                    new WrittenElementInfo()
                    {
                        FhirElementName = element.Name.Replace("[x]", string.Empty),
                        ExportedName = $"{pascal}{elementTag}",
                        ExportedType = $"{Namespace}.{type}",
                        IsList = false,
                    });

                _writer.WriteLineIndented($"public {Namespace}.{type} {pascal}{elementTag}");

                OpenScope();
                _writer.WriteLineIndented($"get {{ return _{pascal}{elementTag}; }}");
                _writer.WriteLineIndented($"set {{ _{pascal}{elementTag} = value; OnPropertyChanged(\"{pascal}{elementTag}\"); }}");
                CloseScope();

                _writer.WriteLineIndented($"private {Namespace}.{type} _{pascal}{elementTag};");
                _writer.WriteLine(string.Empty);
            }
            else
            {
                exportedElements.Add(
                    new WrittenElementInfo()
                    {
                        FhirElementName = element.Name.Replace("[x]", string.Empty),
                        ExportedName = $"{pascal}{elementTag}",
                        ExportedType = $"List<{Namespace}.{type}>",
                        IsList = true,
                    });

                _writer.WriteLineIndented($"public List<{Namespace}.{type}> {pascal}{elementTag}");

                OpenScope();
                _writer.WriteLineIndented($"get {{ if(_{pascal}{elementTag}==null) _{pascal}{elementTag} = new List<{Namespace}.{type}>(); return _{pascal}{elementTag}; }}");
                _writer.WriteLineIndented($"set {{ _{pascal}{elementTag} = value; OnPropertyChanged(\"{pascal}{elementTag}\"); }}");
                CloseScope();

                _writer.WriteLineIndented($"private List<{Namespace}.{type}> _{pascal}{elementTag};");
                _writer.WriteLine(string.Empty);
            }

            if (noElement)
            {
                // only write the one field
                return;
            }

            string matchTrailer = string.Empty;

            if (pascal == exportedComplexName)
            {
                matchTrailer = "_";
            }

            WriteIndentedComment(element.ShortDescription);
            _writer.WriteLineIndented($"/// <remarks>This uses the native .NET datatype, rather than the FHIR equivalent</remarks>");

            _writer.WriteLineIndented("[IgnoreDataMember]");

            if (element.CardinalityMax == 1)
            {
                _writer.WriteLineIndented($"public {nativeType}{optional} {pascal}{matchTrailer}");

                OpenScope();
                _writer.WriteLineIndented($"get {{ return {pascal}Element != null ? {pascal}Element.Value : null; }}");
                _writer.WriteLineIndented("set");
                OpenScope();

                _writer.WriteLineIndented($"if (value == null)");

                _writer.IncreaseIndent();
                _writer.WriteLineIndented($"{pascal}Element = null;");
                _writer.DecreaseIndent();
                _writer.WriteLineIndented("else");
                _writer.IncreaseIndent();
                _writer.WriteLineIndented($"{pascal}Element = new {Namespace}.{type}(value);");
                _writer.DecreaseIndent();
                _writer.WriteLineIndented($"OnPropertyChanged(\"{pascal}{matchTrailer}\");");
                CloseScope(suppressNewline: true);
                CloseScope();
            }
            else
            {
                _writer.WriteLineIndented($"public IEnumerable<{nativeType}{optional}> {pascal}{matchTrailer}");

                OpenScope();
                _writer.WriteLineIndented($"get {{ return {pascal}Element != null ? {pascal}Element.Select(elem => elem.Value) : null; }}");
                _writer.WriteLineIndented("set");
                OpenScope();

                _writer.WriteLineIndented($"if (value == null)");

                _writer.IncreaseIndent();
                _writer.WriteLineIndented($"{pascal}Element = null;");
                _writer.DecreaseIndent();
                _writer.WriteLineIndented("else");
                _writer.IncreaseIndent();
                _writer.WriteLineIndented($"{pascal}Element = new List<{Namespace}.{type}>(value.Select(elem=>new {Namespace}.{type}(elem)));");
                _writer.DecreaseIndent();

                _writer.WriteLineIndented($"OnPropertyChanged(\"{pascal}{matchTrailer}\");");
                CloseScope(suppressNewline: true);
                CloseScope();
            }
        }

        /// <summary>Builds type from path.</summary>
        /// <param name="type">The type.</param>
        /// <returns>A string.</returns>
        private string BuildTypeFromPath(string type)
        {
            // TODO: the following renames (repairs) should be removed when release 4B is official and there is an
            //   explicitname in the definition for attributes:
            //   - Statistic.attributeEstimate.attributeEstimate
            //   - Citation.contributorship.summary

            if (type.StartsWith("Citation") || type.StartsWith("Statistic") || type.StartsWith("DeviceDefinition"))
            {
                string parentName = type.Substring(0, type.IndexOf('.'));
                var sillyBackboneName = type.Substring(parentName.Length);
                type = parentName + "." + capitalizeThoseSillyBackboneNames(sillyBackboneName) + "Component";
            }
            // end of repair
            else if (_info.TryGetExplicitName(type, out string explicitTypeName))
            {
                string parentName = type.Substring(0, type.IndexOf('.'));
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
        private void BuildElementOptionals(
            FhirElement element,
            GenSubset subset,
            out string summary,
            out string choice,
            out string allowedTypes,
            out string resourceReferences)
        {
            choice = string.Empty;
            allowedTypes = string.Empty;
            resourceReferences = string.Empty;
            summary = element.IsSummary ? ", InSummary=true" : string.Empty;

            bool inCommon = subset.HasFlag(GenSubset.Common);

            if (element.ElementTypes != null)
            {
                if (element.ElementTypes.Count == 1)
                {
                    string elementType = element.ElementTypes.First().Value.Name;

                    if (elementType == "Resource")
                    {
                        choice = ", Choice=ChoiceType.ResourceChoice";
                        allowedTypes = $"[AllowedTypes(typeof({Namespace}.Resource))]";
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

                    // When we generating classes in Common, we have to avoid generating an
                    // [AllowedTypes] attribute that contains class names that are not
                    // present in the current version of the standard. So, in principle, we don't generate
                    // this attribute in Common, unless all types mentioned are present in the
                    // exception list above.
                    if (!inCommon || element.ElementTypes.Values
                            .Select(v => v.Name)
                            .All(en => _allCommonTypes.Contains(en)))
                    {
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
                            sb.Append(Namespace);
                            sb.Append(".");

                            if (CSharpFirelyCommon.TypeNameMappings.ContainsKey(elementType.Name))
                            {
                                sb.Append(CSharpFirelyCommon.TypeNameMappings[elementType.Name]);
                            }
                            else
                            {
                                sb.Append(FhirUtils.SanitizedToConvention(elementType.Name, FhirTypeBase.NamingConvention.PascalCase));
                            }

                            sb.Append(")");
                        }

                        sb.Append(")]");
                        allowedTypes = sb.ToString();
                    }
                }
            }

            if (element.ElementTypes != null)
            {
                foreach (FhirElementType elementType in element.ElementTypes.Values)
                {
                    if (elementType.Name == "Reference" && elementType.Profiles.Any())
                    {
                        // When we generating classes in Common, we have to avoid generating an
                        // [ResourceReference] attribute that contains class names that are not
                        // present in the current version of the standard. So, in principle, we don't generate
                        // this attribute in Common, unless all types mentioned are present in the
                        // exception list above.
                        if (!inCommon || elementType.Profiles.Values
                            .Select(v => v.Name)
                            .All(en => _allCommonTypes.Contains(en)))
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
        }

        /// <summary>Writes a property type name.</summary>
        /// <param name="name">      The name.</param>
        private void WritePropertyTypeName(string name)
        {
            WriteIndentedComment("FHIR Type Name");

            _writer.WriteLineIndented($"public override string TypeName {{ get {{ return \"{name}\"; }} }}");

            _writer.WriteLine(string.Empty);
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
        /// <param name="primitives">   The primitives.</param>
        /// <param name="writtenModels">[in,out] The written models.</param>
        private void WritePrimitiveTypes(
            IEnumerable<FhirPrimitive> primitives,
            ref Dictionary<string, WrittenModelInfo> writtenModels)
        {
            foreach (FhirPrimitive primitive in primitives)
            {
                if (_exclusionSet.Contains(primitive.Name))
                {
                    continue;
                }

                WritePrimitiveType(primitive, ref writtenModels);
            }
        }

        /// <summary>Writes a primitive type.</summary>
        /// <param name="primitive">    The primitive.</param>
        /// <param name="writtenModels">[in,out] The written models.</param>
        private void WritePrimitiveType(
            FhirPrimitive primitive,
            ref Dictionary<string, WrittenModelInfo> writtenModels)
        {
            string exportName;
            string typeName;

            if (CSharpFirelyCommon.TypeNameMappings.ContainsKey(primitive.Name))
            {
                exportName = CSharpFirelyCommon.TypeNameMappings[primitive.Name];
            }
            else
            {
                exportName = primitive.NameForExport(FhirTypeBase.NamingConvention.PascalCase);
            }

            if (CSharpFirelyCommon.PrimitiveTypeMap.ContainsKey(primitive.Name))
            {
                typeName = CSharpFirelyCommon.PrimitiveTypeMap[primitive.Name];
            }
            else
            {
                typeName = primitive.BaseTypeName;
            }

            writtenModels.Add(
                primitive.Name,
                new WrittenModelInfo()
                {
                    FhirName = primitive.Name,
                    CsName = $"{Namespace}.{exportName}",
                    IsAbstract = false,   // no abstract primitives
                });

            string filename = Path.Combine(_exportDirectory, "Generated", $"{exportName}.cs");

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

                _writer.WriteLineIndented("[System.Diagnostics.DebuggerDisplay(@\"\\{Value={Value}}\")]");
                WriteSerializable();

                string fhirTypeConstructor = $"\"{primitive.Name}\",\"{primitive.URL}\"";
                _writer.WriteLineIndented($"[FhirType({fhirTypeConstructor})]");

                _writer.WriteLineIndented(
                    $"public partial class" +
                        $" {exportName}" +
                        $" : PrimitiveType, " +
                        PrimitiveValueInterface(typeName));

                // open class
                OpenScope();

                WritePropertyTypeName(primitive.Name);

                if (!string.IsNullOrEmpty(primitive.ValidationRegEx))
                {
                    WriteIndentedComment(
                        $"Must conform to the pattern \"{primitive.ValidationRegEx}\"",
                        false);

                    _writer.WriteLineIndented($"public const string PATTERN = @\"{primitive.ValidationRegEx}\";");
                    _writer.WriteLine(string.Empty);
                }

                _writer.WriteLineIndented($"public {exportName}({typeName} value)");
                OpenScope();
                _writer.WriteLineIndented("Value = value;");
                CloseScope();

                _writer.WriteLineIndented($"public {exportName}(): this(({typeName})null) {{}}");
                _writer.WriteLine(string.Empty);

                WriteIndentedComment("Primitive value of the element");

                _writer.WriteLineIndented("[FhirElement(\"value\", IsPrimitiveValue=true, XmlSerialization=XmlRepresentation.XmlAttr, InSummary=true, Order=30)]");
                _writer.WriteLineIndented($"[DeclaredType(Type = typeof({getSystemTypeForFhirType(primitive.Name)}))]");

                if (CSharpFirelyCommon.PrimitiveValidationPatterns.ContainsKey(primitive.Name))
                {
                    _writer.WriteLineIndented($"[{CSharpFirelyCommon.PrimitiveValidationPatterns[primitive.Name]}]");
                }

                _writer.WriteLineIndented("[DataMember]");
                _writer.WriteLineIndented($"public {typeName} Value");
                OpenScope();
                _writer.WriteLineIndented($"get {{ return ({typeName})ObjectValue; }}");
                _writer.WriteLineIndented("set { ObjectValue = value; OnPropertyChanged(\"Value\"); }");
                CloseScope();

                /* Generate validator for simple string-based values that have a regex to validate them.
                 * Skip validator for some types for which we have more performant, hand-written validators.
                 */
                if (!string.IsNullOrEmpty(primitive.ValidationRegEx) &&
                    exportName != "FhirString" && exportName != "FhirUri" && exportName != "Markdown")
                {
                    _writer.WriteLineIndented("public static bool IsValidValue(string value) => Regex.IsMatch(value, \"^\" + PATTERN + \"$\", RegexOptions.Singleline);");
                    _writer.WriteLine(string.Empty);
                }

                // close class
                CloseScope();

                WriteNamespaceClose();

                WriteFooter();
            }
        }

        private string getSystemTypeForFhirType(string fhirType)
        {
            var systemTypeName = fhirType switch
            {
                "boolean" => "Boolean",
                "integer" => "Integer",
                "unsignedInt" => "Integer",
                "positiveInt" => "Integer",
                "integer64" => "Long",
                "time" => "Time",
                "date" => "Date",
                "instant" => "DateTime",
                "dateTime" => "DateTime",
                "decimal" => "Decimal",
                _ => "String"
            };

            return "SystemPrimitive." + systemTypeName;
        }

        private void WriteSerializable()
        {
            _writer.WriteLineIndented("[Serializable]");
            _writer.WriteLineIndented("[DataContract]");
        }

        private static string PrimitiveValueInterface(string valueType)
        {
            if (valueType.EndsWith("?", StringComparison.InvariantCulture))
            {
                string nullableType = valueType.TrimEnd('?');
                return $"INullableValue<{nullableType}>";
            }
            else
            {
                return $"IValue<{valueType}>";
            }
        }

        /// <summary>Writes the namespace open.</summary>
        private void WriteNamespaceOpen()
        {
            _writer.WriteLineIndented($"namespace {Namespace}");
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

        /// <summary>Writes a header above a primitive class.</summary>
        private void WriteHeaderPrimitive()
        {
            WriteGenerationComment();

            _writer.WriteLineIndented("using System;");
            _writer.WriteLineIndented("using System.Runtime.Serialization;");
            _writer.WriteLineIndented("using System.Text.RegularExpressions;");
            _writer.WriteLineIndented("using Hl7.Fhir.Introspection;");
            _writer.WriteLineIndented("using Hl7.Fhir.Specification;");
            _writer.WriteLineIndented("using Hl7.Fhir.Validation;");
            _writer.WriteLineIndented("using SystemPrimitive = Hl7.Fhir.ElementModel.Types;");
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

        private static void AddModels(
            Dictionary<string, WrittenModelInfo> total,
            IEnumerable<FhirTypeBase> typesToAdd)
        {
            AddModels(total, typesToAdd.Select(ta => CreateWMI(ta)));

            WrittenModelInfo CreateWMI(FhirTypeBase t)
            {
                string exportName = CSharpFirelyCommon.TypeNameMappings.ContainsKey(t.Name) ?
                    CSharpFirelyCommon.TypeNameMappings[t.Name] :
                    t.NameForExport(FhirTypeBase.NamingConvention.PascalCase);

                return new WrittenModelInfo()
                {
                    FhirName = t.Name,
                    CsName = $"{Namespace}.{exportName}",
                    IsAbstract = t is FhirComplex c && c.IsAbstract,
                };
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
            internal bool IsList;
        }

        /// <summary>Information about the written model.</summary>
        private struct WrittenModelInfo
        {
            internal string FhirName;
            internal string CsName;
            internal bool IsAbstract;
        }
    }
}


