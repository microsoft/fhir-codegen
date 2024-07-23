using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Specification.Source;
using Hl7.Fhir.Utility;
//using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.CodeGen.Loader;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;

namespace Microsoft.Health.Fhir.CodeGen.Tests;

static class CanonicalExtensions
{
    public static string? Version(this Canonical url)
    {
        int indexOfVersion = url.Value.IndexOf("|");
        if (indexOfVersion == -1)
            return null;
        var result = url.Value.Substring(indexOfVersion + 1);
        int indexOfFragment = result.IndexOf("#");
        if (indexOfFragment != -1)
            return result.Substring(0, indexOfFragment);
        return result;
    }

    public static void Version(this Canonical url, string version)
    {
        if (string.IsNullOrEmpty(version))
            return;

        string newUrl = $"{url.BaseCanonicalUrl()}|{version}";
        string? fragment = url.Fragment();
        int indexOfVersion = url.Value.IndexOf("|");
        if (!string.IsNullOrEmpty(fragment))
            newUrl += $"#{fragment}";
        url.Value = newUrl;
    }

    public static string BaseCanonicalUrl(this Canonical url)
    {
        int indexOfVersion = url.Value.IndexOf("|");
        if (indexOfVersion != -1)
            return url.Value.Substring(0, indexOfVersion);
        int indexOfFragment = url.Value.IndexOf("#");
        if (indexOfFragment != -1)
            return url.Value.Substring(0, indexOfFragment);
        return url.Value;
    }

    public static string? Fragment(this Canonical url)
    {
        int indexOfFragment = url.Value.IndexOf("#");
        if (indexOfFragment != -1)
            return url.Value.Substring(0, indexOfFragment);
        return null;
    }
}

/// <summary>
/// This resolver will take any non version specific question and then resolve with
/// the designated version number
/// </summary>
internal class FixedVersionResolver : IAsyncResourceResolver
{
    public FixedVersionResolver(string version, IAsyncResourceResolver source)
    {
        _fixedVersion = version;
        _source = source;
    }
    private string _fixedVersion;
    private IAsyncResourceResolver _source;

    public async Task<Resource> ResolveByCanonicalUriAsync(string uri)
    {
        Canonical cu = new Canonical(uri);
        if (string.IsNullOrWhiteSpace(cu.Version()))
            cu.Version(_fixedVersion);
        return await _source.ResolveByCanonicalUriAsync(cu);
    }

    public async Task<Resource> ResolveByUriAsync(string uri)
    {
        Canonical cu = new Canonical(uri);
        if (string.IsNullOrWhiteSpace(cu.Version()))
            cu.Version(_fixedVersion);
        return await _source.ResolveByUriAsync(cu);
    }
}

internal class VersionFilterResolver : IAsyncResourceResolver
{
    public VersionFilterResolver(string version, IAsyncResourceResolver source)
    {
        _fixedVersion = version;
        _source = source;
    }
    private string _fixedVersion;
    private IAsyncResourceResolver _source;

    public async Task<Resource> ResolveByCanonicalUriAsync(string uri)
    {
        string convertedUrl = CrossVersionResolver.ConvertCanonical(uri);
        Canonical cu = new Canonical(convertedUrl);
        if (!string.IsNullOrWhiteSpace(cu.Version()))
        {
            if (cu.Version() != _fixedVersion)
                return null!;
        }
        var result = await _source.ResolveByCanonicalUriAsync(cu.BaseCanonicalUrl());
        return result;
    }

    public async Task<Resource> ResolveByUriAsync(string uri)
    {
        string convertedUrl = CrossVersionResolver.ConvertCanonical(uri);
        Canonical cu = new Canonical(convertedUrl);
        if (!string.IsNullOrWhiteSpace(cu.Version()))
        {
            if (cu.Version() != _fixedVersion)
                return null!;
        }
        return await _source.ResolveByUriAsync(cu.BaseCanonicalUrl());
    }
}

internal class CrossVersionResolver : IAsyncResourceResolver
{
    public CrossVersionResolver()
    {
        var settingsJson = new FhirJsonParsingSettings() { PermissiveParsing = true };
        var settingsXml = new FhirXmlParsingSettings() { PermissiveParsing = true };
        var settingsDir = new DirectorySourceSettings()
        {
            JsonParserSettings = settingsJson,
            XmlParserSettings = settingsXml,
            ParserSettings = new ParserSettings() { ExceptionHandler = (object source, ExceptionNotification args) => { } }
        };

        string crossVersionPackages = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "FhirCrossVersionTests");
        if (!System.IO.Directory.Exists(crossVersionPackages))
        {
            System.Diagnostics.Trace.WriteLine($"Cross Version package cache folder does not exist {crossVersionPackages}");
        }

        //dstu2 = new DirectorySource(Path.Combine(crossVersionPackages, "r2b"), settingsDir);
        //stu3 = new DirectorySource(Path.Combine(crossVersionPackages, "r3"), settingsDir);
        //r4 = new DirectorySource(Path.Combine(crossVersionPackages, "r4"), settingsDir);
        //r4b = new DirectorySource(Path.Combine(crossVersionPackages, "r4b"), settingsDir);
        //r5 = ZipSource.CreateValidationSource();
    }

    public async System.Threading.Tasks.Task<IEnumerable<DefinitionCollection>> Initialize(string[] versions)
    {
        List<DefinitionCollection> result = new List<DefinitionCollection>();
        PackageLoader loader = new(new() { AutoLoadExpansions = false, ResolvePackageDependencies = false });
        foreach (var version in versions)
        {
            if (version == "5")
            {
                if (r5 == null)
            r5 = await LoadPackage(loader, "hl7.fhir.r5.core#5.0.0");
                result.Add((DefinitionCollection)r5);
            }
            if (version == "4B")
            {
                if (r4b == null)
            r4b = await LoadPackage(loader, "hl7.fhir.r4b.core#4.3.0");
                result.Add((DefinitionCollection)r4b);
            }
            if (version == "4")
            {
                if (r4 == null)
            r4 = await LoadPackage(loader, "hl7.fhir.r4.core#4.0.1");
                result.Add((DefinitionCollection)r4);
            }
            if (version == "3")
            {
                if (stu3 == null)
            stu3 = await LoadPackage(loader, "hl7.fhir.r3.core#3.0.2");
                result.Add((DefinitionCollection)stu3);
            }
            if (version == "2")
            {
                if (dstu2 == null)
            dstu2 = await LoadPackage(loader, "hl7.fhir.r2.core#1.0.2");
                result.Add((DefinitionCollection)dstu2);
            }
        }
        return result;
    }

    /// <summary>Loads.</summary>
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    /// <param name="directive">The directive to load.</param>
    /// <returns>A PackageCacheEntry.</returns>
    private async Task<DefinitionCollection> LoadPackage(PackageLoader loader, string directive)
    {
        DefinitionCollection? loaded = await loader.LoadPackages([directive]);
        if (loaded == null)
        {
            throw new ApplicationException($"Failed to load package {directive} into cache");
        }
        return loaded;
    }


    public IAsyncResourceResolver OnlyDstu2 { get { return new VersionFilterResolver("2.0", dstu2); } }
    public IAsyncResourceResolver OnlyStu3 { get { return new VersionFilterResolver("3.0", stu3); } }
    public IAsyncResourceResolver OnlyR4 { get { return new VersionFilterResolver("4.0", r4); } }
    public IAsyncResourceResolver OnlyR4B { get { return new VersionFilterResolver("4.3", r4b); } }
    public IAsyncResourceResolver OnlyR5 { get { return new VersionFilterResolver("5.0", r5); } }

    void CustomExceptionHandler(object source, ExceptionNotification args)
    {
        // System.Diagnostics.Trace.WriteLine(args.Message);
    }

    IAsyncResourceResolver dstu2;
    IAsyncResourceResolver stu3;
    IAsyncResourceResolver r4;
    IAsyncResourceResolver r4b;
    IAsyncResourceResolver r5;

    const string fhirBaseCanonical = "http://hl7.org/fhir/";

    public static string ConvertCanonical(string uri)
    {
        if (uri.StartsWith(fhirBaseCanonical))
        {
            var remainder = uri.Substring(fhirBaseCanonical.Length);
            string resourceName;
            if (remainder.StartsWith("StructureDefinition/"))
                remainder = remainder.Substring("StructureDefinition/".Length);
            if (!remainder.Contains("/"))
                return uri;
            resourceName = remainder.Substring(remainder.IndexOf("/") + 1);
            if (resourceName.StartsWith("StructureDefinition/"))
                resourceName = resourceName.Substring("StructureDefinition/".Length);
            remainder = remainder.Substring(0, remainder.IndexOf("/"));

            // convert this from the old format into the versioned format
            // http://hl7.org/fhir/3.0/StructureDefinition/Account
            // =>
            // http://hl7.org/fhir/StructureDefinition/Account|3.0
            // http://hl7.org/fhir/StructureDefinition/Account|3.0.1
            // http://hl7.org/fhir/StructureDefinition/Account|4.0.1
            // i.e. https://github.com/microsoft/fhir-codegen/blob/dev/src/Microsoft.Health.Fhir.SpecManager/Manager/FhirPackageCommon.cs#L513

            string version = remainder;
            switch (version)
            {
                case "1.0":
                case "1.0.2":
                case "DSTU2":
                    version = "1.0";
                    break;
                case "3.0":
                case "3.0.0":
                case "3.0.1":
                case "3.0.2":
                case "STU3":
                    version = "3.0";
                    break;

                case "4.0":
                case "4.0.0":
                case "4.0.1":
                case "R4":
                    version = "4.0";
                    break;

                case "4.3":
                case "4.3.0":
                case "R4B":
                    version = "4.3";
                    break;

                case "5.0":
                case "5.0.0":
                case "R5":
                    version = "5.0";
                    break;
                default:
                    return uri;
            }
            return $"{fhirBaseCanonical}StructureDefinition/{resourceName}|{version}";
        }
        return uri;
    }

    public async Task<Resource> ResolveByCanonicalUriAsync(string uri)
    {
        string convertedUrl = ConvertCanonical(uri);
        Canonical cu = new Canonical(convertedUrl);
        Resource result;
        if (cu.Version() == "1.0")
            result = await dstu2.ResolveByCanonicalUriAsync(cu.BaseCanonicalUrl());
        else if (cu.Version() == "3.0")
            result = await stu3.ResolveByCanonicalUriAsync(cu.BaseCanonicalUrl());
        else if (cu.Version() == "4.0")
            result = await r4.ResolveByCanonicalUriAsync(cu.BaseCanonicalUrl());
        else if (cu.Version() == "4.3")
            result = await r4b.ResolveByCanonicalUriAsync(cu.BaseCanonicalUrl());
        else if (cu.Version() == "5.0")
            result = await r5.ResolveByCanonicalUriAsync(cu.BaseCanonicalUrl());
        else
            result = await r4.ResolveByCanonicalUriAsync(cu.BaseCanonicalUrl());
        if (result == null)
        {
            Console.WriteLine($"  * Failed to resolve: {uri} at [{cu.Version()}] {cu.BaseCanonicalUrl()}");
        }
        return result;
    }

    public async Task<Resource> ResolveByUriAsync(string uri)
    {
        string convertedUrl = ConvertCanonical(uri);
        Canonical cu = new Canonical(convertedUrl);
        if (cu.Version() == "1.0")
            return await dstu2.ResolveByUriAsync(cu.BaseCanonicalUrl());
        if (cu.Version() == "3.0")
            return await stu3.ResolveByUriAsync(cu.BaseCanonicalUrl());
        if (cu.Version() == "4.0")
            return await r4.ResolveByUriAsync(cu.BaseCanonicalUrl());
        if (cu.Version() == "4.3")
            return await r4b.ResolveByUriAsync(cu.BaseCanonicalUrl());
        if (cu.Version() == "5.0")
            return await r5.ResolveByUriAsync(cu.BaseCanonicalUrl());
        return await r4.ResolveByUriAsync(cu.BaseCanonicalUrl());
    }
}
