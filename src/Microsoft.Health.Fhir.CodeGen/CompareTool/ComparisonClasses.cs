// <copyright file="ComparisonClasses.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Microsoft.Health.Fhir.CodeGen.CompareTool;


internal static class ComparisonUtils
{
    internal const string _leftSide = "Source";
    internal const string _rightSide = "Destination";

    internal static string SanitizeForTable(string value) => string.IsNullOrEmpty(value) ? string.Empty : value.Replace("|", "\\|").Replace("\n", "<br/>").Replace("\r", "<br/>");
}

public interface IComparisonRecord
{
    string Key { get; init; }
    string GetStatusString();
    string GetMarkdownTable();
    string GetMarkdownSummaryTable();
    string GetMarkdownDetailTable();
    string GetMarkdownDetailTableRow();
}

public interface IComparisonRecord<T> : IComparisonRecord where T : IInfoRec
{
    T? Left { get; init; }
    T? Right { get; init; }
    bool NamedMatch { get; init; }
    Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? Relationship { get; init; }
    string Message { get; init; }
}

public interface IComparisonRecord<T, U> : IComparisonRecord<T> where T : IInfoRec where U : IInfoRec
{
    Dictionary<string, ComparisonRecord<U>> Children { get; init; }
}

public interface IComparisonRecord<T, U, V> : IComparisonRecord<T> where T : IInfoRec where U : IInfoRec where V : IInfoRec
{
    Dictionary<string, ComparisonRecord<U, V>> Children { get; init; }
}

public class ComparisonRecord<T> : IComparisonRecord<T> where T : IInfoRec
{
    public required string Key { get; init; }
    public required T? Left { get; init; }
    public required T? Right { get; init; }
    public required bool NamedMatch { get; init; }
    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? Relationship { get; init; }
    public required string Message { get; init; }

    public string GetStatusString()
    {
        if (Left is null)
        {
            return "Added";
        }

        if (Right is null)
        {
            return "Removed";
        }

        return Relationship?.ToString() ?? "-";
    }

    public string GetMarkdownTable()
    {
        if ((Left is null) && (Right is null))
        {
            return string.Empty;
        }

        if (Left is null)
        {
            return string.Join('\n', Right!.GetMarkdownTableHeader(), Right.GetMarkdownTableValueMissing(ComparisonUtils._leftSide), Right.GetMarkdownTableValue(ComparisonUtils._rightSide));
        }

        if (Right is null)
        {
            return string.Join('\n', Left!.GetMarkdownTableHeader(), Left.GetMarkdownTableValue(ComparisonUtils._leftSide), Left.GetMarkdownTableValueMissing(ComparisonUtils._rightSide));
        }

        return string.Join('\n', Left!.GetMarkdownTableHeader(), Left.GetMarkdownTableValue(ComparisonUtils._leftSide), Right.GetMarkdownTableValue(ComparisonUtils._rightSide));
    }

    public string GetMarkdownSummaryTable() => string.Empty;
    public string GetMarkdownDetailTable() => string.Empty;

    public string GetMarkdownDetailTableRow() =>
        $"{Key} |" +
        $" {(Left == null ? "N" : "Y")} |" +
        $" {(Right == null ? "N" : "Y")} |" +
        $" {GetStatusString()} |" +
        $" {ComparisonUtils.SanitizeForTable(Message)} |";

    //public T? AiPrediction { get; init; }
    //public int AiConfidence { get; init; } = 0;
}

public class ComparisonRecord<T, U> : ComparisonRecord<T>, IComparisonRecord<T, U> where T : IInfoRec where U : IInfoRec
{
    public required Dictionary<string, ComparisonRecord<U>> Children { get; init; }

    public new string GetMarkdownSummaryTable()
    {
        StringBuilder sb = new();

        Dictionary<string, int> counts = new Dictionary<string, int>();

        // build summary data
        foreach (ComparisonRecord<U> c in Children.Values)
        {
            string status = c.GetStatusString();
            if (!counts.TryGetValue(status, out int count))
            {
                count = 0;
            }

            counts[status] = count + 1;
        }

        sb.AppendLine("| Status | Count |");
        sb.AppendLine("| ------ | ----- |");
        foreach ((string status, int count) in counts.OrderBy(kvp => kvp.Key))
        {
            sb.AppendLine($"{status} | {count} |");
        }
        sb.AppendLine();

        return sb.ToString();
    }

    public new string GetMarkdownDetailTable()
    {
        StringBuilder sb = new();

        sb.AppendLine("| Key | Source | Dest | Status | Message |");
        sb.AppendLine("| --- | ------ | ---- | ------ | ------- |");

        foreach ((string key, ComparisonRecord<U> c) in Children.OrderBy(kvp => kvp.Key))
        {
            sb.AppendLine(
                $"{key} |" +
                $" {(c.Left == null ? "N" : "Y")} |" +
                $" {(c.Right == null ? "N" : "Y")} |" +
                $" {c.GetStatusString()} |" +
                $" {ComparisonUtils.SanitizeForTable(c.Message)} |");
        }

        sb.AppendLine();
        return sb.ToString();
    }
}

public class ComparisonRecord<T, U, V> : ComparisonRecord<T>, IComparisonRecord<T, U, V> where T : IInfoRec where U : IInfoRec where V : IInfoRec
{
    public required Dictionary<string, ComparisonRecord<U, V>> Children { get; init; }

    public new string GetMarkdownSummaryTable()
    {
        StringBuilder sb = new();

        Dictionary<string, int> counts = new Dictionary<string, int>();

        // build summary data
        foreach (ComparisonRecord<U, V> c in Children.Values)
        {
            string status = c.GetStatusString();
            if (!counts.TryGetValue(status, out int count))
            {
                count = 0;
            }

            counts[status] = count + 1;
        }

        sb.AppendLine("| Status | Count |");
        sb.AppendLine("| ------ | ----- |");
        foreach ((string status, int count) in counts.OrderBy(kvp => kvp.Key))
        {
            sb.AppendLine($"{status} | {count} |");
        }
        sb.AppendLine();

        return sb.ToString();
    }

    public new string GetMarkdownDetailTable()
    {
        StringBuilder sb = new();

        sb.AppendLine("| Key | Source | Dest | Status | Message |");
        sb.AppendLine("| --- | ------ | ---- | ------ | ------- |");

        foreach ((string key, ComparisonRecord<U, V> c) in Children.OrderBy(kvp => kvp.Key))
        {
            sb.AppendLine(
                $"{key} |" +
                $" {(c.Left == null ? "N" : "Y")} |" +
                $" {(c.Right == null ? "N" : "Y")} |" +
                $" {c.GetStatusString()} |" +
                $" {ComparisonUtils.SanitizeForTable(c.Message)} |");
        }

        sb.AppendLine();
        return sb.ToString();
    }
}


public interface IInfoRec
{
    string GetMarkdownTableHeader();
    string GetMarkdownTableValue(string side);
    string GetMarkdownTableValueMissing(string side);
}

public record class ConceptInfoRec : IInfoRec
{
    public required string System { get; init; }
    public required string Code { get; init; }
    public required string Description { get; init; }

    private const string _tableHeader = "| Side | System | Code | Description |\n| --- | --- | --- | --- |";

    public string GetMarkdownTableHeader() => _tableHeader;
    public string GetMarkdownTableValue(string side) =>
        $"{side} | {System} | {Code} | {ComparisonUtils.SanitizeForTable(Description)} |";
    public string GetMarkdownTableValueMissing(string side) =>
        $"{side} | - | - | - |";
}

public record class ValueSetInfoRec : IInfoRec
{
    public required string Url { get; init; }
    public required string Name { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required int ConceptCount { get; init; }

    private const string _tableHeader = "| Side | Name | Url | Title | Description |\n| --- | --- | --- | --- | --- |";

    public string GetMarkdownTableHeader() => _tableHeader;
    public string GetMarkdownTableValue(string side) =>
        $"{side} | {Name} | {Url} | {ComparisonUtils.SanitizeForTable(Title)} | {ComparisonUtils.SanitizeForTable(Description)} |";
    public string GetMarkdownTableValueMissing(string side) =>
        $"{side} | - | - | - | - |";
}

public record class ElementTypeInfoRec : IInfoRec
{
    public required string Name { get; init; }
    public required List<string> Profiles { get; init; }
    public required List<string> TargetProfiles { get; init; }

    private const string _tableHeader = "| Side | Name | Profiles | Target Profiles |\n| --- | --- | --- | --- |";

    public string GetMarkdownTableHeader() => _tableHeader;
    public string GetMarkdownTableValue(string side) =>
        $"{side} | {Name} | {string.Join(", ", Profiles)} | {string.Join(", ", TargetProfiles)} |";
    public string GetMarkdownTableValueMissing(string side) =>
        $"{side} | - | - | - |";
}

public record class ElementInfoRec : IInfoRec
{
    public required string Name { get; init; }
    public required string Path { get; init; }
    public required string Short { get; init; }
    public required string Definition { get; init; }

    public required int MinCardinality { get; init; }
    public required int MaxCardinality { get; init; }
    public required string MaxCardinalityString { get; init; }

    public required Hl7.Fhir.Model.BindingStrength? ValueSetBindingStrength { get; init; }

    public required string BindingValueSet { get; init; }

    private const string _tableHeader = "| Side | Path | Short | Definition | Card | Binding |\n| --- | --- | --- | --- | --- | --- |";

    public string GetMarkdownTableHeader() => _tableHeader;
    public string GetMarkdownTableValue(string side) =>
        $"{side} |" +
        $" {Path} |" +
        $" {ComparisonUtils.SanitizeForTable(Short)} |" +
        $" {ComparisonUtils.SanitizeForTable(Definition)} |" +
        $" {MinCardinality}..{MaxCardinalityString} |" +
        $" {ValueSetBindingStrength} {BindingValueSet} |";
    public string GetMarkdownTableValueMissing(string side) =>
        $"{side} |" +
        $" - |" +
        $" - |" +
        $" - |" +
        $" - |" +
        $" - |";

}

public record class StructureInfoRec : IInfoRec
{
    public required string Name { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string Purpose { get; init; }

    public required int SnapshotCount { get; init; }

    public required int DifferentialCount { get; init; }

    private const string _tableHeader = "| Side | Name | Title | Description | Snapshot | Differential |\n| --- | --- | --- | --- | --- | --- |";
    public string GetMarkdownTableHeader() => _tableHeader;
    public string GetMarkdownTableValue(string side) =>
        $"{side} |" +
        $" {Name} |" +
        $" {ComparisonUtils.SanitizeForTable(Title)} |" +
        $" {ComparisonUtils.SanitizeForTable(Description)} |" +
        $" {SnapshotCount} |" +
        $" {DifferentialCount} |";
    public string GetMarkdownTableValueMissing(string side) =>
        $"{side} |" +
        $" - |" +
        $" - |" +
        $" - |" +
        $" - |" +
        $" - |";
}

public record class PackageComparison
{
    public required string LeftPackageId { get; init; }
    public required string LeftPackageVersion { get; init; }
    public required string RightPackageId { get; init; }
    public required string RightPackageVersion { get; init; }

    public required Dictionary<string, ComparisonRecord<ValueSetInfoRec, ConceptInfoRec>> ValueSets { get; init; }
    public required Dictionary<string, ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>> PrimitiveTypes { get; init; }
    public required Dictionary<string, ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>> ComplexTypes { get; init; }
    public required Dictionary<string, ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>> Resources { get; init; }
    public required Dictionary<string, ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>> LogicalModels { get; init; }
}
