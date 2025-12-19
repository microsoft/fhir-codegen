using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Fhir.CodeGen.Comparison.Models;
using Fhir.CodeGen.Comparison.XVer;
using Microsoft.Extensions.Logging;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;

namespace Fhir.CodeGen.Comparison.CompareTool;

public class FhirMappingComparerVs
{
    private readonly IDbConnection _db;

    private ILoggerFactory? _loggerFactory;
    private ILogger _logger;


    public FhirMappingComparerVs(
        IDbConnection db,
        ILoggerFactory? loggerFactory)
    {
        _db = db;
        _loggerFactory = loggerFactory;
        _logger = _loggerFactory?.CreateLogger<FhirMappingComparerVs>()
            ?? LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<FhirMappingComparerVs>(); ;
    }

    public void Compare()
    {
        List<DbFhirPackage> packages = DbFhirPackage.SelectList(
            _db,
            orderByProperties: [nameof(DbFhirPackage.PackageVersion)]);

        // iterate over packages to use as source
        for (int i = 0; i < packages.Count; i++)
        {
            DbFhirPackage sourcePackage = packages[i];

            // iterate upward over packages to use as target
            for (int j = i + 1; j < packages.Count; j++)
            {
                DbFhirPackage targetPackage = packages[j];
                doComparisons(sourcePackage, targetPackage);
            }

            // iterate downward over packages to use as target
            for (int j = i - 1; j >= 0; j--)
            {
                DbFhirPackage targetPackage = packages[j];
                doComparisons(sourcePackage, targetPackage);
            }
        }
    }

    private void doComparisons(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage)
    {
        DbArtifactMapCache<DbValueSetMappingRecord> vsMapCache = new();
        DbRecordCache<DbValueSetConceptMappingRecord> vsConceptMapCache = new();

        // get the value set mappings between this source and target
        List<DbValueSetMappingRecord> mappings = DbValueSetMappingRecord.SelectList(
            _db,
            SourceFhirPackageKey: sourcePackage.Key,
            TargetFhirPackageKey: targetPackage.Key);

        // iterate across the list and perform the comparisons
        foreach (DbValueSetMappingRecord mapRec in mappings)
        {
            // resolve the source (required) and target (optional) value sets
            DbValueSet sourceVs = DbValueSet.SelectSingle(
                _db,
                Key: mapRec.SourceValueSetKey)
                ?? throw new Exception($"Failed to resolve source value set: {mapRec.SourceFhirPackageKey} for mapping: {mapRec.Key} ({mapRec.IdLong})");

            DbValueSet? targetVs = DbValueSet.SelectSingle(
                _db,
                Key: mapRec.TargetValueSetKey);

            // update based on no-map data
            if (targetVs is null)
            {
                mapRec.ComputedRelationship = null;
                mapRec.HasContentUnmapped = true;
                mapRec.HasContentIdentical = null;
                mapRec.HasContentEquivalent = null;
                mapRec.HasContentBroader = null;
                mapRec.HasContentNarrower = null;

                mapRec.IsIdentical = null;
                mapRec.IsEquivalent = null;
                mapRec.IsBroaderThanTarget = null;
                mapRec.IsNarrowerThanTarget = null;
                mapRec.IsRenamed = null;

                mapRec.Comments ??= $"The {sourcePackage.ShortName} Value Set" +
                    $" {sourceVs.Id} ({sourceVs.UnversionedUrl}) has no representation in" +
                    $" FHIR {targetPackage.ShortName}";

                vsMapCache.CacheUpdate(mapRec);
                continue;
            }

            // resolve the concepts for the source and target value sets
            List<DbValueSetConcept> sourceConcepts = DbValueSetConcept.SelectList(
                _db,
                ValueSetKey: sourceVs.Key);
            List<DbValueSetConcept> targetConcepts = DbValueSetConcept.SelectList(
                _db,
                ValueSetKey: targetVs.Key);

            // get the concept mappings for this value set mapping
            List<DbValueSetConceptMappingRecord> conceptMappings = DbValueSetConceptMappingRecord.SelectList(
                _db,
                ValueSetMappingKey: mapRec.Key);

            // build lookups
            ILookup<string, DbValueSetConcept> targetConceptsByFhirKey = targetConcepts.ToLookup(c => c.FhirKey);
            ILookup<string, DbValueSetConcept> targetConceptsByCode = targetConcepts.ToLookup(c => c.Code);
            ILookup<int, DbValueSetConcept> targetConceptsByKey = targetConcepts.ToLookup(c => c.Key);
            ILookup<int, DbValueSetConceptMappingRecord> conceptMappingLookup = conceptMappings.ToLookup(cm => cm.SourceValueSetConceptKey);

            // iterate across source concepts to compare to target
            foreach (DbValueSetConcept sourceConcept in sourceConcepts)
            {
                // check for any existing mappings (0 or more)
                List<DbValueSetConceptMappingRecord> currentMappings = conceptMappingLookup[sourceConcept.Key].ToList();

                if (currentMappings.Count == 0)
                {
                    // no existing mapping - see if we can find a match in the target
                    List<DbValueSetConcept> matches = targetConceptsByFhirKey[sourceConcept.FhirKey].ToList();
                    if (matches.Count == 0)
                    {
                        matches = targetConceptsByCode[sourceConcept.Code].ToList();
                    }

                    // nothing found means no mapping
                    if (matches.Count == 0)
                    {
                        vsConceptMapCache.CacheAdd(
                            createMappingRecord(
                                sourcePackage,
                                targetPackage,
                                mapRec,
                                sourceVs,
                                targetVs,
                                sourceConcept,
                                targetConcept: null,
                                relationship: null));

                        continue;
                    }

                    if (matches.Count == 1)
                    {
                        vsConceptMapCache.CacheAdd(
                            createMappingRecord(
                                sourcePackage,
                                targetPackage,
                                mapRec,
                                sourceVs,
                                targetVs,
                                sourceConcept,
                                targetConcept: matches[0],
                                relationship: CMR.Equivalent));

                        continue;
                    }

                    // multiple matches found - create multiple mappings
                    foreach (DbValueSetConcept match in matches)
                    {
                        vsConceptMapCache.CacheAdd(
                            createMappingRecord(
                                sourcePackage,
                                targetPackage,
                                mapRec,
                                sourceVs,
                                targetVs,
                                sourceConcept,
                                targetConcept: match,
                                relationship: CMR.SourceIsBroaderThanTarget));
                    }

                    continue;
                }

                // we have mappings, update them with current info
                foreach (DbValueSetConceptMappingRecord currentMapping in currentMappings)
                {
                    // if there is a target concept, resolve it
                    DbValueSetConcept? targetConcept = currentMapping.TargetValueSetConceptKey is null
                        ? null
                        : targetConceptsByKey[currentMapping.TargetValueSetConceptKey.Value].FirstOrDefault();

                    string comments;

                    if (targetConcept is null)
                    {
                        if (targetVs is null)
                        {
                            comments = $"The concept `{sourceConcept.System}#{sourceConcept.Code}` ({sourceConcept.Display}) from" +
                                $" Value Set `{sourceVs.UnversionedUrl}` in" +
                                $" FHIR {sourcePackage.ShortName} has no representation in" +
                                $" FHIR {targetPackage.ShortName}";
                        }
                        else
                        {
                            comments = $"The concept `{sourceConcept.System}#{sourceConcept.Code}` ({sourceConcept.Display}) from" +
                                $" Value Set `{sourceVs.UnversionedUrl}` from" +
                                $" FHIR {sourcePackage.ShortName} has no representation in" +
                                $" Value Set `{targetVs.UnversionedUrl}` from" +
                                $" FHIR {targetPackage.ShortName}";
                        }

                        currentMapping.ExplicitNoMap = false;
                        currentMapping.IsIdentical = false;
                        currentMapping.IsEquivalent = false;
                        currentMapping.CodesAreIdentical = false;
                        currentMapping.Comments = comments;
                        currentMapping.TechnicalNotes = "Auto-generated";
                        currentMapping.Relationship = null;

                        vsConceptMapCache.CacheUpdate(currentMapping);
                        continue;
                    }

                    comments = $"The concept `{sourceConcept.System}#{sourceConcept.Code}` ({sourceConcept.Display}) from" +
                        $" Value Set `{sourceVs.UnversionedUrl}` from" +
                        $" FHIR {sourcePackage.ShortName} maps to" +
                        $" `{targetConcept.System}#{targetConcept.Code}`" +
                        $" Value Set `{targetVs!.UnversionedUrl}` from" +
                        $" FHIR {targetPackage.ShortName}";

                    currentMapping.ExplicitNoMap = false;
                    currentMapping.IsIdentical = sourceConcept.FhirKey == targetConcept.FhirKey;
                    currentMapping.IsEquivalent = sourceConcept.Code == targetConcept.Code;
                    currentMapping.CodesAreIdentical = sourceConcept.Code == targetConcept.Code;
                    currentMapping.Comments = comments;
                    currentMapping.TechnicalNotes = "Auto-generated";
                    currentMapping.Relationship ??= CMR.Equivalent;

                    vsConceptMapCache.CacheUpdate(currentMapping);
                }
            }
        }

        // apply changes
        if (vsMapCache.ToAddCount > 0)
        {
            _logger.LogInformation($"Adding {vsMapCache.ToAddCount} value set mappings from {sourcePackage.ShortName} to {targetPackage.ShortName}");
            vsMapCache.ToAdd.Insert(_db, ignoreDuplicates: true, insertPrimaryKey: true);
        }

        if (vsMapCache.ToUpdateCount > 0)
        {
            _logger.LogInformation($"Updating {vsMapCache.ToUpdateCount} value set mappings from {sourcePackage.ShortName} to {targetPackage.ShortName}");
            vsMapCache.ToUpdate.Update(_db);
        }

        if (vsConceptMapCache.ToAddCount > 0)
        {
            _logger.LogInformation($"Adding {vsConceptMapCache.ToAddCount} value set concept mappings from {sourcePackage.ShortName} to {targetPackage.ShortName}");
            vsConceptMapCache.ToAdd.Insert(_db, ignoreDuplicates: true, insertPrimaryKey: true);
        }

        if (vsConceptMapCache.ToUpdateCount > 0)
        {
            _logger.LogInformation($"Updating {vsConceptMapCache.ToUpdateCount} value set concept mappings from {sourcePackage.ShortName} to {targetPackage.ShortName}");
            vsConceptMapCache.ToUpdate.Update(_db);
        }
    }

    private DbValueSetConceptMappingRecord createMappingRecord(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        DbValueSetMappingRecord valueSetMappingRecord,
        DbValueSet sourceVs,
        DbValueSet? targetVs,
        DbValueSetConcept sourceConcept,
        DbValueSetConcept? targetConcept,
        CMR? relationship = null)
    {
        string comments;

        if (targetConcept is null)
        {
            if (targetVs is null)
            {
                comments = $"The concept `{sourceConcept.System}#{sourceConcept.Code}` ({sourceConcept.Display}) from" +
                    $" Value Set `{sourceVs.UnversionedUrl}` in" +
                    $" FHIR {sourcePackage.ShortName} has no representation in" +
                    $" FHIR {targetPackage.ShortName}";
            }
            else
            {
                comments = $"The concept `{sourceConcept.System}#{sourceConcept.Code}` ({sourceConcept.Display}) from" +
                    $" Value Set `{sourceVs.UnversionedUrl}` from" +
                    $" FHIR {sourcePackage.ShortName} has no representation in" +
                    $" Value Set `{targetVs.UnversionedUrl}` from" +
                    $" FHIR {targetPackage.ShortName}";
            }
        }
        else
        {
            comments = $"The concept `{sourceConcept.System}#{sourceConcept.Code}` ({sourceConcept.Display}) from" +
                $" Value Set `{sourceVs.UnversionedUrl}` from" +
                $" FHIR {sourcePackage.ShortName} maps to" +
                $" `{targetConcept.System}#{targetConcept.Code}`" +
                $" Value Set `{targetVs!.UnversionedUrl}` from" +
                $" FHIR {targetPackage.ShortName}";
        }

        return new()
        {
            Key = DbValueSetConceptMappingRecord.GetIndex(),
            SourceFhirPackageKey = sourcePackage.Key,
            SourceValueSetConceptKey = sourceConcept.Key,
            TargetFhirPackageKey = targetPackage.Key,
            TargetValueSetConceptKey = targetConcept?.Key,
            ValueSetMappingKey = valueSetMappingRecord.Key,

            ExplicitNoMap = false,
            IsIdentical = (targetConcept is not null) && (sourceConcept.FhirKey == targetConcept.FhirKey),
            IsEquivalent = (targetConcept is not null) && (sourceConcept.Code == targetConcept.Code),
            CodesAreIdentical = (targetConcept is not null) && (sourceConcept.Code == targetConcept.Code),
            Comments = comments,
            TechnicalNotes = "Auto-generated",

            Relationship = relationship,
        };
    }
}
