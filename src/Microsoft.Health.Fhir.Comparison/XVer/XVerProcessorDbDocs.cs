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

    public void WriteDocsFromDatabase(FhirArtifactClassEnum? artifactFilter = null, string? outputDir = null)
    {
        // check for no database
        if (_db == null)
        {
            throw new Exception("Cannot generate docs without a loaded database!");
        }

        //string outputDir = !string.IsNullOrEmpty(_config.OutputDirectory)
        //    ? _config.OutputDirectory
        //    : _config.CrossVersionMapSourcePath;

        outputDir ??= _config.CrossVersionMapSourcePath;

        // check for no output location
        if (string.IsNullOrEmpty(outputDir))
        {
            throw new Exception("Cannot write markdown docs without output or map source folder!");
        }

        string docDir = Path.Combine(outputDir, "docs");
        if (!Directory.Exists(docDir))
        {
            Directory.CreateDirectory(docDir);
        }

        _logger.LogInformation($"Writing markdown documentation to {docDir}");

        // if we are writing primitives, put the overall mapping doc in the root
        if ((artifactFilter == null) ||
            (artifactFilter == FhirArtifactClassEnum.PrimitiveType) ||
            (artifactFilter == FhirArtifactClassEnum.ComplexType) ||
            (artifactFilter == FhirArtifactClassEnum.Resource))
        {
            // write the generic primitive mappings (across all versions)
            writeMarkdownRootPrimitiveMaps(docDir);
        }

        // grab the FHIR Packages we are processing
        List<DbFhirPackage> packages = DbFhirPackage.SelectList(_db.DbConnection, orderByProperties: [nameof(DbFhirPackage.ShortName)]);
        List<DbFhirPackageComparisonPair> packageComparisonPairs = DbFhirPackageComparisonPair.SelectList(
            _db.DbConnection,
            orderByProperties: [nameof(DbFhirPackageComparisonPair.SourcePackageKey), nameof(DbFhirPackageComparisonPair.TargetPackageKey)]);

        // iterate over the list of packages
        foreach (DbFhirPackage package in packages)
        {
            // create the export directory for this package
            string packageDir = Path.Combine(docDir, FhirSanitizationUtils.SanitizeForProperty(package.ShortName));

            // check for the directory already existing
            if (Directory.Exists(packageDir))
            {
                // remove the directory and contents (start clean)
                if (artifactFilter == null)
                {
                    Directory.Delete(packageDir, true);
                    Directory.CreateDirectory(packageDir);
                }
                else if (artifactFilter == FhirArtifactClassEnum.ValueSet)
                {
                    Directory.Delete(Path.Combine(packageDir, "ValueSets"), true);
                }
                else if ((artifactFilter == FhirArtifactClassEnum.PrimitiveType) ||
                    (artifactFilter == FhirArtifactClassEnum.ComplexType) ||
                    (artifactFilter == FhirArtifactClassEnum.Resource))
                {
                    if (Directory.Exists(Path.Combine(packageDir, "PrimitiveTypes")))
                    {
                        Directory.Delete(Path.Combine(packageDir, "PrimitiveTypes"), true);
                    }

                    if (Directory.Exists(Path.Combine(packageDir, "ComplexTypes")))
                    {
                        Directory.Delete(Path.Combine(packageDir, "ComplexTypes"), true);
                    }

                    if (Directory.Exists(Path.Combine(packageDir, "Resources")))
                    {
                        Directory.Delete(Path.Combine(packageDir, "Resources"), true);
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(packageDir);
            }

            // write the contents of our value sets if requested
            if ((artifactFilter == null) ||
                (artifactFilter == FhirArtifactClassEnum.ValueSet))
            {
                writeMarkdownValueSets(packages, packageComparisonPairs, package, packageDir);
            }

            if ((artifactFilter == null) ||
                (artifactFilter == FhirArtifactClassEnum.PrimitiveType) ||
                (artifactFilter == FhirArtifactClassEnum.ComplexType) ||
                (artifactFilter == FhirArtifactClassEnum.Resource))
            {
                //writeMarkdownStructures(packages, packageComparisonPairs, package, packageDir, FhirArtifactClassEnum.PrimitiveType);
                writeMarkdownStructures(packages, packageComparisonPairs, package, packageDir, FhirArtifactClassEnum.ComplexType);
                writeMarkdownStructures(packages, packageComparisonPairs, package, packageDir, FhirArtifactClassEnum.Resource);
            }
        }
    }


    private void writeMarkdownStructures(
        List<DbFhirPackage> packages,
        List<DbFhirPackageComparisonPair> packageComparisonPairs,
        DbFhirPackage package,
        string dir,
        FhirArtifactClassEnum artifactClass)
    {
        int keyPackageColIndex = packages.FindIndex(fp => fp.Key == package.Key);

        string artifactPascal = artifactClass switch
        {
            FhirArtifactClassEnum.PrimitiveType => "PrimitiveTypes",
            FhirArtifactClassEnum.ComplexType => "ComplexTypes",
            FhirArtifactClassEnum.Resource => "Resources",
            _ => throw new InvalidOperationException("Invalid artifact class."),
        };

        string artifactLower = artifactPascal.ToLowerInvariant();

        string sdDir = Path.Combine(dir, artifactPascal);
        if (!Directory.Exists(sdDir))
        {
            Directory.CreateDirectory(sdDir);
        }

        string overviewFilename = Path.Combine(dir, artifactPascal + ".md");

        using ExportStreamWriter overviewWriter = createMarkdownWriter(overviewFilename, true, true);

        ConcurrentBag<string> overviewEntries = [];

        // get the list of all Value Sets in this version
        List<DbStructureDefinition> structures = DbStructureDefinition.SelectList(
            _db!.DbConnection,
            FhirPackageKey: package.Key,
            ArtifactClass: artifactClass);

        // iterate over our value sets and generate documents
        Parallel.ForEach(structures, (sd, cancellationToken) =>
        //foreach (DbStructureDefinition sd in structures)
        {
            DbGraphSd sdGraph = new()
            {
                DB = _db!.DbConnection,
                Packages = packages,
                KeySd = sd,
            };

            // get the overview entry for this value set
            string content = getMdOverviewEntry(packages, package, sd, sdGraph.Projection);

            // get the overview entry for this value set
            overviewEntries.Add(content);

            string filename = Path.Combine(sdDir, getSdFilename(sd.Name, artifactClass, includeRelativeDir: false));
            using (ExportStreamWriter vsWriter = createMarkdownWriter(filename, true, true))
            {
                writeMdDetailedSd(_db!.DbConnection, vsWriter, packages, package, keyPackageColIndex, sd, sdGraph);
            }
        });
        //}

        writeMdOverviewSd(overviewWriter, packages, package, artifactClass);
        foreach (string line in overviewEntries.Order())
        {
            overviewWriter.WriteLineIndented(line);
        }

        return;
    }



    private void writeMdDetailedSd(
        IDbConnection db,
        ExportStreamWriter writer,
        List<DbFhirPackage> packages,
        DbFhirPackage package,
        int keyPackageColIndex,
        DbStructureDefinition keySd,
        DbGraphSd sdGraph)
    {
        writer.WriteLine($"""
            ### {keySd.Name}

            |      |     |
            | ---: | --- |
            | Package | {package.PackageId.ForMdTable()}@{package.PackageVersion.ForMdTable()} |
            | Stucture Name | {keySd.Name.ForMdTable()} |
            | Canonical URL | `{keySd.UnversionedUrl.ForMdTable()}` |
            | Version | {keySd.Version.ForMdTable()} |
            | Description | {keySd.Description.ForMdTable()} |
            | Status | `{keySd.Status}` |
            | Artifact Class | `{keySd.ArtifactClass}` |
            | Database Key | `{keySd.Key}` |
            | Database Snapshot Count | `{keySd.SnapshotCount}` |
            | Database Differential Count | `{keySd.DifferentialCount}` |

            ### Elements

            | Id | Path | Name | Base Path | Short | Cardinality | Collated Type | Binding Strength | Binding Value Set |
            | -- | ---- | ---- | --------- | ----- | ----------- | ------------- | ---------------- | ----------------- |
            """);

        foreach (DbElement e in DbElement.SelectList(db, StructureKey: keySd.Key, orderByProperties: [nameof(DbElement.Id)]))
        {
            writer.WriteLine(
                $"| `{e.Id.ForMdTable()}`" +
                $" | `{e.Path.ForMdTable()}`" +
                $" | `{e.Name.ForMdTable()}`" +
                $" | {e.BasePath?.ForMdTable()}" +
                $" | {e.Short.ForMdTable()}" +
                $" | {e.MinCardinality}..{e.MaxCardinalityString}" +
                $" | {e.FullCollatedTypeLiteral.ForMdTable()}" +
                $" | {(e.ValueSetBindingStrength == null ? string.Empty : "`" + e.ValueSetBindingStrength + "`")}" +
                $" | {(e.BindingValueSet == null ? string.Empty : "`" + e.BindingValueSet + "`")}" +
                $" |");
        }

        // if there are no mappings, we are done writing this file
        if ((sdGraph.Projection.Count == 0) ||
            (
                (sdGraph.Projection.Count == 1) &&
                (sdGraph.Projection[0][keyPackageColIndex]?.LeftComparison == null) &&
                (sdGraph.Projection[0][keyPackageColIndex]?.RightComparison == null)
            ))
        {
            writer.WriteLine($"""
                ### Empty Projection

                This Structure ({keySd.ArtifactClass}) resulted in no projection (no mappings to other packages).

                """);

            return;
        }

        int byTwoColumnCount = (packages.Count * 2) - 1;

        string sdNamePascal = keySd.Name.ToPascalCase();
        string sdNameClean = FhirSanitizationUtils.SanitizeForProperty(keySd.Name, convertToConvention: FhirNameConventionExtensions.NamingConvention.PascalCase);

        string artifactPascal = keySd.ArtifactClass switch
        {
            FhirArtifactClassEnum.PrimitiveType => "PrimitiveTypes",
            FhirArtifactClassEnum.ComplexType => "ComplexTypes",
            FhirArtifactClassEnum.Resource => "Resources",
            _ => throw new InvalidOperationException("Invalid artifact class."),
        };

        string[] sdRootUrlsByVersion = packages.Select(fp => $"/docs/{FhirSanitizationUtils.SanitizeForProperty(fp.ShortName)}/{artifactPascal}").ToArray();

        (string key, bool hasMapping)[] allKeys = packages.Select((fp, i) => (fp.ShortName, sdGraph.Projection.Any(r => r[i] != null))).ToArray();

        // generate table showing the mappings
        writer.WriteLine("### Mapping Table");
        writer.WriteLine();
        writer.WriteLine("| " + string.Join(" | Comparison | ", allKeys.Select(v => v.key)));
        writeTableColumns(writer, "---", byTwoColumnCount, appendNewline: true);

        foreach (DbGraphSd.DbSdRow row in sdGraph.Projection)
        {
            int column = -1;
            // traverse columns
            foreach (DbGraphSd.DbSdCell? cell in row)
            {
                column++;

                if (cell == null)
                {
                    writer.Write("| | ");
                    continue;
                }

                writer.Write(
                    $"| [{cell.Sd.Name.ForMdTable()}]({sdRootUrlsByVersion[column]}/{getSdFilename(cell.Sd.Name, cell.Sd.ArtifactClass, includeRelativeDir: false)})" +
                    $"<br/>" +
                    $" `{cell.Sd.VersionedUrl.ForMdTable()}` ");

                if (column == (row.Length - 1))
                {
                    writer.WriteLine();
                    continue;
                }

                (string toRight, string fromRight) = getMappingMdTableCell(cell, true);

                // write mapping notes
                writer.Write(
                    $"| {toRight}" +
                    $"<hr/>" +
                    $"{fromRight}");
            }
        }
        writer.WriteLine();

        // write a section for the element table
        writer.WriteLine("### Element Mappings");
        writer.WriteLine();

        int mapGroupIndex = 0;

        foreach (DbGraphSd.DbSdRow structureRow in sdGraph.Projection)
        {
            if (structureRow[keyPackageColIndex] == null)
            {
                continue;
            }

            writer.WriteLine();
            writer.WriteLine("#### Map Group " + mapGroupIndex++);
            writer.WriteLine();
            writer.WriteLine($"This group is centered on the Structure Definition {structureRow[keyPackageColIndex]!.Sd.Name} from {package.PackageId}@{package.PackageVersion} ({package.ShortName}, key {package.Key}).");
            writer.WriteLine("All elements from this structure are listed while other structures only show contents that have relationships with those elements.");
            writer.WriteLine();

            // write the table header
            for (int col = 0; col < packages.Count; col++)
            {
                if (col > 0)
                {
                    writer.Write("| Relationship ");
                }

                DbGraphSd.DbSdCell? cell = structureRow[col];

                if (cell == null)
                {
                    writer.Write("| *No Map* ");
                    continue;
                }

                if (col == keyPackageColIndex)
                {
                    writer.Write($"| {packages[col].ShortName} {cell.Sd.Name.ForMdTable()}");
                }
                else
                {
                    writer.Write($"| [{packages[col].ShortName} {cell.Sd.Name.ForMdTable()}]({sdRootUrlsByVersion[col]}/{getSdFilename(cell.Sd.Name, cell.Sd.ArtifactClass, includeRelativeDir: false)})");
                }
            }
            writer.WriteLine();
            writeTableColumns(writer, "---", byTwoColumnCount, appendNewline: true);

            HashSet<string>[] elementsPerSd = packages.Select(_ => new HashSet<string>()).ToArray();

            // iterate over the components in the concept projection
            foreach (DbGraphSd.DbElementRow elementRow in structureRow.Projection)
            {
                int column = -1;

                // traverse columns
                foreach (DbGraphSd.DbElementCell? cell in elementRow)
                {
                    column++;

                    if (cell == null)
                    {
                        writer.Write("| | ");
                        continue;
                    }

                    elementsPerSd[column].Add(cell.Element.Id);

                    if (column == keyPackageColIndex)
                    {
                        writer.Write($"| **`{cell.Element.Id.ForMdTable()}`**");
                    }
                    else
                    {
                        writer.Write($"| `{cell.Element.Id.ForMdTable()}`");
                    }

                    if (column == (elementRow.Length - 1))
                    {
                        continue;
                    }

                    if ((cell.RightCell == null) ||
                        (cell.RightComparison == null) ||
                        (cell.RightElement == null))
                    {
                        writer.Write("| ");
                    }
                    else
                    {
                        if ((cell.RightComparison.Relationship == ConceptMap.ConceptMapRelationship.Equivalent) &&
                            (cell.RightCell.LeftComparison?.Relationship == ConceptMap.ConceptMapRelationship.Equivalent))
                        {
                            writer.Write($"| _{cell.RightComparison.Relationship}_<br/>({cell.RightComparison.Key}/{cell.RightCell.LeftComparison?.Key})");
                        }
                        //else if ((cell.RightComparison.Relationship == ConceptMap.ConceptMapRelationship.SourceIsNarrowerThanTarget) &&
                        //    (cell.RightCell.LeftComparison?.Relationship == ConceptMap.ConceptMapRelationship.SourceIsBroaderThanTarget))
                        //{
                        //    writer.Write($"| ↢↢↢ ({cell.RightComparison.Key}/{cell.RightCell.LeftComparison?.Key})");
                        //}
                        //else if ((cell.RightComparison.Relationship == ConceptMap.ConceptMapRelationship.SourceIsBroaderThanTarget) &&
                        //    (cell.RightCell.LeftComparison?.Relationship == ConceptMap.ConceptMapRelationship.SourceIsNarrowerThanTarget))
                        //{
                        //    writer.Write($"| ↣↣↣ ({cell.RightComparison.Key}/{cell.RightCell.LeftComparison?.Key})");
                        //}
                        //if (cell.RightComparison.Relationship != cell.RightCell.LeftComparison?.Relationship)
                        //{
                        //    // write mapping notes
                        //    writer.Write(
                        //        $"| → {cell.RightComparison.Relationship} → " +
                        //        $"<hr/>" +
                        //        $"← {cell.RightCell.LeftComparison?.Relationship} ← ");
                        //}
                        //else if (cell.RightComparison.Relationship == ConceptMap.ConceptMapRelationship.Equivalent)
                        //{
                        //    writer.Write("| == ");
                        //}
                        //else if (cell.RightComparison.Relationship == ConceptMap.ConceptMapRelationship.SourceIsNarrowerThanTarget)
                        //{
                        //    writer.Write("| > ");
                        //}
                        //else if (cell.RightComparison.Relationship == ConceptMap.ConceptMapRelationship.SourceIsBroaderThanTarget)
                        //{
                        //    writer.Write("| < ");
                        //}
                        else
                        {
                            // write mapping notes
                            writer.Write(
                                $"| →→→→ _{cell.RightComparison.Relationship}_ →→→→ <br/>({cell.RightComparison.Key})" +
                                $"<hr/>" +
                                $"←←←← _{cell.RightCell.LeftComparison?.Relationship}_ ←←←← <br/>({cell.RightCell.LeftComparison?.Key})");
                        }
                    }
                }

                writer.WriteLine();
            }

            // check for unused elements in structures
            for (int i = 0; i < structureRow.Length; i++)
            {
                if (i != 0)
                {
                    writer.Write("| ");
                }

                if (structureRow[i] == null)
                {
                    writer.Write("| ");
                }
                else
                {
                    writer.Write($"| *{elementsPerSd[i].Count} of {structureRow[i]!.Sd.SnapshotCount} elements used* ");
                    if (elementsPerSd[i].Count < structureRow[i]!.Sd.SnapshotCount)
                    {
                        HashSet<string> allElements = structureRow[i]!.Elements.Select(c => c.Id).ToHashSet();
                        IEnumerable<string> unusedElements = allElements.Except(elementsPerSd[i]);
                        writer.Write($"<br/>remaining elements:<br/>{string.Join(", ", unusedElements.Select(v => "`" + v + "`"))}");
                    }

                }
            }

            writer.WriteLine();
            writer.WriteLine();
        }

        return;
    }


    private (string to, string from) getMappingMdTableCell(DbGraphSd.DbSdCell cell, bool movingRight)
    {
        DbGraphSd.DbSdCell? targetCell = movingRight ? cell.RightCell : cell.LeftCell;

        if (targetCell == null)
        {
            return ("<br/>*no map*<br/>", "<br/>*no map*<br/>");
        }

        DbStructureComparison? toComparison = movingRight ? cell.RightComparison : cell.LeftComparison;
        DbStructureComparison? fromComparison = movingRight ? targetCell.LeftComparison : cell.RightComparison;

        return (getLink(toComparison, targetCell, "→→→→→→→"), getLink(fromComparison, targetCell, "←←←←←←←"));

        string getLink(DbStructureComparison? comparison, DbGraphSd.DbSdCell? target, string arrows)
        {
            if ((comparison == null) || (target == null))
            {
                return arrows + "<br/>*no map*<br/>" + arrows;
            }

            return
                $"{arrows}" +
                $"<br/>`{comparison.Relationship}`" +
                $"<br/>- DBKey: `{comparison.Key}`" +
                $"<br/>- Reviewed: `{comparison.LastReviewedOn?.ToString("o") ?? "n/a"}`" +
                $"<br/>- By: `{comparison.LastReviewedBy ?? "n/a"}`" +
                $"<br/>- Identical: `{comparison.IsIdentical ?? false}`" +
                $"<br/>{arrows}";
        }
    }

    private void writeMdOverviewSd(
        ExportStreamWriter writer,
        List<DbFhirPackage> packages,
        DbFhirPackage package,
        FhirArtifactClassEnum artifactClass)
    {
        string artifactDisplay = artifactClass switch
        {
            FhirArtifactClassEnum.PrimitiveType => "Primitive Type",
            FhirArtifactClassEnum.ComplexType => "Complex Type",
            FhirArtifactClassEnum.Resource => "Resource",
            _ => throw new InvalidOperationException("Invalid artifact class."),
        };

        /*
            # Contents

            * [Required-Binding Value Sets](#required-binding-value-sets)
            * [Excluded Value Sets](#excluded-value-sets)
            * [Other Value Sets](#other-value-sets)
         */

        writer.Write($"""
            Keyed off: {package.PackageId}@{package.PackageVersion} - {package.ShortName}
            Canonical: {package.CanonicalUrl}
            
            ## {artifactDisplay} Overview
            
            """);


        List<string> headers = ["Name", "Canonical", "Description",];
        foreach (DbFhirPackage targetPackage in packages.OrderBy(fp => fp.ShortName))
        {
            if (targetPackage.Key == package.Key)
            {
                continue;
            }

            headers.Add($"Path to {targetPackage.ShortName.ForMdTable()}");
        }

        writer.WriteLineIndented($"| {string.Join(" | ", headers)} |");
        writer.WriteLineIndented($"| {string.Join(" | ", Enumerable.Repeat("---", headers.Count))} |");
    }

    private string getMdOverviewEntry(
        List<DbFhirPackage> packages,
        DbFhirPackage package,
        DbStructureDefinition sd,
        List<DbGraphSd.DbSdRow> projection)
    {
        List<string> mapsTo = [];
        for (int i = 0; i < packages.Count; i++)
        {
            if (packages[i].Key == sd.FhirPackageKey)
            {
                continue;
            }
            mapsTo.Add(projection.Any(r => r[i] != null) ? "✔" : "");
        }

        return
            $"| [{sd.Name.ForMdTable()}]({getSdFilename(sd.Name, sd.ArtifactClass)})" +
            $" | `{sd.VersionedUrl.ForMdTable()}`" +
            $" | {sd.Description.ForMdTable()}" +
            $" | {string.Join(" | ", mapsTo)} |";
    }


    private void writeMarkdownValueSets(
        List<DbFhirPackage> packages,
        List<DbFhirPackageComparisonPair> packageComparisonPairs,
        DbFhirPackage package,
        string dir)
    {
        int keyPackageColIndex = packages.FindIndex(fp => fp.Key == package.Key);

        string vsDir = Path.Combine(dir, "ValueSets");
        if (!Directory.Exists(vsDir))
        {
            Directory.CreateDirectory(vsDir);
        }

        string overviewFilename = Path.Combine(dir, "ValueSets.md");

        using ExportStreamWriter overviewWriter = createMarkdownWriter(overviewFilename, true, true);

        ConcurrentBag<string> requiredOverviewEntries = [];
        ConcurrentBag<string> excludedOverviewEntries = [];
        ConcurrentBag<string> otherOverviewEntries = [];

        // get the list of all Value Sets in this version
        List<DbValueSet> valueSets = DbValueSet.SelectList(_db!.DbConnection, FhirPackageKey: package.Key);

        // iterate over our value sets and generate documents
        Parallel.ForEach(valueSets, (vs, cancellationToken) =>
        {
            DbGraphVs vsGraph = new()
            {
                DB = _db!.DbConnection,
                Packages = packages,
                KeyVs = vs,
            };

            // get the overview entry for this value set
            string content = getMdOverviewEntry(packages, package, vs, vsGraph.Projection);

            if (vs.IsExcluded == true)
            {
                excludedOverviewEntries.Add(content);
            }
            else if (vs.StrongestBindingCore == BindingStrength.Required)
            {
                // get the overview entry for this value set
                requiredOverviewEntries.Add(content);
            }
            else
            {
                // get the overview entry for this value set
                otherOverviewEntries.Add(content);
            }

            string filename = Path.Combine(vsDir, getVsFilename(vs.Name, includeRelativeDir: false));
            using (ExportStreamWriter vsWriter = createMarkdownWriter(filename, true, true))
            {
                writeMdDetailedVs(_db!.DbConnection, vsWriter, packages, package, keyPackageColIndex, vs, vsGraph);

                //// check for failures - write a stub file with information about the value set
                //if (ca.FailureCode != null)
                //{
                //    writeMdComparisonFailed(vsWriter, vs);
                //    continue;
                //}
            }

        });

        ConcurrentBag<string>[] sectionEntries = [requiredOverviewEntries, excludedOverviewEntries, otherOverviewEntries];

        // write sections
        for (int i = 0; i < sectionEntries.Length; i++)
        {
            writeMdOverviewSectionVs(overviewWriter, packages, package, i);
            foreach (string line in sectionEntries[i].Order())
            {
                overviewWriter.WriteLineIndented(line);
            }
        }

        return;
    }



    private string getMdOverviewEntry(
        List<DbFhirPackage> packages,
        DbFhirPackage package,
        DbValueSet vs,
        List<DbGraphVs.DbVsRow> projection)
    {
        List<string> mapsTo = [];
        for (int i = 0; i < packages.Count; i++)
        {
            if (packages[i].Key == vs.FhirPackageKey)
            {
                continue;
            }

            mapsTo.Add(projection.Any(r => r[i] != null) ? "✔" : string.Empty);
        }

        string expandCell = vs.CanExpand ? "✔" : $"✘ {vs.Message.ForMdTable()}";
        //string excludedCell = vs.IsExcluded ? "⚠" : string.Empty;

        return
            $"| [{vs.Name.ForMdTable()}]({getVsFilename(vs.Name)})" +
            $" | `{vs.VersionedUrl.ForMdTable()}`" +
            $" | {vs.Description?.ForMdTable()}" +
            $" | {expandCell}" +
            $" | {string.Join(" | ", mapsTo)} |";
    }

    private void writeMdOverviewSectionVs(
        ExportStreamWriter writer,
        List<DbFhirPackage> packages,
        DbFhirPackage package,
        int section)
    {
        switch (section)
        {
            case 0:
                writer.Write($"""
                    Keyed off: {package.PackageId}@{package.PackageVersion} - {package.ShortName}
                    Canonical: {package.CanonicalUrl}

                    # Contents

                    * [Required-Binding Value Sets](#required-binding-value-sets)
                    * [Excluded Value Sets](#excluded-value-sets)
                    * [Other Value Sets](#other-value-sets)
            
                    ## Required-Binding Value Sets

                    """);
                break;

            case 1:
                writer.Write($"""

                    ## Excluded Value Sets

                    """);
                break;

            case 2:
                writer.Write($"""

                    ## Other Value Sets

                    """);
                break;
        }

        List<string> headers = ["Name", "Canonical", "Description", "Expands"];
        foreach (DbFhirPackage targetPackage in packages.OrderBy(fp => fp.ShortName))
        {
            if (targetPackage.Key == package.Key)
            {
                continue;
            }

            headers.Add($"Path to {targetPackage.ShortName.ForMdTable()}");
        }

        writer.WriteLineIndented($"| {string.Join(" | ", headers)} |");
        writer.WriteLineIndented($"| {string.Join(" | ", Enumerable.Repeat("---", headers.Count))} |");
    }


    /// <summary>
    /// Writes a detailed markdown with information about this value set, keyed from this version.
    /// </summary>
    /// <remarks>
    /// Note this function is currently too long and very inefficient - will fix once output is
    /// finalized.
    /// </remarks>
    private void writeMdDetailedVs(
        IDbConnection db,
        ExportStreamWriter writer,
        List<DbFhirPackage> packages,
        DbFhirPackage package,
        int keyPackageColIndex,
        DbValueSet keyVs,
        DbGraphVs vsGraph)
    {
        writer.WriteLine($"""
            ### {keyVs.Name}

            |      |     |
            | ---: | --- |
            | Package | {package.PackageId.ForMdTable()}@{package.PackageVersion.ForMdTable()} |
            | VS Name | {keyVs.Name.ForMdTable()} |
            | Canonical URL | `{keyVs.UnversionedUrl.ForMdTable()}` |
            | Version | {keyVs.Version.ForMdTable()} |
            | Description | {keyVs.Description.ForMdTable()} |
            | Status | `{keyVs.Status}` |
            | Has Escape Valve Code | `{keyVs.HasEscapeValveCode}` |
            | Database Key | `{keyVs.Key}` |
            | Database Concept Count | `{keyVs.ConceptCount}` |
            | Database Active Concept Count | `{keyVs.ActiveConcreteConceptCount}` |
            """);

        writer.WriteLine("### Bindings");
        writer.WriteLine();
        writer.WriteLine("| Source | Element | Binding | Strength | Element Short |");
        writer.WriteLine("| ------ | ------- | ------- | -------- | ------------- |");

        // get the elements with bindings
        {
            List<DbElement> boundElements = DbElement.SelectList(db, BindingValueSetKey: keyVs.Key, orderByProperties: [nameof(DbElement.Key)]);

            foreach (DbElement ed in boundElements)
            {
                DbStructureDefinition? sd = DbStructureDefinition.SelectSingle(db, Key: ed.StructureKey);

                if (sd == null)
                {
                    writer.WriteLine(
                        $"| Unresolved Key: `{ed.StructureKey}`" +
                        $" | `{ed.Path.ForMdTable()}`" +
                        $" | `{ed.BindingValueSet.ForMdTable()}`" +
                        $" | `{ed.ValueSetBindingStrength}`" +
                        $" | {ed.Short.ForMdTable()}" +
                        $" |");
                }
                else
                {
                    writer.WriteLine(
                        $"| `{sd.UnversionedUrl.ForMdTable()}`" +
                        $" | `{ed.Path.ForMdTable()}`" +
                        $" | `{ed.BindingValueSet.ForMdTable()}`" +
                        $" | `{ed.ValueSetBindingStrength}`" +
                        $" | {ed.Short.ForMdTable()}" +
                        $" |");
                }
            }
        }

        writer.WriteLine();

        if (keyVs.CanExpand == false)
        {
            writer.WriteLine($"""
                ### Expansion Failure

                Failed to expand this value set: {keyVs.Message}
                """);
            return;
        }

        writer.WriteLine("### Referenced Systems");
        writer.WriteLine();

        if (string.IsNullOrEmpty(keyVs.ReferencedSystems))
        {
            writer.WriteLine("No referenced systems.");
        }
        else
        {
            string[] systems = keyVs.ReferencedSystems.Split(", ");
            foreach (string system in systems)
            {
                writer.WriteLine($"* `{system}`");
            }
        }

        // if there are no mappings, we are done writing this file
        if ((vsGraph.Projection.Count == 0) ||
            (
                (vsGraph.Projection.Count == 1) &&
                (vsGraph.Projection[0][keyPackageColIndex]?.LeftComparison == null) &&
                (vsGraph.Projection[0][keyPackageColIndex]?.RightComparison == null)
            ))
        {
            writer.WriteLine($"""
                ### Empty Projection

                This Value Set resulted in no projection (no mappings to other packages).

                ### Codes

                | System | Code | Display |
                | ------ | ---- | ------- |
                """);

            foreach (DbValueSetConcept c in DbValueSetConcept.SelectList(
                db,
                ValueSetKey: keyVs.Key,
                Inactive: false,
                Abstract: false,
                orderByProperties: [nameof(DbValueSetConcept.System), nameof(DbValueSetConcept.Code)]))
            {
                writer.WriteLine(
                    $"| `{c.System.ForMdTable()}`" +
                    $" | `{c.Code.ForMdTable()}`" +
                    $" | {c.Display?.ForMdTable()}" +
                    $" |");
            }

            return;
        }

        int byTwoColumnCount = (packages.Count * 2) - 1;

        string vsNamePascal = keyVs.Name.ToPascalCase();
        string vsNameClean = FhirSanitizationUtils.SanitizeForProperty(keyVs.Name, convertToConvention: FhirNameConventionExtensions.NamingConvention.PascalCase);

        string[] vsRootUrlsByVersion = packages.Select(fp => $"/docs/{FhirSanitizationUtils.SanitizeForProperty(fp.ShortName)}/ValueSets").ToArray();

        (string key, bool hasMapping)[] allKeys = packages.Select((fp, i) => (fp.ShortName, vsGraph.Projection.Any(r => r[i] != null))).ToArray();

        // generate table showing the mappings
        writer.WriteLine("### Mapping Table");
        writer.WriteLine();
        writer.WriteLine("| " + string.Join(" | Comparison | ", allKeys.Select(v => v.key)));
        writeTableColumns(writer, "---", byTwoColumnCount, appendNewline: true);

        foreach (DbGraphVs.DbVsRow row in vsGraph.Projection)
        {
            int column = -1;
            // traverse columns
            foreach (DbGraphVs.DbVsCell? cell in row)
            {
                column++;

                if (cell == null)
                {
                    writer.Write("| | ");
                    continue;
                }

                writer.Write(
                    $"| [{cell.Vs.Name.ForMdTable()}]({vsRootUrlsByVersion[column]}/{getVsFilename(cell.Vs.Name, includeRelativeDir: false)})" +
                    $"<br/>" +
                    $" `{cell.Vs.VersionedUrl.ForMdTable()}` ");

                if (column == (row.Length - 1))
                {
                    writer.WriteLine();
                    continue;
                }

                (string toRight, string fromRight) = getMappingMdTableCell(cell, true);

                // write mapping notes
                writer.Write(
                    $"| {toRight}" +
                    $"<hr/>" +
                    $"{fromRight}");
            }
        }
        writer.WriteLine();

        // write a section for the code table
        writer.WriteLine("### Code Mappings");
        writer.WriteLine();

        int mapGroupIndex = 0;
        //List<DbValueSetConcept> keyConcepts = DbValueSetConcept.SelectList(
        //    db,
        //    ValueSetKey: keyVs.Key,
        //    orderByProperties: [nameof(DbValueSetConcept.System), nameof(DbValueSetConcept.Code)]);

        foreach (DbGraphVs.DbVsRow valueSetRow in vsGraph.Projection)
        {
            if (valueSetRow[keyPackageColIndex] == null)
            {
                continue;
            }

            writer.WriteLine();
            writer.WriteLine("#### Map Group " + mapGroupIndex++);
            writer.WriteLine();
            writer.WriteLine($"This group is centered on the Value Set {valueSetRow[keyPackageColIndex]!.Vs.Name} from {package.PackageId}@{package.PackageVersion} ({package.ShortName}, key {package.Key}).");
            writer.WriteLine("All codes from this value set are listed while other value sets only show contents that have relationships with those codes.");
            writer.WriteLine();

            // write the table header
            for (int col = 0; col < packages.Count; col++)
            {
                if (col > 0)
                {
                    writer.Write("| Relationship ");
                }

                DbGraphVs.DbVsCell? cell = valueSetRow[col];

                if (cell == null)
                {
                    writer.Write("| *No Map* ");
                    continue;
                }

                if (col == keyPackageColIndex)
                {
                    writer.Write($"| {packages[keyPackageColIndex].ShortName} {cell.Vs.Name.ForMdTable()}");
                }
                else
                {
                    writer.Write($"| [{packages[keyPackageColIndex].ShortName} {cell.Vs.Name.ForMdTable()}]({vsRootUrlsByVersion[col]}/{getVsFilename(cell.Vs.Name, includeRelativeDir: false)})");
                }
            }
            writer.WriteLine();
            writeTableColumns(writer, "---", byTwoColumnCount, appendNewline: true);

            HashSet<string>[] codesPerVs = packages.Select(_ => new HashSet<string>()).ToArray();
            string? lastSystem = null;

            // iterate over the components in the concept projection
            foreach (DbGraphVs.DbVsConceptRow conceptRow in valueSetRow.Projection)
            {
                if (conceptRow[keyPackageColIndex]?.Concept.System != lastSystem)
                {
                    lastSystem = conceptRow[keyPackageColIndex]?.Concept.System;
                    writer.WriteLine($"""| <td colspan="{byTwoColumnCount - 1}">**{package.ShortName.ForMdTable()}** System: `{lastSystem.ForMdTable()}`""");

                    //writeTableColumns(
                    //    writer,
                    //    $"""{package.ShortName.ForMdTable()} System:<br/>`{lastSystem.ForMdTable()}`""",
                    //    byTwoColumnCount,
                    //    appendNewline: true,
                    //    valueOnlyInColumn: keyPackageColIndex * 2);
                }

                int column = -1;

                // traverse columns
                foreach (DbGraphVs.DbVsConceptCell? cell in conceptRow)
                {
                    column++;

                    if (cell == null)
                    {
                        writer.Write("| | ");
                        continue;
                    }

                    codesPerVs[column].Add(cell.Concept.FhirKey);

                    if (column == keyPackageColIndex)
                    {
                        writer.Write($"| **`{cell.Concept.Code.ForMdTable()}`**");
                    }
                    else
                    {
                        writer.Write($"| `{cell.Concept.Code.ForMdTable()}`");
                    }

                    if (column == (conceptRow.Length - 1))
                    {
                        continue;
                    }

                    if ((cell.RightCell == null) ||
                        (cell.RightComparison == null) ||
                        (cell.RightConcept == null))
                    {
                        writer.Write("| ");
                    }
                    else
                    {
                        if ((cell.RightComparison.Relationship == ConceptMap.ConceptMapRelationship.Equivalent) &&
                            (cell.RightCell.LeftComparison?.Relationship == ConceptMap.ConceptMapRelationship.Equivalent))
                        {
                            writer.Write($"| _{cell.RightComparison.Relationship}_ <br/>({cell.RightComparison.Key}/{cell.RightCell.LeftComparison?.Key})");
                        }
                        //else if ((cell.RightComparison.Relationship == ConceptMap.ConceptMapRelationship.SourceIsNarrowerThanTarget) &&
                        //    (cell.RightCell.LeftComparison?.Relationship == ConceptMap.ConceptMapRelationship.SourceIsBroaderThanTarget))
                        //{
                        //    writer.Write($"| ↢↢↢ ({cell.RightComparison.Key}/{cell.RightCell.LeftComparison?.Key})");
                        //}
                        //else if ((cell.RightComparison.Relationship == ConceptMap.ConceptMapRelationship.SourceIsBroaderThanTarget) &&
                        //    (cell.RightCell.LeftComparison?.Relationship == ConceptMap.ConceptMapRelationship.SourceIsNarrowerThanTarget))
                        //{
                        //    writer.Write($"| ↣↣↣ ({cell.RightComparison.Key}/{cell.RightCell.LeftComparison?.Key})");
                        //}
                        //else if (cell.RightComparison.Relationship != cell.RightCell.LeftComparison?.Relationship)
                        //{
                        //    // write mapping notes
                        //    writer.Write(
                        //        $"| → {cell.RightComparison.Relationship} → " +
                        //        $"<hr/>" +
                        //        $"← {cell.RightCell.LeftComparison?.Relationship} ← ");
                        //}
                        else
                        {
                            // write mapping notes
                            writer.Write(
                                $"| →→→→ _{cell.RightComparison.Relationship}_ →→→→ <br/>({cell.RightComparison.Key})" +
                                $"<hr/>" +
                                $"←←←← _{cell.RightCell.LeftComparison?.Relationship}_ ←←←← <br/>({cell.RightCell.LeftComparison?.Key}) ");
                        }
                    }
                }

                writer.WriteLine();
            }

            // check for unused codes in value sets
            for (int i = 0; i < valueSetRow.Length; i++)
            {
                if (i != 0)
                {
                    writer.Write("| ");
                }

                if (valueSetRow[i] == null)
                {
                    writer.Write("| ");
                }
                else
                {
                    writer.Write($"| *{codesPerVs[i].Count} of {valueSetRow[i]!.Vs.ConceptCount} codes used* ");
                    if (codesPerVs[i].Count < valueSetRow[i]!.Vs.ConceptCount)
                    {
                        HashSet<string> allCodes = valueSetRow[i]!.Concepts.Select(c => c.FhirKey).ToHashSet();
                        IEnumerable<string> unusedCodes = allCodes.Except(codesPerVs[i]);
                        writer.Write($"<br/>remaining codes:<br/>{string.Join(", ", unusedCodes.Select(v => "`" + v.Split('#')[^1] + "`"))}");
                    }
                }
            }

            writer.WriteLine();
            writer.WriteLine();
        }

        return;
    }


    private (string to, string from) getMappingMdTableCell(DbGraphVs.DbVsCell cell, bool movingRight)
    {
        DbGraphVs.DbVsCell? targetCell = movingRight ? cell.RightCell : cell.LeftCell;

        if (targetCell == null)
        {
            return ("<br/>*no map*<br/>", "<br/>*no map*<br/>");
        }

        DbValueSetComparison? toComparison = movingRight ? cell.RightComparison : cell.LeftComparison;
        DbValueSetComparison? fromComparison = movingRight ? targetCell.LeftComparison : cell.RightComparison;

        return (getLink(toComparison, targetCell, "→→→→→→→"), getLink(fromComparison, targetCell, "←←←←←←←"));

        string getLink(DbValueSetComparison? comparison, DbGraphVs.DbVsCell? target, string arrows)
        {
            if ((comparison == null) || (target == null))
            {
                return arrows + "<br/>*no map*<br/>" + arrows;
            }

            return
                $"{arrows}" +
                $"<br/>`{comparison.Relationship}`" +
                $"<br/>- DBKey: `{comparison.Key}`" +
                $"<br/>- Reviewed: `{comparison.LastReviewedOn?.ToString("o") ?? "n/a"}`" +
                $"<br/>- By: `{comparison.LastReviewedBy ?? "n/a"}`" +
                $"<br/>- Identical: `{comparison.IsIdentical ?? false}`" +
                $"<br/>{arrows}";

            //return $"[{comparison.CompositeName.ForMdTable()} ({comparison.Key})]" +
            //    $"(/input/codes_v2/{cell.DC.FhirSequence.ToRLiteral()}to{target.DC.FhirSequence.ToRLiteral()}/ConceptMap-{comparison.Name}.json)";
        }
    }

}
