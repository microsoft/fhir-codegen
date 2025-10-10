using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Fhir.CodeGen.Packages.Models;
using Fhir.CodeGen.Common.Packaging;
using Shouldly;

namespace Fhir.CodeGen.Packages.Tests;

public class RegistryCatalogTests
{
    public enum CatalogType
    {
        Primary,
        Secondary,
    }

    private void testCatalogPrimary(RegistryCatalogRecord entry)
    {
        entry.Name.ShouldNotBeNullOrWhiteSpace();
        entry.Description.ShouldNotBeNullOrWhiteSpace();
        entry.FhirVersion.ShouldNotBeNullOrWhiteSpace();
    }

    private void testCatalogSecondary(RegistryCatalogRecord entry)
    {
        entry.Name.ShouldNotBeNullOrWhiteSpace();
        entry.Version.ShouldNotBeNullOrWhiteSpace();

        // use information from the full manifests
        PackageManifestTests._packageJsonExpectations.TryGetValue(
            (entry.Name, entry.Version),
            out PackageManifestTests.PackageManifestExpectationRecord? expectations)
            .ShouldBeTrue($"Failed to find expectations for package '{entry.Name}' version '{entry.Version}'");

        if (expectations == null)
        {
            throw new ArgumentNullException(nameof(expectations));
        }

        entry.Name.ShouldBe(expectations.Name);
        entry.Version.ShouldBe(expectations.Version);
        entry.FhirVersion.ShouldNotBeNullOrWhiteSpace();
        if (expectations.FhirSequence != null)
        {
            FhirReleases.FhirVersionToSequence(entry.FhirVersion).ShouldBe(expectations.FhirSequence.Value);
        }

        if (expectations.Canonical != null)
        {
            entry.Canonical.ShouldBe(expectations.Canonical);
        }

        if (expectations.PackageDate != null)
        {
            entry.PublicationDate.ShouldNotBeNull();

            // need to check date +/- 1 day because of time zone differences
            entry.PublicationDate.Value.Date.ShouldBeInRange(
                expectations.PackageDate.Value.Date.Subtract(new TimeSpan(24, 0, 0)),
                expectations.PackageDate.Value.Date.Add(new TimeSpan(24, 0, 0)));
        }
    }


    [Theory]
    [InlineData("catalog-backport-primary.json", CatalogType.Primary)]
    [InlineData("catalog-backport-primary-r4.json", CatalogType.Primary)]
    [InlineData("catalog-backport-primary-r4b.json", CatalogType.Primary)]
    [InlineData("catalog-backport-secondary.json", CatalogType.Secondary)]
    [InlineData("catalog-backport-secondary-r4.json", CatalogType.Secondary)]
    [InlineData("catalog-backport-secondary-r4b.json", CatalogType.Secondary)]
    [InlineData("catalog-r4-core-primary.json", CatalogType.Primary)]
    [InlineData("catalog-r4-core-secondary.json", CatalogType.Secondary)]
    [InlineData("catalog-r4-primary.json", CatalogType.Primary)]
    [InlineData("catalog-r4-secondary.json", CatalogType.Secondary)]
    [InlineData("catalog-us-core-primary.json", CatalogType.Primary)]
    [InlineData("catalog-us-core-secondary.json", CatalogType.Secondary)]
    public void ParseCatalogSnippet(string catalogFilename, CatalogType catalogType)
    {
        if (string.IsNullOrWhiteSpace(catalogFilename))
        {
            throw new ArgumentException("Filename must be provided.", nameof(catalogFilename));
        }

        string json = readJsonFile(catalogFilename);
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentException("File contents must not be empty.", nameof(catalogFilename));
        }

        List<RegistryCatalogRecord>? records = JsonSerializer.Deserialize<List<RegistryCatalogRecord>>(json);

        records.ShouldNotBeNull();
        records.Count.ShouldBeGreaterThan(0);

        switch (catalogType)
        {
            case CatalogType.Primary:
                {
                    foreach (RegistryCatalogRecord entry in records)
                    {
                        testCatalogPrimary(entry);
                    }
                }
                break;

            case CatalogType.Secondary:
                {
                    foreach (RegistryCatalogRecord entry in records)
                    {
                        testCatalogSecondary(entry);
                    }
                }
                break;
        }
    }

    private static string readJsonFile(string filename)
    {
        // Get the absolute path to the file
        string path = Path.GetRelativePath(
            Directory.GetCurrentDirectory(),
            Path.Combine("TestData", filename));

        if (!File.Exists(path))
        {
            throw new ArgumentException($"Could not find file at path: {path}");
        }

        return File.ReadAllText(path);
    }

}
