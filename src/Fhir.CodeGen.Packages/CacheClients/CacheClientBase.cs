using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fhir.CodeGen.Packages.Models;
using Fhir.CodeGen.Packages.RegistryClients;

namespace Fhir.CodeGen.Packages.CacheClients;

public abstract class CacheClientBase
{
    protected static readonly ParallelOptions _parallelForEachOptions = new()
    {
        MaxDegreeOfParallelism = Math.Min(Environment.ProcessorCount - 1, 1),
    };

    protected List<RegistryEndpointRecord> _registryEndpoints = [];
    protected List<IPackageRegistryClient> _registryClients = [];
    protected HttpClient? _httpClient = null;
    protected ConcurrentDictionary<string, CachedPackageRecord>? _installedPackages = null;
    protected Lock _installedPackageLock = new();

    /// <summary>
    /// Setup local registry endpoint and client objects.
    /// Note that _httpClient must be set *prior* to calling
    /// </summary>
    /// <param name="registryEndpoints"></param>
    /// <param name="registryClients"></param>
    protected void configureRegistryInfo(
        List<RegistryEndpointRecord>? registryEndpoints,
        List<IPackageRegistryClient>? registryClients)
    {
        if ((registryEndpoints is null) &&
            (registryClients is null))
        {
            _registryEndpoints = RegistryEndpointRecord.DefaultEndpoints;
            _registryClients = _registryEndpoints
                .Select(e => IPackageRegistryClient.Create(e, _httpClient))
                .ToList();

            return;
        }

        if (registryClients is null)
        {
            _registryEndpoints = registryEndpoints!;
            _registryClients = _registryEndpoints
                .Select(e => IPackageRegistryClient.Create(e, _httpClient))
                .ToList();

            return;
        }

        if (registryEndpoints is null)
        {
            _registryClients = registryClients!;
            _registryEndpoints = _registryClients
                .Select(c => c.RegistryEndpoint)
                .ToList();

            return;
        }

        _registryEndpoints = registryEndpoints!;
        _registryClients = registryClients!;
    }

    protected List<IPackageRegistryClient> getEffectiveClients(
        List<RegistryEndpointRecord>? paramRegistryEndpoints,
        List<IPackageRegistryClient>? paramRegistryClients,
        PackageDirective.DirectiveVersionCodes forVersionType)
    {
        if ((paramRegistryEndpoints is null) && (paramRegistryClients is null))
        {
            return _registryClients
                .Where(c => c.SupportedVersionTypes.Contains(forVersionType))
                .ToList();
        }

        if (paramRegistryClients is not null)
        {
            return paramRegistryClients
                .Where(c => c.SupportedVersionTypes.Contains(forVersionType))
                .ToList();
        }

        return paramRegistryEndpoints!
            .Select(e => IPackageRegistryClient.Create(e, _httpClient))
            .Where(c => c.SupportedVersionTypes.Contains(forVersionType))
            .ToList();
    }
}
