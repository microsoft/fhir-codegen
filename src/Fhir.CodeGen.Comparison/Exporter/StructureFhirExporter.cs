using System;
using System.Collections.Generic;
using System.Data;
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
                StructureOutcomeKey: sdOutcome.Key,
                orderByProperties: [nameof(DbElementOutcome.SourceResourceOrder)]);

            string? lastSourceId = null;
            ConceptMap.SourceElementComponent? currentSourceElement = null;

            Dictionary<int, string> outcomeUrlComposition = [];

            // iterate over the element outcomes
            foreach (DbElementOutcome edOutcome in edOutcomes)
            {
                // check if we need a new source element
                if ((currentSourceElement is null) ||
                    (lastSourceId != edOutcome.SourceId))
                {
                    // create a new source element
                    currentSourceElement = new()
                    {
                        Code = edOutcome.SourceId,
                        Display = edOutcome.SourceName,
                        Target = [],
                    };
                    edCm.Group[0].Element.Add(currentSourceElement);
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

                if (edOutcome.TargetElementKey is null)
                {
                    string code;
                    if (edOutcome.BasicElementEquivalent is not null)
                    {
                        string[] components = edOutcome.BasicElementEquivalent.Split('.');
                        code = string.Join('.', ["Basic", .. components[1..]]);
                    }
                    else if (edOutcome.ParentElementOutcomeKey is null)
                    {
                        code = edOutcome.ExtensionSubstitutionUrl ?? edOutcome.GenUrl!;
                    }
                    else if (outcomeUrlComposition.TryGetValue(edOutcome.ParentElementOutcomeKey.Value, out string? parentUrl))
                    {
                        code = parentUrl + ":" + edOutcome.GenUrl!;
                    }
                    else
                    {
                        code = edOutcome.GenUrl!;
                    }

                    outcomeUrlComposition[edOutcome.Key] = code;

                    // create our target element
                    ConceptMap.TargetElementComponent targetElement = new()
                    {
                        Code = code,
                        Display = edOutcome.TargetName,
                        Relationship = relationship,
                        Comment = edOutcome.Comments,
                    };
                    currentSourceElement.Target.Add(targetElement);
                }
                else
                {
                    // create our target element
                    ConceptMap.TargetElementComponent targetElement = new()
                    {
                        Code = sdOutcome.TargetCanonicalUnversioned + "#" + edOutcome.TargetId,
                        Display = edOutcome.TargetName,
                        Relationship = relationship,
                        Comment = edOutcome.Comments,
                    };
                    currentSourceElement.Target.Add(targetElement);
                }
            }

            // write the profile to a file
            string filename = $"ConceptMap-{edCm.Id}.json";
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
        igTr.ProfileFiles = exported;
    }

    private ConceptMap createElementConceptMap(
        XVerIgExportTrackingRecord igTr,
        DbStructureOutcome sdOutcome)
    {
        string targetId = sdOutcome.TargetId ?? "Basic";
        string id = $"{igTr.PackagePair.SourcePackageShortName}-{sdOutcome.SourceId}-elements-for-{igTr.PackagePair.TargetPackageShortName}-{targetId}";

        ConceptMap vsCm = new()
        {
            Id = id,
            Url = $"http://hl7.org/fhir/{igTr.PackagePair.SourceFhirVersionShort}/ConceptMap/{id}",
            Name = FhirSanitizationUtils.ReformatIdForName(id),
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

        // write the profile to a file
        string filename = $"ConceptMap-{cm.Id}.json";
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
        igTr.ProfileFiles = exported;
    }

    private ConceptMap createResourceConceptMap(
        XVerIgExportTrackingRecord igTr)
    {
        string id = $"{igTr.PackagePair.SourcePackageShortName}-resources-for-{igTr.PackagePair.TargetPackageShortName}";

        ConceptMap vsCm = new()
        {
            Id = id,
            Url = $"http://hl7.org/fhir/{igTr.PackagePair.SourceFhirVersionShort}/ConceptMap/{id}",
            Name = FhirSanitizationUtils.ReformatIdForName(id),
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
                    (targetSd.Name == "Basic"))
                {
                    DbElementOutcome? rootElementOutcome = DbElementOutcome.SelectSingle(
                            _db,
                            SourceFhirPackageKey: igTr.PackagePair.SourcePackageKey,
                            TargetFhirPackageKey: igTr.PackagePair.TargetPackageKey,
                            StructureOutcomeKey: sdOutcome.Key,
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
                string filename = $"StructureDefinition-{profileSd.Id}.json";
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
        // get the element outcomes
        List<DbElementOutcome> edOutcomes = DbElementOutcome.SelectList(
            _db,
            StructureOutcomeKey: sdOutcome.Key,
            RequiresXVerDefinition: true,
            ParentElementOutcomeKeyIsNull: true);

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
            if (edOutcomeContextLookup.Contains(targetEd.Id))
            {
                targetEdOutcomes.AddRange(edOutcomeContextLookup[targetEd.Id]);
            }

            // if there are none, move on
            if (targetEdOutcomes.Count == 0)
            {
                continue;
            }

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
                            Profile = [ targetEdOutcome.GenUrl ],
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

        StructureDefinition profileSd = new()
        {
            Id = profileId,
            Url = sdOutcome.GenUrl,
            Name = FhirSanitizationUtils.ReformatIdForName(sdOutcome.GenLongId!),
            Version = _exporter._crossDefinitionVersion,
            FhirVersion = EnumUtility.ParseLiteral<FHIRVersion>(igTr.PackagePair.TargetPackage.PackageVersion) ?? FHIRVersion.N5_0_0,
            DateElement = new FhirDateTime(DateTimeOffset.Now),
            Title = $"Cross-version Profile for {igTr.PackagePair.SourceFhirSequence}.{sourceSd.Name} for use in FHIR {igTr.PackagePair.TargetFhirSequence}",
            Description = $"This cross-version profile allows " +
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

        Dictionary<int, string> contentReferenceExtUrlsByEdKey = [];

        // get the element outcomes that need component-style exporting
        List<DbElementOutcome> componentEdOutcomes = DbElementOutcome.SelectList(
            _db,
            SourceFhirPackageKey: igTr.PackagePair.SourcePackageKey,
            TargetFhirPackageKey: igTr.PackagePair.TargetPackageKey,
            RequiresComponentDefinition: true,
            ExtensionSubstitutionKeyIsNull: true,
            orderByProperties: [nameof(DbElementOutcome.SourceStructureKey), nameof(DbElementOutcome.SourceResourceOrder)]);

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

            StructureDefinition extSd = buildExtSd(
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

            contentReferenceExtUrlsByEdKey[sourceEd.Key] = extSd.Url;

            // write the extension to a file
            string filename = $"StructureDefinition-{extSd.Id}.json";
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
            ParentElementOutcomeKeyIsNull: true,
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

            DbStructureOutcome? sdOutcome = null;
            if (edOutcome.SourceResourceOrder == 0)
            {
                sdOutcome = DbStructureOutcome.SelectSingle(
                    _db,
                    SourceFhirPackageKey: igTr.PackagePair.SourcePackageKey,
                    TargetFhirPackageKey: igTr.PackagePair.TargetPackageKey,
                    SourceStructureKey: sourceSd.Key,
                    TargetStructureKey: edOutcome.TargetStructureKey);
            }

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

            StructureDefinition extSd = buildExtSd(
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

            // write the extension to a file
            string filename = $"StructureDefinition-{extSd.Id}.json";
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

        if (elementComparisons.TryGetValue(elementOutcome.ComparisonKey, out DbElementComparison? edComp))
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

            List<DbElement?> contentPath = (igTr.PackagePair.SourceFhirSequence < igTr.PackagePair.TargetFhirSequence)
                ? [r2Ed, r3Ed, r4Ed, r4bEd, r5Ed, r6Ed]
                : [r6Ed, r5Ed, r4bEd, r4Ed, r3Ed, r2Ed];

            mappingTrace = "* " +
                string.Join("\n* ", contentPath
                    .Where(ed => ed is not null)
                    .Select(ed => $"`{ed!.Id}` {ed!.FhirCardinalityString} `{ed!.FullCollatedTypeLiteral}`"));
        }

        return $$$"""
                This extension is part of the cross-version definitions generated to enable use of the
                element `{{{sourceEd.Id}}}` as defined in FHIR {{{igTr.PackagePair.SourceFhirSequence}}}
                in FHIR {{{igTr.PackagePair.TargetFhirSequence}}}.

                The source element is defined as:
                `{{{sourceEd.Id}}}` {{{sourceEd.FhirCardinalityString}}} `{{{sourceEd.FullCollatedTypeLiteral}}}`

                Across FHIR versions, the value set has been mapped as:
                {{{mappingTrace}}}

                Following are the generation technical comments:
                {{{elementOutcome.Comments}}}
                """;
    }

    private StructureDefinition buildExtSd(
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
        // build the initial structure definition for the extension
        StructureDefinition extSd = new()
        {
            //Id = sdOutcome?.GenShortId ?? edOutcome.GenShortId,
            Id =  useComponentDefinition ? elementOutcome.ComponentGenShortId : elementOutcome.GenShortId,
            //Url = sdOutcome?.GenUrl ?? edOutcome.GenUrl,
            Url = useComponentDefinition ? elementOutcome.ComponentGenUrl : elementOutcome.GenUrl,
            //Name = FhirSanitizationUtils.ReformatIdForName(sdOutcome?.GenLongId ?? edOutcome.GenLongId!),
            Name = FhirSanitizationUtils.ReformatIdForName(elementOutcome.GenLongId!),
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
                SliceName = elementUrlOverride ?? edOutcome.GenShortId,
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

        // if this is the root, we need to index it now so that recursive calls can find it
        if (isRoot)
        {
            // need to add immediately if root so that recusive types resolve correctly
            contentReferenceExtUrlsByEdKey[sourceEd.Key] = extSd.Url;
        }

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
            RequiresXVerDefinition: true,
            orderByProperties: [nameof(DbElementOutcome.SourceResourceOrder)]);

        bool hasChildren = childOutcomes.Count > 0;

        // get the unmapped source types so we can determine value[x] types vs. _datatype extension slices
        List<DbElementType> unmappedSourceTypes = (hasChildren || edOutcome.UnmappedTypeKeysLiteral is null)
            ? []
            : DbElementType.SelectList(
                _db,
                KeyValues: edOutcome.UnmappedTypeKeys);

        List<(string tn, bool isValid, DbElementType et)> typeValidity = unmappedSourceTypes
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

        //Dictionary<int, DbElementType> validValueTypes = [];
        //Dictionary<int, DbElementType> invalidValueTypes = [];
        //foreach (DbElementType et in unmappedSourceTypes)
        //{
        //    string typeName = et.TypeName ?? et.Literal;

        //    if (_extensionValueTypeNames[igTr.PackagePair.TargetFhirSequence].Contains(typeName))
        //    {
        //        // TODO: need to also check type and target profiles
        //        validValueTypes[et.Key] = et;
        //    }
        //    else
        //    {
        //        invalidValueTypes[et.Key] = et;
        //    }
        //}

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

            string nextId = extElementId + ".extension:" + childOutcome.GenShortId;
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

                //string dtRootSliceName = "value" + distinctTypeName.ToPascalCase();
                //string dtRootId = extElementId + ".extension:" + dtRootSliceName;
                //string dtRootPath = extElementPath + ".extension";
                ////string dtRootId = extElementId + ".extension:_datatype";

                //ElementDefinition dtRootEd = new()
                //{
                //    ElementId = dtRootId,
                //    Path = dtRootPath,
                //    SliceName = dtRootSliceName,
                //    Short = $"Extension slice for a FHIR {igTr.PackagePair.SourceFhirSequence} `{distinctTypeName}` value",
                //    Definition = $"Extension slice for contents to represent a {igTr.PackagePair.SourceFhirSequence} `{distinctTypeName}` in FHIR {igTr.PackagePair.TargetFhirSequence}",
                //    Min = 0,
                //    Max = sourceEd.MaxCardinalityString ?? "1",
                //    Base = new()
                //    {
                //        Path = "Extension.extension",
                //        Min = 0,
                //        Max = "*",
                //    },
                //};
                //extSd.Differential.Element.Add(dtRootEd);

                //// extension element
                //ElementDefinition dtRootExtEd = new()
                //{
                //    ElementId = dtRootId + ".extension",
                //    Path = dtRootPath + ".extension",
                //    Base = new()
                //    {
                //        Path = "Extension.extension",
                //        Min = 0,
                //        Max = "*",
                //    },
                //    Slicing = new()
                //    {
                //        Discriminator = [
                //        new() {
                //            Type = ElementDefinition.DiscriminatorType.Value,
                //            Path = "url",
                //        }
                //    ],
                //        Ordered = false,
                //        Rules = ElementDefinition.SlicingRules.Open,
                //    },
                //    Min = extensionMinCardinality,
                //    Max = "*",
                //};
                //extSd.Differential.Element.Add(dtRootExtEd);

                //// add _datatype and element slices

                //// url element
                //ElementDefinition dtRootUrlEd = new()
                //{
                //    ElementId = dtRootId + ".url",
                //    Path = dtRootPath + ".url",
                //    //Fixed = new FhirUri("http://hl7.org/fhir/StructureDefinition/_datatype"),
                //    Fixed = new FhirUri(dtRootSliceName),
                //    Min = 1,
                //    Max = "1",
                //    Base = new()
                //    {
                //        Path = "Extension.url",
                //        Min = 1,
                //        Max = "1",
                //    },
                //};
                //extSd.Differential.Element.Add(dtRootUrlEd);

                //// constrained-out value element
                //extSd.Differential.Element.Add(new()
                //{
                //    ElementId = dtRootId + ".value[x]",
                //    Path = dtRootPath + ".value[x]",
                //    Base = new()
                //    {
                //        Path = "Extension.value[x]",
                //        Min = 0,
                //        Max = "1",
                //    },
                //    Min = 0,
                //    Max = "0",
                //    Type = [],
                //});


                //// add the url element



                //extSd.Differential.Element.Add(dtValueLiteralEd);
                //dtValueElements[dtRootId] = dtValueLiteralEd;

                //addDataTypeElementsRecursive(
                //    extSd,
                //    igTr,
                //    sourceElements,
                //    targetElements,
                //    elementComparisons,
                //    contentReferenceExtUrlsByEdKey,
                //    iets.Select(kv => kv.et).ToList(),
                //    extElementPath + ".extension",
                //    extElementId + ".extension");
            }
        }

            //// add any invalid value types as extension slices with the _datatype extension
            //foreach ((int invalidTypeKey, DbElementType et) in invalidValueTypes)
            //{
            //    // first, check to see if this is a content reference type
            //    if (contentReferenceExtUrlsByEdKey.TryGetValue(et.ElementKey, out string? crUrl))
            //    {
            //        // resolve the element
            //        DbElement? crEd = DbElement.SelectSingle(_db, Key: et.ElementKey);
            //        if (crEd is null)
            //        {
            //            throw new Exception($"Failed to resolve data type content reference element: {crUrl}, {et.ElementKey}");
            //        }

            //        // resolve an outcome for this element

            //        // add this as a slice
            //        addToDifferentialRecursive(
            //            extSd,
            //            igTr,
            //            sourceElements,
            //            targetElements,
            //            elementComparisons,
            //            contentReferenceExtUrlsByEdKey,
            //            edOutcome,
            //            sourceSd,
            //            crEd,
            //            extElementId + ".extension:" + crEd.NameClean(),
            //            extElementPath + ".extension",
            //            dtValueElements);

            //        continue;
            //    }

            //    string typeName = et.TypeName ?? et.Literal;
            //    string dtRootId = extElementId + ".extension:_datatype";

            //    if (dtValueElements.TryGetValue(dtRootId, out ElementDefinition? dtValueLiteralEd))
            //    {
            //        if ((!dtValueLiteralEd.TryGetAnnotation(out List<ElementDefinition.TypeRefComponent>? dtTypes)) ||
            //            (dtTypes is null))
            //        {
            //            dtTypes = [];
            //            dtValueLiteralEd.SetAnnotation(dtTypes);
            //        }

            //        ElementDefinition.TypeRefComponent? tr = dtTypes
            //                    .Where(t => t.Code == typeName)
            //                    .FirstOrDefault();

            //        if (tr is null)
            //        {
            //            tr = new()
            //            {
            //                Code = et.TypeName,
            //                Profile = et.TypeProfile is null ? [] : [et.TypeProfile],
            //                TargetProfile = et.TargetProfile is null ? [] : [et.TargetProfile],
            //            };
            //            dtTypes.Add(tr);

            //            if (dtTypes.Count > 1)
            //            {
            //                // not sure if this is legal, see if it happens
            //                Console.Write("");
            //            }

            //            // add the value extension slice
            //            addDataTypeElementsRecursive(
            //                extSd,
            //                igTr,
            //                sourceElements,
            //                targetElements,
            //                elementComparisons,
            //                contentReferenceExtUrlsByEdKey,
            //                et,
            //                extElementPath + ".extension",
            //                extElementId + ".extension");
            //        }
            //        else
            //        {
            //            // check for needing to add a type or target profile
            //            if (et.TypeProfile is not null)
            //            {
            //                tr.ProfileElement.Add(et.TypeProfile);
            //            }

            //            if (et.TargetProfile is not null)
            //            {
            //                tr.TargetProfileElement.Add(et.TargetProfile);
            //            }
            //        }

            //        dtValueLiteralEd.Comment = dtTypes.cgLiteral();

            //        if (dtTypes.Count == 1)
            //        {
            //            dtValueLiteralEd.Fixed = new FhirString(dtTypes[0].Code);
            //        }
            //        else
            //        {
            //            dtValueLiteralEd.Fixed = null;
            //            dtValueLiteralEd.Example = dtTypes
            //                .Select(tr => new ElementDefinition.ExampleComponent()
            //                {
            //                    Label = "Allowed value for type: " + tr.Code,
            //                    Value = new FhirString(tr.Code),
            //                }
            //                ).ToList();
            //        }
            //    }
            //    else
            //    {
            //    }


            //    //// add the elements from this data type
            //    //List<DbElement> typeElements = DbElement.SelectList(
            //    //    _db,
            //    //    StructureKey: et.TypeStructureKey,
            //    //    orderByProperties: [nameof(DbElement.ResourceFieldOrder)]);

            //}

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
                    tr.ProfileElement.Add(et.TypeProfile);
                }

                if (!addedDtTypeProfiles &&
                    (dtTypeProfiles is not null) &&
                    FhirTypeMappings.CanApplyTypeProfiles(typeName))
                {
                    addedDtTypeProfiles = true;
                    tr.ProfileElement ??= [];
                    tr.ProfileElement.AddRange(dtTypeProfiles.Select(v => new Canonical(v)));
                }

                if (et.TargetProfile is not null)
                {
                    tr.TargetProfileElement ??= [];
                    tr.TargetProfileElement.Add(et.TargetProfile);
                }

                if (!addedDtTargetProfiles &&
                    (dtTargetProfiles is not null) &&
                    FhirTypeMappings.CanApplyTargetProfiles(typeName))
                {
                    addedDtTargetProfiles = true;
                    tr.ProfileElement ??= [];
                    tr.ProfileElement.AddRange(dtTargetProfiles.Select(v => new Canonical(v)));
                }
            }

            etValueEd.Type = typeRefs.Values.OrderBy(tr => tr.TypeName).ToList();

            // add our element
            extSd.Differential.Element.Add(etValueEd);
        }
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

    //private void addDataTypeElementsRecursive(
    //    StructureDefinition extSd,
    //    XVerIgExportTrackingRecord igTr,
    //    Dictionary<int, DbElement> sourceElements,
    //    Dictionary<int, DbElement> targetElements,
    //    Dictionary<int, DbElementComparison> elementComparisons,
    //    Dictionary<int, string> contentReferenceExtUrlsByEdKey,
    //    List<DbElementType> ets,
    //    string typeName,
    //    string parentPath,
    //    string parentElementId)
    //{
    //    // add the _datatype extension slice
    //    ElementDefinition dtSliceEd = new()
    //    {
    //        ElementId = parentElementId + ":_datatype",
    //        Path = parentPath,
    //        SliceName = "_datatype",
    //        Short = $"Data type name for `{typeName}`",
    //        Definition = $"Data type name for representing a {igTr.PackagePair.SourceFhirSequence} `{typeName}` in {igTr.PackagePair.TargetFhirSequence}",
    //        Min = 1,
    //        Max = "1",
    //        Base = new()
    //        {
    //            Path = "Extension.extension",
    //            Min = 0,
    //            Max = "*",
    //        },
    //        Type = [
    //            new()
    //            {
    //                Code = "Extension",
    //                Profile = ["http://hl7.org/fhir/StructureDefinition/_datatype"],
    //            }
    //        ],
    //    };
    //    extSd.Differential.Element.Add(dtSliceEd);

    //    // get the elements for this data type
    //    List<DbElement> typeElements = DbElement.SelectList(
    //        _db,
    //        StructureKey: ets[0].TypeStructureKey,
    //        orderByProperties: [nameof(DbElement.ResourceFieldOrder)]);
    //    foreach (DbElement typeEd in typeElements)
    //    {
    //        if (skipElement(typeEd))
    //        {
    //            continue;
    //        }

    //        DbElementOutcome? EdOutcome = null;
    //        if (subEt.TypeStructureKey is not null)
    //        {
    //            tnEdOutcome = DbElementOutcome.SelectSingle(
    //                _db,
    //                SourceFhirPackageKey: igTr.PackagePair.SourcePackageKey,
    //                TargetFhirPackageKey: igTr.PackagePair.TargetPackageKey,
    //                SourceStructureKey: subEt.TypeStructureKey,
    //                SourceResourceOrder: 0);
    //        }

    //        if (tnEdOutcome is null)
    //        {
    //            // check to see if we have a structure outcome that can satisfy this
    //            tnEdOutcome = DbElementOutcome.SelectSingle(
    //                _db,
    //                SourceFhirPackageKey: igTr.PackagePair.SourcePackageKey,
    //                TargetFhirPackageKey: igTr.PackagePair.TargetPackageKey,
    //                SourceId: tn,
    //                SourceResourceOrder: 0);
    //        }

    //        if (tnEdOutcome is null)
    //        {
    //            continue;
    //        }

    //        DbStructureDefinition? tnEdSd = DbStructureDefinition.SelectSingle(
    //            _db,
    //            Key: tnEdOutcome.SourceStructureKey);

    //        if (tnEdSd is null)
    //        {
    //            throw new Exception($"Failed to resolve data type structure definition for type `{tn}` with structure key `{tnEdOutcome.SourceStructureKey}`");
    //        }

    //        DbElement tnEd = DbElement.SelectSingle(
    //            _db,
    //            Key: tnEdOutcome.SourceElementKey)!;

    //        // add the slice
    //        addToDifferentialRecursive(
    //            extSd,
    //            igTr,
    //            sourceElements,
    //            targetElements,
    //            elementComparisons,
    //            contentReferenceExtUrlsByEdKey,
    //            tnEdOutcome,
    //            tnEdSd,
    //            tnEd,
    //            elementId,
    //            elementPath);
    //    }

    //    // add this element as a slice
    //    addToDifferentialRecursive(
    //            extSd,
    //            igTr,
    //            sourceElements,
    //            targetElements,
    //            elementComparisons,
    //            contentReferenceExtUrlsByEdKey,
    //            tnEdOutcome,
    //            tnEdSd,
    //            tnEd,
    //            elementId,
    //            elementPath);


    //        //string elementId = parentElementId + ":" + typeEd.NameClean();
    //        //string elementPath = parentPath + "." + typeEd.Name;

    //        //string elementId = parentElementId + ":" + typeEd.NameClean();
    //        //string elementPath = parentPath + "." + typeEd.Name;

    //        //// add the extension slice element
    //        //ElementDefinition dtExtSliceEd = new()
    //        //{
    //        //    ElementId = elementId,
    //        //    Path = elementPath,
    //        //    SliceName = typeEd.NameClean(),
    //        //    Base = new()
    //        //    {
    //        //        Path = "Extension.extension",
    //        //        Min = 0,
    //        //        Max = "*",
    //        //    },
    //        //    Min = typeEd.MinCardinality,
    //        //    Max = typeEd.MaxCardinalityString,
    //        //};
    //        //extSd.Differential.Element.Add(dtExtSliceEd);

    //        // get the types for this sub-element
    //        List<DbElementType> dtElementTypes = DbElementType.SelectList(
    //            _db,
    //            ElementKey: typeEd.Key);

    //        List<(string tn, bool isValid, DbElementType et)> typeValidity = dtElementTypes
    //            .Select(et => getValidValueType(igTr.PackagePair.TargetFhirSequence, et))
    //            .ToList();

    //        List<(string tn, DbElementType et)> validValueTypes = typeValidity
    //            .Where(tv => tv.isValid)
    //            .Select(tv => (tv.tn, tv.et))
    //            .ToList();
    //        List<(string tn, DbElementType et)> invalidValueTypes = typeValidity
    //            .Where(tv => !tv.isValid)
    //            .Select(tv => (tv.tn, tv.et))
    //            .ToList();

    //        // invalid types need to promote to extensions
    //        if (invalidValueTypes.Count > 0)
    //        {
    //            // check to see if we can resolve as an extension
    //            foreach ((string tn, DbElementType subEt) in invalidValueTypes)
    //            {
    //                DbElementOutcome? tnEdOutcome = null;
    //                if (subEt.TypeStructureKey is not null)
    //                {
    //                    tnEdOutcome = DbElementOutcome.SelectSingle(
    //                        _db,
    //                        SourceFhirPackageKey: igTr.PackagePair.SourcePackageKey,
    //                        TargetFhirPackageKey: igTr.PackagePair.TargetPackageKey,
    //                        SourceStructureKey: subEt.TypeStructureKey,
    //                        SourceResourceOrder: 0);
    //                }

    //                if (tnEdOutcome is null)
    //                {
    //                    // check to see if we have a structure outcome that can satisfy this
    //                    tnEdOutcome = DbElementOutcome.SelectSingle(
    //                        _db,
    //                        SourceFhirPackageKey: igTr.PackagePair.SourcePackageKey,
    //                        TargetFhirPackageKey: igTr.PackagePair.TargetPackageKey,
    //                        SourceId: tn,
    //                        SourceResourceOrder: 0);
    //                }

    //                if (tnEdOutcome is null)
    //                {
    //                    continue;
    //                }

    //                DbStructureDefinition? tnEdSd = DbStructureDefinition.SelectSingle(
    //                    _db,
    //                    Key: tnEdOutcome.SourceStructureKey);

    //                if (tnEdSd is null)
    //                {
    //                    throw new Exception($"Failed to resolve data type structure definition for type `{tn}` with structure key `{tnEdOutcome.SourceStructureKey}`");
    //                }

    //                DbElement tnEd = DbElement.SelectSingle(
    //                    _db,
    //                    Key: tnEdOutcome.SourceElementKey)!;

    //                // add the slice
    //                addToDifferentialRecursive(
    //                    extSd,
    //                    igTr,
    //                    sourceElements,
    //                    targetElements,
    //                    elementComparisons,
    //                    contentReferenceExtUrlsByEdKey,
    //                    tnEdOutcome,
    //                    tnEdSd,
    //                    tnEd,
    //                    elementId,
    //                    elementPath);
    //            }
    //        }

    //        // add the url element
    //        ElementDefinition dtUrlEd = new()
    //        {
    //            ElementId = parentElementId + ".url",
    //            Path = parentPath + ".url",
    //            Fixed = new FhirUri(typeEd.NameClean()),
    //            Min = 1,
    //            Max = "1",
    //            Base = new()
    //            {
    //                Path = "Extension.url",
    //                Min = 1,
    //                Max = "1",
    //            },
    //        };
    //        extSd.Differential.Element.Add(dtUrlEd);

    //        // add the value[x] element (if necessary)
    //        if (validValueTypes.Count == 0)
    //        {
    //            ElementDefinition dtValueEd = new()
    //            {
    //                ElementId = elementId + ".value[x]",
    //                Path = elementPath + ".value[x]",
    //                Min = 0,
    //                Max = "0",
    //                Base = new()
    //                {
    //                    Path = "Extension.value[x]",
    //                    Min = 0,
    //                    Max = "1",
    //                },
    //            };
    //            extSd.Differential.Element.Add(dtValueEd);
    //        }
    //        else
    //        {
    //            ElementDefinition dtValueEd = new()
    //            {
    //                ElementId = elementId + ".value[x]",
    //                Path = elementPath + ".value[x]",
    //                Min = 0,
    //                Max = "1",
    //                Base = new()
    //                {
    //                    Path = "Extension.value[x]",
    //                    Min = 0,
    //                    Max = "1",
    //                },
    //                Type = validValueTypes.Select(tn => new ElementDefinition.TypeRefComponent()
    //                {
    //                    Code = tn.tn,
    //                    TargetProfile = tn.et.TargetProfile is not null
    //                        ? [tn.et.TargetProfile]
    //                        : null,
    //                    Profile = tn.et.TypeProfile is not null
    //                        ? [tn.et.TypeProfile]
    //                        : null,
    //                }).ToList(),
    //            };
    //            extSd.Differential.Element.Add(dtValueEd);
    //        }
    //    }


    //    ElementDefinition dtRootUrlEd = new()
    //    {
    //        ElementId = parentElementId + ".url",
    //        Path = parentPath + ".url",
    //        Fixed = new FhirUri("http://hl7.org/fhir/StructureDefinition/_datatype"),
    //        Min = 1,
    //        Max = "1",
    //        Base = new()
    //        {
    //            Path = "Extension.url",
    //            Min = 1,
    //            Max = "1",
    //        },
    //    };
    //    extSd.Differential.Element.Add(dtRootUrlEd);

    //    ElementDefinition dtValueLiteralEd = new()
    //    {
    //        ElementId = parentElementId + ".value[x]",
    //        Path = parentPath + ".extension.value[x]",
    //        Comment = $"Must be: {typeName}",
    //        Min = 1,
    //        Max = "1",
    //        Base = new()
    //        {
    //            Path = "Extension.value[x]",
    //            Min = 0,
    //            Max = "1",
    //        },
    //        Type = [
    //            new()
    //            {
    //                Code = "string",
    //            }
    //        ],
    //        Fixed = new FhirString(typeName),
    //    };
    //    extSd.Differential.Element.Add(dtValueLiteralEd);


    //}

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
