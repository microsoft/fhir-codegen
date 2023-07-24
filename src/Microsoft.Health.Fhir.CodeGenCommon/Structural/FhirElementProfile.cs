// <copyright file="FhirElementProfile.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using static Microsoft.Health.Fhir.CodeGenCommon.Structural.FhirTypeUtils;

namespace Microsoft.Health.Fhir.CodeGenCommon.Structural;

/// <summary>A FHIR element profile.</summary>
public record class FhirElementProfile
{
    /// <summary>Gets the name.</summary>
    public required string Name { get; init; }

    /// <summary>Gets URL of the document.</summary>
    public required string Url { get; init; }

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() => Name;

    /// <summary>Parse profiles.</summary>
    /// <param name="profiles">The profiles.</param>
    /// <returns>A Dictionary&lt;string,FhirElementProfile&gt;</returns>
    public static Dictionary<string, FhirElementProfile> ParseProfiles(IEnumerable<string> profiles)
    {
        Dictionary<string, FhirElementProfile> dict = new();

        if (profiles?.Any() ?? false)
        {
            foreach (string profileUrl in profiles)
            {
                if (string.IsNullOrEmpty(profileUrl) ||
                    (!TryGetNameAndUrl(profileUrl, out string name, out string url)) ||
                    dict.ContainsKey(name))
                {
                    continue;
                }

                FhirElementProfile profile = new()
                {
                    Name = name,
                    Url = url,
                };

                dict.Add(name, profile);
            }
        }

        return dict;
    }
}
