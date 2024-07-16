using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;

namespace Microsoft.Health.Fhir.CodeGen._ForPackages;

public class DiskPackageCache : Firely.Fhir.Packages.DiskPackageCache
{
    public DiskPackageCache(string? rootDirectory = null)
        : base(rootDirectory)
    {
    }

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
        PackageManifest? manifest = JsonSerializer.Deserialize<PackageManifest>(json);

        return Task.FromResult(manifest);
    }

}
