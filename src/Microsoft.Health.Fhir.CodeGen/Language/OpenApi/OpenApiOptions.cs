// <copyright file="OpenApiOptions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGen.Configuration;
using Microsoft.Health.Fhir.CodeGen.Extensions;
using static Microsoft.Health.Fhir.CodeGen.Language.OpenApi.OpenApiCommon;

namespace Microsoft.Health.Fhir.CodeGen.Language.OpenApi;

/// <summary>An open API options.</summary>
public class OpenApiOptions : ConfigGenerate
{
    /// <summary>Gets or sets the open API version.</summary>
    [ConfigOption(
        ArgName = "--oas-version",
        Description = "Open API version to use.")]
    public OaVersion OpenApiVersion { get; set; } = OaVersion.v2;

    private static ConfigurationOption OpenApiVersionParameter { get; } = new()
    {
        Name = "OasVersion",
        DefaultValue = OaVersion.v2,
        CliOption = new System.CommandLine.Option<OaVersion>("--oas-version", "Open API version to export as.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets the file format.</summary>
    [ConfigOption(
        ArgName = "--format",
        Description = "File format to export.")]
    public OaFileFormat FileFormat { get; set; } = OaFileFormat.JSON;

    private static ConfigurationOption FileFormatParameter { get; } = new()
    {
        Name = "FileFormat",
        DefaultValue = OaFileFormat.JSON,
        CliOption = new System.CommandLine.Option<OaFileFormat>("--format", "File format to export.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    [ConfigOption(
        ArgName = "--title",
        Description = "Title to use in Info section, defaults to 'FHIR [FhirSequence].[VersionString]'.")]
    public string Title { get; set; } = "";

    private static ConfigurationOption TitleParameter { get; } = new()
    {
        Name = "Title",
        DefaultValue = "",
        CliOption = new System.CommandLine.Option<string>("--title", "Title to use in Info section, defaults to 'FHIR [FhirSequence].[VersionString]'.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    [ConfigOption(
        ArgName = "--definition-version",
        Description = "Version number to use in the OpenAPI file, defaults to '[FHIR Version]'")]
    public string DefinitionVersion { get; set; } = "";

    private static ConfigurationOption DefinitionVersionParameter { get; } = new()
    {
        Name = "DefinitionVersion",
        DefaultValue = "",
        CliOption = new System.CommandLine.Option<string>("--definition-version", "Version number to use in the OpenAPI file, defaults to '[FHIR Version]'")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets the FHIR structures to export, default is all.</summary>
    [ConfigOption(
        ArgName = "--extension-support",
        Description = "The level of extensions to include.")]
    public ExtensionSupportLevel ExtensionSupport { get; set; } = ExtensionSupportLevel.NonPrimitive;

    private static ConfigurationOption ExtensionSupportParameter { get; } = new()
    {
        Name = "ExtensionSupport",
        DefaultValue = ExtensionSupportLevel.NonPrimitive,
        CliOption = new System.CommandLine.Option<ExtensionSupportLevel>("--extension-support", "The level of extensions to include.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets the schema level.</summary>
    [ConfigOption(
        ArgName = "--schema-level",
        Description = "The level of detail to include in the schema.")]
    public OaSchemaLevelCodes SchemaLevel { get; set; } = OaSchemaLevelCodes.Names;

    private static ConfigurationOption SchemaLevelParameter { get; } = new()
    {
        Name = "SchemaLevel",
        DefaultValue = OaSchemaLevelCodes.Names,
        CliOption = new System.CommandLine.Option<OaSchemaLevelCodes>("--schema-level", "The level of detail to include in the schema.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets the schema style.</summary>
    [ConfigOption(
        ArgName = "--schema-style",
        Description = "The style of schema to use.")]
    public OaSchemaStyleCodes SchemaStyle { get; set; } = OaSchemaStyleCodes.TypeReferences;

    private static ConfigurationOption SchemaStyleParameter { get; } = new()
    {
        Name = "SchemaStyle",
        DefaultValue = OaSchemaStyleCodes.TypeReferences,
        CliOption = new System.CommandLine.Option<OaSchemaStyleCodes>("--schema-style", "The style of schema to use.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets the maximum recursions.</summary>
    [ConfigOption(
            ArgName = "--max-recursions",
            Description = "The maximum depth to expand recursions.")]
    public int MaxRecursions { get; set; } = 0;

    private static ConfigurationOption MaxRecursionsParameter { get; } = new()
    {
        Name = "MaxRecursions",
        DefaultValue = 0,
        CliOption = new System.CommandLine.Option<int>("--max-recursions", "The maximum depth to expand recursions.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets a list of types of the FHIR mimes.</summary>
    [ConfigOption(
               ArgName = "--fhir-mime-types",
               Description = "Which FHIR MIME types to support.")]
    public OaFhirMimeCodes FhirMimeTypes { get; set; } = OaFhirMimeCodes.Capabilities;

    private static ConfigurationOption FhirMimeTypesParameter { get; } = new()
    {
        Name = "FhirMimeTypes",
        DefaultValue = OaFhirMimeCodes.Capabilities,
        CliOption = new System.CommandLine.Option<OaFhirMimeCodes>("--fhir-mime-types", "Which FHIR MIME types to support.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets a list of types of the patch mimes.</summary>
    [ConfigOption(
        ArgName = "--patch-mime-types",
        Description = "Which FHIR Patch MIME types to support.")]
    public OaPatchMimeCodes PatchMimeTypes { get; set; } = OaPatchMimeCodes.Capabilities;

    private static ConfigurationOption PatchMimeTypesParameter { get; } = new()
    {
        Name = "PatchMimeTypes",
        DefaultValue = OaPatchMimeCodes.Capabilities,
        CliOption = new System.CommandLine.Option<OaPatchMimeCodes>("--patch-mime-types", "Which FHIR Patch MIME types to support.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets the search support.</summary>
    [ConfigOption(
        ArgName = "--search-support",
        Description = "Which HTTP methods to support in search.")]
    public OaHttpMethods SearchSupport { get; set; } = OaHttpMethods.Both;

    private static ConfigurationOption SearchSupportParameter { get; } = new()
    {
        Name = "SearchSupport",
        DefaultValue = OaHttpMethods.Both,
        CliOption = new System.CommandLine.Option<OaHttpMethods>("--search-support", "Which HTTP methods to support in search.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>
    /// Gets or sets a value indicating whether the search parameters should be exported.
    /// </summary>
    [ConfigOption(
        ArgName = "--export-search-params",
        Description = "If search parameters should be included in the schema.")]
    public bool ExportSearchParams { get; set; } = true;

    private static ConfigurationOption ExportSearchParamsParameter { get; } = new()
    {
        Name = "ExportSearchParams",
        DefaultValue = true,
        CliOption = new System.CommandLine.Option<bool>("--export-search-params", "If search parameters should be included in the schema.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets the post search parameter location.</summary>
    [ConfigOption(
        ArgName = "--post-search-param-location",
        Description = "Where to put search parameters in POST requests.")]
    public OaSearchPostParameterLocationCodes PostSearchParamLocation { get; set; } = OaSearchPostParameterLocationCodes.Body;

    private static ConfigurationOption PostSearchParamLocationParameter { get; } = new()
    {
        Name = "PostSearchParamLocation",
        DefaultValue = OaSearchPostParameterLocationCodes.Body,
        CliOption = new System.CommandLine.Option<OaSearchPostParameterLocationCodes>("--post-search-param-location", "Where to put search parameters in POST requests.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>
    /// Gets or sets a value indicating whether the consolidate search parameters.
    /// </summary>
    [ConfigOption(
        ArgName = "--consolidate-search-parameters",
        Description = "If search parameters should be consolidated.")]
    public bool ConsolidateSearchParameters { get; set; } = true;

    private static ConfigurationOption ConsolidateSearchParametersParameter { get; } = new()
    {
        Name = "ConsolidateSearchParameters",
        DefaultValue = true,
        CliOption = new System.CommandLine.Option<bool>("--consolidate-search-parameters", "If search parameters should be consolidated.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets the operation support.</summary>
    [ConfigOption(
        ArgName = "--operation-support",
        Description = "Which HTTP methods to support in operations.")]
    public OaHttpMethods OperationSupport { get; set; } = OaHttpMethods.Both;

    private static ConfigurationOption OperationSupportParameter { get; } = new()
    {
        Name = "OperationSupport",
        DefaultValue = OaHttpMethods.Both,
        CliOption = new System.CommandLine.Option<OaHttpMethods>("--operation-support", "Which HTTP methods to support in operations.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets the update create.</summary>
    [ConfigOption(
        ArgName = "--update-create",
        Description = "If update can commit to a new identity.")]
    public OaCapabilityBoolean UpdateCreate { get; set; } = OaCapabilityBoolean.Capabilities;

    private static ConfigurationOption UpdateCreateParameter { get; } = new()
    {
        Name = "UpdateCreate",
        DefaultValue = OaCapabilityBoolean.Capabilities,
        CliOption = new System.CommandLine.Option<OaCapabilityBoolean>("--update-create", "If update can commit to a new identity.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    [ConfigOption(
        ArgName = "--conditional-read",
        Description = "Override the capability statement conditional read support.")]
    public CapabilityStatement.ConditionalReadStatus? ConditionalRead { get; set; } = null;

    private static ConfigurationOption ConditionalReadParameter { get; } = new()
    {
        Name = "ConditionalRead",
        DefaultValue = null!,
        CliOption = new System.CommandLine.Option<CapabilityStatement.ConditionalReadStatus>("--conditional-read", "Override the capability statement conditional read support.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets the interaction read.</summary>
    [ConfigOption(
        ArgName = "--read",
        Description = "If the read interaction is supported.")]
    public OaCapabilityBoolean InteractionRead { get; set; } = OaCapabilityBoolean.Capabilities;

    private static ConfigurationOption InteractionReadParameter { get; } = new()
    {
        Name = "InteractionRead",
        DefaultValue = OaCapabilityBoolean.Capabilities,
        CliOption = new System.CommandLine.Option<OaCapabilityBoolean>("--read", "If the read interaction is supported.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets the interaction v read.</summary>
    [ConfigOption(
        ArgName = "--vread",
        Description = "If the version-read interaction is supported.")]
    public OaCapabilityBoolean InteractionVRead { get; set; } = OaCapabilityBoolean.Capabilities;

    private static ConfigurationOption InteractionVReadParameter { get; } = new()
    {
        Name = "InteractionVRead",
        DefaultValue = OaCapabilityBoolean.Capabilities,
        CliOption = new System.CommandLine.Option<OaCapabilityBoolean>("--vread", "If the version-read interaction is supported.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets the interaction update.</summary>
    [ConfigOption(
        ArgName = "--update",
        Description = "If the update interaction is supported.")]
    public OaCapabilityBoolean InteractionUpdate { get; set; } = OaCapabilityBoolean.Capabilities;

    private static ConfigurationOption InteractionUpdateParameter { get; } = new()
    {
        Name = "InteractionUpdate",
        DefaultValue = OaCapabilityBoolean.Capabilities,
        CliOption = new System.CommandLine.Option<OaCapabilityBoolean>("--update", "If the update interaction is supported.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets the interaction update conditional.</summary>
    [ConfigOption(
        ArgName = "--update-conditional",
        Description = "If the conditional update interaction is supported.")]
    public OaCapabilityBoolean InteractionUpdateConditional { get; set; } = OaCapabilityBoolean.Capabilities;

    private static ConfigurationOption InteractionUpdateConditionalParameter { get; } = new()
    {
        Name = "InteractionUpdateConditional",
        DefaultValue = OaCapabilityBoolean.Capabilities,
        CliOption = new System.CommandLine.Option<OaCapabilityBoolean>("--update-conditional", "If the conditional update interaction is supported.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets the interaction patch.</summary>
    [ConfigOption(
        ArgName = "--patch",
        Description = "If the patch interaction is supported.")]
    public OaCapabilityBoolean InteractionPatch { get; set; } = OaCapabilityBoolean.Capabilities;

    private static ConfigurationOption InteractionPatchParameter { get; } = new()
    {
        Name = "InteractionPatch",
        DefaultValue = OaCapabilityBoolean.Capabilities,
        CliOption = new System.CommandLine.Option<OaCapabilityBoolean>("--patch", "If the patch interaction is supported.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets the interaction patch conditional.</summary>
    [ConfigOption(
        ArgName = "--patch-conditional",
        Description = "If the conditional patch interaction is supported.")]
    public OaCapabilityBoolean InteractionPatchConditional { get; set; } = OaCapabilityBoolean.Capabilities;

    private static ConfigurationOption InteractionPatchConditionalParameter { get; } = new()
    {
        Name = "InteractionPatchConditional",
        DefaultValue = OaCapabilityBoolean.Capabilities,
        CliOption = new System.CommandLine.Option<OaCapabilityBoolean>("--patch-conditional", "If the conditional patch interaction is supported.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets the interaction delete.</summary>
    [ConfigOption(
        ArgName = "--delete",
        Description = "If the delete interaction is supported.")]
    public OaCapabilityBoolean InteractionDelete { get; set; } = OaCapabilityBoolean.Capabilities;

    private static ConfigurationOption InteractionDeleteParameter { get; } = new()
    {
        Name = "InteractionDelete",
        DefaultValue = OaCapabilityBoolean.Capabilities,
        CliOption = new System.CommandLine.Option<OaCapabilityBoolean>("--delete", "If the delete interaction is supported.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets the interaction delete conditional single.</summary>
    [ConfigOption(
        ArgName = "--delete-conditional-single",
        Description = "If the conditional delete interaction is supported.")]
    public OaCapabilityBoolean InteractionDeleteConditionalSingle { get; set; } = OaCapabilityBoolean.Capabilities;

    private static ConfigurationOption InteractionDeleteConditionalSingleParameter { get; } = new()
    {
        Name = "InteractionDeleteConditionalSingle",
        DefaultValue = OaCapabilityBoolean.Capabilities,
        CliOption = new System.CommandLine.Option<OaCapabilityBoolean>("--delete-conditional-single", "If the conditional delete interaction is supported.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets the interaction delete conditional multiple.</summary>
    [ConfigOption(
        ArgName = "--delete-conditional-multiple",
        Description = "If the conditional delete interaction is supported.")]
    public OaCapabilityBoolean InteractionDeleteConditionalMultiple { get; set; } = OaCapabilityBoolean.Capabilities;

    private static ConfigurationOption InteractionDeleteConditionalMultipleParameter { get; } = new()
    {
        Name = "InteractionDeleteConditionalMultiple",
        DefaultValue = OaCapabilityBoolean.Capabilities,
        CliOption = new System.CommandLine.Option<OaCapabilityBoolean>("--delete-conditional-multiple", "If the conditional delete interaction is supported.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets the interaction delete history.</summary>
    [ConfigOption(
        ArgName = "--delete-history",
        Description = "If the delete history interaction is supported.")]
    public OaCapabilityBoolean InteractionDeleteHistory { get; set; } = OaCapabilityBoolean.Capabilities;

    private static ConfigurationOption InteractionDeleteHistoryParameter { get; } = new()
    {
        Name = "InteractionDeleteHistory",
        DefaultValue = OaCapabilityBoolean.Capabilities,
        CliOption = new System.CommandLine.Option<OaCapabilityBoolean>("--delete-history", "If the delete history interaction is supported.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };


    /// <summary>Gets or sets the interaction delete history version.</summary>
    [ConfigOption(
        ArgName = "--delete-history-version",
        Description = "If the delete history version interaction is supported.")]
    public OaCapabilityBoolean InteractionDeleteHistoryVersion { get; set; } = OaCapabilityBoolean.Capabilities;

    private static ConfigurationOption InteractionDeleteHistoryVersionParameter { get; } = new()
    {
        Name = "InteractionDeleteHistoryVersion",
        DefaultValue = OaCapabilityBoolean.Capabilities,
        CliOption = new System.CommandLine.Option<OaCapabilityBoolean>("--delete-history-version", "If the delete history version interaction is supported.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };


    /// <summary>Gets or sets the interaction history instance.</summary>
    [ConfigOption(
        ArgName = "--history-instance",
        Description = "If history instance read is supported.")]
    public OaCapabilityBoolean InteractionHistoryInstance { get; set; } = OaCapabilityBoolean.Capabilities;

    private static ConfigurationOption InteractionHistoryInstanceParameter { get; } = new()
    {
        Name = "InteractionHistoryInstance",
        DefaultValue = OaCapabilityBoolean.Capabilities,
        CliOption = new System.CommandLine.Option<OaCapabilityBoolean>("--history-instance-read", "If history instance read is supported.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets the type of the interaction history.</summary>
    [ConfigOption(
        ArgName = "--history-type",
        Description = "If reading the history for a resource type is supported.")]
    public OaCapabilityBoolean InteractionHistoryType { get; set; } = OaCapabilityBoolean.Capabilities;

    private static ConfigurationOption InteractionHistoryTypeParameter { get; } = new()
    {
        Name = "InteractionHistoryType",
        DefaultValue = OaCapabilityBoolean.Capabilities,
        CliOption = new System.CommandLine.Option<OaCapabilityBoolean>("--history-type", "If history for a resource type is supported.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    [ConfigOption(
        ArgName = "--history-system",
        Description = "If reading the history for a system is supported.")]
    public OaCapabilityBoolean InteractionHistorySystem { get; set; } = OaCapabilityBoolean.Capabilities;

    private static ConfigurationOption InteractionHistorySystemParameter { get; } = new()
    {
        Name = "InteractionHistorySystem",
        DefaultValue = OaCapabilityBoolean.Capabilities,
        CliOption = new System.CommandLine.Option<OaCapabilityBoolean>("--history-system", "If history for a system is supported.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };


    /// <summary>Gets or sets the interaction create.</summary>
    [ConfigOption(
        ArgName = "--create",
        Description = "If the create interaction is support.")]
    public OaCapabilityBoolean InteractionCreate { get; set; } = OaCapabilityBoolean.Capabilities;
    private static ConfigurationOption InteractionCreateParameter { get; } = new()
    {
        Name = "InteractionCreate",
        DefaultValue = OaCapabilityBoolean.Capabilities,
        CliOption = new System.CommandLine.Option<OaCapabilityBoolean>("--create", "If the create interaction is supported.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets the interaction create conditional.</summary>
    [ConfigOption(
        ArgName = "--create-conditional",
        Description = "If the conditional create interaction is supported.")]
    public OaCapabilityBoolean InteractionCreateConditional { get; set; } = OaCapabilityBoolean.Capabilities;

    private static ConfigurationOption InteractionCreateConditionalParameter { get; } = new()
    {
        Name = "InteractionCreateConditional",
        DefaultValue = OaCapabilityBoolean.Capabilities,
        CliOption = new System.CommandLine.Option<OaCapabilityBoolean>("--create-conditional", "If the conditional create interaction is supported.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets the type of the interaction search.</summary>
    [ConfigOption(
        ArgName = "--search-type",
        Description = "If the type search interaction is supported.")]
    public OaCapabilityBoolean InteractionSearchType { get; set; } = OaCapabilityBoolean.Capabilities;

    private static ConfigurationOption InteractionSearchTypeParameter { get; } = new()
    {
        Name = "InteractionSearchType",
        DefaultValue = OaCapabilityBoolean.Capabilities,
        CliOption = new System.CommandLine.Option<OaCapabilityBoolean>("--search-type", "If the type search interaction is supported.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    [ConfigOption(
        ArgName = "--search-system",
        Description = "If the system search interaction is supported.")]
    public OaCapabilityBoolean InteractionSearchSystem { get; set; } = OaCapabilityBoolean.Capabilities;

    private static ConfigurationOption InteractionSearchSystemParameter { get; } = new()
    {
        Name = "InteractionSearchSystem",
        DefaultValue = OaCapabilityBoolean.Capabilities,
        CliOption = new System.CommandLine.Option<OaCapabilityBoolean>("--search-system", "If the system search interaction is supported.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    [ConfigOption(
        ArgName = "--search-compartment",
        Description = "If the compartment search interaction is supported.")]
    public OaCapabilityBoolean InteractionSearchCompartment { get; set; } = OaCapabilityBoolean.Capabilities;

    private static ConfigurationOption InteractionSearchCompartmentParameter { get; } = new()
    {
        Name = "InteractionSearchCompartment",
        DefaultValue = OaCapabilityBoolean.Capabilities,
        CliOption = new System.CommandLine.Option<OaCapabilityBoolean>("--search-compartment", "If the compartment search interaction is supported.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    [ConfigOption(
        ArgName = "--operation-system",
        Description = "If system-level operation interactions are supported.")]
    public OaCapabilityBoolean InteractionOperationSystem { get; set; } = OaCapabilityBoolean.Capabilities;

    private static ConfigurationOption InteractionOperationSystemParameter { get; } = new()
    {
        Name = "InteractionOperationSystem",
        DefaultValue = OaCapabilityBoolean.Capabilities,
        CliOption = new System.CommandLine.Option<OaCapabilityBoolean>("--operation-system", "If system-level operation interactions are supported.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    [ConfigOption(
               ArgName = "--operation-type",
               Description = "If type-level operation interactions are supported.")]
    public OaCapabilityBoolean InteractionOperationType { get; set; } = OaCapabilityBoolean.Capabilities;

    private static ConfigurationOption InteractionOperationTypeParameter { get; } = new()
    {
        Name = "InteractionOperationType",
        DefaultValue = OaCapabilityBoolean.Capabilities,
        CliOption = new System.CommandLine.Option<OaCapabilityBoolean>("--operation-type", "If type-level operation interactions are supported.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    [ConfigOption(
               ArgName = "--operation-instance",
               Description = "If instance-level operation interactions are supported.")]
    public OaCapabilityBoolean InteractionOperationInstance { get; set; } = OaCapabilityBoolean.Capabilities;

    private static ConfigurationOption InteractionOperationInstanceParameter { get; } = new()
    {
        Name = "InteractionOperationInstance",
        DefaultValue = OaCapabilityBoolean.Capabilities,
        CliOption = new System.CommandLine.Option<OaCapabilityBoolean>("--operation-instance", "If instance-level operation interactions are supported.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets a value indicating whether the system supports the metadata endpoint.</summary>
    [ConfigOption(
        ArgName = "--metadata",
        Description = "If the metadata endpoint should be included in the schema.")]
    public OaCapabilityBoolean InteractionCapabilities { get; set; } = OaCapabilityBoolean.Capabilities;

    private static ConfigurationOption InteractionCapabilitiesParameter { get; } = new()
    {
        Name = "SupportMetadata",
        DefaultValue = true,
        CliOption = new System.CommandLine.Option<bool>("--metadata", "If the metadata endpoint should be included in the schema.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    [ConfigOption(
        ArgName = "--batch",
        Description = "If the batch endpoint should be included in the schema.")]
    public OaCapabilityBoolean InteractionBatch { get; set; } = OaCapabilityBoolean.Capabilities;

    private static ConfigurationOption InteractionBatchParameter { get; } = new()
    {
        Name = "SupportBatch",
        DefaultValue = true,
        CliOption = new System.CommandLine.Option<bool>("--batch", "If the batch endpoint should be included in the schema.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    [ConfigOption(
        ArgName = "--transaction",
        Description = "If the transaction endpoint should be included in the schema.")]
    public OaCapabilityBoolean InteractionTransaction { get; set; } = OaCapabilityBoolean.Capabilities;

    private static ConfigurationOption InteractionTransactionParameter { get; } = new()
    {
        Name = "SupportTransaction",
        DefaultValue = true,
        CliOption = new System.CommandLine.Option<bool>("--transaction", "If the transaction endpoint should be included in the schema.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets a value indicating whether the support bundle.</summary>
    [ConfigOption(
        ArgName = "--bundle",
        Description = "If the bundle endpoint should be included in the schema.")]
    public OaCapabilityBoolean SupportBundle { get; set; } = OaCapabilityBoolean.Capabilities;

    private static ConfigurationOption SupportBundleParameter { get; } = new()
    {
        Name = "SupportBundle",
        DefaultValue = true,
        CliOption = new System.CommandLine.Option<bool>("--bundle", "If the bundle endpoint should be included in the schema.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>
    /// Gets or sets a value indicating whether the read only should be exported.
    /// </summary>
    [ConfigOption(
        ArgName = "--read-only",
        Description = "If the export should only contain HTTP GET support.")]
    public bool ExportReadOnly { get; set; } = false;

    private static ConfigurationOption ExportReadOnlyParameter { get; } = new()
    {
        Name = "ExportReadOnly",
        DefaultValue = false,
        CliOption = new System.CommandLine.Option<bool>("--read-only", "If the export should only contain HTTP GET support.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>
    /// Gets or sets a value indicating whether the write only should be exported.
    /// </summary>
    [ConfigOption(
        ArgName = "--write-only",
        Description = "If the export should only contain HTTP POST, PUT, PATCH, and DELETE support.")]
    public bool ExportWriteOnly { get; set; } = false;

    private static ConfigurationOption ExportWriteOnlyParameter { get; } = new()
    {
        Name = "ExportWriteOnly",
        DefaultValue = false,
        CliOption = new System.CommandLine.Option<bool>("--write-only", "If the export should only contain HTTP POST, PUT, PATCH, and DELETE support.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>
    /// Gets or sets a value indicating whether the descriptions should be included.
    /// </summary>
    [ConfigOption(
        ArgName = "--descriptions",
        Description = "If properties should include descriptions.")]
    public bool IncludeDescriptions { get; set; } = true;
    private static ConfigurationOption IncludeDescriptionsParameter { get; } = new()
    {
        Name = "PropertyDescriptions",
        DefaultValue = true,
        CliOption = new System.CommandLine.Option<bool>("--descriptions", "If properties should include descriptions.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets the length of the description maximum.</summary>
    [ConfigOption(
        ArgName = "--description-max-length",
        Description = "The maximum length of a description.")]
    public int DescriptionMaxLength { get; set; } = 60;

    private static ConfigurationOption DescriptionMaxLengthParameter { get; } = new()
    {
        Name = "DescriptionMaxLength",
        DefaultValue = 60,
        CliOption = new System.CommandLine.Option<int>("--description-max-length", "The maximum length of a description.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>
    /// Gets or sets a value indicating whether the description validation is performed.
    /// </summary>
    [ConfigOption(
        ArgName = "--description-validation",
        Description = "If descriptions are required and should be validated.")]
    public bool PerformDescriptionValidation { get; set; } = false;

    private static ConfigurationOption PerformDescriptionValidationParameter { get; } = new()
    {
        Name = "PerformDescriptionValidation",
        DefaultValue = false,
        CliOption = new System.CommandLine.Option<bool>("--description-validation", "If descriptions are required and should be validated.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets a value indicating whether the expand profiles.</summary>
    [ConfigOption(
        ArgName = "--expand-profiles",
        Description = "If profiles should be expanded.")]
    public bool ExpandProfiles { get; set; } = true;

    private static ConfigurationOption ExpandProfilesParameter { get; } = new()
    {
        Name = "ExpandProfiles",
        DefaultValue = true,
        CliOption = new System.CommandLine.Option<bool>("--expand-profiles", "If profiles should be expanded.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets a value indicating whether the expand references.</summary>
    [ConfigOption(
        ArgName = "--expand-references",
        Description = "If references should be expanded.")]
    public bool ExpandReferences { get; set; } = true;

    private static ConfigurationOption ExpandReferencesParameter { get; } = new()
    {
        Name = "ExpandReferences",
        DefaultValue = true,
        CliOption = new System.CommandLine.Option<bool>("--expand-references", "If references should be expanded.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets a value indicating whether the minify.</summary>
    [ConfigOption(
        ArgName = "--minify",
        Description = "If the output should be minified.")]
    public bool Minify { get; set; } = false;

    private static ConfigurationOption MinifyParameter { get; } = new()
    {
        Name = "Minify",
        DefaultValue = false,
        CliOption = new System.CommandLine.Option<bool>("--minify", "If the output should be minified.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets the identifier naming convention.</summary>
    [ConfigOption(
        ArgName = "--id-convention",
        Description = "The naming convention to use for ids.")]
    public OaNamingConventionCodes IdNamingConvention { get; set; } = OaNamingConventionCodes.Pascal;

    private static ConfigurationOption IdNamingConventionParameter { get; } = new()
    {
        Name = "IdNamingConvention",
        DefaultValue = OaNamingConventionCodes.Pascal,
        CliOption = new System.CommandLine.Option<OaNamingConventionCodes>("--id-convention", "The naming convention to use for ids.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets a value indicating whether the remove uncommon fields.</summary>
    [ConfigOption(
        ArgName = "--remove-uncommon",
        Description = "If the generator should remove some uncommon fields.")]
    public bool RemoveUncommonFields { get; set; } = false;

    private static ConfigurationOption RemoveUncommonFieldsParameter { get; } = new()
    {
        Name = "RemoveUncommonFields",
        DefaultValue = false,
        CliOption = new System.CommandLine.Option<bool>("--remove-uncommon", "If the generator should remove some uncommon fields.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets a value indicating whether the single responses.</summary>
    [ConfigOption(
        ArgName = "--single-responses",
        Description = "If operations should only include a single response.")]
    public bool SingleResponses { get; set; } = false;

    private static ConfigurationOption SingleResponsesParameter { get; } = new()
    {
        Name = "SingleResponses",
        DefaultValue = false,
        CliOption = new System.CommandLine.Option<bool>("--single-responses", "If operations should only include a single response.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>
    /// Gets or sets a value indicating whether the summaries should be included.
    /// </summary>
    [ConfigOption(
        ArgName = "--summaries",
        Description = "If responses should include summaries.")]
    public bool IncludeSummaries { get; set; } = true;

    private static ConfigurationOption IncludeSummariesParameter { get; } = new()
    {
        Name = "IncludeSummaries",
        DefaultValue = true,
        CliOption = new System.CommandLine.Option<bool>("--summaries", "If responses should include summaries.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>
    /// Gets or sets a value indicating whether the HTTP headers should be included.
    /// </summary>
    [ConfigOption(
        ArgName = "--include-headers",
        Description = "If HTTP headers should be included.")]
    public bool IncludeHttpHeaders { get; set; } = false;
    private static ConfigurationOption IncludeHttpHeadersParameter { get; } = new()
    {
        Name = "IncludeHttpHeaders",
        DefaultValue = false,
        CliOption = new System.CommandLine.Option<bool>("--include-headers", "If HTTP headers should be included.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets a value indicating whether the multi file.</summary>
    [ConfigOption(
        ArgName = "--multi-file",
        Description = "If the output should be split into multiple files.")]
    public bool MultiFile { get; set; } = false;

    private static ConfigurationOption MultiFileParameter { get; } = new()
    {
        Name = "MultiFile",
        DefaultValue = false,
        CliOption = new System.CommandLine.Option<bool>("--multi-file", "If the output should be split into multiple files.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    private string _httpCommonParams = string.Join(',', OpenApiCommon._httpCommonParameters);
    private HashSet<string> _httpCommonHash = OpenApiCommon._httpCommonParameters.Keys.ToHashSet();
    public HashSet<string> HttpCommonParamsHash => _httpCommonHash;

    /// <summary>Gets or sets options for controlling the HTTP common.</summary>
    [ConfigOption(
        ArgName = "--http-common-params",
        Description = "Comma-separated list of common parameters to include in HTTP requests.")]
    public string HttpCommonParams
    {
        get => _httpCommonParams;
        set
        {
            _httpCommonParams = value;
            _httpCommonHash = new (_httpCommonParams.Split(',').Select(p => p.Trim()));
        }
    }
    private static ConfigurationOption HttpCommonParamsParameter { get; } = new()
    {
        Name = "HttpCommonParams",
        DefaultValue = string.Join(',', OpenApiCommon._httpCommonParameters),
        CliOption = new System.CommandLine.Option<string>("--http-common-params", "Comma-separated list of common parameters to include in HTTP requests.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };


    private string _httpReadParams = string.Join(',', OpenApiCommon._httpReadParameters);
    private HashSet<string> _httpReadHash = OpenApiCommon._httpReadParameters.Keys.ToHashSet();
    public HashSet<string> HttpReadHash => _httpReadHash;

    /// <summary>Gets or sets options for controlling the HTTP read.</summary>
    [ConfigOption(
        ArgName = "--http-read-params",
        Description = "Comma-separated list of common parameters to include in HTTP read requests.")]
    public string HttpReadParams
    {
        get => _httpReadParams;
        set
        {
            _httpReadParams = value;
            _httpReadHash = new (_httpReadParams.Split(',').Select(p => p.Trim()));
        }
    }

    private static ConfigurationOption HttpReadParamsParameter { get; } = new()
    {
        Name = "HttpReadParams",
        DefaultValue = string.Join(',', OpenApiCommon._httpReadParameters),
        CliOption = new System.CommandLine.Option<string>("--http-read-params", "Comma-separated list of common parameters to include in HTTP read requests.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    private string _searchResultParams = string.Join(',', OpenApiCommon._searchResultParameters);
    private HashSet<string> _searchResultHash = OpenApiCommon._searchResultParameters.Keys.ToHashSet();
    public HashSet<string> SearchResultHash => _searchResultHash;
    /// <summary>Gets or sets options for controlling the search result.</summary>
    [ConfigOption(
        ArgName = "--search-result-params",
        Description = "Comma-separated list of common parameters to include in search results.")]
    public string SearchResultParams
    {
        get => _searchResultParams;
        set
        {
            _searchResultParams = value;
            _searchResultHash = new (_searchResultParams.Split(',').Select(p => p.Trim()));
        }
    }
    private static ConfigurationOption SearchResultParamsParameter { get; } = new()
    {
        Name = "SearchResultParams",
        DefaultValue = string.Join(',', OpenApiCommon._searchResultParameters),
        CliOption = new System.CommandLine.Option<string>("--search-result-params", "Comma-separated list of common parameters to include in search results.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    private string _searchCommonParams = string.Join(',', _searchCommonParameters.Keys);
    private HashSet<string> _searchCommonHash = OpenApiCommon._searchCommonParameters.Keys.ToHashSet();
    public HashSet<string> SearchCommonHash => _searchCommonHash;
    /// <summary>Gets or sets options for controlling the search common.</summary>
    [ConfigOption(
        ArgName = "--search-common-params",
        Description = "Comma-separated list of common parameters to include in search.")]
    public string SearchCommonParams
    {
        get => _searchCommonParams;
        set
        {
            _searchCommonParams = value;
            _searchCommonHash = new (_searchCommonParams.Split(',').Select(p => p.Trim()));
        }
    }
    private static ConfigurationOption SearchCommonParamsParameter { get; } = new()
    {
        Name = "SearchCommonParams",
        DefaultValue = string.Join(',', _searchCommonParameters.Keys),
        CliOption = new System.CommandLine.Option<string>("--search-common-params", "Comma-separated list of common parameters to include in search.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    private string _historyParams = string.Join(',', OpenApiCommon._historyParameters);
    private HashSet<string> _historyHash = OpenApiCommon._historyParameters.Keys.ToHashSet();
    public HashSet<string> HistoryHash => _historyHash;
    /// <summary>Gets or sets options for controlling the history.</summary>
    [ConfigOption(
        ArgName = "--history-params",
        Description = "Comma-separated list of common parameters to include in history.")]
    public string HistoryParams
    {
        get => _historyParams;
        set
        {
            _historyParams = value;
            _historyHash = new (_historyParams.Split(',').Select(p => p.Trim()));
        }
    }
    private static ConfigurationOption HistoryParamsParameter { get; } = new()
    {
        Name = "HistoryParams",
        DefaultValue = string.Join(',', OpenApiCommon._historyParameters),
        CliOption = new System.CommandLine.Option<string>("--history-params", "Comma-separated list of common parameters to include in history.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    private static readonly ConfigurationOption[] _options =
    [
        OpenApiVersionParameter,
        FileFormatParameter,
        TitleParameter,
        DefinitionVersionParameter,
        ExtensionSupportParameter,
        SchemaLevelParameter,
        SchemaStyleParameter,
        MaxRecursionsParameter,
        FhirMimeTypesParameter,
        PatchMimeTypesParameter,
        SearchSupportParameter,
        ExportSearchParamsParameter,
        PostSearchParamLocationParameter,
        ConsolidateSearchParametersParameter,
        OperationSupportParameter,
        UpdateCreateParameter,
        ConditionalReadParameter,
        InteractionReadParameter,
        InteractionVReadParameter,
        InteractionUpdateParameter,
        InteractionUpdateConditionalParameter,
        InteractionPatchParameter,
        InteractionPatchConditionalParameter,
        InteractionDeleteParameter,
        InteractionDeleteConditionalSingleParameter,
        InteractionDeleteConditionalMultipleParameter,
        InteractionDeleteHistoryParameter,
        InteractionDeleteHistoryVersionParameter,
        InteractionHistoryInstanceParameter,
        InteractionHistoryTypeParameter,
        InteractionHistorySystemParameter,
        InteractionCreateParameter,
        InteractionCreateConditionalParameter,
        InteractionSearchTypeParameter,
        InteractionSearchSystemParameter,
        InteractionSearchCompartmentParameter,
        InteractionOperationSystemParameter,
        InteractionOperationTypeParameter,
        InteractionOperationInstanceParameter,
        InteractionCapabilitiesParameter,
        InteractionBatchParameter,
        InteractionTransactionParameter,
        SupportBundleParameter,
        ExportReadOnlyParameter,
        ExportWriteOnlyParameter,
        IncludeDescriptionsParameter,
        DescriptionMaxLengthParameter,
        PerformDescriptionValidationParameter,
        ExpandProfilesParameter,
        ExpandReferencesParameter,
        MinifyParameter,
        IdNamingConventionParameter,
        RemoveUncommonFieldsParameter,
        SingleResponsesParameter,
        IncludeSummariesParameter,
        IncludeHttpHeadersParameter,
        MultiFileParameter,
        HttpCommonParamsParameter,
        HttpReadParamsParameter,
        SearchResultParamsParameter,
        SearchCommonParamsParameter,
        HistoryParamsParameter,
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
                case "OasVersion":
                    OpenApiVersion = GetOpt(parseResult, opt.CliOption, OpenApiVersion);
                    break;
                case "FileFormat":
                    FileFormat = GetOpt(parseResult, opt.CliOption, FileFormat);
                    break;
                case "Title":
                    Title = GetOpt(parseResult, opt.CliOption, Title);
                    break;
                case "DefinitionVersion":
                    DefinitionVersion = GetOpt(parseResult, opt.CliOption, DefinitionVersion);
                    break;
                case "ExtensionSupport":
                    ExtensionSupport = GetOpt(parseResult, opt.CliOption, ExtensionSupport);
                    break;
                case "SchemaLevel":
                    SchemaLevel = GetOpt(parseResult, opt.CliOption, SchemaLevel);
                    break;
                case "SchemaStyle":
                    SchemaStyle = GetOpt(parseResult, opt.CliOption, SchemaStyle);
                    break;
                case "MaxRecursions":
                    MaxRecursions = GetOpt(parseResult, opt.CliOption, MaxRecursions);
                    break;
                case "FhirMimeTypes":
                    FhirMimeTypes = GetOpt(parseResult, opt.CliOption, FhirMimeTypes);
                    break;
                case "PatchMimeTypes":
                    PatchMimeTypes = GetOpt(parseResult, opt.CliOption, PatchMimeTypes);
                    break;
                case "SearchSupport":
                    SearchSupport = GetOpt(parseResult, opt.CliOption, SearchSupport);
                    break;
                case "ExportSearchParams":
                    ExportSearchParams = GetOpt(parseResult, opt.CliOption, ExportSearchParams);
                    break;
                case "PostSearchParamLocation":
                    PostSearchParamLocation = GetOpt(parseResult, opt.CliOption, PostSearchParamLocation);
                    break;
                case "ConsolidateSearchParameters":
                    ConsolidateSearchParameters = GetOpt(parseResult, opt.CliOption, ConsolidateSearchParameters);
                    break;
                case "OperationSupport":
                    OperationSupport = GetOpt(parseResult, opt.CliOption, OperationSupport);
                    break;
                case "UpdateCreate":
                    UpdateCreate = GetOpt(parseResult, opt.CliOption, UpdateCreate);
                    break;
                case "ConditionalRead":
                    ConditionalRead = GetOpt(parseResult, opt.CliOption, ConditionalRead);
                    break;
                case "InteractionRead":
                    InteractionRead = GetOpt(parseResult, opt.CliOption, InteractionRead);
                    break;
                case "InteractionVRead":
                    InteractionVRead = GetOpt(parseResult, opt.CliOption, InteractionVRead);
                    break;
                case "InteractionUpdate":
                    InteractionUpdate = GetOpt(parseResult, opt.CliOption, InteractionUpdate);
                    break;
                case "InteractionUpdateConditional":
                    InteractionUpdateConditional = GetOpt(parseResult, opt.CliOption, InteractionUpdateConditional);
                    break;
                case "InteractionPatch":
                    InteractionPatch = GetOpt(parseResult, opt.CliOption, InteractionPatch);
                    break;
                case "InteractionPatchConditional":
                    InteractionPatchConditional = GetOpt(parseResult, opt.CliOption, InteractionPatchConditional);
                    break;
                case "InteractionDelete":
                    InteractionDelete = GetOpt(parseResult, opt.CliOption, InteractionDelete);
                    break;
                case "InteractionDeleteConditionalSingle":
                    InteractionDeleteConditionalSingle = GetOpt(parseResult, opt.CliOption, InteractionDeleteConditionalSingle);
                    break;
                case "InteractionDeleteConditionalMultiple":
                    InteractionDeleteConditionalMultiple = GetOpt(parseResult, opt.CliOption, InteractionDeleteConditionalMultiple);
                    break;
                case "InteractionDeleteHistory":
                    InteractionDeleteHistory = GetOpt(parseResult, opt.CliOption, InteractionDeleteHistory);
                    break;
                case "InteractionDeleteHistoryVersion":
                    InteractionDeleteHistoryVersion = GetOpt(parseResult, opt.CliOption, InteractionDeleteHistoryVersion);
                    break;
                case "InteractionHistoryInstance":
                    InteractionHistoryInstance = GetOpt(parseResult, opt.CliOption, InteractionHistoryInstance);
                    break;
                case "InteractionHistoryType":
                    InteractionHistoryType = GetOpt(parseResult, opt.CliOption, InteractionHistoryType);
                    break;
                case "InteractionHistorySystem":
                    InteractionHistorySystem = GetOpt(parseResult, opt.CliOption, InteractionHistorySystem);
                    break;
                case "InteractionCreate":
                    InteractionCreate = GetOpt(parseResult, opt.CliOption, InteractionCreate);
                    break;
                case "InteractionCreateConditional":
                    InteractionCreateConditional = GetOpt(parseResult, opt.CliOption, InteractionCreateConditional);
                    break;
                case "InteractionSearchType":
                    InteractionSearchType = GetOpt(parseResult, opt.CliOption, InteractionSearchType);
                    break;
                case "InteractionSearchSystem":
                    InteractionSearchSystem = GetOpt(parseResult, opt.CliOption, InteractionSearchSystem);
                    break;
                case "InteractionSearchCompartment":
                    InteractionSearchCompartment = GetOpt(parseResult, opt.CliOption, InteractionSearchCompartment);
                    break;
                case "InteractionOperationSystem":
                    InteractionOperationSystem = GetOpt(parseResult, opt.CliOption, InteractionOperationSystem);
                    break;
                case "InteractionOperationType":
                    InteractionOperationType = GetOpt(parseResult, opt.CliOption, InteractionOperationType);
                    break;
                case "InteractionOperationInstance":
                    InteractionOperationInstance = GetOpt(parseResult, opt.CliOption, InteractionOperationInstance);
                    break;
                case "SupportMetadata":
                    InteractionCapabilities = GetOpt(parseResult, opt.CliOption, InteractionCapabilities);
                    break;
                case "SupportBundle":
                    SupportBundle = GetOpt(parseResult, opt.CliOption, SupportBundle);
                    break;
                case "ExportReadOnly":
                    ExportReadOnly = GetOpt(parseResult, opt.CliOption, ExportReadOnly);
                    break;
                case "ExportWriteOnly":
                    ExportWriteOnly = GetOpt(parseResult, opt.CliOption, ExportWriteOnly);
                    break;
                case "PropertyDescriptions":
                    IncludeDescriptions = GetOpt(parseResult, opt.CliOption, IncludeDescriptions);
                    break;
                case "DescriptionMaxLength":
                    DescriptionMaxLength = GetOpt(parseResult, opt.CliOption, DescriptionMaxLength);
                    break;
                case "PerformDescriptionValidation":
                    PerformDescriptionValidation = GetOpt(parseResult, opt.CliOption, PerformDescriptionValidation);
                    break;
                case "ExpandProfiles":
                    ExpandProfiles = GetOpt(parseResult, opt.CliOption, ExpandProfiles);
                    break;
                case "ExpandReferences":
                    ExpandReferences = GetOpt(parseResult, opt.CliOption, ExpandReferences);
                    break;
                case "Minify":
                    Minify = GetOpt(parseResult, opt.CliOption, Minify);
                    break;
                case "IdNamingConvention":
                    IdNamingConvention = GetOpt(parseResult, opt.CliOption, IdNamingConvention);
                    break;
                case "RemoveUncommonFields":
                    RemoveUncommonFields = GetOpt(parseResult, opt.CliOption, RemoveUncommonFields);
                    break;
                case "SingleResponses":
                    SingleResponses = GetOpt(parseResult, opt.CliOption, SingleResponses);
                    break;
                case "IncludeSummaries":
                    IncludeSummaries = GetOpt(parseResult, opt.CliOption, IncludeSummaries);
                    break;
                case "IncludeHttpHeaders":
                    IncludeHttpHeaders = GetOpt(parseResult, opt.CliOption, IncludeHttpHeaders);
                    break;
                case "MultiFile":
                    MultiFile = GetOpt(parseResult, opt.CliOption, MultiFile);
                    break;
                case "HttpCommonParams":
                    HttpCommonParams = GetOpt(parseResult, opt.CliOption, HttpCommonParams);
                    break;
                case "HttpReadParams":
                    HttpReadParams = GetOpt(parseResult, opt.CliOption, HttpReadParams);
                    break;
                case "SearchResultParams":
                    SearchResultParams = GetOpt(parseResult, opt.CliOption, SearchResultParams);
                    break;
                case "SearchCommonParams":
                    SearchCommonParams = GetOpt(parseResult, opt.CliOption, SearchCommonParams);
                    break;
                case "HistoryParams":
                    HistoryParams = GetOpt(parseResult, opt.CliOption, HistoryParams);
                    break;
            }
        }
    }

    /// <summary>Gets or sets the write stream to use.</summary>
    public Stream? WriteStream { get; set; } = null;
}
