// <copyright file="ConverterHelper.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Health.Fhir.SpecManager.Manager;

namespace Microsoft.Health.Fhir.SpecManager.Converters
{
    /// <summary>A converter helper.</summary>
    public static class ConverterHelper
    {
        /// <summary>Converter for version.</summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when one or more arguments are outside the
        ///  required range.</exception>
        /// <param name="majorVersion">The major version.</param>
        /// <returns>An IFhirConverter.</returns>
        public static IFhirConverter ConverterForVersion(int majorVersion)
        {
            // create our JSON converter
            switch (majorVersion)
            {
                case 1:
                case 2:
                    return new FromR2();

                case 3:
                    return new FromR3();

                case 4:
                    return new FromR4();

                case 5:
                    return new FromR5();
            }

            throw new ArgumentOutOfRangeException(nameof(majorVersion));
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
            if (string.IsNullOrEmpty(version))
            {
                throw new ArgumentNullException(nameof(version));
            }

            if (FhirManager.VersionToReleaseDict.ContainsKey(version))
            {
                switch (FhirManager.VersionToReleaseDict[version])
                {
                    case "DSTU2": return new FromR2();
                    case "STU3":  return new FromR3();
                    case "R4":    return new FromR4();
                    case "R4B":   return new FromR4();
                    case "R5":    return new FromR5();
                }
            }

            // fallback to guessing
            switch (version[0])
            {
                case '1':
                case '2':
                    return new FromR2();

                case '3':
                    return new FromR3();

                case '4':
                    if (version.StartsWith("4.4", StringComparison.Ordinal))
                    {
                        return new FromR5();
                    }

                    return new FromR4();

                case '5':
                    return new FromR5();
            }

            throw new ArgumentOutOfRangeException(nameof(version));
        }
    }
}
