using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Formats.Tar;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Fhir.CodeGen.Packages.Models;
using Fhir.CodeGen.Packages.RegistryClients;
using Hl7.Fhir.Rest;
using Fhir.CodeGen.Common.Packaging;

namespace Fhir.CodeGen.Packages.CacheClients;

public class DiskCacheClient : CacheClientBase, IFhirCacheClient
{
    private string _cacheDirectory;
    public string CacheDirectory
    {
        get => _cacheDirectory;
        init
        {
            if (string.IsNullOrEmpty(value))
            {
                _cacheDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".fhir");
                _packageDirectory = Path.Combine(_cacheDirectory, "packages");
            }
            else if (Path.GetFileName(value).Equals("packages", StringComparison.OrdinalIgnoreCase) ||
                value.EndsWith("packages", StringComparison.OrdinalIgnoreCase))
            {
                _cacheDirectory = Path.GetFullPath(Path.Combine(value, ".."));
                _packageDirectory = value;
            }
            else
            {
                _cacheDirectory = value;
                _packageDirectory = Path.Combine(_cacheDirectory, "packages");
            }
    }
    }

    private string _packageDirectory;
    public string PackageDirectory => _packageDirectory;

    public string Identifier => _cacheDirectory;

    [SetsRequiredMembers]
    public DiskCacheClient(
        string? cacheDirectory = null,
        List<RegistryEndpointRecord>? registryEndpoints = null,
        List<IPackageRegistryClient>? registryClients = null,
        HttpClient? httpClient = null)
    {
        _cacheDirectory = string.IsNullOrWhiteSpace(cacheDirectory)
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".fhir")
            : cacheDirectory;

        if (string.IsNullOrWhiteSpace(_cacheDirectory))
        {
            throw new ArgumentException("Cache directory cannot be null or whitespace.", nameof(cacheDirectory));
        }

        if (!Directory.Exists(_cacheDirectory))
        {
            Directory.CreateDirectory(_cacheDirectory);
        }

        if (Path.GetFileName(_cacheDirectory).Equals("packages", StringComparison.OrdinalIgnoreCase) ||
            _cacheDirectory.EndsWith("packages", StringComparison.OrdinalIgnoreCase))
        {
            _packageDirectory = _cacheDirectory;
            _cacheDirectory = Path.GetFullPath(Path.Combine(_packageDirectory, ".."));
        }
        else
        {
            _packageDirectory = Path.Combine(_cacheDirectory, "packages");
        }

        if (!Directory.Exists(_packageDirectory))
        {
            Directory.CreateDirectory(_packageDirectory);
        }

        _httpClient = httpClient ?? new();

        configureRegistryInfo(registryEndpoints, registryClients);
    }

    public async Task<List<CachedPackageRecord>> ListCachedPackages(string? packageIdFilter = null, string? versionFilter = null)
    {
        if (_installedPackages is null)
        {
            // indexing packages can take a while, so allow it to run on a background thread
            await Task.Run(indexCachedPackages);

            if (_installedPackages is null)
            {
                throw new InvalidOperationException("Failed to index installed packages.");
            }
        }

        return _installedPackages.Values.ToList();
    }

    private void indexCachedPackages()
    {
        if (_installedPackages is not null)
        {
            return;
        }

        // lock on indexing operation to prevent multiple threads from indexing at the same time
        lock (_installedPackageLock)
        {
            // check if already done after getting the lock
            if (_installedPackages is not null)
            {
                return;
            }

            ConcurrentDictionary<string, CachedPackageRecord> installedPackages = new();

            // get the list of directories in the cache package directory
            string[] directories = Directory.GetDirectories(_packageDirectory);

            Parallel.ForEach(directories, _parallelForEachOptions, (directory) =>
            {
                Console.WriteLine($"Indexing package directory: {directory}");
                string? directive = Path.GetFileName(directory);

                if (string.IsNullOrWhiteSpace(directive))
                {
                    return;
                }

                PackageDirective cacheDirective = new(directive);
                PackageManifest? manifest = null;
                PackageIndex? packageIndex = null;

                // read the package.json file if it exists
                string packageJsonPath = Path.Combine(directory, "package", "package.json");
                if (File.Exists(packageJsonPath))
                {
                    string json = File.ReadAllText(packageJsonPath);
                    manifest = JsonSerializer.Deserialize<PackageManifest>(json);
                    cacheDirective = cacheDirective with
                    {
                        ResolvedVersion = manifest?.ParsedVersion,
                    };
                }

                // read the .index.json file if it exists
                string indexJsonPath = Path.Combine(directory, "package", ".index.json");
                if (File.Exists(indexJsonPath))
                {
                    string json = File.ReadAllText(indexJsonPath);
                    packageIndex = JsonSerializer.Deserialize<PackageIndex>(json);
                }

                string fullPath = Path.GetFullPath(directory);

                installedPackages.TryAdd(
                    cacheDirective.FhirCacheDirective ?? directive,
                    new CachedPackageRecord
                    {
                        Directive = cacheDirective,
                        FullPath = fullPath,
                        FullPackagePath = Path.Combine(fullPath, "package"),
                        Manifest = manifest,
                        FileIndex = packageIndex,
                    });
            });

            _installedPackages = installedPackages;
        }
    }

    private void addPackageToIndex(CachedPackageRecord rec)
    {
        // ensure the index is up-to-date
        if (_installedPackages is null)
        {
            indexCachedPackages();
            if (_installedPackages is null)
            {
                throw new InvalidOperationException("Failed to index installed packages.");
            }
        }

        if (string.IsNullOrEmpty(rec.Directive.FhirCacheDirective))
        {
            throw new Exception($"Invalid package record: {rec}");
        }

        _installedPackages.AddOrUpdate(rec.Directive.FhirCacheDirective, rec, (_, _) => rec);
    }

    private CachedPackageRecord? resolveLocally(PackageDirective directive)
    {
        // ensure the index is up-to-date
        if (_installedPackages is null)
        {
            indexCachedPackages();
            if (_installedPackages is null)
            {
                throw new InvalidOperationException("Failed to index installed packages.");
            }
        }

        switch (directive.VersionType)
        {
            case PackageDirective.DirectiveVersionCodes.Wildcard:
            case PackageDirective.DirectiveVersionCodes.Latest:
                {
                    // if there are none, we are done
                    if (_installedPackages.Count == 0)
                    {
                        return null;
                    }

                    // build a list of local versions that match our package id
                    List<CachedPackageRecord> localDirectives = _installedPackages
                        .Values
                        .Where(rec => rec.Directive.PackageId?.Equals(directive.PackageId, StringComparison.OrdinalIgnoreCase) ?? false)
                        .ToList();

                    // if there are none, we are done
                    if (localDirectives.Count == 0)
                    {
                        return null;
                    }

                    // filter the list based on versions that satisfy our request
                    localDirectives = localDirectives
                        .Where(rec => (
                            rec.Manifest?.ParsedVersion.Satisfies(directive.RequestedVersionParsed) ?? false) ||
                            (rec.Directive.RequestedVersionParsed?.Satisfies(directive.RequestedVersionParsed) ?? false))
                        .ToList();

                    // if there are none, we are done
                    if (localDirectives.Count == 0)
                    {
                        return null;
                    }

                    // return our highest match
                    return localDirectives
                        .OrderByDescending(
                            rec => rec.Directive.ResolvedVersion ?? rec.Directive.RequestedVersionParsed,
                            new FhirSemVerComparer())
                        .FirstOrDefault();
                }

            case PackageDirective.DirectiveVersionCodes.Unknown:
            case PackageDirective.DirectiveVersionCodes.Exact:
            case PackageDirective.DirectiveVersionCodes.LocalBuild:
            case PackageDirective.DirectiveVersionCodes.CiBuild:
            case PackageDirective.DirectiveVersionCodes.NonSemVer:
                {
                    // see if there is a package that has a literal version match
                    _ = _installedPackages.TryGetValue(
                        directive.FhirCacheDirective ?? directive.RequestedDirective,
                        out CachedPackageRecord? localExisting);

                    return localExisting;
                }
            default:
                return null;
        }
    }

    public async Task<CachedPackageRecord?> GetOrInstallAsync(
        string inputDirective,
        bool includeDependencies,
        FhirReleases.FhirSequenceCodes? fhirSequence = null,
        List<RegistryEndpointRecord>? registryEndpoints = null,
        List<IPackageRegistryClient>? registryClients = null,
        bool overwriteExisting = false,
        CancellationToken cancellationToken = default)
    {
        // ensure the index is up-to-date
        if (_installedPackages is null)
        {
            await Task.Run(indexCachedPackages, cancellationToken);
            if (_installedPackages is null)
            {
                throw new InvalidOperationException("Failed to index installed packages.");
            }
        }

        // first, parse the directive so we know what we are dealing with
        PackageDirective directive = new(inputDirective);

        // we cannot install local builds, either they exist or they don't
        if (directive.VersionType == PackageDirective.DirectiveVersionCodes.LocalBuild)
        {
            return resolveLocally(directive);
        }

        // if we have an exact version and are not forcing overwrite, we can short-circuit if it's already installed
        if (!overwriteExisting &&
            (directive.VersionType == PackageDirective.DirectiveVersionCodes.Exact) &&
            (directive.ResolvedVersion is not null))
        {
            CachedPackageRecord? exactExisting = resolveLocally(directive);
            if (exactExisting is not null)
            {
                return exactExisting;
            }
        }

        // use either passed-in registry information or our defaults
        List<IPackageRegistryClient> clients = getEffectiveClients(registryEndpoints, registryClients, directive.VersionType);

        // if we have no clients that can handle this directive, it can be either be satisfied locally or not
        if (clients.Count == 0)
        {
            return resolveLocally(directive);
        }

        List<(IPackageRegistryClient client, ResolvedDirectiveUri resolved)> resolvedUris = [];

        // get the resolutions from all configured registries
        ConcurrentBag<(IPackageRegistryClient client, ResolvedDirectiveUri resolved)> resolvingList = [];

        Parallel.ForEach(clients, _parallelForEachOptions, (registryClient) =>
        {
            ResolvedDirectiveUri? resolved = registryClient.GetDownloadUri(directive);
            if (resolved != null)
            {
                resolvingList.Add((registryClient, resolved));
            }
        });

        // sort by highest version
        resolvedUris = resolvingList
            .OrderByDescending(r => r.resolved.ResolvedVersion, new FhirSemVerComparer())
            .ToList();

        if (resolvedUris.Count == 0)
        {
            Console.WriteLine($"Failed to resolve directive '{directive.ToString()}' with provided registries, attempting cache-only resolution.");
            return resolveLocally(directive);
        }

        (IPackageRegistryClient resolvingClient, ResolvedDirectiveUri resolvedInfo) = resolvedUris.First();

        string cacheDirective = directive.FhirCacheDirective ??
            directive.PackageId + "#" + resolvedInfo.ResolvedVersion.ToString();

        // check to see if *this* version is already installed and we are not forced-overwriting
        if (!overwriteExisting &&
            _installedPackages.TryGetValue(directive.FhirCacheDirective!, out CachedPackageRecord? resolvedExisting))
        {
            switch (directive.VersionType)
            {
                case PackageDirective.DirectiveVersionCodes.Unknown:
                case PackageDirective.DirectiveVersionCodes.Exact:
                case PackageDirective.DirectiveVersionCodes.Wildcard:
                case PackageDirective.DirectiveVersionCodes.Latest:
                case PackageDirective.DirectiveVersionCodes.LocalBuild:
                    // we can use the local version
                    return resolvedExisting;

                case PackageDirective.DirectiveVersionCodes.CiBuild:
                    {
                        // get version information from the CI server
                        PackageManifest? ciServerManifest = resolvingClient.Resolve(directive);
                        if (ciServerManifest is null)
                        {
                            Console.WriteLine($"Failed to re-resolve CI build manifest for '{directive.RequestedDirective}' - using local cache version.");
                            return resolvedExisting;
                        }

                        // if the *dates* match, we can use the local version
                        if ((ciServerManifest.PublicationDate is not null) &&
                            (resolvedExisting.Manifest?.PublicationDate is not null) &&
                            (ciServerManifest.PublicationDate.Value == resolvedExisting.Manifest.PublicationDate.Value))
                        {
                            return resolvedExisting;
                        }
                    }
                    break;
            }
        }

        // get a temporary directory so we can atomically update the cache
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        //string tempDir = Path.Combine(_cacheDirectory, ".fhir.codegen.temp-", Guid.NewGuid().ToString());

        try
        {
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }

            // get the stream for the tarball
            (HttpStatusCode status, Stream? content) = resolvingClient.GetHttpStream(resolvedInfo.TarballUri);
            if (!status.IsSuccessful())
            {
                Console.WriteLine($"Request failed: {status.ToString()} opening tarball stream: '{resolvedInfo.TarballUri}'");
                return null;
            }

            if (content is null)
            {
                Console.WriteLine($"Request returned no content opening tarball stream: '{resolvedInfo.TarballUri}'");
                return null;
            }

            // decompress and extract the tarball into the temp directory
            using (Stream gzipStream = new GZipStream(content, CompressionMode.Decompress))
            {
                await TarFile.ExtractToDirectoryAsync(gzipStream, tempDir, true, cancellationToken);
            }

            PackageManifest? manifest = null;
            PackageIndex? packageIndex = null;

            // read the package.json file if it exists
            string packageJsonPath = Path.Combine(tempDir, "package", "package.json");
            if (File.Exists(packageJsonPath))
            {
                string json = await File.ReadAllTextAsync(packageJsonPath);
                manifest = JsonSerializer.Deserialize<PackageManifest>(json);
            }

            // read the .index.json file if it exists
            string indexJsonPath = Path.Combine(tempDir, "package", ".index.json");
            if (File.Exists(indexJsonPath))
            {
                string json = await File.ReadAllTextAsync(indexJsonPath);
                packageIndex = JsonSerializer.Deserialize<PackageIndex>(json);
            }

            string destinationDir = Path.Combine(_packageDirectory, cacheDirective);
            lock (_installedPackageLock)
            {
                if (Directory.Exists(destinationDir))
                {
                    if (overwriteExisting)
                    {
                        Directory.Delete(destinationDir, true);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Cache directory '{destinationDir}' already exists.");
                    }
                }

                // if we are on windows, we can only move directories on the same volume
                if (OperatingSystem.IsWindows() &&
                    !string.Equals(Path.GetPathRoot(tempDir), Path.GetPathRoot(destinationDir), StringComparison.OrdinalIgnoreCase))
                {
                    // so we need to copy the temp directory to the destination (will be deleted in finally block)
                    copyDirectory(tempDir, destinationDir, true);
                }
                else
                {
                    // move the temp directory to the cache directory
                    Directory.Move(tempDir, destinationDir);
                }
            }

            // create our cached package record
            CachedPackageRecord newRecord = new()
            {
                Directive = new PackageDirective(cacheDirective)
                {
                    ResolvedVersion = resolvedInfo.ResolvedVersion,
                },
                FullPath = Path.GetFullPath(destinationDir),
                FullPackagePath = Path.GetFullPath(Path.Combine(destinationDir, "package")),
                Manifest = manifest,
                FileIndex = packageIndex,
            };

            addPackageToIndex(newRecord);

            return newRecord;
        }
        finally
        {
            // clean up the temp directory if it still exists
            if (Directory.Exists(tempDir))
            {
                try
                {
                    Directory.Delete(tempDir, true);
                }
                catch
                {
                    // best effort
                }
            }
        }
    }

    private static void copyDirectory(string sourceDir, string destinationDir, bool recursive = true)
    {
        // Get information about the source directory
        DirectoryInfo dir = new DirectoryInfo(sourceDir);

        // Check if the source directory exists
        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");
        }

        // If the destination directory doesn't exist, create it
        if (!Directory.Exists(destinationDir))
        {
            Directory.CreateDirectory(destinationDir);
        }

        // Get the files in the source directory and copy them to the destination directory
        foreach (FileInfo file in dir.GetFiles())
        {
            string targetFilePath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(targetFilePath, true);
        }

        // If recursive and copying subdirectories, recursively call this method
        if (recursive)
        {
            foreach (DirectoryInfo subDir in dir.GetDirectories())
            {
                string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                copyDirectory(subDir.FullName, newDestinationDir, true);
            }
        }
    }

    public Task DeletePackage(CachedPackageRecord packageRecord) => throw new NotImplementedException();
    public Task DeletePackage(string directive) => throw new NotImplementedException();
}
