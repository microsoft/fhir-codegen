using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Fhir.CodeGen.Packages.Models;
using Fhir.CodeGen.Packages.Tests.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using Shouldly;

namespace Fhir.CodeGen.Packages.Tests;

public class FhirCiModelTests
{
    private class QaRecExpectations
    {
        public required string Id { get; init; }
        public required string IgVersion { get; init; }
        public required string Name { get; init; }
        public required FhirReleases.FhirSequenceCodes FhirSequence { get; init; }
        public string? Url { get; init; } = null;
        public string? GitHubOrg { get; init; } = null;
        public string? GitHubProject { get; init; } = null;
        public string? GitHubBranch { get; init; } = null;
        public DateTimeOffset? RecordDate { get; init; } = null;
        public int? ErrorCount { get; init; } = null;
        public int? WarningCount { get; init; } = null;
        public int? HintCount { get; init; } = null;
    }

    private class QasFileExpectations
    {
        public required int Count { get; init; }
    }

    private static readonly Dictionary<string, QasFileExpectations> _qasFileExpectations = new()
    {
        {
            "qas-backport.json",
            new()
            {
                Count = 2,
            }
        },
        {
            "qas-full.json",
            new()
            {
                Count = 934,
            }
        },
    };

    private static readonly Dictionary<(string id, string? branch), QaRecExpectations> _qaRecExpectations = new()
    {
        {
            ("hl7.fhir.uv.subscriptions-backport", "propose-changes"),
            new()
            {
                Id = "hl7.fhir.uv.subscriptions-backport",
                IgVersion = "1.1.0",
                Name = "SubscriptionsR5Backport",
                FhirSequence = FhirReleases.FhirSequenceCodes.R4B,
                Url = "http://hl7.org/fhir/uv/subscriptions-backport/ImplementationGuide/hl7.fhir.uv.subscriptions-backport",
                GitHubOrg = "HL7",
                GitHubProject = "fhir-subscription-backport-ig",
                GitHubBranch = "propose-changes",
                RecordDate = new DateTimeOffset(2023, 09, 11, 04, 37, 21, TimeSpan.Zero),
                ErrorCount = 0,
                WarningCount = 6,
                HintCount = 0,
            }
        },
        {
            ("hl7.fhir.uv.subscriptions-backport", "master"),
            new()
            {
                Id = "hl7.fhir.uv.subscriptions-backport",
                IgVersion = "1.1.0",
                Name = "SubscriptionsR5Backport",
                FhirSequence = FhirReleases.FhirSequenceCodes.R4B,
                Url = "http://hl7.org/fhir/uv/subscriptions-backport/ImplementationGuide/hl7.fhir.uv.subscriptions-backport",
                GitHubOrg = "HL7",
                GitHubProject = "fhir-subscription-backport-ig",
                GitHubBranch = "master",
                RecordDate = new DateTimeOffset(2023, 01, 09, 22, 29, 01, TimeSpan.Zero),
                ErrorCount = 0,
                WarningCount = 0,
                HintCount = 0,
            }
        },
    };

    private static readonly HashSet<string> _nonSemVerQaPackages = [
        "hl7.fhir.cl.recetachile",
        "hl7.fhir.uv.order-catalog",
        "hl7.cda.us.ccdar2dot2",
        "hl7.cda.us.ccda",
        "hl7.fhir.uv.pocd",
        "hl7.fhir.us.cimilabs",
    ];

    [Theory]
    [InlineData("qas-backport.json")]
    [InlineData("qas-full.json")]
    public void ParseQaRecords(string qasFilename)
    {
        if (string.IsNullOrEmpty(qasFilename))
        {
            throw new ArgumentNullException(nameof(qasFilename));
        }

        if (!_qasFileExpectations.TryGetValue(qasFilename, out QasFileExpectations? qasFileExpectations))
        {
            throw new ArgumentException($"Failed to find expectations for QAs file '{qasFilename}'");
        }

        string qasJson = readJsonFile(qasFilename);

        if (string.IsNullOrEmpty(qasJson))
        {
            throw new ArgumentException(nameof(qasJson));
        }

        List<FhirCiQaRecord>? qaRecords = JsonSerializer.Deserialize<List<FhirCiQaRecord>>(qasJson);

        qaRecords.ShouldNotBeNull();
        qaRecords.Count().ShouldBe(qasFileExpectations.Count);

        foreach (FhirCiQaRecord qa in qaRecords)
        {
            qa.PackageId.ShouldNotBeNullOrWhiteSpace();
            qa.PackageVersion.ShouldNotBeNullOrWhiteSpace();
            qa.Url.ShouldNotBeNullOrWhiteSpace();
            qa.RepositoryUrl.ShouldNotBeNullOrWhiteSpace();

            (string org, string project, string? branch)? ghInfo = qa.ParseGitHubRepo();

            ghInfo.ShouldNotBeNull();

            if (_qaRecExpectations.TryGetValue((qa.PackageId, ghInfo?.branch), out QaRecExpectations? qaExpectations))
            {
                qa.PackageId.ShouldBe(qaExpectations.Id);
                qa.PackageVersion.ShouldBe(qaExpectations.IgVersion);
                qa.Name.ShouldBe(qaExpectations.Name);
                qa.FhirVersion.ShouldNotBeNullOrEmpty();
                FhirReleases.FhirVersionToSequence(qa.FhirVersion).ShouldBe(qaExpectations.FhirSequence);

                if (qaExpectations.Url != null)
                {
                    qa.Url.ShouldBe(qaExpectations.Url);
                }

                if (qaExpectations.GitHubOrg != null)
                {
                    ghInfo?.org.ShouldBe(qaExpectations.GitHubOrg);
                }

                if (qaExpectations.GitHubProject != null)
                {
                    ghInfo?.project.ShouldBe(qaExpectations.GitHubProject);
                }

                if (qaExpectations.GitHubBranch != null)
                {
                    ghInfo?.branch.ShouldBe(qaExpectations.GitHubBranch);
                }

                if (qaExpectations.RecordDate != null)
                {
                    qa.BuildDate.ShouldBe(qaExpectations.RecordDate);
                }

                if (qaExpectations.ErrorCount != null)
                {
                    qa.ErrorCount.ShouldBe(qaExpectations.ErrorCount);
                }

                if (qaExpectations.WarningCount != null)
                {
                    qa.WarningCount.ShouldBe(qaExpectations.WarningCount);
                }

                if (qaExpectations.HintCount != null)
                {
                    qa.HintCount.ShouldBe(qaExpectations.HintCount);
                }
            }

            if (_nonSemVerQaPackages.Contains(qa.PackageId))
            {
                continue;
            }
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
