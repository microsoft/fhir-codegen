using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Fhir.CodeGen.Packages.Models;

public partial record class FhirSemVer
{
    /// <summary>
    /// Regular expression to parse a *mostly* standard SemVer string.
    /// </summary>
    /// <remarks>
    /// ^ - start of string
    /// (?<major>0|[1-9Xx\\d*) - major version number or single-segment wildcard
    /// \\.(?<minor>0|[1-9Xx]\\d*) - minor version number or single-segment wildcard
    /// \\.(?<patch>0|[1-9Xx]\\d*) - patch version number or single-segment wildcard
    /// (?:-(?<prerelease>(?:0|[1-9]\\d*|\\d*[a-zA-Z-][0-9a-zA-Z-]*) - pre-release version identifier
    /// (?:\\.(?:0|[1-9]\\d*|\\d*[a-zA-Z-][0-9a-zA-Z-]*))*))? - additional pre-release identifiers
    /// (?:\\+(?<buildmetadata>[0-9a-zA-Z-]+(?:\\.[0-9a-zA-Z-]+)*))? - build metadata
    /// $ - end of string
    /// </remarks>
    /// <returns></returns>
    [GeneratedRegex("^(?<major>0|[1-9Xx]\\d*)\\.(?<minor>0|[1-9Xx]\\d*)\\.(?<patch>0|[1-9Xx]\\d*)(?:-(?<prerelease>(?:0|[1-9]\\d*|\\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\\.(?:0|[1-9]\\d*|\\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\\+(?<buildmetadata>[0-9a-zA-Z-]+(?:\\.[0-9a-zA-Z-]+)*))?$")]
    private static partial Regex parseFullFhirSemVerExpression();

    private static Regex _parseSemVerExpression = parseFullFhirSemVerExpression();

    public string? SourceString { get; init; } = null;

    public int? Major { get; init; } = null;
    public int? Minor { get; init; } = null;
    public int? Patch { get; init; } = null;
    public string? PreRelease { get; init; } = null;
    public string? BuildMetadata { get; init; } = null;

    public bool IsValid { get; init; } = true;

    public bool MajorIsWildcard => (Major == null);
    public bool MinorIsWildcard => (Minor == null);
    public bool PatchIsWildcard => (Patch == null);
    public bool IsPrerelease => (PreRelease != null);


    public override string ToString()
    {
        if (SourceString != null)
        {
            return SourceString;
        }

        if (!IsValid)
        {
            return $"Invalid SemVer: {Major}.{Minor}.{Patch}" +
                (PreRelease == null ? string.Empty : ($"-{PreRelease}")) +
                (BuildMetadata == null ? string.Empty : ($"+{BuildMetadata}"));
        }

        if (Major == null && Minor == null && Patch == null)
        {
            return "*";
        }

        if (Major != null && Minor == null && Patch == null)
        {
            return $"{Major}.*";
        }

        if (Major != null && Minor != null && Patch == null)
        {
            return $"{Major}.{Minor}.*";
        }

        return $"{Major ?? 0}.{Minor ?? 0}.{Patch ?? 0}" +
            (PreRelease == null ? string.Empty : ($"-{PreRelease}")) +
            (BuildMetadata == null ? string.Empty : ($"+{BuildMetadata}"));
    }

    public FhirSemVer(string semVerString)
    {
        if (string.IsNullOrWhiteSpace(semVerString))
        {
            IsValid = false;
            return;
        }

        SourceString = semVerString;

        // check for the presence of an `*` wildcard, which is terminal and results in non-parseable SemVer
        if (semVerString.Contains('*'))
        {
            string[] components = semVerString.Split('.');
            switch (components.Length)
            {
                case 1:
                    {
                        if (components[0] == "*")
                        {
                            Major = null;
                            Minor = null;
                            Patch = null;
                            PreRelease = null;
                            BuildMetadata = null;
                            IsValid = true;
                            return;
                        }

                        IsValid = false;
                        return;
                    }

                case 2:
                    {
                        if (components[1] == "*")
                        {
                            switch (components[0])
                            {
                                case "*":
                                case "x":
                                case "X":
                                    Major = null;
                                    break;

                                default:
                                    {
                                        if (int.TryParse(components[0], out int intVal) == false)
                                        {
                                            IsValid = false;
                                            return;
                                        }

                                        Major = intVal;
                                    }
                                    break;
                            }

                            Minor = null;
                            Patch = null;
                            PreRelease = null;
                            BuildMetadata = null;
                            IsValid = true;

                            return;
                        }

                        IsValid = false;
                        return;
                    }

                case 3:
                    {
                        if (components[2] == "*")
                        {
                            switch (components[0])
                            {
                                case "*":
                                case "x":
                                case "X":
                                    break;

                                default:
                                    {
                                        if (int.TryParse(components[0], out int intVal) == false)
                                        {
                                            IsValid = false;
                                            return;
                                        }

                                        Major = intVal;
                                    }
                                    break;
                            }

                            switch (components[1])
                            {
                                case "x":
                                case "X":
                                case "*":
                                    Minor = null;
                                    break;

                                default:
                                    {
                                        if (int.TryParse(components[1], out int intVal) == false)
                                        {
                                            IsValid = false;
                                            return;
                                        }

                                        Minor = intVal;
                                    }
                                    break;
                            }


                            Patch = null;
                            PreRelease = null;
                            BuildMetadata = null;
                            IsValid = true;

                            return;
                        }

                        IsValid = false;
                        return;
                    }

                default:
                    {
                        // asterisk is only legal as a wildcard - cannot appear in labels, etc.
                        IsValid = false;
                        return;
                    }
            }

        }

        Match match = _parseSemVerExpression.Match(semVerString.Trim());
        if (!match.Success)
        {
            IsValid = false;
            return;
        }

        Major = int.TryParse(match.Groups["major"].Value, out int major) ? major : null;
        Minor = int.TryParse(match.Groups["minor"].Value, out int minor) ? minor : null;
        Patch = int.TryParse(match.Groups["patch"].Value, out int patch) ? patch : null;

        PreRelease = match.Groups["prerelease"].Success ? match.Groups["prerelease"].Value : null;
        BuildMetadata = match.Groups["buildmetadata"].Success ? match.Groups["buildmetadata"].Value : null;
    }

    public bool Satisfies(FhirSemVer other)
    {
        if (other == null || !other.IsValid || !IsValid)
        {
            return false;
        }

        // Major version must match or be a wildcard
        if (!other.MajorIsWildcard)
        {
            if (MajorIsWildcard)
            {
                return false;
            }
            if (Major != other.Major)
            {
                return false;
            }
        }

        // Minor version must match or be a wildcard
        if (!other.MinorIsWildcard)
        {
            if (MinorIsWildcard)
            {
                return false;
            }
            if (Minor != other.Minor)
            {
                return false;
            }
        }

        // Patch version must match or be a wildcard
        if (!other.PatchIsWildcard)
        {
            if (PatchIsWildcard)
            {
                return false;
            }
            if (Patch != other.Patch)
            {
                return false;
            }
        }

        // Pre-release and build metadata are not considered for satisfaction
        return true;
    }

    public bool IsSatisfiedBy(FhirSemVer other)
    {
        if (other == null || !other.IsValid || !IsValid)
        {
            return false;
        }

        return other.Satisfies(this);
    }
}

public class FhirSemVerComparer :
    IEqualityComparer, IComparer,
    IEqualityComparer<FhirSemVer>, IComparer<FhirSemVer>
{
    // "final[N]" - a stable release, where N is optional and only used to distinguish between multiple final releases (e.g., for errata)
    // "ballot[N]" - a frozen release used in the ballot process
    // "draft[N]" - a frozen release put out for non - ballot review or QA.
    // "snapshot[N]" - a frozen release of a specification for connectathon, ballot dependencies or other reasons
    // "cibuild" - a 'special' release label that refers to a non - stable release that changes with each commit.
    private enum FhirTagCodes : int
    {
        Other = 0,
        CiBuild = 1,
        Snapshot = 2,
        Draft = 3,
        Ballot = 4,
        Final = 5,
    }

    public int Compare(FhirSemVer? x, FhirSemVer? y)
    {
        if (ReferenceEquals(x, y))
        {
            return 0;
        }

        if (x is null)
        {
            return -1;
        }

        if (y is null)
        {
            return 1;
        }

        // Validity: valid > invalid (invalids sort last)
        if (x.IsValid != y.IsValid)
        {
            return x.IsValid ? -1 : 1;
        }

        // If both invalid, compare input representations (stable, deterministic)
        if (!x.IsValid && !y.IsValid)
        {
            string xs = x.SourceString ?? x.ToString();
            string ys = y.SourceString ?? y.ToString();
            return string.Compare(xs, ys, StringComparison.Ordinal);
        }

        // Major comparison (null = wildcard, sorts before concrete)
        int c = compareNullableInt(x.Major, y.Major);
        if (c != 0)
        {
            return c;
        }

        // Minor comparison
        c = compareNullableInt(x.Minor, y.Minor);
        if (c != 0)
        {
            return c;
        }

        // Patch comparison
        c = compareNullableInt(x.Patch, y.Patch);
        if (c != 0)
        {
            return c;
        }

        //// Pre-release comparison (SemVer rules)
        //if (x.PreRelease == y.PreRelease)
        //{
        //    return 0; // exact same pre-release state
        //}

        // A version without pre-release has higher precedence than one with pre-release
        if ((x.PreRelease is null) && (y.PreRelease is not null))
        {
            return 1; // x (release) > y (pre-release)
        }

        if ((y.PreRelease is null) && (x.PreRelease is not null))
        {
            return -1; // x (pre-release) < y (release)
        }

        // Both have pre-release: compare identifiers per FHIR-SemVer (tags are sorted oddly)
        if ((x.PreRelease is not null) && (y.PreRelease is not null))
        {
            c = comparePreRelease(x.PreRelease, y.PreRelease);
            if (c != 0)
            {
                return c;
            }
        }

        if ((x.BuildMetadata is null) && (y.BuildMetadata is not null))
        {
            return -1; // x (no build) < y (build)
        }

        if ((y.BuildMetadata is null) && (x.BuildMetadata is not null))
        {
            return 1; // x (build) > y (no build)
        }

        if ((x.BuildMetadata is not null) && (y.BuildMetadata is not null))
        {
            // Both have build metadata: compare lexically (per SemVer, build does not affect precedence, but we need a stable order)
            c = string.Compare(x.BuildMetadata, y.BuildMetadata, StringComparison.Ordinal);
            if (c != 0)
            {
                return c;
            }
        }

        // all tests are equal
        return 0;
    }

    public int Compare(object? x, object? y)
    {
        if (ReferenceEquals(x, y))
        {
            return 0;
        }

        if (x is null)
        {
            return -1;
        }

        if (y is null)
        {
            return 1;
        }

        // Support FhirSemVer and string inputs
        if (x is FhirSemVer fx && y is FhirSemVer fy)
        {
            return Compare(fx, fy);
        }

        if (x is string xs && y is string ys)
        {
            return Compare(new FhirSemVer(xs), new FhirSemVer(ys));
        }

        throw new ArgumentException("FhirSemVerComparer can only compare FhirSemVer or string instances.");
    }

    public bool Equals(FhirSemVer? x, FhirSemVer? y)
    {
        return Compare(x, y) == 0;
    }

    public int GetHashCode(FhirSemVer obj)
    {
        if (obj is null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        if (!obj.IsValid)
        {
            // For invalid, hash on validity + source/toString to keep consistency with Compare
            return HashCode.Combine(false, obj.SourceString ?? obj.ToString());
        }

        // Ignore BuildMetadata for equality/ordering (SemVer precedence)
        // Treat wildcard (null) components with a sentinel to differentiate from concrete 0
        int major = obj.Major ?? int.MinValue;
        int minor = obj.Minor ?? int.MinValue;
        int patch = obj.Patch ?? int.MinValue;
        string? pre = obj.PreRelease; // case-sensitive per SemVer ASCII order

        return HashCode.Combine(major, minor, patch, pre);
    }

    bool IEqualityComparer.Equals(object? x, object? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }

        if (x is FhirSemVer fx && y is FhirSemVer fy)
        {
            return Equals(fx, fy);
        }

        if (x is string xs && y is string ys)
        {
            return Equals(new FhirSemVer(xs), new FhirSemVer(ys));
        }

        return false;
    }

    int IEqualityComparer.GetHashCode(object obj)
    {
        if (obj is null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        if (obj is FhirSemVer sv)
        {
            return GetHashCode(sv);
        }

        if (obj is string s)
        {
            return GetHashCode(new FhirSemVer(s));
        }

        // Fallback to object's own hash (not ideal for mixed sets, but avoids throwing in general collections)
        return obj.GetHashCode();
    }

    private static int compareNullableInt(int? a, int? b)
    {
        if (a == b)
        {
            return 0;
        }

        if (a is null)
        {
            return -1; // wildcard sorts before concrete
        }

        if (b is null)
        {
            return 1;
        }

        return a.Value.CompareTo(b.Value);
    }

    private static int comparePreRelease(string a, string b)
    {
        // Split into identifiers
        string[] ap = a.Split('.', StringSplitOptions.RemoveEmptyEntries);
        string[] bp = b.Split('.', StringSplitOptions.RemoveEmptyEntries);

        int len = Math.Min(ap.Length, bp.Length);
        for (int i = 0; i < len; i++)
        {
            string ai = ap[i];
            string bi = bp[i];

            bool aiNum = int.TryParse(ai, out int aiVal);
            bool biNum = int.TryParse(bi, out int biVal);

            if (aiNum && biNum)
            {
                int c = aiVal.CompareTo(biVal);
                if (c != 0)
                {
                    return c;
                }

                continue;
            }

            if (aiNum != biNum)
            {
                // Numeric identifiers have lower precedence than non-numeric
                return aiNum ? -1 : 1;
            }

            // "final[N]" - a stable release, where N is optional and only used to distinguish between multiple final releases (e.g., for errata)
            // "ballot[N]" - a frozen release used in the ballot process
            // "draft[N]" - a frozen release put out for non - ballot review or QA.
            // "snapshot[N]" - a frozen release of a specification for connectathon, ballot dependencies or other reasons
            // "cibuild" - a 'special' release label that refers to a non - stable release that changes with each commit.

            FhirTagCodes atc;
            int aiPrefixLen;

            if (ai.StartsWith("final", StringComparison.OrdinalIgnoreCase))
            {
                atc = FhirTagCodes.Final;
                aiPrefixLen = "final".Length;
            }
            else if (ai.StartsWith("ballot", StringComparison.OrdinalIgnoreCase))
            {
                atc = FhirTagCodes.Ballot;
                aiPrefixLen = "ballot".Length;
            }
            else if (ai.StartsWith("draft", StringComparison.OrdinalIgnoreCase))
            {
                atc = FhirTagCodes.Draft;
                aiPrefixLen = "draft".Length;
            }
            else if (ai.StartsWith("snapshot", StringComparison.OrdinalIgnoreCase))
            {
                atc = FhirTagCodes.Snapshot;
                aiPrefixLen = "snapshot".Length;
            }
            else if (ai.Equals("cibuild", StringComparison.OrdinalIgnoreCase))
            {
                atc = FhirTagCodes.CiBuild;
                aiPrefixLen = "cibuild".Length;
            }
            else
            {
                atc = FhirTagCodes.Other;
                aiPrefixLen = -1;
            }

            FhirTagCodes btc;
            int biPrefixLen;

            if (bi.StartsWith("final", StringComparison.OrdinalIgnoreCase))
            {
                btc = FhirTagCodes.Final;
                biPrefixLen = "final".Length;
            }
            else if (bi.StartsWith("ballot", StringComparison.OrdinalIgnoreCase))
            {
                btc = FhirTagCodes.Ballot;
                biPrefixLen = "ballot".Length;
            }
            else if (bi.StartsWith("draft", StringComparison.OrdinalIgnoreCase))
            {
                btc = FhirTagCodes.Draft;
                biPrefixLen = "draft".Length;
            }
            else if (bi.StartsWith("snapshot", StringComparison.OrdinalIgnoreCase))
            {
                btc = FhirTagCodes.Snapshot;
                biPrefixLen = "snapshot".Length;
            }
            else if (bi.Equals("cibuild", StringComparison.OrdinalIgnoreCase))
            {
                btc = FhirTagCodes.CiBuild;
                biPrefixLen = "cibuild".Length;
            }
            else
            {
                btc = FhirTagCodes.Other;
                biPrefixLen = -1;
            }

            // if we have different tag classes, we can compare directly
            if (atc != btc)
            {
                return ((int)atc).CompareTo((int)btc);
            }

            // when there is the same tag, we need to extract the numeric suffix (if any) and compare

            int aiSuffix = aiPrefixLen > 0
                ? int.TryParse(ai.AsSpan(aiPrefixLen), out int aiNumVal) ? aiNumVal : -1
                : -1;
            int biSuffix = biPrefixLen > 0
                ? int.TryParse(bi.AsSpan(biPrefixLen), out int biNumVal) ? biNumVal : -1
                : -1;

            int numC = aiSuffix.CompareTo(biSuffix);
            if (numC != 0)
            {
                return numC;
            }

            // if there were no numeric suffixes or they are equal, do a standard string on the tags to check for whatever is categorized as "Other"
            int sc = string.Compare(ai, bi, StringComparison.Ordinal);
            if (sc != 0)
            {
                return sc;
            }
        }

        // If all equal so far, shorter set has lower precedence
        return ap.Length.CompareTo(bp.Length);
    }
}
