// <copyright file="FhirServerInfo.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Health.Fhir.SpecManager.Extensions;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    /// <summary>A FHIR server.</summary>
    public class FhirServerInfo
    {
        private readonly List<SystemRestfulInteraction> _serverInteractions;
        private readonly int _fhirMajorVersion;

        /// <summary>Initializes a new instance of the <see cref="FhirServerInfo"/> class.</summary>
        /// <param name="serverInteractions">       The server interaction flags.</param>
        /// <param name="url">                      FHIR Base URL for the server.</param>
        /// <param name="fhirVersion">              The server-reported FHIR version.</param>
        /// <param name="softwareName">             The FHIR Server software name.</param>
        /// <param name="softwareVersion">          The FHIR Server software version.</param>
        /// <param name="softwareReleaseDate">      The FHIR Server software release date.</param>
        /// <param name="implementationDescription">Information describing the implementation.</param>
        /// <param name="implementationUrl">        URL of the implementation.</param>
        /// <param name="resourceInteractions">     The server interactions by resource.</param>
        /// <param name="serverSearchParameters">   The search parameters for searching all resources.</param>
        /// <param name="serverOperations">         The operations defined at the system level operation.</param>
        public FhirServerInfo(
            List<string> serverInteractions,
            string url,
            string fhirVersion,
            string softwareName,
            string softwareVersion,
            string softwareReleaseDate,
            string implementationDescription,
            string implementationUrl,
            Dictionary<string, FhirServerResourceInfo> resourceInteractions,
            Dictionary<string, FhirServerSearchParam> serverSearchParameters,
            Dictionary<string, FhirServerOperation> serverOperations)
        {
            Url = url;
            FhirVersion = fhirVersion;

            if (string.IsNullOrEmpty(fhirVersion))
            {
                _fhirMajorVersion = 0;
            }
            else
            {
                // create our JSON converter
                switch (fhirVersion[0])
                {
                    case '1':
                    case '2':
                        _fhirMajorVersion = 2;
                        break;

                    case '3':
                        _fhirMajorVersion = 3;
                        break;

                    case '4':
                        if (fhirVersion.StartsWith("4.4", StringComparison.Ordinal))
                        {
                            _fhirMajorVersion = 5;
                        }
                        else
                        {
                            _fhirMajorVersion = 4;
                        }

                        break;

                    case '5':
                        _fhirMajorVersion = 5;
                        break;
                }
            }

            SoftwareName = softwareName;
            SoftwareVersion = softwareVersion;
            SoftwareReleaseDate = softwareReleaseDate;
            ImplementationDescription = implementationDescription;
            ImplementationUrl = implementationUrl;
            ResourceInteractions = resourceInteractions;
            ServerSearchParameters = serverSearchParameters;
            ServerOperations = serverOperations;

            _serverInteractions = new List<SystemRestfulInteraction>();

            if (serverInteractions != null)
            {
                foreach (string interaction in serverInteractions)
                {
                    _serverInteractions.Add(interaction.ToFhirEnum<SystemRestfulInteraction>());
                }
            }
        }

        /// <summary>Values that represent system restful interactions.</summary>
        public enum SystemRestfulInteraction : int
        {
            /// <summary>Update, create or delete a set of resources as a single transaction.</summary>
            [FhirLiteral("transaction")]
            Transaction,

            /// <summary>Perform a set of a separate interactions in a single http operation.</summary>
            [FhirLiteral("batch")]
            Batch,

            /// <summary>Search all resources based on some filter criteria.</summary>
            [FhirLiteral("search-system")]
            SearchSystem,

            /// <summary>Retrieve the change history for all resources on a system.</summary>
            [FhirLiteral("history-system")]
            HistorySystem,
        }

        /// <summary>Gets FHIR Base URL for the server.</summary>
        public string Url { get; }

        /// <summary>Gets the server-reported FHIR version.</summary>
        public string FhirVersion { get; }

        /// <summary>Gets the major version.</summary>
        public int MajorVersion => _fhirMajorVersion;

        /// <summary>Gets the FHIR Server software name.</summary>
        public string SoftwareName { get; }

        /// <summary>Gets the FHIR Server software version.</summary>
        public string SoftwareVersion { get; }

        /// <summary>Gets the FHIR Server software release date.</summary>
        public string SoftwareReleaseDate { get; }

        /// <summary>Gets information describing the implementation.</summary>
        public string ImplementationDescription { get; }

        /// <summary>Gets URL of the implementation.</summary>
        public string ImplementationUrl { get; }

        /// <summary>Gets the server interactions by resource.</summary>
        public Dictionary<string, FhirServerResourceInfo> ResourceInteractions { get; }

        /// <summary>Gets the server interactions.</summary>
        public List<SystemRestfulInteraction> ServerInteractions => _serverInteractions;

        /// <summary>Gets the search parameters for searching all resources.</summary>
        public Dictionary<string, FhirServerSearchParam> ServerSearchParameters { get; }

        /// <summary>Gets the operations defined at the system level operation.</summary>
        public Dictionary<string, FhirServerOperation> ServerOperations { get; }
    }
}
