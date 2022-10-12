// <copyright file="OpenApiCommon.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Microsoft.Health.Fhir.SpecManager.Language.OpenApi;

/// <summary>An open API common.</summary>
public static class OpenApiCommon
{
    /// <summary>Values that represent OpenAPI file formats.</summary>
    public enum OaFileFormat
    {
        /// <summary>Export JSON files.</summary>
        Json,

        /// <summary>Export YAML files.</summary>
        Yaml
    }

    /// <summary>How much schema should be included in the output.</summary>
    public enum OaSchemaLevelCodes
    {
        /// <summary>No schema - just use 'objects'.</summary>
        None,

        /// <summary>Resources are named, but are defined as 'objects'.</summary>
        Names,

        /// <summary>Resources inlcude elements.</summary>
        Detailed
    }

    /// <summary>Values that represent how schemas should be built.</summary>
    public enum OaSchemaStyleCodes
    {
        /// <summary>Schemas use references.</summary>
        References,

        /// <summary>Schemas are all inlined.</summary>
        Inline,
    }

    /// <summary>Values that represent FHIR MIME configuration options.</summary>
    public enum OaFhirMimeCodes
    {
        /// <summary>Select FHIR MIME types based on capability statement options.</summary>
        FromCapabilities,

        /// <summary>Support application/fhir+json.</summary>
        FhirJson,

        /// <summary>Support application/fhir+xml.</summary>
        FhirXml,

        /// <summary>Support application/fhir+json and application/fhir+xml.</summary>
        Both,
    }

    /// <summary>Values that represent Patch MIME configuration options.</summary>
    public enum OaPatchMimeCodes
    {
        /// <summary>Select Patch MIME types based on capability statement options.</summary>
        FromCapabilities,

        /// <summary>Allow application/json.</summary>
        Json,

        /// <summary>Allow application/xml.</summary>
        Xml,

        /// <summary>Allow configured FHIR MIME types.</summary>
        FhirMime,

        /// <summary>Allow all valid patch MIME types.</summary>
        All,
    }

    /// <summary>Values that represent HTTP Get vs. Post options.</summary>
    public enum OaHttpSupportCodes
    {
        /// <summary>Enable search via GET and POST.</summary>
        Both,

        /// <summary>Only enable search via GET.</summary>
        Get,

        /// <summary>Only enable search via POST.</summary>
        Post,

        /// <summary>Do not allow search.</summary>
        None,
    }

    /// <summary>Values that represent search post parameter location codes.</summary>
    public enum OaSearchPostParameterLocationCodes
    {
        /// <summary>POST search parameters only appear in the body.</summary>
        Body,

        /// <summary>POST search parameters only appear in the query.</summary>
        Query,

        /// <summary>POST search parameters can be in the query or body.</summary>
        Both,

        /// <summary>Do not enumerate POST search parameters.</summary>
        None,
    }

    /// <summary>Values that represent oa Operation level codes.</summary>
    public enum OaOpLevelCodes
    {
        /// <summary>An enum constant representing the system option.</summary>
        System,

        /// <summary>An enum constant representing the resource option.</summary>
        Type,

        /// <summary>An enum constant representing the instance option.</summary>
        Instance,
    }

    /// <summary>The uncommon fields.</summary>
    public static readonly HashSet<string> _uncommonFields = new HashSet<string>()
    {
        "Annotation.id",
        "Coding.version",
        "Coding.userSelected",
        "Coding.id",
        "CodeableConcept.id",
        "Duration.id",
        "Identifier.id",
        "Period.id",
        "Quantity.id",
        "Range.id",
        "Ratio.id",
        "Reference.id",
        "Reference.identifier",
        "SimpleQuantity.id",
        "Timing.id",
    };

    /// <summary>Dictionary mapping FHIR primitive types to language equivalents.</summary>
    public static readonly Dictionary<string, string> _primitiveTypeMap = new()
    {
        // { "base", "Object" },
        { "base64Binary", "string:byte" },
        { "boolean", "boolean" },           // note, this is here to simplify primitive mapping
        { "canonical", "string" },
        { "code", "string" },
        { "date", "string" },
        { "dateTime", "string" },           // Cannot use "date" because of Partial Dates... may want to consider defining a new type, but not today
        { "decimal", "number:double" },
        { "id", "string" },
        { "instant", "string" },
        { "integer", "integer:int32" },
        { "integer64", "integer:int64" },
        { "markdown", "string" },
        { "oid", "string" },
        { "positiveInt", "integer:int32" },
        { "string", "string" },             // note, this is here to simplify primitive mapping
        { "time", "string" },
        { "unsignedInt", "integer:int32" },
        { "uri", "string" },
        { "url", "string" },
        { "uuid", "string" },
        { "xhtml", "string" },
    };

    /// <summary>(Immutable) The read only interactions.</summary>
    public static readonly HashSet<FhirCapResource.FhirInteractionCodes> _resInteracionHashRO = new()
    {
        FhirCapResource.FhirInteractionCodes.Read,
        FhirCapResource.FhirInteractionCodes.VRead,
        FhirCapResource.FhirInteractionCodes.HistoryInstance,
        FhirCapResource.FhirInteractionCodes.Search,
        FhirCapResource.FhirInteractionCodes.SearchType,
    };

    /// <summary>(Immutable) The read only interactions.</summary>
    public static readonly FhirCapResource.FhirInteractionCodes[] _resInteractionsRO = new[]
    {
        FhirCapResource.FhirInteractionCodes.Read,
        FhirCapResource.FhirInteractionCodes.VRead,
        FhirCapResource.FhirInteractionCodes.HistoryInstance,
        FhirCapResource.FhirInteractionCodes.Search,
        FhirCapResource.FhirInteractionCodes.SearchType,
    };

    /// <summary>(Immutable) The write only interactions.</summary>
    public static readonly HashSet<FhirCapResource.FhirInteractionCodes> _resInteactionHashWO = new()
    {
        FhirCapResource.FhirInteractionCodes.Update,
        FhirCapResource.FhirInteractionCodes.Patch,
        FhirCapResource.FhirInteractionCodes.Create,
    };

    /// <summary>(Immutable) The write only interactions.</summary>
    public static readonly FhirCapResource.FhirInteractionCodes[] _resInteractionsWO = new[]
    {
        FhirCapResource.FhirInteractionCodes.Update,
        FhirCapResource.FhirInteractionCodes.Patch,
        FhirCapResource.FhirInteractionCodes.Create,
    };

    public static readonly FhirCapResource.FhirInteractionCodes[] _resInteractionsRW = new[]
    {
        FhirCapResource.FhirInteractionCodes.Read,
        FhirCapResource.FhirInteractionCodes.VRead,
        FhirCapResource.FhirInteractionCodes.HistoryInstance,
        FhirCapResource.FhirInteractionCodes.Search,
        FhirCapResource.FhirInteractionCodes.SearchType,
        FhirCapResource.FhirInteractionCodes.Update,
        FhirCapResource.FhirInteractionCodes.Patch,
        FhirCapResource.FhirInteractionCodes.Create,
        FhirCapResource.FhirInteractionCodes.Delete,
    };

    /// <summary>(Immutable) The reserved words.</summary>
    public static readonly HashSet<string> _reservedWords = new();

    /// <summary>(Immutable) Options for controlling the HTTP.</summary>
    public static readonly Dictionary<string, OpenApiParameter> _httpReadParameters;

    /// <summary>(Immutable) Options for controlling the HTTP common.</summary>
    public static readonly Dictionary<string, OpenApiParameter> _httpCommonParameters;

    /// <summary>(Immutable) Common search/operation parameters.</summary>
    public static readonly Dictionary<string, OpenApiParameter> _searchCommonParameters;

    /// <summary>(Immutable) Options for controlling the result.</summary>
    public static readonly Dictionary<string, OpenApiParameter> _searchResultParameters;

    /// <summary>(Immutable) Options for controlling the search root.</summary>
    public static readonly Dictionary<string, OpenApiParameter> _searchRootParameters;

    /// <summary>(Immutable) Options for controlling the path.</summary>
    public static readonly Dictionary<string, OpenApiParameter> _pathParameters;

    /// <summary>(Immutable) Options for controlling the history.</summary>
    public static readonly Dictionary<string, OpenApiParameter> _historyParameters;

    /// <summary>(Immutable) The response codes read.</summary>
    public static readonly int[] _responseCodesRead = new int[] { 200, 410, 404 };

    /// <summary>(Immutable) The response codes conditional or patch.</summary>
    public static readonly int[] _responseCodesConditionalOrPatch = new int[] { 200, 400, 401, 404, 412 };

    /// <summary>(Immutable) The response codes update.</summary>
    public static readonly int[] _responseCodesUpdate = new int[] { 200, 400, 401, 404, 405, 409, 412, 422 };

    /// <summary>(Immutable) The response codes create.</summary>
    public static readonly int[] _responseCodesCreate = new int[] { 200, 400, 404, 422 };

    /// <summary>(Immutable) The response codes delete.</summary>
    public static readonly int[] _responseCodesDelete = new int[] { 200, 202, 204, 400, 401, 404, 405, 409 };

    /// <summary>(Immutable) The response codes search.</summary>
    public static readonly int[] _responseCodesSearch = new int[] { 200, 400, 401, 405 };

    /// <summary>The HTTP response descriptions.</summary>
    public static readonly Dictionary<int, string> _httpResponseDescriptions = new()
        {
            { 200, "OK" },
            { 201, "CREATED" },
            { 202, "ACCEPTED" },
            { 204, "NO CONTENT" },
            { 400, "BAD REQUEST" },
            { 401, "NOT AUTHORIZED" },
            { 404, "NOT FOUND" },
            { 405, "METHOD NOT ALLOWED" },
            { 409, "CONFLICT" },
            { 410, "GONE" },
            { 412, "CONFLICT" },
            { 422, "UNPROCESSABLE" },
        };

    /// <summary>
    /// Initializes static members of the <see cref="OpenApiCommon"/> class.
    /// </summary>
    static OpenApiCommon()
    {
        _httpReadParameters = new()
        {
            ["_elements"] = BuildStringParameter("_elements", "Ask for a particular set of elements to be returned"),
            //["_summary"] = BuildSummaryParameter(),
            ["_summary"] = BuildStringParameter("_summary", "Return only portions of resources, based on pre-defined levels"),
        };

        _httpCommonParameters = new()
        {
            ["_format"] = BuildStringParameter("_format", "Override the HTTP content negotiation"),
            ["_pretty"] = BuildStringParameter("_pretty", "Ask for a pretty printed response for human convenience"),
        };

        _searchCommonParameters = new()
        {
            ["_content"] = BuildStringParameter("_content", "Search on the entire content of the resource"),
            ["_filter"] = BuildStringParameter("_filter", "Provide an inline query expression"),
            ["_id"] = BuildStringParameter("_id", "Logical id of this artifact"),
            ["_in"] = BuildStringParameter("_in", "Allows for the retrieval of resources that are active members of a CareTeam, Group, or List"),
            ["_language"] = BuildStringParameter("_language", "Language of the resource content"),
            ["_lastUpdated"] = BuildStringParameter("_lastUpdated", "When the resource version last changed"),
            ["_list"] = BuildStringParameter("_list", "Allows for the retrieval of resources that are referenced by a List resource or by one of the pre-defined functional lists"),
            ["_profile"] = BuildStringParameter("_profile", "Profiles this resource claims to conform to"),
            ["_query"] = BuildStringParameter("_query", "A custom search profile that describes a specific defined query operation"),
            ["_security"] = BuildStringParameter("_security", "Security Labels applied to this resource"),
            ["_source"] = BuildStringParameter("_source", "Identifies where the resource comes from"),
            ["_tag"] = BuildStringParameter("_tag", "Tags applied to this resource"),
            ["_text"] = BuildStringParameter("_text", "Perform searches against the narrative content of a resource"),
        };

        _searchResultParameters = new()
        {
            ["_contained"] = BuildStringParameter("_contained", "Request different types of handling for contained resources"),
            ["_count"] = BuildNumberParameter("_count", "Limit the number of match results per page of response"),
            //["_elements"] = BuildStringParameter("_elements", "Request that only a specific set of elements be returned for resources"),
            ["_graph"] = BuildStringParameter("_graph", "Include additional resources according to a GraphDefinition"),
            ["_include"] = BuildStringParameter("_include", "Include additional resources, based on following links forward across references"),
            ["_revinclude"] = BuildStringParameter("_revinclude", "Include additional resources, based on following reverse links across references"),
            ["_score"] = BuildStringParameter("_score", "Request match relevance in results"),
            ["_sort"] = BuildStringParameter("_sort", "Request which order results should be returned in"),
            ["_total"] = BuildStringParameter("_total", "Request a precision of the total number of results for a request"),
        };

        _searchRootParameters = new()
        {
            ["_type"] = BuildStringParameter("_type", "A resource type filter"),
        };

        _pathParameters = new()
        {
            ["id"] = BuildPathParameter("id", "Resource Logical ID"),
            ["vid"] = BuildPathParameter("vid", "Resource Version Number"),
        };

        _historyParameters = new()
        {
            ["_count"] = BuildNumberParameter("_count", "Limit the number of match results per page of response"),
            ["_since"] = BuildStringParameter("_since", "Only include resource versions that were created at or after the given instant in time"),
            ["_at"] = BuildStringParameter("_at", "Only include resource versions that were current at some point during the time period specified in the date time value"),
            ["_list"] = BuildStringParameter("_list", "Only include resource versions that are referenced in the specified list"),
            ["_sort"] = BuildStringParameter("_sort", "Request which order results should be returned in"),
        };

    }

    /// <summary>Builds summary parameter.</summary>
    /// <returns>An OpenApiParameter.</returns>
    private static OpenApiParameter BuildSummaryParameter()
    {
        return new OpenApiParameter()
        {
            Name = "_summary",
            In = ParameterLocation.Query,
            Description = "Ask for a predefined short form of the resource in response",
            Required = false,
            Schema = new OpenApiSchema()
            {
                Type = "string",
                Enum = new List<IOpenApiAny>()
                    {
                        new OpenApiString("true"),
                        new OpenApiString("text"),
                        new OpenApiString("data"),
                        new OpenApiString("count"),
                        new OpenApiString("false"),
                    },
            },
        };
    }

    /// <summary>Builds path parameter.</summary>
    /// <param name="name">       The name.</param>
    /// <param name="description">The description.</param>
    /// <returns>An OpenApiParameter.</returns>
    internal static OpenApiParameter BuildPathParameter(string name, string description)
    {
        return new OpenApiParameter()
        {
            Name = name,
            In = ParameterLocation.Path,
            Description = description,
            Required = true,
            Schema = new OpenApiSchema()
            {
                Type = "string",
            },
        };
    }

    /// <summary>Builds string parameter.</summary>
    /// <param name="name">       The name.</param>
    /// <param name="description">The description.</param>
    /// <returns>An OpenApiParameter.</returns>
    internal static OpenApiParameter BuildStringParameter(string name, string description)
    {
        return new OpenApiParameter()
        {
            Name = name,
            In = ParameterLocation.Query,
            Description = description,
            Required = false,
            Schema = new OpenApiSchema()
            {
                Type = "string",
            },
        };
    }

    /// <summary>Builds number parameter.</summary>
    /// <param name="name">       The name.</param>
    /// <param name="description">The description.</param>
    /// <returns>An OpenApiParameter.</returns>
    internal static OpenApiParameter BuildNumberParameter(string name, string description)
    {
        return new OpenApiParameter()
        {
            Name = name,
            In = ParameterLocation.Query,
            Description = description,
            Required = false,
            Schema = new OpenApiSchema()
            {
                Type = "number",
            },
        };
    }

    /// <summary>Builds referenced parameter.</summary>
    /// <returns>An OpenApiParameter.</returns>
    internal static OpenApiParameter BuildReferencedParameter(string name)
    {
        return new OpenApiParameter()
        {
            Name = name,
            Reference = new OpenApiReference()
            {
                Id = name,
                Type = ReferenceType.Parameter,
            },
        };
    }
}
