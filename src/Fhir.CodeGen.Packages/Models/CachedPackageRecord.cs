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
}
