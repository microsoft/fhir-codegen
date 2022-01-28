// <copyright file="TypeScript2.cs" company="Microsoft Corporation">
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
    private string _namespaceModels;

    /// <summary>The namespace interfaces.</summary>
    private string _namespaceInterfaces;

    /// <summary>Pathname of the model directory.</summary>
    private string _directoryModels;

    /// <summary>Pathname of the interface directory.</summary>
    private string _directoryInterfaces;

    /// <summary>Pathname of the value set directory.</summary>
    private string _directoryValueSets;

    /// <summary>List of types of the exported resource names and types.</summary>
    private Dictionary<string, string> _exportedResourceNamesAndTypes = new Dictionary<string, string>();

    /// <summary>The exported codes.</summary>
    private HashSet<string> _exportedCodes = new HashSet<string>();

    /// <summary>The currently in-use text writer.</summary>
    private ExportStreamWriter _writer;

    /// <summary>Name of the language.</summary>
    private const string _languageName = "TypeScript2";

    /// <summary>The minimum type script version.</summary>
    private const string _minimumTypeScriptVersion = "3.7";

    /// <summary>The single file export extension - requires directory export.</summary>
    private const string _singleFileExportExtension = null;

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
    private static readonly HashSet<string> _reservedWords = new ()
    {
        "const",
        "enum",
        "export",
        "interface",
    };

    /// <summary>The generics and type hints.</summary>
    private static readonly Dictionary<string, GenericTypeHintInfo> _genericsAndTypeHints = new ()
    {
        {
            "Bundle",
            new GenericTypeHintInfo()
            {
                Alias = "BundleContentType",
                GenericHint = "FhirResource",
                IncludeBase = true,
            }
        },
        {
            "Bundle.entry",
            new GenericTypeHintInfo()
            {
                Alias = "BundleContentType",
                GenericHint = "FhirResource",
                IncludeBase = true,
            }
        },
        {
            "Bundle.entry.resource",
            new GenericTypeHintInfo()
            {
                Alias = "BundleContentType",
                GenericHint = string.Empty,
                IncludeBase = false,
            }
        },
    };

    /// <summary>(Immutable) The Set of all known generic types (needed for casting).</summary>
    private static readonly HashSet<string> _genericTypeHints = new ()
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
        _namespaceModels = "fhirModels";
        _namespaceInterfaces = "fhirInterfaces";

        _exportedResourceNamesAndTypes = new Dictionary<string, string>();
        _exportedCodes = new HashSet<string>();

        _directoryModels = Path.Combine(exportDirectory, "Models");
        if (!Directory.Exists(_directoryModels))
        {
            Directory.CreateDirectory(_directoryModels);
        }

        _directoryInterfaces = Path.Combine(exportDirectory, "Interfaces");
        if (!Directory.Exists(_directoryInterfaces))
        {
            Directory.CreateDirectory(_directoryInterfaces);
        }

        _directoryValueSets = Path.Combine(exportDirectory, "ValueSets");
        if (_exportEnums)
        {
            if (!Directory.Exists(_directoryValueSets))
            {
                Directory.CreateDirectory(_directoryValueSets);
            }
        }

        Dictionary<string, List<string>> classExports = new ();
        Dictionary<string, List<string>> interfaceExports = new ();

        WriteComplexes(_info.ComplexTypes.Values, interfaceExports, classExports, false);
        WriteComplexes(_info.Resources.Values, interfaceExports, classExports, true);

        WriteInterfaceModule(exportDirectory, interfaceExports);
        WriteModelModule(exportDirectory, classExports);

        if (_exportEnums)
        {
            List<string> exportedValueSets = new ();

            WriteValueSets(_info.ValueSetsByUrl.Values, ref exportedValueSets);

            WriteValueSetModule(exportDirectory, exportedValueSets);
        }

        WriteIndexModule(exportDirectory);
    }

    /// <summary>Writes an index module.</summary>
    /// <param name="exportDirectory">Directory to write files.</param>
    private void WriteIndexModule(
        string exportDirectory)
    {
        // create a filename for writing
        string filename = Path.Combine(exportDirectory, "index.ts");

        using (FileStream stream = new FileStream(filename, FileMode.Create))
        using (ExportStreamWriter writer = new ExportStreamWriter(stream))
        {
            _writer = writer;

            WriteHeader(true, false, false);

            _writer.WriteLineIndented("import * as Interfaces from './interfaces';");
            _writer.WriteLineIndented("import * as Models from './models';");

            if (_exportEnums)
            {
                _writer.WriteLineIndented("import * as ValueSets from './valuesets';");
                _writer.WriteLineIndented("export { Interfaces, Models, ValueSets };");
            }
            else
            {
                _writer.WriteLineIndented("export { Interfaces, Models };");
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
        string filename = Path.Combine(exportDirectory, "valuesets.ts");

        using (FileStream stream = new FileStream(filename, FileMode.Create))
        using (ExportStreamWriter writer = new ExportStreamWriter(stream))
        {
            _writer = writer;

            WriteHeader(true, false, false);

            foreach (string exportedSet in exportedValueSets)
            {
                _writer.WriteLineIndented($"import {{ {exportedSet} }} from './ValueSets/{exportedSet}'");
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

    /// <summary>Writes a model module.</summary>
    /// <param name="exportDirectory">  Directory to write files.</param>
    /// <param name="interfaceExports">The modules and exports.</param>
    private void WriteInterfaceModule(
        string exportDirectory,
        Dictionary<string, List<string>> interfaceExports)
    {
        // create a filename for writing
        string filename = Path.Combine(exportDirectory, "interfaces.d.ts");

        using (FileStream stream = new FileStream(filename, FileMode.Create))
        using (ExportStreamWriter writer = new ExportStreamWriter(stream))
        {
            _writer = writer;

            WriteHeader(true, false, false);

            foreach (KeyValuePair<string, List<string>> moduleAndExports in interfaceExports)
            {
                _writer.WriteLineIndented(
                    $"import {{" +
                    $" {string.Join(", ", moduleAndExports.Value)}" +
                    $" }}" +
                    $" from './Interfaces/{moduleAndExports.Key}';");
            }

            _writer.WriteLine();
            WriteExpandedResourceInterfaceBinding(true);
            _writer.WriteLine();

            _writer.OpenScope("export {");

            foreach (List<string> exports in interfaceExports.Values)
            {
                _writer.WriteLineIndented(string.Join(", ", exports) + ",");
            }

            _writer.WriteLineIndented("IFhirResource,");
            _writer.CloseScope();

            WriteFooter();
        }
    }

    /// <summary>Writes a model module.</summary>
    /// <param name="exportDirectory">  Directory to write files.</param>
    /// <param name="classExports">The modules and exports.</param>
    private void WriteModelModule(
        string exportDirectory,
        Dictionary<string, List<string>> classExports)
    {
        // create a filename for writing
        string filename = Path.Combine(exportDirectory, "models.ts");

        using (FileStream stream = new FileStream(filename, FileMode.Create))
        using (ExportStreamWriter writer = new ExportStreamWriter(stream))
        {
            _writer = writer;

            WriteHeader(true, false, false);

            foreach (KeyValuePair<string, List<string>> moduleAndExports in classExports)
            {
                _writer.WriteLineIndented(
                    $"import {{" +
                    $" {string.Join(", ", moduleAndExports.Value)}" +
                    $" }}" +
                    $" from './Models/{moduleAndExports.Key}';");
            }

            _writer.WriteLine();
            WriteExpandedResourceInterfaceBinding(false);
            _writer.WriteLine();
            WriteFhirResourceFactory();
            _writer.WriteLine();

            _writer.OpenScope("export {");

            foreach (List<string> exports in classExports.Values)
            {
                _writer.WriteLineIndented(string.Join(", ", exports) + ",");
            }

            _writer.WriteLineIndented("type FhirResource,");
            _writer.WriteLineIndented("FhirResourceFactory,");
            _writer.CloseScope();

            WriteFooter();
        }
    }

    /// <summary>Writes the FHIR resource factory.</summary>
    private void WriteFhirResourceFactory()
    {
        if (_exportedResourceNamesAndTypes.Count == 0)
        {
            return;
        }

        WriteIndentedComment("Factory creator for FHIR Resources");

        // function open
        _writer.WriteLineIndented("function FhirResourceFactory(source:any) : FhirResource|null {");
        _writer.IncreaseIndent();

        // switch open
        _writer.WriteLineIndented("switch (source[\"resourceType\"]) {");
        _writer.IncreaseIndent();

        foreach (KeyValuePair<string, string> kvp in _exportedResourceNamesAndTypes)
        {
            _writer.WriteLineIndented($"case \"{kvp.Key}\": return new {kvp.Value}(source);");
        }

        _writer.WriteLineIndented("default: return null;");

        // switch close
        _writer.CloseScope();

        // function close
        _writer.CloseScope();
    }

    /// <summary>Writes the expanded resource interface binding.</summary>
    /// <param name="exportAsInterfaces">True to export interfaces.</param>
    private void WriteExpandedResourceInterfaceBinding(bool exportAsInterfaces)
    {
        if (_exportedResourceNamesAndTypes.Count == 0)
        {
            return;
        }

        string prefix = exportAsInterfaces ? "I" : string.Empty;

        WriteIndentedComment("Resource binding for generic use.");

        if (_exportedResourceNamesAndTypes.Count == 1)
        {
            _writer.WriteLineIndented($"type {prefix}FhirResource = {prefix}{_exportedResourceNamesAndTypes.Keys.First()};");
            return;
        }

        _writer.WriteLineIndented($"type {prefix}FhirResource = ");

        _writer.IncreaseIndent();

        int index = 0;
        int last = _exportedResourceNamesAndTypes.Count - 1;
        foreach (string exportedName in _exportedResourceNamesAndTypes.Keys)
        {
            if (index == 0)
            {
                _writer.WriteLineIndented(prefix + exportedName);
            }
            else if (index == last)
            {
                _writer.WriteLineIndented("|" + prefix + exportedName + ";");
            }
            else
            {
                _writer.WriteLineIndented("|" + prefix + exportedName);
            }

            index++;
        }

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

                if (exportedNames.Contains(vsName))
                {
                    Console.WriteLine($"Duplicate export name: {vsName} ({vs.Key})");
                    continue;
                }

                exportedNames.Add(vsName);

                // create a filename for writing
                string filename = Path.Combine(_directoryValueSets, $"{vsName}.ts");

                using (FileStream stream = new FileStream(filename, FileMode.Create))
                using (ExportStreamWriter writer = new ExportStreamWriter(stream))
                {
                    _writer = writer;

                    WriteHeader(true, false, false);

                    _writer.WriteLineIndented($"import {{ Coding }} from '../models'");

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

            _writer.WriteLineIndented($"{constName}: {{");
            _writer.IncreaseIndent();

            _writer.WriteLineIndented($"code: \"{codeValue}\",");

            if (!string.IsNullOrEmpty(concept.Display))
            {
                _writer.WriteLineIndented($"display: \"{FhirUtils.SanitizeForQuoted(concept.Display)}\",");
            }

            _writer.WriteLineIndented($"system: \"{concept.System}\"");

            _writer.DecreaseIndent();

            _writer.WriteLineIndented("} as Coding,");
        }

        _writer.DecreaseIndent();

        _writer.WriteLineIndented("};");
    }

    /// <summary>Writes the complexes.</summary>
    /// <param name="complexes">       The complexes.</param>
    /// <param name="interfaceModules">The interface modules.</param>
    /// <param name="classModules">    The modules and exports.</param>
    /// <param name="isResource">      (Optional) True if is resource, false if not.</param>
    private void WriteComplexes(
        IEnumerable<FhirComplex> complexes,
        Dictionary<string, List<string>> interfaceModules,
        Dictionary<string, List<string>> classModules,
        bool isResource = false)
    {
        string filename;

        List<string> classExports;
        List<string> interfaceExports;

        ExportStringBuilder sbInterface;
        ExportStringBuilder sbClass;

        foreach (FhirComplex complex in complexes)
        {
            interfaceExports = new();
            classExports = new();
            sbInterface = new();
            sbClass = new();

            BuildComplexOutput(complex, isResource, interfaceExports, classExports, sbInterface, sbClass);

            // create a filename for writing
            filename = Path.Combine(_directoryModels, $"{complex.NameCapitalized}.ts");

            using (FileStream stream = new FileStream(filename, FileMode.Create))
            using (ExportStreamWriter writer = new ExportStreamWriter(stream))
            {
                _writer = writer;

                WriteHeader(false, true, false);

                _writer.WriteLineIndented($"import * as {_namespaceModels} from '../models'");
                _writer.WriteLineIndented($"import * as {_namespaceInterfaces} from '../interfaces'");

                _writer.Write(sbClass.ToString());

                WriteFooter();
            }

            classModules.Add(complex.NameCapitalized, classExports);
            interfaceModules.Add("I" + complex.NameCapitalized, interfaceExports);

            // create a filename for writing
            filename = Path.Combine(_directoryInterfaces, $"I{complex.NameCapitalized}.d.ts");

            using (FileStream stream = new FileStream(filename, FileMode.Create))
            using (ExportStreamWriter writer = new ExportStreamWriter(stream))
            {
                _writer = writer;

                WriteHeader(false, true, false);

                _writer.WriteLineIndented($"import * as {_namespaceInterfaces} from '../interfaces'");

                _writer.Write(sbInterface.ToString());

                WriteFooter();
            }
        }
    }

    /// <summary>Writes a complex.</summary>
    /// <param name="complex">         The complex.</param>
    /// <param name="isResource">      True if is resource, false if not.</param>
    /// <param name="interfaceExports">The exports.</param>
    /// <param name="classExports">    The class exports.</param>
    /// <param name="sbInterface">     The interface.</param>
    /// <param name="sbClass">         The class.</param>
    private void BuildComplexOutput(
        FhirComplex complex,
        bool isResource,
        List<string> interfaceExports,
        List<string> classExports,
        ExportStringBuilder sbInterface,
        ExportStringBuilder sbClass)
    {
        // check for nested components
        if (complex.Components != null)
        {
            foreach (FhirComplex component in complex.Components.Values)
            {
                BuildComplexOutput(component, false, interfaceExports, classExports, sbInterface, sbClass);
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
            nameForExport = complex.NameForExport(FhirTypeBase.NamingConvention.PascalCase);
            baseClassName = string.Empty;

            sbInterface.WriteLineIndented($"export interface I{nameForExport} {{");
            sbClass.WriteLineIndented($"export class {nameForExport} implements {_namespaceInterfaces}.I{nameForExport} {{");
        }
        else if (complex.Name.Equals(complex.BaseTypeName, StringComparison.Ordinal))
        {
            nameForExport = complex.NameForExport(FhirTypeBase.NamingConvention.PascalCase, true);
            baseClassName = string.Empty;

            sbInterface.WriteLineIndented($"export interface I{nameForExport} {{");
            sbClass.WriteLineIndented($"export class {nameForExport} implements {_namespaceInterfaces}.I{nameForExport} {{");
        }
        else if ((complex.Components != null) && complex.Components.ContainsKey(complex.Path))
        {
            nameForExport = complex.NameForExport(FhirTypeBase.NamingConvention.PascalCase, true);
            baseClassName = complex.TypeForExport(FhirTypeBase.NamingConvention.PascalCase, _primitiveTypeMap, false);

            if (_genericsAndTypeHints.ContainsKey(complex.Path))
            {
                sbInterface.WriteLineIndented(
                    $"export interface" +
                    $" I{nameForExport}<{_genericsAndTypeHints[complex.Path].Alias} = {_namespaceInterfaces}.I{_genericsAndTypeHints[complex.Path].GenericHint}>" +
                    $" extends {_namespaceInterfaces}.I{baseClassName} {{");

                sbClass.WriteLineIndented(
                    $"export class" +
                    $" {nameForExport}<{_genericsAndTypeHints[complex.Path].Alias} = {_namespaceModels}.{_genericsAndTypeHints[complex.Path].GenericHint}>" +
                    $" extends {_namespaceModels}.{baseClassName} {{");
            }
            else
            {
                sbInterface.WriteLineIndented(
                    $"export interface I{nameForExport}" +
                    $" extends {_namespaceInterfaces}.I{baseClassName} {{");

                sbClass.WriteLineIndented(
                    $"export class {nameForExport}" +
                    $" extends {_namespaceModels}.{baseClassName}" +
                    $" implements {_namespaceInterfaces}.I{nameForExport} {{");
            }
        }
        else
        {
            nameForExport = complex.NameForExport(FhirTypeBase.NamingConvention.PascalCase, true);
            baseClassName = complex.TypeForExport(FhirTypeBase.NamingConvention.PascalCase, _primitiveTypeMap);

            if (_genericsAndTypeHints.ContainsKey(complex.Path))
            {
                sbInterface.WriteLineIndented(
                    $"export interface" +
                    $" I{nameForExport}<{_genericsAndTypeHints[complex.Path].Alias} = {_namespaceInterfaces}.I{_genericsAndTypeHints[complex.Path].GenericHint}>" +
                    $" extends {_namespaceInterfaces}.I{baseClassName} {{");

                sbClass.WriteLineIndented(
                    $"export class" +
                    $" {nameForExport}<{_genericsAndTypeHints[complex.Path].Alias} = {_namespaceModels}.{_genericsAndTypeHints[complex.Path].GenericHint}>" +
                    $" extends {_namespaceModels}.{baseClassName}" +
                    $" implements {_namespaceInterfaces}.I{nameForExport}<{_genericsAndTypeHints[complex.Path].Alias}> {{");
            }
            else
            {
                sbInterface.WriteLineIndented(
                    $"export interface I{nameForExport}" +
                    $" extends {_namespaceInterfaces}.I{baseClassName} {{");

                sbClass.WriteLineIndented(
                    $"export class {nameForExport}" +
                    $" extends {_namespaceModels}.{baseClassName}" +
                    $" implements {_namespaceInterfaces}.I{nameForExport} {{");
            }
        }

        interfaceExports.Add("I" + nameForExport);
        classExports.Add(nameForExport);

        sbInterface.IncreaseIndent();
        sbClass.IncreaseIndent();

        if (isResource)
        {
            if (nameForExport == "Resource")
            {
                WriteIndentedComment(sbInterface, "Resource Type Name");
                sbInterface.WriteLineIndented($"readonly resourceType: string;");

                WriteIndentedComment(sbClass, "Resource Type Name");
                sbClass.WriteLineIndented($"readonly resourceType: string = 'Resource';");
            }
            else if (ShouldWriteResourceName(nameForExport))
            {
                _exportedResourceNamesAndTypes.Add(complex.Name, complex.Name);

                WriteIndentedComment(sbInterface, "Resource Type Name");
                sbInterface.WriteLineIndented($"readonly resourceType: \"{complex.Name}\";");

                WriteIndentedComment(sbClass, "Resource Type Name");
                sbClass.WriteLineIndented($"readonly resourceType = \"{complex.Name}\";");

                resourceNameForValidation = complex.Name;
            }
        }

        // write elements
        BuildElements(
            complex,
            sbInterface,
            sbClass,
            out List<FhirElement> elementsWithCodes,
            out List<KeyValuePair<string, string>> fieldsAndTypes);

        // write constructor
        BuildClassStaticFunctions(
            sbClass,
            nameForExport,
            !string.IsNullOrEmpty(baseClassName),
            fieldsAndTypes,
            resourceNameForValidation);

        // close interface (type)
        sbInterface.CloseScope();
        sbClass.CloseScope();

        if (_exportEnums)
        {
            foreach (FhirElement element in elementsWithCodes)
            {
                BuildCode(element, sbInterface, sbClass);
            }
        }
    }

    /// <summary>Writes a constructor.</summary>
    /// <param name="sbClass">                  The class.</param>
    /// <param name="typeName">                 Name of the type.</param>
    /// <param name="hasParent">                True if has parent, false if not.</param>
    /// <param name="fieldsAndTypes">           List of types of the fields ands.</param>
    /// <param name="resourceNameForValidation">The resource name for validation.</param>
    private void BuildClassStaticFunctions(
        ExportStringBuilder sbClass,
        string typeName,
        bool hasParent,
        List<KeyValuePair<string, string>> fieldsAndTypes,
        string resourceNameForValidation)
    {
        List<string> constructorArgs = new();
        List<string> copyLines = new();

        bool isOptional;
        bool isArray;
        string name;
        //string genericCast;

        ExportStringBuilder sbConstructor = new();
        ExportStringBuilder sbStrict = new();
        ExportStringBuilder sbHasRequired = new();

        sbConstructor.SetIndent(sbClass.Indentation);
        sbStrict.SetIndent(sbClass.Indentation);
        sbHasRequired.SetIndent(sbClass.Indentation);

        // Constructor - open
        WriteIndentedComment(sbConstructor, $"Default constructor for {typeName} from an object that MAY NOT contain all required elements.");
        sbConstructor.WriteLineIndented($"constructor(source:Partial<{_namespaceInterfaces}.I{typeName}>) {{");
        sbConstructor.IncreaseIndent();

        if (hasParent)
        {
            sbConstructor.WriteLineIndented($"super(source);");
        }

        if (!string.IsNullOrEmpty(resourceNameForValidation))
        {
            sbConstructor.WriteLineIndented(
                $"if ((source['resourceType'] !== \"{resourceNameForValidation}\") || (source['resourceType'] !== undefined))" +
                $" {{ throw 'Invalid resourceType for a {resourceNameForValidation}'; }}");
        }

        // HasRequired - open
        WriteIndentedComment(sbHasRequired, $"Check if the current {typeName} contains all required elements.");
        sbHasRequired.WriteLineIndented("checkRequiredElements():string[] {");
        sbHasRequired.IncreaseIndent();
        sbHasRequired.WriteLineIndented("var missingElements:string[] = [];");

        foreach (KeyValuePair<string, string> fieldAndType in fieldsAndTypes)
        {
            //if ((typeName == "BundleEntry") && (fieldAndType.Key.Contains("resource", StringComparison.OrdinalIgnoreCase)))
            //{
            //    Console.Write("");
            //}

            isOptional = fieldAndType.Key.EndsWith('?');
            isArray = fieldAndType.Value.EndsWith(']');

            //genericCast = _genericTypeHints.Contains(fieldAndType.Value)
            //    ? $" as {fieldAndType.Value}|undefined"
            //    : string.Empty;

            if (isOptional)
            {
                name = fieldAndType.Key.Substring(0, fieldAndType.Key.Length - 1);
            }
            else
            {
                name = fieldAndType.Key;
            }

            if (isArray)
            {
                // remove array syntax from type
                string value = fieldAndType.Value.Substring(0, fieldAndType.Value.Length - 2);

                if (value.Equals($"{_namespaceModels}.FhirResource", StringComparison.Ordinal))
                {
                    sbConstructor.WriteLineIndented($"if (source[\"{name}\"] !== undefined) {{");
                    sbConstructor.IncreaseIndent();
                    sbConstructor.WriteLineIndented($"this.{name} = [];");
                    sbConstructor.WriteLineIndented($"source.{name}.forEach((x) => {{");
                    sbConstructor.IncreaseIndent();
                    sbConstructor.WriteLineIndented($"var r = {_namespaceModels}.FhirResourceFactory(x);");
                    sbConstructor.WriteLineIndented($"if (r) {{ this.{name}!.push(r); }}");
                    sbConstructor.CloseScope("});");
                    sbConstructor.CloseScope();
                }
                else if (_genericTypeHints.Contains(fieldAndType.Value))
                {
                    sbConstructor.WriteLineIndented(
                        $"if (source[\"{name}\"] !== undefined)" +
                        $" {{ this.{name} = source.{name}.map((x) => new {value}(x)); }}");

                    //sbConstructor.WriteLineIndented($"if (source[\"{name}\"] !== undefined) {{");
                    //sbConstructor.IncreaseIndent();
                    //sbConstructor.WriteLineIndented($"this.{name} = [];");
                    //sbConstructor.WriteLineIndented($"source.{name}.forEach((x) => {{");
                    //sbConstructor.IncreaseIndent();
                    //sbConstructor.WriteLineIndented($"var r = {_namespaceModels}.FhirResourceFactory(x);");
                    //sbConstructor.WriteLineIndented($"if (r) {{ this.{name}!.push(r as {value}); }}");
                    //sbConstructor.CloseScope("});");
                    //sbConstructor.CloseScope();
                }
                else if (!fieldAndType.Value.StartsWith(_namespaceModels))
                {
                    sbConstructor.WriteLineIndented(
                        $"if (source[\"{name}\"] !== undefined)" +
                        $" {{ this.{name} = source.{name}.map((x) => (x)); }}");
                }
                else
                {
                    sbConstructor.WriteLineIndented(
                        $"if (source[\"{name}\"] !== undefined)" +
                        $" {{ this.{name} = source.{name}.map((x) => new {value}(x)); }}");
                }

                if (!isOptional)
                {
                    sbHasRequired.WriteLineIndented(
                        $"if ((this[\"{name}\"] === undefined) ||" +
                        $" (this[\"{name}\"].length === 0))" +
                        $" {{ missingElements.push(\"{name}\"); }}");
                }
            }
            else
            {
                if (fieldAndType.Value.Equals($"{_namespaceModels}.FhirResource", StringComparison.Ordinal))
                {
                    sbConstructor.WriteLineIndented(
                        $"if (source[\"{name}\"] !== undefined)" +
                        $" {{ this.{name} = ({_namespaceModels}.FhirResourceFactory(source.{name}) ?? undefined); }}");
                }
                else if (_genericTypeHints.Contains(fieldAndType.Value))
                {
                    sbConstructor.WriteLineIndented(
                        $"if (source[\"{name}\"] !== undefined) {{" +
                        $" this.{name} = ({_namespaceModels}.FhirResourceFactory(source.{name}) ?? undefined)" +
                        $" as unknown as {fieldAndType.Value}|undefined;" +
                        $" }}");
                }
                else if (!fieldAndType.Value.StartsWith(_namespaceModels))
                {
                    sbConstructor.WriteLineIndented(
                        $"if (source[\"{name}\"] !== undefined)" +
                        $" {{ this.{name} = source.{name}; }}");
                        //$" {{ this.{name} = source.{name}{genericCast}; }}");
                }
                else
                {
                    sbConstructor.WriteLineIndented(
                        $"if (source[\"{name}\"] !== undefined)" +
                        $" {{ this.{name} = new {fieldAndType.Value}(source.{name}); }}");
                }

                if (!isOptional)
                {
                    sbHasRequired.WriteLineIndented(
                        $"if (this[\"{name}\"] === undefined)" +
                        $" {{ missingElements.push(\"{name}\"); }}");
                }
            }
        }

        // CreatePartial - close
        sbConstructor.CloseScope();

        // HasRequired - close
        if (hasParent)
        {
            sbHasRequired.WriteLineIndented("var parentMissing:string[] = super.checkRequiredElements();");
            sbHasRequired.WriteLineIndented("missingElements.push(...parentMissing);");
        }

        sbHasRequired.WriteLineIndented("return missingElements;");
        sbHasRequired.CloseScope();

        // CreateStrict - open
        WriteIndentedComment(sbStrict, $"Factory function to create a {typeName} from an object that MUST contain all required elements.");
        sbStrict.WriteLineIndented(
            $"static CreateStrict(source:{_namespaceInterfaces}.I{typeName})" +
            $":{typeName} {{");
        sbStrict.IncreaseIndent();

        sbStrict.WriteLineIndented($"var dest:{typeName} = new {typeName}(source);");
        sbStrict.WriteLineIndented("var missingElements:string[] = dest.checkRequiredElements();");
        sbStrict.WriteLineIndented("if (missingElements.length !== 0) {");
        sbStrict.WriteLineIndented($"throw `{typeName} is missing elements: ${{missingElements.join(\", \")}}`");
        sbStrict.WriteLineIndented(" }");
        sbStrict.WriteLineIndented($"return dest;");

        // CreateStrict - close
        sbStrict.CloseScope();

        sbClass.Append(sbConstructor);
        sbClass.Append(sbHasRequired);
        sbClass.Append(sbStrict);

        //// CreateStrict - open
        //WriteIndentedComment(sbClass, $"Factory function to create a {typeName} from an object that MUST contain all required elements.");
        //sbClass.WriteLineIndented(
        //    $"static CreateStrict(source:{_namespaceInterfaces}.I{typeName})" +
        //    $":{typeName} {{");
        //sbClass.IncreaseIndent();

        //sbClass.WriteLineIndented($"var dest:{typeName} = new {typeName}();");

        //if (!string.IsNullOrEmpty(resourceNameForValidation))
        //{
        //    sbClass.WriteLineIndented(
        //        $"if ((source['resourceType'] !== \"{resourceNameForValidation}\") || (source['resourceType'] !== undefined))" +
        //        $" {{ throw 'Invalid resourceType for a {resourceNameForValidation}'; }}");
        //}

        //foreach (KeyValuePair<string, string> fieldAndType in fieldsAndTypes)
        //{
        //    isOptional = fieldAndType.Key.EndsWith('?');
        //    isArray = fieldAndType.Value.EndsWith(']');

        //    genericCast = _genericTypeHints.Contains(fieldAndType.Value)
        //        ? $" as unknown as {fieldAndType.Value}"
        //        : string.Empty;

        //    if (isArray)
        //    {
        //        // remove array syntax
        //        string value = fieldAndType.Value.Substring(0, fieldAndType.Value.Length - 2);

        //        if (isOptional)
        //        {
        //            name = fieldAndType.Key.Substring(0, fieldAndType.Key.Length - 1);

        //            sbClass.WriteLineIndented($"if (source[\"{name}\"] !== undefined) {{");
        //            sbClass.IncreaseIndent();

        //            if (!fieldAndType.Value.StartsWith(_namespaceModels))
        //            {
        //                sbClass.WriteLineIndented($"dest.{name} = [...source.{name}{genericCast}];");
        //            }
        //            else if (!string.IsNullOrEmpty(genericCast))
        //            {
        //                sbClass.WriteLineIndented(
        //                    $"dest.{name} = source.{name}.map((x) => {value}.CreatePartial(x));");
        //            }
        //            else
        //            {
        //                sbClass.WriteLineIndented(
        //                    $"dest.{name} = source.{name}.map((x) => {value}.CreatePartial(x));");
        //            }

        //            sbClass.CloseScope();
        //        }
        //        else
        //        {
        //            name = fieldAndType.Key;

        //            sbClass.WriteLineIndented($"if (source[\"{name}\"] === undefined) {{ throw 'Missing required element {name}';}}");
        //            if (!fieldAndType.Value.StartsWith(_namespaceModels))
        //            {
        //                sbClass.WriteLineIndented($"dest.{name} = [...source.{name}{genericCast}];");
        //            }
        //            else if (!string.IsNullOrEmpty(genericCast))
        //            {
        //                sbClass.WriteLineIndented(
        //                    $"dest.{name} = source.{name}.map((x) => {value}.CreatePartial(x));");
        //            }
        //            else
        //            {
        //                sbClass.WriteLineIndented(
        //                    $"dest.{name} = source.{name}.map((x) => {value}.CreatePartial(x));");
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (isOptional)
        //        {
        //            name = fieldAndType.Key.Substring(0, fieldAndType.Key.Length - 1);
        //            sbClass.WriteLineIndented($"if (source[\"{name}\"] !== undefined) {{ dest.{name} = source.{name}{genericCast}; }}");
        //        }
        //        else
        //        {
        //            name = fieldAndType.Key;
        //            sbClass.WriteLineIndented($"if (source[\"{name}\"] === undefined) {{ throw 'Missing required element {name}';}}");
        //            sbClass.WriteLineIndented($"dest.{name} = source.{name}{genericCast};");
        //        }
        //    }
        //}

        //sbClass.WriteLineIndented("return dest;");

        //// CreateStrict - close
        //sbClass.CloseScope();
    }

    /// <summary>Writes a constructor.</summary>
    /// <param name="sbClass">                  The class.</param>
    /// <param name="typeName">                 Name of the type.</param>
    /// <param name="hasParent">                True if has parent, false if not.</param>
    /// <param name="fieldsAndTypes">           List of types of the fields ands.</param>
    /// <param name="resourceNameForValidation">The resource name for validation.</param>
    private void BuildConstructorV0(
        ExportStringBuilder sbClass,
        string typeName,
        bool hasParent,
        List<KeyValuePair<string, string>> fieldsAndTypes,
        string resourceNameForValidation)
    {
        List<string> constructorArgs = new();
        List<string> copyLines = new();

        WriteIndentedComment(sbClass, "Default constructor");

        sbClass.OpenScope($"constructor(source: {typeName}) {{");

        if (hasParent)
        {
            sbClass.WriteLineIndented("super(source);");
        }

        if (!string.IsNullOrEmpty(resourceNameForValidation))
        {
            sbClass.WriteLineIndented(
                $"if ((source['resourceType'] !== \"{resourceNameForValidation}\") || (source['resourceType'] !== undefined))" +
                $" {{ throw 'Invalid resourceType for a {resourceNameForValidation}'; }}");
        }

        bool isOptional;
        string name;
        string genericCast;

        foreach (KeyValuePair<string, string> fieldAndType in fieldsAndTypes)
        {
            isOptional = fieldAndType.Key.EndsWith('?');

            genericCast = _genericTypeHints.Contains(fieldAndType.Value)
                ? $" as unknown as {fieldAndType.Value}"
                : string.Empty;

            if (isOptional)
            {
                name = fieldAndType.Key.Substring(0, fieldAndType.Key.Length - 1);
                sbClass.WriteLineIndented($"if (source[\"{name}\"] !== undefined) {{ this.{name} = source.{name}{genericCast}; }}");
            }
            else
            {
                name = fieldAndType.Key;
                sbClass.WriteLineIndented($"if (source[\"{name}\"] === undefined) {{ throw 'Missing required element {name}';}}");
                sbClass.WriteLineIndented($"this.{name} = source.{name}{genericCast};");
            }
        }

        // constructor contents
        sbClass.CloseScope();
    }

    /// <summary>Writes a code.</summary>
    /// <param name="element">The element.</param>
    private void BuildCode(
        FhirElement element,
        ExportStringBuilder sbInterface,
        ExportStringBuilder sbClass)
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

        if (_exportedCodes.Contains(codeName))
        {
            return;
        }

        _exportedCodes.Add(codeName);

        WriteIndentedComment(sbInterface, $"Code Values for the {element.Path} field");
        sbInterface.WriteLineIndented($"export enum {codeName} {{");
        sbInterface.IncreaseIndent();

        WriteIndentedComment(sbClass, $"Code Values for the {element.Path} field");
        sbClass.WriteLineIndented($"export enum {codeName} {{");
        sbClass.IncreaseIndent();

        if (_info.TryGetValueSet(element.ValueSet, out FhirValueSet vs))
        {
            foreach (FhirConcept concept in vs.Concepts)
            {
                FhirUtils.SanitizeForCode(concept.Code, _reservedWords, out string name, out string value);

                sbInterface.WriteLineIndented($"{name.ToUpperInvariant()} = \"{value}\",");
                sbClass.WriteLineIndented($"{name.ToUpperInvariant()} = \"{value}\",");
            }
        }
        else
        {
            foreach (string code in element.Codes)
            {
                FhirUtils.SanitizeForCode(code, _reservedWords, out string name, out string value);

                sbInterface.WriteLineIndented($"{name.ToUpperInvariant()} = \"{value}\",");
                sbClass.WriteLineIndented($"{name.ToUpperInvariant()} = \"{value}\",");
            }
        }

        sbInterface.CloseScope();
        sbClass.CloseScope();
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
    /// <param name="sbClass">          The class.</param>
    /// <param name="elementsWithCodes">[out] The elements with codes.</param>
    /// <param name="fieldsAndTypes">   [out] List of types of the fields ands.</param>
    private void BuildElements(
        FhirComplex complex,
        ExportStringBuilder sbInterface,
        ExportStringBuilder sbClass,
        out List<FhirElement> elementsWithCodes,
        out List<KeyValuePair<string, string>> fieldsAndTypes)
    {
        elementsWithCodes = new ();

        fieldsAndTypes = new ();

        foreach (FhirElement element in complex.Elements.Values.OrderBy(s => s.Name))
        {
            if (element.IsInherited)
            {
                continue;
            }

            BuildElement(complex, element, sbInterface, sbClass, ref fieldsAndTypes);

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
    /// <param name="sbClass">       The class.</param>
    /// <param name="fieldsAndTypes">[in,out] List of field names and types.</param>
    private void BuildElement(
        FhirComplex complex,
        FhirElement element,
        ExportStringBuilder sbInterface,
        ExportStringBuilder sbClass,
        ref List<KeyValuePair<string, string>> fieldsAndTypes)
    {
        string optionalFlagString = element.IsOptional ? "?" : string.Empty;
        string arrayFlagString = element.IsArray ? "[]" : string.Empty;

        Dictionary<string, string> values = element.NamesAndTypesForExport(
            FhirTypeBase.NamingConvention.CamelCase,
            FhirTypeBase.NamingConvention.PascalCase,
            false,
            string.Empty,
            complex.Components.ContainsKey(element.Path));

        if ((values.Count > 1) &&
            (!element.IsOptional) &&
            string.IsNullOrEmpty(optionalFlagString))
        {
            optionalFlagString = "?";
        }

        foreach (KeyValuePair<string, string> kvp in values)
        {
            if (!string.IsNullOrEmpty(element.Comment))
            {
                WriteIndentedComment(sbInterface, element.Comment);
                WriteIndentedComment(sbClass, element.Comment);
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

                    nameAndType = new (
                        $"{kvp.Key}{optionalFlagString}",
                        $"{codeName}{arrayFlagString}");
                }
                else if (_info.TryGetValueSet(element.ValueSet, out FhirValueSet vs))
                {
                    // use the full expansion
                    nameAndType = new (
                        $"{kvp.Key}{optionalFlagString}",
                        $"({string.Join("|", vs.Concepts.Select(c => $"\"{c.Code}\""))}){arrayFlagString}");
                }
                else
                {
                    // otherwise, inline the required codes
                    nameAndType = new (
                        $"{kvp.Key}{optionalFlagString}",
                        $"({string.Join("|", element.Codes.Select(c => $"\"{c}\""))}){arrayFlagString}");
                }
            }
            else if (_genericsAndTypeHints.ContainsKey(element.Path))
            {
                //       this.resource = fhirModels.FhirResourceFactory(source.resource) as unknown as BundleContentType;
                GenericTypeHintInfo typeHint = _genericsAndTypeHints[element.Path];

                if (typeHint.IncludeBase)
                {
                    nameAndType = new (
                        $"{kvp.Key}{optionalFlagString}",
                        $"{kvp.Value}<{_genericsAndTypeHints[element.Path].Alias}>{arrayFlagString}");
                }
                else
                {
                    nameAndType = new (
                        $"{kvp.Key}{optionalFlagString}",
                        $"{_genericsAndTypeHints[element.Path].Alias}{arrayFlagString}");
                }
            }
            else if (kvp.Value.Equals("Resource", StringComparison.Ordinal))
            {
                nameAndType = new ($"{kvp.Key}{optionalFlagString}", $"{_namespaceModels}.FhirResource{arrayFlagString}");
            }
            else if (_primitiveTypeMap.ContainsKey(kvp.Value))
            {
                nameAndType = new ($"{kvp.Key}{optionalFlagString}", $"{kvp.Value}{arrayFlagString}");
            }
            else
            {
                nameAndType = new ($"{kvp.Key}{optionalFlagString}", $"{_namespaceModels}.{kvp.Value}{arrayFlagString}");
            }

            // TODO(gino): interface probably needs to check for sub-type being interface too...
            if (nameAndType.Value.Contains(_namespaceModels))
            {
                string replacement = nameAndType.Value.Replace(_namespaceModels + ".", _namespaceInterfaces + ".I");
                sbInterface.WriteLineIndented($"{nameAndType.Key}: {replacement}|undefined;");
            }
            else if (nameAndType.Value.Contains('<', StringComparison.Ordinal))
            {
                sbInterface.WriteLineIndented($"{nameAndType.Key}: I{nameAndType.Value}|undefined;");
            }
            else
            {
                sbInterface.WriteLineIndented($"{nameAndType.Key}: {nameAndType.Value}|undefined;");
            }

            sbClass.WriteLineIndented($"{nameAndType.Key}: {nameAndType.Value}|undefined;");

            fieldsAndTypes.Add(nameAndType);

            if (RequiresExtension(kvp.Value))
            {
                sbInterface.WriteLineIndented($"_{kvp.Key}?: {_namespaceInterfaces}.IElement{arrayFlagString}|undefined;");
                sbClass.WriteLineIndented($"_{kvp.Key}?: {_namespaceModels}.Element{arrayFlagString}|undefined;");

                fieldsAndTypes.Add(new (
                    $"_{kvp.Key}?",
                    $"{_namespaceModels}.Element{arrayFlagString}"));
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
