using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fhir.CodeGen.Packages.Models;

public record class RegistryEndpointRecord
{
    public static readonly RegistryEndpointRecord FhirPrimaryRegistry = new()
    {
        RegistryType = RegistryTypeCodes.FhirNpm,
        Url = "https://packages.fhir.org/",
    };

    public static readonly RegistryEndpointRecord FhirSecondaryRegistry = new()
    {
        RegistryType = RegistryTypeCodes.FhirNpm,
        Url = "https://packages2.fhir.org/packages",
    };

    public static readonly RegistryEndpointRecord FhirCiRegistry = new()
    {
        RegistryType = RegistryTypeCodes.FhirCi,
        Url = "https://build.fhir.org/"
    };

    public static readonly List<RegistryEndpointRecord> DefaultEndpoints = [
        FhirSecondaryRegistry,
        FhirPrimaryRegistry,
        FhirCiRegistry,
    ];

    public enum RegistryTypeCodes
    {
        FhirNpm,
        FhirCi,
        FhirHttp,
        Npm,
        //CacheOnly,
    }

    private string url = null!;

    public required string Url
    {
        get => url;
        init
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("ServerUrl cannot be null or whitespace", nameof(Url));

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
