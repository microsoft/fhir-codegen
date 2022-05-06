// <copyright file="TypeScriptSdk.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.IO;
using Microsoft.Health.Fhir.SpecManager.Manager;
using Microsoft.Health.Fhir.SpecManager.Models;
using static Microsoft.Health.Fhir.SpecManager.Language.TypeScriptSdk.TypeScriptSdkCommon;

namespace Microsoft.Health.Fhir.SpecManager.Language.TypeScriptSdk;

/// <summary>A type script sdk. This class cannot be inherited.</summary>
public sealed class TypeScriptSdk : ILanguage
{
    /// <summary>FHIR information we are exporting.</summary>
    private FhirVersionInfo _info;

    /// <summary>Options for controlling the export.</summary>
    private ExporterOptions _options;

    /// <summary>True to export enums.</summary>
    private bool _exportEnums = true;

    /// <summary>Name of the language.</summary>
    private const string _languageName = "TypeScriptSdk";

    /// <summary>The minimum type script version.</summary>
    private const string _minimumTypeScriptVersion = "3.7";

    /// <summary>The single file export extension - requires directory export.</summary>
    private const string _singleFileExportExtension = null;

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
    Dictionary<string, string> ILanguage.FhirPrimitiveTypeMap => PrimitiveTypeMap;

    /// <summary>Gets the reserved words.</summary>
    /// <value>The reserved words.</value>
    HashSet<string> ILanguage.ReservedWords => ReservedWords;

    /// <summary>
    /// Gets a list of FHIR class types that the language WILL export, regardless of user choices.
    /// Used to provide information to users.
    /// </summary>
    List<ExporterOptions.FhirExportClassType> ILanguage.RequiredExportClassTypes => new()
    {
        ExporterOptions.FhirExportClassType.PrimitiveType,
        ExporterOptions.FhirExportClassType.ComplexType,
        ExporterOptions.FhirExportClassType.Resource,
    };

    /// <summary>
    /// Gets a list of FHIR class types that the language CAN export, depending on user choices.
    /// </summary>
    List<ExporterOptions.FhirExportClassType> ILanguage.OptionalExportClassTypes => new()
    {
        ExporterOptions.FhirExportClassType.Enum,
        ExporterOptions.FhirExportClassType.Profile,
    };

    /// <summary>Gets language-specific options and their descriptions.</summary>
    Dictionary<string, string> ILanguage.LanguageOptions => new()
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

        string modelDirectory = Path.Combine(exportDirectory, "fhir");
        if (!Directory.Exists(modelDirectory))
        {
            Directory.CreateDirectory(modelDirectory);
        }

        string jsonDirectory = Path.Combine(exportDirectory, "fhirJson");
        if (!Directory.Exists(jsonDirectory))
        {
            Directory.CreateDirectory(jsonDirectory);
        }

        string valueSetDirectory = Path.Combine(exportDirectory, "fhirValueSets");
        if (_exportEnums)
        {
            if (!Directory.Exists(valueSetDirectory))
            {
                Directory.CreateDirectory(valueSetDirectory);
            }
        }

        if (options.SupportFiles.StaticFiles.Any())
        {
            foreach (LanguageSupportFiles.SupportFileRec fileRec in options.SupportFiles.StaticFiles)
            {
                string dest = Path.Combine(exportDirectory, fileRec.RelativeFilename);
                string dir = Path.GetDirectoryName(dest);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                File.Copy(fileRec.Filename, dest);
            }
        }

        ModelBuilder tsBuilder = new ModelBuilder(info);
        ModelBuilder.ExportModels exports = tsBuilder.Build();

        foreach (ModelBuilder.ExportPrimitive primitive in exports.PrimitiveTypesByExportName.Values)
        {
            WritePrimitive(primitive, modelDirectory);
        }

        foreach (ModelBuilder.ExportComplex dataType in exports.ComplexDataTypesByExportName.Values)
        {
            WriteComplexJson(dataType, jsonDirectory, exports.ValueSetsByExportName);
            WriteComplex(dataType, modelDirectory);
        }

        foreach (ModelBuilder.ExportComplex resource in exports.ResourcesByExportName.Values)
        {
            WriteComplexJson(resource, jsonDirectory, exports.ValueSetsByExportName);
            WriteComplex(resource, modelDirectory);
        }

        WriteFhirJsonExportModule(exportDirectory, exports);
        WriteFhirExportModule(exportDirectory, exports);

        foreach (ModelBuilder.ExportValueSet vs in exports.ValueSetsByExportName.Values)
        {
            WriteValueSet(vs, valueSetDirectory);
        }

        WriteValueSetModule(exportDirectory, exports);

        WriteValueSetEnumModule(exportDirectory, exports);

        WriteIndexModule(exportDirectory);
    }

    /// <summary>Writes an index module.</summary>
    /// <param name="exportDirectory">Directory to write files.</param>
    private void WriteIndexModule(string exportDirectory)
    {
        ExportStringBuilder sb = new();

        WriteHeader(sb);

        sb.WriteLine(string.Empty);

        sb.WriteLineIndented("import * as fhirJson from './fhirJson.js';");
        sb.WriteLineIndented("import * as fhir from './fhir.js';");
        sb.WriteLineIndented("import * as valueSets from './valueSets.js';");
        sb.WriteLineIndented("import * as valueSetEnums from './valueSetEnums.js';");

        sb.WriteLineIndented("export { fhir, valueSets, valueSetEnums, fhirJson, };");

        string filename = Path.Combine(exportDirectory, $"index.ts");
        using (FileStream stream = new FileStream(filename, FileMode.Create))
        using (ExportStreamWriter writer = new ExportStreamWriter(stream))
        {
            writer.Write(sb);
        }
    }

    /// <summary>Writes a FHIR export module.</summary>
    /// <param name="exportDirectory">Directory to write files.</param>
    /// <param name="exports">        The exports.</param>
    private void WriteValueSetEnumModule(string exportDirectory, ModelBuilder.ExportModels exports)
    {
        ExportStringBuilder sb = new();

        WriteHeader(sb);

        sb.WriteLine(string.Empty);

        foreach (ModelBuilder.ExportValueSet vs in exports.ValueSetsByExportName.Values)
        {
            sb.WriteLineIndented(
                $"import {{" +
                $" {vs.ExportName}Enum," +
                $" }}" +
                $" from './fhirValueSets/{vs.ExportName}.js'");
        }

        sb.WriteLine(string.Empty);
        sb.OpenScope("export {");

        foreach (ModelBuilder.ExportValueSet vs in exports.ValueSetsByExportName.Values)
        {
            sb.WriteLineIndented($"{vs.ExportName}Enum,");
        }

        sb.CloseScope();

        string filename = Path.Combine(exportDirectory, $"valueSetEnums.ts");
        using (FileStream stream = new FileStream(filename, FileMode.Create))
        using (ExportStreamWriter writer = new ExportStreamWriter(stream))
        {
            writer.Write(sb);
        }
    }

    /// <summary>Writes a FHIR value setmodule.</summary>
    /// <param name="exportDirectory">Directory to write files.</param>
    /// <param name="exports">        The exports.</param>
    private void WriteValueSetModule(string exportDirectory, ModelBuilder.ExportModels exports)
    {
        ExportStringBuilder sb = new();

        WriteHeader(sb);

        sb.WriteLine(string.Empty);

        foreach (ModelBuilder.ExportValueSet vs in exports.ValueSetsByExportName.Values)
        {
            sb.WriteLineIndented(
                $"import {{" +
                $" {vs.ExportName}, {vs.ExportName}Type," +
                $" }}" +
                $" from './fhirValueSets/{vs.ExportName}.js'");
        }

        sb.WriteLine(string.Empty);
        sb.OpenScope("export {");

        foreach (ModelBuilder.ExportValueSet vs in exports.ValueSetsByExportName.Values)
        {
            sb.WriteLineIndented($"{vs.ExportName}, type {vs.ExportName}Type,");
        }

        sb.CloseScope();

        string filename = Path.Combine(exportDirectory, $"valueSets.ts");
        using (FileStream stream = new FileStream(filename, FileMode.Create))
        using (ExportStreamWriter writer = new ExportStreamWriter(stream))
        {
            writer.Write(sb);
        }
    }

    /// <summary>Writes a FHIR export module.</summary>
    /// <param name="exportDirectory">Directory to write files.</param>
    /// <param name="exports">        The exports.</param>
    private void WriteFhirExportModule(
        string exportDirectory,
        ModelBuilder.ExportModels exports)
    {
        ExportStringBuilder sb = new();

        WriteHeader(sb);

        sb.WriteLine(string.Empty);

        foreach (ModelBuilder.SortedExportKey exportKey in exports.SortedPrimitives)
        {
            sb.WriteLineIndented(
                $"import {{" +
                $" {string.Join(", ", exportKey.Tokens.Select((t) => t.Token))}" +
                $" }}" +
                $" from './fhir/{exportKey.ExportName}.js';");
        }

        foreach (ModelBuilder.SortedExportKey exportKey in exports.SortedDataTypes)
        {
            sb.WriteLineIndented(
                $"import {{" +
                $" {string.Join(", ", exportKey.Tokens.Select((t) => t.Token))}" +
                $" }}" +
                $" from './fhir/{exportKey.ExportName}.js';");
        }

        foreach (ModelBuilder.SortedExportKey exportKey in exports.SortedResources)
        {
            sb.WriteLineIndented(
                $"import {{" +
                $" {string.Join(", ", exportKey.Tokens.Select((t) => t.Token))}" +
                $" }}" +
                $" from './fhir/{exportKey.ExportName}.js';");
        }

        WriteExpandedResourceBindings(sb, exports);

        WriteFhirResourceFactory(sb, exports);

        WriteFhirConstructorPropsInterface(sb);

        if (_options.SupportFiles.TryGetInputForKey("fhir", out string contents))
        {
            sb.Write(contents);
        }

        sb.WriteLine(string.Empty);
        sb.OpenScope("export {");

        foreach (ModelBuilder.SortedExportKey exportKey in exports.SortedPrimitives)
        {
            sb.WriteLineIndented(string.Join(", ", exportKey.Tokens.Select((t) => t.requiresTypeLiteral ? "type " + t.Token : t.Token)) + ", ");
        }

        foreach (ModelBuilder.SortedExportKey exportKey in exports.SortedDataTypes)
        {
            sb.WriteLineIndented(string.Join(", ", exportKey.Tokens.Select((t) => t.requiresTypeLiteral ? "type " + t.Token : t.Token)) + ", ");
        }

        foreach (ModelBuilder.SortedExportKey exportKey in exports.SortedResources)
        {
            sb.WriteLineIndented(string.Join(", ", exportKey.Tokens.Select((t) => t.requiresTypeLiteral ? "type " + t.Token : t.Token)) + ", ");
        }

        sb.WriteLineIndented("type IFhirResource, type FhirResource, type FhirConstructorOptions, ");
        sb.WriteLineIndented("fhirToJson, ");
        sb.WriteLineIndented("resourceFactory, ");

        sb.CloseScope();

        string filename = Path.Combine(exportDirectory, $"fhir.ts");
        using (FileStream stream = new FileStream(filename, FileMode.Create))
        using (ExportStreamWriter writer = new ExportStreamWriter(stream))
        {
            writer.Write(sb);
        }
    }

    /// <summary>
    /// Writes a FHIR JSON export module.
    /// Note that the 'type literal' flag is inverted because in the JSON models we
    /// want to use the 'bare' export name, but export as an interface.
    /// </summary>
    /// <param name="exportDirectory">Directory to write files.</param>
    /// <param name="exports">        The exports.</param>
    private void WriteFhirJsonExportModule(
        string exportDirectory,
        ModelBuilder.ExportModels exports)
    {
        ExportStringBuilder sb = new();

        WriteHeader(sb);

        sb.WriteLine(string.Empty);

        foreach (ModelBuilder.SortedExportKey exportKey in exports.SortedDataTypes)
        {
            sb.WriteLineIndented(
                $"import {{" +
                $" {string.Join(", ", exportKey.Tokens.Where((t) => (!t.requiresTypeLiteral)).Select((t) => t.Token))}" +
                $" }}" +
                $" from './fhirJson/{exportKey.ExportName}.js';");
        }

        foreach (ModelBuilder.SortedExportKey exportKey in exports.SortedResources)
        {
            sb.WriteLineIndented(
                $"import {{" +
                $" {string.Join(", ", exportKey.Tokens.Where((t) => (!t.requiresTypeLiteral)).Select((t) => t.Token))}" +
                $" }}" +
                $" from './fhirJson/{exportKey.ExportName}.js';");
        }

        WriteExpandedJsonResourceBindings(sb, exports);

        if (_options.SupportFiles.TryGetInputForKey("fhirJson", out string contents))
        {
            sb.Write(contents);
        }

        sb.WriteLine(string.Empty);
        sb.OpenScope("export {");

        foreach (ModelBuilder.SortedExportKey exportKey in exports.SortedDataTypes)
        {
            sb.WriteLineIndented(string.Join(", ", exportKey.Tokens.Where((t) => (!t.requiresTypeLiteral)).Select((t) => "type " + t.Token)) + ", ");
        }

        foreach (ModelBuilder.SortedExportKey exportKey in exports.SortedResources)
        {
            sb.WriteLineIndented(string.Join(", ", exportKey.Tokens.Where((t) => (!t.requiresTypeLiteral)).Select((t) => "type " + t.Token)) + ", ");
        }

        sb.WriteLineIndented("type FhirResource, ");

        sb.CloseScope();

        string filename = Path.Combine(exportDirectory, $"fhirJson.ts");
        using (FileStream stream = new FileStream(filename, FileMode.Create))
        using (ExportStreamWriter writer = new ExportStreamWriter(stream))
        {
            writer.Write(sb);
        }
    }

    /// <summary>Writes a FHIR constructor properties interface.</summary>
    /// <param name="sb">The writer.</param>
    private void WriteFhirConstructorPropsInterface(ExportStringBuilder sb)
    {
        // interface open
        WriteIndentedComment(sb, "FHIR object constructor properties");
        sb.OpenScope("interface FhirConstructorOptions {");

        WriteIndentedComment(sb, "If objects should retain unknown elements.");
        sb.WriteLineIndented("allowUnknownElements?: boolean|undefined;");

        // interface close
        sb.CloseScope();
    }

    /// <summary>Writes a FHIR resource factory.</summary>
    /// <param name="sb">     The writer.</param>
    /// <param name="exports">The exports.</param>
    private void WriteFhirResourceFactory(
        ExportStringBuilder sb,
        ModelBuilder.ExportModels exports)
    {
        if (exports.ResourcesByExportName.Count == 0)
        {
            return;
        }

        // function open
        WriteIndentedComment(sb, "Factory creator for FHIR Resources");
        sb.OpenScope("function resourceFactory(source:any) : FhirResource|null {");

        // switch open
        sb.OpenScope("switch (source[\"resourceType\"]) {");

        foreach (ModelBuilder.ExportComplex complex in exports.ResourcesByExportName.Values)
        {
            sb.WriteLineIndented($"case \"{complex.FhirName}\": return new {complex.ExportClassName}(source);");
        }

        sb.WriteLineIndented("default: return null;");

        // switch close
        sb.CloseScope();

        // function close
        sb.CloseScope();
    }

    /// <summary>Writes the expanded resource interface binding.</summary>
    private void WriteExpandedResourceBindings(
        ExportStringBuilder sb,
        ModelBuilder.ExportModels exports)
    {
        if (exports.ResourcesByExportName.Count == 0)
        {
            return;
        }

        if (exports.ResourcesByExportName.Count == 1)
        {
            sb.WriteLine(string.Empty);
            WriteIndentedComment(sb, "Resource binding for generic use.");
            sb.WriteLineIndented($"type IFhirResource = {exports.ResourcesByExportName.Values.First().ExportInterfaceName};");

            sb.WriteLine(string.Empty);
            WriteIndentedComment(sb, "Resource binding for generic use.");
            sb.WriteLineIndented($"type FhirResource = {exports.ResourcesByExportName.Values.First().ExportClassName};");

            return;
        }

        string spacing = new string(sb.IndentationChar, sb.Indentation + 1);
        string delim = "\n" + spacing + "|";

        sb.WriteLine(string.Empty);
        WriteIndentedComment(sb, "Resource binding for generic use.");
        sb.WriteLineIndented($"type IFhirResource = ");
        sb.WriteLine(spacing + string.Join(delim, exports.ResourcesByExportName.Values.Select((complex) => complex.ExportInterfaceName)) + ";");

        sb.WriteLine(string.Empty);
        WriteIndentedComment(sb, "Resource binding for generic use.");
        sb.WriteLineIndented($"type FhirResource = ");
        sb.WriteLine(spacing + string.Join(delim, exports.ResourcesByExportName.Values.Select((complex) => complex.ExportClassName)) + ";");
    }

    /// <summary>Writes the expanded resource interface binding.</summary>
    private void WriteExpandedJsonResourceBindings(
        ExportStringBuilder sb,
        ModelBuilder.ExportModels exports)
    {
        if (exports.ResourcesByExportName.Count == 0)
        {
            return;
        }

        if (exports.ResourcesByExportName.Count == 1)
        {
            sb.WriteLine(string.Empty);
            WriteIndentedComment(sb, "Resource binding for generic use.");
            sb.WriteLineIndented($"type FhirResource = {exports.ResourcesByExportName.Values.First().ExportClassName};");

            return;
        }

        string spacing = new string(sb.IndentationChar, sb.Indentation + 1);
        string delim = "\n" + spacing + "|";

        sb.WriteLine(string.Empty);
        WriteIndentedComment(sb, "Resource binding for generic use.");
        sb.WriteLineIndented($"type FhirResource = ");
        sb.WriteLine(spacing + string.Join(delim, exports.ResourcesByExportName.Values.Select((complex) => complex.ExportClassName)) + ";");
    }

    /// <summary>Writes a value set.</summary>
    /// <param name="vs">         Set the value belongs to.</param>
    /// <param name="vsDirectory">Pathname of the vs directory.</param>
    private void WriteValueSet(
        ModelBuilder.ExportValueSet vs,
        string vsDirectory)
    {
        ExportStringBuilder sb = new();

        WriteHeader(sb);

        sb.WriteLine($"// FHIR ValueSet: {vs.FhirUrl}|{vs.FhirVersion}");
        sb.WriteLine(string.Empty);
        sb.WriteLineIndented("import { Coding } from '../fhir.js'");
        sb.WriteLine(string.Empty);

        if (!string.IsNullOrEmpty(vs.ExportComment))
        {
            WriteIndentedComment(sb, vs.ExportComment);
        }

        sb.OpenScope($"export const {vs.ExportName} = {{");

        foreach (ModelBuilder.ExportValueSetCoding coding in vs.CodingsByExportName.Values)
        {
            if (string.IsNullOrEmpty(coding.Comment))
            {
                WriteIndentedComment(sb, "Code: " + coding.Code);
            }
            else
            {
                WriteIndentedComment(sb, coding.Code + ": " + coding.Comment);
            }

            sb.OpenScope($"{coding.ExportName}: new Coding({{");

            if (!string.IsNullOrEmpty(coding.Display))
            {
                sb.WriteLineIndented($"display: \"{FhirUtils.SanitizeForQuoted(coding.Display)}\",");
            }

            sb.WriteLineIndented($"code: \"{coding.Code}\",");
            sb.WriteLineIndented($"system: \"{coding.System}\",");

            sb.CloseScope("}),");
        }

        sb.CloseScope("} as const;");

        sb.WriteLine(string.Empty);
        if (!string.IsNullOrEmpty(vs.ExportComment))
        {
            WriteIndentedComment(sb, vs.ExportComment);
        }

        sb.WriteLineIndented($"export type {vs.ExportName}Type = typeof {vs.ExportName};");

        sb.WriteLine(string.Empty);
        if (!string.IsNullOrEmpty(vs.ExportComment))
        {
            WriteIndentedComment(sb, vs.ExportComment);
        }

        sb.OpenScope($"export enum {vs.ExportName}Enum {{");

        foreach (ModelBuilder.ExportValueSetCoding coding in vs.CodingsByExportName.Values)
        {
            if (string.IsNullOrEmpty(coding.Comment))
            {
                WriteIndentedComment(sb, "Code: " + coding.Code);
            }
            else
            {
                WriteIndentedComment(sb, coding.Code + ": " + coding.Comment);
            }

            sb.WriteLineIndented($"{coding.ExportName} = \"{coding.Code}\",");
        }

        sb.CloseScope();

        string filename = Path.Combine(vsDirectory, vs.ExportName + ".ts");
        using (FileStream stream = new FileStream(filename, FileMode.Create))
        using (ExportStreamWriter writer = new ExportStreamWriter(stream))
        {
            writer.Write(sb);
        }
    }

    /// <summary>Writes a primitive.</summary>
    /// <param name="primitive">     The primitive.</param>
    /// <param name="modelDirectory">Pathname of the model directory.</param>
    private void WritePrimitive(
        ModelBuilder.ExportPrimitive primitive,
        string modelDirectory)
    {
        ExportStringBuilder sb = new();

        WriteHeader(sb);

        sb.WriteLine($"// FHIR Primitive: {primitive.FhirName}");
        sb.WriteLine(string.Empty);
        sb.WriteLineIndented("import * as fhir from '../fhir.js';");
        sb.WriteLine(string.Empty);

        sb.WriteLineIndented("import { IssueTypeValueSetEnum } from '../fhirValueSets/IssueTypeValueSet.js';");
        sb.WriteLineIndented("import { IssueSeverityValueSetEnum } from '../fhirValueSets/IssueSeverityValueSet.js';");

        BuildInterfaceForPrimitive(sb, primitive);

        BuildClassForPrimitive(sb, primitive);

        string filename = Path.Combine(modelDirectory, primitive.ExportClassName + ".ts");
        using (FileStream stream = new FileStream(filename, FileMode.Create))
        using (ExportStreamWriter writer = new ExportStreamWriter(stream))
        {
            writer.Write(sb);
        }
    }

    /// <summary>Builds interface for primitive.</summary>
    /// <param name="sb">       The writer.</param>
    /// <param name="primitive">The primitive.</param>
    private void BuildInterfaceForPrimitive(
        ExportStringBuilder sb,
        ModelBuilder.ExportPrimitive primitive)
    {
        sb.WriteLine(string.Empty);

        if (!string.IsNullOrEmpty(primitive.ExportComment))
        {
            WriteIndentedComment(sb, primitive.ExportComment);
        }

        if (string.IsNullOrEmpty(primitive.ExportInterfaceType))
        {
            sb.WriteLineIndented($"export type {primitive.ExportInterfaceName} = {{");
        }
        else
        {
            sb.WriteLineIndented($"export type {primitive.ExportInterfaceName} = {primitive.ExportInterfaceType} & {{ ");
        }

        sb.IncreaseIndent();

        WriteIndentedComment(sb, $"A {primitive.FhirName} value, represented as a JS {primitive.JsonExportType}");
        sb.WriteLineIndented($"value:{primitive.JsonExportType}|null;");

        if (_options.SupportFiles.TryGetInputForKey(primitive.ExportInterfaceName, out string contents))
        {
            sb.Write(contents);
        }

        sb.CloseScope();
    }

    /// <summary>Builds class for primitive.</summary>
    /// <param name="sb">       The writer.</param>
    /// <param name="primitive">The primitive.</param>
    private void BuildClassForPrimitive(
        ExportStringBuilder sb,
        ModelBuilder.ExportPrimitive primitive)
    {
        sb.WriteLine(string.Empty);

        if (!string.IsNullOrEmpty(primitive.ExportComment))
        {
            WriteIndentedComment(sb, primitive.ExportComment);
        }

        if (string.IsNullOrEmpty(primitive.ExportClassType))
        {
            sb.WriteLineIndented($"export class {primitive.ExportClassName} implements {primitive.ExportInterfaceName} {{");
        }
        else
        {
            sb.WriteLineIndented($"export class {primitive.ExportClassName} extends {primitive.ExportClassType} implements {primitive.ExportInterfaceName} {{");
        }

        sb.IncreaseIndent();

        WriteIndentedComment(sb, $"A {primitive.FhirName} value, represented as a JS {primitive.JsonExportType}");
        sb.WriteLineIndented($"value:{primitive.JsonExportType}|null;");

        BuildConstructor(sb, primitive);

        // add model validation function
        //BuildModelValidation(sb, complex);

        // add toJSON override
        //BuildToJson(sb, complex);

        if (_options.SupportFiles.TryGetInputForKey(primitive.ExportClassName, out string contents))
        {
            sb.Write(contents);
        }

        sb.CloseScope();
    }

    /// <summary>Builds a constructor.</summary>
    /// <param name="sb">       The writer.</param>
    /// <param name="primitive">The primitive.</param>
    private void BuildConstructor(
        ExportStringBuilder sb,
        ModelBuilder.ExportPrimitive primitive)
    {
        sb.OpenScope("/**");
        sb.WriteLineIndented($" * Create a {primitive.ExportClassName}");
        sb.WriteLineIndented($" * @param value {primitive.ExportComment}");
        sb.WriteLineIndented(" * @param id Unique id for inter-element referencing (uncommon on primitives)");
        sb.WriteLineIndented(" * @param extension Additional content defined by implementations");
        sb.WriteLineIndented(" * @param options Options to pass to extension constructors");
        sb.CloseScope("*/");

        // Constructor open

        //WriteIndentedComment(
        //    sb,
        //    $"Default constructor for {primitive.ExportClassName}");

        //sb.OpenScope($"constructor(source:Partial<{primitive.ExportInterfaceName}> = {{ }}, options:fhir.FhirConstructorOptions = {{ }}) {{");

        sb.OpenScope(
            $"constructor" +
            $"(value?:{primitive.JsonExportType}|null," +
            $" id?:string," +
            $" extension?:(fhir.Extension|null)[]," +
            $" options:fhir.FhirConstructorOptions = {{ }}" +
            $") {{");

        if (!string.IsNullOrEmpty(primitive.ExportClassType))
        {
            sb.WriteLineIndented("super(value, id, extension, options);");
        }
        //else
        //{
        //    sb.WriteLineIndented("if (options.allowUnknownElements === true) { Object.assign(this, source); }");
        //}

        sb.CloseScope();
    }

    /// <summary>Writes a complex.</summary>
    /// <param name="complex">          The complex.</param>
    /// <param name="modelDirectory">   Pathname of the model directory.</param>
    private void WriteComplex(
        ModelBuilder.ExportComplex complex,
        string modelDirectory)
    {
        ExportStringBuilder sb = new();

        WriteHeader(sb);

        sb.WriteLine($"// FHIR {complex.ArtifactClass}: {complex.FhirName}");
        sb.WriteLine(string.Empty);
        sb.WriteLineIndented("import * as fhir from '../fhir.js';");
        sb.WriteLine(string.Empty);

        foreach (string valueSetExportName in complex.ReferencedValueSetExportNames)
        {
            sb.WriteLineIndented(
                $"import {{" +
                $" {valueSetExportName}," +
                $" {valueSetExportName}Type," +
                $" {valueSetExportName}Enum " +
                $"}} from '../fhirValueSets/{valueSetExportName}.js';");
        }

        if (!complex.ReferencedValueSetExportNames.Contains("IssueTypeValueSet"))
        {
            sb.WriteLineIndented("import { IssueTypeValueSetEnum } from '../fhirValueSets/IssueTypeValueSet.js';");
        }

        if (!complex.ReferencedValueSetExportNames.Contains("IssueSeverityValueSet"))
        {
            sb.WriteLineIndented("import { IssueSeverityValueSetEnum } from '../fhirValueSets/IssueSeverityValueSet.js';");
        }

        BuildInterfaceForComplex(sb, complex);

        BuildClassForComplex(sb, complex);

        string filename = Path.Combine(modelDirectory, complex.ExportClassName + ".ts");
        using (FileStream stream = new FileStream(filename, FileMode.Create))
        using (ExportStreamWriter writer = new ExportStreamWriter(stream))
        {
            writer.Write(sb);
        }
    }

    /// <summary>Writes a complex class.</summary>
    /// <param name="sb">     The writer.</param>
    /// <param name="complex">The complex.</param>
    private void BuildClassForComplex(
        ExportStringBuilder sb,
        ModelBuilder.ExportComplex complex)
    {
        // recurse
        if (complex.Backbones.Any())
        {
            foreach (ModelBuilder.ExportComplex backbone in complex.Backbones)
            {
                // use unknown for backbones to prevent resource/datatype specific fields
                BuildClassForComplex(sb, backbone);
            }
        }

        sb.WriteLine(string.Empty);

        if (!string.IsNullOrEmpty(complex.ExportComment))
        {
            WriteIndentedComment(sb, complex.ExportComment);
        }

        if (string.IsNullOrEmpty(complex.ExportType))
        {
            sb.WriteLineIndented($"export class {complex.ExportClassName} implements {complex.ExportInterfaceName} {{");
        }
        else
        {
            sb.WriteLineIndented($"export class {complex.ExportClassName} extends {complex.ExportType} implements {complex.ExportInterfaceName} {{");
        }

        sb.IncreaseIndent();

        // add actual elements
        foreach (ModelBuilder.ExportElement element in complex.Elements)
        {
            BuildComplexElement(sb, element, false);
        }

        BuildConstructor(sb, complex);

        // add functions to get Value-Set hints for elements with bound value sets
        foreach (ModelBuilder.ExportElement element in complex.Elements)
        {
            if ((!element.HasReferencedValueSet) ||
                (element.BoundValueSetStrength == null))
            {
                continue;
            }

            WriteIndentedComment(sb, $"{element.BoundValueSetStrength}-bound Value Set for {element.ExportName}");
            sb.OpenScope($"public static {element.ExportName}{element.BoundValueSetStrength}ValueSet():{element.ValueSetExportName}Type {{");
            sb.WriteLineIndented($"return {element.ValueSetExportName};");
            sb.CloseScope();
        }

        // add model validation function
        BuildModelValidation(sb, complex);

        // add toJSON override
        BuildToJson(sb, complex);

        if (_options.SupportFiles.TryGetInputForKey(complex.ExportClassName, out string contents))
        {
            sb.Write(contents);
        }

        sb.CloseScope();
    }

    private void BuildToJson(
        ExportStringBuilder sb,
        ModelBuilder.ExportComplex complex)
    {
        // function open
        WriteIndentedComment(sb, "Function to strip invalid element values for serialization.");

        sb.OpenScope("public toJSON() {");

        //if (string.IsNullOrEmpty(complex.ExportType))
        //{
        //    sb.OpenScope("public toJSON() {");
        //}
        //else
        //{
        //    sb.OpenScope("public override toJSON() {");
        //}

        sb.WriteLineIndented("return fhir.fhirToJson(this);");

        // function close
        sb.CloseScope();
    }

    /// <summary>Builds model validation.</summary>
    /// <param name="sb">     The writer.</param>
    /// <param name="complex">The complex.</param>
    private void BuildModelValidation(
        ExportStringBuilder sb,
        ModelBuilder.ExportComplex complex)
    {
        // function open
        WriteIndentedComment(sb, "Function to perform basic model validation (e.g., check if required elements are present).");

        if (string.IsNullOrEmpty(complex.ExportType))
        {
            sb.OpenScope("public doModelValidation():fhir.OperationOutcome {");
            sb.WriteLineIndented("var outcome:fhir.OperationOutcome = new fhir.OperationOutcome({issue:[]});");
        }
        else
        {
            sb.OpenScope("public override doModelValidation():fhir.OperationOutcome {");
            sb.WriteLineIndented("var outcome:fhir.OperationOutcome = super.doModelValidation();");
        }

        foreach (ModelBuilder.ExportElement element in complex.Elements)
        {
            if (!element.IsOptional)
            {
                AddElementModelChecks(sb, element);
            }

            // recurse into other FHIR types
            if (element.ExportType.StartsWith("fhir", StringComparison.Ordinal))
            {
                if (element.IsArray)
                {
                    sb.WriteLineIndented(
                        $"if (this[\"{element.ExportName}\"])" +
                        $" {{" +
                        $" this.{element.ExportName}.forEach((x) =>" +
                        $" {{" +
                        $" outcome.issue!.push(...x.doModelValidation().issue!);" +
                        $" }})" +
                        $" }}");
                }
                else
                {
                    sb.WriteLineIndented(
                        $"if (this[\"{element.ExportName}\"])" +
                        $" {{ outcome.issue!.push(...this.{element.ExportName}.doModelValidation().issue!); }}");
                }
            }
        }

        sb.WriteLineIndented("return outcome;");

        // function close
        sb.CloseScope();
    }

    /// <summary>Gets operation outcome issue.</summary>
    /// <param name="issueSeverity">The issue severity.</param>
    /// <param name="issueType">    Type of the issue.</param>
    /// <param name="message">      The message.</param>
    /// <returns>The operation outcome issue.</returns>
    private string BuildOperationOutcomeIssue(string issueSeverity, string issueType, string message)
    {
        return
            $"new fhir.OperationOutcomeIssue({{" +
            $" severity: {issueSeverity}," +
            $" code: {issueType}, " +
            $" diagnostics: \"{SanitizeForTsQuoted(message) ?? string.Empty}\"," +
            $" }})";
    }

    /// <summary>Adds an element optional array to 'element'.</summary>
    /// <param name="sb">     The writer.</param>
    /// <param name="element">The element.</param>
    private void AddElementModelChecks(ExportStringBuilder sb, ModelBuilder.ExportElement element)
    {
        string propertyInfo =
            $"property" +
            $" {element.ExportName}{(element.IsOptional ? "?" : string.Empty)}" +
            $":{element.ExportType}{(element.IsArray ? "[]" : string.Empty)}" +
            $" fhir:" +
            $" {element.FhirPath}" +
            $":{element.FhirType}";

        string missing = BuildOperationOutcomeIssue(
            TsOutcomeIssueSeverity.Error,
            TsOutcomeIssueType.RequiredElementMissing,
            $"Missing required {propertyInfo}");

        string notArray = BuildOperationOutcomeIssue(
            TsOutcomeIssueSeverity.Error,
            TsOutcomeIssueType.StructuralIssue,
            $"Found scalar in array {propertyInfo}");

        if (element.IsArray)
        {
            sb.OpenScope($"if (!this['{element.ExportName}']) {{");
            sb.WriteLineIndented($"outcome.issue!.push({missing});");
            sb.ReopenScope($"}} else if (!Array.isArray(this.{element.ExportName})) {{");
            sb.WriteLineIndented($"outcome.issue!.push({notArray});");
            sb.ReopenScope($"}} else if (this.{element.ExportName}.length === 0) {{");
            sb.WriteLineIndented($"outcome.issue!.push({missing});");
            sb.CloseScope();
        }
        else
        {
            // TODO: add checks for primitive types
            sb.OpenScope($"if (!this['{element.ExportName}']) {{");
            sb.WriteLineIndented($"outcome.issue!.push({missing});");
            sb.CloseScope();
        }
    }

    /// <summary>Builds a constructor.</summary>
    /// <param name="sb">     The writer.</param>
    /// <param name="complex">The complex.</param>
    private void BuildConstructor(
        ExportStringBuilder sb,
        ModelBuilder.ExportComplex complex)
    {
        // Constructor open
        WriteIndentedComment(
            sb,
            $"Default constructor for {complex.ExportClassName} - initializes any required elements to null if a value is not provided.");

        sb.OpenScope($"constructor(source:Partial<{complex.ExportInterfaceName}> = {{ }}, options:fhir.FhirConstructorOptions = {{ }}) {{");

        if (!string.IsNullOrEmpty(complex.ExportType))
        {
            sb.WriteLineIndented("super(source, options);");
        }
        else
        {
            sb.WriteLineIndented("if (options.allowUnknownElements === true) { Object.assign(this, source); }");
        }

        foreach (ModelBuilder.ExportElement element in complex.Elements)
        {
            if (element.ExportName == "resourceType")
            {
                sb.WriteLineIndented($"this.resourceType = '{complex.FhirName}';");
                continue;
            }

            if (element.IsArray)
            {
                if (element.FhirType == "Resource")
                {
                    sb.OpenScope($"if (source['{element.ExportName}']) {{");
                    sb.WriteLineIndented($"this.{element.ExportName} = [];");
                    sb.OpenScope($"source.{element.ExportName}.forEach((x) => {{");
                    sb.WriteLineIndented("var r = fhir.resourceFactory(x);");
                    sb.WriteLineIndented($"if (r) {{ this.{element.ExportName}!.push(r); }}");
                    sb.CloseScope("});");
                    sb.CloseScope();
                }
                else if (element.ExportType.StartsWith("fhir"))
                {
                    sb.WriteLineIndented(
                        $"if (source['{element.ExportName}'])" +
                        $" {{" +
                        $" this.{element.ExportName} = source.{element.ExportName}.map(" +
                        $"(x) => new {element.ExportType}(x));" +
                        $" }}");
                }
                else
                {
                    sb.WriteLineIndented(
                        $"if (source['{element.ExportName}'])" +
                        $" {{" +
                        $" this.{element.ExportName} = source.{element.ExportName}.map(" +
                        $"(x) => (x));" +
                        $" }}");
                }
            }
            else
            {
                if (element.FhirType == "Resource")
                {
                    sb.WriteLineIndented(
                        $"if (source['{element.ExportName}'])" +
                        $" {{" +
                        $" this.{element.ExportName} = (fhir.resourceFactory(source.{element.ExportName}) ?? undefined);" +
                        $" }}");
                }
                else if (element.ExportType.StartsWith("fhir"))
                {
                    sb.WriteLineIndented(
                        $"if (source['{element.ExportName}'])" +
                        $" {{" +
                        $" this.{element.ExportName} = new {element.ExportType}(source.{element.ExportName}!);" +
                        $" }}");
                }
                else
                {
                    sb.WriteLineIndented(
                        $"if (source['{element.ExportName}'])" +
                        $" {{" +
                        $" this.{element.ExportName} = source.{element.ExportName};" +
                        $" }}");
                }
            }

            if (!element.IsOptional)
            {
                sb.WriteLineIndented($"else {{ this.{element.ExportName} = null; }}");
            }
        }

        sb.CloseScope();
    }

    /// <summary>Writes a complex interface.</summary>
    /// <param name="sb">     The writer.</param>
    /// <param name="complex">The complex.</param>
    private void BuildInterfaceForComplex(
        ExportStringBuilder sb,
        ModelBuilder.ExportComplex complex)
    {
        // recurse
        if (complex.Backbones.Any())
        {
            foreach (ModelBuilder.ExportComplex backbone in complex.Backbones)
            {
                // use unknown for backbones to prevent resource/datatype specific fields
                BuildInterfaceForComplex(sb, backbone);
            }
        }

        sb.WriteLine(string.Empty);

        if (!string.IsNullOrEmpty(complex.ExportComment))
        {
            WriteIndentedComment(sb, complex.ExportComment);
        }

        if (string.IsNullOrEmpty(complex.ExportInterfaceType))
        {
            sb.WriteLineIndented($"export type {complex.ExportInterfaceName} = {{");
        }
        else
        {
            sb.WriteLineIndented($"export type {complex.ExportInterfaceName} = {complex.ExportInterfaceType} & {{ ");
        }

        sb.IncreaseIndent();

        foreach (ModelBuilder.ExportElement element in complex.Elements)
        {
            BuildComplexElement(sb, element, true);
        }

        if (_options.SupportFiles.TryGetInputForKey(complex.ExportInterfaceName, out string contents))
        {
            sb.Write(contents);
        }

        sb.CloseScope();
    }

    /// <summary>Builds content for a complex element.</summary>
    /// <param name="sb">      The writer.</param>
    /// <param name="element"> The element.</param>
    private void BuildComplexElement(
        ExportStringBuilder sb,
        ModelBuilder.ExportElement element,
        bool isInterface)
    {
        if (!string.IsNullOrEmpty(element.ExportComment))
        {
            WriteIndentedComment(sb, element.ExportComment);
        }

        string optionalFlag = element.IsOptional ? "?" : string.Empty;
        string arrayFlag = element.IsArray ? "[]" : string.Empty;
        string typeAddition;

        if (element.IsOptional)
        {
            typeAddition = "|undefined";
        }
        else if (element.ExportInterfaceType.Contains('"'))
        {
            typeAddition = string.Empty;
        }
        else
        {
            typeAddition = "|null";
        }

        if (isInterface)
        {
            sb.WriteLineIndented($"{element.ExportName}{optionalFlag}: {element.ExportInterfaceType}{arrayFlag}{typeAddition};");
        }
        else
        {
            sb.WriteLineIndented($"public {element.ExportName}{optionalFlag}: {element.ExportType}{arrayFlag}{typeAddition};");
        }
    }

    /// <summary>Writes a complex.</summary>
    /// <param name="complex">      The complex.</param>
    /// <param name="jsonDirectory">Pathname of the json model directory.</param>
    private void WriteComplexJson(
        ModelBuilder.ExportComplex complex,
        string jsonDirectory,
        Dictionary<string, ModelBuilder.ExportValueSet> ValueSetsByExportName)
    {
        ExportStringBuilder sb = new();

        WriteHeader(sb);

        sb.WriteLine($"// FHIR {complex.ArtifactClass}: {complex.FhirName}");
        sb.WriteLine(string.Empty);
        sb.WriteLineIndented("import * as fhir from '../fhirJson.js';");
        sb.WriteLine(string.Empty);

        BuildInterfaceForComplexJson(sb, complex, ValueSetsByExportName);

        string filename = Path.Combine(jsonDirectory, complex.ExportClassName + ".ts");
        using (FileStream stream = new FileStream(filename, FileMode.Create))
        using (ExportStreamWriter writer = new ExportStreamWriter(stream))
        {
            writer.Write(sb);
        }
    }

    /// <summary>Writes a complex interface.</summary>
    /// <param name="sb">     The writer.</param>
    /// <param name="complex">The complex.</param>
    private void BuildInterfaceForComplexJson(
        ExportStringBuilder sb,
        ModelBuilder.ExportComplex complex,
        Dictionary<string, ModelBuilder.ExportValueSet> ValueSetsByExportName)
    {
        // recurse
        if (complex.Backbones.Any())
        {
            foreach (ModelBuilder.ExportComplex backbone in complex.Backbones)
            {
                // use unknown for backbones to prevent resource/datatype specific fields
                BuildInterfaceForComplexJson(sb, backbone, ValueSetsByExportName);
            }
        }

        sb.WriteLine(string.Empty);

        if (!string.IsNullOrEmpty(complex.ExportComment))
        {
            WriteIndentedComment(sb, complex.ExportComment);
        }

        if (string.IsNullOrEmpty(complex.ExportType))
        {
            sb.WriteLineIndented($"export interface {complex.ExportClassName} {{");
        }
        else
        {
            sb.WriteLineIndented($"export interface {complex.ExportClassName} extends {complex.ExportType} {{ ");
        }

        sb.IncreaseIndent();

        foreach (ModelBuilder.ExportElement element in complex.Elements)
        {
            BuildComplexElementJson(sb, element, ValueSetsByExportName);
        }

        sb.CloseScope();
    }

    /// <summary>Builds content for a complex element.</summary>
    /// <param name="sb">      The writer.</param>
    /// <param name="element"> The element.</param>
    private void BuildComplexElementJson(
        ExportStringBuilder sb,
        ModelBuilder.ExportElement element,
        Dictionary<string, ModelBuilder.ExportValueSet> ValueSetsByExportName)
    {
        if (!string.IsNullOrEmpty(element.ExportComment))
        {
            WriteIndentedComment(sb, element.ExportComment);
        }

        string optionalFlag = element.IsOptional ? "?" : string.Empty;
        string arrayFlag = element.IsArray ? "[]" : string.Empty;
        string typeAddition;

        if (element.IsOptional)
        {
            typeAddition = "|undefined";
        }
        else if (element.ExportJsonType.Contains('"'))
        {
            typeAddition = string.Empty;
        }
        else
        {
            typeAddition = "|null";
        }

        string exportType;

        if (element.ExportJsonType.EndsWith("Enum", StringComparison.Ordinal))
        {
            if (ValueSetsByExportName.ContainsKey(element.ValueSetExportName))
            {
                exportType = string.Join(
                    "|",
                    ValueSetsByExportName[element.ValueSetExportName].CodingsByExportName.Values.Select((c) => "'" + c.Code + "'"));

                if (element.IsArray)
                {
                    exportType = "(" + exportType + "|null)";
                }
            }
            else
            {
                exportType = "string";
            }
        }
        else
        {
            exportType = element.ExportJsonType;
        }

        sb.WriteLineIndented($"{element.ExportName}{optionalFlag}: {exportType}{arrayFlag}{typeAddition};");

        if (element.RequiresExtensionElement)
        {
            WriteIndentedComment(sb, $"Extended properties for primitive element: {element.FhirPath}");

            if (element.IsArray)
            {
                sb.WriteLineIndented($"_{element.ExportName}?:(fhir.{ComplexTypeSubstitutions["Element"]}|null)[];");
            }
            else
            {
                sb.WriteLineIndented($"_{element.ExportName}?:fhir.{ComplexTypeSubstitutions["Element"]};");
            }
        }
    }

    /// <summary>Sanitize for ts quoted.</summary>
    /// <param name="input">The input.</param>
    /// <returns>A string.</returns>
    private static string SanitizeForTsQuoted(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        input = input.Replace("\"", "'");
        input = input.Replace("\r", "\\r");
        input = input.Replace("\n", "\\n");

        return input;
    }

    /// <summary>Writes a header.</summary>
    /// <param name="sb">The writer.</param>
    private void WriteHeader(ExportStringBuilder sb)
    {
        sb.WriteLineIndented("// <auto-generated/>");
        sb.WriteLineIndented($"// Contents of: {_info.PackageName} version: {_info.VersionString}");

        if ((_options.ExportList != null) && _options.ExportList.Any())
        {
            string restrictions = string.Join("|", _options.ExportList);
            sb.WriteLineIndented($"  // Restricted to: {restrictions}");
        }

        if ((_options.LanguageOptions != null) && (_options.LanguageOptions.Count > 0))
        {
            foreach (KeyValuePair<string, string> kvp in _options.LanguageOptions)
            {
                sb.WriteLineIndented($"  // Language option: \"{kvp.Key}\" = \"{kvp.Value}\"");
            }
        }

        sb.WriteLine($"// Minimum TypeScript Version: {_minimumTypeScriptVersion}");
    }
}
