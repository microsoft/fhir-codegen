// <copyright file="ModelBuilder.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using System.IO;
using System.Net;
using System.Numerics;
using System.Xml.Linq;
using fhirCsR2.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.SpecManager.Manager;
using Microsoft.OpenApi.Models;
using static Microsoft.Health.Fhir.CodeGenCommon.Models.FhirCapResource;
using static Microsoft.Health.Fhir.SpecManager.Language.OpenApi.OpenApiCommon;

namespace Microsoft.Health.Fhir.SpecManager.Language.OpenApi;

/// <summary>An OpenAPI model builder - convert internal (normalized) models into OpenAPI-specific ones.</summary>
public class ModelBuilder
{
    public readonly record struct ExportPath(
        FhirCapResource.FhirInteractionCodes FhirInteraction,
        string Path,
        OperationType HttpOpType,
        string Id,
        string Summary,
        string Description,
        List<string> PathParameterIds,
        List<string> QueryParameterIds,
        List<string> BodyParameterIds,
        bool AddCommonSearchParameters,
        string ReturnType,
        bool BodyIsRequired,
        string BodyResourceType,
        bool WrapBodyInBundle);

    public readonly record struct ExportResource(
        string Name,
        List<string> ParameterIds,
        Dictionary<FhirCapResource.FhirInteractionCodes, ExportPath> FhirInteractionPathsByInteraction,
        Dictionary<string, ExportPath> OperationPathsByCode);

    public readonly record struct ExportParameter(
        string Id,
        string Canonical,
        string Code,
        string Description,
        OpenApiSchema Schema);

    public readonly record struct ExportModels(
        Dictionary<string, OpenApiSchema> SchemasByName,
        Dictionary<string, ExportResource> ResourcesByName,
        Dictionary<string, ExportPath> OperationPathsByCode,
        List<string> AllResourceQueryParameterIds,
        List<string> AllResourceBodyParameterIds,
        Dictionary<string, ExportParameter> PathParametersById,
        Dictionary<string, ExportParameter> QueryParametersById,
        Dictionary<string, ExportParameter> BodyParametersById);

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

    private bool _patchJson = false;
    private bool _patchXml = false;
    private bool _patchFhirJson = false;
    private bool _patchFhirXml = false;

    private bool _searchGet = false;
    private bool _searchPost = false;

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

        Dictionary<string, OpenApiSchema> schemas = BuildSchemas();

        if (_openApiOptions.SchemaStyle != OaSchemaStyleCodes.Inline)
        {
            doc.Components.Schemas = schemas;
        }

        Dictionary<string, OpenApiTag> tags = new();
        tags.Add("System", new OpenApiTag() { Name = "System", Description = "Sever-level requests"});

        doc.Paths = BuildPaths(schemas, tags);

        doc.Tags = tags.Values.ToList();

        return doc;
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
        }
        else if (_openApiOptions.FhirMime == OaFhirMimeCodes.Both)
        {
            _fhirJson = true;
            _fhirXml = true;
        }
        else
        {
            _fhirJson = _openApiOptions.FhirMime == OaFhirMimeCodes.FhirJson;
            _fhirXml = _openApiOptions.FhirMime == OaFhirMimeCodes.FhirXml;
        }

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
    /// <param name="schemas">The schemas.</param>
    /// <param name="tags">   The tags.</param>
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
            }

            // TODO(ginoc): Need to add options for CapabilityStatement operations - and figure out how they are applied
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
                            opPath = $"/{resource.Name}/{{id}}";
                            if (!paths.ContainsKey(opPath))
                            {
                                paths.Add(opPath, new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() });
                                AddCommonHttpParameters(paths[opPath]);
                            }

                            paths[opPath].AddOperation(OperationType.Get, BuildResourceReadOasOp(resource, schemas));
                        }
                        break;

                    case FhirInteractionCodes.VRead:
                        {
                            opPath = $"/{resource.Name}/{{id}}/_history/{{vid}}";
                            if (!paths.ContainsKey(opPath))
                            {
                                paths.Add(opPath, new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() });
                                AddCommonHttpParameters(paths[opPath]);
                            }

                            paths[opPath].AddOperation(OperationType.Get, BuildResourceVReadOasOp(resource, schemas));
                        }
                        break;

                    case FhirInteractionCodes.Update:
                        {
                            opPath = $"/{resource.Name}/{{id}}";
                            if (!paths.ContainsKey(opPath))
                            {
                                paths.Add(opPath, new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() });
                                AddCommonHttpParameters(paths[opPath]);
                            }

                            paths[opPath].AddOperation(OperationType.Put, BuildResourceUpdateOasOp(resource, schemas));
                        }
                        break;

                    case FhirInteractionCodes.Patch:
                        {
                            opPath = $"/{resource.Name}/{{id}}";
                            if (!paths.ContainsKey(opPath))
                            {
                                paths.Add(opPath, new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() });
                                AddCommonHttpParameters(paths[opPath]);
                            }

                            paths[opPath].AddOperation(OperationType.Patch, BuildResourcePatchOasOp(resource, schemas));
                        }
                        break;

                    case FhirInteractionCodes.Delete:
                        {
                            opPath = $"/{resource.Name}/{{id}}";
                            if (!paths.ContainsKey(opPath))
                            {
                                paths.Add(opPath, new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() });
                                AddCommonHttpParameters(paths[opPath]);
                            }

                            paths[opPath].AddOperation(OperationType.Patch, BuildResourceDeleteOasOp(resource, schemas));
                        }
                        break;

                    case FhirInteractionCodes.HistoryInstance:
                        {
                            if (!_openApiOptions.IncludeHistory)
                            {
                                continue;
                            }

                            opPath = $"/{resource.Name}/{{id}}/_history";
                            if (!paths.ContainsKey(opPath))
                            {
                                paths.Add(opPath, new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() });
                                AddCommonHttpParameters(paths[opPath]);
                            }

                            paths[opPath].AddOperation(OperationType.Get, BuildResourceHistoryInstanceOasOp(resource, schemas));
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
                            }
                        }
                        break;

                    //case FhirInteraction.Search:
                    default:
                        break;

                }   // close: foreach interaction


            }       // close: foreach resource

            // traverse the operations for this resource
            foreach (FhirOperation fhirOp in GetResourceFhirOperations(resource.Name))
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

                paths[opPath].AddOperation(OperationType.Get, BuildResourceSearchSystemGetOasOp(schemas));
            }

            if (_searchPost)
            {
                opPath = "/_search";
                if (!paths.ContainsKey(opPath))
                {
                    paths.Add(opPath, new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() });
                    AddCommonHttpParameters(paths[opPath]);
                }

                paths[opPath].AddOperation(OperationType.Post, BuildResourceSearchSystemPostOasOp(schemas));
            }
        }

        // TODO(ginoc): need to check system conditional delete


        // handle system-level operations
        foreach (FhirOperation fhirOp in GetSystemFhirOperations())
        {
            BuildSystemOperationOasPaths(paths, schemas, fhirOp);
        }

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
        // TODO(ginoc): need to find the correct path and add appropriate parameters (input parameters of the operation)
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
        // TODO(ginoc): need to find the correct path and add appropriate parameters (input parameters of the operation)
        if (fhirOp.Kind == FhirOperation.OperationKindCodes.Query)
        {
            return;
        }

        string opPath;

        if (fhirOp.DefinedOnInstance)
        {
            if (TryBuildOperationGetOasOp(resource, fhirOp, schemas, OaOpLevelCodes.Instance, out OpenApiOperation oasOpGet))
            {
                opPath = $"/{resource.Name}/{{id}}/${fhirOp.Code}";
                if (!paths.ContainsKey(opPath))
                {
                    paths.Add(opPath, new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() });
                    AddCommonHttpParameters(paths[opPath]);
                }

                // operations may get listed a few times, make sure we only add them once in each path
                if (!paths[opPath].Operations.ContainsKey(OperationType.Get))
                {
                    paths[opPath].AddOperation(OperationType.Get, oasOpGet);
                }
            }

            if (TryBuildOperationPostOasOp(resource, fhirOp, schemas, OaOpLevelCodes.Instance, out OpenApiOperation oasOpPost))
            {
                opPath = $"/{resource.Name}/{{id}}/${fhirOp.Code}";
                if (!paths.ContainsKey(opPath))
                {
                    paths.Add(opPath, new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() });
                    AddCommonHttpParameters(paths[opPath]);
                }

                // operations may get listed a few times, make sure we only add them once in each path
                if (!paths[opPath].Operations.ContainsKey(OperationType.Post))
                {
                    paths[opPath].AddOperation(OperationType.Post, oasOpPost);
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
            oasOp.Summary = fhirOp.Description;
        }

        if (opLevel == OaOpLevelCodes.Instance)
        {
            // instance level includes {id} segment
            oasOp.Parameters.Add(BuildReferencedParameter("id"));
        }

        HashSet<string> usedParams = new();

        // search for input parameters
        foreach (FhirParameter fhirParam in fhirOp.Parameters.Where(fp => fp.Use.Equals("in", StringComparison.Ordinal)))
        {
            // skip duplicates
            if (usedParams.Contains(fhirParam.Name))
            {
                continue;
            }

            // query parameters can only be 'simple' types
            if (!_info.PrimitiveTypes.ContainsKey(fhirParam.ValueType))
            {
                continue;
            }

            oasOp.Parameters.Add(BuildStringParameter(fhirParam.Name, fhirParam.Documentation));
            usedParams.Add(fhirParam.Name);
        }

        FhirParameter[] outParams = fhirOp.Parameters.Where(fp => fp.Use.Equals("out", StringComparison.Ordinal)).ToArray();

        if (outParams.Length == 1)
        {
            if (_info.Resources.ContainsKey(outParams[0].ValueType))
            {
                oasOp.Responses = BuildResponses(_responseCodesRead, outParams[0].ValueType, schemas);
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
            oasOp.Summary = fhirOp.Description;
        }

        if (opLevel == OaOpLevelCodes.Instance)
        {
            // instance level includes {id} segment
            oasOp.Parameters.Add(BuildReferencedParameter("id"));
        }

        HashSet<string> usedParams = new();

        FhirParameter[] inParams = fhirOp.Parameters.Where(fp => fp.Use.Equals("in", StringComparison.Ordinal)).ToArray();

        FhirParameter[] simpleInParams = inParams.Where(fp => _info.PrimitiveTypes.ContainsKey(fp.ValueType)).ToArray();
        FhirParameter[] resourceInParams = inParams.Where(fp => _info.Resources.ContainsKey(fp.ValueType)).ToArray();

        // check for a single resource parameter and all others are 'simple' (primitive) types
        if ((resourceInParams.Length == 1) &&
            (simpleInParams.Length + resourceInParams.Length) == inParams.Length)
        {
            oasOp.RequestBody = new OpenApiRequestBody()
            {
                Required = true,
                Content = BuildContentMap(resourceInParams[0].ValueType, schemas),
                Description = _openApiOptions.IncludeDescriptions
                    ? resourceInParams[0].Documentation
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

        FhirParameter[] outParams = fhirOp.Parameters.Where(fp => fp.Use.Equals("out", StringComparison.Ordinal)).ToArray();

        if (outParams.Length == 1)
        {
            if (_info.Resources.ContainsKey(outParams[0].ValueType))
            {
                oasOp.Responses = BuildResponses(_responseCodesRead, outParams[0].ValueType, schemas);
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
        }
    }

    /// <summary>Gets body parameter type.</summary>
    /// <param name="code">        The code.</param>
    /// <param name="resourceName">(Optional) Name of the resource.</param>
    /// <returns>The body parameter type.</returns>
    private string GetBodyParamType(string code, string resourceName = "")
    {
        if (string.IsNullOrEmpty(code))
        {
            return "string";
        }

        if ((!string.IsNullOrEmpty(resourceName)) &&
            (_caps?.ResourceInteractions?.ContainsKey(resourceName) ?? false) &&
            (_caps.ResourceInteractions[resourceName].SearchParameters?.ContainsKey(code) ?? false))
        {
            return _caps.ResourceInteractions[resourceName].SearchParameters[code].ParameterType == FhirCapSearchParam.SearchParameterType.Number
                ? "number"
                : "string";
        }

        if (_caps?.ServerSearchParameters?.ContainsKey(code) ?? false)
        {
            return _caps.ServerSearchParameters[code].ParameterType == FhirCapSearchParam.SearchParameterType.Number
                ? "number"
                : "string";
        }

        if (_httpReadParameters.ContainsKey(code))
        {
            return _httpReadParameters[code].Schema.Type;
        }

        if (_httpCommonParameters.ContainsKey(code))
        {
            return _httpCommonParameters[code].Schema.Type;
        }

        if (_searchCommonParameters.ContainsKey(code))
        {
            return _searchCommonParameters[code].Schema.Type;
        }

        if (_searchResultParameters.ContainsKey(code))
        {
            return _searchResultParameters[code].Schema.Type;
        }

        if (_searchRootParameters.ContainsKey(code))
        {
            return _searchRootParameters[code].Schema.Type;
        }

        if (_pathParameters.ContainsKey(code))
        {
            return _pathParameters[code].Schema.Type;
        }

        return "string";
    }

    /// <summary>Builds resource search system post oas operation.</summary>
    /// <param name="schemas">The schemas.</param>
    /// <returns>An OpenApiOperation.</returns>
    private OpenApiOperation BuildResourceSearchSystemPostOasOp(
        Dictionary<string, OpenApiSchema> schemas)
    {
        OpenApiOperation oasOp = new();

        string id = FhirUtils.ToConvention("Search.System.Post", string.Empty, _openApiOptions.IdConvention);

        oasOp.OperationId = id;

        oasOp.Tags.Add(new OpenApiTag() { Reference = new OpenApiReference() { Id = "System", Type = ReferenceType.Tag, } });

        if (_openApiOptions.IncludeSummaries)
        {
            oasOp.Summary = "Search the entire FHIR server";
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
                    Type = GetBodyParamType(code),
                });
            usedParams.Add(code);
        }

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
                    Type = GetBodyParamType(code),
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
                    Type = GetBodyParamType(code),
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

                string advertisedType = capParam?.ParameterType.ToLiteral() ?? string.Empty;

                if (string.IsNullOrEmpty(advertisedType))
                {
                    advertisedType = GetBodyParamType(capParam.Name);
                }

                oasOp.RequestBody.Content["application/x-www-form-urlencoded"].Schema.Properties.Add(
                    capParam.Name,
                    new OpenApiSchema()
                    {
                        Title = capParam.Name,
                        Type = advertisedType,
                    });
                usedParams.Add(capParam.Name);
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

        string id = FhirUtils.ToConvention("Search.Type.Post." + resource.Name, string.Empty, _openApiOptions.IdConvention);

        oasOp.OperationId = id;

        oasOp.Tags.Add(new OpenApiTag() { Reference = new OpenApiReference() { Id = resource.Name, Type = ReferenceType.Tag, } });

        //if (_openApiOptions.IncludeDescriptions)
        //{
        //    oasOp.Description = "Search across all FHIR " + resource.Name + ": " + resource.ShortDescription;
        //}

        if (_openApiOptions.IncludeSummaries)
        {
            oasOp.Summary = $"Search all FHIR {resource.Name}";
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
                    Type = GetBodyParamType(code, resource.Name),
                });
            usedParams.Add(code);
        }

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
                    Type = GetBodyParamType(code, resource.Name),
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
                    Type = GetBodyParamType(code, resource.Name),
                });
            usedParams.Add(code);
        }

        if ((_caps != null) &&
            (_caps.ResourceInteractions != null) &&
            _caps.ResourceInteractions.ContainsKey(resource.Name))
        {
            foreach (FhirCapSearchParam capParam in _caps.ResourceInteractions[resource.Name].SearchParameters.Values)
            {
                if (usedParams.Contains(capParam.Name))
                {
                    continue;
                }

                string advertisedType = capParam?.ParameterType.ToLiteral() ?? string.Empty;

                if (string.IsNullOrEmpty(advertisedType))
                {
                    advertisedType = GetBodyParamType(capParam.Name, resource.Name);
                }

                oasOp.RequestBody.Content["application/x-www-form-urlencoded"].Schema.Properties.Add(
                    capParam.Name,
                    new OpenApiSchema()
                    {
                        Title = capParam.Name,
                        Type = advertisedType,
                    });
                usedParams.Add(capParam.Name);
            }
        }

        oasOp.Responses = BuildResponses(_responseCodesRead, "Bundle", schemas);

        return oasOp;
    }

    /// <summary>Builds resource search system get oas operation.</summary>
    /// <param name="schemas">The schemas.</param>
    /// <returns>An OpenApiOperation.</returns>
    private OpenApiOperation BuildResourceSearchSystemGetOasOp(
        Dictionary<string, OpenApiSchema> schemas)
    {
        OpenApiOperation oasOp = new();

        string id = FhirUtils.ToConvention("Search.System.Get", string.Empty, _openApiOptions.IdConvention);

        oasOp.OperationId = id;

        oasOp.Tags.Add(new OpenApiTag() { Reference = new OpenApiReference() { Id = "System", Type = ReferenceType.Tag, } });

        if (_openApiOptions.IncludeSummaries)
        {
            oasOp.Summary = "System-level search";
        }

        HashSet<string> usedParams = new();

        foreach (string code in _openApiOptions.HttpReadParams)
        {
            if (usedParams.Contains(code))
            {
                continue;
            }

            oasOp.Parameters.Add(BuildReferencedParameter(code));
            usedParams.Add(code);
        }

        foreach (string code in _openApiOptions.SearchCommonParams)
        {
            if (usedParams.Contains(code))
            {
                continue;
            }

            oasOp.Parameters.Add(BuildReferencedParameter(code));
            usedParams.Add(code);
        }

        foreach (string code in _openApiOptions.SearchResultParams)
        {
            if (usedParams.Contains(code))
            {
                continue;
            }

            oasOp.Parameters.Add(BuildReferencedParameter(code));
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

                oasOp.Parameters.Add(BuildStringParameter(capParam.Name, capParam.Documentation));
                usedParams.Add(capParam.Name);
            }
        }

        oasOp.Responses = BuildResponses(_responseCodesRead, "Bundle", schemas);

        return oasOp;
    }

    /// <summary>Builds resource search type oas operation.</summary>
    /// <param name="resource">The resource.</param>
    /// <param name="schemas"> The schemas.</param>
    /// <returns>An OpenApiOperation.</returns>
    private OpenApiOperation BuildResourceSearchTypeGetOasOp(
        FhirComplex resource,
        Dictionary<string, OpenApiSchema> schemas)
    {
        OpenApiOperation oasOp = new();

        string id = FhirUtils.ToConvention("Search.Type.Get." + resource.Name, string.Empty, _openApiOptions.IdConvention);

        oasOp.OperationId = id;

        oasOp.Tags.Add(new OpenApiTag() { Reference = new OpenApiReference() { Id = resource.Name, Type = ReferenceType.Tag, } });

        //if (_openApiOptions.IncludeDescriptions)
        //{
        //    oasOp.Description = "Search across all FHIR " + resource.Name + ": " + resource.ShortDescription;
        //}

        if (_openApiOptions.IncludeSummaries)
        {
            oasOp.Summary = $"Search all FHIR {resource.Name}";
        }

        HashSet<string> usedParams = new();

        foreach (string code in _openApiOptions.HttpReadParams)
        {
            if (usedParams.Contains(code))
            {
                continue;
            }

            oasOp.Parameters.Add(BuildReferencedParameter(code));
            usedParams.Add(code);
        }

        foreach (string code in _openApiOptions.SearchCommonParams)
        {
            if (usedParams.Contains(code))
            {
                continue;
            }

            oasOp.Parameters.Add(BuildReferencedParameter(code));
            usedParams.Add(code);
        }

        foreach (string code in _openApiOptions.SearchResultParams)
        {
            if (usedParams.Contains(code))
            {
                continue;
            }

            oasOp.Parameters.Add(BuildReferencedParameter(code));
            usedParams.Add(code);
        }

        if ((_caps != null) &&
            (_caps.ResourceInteractions != null) &&
            _caps.ResourceInteractions.ContainsKey(resource.Name))
        {
            foreach (FhirCapSearchParam capParam in _caps.ResourceInteractions[resource.Name].SearchParameters.Values)
            {
                if (usedParams.Contains(capParam.Name))
                {
                    continue;
                }

                oasOp.Parameters.Add(BuildStringParameter(capParam.Name, capParam.Documentation));
                usedParams.Add(capParam.Name);
            }
        }

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

        //if (_openApiOptions.IncludeDescriptions)
        //{
        //    oasOp.Description = "Create a FHIR " + resource.Name + ": " + resource.ShortDescription;
        //}

        if (_openApiOptions.IncludeSummaries)
        {
            oasOp.Summary = $"Create a FHIR {resource.Name}";
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

        //if (_openApiOptions.IncludeDescriptions)
        //{
        //    oasOp.Description = "Get the version history of all FHIR " + resource.Name + ": " + resource.ShortDescription;
        //}

        if (_openApiOptions.IncludeSummaries)
        {
            oasOp.Summary = $"Get the history of all FHIR {resource.Name}";
        }

        foreach (string code in _openApiOptions.HttpReadParams)
        {
            oasOp.Parameters.Add(BuildReferencedParameter(code));
        }

        foreach (string code in _openApiOptions.HistoryParams)
        {
            oasOp.Parameters.Add(BuildReferencedParameter(code));
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

        //if (_openApiOptions.IncludeDescriptions)
        //{
        //    oasOp.Description = "Get the version history of a specific FHIR " + resource.Name + ": " + resource.ShortDescription;
        //}

        if (_openApiOptions.IncludeSummaries)
        {
            oasOp.Summary = $"Get the history of a FHIR {resource.Name}";
        }

        // history instance includes {id} segment
        oasOp.Parameters.Add(BuildReferencedParameter("id"));

        foreach (string code in _openApiOptions.HttpReadParams)
        {
            oasOp.Parameters.Add(BuildReferencedParameter(code));
        }

        foreach (string code in _openApiOptions.HistoryParams)
        {
            oasOp.Parameters.Add(BuildReferencedParameter(code));
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
        OpenApiOperation oasOp = new();

        string id = FhirUtils.ToConvention("Delete." + resource.Name, string.Empty, _openApiOptions.IdConvention);

        oasOp.OperationId = id;

        oasOp.Tags.Add(new OpenApiTag() { Reference = new OpenApiReference() { Id = resource.Name, Type = ReferenceType.Tag, } });

        if (_openApiOptions.IncludeSummaries)
        {
            oasOp.Summary = $"Delete a FHIR {resource.Name}";
        }

        // delete includes {id} segment
        oasOp.Parameters.Add(BuildReferencedParameter("id"));

        // conditional deletes take search parameters
        if (ResolveConditiaonlDelete(resource.Name) != ConditionalDeletePolicy.NotSupported)
        {
            HashSet<string> usedParams = new();

            foreach (string code in _openApiOptions.SearchCommonParams)
            {
                if (usedParams.Contains(code))
                {
                    continue;
                }

                oasOp.Parameters.Add(BuildReferencedParameter(code));
                usedParams.Add(code);
            }

            if ((_caps != null) &&
                (_caps.ResourceInteractions != null) &&
                _caps.ResourceInteractions.ContainsKey(resource.Name))
            {
                foreach (FhirCapSearchParam capParam in _caps.ResourceInteractions[resource.Name].SearchParameters.Values)
                {
                    if (usedParams.Contains(capParam.Name))
                    {
                        continue;
                    }

                    oasOp.Parameters.Add(BuildStringParameter(capParam.Name, capParam.Documentation));
                    usedParams.Add(capParam.Name);
                }
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
            oasOp.Summary = $"Patch a FHIR {resource.Name}";
        }

        // patch includes {id} segment
        oasOp.Parameters.Add(BuildReferencedParameter("id"));

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

        string id;

        if (ResolveUpdateCreate(resource.Name))
        {
            id = FhirUtils.ToConvention("UpdateOrCreate." + resource.Name, string.Empty, _openApiOptions.IdConvention);

            if (_openApiOptions.IncludeSummaries)
            {
                oasOp.Summary = $"Update or Create a FHIR {resource.Name}";
            }
        }
        else
        {
            id = FhirUtils.ToConvention("Update." + resource.Name, string.Empty, _openApiOptions.IdConvention);

            if (_openApiOptions.IncludeSummaries)
            {
                oasOp.Summary = $"Update a FHIR {resource.Name}";
            }
        }

        oasOp.OperationId = id;

        oasOp.Tags.Add(new OpenApiTag() { Reference = new OpenApiReference() { Id = resource.Name, Type = ReferenceType.Tag, } });

        // update includes {id} segment
        oasOp.Parameters.Add(BuildReferencedParameter("id"));

        // conditional updates take search parameters
        if (ResolveConditionalUpdate(resource.Name))
        {
            HashSet<string> usedParams = new();

            foreach (string code in _openApiOptions.SearchCommonParams)
            {
                if (usedParams.Contains(code))
                {
                    continue;
                }

                oasOp.Parameters.Add(BuildReferencedParameter(code));
                usedParams.Add(code);
            }

            if ((_caps != null) &&
                (_caps.ResourceInteractions != null) &&
                _caps.ResourceInteractions.ContainsKey(resource.Name))
            {
                foreach (FhirCapSearchParam capParam in _caps.ResourceInteractions[resource.Name].SearchParameters.Values)
                {
                    if (usedParams.Contains(capParam.Name))
                    {
                        continue;
                    }

                    oasOp.Parameters.Add(BuildStringParameter(capParam.Name, capParam.Documentation));
                    usedParams.Add(capParam.Name);
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

        //if (_openApiOptions.IncludeDescriptions)
        //{
        //    oasOp.Description = "VRead a specific version of a FHIR " + resource.Name + ": " + resource.ShortDescription;
        //}

        if (_openApiOptions.IncludeSummaries)
        {
            oasOp.Summary = $"Read a specific version of a FHIR {resource.Name}";
        }

        // vread includes {id} segment
        oasOp.Parameters.Add(BuildReferencedParameter("id"));

        // vread includes {vid} segment
        oasOp.Parameters.Add(BuildReferencedParameter("vid"));

        foreach (string code in _openApiOptions.HttpReadParams)
        {
            oasOp.Parameters.Add(BuildReferencedParameter(code));
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

        //if (_openApiOptions.IncludeDescriptions)
        //{
        //    oasOp.Description = "Read a specific FHIR " + resource.Name + ": " + resource.ShortDescription;
        //}

        if (_openApiOptions.IncludeSummaries)
        {
            oasOp.Summary = $"Read a FHIR {resource.Name}";
        }

        // read includes {id} segment
        oasOp.Parameters.Add(BuildReferencedParameter("id"));

        foreach (string code in _openApiOptions.HttpReadParams)
        {
            oasOp.Parameters.Add(BuildReferencedParameter(code));
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
        OpenApiResponses r = new();

        if (_openApiOptions.SingleResponseCode)
        {
            switch (responseCodes[0])
            {
                case 200:
                case 201:
                    r.Add(
                        responseCodes[0].ToString(),
                        new OpenApiResponse()
                        {
                            Description = _httpResponseDescriptions[responseCodes[0]],
                            Content = BuildContentMap(resourceName, schemas),
                        });
                    break;

                default:
                    r.Add(
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
                            });
                        break;
                }
            }
        }

        return r;
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
    /// <returns>
    /// An enumerator that allows foreach to be used to process the resource FHIR operations in this
    /// collection.
    /// </returns>
    private IEnumerable<FhirOperation> GetResourceFhirOperations(string resourceName)
    {
        if (!IncludeResouce(resourceName))
        {
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
                if (!FhirManager.Current.TryResolveCanonical(_info.FhirSequence, capOp.DefinitionCanonical, out FhirArtifactClassEnum ac, out object fhirOpObj))
                {
                    Console.WriteLine($"Skipping unresolvable Operation: {capOp.DefinitionCanonical}");
                    continue;
                }

                if (!(fhirOpObj is FhirOperation fhirOp))
                {
                    Console.WriteLine($"Operation canonical '{capOp.DefinitionCanonical}' resolved to {ac}");
                    continue;
                }

                if (!(fhirOp.ResourceTypes?.Contains(resourceName) ?? false))
                {
                    Console.WriteLine($"Skipping {resourceName} requested operation '{capOp.DefinitionCanonical}' - definition cannot apply here");
                    continue;
                }

                operations.Add(fhirOp);
            }

            // some servers just shove all operations into system-level reporting
            foreach (FhirCapOperation capOp in _caps.ServerOperations.Values)
            {
                if (!FhirManager.Current.TryResolveCanonical(_info.FhirSequence, capOp.DefinitionCanonical, out FhirArtifactClassEnum ac, out object fhirOpObj))
                {
                    continue;
                }

                if (!(fhirOpObj is FhirOperation fhirOp))
                {
                    continue;
                }

                if (!(fhirOp.ResourceTypes?.Contains(resourceName) ?? false))
                {
                    continue;
                }

                if ((!fhirOp.DefinedOnInstance) && (!fhirOp.DefinedOnType))
                {
                    continue;
                }

                operations.Add(fhirOp);
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

            operations.Add(fhirOp);
        }

        return operations;
    }

    /// <summary>Gets the system FHIR operations in this collection.</summary>
    /// <returns>
    /// An enumerator that allows foreach to be used to process the system FHIR operations in this
    /// collection.
    /// </returns>
    private IEnumerable<FhirOperation> GetSystemFhirOperations()
    {
        List<FhirOperation> operations = new();

        if (_caps != null)
        {
            foreach (FhirCapOperation capOp in _caps.ServerOperations.Values)
            {
                if (!FhirManager.Current.TryResolveCanonical(_info.FhirSequence, capOp.DefinitionCanonical, out FhirArtifactClassEnum ac, out object fhirOpObj))
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

                operations.Add(fhirOp);
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

            operations.Add(fhirOp);
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

        operation.Responses = new OpenApiResponses()
        {
            ["200"] = new OpenApiResponse()
            {
                Description = "OK",
                Content = BuildContentMap(resourceName, schemas),
            },
        };

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
        else
        {
            title = $"FHIR {_info.FhirSequence}:{_info.VersionString}";
        }

        string version;

        if (!string.IsNullOrEmpty(_openApiOptions.Version))
        {
            version = _openApiOptions.Version;
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
