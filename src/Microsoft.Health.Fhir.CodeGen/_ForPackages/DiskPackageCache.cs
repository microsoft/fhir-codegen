using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Firely.Fhir.Packages;
using Newtonsoft.Json;

namespace Microsoft.Health.Fhir.CodeGen._ForPackages;

public class DiskPackageCache : Firely.Fhir.Packages.DiskPackageCache
{
    public DiskPackageCache(string? rootDirectory = null)
        : base(rootDirectory)
    {
    }

    private static readonly JsonSerializerSettings SETTINGS = new()
    {
        MissingMemberHandling = MissingMemberHandling.Ignore,
        //Converters = new List<JsonConverter> { new AuthorJsonConverter(), new ManifestDateJsonConverter() }
    };

    public Task<PackageManifest?> ReadManifestEx(Firely.Fhir.Packages.PackageReference reference)
    {
        string folder = PackageContentFolder(reference);

        if (!Directory.Exists(folder))
        {
            return Task.FromResult<PackageManifest?>(null);
        }

        string path = Path.Combine(folder, Firely.Fhir.Packages.PackageFileNames.MANIFEST);
        if (!File.Exists(path))
        {
            return Task.FromResult<PackageManifest?>(null);
        }

        string json = File.ReadAllText(path);
        PackageManifest? manifest = JsonConvert.DeserializeObject<PackageManifest>(json, SETTINGS);

        return Task.FromResult(manifest);
    }


    /// <summary>
    /// Deletes a package from the disk package cache.
    /// </summary>
    /// <param name="reference">The package reference.</param>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async Task Delete(PackageReference reference)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        var target = PackageContentFolder(reference);
        if (!Directory.Exists(target))
        {
            return;
        }

        // delete the directory recursively
        Directory.Delete(target, true);
    }

}
