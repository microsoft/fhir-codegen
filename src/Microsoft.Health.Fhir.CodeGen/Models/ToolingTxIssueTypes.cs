// <copyright file="TerminologyIssueTypes.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.Model;
using Hl7.Fhir.Support;
using System.Data;

namespace Microsoft.Health.Fhir.CodeGen.Models;

/// <summary>Terminology issue types, pulled from https://build.fhir.org/ig/FHIR/fhir-tools-ig/CodeSystem-tx-issue-type.html.</summary>
public static class ToolingTxIssueTypes
{
    public static CodeableConcept NotFound = new("http://hl7.org/fhir/tools/CodeSystem/tx-issue-type", "not-found", "Not Found An issue that identifies a code system or value set that was not found validating the code(s)");
    public static CodeableConcept NotInVs = new("http://hl7.org/fhir/tools/CodeSystem/tx-issue-type", "not-in-vs", "Not in ValueSet An issue that represents the value set validation failure");
    public static CodeableConcept ThisCodeNotInVs = new("http://hl7.org/fhir/tools/CodeSystem/tx-issue-type", "this-code-not-in-vs", "This code not in valueset An issue that indicates that a particular code as not in the value set(but others might be)");
    public static CodeableConcept InvalidData = new("http://hl7.org/fhir/tools/CodeSystem/tx-issue-type", "invalid-data", "Invalid Data An issue that indicates that the code/Coding/CodeableConcept provided was invalid");
    public static CodeableConcept InvalidCode = new("http://hl7.org/fhir/tools/CodeSystem/tx-issue-type", "invalid-code", "Invalid Code An issue that indicates that a provided code is not valid in a code system");
    public static CodeableConcept InvalidDisplay = new("http://hl7.org/fhir/tools/CodeSystem/tx-issue-type", "invalid-display", "Invalid Display An issue relating to the display provided for a code");
    public static CodeableConcept CannotInfer = new("http://hl7.org/fhir/tools/CodeSystem/tx-issue-type", "cannot-infer", "Cannot Infer System An issue indicating why inferring the code system failed(for type= code)");
    public static CodeableConcept CodeRule = new("http://hl7.org/fhir/tools/CodeSystem/tx-issue-type", "code-rule", "Code Rule An issue indicating that a valid code was not valid in this context");
    public static CodeableConcept VsInvalid = new("http://hl7.org/fhir/tools/CodeSystem/tx-issue-type", "vs-invalid", "ValueSet Invalid An issue relating to a problem processing a value set while validating");
    public static CodeableConcept StatusCheck = new("http://hl7.org/fhir/tools/CodeSystem/tx-issue-type", "status-check", "Status Check An issue pertaining to the status of the underlying resources");
}
