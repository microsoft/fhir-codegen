// <copyright file="FhirServer.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    /// <summary>A FHIR server.</summary>
    public class FhirServer
    {
        /// <summary>Gets FHIR Base URL for the server.</summary>
        public Uri Url { get; }

        /// <summary>Gets the server-reported FHIR version.</summary>
        public string FhirVersion { get; }

        /// <summary>Gets the FHIR Server software name.</summary>
        public string SoftwareName { get; }

        /// <summary>Gets the FHIR Server software version.</summary>
        public string SoftwareVersion { get; }

        /// <summary>Gets the FHIR Server software release date.</summary>
        public string SoftwareReleaseDate { get; }

        /// <summary>Gets information describing the implementation.</summary>
        public string ImplementationDescription { get; }

        /// <summary>Gets URL of the implementation.</summary>
        public Uri ImplementationUrl { get; }
    }
}
