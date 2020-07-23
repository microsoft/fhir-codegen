// <copyright file="FhirServerResourceInfo.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Health.Fhir.SpecManager.Extensions;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    /// <summary>Information about a supported FHIR server resource.</summary>
    public class FhirServerResourceInfo
    {
        private readonly int _interactionFlags;
        private readonly int _referencePolicies;

        /// <summary>
        /// Initializes a new instance of the <see cref="FhirServerResourceInfo"/> class.
        /// </summary>
        /// <param name="interactions">     The interactions.</param>
        /// <param name="resourceType">     The resource type.</param>
        /// <param name="supportedProfiles">The list of supported profile URLs.</param>
        /// <param name="versionSupport">   The supported version policy.</param>
        /// <param name="readHistory">      A value indicating whether vRead can return past versions.</param>
        /// <param name="updateCreate">     A value indicating whether update can commit to a new
        ///  identity.</param>
        /// <param name="conditionalCreate">A value indicating whether allows/uses conditional create.</param>
        /// <param name="conditionalRead">  The conditional read policy for this resource.</param>
        /// <param name="conditionalUpdate">A value indicating whether the conditional update.</param>
        /// <param name="conditionalDelete">The conditional delete.</param>
        /// <param name="referencePolicies">The reference policy.</param>
        /// <param name="searchIncludes">   The _include values supported by the server.</param>
        /// <param name="searchRevIncludes">The _revinclude values supported by the server.</param>
        /// <param name="searchParameters"> The search parameters supported by implementation.</param>
        /// <param name="operations">       The operations supported by implementation.</param>
        public FhirServerResourceInfo(
            List<string> interactions,
            string resourceType,
            List<string> supportedProfiles,
            string versionSupport,
            bool? readHistory,
            bool? updateCreate,
            bool? conditionalCreate,
            string conditionalRead,
            bool? conditionalUpdate,
            string conditionalDelete,
            List<string> referencePolicies,
            List<string> searchIncludes,
            List<string> searchRevIncludes,
            Dictionary<string, FhirServerSearchParam> searchParameters,
            Dictionary<string, FhirServerOperation> operations)
        {
            ResourceType = resourceType;
            SupportedProfiles = supportedProfiles ?? new List<string>();
            ReadHistory = readHistory;
            UpdateCreate = updateCreate;
            ConditionalCreate = conditionalCreate;
            ConditionalUpdate = conditionalUpdate;
            SearchIncludes = searchIncludes ?? new List<string>();
            SearchRevIncludes = searchRevIncludes ?? new List<string>();
            SearchParameters = searchParameters ?? new Dictionary<string, FhirServerSearchParam>();
            Operations = operations ?? new Dictionary<string, FhirServerOperation>();

            _interactionFlags = 0;

            if (interactions != null)
            {
                foreach (string interaction in interactions)
                {
                    _interactionFlags += (int)interaction.ToFhirEnum<FhirInteractions>();
                }
            }

            if (!string.IsNullOrEmpty(versionSupport))
            {
                VersionSupport = versionSupport.ToFhirEnum<VersioningPolicy>();
            }

            if (!string.IsNullOrEmpty(conditionalRead))
            {
                ConditionalRead = conditionalRead.ToFhirEnum<ConditionalReadPolicy>();
            }

            if (!string.IsNullOrEmpty(conditionalDelete))
            {
                ConditionalDelete = conditionalDelete.ToFhirEnum<ConditionalDeletePolicy>();
            }

            _referencePolicies = 0;

            if (referencePolicies != null)
            {
                foreach (string policy in referencePolicies)
                {
                    _referencePolicies += (int)policy.ToFhirEnum<ReferenceHandlingPolicies>();
                }
            }
        }

        /// <summary>A bit-field of flags for specifying resource interactions.</summary>
        [Flags]
        public enum FhirInteractions : int
        {
            /// <summary>Read the current state of the resource..</summary>
            [FhirLiteral("read")]
            Read = 0x0001,

            /// <summary>Read the state of a specific version of the resource.</summary>
            [FhirLiteral("vread")]
            VRead = 0x0002,

            /// <summary>Update an existing resource by its id (or create it if it is new).</summary>
            [FhirLiteral("update")]
            Update = 0x0004,

            /// <summary>Update an existing resource by posting a set of changes to it.</summary>
            [FhirLiteral("patch")]
            Patch = 0x0008,

            /// <summary>Delete a resource.</summary>
            [FhirLiteral("delete")]
            Delete = 0x0010,

            /// <summary>Retrieve the change history for a particular resource.</summary>
            [FhirLiteral("history-instance")]
            HistoryInstance = 0x0020,

            /// <summary>Retrieve the change history for all resources of a particular type.</summary>
            [FhirLiteral("history-type")]
            HistoryType = 0x0040,

            /// <summary>Create a new resource with a server assigned id.</summary>
            [FhirLiteral("create")]
            Create = 0x0080,

            /// <summary>Search all resources of the specified type based on some filter criteria.</summary>
            [FhirLiteral("search-type")]
            SearchType = 0x0100,
        }

        /// <summary>Values that represent versioning policies.</summary>
        public enum VersioningPolicy
        {
            /// <summary>VersionId meta-property is not supported (server) or used (client).</summary>
            [FhirLiteral("no-version")]
            NoVersion,

            /// <summary>VersionId meta-property is supported (server) or used (client).</summary>
            [FhirLiteral("versioned")]
            Versioned,

            /// <summary>VersionId must be correct for updates (server) or will be specified (If-match header) for updates (client).</summary>
            [FhirLiteral("versioned-update")]
            VersionedUpdate,
        }

        /// <summary>Values that represent conditional read policies.</summary>
        public enum ConditionalReadPolicy
        {
            /// <summary>No support for conditional reads.</summary>
            [FhirLiteral("not-supported")]
            NotSupported,

            /// <summary>Conditional reads are supported, but only with the If-Modified-Since HTTP Header.</summary>
            [FhirLiteral("modified-since")]
            ModifiedSince,

            /// <summary>Conditional reads are supported, but only with the If-None-Match HTTP Header.</summary>
            [FhirLiteral("not-match")]
            NotMatch,

            /// <summary>Conditional reads are supported, with both If-Modified-Since and If-None-Match HTTP Headers.</summary>
            [FhirLiteral("full-support")]
            FullSupport,
        }

        /// <summary>Values that represent conditional delete policies.</summary>
        public enum ConditionalDeletePolicy
        {
            /// <summary>No support for conditional deletes.</summary>
            [FhirLiteral("not-supported")]
            NotSupported,

            /// <summary>Conditional deletes are supported, but only single resources at a time.</summary>
            [FhirLiteral("single")]
#pragma warning disable CA1720 // Identifier contains type name
            Single,
#pragma warning restore CA1720 // Identifier contains type name

            /// <summary>Conditional deletes are supported, and multiple resources can be deleted in a single interaction.</summary>
            [FhirLiteral("multiple")]
            Multiple,
        }

        /// <summary>Values that represent reference handling policies.</summary>
        [Flags]
        public enum ReferenceHandlingPolicies
        {
            /// <summary>The server supports and populates Literal references (i.e. using Reference.reference) where they are known (this code does not guarantee that all references are literal; see 'enforced').</summary>
            [FhirLiteral("literal")]
            Literal = 0x0001,

            /// <summary>The server allows logical references (i.e. using Reference.identifier).</summary>
            [FhirLiteral("logical")]
            Logical = 0x0002,

            /// <summary>The server will attempt to resolve logical references to literal references - i.e. converting Reference.identifier to Reference.reference (if resolution fails, the server may still accept resources; see logical).</summary>
            [FhirLiteral("resolves")]
            Resolves = 0x0004,

            /// <summary>The server enforces that references have integrity - e.g. it ensures that references can always be resolved. This is typically the case for clinical record systems, but often not the case for middleware/proxy systems.</summary>
            [FhirLiteral("enforced")]
            Enforced = 0x0008,

            /// <summary>The server does not support references that point to other servers.</summary>
            [FhirLiteral("local")]
            Local = 0x0010,
        }

        /// <summary>Gets the resource type.</summary>
        public string ResourceType { get; }

        /// <summary>Gets the list of supported profile URLs.</summary>
        public List<string> SupportedProfiles { get; }

        /// <summary>Gets the interaction flags.</summary>
        public int InteractionFlags => _interactionFlags;

        /// <summary>Gets the supported version policy.</summary>
        public VersioningPolicy? VersionSupport { get; }

        /// <summary>Gets a value indicating whether vRead can return past versions.</summary>
        public bool? ReadHistory { get; }

        /// <summary>Gets a value indicating whether update can commit to a new identity.</summary>
        public bool? UpdateCreate { get; }

        /// <summary>Gets a value indicating whether allows/uses conditional create.</summary>
        public bool? ConditionalCreate { get; }

        /// <summary>Gets the conditional read policy for this resource.</summary>
        public ConditionalReadPolicy? ConditionalRead { get; }

        /// <summary>Gets a value indicating whether the conditional update.</summary>
        public bool? ConditionalUpdate { get; }

        /// <summary>Gets the conditional delete.</summary>
        public ConditionalDeletePolicy? ConditionalDelete { get; }

        /// <summary>Gets the reference policy.</summary>
        public int ReferencePolicies => _referencePolicies;

        /// <summary>Gets the _include values supported by the server.</summary>
        public List<string> SearchIncludes { get; }

        /// <summary>Gets the _revinclude values supported by the server.</summary>
        public List<string> SearchRevIncludes { get; }

        /// <summary>Gets the search parameters supported by implementation.</summary>
        public Dictionary<string, FhirServerSearchParam> SearchParameters { get; }

        /// <summary>Gets the operations supported by implementation.</summary>
        public Dictionary<string, FhirServerOperation> Operations { get; }

        /// <summary>Check if a specific interaction is supported by this resource.</summary>
        /// <param name="interaction">The interaction.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        public bool SupportsInteraction(FhirInteractions interaction)
        {
            if ((_interactionFlags & (int)interaction) == (int)interaction)
            {
                return true;
            }

            return false;
        }

        /// <summary>Supports policy.</summary>
        /// <param name="policy">The policy.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        public bool SupportsPolicy(ReferenceHandlingPolicies policy)
        {
            if ((_referencePolicies & (int)policy) == (int)policy)
            {
                return true;
            }

            return false;
        }

    }
}
