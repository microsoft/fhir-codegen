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

    /// <summary>The currently in-use text writer.</summary>
    private ExportStreamWriter _writer = null!;

    private Dictionary<string, ExportStreamWriter> _packageWriters = [];

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

        string extensionListingFilename = Path.Combine(options.OutputDirectory, "PackageExtensions.cs");

        // write extension contents
        WriteExtensions(_info.ExtensionsByUrl.Values);

        CloseWriters();

    }

    private void CloseWriters()
    {
        foreach (ExportStreamWriter writer in _packageWriters.Values)
        {
            WriteNamespaceClose(writer);

            WriteFooter(writer);

            writer.Dispose();
        }
    }

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
                Key = packageId + "#"+ packageVersion,
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

    private ExportStreamWriter GetPackageMainWriter(
        PackageData packageData,
        out bool isNew)
    {
        if (_packageWriters.TryGetValue(packageData.Key, out ExportStreamWriter? writer))
        {
            isNew = false;
        }
        else
        {
            string filename = Path.Combine(_options.OutputDirectory, $"{packageData.FilenamePrefix}.cs");

            FileStream stream = new(filename, FileMode.Create);
            writer = new ExportStreamWriter(stream);
            _packageWriters[packageData.Key] = writer;
            isNew = true;
        }

        return writer;
    }

    private void WriteExtensions(IEnumerable<StructureDefinition> extensions)
    {
        foreach (StructureDefinition sd in extensions)
        {
            WriteExtension(sd);
        }
    }


    private void WriteExtension(StructureDefinition sd)
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

        ExportStreamWriter writer = GetPackageMainWriter(packageData, out bool isNew);

        if (isNew)
        {
            WriteHeader(writer);

            WriteNamespaceOpen(writer, packageData.Namespace);
        }

        // build a component for our extension
        ComponentDefinition cd = new(sd);

        // get our extension data
        ExtensionData extData = GetExtensionData(cd);

        // recursively write our extension
        WriteExtensionData(extData, writer);
    }

    private void WriteExtensionData(ExtensionData extData, ExportStreamWriter writer, string parentPrefix = "")
    {
        writer.WriteLine();
        writer.WriteIndentedComment(extData.Summary, isSummary: true);
        writer.WriteIndentedComment(extData.Remarks, isSummary: false, isRemarks: true);

        if (string.IsNullOrEmpty(parentPrefix))
        {
            writer.WriteLineIndented($"public const string ExtUrl{extData.Name} = \"{extData.Url}\"");
        }
        else
        {
            writer.WriteLineIndented($"public const string ExtUrl{parentPrefix}{extData.Name} = \"{extData.Url}\"");
        }

        // write any sub-extensions
        foreach (ExtensionData childData in extData.Children)
        {
            WriteExtensionData(childData, writer, parentPrefix + extData.Name);
        }
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

    private string GetContextString(StructureDefinition.ContextComponent ctx) => ctx switch
    {
        { Type: StructureDefinition.ExtensionContextType.Fhirpath } => $"FhirPath({ctx.Expression})",
        { Type: StructureDefinition.ExtensionContextType.Element } => ctx.Expression,
        { Type: StructureDefinition.ExtensionContextType.Extension } => $"Extension({ctx.Expression})",
        _ => $"{ctx.Type}({ctx.Expression})"
    };


    private string SanitizeVersion(string version)
    {
        return version.Replace('.', '_').Replace('-', '_');
    }


    /// <summary>Writes the namespace open.</summary>
    private void WriteNamespaceOpen(ExportStreamWriter writer, string ns)
    {
        writer.WriteLineIndented(ns);
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

    private void WriteHeader(ExportStreamWriter writer)
    {
        WriteGenerationComment(writer);

        writer.WriteLineIndented("using System;");
        writer.WriteLineIndented("using System.Collections.Generic;");
        writer.WriteLineIndented("using System.Linq;");
        writer.WriteLineIndented("using System.Runtime.Serialization;");
        writer.WriteLineIndented("using Hl7.Fhir.Model;");
        writer.WriteLine(string.Empty);

        //WriteCopyright(writer);
    }

    /// <summary>Writes the generation comment.</summary>
    private void WriteGenerationComment(ExportStreamWriter writer)
    {
        writer.WriteLineIndented("// <auto-generated/>");
        writer.WriteLineIndented($"// Contents of: {string.Join(", ", _info.Manifests.Select(kvp => kvp.Key))}");

        if (_options.ExportKeys.Count != 0)
        {
            string restrictions = string.Join("|", _options.ExportKeys);
            _writer.WriteLine($"  // Restricted to: {restrictions}");
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
