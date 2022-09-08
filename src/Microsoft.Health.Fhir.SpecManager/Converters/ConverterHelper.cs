// <copyright file="ConverterHelper.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.SpecManager.Manager;

namespace Microsoft.Health.Fhir.SpecManager.Converters;

/// <summary>A converter helper.</summary>
public static class ConverterHelper
{
    /// <summary>Get a FHIR Converter for the specified major version.</summary>
    /// <param name="release">The release version.</param>
    /// <returns>An IFhirConverter.</returns>
    public static IFhirConverter ConverterForVersion(FhirPackageCommon.FhirSequenceEnum release)
    {
        // create our JSON converter
        switch (release)
        {
            case FhirPackageCommon.FhirSequenceEnum.DSTU2:
                return new FromR2();

            case FhirPackageCommon.FhirSequenceEnum.STU3:
            case FhirPackageCommon.FhirSequenceEnum.R4:
            case FhirPackageCommon.FhirSequenceEnum.R4B:
            case FhirPackageCommon.FhirSequenceEnum.R5:
            default:
                return new FromNormative();
        }
    }

    /// <summary>Converter for version.</summary>
    /// <exception cref="ArgumentNullException">      Thrown when one or more required arguments are
    ///  null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when one or more arguments are outside the
    ///  required range.</exception>
    /// <param name="version">The version.</param>
    /// <returns>An IFhirConverter.</returns>
    public static IFhirConverter ConverterForVersion(string version)
    {
        return ConverterForVersion(FhirPackageCommon.MajorReleaseForVersion(version));
    }
}
