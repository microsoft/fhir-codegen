﻿// <copyright file="CommonDefinitions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.CodeGenCommon.FhirExtensions;

/// <summary>A common definitions.</summary>
public static class CommonDefinitions
{
    public const string ExtensionComment = "There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.";
    public const string ExtensionDefinition = "May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.";
    public const string ExtensionShort = "Additional content defined by implementations";

    public const string ExtUrlStandardStatus = "http://hl7.org/fhir/StructureDefinition/structuredefinition-standards-status";
    public const string ExtUrlFmm = "http://hl7.org/fhir/StructureDefinition/structuredefinition-fmm";
    public const string ExtUrlWorkGroup = "http://hl7.org/fhir/StructureDefinition/structuredefinition-wg";
    public const string ExtUrlCategory = "http://hl7.org/fhir/StructureDefinition/structuredefinition-category";

    public const string ExtUrlIsInterface = "http://hl7.org/fhir/StructureDefinition/structuredefinition-interface";
    public const string ExtUrlImplements = "http://hl7.org/fhir/StructureDefinition/structuredefinition-implements";

    public const string ExtUrlBestPractice = "http://hl7.org/fhir/StructureDefinition/elementdefinition-bestpractice";
    public const string ExtUrlBestPracticeExplanation = "http://hl7.org/fhir/StructureDefinition/elementdefinition-bestpractice-explanation";

    public const string ExtUrlCapExpectation = "http://hl7.org/fhir/StructureDefinition/capabilitystatement-expectation";
    public const string ExtUrlCapSearchParamCombo = "http://hl7.org/fhir/StructureDefinition/capabilitystatement-search-parameter-combination";

    public const string ExtUrlPackageSource = "http://hl7.org/fhir/StructureDefinition/package-source";

    public const string ExtUrlSdRegex = "http://hl7.org/fhir/StructureDefinition/regex";
    public const string ExtUrlSdRegex2 = "http://hl7.org/fhir/StructureDefinition/structuredefinition-regex";

    public const string ExtUrlFhirType = "http://hl7.org/fhir/StructureDefinition/structuredefinition-fhir-type";

    public const string ExtUrlJsonType = "http://hl7.org/fhir/StructureDefinition/structuredefinition-json-type";
    public const string ExtUrlXmlType = "http://hl7.org/fhir/StructureDefinition/structuredefinition-xml-type";
    public const string ExtUrlRdfType = "http://hl7.org/fhir/StructureDefinition/structuredefinition-rdf-type";

    public const string ExtUrlExplicitTypeName = "http://hl7.org/fhir/StructureDefinition/structuredefinition-explicit-type-name";

    public const string ExtUrlBindingName = "http://hl7.org/fhir/StructureDefinition/elementdefinition-bindingName";
    public const string ExtUrlIsCommonBinding = "http://hl7.org/fhir/StructureDefinition/elementdefinition-isCommonBinding";

    //public const string ExtUrlEdFieldOrder = "http://ginoc.io/fhir/StructureDefinition/elementdefinition-fieldOrder";
    //public const string ExtUrlEdComponentFieldOrder = "http://ginoc.io/fhir/StructureDefinition/elementdefinition-componentFieldOrder";
    public const string ExtUrlSpXPath = "http://hl7.org/fhir/1.0/StructureDefinition/extension-SearchParameter.xpath";
    public const string ExtUrlSpXPathUsage = "http://hl7.org/fhir/1.0/StructureDefinition/extension-SearchParameter.xpathUsage";

    public const string ExtUrlEdProfileElement = "http://hl7.org/fhir/StructureDefinition/elementdefinition-profile-element";

    public const string FhirStructureUrlPrefix = "http://hl7.org/fhir/StructureDefinition/";
    public const string FhirUrlPrefix = "http://hl7.org/fhir/";
    public const string FhirPathUrlPrefix = "http://hl7.org/fhirpath/";
    public const string THOCsUrlPrefix = "http://terminology.hl7.org/CodeSystem/";
    public const string THOUrlPrefix = "http://terminology.hl7.org/";

    public const string ExtUrlSmartOAuth = "http://fhir-registry.smarthealthit.org/StructureDefinition/oauth-uris";

    public const string ExtUrlConceptMapAdditionalUrls = "http://ginoc.io/fhir/StructureDefinition/conceptmap-additional-urls";

    public const string ExtUrlComment = "http://ginoc.io/fhir/StructureDefinition/comment";
}
