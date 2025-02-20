using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGenCommon.Models;

namespace Microsoft.Health.Fhir.Comparison.Models;


public abstract class DbCanonicalResource
{
    [Key]
    public int Key { get; set; }

    public int FhirPackageKey { get; set; }
    public required DbFhirPackage FhirPackage { get; init; } = null!;

    public required string Id { get; set; } = null!;
    public required string Url { get; set; } = null!;
    public required string Name { get; set; } = null!;
    public required string Version { get; set; } = null!;
    public required PublicationStatus? Status { get; set; } = null;
    public required string? Title { get; set; } = null;
    public required string? Description { get; set; } = null;
    public required string? Purpose { get; set; } = null;
}


public class DbValueSet : DbCanonicalResource
{
    public required bool CanExpand { get; set; }
    public required bool? HasEscapeValveCode { get; set; } = null;
    public required string? Message { get; set; } = null;

    public required int ConceptCount { get; set; } = -1;
    public required string? ReferencedSystems { get; set; } = null;

    public required int BindingCountCore { get; set; } = -1;
    public required BindingStrength? StrongestBindingCore { get; set; } = null;
    public required BindingStrength? StrongestBindingCoreCode { get; set; } = null;
    public required BindingStrength? StrongestBindingCoreCoding { get; set; } = null;

    public required int BindingCountExtended { get; set; } = -1;
    public required BindingStrength? StrongestBindingExtended { get; set; } = null;
    public required BindingStrength? StrongestBindingExtendedCode { get; set; } = null;
    public required BindingStrength? StrongestBindingExtendedCoding { get; set; } = null;

    public ICollection<DbValueSetConcept> Concepts { get; init; } = null!;

    public ICollection<ValueSetPairComparison> ComparisonsAsSource { get; init; } = null!;

    public ICollection<ValueSetPairComparison> ComparisonsAsTarget { get; init; } = null!;
}

public class DbValueSetConcept
{
    [Key]
    public int Key { get; set; }

    public int ValueSetKey { get; set; }
    public required DbValueSet ValueSet { get; set; } = null!;

    public int FhirPackageKey { get; set; }
    public required DbFhirPackage FhirPackage { get; init; } = null!;


    public required string System { get; set; } = null!;
    public required string Code { get; set; } = null!;
    public required string? Display { get; set; } = null;
}



public class DbStructureDefinition : DbCanonicalResource
{
    public required string? Comment { get; set; } = null;
    public required string? Message { get; set; } = null;

    public required FhirArtifactClassEnum ArtifactClass { get; set; } = FhirArtifactClassEnum.Unknown;

    public ICollection<DbElement> Elements { get; init; } = null!;

    //public ICollection<ValueSetPairComparison> ComparisonsAsSource { get; init; } = null!;

    //public ICollection<ValueSetPairComparison> ComparisonsAsTarget { get; init; } = null!;
}


public class DbElement
{
    [Key]
    public int Key { get; set; }

    public int StructureKey { get; set; }
    public required DbStructureDefinition Structure { get; init; } = null!;

    public int FhirPackageKey { get; set; }
    public required DbFhirPackage FhirPackage { get; init; } = null!;


    public required int ResourceFieldOrder { get; set; } = -1;
    public required int ComponentFieldOrder { get; set; } = -1;
    public required string Id { get; set; } = null!;
    public required string Path { get; set; } = null!;
    public required int ChildElementCount { get; set; } = -1;
    public required string Name { get; set; } = null!;
    public required string? Short { get; set; } = null;
    public required string? Definition { get; set; } = null;
    public required int MinCardinality { get; set; } = -1;
    public required int MaxCardinality { get; set; } = -1;
    public required string MaxCardinalityString { get; set; } = null!;

    public required string? SliceName { get; set; } = null;

    public required string TypeName { get; set; } = null!;
    public required string? TypeProfile { get; set; } = null;
    public required string? TargetProfile { get; set; } = null;

    public required BindingStrength? ValueSetBindingStrength { get; init; } = null;
    public required string? BindingValueSet { get; set; } = null;


    //public ICollection<DbElementType> ElementTypes { get; set; } = null!;
    //public ICollection<DbElementTypeMap> ElementTypeMappings { get; set; } = null!;
}

//public class DbElementType
//{
//    [Key]
//    public int Key { get; set; }

//    public ICollection<DbElementDefinition> Elements { get; set; } = null!;
//    public ICollection<DbElementTypeMap> ElementTypeMappings { get; set; } = null!;

//    public required string Name { get; set; } = null!;
//    public required string? Profile { get; set; } = null;
//    public required string? TargetProfile { get; set; } = null;

//    public required BindingStrength? ValueSetBindingStrength { get; init; } = null;
//    public required string? BindingValueSet { get; set; } = null;
//}

//public class DbElementTypeMap
//{
//    [Key]
//    public int Key { get; set; }

//    public required int ElementKey { get; set; } = -1;
//    public DbElementDefinition Element { get; set; } = null!;

//    public required int ElementTypeKey { get; set; } = -1;
//    public DbElementType ElementType { get; set; } = null!;

//}
