using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fhir_codegen.SQLiteGenerator;

namespace fhir_codegen.LangSQLite.Models;

[CgSQLiteBaseClass]
public abstract class CgDbMetadataResourceBase : CgDbPackageContentBase
{
    public required string Id { get; set; }
    public required string VersionedUrl { get; set; }
    public required string UnversionedUrl { get; set; }
    public required string Name { get; set; }
    public required string Version { get; set; }
    public required string? VersionAlgorithmString { get; set; }
    public required Hl7.Fhir.Model.Coding? VersionAlgorithmCoding { get; set; }
    public required Hl7.Fhir.Model.PublicationStatus? Status { get; set; }
    public required string? Title { get; set; }
    public required string? Description { get; set; }
    public required string? Purpose { get; set; }
    public required Hl7.Fhir.Model.Narrative? Narrative { get; set; }
    public required string? StandardStatus { get; set; }
    public required string? WorkGroup { get; set; }
    public required int? FhirMaturity { get; set; }
    public required bool? IsExperimental { get; set; }
    public required DateTimeOffset? LastChangedDate { get; set; }
    public required string? Publisher { get; set; }
    public required string? Copyright { get; set; }
    public required string? CopyrightLabel { get; set; }
    public required string? ApprovalDate { get; set; }
    public required string? LastReviewDate { get; set; }
    public required DateTimeOffset? EffectivePeriodStart { get; set; }
    public required DateTimeOffset? EffectivePeriodEnd { get; set; }
    public required List<Hl7.Fhir.Model.CodeableConcept>? Topic { get; set; }
    public required List<Hl7.Fhir.Model.RelatedArtifact>? RelatedArtifacts { get; set; }
    public required List<Hl7.Fhir.Model.CodeableConcept>? Jurisdictions { get; set; }
    public required List<Hl7.Fhir.Model.UsageContext>? UseContexts { get; set; }
    public required List<Hl7.Fhir.Model.ContactDetail>? Contacts { get; set; }
    public required List<Hl7.Fhir.Model.ContactDetail>? Authors { get; set; }
    public required List<Hl7.Fhir.Model.ContactDetail>? Editors { get; set; }
    public required List<Hl7.Fhir.Model.ContactDetail>? Reviewers { get; set; }
    public required List<Hl7.Fhir.Model.ContactDetail>? Endorsers { get; set; }
    public required List<Hl7.Fhir.Model.Extension>? RootExtensions { get; set; }
    public required string? SourcePackageMoniker { get; set; }
}
