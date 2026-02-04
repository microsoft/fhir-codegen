using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
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

            // create the lookup file
            string filename = Path.Combine(dir, $"lookup-sd-{sdOutcome.GenLongId}.md");
            using ExportStreamWriter mdWriter = createMarkdownWriter(filename);

            string targetId = sdOutcome.TargetId ?? "Basic";
            string elementCmId = $"ConceptMap-{igTr.PackagePair.SourcePackageShortName}-{sdOutcome.SourceId}-elements-for-{igTr.PackagePair.TargetPackageShortName}-{targetId}";

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
                $"A computable version of the following element information is available in:" +
                $" [{elementCmId.Replace('-', ' ')}](ConceptMap-{elementCmId}.html)");
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

            // iterate over the element outcomes
            foreach (DbElementOutcome edOutcome in edOutcomes)
            {
                List<string> targetLines = [];

                if (edOutcome.TargetId is not null)
                {
                    targetLines.Add($"[{edOutcome.TargetId}]({targetBaseUrl}{edOutcome.TargetId.Split('.')[0]}.html#resource)");
                }

                if (edOutcome.RequiresXVerDefinition)
                {
                    string targetLabel;
                    string targetLink;

                    if (edOutcome.BasicElementEquivalent is not null)
                    {
                        string[] components = edOutcome.BasicElementEquivalent.Split('.');
                        targetLabel = string.Join('.', ["Basic", .. components[1..]]);
                        targetLink = $"{targetBaseUrl}Basic.html#resource";
                    }
                    else if (edOutcome.ParentElementOutcomeKey is null)
                    {
                        if (edOutcome.ExtensionSubstitutionUrl is not null)
                        {
                            targetLabel = edOutcome.ExtensionSubstitutionUrl;
                            targetLink = edOutcome.ExtensionSubstitutionUrl;
                        }
                        else
                        {
                            targetLabel = edOutcome.GenUrl!;
                            targetLink = $"StructureDefinition-{edOutcome.GenShortId!}.html";
                        }
                    }
                    else if (outcomeAccumulator.TryGetValue(edOutcome.ParentElementOutcomeKey.Value, out (string label, string link) parent))
                    {
                        targetLabel = "Extension slice: " + edOutcome.GenUrl!;
                        targetLink = parent.link;
                    }
                    else
                    {
                        targetLabel = edOutcome.GenUrl!;
                        targetLink = $"StructureDefinition-{edOutcome.GenShortId!}.html";
                    }

                    outcomeAccumulator[edOutcome.Key] = (targetLabel, targetLink);
                    targetLines.Add($"[{targetLabel}]({targetLink})");
                }

                mdWriter.WriteLine(
                    $"| [`{edOutcome.SourceId}`]({sourceBaseUrl}{sdOutcome.SourceName}.html#resource)" +
                    $" | {string.Join("<br/>", targetLines)}");
            }

            // finish writing the file
            mdWriter.Close();

            string fn = Path.GetFileName(filename);
            exported.Add(new()
            {
                FileName = fn,
                FileNameWithoutExtension = fn[..^3],
                IsPageContentFile = true,
                Name = sdOutcome.GenLongId!.ForName(),
                Id = null,
                Url = null,
                ResourceType = null,
                Version = null,
                Description = sdOutcome.GenLongId!.ForName(),
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

            mdWriter.WriteLine(
                $"| [{sdOutcome.SourceName}]({sourceBaseUrl}{sdOutcome.SourceId}.html)" +
                $"| [{targetId}]({targetBaseUrl}{targetId}.html)" +
                $" | [Lookup: {sdOutcome.GenLongId!.Replace('-', ' ')}](lookup-sd-{sdOutcome.GenLongId}.html)" +
                $" | [Profile: {sdOutcome.GenShortId!.Replace('-', ' ')}](StructureDefinition-{sdOutcome.GenShortId}.html)");
        }

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
