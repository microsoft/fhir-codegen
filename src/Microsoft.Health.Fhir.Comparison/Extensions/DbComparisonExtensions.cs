using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Utils;
using Microsoft.Health.Fhir.Comparison.Models;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;

namespace Microsoft.Health.Fhir.Comparison.Extensions;

public static class DbComparisonExtensions
{
    public static bool HasPrimitiveType(
        this DbElement element,
        IDbConnection dbConnection,
        string? dbTableName = null,
        string? structureDbTableName = null)
    {
        dbTableName ??= "ElementTypes";
        structureDbTableName ??= "Structures";

        IDbCommand command = dbConnection.CreateCommand();
        command.CommandText = $"SELECT count(Key) FROM {dbTableName}" +
            $" WHERE ElementKey = $ElementKey" +
            $" AND TypeStructureKey in (select Key from {structureDbTableName} where ArtifactClass = 'PrimitiveType')";

        {
            IDbDataParameter elementKeyParam = command.CreateParameter();
            elementKeyParam.ParameterName = "$ElementKey";
            elementKeyParam.Value = element.Key;
            command.Parameters.Add(elementKeyParam);
        }

        int count = 0;
        using (IDataReader reader = command.ExecuteReader())
        {
            if (reader.Read())
            {
                count = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
            }
        }

        return count > 0;
    }

    public static CMR? TypeLiteralRelationship(
        this IDbConnection dbConnection,
        int SourceFhirPackageKey,
        int TargetFhirPackageKey,
        string SourceElementTypeLiteral,
        string TargetElementTypeLiteral,
        string? dbTableName = null)
    {
        dbTableName ??= "ElementTypeComparisons";

        IDbCommand command = dbConnection.CreateCommand();
        command.CommandText = $"SELECT distinct Relationship" +
            $" FROM {dbTableName}" +
            $" WHERE SourceFhirPackageKey = $SourceFhirPackageKey" +
            $" AND TargetFhirPackageKey = $TargetFhirPackageKey" +
            $" AND SourceElementTypeLiteral = $SourceElementTypeLiteral" +
            $" AND TargetElementTypeLiteral = $TargetElementTypeLiteral";

        {
            IDbDataParameter SourceFhirPackageKeyParam = command.CreateParameter();
            SourceFhirPackageKeyParam.ParameterName = "$SourceFhirPackageKey";
            SourceFhirPackageKeyParam.Value = SourceFhirPackageKey;
            command.Parameters.Add(SourceFhirPackageKeyParam);
        }
        {
            IDbDataParameter TargetFhirPackageKeyParam = command.CreateParameter();
            TargetFhirPackageKeyParam.ParameterName = "$TargetFhirPackageKey";
            TargetFhirPackageKeyParam.Value = TargetFhirPackageKey;
            command.Parameters.Add(TargetFhirPackageKeyParam);
        }
        {
            IDbDataParameter SourceElementTypeLiteralParam = command.CreateParameter();
            SourceElementTypeLiteralParam.ParameterName = "$SourceElementTypeLiteral";
            SourceElementTypeLiteralParam.Value = SourceElementTypeLiteral;
            command.Parameters.Add(SourceElementTypeLiteralParam);
        }
        {
            IDbDataParameter TargetElementTypeLiteralParam = command.CreateParameter();
            TargetElementTypeLiteralParam.ParameterName = "$TargetElementTypeLiteral";
            TargetElementTypeLiteralParam.Value = TargetElementTypeLiteral;
            command.Parameters.Add(TargetElementTypeLiteralParam);
        }

        CMR? relationship = null;

        using (IDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                relationship = applyRelationship(relationship, reader.IsDBNull(0) ? null : Enum.Parse<Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship>(reader.GetString(0)));
            }
        }

        return relationship;

        CMR applyRelationship(CMR? existing, CMR? change) => existing switch
        {
            CMR.Equivalent => change ?? CMR.Equivalent,
            CMR.RelatedTo => (change == CMR.NotRelatedTo) ? CMR.NotRelatedTo : CMR.RelatedTo,
            CMR.SourceIsNarrowerThanTarget => (change == CMR.SourceIsNarrowerThanTarget || change == CMR.Equivalent)
                ? CMR.SourceIsNarrowerThanTarget : CMR.RelatedTo,
            CMR.SourceIsBroaderThanTarget => (change == CMR.SourceIsBroaderThanTarget || change == CMR.Equivalent)
                ? CMR.SourceIsBroaderThanTarget : CMR.RelatedTo,
            CMR.NotRelatedTo => change ?? CMR.NotRelatedTo,
            _ => change ?? existing ?? CMR.NotRelatedTo,
        };
    }

    //public static string GetCompositeName<T>(this DbCanonicalComparison<T> comparison)
    //    where T : DbCanonicalResource
    //{
    //    if (comparison.Target == null)
    //    {
    //        return 
    //            $"{comparison.SourceFhirPackage.ShortName}-" +
    //            $"{FhirSanitizationUtils.SanitizeForProperty(comparison.SourceName).ToPascalCase()}-" +
    //            $"{comparison.TargetFhirPackage.ShortName}";
    //    }

    //    return 
    //        $"{comparison.SourceFhirPackage.ShortName}-" +
    //        $"{FhirSanitizationUtils.SanitizeForProperty(comparison.SourceName).ToPascalCase()}-" +
    //        $"{comparison.TargetFhirPackage.ShortName}-" +
    //        $"{FhirSanitizationUtils.SanitizeForProperty(comparison.Target.Name).ToPascalCase()}";
    //}

    //public static string GetCompositeName(this DbFhirPackageComparisonPair packagePair, DbCanonicalResource source, DbCanonicalResource? target)
    //{
    //    if (target == null)
    //    {
    //        return
    //            $"{packagePair.SourcePackage.ShortName}-" +
    //            $"{FhirSanitizationUtils.SanitizeForProperty(source.Name).ToPascalCase()}-" +
    //            $"{packagePair.TargetPackage.ShortName}";
    //    }

    //    return
    //        $"{packagePair.SourcePackage.ShortName}-" +
    //        $"{FhirSanitizationUtils.SanitizeForProperty(source.Name).ToPascalCase()}-" +
    //        $"{packagePair.TargetPackage.ShortName}-" +
    //        $"{FhirSanitizationUtils.SanitizeForProperty(target.Name).ToPascalCase()}";
    //}
}
