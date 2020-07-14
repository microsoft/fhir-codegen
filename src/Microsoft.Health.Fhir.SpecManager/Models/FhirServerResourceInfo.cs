// <copyright file="FhirServerResourceInfo.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    /// <summary>Information about a supported FHIR server resource.</summary>
    public class FhirServerResourceInfo
    {
        /// <summary>A bit-field of flags for specifying resource interactions.</summary>
        [Flags]
        public enum FhirInteractions : int
        {
            /// <summary>Read the current state of the resource..</summary>
            Read = 0x0001,

            /// <summary>Read the state of a specific version of the resource.</summary>
            VRead = 0x0002,

            /// <summary>Update an existing resource by its id (or create it if it is new).</summary>
            Update = 0x0004,

            /// <summary>Update an existing resource by posting a set of changes to it.</summary>
            Patch = 0x008,

            /// <summary>Delete a resource.</summary>
            Delete = 0x0010,

            /// <summary>Retrieve the change history for a particular resource.</summary>
            HistoryInstance = 0x0020,

            /// <summary>Retrieve the change history for all resources of a particular type.</summary>
            HistoryType = 0x0040,

            /// <summary>Create a new resource with a server assigned id.</summary>
            Create = 0x0080,

            /// <summary>Search all resources of the specified type based on some filter criteria.</summary>
            SearchType = 0x0100,
        }

        /// <summary>Gets the resource type.</summary>
        public string ResourceType { get; }

        /// <summary>Gets the list of supported profile URLs.</summary>
        public List<string> SupportedProfiles { get; }

        /// <summary>Gets the interaction flags.</summary>
        public int InteractionFlags { get; }
    }
}
