using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using fhir_codegen.SQLiteGenerator;
using Hl7.Fhir.ElementModel.Types;
using Microsoft.Data.Sqlite;


namespace Microsoft.Health.Fhir.Comparison.Models;


[CgSQLiteBaseClass]
public abstract class DbMetadataResource : DbPackageContent
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

[CgSQLiteTable(tableName: "ResourceIdentifiers")]
[CgSQLiteIndex(nameof(ResourceKey))]
[CgSQLiteIndex(nameof(ResourceKey), nameof(Order))]
public partial class DbResourceIdentifier : DbResourceRelatedContentBase
{
    public required Hl7.Fhir.Model.Identifier.IdentifierUse? Use { get; set; }
    public required Hl7.Fhir.Model.CodeableConcept IdentifierType { get; set; }
    public required string? System { get; set; }
    public required string? Value { get; set; }
    public required DateTimeOffset? PeriodStart { get; set; }
    public required DateTimeOffset? PeriodEnd { get; set; }
    public required Hl7.Fhir.Model.ResourceReference? Assigner { get; set; }

    public DbResourceIdentifier() { }

    [SetsRequiredMembers]
    public DbResourceIdentifier(Hl7.Fhir.Model.Identifier id)
    {
        Use = id.Use;
        IdentifierType = id.Type;
        System = id.System;
        Value = id.Value;
        PeriodStart = id.Period?.StartElement?.ToDateTimeOffset(TimeSpan.Zero);
        PeriodEnd = id.Period?.EndElement?.ToDateTimeOffset(TimeSpan.Zero);
        Assigner = id.Assigner;
    }

    public Hl7.Fhir.Model.Identifier AsIdentifier()
    {
        return new Hl7.Fhir.Model.Identifier()
        {
            Use = this.Use,
            Type = this.IdentifierType,
            System = this.System,
            Value = this.Value,
            Period = (this.PeriodStart == null) && (this.PeriodEnd == null)
                ? null
                : new Hl7.Fhir.Model.Period()
                {
                    StartElement = this.PeriodStart == null ? null : new Hl7.Fhir.Model.FhirDateTime(this.PeriodStart.Value),
                    EndElement = this.PeriodEnd == null ? null : new Hl7.Fhir.Model.FhirDateTime(this.PeriodEnd.Value),
                },
            Assigner = this.Assigner,
        };
    }
}
