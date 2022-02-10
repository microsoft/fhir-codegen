// <copyright file="FhirGuideInfo.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.SpecManager.Converters;
using Microsoft.Health.Fhir.SpecManager.Models;

namespace Microsoft.Health.Fhir.SpecManager.Manager;

/// <summary>Information about a non-core FHIR package.</summary>
public class FhirGuideInfo : FhirInfoBase, IFhirInfo
{
    private FhirVersionInfo _coreInfo;
    private IFhirConverter _fhirConverter;
    private FhirPackage _packageInfo;

    /// <summary>Initializes a new instance of the <see cref="FhirGuideInfo"/> class.</summary>
    /// <param name="coreInfo">   Information describing the FHIR core version.</param>
    /// <param name="pacakgeInfo">Information describing the pacakge.</param>
    public FhirGuideInfo(
        FhirVersionInfo coreInfo,
        FhirPackage pacakgeInfo)
        : base()
    {
        _coreInfo = coreInfo;
        _fhirConverter = ConverterHelper.ConverterForVersion(coreInfo.MajorVersionEnum);
        _packageInfo = pacakgeInfo;
    }

    /// <summary>Gets information describing the package.</summary>
    public FhirPackage PackageInfo => _packageInfo;

    /// <summary>Determine if we should process resource.</summary>
    /// <param name="resourceName">Name of the resource.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public override bool ShouldProcessResource(string resourceName) => true;

    /// <summary>Determine if we should ignore resource.</summary>
    /// <param name="resourceName">Name of the resource.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public override bool ShouldIgnoreResource(string resourceName) => false;

    /// <summary>Parses resource an object from the given string.</summary>
    /// <param name="json">The JSON.</param>
    /// <returns>A typed Resource object.</returns>
    public override object ParseResource(string json)
    {
        return _fhirConverter.ParseResource(json);
    }

    /// <summary>Attempts to process resource.</summary>
    /// <param name="resource">[out] The resource object.</param>
    public override void ProcessResource(object resource)
    {
        _fhirConverter.ProcessResource(resource, this);
    }

    /// <summary>Determines if we can converter has issues.</summary>
    /// <param name="errorCount">  [out] Number of errors.</param>
    /// <param name="warningCount">[out] Number of warnings.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public override bool ConverterHasIssues(out int errorCount, out int warningCount)
    {
        return _fhirConverter.HasIssues(out errorCount, out warningCount);
    }

    /// <summary>Displays the converter issues.</summary>
    public override void DisplayConverterIssues()
    {
        _fhirConverter.DisplayIssues();
    }
}
