using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fhir.CodeGen.SQLiteGenerator;
using Hl7.Fhir.Utility;

namespace Fhir.CodeGen.LangSQLite.Models;

[CgSQLiteTable(tableName: "SearchParameters")]
[CgSQLiteIndex(nameof(PackageKey), nameof(UnversionedUrl))]
[CgSQLiteIndex(nameof(PackageKey), nameof(Name))]
[CgSQLiteIndex(nameof(PackageKey), nameof(Id))]
public partial class CgDbSearchParameter : CgDbMetadataResourceBase
{
    public required string? DerivedFromCanonical { get; set; }

    public required string Code { get; set; }

    public required string? AliasCodes { get; set; }
    [CgSQLiteIgnore]
    public List<string> AliasCodeList
    {
        set
        {
            AliasCodes = (value == null || value.Count == 0) ? null : string.Join(',', value);
        }
        get
        {
            if (string.IsNullOrEmpty(AliasCodes))
            {
                return [];
            }
            return AliasCodes.Split(',')
                .Select(v => v.Trim())
                .Where(v => !string.IsNullOrEmpty(v))
                .ToList();
        }
    }

    public required string BaseResources { get; set; }
    [CgSQLiteIgnore]
    public List<Hl7.Fhir.Model.Code<Hl7.Fhir.Model.VersionIndependentResourceTypesAll>> BaseResourceList
    {
        set
        {
            BaseResources = (value == null || value.Count == 0) ? string.Empty : string.Join(',', value);
        }
        get
        {
            if (string.IsNullOrEmpty(BaseResources))
            {
                return [];
            }
            return BaseResources
                .Split(',')
                .Select(v => v.Trim())
                .Where(v => !string.IsNullOrEmpty(v))
                .Select(v => EnumUtility.ParseLiteral<Hl7.Fhir.Model.VersionIndependentResourceTypesAll>(v))
                .Select(v => new Hl7.Fhir.Model.Code<Hl7.Fhir.Model.VersionIndependentResourceTypesAll>(v))
                .ToList();
        }
    }

    //public required Hl7.Fhir.Model.SearchParamType? SearchType { get; set; }
    public required string? SearchType { get; set; }
    public required string? Expression { get; set; }
    public required Hl7.Fhir.Model.SearchParameter.SearchProcessingModeType? ProcessingMode { get; set; }
    public required string? SearchParameterConstraint { get; set; }
    public required string? ReferenceTargets { get; set; }
    [CgSQLiteIgnore]
    public List<string> ReferenceTargetList
    {
        set
        {
            ReferenceTargets = (value == null || value.Count == 0) ? null : string.Join(',', value);
        }
        get
        {
            if (string.IsNullOrEmpty(ReferenceTargets))
            {
                return [];
            }
            return ReferenceTargets
                .Split(',')
                .Select(v => v.Trim())
                .Where(v => !string.IsNullOrEmpty(v))
                .ToList();
        }
    }

    public required bool? MultipleOr { get; set; }
    public required bool? MultipleAnd { get; set; }
    public required string? Comparators { get; set; }
    [CgSQLiteIgnore]
    public List<Hl7.Fhir.Model.Code<Hl7.Fhir.Model.SearchComparator>> ComparatorList
    {
        set
        {
            Comparators = (value == null || value.Count == 0)
                ? null
                : string.Join(',', value.Select(v => EnumUtility.GetLiteral(v.Value)));
        }
        get
        {
            if (string.IsNullOrEmpty(Comparators))
            {
                return [];
            }
            return Comparators
                .Split(',')
                .Select(v => v.Trim())
                .Where(v => !string.IsNullOrEmpty(v))
                .Select(v => EnumUtility.ParseLiteral<Hl7.Fhir.Model.SearchComparator>(v))
                .Select(v => new Hl7.Fhir.Model.Code<Hl7.Fhir.Model.SearchComparator>(v))
                .ToList();
        }
    }

    public required string? Modifiers { get; set; }
    [CgSQLiteIgnore]
    public List<Hl7.Fhir.Model.Code<Hl7.Fhir.Model.SearchModifierCode>> ModifierList
    {
        set
        {
            Modifiers = (value == null || value.Count == 0)
                ? null
                : string.Join(',', value.Select(v => EnumUtility.GetLiteral(v.Value)));
        }
        get
        {
            if (string.IsNullOrEmpty(Modifiers))
            {
                return [];
            }
            return Modifiers
                .Split(',')
                .Select(v => v.Trim())
                .Where(v => !string.IsNullOrEmpty(v))
                .Select(v => EnumUtility.ParseLiteral<Hl7.Fhir.Model.SearchModifierCode>(v))
                .Select(v => new Hl7.Fhir.Model.Code<Hl7.Fhir.Model.SearchModifierCode>(v))
                .ToList();
        }
    }

    public required string? ChainableSearchParameters { get; set; }
    [CgSQLiteIgnore]
    public List<string> ChainableSearchParameterList
    {
        set
        {
            ChainableSearchParameters = (value == null || value.Count == 0) ? null : string.Join(',', value);
        }
        get
        {
            if (string.IsNullOrEmpty(ChainableSearchParameters))
            {
                return [];
            }
            return ChainableSearchParameters
                .Split(',')
                .Select(v => v.Trim())
                .Where(v => !string.IsNullOrEmpty(v))
                .ToList();
        }
    }

    public required int ComponentCount { get; set; }
}
