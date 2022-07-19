// <copyright file="LangOpenApi.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Health.Fhir.SpecManager.Manager;
using Microsoft.Health.Fhir.SpecManager.Models;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Newtonsoft.Json;

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

            document.Components.Parameters = new Dictionary<string, OpenApiParameter>()
            {
                ["id"] = BuildPathIdParameter(),
                ["_format"] = BuildFormatParameter(),
            };

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
                OpenApiJsonWriter writer = new OpenApiJsonWriter(sw);

                if (_openApiVersion == 2)
                {
                    document.Serialize(writer, OpenApiSpecVersion.OpenApi2_0);
                }
                else
                {
                    document.Serialize(writer, OpenApiSpecVersion.OpenApi3_0);
                }
            }

            if (minify)
            {
                object obj = JsonConvert.DeserializeObject(File.ReadAllText(filename));

                File.Delete(filename);

                File.WriteAllText(filename, JsonConvert.SerializeObject(obj, Formatting.None));
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

                OpenApiPathItem typePath = new OpenApiPathItem()
                {
                    Operations = new Dictionary<OperationType, OpenApiOperation>(),
                };

                OpenApiPathItem instancePath = new OpenApiPathItem()
                {
                    Operations = new Dictionary<OperationType, OpenApiOperation>(),
                };

                OpenApiPathItem historyTypePath = new OpenApiPathItem()
                {
                    Operations = new Dictionary<OperationType, OpenApiOperation>(),
                };

                OpenApiPathItem historyInstancePath = new OpenApiPathItem()
                {
                    Operations = new Dictionary<OperationType, OpenApiOperation>(),
                };

                foreach (FhirServerResourceInfo.FhirInteraction interaction in resource.Interactions)
                {
                    switch (interaction)
                    {
                        case FhirServerResourceInfo.FhirInteraction.Read:
                        case FhirServerResourceInfo.FhirInteraction.VRead:
                            {
                                if (instancePath.Operations.ContainsKey(OperationType.Get))
                                {
                                    continue;
                                }

                                OpenApiOperation op = BuildPathOperation(
                                    OperationType.Get,
                                    resource.ResourceType,
                                    true);

                                instancePath.Operations.Add(
                                    OperationType.Get,
                                    op);
                            }

                            break;

                        case FhirServerResourceInfo.FhirInteraction.HistoryInstance:
                            {
                                if (!_includeHistory)
                                {
                                    continue;
                                }

                                if (historyInstancePath.Operations.ContainsKey(OperationType.Get))
                                {
                                    continue;
                                }

                                OpenApiOperation op = BuildPathOperation(
                                    OperationType.Get,
                                    resource.ResourceType,
                                    true);

                                historyInstancePath.Operations.Add(
                                    OperationType.Get,
                                    op);
                            }

                            break;

                        case FhirServerResourceInfo.FhirInteraction.Patch:
                        case FhirServerResourceInfo.FhirInteraction.Update:

                            if (_generateReadOnly)
                            {
                                continue;
                            }

                            if (!instancePath.Operations.ContainsKey(OperationType.Put))
                            {
                                instancePath.Operations.Add(
                                    OperationType.Put,
                                    BuildPathOperation(OperationType.Put, resource.ResourceType, true));
                            }

                            break;

                        case FhirServerResourceInfo.FhirInteraction.Create:

                            if (_generateReadOnly)
                            {
                                continue;
                            }

                            if (!typePath.Operations.ContainsKey(OperationType.Put))
                            {
                                typePath.Operations.Add(
                                    OperationType.Put,
                                    BuildPathOperation(OperationType.Put, resource.ResourceType, false));
                            }

                            break;

                        case FhirServerResourceInfo.FhirInteraction.Delete:

                            if (_generateReadOnly)
                            {
                                continue;
                            }

                            if (!instancePath.Operations.ContainsKey(OperationType.Delete))
                            {
                                instancePath.Operations.Add(
                                    OperationType.Delete,
                                    BuildPathOperation(OperationType.Delete, resource.ResourceType, true));
                            }

                            break;

                        case FhirServerResourceInfo.FhirInteraction.SearchType:
                            {
                                if (typePath.Operations.ContainsKey(OperationType.Get))
                                {
                                    continue;
                                }

                                OpenApiOperation op = BuildPathOperation(
                                    OperationType.Get,
                                    resource.ResourceType,
                                    false);

                                AddOperationParameters(_serverInfo.ServerSearchParameters.Values, op);
                                AddOperationParameters(resource.SearchParameters.Values, op);

                                typePath.Operations.Add(
                                    OperationType.Get,
                                    op);
                            }

                            break;

                        case FhirServerResourceInfo.FhirInteraction.HistoryType:
                            {
                                if (!_includeHistory)
                                {
                                    continue;
                                }

                                if (historyTypePath.Operations.ContainsKey(OperationType.Get))
                                {
                                    continue;
                                }

                                OpenApiOperation op = BuildPathOperation(
                                    OperationType.Get,
                                    resource.ResourceType,
                                    false);

                                AddOperationParameters(_serverInfo.ServerSearchParameters.Values, op);
                                AddOperationParameters(resource.SearchParameters.Values, op);

                                historyTypePath.Operations.Add(
                                    OperationType.Get,
                                    op);
                            }

                            break;
                    }
                }

                if (typePath.Operations.Count > 0)
                {
                    paths.Add($"/{resource.ResourceType}", typePath);
                }

                if (historyTypePath.Operations.Count > 0)
                {
                    paths.Add($"/{resource.ResourceType}/_history", historyTypePath);
                }

                if (instancePath.Operations.Count > 0)
                {
                    paths.Add($"/{resource.ResourceType}/{{id}}", instancePath);
                }

                if (historyInstancePath.Operations.Count > 0)
                {
                    paths.Add($"/{resource.ResourceType}/{{id}}/_history", historyInstancePath);
                }
            }

            return paths;
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

        /// <summary>Builds path identifier parameter.</summary>
        /// <returns>An OpenApiParameter.</returns>
        private static OpenApiParameter BuildReferencedPathIdParameter()
        {
            return new OpenApiParameter()
            {
                Name = "id",
                Reference = new OpenApiReference()
                {
                    Id = "id",
                    Type = ReferenceType.Parameter,
                },
            };
        }

        /// <summary>Builds referenced format parameter.</summary>
        /// <returns>An OpenApiParameter.</returns>
        private static OpenApiParameter BuildReferencedFormatParameter()
        {
            return new OpenApiParameter()
            {
                Name = "_format",
                Reference = new OpenApiReference()
                {
                    Id = "_format",
                    Type = ReferenceType.Parameter,
                },
            };
        }

        /// <summary>Builds format parameter.</summary>
        /// <returns>An OpenApiParameter.</returns>
        private OpenApiParameter BuildFormatParameter()
        {
            OpenApiString value;

            if (_fhirJson || (!_fhirXml))
            {
                value = new OpenApiString("application/fhir+json");
            }
            else
            {
                value = new OpenApiString("application/fhir+xml");
            }

            return new OpenApiParameter()
            {
                Name = "_format",
                In = ParameterLocation.Query,
                Description = "Requested content type",
                Required = true,
                Schema = new OpenApiSchema()
                {
                    Type = "string",
                    Default = value,
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
                Required = true,
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

        /// <summary>Builds path operation for a resource.</summary>
        /// <param name="pathOpType">Type of the path operation.</param>
        /// <returns>An OpenApiOperation.</returns>
        private OpenApiOperation BuildPathOperation(
            OperationType pathOpType,
            string resourceName,
            bool isInstanceLevel)
        {
            OpenApiOperation operation = new OpenApiOperation();

            bool wrapInBundle;

            operation.Parameters.Add(BuildReferencedFormatParameter());

            if (isInstanceLevel)
            {
                operation.OperationId = $"{pathOpType}{resourceName}";
                operation.Parameters.Add(BuildReferencedPathIdParameter());
            }
            else
            {
                operation.OperationId = $"{pathOpType}{resourceName}s";
            }

            if (_includeSummaries)
            {
                if (isInstanceLevel)
                {
                    operation.Summary = $"Performs a {pathOpType} on a specific {resourceName}";
                }
                else
                {
                    operation.Summary = $"Performs a {pathOpType} operation at the {resourceName} type level.";
                }
            }

            if (_includeDescriptions)
            {
                if (isInstanceLevel)
                {
                    operation.Description = $"{_instanceOpPrefixes[pathOpType]} a {resourceName}";
                }
                else
                {
                    operation.Description = $"{_typeOpPrefixes[pathOpType]} {resourceName}s at the type level.";
                }
            }

            switch (pathOpType)
            {
                case OperationType.Get:
                    if (isInstanceLevel)
                    {
                        operation.OperationId = $"{_instanceOpPrefixes[pathOpType]}{resourceName}";
                        wrapInBundle = false;
                    }
                    else
                    {
                        operation.OperationId = $"{_typeOpPrefixes[pathOpType]}{resourceName}s";
                        wrapInBundle = true;
                    }

                    if (_singleResponseCode)
                    {
                        operation.Responses = new OpenApiResponses()
                        {
                            ["200"] = new OpenApiResponse()
                            {
                                Description = "OK",
                                Content = BuildContentMap(resourceName, wrapInBundle),
                            },
                        };
                    }
                    else
                    {
                        operation.Responses = new OpenApiResponses()
                        {
                            ["200"] = new OpenApiResponse()
                            {
                                Description = "OK",
                                Content = BuildContentMap(resourceName, wrapInBundle),
                            },
                            ["410"] = new OpenApiResponse()
                            {
                                Description = "DELETED",
                            },
                            ["404"] = new OpenApiResponse()
                            {
                                Description = "NOT FOUND",
                            },
                        };
                    }

                    break;

                case OperationType.Patch:
                case OperationType.Put:
                    if (isInstanceLevel)
                    {
                        operation.OperationId = $"{_instanceOpPrefixes[pathOpType]}{resourceName}";
                        wrapInBundle = false;
                    }
                    else
                    {
                        operation.OperationId = $"{_typeOpPrefixes[pathOpType]}{resourceName}s";
                        wrapInBundle = false;
                    }

                    if (_singleResponseCode)
                    {
                        operation.RequestBody = new OpenApiRequestBody()
                        {
                            Content = BuildContentMap(resourceName, wrapInBundle),
                            Description =
                                _includeDescriptions
                                ? $"A {resourceName}"
                                : null,
                        };

                        operation.Responses = new OpenApiResponses()
                        {
                            ["200"] = new OpenApiResponse()
                            {
                                Description = "OK",
                                Content = BuildContentMap(resourceName, isInstanceLevel),
                            },
                        };
                    }
                    else
                    {
                        operation.Responses = new OpenApiResponses()
                        {
                            ["200"] = new OpenApiResponse()
                            {
                                Description = "OK",
                                Content = BuildContentMap(resourceName, wrapInBundle),
                            },
                            ["201"] = new OpenApiResponse()
                            {
                                Description = "CREATED",
                                Content = BuildContentMap(resourceName, wrapInBundle),
                            },
                            ["400"] = new OpenApiResponse()
                            {
                                Description = "BAD REQUEST",
                            },
                            ["401"] = new OpenApiResponse()
                            {
                                Description = "NOT AUTHORIZED",
                            },
                            ["404"] = new OpenApiResponse()
                            {
                                Description = "NOT FOUND",
                            },
                            ["405"] = new OpenApiResponse()
                            {
                                Description = "METHOD NOT ALLOWED",
                            },
                            ["409"] = new OpenApiResponse()
                            {
                                Description = "CONFLICT",
                            },
                            ["412"] = new OpenApiResponse()
                            {
                                Description = "CONFLICT",
                            },
                            ["422"] = new OpenApiResponse()
                            {
                                Description = "UNPROCESSABLE",
                            },
                        };
                    }

                    break;

                case OperationType.Post:
                    if (isInstanceLevel)
                    {
                        operation.OperationId = $"{_instanceOpPrefixes[pathOpType]}{resourceName}";
                        wrapInBundle = false;
                    }
                    else
                    {
                        operation.OperationId = $"{_typeOpPrefixes[pathOpType]}{resourceName}s";
                        wrapInBundle = false;
                    }

                    if (_singleResponseCode)
                    {
                        operation.RequestBody = new OpenApiRequestBody()
                        {
                            Content = BuildContentMap(resourceName, wrapInBundle),
                            Description =
                                _includeDescriptions
                                ? $"A {resourceName}"
                                : null,
                        };

                        operation.Responses = new OpenApiResponses()
                        {
                            ["200"] = new OpenApiResponse()
                            {
                                Description = "OK",
                                Content = BuildContentMap(resourceName, wrapInBundle),
                            },
                        };
                    }
                    else
                    {
                        operation.Responses = new OpenApiResponses()
                        {
                            ["200"] = new OpenApiResponse()
                            {
                                Description = "OK",
                                Content = BuildContentMap(resourceName, wrapInBundle),
                            },
                            ["201"] = new OpenApiResponse()
                            {
                                Description = "CREATED",
                                Content = BuildContentMap(resourceName, wrapInBundle),
                            },
                            ["400"] = new OpenApiResponse()
                            {
                                Description = "BAD REQUEST",
                            },
                            ["401"] = new OpenApiResponse()
                            {
                                Description = "NOT AUTHORIZED",
                            },
                            ["404"] = new OpenApiResponse()
                            {
                                Description = "NOT FOUND",
                            },
                            ["412"] = new OpenApiResponse()
                            {
                                Description = "CONFLICT",
                            },
                            ["422"] = new OpenApiResponse()
                            {
                                Description = "UNPROCESSABLE",
                            },
                        };
                    }

                    break;

                case OperationType.Delete:
                    if (isInstanceLevel)
                    {
                        operation.OperationId = $"{_instanceOpPrefixes[pathOpType]}{resourceName}";
                    }
                    else
                    {
                        operation.OperationId = $"{_typeOpPrefixes[pathOpType]}{resourceName}s";
                    }

                    operation.Responses = new OpenApiResponses()
                    {
                        ["204"] = new OpenApiResponse()
                        {
                            Description = "NO CONTENT",
                        },
                    };

                    break;

                case OperationType.Options:
                    break;
                case OperationType.Head:
                    break;
                case OperationType.Trace:
                    break;
                default:
                    break;
            }

            if ((operation.Responses == null) ||
                (operation.Responses.Count == 0))
            {
                return null;
            }

            return operation;
        }

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
