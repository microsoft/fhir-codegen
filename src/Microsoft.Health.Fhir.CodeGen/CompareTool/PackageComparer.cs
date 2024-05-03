// <copyright file="PackageComparer.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGen.Configuration;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;
using Microsoft.Health.Fhir.CodeGen.Language;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using static Hl7.Fhir.Model.VerificationResult;
using static Microsoft.Health.Fhir.CodeGen.CompareTool.PackageComparer;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;

namespace Microsoft.Health.Fhir.CodeGen.CompareTool;

public class PackageComparer
{
    private DefinitionCollection _left;
    private DefinitionCollection _right;

    private string _leftPrefix;
    private string _rightPrefix;

    private ConfigCompare _config;

    private HttpClient? _httpClient = null;
    private Uri? _ollamaUri = null;

    private Dictionary<string, ComparisonRecord<ValueSetInfoRec, ConceptInfoRec>> _vsComparisons = [];

    public record class PackagePathRenames
    {
        public required string PackageDirectiveLeft { get; init; }
        public required string PackageDirectiveRight { get; init; }
        public required Dictionary<string, string> LeftRightPath { get; init; }
    }

    private static readonly HashSet<string> _exclusionSet =
    [
        /* UCUM is used as a required binding in a codeable concept. Since we do not
            * use enums in this situation, it is not useful to generate this valueset
            */
        "http://hl7.org/fhir/ValueSet/ucum-units",

        /* R5 made Resource.language a required binding to all-languages, which contains
         * all of bcp:47 and is listed as infinite. This is not useful to generate.
         * Note that in R5, many elements that are required to all-languages also have bound
         * starter value sets.  TODO: consider if we want to generate constants for those.
         */
        "http://hl7.org/fhir/ValueSet/all-languages",

        /* MIME types are infinite, so we do not want to generate these.
         * Note that in R5, many elements that are required to MIME type also have bound
         * starter value sets.  TODO: consider if we want to generate constants for those.
         */
        "http://hl7.org/fhir/ValueSet/mimetypes",
    ];

    public PackageComparer(ConfigCompare config, DefinitionCollection left, DefinitionCollection right)
    {
        _config = config;
        _left = left;
        _right = right;

        _leftPrefix = left.FhirSequence.ToRLiteral();
        _rightPrefix = right.FhirSequence.ToRLiteral();

        //if (!string.IsNullOrEmpty(config.OllamaUrl) &&
        //    !string.IsNullOrEmpty(config.OllamaModel))
        //{
        //    _httpClient = new HttpClient();
        //    _ollamaUri = config.OllamaUrl.EndsWith("generate", StringComparison.OrdinalIgnoreCase)
        //        ? new Uri(config.OllamaUrl)
        //        : new Uri(new Uri(config.OllamaUrl), "api/generate");
        //}
    }

    public PackageComparison Compare()
    {
        Console.WriteLine(
            $"Comparing {_left.MainPackageId}#{_left.MainPackageVersion}" +
            $" and {_right.MainPackageId}#{_right.MainPackageVersion}");

        string outputDir = Path.Combine(_config.OutputDirectory, $"{_leftPrefix}_{_rightPrefix}");

        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        // build our filename
        string mdFilename = "overview.md";

        string mdFullFilename = Path.Combine(outputDir, mdFilename);

        using ExportStreamWriter? mdWriter = _config.NoOutput ? null : CreateMarkdownWriter(mdFullFilename);

        // need to expand every value set for comparison
        _vsComparisons = Compare(GetValueSets(_left), GetValueSets(_right));
        if (mdWriter is not null)
        {
            WriteComparisonOverview(mdWriter, "Value Sets", _vsComparisons.Values);

            string subDir = Path.Combine(outputDir, "ValueSets");
            if (!Directory.Exists(subDir))
            {
                Directory.CreateDirectory(subDir);
            }

            foreach (ComparisonRecord<ValueSetInfoRec, ConceptInfoRec> c in _vsComparisons.Values)
            {
                string filename =
                    (c.Left is null)
                    ? Path.Combine(subDir, $"{_rightPrefix}_{c.Right!.Name.ToPascalCase()}.md")
                    : (c.Right is null)
                        ? Path.Combine(subDir, $"{_leftPrefix}_{c.Left.Name.ToPascalCase()}.md")
                        : Path.Combine(subDir, $"{_leftPrefix}_{c.Left.Name.ToPascalCase()}_{_rightPrefix}_{c.Right.Name.ToPascalCase()}.md");

                using ExportStreamWriter writer = CreateMarkdownWriter(filename);
                {
                    WriteComparisonFile(writer, string.Empty, c);
                }
            }
        }

        Dictionary<string, ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>> primitives = Compare(_left.PrimitiveTypesByName, _right.PrimitiveTypesByName);
        if (mdWriter is not null)
        {
            WriteComparisonOverview(mdWriter, "Primitive Types", primitives.Values);

            string subDir = Path.Combine(outputDir, "PrimitiveTypes");
            if (!Directory.Exists(subDir))
            {
                Directory.CreateDirectory(subDir);
            }

            foreach (ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec> c in primitives.Values)
            {
                string filename =
                    (c.Left is null)
                    ? Path.Combine(subDir, $"{_rightPrefix}_{c.Right!.Name.ToPascalCase()}.md")
                    : (c.Right is null)
                        ? Path.Combine(subDir, $"{_leftPrefix}_{c.Left.Name.ToPascalCase()}.md")
                        : Path.Combine(subDir, $"{_leftPrefix}_{c.Left.Name.ToPascalCase()}_{_rightPrefix}_{c.Right.Name.ToPascalCase()}.md");

                using ExportStreamWriter writer = CreateMarkdownWriter(filename);
                {
                    WriteComparisonFile(writer, string.Empty, c);
                }
            }
        }

        Dictionary<string, ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>> complexTypes = Compare(_left.ComplexTypesByName, _right.ComplexTypesByName);
        if (mdWriter is not null)
        {
            WriteComparisonOverview(mdWriter, "Complex Types", complexTypes.Values);

            string subDir = Path.Combine(outputDir, "ComplexTypes");
            if (!Directory.Exists(subDir))
            {
                Directory.CreateDirectory(subDir);
            }

            foreach (ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec> c in complexTypes.Values)
            {
                string filename =
                    (c.Left is null)
                    ? Path.Combine(subDir, $"{_rightPrefix}_{c.Right!.Name.ToPascalCase()}.md")
                    : (c.Right is null)
                        ? Path.Combine(subDir, $"{_leftPrefix}_{c.Left.Name.ToPascalCase()}.md")
                        : Path.Combine(subDir, $"{_leftPrefix}_{c.Left.Name.ToPascalCase()}_{_rightPrefix}_{c.Right.Name.ToPascalCase()}.md");

                using ExportStreamWriter writer = CreateMarkdownWriter(filename);
                {
                    WriteComparisonFile(writer, string.Empty, c);
                }
            }

        }

        Dictionary<string, ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>> resources = Compare(_left.ResourcesByName, _right.ResourcesByName);
        if (mdWriter is not null)
        {
            WriteComparisonOverview(mdWriter, "Resources", resources.Values);

            string subDir = Path.Combine(outputDir, "Resources");
            if (!Directory.Exists(subDir))
            {
                Directory.CreateDirectory(subDir);
            }

            foreach (ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec> c in resources.Values)
            {
                string filename =
                    (c.Left is null)
                    ? Path.Combine(subDir, $"{_rightPrefix}_{c.Right!.Name.ToPascalCase()}.md")
                    : (c.Right is null)
                        ? Path.Combine(subDir, $"{_leftPrefix}_{c.Left.Name.ToPascalCase()}.md")
                        : Path.Combine(subDir, $"{_leftPrefix}_{c.Left.Name.ToPascalCase()}_{_rightPrefix}_{c.Right.Name.ToPascalCase()}.md");

                using ExportStreamWriter writer = CreateMarkdownWriter(filename);
                {
                    WriteComparisonFile(writer, string.Empty, c);
                }
            }
        }

        Dictionary<string, ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>> logical = Compare(_left.LogicalModelsByName, _right.LogicalModelsByName);
        if (mdWriter is not null)
        {
            WriteComparisonOverview(mdWriter, "Logical Models", logical.Values);

            string subDir = Path.Combine(outputDir, "LogicalModels");
            if (!Directory.Exists(subDir))
            {
                Directory.CreateDirectory(subDir);
            }

            foreach (ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec> c in logical.Values)
            {
                string filename =
                    (c.Left is null)
                    ? Path.Combine(subDir, $"{_rightPrefix}_{c.Right!.Name.ToPascalCase()}.md")
                    : (c.Right is null)
                        ? Path.Combine(subDir, $"{_leftPrefix}_{c.Left.Name.ToPascalCase()}.md")
                        : Path.Combine(subDir, $"{_leftPrefix}_{c.Left.Name.ToPascalCase()}_{_rightPrefix}_{c.Right.Name.ToPascalCase()}.md");

                using ExportStreamWriter writer = CreateMarkdownWriter(filename);
                {
                    WriteComparisonFile(writer, string.Empty, c);
                }
            }
        }

        PackageComparison packageComparison = new()
        {
            LeftPackageId = _left.MainPackageId,
            LeftPackageVersion = _left.MainPackageVersion,
            RightPackageId = _right.MainPackageId,
            RightPackageVersion = _right.MainPackageVersion,
            ValueSets = _vsComparisons,
            PrimitiveTypes = primitives,
            ComplexTypes = complexTypes,
            Resources = resources,
            LogicalModels = logical,
        };

        if (mdWriter is not null)
        {
            mdWriter.Flush();
            mdWriter.Close();
            mdWriter.Dispose();
        }

        if (_config.SaveComparisonResult)
        {
            string jsonFilename = Path.Combine(outputDir, "comparison.json");

            using FileStream jsonFs = new(jsonFilename, FileMode.Create, FileAccess.Write);
            using Utf8JsonWriter jsonWriter = new(jsonFs, new JsonWriterOptions() { Indented = false, });
            {
                JsonSerializer.Serialize(jsonWriter, packageComparison);
            }
        }

        return packageComparison;
    }

    private Dictionary<string, ValueSet> GetValueSets(DefinitionCollection dc)
    {
        Dictionary<string, ValueSet> valueSets = [];

        foreach ((string unversionedUrl, string[] versions) in dc.ValueSetVersions.OrderBy(kvp => kvp.Key))
        {
            if (_exclusionSet.Contains(unversionedUrl))
            {
                continue;
            }

            // only use the latest version
            string vsVersion = versions.OrderDescending().First();
            string versionedUrl = unversionedUrl + "|" + vsVersion;

            IEnumerable<StructureElementCollection> coreBindingsVersioned = dc.CoreBindingsForVs(versionedUrl);
            Hl7.Fhir.Model.BindingStrength? strongestBindingV = dc.StrongestBinding(coreBindingsVersioned);

            IEnumerable<StructureElementCollection> coreBindingsUnversioned = dc.CoreBindingsForVs(unversionedUrl);
            Hl7.Fhir.Model.BindingStrength? strongestBindingU = dc.StrongestBinding(coreBindingsUnversioned);

            if ((strongestBindingV != Hl7.Fhir.Model.BindingStrength.Required) &&
                (strongestBindingU != Hl7.Fhir.Model.BindingStrength.Required))
            {
                continue;
            }

            // always expand based on the versioned
            if (!dc.TryExpandVs(versionedUrl, out ValueSet? vs))
            {
                continue;
            }

            valueSets.Add(unversionedUrl, vs);
        }

        return valueSets;
    }

    private void WriteStructureConceptMap(PackageComparison pc)
    {

    }

    private void WriteElementConceptMap(PackageComparison pc)
    {

    }

    private void WriteComparisonSummaryTableDict(
        ExportStreamWriter writer,
        IEnumerable<IComparisonRecord> values)
    {
        Dictionary<string, int> counts = new Dictionary<string, int>();

        // build summary data
        foreach (IComparisonRecord c in values)
        {
            string status = c.GetStatusString();
            if (!counts.TryGetValue(status, out int count))
            {
                count = 0;
            }

            counts[status] = count + 1;
        }

        writer.WriteLine("| Status | Count |");
        writer.WriteLine("| ------ | ----- |");
        foreach ((string status, int count) in counts.OrderBy(kvp => kvp.Key))
        {
            writer.WriteLine($"{status} | {count} |");
        }
        writer.WriteLine();
        writer.WriteLine();
    }

    private void WriteComparisonOverview(
        ExportStreamWriter writer,
        string header,
        IEnumerable<IComparisonRecord> values)
    {
        writer.WriteLine("## " + header);

        WriteComparisonSummaryTableDict(writer, values);

        writer.WriteLine("<details>");
        writer.WriteLine("<summary>Entry details</summary>");

        writer.WriteLine();

        writer.WriteLine("| Key | Source | Dest | Status | Message |");
        writer.WriteLine("| --- | ------ | ---- | ------ | ------- |");

        foreach (IComparisonRecord c in values.OrderBy(cr => cr.Key))
        {
            writer.WriteLine(c.GetMarkdownDetailTableRow());
        }

        writer.WriteLine();

        writer.WriteLine("</details>");
        writer.WriteLine();
    }

    private void WriteComparisonFile(
        ExportStreamWriter writer,
        string header,
        IComparisonRecord cRec)
    {
        if (!string.IsNullOrEmpty(header))
        {
            writer.WriteLine("## " + header);
        }

        writer.WriteLine(cRec.GetMarkdownTable());

        writer.WriteLine();
        writer.WriteLine($"#### Comparison Result: {cRec.GetStatusString()}");
        writer.WriteLine();
        writer.WriteLine();

        writer.WriteLine(cRec.GetMarkdownSummaryTable());
        writer.WriteLine(cRec.GetMarkdownDetailTable());
    }


    private ExportStreamWriter CreateMarkdownWriter(string filename, bool writeGenerationHeader = true)
    {
        ExportStreamWriter writer = new(filename);

        if (writeGenerationHeader)
        {
            writer.WriteLine($"Comparison of {_left.MainPackageId}#{_left.MainPackageVersion} and {_right.MainPackageId}#{_right.MainPackageVersion}");
            writer.WriteLine($"Generated at {DateTime.Now.ToString("F")}");
            writer.WriteLine();
        }

        return writer;
    }


    private void LoadKnownChanges()
    {
        // TODO(ginoc): implement
    }

    private bool TryCompare(string conceptCode, ConceptInfoRec? left, ConceptInfoRec? right, [NotNullWhen(true)] out ComparisonRecord<ConceptInfoRec>? c)
    {
        if ((left is null) && (right is null))
        {
            c = null;
            return false;
        }

        if (left is null)
        {
            c = new()
            {
                Key = conceptCode,
                Left = null,
                Right = right,
                NamedMatch = false,
                Relationship = null,
                Message = $"{_rightPrefix} added code {conceptCode}",
            };
            return true;
        }

        if (right is null)
        {
            c = new()
            {
                Key = conceptCode,
                Left = left,
                Right = null,
                NamedMatch = false,
                Relationship = null,
                Message = $"{_rightPrefix} removed code {conceptCode}",
            };
            return true;
        }

        List<string> messages = [];

        if (left.System != right.System)
        {
            messages.Add($"has a different system");
        }

        if (left.Description != right.Description)
        {
            messages.Add($"has a different description");
        }

        string message = messages.Count == 0
            ? $"{_rightPrefix} code is equivalent"
            : $"{_rightPrefix} " + string.Join(" and ", messages);

        // note that we can only be here if the codes have already matched, so we are always equivalent
        c = new()
        {
            Key = conceptCode,
            Left = left,
            Right = right,
            NamedMatch = true,
            Relationship = CMR.Equivalent,
            Message = message,
        };
        return true;
    }

    /// <summary>
    /// Tries to compare the type information for two elements and returns a comparison record.
    /// </summary>
    /// <param name="elementName">The name of the element.</param>
    /// <param name="left">The left element type info to compare.</param>
    /// <param name="right">The right element type info to compare.</param>
    /// <param name="c">The comparison record of the elements.</param>
    /// <returns>True if the comparison is successful, false otherwise.</returns>
    private bool TryCompare(
        string typeName,
        ElementTypeInfoRec? left,
        ElementTypeInfoRec? right,
        [NotNullWhen(true)] out ComparisonRecord<ElementTypeInfoRec>? c)
    {
        if ((left is null) && (right is null))
        {
            c = null;
            return false;
        }

        if (left is null)
        {
            c = new()
            {
                Key = typeName,
                Left = null,
                Right = right,
                NamedMatch = false,
                Relationship = null,
                Message = $"{_rightPrefix} added type {typeName}",
            };
            return true;
        }

        if (right is null)
        {
            c = new()
            {
                Key = typeName,
                Left = left,
                Right = null,
                NamedMatch = false,
                Relationship = null,
                Message = $"{_rightPrefix} removed type {typeName}",
            };
            return true;
        }

        // TODO: check existing type maps
        if (left.Name != right.Name)
        {
            throw new Exception("Type names do not match");
        }

        // once we know the types are the same, we want to determine all differences in profiles and target profiles
        List<string> addedProfiles = [];
        List<string> removedProfiles = [];

        HashSet<string> scratch = right.Profiles.ToHashSet();

        foreach (string lp in left.Profiles)
        {
            if (scratch.Contains(lp))
            {
                scratch.Remove(lp);
                continue;
            }

            removedProfiles.Add(lp);
        }

        addedProfiles.AddRange(scratch);

        List<string> addedTargets = [];
        List<string> removedTargets = [];

        scratch = right.TargetProfiles.ToHashSet();

        foreach (string lp in left.TargetProfiles)
        {
            if (scratch.Contains(lp))
            {
                scratch.Remove(lp);
                continue;
            }

            removedTargets.Add(lp);
        }

        addedTargets.AddRange(scratch);

        // start with assuming the types are equivalent
        CMR relationship = CMR.Equivalent;
        List<string> messages = [];

        if (addedProfiles.Any())
        {
            relationship = ApplyRelationship(relationship, CMR.SourceIsNarrowerThanTarget);
            messages.Add($"added profiles: {string.Join(", ", addedProfiles)}");
        }

        if (removedProfiles.Any())
        {
            relationship = ApplyRelationship(relationship, CMR.SourceIsBroaderThanTarget);
            messages.Add($"removed profiles: {string.Join(", ", removedProfiles)}");
        }

        if (addedTargets.Any())
        {
            relationship = ApplyRelationship(relationship, CMR.SourceIsNarrowerThanTarget);
            messages.Add($"added target profiles: {string.Join(", ", addedTargets)}");
        }

        if (removedTargets.Any())
        {
            relationship = ApplyRelationship(relationship, CMR.SourceIsBroaderThanTarget);
            messages.Add($"removed target profiles: {string.Join(", ", removedTargets)}");
        }

        string message = messages.Count == 0
            ? $"{_rightPrefix} type {right.Name} is equivalent."
            : $"{_rightPrefix} type " + string.Join(" and ", messages);

        c = new()
        {
            Key = typeName,
            Left = left,
            Right = right,
            NamedMatch = true,
            Relationship = relationship,
            Message = message,
        };
        return true;
    }

    private CMR ApplyRelationship(CMR existing, CMR? change) => existing switch
    {
        CMR.Equivalent => change ?? CMR.Equivalent,
        CMR.RelatedTo => (change == CMR.NotRelatedTo) ? CMR.NotRelatedTo : existing,
        CMR.SourceIsNarrowerThanTarget => (change == CMR.SourceIsNarrowerThanTarget || change == CMR.Equivalent)
            ? CMR.SourceIsNarrowerThanTarget : CMR.RelatedTo,
        CMR.SourceIsBroaderThanTarget => (change == CMR.SourceIsBroaderThanTarget || change == CMR.Equivalent)
            ? CMR.SourceIsBroaderThanTarget : CMR.RelatedTo,
        CMR.NotRelatedTo => change ?? existing,
        _ => change ?? existing,
    };

    private bool TryCompare(
        string url,
        ValueSetInfoRec? left,
        ValueSetInfoRec? right,
        Dictionary<string, ComparisonRecord<ConceptInfoRec>> conceptComparison,
        [NotNullWhen(true)] out ComparisonRecord<ValueSetInfoRec, ConceptInfoRec>? c)
    {
        if ((left is null) && (right is null))
        {
            c = null;
            return false;
        }

        if (left is null)
        {
            c = new()
            {
                Key = url,
                Left = null,
                Right = right,
                NamedMatch = false,
                Relationship = null,
                Message = $"{_rightPrefix} added value set: {url}",
                Children = conceptComparison,
            };
            return true;
        }

        if (right is null)
        {
            c = new()
            {
                Key = url,
                Left = left,
                Right = null,
                NamedMatch = false,
                Relationship = null,
                Message = $"{_rightPrefix} removed value set: {url}",
                Children = conceptComparison,
            };
            return true;
        }

        // check for all concepts being equivalent
        if (conceptComparison.Values.All(cc => cc.Relationship == CMR.Equivalent))
        {
            c = new()
            {
                Key = url,
                Left = left,
                Right = right,
                NamedMatch = true,
                Relationship = CMR.Equivalent,
                Message = $"{_rightPrefix}:{url} is equivalent",
                Children = conceptComparison,
            };
            return true;
        }

        CMR relationship = CMR.Equivalent;
        foreach (ComparisonRecord<ConceptInfoRec> ec in conceptComparison.Values)
        {
            if (ec.Relationship == CMR.Equivalent)
            {
                continue;
            }

            if (ec.Left is null)
            {
                relationship = ApplyRelationship(relationship, CMR.SourceIsNarrowerThanTarget);
                continue;
            }

            if (ec.Right is null)
            {
                relationship = ApplyRelationship(relationship, CMR.SourceIsBroaderThanTarget);
                continue;
            }

            relationship = ApplyRelationship(relationship, ec.Relationship);
        }

        string message = relationship switch
        {
            CMR.Equivalent => $"{_rightPrefix}:{url} is equivalent",
            CMR.RelatedTo => $"{_rightPrefix}:{url} is related to {_leftPrefix}:{url} (see concepts for details)",
            CMR.SourceIsNarrowerThanTarget => $"{_rightPrefix}:{url} subsumes {_leftPrefix}:{url}",
            CMR.SourceIsBroaderThanTarget => $"{_rightPrefix}:{url} is subsumed by {_leftPrefix}:{url}",
            _ => $"{_rightPrefix}:{url} is related to {_leftPrefix}:{url} (see concepts for details)",
        };

        c = new()
        {
            Key = url,
            Left = left,
            Right = right,
            NamedMatch = true,
            Relationship = relationship,
            Message = message,
            Children = conceptComparison,
        };
        return true;
    }

    private bool TryCompare(
        string edPath,
        ElementInfoRec? left,
        ElementInfoRec? right,
        Dictionary<string, ComparisonRecord<ElementTypeInfoRec>> typeComparison,
        [NotNullWhen(true)] out ComparisonRecord<ElementInfoRec, ElementTypeInfoRec>? c)
    {
        if ((left is null) && (right is null))
        {
            c = null;
            return false;
        }

        if (left is null)
        {
            c = new()
            {
                Key = edPath,
                Left = null,
                Right = right,
                NamedMatch = false,
                Relationship = null,
                Message = $"{_rightPrefix} added element {edPath}",
                Children = typeComparison,
            };
            return true;
        }

        if (right is null)
        {
            c = new()
            {
                Key = edPath,
                Left = left,
                Right = null,
                NamedMatch = false,
                Relationship = null,
                Message = $"{_rightPrefix} removed element {edPath}",
                Children = typeComparison,
            };
            return true;
        }

        // start with assuming the elements are equivalent
        CMR relationship = CMR.Equivalent;
        List<string> messages = [];

        // check for optional becoming mandatory
        if ((left.MinCardinality == 0) && (right.MinCardinality != 0))
        {
            relationship = ApplyRelationship(relationship, CMR.RelatedTo);
            messages.Add("made the element mandatory");
        }

        // check for source allowing fewer than destination requires
        if (left.MinCardinality < right.MinCardinality)
        {
            relationship = ApplyRelationship(relationship, CMR.RelatedTo);
            messages.Add($"increased the minimum cardinality from {left.MinCardinality} to {right.MinCardinality}");
        }

        // check for element being constrained out
        if ((left.MaxCardinality != 0) && (right.MaxCardinality == 0))
        {
            relationship = ApplyRelationship(relationship, CMR.RelatedTo);
            messages.Add("constrained the element out (max cardinality of 0)");
        }

        // check for changing from scalar to array
        if ((left.MaxCardinality == 1) && (right.MaxCardinality != 1))
        {
            relationship = ApplyRelationship(relationship, CMR.RelatedTo);
            messages.Add($"changed from scalar to array (max cardinality from {left.MaxCardinalityString} to {right.MaxCardinalityString})");
        }

        // check for changing from array to scalar
        if ((left.MaxCardinality != 1) && (right.MaxCardinality == 1))
        {
            relationship = ApplyRelationship(relationship, CMR.RelatedTo);
            messages.Add($"changed from array to scalar (max cardinality from {left.MaxCardinalityString} to {right.MaxCardinalityString})");
        }

        // check for source allowing more than destination allows
        if ((right.MaxCardinality != -1) &&
            (left.MaxCardinality > right.MaxCardinality))
        {
            relationship = ApplyRelationship(relationship, CMR.RelatedTo);
            messages.Add($"allows more repetitions (max cardinality from {left.MaxCardinalityString} to {right.MaxCardinalityString})");
        }

        // check to see if there was not a required binding and now there is
        if ((left.ValueSetBindingStrength is not null) || (right.ValueSetBindingStrength is not null))
        {
            if ((left.ValueSetBindingStrength != BindingStrength.Required) && (right.ValueSetBindingStrength == BindingStrength.Required))
            {
                relationship = ApplyRelationship(relationship, CMR.RelatedTo);

                if (left.ValueSetBindingStrength is null)
                {
                    messages.Add($"added a required binding to {right.BindingValueSet}");
                }
                else
                {
                    messages.Add($"made the binding required (from {left.ValueSetBindingStrength}) for {right.BindingValueSet}");
                }
            }
            else if (left.ValueSetBindingStrength != right.ValueSetBindingStrength)
            {
                relationship = ApplyRelationship(relationship, CMR.RelatedTo);
                if (left.ValueSetBindingStrength is null)
                {
                    messages.Add($"added a binding requirement - {right.ValueSetBindingStrength} {right.BindingValueSet}");
                }
                else if (right.ValueSetBindingStrength is null)
                {
                    messages.Add($"removed a binding requirement - {left.ValueSetBindingStrength} {left.BindingValueSet}");
                }
                else
                {
                    messages.Add($"changed the binding strength from {left.ValueSetBindingStrength} to {right.ValueSetBindingStrength}");
                }
            }

            // check to see if we need to lookup a binding comparison
            if ((left.ValueSetBindingStrength == BindingStrength.Required) && (right.ValueSetBindingStrength == BindingStrength.Required))
            {
                // TODO(ginoc): For sanity right now, we assume that the value sets are from the matching releases
                // at some point, we need to check specific versions in case there are explicit references

                string unversionedLeft = left.BindingValueSet.Split('|')[0];
                string unversionedRight = right.BindingValueSet.Split('|')[0];

                // if the types are code, we only need to compare codes
                if (typeComparison.ContainsKey("code"))
                {
                    // check for same value set
                    if (unversionedLeft == unversionedRight)
                    {
                        // look for the value set comparison
                        if (_vsComparisons.TryGetValue(unversionedLeft, out ComparisonRecord<ValueSetInfoRec, ConceptInfoRec>? boundVsInfo))
                        {
                            // we are okay with equivalent and narrower
                            if (boundVsInfo.Relationship == CMR.Equivalent ||
                                boundVsInfo.Relationship == CMR.SourceIsNarrowerThanTarget)
                            {
                                relationship = ApplyRelationship(relationship, (CMR)boundVsInfo.Relationship);
                                messages.Add($"has compatible required binding for code type: {left.BindingValueSet} and {right.BindingValueSet} ({boundVsInfo.Relationship})");
                            }

                            // check to see if the codes are the same but the systems are different (ok in codes)
                            else if (boundVsInfo.Children.Values.All(cc => cc.Left?.Code == cc.Right?.Code))
                            {
                                relationship = ApplyRelationship(relationship, CMR.Equivalent);
                                messages.Add($"has compatible required binding for code type: {left.BindingValueSet} and {right.BindingValueSet} (codes match, though systems are different)");
                            }
                            else
                            {
                                relationship = ApplyRelationship(relationship, boundVsInfo.Relationship);
                                messages.Add($"has INCOMPATIBLE required binding for code type: {left.BindingValueSet} and {right.BindingValueSet}");
                            }
                        }
                        else if (_exclusionSet.Contains(unversionedLeft))
                        {
                            relationship = ApplyRelationship(relationship, CMR.Equivalent);
                            messages.Add($"{unversionedLeft} is exempted and assumed equivalent");
                        }
                        else
                        {
                            relationship = ApplyRelationship(relationship, CMR.RelatedTo);
                            messages.Add($"(failed to compare required binding of {left.BindingValueSet})");
                        }
                    }
                    // since these are codes only, we can look for the value set comparisons and check codes across them
                    else if (!_vsComparisons.TryGetValue(unversionedLeft, out ComparisonRecord<ValueSetInfoRec, ConceptInfoRec>? leftVsI) ||
                             !_vsComparisons.TryGetValue(unversionedRight, out ComparisonRecord<ValueSetInfoRec, ConceptInfoRec>? rightVsI))
                    {
                        relationship = ApplyRelationship(relationship, CMR.RelatedTo);
                        messages.Add($"(failed to compare required binding of {left.BindingValueSet} and {right.BindingValueSet})");
                    }

                    // check for any codes from the left binding source not being present in the right binding destination
                    else if (leftVsI.Children.Values.Any(lc => !rightVsI.Children.Values.Any(rc => lc.Left?.Code == rc.Right?.Code)))
                    {
                        relationship = ApplyRelationship(relationship, CMR.RelatedTo);
                        messages.Add($"has INCOMPATIBLE required binding for code type: {left.BindingValueSet} and {right.BindingValueSet}");
                    }
                    else if (_exclusionSet.Contains(unversionedLeft) && _exclusionSet.Contains(unversionedRight))
                    {
                        relationship = ApplyRelationship(relationship, CMR.Equivalent);
                        messages.Add($"{unversionedLeft} and {unversionedRight} are exempted and assumed equivalent");
                    }
                    else
                    {
                        relationship = ApplyRelationship(relationship, leftVsI.Children.Count == rightVsI.Children.Count ? CMR.Equivalent : CMR.SourceIsNarrowerThanTarget);
                        messages.Add($"has compatible required binding for code type: {left.BindingValueSet} and {right.BindingValueSet}");
                    }
                }

                // check for any non-code types (need to match system)
                if (typeComparison.Any(t => t.Key != "code"))
                {
                    // check for same value set (non-code type)
                    if (unversionedLeft == unversionedRight)
                    {
                        // look for the value set comparison
                        if (!_vsComparisons.TryGetValue(unversionedLeft, out ComparisonRecord<ValueSetInfoRec, ConceptInfoRec>? boundVsInfo))
                        {
                            relationship = ApplyRelationship(relationship, CMR.RelatedTo);
                            messages.Add($"(failed to compare required binding of {left.BindingValueSet} and {right.BindingValueSet})");
                        }
                        // we are okay with equivalent and narrower
                        else if (boundVsInfo.Relationship == CMR.Equivalent ||
                                 boundVsInfo.Relationship == CMR.SourceIsNarrowerThanTarget)
                        {
                            relationship = ApplyRelationship(relationship, (CMR)boundVsInfo.Relationship);
                            messages.Add($"has compatible required binding for non-code type: {left.BindingValueSet} and {right.BindingValueSet} ({boundVsInfo.Relationship})");
                        }
                        else if (_exclusionSet.Contains(unversionedLeft))
                        {
                            relationship = ApplyRelationship(relationship, CMR.Equivalent);
                            messages.Add($"{unversionedLeft} is exempted and assumed equivalent");
                        }
                        else
                        {
                            relationship = ApplyRelationship(relationship, boundVsInfo.Relationship);
                            messages.Add($"has INCOMPATIBLE required binding for non-code type: {left.BindingValueSet} and {right.BindingValueSet}");
                        }
                    }
                    // since these are not only codes, but are different value sets, we can look for the value set comparisons and check system+codes across them
                    else if (!_vsComparisons.TryGetValue(unversionedLeft, out ComparisonRecord<ValueSetInfoRec, ConceptInfoRec>? leftVs) ||
                             !_vsComparisons.TryGetValue(unversionedRight, out ComparisonRecord<ValueSetInfoRec, ConceptInfoRec>? rightVs))
                    {
                        relationship = ApplyRelationship(relationship, CMR.RelatedTo);
                        messages.Add($"(failed to compare required binding of {left.BindingValueSet} and {right.BindingValueSet})");
                    }
                    // check for any keys (system+code) from the left binding source not being present in the right binding destination
                    else if (leftVs.Children.Keys.Any(lk => !rightVs.Children.ContainsKey(lk)))
                    {
                        relationship = ApplyRelationship(relationship, CMR.RelatedTo);
                        messages.Add($"has INCOMPATIBLE required binding for non-code type: {left.BindingValueSet} and {right.BindingValueSet}");
                    }
                    else if (_exclusionSet.Contains(unversionedLeft) && _exclusionSet.Contains(unversionedRight))
                    {
                        relationship = ApplyRelationship(relationship, CMR.Equivalent);
                        messages.Add($"{unversionedLeft} and {unversionedRight} are exempted and assumed equivalent");
                    }
                    else
                    {
                        relationship = ApplyRelationship(relationship, leftVs.Children.Count == rightVs.Children.Count ? CMR.Equivalent : CMR.SourceIsNarrowerThanTarget);
                        messages.Add($"has compatible required binding for code type: {left.BindingValueSet} and {right.BindingValueSet}");
                    }
                }
            }
        }

        // process our type comparisons and promote messages
        foreach (ComparisonRecord<ElementTypeInfoRec> tc in typeComparison.Values)
        {
            // skip equivalent types
            if (tc.Relationship == CMR.Equivalent)
            {
                continue;
            }

            relationship = ApplyRelationship(relationship, tc.Relationship);
            messages.Add($"has change due to type change: {tc.Message}");
        }

        // build our message
        string message = messages.Count == 0
            ? $"{_rightPrefix}:{edPath} is equivalent."
            : $"{_rightPrefix}:{edPath} " + string.Join(" and ", messages);

        // return our info
        c = new()
        {
            Key = edPath,
            Left = left,
            Right = right,
            NamedMatch = true,
            Relationship = relationship,
            Message = message,
            Children = typeComparison,
        };
        return true;
    }


    private bool TryCompare(
        string sdName,
        StructureInfoRec? left,
        StructureInfoRec? right,
        Dictionary<string, ComparisonRecord<ElementInfoRec, ElementTypeInfoRec>> elementComparison,
        [NotNullWhen(true)] out ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>? c)
    {
        if ((left is null) && (right is null))
        {
            c = null;
            return false;
        }

        if (left is null)
        {
            c = new()
            {
                Key = sdName,
                Left = null,
                Right = right,
                NamedMatch = false,
                Relationship = null,
                Message = $"{_rightPrefix} added {sdName}",
                Children = elementComparison,
            };
            return true;
        }

        if (right is null)
        {
            c = new()
            {
                Key = sdName,
                Left = left,
                Right = null,
                NamedMatch = false,
                Relationship = null,
                Message = $"{_rightPrefix} removed {sdName}",
                Children = elementComparison,
            };
            return true;
        }

        // check for all elements being the same
        if (elementComparison.Values.All(ec => ec.Relationship == CMR.Equivalent))
        {
            c = new()
            {
                Key = sdName,
                Left = left,
                Right = right,
                NamedMatch = true,
                Relationship = CMR.Equivalent,
                Message = $"{_rightPrefix}:{sdName} is equivalent",
                Children = elementComparison,
            };
            return true;
        }

        CMR relationship = CMR.Equivalent;

        foreach (ComparisonRecord<ElementInfoRec, ElementTypeInfoRec> ec in elementComparison.Values)
        {
            if (ec.Relationship == CMR.Equivalent)
            {
                continue;
            }

            if (ec.Left is null)
            {
                relationship = ApplyRelationship(relationship, CMR.SourceIsNarrowerThanTarget);
                continue;
            }

            if (ec.Right is null)
            {
                relationship = ApplyRelationship(relationship, CMR.SourceIsBroaderThanTarget);
                continue;
            }

            relationship = ApplyRelationship(relationship, ec.Relationship);
        }

        string message = relationship switch
        {
            CMR.Equivalent => $"{_rightPrefix}:{sdName} is equivalent",
            CMR.RelatedTo => $"{_rightPrefix}:{sdName} is related to {_leftPrefix}:{sdName} (see elements for details)",
            CMR.SourceIsNarrowerThanTarget => $"{_rightPrefix}:{sdName} subsumes {_leftPrefix}:{sdName}",
            CMR.SourceIsBroaderThanTarget => $"{_rightPrefix}:{sdName} is subsumed by {_leftPrefix}:{sdName}",
            _ => $"{_rightPrefix}:{sdName} is related to {_leftPrefix}:{sdName} (see elements for details)",
        };

        c = new()
        {
            Key = sdName,
            Left = left,
            Right = right,
            NamedMatch = true,
            Relationship = relationship,
            Message = message,
            Children = elementComparison,
        };
        return true;
    }

    private Dictionary<string, ComparisonRecord<ConceptInfoRec>> Compare(
        IReadOnlyDictionary<string, FhirConcept> leftConcepts,
        IReadOnlyDictionary<string, FhirConcept> rightConcepts)
    {
        Dictionary<string, ConceptInfoRec> left = leftConcepts.ToDictionary(kvp => kvp.Key, kvp => GetInfo(kvp.Value));
        Dictionary<string, ConceptInfoRec> right = rightConcepts.ToDictionary(kvp => kvp.Key, kvp => GetInfo(kvp.Value));

        Dictionary<string, ComparisonRecord<ConceptInfoRec>> comparison = [];

        IEnumerable<string> keys = left.Keys.Union(right.Keys).Distinct();

        foreach (string conceptCode in keys)
        {
            _ = left.TryGetValue(conceptCode, out ConceptInfoRec? leftInfo);
            _ = right.TryGetValue(conceptCode, out ConceptInfoRec? rightInfo);

            if (TryCompare(conceptCode, leftInfo, rightInfo, out ComparisonRecord<ConceptInfoRec>? c))
            {
                comparison.Add(conceptCode, c);
            }
        }

        return comparison;
    }

    private Dictionary<string, ComparisonRecord<ValueSetInfoRec, ConceptInfoRec>> Compare(
        IReadOnlyDictionary<string, ValueSet> leftInput,
        IReadOnlyDictionary<string, ValueSet> rightInput)
    {
        Dictionary<string, ValueSetInfoRec> left = leftInput.ToDictionary(kvp => _left.UnversionedUrlForVs(kvp.Key), kvp => GetInfo(kvp.Value));
        Dictionary<string, ValueSetInfoRec> right = rightInput.ToDictionary(kvp => _right.UnversionedUrlForVs(kvp.Key), kvp => GetInfo(kvp.Value));

        Dictionary<string, ComparisonRecord<ValueSetInfoRec, ConceptInfoRec>> comparison = [];

        IEnumerable<string> keys = left.Keys.Union(right.Keys).Distinct();

        foreach (string url in keys)
        {
            _ = left.TryGetValue(url, out ValueSetInfoRec? leftInfo);
            _ = right.TryGetValue(url, out ValueSetInfoRec? rightInfo);

            Dictionary<string, FhirConcept> leftConcepts = [];
            Dictionary<string, FhirConcept> rightConcepts = [];

            if (leftInput.TryGetValue(url, out ValueSet? leftVs))
            {
                IEnumerable<FhirConcept> flat = leftVs.cgGetFlatConcepts(_left);
                foreach (FhirConcept concept in flat)
                {
                    _ = leftConcepts.TryAdd(concept.Key, concept);
                }

                leftInfo = leftInfo! with { ConceptCount = leftConcepts.Count };
                left[url] = leftInfo;
            }

            if (rightInput.TryGetValue(url, out ValueSet? rightVs))
            {
                IEnumerable<FhirConcept> flat = rightVs.cgGetFlatConcepts(_left);
                foreach (FhirConcept concept in flat)
                {
                    _ = rightConcepts.TryAdd(concept.Key, concept);
                }

                rightInfo = rightInfo! with { ConceptCount = rightConcepts.Count };
                right[url] = rightInfo;
            }

            // compare our concepts
            Dictionary<string, ComparisonRecord<ConceptInfoRec>> conceptComparison = Compare(leftConcepts, rightConcepts);

            if (TryCompare(url, leftInfo, rightInfo, conceptComparison, out ComparisonRecord<ValueSetInfoRec, ConceptInfoRec>? c))
            {
                comparison.Add(url, c);
            }
        }

        return comparison;
    }

    private Dictionary<string, ComparisonRecord<ElementTypeInfoRec>> Compare(
        IReadOnlyDictionary<string, ElementDefinition.TypeRefComponent> leftInput,
        IReadOnlyDictionary<string, ElementDefinition.TypeRefComponent> rightInput)
    {
        Dictionary<string, ElementTypeInfoRec> left = leftInput.ToDictionary(kvp => kvp.Key, kvp => GetInfo(kvp.Value));
        Dictionary<string, ElementTypeInfoRec> right = rightInput.ToDictionary(kvp => kvp.Key, kvp => GetInfo(kvp.Value));

        Dictionary<string, ComparisonRecord<ElementTypeInfoRec>> comparison = [];

        IEnumerable<string> keys = left.Keys.Union(right.Keys).Distinct();

        // add our comparisons
        foreach (string typeName in keys)
        {
            _ = left.TryGetValue(typeName, out ElementTypeInfoRec? leftInfo);
            _ = right.TryGetValue(typeName, out ElementTypeInfoRec? rightInfo);

            if (TryCompare(typeName, leftInfo, rightInfo, out ComparisonRecord<ElementTypeInfoRec>? c))
            {
                comparison.Add(typeName, c);
            }
        }

        return comparison;
    }

    private Dictionary<string, ComparisonRecord<ElementInfoRec, ElementTypeInfoRec>> Compare(
        IReadOnlyDictionary<string, ElementDefinition> leftDict,
        IReadOnlyDictionary<string, ElementDefinition> rightDict)
    {
        Dictionary<string, ElementInfoRec> leftInfoDict = leftDict.ToDictionary(kvp => kvp.Value.Path, kvp => GetInfo(kvp.Value));
        Dictionary<string, ElementInfoRec> rightInfoDict = rightDict.ToDictionary(kvp => kvp.Value.Path, kvp => GetInfo(kvp.Value));

        Dictionary<string, ComparisonRecord<ElementInfoRec, ElementTypeInfoRec>> comparison = [];

        IEnumerable<string> keys = leftInfoDict.Keys.Union(rightInfoDict.Keys).Distinct();

        // add our matches
        foreach (string edPath in keys)
        {
            IReadOnlyDictionary<string, ElementDefinition.TypeRefComponent> leftTypes;
            IReadOnlyDictionary<string, ElementDefinition.TypeRefComponent> rightTypes;

            if (leftDict.TryGetValue(edPath, out ElementDefinition? leftEd))
            {
                leftTypes = leftEd.cgTypes();
            }
            else
            {
                leftTypes = new Dictionary<string, ElementDefinition.TypeRefComponent>();
            }

            if (rightDict.TryGetValue(edPath, out ElementDefinition? rightEd))
            {
                rightTypes = rightEd.cgTypes();
            }
            else
            {
                rightTypes = new Dictionary<string, ElementDefinition.TypeRefComponent>();
            }

            _ = leftInfoDict.TryGetValue(edPath, out ElementInfoRec? leftInfo);
            _ = rightInfoDict.TryGetValue(edPath, out ElementInfoRec? rightInfo);

            // perform type comparison
            Dictionary<string, ComparisonRecord<ElementTypeInfoRec>> typeComparison = Compare(leftTypes, rightTypes);

            if (TryCompare(edPath, leftInfo, rightInfo, typeComparison, out ComparisonRecord<ElementInfoRec, ElementTypeInfoRec>? c))
            {
                comparison.Add(edPath, c);
            }
        }

        return comparison;
    }

    private Dictionary<string, ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>> Compare(
        IReadOnlyDictionary<string, StructureDefinition> leftInput,
        IReadOnlyDictionary<string, StructureDefinition> rightInput)
    {
        Dictionary<string, StructureInfoRec> left = leftInput.ToDictionary(kvp => kvp.Key, kvp => GetInfo(kvp.Value));
        Dictionary<string, StructureInfoRec> right = rightInput.ToDictionary(kvp => kvp.Key, kvp => GetInfo(kvp.Value));

        Dictionary<string, ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>> comparison = [];

        IEnumerable<string> keys = left.Keys.Union(right.Keys).Distinct();

        // add our matches
        foreach (string sdName in keys)
        {
            _ = left.TryGetValue(sdName, out StructureInfoRec? leftInfo);
            _ = right.TryGetValue(sdName, out StructureInfoRec? rightInfo);

            Dictionary<string, ElementDefinition> leftElements;
            Dictionary<string, ElementDefinition> rightElements;

            if (leftInput.TryGetValue(sdName, out StructureDefinition? leftSd))
            {
                leftElements = leftSd.cgElements().ToDictionary(e => e.Path);
            }
            else
            {
                leftElements = [];
            }

            if (rightInput.TryGetValue(sdName, out StructureDefinition? rightSd))
            {
                rightElements = rightSd.cgElements().ToDictionary(e => e.Path);
            }
            else
            {
                rightElements = [];
            }

            // perform element comparison
            Dictionary<string, ComparisonRecord<ElementInfoRec, ElementTypeInfoRec>> elementComparison = Compare(leftElements, rightElements);

            if (TryCompare(sdName, leftInfo, rightInfo, elementComparison, out ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>? c))
            {
                comparison.Add(sdName, c);
            }
        }

        return comparison;
    }

    private ConceptInfoRec GetInfo(FhirConcept c)
    {
        return new()
        {
            System = c.System,
            Code = c.Code,
            Description = c.Display,
        };
    }

    private ValueSetInfoRec GetInfo(ValueSet vs)
    {
        return new()
        {
            Url = vs.Url,
            Name = vs.Name,
            Title = vs.Title,
            Description = vs.Description,
            ConceptCount = 0,       // will be filled in later
        };
    }

    private ElementTypeInfoRec GetInfo(ElementDefinition.TypeRefComponent tr)
    {
        return new()
        {
            Name = tr.cgName(),
            Profiles = tr.Profile?.ToList() ?? [],
            TargetProfiles = tr.TargetProfile?.ToList() ?? [],
        };
    }

    private ElementInfoRec GetInfo(ElementDefinition ed)
    {
        return new()
        {
            Name = ed.cgName(),
            Path = ed.Path,
            Short = ed.Short,
            Definition = ed.Definition,
            MinCardinality = ed.cgCardinalityMin(),
            MaxCardinality = ed.cgCardinalityMax(),
            MaxCardinalityString = ed.Max,
            ValueSetBindingStrength = ed.Binding?.Strength,
            BindingValueSet = ed.Binding?.ValueSet ?? string.Empty,
        };
    }

    private StructureInfoRec GetInfo(StructureDefinition sd)
    {
        return new StructureInfoRec()
        {
            Name = sd.Name,
            Title = sd.Title,
            Description = sd.Description,
            Purpose = sd.Purpose,
            SnapshotCount = sd.Snapshot?.Element.Count ?? 0,
            DifferentialCount = sd.Differential?.Element.Count ?? 0,
        };
    }

    private class OllamaQuery
    {
        /// <summary>Required: the model name.</summary>
        [JsonPropertyName("model")]
        public required string Model { get; set; }

        /// <summary>The prompt to generate a response for.</summary>
        [JsonPropertyName("prompt")]
        public required string Prompt { get; set; }

        /// <summary>Optional list of base64-encoded images (for multimodal models such as llava)</summary>
        [JsonPropertyName("images")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string[]? Images { get; set; } = null;

        /// <summary>The format to return a response in. Currently the only accepted value is json</summary>
        [JsonPropertyName("format")]
        public string Format { get; } = "json";

        /// <summary>Additional model parameters listed in the documentation for the Model file such as temperature</summary>
        [JsonPropertyName("options")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Options { get; set; } = null;

        /// <summary>System message to (overrides what is defined in the Model file).</summary>
        [JsonPropertyName("system")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? System { get; set; } = null;

        /// <summary>The prompt template to use (overrides what is defined in the Model file)</summary>
        [JsonPropertyName("template")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Template { get; set; } = null;

        /// <summary>The context parameter returned from a previous request to /generate, this can be used to keep a short conversational memory</summary>
        [JsonPropertyName("context")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int[]? Context { get; set; } = null;

        /// <summary>If false the response will be returned as a single response object, rather than a stream of objects.</summary>
        [JsonPropertyName("stream")]
        public required bool Stream { get; set; }

        /// <summary>If true no formatting will be applied to the prompt. You may choose to use the raw parameter if you are specifying a full templated prompt in your request to the API</summary>
        [JsonPropertyName("raw")]
        public bool Raw { get; set; } = false;

        /// <summary>Controls how long the model will stay loaded into memory following the request (default: 5m)</summary>
        [JsonPropertyName("keep_alive")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? KeepAliveDuration { get; set; } = null;
    }


    public class OllamaResponse
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.MinValue;

        [JsonPropertyName("response")]
        public string Response { get; set; } = string.Empty;

        [JsonPropertyName("done")]
        public bool Done { get; set; } = false;

        [JsonPropertyName("context")]
        public int[] Context { get; set; } = [];

        [JsonPropertyName("total_duration")]
        public long TotalDuration { get; set; } = 0;

        [JsonPropertyName("load_duration")]
        public int LoadDuration { get; set; } = 0;

        [JsonPropertyName("prompt_eval_count")]
        public int PromptEvalCount { get; set; } = 0;

        [JsonPropertyName("prompt_eval_duration")]
        public int PromptEvalDuration { get; set; } = 0;

        [JsonPropertyName("eval_count")]
        public int EvalCount { get; set; } = 0;

        [JsonPropertyName("eval_duration")]
        public long EvalDuration { get; set; } = 0;
    }


    private bool TryAskOllama(
        StructureInfoRec known,
        IEnumerable<StructureInfoRec> possibleMatches,
        [NotNullWhen(true)] out StructureInfoRec? guess,
        [NotNullWhen(true)] out int? confidence)
    {
        if (_httpClient == null)
        {
            guess = null;
            confidence = 0;
            return false;
        }

        try
        {
            string system = """
                You are a modeling expert comparing definitions between model structures.
                When presented with a model definition and a set of possible matches, you are asked to select the most likely match.
                You are to respond with ONLY the json representation of the model that you believe is the best match.
                The model definition is a structure with the following properties: Name, Title, Description, and Purpose.
                """;

            //string prompt =
            //    $"Given the following definition:\n{System.Web.HttpUtility.JavaScriptStringEncode(JsonSerializer.Serialize(known))}\n" +
            //    $"Please select the most likely match from the following definitions:\n{System.Web.HttpUtility.JavaScriptStringEncode(JsonSerializer.Serialize(possibleMatches))}." +
            //    $" Respond in JSON with only the definition in the same format.";

            string prompt = $$$"""
                Given the following definition:
                {{{JsonSerializer.Serialize(known)}}}
                Please select the most likely match from the following definitions:
                {{{JsonSerializer.Serialize(possibleMatches)}}}
                """;

                //$"Given the following definition:\n{JsonSerializer.Serialize(known)}\n" +
                //$"Please select the most likely match from the following definitions:\n{JsonSerializer.Serialize(possibleMatches)}." +
                //$" Respond in JSON with only the definition in the same format.";

            // build our prompt
            OllamaQuery query = new()
            {
                Model = _config.OllamaModel,
                System = system,
                Prompt = prompt,
                Stream = false,
            };

            Console.WriteLine("--------------");
            Console.WriteLine($"query:\n{JsonSerializer.Serialize(query)}");

            HttpRequestMessage request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                Content = new StringContent(JsonSerializer.Serialize(query), Encoding.UTF8, "application/json"),
                RequestUri = _ollamaUri,
                Headers =
                {
                    Accept =
                    {
                        new MediaTypeWithQualityHeaderValue("application/json"),
                    },
                },
            };

            HttpResponseMessage response = _httpClient.SendAsync(request).Result;
            System.Net.HttpStatusCode statusCode = response.StatusCode;

            if (statusCode != System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"Request to {request.RequestUri} failed! Returned: {response.StatusCode}");
                guess = null;
                confidence = 0;
                return false;
            }

            string json = response.Content.ReadAsStringAsync().Result;
            if (string.IsNullOrEmpty(json))
            {
                Console.WriteLine($"Request to {request.RequestUri} returned empty body!");
                guess = null;
                confidence = 0;
                return false;
            }

            Console.WriteLine($"response:\n{json}");
            Console.WriteLine("--------------");


            OllamaResponse? olResponse = JsonSerializer.Deserialize<OllamaResponse>(json);

            if (olResponse == null )
            {
                Console.WriteLine($"Failed to deserialize response: {json}");
                guess = null;
                confidence = 0;
                return false;
            }

            if (string.IsNullOrEmpty(olResponse.Response))
            {
                Console.WriteLine($"Ollama response is empty: {json}");
                guess = null;
                confidence = 0;
                return false;
            }

            guess = JsonSerializer.Deserialize<StructureInfoRec>(olResponse.Response);
            if (guess == null)
            {
                Console.WriteLine($"Failed to deserialize response property: {olResponse.Response}");
                guess = null;
                confidence = 0;
                return false;
            }

            confidence = -1;
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"TryAskOllama <<< caught: {ex.Message}{(string.IsNullOrEmpty(ex.InnerException?.Message) ? string.Empty : ex.InnerException.Message)}");
        }

        guess = null;
        confidence = 0;
        return false;
    }

    private string SanitizeVersion(string version)
    {
        return version.Replace('.', '_').Replace('-', '_');
    }
}
