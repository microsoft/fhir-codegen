using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fhir.CodeGen.Common.FhirExtensions;
using Fhir.CodeGen.Common.Packaging;
using Fhir.CodeGen.Common.Utils;
using Fhir.CodeGen.Comparison.Models;
using Fhir.CodeGen.Lib.FhirExtensions;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Utility;
using Microsoft.Extensions.Logging;
using Octokit;
using static Fhir.CodeGen.Comparison.Exporter.IgExporter;

namespace Fhir.CodeGen.Comparison.Exporter;

public class VocabularyExporter
{
    private readonly XVerExporter _exporter;
    private readonly IDbConnection _db;

    private ILoggerFactory _loggerFactory;
    private ILogger _logger;

    public VocabularyExporter(
        XVerExporter exporter,
        IDbConnection db,
        ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<VocabularyExporter>();
        _db = db;
        _exporter = exporter;
    }

    public void Export(XVerExportTrackingRecord tr)
    {
        // iterate over the XVer IGs
        foreach (XVerIgExportTrackingRecord igTr in tr.XVerIgs)
        {
            // export code systems
            exportCodeSystems(igTr);

            // export value sets
            exportValueSets(igTr);

            // export concept maps
        }
    }

    private void exportValueSets(XVerIgExportTrackingRecord igTr)
    {
        if (igTr.VocabularyDir is null)
        {
            throw new Exception("VocabularyDir is null");
        }

        string dir = igTr.VocabularyDir;
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        _logger.LogInformation($"Writing Value Sets for `{igTr.PackageId}`...");

        List<XVerIgFileRecord> exported = [];

        // write the relevant externally-included value sets
        foreach (DbExternalInclusion inclusion in DbExternalInclusion.SelectEnumerable(_db, ResourceType: Hl7.Fhir.Model.FHIRAllTypes.ValueSet))
        {
            if ((inclusion.IncludeInPackages is not null) &&
                !inclusion.GetIncludeInPackagesList().Contains(igTr.PackageId, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            // write the code system to a file
            string filename = $"ValueSet-{inclusion.Id}.json";
            string path = Path.Combine(dir, filename);
            File.WriteAllText(path, inclusion.Json);

            exported.Add(new()
            {
                FileName = filename,
                FileNameWithoutExtension = filename[..^5],
                IsPageContentFile = false,
                Name = inclusion.Name,
                Id = inclusion.Id,
                Url = inclusion.UnversionedUrl,
                ResourceType = Hl7.Fhir.Model.FHIRAllTypes.ValueSet.GetLiteral(),
                Version = inclusion.Version,
                Description = $"Externally-defined ValueSet: {inclusion.Name}",
            });
        }

        // get the value sets in the source package
        Dictionary<int, DbValueSet> sourceValueSets = DbValueSet.SelectDict(
            _db,
            FhirPackageKey: igTr.PackagePair.SourcePackageKey);

        // get the value sets in the target package
        Dictionary<int, DbValueSet> targetValueSets = DbValueSet.SelectDict(
            _db,
            FhirPackageKey: igTr.PackagePair.TargetPackageKey);

        // get the value set concepts in the source package
        Dictionary<int, DbValueSetConcept> sourceConcepts = DbValueSetConcept.SelectDict(
            _db,
            FhirPackageKey: igTr.PackagePair.SourcePackageKey);

        // get the value set comparisons for this package pair
        Dictionary<int, DbValueSetComparison> vsComparisons = DbValueSetComparison.SelectDict(
            _db,
            SourceFhirPackageKey: igTr.PackagePair.SourcePackageKey,
            TargetFhirPackageKey: igTr.PackagePair.TargetPackageKey);

        // get the value set outcomes for this package pair that need exporting
        List<DbValueSetOutcome> vsOutcomes = DbValueSetOutcome.SelectList(
            _db,
            SourceFhirPackageKey: igTr.PackagePair.SourcePackageKey,
            TargetFhirPackageKey: igTr.PackagePair.TargetPackageKey,
            RequiresXVerDefinition: true,
            orderByProperties: [nameof(DbValueSetOutcome.GenLongId)]);

        // iterate over the value set outcomes we need to export
        foreach (DbValueSetOutcome vsOutcome in vsOutcomes)
        {
            if (!sourceValueSets.TryGetValue(vsOutcome.SourceValueSetKey, out DbValueSet? sourceVs))
            {
                _logger.LogError($"Could not resolve source ValueSet `{vsOutcome.SourceCanonicalVersioned}`");
                continue;
            }

            DbValueSet? targetVs = null;
            if ((vsOutcome.TargetValueSetKey is not null) &&
                !targetValueSets.TryGetValue(vsOutcome.TargetValueSetKey.Value, out targetVs))
            {
                _logger.LogError($"Could not resolve target ValueSet `{vsOutcome.TargetCanonicalVersioned}`");
                continue;
            }

            // get the concept outcomes for this vs outcome that need a definition
            List<DbValueSetConceptOutcome> conceptOutcomes = DbValueSetConceptOutcome.SelectList(
                _db,
                ValueSetOutcomeKey: vsOutcome.Key,
                RequiresXVerDefinition: true,
                orderByProperties: [nameof(DbValueSetConceptOutcome.SourceSystem), nameof(DbValueSetConceptOutcome.SourceCode)]);

            if (conceptOutcomes.Count == 0)
            {
                // nothing to include
                continue;
            }

            List<DbValueSetConceptOutcome> nonGeneratedConceptOutcomes = DbValueSetConceptOutcome.SelectList(
                _db,
                ValueSetOutcomeKey: vsOutcome.Key,
                RequiresXVerDefinition: false,
                orderByProperties: [nameof(DbValueSetConceptOutcome.SourceSystem), nameof(DbValueSetConceptOutcome.SourceCode)]);

            List<DbElement> sourceVsBoundElements = DbElement.SelectList(
                _db,
                BindingValueSetKey: sourceVs.Key,
                orderByProperties: [nameof(DbElement.Id)]);

            string? purpose = null;
            if (igTr.PackagePair.Distance == 1)
            {
                purpose = $$$"""
                    This value set is part of the cross-version definitions generated to enable use of the
                    value set `{{{sourceVs.VersionedUrl}}}` as defined in FHIR {{{igTr.PackagePair.SourceFhirSequence}}}
                    in FHIR {{{igTr.PackagePair.TargetFhirSequence}}}.

                    The source value set is bound to the following FHIR {{{igTr.PackagePair.SourceFhirSequence}}} elements:
                    * {{{string.Join("\n* ", sourceVsBoundElements.Select(ed => $"`{ed.Id}`"))}}}

                    The following concepts are not included in this cross-version definition because they have valid representations
                    * {{{(nonGeneratedConceptOutcomes.Count == 0
                        ? "_no concepts_"
                        : string.Join("\n* ", nonGeneratedConceptOutcomes.Select(nco => $"`{nco.SourceSystem}#{nco.SourceCode}`")))}}}

                    Following are the generation technical comments:
                    {{{vsOutcome.Comments}}}
                    """;
            }
            else
            {
                string? mappingTrace = null;

                if (vsComparisons.TryGetValue(vsOutcome.ComparisonKey, out DbValueSetComparison? vsComp))
                {
                    DbValueSet? r2Vs = vsComp.ContentKeyR2 is null
                        ? null
                        : DbValueSet.SelectSingle(_db, Key: vsComp.ContentKeyR2.Value);
                    DbValueSet? r3Vs = vsComp.ContentKeyR3 is null
                        ? null
                        : DbValueSet.SelectSingle(_db, Key: vsComp.ContentKeyR3.Value);
                    DbValueSet? r4Vs = vsComp.ContentKeyR4 is null
                        ? null
                        : DbValueSet.SelectSingle(_db, Key: vsComp.ContentKeyR4.Value);
                    DbValueSet? r4bVs = vsComp.ContentKeyR4B is null
                        ? null
                        : DbValueSet.SelectSingle(_db, Key: vsComp.ContentKeyR4B.Value);
                    DbValueSet? r5Vs = vsComp.ContentKeyR5 is null
                        ? null
                        : DbValueSet.SelectSingle(_db, Key: vsComp.ContentKeyR5.Value);
                    DbValueSet? r6Vs = vsComp.ContentKeyR6 is null
                        ? null
                        : DbValueSet.SelectSingle(_db, Key: vsComp.ContentKeyR6.Value);

                    List<DbValueSet?> contentPath = (igTr.PackagePair.SourceFhirSequence < igTr.PackagePair.TargetFhirSequence)
                        ? [r2Vs, r3Vs, r4Vs, r4bVs, r5Vs, r6Vs]
                        : [r6Vs, r5Vs, r4bVs, r4Vs, r3Vs, r2Vs];

                    mappingTrace = "* " + string.Join("\n* ", contentPath.Where(vs => vs is not null).Select(vs => $"`{vs!.VersionedUrl}`"));
                }

                purpose = $$$"""
                    This value set is part of the cross-version definitions generated to enable use of the
                    value set `{{{sourceVs.VersionedUrl}}}` as defined in FHIR {{{igTr.PackagePair.SourceFhirSequence}}}
                    in FHIR {{{igTr.PackagePair.TargetFhirSequence}}}.

                    The source value set is bound to the following FHIR {{{igTr.PackagePair.SourceFhirSequence}}} elements:
                    * {{{string.Join("\n* ", sourceVsBoundElements.Select(ed => $"`{ed.Id}` as {ed.ValueSetBindingStrength}"))}}}

                    Across FHIR versions, the value set has been mapped as:
                    {{{mappingTrace}}}

                    The following concepts are not included in this cross-version definition because they have valid representations
                    * {{{(nonGeneratedConceptOutcomes.Count == 0
                        ? "_no concepts_"
                        : string.Join("\n* ", nonGeneratedConceptOutcomes.Select(nco => $"`{nco.SourceSystem}#{nco.SourceCode}`")))}}}

                    Following are the generation technical comments:
                    {{{vsOutcome.Comments}}}
                    """;
            }

            // create our vs
            ValueSet fhirVs = new()
            {
                Url = $"http://hl7.org/fhir/{igTr.PackagePair.SourceFhirVersionShort}/ValueSet/{vsOutcome.GenLongId}",
                Id = vsOutcome.GenLongId,
                Version = _exporter._crossDefinitionVersion,
                Name = FhirSanitizationUtils.ReformatIdForName(vsOutcome.GenLongId!),
                Title = $"Cross-version ValueSet {igTr.PackagePair.SourceFhirSequence}.{sourceVs.Name} for use in FHIR {igTr.PackagePair.TargetFhirSequence}",
                Status = PublicationStatus.Active,
                Experimental = false,
                UseContext = sourceVs.UseContexts,
                Jurisdiction = sourceVs.Jurisdictions,
                DateElement = new FhirDateTime(_exporter._runTime),
                Description = (targetVs is null)
                    ? $"This cross-version ValueSet represents content from `{sourceVs.VersionedUrl}` for use in FHIR {igTr.PackagePair.TargetFhirSequence}."
                    : $"This cross-version ValueSet represents content from {sourceVs.VersionedUrl} for use in FHIR {igTr.PackagePair.TargetFhirSequence}" +
                        $" that is appropriate for use but unavailable in `{targetVs.VersionedUrl}`.",
                Purpose = purpose,
                Compose = new()
                {
                    Include = [],
                },
                Expansion = new()
                {
                    TimestampElement = new FhirDateTime(_exporter._runTime),
                    Contains = [],
                },
            };

            // check to see if we should set various root extensions
            if (sourceVs.FhirMaturity != null)
            {
                fhirVs.AddExtension(CommonDefinitions.ExtUrlFmm, new Integer(sourceVs.FhirMaturity));
            }

            // FHIR-I is the default WG responsible if none are specified
            string wg = CommonDefinitions.ResolveWorkgroup(sourceVs.WorkGroup, "fhir");

            // add the work group extension
            fhirVs.AddExtension(CommonDefinitions.ExtUrlWorkGroup, new Hl7.Fhir.Model.Code(wg));

            // ensure there is a publisher, use the WG if there is none
            fhirVs.Publisher = CommonDefinitions.WorkgroupNames[wg];

            // ensure there is a contact point - use the default WG unless there are multiple entries
            if ((fhirVs.Contact == null) || (fhirVs.Contact.Count < 2))
            {
                fhirVs.Contact = [
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

            // check for unexpandable value sets (are stuck using the compose - even though it is likely incorrect)
            if ((sourceVs.CanExpand == false) ||
                (sourceVs.ActiveConcreteConceptCount == 0))
            {
                // use the existing compose
                fhirVs.Compose = sourceVs.Compose;

                // will not have an expansion
                fhirVs.Expansion = null;
            }
            else
            {
                Dictionary<string, ValueSet.ConceptSetComponent> composeIncludes = [];

                // traverse concepts
                foreach (DbValueSetConceptOutcome conceptOutcome in conceptOutcomes)
                {
                    // resolve this concept
                    if (!sourceConcepts.TryGetValue(conceptOutcome.SourceValueSetConceptKey, out DbValueSetConcept? concept))
                    {
                        throw new Exception($"Failed to resolve concept with key {conceptOutcome.SourceValueSetConceptKey} for ValueSet outcome {vsOutcome.Key}!");
                    }

                    string composeKey = concept.System + "|" + concept.SystemVersion;

                    if (!composeIncludes.TryGetValue(composeKey, out ValueSet.ConceptSetComponent? composeInclude))
                    {
                        // create a new include for this concept
                        composeInclude = new()
                        {
                            System = concept.System,
                            Version = concept.SystemVersion,
                            Concept = [],
                        };
                        composeIncludes.Add(composeKey, composeInclude);
                        fhirVs.Compose.Include.Add(composeInclude);
                    }

                    composeInclude.Concept.Add(new()
                    {
                        Code = concept.Code,
                        Display = concept.Display,
                    });

                    // add this concept to the expansion
                    fhirVs.Expansion.Contains.Add(new()
                    {
                        System = concept.System,
                        Version = concept.SystemVersion,
                        Code = concept.Code,
                        Display = concept.Display,
                    });
                }

                // add the compose includes to the value set
                fhirVs.Compose.Include = composeIncludes.Values.ToList();

                // if we have no concepts, do not write this value set
                if (composeIncludes.Count == 0)
                {
                    continue;
                }
            }

            fhirVs.cgAddPackageSource(igTr.PackageId, _exporter._crossDefinitionVersion, igTr.PackageUrl);

            // write the code system to a file
            string filename = $"ValueSet-{fhirVs.Id}.json";
            string path = Path.Combine(dir, filename);
            File.WriteAllText(path, fhirVs.ToJson(new FhirJsonSerializationSettings() { Pretty = true }));

            exported.Add(new()
            {
                FileName = filename,
                FileNameWithoutExtension = filename[..^5],
                IsPageContentFile = false,
                Name = fhirVs.Name,
                Id = fhirVs.Id,
                Url = fhirVs.Url,
                ResourceType = Hl7.Fhir.Model.FHIRAllTypes.ValueSet.GetLiteral(),
                Version = fhirVs.Version,
                Description = fhirVs.Description ?? fhirVs.Title ?? $"ValueSet: {fhirVs.Url}|{fhirVs.Version}",
            });
        }

        _logger.LogInformation($"Wrote {exported.Count} Value Sets for `{igTr.PackageId}`");
        igTr.ValueSetFiles = exported;
    }

    private void exportCodeSystems(XVerIgExportTrackingRecord igTr)
    {
        if (igTr.VocabularyDir is null)
        {
            throw new Exception("VocabularyDir is null");
        }

        string dir = igTr.VocabularyDir;
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        _logger.LogInformation($"Writing Code Systems for `{igTr.PackageId}`...");

        List<XVerIgFileRecord> exported = [];

        // write the relevant externally-included code systems
        foreach (DbExternalInclusion inclusion in DbExternalInclusion.SelectEnumerable(_db, ResourceType: Hl7.Fhir.Model.FHIRAllTypes.CodeSystem))
        {
            if ((inclusion.IncludeInPackages is not null) &&
                !inclusion.GetIncludeInPackagesList().Contains(igTr.PackageId, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            // write the code system to a file
            string filename = $"CodeSystem-{inclusion.Id}.json";
            string path = Path.Combine(dir, filename);
            File.WriteAllText(path, inclusion.Json);

            exported.Add(new()
            {
                FileName = filename,
                FileNameWithoutExtension = filename[..^5],
                IsPageContentFile = false,
                Name = inclusion.Name,
                Id = inclusion.Id,
                Url = inclusion.UnversionedUrl,
                ResourceType = Hl7.Fhir.Model.FHIRAllTypes.CodeSystem.GetLiteral(),
                Version = inclusion.Version,
                Description = $"Externally-defined CodeSystem: {inclusion.Name}",
            });
        }

        // get the list of code systems in the source sourcePackage
        List<DbCodeSystem> codeSystems = DbCodeSystem.SelectList(
            _db,
            FhirPackageKey: igTr.PackagePair.SourcePackageKey);

        // iterate over the code systems to them
        foreach (DbCodeSystem dbCs in codeSystems)
        {
            // create the FHIR CodeSystem
            CodeSystem fhirCs = new()
            {
                Id = dbCs.Id,
                Url = dbCs.UnversionedUrl,
                Name = dbCs.Name,
                Version = dbCs.Version,
                VersionAlgorithm =
                    (dbCs.VersionAlgorithmString != null)
                    ? new FhirString(dbCs.VersionAlgorithmString)
                    : (dbCs.VersionAlgorithmCoding != null)
                    ? dbCs.VersionAlgorithmCoding
                    : null,
                Status = dbCs.Status,
                Title = dbCs.Title,
                Description = dbCs.Description,
                Purpose = dbCs.Purpose,
                Text = dbCs.Narrative,
                Experimental = dbCs.IsExperimental,
                DateElement = (dbCs.LastChangedDate != null) ? new FhirDateTime(dbCs.LastChangedDate.Value) : null,
                Publisher = dbCs.Publisher,
                Copyright = dbCs.Copyright,
                CopyrightLabel = dbCs.CopyrightLabel,
                ApprovalDate = dbCs.ApprovalDate,
                LastReviewDate = dbCs.LastReviewDate,
                EffectivePeriod = (dbCs.EffectivePeriodStart != null || dbCs.EffectivePeriodEnd != null)
                    ? new Period()
                    {
                        StartElement = (dbCs.EffectivePeriodStart != null) ? new FhirDateTime(dbCs.EffectivePeriodStart.Value) : null,
                        EndElement = (dbCs.EffectivePeriodEnd != null) ? new FhirDateTime(dbCs.EffectivePeriodEnd.Value) : null,
                    }
                    : null,
                Topic = dbCs.Topic,
                RelatedArtifact = dbCs.RelatedArtifacts,
                Jurisdiction = dbCs.Jurisdictions,
                UseContext = dbCs.UseContexts,
                Contact = dbCs.Contacts,
                Author = dbCs.Authors,
                Editor = dbCs.Editors,
                Reviewer = dbCs.Reviewers,
                CaseSensitive = dbCs.IsCaseSensitive,
                ValueSet = dbCs.ValueSetVersioned,
                HierarchyMeaning = dbCs.HierarchyMeaning,
                Compositional = dbCs.IsCompositional,
                VersionNeeded = dbCs.VersionNeeded,
                Content = dbCs.Content,
                Supplements = dbCs.SupplementsVersioned,
                Count = dbCs.Count,
            };

            // remove pre-R5 elements if we are in an earlier version
            if (igTr.PackagePair.TargetFhirSequence < FhirReleases.FhirSequenceCodes.R5)
            {
                fhirCs.ApprovalDate = null;
                fhirCs.LastReviewDate = null;
                fhirCs.EffectivePeriod = null;
                fhirCs.Topic = null;
                fhirCs.Author = null;
                fhirCs.Editor = null;
                fhirCs.Reviewer = null;
                fhirCs.RelatedArtifact = null;
            }

            string? wg = null;

            // add standard extensions
            if (dbCs.RootExtensions != null)
            {
                foreach (Hl7.Fhir.Model.Extension ext in dbCs.RootExtensions)
                {
                    switch (ext.Url)
                    {
                        case CommonDefinitions.ExtUrlWorkGroup:
                            {
                                switch (ext.Value)
                                {
                                    case FhirString fhirString:
                                        wg = fhirString.Value;
                                        break;
                                    case Hl7.Fhir.Model.Code code:
                                        wg = code.Value;
                                        break;
                                    case Markdown markdown:
                                        wg = markdown.Value;
                                        break;
                                    default:
                                        continue;
                                }
                            }
                            break;

                        case CommonDefinitions.ExtUrlPackageSource:
                            fhirCs.cgAddPackageSource(igTr.PackageId, _exporter._crossDefinitionVersion, igTr.PackageUrl);
                            break;

                        default:
                            // copy any extensions we have not specifically handled
                            fhirCs.Extension.Add(ext);
                            break;
                    }
                }
            }

            // default to fhir infrastructure work group if none is present
            wg = CommonDefinitions.ResolveWorkgroup(wg, "fhir");

            // add the work group extension
            fhirCs.AddExtension(CommonDefinitions.ExtUrlWorkGroup, new Hl7.Fhir.Model.Code(wg));

            // ensure the publisher matches the WG
            fhirCs.Publisher = CommonDefinitions.WorkgroupNames[wg];

            // ensure there is a contact point - use the default WG unless there are multiple entries
            if ((fhirCs.Contact == null) || (fhirCs.Contact.Count < 2))
            {
                fhirCs.Contact = [
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

            // add filters
            List<DbCodeSystemFilter> csFilters = DbCodeSystemFilter.SelectList(
                _db,
                CodeSystemKey: dbCs.Key);

            foreach (DbCodeSystemFilter dbFilter in csFilters)
            {
                fhirCs.Filter.Add(new CodeSystem.FilterComponent()
                {
                    Code = dbFilter.Code,
                    Description = dbFilter.Description,
                    Operator = dbFilter.Operators.Split('|').Select(op => EnumUtility.ParseLiteral<FilterOperator>(op, true)).ToList(),
                    Value = dbFilter.Value,
                });
            }

            // add property definitions
            List<DbCodeSystemPropertyDefinition> csPropertyDefinitions = DbCodeSystemPropertyDefinition.SelectList(
                _db,
                CodeSystemKey: dbCs.Key);

            foreach (DbCodeSystemPropertyDefinition dbPropDef in csPropertyDefinitions)
            {
                fhirCs.Property.Add(new CodeSystem.PropertyComponent()
                {
                    Code = dbPropDef.Code,
                    Uri = dbPropDef.Uri,
                    Description = dbPropDef.Description,
                    Type = dbPropDef.Type,
                });
            }

            // recursively add concepts
            addDbCodeSystemConcepts(fhirCs.Concept, dbCs.Key);

            // write the code system to a file
            string filename = $"CodeSystem-{fhirCs.Id}.json";
            string path = Path.Combine(dir, filename);
            File.WriteAllText(path, fhirCs.ToJson(new FhirJsonSerializationSettings() { Pretty = true }));

            exported.Add(new()
            {
                FileName = filename,
                FileNameWithoutExtension = filename[..^5],
                IsPageContentFile = false,
                Name = fhirCs.Name,
                Id = fhirCs.Id,
                Url = fhirCs.Url,
                ResourceType = Hl7.Fhir.Model.FHIRAllTypes.CodeSystem.GetLiteral(),
                Version = fhirCs.Version,
                Description = fhirCs.Description ?? fhirCs.Title ?? $"CodeSystem: {fhirCs.Url}|{fhirCs.Version}",
            });
        }

        _logger.LogInformation($"Wrote {exported.Count} Code Systems for `{igTr.PackageId}`");

        igTr.CodeSystemFiles = exported;
    }

    private void addDbCodeSystemConcepts(
        List<CodeSystem.ConceptDefinitionComponent> concepts,
        int dbCsKey,
        int? parentConceptKey = null)
    {
        List<DbCodeSystemConcept> dbConcepts = (parentConceptKey == null)
            ? DbCodeSystemConcept.SelectList(
                _db,
                CodeSystemKey: dbCsKey,
                ParentConceptKeyIsNull: true,
                orderByProperties: [nameof(DbCodeSystemConcept.FlatOrder)])
            : DbCodeSystemConcept.SelectList(
                _db,
                CodeSystemKey: dbCsKey,
                ParentConceptKey: parentConceptKey.Value,
                orderByProperties: [nameof(DbCodeSystemConcept.FlatOrder)]);

        foreach (DbCodeSystemConcept dbConcept in dbConcepts)
        {
            // create the concept
            CodeSystem.ConceptDefinitionComponent fhirConcept = new CodeSystem.ConceptDefinitionComponent()
            {
                Code = dbConcept.Code,
                Display = dbConcept.Display,
                Definition = dbConcept.Definition,
                Designation = dbConcept.Designations,
                Property = dbConcept.Properties,
            };

            concepts.Add(fhirConcept);

            // recursively add child concepts
            if (dbConcept.ChildConceptCount != 0)
            {
                addDbCodeSystemConcepts(fhirConcept.Concept, dbCsKey, dbConcept.Key);
            }
        }
    }
}
