using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fhir.CodeGen.Packages.Models;
using Fhir.CodeGen.Packages.RegistryClients;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;

namespace Fhir.CodeGen.Packages.CacheClients;

public interface ICacheClient
{
    string Identifier { get; }

    Task<List<CachedPackageRecord>> ListCachedPackages(
        string? packageIdFilter = null,
        string? versionFilter = null);

    Task<CachedPackageRecord?> GetOrInstallAsync(
        string inputDirective,
        bool includeDependencies,
        FhirReleases.FhirSequenceCodes? fhirSequence = null,
        List<RegistryEndpointRecord>? registryEndpoints = null,
        List<IRegistryClient>? registryClients = null,
        bool overwriteExisting = false,
        CancellationToken cancellationToken = default);

    Task DeletePackage(CachedPackageRecord packageRecord);
    Task DeletePackage(string directive);
}
