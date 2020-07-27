// <copyright file="LangOpenApi.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Health.Fhir.SpecManager.fhir.r5;
using Microsoft.Health.Fhir.SpecManager.Manager;
using Microsoft.Health.Fhir.SpecManager.Models;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.Health.Fhir.SpecManager.Language
{
    /// <summary>An OpenApi language exporter.</summary>
    public sealed class LangOpenApi : ILanguage
    {
        /// <summary>FHIR information we are exporting.</summary>
        private FhirVersionInfo _info;

        /// <summary>Options for controlling the export.</summary>
        private ExporterOptions _options;

        /// <summary>List of types of the exported resource names and types.</summary>
        private Dictionary<string, string> _exportedResourceNamesAndTypes = new Dictionary<string, string>();

        /// <summary>The exported codes.</summary>
        private HashSet<string> _exportedCodes = new HashSet<string>();

        /// <summary>Name of the language.</summary>
        private const string _languageName = "OpenApi";

        /// <summary>The single file export extension.</summary>
        private const string _singleFileExportExtension = ".json";

        /// <summary>Options for controlling the operation.</summary>
        private Dictionary<string, string> _parameters = new Dictionary<string, string>();

        /// <summary>True to explicit FHIR JSON.</summary>
        private bool _explicitFhirJson = false;

        /// <summary>True to explicit FHIR XML.</summary>
        private bool _explicitFhirXml = false;

        /// <summary>True to single response code.</summary>
        private bool _singleResponseCode = false;

        /// <summary>True to include, false to exclude the summaries.</summary>
        private bool _includeSummaries = true;

        /// <summary>True to include, false to exclude the schemas.</summary>
        private bool _includeSchemas = true;

        /// <summary>True to include, false to exclude the schema descriptions.</summary>
        private bool _includeSchemaDescriptions = true;

        /// <summary>True to expand references based on allowed profiles.</summary>
        private bool _expandProfiles = true;

        /// <summary>True to generate read only.</summary>
        private bool _generateReadOnly = false;

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
        };

        /// <summary>Gets language-specific options and their descriptions.</summary>
        Dictionary<string, string> ILanguage.LanguageOptions => new Dictionary<string, string>()
        {
            { "Title", "Title to use in the Info section." },
            { "OpenApiVersion", "Open API version to use (2, 3)." },
            { "ExplicitFhirJson", "If paths should explicitly support FHIR+JSON (true|false)." },
            { "ExplicitFhirXml", "If paths should explicitly support FHIR+XML (true|false)." },
            { "Responses", "Response inclusion style (single|multiple)." },
            { "Summaries", "If responses should include summaries (true|false)." },
            { "Schemas", "If schemas should be included (true|false)" },
            { "SchemaDescriptions", "If schemas should include descriptions (true|false)" },
            { "ExpandProfiles", "If types should expand based on allowed profiles (true|false)" },
            { "ReadOnly", "If the output should only contain GET operations (false|true)" },
        };

        /// <summary>Export the passed FHIR version into the specified directory.</summary>
        /// <param name="info">           The information.</param>
        /// <param name="options">        Options for controlling the operation.</param>
        /// <param name="exportDirectory">Directory to write files.</param>
        void ILanguage.Export(
            FhirVersionInfo info,
            ExporterOptions options,
            string exportDirectory)
        {
            // set internal vars so we don't pass them to every function
            // this is ugly, but the interface patterns get bad quickly because we need the type map to copy the FHIR info
            _info = info;
            _options = options;

            if (_options.LanguageOptions != null)
            {
                foreach (KeyValuePair<string, string> kvp in _options.LanguageOptions)
                {
                    _parameters.Add(kvp.Key.ToUpperInvariant(), kvp.Value);
                }
            }

            _exportedResourceNamesAndTypes = new Dictionary<string, string>();
            _exportedCodes = new HashSet<string>();

            int openApiVersion = 3;

            if (_parameters.ContainsKey("OPENAPIVERSION"))
            {
                if ((_parameters["OPENAPIVERSION"] == "2") ||
                    (_parameters["OPENAPIVERSION"] == "2.0"))
                {
                    openApiVersion = 2;
                }
            }

            if (_parameters.ContainsKey("EXPLICITFHIRJSON") &&
                (!string.IsNullOrEmpty(_parameters["EXPLICITFHIRJSON"])) &&
                _parameters["EXPLICITFHIRJSON"].StartsWith("T", StringComparison.OrdinalIgnoreCase))
            {
                _explicitFhirJson = true;
            }

            if (_parameters.ContainsKey("EXPLICITFHIRXML") &&
                (!string.IsNullOrEmpty(_parameters["EXPLICITFHIRXML"])) &&
                _parameters["EXPLICITFHIRXML"].StartsWith("T", StringComparison.OrdinalIgnoreCase))
            {
                _explicitFhirXml = true;
            }

            if (_parameters.ContainsKey("RESPONSES") &&
                (!string.IsNullOrEmpty(_parameters["RESPONSES"])) &&
                _parameters["RESPONSES"].StartsWith("S", StringComparison.OrdinalIgnoreCase))
            {
                _singleResponseCode = true;
            }

            if (_parameters.ContainsKey("SUMMARIES") &&
                (!string.IsNullOrEmpty(_parameters["SUMMARIES"])) &&
                _parameters["SUMMARIES"].StartsWith("F", StringComparison.OrdinalIgnoreCase))
            {
                _includeSummaries = false;
            }

            if (_parameters.ContainsKey("SCHEMAS") &&
                (!string.IsNullOrEmpty(_parameters["SCHEMAS"])) &&
                _parameters["SCHEMAS"].StartsWith("F", StringComparison.OrdinalIgnoreCase))
            {
                _includeSchemas = false;
            }

            if (_parameters.ContainsKey("SCHEMADESCRIPTIONS") &&
                (!string.IsNullOrEmpty(_parameters["SCHEMADESCRIPTIONS"])) &&
                _parameters["SCHEMADESCRIPTIONS"].StartsWith("F", StringComparison.OrdinalIgnoreCase))
            {
                _includeSchemaDescriptions = false;
            }

            if (_parameters.ContainsKey("EXPANDPROFILES") &&
                (!string.IsNullOrEmpty(_parameters["EXPANDPROFILES"])) &&
                _parameters["EXPANDPROFILES"].StartsWith("F", StringComparison.OrdinalIgnoreCase))
            {
                _expandProfiles = false;
            }

            if (_parameters.ContainsKey("READONLY") &&
                (!string.IsNullOrEmpty(_parameters["READONLY"])) &&
                _parameters["READONLY"].StartsWith("T", StringComparison.OrdinalIgnoreCase))
            {
                _generateReadOnly = true;
            }

            OpenApiDocument document = new OpenApiDocument();

            document.Info = BuildInfo();

            document.Components = new OpenApiComponents();

            document.Components.Parameters = new Dictionary<string, OpenApiParameter>()
            {
                ["id"] = BuildPathIdParameter(),
            };

            if (_includeSchemas)
            {
                document.Components.Schemas = BuildSchemas();
            }

            if (!string.IsNullOrEmpty(_options.ServerUrl))
            {
                document.Servers = BuildServers();

                document.Paths = BuildPathsForServer();
            }

            // create a filename for writing (single file for now)
            string filename = Path.Combine(exportDirectory, $"OpenApi_R{info.MajorVersion}_v{openApiVersion}.json");

            using (FileStream stream = new FileStream(filename, FileMode.Create))
            using (StreamWriter sw = new StreamWriter(stream))
            {
                OpenApiJsonWriter writer = new OpenApiJsonWriter(sw);

                if (openApiVersion == 2)
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
        /// <returns>A Dictionary&lt;string,OpenApiSchema&gt;</returns>
        private Dictionary<string, OpenApiSchema> BuildSchemas()
        {
            Dictionary<string, OpenApiSchema> schemas = new Dictionary<string, OpenApiSchema>();

            foreach (FhirComplex complex in _info.ComplexTypes.Values.OrderBy(c => c.Name))
            {
                schemas.Add(complex.Name, BuildSchema(complex));
            }

            foreach (FhirComplex complex in _info.Resources.Values.OrderBy(c => c.Name))
            {
                schemas.Add(complex.Name, BuildSchema(complex));
            }

            return schemas;
        }

        /// <summary>Builds a schema.</summary>
        /// <param name="complex">The complex.</param>
        /// <returns>An OpenApiSchema.</returns>
        private OpenApiSchema BuildSchema(
            FhirComplex complex,
            FhirComplex root = null)
        {
            OpenApiSchema schema = new OpenApiSchema()
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>(),
            };

            if (_includeSchemaDescriptions)
            {
                schema.Description = complex.ShortDescription;
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
                        schema.Properties.Add(
                            GetElementName(element), // GetComponentName(component),
                            BuildSchema(complex.Components[element.Path], root));
                    }
                    else
                    {
                        BuildElementSchema(ref schema, element);
                    }
                }
            }

            return schema;
        }

        /// <summary>Builds element schema.</summary>
        /// <param name="parentSchema">[in,out] The parent schema.</param>
        /// <param name="element">     The element.</param>
        private void BuildElementSchema(
            ref OpenApiSchema parentSchema,
            FhirElement element)
        {
            string name = GetElementName(element);
            OpenApiSchema schema = new OpenApiSchema();

            if (_includeSchemaDescriptions)
            {
                schema.Description = element.ShortDescription;
            }

            if ((element.ElementTypes != null) &&
                (element.ElementTypes.Count > 1))
            {
                foreach (FhirElementType elementType in element.ElementTypes.Values)
                {
                    string pascal = FhirUtils.SanitizedToConvention(elementType.Name, FhirTypeBase.NamingConvention.PascalCase);

                    OpenApiSchema subSchema = new OpenApiSchema();

                    if (_includeSchemaDescriptions)
                    {
                        subSchema.Description = element.ShortDescription;
                    }

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
                    schema.OneOf = new List<OpenApiSchema>();

                    if ((elementType.Profiles == null) ||
                        (elementType.Profiles.Count == 0))
                    {
                        foreach (FhirComplex resource in _info.Resources.Values)
                        {
                            OpenApiSchema subSchema = new OpenApiSchema();
                            SetSchemaType(resource.Name, ref subSchema);
                            schema.OneOf.Add(subSchema);
                        }
                    }
                    else
                    {
                        foreach (FhirElementProfile profile in elementType.Profiles.Values)
                        {
                            OpenApiSchema subSchema = new OpenApiSchema();
                            SetSchemaType(profile.Name, ref subSchema);
                            schema.OneOf.Add(subSchema);
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

            SetSchemaType(type, ref schema);

            parentSchema.Properties.Add(
                GetElementName(element),
                schema);
        }

        /// <summary>Sets a type.</summary>
        /// <param name="baseType">Type of the base.</param>
        /// <param name="schema">  [in,out] The schema.</param>
        private static void SetSchemaType(
            string baseType,
            ref OpenApiSchema schema)
        {
            if (_primitiveTypeMap.ContainsKey(baseType))
            {
                string type = _primitiveTypeMap[baseType];

                if (type.Contains(':'))
                {
                    string[] parts = type.Split(':');

                    schema.Type = parts[0];
                    schema.Format = parts[1];
                }
                else
                {
                    schema.Type = type;
                }

                return;
            }

            schema.Reference = new OpenApiReference()
            {
                Id = BuildTypeFromPath(baseType),
                Type = ReferenceType.Schema,
            };
        }

        /// <summary>Builds type from path.</summary>
        /// <param name="path">Full pathname of the file.</param>
        /// <returns>A string.</returns>
        private static string BuildTypeFromPath(string path)
        {
            StringBuilder sb = new StringBuilder();
            string[] components = path.Split('.');

            for (int i = 0; i < components.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        sb.Append(components[i]);
                        break;

                    default:
                        sb.Append("/properties/");
                        sb.Append(components[i]);
                        break;
                }
            }

            return sb.ToString();
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

        /// <summary>Gets component name.</summary>
        /// <param name="component">The component.</param>
        /// <returns>The component name.</returns>
        private static string GetComponentName(FhirComplex component)
        {
            string name;

            if (string.IsNullOrEmpty(component.ExplicitName))
            {
                name = FhirUtils.ToConvention(
                    component.Name,
                    component.Path,
                    FhirTypeBase.NamingConvention.PascalCase);
            }
            else
            {
                name = FhirUtils.SanitizedToConvention(
                    component.ExplicitName,
                    FhirTypeBase.NamingConvention.PascalCase);
            }

            name += "Component";

            return name;
        }

        /// <summary>Builds the OpenAPI paths object based on a known server.</summary>
        /// <returns>The OpenApiPaths.</returns>
        private OpenApiPaths BuildPathsForServer()
        {
            if (_options.ServerInfo == null)
            {
                return null;
            }

            OpenApiPaths paths = new OpenApiPaths();

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

            foreach (FhirServerResourceInfo resource in _options.ServerInfo.ResourceInteractions.Values)
            {
                OpenApiPathItem typePath = new OpenApiPathItem()
                {
                    Operations = new Dictionary<OperationType, OpenApiOperation>(),
                };

                OpenApiPathItem instancePath = new OpenApiPathItem()
                {
                    Operations = new Dictionary<OperationType, OpenApiOperation>(),
                };

                foreach (FhirServerResourceInfo.FhirInteraction interaction in resource.Interactions)
                {
                    switch (interaction)
                    {
                        case FhirServerResourceInfo.FhirInteraction.Read:
                        case FhirServerResourceInfo.FhirInteraction.VRead:
                        case FhirServerResourceInfo.FhirInteraction.HistoryInstance:

                            if (!instancePath.Operations.ContainsKey(OperationType.Get))
                            {
                                instancePath.Operations.Add(
                                    OperationType.Get,
                                    BuildPathOperation(OperationType.Get, resource.ResourceType, true));
                            }

                            break;

                        case FhirServerResourceInfo.FhirInteraction.HistoryType:

                            if (!typePath.Operations.ContainsKey(OperationType.Get))
                            {
                                typePath.Operations.Add(
                                    OperationType.Get,
                                    BuildPathOperation(OperationType.Get, resource.ResourceType, false));
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

                            if (!typePath.Operations.ContainsKey(OperationType.Get))
                            {
                                typePath.Operations.Add(
                                    OperationType.Get,
                                    BuildPathOperation(OperationType.Get, resource.ResourceType, false));
                            }

                            break;
                    }
                }

                if (typePath.Operations.Count > 0)
                {
                    paths.Add($"/{resource.ResourceType}", typePath);
                }

                if (instancePath.Operations.Count > 0)
                {
                    instancePath.Parameters.Add(BuildReferencedPathIdParameter());

                    paths.Add($"/{resource.ResourceType}/{{id}}", instancePath);
                }
            }

            return paths;
        }

        /// <summary>Builds path identifier parameter.</summary>
        /// <returns>An OpenApiParameter.</returns>
        private static OpenApiParameter BuildReferencedPathIdParameter()
        {
            return new OpenApiParameter()
            {
                Reference = new OpenApiReference()
                {
                    Id = "id",
                    Type = ReferenceType.Parameter,
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
        /// <returns>A Dictionary of MIME Types and matching ApiOpenMeidaTypes.</returns>
        private Dictionary<string, OpenApiMediaType> BuildContentMap(
            string resourceName)
        {
            Dictionary<string, OpenApiMediaType> mediaTypes = new Dictionary<string, OpenApiMediaType>();

            if (_explicitFhirJson)
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

                    SetSchemaType(resourceName, ref schema);

                    mediaTypes.Add(
                        "application/fhir+json",
                        new OpenApiMediaType()
                        {
                            Schema = schema,
                        });
                }
            }

            if (_explicitFhirXml)
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

                    SetSchemaType(resourceName, ref schema);

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

        /// <summary>Builds path operation for a resource.</summary>
        /// <param name="pathOpType">Type of the path operation.</param>
        /// <returns>An OpenApiOperation.</returns>
        private OpenApiOperation BuildPathOperation(
            OperationType pathOpType,
            string resourceName,
            bool isInstanceLevel)
        {
            OpenApiOperation operation = new OpenApiOperation();

            string contentResourceName;

            if (isInstanceLevel)
            {
                operation.OperationId = $"{pathOpType}{resourceName}I";
            }
            else
            {
                operation.OperationId = $"{pathOpType}{resourceName}";
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

            switch (pathOpType)
            {
                case OperationType.Get:
                    if (isInstanceLevel)
                    {
                        operation.OperationId = $"get{resourceName}";
                        contentResourceName = resourceName;
                    }
                    else
                    {
                        operation.OperationId = $"list{resourceName}s";
                        contentResourceName = "Bundle";
                    }

                    if (_singleResponseCode)
                    {
                        operation.Responses = new OpenApiResponses()
                        {
                            ["200"] = new OpenApiResponse()
                            {
                                Description = "OK",
                                Content = BuildContentMap(contentResourceName),
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
                                Content = BuildContentMap(contentResourceName),
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
                        operation.OperationId = $"replace{resourceName}";
                        contentResourceName = resourceName;
                    }
                    else
                    {
                        operation.OperationId = $"replace{resourceName}s";
                        contentResourceName = resourceName;
                    }

                    if (_singleResponseCode)
                    {
                        operation.RequestBody = new OpenApiRequestBody()
                        {
                            Content = BuildContentMap(contentResourceName),
                        };

                        operation.Responses = new OpenApiResponses()
                        {
                            ["200"] = new OpenApiResponse()
                            {
                                Description = "OK",
                                Content = BuildContentMap(resourceName),
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
                                Content = BuildContentMap(contentResourceName),
                            },
                            ["201"] = new OpenApiResponse()
                            {
                                Description = "CREATED",
                                Content = BuildContentMap(contentResourceName),
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
                        operation.OperationId = $"create{resourceName}";
                        contentResourceName = resourceName;
                    }
                    else
                    {
                        operation.OperationId = $"create{resourceName}s";
                        contentResourceName = resourceName;
                    }

                    if (_singleResponseCode)
                    {
                        operation.RequestBody = new OpenApiRequestBody()
                        {
                            Content = BuildContentMap(contentResourceName),
                        };

                        operation.Responses = new OpenApiResponses()
                        {
                            ["200"] = new OpenApiResponse()
                            {
                                Description = "OK",
                                Content = BuildContentMap(contentResourceName),
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
                                Content = BuildContentMap(contentResourceName),
                            },
                            ["201"] = new OpenApiResponse()
                            {
                                Description = "CREATED",
                                Content = BuildContentMap(contentResourceName),
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
                        operation.OperationId = $"delete{resourceName}";
                    }
                    else
                    {
                        operation.OperationId = $"delete{resourceName}s";
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

        /// <summary>Builds metadata path operation.</summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>An OpenApiOperation.</returns>
        private OpenApiOperation BuildMetadataPathOperation(
            string resourceName)
        {
            OpenApiOperation operation = new OpenApiOperation();

            operation.OperationId = $"getMetadata";

            if (_includeSummaries)
            {
                operation.Summary = $"Gets metadata information about this server.";
            }

            operation.OperationId = $"getMetadata";

            operation.Responses = new OpenApiResponses()
            {
                ["200"] = new OpenApiResponse()
                {
                    Description = "OK",
                    Content = BuildContentMap(resourceName),
                },
            };

            return operation;
        }

        /// <summary>Builds the list of known FHIR servers as OpenApi Server objects.</summary>
        /// <returns>A List of OpenApiServers.</returns>
        private List<OpenApiServer> BuildServers()
        {
            if (_options.ServerInfo == null)
            {
                return null;
            }

            string description;

            if (!string.IsNullOrEmpty(_options.ServerInfo.ImplementationDescription))
            {
                description = _options.ServerInfo.ImplementationDescription;
            }
            else if (!string.IsNullOrEmpty(_options.ServerInfo.SoftwareName))
            {
                description = _options.ServerInfo.SoftwareName;
            }
            else
            {
                description = $"FHIR Server Version: {_info.VersionString}";
            }

            return new List<OpenApiServer>()
            {
                new OpenApiServer()
                {
                    Url = _options.ServerInfo.Url,
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
    }
}
