// <copyright file="FhirOperation.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.CodeGenCommon.Extensions;

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>A fhir operation.</summary>
public class FhirOperation : FhirModelBase, ICloneable
{
    /// <summary>Values that represent operation kind codes.</summary>
    public enum OperationKindCodes
    {
        /// <summary>An executable operation.</summary>
        [FhirLiteral("operation")]
        Operation,

        /// <summary>A named query.</summary>
        [FhirLiteral("query")]
        Query,
    }

    /// <summary>Initializes a new instance of the <see cref="FhirOperation"/> class.</summary>
    /// <param name="id">               The identifier.</param>
    /// <param name="url">              The URL.</param>
    /// <param name="version">          The version.</param>
    /// <param name="name">             The name.</param>
    /// <param name="description">      The description.</param>
    /// <param name="publicationStatus">The publication status.</param>
    /// <param name="standardStatus">   The standard status.</param>
    /// <param name="fmmLevel">         The fmm level.</param>
    /// <param name="affectsState">     State of the affects.</param>
    /// <param name="definedOnSystem">  True if defined on system, false if not.</param>
    /// <param name="definedOnType">    True if defined on type, false if not.</param>
    /// <param name="definedOnInstance">True if defined on instance, false if not.</param>
    /// <param name="code">             The code.</param>
    /// <param name="comment">          The comment.</param>
    /// <param name="baseDefinition">   The base definition.</param>
    /// <param name="resourceTypes">    A list of types of the resources.</param>
    /// <param name="parameters">       The allowed parameters to this operation.</param>
    /// <param name="isExperimental">   True if is experimental, false if not.</param>
    /// <param name="kind">             The operation kind.</param>
    /// <param name="narrative">        The narrative.</param>
    /// <param name="narrativeStatus">  The narrative status.</param>
    /// <param name="fhirVersion">      The server-reported FHIR version.</param>
    public FhirOperation(
        string id,
        Uri url,
        string version,
        string name,
        string description,
        string publicationStatus,
        string standardStatus,
        int? fmmLevel,
        bool? affectsState,
        bool definedOnSystem,
        bool definedOnType,
        bool definedOnInstance,
        string code,
        string comment,
        string baseDefinition,
        List<string> resourceTypes,
        List<FhirParameter> parameters,
        bool isExperimental,
        string kind,
        string narrative,
        string narrativeStatus,
        string fhirVersion)
        : base(
            FhirArtifactClassEnum.Operation,
            id,
            name,
            string.Empty,
            string.Empty,
            string.Empty,
            version,
            url,
            publicationStatus,
            standardStatus,
            fmmLevel,
            isExperimental,
            description,
            string.Empty,
            comment,
            string.Empty,
            narrative,
            narrativeStatus,
            fhirVersion)
    {
        AffectsState = affectsState;
        DefinedOnSystem = definedOnSystem;
        DefinedOnType = definedOnType;
        DefinedOnInstance = definedOnInstance;
        Code = code;
        BaseDefinition = baseDefinition;
        Parameters = parameters;

        if (string.IsNullOrEmpty(kind))
        {
            Kind = OperationKindCodes.Operation;
        }
        else if (kind.StartsWith("q", StringComparison.OrdinalIgnoreCase))
        {
            Kind = OperationKindCodes.Query;
        }
        else
        {
            Kind = OperationKindCodes.Operation;
        }

        if (resourceTypes != null)
        {
            ResourceTypes = resourceTypes.ToList();
        }
    }

    /// <summary>Gets a value indicating whether the affects state.</summary>
    public bool? AffectsState { get; }

    /// <summary>Gets a value indicating whether the defined on system.</summary>
    /// <value>True if defined on system, false if not.</value>
    public bool DefinedOnSystem { get; }

    /// <summary>Gets a value indicating whether the defined on type.</summary>
    /// <value>True if defined on type, false if not.</value>
    public bool DefinedOnType { get; }

    /// <summary>Gets a value indicating whether the defined on instance.</summary>
    /// <value>True if defined on instance, false if not.</value>
    public bool DefinedOnInstance { get; }

    /// <summary>Gets the code.</summary>
    /// <value>The code.</value>
    public string Code { get; }

    /// <summary>Gets the base definition.</summary>
    public string BaseDefinition { get; }

    /// <summary>Gets a list of types of the resources.</summary>
    /// <value>A list of types of the resources.</value>
    public List<string> ResourceTypes { get; }

    /// <summary>Gets allowed parameters to this operation.</summary>
    /// <value>The allowed parameters to this operation.</value>
    public List<FhirParameter> Parameters { get; }

    /// <summary>Gets the operation kind.</summary>
    public OperationKindCodes Kind { get; }

    /// <summary>Deep copy.</summary>
    /// <returns>A FhirOperation.</returns>
    public object Clone()
    {
        List<string> resourceTypes = new List<string>();

        if (ResourceTypes != null)
        {
            foreach (string resourceType in ResourceTypes)
            {
                resourceTypes.Add(new string(resourceType));
            }
        }

        List<FhirParameter> parameters = new List<FhirParameter>();

        if (Parameters != null)
        {
            foreach (FhirParameter parameter in Parameters)
            {
                parameters.Add(parameter.DeepCopy());
            }
        }

        return new FhirOperation(
            Id,
            URL,
            Version,
            Name,
            Description,
            PublicationStatus,
            StandardStatus,
            FhirMaturityLevel,
            AffectsState,
            DefinedOnSystem,
            DefinedOnType,
            DefinedOnInstance,
            Code,
            Comment,
            BaseDefinition,
            resourceTypes,
            parameters,
            IsExperimental,
            Kind.ToLiteral(),
            NarrativeText,
            NarrativeStatus,
            FhirVersion);
    }
}
