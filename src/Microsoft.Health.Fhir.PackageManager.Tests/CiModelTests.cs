// <copyright file="FhirDirectiveTests.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Text.Json;
using FluentAssertions;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using Microsoft.Health.Fhir.PackageManager;
using Microsoft.Health.Fhir.PackageManager.Models;
using Microsoft.Health.Fhir.PackageManager.Tests.Extensions;
using Xunit.Abstractions;

using static Microsoft.Health.Fhir.PackageManager.Models.FhirDirective;

namespace Microsoft.Health.Fhir.PackageManager.Tests;

/// <summary>FHIR CI Model tests.</summary>
public class CiModelTests
{
    private const int _expectedQasCount = 934;

    /// <summary>(Immutable) The test output helper.</summary>
    private readonly ITestOutputHelper _testOutputHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="CiModelTests"/> class.
    /// </summary>
    /// <param name="testOutputHelper">(Immutable) The test output helper.</param>
    public CiModelTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [FileData("data/manifest-backport.json")]
    internal void ParseVersionInfoBackport(string json)
    {
        FhirPackageVersionInfo? info = JsonSerializer.Deserialize<FhirPackageVersionInfo>(json);

        info.Should().NotBeNull();

        info!.Name.Should().Be("hl7.fhir.uv.subscriptions-backport");
        info!.Version.Should().Be("1.1.0");
        info!.FhirVersion.Should().Be("4.3.0");
        info!.Date.Should().Be("20221202142832");
    }

    [Theory]
    [FileData("data/qas-backport.json")]
    internal void ParseQasBackport(string json)
    {
        FhirQasRec[]? qas = JsonSerializer.Deserialize<FhirQasRec[]>(json);

        qas.Should().NotBeNullOrEmpty();
        qas!.Length.Should().Be(2);

        qas![0].Url.Should().Be("http://hl7.org/fhir/uv/subscriptions-backport/ImplementationGuide/hl7.fhir.uv.subscriptions-backport");
        qas![0].Name.Should().Be("SubscriptionsR5Backport");
        qas![0].PackageId.Should().Be("hl7.fhir.uv.subscriptions-backport");
        qas![0].GuideVersion.Should().Be("1.1.0");
        qas![0].FhirVersion.Should().Be("4.3.0");
        qas![0].RepositoryUrl.Should().Be("HL7/fhir-subscription-backport-ig/branches/propose-changes/qa.json");

        qas![1].Url.Should().Be("http://hl7.org/fhir/uv/subscriptions-backport/ImplementationGuide/hl7.fhir.uv.subscriptions-backport");
        qas![1].Name.Should().Be("SubscriptionsR5Backport");
        qas![1].PackageId.Should().Be("hl7.fhir.uv.subscriptions-backport");
        qas![1].GuideVersion.Should().Be("1.1.0");
        qas![1].FhirVersion.Should().Be("4.3.0");
        qas![1].RepositoryUrl.Should().Be("HL7/fhir-subscription-backport-ig/branches/master/qa.json");
    }

    [Theory]
    [FileData("data/qas-full.json")]
    internal void ParseQasFull(string json)
    {
        FhirQasRec[]? qas = JsonSerializer.Deserialize<FhirQasRec[]>(json);

        qas.Should().NotBeNullOrEmpty();
        qas!.Length.Should().Be(_expectedQasCount);

        foreach (FhirQasRec rec in qas)
        {
            rec.Url.Should().NotBeNullOrEmpty();
            //rec.Name.Should().NotBeNullOrEmpty($"Missing name for {rec.Url}");
            rec.PackageId.Should().NotBeNullOrEmpty();
            if (rec.PackageId.StartsWith("hl7.", StringComparison.Ordinal))
            {
                string directive = rec.PackageId + "#" + rec.GuideVersion;

                // check for exceptions
                switch (directive)
                {
                    // do not test
                    case "hl7.fhir.cl.recetachile#0.9":
                    case "hl7.fhir.uv.order-catalog#current":
                    case "hl7.cda.us.ccdar2dot2#2.2":
                    case "hl7.cda.us.ccda#2.1":
                    case "hl7.fhir.uv.pocd#current":
                    case "hl7.fhir.us.cimilabs#0.1":
                        break;

                    default:
                        FhirCache._matchSemver.IsMatch(rec.GuideVersion).Should().BeTrue($"HL7 packages should use SemVer ({rec.PackageId}#{rec.GuideVersion})");
                        break;
                }
            }
            rec.GuideVersion.Should().NotBeNullOrEmpty();
            rec.FhirVersion.Should().NotBeNullOrEmpty();
            FhirReleases.FhirVersionToSequence(rec.FhirVersion).Should().NotBe(FhirReleases.FhirSequenceCodes.Unknown);
            rec.RepositoryUrl.Should().NotBeNullOrEmpty();
        }
    }
}
