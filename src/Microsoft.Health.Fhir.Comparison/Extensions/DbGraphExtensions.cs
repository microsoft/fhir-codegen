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

public static class DbGraphExtensions
{
    private record struct ValueSetMappingRec(
        int ValueSetKey,
        string Name,
        int FhirPackageKey,
        string FhirVersion,
        string UnversionedUrl,
        string Version,
        string ValueSetId,
        string? Title,
        int Level);

    public static List<(string unversionedUrl, string version)> GetMappedValueSetUrls(
        this IDbConnection dbConnection,
        int sourceFhirPackageKey,
        string sourceUrl,
        int? targetFhirPackageKey)
    {
        IDbCommand command = dbConnection.CreateCommand();
        command.CommandText = $$$"""
                        
            WITH RECURSIVE vs_mappings AS (
            -- Base case: Start with the input vs
            SELECT
              v.Key as vs_key,
              v.Name as name,
              v.FhirPackageKey as fhir_package_key,
              fp.ShortName as fhir_version,
              v.UnversionedUrl as unversioned_url,
              v.Version as vs_version,
              v.Id as vs_id,
              v.Title as title,
              0 as level,
              CAST(v.FhirPackageKey AS TEXT) as visited_packages
            FROM ValueSets v
            JOIN FhirPackages fp ON v.FhirPackageKey = fp.Key
            WHERE v.FhirPackageKey = $SourcePackageKey
            AND (v.UnversionedUrl = $SourceUrl OR v.VersionedUrl = $SourceUrl)

            UNION

            -- Forward mappings: Find value sets that current value sets map TO
            SELECT DISTINCT
              tv.Key as vs_key,
              tv.Name as name,
              tv.FhirPackageKey as fhir_package_key,
              tfp.ShortName as fhir_version,
              tv.UnversionedUrl as unversioned_url,
              tv.Version as vs_version,
              tv.Id as vs_id,
              tv.Title as title,
              vm.level + 1 as level,
              vm.visited_packages || ',' || CAST(tv.FhirPackageKey AS TEXT) as visited_packages
            FROM vs_mappings vm
            JOIN ValueSetComparisons vc ON vc.SourceValueSetKey = vm.vs_key
            JOIN ValueSets tv ON tv.Key = vc.TargetValueSetKey
            JOIN FhirPackages tfp ON tv.FhirPackageKey = tfp.Key
            WHERE
              vm.level < 6 -- Safety limit to prevent infinite recursion
              AND tv.Key IS NOT NULL -- Ensure target value set exists
              -- Prevent cycles: don't revisit FhirPackageKeys we've already seen
              AND instr(',' || vm.visited_packages || ',', ',' || CAST(tv.FhirPackageKey AS TEXT) || ',') = 0

            UNION

            -- Reverse mappings: Find value sets that map TO current structures
            SELECT DISTINCT
              sv.Key as vs_key,
              sv.Name as name,
              sv.FhirPackageKey as fhir_package_key,
              sfp.ShortName as fhir_version,
              sv.UnversionedUrl as unversioned_url,
              sv.Version as vs_version,
              sv.Id as vs_id,
              sv.Title as title,
              vm.level + 1 as level,
              vm.visited_packages || ',' || CAST(sv.FhirPackageKey AS TEXT) as visited_packages
            FROM vs_mappings vm
            JOIN ValueSetComparisons vc ON vc.TargetValueSetKey = vm.vs_key
            JOIN ValueSets sv ON sv.Key = vc.SourceValueSetKey
            JOIN FhirPackages sfp ON sv.FhirPackageKey = sfp.Key
            WHERE
              vm.level < 6 -- Safety limit to prevent infinite recursion
              AND sv.Key IS NOT NULL -- Ensure source value setexists
              -- Prevent cycles: don't revisit FhirPackageKeys we've already seen
              AND instr(',' || vm.visited_packages || ',', ',' || CAST(sv.FhirPackageKey AS TEXT) || ',') = 0
            )

            SELECT DISTINCT
            vs_key,
            name,
            fhir_package_key,
            fhir_version,
            unversioned_url,
            vs_version,
            vs_id,
            title,
            level
            FROM vs_mappings
            ORDER BY fhir_version ASC, name ASC, level ASC;
            """;

        {
            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = "$SourcePackageKey";
            param.Value = sourceFhirPackageKey;
            command.Parameters.Add(param);
        }

        {
            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = "$SourceUrl";
            param.Value = sourceUrl;
            command.Parameters.Add(param);
        }

        List<(string unversionedUrl, string version)> results = [];

        using (IDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                ValueSetMappingRec vmr = new()
                {
                    ValueSetKey = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    FhirPackageKey = reader.GetInt32(2),
                    FhirVersion = reader.GetString(3),
                    UnversionedUrl = reader.GetString(4),
                    Version = reader.GetString(5),
                    ValueSetId = reader.GetString(6),
                    Title = reader.IsDBNull(7) ? null : reader.GetString(7),
                    Level = reader.GetInt32(8)
                };

                if ((targetFhirPackageKey != null) &&
                    (vmr.FhirPackageKey != targetFhirPackageKey))
                {
                    continue; // Skip mappings not in the target package
                }

                results.Add((vmr.UnversionedUrl, vmr.Version));
            }
        }

        return results;
    }

    private record struct StructureMappingRec(
        int StructureKey,
        string Name,
        int FhirPackageKey,
        string FhirVersion,
        string UnversionedUrl,
        string StructureId,
        string Title,
        int Level);

    public static List<string> GetMappedStructureUrls(
        this IDbConnection dbConnection,
        int sourceFhirPackageKey,
        string sourceFhirStrcutreId,
        int? targetFhirPackageKey)
    {

        IDbCommand command = dbConnection.CreateCommand();
        command.CommandText = $$$"""
            WITH RECURSIVE structure_mappings AS (
            -- Base case: Start with the input structure
            SELECT
              s.Key as structure_key,
              s.Name as name,
              s.FhirPackageKey as fhir_package_key,
              fp.ShortName as fhir_version,
              s.UnversionedUrl as unversioned_url,
              s.Id as structure_id,
              s.Title as title,
              0 as level,
              CAST(s.FhirPackageKey AS TEXT) as visited_packages
            FROM Structures s
            JOIN FhirPackages fp ON s.FhirPackageKey = fp.Key
            WHERE s.FhirPackageKey = $SourcePackageKey
            AND s.Id = $SourceStructureId

            UNION

            -- Forward mappings: Find structures that current structures map TO
            SELECT DISTINCT
              ts.Key as structure_key,
              ts.Name as name,
              ts.FhirPackageKey as fhir_package_key,
              tfp.ShortName as fhir_version,
              ts.UnversionedUrl as unversioned_url,
              ts.Id as structure_id,
              ts.Title as title,
              sm.level + 1 as level,
              sm.visited_packages || ',' || CAST(ts.FhirPackageKey AS TEXT) as visited_packages
            FROM structure_mappings sm
            JOIN StructureComparisons sc ON sc.SourceStructureKey = sm.structure_key
            JOIN Structures ts ON ts.Key = sc.TargetStructureKey
            JOIN FhirPackages tfp ON ts.FhirPackageKey = tfp.Key
            WHERE
              sm.level < 6 -- Safety limit to prevent infinite recursion
              AND ts.Key IS NOT NULL -- Ensure target structure exists
              -- Prevent cycles: don't revisit FhirPackageKeys we've already seen
              AND instr(',' || sm.visited_packages || ',', ',' || CAST(ts.FhirPackageKey AS TEXT) || ',') = 0

            UNION

            -- Reverse mappings: Find structures that map TO current structures
            SELECT DISTINCT
              ss.Key as structure_key,
              ss.Name as name,
              ss.FhirPackageKey as fhir_package_key,
              sfp.ShortName as fhir_version,
              ss.UnversionedUrl as unversioned_url,
              ss.Id as structure_id,
              ss.Title as title,
              sm.level + 1 as level,
              sm.visited_packages || ',' || CAST(ss.FhirPackageKey AS TEXT) as visited_packages
            FROM structure_mappings sm
            JOIN StructureComparisons sc ON sc.TargetStructureKey = sm.structure_key
            JOIN Structures ss ON ss.Key = sc.SourceStructureKey
            JOIN FhirPackages sfp ON ss.FhirPackageKey = sfp.Key
            WHERE
              sm.level < 6 -- Safety limit to prevent infinite recursion
              AND ss.Key IS NOT NULL -- Ensure source structure exists
              -- Prevent cycles: don't revisit FhirPackageKeys we've already seen
              AND instr(',' || sm.visited_packages || ',', ',' || CAST(ss.FhirPackageKey AS TEXT) || ',') = 0
            )

            SELECT DISTINCT
            structure_key,
            name,
            fhir_package_key,
            fhir_version,
            unversioned_url,
            structure_id,
            title,
            level
            FROM structure_mappings
            ORDER BY fhir_version ASC, name ASC, level ASC;
            """;

        {
            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = "$SourcePackageKey";
            param.Value = sourceFhirPackageKey;
            command.Parameters.Add(param);
        }

        {
            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = "$SourceStructureId";
            param.Value = sourceFhirStrcutreId;
            command.Parameters.Add(param);
        }

        List<string> results = [];

        using (IDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                StructureMappingRec smr = new()
                {
                    StructureKey = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    FhirPackageKey = reader.GetInt32(2),
                    FhirVersion = reader.GetString(3),
                    UnversionedUrl = reader.GetString(4),
                    StructureId = reader.GetString(5),
                    Title = reader.GetString(6),
                    Level = reader.GetInt32(7)
                };

                if ((targetFhirPackageKey != null) &&
                    (smr.FhirPackageKey != targetFhirPackageKey))
                {
                    continue; // Skip mappings not in the target package
                }

                results.Add(smr.UnversionedUrl);
            }
        }

        return results;
    }

    public static List<DbStructureDefinition> GetMappedStructureDefinitions(
        this IDbConnection dbConnection,
        int sourceFhirPackageKey,
        string sourceFhirStrcutreId,
        int? targetFhirPackageKey)
    {

        IDbCommand command = dbConnection.CreateCommand();
        command.CommandText = $$$"""
            WITH RECURSIVE structure_mappings AS (
            -- Base case: Start with the input structure
            SELECT
              s.Key as structure_key,
              s.Name as name,
              s.FhirPackageKey as fhir_package_key,
              fp.ShortName as fhir_version,
              s.UnversionedUrl as unversioned_url,
              s.Id as structure_id,
              s.Title as title,
              0 as level,
              CAST(s.FhirPackageKey AS TEXT) as visited_packages
            FROM Structures s
            JOIN FhirPackages fp ON s.FhirPackageKey = fp.Key
            WHERE s.FhirPackageKey = $SourcePackageKey
            AND s.Id = $SourceStructureId

            UNION

            -- Forward mappings: Find structures that current structures map TO
            SELECT DISTINCT
              ts.Key as structure_key,
              ts.Name as name,
              ts.FhirPackageKey as fhir_package_key,
              tfp.ShortName as fhir_version,
              ts.UnversionedUrl as unversioned_url,
              ts.Id as structure_id,
              ts.Title as title,
              sm.level + 1 as level,
              sm.visited_packages || ',' || CAST(ts.FhirPackageKey AS TEXT) as visited_packages
            FROM structure_mappings sm
            JOIN StructureComparisons sc ON sc.SourceStructureKey = sm.structure_key
            JOIN Structures ts ON ts.Key = sc.TargetStructureKey
            JOIN FhirPackages tfp ON ts.FhirPackageKey = tfp.Key
            WHERE
              sm.level < 6 -- Safety limit to prevent infinite recursion
              AND ts.Key IS NOT NULL -- Ensure target structure exists
              -- Prevent cycles: don't revisit FhirPackageKeys we've already seen
              AND instr(',' || sm.visited_packages || ',', ',' || CAST(ts.FhirPackageKey AS TEXT) || ',') = 0

            UNION

            -- Reverse mappings: Find structures that map TO current structures
            SELECT DISTINCT
              ss.Key as structure_key,
              ss.Name as name,
              ss.FhirPackageKey as fhir_package_key,
              sfp.ShortName as fhir_version,
              ss.UnversionedUrl as unversioned_url,
              ss.Id as structure_id,
              ss.Title as title,
              sm.level + 1 as level,
              sm.visited_packages || ',' || CAST(ss.FhirPackageKey AS TEXT) as visited_packages
            FROM structure_mappings sm
            JOIN StructureComparisons sc ON sc.TargetStructureKey = sm.structure_key
            JOIN Structures ss ON ss.Key = sc.SourceStructureKey
            JOIN FhirPackages sfp ON ss.FhirPackageKey = sfp.Key
            WHERE
              sm.level < 6 -- Safety limit to prevent infinite recursion
              AND ss.Key IS NOT NULL -- Ensure source structure exists
              -- Prevent cycles: don't revisit FhirPackageKeys we've already seen
              AND instr(',' || sm.visited_packages || ',', ',' || CAST(ss.FhirPackageKey AS TEXT) || ',') = 0
            )

            SELECT DISTINCT
            structure_key,
            name,
            fhir_package_key,
            fhir_version,
            unversioned_url,
            structure_id,
            title,
            level
            FROM structure_mappings
            ORDER BY fhir_version ASC, name ASC, level ASC;
            """;

        {
            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = "$SourcePackageKey";
            param.Value = sourceFhirPackageKey;
            command.Parameters.Add(param);
        }

        {
            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = "$SourceStructureId";
            param.Value = sourceFhirStrcutreId;
            command.Parameters.Add(param);
        }

        List<StructureMappingRec> mappings = [];

        using (IDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                mappings.Add(new StructureMappingRec(
                    StructureKey: reader.GetInt32(0),
                    Name: reader.GetString(1),
                    FhirPackageKey: reader.GetInt32(2),
                    FhirVersion: reader.GetString(3),
                    UnversionedUrl: reader.GetString(4),
                    StructureId: reader.GetString(5),
                    Title: reader.GetString(6),
                    Level: reader.GetInt32(7)
                ));
            }
        }

        List<DbStructureDefinition> results = [];
        foreach (StructureMappingRec mapping in mappings)
        {
            if ((targetFhirPackageKey != null) &&
                (mapping.FhirPackageKey != targetFhirPackageKey))
            {
                continue; // Skip mappings not in the target package
            }

            DbStructureDefinition? sd = DbStructureDefinition.SelectSingle(dbConnection, Key: mapping.StructureKey);
            if (sd != null)
            {
                results.Add(sd);
            }
        }

        return results;
    }
}
