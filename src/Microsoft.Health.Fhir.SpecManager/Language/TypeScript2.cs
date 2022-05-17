// <copyright file="TypeScript2.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.IO;
using System.Runtime.Serialization;
using Microsoft.Health.Fhir.SpecManager.Manager;
using Microsoft.Health.Fhir.SpecManager.Models;

namespace Microsoft.Health.Fhir.SpecManager.Language;

/// <summary>A TypeScript language export.</summary>
public sealed class TypeScript2 : ILanguage
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

    /// <summary>The namespace models.</summary>
    private string _namespaceInternal;

    /// <summary>The dir models.</summary>
    private string _dirModels;

    /// <summary>Pathname of the value set directory.</summary>
    private string _directoryValueSets;

    /// <summary>(Immutable) True to export interfaces as types.</summary>
    private static bool _exportInterfacesAsTypes = true;

    private readonly record struct ExportedPrimitive(
        string FhirName,
        string ExportedName,
        string FileKey);

    private record struct ExportedComplex(
        string FhirName,
        string ExportedName,
        string ExportedParentName,
        string FileKey,
        List<string> ExportedBackbones,
        HashSet<string> ExportedEnums);

    /// <summary>The exported primitives.</summary>
    private Dictionary<string, ExportedPrimitive> _exportedPrimitives = new();

    /// <summary>List of types of the exported complexes.</summary>
    private Dictionary<string, ExportedComplex> _exportedComplexTypes = new();

    /// <summary>The exported resources.</summary>
    private Dictionary<string, ExportedComplex> _exportedResources = new();

    /// <summary>The currently in-use text writer.</summary>
    private ExportStreamWriter _writer;

    /// <summary>Name of the language.</summary>
    private const string _languageName = "TypeScript2";

    /// <summary>The minimum type script version.</summary>
    private const string _minimumTypeScriptVersion = "3.7";

    /// <summary>The single file export extension - requires directory export.</summary>
    private const string _singleFileExportExtension = null;

    /// <summary>(Immutable) Source extension.</summary>
    private const string _sourceExtension = ".ts";

    /// <summary>(Immutable) The interface extension.</summary>
    private const string _interfaceExtension = ".ts";

    /// <summary>Dictionary mapping FHIR primitive types to language equivalents.</summary>
    private static readonly Dictionary<string, string> _primitiveTypeMap = new Dictionary<string, string>()
    {
        { "base", "Object" },
        { "base64Binary", "string" },
        { "bool", "boolean" },
        { "boolean", "boolean" },
        { "canonical", "string" },
        { "code", "string" },
        { "date", "string" },
        { "dateTime", "string" },           // Cannot use "DateTime" because of Partial Dates... may want to consider defining a new type, but not today
        { "decimal", "number" },
        { "id", "string" },
        { "instant", "string" },
        { "int", "number" },
        { "integer", "number" },
        { "integer64", "string" },
        { "markdown", "string" },
        { "number", "number" },
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
    private static readonly HashSet<string> _reservedWords = new(StringComparer.Ordinal)
    {
        "const",
        "enum",
        "export",
        "interface",
        "Element",
    };

    /// <summary>The generics and type hints.</summary>
    private static readonly Dictionary<string, GenericTypeHintInfo> _genericsAndTypeHints = new()
    {
        //{
        //    "Bundle",
        //    new GenericTypeHintInfo()
        //    {
        //        Alias = "BundleContentType",
        //        GenericHint = "FhirResource",
        //        IncludeBase = true,
        //    }
        //},
        //{
        //    "Bundle.entry",
        //    new GenericTypeHintInfo()
        //    {
        //        Alias = "BundleContentType",
        //        GenericHint = "FhirResource",
        //        IncludeBase = true,
        //    }
        //},
        //{
        //    "Bundle.entry.resource",
        //    new GenericTypeHintInfo()
        //    {
        //        Alias = "BundleContentType",
        //        GenericHint = string.Empty,
        //        IncludeBase = false,
        //    }
        //},
    };

    /// <summary>(Immutable) The Set of all known generic types (needed for casting).</summary>
    private static readonly HashSet<string> _genericTypeHints = new()
    {
        "FhirResource",
        "BundleContentType",
        "BundleEntry<BundleContentType>[]",
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
        ExporterOptions.FhirExportClassType.Profile,
    };

    /// <summary>Gets language-specific options and their descriptions.</summary>
    Dictionary<string, string> ILanguage.LanguageOptions => new Dictionary<string, string>()
    {
        { "namespace", "Base namespace for TypeScript classes (default: Fhir.R{VersionNumber})." },
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

        // _namespace = options.GetParam("namespace", $"fhirR{info.MajorVersion}");
        _namespaceInternal = "fhir";

        _exportedComplexTypes = new();
        _exportedResources = new();

        _dirModels = Path.Combine(exportDirectory, "fhir");
        if (!Directory.Exists(_dirModels))
        {
            Directory.CreateDirectory(_dirModels);
        }

        _directoryValueSets = Path.Combine(exportDirectory, "fhirValueSets");
        if (_exportEnums)
        {
            if (!Directory.Exists(_directoryValueSets))
            {
                Directory.CreateDirectory(_directoryValueSets);
            }
        }

        if (options.SupportFiles.StaticFiles.Any())
        {
            foreach (LanguageSupportFiles.SupportFileRec fileRec in options.SupportFiles.StaticFiles.Values)
            {
                File.Copy(fileRec.Filename, Path.Combine(exportDirectory, fileRec.RelativeFilename));
            }
        }

        WriteComplexes(_info.ComplexTypes.Values, FhirArtifactClassEnum.ComplexType);
        WriteComplexes(_info.Resources.Values, FhirArtifactClassEnum.Resource);

        WriteFhirExportModule(exportDirectory);

        List<string> exportedValueSets = new();

        if (_exportEnums)
        {
            WriteValueSets(_info.ValueSetsByUrl.Values, ref exportedValueSets);
            WriteValueSetModule(exportDirectory, exportedValueSets);
        }

        WriteIndexModule(exportDirectory);
    }

    /// <summary>Gets sorted files and tokens.</summary>
    /// <returns>The sorted files and tokens.</returns>
    private List<(string File, List<string> Tokens, List<string> Annotated)> GetSortedFilesAndTokens()
    {
        List<(string File, List<string> Tokens, List<string> Annotated)> sortedItems = new();
        HashSet<string> usedKeys = new();

        foreach (ExportedPrimitive item in _exportedPrimitives.Values)
        {
            if (usedKeys.Contains(item.ExportedName))
            {
                continue;
            }

            sortedItems.Add((
                item.FileKey,
                new List<string>() { item.ExportedName },
                new List<string>() { "type " + item.ExportedName }));
        }

        foreach (ExportedComplex item in _exportedComplexTypes.Values)
        {
            if (usedKeys.Contains(item.ExportedName))
            {
                continue;
            }

            AddComplexForExport(item, sortedItems, usedKeys);
        }

        foreach (ExportedComplex item in _exportedResources.Values)
        {
            if (usedKeys.Contains(item.ExportedName))
            {
                continue;
            }

            AddComplexForExport(item, sortedItems, usedKeys);
        }

        return sortedItems;
    }

    /// <summary>Adds a complex for export.</summary>
    /// <param name="item">       The item.</param>
    /// <param name="sortedItems">The sorted items.</param>
    /// <param name="usedKeys">   The used keys.</param>
    private void AddComplexForExport(
        ExportedComplex item,
        List<(string File, List<string> Tokens, List<string> Annotated)> sortedItems,
        HashSet<string> usedKeys)
    {
        if ((!string.IsNullOrEmpty(item.ExportedParentName)) &&
            (!usedKeys.Contains(item.ExportedParentName)))
        {
            if (_exportedComplexTypes.ContainsKey(item.ExportedParentName))
            {
                AddComplexForExport(
                    _exportedComplexTypes[item.ExportedParentName],
                    sortedItems,
                    usedKeys);
            }

            if (_exportedResources.ContainsKey(item.ExportedParentName))
            {
                AddComplexForExport(
                    _exportedResources[item.ExportedParentName],
                    sortedItems,
                    usedKeys);
            }
        }

        List<string> artifacts = new();
        List<string> annotated = new();

        artifacts.Add("I" + item.ExportedName);
        annotated.Add("type I" + item.ExportedName);

        artifacts.AddRange(item.ExportedBackbones.Select((k) => "I" + k));
        annotated.AddRange(item.ExportedBackbones.Select((k) => "type I" + k));

        artifacts.Add(item.ExportedName);
        annotated.Add(item.ExportedName);

        artifacts.AddRange(item.ExportedBackbones);
        annotated.AddRange(item.ExportedBackbones);

        artifacts.AddRange(item.ExportedEnums);
        //annotated.AddRange(item.ExportedEnums.Select((k) => "type " + k));
        annotated.AddRange(item.ExportedEnums);

        sortedItems.Add((item.FileKey, artifacts, annotated));
        usedKeys.Add(item.ExportedName);
    }

    /// <summary>Writes a FHIR export module.</summary>
    /// <param name="exportDirectory">Directory to write files.</param>
    private void WriteFhirExportModule(
        string exportDirectory)
    {
        // create a filename for writing
        string filename = Path.Combine(exportDirectory, $"fhir{_interfaceExtension}");

        using (FileStream stream = new FileStream(filename, FileMode.Create))
        using (ExportStreamWriter writer = new ExportStreamWriter(stream))
        {
            _writer = writer;

            WriteHeader(true, false, false);

            // JS requires that we export in a resolved dependency order.
            List<(string File, List<string> Tokens, List<string> Annotated)> sortedFilesAndTokens = GetSortedFilesAndTokens();

            foreach ((string exportFile, List<string> tokens, List<string> _) in sortedFilesAndTokens)
            {
                _writer.WriteLineIndented(
                    $"import {{" +
                    $" {string.Join(", ", tokens)}" +
                    $" }}" +
                    $" from './fhir/{exportFile}';");
            }

            _writer.WriteLine();
            WriteExpandedResourceBinding(true);
            _writer.WriteLine();

            _writer.WriteLine();
            WriteExpandedResourceBinding(false);
            _writer.WriteLine();
            WriteFhirResourceFactory();
            _writer.WriteLine();

            _writer.OpenScope("export {");

            foreach ((string _, List<string> _, List<string> annotated) in sortedFilesAndTokens)
            {
                _writer.WriteLineIndented(string.Join(", ", annotated) + ", ");
            }

            //foreach (string fhirKey in fhirFiles)
            //{
            //    List<string> tokens = new();

            //    if (interfaceExports.ContainsKey(fhirKey))
            //    {
            //        tokens.AddRange(interfaceExports[fhirKey].Select((i) => "type " + i));
            //    }

            //    if (classExports.ContainsKey(fhirKey))
            //    {
            //        tokens.AddRange(classExports[fhirKey]);
            //    }

            //    if (codeExports.ContainsKey(fhirKey))
            //    {
            //        tokens.AddRange(codeExports[fhirKey]);
            //    }

            //    _writer.WriteLineIndented(string.Join(", ", tokens) + ", ");
            //}

            _writer.WriteLineIndented("type IFhirResource,");

            _writer.WriteLineIndented("type FhirResource,");
            _writer.WriteLineIndented("fhirResourceFactory,");
            _writer.WriteLineIndented("fhirResourceFactoryStrict,");

            _writer.CloseScope();

            WriteFooter();
        }
    }

    /// <summary>Writes an index module.</summary>
    /// <param name="exportDirectory">Directory to write files.</param>
    private void WriteIndexModule(
        string exportDirectory)
    {
        // create a filename for writing
        string filename = Path.Combine(exportDirectory, $"index{_sourceExtension}");

        using (FileStream stream = new FileStream(filename, FileMode.Create))
        using (ExportStreamWriter writer = new ExportStreamWriter(stream))
        {
            _writer = writer;

            WriteHeader(true, false, false);

            _writer.WriteLineIndented("import * as fhir from './fhir';");

            if (_exportEnums)
            {
                _writer.WriteLineIndented("import * as fhirValueSets from './valuesets';");
                _writer.WriteLineIndented("export { fhir, fhirValueSets };");
            }
            else
            {
                _writer.WriteLineIndented("export { fhir };");
            }

            WriteFooter();
        }
    }

    /// <summary>Writes a value set module.</summary>
    /// <param name="exportDirectory">  Directory to write files.</param>
    /// <param name="exportedValueSets">The exported value sets.</param>
    private void WriteValueSetModule(
        string exportDirectory,
        List<string> exportedValueSets)
    {
        // create a filename for writing
        string filename = Path.Combine(exportDirectory, $"valuesets{_sourceExtension}");

        using (FileStream stream = new FileStream(filename, FileMode.Create))
        using (ExportStreamWriter writer = new ExportStreamWriter(stream))
        {
            _writer = writer;

            WriteHeader(true, false, false);

            foreach (string exportedSet in exportedValueSets)
            {
                _writer.WriteLineIndented($"import {{ {exportedSet} }} from './fhirValueSets/{exportedSet}'");
            }

            _writer.OpenScope("export {");

            foreach (string exportedSet in exportedValueSets)
            {
                _writer.WriteLineIndented(exportedSet + ",");
            }

            _writer.CloseScope();

            WriteFooter();
        }
    }

    /// <summary>Writes the FHIR resource factory.</summary>
    /// <param name="exportedResourceByFhirType">Dictionary mapping FHIR Resources to exported TypeScript types/classes/interfaces.</param>
    private void WriteFhirResourceFactory()
    {
        if (_exportedResources.Count == 0)
        {
            return;
        }

        // function open
        WriteIndentedComment("Factory creator for FHIR Resources");
        _writer.WriteLineIndented("function fhirResourceFactory(source:any) : FhirResource|null {");
        _writer.IncreaseIndent();

        // switch open
        _writer.WriteLineIndented("switch (source[\"resourceType\"]) {");
        _writer.IncreaseIndent();

        foreach ((string fhir, ExportedComplex complex) in _exportedResources)
        {
            _writer.WriteLineIndented($"case \"{fhir}\": return new {complex.ExportedName}(source);");
        }

        _writer.WriteLineIndented("default: return null;");

        // switch close
        _writer.CloseScope();

        // function close
        _writer.CloseScope();

        // function open
        WriteIndentedComment("Factory creator for strict FHIR Resources");
        _writer.WriteLineIndented("function fhirResourceFactoryStrict(source:any) : FhirResource|null {");
        _writer.IncreaseIndent();

        // switch open
        _writer.WriteLineIndented("switch (source[\"resourceType\"]) {");
        _writer.IncreaseIndent();

        foreach ((string fhir, ExportedComplex complex) in _exportedResources)
        {
            _writer.WriteLineIndented($"case \"{fhir}\": return {complex.ExportedName}.fromStrict(source);");
        }

        _writer.WriteLineIndented("default: return null;");

        // switch close
        _writer.CloseScope();

        // function close
        _writer.CloseScope();
    }

    /// <summary>Writes the expanded resource interface binding.</summary>
    /// <param name="exportAsInterfaces">True to export interfaces.</param>
    private void WriteExpandedResourceBinding(bool exportAsInterfaces)
    {
        if (_exportedResources.Count == 0)
        {
            return;
        }

        string prefix = exportAsInterfaces ? "I" : string.Empty;

        WriteIndentedComment("Resource binding for generic use.");

        if (_exportedResources.Count == 1)
        {
            _writer.WriteLineIndented($"type {prefix}FhirResource = {prefix}{_exportedResources.Keys.First()};");
            return;
        }

        _writer.WriteLineIndented($"type {prefix}FhirResource = ");

        _writer.IncreaseIndent();

        string delim = new string(_writer.IndentationChar, _writer.Indentation) + "\n|";
        _writer.WriteLine(string.Join(delim, _exportedResources.Values.Select((complex) => prefix + complex.ExportedName)) + ";");

        //int index = 0;
        //int last = exportedResourceByFhirType.Count - 1;
        //foreach (string exportedName in exportedResourceByFhirType.Keys)
        //{
        //    if (index == 0)
        //    {
        //        _writer.WriteLineIndented(prefix + exportedName);
        //    }
        //    else if (index == last)
        //    {
        //        _writer.WriteLineIndented("|" + prefix + exportedName + ";");
        //    }
        //    else
        //    {
        //        _writer.WriteLineIndented("|" + prefix + exportedName);
        //    }

        //    index++;
        //}

        _writer.DecreaseIndent();
    }

    /// <summary>Writes a value sets.</summary>
    /// <param name="valueSets">        List of valueSetCollections.</param>
    /// <param name="exportedValueSets">[in,out] The export value sets.</param>
    private void WriteValueSets(
        IEnumerable<FhirValueSetCollection> valueSets,
        ref List<string> exportedValueSets)
    {
        Dictionary<string, WrittenCodeInfo> writtenCodesAndNames = new ();
        HashSet<string> writtenNames = new ();

        HashSet<string> exportedNames = new ();

        foreach (FhirValueSetCollection collection in valueSets)
        {
            foreach (FhirValueSet vs in collection.ValueSetsByVersion.Values)
            {
                string vsName = FhirUtils.SanitizeForProperty(vs.Id ?? vs.Name, _reservedWords);
                vsName = FhirUtils.SanitizedToConvention(vsName, FhirTypeBase.NamingConvention.PascalCase);

                vsName = vsName + "ValueSet";

                if (exportedNames.Contains(vsName))
                {
                    Console.WriteLine($"Duplicate export name: {vsName} ({vs.Key})");
                    continue;
                }

                exportedNames.Add(vsName);

                // create a filename for writing
                string filename = Path.Combine(_directoryValueSets, $"{vsName}{_sourceExtension}");

                using (FileStream stream = new FileStream(filename, FileMode.Create))
                using (ExportStreamWriter writer = new ExportStreamWriter(stream))
                {
                    _writer = writer;

                    WriteHeader(true, false, false);

                    _writer.WriteLineIndented($"import {{ Coding }} from '../fhir'");

                    WriteValueSet(
                        vs,
                        vsName,
                        ref writtenCodesAndNames,
                        ref writtenNames);

                    WriteFooter();
                }
            }
        }

        exportedValueSets = exportedNames.ToList<string>();
    }

    /// <summary>Writes a value set.</summary>
    /// <param name="vs">                  The value set.</param>
    /// <param name="exportName">              Name of the vs.</param>
    /// <param name="writtenCodesAndNames">[in,out] List of names of the written codes ands.</param>
    /// <param name="writtenNames">        [in,out] List of names of the writtens.</param>
    private void WriteValueSet(
        FhirValueSet vs,
        string exportName,
        ref Dictionary<string, WrittenCodeInfo> writtenCodesAndNames,
        ref HashSet<string> writtenNames)
    {
        if (!string.IsNullOrEmpty(vs.Description))
        {
            WriteIndentedComment(vs.Description);
        }
        else
        {
            WriteIndentedComment($"Value Set: {vs.URL}|{vs.Version}");
        }

        _writer.WriteLineIndented($"export const {exportName} = {{");
        _writer.IncreaseIndent();

        bool prefixWithSystem = vs.ReferencedCodeSystems.Count > 1;
        HashSet<string> usedValues = new HashSet<string>();

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
                constName = $"{exportName}_{codeName}";
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

            if (!string.IsNullOrEmpty(concept.Definition))
            {
                WriteIndentedComment(concept.Definition);
            }

            _writer.WriteLineIndented($"{constName}: new Coding({{");
            _writer.IncreaseIndent();

            _writer.WriteLineIndented($"code: \"{codeValue}\",");

            if (!string.IsNullOrEmpty(concept.Display))
            {
                _writer.WriteLineIndented($"display: \"{FhirUtils.SanitizeForQuoted(concept.Display)}\",");
            }

            _writer.WriteLineIndented($"system: \"{concept.System}\"");

            _writer.DecreaseIndent();

            _writer.WriteLineIndented("}),");
        }

        _writer.DecreaseIndent();

        _writer.WriteLineIndented("};");
    }

    /// <summary>Writes the complexes.</summary>
    /// <param name="complexes">        The complexes.</param>
    /// <param name="fhirArtifactClass">The FHIR artifact class.</param>
    private void WriteComplexes(
        IEnumerable<FhirComplex> complexes,
        FhirArtifactClassEnum fhirArtifactClass)
    {
        string filename;

        ExportStringBuilder sbInterface;
        ExportStringBuilder sbClass;
        ExportStringBuilder sbCodes;

        foreach (FhirComplex complex in complexes)
        {
            sbInterface = new();
            sbClass = new();
            sbCodes = new();

            string name = FhirUtils.ToConvention(
                complex.Name,
                string.Empty,
                FhirTypeBase.NamingConvention.PascalCase,
                false,
                string.Empty,
                _reservedWords);

            ExportedComplex exportedComplex = new ExportedComplex()
            {
                FhirName = complex.Name,
                ExportedBackbones = new(),
                ExportedEnums = new(),
                FileKey = name,
            };

            BuildComplexOutput(
                complex,
                fhirArtifactClass,
                sbInterface,
                sbClass,
                sbCodes,
                ref exportedComplex);

            switch (fhirArtifactClass)
            {
                case FhirArtifactClassEnum.Resource:
                    _exportedResources.Add(exportedComplex.ExportedName, exportedComplex);
                    break;

                case FhirArtifactClassEnum.ComplexType:
                    _exportedComplexTypes.Add(exportedComplex.ExportedName, exportedComplex);
                    break;
            }

            //codeModules.Add(name, codeExports);
            //classModules.Add(name, classExports);
            //interfaceModules.Add(name, interfaceExports);

            // create a filename for writing
            filename = Path.Combine(_dirModels, $"{name}{_sourceExtension}");

            using (FileStream stream = new FileStream(filename, FileMode.Create))
            using (ExportStreamWriter writer = new ExportStreamWriter(stream))
            {
                _writer = writer;

                WriteHeader(false, true, false);

                _writer.WriteLineIndented($"import * as {_namespaceInternal} from '../fhir'");

                _writer.Write(sbInterface);
                _writer.Write(sbClass);
                _writer.Write(sbCodes);

                WriteFooter();
            }
        }
    }

    /// <summary>Writes a complex.</summary>
    /// <param name="complex">          The complex.</param>
    /// <param name="fhirArtifactClass">The FHIR artifact class.</param>
    /// <param name="sbInterface">      The string builder for interfaces.</param>
    /// <param name="sbClass">          The string builder for classes.</param>
    /// <param name="sbCodes">          The string builder codes.</param>
    /// <param name="exportedInfo">     [in,out] Information describing the exported.</param>
    private void BuildComplexOutput(
        FhirComplex complex,
        FhirArtifactClassEnum fhirArtifactClass,
        ExportStringBuilder sbInterface,
        ExportStringBuilder sbClass,
        ExportStringBuilder sbCodes,
        ref ExportedComplex exportedInfo)
    {
        // check for nested components
        if (complex.Components != null)
        {
            foreach (FhirComplex component in complex.Components.Values)
            {
                // use Unknown for backbone types
                BuildComplexOutput(
                    component,
                    FhirArtifactClassEnum.Unknown,
                    sbInterface,
                    sbClass,
                    sbCodes,
                    ref exportedInfo);
            }
        }

        if (!string.IsNullOrEmpty(complex.Comment))
        {
            WriteIndentedComment(sbInterface, complex.Comment);
            WriteIndentedComment(sbClass, complex.Comment);
        }

        string nameForExport;
        string baseClassName;
        string resourceNameForValidation = string.Empty;

        if (string.IsNullOrEmpty(complex.BaseTypeName) ||
            complex.Name.Equals("Element", StringComparison.Ordinal))
        {
            nameForExport = complex.NameForExport(FhirTypeBase.NamingConvention.PascalCase, false, string.Empty, _reservedWords);
            baseClassName = string.Empty;

            if (_exportInterfacesAsTypes)
            {
                sbInterface.WriteLineIndented($"export type I{nameForExport} = {{");
            }
            else
            {
                sbInterface.WriteLineIndented($"export interface I{nameForExport} {{");
            }

            sbClass.WriteLineIndented($"export class {nameForExport} implements {_namespaceInternal}.I{nameForExport} {{");
        }
        else if (complex.Name.Equals(complex.BaseTypeName, StringComparison.Ordinal))
        {
            nameForExport = complex.NameForExport(FhirTypeBase.NamingConvention.PascalCase, true, string.Empty, _reservedWords);
            baseClassName = string.Empty;

            if (_exportInterfacesAsTypes)
            {
                sbInterface.WriteLineIndented($"export type I{nameForExport} = {{");
            }
            else
            {
                sbInterface.WriteLineIndented($"export interface I{nameForExport} {{");
            }

            sbClass.WriteLineIndented($"export class {nameForExport} implements {_namespaceInternal}.I{nameForExport} {{");
        }
        else if ((complex.Components != null) && complex.Components.ContainsKey(complex.Path))
        {
            nameForExport = complex.NameForExport(FhirTypeBase.NamingConvention.PascalCase, true, string.Empty, _reservedWords);
            baseClassName = complex.TypeForExport(FhirTypeBase.NamingConvention.PascalCase, _primitiveTypeMap, false, string.Empty, _reservedWords);

            if (_genericsAndTypeHints.ContainsKey(complex.Path))
            {
                if (_exportInterfacesAsTypes)
                {
                    sbInterface.WriteLineIndented(
                        $"export type" +
                        $" I{nameForExport}<{_genericsAndTypeHints[complex.Path].Alias} = {_namespaceInternal}.I{_genericsAndTypeHints[complex.Path].GenericHint}>" +
                        $" = {_namespaceInternal}.I{baseClassName} & {{");
                }
                else
                {
                    sbInterface.WriteLineIndented(
                        $"export interface" +
                        $" I{nameForExport}<{_genericsAndTypeHints[complex.Path].Alias} = {_namespaceInternal}.I{_genericsAndTypeHints[complex.Path].GenericHint}>" +
                        $" extends {_namespaceInternal}.I{baseClassName} {{");
                }

                sbClass.WriteLineIndented(
                    $"export class" +
                    $" {nameForExport}<{_genericsAndTypeHints[complex.Path].Alias} = {_namespaceInternal}.{_genericsAndTypeHints[complex.Path].GenericHint}>" +
                    $" extends {_namespaceInternal}.{baseClassName} {{");
            }
            else
            {
                if (_exportInterfacesAsTypes)
                {
                    sbInterface.WriteLineIndented(
                        $"export type I{nameForExport}" +
                        $" = {_namespaceInternal}.I{baseClassName} & {{");
                }
                else
                {
                    sbInterface.WriteLineIndented(
                        $"export interface I{nameForExport}" +
                        $" extends {_namespaceInternal}.I{baseClassName} {{");
                }

                sbClass.WriteLineIndented(
                    $"export class {nameForExport}" +
                    $" extends {_namespaceInternal}.{baseClassName}" +
                    $" implements {_namespaceInternal}.I{nameForExport} {{");
            }
        }
        else
        {
            nameForExport = complex.NameForExport(FhirTypeBase.NamingConvention.PascalCase, true, string.Empty, _reservedWords);
            baseClassName = complex.TypeForExport(FhirTypeBase.NamingConvention.PascalCase, _primitiveTypeMap, false, string.Empty, _reservedWords);

            if (_genericsAndTypeHints.ContainsKey(complex.Path))
            {
                if (_exportInterfacesAsTypes)
                {
                    sbInterface.WriteLineIndented(
                        $"export type" +
                        $" I{nameForExport}<{_genericsAndTypeHints[complex.Path].Alias} = {_namespaceInternal}.I{_genericsAndTypeHints[complex.Path].GenericHint}>" +
                        $" = {_namespaceInternal}.I{baseClassName} & {{");
                }
                else
                {
                    sbInterface.WriteLineIndented(
                        $"export interface" +
                        $" I{nameForExport}<{_genericsAndTypeHints[complex.Path].Alias} = {_namespaceInternal}.I{_genericsAndTypeHints[complex.Path].GenericHint}>" +
                        $" extends {_namespaceInternal}.I{baseClassName} {{");
                }

                sbClass.WriteLineIndented(
                    $"export class" +
                    $" {nameForExport}<{_genericsAndTypeHints[complex.Path].Alias} = {_namespaceInternal}.{_genericsAndTypeHints[complex.Path].GenericHint}>" +
                    $" extends {_namespaceInternal}.{baseClassName}" +
                    $" implements {_namespaceInternal}.I{nameForExport}<{_genericsAndTypeHints[complex.Path].Alias}> {{");
            }
            else
            {
                if (_exportInterfacesAsTypes)
                {
                    sbInterface.WriteLineIndented(
                        $"export type I{nameForExport}" +
                        $" = {_namespaceInternal}.I{baseClassName} & {{");
                }
                else
                {
                    sbInterface.WriteLineIndented(
                        $"export interface I{nameForExport}" +
                        $" extends {_namespaceInternal}.I{baseClassName} = {{");
                }

                sbClass.WriteLineIndented(
                    $"export class {nameForExport}" +
                    $" extends {_namespaceInternal}.{baseClassName}" +
                    $" implements {_namespaceInternal}.I{nameForExport} {{");
            }
        }

        sbInterface.IncreaseIndent();
        sbClass.IncreaseIndent();

        switch (fhirArtifactClass)
        {
            case FhirArtifactClassEnum.Resource:
                if (nameForExport == "Resource")
                {
                    //_artifactFhirToExportDict[FhirArtifactClassEnum.Resource].Add("Resource", "Resource");

                    WriteIndentedComment(sbInterface, "Resource Type Name");
                    sbInterface.WriteLineIndented($"resourceType: string;");

                    WriteIndentedComment(sbClass, "Resource Type Name");
                    //sbClass.WriteLineIndented($"public readonly resourceType: string = 'Resource';");
                    sbClass.WriteLineIndented($"public resourceType: string;");
                }
                else if (ShouldWriteResourceName(nameForExport))
                {
                    //_artifactFhirToExportDict[FhirArtifactClassEnum.Resource].Add(complex.Name, complex.Name);

                    WriteIndentedComment(sbInterface, "Resource Type Name");
                    sbInterface.WriteLineIndented($"resourceType: \"{complex.Name}\";");

                    WriteIndentedComment(sbClass, "Resource Type Name");
                    //sbClass.WriteLineIndented($"public override readonly resourceType = \"{complex.Name}\";");
                    sbClass.WriteLineIndented($"public override resourceType: \"{complex.Name}\";");

                    resourceNameForValidation = complex.Name;
                }

                exportedInfo.ExportedName = nameForExport;
                exportedInfo.ExportedParentName = baseClassName;

                break;

            case FhirArtifactClassEnum.ComplexType:

                exportedInfo.ExportedName = nameForExport;
                exportedInfo.ExportedParentName = baseClassName;

                break;

            case FhirArtifactClassEnum.Unknown:

                exportedInfo.ExportedBackbones.Add(nameForExport);

                break;
        }

        // build elements
        BuildElements(
            complex,
            sbInterface,
            sbClass,
            out List<FhirElement> elementsWithCodes,
            out List<KeyValuePair<string, string>> fieldsAndTypes);

        // build class functions for models
        BuildClassFunctions(
            sbClass,
            nameForExport,
            complex.Path,
            !string.IsNullOrEmpty(baseClassName),
            fhirArtifactClass,
            fieldsAndTypes,
            resourceNameForValidation);

        string contents;

        if (_options.SupportFiles.TryGetInputForKey(complex.Name, out contents))
        {
            sbClass.Write(contents);
        }

        if (_options.SupportFiles.TryGetInputForKey("I" + complex.Name, out contents))
        {
            sbInterface.Write(contents);
        }

        // close interface (type)
        sbInterface.CloseScope();
        sbClass.CloseScope();

        if (_exportEnums)
        {
            foreach (FhirElement element in elementsWithCodes)
            {
                BuildCode(element, sbCodes, ref exportedInfo);
            }
        }
    }

    /// <summary>Writes a constructor.</summary>
    /// <exception cref="`">Thrown when a ` error condition occurs.</exception>
    /// <param name="sbClass">                  The string builder for class functions.</param>
    /// <param name="typeName">                 Name of the type.</param>
    /// <param name="complexPath">              Path to the current complex element.</param>
    /// <param name="hasParent">                True if has parent, false if not.</param>
    /// <param name="fhirArtifactClass">        The FHIR artifact class.</param>
    /// <param name="fieldsAndTypes">           List of types of the fields ands.</param>
    /// <param name="resourceNameForValidation">The resource name for validation.</param>
    private void BuildClassFunctions(
        ExportStringBuilder sbClass,
        string typeName,
        string complexPath,
        bool hasParent,
        FhirArtifactClassEnum fhirArtifactClass,
        List<KeyValuePair<string, string>> fieldsAndTypes,
        string resourceNameForValidation)
    {
        bool isOptional;
        bool isArray;
        string name;
        string type;

        ExportStringBuilder sbConstructor = new();
        ExportStringBuilder sbOptional = new();
        ExportStringBuilder sbStrict = new();
        ExportStringBuilder sbHasRequired = new();

        sbConstructor.SetIndent(sbClass.Indentation);
        sbOptional.SetIndent(sbClass.Indentation);
        sbStrict.SetIndent(sbClass.Indentation);
        sbHasRequired.SetIndent(sbClass.Indentation);

        // Constructor open
        WriteIndentedComment(
            sbConstructor,
            $"Default constructor for {typeName} - initializes any required elements to null if a value is not provided.");

        if (_genericsAndTypeHints.ContainsKey(complexPath))
        {
            sbConstructor.WriteLineIndented(
                $"constructor" +
                $"(source:Partial<{_namespaceInternal}.I{typeName}> = {{}}) {{");
            sbConstructor.IncreaseIndent();
            sbConstructor.WriteLineIndented("super(source);");
        }
        else if (hasParent)
        {
            sbConstructor.WriteLineIndented($"constructor(source:Partial<{_namespaceInternal}.I{typeName}> = {{}}) {{");
            sbConstructor.IncreaseIndent();
            sbConstructor.WriteLineIndented("super(source);");
        }
        else
        {
            sbConstructor.WriteLineIndented($"constructor(source:Partial<{_namespaceInternal}.I{typeName}> = {{}}) {{");
            sbConstructor.IncreaseIndent();
        }

        if (fhirArtifactClass == FhirArtifactClassEnum.Resource)
        {
            sbConstructor.WriteLineIndented($"this.resourceType = '{complexPath}';");
        }

        // HasRequired - open
        WriteIndentedComment(sbHasRequired, $"Check if the current {typeName} contains all required elements.");

        if (hasParent)
        {
            sbHasRequired.WriteLineIndented("override checkRequiredElements():string[] {");
        }
        else
        {
            sbHasRequired.WriteLineIndented("checkRequiredElements():string[] {");
        }

        sbHasRequired.IncreaseIndent();
        sbHasRequired.WriteLineIndented("var missingElements:string[] = [];");

        foreach (KeyValuePair<string, string> fieldAndType in fieldsAndTypes)
        {
            type = fieldAndType.Value.Replace("|unknown", string.Empty);

            isOptional = fieldAndType.Key.EndsWith('?');
            isArray = type.EndsWith(']');

            if (isOptional)
            {
                name = fieldAndType.Key.Substring(0, fieldAndType.Key.Length - 1);
            }
            else
            {
                name = fieldAndType.Key;

                sbConstructor.WriteLineIndented($"this.{name} = null;");
            }

            if (isArray)
            {
                // remove array syntax from type
                string value = type.Substring(0, type.Length - 2);

                if (value.Equals($"{_namespaceInternal}.FhirResource", StringComparison.Ordinal))
                {
                    sbConstructor.WriteLineIndented($"if (source[\"{name}\"]) {{");
                    sbConstructor.IncreaseIndent();
                    sbConstructor.WriteLineIndented($"this.{name} = [];");
                    sbConstructor.WriteLineIndented($"source.{name}.forEach((x) => {{");
                    sbConstructor.IncreaseIndent();
                    sbConstructor.WriteLineIndented($"var r = {_namespaceInternal}.fhirResourceFactory(x);");
                    sbConstructor.WriteLineIndented($"if (r) {{ this.{name}!.push(r); }}");
                    sbConstructor.CloseScope("});");
                    sbConstructor.CloseScope();
                }
                else if (_genericTypeHints.Contains(type))
                {
                    sbConstructor.WriteLineIndented(
                        $"if (source[\"{name}\"])" +
                        $" {{ this.{name} = source.{name}.map(" +
                        $"(x) => new {value}(x));" +
                        $" }}");
                }
                else if (!type.StartsWith(_namespaceInternal))
                {
                    sbConstructor.WriteLineIndented(
                        $"if (source[\"{name}\"])" +
                        $" {{ this.{name} = source.{name}.map(" +
                        $"(x) => (x));" +
                        $" }}");
                }
                else
                {
                    string interfaceHint = value.Insert(_namespaceInternal.Length + 1, "I");

                    sbConstructor.WriteLineIndented(
                        $"if (source[\"{name}\"])" +
                        $" {{ this.{name} = source.{name}.map(" +
                        $"(x:Partial<{interfaceHint}>) => new {value}(x));" +
                        $" }}");
                }

                if (!isOptional)
                {
                    sbHasRequired.WriteLineIndented(
                        $"if ((!this[\"{name}\"]) ||" +
                        $" (this[\"{name}\"].length === 0))" +
                        $" {{ missingElements.push(\"{name}\"); }}");

                    sbConstructor.WriteLineIndented($"if (this.{name} === undefined) {{ this.{name} = null }}");
                }
            }
            else
            {
                if (type.Equals($"{_namespaceInternal}.FhirResource", StringComparison.Ordinal))
                {
                    sbConstructor.WriteLineIndented(
                        $"if (source[\"{name}\"])" +
                        $" {{ this.{name} = ({_namespaceInternal}.fhirResourceFactory(source.{name}) ?? undefined); }}");
                }
                else if (_genericTypeHints.Contains(type))
                {
                    sbConstructor.WriteLineIndented(
                        $"if (source[\"{name}\"])" +
                        $" {{ this.{name} = ({_namespaceInternal}.fhirResourceFactory(source.{name}) ?? undefined)" +
                        $" as unknown as {type}|undefined; }}");
                }
                else if (!type.StartsWith(_namespaceInternal))
                {
                    sbConstructor.WriteLineIndented(
                        $"if (source[\"{name}\"])" +
                        $" {{ this.{name} = source.{name}; }}");
                }
                else
                {
                    sbConstructor.WriteLineIndented(
                        $"if (source[\"{name}\"])" +
                        $" {{ this.{name} = new {type}(source.{name}!); }}");
                }

                if (!isOptional)
                {
                    sbHasRequired.WriteLineIndented(
                        $"if (this[\"{name}\"] === undefined)" +
                        $" {{ missingElements.push(\"{name}\"); }}");

                    sbConstructor.WriteLineIndented($"if (this.{name} === undefined) {{ this.{name} = null }}");
                }
            }
        }

        // constructor - close
        sbConstructor.CloseScope();

        // HasRequired - check parent
        if (hasParent)
        {
            sbHasRequired.WriteLineIndented("var parentMissing:string[] = super.checkRequiredElements();");
            sbHasRequired.WriteLineIndented("missingElements.push(...parentMissing);");
        }

        // HasRequired - close
        sbHasRequired.WriteLineIndented("return missingElements;");
        sbHasRequired.CloseScope();

        // Strict Create - open
        WriteIndentedComment(
            sbStrict,
            $"Factory function to create a {typeName} from an object that MUST contain all required elements.");

        if (hasParent)
        {
            sbStrict.WriteLineIndented(
                $"static override fromStrict(source:{_namespaceInternal}.I{typeName})" +
                $":{typeName} {{");
        }
        else
        {
            sbStrict.WriteLineIndented(
                $"static fromStrict(source:{_namespaceInternal}.I{typeName})" +
                $":{typeName} {{");
        }

        sbStrict.IncreaseIndent();
        sbStrict.WriteLineIndented($"var dest:{typeName} = new {typeName}(source);");
        sbStrict.WriteLineIndented("var missingElements:string[] = dest.checkRequiredElements();");
        sbStrict.WriteLineIndented($"if (missingElements.length !== 0) {{ throw `{typeName} is missing elements: ${{missingElements.join(\", \")}}` }}");
        sbStrict.WriteLineIndented($"return dest;");

        // Strict Create - close
        sbStrict.CloseScope();

        sbClass.Append(sbConstructor);
        sbClass.Append(sbOptional);
        sbClass.Append(sbHasRequired);
        sbClass.Append(sbStrict);
    }

    /// <summary>Writes a code.</summary>
    /// <param name="element">        The element.</param>
    /// <param name="sb">             The string builder to write content into.</param>
    /// <param name="exportedComplex">[in,out] The exported complex.</param>
    private void BuildCode(
        FhirElement element,
        ExportStringBuilder sb,
        ref ExportedComplex exportedComplex)
    {
        string codeName = FhirUtils.ToConvention(
            $"{element.Path}",
            string.Empty,
            FhirTypeBase.NamingConvention.PascalCase);

        if (codeName.Contains("[x]", StringComparison.OrdinalIgnoreCase))
        {
            codeName = codeName.Replace("[x]", string.Empty, StringComparison.OrdinalIgnoreCase);
        }

        codeName += "Enum";

        if (exportedComplex.ExportedEnums.Contains(codeName))
        {
            // TODO: does this need to be file local?
            return;
        }

        exportedComplex.ExportedEnums.Add(codeName);

        WriteIndentedComment(sb, $"Code Values for the {element.Path} field");

        sb.WriteLineIndented($"export enum {codeName} {{");
        sb.IncreaseIndent();

        if (_info.TryGetValueSet(element.ValueSet, out FhirValueSet vs))
        {
            foreach (FhirConcept concept in vs.Concepts)
            {
                FhirUtils.SanitizeForCode(concept.Code, _reservedWords, out string name, out string value);

                sb.WriteLineIndented($"{name.ToUpperInvariant()} = \"{value}\",");
            }
        }
        else
        {
            foreach (string code in element.Codes)
            {
                FhirUtils.SanitizeForCode(code, _reservedWords, out string name, out string value);

                sb.WriteLineIndented($"{name.ToUpperInvariant()} = \"{value}\",");
            }
        }

        sb.CloseScope();
    }

    /// <summary>Writes a code.</summary>
    /// <param name="element">          The element.</param>
    /// <param name="sbInterfaceStrict">The strict interface.</param>
    /// <param name="sbInterfaceOpt">   The optinal interface.</param>
    /// <param name="sbClassStrict">    The strict class.</param>
    /// <param name="sbClassOpt">       The optional class.</param>
    private void BuildCode(
        FhirElement element,
        ExportStringBuilder sbInterfaceStrict,
        ExportStringBuilder sbInterfaceOpt,
        ExportStringBuilder sbClassStrict,
        ExportStringBuilder sbClassOpt,
        ref ExportedComplex exportedComplex)
    {
        string codeName = FhirUtils.ToConvention(
            $"{element.Path}",
            string.Empty,
            FhirTypeBase.NamingConvention.PascalCase);

        if (codeName.Contains("[x]", StringComparison.OrdinalIgnoreCase))
        {
            codeName = codeName.Replace("[x]", string.Empty, StringComparison.OrdinalIgnoreCase);
        }

        codeName += "Enum";

        if (exportedComplex.ExportedEnums.Contains(codeName))
        {
            // TODO: does this need to be file local?
            return;
        }

        exportedComplex.ExportedEnums.Add(codeName);

        WriteIndentedComment(
            $"Code Values for the {element.Path} field",
            sbInterfaceStrict,
            sbInterfaceOpt,
            sbClassStrict,
            sbClassOpt);

        sbInterfaceStrict.WriteLineIndented($"export enum {codeName} {{");
        sbInterfaceStrict.IncreaseIndent();

        sbInterfaceOpt.WriteLineIndented($"export enum {codeName} {{");
        sbInterfaceOpt.IncreaseIndent();

        sbClassStrict.WriteLineIndented($"export enum {codeName} {{");
        sbClassStrict.IncreaseIndent();

        sbClassOpt.WriteLineIndented($"export enum {codeName} {{");
        sbClassOpt.IncreaseIndent();

        if (_info.TryGetValueSet(element.ValueSet, out FhirValueSet vs))
        {
            foreach (FhirConcept concept in vs.Concepts)
            {
                FhirUtils.SanitizeForCode(concept.Code, _reservedWords, out string name, out string value);

                sbInterfaceStrict.WriteLineIndented($"{name.ToUpperInvariant()} = \"{value}\",");
                sbInterfaceOpt.WriteLineIndented($"{name.ToUpperInvariant()} = \"{value}\",");
                sbClassStrict.WriteLineIndented($"{name.ToUpperInvariant()} = \"{value}\",");
                sbClassOpt.WriteLineIndented($"{name.ToUpperInvariant()} = \"{value}\",");
            }
        }
        else
        {
            foreach (string code in element.Codes)
            {
                FhirUtils.SanitizeForCode(code, _reservedWords, out string name, out string value);

                sbInterfaceStrict.WriteLineIndented($"{name.ToUpperInvariant()} = \"{value}\",");
                sbInterfaceOpt.WriteLineIndented($"{name.ToUpperInvariant()} = \"{value}\",");
                sbClassStrict.WriteLineIndented($"{name.ToUpperInvariant()} = \"{value}\",");
                sbClassOpt.WriteLineIndented($"{name.ToUpperInvariant()} = \"{value}\",");
            }
        }

        sbInterfaceStrict.CloseScope();
        sbInterfaceOpt.CloseScope();
        sbClassStrict.CloseScope();
        sbClassOpt.CloseScope();
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
    /// <param name="sbInterface">      The interface.</param>
    /// <param name="sbClass">          The string builder for class functions.</param>
    /// <param name="elementsWithCodes">[out] The elements with codes.</param>
    /// <param name="fieldsAndTypes">   [out] List of types of the fields ands.</param>
    private void BuildElements(
        FhirComplex complex,
        ExportStringBuilder sbInterface,
        ExportStringBuilder sbClass,
        out List<FhirElement> elementsWithCodes,
        out List<KeyValuePair<string, string>> fieldsAndTypes)
    {
        elementsWithCodes = new();

        fieldsAndTypes = new();

        foreach (FhirElement element in complex.Elements.Values.OrderBy(s => s.Name))
        {
            if (element.IsInherited)
            {
                continue;
            }

            BuildElement(
                complex,
                element,
                sbInterface,
                sbClass,
                ref fieldsAndTypes);

            if ((element.Codes != null) && (element.Codes.Count > 0))
            {
                elementsWithCodes.Add(element);
            }
        }
    }

    /// <summary>Writes an element.</summary>
    /// <param name="complex">       The complex.</param>
    /// <param name="element">       The element.</param>
    /// <param name="sbInterface">   The interface.</param>
    /// <param name="sbClass">       The string builder for class functions.</param>
    /// <param name="fieldsAndTypes">[in,out] List of field names and types.</param>
    private void BuildElement(
        FhirComplex complex,
        FhirElement element,
        ExportStringBuilder sbInterface,
        ExportStringBuilder sbClass,
        ref List<KeyValuePair<string, string>> fieldsAndTypes)
    {
        bool isOptional = false;
        string optFlag = "?";
        string optType = "|undefined";
        string strictFlag = string.Empty;
        string strictType = string.Empty;
        string arrayFlagString = element.IsArray ? "[]" : string.Empty;

        Dictionary<string, string> values = element.NamesAndTypesForExport(
            FhirTypeBase.NamingConvention.CamelCase,
            FhirTypeBase.NamingConvention.PascalCase,
            false,
            string.Empty,
            complex.Components.ContainsKey(element.Path));

        if (element.IsOptional || (values.Count > 1))
        {
            isOptional = true;
            strictFlag = optFlag;
            strictType = optType;
        }
        else
        {
            strictType = "|null";
        }

        foreach (KeyValuePair<string, string> kvp in values)
        {
            if (!string.IsNullOrEmpty(element.Comment))
            {
                WriteIndentedComment(element.Comment, sbInterface, sbClass);
            }

            KeyValuePair<string, string> nameAndType;

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
                        $"{element.Path}.Enum",
                        string.Empty,
                        FhirTypeBase.NamingConvention.PascalCase);

                    nameAndType = new(kvp.Key, $"{codeName}{arrayFlagString}");
                }
                else if (_info.TryGetValueSet(element.ValueSet, out FhirValueSet vs))
                {
                    // use the full expansion
                    nameAndType = new(
                        kvp.Key,
                        $"({string.Join("|", vs.Concepts.Select(c => $"\"{c.Code}\""))}){arrayFlagString}");
                }
                else
                {
                    // otherwise, inline the required codes
                    nameAndType = new(
                        kvp.Key,
                        $"({string.Join("|", element.Codes.Select(c => $"\"{c}\""))}){arrayFlagString}");
                }
            }
            else if (_genericsAndTypeHints.ContainsKey(element.Path))
            {
                GenericTypeHintInfo typeHint = _genericsAndTypeHints[element.Path];

                if (typeHint.IncludeBase)
                {
                    nameAndType = new(
                        kvp.Key,
                        $"{kvp.Value}<{_genericsAndTypeHints[element.Path].Alias}>{arrayFlagString}");
                }
                else
                {
                    nameAndType = new(
                        kvp.Key,
                        $"{_genericsAndTypeHints[element.Path].Alias}{arrayFlagString}");
                }
            }
            else if (kvp.Value.Equals("Resource", StringComparison.Ordinal))
            {
                nameAndType = new(kvp.Key, $"{_namespaceInternal}.FhirResource{arrayFlagString}");
            }
            else if (_primitiveTypeMap.ContainsKey(kvp.Value))
            {
                nameAndType = new(kvp.Key, $"{kvp.Value}{arrayFlagString}");
            }
            else if (_info.ExcludedKeys.Contains(kvp.Value))
            {
                nameAndType = new(kvp.Key, $"any{arrayFlagString}");
            }
            else
            {
                nameAndType = new(kvp.Key, $"{_namespaceInternal}.{kvp.Value}{arrayFlagString}");
            }

            // TODO(gino): interface probably needs to check for sub-type being interface too...
            if (nameAndType.Value.Contains(_namespaceInternal))
            {
                string replacement = nameAndType.Value.Insert(_namespaceInternal.Length + 1, "I");

                sbInterface.WriteLineIndented($"{nameAndType.Key}{strictFlag}: {replacement}{strictType};");
            }
            else if (nameAndType.Value.Contains('<', StringComparison.Ordinal))
            {
                sbInterface.WriteLineIndented($"{nameAndType.Key}{strictFlag}: I{nameAndType.Value}{strictType};");
            }
            else
            {
                sbInterface.WriteLineIndented($"{nameAndType.Key}{strictFlag}: {nameAndType.Value}{strictType};");
            }

            sbClass.WriteLineIndented($"public {nameAndType.Key}{strictFlag}: {nameAndType.Value}{strictType};");

            if (isOptional)
            {
                fieldsAndTypes.Add(new(nameAndType.Key + strictFlag, nameAndType.Value));
            }
            else
            {
                fieldsAndTypes.Add(nameAndType);
            }

            if (RequiresExtension(kvp.Value))
            {
                sbInterface.WriteLineIndented($"_{kvp.Key}?: {_namespaceInternal}.IFhirElement{arrayFlagString}|undefined;");
                sbClass.WriteLineIndented($"public _{kvp.Key}?: {_namespaceInternal}.FhirElement{arrayFlagString}|undefined;");

                fieldsAndTypes.Add(new(
                    $"_{kvp.Key}?",
                    $"{_namespaceInternal}.FhirElement{arrayFlagString}"));
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
    /// <param name="isValueSet">     True if this is the header for a ValueSet.</param>
    /// <param name="isModel">        True if this is the header for a Model.</param>
    /// <param name="isSerialization">True if is serialization, false if not.</param>
    private void WriteHeader(bool isValueSet, bool isModel, bool isSerialization)
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

        _writer.WriteLine($"// Minimum TypeScript Version: {_minimumTypeScriptVersion}");
    }

    /// <summary>Writes a footer.</summary>
    private void WriteFooter()
    {
        return;
    }

    /// <summary>Writes an indented comment.</summary>
    /// <param name="sb">       The sb.</param>
    /// <param name="value">    The value.</param>
    /// <param name="isSummary">(Optional) True if is summary, false if not.</param>
    private void WriteIndentedComment(ExportStringBuilder sb, string value, bool isSummary = true)
    {
        string comment;
        string[] lines;

        sb.WriteLineIndented("/**");

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
            sb.WriteIndented(" * ");
            sb.WriteLine(line);
        }

        sb.WriteLineIndented(" */");
    }

    /// <summary>Writes an indented comment.</summary>
    /// <param name="value">    The value.</param>
    /// <param name="sb1">      The 1.</param>
    /// <param name="sb2">      The 2.</param>
    /// <param name="sb3">      The 3.</param>
    /// <param name="sb4">      The 4.</param>
    /// <param name="isSummary">(Optional) True if is summary, false if not.</param>
    private void WriteIndentedComment(
        string value,
        ExportStringBuilder sb1,
        ExportStringBuilder sb2,
        ExportStringBuilder sb3,
        ExportStringBuilder sb4,
        bool isSummary = true)
    {
        string comment;
        string[] lines;

        sb1.WriteLineIndented("/**");
        sb2.WriteLineIndented("/**");
        sb3.WriteLineIndented("/**");
        sb4.WriteLineIndented("/**");

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
            sb1.WriteIndented(" * ");
            sb1.WriteLine(line);

            sb2.WriteIndented(" * ");
            sb2.WriteLine(line);

            sb3.WriteIndented(" * ");
            sb3.WriteLine(line);

            sb4.WriteIndented(" * ");
            sb4.WriteLine(line);
        }

        sb1.WriteLineIndented(" */");
        sb2.WriteLineIndented(" */");
        sb3.WriteLineIndented(" */");
        sb4.WriteLineIndented(" */");
    }

    /// <summary>Writes an indented comment.</summary>
    /// <param name="value">    The value.</param>
    /// <param name="sb1">      The 1.</param>
    /// <param name="sb2">      The 2.</param>
    /// <param name="isSummary">(Optional) True if is summary, false if not.</param>
    private void WriteIndentedComment(
        string value,
        ExportStringBuilder sb1,
        ExportStringBuilder sb2,
        bool isSummary = true)
    {
        string comment;
        string[] lines;

        sb1.WriteLineIndented("/**");
        sb2.WriteLineIndented("/**");

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
            sb1.WriteIndented(" * ");
            sb1.WriteLine(line);

            sb2.WriteIndented(" * ");
            sb2.WriteLine(line);
        }

        sb1.WriteLineIndented(" */");
        sb2.WriteLineIndented(" */");
    }

    /// <summary>Writes an indented comment.</summary>
    /// <param name="value">    The value.</param>
    /// <param name="isSummary">(Optional) True if is summary, false if not.</param>
    private void WriteIndentedComment(string value, bool isSummary = true)
    {
        string comment;
        string[] lines;

        _writer.WriteLineIndented("/**");

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
            _writer.WriteIndented(" * ");
            _writer.WriteLine(line);
        }

        _writer.WriteLineIndented(" */");
    }

    /// <summary>Information about written codes.</summary>
    private struct WrittenCodeInfo
    {
        internal string Name;
        internal string ConstName;
    }

    /// <summary>Information about the generic type hint.</summary>
    private struct GenericTypeHintInfo
    {
        internal string Alias;
        internal bool IncludeBase;
        internal string GenericHint;
    }
}
