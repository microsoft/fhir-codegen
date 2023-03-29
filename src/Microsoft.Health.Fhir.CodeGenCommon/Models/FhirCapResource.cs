// <copyright file="FhirCapResource.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Xml.Linq;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using static Microsoft.Health.Fhir.CodeGenCommon.Models.FhirCapabiltyStatement;

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>A FHIR Resource support record from a CapabilityStatement.</summary>
public class FhirCapResource : ICloneable
{
    private readonly List<ReferenceHandlingPolicy> _referencePolicies;

    /// <summary>Initializes a new instance of the <see cref="FhirCapResource"/> class.</summary>
    /// <param name="resourceType">     The resource type.</param>
    /// <param name="expectation">      The conformance expectation.</param>
    /// <param name="interactions">     The interactions.</param>
    /// <param name="interactionExpectations">The conformance expectations for the interactions.</param>
    /// <param name="supportedProfiles">The list of supported profile URLs.</param>
    /// <param name="supportedProfileExpectations">The conformance expectations for the supported profiles.</param>
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
    /// <param name="searchIncludeExpectations">The conformance expectations for the search includes.</param>
    /// <param name="searchRevIncludes">The _revinclude values supported by the server.</param>
    /// <param name="searchRevIncludeExpectations">The conformance expectations for the reverse search includes.</param>
    /// <param name="searchParameters"> The search parameters supported by implementation.</param>
    /// <param name="operations">       The operations supported by implementation.</param>
    /// <param name="spCombinations">   Defined search parameter combinations.</param>
    public FhirCapResource(
        string resourceType,
        string expectation,
        List<string> interactions,
        List<string> interactionExpectations,
        List<string> supportedProfiles,
        List<string> supportedProfileExpectations,
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
        List<string> searchIncludeExpectations,
        List<string> searchRevIncludes,
        List<string> searchRevIncludeExpectations,
        Dictionary<string, FhirCapSearchParam> searchParameters,
        Dictionary<string, FhirCapOperation> operations,
        IEnumerable<FhirCapSearchParamCombination> spCombinations)
    {
        ResourceType = resourceType;
        ExpectationLiteral = expectation;
        if (expectation.TryFhirEnum(out ExpectationCodes expect))
        {
            Expectation = expect;
        }

        SupportedProfiles = supportedProfiles ?? new();
        SupportedProfilesEx = ProcessExpectationEnumerables(SupportedProfiles, supportedProfileExpectations);

        ReadHistory = readHistory;
        UpdateCreate = updateCreate;
        ConditionalCreate = conditionalCreate;
        ConditionalUpdate = conditionalUpdate;
        ConditionalPatch = conditionalPatch;

        SearchIncludes = searchIncludes ?? new();
        SearchIncludesEx = ProcessExpectationEnumerables(SearchIncludes, searchIncludeExpectations);

        SearchRevIncludes = searchRevIncludes ?? new();
        SearchRevIncludesEx = ProcessExpectationEnumerables(SearchRevIncludes, searchRevIncludeExpectations);

        if ((interactions?.Any() ?? false) &&
            interactions.TryFhirEnum(out IEnumerable<FhirInteractionCodes> fi))
        {
            Interactions = fi.ToList();
        }
        else
        {
            Interactions = new();
        }

        InteractionsEx = ProcessExpectationEnumerables(Interactions, interactionExpectations);

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

        SearchParameters = searchParameters ?? new();
        Operations = operations ?? new();

        SearchParameterCombinations = spCombinations ?? Array.Empty<FhirCapSearchParamCombination>();
    }

    /// <summary>Initializes a new instance of the <see cref="FhirCapResource"/> class.</summary>
    /// <param name="source">Source to copy.</param>
    public FhirCapResource(FhirCapResource source)
    {
        ResourceType = source.ResourceType;
        ExpectationLiteral = source.ExpectationLiteral;
        Expectation = source.Expectation;
        Interactions = source.Interactions.Select(e => e).ToList();
        InteractionsEx = source.InteractionsEx.Select(r => r with { });
        SupportedProfiles = source.SupportedProfiles.Select(s => s).ToList();
        SupportedProfilesEx = source.SupportedProfilesEx.Select(r => r with { });

        VersionSupport = source.VersionSupport;
        ReadHistory = source.ReadHistory;
        UpdateCreate = source.UpdateCreate;
        ConditionalCreate = source.ConditionalCreate;
        ConditionalUpdate = source.ConditionalUpdate;
        ConditionalPatch = source.ConditionalPatch;
        ConditionalRead = source.ConditionalRead;
        ConditionalDelete = source.ConditionalDelete;

        _referencePolicies = source.ReferencePolicies.Select(p => p).ToList();

        SearchIncludes = source.SearchIncludes.Select(s => s).ToList();
        SearchIncludesEx = source.SearchIncludesEx.Select(r => r with { });

        SearchRevIncludes = source.SearchRevIncludes.Select(s => s).ToList();
        SearchRevIncludesEx = source.SearchRevIncludesEx.Select(r => r with { });

        SearchParameters = new();
        foreach (KeyValuePair<string, FhirCapSearchParam> kvp in source.SearchParameters)
        {
            SearchParameters.Add(kvp.Key, new(kvp.Value));
        }

        Operations = new();
        foreach (KeyValuePair<string, FhirCapOperation> kvp in source.Operations)
        {
            Operations.Add(kvp.Key, new(kvp.Value));
        }

        SearchParameterCombinations = source.SearchParameterCombinations.Select(c => new FhirCapSearchParamCombination(c));
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

    /// <summary>Gets the conformance expectation literal.</summary>
    public string ExpectationLiteral { get; }

    /// <summary>Gets the conformance expectation.</summary>
    public FhirCapabiltyStatement.ExpectationCodes? Expectation { get; }

    /// <summary>Gets the list of supported profile URLs.</summary>
    public List<string> SupportedProfiles { get; }

    /// <summary>Gets the supported profile URLs, with conformance expectations.</summary>
    public IEnumerable<ValWithExpectation<string>> SupportedProfilesEx { get; }

    /// <summary>Gets the supported interactions.</summary>
    public List<FhirInteractionCodes> Interactions { get; }

    /// <summary>Gets the supported interactions, with conformance expectations.</summary>
    public IEnumerable<ValWithExpectation<FhirInteractionCodes>> InteractionsEx { get; }

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

    /// <summary>Gets the search includes, with conformance expectations.</summary>
    public IEnumerable<ValWithExpectation<string>> SearchIncludesEx { get; }

    /// <summary>Gets the _revinclude values supported by the server.</summary>
    public List<string> SearchRevIncludes { get; }

    /// <summary>Gets the search reverse includes, with conformance expectations.</summary>
    public IEnumerable<ValWithExpectation<string>> SearchRevIncludesEx { get; }

    /// <summary>Gets the search parameters supported by implementation.</summary>
    public Dictionary<string, FhirCapSearchParam> SearchParameters { get; }

    /// <summary>Gets the operations supported by implementation.</summary>
    public Dictionary<string, FhirCapOperation> Operations { get; }

    /// <summary>Gets the search parameter combinations.</summary>
    public IEnumerable<FhirCapSearchParamCombination> SearchParameterCombinations { get; }

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    public object Clone()
    {
        return new FhirCapResource(this);
    }

    /// <summary>Converts this object to a string with expectation.</summary>
    /// <returns>This object as a string.</returns>
    public string ToStringWithExpectation()
    {
        if (string.IsNullOrEmpty(ExpectationLiteral))
        {
            return ResourceType;
        }

        return $"{ResourceType} ({ExpectationLiteral})";
    }
}
