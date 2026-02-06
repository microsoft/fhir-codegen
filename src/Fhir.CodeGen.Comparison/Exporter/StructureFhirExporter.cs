using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using Fhir.CodeGen.Common.Extensions;
using Fhir.CodeGen.Common.FhirExtensions;
using Fhir.CodeGen.Common.Models;
using Fhir.CodeGen.Common.Packaging;
using Fhir.CodeGen.Common.Utils;
using Fhir.CodeGen.Comparison.CompareTool;
using Fhir.CodeGen.Comparison.Models;
using Fhir.CodeGen.Lib.FhirExtensions;
using Hl7.Fhir.Language.Debugging;
using Hl7.Fhir.Model;
using Hl7.Fhir.Model.CdsHooks;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Utility;
using Microsoft.Extensions.Logging;
using Octokit;
using static Fhir.CodeGen.Comparison.Exporter.IgExporter;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;

namespace Fhir.CodeGen.Comparison.Exporter;

public class StructureFhirExporter
{
    private readonly XVerExporter _exporter;
    private readonly IDbConnection _db;

    private ILoggerFactory _loggerFactory;
    private ILogger _logger;

    private Dictionary<FhirReleases.FhirSequenceCodes, List<DbElementType>> _extensionValueTypes = [];
    private Dictionary<FhirReleases.FhirSequenceCodes, HashSet<string>> _extensionValueTypeNames = [];

    private Dictionary<FhirReleases.FhirSequenceCodes, List<DbElement>> _canonicalTargetElements = [];
    private Dictionary<FhirReleases.FhirSequenceCodes, HashSet<string>> _canonicalTargetResourceNames = [];

    private class PackagePairStructureMappingTracker
    {
        public Dictionary<string, List<string>> TargetStructuresByName { get; set; } = [];
        public Dictionary<string, List<string>> TargetStructuresByUrl { get; set; } = [];
        public Dictionary<string, List<string>> TargetProfilesByName { get; set; } = [];
        public Dictionary<string, List<string>> TargetProfilesByUrl { get; set; } = [];

        public List<string> GetTargets(string source)
        {
            HashSet<string> valid = [];

            if (TargetStructuresByName.TryGetValue(source, out List<string>? values))
            {
                foreach (string v in values)
                {
                    valid.Add(v);
                }
            }

            if (TargetStructuresByUrl.TryGetValue(source, out values))
            {
                foreach (string v in values)
                {
                    valid.Add(v);
                }
            }

            if (TargetProfilesByName.TryGetValue(source, out values))
            {
                foreach (string v in values)
                {
                    valid.Add(v);
                }
            }

            if (TargetProfilesByUrl.TryGetValue(source, out values))
            {
                foreach (string v in values)
                {
                    valid.Add(v);
                }
            }

            return valid.Order().ToList();
        }
    }

    private Dictionary<
        (FhirReleases.FhirSequenceCodes source, FhirReleases.FhirSequenceCodes target),
        PackagePairStructureMappingTracker> _resourceReferenceLookup = [];

    private static readonly HashSet<string> _exportExclusions = [
        "Base",
        "BackboneType",
        "BackboneElement",
        "Element",
        ];

    public StructureFhirExporter(
        XVerExporter exporter,
        IDbConnection db,
        ILoggerFactory loggerFactory)
    {
        _exporter = exporter;
        _db = db;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<StructureFhirExporter>();
    }

    public void Export(XVerExportTrackingRecord tr)
    {
        // iterate over the XVer IGs
        foreach (XVerIgExportTrackingRecord igTr in tr.XVerIgs)
        {
            if (!_extensionValueTypes.ContainsKey(igTr.PackagePair.TargetFhirSequence))
            {
                DbStructureDefinition? extensionStructure = DbStructureDefinition.SelectSingle(
                    _db,
                    FhirPackageKey: igTr.PackagePair.TargetPackageKey,
                    Name: "Extension",
                    ArtifactClass: FhirArtifactClassEnum.ComplexType);

                if (extensionStructure is null)
                {
                    throw new Exception($"Extension structure definition not found in target package `{igTr.PackagePair.TargetPackage.PackageId}`");
                }

                DbElement? extValueElement = DbElement.SelectSingle(
                    _db,
                    FhirPackageKey: igTr.PackagePair.TargetPackageKey,
                    StructureKey: extensionStructure.Key,
                    Id: "Extension.value[x]");

                if (extValueElement is null)
                {
                    throw new Exception($"Extension.value[x] element not found in target package `{igTr.PackagePair.TargetPackage.PackageId}`");
                }

                _extensionValueTypes[igTr.PackagePair.TargetFhirSequence] = DbElementType.SelectList(
                    _db,
                    ElementKey: extValueElement.Key);

                _extensionValueTypeNames[igTr.PackagePair.TargetFhirSequence] = _extensionValueTypes[igTr.PackagePair.TargetFhirSequence]
                    .Select(et => et.TypeName ?? et.Literal)
                    .ToHashSet();
            }

            if (!_canonicalTargetElements.ContainsKey(igTr.PackagePair.TargetFhirSequence))
            {
                List<int> resourceKeys = DbStructureDefinition.SelectList(
                    _db,
                    FhirPackageKey: igTr.PackagePair.TargetPackageKey,
                    ArtifactClass: FhirArtifactClassEnum.Resource)
                    .Select(sd => sd.Key)
                    .ToList();

                List<DbElement> urlElements = DbElement.SelectList(
                    _db,
                    FhirPackageKey: igTr.PackagePair.TargetPackageKey,
                    Name: "url",
                    StructureKeyValues: resourceKeys);

                // remove non-root URL elements
                urlElements.RemoveAll(ed => ed.Id.Count('.') > 1);

                _canonicalTargetElements[igTr.PackagePair.TargetFhirSequence] = urlElements;
                _canonicalTargetResourceNames[igTr.PackagePair.TargetFhirSequence] = urlElements.Select(ed => ed.Id.Split('.')[0]).ToHashSet();
            }

            if (!_resourceReferenceLookup.ContainsKey(igTr.PackagePair.SequencePair))
            {
                PackagePairStructureMappingTracker structureMappingTracker = new();
                _resourceReferenceLookup[igTr.PackagePair.SequencePair] = structureMappingTracker;

                List<DbStructureOutcome> structureOutcomes = DbStructureOutcome.SelectList(
                    _db,
                    SourceFhirPackageKey: igTr.PackagePair.SourcePackageKey,
                    TargetFhirPackageKey: igTr.PackagePair.TargetPackageKey);

                foreach (DbStructureOutcome sdOutcome in structureOutcomes)
                {
                    string sdTargetName = sdOutcome.TargetName ?? "Basic";
                    string sdTargetUrl = sdOutcome.TargetCanonicalUnversioned ?? "http://hl7.org/fhir/StructureDefinition/Basic";

                    if (!structureMappingTracker.TargetStructuresByName.TryGetValue(sdOutcome.SourceName, out List<string>? sdNameTargets))
                    {
                        sdNameTargets = [];
                        structureMappingTracker.TargetStructuresByName[sdOutcome.SourceName] = sdNameTargets;
                    }

                    if (!sdNameTargets.Contains(sdTargetName))
                    {
                        sdNameTargets.Add(sdTargetName);
                    }

                    if (!structureMappingTracker.TargetStructuresByUrl.TryGetValue(sdOutcome.SourceCanonicalUnversioned, out List<string>? sdUrlTargets))
                    {
                        sdUrlTargets = [];
                        structureMappingTracker.TargetStructuresByName[sdOutcome.SourceCanonicalUnversioned] = sdUrlTargets;
                    }

                    if (!sdUrlTargets.Contains(sdTargetUrl))
                    {
                        sdUrlTargets.Add(sdTargetUrl);
                    }

                    string profileTarget = sdOutcome.GenUrl!;
                    if (!structureMappingTracker.TargetProfilesByName.TryGetValue(sdOutcome.SourceName, out List<string>? profileNameTargets))
                    {
                        profileNameTargets = [];
                        structureMappingTracker.TargetProfilesByName[sdOutcome.SourceName] = profileNameTargets;
                    }

                    if (!profileNameTargets.Contains(profileTarget))
                    {
                        profileNameTargets.Add(profileTarget);
                    }

                    if (!structureMappingTracker.TargetProfilesByUrl.TryGetValue(sdOutcome.SourceCanonicalUnversioned, out List<string>? profileUrlTargets))
                    {
                        profileUrlTargets = [];
                        structureMappingTracker.TargetProfilesByName[sdOutcome.SourceCanonicalUnversioned] = profileUrlTargets;
                    }

                    if (!profileUrlTargets.Contains(profileTarget))
                    {
                        profileUrlTargets.Add(profileTarget);
                    }
                }
            }

            // export extensions
            exportExtensions(igTr);

            // export profiles
            exportProfiles(igTr);

            // TODO: decide if we are exporting type maps 

            // export resource maps
            exportResourceMaps(igTr);

            // export element maps
            exportElementMaps(igTr);
        }
    }

    private void exportElementMaps(XVerIgExportTrackingRecord igTr)
    {
        if (igTr.ElementMapDir is null)
        {
            throw new Exception("ElementMapDir is null");
        }

        string dir = igTr.ElementMapDir;
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        _logger.LogInformation($"Writing element maps for `{igTr.PackageId}`...");

        List<XVerIgFileRecord> exported = [];

        // get the structure outcomes for this pair
        List<DbStructureOutcome> sdOutcomes = DbStructureOutcome.SelectList(
            _db,
            SourceFhirPackageKey: igTr.PackagePair.SourcePackageKey,
            TargetFhirPackageKey: igTr.PackagePair.TargetPackageKey);

        // iterate over the outcomes to create element maps for each resource
        foreach (DbStructureOutcome sdOutcome in sdOutcomes)
        {
            // create a concept map for this resource
            ConceptMap edCm = createElementConceptMap(igTr, sdOutcome);

            // get the element outcomes for this structure outcome
            List<DbElementOutcome> edOutcomes = DbElementOutcome.SelectList(
                _db,
                SourceFhirPackageKey: igTr.PackagePair.SourcePackageKey,
                SourceStructureKey: sdOutcome.SourceStructureKey,
                orderByProperties: [nameof(DbElementOutcome.SourceResourceOrder)]);

            //string? lastSourceId = null;
            //ConceptMap.SourceElementComponent? cmSourceElement = null;

            Dictionary<string, (ConceptMap.SourceElementComponent se, HashSet<string> usedTargets)> sourceElementLookup = [];

            Dictionary<int, string> outcomeUrlComposition = [];

            // iterate over the element outcomes
            foreach (DbElementOutcome edOutcome in edOutcomes)
            {
                if (!sourceElementLookup.TryGetValue(edOutcome.SourceId, out (ConceptMap.SourceElementComponent se, HashSet<string> usedTargets) cmSourceInfo))
                {
                    // create a new source element
                    cmSourceInfo.se = new()
                    {
                        Code = edOutcome.SourceId,
                        Display = edOutcome.SourceName,
                        Target = [],
                    };
                    cmSourceInfo.usedTargets = [];
                    edCm.Group[0].Element.Add(cmSourceInfo.se);
                    sourceElementLookup[edOutcome.SourceId] = cmSourceInfo;
                }

                CMR relationship;
                if (sdOutcome.IsIdentical || sdOutcome.IsEquivalent)
                {
                    relationship = CMR.Equivalent;
                }
                else if (sdOutcome.IsBroaderThanTarget)
                {
                    relationship = CMR.SourceIsBroaderThanTarget;
                }
                else if (sdOutcome.IsNarrowerThanTarget)
                {
                    relationship = CMR.SourceIsNarrowerThanTarget;
                }
                else
                {
                    relationship = CMR.RelatedTo;
                }

                // get the list of targets for this element outcome
                List<DbElementOutcomeTarget> outcomeTargets = DbElementOutcomeTarget.SelectList(
                    _db,
                    ElementOutcomeKey: edOutcome.Key,
                    orderByProperties: [nameof(DbElementOutcomeTarget.TargetElementId)]);

                List<DbElementOutcomeTarget> outcomeTargetsWithKeys = outcomeTargets
                    .Where(ot => ot.TargetElementKey is not null)
                    .ToList();

                if (outcomeTargetsWithKeys.Count == 0)
                {
                    string code;
                    if (edOutcome.BasicElementEquivalent is not null)
                    {
                        string[] components = edOutcome.BasicElementEquivalent.Split('.');
                        code = string.Join('.', ["Basic", .. components[1..]]);
                    }
                    else if ((edOutcome.ParentRequiresXverDefinition == true) &&
                        (edOutcome.ParentElementOutcomeKey is not null) &&
                        outcomeUrlComposition.TryGetValue(edOutcome.ParentElementOutcomeKey.Value, out string? parentUrl))
                    {
                        code = parentUrl + ":" + edOutcome.GenUrl!;
                    }
                    else
                    {
                        code = edOutcome.ExtensionSubstitutionUrl ?? edOutcome.GenUrl!;
                    }

                    outcomeUrlComposition[edOutcome.Key] = code;

                    if (!cmSourceInfo.usedTargets.Add(code))
                    {
                        continue;
                    }

                    // create our target element
                    ConceptMap.TargetElementComponent targetElement = new()
                    {
                        Code = code,
                        Display = edOutcome.TargetName,
                        Relationship = relationship,
                        Comment = edOutcome.Comments,
                    };
                    cmSourceInfo.se.Target.Add(targetElement);
                }
                else
                {
                    // create a target for each target element
                    foreach (DbElementOutcomeTarget eot in outcomeTargetsWithKeys)
                    {
                        if ((eot.TargetElementId is null) ||
                            !cmSourceInfo.usedTargets.Add(eot.TargetElementId))
                        {
                            continue;
                        }

                        // create our target element
                        ConceptMap.TargetElementComponent targetElement = new()
                        {
                            Code = sdOutcome.TargetCanonicalUnversioned + "#" + eot.TargetElementId,
                            Display = edOutcome.TargetName,
                            Relationship = relationship,
                            Comment = edOutcome.Comments,
                        };
                        cmSourceInfo.se.Target.Add(targetElement);
                    }
                }
            }

            // write the profile to a file
            string filename = sdOutcome.ElementConceptMapFileName ?? throw new ArgumentNullException(nameof(sdOutcome.ElementConceptMapFileName));
            string path = Path.Combine(dir, filename);
            File.WriteAllText(path, edCm.ToJson(new FhirJsonSerializationSettings() { Pretty = true }));
            exported.Add(new()
            {
                FileName = filename,
                FileNameWithoutExtension = filename[..^5],
                IsPageContentFile = false,
                Name = edCm.Name,
                Id = edCm.Id,
                Url = edCm.Url,
                ResourceType = Hl7.Fhir.Model.FHIRAllTypes.ConceptMap.GetLiteral(),
                Version = edCm.Version,
                Description = edCm.Description ?? edCm.Title ?? $"ConceptMap: {edCm.Url}",
            });
        }

        _logger.LogInformation($"Wrote {exported.Count} element maps for `{igTr.PackageId}`");
        igTr.ElementMapFiles = exported;
    }

    private ConceptMap createElementConceptMap(
        XVerIgExportTrackingRecord igTr,
        DbStructureOutcome sdOutcome)
    {
        string targetId = sdOutcome.TargetId ?? "Basic";

        (_, string name) = igTr.GetName(sdOutcome.ElementConceptMapName!, sdOutcome.ElementConceptMapLongId!);

        ConceptMap vsCm = new()
        {
            Id = sdOutcome.ElementConceptMapLongId,
            Url = sdOutcome.ElementConceptMapUrl,
            Name = name,
            Version = _exporter._crossDefinitionVersion,
            DateElement = new FhirDateTime(DateTimeOffset.Now),
            Title = $"Cross-version ConceptMap for FHIR {igTr.PackagePair.SourceFhirSequence} resources in FHIR {igTr.PackagePair.TargetFhirSequence}",
            Description = $"This ConceptMap represents the cross-version mapping of resource FHIR {igTr.PackagePair.SourceFhirSequence} for use in FHIR {igTr.PackagePair.TargetFhirSequence}.",
            Status = PublicationStatus.Active,
            Experimental = false,
            SourceScope = new FhirUri($"http://hl7.org/fhir/{igTr.PackagePair.SourceFhirVersionShort}/ValueSet/resource-types"),
            TargetScope = new FhirUri($"http://hl7.org/fhir/{igTr.PackagePair.TargetFhirVersionShort}/ValueSet/resource-types"),
            Group = [
                new()
                {
                    Source = sdOutcome.SourceCanonicalVersioned,
                    Element = [],
                }
            ],
        };

        return vsCm;
    }

    private void exportResourceMaps(XVerIgExportTrackingRecord igTr)
    {
        if (igTr.ResourceMapDir is null)
        {
            throw new Exception("ResourceMapDir is null");
        }

        string dir = igTr.ResourceMapDir;
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        _logger.LogInformation($"Writing resource maps for `{igTr.PackageId}`...");

        List<XVerIgFileRecord> exported = [];

        ConceptMap cm = createResourceConceptMap(igTr);

        // get the structure outcomes for this pair
        List<DbStructureOutcome> sdOutcomes = DbStructureOutcome.SelectList(
            _db,
            SourceFhirPackageKey: igTr.PackagePair.SourcePackageKey,
            TargetFhirPackageKey: igTr.PackagePair.TargetPackageKey,
            orderByProperties: [nameof(DbStructureOutcome.SourceName), nameof(DbStructureOutcome.TargetName)]);

        string? lastSourceId = null;

        ConceptMap.SourceElementComponent? currentSourceElement = null;

        // iterate over the outcomes
        foreach (DbStructureOutcome sdOutcome in sdOutcomes)
        {
            // check if we need a new source element
            if ((currentSourceElement is null) ||
                (lastSourceId != sdOutcome.SourceId))
            {
                // create a new source element
                currentSourceElement = new()
                {
                    Code = sdOutcome.SourceId,
                    Display = sdOutcome.SourceName,
                    Target = [],
                };
                cm.Group[0].Element.Add(currentSourceElement);
                lastSourceId = sdOutcome.SourceId;
            }

            CMR relationship;
            if (sdOutcome.IsIdentical || sdOutcome.IsEquivalent)
            {
                relationship = CMR.Equivalent;
            }
            else if (sdOutcome.IsBroaderThanTarget)
            {
                relationship = CMR.SourceIsBroaderThanTarget;
            }
            else if (sdOutcome.IsNarrowerThanTarget)
            {
                relationship = CMR.SourceIsNarrowerThanTarget;
            }
            else
            {
                relationship = CMR.RelatedTo;
            }

            // create our target element
            ConceptMap.TargetElementComponent targetElement = new()
            {
                Code = sdOutcome.TargetId ?? "Basic",
                Display = sdOutcome.TargetName ?? "Basic",
                Relationship = relationship,
                Comment = sdOutcome.Comments,
            };

            currentSourceElement.Target.Add(targetElement);
        }

        // write the resource to a file
        string filename = cm.Id.StartsWith("ConceptMap", StringComparison.OrdinalIgnoreCase)
            ? $"{cm.Id}.json"
            : $"ConceptMap-{cm.Id}.json";
        string path = Path.Combine(dir, filename);
        File.WriteAllText(path, cm.ToJson(new FhirJsonSerializationSettings() { Pretty = true }));
        exported.Add(new()
        {
            FileName = filename,
            FileNameWithoutExtension = filename[..^5],
            IsPageContentFile = false,
            Name = cm.Name,
            Id = cm.Id,
            Url = cm.Url,
            ResourceType = Hl7.Fhir.Model.FHIRAllTypes.ConceptMap.GetLiteral(),
            Version = cm.Version,
            Description = cm.Description ?? cm.Title ?? $"ConceptMap: {cm.Url}",
        });

        _logger.LogInformation($"Wrote {exported.Count} resource maps for `{igTr.PackageId}`");
        igTr.ResourceMapFiles = exported;
    }

    private ConceptMap createResourceConceptMap(
        XVerIgExportTrackingRecord igTr)
    {
        string id = $"ConceptMap-{igTr.PackagePair.SourcePackageShortName}-resources-for-{igTr.PackagePair.TargetPackageShortName}";
        string name =
            $"ConceptMap" +
            $"{igTr.PackagePair.SourcePackageShortName.ToPascalCase()}" +
            $"ResourcesFor" +
            $"{igTr.PackagePair.TargetPackageShortName.ToPascalCase()}";

        (_, name) = igTr.GetName(name, id);

        ConceptMap vsCm = new()
        {
            Id = id,
            Url = $"http://hl7.org/fhir/{igTr.PackagePair.SourceFhirVersionShort}/ConceptMap/{id}",
            Name = name,
            Version = _exporter._crossDefinitionVersion,
            DateElement = new FhirDateTime(DateTimeOffset.Now),
            Title = $"Cross-version ConceptMap for FHIR {igTr.PackagePair.SourceFhirSequence} resources in FHIR {igTr.PackagePair.TargetFhirSequence}",
            Description = $"This ConceptMap represents the cross-version mapping of resource FHIR {igTr.PackagePair.SourceFhirSequence} for use in FHIR {igTr.PackagePair.TargetFhirSequence}.",
            Status = PublicationStatus.Active,
            Experimental = false,
            SourceScope = new FhirUri($"http://hl7.org/fhir/{igTr.PackagePair.SourceFhirVersionShort}/ValueSet/resource-types"),
            TargetScope = new FhirUri($"http://hl7.org/fhir/{igTr.PackagePair.TargetFhirVersionShort}/ValueSet/resource-types"),
            Group = [
                new()
                {
                    Source = $"http://hl7.org/fhir/{igTr.PackagePair.SourceFhirVersionShort}/resource-types",
                    Target = $"http://hl7.org/fhir/{igTr.PackagePair.TargetFhirVersionShort}/resource-types",
                    Element = [],
                }
            ],
        };

        return vsCm;
    }


    private void exportProfiles(XVerIgExportTrackingRecord igTr)
    {
        if (igTr.ProfileDir is null)
        {
            throw new Exception("ProfileDir is null");
        }

        string dir = igTr.ProfileDir;
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        _logger.LogInformation($"Writing Profiles for `{igTr.PackageId}`...");

        List<XVerIgFileRecord> exported = [];

        // get the structures defined in the source package
        List<DbStructureDefinition> sourceStructures = DbStructureDefinition.SelectList(
            _db,
            FhirPackageKey: igTr.PackagePair.SourcePackageKey,
            ArtifactClass: FhirArtifactClassEnum.Resource);

        // iterate over all structures - each will get a profile
        foreach (DbStructureDefinition sourceSd in sourceStructures)
        {
            // get the structure outcomes for this source structure
            List<DbStructureOutcome> sdOutcomes = DbStructureOutcome.SelectList(
                _db,
                SourceFhirPackageKey: igTr.PackagePair.SourcePackageKey,
                TargetFhirPackageKey: igTr.PackagePair.TargetPackageKey,
                SourceStructureKey: sourceSd.Key);

            // iterate over the outcomes and create profiles for each target
            foreach (DbStructureOutcome sdOutcome in sdOutcomes)
            {
                // resolve the target structure (if any)
                DbStructureDefinition? targetSd = sdOutcome.TargetStructureKey is null
                    ? null
                    : DbStructureDefinition.SelectSingle(
                        _db,
                        FhirPackageKey: igTr.PackagePair.TargetPackageKey,
                        Key: sdOutcome.TargetStructureKey.Value);

                // build the initial structure definition for the extension
                StructureDefinition profileSd = createProfileSd(
                    igTr,
                    sourceSd,
                    targetSd,
                    sdOutcome);

                if ((targetSd is null) ||
                    ((targetSd.Name == "Basic") && (sourceSd.Name != "Basic")))
                {
                    DbElementOutcome? rootElementOutcome = DbElementOutcome.SelectSingle(
                            _db,
                            SourceFhirPackageKey: igTr.PackagePair.SourcePackageKey,
                            TargetFhirPackageKey: igTr.PackagePair.TargetPackageKey,
                            SourceStructureKey: sdOutcome.SourceStructureKey,
                            SourceResourceOrder: 0);

                    if (rootElementOutcome is null)
                    {
                        throw new Exception($"First element outcome for source structure `{sourceSd.Name}` in package pair `{igTr.PackageId}` is not the root element");
                    }

                    // if this is a basic resource profile, it only needs the root extension
                    addContentForBasicProfile(
                        igTr,
                        sourceSd,
                        sdOutcome,
                        rootElementOutcome,
                        profileSd);
                }
                else
                {
                    // add the content for a mapped resource
                    addContentForMappedProfile(
                        igTr,
                        sourceSd,
                        targetSd,
                        sdOutcome,
                        profileSd);
                }

                // write the profile to a file
                string filename = sdOutcome.GenFileName ?? throw new ArgumentNullException(nameof(sdOutcome.GenFileName));
                string path = Path.Combine(dir, filename);
                File.WriteAllText(path, profileSd.ToJson(new FhirJsonSerializationSettings() { Pretty = true }));
                exported.Add(new()
                {
                    FileName = filename,
                    FileNameWithoutExtension = filename[..^5],
                    IsPageContentFile = false,
                    Name = profileSd.Name,
                    Id = profileSd.Id,
                    Url = profileSd.Url,
                    ResourceType = Hl7.Fhir.Model.FHIRAllTypes.StructureDefinition.GetLiteral(),
                    Version = profileSd.Version,
                    Description = profileSd.Description ?? profileSd.Title ?? $"Profile: {profileSd.Url}",
                });
            }
        }

        _logger.LogInformation($"Wrote {exported.Count} Profiles for `{igTr.PackageId}`");
        igTr.ProfileFiles = exported;
    }

    private void addContentForMappedProfile(
        XVerIgExportTrackingRecord igTr,
        DbStructureDefinition sourceSd,
        DbStructureDefinition targetSd,
        DbStructureOutcome sdOutcome,
        StructureDefinition profileSd)
    {
        // get the element outcomes that require a definition and are not part of one already
        List<DbElementOutcome> edOutcomes = DbElementOutcome.SelectList(
            _db,
            SourceFhirPackageKey: igTr.PackagePair.SourcePackageKey,
            TargetFhirPackageKey: igTr.PackagePair.TargetPackageKey,
            SourceStructureKey: sdOutcome.SourceStructureKey,
            RequiresXVerDefinition: true,
            ParentRequiresXverDefinition: false);
            //ParentElementOutcomeKeyIsNull: true);

        // build a lookup based on context paths
        ILookup<string, DbElementOutcome> edOutcomeContextLookup = edOutcomes
            .SelectMany(edo => edo.ExtensionContexts, (edo, context) => new { Context = context, Outcome = edo })
            .ToLookup(x => x.Context, x => x.Outcome);

        // we need to traverse the elements in the order of the target structure
        List<DbElement> targetElements = DbElement.SelectList(
            _db,
            StructureKey: targetSd.Key,
            orderByProperties: [nameof(DbElement.ResourceFieldOrder)]);

        if (targetElements.Count == 0)
        {
            throw new Exception($"Resource with no elements!");
        }

        DbElement targetRootEd = targetElements[0];
        ILookup<string, DbElement> targetIdLookup = targetElements.ToLookup(ed => ed.Id);

        // iterate over the elements and add to the differential as necessary
        foreach (DbElement targetEd in targetElements)
        {
            List<DbElementOutcome> targetEdOutcomes = [];
            if (!edOutcomeContextLookup.Contains(targetEd.Id))
            {
                continue;
            }

            List<DbElementOutcome> existingOutcomes = edOutcomeContextLookup[targetEd.Id].ToList();
            foreach (DbElementOutcome existingOutcome in existingOutcomes)
            {
                if ((existingOutcome.RequiresXVerDefinition == true) &&
                    (existingOutcome.ParentRequiresXverDefinition != true))
                {
                    targetEdOutcomes.Add(existingOutcome);
                    continue;
                }

                if ((existingOutcome.RequiresComponentDefinition == true) &&
                    (existingOutcome.ParentRequiresComponentDefinition != true))
                {
                    targetEdOutcomes.Add(existingOutcome);
                    continue;
                }
            }

            // if there are none, move on
            if (targetEdOutcomes.Count == 0)
            {
                continue;
            }

            // build our target information
            string targetId = targetEd.Id + ".extension";
            string targetPath = targetEd.Path + ".extension";
            DbElement actualTargetEd = targetEd;

            // check to see if we need to swap targets
            if (targetIdLookup.Contains(targetId))
            {
                actualTargetEd = targetIdLookup[targetId].First();
            }

            // add the base for this element we need for slicing
            ElementDefinition targetSlicingEd = new()
            {
                ElementId = targetId,
                Path = targetPath,
                Slicing = new()
                {
                    Discriminator = [
                        new ElementDefinition.DiscriminatorComponent()
                        {
                            Type = ElementDefinition.DiscriminatorType.Value,
                            Path = "url",
                        }
                    ],
                    Ordered = false,
                    Rules = ElementDefinition.SlicingRules.Open,
                },
                Base = new ElementDefinition.BaseComponent()
                {
                    Path = actualTargetEd.BasePath ?? "DomainResource.extension",
                    Min = 0,
                    Max = "*",
                },
                Min = 1,
                Max = "*",
            };

            profileSd.Differential.Element.Add(targetSlicingEd);

            // add each outcome that we should have here
            foreach (DbElementOutcome targetEdOutcome in targetEdOutcomes)
            {
                string url;

                if (targetEdOutcome.ExtensionSubstitutionUrl is not null)
                {
                    url = targetEdOutcome.ExtensionSubstitutionUrl;
                }
                else if (targetEdOutcome.RequiresXVerDefinition &&
                    (targetEdOutcome.GenUrl is not null) &&
                    targetEdOutcome.GenUrl.StartsWith("http:", StringComparison.Ordinal))
                {
                    url = targetEdOutcome.GenUrl;
                }
                else if (targetEdOutcome.RequiresComponentDefinition &&
                    (targetEdOutcome.ComponentGenUrl is not null) &&
                    targetEdOutcome.ComponentGenUrl.StartsWith("http:", StringComparison.Ordinal))
                {
                    url = targetEdOutcome.ComponentGenUrl;
                }
                else
                {
                    continue;
                }

                ElementDefinition extEd = new()
                {
                    ElementId = $"{targetId}:{targetEdOutcome.SourceNameClean()}",
                    Path = targetPath,
                    Short = $"Cross-version extension for {targetEdOutcome.SourceId} from {igTr.PackagePair.SourceFhirSequence} for use in FHIR {igTr.PackagePair.TargetFhirSequence}",
                    Min = targetEdOutcome.SourceMinCardinality,
                    Max = targetEdOutcome.SourceMaxCardinalityString,
                    Base = new ElementDefinition.BaseComponent()
                    {
                        Path = "DomainResource.extension",
                        Min = 0,
                        Max = "*",
                    },
                    Type = [
                        new ElementDefinition.TypeRefComponent()
                        {
                            Code = "Extension",
                            Profile = [ url ],
                        },
                    ],
                };

                profileSd.Differential.Element.Add(extEd);
            }
        }
    }

    private void addContentForBasicProfile(
        XVerIgExportTrackingRecord igTr,
        DbStructureDefinition sourceSd,
        DbStructureOutcome sdOutcome,
        DbElementOutcome rootElementOutcome,
        StructureDefinition profileSd)
    {
        profileSd.Differential.Element = [
            new ElementDefinition()
            {
                ElementId = "Basic.extension",
                Path = "Basic.extension",
                Slicing = new()
                {
                    Discriminator = [
                        new ElementDefinition.DiscriminatorComponent()
                        {
                            Type = ElementDefinition.DiscriminatorType.Value,
                            Path = "url",
                        }
                    ],
                    Ordered = false,
                    Rules = ElementDefinition.SlicingRules.Open,
                },
                Base = new ElementDefinition.BaseComponent()
                {
                    Path = "DomainResource.extension",
                    Min = 0,
                    Max = "*",
                },
                Min = 1,
                Max = "*",
            },
            new ElementDefinition()
            {
                ElementId = $"Basic.extension:{sourceSd.Id}",
                Path = "Basic.extension",
                SliceName = sourceSd.Id,
                Short = $"Cross-version extension for {sourceSd.Name} from {igTr.PackagePair.SourceFhirSequence} for use in FHIR {igTr.PackagePair.TargetFhirSequence}",
                Min = 1,
                Max = "1",
                Base = new ElementDefinition.BaseComponent()
                {
                    Path = "DomainResource.extension",
                    Min = 0,
                    Max = "*",
                },
                Type = [
                    new ElementDefinition.TypeRefComponent()
                    {
                        Code = "Extension",
                        Profile = [ rootElementOutcome.GenUrl ],
                    },
                ],
            },
            new ElementDefinition()
            {
                ElementId = "Basic.code",
                Path = "Basic.code",
                Pattern = new CodeableConcept("http://hl7.org/fhir/fhir-types", sourceSd.Id),
                Base = new ElementDefinition.BaseComponent()
                {
                    Path = "Basic.code",
                    Min = 1,
                    Max = "*",
                }
            },
        ];
    }

    private StructureDefinition createProfileSd(
        XVerIgExportTrackingRecord igTr,
        DbStructureDefinition sourceSd,
        DbStructureDefinition? targetSd,
        DbStructureOutcome sdOutcome)
    {
        string targetStructureName = targetSd?.Name ?? "Basic";
        string profileId = sdOutcome.GenShortId!;

        (_, string name) = igTr.GetName(sdOutcome.GenName!, profileId);

        StructureDefinition profileSd = new()
        {
            Id = profileId,
            Url = sdOutcome.GenUrl,
            Name = name,
            Version = _exporter._crossDefinitionVersion,
            FhirVersion = EnumUtility.ParseLiteral<FHIRVersion>(igTr.PackagePair.TargetPackage.PackageVersion) ?? FHIRVersion.N5_0_0,
            DateElement = new FhirDateTime(DateTimeOffset.Now),
            Title = $"Cross-version Profile for {igTr.PackagePair.SourceFhirSequence}.{sourceSd.Name} for use in FHIR {igTr.PackagePair.TargetFhirSequence}",
            Description = $"This cross-version profile allows" +
                $" {igTr.PackagePair.SourceFhirSequence} {sourceSd.Name} content to be represented" +
                $" via FHIR {igTr.PackagePair.TargetFhirSequence} {targetStructureName} resources.",
            Status = PublicationStatus.Active,
            Experimental = false,
            Kind = StructureDefinition.StructureDefinitionKind.Resource,
            Abstract = false,
            Type = targetStructureName,
            BaseDefinition = $"http://hl7.org/fhir/StructureDefinition/{targetStructureName}",
            Derivation = StructureDefinition.TypeDerivationRule.Constraint,
            Differential = [],
        };

        string wg = CommonDefinitions.ResolveWorkgroup("fhir");

        // add the work group extension
        profileSd.AddExtension(CommonDefinitions.ExtUrlWorkGroup, new Hl7.Fhir.Model.Code(wg));

        // ensure there is a publisher, use the WG if there is none
        profileSd.Publisher = CommonDefinitions.WorkgroupNames[wg];

        // ensure there is a contact point - use the default WG unless there are multiple entries
        if ((profileSd.Contact == null) || (profileSd.Contact.Count < 2))
        {
            profileSd.Contact = [
                new()
                {
                    Name = CommonDefinitions.WorkgroupNames[wg],
                    Telecom = [
                        new()
                        {
                            System = ContactPoint.ContactPointSystem.Url,
                            Value = CommonDefinitions.WorkgroupUrls[wg],
                        },
                    ],
                }
            ];
        }

        profileSd.cgAddPackageSource(igTr.PackageId, _exporter._crossDefinitionVersion, null);

        // add the version-specific fhir version information
        profileSd.Extension.Add(new Extension()
        {
            Url = CommonDefinitions.ExtUrlVersionSpecificUse,
            Extension = [
                new()
                {
                    Url = CommonDefinitions.ExtUrlVersionSpecificUseStart,
                    Value = new Code(igTr.PackagePair.TargetFhirVersionShort),
                },
                new()
                {
                    Url = CommonDefinitions.ExtUrlVersionSpecificUseEnd,
                    Value = new Code(igTr.PackagePair.TargetFhirVersionShort),
                },
            ],
        });

        return profileSd;
    }


    private void exportExtensions(XVerIgExportTrackingRecord igTr)
    {
        if (igTr.ExtensionDir is null)
        {
            throw new Exception("ExtensionDir is null");
        }

        string dir = igTr.ExtensionDir;
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        _logger.LogInformation($"Writing extensions for `{igTr.PackageId}`...");

        List<XVerIgFileRecord> exported = [];

        // get the structures defined in the source package
        Dictionary<int, DbStructureDefinition> sourceSds = DbStructureDefinition.SelectDict(
            _db,
            FhirPackageKey: igTr.PackagePair.SourcePackageKey);

        // get the elements defined in the source package
        Dictionary<int, DbElement> sourceEds = DbElement.SelectDict(
            _db,
            FhirPackageKey: igTr.PackagePair.SourcePackageKey);

        // get the structures defined in the target package
        Dictionary<int, DbStructureDefinition> targetSds = DbStructureDefinition.SelectDict(
            _db,
            FhirPackageKey: igTr.PackagePair.TargetPackageKey);

        // get the elements defined in the target package
        Dictionary<int, DbElement> targetEds = DbElement.SelectDict(
            _db,
            FhirPackageKey: igTr.PackagePair.TargetPackageKey);

        // get the element comparisons for the package pair
        Dictionary<int, DbElementComparison> edComparisons = DbElementComparison.SelectDict(
            _db,
            SourceFhirPackageKey: igTr.PackagePair.SourcePackageKey,
            TargetFhirPackageKey: igTr.PackagePair.TargetPackageKey);

        Dictionary<int, string> contentReferenceExtUrlsByEdKey = findContentReferenceUrls(
            igTr.PackagePair.SourcePackageKey,
            igTr.PackagePair.TargetPackageKey);

        // get the element outcomes that need component-style exporting
        List<DbElementOutcome> componentEdOutcomes = DbElementOutcome.SelectList(
            _db,
            SourceFhirPackageKey: igTr.PackagePair.SourcePackageKey,
            TargetFhirPackageKey: igTr.PackagePair.TargetPackageKey,
            RequiresComponentDefinition: true,
            ParentRequiresComponentDefinition: false,
            RequiresXVerDefinition: false,
            ExtensionSubstitutionKeyIsNull: true,
            orderByProperties: [nameof(DbElementOutcome.SourceStructureKey), nameof(DbElementOutcome.SourceResourceOrder)]);

        HashSet<string> generatedExtensionIds = [];

        // iterate over the outcomes that need exporting
        foreach (DbElementOutcome edOutcome in componentEdOutcomes)
        {
            // skip elements that will match their normal definition
            if ((edOutcome.RequiresXVerDefinition == true) &&
                (edOutcome.ComponentGenLongId == edOutcome.GenLongId))
            {
                continue;
            }

            // get the source structure
            if (!sourceSds.TryGetValue(edOutcome.SourceStructureKey, out DbStructureDefinition? sourceSd))
            {
                _logger.LogError($"Source structure with key `{edOutcome.SourceStructureKey}` not found for element outcome with key `{edOutcome.Key}`");
                continue;
            }

            if (_exportExclusions.Contains(edOutcome.SourceId) ||
                _exportExclusions.Contains(sourceSd.Name))
            {
                continue;
            }

            // get the source element
            if (!sourceEds.TryGetValue(edOutcome.SourceElementKey, out DbElement? sourceEd))
            {
                _logger.LogError($"Source element with key `{edOutcome.SourceElementKey}` not found for element outcome with key `{edOutcome.Key}`");
                continue;
            }

            if (skipElement(sourceEd, skipFirstElement: true))
            {
                continue;
            }

            // components can only be on Extension.extension
            List<StructureDefinition.ContextComponent> contexts = [
                new StructureDefinition.ContextComponent()
                {
                    Type = StructureDefinition.ExtensionContextType.Element,
                    Expression = "Extension.extension",
                }];

            string purpose = buildPurpose(
                igTr,
                edComparisons,
                sourceEd,
                edOutcome);

            StructureDefinition? extSd = buildExtSd(
                generatedExtensionIds,
                igTr,
                sourceEds,
                targetEds,
                edComparisons,
                contentReferenceExtUrlsByEdKey,
                edOutcome,
                sourceSd,
                sourceEd,
                purpose,
                contexts,
                useComponentDefinition: true);

            if (extSd is null)
            {
                continue;
            }

            //contentReferenceExtUrlsByEdKey[sourceEd.Key] = extSd.Url;

            // write the extension to a file
            string filename = edOutcome.ComponentGenFileName ?? throw new ArgumentNullException(nameof(edOutcome.ComponentGenFileName));
            string path = Path.Combine(dir, filename);
            File.WriteAllText(path, extSd.ToJson(new FhirJsonSerializationSettings() { Pretty = true }));

            exported.Add(new()
            {
                FileName = filename,
                FileNameWithoutExtension = filename[..^5],
                IsPageContentFile = false,
                Name = extSd.Name,
                Id = extSd.Id,
                Url = extSd.Url,
                ResourceType = Hl7.Fhir.Model.FHIRAllTypes.StructureDefinition.GetLiteral(),
                Version = extSd.Version,
                Description = extSd.Description ?? extSd.Title ?? $"Extension: {extSd.Url}",
            });
        }

        // get the element outcomes for this package pair that need exporting
        List<DbElementOutcome> edOutcomes = DbElementOutcome.SelectList(
            _db,
            SourceFhirPackageKey: igTr.PackagePair.SourcePackageKey,
            TargetFhirPackageKey: igTr.PackagePair.TargetPackageKey,
            RequiresXVerDefinition: true,
            ExtensionSubstitutionKeyIsNull: true,
            ParentRequiresXverDefinition: false,
            //ParentElementOutcomeKeyIsNull: true,
            orderByProperties: [nameof(DbElementOutcome.SourceStructureKey), nameof(DbElementOutcome.SourceResourceOrder)]);

        // iterate over the outcomes that need exporting
        foreach (DbElementOutcome edOutcome in edOutcomes)
        {
            // get the source structure
            if (!sourceSds.TryGetValue(edOutcome.SourceStructureKey, out DbStructureDefinition? sourceSd))
            {
                _logger.LogError($"Source structure with key `{edOutcome.SourceStructureKey}` not found for element outcome with key `{edOutcome.Key}`");
                continue;
            }

            if (_exportExclusions.Contains(edOutcome.SourceId) ||
                _exportExclusions.Contains(sourceSd.Name))
            {
                continue;
            }

            //DbStructureOutcome? sdOutcome = null;
            //if (edOutcome.SourceResourceOrder == 0)
            //{
            //    sdOutcome = DbStructureOutcome.SelectSingle(
            //        _db,
            //        SourceFhirPackageKey: igTr.PackagePair.SourcePackageKey,
            //        TargetFhirPackageKey: igTr.PackagePair.TargetPackageKey,
            //        SourceStructureKey: sourceSd.Key,
            //        TargetStructureKey: edOutcome.TargetStructureKey);
            //}

            // get the source element
            if (!sourceEds.TryGetValue(edOutcome.SourceElementKey, out DbElement? sourceEd))
            {
                _logger.LogError($"Source element with key `{edOutcome.SourceElementKey}` not found for element outcome with key `{edOutcome.Key}`");
                continue;

            }

            if (skipElement(sourceEd, skipFirstElement: false))
            {
                continue;
            }

            List<StructureDefinition.ContextComponent> contexts = edOutcome.ExtensionContexts
                .Select(c => new StructureDefinition.ContextComponent()
                {
                    Type = StructureDefinition.ExtensionContextType.Element,
                    Expression = c,
                })
                .ToList();

            string purpose = buildPurpose(
                igTr,
                edComparisons,
                sourceEd,
                edOutcome);

            StructureDefinition? extSd = buildExtSd(
                generatedExtensionIds,
                igTr,
                sourceEds,
                targetEds,
                edComparisons,
                contentReferenceExtUrlsByEdKey,
                edOutcome,
                sourceSd,
                sourceEd,
                purpose,
                contexts,
                useComponentDefinition: false);

            if (extSd is null)
            {
                continue;
            }

            // write the extension to a file
            string filename = edOutcome.GenFileName ?? throw new ArgumentNullException(nameof(edOutcome.GenFileName));
            string path = Path.Combine(dir, filename);
            File.WriteAllText(path, extSd.ToJson(new FhirJsonSerializationSettings() { Pretty = true }));

            exported.Add(new()
            {
                FileName = filename,
                FileNameWithoutExtension = filename[..^5],
                IsPageContentFile = false,
                Name = extSd.Name,
                Id = extSd.Id,
                Url = extSd.Url,
                ResourceType = Hl7.Fhir.Model.FHIRAllTypes.StructureDefinition.GetLiteral(),
                Version = extSd.Version,
                Description = extSd.Description ?? extSd.Title ?? $"Extension: {extSd.Url}",
            });
        }

        _logger.LogInformation($"Wrote {exported.Count} Extensions for `{igTr.PackageId}`");
        igTr.ExtensionFiles = exported;
    }

    private Dictionary<int, string> findContentReferenceUrls(
        int sourceFhirPackageKey,
        int targetFhirPackageKey)
    {
        Dictionary<int, string> dict = [];

        List<DbElementOutcome> xverCrEdOutcomes = DbElementOutcome.SelectList(
            _db,
            SourceFhirPackageKey: sourceFhirPackageKey,
            TargetFhirPackageKey: targetFhirPackageKey,
            RequiresXVerDefinition: true,
            SourceUsedAsContentReference: true);
        foreach (DbElementOutcome eo in xverCrEdOutcomes)
        {
            if (eo.GenUrl is not null)
            {
                dict[eo.SourceElementKey] = eo.GenUrl;
            }
        }

        List<DbElementOutcome> componentCrEdOutcomes = DbElementOutcome.SelectList(
            _db,
            SourceFhirPackageKey: sourceFhirPackageKey,
            TargetFhirPackageKey: targetFhirPackageKey,
            RequiresComponentDefinition: true,
            RequiresXVerDefinition: false,
            SourceUsedAsContentReference: true);
        foreach (DbElementOutcome ceo in componentCrEdOutcomes)
        {
            if (ceo.ComponentGenUrl is not null)
            {
                dict[ceo.SourceElementKey] = ceo.ComponentGenUrl;
            }
        }

        return dict;
    }

    private string buildPurpose(
        XVerIgExportTrackingRecord igTr,
        Dictionary<int, DbElementComparison> elementComparisons,
        DbElement sourceEd,
        DbElementOutcome elementOutcome)
    {
        if (igTr.PackagePair.Distance == 1)
        {
            return $$$"""
                    This extension is part of the cross-version definitions generated to enable use of the
                    element `{{{sourceEd.Id}}}` as defined in FHIR {{{igTr.PackagePair.SourceFhirSequence}}}
                    in FHIR {{{igTr.PackagePair.TargetFhirSequence}}}.

                    The source element is defined as:
                    `{{{sourceEd.Id}}}` {{{sourceEd.FhirCardinalityString}}} `{{{sourceEd.FullCollatedTypeLiteral}}}`

                    Following are the generation technical comments:
                    {{{elementOutcome.Comments}}}
                    """;
        }

        string? mappingTrace = null;
        List<string> mappingTraceLines = [];

        List<DbElementOutcomeTarget> eots = DbElementOutcomeTarget.SelectList(
            _db,
            ElementOutcomeKey: elementOutcome.Key);

        List<int> elementComparisonKeys = eots
            .Where(eot => eot.ElementComparisonKey is not null)
            .Select(eot => eot.ElementComparisonKey!.Value)
            .ToList();

        List<DbElementComparison> possibleComparisons = elementComparisonKeys.Count == 0
            ? []
            : DbElementComparison.SelectList(
                _db,
                KeyValues: elementComparisonKeys);

        foreach (DbElementComparison edComp in possibleComparisons)
        {
            DbElement? r2Ed = edComp.ContentKeyR2 is null
                ? null
                : DbElement.SelectSingle(_db, Key: edComp.ContentKeyR2.Value);
            DbElement? r3Ed = edComp.ContentKeyR3 is null
                ? null
                : DbElement.SelectSingle(_db, Key: edComp.ContentKeyR3.Value);
            DbElement? r4Ed = edComp.ContentKeyR4 is null
                ? null
                : DbElement.SelectSingle(_db, Key: edComp.ContentKeyR4.Value);
            DbElement? r4bEd = edComp.ContentKeyR4B is null
                ? null
                : DbElement.SelectSingle(_db, Key: edComp.ContentKeyR4B.Value);
            DbElement? r5Ed = edComp.ContentKeyR5 is null
                ? null
                : DbElement.SelectSingle(_db, Key: edComp.ContentKeyR5.Value);
            DbElement? r6Ed = edComp.ContentKeyR6 is null
                ? null
                : DbElement.SelectSingle(_db, Key: edComp.ContentKeyR6.Value);

            List<(FhirReleases.FhirSequenceCodes fv, DbElement? ed)> contentPath = (igTr.PackagePair.SourceFhirSequence < igTr.PackagePair.TargetFhirSequence)
                ? [
                    (FhirReleases.FhirSequenceCodes.DSTU2, r2Ed),
                    (FhirReleases.FhirSequenceCodes.STU3, r3Ed),
                    (FhirReleases.FhirSequenceCodes.R4, r4Ed),
                    (FhirReleases.FhirSequenceCodes.R4B, r4bEd),
                    (FhirReleases.FhirSequenceCodes.R5, r5Ed),
                    (FhirReleases.FhirSequenceCodes.R6, r6Ed)]
                : [
                    (FhirReleases.FhirSequenceCodes.R6, r6Ed),
                    (FhirReleases.FhirSequenceCodes.R5, r5Ed),
                    (FhirReleases.FhirSequenceCodes.R4B, r4bEd),
                    (FhirReleases.FhirSequenceCodes.R4, r4Ed),
                    (FhirReleases.FhirSequenceCodes.STU3, r3Ed),
                    (FhirReleases.FhirSequenceCodes.DSTU2, r2Ed)];

            mappingTraceLines.AddRange(contentPath
                    .Where(cp => cp.ed is not null)
                    .Select(cp => $" {cp.fv}: `{cp.ed!.Id}` {cp.ed!.FhirCardinalityString} `{cp.ed!.FullCollatedTypeLiteral}`"));
        }

        if (mappingTraceLines.Count > 0)
        {
            mappingTrace = "* " + string.Join("\n* ", mappingTraceLines);
        }
        else
        {
            mappingTrace = $"* {igTr.PackagePair.SourceFhirSequence} `{sourceEd.Id}` {sourceEd.FhirCardinalityString} `{sourceEd.FullCollatedTypeLiteral}`";
            if (eots.Count != 0)
            {
                foreach (DbElementOutcomeTarget eot in eots)
                {
                    if (eot.TargetElementId is null)
                    {
                        continue;
                    }
                    mappingTrace += $"\n* {igTr.PackagePair.SourceFhirSequence} `{eot.TargetElementId}`";
                }
            }
        }

        return $$$"""
            This extension is part of the cross-version definitions generated to enable use of the
            element `{{{sourceEd.Id}}}` as defined in FHIR {{{igTr.PackagePair.SourceFhirSequence}}}
            in FHIR {{{igTr.PackagePair.TargetFhirSequence}}}.

            The source element is defined as:
            `{{{sourceEd.Id}}}` {{{sourceEd.FhirCardinalityString}}} `{{{sourceEd.FullCollatedTypeLiteral}}}`

            Across FHIR versions, the element set has been mapped as:
            {{{mappingTrace}}}

            Following are the generation technical comments:
            {{{elementOutcome.Comments}}}
            """;
    }

    private StructureDefinition? buildExtSd(
        HashSet<string> generatedExtensionIds,
        XVerIgExportTrackingRecord igTr,
        Dictionary<int, DbElement> sourceElements,
        Dictionary<int, DbElement> targetElements,
        Dictionary<int, DbElementComparison> elementComparisons,
        Dictionary<int, string> contentReferenceExtUrlsByEdKey,
        DbElementOutcome elementOutcome,
        DbStructureDefinition sourceSd,
        DbElement sourceEd,
        string purpose,
        List<StructureDefinition.ContextComponent> contexts,
        bool useComponentDefinition)
    {
        string id = useComponentDefinition
            ? elementOutcome.ComponentGenShortId!
            : elementOutcome.GenShortId!;

        if (!generatedExtensionIds.Add(id))
        {
            return null;
        }

        (_, string name) = igTr.GetName(
            useComponentDefinition ? elementOutcome.ComponentGenName! : elementOutcome.GenName!,
            id);

        // build the initial structure definition for the extension
        StructureDefinition extSd = new()
        {
            Id = id,
            Url = useComponentDefinition ? elementOutcome.ComponentGenUrl : elementOutcome.GenUrl,
            Name = name,
            Version = _exporter._crossDefinitionVersion,
            FhirVersion = EnumUtility.ParseLiteral<FHIRVersion>(igTr.PackagePair.TargetPackage.PackageVersion),
            Date = _exporter._runTime.ToString("O"),
            Title = $"Cross-version Extension `{igTr.PackagePair.SourceFhirSequence}.{sourceEd.Id}` for use in FHIR {igTr.PackagePair.TargetFhirSequence}",
            Description = $"This cross-version extension represents the FHIR {igTr.PackagePair.SourceFhirSequence} element `{sourceEd.Id}` for use in FHIR {igTr.PackagePair.TargetFhirSequence}.",
            Purpose = purpose ?? elementOutcome.Comments,
            Status = PublicationStatus.Active,
            Experimental = false,
            Kind = StructureDefinition.StructureDefinitionKind.ComplexType,
            Abstract = false,
            Context = contexts,
            Type = "Extension",
            BaseDefinition = "http://hl7.org/fhir/StructureDefinition/Extension",
            Derivation = StructureDefinition.TypeDerivationRule.Constraint,
            Differential = new()
            {
                Element = [],
            },
        };

        // TODO: right now I am setting FHIR-I as the WG responsible since we are creating the extension - should we use the WG from the source resource?
        string wg = "fhir";

        // add the work group extension
        extSd.AddExtension(CommonDefinitions.ExtUrlWorkGroup, new Hl7.Fhir.Model.Code(wg));

        // ensure there is a publisher, use the WG if there is none
        extSd.Publisher = CommonDefinitions.WorkgroupNames[wg];

        // ensure there is a contact point - use the default WG unless there are multiple entries
        if ((extSd.Contact == null) || (extSd.Contact.Count < 2))
        {
            extSd.Contact = [
                new()
                {
                    Name = CommonDefinitions.WorkgroupNames[wg],
                    Telecom = [
                        new()
                        {
                            System = ContactPoint.ContactPointSystem.Url,
                            Value = CommonDefinitions.WorkgroupUrls[wg],
                        },
                    ],
                }
            ];
        }

        extSd.cgAddPackageSource(igTr.PackageId, _exporter._crossDefinitionVersion, igTr.PackageUrl);

        // add the version-specific fhir version information
        extSd.Extension.Add(new Extension()
        {
            Url = CommonDefinitions.ExtUrlVersionSpecificUse,
            Extension = [
                new()
                {
                    Url = CommonDefinitions.ExtUrlVersionSpecificUseStart,
                    Value = new Code(igTr.PackagePair.TargetFhirVersionShort),
                },
                new()
                {
                    Url = CommonDefinitions.ExtUrlVersionSpecificUseEnd,
                    Value = new Code(igTr.PackagePair.TargetFhirVersionShort),
                },
            ],
        });

        // add this element and its children to the differential
        addToDifferentialRecursive(
            extSd,
            igTr,
            sourceElements,
            targetElements,
            elementComparisons,
            contentReferenceExtUrlsByEdKey,
            elementOutcome,
            sourceSd,
            sourceEd);

        return extSd;
    }

    private void addToDifferentialRecursive(
        StructureDefinition extSd,
        XVerIgExportTrackingRecord igTr,
        Dictionary<int, DbElement> sourceElements,
        Dictionary<int, DbElement> targetElements,
        Dictionary<int, DbElementComparison> elementComparisons,
        Dictionary<int, string> contentReferenceExtUrlsByEdKey,
        DbElementOutcome edOutcome,
        DbStructureDefinition sourceSd,
        DbElement sourceEd,
        string? extElementId = null,
        string? extElementPath = null,
        Dictionary<string, ElementDefinition>? dtValueElements = null,
        string? dataTypeNameLiteral = null,
        int? dtBindingVsKey = null,
        List<string>? dtTypeProfiles = null,
        List<string>? dtTargetProfiles = null,
        bool excludeExtensionElement = false,
        string? elementUrlOverride = null)
    {
        extElementId ??= "Extension";
        extElementPath ??= "Extension";
        dtValueElements ??= [];

        //bool isRoot = edOutcome.ParentElementOutcomeKey is null;
        bool isRoot = extSd.Differential.Element.Count == 0;

        // add the starting element for where we are
        if (isRoot)
        {
            string? modReason = (edOutcome.DefineAsModifier != true)
                ? null
                : sourceEd.IsModifierReason
                    ?? $"This extension is a modifier because the source element `{sourceEd.Id}` is flagged as a modifier element.";

            // add the root extension element for the structure
            ElementDefinition rootElement = new()
            {
                ElementId = extElementId,
                Path = extElementPath,
                Short = sourceEd.Short,
                Definition = sourceEd.Definition,
                Comment = sourceEd.Comments,
                //Requirements = (sourceEd.Requirements is null)
                //    ? extSd.Purpose
                //    : sourceEd.Requirements + " " + extSd.Purpose,
                Min = sourceEd.MinCardinality,
                Max = sourceEd.MaxCardinalityString,
                Base = new()
                {
                    Path = "Extension",
                    Min = 0,
                    Max = "*",
                },
                IsModifier = edOutcome.DefineAsModifier,
                IsModifierReason = modReason,
            };

            extSd.Differential.Element.Add(rootElement);
        }
        else if (!excludeExtensionElement)
        {
            // add the primary slice
            ElementDefinition sliceElement = new()
            {
                ElementId = extElementId,
                Path = extElementPath,
                SliceName = elementUrlOverride ?? edOutcome.SourceNameClean(),  // edOutcome.GenShortId,
                Short = sourceEd.Short,
                Definition = sourceEd.Definition,
                Comment = sourceEd.Comments,
                Requirements = (sourceEd.Requirements is null)
                    ? edOutcome.Comments
                    : sourceEd.Requirements + " " + edOutcome.Comments,
                Min = sourceEd.MinCardinality,
                Max = sourceEd.MaxCardinalityString,
                Base = new()
                {
                    Path = "Extension.extension",
                    Min = 0,
                    Max = "*",
                }
            };

            extSd.Differential.Element.Add(sliceElement);
        }

        // check to see if this is an outcome that has a component definition
        if (!isRoot &&
            contentReferenceExtUrlsByEdKey.TryGetValue(sourceEd.Key, out string? crExtUrl) ||
            ((sourceEd.ContentReferenceSourceKey is not null) &&
                contentReferenceExtUrlsByEdKey.TryGetValue(sourceEd.ContentReferenceSourceKey.Value, out crExtUrl)))
        {
            // add the URL element (always required)
            extSd.Differential.Element.Add(new()
            {
                ElementId = extElementId + ".url",
                Path = extElementPath + ".url",
                Base = new()
                {
                    Path = "Extension.url",
                    Min = 1,
                    Max = "1",
                },
                Min = 1,
                Max = "1",
                Fixed = new FhirUri(crExtUrl),
            });

            // add a contrained out value element
            extSd.Differential.Element.Add(new()
            {
                ElementId = extElementId + ".value[x]",
                Path = extElementPath + ".value[x]",
                Base = new()
                {
                    Path = "Extension.value[x]",
                    Min = 0,
                    Max = "1",
                },
                Min = 0,
                Max = "0",
                Type = [],
            });

            // done with this path
            return;
        }

        //// if this is the root, we need to index it now so that recursive calls can find it
        //if (isRoot)
        //{
        //    // need to add immediately if root so that recusive types resolve correctly
        //    contentReferenceExtUrlsByEdKey[sourceEd.Key] = extSd.Url;
        //}

        // if this is a datatype element, add the necessary meta elements
        if (dataTypeNameLiteral is not null)
        {
            addDataTypeMetaElements(
                igTr.PackagePair.SourceFhirSequence,
                igTr.PackagePair.TargetFhirSequence,
                extSd,
                extElementId,
                extElementPath,
                dataTypeNameLiteral,
                addSliceEd: true,
                addUrlEd: true,
                addValueEd: true);
        }

        // get the child outcomes so we know if there are child elements to process
        List<DbElementOutcome> childOutcomes = DbElementOutcome.SelectList(
            _db,
            ParentElementOutcomeKey: edOutcome.Key,
            //RequiresXVerDefinition: true,             // TODO: verify, but once we are in an element I think we need to keep the whole tree
            orderByProperties: [nameof(DbElementOutcome.SourceResourceOrder)]);

        //bool hasChildren = childOutcomes.Count > 0;
        bool hasChildren = sourceEd.ChildElementCount > 0;

        // get the unmapped source types so we can determine value[x] types vs. _datatype extension slices
        List<DbElementType> sourceTypes = (hasChildren || edOutcome.UnmappedTypeKeysLiteral is null)
            ? []
            : DbElementType.SelectList(
                _db,
                KeyValues: edOutcome.UnmappedTypeKeys);


        // if there are no unmapped types, we need to account for everything (e.g., we are in an element that is a child of a type)
        if ((sourceTypes.Count == 0) &&
            !hasChildren)
        {
            sourceTypes = DbElementType.SelectList(
                _db,
                ElementKey: sourceEd.Key);
        }

        List<(string tn, bool isValid, DbElementType et)> typeValidity = sourceTypes
            .Select(et => getValidValueType(igTr.PackagePair.TargetFhirSequence, et))
            .ToList();

        List<(string tn, DbElementType et)> validValueTypes = typeValidity
            .Where(tv => tv.isValid)
            .Select(tv => (tv.tn, tv.et))
            .ToList();

        List<(string tn, DbElementType et)> invalidValueTypes = typeValidity
            .Where(tv => !tv.isValid)
            .Select(tv => (tv.tn, tv.et))
            .ToList();

        bool needsValueElement = validValueTypes.Count > 0;
        bool needsExtensionElement = hasChildren || (invalidValueTypes.Count > 0);

        int extensionMinCardinality = childOutcomes.Sum(eo => eo.SourceMinCardinality);
        if (invalidValueTypes.Count > 0)
        {
            extensionMinCardinality += sourceEd.MinCardinality;
        }

        if (needsExtensionElement)
        {
            ElementDefinition extensionElement = new()
            {
                ElementId = extElementId + ".extension",
                Path = extElementPath + ".extension",
                Base = new()
                {
                    Path = "Extension.extension",
                    Min = 0,
                    Max = "*",
                },
                Slicing = new()
                {
                    Discriminator = [
                        new() {
                            Type = ElementDefinition.DiscriminatorType.Value,
                            Path = "url",
                        }
                    ],
                    Ordered = false,
                    Rules = ElementDefinition.SlicingRules.Open,
                },
                Min = extensionMinCardinality,
                Max = "*",
            };

            extSd.Differential.Element.Add(extensionElement);
        }

        // nest into child elements first
        foreach (DbElementOutcome childOutcome in childOutcomes)
        {
            // get the source element
            if (!sourceElements.TryGetValue(childOutcome.SourceElementKey, out DbElement? childSourceEd))
            {
                _logger.LogError($"Source element with key `{childOutcome.SourceElementKey}` not found for child element outcome with key `{childOutcome.Key}`");
                continue;
            }

            if (skipElement(childSourceEd))
            {
                continue;
            }

            //string nextId = extElementId + ".extension:" + childOutcome.GenShortId;
            string nextId = extElementId + ".extension:" + childOutcome.SourceNameClean();
            string nextPath = extElementPath + ".extension";

            addToDifferentialRecursive(
                extSd,
                igTr,
                sourceElements,
                targetElements,
                elementComparisons,
                contentReferenceExtUrlsByEdKey,
                childOutcome,
                sourceSd,
                childSourceEd,
                nextId,
                nextPath,
                dtValueElements);
        }

        List<string> distinctInvalidTypeNames = invalidValueTypes
            .Select(kvp => kvp.et.TypeName ?? kvp.et.Literal)
            .Distinct()
            .ToList();

        if (invalidValueTypes.Count > 0)
        {
            // create a lookup so we can consolidate types
            ILookup<string, (int invalidTypeKey, DbElementType et)> ietsByCode = invalidValueTypes
                .ToLookup(kvp => kvp.et.TypeName ?? kvp.et.Literal, kvp => (kvp.et.Key, kvp.et));

            foreach (string distinctTypeName in distinctInvalidTypeNames)
            {
                List<(int invalidTypeKey, DbElementType et)> iets = ietsByCode[distinctTypeName].ToList();

                // if there is only one type, it might be a content reference
                if ((iets.Count == 1) &&
                    !isRoot &&
                    contentReferenceExtUrlsByEdKey.TryGetValue(iets[0].et.ElementKey, out string? crUrl))
                {
                    // resolve the element
                    DbElement? crEd = DbElement.SelectSingle(_db, Key: iets[0].et.ElementKey);
                    if (crEd is null)
                    {
                        throw new Exception($"Failed to resolve data type content reference element: {crUrl}, {iets[0].et.ElementKey}, {distinctTypeName}");
                    }

                    // add this as a slice
                    addToDifferentialRecursive(
                        extSd,
                        igTr,
                        sourceElements,
                        targetElements,
                        elementComparisons,
                        contentReferenceExtUrlsByEdKey,
                        edOutcome,
                        sourceSd,
                        crEd,
                        extElementId + ".extension:" + crEd.NameClean(),
                        extElementPath + ".extension",
                        dtValueElements);

                    continue;
                }

                // consolidate profile information
                List<string> ietTypeProfiles = iets
                    .Where(kvp => kvp.et.TypeProfile is not null)
                    .Select(kvp => kvp.et.TypeProfile!)
                    .ToList();

                List<string> ietTargetProfiles = iets
                    .Where(kvp => kvp.et.TargetProfile is not null)
                    .Select(kvp => kvp.et.TargetProfile!)
                    .ToList();

                // resolve a type element for this type
                DbElement? ietEd = DbElement.SelectSingle(
                    _db,
                    FhirPackageKey: igTr.PackagePair.SourcePackageKey,
                    Id: distinctTypeName);

                if (ietEd is null)
                {
                    throw new Exception($"Failed to resolve element: {distinctTypeName} in FHIR {igTr.PackagePair.SourceFhirSequence}");
                }

                if (skipElement(ietEd, skipFirstElement: false, skipIds: true, skipExtensions: true, skipModifierExtenions: true))
                {
                    continue;
                }

                if (distinctInvalidTypeNames.Count == 1)
                {
                    List<DbElement> dtElements = DbElement.SelectList(
                        _db,
                        ParentElementKey: ietEd.Key);

                    // if there is a single type, nest into it directly
                    addDataTypeMetaElements(
                        igTr.PackagePair.SourceFhirSequence,
                        igTr.PackagePair.TargetFhirSequence,
                        extSd,
                        extElementId,
                        extElementPath,
                        distinctTypeName,
                        addSliceEd: true,
                        addUrlEd: true,
                        addValueEd: true);

                    foreach (DbElement dtEtEd in dtElements)
                    {
                        if (skipElement(dtEtEd, skipFirstElement: true, skipIds: true, skipExtensions: true, skipModifierExtenions: true))
                        {
                            continue;
                        }

                        // resolve an element outcome for this source
                        DbElementOutcome? etEdOutcome = DbElementOutcome.SelectSingle(
                            _db,
                            SourceFhirPackageKey: igTr.PackagePair.SourcePackageKey,
                            TargetFhirPackageKey: igTr.PackagePair.TargetPackageKey,
                            SourceElementKey: dtEtEd.Key);

                        if (etEdOutcome is null)
                        {
                            throw new Exception("What do I do here?");
                        }

                        addToDifferentialRecursive(
                            extSd,
                            igTr,
                            sourceElements,
                            targetElements,
                            elementComparisons,
                            contentReferenceExtUrlsByEdKey,
                            etEdOutcome,
                            sourceSd,
                            dtEtEd,
                            extElementId + ".extension:" + dtEtEd.NameClean(),
                            extElementPath + ".extension",
                            dtValueElements,
                            null,
                            sourceEd.BindingValueSetKey,
                            ietTypeProfiles,
                            ietTargetProfiles,
                            excludeExtensionElement: false,
                            elementUrlOverride: dtEtEd.NameClean());
                    }
                }
                else
                {
                    // resolve an element outcome for this source
                    DbElementOutcome? etEdOutcome = DbElementOutcome.SelectSingle(
                        _db,
                        SourceFhirPackageKey: igTr.PackagePair.SourcePackageKey,
                        TargetFhirPackageKey: igTr.PackagePair.TargetPackageKey,
                        SourceElementKey: ietEd.Key);

                    if (etEdOutcome is null)
                    {
                        throw new Exception("What do I do here?");
                    }

                    // if there are more types, nest into them as 'valueX' slices
                    addToDifferentialRecursive(
                        extSd,
                        igTr,
                        sourceElements,
                        targetElements,
                        elementComparisons,
                        contentReferenceExtUrlsByEdKey,
                        etEdOutcome,
                        sourceSd,
                        ietEd,
                        extElementId + ":extension:value" + distinctTypeName,
                        extElementPath + ".extension",
                        dtValueElements,
                        distinctTypeName,
                        sourceEd.BindingValueSetKey,
                        ietTypeProfiles,
                        ietTargetProfiles,
                        excludeExtensionElement: false,
                        elementUrlOverride: $"value{distinctTypeName}");
                }
            }
        }

        // add the URL element (always required)
        extSd.Differential.Element.Add(new()
        {
            ElementId = extElementId + ".url",
            Path = extElementPath + ".url",
            Base = new()
            {
                Path = "Extension.url",
                Min = 1,
                Max = "1",
            },
            Min = 1,
            Max = "1",
            Fixed = new FhirUri(elementUrlOverride ?? edOutcome.GenUrl),
        });

        // add the value element - either with proper types or constrained to zero repetitions
        if (validValueTypes.Count == 0)
        {
            ElementDefinition etValueEd = new()
            {
                ElementId = extElementId + ".value[x]",
                Path = extElementPath + ".value[x]",
                Base = new()
                {
                    Path = "Extension.value[x]",
                    Min = 0,
                    Max = "1",
                },
                Min = 0,
                Max = "0",
                Type = [],
            };

            extSd.Differential.Element.Add(etValueEd);
        }
        else
        {
            // need to adjust the min based on whether we have something that is in the extension space
            int valueMin = sourceEd.MinCardinality;
            if ((valueMin != 0) && needsExtensionElement)
            {
                valueMin = 0;
            }

            ElementDefinition etValueEd = new()
            {
                ElementId = extElementId + ".value[x]",
                Path = extElementPath + ".value[x]",
                Short = sourceEd.Short,
                Definition = sourceEd.Definition,
                Comment = sourceEd.Comments,
                Base = new()
                {
                    Path = "Extension.value[x]",
                    Min = 0,
                    Max = "1",
                },
                Min = valueMin,
                Max = "1",
                Type = [],
            };

            // add a value-set binding (if necessary)
            if (sourceEd.BindingValueSetKey is not null)
            {
                DbValueSetOutcome? vsOutcome = DbValueSetOutcome.SelectSingle(
                    _db,
                    SourceFhirPackageKey: igTr.PackagePair.SourcePackageKey,
                    TargetFhirPackageKey: igTr.PackagePair.TargetPackageKey,
                    SourceValueSetKey: sourceEd.BindingValueSetKey.Value);

                if (vsOutcome is not null)
                {
                    etValueEd.Binding = new()
                    {
                        Strength = sourceEd.ValueSetBindingStrength ?? BindingStrength.Preferred,
                        Description = sourceEd.BindingDescription,
                        ValueSet = vsOutcome.GenUrl,
                    };
                }
            }

            bool addedDtTypeProfiles = false;
            bool addedDtTargetProfiles = false;

            // add the types
            Dictionary<string, ElementDefinition.TypeRefComponent> typeRefs = [];
            foreach ((string typeName, DbElementType et) in validValueTypes)
            {
                if (!typeRefs.TryGetValue(typeName, out ElementDefinition.TypeRefComponent? tr))
                {
                    tr = new()
                    {
                        Code = typeName,
                    };
                    typeRefs[typeName] = tr;
                }

                if ((dtBindingVsKey is not null) &&
                    FhirTypeMappings.CanApplyBindings(typeName))
                {
                    DbValueSetOutcome? vsOutcome = DbValueSetOutcome.SelectSingle(
                        _db,
                        SourceFhirPackageKey: igTr.PackagePair.SourcePackageKey,
                        TargetFhirPackageKey: igTr.PackagePair.TargetPackageKey,
                        SourceValueSetKey: dtBindingVsKey.Value);

                    if (vsOutcome is not null)
                    {
                        etValueEd.Binding = new()
                        {
                            Strength = sourceEd.ValueSetBindingStrength ?? BindingStrength.Preferred,
                            Description = sourceEd.BindingDescription,
                            ValueSet = vsOutcome.GenUrl,
                        };
                    }
                }

                if (et.TypeProfile is not null)
                {
                    tr.ProfileElement ??= [];
                    tr.ProfileElement.AddRange(getValidResourceUrls(igTr.PackagePair.SequencePair, [et.TypeProfile]));
                }

                if (!addedDtTypeProfiles &&
                    (dtTypeProfiles is not null) &&
                    FhirTypeMappings.CanApplyTypeProfiles(typeName))
                {
                    addedDtTypeProfiles = true;
                    tr.ProfileElement ??= [];
                    tr.ProfileElement.AddRange(getValidResourceUrls(igTr.PackagePair.SequencePair, dtTypeProfiles));
                }

                if (et.TargetProfile is not null)
                {
                    tr.TargetProfileElement ??= [];
                    tr.TargetProfileElement.AddRange(getValidResourceUrls(igTr.PackagePair.SequencePair, [et.TargetProfile]));
                }

                if (!addedDtTargetProfiles &&
                    (dtTargetProfiles is not null) &&
                    FhirTypeMappings.CanApplyTargetProfiles(typeName))
                {
                    addedDtTargetProfiles = true;
                    tr.TargetProfileElement ??= [];
                    tr.TargetProfileElement.AddRange(getValidResourceUrls(igTr.PackagePair.SequencePair, dtTargetProfiles));
                }
            }

            etValueEd.Type = typeRefs.Values.OrderBy(tr => tr.TypeName).ToList();

            // add our element
            extSd.Differential.Element.Add(etValueEd);
        }
    }

    private List<Canonical> getValidResourceUrls(
        (FhirReleases.FhirSequenceCodes s, FhirReleases.FhirSequenceCodes t) ps,
        List<string> urls)
    {
        if (!_resourceReferenceLookup.TryGetValue(ps, out PackagePairStructureMappingTracker? mt) ||
            (mt is null))
        {
            return urls.Select(v => new Canonical(v)).ToList();
        }

        HashSet<string> valids = [];
        foreach (string v in urls)
        {
            List<string> targets = mt.GetTargets(v);
            if (targets.Count == 0)
            {
                valids.Add(v);
                continue;
            }

            foreach (string t in mt.GetTargets(v))
            {
                valids.Add(t);
            }
        }

        return valids.Select(v => new Canonical(v)).ToList();
    }

    private bool skipElement(
        DbElement ed,
        bool skipFirstElement = true,
        bool skipIds = true,
        bool skipExtensions = true,
        bool skipModifierExtenions = true) =>
            (skipFirstElement && (ed.ResourceFieldOrder == 0)) ||
            (skipIds && ((ed.BasePath == "id") || (ed.BasePath == "Element.id") || (ed.BasePath == "Resource.id"))) ||
            (skipExtensions && ((ed.Name == "extension") || (ed.FullCollatedTypeLiteral == "Extension"))) ||
            (skipModifierExtenions &&
                ((ed.Name == "modifierExtension") || (ed.BasePath == "DomainResource.modifierExtension") || (ed.BasePath == "BackboneElement.modifierExtension")));

    private void addDataTypeMetaElements(
        FhirReleases.FhirSequenceCodes sourceFhirSequence,
        FhirReleases.FhirSequenceCodes targetFhirSequence,
        StructureDefinition extSd,
        string currentId,
        string currentPath,
        string dataTypeName,
        bool addSliceEd,
        bool addUrlEd,
        bool addValueEd)
    {
        string id = currentId + ".extension:_datatype";
        string path = currentPath + ".extension";

        if (addSliceEd)
        {
            ElementDefinition sliceEd = new()
            {
                ElementId = id,
                Path = path,
                SliceName = "_datatype",
                //SliceName = dataTypeName,
                Short = $"DataType slice for a FHIR {sourceFhirSequence} `{dataTypeName}` value",
                Definition = $"Slice to indicate the presence of a {sourceFhirSequence} `{dataTypeName}` in FHIR {targetFhirSequence}",
                Min = 1,
                Max = "1",
                Base = new()
                {
                    Path = "Extension.extension",
                    Min = 0,
                    Max = "*",
                },
            };
            extSd.Differential.Element.Add(sliceEd);
        }

        if (addUrlEd)
        {
            ElementDefinition urlEd = new()
            {
                ElementId = id + ".url",
                Path = path + ".url",
                Fixed = new FhirUri("http://hl7.org/fhir/StructureDefinition/_datatype"),
                Min = 1,
                Max = "1",
                Base = new()
                {
                    Path = "Extension.url",
                    Min = 1,
                    Max = "1",
                },
            };
            extSd.Differential.Element.Add(urlEd);
        }

        if (addValueEd)
        {
            ElementDefinition valueEd = new()
            {
                ElementId = id + ".value[x]",
                Path = path + ".value[x]",
                Comment = $"Must be: {dataTypeName}",
                Min = 1,
                Max = "1",
                Base = new()
                {
                    Path = "Extension.value[x]",
                    Min = 0,
                    Max = "1",
                },
                Type = [
                    new()
                    {
                        Code = "string",
                    }
                ],
                Fixed = new FhirString(dataTypeName),
            };
            extSd.Differential.Element.Add(valueEd);
        }
    }

    private (string tn, bool isValid, DbElementType et) getValidValueType(FhirReleases.FhirSequenceCodes targetFhirSequence, DbElementType et)
    {
        string desiredType = et.TypeName ?? et.Literal;

        if (_extensionValueTypeNames[targetFhirSequence].Contains(desiredType))
        {
            return (desiredType, true, et);
        }

        if (FhirTypeMappings.PrimitiveTypeFallbacks.TryGetValue(desiredType, out string? fallbackType) &&
            _extensionValueTypeNames[targetFhirSequence].Contains(fallbackType))
        {
            return (fallbackType, true, et);
        }

        return (desiredType, false, et);

        //throw new Exception($"Type `{desiredType}` is not a valid extension value type in FHIR {targetFhirSequence} and has no valid fallback.");
    }
}
