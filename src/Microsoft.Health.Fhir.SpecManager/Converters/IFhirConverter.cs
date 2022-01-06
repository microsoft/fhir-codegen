// -------------------------------------------------------------------------------------------------
// <copyright file="IFhirConverter.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Health.Fhir.SpecManager.Manager;
using Microsoft.Health.Fhir.SpecManager.Models;

namespace Microsoft.Health.Fhir.SpecManager.Converters
{
    /// <summary>Interface for converter.</summary>
    public interface IFhirConverter
    {
        /// <summary>Parses resource an object from the given string.</summary>
        /// <param name="json">The JSON.</param>
        /// <returns>A typed Resource object.</returns>
        object ParseResource(byte[] json);

        /// <summary>Parses resource an object from the given string.</summary>
        /// <param name="json">The JSON.</param>
        /// <returns>A typed Resource object.</returns>
        object ParseResource(string json);

        /// <summary>Attempts to process resource.</summary>
        /// <param name="resourceToParse">The resource object.</param>
        /// <param name="fhirVersionInfo">Primitive types.</param>
        void ProcessResource(
            object resourceToParse,
            FhirVersionInfo fhirVersionInfo);

        /// <summary>Process a FHIR metadata resource into Server Information.</summary>
        /// <param name="metadata">  The metadata resource object (e.g., r4.CapabilitiesStatement).</param>
        /// <param name="serverUrl"> URL of the server.</param>
        /// <param name="serverInfo">[out] Information describing the server.</param>
        void ProcessMetadata(
            object metadata,
            string serverUrl,
            out FhirServerInfo serverInfo);

        /// <summary>Query if 'errorCount' has issues.</summary>
        /// <param name="errorCount">  [out] Number of errors.</param>
        /// <param name="warningCount">[out] Number of warnings.</param>
        /// <returns>True if issues, false if not.</returns>
        bool HasIssues(
            out int errorCount,
            out int warningCount);

        /// <summary>Displays the issues.</summary>
        void DisplayIssues();
    }
}
