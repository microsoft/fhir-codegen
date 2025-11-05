// <copyright file="CSharpFirelyCommon.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.ComponentModel;
using System.Text;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using Microsoft.Health.Fhir.CodeGenCommon.Utils;

#if NETSTANDARD2_0
using Microsoft.Health.Fhir.CodeGenCommon.Polyfill;
#endif

namespace Microsoft.Health.Fhir.CodeGen.Language.Firely;

public static class CSharpFirelyCommon
{

    /// <summary>Dictionary mapping FHIR primitive types to language equivalents (see Template-Model.tt#1252).</summary>
    public static readonly Dictionary<string, string> PrimitiveTypeMap = new()
    {
        { "base64Binary", "byte[]" },
        { "boolean", "bool?" },
        { "canonical", "string" },
        { "code", "string" },
        { "date", "string" },
        { "dateTime", "string" },
        { "decimal", "decimal?" },
        { "id", "string" },
        { "instant", "DateTimeOffset?" },
        { "integer", "int?" },
        { "integer64", "long?" },
        { "oid", "string" },
        { "positiveInt", "int?" },
        { "string", "string" },
        { "time", "string" },
        { "unsignedInt", "int?" },
        { "uri", "string" },
        { "url", "string" },
        { "uuid", "string" },
        { "xhtml", "string" },
        { "markdown", "string" }
    };

    /// <summary>Types that have non-standard names or formatting (see Template-Model.tt#1252).</summary>
    public static readonly Dictionary<string, string> TypeNameMappings = new()
    {
        { "boolean", "FhirBoolean" },
        { "dateTime", "FhirDateTime" },
        { "decimal", "FhirDecimal" },
        { "Reference", "ResourceReference" },
        { "string", "FhirString" },
        { "uri", "FhirUri" },
        { "url", "FhirUrl" },
        { "xhtml", "XHtml" },
    };

    /// <summary>Context types that need to be remapped for use.</summary>
    public static readonly Dictionary<string, string> ContextTypeMappings = new()
    {
        { "Resource", "DomainResource" },
    };

    /// <summary>
    /// Determines the subset of code to generate.
    /// </summary>
    [Flags]
    public enum GenSubset
    {
        // Subset of datatypes and resources used in R3 and later
        [Description("Subset of datatypes and resources used in R3 and later.")]
        Base = 1,

        // Subset of conformance resources used by the SDK
        [Description("Subset of conformance resources used by the SDK.")]
        Conformance = 2,

        // Subset of model classes that are not part of Base or Conformance
        [Description("Subset of model classes that are not part of Base or Conformance.")]
        Satellite = 4,
    }

    /// <summary>Writes an indented comment.</summary>
    /// <param name="writer">    The writer to write the comment to.</param>
    /// <param name="value">     The value.</param>
    /// <param name="isSummary"> (Optional) True if is summary, false if not.</param>
    /// <param name="singleLine">(Optional) True if this is a short comment using a single line
    ///  comment prefix. Implies isSummary = false.</param>
    /// <param name="isRemarks"> (Optional) True if is remarks, false if not.</param>
    public static void WriteIndentedComment(
        this ExportStreamWriter writer,
        string value,
        bool isSummary = true,
        bool singleLine = false,
        bool isRemarks = false)
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        if (singleLine)
        {
            isSummary = false;
        }

        if (isSummary)
        {
            writer.WriteLineIndented("/// <summary>");
        }

        if (isRemarks)
        {
            writer.WriteLineIndented("/// <remarks>");
        }

        string comment = value
            .Replace('\r', '\n')
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace("\n\n", "\n", StringComparison.Ordinal)
            .Replace("&", "&amp;", StringComparison.Ordinal)
            .Replace("<", "&lt;", StringComparison.Ordinal)
            .Replace(">", "&gt;", StringComparison.Ordinal);

        string[] lines = comment.Split('\n');
        foreach (string line in lines)
        {
            writer.WriteIndented(singleLine ? "// " : "/// ");
            writer.WriteLine(line);
        }

        if (isSummary)
        {
            writer.WriteLineIndented("/// </summary>");
        }

        if (isRemarks)
        {
            writer.WriteLineIndented("/// </remarks>");
        }
    }

    /// <summary>Opens the scope.</summary>
    /// <param name="writer">The writer to write the comment to.</param>
    public static void OpenScope(ExportStreamWriter writer)
    {
        writer.WriteLineIndented("{");
        writer.IncreaseIndent();
    }

    /// <summary>Closes the scope.</summary>
    /// <param name="writer">          The writer to write the comment to.</param>
    /// <param name="includeSemicolon">(Optional) True to include, false to exclude the semicolon.</param>
    /// <param name="suppressNewline"> (Optional) True to suppress, false to allow the newline.</param>
    public static void CloseScope(ExportStreamWriter writer, bool includeSemicolon = false, bool suppressNewline = false)
    {
        writer.DecreaseIndent();

        if (includeSemicolon)
        {
            writer.WriteLineIndented("};");
        }
        else
        {
            writer.WriteLineIndented("}");
        }

        if (!suppressNewline)
        {
            writer.WriteLine(string.Empty);
        }
    }

    /// <summary>Convert enum value - see Template-Model.tt#2061.</summary>
    /// <param name="name">The name.</param>
    /// <returns>The enum converted value.</returns>
    public static string ConvertEnumValue(string name)
    {
        // remove a leading underscore
        if (name.StartsWith('_'))
        {
            name = name.Substring(1);
        }

        // expand common literals
        switch (name)
        {
            case "=":
                return "Equal";
            case "!=":
                return "NotEqual";
            case "<":
                return "LessThan";
            case "<=":
                return "LessOrEqual";
            case ">=":
                return "GreaterOrEqual";
            case ">":
                return "GreaterThan";
        }

        string[] bits = name.Split([' ', '-']);
        string result = string.Empty;
        foreach (string bit in bits)
        {
            result += bit.Substring(0, 1).ToUpperInvariant();
            result += bit.Substring(1);
        }

        result = result
            .Replace('.', '_')
            .Replace(')', '_')
            .Replace('(', '_')
            .Replace('/', '_')
            .Replace('+', '_');

        if (char.IsDigit(result[0]))
        {
            result = "N" + result;
        }

        return result;
    }

    /// <summary>Gets an order.</summary>
    /// <param name="element">The element.</param>
    /// <returns>The order.</returns>
    public static int GetOrder(ElementDefinition element)
    {
        //return (element.cgFieldOrder() * 10) + 10;
        return (element.cgComponentFieldOrder() * 10) + 10;
    }

    public static int GetOrder(int relativeOrder)
    {
        return (relativeOrder * 10) + 10;
    }

    public static string BuildOpenAllowedTypesAttribute() => "[AllowedTypes(OpenChoice = true)]";

    public static string BuildAllowedTypesAttribute(IEnumerable<TypeReference> types, FhirReleases.FhirSequenceCodes? since)
    {
        StringBuilder sb = new();
        sb.Append("[AllowedTypes(");

        string typesList = string.Join(",",
            types.Select(t => $"typeof({t.PropertyTypeString})"));

        sb.Append(typesList);
        if (since is not null)
            sb.Append($", Since = FhirRelease.{since}");
        sb.Append(")]");
        return sb.ToString();
    }
}


public static class StringHelpers
{
    public static string EnsurePeriod(this string s) => s.EndsWith('.') ? s : s + ".";
}
