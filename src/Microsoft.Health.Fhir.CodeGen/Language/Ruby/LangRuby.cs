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
using Hl7.Fhir.Utility;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Utils;
using static Microsoft.Health.Fhir.CodeGenCommon.Extensions.FhirNameConventionExtensions;
using static Microsoft.Health.Fhir.CodeGenCommon.Extensions.LinqExtensions;

#if NETSTANDARD2_0
using Microsoft.Health.Fhir.CodeGenCommon.Polyfill;
#endif

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

    private static readonly HashSet<string> _unexportableSystems =
    [
        "http://www.rfc-editor.org/bcp/bcp13.txt",
        "http://hl7.org/fhir/ValueSet/mimetype",
        "http://hl7.org/fhir/ValueSet/mimetypes",
        "http://hl7.org/fhir/ValueSet/ucum-units",
    ];


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

        // write the metadata file
        WriteMetadata(_dc.PrimitiveTypesByName, _dc.ComplexTypesByName, _dc.ResourcesByName);

        // write complex types
        WriteTypes(_dc.ComplexTypesByName);

        // write resources
        WriteResources(_dc.ResourcesByName);
    }

    private ExportStreamWriter OpenWriter(string relative)
    {
        ExportStreamWriter writer = new ExportStreamWriter(Path.Combine(_config.OutputDirectory, relative), false);

        writer.WriteLine($"module {_config.Module}");
        writer.IncreaseIndent();

        return writer;
    }

    private void CloseAndDispose(ExportStreamWriter writer)
    {
        writer.DecreaseIndent();
        writer.WriteLine("end");    // module

        writer.Dispose();
    }

    private static string BuildCommentString(ComponentDefinition cd)
    {
        if (cd.IsRootOfStructure)
        {
            return BuildCommentString(cd.Structure);
        }

        return BuildCommentString(cd.Element);
    }

    private static string BuildCommentString(StructureDefinition sd)
    {
        string[] values = (!string.IsNullOrEmpty(sd.Description) && !string.IsNullOrEmpty(sd.Title) && sd.Description.StartsWith(sd.Title))
            ? new[] { sd.Description, sd.Purpose }
            : new[] { sd.Title, sd.Description, sd.Purpose };

        return string.Join("\n", values.Where(s => !string.IsNullOrEmpty(s)).Distinct()).Trim();
    }

    private static string BuildCommentString(ElementDefinition ed)
    {
        string[] values = (!string.IsNullOrEmpty(ed.Definition) && !string.IsNullOrEmpty(ed.Short) && ed.Definition.StartsWith(ed.Short))
            ? new[] { ed.Definition, ed.Comment }
            : new[] { ed.Short, ed.Definition, ed.Comment };
        return string.Join("\n", values.Where(s => !string.IsNullOrEmpty(s)).Distinct()).Trim();
    }

    private void WriteTypes(IReadOnlyDictionary<string, StructureDefinition> complexTypes)
    {
        string dir = Path.Combine(_config.OutputDirectory, "types");
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        foreach (StructureDefinition sd in complexTypes.Values)
        {
            ExportStreamWriter writer = OpenWriter($"types/{sd.Id}.rb");

            WriteStructure(writer, new ComponentDefinition(sd));

            CloseAndDispose(writer);
        }
    }

    private void WriteResources(IReadOnlyDictionary<string, StructureDefinition> resources)
    {
        string dir = Path.Combine(_config.OutputDirectory, "resources");
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        foreach (StructureDefinition sd in resources.Values)
        {
            ExportStreamWriter writer = OpenWriter($"resources/{sd.Id}.rb");

            WriteStructure(writer, new ComponentDefinition(sd));

            CloseAndDispose(writer);
        }
    }

    /// <summary>Names and types for export.</summary>
    /// <param name="ed">                    The ElementDefinition we are writing.</param>
    /// <param name="nameConvention">        (Optional) The name convention.</param>
    /// <param name="typeConvention">        (Optional) The type convention.</param>
    /// <param name="concatenatePath">       (Optional) True to concatenate path.</param>
    /// <param name="concatenationDelimiter">(Optional) The concatenation delimiter.</param>
    /// <param name="isComponent">           (Optional) True if is component, false if not.</param>
    /// <returns>A Dictionary&lt;string,string&gt;</returns>
    private Dictionary<string, (string, ElementDefinition.TypeRefComponent?)> NamesAndTypesForExport(
        ElementDefinition ed,
        NamingConvention nameConvention = NamingConvention.CamelCase,
        NamingConvention typeConvention = NamingConvention.PascalDelimited,
        bool concatenatePath = false,
        string concatenationDelimiter = "::")
    {
        Dictionary<string, (string, ElementDefinition.TypeRefComponent?)> values = [];

        string baseName = ed.cgName();
        bool isChoice = false;

        IReadOnlyDictionary<string, ElementDefinition.TypeRefComponent> elementTypes = ed.cgTypes(coerceToR5: true);

        if (elementTypes.Count == 0)
        {
            // check for a backbone element
            if (_dc.HasChildElements(ed.Path))
            {
                values.Add(
                    FhirSanitizationUtils.ToConvention(baseName, ed.Path, nameConvention, concatenatePath, concatenationDelimiter),
                    (FhirSanitizationUtils.ToConvention(string.Empty, ed.Path, typeConvention, concatenationDelimiter: concatenationDelimiter), null));

                return values;
            }

            // if there are no types, use the base type
            values.Add(
                FhirSanitizationUtils.ToConvention(baseName, ed.Path, nameConvention, concatenatePath, concatenationDelimiter),
                (FhirSanitizationUtils.ToConvention(ed.cgBaseTypeName(_dc, true, _primitiveTypeMap), string.Empty, typeConvention, concatenationDelimiter: concatenationDelimiter), null));

            return values;
        }

        if (baseName.Contains("[x]", StringComparison.OrdinalIgnoreCase) ||
            (elementTypes.Count > 1))
        {
            baseName = baseName.Replace("[x]", string.Empty, StringComparison.OrdinalIgnoreCase);
            isChoice = true;
        }

        if (isChoice)
        {
            foreach (ElementDefinition.TypeRefComponent elementType in elementTypes.Values.OrderBy(et => et.Code))
            {
                string name = FhirSanitizationUtils.ToConvention(baseName, ed.Path, nameConvention, concatenatePath, concatenationDelimiter);

                string type;
                string combined;

                type = FhirSanitizationUtils.ToConvention(elementType.cgName(), string.Empty, typeConvention);
                combined = name + type;

                _ = values.TryAdd(combined, (type, elementType));
            }
        }
        else
        {
            string type = elementTypes.Values.FirstOrDefault()?.cgName() ?? string.Empty;

            if ((type == "BackboneElement") || (type == "Element"))
            {
                // check for a backbone element
                if (_dc.HasChildElements(ed.Path))
                {
                    values.Add(
                        FhirSanitizationUtils.ToConvention(baseName, ed.Path, nameConvention, concatenatePath, concatenationDelimiter),
                        (FhirSanitizationUtils.ToConvention(string.Empty, ed.Path, typeConvention, concatenationDelimiter: concatenationDelimiter),
                        elementTypes.Values.First()));

                    return values;
                }

                // if there are no types, use the base type
                values.Add(
                    FhirSanitizationUtils.ToConvention(baseName, ed.Path, nameConvention, concatenatePath, concatenationDelimiter),
                    (FhirSanitizationUtils.ToConvention(string.Empty, ed.Path, typeConvention, concatenationDelimiter: concatenationDelimiter), elementTypes.Values.First()));

                return values;
            }

            string cased = FhirSanitizationUtils.ToConvention(baseName, string.Empty, nameConvention, concatenatePath, concatenationDelimiter);

            _ = values.TryAdd(cased, (type, elementTypes.Values.First()));
        }

        return values;
    }

    private void WriteElementMetadata(
        ExportStreamWriter writer,
        ElementDefinition ed,
        bool isLast,
        int depth)
    {
        string rubyTypedPath = depth == 0 ? ed.Path : string.Join(".", ed.Path.Split('.').Skip(depth));
        if (char.IsLower(rubyTypedPath[0]))
        {
            rubyTypedPath = char.ToUpper(rubyTypedPath[0]) + rubyTypedPath[1..];
        }

        Dictionary<string, (string, ElementDefinition.TypeRefComponent?)> values = NamesAndTypesForExport(ed);

        foreach ((string exportName, (string elementType, ElementDefinition.TypeRefComponent? tr)) in values)
        {
            // write our element comment
            writer.WriteRubyComment(BuildCommentString(ed));

            // open the scope with our element name
            writer.OpenScope($"'{exportName}' => {{");              // open element

            // check for a reserved-word name
            if (RubyCommon.ReservedWords.Contains(exportName))
            {
                writer.WriteLineIndented($"'local_name'=>'local_{exportName}'");
            }

            // list codes for any type of binding
            if ((!string.IsNullOrEmpty(ed.Binding?.ValueSet)) &&
                (!_unexportableSystems.Contains(_dc.UnversionedUrlForVs(ed.Binding!.ValueSet))) &&
                _dc.TryExpandVs(ed.Binding.ValueSet, out ValueSet? boundVs))
            {
                // write the full expansion
                //writer.WriteLineIndented($"'valid_codes'=>{{ {string.Join(", ", vs.Expansion.Contains.Select(c => $"'{c.Code}'"))} }}");
                writer.OpenScope($"'valid_codes'=>{{");

                string[] codeSystemUrls = boundVs.cgReferencedCodeSystems().Order().ToArray();

                codeSystemUrls.ForEach((string csUrl, bool isLast) =>
                {
                    if (isLast)
                    {
                        writer.WriteLineIndented($"'{csUrl}'=>[ {string.Join(", ", boundVs.Expansion.Contains.Where(c => c.System == csUrl).Select(c => $"'{c.Code}'"))} ]");
                    }
                    else
                    {
                        writer.WriteLineIndented($"'{csUrl}'=>[ {string.Join(", ", boundVs.Expansion.Contains.Where(c => c.System == csUrl).Select(c => $"'{c.Code}'"))} ],");
                    }

                    return true;
                });

                writer.CloseScope("},");
            }
            else
            {
                boundVs = null;
            }

            // check for target profiles
            if (tr?.TargetProfileElement.Count > 0)
            {
                writer.WriteLineIndented($"'type_profiles'=>[{string.Join(", ", tr.TargetProfileElement.Select(tpe => "'" + tpe.Value + "'"))}],");
            }

            // write the type information
            writer.WriteLineIndented($"'type'=>'{elementType}',");

            // write the element path
            writer.WriteLineIndented($"'path'=>'{rubyTypedPath}',");

            // write the cardinality information
            writer.WriteLineIndented($"'min'=>{ed.cgCardinalityMin()},");
            writer.WriteLineIndented($"'max'=>{(ed.cgCardinalityMax() == -1 ? "Float::INFINITY" : ed.cgCardinalityMax() )}{(boundVs == null ? string.Empty : ",")}");

            if ((boundVs != null) && (ed.Binding?.Strength != null))
            {
                writer.WriteLineIndented($"'binding'=>{{'strength'=>'{EnumUtility.GetLiteral(ed.Binding.Strength)}', 'uri'=>'{boundVs.Url}'}}");
            }

            if (isLast)
            {
                writer.CloseScope("}");                             // close element
            }
            else
            {
                writer.CloseScope("},");                            // close element
            }

            // TODO(ginoc): Determine if we should add primitive extensions - check with Yunwei
            //if (RequiresPrimitiveExtension(elementType))
            //{
            //    _writer.WriteLineIndented($"_{exportName}?: Element{arrayFlagString} | undefined;");
            //}
        }

        return;
    }

    private static bool RequiresPrimitiveExtension(string typeName)
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

    private void WriteElementAccessor(ExportStreamWriter writer, ElementDefinition ed)
    {
        Dictionary<string, (string, ElementDefinition.TypeRefComponent?)> values = NamesAndTypesForExport(ed);

        foreach ((string exportName, (string elementType, ElementDefinition.TypeRefComponent? tr)) in values)
        {
            // write our element comment
            writer.WriteRubyComment(BuildCommentString(ed));

            string name;

            // check for a reserved-word name
            if (RubyCommon.ReservedWords.Contains(exportName))
            {
                name = $"local_{exportName}";
            }
            else
            {
                name = exportName;
            }

            string expandedType;

            // check for target profiles
            if (tr?.TargetProfileElement.Count > 0)
            {
                expandedType = $"{elementType}({string.Join("|", tr.TargetProfileElement)})";
            }
            else
            {
                expandedType = elementType;
            }

            if (ed.cgCardinalityMax() == 1)
            {
                writer.WriteLineIndented(
                    $"attr_accessor :{name,-30}" +
                    $" # {ed.cgCardinalityMin()}-1" +
                    $" {expandedType}");
            }
            else
            {
                writer.WriteLineIndented(
                    $"attr_accessor :{name,-30}" +
                    $" # {ed.cgCardinalityMin()}-*" +
                    $" [ {expandedType} ]");
            }

            // TODO(ginoc): Determine if we should add primitive extensions - check with Yunwei
            //if (RequiresPrimitiveExtension(elementType))
            //{
            //    _writer.WriteLineIndented($"_{exportName}?: Element{arrayFlagString} | undefined;");
            //}
        }

        return;
    }

    private void WriteStructure(ExportStreamWriter writer, ComponentDefinition cd, int depth = 0)
    {
        writer.WriteLine();
        writer.WriteRubyComment(BuildCommentString(cd));
        writer.OpenScope($"class {cd.cgName()} < FHIR::Model");     // open class
        writer.WriteLineIndented("include FHIR::Hashable");
        writer.WriteLineIndented("include FHIR::Json");
        writer.WriteLineIndented("include FHIR::Xml");

        writer.WriteLine();

        // check for root of structure to write search parameters
        if (cd.IsRootOfStructure)
        {
            IReadOnlyDictionary<string, SearchParameter> searchParams = _dc.SearchParametersForBase(cd.Structure.Id);

            // check for search parameters defined for this class
            if (searchParams.Any())
            {
                writer.WriteLineIndented($"SEARCH_PARAMS = [{string.Join(", ", searchParams.Values.Select(sp => "'" + sp.Code + "'").Distinct().Order())}]");
            }
            else
            {
                writer.WriteLineIndented("SEARCH_PARAMS = []");
            }
        }

        // get the elements for this level of this component
        ElementDefinition[] elements = cd.cgGetChildren(false, true).ToArray();

        // write choice element roots as MULTIPLE_TYPES
        ElementDefinition[] choiceElements = elements.Where(ed => ed.IsChoice()).ToArray();

        if (choiceElements.Length != 0)
        {
            writer.OpenScope("MULTIPLE_TYPES = {");     // open MULTIPLE_TYPES

            for (int i = 0; i < choiceElements.Length; i++)
            {
                ElementDefinition ed = choiceElements[i];
                writer.WriteLineIndented(
                    $"'{ed.cgName()}' => [{string.Join(", ", ed.cgTypes(coerceToR5: true).Keys.Order().Select(v => "'" + v + "'"))}]" +
                    (i < choiceElements.Length - 1 ? "," : string.Empty));
            }

            writer.CloseScope("}");                     // close MULTIPLE_TYPES
        }

        if (elements.Length != 0)
        {
            writer.OpenScope("METADATA = {");           // open METADATA

            // write elements
            elements.ForEach((ElementDefinition ed, bool isLast) =>
            {
                WriteElementMetadata(writer, ed, isLast, depth);
                return true;
            });

            writer.CloseScope("}");                     // close METADATA
        }

        // nest into component definitions
        foreach (ComponentDefinition childComponent in cd.cgChildComponents(_dc, false))
        {
            WriteStructure(writer, childComponent, depth + 1);
        }

        // write attribute accessors
        if (elements.Length != 0)
        {
            // write elements
            foreach (ElementDefinition ed in elements)
            {
                WriteElementAccessor(writer, ed);
            }
        }

        // if this is a resource, add the resource type definition
        if (cd.IsRootOfStructure &&
            (cd.Structure.cgArtifactClass() == CodeGenCommon.Models.FhirArtifactClassEnum.Resource))
        {
            writer.WriteLine();
            writer.OpenScope("def resourceType");
            writer.WriteLineIndented($"'{cd.Structure.Id}'");
            writer.CloseScope("end");
        }

        writer.CloseScope("end");                                   // close class
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

            // need to escape backslashes in the regex
            string regex = sd.cgpValidationRegEx().Replace("\\", "\\\\");

            writer.WriteRubyComment(BuildCommentString(sd));

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
