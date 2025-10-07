using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Fhir.CodeGen.Packages.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;

namespace Fhir.CodeGen.Packages.CacheClients;

public class DiskCacheClient : ICacheClient
{
    private static readonly ParallelOptions _parallelForEachOptions = new()
    {
        MaxDegreeOfParallelism = Math.Min(Environment.ProcessorCount - 1, 1),
    };

    private string _cacheDirectory;
    public string CacheDirectory
    {
        get => _cacheDirectory;
        init => _cacheDirectory = string.IsNullOrWhiteSpace(value)
            ? System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".fhir", "packages")
            : value!;
    }

    private List<PackageRegistryRecord> _registries = [];

    public List<PackageRegistryRecord> Registries
    {
        get => _registries;
        init => _registries = value ?? [];
    }

    private HttpClient? _httpClient = null;

    private Dictionary<string, CachedPackageRecord>? _installedPackages = null;
    private Lock _installedPackageLock = new();

    [SetsRequiredMembers]
    public DiskCacheClient(
        string? cacheDirectory = null,
        List<PackageRegistryRecord>? registries = null,
        HttpClient? httpClient = null)
    {
        _cacheDirectory = string.IsNullOrWhiteSpace(cacheDirectory)
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".fhir", "packages")
            : cacheDirectory;

        if (string.IsNullOrWhiteSpace(_cacheDirectory))
        {
            throw new ArgumentException("Cache directory cannot be null or whitespace.", nameof(cacheDirectory));
        }

        if (!Directory.Exists(_cacheDirectory))
        {
            Directory.CreateDirectory(_cacheDirectory);
        }

        _registries = registries ?? PackageRegistryRecord.DefaultRegistries;
        _httpClient = httpClient;
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
        lock (_installedPackageLock)
        {
            if (_installedPackages is not null)
            {
                return;
            }

            _installedPackages = new(StringComparer.OrdinalIgnoreCase);
        }

        // get the list of directories in the cache directory
        string[] directories = Directory.GetDirectories(_cacheDirectory);

        Parallel.ForEach(directories, _parallelForEachOptions, async (directory) =>
        {
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
                string json = await File.ReadAllTextAsync(packageJsonPath);
                manifest = JsonSerializer.Deserialize<PackageManifest>(json);
            }

            // read the .index.json file if it exists
            string indexJsonPath = Path.Combine(directory, "package", ".index.json");
            if (File.Exists(indexJsonPath))
            {
                string json = await File.ReadAllTextAsync(indexJsonPath);
                packageIndex = JsonSerializer.Deserialize<PackageIndex>(json);
            }

            string fullPath = Path.GetFullPath(directory);

            lock (_installedPackageLock)
            {
                _installedPackages[cacheDirective.FhirCacheDirective ?? directive] = new CachedPackageRecord
                {
                    Directive = cacheDirective,
                    FullPath = fullPath,
                    FullPackagePath = Path.Combine(fullPath, "package"),
                    Manifest = manifest,
                    FileIndex = packageIndex,
                };
            }

            return;
        });
    }

    public async Task<CachedPackageRecord?> InstallPackageAsync(
        string directive,
        bool includeDependencies,
        FhirReleases.FhirSequenceCodes? fhirSequence = null,
        List<PackageRegistryRecord>? registries = null,
        CancellationToken cancellationToken = default)
    {
        registries ??= _registries;
        return null!;
    }

    public Task DeletePackage(CachedPackageRecord packageRecord) => throw new NotImplementedException();
    public Task DeletePackage(string directive) => throw new NotImplementedException();
}
