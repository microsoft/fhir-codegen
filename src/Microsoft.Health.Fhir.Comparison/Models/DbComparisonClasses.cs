using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Health.Fhir.CodeGen.Utils;
using Microsoft.Health.Fhir.CodeGenCommon.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Utils;
using fhir_codegen.SQLiteGenerator;
using System.Diagnostics.CodeAnalysis;


namespace Microsoft.Health.Fhir.Comparison.Models;


[CgSQLiteTable(tableName: "PackageComparisonPairs")]
public partial class DbFhirPackageComparisonPair
{
    [CgSQLiteKey]
    public int Key { get; set; } = -1;
    public int InverseComparisonKey { get; set; } = -1;

    [CgSQLiteForeignKey(referenceTable: "FhirPackages", referenceColumn: nameof(DbFhirPackage.Key))]
    public required int SourcePackageKey { get; set; }

    public required string SourcePackageShortName { get; set; }

    [CgSQLiteForeignKey(referenceTable: "FhirPackages", referenceColumn: nameof(DbFhirPackage.Key))]
    public required int TargetPackageKey { get; set; }

    public required string TargetPackageShortName { get; set; }

    public DateTime ProccessedAt { get; set; } = DateTime.UtcNow;
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
public abstract class DbPackageComparisonContent
{
    [CgSQLiteKey]
    public int Key { get; set; } = -1;

    public int? InverseComparisonKey { get; set; } = -1;

    [CgSQLiteForeignKey(referenceTable: "PackageComparisonPairs", referenceColumn: nameof(DbFhirPackageComparisonPair.Key))]
    public required int PackageComparisonKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "FhirPackages", referenceColumn: nameof(DbFhirPackage.Key))]
    public required int SourceFhirPackageKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "FhirPackages", referenceColumn: nameof(DbFhirPackage.Key))]
    public required int TargetFhirPackageKey { get; set; }

    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? Relationship { get; set; }
    public required string? Message { get; set; }
    public required bool? IsGenerated { get; set; }
    public string? LastReviewedBy { get; set; } = null;
    public DateTime? LastReviewedOn { get; set; } = null;
}


[CgSQLiteTable(tableName: "ValueSetComparisons")]
[CgSQLiteIndex(nameof(PackageComparisonKey), nameof(SourceFhirPackageKey), nameof(TargetFhirPackageKey), nameof(SourceValueSetKey))]
[CgSQLiteIndex(nameof(PackageComparisonKey), nameof(SourceFhirPackageKey), nameof(SourceValueSetKey), nameof(TargetFhirPackageKey), nameof(TargetValueSetKey))]
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

    [CgSQLiteIgnore]
    public int SourceContentKey => SourceValueSetKey;
    [CgSQLiteIgnore]
    public int? TargetContentKey => TargetValueSetKey;

}

[CgSQLiteTable(tableName: "ConceptComparisons")]
[CgSQLiteIndex(nameof(ValueSetComparisonKey))]
[CgSQLiteIndex(nameof(ValueSetComparisonKey), nameof(SourceValueSetKey), nameof(SourceConceptKey), nameof(TargetFhirPackageKey))]
[CgSQLiteIndex(nameof(PackageComparisonKey), nameof(SourceFhirPackageKey), nameof(SourceValueSetKey), nameof(SourceConceptKey), nameof(TargetFhirPackageKey), nameof(TargetValueSetKey), nameof(TargetConceptKey))]
public partial class DbValueSetConceptComparison : DbPackageComparisonContent, IDbPackageComparisonContent
{
    [CgSQLiteForeignKey(referenceTable: "ValueSetComparisons", referenceColumn: nameof(DbValueSetComparison.Key))]
    public required int ValueSetComparisonKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public required int SourceValueSetKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public required int? TargetValueSetKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Concepts", referenceColumn: nameof(DbValueSetConcept.Key))]
    public required int SourceConceptKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Concepts", referenceColumn: nameof(DbValueSetConcept.Key))]
    public required int? TargetConceptKey { get; set; }

    public required bool? NoMap { get; set; }

    [CgSQLiteIgnore]
    public int SourceContentKey => SourceConceptKey;
    [CgSQLiteIgnore]
    public int? TargetContentKey => TargetConceptKey;

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


    [CgSQLiteForeignKey(referenceTable: "Concepts", referenceColumn: nameof(DbValueSetConcept.Key))]
    public required int? SourceConceptKey { get; set; }
    public required bool SourceConceptExists { get; set; } = false;
    public required string SourceSystem { get; set; }
    public required string SourceCode { get; set; }
    public required string? SourceDisplay { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Concepts", referenceColumn: nameof(DbValueSetConcept.Key))]
    public required int? TargetConceptKey { get; set; }
    public required bool? TargetConceptExists { get; set; }
    public required string? TargetSystem { get; set; }
    public required string? TargetCode { get; set; }
    public required string? TargetDisplay { get; set; }

    public required bool? NoMap { get; set; }
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

    [CgSQLiteIgnore]
    public int SourceContentKey => SourceElementKey;
    [CgSQLiteIgnore]
    public int? TargetContentKey => TargetElementKey;
}

[CgSQLiteTable(tableName: "ElementTypeComparisons")]
[CgSQLiteIndex(nameof(SourceElementKey), nameof(TargetElementKey))]
public partial class DbElementTypeComparison : DbPackageComparisonContent, IDbPackageComparisonContent
{
    [CgSQLiteForeignKey(referenceTable: "StructureComparisons", referenceColumn: nameof(DbStructureComparison.Key))]
    public required int StructureComparisonKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key))]
    public required int SourceStructureKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key))]
    public required int? TargetStructureKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Elements", referenceColumn: nameof(DbElementType.Key))]
    public required int SourceElementKey { get; set; }
    public required string SourceElementTypeLiteral { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Elements", referenceColumn: nameof(DbElementType.Key))]
    public required int? TargetElementKey { get; set; }
    public required string? TargetElementTypeLiteral { get; set; }

    public required bool? NoMap { get; set; }
    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? ConceptDomainRelationship { get; set; }
    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? ValueDomainRelationship { get; set; }

    [CgSQLiteIgnore]
    public int SourceContentKey => SourceElementKey;
    [CgSQLiteIgnore]
    public int? TargetContentKey => TargetElementKey;
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

    public bool TryGet(int key, [NotNullWhen(true)] out T? value) => _byKey.TryGetValue(key, out value);
    public bool TryGet((int sourceKey, int? targetKey) pair, [NotNullWhen(true)] out T? value) => _byPair.TryGetValue(pair, out value);

    public T? Get(int key) => _byKey.TryGetValue(key, out var value) ? value : default(T);
    public T? Get(int sourceKey, int? targetKey) => _byPair.TryGetValue((sourceKey, targetKey), out var value) ? value : default(T);

    public IEnumerable<T> ForSource(int sourceKey)
    {
        return _byPair
            .Where(kvp => kvp.Key.sourceKey == sourceKey)
            .Select(kvp => kvp.Value);
    }

    public IEnumerable<T> ComparisonsToAdd => _toAdd.Values;
    public IEnumerable<T> ComparisonsToUpdate => _toUpdate.Values;

    public void Clear()
    {
        _byKey.Clear();
        _byPair.Clear();
        _toAdd.Clear();
        _toUpdate.Clear();
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

    public void Changed(T item)
    {
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
