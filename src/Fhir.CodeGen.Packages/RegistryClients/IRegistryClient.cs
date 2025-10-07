using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fhir.CodeGen.Packages.Models;

namespace Fhir.CodeGen.Packages.RegistryClients;

public interface IRegistryClient
{
    //virtual static IRegistryClient Create(PackageRegistryRecord record, HttpClient? client = null) => throw new NotImplementedException();

    bool SupportsFindByName { get; }
    bool SupportsFindByCanonical { get; }
    bool SupportsFindByFhirVersion { get; }

    List<RegistryCatalogRecord>? Find(
        string? name = null,
        string? packageCanonical = null,
        string? canonical = null,
        string? fhirVersion = null);

    FullPackageManifest? GetFullManifest(string packageId);
}
