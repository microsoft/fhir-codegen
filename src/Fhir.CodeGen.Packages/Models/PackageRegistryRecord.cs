using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fhir.CodeGen.Packages.Models;

public record class PackageRegistryRecord
{
    public static readonly PackageRegistryRecord FhirPrimaryRegistry = new()
    {
        RegistryType = RegistryTypeCodes.FhirPrimary,
        ServerUrl = "https://packages.fhir.org/",
    };

    public static readonly PackageRegistryRecord FhirSecondaryRegistry = new()
    {
        RegistryType = RegistryTypeCodes.FhirSecondary,
        ServerUrl = "https://packages2.fhir.org/packages",
    };

    public static readonly PackageRegistryRecord FhirCiRegistry = new()
    {
        RegistryType = RegistryTypeCodes.FhirCi,
        ServerUrl = "https://build.fhir.org/"
    };

    public static readonly List<PackageRegistryRecord> DefaultRegistries = [
        FhirPrimaryRegistry,
        FhirSecondaryRegistry,
        FhirCiRegistry,
    ];

    public enum RegistryTypeCodes
    {
        Unknown,
        FhirPrimary,
        FhirSecondary,
        FhirCi,
        FhirHttp,
        Npm,
    }

    private string url = null!;

    public required string ServerUrl
    {
        get => url;
        init
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("ServerUrl cannot be null or whitespace", nameof(ServerUrl));

            if (value.EndsWith('/') ||
                value.Contains('?'))
            {
                url = value;
                uri = new(value);
                return;
            }

            url = value + "/";
            uri = new(value + "/");
        }
    }

    private Uri uri = null!;
    public Uri ServerUri => uri;

    public required RegistryTypeCodes RegistryType { get; init; }

    public string? AuthHeaderValue { get; init; } = null;
    public List<(string Name, string Value)>? CustomHeaders { get; init; } = null;
    public string? UserAgent { get; init; } = null;
}
