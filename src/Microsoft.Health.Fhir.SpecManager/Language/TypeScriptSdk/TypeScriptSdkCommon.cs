// <copyright file="TypeScriptSdkCommon.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.SpecManager.Language.TypeScriptSdk;

/// <summary>A type script sdk common.</summary>
internal static class TypeScriptSdkCommon
{
    /// <summary>
    /// (Immutable) Dictionary mapping FHIR primitive types to language equivalents.
    /// </summary>
    internal static readonly Dictionary<string, string> PrimitiveTypeMap = new()
    {
        { "base", "Object" },
        { "base64Binary", "string" },
        { "bool", "boolean" },
        { "boolean", "boolean" },
        { "canonical", "string" },
        { "code", "string" },
        { "date", "string" },
        { "dateTime", "string" },           // Cannot use "DateTime" because of Partial Dates... may want to consider defining a new type, but not today
        { "decimal", "number" },
        { "id", "string" },
        { "instant", "string" },
        { "int", "number" },
        { "integer", "number" },
        { "integer64", "string" },
        { "markdown", "string" },
        { "number", "number" },
        { "oid", "string" },
        { "positiveInt", "number" },
        { "string", "string" },
        { "time", "string" },
        { "unsignedInt", "number" },
        { "uri", "string" },
        { "url", "string" },
        { "uuid", "string" },
        { "xhtml", "string" },
    };

    /// <summary>
    /// (Immutable) Dictionary mapping FHIR complex types and resource names to language-specific
    /// substitutions.
    /// </summary>
    internal static readonly Dictionary<string, string> ComplexTypeSubstitutions = new()
    {
        { "Resource", "FhirResource" },
        { "Element", "FhirElement" },
    };

    /// <summary>The systems named by display.</summary>
    internal static readonly HashSet<string> SystemsNamedByDisplay = new()
    {
        /// <summary>Units of Measure have incomprehensible codes after naming substitutions.</summary>
        "http://unitsofmeasure.org",
    };

    /// <summary>The systems named by code.</summary>
    internal static readonly HashSet<string> SystemsNamedByCode = new()
    {
        /// <summary>Operation Outcomes include c-style string formats in display.</summary>
        "http://terminology.hl7.org/CodeSystem/operation-outcome",

        /// <summary>Descriptions have quoted values.</summary>
        "http://terminology.hl7.org/CodeSystem/smart-capabilities",

        /// <summary>Descriptions have quoted values.</summary>
        "http://hl7.org/fhir/v2/0301",

        /// <summary>Display values are too long to be useful.</summary>
        "http://terminology.hl7.org/CodeSystem/v2-0178",

        /// <summary>Display values are too long to be useful.</summary>
        "http://terminology.hl7.org/CodeSystem/v2-0277",

        /// <summary>Display values are too long to be useful.</summary>
        "http://terminology.hl7.org/CodeSystem/v3-VaccineManufacturer",

        /// <summary>Display values are too long to be useful.</summary>
        "http://hl7.org/fhir/v2/0278",

        /// <summary>Display includes operation symbols: $.</summary>
        "http://terminology.hl7.org/CodeSystem/testscript-operation-codes",

        /// <summary>Names are often just symbols.</summary>
        "http://hl7.org/fhir/v2/0290",

        /// <summary>Display includes too many Unicode characters (invalid export names).</summary>
        "http://hl7.org/fhir/v2/0255",

        /// <summary>Display includes too many Unicode characters (invalid export names).</summary>
        "http://hl7.org/fhir/v2/0256",
    };

    /// <summary>(Immutable) Language reserved words.</summary>
    internal static readonly HashSet<string> ReservedWords = new()
    {
        "const",
        "enum",
        "export",
        "interface",
        "Element",
        "string",
        "number",
        "boolean",
        "Object",
    };

    /// <summary>Writes an indented comment.</summary>
    /// <param name="sb">   The writer.</param>
    /// <param name="value">The value.</param>
    internal static void WriteIndentedComment(ExportStringBuilder sb, string value)
    {
        string comment;
        string[] lines;

        sb.WriteLineIndented("/**");

        comment = value
            .Replace('\r', '\n')
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace("\n\n", "\n", StringComparison.Ordinal)
            .Replace("&", "&amp;", StringComparison.Ordinal)
            .Replace("<", "&lt;", StringComparison.Ordinal)
            .Replace(">", "&gt;", StringComparison.Ordinal);

        lines = comment.Split('\n');
        foreach (string line in lines)
        {
            sb.WriteIndented(" * ");
            sb.WriteLine(line);
        }

        sb.WriteLineIndented(" */");
    }
}
