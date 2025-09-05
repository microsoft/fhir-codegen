using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fhir_codegen.SQLiteGenerator;
using Hl7.Fhir.Utility;

namespace fhir_codegen.LangSQLite.Models;

[CgSQLiteTable(tableName: "OperationParameters")]
[CgSQLiteIndex(nameof(PackageKey), nameof(OperationKey), nameof(OperationParameterOrder))]
[CgSQLiteIndex(nameof(OperationKey), nameof(OperationParameterOrder))]
public partial class CgDbOperationParameter : CgDbPackageContentBase
{
    [CgSQLiteForeignKey(referenceTable: "Operations", referenceColumn: nameof(CgDbOperation.Key))]
    public required int OperationKey { get; set; }

    public required string Name { get; set; }
    public required Hl7.Fhir.Model.OperationParameterUse Use { get; set; }
    public required string? Scopes { get; set; }
    [CgSQLiteIgnore]
    public List<Hl7.Fhir.Model.Code<Hl7.Fhir.Model.OperationDefinition.OperationParameterScope>> ScopeList
    {
        set
        {
            Scopes = (value == null || value.Count == 0) ? null : string.Join(',', value);
        }
        get
        {
            if (string.IsNullOrEmpty(Scopes))
            {
                return [];
            }
            return Scopes
                .Split(',')
                .Select(v => v.Trim())
                .Where(v => !string.IsNullOrEmpty(v))
                .Select(v => EnumUtility.ParseLiteral<Hl7.Fhir.Model.OperationDefinition.OperationParameterScope>(v))
                .Select(v => new Hl7.Fhir.Model.Code<Hl7.Fhir.Model.OperationDefinition.OperationParameterScope>(v))
                .ToList();
        }
    }

    public required int Min { get; set; }
    public required string Max { get; set; }
    public required string? Documentation { get; set; }
    public required string? Type { get; set; }
    public required string? AllowedTypes { get; set; }
    [CgSQLiteIgnore]
    public List<Hl7.Fhir.Model.Code<Hl7.Fhir.Model.FHIRAllTypes>> AllowedTypeList
    {
        set
        {
            AllowedTypes = (value == null || value.Count == 0) ? null : string.Join(',', value);
        }
        get
        {
            if (string.IsNullOrEmpty(AllowedTypes))
            {
                return [];
            }
            return AllowedTypes
                .Split(',')
                .Select(v => v.Trim())
                .Where(v => !string.IsNullOrEmpty(v))
                .Select(v => EnumUtility.ParseLiteral<Hl7.Fhir.Model.FHIRAllTypes>(v))
                .Select(v => new Hl7.Fhir.Model.Code<Hl7.Fhir.Model.FHIRAllTypes>(v))
                .ToList();
        }
    }

    public required string? TargetProfileCanonicals { get; set; }
    [CgSQLiteIgnore]
    public List<string> TargetProfileCanonicalList
    {
        set
        {
            TargetProfileCanonicals = (value == null || value.Count == 0) ? null : string.Join(',', value);
        }
        get
        {
            if (string.IsNullOrEmpty(TargetProfileCanonicals))
            {
                return [];
            }
            return TargetProfileCanonicals
                .Split(',')
                .Select(v => v.Trim())
                .Where(v => !string.IsNullOrEmpty(v))
                .ToList();
        }
    }

    public required Hl7.Fhir.Model.SearchParamType? SearchType { get; set; }

    public required Hl7.Fhir.Model.BindingStrength? BindingStrength { get; set; }
    public required string? BindingValueSetCanonical { get; set; }

    public required List<Hl7.Fhir.Model.OperationDefinition.ReferencedFromComponent>? ReferencedFrom { get; set; }


    [CgSQLiteForeignKey(referenceTable: "OperationParameters", referenceColumn: nameof(CgDbOperationParameter.Key))]
    public required int? ParentParameterKey { get; set; }

    public required int ChildParameterCount { get; set; }

    public required int OperationParameterOrder { get; set; }
    public required int ParameterPartOrder { get; set; }
}
