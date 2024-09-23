// <copyright file="LangCql.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using static Microsoft.Health.Fhir.CodeGen.Language.Firely.CSharpFirely2;

#if NETSTANDARD2_0
using Microsoft.Health.Fhir.CodeGenCommon.Polyfill;
#endif

namespace Microsoft.Health.Fhir.CodeGen.Language.Cql;

public class LangCql : ILanguage
{
    public Type ConfigType => typeof(CqlOptions);

    public string Name => "CQL";

    /// <summary>Dictionary mapping FHIR primitive types to language equivalents.</summary>
    private static readonly Dictionary<string, string> _primitiveTypeMap = [];

    public Dictionary<string, string> FhirPrimitiveTypeMap => _primitiveTypeMap;

    public bool IsIdempotent => true;

    private CqlOptions _config = null!;
    private DefinitionCollection _dc = null!;

    private readonly Dictionary<string, string> _valueTypeConverters = new()
    {
        { "CodeableConcept", "ToConcept" },
        { "Coding", "ToCoding" },
        { "Period", "ToInterval" },
        { "Quantity", "ToQuantity" },
        { "Range", "ToInterval" },
        { "Ratio", "ToRatio" },
    };

    private readonly Dictionary<string, string> _primitiveConverters = new()
    {
        { "base64Binary", "ToString" },
        { "boolean", "ToBoolean" },
        { "canonical", "ToString" },
        //{ "code", "ToString" },           // TODO(ginoc): I cannot find a function for this, what is the desired behavior?
        { "date", "ToDate" },
        { "dateTime", "ToDateTime" },
        { "decimal", "ToDecimal" },
        { "id", "ToString" },
        { "instant", "ToDateTime" },
        { "integer", "ToInteger" },
        { "integer64", "ToString" },       // TODO(ginoc): Is there a version we can start using a non-string type?
        { "markdown", "ToString" },
        { "oid", "ToString" },
        { "positiveInt", "ToInteger" },
        { "string", "ToString" },
        { "time", "ToTime" },
        { "unsignedInt", "ToInteger" },
        { "uri", "ToString" },
        { "url", "ToString" },
        { "uuid", "ToString" },
        { "xhtml", "ToString" },
    };

    internal readonly HashSet<string> _exclusionSet =
    [
        /* UCUM is used as a required binding in a codeable concept. Since we do not
         * use enums in this situation, it is not useful to generate this valueset
         */
        "http://hl7.org/fhir/ValueSet/ucum-units",

        /* R5 made Resource.language a required binding to all-languages, which contains
         * all of bcp:47 and is listed as infinite. This is not useful to generate.
         * Note that in R5, many elements that are required to all-languages also have bound
         * starter value sets.  TODO: consider if we want to generate constants for those.
         */
        "http://hl7.org/fhir/ValueSet/all-languages",

        /* MIME types are infinite, so we do not want to generate these.
         * Note that in R5, many elements that are required to MIME type also have bound
         * starter value sets.  TODO: consider if we want to generate constants for those.
         */
        "http://hl7.org/fhir/ValueSet/mimetypes",
    ];

    private readonly char[] _vsNameDelimitersToRemove = [' ', '.', '-'];


    public void Export(object untypedConfig, DefinitionCollection definitions)
    {
        if (untypedConfig is not CqlOptions config)
        {
            throw new ArgumentException("Invalid configuration type");
        }

        _config = config;
        _dc = definitions;

        // if the primary package directive is a core package, we need to write helpers
        if (FhirPackageUtils.PackageIsFhirCore(definitions.MainPackageId))
        {
            WriteFhirHelperFile();
        }
    }

    private void WriteFhirHelperFile()
    {
        string version = _dc.MainPackageVersion.Split('-')[0];

        ExportStreamWriter writer = OpenWriter($"FHIRHelpers-{version}.cql");
        WriteHelperHeader(writer);

        writer.WriteLineIndented($"library FHIRHelpers version '{version}'");
        writer.WriteLine();
        writer.WriteLineIndented($"using FHIR version '{version}'");
        writer.WriteLine();

        WriteHelperToInterval(writer);
        WriteHelperToCalendarUnit(writer);
        WriteHelperToQuantity(writer);
        WriteHelperToRatio(writer);
        WriteHelperToCode(writer);
        WriteHelperToConcept(writer);
        WriteHelperToValueSet(writer);
        WriteHelperToReference(writer);
        WriteHelperToValue(writer);
        WriteHelperFhirPathFunctions(writer);
        WriteHelperPrimitiveConverters(writer);
        WriteHelperValueSetCodeConverters(writer);

        CloseAndDispose(writer);
    }

    private void WriteHelperToInterval(ExportStreamWriter writer)
    {
        writer.WriteLine("""
            define function ToInterval(period FHIR.Period):
                if period is null then
                    null
                else
                    if period."start" is null then
                        Interval(period."start".value, period."end".value]
                    else
                        Interval[period."start".value, period."end".value]

            define function ToInterval(period FHIR.Period):
                if period is null then
                    null
                else
                    if period."start" is null then
                        Interval(period."start".value, period."end".value]
                    else
                        Interval[period."start".value, period."end".value]

            define function ToInterval(quantity FHIR.Quantity):
                if quantity is null then null else
                    case quantity.comparator.value
                        when '<' then
                            Interval[
                                null,
                                ToQuantityIgnoringComparator(quantity)
                            )
                        when '<=' then
                            Interval[
                                null,
                                ToQuantityIgnoringComparator(quantity)
                            ]
                        when '>=' then
                            Interval[
                                ToQuantityIgnoringComparator(quantity),
                                null
                            ]
                        when '>' then
                            Interval(
                                ToQuantityIgnoringComparator(quantity),
                                null
                            ]
                        else
                            Interval[ToQuantity(quantity), ToQuantity(quantity)]
                    end
            
            define function ToInterval(quantity FHIR.Quantity):
                if quantity is null then null else
                    case quantity.comparator.value
                        when '<' then
                            Interval[
                                null,
                                ToQuantityIgnoringComparator(quantity)
                            )
                        when '<=' then
                            Interval[
                                null,
                                ToQuantityIgnoringComparator(quantity)
                            ]
                        when '>=' then
                            Interval[
                                ToQuantityIgnoringComparator(quantity),
                                null
                            ]
                        when '>' then
                            Interval(
                                ToQuantityIgnoringComparator(quantity),
                                null
                            ]
                        else
                            Interval[ToQuantity(quantity), ToQuantity(quantity)]
                    end
            
            define function ToInterval(range FHIR.Range):
                if range is null then
                    null
                else
                    Interval[ToQuantity(range.low), ToQuantity(range.high)]

            define function ToInterval(range FHIR.Range):
                if range is null then
                    null
                else
                    Interval[ToQuantity(range.low), ToQuantity(range.high)]
            """);

        writer.WriteLine();
    }

    private void WriteHelperToCalendarUnit(ExportStreamWriter writer)
    {
        writer.WriteLine("""
            define function ToCalendarUnit(unit System.String):
                case unit
                    when 'ms' then 'millisecond'
                    when 's' then 'second'
                    when 'min' then 'minute'
                    when 'h' then 'hour'
                    when 'd' then 'day'
                    when 'wk' then 'week'
                    when 'mo' then 'month'
                    when 'a' then 'year'
                    else unit
                end
            define function ToCalendarUnit(unit System.String):
                case unit
                    when 'ms' then 'millisecond'
                    when 's' then 'second'
                    when 'min' then 'minute'
                    when 'h' then 'hour'
                    when 'd' then 'day'
                    when 'wk' then 'week'
                    when 'mo' then 'month'
                    when 'a' then 'year'
                    else unit
                end
            """);

        writer.WriteLine();
    }

    private void WriteHelperToQuantity(ExportStreamWriter writer)
    {
        writer.WriteLine("""
            define function ToQuantity(quantity FHIR.Quantity):
                case
                    when quantity is null then null
                    when quantity.value is null then null
                    when quantity.comparator is not null then
                        Message(null, true, 'FHIRHelpers.ToQuantity.ComparatorQuantityNotSupported', 'Error', 'FHIR Quantity value has a comparator and cannot be converted to a System.Quantity value.')
                    when quantity.system is null or quantity.system.value = 'http://unitsofmeasure.org'
                          or quantity.system.value = 'http://hl7.org/fhirpath/CodeSystem/calendar-units' then
                        System.Quantity { value: quantity.value.value, unit: ToCalendarUnit(Coalesce(quantity.code.value, quantity.unit.value, '1')) }
                    else
                        Message(null, true, 'FHIRHelpers.ToQuantity.InvalidFHIRQuantity', 'Error', 'Invalid FHIR Quantity code: ' & quantity.unit.value & ' (' & quantity.system.value & '|' & quantity.code.value & ')')
                end

            define function ToQuantityIgnoringComparator(quantity FHIR.Quantity):
                case
                    when quantity is null then null
                    when quantity.value is null then null
                    when quantity.system is null or quantity.system.value = 'http://unitsofmeasure.org'
                          or quantity.system.value = 'http://hl7.org/fhirpath/CodeSystem/calendar-units' then
                        System.Quantity { value: quantity.value.value, unit: ToCalendarUnit(Coalesce(quantity.code.value, quantity.unit.value, '1')) }
                    else
                        Message(null, true, 'FHIRHelpers.ToQuantity.InvalidFHIRQuantity', 'Error', 'Invalid FHIR Quantity code: ' & quantity.unit.value & ' (' & quantity.system.value & '|' & quantity.code.value & ')')
                end
            """);

        writer.WriteLine();
    }

    private void WriteHelperToRatio(ExportStreamWriter writer)
    {
        writer.WriteLine("""
            define function ToRatio(ratio FHIR.Ratio):
                if ratio is null then
                    null
                else
                    System.Ratio { numerator: ToQuantity(ratio.numerator), denominator: ToQuantity(ratio.denominator) }
            define function ToRatio(ratio FHIR.Ratio):
                if ratio is null then
                    null
                else
                    System.Ratio { numerator: ToQuantity(ratio.numerator), denominator: ToQuantity(ratio.denominator) }
            """);

        writer.WriteLine();
    }

    private void WriteHelperToCode(ExportStreamWriter writer)
    {
        writer.WriteLine("""
            define function ToCode(coding FHIR.Coding):
                if coding is null then
                    null
                else
                    System.Code {
                      code: coding.code.value,
                      system: coding.system.value,
                      version: coding.version.value,
                      display: coding.display.value
                    }
            define function ToCode(coding FHIR.Coding):
                if coding is null then
                    null
                else
                    System.Code {
                      code: coding.code.value,
                      system: coding.system.value,
                      version: coding.version.value,
                      display: coding.display.value
                    }
            """);

        writer.WriteLine();
    }

    private void WriteHelperToConcept(ExportStreamWriter writer)
    {
        writer.WriteLine("""
            define function ToConcept(concept FHIR.CodeableConcept):
                if concept is null then
                    null
                else
                    System.Concept {
                        codes: concept.coding C return ToCode(C),
                        display: concept.text.value
                    }
            define function ToConcept(concept FHIR.CodeableConcept):
                if concept is null then
                    null
                else
                    System.Concept {
                        codes: concept.coding C return ToCode(C),
                        display: concept.text.value
                    }
            """);

        writer.WriteLine();
    }

    private void WriteHelperToValueSet(ExportStreamWriter writer)
    {
        writer.WriteLine("""
            define function ToValueSet(uri String):
                if uri is null then
                    null
                else
                    System.ValueSet {
                        id: uri
                    }
            define function ToValueSet(uri String):
                if uri is null then
                    null
                else
                    System.ValueSet {
                        id: uri
                    }
            """);

        writer.WriteLine();
    }

    private void WriteHelperToReference(ExportStreamWriter writer)
    {
        writer.WriteLine("""
            define function reference(reference String):
                if reference is null then
                    null
                else
                    Reference { reference: string { value: reference } }
            define function reference(reference String):
                if reference is null then
                    null
                else
                    Reference { reference: string { value: reference } }
            """);

        writer.WriteLine();
    }

    private void WriteHelperToValue(ExportStreamWriter writer)
    {
        List<string> allTypeNames = [];

        allTypeNames.AddRange(_dc.PrimitiveTypesByName.Keys.OrderBy(x => x));
        allTypeNames.AddRange(_dc.ComplexTypesByName.Keys.OrderBy(x => x));

        writer.WriteIndented("define function ToValue(value Choice<");
        writer.Write(string.Join(",", allTypeNames));
        writer.WriteLine(">):");
        writer.IncreaseIndent();        // open function
        writer.WriteLineIndented("case");
        writer.IncreaseIndent();        // open case

        // traverse primitives
        foreach (StructureDefinition sd in _dc.PrimitiveTypesByName.Values.OrderBy(x => x.Name))
        {
            writer.WriteLineIndented($"when value is {sd.Name} then (value as {sd.Name}).value");
        }

        List<string> unconverted = [];

        // traverse complex types looking for mappings
        foreach (StructureDefinition sd in _dc.ComplexTypesByName.Values.OrderBy(x => x.Name))
        {
            if (_valueTypeConverters.TryGetValue(sd.Name, out string? converterName) ||
                ((sd.BaseDefinition?.StartsWith("http://hl7.org/fhir/StructureDefinition/", StringComparison.Ordinal) ?? false) &&
                 _valueTypeConverters.TryGetValue(sd.BaseDefinition.Split('/')[^1], out converterName)))
            {
                writer.WriteLineIndented($"when value is {sd.Name} then {converterName}(value as {sd.Name})");
                continue;
            }

            unconverted.Add(sd.Name);
        }

        writer.WriteIndented("else value as Choice<");
        writer.Write(string.Join(",", unconverted));
        writer.WriteLine(">");
        writer.DecreaseIndent();        // close case
        writer.WriteLineIndented("end");
        writer.DecreaseIndent();        // close function
        writer.WriteLine();
    }

    private void WriteHelperFhirPathFunctions(ExportStreamWriter writer)
    {
        writer.WriteLine("""
            define function resolve(reference String) returns Resource: external
            define function resolve(reference Reference) returns Resource: external
            define function reference(resource Resource) returns Reference: external
            define function extension(element Element, url String) returns List<Extension>: external
            define function extension(resource DomainResource, url String) returns List<Extension>: external
            define function modifierExtension(element BackboneElement, url String) returns List<Extension>: external
            define function modifierExtension(resource DomainResource, url String) returns List<Extension>: external
            define function hasValue(element Element) returns Boolean: external
            define function getValue(element Element) returns Any: external
            define function ofType(identifier String) returns List<Any>: external
            define function is(identifier String) returns Boolean: external
            define function as(identifier String) returns Any: external
            define function elementDefinition(element Element) returns ElementDefinition: external
            define function slice(element Element, url String, name String) returns List<Element>: external
            define function checkModifiers(resource Resource) returns Resource: external
            define function checkModifiers(resource Resource, modifier String) returns Resource: external
            define function checkModifiers(element Element) returns Element: external
            define function checkModifiers(element Element, modifier String) returns Element: external
            define function conformsTo(resource Resource, structure String) returns Boolean: external
            define function memberOf(code code, valueSet String) returns Boolean: external
            define function memberOf(coding Coding, valueSet String) returns Boolean: external
            define function memberOf(concept CodeableConcept, valueSet String) returns Boolean: external
            define function subsumes(coding Coding, subsumedCoding Coding) returns Boolean: external
            define function subsumes(concept CodeableConcept, subsumedConcept CodeableConcept) returns Boolean: external
            define function subsumedBy(coding Coding, subsumingCoding Coding) returns Boolean: external
            define function subsumedBy(concept CodeableConcept, subsumingConcept CodeableConcept) returns Boolean: external
            define function htmlChecks(element Element) returns Boolean: external
            """);

        writer.WriteLine();
    }

    private void WriteHelperPrimitiveConverters(ExportStreamWriter writer)
    {
        // traverse the primitives
        foreach ((string typeName, string functionName) in _primitiveConverters)
        {
            if (!_dc.PrimitiveTypesByName.ContainsKey(typeName))
            {
                continue;
            }

            writer.WriteLineIndented($"define function {functionName}(value {typeName}): value.value");
        }

        writer.WriteLine();
    }

    private void WriteHelperValueSetCodeConverters(ExportStreamWriter writer)
    {
        HashSet<string> writtenNames = [];

        // traverse all versions of all value sets
        foreach ((string unversionedUrl, string[] versions) in _dc.ValueSetVersions.OrderBy(kvp => kvp.Key))
        {
            if (_exclusionSet.Contains(unversionedUrl))
            {
                continue;
            }

            // traverse value sets starting with highest version
            foreach (string vsVersion in versions.OrderDescending())
            {
                if (!_dc.TryGetValueSet(unversionedUrl, vsVersion, out ValueSet? vs))
                {
                    continue;
                }

                string name = vs.Name.ToPascalCase(delimitersToRemove: _vsNameDelimitersToRemove);

                if (writtenNames.Contains(name))
                {
                    continue;
                }

                IEnumerable<StructureElementCollection> coreBindings = _dc.CoreBindingsForVs(vs.Url);
                IReadOnlyDictionary<string, BindingStrength> bindingDict = _dc.BindingStrengthByType(coreBindings);

                // we only care about required bindings
                if (!bindingDict.TryGetValue("code", out BindingStrength strongest) ||
                    (strongest != Hl7.Fhir.Model.BindingStrength.Required))
                {
                    continue;
                }

                writtenNames.Add(name);

                string value = $"define function ToString(value {name}): value.value";

                // write our conversion function
                writer.WriteLineIndented($"{value,-90}\t// {vs.Url} - {vs.Description}");
            }
        }
    }

    private ExportStreamWriter OpenWriter(string relative)
    {
        ExportStreamWriter writer = new ExportStreamWriter(Path.Combine(_config.OutputDirectory, relative), false);

        return writer;
    }

    private void CloseAndDispose(ExportStreamWriter writer)
    {
        writer.Flush();
        writer.Close();
        writer.Dispose();
    }

    private void WriteHelperHeader(ExportStreamWriter writer)
    {
        writer.WriteLine("/*");

        writer.WriteLine(" * @author: Gino Canessa");
        writer.WriteLine(" * @description: This library defines functions to convert between FHIR ");
        writer.WriteLine(" *  data types and CQL system-defined types, as well as functions to support");
        writer.WriteLine(" *  FHIRPath implementation. For more information, the FHIRHelpers wiki page:");
        writer.WriteLine(" *  https://github.com/cqframework/clinical_quality_language/wiki/FHIRHelpers");
        writer.WriteLine(" * @allowFluent: true");

        writer.WriteLine(" */");
    }
}
