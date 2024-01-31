// <copyright file="FhirVersionInfo.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.IO;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.SpecManager.Converters;

namespace Microsoft.Health.Fhir.SpecManager.Manager;

/// <summary>Information about a FHIR package.</summary>
public class FhirVersionInfo : IPackageImportable, IPackageExportable
{
    private static HashSet<string> _npmFilesToIgnore = new()
    {
        ".index.json",
        "package.json",
        "StructureDefinition-example.json",
    };

    private IFhirConverter _fhirConverter;
    private Dictionary<string, FhirPrimitive> _primitiveTypesByName;
    private Dictionary<string, FhirComplex> _complexTypesByName;
    private Dictionary<string, FhirComplex> _resourcesByName;
    private Dictionary<string, FhirComplex> _logicalModelsByName;
    private Dictionary<string, FhirComplex> _extensionsByUrl;
    private Dictionary<string, Dictionary<string, FhirComplex>> _extensionsByPath;
    private Dictionary<string, FhirComplex> _profilesByUrl;
    private Dictionary<string, Dictionary<string, FhirComplex>> _profilesByBaseType;
    private Dictionary<string, FhirOperation> _systemOperations;
    private Dictionary<string, FhirOperation> _operationsByUrl;
    private Dictionary<string, FhirSearchParam> _globalSearchParameters;
    private Dictionary<string, FhirSearchParam> _searchResultParameters;
    private Dictionary<string, FhirSearchParam> _allInteractionParameters;
    private Dictionary<string, FhirSearchParam> _searchParamsByUrl;
    private Dictionary<string, FhirCodeSystem> _codeSystemsByUrl;
    private Dictionary<string, FhirValueSetCollection> _valueSetsByUrl;
    private Dictionary<string, FhirNodeInfo> _nodeInfoByPath;
    private Dictionary<FhirArtifactClassEnum, List<FhirArtifactRecord>> _artifactsByClass;
    private Dictionary<string, FhirArtifactClassEnum> _artifactClassByUrl;
    private Dictionary<string, FhirImplementationGuide> _igsByUri;
    private Dictionary<string, FhirCapabiltyStatement> _capsByUrl;
    private Dictionary<string, FhirCompartment> _compartmentsByUrl;

    private HashSet<string> _excludedKeys;

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirVersionInfo"/> class.
    /// </summary>
    public FhirVersionInfo()
    {
        // create our info dictionaries
        _primitiveTypesByName = new();
        _complexTypesByName = new();
        _resourcesByName = new();
        _logicalModelsByName = new();
        _extensionsByUrl = new();
        _extensionsByPath = new();
        _profilesByUrl = new();
        _profilesByBaseType = new();
        _systemOperations = new();
        _operationsByUrl = new();
        _globalSearchParameters = new();
        _searchResultParameters = new();
        _allInteractionParameters = new();
        _searchParamsByUrl = new();
        _codeSystemsByUrl = new();
        _valueSetsByUrl = new();
        _nodeInfoByPath = new();
        _artifactClassByUrl = new();
        _igsByUri = new();
        _capsByUrl = new();
        _compartmentsByUrl = new();

        _excludedKeys = new();

        _artifactsByClass = new();
        foreach (FhirArtifactClassEnum artifactClass in (FhirArtifactClassEnum[])Enum.GetValues(typeof(FhirArtifactClassEnum)))
        {
            _artifactsByClass.Add(artifactClass, new());
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirVersionInfo"/>
    /// class.
    /// </summary>
    /// <param name="fhirSequence">The fhir version.</param>
    public FhirVersionInfo(FhirPackageCommon.FhirSequenceEnum fhirSequence)
        : this()
    {
        _fhirConverter = ConverterHelper.ConverterForVersion(fhirSequence);
        FhirSequence = fhirSequence;

        PackageDetails = new NpmPackageDetails()
        {
            Name = "resolved.canonicals" + fhirSequence.ToString().ToLowerInvariant(),
            Version = "0.0.0",
            URL = new Uri("http://localhost/" + fhirSequence.ToString().ToLowerInvariant()),
            Canonical = "http://localhost/" + fhirSequence.ToString().ToLowerInvariant(),
        };

        VersionString = "0.0.0";
        PackageName = PackageDetails.Name;
        CanonicalUrl = PackageDetails.URL;
    }

    /// <summary>Initializes a new instance of the <see cref="FhirVersionInfo"/> class.</summary>
    /// <exception cref="DirectoryNotFoundException">Thrown when the requested directory is not
    ///  present.</exception>
    /// <param name="packageDirectory">Pathname of the package directory.</param>
    public FhirVersionInfo(string packageDirectory)
        : this()
    {
        if (!Directory.Exists(packageDirectory))
        {
            throw new DirectoryNotFoundException($"Package directory {packageDirectory} not found!");
        }

        string directive = Path.GetFileName(packageDirectory);

        PackageDetails = NpmPackageDetails.Load(packageDirectory);
        CanonicalUrl = PackageDetails.URL;

        if (FhirPackageCommon.PackageIsFhirCore(PackageDetails.Name))
        {
            LoadFromCorePackage(directive);
        }
        else
        {
            LoadFromGuidePackage(directive);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirVersionInfo"/> class. Require major version
    /// (release #) to validate it is supported.
    /// </summary>
    /// <param name="source"> Source for the.</param>
    /// <param name="options">Options for controlling the operation.</param>
    public FhirVersionInfo(FhirVersionInfo source, PackageCopyOptions options, bool resolveExternal)
        : this()
    {
        _fhirConverter = ConverterHelper.ConverterForVersion(source.FhirSequence);
        FhirSequence = source.FhirSequence;

        CanonicalUrl = source.CanonicalUrl;
#pragma warning disable CS0618 // Type or member is obsolete
        MajorVersion = source.MajorVersion;
#pragma warning restore CS0618 // Type or member is obsolete
        ReleaseName = source.ReleaseName;
        BallotPrefix = source.BallotPrefix;
        PackageName = source.PackageName;
        ExamplesPackageName = source.ExamplesPackageName;
        ExpansionsPackageName = source.ExpansionsPackageName;
        VersionString = source.VersionString;
        IsDevBuild = source.IsDevBuild;
        DevBranch = source.DevBranch;
        BuildId = source.BuildId;
        IsLocalBuild = source.IsLocalBuild;
        LocalDirectory = source.LocalDirectory;
        IsOnDisk = source.IsOnDisk;
        LastDownloaded = source.LastDownloaded;
        PackageType = source.PackageType;
        PackageDetails = source.PackageDetails;     // TODO(ginoc): need to make a copy constructor for this type

        bool restrictOutput = false;
        bool restrictResources = false;

        HashSet<string> exportSet = new();
        _excludedKeys = new();

        // figure out all the the dependencies we need to include based on requests
        if (options.ExportList != null)
        {
            foreach (string path in options.ExportList)
            {
                AddToExportSet(path, exportSet, source);
            }

            if (exportSet.Count > 0)
            {
                restrictOutput = true;
            }
        }

        // only want server restrictions if there is not an explicit one
        if ((options.CapStatmentFilter != null) &&
            (exportSet.Count == 0))
        {
            foreach (FhirCapResource resource in options.CapStatmentFilter.ResourceInteractions.Values)
            {
                AddToExportSet(resource.ResourceType, exportSet, source);
            }

            if (exportSet.Count > 0)
            {
                restrictResources = true;

                // make sure Bundle is included so we can search, etc.
                AddToExportSet("Bundle", exportSet, source);
            }
        }

        Dictionary<string, ValueSetReferenceInfo> valueSetReferences = new();

        // check if we are exporting primitives
        if (options.CopyPrimitives)
        {
            foreach (KeyValuePair<string, FhirPrimitive> kvp in source._primitiveTypesByName)
            {
                // check for restricting output
                if (restrictOutput && (!exportSet.Contains(kvp.Key)))
                {
                    _excludedKeys.Add(kvp.Key);
                    continue;
                }

                // check for experimental - unless this is specifically included
                if ((!restrictOutput) &&
                    (!options.IncludeExperimental) &&
                    kvp.Value.IsExperimental)
                {
                    _excludedKeys.Add(kvp.Key);
                    continue;
                }

                FhirPrimitive node = (FhirPrimitive)kvp.Value.Clone();

                _nodeInfoByPath.Add(
                    node.Path,
                    new FhirNodeInfo(FhirNodeInfo.FhirNodeType.Primitive, node));

                _primitiveTypesByName.Add(kvp.Key, node);

                // update type to reflect language
                if (options.PrimitiveTypeMap.ContainsKey(kvp.Value.Name))
                {
                    node.BaseTypeName = options.PrimitiveTypeMap[kvp.Value.Name];
                }
            }
        }

        // check if we are exporting complex types
        if (options.CopyComplexTypes)
        {
            foreach (KeyValuePair<string, FhirComplex> kvp in source._complexTypesByName)
            {
                // check for restricting output
                if (restrictOutput && (!exportSet.Contains(kvp.Key)))
                {
                    _excludedKeys.Add(kvp.Key);
                    continue;
                }

                // check for experimental - unless this is specifically included
                if ((!restrictOutput) &&
                    (!options.IncludeExperimental) &&
                    kvp.Value.IsExperimental)
                {
                    _excludedKeys.Add(kvp.Key);
                    continue;
                }

                FhirComplex node = kvp.Value.DeepCopy(
                    options.PrimitiveTypeMap,
                    true,
                    false,
                    valueSetReferences,
                    _nodeInfoByPath,
                    null,
                    null,
                    null,
                    null,
                    options.IncludeExperimental);

                _nodeInfoByPath.Add(
                    node.Path,
                    new FhirNodeInfo(FhirNodeInfo.FhirNodeType.DataType, node));

                _complexTypesByName.Add(
                    kvp.Key,
                    node);
            }
        }

        // check if we are exporting resources
        if (options.CopyResources)
        {
            foreach (KeyValuePair<string, FhirComplex> kvp in source._resourcesByName)
            {
                // check for restricting output
                if (restrictOutput && (!exportSet.Contains(kvp.Key)))
                {
                    _excludedKeys.Add(kvp.Key);
                    continue;
                }

                if (restrictResources && (!exportSet.Contains(kvp.Key)))
                {
                    _excludedKeys.Add(kvp.Key);
                    continue;
                }

                // check for experimental - unless this is specifically included
                if (((!restrictOutput) || (!restrictResources)) &&
                    (!options.IncludeExperimental) &&
                    kvp.Value.IsExperimental)
                {
                    _excludedKeys.Add(kvp.Key);
                    continue;
                }

                if ((options.CapStatmentFilter == null) ||
                    (!options.CapStatmentFilter.ResourceInteractions.ContainsKey(kvp.Key)))
                {
                    FhirComplex node = kvp.Value.DeepCopy(
                        options.PrimitiveTypeMap,
                        true,
                        false,
                        valueSetReferences,
                        _nodeInfoByPath,
                        null,
                        null,
                        null,
                        null,
                        options.IncludeExperimental);

                    _nodeInfoByPath.Add(
                        node.Path,
                        new FhirNodeInfo(FhirNodeInfo.FhirNodeType.Resource, node));

                    _resourcesByName.Add(
                        kvp.Key,
                        node);
                }
                else
                {
                    FhirComplex node = kvp.Value.DeepCopy(
                        options.PrimitiveTypeMap,
                        true,
                        false,
                        valueSetReferences,
                        _nodeInfoByPath,
                        options.CapStatmentFilter.ResourceInteractions[kvp.Key].SearchParameters,
                        options.CapStatmentFilter.ServerSearchParameters,
                        options.CapStatmentFilter.ResourceInteractions[kvp.Key].Operations,
                        options.CapStatmentFilter.ServerOperations,
                        options.IncludeExperimental);

                    _nodeInfoByPath.Add(
                        node.Path,
                        new FhirNodeInfo(FhirNodeInfo.FhirNodeType.Resource, node));

                    _resourcesByName.Add(
                        kvp.Key,
                        node);
                }
            }
        }

        if (options.CopyLogicalModels)
        {
            foreach (KeyValuePair<string, FhirComplex> kvp in source._logicalModelsByName)
            {
                // check for restricting output
                if (restrictOutput && (!exportSet.Contains(kvp.Key)))
                {
                    _excludedKeys.Add(kvp.Key);
                    continue;
                }

                if (restrictResources && (!exportSet.Contains(kvp.Key)))
                {
                    _excludedKeys.Add(kvp.Key);
                    continue;
                }

                // check for experimental - unless this is specifically included
                if (((!restrictOutput) || (!restrictResources)) &&
                    (!options.IncludeExperimental) &&
                    kvp.Value.IsExperimental)
                {
                    _excludedKeys.Add(kvp.Key);
                    continue;
                }

                if ((options.CapStatmentFilter == null) ||
                    (!options.CapStatmentFilter.ResourceInteractions.ContainsKey(kvp.Key)))
                {
                    FhirComplex node = kvp.Value.DeepCopy(
                        options.PrimitiveTypeMap,
                        true,
                        false,
                        valueSetReferences,
                        _nodeInfoByPath,
                        null,
                        null,
                        null,
                        null,
                        options.IncludeExperimental);

                    _nodeInfoByPath.Add(
                        node.Path,
                        new FhirNodeInfo(FhirNodeInfo.FhirNodeType.LogicalModel, node));

                    _logicalModelsByName.Add(
                        kvp.Key,
                        node);
                }
                else
                {
                    FhirComplex node = kvp.Value.DeepCopy(
                        options.PrimitiveTypeMap,
                        true,
                        false,
                        valueSetReferences,
                        _nodeInfoByPath,
                        options.CapStatmentFilter.ResourceInteractions[kvp.Key].SearchParameters,
                        options.CapStatmentFilter.ServerSearchParameters,
                        options.CapStatmentFilter.ResourceInteractions[kvp.Key].Operations,
                        options.CapStatmentFilter.ServerOperations,
                        options.IncludeExperimental);

                    _nodeInfoByPath.Add(
                        node.Path,
                        new FhirNodeInfo(FhirNodeInfo.FhirNodeType.LogicalModel, node));

                    _logicalModelsByName.Add(
                        kvp.Key,
                        node);
                }
            }
        }

        if (options.CopyProfiles)
        {
            foreach ((string profileFor, Dictionary<string, FhirComplex> profiles) in source._profilesByBaseType)
            {
                if (restrictOutput && (!exportSet.Contains(profileFor)))
                {
                    continue;
                }

                if (restrictResources && (!exportSet.Contains(profileFor)))
                {
                    continue;
                }

                foreach (FhirComplex profile in profiles.Values)
                {
                    FhirComplex node = profile.DeepCopy(
                        options.PrimitiveTypeMap,
                        true,
                        false,
                        valueSetReferences,
                        null,
                        null,
                        null,
                        null,
                        null,
                        options.IncludeExperimental);

                    _nodeInfoByPath.Add(
                        node.Id,
                        new FhirNodeInfo(FhirNodeInfo.FhirNodeType.Profile, node));

                    AddProfile(node);
                }
            }
        }

        if (options.CopySearchParameters)
        {
            if (restrictOutput || restrictResources)
            {
                foreach (FhirSearchParam sp in source._searchParamsByUrl.Values)
                {
                    if ((sp.ResourceTypes == null) || (!sp.ResourceTypes.Any()))
                    {
                        AddSearchParameter((FhirSearchParam)sp.Clone());
                        continue;
                    }

                    foreach (string spBaseType in sp.ResourceTypes)
                    {
                        if (exportSet.Contains(spBaseType))
                        {
                            AddSearchParameter((FhirSearchParam)sp.Clone());
                            break;
                        }
                    }
                }
            }
            else
            {
                foreach (FhirSearchParam sp in source._searchParamsByUrl.Values)
                {
                    AddSearchParameter((FhirSearchParam)sp.Clone());
                }
            }

            // resolve composite parameter info
            foreach (FhirSearchParam sp in _searchParamsByUrl.Values)
            {
                if (sp.Components?.Any() ?? false)
                {
                    sp.Resolve(_searchParamsByUrl);
                }
            }

            // traverse resources looking for search parameters to resolve as well
            foreach (FhirComplex fc in _resourcesByName.Values)
            {
                if (!(fc.SearchParameters?.Any() ?? false))
                {
                    continue;
                }

                foreach (FhirSearchParam sp in fc.SearchParameters.Values)
                {
                    if (sp.Components?.Any() ?? false)
                    {
                        sp.Resolve(_searchParamsByUrl);
                    }
                }
            }
        }

        if (options.CopyOperations)
        {
            if (options.CapStatmentFilter == null)
            {
                if (restrictOutput || restrictResources)
                {
                    foreach (FhirOperation op in source._operationsByUrl.Values)
                    {
                        foreach (string opBaseType in op.ResourceTypes)
                        {
                            if (exportSet.Contains(opBaseType))
                            {
                                AddOperation((FhirOperation)op.Clone());
                                break;
                            }
                        }
                    }
                }
                else
                {
                    foreach (FhirOperation op in source._operationsByUrl.Values)
                    {
                        AddOperation((FhirOperation)op.Clone());
                    }
                }
            }
            else
            {
                if (options.CapStatmentFilter.ServerOperations != null)
                {
                    foreach (FhirCapOperation serverOp in options.CapStatmentFilter.ServerOperations.Values)
                    {
                        if (FhirManager.Current.TryResolveCanonical(
                                FhirSequence,
                                string.Empty,
                                "OperationDefinition",
                                serverOp.DefinitionCanonical,
                                resolveExternal,
                                out FhirArtifactClassEnum ac,
                                out object resource) &&
                            (ac == FhirArtifactClassEnum.Operation))
                        {
                            AddOperation((FhirOperation)((FhirOperation)resource).Clone());
                        }
                    }
                }

                if (options.CapStatmentFilter.ResourceInteractions != null)
                {
                    foreach (FhirCapResource resourceInteraction in options.CapStatmentFilter.ResourceInteractions.Values)
                    {
                        if (resourceInteraction.Operations != null)
                        {
                            foreach (FhirCapOperation resourceOp in resourceInteraction.Operations.Values)
                            {
                                if (FhirManager.Current.TryResolveCanonical(
                                        FhirSequence,
                                        string.Empty,
                                        "OperationDefinition",
                                        resourceOp.DefinitionCanonical,
                                        resolveExternal,
                                        out FhirArtifactClassEnum ac,
                                        out object resource) &&
                                    (ac == FhirArtifactClassEnum.Operation))
                                {
                                    AddOperation((FhirOperation)((FhirOperation)resource).Clone());
                                }
                            }
                        }
                    }
                }
            }
        }

        bool checkUrls = (options.ExtensionUrls != null) && (options.ExtensionUrls.Count != 0);
        bool checkPaths = (options.ExtensionElementPaths != null) && (options.ExtensionElementPaths.Count != 0);

        if (options.CopyExtensions)
        {
            // need to work directly with extensions due to nature of filtering
            foreach (FhirComplex extension in source._extensionsByUrl.Values)
            {
                // check for restricting output
                if (restrictOutput)
                {
                    foreach (string path in extension.ContextElements)
                    {
                        string[] components = path.Split('.');
                        if (!exportSet.Contains(components[0]))
                        {
                            continue;
                        }
                    }
                }

                // check for including extensions by url
                if (checkUrls && (!options.ExtensionUrls.Contains(extension.URL.ToString())))
                {
                    continue;
                }

                // check for including extensions by path
                if (checkPaths && (!extension.ContextElements.Union(options.ExtensionElementPaths).Any()))
                {
                    continue;
                }

                // add this extension using the primary function (adds to multiple dictionaries)
                AddExtension(
                    extension.DeepCopy(
                        options.PrimitiveTypeMap,
                        true,
                        false,
                        valueSetReferences,
                        null));
            }
        }

        if (options.CopyImplementationGuides)
        {
            foreach ((string key, FhirImplementationGuide ig) in source._igsByUri)
            {
                _igsByUri.Add(key, (FhirImplementationGuide)ig.Clone());
            }
        }

        if (options.CopyCompartments)
        {
            _compartmentsByUrl = source._compartmentsByUrl.DeepCopy();
        }

        if (options.CopyCapabilityStatements)
        {
            _capsByUrl = source._capsByUrl.DeepCopy();
        }

        if (options.CapStatmentFilter == null)
        {
            foreach (KeyValuePair<string, FhirOperation> kvp in source._systemOperations)
            {
                if (_systemOperations.ContainsKey(kvp.Key))
                {
                    continue;
                }

                _systemOperations.Add(kvp.Key, (FhirOperation)kvp.Value.Clone());
            }
        }
        else
        {
            foreach (KeyValuePair<string, FhirOperation> kvp in source._systemOperations)
            {
                if (_systemOperations.ContainsKey(kvp.Key))
                {
                    continue;
                }

                if (options.CapStatmentFilter.ServerOperations.ContainsKey(kvp.Value.Code))
                {
                    _systemOperations.Add(kvp.Key, (FhirOperation)kvp.Value.Clone());
                }
            }

            foreach (KeyValuePair<string, FhirCapOperation> kvp in options.CapStatmentFilter.ServerOperations)
            {
                if (_systemOperations.ContainsKey(kvp.Key))
                {
                    continue;
                }

                _systemOperations.Add(
                    kvp.Key,
                    new FhirOperation(
                        kvp.Key,
                        new Uri(kvp.Value.DefinitionCanonical),
                        string.Empty,
                        kvp.Value.Name,
                        kvp.Value.Documentation,
                        string.Empty,
                        string.Empty,
                        null,
                        null,
                        true,
                        false,
                        false,
                        kvp.Value.Name,
                        kvp.Value.Documentation,
                        string.Empty,
                        null,
                        new List<FhirParameter>(),
                        false,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        options.CapStatmentFilter.FhirVersion));
            }
        }

        if (options.CapStatmentFilter == null)
        {
            foreach (KeyValuePair<string, FhirSearchParam> kvp in source._globalSearchParameters)
            {
                if (_globalSearchParameters.ContainsKey(kvp.Key))
                {
                    continue;
                }

                if ((!options.IncludeExperimental) && kvp.Value.IsExperimental)
                {
                    continue;
                }

                _globalSearchParameters.Add(kvp.Key, (FhirSearchParam)kvp.Value.Clone());
            }
        }
        else
        {
            foreach (KeyValuePair<string, FhirSearchParam> kvp in source._globalSearchParameters)
            {
                if (_globalSearchParameters.ContainsKey(kvp.Key))
                {
                    continue;
                }

                if (options.CapStatmentFilter.ServerSearchParameters.ContainsKey(kvp.Value.Code))
                {
                    _globalSearchParameters.Add(kvp.Key, (FhirSearchParam)kvp.Value.Clone());
                }
            }
        }

        foreach (KeyValuePair<string, FhirSearchParam> kvp in source._searchResultParameters)
        {
            if (_searchResultParameters.ContainsKey(kvp.Key))
            {
                continue;
            }

            if ((!options.IncludeExperimental) && kvp.Value.IsExperimental)
            {
                continue;
            }

            _searchResultParameters.Add(kvp.Key, (FhirSearchParam)kvp.Value.Clone());
        }

        foreach (KeyValuePair<string, FhirSearchParam> kvp in source._allInteractionParameters)
        {
            if (_allInteractionParameters.ContainsKey(kvp.Key))
            {
                continue;
            }

            if ((!options.IncludeExperimental) && kvp.Value.IsExperimental)
            {
                continue;
            }

            _allInteractionParameters.Add(kvp.Key, (FhirSearchParam)kvp.Value.Clone());
        }

        foreach ((string vsUrl, FhirValueSetCollection vsCollection) in source._valueSetsByUrl)
        {
            foreach ((string vsVersion, FhirValueSet valueSet) in vsCollection.ValueSetsByVersion)
            {
                string key = $"{vsUrl}|{vsVersion}";

                if (HasValueSet(key))
                {
                    continue;
                }

                // check for restricted output and not seeing this valueSet
                if (restrictOutput)
                {
                    if ((!valueSetReferences.ContainsKey(vsUrl)) &&
                        (!exportSet.Contains(vsUrl)) &&
                        (!exportSet.Contains(key)))
                    {
                        continue;
                    }
                }

                valueSet.Resolve(source._codeSystemsByUrl);

                if ((valueSet.Concepts == null) ||
                    (valueSet.Concepts.Count == 0))
                {
                    continue;
                }

                if (!_valueSetsByUrl.ContainsKey(vsUrl))
                {
                    _valueSetsByUrl.Add(vsUrl, new FhirValueSetCollection(vsUrl));
                }

                FhirValueSet vs = (FhirValueSet)valueSet.Clone();

                if (valueSetReferences.ContainsKey(vsUrl))
                {
                    vs.SetReferences(valueSetReferences[vsUrl]);
                }

                _valueSetsByUrl[vsUrl].AddValueSet(vs);
            }
        }
    }

    /// <summary>Gets or sets the type of the package group.</summary>
    public FhirPackageCommon.FhirPackageTypeEnum PackageType { get; set; }

    /// <summary>Gets or sets the major version.</summary>
    public FhirPackageCommon.FhirSequenceEnum FhirSequence { get; set; }

    // TODO(ginoc): Switch to CachePackageManifest?
    /// <summary>Gets or sets the package details.</summary>
    public NpmPackageDetails PackageDetails { get; set; }

    /// <summary>Gets or sets canonical URL of this package.</summary>
    public Uri CanonicalUrl { get; set; }

    /// <summary>Gets or sets the name of the release.</summary>
    [Obsolete("R4B made major versions as integers tricky, use FhirSequence", false)]
    public int MajorVersion { get; set; }

    /// <summary>Gets or sets the name of the package release.</summary>
    public string ReleaseName { get; set; }

    /// <summary>Gets or sets the name of the package.</summary>
    public string PackageName { get; set; }

    /// <summary>Gets or sets the name of the examples package.</summary>
    public string ExamplesPackageName { get; set; }

    /// <summary>Gets or sets the name of the expansions package.</summary>
    public string ExpansionsPackageName { get; set; }

    /// <summary>Gets or sets the version string.</summary>
    public string VersionString { get; set; }

    /// <summary>Gets or sets the ballot prefix (e.g., 2021Jan).</summary>
    public string BallotPrefix { get; set; }

    /// <summary>Gets or sets a value indicating whether this object is development build.</summary>
    public bool IsDevBuild { get; set; }

    /// <summary>Gets or sets the development branch.</summary>
    public string DevBranch { get; set; }

    /// <summary>Gets or sets the identifier of the build.</summary>
    public string BuildId { get; set; }

    /// <summary>Gets or sets a value indicating whether this object is local build.</summary>
    public bool IsLocalBuild { get; set; }

    /// <summary>Gets or sets the pathname of the local directory.</summary>
    public string LocalDirectory { get; set; }

    /// <summary>Gets or sets a value indicating whether this object is on disk.</summary>
    public bool IsOnDisk { get; set; }

    /// <summary>Gets or sets the Date/Time of the last downloaded.</summary>
    public DateTime? LastDownloaded { get; set; }

    /// <summary>Gets a dictionary with the known primitive types for this version of FHIR.</summary>
    public Dictionary<string, FhirPrimitive> PrimitiveTypes { get => _primitiveTypesByName; }

    /// <summary>Gets a dictionary with the known complex types for this version of FHIR.</summary>
    public Dictionary<string, FhirComplex> ComplexTypes { get => _complexTypesByName; }

    /// <summary>Gets a dictionary with the known resources for this version of FHIR.</summary>
    public Dictionary<string, FhirComplex> Resources { get => _resourcesByName; }

    /// <summary>Gets a dictionary with the known logical models by name.</summary>
    public Dictionary<string, FhirComplex> LogicalModels { get => _logicalModelsByName; }

    /// <summary>Gets the profiles by id dictionary.</summary>
    public Dictionary<string, FhirComplex> Profiles { get => _profilesByUrl; }

    /// <summary>Gets a profiles by base type dictionary.</summary>
    public Dictionary<string, Dictionary<string, FhirComplex>> ProfilesByBaseType { get => _profilesByBaseType; }

    /// <summary>Gets an extensions by URL dictionary.</summary>
    public Dictionary<string, FhirComplex> ExtensionsByUrl { get => _extensionsByUrl; }

    /// <summary>Gets the code systems by URL dictionary.</summary>
    public Dictionary<string, FhirCodeSystem> CodeSystems { get => _codeSystemsByUrl; }

    /// <summary>Gets the value sets by URL dictionary.</summary>
    public Dictionary<string, FhirValueSetCollection> ValueSetsByUrl { get => _valueSetsByUrl; }

    /// <summary>Gets the extensions per path, in a dictionary keyed by URL.</summary>
    public Dictionary<string, Dictionary<string, FhirComplex>> ExtensionsByPath { get => _extensionsByPath; }

    /// <summary>Gets the system operations.</summary>
    public Dictionary<string, FhirOperation> SystemOperations { get => _systemOperations; }

    /// <summary>Gets known operations, keyed by URL.</summary>
    public Dictionary<string, FhirOperation> OperationsByUrl { get => _operationsByUrl; }

    /// <summary>Gets search parameters defined for all resources.</summary>
    public Dictionary<string, FhirSearchParam> AllResourceParameters { get => _globalSearchParameters; }

    /// <summary>Gets search parameters that control search results.</summary>
    public Dictionary<string, FhirSearchParam> SearchResultParameters { get => _searchResultParameters; }

    /// <summary>Gets search parameters defined for all interactions.</summary>
    public Dictionary<string, FhirSearchParam> AllInteractionParameters { get => _allInteractionParameters; }

    /// <summary>Gets known search parameters, keyed by URL.</summary>
    public Dictionary<string, FhirSearchParam> SearchParametersByUrl { get => _searchParamsByUrl; }

    /// <summary>Gets known implementation guides, keyed by URL.</summary>
    public Dictionary<string, FhirImplementationGuide> ImplementationGuidesByUrl { get => _igsByUri; }

    /// <summary>Gets known capability statements, keyed by URL.</summary>
    public Dictionary<string, FhirCapabiltyStatement> CapabilitiesByUrl { get => _capsByUrl; }

    /// <summary>Gets the compartments by URL.</summary>
    public Dictionary<string, FhirCompartment> CompartmentsByUrl { get => _compartmentsByUrl; }

    /// <summary>Gets the node info by path dictionary.</summary>
    public Dictionary<string, FhirNodeInfo> NodeByPath { get => _nodeInfoByPath; }

    /// <summary>Gets the artifacts by class.</summary>
    public Dictionary<FhirArtifactClassEnum, List<FhirArtifactRecord>> ArtifactRecordsByClass => _artifactsByClass;

    /// <summary>Gets URL of the artifact class by.</summary>
    public Dictionary<string, FhirArtifactClassEnum> ArtifactClassByUrl => _artifactClassByUrl;

    /// <summary>Gets the excluded keys.</summary>
    public HashSet<string> ExcludedKeys => _excludedKeys;

    /// <summary>Gets artifact class.</summary>
    /// <param name="token">The ID or URL of the artifact.</param>
    /// <returns>The artifact class.</returns>
    public FhirArtifactClassEnum GetArtifactClass(string token)
    {
        if (_artifactClassByUrl.ContainsKey(token))
        {
            return _artifactClassByUrl[token];
        }

        if (!string.IsNullOrEmpty(PackageDetails?.Canonical))
        {
            string joined = PackageDetails.Canonical + "/" + token;

            if (_artifactClassByUrl.ContainsKey(joined))
            {
                return _artifactClassByUrl[joined];
            }

            foreach (string definitionType in FhirPackageLoader.DefinitionalResourceTypesToLoad)
            {
                joined = PackageDetails.Canonical + "/" + definitionType + "/" + token;
                if (_artifactClassByUrl.ContainsKey(joined))
                {
                    return _artifactClassByUrl[joined];
                }
            }
        }

        foreach (string definitionType in FhirPackageLoader.DefinitionalResourceTypesToLoad)
        {
            string url = new Uri(CanonicalUrl, definitionType + "/" + token).ToString();
            if (_artifactClassByUrl.ContainsKey(url))
            {
                return _artifactClassByUrl[url];
            }
        }

        return FhirArtifactClassEnum.Unknown;
    }

    public bool AddCanonicalAlias(string canonicalUrl, string canonicalAlias)
    {
        if (string.IsNullOrEmpty(canonicalUrl) ||
            string.IsNullOrEmpty(canonicalAlias))
        {
            return false;
        }

        if (canonicalUrl.Equals(canonicalAlias, StringComparison.Ordinal) ||
            _artifactClassByUrl.ContainsKey(canonicalAlias))
        {
            return true;
        }

        if (!_artifactClassByUrl.TryGetValue(canonicalUrl, out FhirArtifactClassEnum ac))
        {
            return false;
        }

        _artifactClassByUrl.Add(canonicalAlias, ac);

        switch (ac)
        {
            case FhirArtifactClassEnum.Extension:
                _extensionsByUrl.Add(canonicalAlias, _extensionsByUrl[canonicalUrl]);
                return true;

            case FhirArtifactClassEnum.Operation:
                _operationsByUrl.Add(canonicalAlias, _operationsByUrl[canonicalUrl]);
                return true;

            case FhirArtifactClassEnum.SearchParameter:
                _searchParamsByUrl.Add(canonicalAlias, _searchParamsByUrl[canonicalUrl]);
                return true;

            case FhirArtifactClassEnum.CodeSystem:
                _codeSystemsByUrl.Add(canonicalAlias, _codeSystemsByUrl[canonicalUrl]);
                return true;

            case FhirArtifactClassEnum.ValueSet:
                _valueSetsByUrl.Add(canonicalAlias, _valueSetsByUrl[canonicalUrl]);
                return true;

            case FhirArtifactClassEnum.Profile:
                _profilesByUrl.Add(canonicalAlias, _profilesByUrl[canonicalUrl]);
                return true;

            case FhirArtifactClassEnum.ImplementationGuide:
                _igsByUri.Add(canonicalAlias, _igsByUri[canonicalUrl]);
                return true;

            case FhirArtifactClassEnum.CapabilityStatement:
                _capsByUrl.Add(canonicalAlias, _capsByUrl[canonicalUrl]);
                return true;

            case FhirArtifactClassEnum.Compartment:
                _compartmentsByUrl.Add(canonicalAlias, _compartmentsByUrl[canonicalUrl]);
                return true;

            case FhirArtifactClassEnum.LogicalModel:
            case FhirArtifactClassEnum.ConceptMap:
            case FhirArtifactClassEnum.NamingSystem:
            case FhirArtifactClassEnum.StructureMap:
            case FhirArtifactClassEnum.Resource:
            case FhirArtifactClassEnum.PrimitiveType:
            case FhirArtifactClassEnum.ComplexType:
            case FhirArtifactClassEnum.Unknown:
            default:
                break;
        }


        return false;
    }

    /// <summary>Resolve in dictionary.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="token">             The ID or URL of the artifact.</param>
    /// <param name="dict">              The dictionary.</param>
    /// <returns>An object.</returns>
    private object ResolveInDict<T>(
        string token,
        Dictionary<string, T> dict)
    {
        if (dict.ContainsKey(token))
        {
            return dict[token];
        }

        if (!string.IsNullOrEmpty(PackageDetails?.Canonical))
        {
            string joined = PackageDetails.Canonical + "/" + token;

            if (dict.ContainsKey(joined))
            {
                return dict[joined];
            }
        }

        if (token.Contains('/'))
        {
            string id = token.Substring(token.LastIndexOf('/') + 1);

            if (dict.ContainsKey(id))
            {
                return dict[id];
            }

            return null;
        }

        foreach (string definitionType in FhirPackageLoader.DefinitionalResourceTypesToLoad)
        {
            string url = new Uri(CanonicalUrl, definitionType + "/" + token).ToString();
            if (dict.ContainsKey(url))
            {
                return dict[url];
            }
        }

        return null;
    }

    /// <summary>
    /// Attempts to get artifact complexes an IEnumerable&lt;FhirComplex&gt; from the given
    /// FhirArtifactClassEnum.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="artifactClass">[out] The artifact class enum.</param>
    /// <param name="values">       [out] The artifact values.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool TryGetArtifactValues<T>(
        FhirArtifactClassEnum artifactClass,
        out IEnumerable<T> values)
    {
        switch (artifactClass)
        {
            case FhirArtifactClassEnum.PrimitiveType:
                {
                    if (typeof(T) != typeof(FhirPrimitive))
                    {
                        values = null;
                        return false;
                    }

                    values = (IEnumerable<T>)_primitiveTypesByName.Values.AsEnumerable<FhirPrimitive>();
                    return true;
                }

            case FhirArtifactClassEnum.ComplexType:
                {
                    if (typeof(T) != typeof(FhirComplex))
                    {
                        values = null;
                        return false;
                    }

                    values = (IEnumerable<T>)_complexTypesByName.Values.AsEnumerable<FhirComplex>();
                    return true;
                }

            case FhirArtifactClassEnum.Resource:
                {
                    if (typeof(T) != typeof(FhirComplex))
                    {
                        values = null;
                        return false;
                    }

                    values = (IEnumerable<T>)_resourcesByName.Values.AsEnumerable<FhirComplex>();
                    return true;
                }

            case FhirArtifactClassEnum.LogicalModel:
                {
                    if (typeof(T) != typeof(FhirComplex))
                    {
                        values = null;
                        return false;
                    }

                    values = (IEnumerable<T>)_logicalModelsByName.Values.AsEnumerable<FhirComplex>();
                    return true;
                }

            case FhirArtifactClassEnum.Extension:
                {
                    if (typeof(T) != typeof(FhirComplex))
                    {
                        values = null;
                        return false;
                    }

                    values = (IEnumerable<T>)_extensionsByUrl.Values.AsEnumerable<FhirComplex>();
                    return true;
                }

            case FhirArtifactClassEnum.Operation:
                {
                    if (typeof(T) != typeof(FhirOperation))
                    {
                        values = null;
                        return false;
                    }

                    values = (IEnumerable<T>)_operationsByUrl.Values.AsEnumerable<FhirOperation>();
                    return true;
                }

            case FhirArtifactClassEnum.SearchParameter:
                {
                    if (typeof(T) != typeof(FhirSearchParam))
                    {
                        values = null;
                        return false;
                    }

                    values = (IEnumerable<T>)_searchParamsByUrl.Values.AsEnumerable<FhirSearchParam>();
                    return true;
                }

            case FhirArtifactClassEnum.CodeSystem:
                {
                    if (typeof(T) != typeof(FhirCodeSystem))
                    {
                        values = null;
                        return false;
                    }

                    values = (IEnumerable<T>)_codeSystemsByUrl.Values.AsEnumerable<FhirCodeSystem>();
                    return true;
                }

            case FhirArtifactClassEnum.ValueSet:
                {
                    if (typeof(T) == typeof(FhirValueSetCollection))
                    {
                        values = (IEnumerable<T>)_valueSetsByUrl.Values.AsEnumerable<FhirValueSetCollection>();
                        return true;
                    }

                    if (typeof(T) == typeof(FhirValueSet))
                    {
                        List<FhirValueSet> vs = new List<FhirValueSet>();
                        foreach (FhirValueSetCollection vsc in _valueSetsByUrl.Values)
                        {
                            if (vsc.ValueSetsByVersion.Any())
                            {
                                vs.Add(vsc.ValueSetsByVersion.First().Value);
                            }
                        }

                        values = (IEnumerable<T>)vs.AsEnumerable<FhirValueSet>();
                        return true;
                    }

                    values = null;
                    return true;
                }

            case FhirArtifactClassEnum.Profile:
                {
                    if (typeof(T) != typeof(FhirComplex))
                    {
                        values = null;
                        return false;
                    }

                    values = (IEnumerable<T>)_profilesByUrl.Values.AsEnumerable();
                    return true;
                }

            case FhirArtifactClassEnum.ImplementationGuide:
                {
                    if (typeof(T) != typeof(FhirImplementationGuide))
                    {
                        values = null;
                        return false;
                    }

                    values = (IEnumerable<T>)_igsByUri.Values.AsEnumerable();
                    return true;
                }

            case FhirArtifactClassEnum.CapabilityStatement:
                {
                    if (typeof(T) != typeof(FhirCapabiltyStatement))
                    {
                        values = null;
                        return false;
                    }

                    values = (IEnumerable<T>)_capsByUrl.Values.AsEnumerable();
                    return true;
                }

            case FhirArtifactClassEnum.Compartment:
                {
                    if (typeof(T) != typeof(FhirCompartment))
                    {
                        values = null;
                        return false;
                    }

                    values = (IEnumerable<T>)_compartmentsByUrl.Values.AsEnumerable();
                    return true;
                }

            case FhirArtifactClassEnum.Unknown:
            case FhirArtifactClassEnum.ConceptMap:
            case FhirArtifactClassEnum.NamingSystem:
            case FhirArtifactClassEnum.StructureMap:
            default:
                {
                    values = null;
                    return false;
                }
        }
    }

    /// <summary>Information about the resolved artifact.</summary>
    /// <param name="ArtifactClass">  The artifact class.</param>
    /// <param name="ResolvedPackage">The resolved package.</param>
    /// <param name="Id">             The identifier.</param>
    /// <param name="Url">            URL of the resource.</param>
    /// <param name="Artifact">       The artifact.</param>
    public readonly record struct ResolvedArtifactRecord(
        FhirArtifactClassEnum ArtifactClass,
        string ResolvedPackage,
        object Artifact);

    /// <summary>Gets the artifacts in this collection.</summary>
    /// <param name="token">The ID or URL of the artifact.</param>
    /// <returns>
    /// An enumerator that allows foreach to be used to process the artifacts in this collection.
    /// </returns>
    public IEnumerable<ResolvedArtifactRecord> GetArtifacts(string token)
    {
        Dictionary<string, ResolvedArtifactRecord> recs = new();

        foreach (FhirArtifactClassEnum ac in Enum.GetValues(typeof(FhirArtifactClassEnum)))
        {
            if (TryGetArtifact(token, out object artifact, out FhirArtifactClassEnum resolvedAc, out string resolvedPackage, false, ac))
            {
                string key = resolvedPackage + ":" + ac.ToString() + ":" + token;

                if (!recs.ContainsKey(key))
                {
                    recs.Add(key, new ResolvedArtifactRecord(
                        resolvedAc,
                        resolvedPackage,
                        artifact));
                }
            }
        }

        return recs.Values;
    }

    /// <summary>Attempts to get artifact.</summary>
    /// <param name="token">             The ID or URL of the artifact.</param>
    /// <param name="artifact">          [out] The artifact.</param>
    /// <param name="artifactClass">     [out] The artifact class enum.</param>
    /// <param name="resolvedPackage">   [out] The resolved package.</param>
    /// <param name="resolveParentLinks">(Optional) True to resolve parent links.</param>
    /// <param name="knownArtifactClass"> (Optional) Class hint to resolve artifacts</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool TryGetArtifact(
        string token,
        out object artifact,
        out FhirArtifactClassEnum artifactClass,
        out string resolvedPackage,
        bool resolveParentLinks = true,
        FhirArtifactClassEnum knownArtifactClass = FhirArtifactClassEnum.Unknown)
    {
        if (knownArtifactClass != FhirArtifactClassEnum.Unknown)
        {
            artifactClass = knownArtifactClass;
        }
        else
        {
            artifactClass = GetArtifactClass(token);
        }

        switch (artifactClass)
        {
            case FhirArtifactClassEnum.PrimitiveType:
                {
                    artifact = ResolveInDict(token, _primitiveTypesByName);
                    resolvedPackage = PackageDetails.Name + "#" + PackageDetails.Version;
                    return artifact != null;
                }

            case FhirArtifactClassEnum.ComplexType:
                {
                    artifact = ResolveInDict(token, _complexTypesByName);
                    if (artifact == null)
                    {
                        resolvedPackage = string.Empty;
                        return false;
                    }

                    FhirComplex current = (FhirComplex)artifact;

                    if (resolveParentLinks &&
                        (!string.IsNullOrEmpty(current.BaseTypeName)) &&
                        (!current.BaseTypeName.Equals(current.Name, StringComparison.Ordinal)) &&
                        (current.Parent == null) &&
                        TryGetArtifact(
                            current.BaseTypeName,
                            out object parent,
                            out FhirArtifactClassEnum parentClass,
                            out string parentResolvedDirective,
                            true))
                    {
                        ((FhirComplex)artifact).Parent = (FhirComplex)parent;
                        ((FhirComplex)artifact).ParentArtifactClass = parentClass;
                        ((FhirComplex)artifact).ResolvedParentDirective = parentResolvedDirective;
                    }

                    resolvedPackage = PackageDetails.Name + "#" + PackageDetails.Version;
                    return true;
                }

            case FhirArtifactClassEnum.Resource:
                {
                    artifact = ResolveInDict(token, _resourcesByName);

                    if (artifact == null)
                    {
                        resolvedPackage = string.Empty;
                        return false;
                    }

                    FhirComplex current = (FhirComplex)artifact;

                    if (resolveParentLinks &&
                        (!string.IsNullOrEmpty(current.BaseTypeName)) &&
                        (!current.BaseTypeName.Equals(current.Name, StringComparison.Ordinal)) &&
                        (current.Parent == null) &&
                        TryGetArtifact(
                            current.BaseTypeName,
                            out object parent,
                            out FhirArtifactClassEnum parentClass,
                            out string parentResolvedDirective,
                            true))
                    {
                        ((FhirComplex)artifact).Parent = (FhirComplex)parent;
                        ((FhirComplex)artifact).ParentArtifactClass = parentClass;
                        ((FhirComplex)artifact).ResolvedParentDirective = parentResolvedDirective;
                    }

                    resolvedPackage = PackageDetails.Name + "#" + PackageDetails.Version;
                    return true;
                }

            case FhirArtifactClassEnum.LogicalModel:
                {
                    artifact = ResolveInDict(token, _logicalModelsByName);

                    if (artifact == null)
                    {
                        resolvedPackage = string.Empty;
                        return false;
                    }

                    FhirComplex current = (FhirComplex)artifact;

                    if (resolveParentLinks &&
                        (!string.IsNullOrEmpty(current.BaseTypeName)) &&
                        (!current.BaseTypeName.Equals(current.Name, StringComparison.Ordinal)) &&
                        (current.Parent == null) &&
                        TryGetArtifact(
                            current.BaseTypeName,
                            out object parent,
                            out FhirArtifactClassEnum parentClass,
                            out string parentResolvedDirective,
                            true))
                    {
                        ((FhirComplex)artifact).Parent = (FhirComplex)parent;
                        ((FhirComplex)artifact).ParentArtifactClass = parentClass;
                        ((FhirComplex)artifact).ResolvedParentDirective = parentResolvedDirective;
                    }

                    resolvedPackage = PackageDetails.Name + "#" + PackageDetails.Version;
                    return true;
                }

            case FhirArtifactClassEnum.Extension:
                {
                    artifact = ResolveInDict(token, _extensionsByUrl);

                    if (artifact == null)
                    {
                        resolvedPackage = string.Empty;
                        return false;
                    }

                    resolvedPackage = PackageDetails.Name + "#" + PackageDetails.Version;
                    return true;
                }

            case FhirArtifactClassEnum.Operation:
                {
                    artifact = ResolveInDict(token, _operationsByUrl);

                    if (artifact == null)
                    {
                        resolvedPackage = string.Empty;
                        return false;
                    }

                    resolvedPackage = PackageDetails.Name + "#" + PackageDetails.Version;
                    return true;
                }

            case FhirArtifactClassEnum.SearchParameter:
                {
                    artifact = ResolveInDict(token, _searchParamsByUrl);

                    if (artifact == null)
                    {
                        resolvedPackage = string.Empty;
                        return false;
                    }

                    resolvedPackage = PackageDetails.Name + "#" + PackageDetails.Version;
                    return true;
                }

            case FhirArtifactClassEnum.CodeSystem:
                {
                    artifact = ResolveInDict(token, _codeSystemsByUrl);

                    if (artifact == null)
                    {
                        resolvedPackage = string.Empty;
                        return false;
                    }

                    resolvedPackage = PackageDetails.Name + "#" + PackageDetails.Version;
                    return true;
                }

            case FhirArtifactClassEnum.ValueSet:
                {
                    FhirValueSetCollection collection = (FhirValueSetCollection)ResolveInDict(token, _valueSetsByUrl);
                    if ((collection == null) ||
                        (collection.ValueSetsByVersion == null) ||
                        (!collection.ValueSetsByVersion.Any()))
                    {
                        artifact = null;
                        resolvedPackage = string.Empty;
                        return false;
                    }

                    artifact = collection.ValueSetsByVersion.First().Value;

                    resolvedPackage = PackageDetails.Name + "#" + PackageDetails.Version;
                    return true;
                }

            case FhirArtifactClassEnum.Profile:
                {
                    artifact = ResolveInDict(token, _profilesByUrl);
                    if (artifact == null)
                    {
                        resolvedPackage = string.Empty;
                        return false;
                    }

                    FhirComplex current = (FhirComplex)artifact;
                    string baseKey = string.IsNullOrEmpty(current.BaseTypeCanonical)
                        ? current.BaseTypeName
                        : current.BaseTypeCanonical;

                    if (resolveParentLinks &&
                        (!string.IsNullOrEmpty(current.BaseTypeName)) &&
                        (!current.BaseTypeName.Equals(current.Name, StringComparison.Ordinal)) &&
                        (current.Parent == null) &&
                        TryGetArtifact(
                            baseKey,
                            out object parent,
                            out FhirArtifactClassEnum parentClass,
                            out string parentResolvedDirective,
                            true))
                    {
                        ((FhirComplex)artifact).Parent = (FhirComplex)parent;
                        ((FhirComplex)artifact).ParentArtifactClass = parentClass;
                        ((FhirComplex)artifact).ResolvedParentDirective = parentResolvedDirective;
                    }

                    resolvedPackage = PackageDetails.Name + "#" + PackageDetails.Version;
                    return true;
                }

            case FhirArtifactClassEnum.Unknown:
                {
                    if ((PackageDetails == null) ||
                        (PackageDetails.Dependencies == null) ||
                        (!PackageDetails.Dependencies.Any()))
                    {
                        artifact = null;
                        resolvedPackage = string.Empty;
                        return false;
                    }

                    // check dependencies
                    foreach ((string packageName, string version) in PackageDetails.Dependencies)
                    {
                        string directive = packageName + "#" + version;

                        if (FhirManager.Current.InfoByDirective.ContainsKey(directive))
                        {
                            return FhirManager.Current.InfoByDirective[directive].TryGetArtifact(
                                token,
                                out artifact,
                                out artifactClass,
                                out resolvedPackage,
                                resolveParentLinks);
                        }
                    }

                    artifact = null;
                    resolvedPackage = string.Empty;
                    return false;
                }

            case FhirArtifactClassEnum.ImplementationGuide:
                {
                    artifact = ResolveInDict(token, _igsByUri);

                    if (artifact == null)
                    {
                        resolvedPackage = string.Empty;
                        return false;
                    }

                    resolvedPackage = PackageDetails.Name + "#" + PackageDetails.Version;
                    return true;
                }

            case FhirArtifactClassEnum.CapabilityStatement:
                {
                    artifact = ResolveInDict(token, _capsByUrl);

                    if (artifact == null)
                    {
                        resolvedPackage = string.Empty;
                        return false;
                    }

                    resolvedPackage = PackageDetails.Name + "#" + PackageDetails.Version;
                    return true;
                }

            case FhirArtifactClassEnum.Compartment:
                {
                    artifact = ResolveInDict(token, _compartmentsByUrl);

                    if (artifact == null)
                    {
                        resolvedPackage = string.Empty;
                        return false;
                    }

                    resolvedPackage = PackageDetails.Name + "#" + PackageDetails.Version;
                    return true;
                }
                
            case FhirArtifactClassEnum.ConceptMap:
            case FhirArtifactClassEnum.NamingSystem:
            case FhirArtifactClassEnum.StructureMap:
            default:
                {
                    artifact = null;
                    resolvedPackage = string.Empty;
                    return false;
                }
        }
    }

    /// <summary>Attempts to get node information about the node described by the path.</summary>
    /// <param name="path">Full pathname of the file.</param>
    /// <param name="node">[out] The node information.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool TryGetNodeInfo(string path, out FhirNodeInfo node)
    {
        if (_nodeInfoByPath.ContainsKey(path))
        {
            node = _nodeInfoByPath[path];
            return true;
        }

        node = null;
        return false;
    }

    /// <summary>Adds a primitive.</summary>
    /// <param name="primitive">The primitive.</param>
    public void AddPrimitive(FhirPrimitive primitive)
    {
        _primitiveTypesByName.Add(primitive.Name, primitive);

        Uri url = primitive.URL ?? new Uri(CanonicalUrl, "StructureDefinition/" + primitive.Id);
        _artifactClassByUrl.Add(url.ToString(), FhirArtifactClassEnum.PrimitiveType);
        _artifactsByClass[FhirArtifactClassEnum.PrimitiveType].Add(new()
        {
            ArtifactClass = FhirArtifactClassEnum.PrimitiveType,
            Id = primitive.Id,
            Url = url,
            DefinitionResourceType = "StructureDefinition",
        });
    }

    /// <summary>Adds a complex type.</summary>
    /// <param name="complex">The complex.</param>
    public void AddComplexType(FhirComplex complex)
    {
        _complexTypesByName.Add(complex.Path, complex);

        Uri url = complex.URL ?? new Uri(CanonicalUrl, "StructureDefinition/" + complex.Id);
        _artifactClassByUrl.Add(url.ToString(), FhirArtifactClassEnum.ComplexType);
        _artifactsByClass[FhirArtifactClassEnum.ComplexType].Add(new()
        {
            ArtifactClass = FhirArtifactClassEnum.ComplexType,
            Id = complex.Id,
            Url = url,
            DefinitionResourceType = "StructureDefinition",
        });
    }

    /// <summary>Adds a resource.</summary>
    /// <param name="resource">The resource object.</param>
    public void AddResource(FhirComplex resource)
    {
        _resourcesByName.Add(resource.Path, resource);

        Uri url = resource.URL ?? new Uri(CanonicalUrl, "StructureDefinition/" + resource.Id);
        _artifactClassByUrl.Add(url.ToString(), FhirArtifactClassEnum.Resource);
        _artifactsByClass[FhirArtifactClassEnum.Resource].Add(new()
        {
            ArtifactClass = FhirArtifactClassEnum.Resource,
            Id = resource.Id,
            Url = url,
            DefinitionResourceType = "StructureDefinition",
        });
    }

    /// <summary>Adds a logical model.</summary>
    /// <param name="logicalModel">The logical model.</param>
    public void AddLogicalModel(FhirComplex logicalModel)
    {
        _logicalModelsByName.Add(logicalModel.Id, logicalModel);

        Uri url = logicalModel.URL ?? new Uri(CanonicalUrl, "StructureDefinition/" + logicalModel.Id);
        _artifactClassByUrl.Add(url.ToString(), FhirArtifactClassEnum.LogicalModel);
        _artifactsByClass[FhirArtifactClassEnum.LogicalModel].Add(new()
        {
            ArtifactClass = FhirArtifactClassEnum.LogicalModel,
            Id = logicalModel.Id,
            Url = url,
            DefinitionResourceType = "StructureDefinition",
        });
    }

    /// <summary>Adds a profile.</summary>
    /// <param name="complex">The complex.</param>
    public void AddProfile(FhirComplex complex)
    {
        _profilesByUrl.Add(complex.URL.ToString(), complex);

        Uri url = complex.URL ?? new Uri(CanonicalUrl, "StructureDefinition/" + complex.Id);
        _artifactClassByUrl.Add(url.ToString(), FhirArtifactClassEnum.Profile);
        _artifactsByClass[FhirArtifactClassEnum.Profile].Add(new()
        {
            ArtifactClass = FhirArtifactClassEnum.Profile,
            Id = complex.Id,
            Url = url,
            DefinitionResourceType = "StructureDefinition",
        });

        if (string.IsNullOrEmpty(complex.BaseTypeName))
        {
            return;
        }

        if (!_profilesByBaseType.ContainsKey(complex.BaseTypeName))
        {
            _profilesByBaseType.Add(complex.BaseTypeName, new Dictionary<string, FhirComplex>());
        }

        _profilesByBaseType[complex.BaseTypeName].Add(complex.Id, complex);
    }

    /// <summary>Adds a search parameter.</summary>
    /// <param name="searchParam">The search parameter.</param>
    public void AddSearchParameter(FhirSearchParam searchParam)
    {
        if (!_searchParamsByUrl.ContainsKey(searchParam.URL.ToString()))
        {
            _searchParamsByUrl.Add(searchParam.URL.ToString(), searchParam);
        }

        Uri url = searchParam.URL ?? new Uri(CanonicalUrl, "SearchParameter/" + searchParam.Id);

        // duplicate URLs are present in STU3
        if (!_artifactClassByUrl.ContainsKey(url.ToString()))
        {
            _artifactClassByUrl.Add(url.ToString(), FhirArtifactClassEnum.SearchParameter);
            _artifactsByClass[FhirArtifactClassEnum.SearchParameter].Add(new()
            {
                ArtifactClass = FhirArtifactClassEnum.SearchParameter,
                Id = searchParam.Id,
                Url = url,
                DefinitionResourceType = "SearchParameter",
            });
        }

        // traverse resources in the search parameter
        if (searchParam.ResourceTypes != null)
        {
            foreach (string resourceName in searchParam.ResourceTypes)
            {
                // check for search parameters on 'Resource', means they are global
                if (resourceName.Equals("Resource", StringComparison.Ordinal))
                {
                    // add to global
                    if (!_globalSearchParameters.ContainsKey(searchParam.Code))
                    {
                        _globalSearchParameters.Add(searchParam.Code, searchParam);
                    }

                    continue;
                }

                // check for having this resource
                if (!_resourcesByName.ContainsKey(resourceName))
                {
                    continue;
                }

                _resourcesByName[resourceName].AddSearchParameter(searchParam);
            }
        }
    }

    /// <summary>Adds an operation.</summary>
    /// <param name="operation">The operation.</param>
    public void AddOperation(FhirOperation operation)
    {
        if (!_operationsByUrl.ContainsKey(operation.URL.ToString()))
        {
            _operationsByUrl.Add(operation.URL.ToString(), operation);
        }

        Uri url = operation.URL ?? new Uri(CanonicalUrl, "OperationDefinition/" + operation.Id);
        string urlS = url.ToString();

        if (!_artifactClassByUrl.ContainsKey(urlS))
        {
            _artifactClassByUrl.Add(urlS, FhirArtifactClassEnum.Operation);
            _artifactsByClass[FhirArtifactClassEnum.Operation].Add(new()
            {
                ArtifactClass = FhirArtifactClassEnum.Operation,
                Id = operation.Id,
                Url = url,
                DefinitionResourceType = "OperationDefinition",
            });
        }

        // check for system level operation
        if (operation.DefinedOnSystem)
        {
            if (!_systemOperations.ContainsKey(operation.Code))
            {
                _systemOperations.Add(operation.Code, operation);
            }
        }

        // look for resources this should be defined on
        if (operation.ResourceTypes != null)
        {
            foreach (string resourceName in operation.ResourceTypes)
            {
                if (!_resourcesByName.ContainsKey(resourceName))
                {
                    continue;
                }

                _resourcesByName[resourceName].AddOperation(operation);
            }
        }
    }

    /// <summary>Adds a code system.</summary>
    /// <param name="codeSystem">The code system.</param>
    public void AddCodeSystem(FhirCodeSystem codeSystem)
    {
        if ((codeSystem.URL == null) ||
            _codeSystemsByUrl.ContainsKey(codeSystem.URL))
        {
            return;
        }

        _codeSystemsByUrl.Add(codeSystem.URL, codeSystem);

        _artifactClassByUrl.Add(codeSystem.URL, FhirArtifactClassEnum.CodeSystem);
        _artifactsByClass[FhirArtifactClassEnum.CodeSystem].Add(new()
        {
            ArtifactClass = FhirArtifactClassEnum.CodeSystem,
            Id = codeSystem.Id,
            Url = new Uri(codeSystem.URL),
            DefinitionResourceType = "CodeSystem",
        });
    }

    /// <summary>Adds an implementation guide.</summary>
    /// <param name="ig">The ig.</param>
    public void AddImplementationGuide(FhirImplementationGuide ig)
    {
        if ((ig == null) ||
            (ig.URL == null) ||
            _igsByUri.ContainsKey(ig.URL.ToString()))
        {
            return;
        }

        _igsByUri.Add(ig.URL.ToString(), ig);

        _artifactClassByUrl.Add(ig.URL.ToString(), FhirArtifactClassEnum.ImplementationGuide);
        _artifactsByClass[FhirArtifactClassEnum.ImplementationGuide].Add(new()
        {
            ArtifactClass = FhirArtifactClassEnum.ImplementationGuide,
            Id = ig.Id,
            Url = ig.URL,
            DefinitionResourceType = "ImplementationGuide",
        });
    }

    /// <summary>Adds a capability statement.</summary>
    /// <param name="cap">The capability.</param>
    public void AddCapabilityStatement(FhirCapabiltyStatement cap)
    {
        if ((cap == null) ||
            string.IsNullOrEmpty(cap.Url) ||
            _capsByUrl.ContainsKey(cap.Url))
        {
            return;
        }

        _capsByUrl.Add(cap.Url, cap);

        _artifactClassByUrl.Add(cap.Url, FhirArtifactClassEnum.CapabilityStatement);
        _artifactsByClass[FhirArtifactClassEnum.CapabilityStatement].Add(new()
        {
            ArtifactClass = FhirArtifactClassEnum.CapabilityStatement,
            Id = cap.Id,
            Url = new Uri(cap.Url),
            DefinitionResourceType = "CapabilityStatement",
        });
    }

    public void AddCompartment(FhirCompartment compartment)
    {
        if ((compartment == null) ||
            string.IsNullOrEmpty(compartment.Url) ||
            _compartmentsByUrl.ContainsKey(compartment.Url))
        {
            return;
        }

        _compartmentsByUrl.Add(compartment.Url, compartment);

        _artifactClassByUrl.Add(compartment.Url, FhirArtifactClassEnum.Compartment);
        _artifactsByClass[FhirArtifactClassEnum.Compartment].Add(new()
        {
            ArtifactClass = FhirArtifactClassEnum.Compartment,
            Id = compartment.Id,
            Url = new Uri(compartment.Url),
            DefinitionResourceType = "CompartmentDefinition",
        });
    }

    /// <summary>Adds a value set.</summary>
    /// <param name="valueSet">Set the value belongs to.</param>
    public void AddValueSet(FhirValueSet valueSet)
    {
        if (valueSet.URL == null)
        {
            return;
        }

        if (!_valueSetsByUrl.ContainsKey(valueSet.URL))
        {
            _valueSetsByUrl.Add(valueSet.URL, new FhirValueSetCollection(valueSet.URL));
        }

        _valueSetsByUrl[valueSet.URL].AddValueSet(valueSet);

        _artifactClassByUrl.Add(valueSet.URL, FhirArtifactClassEnum.ValueSet);
        _artifactsByClass[FhirArtifactClassEnum.ValueSet].Add(new()
        {
            ArtifactClass = FhirArtifactClassEnum.ValueSet,
            Id = valueSet.Id,
            Url = new Uri(valueSet.URL),
            DefinitionResourceType = "CodeSystem",
        });
    }

    /// <summary>Query if 'urlOrKey' has value set.</summary>
    /// <param name="urlOrKey">The URL or key.</param>
    /// <returns>True if value set, false if not.</returns>
    public bool HasValueSet(string urlOrKey)
    {
        string url;
        string version = string.Empty;

        if (urlOrKey.Contains('|'))
        {
            string[] components = urlOrKey.Split('|');
            url = components[0];
            version = components[1];
        }
        else
        {
            url = urlOrKey;
        }

        if (!_valueSetsByUrl.ContainsKey(url))
        {
            return false;
        }

        if (_valueSetsByUrl[url].HasVersion(version))
        {
            return true;
        }

        return false;
    }

    /// <summary>Attempts to get value set a FhirValueSet from the given string.</summary>
    /// <param name="urlOrKey">The URL or key.</param>
    /// <param name="vs">      [out] The vs.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool TryGetValueSet(string urlOrKey, out FhirValueSet vs)
    {
        string url;
        string version = string.Empty;

        if (urlOrKey.Contains('|'))
        {
            string[] components = urlOrKey.Split('|');
            url = components[0];
            version = components[1];
        }
        else
        {
            url = urlOrKey;
        }

        if (!_valueSetsByUrl.ContainsKey(url))
        {
            vs = null;
            return false;
        }

        if (_valueSetsByUrl[url].HasVersion(version))
        {
            return _valueSetsByUrl[url].TryGetValueSet(version, out vs);
        }

        vs = null;
        return false;
    }

    /// <summary>Adds an extension.</summary>
    /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
    /// <param name="extension">The extension.</param>
    public void AddExtension(FhirComplex extension)
    {
        string url = extension.URL.ToString();

        // add this extension to our primary dictionary
        if (!_extensionsByUrl.ContainsKey(url))
        {
            _extensionsByUrl.Add(url, extension);
        }

        // check for needing to use the base type name for context
        if (!extension.ContextElements.Any())
        {
            if (string.IsNullOrEmpty(extension.BaseTypeName))
            {
                throw new ArgumentNullException(
                    nameof(extension),
                    $"ContextElements ({extension.ContextElements}) or BaseTypeName ({extension.BaseTypeName}) is required");
            }

            // add the base type name as a context element
            extension.AddContextElement(extension.BaseTypeName);
        }

        foreach (string elementPath in extension.ContextElements)
        {
            if (string.IsNullOrEmpty(elementPath))
            {
                continue;
            }

            // check for this path being new
            if (!_extensionsByPath.ContainsKey(elementPath))
            {
                _extensionsByPath.Add(elementPath, new Dictionary<string, FhirComplex>());
            }

            // add this extension (if necessary)
            if (!_extensionsByPath[elementPath].ContainsKey(url))
            {
                // add as reference to main dictionary
                _extensionsByPath[elementPath].Add(url, _extensionsByUrl[url]);
            }
        }

        _artifactClassByUrl.Add(extension.URL.OriginalString, FhirArtifactClassEnum.Extension);
        _artifactsByClass[FhirArtifactClassEnum.Extension].Add(new()
        {
            ArtifactClass = FhirArtifactClassEnum.Extension,
            Id = extension.Id,
            Url = extension.URL,
            DefinitionResourceType = "Extension",
        });
    }

    /// <summary>Query if 'key' is a known complex data type.</summary>
    /// <param name="key">The key.</param>
    /// <returns>True if 'key' exists, false if not.</returns>
    public bool HasComplex(string key)
    {
        if (_primitiveTypesByName.ContainsKey(key))
        {
            return true;
        }

        if (_complexTypesByName.ContainsKey(key))
        {
            return true;
        }

        return false;
    }

    /// <summary>Attempts to get an explicit name string from the given string.</summary>
    /// <param name="path">        Full pathname of the file.</param>
    /// <param name="explicitName">[out] Name of the explicit.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool TryGetExplicitName(string path, out string explicitName)
    {
        if (string.IsNullOrEmpty(path))
        {
            explicitName = string.Empty;
            return false;
        }

        int index = path.IndexOf('.', StringComparison.Ordinal);

        string currentPath;

        if (index != -1)
        {
            currentPath = path.Substring(0, index);
        }
        else
        {
            currentPath = path;
            index = 0;
        }

        if (_complexTypesByName.ContainsKey(currentPath))
        {
            return _complexTypesByName[currentPath].TryGetExplicitName(path, out explicitName, index);
        }

        if (_resourcesByName.ContainsKey(currentPath))
        {
            return _resourcesByName[currentPath].TryGetExplicitName(path, out explicitName, index);
        }

        if (_logicalModelsByName.ContainsKey(currentPath))
        {
            return _logicalModelsByName[currentPath].TryGetExplicitName(path, out explicitName, index);
        }

        explicitName = string.Empty;
        return false;
    }

    /// <summary>Gets inheritance names hash.</summary>
    /// <param name="key">The key.</param>
    /// <returns>The inheritance names hash.</returns>
    public HashSet<string> GetInheritanceNamesHash(string key)
    {
        HashSet<string> hs = new();

        if (_complexTypesByName.ContainsKey(key))
        {
            FhirComplex c = _complexTypesByName[key];

            hs.Add(c.Name);

            if ((!string.IsNullOrEmpty(c.BaseTypeName)) &&
                (!c.Name.Equals(c.BaseTypeName, StringComparison.OrdinalIgnoreCase)))
            {
                hs.UnionWith(GetInheritanceNamesHash(c.BaseTypeName));
            }
        }

        if (_resourcesByName.ContainsKey(key))
        {
            FhirComplex c = _resourcesByName[key];

            hs.Add(c.Name);

            if ((!string.IsNullOrEmpty(c.BaseTypeName)) &&
                (!c.Name.Equals(c.BaseTypeName, StringComparison.OrdinalIgnoreCase)))
            {
                hs.UnionWith(GetInheritanceNamesHash(c.BaseTypeName));
            }
        }

        if (_profilesByUrl.ContainsKey(key))
        {
            FhirComplex c = _profilesByUrl[key];

            hs.Add(c.Name);

            if ((!string.IsNullOrEmpty(c.BaseTypeName)) &&
                (!c.Name.Equals(c.BaseTypeName, StringComparison.OrdinalIgnoreCase)))
            {
                hs.UnionWith(GetInheritanceNamesHash(c.BaseTypeName));
            }
        }

        if (_logicalModelsByName.ContainsKey(key))
        {
            FhirComplex c = _logicalModelsByName[key];

            hs.Add(c.Name);

            if ((!string.IsNullOrEmpty(c.BaseTypeName)) &&
                (!c.Name.Equals(c.BaseTypeName, StringComparison.OrdinalIgnoreCase)))
            {
                hs.UnionWith(GetInheritanceNamesHash(c.BaseTypeName));
            }
        }

        return hs;
    }

    /// <summary>Determine if we should process resource.</summary>
    /// <param name="resourceName">Name of the resource.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool ShouldProcessResource(string resourceName)
    {
        switch (PackageType)
        {
            case FhirPackageCommon.FhirPackageTypeEnum.Core:
                return FhirPackageCommon.ShouldProcessResource(FhirSequence, resourceName);

            case FhirPackageCommon.FhirPackageTypeEnum.Unknown:
            case FhirPackageCommon.FhirPackageTypeEnum.IG:
            default:
                return FhirPackageCommon.ShouldProcessResource(resourceName);
        }
    }

    /// <summary>Determine if we should ignore resource.</summary>
    /// <param name="resourceName">Name of the resource.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool ShouldIgnoreResource(string resourceName)
    {
        switch (PackageType)
        {
            case FhirPackageCommon.FhirPackageTypeEnum.Core:
                return FhirPackageCommon.ShouldIgnoreResource(FhirSequence, resourceName);

            case FhirPackageCommon.FhirPackageTypeEnum.Unknown:
            case FhirPackageCommon.FhirPackageTypeEnum.IG:
            default:
                return false;
        }
    }

    /// <summary>Determine if we should skip file.</summary>
    /// <param name="filename">Filename of the file.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool ShouldSkipFile(string filename)
    {
        if (_npmFilesToIgnore.Contains(filename))
        {
            return true;
        }

        switch (PackageType)
        {
            case FhirPackageCommon.FhirPackageTypeEnum.Core:
                return FhirPackageCommon.ShouldSkipFile(FhirSequence, filename);

            case FhirPackageCommon.FhirPackageTypeEnum.Unknown:
            case FhirPackageCommon.FhirPackageTypeEnum.IG:
            default:
                return false;
        }
    }

    /// <summary>Parses resource an object from the given string.</summary>
    /// <param name="json">The JSON.</param>
    /// <returns>A typed Resource object.</returns>
    public bool TryParseResource(string json, out object resource, out string resourceType)
    {
        return _fhirConverter.TryParseResource(json, out resource, out resourceType);
    }

    /// <summary>Attempts to get first from bundle.</summary>
    /// <param name="json">        The JSON.</param>
    /// <param name="resource">    [out] The resource object.</param>
    /// <param name="resourceType">[out] Type of the resource.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool TryGetFirstFromBundle(string json, out object resource, out string resourceType)
    {
        return _fhirConverter.TryGetFirstFromBundle(json, out resource, out resourceType);
    }

    /// <summary>Attempts to process resource.</summary>
    /// <param name="resource">[out] The resource object.</param>
    public void ProcessResource(object resource)
    {
        // process this per the correct FHIR version
        _fhirConverter.ProcessResource(resource, this);
    }

    /// <summary>Attempts to process resource.</summary>
    /// <param name="resource">[out] The resource object.</param>
    /// <param name="resourceCanonical"> Canonical of the processed resource, or string.Empty if not processed.</param>
    /// <param name="artifactClass">  Class of the resource parsed</param>
    public void ProcessResource(object resource, out string resourceCanonical, out FhirArtifactClassEnum artifactClass)
    {
        // process this per the correct FHIR version
        _fhirConverter.ProcessResource(resource, this, out resourceCanonical, out artifactClass);
    }

    /// <summary>
    /// Replace a value in a parsed but not-yet processed resource
    /// </summary>
    /// <param name="resource"></param>
    /// <param name="path"></param>
    /// <param name="value"></param>
    public void ReplaceParsedValue(
        object resource,
        string[] path,
        object value)
    {
        _fhirConverter.ReplaceValue(resource, path, value);
    }

    /// <summary>Determines if we can converter has issues.</summary>
    /// <param name="errorCount">  [out] Number of errors.</param>
    /// <param name="warningCount">[out] Number of warnings.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool ConverterHasIssues(out int errorCount, out int warningCount)
    {
        return _fhirConverter.HasIssues(out errorCount, out warningCount);
    }

    /// <summary>Displays the converter issues.</summary>
    public void DisplayConverterIssues()
    {
        _fhirConverter.DisplayIssues();
    }

    /// <summary>Adds a versioned parameter.</summary>
    /// <param name="searchMagicType">Type of the search magic.</param>
    /// <param name="name">           The name.</param>
    /// <param name="parameterType">  Type of the parameter.</param>
    public void AddVersionedParam(
        FhirSearchParam.ParameterGrouping searchMagicType,
        string name,
        string parameterType)
    {
        switch (searchMagicType)
        {
            case FhirSearchParam.ParameterGrouping.Global:
                AddVersionedParam(_globalSearchParameters, name, parameterType);
                break;

            case FhirSearchParam.ParameterGrouping.Result:
                AddVersionedParam(_searchResultParameters, name, parameterType);
                break;

            case FhirSearchParam.ParameterGrouping.Interaction:
                AddVersionedParam(_allInteractionParameters, name, parameterType);
                break;
        }
    }

    /// <summary>Adds a versioned parameter.</summary>
    /// <param name="dict">The dictionary.</param>
    /// <param name="name">The name.</param>
    /// <param name="type">The type.</param>
    private void AddVersionedParam(
        Dictionary<string, FhirSearchParam> dict,
        string name,
        string type)
    {
        if (dict.ContainsKey(name))
        {
            return;
        }

        string url = $"http://hl7.org/fhir/{ReleaseName}/search.html#{name.Substring(1)}";

        dict.Add(
            name,
            new FhirSearchParam(
                name,
                new Uri(url),
                VersionString,
                name,
                $"Filter search by {name}",
                string.Empty,
                name,
                null,
                null,
                type,
                string.Empty,
                string.Empty,
                null,
                false,
                string.Empty,
                string.Empty,
                string.Empty));

        if (!_searchParamsByUrl.ContainsKey(url))
        {
            _searchParamsByUrl.Add(url, dict[name]);
        }
    }

    /// <summary>Adds a complex to export set.</summary>
    /// <param name="complex">   The complex.</param>
    /// <param name="set">       The set.</param>
    /// <param name="isResource">True if is resource, false if not.</param>
    /// <param name="source">    Source for the.</param>
    internal void AddComplexToExportSet(
        FhirComplex complex,
        HashSet<string> set,
        bool isResource,
        FhirVersionInfo source)
    {
        // add this item
        set.Add(complex.Name);

        // check for a parent type
        if (!string.IsNullOrEmpty(complex.BaseTypeName))
        {
            // add the parent
            AddToExportSet(complex.BaseTypeName, set, source);

            if (isResource)
            {
                // Resources cannot inherit patterns, but they are listed that way today
                // see https://chat.fhir.org/#narrow/stream/179177-conformance/topic/Inheritance.20and.20Cardinality.20Changes
                switch (complex.BaseTypeName)
                {
                    case "CanonicalResource":
                    case "MetadataResource":
                        AddToExportSet("DomainResource", set, source);
                        break;
                }
            }
        }

        // check for element types
        if (complex.Elements.Any())
        {
            foreach (KeyValuePair<string, FhirElement> kvp in complex.Elements)
            {
                if (kvp.Key.Equals("Extension.value[x]", StringComparison.Ordinal))
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(kvp.Value.BaseTypeName))
                {
                    // add the element type
                    AddToExportSet(kvp.Value.BaseTypeName, set, source);
                }

                if (kvp.Value.ElementTypes != null)
                {
                    foreach (FhirElementType elementType in kvp.Value.ElementTypes.Values)
                    {
                        // add the element type
                        AddToExportSet(elementType.Name, set, source);

                        // TODO: figure out if this is actually desired
                        //if (elementType.Profiles != null)
                        //{
                        //    if (elementType.Profiles.Count > 1)
                        //    {
                        //        Console.Write("");
                        //    }

                        //    foreach (FhirElementProfile profile in elementType.Profiles.Values)
                        //    {
                        //        AddToExportSet(profile.Name, set, source);
                        //    }
                        //}
                    }
                }
            }
        }

        if (complex.Components.Any())
        {
            if (source._complexTypesByName.ContainsKey("BackboneElement") &&
                (!set.Contains("BackboneElement")))
            {
                set.Add("BackboneElement");
            }

            foreach (FhirComplex component in complex.Components.Values)
            {
                AddComplexToExportSet(component, set, false, source);
            }
        }
    }

    /// <summary>Recursively adds a resource or type to the export set.</summary>
    /// <param name="name">  The name.</param>
    /// <param name="set">   The set.</param>
    /// <param name="source">(Optional) Source for the.</param>
    internal void AddToExportSet(string name, HashSet<string> set, FhirVersionInfo source)
    {
        // if we've already added this, we're done
        if (set.Contains(name))
        {
            return;
        }

        // check for primitive
        if (source._primitiveTypesByName.ContainsKey(name))
        {
            // add this item
            set.Add(name);

            // no recursion on primitive types
            return;
        }

        // check for this being a type
        if (source._complexTypesByName.ContainsKey(name))
        {
            AddComplexToExportSet(source._complexTypesByName[name], set, false, source);
        }

        // check for this being a resource
        if (source._resourcesByName.ContainsKey(name))
        {
            AddComplexToExportSet(source._resourcesByName[name], set, true, source);
        }

        if (source._logicalModelsByName.ContainsKey(name))
        {
            AddComplexToExportSet(source._logicalModelsByName[name], set, true, source);
        }

        if (source._profilesByUrl.ContainsKey(name))
        {
            AddComplexToExportSet(source._profilesByUrl[name], set, true, source);
        }
    }

    /// <summary>Loads from FHIR Core package.</summary>
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    /// <param name="directive">The directive.</param>
    private void LoadFromCorePackage(string directive)
    {
        NpmPackageDetails details = PackageDetails;

        if (!FhirPackageCommon.TryGetReleaseByPackage(details.Name, out FhirPackageCommon.FhirSequenceEnum sequence))
        {
            throw new Exception($"Could not determine the FHIR sequence for package: {details.Name}#{details.Version}");
        }

        FhirSequence = sequence;
        _fhirConverter = ConverterHelper.ConverterForVersion(sequence);

#pragma warning disable CS0618 // Type or member is obsolete
        MajorVersion = FhirPackageCommon.MajorIntForVersion(sequence);
#pragma warning restore CS0618 // Type or member is obsolete
        ReleaseName = sequence.ToString();

        BallotPrefix = string.Empty;
        if ((!string.IsNullOrEmpty(details.Canonical)) && (details.URL != null))
        {
            string url = details.URL.AbsoluteUri;
            if (url.Length > details.Canonical.Length)
            {
                string ballot = url.Substring(details.Canonical.Length);
                if (ballot.StartsWith('/'))
                {
                    ballot = ballot.Substring(1);
                }

                BallotPrefix = ballot;
            }
        }

        PackageName = details.Name;
        PackageType = FhirPackageCommon.FhirPackageTypeEnum.Core;

        ExamplesPackageName = details.Name.Replace(".core", ".examples", StringComparison.Ordinal);
        ExpansionsPackageName = details.Name.Replace(".core", ".expansions", StringComparison.Ordinal);

        if ((details.FhirVersionList != null) && details.FhirVersionList.Any())
        {
            VersionString = details.FhirVersionList.First();
        }
        else if ((details.FhirVersions != null) && details.FhirVersions.Any())
        {
            VersionString = details.FhirVersions.First();
        }
        else if (!string.IsNullOrEmpty(details.OriginalVersion))
        {
            VersionString = details.OriginalVersion;
        }
        else if (!string.IsNullOrEmpty(details.Version))
        {
            VersionString = details.Version;
        }

        // FIX: There is a published version of 5.0.0-snapshot1 that has FhirVersions set to 3.0.1, let's correct that here
        if (details.Version == "5.0.0-snapshot1" && VersionString == "3.0.1")
            VersionString = "5.0.0-snapshot1";

        // TODO(ginoc): verify from here down - should not make any difference because of cache manager and can probably be removed

        if (directive.EndsWith("current", StringComparison.OrdinalIgnoreCase) ||
            directive.EndsWith("dev", StringComparison.OrdinalIgnoreCase))
        {
            IsDevBuild = true;
            DevBranch = string.Empty;
            BuildId = details.BuildDate;
        }
        else
        {
            IsDevBuild = false;
            DevBranch = string.Empty;
            BuildId = string.Empty;
        }

        IsLocalBuild = false;
        LocalDirectory = string.Empty;
        IsOnDisk = true;
        LastDownloaded = null;
    }

    /// <summary>Loads from FHIR IG package.</summary>
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    /// <param name="directive">The directive.</param>
    private void LoadFromGuidePackage(string directive)
    {
        NpmPackageDetails details = PackageDetails;

        FhirPackageCommon.FhirSequenceEnum sequence = FhirPackageCommon.FhirSequenceEnum.R4;
        bool foundSequence = false;

        if ((details.FhirVersions != null) && details.FhirVersions.Any())
        {
            sequence = FhirPackageCommon.MajorReleaseForVersion(details.FhirVersions.First());
            foundSequence = true;
        }
        else if ((details.Dependencies != null) && details.Dependencies.Any())
        {
            foreach (string key in details.Dependencies.Keys)
            {
                if (FhirPackageCommon.PackageIsFhirCore(key))
                {
                    sequence = FhirPackageCommon.MajorReleaseForVersion(key);
                    foundSequence = true;
                    break;
                }
            }
        }

        if (!foundSequence)
        {
            Console.WriteLine($"Warning could not determine FHIR version of {directive}, assuming R4 for parsing...");
        }

        FhirSequence = sequence;
        _fhirConverter = ConverterHelper.ConverterForVersion(sequence);

#pragma warning disable CS0618 // Type or member is obsolete
        MajorVersion = FhirPackageCommon.MajorIntForVersion(sequence);
#pragma warning restore CS0618 // Type or member is obsolete
        ReleaseName = sequence.ToString();

        PackageName = details.Name;
        PackageType = FhirPackageCommon.FhirPackageTypeEnum.IG;
        VersionString = details.Version;

        // TODO(ginoc): verify from here down - should not make any difference because of cache manager and can probably be removed

        if (directive.EndsWith("current", StringComparison.OrdinalIgnoreCase) ||
            directive.EndsWith("dev", StringComparison.OrdinalIgnoreCase))
        {
            IsDevBuild = true;
            DevBranch = string.Empty;
            BuildId = details.BuildDate;
        }
        else
        {
            IsDevBuild = false;
            DevBranch = string.Empty;
            BuildId = string.Empty;
        }

        IsLocalBuild = false;
        LocalDirectory = string.Empty;
        IsOnDisk = true;
        LastDownloaded = null;
    }
}
