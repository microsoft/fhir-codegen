// <copyright file="FhirVersionInfo.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.IO;
using Microsoft.Health.Fhir.SpecManager.Converters;

namespace Microsoft.Health.Fhir.SpecManager.Manager;

/// <summary>Information about a FHIR package.</summary>
public class FhirVersionInfo : IPackageImportable, IPackageExportable
{
    private static HashSet<string> _npmFilesToIgnore = new ()
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

        _excludedKeys = new();

        _artifactsByClass = new();
        foreach (FhirArtifactClassEnum artifactClass in (FhirArtifactClassEnum[])Enum.GetValues(typeof(FhirArtifactClassEnum)))
        {
            _artifactsByClass.Add(artifactClass, new());
        }
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
    public FhirVersionInfo(FhirVersionInfo source, PackageCopyOptions options)
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
        if ((options.ServerInfo != null) &&
            (exportSet.Count == 0))
        {
            foreach (FhirServerResourceInfo resource in options.ServerInfo.ResourceInteractions.Values)
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

        Dictionary<string, ValueSetReferenceInfo> valueSetReferences = new Dictionary<string, ValueSetReferenceInfo>();

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

                if ((options.ServerInfo == null) ||
                    (!options.ServerInfo.ResourceInteractions.ContainsKey(kvp.Key)))
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
                        options.ServerInfo.ResourceInteractions[kvp.Key].SearchParameters,
                        options.ServerInfo.ServerSearchParameters,
                        options.ServerInfo.ResourceInteractions[kvp.Key].Operations,
                        options.ServerInfo.ServerOperations,
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

                if ((options.ServerInfo == null) ||
                    (!options.ServerInfo.ResourceInteractions.ContainsKey(kvp.Key)))
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
                        options.ServerInfo.ResourceInteractions[kvp.Key].SearchParameters,
                        options.ServerInfo.ServerSearchParameters,
                        options.ServerInfo.ResourceInteractions[kvp.Key].Operations,
                        options.ServerInfo.ServerOperations,
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
        }

        if (options.CopyOperations)
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

        if (options.ServerInfo == null)
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

                if (options.ServerInfo.ServerOperations.ContainsKey(kvp.Value.Code))
                {
                    _systemOperations.Add(kvp.Key, (FhirOperation)kvp.Value.Clone());
                }
            }

            foreach (KeyValuePair<string, FhirServerOperation> kvp in options.ServerInfo.ServerOperations)
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
                        false));
            }
        }

        if (options.ServerInfo == null)
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

                if (options.ServerInfo.ServerSearchParameters.ContainsKey(kvp.Value.Code))
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

    /// <summary>Gets URL of the operations by.</summary>
    public Dictionary<string, FhirOperation> OperationsByUrl { get => _operationsByUrl; }

    /// <summary>Gets search parameters defined for all resources.</summary>
    public Dictionary<string, FhirSearchParam> AllResourceParameters { get => _globalSearchParameters; }

    /// <summary>Gets search parameters that control search results.</summary>
    public Dictionary<string, FhirSearchParam> SearchResultParameters { get => _searchResultParameters; }

    /// <summary>Gets search parameters defined for all interactions.</summary>
    public Dictionary<string, FhirSearchParam> AllInteractionParameters { get => _allInteractionParameters; }

    /// <summary>Gets URL of the search parameters by.</summary>
    public Dictionary<string, FhirSearchParam> SearchParametersByUrl { get => _searchParamsByUrl; }

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

                    values = (IEnumerable<T>)_profilesByUrl.Values.AsEnumerable<FhirComplex>();
                    return true;
                }

            case FhirArtifactClassEnum.Unknown:
            case FhirArtifactClassEnum.CapabilityStatement:
            case FhirArtifactClassEnum.Compartment:
            case FhirArtifactClassEnum.ConceptMap:
            case FhirArtifactClassEnum.NamingSystem:
            case FhirArtifactClassEnum.StructureMap:
            case FhirArtifactClassEnum.ImplementationGuide:
            default:
                {
                    values = null;
                    return false;
                }
        }
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

            case FhirArtifactClassEnum.CapabilityStatement:
            case FhirArtifactClassEnum.Compartment:
            case FhirArtifactClassEnum.ConceptMap:
            case FhirArtifactClassEnum.NamingSystem:
            case FhirArtifactClassEnum.StructureMap:
            case FhirArtifactClassEnum.ImplementationGuide:
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
        _artifactClassByUrl.Add(url.ToString(), FhirArtifactClassEnum.Operation);
        _artifactsByClass[FhirArtifactClassEnum.Operation].Add(new()
        {
            ArtifactClass = FhirArtifactClassEnum.Operation,
            Id = operation.Id,
            Url = url,
            DefinitionResourceType = "OperationDefinition",
        });

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
        if ((extension.ContextElements == null) || (extension.ContextElements.Count == 0))
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

    /// <summary>Attempts to get explicit name a string from the given string.</summary>
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
                return true;
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
    public object ParseResource(string json)
    {
        return _fhirConverter.ParseResource(json);
    }

    /// <summary>Attempts to process resource.</summary>
    /// <param name="resource">[out] The resource object.</param>
    public void ProcessResource(object resource)
    {
        // process this per the correct FHIR version
        _fhirConverter.ProcessResource(resource, this);
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
        if (complex.Elements != null)
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

        if (complex.Components != null)
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
