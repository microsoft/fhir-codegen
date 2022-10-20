// <copyright file="FhirCapResource.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.CodeGenCommon.Extensions;

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>A FHIR Resource support record from a CapabilityStatement.</summary>
public class FhirCapResource : ICloneable
{
    private readonly List<FhirInteractionCodes> _interactions;
    private readonly List<ReferenceHandlingPolicy> _referencePolicies;

    /// <summary>Initializes a new instance of the <see cref="FhirCapResource"/> class.</summary>
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
    /// <param name="conditionalPatch"> If the server allows/uses conditional patch.</param>
    /// <param name="conditionalDelete">The conditional delete.</param>
    /// <param name="referencePolicies">The reference policy.</param>
    /// <param name="searchIncludes">   The _include values supported by the server.</param>
    /// <param name="searchRevIncludes">The _revinclude values supported by the server.</param>
    /// <param name="searchParameters"> The search parameters supported by implementation.</param>
    /// <param name="operations">       The operations supported by implementation.</param>
    public FhirCapResource(
        List<string> interactions,
        string resourceType,
        List<string> supportedProfiles,
        string versionSupport,
        bool? readHistory,
        bool? updateCreate,
        bool? conditionalCreate,
        string conditionalRead,
        bool? conditionalUpdate,
        bool? conditionalPatch,
        string conditionalDelete,
        List<string> referencePolicies,
        List<string> searchIncludes,
        List<string> searchRevIncludes,
        Dictionary<string, FhirCapSearchParam> searchParameters,
        Dictionary<string, FhirCapOperation> operations)
    {
        ResourceType = resourceType;
        SupportedProfiles = supportedProfiles ?? new List<string>();
        ReadHistory = readHistory;
        UpdateCreate = updateCreate;
        ConditionalCreate = conditionalCreate;
        ConditionalUpdate = conditionalUpdate;
        ConditionalPatch = conditionalPatch;
        SearchIncludes = searchIncludes ?? new List<string>();
        SearchRevIncludes = searchRevIncludes ?? new List<string>();
        SearchParameters = searchParameters ?? new Dictionary<string, FhirCapSearchParam>();
        Operations = operations ?? new Dictionary<string, FhirCapOperation>();

        _interactions = new List<FhirInteractionCodes>();

        if (interactions != null)
        {
            foreach (string interaction in interactions)
            {
                _interactions.Add(interaction.ToFhirEnum<FhirInteractionCodes>());
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

        _referencePolicies = new List<ReferenceHandlingPolicy>();

        if (referencePolicies != null)
        {
            foreach (string policy in referencePolicies)
            {
                _referencePolicies.Add(policy.ToFhirEnum<ReferenceHandlingPolicy>());
            }
        }
    }

    /// <summary>Initializes a new instance of the <see cref="FhirCapResource"/> class.</summary>
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
    /// <param name="conditionalPatch"> If the server allows/uses conditional patch.</param>
    /// <param name="conditionalDelete">The conditional delete.</param>
    /// <param name="referencePolicies">The reference policy.</param>
    /// <param name="searchIncludes">   The _include values supported by the server.</param>
    /// <param name="searchRevIncludes">The _revinclude values supported by the server.</param>
    /// <param name="searchParameters"> The search parameters supported by implementation.</param>
    /// <param name="operations">       The operations supported by implementation.</param>
    public FhirCapResource(
        List<FhirInteractionCodes> interactions,
        string resourceType,
        List<string> supportedProfiles,
        VersioningPolicy? versionSupport,
        bool? readHistory,
        bool? updateCreate,
        bool? conditionalCreate,
        ConditionalReadPolicy? conditionalRead,
        bool? conditionalUpdate,
        bool? conditionalPatch,
        ConditionalDeletePolicy? conditionalDelete,
        List<ReferenceHandlingPolicy> referencePolicies,
        List<string> searchIncludes,
        List<string> searchRevIncludes,
        Dictionary<string, FhirCapSearchParam> searchParameters,
        Dictionary<string, FhirCapOperation> operations)
    {
        ResourceType = resourceType;
        SupportedProfiles = supportedProfiles ?? new List<string>();
        ReadHistory = readHistory;
        UpdateCreate = updateCreate;
        ConditionalCreate = conditionalCreate;
        ConditionalUpdate = conditionalUpdate;
        ConditionalPatch = conditionalPatch;
        ConditionalRead = conditionalRead;
        ConditionalDelete = conditionalDelete;
        SearchIncludes = searchIncludes ?? new List<string>();
        SearchRevIncludes = searchRevIncludes ?? new List<string>();
        SearchParameters = searchParameters ?? new Dictionary<string, FhirCapSearchParam>();
        Operations = operations ?? new Dictionary<string, FhirCapOperation>();

        _interactions = interactions;

        VersionSupport = versionSupport;

        _referencePolicies = referencePolicies;
    }

    /// <summary>
    /// Values that represent FHIR resource interactions.
    /// Codes from https://hl7.org/fhir/codesystem-restful-interaction.html
    /// </summary>
    public enum FhirInteractionCodes
    {
        /// <summary>Read the current state of the resource..</summary>
        [FhirLiteral("read")]
        Read,

        /// <summary>Read the state of a specific version of the resource.</summary>
        [FhirLiteral("vread")]
        VRead,

        /// <summary>Update an existing resource by its id (or create it if it is new).</summary>
        [FhirLiteral("update")]
        Update,

        /// <summary>Update an existing resource by posting a set of changes to it.</summary>
        [FhirLiteral("patch")]
        Patch,

        /// <summary>Delete a resource.</summary>
        [FhirLiteral("delete")]
        Delete,

        /// <summary>Retrieve the change history for a particular resource.</summary>
        [FhirLiteral("history-instance")]
        HistoryInstance,

        /// <summary>Retrieve the change history for all resources of a particular type.</summary>
        [FhirLiteral("history-type")]
        HistoryType,

        /// <summary>Retrieve the change history for all resources on a system.</summary>
        [FhirLiteral("history-system")]
        HistorySystem,

        /// <summary>Create a new resource with a server assigned id.</summary>
        [FhirLiteral("create")]
        Create,

        /// <summary>Search a resource type or all resources based on some filter criteria.</summary>
        [FhirLiteral("search")]
        Search,

        /// <summary>Search all resources of the specified type based on some filter criteria.</summary>
        [FhirLiteral("search-type")]
        SearchType,

        /// <summary>Search all resources based on some filter criteria.</summary>
        [FhirLiteral("search-system")]
        SearchSystem,

        /// <summary>Perform an operation as defined by an OperationDefinition.</summary>
        [FhirLiteral("operation")]
        Operation,

        /// <summary>Get a Capability Statement for the system.</summary>
        [FhirLiteral("capabilities")]
        Capabilities,

        /// <summary>Get a Capability Statement for the system.</summary>
        [FhirLiteral("transaction")]
        Transaction,

        /// <summary>Get a Capability Statement for the system.</summary>
        [FhirLiteral("batch")]
        Batch,
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
    public enum ReferenceHandlingPolicy
    {
        /// <summary>The server supports and populates Literal references (i.e. using Reference.reference) where they are known (this code does not guarantee that all references are literal; see 'enforced').</summary>
        [FhirLiteral("literal")]
        Literal,

        /// <summary>The server allows logical references (i.e. using Reference.identifier).</summary>
        [FhirLiteral("logical")]
        Logical,

        /// <summary>The server will attempt to resolve logical references to literal references - i.e. converting Reference.identifier to Reference.reference (if resolution fails, the server may still accept resources; see logical).</summary>
        [FhirLiteral("resolves")]
        Resolves,

        /// <summary>The server enforces that references have integrity - e.g. it ensures that references can always be resolved. This is typically the case for clinical record systems, but often not the case for middleware/proxy systems.</summary>
        [FhirLiteral("enforced")]
        Enforced,

        /// <summary>The server does not support references that point to other servers.</summary>
        [FhirLiteral("local")]
        Local,
    }

    /// <summary>Gets the resource type.</summary>
    public string ResourceType { get; }

    /// <summary>Gets the list of supported profile URLs.</summary>
    public List<string> SupportedProfiles { get; }

    /// <summary>Gets the supported interactions.</summary>
    public List<FhirInteractionCodes> Interactions => _interactions;

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

    /// <summary>If the server allows/uses conditional update.</summary>
    public bool? ConditionalUpdate { get; }

    /// <summary>If the server allows/uses conditional patch.</summary>
    public bool? ConditionalPatch { get; }

    /// <summary>Gets the conditional delete.</summary>
    public ConditionalDeletePolicy? ConditionalDelete { get; }

    /// <summary>Gets the reference policy.</summary>
    public List<ReferenceHandlingPolicy> ReferencePolicies => _referencePolicies;

    /// <summary>Gets the _include values supported by the server.</summary>
    public List<string> SearchIncludes { get; }

    /// <summary>Gets the _revinclude values supported by the server.</summary>
    public List<string> SearchRevIncludes { get; }

    /// <summary>Gets the search parameters supported by implementation.</summary>
    public Dictionary<string, FhirCapSearchParam> SearchParameters { get; }

    /// <summary>Gets the operations supported by implementation.</summary>
    public Dictionary<string, FhirCapOperation> Operations { get; }

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    public object Clone()
    {
        List<FhirInteractionCodes> interactions = new List<FhirInteractionCodes>();
        _interactions.ForEach(i => interactions.Add(i));

        List<ReferenceHandlingPolicy> referencePolicy = new List<ReferenceHandlingPolicy>();
        _referencePolicies.ForEach(r => referencePolicy.Add(r));

        List<string> searchIncludes = SearchIncludes.Select(s => (string)s.Clone()).ToList();
        List<string> searchRevIncludes = SearchRevIncludes.Select(s => (string)s.Clone()).ToList();

        Dictionary<string, FhirCapSearchParam> searchParameters = new Dictionary<string, FhirCapSearchParam>();
        foreach (KeyValuePair<string, FhirCapSearchParam> kvp in SearchParameters)
        {
            searchParameters.Add(kvp.Key, (FhirCapSearchParam)kvp.Value.Clone());
        }

        Dictionary<string, FhirCapOperation> operations = new Dictionary<string, FhirCapOperation>();
        foreach (KeyValuePair<string, FhirCapOperation> kvp in Operations)
        {
            operations.Add(kvp.Key, (FhirCapOperation)kvp.Value.Clone());
        }

        return new FhirCapResource(
            interactions,
            ResourceType,
            SupportedProfiles,
            VersionSupport,
            ReadHistory,
            UpdateCreate,
            ConditionalCreate,
            ConditionalRead,
            ConditionalUpdate,
            ConditionalPatch,
            ConditionalDelete,
            referencePolicy,
            searchIncludes,
            searchRevIncludes,
            searchParameters,
            operations);
    }
}
