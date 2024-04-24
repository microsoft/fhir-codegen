// <copyright file="FirelyNetIG.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGen.Configuration;
using Microsoft.Health.Fhir.CodeGen.Extensions;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;

using static Microsoft.Health.Fhir.CodeGen.Language.Firely.CSharpFirelyCommon;

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

    private const string _extGetterPrefixScalar = "Get an Extension representing the ";
    private const string _extGetterPrefixArray = "Get any Extensions representing the ";

    private record struct PackageData
    {
        public required string Key { get; set; }
        public required string PackageId { get; init; }
        public required string PackageVersion { get; init; }
        public required string Namespace { get; init; }
        public required string ClassPrefix { get; init; }
        public required string VersionSanitized { get; init; }
        public required string FilenamePrefix { get; init; }
    }

    private record class ExtensionData()
    {
        public required ComponentDefinition Component { get; init; }
        public required string Name { get; init; }
        public required string Url { get; init; }
        public required string Summary { get; init; }
        public required string Remarks { get; init; }
        public required bool IsComplex { get; init; }
        public required ElementDefinition? ValueElement { get; init; }
        public required ExtensionData[] Children { get; init; }
    }

    /// <summary>FHIR information we are exporting.</summary>
    private DefinitionCollection _info = null!;

    /// <summary>Options for controlling the export.</summary>
    private FirelyNetIGOptions _options = null!;

    public Type ConfigType => typeof(FirelyNetIGOptions);
    public class FirelyNetIGOptions : ConfigGenerate
    {
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

        Dictionary<string, ExtensionData> exportedExtensions = [];

        // write extension contents
        foreach (StructureDefinition sd in _info.ExtensionsByUrl.Values)
        {
            WriteExtension(sd, exportedExtensions);
        }

        // write profile contents
        //foreach (StructureDefinition sd in _info.ProfilesByUrl.Values)
        //{
        //    WriteProfile(sd, exportedExtensions);
        //}

        CloseWriters();
    }


    private void WriteExtensionAccessors(
        ExtensionData extData,
        ComponentDefinition extCd,
        ExportStreamWriter writer)
    {
        bool isArray = extCd.Element.cgIsArray();

        StructureDefinition.ContextComponent[] contexts = extCd?.Structure.Context?.ToArray()
            ?? [new() { Type = StructureDefinition.ExtensionContextType.Element, Expression = "Element" }];

        string returnType = isArray ? "IEnumerable<Extension>" : "Extension?";
        string getAlias = isArray ? "GetExtensions" : "GetExtension";

        HashSet<string> usedTypes = [];

        // if there is a context of "Element", all other contexts are noise
        if (contexts.Any(ctx => ctx.Expression == "Element"))
        {
            contexts = [new() { Type = StructureDefinition.ExtensionContextType.Element, Expression = "Element" }];
        }

        // iterate over all the allowed contexts
        foreach (StructureDefinition.ContextComponent ctx in contexts)
        {
            // for now, only handle element contexts
            if (ctx.Type != StructureDefinition.ExtensionContextType.Element)
            {
                continue;
            }

            // check to see if everything in the path sequence is a scalar
            if (_info.TryResolveElementTree(ctx.Expression, out StructureDefinition? baseSd, out ElementDefinition[] elements))
            {
                if (elements.Length == 0)
                {
                    continue;
                }

                ElementDefinition contextElement = elements.Last();

                ComponentDefinition contextCd = new()
                {
                    Structure = baseSd,
                    Element = contextElement,
                    IsRootOfStructure = elements.Length == 1,
                };

                string[] contextTypes;

                if (elements.Length == 1)
                {
                    contextTypes = [baseSd.Name.ToPascalCase()];
                }
                else
                {
                    contextTypes = contextElement.cgTypes().Keys.ToArray();

                    if (contextTypes.Length == 0)
                    {
                        contextTypes = [contextElement.cgBaseTypeName(_info, false)];
                    }
                }

                // traverse the types for this element
                foreach (string typeName in contextTypes)
                {
                    string sdkType = _info.HasChildElements(contextElement.Path)
                        ? GetFirelyComponentType(contextCd)
                        : typeName;

                    if (PrimitiveTypeMap.TryGetValue(sdkType, out string? mappedType))
                    {
                        sdkType = mappedType;
                    }

                    if (TypeNameMappings.TryGetValue(sdkType, out mappedType))
                    {
                        sdkType = mappedType;
                    }

                    if (usedTypes.Contains(sdkType))
                    {
                        continue;
                    }

                    usedTypes.Add(sdkType);

                    // write a comment
                    if (isArray)
                    {
                        writer.WriteIndentedComment(_extGetterPrefixArray + extData.Summary, isSummary: true);
                    }
                    else
                    {
                        writer.WriteIndentedComment(_extGetterPrefixScalar + extData.Summary, isSummary: true);
                    }

                    writer.WriteIndentedComment(extData.Remarks, isSummary: false, isRemarks: true);

                    // write a getter, which returns either a single extension or an enumerable of extensions
                    writer.WriteLineIndented($"public static {returnType} {extData.Name}Get(this {sdkType} o) =>");
                    writer.WriteLineIndented($"  o.{getAlias}(Definitions.{extData.Name});");
                    writer.WriteLine();

                    // handle complex extensions
                    if (extData.Children.Length != 0)
                    {
                        // check to see if all sub-extensions resolve to a single type
                        if (ExtensionResolvesToSingleType(extData))
                        {
                            // write a comment
                            if (isArray)
                            {
                                writer.WriteIndentedComment(_extGetterPrefixArray + extData.Summary, isSummary: true);
                            }
                            else
                            {
                                writer.WriteIndentedComment(_extGetterPrefixScalar + extData.Summary, isSummary: true);
                            }

                            writer.WriteIndentedComment(extData.Remarks, isSummary: false, isRemarks: true);

                            // we can make a tuple return
                            List<KeyValuePair<string, string>> extensionTuple = BuildReturnTuple(extData);

                            string retTuple = string.Join(", ", extensionTuple.Select(t => $"{t.Value}? {t.Key}"));

                            if (isArray)
                            {
                                writer.WriteLineIndented($"public static IEnumerable<({retTuple})> {extData.Name}GetValues(this {sdkType} o)");
                                OpenScope(writer);      // function open
                                writer.WriteLineIndented($"IEnumerable<Extension> roots = o.GetExtensions(Definitions.{extData.Name});");
                                writer.WriteLineIndented("if (!roots.Any()) yield break;");
                                writer.WriteLineIndented($"foreach (Extension root in roots)");
                                OpenScope(writer);      // foreach open

                                // pull values from the extension tree
                                WriteSubExtensionTupleRecurse(writer, extData, "root");

                                // add to our list
                                writer.WriteLineIndented($"yield return ({string.Join(", ", extensionTuple.Select(kvp => "val" + kvp.Key))});");

                                CloseScope(writer, suppressNewline:true);     // foreach close
                                CloseScope(writer);     // function close
                            }
                            else
                            {
                                writer.WriteLineIndented($"public static ({retTuple})? {extData.Name}GetValue(this {sdkType} o)");
                                OpenScope(writer);
                                writer.WriteLineIndented($"Extension? root = o.GetExtension(Definitions.{extData.Name});");
                                writer.WriteLineIndented("if (root == null) return null;");

                                // pull values from the extension tree
                                WriteSubExtensionTupleRecurse(writer, extData, "root");

                                writer.WriteLineIndented($"return ({string.Join(", ", extensionTuple.Select(kvp => "val" + kvp.Key))});");

                                CloseScope(writer);
                            }
                        }
                    }
                    else if (extData.ValueElement != null)
                    {
                        // check the possible return types for this extension
                        foreach (ElementDefinition.TypeRefComponent valueType in extData.ValueElement.cgTypes().Values)
                        {
                            string type = valueType.cgName();

                            if (PrimitiveTypeMap.TryGetValue(type, out mappedType))
                            {
                                type = mappedType;
                            }

                            if (type.EndsWith('?'))
                            {
                                type = type.Substring(0, type.Length - 1);
                            }

                            // write a comment
                            if (isArray)
                            {
                                writer.WriteIndentedComment(_extGetterPrefixArray + extData.Summary, isSummary: true);
                            }
                            else
                            {
                                writer.WriteIndentedComment(_extGetterPrefixScalar + extData.Summary, isSummary: true);
                            }

                            writer.WriteIndentedComment(extData.Remarks, isSummary: false, isRemarks: true);

                            // build the getter name
                            string valueGetterName = extData.Name + "Get" + type.ToPascalCase();

                            if (isArray)
                            {
                                writer.WriteLineIndented($"public static IEnumerable<{type}> {valueGetterName}(this {sdkType} o) =>");
                                writer.WriteLineIndented($"  o.GetExtensions(Definitions.{extData.Name});");
                            }
                            else
                            {
                                writer.WriteLineIndented($"public static {type}? {valueGetterName}(this {sdkType} o) =>");

                                switch (type)
                                {
                                    case "bool":
                                        writer.WriteLineIndented($"  o.GetBoolExtension(Definitions.{extData.Name});");
                                        break;

                                    case "int":
                                        writer.WriteLineIndented($"  o.GetIntegerExtension(Definitions.{extData.Name});");
                                        break;

                                    case "string":
                                        writer.WriteLineIndented($"  o.GetStringExtension(Definitions.{extData.Name});");
                                        break;

                                    default:
                                        writer.WriteLineIndented($"  o.GetExtensionValue<{type}>(Definitions.{extData.Name});");
                                        break;
                                }
                            }

                            writer.WriteLine();
                        }
                    }
                }
            }
            else
            {
                // write a comment
                if (isArray)
                {
                    writer.WriteIndentedComment(_extGetterPrefixArray + extData.Summary, isSummary: true);
                }
                else
                {
                    writer.WriteIndentedComment(_extGetterPrefixScalar + extData.Summary, isSummary: true);
                }

                writer.WriteIndentedComment(extData.Remarks, isSummary: false, isRemarks: true);

                // build the getter name
                string getterName = extData.Name.EndsWith("Extension", StringComparison.Ordinal)
                    ? extData.Name + "Get"
                    : extData.Name + "GetExt";

                // write a getter, which returns either a single extension or an enumerable of extensions
                writer.WriteLineIndented($"public static {returnType} {getterName}(this Element e) =>");
                writer.WriteLineIndented($"  e.{getAlias}(Definitions.{extData.Name});");
                writer.WriteLine();
            }
        }
    }

    private void WriteSubExtensionTupleRecurse(ExportStreamWriter writer, ExtensionData extData, string parentVarName)
    {
        if (extData.ValueElement != null)
        {
            string typeName;

            if (extData.ValueElement.Type.Any())
            {
                typeName = extData.ValueElement.Type.First().cgName();
            }
            else
            {
                typeName = extData.ValueElement.cgBaseTypeName(_info, false);
            }

            if (PrimitiveTypeMap.TryGetValue(typeName, out string? mappedType))
            {
                typeName = mappedType;
            }

            if (typeName.EndsWith('?'))
            {
                typeName = typeName.Substring(0, typeName.Length - 1);
            }

            if (extData.ValueElement.cgIsArray())
            {
                writer.WriteLineIndented($"IEnumerable<{typeName}>? val{extData.Name} = {parentVarName}" +
                    $".GetExtensions(Definitions.{extData.Name})" +
                    $".Where(e => e.Value is not null && e.Value is {typeName})" +
                    $".Select(e => ({typeName});");
            }
            else
            {
                switch (typeName)
                {
                    case "bool":
                        writer.WriteLineIndented($"{typeName}? val{extData.Name} = {parentVarName}.GetBoolExtension(Definitions.{extData.Name});");
                        break;

                    case "int":
                        writer.WriteLineIndented($"{typeName}? val{extData.Name} = {parentVarName}.GetIntegerExtension(Definitions.{extData.Name});");
                        break;

                    case "string":
                        writer.WriteLineIndented($"{typeName}? val{extData.Name} = {parentVarName}.GetStringExtension(Definitions.{extData.Name});");
                        break;

                    default:
                        writer.WriteLineIndented($"{typeName}? val{extData.Name} = {parentVarName}.GetExtensionValue<{typeName}>(Definitions.{extData.Name});");
                        break;
                }
            }

            return;
        }

        foreach (ExtensionData extensionData in extData.Children)
        {
            if (extensionData.Children.Length != 0)
            {
                writer.WriteLineIndented($"Extension? ext{extensionData.Name} = {parentVarName}.GetExtension(Definitions.{extensionData.Name});");
                writer.WriteLineIndented($"if (ext{extensionData.Name} != null)");
                OpenScope(writer);

                // nest through children
                foreach (ExtensionData child in extensionData.Children)
                {
                    WriteSubExtensionTupleRecurse(writer, child, $"ext{extensionData.Name}");
                }

                CloseScope(writer);
                continue;
            }

            WriteSubExtensionTupleRecurse(writer, extensionData, parentVarName);
        }
    }

    private List<KeyValuePair<string, string>> BuildReturnTuple(ExtensionData extData)
    {
        if (extData.ValueElement != null)
        {
            string typeName;

            if (extData.ValueElement.Type.Any())
            {
                typeName = extData.ValueElement.Type.First().cgName();
            }
            else
            {
                typeName = extData.ValueElement.cgBaseTypeName(_info, false);
            }

            if (PrimitiveTypeMap.TryGetValue(typeName, out string? mappedType))
            {
                typeName = mappedType;
            }

            if (typeName.EndsWith('?'))
            {
                typeName = typeName.Substring(0, typeName.Length - 1);
            }

            if (extData.ValueElement.cgIsArray())
            {
                typeName = "IEnumerable<" + typeName + ">";
            }

            return [new KeyValuePair<string, string>(extData.Name, typeName),];
        }

        List<KeyValuePair<string, string>> tupleTypes = [];

        foreach (ExtensionData extensionData in extData.Children)
        {
            tupleTypes.AddRange(BuildReturnTuple(extensionData));
        }

        return tupleTypes;
    }

    private bool ExtensionResolvesToSingleType(ExtensionData extData)
    {
        if (extData.ValueElement != null)
        {
            return extData.ValueElement.cgTypes().Count == 1;
        }

        if (extData.Children.Length > 0)
        {
            return extData.Children.All(ExtensionResolvesToSingleType);
        }

        return false;
    }

    /// <summary>Gets the Firely Net SDK component type.</summary>
    /// <param name="cd">The component definition.</param>
    /// <returns>The Firely Net SDK component type.</returns>
    private string GetFirelyComponentType(ComponentDefinition cd)
    {
        if (cd.IsRootOfStructure)
        {
            return cd.Structure.Name.ToPascalCase();
        }

        string structureName = cd.Structure.Name.ToPascalCase();

        if (string.IsNullOrEmpty(cd.cgExplicitName()))
        {
            return $"{structureName}.{cd.cgName(FhirNameConventionExtensions.NamingConvention.PascalCase)}Component";
        }

        return $"{structureName}.{cd.cgExplicitName()}Component";
    }


    //private void WriteProfile(StructureDefinition sd, Dictionary<string, ExtensionData> exportedExtensions)
    //{
    //    if (!_info.TryGetPackageSource(sd, out string packageId, out string packageVersion))
    //    {
    //        packageId = _info.MainPackageId;
    //        packageVersion = _info.MainPackageVersion;
    //    }

    //    if (FhirPackageUtils.PackageIsFhirCore(packageId))
    //    {
    //        // skip core packages
    //        return;
    //    }

    //    PackageData packageData = GetPackageData(packageId, packageVersion);

    //    string name = string.IsNullOrEmpty(sd.Name)
    //        ? sd.Id.ToPascalCase()
    //        : sd.Name.ToPascalCase();

    //    // remove any hyphens from the name
    //    name = name.Replace('-', '_');

    //    string filename = Path.Combine(_options.OutputDirectory, packageData.FilenamePrefix, $"{name}.cs");

    //    if (!Directory.Exists(Path.GetDirectoryName(filename)))
    //    {
    //        Directory.CreateDirectory(Path.GetDirectoryName(filename)!);
    //    }

    //    using (FileStream stream = new(filename, FileMode.Create))
    //    using (ExportStreamWriter writer = new(stream))
    //    {
    //        WriteHeader(writer, [packageData.Namespace]);

    //        WriteNamespaceOpen(writer, packageData.Namespace + ".Profiles");

    //        if (!string.IsNullOrEmpty(sd.Description))
    //        {
    //            WriteIndentedComment(writer, sd.Description, isSummary: true);
    //        }
    //        else
    //        {
    //            WriteIndentedComment(writer, sd.Title, isSummary: true);
    //        }

    //        // check for must-support elements
    //        IEnumerable<ElementDefinition> mustSupports = sd.cgElements().Where(ed => ed.MustSupport == true);

    //        if (mustSupports.Any())
    //        {
    //            writer.WriteLineIndented("/// <remarks>");
    //            writer.WriteIndentedComment("Must support elements:", isSummary: false);

    //            foreach (ElementDefinition ms in mustSupports)
    //            {
    //                writer.WriteIndentedComment($"- {ms.Path}: {ms.cgShort()}", isSummary: false);
    //            }

    //            writer.WriteLineIndented("/// </remarks>");
    //        }

    //        writer.WriteLineIndented($"public static class {name}");
    //        OpenScope(writer);      // class

    //        // write the profile URL
    //        WriteIndentedComment(writer, "The official URL for this profile.", isSummary: true);
    //        writer.WriteLineIndented($"public const string ProfileUrl = \"{sd.Url}\"");
    //        writer.WriteLine();

    //        // write the profile version
    //        if (!string.IsNullOrEmpty(sd.Version))
    //        {
    //            WriteIndentedComment(writer, "The declared version of this profile.", isSummary: true);
    //            writer.WriteLineIndented($"public const string ProfileVersion = \"{sd.Version}\"");
    //            writer.WriteLine();
    //        }
    //        else
    //        {
    //            WriteIndentedComment(writer, "The package version of this profile.", isSummary: true);
    //            writer.WriteLineIndented($"public const string ProfileVersion = \"{packageData.PackageVersion}\"");
    //            writer.WriteLine();
    //        }

    //        // check for all elements that are slices
    //        ElementDefinition[] slices = sd.cgElements(skipSlices: false).Where(ed => ed.ElementId.LastIndexOf(':') > ed.ElementId.LastIndexOf('.')).ToArray();

    //        foreach (ElementDefinition ed in slices)
    //        {
    //            string sliceName = !string.IsNullOrEmpty(ed.SliceName)
    //                ? ed.SliceName
    //                : ed.ElementId.Substring(ed.ElementId.LastIndexOf(':') + 1);

    //            // get the types for this slice
    //            ElementDefinition.TypeRefComponent[] edTypes = ed.cgTypes().Values.ToArray();

    //            foreach (ElementDefinition.TypeRefComponent edType in edTypes)
    //            {
    //                string typeName = edType.cgName();

    //                switch (typeName)
    //                {
    //                    case "Extension":
    //                        {
    //                            // iterate over each profile
    //                            foreach (string profileUrl in edType.Profile)
    //                            {
    //                                WriteProfileExtensionAccessors(
    //                                    writer,
    //                                    sd,
    //                                    name,
    //                                    ed,
    //                                    sliceName,
    //                                    profileUrl,
    //                                    exportedExtensions);
    //                            }
    //                        }
    //                        break;
    //                }
    //            }
    //        }

    //        CloseScope(writer);     // class

    //        WriteNamespaceClose(writer);
    //    }
    //}

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
    //                    writer.WriteLineIndented($"  r.{getAlias}(Definitions.{exportedExt.Name})");
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
    //                    writer.WriteLineIndented($"  c.{getAlias}(Definitions.{exportedExt.Name})");
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
    //                writer.WriteLineIndented($"  e.{getAlias}(Definitions.{exportedExt.Name})");
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

        return;

        void CloseAndDispose(ExportStreamWriter writer)
        {
            writer.CloseScope();        // class

            WriteNamespaceClose(writer);

            WriteFooter(writer);

            writer.Dispose();
        }
    }

    /// <summary>Gets package data.</summary>
    /// <param name="packageId">     Identifier for the package.</param>
    /// <param name="packageVersion">The package version.</param>
    /// <returns>The package data.</returns>
    private PackageData GetPackageData(
        string packageId,
        string packageVersion)
    {
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

            return new()
            {
                Key = packageId + "#" + packageVersion,
                PackageId = packageId,
                PackageVersion = packageVersion,
                Namespace = $"Hl7.Fhir.Packages.{realm}.{name}_{versionSanitized}",
                ClassPrefix = $"{realm.ToPascalCase()}{name.ToPascalCase()}",
                VersionSanitized = versionSanitized,
                FilenamePrefix = $"{realm.ToPascalCase()}{name.ToPascalCase()}_{versionSanitized}",
            };
        }

        return new()
        {
            Key = packageId + "#" + packageVersion,
            PackageId = packageId,
            PackageVersion = packageVersion,
            Namespace = $"Hl7.Fhir.Packages.{packageId.ToPascalCase()}_{versionSanitized}",
            ClassPrefix = packageId.ToPascalCase(),
            VersionSanitized = versionSanitized,
            FilenamePrefix = $"{packageId.ToPascalCase()}_{versionSanitized}",
        };
    }

    private ExportStreamWriter GetDefinitionWriter(PackageData packageData)
    {
        if (_definitionWriters.TryGetValue(packageData.Key, out ExportStreamWriter? writer))
        {
            return writer;
        }

        string directory = Path.Combine(_options.OutputDirectory, packageData.FilenamePrefix);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        //string filename = Path.Combine(_options.OutputDirectory, $"{packageData.FilenamePrefix}.cs");
        string filename = Path.Combine(_options.OutputDirectory, packageData.FilenamePrefix, $"Definitions.cs");

        FileStream stream = new(filename, FileMode.Create);
        writer = new ExportStreamWriter(stream);
        _definitionWriters[packageData.Key] = writer;

        WriteHeader(writer);

        WriteNamespaceOpen(writer, packageData.Namespace);

        WriteIndentedComment(writer, $"Definitions common to the {packageData.Key} package");

        writer.WriteLineIndented($"public static class Definitions");
        writer.OpenScope();

        return writer;
    }

    private ExportStreamWriter GetExtensionWriter(PackageData packageData)
    {
        if (_extensionWriters.TryGetValue(packageData.Key, out ExportStreamWriter? writer))
        {
            return writer;
        }

        string directory = Path.Combine(_options.OutputDirectory, packageData.FilenamePrefix);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string filename = Path.Combine(_options.OutputDirectory, packageData.FilenamePrefix, $"Extensions.cs");

        FileStream stream = new(filename, FileMode.Create);
        writer = new ExportStreamWriter(stream);
        _extensionWriters[packageData.Key] = writer;

        WriteHeader(writer);

        WriteNamespaceOpen(writer, packageData.Namespace);

        WriteIndentedComment(writer, $"Extension accessors for the {packageData.Key} package");

        writer.WriteLineIndented($"public static class Extensions");
        writer.OpenScope();

        return writer;
    }

    /// <summary>Writes an extension.</summary>
    /// <param name="sd">The SD.</param>
    private void WriteExtension(StructureDefinition sd, Dictionary<string, ExtensionData> exportedExtensions)
    {
        if (!_info.TryGetPackageSource(sd, out string packageId, out string packageVersion))
        {
            packageId = _info.MainPackageId;
            packageVersion = _info.MainPackageVersion;
        }

        if (FhirPackageUtils.PackageIsFhirCore(packageId))
        {
            // skip core packages
            return;
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

        // add our extension to the exported extensions
        exportedExtensions.Add(cd.Structure.Url, extData);

        // get an extension writer
        ExportStreamWriter extensionWriter = GetExtensionWriter(packageData);

        // recursively write our extension accessors
        WriteExtensionAccessors(extData, cd, extensionWriter);
    }

    /// <summary>Recursively writes extension urls.</summary>
    /// <param name="extData">     Information describing the extension.</param>
    /// <param name="writer">      The currently in-use text writer.</param>
    /// <param name="parentPrefix">(Optional) The parent prefix.</param>
    private void WriteExtensionDefinition(ExtensionData extData, ExportStreamWriter writer, string parentPrefix = "")
    {
        writer.WriteIndentedComment(extData.Summary, isSummary: true);
        writer.WriteIndentedComment(extData.Remarks, isSummary: false, isRemarks: true);

        if (string.IsNullOrEmpty(parentPrefix))
        {
            writer.WriteLineIndented($"public const string ExtUrl{extData.Name} = \"{extData.Url}\";");
        }
        else
        {
            writer.WriteLineIndented($"public const string ExtUrl{parentPrefix}{extData.Name} = \"{extData.Url}\";");
        }

        writer.WriteLine();

        // write any sub-extensions
        foreach (ExtensionData childData in extData.Children)
        {
            WriteExtensionDefinition(childData, writer, parentPrefix + extData.Name);
        }
    }


    /// <summary>Gets extension data based on a component definition.</summary>
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    /// <param name="cd">          The component definition for a root extension or slice (sub-extension).</param>
    /// <param name="parentExtUrl">(Optional) URL of the parent extent.</param>
    /// <returns>The extension data.</returns>
    private ExtensionData GetExtensionData(
        ComponentDefinition cd,
        string? parentExtUrl = null)
    {
        string summary = cd.cgShort();
        string remarks;
        bool isComplex;
        string url;
        ElementDefinition? valueElement = null;
        List<ExtensionData> childExtensions = [];

        string contexts = cd.IsRootOfStructure
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

        // check for being a 'simple' extension (no sub-extensions)
        if (extSlices.Length == 0)
        {
            // this is a simple extension
            isComplex = false;

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

            remarks = $"{contexts}\n{types}";
        }
        else
        {
            // this is a complex extension
            isComplex = true;

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
                    parentExtUrl: cd.Structure.Url));
            }

            remarks = $"{contexts}\nSub-extensions:\n - {string.Join("\n - ", childExtensions.Select(child => child.Url))}";
        }

        return new ExtensionData()
        {
            Component = cd,
            Name = cd.IsRootOfStructure ? cd.Structure.cgName() : url.ToPascalCase(),
            Url = url,
            Summary = summary,
            Remarks = remarks,
            IsComplex = isComplex,
            ValueElement = valueElement,
            Children = childExtensions.ToArray(),
        };
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
