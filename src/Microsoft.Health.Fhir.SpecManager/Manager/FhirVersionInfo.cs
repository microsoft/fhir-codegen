// -------------------------------------------------------------------------------------------------
// <copyright file="FhirVersionInfo.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Health.Fhir.SpecManager.Converters;
using Microsoft.Health.Fhir.SpecManager.Language;
using Microsoft.Health.Fhir.SpecManager.Models;

namespace Microsoft.Health.Fhir.SpecManager.Manager
{
    /// <summary>Information about a FHIR release.</summary>
    public class FhirVersionInfo
    {
        /// <summary>Extension URL for JSON type information.</summary>
        public const string UrlJsonType = "http://hl7.org/fhir/StructureDefinition/structuredefinition-json-type";

        /// <summary>Extension URL for XML type information.</summary>
        public const string UrlXmlType = "http://hl7.org/fhir/StructureDefinition/structuredefinition-xml-type";

        /// <summary>Extension URL for FHIR type information (added R4).</summary>
        public const string UrlFhirType = "http://hl7.org/fhir/StructureDefinition/structuredefinition-fhir-type";

        /// <summary>The known version numbers (for fast checking on version load requests).</summary>
        private static HashSet<int> _knownVersionNumbers = new HashSet<int>()
        {
            2,
            3,
            4,
            5,
        };

        /// <summary>Types of resources to process, by FHIR version.</summary>
        private static Dictionary<int, HashSet<string>> _versionResourcesToProcess = new Dictionary<int, HashSet<string>>()
        {
            {
                2,
                new HashSet<string>()
                {
                    "OperationDefinition",
                    "SearchParameter",
                    "StructureDefinition",
                    "ValueSet",
                }
            },
            {
                3,
                new HashSet<string>()
                {
                    "CapabilityStatement",
                    "CodeSystem",
                    "NamingSystem",
                    "OperationDefinition",
                    "SearchParameter",
                    "StructureDefinition",
                    "ValueSet",
                }
            },
            {
                4,
                new HashSet<string>()
                {
                    "CapabilityStatement",
                    "CodeSystem",
                    "NamingSystem",
                    "OperationDefinition",
                    "SearchParameter",
                    "StructureDefinition",
                    "ValueSet",
                }
            },
            {
                5,
                new HashSet<string>()
                {
                    "CapabilityStatement",
                    "CodeSystem",
                    "NamingSystem",
                    "OperationDefinition",
                    "SearchParameter",
                    "StructureDefinition",
                    "ValueSet",
                }
            },
        };

        /// <summary>Types of resources to ignore, by FHIR version.</summary>
        private static Dictionary<int, HashSet<string>> _versionResourcesToIgnore = new Dictionary<int, HashSet<string>>()
        {
            {
                2,
                new HashSet<string>()
                {
                    "Conformance",
                    "NamingSystem",
                    "ConceptMap",
                    "ImplementationGuide",
                }
            },
            {
                3,
                new HashSet<string>()
                {
                    "CompartmentDefinition",
                    "ConceptMap",
                    "ImplementationGuide",
                    "StructureMap",
                }
            },
            {
                4,
                new HashSet<string>()
                {
                    "CompartmentDefinition",
                    "ConceptMap",
                    "StructureMap",
                }
            },
            {
                5,
                new HashSet<string>()
                {
                    "CompartmentDefinition",
                    "ConceptMap",
                    "StructureMap",
                }
            },
        };

        private static HashSet<string> _npmFilesToIgnore = new HashSet<string>()
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
        private Dictionary<string, FhirSearchParam> _globalSearchParameters;
        private Dictionary<string, FhirSearchParam> _searchResultParameters;
        private Dictionary<string, FhirSearchParam> _allInteractionParameters;
        private Dictionary<string, FhirCodeSystem> _codeSystemsByUrl;
        private Dictionary<string, FhirValueSetCollection> _valueSetsByUrl;
        private Dictionary<string, FhirNodeInfo> _nodeInfoByPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="FhirVersionInfo"/> class. Require major version
        /// (release #) to validate it is supported.
        /// </summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <param name="majorVersion">The major version.</param>
        public FhirVersionInfo(int majorVersion)
        {
            if (!_knownVersionNumbers.Contains(majorVersion))
            {
                throw new Exception($"Invalid FHIR major version: {majorVersion}!");
            }

            // copy required fields
            MajorVersion = majorVersion;

            _fhirConverter = ConverterHelper.ConverterForVersion(majorVersion);

            // create our info dictionaries
            _primitiveTypesByName = new Dictionary<string, FhirPrimitive>();
            _complexTypesByName = new Dictionary<string, FhirComplex>();
            _resourcesByName = new Dictionary<string, FhirComplex>();
            _extensionsByUrl = new Dictionary<string, FhirComplex>();
            _extensionsByPath = new Dictionary<string, Dictionary<string, FhirComplex>>();
            _profilesById = new Dictionary<string, FhirComplex>();
            _profilesByBaseType = new Dictionary<string, Dictionary<string, FhirComplex>>();
            _systemOperations = new Dictionary<string, FhirOperation>();
            _globalSearchParameters = new Dictionary<string, FhirSearchParam>();
            _searchResultParameters = new Dictionary<string, FhirSearchParam>();
            _allInteractionParameters = new Dictionary<string, FhirSearchParam>();
            _codeSystemsByUrl = new Dictionary<string, FhirCodeSystem>();
            _valueSetsByUrl = new Dictionary<string, FhirValueSetCollection>();
            _nodeInfoByPath = new Dictionary<string, FhirNodeInfo>();
        }

        /// <summary>Values that represent search magic parameters.</summary>
        internal enum SearchMagicParameter
        {
            /// <summary>An enum constant representing all resource option.</summary>
            Global,

            /// <summary>An enum constant representing the search result option.</summary>
            Result,

            /// <summary>An enum constant representing all interaction option.</summary>
            Interaction,
        }

        /// <summary>Gets or sets the major version.</summary>
        ///
        /// <value>The major version.</value>
        public int MajorVersion { get; set; }

        /// <summary>Gets or sets the name of the package release.</summary>
        /// <value>The name of the package release.</value>
        public string ReleaseName { get; set; }

        /// <summary>Gets or sets the name of the package.</summary>
        ///
        /// <value>The name of the package.</value>
        public string PackageName { get; set; }

        /// <summary>Gets or sets the name of the examples package.</summary>
        public string ExamplesPackageName { get; set; }

        /// <summary>Gets or sets the name of the expansions package.</summary>
        public string ExpansionsPackageName { get; set; }

        /// <summary>Gets or sets the version string.</summary>
        /// <value>The version string.</value>
        public string VersionString { get; set; }

        /// <summary>Gets or sets a value indicating whether this object is development build.</summary>
        /// <value>True if this object is development build, false if not.</value>
        public bool IsDevBuild { get; set; }

        /// <summary>Gets or sets the development branch.</summary>
        /// <value>The development branch.</value>
        public string DevBranch { get; set; }

        /// <summary>Gets or sets a value indicating whether this object is local build.</summary>
        ///
        /// <value>True if this object is local build, false if not.</value>
        public bool IsLocalBuild { get; set; }

        /// <summary>Gets or sets the pathname of the local directory.</summary>
        ///
        /// <value>The pathname of the local directory.</value>
        public string LocalDirectory { get; set; }

        /// <summary>Gets or sets a value indicating whether this object is on disk.</summary>
        ///
        /// <value>True if available, false if not.</value>
        public bool IsOnDisk { get; set; }

        /// <summary>Gets or sets the Date/Time of the last downloaded.</summary>
        ///
        /// <value>The last downloaded.</value>
        public DateTime? LastDownloaded { get; set; }

        /// <summary>Gets a dictionary with the known primitive types for this version of FHIR.</summary>
        /// <value>A dictionary of the primitive types.</value>
        public Dictionary<string, FhirPrimitive> PrimitiveTypes { get => _primitiveTypesByName; }

        /// <summary>Gets a dictionary with the known complex types for this version of FHIR.</summary>
        /// <value>A dictionary of the complex types.</value>
        public Dictionary<string, FhirComplex> ComplexTypes { get => _complexTypesByName; }

        /// <summary>Gets a dictionary with the known resources for this version of FHIR.</summary>
        /// <value>A dictionary of the resources.</value>
        public Dictionary<string, FhirComplex> Resources { get => _resourcesByName; }

        /// <summary>Gets the profiles.</summary>
        public Dictionary<string, FhirComplex> Profiles { get => _profilesById; }

        /// <summary>Gets the type of the profiles by base.</summary>
        public Dictionary<string, Dictionary<string, FhirComplex>> ProfilesByBaseType { get => _profilesByBaseType; }

        /// <summary>Gets URL of the extensions by.</summary>
        /// <value>The extensions by URL.</value>
        public Dictionary<string, FhirComplex> ExtensionsByUrl { get => _extensionsByUrl; }

        /// <summary>Gets the code systems.</summary>
        /// <value>The code systems.</value>
        public Dictionary<string, FhirCodeSystem> CodeSystems { get => _codeSystemsByUrl; }

        /// <summary>Gets the value sets by URL by version.</summary>
        /// <value>The value sets by URL by version.</value>
        public Dictionary<string, FhirValueSetCollection> ValueSetsByUrl { get => _valueSetsByUrl; }

        /// <summary>Gets the extensions per path, in a dictionary keyed by URL.</summary>
        /// <value>The extensions.</value>
        public Dictionary<string, Dictionary<string, FhirComplex>> ExtensionsByPath { get => _extensionsByPath; }

        /// <summary>Gets the system operations.</summary>
        /// <value>The system operations.</value>
        public Dictionary<string, FhirOperation> SystemOperations { get => _systemOperations; }

        /// <summary>Gets options for controlling all resource.</summary>
        /// <value>Options that control all resource.</value>
        public Dictionary<string, FhirSearchParam> AllResourceParameters { get => _globalSearchParameters; }

        /// <summary>Gets options for controlling the search result.</summary>
        /// <value>Options that control the search result.</value>
        public Dictionary<string, FhirSearchParam> SearchResultParameters { get => _searchResultParameters; }

        /// <summary>Gets options for controlling all interaction.</summary>
        /// <value>Options that control all interaction.</value>
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
        internal void AddPrimitive(FhirPrimitive primitive)
        {
            _primitiveTypesByName.Add(primitive.Name, primitive);
        }

        /// <summary>Adds a complex type.</summary>
        /// <param name="complex">The complex.</param>
        internal void AddComplexType(FhirComplex complex)
        {
            _complexTypesByName.Add(complex.Path, complex);
        }

        /// <summary>Adds a resource.</summary>
        /// <param name="resource">The resource object.</param>
        internal void AddResource(FhirComplex resource)
        {
            _resourcesByName.Add(resource.Path, resource);
        }

        /// <summary>Adds a profile.</summary>
        /// <param name="complex">The complex.</param>
        internal void AddProfile(FhirComplex complex)
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
        internal void AddSearchParameter(FhirSearchParam searchParam)
        {
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
        internal void AddOperation(FhirOperation operation)
        {
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
        internal void AddCodeSystem(FhirCodeSystem codeSystem)
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
        internal void AddValueSet(FhirValueSet valueSet)
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
        internal bool HasValueSet(string urlOrKey)
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
        internal bool TryGetValueSet(string urlOrKey, out FhirValueSet vs)
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
        internal void AddExtension(FhirComplex extension)
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

        /// <summary>Determine if we should process resource.</summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        public bool ShouldProcessResource(string resourceName)
        {
            if (_versionResourcesToProcess[MajorVersion].Contains(resourceName))
            {
                return true;
            }

            return false;
        }

        /// <summary>Determine if we should ignore resource.</summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        public bool ShouldIgnoreResource(string resourceName)
        {
            if (_versionResourcesToIgnore[MajorVersion].Contains(resourceName))
            {
                return true;
            }

            return false;
        }

        /// <summary>Determine if we should skip file.</summary>
        /// <param name="filename">Filename of the file.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        public static bool ShouldSkipFile(string filename)
        {
            if (_npmFilesToIgnore.Contains(filename))
            {
                return true;
            }

            return false;
        }

        /// <summary>Query if 'key' has complex.</summary>
        /// <param name="key">The key.</param>
        /// <returns>True if complex, false if not.</returns>
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

            int index = path.IndexOf('.');

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

        /// <summary>Parses resource an object from the given string.</summary>
        /// <exception cref="JsonException">Thrown when a JSON error condition occurs.</exception>
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

        /// <summary>Adds a versioned parameter.</summary>
        /// <param name="searchMagicType">Type of the search magic.</param>
        /// <param name="name">           The name.</param>
        /// <param name="parameterType">  Type of the parameter.</param>
        internal void AddVersionedParam(
            SearchMagicParameter searchMagicType,
            string name,
            string parameterType)
        {
            switch (searchMagicType)
            {
                case SearchMagicParameter.Global:
                    AddVersionedParam(_globalSearchParameters, name, parameterType);
                    break;

                case SearchMagicParameter.Result:
                    AddVersionedParam(_searchResultParameters, name, parameterType);
                    break;

                case SearchMagicParameter.Interaction:
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

            dict.Add(
                name,
                new FhirSearchParam(
                    name,
                    new Uri($"http://hl7.org/fhir/{ReleaseName}/search.html#{name.Substring(1)}"),
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
        }

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

        /// <summary>Copies for export.</summary>
        /// <param name="primitiveTypeMap">     The FHIR to language primitive map.</param>
        /// <param name="exportList">           List of exports.</param>
        /// <param name="copyPrimitives">       (Optional) True to copy primitives.</param>
        /// <param name="copyComplexTypes">     (Optional) True to copy complex types.</param>
        /// <param name="copyResources">        (Optional) True to copy resources.</param>
        /// <param name="copyExtensions">       (Optional) True to copy extensions.</param>
        /// <param name="copyProfiles">         (Optional) True to copy profiles.</param>
        /// <param name="extensionUrls">        (Optional) The extension URLs.</param>
        /// <param name="extensionElementPaths">(Optional) The extension paths.</param>
        /// <param name="serverInfo">           (Optional) Information describing the server.</param>
        /// <param name="includeExperimental">  (Optional) True to include, false to exclude the
        ///  experimental.</param>
        /// <returns>A FhirVersionInfo.</returns>
        internal FhirVersionInfo CopyForExport(
            Dictionary<string, string> primitiveTypeMap,
            IEnumerable<string> exportList,
            bool copyPrimitives = true,
            bool copyComplexTypes = true,
            bool copyResources = true,
            bool copyExtensions = true,
            bool copyProfiles = true,
            HashSet<string> extensionUrls = null,
            HashSet<string> extensionElementPaths = null,
            FhirServerInfo serverInfo = null,
            bool includeExperimental = false)
        {
            // create our return object
            FhirVersionInfo info = new FhirVersionInfo(MajorVersion)
            {
                ReleaseName = this.ReleaseName,
                PackageName = this.PackageName,
                VersionString = this.VersionString,
                IsDevBuild = this.IsDevBuild,
                DevBranch = this.DevBranch,
                IsLocalBuild = this.IsLocalBuild,
                LocalDirectory = this.LocalDirectory,
                IsOnDisk = this.IsOnDisk,
                LastDownloaded = this.LastDownloaded,
            };

            bool restrictOutput = false;
            bool restrictResources = false;

            HashSet<string> exportSet = new HashSet<string>();

            // figure out all the the dependencies we need to include based on requests
            if (exportList != null)
            {
                foreach (string path in exportList)
                {
                    AddToExportSet(path, ref exportSet);
                }

                if (exportSet.Count > 0)
                {
                    restrictOutput = true;
                }
            }

            // only want server restrictions if there is not an explicit one
            if ((serverInfo != null) &&
                (exportSet.Count == 0))
            {
                foreach (FhirServerResourceInfo resource in serverInfo.ResourceInteractions.Values)
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
            if (copyPrimitives)
            {
                foreach (KeyValuePair<string, FhirPrimitive> kvp in _primitiveTypesByName)
                {
                    // check for restricting output
                    if (restrictOutput && (!exportSet.Contains(kvp.Key)))
                    {
                        continue;
                    }

                    // check for experimental - unless this is specifically included
                    if ((!restrictOutput) &&
                        (!includeExperimental) &&
                        kvp.Value.IsExperimental)
                    {
                        continue;
                    }

                    FhirPrimitive node = (FhirPrimitive)kvp.Value.Clone();

                    info._nodeInfoByPath.Add(
                        node.Path,
                        new FhirNodeInfo(FhirNodeInfo.FhirNodeType.Primitive, node));

                    info._primitiveTypesByName.Add(kvp.Key, node);

                    // update type to reflect language
                    if (primitiveTypeMap.ContainsKey(kvp.Value.Name))
                    {
                        node.BaseTypeName = primitiveTypeMap[kvp.Value.Name];
                    }
                }
            }

            // check if we are exporting complex types
            if (copyComplexTypes)
            {
                foreach (KeyValuePair<string, FhirComplex> kvp in _complexTypesByName)
                {
                    // check for restricting output
                    if (restrictOutput && (!exportSet.Contains(kvp.Key)))
                    {
                        continue;
                    }

                    // check for experimental - unless this is specifically included
                    if ((!restrictOutput) &&
                        (!includeExperimental) &&
                        kvp.Value.IsExperimental)
                    {
                        continue;
                    }

                    FhirComplex node = kvp.Value.DeepCopy(
                        primitiveTypeMap,
                        true,
                        false,
                        valueSetReferences,
                        info._nodeInfoByPath,
                        null,
                        null,
                        null,
                        null,
                        includeExperimental);

                    info._nodeInfoByPath.Add(
                        node.Path,
                        new FhirNodeInfo(FhirNodeInfo.FhirNodeType.DataType, node));

                    info._complexTypesByName.Add(
                        kvp.Key,
                        node);
                }
            }

            // check if we are exporting resources
            if (copyResources)
            {
                foreach (KeyValuePair<string, FhirComplex> kvp in _resourcesByName)
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
                        (!includeExperimental) &&
                        kvp.Value.IsExperimental)
                    {
                        continue;
                    }

                    if ((serverInfo == null) ||
                        (!serverInfo.ResourceInteractions.ContainsKey(kvp.Key)))
                    {
                        FhirComplex node = kvp.Value.DeepCopy(
                            primitiveTypeMap,
                            true,
                            false,
                            valueSetReferences,
                            info._nodeInfoByPath,
                            null,
                            null,
                            null,
                            null,
                            includeExperimental);

                        info._nodeInfoByPath.Add(
                            node.Path,
                            new FhirNodeInfo(FhirNodeInfo.FhirNodeType.Resource, node));

                        info._resourcesByName.Add(
                            kvp.Key,
                            node);
                    }
                    else
                    {
                        FhirComplex node = kvp.Value.DeepCopy(
                            primitiveTypeMap,
                            true,
                            false,
                            valueSetReferences,
                            info._nodeInfoByPath,
                            serverInfo.ResourceInteractions[kvp.Key].SearchParameters,
                            serverInfo.ServerSearchParameters,
                            serverInfo.ResourceInteractions[kvp.Key].Operations,
                            serverInfo.ServerOperations,
                            includeExperimental);

                        info._nodeInfoByPath.Add(
                            node.Path,
                            new FhirNodeInfo(FhirNodeInfo.FhirNodeType.Resource, node));

                        info._resourcesByName.Add(
                            kvp.Key,
                            node);
                    }
                }
            }

            if (copyProfiles)
            {
                foreach (FhirComplex profile in _profilesById.Values)
                {
                    if (info._resourcesByName.ContainsKey(profile.BaseTypeName) ||
                        info._complexTypesByName.ContainsKey(profile.BaseTypeName) ||
                        info._primitiveTypesByName.ContainsKey(profile.BaseTypeName))
                    {
                        FhirComplex node = profile.DeepCopy(
                            primitiveTypeMap,
                            true,
                            false,
                            valueSetReferences,
                            null,
                            null,
                            null,
                            null,
                            null,
                            includeExperimental);

                        info._nodeInfoByPath.Add(
                            node.Id,
                            new FhirNodeInfo(FhirNodeInfo.FhirNodeType.Profile, node));

                        info.AddProfile(node);
                    }
                }
            }

            bool checkUrls = (extensionUrls != null) && (extensionUrls.Count != 0);
            bool checkPaths = (extensionElementPaths != null) && (extensionElementPaths.Count != 0);

            if (copyExtensions)
            {
                // need to work directly with extensions due to nature of filtering
                foreach (FhirComplex extension in _extensionsByUrl.Values)
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
                    if (checkUrls && (!extensionUrls.Contains(extension.URL.ToString())))
                    {
                        continue;
                    }

                    // check for including extensions by path
                    if (checkPaths && (!extension.ContextElements.Union(extensionElementPaths).Any()))
                    {
                        continue;
                    }

                    // add this extension using the primary function (adds to multiple dictionaries)
                    info.AddExtension(
                        extension.DeepCopy(
                            primitiveTypeMap,
                            true,
                            false,
                            valueSetReferences,
                            null));
                }
            }

            if (serverInfo == null)
            {
                foreach (KeyValuePair<string, FhirOperation> kvp in _systemOperations)
                {
                    info._systemOperations.Add(kvp.Key, (FhirOperation)kvp.Value.Clone());
                }
            }
            else
            {
                foreach (KeyValuePair<string, FhirOperation> kvp in _systemOperations)
                {
                    if (serverInfo.ServerOperations.ContainsKey(kvp.Value.Code))
                    {
                        info._systemOperations.Add(kvp.Key, (FhirOperation)kvp.Value.Clone());
                    }
                }

                foreach (KeyValuePair<string, FhirServerOperation> kvp in serverInfo.ServerOperations)
                {
                    if (info._systemOperations.ContainsKey(kvp.Key))
                    {
                        continue;
                    }

                    info._systemOperations.Add(
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

            if (serverInfo == null)
            {
                foreach (KeyValuePair<string, FhirSearchParam> kvp in _globalSearchParameters)
                {
                    if ((!includeExperimental) && kvp.Value.IsExperimental)
                    {
                        continue;
                    }

                    info._globalSearchParameters.Add(kvp.Key, (FhirSearchParam)kvp.Value.Clone());
                }
            }
            else
            {
                foreach (KeyValuePair<string, FhirSearchParam> kvp in _globalSearchParameters)
                {
                    if (serverInfo.ServerSearchParameters.ContainsKey(kvp.Value.Code))
                    {
                        info._globalSearchParameters.Add(kvp.Key, (FhirSearchParam)kvp.Value.Clone());
                    }
                }
            }

            foreach (KeyValuePair<string, FhirSearchParam> kvp in _searchResultParameters)
            {
                if ((!includeExperimental) && kvp.Value.IsExperimental)
                {
                    continue;
                }

                info._searchResultParameters.Add(kvp.Key, (FhirSearchParam)kvp.Value.Clone());
            }

            foreach (KeyValuePair<string, FhirSearchParam> kvp in _allInteractionParameters)
            {
                if ((!includeExperimental) && kvp.Value.IsExperimental)
                {
                    continue;
                }

                info._allInteractionParameters.Add(kvp.Key, (FhirSearchParam)kvp.Value.Clone());
            }

            foreach (KeyValuePair<string, FhirValueSetCollection> collectionKvp in _valueSetsByUrl)
            {
                foreach (KeyValuePair<string, FhirValueSet> versionKvp in collectionKvp.Value.ValueSetsByVersion)
                {
                    string key = $"{collectionKvp.Key}|{versionKvp.Key}";

                    if (info.HasValueSet(key))
                    {
                        continue;
                    }

                    // check for restricted output and not seeing this valueSet
                    if (restrictOutput &&
                        (!valueSetReferences.ContainsKey(collectionKvp.Key)))
                    {
                        continue;
                    }

                    versionKvp.Value.Resolve(_codeSystemsByUrl);

                    if ((versionKvp.Value.Concepts == null) ||
                        (versionKvp.Value.Concepts.Count == 0))
                    {
                        continue;
                    }

                    if (!info._valueSetsByUrl.ContainsKey(collectionKvp.Key))
                    {
                        info._valueSetsByUrl.Add(collectionKvp.Key, new FhirValueSetCollection(collectionKvp.Key));
                    }

                    FhirValueSet vs = (FhirValueSet)versionKvp.Value.Clone();

                    if (valueSetReferences.ContainsKey(collectionKvp.Key))
                    {
                        vs.SetReferences(valueSetReferences[collectionKvp.Key]);
                    }

                    info._valueSetsByUrl[collectionKvp.Key].AddValueSet(vs);
                }
            }

            return info;
        }

        /// <summary>Gets major version.</summary>
        /// <param name="versionString">The version string.</param>
        /// <returns>The major version.</returns>
        internal static int GetMajorVersion(string versionString)
        {
            if (string.IsNullOrEmpty(versionString))
            {
                return 0;
            }
            else
            {
                // create our JSON converter
                switch (versionString[0])
                {
                    case '1':
                    case '2':
                        return 2;

                    case '3':
                        return 3;

                    case '4':

                        if ((versionString[2] == '4') ||
                            (versionString[2] == '5') ||
                            (versionString[2] == '6'))
                        {
                            return 5;
                        }

                        return 4;

                    case '5':
                        return 5;
                }
            }

            return 0;
        }
    }
}
