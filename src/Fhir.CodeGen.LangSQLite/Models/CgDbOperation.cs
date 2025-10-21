using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fhir.CodeGen.SQLiteGenerator;
using Hl7.Fhir.Utility;

namespace Fhir.CodeGen.LangSQLite.Models;

[CgSQLiteTable(tableName: "Operations")]
[CgSQLiteIndex(nameof(PackageKey), nameof(UnversionedUrl))]
[CgSQLiteIndex(nameof(PackageKey), nameof(Name))]
[CgSQLiteIndex(nameof(PackageKey), nameof(Code))]
[CgSQLiteIndex(nameof(PackageKey), nameof(Id))]
public partial class CgDbOperation : CgDbMetadataResourceBase
{
    public required Hl7.Fhir.Model.OperationDefinition.OperationKind Kind { get; set; }
    public required bool? AffectsState { get; set; }
    public required string? Synchronicity { get; set; }         // TODO: ValueSet is in the Extensions pack...
    public required string? Code { get; set; }
    public required string? Comment { get; set; }
    public required string? BaseCanonical { get; set; }
    public required string? ResourceTypes { get; set; }
    [CgSQLiteIgnore]
    public List<Hl7.Fhir.Model.Code<Hl7.Fhir.Model.VersionIndependentResourceTypesAll>> ResourceTypeList
    {
        set
        {
            ResourceTypes = (value == null || value.Count == 0) ? null : string.Join(',', value);
        }
        get
        {
            if (string.IsNullOrEmpty(ResourceTypes))
            {
                return [];
            }
            return ResourceTypes
                .Split(',')
                .Select(v => v.Trim())
                .Where(v => !string.IsNullOrEmpty(v))
                .Select(v => EnumUtility.ParseLiteral<Hl7.Fhir.Model.VersionIndependentResourceTypesAll>(v))
                .Select(v => new Hl7.Fhir.Model.Code<Hl7.Fhir.Model.VersionIndependentResourceTypesAll>(v))
                .ToList();
        }
    }

    public required string? AdditionalResourceTypes { get; set; }
    [CgSQLiteIgnore]
    public List<string> AdditionalResourceTypeList
    {
        set
        {
            AdditionalResourceTypes = (value == null || value.Count == 0) ? null : string.Join(',', value);
        }
        get
        {
            if (string.IsNullOrEmpty(AdditionalResourceTypes))
            {
                return [];
            }
            return AdditionalResourceTypes
                .Split(',')
                .Select(v => v.Trim())
                .Where(v => !string.IsNullOrEmpty(v))
                .ToList();
        }
    }

    public required bool InvokeOnSystem { get; set; }
    public required bool InvokeOnType { get; set; }
    public required bool InvokeOnInstance { get; set; }

    public required string? InputProfileCanonical { get; set; }
    public required string? OutputProfileCanonical { get; set; }

    public required int ParameterCount { get; set; }

    public required List<Hl7.Fhir.Model.OperationDefinition.OverloadComponent>? Overloads { get; set; }
}
