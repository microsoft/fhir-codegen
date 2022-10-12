// <copyright file="OpenApiOptions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.SpecManager.Manager;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using static Microsoft.Health.Fhir.CodeGenCommon.Models.FhirCapResource;

namespace Microsoft.Health.Fhir.SpecManager.Language.OpenApi;

public class OpenApiOptions
{
    /// <summary>Gets options for controlling the language.</summary>
    public static Dictionary<string, string> LanguageOptions => new()
    {
        { "OpenApiVersion", "Open API version to use (2|3)." },
        { "FileFormat", "File format to export (json|yaml)." },
        { "Title", "Title to use in the Info section, defaults to 'FHIR {FhirSequence}:{VersionString}'." },
        { "Version", "Version number to use in the OpenAPI file, defaults to '{FHIR Version}'." },

        { "SchemaLevel", "How much schema to include (none|names|detailed)." },
        { "SchemaStyle", "How schemas should be built (references|inline)." },
        { "MaxRecursions", "Maximum depth to expand recursions (0)." },

        { "FhirMime", "Which FHIR MIME types to support (capabilities|json|xml|both)." },
        { "PatchMime", "Which FHIR Patch types to support (capabilities|json|xml|fhirMime|all)." },

        { "SearchSupport", "Supported search methods (both|get|post|none)." },
        { "IncludeSearchParams", "If search parameters should be included in the definitions (true|false)." },
        { "SearchPostParams", "Where search params should appear in post-based search (body|query|both|none)." },
        { "ConsolidateSearchParams", "If search parameters should be consolidated (true|false)." },

        { "OperationSupport", "Supported Operation calling styles (post|get|both|none)." },

        { "UpdateCreate", "If update can commit to a new identity (capabilities|false|true)." },
        { "ConditionalCreate", "If the server allows/uses conditional create (capabilities|false|true)." },
        { "ConditionalRead", "Conditional read policy (capabilities|not-supported|modified-since|not-match|full-support)." },
        { "ConditionalUpdate", "If the server allows/uses conditional update (capabilities|false|true)." },
        { "ConditionalPatch", "If the server allows/uses conditional patch (capabilities|false|true)." },
        { "ConditionalDelete", "How conditional delete is supported (capabilities|not-supported|single|multiple)." },

        { "History", "If _history GET operations should be included (false|true)." },

        { "Metadata", "If the JSON should include a link to /metadata (true|false)." },
        { "BundleOperations", "If the generator should include /Bundle, etc. (true|false)." },

        { "ReadOnly", "If the output should only contain GET operations (false|true)." },
        { "WriteOnly", "If the output should only contain POST/PUT/DELETE operations (false|true)." },

        { "Descriptions", "If properties should include descriptions (true|false)." },
        { "DescriptionMaxLen", "Maximum length of descriptions, if being validated (60)." },
        { "DescriptionValidation", "If descriptions are required and should be validated (false|true)." },

        { "ExpandProfiles", "If types should expand based on allowed profiles (true|false)." },
        { "ExpandReferences", "If types should expand through references (true|false)." },

        { "Minify", "If the output JSON should be minified (false|true)." },
        { "IdConvention", "Naming convention to use for OpenAPI ids (pascal|camel|upper|lower)." },

        { "HttpCommonParams", "Comma-separated list of query params common to all ops, '-' for none (_format,_pretty)." },
        { "HttpReadParams", "Comma-separated list of query params common to reads, '-' for none (_elements,_summary)." },
        { "SearchResultParams", "Comma-separated list of search result params, '-' for none (_contained,_count,_graph,_include,_revinclude,_score,_sort,_total)." },
        { "SearchCommonParams", "Comma-separated list of common search params, '-' for none (_content,_filger,_id,_in,_language,_lastUpdated,_list,_profile,_query,_security,_source,_tag,_text)." },
        { "HistoryParams", "Comma-separated list of allowed history params, '-' for none (_count,_since,_at,_list,_sort)." },

        { "RemoveUncommonFields", "If the generator should remove some uncommon fields (false|true)." },
        { "SingleResponses", "If operations should only include a single response (false|true)." },
        { "Summaries", "If responses should include summaries (true|false)." },

    };

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenApiOptions"/> class.
    /// </summary>
    internal OpenApiOptions()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="OpenApiOptions"/> class.</summary>
    /// <param name="options">Options for controlling the operation.</param>
    internal OpenApiOptions(ExporterOptions options)
    {
        string val;
        int i;

        i = options.GetParam("OpenApiVersion", 2);
        switch (i)
        {
            case 3:
                OpenApiVersion = OpenApiSpecVersion.OpenApi3_0;
                break;

            case 2:
            default:
                OpenApiVersion = OpenApiSpecVersion.OpenApi2_0;
                break;
        }

        val = options.GetParam("FileFormat", "json");
        switch (val.FirstOrDefault('j'))
        {
            case 'j':
            case 'J':
            default:
                FileFormat = OpenApiCommon.OaFileFormat.Json;
                break;

            case 'y':
            case 'Y':
                FileFormat = OpenApiCommon.OaFileFormat.Yaml;
                break;
        }

        Title = options.GetParam("Title", string.Empty);
        Version = options.GetParam("Version", string.Empty);

        val = options.GetParam("SchemaLevel", string.Empty);
        switch (val.ToUpperInvariant())
        {
            case "NONE":
            default:
                SchemaLevel = OpenApiCommon.OaSchemaLevelCodes.None;
                break;

            case "NAME":
            case "NAMES":
            case "NAMED":
                SchemaLevel = OpenApiCommon.OaSchemaLevelCodes.Names;
                break;

            case "DETAILED":
                SchemaLevel = OpenApiCommon.OaSchemaLevelCodes.Detailed;
                break;
        }

        val = options.GetParam("SchemaStyle", "references");
        switch (val.FirstOrDefault('r'))
        {
            case 'r':
            case 'R':
            default:
                SchemaStyle = OpenApiCommon.OaSchemaStyleCodes.References;
                break;

            case 'i':
            case 'I':
                SchemaStyle = OpenApiCommon.OaSchemaStyleCodes.Inline;
                break;
        }

        MaxRecursions = options.GetParam("MaxRecursions", 0);

        val = options.GetParam("FhirMime", "capabilities");
        switch (val.FirstOrDefault('j'))
        {
            case 'c':
            case 'C':
            default:
                FhirMime = OpenApiCommon.OaFhirMimeCodes.FromCapabilities;
                break;

            case 'j':
            case 'J':
                FhirMime = OpenApiCommon.OaFhirMimeCodes.FhirJson;
                break;

            case 'x':
            case 'X':
                FhirMime = OpenApiCommon.OaFhirMimeCodes.FhirXml;
                break;

            case 'b':
            case 'B':
                FhirMime = OpenApiCommon.OaFhirMimeCodes.Both;
                break;
        }

        val = options.GetParam("PatchMime", "capabilities");
        switch (val.FirstOrDefault('c'))
        {
            case 'c':
            case 'C':
            default:
                PatchMime = OpenApiCommon.OaPatchMimeCodes.FromCapabilities;
                break;

            case 'j':
            case 'J':
                PatchMime = OpenApiCommon.OaPatchMimeCodes.Json;
                break;

            case 'x':
            case 'X':
                PatchMime = OpenApiCommon.OaPatchMimeCodes.Xml;
                break;

            case 'f':
            case 'F':
                PatchMime = OpenApiCommon.OaPatchMimeCodes.FhirMime;
                break;

            case 'a':
            case 'A':
                PatchMime = OpenApiCommon.OaPatchMimeCodes.All;
                break;
        }

        val = options.GetParam("SearchSupport", "both");
        switch (val.ToUpperInvariant())
        {
            case "NONE":
                SearchSupport = OpenApiCommon.OaHttpSupportCodes.None;
                break;

            case "GET":
                SearchSupport = OpenApiCommon.OaHttpSupportCodes.Get;
                break;

            case "POST":
                SearchSupport = OpenApiCommon.OaHttpSupportCodes.Post;
                break;

            default:
            case "BOTH":
                SearchSupport = OpenApiCommon.OaHttpSupportCodes.Both;
                break;
        }

        IncludeSearchParams = options.GetParam("IncludeSearchParams", true);

        val = options.GetParam("SearchPostParams", "body");
        switch (val.ToUpperInvariant())
        {
            case "NONE":
                SearchParamLoc = OpenApiCommon.OaSearchPostParameterLocationCodes.None;
                break;

            case "BOTH":
                SearchParamLoc = OpenApiCommon.OaSearchPostParameterLocationCodes.Both;
                break;

            case "QUERY":
                SearchParamLoc = OpenApiCommon.OaSearchPostParameterLocationCodes.Query;
                break;

            default:
            case "BODY":
                SearchParamLoc = OpenApiCommon.OaSearchPostParameterLocationCodes.Body;
                break;
        }

        ConsolidateSearchParams = options.GetParam("ConsolidateSearchParams", true);

        val = options.GetParam("OperationSupport", "post");
        switch (val.ToUpperInvariant())
        {
            case "NONE":
                OperationSupport = OpenApiCommon.OaHttpSupportCodes.None;
                break;

            case "GET":
                OperationSupport = OpenApiCommon.OaHttpSupportCodes.Get;
                break;

            case "POST":
                OperationSupport = OpenApiCommon.OaHttpSupportCodes.Post;
                break;

            case "BOTH":
            default:
                OperationSupport = OpenApiCommon.OaHttpSupportCodes.Both;
                break;
        }

        val = options.GetParam("HttpCommonParams", string.Empty);
        if (!string.IsNullOrEmpty(val))
        {
            if (!val.Equals('-'))
            {
                HttpCommonParams = val.Split(',').ToList();
            }
        }
        else
        {
            HttpCommonParams = OpenApiCommon._httpCommonParameters.Keys.ToList();
        }

        val = options.GetParam("HttpReadParams", string.Empty);
        if (!string.IsNullOrEmpty(val))
        {
            if (!val.Equals('-'))
            {
                HttpReadParams = val.Split(',').ToList();
            }
        }
        else
        {
            HttpReadParams = OpenApiCommon._httpReadParameters.Keys.ToList();
        }

        val = options.GetParam("SearchResultParams", string.Empty);
        if (!string.IsNullOrEmpty(val))
        {
            if (!val.Equals('-'))
            {
                SearchResultParams = val.Split(',').ToList();
            }
        }
        else
        {
            SearchResultParams = OpenApiCommon._searchResultParameters.Keys.ToList();
        }

        val = options.GetParam("SearchCommonParams", string.Empty);
        if (!string.IsNullOrEmpty(val))
        {
            if (!val.Equals('-'))
            {
                SearchCommonParams = val.Split(',').ToList();
            }
        }
        else
        {
            SearchCommonParams = OpenApiCommon._searchCommonParameters.Keys.ToList();
        }

        val = options.GetParam("HistoryParams", string.Empty);
        if (!string.IsNullOrEmpty(val))
        {
            if (!val.Equals('-'))
            {
                HistoryParams = val.Split(',').ToList();
            }
        }
        else
        {
            HistoryParams = OpenApiCommon._historyParameters.Keys.ToList();
        }

        UpdateCreate = options.GetParamBoolN("UpdateCreate");
        ConditionalCreate = options.GetParamBoolN("ConditionalCreate");
        ConditionalUpdate = options.GetParamBoolN("ConditionalUpdate");
        ConditionalPatch = options.GetParamBoolN("ConditionalPatch");

        val = options.GetParam("ConditionalRead", string.Empty).ToLowerInvariant();
        switch (val)
        {
            case "not-supported":
                ConditionalRead = ConditionalReadPolicy.NotSupported;
                break;

            case "modified-since":
                ConditionalRead = ConditionalReadPolicy.ModifiedSince;
                break;

            case "not-match":
                ConditionalRead = ConditionalReadPolicy.NotMatch;
                break;

            case "full-support":
                ConditionalRead = ConditionalReadPolicy.FullSupport;
                break;

            default:
                ConditionalRead = null;
                break;
        }

        val = options.GetParam("ConditionalDelete", string.Empty).ToLowerInvariant();
        switch (val)
        {
            case "not-supported":
                ConditionalDelete = ConditionalDeletePolicy.NotSupported;
                break;

            case "single":
                ConditionalDelete = ConditionalDeletePolicy.Single;
                break;

            case "multiple":
                ConditionalDelete = ConditionalDeletePolicy.Multiple;
                break;

            default:
                ConditionalDelete = null;
                break;
        }


        IncludeBundleOperations = options.GetParam("BundleOperations", true);
        IncludeDescriptions = options.GetParam("Descriptions", true);
        DescriptionMaxLen = options.GetParam("DescriptionMaxLen", 60);
        DescriptionValidation = options.GetParam("DescriptionValidation", false);
        ExpandProfiles = options.GetParam("ExpandProfiles", true);
        ExpandReferences = options.GetParam("ExpandReferences", true);


        IncludeHistory = options.GetParam("History", false);
        IncludeMetadata = options.GetParam("Metadata", false);
        Minify = options.GetParam("Minify", false);
        GenerateReadOnly = options.GetParam("ReadOnly", false);
        RemoveUncommonFields = options.GetParam("RemoveUncommonFields", false);


        SingleResponseCode = options.GetParam("SingleResponses", false);
        IncludeSummaries = options.GetParam("Summaries", true);
        GenerateWriteOnly = options.GetParam("WriteOnly", false);

        string opConvention = options.GetParam("IdConvention", "pascal");
        switch (opConvention.ToUpperInvariant())
        {
            case "CAMEL":
                IdConvention = FhirTypeBase.NamingConvention.CamelCase;
                break;

            case "UPPER":
                IdConvention = FhirTypeBase.NamingConvention.UpperCase;
                break;

            case "LOWER":
                IdConvention = FhirTypeBase.NamingConvention.LowerCase;
                break;

            case "PASCAL":
            default:
                IdConvention = FhirTypeBase.NamingConvention.PascalCase;
                break;
        }

    }

    /// <summary>Gets or sets the file format.</summary>
    internal OpenApiCommon.OaFileFormat FileFormat { get; } = OpenApiCommon.OaFileFormat.Json;

    /// <summary>Gets or sets the title.</summary>
    internal string Title { get; set; } = "";

    /// <summary>Gets or sets the version.</summary>
    internal string Version { get; set; } = "";

    /// <summary>Gets or sets the schema level.</summary>
    internal OpenApiCommon.OaSchemaLevelCodes SchemaLevel { get; set; } = OpenApiCommon.OaSchemaLevelCodes.None;

    /// <summary>Gets or sets the schema style.</summary>
    internal OpenApiCommon.OaSchemaStyleCodes SchemaStyle { get; set; } = OpenApiCommon.OaSchemaStyleCodes.References;

    /// <summary>Maximum number of times to recurse.</summary>
    internal int MaxRecursions { get; set; } = 0;

    /// <summary>Gets or sets the FHIR MIME type support.</summary>
    internal OpenApiCommon.OaFhirMimeCodes FhirMime { get; set; } = OpenApiCommon.OaFhirMimeCodes.FromCapabilities;

    /// <summary>Gets or sets the patch mime.</summary>
    internal OpenApiCommon.OaPatchMimeCodes PatchMime { get; set; } = OpenApiCommon.OaPatchMimeCodes.FromCapabilities;

    /// <summary>The search support.</summary>
    internal OpenApiCommon.OaHttpSupportCodes SearchSupport { get; set; } = OpenApiCommon.OaHttpSupportCodes.Both;

    /// <summary>
    /// Gets or sets a value indicating whether the search parameters should be included.
    /// </summary>
    internal bool IncludeSearchParams { get; set; } = true;

    /// <summary>The search parameter location.</summary>
    internal OpenApiCommon.OaSearchPostParameterLocationCodes SearchParamLoc { get; set; } = OpenApiCommon.OaSearchPostParameterLocationCodes.Body;

    /// <summary>Gets or sets the search parameter consolidation.</summary>
    internal bool ConsolidateSearchParams { get; set; } = true;

    /// <summary>Gets or sets the operation support.</summary>
    internal OpenApiCommon.OaHttpSupportCodes OperationSupport { get; set; } = OpenApiCommon.OaHttpSupportCodes.Post;

    /// <summary>Gets or sets the update can create behavior - null indicates code should use what is in a capability statement.</summary>
    internal bool? UpdateCreate { get; set; } = null;

    /// <summary>Gets or sets the conditional create behavior - null indicates code should use what is in a capability statement.</summary>
    internal bool? ConditionalCreate { get; set; } = null;

    /// <summary>Gets or sets the conditional read policy - null indicates code should use what is present in a capability statement.</summary>
    internal ConditionalReadPolicy? ConditionalRead { get; set; } = null;

    /// <summary>Gets or sets the conditional update - null indicates code should use what is present in a capability statement.</summary>
    internal bool? ConditionalUpdate { get; set; } = null;

    /// <summary>Gets or sets the conditional patch - null indicates code should use what is present in a capability statement.</summary>
    internal bool? ConditionalPatch { get; set; } = null;

    /// <summary>Gets or sets the conditional delete - null indicates code should use what is present in a capability statement.</summary>
    internal ConditionalDeletePolicy? ConditionalDelete { get; set; } = null;

    /// <summary>True to single response code.</summary>
    internal bool SingleResponseCode { get; set; } = false;

    /// <summary>True to include, false to exclude the summaries.</summary>
    internal bool IncludeSummaries { get; set; } = true;

    /// <summary>True to include, false to exclude the schema descriptions.</summary>
    internal bool IncludeDescriptions { get; set; } = true;

    /// <summary>True to validate descriptions.</summary>
    internal bool DescriptionValidation { get; set; } = false;

    /// <summary>Length of the description maximum.</summary>
    internal int DescriptionMaxLen { get; set; } = 60;

    /// <summary>True to expand references based on allowed profiles.</summary>
    internal bool ExpandProfiles { get; set; } = true;

    /// <summary>True to expand references.</summary>
    internal bool ExpandReferences { get; set; } = true;

    /// <summary>True to generate read only.</summary>
    internal bool GenerateReadOnly { get; set; } = false;

    /// <summary>True to generate write only.</summary>
    internal bool GenerateWriteOnly { get; set; } = false;

    /// <summary>True to include, false to exclude the bundle operations.</summary>
    internal bool IncludeBundleOperations { get; set; } = true;

    /// <summary>True to include, false to exclude the metadata.</summary>
    internal bool IncludeMetadata { get; set; } = false;

    /// <summary>True to include, false to exclude the history.</summary>
    internal bool IncludeHistory { get; set; } = false;

    /// <summary>True to remove uncommon fields.</summary>
    internal bool RemoveUncommonFields { get; set; } = false;

    /// <summary>The open API version.</summary>
    internal OpenApiSpecVersion OpenApiVersion { get; set; } = OpenApiSpecVersion.OpenApi2_0;

    /// <summary>True to minify the export file, false for 'readable' formatting.</summary>
    internal bool Minify { get; set; } = false;

    /// <summary>List of common HTTP query params for all operations, e.g.: _format,_pretty.</summary>
    internal List<string> HttpCommonParams { get; } = new();

    /// <summary>List of common HTTP query params for 'read' operations, e.g.: _elements,_summary.</summary>
    internal List<string> HttpReadParams { get; } = new();

    /// <summary>Comma-separated list of search result params, e.g.: _contained,_count,_graph,_include,_revinclude,_score,_sort,_total.</summary>
    internal List<string> SearchResultParams { get; } = new();

    /// <summary>Comma-separated list of common search params, e.g.: _content,_filger,_id,_in,_language,_lastUpdated,_list,_profile,_query,_security,_source,_tag,_text.</summary>
    internal List<string> SearchCommonParams { get; } = new();

    /// <summary>Comma-separated list of allowed history params, e.g.: _count,_since,_at,_list,_sort.</summary>
    internal List<string> HistoryParams { get; } = new();

    /// <summary>Gets or sets the operation id naming convention.</summary>
    internal FhirTypeBase.NamingConvention IdConvention { get; set; } = FhirTypeBase.NamingConvention.PascalCase;
}
