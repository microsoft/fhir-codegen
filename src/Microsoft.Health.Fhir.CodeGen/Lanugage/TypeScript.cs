// <copyright file="TypeScript.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using static Microsoft.Health.Fhir.CodeGenCommon.Extensions.FhirNameConventionExtensions;
using System.Runtime.Serialization;
using Microsoft.Health.Fhir.CodeGen.Extensions;
using Microsoft.Health.Fhir.CodeGen.Configuration;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;
using Microsoft.Health.Fhir.CodeGen.Utils;

namespace Microsoft.Health.Fhir.CodeGen.Lanugage;

public class TypeScript : ILanguage<TypeScript.TypeScriptOptions>
{
    private const string _defaultNamespace = "fhir{VersionNumber}";
    private const string _defaultMinTsVersion = "3.7";

    public class TypeScriptOptions : ConfigGenerate
    {
        /// <summary>Gets or sets the namespace.</summary>
        [ConfigOption(
            ArgName = "--namespace",
            Description = "Base namespace for TypeScript files, default is 'fhir{VersionNumber}', use '' (empty string) for none.")]
        public string Namespace { get; set; } = _defaultNamespace;

        /// <summary>Gets or sets the minimum ts version.</summary>
        [ConfigOption(
            ArgName = "--min-ts-version",
            Description = "Minimum TypeScript version, use '' (empty string) for none.")]
        public string MinTsVersion { get; set; } = _defaultMinTsVersion;

        /// <summary>Gets or sets the write stream to use.</summary>
        public Stream? WriteStream { get; set; } = null;
    }

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

        /// <summary>Display are often just symbols.</summary>
        "http://hl7.org/fhir/v2/0290",

        /// <summary>Display includes too many Unicode characters (invalid export names).</summary>
        "http://hl7.org/fhir/v2/0255",

        /// <summary>Display includes too many Unicode characters (invalid export names).</summary>
        "http://hl7.org/fhir/v2/0256",
    };

    /// <summary>FHIR information we are exporting.</summary>
    private DefinitionCollection _dc = null!;

    /// <summary>Options for controlling the export.</summary>
    private TypeScriptOptions _options = null!;

    /// <summary>True to export enums.</summary>
    private bool _exportEnums;

    /// <summary>
    /// True if we should write a namespace directive
    /// </summary>
    private bool _includeNamespace = false;

    /// <summary>
    /// The namespace to use.
    /// </summary>
    private string _namespace = string.Empty;

    /// <summary>The exported codes.</summary>
    private HashSet<string> _exportedCodes = new HashSet<string>();

    /// <summary>The exported resources.</summary>
    private List<string> _exportedResources = new List<string>();

    /// <summary>The currently in-use text writer.</summary>
    private ExportStreamWriter _writer = null!;

    /// <summary>The single file export extension.</summary>
    private const string _singleFileExportExtension = ".ts";

    /// <summary>The minimum type script version.</summary>
    private string _minimumTypeScriptVersion = "3.7";

    /// <summary>Dictionary mapping FHIR primitive types to language equivalents.</summary>
    private static readonly Dictionary<string, string> _primitiveTypeMap = new Dictionary<string, string>()
        {
            { "base", "Object" },
            { "base64Binary", "string" },
            { "boolean", "boolean" },
            { "canonical", "string" },
            { "code", "string" },
            { "date", "string" },
            { "dateTime", "string" },
            { "decimal", "number" },
            { "id", "string" },
            { "instant", "string" },
            { "integer", "number" },
            { "integer64", "string" },       // int64 serializes as string, need to add custom handling here
            { "markdown", "string" },
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
    private static readonly HashSet<string> _reservedWords = new()
    {
        "const",
        "enum",
        "export",
        "interface",
    };

    private static readonly HashSet<string> _unexportableSystems = new()
    {
        "http://www.rfc-editor.org/bcp/bcp13.txt",
        "http://hl7.org/fhir/ValueSet/mimetype",
        "http://hl7.org/fhir/ValueSet/mimetypes",
        "http://hl7.org/fhir/ValueSet/ucum-units",
    };

    /// <summary>The generics and type hints.</summary>
    private static readonly Dictionary<string, GenericTypeHintInfo> _genericsAndTypeHints = new Dictionary<string, GenericTypeHintInfo>()
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

    /// <summary>Gets the name of the language.</summary>
    /// <value>The name of the language.</value>
    public string Name => "TypeScript";

    /// <summary>Gets the FHIR primitive type map.</summary>
    /// <value>The FHIR primitive type map.</value>
    public Dictionary<string, string> FhirPrimitiveTypeMap => _primitiveTypeMap;

    /// <summary>Gets a value indicating whether this language is idempotent.</summary>
    public bool IsIdempotent => true;

    /// <summary>Gets the reserved words.</summary>
    /// <value>The reserved words.</value>
    public HashSet<string> ReservedWords => _reservedWords;

    /// <summary>Export the passed FHIR version into the specified directory.</summary>
    /// <param name="info">           The information.</param>
    /// <param name="serverInfo">     Information describing the server.</param>
    /// <param name="options">        Options for controlling the operation.</param>
    /// <param name="exportDirectory">Directory to write files.</param>
    public void Export(
        TypeScriptOptions config,
        DefinitionCollection definitions)
    {
        // set internal vars so we don't pass them to every function
        // this is ugly, but the interface patterns get bad quickly because we need the type map to copy the FHIR info
        _dc = definitions;
        _options = config;

        _includeNamespace = string.IsNullOrEmpty(config.Namespace);

        _namespace = config.Namespace.Equals(_defaultNamespace, StringComparison.Ordinal)
            ? $"fhir{definitions.FhirSequence.ToRLiteral()}"
            : config.Namespace;

        _minimumTypeScriptVersion = config.MinTsVersion;

        _exportedCodes = new HashSet<string>();
        _exportedResources = new List<string>();

        if (config.ExportStructures.Contains(CodeGenCommon.Models.FhirArtifactClassEnum.ValueSet))
        {
            _exportEnums = true;
        }
        else
        {
            _exportEnums = false;
        }

        // create a filename for writing (single file for now)
        string filename = Path.Combine(config.OutputDirectory, $"{definitions.FhirSequence.ToRLiteral()}.ts");

        using (config.WriteStream == null
            ? _writer = new ExportStreamWriter(new FileStream(filename, FileMode.Create), System.Text.Encoding.UTF8, 1024, true)
            : _writer = new ExportStreamWriter(config.WriteStream, System.Text.Encoding.UTF8, 1024, true))
        {
            WriteHeader();

            WriteStructures(_dc.ComplexTypesByName.Values, false);
            WriteStructures(_dc.ResourcesByName.Values, true);

            if (_exportEnums)
            {
                WriteValueSets(_dc.ValueSetsByVersionedUrl.Values);
            }

            WriteExpandedResourceInterfaceBinding();

            WriteFooter();
        }
    }

    /// <summary>Writes the expanded resource interface binding.</summary>
    private void WriteExpandedResourceInterfaceBinding()
    {
        if (_exportedResources.Count == 0)
        {
            return;
        }

        _exportedResources.Sort();

        WriteIndentedComment("Resource binding for generic use.");

        if (_exportedResources.Count == 1)
        {
            _writer.WriteLineIndented($"export type FhirResource = {_exportedResources[0]};");
            return;
        }

        _writer.WriteLineIndented("export type FhirResource = ");

        _writer.IncreaseIndent();

        int index = 0;
        int last = _exportedResources.Count - 1;
        foreach (string exportedName in _exportedResources)
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
    /// <param name="valueSets">List of valueSetCollections.</param>
    private void WriteValueSets(
        IEnumerable<ValueSet> valueSets)
    {
        Dictionary<string, WrittenCodeInfo> writtenCodesAndNames = new Dictionary<string, WrittenCodeInfo>();
        HashSet<string> writtenNames = new HashSet<string>();

        foreach (ValueSet vs in valueSets.OrderBy(v => v.Url))
        {
            // only write expansions

            ValueSet? expanded = _dc.ExpandVs(vs.Url).Result;

            if (expanded == null)
            {
                continue;
            }

            WriteValueSet(
                expanded,
                ref writtenCodesAndNames,
                ref writtenNames);
        }
    }

    /// <summary>Writes a value set.</summary>
    /// <param name="vs">                  The value set.</param>
    /// <param name="writtenCodesAndNames">[in,out] The written codes, to prevent duplication
    ///  without writing all code systems.</param>
    /// <param name="writtenNames">        [in,out] List of names of the writtens.</param>
    private void WriteValueSet(
        ValueSet vs,
        ref Dictionary<string, WrittenCodeInfo> writtenCodesAndNames,
        ref HashSet<string> writtenNames)
    {
        string vsName = FhirSanitizationUtils.SanitizeForProperty(vs.Id ?? vs.Name, _reservedWords).ToPascalCase();

        foreach (ValueSet.ContainsComponent contains in vs.Expansion.Contains.OrderBy(c => c.Code))
        {
            string key = contains.cgKey();
            if (writtenCodesAndNames.ContainsKey(key))
            {
                continue;
            }

            string input = contains.Display;
            if (_systemsNamedByDisplay.Contains(contains.System))
            {
                input = contains.Display;
            }
            else if (_systemsNamedByCode.Contains(contains.System))
            {
                input = contains.Code;
            }
            else if (string.IsNullOrEmpty(input))
            {
                input = contains.Code;
            }

            string codeName = FhirSanitizationUtils.SanitizeForProperty(input, _reservedWords).ToPascalCase(true);
            string codeValue = FhirSanitizationUtils.SanitizeForValue(contains.Code);

            string systemLocalName = contains.cgSystemLocalName();

            string constName;
            if (!string.IsNullOrEmpty(systemLocalName))
            {
                constName = $"{systemLocalName}_{codeName}";
            }
            else
            {
                constName = $"{vsName}_{codeName}";
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
                key,
                new WrittenCodeInfo() { Name = codeName, ConstName = constName });
            writtenNames.Add(constName);

            _writer.WriteLineIndented($"const {constName}: Coding = {{");
            _writer.IncreaseIndent();

            _writer.WriteLineIndented($"code: \"{codeValue}\",");

            if (!string.IsNullOrEmpty(contains.Display))
            {
                _writer.WriteLineIndented($"display: \"{FhirSanitizationUtils.SanitizeForQuoted(contains.Display)}\",");
            }

            _writer.WriteLineIndented($"system: \"{contains.System}\"");

            _writer.DecreaseIndent();

            _writer.WriteLineIndented("};");
        }

        if (!string.IsNullOrEmpty(vs.Description))
        {
            WriteIndentedComment(vs.Description);
        }
        else
        {
            WriteIndentedComment($"Value Set: {_dc.VersionedUrlForVs(vs.Url)}");
        }

        _writer.WriteLineIndented($"export const {vsName} = {{");
        _writer.IncreaseIndent();

        bool prefixWithSystem = vs.cgReferencedCodeSystems().Count() > 1;
        HashSet<string> usedValues = new HashSet<string>();

        // TODO: shouldn't loop over this twice, but writer functions don't allow writing in two places at once yet
        foreach (ValueSet.ContainsComponent contains in vs.Expansion.Contains.OrderBy(c => c.Code))
        {
            string codeKey = contains.cgKey();
            string systemLocalName = contains.cgSystemLocalName();
            string definition = _dc.ConceptDefinition(contains.System, contains.Code);

            if (!string.IsNullOrEmpty(definition))
            {
                WriteIndentedComment(definition);
            }

            string name;

            if (prefixWithSystem)
            {
                name = $"{writtenCodesAndNames[codeKey].Name}_{systemLocalName}";
            }
            else
            {
                name = writtenCodesAndNames[codeKey].Name;
            }

            if (usedValues.Contains(name))
            {
                // start at 2 so that the unadorned version makes sense as v1
                for (int i = 2; i < 1000; i++)
                {
                    if (usedValues.Contains($"{name}_{i}"))
                    {
                        continue;
                    }

                    name = $"{name}_{i}";
                    break;
                }
            }

            usedValues.Add(name);

            _writer.WriteLineIndented($"{name}: {writtenCodesAndNames[codeKey].ConstName},");
        }

        _writer.DecreaseIndent();

        _writer.WriteLineIndented("};");
    }

    /// <summary>Writes each of the StructureDefinitions in the enumeration.</summary>
    /// <param name="structures"> The Structure Definitions.</param>
    /// <param name="isResource">(Optional) True if is resource, false if not.</param>
    private void WriteStructures(
        IEnumerable<StructureDefinition> structures,
        bool isResource = false)
    {
        foreach (StructureDefinition sd in structures.OrderBy(c => c.Name))
        {
            // get each component
            foreach (ComponentDefinition component in sd.cgComponents(_dc, null, true, false).OrderBy(c => c.Element?.Path ?? string.Empty, FhirDotNestComparer.Instance))
            {
                WriteInterface(component, isResource && !component.Element.Path.Contains('.'));
            }
        }
    }

    /// <summary>Writes a StructureDefinition.</summary>
    /// <param name="cd">        The ComponentDefinition we are writing from.</param>
    /// <param name="isResource">True if is resource, false if not.</param>
    private void WriteInterface(
        ComponentDefinition cd,
        bool isResource)
    {
        string exportName;
        string typeName;

        if (cd.IsRootOfStructure)
        {
            WriteIndentedComment(cd.Structure.cgpComment());

            // get the base type name from the root element
            typeName = cd.Structure.cgBaseTypeName(_dc, _primitiveTypeMap);
        }
        else
        {
            string comment = string.Join('\n', cd.Element.Short, cd.Element.Comment).Trim();
            WriteIndentedComment(comment);

            // get the base type name from the root element of this path
            typeName = cd.Element.cgBaseTypeName(_dc, _primitiveTypeMap);
        }

        if (string.IsNullOrEmpty(typeName) || cd.Element.Path.Equals(typeName, StringComparison.Ordinal))
        {
            exportName = cd.Element.cgNameForExport(NamingConvention.PascalCase);
            _writer.WriteLineIndented($"export interface {exportName} {{");
        }
        else
        {
            exportName = cd.Element.cgNameForExport(NamingConvention.PascalCase, true);
            string exportTypeName = typeName.ToPascalCase();

            if (_genericsAndTypeHints.TryGetValue(cd.Element.Path, out GenericTypeHintInfo hint))
            {
                _writer.WriteLineIndented(
                    $"export interface" +
                    $" {exportName}<{hint.Alias} = {hint.GenericHint}>" +
                    $" extends {exportTypeName} {{");
            }
            else
            {
                _writer.WriteLineIndented($"export interface {exportName} extends {exportTypeName} {{");
            }
        }

        _writer.IncreaseIndent();

        if (isResource)
        {
            if (ShouldWriteResourceType(cd.Structure.Name))
            {
                _exportedResources.Add(exportName);

                _writer.WriteLineIndented("/** Resource Type Name (for serialization) */");
                _writer.WriteLineIndented($"readonly resourceType: '{cd.Structure.Name}';");
            }
            else
            {
                _writer.WriteLineIndented("/** Resource Type Name (for serialization) */");
                _writer.WriteLineIndented($"readonly resourceType: string;");
            }
        }

        // write elements
        WriteElements(cd, out List<ElementDefinition> elementsWithCodes);

        _writer.DecreaseIndent();

        // close interface (type)
        _writer.WriteLineIndented("}");

        if (_exportEnums)
        {
            foreach (ElementDefinition element in elementsWithCodes)
            {
                WriteCodes(element);
            }
        }
    }

    /// <summary>Writes a code.</summary>
    /// <param name="element">The element.</param>
    private void WriteCodes(
        ElementDefinition element)
    {
        string codeName = FhirSanitizationUtils.ToConvention(
            $"{element.Path}.Codes",
            string.Empty,
            NamingConvention.PascalCase);

        if (codeName.Contains("[x]"))
        {
            codeName = codeName.Replace("[x]", string.Empty);
        }

        if (_exportedCodes.Contains(codeName))
        {
            return;
        }

        _exportedCodes.Add(codeName);

        _writer.WriteLineIndented($"/**");
        _writer.WriteLineIndented($" * Code Values for the {element.Path} field");
        _writer.WriteLineIndented($" */");

        _writer.WriteLineIndented($"export enum {codeName} {{");

        _writer.IncreaseIndent();

        foreach (string code in element.cgCodes(_dc))
        {
            FhirSanitizationUtils.SanitizeForCode(code, _reservedWords, out string name, out string value);

            _writer.WriteLineIndented($"{name.ToUpperInvariant()} = \"{value}\",");
        }

        _writer.DecreaseIndent();

        _writer.WriteLineIndented("}");
    }

    /// <summary>Determine if we should write resource name.</summary>
    /// <param name="name">The name.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private static bool ShouldWriteResourceType(string name)
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

    /// <summary>Determine if the export should support generics</summary>
    /// <param name="name">The name.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private static bool ShouldSupportGenerics(string name)
    {
        switch (name)
        {
            case "Bundle":
            case "Bundle.entry":
            case "Bundle.entry.resource":
                return true;
        }

        return false;
    }

    /// <summary>Writes the elements.</summary>
    /// <param name="cd">          The complex.</param>
    /// <param name="elementsWithCodes">[out] The elements with codes.</param>
    private void WriteElements(
        ComponentDefinition cd,
        out List<ElementDefinition> elementsWithCodes)
    {
        elementsWithCodes = new List<ElementDefinition>();

        foreach (ElementDefinition ed in cd.Structure.cgElements(cd.Element.Path, true, false).OrderBy(e => e.Path))
        {
            if (ed.cgIsInherited(cd.Structure))
            {
                continue;
            }

            WriteElement(ed);

            if (ed.cgHasCodes())
            {
                elementsWithCodes.Add(ed);
            }
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
    private Dictionary<string, string> NamesAndTypesForExport(
        ElementDefinition ed,
        NamingConvention nameConvention = NamingConvention.CamelCase,
        NamingConvention typeConvention = NamingConvention.PascalCase,
        bool concatenatePath = false,
        string concatenationDelimiter = "")
    {
        Dictionary<string, string> values = new Dictionary<string, string>();

        string baseName = ed.cgName();
        bool isChoice = false;

        // check for a generic type hint override
        if (_genericsAndTypeHints.ContainsKey(ed.Path))
        {
            values.Add(
                FhirSanitizationUtils.ToConvention(baseName, ed.Path, nameConvention, concatenatePath, concatenationDelimiter),
                FhirSanitizationUtils.ToConvention(ed.Path, string.Empty, typeConvention));

            return values;
        }

        IReadOnlyDictionary<string, ElementDefinition.TypeRefComponent> elementTypes = ed.cgTypes();

        if (!elementTypes.Any())
        {
            // check for a backbone element
            if (_dc.HasChildElements(ed.Path))
            {
                values.Add(
                    FhirSanitizationUtils.ToConvention(baseName, ed.Path, nameConvention, concatenatePath, concatenationDelimiter),
                    FhirSanitizationUtils.ToConvention(ed.Path, string.Empty, typeConvention));

                return values;
            }

            // if there are no types, use the base type
            values.Add(
                FhirSanitizationUtils.ToConvention(baseName, ed.Path, nameConvention, concatenatePath, concatenationDelimiter),
                FhirSanitizationUtils.ToConvention(ed.cgBaseTypeName(_dc, _primitiveTypeMap), string.Empty, typeConvention));

            return values;
        }

        if (baseName.Contains("[x]", StringComparison.OrdinalIgnoreCase))
        {
            baseName = baseName.Replace("[x]", string.Empty, StringComparison.OrdinalIgnoreCase);
            isChoice = true;
        }

        if (isChoice)
        {
            foreach (ElementDefinition.TypeRefComponent elementType in elementTypes.Values.OrderBy(et => et.Code))
            {
                string name = FhirSanitizationUtils.ToConvention(baseName, ed.Path, nameConvention, concatenatePath, concatenationDelimiter);
                string type = _primitiveTypeMap.TryGetValue(elementType.cgName(), out string? mapped)
                    ? mapped
                    : FhirSanitizationUtils.ToConvention(elementType.cgName(), string.Empty, typeConvention);

                string combined = $"{name}{type}";

                if (!values.ContainsKey(combined))
                {
                    values.Add(combined, type);
                }
            }
        }
        else
        {
            string types = string.Empty;

            foreach (ElementDefinition.TypeRefComponent elementType in elementTypes.Values.OrderBy(et => et.Code))
            {
                string type = _primitiveTypeMap.TryGetValue(elementType.cgName(), out string? mapped)
                    ? mapped
                    : elementType.cgName();

                if (string.IsNullOrEmpty(types))
                {
                    types = type;
                }
                else
                {
                    types += "|" + type;
                }
            }

            if (types.Equals("BackboneElement", StringComparison.Ordinal))
            {
                values.Add(
                    FhirSanitizationUtils.ToConvention(baseName, ed.Path, nameConvention, concatenatePath, concatenationDelimiter),
                    FhirSanitizationUtils.ToConvention(ed.Path, string.Empty, typeConvention));

                return values;
            }

            string cased = FhirSanitizationUtils.ToConvention(baseName, string.Empty, nameConvention, concatenatePath, concatenationDelimiter);

            if (!values.ContainsKey(cased))
            {
                values.Add(cased, types);
            }
        }

        return values;
    }

    /// <summary>Writes an element.</summary>
    /// <param name="cd">The ComponentDefinition we are writing from.</param>
    /// <param name="ed">The ElementDefinition we are writing.</param>
    private void WriteElement(
        ElementDefinition ed)
    {
        bool isOptional = ed.cgIsOptional();

        string optionalFlagString = isOptional ? "?" : string.Empty;
        string optionalSuffixString = isOptional ? " | undefined" : string.Empty;
        string arrayFlagString = ed.cgIsArray() ? "[]" : string.Empty;

        Dictionary<string, string> values = NamesAndTypesForExport(ed);

        if ((values.Count > 1) &&
            (!ed.cgIsOptional()) &&
            string.IsNullOrEmpty(optionalFlagString))
        {
            optionalFlagString = "?";
            optionalSuffixString = " | undefined";
        }

        foreach ((string exportName, string elementTypes) in values)
        {
            string comment = string.Join('\n', ed.Definition, ed.Comment).Trim();
            WriteIndentedComment(comment);

            IEnumerable<string> codes = ed.cgCodes(_dc);

            // Use generated enum for codes when required strength
            // EXCLUDE the MIME type value set - those should be bound to strings
            if (codes.Any() &&
                (!string.IsNullOrEmpty(ed.Binding.ValueSet)) &&
                (!_unexportableSystems.Contains(_dc.UnversionedUrlForVs(ed.Binding.ValueSet))))
            {
                if (_exportEnums)
                {
                    // If we are building enum, reference
                    string codeName = $"{ed.Path}.Codes".ToPascalCase();

                    _writer.WriteLineIndented($"{exportName}{optionalFlagString}: {codeName}{arrayFlagString}{optionalSuffixString};");
                }
                else if (_dc.TryExpandVs(ed.Binding.ValueSet, out ValueSet? vs))
                {
                    // use the full expansion
                    _writer.WriteLineIndented($"{exportName}{optionalFlagString}: ({string.Join("|", vs.Expansion.Contains.Select(c => $"'{c.Code}'"))}){arrayFlagString}{optionalSuffixString};");
                }
                else
                {
                    // otherwise, inline the required codes
                    _writer.WriteLineIndented($"{exportName}{optionalFlagString}: ({string.Join("|", codes.Select(c => $"'{c}'"))}){arrayFlagString}{optionalSuffixString};");
                }
            }
            else if (_genericsAndTypeHints.TryGetValue(ed.Path, out GenericTypeHintInfo typeHint))
            {
                if (typeHint.IncludeBase)
                {
                    _writer.WriteLineIndented(
                        $"{exportName}{optionalFlagString}:" +
                        $" {elementTypes}" +
                        $"<{typeHint.Alias}>{arrayFlagString}{optionalSuffixString};");
                }
                else
                {
                    _writer.WriteLineIndented(
                        $"{exportName}{optionalFlagString}:" +
                        $" {_genericsAndTypeHints[ed.Path].Alias}{arrayFlagString}{optionalSuffixString};");
                }
            }
            else if (elementTypes.Equals("Resource", StringComparison.Ordinal))
            {
                _writer.WriteLineIndented($"{exportName}{optionalFlagString}: FhirResource{arrayFlagString}{optionalSuffixString};");
            }
            else
            {
                _writer.WriteLineIndented($"{exportName}{optionalFlagString}: {elementTypes}{arrayFlagString}{optionalSuffixString};");
            }

            if (RequiresPrimitiveExtension(elementTypes))
            {
                _writer.WriteLineIndented($"_{exportName}?: Element{arrayFlagString} | undefined;");
            }
        }
    }

    /// <summary>Requires extension.</summary>
    /// <param name="typeName">Name of the type.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
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

    /// <summary>Writes a header.</summary>
    private void WriteHeader()
    {
        _writer.WriteLineIndented("// <auto-generated/>");
        _writer.WriteLineIndented($"// Contents of: {string.Join(", ", _dc.Manifests.Select(kvp => kvp.Key))}");
        _writer.WriteLineIndented($"  // Primitive Naming Style: {NamingConvention.None}");
        _writer.WriteLineIndented($"  // Complex Type / Resource Naming Style: {NamingConvention.PascalCase}");
        _writer.WriteLineIndented($"  // Interaction Naming Style: {NamingConvention.None}");
        //_writer.WriteLineIndented($"  // Extension Support: {_options.ExtensionSupport}");

        if (_options.ExportStructures.Any())
        {
            string restrictions = string.Join("|", _options.ExportStructures);
            _writer.WriteLine($"  // Export structures: {restrictions}");
        }

        if (_options.ExportKeys.Any())
        {
            string restrictions = string.Join("|", _options.ExportKeys);
            _writer.WriteLine($"  // Restricted to: {restrictions}");
        }

        foreach (System.Reflection.PropertyInfo prop in _options.GetType().GetProperties())
        {
            object? val = prop.GetValue(_options, null);

            switch (val)
            {
                case bool b:
                    _writer.WriteLineIndented($"  // Option: \"{prop.Name}\" = \"{b}\"");
                    break;

                case string s:
                    _writer.WriteLineIndented($"  // Option: \"{prop.Name}\" = \"{s}\"");
                    break;

                case IEnumerable<string> es:
                    _writer.WriteLineIndented($"  // Option: \"{prop.Name}\" = \"{string.Join(", ", es)}\"");
                    break;
            }
        }

        if (!string.IsNullOrEmpty(_minimumTypeScriptVersion))
        {
            _writer.WriteLine($"// Minimum TypeScript Version: {_minimumTypeScriptVersion}");
        }

        if (_includeNamespace)
        {
            _writer.WriteLineIndented($"export as namespace {_namespace};");
        }
    }

    /// <summary>Writes a footer.</summary>
    private void WriteFooter()
    {
        return;
    }

    /// <summary>Writes an indented comment.</summary>
    /// <param name="value">The value.</param>
    private void WriteIndentedComment(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        _writer.WriteLineIndented($"/**");

        string comment = value.Replace('\r', '\n').Replace("\r\n", "\n", StringComparison.Ordinal).Replace("\n\n", "\n", StringComparison.Ordinal);

        string[] lines = comment.Split('\n');
        foreach (string line in lines)
        {
            _writer.WriteIndented(" * ");
            _writer.WriteLine(line);
        }

        _writer.WriteLineIndented($" */");
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

