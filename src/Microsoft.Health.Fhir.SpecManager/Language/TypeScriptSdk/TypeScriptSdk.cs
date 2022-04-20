// <copyright file="TypeScriptSdk.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.IO;
using Microsoft.Health.Fhir.SpecManager.Manager;
using Microsoft.Health.Fhir.SpecManager.Models;

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

    /// <summary>Dictionary mapping FHIR primitive types to language equivalents.</summary>
    private static readonly Dictionary<string, string> _primitiveTypeMap = new()
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
                File.Copy(fileRec.Filename, Path.Combine(exportDirectory, fileRec.RelativeFilename));
            }
        }

        ModelBuilder tsBuilder = new ModelBuilder(info);
        ModelBuilder.ExportModels exports = tsBuilder.Build();

        foreach (ModelBuilder.ExportComplex dataType in exports.ComplexDataTypesByExportName.Values)
        {
            WriteComplex(dataType, modelDirectory);
        }

        foreach (ModelBuilder.ExportComplex resource in exports.ResourcesByExportName.Values)
        {
            WriteComplex(resource, modelDirectory);
        }

        WriteFhirExportModule(exportDirectory, exports);

        foreach (ModelBuilder.ExportValueSet vs in exports.ValueSetsByExportName.Values)
        {
            WriteValueSet(vs, valueSetDirectory);
        }
    }

    /// <summary>Writes a FHIR export module.</summary>
    /// <param name="exportDirectory">Directory to write files.</param>
    /// <param name="exports">        The exports.</param>
    private void WriteFhirExportModule(string exportDirectory, ModelBuilder.ExportModels exports)
    {
        ExportStringBuilder sb = new();

        WriteHeader(sb);

        sb.WriteLine(string.Empty);

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

        sb.WriteLine(string.Empty);
        sb.OpenScope("export {");

        foreach (ModelBuilder.SortedExportKey exportKey in exports.SortedDataTypes)
        {
            sb.WriteLineIndented(string.Join(", ", exportKey.Tokens.Select((t) => t.requiresTypeLiteral ? "type " + t.Token : t.Token)) + ", ");
        }

        foreach (ModelBuilder.SortedExportKey exportKey in exports.SortedResources)
        {
            sb.WriteLineIndented(string.Join(", ", exportKey.Tokens.Select((t) => t.requiresTypeLiteral ? "type " + t.Token : t.Token)) + ", ");
        }

        sb.WriteLineIndented("IFhirResource, FhirResource, ");

        sb.CloseScope();

        string filename = Path.Combine(exportDirectory, $"fhir.ts");
        using (FileStream stream = new FileStream(filename, FileMode.Create))
        using (ExportStreamWriter writer = new ExportStreamWriter(stream))
        {
            writer.Write(sb);
        }
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

    /// <summary>Writes a value set.</summary>
    /// <param name="vs">         Set the value belongs to.</param>
    /// <param name="vsDirectory">Pathname of the vs directory.</param>
    private void WriteValueSet(
        ModelBuilder.ExportValueSet vs,
        string vsDirectory)
    {
        ExportStringBuilder sb = new();

        WriteHeader(sb);

        sb.WriteLineIndented("import { Coding } from '../fhir.js'");

        sb.WriteLine(string.Empty);

        if (!string.IsNullOrEmpty(vs.ExportComment))
        {
            WriteIndentedComment(sb, vs.ExportComment);
        }

        sb.OpenScope($"export const {vs.ExportName} = {{");

        foreach (ModelBuilder.ExportValueSetCoding coding in vs.CodingsByExportName.Values)
        {
            if (!string.IsNullOrEmpty(coding.Comment))
            {
                WriteIndentedComment(sb, coding.Comment);
            }

            sb.OpenScope($"{coding.ExportName}: new Coding({{");

            if (!string.IsNullOrEmpty(coding.Display))
            {
                sb.WriteLineIndented($"display: \"{coding.Display}\",");
            }

            sb.WriteLineIndented($"code: \"{coding.Code}\",");
            sb.WriteLineIndented($"system: \"{coding.System}\",");

            sb.CloseScope("}),");
        }

        sb.CloseScope("};");

        string filename = Path.Combine(vsDirectory, vs.ExportName + ".ts");
        using (FileStream stream = new FileStream(filename, FileMode.Create))
        using (ExportStreamWriter writer = new ExportStreamWriter(stream))
        {
            writer.Write(sb);
        }
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

        sb.WriteLineIndented("import * as fhir from '../fhir.js'");

        BuildInterfaceForComplex(sb, complex);

        BuildClassForComplex(sb, complex);

        BuildCodesForComplex(sb, complex);

        string filename = Path.Combine(modelDirectory, complex.ExportClassName + ".ts");
        using (FileStream stream = new FileStream(filename, FileMode.Create))
        using (ExportStreamWriter writer = new ExportStreamWriter(stream))
        {
            writer.Write(sb);
        }
    }

    /// <summary>Builds codes for complex.</summary>
    /// <param name="sb">     The writer.</param>
    /// <param name="complex">The complex.</param>
    private void BuildCodesForComplex(
        ExportStringBuilder sb,
        ModelBuilder.ExportComplex complex)
    {
        // recurse
        if (complex.Backbones.Any())
        {
            foreach (ModelBuilder.ExportComplex backbone in complex.Backbones)
            {
                BuildCodesForComplex(sb, backbone);
            }
        }

        foreach (ModelBuilder.ExportCodeEnum exportCode in complex.CodesByExportName.Values)
        {
            sb.WriteLine(string.Empty);
            WriteIndentedComment(sb, $"Code Values for the {exportCode.FhirSourcePath} field");

            sb.WriteLineIndented($"export enum {exportCode.ExportName} {{");
            sb.IncreaseIndent();

            foreach (ModelBuilder.ExportCodeEnumValue value in exportCode.CodeValues)
            {
                if (!string.IsNullOrEmpty(value.Comment))
                {
                    WriteIndentedComment(sb, value.Comment);
                }

                sb.WriteLineIndented($"{value.CodeName} = \"{value.CodeValue}\",");
            }

            sb.CloseScope();
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

        foreach (ModelBuilder.ExportElement element in complex.Elements)
        {
            BuildComplexElement(sb, element, "public");
        }

        sb.WriteLineIndented($"constructor({{ }}) {{ }}");

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
            BuildComplexElement(sb, element);
        }

        sb.CloseScope();
    }

    /// <summary>Builds content for a complex element.</summary>
    /// <param name="sb">      The writer.</param>
    /// <param name="element"> The element.</param>
    /// <param name="accessModifier">(Optional) The access modifier.</param>
    private void BuildComplexElement(
        ExportStringBuilder sb,
        ModelBuilder.ExportElement element,
        string accessModifier = "")
    {
        if (!string.IsNullOrEmpty(element.ExportComment))
        {
            WriteIndentedComment(sb, element.ExportComment);
        }

        string optionalFlag = element.isOptional ? "?" : string.Empty;
        string arrayFlag = element.isArray ? "[]" : string.Empty;
        string typeAddition;

        if (element.isOptional)
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

        if (string.IsNullOrEmpty(accessModifier))
        {
            sb.WriteLineIndented($"{element.ExportName}{optionalFlag}: {element.ExportInterfaceType}{arrayFlag}{typeAddition};");
        }
        else
        {
            sb.WriteLineIndented($"{accessModifier} {element.ExportName}{optionalFlag}: {element.ExportInterfaceType}{arrayFlag}{typeAddition};");
        }

    }

    /// <summary>Writes an indented comment.</summary>
    /// <param name="sb">   The writer.</param>
    /// <param name="value">The value.</param>
    private void WriteIndentedComment(ExportStringBuilder sb, string value)
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
