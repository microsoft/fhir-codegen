// <copyright file="FhirResourceCapability.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>A fhir capability.</summary>
public class FhirResourceCapability
{
    private Dictionary<string, FhirSearchParam> _searchParameters;
    private Dictionary<string, FhirOperation> _operations;

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirResourceCapability"/> class.
    /// </summary>
    /// <exception cref="InvalidDataException">Thrown when an Invalid Data error condition occurs.</exception>
    /// <param name="name">             The name.</param>
    /// <param name="url">              The URL.</param>
    /// <param name="resourceName">     The name of the resource.</param>
    /// <param name="resourceProfile">  The resource profile.</param>
    /// <param name="interactions">     The interactions.</param>
    /// <param name="readHistory">      True if read history, false if not.</param>
    /// <param name="updateCreate">     True if update create, false if not.</param>
    /// <param name="conditionalCreate">True if conditional create, false if not.</param>
    /// <param name="conditionalRead">  The conditional read.</param>
    /// <param name="conditionalUpdate">True if conditional update, false if not.</param>
    /// <param name="conditionalDelete">The conditional delete.</param>
    /// <param name="searchIncludes">   The _include values supported by the server.</param>
    /// <param name="searchRevIncludes">The _revinclude values supported by the server.</param>
    /// <param name="searchParams">     Options for controlling the search.</param>
    /// <param name="operations">       The operations.</param>
    public FhirResourceCapability(
        string name,
        Uri url,
        string resourceName,
        string resourceProfile,
        string[] interactions,
        bool readHistory,
        bool updateCreate,
        bool conditionalCreate,
        string conditionalRead,
        bool conditionalUpdate,
        string conditionalDelete,
        List<string> searchIncludes,
        List<string> searchRevIncludes,
        List<FhirSearchParam> searchParams,
        List<FhirOperation> operations)
    {
        _operations = new Dictionary<string, FhirOperation>();
        _searchParameters = new Dictionary<string, FhirSearchParam>();

        Name = name;
        URL = url;
        ResourceName = resourceName;
        ResourceProfile = resourceProfile;
        ReadHistory = readHistory;
        UpdateCreate = updateCreate;
        ConditionalCreate = conditionalCreate;
        ConditionalUpdate = conditionalUpdate;
        SearchIncludes = searchIncludes;
        SearchRevIncludes = searchRevIncludes;

        // interactions default to off
        InteractionCreate = false;
        InteractionDelete = false;
        InteractionHistoryInstance = false;
        InteractionHistoryType = false;
        InteractionPatch = false;
        InteractionRead = false;
        InteractionSearchType = false;
        InteractionUpdate = false;
        InteractionVRead = false;

        // process interactions
        if (interactions != null)
        {
            foreach (string interaction in interactions)
            {
                switch (interaction)
                {
                    case "read":
                        InteractionRead = true;
                        break;

                    case "vread":
                        InteractionVRead = true;
                        break;

                    case "update":
                        InteractionUpdate = true;
                        break;

                    case "patch":
                        InteractionPatch = true;
                        break;

                    case "delete":
                        InteractionDelete = true;
                        break;

                    case "history-instance":
                        InteractionHistoryInstance = true;
                        break;

                    case "history-type":
                        InteractionHistoryType = true;
                        break;

                    case "create":
                        InteractionCreate = true;
                        break;

                    case "search-type":
                        InteractionSearchType = true;
                        break;

                    default:
                        throw new InvalidDataException($"unknown interaction: {interaction}");
                }
            }
        }

        // process conditional read
        switch (conditionalRead)
        {
            case "modified-since":
                ConditionalRead = ConditionalReadStatus.ModifiedSince;
                break;

            case "not-match":
                ConditionalRead = ConditionalReadStatus.NotMatch;
                break;

            case "full-support":
                ConditionalRead = ConditionalReadStatus.FullSupport;
                break;

            case "not-supported":
            default:
                ConditionalRead = ConditionalReadStatus.NotSupported;
                break;
        }

        // process conditional delete
        switch (conditionalDelete)
        {
            case "single":
                ConditionalDelete = ConditionalDeleteStatus.Single;
                break;

            case "multiple":
                ConditionalDelete = ConditionalDeleteStatus.Multiple;
                break;

            case "not-supported":
            default:
                ConditionalDelete = ConditionalDeleteStatus.NotSupported;
                break;
        }

        // process search parameters
        if (searchParams != null)
        {
            foreach (FhirSearchParam parameter in searchParams)
            {
                _searchParameters.Add(parameter.Code, parameter);
            }
        }

        // process operations
        if (operations != null)
        {
            foreach (FhirOperation operation in operations)
            {
                _operations.Add(operation.Code, operation);
            }
        }
    }

    /// <summary>Values that represent conditional read status.</summary>
    public enum ConditionalReadStatus
    {
        /// <summary>No support for conditional reads.</summary>
        NotSupported,

        /// <summary>Conditional reads are supported, but only with the If-Modified-Since HTTP Header.</summary>
        ModifiedSince,

        /// <summary>Conditional reads are supported, but only with the If-None-Match HTTP Header.</summary>
        NotMatch,

        /// <summary>Conditional reads are supported, with both If-Modified-Since and If-None-Match HTTP Headers.</summary>
        FullSupport,
    }

    /// <summary>Values that represent conditional delete status.</summary>
    public enum ConditionalDeleteStatus
    {
        /// <summary>No support for conditional deletes.</summary>
        NotSupported,

        /// <summary>Conditional deletes are supported, but only single resources at a time.</summary>
#pragma warning disable CA1720 // Identifier contains type name
        Single,
#pragma warning restore CA1720 // Identifier contains type name

        /// <summary>Conditional deletes are supported, and multiple resources can be deleted in a single interaction.</summary>
        Multiple,
    }

    /// <summary>Values that represent versioning policies.</summary>
    public enum VersioningPolicy
    {
        /// <summary>VersionId meta-property is not supported (server) or used (client).</summary>
        NoVersion,

        /// <summary>VersionId meta-property is supported (server) or used (client).</summary>
        Versioned,

        /// <summary>VersionId must be correct for updates (server) or will be specified (If-match header) for updates (client).</summary>
        VersionedUpdate,
    }

    /// <summary>Gets the name.</summary>
    /// <value>The name.</value>
    public string Name { get; }

    /// <summary>Gets URL of the document.</summary>
    /// <value>The URL.</value>
    public Uri URL { get; }

    /// <summary>Gets the name of the resource.</summary>
    /// <value>The name of the resource.</value>
    public string ResourceName { get; }

    /// <summary>Gets the resource profile.</summary>
    /// <value>The resource profile.</value>
    public string ResourceProfile { get; }

    /// <summary>
    /// Gets a value indicating whether the system supports reading the current state of the resource.
    /// </summary>
    /// <value>True if the system supports instance read, false if not.</value>
    public bool InteractionRead { get; }

    /// <summary>
    /// Gets a value indicating whether the system supports reading the state of a
    /// specific version of the resource.
    /// </summary>
    /// <value>True if the system supports instance vRead, false if not.</value>
    public bool InteractionVRead { get; }

    /// <summary>
    /// Gets a value indicating whether the system supports updating an existing resource by its
    /// id (or create it if it is new).
    /// </summary>
    /// <value>True if the system supports instance update, false if not.</value>
    public bool InteractionUpdate { get; }

    /// <summary>
    /// Gets a value indicating whether the system supports updates to an existing resource by
    /// posting a set of changes to it.
    /// </summary>
    /// <value>True if the system supports instance patch, false if not.</value>
    public bool InteractionPatch { get; }

    /// <summary>Gets a value indicating whether the system supports deleting a resource.</summary>
    /// <value>True if the system supports instance delete, false if not.</value>
    public bool InteractionDelete { get; }

    /// <summary>
    /// Gets a value indicating whether the system supports retrieving the change history
    /// for a particular resource.
    /// </summary>
    /// <value>True if the system supports instance history, false if not.</value>
    public bool InteractionHistoryInstance { get; }

    /// <summary>
    /// Gets a value indicating whether the system supports retrieving the change history
    /// for all resources of a particular type.
    /// </summary>
    /// <value>True if supports history type, false if not.</value>
    public bool InteractionHistoryType { get; }

    /// <summary>
    /// Gets a value indicating whether the system supports creating a resource with a system-assigned id.
    /// </summary>
    /// <value>True if the system supports typed create, false if not.</value>
    public bool InteractionCreate { get; }

    /// <summary>
    /// Gets a value indicating whether the system supports searching all resources of the specified type.
    /// </summary>
    /// <value>True if the system supports typed search, false if not.</value>
    public bool InteractionSearchType { get; }

    /// <summary>Gets a value indicating whether vRead can return past versions.</summary>
    /// <value>True if read history, false if not.</value>
    public bool ReadHistory { get; }

    /// <summary>Gets a value indicating whether an update can commit to a new identity.</summary>
    /// <value>True if update create, false if not.</value>
    public bool UpdateCreate { get; }

    /// <summary>Gets a value indicating whether the system allows/uses conditional create.</summary>
    /// <value>True if conditional create, false if not.</value>
    public bool ConditionalCreate { get; }

    /// <summary>Gets the conditional read support.</summary>
    /// <value>The conditional read.</value>
    public ConditionalReadStatus ConditionalRead { get; }

    /// <summary>Gets a value indicating whether the system allows/uses conditional update.</summary>
    /// <value>True if conditional update, false if not.</value>
    public bool ConditionalUpdate { get; }

    /// <summary>Gets the conditional delete support.</summary>
    /// <value>The conditional delete.</value>
    public ConditionalDeleteStatus ConditionalDelete { get; }

    /// <summary>Gets the _include values supported by the server.</summary>
    /// <value>The _include values supported by the server.</value>
    public List<string> SearchIncludes { get; }

    /// <summary>Gets the _revinclude values supported by the server.</summary>
    /// <value>The _revinclude values supported by the server.</value>
    public List<string> SearchRevIncludes { get; }

    /// <summary>Gets the list of search parameters for this capability.</summary>
    /// <value>Search parameters for this capability.</value>
    public Dictionary<string, FhirSearchParam> SearchParameters { get => _searchParameters; }

    /// <summary>Gets the operations.</summary>
    /// <value>The operations.</value>
    public Dictionary<string, FhirOperation> Operations { get => _operations; }
}
