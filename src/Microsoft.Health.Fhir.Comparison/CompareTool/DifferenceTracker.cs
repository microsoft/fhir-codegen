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
using fhir_codegen.SQLiteGenerator;
using Hl7.Fhir.Model;
using Microsoft.Data.Sqlite;
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

    private DefinitionCollection _source;
    private DefinitionCollection _target;

    private FhirReleases.FhirSequenceCodes _sourceFhirSequence;
    private string _sourcePackageCanonical;
    private string _sourceRLiteral;
    private string _sourceShortVersion;
    private string _sourceShortVersionUrlSegment;
    private string _sourcePackageVersion;

    private FhirReleases.FhirSequenceCodes _targetFhirSequence;
    private string _targetPackageCanonical;
    private string _targetRLiteral;
    private string _targetShortVersion;
    private string _targetShortVersionUrlSegment;
    private string _targetPackageVersion;

    private string _sourceToTargetWithR;
    private int _sourceToTargetWithRLen;
    private string _sourceToTargetNoR;
    private int _sourceToTargetNoRLen;

    private string _canonicalUrl;
    private string _packageId;
    private bool _isCoreComparison;
    private bool _isVersionComparison;

    private IDbConnection? _dbConnection = null;
    private string _dbPath;

    private string _dbName;

    public DifferenceTracker(
        DefinitionCollection source,
        DefinitionCollection target,
        string dbPath,
        ILoggerFactory? loggerFactory = null)
    {
        _dbPath = dbPath;
        if (!Directory.Exists(_dbPath))
        {
            Directory.CreateDirectory(_dbPath);
        }

        _source = source;

        _sourcePackageCanonical = source.MainPackageCanonical;
        _sourceFhirSequence = source.FhirSequence;
        _sourceRLiteral = source.FhirSequence.ToRLiteral();
        _sourceShortVersion = source.FhirSequence.ToShortVersion();
        _sourceShortVersionUrlSegment = "/" + _sourceShortVersion + "/";
        _sourcePackageVersion = source.FhirVersionLiteral;

        _target = target;

        _targetPackageCanonical = target.MainPackageCanonical;
        _targetFhirSequence = target.FhirSequence;
        _targetRLiteral = target.FhirSequence.ToRLiteral();
        _targetShortVersion = target.FhirSequence.ToShortVersion();
        _targetShortVersionUrlSegment = "/" + _targetShortVersion + "/";
        _targetPackageVersion = target.FhirVersionLiteral;

        _sourceToTargetWithR = $"{_sourceRLiteral}to{_targetRLiteral}";
        _sourceToTargetWithRLen = _sourceToTargetWithR.Length;
        _sourceToTargetNoR = $"{_sourceRLiteral[1..]}to{_targetRLiteral[1..]}";
        _sourceToTargetNoRLen = _sourceToTargetNoR.Length;

        //_mapCanonical = $"http://hl7.org/fhir/uv/xver/{_leftRLiteral.ToLowerInvariant()}-{_rightRLiteral.ToLowerInvariant()}";
        _canonicalUrl = buildCanonicalUrl("{0}{3}-{5}", _canonicalRootCrossVersion);
        _isCoreComparison = FhirPackageUtils.PackageIsFhirCore(_source.MainPackageId) && FhirPackageUtils.PackageIsFhirCore(_target.MainPackageId);
        _packageId = _isCoreComparison
            ? $"hl7.fhir.uv.xver.{_sourceRLiteral.ToLowerInvariant()}-{_targetRLiteral.ToLowerInvariant()}"
            : $"hl7.fhir.uv.xver.{_source.MainPackageId}-{_target.MainPackageId}";

        _isVersionComparison = _source.MainPackageId == _target.MainPackageId;

        if (_isVersionComparison)
        {
            _dbName = $"V{_sourcePackageVersion.Replace('.', '_')}-V{_targetPackageVersion.Replace('.', '_')}.db";
        }
        else if (_isCoreComparison)
        {
            _dbName = $"{_source.FhirSequence.ToRLiteral()}-{_target.FhirSequence.ToRLiteral()}.db";
        }
        else
        {
            _dbName =
                $"{_source.MainPackageId.ToPascalCase()}-V{_sourcePackageVersion.Replace('.', '_')}" +
                $"-{_target.MainPackageId.ToPascalCase()}-V{_targetPackageVersion.Replace('.', '_')}.db";
        }

        _loggerFactory = loggerFactory;
        _logger = loggerFactory?.CreateLogger<DifferenceTracker>() ?? _source.Logger;
    }


    /// <summary>
    /// Initializes the database connection and sets up the necessary tables and metadata.
    /// </summary>
    /// <param name="createdNew">Outputs a boolean indicating whether a new database was created.</param>
    /// <exception cref="Exception">Thrown when the package information does not match or the database connection is not initialized.</exception>
    public void InitDb(
        HashSet<string> _exclusionSet,
        HashSet<string> _escapeValveCodes,
        out bool createdNew)
    {
        string connectionString = new SqliteConnectionStringBuilder()
        {
            DataSource = Path.Combine(_dbPath, _dbName),
            Mode = SqliteOpenMode.ReadWriteCreate,
        }.ToString();

        _dbConnection = new SqliteConnection(connectionString);

        _dbConnection.Open();

        // ensure our metadata exists
        ComparisonMetadata.CreateTable(_dbConnection);

        // get or set package information
        ComparisonMetadata? ci = ComparisonMetadata.SelectSingle(_dbConnection);
        if (ci == null)
        {
            ci = new()
            {
                Id = -1,
                SourcePackageId = _source.MainPackageId,
                SourcePackageVersion = _source.MainPackageVersion,
                TargetPackageId = _target.MainPackageId,
                TargetPackageVersion = _target.MainPackageVersion,
                Name = $"Comparison between {_source.MainPackageId}:{_sourcePackageVersion} and {_target.MainPackageId}:{_targetPackageVersion}",
                PackageId = _packageId,
                PackageVersion = "0.0.1",
                CanonicalUrl = _canonicalUrl,
            };

            ci = ComparisonMetadata.Insert(_dbConnection, ci);
            createdNew = true;
        }
        else
        {
            // make sure this database represents this comparison
            if ((ci.SourcePackageId != _source.MainPackageId) ||
                (ci.SourcePackageVersion != _source.MainPackageVersion) ||
                (ci.TargetPackageId != _target.MainPackageId) ||
                (ci.TargetPackageVersion != _target.MainPackageVersion))
            {
                throw new Exception("Package information does not match.");
            }

            createdNew = false;
        }

        // make sure the relationship mapping is created
        RelationshipLookup.CreateTable(_dbConnection);

        // iterate over all the relationship values in the enum and add to the table if it does not exist
        foreach (CMR value in Enum.GetValues<CMR>())
        {
            if (RelationshipLookup.SelectSingle(_dbConnection, Relationship: value) != null)
            {
                continue;
            }

            RelationshipLookup.Insert(_dbConnection, new RelationshipLookup()
            {
                Relationship = value,
                Name = value.ToString(),
            });
        }

        // load our value sets
        loadValueSets(_source, _prefixSource, _exclusionSet, _escapeValveCodes);
        loadValueSets(_target, _prefixTarget, _exclusionSet, _escapeValveCodes);
    }

    private void loadValueSets(
        DefinitionCollection dc,
        string prefix,
        HashSet<string> _exclusionSet,
        HashSet<string> _escapeValveCodes)
    {
        if (_dbConnection == null)
        {
            throw new Exception("Database connection not initialized.");
        }

        // make sure our value set metadata table exists
        ValueSetMetadata.CreateTable(_dbConnection);

        // iterate over the value sets in the definition collection
        foreach ((string unversionedUrl, string[] versions) in dc.ValueSetVersions.OrderBy(kvp => kvp.Key))
        {
            // only use the highest version in the package
            string vsVersion = versions.OrderDescending().First();
            string versionedUrl = unversionedUrl + "|" + vsVersion;

            // check to see if this value set already exists
            if (ValueSetMetadata.SelectSingle(_dbConnection, CanonicalUrl: versionedUrl) != null)
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
                    Id = -1,
                    PackageId = dc.MainPackageId,
                    PackageVersion = dc.MainPackageVersion,
                    CanonicalUrl = versionedUrl,
                    Name = uvs.Name,
                    Version = vsVersion,
                    TableName = prefix + _prefixVSExpansion + FhirSanitizationUtils.SanitizeForProperty(uvs.Name).Replace('-', '_'),
                    Description = uvs.Description,
                    CanExpand = canExpand,
                    HasEscapeValveCode = hasEscapeCode,
                    Message = expandMessage,
                };

                ValueSetMetadata.Insert(_dbConnection, vsmExcluded);

                continue;
            }

            // create a new metadata record
            ValueSetMetadata vsm = new()
            {
                Id = -1,
                PackageId = dc.MainPackageId,
                PackageVersion = dc.MainPackageVersion,
                CanonicalUrl = versionedUrl,
                Name = uvs.Name,
                Version = vsVersion,
                TableName = prefix + _prefixVSExpansion + FhirSanitizationUtils.SanitizeForProperty(uvs.Name).Replace('-', '_'),
                Description = uvs.Description,
                CanExpand = canExpand,
                HasEscapeValveCode = hasEscapeCode,
                Message = expandMessage,
            };

            // insert and update our local copy for the id
            vsm = ValueSetMetadata.Insert(_dbConnection, vsm);

            // create a table for our value set contents
            ValueSetContent.CreateTable(_dbConnection, vsm.TableName);

            // iterate over all the contents of the value set
            foreach (FhirConcept fc in vs.cgGetFlatConcepts(dc))
            {
                // check for this system and code already existing
                if (ValueSetContent.SelectSingle(_dbConnection, vsm.TableName, System: fc.System, Code: fc.Code) != null)
                {
                    continue;
                }

                // create a new content record
                ValueSetContent vsc = new()
                {
                    Id = -1,
                    ValueSetMetadataId = vsm.Id,
                    System = fc.System,
                    Code = fc.Code,
                    Display = fc.Display,
                };

                // insert our record, we do not need the id back
                ValueSetContent.Insert(_dbConnection, vsc, vsm.TableName);
            }
        }
    }


    public void LoadFromCrossVersionMaps(CrossVersionMapCollection cv)
    {
        loadCrossVersionValueSetMaps(cv);
    }

    private void loadCrossVersionValueSetMaps(CrossVersionMapCollection cv)
    {
        if (_dbConnection == null)
        {
            throw new Exception("Database connection not initialized.");
        }

        ValueSetPairComparison.CreateTable(_dbConnection);

        // iterate over the maps in the collection
        foreach (ConceptMap cm in cv._dc.ConceptMapsByUrl.Values)
        {
            // skip ones that are not value set comparisons
            if (!CrossVersionMapCollection.isValueSetConceptMap(cm))
            {
                continue;
            }

            string sourceScopeUrl = string.Empty;
            string targetScopeUrl = string.Empty;

            if (cm.SourceScope is Canonical originalSourceCanonical)
            {
                sourceScopeUrl = originalSourceCanonical.Uri + "|" + originalSourceCanonical.Version;
            }

            if (cm.SourceScope is FhirUri originalSourceUri)
            {
                string elementPath = originalSourceUri.Value.Split('#')[^1];
                if (_source.TryFindElementByPath(elementPath, out StructureDefinition? sd, out ElementDefinition? ed))
                {
                    sourceScopeUrl = ed.Binding?.ValueSet ?? string.Empty;
                }
            }

            if (cm.TargetScope is Canonical originalTargetCanonical)
            {
                targetScopeUrl = originalTargetCanonical.Uri + "|" + originalTargetCanonical.Version;
            }

            if (cm.TargetScope is FhirUri originalTargetUri)
            {
                string elementPath = originalTargetUri.Value.Split('#')[^1];
                if (_target.TryFindElementByPath(elementPath, out StructureDefinition? sd, out ElementDefinition? ed))
                {
                    targetScopeUrl = ed.Binding?.ValueSet ?? string.Empty;
                }
            }

            string unversionedSourceUrl = sourceScopeUrl.Contains('|') ? sourceScopeUrl.Split('|')[0] : sourceScopeUrl;
            string unversionedTargetUrl = targetScopeUrl.Contains('|') ? targetScopeUrl.Split('|')[0] : targetScopeUrl;

            if (string.IsNullOrEmpty(sourceScopeUrl) || string.IsNullOrEmpty(targetScopeUrl))
            {
                throw new Exception($"Invalid Concept Map {cm.Id} ({cm.Url}): sourceScope: {sourceScopeUrl} targetScope: {targetScopeUrl}");
            }

            // check for exclusions
            if (PackageComparer._exclusionSet.Contains(sourceScopeUrl))
            {
                continue;
            }

            // try to resolve the source value set
            if (!_source.TryGetValueSet(unversionedSourceUrl, out ValueSet? sourceVs))
            {
                _logger.LogFailedToResolveVs(unversionedSourceUrl, _source.MainPackageId, _source.MainPackageVersion);
                continue;
            }

            ILookup<string, FhirConcept> sourceConcepts = sourceVs.cgGetFlatConcepts(_source).ToLookup(fc => fc.Key);

            // try to resolve the target value set
            if (!_target.TryGetValueSet(unversionedTargetUrl, out ValueSet? targetVs))
            {
                _logger.LogFailedToResolveVs(unversionedTargetUrl, _target.MainPackageId, _target.MainPackageVersion);
                return;
            }

            ILookup<string, FhirConcept> targetConcepts = targetVs.cgGetFlatConcepts(_target).ToLookup(fc => fc.Key);

            CMR? vsRelationship = cm.GetExtensionValue<Code<ConceptMap.ConceptMapRelationship>>(CommonDefinitions.ExtUrlConceptMapAggregateRelationship)?.Value;

            string cName = getCompositeName(sourceVs.Name, targetVs.Name);

            ValueSetPairComparison vsComp = new()
            {
                Id = -1,
                SourceCanonical = sourceVs.Url,
                SourceName = sourceVs.Name,
                SourceVersion = sourceVs.Version,
                TargetCanonical = targetVs.Url,
                TargetName = targetVs.Name,
                TargetVersion = targetVs.Version,
                CompositeName = cName,
                TableName = _prefixVSComparison + FhirSanitizationUtils.SanitizeForProperty(cName).Replace('-', '_'),
                Relationship = vsRelationship,
                IssueCode = null,
                Message = $"Created from existing map: {cm.Url}",
                Map = cm,
                LastReviewedBy = null,
                LastReviewedOn = null,
            };

            // insert and update our local copy for the id
            vsComp = ValueSetPairComparison.Insert(_dbConnection, vsComp);

            // create a table for our comparisons
            ValueSetCodeComparisonRec.CreateTable(_dbConnection, vsComp.TableName);

            // iterate over the groups
            foreach (ConceptMap.GroupComponent group in cm.Group)
            {
                // iterate over all the elements
                foreach (ConceptMap.SourceElementComponent se in group.Element)
                {
                    string sourceDisplay = se.Display;
                    if (string.IsNullOrEmpty(sourceDisplay))
                    {
                        string key = group.Source + "#" + se.Code;
                        if (sourceConcepts.Contains(key))
                        {
                            sourceDisplay = sourceConcepts[key].First().Display;
                        }
                    }

                    // iterate over all the targets
                    foreach (ConceptMap.TargetElementComponent te in se.Target)
                    {
                        // check for properties
                        bool? isGenerated = te.Property
                            .Where(p => p.Code == CommonDefinitions.ConceptMapPropertyGenerated)
                            .FirstOrDefault()?.Value is FhirBoolean fbIsGenerated
                            ? fbIsGenerated.Value
                            : null;

                        bool? needsReview = te.Property
                            .Where(p => p.Code == CommonDefinitions.ConceptMapPropertyNeedsReview)
                            .FirstOrDefault()?.Value is FhirBoolean fbNeedsReview
                            ? fbNeedsReview.Value
                            : null;

                        string targetDisplay = te.Display;
                        if (string.IsNullOrEmpty(targetDisplay))
                        {
                            string key = group.Target + "#" + te.Code;
                            if (targetConcepts.Contains(key))
                            {
                                targetDisplay = targetConcepts[key].First().Display;
                            }
                        }

                        // create a new comparison record
                        ValueSetCodeComparisonRec codeComparison = new()
                        {
                            Id = -1,
                            ValueSetPairComparisonId = vsComp.Id,
                            SourceSystem = group.Source,
                            SourceCode = se.Code,
                            SourceDisplay = sourceDisplay,
                            NoMap = false,
                            TargetSystem = group.Target,
                            TargetCode = te.Code,
                            TargetDisplay = targetDisplay,
                            Relationship = te.Relationship,
                            Comment = te.Comment,
                            IsGenerated = isGenerated,
                            NeedsReview = needsReview,
                        };

                        // insert our record, we do not need the id back
                        ValueSetCodeComparisonRec.Insert(_dbConnection, codeComparison, vsComp.TableName);
                    }

                    // check for no-maps
                    if (se.NoMap == true)
                    {
                        // create a new comparison record
                        ValueSetCodeComparisonRec codeComparison = new()
                        {
                            Id = -1,
                            ValueSetPairComparisonId = vsComp.Id,
                            SourceSystem = group.Source,
                            SourceCode = se.Code,
                            SourceDisplay = sourceDisplay,
                            NoMap = false,
                            TargetSystem = string.IsNullOrEmpty(group.Target) ? null : group.Target,
                            TargetCode = null,
                            TargetDisplay = null,
                            Relationship = null,
                            Comment = null,
                            IsGenerated = null,
                            NeedsReview = null,
                        };

                        // insert our record, we do not need the id back
                        ValueSetCodeComparisonRec.Insert(_dbConnection, codeComparison, vsComp.TableName);
                    }
                }
            }
        }
    }

    private string getCompositeName(string sourceName, string? targetName)
    {
        if (_isVersionComparison)
        {
            if (targetName == null)
            {
                return $"V{_source.MainPackageVersion}-{sourceName}";
            }

            return $"V{_source.MainPackageVersion}-{sourceName}-V{_target.MainPackageVersion}-{targetName}";
        }

        if (_isCoreComparison)
        {
            if (targetName == null)
            {
                return $"{_sourceRLiteral}-{sourceName}";
            }

            return $"{_sourceRLiteral}-{sourceName}-{_targetRLiteral}-{targetName}";
        }

        if (targetName == null)
        {
            return $"{_source.MainPackageId}-{sourceName}";
        }

        return $"{_source.MainPackageId}-{sourceName}-{_target.MainPackageId}-{targetName}";
    }

    /// <summary>Applies the canonical format.</summary>
    /// <param name="formatString">The format string.</param>
    /// <param name="canonical">   The canonical.</param>
    /// <param name="name">        The name.</param>
    /// <returns>A string.</returns>
    /// <remarks>
    /// 0 = canonical, 1 = resourceType, 2 = name, 3 = leftRLiteral, 4 = leftShortVersion, 5 = rightRLiteral, 6 = rightShortVersion
    /// </remarks>
    private string buildCanonicalUrl(string formatString, string canonical, string resourceType = "", string name = "") =>
        string.Format(formatString, canonical, resourceType, name, _sourceRLiteral, _sourceShortVersion, _targetRLiteral, _targetShortVersion);


    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                if (_dbConnection != null)
                {
                    _dbConnection.Close();
                    _dbConnection.Dispose();
                    _dbConnection = null;
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
