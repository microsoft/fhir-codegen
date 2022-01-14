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

    /// <summary>Pathname of the model directory.</summary>
    private string _directoryModels;

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

        Dictionary<string, List<string>> modulesAndExports = new ();

        WriteComplexes(_info.ComplexTypes.Values, ref modulesAndExports, false);
        WriteComplexes(_info.Resources.Values, ref modulesAndExports, true);

        WriteModelModule(exportDirectory, modulesAndExports);

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

            _writer.WriteLineIndented("import * as Models from './models';");

            if (_exportEnums)
            {
                _writer.WriteLineIndented("import * as ValueSets from './valuesets';");
                _writer.WriteLineIndented("export { Models, ValueSets };");
            }
            else
            {
                _writer.WriteLineIndented("export { Models };");
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
    /// <param name="modulesAndExports">The modules and exports.</param>
    private void WriteModelModule(
        string exportDirectory,
        Dictionary<string, List<string>> modulesAndExports)
    {
        // create a filename for writing
        string filename = Path.Combine(exportDirectory, "models.ts");

        using (FileStream stream = new FileStream(filename, FileMode.Create))
        using (ExportStreamWriter writer = new ExportStreamWriter(stream))
        {
            _writer = writer;

            WriteHeader(true, false, false);

            foreach (KeyValuePair<string, List<string>> moduleAndExports in modulesAndExports)
            {
                _writer.WriteLineIndented($"import {{ {string.Join(", ", moduleAndExports.Value)} }} from './Models/{moduleAndExports.Key}';");
            }

            _writer.OpenScope("export {");

            foreach (List<string> exports in modulesAndExports.Values)
            {
                _writer.WriteLineIndented(string.Join(", ", exports) + ",");
            }

            _writer.CloseScope();

            WriteExpandedResourceInterfaceBinding();

            WriteFooter();
        }
    }

    /// <summary>Writes the expanded resource interface binding.</summary>
    private void WriteExpandedResourceInterfaceBinding()
    {
        if (_exportedResourceNamesAndTypes.Count == 0)
        {
            return;
        }

        WriteIndentedComment("Resource binding for generic use.");

        if (_exportedResourceNamesAndTypes.Count == 1)
        {
            _writer.WriteLineIndented($"export type FhirResource = {_exportedResourceNamesAndTypes.Keys.First()};");
            return;
        }

        _writer.WriteLineIndented("export type FhirResource = ");

        _writer.IncreaseIndent();

        int index = 0;
        int last = _exportedResourceNamesAndTypes.Count - 1;
        foreach (string exportedName in _exportedResourceNamesAndTypes.Keys)
        {
            if (index == 0)
            {
                _writer.WriteLineIndented(exportedName);
            }
            else if (index == last)
            {
                _writer.WriteLineIndented("|" + exportedName + ";");
            }
            else
            {
                _writer.WriteLineIndented("|" + exportedName);
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
    /// <param name="complexes">        The complexes.</param>
    /// <param name="modulesAndExports">[in,out] The modules and exports.</param>
    /// <param name="isResource">       (Optional) True if is resource, false if not.</param>
    private void WriteComplexes(
        IEnumerable<FhirComplex> complexes,
        ref Dictionary<string, List<string>> modulesAndExports,
        bool isResource = false)
    {
        if (modulesAndExports == null)
        {
            modulesAndExports = new ();
        }

        foreach (FhirComplex complex in complexes)
        {
            // create a filename for writing
            string filename = Path.Combine(_directoryModels, $"{complex.NameCapitalized}.ts");

            List<string> exports = new ();

            using (FileStream stream = new FileStream(filename, FileMode.Create))
            using (ExportStreamWriter writer = new ExportStreamWriter(stream))
            {
                _writer = writer;

                WriteHeader(false, true, false);

                _writer.WriteLineIndented($"import * as {_namespaceModels} from '../models'");

                WriteComplex(complex, isResource, ref exports);

                WriteFooter();
            }

            modulesAndExports.Add(complex.NameCapitalized, exports);
        }
    }

    /// <summary>Writes a complex.</summary>
    /// <param name="complex">   The complex.</param>
    /// <param name="isResource">True if is resource, false if not.</param>
    /// <param name="exports">   The exports.</param>
    private void WriteComplex(
        FhirComplex complex,
        bool isResource,
        ref List<string> exports)
    {
        if (exports == null)
        {
            exports = new ();
        }

        // check for nested components
        if (complex.Components != null)
        {
            foreach (FhirComplex component in complex.Components.Values)
            {
                WriteComplex(component, false, ref exports);
            }
        }

        if (!string.IsNullOrEmpty(complex.Comment))
        {
            WriteIndentedComment(complex.Comment);
        }

        string nameForExport;
        string baseClassName;
        string resourceNameForValidation = string.Empty;

        if (string.IsNullOrEmpty(complex.BaseTypeName) ||
            complex.Name.Equals("Element", StringComparison.Ordinal))
        {
            nameForExport = complex.NameForExport(FhirTypeBase.NamingConvention.PascalCase);
            baseClassName = string.Empty;

            _writer.WriteLineIndented($"export class {nameForExport} {{");
        }
        else if (complex.Name.Equals(complex.BaseTypeName, StringComparison.Ordinal))
        {
            nameForExport = complex.NameForExport(FhirTypeBase.NamingConvention.PascalCase, true);
            baseClassName = string.Empty;

            _writer.WriteLineIndented($"export class {nameForExport} {{");
        }
        else if ((complex.Components != null) && complex.Components.ContainsKey(complex.Path))
        {
            nameForExport = complex.NameForExport(FhirTypeBase.NamingConvention.PascalCase, true);
            baseClassName = complex.TypeForExport(FhirTypeBase.NamingConvention.PascalCase, _primitiveTypeMap, false);

            if (_genericsAndTypeHints.ContainsKey(complex.Path))
            {
                _writer.WriteLineIndented(
                    $"export class" +
                    $" {nameForExport}<{_genericsAndTypeHints[complex.Path].Alias} = {_namespaceModels}.{_genericsAndTypeHints[complex.Path].GenericHint}>" +
                    $" extends {_namespaceModels}.{baseClassName} {{");
            }
            else
            {
                _writer.WriteLineIndented($"export class {nameForExport} extends {_namespaceModels}.{baseClassName} {{");
            }
        }
        else
        {
            nameForExport = complex.NameForExport(FhirTypeBase.NamingConvention.PascalCase, true);
            baseClassName = complex.TypeForExport(FhirTypeBase.NamingConvention.PascalCase, _primitiveTypeMap);

            if (_genericsAndTypeHints.ContainsKey(complex.Path))
            {
                _writer.WriteLineIndented(
                    $"export class" +
                    $" {nameForExport}<{_genericsAndTypeHints[complex.Path].Alias} = {_namespaceModels}.{_genericsAndTypeHints[complex.Path].GenericHint}>" +
                    $" extends {_namespaceModels}.{baseClassName} {{");
            }
            else
            {
                _writer.WriteLineIndented($"export class {nameForExport} extends {_namespaceModels}.{baseClassName} {{");
            }
        }

        exports.Add(nameForExport);

        _writer.IncreaseIndent();

        if (isResource)
        {
            if (nameForExport == "Resource")
            {
                WriteIndentedComment("Resource Type Name");
                _writer.WriteLineIndented($"readonly resourceType: string = 'Resource';");
            }
            else if (ShouldWriteResourceName(nameForExport))
            {
                _exportedResourceNamesAndTypes.Add(complex.Name, complex.Name);

                WriteIndentedComment("Resource Type Name");
                _writer.WriteLineIndented($"readonly resourceType: string = \"{complex.Name}\";");

                resourceNameForValidation = complex.Name;
            }
        }

        // write elements
        WriteElements(
            complex,
            out List<FhirElement> elementsWithCodes,
            out List<KeyValuePair<string, string>> fieldsAndTypes);

        // write constructor
        WriteConstructor(
            nameForExport,
            !string.IsNullOrEmpty(baseClassName),
            fieldsAndTypes,
            resourceNameForValidation);

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

    /// <summary>Writes a constructor.</summary>
    /// <param name="hasParent">                True if has parent, false if not.</param>
    /// <param name="fieldsAndTypes">           List of types of the fields ands.</param>
    /// <param name="resourceNameForValidation">The resource name for validation.</param>
    private void WriteConstructor(
        string typeName,
        bool hasParent,
        List<KeyValuePair<string, string>> fieldsAndTypes,
        string resourceNameForValidation)
    {
        WriteIndentedComment("Default constructor");

        _writer.OpenScope($"constructor(source: {typeName}) {{");

        if (hasParent)
        {
            _writer.WriteLineIndented("super(source);");
        }

        if (!string.IsNullOrEmpty(resourceNameForValidation))
        {
            _writer.WriteLineIndented(
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
                _writer.WriteLineIndented($"if (source[\"{name}\"] !== undefined) {{ this.{name} = source.{name}{genericCast}; }}");
            }
            else
            {
                name = fieldAndType.Key;
                _writer.WriteLineIndented($"if (source[\"{name}\"] === undefined) {{ throw 'Missing required element {name}';}}");
                _writer.WriteLineIndented($"this.{name} = source.{name}{genericCast};");
            }
        }

        // constructor contents
        _writer.CloseScope();
    }

    /// <summary>Writes a code.</summary>
    /// <param name="element">The element.</param>
    private void WriteCode(
        FhirElement element)
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

        WriteIndentedComment($"Code Values for the {element.Path} field");
        _writer.WriteLineIndented($"export enum {codeName} {{");

        _writer.IncreaseIndent();

        if (_info.TryGetValueSet(element.ValueSet, out FhirValueSet vs))
        {
            foreach (FhirConcept concept in vs.Concepts)
            {
                FhirUtils.SanitizeForCode(concept.Code, _reservedWords, out string name, out string value);

                _writer.WriteLineIndented($"{name.ToUpperInvariant()} = \"{value}\",");
            }
        }
        else
        {
            foreach (string code in element.Codes)
            {
                FhirUtils.SanitizeForCode(code, _reservedWords, out string name, out string value);

                _writer.WriteLineIndented($"{name.ToUpperInvariant()} = \"{value}\",");
            }
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
    /// <param name="fieldsAndTypes">   [out] List of types of the fields ands.</param>
    private void WriteElements(
        FhirComplex complex,
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

            WriteElement(complex, element, ref fieldsAndTypes);

            if ((element.Codes != null) && (element.Codes.Count > 0))
            {
                elementsWithCodes.Add(element);
            }
        }
    }

    /// <summary>Writes an element.</summary>
    /// <param name="complex">       The complex.</param>
    /// <param name="element">       The element.</param>
    /// <param name="fieldsAndTypes">[in,out] List of types of the fields ands.</param>
    private void WriteElement(
        FhirComplex complex,
        FhirElement element,
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
                WriteIndentedComment(element.Comment);
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

            _writer.WriteLineIndented($"{nameAndType.Key}: {nameAndType.Value};");
            fieldsAndTypes.Add(nameAndType);

            if (RequiresExtension(kvp.Value))
            {
                _writer.WriteLineIndented($"_{kvp.Key}?: {_namespaceModels}.Element{arrayFlagString};");

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
