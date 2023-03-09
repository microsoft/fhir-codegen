// <copyright file="ModelBuilder.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.SpecManager.Manager;
using Microsoft.OpenApi.Models;
using static Microsoft.Health.Fhir.CodeGenCommon.Models.FhirCapResource;
using static Microsoft.Health.Fhir.SpecManager.Language.OpenApi.OpenApiCommon;

namespace Microsoft.Health.Fhir.SpecManager.Language.OpenApi;

/// <summary>An OpenAPI model builder - convert internal (normalized) models into OpenAPI-specific ones.</summary>
public class ModelBuilder
{
    /// <summary>The information.</summary>
    private IPackageExportable _info;

    /// <summary>Options for controlling the operation.</summary>
    private OpenApiOptions _openApiOptions;

    /// <summary>Options for controlling the exporter.</summary>
    private ExporterOptions _exporterOptions;

    /// <summary>True to use export key filter.</summary>
    private bool _useExportKeyFilter = false;

    /// <summary>The export filter keys.</summary>
    private HashSet<string> _exportFilterKeys = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>The capabilities.</summary>
    private FhirCapabiltyStatement _caps = null;

    private bool _fhirJson = false;
    private bool _fhirXml = false;
    private bool _fhirTurtle = false;

    private bool _patchJson = false;
    private bool _patchXml = false;
    private bool _patchFhirJson = false;
    private bool _patchFhirXml = false;

    private bool _searchGet = false;
    private bool _searchPost = false;

    private long _totalPaths = 0;
    private long _totalOperations = 0;
    private long _totalSchemas = 0;
    private long _totalQueryParameters = 0;
    private long _totalParamInstances = 0;

    /// <summary>Initializes a new instance of the <see cref="ModelBuilder"/> class.</summary>
    /// <param name="info">           The information.</param>
    /// <param name="openApiOptions"> Options for controlling the operation.</param>
    /// <param name="exporterOptions">Options for controlling the exporter.</param>
    /// <param name="caps">           (Optional) The capabilities.</param>
    public ModelBuilder(
        IPackageExportable info,
        OpenApiOptions openApiOptions,
        ExporterOptions exporterOptions,
        FhirCapabiltyStatement caps = null)
    {
        _info = info;
        _openApiOptions = openApiOptions;
        _exporterOptions = exporterOptions;

        if ((exporterOptions.ExportList != null) && exporterOptions.ExportList.Any())
        {
            _useExportKeyFilter = true;
            foreach (string key in exporterOptions.ExportList)
            {
                if (!_exportFilterKeys.Contains(key))
                {
                    _exportFilterKeys.Add(key);
                }
            }
        }

        _caps = caps;

        ConfigureMimeSupport();
        ConfigureSearchSupport();
    }

    /// <summary>Builds an OpenApiDocument, optionally filtered by the specified Capabilities.</summary>
    /// <param name="caps">(Optional) The capabilities.</param>
    /// <returns>An OpenApiDocument.</returns>
    public OpenApiDocument Build()
    {
        OpenApiDocument doc = new();

        doc.Info = BuildDocumentInfo();

        if (_caps != null)
        {
            doc.Servers = BuildServers();
        }

        doc.Components = new();

        doc.Components.Parameters = BuildCommonParameters();

        if (_openApiOptions.IncludeSearchParams)
        {
            if (_openApiOptions.ConsolidateSearchParams)
            {
                Dictionary<string, OpenApiParameter> consolidated = ConsolidateResourceParameters();

                foreach ((string key, OpenApiParameter oasParam) in consolidated)
                {
                    if (doc.Components.Parameters.ContainsKey(key))
                    {
                        continue;
                    }

                    doc.Components.Parameters.Add(key, oasParam);
                }
            }
        }

        Dictionary<string, OpenApiSchema> schemas = BuildSchemas();

        if (_openApiOptions.SchemaStyle != OaSchemaStyleCodes.Inline)
        {
            doc.Components.Schemas = schemas;
        }

        // grab initial counts
        _totalQueryParameters = doc.Components.Parameters.Count;
        _totalSchemas = schemas.Count;

        Dictionary<string, OpenApiTag> tags = new()
        {
            { "System", new OpenApiTag() { Name = "System", Description = "Server-level requests" } }
        };

        doc.Paths = BuildPaths(schemas, tags);

        doc.Tags = tags.Values.ToList();

        Console.WriteLine($"OpenAPI stats:");
        Console.WriteLine($"          totalPaths: {_totalPaths} ");
        Console.WriteLine($"     totalOperations: {_totalOperations} ");
        Console.WriteLine($"        totalSchemas: {_totalSchemas} ");
        Console.WriteLine($"totalQueryParameters: {_totalQueryParameters} ");
        Console.WriteLine($" totalParamInstances: {_totalParamInstances} ");
        Console.WriteLine($"");

        return doc;
    }

    /// <summary>Consolidate resource parameters.</summary>
    /// <returns>A Dictionary&lt;string,OpenApiParameter&gt;</returns>
    private Dictionary<string, OpenApiParameter> ConsolidateResourceParameters()
    {
        Dictionary<string, OpenApiParameter> parameters = new();

        IEnumerable<FhirComplex> resources = GetFilteredResouces();

        foreach (FhirComplex resource in resources)
        {
            IEnumerable<FhirSearchParam> fhirSps = GetResourceSearchParameters(resource.Name);

            foreach (FhirSearchParam fhirSp in fhirSps)
            {
                if (parameters.ContainsKey(fhirSp.Code))
                {
                    continue;
                }

                if (fhirSp.ValueType?.Equals("number", StringComparison.OrdinalIgnoreCase) ?? false)
                {
                    parameters.Add(fhirSp.Code, BuildNumberParameter(fhirSp.Code, fhirSp.Description, ParameterLocation.Query));
                }
                else
                {
                    parameters.Add(fhirSp.Code, BuildStringParameter(fhirSp.Code, fhirSp.Description, ParameterLocation.Query));
                }
            }
        }

        return parameters;
    }

    /// <summary>Builds common parameters.</summary>
    /// <returns>A Dictionary&lt;string,OpenApiParameter&gt;</returns>
    private Dictionary<string, OpenApiParameter> BuildCommonParameters()
    {
        Dictionary<string, OpenApiParameter> p = new();

        foreach (string code in _openApiOptions.HttpCommonParams)
        {
            if (_httpCommonParameters.ContainsKey(code))
            {
                p.Add(code, _httpCommonParameters[code]);
                continue;
            }

            p.Add(code, BuildStringParameter(code, string.Empty));
        }

        foreach (string code in _openApiOptions.HttpReadParams)
        {
            if (_httpReadParameters.ContainsKey(code))
            {
                p.Add(code, _httpReadParameters[code]);
                continue;
            }

            p.Add(code, BuildStringParameter(code, string.Empty));
        }

        if (_openApiOptions.IncludeSearchParams)
        {
            foreach (string code in _openApiOptions.SearchCommonParams)
            {
                if (_searchCommonParameters.ContainsKey(code))
                {
                    p.Add(code, _searchCommonParameters[code]);
                    continue;
                }

                p.Add(code, BuildStringParameter(code, string.Empty));
            }

            foreach (string code in _openApiOptions.SearchResultParams)
            {
                if (_searchResultParameters.ContainsKey(code))
                {
                    p.Add(code, _searchResultParameters[code]);
                    continue;
                }

                p.Add(code, BuildStringParameter(code, string.Empty));
            }
        }

        foreach ((string code, OpenApiParameter parameter) in _pathParameters)
        {
            p.Add(code, parameter);
        }

        if (_openApiOptions.IncludeHistory)
        {
            foreach ((string code, OpenApiParameter parameter) in _historyParameters)
            {
                p.Add(code, parameter);
            }
        }

        if (_openApiOptions.IncludeHeaders)
        {
            foreach ((string code, OpenApiParameter parameter) in _httpRequestHeaders)
            {
                p.Add(code, parameter);
            }
        }

        return p;
    }

    /// <summary>Configure search support.</summary>
    private void ConfigureSearchSupport()
    {
        _searchGet = (_openApiOptions.SearchSupport == OaHttpSupportCodes.Both) || (_openApiOptions.SearchSupport == OaHttpSupportCodes.Get);
        _searchPost = (_openApiOptions.SearchSupport == OaHttpSupportCodes.Both) || (_openApiOptions.SearchSupport == OaHttpSupportCodes.Post);
    }

    /// <summary>Configure mime support.</summary>
    private void ConfigureMimeSupport()
    {
        if ((_openApiOptions.FhirMime == OaFhirMimeCodes.FromCapabilities) &&
            (_caps != null))
        {
            _fhirJson = _caps.SupportsFhirJson();
            _fhirXml = _caps.SupportsFhirXml();
            _fhirTurtle = _caps.SupportsFhirTurtle();
        }
        else if (_openApiOptions.FhirMime == OaFhirMimeCodes.Common)
        {
            _fhirJson = true;
            _fhirXml = true;
            _fhirTurtle = false;
        }
        else if (_openApiOptions.FhirMime == OaFhirMimeCodes.All)
        {
            _fhirJson = true;
            _fhirXml = true;
            _fhirTurtle = true;
        }
        else
        {
            _fhirJson = _openApiOptions.FhirMime == OaFhirMimeCodes.FhirJson;
            _fhirXml = _openApiOptions.FhirMime == OaFhirMimeCodes.FhirXml;
            _fhirTurtle = _openApiOptions.FhirMime == OaFhirMimeCodes.FhirTurtle;
        }

        // ensure that *something* is supported


        if ((_openApiOptions.PatchMime == OaPatchMimeCodes.FromCapabilities) &&
            (_caps != null))
        {
            _patchJson = _caps.SupportsPatchJson();
            _patchXml = _caps.SupportsPatchXml();
            _patchFhirJson = _caps.SupportsPatchFhirJson();
            _patchFhirXml = _caps.SupportsPatchFhirXml();
        }
        else if (_openApiOptions.PatchMime == OaPatchMimeCodes.All)
        {
            _patchJson = true;
            _patchXml = true;
            _patchFhirJson = true;
            _patchFhirXml = true;
        }
        else if (_openApiOptions.PatchMime == OaPatchMimeCodes.FhirMime)
        {
            _patchFhirJson = _fhirJson;
            _patchFhirXml = _fhirXml;
        }
        else
        {
            _patchJson = _openApiOptions.PatchMime == OaPatchMimeCodes.Json;
            _patchXml = _openApiOptions.PatchMime == OaPatchMimeCodes.Xml;
        }
    }

    /// <summary>Resolve update create.</summary>
    /// <param name="resourceName">Name of the resource.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool ResolveUpdateCreate(string resourceName)
    {
        return _openApiOptions.UpdateCreate
            ?? _caps?.ResourceInteractions[resourceName]?.UpdateCreate
            ?? false;
    }

    /// <summary>Resolve conditional create.</summary>
    /// <param name="resourceName">Name of the resource.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool ResolveConditionalCreate(string resourceName)
    {
        return _openApiOptions.ConditionalCreate
            ?? _caps?.ResourceInteractions[resourceName]?.ConditionalCreate
            ?? false;
    }

    /// <summary>Resolve conditional read.</summary>
    /// <param name="resourceName">Name of the resource.</param>
    /// <returns>A ConditionalReadPolicy.</returns>
    private ConditionalReadPolicy ResolveConditionalRead(string resourceName)
    {
        return _openApiOptions.ConditionalRead
            ?? _caps?.ResourceInteractions[resourceName]?.ConditionalRead
            ?? ConditionalReadPolicy.NotSupported;
    }

    /// <summary>Resolve conditional update.</summary>
    /// <param name="resourceName">Name of the resource.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool ResolveConditionalUpdate(string resourceName)
    {
        return _openApiOptions.ConditionalUpdate
            ?? _caps?.ResourceInteractions[resourceName]?.ConditionalUpdate
            ?? false;
    }

    /// <summary>Resolve conditional patch.</summary>
    /// <param name="resourceName">Name of the resource.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool ResolveConditionalPatch(string resourceName)
    {
        return _openApiOptions.ConditionalPatch
            ?? _caps?.ResourceInteractions[resourceName]?.ConditionalPatch
            ?? false;
    }

    /// <summary>Resolve conditiaonl delete.</summary>
    /// <param name="resourceName">Name of the resource.</param>
    /// <returns>A ConditionalDeletePolicy.</returns>
    private ConditionalDeletePolicy ResolveConditiaonlDelete(string resourceName)
    {
        return _openApiOptions.ConditionalDelete
            ?? _caps?.ResourceInteractions[resourceName]?.ConditionalDelete
            ?? ConditionalDeletePolicy.NotSupported;
    }

    /// <summary>Builds the OpenAPI paths.</summary>
    /// <param name="schemas">        The schemas.</param>
    /// <param name="tags">           The tags.</param>
    /// <param name="consolidatedSps">The consolidated search parameter keys.</param>
    /// <returns>The OpenApiPaths.</returns>
    private OpenApiPaths BuildPaths(
        Dictionary<string, OpenApiSchema> schemas,
        Dictionary<string, OpenApiTag> tags)
    {
        OpenApiPaths paths = new();

        if (_openApiOptions.IncludeMetadata)
        {
            if (_info.FhirSequence == FhirPackageCommon.FhirSequenceEnum.DSTU2)
            {
                OpenApiPathItem metadataPath = new OpenApiPathItem()
                {
                    Operations = new Dictionary<OperationType, OpenApiOperation>(),
                };

                metadataPath.Operations.Add(
                    OperationType.Get,
                    BuildReadOperation("Conformance", schemas));

                paths.Add($"/metadata", metadataPath);

                _totalOperations++;
            }
            else
            {
                OpenApiPathItem metadataPath = new OpenApiPathItem()
                {
                    Operations = new Dictionary<OperationType, OpenApiOperation>(),
                };

                metadataPath.Operations.Add(
                    OperationType.Get,
                    BuildReadOperation("CapabilityStatement", schemas));

                paths.Add($"/metadata", metadataPath);

                _totalOperations++;
            }
        }

        // handle resource-specific things first (alphebetical)
        foreach (FhirComplex resource in GetFilteredResouces(true))
        {
            if (!tags.ContainsKey(resource.Name))
            {
                tags.Add(
                    resource.Name,
                    new()
                    {
                        Name = resource.Name,
                        Description = resource.ShortDescription,
                    });
            }

            // handle basic interactions
            foreach (FhirInteractionCodes fhirInteraction in GetInteractions(resource.Name))
            {
                string opPath;

                switch (fhirInteraction)
                {
                    case FhirInteractionCodes.Read:
                        {
                            opPath = $"/{resource.Name}/{{{PathComponentLogicalId}}}";
                            if (!paths.ContainsKey(opPath))
                            {
                                paths.Add(opPath, new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() });
                                AddCommonHttpParameters(paths[opPath]);
                            }

                            paths[opPath].AddOperation(OperationType.Get, BuildResourceReadOasOp(resource, schemas));
                            _totalOperations++;
                        }
                        break;

                    case FhirInteractionCodes.VRead:
                        {
                            opPath = $"/{resource.Name}/{{{PathComponentLogicalId}}}/_history/{{{PathComponentVersionId}}}";
                            if (!paths.ContainsKey(opPath))
                            {
                                paths.Add(opPath, new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() });
                                AddCommonHttpParameters(paths[opPath]);
                            }

                            paths[opPath].AddOperation(OperationType.Get, BuildResourceVReadOasOp(resource, schemas));
                            _totalOperations++;
                        }
                        break;

                    case FhirInteractionCodes.Update:
                        {
                            opPath = $"/{resource.Name}/{{{PathComponentLogicalId}}}";
                            if (!paths.ContainsKey(opPath))
                            {
                                paths.Add(opPath, new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() });
                                AddCommonHttpParameters(paths[opPath]);
                            }

                            paths[opPath].AddOperation(OperationType.Put, BuildResourceUpdateOasOp(resource, schemas));
                            _totalOperations++;
                        }
                        break;

                    case FhirInteractionCodes.Patch:
                        {
                            opPath = $"/{resource.Name}/{{{PathComponentLogicalId}}}";
                            if (!paths.ContainsKey(opPath))
                            {
                                paths.Add(opPath, new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() });
                                AddCommonHttpParameters(paths[opPath]);
                            }

                            paths[opPath].AddOperation(OperationType.Patch, BuildResourcePatchOasOp(resource, schemas));
                            _totalOperations++;
                        }
                        break;

                    case FhirInteractionCodes.Delete:
                        {
                            opPath = $"/{resource.Name}/{{{PathComponentLogicalId}}}";
                            if (!paths.ContainsKey(opPath))
                            {
                                paths.Add(opPath, new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() });
                                AddCommonHttpParameters(paths[opPath]);
                            }

                            paths[opPath].AddOperation(OperationType.Delete, BuildResourceDeleteOasOp(resource, schemas));
                            _totalOperations++;
                        }
                        break;

                    case FhirInteractionCodes.HistoryInstance:
                        {
                            if (!_openApiOptions.IncludeHistory)
                            {
                                continue;
                            }

                            opPath = $"/{resource.Name}/{{{PathComponentLogicalId}}}/_history";
                            if (!paths.ContainsKey(opPath))
                            {
                                paths.Add(opPath, new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() });
                                AddCommonHttpParameters(paths[opPath]);
                            }

                            paths[opPath].AddOperation(OperationType.Get, BuildResourceHistoryInstanceOasOp(resource, schemas));
                            _totalOperations++;
                        }
                        break;

                    case FhirInteractionCodes.HistoryType:
                        {
                            if (!_openApiOptions.IncludeHistory)
                            {
                                continue;
                            }

                            opPath = $"/{resource.Name}/_history";
                            if (!paths.ContainsKey(opPath))
                            {
                                paths.Add(opPath, new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() });
                                AddCommonHttpParameters(paths[opPath]);
                            }

                            paths[opPath].AddOperation(OperationType.Get, BuildResourceHistoryTypeOasOp(resource, schemas));
                            _totalOperations++;
                        }
                        break;

                    case FhirInteractionCodes.Create:
                        {
                            opPath = $"/{resource.Name}";
                            if (!paths.ContainsKey(opPath))
                            {
                                paths.Add(opPath, new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() });
                                AddCommonHttpParameters(paths[opPath]);
                            }

                            paths[opPath].AddOperation(OperationType.Post, BuildResourceCreateOasOp(resource, schemas));
                            _totalOperations++;
                        }
                        break;

                    case FhirInteractionCodes.SearchType:
                        {
                            if (_searchGet)
                            {
                                opPath = $"/{resource.Name}";
                                if (!paths.ContainsKey(opPath))
                                {
                                    paths.Add(opPath, new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() });
                                    AddCommonHttpParameters(paths[opPath]);
                                }

                                paths[opPath].AddOperation(OperationType.Get, BuildResourceSearchTypeGetOasOp(resource, schemas));
                                _totalOperations++;
                            }

                            if (_searchPost)
                            {
                                opPath = $"/{resource.Name}/_search";
                                if (!paths.ContainsKey(opPath))
                                {
                                    paths.Add(opPath, new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() });
                                    AddCommonHttpParameters(paths[opPath]);
                                }

                                paths[opPath].AddOperation(OperationType.Post, BuildResourceSearchTypePostOasOp(resource, schemas));
                                _totalOperations++;
                            }
                        }
                        break;

                    //case FhirInteraction.Search:
                    default:
                        break;

                }   // close: foreach interaction


            }       // close: foreach resource

            // traverse the operations for this resource
            foreach (FhirOperation fhirOp in GetResourceFhirOperations(resource.Name, FhirOperation.OperationKindCodes.Operation))
            {
                BuildResourceOperationOasPaths(paths, resource, schemas, fhirOp);
            }

            // TODO(ginoc): handle compartments
        }

        // handle system-level paths
        {
            string opPath;

            if (_searchGet)
            {
                opPath = "/";
                if (!paths.ContainsKey(opPath))
                {
                    paths.Add(opPath, new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() });
                    AddCommonHttpParameters(paths[opPath]);
                }

                paths[opPath].AddOperation(OperationType.Get, BuildSystemSearchGetOasOp(schemas));
                _totalOperations++;
            }

            if (_searchPost)
            {
                opPath = "/_search";
                if (!paths.ContainsKey(opPath))
                {
                    paths.Add(opPath, new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() });
                    AddCommonHttpParameters(paths[opPath]);
                }

                paths[opPath].AddOperation(OperationType.Post, BuildSystemSearchPostOasOp(schemas));
                _totalOperations++;
            }
        }

        // TODO(ginoc): need to check system conditional delete


        // handle system-level operations
        foreach (FhirOperation fhirOp in GetSystemFhirOperations(FhirOperation.OperationKindCodes.Operation))
        {
            BuildSystemOperationOasPaths(paths, schemas, fhirOp);
        }

        _totalPaths += paths.Count;

        return paths;
    }

    /// <summary>Builds system operation oas paths.</summary>
    /// <param name="paths">  The paths.</param>
    /// <param name="schemas">The schemas.</param>
    /// <param name="fhirOp"> The FHIR operation.</param>
    private void BuildSystemOperationOasPaths(
        OpenApiPaths paths,
        Dictionary<string, OpenApiSchema> schemas,
        FhirOperation fhirOp)
    {
        // query operations cannot be invoked directly, they are added to search
        if (fhirOp.Kind == FhirOperation.OperationKindCodes.Query)
        {
            return;
        }

        Dictionary<string, FhirComplex> resources = GetFilteredResouces().ToDictionary(c => c.Name);

        if ((fhirOp.DefinedOnInstance || fhirOp.DefinedOnType) &&
            (fhirOp.ResourceTypes?.Any() ?? false))
        {
            foreach (string resourceName in fhirOp.ResourceTypes)
            {
                if (!resources.ContainsKey(resourceName))
                {
                    continue;
                }

                FhirComplex resource = resources[resourceName];

                BuildResourceOperationOasPaths(
                    paths,
                    resource,
                    schemas,
                    fhirOp);
            }
        }

        if (fhirOp.DefinedOnSystem)
        {
            string opPath;

            if (TryBuildOperationGetOasOp(null, fhirOp, schemas, OaOpLevelCodes.Instance, out OpenApiOperation oasOpGet))
            {
                opPath = $"/${fhirOp.Code}";
                if (!paths.ContainsKey(opPath))
                {
                    paths.Add(opPath, new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() });
                    AddCommonHttpParameters(paths[opPath]);
                }

                // operations may get listed a few times, make sure we only add them once in each path
                if (!paths[opPath].Operations.ContainsKey(OperationType.Get))
                {
                    paths[opPath].AddOperation(OperationType.Get, oasOpGet);
                    _totalOperations++;
                }
            }

            if (TryBuildOperationPostOasOp(null, fhirOp, schemas, OaOpLevelCodes.Instance, out OpenApiOperation oasOpPost))
            {
                opPath = $"/${fhirOp.Code}";
                if (!paths.ContainsKey(opPath))
                {
                    paths.Add(opPath, new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() });
                    AddCommonHttpParameters(paths[opPath]);
                }

                // operations may get listed a few times, make sure we only add them once in each path
                if (!paths[opPath].Operations.ContainsKey(OperationType.Post))
                {
                    paths[opPath].AddOperation(OperationType.Post, oasOpPost);
                    _totalOperations++;
                }
            }
        }
    }

    /// <summary>Builds resource operation oas paths.</summary>
    /// <param name="paths">   The paths.</param>
    /// <param name="resource">The resource.</param>
    /// <param name="schemas"> The schemas.</param>
    /// <param name="fhirOp">  The FHIR operation.</param>
    /// <returns>An OpenApiOperation.</returns>
    private void BuildResourceOperationOasPaths(
        OpenApiPaths paths,
        FhirComplex resource,
        Dictionary<string, OpenApiSchema> schemas,
        FhirOperation fhirOp)
    {
        // query operations cannot be invoked directly, they are added to search
        if (fhirOp.Kind == FhirOperation.OperationKindCodes.Query)
        {
            return;
        }

        string opPath;

        if (fhirOp.DefinedOnInstance)
        {
            if (TryBuildOperationGetOasOp(resource, fhirOp, schemas, OaOpLevelCodes.Instance, out OpenApiOperation oasOpGet))
            {
                opPath = $"/{resource.Name}/{{{PathComponentLogicalId}}}/${fhirOp.Code}";
                if (!paths.ContainsKey(opPath))
                {
                    paths.Add(opPath, new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() });
                    AddCommonHttpParameters(paths[opPath]);
                }

                // operations may get listed a few times, make sure we only add them once in each path
                if (!paths[opPath].Operations.ContainsKey(OperationType.Get))
                {
                    paths[opPath].AddOperation(OperationType.Get, oasOpGet);
                    _totalOperations++;
                }
            }

            if (TryBuildOperationPostOasOp(resource, fhirOp, schemas, OaOpLevelCodes.Instance, out OpenApiOperation oasOpPost))
            {
                opPath = $"/{resource.Name}/{{{PathComponentLogicalId}}}/${fhirOp.Code}";
                if (!paths.ContainsKey(opPath))
                {
                    paths.Add(opPath, new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() });
                    AddCommonHttpParameters(paths[opPath]);
                }

                // operations may get listed a few times, make sure we only add them once in each path
                if (!paths[opPath].Operations.ContainsKey(OperationType.Post))
                {
                    paths[opPath].AddOperation(OperationType.Post, oasOpPost);
                    _totalOperations++;
                }
            }
        }

        if (fhirOp.DefinedOnType)
        {
            if (TryBuildOperationGetOasOp(resource, fhirOp, schemas, OaOpLevelCodes.Type, out OpenApiOperation oasOpGet))
            {
                opPath = $"/{resource.Name}/${fhirOp.Code}";
                if (!paths.ContainsKey(opPath))
                {
                    paths.Add(opPath, new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() });
                    AddCommonHttpParameters(paths[opPath]);
                }

                // operations may get listed a few times, make sure we only add them once in each path
                if (!paths[opPath].Operations.ContainsKey(OperationType.Get))
                {
                    paths[opPath].AddOperation(OperationType.Get, oasOpGet);
                    _totalOperations++;
                }
            }

            if (TryBuildOperationPostOasOp(resource, fhirOp, schemas, OaOpLevelCodes.Type, out OpenApiOperation oasOpPost))
            {
                opPath = $"/{resource.Name}/${fhirOp.Code}";
                if (!paths.ContainsKey(opPath))
                {
                    paths.Add(opPath, new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() });
                    AddCommonHttpParameters(paths[opPath]);
                }

                // operations may get listed a few times, make sure we only add them once in each path
                if (!paths[opPath].Operations.ContainsKey(OperationType.Post))
                {
                    paths[opPath].AddOperation(OperationType.Post, oasOpPost);
                    _totalOperations++;
                }
            }
        }
    }

    /// <summary>Builds a resource operation get oas operation.</summary>
    /// <param name="resource">The resource.</param>
    /// <param name="fhirOp">  The FHIR operation.</param>
    /// <param name="schemas"> The schemas.</param>
    /// <param name="opLevel"> The operation level.</param>
    /// <param name="oasOp">   [out] The oas operation.</param>
    /// <returns>An OpenApiOperation.</returns>
    private bool TryBuildOperationGetOasOp(
        FhirComplex resource,
        FhirOperation fhirOp,
        Dictionary<string, OpenApiSchema> schemas,
        OaOpLevelCodes opLevel,
        out OpenApiOperation oasOp)
    {
        // operation get needs to be enabled
        if ((_openApiOptions.OperationSupport != OaHttpSupportCodes.Both) &&
            (_openApiOptions.OperationSupport != OaHttpSupportCodes.Get))
        {
            oasOp = null;
            return false;
        }

        // only enable get if we know it does not affect state
        if (fhirOp.AffectsState != false)
        {
            oasOp = null;
            return false;
        }

        if ((opLevel != OaOpLevelCodes.System) &&
            (resource == null))
        {
            oasOp = null;
            return false;
        }

        oasOp = new();
        string id = string.Empty;

        if (opLevel == OaOpLevelCodes.Instance)
        {
            id = FhirUtils.ToConvention($"{resource.Name}.Instance.{fhirOp.Code}.Get", string.Empty, _openApiOptions.IdConvention);
        }

        if (opLevel == OaOpLevelCodes.Type)
        {
            id = FhirUtils.ToConvention($"{resource.Name}.{fhirOp.Code}.Get", string.Empty, _openApiOptions.IdConvention);
        }

        if (opLevel == OaOpLevelCodes.System)
        {
            id = FhirUtils.ToConvention($"System.{fhirOp.Code}.Get", string.Empty, _openApiOptions.IdConvention);
        }

        if (string.IsNullOrEmpty(id))
        {
            oasOp = null;
            return false;
        }

        oasOp.OperationId = id;

        if (opLevel == OaOpLevelCodes.System)
        {
            oasOp.Tags.Add(new OpenApiTag() { Reference = new OpenApiReference() { Id = "System", Type = ReferenceType.Tag, } });
        }
        else
        {
            oasOp.Tags.Add(new OpenApiTag() { Reference = new OpenApiReference() { Id = resource.Name, Type = ReferenceType.Tag, } });
        }

        if (_openApiOptions.IncludeSummaries)
        {
            if (string.IsNullOrEmpty(fhirOp.Description))
            {
                oasOp.Summary = $"operation: execute the {fhirOp.Name} operation";
            }
            else
            {
                oasOp.Summary = "operation: " + fhirOp.Description;
            }
        }

        if (opLevel == OaOpLevelCodes.Instance)
        {
            // instance level includes PathComponentLogicalId segment
            oasOp.Parameters.Add(BuildReferencedParameter(PathComponentLogicalId));
        }

        if (_openApiOptions.IncludeHeaders)
        {
            oasOp.Parameters.Add(BuildReferencedParameter("Accept"));
            oasOp.Parameters.Add(BuildReferencedParameter("Prefer"));
        }

        HashSet<string> usedParams = new();

        List<FhirParameter> inParams = new();
        List<FhirParameter> outParams = new();

        HashSet<string> inResourceParamNames = new();

        foreach (FhirParameter p in fhirOp.Parameters)
        {
            switch (p.Use.FirstOrDefault('-'))
            {
                case 'i':
                case 'I':
                    inParams.Add(p);

                    if (p.ValueType?.Equals("resource", StringComparison.OrdinalIgnoreCase) ?? false)
                    {
                        inResourceParamNames.Add(p.Name);
                    }

                    break;

                case 'o':
                case 'O':
                    outParams.Add(p);
                    break;
            }
        }

        // can skip this parameter if we are on instance level and there is a single input resource
        bool canSkipInputResource = (opLevel == OaOpLevelCodes.Instance) && (inResourceParamNames.Count() == 1);

        // search for input parameters
        foreach (FhirParameter fhirParam in inParams)
        {
            _totalParamInstances++;

            // skip duplicates
            if (usedParams.Contains(fhirParam.Name))
            {
                continue;
            }

            // skip 'resource' on input if instance level
            if (canSkipInputResource && inResourceParamNames.Contains(fhirParam.Name))
            {
                continue;
            }

            // if we have parameter scopes, skip ones that do not patch
            if ((fhirParam.Scopes?.Any() ?? false) &&
                (!fhirParam.Scopes.Contains(opLevel.ToLiteral())))
            {
                continue;
            }

            // query parameters can only be 'simple' types
            if (!_info.PrimitiveTypes.ContainsKey(fhirParam.ValueType ?? "string"))
            {
                // if we cannot add a required value, fail
                if (fhirParam.Min > 0)
                {
                    oasOp = null;
                    return false;
                }

                continue;
            }

            oasOp.Parameters.Add(BuildStringParameter(fhirParam.Name, fhirParam.Documentation));
            usedParams.Add(fhirParam.Name);
            _totalQueryParameters++;
        }

        if (outParams.Count == 1)
        {
            if (_info.Resources.ContainsKey(outParams[0].ValueType ?? "string"))
            {
                oasOp.Responses = BuildResponses(_responseCodesRead, outParams[0].ValueType ?? "string", schemas);
            }
            else
            {
                oasOp.Responses = BuildResponses(_responseCodesRead, "Parameters", schemas);
            }
        }
        else
        {
            oasOp.Responses = BuildResponses(_responseCodesRead, "Parameters", schemas);
        }

        return true;
    }

    /// <summary>Builds a resource fhir operation post oas operation.</summary>
    /// <param name="resource">The resource.</param>
    /// <param name="fhirOp">  The FHIR operation.</param>
    /// <param name="schemas"> The schemas.</param>
    /// <param name="opLevel"> The operation level.</param>
    /// <param name="oasOp">   [out] The oas operation.</param>
    /// <returns>An OpenApiOperation.</returns>
    private bool TryBuildOperationPostOasOp(
        FhirComplex resource,
        FhirOperation fhirOp,
        Dictionary<string, OpenApiSchema> schemas,
        OaOpLevelCodes opLevel,
        out OpenApiOperation oasOp)
    {
        // operation post needs to be enabled
        if ((_openApiOptions.OperationSupport != OaHttpSupportCodes.Both) &&
            (_openApiOptions.OperationSupport != OaHttpSupportCodes.Post))
        {
            oasOp = null;
            return false;
        }

        if ((opLevel != OaOpLevelCodes.System) &&
            (resource == null))
        {
            oasOp = null;
            return false;
        }

        oasOp = new();

        string id = string.Empty;

        if (opLevel == OaOpLevelCodes.Instance)
        {
            id = FhirUtils.ToConvention($"{resource.Name}.Instance.{fhirOp.Code}.Post", string.Empty, _openApiOptions.IdConvention);
        }

        if (opLevel == OaOpLevelCodes.Type)
        {
            id = FhirUtils.ToConvention($"{resource.Name}.{fhirOp.Code}.Post", string.Empty, _openApiOptions.IdConvention);
        }

        if (opLevel == OaOpLevelCodes.System)
        {
            id = FhirUtils.ToConvention($"System.{fhirOp.Code}.Post", string.Empty, _openApiOptions.IdConvention);
        }

        if (string.IsNullOrEmpty(id))
        {
            oasOp = null;
            return false;
        }

        oasOp.OperationId = id;

        if (opLevel == OaOpLevelCodes.System)
        {
            oasOp.Tags.Add(new OpenApiTag() { Reference = new OpenApiReference() { Id = "System", Type = ReferenceType.Tag, } });
        }
        else
        {
            oasOp.Tags.Add(new OpenApiTag() { Reference = new OpenApiReference() { Id = resource.Name, Type = ReferenceType.Tag, } });
        }

        if (_openApiOptions.IncludeSummaries)
        {
            if (string.IsNullOrEmpty(fhirOp.Description))
            {
                oasOp.Summary = $"operation: execute the {fhirOp.Name} operation";
            }
            else
            {
                oasOp.Summary = "operation: " + fhirOp.Description;
            }
        }

        if (opLevel == OaOpLevelCodes.Instance)
        {
            // instance level includes PathComponentLogicalId segment
            oasOp.Parameters.Add(BuildReferencedParameter(PathComponentLogicalId));
        }

        if (_openApiOptions.IncludeHeaders)
        {
            oasOp.Parameters.Add(BuildReferencedParameter("Accept"));
            oasOp.Parameters.Add(BuildReferencedParameter("Prefer"));
        }

        HashSet<string> usedParams = new();

        List<FhirParameter> inParams = new();
        List<FhirParameter> simpleInParams = new();
        List<FhirParameter> outParams = new();

        HashSet<string> inResourceParamNames = new();
        FhirParameter inputResourceParameter = null;

        int complexInParamCount = 0;

        foreach (FhirParameter p in fhirOp.Parameters)
        {
            switch (p.Use.FirstOrDefault('-'))
            {
                case 'i':
                case 'I':
                    inParams.Add(p);

                    if (p.ValueType?.Equals("resource", StringComparison.OrdinalIgnoreCase) ?? false)
                    {
                        inResourceParamNames.Add(p.Name);
                        inputResourceParameter = p;
                    }
                    else if (_info.Resources.ContainsKey(p.ValueType ?? string.Empty))
                    {
                        inResourceParamNames.Add(p.Name);
                        inputResourceParameter = p;
                    }
                    else if (_info.PrimitiveTypes.ContainsKey(p.ValueType ?? "string"))
                    {
                        simpleInParams.Add(p);
                    }
                    else
                    {
                        complexInParamCount++;
                    }

                    break;

                case 'o':
                case 'O':
                    outParams.Add(p);
                    break;
            }
        }

        // check for a single resource parameter and all others are 'simple' (primitive) types
        if ((inResourceParamNames.Count() == 1) &&
            (complexInParamCount == 0) &&
            (inputResourceParameter != null))
        {
            oasOp.RequestBody = new OpenApiRequestBody()
            {
                Required = true,
                Content = BuildContentMap(inputResourceParameter.ValueType ?? "Resource", schemas),
                Description = _openApiOptions.IncludeDescriptions
                    ? inputResourceParameter.Documentation
                    : null,
            };

            foreach (FhirParameter fhirParam in simpleInParams)
            {
                // skip duplicates
                if (usedParams.Contains(fhirParam.Name))
                {
                    continue;
                }

                oasOp.Parameters.Add(BuildStringParameter(fhirParam.Name, fhirParam.Documentation));
                usedParams.Add(fhirParam.Name);
                _totalQueryParameters++;
                _totalParamInstances++;
            }
        }
        else
        {
            oasOp.RequestBody = new OpenApiRequestBody()
            {
                Required = true,
                Content = BuildContentMap("Parameters", schemas),
                Description = _openApiOptions.IncludeDescriptions
                    ? "Input parameters to the operation"
                    : null,
            };
        }

        if (outParams.Count() == 1)
        {
            if (_info.Resources.ContainsKey(outParams[0].ValueType ?? "string"))
            {
                oasOp.Responses = BuildResponses(_responseCodesRead, outParams[0].ValueType ?? "string", schemas);
            }
            else
            {
                oasOp.Responses = BuildResponses(_responseCodesRead, "Parameters", schemas);
            }
        }
        else
        {
            oasOp.Responses = BuildResponses(_responseCodesRead, "Parameters", schemas);
        }

        return true;
    }

    /// <summary>Adds a common HTTP parameters.</summary>
    /// <param name="pathItem">The path item.</param>
    private void AddCommonHttpParameters(OpenApiPathItem pathItem)
    {
        foreach (string code in _openApiOptions.HttpCommonParams)
        {
            pathItem.Parameters.Add(BuildReferencedParameter(code));
            _totalParamInstances++;
        }
    }

    private const string POST_SEARCH_TYPE = "string";

    /// <summary>Builds resource search system post oas operation.</summary>
    /// <param name="schemas">The schemas.</param>
    /// <returns>An OpenApiOperation.</returns>
    private OpenApiOperation BuildSystemSearchPostOasOp(
        Dictionary<string, OpenApiSchema> schemas)
    {
        OpenApiOperation oasOp = new();

        string id = FhirUtils.ToConvention("Search.System.Post", string.Empty, _openApiOptions.IdConvention);

        oasOp.OperationId = id;

        oasOp.Tags.Add(new OpenApiTag() { Reference = new OpenApiReference() { Id = "System", Type = ReferenceType.Tag, } });

        if (_openApiOptions.IncludeSummaries)
        {
            oasOp.Summary = "search-system: Search all resources";
        }

        if (_openApiOptions.IncludeHeaders)
        {
            oasOp.Parameters.Add(BuildReferencedParameter("Accept"));
            oasOp.Parameters.Add(BuildReferencedParameter("Prefer"));
        }

        oasOp.RequestBody = new OpenApiRequestBody()
        {
            Required = true,
            Content = new Dictionary<string, OpenApiMediaType>()
            {
                { "application/x-www-form-urlencoded", new OpenApiMediaType()
                    {
                        Schema = new OpenApiSchema()
                        {
                            Type = "object"
                        },
                    }
                },
            },
            Description = _openApiOptions.IncludeDescriptions
                ? $"FHIR search parameters"
                : null,
        };

        HashSet<string> usedParams = new();

        foreach (string code in _openApiOptions.HttpReadParams)
        {
            if (usedParams.Contains(code))
            {
                continue;
            }

            oasOp.RequestBody.Content["application/x-www-form-urlencoded"].Schema.Properties.Add(
                code,
                new OpenApiSchema()
                {
                    Title = code,
                    Type = POST_SEARCH_TYPE,
                });
            usedParams.Add(code);
        }

        if (_openApiOptions.IncludeSearchParams)
        {
            foreach (string code in _openApiOptions.SearchCommonParams)
            {
                if (usedParams.Contains(code))
                {
                    continue;
                }

                oasOp.RequestBody.Content["application/x-www-form-urlencoded"].Schema.Properties.Add(
                    code,
                    new OpenApiSchema()
                    {
                        Title = code,
                        Type = POST_SEARCH_TYPE,
                    });
                usedParams.Add(code);
            }

            foreach (string code in _openApiOptions.SearchResultParams)
            {
                if (usedParams.Contains(code))
                {
                    continue;
                }

                oasOp.RequestBody.Content["application/x-www-form-urlencoded"].Schema.Properties.Add(
                    code,
                    new OpenApiSchema()
                    {
                        Title = code,
                        Type = POST_SEARCH_TYPE,
                    });
                usedParams.Add(code);
            }

            if ((_caps != null) &&
                (_caps.ServerSearchParameters != null))
            {
                foreach (FhirCapSearchParam capParam in _caps.ServerSearchParameters.Values)
                {
                    if (usedParams.Contains(capParam.Name))
                    {
                        continue;
                    }

                    oasOp.RequestBody.Content["application/x-www-form-urlencoded"].Schema.Properties.Add(
                        capParam.Name,
                        new OpenApiSchema()
                        {
                            Title = capParam.Name,
                            Type = POST_SEARCH_TYPE,
                            Description = _openApiOptions.IncludeDescriptions
                                ? capParam.Documentation
                                : null,
                        });
                    usedParams.Add(capParam.Name);
                }
            }
        }

        oasOp.Responses = BuildResponses(_responseCodesRead, "Bundle", schemas);

        return oasOp;
    }

    /// <summary>Builds resource search type oas operation.</summary>
    /// <param name="resource">The resource.</param>
    /// <param name="schemas"> The schemas.</param>
    /// <returns>An OpenApiOperation.</returns>
    private OpenApiOperation BuildResourceSearchTypePostOasOp(
        FhirComplex resource,
        Dictionary<string, OpenApiSchema> schemas)
    {
        OpenApiOperation oasOp = new();

        string id = FhirUtils.ToConvention($"Search.{resource.Name}.Post", string.Empty, _openApiOptions.IdConvention);

        oasOp.OperationId = id;

        oasOp.Tags.Add(new OpenApiTag() { Reference = new OpenApiReference() { Id = resource.Name, Type = ReferenceType.Tag, } });

        if (_openApiOptions.IncludeSummaries)
        {
            oasOp.Summary = $"search-type: Search for {resource.Name} instances";
        }

        if (_openApiOptions.IncludeHeaders)
        {
            oasOp.Parameters.Add(BuildReferencedParameter("Accept"));
            oasOp.Parameters.Add(BuildReferencedParameter("Prefer"));
        }

        oasOp.RequestBody = new OpenApiRequestBody()
        {
            Required = true,
            Content = new Dictionary<string, OpenApiMediaType>()
            {
                { "application/x-www-form-urlencoded", new OpenApiMediaType()
                    {
                        Schema = new OpenApiSchema()
                        {
                            Type = "object"
                        },
                    }
                },
            },
            Description = _openApiOptions.IncludeDescriptions
                ? $"FHIR search parameters"
                : null,
        };

        HashSet<string> usedParams = new();

        foreach (string code in _openApiOptions.HttpReadParams)
        {
            if (usedParams.Contains(code))
            {
                continue;
            }

            oasOp.RequestBody.Content["application/x-www-form-urlencoded"].Schema.Properties.Add(
                code,
                new OpenApiSchema()
                {
                    Title = code,
                    Type = POST_SEARCH_TYPE,
                });
            usedParams.Add(code);
        }

        if (_openApiOptions.IncludeSearchParams)
        {
            foreach (string code in _openApiOptions.SearchCommonParams)
            {
                if (usedParams.Contains(code))
                {
                    continue;
                }

                oasOp.RequestBody.Content["application/x-www-form-urlencoded"].Schema.Properties.Add(
                    code,
                    new OpenApiSchema()
                    {
                        Title = code,
                        Type = POST_SEARCH_TYPE,
                    });
                usedParams.Add(code);
            }

            foreach (string code in _openApiOptions.SearchResultParams)
            {
                if (usedParams.Contains(code))
                {
                    continue;
                }

                oasOp.RequestBody.Content["application/x-www-form-urlencoded"].Schema.Properties.Add(
                    code,
                    new OpenApiSchema()
                    {
                        Title = code,
                        Type = POST_SEARCH_TYPE,
                    });
                usedParams.Add(code);
            }

            foreach (FhirSearchParam fhirSp in GetResourceSearchParameters(resource.Name))
            {
                if (usedParams.Contains(fhirSp.Code))
                {
                    continue;
                }

                oasOp.RequestBody.Content["application/x-www-form-urlencoded"].Schema.Properties.Add(
                    fhirSp.Code,
                    new OpenApiSchema()
                    {
                        Title = fhirSp.Name,
                        Type = "string",
                    });

                usedParams.Add(fhirSp.Code);
            }
        }

        oasOp.Responses = BuildResponses(_responseCodesRead, "Bundle", schemas);

        return oasOp;
    }

    /// <summary>Builds resource search system get oas operation.</summary>
    /// <param name="schemas">The schemas.</param>
    /// <returns>An OpenApiOperation.</returns>
    private OpenApiOperation BuildSystemSearchGetOasOp(
        Dictionary<string, OpenApiSchema> schemas)
    {
        OpenApiOperation oasOp = new();

        string id = FhirUtils.ToConvention("Search.System.Get", string.Empty, _openApiOptions.IdConvention);

        oasOp.OperationId = id;

        oasOp.Tags.Add(new OpenApiTag() { Reference = new OpenApiReference() { Id = "System", Type = ReferenceType.Tag, } });

        if (_openApiOptions.IncludeSummaries)
        {
            oasOp.Summary = "search-system: Search all resources";
        }

        if (_openApiOptions.IncludeHeaders)
        {
            oasOp.Parameters.Add(BuildReferencedParameter("Accept"));
            oasOp.Parameters.Add(BuildReferencedParameter("Prefer"));
        }

        HashSet<string> usedParams = new();

        foreach (string code in _openApiOptions.HttpReadParams)
        {
            _totalParamInstances++;

            if (usedParams.Contains(code))
            {
                continue;
            }

            oasOp.Parameters.Add(BuildReferencedParameter(code));
            usedParams.Add(code);
            _totalQueryParameters++;
        }

        if (_openApiOptions.IncludeSearchParams)
        {
            foreach (string code in _openApiOptions.SearchCommonParams)
            {
                _totalParamInstances++;
                
                if (usedParams.Contains(code))
                {
                    continue;
                }

                oasOp.Parameters.Add(BuildReferencedParameter(code));
                usedParams.Add(code);
                _totalQueryParameters++;
            }

            foreach (string code in _openApiOptions.SearchResultParams)
            {
                _totalParamInstances++;

                if (usedParams.Contains(code))
                {
                    continue;
                }

                oasOp.Parameters.Add(BuildReferencedParameter(code));
                usedParams.Add(code);
                _totalQueryParameters++;
            }

            if ((_caps != null) &&
                (_caps.ServerSearchParameters != null))
            {
                foreach (FhirCapSearchParam capParam in _caps.ServerSearchParameters.Values)
                {
                    _totalParamInstances++;
                    
                    if (usedParams.Contains(capParam.Name))
                    {
                        continue;
                    }

                    oasOp.Parameters.Add(BuildStringParameter(capParam.Name, capParam.Documentation));
                    usedParams.Add(capParam.Name);
                    _totalQueryParameters++;
                }
            }
        }

        oasOp.Responses = BuildResponses(_responseCodesRead, "Bundle", schemas);

        return oasOp;
    }

    /// <summary>Builds resource search type oas operation.</summary>
    /// <param name="resource">       The resource.</param>
    /// <param name="schemas">        The schemas.</param>
    /// <param name="consolidatedSps">The consolidated search parameter keys.</param>
    /// <returns>An OpenApiOperation.</returns>
    private OpenApiOperation BuildResourceSearchTypeGetOasOp(
        FhirComplex resource,
        Dictionary<string, OpenApiSchema> schemas)
    {
        OpenApiOperation oasOp = new();

        string id = FhirUtils.ToConvention($"Search.{resource.Name}.Get", string.Empty, _openApiOptions.IdConvention);

        oasOp.OperationId = id;

        oasOp.Tags.Add(new OpenApiTag() { Reference = new OpenApiReference() { Id = resource.Name, Type = ReferenceType.Tag, } });

        if (_openApiOptions.IncludeSummaries)
        {
            oasOp.Summary = $"search-type: Search for {resource.Name} instances";
        }

        if (_openApiOptions.IncludeHeaders)
        {
            oasOp.Parameters.Add(BuildReferencedParameter("Accept"));
            oasOp.Parameters.Add(BuildReferencedParameter("Prefer"));
        }

        HashSet<string> usedParams = new();

        foreach (string code in _openApiOptions.HttpReadParams)
        {
            _totalParamInstances++;

            if (usedParams.Contains(code))
            {
                continue;
            }

            oasOp.Parameters.Add(BuildReferencedParameter(code));
            usedParams.Add(code);
            _totalQueryParameters++;
        }

        if (_openApiOptions.IncludeSearchParams)
        {
            foreach (string code in _openApiOptions.SearchCommonParams)
            {
                _totalParamInstances++;

                if (usedParams.Contains(code))
                {
                    continue;
                }

                oasOp.Parameters.Add(BuildReferencedParameter(code));
                usedParams.Add(code);
                _totalQueryParameters++;
            }

            foreach (string code in _openApiOptions.SearchResultParams)
            {
                _totalParamInstances++;

                if (usedParams.Contains(code))
                {
                    continue;
                }

                oasOp.Parameters.Add(BuildReferencedParameter(code));
                usedParams.Add(code);
                _totalQueryParameters++;
            }

            foreach (FhirSearchParam fhirSp in GetResourceSearchParameters(resource.Name))
            {
                _totalParamInstances++;

                if (usedParams.Contains(fhirSp.Code))
                {
                    continue;
                }

                if (_openApiOptions.ConsolidateSearchParams)
                {
                    oasOp.Parameters.Add(BuildReferencedParameter(fhirSp.Code));
                    usedParams.Add(fhirSp.Code);
                    _totalQueryParameters++;
                    continue;
                }

                if (fhirSp.ValueType?.Equals("number") ?? false)
                {
                    oasOp.Parameters.Add(BuildNumberParameter(fhirSp.Code, fhirSp.Description));
                }
                else
                {
                    oasOp.Parameters.Add(BuildStringParameter(fhirSp.Code, fhirSp.Description));
                }

                usedParams.Add(fhirSp.Code);
                _totalQueryParameters++;
            }
        }

        //if ((_caps != null) &&
        //    (_caps.ResourceInteractions != null) &&
        //    _caps.ResourceInteractions.ContainsKey(resource.Name))
        //{
        //    foreach (FhirCapSearchParam capParam in _caps.ResourceInteractions[resource.Name].SearchParameters.Values)
        //    {
        //        if (usedParams.Contains(capParam.Name))
        //        {
        //            continue;
        //        }

        //        oasOp.Parameters.Add(BuildStringParameter(capParam.Name, capParam.Documentation));
        //        usedParams.Add(capParam.Name);
        //    }
        //}

        oasOp.Responses = BuildResponses(_responseCodesRead, "Bundle", schemas);

        return oasOp;
    }

    /// <summary>Builds resource update oas operation.</summary>
    /// <param name="resource">The resource.</param>
    /// <param name="schemas"> The schemas.</param>
    /// <returns>An OpenApiOperation.</returns>
    private OpenApiOperation BuildResourceCreateOasOp(
        FhirComplex resource,
        Dictionary<string, OpenApiSchema> schemas)
    {
        OpenApiOperation oasOp = new();

        string id = FhirUtils.ToConvention("Create." + resource.Name, string.Empty, _openApiOptions.IdConvention);

        oasOp.OperationId = id;

        oasOp.Tags.Add(new OpenApiTag() { Reference = new OpenApiReference() { Id = resource.Name, Type = ReferenceType.Tag, } });

        if (_openApiOptions.IncludeSummaries)
        {
            oasOp.Summary = $"create: Create a new {resource.Name} instance";
        }

        if (_openApiOptions.IncludeHeaders)
        {
            oasOp.Parameters.Add(BuildReferencedParameter("Accept"));
            oasOp.Parameters.Add(BuildReferencedParameter("Prefer"));

            if (ResolveConditionalCreate(resource.Name))
            {
                oasOp.Parameters.Add(BuildReferencedParameter("If-None-Exist"));
                _totalParamInstances++;
            }
        }

        oasOp.RequestBody = new OpenApiRequestBody()
        {
            Required = true,
            Content = BuildContentMap(resource.Name, schemas),
            Description = _openApiOptions.IncludeDescriptions
                ? $"A FHIR {resource.Name}"
                : null,
        };

        oasOp.Responses = BuildResponses(_responseCodesUpdate, resource.Name, schemas);

        return oasOp;
    }


    /// <summary>Builds resource history type oas operation.</summary>
    /// <param name="resource">The resource.</param>
    /// <param name="schemas"> The schemas.</param>
    /// <returns>An OpenApiOperation.</returns>
    private OpenApiOperation BuildResourceHistoryTypeOasOp(
        FhirComplex resource,
        Dictionary<string, OpenApiSchema> schemas)
    {
        OpenApiOperation oasOp = new();

        string id = FhirUtils.ToConvention("History.Type." + resource.Name, string.Empty, _openApiOptions.IdConvention);

        oasOp.OperationId = id;

        oasOp.Tags.Add(new OpenApiTag() { Reference = new OpenApiReference() { Id = resource.Name, Type = ReferenceType.Tag, } });

        if (_openApiOptions.IncludeSummaries)
        {
            oasOp.Summary = $"history-type: Get the change history of all {resource.Name} resources";
        }

        if (_openApiOptions.IncludeHeaders)
        {
            oasOp.Parameters.Add(BuildReferencedParameter("Accept"));
            oasOp.Parameters.Add(BuildReferencedParameter("Prefer"));

            switch (ResolveConditionalRead(resource.Name))
            {
                case ConditionalReadPolicy.NotSupported:
                    break;
                case ConditionalReadPolicy.ModifiedSince:
                    oasOp.Parameters.Add(BuildReferencedParameter("If-Modified-Since"));
                    _totalParamInstances++;
                    break;
                case ConditionalReadPolicy.NotMatch:
                    oasOp.Parameters.Add(BuildReferencedParameter("If-None-Match"));
                    _totalParamInstances++;
                    break;
                case ConditionalReadPolicy.FullSupport:
                    oasOp.Parameters.Add(BuildReferencedParameter("If-Modified-Since"));
                    oasOp.Parameters.Add(BuildReferencedParameter("If-None-Match"));
                    _totalParamInstances++;
                    _totalParamInstances++;
                    break;
            }
        }

        foreach (string code in _openApiOptions.HttpReadParams)
        {
            oasOp.Parameters.Add(BuildReferencedParameter(code));
            _totalParamInstances++;
        }

        foreach (string code in _openApiOptions.HistoryParams)
        {
            oasOp.Parameters.Add(BuildReferencedParameter(code));
            _totalParamInstances++;
        }

        oasOp.Responses = BuildResponses(_responseCodesRead, "Bundle", schemas);

        return oasOp;
    }

    /// <summary>Builds resource history instance oas operation.</summary>
    /// <param name="resource">The resource.</param>
    /// <param name="schemas"> The schemas.</param>
    /// <returns>An OpenApiOperation.</returns>
    private OpenApiOperation BuildResourceHistoryInstanceOasOp(
        FhirComplex resource,
        Dictionary<string, OpenApiSchema> schemas)
    {
        OpenApiOperation oasOp = new();

        string id = FhirUtils.ToConvention("History.Instance." + resource.Name, string.Empty, _openApiOptions.IdConvention);

        oasOp.OperationId = id;

        oasOp.Tags.Add(new OpenApiTag() { Reference = new OpenApiReference() { Id = resource.Name, Type = ReferenceType.Tag, } });

        if (_openApiOptions.IncludeSummaries)
        {
            oasOp.Summary = $"history-instance: Get the change history of a specific {resource.Name} resource";
        }

        // history instance includes PathComponentLogicalId segment
        oasOp.Parameters.Add(BuildReferencedParameter(PathComponentLogicalId));

        if (_openApiOptions.IncludeHeaders)
        {
            oasOp.Parameters.Add(BuildReferencedParameter("Accept"));
            oasOp.Parameters.Add(BuildReferencedParameter("Prefer"));

            switch (ResolveConditionalRead(resource.Name))
            {
                case ConditionalReadPolicy.NotSupported:
                    break;
                case ConditionalReadPolicy.ModifiedSince:
                    oasOp.Parameters.Add(BuildReferencedParameter("If-Modified-Since"));
                    _totalParamInstances++;
                    break;
                case ConditionalReadPolicy.NotMatch:
                    oasOp.Parameters.Add(BuildReferencedParameter("If-None-Match"));
                    _totalParamInstances++;
                    break;
                case ConditionalReadPolicy.FullSupport:
                    oasOp.Parameters.Add(BuildReferencedParameter("If-Modified-Since"));
                    oasOp.Parameters.Add(BuildReferencedParameter("If-None-Match"));
                    _totalParamInstances++;
                    _totalParamInstances++;
                    break;
            }
        }

        foreach (string code in _openApiOptions.HttpReadParams)
        {
            oasOp.Parameters.Add(BuildReferencedParameter(code));
            _totalParamInstances++;
        }

        foreach (string code in _openApiOptions.HistoryParams)
        {
            oasOp.Parameters.Add(BuildReferencedParameter(code));
            _totalParamInstances++;
        }

        oasOp.Responses = BuildResponses(_responseCodesRead, "Bundle", schemas);

        return oasOp;
    }

    /// <summary>Builds resource delete oas operation.</summary>
    /// <param name="resource">The resource.</param>
    /// <param name="schemas"> The schemas.</param>
    /// <returns>An OpenApiOperation.</returns>
    private OpenApiOperation BuildResourceDeleteOasOp(
        FhirComplex resource,
        Dictionary<string, OpenApiSchema> schemas)
    {
        ConditionalDeletePolicy conditionalDelete = ResolveConditiaonlDelete(resource.Name);

        OpenApiOperation oasOp = new();

        string id = FhirUtils.ToConvention("Delete." + resource.Name, string.Empty, _openApiOptions.IdConvention);

        oasOp.OperationId = id;

        oasOp.Tags.Add(new OpenApiTag() { Reference = new OpenApiReference() { Id = resource.Name, Type = ReferenceType.Tag, } });

        if (_openApiOptions.IncludeSummaries)
        {
            oasOp.Summary = $"delete: Perform a loical delete on a {resource.Name} instance";
        }

        // delete includes PathComponentLogicalId segment
        oasOp.Parameters.Add(BuildReferencedParameter(PathComponentLogicalId));

        if (_openApiOptions.IncludeHeaders)
        {
            oasOp.Parameters.Add(BuildReferencedParameter("Accept"));
            oasOp.Parameters.Add(BuildReferencedParameter("Prefer"));

            if (conditionalDelete != ConditionalDeletePolicy.NotSupported)
            {
                oasOp.Parameters.Add(BuildReferencedParameter("If-Match"));
                _totalParamInstances++;
            }
        }

        // conditional deletes take search parameters
        if ((conditionalDelete != ConditionalDeletePolicy.NotSupported) &&
            _openApiOptions.IncludeSearchParams)
        {
            HashSet<string> usedParams = new();

            foreach (string code in _openApiOptions.SearchCommonParams)
            {
                _totalParamInstances++;

                if (usedParams.Contains(code))
                {
                    continue;
                }

                oasOp.Parameters.Add(BuildReferencedParameter(code));
                usedParams.Add(code);
                _totalQueryParameters++;
            }

            foreach (FhirSearchParam fhirSp in GetResourceSearchParameters(resource.Name))
            {
                _totalParamInstances++;

                if (_openApiOptions.ConsolidateSearchParams)
                {
                    oasOp.Parameters.Add(BuildReferencedParameter(fhirSp.Code));
                    usedParams.Add(fhirSp.Code);
                    _totalQueryParameters++;
                    continue;
                }

                if (fhirSp.ValueType?.Equals("number") ?? false)
                {
                    oasOp.Parameters.Add(BuildNumberParameter(fhirSp.Code, fhirSp.Description));
                }
                else
                {
                    oasOp.Parameters.Add(BuildStringParameter(fhirSp.Code, fhirSp.Description));
                }

                usedParams.Add(fhirSp.Code);
                _totalQueryParameters++;
            }
        }

        oasOp.Responses = BuildResponses(_responseCodesDelete, resource.Name, schemas);

        return oasOp;
    }

    /// <summary>Builds resource patch oas operation.</summary>
    /// <param name="resource">The resource.</param>
    /// <param name="schemas"> The schemas.</param>
    /// <returns>An OpenApiOperation.</returns>
    private OpenApiOperation BuildResourcePatchOasOp(
        FhirComplex resource,
        Dictionary<string, OpenApiSchema> schemas)
    {
        OpenApiOperation oasOp = new();

        string id = FhirUtils.ToConvention("Patch." + resource.Name, string.Empty, _openApiOptions.IdConvention);

        oasOp.OperationId = id;

        oasOp.Tags.Add(new OpenApiTag() { Reference = new OpenApiReference() { Id = resource.Name, Type = ReferenceType.Tag, } });

        if (_openApiOptions.IncludeSummaries)
        {
            oasOp.Summary = $"patch: Patch a {resource.Name} instance";
        }

        // patch includes PathComponentLogicalId segment
        oasOp.Parameters.Add(BuildReferencedParameter(PathComponentLogicalId));

        if (_openApiOptions.IncludeHeaders)
        {
            oasOp.Parameters.Add(BuildReferencedParameter("Accept"));
            oasOp.Parameters.Add(BuildReferencedParameter("Prefer"));

            if (ResolveConditionalPatch(resource.Name))
            {
                oasOp.Parameters.Add(BuildReferencedParameter("If-Match"));
                _totalParamInstances++;
            }
        }

        oasOp.RequestBody = new OpenApiRequestBody()
        {
            Required = true,
            Content = BuildContentMapForPatch(resource.Name, schemas),
            Description = _openApiOptions.IncludeDescriptions
                ? $"Patch to apply to a FHIR {resource.Name} record"
                : null,
        };

        oasOp.Responses = BuildResponses(_responseCodesConditionalOrPatch, resource.Name, schemas);

        return oasOp;
    }

    /// <summary>Builds resource update oas operation.</summary>
    /// <param name="resource">The resource.</param>
    /// <param name="schemas"> The schemas.</param>
    /// <returns>An OpenApiOperation.</returns>
    private OpenApiOperation BuildResourceUpdateOasOp(
        FhirComplex resource,
        Dictionary<string, OpenApiSchema> schemas)
    {
        OpenApiOperation oasOp = new();

        bool supportUpdateCreate = ResolveUpdateCreate(resource.Name);
        bool supportConditionalCreate = supportUpdateCreate && ResolveConditionalCreate(resource.Name);
        bool supportConditionalUpdate = ResolveConditionalUpdate(resource.Name);

        string id;

        if (ResolveUpdateCreate(resource.Name))
        {
            id = FhirUtils.ToConvention("UpdateOrCreate." + resource.Name, string.Empty, _openApiOptions.IdConvention);

            if (_openApiOptions.IncludeSummaries)
            {
                oasOp.Summary = $"update,create: Update or Create a {resource.Name} instance";
            }
        }
        else
        {
            id = FhirUtils.ToConvention("Update." + resource.Name, string.Empty, _openApiOptions.IdConvention);

            if (_openApiOptions.IncludeSummaries)
            {
                oasOp.Summary = $"update: Update a {resource.Name} instance";
            }
        }

        oasOp.OperationId = id;

        oasOp.Tags.Add(new OpenApiTag() { Reference = new OpenApiReference() { Id = resource.Name, Type = ReferenceType.Tag, } });

        // update includes PathComponentLogicalId segment
        oasOp.Parameters.Add(BuildReferencedParameter(PathComponentLogicalId));

        if (_openApiOptions.IncludeHeaders)
        {
            oasOp.Parameters.Add(BuildReferencedParameter("Accept"));
            oasOp.Parameters.Add(BuildReferencedParameter("Prefer"));

            if (supportUpdateCreate && supportConditionalCreate)
            {
                oasOp.Parameters.Add(BuildReferencedParameter("If-None-Exist"));
                _totalParamInstances++;
            }

            if (supportConditionalUpdate)
            {
                oasOp.Parameters.Add(BuildReferencedParameter("If-Match"));
                oasOp.Parameters.Add(BuildReferencedParameter("If-None-Match"));
                _totalParamInstances++;
                _totalParamInstances++;
            }
        }

        // conditional updates take search parameters
        if (ResolveConditionalUpdate(resource.Name))
        {
            if (_openApiOptions.IncludeSearchParams)
            {
                HashSet<string> usedParams = new();

                foreach (string code in _openApiOptions.SearchCommonParams)
                {
                    _totalParamInstances++;

                    if (usedParams.Contains(code))
                    {
                        continue;
                    }

                    oasOp.Parameters.Add(BuildReferencedParameter(code));
                    usedParams.Add(code);
                    _totalQueryParameters++;
                }

                foreach (FhirSearchParam fhirSp in GetResourceSearchParameters(resource.Name))
                {
                    _totalParamInstances++;

                    if (_openApiOptions.ConsolidateSearchParams)
                    {
                        oasOp.Parameters.Add(BuildReferencedParameter(fhirSp.Code));
                        usedParams.Add(fhirSp.Code);
                        _totalQueryParameters++;
                        continue;
                    }

                    if (fhirSp.ValueType?.Equals("number") ?? false)
                    {
                        oasOp.Parameters.Add(BuildNumberParameter(fhirSp.Code, fhirSp.Description));
                    }
                    else
                    {
                        oasOp.Parameters.Add(BuildStringParameter(fhirSp.Code, fhirSp.Description));
                    }

                    usedParams.Add(fhirSp.Code);
                    _totalQueryParameters++;
                }
            }
        }

        oasOp.RequestBody = new OpenApiRequestBody()
        {
            Required = true,
            Content = BuildContentMap(resource.Name, schemas),
            Description = _openApiOptions.IncludeDescriptions
                ? $"An updated FHIR {resource.Name}"
                : null,
        };

        oasOp.Responses = BuildResponses(_responseCodesUpdate, resource.Name, schemas);

        return oasOp;
    }

    /// <summary>Builds resource VRead oas operation.</summary>
    /// <param name="resource">The resource.</param>
    /// <param name="schemas"> The schemas.</param>
    /// <returns>An OpenApiOperation.</returns>
    private OpenApiOperation BuildResourceVReadOasOp(
        FhirComplex resource,
        Dictionary<string, OpenApiSchema> schemas)
    {
        OpenApiOperation oasOp = new();

        string id = FhirUtils.ToConvention("VRead." + resource.Name, string.Empty, _openApiOptions.IdConvention);

        oasOp.OperationId = id;

        oasOp.Tags.Add(new OpenApiTag() { Reference = new OpenApiReference() { Id = resource.Name, Type = ReferenceType.Tag, } });

        if (_openApiOptions.IncludeSummaries)
        {
            oasOp.Summary = $"vread: Read {resource.Name} instance with specific version";
        }

        // vread includes PathComponentLogicalId segment
        oasOp.Parameters.Add(BuildReferencedParameter(PathComponentLogicalId));

        // vread includes PathComponentVersionId segment
        oasOp.Parameters.Add(BuildReferencedParameter(PathComponentVersionId));

        foreach (string code in _openApiOptions.HttpReadParams)
        {
            oasOp.Parameters.Add(BuildReferencedParameter(code));
        }

        if (_openApiOptions.IncludeHeaders)
        {
            oasOp.Parameters.Add(BuildReferencedParameter("Accept"));
            oasOp.Parameters.Add(BuildReferencedParameter("Prefer"));

            switch (ResolveConditionalRead(resource.Name))
            {
                case ConditionalReadPolicy.NotSupported:
                    break;
                case ConditionalReadPolicy.ModifiedSince:
                    oasOp.Parameters.Add(BuildReferencedParameter("If-Modified-Since"));
                    _totalParamInstances++;
                    break;
                case ConditionalReadPolicy.NotMatch:
                    oasOp.Parameters.Add(BuildReferencedParameter("If-None-Match"));
                    _totalParamInstances++;
                    break;
                case ConditionalReadPolicy.FullSupport:
                    oasOp.Parameters.Add(BuildReferencedParameter("If-Modified-Since"));
                    oasOp.Parameters.Add(BuildReferencedParameter("If-None-Match"));
                    _totalParamInstances++;
                    _totalParamInstances++;
                    break;
            }
        }

        oasOp.Responses = BuildResponses(_responseCodesRead, resource.Name, schemas);

        return oasOp;
    }

    /// <summary>Builds resource read oas operation.</summary>
    /// <param name="resource">The resource.</param>
    /// <param name="schemas"> The schemas.</param>
    /// <returns>An OpenApiOperation.</returns>
    private OpenApiOperation BuildResourceReadOasOp(
        FhirComplex resource,
        Dictionary<string, OpenApiSchema> schemas)
    {
        OpenApiOperation oasOp = new();

        string id = FhirUtils.ToConvention("Read." + resource.Name, string.Empty, _openApiOptions.IdConvention);

        oasOp.OperationId = id;

        oasOp.Tags.Add(new OpenApiTag() { Reference = new OpenApiReference() { Id = resource.Name, Type = ReferenceType.Tag, } });

        if (_openApiOptions.IncludeSummaries)
        {
            oasOp.Summary = $"read: Read a FHIR {resource.Name}";
        }

        // read includes PathComponentLogicalId segment
        oasOp.Parameters.Add(BuildReferencedParameter(PathComponentLogicalId));

        foreach (string code in _openApiOptions.HttpReadParams)
        {
            oasOp.Parameters.Add(BuildReferencedParameter(code));
        }

        if (_openApiOptions.IncludeHeaders)
        {
            oasOp.Parameters.Add(BuildReferencedParameter("Accept"));
            oasOp.Parameters.Add(BuildReferencedParameter("Prefer"));

            switch (ResolveConditionalRead(resource.Name))
            {
                case ConditionalReadPolicy.NotSupported:
                    break;
                case ConditionalReadPolicy.ModifiedSince:
                    oasOp.Parameters.Add(BuildReferencedParameter("If-Modified-Since"));
                    _totalParamInstances++;
                    break;
                case ConditionalReadPolicy.NotMatch:
                    oasOp.Parameters.Add(BuildReferencedParameter("If-None-Match"));
                    _totalParamInstances++;
                    break;
                case ConditionalReadPolicy.FullSupport:
                    oasOp.Parameters.Add(BuildReferencedParameter("If-Modified-Since"));
                    oasOp.Parameters.Add(BuildReferencedParameter("If-None-Match"));
                    _totalParamInstances++;
                    _totalParamInstances++;
                    break;
            }
        }

        oasOp.Responses = BuildResponses(_responseCodesRead, resource.Name, schemas);

        return oasOp;
    }

    /// <summary>Builds the responses.</summary>
    /// <param name="responseCodes">The response codes.</param>
    /// <param name="resourceName"> Name of the resource.</param>
    /// <param name="schemas">      The schemas.</param>
    private OpenApiResponses BuildResponses(
        int[] responseCodes,
        string resourceName,
        Dictionary<string, OpenApiSchema> schemas)
    {
        // TODO(ginoc): figure out which response headers belong on which response types

        OpenApiResponses r = new();
        emitResponse(_openApiOptions.SingleResponseCode ? responseCodes.Take(1) : responseCodes, resourceName, schemas, r);
        return r;

        void emitResponse(IEnumerable<int> responseCodes, string resourceName, Dictionary<string, OpenApiSchema> schemas, OpenApiResponses r)
        {
            foreach (int code in responseCodes)
            {
                bool isErrorResponse = code >= 400;

                switch (code)
                {
                    case 200:
                    case 201:
                        r.Add(
                            code.ToString(),
                            new OpenApiResponse()
                            {
                                Description = _httpResponseDescriptions[code],
                                Content = BuildContentMap(resourceName, schemas),
                            });
                        break;

                    default:
                        r.Add(
                            code.ToString(),
                            new OpenApiResponse()
                            {
                                Description = _httpResponseDescriptions[code],
                                Content = isErrorResponse ? BuildContentMap("OperationOutcome", schemas) : new OpenApiResponse().Content
                            });
                        break;
                }
            }
        }
    }

    /// <summary>Gets the interactions in this collection.</summary>
    /// <param name="resourceName">Name of the resource.</param>
    /// <returns>
    /// An enumerator that allows foreach to be used to process the interactions in this collection.
    /// </returns>
    private IEnumerable<FhirInteractionCodes> GetInteractions(string resourceName)
    {
        if (!IncludeResouce(resourceName))
        {
            return Array.Empty<FhirInteractionCodes>();
        }

        List<FhirInteractionCodes> interactions = new();

        if (_caps != null)
        {
            if (!_caps.ResourceInteractions.ContainsKey(resourceName))
            {
                return Array.Empty<FhirInteractionCodes>();
            }

            if (_openApiOptions.GenerateReadOnly)
            {
                return _caps.ResourceInteractions[resourceName].Interactions.Where(i => _resInteracionHashRO.Contains(i));
            }

            if (_openApiOptions.GenerateWriteOnly)
            {
                return _caps.ResourceInteractions[resourceName].Interactions.Where(i => _resInteactionHashWO.Contains(i));
            }

            return _caps.ResourceInteractions[resourceName].Interactions;
        }

        if (_openApiOptions.GenerateReadOnly)
        {
            return _resInteractionsRO;
        }

        if (_openApiOptions.GenerateWriteOnly)
        {
            return _resInteractionsWO;
        }

        return _resInteractionsRW;
    }

    /// <summary>Gets the resource FHIR operations in this collection.</summary>
    /// <param name="resourceName">Name of the resource.</param>
    /// <param name="kindFilter">  (Optional) A filter specifying the kind.</param>
    /// <returns>
    /// An enumerator that allows foreach to be used to process the resource FHIR operations in this
    /// collection.
    /// </returns>
    private IEnumerable<FhirOperation> GetResourceFhirOperations(
        string resourceName,
        FhirOperation.OperationKindCodes? kindFilter = null)
    {
        if (!IncludeResouce(resourceName))
        {
            return Array.Empty<FhirOperation>();
        }

        HashSet<string> resourceAndParentsHash = _info.GetInheritanceNamesHash(resourceName);

        if (!resourceAndParentsHash.Any())
        {
            // unknown resource name
            return Array.Empty<FhirOperation>();
        }

        List<FhirOperation> operations = new();

        if (_caps != null)
        {
            if (!_caps.ResourceInteractions.ContainsKey(resourceName))
            {
                return Array.Empty<FhirOperation>();
            }

            // check operations for this specific resource
            foreach (FhirCapOperation capOp in _caps.ResourceInteractions[resourceName].Operations.Values)
            {
                if (!FhirManager.Current.TryResolveCanonical(_info.FhirSequence, capOp.DefinitionCanonical, _exporterOptions.ResolveExternal, out FhirArtifactClassEnum ac, out object fhirOpObj))
                {
                    Console.WriteLine($"Skipping unresolvable Operation: {capOp.DefinitionCanonical}");
                    continue;
                }

                if (!(fhirOpObj is FhirOperation fhirOp))
                {
                    Console.WriteLine($"Operation canonical '{capOp.DefinitionCanonical}' resolved to {ac}");
                    continue;
                }

                // since some operation definitions exclude the resource type when they mean 'Resource', missing defaults to true
                if (!(fhirOp.ResourceTypes?.Any(rt => resourceAndParentsHash.Contains(rt)) ?? true))
                {
                    Console.WriteLine($"Skipping {resourceName} requested operation '{capOp.DefinitionCanonical}' - definition cannot apply here");
                    continue;
                }

                if ((kindFilter == null) ||
                    (kindFilter == fhirOp.Kind))
                {
                    operations.Add(fhirOp);
                }
            }

            // some servers just shove all operations into system-level reporting
            foreach (FhirCapOperation capOp in _caps.ServerOperations.Values)
            {
                if (!FhirManager.Current.TryResolveCanonical(_info.FhirSequence, capOp.DefinitionCanonical, _exporterOptions.ResolveExternal, out FhirArtifactClassEnum ac, out object fhirOpObj))
                {
                    continue;
                }

                if (!(fhirOpObj is FhirOperation fhirOp))
                {
                    continue;
                }

                // since some operation definitions exclude the resource type when they mean 'Resource', missing defaults to true
                if (!(fhirOp.ResourceTypes?.Any(rt => resourceAndParentsHash.Contains(rt)) ?? true))
                {
                    continue;
                }

                if ((!fhirOp.DefinedOnInstance) && (!fhirOp.DefinedOnType))
                {
                    continue;
                }

                if ((kindFilter == null) ||
                    (kindFilter == fhirOp.Kind))
                {
                    operations.Add(fhirOp);
                }
            }

            return operations;
        }

        // if there is no capabilties, just check everything in the package
        foreach (FhirOperation fhirOp in _info.OperationsByUrl.Values)
        {
            if (!(fhirOp.ResourceTypes?.Contains(resourceName) ?? false))
            {
                continue;
            }

            if ((!fhirOp.DefinedOnInstance) && (!fhirOp.DefinedOnType))
            {
                continue;
            }

            if ((kindFilter == null) ||
                (kindFilter == fhirOp.Kind))
            {
                operations.Add(fhirOp);
            }
        }

        return operations;
    }

    /// <summary>Gets FHIR query search parameter.</summary>
    /// <param name="uriBase">     The URI base.</param>
    /// <param name="resourceName">Name of the resource.</param>
    /// <returns>The FHIR query search parameter.</returns>
    private FhirSearchParam GetFhirQuerySearchParam(
        Uri uriBase,
        string resourceName)
    {
        return new FhirSearchParam(
            "_query",
            new Uri(uriBase, "/SearchParameter/Resource-query"),
            _info.VersionString,
            "_query",
            "A custom search profile that describes a specific defined query operation",
            "A custom search profile that describes a specific defined query operation",
            "_query",
            new List<string>() { resourceName },
            null,
            "string",
            string.Empty,
            string.Empty,
            null,
            false,
            string.Empty,
            string.Empty,
            string.Empty);
    }

    /// <summary>Gets the resource search parameters in this collection.</summary>
    /// <param name="resourceName">Name of the resource.</param>
    /// <returns>
    /// An enumerator that allows foreach to be used to process the resource FHIR search parameters in this
    /// collection.
    /// </returns>
    private IEnumerable<FhirSearchParam> GetResourceSearchParameters(string resourceName)
    {
        if (!IncludeResouce(resourceName))
        {
            return Array.Empty<FhirSearchParam>();
        }

        HashSet<string> used = new();
        List<FhirSearchParam> searchParameters = new();

        if (_caps != null)
        {
            Uri uriBase = string.IsNullOrEmpty(_caps.Url)
                ? new Uri("http://fhir-codegen-local/SearchParameter")
                : new Uri(_caps.Url);

            if (!_caps.ResourceInteractions.ContainsKey(resourceName))
            {
                return Array.Empty<FhirSearchParam>();
            }

            // check operations for this specific resource
            foreach (FhirCapSearchParam capSp in _caps.ResourceInteractions[resourceName].SearchParameters.Values)
            {
                if (used.Contains(capSp.Name))
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(capSp.DefinitionCanonical))
                {
                    if (FhirManager.Current.TryResolveCanonical(_info.FhirSequence, capSp.DefinitionCanonical, _exporterOptions.ResolveExternal, out FhirArtifactClassEnum ac, out object fhirSpObj) &&
                        (fhirSpObj is FhirSearchParam fhirSp))
                    {
                        searchParameters.Add(fhirSp);
                        used.Add(capSp.Name);
                        continue;
                    }
                }

                // create a 'local' canonical for referecing
                searchParameters.Add(new FhirSearchParam(
                    capSp.Name,
                    string.IsNullOrEmpty(capSp.DefinitionCanonical)
                        ? new Uri(uriBase, resourceName + "-" + capSp.Name)
                        : new Uri(capSp.DefinitionCanonical),
                    _info.VersionString,
                    capSp.Name,
                    capSp.Documentation,
                    capSp.Documentation,
                    capSp.Name,
                    new List<string>() { resourceName },
                    null,
                    capSp.ParameterType.ToLiteral(),
                    string.Empty,
                    string.Empty,
                    null,
                    false,
                    string.Empty,
                    string.Empty,
                    string.Empty));

                used.Add(capSp.Name);
            }

            // check for 'query' operations
            foreach (FhirOperation fhirOp in GetResourceFhirOperations(resourceName, FhirOperation.OperationKindCodes.Query))
            {
                // if we have any query operations, make sure _query is added to this resource
                if (!used.Contains("_query"))
                {
                    searchParameters.Add(GetFhirQuerySearchParam(uriBase, resourceName));
                    used.Add("_query");
                }

                foreach (FhirParameter opParam in fhirOp.Parameters)
                {
                    if (!opParam.Use.Equals("in", StringComparison.Ordinal))
                    {
                        continue;
                    }

                    // do not include parameters that do not explicitly state their search type (per spec)
                    if (string.IsNullOrEmpty(opParam.SearchType))
                    {
                        continue;
                    }

                    if (used.Contains(opParam.Name))
                    {
                        continue;
                    }

                    // create a 'local' canonical for referecing
                    searchParameters.Add(new FhirSearchParam(
                        opParam.Name,
                        (fhirOp.URL == null)
                            ? new Uri(uriBase, $"{resourceName}-{fhirOp.Name}-{opParam.Name}")
                            : new Uri(fhirOp.URL, $"-{opParam.Name}"),
                        _info.VersionString,
                        opParam.Name,
                        opParam.Documentation,
                        opParam.Documentation,
                        opParam.Name,
                        new List<string>() { resourceName },
                        null,
                        opParam.SearchType,
                        string.Empty,
                        string.Empty,
                        null,
                        false,
                        string.Empty,
                        string.Empty,
                        string.Empty));

                    used.Add(opParam.Name);
                }
            }

            // exit out since we have everything based on the capability statement
            return searchParameters;
        }

        // if there is no capabilties, just use everything in the package
        searchParameters.AddRange(_info.Resources[resourceName].SearchParameters.Values);

        // check for 'query' operations
        foreach (FhirOperation fhirOp in _info.Resources[resourceName].TypeOperations.Values)
        {
            if (fhirOp.Kind != FhirOperation.OperationKindCodes.Query)
            {
                continue;
            }

            Uri uriBase = (_info.PackageDetails?.URL == null)
                ? new Uri("http://fhir-codegen-local/SearchParameter")
                : _info.PackageDetails.URL;

            // if we have any query operations, make sure _query is added to this resource
            if (!used.Contains("_query"))
            {
                searchParameters.Add(GetFhirQuerySearchParam(uriBase, resourceName));
                used.Add("_query");
            }

            foreach (FhirParameter opParam in fhirOp.Parameters)
            {
                if (!opParam.Use.Equals("in", StringComparison.Ordinal))
                {
                    continue;
                }

                // do not include parameters that do not explicitly state their search type (per spec)
                if (string.IsNullOrEmpty(opParam.SearchType))
                {
                    continue;
                }

                if (used.Contains(opParam.Name))
                {
                    continue;
                }

                // create a 'local' canonical for referecing
                searchParameters.Add(new FhirSearchParam(
                    opParam.Name,
                    (fhirOp.URL == null)
                        ? new Uri(uriBase, $"{resourceName}-{fhirOp.Name}-{opParam.Name}")
                        : new Uri(fhirOp.URL, $"{opParam.Name}"),
                    _info.VersionString,
                    opParam.Name,
                    opParam.Documentation,
                    opParam.Documentation,
                    opParam.Name,
                    new List<string>() { resourceName },
                    null,
                    opParam.SearchType,
                    string.Empty,
                    string.Empty,
                    null,
                    false,
                    string.Empty,
                    string.Empty,
                    string.Empty));

                used.Add(opParam.Name);
            }
        }

        foreach (FhirOperation fhirOp in _info.Resources[resourceName].InstanceOperations.Values)
        {
            if (fhirOp.Kind != FhirOperation.OperationKindCodes.Query)
            {
                continue;
            }

            Uri uriBase = (_info.PackageDetails?.URL == null)
                ? new Uri("http://fhir-codegen-local/SearchParameter")
                : _info.PackageDetails.URL;

            // if we have any query operations, make sure _query is added to this resource
            if (!used.Contains("_query"))
            {
                searchParameters.Add(GetFhirQuerySearchParam(uriBase, resourceName));
                used.Add("_query");
            }

            foreach (FhirParameter opParam in fhirOp.Parameters)
            {
                if (!opParam.Use.Equals("in", StringComparison.Ordinal))
                {
                    continue;
                }

                // do not include parameters that do not explicitly state their search type (per spec)
                if (string.IsNullOrEmpty(opParam.SearchType))
                {
                    continue;
                }

                if (used.Contains(opParam.Name))
                {
                    continue;
                }

                // create a 'local' canonical for referecing
                searchParameters.Add(new FhirSearchParam(
                    opParam.Name,
                    (fhirOp.URL == null)
                        ? new Uri(uriBase, $"{resourceName}-{fhirOp.Name}-{opParam.Name}")
                        : new Uri(fhirOp.URL, $"{opParam.Name}"),
                    _info.VersionString,
                    opParam.Name,
                    opParam.Documentation,
                    opParam.Documentation,
                    opParam.Name,
                    new List<string>() { resourceName },
                    null,
                    opParam.SearchType,
                    string.Empty,
                    string.Empty,
                    null,
                    false,
                    string.Empty,
                    string.Empty,
                    string.Empty));

                used.Add(opParam.Name);
            }
        }

        return searchParameters;
    }

    /// <summary>Gets the system FHIR operations in this collection.</summary>
    /// <param name="kindFilter">(Optional) A filter specifying the kind.</param>
    /// <returns>
    /// An enumerator that allows foreach to be used to process the system FHIR operations in this
    /// collection.
    /// </returns>
    private IEnumerable<FhirOperation> GetSystemFhirOperations(
        FhirOperation.OperationKindCodes? kindFilter = null)
    {
        List<FhirOperation> operations = new();

        if (_caps != null)
        {
            foreach (FhirCapOperation capOp in _caps.ServerOperations.Values)
            {
                if (!FhirManager.Current.TryResolveCanonical(_info.FhirSequence, capOp.DefinitionCanonical, _exporterOptions.ResolveExternal, out FhirArtifactClassEnum ac, out object fhirOpObj))
                {
                    continue;
                }

                if (!(fhirOpObj is FhirOperation fhirOp))
                {
                    continue;
                }

                if (!fhirOp.DefinedOnSystem)
                {
                    continue;
                }

                if ((kindFilter == null) ||
                    (kindFilter == fhirOp.Kind))
                {
                    operations.Add(fhirOp);
                }
            }

            return operations;
        }

        // if there is no capabilties, just check everything in the package
        foreach (FhirOperation fhirOp in _info.OperationsByUrl.Values)
        {
            if (!fhirOp.DefinedOnSystem)
            {
                continue;
            }

            if ((kindFilter == null) ||
                (kindFilter == fhirOp.Kind))
            {
                operations.Add(fhirOp);
            }
        }

        return operations;
    }

    /// <summary>Builds read operation.</summary>
    /// <param name="resourceName">Name of the resource.</param>
    /// <returns>An OpenApiOperation.</returns>
    private OpenApiOperation BuildReadOperation(
        string resourceName,
        Dictionary<string, OpenApiSchema> schemas)
    {
        OpenApiOperation operation = new OpenApiOperation()
        {
            OperationId = FhirUtils.ToConvention("Get.Metadata", string.Empty, _openApiOptions.IdConvention),
            Summary =
                _openApiOptions.IncludeSummaries
                ? "Read metadata information about this server."
                : null,
            Description =
                _openApiOptions.IncludeDescriptions
                ? "Read metadata."
                : null,
        };

        operation.Responses = BuildResponses(new[] { 200 }, resourceName, schemas);

        return operation;
    }

    /// <summary>Builds a content map.</summary>
    /// <param name="resourceName">Name of the resource.</param>
    /// <param name="wrapInBundle">True to wrap in bundle.</param>
    /// <returns>A Dictionary of MIME Types and matching ApiOpenMeidaTypes.</returns>
    private Dictionary<string, OpenApiMediaType> BuildContentMap(
        string resourceName,
        Dictionary<string, OpenApiSchema> schemas)
    {
        Dictionary<string, OpenApiMediaType> mediaTypes = new();

        string name = schemas.ContainsKey(resourceName) ? resourceName : "Resource";

        if (_fhirJson)
        {
            mediaTypes.Add(
                "application/fhir+json",
                OasMediaForResource(name, schemas));
        }

        if (_fhirXml)
        {
            mediaTypes.Add(
                "application/fhir+xml",
                OasMediaForResource(name, schemas));
        }

        if (_fhirTurtle)
        {
            mediaTypes.Add(
                "application/fhir+turtle",
                OasMediaForResource(name, schemas));
        }

        return mediaTypes;
    }

    /// <summary>Oas media for resource.</summary>
    /// <param name="name">   The name.</param>
    /// <param name="schemas">The schemas.</param>
    /// <returns>An OpenApiMediaType.</returns>
    private OpenApiMediaType OasMediaForResource(string name, Dictionary<string, OpenApiSchema> schemas)
    {
        if (string.IsNullOrEmpty(name))
        {
            return new OpenApiMediaType()
            {
                Schema = new OpenApiSchema() { Type = "object", },
            };
        }

        if (_openApiOptions.SchemaStyle == OaSchemaStyleCodes.Inline)
        {
            return new OpenApiMediaType()
            {
                Schema = schemas[name],
            };
        }

        return new OpenApiMediaType()
        {
            Schema = new OpenApiSchema()
            {
                Reference = new OpenApiReference()
                {
                    Id = BuildTypeNameFromPath(name),
                    Type = ReferenceType.Schema,
                },
                Type = "object",
            },
        };
    }

    /// <summary>Builds content map for patch.</summary>
    /// <param name="resourceName">Name of the resource.</param>
    /// <returns>A dictionary of OpenApiMediaType records, keyed by MIME type.</returns>
    private Dictionary<string, OpenApiMediaType> BuildContentMapForPatch(
        string resourceName,
        Dictionary<string, OpenApiSchema> schemas)
    {
        Dictionary<string, OpenApiMediaType> mediaTypes = new Dictionary<string, OpenApiMediaType>();

        string name = schemas.ContainsKey(resourceName) ? resourceName : "Resource";

        if (_patchJson)
        {
            mediaTypes.Add(
                "application/json-patch+json",
                OasMediaForResource(name, schemas));
        }

        if (_patchXml)
        {
            mediaTypes.Add(
                "application/xml-patch+xml",
                OasMediaForResource(name, schemas));
        }

        if (_patchFhirJson)
        {
            mediaTypes.Add(
                "application/fhir+json",
                OasMediaForResource(name, schemas));
        }

        if (_patchFhirXml)
        {
            mediaTypes.Add(
                "application/fhir+xml",
                OasMediaForResource(name, schemas));
        }

        return mediaTypes;
    }

    /// <summary>Gets the filtered resouces in this collection.</summary>
    /// <param name="sort">(Optional) True to sort.</param>
    /// <returns>
    /// An enumerator that allows foreach to be used to process the filtered resouces in this
    /// collection.
    /// </returns>
    private IEnumerable<FhirComplex> GetFilteredResouces(bool sort = false)
    {
        if (sort)
        {
            _info.Resources.Values.Where(c => IncludeResouce(c.Name)).OrderBy(c => c.Name);
        }

        return _info.Resources.Values.Where(c => IncludeResouce(c.Name));
    }

    /// <summary>Determine if we should filter out resource.</summary>
    /// <param name="resourceName">Name of the resource.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool IncludeResouce(string resourceName)
    {
        if (resourceName.Equals("Resource", StringComparison.Ordinal))
        {
            // always include 'resource' for schema
            return true;
        }

        if ((_caps != null) &&
            (_caps.ResourceInteractions != null) &&
            (!_caps.ResourceInteractions.ContainsKey(resourceName)))
        {
            // do not include
            return false;
        }

        if (_useExportKeyFilter && (!_exportFilterKeys.Contains(resourceName)))
        {
            // do not include
            return false;
        }

        // include
        return true;
    }

    /// <summary>Builds the servers.</summary>
    /// <returns>A List&lt;OpenApiServer&gt;</returns>
    private List<OpenApiServer> BuildServers()
    {
        if (_caps == null)
        {
            return null;
        }

        string description;

        if (!string.IsNullOrEmpty(_caps.ImplementationDescription))
        {
            description = SanitizeDescription(_caps.ImplementationDescription);
        }
        else if (!string.IsNullOrEmpty(_caps.SoftwareName))
        {
            description = SanitizeDescription(_caps.SoftwareName);
        }
        else
        {
            description = $"FHIR Server Version: {_info.VersionString}";
        }

        return new List<OpenApiServer>()
            {
                new OpenApiServer()
                {
                    Url = _caps.Url,
                    Description = description,
                },
            };
    }

    /// <summary>Builds document information.</summary>
    /// <param name="caps">The capabilities.</param>
    /// <returns>An OpenApiInfo.</returns>
    private OpenApiInfo BuildDocumentInfo()
    {
        string title;

        if (!string.IsNullOrEmpty(_openApiOptions.Title))
        {
            title = _openApiOptions.Title;
        }
        else if (!string.IsNullOrEmpty(_caps?.Title ?? null))
        {
            title = _caps.Title;
        }
        else if (!string.IsNullOrEmpty(_caps?.SoftwareName ?? null))
        {
            title = _caps.SoftwareName;
        }
        else
        {
            title = $"FHIR {_info.FhirSequence}:{_info.VersionString}";
        }

        string version;

        if (!string.IsNullOrEmpty(_openApiOptions.Version))
        {
            version = _openApiOptions.Version;
        }
        else if (!string.IsNullOrEmpty(_caps?.SoftwareVersion ?? null))
        {
            version = _caps.SoftwareVersion;
        }
        else if (!string.IsNullOrEmpty(_caps?.FhirVersion ?? null))
        {
            version = _caps.FhirVersion;
        }
        else
        {
            version = _info.VersionString;
        }

        return new OpenApiInfo()
        {
            Version = version,
            Title = title,
        };
    }

    /// <summary>Builds the schemas.</summary>
    /// <param name="caps">The capabilities.</param>
    /// <returns>A filled out schema dictionary for the requested models.</returns>
    private Dictionary<string, OpenApiSchema> BuildSchemas()
    {
        Dictionary<string, OpenApiSchema> schemas = new();

        switch (_openApiOptions.SchemaLevel)
        {
            case OaSchemaLevelCodes.None:
                {
                    schemas.Add(
                        FhirUtils.ToConvention("resource", string.Empty, _openApiOptions.IdConvention),
                        new OpenApiSchema()
                        {
                            Type = "object",
                            Description =
                                _openApiOptions.IncludeDescriptions
                                ? "A FHIR Resource"
                                : null,
                        });
                }
                break;

            case OaSchemaLevelCodes.Names:
                {
                    foreach (FhirComplex complex in GetFilteredResouces(true))
                    {
                        string name = BuildTypeNameFromPath(complex.Path);

                        if (schemas.ContainsKey(name))
                        {
                            continue;
                        }

                        OpenApiSchema schema = new OpenApiSchema()
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>(),
                            Description =
                                _openApiOptions.IncludeDescriptions
                                ? SanitizeDescription(complex.ShortDescription)
                                : null,
                        };

                        schemas.Add(name, schema);
                    }
                }
                break;

            case OaSchemaLevelCodes.Detailed:
                {
                    if (_openApiOptions.SchemaStyle == OaSchemaStyleCodes.Inline)
                    {
                        foreach (FhirComplex complex in GetFilteredResouces(true))
                        {
                            string name = BuildTypeNameFromPath(complex.Path);

                            if (schemas.ContainsKey(name))
                            {
                                continue;
                            }

                            OpenApiSchema schema = new OpenApiSchema();
                            int schemaElementCount = 0;

                            Dictionary<string, int> pathReferenceCounts = new Dictionary<string, int>();
                            BuildInlineSchemaRecurse(
                                ref schema,
                                ref schemaElementCount,
                                complex,
                                null,
                                name,
                                pathReferenceCounts,
                                string.Empty,
                                0);
                        }
                    }

                    if (_openApiOptions.SchemaStyle == OaSchemaStyleCodes.References)
                    {
                        foreach (FhirComplex complex in _info.ComplexTypes.Values.OrderBy(c => c.Name))
                        {
                            BuildSchema(
                                schemas,
                                complex,
                                null,
                                false);
                        }

                        foreach (FhirComplex complex in GetFilteredResouces(true))
                        {
                            string name = BuildTypeNameFromPath(complex.Path);

                            if (schemas.ContainsKey(name))
                            {
                                continue;
                            }

                            BuildSchema(
                                schemas,
                                complex,
                                null,
                                true);
                        }
                    }
                }
                break;
        }

        return schemas;
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
                    _openApiOptions.IncludeDescriptions
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

        if ((_openApiOptions.MaxRecursions > 0) && (currentDepth > _openApiOptions.MaxRecursions))
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
                        Description = _openApiOptions.IncludeDescriptions ? "Resource type name" : null,
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

    /// <summary>Determine if we should add extension.</summary>
    /// <param name="path">      Full pathname of the file.</param>
    /// <param name="parentPath">Path to the parent element.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool ShouldAddExtension(
        string path,
        string parentPath)
    {
        switch (_exporterOptions.ExtensionSupport)
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

    /// <summary>Skip as uncommon.</summary>
    /// <param name="element">The element.</param>
    /// <param name="parent"> The parent.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool SkipAsUncommon(
        FhirElement element,
        FhirComplex parent)
    {
        if (!_openApiOptions.RemoveUncommonFields)
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
                _openApiOptions.IncludeDescriptions
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

        if (_openApiOptions.RemoveUncommonFields &&
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
                        _openApiOptions.IncludeDescriptions
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

            if (_openApiOptions.ExpandReferences && (elementType.TypeProfiles.Count == 1))
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

        OpenApiSchema schema = new OpenApiSchema()
        {
            Type = "object",
            Properties = new Dictionary<string, OpenApiSchema>(),
            Description =
                _openApiOptions.IncludeDescriptions
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
                    Description = _openApiOptions.IncludeDescriptions ? "Resource type name" : null,
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
                _openApiOptions.IncludeDescriptions
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
                        _openApiOptions.IncludeDescriptions
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

            if (_openApiOptions.ExpandProfiles && (type == "Resource"))
            {
                // need versions higher than 2 for 'one of' support
                if (_openApiOptions.OpenApiVersion != Microsoft.OpenApi.OpenApiSpecVersion.OpenApi2_0)
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
                else
                {
                    SetSchemaType(type, ref schema, element.IsArray);
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


    /// <summary>Sanitize description.</summary>
    /// <param name="value">The value.</param>
    /// <returns>A string.</returns>
    private string SanitizeDescription(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return _openApiOptions.DescriptionValidation ? "N/A." : string.Empty;
        }

        if (!_openApiOptions.DescriptionValidation)
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

        if (value.Length > _openApiOptions.DescriptionMaxLen)
        {
            value = value.Substring(0, _openApiOptions.DescriptionMaxLen - 3) + "...";
        }

        return value;
    }
}
