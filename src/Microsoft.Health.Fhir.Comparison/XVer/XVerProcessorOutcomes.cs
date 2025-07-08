using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.Comparison.CompareTool;
using Microsoft.Health.Fhir.CodeGen.Configuration;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;
using Microsoft.Health.Fhir.CodeGen.Language;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using Microsoft.Health.Fhir.CodeGenCommon.Utils;
using System.CommandLine;
using System.Linq;
using System.Data.Common;
using System.Collections.Concurrent;
using Microsoft.Health.Fhir.CodeGenCommon.Models;
using Microsoft.Health.Fhir.Comparison.Models;
using System.Xml.Linq;
using System.Data;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Specification.Snapshot;
using static System.Net.Mime.MediaTypeNames;
using Hl7.FhirPath.Sprache;
using Tasks = System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Formats.Tar;

namespace Microsoft.Health.Fhir.Comparison.XVer;

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
        /// The element is mapped to one of several possible elements in the target version.
        /// </summary>
        UseOneOfElements,
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

        /// <summary>
        /// Gets the URL of the target extension, if the mapping resulted in an extension.
        /// </summary>
        public required string? TargetExtensionUrl { get; init; }

        /// <summary>
        /// Gets the URL of a replacement extension, if the mapping resulted in a substitution.
        /// </summary>
        public required string? ReplacementExtensionUrl { get; init; }
    }



    private void writeXverOutcomes(
        List<PackageXverSupport> packageSupports,
        Dictionary<(int, string), List<List<XverOutcome>>> xverOutcomes,
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

                DbFhirPackage sourcePackage = packageSupports[sourcePackageIndex].Package;
                DbFhirPackage targetPackage = packageSupports[targetPackageIndex].Package;

                string packageFor = $"{sourcePackage.ShortName}-for-{targetPackage.ShortName}";
                string htmlDir;
                string mdDir;
                if (createdDirs.Contains(packageFor))
                {
                    htmlDir = Path.Combine(fhirDir, packageFor, "package", "doc");
                    mdDir = Path.Combine(docsDir, packageFor);
                }
                else
                {
                    htmlDir = Path.Combine(fhirDir, $"{sourcePackage.ShortName}-for-{targetPackage.ShortName}");
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

                    mdDir = Path.Combine(docsDir, packageFor);
                    if (!Directory.Exists(mdDir))
                    {
                        Directory.CreateDirectory(mdDir);
                    }

                    createdDirs.Add(packageFor);
                }

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


}
