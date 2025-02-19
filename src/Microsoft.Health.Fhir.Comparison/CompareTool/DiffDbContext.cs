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
using Microsoft.EntityFrameworkCore;

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

    public DbSet<PackageMetadata> Packages { get; set; }
    public DbSet<PackageDiffPair> PackageDiffPairs { get; set; }

    public DbSet<ValueSetMetadata> ValueSets { get; set; }
    public DbSet<ValueSetConcept> Concepts { get; set; }
    public DbSet<ValueSetConceptMapping> ConceptMappings { get; set; }

    public DbSet<ValueSetPairComparison> ValueSetComparisons { get; set; }
    public DbSet<ValueSetCodeComparisonRec> ValueSetCodeComparisons { get; set; }

    public DbSet<StructureDefinitionMetadata> StructureDefinitions { get; set; }
    public DbSet<StructureElement> Elements { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options) =>
        options
            //.UseLazyLoadingProxies()
            .UseSqlite($"Data Source={DbPath}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PackageMetadata>()
            .HasKey(nameof(PackageMetadata.Key));
        modelBuilder.Entity<PackageMetadata>()
            .HasMany(e => e.SourceDiffs)
            .WithOne(e => e.SourcePackage)
            .HasForeignKey(e => e.SourcePackageKey)
            .HasPrincipalKey(e => e.Key);
        modelBuilder.Entity<PackageMetadata>()
            .HasMany(e => e.TargetDiffs)
            .WithOne(e => e.TargetPackage)
            .HasForeignKey(e => e.TargetPackageKey)
            .HasPrincipalKey(e => e.Key);
        modelBuilder.Entity<PackageMetadata>()
            .HasMany(e => e.ValueSets)
            .WithOne(e => e.ContainingPackage)
            .HasForeignKey(e => e.ContainingPackageKey)
            .HasPrincipalKey(e => e.Key);
        modelBuilder.Entity<PackageMetadata>()
            .ToTable("Packages");

        modelBuilder.Entity<PackageDiffPair>()
            .HasKey(nameof(PackageDiffPair.Key));
        modelBuilder.Entity<PackageDiffPair>()
            .HasOne(e => e.SourcePackage);
        modelBuilder.Entity<PackageDiffPair>()
            .ToTable("PackageDiffPairs");

        modelBuilder.Entity<ValueSetMetadata>()
            .HasKey(nameof(ValueSetMetadata.Key));
        modelBuilder.Entity<ValueSetMetadata>()
            .HasOne(e => e.ContainingPackage);
        modelBuilder.Entity<ValueSetMetadata>()
            .HasMany(e => e.Concepts)
            .WithMany(e => e.ValueSets)
            .UsingEntity<ValueSetConceptMapping>();
        modelBuilder.Entity<ValueSetMetadata>()
            .HasMany(e => e.ComparisonsAsSource)
            .WithOne(e => e.SourceVsMeta)
            .HasForeignKey(e => e.SourceVsMetaKey)
            .HasPrincipalKey(e => e.Key);
        modelBuilder.Entity<ValueSetMetadata>()
            .HasMany(e => e.ComparisonsAsTarget)
            .WithOne(e => e.TargetVsMeta)
            .HasForeignKey(e => e.TargetVsMetaKey)
            .HasPrincipalKey(e => e.Key);
        modelBuilder.Entity<ValueSetMetadata>()
            .HasIndex(nameof(ValueSetMetadata.ContainingPackageKey));
        modelBuilder.Entity<ValueSetMetadata>()
            .HasIndex(nameof(ValueSetMetadata.CanonicalUrl));
        modelBuilder.Entity<ValueSetMetadata>()
            .HasIndex(nameof(ValueSetMetadata.CanonicalUrl), nameof(ValueSetMetadata.Version));
        modelBuilder.Entity<ValueSetMetadata>()
            .ToTable("ValueSets");


        modelBuilder.Entity<ValueSetConcept>()
            .HasKey(nameof(ValueSetConcept.Key));
        modelBuilder.Entity<ValueSetConcept>()
            .HasMany(e => e.ValueSets)
            .WithMany(e => e.Concepts)
            .UsingEntity<ValueSetConceptMapping>();
        modelBuilder.Entity<ValueSetConcept>()
            .HasIndex(nameof(ValueSetConcept.System));
        modelBuilder.Entity<ValueSetConcept>()
            .HasIndex(nameof(ValueSetConcept.Code));
        modelBuilder.Entity<ValueSetConcept>()
            .HasIndex(nameof(ValueSetConcept.System), nameof(ValueSetConcept.Code));
        modelBuilder.Entity<ValueSetConcept>()
            .ToTable("Concepts");

        modelBuilder.Entity<ValueSetConceptMapping>()
            .HasKey(nameof(ValueSetConceptMapping.Key));
        modelBuilder.Entity<ValueSetConceptMapping>()
            .HasIndex(nameof(ValueSetConceptMapping.VsMetaKey));
        modelBuilder.Entity<ValueSetConceptMapping>()
            .HasIndex(nameof(ValueSetConceptMapping.VsConceptKey));
        modelBuilder.Entity<ValueSetConceptMapping>()
            .ToTable("ValueSetConceptMappings");


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
            .ToTable("ValueSetCodeComparisons");







        modelBuilder.Entity<StructureDefinitionMetadata>()
            .HasKey(nameof(StructureDefinitionMetadata.Key));
        modelBuilder.Entity<StructureDefinitionMetadata>()
            .HasOne(e => e.ContainingPackage);
        modelBuilder.Entity<StructureDefinitionMetadata>()
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
        modelBuilder.Entity<StructureDefinitionMetadata>()
            .HasIndex(nameof(StructureDefinitionMetadata.ContainingPackageKey));
        modelBuilder.Entity<StructureDefinitionMetadata>()
            .HasIndex(nameof(StructureDefinitionMetadata.CanonicalUrl));
        modelBuilder.Entity<StructureDefinitionMetadata>()
            .HasIndex(nameof(StructureDefinitionMetadata.CanonicalUrl), nameof(ValueSetMetadata.Version));
        modelBuilder.Entity<StructureDefinitionMetadata>()
            .ToTable("Structures");


        modelBuilder.Entity<StructureElement>()
            .HasKey(nameof(StructureElement.Key));
        modelBuilder.Entity<StructureElement>()
            .HasOne(e => e.Structure)
            .WithMany(e => e.Elements)
            .HasForeignKey(e => e.StructureKey);
        modelBuilder.Entity<StructureElement>()
            .HasIndex(nameof(StructureElement.Id));
        modelBuilder.Entity<StructureElement>()
            .HasIndex(nameof(StructureElement.Path));
        modelBuilder.Entity<StructureElement>()
            .HasIndex(nameof(StructureElement.FieldOrder));
        modelBuilder.Entity<StructureElement>()
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
