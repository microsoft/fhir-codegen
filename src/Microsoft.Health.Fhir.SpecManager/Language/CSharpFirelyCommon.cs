// <copyright file="CSharpFirelyCommon.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.SpecManager.Language
{
    /// <summary>
    /// Contains the methods and properties shared between the <see cref="CSharpFirely1"/> and <see cref="CSharpFirely2" /> generator classes.
    /// </summary>
    internal static class CSharpFirelyCommon
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
            { "markdown", "string" }
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
            ["code"] = "CodePattern",
            ["time"] = "TimePattern",
            ["string"] = "StringPattern",
            ["markdown"] = "MarkdownPattern"
        };

        /// <summary>Writes an indented comment.</summary>
        /// <param name="writer">The writer to write the comment to.</param>
        /// <param name="value">The value.</param>
        /// <param name="isSummary">(Optional) True if is summary, false if not.</param>
        /// <param name="singleLine">(Optional) True if this is a short comment using a single line comment prefix. Implies isSummary = false.</param>
        public static void WriteIndentedComment(this ExportStreamWriter writer, string value, bool isSummary = true, bool singleLine = false)
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

#pragma warning disable CA1307 // Specify StringComparison
            string comment = value.Replace('\r', '\n').Replace("\r\n", "\n").Replace("\n\n", "\n")
#pragma warning restore CA1307 // Specify StringComparison
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
            if (name.StartsWith("_", StringComparison.Ordinal))
            {
                name = name.Substring(1);
            }

            if (name == "=")
            {
                return "Equal";
            }

            if (name == "!=")
            {
                return "NotEqual";
            }

            if (name == "<")
            {
                return "LessThan";
            }

            if (name == "<=")
            {
                return "LessOrEqual";
            }

            if (name == ">=")
            {
                return "GreaterOrEqual";
            }

            if (name == ">")
            {
                return "GreaterThan";
            }

            string[] bits = name.Split(new char[] { ' ', '-' });
            string result = null;
            foreach (string bit in bits)
            {
                result += bit.Substring(0, 1).ToUpperInvariant();
                result += bit.Substring(1);
            }

#pragma warning disable CA1307 // Specify StringComparison
            result = result.Replace(".", "_");
            result = result.Replace(")", "_");
            result = result.Replace("(", "_");
            result = result.Replace("/", "_");
            result = result.Replace("+", "_");
#pragma warning restore CA1307 // Specify StringComparison

            if (char.IsDigit(result[0]))
            {
                result = "N" + result;
            }

            return result;
        }

        /// <summary>Gets an order.</summary>
        /// <param name="element">The element.</param>
        /// <returns>The order.</returns>
        public static int GetOrder(FhirElement element)
        {
            return (element.FieldOrder * 10) + 10;
        }
    }
}
