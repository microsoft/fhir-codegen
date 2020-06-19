// <copyright file="FhirTriplet.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    /// <summary>A fhir triplet.</summary>
    public class FhirTriplet
    {
        /// <summary>Initializes a new instance of the <see cref="FhirTriplet"/> class.</summary>
        /// <param name="system"> The system.</param>
        /// <param name="code">   The code.</param>
        /// <param name="display">The display.</param>
        /// <param name="version">The version.</param>
        public FhirTriplet(
            string system,
            string code,
            string display,
            string version)
        {
            if (string.IsNullOrEmpty(system))
            {
                System = string.Empty;
            }
            else
            {
                System = system;
            }

            if (string.IsNullOrEmpty(code))
            {
                Code = string.Empty;
            }
            else
            {
                Code = code;
            }

            if (string.IsNullOrEmpty(display))
            {
                Display = string.Empty;
            }
            else
            {
                Display = display;
            }

            if (string.IsNullOrEmpty(version))
            {
                Version = string.Empty;
            }
            else
            {
                Version = version;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FhirTriplet"/> class.
        /// </summary>
        /// <param name="system"> The system.</param>
        /// <param name="code">   The code.</param>
        /// <param name="display">The display.</param>
        public FhirTriplet(
            string system,
            string code,
            string display)
            : this(system, code, display, string.Empty)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="FhirTriplet"/> class.</summary>
        /// <param name="system">The system.</param>
        /// <param name="code">  The code.</param>
        public FhirTriplet(
            string system,
            string code)
            : this(system, code, string.Empty, string.Empty)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="FhirTriplet"/> class.</summary>
        private FhirTriplet()
            : this(string.Empty, string.Empty, string.Empty, string.Empty)
        {
        }

        /// <summary>Gets the system.</summary>
        /// <value>The system.</value>
        public string System { get; }

        /// <summary>Gets the code.</summary>
        /// <value>The code.</value>
        public string Code { get; }

        /// <summary>Gets the display.</summary>
        /// <value>The display.</value>
        public string Display { get; }

        /// <summary>Gets the version.</summary>
        /// <value>The version.</value>
        public string Version { get; }

        /// <summary>System and code.</summary>
        /// <returns>A string.</returns>
        public string SystemAndCode() => $"{System}#{Code}";

        /// <summary>Code and system.</summary>
        /// <returns>A string.</returns>
        public string CodeAndSystem() => $"{Code} {System}";

        /// <summary>Gets the key.</summary>
        /// <returns>A string.</returns>
        public string Key() => $"{System}#{Code}";
    }
}
