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
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGen.Utils;
using Microsoft.Health.Fhir.CodeGenCommon.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Utils;

namespace Microsoft.Health.Fhir.CodeGen.CompareTool;

internal static class ComparisonUtils
{
    internal static string ForName(this string value) => FhirSanitizationUtils.SanitizeForProperty(value);
    internal static string ForVersion(this string value) => value.Replace('.', '_').Replace('-', '_');
    internal static string ForMdTable(this string value) => string.IsNullOrEmpty(value) ? string.Empty : value.Replace("|", "\\|").Replace("\n", "<br/>").Replace("\r", "<br/>");
}

public record class SerializationMapInfo
{
    public required string Source { get; init; }
    public required string Target { get; init; }
    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship Relationship { get; init; }
    public required string Message { get; init; }
}

public interface IComparisonRecord
{
    //string RecordTypeDiscriminator { get; }
    FhirArtifactClassEnum ComparisonArtifactType { get; init; }
    string Key { get; init; }
    bool KeyInLeft { get; init; }
    bool KeyInRight { get; init; }
    string CompositeName { get; init; }
    bool NamedMatch { get; init; }
    Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? Relationship { get; init; }
    string Message { get; init; }
    Dictionary<string, SerializationMapInfo>? TypeSerializationInfo { get; init; }

    string GetStatusString();
    Dictionary<string, int> GetStatusCounts(bool inLeft = true, bool inRight = true);
    IEnumerable<string[]> GetChildComparisonRows(bool inLeft, bool inRight);
    string[] GetComparisonRow();
}

public interface IComparisonRecord<T> : IComparisonRecord
{
    List<T> Left { get; init; }
    List<T> Right { get; init; }
}

public interface IComparisonRecord<T, U> : IComparisonRecord<T>
{
    Dictionary<string, ComparisonRecord<U>> Children { get; init; }
}

public interface IComparisonRecord<T, U, V> : IComparisonRecord<T>
{
    Dictionary<string, ComparisonRecord<U, V>> Children { get; init; }
}

public class ComparisonRecord<T> : IComparisonRecord<T>
{
    //public required string RecordTypeDiscriminator { get; init; }
    public required FhirArtifactClassEnum ComparisonArtifactType { get; init; }
    public required string Key { get; init; }
    public required string CompositeName { get; init; }
    public required List<T> Left { get; init; }
    public required bool KeyInLeft { get; init; }
    public required List<T> Right { get; init; }
    public required bool KeyInRight { get; init; }
    public required bool NamedMatch { get; init; }
    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? Relationship { get; init; }
    public required string Message { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, SerializationMapInfo>? TypeSerializationInfo { get; init; } = null;


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


    public Dictionary<string, int> GetStatusCounts(bool inLeft = true, bool inRight = true) => [];
    public IEnumerable<string[]> GetChildComparisonRows(bool inLeft, bool inRight) => Enumerable.Empty<string[]>();

    public string[] GetComparisonRow() => [
        Key,
        Left.Count.ToString(),
        Right.Count.ToString(),
        GetStatusString(),
        ComparisonUtils.ForMdTable(Message),
    ];

    //public T? AiPrediction { get; init; }
    //public int AiConfidence { get; init; } = 0;
}

public class ComparisonRecord<T, U> : ComparisonRecord<T>, IComparisonRecord<T, U>
{
    public required Dictionary<string, ComparisonRecord<U>> Children { get; init; }

    public new Dictionary<string, int> GetStatusCounts(bool inLeft = true, bool inRight = true)
    {
        Dictionary<string, int> counts = [];

        bool checkLeft = inLeft && !inRight;
        bool checkRight = inRight && !inLeft;

        // build summary data
        foreach (ComparisonRecord<U> c in Children.Values)
        {
            if (checkLeft && !c.KeyInLeft)
            {
                continue;
            }

            if (checkRight && !c.KeyInRight)
            {
                continue;
            }

            string status = c.GetStatusString();
            if (!counts.TryGetValue(status, out int count))
            {
                count = 0;
            }

            counts[status] = count + 1;
        }

        return counts;
    }

    public new IEnumerable<string[]> GetChildComparisonRows(bool inLeft = true, bool inRight = true)
    {
        bool checkLeft = inLeft && !inRight;
        bool checkRight = inRight && !inLeft;

        foreach ((string key, ComparisonRecord<U> c) in Children.OrderBy(kvp => kvp.Key))
        {
            if (checkLeft && !c.KeyInLeft)
            {
                continue;
            }

            if (checkRight && !c.KeyInRight)
            {
                continue;
            }

            yield return c.GetComparisonRow();
        }
    }
}

public class ComparisonRecord<T, U, V> : ComparisonRecord<T>, IComparisonRecord<T, U, V>
{
    public required Dictionary<string, ComparisonRecord<U, V>> Children { get; init; }

    public new Dictionary<string, int> GetStatusCounts(bool inLeft = true, bool inRight = true)
    {
        Dictionary<string, int> counts = [];

        bool checkLeft = inLeft && !inRight;
        bool checkRight = inRight && !inLeft;

        // build summary data
        foreach (ComparisonRecord<U, V> c in Children.Values)
        {
            if (checkLeft && !c.KeyInLeft)
            {
                continue;
            }

            if (checkRight && !c.KeyInRight)
            {
                continue;
            }

            string status = c.GetStatusString();
            if (!counts.TryGetValue(status, out int count))
            {
                count = 0;
            }

            counts[status] = count + 1;
        }

        return counts;
    }

    public new IEnumerable<string[]> GetChildComparisonRows(bool inLeft = true, bool inRight = true)
    {
        bool checkLeft = inLeft && !inRight;
        bool checkRight = inRight && !inLeft;

        foreach ((string key, ComparisonRecord<U> c) in Children.OrderBy(kvp => kvp.Key))
        {
            if (checkLeft && !c.KeyInLeft)
            {
                continue;
            }

            if (checkRight && !c.KeyInRight)
            {
                continue;
            }

            yield return c.GetComparisonRow();
        }
    }
}

public record class ConceptInfoRec
{
    public required string System { get; init; }
    public required string Code { get; init; }
    public required string Description { get; init; }
}

public record class ValueSetInfoRec
{
    public required string Url { get; init; }
    public required string Name { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required int ConceptCount { get; init; }
}

public record class ElementTypeInfoRec
{
    public required string Name { get; init; }
    public required List<string> Profiles { get; init; }
    public required List<string> TargetProfiles { get; init; }
}

public record class ElementInfoRec
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
}

public record class StructureInfoRec
{
    public required string Name { get; init; }
    public required string Url { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string Purpose { get; init; }
    public required int SnapshotCount { get; init; }
    public required int DifferentialCount { get; init; }
}

public record class PackageComparison
{
    public required string LeftPackageId { get; init; }
    public required string LeftPackageVersion { get; init; }
    public required string RightPackageId { get; init; }
    public required string RightPackageVersion { get; init; }

    public required Dictionary<string, ComparisonRecord<ValueSetInfoRec, ConceptInfoRec>> ValueSets { get; init; }
    public required Dictionary<string, ComparisonRecord<StructureInfoRec>> PrimitiveTypes { get; init; }
    public required Dictionary<string, ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>> ComplexTypes { get; init; }
    public required Dictionary<string, ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>> Resources { get; init; }
    //public required Dictionary<string, ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>> LogicalModels { get; init; }
}
