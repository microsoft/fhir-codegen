using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Utils;
using Microsoft.Health.Fhir.Comparison.Models;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;

namespace Microsoft.Health.Fhir.Comparison.Extensions;

public static class DbContentExtensions
{
    public static bool ElementHasNonPrimitiveTypes(
        this IDbConnection dbConnection,
        DbElement element,
        string? elementTypeTableName = null,
        string? structureTypeTableName = null)
    {
        elementTypeTableName ??= DbElementType.DefaultTableName;
        structureTypeTableName ??= DbStructureDefinition.DefaultTableName;

        IDbCommand command = dbConnection.CreateCommand();
        command.CommandText = $$$"""
            SELECT COUNT(Key)
            FROM {{{elementTypeTableName}}}
            WHERE ElementKey = {{{element.Key}}}
            AND TypeName NOT IN (
                SELECT DISTINCT Id
                FROM {{{structureTypeTableName}}}
                where FhirPackageKey = {{{element.FhirPackageKey}}}
                AND ArtifactClass = 'PrimitiveType'
            )
            """;

        int count = Convert.ToInt32(command.ExecuteScalar());

        return count > 0;
    }

    public static string? ProcessCoreTextForLinks(
        this string? input,
        string fhirVersionLiteral)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        // Process special markdown links first, then HTML links, then core links.
        string? output = FhirSanitizationUtils.ProcessFhirSpecialMdLinks(input, fhirVersionLiteral);
        output = FhirSanitizationUtils.ProcessFhirHtmlLinks(output, fhirVersionLiteral);
        output = FhirSanitizationUtils.ProcessFhirCoreLinks(output, fhirVersionLiteral);

        return output;
    }

    public static Narrative? ProcessCoreTextForLinks(this Narrative? input, string fhirVersionLiteral)
    {
        if ((input == null) || string.IsNullOrEmpty(input.Div))
        {
            return input;
        }

        // Process special markdown links first, then HTML links, then core links.
        string? output = FhirSanitizationUtils.ProcessFhirSpecialMdLinks(input.Div, fhirVersionLiteral);
        output = FhirSanitizationUtils.ProcessFhirHtmlLinks(output, fhirVersionLiteral);
        output = FhirSanitizationUtils.ProcessFhirCoreLinks(output, fhirVersionLiteral);

        return new Narrative
        {
            Status = input.Status,
            Div = output,
        };
    }

    public static int ConceptCountWithLiteralMatches(
        this IDbConnection dbConnection,
        int vsKeyA,
        int vsKeyB,
        string? conceptTableName = null)
    {
        conceptTableName ??= DbValueSetConcept.DefaultTableName;

        IDbCommand command = dbConnection.CreateCommand();
        command.CommandText = $$$"""
            SELECT COUNT(C1.Key)
            FROM {{{conceptTableName}}} C1
            JOIN {{{conceptTableName}}} C2 ON C1.Code = C2.Code
            WHERE C1.ValueSetKey = {{{vsKeyA}}}
            AND C2.ValueSetKey = {{{vsKeyB}}}
            """;

        return Convert.ToInt32(command.ExecuteScalar());
    }

    public static int ElementCountThatLookIdentical(
        this IDbConnection dbConnection,
        int structureKeyA,
        int structureKeyB,
        string? elementTableName = null)
    {
        elementTableName ??= DbElement.DefaultTableName;

        IDbCommand command = dbConnection.CreateCommand();
        command.CommandText = $$$"""
            SELECT COUNT(E1.Key)
            FROM {{{elementTableName}}} E1
            JOIN {{{elementTableName}}} E2 ON E1.Id = E2.Id
            WHERE E1.StructureKey = {{{structureKeyA}}}
            AND E2.StructureKey = {{{structureKeyB}}}
            AND (
                E1.MinCardinality != E2.MinCardinality
                OR E1.MaxCardinality != E2.MaxCardinality
                OR E1.FullCollatedTypeLiteral != E2.FullCollatedTypeLiteral
                OR (
                    (E1.ValueSetBindingStrength is NULL AND E2.ValueSetBindingStrength is NOT NULL)
                    or (E1.ValueSetBindingStrength is NOT NULL AND E2.ValueSetBindingStrength is NULL)
                    or (E1.ValueSetBindingStrength != E2.ValueSetBindingStrength)
                    )
                OR E1.IsModifier != E2.IsModifier
                )
            """;

        return Convert.ToInt32(command.ExecuteScalar());
    }

    public static int UpdateCollatedTypeStructureKeys(
        this IDbConnection dbConnection,
        int FhirPackageKey,
        string? collatedTypeTableName = null,
        string? structureTableName = null)
    {
        collatedTypeTableName ??= DbCollatedType.DefaultTableName;
        structureTableName ??= DbStructureDefinition.DefaultTableName;

        IDbCommand command = dbConnection.CreateCommand();
        command.CommandText = $$$"""
            UPDATE {{{collatedTypeTableName}}}
            SET
                TypeStructureKey = S.Key
            FROM {{{structureTableName}}} S
            WHERE {{{collatedTypeTableName}}}.TypeName = S.Id
            AND {{{collatedTypeTableName}}}.FhirPackageKey = S.FhirPackageKey
            AND {{{collatedTypeTableName}}}.FhirPackageKey = {{{FhirPackageKey}}}
            """;

        return command.ExecuteNonQuery();
    }

    public static int UpdateElementTypeStructureKeys(
        this IDbConnection dbConnection,
        int FhirPackageKey,
        string? elementTypeTableName = null,
        string? structureTableName = null)
    {
        elementTypeTableName ??= DbElementType.DefaultTableName;
        structureTableName ??= DbStructureDefinition.DefaultTableName;

        IDbCommand command = dbConnection.CreateCommand();
        command.CommandText = $$$"""
            UPDATE {{{elementTypeTableName}}}
            SET
                TypeStructureKey = S.Key
            FROM {{{structureTableName}}} S
            WHERE {{{elementTypeTableName}}}.TypeName = S.Id
            AND {{{elementTypeTableName}}}.FhirPackageKey = S.FhirPackageKey
            AND {{{elementTypeTableName}}}.FhirPackageKey = {{{FhirPackageKey}}}
            """;

        return command.ExecuteNonQuery();
    }

    public static int UpdateElementBaseKeys(
        this IDbConnection dbConnection,
        int FhirPackageKey,
        string? dbTableName = null)
    {
        dbTableName ??= DbElement.DefaultTableName;

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
