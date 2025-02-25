// <copyright file="DiffDbContext.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Models;

namespace Microsoft.Health.Fhir.Comparison.Models;

public class ComparisonDbContext : DbContext
{
    public string DbPath { get; private set; }

    public ComparisonDbContext(string dbPath)
    {
        DbPath = dbPath;
    }

    public DbSet<DbFhirPackage> Packages { get; set; }
    public DbSet<DbFhirPackageComparisonPair> PackagePairs { get; set; }

    public DbSet<DbValueSet> ValueSets { get; set; }
    public DbSet<DbValueSetConcept> ValueSetConcepts { get; set; }

    public DbSet<DbValueSetComparison> ValueSetComparisons { get; set; }
    public DbSet<DbValueSetConceptComparison> ValueSetConceptComparisons { get; set; }

    public DbSet<DbInvalidConceptComparison> InvalidImportedConceptComparisons { get; set; }

    public DbSet<DbStructureDefinition> Structures { get; set; }
    public DbSet<DbElement> Elements { get; set; }
    //public DbSet<DbElementType> ElementTypes { get; set; }

    //public DbSet<DbElementTypeMap> ElementTypeMappings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options) => options
        //.UseLazyLoadingProxies()
        //.LogTo(message => Debug.WriteLine(message))
        //.LogTo(Console.WriteLine)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors()
        .UseSqlite($"Data Source={DbPath}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbFhirPackage>()
            .HasKey(nameof(DbFhirPackage.Key));
        modelBuilder.Entity<DbFhirPackage>()
            .HasMany(e => e.ValueSets)
            .WithOne(e => e.FhirPackage)
            .HasForeignKey(e => e.FhirPackageKey)
            .HasPrincipalKey(e => e.Key);
        modelBuilder.Entity<DbFhirPackage>()
            .HasMany(e => e.ValueSetConcepts)
            .WithOne(e => e.FhirPackage)
            .HasForeignKey(e => e.FhirPackageKey)
            .HasPrincipalKey(e => e.Key);
        modelBuilder.Entity<DbFhirPackage>()
            .HasMany(e => e.Structures)
            .WithOne(e => e.FhirPackage)
            .HasForeignKey(e => e.FhirPackageKey)
            .HasPrincipalKey(e => e.Key);
        modelBuilder.Entity<DbFhirPackage>()
            .HasMany(e => e.Elements)
            .WithOne(e => e.FhirPackage)
            .HasForeignKey(e => e.FhirPackageKey)
            .HasPrincipalKey(e => e.Key);

        modelBuilder.Entity<DbFhirPackage>()
            .ToTable("FhirPackages");

        modelBuilder.Entity<DbFhirPackageComparisonPair>()
            .HasKey(nameof(DbFhirPackageComparisonPair.Key));
        modelBuilder.Entity<DbFhirPackageComparisonPair>()
            .HasOne(e => e.SourcePackage)
            .WithMany(e => e.ComparisonsAsSource)
            .HasForeignKey(e => e.SourcePackageKey)
            .HasPrincipalKey(e => e.Key);
        modelBuilder.Entity<DbFhirPackageComparisonPair>()
            .HasOne(e => e.TargetPackage)
            .WithMany(e => e.ComparisonsAsTarget)
            .HasForeignKey(e => e.TargetPackageKey)
            .HasPrincipalKey(e => e.Key);
        modelBuilder.Entity<DbFhirPackageComparisonPair>()
            .HasMany(e => e.ValueSetComparisons)
            .WithOne(e => e.PackageComparison)
            .HasForeignKey(e => e.PackageComparisonKey)
            .HasPrincipalKey(e => e.Key);
        modelBuilder.Entity<DbFhirPackageComparisonPair>()
            .HasMany(e => e.ValueSetConceptComparisons)
            .WithOne(e => e.PackageComparison)
            .HasForeignKey(e => e.PackageComparisonKey)
            .HasPrincipalKey(e => e.Key);
        modelBuilder.Entity<DbFhirPackageComparisonPair>()
            .HasMany(e => e.InvalidImportedConceptComparisons)
            .WithOne(e => e.PackageComparison)
            .HasForeignKey(e => e.PackageComparisonKey)
            .HasPrincipalKey(e => e.Key);
        modelBuilder.Entity<DbFhirPackageComparisonPair>()
            .ToTable("FhirPackagePairs");

        modelBuilder.Entity<DbValueSet>()
            .HasKey(nameof(DbValueSet.Key));
        modelBuilder.Entity<DbValueSet>()
            .Property(e => e.Status)
            .HasConversion(
                v => v.HasValue ? EnumUtility.GetLiteral(v.Value) : null,
                v => EnumUtility.ParseLiteral<Hl7.Fhir.Model.PublicationStatus>(v, true));
        modelBuilder.Entity<DbValueSet>()
            .Property(e => e.StrongestBindingCore)
            .HasConversion(
                v => v.HasValue ? EnumUtility.GetLiteral(v.Value) : null,
                v => EnumUtility.ParseLiteral<Hl7.Fhir.Model.BindingStrength>(v, true));
        modelBuilder.Entity<DbValueSet>()
            .Property(e => e.StrongestBindingCoreCode)
            .HasConversion(
                v => v.HasValue ? EnumUtility.GetLiteral(v.Value) : null,
                v => EnumUtility.ParseLiteral<Hl7.Fhir.Model.BindingStrength>(v, true));
        modelBuilder.Entity<DbValueSet>()
            .Property(e => e.StrongestBindingCoreCoding)
            .HasConversion(
                v => v.HasValue ? EnumUtility.GetLiteral(v.Value) : null,
                v => EnumUtility.ParseLiteral<Hl7.Fhir.Model.BindingStrength>(v, true));
        modelBuilder.Entity<DbValueSet>()
            .Property(e => e.StrongestBindingExtended)
            .HasConversion(
                v => v.HasValue ? EnumUtility.GetLiteral(v.Value) : null,
                v => EnumUtility.ParseLiteral<Hl7.Fhir.Model.BindingStrength>(v, true));
        modelBuilder.Entity<DbValueSet>()
            .Property(e => e.StrongestBindingExtendedCode)
            .HasConversion(
                v => v.HasValue ? EnumUtility.GetLiteral(v.Value) : null,
                v => EnumUtility.ParseLiteral<Hl7.Fhir.Model.BindingStrength>(v, true));
        modelBuilder.Entity<DbValueSet>()
            .Property(e => e.StrongestBindingExtendedCoding)
            .HasConversion(
                v => v.HasValue ? EnumUtility.GetLiteral(v.Value) : null,
                v => EnumUtility.ParseLiteral<Hl7.Fhir.Model.BindingStrength>(v, true));
        modelBuilder.Entity<DbValueSet>()
            .HasOne(e => e.FhirPackage);
        modelBuilder.Entity<DbValueSet>()
            .HasMany(e => e.Concepts)
            .WithOne(e => e.ValueSet)
            .HasForeignKey(e => e.ValueSetKey)
            .HasPrincipalKey(e => e.Key);
        modelBuilder.Entity<DbValueSet>()
            .HasMany(e => e.ComparisonsAsSource)
            .WithOne(e => e.Source)
            .HasForeignKey(e => e.SourceKey)
            .HasPrincipalKey(e => e.Key);
        modelBuilder.Entity<DbValueSet>()
            .HasMany(e => e.ComparisonsAsTarget)
            .WithOne(e => e.Target)
            .HasForeignKey(e => e.TargetKey)
            .HasPrincipalKey(e => e.Key);
        modelBuilder.Entity<DbValueSet>()
            .HasIndex(nameof(DbValueSet.FhirPackageKey));
        modelBuilder.Entity<DbValueSet>()
            .HasIndex(nameof(DbValueSet.VersionedUrl));
        modelBuilder.Entity<DbValueSet>()
            .HasIndex(nameof(DbValueSet.VersionedUrl), nameof(DbValueSet.Version));
        modelBuilder.Entity<DbValueSet>()
            .HasIndex(nameof(DbValueSet.Name));
        modelBuilder.Entity<DbValueSet>()
            .HasIndex(nameof(DbValueSet.Name), nameof(DbValueSet.Version));
        modelBuilder.Entity<DbValueSet>()
            .HasIndex(nameof(DbValueSet.FhirPackageKey), nameof(DbValueSet.VersionedUrl));
        modelBuilder.Entity<DbValueSet>()
            .HasIndex(nameof(DbValueSet.FhirPackageKey), nameof(DbValueSet.Name));
        modelBuilder.Entity<DbValueSet>()
            .ToTable("ValueSets");

        modelBuilder.Entity<DbValueSetConcept>()
            .HasKey(nameof(DbValueSetConcept.Key));
        modelBuilder.Entity<DbValueSetConcept>()
            .HasOne(e => e.ValueSet)
            .WithMany(e => e.Concepts)
            .HasForeignKey(e => e.ValueSetKey)
            .HasPrincipalKey(e => e.Key);
        modelBuilder.Entity<DbValueSetConcept>()
            .HasOne(e => e.FhirPackage)
            .WithMany(e => e.ValueSetConcepts)
            .HasForeignKey(e => e.FhirPackageKey)
            .HasPrincipalKey(e => e.Key);
        modelBuilder.Entity<DbValueSetConcept>()
            .HasIndex(nameof(DbValueSetConcept.FhirPackageKey));
        modelBuilder.Entity<DbValueSetConcept>()
            .HasIndex(nameof(DbValueSetConcept.ValueSetKey));
        modelBuilder.Entity<DbValueSetConcept>()
            .HasIndex(nameof(DbValueSetConcept.System));
        modelBuilder.Entity<DbValueSetConcept>()
            .HasIndex(nameof(DbValueSetConcept.FhirPackageKey), nameof(DbValueSetConcept.System));
        modelBuilder.Entity<DbValueSetConcept>()
            .HasIndex(nameof(DbValueSetConcept.Code));
        modelBuilder.Entity<DbValueSetConcept>()
            .HasIndex(nameof(DbValueSetConcept.System), nameof(DbValueSetConcept.Code));
        modelBuilder.Entity<DbValueSetConcept>()
            .HasIndex(nameof(DbValueSetConcept.FhirPackageKey), nameof(DbValueSetConcept.System), nameof(DbValueSetConcept.Code));
        modelBuilder.Entity<DbValueSetConcept>()
            .HasIndex(nameof(DbValueSetConcept.ValueSetKey), nameof(DbValueSetConcept.System), nameof(DbValueSetConcept.Code));
        modelBuilder.Entity<DbValueSetConcept>()
            .ToTable("ValueSetConcepts");


        modelBuilder.Entity<DbValueSetComparison>()
            .HasKey(nameof(DbValueSetComparison.Key));
        modelBuilder.Entity<DbValueSetComparison>()
            .HasOne(e => e.SourceFhirPackage);
        modelBuilder.Entity<DbValueSetComparison>()
            .HasOne(e => e.Source)
            .WithMany(e => e.ComparisonsAsSource);
        modelBuilder.Entity<DbValueSetComparison>()
            .HasOne(e => e.TargetFhirPackage);
        modelBuilder.Entity<DbValueSetComparison>()
            .HasOne(e => e.Target)
            .WithMany(e => e.ComparisonsAsTarget);
        modelBuilder.Entity<DbValueSetComparison>()
            .HasOne(e => e.PackageComparison)
            .WithMany(e => e.ValueSetComparisons)
            .HasForeignKey(e => e.PackageComparisonKey);
        modelBuilder.Entity<DbValueSetComparison>()
            .HasMany(e => e.ComponentComparisons)
            .WithOne(e => e.CanonicalComparison)
            .HasForeignKey(e => e.CanonicalComparisonKey)
            .HasPrincipalKey(e => e.Key);
        modelBuilder.Entity<DbValueSetComparison>()
            .HasMany(e => e.InvalidImportedComparisons)
            .WithOne(e => e.CanonicalComparison)
            .HasForeignKey(e => e.CanonicalComparisonKey)
            .HasPrincipalKey(e => e.Key);
        modelBuilder.Entity<DbValueSetComparison>()
            .Property(e => e.Relationship)
            .HasConversion(
                v => v.HasValue ? EnumUtility.GetLiteral(v.Value) : null,
                v => EnumUtility.ParseLiteral<Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship>(v, true));
        modelBuilder.Entity<DbValueSetComparison>()
            .HasIndex(nameof(DbValueSetComparison.SourceKey));
        modelBuilder.Entity<DbValueSetComparison>()
            .HasIndex(nameof(DbValueSetComparison.SourceName));
        modelBuilder.Entity<DbValueSetComparison>()
            .HasIndex(nameof(DbValueSetComparison.SourceCanonicalVersioned));
        modelBuilder.Entity<DbValueSetComparison>()
            .HasIndex(nameof(DbValueSetComparison.SourceVersion));
        modelBuilder.Entity<DbValueSetComparison>()
            .HasIndex(nameof(DbValueSetComparison.SourceCanonicalVersioned), nameof(DbValueSetComparison.SourceVersion));
        modelBuilder.Entity<DbValueSetComparison>()
            .HasIndex(nameof(DbValueSetComparison.SourceName), nameof(DbValueSetComparison.SourceVersion));
        modelBuilder.Entity<DbValueSetComparison>()
            .HasIndex(nameof(DbValueSetComparison.TargetKey));
        modelBuilder.Entity<DbValueSetComparison>()
            .HasIndex(nameof(DbValueSetComparison.TargetName));
        modelBuilder.Entity<DbValueSetComparison>()
            .HasIndex(nameof(DbValueSetComparison.TargetCanonicalVersioned));
        modelBuilder.Entity<DbValueSetComparison>()
            .HasIndex(nameof(DbValueSetComparison.TargetVersion));
        modelBuilder.Entity<DbValueSetComparison>()
            .HasIndex(nameof(DbValueSetComparison.TargetCanonicalVersioned), nameof(DbValueSetComparison.TargetVersion));
        modelBuilder.Entity<DbValueSetComparison>()
            .HasIndex(nameof(DbValueSetComparison.TargetName), nameof(DbValueSetComparison.TargetVersion));
        modelBuilder.Entity<DbValueSetComparison>()
            .ToTable("ValueSetComparisons");

        modelBuilder.Entity<DbValueSetConceptComparison>()
            .HasKey(nameof(DbValueSetConceptComparison.Key));
        modelBuilder.Entity<DbValueSetConceptComparison>()
            .HasOne(e => e.SourceFhirPackage);
        modelBuilder.Entity<DbValueSetConceptComparison>()
            .HasOne(e => e.TargetFhirPackage);
        modelBuilder.Entity<DbValueSetConceptComparison>()
            .HasOne(e => e.PackageComparison)
            .WithMany(e => e.ValueSetConceptComparisons)
            .HasForeignKey(e => e.PackageComparisonKey)
            .HasPrincipalKey(e => e.Key);
        modelBuilder.Entity<DbValueSetConceptComparison>()
            .HasOne(e => e.CanonicalComparison);
        modelBuilder.Entity<DbValueSetConceptComparison>()
            .HasOne(e => e.SourceCanonical);
        modelBuilder.Entity<DbValueSetConceptComparison>()
            .HasOne(e => e.TargetCanonical);
        modelBuilder.Entity<DbValueSetConceptComparison>()
            .HasOne(e => e.Source);
        modelBuilder.Entity<DbValueSetConceptComparison>()
            .HasOne(e => e.Target);
        modelBuilder.Entity<DbValueSetConceptComparison>()
            .Property(e => e.Relationship)
            .HasConversion(
                v => v.HasValue ? EnumUtility.GetLiteral(v.Value) : null,
                v => EnumUtility.ParseLiteral<Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship>(v, true));
        modelBuilder.Entity<DbValueSetConceptComparison>()
            .HasIndex(nameof(DbValueSetConceptComparison.CanonicalComparisonKey));
        modelBuilder.Entity<DbValueSetConceptComparison>()
            .HasIndex(nameof(DbValueSetConceptComparison.SourceCanonicalKey));
        modelBuilder.Entity<DbValueSetConceptComparison>()
            .HasIndex(nameof(DbValueSetConceptComparison.SourceKey));
        modelBuilder.Entity<DbValueSetConceptComparison>()
            .ToTable("ValueSetConceptComparisons");


        modelBuilder.Entity<DbInvalidConceptComparison>()
            .HasKey(nameof(DbInvalidConceptComparison.Key));
        modelBuilder.Entity<DbInvalidConceptComparison>()
            .HasOne(e => e.SourceFhirPackage);
        modelBuilder.Entity<DbInvalidConceptComparison>()
            .HasOne(e => e.TargetFhirPackage);
        modelBuilder.Entity<DbInvalidConceptComparison>()
            .HasOne(e => e.PackageComparison)
            .WithMany(e => e.InvalidImportedConceptComparisons)
            .HasForeignKey(e => e.PackageComparisonKey)
            .HasPrincipalKey(e => e.Key);
        modelBuilder.Entity<DbInvalidConceptComparison>()
            .HasOne(e => e.CanonicalComparison);
        modelBuilder.Entity<DbInvalidConceptComparison>()
            .HasOne(e => e.SourceCanonical);
        modelBuilder.Entity<DbInvalidConceptComparison>()
            .HasOne(e => e.TargetCanonical);
        modelBuilder.Entity<DbInvalidConceptComparison>()
            .Property(e => e.Relationship)
            .HasConversion(
                v => v.HasValue ? EnumUtility.GetLiteral(v.Value) : null,
                v => EnumUtility.ParseLiteral<Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship>(v, true));
        modelBuilder.Entity<DbInvalidConceptComparison>()
            .ToTable("InvalidImportedConceptComparisons");


        modelBuilder.Entity<DbStructureDefinition>()
            .HasKey(nameof(DbStructureDefinition.Key));
        modelBuilder.Entity<DbStructureDefinition>()
            .Property(e => e.ArtifactClass)
            .HasConversion(
                v => v.ToString(),
                v => (FhirArtifactClassEnum)Enum.Parse(typeof(FhirArtifactClassEnum), v));
        modelBuilder.Entity<DbStructureDefinition>()
            .Property(e => e.Status)
            .HasConversion(
                v => v.HasValue ? EnumUtility.GetLiteral(v.Value) : null,
                v => EnumUtility.ParseLiteral<Hl7.Fhir.Model.PublicationStatus>(v, true));
        modelBuilder.Entity<DbStructureDefinition>()
            .HasOne(e => e.FhirPackage);
        modelBuilder.Entity<DbStructureDefinition>()
            .HasMany(e => e.Elements)
            .WithOne(e => e.Structure)
            .HasForeignKey(e => e.StructureKey);
        modelBuilder.Entity<DbStructureDefinition>()
            .HasIndex(nameof(DbStructureDefinition.FhirPackageKey));
        modelBuilder.Entity<DbStructureDefinition>()
            .HasIndex(nameof(DbStructureDefinition.VersionedUrl));
        modelBuilder.Entity<DbStructureDefinition>()
            .HasIndex(nameof(DbStructureDefinition.VersionedUrl), nameof(DbStructureDefinition.Version));
        modelBuilder.Entity<DbStructureDefinition>()
            .HasIndex(nameof(DbStructureDefinition.Name));
        modelBuilder.Entity<DbStructureDefinition>()
            .HasIndex(nameof(DbStructureDefinition.Name), nameof(DbStructureDefinition.Version));
        modelBuilder.Entity<DbStructureDefinition>()
            .HasIndex(nameof(DbStructureDefinition.FhirPackageKey), nameof(DbStructureDefinition.VersionedUrl));
        modelBuilder.Entity<DbStructureDefinition>()
            .HasIndex(nameof(DbStructureDefinition.FhirPackageKey), nameof(DbStructureDefinition.Name));
        modelBuilder.Entity<DbStructureDefinition>()
            .ToTable("Structures");

        modelBuilder.Entity<DbElement>()
            .HasKey(nameof(DbElement.Key));
        modelBuilder.Entity<DbElement>()
            .HasOne(e => e.Structure)
            .WithMany(e => e.Elements)
            .HasForeignKey(e => e.StructureKey);
        modelBuilder.Entity<DbElement>()
            .HasOne(e => e.FhirPackage);
        //modelBuilder.Entity<DbElementDefinition>()
        //    .HasMany(e => e.ElementTypes)
        //    .WithMany(e => e.Elements)
        //    .UsingEntity<DbElementTypeMap>(
        //        l => l.HasOne<DbElementType>().WithMany(e => e.ElementTypeMappings).HasPrincipalKey(e => e.Key).HasForeignKey(e => e.ElementKey),
        //        r => r.HasOne<DbElementDefinition>().WithMany(e => e.ElementTypeMappings).HasPrincipalKey(e => e.Key).HasForeignKey(e => e.ElementTypeKey));
        modelBuilder.Entity<DbElement>()
            .Property(e => e.ValueSetBindingStrength)
            .HasConversion(
                v => v.HasValue ? EnumUtility.GetLiteral(v.Value) : null,
                v => EnumUtility.ParseLiteral<Hl7.Fhir.Model.BindingStrength>(v, true));
        modelBuilder.Entity<DbElement>()
            .HasIndex(nameof(DbElement.Id));
        modelBuilder.Entity<DbElement>()
            .HasIndex(nameof(DbElement.Path));
        modelBuilder.Entity<DbElement>()
            .HasIndex(nameof(DbElement.ResourceFieldOrder));
        modelBuilder.Entity<DbElement>()
            .ToTable("Elements");


        //modelBuilder.Entity<DbElementType>()
        //    .HasKey(nameof(DbElementType.Key));
        ////modelBuilder.Entity<DbElementType>()
        ////    .HasMany(e => e.Elements)
        ////    .WithMany(e => e.ElementTypes)
        ////    .UsingEntity<DbElementTypeMap>(
        ////        l => l.HasOne<DbElementDefinition>().WithMany(e => e.ElementTypeMappings).HasPrincipalKey(e => e.Key).HasForeignKey(e => e.ElementTypeKey),
        ////        r => r.HasOne<DbElementType>().WithMany(e => e.ElementTypeMappings).HasPrincipalKey(e => e.Key).HasForeignKey(e => e.ElementKey));
        //modelBuilder.Entity<DbElementType>()
        //    .Property(e => e.ValueSetBindingStrength)
        //    .HasConversion(
        //        v => v.HasValue ? EnumUtility.GetLiteral(v.Value) : null,
        //        v => EnumUtility.ParseLiteral<Hl7.Fhir.Model.BindingStrength>(v, true));
        //modelBuilder.Entity<DbElementType>()
        //    .HasIndex(nameof(DbElementType.Name));
        //modelBuilder.Entity<DbElementType>()
        //    .HasIndex(nameof(DbElementType.Name), nameof(DbElementType.Profile), nameof(DbElementType.TargetProfile));
        //modelBuilder.Entity<DbElementType>()
        //    .HasIndex(
        //        nameof(DbElementType.Name),
        //        nameof(DbElementType.Profile),
        //        nameof(DbElementType.TargetProfile),
        //        nameof(DbElementType.ValueSetBindingStrength),
        //        nameof(DbElementType.BindingValueSet));
        //modelBuilder.Entity<DbElementType>()
        //    .ToTable("ElementTypes");

        ////modelBuilder.Entity<DbElementTypeMap>()
        ////    .HasKey(nameof(DbElementTypeMap.Key));
        ////modelBuilder.Entity<DbElementTypeMap>()
        ////    .HasOne(e => e.Element)
        ////    .WithMany(e => e.ElementTypeMappings)
        ////    .HasForeignKey(e => e.ElementKey)
        ////    .HasPrincipalKey(e => e.Key);
        ////modelBuilder.Entity<DbElementTypeMap>()
        ////    .HasOne(e => e.ElementType)
        ////    .WithMany(e => e.ElementTypeMappings)
        ////    .HasForeignKey(e => e.ElementTypeKey)
        ////    .HasPrincipalKey(e => e.Key);
        ////modelBuilder.Entity<DbElementTypeMap>()
        ////    .ToTable("ElementTypeMappings");




        //// add default value for Guid properties with DatabaseGeneratedAttribute
        //foreach (EntityFrameworkCore.Metadata.IMutableEntityType? entity in modelBuilder.Model.GetEntityTypes()
        //    .Where(t => t.ClrType.GetProperties().Any(p => p.CustomAttributes.Any(a => a.AttributeType == typeof(DatabaseGeneratedAttribute)))))
        //{
        //    foreach (var property in entity.ClrType.GetProperties()
        //        .Where(p => p.PropertyType == typeof(Guid) && p.CustomAttributes.Any(a => a.AttributeType == typeof(DatabaseGeneratedAttribute))))
        //    {
        //        modelBuilder
        //            .Entity(entity.ClrType)
        //            .Property(property.Name)
        //            .HasDefaultValueSql("NEWID()");
        //    }
        //}
    }
}
