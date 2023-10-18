// <copyright file="FhirQasRec.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Text.Json.Serialization;
using Microsoft.Health.Fhir.PackageManager.Models;

namespace Microsoft.Health.Fhir.PackageManager;

/// <summary>Information about CI Builds, from qas.json.</summary>
internal class FhirQasRec
{
    private string _fhirVersion = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("package-id")]
    public string PackageId { get; set; } = string.Empty;

    [JsonPropertyName("ig-ver")]
    public string GuideVersion { get; set; } = string.Empty;

    [JsonPropertyName("date")]
    public string BuildDate { get; set; } = string.Empty;

    [JsonPropertyName("errs")]
    public int ErrorCount { get; set; } = 0;

    [JsonPropertyName("warnings")]
    public int WarningCount { get; set; } = 0;

    [JsonPropertyName("hints")]
    public int HintCount { get; set; } = 0;

    [JsonPropertyName("version")]
    [JsonConverter(typeof(FhirVersionConverter))]
    public string FhirVersion
    {
        get => _fhirVersion;
        set
        {
            string temp = value.Trim();

            if (temp.StartsWith('['))
            {
                temp = temp.Replace("[", string.Empty).Replace("]", string.Empty);

                string[] versions = temp.Split(',');

                _fhirVersion = versions[0];

                return;
            }

            _fhirVersion = value;
        }
    }
    [JsonPropertyName("tool")]
    public string ToolingVersion { get; set; } = string.Empty;

    [JsonPropertyName("repo")]
    public string RespositoryUrl { get; set; } = string.Empty;
}
