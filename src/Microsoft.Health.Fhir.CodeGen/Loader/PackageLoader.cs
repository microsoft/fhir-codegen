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
using Hl7.Fhir.Serialization;
using System.Text.Json;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;
using Microsoft.Health.Fhir.CodeGenCommon.Models;
using Firely.Fhir.Packages;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;
using System.Xml.Linq;
using Microsoft.Health.Fhir.CodeGen.Configuration;
using System.Linq;
using Microsoft.Health.Fhir.CodeGen._ForPackages;
using Microsoft.Health.Fhir.CodeGen.Utils;

namespace Microsoft.Health.Fhir.CodeGen.Loader;

/// <summary>A package loader.</summary>
public class PackageLoader : IDisposable
{
    internal enum VersionHandlingTypes
    {
        /// <summary>Unprocessed / unknown / SemVer / ranges / etc (pass through).</summary>
        Passthrough,

        /// <summary>Latest release.</summary>
        Latest,

        /// <summary>Local build.</summary>
        Local,

        /// <summary>CI Build.</summary>
        ContinuousIntegration,
    }

    /// <summary>(Immutable) The cache.</summary>
    private readonly _ForPackages.DiskPackageCache _cache;

    /// <summary>(Immutable) The package clients.</summary>
    private readonly List<IPackageServer> _packageClients = [];

    private bool _disposedValue = false;

    /// <summary>(Immutable) The sorted order for definition types we want to load.</summary>
    private static readonly string[] _sortedLoadOrder =
    [
        "CodeSystem",
        "ValueSet",
        "StructureDefinition",
        "SearchParameter",
        "OperationDefinition",
        "CapabilityStatement",
        "Conformance",
        "ImplementationGuide",
        "CompartmentDefinition",
    ];

    private ConfigRoot _rootConfiguration;

    private FhirReleases.FhirSequenceCodes _defaultFhirVersion;

    /// <summary>The JSON model.</summary>
    private LoaderOptions.JsonDeserializationModel _jsonModel;

    /// <summary>Options for controlling the JSON.</summary>
    private JsonSerializerOptions _jsonOptions;

    /// <summary>The lenient JSON parser.</summary>
    private FhirJsonPocoDeserializer _jsonParser;

#if !DISABLE_XML
    /// <summary>The lenient XML parser.</summary>
    private FhirXmlPocoDeserializer _xmlParser;
#endif

    private Microsoft.Health.Fhir.CrossVersion.Converter_43_50? _converter_43_50 = null;
    private Microsoft.Health.Fhir.CrossVersion.Converter_30_50? _converter_30_50 = null;
    private Microsoft.Health.Fhir.CrossVersion.Converter_20_50? _converter_20_50 = null;

    private object _convertLockObject = new();

    /// <summary>Initializes a new instance of the <see cref="PackageLoader"/> class.</summary>
    /// <param name="opts">    (Optional) Options for controlling the operation.</param>
    public PackageLoader(ConfigRoot? config = null, LoaderOptions? opts = null)
    {
        // use defaults if nothing was specified
        opts ??= new();

        _rootConfiguration = config ?? new();

        _cache = new(_rootConfiguration.FhirCacheDirectory);

        // check if we are using the official registries
        if (_rootConfiguration.UseOfficialRegistries == true)
        {
            _packageClients.Add(PackageClient.Create("https://packages.fhir.org"));
            _packageClients.Add(PackageClient.Create("https://packages2.fhir.org/packages"));
            _packageClients.Add(new _ForPackages.FhirCiClient(-1));
        }

        if (_rootConfiguration.AdditionalFhirRegistryUrls.Any())
        {
            foreach (string url in _rootConfiguration.AdditionalFhirRegistryUrls)
            {
                _packageClients.Add(PackageClient.Create(url, npm: false));
            }
        }

        if (_rootConfiguration.AdditionalNpmRegistryUrls.Any())
        {
            foreach (string url in _rootConfiguration.AdditionalNpmRegistryUrls)
            {
                _packageClients.Add(PackageClient.Create(url, npm: true));
            }
        }

        _defaultFhirVersion = FhirReleases.FhirVersionToSequence(_rootConfiguration.FhirVersion);

        _jsonOptions = opts.FhirJsonOptions;
        _jsonParser = new(opts.FhirJsonSettings);

#if !DISABLE_XML
        _xmlParser = new(opts.FhirXmlSettings);
#endif
        _jsonModel = opts.JsonModel;
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
            AllowedValues = ["true", "false", "count", "data", "text",],
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
                AllowedValues = [ "true", "false", ],
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
            AllowedValues = [ "true", "false", "both", ],
        });

        dc.AddSearchResultParameter(new()
        {
            Name = "_containedType",
            Url = "http://hl7.org/fhir/search.html#containedType",
            Description = "When contained resources are being returned, whether the server should return either the container or the contained resource.",
            ParamType = SearchParamType.Token,
            AllowedValues = [ "container", "contained", ],
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
                AllowedValues = [ "none", "estimate", "accurate" ],
            });
        }
    }

    /// <summary>Adds a missing core search parameters to a core definition collection.</summary>
    /// <param name="dc">The device-context.</param>
    private void AddMissingCoreSearchParameters(DefinitionCollection dc, string packageId, string packageVersion)
    {
        dc.AddSearchParameter(new()
        {
            Id = "Resource-content",
            Name = "_content",
            Code = "_content",
            Url = "http://hl7.org/fhir/SearchParameter/Resource-content",
            Version = dc.FhirSequence.ToLongVersion(),
            Title = "Resource content filter",
            Status = PublicationStatus.Active,
            Description = "Search on the entire content of the resource.",
            Base = [ VersionIndependentResourceTypesAll.Resource ],
            Type = SearchParamType.Special,
        }, packageId, packageVersion, true);

        dc.AddSearchParameter(new()
        {
            Id = "Resource-filter",
            Name = "_filter",
            Code = "_filter",
            Url = "http://hl7.org/fhir/SearchParameter/Resource-filter",
            Version = dc.FhirSequence.ToLongVersion(),
            Title = "Advanced search filter",
            Status = PublicationStatus.Active,
            Description = "Filter search parameter which supports a more sophisticated grammar for searching.",
            Base = [ VersionIndependentResourceTypesAll.Resource ],
            Type = SearchParamType.Special,
        }, packageId, packageVersion, true);

        dc.AddSearchParameter(new()
        {
            Id = "Resource-text",
            Name = "_text",
            Code = "_text",
            Url = "http://hl7.org/fhir/SearchParameter/Resource-text",
            Version = dc.FhirSequence.ToLongVersion(),
            Title = "Resource text filter",
            Status = PublicationStatus.Active,
            Description = "Search the narrative content of a resource.",
            Base = [ VersionIndependentResourceTypesAll.Resource ],
            Type = SearchParamType.String,
        }, packageId, packageVersion, true);

        dc.AddSearchParameter(new()
        {
            Id = "Resource-list",
            Name = "_list",
            Code = "_list",
            Url = "http://hl7.org/fhir/SearchParameter/Resource-list",
            Version = dc.FhirSequence.ToLongVersion(),
            Title = "List reference filter",
            Status = PublicationStatus.Active,
            Description = "Filter based on resources referenced by a List resource.",
            Base = [ VersionIndependentResourceTypesAll.Resource ],
            Type = SearchParamType.Reference,
            Target = [ VersionIndependentResourceTypesAll.List ],
        }, packageId, packageVersion, true);

        if (dc.FhirSequence >= FhirReleases.FhirSequenceCodes.R4)
        {
            dc.AddSearchParameter(new()
            {
                Id = "Resource-has",
                Name = "_has",
                Code = "_has",
                Url = "http://hl7.org/fhir/SearchParameter/Resource-has",
                Version = dc.FhirSequence.ToLongVersion(),
                Title = "Limited support for reverse chaining",
                Status = PublicationStatus.Active,
                Description = "For selecting resources based on the properties of resources that refer to them.",
                Base = [ VersionIndependentResourceTypesAll.Resource ],
                Type = SearchParamType.Special,
            }, packageId, packageVersion, true);

            dc.AddSearchParameter(new()
            {
                Id = "Resource-type",
                Name = "_type",
                Code = "_type",
                Url = "http://hl7.org/fhir/SearchParameter/Resource-type",
                Version = dc.FhirSequence.ToLongVersion(),
                Title = "Resource type filter",
                Status = PublicationStatus.Active,
                Description = "For filtering resources based on their type in searches across resource types.",
                Base = [ VersionIndependentResourceTypesAll.Resource ],
                Type = SearchParamType.Token,
            }, packageId, packageVersion, true);
        }
    }

    private VersionHandlingTypes GetVersionHandlingType(string? version)
    {
        // handle simple literals
        switch (version)
        {
            case null:
            case "":
            case "latest":
                return VersionHandlingTypes.Latest;

            case "current":
                return VersionHandlingTypes.ContinuousIntegration;

            case "dev":
                return VersionHandlingTypes.Local;
        }

        // check for local or current with branch names
        if (version.StartsWith("current$", StringComparison.Ordinal))
        {
            return VersionHandlingTypes.ContinuousIntegration;
        }

        if (version.StartsWith("dev$", StringComparison.Ordinal))
        {
            return VersionHandlingTypes.Local;
        }

        return VersionHandlingTypes.Passthrough;
    }

    private async ValueTask<(PackageReference, IPackageServer?)> ResolveLatest(string name)
    {
        List<(PackageReference pr, IPackageServer server)> latestRecs = new();

        foreach (IPackageServer server in _packageClients)
        {
            PackageReference pr = await server.GetLatest(name);
            if (pr == PackageReference.None)
            {
                continue;
            }
            latestRecs.Add((pr, server));
        }

        if (latestRecs.Count == 0)
        {
            return (PackageReference.None, null);
        }

        return latestRecs.OrderByDescending(v => v.pr.Version).First();
    }

    /// <summary>
    /// Installs a package with the specified package reference.
    /// </summary>
    /// <param name="packageReference">The package reference.</param>
    /// <param name="retries">The number of retries.</param>
    /// <returns><c>true</c> if the package is installed successfully; otherwise, <c>false</c>.</returns>
    private async Task<bool> InstallPackage(PackageReference packageReference, int retries = 5)
    {
        Random r = new();

        for (int i = 0; i < retries; i++)
        {
            foreach (IPackageServer pc in _packageClients)
            {
                try
                {
                    if (packageReference.Scope == FhirCiClient.FhirCiScope)
                    {
                        if (pc is not FhirCiClient ciClient)
                        {
                            // cannot install CI packages from non-CI clients
                            continue;
                        }

                        await ciClient.InstallOrUpdate(packageReference, _cache);
                        return true;
                    }

                    // try to download this package
                    byte[] data = await pc.GetPackage(packageReference);

                    // try to install this package
                    await _cache.Install(packageReference, data);

                    // only need to install from first hit
                    return true;
                }
                catch (Exception)
                {
                    // ignore
                }
            }

            await System.Threading.Tasks.Task.Delay(500 + (int)(r.NextDouble() * 1000));
        }

        return false;
    }

    private void CreateConverterIfRequired(FhirReleases.FhirSequenceCodes fhirSequence)
    {
        // create the converter we need
        switch (fhirSequence)
        {
            case FhirReleases.FhirSequenceCodes.DSTU2:
                {
                    if (_converter_20_50 == null)
                    {
                        lock (_convertLockObject)
                        {
                            _converter_20_50 ??= new();
                        }
                    }
                }
                break;

            case FhirReleases.FhirSequenceCodes.STU3:
                {
                    if (_converter_30_50 == null)
                    {
                        lock (_convertLockObject)
                        {
                            _converter_30_50 ??= new();
                        }
                    }
                }
                break;

            case FhirReleases.FhirSequenceCodes.R4:
            case FhirReleases.FhirSequenceCodes.R4B:
                {
                    if (_converter_43_50 == null)
                    {
                        lock (_convertLockObject)
                        {
                            _converter_43_50 ??= new();
                        }
                    }
                }
                break;

            default:
            case FhirReleases.FhirSequenceCodes.R5:
                {
                }
                break;
        }
    }

    private bool LoadFromDirectory(ref DefinitionCollection? definitions, string sourcePath, string? fhirVersion)
    {
        FhirReleases.FhirSequenceCodes fhirSequence = string.IsNullOrEmpty(fhirVersion)
            ? _defaultFhirVersion
            : FhirReleases.FhirVersionToSequence(fhirVersion!);

        if (fhirSequence == FhirReleases.FhirSequenceCodes.Unknown)
        {
            throw new Exception("Cannot load from a directory with an unknown FHIR version");
        }

        CreateConverterIfRequired(fhirSequence);

        string? name = Path.GetFileName(sourcePath);

        if (name == null)
        {
            throw new Exception($"Failed to get directory name from '{sourcePath}'");
        }

        string canonical = $"file://{sourcePath}";

        definitions ??= new()
        {
            Name = name,
            FhirSequence = fhirSequence,
            FhirVersionLiteral = fhirSequence.ToLiteral(),
            MainPackageId = name,
            MainPackageVersion = "dev",
            MainPackageCanonical = canonical,
        };

        // get files in the directory
        string[] files = Directory.GetFiles(sourcePath, "*.json", SearchOption.TopDirectoryOnly);
        foreach (string path in files)
        {
            string fileExtension = Path.GetExtension(path);

            object? r;

            switch (fhirSequence)
            {
                case FhirReleases.FhirSequenceCodes.DSTU2:
                    {
                        r = ParseContents20(fileExtension, path: path);
                    }
                    break;

                case FhirReleases.FhirSequenceCodes.STU3:
                    {
                        r = ParseContents30(fileExtension, path: path);
                    }
                    break;

                case FhirReleases.FhirSequenceCodes.R4:
                case FhirReleases.FhirSequenceCodes.R4B:
                    {
                        r = ParseContents43(fileExtension, path: path);
                    }
                    break;

                default:
                case FhirReleases.FhirSequenceCodes.R5:
                    {
                        r = ParseContentsPoco(fileExtension, path);
                    }
                    break;
            }

            if (r == null)
            {
                throw new Exception($"Failed to parse '{path}'");
            }

            definitions.AddResource(r, _defaultFhirVersion, name, "dev", canonical);
        }

        return true;
    }

    /// <summary>Loads a package.</summary>
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    /// <param name="packages">   The cached package.</param>
    /// <param name="definitions">(Optional) The definitions.</param>
    /// <param name="fhirVersion">(Optional) The FHIR version.</param>
    /// <returns>An asynchronous result that yields the package.</returns>
    public async Task<DefinitionCollection?> LoadPackages(
        IEnumerable<string> packages,
        DefinitionCollection? definitions = null,
        string? fhirVersion = null)
    {
        string? requestedFhirVersion = fhirVersion;

        if (!packages.Any())
        {
            return null;
        }

        using InterProcessSync synchronizationObject = new();

        // we need to filter structures post parsing if we are not loading all known types
        bool filterStructureDefinitions = !_rootConfiguration.LoadStructures.Contains(FhirArtifactClassEnum.PrimitiveType) ||
                                          !_rootConfiguration.LoadStructures.Contains(FhirArtifactClassEnum.ComplexType) ||
                                          !_rootConfiguration.LoadStructures.Contains(FhirArtifactClassEnum.Resource) ||
                                          !_rootConfiguration.LoadStructures.Contains(FhirArtifactClassEnum.Extension) ||
                                          !_rootConfiguration.LoadStructures.Contains(FhirArtifactClassEnum.Profile) ||
                                          !_rootConfiguration.LoadStructures.Contains(FhirArtifactClassEnum.LogicalModel) ||
                                          !_rootConfiguration.LoadStructures.Contains(FhirArtifactClassEnum.Interface);

        foreach (string inputDirective in packages)
        {
            if (string.IsNullOrEmpty(inputDirective))
            {
                continue;
            }

            // check to see if this is actually a directory
            if ((inputDirective.IndexOfAny(Path.GetInvalidPathChars()) == -1) &&
                Directory.Exists(inputDirective))
            {
                Console.WriteLine($"Processing directory {inputDirective}...");

                if (!LoadFromDirectory(ref definitions, inputDirective, fhirVersion))
                {
                    throw new Exception($"Failed to load package from directory: {inputDirective}");
                }

                // if we loaded from the directory, just continue
                continue;
            }

            // TODO(ginoc): PR in to Parse FHIR-style directives, remove when added.
            string directive = inputDirective.Contains('@')
                ? inputDirective
                : inputDirective.Replace('#', '@');

            PackageReference packageReference = PackageReference.Parse(directive);

            if (packageReference.Name == null)
            {
                throw new Exception($"Failed to parse package reference: {directive}");
            }

            // check to see if this is a FHIR release that is no longer available
            if (FhirPackageUtils.PackageIsFhirRelease(packageReference.Name) &&
                FhirReleases.VersionIsUnavailable(packageReference.Version ?? string.Empty))
            {
                packageReference.Version = FhirReleases.GetCurrentPatch(packageReference.Version ?? string.Empty);

                if (definitions?.Manifests.ContainsKey(packageReference.Moniker) == true)
                {
                    // we have already loaded this package, just continue
                    continue;
                }
            }

            // create our definition collection shell if we do not have one
            if (definitions == null)
            {
                definitions = new()
                {
                    Name = packageReference.Name,
                };

                if (!string.IsNullOrEmpty(fhirVersion))
                {
                    definitions.FhirSequence = FhirReleases.FhirVersionToSequence(fhirVersion!);
                }
            }

            Guid? syncHandle = null;

            try
            {
                string sName = "fcg-" + packageReference.Moniker;

                (syncHandle, bool _) = await synchronizationObject.TryGetLock(sName);

                bool needsInstall = true;

                VersionHandlingTypes vht = GetVersionHandlingType(packageReference.Version);

                // do special handling for versions if necessary
                switch (vht)
                {
                    case VersionHandlingTypes.Latest:
                        {
                            // resolve the version via Firely Packages so that we have access to the actual version number
                            (PackageReference pr, IPackageServer? _) = await ResolveLatest(packageReference.Name);

                            if ((pr == PackageReference.None) || (pr.Name == null))
                            {
                                throw new Exception($"Failed to resolve latest version of {packageReference.Name} ({directive})");
                            }

                            packageReference = pr;
                            needsInstall = !(await _cache.IsInstalled(packageReference));
                        }
                        break;

                    case VersionHandlingTypes.Local:
                        // ensure there is a local build, there is no other source
                        {
                            if (!_cache.IsInstalled(packageReference).Result)
                            {
                                throw new Exception($"Local build of {packageReference.Name} is not installed ({directive})");
                            }
                        }
                        break;

                    case VersionHandlingTypes.ContinuousIntegration:
                        // always trigger install/update for CI builds
                        needsInstall = true;
                        break;

                    default:
                        needsInstall = !(await _cache.IsInstalled(packageReference));
                        break;
                }

                // check if we are flagged to load expansions and this is a core package
                if (_rootConfiguration.AutoLoadExpansions && FhirPackageUtils.PackageIsFhirCore(packageReference.Name))
                {
                    string expansionPackageName = packageReference.Name.Replace(".core", ".expansions");
                    string expansionDirective = expansionPackageName + "@" + packageReference.Version;

                    Console.WriteLine($"Auto-loading core expansions: {expansionDirective}...");

                    await LoadPackages([expansionDirective], definitions, requestedFhirVersion);
                }

                // skip if we have already loaded this package
                if (definitions.Manifests.ContainsKey(packageReference.Moniker))
                {
                    Console.WriteLine($"Skipping already loaded dependency: {packageReference.Moniker}");
                    continue;
                }

                Console.WriteLine($"Processing {packageReference.Moniker}...");

                // check to see if this package needs to be installed
                if (needsInstall &&
                    (await InstallPackage(packageReference) == false))
                {
                    // failed to install
                    throw new Exception($"Failed to install package {packageReference.Moniker} as requested by {inputDirective}");
                }
            }
            finally
            {
                if (syncHandle != null)
                {
                    synchronizationObject.ReleaseLock(syncHandle.Value);
                }
            }

            _ForPackages.PackageManifest manifest = await _cache.ReadManifestEx(packageReference) ?? throw new Exception("Failed to load package manifest");

            // check to see if we have a restricted FHIR version and need to filter
            if ((!string.IsNullOrEmpty(requestedFhirVersion)) &&
                (definitions.FhirSequence != FhirReleases.FhirSequenceCodes.Unknown) &&
                (manifest.AnyFhirVersions?.FirstOrDefault() is string manifestFhirVersion) &&
                !string.IsNullOrEmpty(manifestFhirVersion) &&
                (definitions.FhirSequence != FhirReleases.FhirVersionToSequence(manifestFhirVersion)))
            {
                Console.WriteLine($"Package {packageReference.Moniker} ({manifestFhirVersion}) does not match requested FHIR version {fhirVersion}!");

                string packageIdSuffix = packageReference.Name.Split('.')[^1];
                FhirReleases.FhirSequenceCodes packageIdSuffixCode = FhirReleases.FhirVersionToSequence(packageIdSuffix);

                // check for NOT having a version already specified
                if (packageIdSuffixCode == FhirReleases.FhirSequenceCodes.Unknown)
                {
                    string requiredRLiteral = definitions.FhirSequence.ToRLiteral().ToLowerInvariant();

                    string desiredMoniker = $"{packageReference.Name}.{requiredRLiteral}@{packageReference.Version}";

                    await LoadPackages([desiredMoniker], definitions, requestedFhirVersion);

                    if (definitions.Manifests.ContainsKey(desiredMoniker))
                    {
                        Console.WriteLine($"Package {desiredMoniker} loaded in place of {packageReference.Moniker}!");
                    }
                    else
                    {
                        Console.WriteLine($"Could not find substitute for {packageReference.Moniker} - please specify manually if this is required!");
                    }
                }
                else
                {
                    Console.WriteLine($"Package {packageReference.Moniker} specifies an incorrect FHIR version - please correct!");
                }

                // whether it loaded or not, we cannot do more this pass
                continue;
            }

            //// check to see if this is a different FHIR version from what we expect
            //if ((definitions.FhirSequence != FhirReleases.FhirSequenceCodes.Unknown) &&
            //    (manifest.GetFhirVersion() is string manifestFhirVersion) &&
            //    !string.IsNullOrEmpty(manifestFhirVersion) &&
            //    (definitions.FhirSequence != FhirReleases.FhirVersionToSequence(manifestFhirVersion)))
            //{
            //    Console.WriteLine($"Package {packageReference.Moniker} FHIR version mismatch: {manifest?.GetFhirVersion()} != {fhirVersion}, attempting to resolve...");

            //    string requiredRLiteral = definitions.FhirSequence.ToRLiteral().ToLowerInvariant();

            //    string desiredMoniker = $"{packageReference.Name}.{requiredRLiteral}@{packageReference.Version}";

            //    await LoadPackages([desiredMoniker], definitions);

            //    if (!definitions.Manifests.ContainsKey(desiredMoniker))
            //    {
            //        throw new Exception($"Package {packageReference.Moniker} FHIR version mismatch: {manifest?.GetFhirVersion()} != {fhirVersion} and could not be resolved!");
            //    }
            //}

            // flag we are tracking this package
            definitions.Manifests.Add(packageReference.Moniker, manifest);

            if (string.IsNullOrEmpty(definitions.MainPackageId) || (definitions.Name == manifest.Name))
            {
                definitions.MainPackageId = manifest.Name;
                definitions.MainPackageVersion = manifest.Version;
                definitions.MainPackageCanonical = manifest.Canonical ?? throw new Exception($"Main package {packageReference.Moniker} manifest does not contain a canonical URL");
            }

            string? packageFhirVersionLiteral = manifest.AnyFhirVersions?.FirstOrDefault();

            // update the collection FHIR version based on the first package we come across with one
            if (string.IsNullOrEmpty(definitions.FhirVersionLiteral) && (!string.IsNullOrEmpty(packageFhirVersionLiteral)))
            {
                definitions.FhirVersionLiteral = packageFhirVersionLiteral!;

                definitions.FhirSequence = FhirReleases.FhirVersionToSequence(packageFhirVersionLiteral!);

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

            FhirReleases.FhirSequenceCodes packageFhirVersion = string.IsNullOrEmpty(packageFhirVersionLiteral)
                ? definitions.FhirSequence
                : FhirReleases.FhirVersionToSequence(packageFhirVersionLiteral ?? string.Empty);

            CreateConverterIfRequired(definitions.FhirSequence);

            // if we are resolving dependencies, check them now
            if (_rootConfiguration.ResolvePackageDependencies && (manifest.Dependencies?.Any() ?? false))
            {
                await LoadPackages(manifest.Dependencies.Select(kvp => $"{kvp.Key}@{kvp.Value}"), definitions, requestedFhirVersion);

                Console.WriteLine($"Dependencies resolved - loading package {packageReference.Moniker}...");
            }
            else
            {
                Console.WriteLine($"Loading {packageReference.Moniker}...");
            }

            // grab the contents of our package
            CanonicalIndex? packageIndex = await _cache.GetCanonicalIndex(packageReference) ?? throw new Exception("Failed to load package contents");
            string packageDirectory = _cache.PackageContentFolder(packageReference);

            if (!Directory.Exists(packageDirectory))
            {
                throw new Exception($"Package directory {packageDirectory} does not exist!");
            }

            if (string.IsNullOrEmpty(packageDirectory))
            {
                throw new Exception("Package directory is empty");
            }

            if (!(packageIndex.Files?.Any() ?? false))
            {
                throw new Exception("Package contents are empty");
            }

            string packageRootDirectory = Path.Combine(packageDirectory, "..");

            definitions.ContentListings.Add(packageReference.Moniker, packageIndex);

            // create an dictionary of indexes we are going to load - note that we are essentially traversing twice, but that is better than projecting each time
            List<int>[] sortedFileIndexes = new List<int>[_sortedLoadOrder.Length];

            for (int i = 0; i < sortedFileIndexes.Length; i++)
            {
                sortedFileIndexes[i] = [];
            }

            // traverse our files
            int fileIndex = 0;
            foreach (ResourceMetadata resourceListing in packageIndex.Files)
            {
                int loadIndex = Array.IndexOf(_sortedLoadOrder, resourceListing.ResourceType);
                if (loadIndex < 0)
                {
                    fileIndex++;
                    continue;
                }

                // add this index to the list of indexes for this type
                sortedFileIndexes[loadIndex].Add(fileIndex++);
            }

            // iterate over our sorted list of indexes
            for (int i = 0; i < _sortedLoadOrder.Length; i++)
            {
                string rt = _sortedLoadOrder[i];

                Type? netType = Hl7.Fhir.Model.ModelInfo.GetTypeForFhirType(rt);

                if (netType == null)
                {
                    if (rt == "Conformance")
                    {
                        netType = typeof(CapabilityStatement);
                    }
                    else
                    {
                        throw new Exception($"Failed to find .NET type for FHIR type {rt}");
                    }
                }

                // check to see if we want to load this type
                switch (rt)
                {
                    case "CodeSystem":
                        if (!_rootConfiguration.LoadStructures.Contains(FhirArtifactClassEnum.CodeSystem))
                        {
                            continue;
                        }
                        break;

                    case "ValueSet":
                        if (!_rootConfiguration.LoadStructures.Contains(FhirArtifactClassEnum.ValueSet))
                        {
                            continue;
                        }
                        break;

                    case "StructureDefinition":
                        // note: structure definitions can be one of several types and need to be filtered again after parsing
                        if (_rootConfiguration.LoadStructures.Contains(FhirArtifactClassEnum.PrimitiveType) ||
                            _rootConfiguration.LoadStructures.Contains(FhirArtifactClassEnum.ComplexType) ||
                            _rootConfiguration.LoadStructures.Contains(FhirArtifactClassEnum.Resource) ||
                            _rootConfiguration.LoadStructures.Contains(FhirArtifactClassEnum.Extension) ||
                            _rootConfiguration.LoadStructures.Contains(FhirArtifactClassEnum.Profile) ||
                            _rootConfiguration.LoadStructures.Contains(FhirArtifactClassEnum.LogicalModel) ||
                            _rootConfiguration.LoadStructures.Contains(FhirArtifactClassEnum.Interface))
                        {
                            break;
                        }
                        continue;

                    case "SearchParameter":
                        if (!_rootConfiguration.LoadStructures.Contains(FhirArtifactClassEnum.SearchParameter))
                        {
                            continue;
                        }
                        break;

                    case "OperationDefinition":
                        if (!_rootConfiguration.LoadStructures.Contains(FhirArtifactClassEnum.Operation))
                        {
                            continue;
                        }
                        break;

                    case "Conformance":
                    case "CapabilityStatement":
                        if (!_rootConfiguration.LoadStructures.Contains(FhirArtifactClassEnum.CapabilityStatement))
                        {
                            continue;
                        }
                        break;

                    case "ImplementationGuide":
                        if (!_rootConfiguration.LoadStructures.Contains(FhirArtifactClassEnum.ImplementationGuide))
                        {
                            continue;
                        }
                        break;

                    case "CompartmentDefinition":
                        if (!_rootConfiguration.LoadStructures.Contains(FhirArtifactClassEnum.Compartment))
                        {
                            continue;
                        }
                        break;
                }

                foreach (int fi in sortedFileIndexes[i])
                {
                    ResourceMetadata pFile = packageIndex.Files[fi];

                    // load the file
                    string path = Path.Combine(packageRootDirectory, pFile.FilePath);

                    if (!File.Exists(path))
                    {
                        if (pFile.FileName.StartsWith("ig-") && pFile.FileName.EndsWith(".json"))
                        {
                            // ignore IG artifacts that do not exist - issue in IG publisher for IGs that target multiple FHIR versions
                            continue;
                        }

                        throw new Exception($"Listed file {packageReference.Moniker}:{pFile.FileName} does not exist (looking for '{path}')");
                    }

                    string fileExtension = Path.GetExtension(pFile.FileName);

                    object? r;

                    switch (definitions.FhirSequence)
                    {
                        case FhirReleases.FhirSequenceCodes.DSTU2:
                            {
                                r = ParseContents20(fileExtension, path: path);
                            }
                            break;

                        case FhirReleases.FhirSequenceCodes.STU3:
                            {
                                r = ParseContents30(fileExtension, path: path);
                            }
                            break;

                        case FhirReleases.FhirSequenceCodes.R4:
                        case FhirReleases.FhirSequenceCodes.R4B:
                            {
                                r = ParseContents43(fileExtension, path: path);
                            }
                            break;

                        default:
                        case FhirReleases.FhirSequenceCodes.R5:
                            {
                                if (_jsonModel == LoaderOptions.JsonDeserializationModel.SystemTextJson)
                                {
                                    r = ParseContentsSystemTextStream(fileExtension, netType, path: path);
                                }
                                else
                                {
                                    r = ParseContentsPoco(fileExtension, path);
                                }
                            }
                            break;
                    }

                    if (r == null)
                    {
                        throw new Exception($"Failed to parse {rt} {packageReference.Moniker}:{pFile.FileName}");
                    }

                    // filter for structure types we are not processing
                    if (filterStructureDefinitions && r is StructureDefinition sd)
                    {
                        if (!_rootConfiguration.LoadStructures.Contains(sd.cgArtifactClass()))
                        {
                            // skip this artifact
                            continue;
                        }
                    }

                    definitions.AddResource(r, packageFhirVersion, manifest.Name, manifest.Version, manifest.Canonical!);
                }
            }

            // check for loading canonical examples

            if (_rootConfiguration.LoadCanonicalExamples)
            {
                string exampleDir = Path.Combine(packageDirectory, "examples");

                if (Directory.Exists(exampleDir))
                {
                    // get files in the directory
                    string[] files = Directory.GetFiles(exampleDir, "*.json", SearchOption.TopDirectoryOnly);
                    foreach (string path in files)
                    {
                        string fileExtension = Path.GetExtension(path);

                        object? r;

                        switch (definitions.FhirSequence)
                        {
                            case FhirReleases.FhirSequenceCodes.DSTU2:
                                {
                                    r = ParseContents20(fileExtension, path: path);
                                }
                                break;

                            case FhirReleases.FhirSequenceCodes.STU3:
                                {
                                    r = ParseContents30(fileExtension, path: path);
                                }
                                break;

                            case FhirReleases.FhirSequenceCodes.R4:
                            case FhirReleases.FhirSequenceCodes.R4B:
                                {
                                    r = ParseContents43(fileExtension, path: path);
                                }
                                break;

                            default:
                            case FhirReleases.FhirSequenceCodes.R5:
                                {
                                    r = ParseContentsPoco(fileExtension, path);
                                }
                                break;
                        }

                        if (r == null)
                        {
                            throw new Exception($"Failed to parse '{path}'");
                        }

                        definitions.AddResource(r, _defaultFhirVersion, manifest.Name, manifest.Version, manifest.Canonical!);
                    }
                }
            }

            // check to see if this package is a 'core' FHIR package to add missing contents
            if ((manifest.Type == "core") ||
                (manifest.Type == "fhir.core") ||
                FhirPackageUtils.PackageIsFhirRelease(packageReference.Name))
            {
                AddMissingCoreSearchParameters(definitions!, manifest.Name, manifest.Version);
                AddAllInteractionParameters(definitions!);
                AddSearchResultParameters(definitions!);
            }
        }

        // generate any missing Snapshots - note this has to be done after all resources are loaded so dependencies can be resolved
        if (definitions != null)
        {
            _ = await definitions.TryGenerateMissingSnapshots();
        }

        return definitions;
    }

    /// <summary>Parse contents.</summary>
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    /// <param name="format"> Describes the format to use.</param>
    /// <param name="path">   (Optional) Full pathname of the file.</param>
    /// <param name="content">(Optional) The content.</param>
    /// <returns>A TResource?</returns>
    public object? ParseContentsPoco(string format, string path = "", string content = "")
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
                    if (string.IsNullOrEmpty(content))
                    {
#if NET8_0_OR_GREATER
                        content = File.ReadAllTextAsync(path).Result;
#else
                        content = File.ReadAllText(path);
#endif
                    }

                    // always use lenient parsing
                    Resource parsed = _jsonParser.DeserializeResource(content);
                    return parsed;
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
#if DISABLE_XML
                throw new Exception("XML is currently disabled");
#else
                try
                {
                    if (string.IsNullOrEmpty(content))
                    {
                        content = File.ReadAllTextAsync(path).Result;
                    }

                    // always use lenient parsing
                    Resource parsed = _xmlParser.DeserializeResource(content);
                    return parsed;
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
#endif
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
    public object? ParseContents43(string format, string path = "", string content = "")
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
                    if (string.IsNullOrEmpty(content))
                    {
                        content = File.ReadAllText(path);
                    }

                    Hl7.Fhir.ElementModel.ISourceNode sn = FhirJsonNode.Parse(content);
                    return _converter_43_50!.Convert(sn);
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
                    if (string.IsNullOrEmpty(content))
                    {
                        content = File.ReadAllText(path);
                    }

                    Hl7.Fhir.ElementModel.ISourceNode sn = FhirXmlNode.Parse(content);
                    return _converter_43_50!.Convert(sn);
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

    public object? ParseContents30(string format, string path = "", string content = "")
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
                    if (string.IsNullOrEmpty(content))
                    {
                        content = File.ReadAllText(path);
                    }

                    Hl7.Fhir.ElementModel.ISourceNode sn = FhirJsonNode.Parse(content);
                    return _converter_30_50!.Convert(sn);
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
                    if (string.IsNullOrEmpty(content))
                    {
#if NET8_0_OR_GREATER
                        content = File.ReadAllTextAsync(path).Result;
#else
                        content = File.ReadAllText(path);
#endif
                    }

                    Hl7.Fhir.ElementModel.ISourceNode sn = FhirXmlNode.Parse(content);
                    return _converter_30_50!.Convert(sn);
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

    public object? ParseContents20(string format, string path = "", string content = "")
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
                    if (string.IsNullOrEmpty(content))
                    {
#if NET8_0_OR_GREATER
                        content = File.ReadAllTextAsync(path).Result;
#else
                        content = File.ReadAllText(path);
#endif
                    }

                    Hl7.Fhir.ElementModel.ISourceNode sn = FhirJsonNode.Parse(content);
                    return _converter_20_50!.Convert(sn);
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
                    if (string.IsNullOrEmpty(content))
                    {
#if NET8_0_OR_GREATER
                        content = File.ReadAllTextAsync(path).Result;
#else
                        content = File.ReadAllText(path);
#endif
                    }

                    Hl7.Fhir.ElementModel.ISourceNode sn = FhirXmlNode.Parse(content);
                    return _converter_20_50!.Convert(sn);
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
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    /// <param name="format">      The format of the content.</param>
    /// <param name="resourceType">Type of the resource.</param>
    /// <param name="path">        (Optional) Full pathname of the file.</param>
    /// <param name="content">     (Optional) The content.</param>
    /// <returns>
    /// The parsed resource of type <typeparamref name="TResource"/> or null if parsing fails.
    /// </returns>
    public object? ParseContentsSystemTextStream(string format, Type resourceType, string path = "", string content = "")
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
                    if (!string.IsNullOrEmpty(content))
                    {
                        return JsonSerializer.Deserialize(content, resourceType, _jsonOptions);
                    }

                    using (FileStream fs = new(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        return JsonSerializer.Deserialize(fs, resourceType, _jsonOptions);
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
#if DISABLE_XML
                throw new Exception("XML is currently disabled");
#else
                try
                {
                    if (string.IsNullOrEmpty(content))
                    {
                        content = File.ReadAllText(path);
                    }

                    // always use lenient parsing
                    Resource parsed = _xmlParser.DeserializeResource(content);
                    return parsed;
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
#endif
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


    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="FhirCache"/>
    /// and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">True to release both managed and unmanaged resources; false to
    ///  release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
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
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposedValue = true;
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
    /// resources.
    /// </summary>
    void IDisposable.Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
