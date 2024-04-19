// <copyright file="ConfigCli.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Collections.Generic;
using Microsoft.Health.Fhir.CodeGen.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using Microsoft.Health.Fhir.CodeGenCommon.Smart;

namespace Microsoft.Health.Fhir.CodeGen.Configuration;

/// <summary>Configuration settings when performing a single-pass generation.</summary>
public class ConfigGenerate : ConfigRoot
{
    private static readonly FhirArtifactClassEnum[] _defaultExportStructures =
    [
        FhirArtifactClassEnum.PrimitiveType,
        FhirArtifactClassEnum.ComplexType,
        FhirArtifactClassEnum.Resource,
        FhirArtifactClassEnum.Interface,
        FhirArtifactClassEnum.Extension,
        FhirArtifactClassEnum.Operation,
        FhirArtifactClassEnum.SearchParameter,
        FhirArtifactClassEnum.CodeSystem,
        FhirArtifactClassEnum.ValueSet,
        FhirArtifactClassEnum.Profile,
        FhirArtifactClassEnum.LogicalModel,
        FhirArtifactClassEnum.Compartment,
    ];

    /// <summary>Gets or sets the FHIR structures to export, default is all.</summary>
    [ConfigOption(
        ArgName = "--structures",
        EnvName = "Export_Structures",
        Description = "Types of FHIR structures to export.",
        ArgArity = "0..*")]
    public FhirArtifactClassEnum[] ExportStructures { get; set; } = _defaultExportStructures;

    //public HashSet<FhirArtifactClassEnum> ExportStructures { get; set; } = new();

    private static ConfigurationOption ExportStructuresParameter { get; } = new()
    {
        Name = "ExportStructures",
        EnvVarName = "Export_Structures",
        DefaultValue = _defaultExportStructures,
        CliOption = new System.CommandLine.Option<FhirArtifactClassEnum[]>("--structures", "Types of FHIR structures to export.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrMore,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets the export keys.</summary>
    [ConfigOption(
        ArgName = "--export-keys",
        EnvName = "Export_Keys",
        Description = "Keys of FHIR structures to export (e.g., Patient), empty means all.",
        ArgArity = "0..*")]
    public HashSet<string> ExportKeys { get; set; } = [];

    private static ConfigurationOption ExportKeysParameter { get; } = new()
    {
        Name = "ExportKeys",
        EnvVarName = "Export_Keys",
        DefaultValue = new HashSet<string>(),
        CliOption = new System.CommandLine.Option<HashSet<string>>("--export-keys", "Keys of FHIR structures to export (e.g., Patient), empty means all.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrMore,
            IsRequired = false,
        },
    };

    /// <summary>Gets a value indicating whether the experimental should be included.</summary>
    [ConfigOption(
        ArgAliases = ["--include-experimental", "--experimental"],
        EnvName = "Include_Experimental",
        Description = "If the output should include structures marked experimental.")]
    public bool IncludeExperimental { get; set; } = false;

    private static ConfigurationOption IncludeExperimentalParameter { get; } = new()
    {
        Name = "IncludeExperimental",
        EnvVarName = "Include_Experimental",
        DefaultValue = false,
        CliOption = new System.CommandLine.Option<bool>([ "--include-experimental", "--experimental" ], "If the output should include structures marked experimental.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets URL of the FHIR server.</summary>
    [ConfigOption(
        ArgName = "--fhir-server-url",
        EnvName = "Fhir_Server_Url",
        Description = "FHIR Server URL to pull a CapabilityStatement (or Conformance) from. Requires application/fhir+json.")]
    public string FhirServerUrl { get; set; } = string.Empty;

    private static ConfigurationOption FhirServerUrlParameter { get; } = new()
    {
        Name = "FhirServerUrl",
        EnvVarName = "Fhir_Server_Url",
        DefaultValue = string.Empty,
        CliOption = new System.CommandLine.Option<string>("--fhir-server-url", "FHIR Server URL to pull a CapabilityStatement (or Conformance) from. Requires application/fhir+json.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets the FHIR server headers.</summary>
    [ConfigOption(
        ArgName = "--fhir-server-header",
        EnvName = "Fhir_Server_Header",
        Description = "FHIR Server headers to use when pulling a CapabilityStatement (or Conformance) from a FHIR server.")]
    public List<string> FhirServerHeaders { get; set; } = [];

    private static ConfigurationOption FhirServerHeadersParameter { get; } = new()
    {
        Name = "FhirServerHeader",
        EnvVarName = "Fhir_Server_Header",
        DefaultValue = string.Empty,
        CliOption = new System.CommandLine.Option<string>("--fhir-server-header", "FHIR Server headers to use when pulling a CapabilityStatement (or Conformance) from a FHIR server.  Use <key>=<value> format.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrMore,
            IsRequired = false,
        },
    };

    [ConfigOption(
        ArgName = "--resolve-server-canonicals",
        EnvName = "Resolve_Server_Canonicals",
        Description = "If canonical URLs used in server Capability Statement should be resolved.")]
    public bool ResolveServerCanonicals { get; set; } = true;

    private static ConfigurationOption ResolveServerCanonicalsParameter { get; } = new()
    {
        Name = "ResolveServerCanonicals",
        EnvVarName = "Resolve_Server_Canonicals",
        DefaultValue = true,
        CliOption = new System.CommandLine.Option<bool>("--resolve-server-canonicals", "If canonical URLs used in server Capability Statement should be resolved.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    [ConfigOption(
        ArgName = "--resolve-external-canonicals",
        EnvName = "Resolve_External_Canonicals",
        Description = "If canonical URLs external to the server should be resolved.")]
    public bool ResolveExternalCanonicals { get; set; } = false;

    private static ConfigurationOption ResolveExternalCanonicalsParameter { get; } = new()
    {
        Name = "ResolveExternalCanonicals",
        EnvVarName = "Resolve_External_Canonicals",
        DefaultValue = false,
        CliOption = new System.CommandLine.Option<bool>("--resolve-external-canonicals", "If canonical URLs external to the server should be resolved.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets the server capabilities, retrieved prior to export for version-conversion.</summary>
    public Hl7.Fhir.Model.CapabilityStatement? ServerCapabilities { get; set; }

    /// <summary>Gets or sets the server FHIR sequence.</summary>
    public FhirReleases.FhirSequenceCodes? ServerFhirSequence { get; set; }

    /// <summary>Gets or sets the server smart configuration, retrieved prior to export if possible.</summary>
    public SmartWellKnown? ServerSmartConfig { get; set; }

    /// <summary>(Immutable) Options that get promoted to user-settable options based on this type.</summary>
    private static readonly ConfigurationOption[] _options =
    [
        ExportStructuresParameter,
        ExportKeysParameter,
        IncludeExperimentalParameter,
        FhirServerUrlParameter,
        FhirServerHeadersParameter,
        ResolveServerCanonicalsParameter,
        ResolveExternalCanonicalsParameter,
    ];

    /// <summary>
    /// Gets the configuration options for the current instance and its base class.
    /// </summary>
    /// <returns>An array of configuration options.</returns>
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
                case "ExportStructures":
                    ExportStructures = GetOptArray(parseResult, opt.CliOption, ExportStructures);
                    break;
                case "ExportKeys":
                    ExportKeys = GetOptHash(parseResult, opt.CliOption, ExportKeys);
                    break;
                case "IncludeExperimental":
                    IncludeExperimental = GetOpt(parseResult, opt.CliOption, IncludeExperimental);
                    break;
                case "FhirServerUrl":
                    FhirServerUrl = GetOpt(parseResult, opt.CliOption, FhirServerUrl);
                    break;
                case "FhirServerHeaders":
                    FhirServerHeaders = GetOpt(parseResult, opt.CliOption, FhirServerHeaders);
                    break;
                case "ResolveServerCanonicals":
                    ResolveServerCanonicals = GetOpt(parseResult, opt.CliOption, ResolveServerCanonicals);
                    break;
                case "ResolveExternalCanonicals":
                    ResolveExternalCanonicals = GetOpt(parseResult, opt.CliOption, ResolveExternalCanonicals);
                    break;
            }
        }
    }

}
