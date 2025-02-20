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
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGen.Utils;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.FhirExtensions;
using Microsoft.Health.Fhir.CodeGenCommon.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using Microsoft.Health.Fhir.CodeGenCommon.Utils;
using Microsoft.Health.Fhir.Comparison.CompareTool;
using Microsoft.Health.Fhir.Comparison.Models;
using Newtonsoft.Json.Linq;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;

namespace Microsoft.Health.Fhir.Comparison.Models;


internal static partial class ContentDatabaseLogMessages
{
    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to resolve ValueSet {vsUrl} in {sourcePackage}:{sourceVersion}.")]
    internal static partial void LogFailedToResolveVs(this ILogger logger, string vsUrl, string sourcePackage, string sourceVersion);
}


public class ComparisonDatabase : IDisposable
{
    private const string _canonicalRootCrossVersion = "http://hl7.org/fhir/uv/xver/";
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

    private (DefinitionCollection dc, DcInfoRec info)[]? _definitions = null;

    private bool _isCoreComparison;
    private bool _isVersionComparison;

    private string _dbPath;
    private string _dbName;

    private ComparisonDbContext _db;

    public ComparisonDatabase(
        DefinitionCollection[] definitions,
        string dbPath,
        ILoggerFactory? loggerFactory = null)
    {
        if (definitions.Length == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(definitions));
        }

        _loggerFactory = loggerFactory;
        _logger = loggerFactory?.CreateLogger<DifferenceTracker>() ?? definitions[0].Logger;
        _definitions = definitions.Select(dc => (dc, new DcInfoRec()
        {
            FhirSequence = dc.FhirSequence,
            PackageCanonical = dc.MainPackageCanonical,
            RLiteral = dc.FhirSequence.ToRLiteral(),
            ShortVersion = dc.FhirSequence.ToShortVersion(),
            ShortVersionUrlSegment = "/" + dc.FhirSequence.ToShortVersion() + "/",
            PackageVersion = dc.FhirVersionLiteral,
        })).ToArray();

        _dbPath = dbPath;
        if (!Directory.Exists(_dbPath))
        {
            Directory.CreateDirectory(_dbPath);
        }

        _isCoreComparison = definitions.All(dc => FhirPackageUtils.PackageIsFhirCore(dc.MainPackageId));
        _isVersionComparison = definitions.Select(dc => dc.MainPackageId).Distinct().Count() == 1;

        switch (definitions.Length)
        {
            case 1:
                _dbName = $"{definitions[0].MainPackageId.ToPascalCase()}-V{definitions[0].MainPackageVersion.Replace('.', '_')}.db";
                break;
            case 2:
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
                break;
            default:
                _dbName = "fhir-comparison.db";
                break;
        }

        _db = new(Path.Combine(_dbPath, _dbName));

        initNewDb(true);
    }

    public ComparisonDatabase(
        string dbPath,
        string dbName,
        ILoggerFactory? loggerFactory = null)
    {
        _loggerFactory = loggerFactory;
        _logger = loggerFactory?.CreateLogger<DifferenceTracker>() ?? NullLoggerFactory.Instance.CreateLogger(nameof(ComparisonDatabase));

        _dbPath = dbPath;
        if (!Directory.Exists(_dbPath))
        {
            Directory.CreateDirectory(_dbPath);
        }

        _dbName = dbName;
        _db = new(Path.Combine(_dbPath, _dbName));

        _db.Database.EnsureCreated();
    }


    /// <summary>
    /// Initializes the database connection and sets up the necessary tables and metadata.
    /// </summary>
    /// <param name="createdNew">Outputs a boolean indicating whether a new database was created.</param>
    /// <exception cref="Exception">Thrown when the package information does not match or the database connection is not initialized.</exception>
    private void initNewDb(
        bool ensureDeleted)
    {
        if (_definitions == null)
        {
            throw new Exception($"Cannot initialize clean database without packages!");
        }

        if (ensureDeleted)
        {
            _db.Database.EnsureDeleted();
        }

        _db.Database.EnsureCreated();

        foreach ((DefinitionCollection dc, DcInfoRec _) in _definitions)
        {
            // add data about our packages
            if (!_db.Packages.Any(pm => (pm.PackageId == dc.MainPackageId) && (pm.PackageVersion == dc.MainPackageVersion)))
            {
                _db.Add(new DbFhirPackage()
                {
                    Name = dc.Name,
                    PackageId = dc.MainPackageId,
                    PackageVersion = dc.MainPackageVersion,
                    CanonicalUrl = dc.MainPackageCanonical,
                });
            }
        }

        _db.SaveChanges();
    }

    public void ExportCollectionContents(
        HashSet<string> _exclusionSet,
        HashSet<string> _escapeValveCodes)
    {
        if (_definitions == null)
        {
            throw new Exception("Cannot export contents without a DefinitionCollection!");
        }

        foreach ((DefinitionCollection dc, DcInfoRec _) in _definitions)
        {
            // load our value sets
            addValueSetsToDb(dc, _exclusionSet, _escapeValveCodes);

            // load our structures
            addStructuresToDb(dc, _exclusionSet);

            // ensure all contents are written
            _db.SaveChanges();
        }
    }

    private void addValueSetsToDb(
        DefinitionCollection dc,
        HashSet<string> _exclusionSet,
        HashSet<string> _escapeValveCodes)
    {
        // get the package metadata for this definition collection
        DbFhirPackage pm = _db.Packages.Single(pm => (pm.PackageId == dc.MainPackageId) && (pm.PackageVersion == dc.MainPackageVersion));

        // iterate over the value sets in the definition collection
        foreach ((string unversionedUrl, string[] versions) in dc.ValueSetVersions.OrderBy(kvp => kvp.Key))
        {
            // only use the highest version in the package
            string vsVersion = versions.OrderDescending().First();
            string versionedUrl = unversionedUrl + "|" + vsVersion;

            // check to see if this value set already exists
            if (_db.ValueSets.Any(vsm => (vsm.Url == unversionedUrl) && (vsm.Version == vsVersion)))
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

            IEnumerable<StructureElementCollection> coreBindings = dc.CoreBindingsForVs(versionedUrl);
            BindingStrength? strongestBindingCore = dc.StrongestBinding(coreBindings);
            IReadOnlyDictionary<string, BindingStrength> coreBindingStrengthByType = dc.BindingStrengthByType(coreBindings);

            IEnumerable<StructureElementCollection> extendedBindings = dc.ExtendedBindingsForVs(versionedUrl);
            BindingStrength? strongestBindingExtended = dc.StrongestBinding(extendedBindings);
            IReadOnlyDictionary<string, BindingStrength> extendedBindingStrengthByType = dc.BindingStrengthByType(extendedBindings);

            // will not further process value sets we know we will not process
            if (_exclusionSet.Contains(unversionedUrl) ||
                !canExpand ||
                (vs == null))
            {
                // still add a metadata record
                DbValueSet vsmExcluded = new()
                {
                    FhirPackage = pm,
                    Id = uvs.Id,
                    Url = versionedUrl,
                    Name = uvs.Name,
                    Version = vsVersion,
                    Status = uvs.Status,
                    Title = uvs.Title,
                    Description = uvs.Description,
                    Purpose = uvs.Purpose,
                    CanExpand = canExpand,
                    HasEscapeValveCode = hasEscapeCode,
                    Message = expandMessage,
                    BindingCountCore = coreBindings.Count(),
                    StrongestBindingCore = strongestBindingCore,
                    StrongestBindingCoreCode = coreBindingStrengthByType.TryGetValue("code", out BindingStrength ebscCode) ? ebscCode : null,
                    StrongestBindingCoreCoding = coreBindingStrengthByType.TryGetValue("Coding", out BindingStrength ebscCoding) ? ebscCoding : null,
                    BindingCountExtended = extendedBindings.Count(),
                    StrongestBindingExtended = strongestBindingExtended,
                    StrongestBindingExtendedCode = extendedBindingStrengthByType.TryGetValue("code", out BindingStrength ebseCode) ? ebseCode : null,
                    StrongestBindingExtendedCoding = extendedBindingStrengthByType.TryGetValue("Coding", out BindingStrength ebseCoding) ? ebseCoding : null,
                };

                _db.ValueSets.Add(vsmExcluded);

                continue;
            }

            // create a new metadata record
            DbValueSet dbVs = new()
            {
                FhirPackage = pm,
                Id = vs.Id,
                Url = versionedUrl,
                Name = vs.Name,
                Version = vsVersion,
                Status = vs.Status,
                Title = vs.Title,
                Description = vs.Description,
                Purpose = vs.Purpose,
                CanExpand = canExpand,
                HasEscapeValveCode = hasEscapeCode,
                Message = expandMessage,
                BindingCountCore = coreBindings.Count(),
                StrongestBindingCore = strongestBindingCore,
                StrongestBindingCoreCode = coreBindingStrengthByType.TryGetValue("code", out BindingStrength bscCode) ? bscCode : null,
                StrongestBindingCoreCoding = coreBindingStrengthByType.TryGetValue("Coding", out BindingStrength bscCoding) ? bscCoding : null,
                BindingCountExtended = extendedBindings.Count(),
                StrongestBindingExtended = strongestBindingExtended,
                StrongestBindingExtendedCode = extendedBindingStrengthByType.TryGetValue("code", out BindingStrength bseCode) ? bseCode : null,
                StrongestBindingExtendedCoding = extendedBindingStrengthByType.TryGetValue("Coding", out BindingStrength bseCoding) ? bseCoding : null,
            };

            // insert and update our local copy for the id
            _db.ValueSets.Add(dbVs);

            // iterate over all the contents of the value set
            foreach (FhirConcept fc in vs.cgGetFlatConcepts(dc))
            {
                // check for this record already existing
                if (_db.Concepts.Any(vsc => vsc.ValueSetKey == dbVs.Key && vsc.System == fc.System && vsc.Code == fc.Code))
                {
                    continue;
                }

                // create a new content record
                DbValueSetConcept dbConcept = new()
                {
                    FhirPackage = pm,
                    ValueSet = dbVs,
                    System = fc.System,
                    Code = fc.Code,
                    Display = fc.Display,
                };

                _db.Concepts.Add(dbConcept);
            }
        }

        return;
    }

    private void addStructuresToDb(
        DefinitionCollection dc,
        HashSet<string> _exclusionSet)
    {
        // get the package metadata for this definition collection
        DbFhirPackage pm = _db.Packages.Single(pm => (pm.PackageId == dc.MainPackageId) && (pm.PackageVersion == dc.MainPackageVersion));

        // iterate over the types of structures
        foreach ((IEnumerable<StructureDefinition> structures, FhirArtifactClassEnum cgClass) in getStructures(dc))
        {
            foreach (StructureDefinition sd in structures)
            {
                // will not further process value sets we know we will not process
                if (_exclusionSet.Contains(sd.Url))
                {
                    // still add a metadata record
                    DbStructureDefinition sdmExcluded = new()
                    {
                        FhirPackage = pm,
                        Id = sd.Id,
                        Url = sd.Url,
                        Name = sd.Name,
                        Version = sd.Version,
                        Status = sd.Status,
                        Title = sd.Title ?? sd.Snapshot?.Element.FirstOrDefault()?.Short,
                        Description = sd.Description ?? sd.Snapshot?.Element.FirstOrDefault()?.Definition,
                        Purpose = sd.Purpose,
                        Comment = sd.Snapshot?.Element.FirstOrDefault()?.Comment,
                        ArtifactClass = cgClass,
                        Message = "Manually excluded",
                    };

                    _db.Structures.Add(sdmExcluded);

                    continue;
                }

                // create a new metadata record
                DbStructureDefinition dbStructure = new()
                {
                    FhirPackage = pm,
                    Id = sd.Id,
                    Url = sd.Url,
                    Name = sd.Name,
                    Version = sd.Version,
                    Status = sd.Status,
                    Title = sd.Title ?? sd.Snapshot?.Element.FirstOrDefault()?.Short,
                    Description = sd.Description ?? sd.Snapshot?.Element.FirstOrDefault()?.Definition,
                    Purpose = sd.Purpose,
                    Comment = sd.Snapshot?.Element.FirstOrDefault()?.Comment,
                    ArtifactClass = cgClass,
                    Message = string.Empty,
                };

                // insert and update our local copy for the id
                _db.Structures.Add(dbStructure);

                // iterate over all the elements of the structure
                foreach (ElementDefinition ed in sd.cgElements(skipSlices: false))
                {
                    addElement(dbStructure, sd, ed);
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

        void addElement(DbStructureDefinition dbStructure, StructureDefinition sd, ElementDefinition ed)
        {
            // check for children
            int childCount = sd.cgElements(
                ed.Path,
                topLevelOnly: true,
                includeRoot: false,
                skipSlices: true).Count();

            List<(string typeName, string? typeProfile, string? profile)> elementTypes = [];

            IEnumerable<ElementDefinition.TypeRefComponent> definedTypes = ed.Type.Select(tr => tr.cgAsR5());
            foreach (ElementDefinition.TypeRefComponent tr in definedTypes)
            {
                if ((tr.ProfileElement.Count == 0) &&
                    (tr.TargetProfileElement.Count == 0))
                {
                    elementTypes.Add((tr.cgName(), null, null));
                    continue;
                }

                if (tr.ProfileElement.Count == 0)
                {
                    foreach (Canonical tp in tr.TargetProfile)
                    {
                        elementTypes.Add((tr.cgName(), null, tp.Value));
                    }

                    continue;
                }

                if (tr.TargetProfileElement.Count == 0)
                {
                    foreach (Canonical p in tr.Profile)
                    {
                        elementTypes.Add((tr.cgName(), p.Value, null));
                    }

                    continue;
                }

                foreach (Canonical p in tr.Profile)
                {
                    foreach (Canonical tp in tr.TargetProfile)
                    {
                        elementTypes.Add((tr.cgName(), p.Value, tp.Value));
                    }
                }
            }

            foreach ((string typeName, string? typeProfile, string? targetProfile) in elementTypes)
            {
                DbElement dbElement = new()
                {
                    FhirPackage = pm,
                    Structure = dbStructure,
                    ResourceFieldOrder = ed.cgFieldOrder(),
                    ComponentFieldOrder = ed.cgComponentFieldOrder(),
                    Id = ed.ElementId,
                    Path = ed.Path,
                    ChildElementCount = childCount,
                    Name = ed.cgName(),
                    Short = ed.Short,
                    Definition = ed.Definition,
                    MinCardinality = ed.cgCardinalityMin(),
                    MaxCardinality = ed.cgCardinalityMax(),
                    MaxCardinalityString = ed.Max ?? "*",
                    SliceName = ed.SliceName,
                    ValueSetBindingStrength = ed.Binding?.Strength,
                    BindingValueSet = ed.Binding?.ValueSet,
                    TypeName = typeName,
                    TypeProfile = typeProfile,
                    TargetProfile = typeProfile,
                };

                _db.Elements.Add(dbElement);
            }
        }


        //DbElementDefinition addElement(DbStructureDefinition dbStructure, StructureDefinition sd, ElementDefinition ed)
        //{
        //    // check for children
        //    int childCount = sd.cgElements(
        //        ed.Path,
        //        topLevelOnly: true,
        //        includeRoot: false,
        //        skipSlices: true).Count();

        //    DbElementDefinition dbElement = new()
        //    {
        //        Structure = dbStructure,
        //        ResourceFieldOrder = ed.cgFieldOrder(),
        //        ComponentFieldOrder = ed.cgComponentFieldOrder(),
        //        Id = ed.ElementId,
        //        Path = ed.Path,
        //        ChildElementCount = childCount,
        //        Name = ed.cgName(),
        //        Short = ed.Short,
        //        Definition = ed.Definition,
        //        MinCardinality = ed.cgCardinalityMin(),
        //        MaxCardinality = ed.cgCardinalityMax(),
        //        MaxCardinalityString = ed.Max ?? "*",
        //        SliceName = ed.SliceName,
        //        ValueSetBindingStrength = ed.Binding?.Strength,
        //        BindingValueSet = ed.Binding?.ValueSet,
        //        ElementTypes = [],
        //        ElementTypeMappings = [],
        //    };

        //    _db.Elements.Add(dbElement);

        //    addElementTypes(dbElement, ed);

        //    return dbElement;
        //}

        //void addElementTypes(DbElementDefinition dbElement, ElementDefinition ed)
        //{
        //    IReadOnlyDictionary<string, ElementDefinition.TypeRefComponent> edTypes = ed.cgTypes(coerceToR5: true);

        //    foreach ((string typeName, ElementDefinition.TypeRefComponent tr) in edTypes)
        //    {
        //        if ((tr.ProfileElement.Count == 0) &&
        //            (tr.TargetProfileElement.Count == 0))
        //        {
        //            dbElement.ElementTypes.Add(getOrAddType(tr.cgName(), null, null, ed.Binding?.Strength, ed.Binding?.ValueSet));
        //            continue;
        //        }

        //        if (tr.ProfileElement.Count == 0)
        //        {
        //            foreach (Canonical tp in tr.TargetProfile)
        //            {
        //                dbElement.ElementTypes.Add(getOrAddType(tr.cgName(), null, tp.Value, ed.Binding?.Strength, ed.Binding?.ValueSet));
        //            }

        //            continue;
        //        }

        //        if (tr.TargetProfileElement.Count == 0)
        //        {
        //            foreach (Canonical p in tr.Profile)
        //            {
        //                dbElement.ElementTypes.Add(getOrAddType(tr.cgName(), p.Value, null, ed.Binding?.Strength, ed.Binding?.ValueSet));
        //            }

        //            continue;
        //        }

        //        foreach (Canonical p in tr.Profile)
        //        {
        //            foreach (Canonical tp in tr.TargetProfile)
        //            {
        //                dbElement.ElementTypes.Add(getOrAddType(tr.cgName(), p.Value, tp.Value, ed.Binding?.Strength, ed.Binding?.ValueSet));
        //            }
        //        }
        //    }
        //}

        //DbElementType getOrAddType(string typeName, string? profile, string? targetProfile, BindingStrength? bS, string? bVs)
        //{
        //    DbElementType? et = _db.ElementTypes
        //        .FirstOrDefault(et =>
        //            (et.Name == typeName) &&
        //            (et.Profile == profile) &&
        //            (et.TargetProfile == targetProfile) &&
        //            (et.ValueSetBindingStrength == bS) &&
        //            (et.BindingValueSet == bVs));
        //    if (et != null)
        //    {
        //        return et;
        //    }

        //    et = new()
        //    {
        //        Name = typeName,
        //        Profile = profile,
        //        TargetProfile = targetProfile,
        //        ValueSetBindingStrength = bS,
        //        BindingValueSet = bVs,
        //        Elements = [],
        //        ElementTypeMappings = [],
        //    };

        //    _db.ElementTypes.Add(et);
        //    _db.SaveChanges();

        //    return et;
        //}
    }

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
