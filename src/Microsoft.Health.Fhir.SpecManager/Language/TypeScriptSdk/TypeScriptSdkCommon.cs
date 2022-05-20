// <copyright file="TypeScriptSdkCommon.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.SpecManager.Language.TypeScriptSdk;

/// <summary>A type script sdk common.</summary>
internal static class TypeScriptSdkCommon
{
    /// <summary>
    /// Literal to append to Value Set Coding Objects.
    /// </summary>
    internal const string CodingObjectSuffix = "Codings";

    /// <summary>
    /// Literal to append to Value Set Coding types.
    /// </summary>
    internal const string CodingTypeSuffix = "CodingType";

    /// <summary>
    /// Literal to append to code objects.
    /// </summary>
    internal const string CodeObjectSuffix = "Codes";

    /// <summary>
    /// Literal to append to code object types.
    /// </summary>
    internal const string CodeTypeSuffix = "CodeType";

    /// <summary>
    /// (Immutable) Dictionary mapping FHIR primitive types to language equivalents.
    /// </summary>
    internal static readonly Dictionary<string, string> PrimitiveTypeMap = new(StringComparer.OrdinalIgnoreCase)
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
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

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

    /// <summary>A HashSet&lt;string&gt; extension method that conditional add.</summary>
    /// <param name="hs"> The hs to act on.</param>
    /// <param name="val">The value.</param>
    internal static void ConditionalAdd(this HashSet<string> hs, string val)
    {
        if (!hs.Contains(val))
        {
            hs.Add(val);
        }
    }

    /// <summary>An operation outcome issue severity.</summary>
    public static class TsOutcomeIssueSeverity
    {
        /// <summary>
        /// (Immutable)
        /// The issue is sufficiently important to cause the action to fail.
        /// </summary>
        public const string Error = "error";

        /// <summary>
        /// (Immutable)
        /// The issue caused the action to fail and no further checking could be performed.
        /// </summary>
        public const string Fatal = "fatal";

        /// <summary>
        /// (Immutable)
        /// The issue has no relation to the degree of success of the action.
        /// </summary>
        public const string Information = "information";

        /// <summary>
        /// (Immutable)
        /// The issue is not important enough to cause the action to fail but may cause it to be
        /// performed suboptimally or in a way that is not as desired.
        /// </summary>
        public const string Warning = "warning";
    }

    /// <summary>An operation outcome issue type.</summary>
    public static class TsOutcomeIssueType
    {
        /// <summary>
        /// Literal for code: BusinessRuleViolation.
        /// </summary>
        public const string BusinessRuleViolation = "business-rule";

        /// <summary>
        /// Literal for code: InvalidCode.
        /// </summary>
        public const string InvalidCode = "code-invalid";

        /// <summary>
        /// Literal for code: EditVersionConflict.
        /// </summary>
        public const string EditVersionConflict = "conflict";

        /// <summary>
        /// Literal for code: Deleted.
        /// </summary>
        public const string Deleted = "deleted";

        /// <summary>
        /// Literal for code: Duplicate.
        /// </summary>
        public const string Duplicate = "duplicate";

        /// <summary>
        /// Literal for code: Exception.
        /// </summary>
        public const string Exception = "exception";

        /// <summary>
        /// Literal for code: SessionExpired.
        /// </summary>
        public const string SessionExpired = "expired";

        /// <summary>
        /// Literal for code: UnacceptableExtension.
        /// </summary>
        public const string UnacceptableExtension = "extension";

        /// <summary>
        /// Literal for code: Forbidden.
        /// </summary>
        public const string Forbidden = "forbidden";

        /// <summary>
        /// Literal for code: IncompleteResults.
        /// </summary>
        public const string IncompleteResults = "incomplete";

        /// <summary>
        /// Literal for code: InformationalNote.
        /// </summary>
        public const string InformationalNote = "informational";

        /// <summary>
        /// Literal for code: InvalidContent.
        /// </summary>
        public const string InvalidContent = "invalid";

        /// <summary>
        /// Literal for code: ValidationRuleFailed.
        /// </summary>
        public const string ValidationRuleFailed = "invariant";

        /// <summary>
        /// Literal for code: LockError.
        /// </summary>
        public const string LockError = "lock-error";

        /// <summary>
        /// Literal for code: LoginRequired.
        /// </summary>
        public const string LoginRequired = "login";

        /// <summary>
        /// Literal for code: MultipleMatches.
        /// </summary>
        public const string MultipleMatches = "multiple-matches";

        /// <summary>
        /// Literal for code: NoStoreAvailable.
        /// </summary>
        public const string NoStoreAvailable = "no-store";

        /// <summary>
        /// Literal for code: NotFound.
        /// </summary>
        public const string NotFound = "not-found";

        /// <summary>
        /// Literal for code: ContentNotSupported.
        /// </summary>
        public const string ContentNotSupported = "not-supported";

        /// <summary>
        /// Literal for code: ProcessingFailure.
        /// </summary>
        public const string ProcessingFailure = "processing";

        /// <summary>
        /// Literal for code: RequiredElementMissing.
        /// </summary>
        public const string RequiredElementMissing = "required";

        /// <summary>
        /// Literal for code: SecurityProblem.
        /// </summary>
        public const string SecurityProblem = "security";

        /// <summary>
        /// Literal for code: StructuralIssue.
        /// </summary>
        public const string StructuralIssue = "structure";

        /// <summary>
        /// Literal for code: InformationSuppressed.
        /// </summary>
        public const string InformationSuppressed = "suppressed";

        /// <summary>
        /// Literal for code: Throttled.
        /// </summary>
        public const string Throttled = "throttled";

        /// <summary>
        /// Literal for code: Timeout.
        /// </summary>
        public const string Timeout = "timeout";

        /// <summary>
        /// Literal for code: OperationTooCostly.
        /// </summary>
        public const string OperationTooCostly = "too-costly";

        /// <summary>
        /// Literal for code: ContentTooLong.
        /// </summary>
        public const string ContentTooLong = "too-long";

        /// <summary>
        /// Literal for code: TransientIssue.
        /// </summary>
        public const string TransientIssue = "transient";

        /// <summary>
        /// Literal for code: UnknownUser.
        /// </summary>
        public const string UnknownUser = "unknown";

        /// <summary>
        /// Literal for code: ElementValueInvalid.
        /// </summary>
        public const string ElementValueInvalid = "value";
    }
}
