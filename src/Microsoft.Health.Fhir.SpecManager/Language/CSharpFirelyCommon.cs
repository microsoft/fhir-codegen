// <copyright file="CSharpFirely.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
using System.Collections.Generic;

namespace Microsoft.Health.Fhir.SpecManager.Language
{
    /// <summary>
    /// Contains the methods and properties shared between the <see cref="CSharpFirely1"/> and <see cref="CSharpFirely2" /> generator classes.
    /// </summary>
    internal sealed class CSharpFirelyCommon
    {
        /// <summary>Dictionary mapping FHIR primitive types to language equivalents (see Template-Model.tt#1252).</summary>
        public static readonly Dictionary<string, string> PrimitiveTypeMap = new Dictionary<string, string>()
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
            { "xhtml", "string" },
        };

        /// <summary>Types that have non-standard names or formatting (see Template-Model.tt#1252).</summary>
        public static readonly Dictionary<string, string> TypeNameMappings = new Dictionary<string, string>()
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

        /// <summary>Primitive types that have a specific validation attribute on their Value property.</summary>
        public static readonly Dictionary<string, string> PrimitiveValidationPatterns = new Dictionary<string, string>
        {
            ["uri"] = "UriPattern",
            ["uuid"] = "UuidPattern",
            ["id"] = "IdPattern",
            ["date"] = "DatePattern",
            ["dateTime"] = "DateTimePattern",
            ["oid"] = "OidPattern",
        };

        /// <summary>Writes an indented comment.</summary>
        /// <param name="writer">The writer to write the comment to.</param>
        /// <param name="value">The value.</param>
        /// <param name="isSummary">(Optional) True if is summary, false if not.</param>
        /// <param name="singleLine">(Optional) True if this is a short comment using a single line comment prefix. Implies isSummary = false.</param>
        public static void WriteIndentedComment(ExportStreamWriter writer, string value, bool isSummary = true, bool singleLine = false)
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

            string comment = value.Replace('\r', '\n').Replace("\r\n", "\n").Replace("\n\n", "\n")
                .Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");

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
        }

    }
}
