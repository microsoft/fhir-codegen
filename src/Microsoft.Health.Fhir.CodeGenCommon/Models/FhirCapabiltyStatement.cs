// <copyright file="FhirCapabiltyStatement.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.CodeGenCommon.Extensions;

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>A FHIR server.</summary>
public class FhirCapabiltyStatement : ICloneable
{
    private readonly List<SystemRestfulInteraction> _serverInteractions;

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirCapabiltyStatement"/> class.
    /// </summary>
    /// <param name="serverInteractions">       The server interaction flags.</param>
    /// <param name="id">                       The identifier.</param>
    /// <param name="url">                      FHIR Base URL for the server.</param>
    /// <param name="name">                     The name.</param>
    /// <param name="title">                    The title.</param>
    /// <param name="fhirVersion">              The server-reported FHIR version.</param>
    /// <param name="fhirMimeTypes">            List of types of the FHIR mimes.</param>
    /// <param name="patchMimeTypes">           A list of types of the FHIR patch mimes.</param>
    /// <param name="softwareName">             The FHIR Server software name.</param>
    /// <param name="softwareVersion">          The FHIR Server software version.</param>
    /// <param name="softwareReleaseDate">      The FHIR Server software release date.</param>
    /// <param name="implementationDescription">Information describing the implementation.</param>
    /// <param name="implementationUrl">        URL of the implementation.</param>
    /// <param name="instantiates">             Canonical URL of another capability statement this
    ///  implements.</param>
    /// <param name="implementationGuides">     Implementation guides supported.</param>
    /// <param name="resourceInteractions">     The server interactions by resource.</param>
    /// <param name="serverSearchParameters">   The search parameters for searching all resources.</param>
    /// <param name="serverOperations">         The operations defined at the system level operation.</param>
    public FhirCapabiltyStatement(
        List<string> serverInteractions,
        string id,
        string url,
        string name,
        string title,
        string fhirVersion,
        IEnumerable<string> fhirMimeTypes,
        IEnumerable<string> patchMimeTypes,
        string softwareName,
        string softwareVersion,
        string softwareReleaseDate,
        string implementationDescription,
        string implementationUrl,
        IEnumerable<string> instantiates,
        IEnumerable<string> implementationGuides,
        Dictionary<string, FhirCapResource> resourceInteractions,
        Dictionary<string, FhirCapSearchParam> serverSearchParameters,
        Dictionary<string, FhirCapOperation> serverOperations)
    {
        Id = id;
        Url = url;
        Name = name;
        Title = title;
        FhirVersion = fhirVersion;
        FhirMimeTypes = fhirMimeTypes?.ToArray() ?? Array.Empty<string>();
        PatchMimeTypes = patchMimeTypes?.ToArray() ?? Array.Empty<string>();
        SoftwareName = softwareName;
        SoftwareVersion = softwareVersion;
        SoftwareReleaseDate = softwareReleaseDate;
        ImplementationDescription = implementationDescription;
        ImplementationUrl = implementationUrl;
        Instantiates = instantiates;
        ImplementationGuides = implementationGuides;
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

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirCapabiltyStatement"/> class.
    /// </summary>
    /// <param name="serverInteractions">       The server interaction flags.</param>
    /// <param name="id">                       The identifier.</param>
    /// <param name="url">                      FHIR Base URL for the server.</param>
    /// <param name="name">                     The name.</param>
    /// <param name="title">                    The title.</param>
    /// <param name="fhirVersion">              The server-reported FHIR version.</param>
    /// <param name="fhirMimeTypes">            List of types of the FHIR mimes.</param>
    /// <param name="patchMimeTypes">           A list of types of the FHIR patch mimes.</param>
    /// <param name="softwareName">             The FHIR Server software name.</param>
    /// <param name="softwareVersion">          The FHIR Server software version.</param>
    /// <param name="softwareReleaseDate">      The FHIR Server software release date.</param>
    /// <param name="implementationDescription">Information describing the implementation.</param>
    /// <param name="implementationUrl">        URL of the implementation.</param>
    /// <param name="instantiates">             Canonical URL of another capability statement this
    ///  implements.</param>
    /// <param name="implementationGuides">     Implementation guides supported.</param>
    /// <param name="resourceInteractions">     The server interactions by resource.</param>
    /// <param name="serverSearchParameters">   The search parameters for searching all resources.</param>
    /// <param name="serverOperations">         The operations defined at the system level operation.</param>
    public FhirCapabiltyStatement(
        List<SystemRestfulInteraction> serverInteractions,
        string id,
        string url,
        string name,
        string title,
        string fhirVersion,
        IEnumerable<string> fhirMimeTypes,
        IEnumerable<string> patchMimeTypes,
        string softwareName,
        string softwareVersion,
        string softwareReleaseDate,
        string implementationDescription,
        string implementationUrl,
        IEnumerable<string> instantiates,
        IEnumerable<string> implementationGuides,
        Dictionary<string, FhirCapResource> resourceInteractions,
        Dictionary<string, FhirCapSearchParam> serverSearchParameters,
        Dictionary<string, FhirCapOperation> serverOperations)
    {
        Id = id;
        Url = url;
        Name = name;
        Title = title;
        FhirVersion = fhirVersion;
        FhirMimeTypes = fhirMimeTypes?.ToArray() ?? Array.Empty<string>();
        PatchMimeTypes = patchMimeTypes?.ToArray() ?? Array.Empty<string>();
        SoftwareName = softwareName;
        SoftwareVersion = softwareVersion;
        SoftwareReleaseDate = softwareReleaseDate;
        ImplementationDescription = implementationDescription;
        ImplementationUrl = implementationUrl;
        Instantiates = instantiates;
        ImplementationGuides = implementationGuides;
        ResourceInteractions = resourceInteractions;
        ServerSearchParameters = serverSearchParameters;
        ServerOperations = serverOperations;

        _serverInteractions = serverInteractions;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirCapabiltyStatement"/> class.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
    /// <param name="source">Source for the.</param>
    public FhirCapabiltyStatement(FhirCapabiltyStatement source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        Id = source.Id;
        Url = source.Url;
        Name = source.Name;
        Title = source.Title;
        FhirVersion = source.FhirVersion;
        FhirMimeTypes = source.FhirMimeTypes?.ToArray() ?? Array.Empty<string>();
        PatchMimeTypes = source.PatchMimeTypes?.ToArray() ?? Array.Empty<string>();
        SoftwareName = source.SoftwareName;
        SoftwareVersion = source.SoftwareVersion;
        SoftwareReleaseDate = source.SoftwareReleaseDate;
        ImplementationDescription = source.ImplementationDescription;
        ImplementationUrl = source.ImplementationUrl;

        Instantiates = source.Instantiates?.ToArray();
        ImplementationGuides = source.ImplementationGuides?.ToArray();

        Dictionary<string, FhirCapResource> resourceInteractions = new Dictionary<string, FhirCapResource>();

        foreach (KeyValuePair<string, FhirCapResource> kvp in source.ResourceInteractions)
        {
            //if (!info.Resources.ContainsKey(kvp.Key))
            //{
            //    continue;
            //}

            resourceInteractions.Add(kvp.Key, (FhirCapResource)kvp.Value.Clone());
        }

        _serverInteractions = new List<SystemRestfulInteraction>();
        source.ServerInteractions.ForEach(i => _serverInteractions.Add(i));

        Dictionary<string, FhirCapSearchParam> serverSearchParameters = new Dictionary<string, FhirCapSearchParam>();
        foreach (KeyValuePair<string, FhirCapSearchParam> kvp in source.ServerSearchParameters)
        {
            serverSearchParameters.Add(kvp.Key, (FhirCapSearchParam)kvp.Value.Clone());
        }

        Dictionary<string, FhirCapOperation> serverOperations = new Dictionary<string, FhirCapOperation>();
        foreach (KeyValuePair<string, FhirCapOperation> kvp in source.ServerOperations)
        {
            serverOperations.Add(kvp.Key, (FhirCapOperation)kvp.Value.Clone());
        }

        ResourceInteractions = resourceInteractions;
        ServerSearchParameters = serverSearchParameters;
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

    /// <summary>Gets the identifier.</summary>
    public string Id { get; }

    /// <summary>Gets FHIR Base URL for the server.</summary>
    public string Url { get; }

    /// <summary>Gets the name.</summary>
    public string Name { get; }

    /// <summary>Gets the title.</summary>
    public string Title { get; }

    /// <summary>Gets the listed FHIR version.</summary>
    public string FhirVersion { get; }

    /// <summary>Gets a list of types of the FHIR mimes.</summary>
    public IEnumerable<string> FhirMimeTypes { get; }

    /// <summary>Gets a list of types of the FHIR patch mimes.</summary>
    public IEnumerable<string> PatchMimeTypes { get; }

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

    /// <summary>Gets the Implementation guides supported.</summary>
    public IEnumerable<string> ImplementationGuides { get; }

    /// <summary>Gets the server interactions by resource.</summary>
    public Dictionary<string, FhirCapResource> ResourceInteractions { get; }

    /// <summary>Gets the server interactions.</summary>
    public List<SystemRestfulInteraction> ServerInteractions => _serverInteractions;

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
}
