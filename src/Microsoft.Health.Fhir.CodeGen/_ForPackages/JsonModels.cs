using System;
using System.Collections.Generic;
using System.Text;
using Firely.Fhir.Packages;
using Newtonsoft.Json;

namespace Microsoft.Health.Fhir.CodeGen._ForPackages
{
    public class PackageManifest
    {
        /// <summary>
        /// Initialize a new package manifest
        /// </summary>
        /// <param name="name">Package name </param>
        /// <param name="version">Version of the package</param>
        [JsonConstructor]
        public PackageManifest(string name, string version)
        {
            Name = name;
            Version = version;
        }

        /// <summary>
        /// The globally unique name for the package.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name;

        /// <summary>
        /// Semver-based version for the package
        /// </summary>
        [JsonProperty(PropertyName = "version")]
        public string Version;

        /// <summary>
        /// Description of the package.
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string? Description;

        /// <summary>
        /// Author of the package.
        /// </summary>
        [JsonIgnore]
        public string? Author
        {
            get => (AuthorInformation != null ? AuthorSerializer.Serialize(AuthorInformation) : null);
            set
            {
                if (value is not null)
                    AuthorInformation = AuthorSerializer.Deserialize(value);

            }
        }

        [JsonConverter(typeof(AuthorJsonConverter))]
        [JsonProperty(PropertyName = "author")]
        public AuthorInfo? AuthorInformation;

        /// <summary>
        /// Other packages that the contents of this packages depend on.
        /// </summary>
        /// <remarks>
        /// Keys are package names, values are package versions
        /// </remarks>
        [JsonProperty(PropertyName = "dependencies")]
        public Dictionary<string, string?>? Dependencies;

        /// <summary>
        /// Other packages necessary during development of this package.
        /// </summary>
        [JsonProperty(PropertyName = "devDependencies")]
        public Dictionary<string, string>? DevDependencies;

        /// <summary>
        /// List of keywords to help with discovery.
        /// </summary>
        [JsonProperty(PropertyName = "keywords")]
        public List<string>? Keywords;

        /// <summary>
        /// SPDX-convention license name.
        /// </summary>
        [JsonProperty(PropertyName = "license")]
        public string? License;

        /// <summary>
        /// The url to the project homepage.
        /// </summary>
        [JsonProperty(PropertyName = "homepage")]
        public string? Homepage;

        /// <summary>
        /// Describes the structure of the package.
        /// </summary>
        /// <remarks>Some of the common keys used are defined in <see cref="DirectoryKeys"/>.</remarks>
        [JsonProperty(PropertyName = "directories")]
        public Dictionary<string, string>? Directories;

        /// <summary>
        /// String-based keys used in the <see cref="Directories"/> dictionary.
        /// </summary>
        /// <remarks>
        /// Known directory keys are:
        ///  - lib: contains the package's definitions
        ///  - bin: contains any executables the package provides
        ///  - man: contains any man pages the package provides
        ///  - doc: contains documentation for the package
        ///  - example: contains example resource instances for the package
        ///  - test: contains test resources for the package.
        /// Note that this list does NOT include:
        /// - xml: XML schema definitions for the package (.sch)
        /// - openapi: OpenAPI definitions for the package (.json)
        /// - rdf: RDF definitions for the package (.ttl)
        /// TODO: not documented at
        ///  https://confluence.hl7.org/pages/viewpage.action?pageId=35718629#NPMPackageSpecification-Packagemanifest.
        /// </remarks>
        public class DirectoryKeys
        {
            /// <summary>
            /// Where the bulk of the library is.
            /// </summary>
            public const string DIRECTORY_KEY_LIB = "lib";
            public const string DIRECTORY_KEY_BIN = "bin";
            public const string DIRECTORY_KEY_MAN = "man";
            public const string DIRECTORY_KEY_DOC = "doc";
            public const string DIRECTORY_KEY_EXAMPLE = "example";
            public const string DIRECTORY_KEY_TEST = "test";
        }

        /// <summary>
        /// Title for the package.
        /// </summary>
        [JsonProperty(PropertyName = "title")]
        public string? Title;

        /// <summary>
        /// Versions of the FHIR standard used in artifacts within this package.
        /// </summary>
        /// <remarks>Largely obsolete, and replaced by actual dependencies on the
        /// core packages.</remarks>
        [JsonProperty(PropertyName = "fhirVersions")]
        public List<string>? FhirVersions;

        /// <summary>
        /// Versions of the FHIR standard used in artifacts within this package.
        /// </summary>
        /// <remarks>It seems this is mistakenly generated in the core packages
        /// published by HL7 and should be the same as <see cref="FhirVersions"/> above.</remarks>
        [JsonProperty(PropertyName = "fhir-version-list")]
        public List<string>? FhirVersionList;

        /// <summary>Gets the FHIR versions from FhirVersions, FhirVersionList, or dependencies.</summary>
        [JsonIgnore]
        public List<string>? AnyFhirVersions =>
            FhirVersions?.Count > 0
            ? FhirVersions
            : FhirVersionList?.Count > 0
                ? FhirVersionList
                : VersionExtensions.FhirVersionsFromPackages(Dependencies);

        /// <summary>
        /// An optional value to indicate the type of package generated by the IG build tool
        /// </summary>
        /// <remarks>
        /// It's fairly random, so please do depend on it. Also note that the HL7 IG build tool
        /// creates template and tool packages and publishes them as a FHIR package
        /// even though they have little to do with FHIR packages.
        /// Known types are (case-insensitive):
        /// - Conformance: a set of Conformance Resources in the base package folder (/package)
        /// - IG: a FHIR implementation guide package(has an ImplementationGuide resource in /package,
        ///       along with conformance resources, and also contains example resources in
        ///       /package/example)
        /// - Core: contains the conformance related resources for the main FHIR
        ///         specification(effectively, this is a special type of "conformance" that marks it as
        ///         the core specification, which could also be inferred from it's name such as
        ///         hl7.fhir.r4.core, but other branches / ballots etc may vary, so this is simpler than
        ///         inferring from the name)
        /// - fhir.core: older format of 'core' package (deprecated)
        /// - Examples : contains the example resources found in the main FHIR specification in /package.
        /// - Group: a package that only includes (e.g.depends on) other packages (won't contain FHIR
        ///          resources directly). The versions listed in this package must include all the
        ///          versions found in the included packages. Note that this is is used for the set of
        ///          packages that represent a full core specification
        /// - Tool: A package that contains tool specific files to support specific tools (won't contain
        ///         FHIR resources or specify a FHIR version)
        /// - IG-Template: an IG template for use by IG publishing tools (won't contain FHIR resources or
        ///                specify a version)
        /// </remarks>
        [JsonProperty(PropertyName = "type")]
        public string? Type;

        public class Maintainer
        {
            [JsonProperty(PropertyName = "name")]
            public string? Name;

            [JsonProperty(PropertyName = "email")]
            public string? Email;

            [JsonProperty(PropertyName = "url")]
            public string? Url;
        }

        /// <summary>
        /// List of individual(s) responsible for maintaining the package.
        /// </summary>
        [JsonProperty(PropertyName = "maintainers")]
        public List<Maintainer>? Maintainers;

        /// <summary>
        /// For IG packages: The canonical url of the IG (equivalent to ImplementationGuide.url).
        /// </summary>
        [JsonProperty(PropertyName = "canonical")]
        public string? Canonical;

        /// <summary>
        /// For IG packages: Where the human readable representation (e.g. IG) is published on the web.
        /// </summary>
        [JsonProperty(PropertyName = "url")]
        public string? Url;

        /// <summary>
        /// Country code for the jurisdiction under which this package is published.
        /// </summary>
        /// <remarks>
        /// Formatted as an urn specifying the code system and code, e.g. "urn:iso:std:iso:3166#US".
        /// Typically from CommonJurisdictionCodes (http://hl7.org/fhir/ValueSet/jurisdiction-common) from the
        /// fhir.tx.support.* package (fhir.tx.support.r3, fhir.tx.support.r4, ...)
        /// </remarks>
        [JsonProperty(PropertyName = "jurisdiction")]
        public string? Jurisdiction;

        /// <summary>The date the package was published.</summary>
        /// <remarks>TODO: not documented at https://confluence.hl7.org/pages/viewpage.action?pageId=35718629#NPMPackageSpecification-Packagemanifest </remarks>
        [JsonConverter(typeof(ManifestDateJsonConverter))]
        [JsonProperty(PropertyName = "date")]
        public DateTimeOffset? Date;
    }


    public class AuthorInfo : Firely.Fhir.Packages.AuthorInfo
    {
        /// <summary>
        /// The npm specification allows author information to be serialized in json as a single string, or as a complex object. 
        /// This boolean keeps track of it was parsed from either one, so it can be serialized to the same output again.
        /// </summary>
        /// See issue: https://github.com/FirelyTeam/Firely.Fhir.Packages/issues/94
        [JsonIgnore]
        internal bool ParsedFromString = false;
    }

    /// <summary>
    /// Parse AuthorInfo object based on the following pattern "name <email> (url)"
    /// </summary>
    internal static class AuthorSerializer
    {

        private const char EMAIL_START_CHAR = '<';
        private const char EMAIL_END_CHAR = '>';
        private const char URL_START_CHAR = '(';
        private const char URL_END_CHAR = ')';

        internal static AuthorInfo Deserialize(string authorString)
        {
            var authorInfo = new AuthorInfo();

            // Extract name
            authorInfo.Name = getName(authorString);

            // Extract email
            authorInfo.Email = getStringBetweenCharacters(EMAIL_START_CHAR, EMAIL_END_CHAR, authorString);

            // Extract Url
            authorInfo.Url = getStringBetweenCharacters(URL_START_CHAR, URL_END_CHAR, authorString);

            //If author was set using parsing of a string, we will think it should be deserialized as a string too.
            authorInfo.ParsedFromString = true;

            return authorInfo;
        }

        internal static string Serialize(AuthorInfo authorInfo)
        {
            var builder = new StringBuilder();
            if (authorInfo.Name != null)
            {
                builder.Append(authorInfo.Name);
            }
            if (authorInfo.Email != null)
            {
                builder.Append($" {EMAIL_START_CHAR}{authorInfo.Email}{EMAIL_END_CHAR}");
            }
            if (authorInfo.Url != null)
            {
                builder.Append($" {URL_START_CHAR}{authorInfo.Url}{URL_END_CHAR}");
            }
            return builder.ToString().TrimStart();
        }

        private static string? getStringBetweenCharacters(char start, char end, string input)
        {
            // Extract email
            var urlStartIndex = input.IndexOf(start);
            if (urlStartIndex != -1)
            {
                var urlEndIndex = input.IndexOf(end, urlStartIndex);
                if (urlEndIndex != -1)
                {
                    return input.Substring(urlStartIndex + 1, urlEndIndex - urlStartIndex - 1).Trim();
                }
            }
            return null;
        }

        private static string? getName(string input)
        {
            if (input[0] == EMAIL_START_CHAR || input[0] == URL_START_CHAR)
                return null;

            var nameStartIndex = 0;

            var nameEndIndex = input.IndexOf(EMAIL_START_CHAR, nameStartIndex);
            if (nameEndIndex != -1)
            {
                return input.Substring(nameStartIndex, nameEndIndex - nameStartIndex).Trim();
            }
            else
            {
                nameEndIndex = input.IndexOf(URL_START_CHAR, nameStartIndex);
                return nameEndIndex != -1
                    ? input.Substring(nameStartIndex, nameEndIndex - nameStartIndex).Trim()
                    : input.Substring(nameStartIndex).Trim();
            }
        }

    }


    /// <summary>Information about a CI branch, as returned from a branch query to the server.</summary>
    public class CiBranchRecord
    {
        /// <summary>The relative name for this record.</summary>
        [JsonProperty(PropertyName = "name")]
        public string? Name;

        /// <summary>The size of the directory or file.</summary>
        [JsonProperty(PropertyName = "size")]
        public long? Size;

        /// <summary>URL of the resource, relative to the current URL.</summary>
        [JsonProperty(PropertyName = "url")]
        public string? Url;

        /// <summary>The file/directory mode.</summary>
        /// <remarks>This looks like a flag, but I cannot find documentation on values.</remarks>
        [JsonProperty(PropertyName = "mode")]
        public long? ModeFlag;

        /// <summary>True if is directory, false if not.</summary>
        [JsonProperty(PropertyName = "is_dir")]
        public bool? IsDirectory;

        /// <summary>True if is symbolic link, false if not.</summary>
        [JsonProperty(PropertyName = "is_symlink")]
        public bool? IsSymbolicLink;
    }

    /// <summary>FHIR QA record from the CI server.</summary>
    public class FhirCiQaRecord
    {
        [JsonProperty(PropertyName = "url")]
        public string? Url { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string? Name { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string? Title { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string? Description { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string? Status { get; set; }

        [JsonProperty(PropertyName = "package-id")]
        public string? PackageId { get; set; }

        [JsonProperty(PropertyName = "ig-ver")]
        public string? PackageVersion { get; set; }

        [JsonProperty(PropertyName = "date")]
        public DateTimeOffset? BuildDate { get; set; }

        [JsonProperty(PropertyName = "dateISO8601")]
        public DateTimeOffset? BuildDateIso { get; set; }

        [JsonProperty(PropertyName = "errs")]
        public int? ErrorCount { get; set; }

        [JsonProperty(PropertyName = "warnings")]
        public int? WarningCount { get; set; }

        [JsonProperty(PropertyName = "hints")]
        public int? HintCount { get; set; }

        [JsonProperty(PropertyName = "suppressed-hints")]
        public int? SuppressedHintCount { get; set; }

        [JsonProperty(PropertyName = "suppressed-warnings")]
        public int? SuppressedWarningCount { get; set; }

        [JsonProperty(PropertyName = "version")]
        public string? FhirVersion { get; set; }

        [JsonProperty(PropertyName = "tool")]
        public string? ToolingVersion { get; set; }

        [JsonProperty(PropertyName = "maxMemory")]
        public long? MaxMemoryUsedToBuild { get; set; }

        [JsonProperty(PropertyName = "repo")]
        public string? RepositoryUrl { get; set; }
    }
}
