// <copyright file="DiffDbContext.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Models;

namespace Microsoft.Health.Fhir.Comparison.CompareTool;

public class DiffDbContext : DbContext
{
    public string DbPath { get; private set; }

    public DiffDbContext(string dbPath)
    {
        DbPath = dbPath;
    }

    //public DiffDbContext(string dbPath, DbContextOptions<DiffDbContext> options)
    //    : base(options)
    //{
    //    DbPath = dbPath;
    //}

    public DbSet<DbFhirPackage> Packages { get; set; }
    public DbSet<DbFhirPackageComparisonPair> PackageDiffPairs { get; set; }

    public DbSet<DbValueSet> ValueSets { get; set; }
    public DbSet<DbValueSetConcept> Concepts { get; set; }

    public DbSet<ValueSetPairComparison> ValueSetComparisons { get; set; }
    public DbSet<ValueSetCodeComparisonRec> ValueSetCodeComparisons { get; set; }

    public DbSet<DbStructureDefinition> StructureDefinitions { get; set; }
    public DbSet<DbElementDefinition> ElementDefinitions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options) =>
        options
            //.UseLazyLoadingProxies()
            .UseSqlite($"Data Source={DbPath}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbFhirPackage>()
            .HasKey(nameof(DbFhirPackage.Key));
        modelBuilder.Entity<DbFhirPackage>()
            .HasMany(e => e.SourceDiffs)
            .WithOne(e => e.FhirPackage)
            .HasForeignKey(e => e.FhirPackageKey)
            .HasPrincipalKey(e => e.Key);
        modelBuilder.Entity<DbFhirPackage>()
            .HasMany(e => e.TargetDiffs)
            .WithOne(e => e.TargetPackage)
            .HasForeignKey(e => e.TargetPackageKey)
            .HasPrincipalKey(e => e.Key);
        modelBuilder.Entity<DbFhirPackage>()
            .HasMany(e => e.ValueSets)
            .WithOne(e => e.FhirPackage)
            .HasForeignKey(e => e.FhirPackageKey)
            .HasPrincipalKey(e => e.Key);
        modelBuilder.Entity<DbFhirPackage>()
            .ToTable("FhirPackages");

        modelBuilder.Entity<DbFhirPackageComparisonPair>()
            .HasKey(nameof(DbFhirPackageComparisonPair.Key));
        modelBuilder.Entity<DbFhirPackageComparisonPair>()
            .HasOne(e => e.FhirPackage);
        modelBuilder.Entity<DbFhirPackageComparisonPair>()
            .ToTable("FhirPackageComparisonPairs");

        modelBuilder.Entity<DbValueSet>()
            .HasKey(nameof(DbValueSet.Key));
        modelBuilder.Entity<DbValueSet>()
            .Property(e => e.Status)
            .HasConversion(
                v => v.HasValue ? EnumUtility.GetLiteral(v.Value) : null,
                v => EnumUtility.ParseLiteral<Hl7.Fhir.Model.PublicationStatus>(v, true));
        modelBuilder.Entity<DbValueSet>()
            .HasOne(e => e.FhirPackage);
        modelBuilder.Entity<DbValueSet>()
            .HasMany(e => e.Concepts)
            .WithOne(e => e.ValueSet)
            .HasForeignKey(e => e.ValueSetKey);
        modelBuilder.Entity<DbValueSet>()
            .HasMany(e => e.ComparisonsAsSource)
            .WithOne(e => e.SourceVsMeta)
            .HasForeignKey(e => e.SourceVsMetaKey)
            .HasPrincipalKey(e => e.Key);
        modelBuilder.Entity<DbValueSet>()
            .HasMany(e => e.ComparisonsAsTarget)
            .WithOne(e => e.TargetVsMeta)
            .HasForeignKey(e => e.TargetVsMetaKey)
            .HasPrincipalKey(e => e.Key);
        modelBuilder.Entity<DbValueSet>()
            .HasIndex(nameof(DbValueSet.FhirPackageKey));
        modelBuilder.Entity<DbValueSet>()
            .HasIndex(nameof(DbValueSet.Url));
        modelBuilder.Entity<DbValueSet>()
            .HasIndex(nameof(DbValueSet.Url), nameof(DbValueSet.Version));
        modelBuilder.Entity<DbValueSet>()
            .HasIndex(nameof(DbValueSet.Name));
        modelBuilder.Entity<DbValueSet>()
            .HasIndex(nameof(DbValueSet.Name), nameof(DbValueSet.Version));
        modelBuilder.Entity<DbValueSet>()
            .HasIndex(nameof(DbValueSet.FhirPackageKey), nameof(DbValueSet.Url));
        modelBuilder.Entity<DbValueSet>()
            .HasIndex(nameof(DbValueSet.FhirPackageKey), nameof(DbValueSet.Name));
        modelBuilder.Entity<DbValueSet>()
            .ToTable("ValueSets");


        modelBuilder.Entity<DbValueSetConcept>()
            .HasKey(nameof(DbValueSetConcept.Key));
        modelBuilder.Entity<DbValueSetConcept>()
            .HasOne(e => e.ValueSet);
        modelBuilder.Entity<DbValueSetConcept>()
            .HasOne(e => e.FhirPackage);
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


        modelBuilder.Entity<ValueSetPairComparison>()
            .HasKey(nameof(ValueSetPairComparison.Key));
        modelBuilder.Entity<ValueSetPairComparison>()
            .HasOne(e => e.SourceVsMeta);
        modelBuilder.Entity<ValueSetPairComparison>()
            .HasOne(e => e.TargetVsMeta);
        modelBuilder.Entity<ValueSetPairComparison>()
            .HasMany(e => e.CodeComparisons)
            .WithOne(e => e.VsPairComparison)
            .HasForeignKey(e => e.VsPairComparisonKey)
            .HasPrincipalKey(e => e.Key);
        modelBuilder.Entity<ValueSetPairComparison>()
            .HasIndex(nameof(ValueSetPairComparison.SourceVsMetaKey));
        modelBuilder.Entity<ValueSetPairComparison>()
            .HasIndex(nameof(ValueSetPairComparison.SourceName));
        modelBuilder.Entity<ValueSetPairComparison>()
            .HasIndex(nameof(ValueSetPairComparison.SourceCanonical));
        modelBuilder.Entity<ValueSetPairComparison>()
            .HasIndex(nameof(ValueSetPairComparison.SourceVersion));
        modelBuilder.Entity<ValueSetPairComparison>()
            .HasIndex(nameof(ValueSetPairComparison.SourceCanonical), nameof(ValueSetPairComparison.SourceVersion));
        modelBuilder.Entity<ValueSetPairComparison>()
            .HasIndex(nameof(ValueSetPairComparison.SourceName), nameof(ValueSetPairComparison.SourceVersion));
        modelBuilder.Entity<ValueSetPairComparison>()
            .HasIndex(nameof(ValueSetPairComparison.TargetVsMetaKey));
        modelBuilder.Entity<ValueSetPairComparison>()
            .HasIndex(nameof(ValueSetPairComparison.TargetName));
        modelBuilder.Entity<ValueSetPairComparison>()
            .HasIndex(nameof(ValueSetPairComparison.TargetCanonical));
        modelBuilder.Entity<ValueSetPairComparison>()
            .HasIndex(nameof(ValueSetPairComparison.TargetVersion));
        modelBuilder.Entity<ValueSetPairComparison>()
            .HasIndex(nameof(ValueSetPairComparison.TargetCanonical), nameof(ValueSetPairComparison.TargetVersion));
        modelBuilder.Entity<ValueSetPairComparison>()
            .HasIndex(nameof(ValueSetPairComparison.TargetName), nameof(ValueSetPairComparison.TargetVersion));
        modelBuilder.Entity<ValueSetPairComparison>()
            .ToTable("ValueSetComparisons");

        modelBuilder.Entity<ValueSetCodeComparisonRec>()
            .HasKey(nameof(ValueSetCodeComparisonRec.Key));
        modelBuilder.Entity<ValueSetCodeComparisonRec>()
            .HasOne(e => e.VsPairComparison);
        modelBuilder.Entity<ValueSetCodeComparisonRec>()
            .HasIndex(nameof(ValueSetCodeComparisonRec.VsPairComparisonKey));
        modelBuilder.Entity<ValueSetCodeComparisonRec>()
            .HasIndex(nameof(ValueSetCodeComparisonRec.SourceSystem));
        modelBuilder.Entity<ValueSetCodeComparisonRec>()
            .HasIndex(nameof(ValueSetCodeComparisonRec.SourceCode));
        modelBuilder.Entity<ValueSetCodeComparisonRec>()
            .HasIndex(nameof(ValueSetCodeComparisonRec.SourceSystem), nameof(ValueSetCodeComparisonRec.SourceCode));
        modelBuilder.Entity<ValueSetCodeComparisonRec>()
            .HasIndex(nameof(ValueSetCodeComparisonRec.TargetSystem));
        modelBuilder.Entity<ValueSetCodeComparisonRec>()
            .HasIndex(nameof(ValueSetCodeComparisonRec.TargetCode));
        modelBuilder.Entity<ValueSetCodeComparisonRec>()
            .HasIndex(nameof(ValueSetCodeComparisonRec.TargetSystem), nameof(ValueSetCodeComparisonRec.TargetCode));
        modelBuilder.Entity<ValueSetCodeComparisonRec>()
            .ToTable("ValueSetConceptComparisons");







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
        //modelBuilder.Entity<StructureDefinitionMetadata>()
        //    .HasMany(e => e.ComparisonsAsSource)
        //    .WithOne(e => e.SourceSdMeta)
        //    .HasForeignKey(e => e.SourceSdMetaKey)
        //    .HasPrincipalKey(e => e.Key);
        //modelBuilder.Entity<StructureDefinitionMetadata>()
        //    .HasMany(e => e.ComparisonsAsTarget)
        //    .WithOne(e => e.TargetSdMeta)
        //    .HasForeignKey(e => e.TargetSdMetaKey)
        //    .HasPrincipalKey(e => e.Key);
        modelBuilder.Entity<DbStructureDefinition>()
            .HasIndex(nameof(DbStructureDefinition.FhirPackageKey));
        modelBuilder.Entity<DbStructureDefinition>()
            .HasIndex(nameof(DbStructureDefinition.Url));
        modelBuilder.Entity<DbStructureDefinition>()
            .HasIndex(nameof(DbStructureDefinition.Url), nameof(DbStructureDefinition.Version));
        modelBuilder.Entity<DbStructureDefinition>()
            .HasIndex(nameof(DbStructureDefinition.Name));
        modelBuilder.Entity<DbStructureDefinition>()
            .HasIndex(nameof(DbStructureDefinition.Name), nameof(DbStructureDefinition.Version));
        modelBuilder.Entity<DbStructureDefinition>()
            .HasIndex(nameof(DbStructureDefinition.FhirPackageKey), nameof(DbStructureDefinition.Url));
        modelBuilder.Entity<DbStructureDefinition>()
            .HasIndex(nameof(DbStructureDefinition.FhirPackageKey), nameof(DbStructureDefinition.Name));
        modelBuilder.Entity<DbStructureDefinition>()
            .ToTable("Structures");


        modelBuilder.Entity<DbElementDefinition>()
            .HasKey(nameof(DbElementDefinition.Key));
        modelBuilder.Entity<DbElementDefinition>()
            .HasOne(e => e.Structure)
            .WithMany(e => e.Elements)
            .HasForeignKey(e => e.StructureKey);
        modelBuilder.Entity<DbElementDefinition>()
            .HasIndex(nameof(DbElementDefinition.Id));
        modelBuilder.Entity<DbElementDefinition>()
            .HasIndex(nameof(DbElementDefinition.Path));
        modelBuilder.Entity<DbElementDefinition>()
            .HasIndex(nameof(DbElementDefinition.FieldOrder));
        modelBuilder.Entity<DbElementDefinition>()
            .ToTable("Elements");




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
