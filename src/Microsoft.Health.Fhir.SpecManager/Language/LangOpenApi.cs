// <copyright file="LangOpenApi.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.IO;
using Microsoft.Health.Fhir.SpecManager.Manager;
using Microsoft.Health.Fhir.SpecManager.Models;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Microsoft.VisualBasic;
using static Microsoft.Health.Fhir.CodeGenCommon.Models.FhirServerResourceInfo;

namespace Microsoft.Health.Fhir.SpecManager.Language
{
    /// <summary>An OpenApi language exporter.</summary>
    public sealed class LangOpenApi : ILanguage
    {
        /// <summary>FHIR information we are exporting.</summary>
        private FhirVersionInfo _info;

        /// <summary>Information describing the server.</summary>
        private FhirServerInfo _serverInfo;

        /// <summary>Options for controlling the export.</summary>
        private ExporterOptions _options;

        /// <summary>The exported codes.</summary>
        private HashSet<string> _exportedCodes = new HashSet<string>();

        /// <summary>Name of the language.</summary>
        private const string _languageName = "OpenApi";

        /// <summary>The single file export extension.</summary>
        private const string _singleFileExportExtension = ".json";

        /// <summary>Options for controlling the operation.</summary>
        private Dictionary<string, string> _parameters = new Dictionary<string, string>();

        /// <summary>The instance operation prefixes.</summary>
        private Dictionary<OperationType, string> _instanceOpPrefixes;

        /// <summary>The type operation prefixes.</summary>
        private Dictionary<OperationType, string> _typeOpPrefixes;

        /// <summary>True to explicit FHIR JSON.</summary>
        private bool _fhirJson = false;

        /// <summary>True to explicit FHIR XML.</summary>
        private bool _fhirXml = false;

        /// <summary>True to patch JSON.</summary>
        private bool _patchJson = false;

        /// <summary>True to patch XML.</summary>
        private bool _patchXml = false;

        /// <summary>True to patch FHIR JSON.</summary>
        private bool _patchFhirJson = false;

        /// <summary>True to patch FHIR XML.</summary>
        private bool _patchFhirXml = false;

        /// <summary>The search support.</summary>
        private FhirServerResourceInfo.SearchSupportCodes _searchSupport = FhirServerResourceInfo.SearchSupportCodes.Both;

        /// <summary>The search parameter location.</summary>
        private FhirServerResourceInfo.SearchPostParameterLocationCodes _searchParamLoc = SearchPostParameterLocationCodes.Body;

        private Dictionary<string, OpenApiParameter> _commonParameters;

        /// <summary>True to single response code.</summary>
        private bool _singleResponseCode = false;

        /// <summary>True to include, false to exclude the summaries.</summary>
        private bool _includeSummaries = true;

        /// <summary>True to include, false to exclude the schemas.</summary>
        private bool _includeSchemas = true;

        /// <summary>True to include, false to exclude the schema descriptions.</summary>
        private bool _includeDescriptions = true;

        /// <summary>True to validate descriptions.</summary>
        private bool _descriptionValidation = false;

        /// <summary>Length of the description maximum.</summary>
        private int _descriptionMaxLen = 60;

        /// <summary>True to expand references based on allowed profiles.</summary>
        private bool _expandProfiles = true;

        /// <summary>True to expand references.</summary>
        private bool _expandReferences = true;

        /// <summary>True to generate read only.</summary>
        private bool _generateReadOnly = false;

        /// <summary>True to generate write only.</summary>
        private bool _generateWriteOnly = false;

        /// <summary>True to include, false to exclude the bundle operations.</summary>
        private bool _includeBundleOperations = true;

        /// <summary>True to include, false to exclude the metadata.</summary>
        private bool _includeMetadata = false;

        /// <summary>True to inline schemas.</summary>
        private bool _inlineSchemas = false;

        /// <summary>True to include, false to exclude the history.</summary>
        private bool _includeHistory = false;

        /// <summary>Maximum number of times to recurse.</summary>
        private int _maxRecursions = 0;

        /// <summary>True to remove uncommon fields.</summary>
        private bool _removeUncommonFields = false;

        private HashSet<string> _resourcesToExport = null;

        /// <summary>The uncommon fields.</summary>
        private static readonly HashSet<string> _uncommonFields = new HashSet<string>()
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

        /// <summary>The open API version.</summary>
        private int _openApiVersion = 2;

        /// <summary>Dictionary mapping FHIR primitive types to language equivalents.</summary>
        private static readonly Dictionary<string, string> _primitiveTypeMap = new Dictionary<string, string>()
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

        /// <summary>Gets the reserved words.</summary>
        /// <value>The reserved words.</value>
        private static readonly HashSet<string> _reservedWords = new HashSet<string>();

        /// <summary>The HTTP response descriptions.</summary>
        private static readonly Dictionary<int, string> _httpResponseDescriptions = new()
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

        /// <summary>Gets the name of the language.</summary>
        /// <value>The name of the language.</value>
        string ILanguage.LanguageName => _languageName;

        /// <summary>
        /// Gets the single file extension for this language - null or empty indicates a multi-file
        /// export (exporter should copy the contents of the directory).
        /// </summary>
        string ILanguage.SingleFileExportExtension => _singleFileExportExtension;

        /// <summary>Gets the FHIR primitive type map.</summary>
        /// <value>The FHIR primitive type map.</value>
        Dictionary<string, string> ILanguage.FhirPrimitiveTypeMap => _primitiveTypeMap;

        /// <summary>Gets the reserved words.</summary>
        /// <value>The reserved words.</value>
        HashSet<string> ILanguage.ReservedWords => _reservedWords;

        /// <summary>
        /// Gets a list of FHIR class types that the language WILL export, regardless of user choices.
        /// Used to provide information to users.
        /// </summary>
        List<ExporterOptions.FhirExportClassType> ILanguage.RequiredExportClassTypes => new List<ExporterOptions.FhirExportClassType>()
        {
        };

        /// <summary>
        /// Gets a list of FHIR class types that the language CAN export, depending on user choices.
        /// </summary>
        List<ExporterOptions.FhirExportClassType> ILanguage.OptionalExportClassTypes => new List<ExporterOptions.FhirExportClassType>()
        {
            ExporterOptions.FhirExportClassType.ComplexType,
            ExporterOptions.FhirExportClassType.Resource,
            ExporterOptions.FhirExportClassType.Interaction,
            ExporterOptions.FhirExportClassType.Profile,
        };

        /// <summary>Gets language-specific options and their descriptions.</summary>
        Dictionary<string, string> ILanguage.LanguageOptions => new Dictionary<string, string>()
        {
            { "BundleOperations", "If the generator should include /Bundle, etc. (true|false)" },
            { "Descriptions", "If properties should include descriptions (true|false)." },
            { "DescriptionMaxLen", "Maximum length of descriptions, if being validated (60)." },
            { "DescriptionValidation", "If descriptions are required and should be validated (false|true)." },
            { "ExpandProfiles", "If types should expand based on allowed profiles (true|false)." },
            { "ExpandReferences", "If types should expand through references (true|false)" },
            { "FhirJson", "If paths should explicitly support FHIR+JSON (true|false)." },
            { "FhirXml", "If paths should explicitly support FHIR+XML (false|true)." },

            { "PatchJson", "If PATCH operations should explicitly support json-patch (false|true)." },
            { "PatchXml", "If PATCH operations should explicitly support XML-patch (false|true)." },
            { "PatchFhir", "If PATCH operations should explicitly support FHIR types (true|false)." },

            { "SearchSupport", "Supported search methods (both|get|post|none)." },
            { "SearchPostParams", "Where search params should appear in post-based search (body|query|both|none)." },

            //{ "ModelOnlyObjects", "If all models should be reduced to 'object' (false|true)." },

            { "History", "If _history GET operations should be included (false|true)" },
            { "MaxRecurisions", "Maximum depth to expand recursions (0)." },
            { "Metadata", "If the JSON should include a link to /metadata (false|true)." },
            { "Minify", "If the output JSON should be minified (false|true)." },
            { "OpenApiVersion", "Open API version to use (2, 3)." },
            { "OperationCase", "Case of the first letter of Operation IDs (upper|lower)." },
            { "ReadOnly", "If the output should only contain GET operations (false|true)." },
            { "RemoveUncommonFields", "If the generator should remove some uncommon fields (false|true)" },
            { "Schemas", "If schemas should be included (true|false)." },
            { "SchemasInline", "If the output should inline all schemas (no inheritance) (false|true)." },
            { "SingleResponses", "If operations should only include a single response (false|true)." },
            { "Summaries", "If responses should include summaries (true|false)." },
            { "Title", "Title to use in the Info section." },
            { "WriteOnly", "If the output should only contain POST/PUT/DELETE operations (false|true)." },
        };

        /// <summary>Export the passed FHIR version into the specified directory.</summary>
        /// <param name="info">           The information.</param>
        /// <param name="serverInfo">     Information describing the server.</param>
        /// <param name="options">        Options for controlling the operation.</param>
        /// <param name="exportDirectory">Directory to write files.</param>
        void ILanguage.Export(
            FhirVersionInfo info,
            FhirServerInfo serverInfo,
            ExporterOptions options,
            string exportDirectory)
        {
            // set internal vars so we don't pass them to every function
            // this is ugly, but the interface patterns get bad quickly because we need the type map to copy the FHIR info
            _info = info;
            _serverInfo = serverInfo;
            _options = options;

            if (_options.LanguageOptions != null)
            {
                _parameters = _options.LanguageOptions;
            }

            if ((options.ExportList != null) && options.ExportList.Any())
            {
                _resourcesToExport = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

                foreach (string key in options.ExportList)
                {
                    _resourcesToExport.Add(key);
                }
            }

            _exportedCodes = new HashSet<string>();

            _includeBundleOperations = _options.GetParam("BundleOperations", true);
            _includeDescriptions = _options.GetParam("Descriptions", true);
            _descriptionMaxLen = _options.GetParam("DescriptionMaxLen", 60);
            _descriptionValidation = _options.GetParam("DescriptionValidation", false);
            _expandProfiles = _options.GetParam("ExpandProfiles", true);
            _expandReferences = _options.GetParam("ExpandReferences", true);
            _fhirJson = _options.GetParam("FhirJson", true);
            _fhirXml = _options.GetParam("FhirXml", false);

            _patchJson = _options.GetParam("PatchJson", false);
            _patchXml = _options.GetParam("PatchXml", false);
            _patchFhirJson = _options.GetParam("PatchFhir", true) && _fhirJson;
            _patchFhirXml = _options.GetParam("PatchFhir", true) && _fhirXml;

            string searchSupport = _options.GetParam("SearchSupport", "both");
            switch (searchSupport.ToUpperInvariant())
            {
                case "NONE":
                    _searchSupport = FhirServerResourceInfo.SearchSupportCodes.None;
                    break;

                case "GET":
                    _searchSupport = FhirServerResourceInfo.SearchSupportCodes.Get;
                    break;

                case "POST":
                    _searchSupport = FhirServerResourceInfo.SearchSupportCodes.Post;
                    break;

                default:
                case "BOTH":
                    _searchSupport = FhirServerResourceInfo.SearchSupportCodes.Both;
                    break;
            }

            string postParamLoc = _options.GetParam("SearchPostParams", "body");
            switch (postParamLoc.ToUpperInvariant())
            {
                case "NONE":
                    _searchParamLoc = SearchPostParameterLocationCodes.None;
                    break;

                case "BOTH":
                    _searchParamLoc = SearchPostParameterLocationCodes.Both;
                    break;

                case "QUERY":
                    _searchParamLoc = SearchPostParameterLocationCodes.Query;
                    break;

                default:
                case "BODY":
                    _searchParamLoc = SearchPostParameterLocationCodes.Body;
                    break;
            }

            _includeHistory = _options.GetParam("History", false);
            _maxRecursions = _options.GetParam("MaxRecursions", 0);
            _includeMetadata = _options.GetParam("Metadata", false);
            bool minify = _options.GetParam("Minify", false);
            _openApiVersion = _options.GetParam("OpenApiVersion", 2);
            _generateReadOnly = _options.GetParam("ReadOnly", false);
            _removeUncommonFields = _options.GetParam("RemoveUncommonFields", false);
            _includeSchemas = _options.GetParam("Schemas", true);
            _inlineSchemas = _options.GetParam("SchemasInline", false);
            _singleResponseCode = _options.GetParam("SingleResponses", false);
            _includeSummaries = _options.GetParam("Summaries", true);
            _generateWriteOnly = _options.GetParam("WriteOnly", false);

            string opCase = _options.GetParam("OperationCase", "Upper");
            if (opCase.StartsWith("L", StringComparison.OrdinalIgnoreCase))
            {
                _instanceOpPrefixes = new Dictionary<OperationType, string>()
                {
                    [OperationType.Delete] = "delete",
                    [OperationType.Get] = "get",
                    [OperationType.Head] = "head",
                    [OperationType.Options] = "options",
                    [OperationType.Patch] = "update",
                    [OperationType.Post] = "create",
                    [OperationType.Put] = "update",
                    [OperationType.Trace] = "trace",
                };

                _typeOpPrefixes = new Dictionary<OperationType, string>()
                {
                    [OperationType.Delete] = "delete",
                    [OperationType.Get] = "list",
                    [OperationType.Head] = "head",
                    [OperationType.Options] = "options",
                    [OperationType.Patch] = "update",
                    [OperationType.Post] = "create",
                    [OperationType.Put] = "update",
                    [OperationType.Trace] = "trace",
                };
            }
            else
            {
                _instanceOpPrefixes = new Dictionary<OperationType, string>()
                {
                    [OperationType.Delete] = "Delete",
                    [OperationType.Get] = "Get",
                    [OperationType.Head] = "Head",
                    [OperationType.Options] = "Options",
                    [OperationType.Patch] = "Update",
                    [OperationType.Post] = "Create",
                    [OperationType.Put] = "Update",
                    [OperationType.Trace] = "Trace",
                };

                _typeOpPrefixes = new Dictionary<OperationType, string>()
                {
                    [OperationType.Delete] = "Delete",
                    [OperationType.Get] = "List",
                    [OperationType.Head] = "Head",
                    [OperationType.Options] = "Options",
                    [OperationType.Patch] = "Update",
                    [OperationType.Post] = "Create",
                    [OperationType.Put] = "Update",
                    [OperationType.Trace] = "Trace",
                };
            }

            OpenApiDocument document = new OpenApiDocument();

            document.Info = BuildInfo();

            document.Components = new OpenApiComponents();

            _commonParameters = new Dictionary<string, OpenApiParameter>()
            {
                ["id"] = BuildPathIdParameter(),
                ["vid"] = BuildPathVidParameter(),
                ["_format"] = BuildStringParameter("_format", "Override the HTTP content negotiation"),
                ["_pretty"] = BuildStringParameter("_pretty", "Ask for a pretty printed response for human convenience"),
                ["_summary"] = BuildSummaryParameter(),
                ["_elements"] = BuildStringParameter("_elements", "Ask for a particular set of elements to be returned"),
                ["_content"] = BuildStringParameter("_content", "Search on the entire content of the resource"),
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
                ["_type"] = BuildStringParameter("_type", "A resource type filter"),
            };

            document.Components.Parameters = _commonParameters;

            if (_includeSchemas && (!_inlineSchemas))
            {
                document.Components.Schemas = BuildSchemas();
            }

            if (_serverInfo != null)
            {
                document.Servers = BuildServers();

                document.Paths = BuildPathsForServer();
            }

            // create a filename for writing (single file for now)
            string filename = Path.Combine(exportDirectory, $"OpenApi_{info.FhirSequence}_v{_openApiVersion}.json");

            using (FileStream stream = new FileStream(filename, FileMode.Create))
            using (StreamWriter sw = new StreamWriter(stream))
            {
                // OpenApiYamlWriter writer = new OpenApiYamlWriter(sw);
                OpenApiJsonWriter writer = new OpenApiJsonWriter(sw, new OpenApiJsonWriterSettings() { Terse = minify });

                if (_openApiVersion == 2)
                {
                    document.Serialize(writer, OpenApiSpecVersion.OpenApi2_0);
                }
                else
                {
                    document.Serialize(writer, OpenApiSpecVersion.OpenApi3_0);
                }
            }
        }

        /// <summary>Builds the schemas.</summary>
        /// <returns>A filled out schema dictionary for the requested models.</returns>
        private Dictionary<string, OpenApiSchema> BuildSchemas()
        {
            Dictionary<string, OpenApiSchema> schemas = new Dictionary<string, OpenApiSchema>();

            foreach (FhirComplex complex in _info.ComplexTypes.Values.OrderBy(c => c.Name))
            {
                BuildSchema(
                    schemas,
                    complex,
                    null,
                    false);
            }

            foreach (FhirComplex complex in _info.Resources.Values.OrderBy(c => c.Name))
            {
                BuildSchema(
                    schemas,
                    complex,
                    null,
                    true);
            }

            return schemas;
        }

        /// <summary>Builds type name from path.</summary>
        /// <param name="type">The type.</param>
        /// <returns>A string.</returns>
        private string BuildTypeNameFromPath(string type)
        {
            if (_info.FhirSequence == FhirPackageCommon.FhirSequenceEnum.DSTU2)
            {
                switch (type)
                {
                    case "TestScript.setup.action":
                        return $"TestScriptSetupActionComponent";

                    case "TestScript.test.action":
                        return $"TestScriptTestActionComponent";

                    case "TestScript.teardown.action":
                        return $"TestScriptTearDownActionComponent";
                }
            }

            if (_info.TryGetExplicitName(type, out string explicitTypeName))
            {
                string parentName = type.Substring(0, type.IndexOf('.'));
                type = $"{parentName}" +
                    $"{explicitTypeName}" +
                    $"Component";
            }
            else
            {
                StringBuilder sb = new StringBuilder();

                string[] components = type.Split('.');

                for (int i = 0; i < components.Length; i++)
                {
                    if (i == 0)
                    {
                        sb.Append(components[i]);
                        continue;
                    }

                    // AdverseEvent.suspectEntity.Causality does not prefix?
                    if (i == components.Length - 1)
                    {
                        sb.Append(FhirUtils.SanitizedToConvention(components[i], FhirTypeBase.NamingConvention.PascalCase));
                    }
                }

                if (components.Length > 1)
                {
                    sb.Append("Component");
                }

                type = sb.ToString();
            }

            return type;
        }

        /// <summary>Builds a schema.</summary>
        /// <param name="schemas">   The schemas.</param>
        /// <param name="complex">   The complex.</param>
        /// <param name="root">      (Optional) The root.</param>
        /// <param name="isResource">(Optional) True if is resource, false if not.</param>
        private void BuildSchema(
            Dictionary<string, OpenApiSchema> schemas,
            FhirComplex complex,
            FhirComplex root = null,
            bool isResource = false)
        {
            string typeName = BuildTypeNameFromPath(complex.Path);

            if (schemas.ContainsKey(typeName))
            {
                return;
            }

            OpenApiSchema schema = new OpenApiSchema()
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>(),
                Description =
                    _includeDescriptions
                    ? SanitizeDescription(complex.ShortDescription)
                    : null,
            };

            if (complex.Name == "Extension")
            {
                schemas.Add(typeName, schema);
                return;
            }

            if (string.IsNullOrEmpty(complex.BaseTypeName) ||
                complex.Name.Equals("Element", StringComparison.Ordinal))
            {
                schema.Type = "object";
            }
            else if (complex.Name.Equals(complex.BaseTypeName, StringComparison.Ordinal))
            {
                schema.Type = "object";
            }
            else
            {
                schema.Type = "object";

                if ((complex.Path != "Resource") &&
                    (complex.Path != "Element"))
                {
                    schema.AllOf = new List<OpenApiSchema>()
                    {
                        new OpenApiSchema()
                        {
                            Reference = new OpenApiReference()
                            {
                                Id = BuildTypeNameFromPath(complex.BaseTypeName),
                                Type = ReferenceType.Schema,
                            },
                        },
                    };
                }
            }

            if (complex.Path == "Resource")
            {
                schema.Properties.Add(
                    "resourceType",
                    new OpenApiSchema()
                    {
                        Type = "string",
                        Description = _includeDescriptions ? "Resource type name" : null,
                    });

                schema.Discriminator = new OpenApiDiscriminator()
                {
                    PropertyName = "resourceType",
                    Mapping = new Dictionary<string, string>(),
                };

                foreach (string resourceName in _info.Resources.Keys.OrderBy(s => s))
                {
                    if (resourceName == "Resource")
                    {
                        continue;
                    }

                    schema.Discriminator.Mapping.Add(resourceName, resourceName);
                }

                schema.Required = new HashSet<string>()
                {
                    "resourceType",
                };
            }
            else if (isResource)
            {
                schema.Pattern = complex.Path;
            }

            if (root == null)
            {
                root = complex;
            }

            if (complex.Elements != null)
            {
                foreach (FhirElement element in complex.Elements.Values.OrderBy(e => e.Name))
                {
                    if (complex.Components.ContainsKey(element.Path))
                    {
                        BuildSchema(
                            schemas,
                            complex.Components[element.Path],
                            root);
                    }

                    BuildElementSchema(ref schema, element);
                }
            }

            schemas.Add(typeName, schema);
        }

        /// <summary>Builds element schema.</summary>
        /// <param name="parentSchema">[in,out] The parent schema.</param>
        /// <param name="element">     The element.</param>
        private void BuildElementSchema(
            ref OpenApiSchema parentSchema,
            FhirElement element)
        {
            string name = GetElementName(element);
            OpenApiSchema schema = new OpenApiSchema()
            {
                Description =
                    _includeDescriptions
                    ? SanitizeDescription(element.ShortDescription)
                    : null,
            };

            if (element.Path == "Reference.identifier")
            {
                schema.Type = "object";
                parentSchema.Properties.Add(
                    GetElementName(element),
                    schema);
                return;
            }

            if ((element.ElementTypes != null) &&
                (element.ElementTypes.Count > 1))
            {
                foreach (FhirElementType elementType in element.ElementTypes.Values)
                {
                    string pascal = FhirUtils.SanitizedToConvention(elementType.Name, FhirTypeBase.NamingConvention.PascalCase);

                    OpenApiSchema subSchema = new OpenApiSchema()
                    {
                        Description =
                            _includeDescriptions
                            ? SanitizeDescription(element.ShortDescription)
                            : null,
                    };

                    SetSchemaType(elementType.Name, ref subSchema);

                    parentSchema.Properties.Add(
                        $"{name}{pascal}",
                        subSchema);
                }

                return;
            }

            string type;

            if (!string.IsNullOrEmpty(element.BaseTypeName))
            {
                type = element.BaseTypeName;
            }
            else if (element.ElementTypes.Count == 1)
            {
                FhirElementType elementType = element.ElementTypes.First().Value;

                type = elementType.Name;

                if (_expandProfiles && (type == "Resource"))
                {
                    if (_openApiVersion == 2)
                    {
                        SetSchemaType(type, ref schema, element.IsArray);
                    }
                    else
                    {
                        schema.OneOf = new List<OpenApiSchema>();

                        if ((elementType.Profiles == null) ||
                            (elementType.Profiles.Count == 0))
                        {
                            foreach (FhirComplex resource in _info.Resources.Values)
                            {
                                OpenApiSchema subSchema = new OpenApiSchema();
                                SetSchemaType(resource.Name, ref subSchema, element.IsArray);
                                schema.OneOf.Add(subSchema);
                            }
                        }
                        else
                        {
                            foreach (FhirElementProfile profile in elementType.Profiles.Values)
                            {
                                OpenApiSchema subSchema = new OpenApiSchema();
                                SetSchemaType(profile.Name, ref subSchema, element.IsArray);
                                schema.OneOf.Add(subSchema);
                            }
                        }
                    }

                    parentSchema.Properties.Add(
                        GetElementName(element),
                        schema);

                    return;
                }
            }
            else
            {
                type = "Element";
            }

            SetSchemaType(type, ref schema, element.IsArray);

            parentSchema.Properties.Add(
                GetElementName(element),
                schema);
        }

        /// <summary>Sets a type.</summary>
        /// <param name="baseType">Type of the base.</param>
        /// <param name="schema">  [in,out] The schema.</param>
        /// <param name="isArray"> (Optional) True if is array, false if not.</param>
        private void SetSchemaType(
            string baseType,
            ref OpenApiSchema schema,
            bool isArray = false)
        {
            if (_primitiveTypeMap.ContainsKey(baseType))
            {
                string type = _primitiveTypeMap[baseType];

                if (type.Contains(':'))
                {
                    string[] parts = type.Split(':');

                    if (isArray)
                    {
                        schema.Type = "array";
                        schema.Items = new OpenApiSchema()
                        {
                            Type = parts[0],
                            Format = parts[1],
                        };
                    }
                    else
                    {
                        schema.Type = parts[0];
                        schema.Format = parts[1];
                    }
                }
                else
                {
                    if (isArray)
                    {
                        schema.Type = "array";
                        schema.Items = new OpenApiSchema()
                        {
                            Type = type,
                        };
                    }
                    else
                    {
                        schema.Type = type;
                    }
                }

                return;
            }

            if (isArray)
            {
                schema.Type = "array";
                schema.Items = new OpenApiSchema()
                {
                    Reference = new OpenApiReference()
                    {
                        Id = BuildTypeNameFromPath(baseType),
                        Type = ReferenceType.Schema,
                    },
                };
            }
            else
            {
                schema.Reference = new OpenApiReference()
                {
                    Id = BuildTypeNameFromPath(baseType),
                    Type = ReferenceType.Schema,
                };
            }
        }

        /// <summary>Gets element name.</summary>
        /// <param name="element">The element.</param>
        /// <returns>The element name.</returns>
        private static string GetElementName(FhirElement element)
        {
            string name = element.Name.Replace("[x]", string.Empty);

            return FhirUtils.SanitizedToConvention(
                name,
                FhirTypeBase.NamingConvention.CamelCase);
        }

        /// <summary>Builds the OpenAPI paths object based on a known server.</summary>
        /// <returns>The OpenApiPaths.</returns>
        private OpenApiPaths BuildPathsForServer()
        {
            if (_serverInfo == null)
            {
                return null;
            }

            OpenApiPaths paths = new OpenApiPaths();

            if (_includeMetadata)
            {
                if (_info.Resources.ContainsKey("CapabilityStatement"))
                {
                    OpenApiPathItem metadataPath = new OpenApiPathItem()
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>(),
                    };

                    metadataPath.Operations.Add(
                        OperationType.Get,
                        BuildMetadataPathOperation("CapabilityStatement"));

                    paths.Add($"/metadata", metadataPath);
                }
                else if (_info.Resources.ContainsKey("Conformance"))
                {
                    OpenApiPathItem metadataPath = new OpenApiPathItem()
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>(),
                    };

                    metadataPath.Operations.Add(
                        OperationType.Get,
                        BuildMetadataPathOperation("Conformance"));

                    paths.Add($"/metadata", metadataPath);
                }
            }

            foreach (FhirServerResourceInfo resource in _serverInfo.ResourceInteractions.Values)
            {
                if ((!_includeBundleOperations) &&
                    (resource.ResourceType == "Bundle"))
                {
                    continue;
                }

                if ((_resourcesToExport != null) &&
                    (!_resourcesToExport.Contains(resource.ResourceType)))
                {
                    continue;
                }

                Dictionary<string, Dictionary<OperationType, OpenApiOperation>> opsByMethodByPath = new();

                foreach (FhirServerResourceInfo.FhirInteraction interaction in resource.Interactions)
                {
                    switch (interaction)
                    {
                        case FhirServerResourceInfo.FhirInteraction.Read:
                            AddPath(
                                opsByMethodByPath,
                                $"/{resource.ResourceType}/{{id}}",
                                OperationType.Get,
                                $"{interaction}{resource.ResourceType}",
                                $"{interaction} a {resource.ResourceType} instance",
                                $"{interaction} a {resource.ResourceType} instance",
                                true,
                                false,
                                new int[] { 200, 410, 404 },
                                resource.ResourceType,
                                false,
                                false,
                                null,
                                null);

                            break;

                        case FhirServerResourceInfo.FhirInteraction.VRead:
                            AddPath(
                                opsByMethodByPath,
                                $"/{resource.ResourceType}/{{id}}/_history/{{vid}}",
                                OperationType.Get,
                                $"{interaction}{resource.ResourceType}",
                                $"{interaction} a {resource.ResourceType} instance",
                                $"{interaction} a {resource.ResourceType} instance",
                                true,
                                true,
                                new int[] { 200, 410, 404 },
                                resource.ResourceType,
                                false,
                                false,
                                null,
                                null);

                            break;

                        case FhirServerResourceInfo.FhirInteraction.HistoryInstance:
                            if (!_includeHistory)
                            {
                                continue;
                            }

                            AddPath(
                                opsByMethodByPath,
                                $"/{resource.ResourceType}/{{id}}/_history",
                                OperationType.Get,
                                $"{interaction}{resource.ResourceType}",
                                $"History of a {resource.ResourceType} instance",
                                $"Get the history of a {resource.ResourceType} instance",
                                true,
                                false,
                                new int[] { 200, 410, 404 },
                                resource.ResourceType,
                                true,
                                false,
                                null,
                                null);

                            break;

                        case FhirServerResourceInfo.FhirInteraction.HistoryType:
                            if (!_includeHistory)
                            {
                                continue;
                            }

                            AddPath(
                                opsByMethodByPath,
                                $"/{resource.ResourceType}/_history",
                                OperationType.Get,
                                $"{interaction}{resource.ResourceType}",
                                $"History of all {resource.ResourceType}s",
                                $"Get the history of all {resource.ResourceType} instances",
                                false,
                                false,
                                new int[] { 200, 410, 404 },
                                resource.ResourceType,
                                true,
                                true,
                                resource.SearchParameters.Values,
                                null);

                            break;

                        case FhirServerResourceInfo.FhirInteraction.Patch:
                            if (_generateReadOnly)
                            {
                                continue;
                            }

                            AddPath(
                                opsByMethodByPath,
                                $"/{resource.ResourceType}/{{id}}",
                                OperationType.Patch,
                                $"{interaction}{resource.ResourceType}",
                                $"{interaction} a {resource.ResourceType} instance",
                                $"{interaction} a {resource.ResourceType} instance",
                                true,
                                false,
                                new int[] { 200, 400, 401, 404, 412, },
                                resource.ResourceType,
                                false,
                                false,
                                null,
                                new OpenApiRequestBody()
                                {
                                    Content = BuildContentMapForPatch(resource.ResourceType),
                                    Description = _includeDescriptions ? $"A {resource.ResourceType}" : null,
                                });

                            break;

                        case FhirServerResourceInfo.FhirInteraction.Update:
                            if (_generateReadOnly)
                            {
                                continue;
                            }

                            AddPath(
                                opsByMethodByPath,
                                $"/{resource.ResourceType}/{{id}}",
                                OperationType.Put,
                                $"{interaction}{resource.ResourceType}",
                                $"{interaction} a {resource.ResourceType} instance",
                                $"{interaction} a {resource.ResourceType} instance",
                                true,
                                false,
                                new int[] { 200, 400, 401, 404, 405, 409, 412, 422, },
                                resource.ResourceType,
                                false,
                                false,
                                null,
                                new OpenApiRequestBody()
                                {
                                    Required = true,
                                    Content = BuildContentMap(resource.ResourceType, false),
                                    Description = _includeDescriptions ? $"A {resource.ResourceType}" : null,
                                });

                            break;

                        case FhirServerResourceInfo.FhirInteraction.Create:
                            if (_generateReadOnly)
                            {
                                continue;
                            }

                            AddPath(
                                opsByMethodByPath,
                                $"/{resource.ResourceType}",
                                OperationType.Post,
                                $"{interaction}{resource.ResourceType}",
                                $"{interaction} a {resource.ResourceType} instance",
                                $"{interaction} a {resource.ResourceType} instance",
                                false,
                                false,
                                new int[] { 200, 400, 404, 422, },
                                resource.ResourceType,
                                false,
                                false,
                                null,
                                new OpenApiRequestBody()
                                {
                                    Required = true,
                                    Content = BuildContentMap(resource.ResourceType, false),
                                    Description = _includeDescriptions ? $"A {resource.ResourceType}" : null,
                                });

                            if (resource.ConditionalCreate == true)
                            {
                                AddPath(
                                    opsByMethodByPath,
                                    $"/{resource.ResourceType}",
                                    OperationType.Put,
                                    $"ConditionalCreate{resource.ResourceType}",
                                    $"Conditionally Create a {resource.ResourceType} instance",
                                    $"Conditionally Create a {resource.ResourceType} instance",
                                    false,
                                    false,
                                    new int[] { 200, 400, 404, 412, },
                                    resource.ResourceType,
                                    false,
                                    true,
                                    resource.SearchParameters.Values,
                                    new OpenApiRequestBody()
                                    {
                                        Required = true,
                                        Content = BuildContentMap(resource.ResourceType, false),
                                        Description = _includeDescriptions ? $"A {resource.ResourceType}" : null,
                                    });
                            }

                            break;

                        case FhirServerResourceInfo.FhirInteraction.Delete:
                            if (_generateReadOnly)
                            {
                                continue;
                            }

                            AddPath(
                                opsByMethodByPath,
                                $"/{resource.ResourceType}/{{id}}",
                                OperationType.Delete,
                                $"{interaction}{resource.ResourceType}",
                                $"{interaction} a {resource.ResourceType} instance",
                                $"{interaction} a {resource.ResourceType} instance",
                                true,
                                false,
                                new int[] { 200, 202, 204, 400, 401, 404, 405, 409, },
                                resource.ResourceType,
                                false,
                                false,
                                null,
                                null);

                            if ((resource.ConditionalDelete == ConditionalDeletePolicy.Single) ||
                                (resource.ConditionalDelete == ConditionalDeletePolicy.Multiple))
                            {
                                AddPath(
                                    opsByMethodByPath,
                                    $"/{resource.ResourceType}",
                                    OperationType.Delete,
                                    $"ConditionalDelete{resource.ResourceType}",
                                    $"Conditionally Delete a {resource.ResourceType} instance",
                                    $"Conditionally Delete a {resource.ResourceType} instance",
                                    false,
                                    false,
                                    new int[] { 200, 202, 204, 400, 404, 412, },
                                    resource.ResourceType,
                                    false,
                                    true,
                                    resource.SearchParameters.Values,
                                    null);
                            }

                            break;

                        case FhirServerResourceInfo.FhirInteraction.SearchType:

                            if ((_searchSupport == SearchSupportCodes.Both) || (_searchSupport == SearchSupportCodes.Get))
                            {
                                AddPath(
                                    opsByMethodByPath,
                                    $"/{resource.ResourceType}",
                                    OperationType.Get,
                                    $"SearchGet{resource.ResourceType}",
                                    $"Search {resource.ResourceType}s",
                                    $"Search across all {resource.ResourceType} instances",
                                    false,
                                    false,
                                    new int[] { 200, 400, 401, 405, },
                                    resource.ResourceType,
                                    false,
                                    true,
                                    resource.SearchParameters.Values,
                                    null);
                            }

                            if ((_searchSupport == SearchSupportCodes.Both) || (_searchSupport == SearchSupportCodes.Post))
                            {
                                AddPath(
                                    opsByMethodByPath,
                                    $"/{resource.ResourceType}/_search",
                                    OperationType.Post,
                                    $"SearchPost{resource.ResourceType}",
                                    $"Search {resource.ResourceType}s",
                                    $"Search across all {resource.ResourceType} instances",
                                    false,
                                    false,
                                    new int[] { 200, 400, 401, 405, },
                                    resource.ResourceType,
                                    false,
                                    true,
                                    resource.SearchParameters.Values,
                                    new OpenApiRequestBody()
                                    {
                                        Content = new Dictionary<string, OpenApiMediaType>()
                                            {
                                                { "application/x-www-form-urlencoded", new OpenApiMediaType() },
                                            },
                                        Description = _includeDescriptions ? "Search parameters" : null,
                                    });
                            }

                            break;
                    }

                    // check for a server only listing their operations at the end
                    //if (!resource.Operations.Any())
                    //{
                    //}

                    // TODO(ginoc): Operations in the CS do not indicate if they are instance or type level...
                    //  Need to see if we can figure out the source of the operation (based on definition) and
                    //  use that to create the definition.  Need path location and parameters.
                    //foreach (FhirServerOperation serverOp in resource.Operations.Values)
                    //{
                    //    if (string.IsNullOrEmpty(serverOp.DefinitionCanonical))
                    //    {
                    //        continue;
                    //    }

                    //    if ((!_info.TryGetArtifact(
                    //            serverOp.DefinitionCanonical,
                    //            out object artifact,
                    //            out FhirArtifactClassEnum artifactClass,
                    //            out string resolvedPackage)) &&
                    //        (!FhirManager.Current.TryResolveCanonical(
                    //            _info.FhirSequence,
                    //            serverOp.DefinitionCanonical,
                    //            out artifactClass,
                    //            out artifact)))
                    //    {
                    //        continue;
                    //    }

                    //    FhirOperation fhirOp = (FhirOperation)artifact;

                    //    string path = $"/{resource.ResourceType}";
                    //    OperationType ot = OperationType.Post;

                    //    if (!opsByMethodByPath.ContainsKey(path))
                    //    {
                    //        opsByMethodByPath.Add(path, new());
                    //    }

                    //    if (opsByMethodByPath[path].ContainsKey(ot))
                    //    {
                    //        continue;
                    //    }

                    //    OpenApiOperation op = new();
                    //    AddOperationBasicProps(
                    //        op,
                    //        $"{FhirInteraction.Operation}{fhirOp}",
                    //        $"{FhirInteraction.Operation} a {resource.ResourceType} instance",
                    //        $"{FhirInteraction.Operation} a {resource.ResourceType} instance",
                    //        false,
                    //        false);

                    //    op.RequestBody = new OpenApiRequestBody()
                    //    {
                    //        Content = BuildContentMap(resource.ResourceType, false),
                    //        Description = _includeDescriptions ? $"A {resource.ResourceType}" : null,
                    //    };

                    //    if (_singleResponseCode)
                    //    {
                    //        op.Responses = new OpenApiResponses()
                    //        {
                    //            ["200"] = new OpenApiResponse()
                    //            {
                    //                Description = "OK",
                    //                Content = BuildContentMap(resource.ResourceType, false),
                    //            },
                    //        };
                    //    }
                    //    else
                    //    {
                    //        op.Responses = new OpenApiResponses()
                    //        {
                    //            ["200"] = new OpenApiResponse()
                    //            {
                    //                Description = "OK",
                    //                Content = BuildContentMap(resource.ResourceType, false),
                    //            },
                    //            ["201"] = new OpenApiResponse()
                    //            {
                    //                Description = "CREATED",
                    //                Content = BuildContentMap(resource.ResourceType, false),
                    //            },
                    //            ["400"] = new OpenApiResponse()
                    //            {
                    //                Description = "BAD REQUEST",
                    //            },
                    //            ["401"] = new OpenApiResponse()
                    //            {
                    //                Description = "NOT AUTHORIZED",
                    //            },
                    //            ["404"] = new OpenApiResponse()
                    //            {
                    //                Description = "NOT FOUND",
                    //            },
                    //            ["405"] = new OpenApiResponse()
                    //            {
                    //                Description = "METHOD NOT ALLOWED",
                    //            },
                    //            ["412"] = new OpenApiResponse()
                    //            {
                    //                Description = "CONFLICT",
                    //            },
                    //            ["422"] = new OpenApiResponse()
                    //            {
                    //                Description = "UNPROCESSABLE",
                    //            },
                    //        };
                    //    }

                    //    opsByMethodByPath[path].Add(ot, op);
                    //}
                }

                //foreach (FhirServerOperation serverOp in _serverInfo.ServerOperations.Values)
                //{
                //    if (string.IsNullOrEmpty(serverOp.DefinitionCanonical))
                //    {
                //        continue;
                //    }

                //    if ((!_info.TryGetArtifact(
                //            serverOp.DefinitionCanonical,
                //            out object artifact,
                //            out FhirArtifactClassEnum artifactClass,
                //            out string resolvedPackage)) &&
                //        (!FhirManager.Current.TryResolveCanonical(
                //            _info.FhirSequence,
                //            serverOp.DefinitionCanonical,
                //            out artifactClass,
                //            out artifact)))
                //    {
                //        continue;
                //    }

                //    if (!(artifact is FhirOperation fhirOp))
                //    {
                //        continue;
                //    }

                //    string path = string.Empty;
                //    OperationType ot = OperationType.Post;

                //    if (!opsByMethodByPath.ContainsKey(path))
                //    {
                //        opsByMethodByPath.Add(path, new());
                //    }

                //    if (opsByMethodByPath[path].ContainsKey(ot))
                //    {
                //        continue;
                //    }

                //    OpenApiOperation op = new();
                //    AddOperationBasicProps(
                //        op,
                //        $"{FhirInteraction.Operation}{fhirOp.Id}",
                //        $"{FhirInteraction.Operation} {fhirOp.Name}",
                //        $"{FhirInteraction.Operation} {fhirOp.Name}",
                //        false,
                //        false);

                //    string opInput;
                //    string opOutput;


                //    op.RequestBody = new OpenApiRequestBody()
                //    {
                //        Content = BuildContentMap("Parameters", false),
                //        Description = _includeDescriptions ? $"A Parameters" : null,
                //    };

                //    if (_singleResponseCode)
                //    {
                //        op.Responses = new OpenApiResponses()
                //        {
                //            ["200"] = new OpenApiResponse()
                //            {
                //                Description = "OK",
                //                Content = BuildContentMap(resource.ResourceType, false),
                //            },
                //        };
                //    }
                //    else
                //    {
                //        op.Responses = new OpenApiResponses()
                //        {
                //            ["200"] = new OpenApiResponse()
                //            {
                //                Description = "OK",
                //                Content = BuildContentMap(resource.ResourceType, false),
                //            },
                //            ["201"] = new OpenApiResponse()
                //            {
                //                Description = "CREATED",
                //                Content = BuildContentMap(resource.ResourceType, false),
                //            },
                //            ["400"] = new OpenApiResponse()
                //            {
                //                Description = "BAD REQUEST",
                //            },
                //            ["401"] = new OpenApiResponse()
                //            {
                //                Description = "NOT AUTHORIZED",
                //            },
                //            ["404"] = new OpenApiResponse()
                //            {
                //                Description = "NOT FOUND",
                //            },
                //            ["405"] = new OpenApiResponse()
                //            {
                //                Description = "METHOD NOT ALLOWED",
                //            },
                //            ["412"] = new OpenApiResponse()
                //            {
                //                Description = "CONFLICT",
                //            },
                //            ["422"] = new OpenApiResponse()
                //            {
                //                Description = "UNPROCESSABLE",
                //            },
                //        };
                //    }

                //    opsByMethodByPath[path].Add(ot, op);
                //}

                foreach ((string path, Dictionary<OperationType, OpenApiOperation> opsByType) in opsByMethodByPath)
                {
                    OpenApiPathItem pathItem = new()
                    {
                        Operations = opsByType,
                        Parameters = new List<OpenApiParameter>(),
                    };

                    pathItem.Parameters.Add(BuildReferencedParameter("_format"));
                    pathItem.Parameters.Add(BuildReferencedParameter("_pretty"));
                    pathItem.Parameters.Add(BuildReferencedParameter("_summary"));
                    pathItem.Parameters.Add(BuildReferencedParameter("_elements"));

                    paths.Add(path, pathItem);
                }
            }

            return paths;
        }

        /// <summary>Adds a path.</summary>
        /// <param name="opsByMethodByPath">        Full pathname of the ops by method by file.</param>
        /// <param name="path">                     Full pathname of the file.</param>
        /// <param name="ot">                       The ot.</param>
        /// <param name="operationId">              Identifier for the operation.</param>
        /// <param name="summary">                  The summary.</param>
        /// <param name="description">              The description.</param>
        /// <param name="includeIdParam">           True to include, false to exclude the identifier
        ///  parameter.</param>
        /// <param name="includeVidParam">          True to include, false to exclude the vid parameter.</param>
        /// <param name="responseCodes">            The response codes.</param>
        /// <param name="returnType">               Type of the return.</param>
        /// <param name="wrapReturnInBundle">       True to wrap return in bundle.</param>
        /// <param name="includeServerSearchParams">True to include, false to exclude the server search
        ///  parameters.</param>
        /// <param name="opSearchParams">           Options for controlling the operation search.</param>
        /// <param name="body">                     The body.</param>
        private void AddPath(
            Dictionary<string, Dictionary<OperationType, OpenApiOperation>> opsByMethodByPath,
            string path,
            OperationType ot,
            string operationId,
            string summary,
            string description,
            bool includeIdParam,
            bool includeVidParam,
            int[] responseCodes,
            string returnType,
            bool wrapReturnInBundle,
            bool includeServerSearchParams,
            IEnumerable<FhirServerSearchParam> opSearchParams,
            OpenApiRequestBody body)
        {
            if (!opsByMethodByPath.ContainsKey(path))
            {
                opsByMethodByPath.Add(path, new());
            }

            if (opsByMethodByPath[path].ContainsKey(ot))
            {
                return;
            }

            if ((responseCodes == null) || (responseCodes.Length == 0))
            {
                return;
            }

            OpenApiOperation op = new();
            AddOperationBasicProps(
                op,
                operationId,
                summary,
                description,
                includeIdParam,
                includeVidParam);

            if (body != null)
            {
                op.RequestBody = body;
            }

            if (includeServerSearchParams)
            {
                if ((ot != OperationType.Post) ||
                    (_searchParamLoc == SearchPostParameterLocationCodes.Both) ||
                    (_searchParamLoc == SearchPostParameterLocationCodes.Query))
                {
                    AddOperationParameters(_serverInfo.ServerSearchParameters.Values, op);
                }

                if ((ot == OperationType.Post) &&
                    ((_searchParamLoc == SearchPostParameterLocationCodes.Both) ||
                     (_searchParamLoc == SearchPostParameterLocationCodes.Body)))
                {
                    AddOperationParametersToBody(
                        op.RequestBody.Content["application/x-www-form-urlencoded"],
                        _serverInfo.ServerSearchParameters.Values);
                }
            }

            if (opSearchParams != null)
            {
                if ((ot != OperationType.Post) ||
                    (_searchParamLoc == SearchPostParameterLocationCodes.Both) ||
                    (_searchParamLoc == SearchPostParameterLocationCodes.Query))
                {
                    AddOperationParameters(opSearchParams, op);
                }

                if ((ot == OperationType.Post) &&
                    ((_searchParamLoc == SearchPostParameterLocationCodes.Both) ||
                     (_searchParamLoc == SearchPostParameterLocationCodes.Body)))
                {
                    AddOperationParametersToBody(
                        op.RequestBody.Content["application/x-www-form-urlencoded"],
                        opSearchParams);
                }
            }

            op.Responses = new OpenApiResponses();

            if (_singleResponseCode)
            {
                switch (responseCodes[0])
                {
                    case 200:
                    case 201:
                        op.Responses.Add(
                            responseCodes[0].ToString(),
                            new OpenApiResponse()
                            {
                                Description = _httpResponseDescriptions[responseCodes[0]],
                                Content = BuildContentMap(returnType, wrapReturnInBundle),
                            });
                        break;

                    default:
                        op.Responses.Add(
                            responseCodes[0].ToString(),
                            new OpenApiResponse()
                            {
                                Description = _httpResponseDescriptions[responseCodes[0]],
                            });
                        break;
                }
            }
            else
            {
                foreach (int code in responseCodes)
                {
                    switch (code)
                    {
                        case 200:
                        case 201:
                            op.Responses.Add(
                                code.ToString(),
                                new OpenApiResponse()
                                {
                                    Description = _httpResponseDescriptions[code],
                                    Content = BuildContentMap(returnType, wrapReturnInBundle),
                                });
                            break;

                        default:
                            op.Responses.Add(
                                code.ToString(),
                                new OpenApiResponse()
                                {
                                    Description = _httpResponseDescriptions[code],
                                });
                            break;
                    }
                }
            }

            opsByMethodByPath[path].Add(ot, op);
        }

        /// <summary>Adds requested operation parameters to a given OpenApiOperation.</summary>
        /// <param name="searchParameters">Options for controlling the search.</param>
        /// <param name="op">              The operation.</param>
        private void AddOperationParameters(
            IEnumerable<FhirServerSearchParam> searchParameters,
            OpenApiOperation op)
        {
            foreach (FhirServerSearchParam sp in searchParameters)
            {
                if (_commonParameters.ContainsKey(sp.Name))
                {
                    op.Parameters.Add(BuildReferencedParameter(sp.Name));
                    continue;
                }

                OpenApiParameter opParam = new OpenApiParameter()
                {
                    Name = sp.Name,
                    In = ParameterLocation.Query,
                    Description =
                            _includeDescriptions
                            ? SanitizeDescription(sp.Documentation)
                            : null,
                };

                switch (sp.ParameterType)
                {
                    case FhirServerSearchParam.SearchParameterType.Number:
                        opParam.Schema = new OpenApiSchema()
                        {
                            Type = "number",
                        };
                        break;

                    case FhirServerSearchParam.SearchParameterType.Date:
                    case FhirServerSearchParam.SearchParameterType.String:
                    case FhirServerSearchParam.SearchParameterType.Token:
                    case FhirServerSearchParam.SearchParameterType.Reference:
                    case FhirServerSearchParam.SearchParameterType.Composite:
                    case FhirServerSearchParam.SearchParameterType.Quantity:
                    case FhirServerSearchParam.SearchParameterType.Uri:
                    case FhirServerSearchParam.SearchParameterType.Special:
                    default:

                        opParam.Schema = new OpenApiSchema()
                        {
                            Type = "string",
                        };

                        break;
                }

                op.Parameters.Add(opParam);
            }
        }

        /// <summary>Adds an operation parameters to body to 'searchParameters'.</summary>
        /// <param name="mt">              The mt.</param>
        /// <param name="searchParameters">Options for controlling the search.</param>
        private void AddOperationParametersToBody(
            OpenApiMediaType mt,
            IEnumerable<FhirServerSearchParam> searchParameters)
        {
            mt.Schema = new OpenApiSchema();

            foreach (FhirServerSearchParam sp in searchParameters)
            {
                switch (sp.ParameterType)
                {
                    case FhirServerSearchParam.SearchParameterType.Number:
                        mt.Schema.Properties.Add(sp.Name, new OpenApiSchema()
                            {
                                Title = sp.Name,
                                Type = "number",
                            });
                        break;

                    case FhirServerSearchParam.SearchParameterType.Date:
                    case FhirServerSearchParam.SearchParameterType.String:
                    case FhirServerSearchParam.SearchParameterType.Token:
                    case FhirServerSearchParam.SearchParameterType.Reference:
                    case FhirServerSearchParam.SearchParameterType.Composite:
                    case FhirServerSearchParam.SearchParameterType.Quantity:
                    case FhirServerSearchParam.SearchParameterType.Uri:
                    case FhirServerSearchParam.SearchParameterType.Special:
                    default:
                        mt.Schema.Properties.Add(sp.Name, new OpenApiSchema()
                            {
                                Title = sp.Name,
                                Type = "string",
                            });

                        break;
                }
            }
        }

        /// <summary>Builds a required path parameter.</summary>
        /// <returns>An OpenApiParameter.</returns>
        private static OpenApiParameter BuildReferencedPathParameter(string name)
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

        /// <summary>Builds referenced parameter.</summary>
        /// <returns>An OpenApiParameter.</returns>
        private static OpenApiParameter BuildReferencedParameter(string name)
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

        /// <summary>Builds summary parameter.</summary>
        /// <returns>An OpenApiParameter.</returns>
        private OpenApiParameter BuildSummaryParameter()
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

        /// <summary>Builds string parameter.</summary>
        /// <param name="name">       The name.</param>
        /// <param name="description">The description.</param>
        /// <returns>An OpenApiParameter.</returns>
        private OpenApiParameter BuildStringParameter(string name, string description)
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

        /// <summary>Builds path identifier parameter.</summary>
        /// <returns>An OpenApiParameter.</returns>
        private static OpenApiParameter BuildPathIdParameter()
        {
            return new OpenApiParameter()
            {
                Name = "id",
                In = ParameterLocation.Path,
                Description = "Resource ID",
                //Required = true,
                Schema = new OpenApiSchema()
                {
                    Type = "string",
                },
            };
        }

        /// <summary>Builds path vid parameter.</summary>
        /// <returns>An OpenApiParameter.</returns>
        private static OpenApiParameter BuildPathVidParameter()
        {
            return new OpenApiParameter()
            {
                Name = "vid",
                In = ParameterLocation.Path,
                Description = "Resource Version ID",
                //Required = true,
                Schema = new OpenApiSchema()
                {
                    Type = "string",
                },
            };
        }

        /// <summary>Builds a content map.</summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="wrapInBundle">True to wrap in bundle.</param>
        /// <returns>A Dictionary of MIME Types and matching ApiOpenMeidaTypes.</returns>
        private Dictionary<string, OpenApiMediaType> BuildContentMap(
            string resourceName,
            bool wrapInBundle)
        {
            Dictionary<string, OpenApiMediaType> mediaTypes = new Dictionary<string, OpenApiMediaType>();

            if (_fhirJson)
            {
                if (string.IsNullOrEmpty(resourceName))
                {
                    mediaTypes.Add(
                        "application/fhir+json",
                        new OpenApiMediaType());
                }
                else
                {
                    OpenApiSchema schema = new OpenApiSchema();
                    int schemaElementCount = 1;

                    if (_inlineSchemas)
                    {
                        BuildInlineSchema(ref schema, ref schemaElementCount, resourceName, wrapInBundle);
                    }
                    else
                    {
                        if (wrapInBundle)
                        {
                            SetSchemaType("Bundle", ref schema);
                        }
                        else
                        {
                            SetSchemaType(resourceName, ref schema);
                        }
                    }

                    mediaTypes.Add(
                        "application/fhir+json",
                        new OpenApiMediaType()
                        {
                            Schema = schema,
                        });
                }
            }

            if (_fhirXml)
            {
                if (string.IsNullOrEmpty(resourceName))
                {
                    mediaTypes.Add(
                        "application/fhir+xml",
                        new OpenApiMediaType());
                }
                else
                {
                    OpenApiSchema schema = new OpenApiSchema();
                    int schemaElementCount = 1;

                    if (_inlineSchemas)
                    {
                        BuildInlineSchema(ref schema, ref schemaElementCount, resourceName, wrapInBundle);
                    }
                    else
                    {
                        if (wrapInBundle)
                        {
                            SetSchemaType("Bundle", ref schema);
                        }
                        else
                        {
                            SetSchemaType(resourceName, ref schema);
                        }
                    }

                    mediaTypes.Add(
                        "application/fhir+xml",
                        new OpenApiMediaType()
                        {
                            Schema = schema,
                        });
                }
            }

            return mediaTypes;
        }

        /// <summary>Builds content map for patch.</summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>A dictionary of OpenApiMediaType records, keyed by MIME type.</returns>
        private Dictionary<string, OpenApiMediaType> BuildContentMapForPatch(string resourceName)
        {
            Dictionary<string, OpenApiMediaType> mediaTypes = new Dictionary<string, OpenApiMediaType>();

            if (_patchJson)
            {
                if (string.IsNullOrEmpty(resourceName))
                {
                    mediaTypes.Add(
                        "application/json-patch+json",
                        new OpenApiMediaType());
                }
                else
                {
                    OpenApiSchema schema = new OpenApiSchema();
                    int schemaElementCount = 1;

                    if (_inlineSchemas)
                    {
                        BuildInlineSchema(ref schema, ref schemaElementCount, resourceName, false);
                    }
                    else
                    {
                        SetSchemaType(resourceName, ref schema);
                    }

                    mediaTypes.Add(
                        "application/json-patch+json",
                        new OpenApiMediaType()
                        {
                            Schema = schema,
                        });
                }
            }

            if (_patchXml)
            {
                if (string.IsNullOrEmpty(resourceName))
                {
                    mediaTypes.Add(
                        "application/xml-patch+xml",
                        new OpenApiMediaType());
                }
                else
                {
                    OpenApiSchema schema = new OpenApiSchema();
                    int schemaElementCount = 1;

                    if (_inlineSchemas)
                    {
                        BuildInlineSchema(ref schema, ref schemaElementCount, resourceName, false);
                    }
                    else
                    {
                        SetSchemaType(resourceName, ref schema);
                    }

                    mediaTypes.Add(
                        "application/xml-patch+xml",
                        new OpenApiMediaType()
                        {
                            Schema = schema,
                        });
                }
            }

            if (_patchFhirJson)
            {
                if (string.IsNullOrEmpty(resourceName))
                {
                    mediaTypes.Add(
                        "application/fhir+json",
                        new OpenApiMediaType());
                }
                else
                {
                    OpenApiSchema schema = new OpenApiSchema();
                    int schemaElementCount = 1;

                    if (_inlineSchemas)
                    {
                        BuildInlineSchema(ref schema, ref schemaElementCount, resourceName, false);
                    }
                    else
                    {
                        SetSchemaType(resourceName, ref schema);
                    }

                    mediaTypes.Add(
                        "application/fhir+json",
                        new OpenApiMediaType()
                        {
                            Schema = schema,
                        });
                }
            }

            if (_patchFhirXml)
            {
                if (string.IsNullOrEmpty(resourceName))
                {
                    mediaTypes.Add(
                        "application/fhir+xml",
                        new OpenApiMediaType());
                }
                else
                {
                    OpenApiSchema schema = new OpenApiSchema();
                    int schemaElementCount = 1;

                    if (_inlineSchemas)
                    {
                        BuildInlineSchema(ref schema, ref schemaElementCount, resourceName, false);
                    }
                    else
                    {
                        SetSchemaType(resourceName, ref schema);
                    }

                    mediaTypes.Add(
                        "application/fhir+xml",
                        new OpenApiMediaType()
                        {
                            Schema = schema,
                        });
                }
            }

            return mediaTypes;
        }

        /// <summary>Builds inline schema.</summary>
        /// <param name="schema">      [in,out] The schema.</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="wrapInBundle">True to wrap in bundle.</param>
        private void BuildInlineSchema(
            ref OpenApiSchema schema,
            ref int schemaElementCount,
            string resourceName,
            bool wrapInBundle)
        {
            if (!_info.Resources.ContainsKey(resourceName))
            {
                schema.Type = "object";
                schema.Description = _includeDescriptions ? "N/A." : null;
                return;
            }

            Dictionary<string, int> pathReferenceCounts = new Dictionary<string, int>();

            if (wrapInBundle)
            {
                BuildInlineSchemaRecurse(
                    ref schema,
                    ref schemaElementCount,
                    _info.Resources["Bundle"],
                    null,
                    resourceName,
                    pathReferenceCounts,
                    string.Empty,
                    0);
            }
            else
            {
                BuildInlineSchemaRecurse(
                    ref schema,
                    ref schemaElementCount,
                    _info.Resources[resourceName],
                    null,
                    resourceName,
                    pathReferenceCounts,
                    string.Empty,
                    0);
            }
        }

        /// <summary>Determine if we should add extension.</summary>
        /// <param name="path">      Full pathname of the file.</param>
        /// <param name="parentPath">Path to the parent element.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        private bool ShouldAddExtension(
            string path,
            string parentPath)
        {
            switch (_options.ExtensionSupport)
            {
                case ExporterOptions.ExtensionSupportLevel.None:
                    return false;

                case ExporterOptions.ExtensionSupportLevel.OfficialNonPrimitive:
                    if (_info.PrimitiveTypes.ContainsKey(path))
                    {
                        return false;
                    }

                    if (_info.ExtensionsByPath.ContainsKey(path) ||
                        _info.ExtensionsByPath.ContainsKey(parentPath))
                    {
                        return true;
                    }

                    break;

                case ExporterOptions.ExtensionSupportLevel.Official:
                    if (_info.ExtensionsByPath.ContainsKey(path) ||
                        _info.ExtensionsByPath.ContainsKey(parentPath))
                    {
                        return true;
                    }

                    break;

                case ExporterOptions.ExtensionSupportLevel.NonPrimitive:
                    if (_info.PrimitiveTypes.ContainsKey(path))
                    {
                        return false;
                    }

                    return true;

                case ExporterOptions.ExtensionSupportLevel.All:
                    return true;

                // TODO: add support for this option
                case ExporterOptions.ExtensionSupportLevel.ByExtensionUrl:
                    break;

                // TODO: add support for this option
                case ExporterOptions.ExtensionSupportLevel.ByElementPath:
                    break;
            }

            return false;
        }

        /// <summary>Builds inline schema.</summary>
        /// <param name="schema">             [in,out] The schema.</param>
        /// <param name="schemaElementCount"> [in,out] Number of schema elements.</param>
        /// <param name="complex">            The complex.</param>
        /// <param name="parent">             The parent.</param>
        /// <param name="rootResourceName">   Name of the root resource.</param>
        /// <param name="pathReferenceCounts">The path tracker.</param>
        /// <param name="parentPath">         (Optional) Path to the parent element.</param>
        private void BuildInlineSchemaRecurse(
            ref OpenApiSchema schema,
            ref int schemaElementCount,
            FhirComplex complex,
            FhirComplex parent,
            string rootResourceName,
            Dictionary<string, int> pathReferenceCounts,
            string parentPath = "",
            int currentDepth = 0)
        {
            schema.Type = "object";
            schema.Properties = new Dictionary<string, OpenApiSchema>();

            if (string.IsNullOrEmpty(schema.Description))
            {
                schema.Description =
                        _includeDescriptions
                        ? SanitizeDescription(complex.ShortDescription)
                        : null;
            }

            if (complex.Path == "Extension")
            {
                return;
            }

            if (pathReferenceCounts.ContainsKey(complex.Path))
            {
                return;
            }

            if ((_maxRecursions > 0) && (currentDepth > _maxRecursions))
            {
                return;
            }

            pathReferenceCounts.Add(complex.Path, 1);

            if (string.IsNullOrEmpty(complex.BaseTypeName))
            {
                schema.Type = "object";
            }
            else if (complex.Name.Equals(complex.BaseTypeName, StringComparison.Ordinal))
            {
                schema.Type = "object";
            }
            else
            {
                schema.Type = "object";
            }

            if (_info.Resources.ContainsKey(complex.Path))
            {
                if ((complex.Path == "Resource") ||
                    (complex.Path == "DomainResource"))
                {
                    Dictionary<string, int> subPaths = pathReferenceCounts.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                    BuildInlineSchemaRecurse(
                        ref schema,
                        ref schemaElementCount,
                        _info.Resources["Resource"],
                        complex,
                        rootResourceName,
                        subPaths,
                        string.Empty,
                        currentDepth + 1);
                }
                else
                {
                    schema.Properties.Add(
                        "resourceType",
                        new OpenApiSchema()
                        {
                            Type = "string",
                            Description = _includeDescriptions ? "Resource type name" : null,
                            Pattern = complex.Path,
                        });
                    schemaElementCount++;

                    schema.Required.Add("resourceType");
                }
            }

            if (complex.Elements != null)
            {
                foreach (FhirElement element in complex.Elements.Values.OrderBy(e => e.Name))
                {
                    if ((element.Name == "extension") &&
                        (!ShouldAddExtension(complex.Path, parentPath)))
                    {
                        continue;
                    }

                    if (SkipAsUncommon(element, parent))
                    {
                        continue;
                    }

                    Dictionary<string, int> subPaths = pathReferenceCounts.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                    BuildInlineElementSchema(
                        ref schema,
                        ref schemaElementCount,
                        complex,
                        element,
                        rootResourceName,
                        subPaths,
                        currentDepth);
                }
            }
        }

        /// <summary>Skip as uncommon.</summary>
        /// <param name="element">The element.</param>
        /// <param name="parent"> The parent.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        private bool SkipAsUncommon(
            FhirElement element,
            FhirComplex parent)
        {
            if (!_removeUncommonFields)
            {
                return false;
            }

            if (_uncommonFields.Contains(element.Path))
            {
                return true;
            }

            if ((parent != null) &&
                _uncommonFields.Contains($"{parent.Name}.{element.Name}"))
            {
                return true;
            }

            return false;
        }

        /// <summary>Builds element schema.</summary>
        /// <param name="schema">             [in,out] The parent schema.</param>
        /// <param name="schemaElementCount"> [in,out] Number of schema elements.</param>
        /// <param name="complex">            The complex.</param>
        /// <param name="element">            The element.</param>
        /// <param name="rootResourceName">   Name of the root resource.</param>
        /// <param name="pathReferenceCounts">The path tracker.</param>
        private void BuildInlineElementSchema(
            ref OpenApiSchema schema,
            ref int schemaElementCount,
            FhirComplex complex,
            FhirElement element,
            string rootResourceName,
            Dictionary<string, int> pathReferenceCounts,
            int currentDepth = 0)
        {
            string name = GetElementName(element);

            if (schema.Properties.ContainsKey(name))
            {
                return;
            }

            if (element.CardinalityMax == 0)
            {
                return;
            }

            OpenApiSchema elementSchema = new OpenApiSchema()
            {
                Description =
                    _includeDescriptions
                    ? SanitizeDescription(element.ShortDescription)
                    : null,
            };

            if (element.Path == "Reference.identifier")
            {
                elementSchema.Type = "object";
                schema.Properties.Add(
                    name,
                    elementSchema);
                schemaElementCount++;
                return;
            }

            if (_removeUncommonFields &&
                (element.Path == "Bundle.entry.request") &&
                (!string.IsNullOrEmpty(rootResourceName)))
            {
                elementSchema.Type = "object";
                schema.Properties.Add(
                    name,
                    elementSchema);
                schemaElementCount++;
                return;
            }

            if ((element.ElementTypes != null) &&
                (element.ElementTypes.Count > 1))
            {
                foreach (FhirElementType elementType in element.ElementTypes.Values)
                {
                    string pascal = FhirUtils.SanitizedToConvention(elementType.Name, FhirTypeBase.NamingConvention.PascalCase);

                    OpenApiSchema subSchema = new OpenApiSchema()
                    {
                        Description =
                            _includeDescriptions
                            ? SanitizeDescription(element.ShortDescription)
                            : null,
                    };

                    if (_info.TryGetNodeInfo(elementType.Name, out FhirNodeInfo elementNodeInfo) &&
                        (elementNodeInfo.SourceType != FhirNodeInfo.FhirNodeType.Primitive))
                    {
                        BuildInlineSchemaRecurse(
                            ref subSchema,
                            ref schemaElementCount,
                            elementNodeInfo.GetSource<FhirComplex>(),
                            complex,
                            rootResourceName,
                            pathReferenceCounts,
                            element.Path,
                            currentDepth + 1);
                    }
                    else
                    {
                        SetSchemaType(elementType.Name, ref subSchema);
                    }

                    schema.Properties.Add(
                        $"{name}{pascal}",
                        subSchema);
                    schemaElementCount++;
                }

                return;
            }

            string type;

            if (!string.IsNullOrEmpty(element.BaseTypeName))
            {
                type = element.BaseTypeName;
            }
            else if (element.ElementTypes.Count == 1)
            {
                FhirElementType elementType = element.ElementTypes.First().Value;

                type = elementType.Name;

                if (_expandReferences && (elementType.TypeProfiles.Count == 1))
                {
                    string profileName = elementType.TypeProfiles.First().Value.Name;
                    if (_info.NodeByPath.ContainsKey(profileName))
                    {
                        type = profileName;
                    }
                }
            }
            else
            {
                type = "Element";
            }

            if (_info.TryGetNodeInfo(type, out FhirNodeInfo nodeInfo) &&
                (nodeInfo.SourceType != FhirNodeInfo.FhirNodeType.Primitive))
            {
                if ((element.Path == "Bundle.entry.resource") &&
                    (!string.IsNullOrEmpty(rootResourceName)))
                {
                    BuildInlineSchemaRecurse(
                        ref elementSchema,
                        ref schemaElementCount,
                        _info.Resources[rootResourceName],
                        complex,
                        rootResourceName,
                        pathReferenceCounts,
                        element.Path,
                        currentDepth + 1);
                }
                else
                {
                    BuildInlineSchemaRecurse(
                        ref elementSchema,
                        ref schemaElementCount,
                        nodeInfo.GetSource<FhirComplex>(),
                        complex,
                        rootResourceName,
                        pathReferenceCounts,
                        element.Path,
                        currentDepth + 1);
                }
            }
            else
            {
                SetSchemaType(type, ref elementSchema, element.IsArray);
            }

            schema.Properties.Add(
                name,
                elementSchema);
            schemaElementCount++;
        }

        /// <summary>Adds an operation basic properties.</summary>
        /// <param name="operation">      The operation.</param>
        /// <param name="operationId">    Identifier for the operation.</param>
        /// <param name="summary">        The summary.</param>
        /// <param name="description">    The description.</param>
        /// <param name="includeIdParam"> (Optional) True to include, false to exclude the identifier
        ///  parameter.</param>
        /// <param name="includeVidParam">(Optional) True to include, false to exclude the vid parameter.</param>
        private void AddOperationBasicProps(
            OpenApiOperation operation,
            string operationId,
            string summary,
            string description,
            bool includeIdParam = false,
            bool includeVidParam = false)
        {
            if (includeIdParam)
            {
                operation.Parameters.Add(BuildReferencedPathParameter("id"));
            }

            if (includeVidParam)
            {
                operation.Parameters.Add(BuildReferencedPathParameter("vid"));
            }

            //operation.Parameters.Add(BuildReferencedParameter("_format"));
            //operation.Parameters.Add(BuildReferencedParameter("_pretty"));
            //operation.Parameters.Add(BuildReferencedParameter("_summary"));
            //operation.Parameters.Add(BuildReferencedParameter("_elements"));

            operation.OperationId = operationId;

            if (_includeSummaries)
            {
                operation.Summary = summary;
            }

            if (_includeDescriptions)
            {
                operation.Description = description;
            }
        }

        /// <summary>Adds an accept header.</summary>
        /// <param name="op">[in,out] The operation.</param>
        private void AddAcceptHeader(ref OpenApiOperation op)
        {
            if (op.Parameters == null)
            {
                op.Parameters = new List<OpenApiParameter>();
            }

            if (_fhirJson)
            {
                op.Parameters.Add(new OpenApiParameter()
                {
                    Name = "Accept",
                    In = ParameterLocation.Header,
                });
            }
        }

        /// <summary>Builds metadata path operation.</summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>An OpenApiOperation.</returns>
        private OpenApiOperation BuildMetadataPathOperation(
            string resourceName)
        {
            OpenApiOperation operation = new OpenApiOperation()
            {
                OperationId = $"{_instanceOpPrefixes[OperationType.Get]}Metadata",
                Summary =
                    _includeSummaries
                    ? $"{_instanceOpPrefixes[OperationType.Get]} metadata information about this server."
                    : null,
                Description =
                    _includeDescriptions
                    ? $"{_instanceOpPrefixes[OperationType.Get]} metadata."
                    : null,
            };

            operation.Responses = new OpenApiResponses()
            {
                ["200"] = new OpenApiResponse()
                {
                    Description = "OK",
                    Content = BuildContentMap(resourceName, true),
                },
            };

            return operation;
        }

        /// <summary>Builds the list of known FHIR servers as OpenApi Server objects.</summary>
        /// <returns>A List of OpenApiServers.</returns>
        private List<OpenApiServer> BuildServers()
        {
            if (_serverInfo == null)
            {
                return null;
            }

            string description;

            if (!string.IsNullOrEmpty(_serverInfo.ImplementationDescription))
            {
                description = SanitizeDescription(_serverInfo.ImplementationDescription);
            }
            else if (!string.IsNullOrEmpty(_serverInfo.SoftwareName))
            {
                description = SanitizeDescription(_serverInfo.SoftwareName);
            }
            else
            {
                description = $"FHIR Server Version: {_info.VersionString}";
            }

            return new List<OpenApiServer>()
            {
                new OpenApiServer()
                {
                    Url = _serverInfo.Url,
                    Description = description,
                },
            };
        }

        /// <summary>Builds the OpenApi Info object.</summary>
        /// <returns>An OpenApiInfo.</returns>
        private OpenApiInfo BuildInfo()
        {
            string title;

            if (_parameters.ContainsKey("TITLE"))
            {
                title = _parameters["TITLE"];
            }
            else
            {
                title = $"FHIR {_info.ReleaseName}:{_info.VersionString}";
            }

            return new OpenApiInfo()
            {
                Version = "1.0.0",
                Title = title,
            };
        }

        /// <summary>Sanitize description.</summary>
        /// <param name="value">The value.</param>
        /// <returns>A string.</returns>
        private string SanitizeDescription(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return _descriptionValidation ? "N/A." : string.Empty;
            }

            if (!_descriptionValidation)
            {
                return value.Replace("\r", string.Empty).Replace("\n", string.Empty);
            }

            value = value.Replace("\r", string.Empty).Replace("\n", string.Empty);

            if (!value.EndsWith(".", StringComparison.Ordinal))
            {
                value += ".";
            }

            if (!char.IsUpper(value[0]))
            {
                if (value.Contains('|'))
                {
                    value = "Values: " + value.Replace(" | ", "|");
                }
                else
                {
                    value = value.Substring(0, 1).ToUpperInvariant() + value.Substring(1);
                }
            }

            if (value.Length > _descriptionMaxLen)
            {
                value = value.Substring(0, _descriptionMaxLen - 3) + "...";
            }

            return value;
        }
    }
}
