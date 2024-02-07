// <copyright file="CommonDefinitions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.CodeGen.FhirWrappers;

/// <summary>A common definitions.</summary>
public abstract class CommonDefinitions
{
    public const string ExtensionComment = "There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.";
    public const string ExtensionDefinition = "May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.";
    public const string ExtensionShort = "Additional content defined by implementations";

    public const string ExtUrlStandardStatus = "http://hl7.org/fhir/StructureDefinition/structuredefinition-standards-status";
    public const string ExtUrlFmm = "http://hl7.org/fhir/StructureDefinition/structuredefinition-fmm";
    public const string ExtUrlCapExpectation = "http://hl7.org/fhir/StructureDefinition/capabilitystatement-expectation";
    public const string ExtUrlCapSearchParamCombo = "http://hl7.org/fhir/StructureDefinition/capabilitystatement-search-parameter-combination";

    public const string ExtUrlSdRegex = "http://hl7.org/fhir/StructureDefinition/regex";
    public const string ExtUrlSdRegex2 = "http://hl7.org/fhir/StructureDefinition/structuredefinition-regex";

    public const string ExtUrlFhirType = "http://hl7.org/fhir/StructureDefinition/structuredefinition-fhir-type";

    public const string ExtUrlJsonType = "http://hl7.org/fhir/StructureDefinition/structuredefinition-json-type";
    public const string ExtUrlXmlType = "http://hl7.org/fhir/StructureDefinition/structuredefinition-xml-type";
    public const string ExtUrlRdfType = "http://hl7.org/fhir/StructureDefinition/structuredefinition-rdf-type";

    public const string ExtUrlExplicitTypeName = "http://hl7.org/fhir/StructureDefinition/structuredefinition-explicit-type-name";
}
