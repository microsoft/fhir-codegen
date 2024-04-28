// <copyright file="FirelyNetIG.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.CommandLine;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Microsoft.Health.Fhir.CodeGen.Configuration;
using Microsoft.Health.Fhir.CodeGen.Extensions;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGen.Utils;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;

using static Microsoft.Health.Fhir.CodeGen.Language.Firely.CSharpFirelyCommon;
using static Microsoft.Health.Fhir.CodeGenCommon.Extensions.FhirNameConventionExtensions;

namespace Microsoft.Health.Fhir.CodeGen.Language.Firely;

public class FirelyNetIG : ILanguage
{
    /// <summary>(Immutable) Name of the language.</summary>
    private const string LanguageName = "FirelyNetIg";

    /// <summary>Gets the language name.</summary>
    public string Name => LanguageName;

    /// <summary>Gets the FHIR primitive type map.</summary>
    public Dictionary<string, string> FhirPrimitiveTypeMap => CSharpFirelyCommon.PrimitiveTypeMap;

    /// <summary>Gets a value indicating whether this language is idempotent.</summary>
    public bool IsIdempotent => true;

    /// <summary>The definition writers.</summary>
    private Dictionary<string, ExportStreamWriter> _definitionWriters = [];

    /// <summary>The extension writers.</summary>
    private Dictionary<string, ExportStreamWriter> _extensionWriters = [];

    /// <summary>The value set writers.</summary>
    private Dictionary<string, ExportStreamWriter> _valueSetWriters = [];

    private const string _classNameDefinitions = "Definitions";
    private const string _classNameExtensions = "ExtensionUtils";
    private const string _classNameValueSets = "ValueSetUtils";
    private const string _namespaceSuffixProfiles = ".Profiles";

    private const string _extUrlPrefix = "ExtUrl";
    private const string _processingValuePrefix = "val";
    private const string _processingExtPrefix = "ext";
    private const string _processingExtAddPrefix = "addExt";
    private const string _processingArraySuffix = "Rec";
    private const string _recordClassSuffix = "Record";
    private const string _recordValueSuffix = "";

    private const string _extGetterPrefixScalar = "Get an Extension representing the ";
    private const string _extGetterPrefixArray = "Get any Extensions representing the ";

    private const string _extValueGetterPrefixScalar = "Get the ";
    private const string _extValueGetterPrefixArray = "Get all of the ";

    private const string _extSetterPrefixScalar = "Set an Extension representing the ";
    private const string _extSetterPrefixArray = "Set any Extensions representing the ";

    private const string _extValueSetterPrefixScalar = "Set the ";
    private const string _extValueSetterPrefixArray = "Set all of the ";

    private Dictionary<string, CSharpFirely2.WrittenValueSetInfo> _valueSetInfoByUrl = [];
    private Dictionary<string, string> _valueSetNameMaps = [];

    /// <summary>The package data by directive.</summary>
    private Dictionary<string, PackageData> _packageDataByDirective = [];
    private Dictionary<string, HashSet<string>> _extensionNamesByPackageDirective = [];

    private record struct PackageData
    {
        public required string Key { get; set; }
        public required string PackageId { get; init; }
        public required string PackageVersion { get; init; }
        public required string Namespace { get; init; }
        public required string ClassPrefix { get; init; }
        public required string VersionSanitized { get; init; }
        public required string FolderName { get; init; }
    }

    /// <summary>URL of the extension data by.</summary>
    private Dictionary<string, ExtensionData> _extensionDataByUrl = [];

    private record class ExtensionData()
    {
        public required ComponentDefinition Component { get; init; }
        public required string Name { get; init; }
        public required string Url { get; init; }
        public required string Summary { get; init; }
        public required string Remarks { get; init; }
        public required bool IsList { get; init; }
        public required string ParentName { get; init; }
        public required string DefinitionDirective { get; init; }
        public required ExtensionContextRec[] Contexts { get; init; }
        public required ElementDefinition? ValueElement { get; init; }
        public required CSharpFirely2.WrittenElementInfo? ElementInfo { get; init; }
        public required ExtensionData[] Children { get; init; }
    }

    private record class ExtensionContextRec
    {
        public required StructureDefinition.ContextComponent AllowedContext { get; init; }

        public required ComponentDefinition? ContextTarget { get; init; }

        public required CSharpFirely2.WrittenElementInfo? ContextElementInfo { get; init; }
    }


    /// <summary>FHIR information we are exporting.</summary>
    private DefinitionCollection _info = null!;

    /// <summary>Options for controlling the export.</summary>
    private FirelyNetIGOptions _options = null!;

    public enum ExtensionAccessorExportCodes
    {
        [Description("Do not export export extension accessor functions.")]
        None,

        [Description("Export extension accessor functions based on Extension objects.")]
        ExtensionAccessors,

        //[Description("Export extension accessor functions that use tuples for values.")]
        //ValueTupleAccessors,

        [Description("Export extension accessor functions that use record classes for values.")]
        ValueRecordAccessors,
    }

    public Type ConfigType => typeof(FirelyNetIGOptions);
    public class FirelyNetIGOptions : ConfigGenerate
    {
        [ConfigOption(
            ArgName = "--extension-accessors",
            Description = "Style to export extension accessors with.")]
        public ExtensionAccessorExportCodes ExtensionAccessorExport { get; set; } = ExtensionAccessorExportCodes.ExtensionAccessors;

        private static ConfigurationOption ExtensionAccessorExportParameter { get; } = new()
        {
            Name = "ExtensionAccessorExport",
            DefaultValue = ExtensionAccessorExportCodes.ExtensionAccessors,
            CliOption = new System.CommandLine.Option<ExtensionAccessorExportCodes>("--extension-accessors", "Style to export extension accessors with.")
            {
                Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
                IsRequired = false,
            },
        };

        private static readonly ConfigurationOption[] _options =
        [
            ExtensionAccessorExportParameter,
        ];

        public override ConfigurationOption[] GetOptions()
        {
            return [.. base.GetOptions(), .. _options];
        }

        public override void Parse(System.CommandLine.Parsing.ParseResult parseResult)
        {
            // parse base properties
            base.Parse(parseResult);

            // iterate over options for ones we are interested in
            foreach (ConfigurationOption opt in _options)
            {
                switch (opt.Name)
                {
                    case "ExtensionAccessorExport":
                        ExtensionAccessorExport = GetOpt(parseResult, opt.CliOption, ExtensionAccessorExport);
                        break;
                }
            }
        }


        //private const string DefaultNamespace = "Hl7.Fhir.{realm}.{package}-{version}";

        ///// <summary>Gets or sets the namespace.</summary>
        //[ConfigOption(
        //    ArgName = "--namespace",
        //    Description = $"Base namespace prefix for exported classes, default is '{DefaultNamespace}', use '' (empty string) for none.")]
        //public string Namespace { get; set; } = DefaultNamespace;

        //private static ConfigurationOption NamespaceParameter { get; } = new()
        //{
        //    Name = "Namespace",
        //    DefaultValue = DefaultNamespace,
        //    CliOption = new System.CommandLine.Option<string>("--namespace", $"Base namespace for exported classes, default is '{DefaultNamespace}', use '' (empty string) for none.")
        //    {
        //        Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
        //        IsRequired = false,
        //    },
        //};
    }

    /// <summary>Opens the scope.</summary>
    private void OpenScope(ExportStreamWriter writer)
        => CSharpFirelyCommon.OpenScope(writer);

    /// <summary>Closes the scope.</summary>
    private void CloseScope(ExportStreamWriter writer, bool includeSemicolon = false, bool suppressNewline = false)
        => CSharpFirelyCommon.CloseScope(writer, includeSemicolon, suppressNewline);

    /// <summary>Writes an indented comment.</summary>
    /// <param name="value">    The value.</param>
    /// <param name="isSummary">(Optional) True if is summary, false if not.</param>
    /// <param name="singleLine"></param>
    private void WriteIndentedComment(ExportStreamWriter writer, string value, bool isSummary = true, bool singleLine = false, bool isRemarks = false)
        => writer.WriteIndentedComment(value, isSummary, singleLine, isRemarks);

    public void Export(object untypedOptions, DefinitionCollection info)
    {
        if (untypedOptions is not FirelyNetIGOptions options)
        {
            throw new ArgumentException("Options must be of type FirelyNetIGOptions");
        }

        // set internal vars so we don't pass them to every function
        _info = info;
        _options = options;

        if (!Directory.Exists(options.OutputDirectory))
        {
            Directory.CreateDirectory(options.OutputDirectory);
        }

        // need to process ValueSets so that we know which ones have enums
        ProcessValueSets();

        // write extension contents
        foreach (StructureDefinition sd in _info.ExtensionsByUrl.Values)
        {
            WriteExtension(sd);
        }

        // write profile contents
        foreach (StructureDefinition sd in _info.ProfilesByUrl.Values)
        {
            WriteProfile(sd);
        }

        CloseWriters();
    }

    /// <summary>
    /// Process all value sets.
    /// This is a hybrid processor that matches both
    /// CSharpFirely2.WriteSharedValueSets and CSharpFirely2.WriteEnums.
    /// Essentially, it tracks manual exclusions and determines the
    /// correct namespace based on bindings.
    /// </summary>
    private void ProcessValueSets()
    {
        Dictionary<string, HashSet<string>> usedEnumsByPackage = [];

        // traverse all versions of all value sets
        foreach ((string unversionedUrl, string[] versions) in _info.ValueSetVersions.OrderBy(kvp => kvp.Key))
        {
            // check for exclusions
            if (CSharpFirely2._exclusionSet.Contains(unversionedUrl))
            {
                continue;
            }

            // traverse value sets starting with highest version
            foreach (string vsVersion in versions.OrderDescending())
            {
                if (!_info.TryGetValueSet(unversionedUrl, vsVersion, out ValueSet? unexpandedVs))
                {
                    continue;
                }

                IEnumerable < StructureElementCollection> bindings = _info.AllBindingsForVs(unexpandedVs.Url);
                Hl7.Fhir.Model.BindingStrength? strongestBinding = _info.StrongestBinding(bindings);

                if (strongestBinding != Hl7.Fhir.Model.BindingStrength.Required)
                {
                    /* Since required bindings cannot be extended, those are the only bindings that
                       can be represented using enums in the POCO classes (using <c>Code&lt;T&gt;</c>). All other coded members
                       use <c>Code</c>, <c>Coding</c> or <c>CodeableConcept</c>.
                       Consequently, we only need to generate enums for valuesets that are used as
                       required bindings anywhere in the data model. */
                    continue;
                }

                if (!_info.TryExpandVs(unversionedUrl + "|" + vsVersion, out ValueSet? vs))
                {
                    continue;
                }

                IEnumerable<string> referencedBy = bindings.cgExtractBaseTypes(_info);

                string vsClassName = string.Empty;

                if ((referencedBy.Count() < 2) && !CSharpFirely2._explicitSharedValueSets.Contains((_info.FhirSequence.ToString(), vs.Url)))
                {
                    /* ValueSets that are used in a single POCO are generated as a nested enum inside that
                     * POCO, not here in the shared valuesets */

                    vsClassName = referencedBy.First();
                }

                if (!_info.TryGetPackageSource(vs, out string packageId, out string packageVersion))
                {
                    packageId = _info.MainPackageId;
                    packageVersion = _info.MainPackageVersion;
                }

                if (!usedEnumsByPackage.TryGetValue($"{packageId}#{packageVersion}", out HashSet<string>? usedEnumNames))
                {
                    usedEnumNames = [];
                    usedEnumsByPackage.Add($"{packageId}#{packageVersion}", usedEnumNames);
                }

                ProcessValueSet(vs, vsClassName, usedEnumNames, packageId, packageVersion);
            }
        }
    }

    private void ProcessValueSet(ValueSet vs, string className, HashSet<string> usedEnumNames, string packageId, string packageVersion)
    {
        if (_valueSetInfoByUrl.ContainsKey(vs.Url))
        {
            return;
        }

        string name = (vs.Name ?? vs.Id)
            .Replace(" ", string.Empty, StringComparison.Ordinal)
            .Replace("_", string.Empty, StringComparison.Ordinal);

        string nameSanitized = FhirSanitizationUtils.SanitizeForProperty(name, [], NamingConvention.PascalCase);

        // Enums and their containing classes cannot have the same name,
        // so we have to correct these here
        if (CSharpFirely2._enumNamesOverride.TryGetValue(vs.Url, out var replacementName))
        {
            nameSanitized = replacementName;
        }

        if (usedEnumNames.Contains(nameSanitized))
        {
            return;
        }

        usedEnumNames.Add(nameSanitized);

        // set our value set info
        _valueSetInfoByUrl.Add(
            vs.Url,
            new CSharpFirely2.WrittenValueSetInfo()
            {
                ClassName = className,
                ValueSetName = nameSanitized,
            });

        // do not write the value set if it is in a core package
        if (FhirPackageUtils.PackageIsFhirRelease(packageId))
        {
            return;
        }

        PackageData packageData = GetPackageData(packageId, packageVersion);

        // build an updated name for the value set
        string defaultName = $"Hl7.Fhir.Model.Extension.{nameSanitized}";
        string fixedName = packageData.Namespace + "." + nameSanitized;

        if (defaultName != fixedName)
        {
            if (_valueSetNameMaps.ContainsKey(defaultName))
            {
                Console.Write("");
            }
            else
            {
                // add the mapping to the global list
                _valueSetNameMaps.Add(defaultName, fixedName);
            }
        }

        // get a writer for the value set
        ExportStreamWriter writer = GetValueSetWriter(packageData);

        IEnumerable<string> referencedCodeSystems = vs.cgReferencedCodeSystems();
        if (referencedCodeSystems.Count() == 1)
        {
            WriteIndentedComment(
                writer,
                $"{vs.Description}\n" +
                $"(url: {vs.Url})\n" +
                $"(system: {referencedCodeSystems.First()})");
        }
        else
        {
            WriteIndentedComment(
                writer,
                $"{vs.Description}\n" +
                $"(url: {vs.Url})\n" +
                $"(systems: {referencedCodeSystems.Count()})");
        }

        IEnumerable<FhirConcept> concepts = vs.cgGetFlatConcepts(_info);

        string defaultSystem = concepts.Select(c => c.System)
                        .GroupBy(c => c)
                        .OrderByDescending(c => c.Count())
                        .First().Key;

        writer.WriteLineIndented($"[FhirEnumeration(\"{name}\", \"{vs.Url}\", \"{defaultSystem}\")]");

        writer.WriteLineIndented($"public enum {nameSanitized}");

        OpenScope(writer);      // open enum

        HashSet<string> usedLiterals = [];

        foreach (FhirConcept concept in concepts)
        {
            string codeName = ConvertEnumValue(concept.Code);
            string codeValue = FhirSanitizationUtils.SanitizeForValue(concept.Code);
            string description = string.IsNullOrEmpty(concept.Definition)
                ? $"MISSING DESCRIPTION\n(system: {concept.System})"
                : $"{concept.Definition}\n(system: {concept.System})";

            if (concept.HasProperty("status", "deprecated"))
            {
                description += "\nThis enum is DEPRECATED.";
            }

            WriteIndentedComment(writer, description);

            string display = FhirSanitizationUtils.SanitizeForValue(concept.Display);

            if (concept.System != defaultSystem)
            {
                writer.WriteLineIndented($"[EnumLiteral(\"{codeValue}\", \"{concept.System}\"), Description(\"{display}\")]");
            }
            else
            {
                writer.WriteLineIndented($"[EnumLiteral(\"{codeValue}\"), Description(\"{display}\")]");
            }

            if (usedLiterals.Contains(codeName))
            {
                // start at 2 so that the unadorned version makes sense as v1
                for (int i = 2; i < 1000; i++)
                {
                    if (usedLiterals.Contains($"{codeName}_{i}"))
                    {
                        continue;
                    }

                    codeName = $"{codeName}_{i}";
                    break;
                }
            }

            usedLiterals.Add(codeName);

            writer.WriteLineIndented($"{codeName},");
        }

        CloseScope(writer);     // close enum
    }

    private void WriteExtensionAccessors(
        ExtensionData extData,
        ExportStreamWriter writer)
    {
        switch (_options.ExtensionAccessorExport)
        {
            case ExtensionAccessorExportCodes.ExtensionAccessors:
                WriteExtensionAccessorsExt(extData, writer);
                break;

            //case ExtensionAccessorExportCodes.ValueTupleAccessors:
            //    WriteExtensionAccessorsTuples(extData, writer);
            //    break;

            case ExtensionAccessorExportCodes.ValueRecordAccessors:
                WriteExtensionAccessorsRecords(extData, writer);
                break;

            default:
                break;
        }
    }

    private void WriteExtensionAccessorsTuples(ExtensionData extData, ExportStreamWriter writer)
    {
        foreach (ExtensionContextRec ctx in extData.Contexts)
        {
            string contextType = ctx.ContextElementInfo?.ElementType ?? "Element";

            // check for simple extensions
            if ((extData.ValueElement != null) && (extData.ElementInfo != null))
            {
                string elementType = extData.ElementInfo.IsPrimitive ? extData.ElementInfo.PrimitiveHelperType! : extData.ElementInfo.ElementType!;

                // write a getter comment
                if (extData.ElementInfo.IsList)
                {
                    writer.WriteIndentedComment(_extValueGetterPrefixArray + extData.Summary, isSummary: true);
                    writer.WriteIndentedComment(extData.Remarks, isSummary: false, isRemarks: true);
                }
                else
                {
                    writer.WriteIndentedComment(_extValueGetterPrefixScalar + extData.Summary, isSummary: true);
                    writer.WriteIndentedComment(extData.Remarks, isSummary: false, isRemarks: true);
                }

                // build the getter name
                //string valueGetterName = extData.Name + "Get" + (extData.ValueInfo.Length > 1 ? type.ToPascalCase() : string.Empty);

                if (extData.ElementInfo.IsList)
                {
                    writer.WriteLineIndented($"public static IEnumerable<{extData.ElementInfo.ElementType}> {extData.Name}Get(this {contextType} o) =>");
                    writer.WriteLineIndented($"  o.GetExtensions({_classNameDefinitions}.{_extUrlPrefix}{extData.Name});");
                }
                else
                {
                    writer.WriteLineIndented($"public static {elementType}? {extData.Name}Get(this {contextType} o) =>");

                    switch (elementType)
                    {
                        case "bool":
                            writer.WriteLineIndented($"  o.GetBoolExtension({_classNameDefinitions}.{_extUrlPrefix}{extData.Name});");
                            break;

                        case "int":
                            writer.WriteLineIndented($"  o.GetIntegerExtension({_classNameDefinitions}.{_extUrlPrefix}{extData.Name});");
                            break;

                        case "string":
                            writer.WriteLineIndented($"  o.GetStringExtension({_classNameDefinitions}.{_extUrlPrefix}{extData.Name});");
                            break;

                        default:
                            writer.WriteLineIndented($"  o.GetExtensionValue<{elementType}>({_classNameDefinitions}.{_extUrlPrefix}{extData.Name});");
                            break;
                    }
                }

                writer.WriteLine();

                // write a setter comment
                if (extData.ElementInfo.IsList)
                {
                    writer.WriteIndentedComment(_extValueSetterPrefixArray + extData.Summary, isSummary: true);
                    writer.WriteIndentedComment(extData.Remarks, isSummary: false, isRemarks: true);
                }
                else
                {
                    writer.WriteIndentedComment(_extValueSetterPrefixScalar + extData.Summary, isSummary: true);
                    writer.WriteIndentedComment(extData.Remarks, isSummary: false, isRemarks: true);
                }

                // build the setter for either a single value or an array
                if (extData.ElementInfo.IsList)
                {
                    writer.WriteLineIndented($"public static void {extData.Name}Set(this {contextType} o, IEnumerable<{elementType}>? val)");
                    OpenScope(writer);      // setter function open
                    writer.WriteLineIndented($"o.RemoveExtension({_classNameDefinitions}.{_extUrlPrefix}{extData.Name});");
                    writer.WriteLineIndented("if (val == null) return;");
                    writer.WriteLineIndented("if (!val.Any()) return;");
                    writer.WriteLineIndented($"foreach ({elementType} v in val)");
                    OpenScope(writer);      // foreach open

                    // need to pull the original type so we can create the correct datatype
                    if (extData.ElementInfo.IsPrimitive)
                    {
                        writer.WriteLineIndented($"o.AddExtension({_classNameDefinitions}.{_extUrlPrefix}{extData.Name}, new {extData.ElementInfo.ElementType}(v));");
                    }
                    else
                    {
                        writer.WriteLineIndented($"o.AddExtension({_classNameDefinitions}.{_extUrlPrefix}{extData.Name}, v);");
                    }

                    CloseScope(writer, suppressNewline: true);     // foreach close
                    CloseScope(writer);      // setter function close
                }
                else
                {
                    writer.WriteLineIndented($"public static void {extData.Name}Set(this {contextType} o, {elementType}? val)");
                    OpenScope(writer);      // setter function open
                    writer.WriteLineIndented($"o.RemoveExtension({_classNameDefinitions}.{_extUrlPrefix}{extData.Name});");
                    writer.WriteLineIndented("if (val == null) return;");
                    // need to pull the original type so we can create the correct datatype
                    if (extData.ElementInfo.IsPrimitive)
                    {
                        writer.WriteLineIndented($"o.AddExtension({_classNameDefinitions}.{_extUrlPrefix}{extData.Name}, new {extData.ElementInfo.ElementType}(val));");
                    }
                    else
                    {
                        writer.WriteLineIndented($"o.AddExtension({_classNameDefinitions}.{_extUrlPrefix}{extData.Name}, val);");
                    }

                    CloseScope(writer);      // setter function close
                }
            }
            else
            {
                // write a comment for the getter
                if (extData.IsList)
                {
                    writer.WriteIndentedComment(_extValueGetterPrefixArray + extData.Summary, isSummary: true);
                    writer.WriteIndentedComment(extData.Remarks, isSummary: false, isRemarks: true);
                }
                else
                {
                    writer.WriteIndentedComment(_extValueGetterPrefixScalar + extData.Summary, isSummary: true);
                    writer.WriteIndentedComment(extData.Remarks, isSummary: false, isRemarks: true);
                }

                // we can make a tuple return
                List<KeyValuePair<string, string>> extensionTuple = BuildReturnTuple(extData);

                string retTuple = string.Join(", ", extensionTuple.Select(t => $"{t.Value}? {t.Key}"));

                if (extData.IsList)
                {
                    writer.WriteLineIndented($"public static IEnumerable<({retTuple})> {extData.Name}GetValues(this {contextType} o)");
                    OpenScope(writer);      // function open
                    writer.WriteLineIndented($"IEnumerable<Extension> roots = o.GetExtensions({_classNameDefinitions}.{_extUrlPrefix}{extData.Name});");
                    writer.WriteLineIndented("if (!roots.Any()) yield break;");
                    writer.WriteLineIndented($"foreach (Extension root in roots)");
                    OpenScope(writer);      // foreach open

                    // pull values from the extension tree
                    WriteSubExtensionGetTupleRecurse(writer, extData, "root");

                    // add to our list
                    writer.WriteLineIndented($"yield return ({string.Join(", ", extensionTuple.Select(kvp => kvp.Key))});");

                    CloseScope(writer, suppressNewline: true);     // foreach close
                    CloseScope(writer);     // function close
                }
                else
                {
                    writer.WriteLineIndented($"public static ({retTuple})? {extData.Name}GetValue(this {contextType} o)");
                    OpenScope(writer);
                    writer.WriteLineIndented($"Extension? root = o.GetExtension({_classNameDefinitions}.{_extUrlPrefix}{extData.Name});");
                    writer.WriteLineIndented("if (root == null) return null;");

                    // pull values from the extension tree
                    WriteSubExtensionGetTupleRecurse(writer, extData, "root");

                    writer.WriteLineIndented($"return ({string.Join(", ", extensionTuple.Select(kvp => kvp.Key))});");

                    CloseScope(writer);
                }

                // write a comment for the setter
                if (extData.IsList)
                {
                    writer.WriteIndentedComment(_extValueSetterPrefixArray + extData.Summary, isSummary: true);
                    writer.WriteIndentedComment(extData.Remarks, isSummary: false, isRemarks: true);
                }
                else
                {
                    writer.WriteIndentedComment(_extValueSetterPrefixScalar + extData.Summary, isSummary: true);
                    writer.WriteIndentedComment(extData.Remarks, isSummary: false, isRemarks: true);
                }

                // write a setter for either a single value or an array
                if (extData.IsList)
                {
                    string innerType = string.Join(", ", extensionTuple.Select(kvp => $"{kvp.Value}? {kvp.Key}"));

                    writer.WriteLineIndented($"public static void {extData.Name}Set(" +
                        $"this {contextType} o, " +
                        $"IEnumerable<({innerType})> values)");
                    OpenScope(writer);      // setter function open
                    writer.WriteLineIndented($"o.RemoveExtension({_classNameDefinitions}.{_extUrlPrefix}{extData.Name});");

                    writer.WriteLineIndented($"if (values == null) return;");
                    writer.WriteLineIndented($"if (!values.Any()) return;");

                    writer.WriteLineIndented($"foreach (({innerType}) in values)");
                    OpenScope(writer);       // foreach open

                    if (string.IsNullOrEmpty(extData.ParentName))
                    {
                        writer.WriteLineIndented($"Extension root = new Extension() {{ Url = {_classNameDefinitions}.{_extUrlPrefix}{extData.Name} }};");
                    }
                    else
                    {
                        writer.WriteLineIndented($"Extension root = new Extension() {{ Url = {_classNameDefinitions}.{_extUrlPrefix}{extData.ParentName}{extData.Name} }};");
                    }
                    writer.WriteLineIndented($"bool rootAdded = false;");

                    // pull values from the extension tree
                    WriteSubExtensionSetTupleRecurse(writer, extData, "root");

                    writer.WriteLineIndented($"if (rootAdded) o.Extension.Add(root);");

                    CloseScope(writer, suppressNewline: true);      // foreach close
                    CloseScope(writer);     // setter function close
                }
                else
                {
                    writer.WriteLineIndented($"public static void {extData.Name}Set(" +
                        $"this {contextType} o, " +
                        $"{string.Join(", ", extensionTuple.Select(kvp => $"{kvp.Value}? {kvp.Key}"))})");
                    OpenScope(writer);      // setter function open
                    writer.WriteLineIndented($"o.RemoveExtension({_classNameDefinitions}.{_extUrlPrefix}{extData.Name});");

                    writer.WriteLineIndented($"if ({string.Join(" && ", extensionTuple.Select(kvp => $"({kvp.Key} == null)"))}) return;");

                    writer.WriteLineIndented($"Extension root = new Extension() {{ Url = {_classNameDefinitions}.{_extUrlPrefix}{extData.Name} }};");

                    // pull values from the extension tree
                    WriteSubExtensionSetTupleRecurse(writer, extData, "root");

                    writer.WriteLineIndented($"o.Extension.Add(root);");

                    CloseScope(writer);     // setter function close
                }
            }
        }
    }

    private void WriteExtensionAccessorsRecords(ExtensionData extData, ExportStreamWriter writer)
    {
        HashSet<string> usedRecordTypes = [];

        foreach (ExtensionContextRec ctx in extData.Contexts)
        {
            string contextType = ctx.ContextTarget?.IsRootOfStructure == true
                ? ctx.ContextTarget.Element.Path
                : ctx.ContextElementInfo?.ElementType ?? "Element";

            if (!contextType.Contains('.'))
            {
                contextType = "Hl7.Fhir.Model." + contextType;
            }

            //writer.WriteLineIndented($"// Exported for {ctx.AllowedContext.Type}:{ctx.AllowedContext.Expression}.");

            //if (ctx.ContextTarget != null)
            //{
            //    writer.WriteLineIndented($"// Expanded to {ctx.ContextTarget.Element.Path}.");
            //}

            //if (ctx.ContextElementInfo != null)
            //{
            //    writer.WriteLineIndented($"// Type: {ctx.ContextElementInfo.ElementType}.");
            //}

            // check for simple extensions
            if (extData.ElementInfo != null)
            {
                string elementType = extData.ElementInfo.IsPrimitive ? extData.ElementInfo.PrimitiveHelperType! : extData.ElementInfo.ElementType!;

                // write a getter comment
                if (extData.ElementInfo.IsList)
                {
                    writer.WriteIndentedComment(_extValueGetterPrefixArray + extData.Summary, isSummary: true);
                    writer.WriteIndentedComment(extData.Remarks, isSummary: false, isRemarks: true);
                }
                else
                {
                    writer.WriteIndentedComment(_extValueGetterPrefixScalar + extData.Summary, isSummary: true);
                    writer.WriteIndentedComment(extData.Remarks, isSummary: false, isRemarks: true);
                }

                // build the getter name
                //string valueGetterName = extData.Name + "Get" + (extData.ValueInfo.Length > 1 ? type.ToPascalCase() : string.Empty);

                if (extData.ElementInfo.IsList)
                {
                    writer.WriteLineIndented($"public static IEnumerable<{extData.ElementInfo.ElementType}> {extData.Name}Get(this {contextType} o) =>");
                    writer.WriteLineIndented($"  o.GetExtensions({_classNameDefinitions}.{_extUrlPrefix}{extData.Name});");
                }
                else
                {
                    writer.WriteLineIndented($"public static {elementType}? {extData.Name}Get(this {contextType} o) =>");

                    switch (elementType)
                    {
                        case "bool":
                        case "bool?":
                            writer.WriteLineIndented($"  o.GetBoolExtension({_classNameDefinitions}.{_extUrlPrefix}{extData.Name});");
                            break;

                        case "int":
                        case "int?":
                            writer.WriteLineIndented($"  o.GetIntegerExtension({_classNameDefinitions}.{_extUrlPrefix}{extData.Name});");
                            break;

                        case "string":
                        case "string?":
                            writer.WriteLineIndented($"  o.GetStringExtension({_classNameDefinitions}.{_extUrlPrefix}{extData.Name});");
                            break;

                        default:
                            writer.WriteLineIndented($"  o.GetExtensionValue<{elementType}>({_classNameDefinitions}.{_extUrlPrefix}{extData.Name});");
                            break;
                    }
                }

                writer.WriteLine();

                // write a setter comment
                if (extData.ElementInfo.IsList)
                {
                    writer.WriteIndentedComment(_extValueSetterPrefixArray + extData.Summary, isSummary: true);
                    writer.WriteIndentedComment(extData.Remarks, isSummary: false, isRemarks: true);
                }
                else
                {
                    writer.WriteIndentedComment(_extValueSetterPrefixScalar + extData.Summary, isSummary: true);
                    writer.WriteIndentedComment(extData.Remarks, isSummary: false, isRemarks: true);
                }

                // build the setter for either a single value or an array
                if (extData.ElementInfo.IsList)
                {
                    writer.WriteLineIndented($"public static void {extData.Name}Set(this {contextType} o, IEnumerable<{elementType}>? val)");
                    OpenScope(writer);      // setter function open
                    writer.WriteLineIndented($"o.RemoveExtension({_classNameDefinitions}.{_extUrlPrefix}{extData.Name});");
                    writer.WriteLineIndented("if (val == null) return;");
                    writer.WriteLineIndented("if (!val.Any()) return;");
                    writer.WriteLineIndented($"foreach ({elementType} v in val)");
                    OpenScope(writer);      // foreach open

                    // need to pull the original type so we can create the correct datatype
                    if (extData.ElementInfo.IsPrimitive)
                    {
                        writer.WriteLineIndented($"o.AddExtension({_classNameDefinitions}.{_extUrlPrefix}{extData.Name}, new {extData.ElementInfo.ElementType}(v));");
                    }
                    else
                    {
                        writer.WriteLineIndented($"o.AddExtension({_classNameDefinitions}.{_extUrlPrefix}{extData.Name}, v);");
                    }

                    CloseScope(writer, suppressNewline: true);     // foreach close
                    CloseScope(writer);      // setter function close
                }
                else
                {
                    writer.WriteLineIndented($"public static void {extData.Name}Set(this {contextType} o, {elementType}? val)");
                    OpenScope(writer);      // setter function open
                    writer.WriteLineIndented($"o.RemoveExtension({_classNameDefinitions}.{_extUrlPrefix}{extData.Name});");
                    writer.WriteLineIndented("if (val == null) return;");
                    // need to pull the original type so we can create the correct datatype
                    if (extData.ElementInfo.IsPrimitive)
                    {
                        writer.WriteLineIndented($"o.AddExtension({_classNameDefinitions}.{_extUrlPrefix}{extData.Name}, new {extData.ElementInfo.ElementType}(val));");
                    }
                    else
                    {
                        writer.WriteLineIndented($"o.AddExtension({_classNameDefinitions}.{_extUrlPrefix}{extData.Name}, val);");
                    }

                    CloseScope(writer);      // setter function close
                }
            }
            else
            {
                // write the accessor class
                if (WriteExtensionAccessorClass(writer, extData, usedRecordTypes, out string recClassName))
                {
                    usedRecordTypes.Add(recClassName);
                }

                // write a comment for the getter
                if (extData.IsList)
                {
                    writer.WriteIndentedComment(_extValueGetterPrefixArray + extData.Summary, isSummary: true);
                    writer.WriteIndentedComment(extData.Remarks, isSummary: false, isRemarks: true);
                }
                else
                {
                    writer.WriteIndentedComment(_extValueGetterPrefixScalar + extData.Summary, isSummary: true);
                    writer.WriteIndentedComment(extData.Remarks, isSummary: false, isRemarks: true);
                }

                if (extData.IsList)
                {
                    writer.WriteLineIndented($"public static IEnumerable<{recClassName}> {extData.Name}Get(this {contextType} o)");
                    OpenScope(writer);      // function open
                    writer.WriteLineIndented($"IEnumerable<Extension> roots = o.GetExtensions({_classNameDefinitions}.{_extUrlPrefix}{extData.Name});");
                    writer.WriteLineIndented("if (!roots.Any()) yield break;");
                    writer.WriteLineIndented($"foreach (Extension root in roots)");
                    OpenScope(writer);      // foreach open

                    // pull values from the extension tree
                    WriteExtensionRecordPropertyReads(writer, extData, "root");

                    // add to our list
                    writer.WriteLineIndented($"yield return new {recClassName}()");
                    WriteExtensionRecordCreateFromProperties(writer, extData);

                    CloseScope(writer, suppressNewline: true);     // foreach close
                    CloseScope(writer);     // function close
                }
                else
                {
                    writer.WriteLineIndented($"public static {recClassName}? {extData.Name}Get(this {contextType} o)");
                    OpenScope(writer);
                    writer.WriteLineIndented($"Extension? root = o.GetExtension({_classNameDefinitions}.{_extUrlPrefix}{extData.Name});");
                    writer.WriteLineIndented("if (root == null) return null;");

                    // pull values from the extension tree
                    WriteExtensionRecordPropertyReads(writer, extData, "root");

                    writer.WriteLineIndented($"return new {recClassName}()");
                    WriteExtensionRecordCreateFromProperties(writer, extData);

                    CloseScope(writer);
                }

                // write a comment for the setter
                if (extData.IsList)
                {
                    writer.WriteIndentedComment(_extValueSetterPrefixArray + extData.Summary, isSummary: true);
                    writer.WriteIndentedComment(extData.Remarks, isSummary: false, isRemarks: true);
                }
                else
                {
                    writer.WriteIndentedComment(_extValueSetterPrefixScalar + extData.Summary, isSummary: true);
                    writer.WriteIndentedComment(extData.Remarks, isSummary: false, isRemarks: true);
                }

                // write a setter for either a single value or an array
                if (extData.IsList)
                {
                    writer.WriteLineIndented($"public static void {extData.Name}Set(this {contextType} o, IEnumerable<{recClassName}> values)");
                    OpenScope(writer);      // setter function open
                    writer.WriteLineIndented($"o.RemoveExtension({_classNameDefinitions}.{_extUrlPrefix}{extData.Name});");

                    WriteExtensionRecordPropertyWrites(writer, extData, "o", "values");


                    //writer.WriteLineIndented($"if (values == null) return;");
                    //writer.WriteLineIndented($"if (!values.Any()) return;");

                    //writer.WriteLineIndented($"foreach ({recClassName} val{extData.Name} in values)");
                    //OpenScope(writer);       // foreach open

                    //if (string.IsNullOrEmpty(extData.ParentName))
                    //{
                    //    writer.WriteLineIndented($"Extension root = new Extension() {{ Url = {_classNameDefinitions}.{_extUrlPrefix}{extData.Name} }};");
                    //}
                    //else
                    //{
                    //    writer.WriteLineIndented($"Extension root = new Extension() {{ Url = {_classNameDefinitions}.{_extUrlPrefix}{extData.ParentName}{extData.Name} }};");
                    //}
                    //writer.WriteLineIndented($"bool rootAdded = false;");

                    //// pull values from the extension tree
                    //WriteExtensionRecordPropertyWrites(writer, extData, "root");

                    //writer.WriteLineIndented($"if (rootAdded) o.Extension.Add(root);");

                    //CloseScope(writer, suppressNewline: true);      // foreach close
                    CloseScope(writer);     // setter function close
                }
                else
                {
                    writer.WriteLineIndented($"public static void {extData.Name}Set(this {contextType} o, {recClassName}? value)");
                    OpenScope(writer);      // setter function open
                    writer.WriteLineIndented($"o.RemoveExtension({_classNameDefinitions}.{_extUrlPrefix}{extData.Name});");

                    WriteExtensionRecordPropertyWrites(writer, extData, "o", "value");

                    //WriteExtensionRecordPropertyWrites(writer, extData, $"val{extData.Name}");

                    //writer.WriteLineIndented($"if ({string.Join(" && ", extData.Children.Select(propEx => $"({propEx.Name}{_recordValueSuffix} == null)"))}) return;");

                    //writer.WriteLineIndented($"Extension root = new Extension() {{ Url = {_classNameDefinitions}.{_extUrlPrefix}{extData.Name} }};");

                    //// pull values from the extension tree
                    //WriteExtensionRecordPropertyWrites(writer, extData, "root");

                    //writer.WriteLineIndented($"o.Extension.Add(root);");

                    CloseScope(writer);     // setter function close
                }
            }
        }
    }


    private void WriteExtensionAccessorsExt(ExtensionData extData, ExportStreamWriter writer)
    {
        string returnType = extData.IsList ? "IEnumerable<Extension>" : "Extension?";
        string getAlias = extData.IsList ? "GetExtensions" : "GetExtension";

        foreach (ExtensionContextRec ctx in extData.Contexts)
        {
            string contextType = ctx.ContextElementInfo?.ElementType ?? "Element";

            // write a comment for the getter
            if (extData.IsList)
            {
                writer.WriteIndentedComment(_extGetterPrefixArray + extData.Summary, isSummary: true);
                writer.WriteIndentedComment(extData.Remarks, isSummary: false, isRemarks: true);
            }
            else
            {
                writer.WriteIndentedComment(_extGetterPrefixScalar + extData.Summary, isSummary: true);
                writer.WriteIndentedComment(extData.Remarks, isSummary: false, isRemarks: true);
            }

            // write a getter, which returns either a single extension or an enumerable of extensions
            writer.WriteLineIndented($"public static {returnType} {extData.Name}Get(this {contextType} o) =>");
            writer.WriteLineIndented($"  o.{getAlias}({_classNameDefinitions}.{_extUrlPrefix}{extData.Name});");
            writer.WriteLine();

            // write a comment for the setter
            if (extData.IsList)
            {
                writer.WriteIndentedComment(_extSetterPrefixArray + extData.Summary, isSummary: true);
                writer.WriteIndentedComment(extData.Remarks, isSummary: false, isRemarks: true);
            }
            else
            {
                writer.WriteIndentedComment(_extSetterPrefixScalar + extData.Summary, isSummary: true);
                writer.WriteIndentedComment(extData.Remarks, isSummary: false, isRemarks: true);
            }

            // write the appropriate setter (single or array)
            if (extData.IsList)
            {
                writer.WriteLineIndented($"public static void {extData.Name}Set(this {contextType} o, {returnType}? val)");
                OpenScope(writer);      // setter function open
                writer.WriteLineIndented($"o.RemoveExtension({_classNameDefinitions}.{_extUrlPrefix}{extData.Name});");
                writer.WriteLineIndented("if (val == null) return;");
                writer.WriteLineIndented("if (!val.Any()) return;");
                writer.WriteLineIndented("foreach (Extension e in val)");
                OpenScope(writer);      // foreach open
                writer.WriteLineIndented($"if (e.Url != {_classNameDefinitions}.{_extUrlPrefix}{extData.Name}) e.Url = {_classNameDefinitions}.{_extUrlPrefix}{extData.Name};");
                writer.WriteLineIndented("o.Extension.Add(e);");
                CloseScope(writer, suppressNewline: true);     // foreach close
                CloseScope(writer);      // setter function close
            }
            else
            {
                writer.WriteLineIndented($"public static void {extData.Name}Set(this {contextType} o, {returnType} val)");
                OpenScope(writer);      // setter function open
                writer.WriteLineIndented($"o.RemoveExtension({_classNameDefinitions}.{_extUrlPrefix}{extData.Name});");
                writer.WriteLineIndented("if (val == null) return;");
                writer.WriteLineIndented($"if (val.Url != {_classNameDefinitions}.{_extUrlPrefix}{extData.Name}) val.Url = {_classNameDefinitions}.{_extUrlPrefix}{extData.Name};");
                writer.WriteLineIndented("o.Extension.Add(val);");
                CloseScope(writer);      // setter function close
            }
        }

    }

    private void WriteSubExtensionSetTupleRecurse(ExportStreamWriter writer, ExtensionData extData, string parentVarName)
    {
        if (extData.ValueElement != null)
        {
            string valName = _processingValuePrefix + extData.Name;
            string extName = _extUrlPrefix + extData.ParentName + extData.Name;
            string typeName;

            if (extData.ValueElement.Type.Any())
            {
                typeName = extData.ValueElement.Type.First().cgName();
            }
            else
            {
                typeName = extData.ValueElement.cgBaseTypeName(_info, false);
            }

            string fhirNetType = TypeNameMappings.TryGetValue(typeName, out string? v) ? v : typeName;
            bool isPrimitive = false;

            if (PrimitiveTypeMap.TryGetValue(typeName, out string? mappedType))
            {
                typeName = mappedType;
                isPrimitive = true;
            }

            if (extData.ValueElement.cgIsArray())
            {
                writer.WriteLineIndented($"if ({valName} != null)");
                OpenScope(writer);          // if open
                writer.WriteLineIndented($"foreach ({typeName} vt{extData.Name} in {valName})");
                OpenScope(writer);          // foreach open

                if (isPrimitive)
                {
                    writer.WriteLineIndented($"{parentVarName}.AddExtension({_classNameDefinitions}.{extName}, new {fhirNetType}({valName}));");
                }
                else
                {
                    writer.WriteLineIndented($"{parentVarName}.AddExtension({_classNameDefinitions}.{extName}, {valName});");
                }

                CloseScope(writer);         // foreach close
                writer.WriteLineIndented($"{parentVarName}Added = true;");
                CloseScope(writer);         // if close
            }
            else if (isPrimitive)
            {
                writer.WriteLineIndented($"if ({valName} != null)");
                OpenScope(writer);          // if open
                writer.WriteLineIndented($"{parentVarName}.AddExtension({_classNameDefinitions}.{extName}, new {fhirNetType}({valName}));");
                writer.WriteLineIndented($"{parentVarName}Added = true;");
                CloseScope(writer);         // if close
            }
            else
            {
                writer.WriteLineIndented($"if ({valName} != null)");
                OpenScope(writer);          // if open
                writer.WriteLineIndented($"{parentVarName}.AddExtension({_classNameDefinitions}.{extName}, {valName});");
                writer.WriteLineIndented($"{parentVarName}Added = true;");
                CloseScope(writer);         // if close
            }

            return;
        }

        foreach (ExtensionData extensionData in extData.Children)
        {
            //string valName = _tupleValuePrefix + extensionData.Name;
            string extName = _extUrlPrefix + extensionData.ParentName + extensionData.Name;

            if (extensionData.Children.Length != 0)
            {
                writer.WriteLineIndented($"Extension ext{extensionData.Name} = new Extension() {{ Url = {_classNameDefinitions}.{extName} }};");
                writer.WriteLineIndented($"bool ext{extensionData.Name}Added = false;");

                // nest through children
                foreach (ExtensionData child in extensionData.Children)
                {
                    WriteSubExtensionSetTupleRecurse(writer, child, $"ext{extensionData.Name}");
                }

                writer.WriteLineIndented($"if (ext{extensionData.Name}Added) {parentVarName}.Extension.Add(ext{extensionData.Name});");

                continue;
            }

            WriteSubExtensionSetTupleRecurse(writer, extensionData, parentVarName);
        }
    }


    private void WriteSubExtensionGetTupleRecurse(ExportStreamWriter writer, ExtensionData extData, string parentVarName)
    {
        if ((extData.ValueElement != null) && (extData.ElementInfo != null))
        {
            string elementType = extData.ElementInfo.IsPrimitive ? extData.ElementInfo.PrimitiveHelperType! : extData.ElementInfo.ElementType!;

            string valName = _processingValuePrefix + extData.Name;
            string extName = extData.ParentName + extData.Name;

            if (extData.ElementInfo.IsList)
            {
                writer.WriteLineIndented($"IEnumerable<{elementType}>? {valName} = {parentVarName}" +
                    $".GetExtensions({_classNameDefinitions}.{extName})" +
                    $".Where(e => e.Value is not null && e.Value is {elementType})" +
                    $".Select(e => ({elementType})e;");
            }
            else
            {
                switch (elementType)
                {
                    case "bool":
                        writer.WriteLineIndented($"{elementType}? {valName} = {parentVarName}.GetBoolExtension({_classNameDefinitions}.{_extUrlPrefix}{extName});");
                        break;

                    case "int":
                        writer.WriteLineIndented($"{elementType}? {valName} = {parentVarName}.GetIntegerExtension({_classNameDefinitions}.{_extUrlPrefix}{extName});");
                        break;

                    case "string":
                        writer.WriteLineIndented($"{elementType}? {valName} = {parentVarName}.GetStringExtension({_classNameDefinitions}.{_extUrlPrefix}{extName});");
                        break;

                    default:
                        writer.WriteLineIndented($"{elementType}? {valName} = {parentVarName}.GetExtensionValue<{elementType}>({_classNameDefinitions}.{_extUrlPrefix}{extName});");
                        break;
                }
            }

            return;
        }

        foreach (ExtensionData extensionData in extData.Children)
        {
            if (extensionData.Children.Length != 0)
            {
                writer.WriteLineIndented($"Extension? ext{extensionData.Name} = {parentVarName}.GetExtension({_classNameDefinitions}.{_extUrlPrefix}{extensionData.Name});");
                writer.WriteLineIndented($"if (ext{extensionData.Name} != null)");
                OpenScope(writer);

                // nest through children
                foreach (ExtensionData child in extensionData.Children)
                {
                    WriteSubExtensionGetTupleRecurse(writer, child, $"ext{extensionData.Name}");
                }

                CloseScope(writer);
                continue;
            }

            WriteSubExtensionGetTupleRecurse(writer, extensionData, parentVarName);
        }
    }

    private void WriteExtensionRecordPropertyReads(ExportStreamWriter writer, ExtensionData extData, string parentVarName)
    {
        if ((extData.ValueElement != null) && (extData.ElementInfo != null))
        {
            string elementType = extData.ElementInfo.IsPrimitive ? extData.ElementInfo.PrimitiveHelperType! : extData.ElementInfo.ElementType!;

            string valName = _processingValuePrefix + extData.Name;
            string extName = extData.ParentName + extData.Name;

            if (extData.IsList)
            {
                writer.WriteLineIndented($"IEnumerable<{elementType}>? {valName} = {parentVarName}" +
                    $".GetExtensions({_classNameDefinitions}.{extName})" +
                    $".Where(e => e.Value is not null && e.Value is {elementType})" +
                    $".Select(e => ({elementType})e;");
            }
            else
            {
                switch (elementType)
                {
                    case "bool":
                        writer.WriteLineIndented($"{elementType}? {valName} = {parentVarName}.GetBoolExtension({_classNameDefinitions}.{_extUrlPrefix}{extName});");
                        break;

                    case "int":
                        writer.WriteLineIndented($"{elementType}? {valName} = {parentVarName}.GetIntegerExtension({_classNameDefinitions}.{_extUrlPrefix}{extName});");
                        break;

                    case "string":
                        writer.WriteLineIndented($"{elementType}? {valName} = {parentVarName}.GetStringExtension({_classNameDefinitions}.{_extUrlPrefix}{extName});");
                        break;

                    default:
                        writer.WriteLineIndented($"{elementType}? {valName} = {parentVarName}.GetExtensionValue<{elementType}>({_classNameDefinitions}.{_extUrlPrefix}{extName});");
                        break;
                }
            }

            return;
        }

        foreach (ExtensionData extensionData in extData.Children)
        {
            if (extensionData.Children.Length != 0)
            {
                writer.WriteLineIndented($"Extension? ext{extensionData.Name} = {parentVarName}.GetExtension({_classNameDefinitions}.{_extUrlPrefix}{extensionData.Name});");
                writer.WriteLineIndented($"if (ext{extensionData.Name} != null)");
                OpenScope(writer);

                // nest through children
                foreach (ExtensionData child in extensionData.Children)
                {
                    WriteExtensionRecordPropertyReads(writer, child, $"ext{extensionData.Name}");
                }

                CloseScope(writer);
                continue;
            }

            WriteExtensionRecordPropertyReads(writer, extensionData, parentVarName);
        }
    }

    private void WriteExtensionRecordCreateFromProperties(ExportStreamWriter writer, ExtensionData extData)
    {
        OpenScope(writer);      // new record open

        // traverse our sub-extensions to use as properties
        foreach (ExtensionData subExtData in extData.Children)
        {
            string valName = _processingValuePrefix + subExtData.Name;

            writer.WriteLineIndented($"{subExtData.Name}{_recordValueSuffix} = {valName},");
        }

        CloseScope(writer, includeSemicolon: true, suppressNewline: true);     // new record close
    }

    private void WriteExtensionRecordPropertyWrites(
        ExportStreamWriter writer,
        ExtensionData extData,
        string parentObjectName,
        string parentValueName,
        string parentBoolName = "")
    {
        string extensionLiteral = $"{_classNameDefinitions}.{_extUrlPrefix}{extData.ParentName}{extData.Name}";
        string extProcessingName = _processingExtPrefix + extData.Name;
        string valName = string.IsNullOrEmpty(extData.ParentName)
            ? parentValueName
            : $"{parentValueName}.{extData.Name}{_recordValueSuffix}";

        // check if this is a terminal leaf (has a value element)
        if ((extData.ValueElement != null) && (extData.ElementInfo != null))
        {
            writer.WriteLineIndented($"if ({valName} != null)");
            OpenScope(writer);      // if open

            string elementType = extData.ElementInfo.IsPrimitive ? extData.ElementInfo.PrimitiveHelperType! : extData.ElementInfo.ElementType!;

            if (extData.ElementInfo.IsList)
            {
                string iteratorValName = _processingValuePrefix + extData.Name + _processingArraySuffix;
                string iteratorBoolName = _processingExtAddPrefix + extData.Name + _processingArraySuffix;

                writer.WriteLineIndented($"if ({valName}.Any())");
                OpenScope(writer);          // if open

                if (!string.IsNullOrEmpty(parentBoolName))
                {
                    writer.WriteLineIndented($"bool {iteratorBoolName} = false;");
                }

                writer.WriteLineIndented($"foreach ({elementType}? {iteratorValName} in {valName})");
                OpenScope(writer);          // foreach open

                writer.WriteLineIndented($"if ({_processingValuePrefix}{extData.Name}{_processingArraySuffix} == null) continue;");

                if (extData.ElementInfo.IsPrimitive)
                {
                    writer.WriteLineIndented($"{parentObjectName}.AddExtension({extensionLiteral}, new {extData.ElementInfo.ElementType!}({iteratorValName}));");
                }
                else
                {
                    writer.WriteLineIndented($"{parentObjectName}.AddExtension({extensionLiteral}, {iteratorValName});");
                }

                if (!string.IsNullOrEmpty(parentBoolName))
                {
                    writer.WriteLineIndented($"{iteratorBoolName} = true;");
                }

                CloseScope(writer);         // foreach close

                if (!string.IsNullOrEmpty(parentBoolName))
                {
                    writer.WriteLineIndented($"if ({iteratorBoolName}) {parentBoolName} = true ");
                }

                CloseScope(writer);         // if close
            }
            else if (extData.ElementInfo.IsPrimitive)
            {
                writer.WriteLineIndented($"{parentObjectName}.AddExtension({extensionLiteral}, new {extData.ElementInfo.ElementType!}({valName}));");
                if (!string.IsNullOrEmpty(parentBoolName))
                {
                    writer.WriteLineIndented($"{parentBoolName} = true;");
                }
            }
            else
            {
                writer.WriteLineIndented($"{parentObjectName}.AddExtension({extensionLiteral}, {valName});");
                if (!string.IsNullOrEmpty(parentBoolName))
                {
                    writer.WriteLineIndented($"{parentBoolName} = true;");
                }
            }

            CloseScope(writer);      // if close
            return;
        }

        string boolName = _processingExtAddPrefix + extData.Name;
        string extIteratorValName = _processingValuePrefix + extData.Name + _processingArraySuffix;

        writer.WriteLineIndented($"if ({valName} != null)");
        OpenScope(writer);      // if open

        string extParentValueName = extData.IsList
            ? extData.ParentName + extData.Name + _recordClassSuffix
            : valName;

        // check for array
        if (extData.IsList)
        {
            writer.WriteLineIndented($"foreach ({extData.ParentName}{extData.Name}{_recordClassSuffix} {extIteratorValName} in {valName})");
            OpenScope(writer);      // foreach open
            writer.WriteLineIndented($"Extension {extProcessingName} = new Extension() {{ Url = {extensionLiteral} }};");
            writer.WriteLineIndented($"bool {boolName} = false;");

            foreach (ExtensionData subExt in extData.Children)
            {
                WriteExtensionRecordPropertyWrites(writer, subExt, extProcessingName, extIteratorValName, boolName);
            }

            writer.WriteLineIndented($"if ({boolName}) {parentObjectName}.Extension.Add({extProcessingName});");

            CloseScope(writer, suppressNewline: true);      // foreach close
        }
        else
        {
            writer.WriteLineIndented($"Extension {extProcessingName} = new Extension() {{ Url = {extensionLiteral} }};");
            writer.WriteLineIndented($"bool {boolName} = false;");

            foreach (ExtensionData subExt in extData.Children)
            {
                WriteExtensionRecordPropertyWrites(writer, subExt, extProcessingName, extParentValueName, boolName);
            }

            writer.WriteLineIndented($"if ({boolName}) {parentObjectName}.Extension.Add({extProcessingName});");
        }

        CloseScope(writer, suppressNewline:true);     // if close
    }

    private bool WriteExtensionAccessorClass(ExportStreamWriter writer, ExtensionData extData, HashSet<string> usedRecordTypes, out string recClassName)
    {
        // build our class name
        recClassName = string.IsNullOrEmpty(extData.ParentName)
            ? extData.Name + _recordClassSuffix
            : extData.ParentName + extData.Name + _recordClassSuffix;

        if (usedRecordTypes.Contains(recClassName))
        {
            return false;
        }

        if (string.IsNullOrEmpty(extData.ParentName))
        {
            // write a comment for the accessor record class
            writer.WriteIndentedComment($"Record class to access the contents of the {extData.Name} extension\nExtension Url: {extData.Url}", isSummary: true);
            writer.WriteIndentedComment(extData.Remarks, isSummary: false, isRemarks: true);
        }
        else
        {
            // write a comment for the accessor record class
            writer.WriteIndentedComment($"Record class to access the contents of the {extData.ParentName}.{extData.Name} sub extension\nExtension Url: {extData.Url})", isSummary: true);
            writer.WriteIndentedComment(extData.Remarks, isSummary: false, isRemarks: true);
        }

        // open our class
        writer.WriteLineIndented($"public record class {recClassName}");
        OpenScope(writer);

        // traverse our sub-extensions to use as properties
        foreach (ExtensionData subExtData in extData.Children)
        {
            string et;

            // check if we have type information
            if ((subExtData.ValueElement != null) && (subExtData.ElementInfo != null))
            {
                et = subExtData.ElementInfo.IsPrimitive ? subExtData.ElementInfo.PrimitiveHelperType! : subExtData.ElementInfo.ElementType!;

                // get optional flags
                BuildElementOptionalFlags(
                    subExtData.ValueElement,
                    out string summary,
                    out string isModifier,
                    out string choice,
                    out string allowedTypes,
                    out string resourceReferences);

                // write our comments
                WriteIndentedComment(
                    writer,
                    subExtData.Summary +
                    (string.IsNullOrEmpty(choice) ? string.Empty : "\nChoice type: " + choice[9..]) +
                    (string.IsNullOrEmpty(allowedTypes) ? string.Empty : "\nAllowed Types: " + allowedTypes[14..^2]) +
                    (string.IsNullOrEmpty(resourceReferences) ? string.Empty : "\nReferences: " + resourceReferences[12..^2]),
                    isSummary: true);
                WriteIndentedComment(writer, subExtData.Remarks, isSummary: false, isRemarks: true);
            }
            else
            {
                et = extData.Name + subExtData.Name + _recordClassSuffix;

                // write our comments
                WriteIndentedComment(writer, subExtData.Summary, isSummary: true);
                WriteIndentedComment(writer, subExtData.Remarks, isSummary: false, isRemarks: true);
            }

            // write this property
            if (subExtData.IsList)
            {
                writer.WriteLineIndented($"public List<{et}>? {subExtData.Name}{_recordValueSuffix} {{ get; init; }}");
            }
            else
            {
                writer.WriteLineIndented($"public {et}? {subExtData.Name}{_recordValueSuffix} {{ get; init; }}");
            }
        }

        // close our record class
        CloseScope(writer);

        // check for sub-extensions that we need to nest into
        foreach (ExtensionData subExtData in extData.Children)
        {
            if ((subExtData.ValueElement == null) || (subExtData.ElementInfo == null))
            {
                WriteExtensionAccessorClass(writer, subExtData, usedRecordTypes, out _);
            }
        }

        return true;
    }

    private List<KeyValuePair<string, string>> BuildReturnTuple(ExtensionData extData)
    {
        string valName = _processingValuePrefix + extData.Name;

        if ((extData.ValueElement != null) && (extData.ElementInfo != null))
        {
            string elementType = extData.ElementInfo.IsPrimitive ? extData.ElementInfo.PrimitiveHelperType! : extData.ElementInfo.ElementType!;

            if (extData.ElementInfo.IsList)
            {
                return [new KeyValuePair<string, string>(valName, "IEnumerable<" + elementType + ">"),];
            }

            return [new KeyValuePair<string, string>(valName, elementType),];
        }

        // if we are in the root, do not nest into the tuple
        if (string.IsNullOrEmpty(extData.ParentName))
        {
            List<KeyValuePair<string, string>> tupleTypes = [];

            foreach (ExtensionData extensionData in extData.Children)
            {
                tupleTypes.AddRange(BuildReturnTuple(extensionData));
            }

            return tupleTypes;
        }

        List<KeyValuePair<string, string>> subTypes = [];

        foreach (ExtensionData extensionData in extData.Children)
        {
            subTypes.AddRange(BuildReturnTuple(extensionData));
        }

        return [new KeyValuePair<string, string>(valName, "(" + string.Join(", ", subTypes.Select(kvp => kvp.Value + "? " + kvp.Key)) + ")"), ];
    }


    private void WriteProfile(StructureDefinition sd)
    {
        if (!_info.TryGetPackageSource(sd, out string packageId, out string packageVersion))
        {
            packageId = _info.MainPackageId;
            packageVersion = _info.MainPackageVersion;
        }

        if (FhirPackageUtils.PackageIsFhirRelease(packageId))
        {
            // skip core packages
            return;
        }

        PackageData packageData = GetPackageData(packageId, packageVersion);

        string name = string.IsNullOrEmpty(sd.Name)
            ? sd.Id.ToPascalCase()
            : sd.Name.ToPascalCase();

        // remove any hyphens from the name
        name = name.Replace('-', '_');

        string filename = Path.Combine(_options.OutputDirectory, packageData.FolderName, $"{name}.cs");

        if (!Directory.Exists(Path.GetDirectoryName(filename)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filename)!);
        }

        using (FileStream stream = new(filename, FileMode.Create))
        using (ExportStreamWriter writer = new(stream))
        {
            WriteHeader(writer, [packageData.Namespace]);

            WriteNamespaceOpen(writer, packageData.Namespace + _namespaceSuffixProfiles);

            if (!string.IsNullOrEmpty(sd.Description))
            {
                WriteIndentedComment(writer, sd.Description, isSummary: true);
            }
            else
            {
                WriteIndentedComment(writer, sd.Title, isSummary: true);
            }

            // check for must-support elements
            IEnumerable<ElementDefinition> mustSupports = sd.cgElements().Where(ed => ed.MustSupport == true);

            if (mustSupports.Any())
            {
                writer.WriteLineIndented("/// <remarks>");
                writer.WriteIndentedComment("Must support elements:", isSummary: false);

                foreach (ElementDefinition ms in mustSupports)
                {
                    writer.WriteIndentedComment($"- {ms.Path}: {ms.cgShort()}", isSummary: false);
                }

                writer.WriteLineIndented("/// </remarks>");
            }

            writer.WriteLineIndented($"public static class {name}");
            OpenScope(writer);      // class

            // write the profile URL
            WriteIndentedComment(writer, "The official URL for this profile.", isSummary: true);
            writer.WriteLineIndented($"public const string ProfileUrl = \"{sd.Url}\"");
            writer.WriteLine();

            // write the profile version
            if (!string.IsNullOrEmpty(sd.Version))
            {
                WriteIndentedComment(writer, "The declared version of this profile.", isSummary: true);
                writer.WriteLineIndented($"public const string ProfileVersion = \"{sd.Version}\"");
                writer.WriteLine();
            }
            else
            {
                WriteIndentedComment(writer, "The package version of this profile.", isSummary: true);
                writer.WriteLineIndented($"public const string ProfileVersion = \"{packageData.PackageVersion}\"");
                writer.WriteLine();
            }
            // check for all elements that are slices
            ElementDefinition[] slices = sd.cgElements(skipSlices: false).Where(ed => ed.ElementId.LastIndexOf(':') > ed.ElementId.LastIndexOf('.')).ToArray();

            foreach (ElementDefinition ed in slices)
            {
                string sliceName = string.IsNullOrEmpty(ed.SliceName) ? ed.ElementId.Substring(ed.ElementId.LastIndexOf(':') + 1) : ed.SliceName;

                writer.WriteLineIndented($"// Slice: {ed.ElementId} ({sliceName})");

                // get the parent that defines the slicing
                if (!sd.cgTryGetSlicingParent(ed, out ElementDefinition? slicingEd))
                {
                    writer.WriteLineIndented($"// Could not resolve the parent element for: {ed.ElementId}.");

                    // TODO(ginoc): extensions can be implicitly sliced
                    continue;
                }

                writer.WriteLineIndented($"// Slicing Parent: {slicingEd.ElementId}");

                // get all the elements for this slice
                IEnumerable<ElementDefinition> sliceElements = sd.cgElementsForSlice(ed, sliceName);

                // use our sliced element and parent to get the discriminators
                IEnumerable<SliceDiscriminator> sliceDiscriminators = sd.cgDiscriminatedValues(_info, slicingEd, sliceName, sliceElements);

                foreach (SliceDiscriminator discriminator in sliceDiscriminators)
                {
                    writer.WriteLineIndented("// Discriminator:");
                    writer.WriteLineIndented($"//                Id: {discriminator.Id}");
                    writer.WriteLineIndented($"//              Type: {discriminator.Type}");
                    writer.WriteLineIndented($"//              Path: {discriminator.Path}");
                    writer.WriteLineIndented($"//   PostResolvePath: {discriminator.PostResolvePath}");
                    writer.WriteLineIndented($"//             Value: {discriminator.Value}");
                    writer.WriteLineIndented($"//         IsBinding: {discriminator.IsBinding}");
                    writer.WriteLineIndented($"//       BindingName: {discriminator.BindingName}");

                    bool isExtensionSlice = discriminator.Path.EndsWith(".extension.url", StringComparison.Ordinal);

                    if (isExtensionSlice)
                    {
                        // figure out the path we want to root our C# extension on
                        string rootPath = slicingEd.Path.Substring(0, slicingEd.Path.Length - ".extension.url".Length);

                        string extUrl = discriminator.Value.ToString() ?? string.Empty;

                        // try to resolve this extension
                        if (TryGetExtensionData(extUrl, out ExtensionData? extData) &&
                            TryGetPackageData(extData.DefinitionDirective, out PackageData extPackageData))
                        {
                            // we have a definition we can link to
                            writer.WriteLineIndented($"// Slice uses extension: {extPackageData.Namespace}.{extData.Name.ToPascalCase()}");
                        }
                        else
                        {
                            writer.WriteLineIndented($"// Could not resolve extension: {extUrl}");
                        }
                    }
                    else
                    {
                        // figure out the path we want to root our C# extension on
                        string rootPath = slicingEd.Path;

                        writer.WriteLineIndented($"// Non-extension slice...");
                    }

                }

                writer.WriteLine();

                //    string sliceName = !string.IsNullOrEmpty(ed.SliceName)
                //        ? ed.SliceName
                //        : ed.ElementId.Substring(ed.ElementId.LastIndexOf(':') + 1);

                //    // get the types for this slice
                //    ElementDefinition.TypeRefComponent[] edTypes = ed.cgTypes().Values.ToArray();

                //    foreach (ElementDefinition.TypeRefComponent edType in edTypes)
                //    {
                //        string typeName = edType.cgName();

                //        switch (typeName)
                //        {
                //            case "Extension":
                //                {
                //                    // iterate over each profile
                //                    foreach (string profileUrl in edType.Profile)
                //                    {
                //                        WriteProfileExtensionAccessors(
                //                            writer,
                //                            sd,
                //                            name,
                //                            ed,
                //                            sliceName,
                //                            profileUrl,
                //                            exportedExtensions);
                //                    }
                //                }
                //                break;
                //        }
                //    }
            }

            CloseScope(writer);     // class

            WriteNamespaceClose(writer);
        }
    }

    //private void WriteProfileExtensionAccessors(
    //    ExportStreamWriter writer,
    //    StructureDefinition profileSd,
    //    string profileName,
    //    ElementDefinition profiledElement,
    //    string sliceName,
    //    string extensionUrl,
    //    Dictionary<string, ExtensionData> exportedExtensions)
    //{
    //    ComponentDefinition? extCd = null;
    //    bool isArray = false;

    //    _ = exportedExtensions.TryGetValue(extensionUrl, out ExtensionData? exportedExt);

    //    // try to resolve this extension
    //    if (_info.TryResolveByCanonicalUri(extensionUrl, out Resource? extR) &&
    //        (extR is StructureDefinition extSd))
    //    {
    //        extCd = new(extSd);
    //        isArray = extCd.Element.cgCardinalityMax() > 1;
    //    }

    //    StructureDefinition.ContextComponent[] contexts = extCd?.Structure.Context?.ToArray()
    //        ?? [new() { Type = StructureDefinition.ExtensionContextType.Element, Expression = "Element" }];

    //    string returnType = isArray ? "Extension?" : "IEnumerable<Extension>";
    //    string getAlias = isArray ? "GetExtension" : "GetExtensions";

    //    // strip off the .extension suffix if it exists
    //    string elementPath = profiledElement.Path.EndsWith(".extension", StringComparison.Ordinal)
    //        ? profiledElement.Path.Substring(0, profiledElement.Path.Length - ".extension".Length)
    //        : profiledElement.Path;

    //    // iterate over all the allowed contexts
    //    foreach (StructureDefinition.ContextComponent ctx in contexts)
    //    {
    //        // for now, only handle element contexts
    //        if (ctx.Type != StructureDefinition.ExtensionContextType.Element)
    //        {
    //            continue;
    //        }

    //        // check to see if everything in the path sequence is a scalar
    //        if (_info.TryResolveElementTree(elementPath, out StructureDefinition? baseSd, out ElementDefinition[] elements))
    //        {
    //            bool allScalars = elements.All(ed => ed.cgCardinalityMax() == 1);

    //            // if everything is scalar, we can base off the root type
    //            if (allScalars)
    //            {
    //                // write a getter, which returns either a single extension or an enumerable of extensions
    //                writer.WriteLineIndented($"public static {returnType} {profileName}{sliceName.ToPascalCase()}(this {baseSd.cgName().ToPascalCase()} r) =>");

    //                if (exportedExt != null)
    //                {
    //                    writer.WriteLineIndented($"  r.{getAlias}({_classNameDefinitions}.{exportedExt.Name})");
    //                }
    //                else
    //                {
    //                    writer.WriteLineIndented($"  r.{getAlias}(\"{extensionUrl}\");");
    //                }
    //            }
    //            else
    //            {
    //                // write a getter, which returns either a single extension or an enumerable of extensions
    //                writer.WriteLineIndented($"public static {returnType} {profileName}{sliceName.ToPascalCase()}(this SomeComponentDefinition c) =>");

    //                if (exportedExt != null)
    //                {
    //                    writer.WriteLineIndented($"  c.{getAlias}({_classNameDefinitions}.{exportedExt.Name})");
    //                }
    //                else
    //                {
    //                    writer.WriteLineIndented($"  c.{getAlias}(\"{extensionUrl}\");");
    //                }
    //            }
    //        }
    //        else
    //        {
    //            // write a getter, which returns either a single extension or an enumerable of extensions
    //            writer.WriteLineIndented($"public static {returnType} {profileName}{sliceName.ToPascalCase()}(this Element e) =>");

    //            if (exportedExt != null)
    //            {
    //                writer.WriteLineIndented($"  e.{getAlias}({_classNameDefinitions}.{exportedExt.Name})");
    //            }
    //            else
    //            {
    //                writer.WriteLineIndented($"  e.{getAlias}(\"{extensionUrl}\");");
    //            }
    //        }

    //        writer.WriteLine();
    //    }
    //}

    /// <summary>
    /// Closes the writers used for exporting.
    /// </summary>
    private void CloseWriters()
    {
        foreach (ExportStreamWriter writer in _definitionWriters.Values)
        {
            CloseAndDispose(writer);
        }
        _definitionWriters.Clear();

        foreach (ExportStreamWriter writer in _extensionWriters.Values)
        {
            CloseAndDispose(writer);
        }
        _extensionWriters.Clear();

        foreach (ExportStreamWriter writer in _valueSetWriters.Values)
        {
            CloseAndDispose(writer);
        }
        _valueSetWriters.Clear();

        return;

        void CloseAndDispose(ExportStreamWriter writer)
        {
            writer.CloseScope();        // class

            WriteNamespaceClose(writer);

            WriteFooter(writer);

            writer.Dispose();
        }
    }

    /// <summary>Attempts to get package data a PackageData from the given string.</summary>
    /// <param name="directive">  The directive.</param>
    /// <param name="packageData">[out] Information describing the package.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool TryGetPackageData(string directive, out PackageData packageData)
    {
        string[] components = directive.Split('#');
        if (components.Length != 2)
        {
            // invalid directive
            packageData = default;
            return false;
        }

        if (_packageDataByDirective.TryGetValue(directive, out packageData))
        {
            return true;
        }

        packageData = GetPackageData(components[0], components[1]);

        return true;
    }

    /// <summary>Gets package data.</summary>
    /// <param name="packageId">     Identifier for the package.</param>
    /// <param name="packageVersion">The package version.</param>
    /// <returns>The package data.</returns>
    private PackageData GetPackageData(
        string packageId,
        string packageVersion)
    {
        string directive = $"{packageId}#{packageVersion}";

        if (_packageDataByDirective.TryGetValue(directive, out PackageData pd))
        {
            return pd;
        }

        string versionSanitized = SanitizeVersion(packageVersion);

        if (packageId.StartsWith("hl7.fhir.", StringComparison.OrdinalIgnoreCase))
        {
            string[] components = packageId.Split('.');
            string realm;
            string name;

            if (components.Length > 3)
            {
                realm = components[2].ToPascalCase();
                name = string.Join('.', components[3..]).ToPascalCase();
            }
            else
            {
                realm = "Uv";
                name = packageId.ToPascalCase();
            }

            pd = new()
            {
                Key = packageId + "#" + packageVersion,
                PackageId = packageId,
                PackageVersion = packageVersion,
                Namespace = $"Hl7.Fhir.Packages.{realm}.{name}_{versionSanitized}",
                ClassPrefix = $"{realm.ToPascalCase()}{name.ToPascalCase()}",
                VersionSanitized = versionSanitized,
                FolderName = $"{realm.ToPascalCase()}{name.ToPascalCase()}_{versionSanitized}",
            };

            _packageDataByDirective.Add(directive, pd);

            return pd;
        }

        pd = new()
        {
            Key = packageId + "#" + packageVersion,
            PackageId = packageId,
            PackageVersion = packageVersion,
            Namespace = $"Hl7.Fhir.Packages.{packageId.ToPascalCase()}_{versionSanitized}",
            ClassPrefix = packageId.ToPascalCase(),
            VersionSanitized = versionSanitized,
            FolderName = $"{packageId.ToPascalCase()}_{versionSanitized}",
        };

        _packageDataByDirective.Add(directive, pd);

        return pd;
    }

    private ExportStreamWriter GetDefinitionWriter(PackageData packageData)
    {
        if (_definitionWriters.TryGetValue(packageData.Key, out ExportStreamWriter? writer))
        {
            return writer;
        }

        string directory = Path.Combine(_options.OutputDirectory, packageData.FolderName);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        //string filename = Path.Combine(_options.OutputDirectory, $"{packageData.FilenamePrefix}.cs");
        string filename = Path.Combine(_options.OutputDirectory, packageData.FolderName, $"{_classNameDefinitions}.cs");

        FileStream stream = new(filename, FileMode.Create);
        writer = new ExportStreamWriter(stream);
        _definitionWriters[packageData.Key] = writer;

        WriteHeader(writer);

        WriteNamespaceOpen(writer, packageData.Namespace);

        WriteIndentedComment(writer, $"Definitions common to the {packageData.Key} package");

        writer.WriteLineIndented($"public static class {_classNameDefinitions}");
        writer.OpenScope();

        return writer;
    }

    private ExportStreamWriter GetExtensionWriter(PackageData packageData)
    {
        if (_extensionWriters.TryGetValue(packageData.Key, out ExportStreamWriter? writer))
        {
            return writer;
        }

        string directory = Path.Combine(_options.OutputDirectory, packageData.FolderName);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string filename = Path.Combine(_options.OutputDirectory, packageData.FolderName, $"{_classNameExtensions}.cs");

        FileStream stream = new(filename, FileMode.Create);
        writer = new ExportStreamWriter(stream);
        _extensionWriters[packageData.Key] = writer;

        WriteHeader(writer);

        WriteNamespaceOpen(writer, packageData.Namespace);

        WriteIndentedComment(writer, $"Extension accessors for the {packageData.Key} package");

        writer.WriteLineIndented($"public static class {_classNameExtensions}");
        writer.OpenScope();

        return writer;
    }

    private ExportStreamWriter GetValueSetWriter(PackageData packageData)
    {
        if (_valueSetWriters.TryGetValue(packageData.Key, out ExportStreamWriter? writer))
        {
            return writer;
        }

        string directory = Path.Combine(_options.OutputDirectory, packageData.FolderName);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string filename = Path.Combine(_options.OutputDirectory, packageData.FolderName, $"{_classNameValueSets}.cs");

        FileStream stream = new(filename, FileMode.Create);
        writer = new ExportStreamWriter(stream);
        _valueSetWriters[packageData.Key] = writer;

        WriteHeader(writer);

        WriteNamespaceOpen(writer, packageData.Namespace);

        WriteIndentedComment(writer, $"ValueSet definitions and utility functions for the {packageData.Key} package");

        writer.WriteLineIndented($"public static class {_classNameValueSets}");
        writer.OpenScope();

        return writer;
    }


    /// <summary>Writes an extension.</summary>
    /// <param name="sd">The SD.</param>
    private void WriteExtension(StructureDefinition sd)
    {
        if (!_info.TryGetPackageSource(sd, out string packageId, out string packageVersion))
        {
            packageId = _info.MainPackageId;
            packageVersion = _info.MainPackageVersion;
        }

        PackageData packageData = GetPackageData(packageId, packageVersion);

        // get a definition writer
        ExportStreamWriter definitionWriter = GetDefinitionWriter(packageData);

        // build a component for our extension
        ComponentDefinition cd = new(sd);

        // get our extension data
        ExtensionData extData = GetExtensionData(cd);

        // recursively write our extension urls into definitions
        WriteExtensionDefinition(extData, definitionWriter);

        // get an extension writer
        ExportStreamWriter extensionWriter = GetExtensionWriter(packageData);

        WriteExtensionAccessors(extData, extensionWriter);
    }

    /// <summary>Recursively writes extension urls.</summary>
    /// <param name="extData">     Information describing the extension.</param>
    /// <param name="writer">      The currently in-use text writer.</param>
    /// <param name="parentPrefix">(Optional) The parent prefix.</param>
    private void WriteExtensionDefinition(ExtensionData extData, ExportStreamWriter writer)
    {
        writer.WriteIndentedComment(extData.Summary, isSummary: true);
        writer.WriteIndentedComment(extData.Remarks, isSummary: false, isRemarks: true);

        if (string.IsNullOrEmpty(extData.ParentName))
        {
            writer.WriteLineIndented($"public const string {_extUrlPrefix}{extData.Name} = \"{extData.Url}\";");
        }
        else
        {
            writer.WriteLineIndented($"public const string {_extUrlPrefix}{extData.ParentName}{extData.Name} = \"{extData.Url}\";");
        }

        writer.WriteLine();

        // write any sub-extensions
        foreach (ExtensionData childData in extData.Children)
        {
            WriteExtensionDefinition(childData, writer);
        }
    }

    private bool TryGetExtensionData(string url, [NotNullWhen(true)] out ExtensionData? extensionData)
    {
        // try to resolve this extension
        if (_info.TryResolveByCanonicalUri(url, out Resource? extR) &&
            (extR is StructureDefinition extSd))
        {
            // build a component definition for this extension
            ComponentDefinition extCd = new(extSd);

            // get extension data for this extension
            extensionData = GetExtensionData(extCd);

            return true;
        }

        extensionData = null;
        return false;
    }


    internal CSharpFirely2.WrittenElementInfo BuildElementInfo(
        string exportedComplexName,
        ElementDefinition element)
    {
        CSharpFirely2.WrittenElementInfo ei = CSharpFirely2.BuildElementInfo(_info, exportedComplexName, element, _valueSetInfoByUrl);

        // for our use, we are going to make every type optional, so strip all of them here for sanity
        if (ei.ElementType?.EndsWith('?') ?? false)
        {
            ei.ElementType = ei.ElementType[..^1];
        }

        if (ei.PrimitiveHelperName?.EndsWith('?') ?? false)
        {
            ei.PrimitiveHelperName = ei.PrimitiveHelperName[..^1];
        }

        if (ei.PrimitiveHelperType?.EndsWith('?') ?? false)
        {
            ei.PrimitiveHelperType = ei.PrimitiveHelperType[..^1];
        }

        if (ei.PropertyType?.EndsWith('?') ?? false)
        {
            ei.PropertyType = ei.PropertyType[..^1];
        }

        // the Firely builder assumes all ValueSets are in the HL7.Fhir.Model namespace, so we need to fix any that are not
        if (ei.ElementType?.StartsWith("Code<", StringComparison.Ordinal) ?? false)
        {
            string srcType = ei.PrimitiveHelperType ?? string.Empty;

            if (_valueSetNameMaps.TryGetValue(srcType, out string? fixedType))
            {
                ei.ElementType = ei.ElementType?.Replace(srcType, fixedType, StringComparison.Ordinal);
                ei.PrimitiveHelperName = ei.PrimitiveHelperName?.Replace(srcType, fixedType, StringComparison.Ordinal);
                ei.PrimitiveHelperType = ei.PrimitiveHelperType?.Replace(srcType, fixedType, StringComparison.Ordinal);
                ei.PropertyType = ei.PropertyType?.Replace(srcType, fixedType, StringComparison.Ordinal);
            }
        }

        return ei;
    }

    private void BuildElementOptionalFlags(
        ElementDefinition element,
        out string summary,
        out string isModifier,
        out string choice,
        out string allowedTypes,
        out string resourceReferences)
    {
        CSharpFirely2.BuildElementOptionalFlags(
            _info,
            element,
            GenSubset.Satellite,
            out summary,
            out isModifier,
            out choice,
            out allowedTypes,
            out resourceReferences);
    }

    /// <summary>Gets extension data based on a component definition.</summary>
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    /// <param name="cd">          The component definition for a root extension or slice (sub-extension).</param>
    /// <param name="parentExtUrl">(Optional) URL of the parent extent.</param>
    /// <returns>The extension data.</returns>
    private ExtensionData GetExtensionData(
        ComponentDefinition cd,
        string? parentExtUrl = null,
        ComponentDefinition? parentCd = null)
    {
        string summary = cd.cgShort();
        string remarks;
        string url;
        ElementDefinition? valueElement = null;
        List<ExtensionData> childExtensions = [];

        string contextString = cd.IsRootOfStructure
            ? "Allowed Contexts: " + string.Join(", ", cd.Structure.Context.Select(GetContextString))
            : "Allowed Contexts: " + (parentExtUrl ?? "Parent extension");

        ElementDefinition[] children = cd.cgGetChildren(includeDescendants: false, skipSlices: false).ToArray();

        ElementDefinition[] extSlices = children.Where(ced => ced.ElementId.StartsWith(cd.Element.Path + ".extension:", StringComparison.Ordinal)).ToArray();

        if (cd.IsRootOfStructure)
        {
            url = cd.Structure.Url;
        }
        else
        {
            ElementDefinition? urlElement = children.FirstOrDefault(ced => ced.Path.EndsWith(".url", StringComparison.Ordinal));

            if ((urlElement?.Fixed != null) &&
                (urlElement.Fixed is FhirUri extUri))
            {
                url = extUri.Value;
            }
            else
            {
                url = cd.Element.ElementId.Substring(cd.Element.ElementId.LastIndexOf(":") + 1);
            }
        }

        if (_extensionDataByUrl.TryGetValue(url, out ExtensionData? extData))
        {
            return extData;
        }

        // check for being a 'simple' extension (no sub-extensions)
        if (extSlices.Length == 0)
        {
            // get the value element
            valueElement = children.Where(ed => ed.Path.EndsWith("value[x]", StringComparison.Ordinal)).FirstOrDefault();

            if (valueElement == null)
            {
                throw new Exception($"Simple extension does not have a value element: {cd.Structure.Url}:{cd.Element.ElementId}");
            }

            // grab our types
            ElementDefinition.TypeRefComponent[] valueTypes = valueElement.cgTypes().Values.ToArray();

            string types;

            if (valueTypes.Length == 1)
            {
                types = "Allowed Type: " + valueTypes[0].cgName();
            }
            else
            {
                types = "Allowed Types: " + string.Join(", ", valueTypes.Select(t => t.cgName()).Distinct());
            }

            remarks = $"{contextString}\n{types}";
        }
        else
        {
            // traverse our child elements and look for extension slices
            foreach (ElementDefinition child in extSlices)
            {
                childExtensions.Add(GetExtensionData(
                    new ComponentDefinition()
                    {
                        Structure = cd.Structure,
                        Element = child,
                        IsRootOfStructure = false,
                    },
                    parentExtUrl: cd.Structure.Url,
                    parentCd: cd));
            }

            remarks = $"{contextString}\nSub-extensions:\n - {string.Join("\n - ", childExtensions.Select(child => child.Url))}";
        }

        string directive;
        if (_info.TryGetPackageSource(cd.Structure, out string packageId, out string packageVersion))
        {
            directive = $"{packageId}#{packageVersion}";
        }
        else
        {
            directive = $"{_info.MainPackageId}#{_info.MainPackageVersion}";
        }

        string name;
        string parentName;

        // check for root-level extension name conflicts
        if (cd.IsRootOfStructure)
        {
            name = cd.Structure.cgName().ToPascalCase(removeDelimiters: true);
            parentName = string.Empty;

            if (!_extensionNamesByPackageDirective.TryGetValue(directive, out HashSet<string>? usedNames))
            {
                usedNames = [];
            }

            if (usedNames.Contains(name))
            {
                int i = 1;
                string newName = name + i;

                while (usedNames.Contains(newName))
                {
                    i++;
                    newName = name + "_" + i;
                }

                name = newName;
            }

            usedNames.Add(name);
        }
        else
        {
            name = url.ToPascalCase(removeDelimiters: true);

            // we never want a choice type '[x]' suffix in our name
            if (name.EndsWith("[x]", StringComparison.Ordinal))
            {
                name = name[..^3];
            }

            parentName = cd.Structure.cgName().ToPascalCase(removeDelimiters: true);
        }

        List<ExtensionContextRec> extensionContexts = [];

        if (cd.IsRootOfStructure)
        {
            // build context information based on extension contexts
            StructureDefinition.ContextComponent[] contexts = cd.Structure.Context?.ToArray() ?? [];

            // if any context is "Element", we can just use the default rec we create for empty contexts
            if (contexts.Any(ctx => ctx.Expression == "Element"))
            {
                contexts = [];
            }

            // iterate over all the allowed contexts
            foreach (StructureDefinition.ContextComponent ctx in contexts)
            {
                if (ctx.Type == StructureDefinition.ExtensionContextType.Extension)
                {
                    extensionContexts.Add(new()
                    {
                        AllowedContext = ctx,
                        ContextTarget = ctx.Expression == parentExtUrl ? parentCd : null,
                        ContextElementInfo = BuildElementInfo(string.Empty, parentCd == null ? cd.Element : parentCd.Element),
                    });

                    continue;
                }

                // for now, only handle element contexts
                if (ctx.Type != StructureDefinition.ExtensionContextType.Element)
                {
                    Console.WriteLine($"Skipping {cd.Structure.Url} context {ctx.Type}:{ctx.Expression}");
                    continue;
                }

                // resolve the path the extension is attached to
                if (_info.TryResolveElementTree(ctx.Expression, out StructureDefinition? baseSd, out ElementDefinition[] elements) &&
                    (elements.Length > 0))
                {
                    ElementDefinition contextElement = elements.Last();

                    ComponentDefinition contextCd = new()
                    {
                        Structure = baseSd,
                        Element = contextElement,
                        IsRootOfStructure = elements.Length == 1,
                    };

                    extensionContexts.Add(new()
                    {
                        AllowedContext = ctx,
                        ContextTarget = contextCd,
                        ContextElementInfo = BuildElementInfo(contextCd.Structure.cgName(), contextCd.Element),
                    });
                }
            }

            // if there is a context for 'DataType' (forced by choice type), we want to add a single context for all 'DataType' types
            if (extensionContexts.Any(ctx => ctx.ContextElementInfo?.ElementType == "Hl7.Fhir.Model.DataType"))
            {
                extensionContexts.RemoveAll(ctx => ExtensionContextDerivesFromDataType(ctx) || (ctx.ContextElementInfo?.ElementType == "Hl7.Fhir.Model.DataType"));

                // generate a fake element information
                extensionContexts.Add(new()
                {
                    AllowedContext = new StructureDefinition.ContextComponent()
                    {
                        Type = StructureDefinition.ExtensionContextType.Element,
                        Expression = "DataType",
                    },
                    ContextTarget = null,
                    ContextElementInfo = new CSharpFirely2.WrittenElementInfo()
                    {
                        ElementType = "Hl7.Fhir.Model.DataType",
                        PropertyType = "Hl7.Fhir.Model.DataType",
                        IsPrimitive = false,
                        IsChoice = true,
                        FhirElementName = "",
                        IsList = false,
                        IsCodedEnum = false,
                    },
                });

            }

            // if there were no contexts, add an "Element" context
            if (contexts.Length == 0)
            {
                ComponentDefinition? targetCd = null;

                if (_info.ComplexTypesByName.TryGetValue("Element", out StructureDefinition? elementSd))
                {
                    targetCd = new(elementSd);
                }

                extensionContexts.Add(new()
                {
                    AllowedContext = new StructureDefinition.ContextComponent()
                    {
                        Type = StructureDefinition.ExtensionContextType.Element,
                        Expression = "Element",
                    },
                    ContextTarget = targetCd,
                    ContextElementInfo = BuildElementInfo(string.Empty, parentCd == null ? cd.Element : parentCd.Element),
                });
            }
        }
        else
        {
            // context is parent extension
            extensionContexts.Add(new()
            {
                AllowedContext = new StructureDefinition.ContextComponent()
                {
                    Type = StructureDefinition.ExtensionContextType.Extension,
                    Expression = parentExtUrl,
                },
                ContextTarget = parentCd,
                ContextElementInfo = BuildElementInfo(string.Empty, parentCd == null ? cd.Element : parentCd.Element),
            });
        }

        // create our extension data
        extData = new ExtensionData()
        {
            Component = cd,
            Name = name,
            Url = url,
            Summary = summary,
            Remarks = remarks,
            IsList = cd.Element.cgCardinalityMax() != 1,
            ParentName = parentName,
            DefinitionDirective = directive,
            Contexts = extensionContexts.ToArray(),
            ValueElement = valueElement,
            ElementInfo = valueElement == null ? null : BuildElementInfo(string.Empty, valueElement),
            Children = childExtensions.ToArray(),
        };

        _extensionDataByUrl.Add(url, extData);

        return extData;
    }

    private bool ExtensionContextDerivesFromDataType(ExtensionContextRec ctx)
    {
        if (ctx.ContextTarget == null)
        {
            return false;
        }

        string btName = ctx.ContextTarget.cgBaseTypeName(_info, false);

        return _info.PrimitiveTypesByName.ContainsKey(btName) || _info.ComplexTypesByName.ContainsKey(btName);
    }   

    /// <summary>Gets a comment-formatted string for an Extension context.</summary>
    /// <param name="ctx">The context.</param>
    /// <returns>The context string.</returns>
    private string GetContextString(StructureDefinition.ContextComponent ctx) => ctx switch
    {
        { Type: StructureDefinition.ExtensionContextType.Fhirpath } => $"FhirPath({ctx.Expression})",
        { Type: StructureDefinition.ExtensionContextType.Element } => ctx.Expression,
        { Type: StructureDefinition.ExtensionContextType.Extension } => $"Extension({ctx.Expression})",
        _ => $"{ctx.Type}({ctx.Expression})"
    };

    /// <summary>Sanitize a version string.</summary>
    /// <param name="version">The version.</param>
    /// <returns>A string.</returns>
    private string SanitizeVersion(string version)
    {
        return version.Replace('.', '_').Replace('-', '_');
    }

    /// <summary>Writes the namespace open.</summary>
    private void WriteNamespaceOpen(ExportStreamWriter writer, string ns)
    {
        writer.WriteLineIndented("namespace " + ns);
        OpenScope(writer);
    }

    /// <summary>Writes the namespace close.</summary>
    private void WriteNamespaceClose(ExportStreamWriter writer)
    {
        CloseScope(writer);
    }

    private void WriteFooter(ExportStreamWriter writer)
    {
        writer.WriteIndentedComment("end of file", singleLine: true);
    }

    private void WriteHeader(ExportStreamWriter writer, string[]? additionalUsingLibs = null)
    {
        WriteGenerationComment(writer);

        writer.WriteLineIndented("using System;");
        writer.WriteLineIndented("using System.Collections.Generic;");
        writer.WriteLineIndented("using System.Linq;");
        writer.WriteLineIndented("using System.Runtime.Serialization;");
        writer.WriteLineIndented("using Hl7.Fhir.Model;");

        if (additionalUsingLibs != null)
        {
            foreach (string usingStatement in additionalUsingLibs)
            {
                writer.WriteLineIndented("using " + usingStatement);
            }
        }

        writer.WriteLine(string.Empty);

        //WriteCopyright(writer);
    }

    /// <summary>Writes the generation comment.</summary>
    private void WriteGenerationComment(ExportStreamWriter writer)
    {
        writer.WriteLineIndented("// <auto-generated/>");
        writer.WriteLineIndented($"// Generated via export of: {string.Join(", ", _info.Manifests.Select(kvp => kvp.Key))}");

        if (_options.ExportKeys.Count != 0)
        {
            string restrictions = string.Join("|", _options.ExportKeys);
            writer.WriteLine($"  // Restricted to: {restrictions}");
        }

        writer.WriteLine(string.Empty);
    }

    /// <summary>Writes the copyright.</summary>
    private void WriteCopyright(ExportStreamWriter writer)
    {
        writer.WriteLineIndented("/*");
        writer.WriteLineIndented("  Copyright (c) 2011+, HL7, Inc.");
        writer.WriteLineIndented("  All rights reserved.");
        writer.WriteLineIndented("  ");
        writer.WriteLineIndented("  Redistribution and use in source and binary forms, with or without modification, ");
        writer.WriteLineIndented("  are permitted provided that the following conditions are met:");
        writer.WriteLineIndented("  ");
        writer.WriteLineIndented("   * Redistributions of source code must retain the above copyright notice, this ");
        writer.WriteLineIndented("     list of conditions and the following disclaimer.");
        writer.WriteLineIndented("   * Redistributions in binary form must reproduce the above copyright notice, ");
        writer.WriteLineIndented("     this list of conditions and the following disclaimer in the documentation ");
        writer.WriteLineIndented("     and/or other materials provided with the distribution.");
        writer.WriteLineIndented("   * Neither the name of HL7 nor the names of its contributors may be used to ");
        writer.WriteLineIndented("     endorse or promote products derived from this software without specific ");
        writer.WriteLineIndented("     prior written permission.");
        writer.WriteLineIndented("  ");
        writer.WriteLineIndented("  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS \"AS IS\" AND ");
        writer.WriteLineIndented("  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED ");
        writer.WriteLineIndented("  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. ");
        writer.WriteLineIndented("  IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, ");
        writer.WriteLineIndented("  INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT ");
        writer.WriteLineIndented("  NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR ");
        writer.WriteLineIndented("  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, ");
        writer.WriteLineIndented("  WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ");
        writer.WriteLineIndented("  ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE ");
        writer.WriteLineIndented("  POSSIBILITY OF SUCH DAMAGE.");
        writer.WriteLineIndented("  ");
        writer.WriteLineIndented("*/");
        writer.WriteLine(string.Empty);
    }

}
