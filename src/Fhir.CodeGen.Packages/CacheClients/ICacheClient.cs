using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fhir.CodeGen.Packages.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;

namespace Fhir.CodeGen.Packages.CacheClients;

public interface ICacheClient
{

    Task<List<CachedPackageRecord>> ListCachedPackages(
        string? packageIdFilter = null,
        string? versionFilter = null);

    Task<CachedPackageRecord?> InstallPackageAsync(
        string directive,
        bool includeDependencies,
        FhirReleases.FhirSequenceCodes? fhirSequence = null,
        List<PackageRegistryRecord>? registries = null,
        CancellationToken cancellationToken = default);

    Task DeletePackage(CachedPackageRecord packageRecord);
    Task DeletePackage(string directive);
}
