// <copyright file="FhirCapabilityStatement.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Expectations;

namespace Microsoft.Health.Fhir.CodeGenCommon.Instance;

/// <summary>A FHIR capability statement.</summary>
public record class FhirCapabilityStatement : FhirCanonicalBase, ICloneable
{
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

    /// <summary>Values that represent system restful interactions.</summary>
    public enum SystemRestfulInteractionCodes : int
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

    /// <summary>Values that represent versioning policies.</summary>
    public enum VersioningPolicyCodes
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
    public enum ConditionalReadPolicyCodes
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
    public enum ConditionalDeletePolicyCodes
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
    public enum ReferenceHandlingPolicyCodes
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

    /// <summary>A capability operation.</summary>
    public record class CapabilityOperation : ConformanceAnnotatedBase, ICloneable
    {
        /// <summary>Initializes a new instance of the CapabilityOperation class.</summary>
        public CapabilityOperation() : base() { }

        /// <summary>Initializes a new instance of the CapabilityOperation class.</summary>
        /// <param name="other">The other.</param>
        [SetsRequiredMembers]
        protected CapabilityOperation(CapabilityOperation other)
            : base(other)
        {
            Name = other.Name;
            DefinitionCanonical = other.DefinitionCanonical;
            Documentation = other.Documentation;
            AdditionalDefinitions = other.AdditionalDefinitions.Select(v => v);
        }

        /// <summary>Gets the name.</summary>
        public required string Name { get; init; }

        /// <summary>Gets the definition canonical.</summary>
        public required string DefinitionCanonical { get; init; }

        /// <summary>Gets the documentation (markdown).</summary>
        public string Documentation { get; init; } = string.Empty;

        /// <summary>Gets the additional definitions.</summary>
        public IEnumerable<string> AdditionalDefinitions { get; init; } = Enumerable.Empty<string>();

        /// <summary>Convert this object into a string representation.</summary>
        /// <returns>A string that represents this object.</returns>
        public override string ToString()
        {
            if (!ObligationsByActor.Any())
            {
                return Name;
            }

            return Name + ": " + string.Join("; ", ObligationsByActor.Select(kvp => (string.IsNullOrEmpty(kvp.Key) ? "" : $"{kvp.Key}: ") + string.Join(", ", kvp.Value)));
        }

        /// <summary>Makes a deep copy of this object.</summary>
        /// <returns>A copy of this object.</returns>
        object ICloneable.Clone() => this with { };
    }

    /// <summary>A capability search parameter.</summary>
    public record class CapabilitySearchParam : ConformanceAnnotatedBase, ICloneable
    {
        private string _searchTypeLiteral = string.Empty;
        private readonly FhirSearchParam.SearchParameterTypeCodes _searchType;

        /// <summary>Initializes a new instance of the CapabilitySearchParam class.</summary>
        public CapabilitySearchParam() : base() { }

        /// <summary>Initializes a new instance of the CapabilitySearchParam class.</summary>
        /// <param name="other">The other.</param>
        [SetsRequiredMembers]
        public CapabilitySearchParam(CapabilitySearchParam other)
            : base(other)
        {
            Name = other.Name;
            DefinitionCanonical = other.DefinitionCanonical;
            Documentation = other.Documentation;
            SearchTypeLiteral = other.SearchTypeLiteral;
        }

        /// <summary>Gets or initializes the name.</summary>
        public required string Name { get; init; }

        /// <summary>Gets or initializes the definition canonical.</summary>
        public string DefinitionCanonical { get; init; } = string.Empty;

        /// <summary>Gets or initializes the documentation (markdown).</summary>
        public string Documentation { get; init; } = string.Empty;

        /// <summary>Gets the type of the search parameter input.</summary>
        public FhirSearchParam.SearchParameterTypeCodes SearchType => _searchType;

        /// <summary>Gets the FHIR Literal search parameter type.</summary>
        public required string SearchTypeLiteral
        {
            get => _searchTypeLiteral;
            init
            {
                _searchTypeLiteral = value;
                if (_searchTypeLiteral.TryFhirEnum(out FhirSearchParam.SearchParameterTypeCodes v))
                {
                    _searchType = v;
                }
            }
        }

        /// <summary>Convert this object into a string representation.</summary>
        /// <returns>A string that represents this object.</returns>
        public override string ToString()
        {
            if (!ObligationsByActor.Any())
            {
                return Name;
            }

            return Name + ": " + string.Join("; ", ObligationsByActor.Select(kvp => (string.IsNullOrEmpty(kvp.Key) ? "" : $"{kvp.Key}: ") + string.Join(", ", kvp.Value)));
        }

        /// <summary>Makes a deep copy of this object.</summary>
        /// <returns>A copy of this object.</returns>
        object ICloneable.Clone() => this with { };
    }

    /// <summary>A capability search combination.</summary>
    public record class CapabilitySearchCombination : ConformanceAnnotatedBase, ICloneable
    {
        /// <summary>Initializes a new instance of the CapabilitySearchCombination class.</summary>
        public CapabilitySearchCombination() : base() { }

        /// <summary>Initializes a new instance of the CapabilitySearchCombination class.</summary>
        /// <param name="other">The other.</param>
        [SetsRequiredMembers]
        public CapabilitySearchCombination(CapabilitySearchCombination other)
            : base(other)
        {
            RequiredParams = other.RequiredParams.Select(v => v);
            OptionalParams = other.OptionalParams.Select(v => v);
        }

        /// <summary>Gets the required parameters for this combination definition.</summary>
        public IEnumerable<string> RequiredParams { get; init; } = Enumerable.Empty<string>();

        /// <summary>Gets the optional parameters for this combination definition.</summary>
        public IEnumerable<string> OptionalParams { get; init; } = Enumerable.Empty<string>();

        /// <summary>Convert this object into a string representation.</summary>
        /// <returns>A string that represents this object.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (RequiredParams.Any())
            {
                sb.Append("Required: ");
                sb.Append(string.Join(", ", RequiredParams));
            }

            if (OptionalParams.Any())
            {
                sb.Append(" - Optional: ");
                sb.Append(string.Join(", ", OptionalParams));
            }

            if (ObligationsByActor.Any())
            {
                sb.Append(string.Join(" - Obligations: ", ObligationsByActor.Select(kvp => (string.IsNullOrEmpty(kvp.Key) ? "" : $"{kvp.Key}: ") + string.Join(", ", kvp.Value))));
            }

            return sb.ToString();
        }

        /// <summary>Makes a deep copy of this object.</summary>
        /// <returns>A copy of this object.</returns>
        object ICloneable.Clone() => this with { };
    }

    /// <summary>A capability resource.</summary>
    public record class CapabilityResource : ConformanceAnnotatedBase, ICloneable
    {
        private readonly ConformanceEnum<VersioningPolicyCodes> _versionSupport = new() { Value = VersioningPolicyCodes.NoVersion, };
        private readonly ConformanceEnum<ConditionalReadPolicyCodes> _conditionalRead = new() { Value = ConditionalReadPolicyCodes.NotSupported, };
        private readonly ConformanceEnum<ConditionalDeletePolicyCodes> _conditionalDelete = new() { Value = ConditionalDeletePolicyCodes.NotSupported, };
        private IEnumerable<ConformanceEnum<FhirInteractionCodes>> _interactions = Enumerable.Empty<ConformanceEnum<FhirInteractionCodes>>();
        private IEnumerable<ConformanceVal<string>> _interactionLiterals = Enumerable.Empty<ConformanceVal<string>>();
        private ConformanceVal<string>? _versionSupportLiteral;
        private ConformanceVal<string>? _conditionalReadLiteral;
        private ConformanceVal<string>? _conditionalDeleteLiteral;
        private IEnumerable<ConformanceEnum<ReferenceHandlingPolicyCodes>> _referencePolicies = Enumerable.Empty<ConformanceEnum<ReferenceHandlingPolicyCodes>>();
        private IEnumerable<ConformanceVal<string>> _referencePolicyLiterals = Enumerable.Empty<ConformanceVal<string>>();

        /// <summary>Initializes a new instance of the CapabilityResource class.</summary>
        public CapabilityResource() : base() { }

        /// <summary>Initializes a new instance of the CapabilityResource class.</summary>
        /// <param name="other">The other.</param>
        [SetsRequiredMembers]
        protected CapabilityResource(CapabilityResource other)
            : base(other)
        {
            ResourceType = other.ResourceType;
            SupportedProfiles = other.SupportedProfiles.Select(v => v with { });
            InteractionLiterals = other.InteractionLiterals.Select(v => v with { });
            VersionSupportLiteral = other.VersionSupportLiteral == null ? null : other.VersionSupportLiteral with { };
            ReadHistory = other.ReadHistory with { };
            UpdateCreate = other.UpdateCreate with { };
            ConditionalCreate = other.ConditionalCreate with { };
            ConditionalReadLiteral = other.ConditionalReadLiteral == null ? null : other.ConditionalReadLiteral with { };
            ConditionalUpdate = other.ConditionalUpdate with { };
            ConditionalPatch = other.ConditionalPatch with { };
            ConditionalDeleteLiteral = other.ConditionalDeleteLiteral == null ? null : other.ConditionalDeleteLiteral with { };
            ReferencePolicyLiterals = other.ReferencePolicyLiterals.Select(v => v with { });
            SearchParameters = other.SearchParameters.ToDictionary(kvp => kvp.Key, kvp => kvp.Value with { });
            SearchParameterCombinations = other.SearchParameterCombinations.Select(v => v with { });
            SearchIncludes = other.SearchIncludes.Select(v => v with { });
            SearchRevIncludes = other.SearchRevIncludes.Select(v => v with { });
            Operations = other.Operations.ToDictionary(kvp => kvp.Key, kvp => kvp.Value with { });
        }

        /// <summary>Gets the resource type.</summary>
        public required string ResourceType { get; init; }

        /// <summary>Gets the list of supported profile URLs.</summary>
        public IEnumerable<ConformanceVal<string>> SupportedProfiles { get; init; } = Enumerable.Empty<ConformanceVal<string>>();

        /// <summary>Gets the supported interactions.</summary>
        public IEnumerable<ConformanceEnum<FhirInteractionCodes>> Interactions => _interactions;

        public IEnumerable<ConformanceVal<string>> InteractionLiterals
        {
            get => _interactionLiterals;
            init
            {
                _interactionLiterals = value;
                _interactions = value.ToEnum<FhirInteractionCodes>();
            }
        }

        /// <summary>Gets the supported version policy.</summary>
        public ConformanceEnum<VersioningPolicyCodes>? VersionSupport => _versionSupport;

        /// <summary>Gets or initializes the version support literal.</summary>
        public ConformanceVal<string>? VersionSupportLiteral
        {
            get => _versionSupportLiteral;
            init
            {
                _versionSupportLiteral = value;
                _versionSupport = value.ToEnum(VersioningPolicyCodes.NoVersion);
            }
        }

        /// <summary>Gets a value indicating whether vRead can return past versions.</summary>
        public ConformanceVal<bool> ReadHistory { get; init; } = new() { Value = false, };

        /// <summary>Gets a value indicating whether update can commit to a new identity.</summary>
        public ConformanceVal<bool> UpdateCreate { get; init; } = new() { Value = false };

        /// <summary>Gets a value indicating whether allows/uses conditional create.</summary>
        public ConformanceVal<bool> ConditionalCreate { get; init; } = new() { Value = false };

        /// <summary>Gets the conditional read policy for this resource.</summary>
        public ConformanceEnum<ConditionalReadPolicyCodes> ConditionalRead => _conditionalRead;

        /// <summary>Gets or initializes the conditional read literal.</summary>
        public ConformanceVal<string>? ConditionalReadLiteral
        {
            get => _conditionalReadLiteral;
            init
            {
                _conditionalReadLiteral = value;
                _conditionalRead = value.ToEnum(ConditionalReadPolicyCodes.NotSupported);
            }
        }
        /// <summary>If the server allows/uses conditional update.</summary>
        public ConformanceVal<bool> ConditionalUpdate { get; init; } = new() { Value = false };

        /// <summary>If the server allows/uses conditional patch.</summary>
        public ConformanceVal<bool> ConditionalPatch { get; init; } = new() { Value = false };

        /// <summary>Gets the conditional delete.</summary>
        public ConformanceEnum<ConditionalDeletePolicyCodes> ConditionalDelete => _conditionalDelete;

        /// <summary>Gets or initializes the conditional delete literal.</summary>
        public ConformanceVal<string>? ConditionalDeleteLiteral
        {
            get => _conditionalDeleteLiteral;
            init
            {
                _conditionalDeleteLiteral = value;
                _conditionalDelete = value.ToEnum(ConditionalDeletePolicyCodes.NotSupported);
            }
        }

        /// <summary>Gets the reference policy.</summary>
        public IEnumerable<ConformanceEnum<ReferenceHandlingPolicyCodes>> ReferencePolicies => _referencePolicies;

        /// <summary>Gets or initializes the reference policy literals.</summary>
        public IEnumerable<ConformanceVal<string>> ReferencePolicyLiterals
        {
            get => _referencePolicyLiterals;
            init
            {
                _referencePolicyLiterals = value;
                _referencePolicies = value.ToEnum<ReferenceHandlingPolicyCodes>();
            }
        }

        /// <summary>Gets the search parameters supported by implementation.</summary>
        public Dictionary<string, CapabilitySearchParam> SearchParameters { get; init; } = new();

        /// <summary>Gets the search parameter combinations.</summary>
        public IEnumerable<CapabilitySearchCombination> SearchParameterCombinations { get; init; } = Enumerable.Empty<CapabilitySearchCombination>();

        /// <summary>Gets the _include values supported by the server.</summary>
        public IEnumerable<ConformanceVal<string>> SearchIncludes { get; init; } = Enumerable.Empty<ConformanceVal<string>>();

        /// <summary>Gets the _revinclude values supported by the server.</summary>
        public IEnumerable<ConformanceVal<string>> SearchRevIncludes { get; init; } = Enumerable.Empty<ConformanceVal<string>>();

        /// <summary>Gets the operations supported by implementation.</summary>
        public Dictionary<string, CapabilityOperation> Operations { get; init; } = new();

        /// <summary>Convert this object into a string representation.</summary>
        /// <returns>A string that represents this object.</returns>
        public override string ToString()
        {
            if (!ObligationsByActor.Any())
            {
                return ResourceType;
            }

            return ResourceType + ": " + string.Join("; ", ObligationsByActor.Select(kvp => (string.IsNullOrEmpty(kvp.Key) ? "" : $"{kvp.Key}: ") + string.Join(", ", kvp.Value)));
        }

        /// <summary>Makes a deep copy of this object.</summary>
        /// <returns>A copy of this object.</returns>
        object ICloneable.Clone() => this with { };
    }

    /// <summary>A FHIR security scheme.</summary>
    public abstract record class CapabilitySecurityScheme : ConformanceAnnotatedBase, ICloneable
    {
        /// <summary>Initializes a new instance of the CapabilitySecurityScheme class.</summary>
        public CapabilitySecurityScheme() : base() { }

        /// <summary>Initializes a new instance of the CapabilitySecurityScheme class.</summary>
        /// <param name="other">The other.</param>
        [SetsRequiredMembers]
        protected CapabilitySecurityScheme(CapabilitySecurityScheme other) : base(other) { }

        /// <summary>Makes a deep copy of this object.</summary>
        /// <returns>A copy of this object.</returns>
        object ICloneable.Clone() => this with { };
    }

    /// <summary>A FHIR capability smart OAuth scheme.</summary>
    public record class CapabilityOAuthScheme : CapabilitySecurityScheme, ICloneable
    {
        /// <summary>Initializes a new instance of the CapabilityOAuthScheme class.</summary>
        public CapabilityOAuthScheme() : base() { }

        /// <summary>Initializes a new instance of the FhirCapSmartOAuthScheme class.</summary>
        /// <param name="other">The other.</param>
        [SetsRequiredMembers]
        protected CapabilityOAuthScheme(CapabilityOAuthScheme other)
            : base(other)
        {
            TokenEndpoint = other.TokenEndpoint;
            AuthorizeEndpoint = other.AuthorizeEndpoint;
            IntrospectEndpoint = other.IntrospectEndpoint;
            RevokeEndpoint = other.RevokeEndpoint;
        }

        /// <summary>Gets the token endpoint.</summary>
        public required string TokenEndpoint { get; init; }

        /// <summary>Gets the authorize endpoint.</summary>
        public required string AuthorizeEndpoint { get; init; }

        /// <summary>Gets the introspect endpoint.</summary>
        public string IntrospectEndpoint { get; init; } = string.Empty;

        /// <summary>Gets the revoke endpoint.</summary>
        public string RevokeEndpoint { get; init; } = string.Empty;

        /// <summary>Makes a deep copy of this object.</summary>
        /// <returns>A copy of this object.</returns>
        object ICloneable.Clone() => this with { };
    }

    private IEnumerable<ConformanceEnum<SystemRestfulInteractionCodes>> _serverInteractions = Enumerable.Empty<ConformanceEnum<SystemRestfulInteractionCodes>>();
    private IEnumerable<ConformanceVal<string>> _serverInteractionLiterals = Enumerable.Empty<ConformanceVal<string>>();

    /// <summary>Initializes a new instance of the FhirCapabilityStatement class.</summary>
    public FhirCapabilityStatement() : base() { }

    /// <summary>Initializes a new instance of the FhirCapabilityStatement class.</summary>
    /// <param name="other">The other.</param>
    [SetsRequiredMembers]
    public FhirCapabilityStatement(FhirCapabilityStatement other)
        : base(other)
    {
        Formats = other.Formats.Select(v => v with { });
        PatchFormats = other.PatchFormats.Select(v => v with { });
        SoftwareName = other.SoftwareName;
        SoftwareVersion = other.SoftwareVersion;
        SoftwareReleaseDate = other.SoftwareReleaseDate;
        ImplementationDescription = other.ImplementationDescription;
        ImplementationUrl = other.ImplementationUrl;
        Instantiates = other.Instantiates.Select(v => v with { });
        ImplementationGuides = other.ImplementationGuides.Select(v => v with { });
        ResourceInteractions = other.ResourceInteractions.ToDictionary(kvp => kvp.Key, kvp => kvp.Value with { });
        ServerInteractionLiterals = other.ServerInteractionLiterals.Select(v => v with { });
        ServerSearchParameters = other.ServerSearchParameters.ToDictionary(kvp => kvp.Key, kvp => kvp.Value with { });
        ServerOperations = other.ServerOperations.ToDictionary(kvp => kvp.Key, kvp => kvp.Value with { });
        SecuritySchemes = other.SecuritySchemes.Select(v => v with { });
    }

    /// <summary>Gets or initializes the formats (MIME types).</summary>
    public required IEnumerable<ConformanceVal<string>> Formats { get; init; }

    /// <summary>Gets or initializes the patch formats (MIME types).</summary>
    public IEnumerable<ConformanceVal<string>> PatchFormats { get; init; } = Enumerable.Empty<ConformanceVal<string>>();

    /// <summary>Gets the FHIR Server software name.</summary>
    public string SoftwareName { get; init; } = string.Empty;

    /// <summary>Gets the FHIR Server software version.</summary>
    public string SoftwareVersion { get; init; } = string.Empty;

    /// <summary>Gets the FHIR Server software release date.</summary>
    public string SoftwareReleaseDate { get; init; } = string.Empty;

    /// <summary>Gets information describing the implementation.</summary>
    public string ImplementationDescription { get; init; } = string.Empty;

    /// <summary>Gets URL of the implementation.</summary>
    public string ImplementationUrl { get; init; } = string.Empty;

    /// <summary>Gets the Canonical URLs of other capability statement this implements.</summary>
    public IEnumerable<ConformanceVal<string>> Instantiates { get; init; } = Enumerable.Empty<ConformanceVal<string>>();

    /// <summary>Gets the Implementation guides supported.</summary>
    public IEnumerable<ConformanceVal<string>> ImplementationGuides { get; init; } = Enumerable.Empty<ConformanceVal<string>>();

    /// <summary>Gets the server interactions by resource.</summary>
    public Dictionary<string, CapabilityResource> ResourceInteractions { get; init; } = new();

    /// <summary>Gets the server interactions.</summary>
    public IEnumerable<ConformanceEnum<SystemRestfulInteractionCodes>> ServerInteractions => _serverInteractions;

    public IEnumerable<ConformanceVal<string>> ServerInteractionLiterals
    {
        get => _serverInteractionLiterals;
        init
        {
            _serverInteractionLiterals = value;
            _serverInteractions = value.ToEnum<SystemRestfulInteractionCodes>();
        }
    }

    /// <summary>Gets the search parameters for searching all resources.</summary>
    public Dictionary<string, CapabilitySearchParam> ServerSearchParameters { get; init; } = new();

    /// <summary>Gets the operations defined at the system level operation.</summary>
    public Dictionary<string, CapabilityOperation> ServerOperations { get; init; } = new();

    public IEnumerable<CapabilitySecurityScheme> SecuritySchemes { get; init; } = Enumerable.Empty<CapabilitySecurityScheme>();

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    object ICloneable.Clone() => this with { };

    /// <summary>Determines if we can supports FHIR JSON.</summary>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool SupportsFhirJson()
    {
        if ((Formats == null) ||
            (!Formats.Any()))
        {
            return false;
        }

        return
            Formats.Any(v => v.Value.Equals("application/fhir+json", StringComparison.OrdinalIgnoreCase) ||
            Formats.Any(v => v.Value.Equals("fhir+json", StringComparison.OrdinalIgnoreCase)) ||
            Formats.Any(v => v.Value.Equals("json", StringComparison.OrdinalIgnoreCase)));
    }

    /// <summary>Determines if we can supports FHIR XML.</summary>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool SupportsFhirXml()
    {
        if ((Formats == null) ||
            (!Formats.Any()))
        {
            return false;
        }

        return
            Formats.Any(v => v.Value.Equals("application/fhir+xml", StringComparison.OrdinalIgnoreCase)) ||
            Formats.Any(v => v.Value.Equals("fhir+xml", StringComparison.OrdinalIgnoreCase)) ||
            Formats.Any(v => v.Value.Equals("xml", StringComparison.OrdinalIgnoreCase)) ||
            Formats.Any(v => v.Value.Equals("text/fhir+xml", StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>Determines if we can supports FHIR turtle.</summary>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool SupportsFhirTurtle()
    {
        if ((Formats == null) ||
            (!Formats.Any()))
        {
            return false;
        }

        return
            Formats.Any(v => v.Value.Equals("application/x-turtle", StringComparison.OrdinalIgnoreCase) ||
            Formats.Any(v => v.Value.Equals("ttl", StringComparison.OrdinalIgnoreCase)));
    }

    /// <summary>Determines if we can supports patch JSON.</summary>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool SupportsPatchJson()
    {
        if ((PatchFormats == null) ||
            (!PatchFormats.Any()))
        {
            return false;
        }

        return
            PatchFormats.Any(v => v.Value.Equals("application/json-patch+json", StringComparison.OrdinalIgnoreCase) ||
            PatchFormats.Any(v => v.Value.Equals("json", StringComparison.OrdinalIgnoreCase)) ||
            PatchFormats.Any(v => v.Value.Equals("application/json", StringComparison.OrdinalIgnoreCase)));
    }

    /// <summary>Determines if we can supports patch XML.</summary>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool SupportsPatchXml()
    {
        if ((PatchFormats == null) ||
            (!PatchFormats.Any()))
        {
            return false;
        }

        return
            PatchFormats.Any(v => v.Value.Equals("application/xml", StringComparison.OrdinalIgnoreCase)) ||
            PatchFormats.Any(v => v.Value.Equals("xml", StringComparison.OrdinalIgnoreCase)) ||
            PatchFormats.Any(v => v.Value.Equals("text/xml", StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>Determines if we can supports patch FHIR JSON.</summary>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool SupportsPatchFhirJson()
    {
        if ((PatchFormats == null) ||
            (!PatchFormats.Any()))
        {
            return false;
        }

        return
            PatchFormats.Any(v => v.Value.Equals("application/fhir+json", StringComparison.OrdinalIgnoreCase) ||
            PatchFormats.Any(v => v.Value.Equals("fhir+json", StringComparison.OrdinalIgnoreCase)));
    }

    /// <summary>Determines if we can supports patch FHIR XML.</summary>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool SupportsPatchFhirXml()
    {
        if ((PatchFormats == null) ||
            (!PatchFormats.Any()))
        {
            return false;
        }

        return
            PatchFormats.Any(v => v.Value.Equals("application/fhir+xml", StringComparison.OrdinalIgnoreCase)) ||
            PatchFormats.Any(v => v.Value.Equals("fhir+xml", StringComparison.OrdinalIgnoreCase)) ||
            PatchFormats.Any(v => v.Value.Equals("text/fhir+xml", StringComparison.OrdinalIgnoreCase));
    }
}
