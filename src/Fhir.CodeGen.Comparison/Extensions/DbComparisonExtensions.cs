using System.Data;
using Fhir.CodeGen.Comparison.Models;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;

namespace Fhir.CodeGen.Comparison.Extensions;

public static class DbComparisonExtensions
{
    public static bool HasPrimitiveType(
        this DbElement element,
        IDbConnection dbConnection,
        string? dbTableName = null,
        string? structureDbTableName = null)
    {
        dbTableName ??= DbElementType.DefaultTableName;
        structureDbTableName ??= DbStructureDefinition.DefaultTableName;

        IDbCommand command = dbConnection.CreateCommand();
        command.CommandText = $"SELECT count({nameof(DbElementType.Key)}) FROM {dbTableName}" +
            $" WHERE {nameof(DbElementType.ElementKey)} = $ElementKey" +
            $" AND {nameof(DbElementType.TypeStructureKey)} in" +
            $" (select {nameof(DbStructureDefinition.Key)} from {structureDbTableName} where {nameof(DbStructureDefinition.ArtifactClass)} = 'PrimitiveType')";

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
        throw new NotImplementedException();
#if false
        dbTableName ??= DbElementTypeComparison.DefaultTableName;

        IDbCommand command = dbConnection.CreateCommand();
        command.CommandText = $"SELECT distinct {nameof(DbElementTypeComparison.Relationship)}" +
            $" FROM {dbTableName}" +
            $" WHERE {nameof(DbElementTypeComparison.SourceFhirPackageKey)} = $SourceFhirPackageKey" +
            $" AND {nameof(DbElementTypeComparison.TargetFhirPackageKey)} = $TargetFhirPackageKey" +
            $" AND {nameof(DbElementTypeComparison.SourceTypeLiteral)} = $SourceElementTypeLiteral" +
            $" AND {nameof(DbElementTypeComparison.TargetTypeLiteral)} = $TargetElementTypeLiteral";

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
#endif
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

    //public static string GetCompositeName(this FhirPackageComparisonPair packagePair, DbCanonicalResource source, DbCanonicalResource? target)
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
