using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using Fhir.CodeGen.Common.Extensions;
using Fhir.CodeGen.Common.FhirExtensions;
using Fhir.CodeGen.Common.Models;
using Fhir.CodeGen.Common.Packaging;
using Fhir.CodeGen.Common.Utils;
using Fhir.CodeGen.Comparison.Models;
using Fhir.CodeGen.Lib.FhirExtensions;
using Fhir.CodeGen.Lib.Language;
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

public class StructurePageExporter
{
    private readonly XVerExporter _exporter;
    private readonly IDbConnection _db;

    private ILoggerFactory _loggerFactory;
    private ILogger _logger;

    private static readonly HashSet<string> _exportExclusions = [
        "Base",
        "BackboneType",
        "BackboneElement",
        "Element",
        ];

    public StructurePageExporter(
        XVerExporter exporter,
        IDbConnection db,
        ILoggerFactory loggerFactory)
    {
        _exporter = exporter;
        _db = db;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<StructurePageExporter>();
    }

    public void Export(XVerExportTrackingRecord tr)
    {
        // iterate over the XVer IGs
        foreach (XVerIgExportTrackingRecord igTr in tr.XVerIgs)
        {
            // export package structure index page
            exportStructureIndexPage(igTr);

            // export individual structure lookup pages
            exportStructureLookupPages(igTr);
        }
    }

    private void exportStructureLookupPages(XVerIgExportTrackingRecord igTr)
    {
        if (igTr.PageContentDir is null)
        {
            throw new Exception($"IG `{igTr.PackageId}` page content directory not set");
        }

        string dir = igTr.PageContentDir;
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        _logger.LogInformation($"Writing structure lookup pages for `{igTr.PackageId}`...");

        List<XVerIgFileRecord> exported = [];

        // get the structure outcomes for this pair
        List<DbStructureOutcome> sdOutcomes = DbStructureOutcome.SelectList(
            _db,
            SourceFhirPackageKey: igTr.PackagePair.SourcePackageKey,
            TargetFhirPackageKey: igTr.PackagePair.TargetPackageKey,
            orderByProperties: [nameof(DbStructureOutcome.SourceName), nameof(DbStructureOutcome.TargetName)]);

        string sourceBaseUrl = igTr.PackagePair.SourceFhirSequence.ToWebUrlRoot();
        string targetBaseUrl = igTr.PackagePair.TargetFhirSequence.ToWebUrlRoot();

        // iterate over the outcomes to create lookup pages
        foreach (DbStructureOutcome sdOutcome in sdOutcomes)
        {
            if ((sdOutcome.SourceArtifactClass != FhirArtifactClassEnum.Resource) ||
                _exportExclusions.Contains(sdOutcome.SourceId) ||
                _exportExclusions.Contains(sdOutcome.SourceName))
            {
                continue;
            }

            string id = sdOutcome.GenShortId!;
            id = id.StartsWith("profile-", StringComparison.OrdinalIgnoreCase)
                ? id["profile-".Length..]
                : id.StartsWith("profile", StringComparison.OrdinalIgnoreCase)
                ? id["profile".Length..]
                : id.StartsWith("prfl-", StringComparison.OrdinalIgnoreCase)
                ? id["prfl-".Length..]
                : id.StartsWith("prfl", StringComparison.OrdinalIgnoreCase)
                ? id["prfl".Length..]
                : id;

            // create the lookup file
            string filename = Path.Combine(dir, $"lookup-sd-{id}.md");
            using ExportStreamWriter mdWriter = createMarkdownWriter(filename);

            string targetId = sdOutcome.TargetId ?? "Basic";
            //string elementCmId = $"ConceptMap-{igTr.PackagePair.SourcePackageShortName}-{sdOutcome.SourceId}-elements-for-{igTr.PackagePair.TargetPackageShortName}-{targetId}";
            //string elementCmId = $"conceptmap-{igTr.PackagePair.SourcePackageShortName}-{sdOutcome.SourceId}-elements-for-{igTr.PackagePair.TargetPackageShortName}-{targetId}";

            // write a header
            mdWriter.WriteLine(
                $"### Lookup for [FHIR {igTr.PackagePair.SourceFhirSequence}]({sourceBaseUrl})" +
                $" [{sdOutcome.SourceName}]({sourceBaseUrl}{sdOutcome.SourceName}.html)" +
                $" for use in [FHIR {igTr.PackagePair.TargetFhirSequence}]({targetBaseUrl})");
            mdWriter.WriteLine();
            mdWriter.WriteLine(
                $"The FHIR {igTr.PackagePair.SourceFhirSequence} resource is represented in" +
                $" FHIR {igTr.PackagePair.TargetFhirSequence} via the {targetId} resource.");
            mdWriter.WriteLine();

            mdWriter.WriteLine(
                $"Note that there is a profile defined to simplify use of this cross-version resource representation:" +
                $"[Profile: {id}](StructureDefinition-{sdOutcome.GenShortId}.html)");
            mdWriter.WriteLine();

            mdWriter.WriteLine(
                $"A computable version of the following element information is available in:" +
                $" [{sdOutcome.ElementConceptMapName}](ConceptMap-{(sdOutcome.ElementConceptMapFileName![..^4] + "html")})");
            mdWriter.WriteLine();
            mdWriter.WriteLine($"| Source Element (FHIR {igTr.PackagePair.SourceFhirSequence}) | Target(s) |");
            mdWriter.WriteLine("| -------------- | ---- |");

            // get the element outcomes for this structure outcome
            List<DbElementOutcome> edOutcomes = DbElementOutcome.SelectList(
                _db,
                SourceFhirPackageKey: sdOutcome.SourceFhirPackageKey,
                TargetFhirPackageKey: sdOutcome.TargetFhirPackageKey,
                SourceStructureKey: sdOutcome.SourceStructureKey,
                orderByProperties: [nameof(DbElementOutcome.SourceResourceOrder)]);

            Dictionary<int, (string label, string link)> outcomeAccumulator = [];

            HashSet<int> targetedStructureKeys = [];

            // iterate over the element outcomes
            foreach (DbElementOutcome edOutcome in edOutcomes)
            {
                List<string> targetLines = [];

                // get the targets for this outcome
                List<DbElementOutcomeTarget> edTargets = DbElementOutcomeTarget.SelectList(
                    _db,
                    ElementOutcomeKey: edOutcome.Key,
                    orderByProperties: [nameof(DbElementOutcomeTarget.TargetElementId)]);

                foreach (DbElementOutcomeTarget edTarget in edTargets)
                {
                    if (edTarget.TargetStructureKey is not null)
                    {
                        targetedStructureKeys.Add(edTarget.TargetStructureKey.Value);
                    }

                    if (edTarget.TargetElementId is not null)
                    {
                        targetLines.Add($"[{edTarget.TargetElementId}]({targetBaseUrl}{edTarget.TargetElementId.Split('.')[0]}.html#resource)");
                    }
                }

                if (edOutcome.BasicElementEquivalent is not null)
                {
                    string targetLabel;
                    string targetLink;

                    string[] components = edOutcome.BasicElementEquivalent.Split('.');
                    targetLabel = string.Join('.', ["Basic", .. components[1..]]);
                    targetLink = $"{targetBaseUrl}Basic.html#resource";

                    outcomeAccumulator[edOutcome.Key] = (targetLabel, targetLink);
                    targetLines.Add($"[{targetLabel}]({targetLink})");
                }

                if (edOutcome.RequiresComponentDefinition)
                {
                    string targetLabel;
                    string targetLink;

                    targetLabel = edOutcome.ComponentGenUrl!;
                    targetLink = edOutcome.ComponentGenFileName![..^4] + "html";

                    targetLines.Add($"[{targetLabel}]({targetLink})");
                }

                bool isAlternateCanonical = edOutcome.ExtensionSubstitutionUrl == CommonDefinitions.ExtUrlAlternateCanonical;
                bool isAlternateReference = edOutcome.ExtensionSubstitutionUrl == CommonDefinitions.ExtUrlAlternateReference;

                bool additionalAlternateCanonical = !isAlternateCanonical &&
                    (edOutcome.AlternateCanonicalTargetsLiteral is not null);

                bool additionalAlternateReference = !isAlternateReference &&
                    (edOutcome.AlternateReferenceTargetsLiteral is not null);

                if (edOutcome.RequiresXVerDefinition)
                {
                    string targetLabel;
                    string targetLink;

                    if (edOutcome.ExtensionSubstitutionUrl is not null)
                    {
                        targetLabel = edOutcome.ExtensionSubstitutionUrl;
                        targetLink = edOutcome.ExtensionSubstitutionUrl;
                    }
                    else if (edOutcome.ParentRequiresXverDefinition &&
                        (edOutcome.ParentElementOutcomeKey is not null) &&
                        outcomeAccumulator.TryGetValue(edOutcome.ParentElementOutcomeKey.Value, out (string label, string link) parent))
                    {
                        targetLabel = "Extension slice: " + edOutcome.GenUrl!;
                        targetLink = parent.link;
                    }
                    else
                    {
                        targetLabel = edOutcome.GenUrl!;
                        targetLink = edOutcome.GenFileName![..^4] + "html";
                    }

                    outcomeAccumulator[edOutcome.Key] = (targetLabel, targetLink);
                    targetLines.Add($"[{targetLabel}]({targetLink})");
                }

                if (additionalAlternateCanonical)
                {
                    targetLines.Add($"[alternate-canonical]({CommonDefinitions.ExtUrlAlternateCanonical})");
                }

                if (additionalAlternateReference)
                {
                    targetLines.Add($"[alternate-reference]({CommonDefinitions.ExtUrlAlternateReference})");
                }

                mdWriter.WriteLine(
                    $"| [`{edOutcome.SourceId}`]({sourceBaseUrl}{sdOutcome.SourceName}.html#resource)" +
                    $" | {string.Join("<br/>", targetLines)}" +
                    $" |");
            }
            mdWriter.WriteLine("{: .grid }");
            mdWriter.WriteLine();


            // if we have multiple target structures, generate tables for each
            if (targetedStructureKeys.Count > 1)
            {
                foreach (int targetSdKey in targetedStructureKeys)
                {
                    // resolve the target strcutre
                    DbStructureDefinition targetSd = DbStructureDefinition.SelectSingle(
                        _db,
                        Key: targetSdKey)
                        ?? throw new Exception($"Failed to resolve target structure key: {targetSdKey}");

                    bool targetsBasic = targetSd.Name == "Basic";

                    mdWriter.WriteLine();
                    mdWriter.WriteLine($"The following table contains the lookup information specific to when using a FHIR {igTr.PackagePair.TargetFhirSequence} {targetSd.Name}.");
                    mdWriter.WriteLine();
                    mdWriter.WriteLine($"| Source Element (FHIR {igTr.PackagePair.SourceFhirSequence}) | Target(s) |");
                    mdWriter.WriteLine("| -------------- | ---- |");

                    // iterate over the element outcomes
                    foreach (DbElementOutcome edOutcome in edOutcomes)
                    {
                        List<string> targetLines = [];

                        // get the targets for this outcome
                        List<DbElementOutcomeTarget> edTargets = DbElementOutcomeTarget.SelectList(
                            _db,
                            ElementOutcomeKey: edOutcome.Key,
                            TargetStructureKey: targetSdKey,
                            orderByProperties: [nameof(DbElementOutcomeTarget.TargetElementId)]);

                        foreach (DbElementOutcomeTarget edTarget in edTargets)
                        {
                            if (edTarget.TargetElementId is not null)
                            {
                                targetLines.Add($"[{edTarget.TargetElementId}]({targetBaseUrl}{edTarget.TargetElementId.Split('.')[0]}.html#resource)");
                            }
                        }

                        if (targetsBasic &&
                            (edOutcome.BasicElementEquivalent is not null))
                        {
                            string targetLabel;
                            string targetLink;

                            string[] components = edOutcome.BasicElementEquivalent.Split('.');
                            targetLabel = string.Join('.', ["Basic", .. components[1..]]);
                            targetLink = $"{targetBaseUrl}Basic.html#resource";

                            outcomeAccumulator[edOutcome.Key] = (targetLabel, targetLink);
                            targetLines.Add($"[{targetLabel}]({targetLink})");
                        }

                        // only list extensions that can target this
                        if (edOutcome.RequiresXVerDefinition &&
                            edOutcome.ExtensionContexts.Any(ec => ec.StartsWith(targetSd.Name, StringComparison.Ordinal) || (ec == "Element")))
                        {
                            string targetLabel;
                            string targetLink;

                            bool isAlternateCanonical = edOutcome.ExtensionSubstitutionUrl == CommonDefinitions.ExtUrlAlternateCanonical;
                            bool isAlternateReference = edOutcome.ExtensionSubstitutionUrl == CommonDefinitions.ExtUrlAlternateReference;

                            bool additionalAlternateCanonical = !isAlternateCanonical &&
                                (edOutcome.AlternateCanonicalTargetsLiteral is not null);

                            bool additionalAlternateReference = !isAlternateReference &&
                                (edOutcome.AlternateReferenceTargetsLiteral is not null);

                            if (edOutcome.ExtensionSubstitutionUrl is not null)
                            {
                                targetLabel = edOutcome.ExtensionSubstitutionUrl;
                                targetLink = edOutcome.ExtensionSubstitutionUrl;
                            }
                            else if (edOutcome.ParentRequiresXverDefinition &&
                                (edOutcome.ParentElementOutcomeKey is not null) &&
                                outcomeAccumulator.TryGetValue(edOutcome.ParentElementOutcomeKey.Value, out (string label, string link) parent))
                            {
                                targetLabel = "Extension slice: " + edOutcome.GenUrl!;
                                targetLink = parent.link;
                            }
                            else
                            {
                                targetLabel = edOutcome.GenUrl!;
                                targetLink = edOutcome.GenFileName![..^4] + "html";
                            }

                            outcomeAccumulator[edOutcome.Key] = (targetLabel, targetLink);
                            targetLines.Add($"[{targetLabel}]({targetLink})");

                            if (additionalAlternateCanonical)
                            {
                                targetLines.Add($"[alternate-canonical]({CommonDefinitions.ExtUrlAlternateCanonical})");
                            }

                            if (additionalAlternateReference)
                            {
                                targetLines.Add($"[alternate-reference]({CommonDefinitions.ExtUrlAlternateReference})");
                            }
                        }

                        if (targetLines.Count == 0)
                        {
                            targetLines.Add("<i>Not Available</i>");
                        }

                        mdWriter.WriteLine(
                            $"| [`{edOutcome.SourceId}`]({sourceBaseUrl}{sdOutcome.SourceName}.html#resource)" +
                            $" | {string.Join("<br/>", targetLines)}" +
                            $" |");
                    }
                }
                mdWriter.WriteLine("{: .grid }");
                mdWriter.WriteLine();
            }



            // finish writing the file
            mdWriter.Close();

            string fn = Path.GetFileName(filename);
            exported.Add(new()
            {
                FileName = fn,
                FileNameWithoutExtension = fn[..^3],
                IsPageContentFile = true,
                Name = sdOutcome.GenName!,
                Id = null,
                Url = null,
                ResourceType = null,
                Version = null,
                Description = sdOutcome.GenName!,
            });
        }

        _logger.LogInformation($"Wrote {exported.Count} structure lookup pages for `{igTr.PackageId}`");
        igTr.SdPageContentFiles.AddRange(exported);
    }

    private void exportStructureIndexPage(XVerIgExportTrackingRecord igTr)
    {
        if (igTr.PageContentDir is null)
        {
            throw new Exception($"IG `{igTr.PackageId}` page content directory not set");
        }

        string dir = igTr.PageContentDir;
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        _logger.LogInformation($"Writing structure index page for `{igTr.PackageId}`...");

        List<XVerIgFileRecord> exported = [];

        string sourceBaseUrl = igTr.PackagePair.SourceFhirSequence.ToWebUrlRoot();
        string targetBaseUrl = igTr.PackagePair.TargetFhirSequence.ToWebUrlRoot();

        // create the lookup file
        string filename = Path.Combine(dir, "lookup-sd.md");
        using ExportStreamWriter mdWriter = createMarkdownWriter(filename);

        mdWriter.WriteLine($"### FHIR {igTr.PackageId} Cross-Version Artifact Lookup");
        mdWriter.WriteLine();
        mdWriter.WriteLine("The following table links to documentation for the source version of FHIR, for implementers to understand if there is an extension for the element they are trying to use.");
        mdWriter.WriteLine($"These are structures defined in FHIR {igTr.PackagePair.SourceFhirSequence} (the source package), with applicable usage as mapped into FHIR {igTr.PackagePair.TargetFhirSequence} (the target package).");
        mdWriter.WriteLine();

        string cmId = $"ConceptMap-{igTr.PackagePair.SourcePackageShortName}-resources-for-{igTr.PackagePair.TargetPackageShortName}";
        string cmName =
            $"ConceptMap" +
            $"{igTr.PackagePair.SourcePackageShortName.ToPascalCase()}" +
            $"ResourcesFor" +
            $"{igTr.PackagePair.TargetPackageShortName.ToPascalCase()}";

        mdWriter.WriteLine(
            $"A computable version of the following element information is available in:" +
            $" [{cmName}](ConceptMap-{cmId}.html)");
        mdWriter.WriteLine();
        mdWriter.WriteLine(
            $"| {igTr.PackagePair.SourceFhirSequence} Structure" +
            $" | {igTr.PackagePair.TargetFhirSequence} Structure" +
            $" | Lookup File" +
            $" | Profile" +
            $" |");
        mdWriter.WriteLine("| --------- | ----------- | ----------- | ----------- |");

        // get the structure outcomes for this pair
        List<DbStructureOutcome> sdOutcomes = DbStructureOutcome.SelectList(
            _db,
            SourceFhirPackageKey: igTr.PackagePair.SourcePackageKey,
            TargetFhirPackageKey: igTr.PackagePair.TargetPackageKey,
            orderByProperties: [nameof(DbStructureOutcome.SourceName), nameof(DbStructureOutcome.TargetName)]);

        // iterate over the outcomes
        foreach (DbStructureOutcome sdOutcome in sdOutcomes)
        {
            if ((sdOutcome.SourceArtifactClass != FhirArtifactClassEnum.Resource) ||
                _exportExclusions.Contains(sdOutcome.SourceId) ||
                _exportExclusions.Contains(sdOutcome.SourceName))
            {
                continue;
            }

            string targetId = sdOutcome.TargetId ?? "Basic";

            string id = sdOutcome.GenShortId!;
            id = id.StartsWith("profile-", StringComparison.OrdinalIgnoreCase)
                ? id["profile-".Length..]
                : id.StartsWith("profile", StringComparison.OrdinalIgnoreCase)
                ? id["profile".Length..]
                : id.StartsWith("prfl-", StringComparison.OrdinalIgnoreCase)
                ? id["prfl-".Length..]
                : id.StartsWith("prfl", StringComparison.OrdinalIgnoreCase)
                ? id["prfl".Length..]
                : id;

            mdWriter.WriteLine(
                $"| [{igTr.PackagePair.SourceFhirSequence} {sdOutcome.SourceName}]({sourceBaseUrl}{sdOutcome.SourceId}.html)" +
                $" | [{igTr.PackagePair.TargetFhirSequence} {targetId}]({targetBaseUrl}{targetId}.html)" +
                $" | [XVer Lookup: {id}](lookup-sd-{id}.html)" +
                $" | [XVer Profile: {id}](StructureDefinition-{sdOutcome.GenShortId}.html)" +
                $" |");
        }

        mdWriter.WriteLine("{: .grid }");
        mdWriter.WriteLine();

        mdWriter.Close();

        string fn = Path.GetFileName(filename);
        exported.Add(new()
        {
            FileName = fn,
            FileNameWithoutExtension = fn[..^3],
            IsPageContentFile = true,
            Name = "Structure Lookup",
            Id = null,
            Url = null,
            ResourceType = null,
            Version = null,
            Description = "Structure Lookup",
        });

        _logger.LogInformation($"Wrote {exported.Count} structure index pages for `{igTr.PackageId}`");
        igTr.SdPageContentFiles.AddRange(exported);
    }
}
