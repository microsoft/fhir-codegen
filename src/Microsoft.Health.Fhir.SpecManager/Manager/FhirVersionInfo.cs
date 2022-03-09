// <copyright file="FhirVersionInfo.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.IO;
using Microsoft.Health.Fhir.SpecManager.Converters;
using Microsoft.Health.Fhir.SpecManager.Models;

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
    private Dictionary<string, FhirComplex> _extensionsByUrl;
    private Dictionary<string, Dictionary<string, FhirComplex>> _extensionsByPath;
    private Dictionary<string, FhirComplex> _profilesById;
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

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirVersionInfo"/> class.
    /// </summary>
    public FhirVersionInfo()
    {
        // create our info dictionaries
        _primitiveTypesByName = new();
        _complexTypesByName = new();
        _resourcesByName = new();
        _extensionsByUrl = new();
        _extensionsByPath = new();
        _profilesById = new();
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

        // figure out all the the dependencies we need to include based on requests
        if (options.ExportList != null)
        {
            foreach (string path in options.ExportList)
            {
                AddToExportSet(path, ref exportSet);
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
                AddToExportSet(resource.ResourceType, ref exportSet);
            }

            if (exportSet.Count > 0)
            {
                restrictResources = true;

                // make sure Bundle is included so we can search, etc.
                AddToExportSet("Bundle", ref exportSet);
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
                    continue;
                }

                // check for experimental - unless this is specifically included
                if ((!restrictOutput) &&
                    (!options.IncludeExperimental) &&
                    kvp.Value.IsExperimental)
                {
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
                    continue;
                }

                // check for experimental - unless this is specifically included
                if ((!restrictOutput) &&
                    (!options.IncludeExperimental) &&
                    kvp.Value.IsExperimental)
                {
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
                    continue;
                }

                if (restrictResources && (!exportSet.Contains(kvp.Key)))
                {
                    continue;
                }

                // check for experimental - unless this is specifically included
                if (((!restrictOutput) || (!restrictResources)) &&
                    (!options.IncludeExperimental) &&
                    kvp.Value.IsExperimental)
                {
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

        if (options.CopyProfiles)
        {
            foreach (FhirComplex profile in source._profilesById.Values)
            {
                if (_resourcesByName.ContainsKey(profile.BaseTypeName) ||
                    _complexTypesByName.ContainsKey(profile.BaseTypeName) ||
                    _primitiveTypesByName.ContainsKey(profile.BaseTypeName))
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
            foreach (FhirSearchParam sp in source._searchParamsByUrl.Values)
            {
                AddSearchParameter((FhirSearchParam)sp.Clone());
            }
        }

        if (options.CopyOperations)
        {
            foreach (FhirOperation op in source._operationsByUrl.Values)
            {
                AddOperation((FhirOperation)op.Clone());
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
                _systemOperations.Add(kvp.Key, (FhirOperation)kvp.Value.Clone());
            }
        }
        else
        {
            foreach (KeyValuePair<string, FhirOperation> kvp in source._systemOperations)
            {
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
                        true,
                        false,
                        false,
                        kvp.Value.Name,
                        kvp.Value.Documentation,
                        null,
                        new List<FhirParameter>(),
                        false));
            }
        }

        if (options.ServerInfo == null)
        {
            foreach (KeyValuePair<string, FhirSearchParam> kvp in source._globalSearchParameters)
            {
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
                if (options.ServerInfo.ServerSearchParameters.ContainsKey(kvp.Value.Code))
                {
                    _globalSearchParameters.Add(kvp.Key, (FhirSearchParam)kvp.Value.Clone());
                }
            }
        }

        foreach (KeyValuePair<string, FhirSearchParam> kvp in source._searchResultParameters)
        {
            if ((!options.IncludeExperimental) && kvp.Value.IsExperimental)
            {
                continue;
            }

            _searchResultParameters.Add(kvp.Key, (FhirSearchParam)kvp.Value.Clone());
        }

        foreach (KeyValuePair<string, FhirSearchParam> kvp in source._allInteractionParameters)
        {
            if ((!options.IncludeExperimental) && kvp.Value.IsExperimental)
            {
                continue;
            }

            _allInteractionParameters.Add(kvp.Key, (FhirSearchParam)kvp.Value.Clone());
        }

        foreach (KeyValuePair<string, FhirValueSetCollection> collectionKvp in source._valueSetsByUrl)
        {
            foreach (KeyValuePair<string, FhirValueSet> versionKvp in collectionKvp.Value.ValueSetsByVersion)
            {
                string key = $"{collectionKvp.Key}|{versionKvp.Key}";

                if (HasValueSet(key))
                {
                    continue;
                }

                // check for restricted output and not seeing this valueSet
                if (restrictOutput &&
                    (!valueSetReferences.ContainsKey(collectionKvp.Key)))
                {
                    continue;
                }

                versionKvp.Value.Resolve(source._codeSystemsByUrl);

                if ((versionKvp.Value.Concepts == null) ||
                    (versionKvp.Value.Concepts.Count == 0))
                {
                    continue;
                }

                if (!_valueSetsByUrl.ContainsKey(collectionKvp.Key))
                {
                    _valueSetsByUrl.Add(collectionKvp.Key, new FhirValueSetCollection(collectionKvp.Key));
                }

                FhirValueSet vs = (FhirValueSet)versionKvp.Value.Clone();

                if (valueSetReferences.ContainsKey(collectionKvp.Key))
                {
                    vs.SetReferences(valueSetReferences[collectionKvp.Key]);
                }

                _valueSetsByUrl[collectionKvp.Key].AddValueSet(vs);
            }
        }
    }

    /// <summary>Gets or sets the type of the package group.</summary>
    public FhirPackageCommon.FhirPackageTypeEnum PackageType { get; set; }

    /// <summary>Gets or sets the major version.</summary>
    public FhirPackageCommon.FhirSequenceEnum FhirSequence { get; set; }

    /// <summary>Gets or sets the package details.</summary>
    public NpmPackageDetails PackageDetails { get; set; }

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

    /// <summary>Gets the profiles by id dictionary.</summary>
    public Dictionary<string, FhirComplex> Profiles { get => _profilesById; }

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

    /// <summary>Gets search parameters defined for all resources.</summary>
    public Dictionary<string, FhirSearchParam> AllResourceParameters { get => _globalSearchParameters; }

    /// <summary>Gets search parameters that control search results.</summary>
    public Dictionary<string, FhirSearchParam> SearchResultParameters { get => _searchResultParameters; }

    /// <summary>Gets search parameters defined for all interactions.</summary>
    public Dictionary<string, FhirSearchParam> AllInteractionParameters { get => _allInteractionParameters; }

    /// <summary>Gets the node info by path dictionary.</summary>
    public Dictionary<string, FhirNodeInfo> NodeByPath { get => _nodeInfoByPath; }

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
    }

    /// <summary>Adds a complex type.</summary>
    /// <param name="complex">The complex.</param>
    public void AddComplexType(FhirComplex complex)
    {
        _complexTypesByName.Add(complex.Path, complex);
    }

    /// <summary>Adds a resource.</summary>
    /// <param name="resource">The resource object.</param>
    public void AddResource(FhirComplex resource)
    {
        _resourcesByName.Add(resource.Path, resource);
    }

    /// <summary>Adds a profile.</summary>
    /// <param name="complex">The complex.</param>
    public void AddProfile(FhirComplex complex)
    {
        _profilesById.Add(complex.Id, complex);

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

        // traverse resources in the search parameter
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

    /// <summary>Adds an operation.</summary>
    /// <param name="operation">The operation.</param>
    public void AddOperation(FhirOperation operation)
    {
        if (!_operationsByUrl.ContainsKey(operation.URL.ToString()))
        {
            _operationsByUrl.Add(operation.URL.ToString(), operation);
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
    /// <param name="set">       [in,out] The set.</param>
    /// <param name="isResource">True if is resource, false if not.</param>
    internal void AddComplexToExportSet(
        FhirComplex complex,
        ref HashSet<string> set,
        bool isResource)
    {
        // add this item
        set.Add(complex.Name);

        // check for a parent type
        if (!string.IsNullOrEmpty(complex.BaseTypeName))
        {
            // add the parent
            AddToExportSet(complex.BaseTypeName, ref set);

            if (isResource)
            {
                // Resources cannot inherit patterns, but they are listed that way today
                // see https://chat.fhir.org/#narrow/stream/179177-conformance/topic/Inheritance.20and.20Cardinality.20Changes
                switch (complex.BaseTypeName)
                {
                    case "CanonicalResource":
                    case "MetadataResource":
                        AddToExportSet("DomainResource", ref set);
                        break;
                }
            }
        }

        // check for element types
        if (complex.Elements != null)
        {
            foreach (KeyValuePair<string, FhirElement> kvp in complex.Elements)
            {
                if (!string.IsNullOrEmpty(kvp.Value.BaseTypeName))
                {
                    // add the element type
                    AddToExportSet(kvp.Value.BaseTypeName, ref set);
                }

                if (kvp.Value.ElementTypes != null)
                {
                    foreach (FhirElementType elementType in kvp.Value.ElementTypes.Values)
                    {
                        // add the element type
                        AddToExportSet(elementType.Name, ref set);

                        if (elementType.Profiles != null)
                        {
                            foreach (FhirElementProfile profile in elementType.Profiles.Values)
                            {
                                AddToExportSet(profile.Name, ref set);
                            }
                        }
                    }
                }
            }
        }

        if (complex.Components != null)
        {
            if (_complexTypesByName.ContainsKey("BackboneElement") &&
                (!set.Contains("BackboneElement")))
            {
                set.Add("BackboneElement");
            }

            foreach (FhirComplex component in complex.Components.Values)
            {
                AddComplexToExportSet(component, ref set, false);
            }
        }
    }

    /// <summary>Recursively adds a resource or type to the export set.</summary>
    /// <param name="name">The name.</param>
    /// <param name="set"> [in,out] The set.</param>
    internal void AddToExportSet(string name, ref HashSet<string> set)
    {
        // if we've already added this, we're done
        if (set.Contains(name))
        {
            return;
        }

        // check for primitive
        if (_primitiveTypesByName.ContainsKey(name))
        {
            // add this item
            set.Add(name);

            // no recursion on primitive types
            return;
        }

        // check for this being a type
        if (_complexTypesByName.ContainsKey(name))
        {
            AddComplexToExportSet(_complexTypesByName[name], ref set, false);
        }

        // check for this being a resource
        if (_resourcesByName.ContainsKey(name))
        {
            AddComplexToExportSet(_resourcesByName[name], ref set, true);
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

    /// <summary>Builds artifact records.</summary>
    /// <returns>The FhirPackageArtifacts.</returns>
    public Dictionary<FhirArtifactClassEnum, IEnumerable<FhirArtifactRecord>> BuildArtifactRecords()
    {
        Dictionary<FhirArtifactClassEnum, IEnumerable<FhirArtifactRecord>> index = new();

        List<FhirArtifactRecord> records = new();

        foreach (FhirPrimitive obj in _primitiveTypesByName.Values)
        {
            records.Add(new()
            {
                ArtifactClass = FhirArtifactClassEnum.PrimitiveType,
                Id = obj.Id,
                Url = obj.URL,
                DefinitionResourceType = "StructureDefinition",
            });
        }

        if (records.Any())
        {
            index.Add(FhirArtifactClassEnum.PrimitiveType, records.ToArray());
            records.Clear();
        }

        foreach (FhirComplex obj in _complexTypesByName.Values)
        {
            records.Add(new()
            {
                ArtifactClass = FhirArtifactClassEnum.ComplexType,
                Id = obj.Id,
                Url = obj.URL,
                DefinitionResourceType = "StructureDefinition",
            });
        }

        if (records.Any())
        {
            index.Add(FhirArtifactClassEnum.ComplexType, records.ToArray());
            records.Clear();
        }

        foreach (FhirComplex obj in _resourcesByName.Values)
        {
            records.Add(new()
            {
                ArtifactClass = FhirArtifactClassEnum.Resource,
                Id = obj.Id,
                Url = obj.URL,
                DefinitionResourceType = "StructureDefinition",
            });
        }

        if (records.Any())
        {
            index.Add(FhirArtifactClassEnum.Resource, records.ToArray());
            records.Clear();
        }

        foreach (FhirComplex obj in _extensionsByUrl.Values)
        {
            records.Add(new()
            {
                ArtifactClass = FhirArtifactClassEnum.Extension,
                Id = obj.Id,
                Url = obj.URL,
                DefinitionResourceType = "StructureDefinition",
            });
        }

        if (records.Any())
        {
            index.Add(FhirArtifactClassEnum.Extension, records.ToArray());
            records.Clear();
        }

        foreach (FhirOperation obj in _operationsByUrl.Values)
        {
            records.Add(new()
            {
                ArtifactClass = FhirArtifactClassEnum.Operation,
                Id = obj.Id,
                Url = obj.URL,
                DefinitionResourceType = "OperationDefinition",
            });
        }

        if (records.Any())
        {
            index.Add(FhirArtifactClassEnum.Operation, records.ToArray());
            records.Clear();
        }

        foreach (FhirSearchParam obj in _searchParamsByUrl.Values)
        {
            records.Add(new()
            {
                ArtifactClass = FhirArtifactClassEnum.SearchParameter,
                Id = obj.Id,
                Url = obj.URL,
                DefinitionResourceType = "SearchParameter",
            });
        }

        if (records.Any())
        {
            index.Add(FhirArtifactClassEnum.SearchParameter, records.ToArray());
            records.Clear();
        }

        foreach (FhirCodeSystem obj in _codeSystemsByUrl.Values)
        {
            records.Add(new()
            {
                ArtifactClass = FhirArtifactClassEnum.CodeSystem,
                Id = obj.Id,
                Url = new Uri(obj.URL),
                DefinitionResourceType = "CodeSystem",
            });
        }

        if (records.Any())
        {
            index.Add(FhirArtifactClassEnum.CodeSystem, records.ToArray());
            records.Clear();
        }

        foreach (FhirValueSetCollection collection in _valueSetsByUrl.Values)
        {
            foreach (FhirValueSet obj in collection.ValueSetsByVersion.Values)
            {
                records.Add(new()
                {
                    ArtifactClass = FhirArtifactClassEnum.ValueSet,
                    Id = obj.Id,
                    Url = new Uri(obj.URL),
                    DefinitionResourceType = "ValueSet",
                });
            }
        }

        if (records.Any())
        {
            index.Add(FhirArtifactClassEnum.ValueSet, records.ToArray());
            records.Clear();
        }

        foreach (FhirComplex obj in _profilesById.Values)
        {
            records.Add(new()
            {
                ArtifactClass = FhirArtifactClassEnum.Profile,
                Id = obj.Id,
                Url = obj.URL,
                DefinitionResourceType = "StructureDefinition",
            });
        }

        if (records.Any())
        {
            index.Add(FhirArtifactClassEnum.Profile, records.ToArray());
            records.Clear();
        }

        return index;
    }
}
