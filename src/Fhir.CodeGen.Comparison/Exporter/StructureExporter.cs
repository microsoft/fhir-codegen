using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fhir.CodeGen.Common.FhirExtensions;
using Fhir.CodeGen.Common.Models;
using Fhir.CodeGen.Common.Packaging;
using Fhir.CodeGen.Common.Utils;
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

namespace Fhir.CodeGen.Comparison.Exporter;

public class StructureExporter
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

    public StructureExporter(
        XVerExporter exporter,
        IDbConnection db,
        ILoggerFactory loggerFactory)
    {
        _exporter = exporter;
        _db = db;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<StructureExporter>();
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

            // export type maps (maybe?)

            // export resource maps

            // export element maps
        }
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

                //// if the differential is empty, skip this profile
                //if (profileSd.Differential.Element.Count == 0)
                //{
                //    continue;
                //}

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
                    ElementId = $"{targetId}:{targetEdOutcome.SourceName}",
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

        _logger.LogInformation($"Writing Extensions for `{igTr.PackageId}`...");

        List<XVerIgFileRecord> exported = [];

        // get the structures defined in the source package
        Dictionary<int, DbStructureDefinition> sourceStructures = DbStructureDefinition.SelectDict(
            _db,
            FhirPackageKey: igTr.PackagePair.SourcePackageKey);

        // get the elements defined in the source package
        Dictionary<int, DbElement> sourceElements = DbElement.SelectDict(
            _db,
            FhirPackageKey: igTr.PackagePair.SourcePackageKey);

        // get the structures defined in the target package
        Dictionary<int, DbStructureDefinition> targetStructures = DbStructureDefinition.SelectDict(
            _db,
            FhirPackageKey: igTr.PackagePair.TargetPackageKey);

        // get the elements defined in the target package
        Dictionary<int, DbElement> targetElements = DbElement.SelectDict(
            _db,
            FhirPackageKey: igTr.PackagePair.TargetPackageKey);

        // get the element comparisons for the package pair
        Dictionary<int, DbElementComparison> elementComparisons = DbElementComparison.SelectDict(
            _db,
            SourceFhirPackageKey: igTr.PackagePair.SourcePackageKey,
            TargetFhirPackageKey: igTr.PackagePair.TargetPackageKey);

        // get the element outcomes for this package pair that need exporting
        List<DbElementOutcome> elementOutcomes = DbElementOutcome.SelectList(
            _db,
            SourceFhirPackageKey: igTr.PackagePair.SourcePackageKey,
            TargetFhirPackageKey: igTr.PackagePair.TargetPackageKey,
            RequiresXVerDefinition: true,
            ExtensionSubstitutionKeyIsNull: true,
            ParentElementOutcomeKeyIsNull: true,
            orderByProperties: [nameof(DbElementOutcome.SourceStructureKey), nameof(DbElementOutcome.SourceResourceOrder)]);

        // iterate over the outcomes that need exporting
        foreach (DbElementOutcome elementOutcome in elementOutcomes)
        {
            // get the source structure
            if (!sourceStructures.TryGetValue(elementOutcome.SourceStructureKey, out DbStructureDefinition? sourceSd))
            {
                _logger.LogError($"Source structure with key `{elementOutcome.SourceStructureKey}` not found for element outcome with key `{elementOutcome.Key}`");
                continue;
            }

            if (_exportExclusions.Contains(elementOutcome.SourceId) ||
                _exportExclusions.Contains(sourceSd.Name))
            {
                continue;
            }

            DbStructureOutcome? sdOutcome = null;
            if (elementOutcome.SourceResourceOrder == 0)
            {
                sdOutcome = DbStructureOutcome.SelectSingle(
                    _db,
                    SourceFhirPackageKey: igTr.PackagePair.SourcePackageKey,
                    TargetFhirPackageKey: igTr.PackagePair.TargetPackageKey,
                    SourceStructureKey: sourceSd.Key,
                    TargetStructureKey: elementOutcome.TargetStructureKey);
            }

            // get the source element
            if (!sourceElements.TryGetValue(elementOutcome.SourceElementKey, out DbElement? sourceEd))
            {
                _logger.LogError($"Source element with key `{elementOutcome.SourceElementKey}` not found for element outcome with key `{elementOutcome.Key}`");
                continue;

            }

            List<StructureDefinition.ContextComponent> contexts = elementOutcome.ExtensionContexts
                .Select(c => new StructureDefinition.ContextComponent()
                {
                    Type = StructureDefinition.ExtensionContextType.Element,
                    Expression = c,
                })
                .ToList();


            string? purpose = null;
            if (igTr.PackagePair.Distance == 1)
            {
                purpose = $$$"""
                    This extension is part of the cross-version definitions generated to enable use of the
                    element `{{{sourceEd.Id}}}` as defined in FHIR {{{igTr.PackagePair.SourceFhirSequence}}}
                    in FHIR {{{igTr.PackagePair.TargetFhirSequence}}}.

                    The source element is defined as:
                    `{{{sourceEd.Id}}}` {{{sourceEd.FhirCardinalityString}}} `{{{sourceEd.FullCollatedTypeLiteral}}}`

                    Following are the generation technical comments:
                    {{{elementOutcome.Comments}}}
                    """;
            }
            else
            {
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

                purpose = $$$"""
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

            // build the initial structure definition for the extension
            StructureDefinition extSd = new()
            {
                Id = sdOutcome?.GenShortId ?? elementOutcome.GenShortId,
                Url = sdOutcome?.GenUrl ?? elementOutcome.GenUrl,
                Name = FhirSanitizationUtils.ReformatIdForName(sdOutcome?.GenLongId ?? elementOutcome.GenLongId!),
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
                elementOutcome,
                sourceSd,
                sourceEd,
                sourceElements,
                targetElements,
                elementComparisons);

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

    private void addToDifferentialRecursive(
        StructureDefinition extSd,
        XVerIgExportTrackingRecord igTr,
        DbElementOutcome edOutcome,
        DbStructureDefinition sourceSd,
        DbElement sourceEd,
        Dictionary<int, DbElement> sourceElements,
        Dictionary<int, DbElement> targetElements,
        Dictionary<int, DbElementComparison> elementComparisons,
        string? extElementId = null,
        string? extElementPath = null)
    {
        extElementId ??= "Extension";
        extElementPath ??= "Extension";

        bool isRoot = edOutcome.ParentElementOutcomeKey is null;

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
        else
        {
            // add the primary slice
            ElementDefinition sliceElement = new()
            {
                ElementId = extElementId,
                Path = extElementPath,
                SliceName = edOutcome.GenShortId,
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

        // get the child outcomes so we know if there are child elements to process
        List<DbElementOutcome> childOutcomes = DbElementOutcome.SelectList(
            _db,
            ParentElementOutcomeKey: edOutcome.Key,
            orderByProperties: [nameof(DbElementOutcome.SourceResourceOrder)]);

        bool hasChildren = childOutcomes.Count > 0;

        // get the unmapped source types so we can determine value[x] types vs. _datatype extension slices
        List<DbElementType> unmappedSourceTypes = (hasChildren || edOutcome.UnmappedTypeKeysLiteral is null)
            ? []
            : DbElementType.SelectList(
                _db,
                KeyValues: edOutcome.UnmappedTypeKeys);

        Dictionary<int, DbElementType> validSourceTypes = [];
        Dictionary<int, DbElementType> invalidSourceTypes = [];
        foreach (DbElementType et in unmappedSourceTypes)
        {
            string typeName = et.TypeName ?? et.Literal;

            if (_extensionValueTypeNames[igTr.PackagePair.TargetFhirSequence].Contains(typeName))
            {
                // TODO: need to also check type and target profiles
                validSourceTypes[et.Key] = et;
            }
            else
            {
                invalidSourceTypes[et.Key] = et;
            }
        }

        bool needsValueElement = validSourceTypes.Count > 0;
        bool needsExtensionElement = hasChildren || (invalidSourceTypes.Count > 0);

        int extensionMinCardinality = childOutcomes.Sum(eo => eo.SourceMinCardinality);
        if (invalidSourceTypes.Count > 0)
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

            addToDifferentialRecursive(
                extSd,
                igTr,
                childOutcome,
                sourceSd,
                childSourceEd,
                sourceElements,
                targetElements,
                elementComparisons,
                extElementId + ".extension:" + childOutcome.GenShortId,
                extElementPath + ".extension:" + childOutcome.GenShortId);
        }

        ElementDefinition? dtValueEd = null;

        // add any invalid source types as extension slices with the _datatype extension
        foreach ((int invalidTypeKey, DbElementType et) in invalidSourceTypes)
        {
            string typeName = et.TypeName ?? et.Literal;

            // add the _datatype extension slice
            ElementDefinition dtExtensionEd = new()
            {
                ElementId = extElementId + ".extension:_datatype",
                Path = extElementPath + ".extension",
                SliceName = "_datatype",
                Short = $"Data type name for unrepresentable types in {igTr.PackagePair.SourceFhirSequence} `{sourceEd.Id}`",
                Definition = $"Data type name for unrepresentable types in {igTr.PackagePair.SourceFhirSequence} `{sourceEd.Id}`",
                Min = 0,
                Max = "1",
                Base = new()
                {
                    Path = "Extension.extension",
                    Min = 0,
                    Max = "*",
                },
                Type = [
                    new()
                    {
                        Code = "Extension",
                        Profile = ["http://hl7.org/fhir/StructureDefinition/_datatype"],
                    }
                ],
            };

            extSd.Differential.Element.Add(dtExtensionEd);

            // add the url element
            ElementDefinition dtUrlEd = new()
            {
                ElementId = extElementId + ".extension:_datatype.url",
                Path = extElementPath + ".extension.url",
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

            extSd.Differential.Element.Add(dtUrlEd);

            // add or update the value element
            if (dtValueEd is null)
            {
                dtValueEd = new()
                {
                    ElementId = extElementId + ".extension:_datatype.value[x]",
                    Path = extElementPath + ".extension.value[x]",
                    Comment = $"Must be: {typeName}",
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
                    Fixed = new FhirString(typeName),
                };

                extSd.Differential.Element.Add(dtValueEd);
            }
            else
            {
                dtValueEd.Comment += $"|{typeName}";
                dtValueEd.Fixed = null;
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
            Fixed = new FhirUri(edOutcome.GenUrl),
        });

        // add the value element - either with proper types or constrained to zero repetitions
        if (validSourceTypes.Count == 0)
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

            // add the types
            Dictionary<string, ElementDefinition.TypeRefComponent> typeRefs = [];
            foreach ((int validTypeKey, DbElementType et) in validSourceTypes)
            {
                string typeName = et.TypeName ?? et.Literal;

                if (!typeRefs.TryGetValue(typeName, out ElementDefinition.TypeRefComponent? tr))
                {
                    tr = new()
                    {
                        Code = typeName,
                    };
                    typeRefs[typeName] = tr;
                }

                if (et.TypeProfile is not null)
                {
                    tr.ProfileElement ??= [];
                    tr.ProfileElement.Add(et.TypeProfile);
                }

                if (et.TargetProfile is not null)
                {
                    tr.TargetProfileElement ??= [];
                    tr.TargetProfileElement.Add(et.TargetProfile);
                }
            }

            etValueEd.Type = typeRefs.Values.OrderBy(tr => tr.TypeName).ToList();

            // add our element
            extSd.Differential.Element.Add(etValueEd);
        }
    }
}
