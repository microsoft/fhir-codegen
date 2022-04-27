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

    /// <summary>An operation outcome issue severity.</summary>
    public static class TsOutcomeIssueSeverity
    {
        /// <summary>(Immutable) The root TS enum.</summary>
        private const string _root = "IssueSeverityValueSetEnum.";

        /// <summary>
        /// (Immutable)
        /// The issue is sufficiently important to cause the action to fail.
        /// </summary>
        public const string Error = _root + "Error";

        /// <summary>
        /// (Immutable)
        /// The issue caused the action to fail and no further checking could be performed.
        /// </summary>
        public const string Fatal = _root + "Fatal";

        /// <summary>
        /// (Immutable)
        /// The issue has no relation to the degree of success of the action.
        /// </summary>
        public const string Information = _root + "Information";

        /// <summary>
        /// (Immutable)
        /// The issue is not important enough to cause the action to fail but may cause it to be
        /// performed suboptimally or in a way that is not as desired.
        /// </summary>
        public const string Warning = _root + "Warning";
    }

    /// <summary>An operation outcome issue type.</summary>
    public static class TsOutcomeIssueType
    {
        /// <summary>(Immutable) The root TS enum.</summary>
        private const string _root = "IssueTypeValueSetEnum.";

        /// <summary>
        /// (Immutable)
        /// The content/operation failed to pass some business rule and so could not proceed.
        /// </summary>
        public const string BusinessRuleViolation = _root + "BusinessRuleViolation";

        /// <summary>
        /// (Immutable)
        /// The code or system could not be understood, or it was not valid in the context of a
        /// particular ValueSet.code.
        /// </summary>
        public const string InvalidCode = _root + "InvalidCode";

        /// <summary>
        /// (Immutable)
        /// Content could not be accepted because of an edit conflict (i.e. version aware updates). (In a
        /// pure RESTful environment, this would be an HTTP 409 error, but this code may be used where
        /// the conflict is discovered further into the application architecture.).
        /// </summary>
        public const string EditVersionConflict = _root + "EditVersionConflict";

        /// <summary>
        /// (Immutable)
        /// The reference pointed to content (usually a resource) that has been deleted.
        /// </summary>
        public const string Deleted = _root + "Deleted";

        /// <summary>
        /// (Immutable)
        /// An attempt was made to create a duplicate record.
        /// </summary>
        public const string Duplicate = _root + "Duplicate";

        /// <summary>
        /// (Immutable)
        /// An unexpected internal error has occurred.
        /// </summary>
        public const string Exception = _root + "Exception";

        /// <summary>
        /// (Immutable)
        /// User session expired; a login may be required.
        /// </summary>
        public const string SessionExpired = _root + "SessionExpired";

        /// <summary>
        /// (Immutable)
        /// An extension was found that was not acceptable, could not be resolved, or a modifierExtension
        /// was not recognized.
        /// </summary>
        public const string UnacceptableExtension = _root + "UnacceptableExtension";

        /// <summary>
        /// (Immutable)
        /// The user does not have the rights to perform this action.
        /// </summary>
        public const string Forbidden = _root + "Forbidden";

        /// <summary>
        /// (Immutable)
        /// Not all data sources typically accessed could be reached or responded in time, so the
        /// returned information might not be complete (applies to search interactions and some
        /// operations).
        /// </summary>
        public const string IncompleteResults = _root + "IncompleteResults";

        /// <summary>
        /// (Immutable)
        /// A message unrelated to the processing success of the completed operation (examples of the
        /// latter include things like reminders of password expiry, system maintenance times, etc.).
        /// </summary>
        public const string InformationalNote = _root + "InformationalNote";

        /// <summary>
        /// (Immutable)
        /// Content invalid against the specification or a profile.
        /// </summary>
        public const string InvalidContent = _root + "InvalidContent";

        /// <summary>
        /// (Immutable)
        /// A content validation rule failed - e.g. a schematron rule.
        /// </summary>
        public const string ValidationRuleFailed = _root + "ValidationRuleFailed";

        /// <summary>
        /// (Immutable)
        /// A resource/record locking failure (usually in an underlying database).
        /// </summary>
        public const string LockError = _root + "LockError";

        /// <summary>
        /// (Immutable)
        /// The client needs to initiate an authentication process.
        /// </summary>
        public const string LoginRequired = _root + "LoginRequired";

        /// <summary>
        /// (Immutable)
        /// Multiple matching records were found when the operation required only one match.
        /// </summary>
        public const string MultipleMatches = _root + "MultipleMatches";

        /// <summary>
        /// (Immutable)
        /// The persistent store is unavailable; e.g. the database is down for maintenance or similar
        /// action, and the interaction or operation cannot be processed.
        /// </summary>
        public const string NoStoreAvailable = _root + "NoStoreAvailable";

        /// <summary>
        /// (Immutable)
        /// The reference provided was not found. In a pure RESTful environment, this would be an HTTP
        /// 404 error, but this code may be used where the content is not found further into the
        /// application architecture.
        /// </summary>
        public const string NotFound = _root + "NotFound";

        /// <summary>
        /// (Immutable)
        /// The interaction, operation, resource or profile is not supported.
        /// </summary>
        public const string ContentNotSupported = _root + "ContentNotSupported";

        /// <summary>
        /// (Immutable)
        /// Processing issues. These are expected to be final e.g. there is no point resubmitting the
        /// same content unchanged.
        /// </summary>
        public const string ProcessingFailure = _root + "ProcessingFailure";

        /// <summary>
        /// (Immutable)
        /// A required element is missing.
        /// </summary>
        public const string RequiredElementMissing = _root + "RequiredElementMissing";

        /// <summary>
        /// (Immutable)
        /// An authentication/authorization/permissions issue of some kind.
        /// </summary>
        public const string SecurityProblem = _root + "SecurityProblem";

        /// <summary>
        /// (Immutable)
        /// A structural issue in the content such as wrong namespace, unable to parse the content
        /// completely, invalid syntax, etc.
        /// </summary>
        public const string StructuralIssue = _root + "StructuralIssue";

        /// <summary>
        /// (Immutable)
        /// Some information was not or might not have been returned due to business rules, consent or
        /// privacy rules, or access permission constraints.  This information may be accessible through
        /// alternate processes.
        /// </summary>
        public const string InformationSuppressed = _root + "InformationSuppressed";

        /// <summary>
        /// (Immutable)
        /// The system is not prepared to handle this request due to load management.
        /// </summary>
        public const string Throttled = _root + "Throttled";

        /// <summary>
        /// (Immutable)
        /// An internal timeout has occurred.
        /// </summary>
        public const string Timeout = _root + "Timeout";

        /// <summary>
        /// (Immutable)
        /// The operation was stopped to protect server resources; e.g. a request for a value set
        /// expansion on all of SNOMED CT.
        /// </summary>
        public const string OperationTooCostly = _root + "OperationTooCostly";

        /// <summary>
        /// (Immutable)
        /// Provided content is too long (typically, this is a denial of service protection type of
        /// error).
        /// </summary>
        public const string ContentTooLong = _root + "ContentTooLong";

        /// <summary>
        /// (Immutable)
        /// Transient processing issues. The system receiving the message may be able to resubmit the
        /// same content once an underlying issue is resolved.
        /// </summary>
        public const string TransientIssue = _root + "TransientIssue";

        /// <summary>
        /// (Immutable)
        /// The user or system was not able to be authenticated (either there is no process, or the
        /// proferred token is unacceptable).
        /// </summary>
        public const string UnknownUser = _root + "UnknownUser";

        /// <summary>
        /// (Immutable)
        /// An element or header value is invalid.
        /// </summary>
        public const string ElementValueInvalid = _root + "ElementValueInvalid";
    }
}
