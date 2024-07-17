// <copyright file="FhirReleases.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Health.Fhir.CodeGenCommon.Packaging;

/// <summary>FHIR release information and utilities.</summary>
public static class FhirReleases
{
    /// <summary>Values that represent FHIR major releases.</summary>
    public enum FhirSequenceCodes : int
    {
        /// <summary>Unknown FHIR Version.</summary>
        Unknown = 0,

        /// <summary>FHIR DSTU2.</summary>
        DSTU2 = 1,

        /// <summary>FHIR STU3.</summary>
        STU3 = 2,

        /// <summary>FHIR R4.</summary>
        R4 = 3,

        /// <summary>FHIR R4B.</summary>
        R4B = 4,

        /// <summary>FHIR R5.</summary>
        R5 = 5,

        /// <summary>FHIR R5.</summary>
        R6 = 6,
    }

    /// <summary>Information about the published release.</summary>
    public readonly record struct PublishedReleaseInformation(
        FhirSequenceCodes Sequence,
        DateOnly PublicationDate,
        bool IsSequenceOfficial,
        string Version,
        string Description,
        string? BallotPrefix = null)
    {
        public override string ToString() => Description;
    };

    /// <summary>The FHIR releases.</summary>
    public static readonly FrozenDictionary<string, PublishedReleaseInformation> FhirPublishedVersions = new Dictionary<string, PublishedReleaseInformation>()
    {
        { "1.0.2",             new (FhirSequenceCodes.DSTU2, new DateOnly(2015, 10, 24), true, "1.0.2",             "DSTU2 Release with 1 technical errata") },
        { "3.0.2",             new (FhirSequenceCodes.STU3,  new DateOnly(2019, 10, 24), true, "3.0.2",             "STU3 Release with 2 technical errata") },
        { "3.2.0",             new (FhirSequenceCodes.R4,    new DateOnly(2018, 04, 02), false, "3.2.0",             "R4 Draft for comment / First Candidate Normative Content", "2018Jan") },
        { "3.3.0",             new (FhirSequenceCodes.R4,    new DateOnly(2018, 05, 02), false, "3.3.0",             "R4 Ballot #1 : Mixed Normative/Trial use (First Normative ballot)", "2018May") },
        { "3.5.0",             new (FhirSequenceCodes.R4,    new DateOnly(2018, 08, 21), false, "3.5.0",             "R4 Ballot #2 : Mixed Normative/Trial use (Second Normative ballot + Baltimore Connectathon)", "2018Sep") },
        { "3.5a.0",            new (FhirSequenceCodes.R4,    new DateOnly(2018, 11, 09), false, "3.5a.0",            "Special R4 Ballot #3 : Normative Packages for Terminology / Conformance + Observation", "2018Dec") },
        { "4.0.1",             new (FhirSequenceCodes.R4,    new DateOnly(2019, 10, 30), true, "4.0.1",             "R4 Release with 1 technical errata") },
        { "4.1.0",             new (FhirSequenceCodes.R4B,   new DateOnly(2021, 03, 11), false, "4.1.0",             "R4B Ballot #1", "2021Mar") },
        { "4.3.0-snapshot1",   new (FhirSequenceCodes.R4B,   new DateOnly(2021, 12, 20), false, "4.3.0-snapshot1",   "R4B January 2022 Connectathon") },
        { "4.3.0",             new (FhirSequenceCodes.R4B,   new DateOnly(2022, 05, 28), true, "4.3.0",             "R4B Release") },
        { "4.2.0",             new (FhirSequenceCodes.R5,    new DateOnly(2019, 12, 31), false, "4.2.0",             "R5 Preview #1", "2020Feb") },
        { "4.4.0",             new (FhirSequenceCodes.R5,    new DateOnly(2020, 05, 04), false, "4.4.0",             "R5 Preview #2", "2020May") },
        { "4.5.0",             new (FhirSequenceCodes.R5,    new DateOnly(2020, 08, 20), false, "4.5.0",             "R5 Preview #3", "2020Sep") },
        { "4.6.0",             new (FhirSequenceCodes.R5,    new DateOnly(2021, 04, 15), false, "4.6.0",             "R5 Draft Ballot", "2021May") },
        { "5.0.0-snapshot1",   new (FhirSequenceCodes.R5,    new DateOnly(2021, 12, 19), false, "5.0.0-snapshot1",   "R5 January 2022 Connectathon") },
        { "5.0.0-ballot",      new (FhirSequenceCodes.R5,    new DateOnly(2022, 09, 10), false, "5.0.0-ballot",      "R5 Ballot #1") },
        { "5.0.0-snapshot3",   new (FhirSequenceCodes.R5,    new DateOnly(2022, 12, 14), false, "5.0.0-snapshot3",   "R5 Connectathon 32 Base") },
        { "5.0.0-draft-final", new (FhirSequenceCodes.R5,    new DateOnly(2023, 03, 01), false, "5.0.0-draft-final", "R5 Final QA") },
        { "5.0.0",             new (FhirSequenceCodes.R5,    new DateOnly(2023, 03, 26), true, "5.0.0",             "R5 Release")  },
    }.ToFrozenDictionary();

    /// <summary>(Immutable) The FHIR sequence map.</summary>
    private static readonly FrozenDictionary<string, FhirSequenceCodes> _fhirSequenceMap = new Dictionary<string, FhirSequenceCodes>()
    {
        // unknown mapping (for performance)
        { "", FhirSequenceCodes.Unknown },
        { "Unknown", FhirSequenceCodes.Unknown },

        // DSTU2
        { "DSTU2", FhirSequenceCodes.DSTU2 },
        { "R2", FhirSequenceCodes.DSTU2 },
        { "2", FhirSequenceCodes.DSTU2 },
        { "0.4", FhirSequenceCodes.DSTU2 },
        { "0.4.0", FhirSequenceCodes.DSTU2 },
        { "0.5", FhirSequenceCodes.DSTU2 },
        { "0.5.0", FhirSequenceCodes.DSTU2 },
        { "1.0", FhirSequenceCodes.DSTU2 },
        { "1.0.0", FhirSequenceCodes.DSTU2 },
        { "1.0.1", FhirSequenceCodes.DSTU2 },
        { "1.0.2", FhirSequenceCodes.DSTU2 },
        { "hl7.fhir.r2", FhirSequenceCodes.DSTU2 },
        { "hl7.fhir.r2.core", FhirSequenceCodes.DSTU2 },

        // STU3
        { "STU3", FhirSequenceCodes.STU3 },
        { "R3", FhirSequenceCodes.STU3 },
        { "3", FhirSequenceCodes.STU3 },
        { "1.1", FhirSequenceCodes.STU3 },
        { "1.1.0", FhirSequenceCodes.STU3 },
        { "1.2", FhirSequenceCodes.STU3 },
        { "1.2.0", FhirSequenceCodes.STU3 },
        { "1.4", FhirSequenceCodes.STU3 },
        { "1.4.0", FhirSequenceCodes.STU3 },
        { "1.6", FhirSequenceCodes.STU3 },
        { "1.6.0", FhirSequenceCodes.STU3 },
        { "1.8", FhirSequenceCodes.STU3 },
        { "1.8.0", FhirSequenceCodes.STU3 },
        { "3.0", FhirSequenceCodes.STU3 },
        { "3.0.0", FhirSequenceCodes.STU3 },
        { "3.0.1", FhirSequenceCodes.STU3 },
        { "3.0.2", FhirSequenceCodes.STU3 },
        { "hl7.fhir.r3", FhirSequenceCodes.STU3 },
        { "hl7.fhir.r3.core", FhirSequenceCodes.STU3 },

        // R4
        { "R4", FhirSequenceCodes.R4 },
        { "4", FhirSequenceCodes.R4 },
        { "3.2", FhirSequenceCodes.R4 },
        { "3.2.0", FhirSequenceCodes.R4 },
        { "3.3", FhirSequenceCodes.R4 },
        { "3.3.0", FhirSequenceCodes.R4 },
        { "3.5", FhirSequenceCodes.R4 },
        { "3.5.0", FhirSequenceCodes.R4 },
        { "3.5a", FhirSequenceCodes.R4 },
        { "3.5a.0", FhirSequenceCodes.R4 },
        { "4.0", FhirSequenceCodes.R4 },
        { "4.0.1", FhirSequenceCodes.R4 },
        { "hl7.fhir.r4", FhirSequenceCodes.R4 },
        { "hl7.fhir.r4.core", FhirSequenceCodes.R4 },

        // R4B
        { "R4B", FhirSequenceCodes.R4B },
        { "4B", FhirSequenceCodes.R4B },
        { "4.1", FhirSequenceCodes.R4B },
        { "4.1.0", FhirSequenceCodes.R4B },
        { "4.3", FhirSequenceCodes.R4B },
        { "4.3.0", FhirSequenceCodes.R4B },
        { "4.3.0-snapshot1", FhirSequenceCodes.R4B },
        { "hl7.fhir.r4b", FhirSequenceCodes.R4B },
        { "hl7.fhir.r4b.core", FhirSequenceCodes.R4B },

        // R5
        { "R5", FhirSequenceCodes.R5 },
        { "5", FhirSequenceCodes.R5 },
        { "4.2", FhirSequenceCodes.R5 },
        { "4.2.0", FhirSequenceCodes.R5 },
        { "4.4", FhirSequenceCodes.R5 },
        { "4.4.0", FhirSequenceCodes.R5 },
        { "4.5", FhirSequenceCodes.R5 },
        { "4.5.0", FhirSequenceCodes.R5 },
        { "4.6", FhirSequenceCodes.R5 },
        { "4.6.0", FhirSequenceCodes.R5 },
        { "5.0", FhirSequenceCodes.R5 },
        { "5.0.0", FhirSequenceCodes.R5 },
        { "5.0.0-cibuild", FhirSequenceCodes.R5 },
        { "5.0.0-snapshot1", FhirSequenceCodes.R5 },
        { "5.0.0-ballot", FhirSequenceCodes.R5 },
        { "5.0.0-snapshot3", FhirSequenceCodes.R5 },
        { "5.0.0-draft-final", FhirSequenceCodes.R5 },
        { "hl7.fhir.r5", FhirSequenceCodes.R5 },
        { "hl7.fhir.r5.core", FhirSequenceCodes.R5 },

        // R6
        { "R6", FhirSequenceCodes.R6 },
        { "6", FhirSequenceCodes.R6 },
        { "6.0", FhirSequenceCodes.R6 },
        { "6.0.0", FhirSequenceCodes.R6 },
        { "6.0.0-cibuild", FhirSequenceCodes.R6 },
        { "hl7.fhir.r6", FhirSequenceCodes.R6 },
        { "hl7.fhir.r6.core", FhirSequenceCodes.R6 },

    }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

    /// <summary>Attempts to get sequence the FhirSequenceCodes from the given string.</summary>
    /// <param name="literal">     The literal.</param>
    /// <param name="fhirSequence">[out] The FHIR sequence.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public static bool TryGetSequence(string literal, [NotNullWhen(true)] out FhirSequenceCodes fhirSequence)
    {
        if (_fhirSequenceMap.TryGetValue(literal, out fhirSequence))
        {
            return true;
        }

        if (literal.Contains('#'))
        {
            if (_fhirSequenceMap.TryGetValue(literal.Substring(0, literal.IndexOf('#')), out fhirSequence))
            {
                return true;
            }
        }

        if (literal.Contains('-'))
        {
            if (_fhirSequenceMap.TryGetValue(literal.Substring(0, literal.IndexOf('-')), out fhirSequence))
            {
                return true;
            }
        }

        fhirSequence = FhirSequenceCodes.Unknown;
        return false;
    }

    /// <summary>
    /// Converts a FHIR version string (number, literal, r-literal) to a sequence code.
    /// </summary>
    /// <param name="version">The version string.</param>
    /// <returns>The FhirSequenceCodes.</returns>
    public static FhirSequenceCodes FhirVersionToSequence(string version)
    {
        if (_fhirSequenceMap.TryGetValue(version, out FhirSequenceCodes sequence))
        {
            return sequence;
        }

        // check for a tag we do not know
        if (version.Contains('-'))
        {
            if (_fhirSequenceMap.TryGetValue(version.Substring(0, version.IndexOf('-')), out FhirSequenceCodes sequence2))
            {
                return sequence2;
            }
        }

        return FhirSequenceCodes.Unknown;
    }

    /// <summary>Convert a FHIR sequence code to the literal for that version.</summary>
    /// <param name="sequence">The sequence.</param>
    /// <returns>A string.</returns>
    public static string ToLiteral(this FhirSequenceCodes sequence) => sequence switch
    {
        FhirSequenceCodes.DSTU2 => "DSTU2",
        FhirSequenceCodes.STU3 => "STU3",
        FhirSequenceCodes.R4 => "R4",
        FhirSequenceCodes.R4B => "R4B",
        FhirSequenceCodes.R5 => "R5",
        FhirSequenceCodes.R6 => "R6",
        _ => "Unknown"
    };

    /// <summary>Converts a fhir version string to a literal.</summary>
    /// <param name="version">[out] The version literal (e.g., DSTU2).</param>
    /// <returns>Sequence as a string.</returns>
    public static string FhirVersionToLiteral(string version)
    {
        if (_fhirSequenceMap.TryGetValue(version, out FhirSequenceCodes sequence))
        {
            return sequence.ToLiteral();
        }

        // check for a tag we do not know
        if (version.Contains('-'))
        {
            if (_fhirSequenceMap.TryGetValue(version.Substring(0, version.IndexOf('-')), out FhirSequenceCodes sequence2))
            {
                return sequence2.ToLiteral();
            }
        }

        return string.Empty;
    }

    /// <summary>Converts a sequence to a r literal.</summary>
    /// <param name="sequence">The sequence.</param>
    /// <returns>Sequence as a string.</returns>
    public static string ToRLiteral(this FhirSequenceCodes sequence) => sequence switch
    {
        FhirSequenceCodes.DSTU2 => "R2",
        FhirSequenceCodes.STU3 => "R3",
        FhirSequenceCodes.R4 => "R4",
        FhirSequenceCodes.R4B => "R4B",
        FhirSequenceCodes.R5 => "R5",
        FhirSequenceCodes.R6 => "R6",
        _ => "Unknown"
    };

    /// <summary>Converts a fhir version string to an R-literal.</summary>
    /// <param name="version">[out] The R-Literal string (e.g., R4).</param>
    /// <returns>Sequence as a string.</returns>
    public static string FhirVersionToRLiteral(string version)
    {
        if (_fhirSequenceMap.TryGetValue(version, out FhirSequenceCodes sequence))
        {
            return sequence.ToRLiteral();
        }

        // check for a tag we do not know
        if (version.Contains('-'))
        {
            if (_fhirSequenceMap.TryGetValue(version.Substring(0, version.IndexOf('-')), out FhirSequenceCodes sequence2))
            {
                return sequence2.ToRLiteral();
            }
        }

        return string.Empty;
    }

    /// <summary>Converts a sequence to a short version.</summary>
    /// <param name="sequence">The sequence.</param>
    /// <returns>Sequence as a string.</returns>
    public static string ToShortVersion(this FhirSequenceCodes sequence) => sequence switch
    {
        FhirSequenceCodes.DSTU2 => "1.0",
        FhirSequenceCodes.STU3 => "3.0",
        FhirSequenceCodes.R4 => "4.0",
        FhirSequenceCodes.R4B => "4.3",
        FhirSequenceCodes.R5 => "5.0",
        FhirSequenceCodes.R6 => "6.0",
        _ => "Unknown"
    };

    /// <summary>Converts a fhir version string to a short version number.</summary>
    /// <param name="version">[out] The version string (e.g., 4.0.1).</param>
    /// <returns>Sequence as a string.</returns>
    public static string FhirVersionToShortVersion(string version)
    {
        if (_fhirSequenceMap.TryGetValue(version, out FhirSequenceCodes sequence))
        {
            return sequence.ToShortVersion();
        }

        // check for a tag we do not know
        if (version.Contains('-'))
        {
            if (_fhirSequenceMap.TryGetValue(version.Substring(0, version.IndexOf('-')), out FhirSequenceCodes sequence2))
            {
                return sequence2.ToShortVersion();
            }
        }

        if (version.StartsWith("R", StringComparison.OrdinalIgnoreCase))
        {
            return version.Substring(1, 1) + ".0";
        }

        return string.Empty;
    }

    /// <summary>Converts a sequence to a long version.</summary>
    /// <param name="sequence">The sequence.</param>
    /// <returns>Sequence as a string.</returns>
    public static string ToLongVersion(this FhirSequenceCodes sequence) => sequence switch
    {
        FhirSequenceCodes.DSTU2 => "1.0.2",
        FhirSequenceCodes.STU3 => "3.0.2",
        FhirSequenceCodes.R4 => "4.0.1",
        FhirSequenceCodes.R4B => "4.3.0",
        FhirSequenceCodes.R5 => "5.0.0",
        FhirSequenceCodes.R6 => "6.0.0",
        _ => "Unknown"
    };

    /// <summary>FHIR version to long version.</summary>
    /// <param name="version">[out] The version literal (e.g., DSTU2).</param>
    /// <returns>A string.</returns>
    public static string FhirVersionToLongVersion(string version)
    {
        if (_fhirSequenceMap.TryGetValue(version, out FhirSequenceCodes sequence))
        {
            return sequence.ToLongVersion();
        }

        // check for a tag we do not know
        if (version.Contains('-'))
        {
            if (_fhirSequenceMap.TryGetValue(version.Substring(0, version.IndexOf('-')), out FhirSequenceCodes sequence2))
            {
                return sequence2.ToLongVersion();
            }
        }

        if (version.StartsWith("R", StringComparison.OrdinalIgnoreCase))
        {
            return version.Substring(1, 1) + ".0.0";
        }

        return string.Empty;
    }

}
