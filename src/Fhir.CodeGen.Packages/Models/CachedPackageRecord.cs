using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fhir.CodeGen.Packages.Models;

public record class CachedPackageRecord
{
    public required PackageDirective Directive { get; init; }

    public required string FullPath { get; init; }
    public required string FullPackagePath { get; init; }

    public required PackageManifest? Manifest { get; init; }

    public required PackageIndex? FileIndex { get; init; }

    public string GetContentPath()
    {
        if ((Manifest is not null) &&
            (Manifest.Directories is not null) &&
            Manifest.Directories.TryGetValue("lib", out string? dir) &&
            (dir is not null))
        {
            return Path.Combine(FullPath, dir);
        }

        return FullPackagePath;
    }
}
