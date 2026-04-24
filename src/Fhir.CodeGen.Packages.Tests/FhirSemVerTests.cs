using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fhir.CodeGen.Packages.Models;
using Hl7.Fhir.Model;
using Shouldly;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Hl7.Fhir.Serialization.FhirSerializationEngineFactory;

namespace Fhir.CodeGen.Packages.Tests;

public class FhirSemVerTests
{
    [Theory]
    [InlineData("*", true)]
    [InlineData("x.x.x", true)]
    [InlineData("1.*", true, 1)]
    [InlineData("1.0.x", true, 1, 0)]
    [InlineData("1.0.*", true, 1, 0)]
    [InlineData("1.x.x", true, 1)]
    [InlineData("1.x.*", true, 1)]
    [InlineData("0.0.4", true, 0, 0, 4)]
    [InlineData("1.2.3", true, 1, 2, 3)]
    [InlineData("10.20.30", true, 10, 20, 30)]
    [InlineData("1.1.2-prerelease+meta", true, 1, 1, 2, "prerelease", "meta")]
    [InlineData("1.1.2+meta", true, 1, 1, 2, null, "meta")]
    [InlineData("1.1.2+meta-valid", true, 1, 1, 2, null, "meta-valid")]
    [InlineData("1.0.0-alpha", true, 1, 0, 0, "alpha")]
    [InlineData("1.0.0-beta", true, 1, 0, 0, "beta")]
    [InlineData("1.0.0-alpha.beta", true, 1, 0, 0, "alpha.beta")]
    [InlineData("1.0.0-alpha.beta.1", true, 1, 0, 0, "alpha.beta.1")]
    [InlineData("1.0.0-alpha.1", true, 1, 0, 0, "alpha.1")]
    [InlineData("1.0.0-alpha0.valid", true, 1, 0, 0, "alpha0.valid")]
    [InlineData("1.0.0-alpha.0valid", true, 1, 0, 0, "alpha.0valid")]
    [InlineData("1.0.0-alpha-a.b-c-somethinglong+build.1-aef.1-its-okay", true, 1, 0, 0, "alpha-a.b-c-somethinglong", "build.1-aef.1-its-okay")]
    [InlineData("1.0.0-rc.1+build.1", true, 1, 0, 0, "rc.1", "build.1")]
    [InlineData("2.0.0-rc.1+build.123", true, 2, 0, 0, "rc.1", "build.123")]
    [InlineData("1.2.3-beta", true, 1, 2, 3, "beta")]
    [InlineData("10.2.3-DEV-SNAPSHOT", true, 10, 2, 3, "DEV-SNAPSHOT")]
    [InlineData("1.2.3-SNAPSHOT-123", true, 1, 2, 3, "SNAPSHOT-123")]
    [InlineData("1.0.0", true, 1, 0, 0)]
    [InlineData("2.0.0", true, 2, 0, 0)]
    [InlineData("1.1.7", true, 1, 1, 7)]
    [InlineData("2.0.0+build.1848", true, 2, 0, 0, null, "build.1848")]
    [InlineData("2.0.1-alpha.1227", true, 2, 0, 1, "alpha.1227")]
    [InlineData("1.0.0-alpha+beta", true, 1, 0, 0, "alpha", "beta")]
    [InlineData("1.2.3----RC-SNAPSHOT.12.9.1--.12+788", true, 1, 2, 3, "---RC-SNAPSHOT.12.9.1--.12", "788")]
    [InlineData("1.2.3----R-S.12.9.1--.12+meta", true, 1, 2, 3, "---R-S.12.9.1--.12", "meta")]
    [InlineData("1.2.3----RC-SNAPSHOT.12.9.1--.12", true, 1, 2, 3, "---RC-SNAPSHOT.12.9.1--.12")]
    [InlineData("1.0.0+0.build.1-rc.10000aaa-kk-0.1", true, 1, 0, 0, null, "0.build.1-rc.10000aaa-kk-0.1")]
    // this is technically valid per semver.org, but the FHIR rules restrict to 32-bit instead of 64-bit
    // [InlineData("99999999999999999999999.999999999999999999.99999999999999999", true, 99999999999999999999999, 999999999999999999, 999999999999999999)]
    [InlineData("1.0.0-0A.is.legal", true, 1, 0, 0, "0A.is.legal")]
    [InlineData("1", false)]
    [InlineData("1.2", false)]
    [InlineData("1.2.3-0123", false)]
    [InlineData("1.2.3-0123.0123", false)]
    [InlineData("1.1.2+.123", false)]
    [InlineData("+invalid", false)]
    [InlineData("-invalid", false)]
    [InlineData("-invalid+invalid", false)]
    [InlineData("-invalid.01", false)]
    [InlineData("alpha", false)]
    [InlineData("alpha.beta", false)]
    [InlineData("alpha.beta.1", false)]
    [InlineData("alpha.1", false)]
    [InlineData("alpha+beta", false)]
    [InlineData("alpha_beta", false)]
    [InlineData("alpha.", false)]
    [InlineData("alpha..", false)]
    [InlineData("beta", false)]
    [InlineData("1.0.0-alpha_beta", false)]
    [InlineData("-alpha.", false)]
    [InlineData("1.0.0-alpha..", false)]
    [InlineData("1.0.0-alpha..1", false)]
    [InlineData("1.0.0-alpha...1", false)]
    [InlineData("1.0.0-alpha....1", false)]
    [InlineData("1.0.0-alpha.....1", false)]
    [InlineData("1.0.0-alpha......1", false)]
    [InlineData("1.0.0-alpha.......1", false)]
    [InlineData("01.1.1", false)]
    [InlineData("1.01.1", false)]
    [InlineData("1.1.01", false)]
    [InlineData("1.2.3.DEV", false)]
    [InlineData("1.2-SNAPSHOT", false)]
    [InlineData("1.2.31.2.3----RC-SNAPSHOT.12.09.1--..12+788", false)]
    [InlineData("1.2-RC-SNAPSHOT", false)]
    [InlineData("-1.0.3-gamma+b7718", false)]
    [InlineData("+justmeta", false)]
    [InlineData("9.8.7+meta+meta", false)]
    [InlineData("9.8.7-whatever+meta+meta", false)]
    [InlineData("99999999999999999999999.999999999999999999.99999999999999999----RC-SNAPSHOT.12.09.1--------------------------------..12", false)]

    public void ParseFhirSemVer(
        string input,
        bool isValid,
        int? major = null,
        int? minor = null,
        int? patch = null,
        string? prerelease = null,
        string? build = null)
    {
        input.ShouldNotBeNullOrWhiteSpace();

        FhirSemVer parsed = new(input);
        parsed.IsValid.ShouldBe(isValid, $"Input '{input}' validity");
        if (isValid)
        {
            parsed.Major.ShouldBe(major, $"Input '{input}' Major");
            parsed.Minor.ShouldBe(minor, $"Input '{input}' Minor");
            parsed.Patch.ShouldBe(patch, $"Input '{input}' Patch");
            parsed.PreRelease.ShouldBe(prerelease, $"Input '{input}' Prerelease");
            parsed.BuildMetadata.ShouldBe(build, $"Input '{input}' Build");
            // round trip
            parsed.ToString().ShouldBe(input, $"Input '{input}' ToString()");
        }
    }

    [Theory]
    [InlineData("1.0.0", "1.0.0", true)]
    [InlineData("1.0.0", "1.0.0-ballot1", true)]
    [InlineData("1.0.x", "1.0.0", true)]
    [InlineData("1.0.x", "1.0.1", true)]
    [InlineData("1.0.x", "1.1.0", false)]
    [InlineData("1.0.*", "1.0.0", true)]
    [InlineData("1.0.*", "1.0.2", true)]
    [InlineData("1.0.*", "1.0.1-ballot1", true)]
    [InlineData("1.0.*", "1.1.0", false)]
    [InlineData("1.x.x", "1.0.0", true)]
    [InlineData("1.x.x", "2.0.0", false)]
    [InlineData("1.*", "1.0.0", true)]
    [InlineData("1.*", "2.0.0", false)]
    [InlineData("*", "1.0.0", true)]
    [InlineData("1.1.x", "1.0.0", false)]
    [InlineData("1.1.*", "1.0.0", false)]
    public void VersionSatisfaction(
        string wildcardVersion,
        string concreteVersion,
        bool shouldSatisfy)
    {
        FhirSemVer wildcard = new(wildcardVersion);
        FhirSemVer concrete = new(concreteVersion);
        concrete.Satisfies(wildcard).ShouldBe(shouldSatisfy);
        wildcard.IsSatisfiedBy(concrete).ShouldBe(shouldSatisfy);
    }

    [Theory]
    [InlineData("1.0.0", "1.0.1", "1.0.2", "1.1.0", "1.2.0", "2.0.0", "2.0.1")]
    [InlineData("1.0.0-cibuild", "1.0.0")]
    [InlineData("1.0.0", "2.0.0-cibuild")]
    [InlineData("1.0.0+001", "1.0.0+002")]
    [InlineData("1.0.0+001", "2.0.0-cibuild+001")]
    [InlineData("1.0.0-other", "1.0.0-cibuild", "1.0.0-snapshot", "1.0.0-draft", "1.0.0-ballot", "1.0.0-final", "1.0.0")]
    [InlineData("1.0.0-ballot1", "1.0.0-ballot2", "1.0.0-ballot10")]
    public void VersionSorting(
        params string[] ascendingSortedVersions)
    {
        ascendingSortedVersions.Length.ShouldBeGreaterThan(1, "At least two versions needed to test sorting");

        List<FhirSemVer> sourceForward = ascendingSortedVersions.Select(s => new FhirSemVer(s)).ToList();
        List<FhirSemVer> sourceReverse = sourceForward.Select(v => v).ToList();
        sourceReverse.Reverse();

        sourceForward.First().ShouldBe(sourceReverse.Last(), "First forward is last reverse");

        List<FhirSemVer> sortedForward = sourceForward.Order(new FhirSemVerComparer()).ToList();
        List<FhirSemVer> sortedReverse = sourceReverse.Order(new FhirSemVerComparer()).ToList();

        sourceForward.Count.ShouldBe(ascendingSortedVersions.Length, "Sorted forward count");
        sourceReverse.Count.ShouldBe(ascendingSortedVersions.Length, "Sorted reverse count");

        for (int i = 0; i < ascendingSortedVersions.Length; i++)
        {
            sortedForward[i].ToString().ShouldBe(ascendingSortedVersions[i], $"Sorted forward index {i}");
            sortedReverse[i].ToString().ShouldBe(ascendingSortedVersions[i], $"Sorted reverse index {i}");
        }
    }

    [Theory]
    [InlineData(7, "1.0.0", "1.0.1", "1.0.2", "1.1.0", "1.2.0", "2.0.0", "2.0.1")]
    [InlineData(1, "1.0.0-cibuild", "1.0.0")]
    [InlineData(1, "1.0.0", "2.0.0-cibuild")]
    [InlineData(1, "1.0.0-other", "1.0.0-cibuild", "1.0.0-snapshot", "1.0.0-draft", "1.0.0-ballot", "1.0.0-final", "1.0.0")]
    [InlineData(0, "1.0.0-ballot1", "1.0.0-ballot2", "1.0.0-ballot10")]
    public void PrereleaseFiltering(
        int releaseCount, params string[] versions)
    {
        versions.Length.ShouldBeGreaterThan(0, "At least one version is required for testing");

        List<FhirSemVer> parsed = versions
            .Select(s => new FhirSemVer(s))
            .Where(sv => sv.IsPrerelease == false)
            .ToList();

        parsed.Count.ShouldBe(releaseCount, "Release count");
    }
}
