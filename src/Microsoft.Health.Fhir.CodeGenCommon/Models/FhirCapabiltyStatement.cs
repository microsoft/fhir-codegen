// <copyright file="FhirCapabiltyStatement.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.CodeGenCommon.Extensions;

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>A FHIR server.</summary>
public class FhirCapabiltyStatement : FhirModelBase, ICloneable
{
    /// <summary>Values that represent conformance expectation codes.</summary>
    public enum ExpectationCodes
    {
        /// <summary>An enum constant representing the may option.</summary>
        [FhirLiteral("MAY")]
        MAY,

        /// <summary>An enum constant representing the should option.</summary>
        [FhirLiteral("SHOULD")]
        SHOULD,

        /// <summary>An enum constant representing the shall option.</summary>
        [FhirLiteral("SHALL")]
        SHALL,

        /// <summary>No conformance expectation has been specified.</summary>
        NotSpecified,
    }

    /// <summary>A value with a conformance expectation.</summary>
    /// <param name="Value">             The value.</param>
    /// <param name="ExpectationLiteral">The expectation literal.</param>
    /// <param name="ExpectationCode">   The expectation code.</param>
    public record ValWithExpectation<T>(T Value, string ExpectationLiteral, ExpectationCodes ExpectationCode);

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirCapabiltyStatement"/> class.
    /// </summary>
    /// <param name="serverInteractions">       The server interaction flags.</param>
    /// <param name="serverInteractionExpectations">Conformance expectations for server interactions.</param>
    /// <param name="id">                       The identifier.</param>
    /// <param name="url">                      FHIR Base URL for the server.</param>
    /// <param name="name">                     The name.</param>
    /// <param name="title">                    The title.</param>
    /// <param name="version">                  Version of this Capability Statement.</param>
    /// <param name="publicationStatus">        Publication Status (draft,etc.).</param>
    /// <param name="standardStatus">           Standard status (e.g., STU, Normative).</param>
    /// <param name="fmmLevel">                 FHIR Maturity Model number.</param>
    /// <param name="isExperimental">           A value indicating whether this object is experimental.</param>
    /// <param name="description">              The description.</param>
    /// <param name="narrative">                The narrative.</param>
    /// <param name="narrativeStatus">          The narrative status.</param>
    /// <param name="fhirVersion">              The server-reported FHIR version.</param>
    /// <param name="capabilityStatementKind">  The capability statement kind.</param>
    /// <param name="fhirMimeTypes">            List of supported FHIR MIME types.</param>
    /// <param name="fhirMimeTypeExpectations"> List of conformance expectations for FHIR MIME types.</param>
    /// <param name="patchMimeTypes">           List of supported patch MIME types.</param>
    /// <param name="patchMimeTypeExpectations">List of conformance expectations for patch MIME types.</param>
    /// <param name="softwareName">             The FHIR Server software name.</param>
    /// <param name="softwareVersion">          The FHIR Server software version.</param>
    /// <param name="softwareReleaseDate">      The FHIR Server software release date.</param>
    /// <param name="implementationDescription">Information describing the implementation.</param>
    /// <param name="implementationUrl">        URL of the implementation.</param>
    /// <param name="instantiates">             Canonical URL of another capability statement this CS implements.</param>
    /// <param name="instantiateExpectations">  Conformance expectations for supported capability statements,.</param>
    /// <param name="implementationGuides">     Implementation guides supported.</param>
    /// <param name="implementationGuideExpectations">Implementation guide conformance expectations.</param>
    /// <param name="resourceInteractions">     The server interactions by resource.</param>
    /// <param name="serverSearchParameters">   The search parameters for searching all resources.</param>
    /// <param name="serverOperations">         The operations defined at the system level operation.</param>
    public FhirCapabiltyStatement(
        List<string> serverInteractions,
        List<string> serverInteractionExpectations,
        string id,
        string url,
        string name,
        string title,
        string version,
        string publicationStatus,
        string standardStatus,
        int? fmmLevel,
        bool isExperimental,
        string description,
        string narrative,
        string narrativeStatus,
        string fhirVersion,
        string capabilityStatementKind,
        IEnumerable<string> fhirMimeTypes,
        IEnumerable<string> fhirMimeTypeExpectations,
        IEnumerable<string> patchMimeTypes,
        IEnumerable<string> patchMimeTypeExpectations,
        string softwareName,
        string softwareVersion,
        string softwareReleaseDate,
        string implementationDescription,
        string implementationUrl,
        IEnumerable<string> instantiates,
        IEnumerable<string> instantiateExpectations,
        IEnumerable<string> implementationGuides,
        IEnumerable<string> implementationGuideExpectations,
        Dictionary<string, FhirCapResource> resourceInteractions,
        Dictionary<string, FhirCapSearchParam> serverSearchParameters,
        Dictionary<string, FhirCapOperation> serverOperations)
        : base(
            FhirArtifactClassEnum.CapabilityStatement,
            id,
            name,
            string.Empty,
            string.Empty,
            string.Empty,
            version,
            string.IsNullOrEmpty(url) ? null : new Uri(url),
            publicationStatus,
            standardStatus,
            fmmLevel,
            isExperimental,
            title,
            description,
            string.Empty,
            string.Empty,
            narrative,
            narrativeStatus,
            fhirVersion)
    {
        FhirMimeTypes = fhirMimeTypes ?? Array.Empty<string>();
        FhirMimeTypesEx = ProcessExpectationEnumerables(FhirMimeTypes, fhirMimeTypeExpectations);

        PatchMimeTypes = patchMimeTypes ?? Array.Empty<string>();
        PatchMimeTypesEx = ProcessExpectationEnumerables(PatchMimeTypes, patchMimeTypeExpectations);

        SoftwareName = softwareName;
        SoftwareVersion = softwareVersion;
        SoftwareReleaseDate = softwareReleaseDate;
        ImplementationDescription = implementationDescription;
        ImplementationUrl = implementationUrl;

        Instantiates = instantiates ?? Array.Empty<string>();
        InstantiatesEx = ProcessExpectationEnumerables(Instantiates, instantiateExpectations);

        ImplementationGuides = implementationGuides ?? Array.Empty<string>();
        ImplementationGuidesEx = ProcessExpectationEnumerables(ImplementationGuides, implementationGuideExpectations);

        ResourceInteractions = resourceInteractions ?? new();
        ServerSearchParameters = serverSearchParameters ?? new();
        ServerOperations = serverOperations ?? new();

        if ((serverInteractions != null) &&
            serverInteractions.TryFhirEnum(out IEnumerable<SystemRestfulInteraction> si))
        {
            ServerInteractions = si;
        }
        else
        {
            ServerInteractions = Array.Empty<SystemRestfulInteraction>();
        }

        ServerInteractionsEx = ProcessExpectationEnumerables(ServerInteractions, serverInteractionExpectations);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirCapabiltyStatement"/> class.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
    /// <param name="source">Source for the.</param>
    public FhirCapabiltyStatement(FhirCapabiltyStatement source)
        : base (source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        FhirMimeTypes = source.FhirMimeTypes.Select(s => s);
        FhirMimeTypesEx = source.FhirMimeTypesEx.Select(r => r with { });
        PatchMimeTypes = source.PatchMimeTypes.Select(s => s);
        PatchMimeTypesEx = source.PatchMimeTypesEx.Select(r => r with { });
        SoftwareName = source.SoftwareName;
        SoftwareVersion = source.SoftwareVersion;
        SoftwareReleaseDate = source.SoftwareReleaseDate;
        ImplementationDescription = source.ImplementationDescription;
        ImplementationUrl = source.ImplementationUrl;

        Instantiates = source.Instantiates.Select(s => s);
        InstantiatesEx = source.InstantiatesEx.Select(r => r with { });
        ImplementationGuides = source.ImplementationGuides.Select(s => s);
        ImplementationGuidesEx = source.ImplementationGuidesEx.Select(r => r with { });

        Dictionary<string, FhirCapResource> resourceInteractions = new Dictionary<string, FhirCapResource>();
        foreach (KeyValuePair<string, FhirCapResource> kvp in source.ResourceInteractions)
        {
            resourceInteractions.Add(kvp.Key, (FhirCapResource)kvp.Value.Clone());
        }
        ResourceInteractions = resourceInteractions;

        ServerInteractions = source.ServerInteractions.Select(e => e);
        ServerInteractionsEx = source.ServerInteractionsEx.Select(r => r with { });

        Dictionary<string, FhirCapSearchParam> serverSearchParameters = new Dictionary<string, FhirCapSearchParam>();
        foreach (KeyValuePair<string, FhirCapSearchParam> kvp in source.ServerSearchParameters)
        {
            serverSearchParameters.Add(kvp.Key, new(kvp.Value));
        }
        ServerSearchParameters = serverSearchParameters;

        Dictionary<string, FhirCapOperation> serverOperations = new Dictionary<string, FhirCapOperation>();
        foreach (KeyValuePair<string, FhirCapOperation> kvp in source.ServerOperations)
        {
            serverOperations.Add(kvp.Key, new(kvp.Value));
        }
        ServerOperations = serverOperations;
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

    /// <summary>Gets the FHIR MIME types.</summary>
    public IEnumerable<string> FhirMimeTypes { get; }

    /// <summary>Gets the FHIR MIME types, with conformance expectations.</summary>
    public IEnumerable<ValWithExpectation<string>> FhirMimeTypesEx { get; }

    /// <summary>Gets the patch MIME types.</summary>
    public IEnumerable<string> PatchMimeTypes { get; }

    /// <summary>Gets the patch MIME types, with conformance expectations.</summary>
    public IEnumerable<ValWithExpectation<string>> PatchMimeTypesEx { get; }

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

    /// <summary>Gets the Canonical URLs of other capability statement this implements.</summary>
    public IEnumerable<string> Instantiates { get; }

    /// <summary>Gets the instantiate information, with conformation expectations.</summary>
    public IEnumerable<ValWithExpectation<string>> InstantiatesEx { get; }

    /// <summary>Gets the Implementation guides supported.</summary>
    public IEnumerable<string> ImplementationGuides { get; }

    /// <summary>Gets the implementation guides supported, with conformance expectations.</summary>
    public IEnumerable<ValWithExpectation<string>> ImplementationGuidesEx { get; }

    /// <summary>Gets the server interactions by resource.</summary>
    public Dictionary<string, FhirCapResource> ResourceInteractions { get; }

    /// <summary>Gets the server interactions.</summary>
    public IEnumerable<SystemRestfulInteraction> ServerInteractions { get; }

    /// <summary>Gets the server interactions, with conformance expectations.</summary>
    public IEnumerable<ValWithExpectation<SystemRestfulInteraction>> ServerInteractionsEx { get; }

    /// <summary>Gets the search parameters for searching all resources.</summary>
    public Dictionary<string, FhirCapSearchParam> ServerSearchParameters { get; }

    /// <summary>Gets the operations defined at the system level operation.</summary>
    public Dictionary<string, FhirCapOperation> ServerOperations { get; }

    /// <summary>Creates a new object that is a copy of the current instance.</summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone()
    {
        return new FhirCapabiltyStatement(this);
    }

    /// <summary>Determines if we can supports FHIR JSON.</summary>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool SupportsFhirJson()
    {
        if ((FhirMimeTypes == null) ||
            (!FhirMimeTypes.Any()))
        {
            return false;
        }

        if (FhirMimeTypes.Contains("json") ||
            FhirMimeTypes.Contains("fhir+json") ||
            FhirMimeTypes.Contains("application/fhir+json"))
        {
            return true;
        }

        return false;
    }

    /// <summary>Determines if we can supports FHIR XML.</summary>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool SupportsFhirXml()
    {
        if ((FhirMimeTypes == null) ||
            (!FhirMimeTypes.Any()))
        {
            return false;
        }

        if (FhirMimeTypes.Contains("xml") ||
            FhirMimeTypes.Contains("fhir+xml") ||
            FhirMimeTypes.Contains("application/fhir+xml") ||
            FhirMimeTypes.Contains("text/fhir+xml"))
        {
            return true;
        }

        return false;
    }

    /// <summary>Determines if we can supports FHIR turtle.</summary>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool SupportsFhirTurtle()
    {
        if ((FhirMimeTypes == null) ||
            (!FhirMimeTypes.Any()))
        {
            return false;
        }

        if (FhirMimeTypes.Contains("ttl") ||
            FhirMimeTypes.Contains("application/x-turtle"))
        {
            return true;
        }

        return false;
    }


    /// <summary>Determines if we can supports patch JSON.</summary>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool SupportsPatchJson()
    {
        if ((PatchMimeTypes == null) ||
            (!PatchMimeTypes.Any()))
        {
            return false;
        }

        if (PatchMimeTypes.Contains("json") ||
            PatchMimeTypes.Contains("application/json") ||
            PatchMimeTypes.Contains("application/json-patch+json"))
        {
            return true;
        }

        return false;
    }

    /// <summary>Determines if we can supports patch XML.</summary>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool SupportsPatchXml()
    {
        if ((PatchMimeTypes == null) ||
            (!PatchMimeTypes.Any()))
        {
            return false;
        }

        if (PatchMimeTypes.Contains("xml") ||
            PatchMimeTypes.Contains("application/xml") ||
            PatchMimeTypes.Contains("text/xml"))
        {
            return true;
        }

        return false;
    }

    /// <summary>Determines if we can supports patch FHIR JSON.</summary>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool SupportsPatchFhirJson()
    {
        if ((PatchMimeTypes == null) ||
            (!PatchMimeTypes.Any()))
        {
            return false;
        }

        if (PatchMimeTypes.Contains("fhir+json") ||
            PatchMimeTypes.Contains("application/fhir+json"))
        {
            return true;
        }

        return false;
    }

    /// <summary>Determines if we can supports patch FHIR XML.</summary>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool SupportsPatchFhirXml()
    {
        if ((PatchMimeTypes == null) ||
            (!PatchMimeTypes.Any()))
        {
            return false;
        }

        if (PatchMimeTypes.Contains("fhir+xml") ||
            PatchMimeTypes.Contains("application/fhir+xml") ||
            PatchMimeTypes.Contains("text/fhir+xml"))
        {
            return true;
        }

        return false;
    }


    /// <summary>Process the expectation enumerables.</summary>
    /// <param name="requiredLength">     Length of the required.</param>
    /// <param name="source">             Source for the.</param>
    /// <param name="expectationLiterals">[out] The expectation literals.</param>
    /// <param name="expectations">       [out] The expectations.</param>
    public static IEnumerable<ValWithExpectation<T>> ProcessExpectationEnumerables<T>(
        IEnumerable<T> sourceValues,
        IEnumerable<string> sourceExpectations)
    {
        if (!(sourceValues?.Any() ?? false))
        {
            return Array.Empty<ValWithExpectation<T>>();
        }

        List<ValWithExpectation<T>> expectList = new();

        using (IEnumerator<string> sourceExE = (sourceExpectations ?? Array.Empty<string>()).GetEnumerator())
        {
            foreach (T val in sourceValues)
            {
                string lit = sourceExE.MoveNext() ? sourceExE.Current : string.Empty;

                if (lit.TryFhirEnum(out ExpectationCodes code))
                {
                    expectList.Add(new(val, lit, code));
                }
                else
                {
                    expectList.Add(new(val, lit, ExpectationCodes.NotSpecified));
                }
            }
        }

        return expectList.ToArray();
    }
}
