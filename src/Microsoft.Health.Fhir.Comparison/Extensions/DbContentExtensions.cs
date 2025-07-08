using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Utils;
using Microsoft.Health.Fhir.Comparison.Models;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;

namespace Microsoft.Health.Fhir.Comparison.Extensions;

public static class DbContentExtensions
{
    public static int UpdateCollatedTypeStructureKeys(
        this IDbConnection dbConnection,
        int FhirPackageKey,
        string? dbTableName = null,
        string? structureDbTableName = null)
    {
        dbTableName ??= "CollatedTypes";
        structureDbTableName ??= "Structures";

        IDbCommand command = dbConnection.CreateCommand();
        command.CommandText = $$$"""
            UPDATE {{{dbTableName}}}
            SET
                TypeStructureKey = S.Key
            FROM {{{structureDbTableName}}} S
            WHERE {{{dbTableName}}}.TypeName = S.Id
            AND {{{dbTableName}}}.FhirPackageKey = S.FhirPackageKey
            AND {{{dbTableName}}}.FhirPackageKey = {{{FhirPackageKey}}}
            """;

        return command.ExecuteNonQuery();
    }

    public static int UpdateElementTypeStructureKeys(
        this IDbConnection dbConnection,
        int FhirPackageKey,
        string? dbTableName = null,
        string? structureDbTableName = null)
    {
        dbTableName ??= "ElementTypes";
        structureDbTableName ??= "Structures";

        IDbCommand command = dbConnection.CreateCommand();
        command.CommandText = $$$"""
            UPDATE {{{dbTableName}}}
            SET
                TypeStructureKey = S.Key
            FROM {{{structureDbTableName}}} S
            WHERE {{{dbTableName}}}.TypeName = S.Id
            AND {{{dbTableName}}}.FhirPackageKey = S.FhirPackageKey
            AND {{{dbTableName}}}.FhirPackageKey = {{{FhirPackageKey}}}
            """;

        return command.ExecuteNonQuery();
    }

    public static int UpdateElementBaseKeys(
        this IDbConnection dbConnection,
        int FhirPackageKey,
        string? dbTableName = null)
    {
        dbTableName ??= "Elements";

        IDbCommand command = dbConnection.CreateCommand();
        command.CommandText = $$$"""
            UPDATE {{{dbTableName}}}
            SET
                BaseElementKey = E.Key,
                BaseStructureKey = E.StructureKey
            FROM {{{dbTableName}}} E
            WHERE {{{dbTableName}}}.Key = E.Key
            AND {{{dbTableName}}}.BasePath is not NULL
            AND {{{dbTableName}}}.BaseElementKey is NULL
            AND {{{dbTableName}}}.FhirPackageKey = {{{FhirPackageKey}}}
            """;

        return command.ExecuteNonQuery();
    }

    private static readonly string[] _quantityTypes = [
        "Quantity",
        "Age",
        "Count",
        "Distance",
        "Duration",
        "Money",
        "MoneyQuantity",
        "SimpleQuantity",
        ];

    public static bool IsQuantityType(this DbElementType et)
    {
        if (et == null || string.IsNullOrEmpty(et.TypeName))
        {
            return false;
        }

        if (_quantityTypes.Contains(et.TypeName, StringComparer.OrdinalIgnoreCase))
        {
            return true;
        }

        return false;
    }

    public static string GetNormalizedName(this DbElementType et)
    {
        if (string.IsNullOrEmpty(et.TypeName))
        {
            return string.Empty;
        }

        if ((et.TypeName == "Quantity") &&
            (et.TypeProfile?.StartsWith("http://hl7.org/", StringComparison.Ordinal) == true))
        {
            return et.TypeProfile.Split('/')[^1];
        }

        return et.TypeName;
    }

    public static bool IsEquivalent(this DbElementType a, DbElementType b)
    {
        if (a == null || b == null)
        {
            return false;
        }

        if ((a.TypeName == b.TypeName) &&
            equalsOrBothNull(a.TypeProfile, b.TypeProfile) &&
            equalsOrBothNull(a.TargetProfile, b.TargetProfile))
        {
            return true;
        }

        return false;

        bool equalsOrBothNull(string? a, string? b)
        {
            return (a == b) || ((a == null) && (b == null));
        }
    }

    public static bool HaveEquivalentProfiles(this DbElementType a, DbElementType b)
    {
        if (a == null || b == null)
        {
            return false;
        }

        if (equalsOrBothNull(a.TypeProfile, b.TypeProfile) &&
            equalsOrBothNull(a.TargetProfile, b.TargetProfile))
        {
            return true;
        }

        return false;

        bool equalsOrBothNull(string? a, string? b)
        {
            return (a == b) || ((a == null) && (b == null));
        }
    }
}
