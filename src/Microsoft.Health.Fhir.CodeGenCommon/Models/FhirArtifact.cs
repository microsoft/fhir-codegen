// <copyright file="FhirArtifact.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using System.ComponentModel;

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>
/// Values that represent FHIR artifact class enums.
/// </summary>
public enum FhirArtifactClassEnum
{
    /// <summary>
    /// Primitive Type (e.g., string, url) definition (StructureDefinition).
    /// </summary>
    [Description("StructureDefinition for a Primitive Type (e.g., string, url).")]
    PrimitiveType,

    /// <summary>
    /// Complex Type (e.g., Address, ContactPoint) definition (StructureDefinition).
    /// </summary>
    [Description("StructureDefinition for a Complex Type (e.g., Address, ContactPoint).")]
    ComplexType,

    /// <summary>
    /// Resource (e.g., Patient, Encounter) definition (StructureDefinition).
    /// </summary>
    [Description("StructureDefinition for a Resource (e.g., Patient, Encounter).")]
    Resource,

    /// <summary>
    /// Extension (e.g., .../event-location) definition (StructureDefinition).
    /// </summary>
    [Description("StructureDefinition for an Extension (e.g., .../event-location).")]
    Extension,

    /// <summary>
    /// Operation (e.g., $export, $metadata) definition (OperationDefinition).
    /// </summary>
    [Description("OperationDefinition for an Operation (e.g., $export, $metadata).")]
    Operation,

    /// <summary>
    /// Search Parameters (e.g., _id (Resource-id)) definition (SearchParameter).
    /// </summary>
    [Description("SearchParameter definition (e.g., _id (Resource-id)).")]
    SearchParameter,

    /// <summary>
    /// Code System (e.g., encounter-status, group-type) (CodeSystem).
    /// </summary>
    [Description("CodeSystem resource (e.g., encounter-status, group-type).")]
    CodeSystem,

    /// <summary>
    /// Value Set (e.g., encounter-status, group-type) (ValueSet).
    /// </summary>
    [Description("ValueSet resource (e.g., encounter-status, group-type).")]
    ValueSet,

    /// <summary>
    /// Profile (e.g., http://hl7.org/fhir/StructureDefinition/vitalsigns) definition (StructureDefinition).
    /// </summary>
    [Description("StructureDefinition for a Profile (e.g., .../vitalsigns).")]
    Profile,

    /// <summary>
    /// Logical Model (e.g., the Event pattern - http://hl7.org/fhir/StructureDefinition/Event) definition (StructureDefinition).
    /// </summary>
    [Description("StructureDefinition for a Logical Model (e.g., the Event pattern).")]
    LogicalModel,

    /// <summary>
    /// StructureDefinition for a FHIR Interface (e.g., CanonicalResource).
    /// </summary>
    [Description("StructureDefinition for a FHIR Interface (e.g., CanonicalResource).")]
    Interface,

    /// <summary>
    /// CapabilityStatement resource.
    /// </summary>
    CapabilityStatement,

    /// <summary>
    /// Compartment Definition (e.g., PatientCompartment) definition (CompartmentDefinition).
    /// </summary>
    [Description("CompartmentDefinition resource (e.g., PatientCompartment).")]
    Compartment,

    /// <summary>
    /// Concept Map - A statement of relationships from one set of concepts to one or more other concepts - either concepts in code systems, or data element/data element concepts, or classes in class models.
    /// </summary>
    [Description("A statement of relationships from one set of concepts to one or more other concepts - either concepts in code systems, or data element/data element concepts, or classes in class models.")]
    ConceptMap,

    /// <summary>
    /// NamingSystem.
    /// </summary>
    NamingSystem,

    /// <summary>
    /// Structure Map - A Map of relationships between 2 structures that can be used to transform data.
    /// </summary>
    [Description("A Map of relationships between 2 structures that can be used to transform data.")]
    StructureMap,

    /// <summary>
    /// ImplementationGuide resource.
    /// </summary>
    ImplementationGuide,

    /// <summary>
    /// An enum constant representing the unknown option.
    /// </summary>
    Unknown,
}
