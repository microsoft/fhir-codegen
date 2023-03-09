// <copyright file="FhirArtifactIndex.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>Values that represent FHIR artifact class enums.</summary>
public enum FhirArtifactClassEnum
{
    /// <summary>An enum constant representing the primitive type option.</summary>
    PrimitiveType,

    /// <summary>An enum constant representing the complex type option.</summary>
    ComplexType,

    /// <summary>An enum constant representing the resource option.</summary>
    Resource,

    /// <summary>An enum constant representing the extension option.</summary>
    Extension,

    /// <summary>An enum constant representing the operation option.</summary>
    Operation,

    /// <summary>An enum constant representing the search parameter option.</summary>
    SearchParameter,

    /// <summary>An enum constant representing the code system option.</summary>
    CodeSystem,

    /// <summary>An enum constant representing the value set option.</summary>
    ValueSet,

    /// <summary>An enum constant representing the profile option.</summary>
    Profile,

    /// <summary>An enum constant representing the logical model option.</summary>
    LogicalModel,


    /// <summary>An enum constant representing the capability statement option.</summary>
    CapabilityStatement,

    /// <summary>An enum constant representing the compartment option.</summary>
    Compartment,

    /// <summary>An enum constant representing the concept map option.</summary>
    ConceptMap,

    /// <summary>An enum constant representing the naming system option.</summary>
    NamingSystem,

    /// <summary>An enum constant representing the structure map option.</summary>
    StructureMap,


    /// <summary>An enum constant representing the implementation guide option.</summary>
    ImplementationGuide,

    /// <summary>An enum constant representing the unknown option.</summary>
    Unknown,
}

/// <summary>Information about the FHIR artifact.</summary>
public record struct FhirArtifactRecord(
    FhirArtifactClassEnum ArtifactClass,
    string Id,
    Uri Url,
    string DefinitionResourceType);
