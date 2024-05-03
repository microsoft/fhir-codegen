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

    internal static string[] GetEmptyValueArray(string prefix, int count)
    {
        if (string.IsNullOrEmpty(prefix))
        {
            return Enumerable.Repeat("-", count).ToArray();
        }

        List<string> values = [ prefix ];
        values.AddRange(Enumerable.Repeat("-", count - 1));
        return values.ToArray();
    }
}

public interface IComparisonRecord
{
    string Key { get; init; }
    string GetStatusString();
    void GetTableData(out string[] header, out string[] left, out string[] right);
    Dictionary<string, int> GetStatusCounts();
    IEnumerable<string[]> GetChildrenDetailTableRows();
    string[] GetDetailTableRow();
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

    public void GetTableData(out string[] header, out string[] left, out string[] right)
    {
        if ((Left is null) && (Right is null))
        {
            header = Array.Empty<string>();
            left = Array.Empty<string>();
            right = Array.Empty<string>();
            return;
        }

        if (Left is null)
        {
            int colCount = Right!.TableColumnCount + 1;
            header = Right!.GetTableHeader("Side");
            left = ComparisonUtils.GetEmptyValueArray("Source", colCount);
            right = Right.GetTableValue("Destination");
            return;
        }

        if (Right is null)
        {
            int colCount = Left!.TableColumnCount + 1;
            header = Left!.GetTableHeader("Side");
            left = Left.GetTableValue("Source");
            right = ComparisonUtils.GetEmptyValueArray("Destination", colCount);
            return;
        }

        header = Left!.GetTableHeader("Side");
        left = Left.GetTableValue("Source");
        right = Right.GetTableValue("Destination");
    }

    public Dictionary<string, int> GetStatusCounts() => [];
    public IEnumerable<string[]> GetChildrenDetailTableRows() => Enumerable.Empty<string[]>();

    public string[] GetDetailTableRow() => [
        Key,
        (Left == null ? "N" : "Y"),
        (Right == null ? "N" : "Y"),
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
    int TableColumnCount { get; }
    string[] GetTableHeader(string prefixColHeader = "");
    string[] GetTableValue(string prefixColValue = "");
}

public record class ConceptInfoRec : IInfoRec
{
    public required string System { get; init; }
    public required string Code { get; init; }
    public required string Description { get; init; }

    public int TableColumnCount => 3;
    public string[] GetTableHeader(string prefixColHeader = "") => string.IsNullOrEmpty(prefixColHeader)
        ? ["System", "Code", "Description"]
        : [prefixColHeader, "System", "Code", "Description"];

    public string[] GetTableValue(string prefixColValue = "") => string.IsNullOrEmpty(prefixColValue)
        ? [System, Code, ComparisonUtils.SanitizeForTable(Description)]
        : [prefixColValue, System, Code, ComparisonUtils.SanitizeForTable(Description)];
}

public record class ValueSetInfoRec : IInfoRec
{
    public required string Url { get; init; }
    public required string Name { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required int ConceptCount { get; init; }

    public int TableColumnCount => 4;
    public string[] GetTableHeader(string prefixColHeader = "") => string.IsNullOrEmpty(prefixColHeader)
        ? ["Url", "Name", "Title", "Description"]
        : [prefixColHeader, "Url", "Name", "Title", "Description"];
    public string[] GetTableValue(string prefixColValue = "") => string.IsNullOrEmpty(prefixColValue)
        ? [Url, Name, ComparisonUtils.SanitizeForTable(Title), ComparisonUtils.SanitizeForTable(Description)]
        : [prefixColValue, Url, Name, ComparisonUtils.SanitizeForTable(Title), ComparisonUtils.SanitizeForTable(Description)];
}

public record class ElementTypeInfoRec : IInfoRec
{
    public required string Name { get; init; }
    public required List<string> Profiles { get; init; }
    public required List<string> TargetProfiles { get; init; }

    public int TableColumnCount => 3;
    public string[] GetTableHeader(string prefixColHeader = "") => string.IsNullOrEmpty(prefixColHeader)
        ? ["Name", "Profiles", "Target Profiles"]
        : [prefixColHeader, "Name", "Profiles", "Target Profiles"];
    public string[] GetTableValue(string prefixColValue = "") => string.IsNullOrEmpty(prefixColValue)
        ? [Name, string.Join(", ", Profiles), string.Join(", ", TargetProfiles)]
        : [prefixColValue, Name, string.Join(", ", Profiles), string.Join(", ", TargetProfiles)];
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

    public int TableColumnCount => 5;
    public string[] GetTableHeader(string prefixColHeader = "") => string.IsNullOrEmpty(prefixColHeader)
        ? ["Name", "Path", "Short", "Definition", "Card", "Binding"]
        : [prefixColHeader, "Name", "Path", "Short", "Definition", "Card", "Binding"];
    public string[] GetTableValue(string prefixColValue = "") => string.IsNullOrEmpty(prefixColValue)
        ? [Name, Path, Short, Definition, $"{MinCardinality}..{MaxCardinalityString}", $"{ValueSetBindingStrength} {BindingValueSet}"]
        : [prefixColValue, Name, Path, Short, Definition, $"{MinCardinality}..{MaxCardinalityString}", $"{ValueSetBindingStrength} {BindingValueSet}"];
}

public record class StructureInfoRec : IInfoRec
{
    public required string Name { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string Purpose { get; init; }

    public required int SnapshotCount { get; init; }

    public required int DifferentialCount { get; init; }

    public int TableColumnCount => 5;
    public string[] GetTableHeader(string prefixColHeader = "") => string.IsNullOrEmpty(prefixColHeader)
        ? ["Name", "Title", "Description", "Snapshot", "Differential"]
        : [prefixColHeader, "Name", "Title", "Description", "Snapshot", "Differential"];
    public string[] GetTableValue(string prefixColValue = "") => string.IsNullOrEmpty(prefixColValue)
        ? [Name, ComparisonUtils.SanitizeForTable(Title), ComparisonUtils.SanitizeForTable(Description), SnapshotCount.ToString(), DifferentialCount.ToString()]
        : [prefixColValue, Name, ComparisonUtils.SanitizeForTable(Title), ComparisonUtils.SanitizeForTable(Description), SnapshotCount.ToString(), DifferentialCount.ToString()];
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
