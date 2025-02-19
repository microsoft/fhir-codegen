// <copyright file="DifferenceTracker.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using Hl7.Fhir.Model;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGen.Utils;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.FhirExtensions;
using Microsoft.Health.Fhir.CodeGenCommon.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using Microsoft.Health.Fhir.CodeGenCommon.Utils;
using Newtonsoft.Json.Linq;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;

namespace Microsoft.Health.Fhir.Comparison.CompareTool;


internal static partial class DifferenceTrackerLogMessages
{
    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to resolve ValueSet {vsUrl} in {sourcePackage}:{sourceVersion}.")]
    internal static partial void LogFailedToResolveVs(this ILogger logger, string vsUrl, string sourcePackage, string sourceVersion);


}

/// <summary>
/// Class to track differences between two FHIR packages.
/// </summary>
public class DifferenceTracker : IDisposable
{
    private const string _canonicalRootCrossVersion = "http://hl7.org/fhir/uv/xver/";
    private const string _canonicalRootHl7 = "http://hl7.org/fhir/";
    private const string _canonicalRootCi = "http://build.fhir.org/";
    private const string _canonicalRootTHO = "http://terminology.hl7.org/";

    private const string _prefixSource = "S_";
    private const string _prefixTarget = "T_";

    private const string _prefixVSExpansion = "VS_";
    private const string _prefixVSComparison = "X_VS_";

    private bool _disposedValue;

    private ILoggerFactory? _loggerFactory;
    private ILogger _logger;

    private class DcInfoRec
    {
        public required FhirReleases.FhirSequenceCodes FhirSequence;
        public required string PackageCanonical;
        public required string RLiteral;
        public required string ShortVersion;
        public required string ShortVersionUrlSegment;
        public required string PackageVersion;
    }

    //private class DcCompInfoRec
    //{
    //    public required string _sourceToTargetWithR = $"{_sourceRLiteral}to{_targetRLiteral}";
    //    public required string _sourceToTargetWithRLen = _sourceToTargetWithR.Length;
    //    public required string _sourceToTargetNoR = $"{_sourceRLiteral[1..]}to{_targetRLiteral[1..]}";
    //    public required string _sourceToTargetNoRLen = _sourceToTargetNoR.Length;
    //}

    private (DefinitionCollection dc, DcInfoRec info)[] _definitions;

    private bool _isCoreComparison;
    private bool _isVersionComparison;

    //private IDbConnection? _dbConnection = null;
    private string _dbPath;
    private string _dbName;

    private DiffDbContext _db;

    public DifferenceTracker(
        DefinitionCollection[] definitions,
        string dbPath,
        ILoggerFactory? loggerFactory = null)
    {
        _dbPath = dbPath;
        if (!Directory.Exists(_dbPath))
        {
            Directory.CreateDirectory(_dbPath);
        }

        _definitions = definitions.Select(dc => (dc, new DcInfoRec()
        {
            FhirSequence = dc.FhirSequence,
            PackageCanonical = dc.MainPackageCanonical,
            RLiteral = dc.FhirSequence.ToRLiteral(),
            ShortVersion = dc.FhirSequence.ToShortVersion(),
            ShortVersionUrlSegment = "/" + dc.FhirSequence.ToShortVersion() + "/",
            PackageVersion = dc.FhirVersionLiteral,
        })).ToArray();

        _isCoreComparison = definitions.All(dc => FhirPackageUtils.PackageIsFhirCore(dc.MainPackageId));
        _isVersionComparison = definitions.Select(dc => dc.MainPackageId).Distinct().Count() == 1;

        if (definitions.Length > 2)
        {
            _dbName = "fhir-comparison.db";
        }
        else
        {
            if (_isVersionComparison)
            {
                _dbName = string.Join('_', definitions.Select(dc => $"V{dc.MainPackageVersion.Replace('.', '_')}")) + ".db";
            }
            else if (_isCoreComparison)
            {
                _dbName = string.Join('_', definitions.Select(dc => $"{dc.FhirSequence.ToRLiteral()}")) + ".db";
            }
            else
            {
                _dbName = string.Join('_', definitions.Select(dc => $"{dc.MainPackageId.ToPascalCase()}-V{dc.MainPackageVersion.Replace('.', '_')}")) + ".db";
            }
        }

        _loggerFactory = loggerFactory;
        _logger = loggerFactory?.CreateLogger<DifferenceTracker>() ?? definitions.First().Logger;

        _db = new(Path.Combine(_dbPath, _dbName));
    }

    /// <summary>
    /// Initializes the database connection and sets up the necessary tables and metadata.
    /// </summary>
    /// <param name="createdNew">Outputs a boolean indicating whether a new database was created.</param>
    /// <exception cref="Exception">Thrown when the package information does not match or the database connection is not initialized.</exception>
    public void InitDb(
        bool ensureDeleted,
        HashSet<string> _exclusionSet,
        HashSet<string> _escapeValveCodes)
    {
        if (ensureDeleted)
        {
            _db.Database.EnsureDeleted();
        }

        _db.Database.EnsureCreated();

        //string connectionString = new SqliteConnectionStringBuilder()
        //{
        //    DataSource = Path.Combine(_dbPath, _dbName),
        //    Mode = SqliteOpenMode.ReadWriteCreate,
        //}.ToString();

        //_dbConnection = new SqliteConnection(connectionString);

        //_dbConnection.Open();

        // add data about our packages
        foreach ((DefinitionCollection dc, DcInfoRec info) in _definitions)
        {
            if (_db.Packages.Any(pm => (pm.PackageId == dc.MainPackageId) && (pm.PackageVersion == dc.MainPackageVersion)))
            {
                continue;
            }

            _db.Add(new PackageMetadata()
            {
                Name = dc.Name,
                PackageId = dc.MainPackageId,
                PackageVersion = dc.MainPackageVersion,
                CanonicalUrl = dc.MainPackageCanonical,
            });
        }
        _db.SaveChanges();

        //// make sure the relationship mapping is created
        //RelationshipLookup.CreateTable(_dbConnection);

        //// iterate over all the relationship values in the enum and add to the table if it does not exist
        //foreach (CMR value in Enum.GetValues<CMR>())
        //{
        //    if (RelationshipLookup.SelectSingle(_dbConnection, Relationship: value) != null)
        //    {
        //        continue;
        //    }

        //    RelationshipLookup.Insert(_dbConnection, new RelationshipLookup()
        //    {
        //        Relationship = value,
        //        Name = value.ToString(),
        //    });
        //}

        // load contents
        foreach ((DefinitionCollection dc, DcInfoRec info) in _definitions)
        {
            loadValueSets(dc, _exclusionSet, _escapeValveCodes);
            loadStructures(dc, _exclusionSet);
        }

        _db.SaveChanges();
    }

    private void loadValueSets(
        DefinitionCollection dc,
        HashSet<string> _exclusionSet,
        HashSet<string> _escapeValveCodes)
    {
        // get the package metadata for this definition collection
        PackageMetadata pm = _db.Packages.Single(pm => (pm.PackageId == dc.MainPackageId) && (pm.PackageVersion == dc.MainPackageVersion));

        // iterate over the value sets in the definition collection
        foreach ((string unversionedUrl, string[] versions) in dc.ValueSetVersions.OrderBy(kvp => kvp.Key))
        {
            // only use the highest version in the package
            string vsVersion = versions.OrderDescending().First();
            string versionedUrl = unversionedUrl + "|" + vsVersion;

            // check to see if this value set already exists
            if (_db.ValueSets.Any(vsm => (vsm.CanonicalUrl == unversionedUrl) && (vsm.Version == vsVersion)))
            {
                continue;
            }

            // try to expand this value set
            if (!dc.ValueSetsByVersionedUrl.TryGetValue(versionedUrl, out ValueSet? uvs))
            {
                throw new Exception($"Failed to resolve ValueSet {versionedUrl} in {dc.MainPackageId}:{dc.MainPackageVersion}.");
            }

            bool canExpand = dc.TryExpandVs(versionedUrl, out ValueSet? vs, out string? expandMessage);

            bool? hasEscapeCode = !canExpand
                ? null
                : vs?.cgHasCode(_escapeValveCodes);

            // will not further process value sets we know we will not process
            if (_exclusionSet.Contains(unversionedUrl) ||
                !canExpand ||
                (vs == null))
            {
                // still add a metadata record
                ValueSetMetadata vsmExcluded = new()
                {
                    ContainingPackage = pm,
                    CanonicalUrl = versionedUrl,
                    Name = uvs.Name,
                    Version = vsVersion,
                    Description = uvs.Description,
                    CanExpand = canExpand,
                    HasEscapeValveCode = hasEscapeCode,
                    Message = expandMessage,
                };

                _db.Add(vsmExcluded);

                continue;
            }

            // create a new metadata record
            ValueSetMetadata vsm = new()
            {
                ContainingPackage = pm,
                CanonicalUrl = versionedUrl,
                Name = uvs.Name,
                Version = vsVersion,
                Description = uvs.Description,
                CanExpand = canExpand,
                HasEscapeValveCode = hasEscapeCode,
                Message = expandMessage,
            };

            // insert and update our local copy for the id
            _db.Add(vsm);

            // iterate over all the contents of the value set
            foreach (FhirConcept fc in vs.cgGetFlatConcepts(dc))
            {
                ValueSetConcept? vsc = _db.Concepts
                    .Where(vsc => (vsc.System == fc.System) && (vsc.Code == fc.Code) && (vsc.Display == fc.Display))
                    .FirstOrDefault();

                if (vsc == null)
                {
                    vsc = new()
                    {
                        System = fc.System,
                        Code = fc.Code,
                        Display = fc.Display,
                    };

                    _db.Add(vsc);
                }

                // check for this concept already having a mapping
                if (_db.ConceptMappings.Any(vscm => vscm.VsMeta == vsm && vscm.VsConcept == vsc))
                {
                    continue;
                }

                // create a new mapping record
                ValueSetConceptMapping vscm = new()
                {
                    VsMeta = vsm,
                    VsConcept = vsc,
                };
                _db.Add(vscm);
            }

            // save changes
            _db.SaveChanges();
        }
    }

    private void loadStructures(
        DefinitionCollection dc,
        HashSet<string> _exclusionSet)
    {
        // get the package metadata for this definition collection
        PackageMetadata pm = _db.Packages.Single(pm => (pm.PackageId == dc.MainPackageId) && (pm.PackageVersion == dc.MainPackageVersion));

        // iterate over the types of structures
        foreach ((IEnumerable<StructureDefinition> structures, FhirArtifactClassEnum cgClass) in getStructures(dc))
        {
            foreach (StructureDefinition sd in structures)
            {
                // will not further process value sets we know we will not process
                if (_exclusionSet.Contains(sd.Url))
                {
                    // still add a metadata record
                    StructureDefinitionMetadata sdmExcluded = new()
                    {
                        ContainingPackage = pm,
                        CanonicalUrl = sd.Url,
                        Name = sd.Name,
                        Version = sd.Version,
                        Description = sd.Description ?? string.Empty,
                        Message = "Manually excluded",
                    };

                    _db.Add(sdmExcluded);

                    continue;
                }

                // create a new metadata record
                StructureDefinitionMetadata sdm = new()
                {
                    ContainingPackage = pm,
                    CanonicalUrl = sd.Url,
                    Name = sd.Name,
                    Version = sd.Version,
                    Description = sd.Description ?? string.Empty,
                    Message = string.Empty,
                };

                // insert and update our local copy for the id
                _db.Add(sdm);

                // iterate over all the elements of the structure
                foreach (ElementDefinition ed in sd.cgElements(skipSlices: false))
                {
                    StructureElement se = new()
                    {
                        Structure = sdm,
                        FieldOrder = ed.cgFieldOrder(),
                        Id = ed.ElementId,
                        Path = ed.Path,
                    };

                    _db.Add(se);

                    // TODO: finish structure properties
                    // TODO: add types
                }
            }
        }

        // save changes
        _db.SaveChanges();

        return;

        (IEnumerable<StructureDefinition> structures, FhirArtifactClassEnum cgClass)[] getStructures(DefinitionCollection dc) => [
            (dc.PrimitiveTypesByName.Values, FhirArtifactClassEnum.PrimitiveType),
            (dc.ComplexTypesByName.Values, FhirArtifactClassEnum.ComplexType),
            (dc.ResourcesByName.Values, FhirArtifactClassEnum.Resource),
            (dc.ExtensionsByUrl.Values, FhirArtifactClassEnum.Extension),
            (dc.ProfilesByUrl.Values, FhirArtifactClassEnum.Profile),
            (dc.LogicalModelsByUrl.Values, FhirArtifactClassEnum.LogicalModel),
            ];
    }


    public void LoadFromCrossVersionMaps(CrossVersionMapCollection cv)
    {
        loadCrossVersionValueSetMaps(cv);
    }

    private void loadCrossVersionValueSetMaps(CrossVersionMapCollection cv)
    {
        //ValueSetPairComparison.CreateTable(_dbConnection);

        //// iterate over the maps in the collection
        //foreach (ConceptMap cm in cv._dc.ConceptMapsByUrl.Values)
        //{
        //    // skip ones that are not value set comparisons
        //    if (!CrossVersionMapCollection.isValueSetConceptMap(cm))
        //    {
        //        continue;
        //    }

        //    string sourceScopeUrl = string.Empty;
        //    string targetScopeUrl = string.Empty;

        //    if (cm.SourceScope is Canonical originalSourceCanonical)
        //    {
        //        sourceScopeUrl = originalSourceCanonical.Uri + "|" + originalSourceCanonical.Version;
        //    }

        //    if (cm.SourceScope is FhirUri originalSourceUri)
        //    {
        //        string elementPath = originalSourceUri.Value.Split('#')[^1];
        //        if (_source.TryFindElementByPath(elementPath, out StructureDefinition? sd, out ElementDefinition? ed))
        //        {
        //            sourceScopeUrl = ed.Binding?.ValueSet ?? string.Empty;
        //        }
        //    }

        //    if (cm.TargetScope is Canonical originalTargetCanonical)
        //    {
        //        targetScopeUrl = originalTargetCanonical.Uri + "|" + originalTargetCanonical.Version;
        //    }

        //    if (cm.TargetScope is FhirUri originalTargetUri)
        //    {
        //        string elementPath = originalTargetUri.Value.Split('#')[^1];
        //        if (_target.TryFindElementByPath(elementPath, out StructureDefinition? sd, out ElementDefinition? ed))
        //        {
        //            targetScopeUrl = ed.Binding?.ValueSet ?? string.Empty;
        //        }
        //    }

        //    string unversionedSourceUrl = sourceScopeUrl.Contains('|') ? sourceScopeUrl.Split('|')[0] : sourceScopeUrl;
        //    string unversionedTargetUrl = targetScopeUrl.Contains('|') ? targetScopeUrl.Split('|')[0] : targetScopeUrl;

        //    if (string.IsNullOrEmpty(sourceScopeUrl) || string.IsNullOrEmpty(targetScopeUrl))
        //    {
        //        throw new Exception($"Invalid Concept Map {cm.Id} ({cm.Url}): sourceScope: {sourceScopeUrl} targetScope: {targetScopeUrl}");
        //    }

        //    // check for exclusions
        //    if (PackageComparer._exclusionSet.Contains(sourceScopeUrl))
        //    {
        //        continue;
        //    }

        //    // try to resolve the source value set
        //    if (!_source.TryGetValueSet(unversionedSourceUrl, out ValueSet? sourceVs))
        //    {
        //        _logger.LogFailedToResolveVs(unversionedSourceUrl, _source.MainPackageId, _source.MainPackageVersion);
        //        continue;
        //    }

        //    ILookup<string, FhirConcept> sourceConcepts = sourceVs.cgGetFlatConcepts(_source).ToLookup(fc => fc.Key);

        //    // try to resolve the target value set
        //    if (!_target.TryGetValueSet(unversionedTargetUrl, out ValueSet? targetVs))
        //    {
        //        _logger.LogFailedToResolveVs(unversionedTargetUrl, _target.MainPackageId, _target.MainPackageVersion);
        //        return;
        //    }

        //    ILookup<string, FhirConcept> targetConcepts = targetVs.cgGetFlatConcepts(_target).ToLookup(fc => fc.Key);

        //    CMR? vsRelationship = cm.GetExtensionValue<Code<ConceptMap.ConceptMapRelationship>>(CommonDefinitions.ExtUrlConceptMapAggregateRelationship)?.Value;

        //    string cName = getCompositeName(sourceVs.Name, targetVs.Name);

        //    ValueSetPairComparison vsComp = new()
        //    {
        //        Id = -1,
        //        SourceCanonical = sourceVs.Url,
        //        SourceName = sourceVs.Name,
        //        SourceVersion = sourceVs.Version,
        //        TargetCanonical = targetVs.Url,
        //        TargetName = targetVs.Name,
        //        TargetVersion = targetVs.Version,
        //        CompositeName = cName,
        //        TableName = _prefixVSComparison + FhirSanitizationUtils.SanitizeForProperty(cName).Replace('-', '_'),
        //        Relationship = vsRelationship,
        //        IssueCode = null,
        //        Message = $"Created from existing map: {cm.Url}",
        //        Map = cm,
        //        LastReviewedBy = null,
        //        LastReviewedOn = null,
        //    };

        //    // insert and update our local copy for the id
        //    vsComp = ValueSetPairComparison.Insert(_dbConnection, vsComp);

        //    // create a table for our comparisons
        //    ValueSetCodeComparisonRec.CreateTable(_dbConnection, vsComp.TableName);

        //    // iterate over the groups
        //    foreach (ConceptMap.GroupComponent group in cm.Group)
        //    {
        //        // iterate over all the elements
        //        foreach (ConceptMap.SourceElementComponent se in group.Element)
        //        {
        //            string sourceDisplay = se.Display;
        //            if (string.IsNullOrEmpty(sourceDisplay))
        //            {
        //                string key = group.Source + "#" + se.Code;
        //                if (sourceConcepts.Contains(key))
        //                {
        //                    sourceDisplay = sourceConcepts[key].First().Display;
        //                }
        //            }

        //            // iterate over all the targets
        //            foreach (ConceptMap.TargetElementComponent te in se.Target)
        //            {
        //                // check for properties
        //                bool? isGenerated = te.Property
        //                    .Where(p => p.Code == CommonDefinitions.ConceptMapPropertyGenerated)
        //                    .FirstOrDefault()?.Value is FhirBoolean fbIsGenerated
        //                    ? fbIsGenerated.Value
        //                    : null;

        //                bool? needsReview = te.Property
        //                    .Where(p => p.Code == CommonDefinitions.ConceptMapPropertyNeedsReview)
        //                    .FirstOrDefault()?.Value is FhirBoolean fbNeedsReview
        //                    ? fbNeedsReview.Value
        //                    : null;

        //                string targetDisplay = te.Display;
        //                if (string.IsNullOrEmpty(targetDisplay))
        //                {
        //                    string key = group.Target + "#" + te.Code;
        //                    if (targetConcepts.Contains(key))
        //                    {
        //                        targetDisplay = targetConcepts[key].First().Display;
        //                    }
        //                }

        //                // create a new comparison record
        //                ValueSetCodeComparisonRec codeComparison = new()
        //                {
        //                    Id = -1,
        //                    ValueSetPairComparisonId = vsComp.Id,
        //                    SourceSystem = group.Source,
        //                    SourceCode = se.Code,
        //                    SourceDisplay = sourceDisplay,
        //                    NoMap = false,
        //                    TargetSystem = group.Target,
        //                    TargetCode = te.Code,
        //                    TargetDisplay = targetDisplay,
        //                    Relationship = te.Relationship,
        //                    Comment = te.Comment,
        //                    IsGenerated = isGenerated,
        //                    NeedsReview = needsReview,
        //                };

        //                // insert our record, we do not need the id back
        //                ValueSetCodeComparisonRec.Insert(_dbConnection, codeComparison, vsComp.TableName);
        //            }

        //            // check for no-maps
        //            if (se.NoMap == true)
        //            {
        //                // create a new comparison record
        //                ValueSetCodeComparisonRec codeComparison = new()
        //                {
        //                    Id = -1,
        //                    ValueSetPairComparisonId = vsComp.Id,
        //                    SourceSystem = group.Source,
        //                    SourceCode = se.Code,
        //                    SourceDisplay = sourceDisplay,
        //                    NoMap = false,
        //                    TargetSystem = string.IsNullOrEmpty(group.Target) ? null : group.Target,
        //                    TargetCode = null,
        //                    TargetDisplay = null,
        //                    Relationship = null,
        //                    Comment = null,
        //                    IsGenerated = null,
        //                    NeedsReview = null,
        //                };

        //                // insert our record, we do not need the id back
        //                ValueSetCodeComparisonRec.Insert(_dbConnection, codeComparison, vsComp.TableName);
        //            }
        //        }
        //    }
        //}
    }

    //private string getCompositeName(string sourceName, string? targetName)
    //{
    //    if (_isVersionComparison)
    //    {
    //        if (targetName == null)
    //        {
    //            return $"V{_source.MainPackageVersion}-{sourceName}";
    //        }

    //        return $"V{_source.MainPackageVersion}-{sourceName}-V{_target.MainPackageVersion}-{targetName}";
    //    }

    //    if (_isCoreComparison)
    //    {
    //        if (targetName == null)
    //        {
    //            return $"{_sourceRLiteral}-{sourceName}";
    //        }

    //        return $"{_sourceRLiteral}-{sourceName}-{_targetRLiteral}-{targetName}";
    //    }

    //    if (targetName == null)
    //    {
    //        return $"{_source.MainPackageId}-{sourceName}";
    //    }

    //    return $"{_source.MainPackageId}-{sourceName}-{_target.MainPackageId}-{targetName}";
    //}

    ///// <summary>Applies the canonical format.</summary>
    ///// <param name="formatString">The format string.</param>
    ///// <param name="canonical">   The canonical.</param>
    ///// <param name="name">        The name.</param>
    ///// <returns>A string.</returns>
    ///// <remarks>
    ///// 0 = canonical, 1 = resourceType, 2 = name, 3 = leftRLiteral, 4 = leftShortVersion, 5 = rightRLiteral, 6 = rightShortVersion
    ///// </remarks>
    //private string buildCanonicalUrl(string formatString, string canonical, string resourceType = "", string name = "") =>
    //    string.Format(formatString, canonical, resourceType, name, _sourceRLiteral, _sourceShortVersion, _targetRLiteral, _targetShortVersion);


    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                if (_db != null)
                {
                    _db.SaveChanges();
                    _db.Dispose();
                }
            }

            _disposedValue = true;
        }
    }

    void IDisposable.Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
