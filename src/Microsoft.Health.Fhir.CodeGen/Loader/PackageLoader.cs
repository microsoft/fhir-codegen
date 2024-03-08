// <copyright file="PackageLoader.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using Hl7.Fhir.Language.Debugging;
using System.Diagnostics;
using System.Net;
using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using Microsoft.Health.Fhir.PackageManager;
using Hl7.Fhir.Serialization;
using System.Text.Json;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;
using Microsoft.Health.Fhir.CodeGenCommon.Models;

namespace Microsoft.Health.Fhir.CodeGen.Loader;

/// <summary>A package loader.</summary>
public class PackageLoader
{
    /// <summary>(Immutable) The cache.</summary>
    private readonly IFhirPackageClient _cache;

    /// <summary>(Immutable) The sorted order for definition types we want to load.</summary>
    private static readonly string[] _sortedLoadOrder = new string[]
    {
        "CodeSystem",
        "ValueSet",
        "StructureDefinition",
        "SearchParameter",
        "OperationDefinition",
        "CapabilityStatement",
        "Conformance",
        "ImplementationGuide",
        "CompartmentDefinition",
    };

    /// <summary>(Immutable) List of types of the sorted resources.</summary>
    private static readonly HashSet<string> _sortedResourceTypes = new HashSet<string>(_sortedLoadOrder);

    private JsonSerializerOptions _jsonOptions;

    /// <summary>The lenient JSON parser.</summary>
    private FhirJsonPocoDeserializer _jsonParser;

    /// <summary>The lenient XML parser.</summary>
    private FhirXmlPocoDeserializer _xmlParser;

    private class LoadFunctions
    {
        public required Func<string, string, Task<CapabilityStatement?>> ParseCapabilityStatement;
        public required Func<string, string, Task<CodeSystem?>> ParseCodeSystem;
        public required Func<string, string, Task<CompartmentDefinition?>> ParseCompartmentDef;
        public required Func<string, string, Task<ImplementationGuide?>> ParseImplementationGuide;
        public required Func<string, string, Task<OperationDefinition?>> ParseOperationDef;
        public required Func<string, string, Task<SearchParameter?>> ParseSearchParam;
        public required Func<string, string, Task<StructureDefinition?>> ParseStructureDef;
        public required Func<string, string, Task<ValueSet?>> ParseValueSet;
    }

    private LoadFunctions _loadFunctionsR5;
    private LoadFunctions _loadFunctionsR4B;
    private LoadFunctions _loadFunctionsR3;
    private LoadFunctions _loadFunctionsR2;

    private Microsoft.Health.Fhir.CrossVersion.Converter_43_50? _converter_43_50 = null;
    private Microsoft.Health.Fhir.CrossVersion.Converter_30_50? _converter_30_50 = null;
    private Microsoft.Health.Fhir.CrossVersion.Converter_20_50? _converter_20_50 = null;

    private object _convertLockObject = new();

    /// <summary>Initializes a new instance of the <see cref="PackageLoader"/> class.</summary>
    /// <param name="cache">The cache.</param>
    /// <param name="opts"> Options for controlling the operation.</param>
    public PackageLoader(IFhirPackageClient cache, LoaderOptions opts)
    {
        _cache = cache;

        _jsonOptions = opts.FhirJsonOptions;
        _jsonParser = new(opts.FhirJsonSettings);

        _xmlParser = new(opts.FhirXmlSettings);

        switch (opts.JsonModel)
        {
            default:
            case LoaderOptions.JsonDeserializationModel.Poco:
                {
                    _loadFunctionsR5 = new()
                    {
                        ParseCapabilityStatement = ParseContentsPoco<CapabilityStatement>,
                        ParseCodeSystem = ParseContentsPoco<CodeSystem>,
                        ParseCompartmentDef = ParseContentsPoco<CompartmentDefinition>,
                        ParseImplementationGuide = ParseContentsPoco<ImplementationGuide>,
                        ParseOperationDef = ParseContentsPoco<OperationDefinition>,
                        ParseSearchParam = ParseContentsPoco<SearchParameter>,
                        ParseStructureDef = ParseContentsPoco<StructureDefinition>,
                        ParseValueSet = ParseContentsPoco<ValueSet>,
                    };
                }
                break;

            case LoaderOptions.JsonDeserializationModel.Default:
            case LoaderOptions.JsonDeserializationModel.SystemTextJson:
                {
                    _loadFunctionsR5 = new()
                    {
                        ParseCapabilityStatement = ParseContentsSystemTextStream<CapabilityStatement>,
                        ParseCodeSystem = ParseContentsSystemTextStream<CodeSystem>,
                        ParseCompartmentDef = ParseContentsSystemTextStream<CompartmentDefinition>,
                        ParseImplementationGuide = ParseContentsSystemTextStream<ImplementationGuide>,
                        ParseOperationDef = ParseContentsSystemTextStream<OperationDefinition>,
                        ParseSearchParam = ParseContentsSystemTextStream<SearchParameter>,
                        ParseStructureDef = ParseContentsSystemTextStream<StructureDefinition>,
                        ParseValueSet = ParseContentsSystemTextStream<ValueSet>,
                    };
                }
                break;
        }
        
        _loadFunctionsR4B = new()
        {
            ParseCapabilityStatement = ParseContents43<CapabilityStatement>,
            ParseCodeSystem = ParseContents43<CodeSystem>,
            ParseCompartmentDef = ParseContents43<CompartmentDefinition>,
            ParseImplementationGuide = ParseContents43<ImplementationGuide>,
            ParseOperationDef = ParseContents43<OperationDefinition>,
            ParseSearchParam = ParseContents43<SearchParameter>,
            ParseStructureDef = ParseContents43<StructureDefinition>,
            ParseValueSet = ParseContents43<ValueSet>,
        };

        _loadFunctionsR3 = new()
        {
            ParseCapabilityStatement = ParseContents30<CapabilityStatement>,
            ParseCodeSystem = ParseContents30<CodeSystem>,
            ParseCompartmentDef = ParseContents30<CompartmentDefinition>,
            ParseImplementationGuide = ParseContents30<ImplementationGuide>,
            ParseOperationDef = ParseContents30<OperationDefinition>,
            ParseSearchParam = ParseContents30<SearchParameter>,
            ParseStructureDef = ParseContents30<StructureDefinition>,
            ParseValueSet = ParseContents30<ValueSet>,
        };

        _loadFunctionsR2 = new()
        {
            ParseCapabilityStatement = ParseContents20<CapabilityStatement>,
            ParseCodeSystem = ParseContents20<CodeSystem>,
            ParseCompartmentDef = ParseContents20<CompartmentDefinition>,
            ParseImplementationGuide = ParseContents20<ImplementationGuide>,
            ParseOperationDef = ParseContents20<OperationDefinition>,
            ParseSearchParam = ParseContents20<SearchParameter>,
            ParseStructureDef = ParseContents20<StructureDefinition>,
            ParseValueSet = ParseContents20<ValueSet>,
        };
    }

    /// <summary>Adds all interaction parameters to a core definition collection.</summary>
    /// <param name="dc">The device-context.</param>
    private void AddAllInteractionParameters(DefinitionCollection dc)
    {
        dc.AddHttpQueryParameter(new()
        {
            Name = "_format",
            Url = "http://hl7.org/fhir/http.html#format",
            Description = "Parameter to specify alternative response formats by their MIME-types.",
            ParamType = SearchParamType.String,
        });

        dc.AddHttpQueryParameter(new()
        {
            Name = "_summary",
            Url = "http://hl7.org/fhir/search.html#summary",
            Description = "Request to return a portion of matching resources.",
            ParamType = SearchParamType.Token,
            AllowedValues = new[] { "true", "false", "count", "data", "text", },
        });

        dc.AddHttpQueryParameter(new()
        {
            Name = "_elements",
            Url = "http://hl7.org/fhir/search.html#elements",
            Description = "Request to return specific elements from resources.",
            ParamType = SearchParamType.Token,
        });

        // add parameters from R4 and later
        if (dc.FhirSequence >= FhirReleases.FhirSequenceCodes.R4)
        {
            dc.AddHttpQueryParameter(new()
            {
                Name = "_pretty",
                Url = "http://hl7.org/fhir/http.html#pretty",
                Description = "Ask for a pretty printed response for human convenience.",
                ParamType = SearchParamType.String,
                AllowedValues = new[] { "true", "false", },
            });
        }
    }

    /// <summary>Adds a search result parameters to a core definition collection.</summary>
    /// <param name="dc">The device-context.</param>
    private void AddSearchResultParameters(DefinitionCollection dc)
    {
        dc.AddSearchResultParameter(new()
        {
            Name = "_sort",
            Url = "http://hl7.org/fhir/search.html#sort",
            Description = "Used to indicate which order to return the results, which can have a value of one of the search parameters.",
            ParamType = SearchParamType.String,
        });

        dc.AddSearchResultParameter(new()
        {
            Name = "_count",
            Url = "http://hl7.org/fhir/search.html#count",
            Description = "A hint to the server regarding how many resources should be returned in a single page. Servers SHALL NOT return more resources than requested but are allowed to return less than the client requested.",
            ParamType = SearchParamType.Number,
        });

        dc.AddSearchResultParameter(new()
        {
            Name = "_include",
            Url = "http://hl7.org/fhir/search.html#include",
            Description = "Request to return resources related to the search results, by moving forward across references.",
            ParamType = SearchParamType.String,
        });

        dc.AddSearchResultParameter(new()
        {
            Name = "_revinclude",
            Url = "http://hl7.org/fhir/search.html#revinclude",
            Description = "Request to return resources related to the search results, by moving backwards across references.",
            ParamType = SearchParamType.String,
        });

        dc.AddSearchResultParameter(new()
        {
            Name = "_contained",
            Url = "http://hl7.org/fhir/search.html#contained",
            Description = "Request modification to handling of contained resource searching.",
            ParamType = SearchParamType.Token,
            AllowedValues = new[] { "true", "false", "both", },
        });

        dc.AddSearchResultParameter(new()
        {
            Name = "_containedType",
            Url = "http://hl7.org/fhir/search.html#containedType",
            Description = "When contained resources are being returned, whether the server should return either the container or the contained resource.",
            ParamType = SearchParamType.Token,
            AllowedValues = new[] { "container", "contained", },
        });

        // add parameters from R4 and later
        if (dc.FhirSequence >= FhirReleases.FhirSequenceCodes.R4)
        {
            dc.AddSearchResultParameter(new()
            {
                Name = "_total",
                Url = "http://hl7.org/fhir/search.html#total",
                Description = "Optimization hint for servers indicating reliance on the Bundle.total element.",
                ParamType = SearchParamType.Token,
                AllowedValues = new[] { "none", "estimate", "accurate" },
            });
        }
    }

    /// <summary>Adds a missing core search parameters to a core definition collection.</summary>
    /// <param name="dc">The device-context.</param>
    private void AddMissingCoreSearchParameters(DefinitionCollection dc)
    {
        dc.AddSearchParameter(doNotOverwrite: true, sp: new()
        {
            Id = "Resource-content",
            Name = "_content",
            Code = "_content",
            Url = "http://hl7.org/fhir/SearchParameter/Resource-content",
            Version = dc.FhirSequence.ToLongVersion(),
            Title = "Resource content filter",
            Status = PublicationStatus.Active,
            Description = "Search on the entire content of the resource.",
            Base = new VersionIndependentResourceTypesAll?[] { VersionIndependentResourceTypesAll.Resource },
            Type = SearchParamType.Special,
        });

        dc.AddSearchParameter(doNotOverwrite: true, sp: new()
        {
            Id = "Resource-filter",
            Name = "_filter",
            Code = "_filter",
            Url = "http://hl7.org/fhir/SearchParameter/Resource-filter",
            Version = dc.FhirSequence.ToLongVersion(),
            Title = "Advanced search filter",
            Status = PublicationStatus.Active,
            Description = "Filter search parameter which supports a more sophisticated grammar for searching.",
            Base = new VersionIndependentResourceTypesAll?[] { VersionIndependentResourceTypesAll.Resource },
            Type = SearchParamType.Special,
        });

        dc.AddSearchParameter(doNotOverwrite: true, sp: new()
        {
            Id = "Resource-text",
            Name = "_text",
            Code = "_text",
            Url = "http://hl7.org/fhir/SearchParameter/Resource-text",
            Version = dc.FhirSequence.ToLongVersion(),
            Title = "Resource text filter",
            Status = PublicationStatus.Active,
            Description = "Search the narrative content of a resource.",
            Base = new VersionIndependentResourceTypesAll?[] { VersionIndependentResourceTypesAll.Resource },
            Type = SearchParamType.String,
        });

        dc.AddSearchParameter(doNotOverwrite: true, sp: new()
        {
            Id = "Resource-list",
            Name = "_list",
            Code = "_list",
            Url = "http://hl7.org/fhir/SearchParameter/Resource-list",
            Version = dc.FhirSequence.ToLongVersion(),
            Title = "List reference filter",
            Status = PublicationStatus.Active,
            Description = "Filter based on resources referenced by a List resource.",
            Base = new VersionIndependentResourceTypesAll?[] { VersionIndependentResourceTypesAll.Resource },
            Type = SearchParamType.Reference,
            Target = new VersionIndependentResourceTypesAll?[] { VersionIndependentResourceTypesAll.List },
        });

        if (dc.FhirSequence >= FhirReleases.FhirSequenceCodes.R4)
        {
            dc.AddSearchParameter(doNotOverwrite: true, sp: new()
            {
                Id = "Resource-has",
                Name = "_has",
                Code = "_has",
                Url = "http://hl7.org/fhir/SearchParameter/Resource-has",
                Version = dc.FhirSequence.ToLongVersion(),
                Title = "Limited support for reverse chaining",
                Status = PublicationStatus.Active,
                Description = "For selecting resources based on the properties of resources that refer to them.",
                Base = new VersionIndependentResourceTypesAll?[] { VersionIndependentResourceTypesAll.Resource },
                Type = SearchParamType.Special,
            });

            dc.AddSearchParameter(doNotOverwrite: true, sp: new()
            {
                Id = "Resource-type",
                Name = "_type",
                Code = "_type",
                Url = "http://hl7.org/fhir/SearchParameter/Resource-type",
                Version = dc.FhirSequence.ToLongVersion(),
                Title = "Resource type filter",
                Status = PublicationStatus.Active,
                Description = "For filtering resources based on their type in searches across resource types.",
                Base = new VersionIndependentResourceTypesAll?[] { VersionIndependentResourceTypesAll.Resource },
                Type = SearchParamType.Token,
            });
        }
    }

    /// <summary>Loads a package.</summary>
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    /// <param name="name">    The name.</param>
    /// <param name="packages">The cached package.</param>
    /// <returns>An asynchronous result that yields the package.</returns>
    public async Task<DefinitionCollection?> LoadPackages(string name, IEnumerable<PackageCacheEntry> packages)
    {
        DefinitionCollection definitions = new()
        {
            Name = name,
        };

        foreach (PackageCacheEntry cachedPackage in packages)
        {
            CachePackageManifest? manifest = _cache.GetManifest(cachedPackage);
            if (manifest == null)
            {
                throw new Exception("Failed to load package manifest");
            }
            definitions.Manifests.Add(cachedPackage.ResolvedDirective, manifest);

            if (string.IsNullOrEmpty(definitions.MainPackageId) || name.Equals(manifest.Name, StringComparison.OrdinalIgnoreCase))
            {
                definitions.MainPackageId = manifest.Name;
                definitions.MainPackageVersion = manifest.Version;
                definitions.MainPackageCanonical = manifest.CanonicalUrl;
            }

            // update the collection FHIR version based on the first package we come across with one
            if ((definitions.FhirVersion == null) && manifest.AllFhirVersions.Any())
            {
                definitions.FhirSequence = FhirReleases.FhirVersionToSequence(manifest.AllFhirVersions.First());

                definitions.FhirVersion = definitions.FhirSequence switch
                {
                    FhirReleases.FhirSequenceCodes.Unknown => null,
                    FhirReleases.FhirSequenceCodes.DSTU2 => FHIRVersion.N1_0,
                    FhirReleases.FhirSequenceCodes.STU3 => FHIRVersion.N3_0,
                    FhirReleases.FhirSequenceCodes.R4 => FHIRVersion.N4_0,
                    FhirReleases.FhirSequenceCodes.R4B => FHIRVersion.N4_3,
                    FhirReleases.FhirSequenceCodes.R5 => FHIRVersion.N5_0,
                    _ => null,
                };
            }

            FhirReleases.FhirSequenceCodes packageFhirVersion = manifest.AllFhirVersions.Any()
                ? FhirReleases.FhirVersionToSequence(manifest.AllFhirVersions.First())
                : definitions.FhirSequence;

            // set the load functions for the correct version of FHIR
            LoadFunctions lf;

            switch (definitions.FhirSequence)
            {
                case FhirReleases.FhirSequenceCodes.DSTU2:
                    {
                        lf = _loadFunctionsR2;
                        if (_converter_20_50 == null)
                        {
                            lock (_convertLockObject)
                            {
                                if (_converter_20_50 == null)
                                {
                                    _converter_20_50 = new();
                                }
                            }
                        }
                    }
                    break;

                case FhirReleases.FhirSequenceCodes.STU3:
                    {
                        lf = _loadFunctionsR3;
                        if (_converter_30_50 == null)
                        {
                            lock (_convertLockObject)
                            {
                                if (_converter_30_50 == null)
                                {
                                    _converter_30_50 = new();
                                }
                            }
                        }
                    }
                    break;

                case FhirReleases.FhirSequenceCodes.R4:
                case FhirReleases.FhirSequenceCodes.R4B:
                    {
                        lf = _loadFunctionsR4B;
                        if (_converter_43_50 == null)
                        {
                            lock (_convertLockObject)
                            {
                                if (_converter_43_50 == null)
                                {
                                    _converter_43_50 = new();
                                }
                            }
                        }
                    }
                    break;

                default:
                case FhirReleases.FhirSequenceCodes.R5:
                    {
                        lf = _loadFunctionsR5;
                    }
                    break;
            }

            PackageContents? packageContents = _cache.GetIndexedContents(cachedPackage);

            if (packageContents == null)
            {
                throw new Exception("Failed to load package contents");
            }

            if (string.IsNullOrEmpty(cachedPackage.Directory))
            {
                throw new Exception("Package directory is empty");
            }

            if (!packageContents.Files.Any())
            {
                throw new Exception("Package contents are empty");
            }

            definitions.ContentListings.Add(cachedPackage.ResolvedDirective, packageContents);

            // build a dictionary of the contents based on resource type
            Dictionary<string, List<PackageContents.PackageFile>> filesByType =
                packageContents.Files.GroupBy(f => f.ResourceType).ToDictionary(g => g.Key, g => g.ToList());

            // first iterate over our sorted resource types to load them
            foreach (string rt in _sortedLoadOrder)
            {
                if ((!filesByType.TryGetValue(rt, out List<PackageContents.PackageFile>? files)) ||
                    (!files.Any()))
                {
                    continue;
                }

                foreach (PackageContents.PackageFile pFile in files)
                {
                    // load the file
                    string path = Path.Combine(cachedPackage.Directory, "package", pFile.FileName);

                    if (!File.Exists(path))
                    {
                        throw new Exception($"Listed file {cachedPackage.ResolvedDirective}:{pFile.FileName} does not exist");
                    }

                    string fileExtension = Path.GetExtension(pFile.FileName);
                    //string contents = await File.ReadAllTextAsync(path);

                    switch (rt)
                    {
                        case "CodeSystem":
                            {
                                CodeSystem? r = await lf.ParseCodeSystem(fileExtension, path);
                                if (r == null)
                                {
                                    throw new Exception($"Failed to parse CodeSystem file {cachedPackage.ResolvedDirective}:{pFile.FileName}");
                                }
                                definitions.AddCodeSystem(r);
                            }
                            break;

                        case "ValueSet":
                            {
                                ValueSet? r = await lf.ParseValueSet(fileExtension, path);
                                if (r == null)
                                {
                                    throw new Exception($"Failed to parse ValueSet file {cachedPackage.ResolvedDirective}:{pFile.FileName}");
                                }
                                definitions.AddValueSet(r);
                            }
                            break;

                        case "StructureDefinition":
                            {
                                StructureDefinition? r = await lf.ParseStructureDef(fileExtension, path);
                                if (r == null)
                                {
                                    throw new Exception($"Failed to parse StructureDefinition file {cachedPackage.ResolvedDirective}:{pFile.FileName}");
                                }

                                switch (r.cgArtifactClass())
                                {
                                    case FhirArtifactClassEnum.PrimitiveType:
                                        definitions.AddPrimitiveType(r, packageFhirVersion);
                                        break;

                                    case FhirArtifactClassEnum.LogicalModel:
                                        definitions.AddLogicalModel(r, packageFhirVersion);
                                        break;

                                    case FhirArtifactClassEnum.Extension:
                                        definitions.AddExtension(r, packageFhirVersion);
                                        break;

                                    case FhirArtifactClassEnum.Profile:
                                        definitions.AddProfile(r, packageFhirVersion);
                                        break;

                                    case FhirArtifactClassEnum.ComplexType:
                                        definitions.AddComplexType(r, packageFhirVersion);
                                        break;

                                    case FhirArtifactClassEnum.Resource:
                                        definitions.AddResource(r, packageFhirVersion);
                                        break;
                                }
                            }
                            break;
                        case "SearchParameter":
                            {
                                SearchParameter? r = await lf.ParseSearchParam(fileExtension, path);
                                if (r == null)
                                {
                                    throw new Exception($"Failed to parse SearchParameter file {cachedPackage.ResolvedDirective}:{pFile.FileName}");
                                }
                                definitions.AddSearchParameter(r);
                            }
                            break;

                        case "OperationDefinition":
                            {
                                OperationDefinition? r = await lf.ParseOperationDef(fileExtension, path);
                                if (r == null)
                                {
                                    throw new Exception($"Failed to parse OperationDefinition file {cachedPackage.ResolvedDirective}:{pFile.FileName}");
                                }

                                definitions.AddOperation(r);
                            }
                            break;

                        case "Conformance":
                        case "CapabilityStatement":
                            {
                                CapabilityStatement? r = await lf.ParseCapabilityStatement(fileExtension, path);
                                if (r == null)
                                {
                                    throw new Exception($"Failed to parse CapabilityStatement file {cachedPackage.ResolvedDirective}:{pFile.FileName}");
                                }
                                definitions.AddCapabilityStatement(r, manifest);
                            }
                            break;

                        case "ImplementationGuide":
                            {
                                ImplementationGuide? r = await lf.ParseImplementationGuide(fileExtension, path);
                                if (r == null)
                                {
                                    throw new Exception($"Failed to parse ImplementationGuide file {cachedPackage.ResolvedDirective}:{pFile.FileName}");
                                }
                                definitions.AddImplementationGuide(r);
                            }
                            break;

                        case "CompartmentDefinition":
                            {
                                CompartmentDefinition? r = await lf.ParseCompartmentDef(fileExtension, path);
                                if (r == null)
                                {
                                    throw new Exception($"Failed to parse CompartmentDefinition file {cachedPackage.ResolvedDirective}:{pFile.FileName}");
                                }
                                definitions.AddCompartment(r);
                            }
                            break;
                    }
                }
            }

            // check to see if this package is a 'core' FHIR package to add missing contents
            if (manifest.Type.Equals("core", StringComparison.OrdinalIgnoreCase) ||
                manifest.Type.Equals("fhir.core", StringComparison.OrdinalIgnoreCase))
            {
                AddMissingCoreSearchParameters(definitions);
                AddAllInteractionParameters(definitions);
                AddSearchResultParameters(definitions);
            }
        }

        // clean up
        if (_converter_43_50 != null)
        {
            lock (_convertLockObject)
            {
                _converter_43_50 = null;
            }
        }

        if (_converter_30_50 != null)
        {
            lock (_convertLockObject)
            {
                _converter_30_50 = null;
            }
        }

        if (_converter_20_50 != null)
        {
            lock (_convertLockObject)
            {
                _converter_20_50 = null;
            }
        }

        return definitions;
    }

    /// <summary>Parse contents.</summary>
    /// <typeparam name="TResource">Type of the resource.</typeparam>
    /// <param name="format">Describes the format to use.</param>
    /// <param name="path">  Full pathname of the file.</param>
    /// <returns>A TResource?</returns>
    public async Task<TResource?> ParseContentsPoco<TResource>(string format, string path) where TResource : Resource, new()
    {
        string content = await File.ReadAllTextAsync(path);

        switch (format.ToLowerInvariant())
        {
            case ".json":
            case "json":
            case "fhir+json":
            case "application/json":
            case "application/fhir+json":
                try
                {
                    // always use lenient parsing
                    Resource parsed = _jsonParser.DeserializeResource(content);
                    if (parsed is TResource)
                    {
                        return (TResource)parsed;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException == null)
                    {
                        Console.WriteLine($"Error parsing JSON: {ex.Message}");
                    }
                    else
                    {
                        Console.WriteLine($"Error parsing JSON: {ex.Message} ({ex.InnerException.Message})");
                    }
                    return null;
                }

            case ".xml":
            case "xml":
            case "fhir+xml":
            case "application/xml":
            case "application/fhir+xml":
                try
                {
                    // always use lenient parsing
                    Resource parsed = _xmlParser.DeserializeResource(content);
                    if (parsed is TResource)
                    {
                        return (TResource)parsed;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException == null)
                    {
                        Console.WriteLine($"Error parsing XML: {ex.Message}");
                    }
                    else
                    {
                        Console.WriteLine($"Error parsing XML: {ex.Message} ({ex.InnerException.Message})");
                    }
                    return null;
                }

            default:
                {
                    Console.WriteLine($"Unsupported parse format: {format}");
                    return null;
                }
        }
    }

    /// <summary>Parse contents of in the FHIR R4B format.</summary>
    /// <typeparam name="TResource">Type of the resource.</typeparam>
    /// <param name="format">Describes the format to use.</param>
    /// <param name="path">  Full pathname of the file.</param>
    /// <returns>An asynchronous result that yields a TResource?</returns>
    public async Task<TResource?> ParseContents43<TResource>(string format, string path) where TResource : Resource, new()
    {
        switch (format.ToLowerInvariant())
        {
            case ".json":
            case "json":
            case "fhir+json":
            case "application/json":
            case "application/fhir+json":
                try
                {
                    Hl7.Fhir.ElementModel.ISourceNode sn = FhirJsonNode.Parse(await File.ReadAllTextAsync(path));
                    return _converter_43_50!.Convert(sn) as TResource;
                }
                catch (Exception ex)
                {
                    if (ex.InnerException == null)
                    {
                        Console.WriteLine($"Error parsing R4B JSON: {ex.Message}");
                    }
                    else
                    {
                        Console.WriteLine($"Error parsing R4B JSON: {ex.Message} ({ex.InnerException.Message})");
                    }
                    return null;
                }

            case ".xml":
            case "xml":
            case "fhir+xml":
            case "application/xml":
            case "application/fhir+xml":
                try
                {
                    Hl7.Fhir.ElementModel.ISourceNode sn = FhirXmlNode.Parse(await File.ReadAllTextAsync(path));
                    return _converter_43_50!.Convert(sn) as TResource;
                }
                catch (Exception ex)
                {
                    if (ex.InnerException == null)
                    {
                        Console.WriteLine($"Error parsing R4B XML: {ex.Message}");
                    }
                    else
                    {
                        Console.WriteLine($"Error parsing R4B XML: {ex.Message} ({ex.InnerException.Message})");
                    }
                    return null;
                }

            default:
                {
                    Console.WriteLine($"Unsupported parse format: {format}");
                    return null;
                }
        }
    }

    public async Task<TResource?> ParseContents30<TResource>(string format, string path) where TResource : Resource, new()
    {
        switch (format.ToLowerInvariant())
        {
            case ".json":
            case "json":
            case "fhir+json":
            case "application/json":
            case "application/fhir+json":
                try
                {
                    Hl7.Fhir.ElementModel.ISourceNode sn = FhirJsonNode.Parse(await File.ReadAllTextAsync(path));
                    return _converter_30_50!.Convert(sn) as TResource;
                }
                catch (Exception ex)
                {
                    if (ex.InnerException == null)
                    {
                        Console.WriteLine($"Error parsing STU3 JSON: {ex.Message}");
                    }
                    else
                    {
                        Console.WriteLine($"Error parsing STU3 JSON: {ex.Message} ({ex.InnerException.Message})");
                    }
                    return null;
                }

            case ".xml":
            case "xml":
            case "fhir+xml":
            case "application/xml":
            case "application/fhir+xml":
                try
                {
                    Hl7.Fhir.ElementModel.ISourceNode sn = FhirXmlNode.Parse(await File.ReadAllTextAsync(path));
                    return _converter_30_50!.Convert(sn) as TResource;
                }
                catch (Exception ex)
                {
                    if (ex.InnerException == null)
                    {
                        Console.WriteLine($"Error parsing STU3 XML: {ex.Message}");
                    }
                    else
                    {
                        Console.WriteLine($"Error parsing STU3 XML: {ex.Message} ({ex.InnerException.Message})");
                    }
                    return null;
                }

            default:
                {
                    Console.WriteLine($"Unsupported parse format: {format}");
                    return null;
                }
        }
    }

    public async Task<TResource?> ParseContents20<TResource>(string format, string path) where TResource : Resource, new()
    {
        switch (format.ToLowerInvariant())
        {
            case ".json":
            case "json":
            case "fhir+json":
            case "application/json":
            case "application/fhir+json":
                try
                {
                    Hl7.Fhir.ElementModel.ISourceNode sn = FhirJsonNode.Parse(await File.ReadAllTextAsync(path));
                    return _converter_20_50!.Convert(sn) as TResource;
                }
                catch (Exception ex)
                {
                    if (ex.InnerException == null)
                    {
                        Console.WriteLine($"Error parsing DSTU2 JSON: {ex.Message}");
                    }
                    else
                    {
                        Console.WriteLine($"Error parsing DSTU2 JSON: {ex.Message} ({ex.InnerException.Message})");
                    }
                    return null;
                }

            case ".xml":
            case "xml":
            case "fhir+xml":
            case "application/xml":
            case "application/fhir+xml":
                try
                {
                    Hl7.Fhir.ElementModel.ISourceNode sn = FhirXmlNode.Parse(await File.ReadAllTextAsync(path));
                    return _converter_20_50!.Convert(sn) as TResource;
                }
                catch (Exception ex)
                {
                    if (ex.InnerException == null)
                    {
                        Console.WriteLine($"Error parsing DSTU2 XML: {ex.Message}");
                    }
                    else
                    {
                        Console.WriteLine($"Error parsing DSTU2 XML: {ex.Message} ({ex.InnerException.Message})");
                    }
                    return null;
                }

            default:
                {
                    Console.WriteLine($"Unsupported parse format: {format}");
                    return null;
                }
        }
    }

    /// <summary>Parse the contents of a resource in the specified format.</summary>
    /// <typeparam name="TResource">The type of the resource.</typeparam>
    /// <param name="format">The format of the content.</param>
    /// <param name="path">  Full pathname of the file.</param>
    /// <returns>
    /// The parsed resource of type <typeparamref name="TResource"/> or null if parsing fails.
    /// </returns>
    public async Task<TResource?> ParseContentsSystemTextStream<TResource>(string format, string path) where TResource : Resource, new()
    {
        switch (format.ToLowerInvariant())
        {
            case ".json":
            case "json":
            case "fhir+json":
            case "application/json":
            case "application/fhir+json":
                try
                {
                    using (FileStream fs = new(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        return JsonSerializer.Deserialize<TResource>(fs, _jsonOptions);
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException == null)
                    {
                        Console.WriteLine($"Error parsing JSON: {ex.Message}");
                    }
                    else
                    {
                        Console.WriteLine($"Error parsing JSON: {ex.Message} ({ex.InnerException.Message})");
                    }
                    return null;
                }

            case ".xml":
            case "xml":
            case "fhir+xml":
            case "application/xml":
            case "application/fhir+xml":
                try
                {
                    string content = await File.ReadAllTextAsync(path);

                    // always use lenient parsing
                    Resource parsed = _xmlParser.DeserializeResource(content);
                    if (parsed is TResource)
                    {
                        return (TResource)parsed;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException == null)
                    {
                        Console.WriteLine($"Error parsing XML: {ex.Message}");
                    }
                    else
                    {
                        Console.WriteLine($"Error parsing XML: {ex.Message} ({ex.InnerException.Message})");
                    }
                    return null;
                }

            default:
                {
                    Console.WriteLine($"Unsupported parse format: {format}");
                    return null;
                }
        }
    }

    /// <summary>Error handler as mitigation for https://github.com/FirelyTeam/firely-net-sdk/issues/2701. </summary>
    /// <param name="reader">           [in,out] The reader.</param>
    /// <param name="targetType">       Type of the target.</param>
    /// <param name="originalValue">    The original value.</param>
    /// <param name="originalException">Details of the exception.</param>
    /// <returns>A Tuple.</returns>
    public static (object?, FhirJsonException?) LocalPrimitiveParseHandler(
        ref Utf8JsonReader reader,
        Type targetType,
        object? originalValue,
        FhirJsonException originalException)
    {
        if (targetType == typeof(long))
        {
            if (originalValue is long ol)
            {
                return (ol, null);
            }

            if (originalValue is string s)
            {
                if (long.TryParse(s, out long l))
                {
                    return (l, null);
                }
            }
        }

        return (originalValue, originalException);
    }


}
