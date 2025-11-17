using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.CommandLine;
using System.Data;
using System.Data.Common;
using System.Formats.Tar;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;
using Fhir.CodeGen.Common.Extensions;
using Fhir.CodeGen.Common.Models;
using Fhir.CodeGen.Common.Packaging;
using Fhir.CodeGen.Common.Utils;
using Fhir.CodeGen.Comparison.CompareTool;
using Fhir.CodeGen.Comparison.Models;
using Fhir.CodeGen.Lib.Configuration;
using Fhir.CodeGen.Lib.FhirExtensions;
using Fhir.CodeGen.Lib.Language;
using Fhir.CodeGen.Lib.Models;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Specification.Snapshot;
using Hl7.Fhir.Utility;
using Hl7.FhirPath.Sprache;
using Microsoft.Extensions.Logging;
using Octokit;
using static System.Net.Mime.MediaTypeNames;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;
using Tasks = System.Threading.Tasks;

namespace Fhir.CodeGen.Comparison.XVer;

public partial class XVerProcessor
{

    /// <summary>
    /// Enumerates the possible outcomes for cross-version element mapping in FHIR processing.
    /// </summary>
    private enum XverOutcomeCodes
    {
        /// <summary>
        /// The element is used with the same name in the target version.
        /// </summary>
        UseElementSameName,
        /// <summary>
        /// The element is used but has been renamed in the target version.
        /// </summary>
        UseElementRenamed,
        /// <summary>
        /// The element is represented as an extension in the target version.
        /// </summary>
        UseExtension,
        /// <summary>
        /// The element is represented as an extension inherited from an ancestor.
        /// </summary>
        UseExtensionFromAncestor,
        /// <summary>
        /// The element is mapped to a basic element in the target version.
        /// </summary>
        UseBasicElement,
        /// <summary>
        /// The element is mapped to one of several possible elements, and possibly an extension, in the target version.
        /// </summary>
        UseOneOf,
    }

    /// <summary>
    /// Represents the outcome of a cross-version element mapping operation.
    /// </summary>
    private record class XverOutcome
    {
        /// <summary>
        /// Gets the key of the source FHIR package.
        /// </summary>
        public required int SourcePackageKey { get; init; }

        /// <summary>
        /// Gets the name of the source structure.
        /// </summary>
        public required string SourceStructureName { get; init; }

        /// <summary>
        /// Gets the identifier of the source element.
        /// </summary>
        public required string SourceElementId { get; init; }

        public required string? SourceElementBasePath { get; init; }

        /// <summary>
        /// Gets the field order of the source element within the structure.
        /// </summary>
        public required int SourceElementFieldOrder { get; init; }

        /// <summary>
        /// Gets the key of the target FHIR package.
        /// </summary>
        public required int TargetPackageKey { get; init; }

        /// <summary>
        /// Gets the outcome code describing the mapping result.
        /// </summary>
        public required XverOutcomeCodes OutcomeCode { get; init; }

        /// <summary>
        /// Gets the identifier of the target element, if applicable.
        /// </summary>
        public required string? TargetElementId { get; init; }

        public required string? TargetElementBasePath { get; init; }

        /// <summary>
        /// Gets the URL of the target extension, if the mapping resulted in an extension.
        /// </summary>
        public required string? TargetExtensionUrl { get; init; }

        /// <summary>
        /// Gets the URL of the target extension, if the mapping resulted in an extension.
        /// </summary>
        public required string? TargetExtensionId { get; init; }

        /// <summary>
        /// Gets the URL of a replacement extension, if the mapping resulted in a substitution.
        /// </summary>
        public required string? ReplacementExtensionUrl { get; init; }
    }


    private void writeXverOutcomes(
        List<PackageXverSupport> packageSupports,
        Dictionary<(int, string), List<List<XverOutcome>>> xverOutcomes,
        List<XverPackageIndexInfo> indexInfos,
        string outputDir)
    {
        HashSet<string> createdDirs = [];

        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        string fhirDir = Path.Combine(outputDir, "fhir");
        if (!Directory.Exists(fhirDir))
        {
            Directory.CreateDirectory(fhirDir);
        }

        string docsDir = Path.Combine(outputDir, "docs");
        if (!Directory.Exists(docsDir))
        {
            Directory.CreateDirectory(docsDir);
        }

        ILookup<(int sourcePackageKey, int targetPackageKey), XverPackageIndexInfo> indexInfoLookup =
            indexInfos.ToLookup(ii => (ii.SourcePackageSupport.PackageIndex, ii.TargetPackageSupport.PackageIndex));

        Dictionary<string, (string sourceVersion, string targetVersion)> packageVersions = [];

        // iterate over each structure in each source package
        foreach (((int sourcePackageIndex, string sourceStructureName), List<List<XverOutcome>> structureOutcomesByTarget) in xverOutcomes)
        {
            // iterate across each of our targets
            foreach ((List<XverOutcome> outcomes, int targetPackageIndex) in structureOutcomesByTarget.Select((ol, i) => (ol, i)))
            {
                if (sourcePackageIndex == targetPackageIndex)
                {
                    continue;
                }

                XverPackageIndexInfo? indexInfo = indexInfoLookup[(sourcePackageIndex, targetPackageIndex)].FirstOrDefault();

                if (indexInfo is null)
                {
                    _logger.LogWarning(
                        "No XVer index info found for source package index {SourcePackageIndex} and target package index {TargetPackageIndex}.",
                        sourcePackageIndex,
                        targetPackageIndex);
                    continue;
                }

                DbFhirPackage sourcePackage = packageSupports[sourcePackageIndex].Package;
                DbFhirPackage targetPackage = packageSupports[targetPackageIndex].Package;

                string packageId = getPackageId(sourcePackage, targetPackage);
                string htmlDir;
                string mdDir;

                if (createdDirs.Contains(packageId))
                {
                    if (_config.XverExportForPublisher)
                    {
                        htmlDir = string.Empty;
                        mdDir = Path.Combine(fhirDir, packageId, "input", "pagecontent");
                    }
                    else
                    {
                        htmlDir = Path.Combine(fhirDir, packageId, "package", "doc");
                        mdDir = Path.Combine(docsDir, packageId);
                    }
                }
                else
                {
                    packageVersions.Add(packageId, (sourcePackage.PackageVersion, targetPackage.PackageVersion));

                    if (_config.XverExportForPublisher)
                    {
                        htmlDir = string.Empty;
                        mdDir = Path.Combine(fhirDir, packageId, "input", "pagecontent");
                    }
                    else
                    {
                        htmlDir = Path.Combine(fhirDir, packageId);
                        if (!Directory.Exists(htmlDir))
                        {
                            Directory.CreateDirectory(htmlDir);
                        }

                        htmlDir = Path.Combine(htmlDir, "package");
                        if (!Directory.Exists(htmlDir))
                        {
                            Directory.CreateDirectory(htmlDir);
                        }

                        htmlDir = Path.Combine(htmlDir, "doc");
                        if (!Directory.Exists(htmlDir))
                        {
                            Directory.CreateDirectory(htmlDir);
                        }

                        mdDir = Path.Combine(docsDir, packageId);
                        if (!Directory.Exists(mdDir))
                        {
                            Directory.CreateDirectory(mdDir);
                        }
                    }

                    createdDirs.Add(packageId);
                }

                if (_config.XverExportForPublisher)
                {
                    string sourceBaseUrl = sourcePackage.DefinitionFhirSequence.ToWebUrlRoot();
                    string targetBaseUrl = targetPackage.DefinitionFhirSequence.ToWebUrlRoot();

                    // create a filename for this structure's md file
                    string mdFilename = $"Lookup-{sourcePackage.ShortName}-{sourceStructureName}-{targetPackage.ShortName}.md";

                    indexInfo.StructureLookupFiles.Add(new()
                    {
                        FileName = mdFilename,
                        FileNameWithoutExtension = mdFilename[..^3],
                        IsPageContentFile = true,
                        Name = sourceStructureName,
                        Id = sourceStructureName,
                        Url = string.Empty,
                        ResourceType = "StructureDefinition",
                        Version = _crossDefinitionVersion,
                        Description = $"Lookup for FHIR {sourcePackage.ShortName} {sourceStructureName} for use in FHIR {targetPackage.ShortName}"
                    });

                    // open our files
                    using ExportStreamWriter mdWriter = createMarkdownWriter(Path.Combine(mdDir, mdFilename), false, false);

                    // write a header
                    mdWriter.WriteLine(
                        $"### Lookup for [FHIR {sourcePackage.ShortName}]({sourceBaseUrl})" +
                        $" [{sourceStructureName}]({sourceBaseUrl}{sourceStructureName}.html)" +
                        $" for use in [FHIR {targetPackage.ShortName}]({targetBaseUrl})");

                    mdWriter.WriteLine();
                    mdWriter.WriteLine($"| Source Element (FHIR {sourcePackage.ShortName}) | Usage | Target |");
                    mdWriter.WriteLine("| -------------- | ----- | ------ |");

                    // iterate over the elements of this structure in element order
                    foreach (XverOutcome outcome in outcomes.OrderBy(xo => xo.SourceElementFieldOrder))
                    {
                        string target = outcome.OutcomeCode switch
                        {
                            XverOutcomeCodes.UseElementSameName => $"[{outcome.TargetElementId}]({targetBaseUrl}{outcome.TargetElementId!.Split('.')[0]}.html#resource)",
                            XverOutcomeCodes.UseElementRenamed => $"[{outcome.TargetElementId}]({targetBaseUrl}{outcome.TargetElementId!.Split('.')[0]}.html#resource)",
                            XverOutcomeCodes.UseExtension => outcome.ReplacementExtensionUrl != null
                                ? $"[{outcome.ReplacementExtensionUrl}]({outcome.ReplacementExtensionUrl})"
                                : $"[{outcome.TargetExtensionUrl}](StructureDefinition-{outcome.TargetExtensionId}.html)",
                            XverOutcomeCodes.UseExtensionFromAncestor => "-",
                            XverOutcomeCodes.UseBasicElement => $"[{outcome.TargetElementId}]({targetBaseUrl}{outcome.TargetElementId!.Split('.')[0]}.html#resource)",
                            XverOutcomeCodes.UseOneOf => publisherOneOfText(outcome, targetBaseUrl),
                            _ => "-"
                        };

                        //string target = outcome.ReplacementExtensionUrl ?? outcome.TargetElementId ?? outcome.TargetExtensionUrl ?? "-";
                        mdWriter.WriteLine(
                            $"| [{outcome.SourceElementId}]({sourceBaseUrl}{sourceStructureName}.html#resource) " +
                            $"| `{outcome.OutcomeCode}` " +
                            $"| {target} " +
                            $"|");
                    }

                    mdWriter.Close();
                }
                else
                {
                    // create a filename for this structure's md file
                    string mdFilename = $"Lookup-{sourcePackage.ShortName}-{sourceStructureName}-{targetPackage.ShortName}.md";
                    string htmlFilename = $"Lookup-{sourcePackage.ShortName}-{sourceStructureName}-{targetPackage.ShortName}.html";

                    // open our files
                    using ExportStreamWriter mdWriter = createMarkdownWriter(Path.Combine(mdDir, mdFilename), false, false);
                    using ExportStreamWriter htmlWriter = createHtmlWriter(Path.Combine(htmlDir, htmlFilename), false, false);

                    // write a header
                    mdWriter.WriteLine($"### Lookup for FHIR {sourcePackage.ShortName} {sourceStructureName} for use in FHIR {targetPackage.ShortName}");
                    htmlWriter.WriteLine($"<h2>Lookup for FHIR {sourcePackage.ShortName} {sourceStructureName} for use in FHIR {targetPackage.ShortName}</h2>");

                    mdWriter.WriteLine();
                    mdWriter.WriteLine("| Source Element | Usage | Target |");
                    mdWriter.WriteLine("| -------------- | ----- | ------ |");

                    htmlWriter.WriteLine();
                    htmlWriter.WriteLine("<table border=\"1\">");
                    htmlWriter.WriteLine("<tr><th>Source Element</th><th>Usage</th><th>Target</th></tr>");

                    // iterate over the elements of this structure in element order
                    foreach (XverOutcome outcome in outcomes.OrderBy(xo => xo.SourceElementFieldOrder))
                    {
                        string target = outcome.ReplacementExtensionUrl ?? outcome.TargetElementId ?? outcome.TargetExtensionUrl ?? "-";
                        mdWriter.WriteLine($"| {outcome.SourceElementId} | {outcome.OutcomeCode} | {target} |");
                        htmlWriter.WriteLine($"<tr><td>{outcome.SourceElementId}</td><td>{outcome.OutcomeCode}</td><td>{target}</td></tr>");
                    }

                    htmlWriter.WriteLine("</table>");

                    mdWriter.Close();
                    htmlWriter.Close();
                }
            }
        }

        // if we are exporting for the publisher, we need to create a single index file per package
        if (_config.XverExportForPublisher)
        {
            // iterate across each of our targets
            foreach (XverPackageIndexInfo indexInfo in indexInfos)
            {
                if (indexInfo.StructureLookupFiles.Count == 0)
                {
                    continue; // no files for this package, skip it
                }

                string packageId = indexInfo.PackageId;
                string sourceVersion = indexInfo.SourcePackageSupport.Package.PackageVersion;
                string targetVersion = indexInfo.TargetPackageSupport.Package.PackageVersion;

                // create the index file
                using ExportStreamWriter indexWriter = createMarkdownWriter(Path.Combine(fhirDir, packageId, "input", "pagecontent", "lookup.md"), false, false);
                indexWriter.WriteLine($"### FHIR {packageId} Cross-Version Artifact Lookup");
                indexWriter.WriteLine();
                indexWriter.WriteLine("The following table links to documentation for the source version of FHIR, for implementers to understand if there is an extension for the element they are trying to use.");
                indexWriter.WriteLine($"These are structures defined in FHIR {sourceVersion} (the source package), with applicable usage as mapped into FHIR {targetVersion} (the target package).");
                indexWriter.WriteLine();
                indexWriter.WriteLine($"| {sourceVersion} Structure | Lookup File |");
                indexWriter.WriteLine("| --------- | ----------- |");

                foreach (XVerIgFileRecord fileRec in indexInfo.StructureLookupFiles)
                {
                    indexWriter.WriteLine($"| {fileRec.Name} | [{fileRec.FileNameWithoutExtension}]({fileRec.FileNameWithoutExtension}.html) |");
                }

                indexWriter.Close();
            }


            //foreach ((string packageId, List<(string structureName, string filename)> mdFiles) in packageMdList)
            //{
            //    if (mdFiles.Count == 0)
            //    {
            //        continue; // no files for this package, skip it
            //    }

            //    (string sourceVersion, string targetVersion) = packageVersions[packageId];

            //    // create the index file
            //    using ExportStreamWriter indexWriter = createMarkdownWriter(Path.Combine(fhirDir, packageId, "input", "pagecontent", "lookup.md"), false, false);
            //    indexWriter.WriteLine($"### FHIR {packageId} Cross-Version Artifact Lookup");
            //    indexWriter.WriteLine();
            //    indexWriter.WriteLine("The following table links to documentation for the source version of FHIR, for implementers to understand if there is an extension for the element they are trying to use.");
            //    indexWriter.WriteLine($"These are structures defined in FHIR {sourceVersion} (the source package), with applicable usage as mapped into FHIR {targetVersion} (the target package).");
            //    indexWriter.WriteLine();
            //    indexWriter.WriteLine($"| {sourceVersion} Structure | Lookup File |");
            //    indexWriter.WriteLine("| --------- | ----------- |");

            //    foreach ((string structureName, string filename) in mdFiles.OrderBy(x => x.structureName))
            //    {
            //        indexWriter.WriteLine($"| {structureName} | [{filename}]({filename}.html) |");
            //    }

            //    indexWriter.Close();
            //}
        }

        return;

        string publisherOneOfText(XverOutcome outcome, string targetBaseUrl)
        {
            string[] oneOfElements = outcome.TargetElementId?.Split(',') ?? [];

            return string.Join("<br />", oneOfElements.Select(e => $"[{e}]({targetBaseUrl}{e.Split('.')[0]}.html#resource)")) +
                   (outcome.TargetExtensionUrl != null ? $"<br/>[{outcome.TargetExtensionUrl}]({outcome.TargetExtensionUrl})" : "");
        }
    }
}
