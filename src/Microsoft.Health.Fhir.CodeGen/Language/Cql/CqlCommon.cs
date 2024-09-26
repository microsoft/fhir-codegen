// <copyright file="CqlCommon.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.CodeGen.Language.Cql;

public static class CqlCommon
{
    /// <summary>  
    /// A set of primary code paths used in the CQL (Clinical Quality Language) context.  
    /// </summary>
    /// <remarks>
    /// TODO: I am uncomfortable with this being here. These should be in a ConceptMap, ValueSet, or something else
    /// and loaded at runtime. Check with Bryn what can be done.
    /// </remarks>
    public static readonly Dictionary<string, string> PrimaryCodePaths = new()
    {
        { "Account", "type" },
        { "ActivityDefinition", "topic" },
        { "AdverseEvent", "event" },
        { "AllergyIntolerance", "code" },
        { "Appointment", "serviceType" },
        { "Basic", "code" },
        { "BodyStructure", "location" },
        { "CarePlan", "category" },
        { "CareTeam", "category" },
        { "ChargeItemDefinition", "code" },
        { "Claim", "type" },
        { "ClinicalImpression", "code" },
        { "Communication", "reasonCode" },
        { "CommunicationRequest", "reasonCode" },
        { "Composition", "type" },
        { "Condition", "code" },
        { "Consent", "category" },
        { "Coverage", "type" },
        { "DetectedIssue", "code" },
        { "Device", "type" },
        { "DeviceMetric", "type" },
        { "DeviceRequest", "code" },
        { "DeviceUseStatement", "device.code" },
        { "DiagnosticReport", "code" },
        { "Encounter", "type" },
        { "EpisodeOfCare", "type" },
        { "ExplanationOfBenefit", "type" },
        { "Flag", "code" },
        { "Goal", "category" },
        { "GuidanceResponse", "module" },
        { "HealthcareService", "type" },
        { "Immunization", "vaccineCode" },
        { "Library", "topic" },
        { "Location", "type" },
        { "Measure", "topic" },
        { "MeasureReport", "type" },
        { "Medication", "code" },
        { "MedicationAdministration", "medication" },
        { "MedicationDispense", "medication" },
        { "MedicationRequest", "medication" },
        { "MedicationStatement", "medication" },
        { "MessageDefinition", "event" },
        { "Observation", "code" },
        { "OperationOutcome", "issue.code" },
        { "PractitionerRole", "code" },
        { "Procedure", "code" },
        { "Questionnaire", "name" },
        { "RelatedPerson", "relationship" },
        { "RiskAssessment", "code" },
        { "SearchParameter", "target" },
        { "ServiceRequest", "code" },
        { "Specimen", "type" },
        { "Substance", "code" },
        { "SupplyDelivery", "type" },
        { "SupplyRequest", "category" },
        {  "Task", "code" },
    };
}
