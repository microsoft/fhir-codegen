// <copyright file="LangRuby.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;
using Microsoft.Health.Fhir.CodeGen.Models;

namespace Microsoft.Health.Fhir.CodeGen.Language.Ruby;

public class LangRuby : ILanguage
{
    public Type ConfigType => typeof(RubyOptions);

    public string Name => "Ruby";

    /// <summary>Dictionary mapping FHIR primitive types to language equivalents.</summary>
    private static readonly Dictionary<string, string> _primitiveTypeMap = new()
    {
        //{ "base", "Object" },
        { "base64Binary", "string" },
        { "boolean", "boolean" },
        { "canonical", "string" },
        { "code", "string" },
        { "date", "date" },
        { "dateTime", "datetime" },
        { "decimal", "decimal" },
        { "id", "string" },
        { "instant", "datetime" },
        { "integer", "integer" },
        { "integer64", "string" },       // int64 serializes as string
        { "markdown", "string" },
        { "oid", "string" },
        { "positiveInt", "string" },
        { "string", "string" },
        { "time", "time" },
        { "unsignedInt", "string" },
        { "uri", "string" },
        { "url", "string" },
        { "uuid", "string" },
        { "xhtml", "string" },
    };

    public Dictionary<string, string> FhirPrimitiveTypeMap => _primitiveTypeMap;

    public bool IsIdempotent => true;

    private RubyOptions _config = null!;
    private DefinitionCollection _dc = null!;

    public void Export(object untypedConfig, DefinitionCollection definitions)
    {
        if (untypedConfig is not RubyOptions config)
        {
            throw new ArgumentException("Invalid configuration type");
        }

        _config = config;
        _dc = definitions;

        WriteMetadata(_dc.PrimitiveTypesByName, _dc.ComplexTypesByName, _dc.ResourcesByName);
    }

    private ExportStreamWriter OpenWriter(string relative)
    {
        ExportStreamWriter writer = new ExportStreamWriter(Path.Combine(_config.OutputDirectory, relative), false);

        writer.WriteLine($"module {_config.Module}");
        writer.IncreaseIndent();

        return writer;
    }

    void CloseAndDispose(ExportStreamWriter writer)
    {
        writer.DecreaseIndent();
        writer.WriteLine("end");    // module

        writer.Dispose();
    }

    private static string BuildCommentString(StructureDefinition sd)
    {
        string[] values = (!string.IsNullOrEmpty(sd.Description) && !string.IsNullOrEmpty(sd.Title) && sd.Description.StartsWith(sd.Title))
            ? new[] { sd.Description, sd.Purpose }
            : new[] { sd.Title, sd.Description, sd.Purpose };

        return string.Join('\n', values.Where(s => !string.IsNullOrEmpty(s)).Distinct()).Trim();
    }

    private static string BuildCommentString(ElementDefinition ed)
    {
        string[] values = (!string.IsNullOrEmpty(ed.Definition) && !string.IsNullOrEmpty(ed.Short) && ed.Definition.StartsWith(ed.Short))
            ? new[] { ed.Definition, ed.Comment }
            : new[] { ed.Short, ed.Definition, ed.Comment };
        return string.Join('\n', values.Where(s => !string.IsNullOrEmpty(s)).Distinct()).Trim();
    }

    private void WriteTypes(IReadOnlyDictionary<string, StructureDefinition> complexTypes)
    {
        foreach (StructureDefinition sd in complexTypes.Values)
        {
            ExportStreamWriter writer = OpenWriter($"types/{sd.Id}.rb");



            CloseAndDispose(writer);
        }
    }

    private void WriteStructure(ExportStreamWriter writer, ComponentDefinition cd)
    {

    }

    private void WriteMetadata(
        IReadOnlyDictionary<string, StructureDefinition> primitives,
        IReadOnlyDictionary<string, StructureDefinition> complexTypes,
        IReadOnlyDictionary<string, StructureDefinition> resources)
    {
        ExportStreamWriter writer = OpenWriter("metadata.rb");

        // start with primitive types

        writer.OpenScope("PRIMITIVES = {");     // PRIMITIVES

        foreach (StructureDefinition sd in _dc.PrimitiveTypesByName.Values.OrderBy(v => v.Id))
        {
            if (!_primitiveTypeMap.TryGetValue(sd.Id, out string? rubyType))
            {
                rubyType = sd.Id.ToLowerInvariant();
            }

            string regex = sd.cgpValidationRegEx();

            writer.WriteIndentedComment(BuildCommentString(sd));

            if (string.IsNullOrEmpty(regex))
            {
                writer.WriteLineIndented($"'{sd.Name}' => {{'type'=>'{rubyType}' }},");
            }
            else
            {
                writer.WriteLineIndented($"'{sd.Name}' => {{'type'=>'{rubyType}', 'regex'=>'{regex}' }},");
            }
        }

        writer.CloseScope();                    // PRIMITIVES

        // next are complex types

        // force Element and BackboneElement to be first
        List<string> cts = new() { "Element", "BackboneElement" };
        cts.AddRange(complexTypes.Keys.Order());
        writer.WriteLineIndented($"TYPES = [ {string.Join(", ", cts.Distinct().Select(v => $"'{v}'"))} ]");

        // force Resource to be first
        List<string> rs = new() { "Resource" };
        rs.AddRange(resources.Keys.Order());
        writer.WriteLineIndented($"RESOURCES = [ {string.Join(", ", rs.Distinct().Select(v => $"'{v}'"))} ]");

        CloseAndDispose(writer);
    }
}
