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
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CodeGen.CompareTool;

internal static class ComparisonUtils
{
    internal static string SanitizeForTable(string value) => string.IsNullOrEmpty(value) ? string.Empty : value.Replace("|", "\\|").Replace("\n", "<br/>").Replace("\r", "<br/>");
}

public interface IComparisonRecord
{
    string Key { get; init; }
    string CompositeName { get; init; }
    bool NamedMatch { get; init; }
    Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? Relationship { get; init; }
    string Message { get; init; }
    string[] TableColumns { get; init; }


    string GetStatusString();
    void GetTableData(out string[] header, out List<string[]> left, out List<string[]> right);
    Dictionary<string, int> GetStatusCounts();
    IEnumerable<string[]> GetChildrenDetailTableRows();
    string[] GetDetailTableRow();
}

public interface IComparisonRecord<T> : IComparisonRecord where T : IInfoRec
{
    List<T> Left { get; init; }
    List<T> Right { get; init; }
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
    private const string _leftSide = "Source";
    private const string _rightSide = "Destination";

    public required string Key { get; init; }
    public required string CompositeName { get; init; }
    public required List<T> Left { get; init; }
    public required List<T> Right { get; init; }
    public required bool NamedMatch { get; init; }
    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? Relationship { get; init; }
    public required string Message { get; init; }

    public required string[] TableColumns { get; init; }

    public string GetStatusString()
    {
        if (Left.Count == 0)
        {
            return "Added";
        }

        if (Right.Count == 0)
        {
            return "Removed";
        }

        return Relationship?.ToString() ?? "-";
    }

    public void GetTableData(
        out string[] header,
        out List<string[]> left,
        out List<string[]> right)
    {
        int colCount = TableColumns.Length + 1;
        header = ["Side", .. TableColumns];

        if (Left.Count == 0)
        {
            left = [[_leftSide, .. Enumerable.Repeat("-", colCount).ToArray()]];
        }
        else
        {
            left = Left.Select(i => (string[])[_leftSide, .. i.GetTableRow()]).ToList();
        }

        if (Right.Count == 0)
        {
            right = [[_rightSide, .. Enumerable.Repeat("-", colCount).ToArray()]];
        }
        else
        {
            right = Right.Select(i => (string[])[_rightSide, .. i.GetTableRow()]).ToList();
        }
    }

    public Dictionary<string, int> GetStatusCounts() => [];
    public IEnumerable<string[]> GetChildrenDetailTableRows() => Enumerable.Empty<string[]>();

    public string[] GetDetailTableRow() => [
        Key,
        Left.Count.ToString(),
        Right.Count.ToString(),
        GetStatusString(),
        ComparisonUtils.SanitizeForTable(Message),
    ];

    //public T? AiPrediction { get; init; }
    //public int AiConfidence { get; init; } = 0;
}

public class ComparisonRecord<T, U> : ComparisonRecord<T>, IComparisonRecord<T, U> where T : IInfoRec where U : IInfoRec
{
    public required Dictionary<string, ComparisonRecord<U>> Children { get; init; }

    public new Dictionary<string, int> GetStatusCounts()
    {
        Dictionary<string, int> counts = [];

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

        return counts;
    }

    public new IEnumerable<string[]> GetChildrenDetailTableRows()
    {
        foreach ((string key, ComparisonRecord<U> c) in Children.OrderBy(kvp => kvp.Key))
        {
            yield return c.GetDetailTableRow();
        }
    }
}

public class ComparisonRecord<T, U, V> : ComparisonRecord<T>, IComparisonRecord<T, U, V> where T : IInfoRec where U : IInfoRec where V : IInfoRec
{
    public required Dictionary<string, ComparisonRecord<U, V>> Children { get; init; }

    public new Dictionary<string, int> GetStatusCounts()
    {
        Dictionary<string, int> counts = [];

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

        return counts;
    }

    public new IEnumerable<string[]> GetChildrenDetailTableRows()
    {
        foreach ((string key, ComparisonRecord<U> c) in Children.OrderBy(kvp => kvp.Key))
        {
            yield return c.GetDetailTableRow();
        }
    }
}


public interface IInfoRec
{
    static string[] TableColumns => [];
    string[] GetTableRow();
}

public record class ConceptInfoRec : IInfoRec
{
    public static string[] TableColumns => ["System", "Code", "Description"];

    public required string System { get; init; }
    public required string Code { get; init; }
    public required string Description { get; init; }

    public string[] GetTableRow() => [System, Code, ComparisonUtils.SanitizeForTable(Description)];
}

public record class ValueSetInfoRec : IInfoRec
{
    public static string[] TableColumns => ["Url", "Name", "Title", "Description"];

    public required string Url { get; init; }
    public required string Name { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required int ConceptCount { get; init; }

    public string[] GetTableRow() => [Url, Name, ComparisonUtils.SanitizeForTable(Title), ComparisonUtils.SanitizeForTable(Description)];
}

public record class ElementTypeInfoRec : IInfoRec
{
    public static string[] TableColumns => ["Name", "Profiles", "Target Profiles"];

    public required string Name { get; init; }
    public required List<string> Profiles { get; init; }
    public required List<string> TargetProfiles { get; init; }

    public string[] GetTableRow() => [Name, string.Join(", ", Profiles), string.Join(", ", TargetProfiles)];
}

public record class ElementInfoRec : IInfoRec
{
    public static string[] TableColumns => ["Name", "Path", "Short", "Definition", "Card", "Binding"];

    public required string Name { get; init; }
    public required string Path { get; init; }
    public required string Short { get; init; }
    public required string Definition { get; init; }
    public required int MinCardinality { get; init; }
    public required int MaxCardinality { get; init; }
    public required string MaxCardinalityString { get; init; }
    public required Hl7.Fhir.Model.BindingStrength? ValueSetBindingStrength { get; init; }
    public required string BindingValueSet { get; init; }

    public string[] GetTableRow() => [Name, Path, Short, Definition, $"{MinCardinality}..{MaxCardinalityString}", $"{ValueSetBindingStrength} {BindingValueSet}"];
}

public record class StructureInfoRec : IInfoRec
{
    public static string[] TableColumns => ["Name", "Title", "Description", "Snapshot", "Differential"];

    public required string Name { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string Purpose { get; init; }
    public required int SnapshotCount { get; init; }
    public required int DifferentialCount { get; init; }

    public string[] GetTableRow() => [
        Name,
        ComparisonUtils.SanitizeForTable(Title),
        ComparisonUtils.SanitizeForTable(Description),
        SnapshotCount.ToString(),
        DifferentialCount.ToString()
        ];
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
