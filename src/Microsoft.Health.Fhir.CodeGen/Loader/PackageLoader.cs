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
        "ImplementationGuide",
        "CompartmentDefinition",
    };

    /// <summary>(Immutable) List of types of the sorted resources.</summary>
    private static readonly HashSet<string> _sortedResourceTypes = new HashSet<string>(_sortedLoadOrder);

    /// <summary>The lenient JSON parser.</summary>
    private static FhirJsonPocoDeserializer _jsonParser = new(new FhirJsonPocoDeserializerSettings()
    {
        DisableBase64Decoding = false,
        Validator = null,
        OnPrimitiveParseFailed = LocalPrimitiveParseHandler,        // TODO(ginoc): remove after https://github.com/FirelyTeam/firely-net-sdk/issues/2701 is fixed
    });

    /// <summary>The lenient XML parser.</summary>
    private static FhirXmlPocoDeserializer _xmlParser = new(new FhirXmlPocoDeserializerSettings()
    {
        DisableBase64Decoding = false,
        Validator = null,
    });


    /// <summary>
    /// Initializes a new instance of the <see cref="PackageLoader"/> class.
    /// </summary>
    /// <param name="cache">The cache.</param>
    public PackageLoader(IFhirPackageClient cache)
    {
        _cache = cache;
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
            if ((definitions.FhirVersion == null) && manifest.FhirVersions.Any())
            {
                definitions.FhirSequence = FhirReleases.FhirVersionToSequence(manifest.FhirVersions.First());

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
                        throw new Exception($"Listed file {pFile.FileName} does not exist");
                    }

                    string fileExtension = Path.GetExtension(pFile.FileName);
                    string contents = await File.ReadAllTextAsync(path);

                    switch (rt)
                    {
                        case "CodeSystem":
                            {
                                CodeSystem? r = ParseContents<CodeSystem>(fileExtension, contents);
                                if (r == null)
                                {
                                    throw new Exception($"Failed to parse CodeSystem file {pFile.FileName}");
                                }
                                definitions.AddCodeSystem(r);
                            }
                            break;

                        case "ValueSet":
                            {
                                ValueSet? r = ParseContents<ValueSet>(fileExtension, contents);
                                if (r == null)
                                {
                                    throw new Exception($"Failed to parse ValueSet file {pFile.FileName}");
                                }
                                definitions.AddValueSet(r);
                            }
                            break;

                        case "StructureDefinition":
                            {
                                StructureDefinition? r = ParseContents<StructureDefinition>(fileExtension, contents);
                                if (r == null)
                                {
                                    throw new Exception($"Failed to parse StructureDefinition file {pFile.FileName}");
                                }

                                switch (r.cgArtifactClass())
                                {
                                    case FhirArtifactClassEnum.PrimitiveType:
                                        definitions.AddPrimitiveType(r);
                                        break;

                                    case FhirArtifactClassEnum.LogicalModel:
                                        definitions.AddLogicalModel(r);
                                        break;

                                    case FhirArtifactClassEnum.Extension:
                                        definitions.AddExtension(r);
                                        break;

                                    case FhirArtifactClassEnum.Profile:
                                        definitions.AddProfile(r);
                                        break;

                                    case FhirArtifactClassEnum.ComplexType:
                                        definitions.AddComplexType(r);
                                        break;

                                    case FhirArtifactClassEnum.Resource:
                                        definitions.AddResource(r);
                                        break;
                                }
                            }
                            break;
                        case "SearchParameter":
                            {
                                SearchParameter? r = ParseContents<SearchParameter>(fileExtension, contents);
                                if (r == null)
                                {
                                    throw new Exception($"Failed to parse SearchParameter file {pFile.FileName}");
                                }
                                definitions.AddSearchParameter(r);
                            }
                            break;

                        case "OperationDefinition":
                            {
                                OperationDefinition? r = ParseContents<OperationDefinition>(fileExtension, contents);
                                if (r == null)
                                {
                                    throw new Exception($"Failed to parse OperationDefinition file {pFile.FileName}");
                                }

                                definitions.AddOperation(r);
                            }
                            break;

                        case "CapabilityStatement":
                            {
                                CapabilityStatement? r = ParseContents<CapabilityStatement>(fileExtension, contents);
                                if (r == null)
                                {
                                    throw new Exception($"Failed to parse CapabilityStatement file {pFile.FileName}");
                                }
                                definitions.AddCapabilityStatement(r);
                            }
                            break;

                        case "ImplementationGuide":
                            {
                                ImplementationGuide? r = ParseContents<ImplementationGuide>(fileExtension, contents);
                                if (r == null)
                                {
                                    throw new Exception($"Failed to parse ImplementationGuide file {pFile.FileName}");
                                }
                                definitions.AddImplementationGuide(r);
                            }
                            break;

                        case "CompartmentDefinition":
                            {
                                CompartmentDefinition? r = ParseContents<CompartmentDefinition>(fileExtension, contents);
                                if (r == null)
                                {
                                    throw new Exception($"Failed to parse CompartmentDefinition file {pFile.FileName}");
                                }
                                definitions.AddCompartment(r);
                            }
                            break;
                    }
                }
            }

            // check to see if this package is a 'core' FHIR package to add missing contents
            if (manifest.Type.Equals("core", StringComparison.OrdinalIgnoreCase))
            {
                AddMissingCoreSearchParameters(definitions);
                AddAllInteractionParameters(definitions);
                AddSearchResultParameters(definitions);
            }
        }

        return definitions;
    }

    /// <summary>Parse contents.</summary>
    /// <typeparam name="TResource">Type of the resource.</typeparam>
    /// <param name="format"> Describes the format to use.</param>
    /// <param name="content">The content.</param>
    /// <returns>A TResource?</returns>
    public TResource? ParseContents<TResource>(string format, string content) where TResource : Resource, new()
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

    /// <summary>Error handler as mitigation for https://github.com/FirelyTeam/firely-net-sdk/issues/2701. </summary>
    /// <param name="reader">           [in,out] The reader.</param>
    /// <param name="targetType">       Type of the target.</param>
    /// <param name="originalValue">    The original value.</param>
    /// <param name="originalException">Details of the exception.</param>
    /// <returns>A Tuple.</returns>
    private static (object?, FhirJsonException?) LocalPrimitiveParseHandler(
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
