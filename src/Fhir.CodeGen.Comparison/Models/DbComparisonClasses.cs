using Fhir.CodeGen.SQLiteGenerator;
using System.Diagnostics.CodeAnalysis;
using System.Data;

namespace Fhir.CodeGen.Comparison.Models;

public enum BidirectionalRelationshipCodes
{
    None = 0,
    Equivalent,
    NewerNarrows,
    NewerBroadens,
    Related,
    NotRelated,
    Mismatched,
}

public interface IDbComparisonCell
{
    IDbComparisonCell? LeftCell { get; }
    DbPackageComparisonContent? LeftComparison { get; }

    IDbComparisonCell? RightCell { get; }
    DbPackageComparisonContent? RightComparison { get; }
}

public static class DbComparisonCellExtensions
{
    public static string ToRightMessage(this IDbComparisonCell c) => (c.RightComparison == null)
        ? string.Empty
        : $"{c.RightComparison.Relationship}: {c.RightComparison.TechnicalMessage}";

    public static string FromRightMessage(this IDbComparisonCell c) => (c.RightCell?.LeftComparison == null)
        ? string.Empty
        : $"{c.RightCell.LeftComparison.Relationship}: {c.RightCell.LeftComparison.TechnicalMessage}";

    public static string ToLeftMessage(this IDbComparisonCell c) => (c.LeftComparison == null)
        ? string.Empty
        : $"{c.LeftComparison.Relationship}: {c.LeftComparison.TechnicalMessage}";
    public static string FromLeftMessage(this IDbComparisonCell c) => (c.LeftCell?.RightComparison == null)
        ? string.Empty
        : $"{c.LeftCell.RightComparison.Relationship}: {c.LeftCell.RightComparison.TechnicalMessage}";

    public static Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? LeftToRight(this BidirectionalRelationshipCodes br) => br switch
    {
        BidirectionalRelationshipCodes.Equivalent => Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.Equivalent,
        BidirectionalRelationshipCodes.NewerNarrows => Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.SourceIsBroaderThanTarget,
        BidirectionalRelationshipCodes.NewerBroadens => Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.SourceIsNarrowerThanTarget,
        BidirectionalRelationshipCodes.Related => Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.RelatedTo,
        BidirectionalRelationshipCodes.NotRelated => Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.NotRelatedTo,
        _ => null,
    };

    public static Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? RightToLeft(this BidirectionalRelationshipCodes br) => br switch
    {
        BidirectionalRelationshipCodes.Equivalent => Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.Equivalent,
        BidirectionalRelationshipCodes.NewerNarrows => Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.SourceIsNarrowerThanTarget,
        BidirectionalRelationshipCodes.NewerBroadens => Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.SourceIsBroaderThanTarget,
        BidirectionalRelationshipCodes.Related => Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.RelatedTo,
        BidirectionalRelationshipCodes.NotRelated => Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.NotRelatedTo,
        _ => null,
    };

    public static BidirectionalRelationshipCodes? BidirectionalRight(this IDbComparisonCell c)
    {
        if ((c.RightComparison == null) || (c.RightCell?.LeftComparison == null))
        {
            return null;
        }

        Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? up = c.RightComparison.Relationship;
        Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? down = c.RightCell.LeftComparison.Relationship;

        if ((up == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.Equivalent) &&
            (down == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.Equivalent))
        {
            return BidirectionalRelationshipCodes.Equivalent;
        }

        if ((up == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.SourceIsNarrowerThanTarget) &&
            (down == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.SourceIsBroaderThanTarget))
        {
            return BidirectionalRelationshipCodes.NewerBroadens;
        }

        if ((up == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.SourceIsBroaderThanTarget) &&
            (down == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.SourceIsNarrowerThanTarget))
        {
            return BidirectionalRelationshipCodes.NewerNarrows;
        }

        if ((up == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.RelatedTo) &&
            (down == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.RelatedTo))
        {
            return BidirectionalRelationshipCodes.Related;
        }

        if ((up == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.NotRelatedTo) &&
            (down == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.NotRelatedTo))
        {
            return BidirectionalRelationshipCodes.NotRelated;
        }

        return BidirectionalRelationshipCodes.Mismatched;
    }

    public static BidirectionalRelationshipCodes? BidirectionalLeft(this IDbComparisonCell c)
    {
        if ((c.LeftComparison == null) || (c.LeftCell?.RightComparison == null))
        {
            return null;
        }

        Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? up = c.LeftCell.RightComparison.Relationship;
        Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? down = c.LeftComparison.Relationship;

        if ((up == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.Equivalent) &&
            (down == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.Equivalent))
        {
            return BidirectionalRelationshipCodes.Equivalent;
        }

        if ((up == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.SourceIsNarrowerThanTarget) &&
            (down == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.SourceIsBroaderThanTarget))
        {
            return BidirectionalRelationshipCodes.NewerBroadens;
        }

        if ((up == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.SourceIsBroaderThanTarget) &&
            (down == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.SourceIsNarrowerThanTarget))
        {
            return BidirectionalRelationshipCodes.NewerNarrows;
        }

        if ((up == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.RelatedTo) &&
            (down == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.RelatedTo))
        {
            return BidirectionalRelationshipCodes.Related;
        }

        if ((up == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.NotRelatedTo) &&
            (down == Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.NotRelatedTo))
        {
            return BidirectionalRelationshipCodes.NotRelated;
        }

        return BidirectionalRelationshipCodes.Mismatched;
    }
}


[CgSQLiteTable(tableName: "PackageComparisonPairs")]
public partial class DbFhirPackageComparisonPair : DbRecordBase
{
    public int InverseComparisonKey { get; set; } = -1;

    [CgSQLiteForeignKey(referenceTable: "FhirPackages", referenceColumn: nameof(DbFhirPackage.Key))]
    public required int SourcePackageKey { get; set; }

    public required string SourcePackageShortName { get; set; }

    [CgSQLiteForeignKey(referenceTable: "FhirPackages", referenceColumn: nameof(DbFhirPackage.Key))]
    public required int TargetPackageKey { get; set; }

    public required string TargetPackageShortName { get; set; }

    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}


public interface IDbPackageComparisonContent
{
    int Key { get; }
    int? InverseComparisonKey { get; }
    int PackageComparisonKey { get; }
    int SourceFhirPackageKey { get; }
    int TargetFhirPackageKey { get; }

    int SourceContentKey { get; }
    int? TargetContentKey { get; }
}

[CgSQLiteBaseClass]
public abstract class DbPackageComparisonContent : DbRecordBase
{
    public int? InverseComparisonKey { get; set; } = -1;

    [CgSQLiteForeignKey(referenceTable: "PackageComparisonPairs", referenceColumn: nameof(DbFhirPackageComparisonPair.Key))]
    public required int PackageComparisonKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "FhirPackages", referenceColumn: nameof(DbFhirPackage.Key))]
    public required int SourceFhirPackageKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "FhirPackages", referenceColumn: nameof(DbFhirPackage.Key))]
    public required int TargetFhirPackageKey { get; set; }

    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? Relationship { get; set; }
    public required string? TechnicalMessage { get; set; }
    public required string? UserMessage { get; set; }
    public required bool? IsGenerated { get; set; }
    public string? LastReviewedBy { get; set; } = null;
    public DateTime? LastReviewedOn { get; set; } = null;
}


[CgSQLiteTable(tableName: "ValueSetComparisons")]
[CgSQLiteIndex(nameof(PackageComparisonKey), nameof(SourceFhirPackageKey), nameof(TargetFhirPackageKey), nameof(SourceValueSetKey))]
[CgSQLiteIndex(nameof(PackageComparisonKey), nameof(SourceFhirPackageKey), nameof(SourceValueSetKey), nameof(TargetFhirPackageKey), nameof(TargetValueSetKey))]
[CgSQLiteIndex(nameof(SourceValueSetKey), nameof(TargetFhirPackageKey), nameof(TargetValueSetKey))]
public partial class DbValueSetComparison : DbPackageComparisonContent, IDbPackageComparisonContent
{
    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public required int SourceValueSetKey { get; set; }

    public required string SourceCanonicalVersioned { get; set; }
    public required string SourceCanonicalUnversioned { get; set; }
    public required string SourceName { get; set; }
    public required string SourceVersion { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public required int? TargetValueSetKey { get; set; }

    public required string? TargetCanonicalVersioned { get; set; }
    public required string? TargetCanonicalUnversioned { get; set; }
    public required string? TargetName { get; set; }
    public required string? TargetVersion { get; set; }

    public required string CompositeName { get; set; }
    public required string? SourceConceptMapUrl { get; set; }
    public required string? SourceConceptMapAdditionalUrls { get; set; }

    public required bool? IsIdentical { get; set; }
    public required bool? CodesAreIdentical { get; set; }

    [CgSQLiteIgnore]
    public int SourceContentKey => SourceValueSetKey;
    [CgSQLiteIgnore]
    public int? TargetContentKey => TargetValueSetKey;

    private static DbValueSetComparison _empty = EmptyCopy;

    [CgSQLiteIgnore]
    public static DbValueSetComparison Empty => _empty;

    [CgSQLiteIgnore]
    public static DbValueSetComparison EmptyCopy => new()
    {
        Key = -1,
        PackageComparisonKey = -1,
        SourceFhirPackageKey = -1,
        SourceValueSetKey = -1,
        SourceCanonicalVersioned = string.Empty,
        SourceCanonicalUnversioned = string.Empty,
        SourceName = string.Empty,
        SourceVersion = string.Empty,
        TargetFhirPackageKey = -1,
        TargetValueSetKey = null,
        TargetCanonicalVersioned = null,
        TargetCanonicalUnversioned = null,
        TargetName = null,
        TargetVersion = null,
        CompositeName = string.Empty,
        SourceConceptMapUrl = null,
        SourceConceptMapAdditionalUrls = null,
        Relationship = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.NotRelatedTo,
        TechnicalMessage = null,
        UserMessage = null,
        IsGenerated = false,
        IsIdentical = null,
        CodesAreIdentical = null,
    };
}

[CgSQLiteTable(tableName: "ValueSetConceptComparisons")]
[CgSQLiteIndex(nameof(ValueSetComparisonKey))]
[CgSQLiteIndex(nameof(ValueSetComparisonKey), nameof(SourceConceptKey), nameof(TargetConceptKey))]
[CgSQLiteIndex(nameof(ValueSetComparisonKey), nameof(SourceValueSetKey), nameof(SourceConceptKey), nameof(TargetFhirPackageKey))]
[CgSQLiteIndex(nameof(PackageComparisonKey), nameof(SourceFhirPackageKey), nameof(SourceValueSetKey), nameof(SourceConceptKey), nameof(TargetFhirPackageKey), nameof(TargetValueSetKey), nameof(TargetConceptKey))]
[CgSQLiteIndex(nameof(ValueSetComparisonKey), nameof(SourceValueSetKey), nameof(TargetConceptKey))]
[CgSQLiteIndex(nameof(SourceValueSetKey), nameof(SourceConceptKey), nameof(TargetFhirPackageKey))]
public partial class DbValueSetConceptComparison : DbPackageComparisonContent, IDbPackageComparisonContent
{
    [CgSQLiteForeignKey(referenceTable: "ValueSetComparisons", referenceColumn: nameof(DbValueSetComparison.Key))]
    public required int ValueSetComparisonKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public required int SourceValueSetKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public required int? TargetValueSetKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ValueSetConcepts", referenceColumn: nameof(DbValueSetConcept.Key))]
    public required int SourceConceptKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ValueSetConcepts", referenceColumn: nameof(DbValueSetConcept.Key))]
    public required int? TargetConceptKey { get; set; }

    public required bool? NoMap { get; set; }
    public required bool? IsIdentical { get; set; }
    public required bool? CodesAreIdentical { get; set; }

    [CgSQLiteIgnore]
    public int SourceContentKey => SourceConceptKey;
    [CgSQLiteIgnore]
    public int? TargetContentKey => TargetConceptKey;

    private static DbValueSetConceptComparison _empty = EmptyCopy;

    [CgSQLiteIgnore]
    public static DbValueSetConceptComparison Empty => _empty;

    [CgSQLiteIgnore]
    public static DbValueSetConceptComparison EmptyCopy => new()
    {
        Key = -1,
        PackageComparisonKey = -1,
        ValueSetComparisonKey = -1,
        SourceFhirPackageKey = -1,
        SourceValueSetKey = -1,
        SourceConceptKey = -1,
        TargetFhirPackageKey = -1,
        TargetValueSetKey = null,
        TargetConceptKey = null,
        Relationship = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.NotRelatedTo,
        TechnicalMessage = null,
        UserMessage = null,
        NoMap = null,
        IsGenerated = false,
        IsIdentical = null,
        CodesAreIdentical = null,
    };

}

[CgSQLiteTable(tableName: "UnresolvedConceptComparisons")]
public partial class DbUnresolvedConceptComparison : DbPackageComparisonContent
{
    [CgSQLiteForeignKey(referenceTable: "ValueSetComparisons", referenceColumn: nameof(DbValueSetComparison.Key))]
    public int ValueSetComparisonKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public required int SourceValueSetKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public required int? TargetValueSetKey { get; set; }


    public required string ConceptMapId { get; set; }
    public required string ConceptMapUrl { get; set; }


    [CgSQLiteForeignKey(referenceTable: "ValueSetConcept", referenceColumn: nameof(DbValueSetConcept.Key))]
    public required int? SourceConceptKey { get; set; }
    public required bool SourceConceptExists { get; set; } = false;
    public required string SourceSystem { get; set; }
    public required string SourceCode { get; set; }
    public required string? SourceDisplay { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ValueSetConcept", referenceColumn: nameof(DbValueSetConcept.Key))]
    public required int? TargetConceptKey { get; set; }
    public required bool? TargetConceptExists { get; set; }
    public required string? TargetSystem { get; set; }
    public required string? TargetCode { get; set; }
    public required string? TargetDisplay { get; set; }

    public required bool? NoMap { get; set; }
}

public enum StructureReviewTypeCodes : int
{
    None = 0,
    StructureMappings = 1,
    ElementMappings = 2,
    Complete = 3,
}

[CgSQLiteTable(tableName: "StructureComparisons")]
[CgSQLiteIndex(nameof(PackageComparisonKey), nameof(SourceFhirPackageKey), nameof(TargetFhirPackageKey), nameof(SourceStructureKey))]
[CgSQLiteIndex(nameof(PackageComparisonKey), nameof(SourceFhirPackageKey), nameof(SourceStructureKey), nameof(TargetFhirPackageKey), nameof(TargetStructureKey))]
public partial class DbStructureComparison : DbPackageComparisonContent, IDbPackageComparisonContent
{
    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key))]
    public required int SourceStructureKey { get; set; }

    public required string SourceCanonicalVersioned { get; set; }
    public required string SourceCanonicalUnversioned { get; set; }
    public required string SourceName { get; set; }
    public required string SourceVersion { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key))]
    public required int? TargetStructureKey { get; set; }
    public required string? TargetCanonicalVersioned { get; set; }
    public required string? TargetCanonicalUnversioned { get; set; }
    public required string? TargetName { get; set; }
    public required string? TargetVersion { get; set; }

    public required string CompositeName { get; set; }
    public required string? SourceOverviewConceptMapUrl { get; set; }
    public required string? SourceStructureFmlUrl { get; set; }

    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? ConceptDomainRelationship { get; set; }
    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? ValueDomainRelationship { get; set; }
    public required bool? IsIdentical { get; set; }

    public required StructureReviewTypeCodes? ReviewType { get; set; }

    [CgSQLiteIgnore]
    public int SourceContentKey => SourceStructureKey;
    [CgSQLiteIgnore]
    public int? TargetContentKey => TargetStructureKey;

}


[CgSQLiteTable(tableName: "UnresolvedStructureComparisons")]
public partial class DbUnresolvedStructureComparison : DbPackageComparisonContent
{
    public required string ConceptMapId { get; set; }
    public required string ConceptMapUrl { get; set; }


    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key))]
    public required int? SourceStructureKey { get; set; }

    public required string? SourceCanonicalVersioned { get; set; }
    public required string? SourceCanonicalUnversioned { get; set; }
    public required string? SourceName { get; set; }
    public required string? SourceVersion { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key))]
    public required int? TargetStructureKey { get; set; }
    public required string? TargetCanonicalVersioned { get; set; }
    public required string? TargetCanonicalUnversioned { get; set; }
    public required string? TargetName { get; set; }
    public required string? TargetVersion { get; set; }
}


[CgSQLiteTable(tableName: "ElementComparisons")]
[CgSQLiteIndex(nameof(StructureComparisonKey), nameof(SourceStructureKey), nameof(SourceElementKey), nameof(TargetFhirPackageKey))]
[CgSQLiteIndex(nameof(StructureComparisonKey), nameof(SourceElementKey), nameof(TargetElementKey))]
[CgSQLiteIndex(nameof(StructureComparisonKey))]
public partial class DbElementComparison : DbPackageComparisonContent, IDbPackageComparisonContent
{
    [CgSQLiteForeignKey(referenceTable: "StructureComparisons", referenceColumn: nameof(DbStructureComparison.Key))]
    public required int StructureComparisonKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key))]
    public required int SourceStructureKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key))]
    public required int? TargetStructureKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Elements", referenceColumn: nameof(DbElement.Key))]
    public required int SourceElementKey { get; set; }
    public required string SourceStructureUrl { get; set; }
    public required string SourceElementToken { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Elements", referenceColumn: nameof(DbElement.Key))]
    public required int? TargetElementKey { get; set; }
    public required string? TargetStructureUrl { get; set; }
    public required string? TargetElementToken { get; set; }

    public required bool? NoMap { get; set; }
    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? ConceptDomainRelationship { get; set; }
    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? ValueDomainRelationship { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ElementTypeComparisons", referenceColumn: nameof(DbCollatedTypeComparison.Key))]
    public required int? ElementTypeComparisonKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ValueSetComparisons", referenceColumn: nameof(DbValueSetComparison.Key))]
    public required int? BoundValueSetComparisonKey { get; set; }

    public required bool? IsIdentical { get; set; }

    [CgSQLiteIgnore]
    public int SourceContentKey => SourceElementKey;
    [CgSQLiteIgnore]
    public int? TargetContentKey => TargetElementKey;


    private static DbElementComparison _empty = EmptyCopy;

    [CgSQLiteIgnore]
    public static DbElementComparison Empty => _empty;

    [CgSQLiteIgnore]
    public static DbElementComparison EmptyCopy => new()
    {
        Key = -1,
        PackageComparisonKey = -1,
        StructureComparisonKey = -1,
        SourceFhirPackageKey = -1,
        SourceStructureKey = -1,
        SourceElementKey = -1,
        SourceStructureUrl = string.Empty,
        SourceElementToken = string.Empty,
        TargetFhirPackageKey = -1,
        TargetStructureKey = null,
        TargetElementKey = null,
        TargetStructureUrl = null,
        TargetElementToken = null,
        Relationship = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.NotRelatedTo,
        ConceptDomainRelationship = null,
        ValueDomainRelationship = null,
        TechnicalMessage = null,
        UserMessage = null,
        NoMap = null,
        IsGenerated = false,
        IsIdentical = null,
        ElementTypeComparisonKey = null,
        BoundValueSetComparisonKey = null,
    };
}

[CgSQLiteTable(tableName: "CollatedTypeComparisons")]
//[CgSQLiteIndex(nameof(SourceElementKey), nameof(TargetElementKey))]
//[CgSQLiteIndex(nameof(SourceFhirPackageKey), nameof(TargetFhirPackageKey))]
public partial class DbCollatedTypeComparison : DbPackageComparisonContent, IDbPackageComparisonContent
{
    [CgSQLiteForeignKey(referenceTable: "ElementComparisons", referenceColumn: nameof(DbElementComparison.Key))]
    public required int ElementComparisonKey { get; set; }


    [CgSQLiteForeignKey(referenceTable: "Elements", referenceColumn: nameof(DbElementType.Key))]
    public required int SourceElementKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "CollatedTypes", referenceColumn: nameof(DbElementType.Key))]
    public required int SourceCollatedTypeKey { get; set; }


    [CgSQLiteForeignKey(referenceTable: "Elements", referenceColumn: nameof(DbElementType.Key))]
    public required int? TargetElementKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "CollatedTypes", referenceColumn: nameof(DbElementType.Key))]
    public required int? TargetCollatedTypeKey { get; set; }

    public required bool? NoMap { get; set; }
    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? ConceptDomainRelationship { get; set; }
    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? ValueDomainRelationship { get; set; }

    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? TargetProfileRelationship { get; set; }
    public required string TargetProfileMessage { get; set; }
    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? TypeProfileRelationship { get; set; }
    public required string TypeProfileMessage { get; set; }

    [CgSQLiteIgnore]
    public int SourceContentKey => SourceElementKey;
    [CgSQLiteIgnore]
    public int? TargetContentKey => TargetElementKey;
}

[CgSQLiteTable(tableName: "ElementTypeComparisons")]
[CgSQLiteIndex(nameof(CollatedTypeComparisonKey), nameof(SourceTypeKey))]
[CgSQLiteIndex(nameof(SourceElementKey), nameof(TargetElementKey))]
[CgSQLiteIndex(nameof(SourceElementKey), nameof(TargetElementKey), nameof(SourceTypeKey))]
//[CgSQLiteIndex(nameof(SourceFhirPackageKey), nameof(TargetFhirPackageKey), nameof(SourceElementTypeLiteral), nameof(TargetElementTypeLiteral))]
public partial class DbElementTypeComparison : DbPackageComparisonContent, IDbPackageComparisonContent
{
    [CgSQLiteForeignKey(referenceTable: "ElementComparisons", referenceColumn: nameof(DbElementComparison.Key))]
    public required int ElementComparisonKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "CollatedTypeComparisons", referenceColumn: nameof(DbCollatedTypeComparison.Key))]
    public required int CollatedTypeComparisonKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Elements", referenceColumn: nameof(DbElementType.Key))]
    public required int SourceElementKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ElementTypes", referenceColumn: nameof(DbElementType.Key))]
    public required int SourceTypeKey { get; set; }

    public required string SourceTypeLiteral { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Elements", referenceColumn: nameof(DbElementType.Key))]
    public required int? TargetElementKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ElementTypes", referenceColumn: nameof(DbElementType.Key))]
    public required int? TargetTypeKey { get; set; }

    public required string? TargetTypeLiteral { get; set; }

    public required bool? NoMap { get; set; }
    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? ConceptDomainRelationship { get; set; }
    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? ValueDomainRelationship { get; set; }

    [CgSQLiteIgnore]
    public int SourceContentKey => SourceTypeKey;
    [CgSQLiteIgnore]
    public int? TargetContentKey => TargetTypeKey;
}


[CgSQLiteTable(tableName: "UnresolvedElementComparisons")]
public partial class DbUnresolvedElementComparison : DbPackageComparisonContent
{
    [CgSQLiteForeignKey(referenceTable: "StructureComparisons", referenceColumn: nameof(DbStructureComparison.Key))]
    public required int? StructureComparisonKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "UnresolvedStructureComparisons", referenceColumn: nameof(DbUnresolvedStructureComparison.Key))]
    public required int? UnresolvedStructureComparisonKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key))]
    public required int? SourceStructureKey { get; set; }

    public required string SourceStructureUrl { get; set; }
    public required string SourceElementToken { get; set; }
    public required bool SourceElementExists { get; set; } = false;

    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key))]
    public required int? TargetStructureKey { get; set; }

    public required string? TargetStructureUrl { get; set; }
    public required string? TargetElementToken { get; set; }
    public required bool TargetElementExists { get; set; } = false;

    public required bool? NoMap { get; set; }
}

public class DbComparisonCache<T>
        where T : IDbPackageComparisonContent
{
    private readonly Dictionary<int, T> _byKey = [];
    private readonly Dictionary<(int sourceKey, int? targetKey), T> _byPair = [];

    private readonly Dictionary<int, T> _toAdd = [];
    private readonly Dictionary<int, T> _toUpdate = [];
    private readonly Dictionary<int, T> _toDelete = [];

    public bool TryGet(int key, [NotNullWhen(true)] out T? value) => _byKey.TryGetValue(key, out value);
    public bool TryGet((int sourceKey, int? targetKey) pair, [NotNullWhen(true)] out T? value) => _byPair.TryGetValue(pair, out value);

    public T? Get(int key) => _byKey.TryGetValue(key, out T? value) ? value : default(T);
    public T? Get(int sourceKey, int? targetKey) => _byPair.TryGetValue((sourceKey, targetKey), out T? value) ? value : default(T);

    public IEnumerable<T> ForSource(int sourceKey)
    {
        return _byPair
            .Where(kvp => kvp.Key.sourceKey == sourceKey)
            .Select(kvp => kvp.Value);
    }

    public IEnumerable<T> ForTarget(int targetKey)
    {
        return _byPair
            .Where(kvp => kvp.Key.targetKey == targetKey)
            .Select(kvp => kvp.Value);
    }

    public IEnumerable<T> Values => _byKey.Values;

    public IEnumerable<T> ComparisonsToAdd => _toAdd.Values;
    public IEnumerable<T> ComparisonsToUpdate => _toUpdate.Values;
    public IEnumerable<T> ComparisonsToDelete => _toDelete.Values;

    public void Clear()
    {
        _byKey.Clear();
        _byPair.Clear();
        _toAdd.Clear();
        _toUpdate.Clear();
        _toDelete.Clear();
    }

    public void CacheAdd(T item)
    {
        _byKey[item.Key] = item;
        _byPair[(item.SourceContentKey, item.TargetContentKey)] = item;
        _toAdd[item.Key] = item;
    }

    public void CacheUpdate(T item)
    {
        _byKey[item.Key] = item;
        _byPair[(item.SourceContentKey, item.TargetContentKey)] = item;
        _toUpdate[item.Key] = item;
    }

    public void CacheDelete(T item)
    {
        _byKey[item.Key] = item;
        _byPair[(item.SourceContentKey, item.TargetContentKey)] = item;
        _toDelete[item.Key] = item;
    }

    public void Changed(T item)
    {
        if (!_byKey.ContainsKey(item.Key))
        {
            _byKey[item.Key] = item;
        }

        if (!_byPair.ContainsKey((item.SourceContentKey, item.TargetContentKey)))
        {
            _byPair[(item.SourceContentKey, item.TargetContentKey)] = item;
        }

        if (_toDelete.ContainsKey(item.Key))
        {
            _toDelete.Remove(item.Key);
        }

        if (_toAdd.ContainsKey(item.Key))
        {
            _toAdd[item.Key] = item;
            return;
        }

        _toUpdate[item.Key] = item;
    }

    public int Count => _byKey.Count;
}


//public partial class DbXverSourceFml : DbPackageComparisonContent
//{
//    [CgSQLiteForeignKey(referenceTable: "StructureComparisons", referenceColumn: nameof(DbStructureComparison.Key))]
//    public required int? StructureComparisonKey { get; set; }

//    [CgSQLiteForeignKey(referenceTable: "UnresolvedStructureComparisons", referenceColumn: nameof(DbUnresolvedStructureComparison.Key))]
//    public required int? UnresolvedStructureComparisonKey { get; set; }

//    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key))]
//    public required int? SourceStructureKey { get; set; }

//    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key))]
//    public required int? TargetStructureKey { get; set; }

//}
