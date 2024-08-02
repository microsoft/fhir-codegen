// <copyright file="ModelBuilder.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Utility;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGen.Utils;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Smart;
using Microsoft.OpenApi.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Utils;
using static Microsoft.Health.Fhir.CodeGen.Language.OpenApi.OpenApiCommon;
using static Microsoft.Health.Fhir.CodeGenCommon.Extensions.FhirNameConventionExtensions;

#if NETSTANDARD2_0
using Microsoft.Health.Fhir.CodeGenCommon.Polyfill;
#endif

namespace Microsoft.Health.Fhir.CodeGen.Language.OpenApi;

public partial class ModelBuilder
{
    /// <summary>The information.</summary>
    private DefinitionCollection _dc;

    /// <summary>Options for controlling the operation.</summary>
    private OpenApiOptions _options;

    /// <summary>True to use export key filter.</summary>
    private bool _useExportKeyFilter = false;

    /// <summary>The export filter keys.</summary>
    private HashSet<string> _exportFilterKeys = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>The resources listed in the capability statement.</summary>
    private Dictionary<string, CapabilityStatement.ResourceComponent> _capResources = [];

    private HashSet<OaExpandedInteractionCodes> _allowedResourceInteractions = [];
    private HashSet<OaExpandedInteractionCodes> _requiredResourceInteractions = [];

    private FhirNameConventionExtensions.NamingConvention _idNamingConvention;

    private string _fhirBaseUrl;

    /// <summary>The capabilities.</summary>
    private CapabilityStatement? _caps;
    private SmartWellKnown? _smartConfig;

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

#if NET8_0_OR_GREATER
    // Match Markdown links [xxxxx](yyyy.html), where xxxx is a capitalized string
    [GeneratedRegex("(\\[[A-Z].*\\])\\((.*.html)\\)")]
    private static partial Regex MarkdownLinkRegex();
    private static readonly Regex _markdownLinkRegex = MarkdownLinkRegex();
#else
    private static readonly Regex _markdownLinkRegex = new Regex("(\\[[A-Z].*\\])\\((.*.html)\\)", RegexOptions.Compiled);
#endif

    /// <summary>Initializes a new instance of the <see cref="ModelBuilder"/> class.</summary>
    /// <param name="info">   The information.</param>
    /// <param name="options">Options for controlling the operation.</param>
    /// <param name="caps">   (Optional) The capabilities.</param>
    public ModelBuilder(
        DefinitionCollection info,
        OpenApiOptions options)
    {
        _dc = info;
        _options = options;

        if ((options.ExportKeys != null) && (options.ExportKeys.Count != 0))
        {
            _useExportKeyFilter = true;
            foreach (string key in options.ExportKeys)
            {
                if (!_exportFilterKeys.Contains(key))
                {
                    _exportFilterKeys.Add(key);
                }
            }
        }

        _fhirBaseUrl = options.FhirServerUrl;
        _caps = options.ServerCapabilities;
        _smartConfig = options.ServerSmartConfig;

        if (_caps?.Rest.Count != 0)
        {
            foreach (CapabilityStatement.ResourceComponent rc in _caps?.Rest.SelectMany(rest => rest.Resource) ?? [])
            {
                _capResources.Add(rc.Type, rc);
            }
        }

        switch (_options.IdNamingConvention)
        {
            case OaNamingConventionCodes.Camel:
                _idNamingConvention = FhirNameConventionExtensions.NamingConvention.CamelCase;
                break;
            case OaNamingConventionCodes.Pascal:
                _idNamingConvention = FhirNameConventionExtensions.NamingConvention.PascalCase;
                break;
            case OaNamingConventionCodes.Upper:
                _idNamingConvention = FhirNameConventionExtensions.NamingConvention.UpperCase;
                break;
            case OaNamingConventionCodes.Lower:
                _idNamingConvention = FhirNameConventionExtensions.NamingConvention.LowerCase;
                break;
            default:
                _idNamingConvention = FhirNameConventionExtensions.NamingConvention.PascalCase;
                break;
        }

        ConfigureResourceInteractions();
        ConfigureMimeSupport();
        ConfigureSearchSupport();
    }

    /// <summary>Configure default resource interactions.</summary>
    private void ConfigureResourceInteractions()
    {
        if (_options.InteractionRead != OaCapabilityBoolean.False)
        {
            if (_options.InteractionRead == OaCapabilityBoolean.True)
            {
                _requiredResourceInteractions.Add(OaExpandedInteractionCodes.Read);
            }

            _allowedResourceInteractions.Add(OaExpandedInteractionCodes.Read);
        }

        if ((_options.ConditionalRead != null) &&
            (_options.ConditionalRead != CapabilityStatement.ConditionalReadStatus.NotSupported))
        {
            _requiredResourceInteractions.Add(OaExpandedInteractionCodes.ReadConditional);
            _allowedResourceInteractions.Add(OaExpandedInteractionCodes.ReadConditional);
        }

        if (_options.InteractionVRead != OaCapabilityBoolean.False)
        {
            if (_options.InteractionVRead == OaCapabilityBoolean.True)
            {
                _requiredResourceInteractions.Add(OaExpandedInteractionCodes.VRead);
            }

            _allowedResourceInteractions.Add(OaExpandedInteractionCodes.VRead);
        }

        if (_options.InteractionUpdate != OaCapabilityBoolean.False)
        {
            if (_options.InteractionUpdate == OaCapabilityBoolean.True)
            {
                _requiredResourceInteractions.Add(OaExpandedInteractionCodes.Update);
            }

            _allowedResourceInteractions.Add(OaExpandedInteractionCodes.Update);
        }

        if (_options.InteractionUpdateConditional != OaCapabilityBoolean.False)
        {
            if (_options.InteractionUpdateConditional == OaCapabilityBoolean.True)
            {
                _requiredResourceInteractions.Add(OaExpandedInteractionCodes.UpdateConditional);
            }

            _allowedResourceInteractions.Add(OaExpandedInteractionCodes.UpdateConditional);
        }

        if (_options.InteractionPatch != OaCapabilityBoolean.False)
        {
            if (_options.InteractionPatch == OaCapabilityBoolean.True)
            {
                _requiredResourceInteractions.Add(OaExpandedInteractionCodes.Patch);
            }

            _allowedResourceInteractions.Add(OaExpandedInteractionCodes.Patch);
        }

        if (_options.InteractionPatchConditional != OaCapabilityBoolean.False)
        {
            if (_options.InteractionPatchConditional == OaCapabilityBoolean.True)
            {
                _requiredResourceInteractions.Add(OaExpandedInteractionCodes.PatchConditional);
            }

            _allowedResourceInteractions.Add(OaExpandedInteractionCodes.PatchConditional);
        }

        if (_options.InteractionDelete != OaCapabilityBoolean.False)
        {
            if (_options.InteractionDelete == OaCapabilityBoolean.True)
            {
                _requiredResourceInteractions.Add(OaExpandedInteractionCodes.Delete);
            }

            _allowedResourceInteractions.Add(OaExpandedInteractionCodes.Delete);
        }

        if (_options.InteractionDeleteConditionalSingle != OaCapabilityBoolean.False)
        {
            if (_options.InteractionDeleteConditionalSingle == OaCapabilityBoolean.True)
            {
                _requiredResourceInteractions.Add(OaExpandedInteractionCodes.DeleteConditionalSingle);
            }

            _allowedResourceInteractions.Add(OaExpandedInteractionCodes.DeleteConditionalSingle);
        }

        if (_options.InteractionDeleteConditionalMultiple != OaCapabilityBoolean.False)
        {
            if (_options.InteractionDeleteConditionalMultiple == OaCapabilityBoolean.True)
            {
                _requiredResourceInteractions.Add(OaExpandedInteractionCodes.DeleteConditionalMultiple);
            }

            _allowedResourceInteractions.Add(OaExpandedInteractionCodes.DeleteConditionalMultiple);
        }

        if (_options.InteractionDeleteHistory != OaCapabilityBoolean.False)
        {
            if (_options.InteractionDeleteHistory == OaCapabilityBoolean.True)
            {
                _requiredResourceInteractions.Add(OaExpandedInteractionCodes.DeleteHistory);
            }

            _allowedResourceInteractions.Add(OaExpandedInteractionCodes.DeleteHistory);
        }

        if (_options.InteractionDeleteHistoryVersion != OaCapabilityBoolean.False)
        {
            if (_options.InteractionDeleteHistoryVersion == OaCapabilityBoolean.True)
            {
                _requiredResourceInteractions.Add(OaExpandedInteractionCodes.DeleteHistoryVersion);
            }

            _allowedResourceInteractions.Add(OaExpandedInteractionCodes.DeleteHistoryVersion);
        }

        if (_options.InteractionHistoryInstance != OaCapabilityBoolean.False)
        {
            if (_options.InteractionHistoryInstance == OaCapabilityBoolean.True)
            {
                _requiredResourceInteractions.Add(OaExpandedInteractionCodes.HistoryInstance);
            }

            _allowedResourceInteractions.Add(OaExpandedInteractionCodes.HistoryInstance);
        }

        if (_options.InteractionHistoryType != OaCapabilityBoolean.False)
        {
            if (_options.InteractionHistoryType == OaCapabilityBoolean.True)
            {
                _requiredResourceInteractions.Add(OaExpandedInteractionCodes.HistoryType);
            }

            _allowedResourceInteractions.Add(OaExpandedInteractionCodes.HistoryType);
        }

        if (_options.InteractionCreate != OaCapabilityBoolean.False)
        {
            if (_options.InteractionCreate == OaCapabilityBoolean.True)
            {
                _requiredResourceInteractions.Add(OaExpandedInteractionCodes.Create);
            }

            _allowedResourceInteractions.Add(OaExpandedInteractionCodes.Create);
        }

        if (_options.InteractionCreateConditional != OaCapabilityBoolean.False)
        {
            if (_options.InteractionCreateConditional == OaCapabilityBoolean.True)
            {
                _requiredResourceInteractions.Add(OaExpandedInteractionCodes.CreateConditional);
            }

            _allowedResourceInteractions.Add(OaExpandedInteractionCodes.CreateConditional);
        }

        if (_options.InteractionSearchType != OaCapabilityBoolean.False)
        {
            if (_options.InteractionSearchType == OaCapabilityBoolean.True)
            {
                _requiredResourceInteractions.Add(OaExpandedInteractionCodes.SearchType);
            }

            _allowedResourceInteractions.Add(OaExpandedInteractionCodes.SearchType);
        }

        if (_options.InteractionOperationType != OaCapabilityBoolean.False)
        {
            if (_options.InteractionOperationType == OaCapabilityBoolean.True)
            {
                _requiredResourceInteractions.Add(OaExpandedInteractionCodes.OperationType);
            }

            _allowedResourceInteractions.Add(OaExpandedInteractionCodes.OperationType);
        }

        if (_options.InteractionOperationInstance != OaCapabilityBoolean.False)
        {
            if (_options.InteractionOperationInstance == OaCapabilityBoolean.True)
            {
                _requiredResourceInteractions.Add(OaExpandedInteractionCodes.OperationInstance);
            }

            _allowedResourceInteractions.Add(OaExpandedInteractionCodes.OperationInstance);
        }
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

        if (_options.ExportSearchParams)
        {
            if (_options.ConsolidateSearchParameters)
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

        if (_options.SchemaStyle != OpenApiCommon.OaSchemaStyleCodes.Inline)
        {
            doc.Components.Schemas = schemas;
        }

        // grab initial counts
        _totalQueryParameters = doc.Components.Parameters.Count;
        _totalSchemas = schemas.Count;

        Dictionary<string, OpenApiTag> tags = new()
        {
            { SYSTEM_TAG_REF.Reference.Id, new OpenApiTag() { Name = SYSTEM_TAG_REF.Reference.Id, Description = "Sever-level requests" } }
        };

        doc.Paths = BuildPaths(schemas, tags);

        doc.Tags = tags.Values.ToList();

        AddSmartOAuthSchemeIfRequired(doc);

        Console.WriteLine($"OpenAPI stats:");
        Console.WriteLine($"          totalPaths: {_totalPaths} ");
        Console.WriteLine($"     totalOperations: {_totalOperations} ");
        Console.WriteLine($"        totalSchemas: {_totalSchemas} ");
        Console.WriteLine($"totalQueryParameters: {_totalQueryParameters} ");
        Console.WriteLine($" totalParamInstances: {_totalParamInstances} ");
        Console.WriteLine($"");

        return doc;
    }

    private void AddSmartOAuthSchemeIfRequired(OpenApiDocument doc)
    {
        // only generate if we have a FHIR server url
        if (string.IsNullOrEmpty(_fhirBaseUrl))
        {
            return;
        }

        // OpenIdConnect is only supported in OpenApi 3.0
        if (_options.OpenApiVersion == OpenApiCommon.OaVersion.v2)
        {
            return;
        }

        string configUrl = _fhirBaseUrl.EndsWith('/')
                ? $"{_fhirBaseUrl}.well-known/smart-configuration"
                : $"{_fhirBaseUrl}/.well-known/smart-configuration";

        OpenApiSecurityScheme schema = new()
        {
            Name = "openId",
            Type = SecuritySchemeType.OpenIdConnect,
            OpenIdConnectUrl = new Uri(configUrl)
        };

        doc.Components.SecuritySchemes.Add("openId", schema);

        var schemeRef = new OpenApiSecurityScheme()
        {
            Reference = new OpenApiReference() { Id = schema.Name, Type = ReferenceType.SecurityScheme }
        };

        if (_options.BasicScopesOnly)
        {
            doc.SecurityRequirements.Add(new OpenApiSecurityRequirement()
            {
                { schemeRef, _smartConfig?.SupportedScopes.Where(s => !s.Contains('/')).ToArray() ?? [] }
            });
        }
        else
        {
            doc.SecurityRequirements.Add(new OpenApiSecurityRequirement()
            {
                { schemeRef, _smartConfig?.SupportedScopes.ToArray() ?? [] }
            });
        }
    }

    /// <summary>Consolidate resource parameters.</summary>
    /// <returns>A Dictionary&lt;string,OpenApiParameter&gt;</returns>
    private Dictionary<string, OpenApiParameter> ConsolidateResourceParameters()
    {
        Dictionary<string, OpenApiParameter> parameters = new();

        IEnumerable<StructureDefinition> resources = GetFilteredResources();

        foreach (StructureDefinition resource in resources)
        {
            IEnumerable<SearchParameter> fhirSps = GetResourceSearchParameters(resource.Name);

            foreach (SearchParameter fhirSp in fhirSps)
            {
                if (parameters.ContainsKey(fhirSp.Code))
                {
                    continue;
                }

                if (fhirSp.Type == SearchParamType.Number)
                {
                    parameters.Add(fhirSp.Code, BuildNumberParameter(fhirSp.Code, SanitizeDescription(fhirSp.Description), ParameterLocation.Query));
                }
                else
                {
                    parameters.Add(fhirSp.Code, BuildStringParameter(fhirSp.Code, SanitizeDescription(fhirSp.Description), ParameterLocation.Query));
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

        foreach (string code in _options.HttpCommonParamsHash)
        {
            if (_httpCommonParameters.ContainsKey(code))
            {
                _ = p.TryAdd(code, _httpCommonParameters[code]);
                continue;
            }

            _ = p.TryAdd(code, BuildStringParameter(code, string.Empty));
        }

        foreach (string code in _options.HttpReadHash)
        {
            if (_httpReadParameters.ContainsKey(code))
            {
                _ = p.TryAdd(code, _httpReadParameters[code]);
                continue;
            }

            _ = p.TryAdd(code, BuildStringParameter(code, string.Empty));
        }

        if (_options.ExportSearchParams)
        {
            foreach (string code in _options.SearchCommonHash)
            {
                if (_searchCommonParameters.ContainsKey(code))
                {
                    _ = p.TryAdd(code, _searchCommonParameters[code]);
                    continue;
                }

                _ = p.TryAdd(code, BuildStringParameter(code, string.Empty));
            }

            foreach (string code in _options.SearchResultHash)
            {
                if (_searchResultParameters.ContainsKey(code))
                {
                    _ = p.TryAdd(code, _searchResultParameters[code]);
                    continue;
                }

                _ = p.TryAdd(code, BuildStringParameter(code, string.Empty));
            }
        }

        foreach ((string code, OpenApiParameter parameter) in _pathParameters)
        {
            _ = p.TryAdd(code, parameter);
        }

        if ((_options.InteractionHistoryType == OaCapabilityBoolean.True) ||
            (_options.InteractionHistoryInstance == OaCapabilityBoolean.True))
        {
            foreach ((string code, OpenApiParameter parameter) in _historyParameters)
            {
                _ = p.TryAdd(code, parameter);
            }
        }
        else if ((_options.InteractionHistoryType == OaCapabilityBoolean.Capabilities) &&
                (_caps?.Rest.Any(rest => rest.Resource.Any(resource => resource.Interaction.Any(i => i.Code == CapabilityStatement.TypeRestfulInteraction.HistoryType))) == true))
        {
            foreach ((string code, OpenApiParameter parameter) in _historyParameters)
            {
                _ = p.TryAdd(code, parameter);
            }
        }
        else if ((_options.InteractionHistoryInstance == OaCapabilityBoolean.Capabilities) &&
                (_caps?.Rest.Any(rest => rest.Resource.Any(resource => resource.Interaction.Any(i => i.Code == CapabilityStatement.TypeRestfulInteraction.HistoryInstance))) == true))
        {
            foreach ((string code, OpenApiParameter parameter) in _historyParameters)
            {
                _ = p.TryAdd(code, parameter);
            }
        }

        if (_options.IncludeHttpHeaders)
        {
            foreach ((string code, OpenApiParameter parameter) in _httpRequestHeaders)
            {
                _ = p.TryAdd(code, parameter);
            }
        }

        return p;
    }

    /// <summary>Configure search support.</summary>
    private void ConfigureSearchSupport()
    {
        _searchGet = (_options.SearchSupport == OaHttpMethods.Both) || (_options.SearchSupport == OaHttpMethods.Get);
        _searchPost = (_options.SearchSupport == OaHttpMethods.Both) || (_options.SearchSupport == OaHttpMethods.Post);
    }

    /// <summary>Configure mime support.</summary>
    private void ConfigureMimeSupport()
    {
        if ((_options.FhirMimeTypes == OaFhirMimeCodes.Capabilities) &&
            (_caps != null))
        {
            _fhirJson = _caps.Format.Any(f => f.Contains("json", StringComparison.Ordinal));
            _fhirXml = _caps.Format.Any(f => f.Contains("xml", StringComparison.Ordinal));
            _fhirTurtle = _caps.Format.Any(f => f.Contains("ttl", StringComparison.Ordinal));
        }
        else if (_options.FhirMimeTypes == OaFhirMimeCodes.Common)
        {
            _fhirJson = true;
            _fhirXml = true;
            _fhirTurtle = false;
        }
        else if (_options.FhirMimeTypes == OaFhirMimeCodes.All)
        {
            _fhirJson = true;
            _fhirXml = true;
            _fhirTurtle = true;
        }
        else
        {
            _fhirJson = _options.FhirMimeTypes == OaFhirMimeCodes.JSON;
            _fhirXml = _options.FhirMimeTypes == OaFhirMimeCodes.XML;
            _fhirTurtle = _options.FhirMimeTypes == OaFhirMimeCodes.Turtle;
        }

        if ((_options.PatchMimeTypes == OaPatchMimeCodes.Capabilities) &&
            (_caps != null))
        {
            _patchJson = _caps.Format.Any(f => f.StartsWith("json", StringComparison.Ordinal) || f.Contains("/json", StringComparison.Ordinal));
            _patchXml = _caps.Format.Any(f => f.StartsWith("xml", StringComparison.Ordinal) || f.Contains("/xml", StringComparison.Ordinal));
            _patchFhirJson = _caps.Format.Any(f => f.Contains("fhir+json", StringComparison.Ordinal));
            _patchFhirXml = _caps.Format.Any(f => f.Contains("fhir+xml", StringComparison.Ordinal));
        }
        else if (_options.PatchMimeTypes == OaPatchMimeCodes.All)
        {
            _patchJson = true;
            _patchXml = true;
            _patchFhirJson = true;
            _patchFhirXml = true;
        }
        else if (_options.PatchMimeTypes == OaPatchMimeCodes.FhirMIME)
        {
            _patchFhirJson = _fhirJson;
            _patchFhirXml = _fhirXml;
        }
        else
        {
            _patchJson = _options.PatchMimeTypes == OaPatchMimeCodes.JSON;
            _patchXml = _options.PatchMimeTypes == OaPatchMimeCodes.XML;
        }
    }

    /// <summary>Resolve update create.</summary>
    /// <param name="resourceName">Name of the resource.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool ResolveUpdateCreate(string resourceName)
    {
        if (_options.UpdateCreate == OaCapabilityBoolean.Capabilities)
        {
            if (_capResources.TryGetValue(resourceName, out CapabilityStatement.ResourceComponent? rc))
            {
                return rc.UpdateCreate ?? false;
            }
        }

        return _options.UpdateCreate == OaCapabilityBoolean.True;
    }

    /// <summary>Resolve conditional create.</summary>
    /// <param name="resourceName">Name of the resource.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool ResolveConditionalCreate(string resourceName)
    {
        if (_options.InteractionCreateConditional == OaCapabilityBoolean.Capabilities)
        {
            if (_capResources.TryGetValue(resourceName, out CapabilityStatement.ResourceComponent? rc))
            {
                return rc.ConditionalCreate ?? false;
            }
        }

        return _options.InteractionCreateConditional == OaCapabilityBoolean.True;
    }

    /// <summary>Resolve conditional read.</summary>
    /// <param name="resourceName">Name of the resource.</param>
    /// <returns>A CapabilityStatement.ConditionalReadStatus.</returns>
    private CapabilityStatement.ConditionalReadStatus ResolveConditionalRead(string resourceName)
    {
        if (_options.ConditionalRead != null)
        {
            return _options.ConditionalRead.Value;
        }

        if (_capResources.TryGetValue(resourceName, out CapabilityStatement.ResourceComponent? rc))
        {
            return rc.ConditionalRead ?? CapabilityStatement.ConditionalReadStatus.NotSupported;
        }

        return CapabilityStatement.ConditionalReadStatus.NotSupported;
    }

    /// <summary>Resolve conditional update.</summary>
    /// <param name="resourceName">Name of the resource.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool ResolveConditionalUpdate(string resourceName)
    {
        if (_options.InteractionUpdateConditional == OaCapabilityBoolean.Capabilities)
        {
            if (_capResources.TryGetValue(resourceName, out CapabilityStatement.ResourceComponent? rc))
            {
                return rc.ConditionalUpdate ?? false;
            }
        }

        return _options.InteractionUpdateConditional == OaCapabilityBoolean.True;
    }

    /// <summary>Resolve conditional patch.</summary>
    /// <param name="resourceName">Name of the resource.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool ResolveConditionalPatch(string resourceName)
    {
        if (_options.InteractionPatchConditional == OaCapabilityBoolean.Capabilities)
        {
            if (_capResources.TryGetValue(resourceName, out CapabilityStatement.ResourceComponent? rc))
            {
                return rc.ConditionalPatch ?? false;
            }
        }

        return _options.InteractionPatchConditional == OaCapabilityBoolean.True;
    }

    /// <summary>Resolve conditional delete.</summary>
    /// <param name="resourceName">Name of the resource.</param>
    /// <returns>A CapabilityStatement.ConditionalDeleteStatus.</returns>
    private CapabilityStatement.ConditionalDeleteStatus ResolveConditionalDelete(string resourceName)
    {
        if (_options.InteractionDeleteConditionalMultiple == OaCapabilityBoolean.True)
        {
            return CapabilityStatement.ConditionalDeleteStatus.Multiple;
        }

        if (_options.InteractionDeleteConditionalSingle == OaCapabilityBoolean.True)
        {
            return CapabilityStatement.ConditionalDeleteStatus.Single;
        }

        if ((_options.InteractionDeleteConditionalSingle == OaCapabilityBoolean.False) ||
            (_options.InteractionDeleteConditionalMultiple == OaCapabilityBoolean.False))
        {
            return CapabilityStatement.ConditionalDeleteStatus.NotSupported;
        }

        if (_capResources.TryGetValue(resourceName, out CapabilityStatement.ResourceComponent? rc))
        {
            CapabilityStatement.ConditionalDeleteStatus? resourceCd = rc.ConditionalDelete;

            if ((resourceCd == CapabilityStatement.ConditionalDeleteStatus.Multiple) &&
                (_options.InteractionDeleteConditionalMultiple == OaCapabilityBoolean.False))
            {
                return (_options.InteractionDeleteConditionalSingle == OaCapabilityBoolean.False)
                    ? CapabilityStatement.ConditionalDeleteStatus.NotSupported
                    : CapabilityStatement.ConditionalDeleteStatus.Single;
            }

            if ((resourceCd == CapabilityStatement.ConditionalDeleteStatus.Single) &&
                (_options.InteractionDeleteConditionalSingle == OaCapabilityBoolean.False))
            {
                return CapabilityStatement.ConditionalDeleteStatus.NotSupported;
            }

            return resourceCd ?? CapabilityStatement.ConditionalDeleteStatus.NotSupported;
        }

        return CapabilityStatement.ConditionalDeleteStatus.NotSupported;
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

        // FHIR requires this, so unless explicitly disabled include the metadata endpoint
        if (_options.InteractionCapabilities != OaCapabilityBoolean.False)
        {
            if (_dc.FhirSequence == CodeGenCommon.Packaging.FhirReleases.FhirSequenceCodes.DSTU2)
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

        // handle resource-specific things first (alphabetical)
        foreach (StructureDefinition resource in GetFilteredResources(true))
        {
            // abstract resources do not get endpoints
            if (resource.Abstract == true)
            {
                continue;
            }

            // the list of resources can contain extra due to the need to build schemas, if we have a capability statement, check here
            if ((_caps != null) && !_capResources.ContainsKey(resource.Name))
            {
                continue;
            }

            // the list of resources can contain extra due to the need to build schemas, if we have a filter, check here
            if (_useExportKeyFilter && !_exportFilterKeys.Contains(resource.Name))
            {
                continue;
            }

            if (!tags.ContainsKey(resource.Name))
            {
                tags.Add(
                    resource.Name,
                    new()
                    {
                        Name = resource.Name,
                        Description = resource.Description,
                    });
            }

            // handle basic interactions
            foreach (OaExpandedInteractionCodes fhirInteraction in GetInteractions(resource.Name))
            {
                string opPath;

                switch (fhirInteraction)
                {
                    case OaExpandedInteractionCodes.Read:
                        {
                            opPath = $"/{resource.Name}/{{{PathComponentLogicalId}}}";
                            if (!paths.TryGetValue(opPath, out OpenApiPathItem? oaPathItem))
                            {
                                oaPathItem = new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() };
                                AddCommonHttpParameters(oaPathItem);
                                paths.Add(opPath, oaPathItem);
                            }

                            oaPathItem.AddOperation(OperationType.Get, BuildResourceReadOasOp(resource, schemas));
                            _totalOperations++;
                        }
                        break;

                    case OaExpandedInteractionCodes.VRead:
                        {
                            opPath = $"/{resource.Name}/{{{PathComponentLogicalId}}}/_history/{{{PathComponentVersionId}}}";
                            if (!paths.TryGetValue(opPath, out OpenApiPathItem? oaPathItem))
                            {
                                oaPathItem = new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() };
                                AddCommonHttpParameters(oaPathItem);
                                paths.Add(opPath, oaPathItem);
                            }

                            oaPathItem.AddOperation(OperationType.Get, BuildResourceVReadOasOp(resource, schemas));
                            _totalOperations++;
                        }
                        break;

                    case OaExpandedInteractionCodes.Update:
                        {
                            opPath = $"/{resource.Name}/{{{PathComponentLogicalId}}}";
                            if (!paths.TryGetValue(opPath, out OpenApiPathItem? oaPathItem))
                            {
                                oaPathItem = new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() };
                                AddCommonHttpParameters(oaPathItem);
                                paths.Add(opPath, oaPathItem);
                            }

                            oaPathItem.AddOperation(OperationType.Put, BuildResourceUpdateOasOp(resource, schemas));
                            _totalOperations++;
                        }
                        break;

                    case OaExpandedInteractionCodes.Patch:
                        {
                            opPath = $"/{resource.Name}/{{{PathComponentLogicalId}}}";
                            if (!paths.TryGetValue(opPath, out OpenApiPathItem? oaPathItem))
                            {
                                oaPathItem = new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() };
                                AddCommonHttpParameters(oaPathItem);
                                paths.Add(opPath, oaPathItem);
                            }

                            oaPathItem.AddOperation(OperationType.Patch, BuildResourcePatchOasOp(resource, schemas));
                            _totalOperations++;
                        }
                        break;

                    case OaExpandedInteractionCodes.Delete:
                        {
                            opPath = $"/{resource.Name}/{{{PathComponentLogicalId}}}";
                            if (!paths.TryGetValue(opPath, out OpenApiPathItem? oaPathItem))
                            {
                                oaPathItem = new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() };
                                AddCommonHttpParameters(oaPathItem);
                                paths.Add(opPath, oaPathItem);
                            }

                            oaPathItem.AddOperation(OperationType.Delete, BuildResourceDeleteOasOp(resource, schemas));
                            _totalOperations++;
                        }
                        break;

                    case OaExpandedInteractionCodes.HistoryInstance:
                        {
                            // verify - but this should be filtered when building interactions for the resource
                            //if (!_options.IncludeHistory)
                            //{
                            //    continue;
                            //}

                            opPath = $"/{resource.Name}/{{{PathComponentLogicalId}}}/_history";
                            if (!paths.TryGetValue(opPath, out OpenApiPathItem? oaPathItem))
                            {
                                oaPathItem = new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() };
                                AddCommonHttpParameters(oaPathItem);
                                paths.Add(opPath, oaPathItem);
                            }

                            oaPathItem.AddOperation(OperationType.Get, BuildResourceHistoryInstanceOasOp(resource, schemas));
                            _totalOperations++;
                        }
                        break;

                    case OaExpandedInteractionCodes.HistoryType:
                        {
                            // verify - but this should be filtered when building interactions for the resource
                            //if (!_options.IncludeHistory)
                            //{
                            //    continue;
                            //}

                            opPath = $"/{resource.Name}/_history";
                            if (!paths.TryGetValue(opPath, out OpenApiPathItem? oaPathItem))
                            {
                                oaPathItem = new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() };
                                AddCommonHttpParameters(oaPathItem);
                                paths.Add(opPath, oaPathItem);
                            }

                            oaPathItem.AddOperation(OperationType.Get, BuildResourceHistoryTypeOasOp(resource, schemas));
                            _totalOperations++;
                        }
                        break;

                    case OaExpandedInteractionCodes.Create:
                        {
                            opPath = $"/{resource.Name}";
                            if (!paths.TryGetValue(opPath, out OpenApiPathItem? oaPathItem))
                            {
                                oaPathItem = new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() };
                                AddCommonHttpParameters(oaPathItem);
                                paths.Add(opPath, oaPathItem);
                            }

                            oaPathItem.AddOperation(OperationType.Post, BuildResourceCreateOasOp(resource, schemas));
                            _totalOperations++;
                        }
                        break;

                    case OaExpandedInteractionCodes.SearchType:
                        {
                            if (_searchGet)
                            {
                                opPath = $"/{resource.Name}";
                                if (!paths.TryGetValue(opPath, out OpenApiPathItem? oaPathItem))
                                {
                                    oaPathItem = new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() };
                                    AddCommonHttpParameters(oaPathItem);
                                    paths.Add(opPath, oaPathItem);
                                }

                                oaPathItem.AddOperation(OperationType.Get, BuildResourceSearchTypeGetOasOp(resource, schemas));
                                _totalOperations++;
                            }

                            if (_searchPost)
                            {
                                opPath = $"/{resource.Name}/_search";
                                if (!paths.TryGetValue(opPath, out OpenApiPathItem? oaPathItem))
                                {
                                    oaPathItem = new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() };
                                    AddCommonHttpParameters(oaPathItem);
                                    paths.Add(opPath, oaPathItem);
                                }

                                oaPathItem.AddOperation(OperationType.Post, BuildResourceSearchTypePostOasOp(resource, schemas));
                                _totalOperations++;
                            }
                        }
                        break;

                    // TODO(ginoc): Add compartment support
                    //case OaExpandedInteractionCodes.SearchCompartment:
                    default:
                        break;

                }   // close: foreach interaction


            }       // close: foreach resource

            // traverse the operations for this resource
            foreach (OperationDefinition fhirOp in GetResourceFhirOperations(resource.Name))
            {
                BuildResourceOperationOasPaths(paths, resource, schemas, fhirOp);
            }

            // TODO(ginoc): handle compartments
        }

        // handle system-level search
        if ((_options.InteractionSearchSystem == OaCapabilityBoolean.True) ||
            ((_options.InteractionSearchSystem == OaCapabilityBoolean.Capabilities) &&
             (_caps?.Rest.Any(rest => rest.Interaction.Any(ri => ri.Code == CapabilityStatement.SystemRestfulInteraction.SearchSystem)) == true)))
        {
            string opPath;

            if (_searchGet)
            {
                opPath = "/";
                if (!paths.TryGetValue(opPath, out OpenApiPathItem? oaPathItem))
                {
                    oaPathItem = new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() };
                    AddCommonHttpParameters(oaPathItem);
                    paths.Add(opPath, oaPathItem);
                }

                oaPathItem.AddOperation(OperationType.Get, BuildSystemSearchGetOasOp(schemas));
                _totalOperations++;
            }

            if (_searchPost)
            {
                opPath = "/_search";
                if (!paths.TryGetValue(opPath, out OpenApiPathItem? oaPathItem))
                {
                    oaPathItem = new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() };
                    AddCommonHttpParameters(oaPathItem);
                    paths.Add(opPath, oaPathItem);
                }

                oaPathItem.AddOperation(OperationType.Post, BuildSystemSearchPostOasOp(schemas));
                _totalOperations++;
            }
        }

        // check for system-level bundle support
        if ((_options.InteractionBatch == OaCapabilityBoolean.True) ||
            (_options.InteractionTransaction == OaCapabilityBoolean.True) ||
            ((_options.InteractionBatch == OaCapabilityBoolean.Capabilities) &&
             (_caps?.Rest.Any(rest => rest.Interaction.Any(ri => ri.Code == CapabilityStatement.SystemRestfulInteraction.Batch)) == true)) ||
            ((_options.InteractionTransaction == OaCapabilityBoolean.Capabilities) &&
             (_caps?.Rest.Any(rest => rest.Interaction.Any(ri => ri.Code == CapabilityStatement.SystemRestfulInteraction.Transaction)) == true)))
        {
            string opPath = "/";
            if (!paths.TryGetValue(opPath, out OpenApiPathItem? oaPathItem))
            {
                oaPathItem = new OpenApiPathItem() { Operations = new Dictionary<OperationType, OpenApiOperation>() };
                AddCommonHttpParameters(oaPathItem);
                paths.Add(opPath, oaPathItem);
            }

            oaPathItem.AddOperation(OperationType.Post, BuildSystemBundlePostOasOp(schemas));
            _totalOperations++;
        }


        // TODO(ginoc): need to check system conditional delete


        // handle system-level operations
        foreach (OperationDefinition fhirOp in GetSystemFhirOperations(OperationDefinition.OperationKind.Operation))
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
        OperationDefinition fhirOp)
    {
        // query operations cannot be invoked directly, they are added to search
        if (fhirOp.Kind == OperationDefinition.OperationKind.Query)
        {
            return;
        }

        Dictionary<string, StructureDefinition> resources = GetFilteredResources().ToDictionary(c => c.Name);

        if (((fhirOp.Instance == true) || (fhirOp.Type == true)) &&
            (fhirOp.Resource?.Any() ?? false))
        {
            foreach (string? resourceName in fhirOp.Resource.Select(r => r.GetLiteral()))
            {
                if (string.IsNullOrEmpty(resourceName) ||
                    !resources.ContainsKey(resourceName!))
                {
                    continue;
                }

                StructureDefinition resource = resources[resourceName!];

                BuildResourceOperationOasPaths(
                    paths,
                    resource,
                    schemas,
                    fhirOp);
            }
        }

        if (fhirOp.System == true)
        {
            string opPath;

            if (TryBuildOperationGetOasOp(null, fhirOp, schemas, OaOpLevelCodes.Instance, out OpenApiOperation? oasOpGet))
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

            if (TryBuildOperationPostOasOp(null, fhirOp, schemas, OaOpLevelCodes.Instance, out OpenApiOperation? oasOpPost))
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
        StructureDefinition resource,
        Dictionary<string, OpenApiSchema> schemas,
        OperationDefinition fhirOp)
    {
        // query operations cannot be invoked directly, they are added to search
        if (fhirOp.Kind == OperationDefinition.OperationKind.Query)
        {
            return;
        }

        string opPath;

        if (fhirOp.Instance == true)
        {
            if (TryBuildOperationGetOasOp(resource, fhirOp, schemas, OaOpLevelCodes.Instance, out OpenApiOperation? oasOpGet))
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

            if (TryBuildOperationPostOasOp(resource, fhirOp, schemas, OaOpLevelCodes.Instance, out OpenApiOperation? oasOpPost))
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

        if (fhirOp.Type == true)
        {
            if (TryBuildOperationGetOasOp(resource, fhirOp, schemas, OaOpLevelCodes.Type, out OpenApiOperation? oasOpGet))
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

            if (TryBuildOperationPostOasOp(resource, fhirOp, schemas, OaOpLevelCodes.Type, out OpenApiOperation? oasOpPost))
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

    private static readonly OpenApiTag SYSTEM_TAG_REF =
        new OpenApiTag() { Reference = new OpenApiReference() { Id = "System", Type = ReferenceType.Tag } };

    /// <summary>Builds a resource operation get oas operation.</summary>
    /// <param name="resource">The resource.</param>
    /// <param name="fhirOp">  The FHIR operation.</param>
    /// <param name="schemas"> The schemas.</param>
    /// <param name="opLevel"> The operation level.</param>
    /// <param name="oasOp">   [out] The oas operation.</param>
    /// <returns>An OpenApiOperation.</returns>
    private bool TryBuildOperationGetOasOp(
        StructureDefinition? resource,
        OperationDefinition fhirOp,
        Dictionary<string, OpenApiSchema> schemas,
        OaOpLevelCodes opLevel,
        [NotNullWhen(true)] out OpenApiOperation? oasOp)
    {
        // operation get needs to be enabled
        if ((_options.OperationSupport != OaHttpMethods.Both) &&
            (_options.OperationSupport != OaHttpMethods.Get))
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
            id = $"{resource!.Name}.Instance.{fhirOp.Code}.Get".ToConvention(_idNamingConvention);
        }

        if (opLevel == OaOpLevelCodes.Type)
        {
            id = $"{resource!.Name}.{fhirOp.Code}.Get".ToConvention(_idNamingConvention);
        }

        if (opLevel == OaOpLevelCodes.System)
        {
            id = $"System.{fhirOp.Code}.Get".ToConvention(_idNamingConvention);
        }

        if (string.IsNullOrEmpty(id))
        {
            oasOp = null;
            return false;
        }

        oasOp.OperationId = id;

        if (opLevel == OaOpLevelCodes.System)
        {
            oasOp.Tags.Add(SYSTEM_TAG_REF);
        }
        else
        {
            oasOp.Tags.Add(new OpenApiTag() { Reference = new OpenApiReference() { Id = resource!.Name, Type = ReferenceType.Tag, } });
        }

        if (_options.IncludeSummaries)
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

        if (_options.IncludeHttpHeaders)
        {
            oasOp.Parameters.Add(BuildReferencedParameter("Accept"));
            oasOp.Parameters.Add(BuildReferencedParameter("Prefer"));
        }

        HashSet<string> usedParams = [];

        List<OperationDefinition.ParameterComponent> inParams = [];
        List<OperationDefinition.ParameterComponent> outParams = [];

        HashSet<string> inResourceParamNames = [];

        foreach (OperationDefinition.ParameterComponent p in fhirOp.Parameter)
        {
            switch (p.Use)
            {
                case OperationParameterUse.In:
                    inParams.Add(p);

                    if (p.Type == FHIRAllTypes.Resource)
                    {
                        inResourceParamNames.Add(p.Name);
                    }

                    break;

                case OperationParameterUse.Out:
                    outParams.Add(p);
                    break;
            }
        }

        // can skip this parameter if we are on instance level and there is a single input resource
        bool canSkipInputResource = (opLevel == OaOpLevelCodes.Instance) && (inResourceParamNames.Count() == 1);

        // search for input parameters
        foreach (OperationDefinition.ParameterComponent fhirParam in inParams)
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

            // if we have parameter scopes, skip ones that do not match
            if (fhirParam.Scope?.Any(s => s.GetLiteral() == opLevel.ToLiteral()) ?? false)
            {
                continue;
            }

            // query parameters can only be 'simple' types
            if (!_dc.PrimitiveTypesByName.ContainsKey(fhirParam.Type?.GetLiteral() ?? string.Empty))
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
            if (_dc.ResourcesByName.ContainsKey(outParams[0].Type?.GetLiteral() ?? string.Empty))
            {
                oasOp.Responses = BuildResponses(_responseCodesRead, outParams[0].Type.GetLiteral()!, schemas);
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
        StructureDefinition? resource,
        OperationDefinition fhirOp,
        Dictionary<string, OpenApiSchema> schemas,
        OaOpLevelCodes opLevel,
        [NotNullWhen(true)] out OpenApiOperation? oasOp)
    {
        // operation post needs to be enabled
        if ((_options.OperationSupport != OaHttpMethods.Both) &&
            (_options.OperationSupport != OaHttpMethods.Post))
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
            id = $"{resource!.Name}.Instance.{fhirOp.Code}.Post".ToConvention(_idNamingConvention);
        }

        if (opLevel == OaOpLevelCodes.Type)
        {
            id = $"{resource!.Name}.{fhirOp.Code}.Post".ToConvention(_idNamingConvention);
        }

        if (opLevel == OaOpLevelCodes.System)
        {
            id = $"System.{fhirOp.Code}.Post".ToConvention(_idNamingConvention);
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
            oasOp.Tags.Add(new OpenApiTag() { Reference = new OpenApiReference() { Id = resource!.Name, Type = ReferenceType.Tag, } });
        }

        if (_options.IncludeSummaries)
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

        if (_options.IncludeHttpHeaders)
        {
            oasOp.Parameters.Add(BuildReferencedParameter("Accept"));
            oasOp.Parameters.Add(BuildReferencedParameter("Prefer"));
        }

        HashSet<string> usedParams = [];

        List<OperationDefinition.ParameterComponent> inParams = [];
        List<OperationDefinition.ParameterComponent> simpleInParams = [];
        List<OperationDefinition.ParameterComponent> outParams = [];

        HashSet<string> inResourceParamNames = [];
        OperationDefinition.ParameterComponent? inputResourceParameter = null;

        int complexInParamCount = 0;

        foreach (OperationDefinition.ParameterComponent p in fhirOp.Parameter)
        {
            switch (p.Use)
            {
                case OperationParameterUse.In:
                    inParams.Add(p);

                    if (p.Type == FHIRAllTypes.Resource)
                    {
                        inResourceParamNames.Add(p.Name);
                        inputResourceParameter = p;
                    }
                    else if (_dc.ResourcesByName.ContainsKey(p.Type?.GetLiteral() ?? string.Empty))
                    {
                        inResourceParamNames.Add(p.Name);
                        inputResourceParameter = p;
                    }
                    else if (_dc.PrimitiveTypesByName.ContainsKey(p.Type?.GetLiteral() ?? string.Empty))
                    {
                        simpleInParams.Add(p);
                    }
                    else
                    {
                        complexInParamCount++;
                    }

                    break;

                case OperationParameterUse.Out:
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
                Content = BuildContentMap(inputResourceParameter.Type?.GetLiteral() ?? "Resource", schemas),
                Description = _options.IncludeDescriptions
                    ? inputResourceParameter.Documentation
                    : null,
            };

            foreach (OperationDefinition.ParameterComponent fhirParam in simpleInParams)
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
                Description = _options.IncludeDescriptions
                    ? "Input parameters to the operation"
                    : null,
            };
        }

        if (outParams.Count() == 1)
        {
            if (_dc.ResourcesByName.ContainsKey(outParams[0].Type?.GetLiteral() ?? string.Empty))
            {
                oasOp.Responses = BuildResponses(_responseCodesRead, outParams[0].Type?.GetLiteral() ?? string.Empty, schemas);
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
        foreach (string code in _options.HttpCommonParamsHash)
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

        string id = "Search.System.Post".ToConvention(_idNamingConvention);

        oasOp.OperationId = id;

        oasOp.Tags.Add(new OpenApiTag() { Reference = new OpenApiReference() { Id = "System", Type = ReferenceType.Tag, } });

        if (_options.IncludeSummaries)
        {
            oasOp.Summary = "search-system: Search all resources";
        }

        if (_options.IncludeHttpHeaders)
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
            Description = _options.IncludeDescriptions
                ? $"FHIR search parameters"
                : null,
        };

        HashSet<string> usedParams = new();

        foreach (string code in _options.HttpReadHash)
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

        if (_options.ExportSearchParams)
        {
            foreach (string code in _options.SearchCommonHash)
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

            foreach (string code in _options.SearchResultHash)
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

            if (_caps?.Rest.Any(rest => rest.SearchParam.Count != 0) ?? false)
            {
                foreach (CapabilityStatement.SearchParamComponent capParam in _caps.Rest.SelectMany(rest => rest.SearchParam))
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
                            Description = _options.IncludeDescriptions
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

    /// <summary>Builds system bundle post oas operation.</summary>
    /// <param name="schemas">The schemas.</param>
    /// <returns>An OpenApiOperation.</returns>
    private OpenApiOperation BuildSystemBundlePostOasOp(
        Dictionary<string, OpenApiSchema> schemas)
    {
        OpenApiOperation oasOp = new();

        string id = "Bundle.System.Post".ToConvention(_idNamingConvention);

        oasOp.OperationId = id;

        oasOp.Tags.Add(new OpenApiTag() { Reference = new OpenApiReference() { Id = "System", Type = ReferenceType.Tag, } });

        if (_options.IncludeSummaries)
        {
            oasOp.Summary = "bundle-system: Submit a batch or transaction";
        }

        if (_options.IncludeHttpHeaders)
        {
            oasOp.Parameters.Add(BuildReferencedParameter("Accept"));
            oasOp.Parameters.Add(BuildReferencedParameter("Prefer"));
        }

        oasOp.RequestBody = new OpenApiRequestBody()
        {
            Required = true,
            Content = BuildContentMap("Bundle", schemas),
            Description = _options.IncludeDescriptions
                ? $"Bundle being submitted"
                : null,
        };

        oasOp.Responses = BuildResponses(_responseCodesUpdate, "Bundle", schemas);

        return oasOp;
    }

    /// <summary>Builds resource search type oas operation.</summary>
    /// <param name="resource">The resource.</param>
    /// <param name="schemas"> The schemas.</param>
    /// <returns>An OpenApiOperation.</returns>
    private OpenApiOperation BuildResourceSearchTypePostOasOp(
        StructureDefinition resource,
        Dictionary<string, OpenApiSchema> schemas)
    {
        OpenApiOperation oasOp = new();

        string id = $"Search.{resource.Name}.Post".ToConvention(_idNamingConvention);

        oasOp.OperationId = id;

        oasOp.Tags.Add(new OpenApiTag() { Reference = new OpenApiReference() { Id = resource.Name, Type = ReferenceType.Tag, } });

        if (_options.IncludeSummaries)
        {
            oasOp.Summary = $"search-type: Search for {resource.Name} instances";
        }

        if (_options.IncludeHttpHeaders)
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
            Description = _options.IncludeDescriptions
                ? $"FHIR search parameters"
                : null,
        };

        HashSet<string> usedParams = new();

        foreach (string code in _options.HttpReadHash)
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

        if (_options.ExportSearchParams)
        {
            foreach (string code in _options.SearchCommonHash)
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

            foreach (string code in _options.SearchResultHash)
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

            foreach (SearchParameter fhirSp in GetResourceSearchParameters(resource.Name))
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

        string id = "Search.System.Get".ToConvention(_idNamingConvention);

        oasOp.OperationId = id;

        oasOp.Tags.Add(new OpenApiTag() { Reference = new OpenApiReference() { Id = "System", Type = ReferenceType.Tag, } });

        if (_options.IncludeSummaries)
        {
            oasOp.Summary = "search-system: Search all resources";
        }

        if (_options.IncludeHttpHeaders)
        {
            oasOp.Parameters.Add(BuildReferencedParameter("Accept"));
            oasOp.Parameters.Add(BuildReferencedParameter("Prefer"));
        }

        HashSet<string> usedParams = new();

        foreach (string code in _options.HttpReadHash)
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

        if (_options.ExportSearchParams)
        {
            foreach (string code in _options.SearchCommonHash)
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

            foreach (string code in _options.SearchResultHash)
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

            if (_caps?.Rest.Any(rest => rest.SearchParam.Count != 0) ?? false)
            {
                foreach (CapabilityStatement.SearchParamComponent capParam in _caps.Rest.SelectMany(rest => rest.SearchParam))
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
        StructureDefinition resource,
        Dictionary<string, OpenApiSchema> schemas)
    {
        OpenApiOperation oasOp = new();

        string id = $"Search.{resource.Name}.Get".ToConvention(_idNamingConvention);

        oasOp.OperationId = id;

        oasOp.Tags.Add(new OpenApiTag() { Reference = new OpenApiReference() { Id = resource.Name, Type = ReferenceType.Tag, } });

        if (_options.IncludeSummaries)
        {
            oasOp.Summary = $"search-type: Search for {resource.Name} instances";
        }

        if (_options.IncludeHttpHeaders)
        {
            oasOp.Parameters.Add(BuildReferencedParameter("Accept"));
            oasOp.Parameters.Add(BuildReferencedParameter("Prefer"));
        }

        HashSet<string> usedParams = new();

        foreach (string code in _options.HttpReadHash)
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

        if (_options.ExportSearchParams)
        {
            foreach (string code in _options.SearchCommonHash)
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

            foreach (string code in _options.SearchResultHash)
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

            foreach (SearchParameter fhirSp in GetResourceSearchParameters(resource.Name))
            {
                _totalParamInstances++;

                if (usedParams.Contains(fhirSp.Code))
                {
                    continue;
                }

                if (_options.ConsolidateSearchParameters)
                {
                    oasOp.Parameters.Add(BuildReferencedParameter(fhirSp.Code));
                    usedParams.Add(fhirSp.Code);
                    _totalQueryParameters++;
                    continue;
                }

                if (fhirSp.Type == SearchParamType.Number)
                {
                    oasOp.Parameters.Add(BuildNumberParameter(fhirSp.Code, SanitizeDescription(fhirSp.Description)));
                }
                else
                {
                    oasOp.Parameters.Add(BuildStringParameter(fhirSp.Code, SanitizeDescription(fhirSp.Description)));
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
        StructureDefinition resource,
        Dictionary<string, OpenApiSchema> schemas)
    {
        OpenApiOperation oasOp = new();

        string id = $"Create.{resource.Name}".ToConvention(_idNamingConvention);

        oasOp.OperationId = id;

        oasOp.Tags.Add(new OpenApiTag() { Reference = new OpenApiReference() { Id = resource.Name, Type = ReferenceType.Tag, } });

        if (_options.IncludeSummaries)
        {
            oasOp.Summary = $"create: Create a new {resource.Name} instance";
        }

        if (_options.IncludeHttpHeaders)
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
            Description = _options.IncludeDescriptions
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
        StructureDefinition resource,
        Dictionary<string, OpenApiSchema> schemas)
    {
        OpenApiOperation oasOp = new();

        string id = $"History.Type.{resource.Name}".ToConvention(_idNamingConvention);

        oasOp.OperationId = id;

        oasOp.Tags.Add(new OpenApiTag() { Reference = new OpenApiReference() { Id = resource.Name, Type = ReferenceType.Tag, } });

        if (_options.IncludeSummaries)
        {
            oasOp.Summary = $"history-type: Get the change history of all {resource.Name} resources";
        }

        if (_options.IncludeHttpHeaders)
        {
            oasOp.Parameters.Add(BuildReferencedParameter("Accept"));
            oasOp.Parameters.Add(BuildReferencedParameter("Prefer"));

            switch (ResolveConditionalRead(resource.Name))
            {
                case CapabilityStatement.ConditionalReadStatus.NotSupported:
                    break;
                case CapabilityStatement.ConditionalReadStatus.ModifiedSince:
                    oasOp.Parameters.Add(BuildReferencedParameter("If-Modified-Since"));
                    _totalParamInstances++;
                    break;
                case CapabilityStatement.ConditionalReadStatus.NotMatch:
                    oasOp.Parameters.Add(BuildReferencedParameter("If-None-Match"));
                    _totalParamInstances++;
                    break;
                case CapabilityStatement.ConditionalReadStatus.FullSupport:
                    oasOp.Parameters.Add(BuildReferencedParameter("If-Modified-Since"));
                    oasOp.Parameters.Add(BuildReferencedParameter("If-None-Match"));
                    _totalParamInstances++;
                    _totalParamInstances++;
                    break;
            }
        }

        foreach (string code in _options.HttpReadHash)
        {
            oasOp.Parameters.Add(BuildReferencedParameter(code));
            _totalParamInstances++;
        }

        foreach (string code in _options.HistoryHash)
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
        StructureDefinition resource,
        Dictionary<string, OpenApiSchema> schemas)
    {
        OpenApiOperation oasOp = new();

        string id = $"History.Instance.{resource.Name}".ToConvention(_idNamingConvention);

        oasOp.OperationId = id;

        oasOp.Tags.Add(new OpenApiTag() { Reference = new OpenApiReference() { Id = resource.Name, Type = ReferenceType.Tag, } });

        if (_options.IncludeSummaries)
        {
            oasOp.Summary = $"history-instance: Get the change history of a specific {resource.Name} resource";
        }

        // history instance includes PathComponentLogicalId segment
        oasOp.Parameters.Add(BuildReferencedParameter(PathComponentLogicalId));

        if (_options.IncludeHttpHeaders)
        {
            oasOp.Parameters.Add(BuildReferencedParameter("Accept"));
            oasOp.Parameters.Add(BuildReferencedParameter("Prefer"));

            switch (ResolveConditionalRead(resource.Name))
            {
                case CapabilityStatement.ConditionalReadStatus.NotSupported:
                    break;
                case CapabilityStatement.ConditionalReadStatus.ModifiedSince:
                    oasOp.Parameters.Add(BuildReferencedParameter("If-Modified-Since"));
                    _totalParamInstances++;
                    break;
                case CapabilityStatement.ConditionalReadStatus.NotMatch:
                    oasOp.Parameters.Add(BuildReferencedParameter("If-None-Match"));
                    _totalParamInstances++;
                    break;
                case CapabilityStatement.ConditionalReadStatus.FullSupport:
                    oasOp.Parameters.Add(BuildReferencedParameter("If-Modified-Since"));
                    oasOp.Parameters.Add(BuildReferencedParameter("If-None-Match"));
                    _totalParamInstances++;
                    _totalParamInstances++;
                    break;
            }
        }

        foreach (string code in _options.HttpReadHash)
        {
            oasOp.Parameters.Add(BuildReferencedParameter(code));
            _totalParamInstances++;
        }

        foreach (string code in _options.HistoryHash)
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
        StructureDefinition resource,
        Dictionary<string, OpenApiSchema> schemas)
    {
        CapabilityStatement.ConditionalDeleteStatus conditionalDelete = ResolveConditionalDelete(resource.Name);

        OpenApiOperation oasOp = new();

        string id = $"Delete.{resource.Name}".ToConvention(_idNamingConvention);

        oasOp.OperationId = id;

        oasOp.Tags.Add(new OpenApiTag() { Reference = new OpenApiReference() { Id = resource.Name, Type = ReferenceType.Tag, } });

        if (_options.IncludeSummaries)
        {
            oasOp.Summary = $"delete: Perform a logical delete on a {resource.Name} instance";
        }

        // delete includes PathComponentLogicalId segment
        oasOp.Parameters.Add(BuildReferencedParameter(PathComponentLogicalId));

        if (_options.IncludeHttpHeaders)
        {
            oasOp.Parameters.Add(BuildReferencedParameter("Accept"));
            oasOp.Parameters.Add(BuildReferencedParameter("Prefer"));

            if (conditionalDelete != CapabilityStatement.ConditionalDeleteStatus.NotSupported)
            {
                oasOp.Parameters.Add(BuildReferencedParameter("If-Match"));
                _totalParamInstances++;
            }
        }

        // conditional deletes take search parameters
        if ((conditionalDelete != CapabilityStatement.ConditionalDeleteStatus.NotSupported) &&
            _options.ExportSearchParams)
        {
            HashSet<string> usedParams = new();

            foreach (string code in _options.SearchCommonHash)
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

            foreach (SearchParameter fhirSp in GetResourceSearchParameters(resource.Name))
            {
                if (usedParams.Contains(fhirSp.Code))
                {
                    continue;
                }

                _totalParamInstances++;

                if (_options.ConsolidateSearchParameters)
                {
                    oasOp.Parameters.Add(BuildReferencedParameter(fhirSp.Code));
                    usedParams.Add(fhirSp.Code);
                    _totalQueryParameters++;
                    continue;
                }

                if (fhirSp.Type == SearchParamType.Number)
                {
                    oasOp.Parameters.Add(BuildNumberParameter(fhirSp.Code, SanitizeDescription(fhirSp.Description)));
                }
                else
                {
                    oasOp.Parameters.Add(BuildStringParameter(fhirSp.Code, SanitizeDescription(fhirSp.Description)));
                }

                usedParams.Add(fhirSp.Code);
                _totalQueryParameters++;
            }
        }

        // delete responses should not contain a body resource
        oasOp.Responses = BuildResponses(_responseCodesDelete, string.Empty, schemas);

        return oasOp;
    }

    /// <summary>Builds resource patch oas operation.</summary>
    /// <param name="resource">The resource.</param>
    /// <param name="schemas"> The schemas.</param>
    /// <returns>An OpenApiOperation.</returns>
    private OpenApiOperation BuildResourcePatchOasOp(
        StructureDefinition resource,
        Dictionary<string, OpenApiSchema> schemas)
    {
        OpenApiOperation oasOp = new();

        string id = $"Patch.{resource.Name}".ToConvention(_idNamingConvention);

        oasOp.OperationId = id;

        oasOp.Tags.Add(new OpenApiTag() { Reference = new OpenApiReference() { Id = resource.Name, Type = ReferenceType.Tag, } });

        if (_options.IncludeSummaries)
        {
            oasOp.Summary = $"patch: Patch a {resource.Name} instance";
        }

        // patch includes PathComponentLogicalId segment
        oasOp.Parameters.Add(BuildReferencedParameter(PathComponentLogicalId));

        if (_options.IncludeHttpHeaders)
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
            Description = _options.IncludeDescriptions
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
        StructureDefinition resource,
        Dictionary<string, OpenApiSchema> schemas)
    {
        OpenApiOperation oasOp = new();

        bool supportUpdateCreate = ResolveUpdateCreate(resource.Name);
        bool supportConditionalCreate = supportUpdateCreate && ResolveConditionalCreate(resource.Name);
        bool supportConditionalUpdate = ResolveConditionalUpdate(resource.Name);

        string id;

        if (ResolveUpdateCreate(resource.Name))
        {
            id = $"UpdateOrCreate.{resource.Name}".ToConvention(_idNamingConvention);

            if (_options.IncludeSummaries)
            {
                oasOp.Summary = $"update,create: Update or Create a {resource.Name} instance";
            }
        }
        else
        {
            id = $"Update.{resource.Name}".ToConvention(_idNamingConvention);

            if (_options.IncludeSummaries)
            {
                oasOp.Summary = $"update: Update a {resource.Name} instance";
            }
        }

        oasOp.OperationId = id;

        oasOp.Tags.Add(new OpenApiTag() { Reference = new OpenApiReference() { Id = resource.Name, Type = ReferenceType.Tag, } });

        // update includes PathComponentLogicalId segment
        oasOp.Parameters.Add(BuildReferencedParameter(PathComponentLogicalId));

        if (_options.IncludeHttpHeaders)
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
            if (_options.ExportSearchParams)
            {
                HashSet<string> usedParams = new();

                foreach (string code in _options.SearchCommonHash)
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

                foreach (SearchParameter fhirSp in GetResourceSearchParameters(resource.Name))
                {
                    _totalParamInstances++;

                    if (_options.ConsolidateSearchParameters)
                    {
                        oasOp.Parameters.Add(BuildReferencedParameter(fhirSp.Code));
                        usedParams.Add(fhirSp.Code);
                        _totalQueryParameters++;
                        continue;
                    }

                    if (fhirSp.Type == SearchParamType.Number)
                    {
                        oasOp.Parameters.Add(BuildNumberParameter(fhirSp.Code, SanitizeDescription(fhirSp.Description)));
                    }
                    else
                    {
                        oasOp.Parameters.Add(BuildStringParameter(fhirSp.Code, SanitizeDescription(fhirSp.Description)));
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
            Description = _options.IncludeDescriptions
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
        StructureDefinition resource,
        Dictionary<string, OpenApiSchema> schemas)
    {
        OpenApiOperation oasOp = new();

        string id = $"VRead.${resource.Name}".ToConvention(_idNamingConvention);

        oasOp.OperationId = id;

        oasOp.Tags.Add(new OpenApiTag() { Reference = new OpenApiReference() { Id = resource.Name, Type = ReferenceType.Tag, } });

        if (_options.IncludeSummaries)
        {
            oasOp.Summary = $"vread: Read {resource.Name} instance with specific version";
        }

        // vread includes PathComponentLogicalId segment
        oasOp.Parameters.Add(BuildReferencedParameter(PathComponentLogicalId));

        // vread includes PathComponentVersionId segment
        oasOp.Parameters.Add(BuildReferencedParameter(PathComponentVersionId));

        foreach (string code in _options.HttpReadHash)
        {
            oasOp.Parameters.Add(BuildReferencedParameter(code));
        }

        if (_options.IncludeHttpHeaders)
        {
            oasOp.Parameters.Add(BuildReferencedParameter("Accept"));
            oasOp.Parameters.Add(BuildReferencedParameter("Prefer"));

            switch (ResolveConditionalRead(resource.Name))
            {
                case CapabilityStatement.ConditionalReadStatus.NotSupported:
                    break;
                case CapabilityStatement.ConditionalReadStatus.ModifiedSince:
                    oasOp.Parameters.Add(BuildReferencedParameter("If-Modified-Since"));
                    _totalParamInstances++;
                    break;
                case CapabilityStatement.ConditionalReadStatus.NotMatch:
                    oasOp.Parameters.Add(BuildReferencedParameter("If-None-Match"));
                    _totalParamInstances++;
                    break;
                case CapabilityStatement.ConditionalReadStatus.FullSupport:
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
        StructureDefinition resource,
        Dictionary<string, OpenApiSchema> schemas)
    {
        OpenApiOperation oasOp = new();

        string id = $"Read.{resource.Name}".ToConvention(_idNamingConvention);

        oasOp.OperationId = id;

        oasOp.Tags.Add(new OpenApiTag() { Reference = new OpenApiReference() { Id = resource.Name, Type = ReferenceType.Tag, } });

        if (_options.IncludeSummaries)
        {
            oasOp.Summary = $"read: Read a FHIR {resource.Name}";
        }

        // read includes PathComponentLogicalId segment
        oasOp.Parameters.Add(BuildReferencedParameter(PathComponentLogicalId));

        foreach (string code in _options.HttpReadHash)
        {
            oasOp.Parameters.Add(BuildReferencedParameter(code));
        }

        if (_options.IncludeHttpHeaders)
        {
            oasOp.Parameters.Add(BuildReferencedParameter("Accept"));
            oasOp.Parameters.Add(BuildReferencedParameter("Prefer"));

            switch (ResolveConditionalRead(resource.Name))
            {
                case CapabilityStatement.ConditionalReadStatus.NotSupported:
                    break;
                case CapabilityStatement.ConditionalReadStatus.ModifiedSince:
                    oasOp.Parameters.Add(BuildReferencedParameter("If-Modified-Since"));
                    _totalParamInstances++;
                    break;
                case CapabilityStatement.ConditionalReadStatus.NotMatch:
                    oasOp.Parameters.Add(BuildReferencedParameter("If-None-Match"));
                    _totalParamInstances++;
                    break;
                case CapabilityStatement.ConditionalReadStatus.FullSupport:
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
        emitResponse(_options.SingleResponses ? responseCodes.Take(1) : responseCodes, resourceName, schemas, r);
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
                                Content = string.IsNullOrEmpty(resourceName) ? [] : BuildContentMap(resourceName, schemas),
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

    /// <summary>
    /// Retrieves the list of expanded interaction codes for the given resource component.
    /// </summary>
    /// <param name="rComp">The resource component.</param>
    /// <returns>The list of expanded interaction codes.</returns>
    private List<OaExpandedInteractionCodes> InteractionsFor(CapabilityStatement.ResourceComponent rComp)
    {
        List<OaExpandedInteractionCodes> interactions = [];

        foreach (CapabilityStatement.TypeRestfulInteraction? ri in rComp.Interaction.Select(i => i.Code))
        {
            if (ri == null)
            {
                continue;
            }

            switch (ri)
            {
                case CapabilityStatement.TypeRestfulInteraction.Read:
                    interactions.Add(OaExpandedInteractionCodes.Read);
                    if ((rComp.ConditionalRead != null) &&
                        (rComp.ConditionalRead != CapabilityStatement.ConditionalReadStatus.NotMatch))
                    {
                        interactions.Add(OaExpandedInteractionCodes.ReadConditional);
                    }
                    break;

                case CapabilityStatement.TypeRestfulInteraction.Vread:
                    interactions.Add(OaExpandedInteractionCodes.VRead);
                    break;

                case CapabilityStatement.TypeRestfulInteraction.HistoryInstance:
                    interactions.Add(OaExpandedInteractionCodes.HistoryInstance);
                    break;

                case CapabilityStatement.TypeRestfulInteraction.HistoryType:
                    interactions.Add(OaExpandedInteractionCodes.HistoryType);
                    break;

                case CapabilityStatement.TypeRestfulInteraction.SearchType:
                    interactions.Add(OaExpandedInteractionCodes.SearchType);
                    break;

                case CapabilityStatement.TypeRestfulInteraction.Create:
                    interactions.Add(OaExpandedInteractionCodes.Create);
                    if (rComp.ConditionalCreate == true)
                    {
                        interactions.Add(OaExpandedInteractionCodes.CreateConditional);
                    }
                    break;

                case CapabilityStatement.TypeRestfulInteraction.Update:
                    interactions.Add(OaExpandedInteractionCodes.Update);
                    if (rComp.ConditionalUpdate == true)
                    {
                        interactions.Add(OaExpandedInteractionCodes.UpdateConditional);
                    }
                    break;

                case CapabilityStatement.TypeRestfulInteraction.Patch:
                    interactions.Add(OaExpandedInteractionCodes.Patch);
                    if (rComp.ConditionalPatch == true)
                    {
                        interactions.Add(OaExpandedInteractionCodes.PatchConditional);
                    }
                    break;

                case CapabilityStatement.TypeRestfulInteraction.Delete:
                    interactions.Add(OaExpandedInteractionCodes.Delete);
                    if (rComp.ConditionalDelete == CapabilityStatement.ConditionalDeleteStatus.Single)
                    {
                        interactions.Add(OaExpandedInteractionCodes.DeleteConditionalSingle);
                    }
                    else if (rComp.ConditionalDelete == CapabilityStatement.ConditionalDeleteStatus.Multiple)
                    {
                        interactions.Add(OaExpandedInteractionCodes.DeleteConditionalMultiple);
                        interactions.Add(OaExpandedInteractionCodes.DeleteConditionalSingle);
                    }
                    break;
            }
        }

        if (rComp.Operation.Count != 0)
        {
            if (_options.InteractionOperationType != OaCapabilityBoolean.False)
            {
                interactions.Add(OaExpandedInteractionCodes.OperationType);
            }

            if (_options.InteractionOperationInstance == OaCapabilityBoolean.True)
            {
                interactions.Add(OaExpandedInteractionCodes.OperationInstance);
            }
        }

        return interactions;
    }

    /// <summary>Gets the interactions in this collection.</summary>
    /// <param name="resourceName">Name of the resource.</param>
    /// <returns>
    /// An enumerator that allows foreach to be used to process the interactions in this collection.
    /// </returns>
    private IEnumerable<OaExpandedInteractionCodes> GetInteractions(string resourceName)
    {
        if (!IncludeResource(resourceName))
        {
            return [];
        }

        HashSet<OaExpandedInteractionCodes> capabilityInteractions;

        if (_capResources.TryGetValue(resourceName, out CapabilityStatement.ResourceComponent? rComp))
        {
            capabilityInteractions = new(InteractionsFor(rComp));
        }
        else
        {
            capabilityInteractions = _allowedResourceInteractions.DeepCopy();
        }

        // always apply any configured 'required' interactions (overridden by configuration)
        capabilityInteractions.UnionWith(_requiredResourceInteractions);

        if (_options.ExportReadOnly)
        {
            capabilityInteractions.UnionWith(_resInteractionHashRO);
        }

        if (_options.ExportWriteOnly)
        {
            capabilityInteractions.UnionWith(_resInteractionHashWO);
        }

        return capabilityInteractions;
    }

    /// <summary>Gets the resource FHIR operations in this collection.</summary>
    /// <param name="resourceName">Name of the resource.</param>
    /// <param name="kindFilter">  (Optional) A filter specifying the kind.</param>
    /// <returns>
    /// An enumerator that allows foreach to be used to process the resource FHIR operations in this
    /// collection.
    /// </returns>
    private IEnumerable<OperationDefinition> GetResourceFhirOperations(
        string resourceName)
    {
        if (!IncludeResource(resourceName))
        {
            return [];
        }

        // we need to get parent resources to find operations that are defined on (e.g.) Resource
        HashSet<string> resourceAndParents = new(_dc.GetResourceParents(resourceName));

        if (!resourceAndParents.Any())
        {
            // unknown resource name
            return [];
        }

        HashSet<string> used = [];
        List<OperationDefinition> operations = [];

        if (_caps != null)
        {
            if (!_capResources.TryGetValue(resourceName, out CapabilityStatement.ResourceComponent? rComp))
            {
                return [];
            }

            // iterate over the operations listed in this resource
            foreach (CapabilityStatement.OperationComponent capOp in rComp.Operation)
            {
                // we cannot do anything with an operation that does not have a definition
                if (string.IsNullOrEmpty(capOp.Definition))
                {
                    continue;
                }

                if (_dc.TryResolveByCanonicalUri(capOp.Definition, out Resource? resolvedResource) &&
                    (resolvedResource is OperationDefinition fhirOp))
                {
                    // skip query operations here
                    if (fhirOp.Kind == OperationDefinition.OperationKind.Query)
                    {
                        continue;
                    }
                }
                else
                {
                    // we cannot do anything with an operation that does not resolve
                    continue;
                }

                if (used.Contains(fhirOp.Name))
                {
                    continue;
                }

                if ((fhirOp.Instance != true) && (!fhirOp.Type != true))
                {
                    continue;
                }

                operations.Add(fhirOp);
                used.Add(fhirOp.Name);
            }

            // some servers just shove all operations into system-level reporting
            foreach (CapabilityStatement.OperationComponent capOp in _caps.Rest.SelectMany(rest => rest.Operation))
            {
                // we cannot do anything with an operation that does not have a definition
                if (string.IsNullOrEmpty(capOp.Definition))
                {
                    continue;
                }

                if (_dc.TryResolveByCanonicalUri(capOp.Definition, out Resource? resolvedResource) &&
                    (resolvedResource is OperationDefinition fhirOp))
                {
                    // skip query operations here
                    if (fhirOp.Kind == OperationDefinition.OperationKind.Query)
                    {
                        continue;
                    }
                }
                else
                {
                    // we cannot do anything with an operation that does not resolve
                    continue;
                }

                if (used.Contains(fhirOp.Name))
                {
                    continue;
                }

                if ((fhirOp.Instance != true) && (!fhirOp.Type != true))
                {
                    continue;
                }

                // since some operation definitions exclude the resource type when they mean 'Resource', missing defaults to true
                if (!(fhirOp.Resource?.Any(rt => resourceAndParents.Contains(rt.GetLiteral()!)) ?? true))
                {
                    continue;
                }

                operations.Add(fhirOp);
                used.Add(fhirOp.Name);
            }

            return operations;
        }

        // if there is no capability statement, just check everything in the package
        foreach (OperationDefinition fhirOp in _dc.OperationsByUrl.Values)
        {
            if (used.Contains(fhirOp.Name))
            {
                continue;
            }

            // since some operation definitions exclude the resource type when they mean 'Resource', missing defaults to true
            if (!(fhirOp.Resource?.Any(rt => resourceAndParents.Contains(rt.GetLiteral()!)) ?? true))
            {
                continue;
            }

            if ((fhirOp.Instance != true) && (!fhirOp.Type != true))
            {
                continue;
            }

            operations.Add(fhirOp);
            used.Add(fhirOp.Name);
        }

        return operations;
    }

    /// <summary>Gets FHIR query search parameter.</summary>
    /// <param name="uriBase">     The URI base.</param>
    /// <param name="resourceName">Name of the resource.</param>
    /// <returns>The FHIR query search parameter.</returns>
    private SearchParameter GetFhirQuerySearchParam(
        Uri uriBase,
        string resourceName)
    {
        return new SearchParameter()
        {
            Url = new Uri(uriBase, "/SearchParameter/Resource-query").ToString(),
            Name = "_query",
            Code = "_query",
            Type = SearchParamType.Token,
            Base = [ EnumUtility.ParseLiteral<VersionIndependentResourceTypesAll>(resourceName) ],
            Description = "A custom search profile that describes a specific defined query operation",
            Purpose = "A custom search profile that describes a specific defined query operation",
        };
    }

    /// <summary>Gets the resource search parameters in this collection.</summary>
    /// <param name="resourceName">Name of the resource.</param>
    /// <returns>
    /// An enumerator that allows foreach to be used to process the resource FHIR search parameters in this
    /// collection.
    /// </returns>
    private IEnumerable<SearchParameter> GetResourceSearchParameters(string resourceName)
    {
        if (!IncludeResource(resourceName))
        {
            return [];
        }

        HashSet<string> used = new();
        List<SearchParameter> searchParameters = new();

        // if we are using a capability statement, use the search parameters from there
        if ((_caps != null) && _capResources.TryGetValue(resourceName, out CapabilityStatement.ResourceComponent? capResource))
        {
            Uri uriBase = string.IsNullOrEmpty(_caps.Url)
                ? new Uri("http://fhir-codegen-local/SearchParameter")
                : new Uri(_caps.Url);

            foreach (CapabilityStatement.SearchParamComponent capSp in capResource.SearchParam)
            {
                if (used.Contains(capSp.Name))
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(capSp.Definition))
                {
                    if (_dc.TryResolveByCanonicalUri(capSp.Definition, out Resource? resolvedResource) &&
                        (resolvedResource is SearchParameter fhirSp))
                    {
                        searchParameters.Add(fhirSp);
                        used.Add(capSp.Name);
                        continue;
                    }
                }

                //string doc = capSp.Documentation ?? string.Empty;
                //string pattern = @"(\[[A-Z].*\])\((.*.html)\)";  // Match Markdown links [xxxxx](yyyy.html), where xxxx is a capitalized string
                //string fixedDoc = string.IsNullOrEmpty(doc) ? string.Empty : Regex.Replace(doc, pattern, "$1(http://hl7.org/fhir/$2)");    // prefix those links with HL7 FHIR spec base
                string fixedDoc = string.IsNullOrEmpty(capSp.Documentation)
                    ? string.Empty :
                    _markdownLinkRegex.Replace(capSp.Documentation, "$1(http://hl7.org/fhir/$2)");    // prefix those links with HL7 FHIR spec base

                // create a 'local' canonical for referencing
                searchParameters.Add(new SearchParameter()
                {
                    Url = string.IsNullOrEmpty(capSp.Definition)
                        ? new Uri(uriBase, resourceName + "-" + capSp.Name).ToString()
                        : new Uri(capSp.Definition).ToString(),
                    Name = capSp.Name,
                    Code = capSp.Name,
                    Type = capSp.Type,
                    Base = [ EnumUtility.ParseLiteral<VersionIndependentResourceTypesAll>(resourceName) ],
                    Description = fixedDoc,
                    Purpose = fixedDoc,
                });

                used.Add(capSp.Name);
            }

            // check for 'query' operations
            foreach (CapabilityStatement.OperationComponent capOp in capResource.Operation)
            {
                // we cannot do anything with an operation that does not have a definition
                if (string.IsNullOrEmpty(capOp.Definition))
                {
                    continue;
                }

                if (_dc.TryResolveByCanonicalUri(capOp.Definition, out Resource? resolvedResource) &&
                    (resolvedResource is OperationDefinition fhirOp))
                {
                    // we only care about query operations here
                    if (fhirOp.Kind != OperationDefinition.OperationKind.Query)
                    {
                        continue;
                    }
                }
                else
                {
                    // we cannot do anything with an operation that does not resolve
                    continue;
                }

                // if we have any query operations, make sure _query is added to this resource
                if (!used.Contains("_query"))
                {
                    searchParameters.Add(GetFhirQuerySearchParam(uriBase, resourceName));
                    used.Add("_query");
                }

                AddOperationParameters(
                    resourceName,
                    fhirOp,
                    uriBase,
                    searchParameters,
                    used);
            }

            // exit out since we have everything based on the capability statement
            return searchParameters;
        }

        // if there is no capability statement, just use everything in the package
        searchParameters.AddRange(_dc.SearchParametersForBase(resourceName).Values);

        // traverse type operations for this resource type
        foreach (OperationDefinition fhirOp in _dc.TypeOperationsForResource(resourceName).Values)
        {
            // we only care about query operations
            if (fhirOp.Kind != OperationDefinition.OperationKind.Query)
            {
                continue;
            }

            Uri uriBase = string.IsNullOrEmpty(_caps?.Url)
                ? new Uri("http://fhir-codegen-local/SearchParameter")
                : new Uri(_caps!.Url);

            // if we have any query operations, make sure _query is added to this resource
            if (!used.Contains("_query"))
            {
                searchParameters.Add(GetFhirQuerySearchParam(uriBase, resourceName));
                used.Add("_query");
            }

            AddOperationParameters(
                resourceName,
                fhirOp,
                uriBase,
                searchParameters,
                used);
        }

        // traverse instance operations for this resource type
        foreach (OperationDefinition fhirOp in _dc.TypeOperationsForResource(resourceName).Values)
        {
            // we only care about query operations
            if (fhirOp.Kind != OperationDefinition.OperationKind.Query)
            {
                continue;
            }

            Uri uriBase = string.IsNullOrEmpty(_caps?.Url)
                ? new Uri("http://fhir-codegen-local/SearchParameter")
                : new Uri(_caps!.Url);

            // if we have any query operations, make sure _query is added to this resource
            if (!used.Contains("_query"))
            {
                searchParameters.Add(GetFhirQuerySearchParam(uriBase, resourceName));
                used.Add("_query");
            }

            AddOperationParameters(
                resourceName,
                fhirOp,
                uriBase,
                searchParameters,
                used);
        }

        return searchParameters;
    }

    /// <summary>
    /// Adds search parameter definitions for operation parameters.
    /// </summary>
    private void AddOperationParameters(
        string resourceName,
        OperationDefinition fhirOp,
        Uri uriBase,
        List<SearchParameter> searchParameters,
        HashSet<string> used)
    {
        Uri fhirOpUri = string.IsNullOrEmpty(fhirOp.Url) ? new Uri(uriBase, fhirOp.Name) : new Uri(fhirOp.Url);

        // traverse the operation parameters to add as search parameters
        foreach (OperationDefinition.ParameterComponent opParam in fhirOp.Parameter)
        {
            // we only care about input parameters
            if (opParam.Use != OperationParameterUse.In)
            {
                continue;
            }

            // do not include parameters that do not explicitly state their search type (per spec)
            if (opParam.SearchType == null)
            {
                continue;
            }

            if (used.Contains(opParam.Name))
            {
                continue;
            }

            // create a 'local' canonical search parameter for referencing
            searchParameters.Add(new SearchParameter()
            {
                Url = string.IsNullOrEmpty(fhirOp.Url)
                    ? new Uri(uriBase, $"{resourceName}-{fhirOp.Name}-{opParam.Name}").ToString()
                    : new Uri(fhirOpUri, $"-{opParam.Name}").ToString(),
                Name = opParam.Name,
                Code = opParam.Name,
                Type = opParam.SearchType,
                Base = [EnumUtility.ParseLiteral<VersionIndependentResourceTypesAll>(resourceName)],
                Description = opParam.Documentation,
                Purpose = opParam.Documentation,
            });

            used.Add(opParam.Name);
        }
    }

    /// <summary>Gets the system FHIR operations in this collection.</summary>
    /// <param name="kindFilter">(Optional) A filter specifying the kind.</param>
    /// <returns>
    /// An enumerator that allows foreach to be used to process the system FHIR operations in this
    /// collection.
    /// </returns>
    private IEnumerable<OperationDefinition> GetSystemFhirOperations(
        OperationDefinition.OperationKind? kindFilter = null)
    {
        List<OperationDefinition> operations = new();

        if (_caps != null)
        {
            foreach (CapabilityStatement.OperationComponent capOp in _caps.Rest.SelectMany(rest => rest.Operation))
            {
                if (!_dc.TryResolveByCanonicalUri(capOp.Definition, out Resource? resolvedResource) ||
                    (resolvedResource is not OperationDefinition fhirOp))
                {
                    // we cannot do anything with an operation that does not resolve
                    continue;
                }

                // only interested in system-level operations here
                if (fhirOp.System != true)
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

        // if there are no capabilities, just check everything in the package
        foreach (OperationDefinition fhirOp in _dc.OperationsByUrl.Values)
        {
            if (fhirOp.System != true)
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
            OperationId = "Get.Metadata".ToConvention(_idNamingConvention),
            Summary =
                _options.IncludeSummaries
                ? "Read metadata information about this server."
                : null,
            Description =
                _options.IncludeDescriptions
                ? "Read metadata."
                : null,
        };

        operation.Tags.Add(SYSTEM_TAG_REF);
        operation.Responses = BuildResponses([ 200 ], resourceName, schemas);

        return operation;
    }

    /// <summary>Builds a content map.</summary>
    /// <param name="resourceName">Name of the resource.</param>
    /// <param name="wrapInBundle">True to wrap in bundle.</param>
    /// <returns>A Dictionary of MIME Types and matching ApiOpenMediaTypes.</returns>
    private Dictionary<string, OpenApiMediaType> BuildContentMap(
        string resourceName,
        Dictionary<string, OpenApiSchema> schemas)
    {
        Dictionary<string, OpenApiMediaType> mediaTypes = [];

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

        if (_options.SchemaStyle == OaSchemaStyleCodes.Inline)
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
                    Id = BuildTypeName(name),
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
        Dictionary<string, OpenApiMediaType> mediaTypes = [];

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

    /// <summary>Gets the filtered resources in this collection.</summary>
    /// <param name="sort">(Optional) True to sort.</param>
    /// <returns>
    /// An enumerator that allows foreach to be used to process the filtered resources in this
    /// collection.
    /// </returns>
    private IEnumerable<StructureDefinition> GetFilteredResources(bool sort = false)
    {
        if (sort)
        {
            _dc.ResourcesByName.Values.Where(c => IncludeResource(c.Name)).OrderBy(c => c.Name);
        }

        return _dc.ResourcesByName.Values.Where(c => IncludeResource(c.Name));
    }

    /// <summary>Determine if we should filter out resource.</summary>
    /// <param name="resourceName">Name of the resource.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool IncludeResource(string resourceName)
    {
        // always include Resource, Bundle, and OperationOutcome for schema
        if ((resourceName == "Resource") ||
            (resourceName == "Bundle") ||
            (resourceName == "OperationOutcome"))
        {
            return true;
        }

        // check to see if the metadata endpoint is enabled and force CapabilityStatement or Conformance
        if ((_options.InteractionCapabilities != OaCapabilityBoolean.False) &&
            (resourceName == "CapabilityStatement") || (resourceName == "Conformance"))
        {
            return true;
        }

        // do not include resources that are not in the capability statement
        if ((_capResources.Count != 0) &&
            (!_capResources.ContainsKey(resourceName)))
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
    private List<OpenApiServer>? BuildServers()
    {
        if (_caps == null)
        {
            return null;
        }

        string description;

        if (!string.IsNullOrEmpty(_caps.Implementation?.Description))
        {
            description = SanitizeDescription(_caps.Implementation!.Description);
        }
        else if (!string.IsNullOrEmpty(_caps.Software?.Name))
        {
            description = SanitizeDescription(_caps.Software!.Name);
        }
        else
        {
            description = $"FHIR Server Version: {_dc.FhirVersionLiteral}";
        }

        return new List<OpenApiServer>()
        {
            new OpenApiServer()
            {
                Url = string.IsNullOrEmpty(_fhirBaseUrl) ? _caps.Url : _fhirBaseUrl,
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

        if (!string.IsNullOrEmpty(_options.Title))
        {
            title = _options.Title;
        }
        else if (!string.IsNullOrEmpty(_caps?.Title))
        {
            title = _caps!.Title;
        }
        else if (!string.IsNullOrEmpty(_caps?.Software.Name))
        {
            title = _caps!.Software.Name;
        }
        else
        {
            title = $"FHIR {_dc.FhirSequence}:{_dc.FhirVersionLiteral}";
        }

        string version;

        if (!string.IsNullOrEmpty(_options.DefinitionVersion))
        {
            version = _options.DefinitionVersion;
        }
        else if (!string.IsNullOrEmpty(_caps?.Software.Version))
        {
            version = _caps!.Software.Version;
        }
        else if (_caps?.FhirVersion != null)
        {
            version = _caps.FhirVersion.GetLiteral()!;
        }
        else
        {
            version = _dc.FhirVersionLiteral;
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

        switch (_options.SchemaLevel)
        {
            case OaSchemaLevelCodes.None:
                {
                    schemas.Add(
                        "resource".ToConvention(_idNamingConvention),
                        new OpenApiSchema()
                        {
                            Type = "object",
                            Description =
                                _options.IncludeDescriptions
                                ? "A FHIR Resource"
                                : null,
                        });
                }
                break;

            case OaSchemaLevelCodes.Names:
                {
                    foreach (StructureDefinition sd in GetFilteredResources(true))
                    {
                        string name = BuildTypeName(sd.Name);

                        if (schemas.ContainsKey(name))
                        {
                            continue;
                        }

                        OpenApiSchema schema = new OpenApiSchema()
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>(),
                            Description =
                                _options.IncludeDescriptions
                                ? SanitizeDescription(sd.Description)
                                : null,
                        };

                        schemas.Add(name, schema);
                    }
                }
                break;

            case OaSchemaLevelCodes.Detailed:
                BuildDetailedSchemas(schemas);
                break;
        }

        return schemas;
    }

    private void BuildDetailedSchemas(Dictionary<string, OpenApiSchema> schemas)
    {
        switch (_options.SchemaStyle)
        {
            case OaSchemaStyleCodes.TypeReferences:
                {
                    //foreach (FhirComplex complex in _dc.ComplexTypes.Values.OrderBy(c => c.Name))
                    //{
                    //    BuildSchema(
                    //        schemas,
                    //        complex,
                    //        null,
                    //        false);
                    //}

                    //foreach (FhirComplex complex in GetFilteredResources(true))
                    //{
                    //    string name = BuildTypeNameFromPath(complex.Path);

                    //    if (schemas.ContainsKey(name))
                    //    {
                    //        continue;
                    //    }

                    //    BuildSchema(
                    //        schemas,
                    //        complex,
                    //        null,
                    //        true);
                    //}
                }
                break;

            case OaSchemaStyleCodes.BackboneReferences:
                break;

            case OaSchemaStyleCodes.Inline:
                {
                    foreach (StructureDefinition sd in GetFilteredResources(true))
                    {
                        string name = BuildTypeName(sd.Name);

                        if (schemas.ContainsKey(name))
                        {
                            continue;
                        }

                        OpenApiSchema schema = new OpenApiSchema();

                        BuildInlineSchema(
                            ref schema,
                            new ComponentDefinition(sd));

                        schemas.Add(name, schema);
                    }
                }
                break;

            default:
                break;
        }
    }


    private void BuildInlineSchema(
        ref OpenApiSchema schema,
        ComponentDefinition cd,
        int currentDepth = 0,
        StructureDefinition? dataTypeSd = null,
        ElementDefinition? element = null)
    {
        bool isArray = (element == null) ? false : element.cgIsArray();

        if (isArray)
        {
            schema.Type = "array";
            schema.Items = new OpenApiSchema()
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>(),
            };
        }
        else
        {
            schema.Type = "object";
            schema.Properties = new Dictionary<string, OpenApiSchema>();
        }

        if (string.IsNullOrEmpty(schema.Description))
        {
            schema.Description =
                    _options.IncludeDescriptions
                    ? SanitizeDescription(element?.cgShort() ?? cd.cgShort())
                    : null;
        }

        if ((_options.MaxRecursions > 0) && (currentDepth > _options.MaxRecursions))
        {
            return;
        }

        // check for nesting into extension
        if (dataTypeSd?.Id == "Extension")
        {
            // do not continue here
            return;
        }

        IEnumerable<ElementDefinition> elements =
            dataTypeSd?.cgElements(topLevelOnly: true, includeRoot: false)
            ?? cd.cgGetChildren(includeDescendants: false).OrderBy(ed => ed.cgFieldOrder());

        // traverse elements at this level
        foreach (ElementDefinition ed in elements)
        {
            // check for extensions based on policy
            if ((ed.cgName() == "extension") &&
                (!ShouldAddExtension(ed.Path, element?.Path ?? cd.Element.Path)))
            {
                continue;
            }

            // check for elements that should be skipped
            if (SkipAsUncommon(ed, cd.Structure))
            {
                continue;
            }

            // check for elements that have been profiled out
            if (ed.cgCardinalityMax() == 0)
            {
                continue;
            }

            // start building our element schema
            OpenApiSchema elementSchema = new OpenApiSchema()
            {
                Description =
                    _options.IncludeDescriptions
                    ? SanitizeDescription(ed.cgShort())
                    : null,
            };

            string name = ed.cgName().Replace("[x]", "", StringComparison.Ordinal);

            // check for paths that have special handling
            switch (ed.Path)
            {
                case "Reference.identifier":
                    {
                        elementSchema.Type = "object";
                        schema.Properties.Add(
                            name,
                            elementSchema);
                        continue;
                    }

                case "Bundle.entry.request":
                    {
                        if (_options.RemoveUncommonFields &&
                            !cd.IsRootOfStructure)
                        {
                            elementSchema.Type = "object";
                            schema.Properties.Add(
                                name,
                                elementSchema);
                            continue;
                        }
                    }
                    break;
            }

            // get the types for this element
            IReadOnlyDictionary<string, ElementDefinition.TypeRefComponent> ets = ed.cgTypes();

            if (ets.Count == 0)
            {
                string typeName = ed.cgBaseTypeName(_dc, true);

                if (_dc.PrimitiveTypesByName.ContainsKey(typeName))
                {
                    SetSchemaType(typeName, ref elementSchema, ed.cgCardinalityMax() > 1);
                }
                else if (_dc.ComplexTypesByName.TryGetValue(typeName, out StructureDefinition? cSd))
                {
                    // nest into this type
                    BuildInlineSchema(
                        ref elementSchema,
                        cd,
                        currentDepth + 1,
                        cSd,
                        ed);
                }
                else if (_dc.ResourcesByName.TryGetValue(typeName, out StructureDefinition? rSd))
                {
                    // nest into this resource
                    BuildInlineSchema(
                        ref elementSchema,
                        cd,
                        currentDepth + 1,
                        rSd,
                        ed);
                }
                else if (_dc.HasChildElements(ed.Path))
                {
                    // nest into this type
                    BuildInlineSchema(
                        ref elementSchema,
                        cd with { Element = ed, IsRootOfStructure = false, },
                        currentDepth + 1);
                }
                else if (_dc.TryFindElementByPath(typeName, out StructureDefinition? namedRefSd, out ElementDefinition? namedRefEd))
                {
                    // nest into this type
                    BuildInlineSchema(
                        ref elementSchema,
                        new ComponentDefinition()
                        {
                            Structure = namedRefSd,
                            Element = namedRefEd,
                            IsRootOfStructure = false,
                        },
                        currentDepth + 1);
                }
                else
                {
                    SetSchemaType(typeName, ref elementSchema);
                }

                if (isArray)
                {
                    schema.Items.Properties.Add(name, elementSchema);
                }
                else
                {
                    schema.Properties.Add(name, elementSchema);
                }
            }
            else
            {
                foreach (ElementDefinition.TypeRefComponent et in ets.Values)
                {
                    OpenApiSchema subSchema = new OpenApiSchema()
                    {
                        Description =
                            _options.IncludeDescriptions
                            ? SanitizeDescription(ed.cgShort())
                            : null,
                    };

                    string typeName = et.cgName();

                    if (_dc.PrimitiveTypesByName.ContainsKey(typeName))
                    {
                        SetSchemaType(typeName, ref subSchema, ed.cgCardinalityMax() > 1);
                    }
                    else if (_dc.ComplexTypesByName.TryGetValue(typeName, out StructureDefinition? cSd))
                    {
                        if (((typeName == "BackboneElement") || (typeName == "Element")) &&
                            _dc.HasChildElements(ed.Path))
                        {
                            // nest into this type
                            BuildInlineSchema(
                                ref subSchema,
                                cd with { Element = ed, IsRootOfStructure = false, },
                                currentDepth + 1);
                        }
                        else
                        {
                            // nest into this type
                            BuildInlineSchema(
                                ref subSchema,
                                cd,
                                currentDepth + 1,
                                cSd,
                                ed);
                        }
                    }
                    else if (_dc.ResourcesByName.TryGetValue(typeName, out StructureDefinition? rSd))
                    {
                        // nest into this resource
                        BuildInlineSchema(
                            ref subSchema,
                            cd,
                            currentDepth + 1,
                            rSd,
                            ed);
                    }
                    else if (_dc.HasChildElements(ed.Path))
                    {
                        // nest into this type
                        BuildInlineSchema(
                            ref subSchema,
                            cd with { Element = ed, IsRootOfStructure = false, },
                            currentDepth + 1);
                    }
                    else
                    {
                        SetSchemaType(typeName, ref subSchema);
                    }

                    if (ets.Count == 1)
                    {
                        if (isArray)
                        {
                            schema.Items.Properties.Add(name, subSchema);
                        }
                        else
                        {
                            schema.Properties.Add(name, subSchema);
                        }
                    }
                    else
                    {
                        if (isArray)
                        {
                            schema.Items.Properties.Add($"{name}{typeName.ToPascalCase()}", subSchema);
                        }
                        else
                        {
                            schema.Properties.Add($"{name}{typeName.ToPascalCase()}", subSchema);
                        }
                    }
                }
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
        switch (_options.ExtensionSupport)
        {
            case ExtensionSupportLevel.None:
                return false;

            case ExtensionSupportLevel.OfficialNonPrimitive:
                if (_dc.PrimitiveTypesByName.ContainsKey(path))
                {
                    return false;
                }

                if (_dc.ExtensionsByPath.ContainsKey(path) ||
                    _dc.ExtensionsByPath.ContainsKey(parentPath))
                {
                    return true;
                }

                break;

            case ExtensionSupportLevel.Official:
                if (_dc.ExtensionsByPath.ContainsKey(path) ||
                    _dc.ExtensionsByPath.ContainsKey(parentPath))
                {
                    return true;
                }

                break;

            case ExtensionSupportLevel.NonPrimitive:
                if (_dc.PrimitiveTypesByName.ContainsKey(path))
                {
                    return false;
                }

                return true;

            case ExtensionSupportLevel.All:
                return true;

            // TODO: add support for this option
            case ExtensionSupportLevel.ByExtensionUrl:
                break;

            // TODO: add support for this option
            case ExtensionSupportLevel.ByElementPath:
                break;
        }

        return false;
    }

    /// <summary>Skip as uncommon.</summary>
    /// <param name="element">The element.</param>
    /// <param name="parent"> The parent.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool SkipAsUncommon(
        ElementDefinition element,
        StructureDefinition parent)
    {
        if (!_options.RemoveUncommonFields)
        {
            return false;
        }

        if (_uncommonFields.Contains(element.Path))
        {
            return true;
        }

        if ((parent != null) &&
            _uncommonFields.Contains($"{parent.Name}.{element.cgName()}"))
        {
            return true;
        }

        return false;
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
                    Id = BuildTypeName(baseType),
                    Type = ReferenceType.Schema,
                },
            };
        }
        else
        {
            schema.Reference = new OpenApiReference()
            {
                Id = BuildTypeName(baseType),
                Type = ReferenceType.Schema,
            };
        }
    }

    /// <summary>Gets element name.</summary>
    /// <param name="element">The element.</param>
    /// <returns>The element name.</returns>
    private static string GetElementName(ElementDefinition element)
    {
        string name = element.cgName().Replace("[x]", string.Empty);

        return FhirSanitizationUtils.SanitizedToConvention(name, FhirNameConventionExtensions.NamingConvention.CamelCase);
    }


    /// <summary>Builds type name from path.</summary>
    /// <param name="type">The type.</param>
    /// <returns>A string.</returns>
    private string BuildTypeName(string type, string explicitTypeName = "")
    {
        if (_dc.FhirSequence == CodeGenCommon.Packaging.FhirReleases.FhirSequenceCodes.DSTU2)
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

        if (!string.IsNullOrEmpty(explicitTypeName))
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
                    sb.Append(FhirSanitizationUtils.SanitizedToConvention(components[i], FhirNameConventionExtensions.NamingConvention.PascalCase));
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
            return _options.PerformDescriptionValidation ? "N/A." : string.Empty;
        }

        value = _markdownLinkRegex.Replace(value, "$1(http://hl7.org/fhir/$2)");    // prefix those links with HL7 FHIR spec base

        if (!_options.PerformDescriptionValidation)
        {
            return value;
            //return value.Replace("\r", string.Empty).Replace("\n", string.Empty);
        }

        value = value.Replace("\r", string.Empty).Replace("\n", string.Empty);

        if (!value.EndsWith('.'))
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

        if (value.Length > _options.DescriptionMaxLength)
        {
            value = value.Substring(0, _options.DescriptionMaxLength - 3) + "...";
        }

        return value;
    }
}
