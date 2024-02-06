// <copyright file="LangInfo.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.CommandLine;
using Hl7.Fhir.Model;
using Hl7.FhirPath.Sprache;
using Microsoft.Health.Fhir.CodeGen.Configuration;
using Microsoft.Health.Fhir.CodeGen.Extensions;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using static Microsoft.Health.Fhir.CodeGen.Lanugage.Info.LangInfo;
using static Microsoft.Health.Fhir.CodeGenCommon.Extensions.FhirNameConventionExtensions;

namespace Microsoft.Health.Fhir.CodeGen.Lanugage.Info;

/// <summary>Class used to export package/specification information.</summary>
public class LangInfo : ILanguage<InfoOptions>
{
    /// <summary>Values that represent Information formats.</summary>
    public enum InfoFormat
    {
        Text,
        Json,
    }

    /// <summary>An information options.</summary>
    public class InfoOptions : ConfigGenerate
    {
        /// <summary>Gets or sets the file format.</summary>
        [ConfigOption(
            ArgName = "--format",
            Description = "File format to export.")]
        public LangInfo.InfoFormat FileFormat { get; set; } = LangInfo.InfoFormat.Text;
    }

    /// <summary>Gets the language name.</summary>
    public string Name => "Info";

    /// <summary>Dictionary mapping FHIR primitive types to language equivalents.</summary>
    private static readonly Dictionary<string, string> _primitiveTypeMap = new()
        {
            { "base", "base" },
            { "base64Binary", "base64Binary" },
            { "boolean", "boolean" },
            { "canonical", "canonical" },
            { "code", "code" },
            { "date", "date" },
            { "dateTime", "dateTime" },
            { "decimal", "decimal" },
            { "id", "id" },
            { "instant", "instant" },
            { "integer", "integer" },
            { "integer64", "integer64" },
            { "markdown", "markdown" },
            { "oid", "oid" },
            { "positiveInt", "positiveInt" },
            { "string", "string" },
            { "time", "time" },
            { "unsignedInt", "unsignedInt" },
            { "uri", "uri" },
            { "url", "url" },
            { "uuid", "uuid" },
            { "xhtml", "xhtml" },
        };

    /// <summary>Gets the FHIR primitive type map.</summary>
    public Dictionary<string, string> FhirPrimitiveTypeMap => _primitiveTypeMap;

    /// <summary>The currently in-use text writer.</summary>
    private ExportStreamWriter _writer = null!;

    /// <summary>Exports the given configuration.</summary>
    /// <param name="config">     The configuration.</param>
    /// <param name="definitions">The definitions to export.</param>
    /// <param name="writeStream">(Optional) Stream to write data to.</param>
    public void Export(
        InfoOptions config,
        DefinitionCollection definitions,
        Stream? writeStream = null)
    {
        // TODO(ginoc): actually open the file
        // create a filename for writing (single file for now)
        //string filename = Path.Combine(config.OutputDirectory, $"Info_{definitions.FhirSequence.ToRLiteral()}.txt");
        //using (FileStream stream = new(filename, FileMode.Create))
        using (writeStream == null
            ? _writer = new ExportStreamWriter(Console.OpenStandardOutput(), System.Text.Encoding.UTF8, 1024, true)
            : _writer = new ExportStreamWriter(writeStream, System.Text.Encoding.UTF8, 1024, true))
        {
            WriteHeader(config, definitions);
            
            WritePrimitives(definitions.PrimitiveTypesByName);
        }

    }

    private void WritePrimitives(IReadOnlyDictionary<string, StructureDefinition> primitives)
    {
        _writer.WriteLineIndented($"Primitive Types: {primitives.Count()}");

        // traverse primitives
        foreach (StructureDefinition sd in primitives.Values.OrderBy(s => s.Id))
        {
            WritePrimitive(sd);
        }
    }

    private void WritePrimitive(StructureDefinition sd)
    {
        IGenPrimitive? primitive = sd.AsPrimitive();

        if (primitive == null)
        {
            throw new Exception($"Failed to process {sd.Id} ({sd.Name}) as a primitive!");
        }

        string snip = BuildStandardSnippet(primitive.StandardStatus, primitive.MaturityLevel, primitive.IsExperimental);

        _writer.WriteLineIndented(
            $"- {primitive.Name}:" +
                $" {primitive.Name.ToCamelCase()}" +
                $"::{primitive.TypeForExport(NamingConvention.CamelCase, _primitiveTypeMap)}" +
                $"{snip}");

        _writer.IncreaseIndent();

        // check for regex
        if (!string.IsNullOrEmpty(primitive.ValidationRegEx))
        {
            _writer.WriteLineIndented($"[{primitive.ValidationRegEx}]");
        }

        //if (_info.ExtensionsByPath.ContainsKey(primitive.Id))
        //{
        //    WriteExtensions(_info.ExtensionsByPath[primitive.Id].Values);
        //}

        //if (_info.ProfilesByBaseType.ContainsKey(primitive.Id))
        //{
        //    WriteProfiles(_info.ProfilesByBaseType[primitive.Id].Values);
        //}

        _writer.DecreaseIndent();

    }

    /// <summary>Builds standard snippet.</summary>
    /// <param name="standardStatus">The standard status.</param>
    /// <param name="fmmLevel">      The fmm level.</param>
    /// <returns>A string.</returns>
    private static string BuildStandardSnippet(string standardStatus, int? fmmLevel, bool? isExperimental)
    {
        string val = standardStatus;

        if (fmmLevel != null)
        {
            if (string.IsNullOrEmpty(val))
            {
                val = "FMM: " + fmmLevel.ToString();
            }
            else
            {
                val = val + " FMM: " + fmmLevel.ToString();
            }
        }

        if (isExperimental == true)
        {
            if (string.IsNullOrEmpty(val))
            {
                val = "experimental";
            }
            else
            {
                val += " experimental";
            }
        }

        if (string.IsNullOrEmpty(val))
        {
            return string.Empty;
        }

        return " (" + val + ")";
    }


    /// <summary>Writes a header.</summary>
    /// <param name="config">     The configuration.</param>
    /// <param name="definitions">The definitions to export.</param>
    private void WriteHeader(
        InfoOptions config,
        DefinitionCollection definitions)
    {
        _writer.WriteLine($"Contents of: {string.Join(", ", definitions.Manifests.Select(kvp => kvp.Key))}");
        _writer.WriteLine($"  Primitive Naming Style: {NamingConvention.CamelCase}");
        _writer.WriteLine($"  Element Naming Style: {NamingConvention.CamelCase}");
        _writer.WriteLine($"  Complex Type / Resource Naming Style: {NamingConvention.PascalCase}");
        _writer.WriteLine($"  Enum Naming Style: {NamingConvention.FhirDotNotation}");
        _writer.WriteLine($"  Interaction Naming Style: {NamingConvention.PascalCase}");
        //_writer.WriteLine($"  Extension Support: {_options.ExtensionSupport}");

        if (config.ExportStructures.Any())
        {
            string restrictions = string.Join("|", config.ExportStructures);
            _writer.WriteLine($"  Export structures: {restrictions}");
        }

        if (config.ExportKeys.Any())
        {
            string restrictions = string.Join("|", config.ExportKeys);
            _writer.WriteLine($"  Export keys: {restrictions}");
        }

        //if ((_options.LanguageOptions != null) && (_options.LanguageOptions.Count > 0))
        //{
        //    foreach (KeyValuePair<string, string> kvp in _options.LanguageOptions)
        //    {
        //        _writer.WriteLine($"  Language option: \"{kvp.Key}\" = \"{kvp.Value}\"");
        //    }
        //}
    }

}
