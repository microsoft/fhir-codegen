﻿// <copyright file="LangCql.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;
using Microsoft.Health.Fhir.CodeGen.Loader;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using Microsoft.Health.Fhir.CodeGenCommon.Utils;
using System.Reflection;
using Firely.Fhir.Packages;



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
    private readonly Dictionary<string, string> _exportedValueSets = [];

    private readonly Dictionary<string, string> _valueTypeConverters = new()
    {
        { "CodeableConcept", "ToConcept" },
        { "Coding", "ToCoding" },
        { "Period", "ToInterval" },
        { "Quantity", "ToQuantity" },
        { "Range", "ToInterval" },
        { "Ratio", "ToRatio" },
    };

    private string _name = string.Empty;
    private readonly Dictionary<string, string> _fhirCqlTypeMap = [];
    private CqlFhirParameters? _cqlFhirParameters = null;

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
        { "integer64", "ToLong" },          // all CQL tooling expects 1.5 or higher, so we can use Long
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

        if (FhirPackageUtils.PackageIsFhirCore(definitions.MainPackageId))
        {
            _name = "FHIR";
        }
        else
        {
            _name = FhirSanitizationUtils.SanitizeForProperty(definitions.Name);
        }

        // load the necessary CQL support files
        LoadCqlSupport();

        // write the relevant helper file
        WriteHelperFile();

        // build our CQL Model Info
        ModelInfo cqlModelInfo = BuildModelInfo();

        System.Xml.Serialization.XmlSerializer serializer = new(typeof(ModelInfo));
        string modelInfoFilename = Path.Combine(_config.OutputDirectory, $"{_name}-modelinfo-{_dc.MainPackageVersion}.xml");
        using (StreamWriter writer = new(modelInfoFilename))
        {
            serializer.Serialize(writer, cqlModelInfo);
            writer.Flush();
            writer.Close();
        }
    }

    private ModelInfo BuildModelInfo()
    {
        ModelInfo model = new()
        {
            name = _name,
            version = _dc.MainPackageVersion,
            url = _dc.MainPackageCanonical,
            targetQualifier = _name.ToLowerInvariant(),
        };

        if (_cqlFhirParameters?.ModelProperties.Any() ?? false)
        {
            // Reflect ModelInfo to get the list of properties it has
            PropertyInfo[] properties = typeof(ModelInfo).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                // check to see if we have a value for this property
                if (_cqlFhirParameters.ModelProperties.TryGetValue(property.Name, out string? value))
                {
                    property.SetValue(model, value);
                }
            }
        }

        // always require system
        model.requiredModelInfo = [ new() { name = "System", version = "1.0.0" } ];

        List<Cql.TypeInfo> modelTypes = [];

        // iterate over primitive types only if we are in a FHIR namespace
        if (_name == "FHIR")
        {
            modelTypes.AddRange(ModelTypesForPrimitives(_dc.PrimitiveTypesByName.Values));
        }

        // add the complex types
        modelTypes.AddRange(ModelTypesForStructures(_dc.ComplexTypesByName.Values.Select(sd => new ComponentDefinition(sd)), false));

        // add all our computed types
        model.typeInfo = modelTypes.ToArray();

        // TODO(ginoc): add conversion functions: model.conversionInfo


        // return our model
        return model;
    }

    private List<Cql.ClassInfo> ModelTypesForPrimitives(IEnumerable<StructureDefinition> primitives)
    {
        List<Cql.ClassInfo> modelTypes = [];

        foreach (StructureDefinition sd in primitives)
        {
            // grab the base for this primitive
            string bt = sd.BaseDefinition.Split('/')[^1];

            // create our type info
            Cql.ClassInfo ti = new Cql.ClassInfo()
            {
                baseType = $"{_name}.{bt}",
                @namespace = _name,
                name = sd.Name,
                identifier = sd.Url,
                label = sd.Name,
                retrievable = false,
            };

            // if this is a 'base' primitive, we need to specify the value element
            if (bt == "Element")
            {
                // add the value element
                if (_fhirCqlTypeMap.TryGetValue(sd.Name, out string? cqlType))
                {
                    ti.element = [ new()
                    {
                        name = "value",
                        elementType = cqlType,
                    }];
                }
                else
                {
                    ti.element = [ new()
                    {
                        name = "value",
                        elementType = sd.Name,
                    }];
                }
            }

            // add the type info to our list
            modelTypes.Add(ti);
        }

        return modelTypes;
    }

    private List<Cql.ClassInfo> ModelTypesForStructures(IEnumerable<ComponentDefinition> components, bool isRetrievable = false)
    {
        List<Cql.ClassInfo> modelTypes = [];

        foreach (ComponentDefinition cd in components)
        {
            // grab the base for this primitive
            string bt = cd.IsRootOfStructure
                ? cd.Structure.cgBaseTypeName()
                : cd.Element.cgBaseTypeName(_dc, false);

            // create our type info
            Cql.ClassInfo ti = cd.IsRootOfStructure
                ? new Cql.ClassInfo()
                {
                    baseType = $"{_name}.{bt}",
                    @namespace = _name,
                    name = cd.Structure.Name,
                    identifier = cd.Structure.Url,
                    label = cd.Structure.Name,
                    retrievable = isRetrievable,
                }
                : new Cql.ClassInfo()
                {
                    baseType = $"{_name}.{bt}",
                    @namespace = _name,
                    name = cd.Element.Path,
                    retrievable = isRetrievable,
                };

            List<Cql.ClassInfoElement> cqlElements = [];

            // get the elements for this level of this component
            foreach (ElementDefinition element in cd.cgGetChildren(false, true))
            {
                if (element.cgIsInherited(cd.Structure))
                {
                    continue;
                }

                ComponentDefinition[] subComponents = cd.Structure.cgComponents(_dc, element, false, true).ToArray();

                Cql.ClassInfoElement cqlElement = new()
                {
                    name = element.cgName(),
                };

                IReadOnlyDictionary<string, ElementDefinition.TypeRefComponent> ets = element.cgTypes();

                // check if this is a choice type
                if (ets.Count > 1)
                {
                    List<Cql.NamedTypeSpecifier> choices = [];
                    foreach (ElementDefinition.TypeRefComponent et in ets.Values)
                    {
                        choices.Add(new Cql.NamedTypeSpecifier()
                        {
                            @namespace = _name,
                            name = et.cgName(),
                        });
                    }

                    cqlElement.elementTypeSpecifier = new Cql.ChoiceTypeSpecifier()
                    {
                        choice = choices.ToArray(),
                    };
                }
                // check for simple element
                else if (ets.Count == 1)
                {
                    ElementDefinition.TypeRefComponent et = ets.First().Value;

                    string typeName = et.cgName();

                    // check for bound elements that need to use an exported code type
                    if ((element.Binding?.Strength == Hl7.Fhir.Model.BindingStrength.Required) &&
                        ((typeName == "code") || (typeName == "coding")) &&
                        _exportedValueSets.TryGetValue(element.Binding.ValueSet, out string? exportedName))
                    {
                        typeName = exportedName;
                    }

                    // check cardinality
                    if (element.cgIsArray())
                    {
                        cqlElement.elementTypeSpecifier = new Cql.ListTypeSpecifier()
                        {
                            elementType = $"{_name}.{typeName}",
                        };
                    }
                    else
                    {
                        cqlElement.elementType = $"{_name}.{typeName}";
                    }
                }

                cqlElements.Add(cqlElement);

                // nest through sub-components
                modelTypes.AddRange(ModelTypesForStructures(subComponents, false));
            }

            // add our elements
            if (cqlElements.Count > 0)
            {
                ti.element = cqlElements.ToArray();
            }

            // add the type info to our list
            modelTypes.Add(ti);
        }

        return modelTypes;
    }

    private void LoadCqlSupport()
    {
        if (string.IsNullOrEmpty(_config.CqlSupportDir))
        {
            return;
        }

        PackageLoader loader = new();

        // first, we want to load the type map
        string filename = Path.Combine(_config.CqlSupportDir, $"ConceptMap-cql-fhir-types-{_dc.FhirSequence.ToRLiteral()}.json");

        if (File.Exists(filename))
        {
            object? parsed = loader.ParseContentsPoco("application/fhir+json", filename);
            if (parsed is ConceptMap typeMap)
            {
                _fhirCqlTypeMap.Clear();

                foreach (ConceptMap.GroupComponent group in typeMap.Group)
                {
                    foreach (ConceptMap.SourceElementComponent element in group.Element)
                    {
                        if (element.Target.Count != 1)
                        {
                            continue;
                        }

                        _fhirCqlTypeMap[element.Code] = element.Target[0].Code;
                    }
                }
            }
        }

        // next, look for a properties file (versioned, then unversioned)
        filename = Path.Combine(_config.CqlSupportDir, $"Parameters-cql-{_dc.MainPackageId.Replace('.', '-')}-{_dc.MainPackageVersion.Replace('.', '-')}.json");
        if (!File.Exists(filename))
        {
            filename = Path.Combine(_config.CqlSupportDir, $"Parameters-cql-{_dc.MainPackageId.Replace('.', '-')}.json");
        }
        if (File.Exists(filename))
        {
            object? parsed = loader.ParseContentsPoco("application/fhir+json", filename);
            if (parsed is Parameters cqlFhirParams)
            {
                _cqlFhirParameters = new();
                foreach (Parameters.ParameterComponent param in cqlFhirParams.Parameter)
                {
                    switch (param.Name)
                    {
                        case "context":
                            {
                                foreach (Parameters.ParameterComponent inner in param.Part)
                                {
                                    switch (inner.Name)
                                    {
                                        case "packageId":
                                            {
                                                _cqlFhirParameters.PackageId = (inner.Value is FhirString fs) ? fs.Value : string.Empty;
                                            }
                                            break;

                                        case "packageVersion":
                                            {
                                                _cqlFhirParameters.PackageVersion = (inner.Value is FhirString fs) ? fs.Value : string.Empty;
                                            }
                                            break;
                                    }
                                }
                            }
                            break;

                        case "supersedes":
                            {
                                string supersedesPackage = string.Empty;
                                string supersedesVersion = string.Empty;

                                foreach (Parameters.ParameterComponent inner in param.Part)
                                {
                                    switch (inner.Name)
                                    {
                                        case "packageId":
                                            {
                                                supersedesPackage = (inner.Value is FhirString fs) ? fs.Value : string.Empty;
                                            }
                                            break;

                                        case "packageVersion":
                                            {
                                                supersedesVersion = (inner.Value is FhirString fs) ? "@" + fs.Value : string.Empty;
                                            }
                                            break;
                                    }
                                }

                                if (!string.IsNullOrEmpty(supersedesPackage))
                                {
                                    _cqlFhirParameters.Supersedes.Add(supersedesPackage + supersedesPackage);
                                }
                            }
                            break;

                        case "modelProperties":
                            {
                                foreach (Parameters.ParameterComponent inner in param.Part)
                                {
                                    string key = inner.Name;
                                    string value = (inner.Value is FhirString fs) ? fs.Value : string.Empty;

                                    if (!string.IsNullOrEmpty(value))
                                    {
                                        _cqlFhirParameters.ModelProperties.Add(key, value);
                                    }
                                }
                            }
                            break;

                        // everything else is should be a structure canonical
                        default:
                            {
                                string canonical = param.Name;
                                Dictionary<string, string> sProps = [];

                                foreach (Parameters.ParameterComponent inner in param.Part)
                                {
                                    string key = inner.Name;
                                    string value = (inner.Value is FhirString fs) ? fs.Value : string.Empty;

                                    if (!string.IsNullOrEmpty(value))
                                    {
                                        sProps.Add(key, value);
                                    }
                                }

                                _cqlFhirParameters.StructureProperties.Add(canonical, sProps);
                            }
                            break;
                    }
                }

            }
        }

    }

    private void WriteHelperFile()
    {
        string packageVersion = _dc.MainPackageVersion.Split('-')[0];
        string fhirVersion = _dc.FhirVersionLiteral;

        ExportStreamWriter writer = OpenWriter($"{_name}Helpers-{packageVersion}.cql");
        WriteHelperHeader(writer);

        writer.WriteLineIndented($"library {_name}Helpers version '{packageVersion}'");
        writer.WriteLine();
        writer.WriteLineIndented($"using FHIR version '{fhirVersion}'");
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
                IReadOnlyDictionary<string, Hl7.Fhir.Model.BindingStrength> bindingDict = _dc.BindingStrengthByType(coreBindings);

                // we only care about required bindings
                if (!bindingDict.TryGetValue("code", out Hl7.Fhir.Model.BindingStrength strongest) ||
                    (strongest != Hl7.Fhir.Model.BindingStrength.Required))
                {
                    continue;
                }

                writtenNames.Add(name);

                string value = $"define function ToString(value {name}): value.value";

                // write our conversion function
                writer.WriteLineIndented($"{value,-90}\t// {vs.Url} - {vs.Description}");

                // add to tracking
                _exportedValueSets[vs.Url] = name;
                _exportedValueSets[vs.Url + "|" + vsVersion] = name;
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