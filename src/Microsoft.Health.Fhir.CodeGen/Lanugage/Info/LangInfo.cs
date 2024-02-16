// <copyright file="LangInfo.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.CommandLine;
using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;
using Hl7.FhirPath.Sprache;
using Microsoft.Health.Fhir.CodeGen.Configuration;
using Microsoft.Health.Fhir.CodeGen.Extensions;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Hl7.Fhir.Model.CodeSystem;
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
    private static readonly Dictionary<string, string> _primitiveTypeMap = new();
        //{
        //    { "base", "base" },
        //    { "base64Binary", "base64Binary" },
        //    { "boolean", "boolean" },
        //    { "canonical", "canonical" },
        //    { "code", "code" },
        //    { "date", "date" },
        //    { "dateTime", "dateTime" },
        //    { "decimal", "decimal" },
        //    { "id", "id" },
        //    { "instant", "instant" },
        //    { "integer", "integer" },
        //    { "integer64", "integer64" },
        //    { "markdown", "markdown" },
        //    { "oid", "oid" },
        //    { "positiveInt", "positiveInt" },
        //    { "string", "string" },
        //    { "time", "time" },
        //    { "unsignedInt", "unsignedInt" },
        //    { "uri", "uri" },
        //    { "url", "url" },
        //    { "uuid", "uuid" },
        //    { "xhtml", "xhtml" },
        //};

    /// <summary>Gets the FHIR primitive type map.</summary>
    public Dictionary<string, string> FhirPrimitiveTypeMap => _primitiveTypeMap;

    /// <summary>The currently in-use text writer.</summary>
    private ExportStreamWriter _writer = null!;

    private DefinitionCollection _definitions = null!;

    /// <summary>Exports the given configuration.</summary>
    /// <param name="config">     The configuration.</param>
    /// <param name="definitions">The definitions to export.</param>
    /// <param name="writeStream">(Optional) Stream to write data to.</param>
    public void Export(
        InfoOptions config,
        DefinitionCollection definitions,
        Stream? writeStream = null)
    {
        _definitions = definitions;

        // TODO(ginoc): actually open the file
        // create a filename for writing (single file for now)
        //string filename = Path.Combine(config.OutputDirectory, $"Info_{definitions.FhirSequence.ToRLiteral()}.txt");
        //using (FileStream stream = new(filename, FileMode.Create))
        using (writeStream == null
            ? _writer = new ExportStreamWriter(Console.OpenStandardOutput(), System.Text.Encoding.UTF8, 1024, true)
            : _writer = new ExportStreamWriter(writeStream, System.Text.Encoding.UTF8, 1024, true))
        {
            WriteHeader(config, definitions);

            WriteStructures(definitions.PrimitiveTypesByName.Values, "Primitive Types");
            WriteStructures(definitions.ComplexTypesByName.Values, "Complex Types");
            WriteStructures(definitions.ResourcesByName.Values, "Resources");

            WriteStructures(definitions.ProfilesByUrl.Values, "Profiles");

            //WriteOperations(_info.SystemOperations.Values, true, "System Operations");
            //WriteSearchParameters(_info.AllResourceParameters.Values, "All Resource Parameters");
            //WriteSearchParameters(_info.SearchResultParameters.Values, "Search Result Parameters");
            //WriteSearchParameters(_info.AllInteractionParameters.Values, "All Interaction Parameters");

            //WriteValueSets(_info.ValueSetsByUrl.Values, "Value Sets");

            //WriteFooter();

        }

    }

    /// <summary>Writes the structures.</summary>
    /// <param name="structures">The structures.</param>
    /// <param name="headerHint">(Optional) The header hint.</param>
    private void WriteStructures(
        IEnumerable<StructureDefinition> structures,
        string headerHint = "")
    {
        if (!string.IsNullOrEmpty(headerHint))
        {
            _writer.WriteLineIndented($"{headerHint}: {structures.Count()}");
        }

        foreach (StructureDefinition sd in structures.OrderBy(c => c.Id))
        {
            WriteStructure(sd);
        }
    }

    /// <summary>Writes a primitive.</summary>
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    /// <param name="sd">The SD.</param>
    private void WritePrimitive(StructureDefinition sd)
    {
        string snip = BuildStandardSnippet(sd.cgStandardStatus(), sd.cgMaturityLevel(), sd.cgIsExperimental());

        _writer.WriteLineIndented(
            $"- {sd.Name}:" +
                $" {sd.Name.ToCamelCase()}" +
                $"::{sd.cgpTypeForExport(NamingConvention.CamelCase, _primitiveTypeMap)}" +
                $"{snip}");

        _writer.IncreaseIndent();

        // check for regex
        if (!string.IsNullOrEmpty(sd.cgpValidationRegEx()))
        {
            _writer.WriteLineIndented($"[{sd.cgpValidationRegEx()}]");
        }

        if (_definitions.ExtensionsByPath.ContainsKey(sd.Id))
        {
            WriteExtensions(_definitions.ExtensionsByPath[sd.Id].Values);
        }

        // TODO(ginoc)
        //if (_info.ProfilesByBaseType.ContainsKey(primitive.Id))
        //{
        //    WriteProfiles(_info.ProfilesByBaseType[primitive.Id].Values);
        //}

        _writer.DecreaseIndent();
    }

    /// <summary>Writes the extensions.</summary>
    /// <param name="extensions">The extensions.</param>
    private void WriteExtensions(
        IEnumerable<StructureDefinition> extensions)
    {
        _writer.WriteLineIndented($"Extensions: {extensions.Count()}");

        foreach (StructureDefinition extension in extensions.OrderBy(e => e.Id))
        {
            WriteExtension(extension);
        }
    }

    /// <summary>Writes an extension.</summary>
    /// <param name="sd">The extension.</param>
    private void WriteExtension(
        StructureDefinition sd)
    {
        string card = sd.cgRootElement()?.cgCardinality() ?? string.Empty;

        if (string.IsNullOrEmpty(card))
        {
            _writer.WriteLineIndented($"+{sd.Url}");
        }
        else
        {
            _writer.WriteLineIndented($"+{sd.Url} [{card}]");
        }

        _writer.IncreaseIndent();

        // check for a 'simple' extension
        if (((sd.Snapshot?.Element.Count ?? 0) == 5) || (sd.Differential.Element.Count == 4))
        {
            ElementDefinition? eleUrl = (sd.Snapshot?.Element.Any() ?? false)
                ? sd.Snapshot.Element[3]
                : sd.Differential.Element[2];

            _writer.WriteLineIndented($"- {eleUrl.cgNameForExport(NamingConvention.CamelCase)}[{eleUrl.cgCardinality()}]: fixed to {eleUrl.Fixed}");

            ElementDefinition? eleValue = (sd.Snapshot?.Element.Any() ?? false)
                ? sd.Snapshot.Element[4]
                : sd.Differential.Element[3];

            string propertyType = string.Empty;

            if (eleValue.Type.Any())
            {
                IReadOnlyDictionary<string, ElementDefinition.TypeRefComponent> types = eleValue.cgTypes();

                foreach ((string name, ElementDefinition.TypeRefComponent et) in types.OrderBy(r => r.Key))
                {
                    string joiner = string.IsNullOrEmpty(propertyType) ? string.Empty : "|";

                    string profiles = string.Empty;
                    if (et.Profile.Any())
                    {
                        profiles = "(" + string.Join("|", et.cgTypeProfiles().Keys) + ")";
                    }

                    string targets = string.Empty;
                    if (et.TargetProfile.Any())
                    {
                        targets = "(" + string.Join("|", et.cgTargetProfiles().Keys) + ")";
                    }
                    propertyType = $"{propertyType}{joiner}{name}{profiles}{targets}";
                }
            }

            _writer.WriteLineIndented($"- {eleValue.cgNameForExport(NamingConvention.CamelCase)}[{eleValue.cgCardinality()}]: {propertyType}");
        }
        else
        {
            //WriteStructure(sd);
        }

        _writer.DecreaseIndent();
    }

    /// <summary>Writes a complex.</summary>
    /// <param name="sd">The complex.</param>
    private void WriteStructure(StructureDefinition sd)
    {
        bool indented = false;

        // check for artifacts that thave alternative renderings
        switch (sd.cgArtifactClass())
        {
            case CodeGenCommon.Models.FhirArtifactClassEnum.Extension:
            {
                WriteExtension(sd);
                return;
            }

            case CodeGenCommon.Models.FhirArtifactClassEnum.PrimitiveType:
            {
                WritePrimitive(sd);
                return;
            }
        }

        ElementDefinition? rootElement = sd.cgRootElement();

        // write this type's line, if it's a root element
        // (sub-properties are written with cardinality in the prior loop)
        if (_writer.Indentation == 0)
        {
            string snip = BuildStandardSnippet(sd.cgStandardStatus(), sd.cgMaturityLevel(), sd.cgIsExperimental());

            if (sd.cgArtifactClass() == CodeGenCommon.Models.FhirArtifactClassEnum.Profile)
            {
                _writer.WriteLine($"- {sd.Name}: ({sd.Id}) {sd.cgBaseTypeName()}{snip}");
            }
            else
            {
                _writer.WriteLine($"- {sd.Name}: {sd.cgBaseTypeName()}{snip} (abstract: {sd.Abstract == true})");
            }

            if (rootElement != null)
            {
                WriteElement(sd, rootElement, true);
            }

            _writer.IncreaseIndent();
            indented = true;

            IEnumerable<ElementDefinition.ConstraintComponent> constraints = sd.cgConstraints(includeInherited: true);

            if (constraints.Any() == true)
            {
                WriteConstraints(sd, constraints);
            }
        }
        else if (rootElement != null)
        {
            WriteElement(sd, rootElement, true);
        }

        // write elements
        WriteElements(sd, sd.cgElements(topLevelOnly: true));

        // check for extensions if this is the first time we write this type
        if (indented &&
            _definitions.ExtensionsByPath.TryGetValue(sd.Type, out Dictionary<string, StructureDefinition>? extDict) && (extDict != null))
        {
            WriteExtensions(extDict.Values);
        }

        // check for search parameters on this object
        if (_definitions.SearchParametersForBase(sd.Type).Any())
        {
            WriteSearchParameters(_definitions.SearchParametersForBase(sd.Type).Values.OrderBy(sp => sp.Code));
        }

        // check for type operations
        //if (sd.TypeOperations.Any())
        //{
        //    WriteOperations(sd.TypeOperations.Values, true);
        //}

        //// check for instance operations
        //if (sd.InstanceOperations.Any())
        //{
        //    WriteOperations(sd.TypeOperations.Values, false);
        //}

        //if (_info.ProfilesByBaseType.TryGetValue(sd.Path, out Dictionary<string, FhirComplex> profileDict))
        //{
        //    WriteProfiles(profileDict.Values);
        //}

        if (indented)
        {
            _writer.DecreaseIndent();
        }
    }

    /// <summary>Writes the operations.</summary>
    /// <param name="operations"> The operations.</param>
    /// <param name="isTypeLevel">True if is type level, false if not.</param>
    /// <param name="headerHint"> (Optional) The header hint.</param>
    private void WriteOperations(
        IEnumerable<OperationDefinition> operations,
        bool isTypeLevel,
        string headerHint = "")
    {
        bool indented = false;

        if (!string.IsNullOrEmpty(headerHint))
        {
            _writer.WriteLineIndented($"{headerHint}: {operations.Count()}");
            _writer.IncreaseIndent();
            indented = true;
        }

        foreach (OperationDefinition operation in operations.OrderBy(o => o.Code))
        {
            string snip = BuildStandardSnippet(operation.cgStandardStatus(), operation.cgMaturityLevel(), operation.cgIsExperimental());

            if (isTypeLevel)
            {
                _writer.WriteLineIndented($"${operation.Code}{snip}");
            }
            else
            {
                _writer.WriteLineIndented($"/{{id}}/${operation.Code}{snip}");
            }

            if (operation.Parameter.Any())
            {
                _writer.IncreaseIndent();

                // write operation parameters inline
                foreach (OperationDefinition.ParameterComponent parameter in operation.Parameter.OrderBy(p => p.cgFieldOrder()))
                {
                    string st = (parameter.SearchType == null) ? string.Empty : "<" + parameter.SearchType.GetLiteral() + ">";
                    _writer.WriteLineIndented(
                        $"{parameter.Use}:" +
                        $" {parameter.Name}:" +
                        $" {parameter.Type}{st}[{parameter.cgCardinality()}]");
                }

                _writer.DecreaseIndent();
            }
        }

        if (indented)
        {
            _writer.DecreaseIndent();
        }
    }

    /// <summary>Writes search parameters.</summary>
    /// <param name="searchParameters">Options for controlling the search.</param>
    /// <param name="headerHint">      (Optional) The header hint.</param>
    private void WriteSearchParameters(
        IEnumerable<SearchParameter> searchParameters,
        string headerHint = "")
    {
        bool indented = false;

        if (!string.IsNullOrEmpty(headerHint))
        {
            _writer.WriteLineIndented($"{headerHint}: {searchParameters.Count()}");
            _writer.IncreaseIndent();
            indented = true;
        }

        foreach (SearchParameter searchParam in searchParameters.OrderBy(s => s.Code))
        {
            if (searchParam.Component.Any())
            {
                _writer.WriteLineIndented($"?{searchParam.Name}: {searchParam.Code} is composite (resolves: {searchParam.cgCompositeResolves(_definitions.SearchParametersByUrl.Keys)})");

                _writer.IncreaseIndent();

                foreach (SearchParameter.ComponentComponent c in searchParam.Component)
                {
                    if (_definitions.SearchParametersByUrl.TryGetValue(c.Definition, out SearchParameter? compParam))
                    {
                        _writer.WriteLineIndented($"$({c.Definition}):{compParam.Type}");
                    }
                    else
                    {
                        _writer.WriteLineIndented($"$({c.Definition}):unresolved");
                    }
                }

                _writer.DecreaseIndent();
            }
            else
            {
                string snip = BuildStandardSnippet(searchParam.cgStandardStatus(), searchParam.cgMaturityLevel(), searchParam.cgIsExperimental());
                _writer.WriteLineIndented($"?{searchParam.Name}: {searchParam.Code}={searchParam.Type}{snip}");
            }
        }

        if (indented)
        {
            _writer.DecreaseIndent();
        }
    }
    /// <summary>Writes the constraints.</summary>
    /// <param name="constraints">The constraints.</param>
    private void WriteConstraints(StructureDefinition sd, IEnumerable<ElementDefinition.ConstraintComponent> constraints)
    {
        foreach (ElementDefinition.ConstraintComponent constraint in constraints)
        {
            string inherited = constraint.cgIsInherited(sd) ? "inherited" : "local";
            _writer.WriteLineIndented($"!{constraint.Key}: {inherited} {constraint.Severity.GetLiteral()}: {constraint.Expression}");
        }
    }

    /// <summary>Writes the elements.</summary>
    /// <param name="complex">    The complex.</param>
    private void WriteElements(StructureDefinition sd, IEnumerable<ElementDefinition> elements)
    {
        foreach (ElementDefinition element in elements.OrderBy(s => s.cgFieldOrder()))
        {
            WriteElement(sd, element);
        }
    }

    /// <summary>Writes an element.</summary>
    /// <param name="sd">      The complex.</param>
    /// <param name="ed">      The element.</param>
    /// <param name="writeAsRootElementInfo">(Optional) True if is root element, false if not.</param>
    private void WriteElement(
        StructureDefinition sd,
        ElementDefinition ed,
        bool writeAsRootElementInfo = false)
    {
        string propertyType = string.Empty;

        if (ed.Type.Any())
        {
            IReadOnlyDictionary<string, ElementDefinition.TypeRefComponent> types = ed.cgTypes();

            foreach ((string name, ElementDefinition.TypeRefComponent et) in types.OrderBy(r => r.Key))
            {
                string joiner = string.IsNullOrEmpty(propertyType) ? string.Empty : "|";

                string profiles = string.Empty;
                if (et.Profile.Any())
                {
                    profiles = "(" + string.Join("|", et.cgTypeProfiles().Keys) + ")";
                }

                string targets = string.Empty;
                if (et.TargetProfile.Any())
                {
                    targets = "(" + string.Join("|", et.cgTargetProfiles().Keys) + ")";
                }

                propertyType = $"{propertyType}{joiner}{name}{profiles}{targets}";
            }
        }

        if (string.IsNullOrEmpty(propertyType))
        {
            propertyType = ed.Base?.Path ?? "!!! unknown type !!!";
        }

        if (ed.cgIsSimple())
        {
            propertyType += " *simple*";
        }

        if (ed.Constraint.Any(c => !c.cgIsInherited(sd)))
        {
            propertyType = propertyType +
                " constraints: " +
                string.Join(", ", ed.Constraint.Where(c => !c.cgIsInherited(sd)).Select(c => c.Key).OrderBy(v => v, NaturalComparer.Instance)) +
                "";
        }

        if (ed.Condition.Any())
        {
            propertyType = propertyType +
                " conditions: " +
                string.Join(", ", ed.Condition.OrderBy(v => v, NaturalComparer.Instance)) +
                "";
        }

        if (!string.IsNullOrEmpty(ed.cgBindingName()))
        {
            propertyType = propertyType + " binding name: " + ed.cgBindingName();
        }

        string fiveWs = string.Empty;

        if (!string.IsNullOrEmpty(ed.cgFiveWs()))
        {
            fiveWs = " (W5: " + ed.cgFiveWs() + ")";
        }

        if (writeAsRootElementInfo)
        {
            _writer.IncreaseIndent();
            _writer.WriteLineIndented(
                $"^{ed.cgNameForExport(NamingConvention.CamelCase)}[{ed.cgCardinality()}]:" +
                $" {propertyType}" +
                $"{fiveWs}");
            _writer.DecreaseIndent();
        }
        else
        {
            _writer.WriteLineIndented(
                $"-" +
                $" {ed.cgNameForExport(NamingConvention.CamelCase)}[{ed.cgCardinality()}]:" +
                $" {propertyType}" +
                $"{fiveWs}");
        }

        // check for regex
        if (!string.IsNullOrEmpty(ed.cgValidationRegEx()))
        {
            _writer.WriteLineIndented($"[{ed.cgValidationRegEx()}]");
        }

        // check for default value
        if (ed.DefaultValue != null)
        {
            _writer.WriteLineIndented($".{ed.cgDefaultFieldName()} = {ed.DefaultValue}");
        }

        // check for fixed value
        if (ed.Fixed != null)
        {
            _writer.WriteLineIndented($".{ed.cgFixedFieldName()} = {ed.Fixed}");
        }

        // check for pattern value
        if (ed.Pattern != null)
        {
            _writer.WriteLineIndented($".{ed.cgPatternFieldName()} = {ed.Pattern}");
        }

        IEnumerable<string> codes = ed.cgCodes(_definitions);

        if (codes.Any())
        {
            _writer.IncreaseIndent();
            _writer.WriteLineIndented($"{{{string.Join('|', codes)}}}");
            _writer.DecreaseIndent();
        }

        if (!writeAsRootElementInfo)
        {
            if (_definitions.ExtensionsByPath.ContainsKey(ed.Path))
            {
                _writer.IncreaseIndent();
                WriteExtensions(_definitions.ExtensionsByPath[ed.Path].Values);
                _writer.DecreaseIndent();
            }

            if (_definitions.IsBackbonePath(ed.Path))
            {
                _writer.IncreaseIndent();
                WriteElements(sd, sd.cgElements(forBackbonePath: ed.Path, topLevelOnly: true));
                _writer.DecreaseIndent();
            }

            // check for slicing information defined by the current sd
            if (_definitions.TryGetSliceNames(ed.Path, out string[]? sliceNames, sd) &&
                (sliceNames?.Any() ?? false))
            {
                WriteSlices(sd, ed, sliceNames);
            }
        }

        //_writer.DecreaseIndent();
    }

    /// <summary>Writes the slicings.</summary>
    /// <param name="sliceNames">The slicings.</param>
    private void WriteSlices(
        StructureDefinition sd,
        ElementDefinition ed,
        string[] sliceNames)
    {
        _writer.IncreaseIndent();

        if (ed.Slicing == null)
        {
            _writer.WriteLineIndented($"@ No defined slicing");
        }
        else
        {
            _writer.WriteLineIndented($"@ {ed.Slicing.Rules.GetLiteral()}: {string.Join(", ", ed.Slicing.Discriminator.Select(d => $"{d.Type.GetLiteral()}:{d.Path}"))}");
        }

        _writer.IncreaseIndent();

        foreach (string sliceName in sliceNames)
        {
            // get the elements for this slice
            IEnumerable<ElementDefinition> sliceElements = sd.cgElementsForSlice(ed, sliceName);

            // get the discriminated values (if defined)
            IEnumerable<(string type, string path, string value)> dvs = sd.cgDiscriminatedValues(ed, sliceName, sliceElements);

            if (dvs.Any())
            {
                _writer.WriteLineIndented($":{sliceName}");
                _writer.IncreaseIndent();

                foreach ((string type, string path, string value) in dvs)
                {
                    _writer.WriteLineIndented($"- {type} @ {path} = {value}");
                }

                _writer.DecreaseIndent();
            }

            //// write the sub-elements defined within this slice
            //WriteElements(sd, sliceElements.Skip(1));
        }

        //IEnumerable<CodeGenSlice> slicings = sd.cgSlices(ed, sliceNames);
        //foreach (CodeGenSlice slicing in slicings)
        //{
        //    WriteSlice(sd, path, slicing);
        //}

        _writer.DecreaseIndent();
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
