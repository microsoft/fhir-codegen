using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using Firely.Fhir.Packages;
using Microsoft.Health.Fhir.CodeGenCommon.Polyfill;
using Newtonsoft.Json;

#nullable enable

namespace Microsoft.Health.Fhir.CodeGen._ForPackages
{
    /// <summary>
    /// Represents a client for interacting with a FHIR Continuous Integration (CI) server.
    /// </summary>
    public class FhirCiClient : IPackageServer, IDisposable
    {
        /// <summary>(Immutable) The ci scope.</summary>
        public const string FhirCiScope = "build.fhir.org";

        /// <summary>(Immutable) The default time to refresh the CI build listings, 0 for every request (no caching), -1 to never automatically refresh.</summary>
        private const int _defaultInvalidationSeconds = -1;

        /// <summary>(Immutable) Literal used to designate the branch name in the version string.</summary>
        private const string _ciBranchDelimiter = ".b-";

        /// <summary>(Immutable) The CI version date format.</summary>
        private const string _ciVersionDateFormat = "yyyyMMdd-HHmmssZ";

        /// <summary>(Immutable) The default branch names.</summary>
        private static readonly HashSet<string> _defaultBranchNames = new() { "main", "master" };

        /// <summary>(Immutable) URI of the FHIR CI server.</summary>
        private static readonly Uri _ciUri = new("http://build.fhir.org/");

        /// <summary>(Immutable) The ci URI using HTTPS.</summary>
        private static readonly Uri _ciUriS = new("https://build.fhir.org/");

        private static readonly Uri _ciBranchUri = new("https://build.fhir.org/branches/");

        /// <summary>(Immutable) URI of the qas.</summary>
        private static readonly Uri _qasUri = new("https://build.fhir.org/ig/qas.json");

        /// <summary>(Immutable) URI of the ig list.</summary>
        private static readonly Uri _igListUri = new("https://github.com/FHIR/ig-registry/blob/master/fhir-ig-list.json");

        /// <summary>(Immutable) The HTTP client.</summary>
        private readonly HttpClient _httpClient;

        /// <summary>The QAS records for the CI server, grouped by package id.</summary>
        private Dictionary<string, List<FhirCiQaRecord>> _qasByPackageId = new();

        /// <summary>The QAS records for the CI server.</summary>
        private FhirCiQaRecord[] _qas = Array.Empty<FhirCiQaRecord>();

        /// <summary>When the local copy of the CI QAS was last updated.</summary>
        private DateTimeOffset _qasLastUpdated = DateTimeOffset.MinValue;

        /// <summary>The listing invalidation in seconds.</summary>
        private int _listingInvalidationSeconds;

        /// <summary>Initializes a new instance of the <see cref="FhirCiClient"/> class.</summary>
        /// <param name="listingInvalidationSeconds">(Optional) The listing invalidation in seconds, 0 for
        ///  every request (no caching), -1 to never automatically refresh.</param>
        /// <param name="client">                    (Optional) The <see cref="HttpClient"/> instance to
        ///  use. If null, a new instance will be created.</param>
        public FhirCiClient(int listingInvalidationSeconds = _defaultInvalidationSeconds, HttpClient? client = null)
        {
            _listingInvalidationSeconds = listingInvalidationSeconds;
            _httpClient = client ?? new HttpClient();
        }

        /// <summary>Creates a new instance of the <see cref="FhirCiClient"/> class.</summary>
        /// <param name="listingInvalidationSeconds">(Optional) The listing invalidation in seconds, 0 for
        ///  every request (no caching), -1 to never automatically refresh.</param>
        /// <param name="insecure">                  (Optional) Specifies whether to create an insecure
        ///  client.</param>
        /// <returns>A new instance of the <see cref="FhirCiClient"/> class.</returns>
        public static FhirCiClient Create(int listingInvalidationSeconds = _defaultInvalidationSeconds, bool insecure = false)
        {
            var httpClient = new HttpClient();
            return new FhirCiClient(listingInvalidationSeconds, httpClient);
        }

        /// <inheritdoc/>
        public override string? ToString() => _ciUriS.ToString();

        /// <summary>Updates the ci listing cache.</summary>
        /// <returns>An asynchronous result.</returns>
        public async Task UpdateCiListingCache()
        {
            _ = await getQAs(true);
        }

        /// <summary>Get the QA records and update the cache if necessary.</summary>
        /// <param name="forceRefresh">(Optional) True to force a refresh.</param>
        /// <returns>An asynchronous result that yields a list of.</returns>
        private async Task<(FhirCiQaRecord[] qas, Dictionary<string, List<FhirCiQaRecord>> qasByPackageId)> getQAs(bool forceRefresh = false)
        {
            // check for having a cached copy and configuration to never refresh the cache
            if (!forceRefresh && (_listingInvalidationSeconds == -1) && (_qas.Length != 0))
            {
                // return what we have
                return (_qas, _qasByPackageId);
            }

            // check for having a cached copy and not needing to refresh
            if (!forceRefresh &&
                (_listingInvalidationSeconds > 0) &&
                (_qasLastUpdated.AddSeconds(_listingInvalidationSeconds) >= DateTimeOffset.Now))
            {
                // return what we have
                return (_qas, _qasByPackageId);
            }

            List<FhirCiQaRecord>? updatedGuideQas = await downloadGuideQAs();
            List<FhirCiQaRecord>? updatedCoreQas = await downloadCoreQAs();

            // join our sets together
            FhirCiQaRecord[] updatedQAs = (updatedCoreQas ?? Enumerable.Empty<FhirCiQaRecord>()).Concat(updatedGuideQas ?? Enumerable.Empty<FhirCiQaRecord>()).ToArray();
            if (updatedQAs.Length == 0)
            {
                return (_qas, _qasByPackageId);
            }

            Dictionary<string, List<FhirCiQaRecord>> qasByPackageId = new();

            // iterate over the QAS records and add them to a dictionary
            foreach (FhirCiQaRecord qas in updatedQAs)
            {
                if (qas.PackageId == null)
                {
                    continue;
                }

                if (!qasByPackageId.TryGetValue(qas.PackageId, out List<FhirCiQaRecord>? qasRecs))
                {
                    qasRecs = new List<FhirCiQaRecord>();
                    qasByPackageId.Add(qas.PackageId, qasRecs);
                }

                qasRecs.Add(qas);
            }

            // update our cache if necessary
            if (forceRefresh || (_listingInvalidationSeconds != 0))
            {
                _qas = updatedQAs;
                _qasByPackageId = qasByPackageId;
                _qasLastUpdated = DateTimeOffset.Now;

                // return our updated version
                return (_qas, _qasByPackageId);
            }

            // return what we downloaded
            return (updatedQAs, qasByPackageId);
        }

        /// <summary>
        /// Downloads the current qas.json file from the build server and deserializes into an array of <see cref="FhirCiQaRecord"/>.
        /// </summary>
        /// <returns>An array of FhirCiQaRecord objects representing the current FhirCiQaRecords.</returns>
        private async Task<List<FhirCiQaRecord>?> downloadGuideQAs()
        {
            // download the QA records from the build server
            HttpRequestMessage request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = _qasUri,
                Headers =
                {
                    Accept =
                    {
                        new MediaTypeWithQualityHeaderValue("application/json"),
                    },
                },
            };

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            System.Net.HttpStatusCode statusCode = response.StatusCode;

            if (statusCode != System.Net.HttpStatusCode.OK)
            {
                return null;
            }

            string json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            List<FhirCiQaRecord>? qas = JsonConvert.DeserializeObject<List<FhirCiQaRecord>>(json);

            // if we do not have records, we are done
            if (qas == null)
            {
                return null;
            }

            return qas;
        }

        private async Task<List<FhirCiQaRecord>> downloadCoreQAs()
        {
            List<FhirCiQaRecord> qas = new();

            // download the branch list from the core build
            HttpRequestMessage request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = _ciBranchUri,
                Headers =
                {
                    Accept =
                    {
                        new MediaTypeWithQualityHeaderValue("application/json"),
                    },
                },
            };

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            System.Net.HttpStatusCode statusCode = response.StatusCode;

            if (statusCode != System.Net.HttpStatusCode.OK)
            {
                return qas;
            }

            string json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(json))
            {
                return qas;
            }

            CiBranchRecord[]? coreBranches = JsonConvert.DeserializeObject<CiBranchRecord[]>(json);

            if (coreBranches == null)
            {
                return qas;
            }

            // traverse the core branches to build their records
            foreach (CiBranchRecord ciBranchRec in coreBranches)
            {
                if ((ciBranchRec.Name == null) ||
                    string.IsNullOrEmpty(ciBranchRec.Name) ||
                    string.IsNullOrEmpty(ciBranchRec.Url))
                {
                    continue;
                }

                // download the version.info for this branch
                request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(_ciBranchUri, ciBranchRec.Url + "version.info"),
                    Headers =
                    {
                        Accept =
                        {
                            new MediaTypeWithQualityHeaderValue("text/plain"),
                        },
                    },
                };

                response = await _httpClient.SendAsync(request);
                statusCode = response.StatusCode;

                if (statusCode != System.Net.HttpStatusCode.OK)
                {
                    continue;
                }

                string contents = await response.Content.ReadAsStringAsync();

                // grab the contents we can out of the version.info file
                parseVersionInfoIni(
                    contents,
                    out string ciFhirVersion,
                    out string ciVersion,
                    out string ciBuildId,
                    out DateTimeOffset? ciBuildDate);

                string packageId = $"hl7.fhir.r{ciFhirVersion.Split('.').First()}.core";

                if (ciBranchRec.Name == "master/")
                {
                    // build a QA record for the main branch
                    qas.Add(new()
                    {
                        Url = "https://build.fhir.org",
                        Name = "FHIR Core " + ciFhirVersion,
                        Title = "FHIR Core build",
                        Status = "draft",
                        PackageId = packageId,
                        PackageVersion = ciVersion,
                        BuildDate = ciBuildDate,
                        BuildDateIso = ciBuildDate,
                        FhirVersion = ciFhirVersion,
                        RepositoryUrl = "HL7/fhir/branches/master/qa.json"
                    });
                }
                else
                {
                    // build a QA record for this branch
                    qas.Add(new()
                    {
                        Url = "https://build.fhir.org/branches/" + ciBranchRec.Name.Substring(0, ciBranchRec.Name.Length - 1),
                        Name = "FHIR Core " + ciFhirVersion + " branch: " + ciBranchRec.Name,
                        Title = "FHIR Core build",
                        Status = "draft",
                        PackageId = packageId,
                        PackageVersion = ciVersion,
                        BuildDate = ciBuildDate,
                        BuildDateIso = ciBuildDate,
                        FhirVersion = ciFhirVersion,
                        RepositoryUrl = $"HL7/fhir/branches/{ciBranchRec.Name}/qa.json"
                    });
                }
            }

            return qas;
        }


        /// <summary>Gets local version information.</summary>
        /// <exception cref="FileNotFoundException">Thrown when the requested file is not present.</exception>
        /// <param name="contents">   The contents.</param>
        /// <param name="fhirVersion">[out] The FHIR version.</param>
        /// <param name="version">    [out] The version string (e.g., 4.0.1).</param>
        /// <param name="buildId">    [out] Identifier for the build.</param>
        /// <param name="buildDate">  [out] The build date.</param>
        private static void parseVersionInfoIni(
            string contents,
            out string fhirVersion,
            out string version,
            out string buildId,
            out DateTimeOffset? buildDate)
        {
            IEnumerable<string> lines = contents.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            fhirVersion = string.Empty;
            version = string.Empty;
            buildId = string.Empty;
            buildDate = null;

            foreach (string line in lines)
            {
                if (!line.Contains('='))
                {
                    continue;
                }

                string[] kvp = line.Split('=');

                if (kvp.Length != 2)
                {
                    continue;
                }

                switch (kvp[0])
                {
                    case "FhirVersion":
                        fhirVersion = kvp[1];
                        break;

                    case "version":
                        version = kvp[1];
                        break;

                    case "buildId":
                        buildId = kvp[1];
                        break;

                    case "date":
                        {
                            if (DateTimeOffset.TryParseExact(
                                kvp[1],
                                "yyyyMMddHHmmss",
                                CultureInfo.InvariantCulture.DateTimeFormat,
                                DateTimeStyles.None, out DateTimeOffset dto))
                            {
                                buildDate = dto;
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Builds the version string for a FhirCiQaRecord.
        /// </summary>
        /// <param name="qa">The FhirCiQaRecord.</param>
        /// <returns>The version string.</returns>
        private static string buildVersionString(FhirCiQaRecord qa)
        {
            string versionPrerelease = qa.PackageVersion?.Contains('-') ?? false
                ? string.Empty
                : "-cibuild";

            // prefer the date of the build as the build metadata
            string? buildMeta = qa.BuildDateIso?.ToUniversalTime().ToString(_ciVersionDateFormat)
                ?? qa.BuildDate?.ToUniversalTime().ToString(_ciVersionDateFormat);

            // if we do not have a date, mangle the branch info
            if (buildMeta == null)
            {
                (string? branchName, bool isDefaultBranch) = GetBranchNameRepoLiteral(qa.RepositoryUrl);

                versionPrerelease = isDefaultBranch || string.IsNullOrEmpty(branchName)
                    ? versionPrerelease
                    : (versionPrerelease + _ciBranchDelimiter + cleanForSemVer(branchName!));

                // add the repo portions to the version
                string[] repoComponents = qa.RepositoryUrl?.Split('/') ?? Array.Empty<string>();
                if (repoComponents.Length > 2)
                {
                    buildMeta = repoComponents[0] + "." + repoComponents[1];
                }
                else
                {
                    buildMeta = "ci";
                }
            }

            string packageVersion = qa.PackageVersion ?? "0.0.0";

            return $"{packageVersion}{versionPrerelease}+{cleanForSemVer(buildMeta)}";
        }

        /// <summary>
        /// Generates a <see cref="PackageListing"/> from a list of <see cref="FhirCiQaRecord"/>.
        /// </summary>
        /// <param name="qaRecs">The list of <see cref="FhirCiQaRecord"/>.</param>
        /// <returns>The generated <see cref="PackageListing"/>.</returns>
        private PackageListing? listingFromQaRecs(List<FhirCiQaRecord> qaRecs)
        {
            PackageListing? listing = null;

            // iterate over the records in our dictionary - sort by status so that 'active' comes before 'retired'
            foreach (FhirCiQaRecord qa in qaRecs.OrderBy(qa => qa.Status))
            {
                listing ??= new()
                {
                    Id = qa.PackageId,
                    Name = qa.Name,
                    Description = $"CI Build of {qa.PackageId}",
                    DistTags = null,
                    Versions = new(),
                };

                int igUrlIndex = qa.Url?.IndexOf("/ImplementationGuide/", StringComparison.Ordinal) ?? -1;
                string? qaUrl = igUrlIndex == -1 ? qa.Url : qa.Url!.Substring(0, igUrlIndex);

                string versionLiteral = buildVersionString(qa);

                if (listing.Versions!.ContainsKey(versionLiteral))
                {
                    continue;
                }

                listing.Versions.Add(versionLiteral, new()
                {
                    Name = qa.PackageId,
                    Version = versionLiteral,
                    Description = qa.Description ?? qa.Title ?? qa.RepositoryUrl,
                    FhirVersion = qa.FhirVersion,
                    Url = qaUrl,
                    Dist = new()
                    {
                        Tarball = qaUrl,
                    },
                });
            }

            if ((listing?.DistTags == null) && (listing?.Versions?.Count > 0))
            {
                listing.DistTags ??= new();

                foreach (FhirCiQaRecord qa in qaRecs.OrderBy(qa => qa.BuildDateIso ?? qa.BuildDate))
                {
                    (string? branchName, bool isDefaultBranch) = GetBranchNameRepoLiteral(qa.RepositoryUrl);

                    string tag = branchName == null
                        ? "current"
                        : "current$" + branchName;

                    string versionLiteral = buildVersionString(qa);

                    // add the full tag if we have it
                    if (!listing.DistTags.ContainsKey(tag))
                    {
                        listing.DistTags.Add(tag, versionLiteral);
                    }

                    // check for default branches to add them as well
                    if (isDefaultBranch &&
                        (!listing.DistTags.ContainsKey("current")))
                    {
                        listing.DistTags.Add("current", versionLiteral);
                    }
                }
            }

            return listing;
        }

        /// <summary>
        /// Retrieve a package listing for a Package
        /// </summary>
        /// <remarks>
        /// Note that is function creates package listing records based on QA records from the build server.
        /// </remarks>
        /// <param name="pkgname">Name of the package.</param>
        /// <returns>Package listing.</returns>
        public async ValueTask<PackageListing?> DownloadListingAsync(string pkgname)
        {
            (FhirCiQaRecord[] _, Dictionary<string, List<FhirCiQaRecord>> qasByPackageId) = await getQAs();

            if (string.IsNullOrEmpty(pkgname) ||
                !qasByPackageId.TryGetValue(pkgname, out List<FhirCiQaRecord>? qaRecs) ||
                qaRecs.Count == 0)
            {
                return null;
            }

            return listingFromQaRecs(qaRecs);
        }

        /// <summary>
        /// Extracts the branch name from a repository URL.
        /// </summary>
        /// <param name="partialRepoLiteral">The partial repository URL.</param>
        /// <returns>The branch name extracted from the repository URL, or null if the branch name cannot be determined.</returns>
        public static (string? branchName, bool isDefaultBranch) GetBranchNameRepoLiteral(string? partialRepoLiteral)
        {
            if ((partialRepoLiteral == null) ||
                string.IsNullOrEmpty(partialRepoLiteral))
            {
                return (null, false);
            }

            int branchStart = partialRepoLiteral.IndexOf("branches/") + 9;

            if (branchStart == -1)
            {
                branchStart = partialRepoLiteral.IndexOf("tree/") + 5;
            }

            if (branchStart == -1)
            {
                return (null, false);
            }

            int branchEnd = partialRepoLiteral.IndexOf('/', branchStart);

            string branchName = branchEnd == -1
                ? partialRepoLiteral.Substring(branchStart)
                : partialRepoLiteral.Substring(branchStart, branchEnd - branchStart);

            return (branchName, _defaultBranchNames.Contains(branchName));
        }

        /// <summary>Get a list of package catalogs, based on optional parameters.</summary>
        /// <param name="pkgname">    (Optional) Name of the package.</param>
        /// <param name="fhirversion">(Optional) The FHIR version of a package.</param>
        /// <param name="site">       (Optional) URL of the site.</param>
        /// <param name="repo">       (Optional) The repository.</param>
        /// <param name="branch">     (Optional) The branch.</param>
        /// <param name="preview">    (Optional) Allow for prelease packages.</param>
        /// <returns>A list of package catalogs that conform to the parameters.</returns>
        public async ValueTask<List<PackageCatalogEntry>> CatalogPackagesAsync(
            string? pkgname = null,
            string? fhirversion = null,
            string? site = null,
            string? repo = null,
            string? branch = null,
            bool preview = true)
        {
            List<PackageCatalogEntry> entries = new();

            (FhirCiQaRecord[] qas, Dictionary<string, List<FhirCiQaRecord>> qasByPackageId) = await getQAs();

            HashSet<string> usedIds = new();

            // remove any trailing slashes from the site URL - QAs.json does not have them
            if ((site != null) && site.EndsWith("/"))
            {
                site = site.Substring(0, site.Length - 1);
            }

            // sanitize any repository URL - QAs.json does not repeat the GitHub URL portion
            if ((repo != null) && repo.StartsWith("http://github.com/"))
            {
                repo = repo.Substring(18);
            }
            else if ((repo != null) && repo.StartsWith("https://github.com/"))
            {
                repo = repo.Substring(19);
            }

            if ((branch != null) && (!branch.Contains('/')))
            {
                branch = "/branches/" + branch + "/qa.json";
            }

            // if there was a package name provided, we can use our dictionary for lookup
            if (pkgname != null)
            {
                if (!qasByPackageId.TryGetValue(pkgname, out List<FhirCiQaRecord>? qasRecs))
                {
                    return entries;
                }

                // iterate over the records in our dictionary
                foreach (FhirCiQaRecord qa in qasRecs)
                {
                    // skip anything we have already added - duplicates are likely forks but we lose that granularity here
                    if (usedIds.Contains(qa.PackageId ?? string.Empty))
                    {
                        continue;
                    }

                    if ((fhirversion != null) && (qa.FhirVersion != fhirversion))
                    {
                        continue;
                    }

                    if ((site != null) && (qa.Url != site))
                    {
                        continue;
                    }

                    if ((repo != null) && (!qa.RepositoryUrl?.StartsWith(repo) ?? false))
                    {
                        continue;
                    }

                    if ((branch != null) && (!qa.RepositoryUrl?.EndsWith(branch) ?? false))
                    {
                        continue;
                    }

                    // only use this package if it passes all the other filters
                    usedIds.Add(qa.PackageId ?? string.Empty);
                    entries.Add(new PackageCatalogEntry()
                    {
                        Name = qa.PackageId,
                        Description = qa.RepositoryUrl,
                        FhirVersion = qa.FhirVersion,
                    });
                }

                return entries;
            }

            // iterate over all the QA records
            foreach (FhirCiQaRecord qa in qas)
            {
                // skip anything we have already added - duplicates are likely forks but we lose that granularity here
                if (usedIds.Contains(qa.PackageId ?? string.Empty))
                {
                    continue;
                }

                if ((fhirversion != null) && (qa.FhirVersion != fhirversion))
                {
                    continue;
                }

                if ((site != null) && (qa.Url != site))
                {
                    continue;
                }

                if ((repo != null) && (!qa.RepositoryUrl?.StartsWith(repo) ?? false))
                {
                    continue;
                }

                if ((branch != null) && (!qa.RepositoryUrl?.EndsWith(branch) ?? false))
                {
                    continue;
                }

                // only use this package if it passes all the other filters
                usedIds.Add(qa.PackageId ?? string.Empty);
                entries.Add(new PackageCatalogEntry()
                {
                    Name = qa.PackageId,
                    Description = qa.RepositoryUrl,
                    FhirVersion = qa.FhirVersion,
                });
            }

            return entries;
        }

        /// <summary>
        /// Cleans the input string to be usable in a semantic versioning component.
        /// </summary>
        /// <param name="value">The input string to be cleaned.</param>
        /// <returns>The cleaned string for semantic versioning.</returns>
        private static string cleanForSemVer(string value)
        {
            List<char> clean = new(value.Length);

            foreach (char c in value)
            {
                if (char.IsLetterOrDigit(c))
                {
                    clean.Add(c);
                    continue;
                }

                clean.Add('-');
            }

            return new string(clean.ToArray());
        }

        /// <summary>
        /// Retrieves the FhirCiQaRecord for the specified package name and version or tag.
        /// </summary>
        /// <param name="name">The name of the package.</param>
        /// <param name="versionDiscriminator">The version, tag, or branch of the package.</param>
        /// <returns>The FhirCiQaRecord for the specified package name and version or tag, or null if not found.</returns>
        private async ValueTask<FhirCiQaRecord?> getQaRecord(string? name, string? versionDiscriminator)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            (FhirCiQaRecord[] _, Dictionary<string, List<FhirCiQaRecord>> qasByPackageId) = await getQAs();

            if (!qasByPackageId.TryGetValue(name!, out List<FhirCiQaRecord>? qaRecs))
            {
                return null;
            }

            // no version resolves to the 'current' tag by default
            string requestedVersion = versionDiscriminator switch
            {
                null => "current",
                _ => versionDiscriminator,
            };

            // check for using a tag and resolve it to a version
            if (!requestedVersion.Contains('+'))
            {
                PackageListing? listing = listingFromQaRecs(qaRecs);

                if (listing != null)
                {
                    // check for a tag
                    if ((listing.DistTags?.TryGetValue(requestedVersion, out string? tagVersion) == true) &&
                        (listing.Versions?.TryGetValue(tagVersion, out PackageRelease? _) == true))
                    {
                        requestedVersion = tagVersion;
                    }
                    // check for a branch name
                    else if ((listing.DistTags?.TryGetValue("current$" + requestedVersion, out tagVersion) == true) &&
                             (listing.Versions?.TryGetValue(tagVersion, out PackageRelease? _) == true))
                    {
                        requestedVersion = tagVersion;
                    }
                }
            }

            string[] rvComponents = requestedVersion.Split('+');

            if ((rvComponents.Length > 1) &&
                DateTimeOffset.TryParseExact(
                    rvComponents.Last(),
                    _ciVersionDateFormat,
                    CultureInfo.InvariantCulture.DateTimeFormat,
                    DateTimeStyles.None, out DateTimeOffset requestedDto))
            {
                // traverse the records in the package looking for a match
                foreach (FhirCiQaRecord qa in qaRecs)
                {
                    if ((qa.BuildDateIso == requestedDto) ||
                        (qa.BuildDate == requestedDto))
                    {
                        return qa;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Downloads a package from the source.
        /// </summary>
        /// <param name="reference">Package reference of the package to be downloaded.</param>
        /// <returns>Package content as a byte array.</returns>
        private async ValueTask<byte[]> downloadPackage(PackageReference reference, FhirCiQaRecord? qa = null)
        {
            if (reference.Scope != FhirCiScope)
            {
                throw new Exception($"CI packages must be tagged with the Scope: {FhirCiScope}!");
            }

            // find our package in the QA listings
            qa ??= await getQaRecord(reference.Name, reference.Version);

            if (qa == null)
            {
                throw new Exception($"Cannot resolve {reference.Moniker} in CI Build QA records!");
            }

            // extract the branch name
            (string? branchName, bool _) = GetBranchNameRepoLiteral(qa.RepositoryUrl);

            // build the URL
            int igUrlIndex = qa.RepositoryUrl?.IndexOf("/qa.json", StringComparison.Ordinal) ?? -1;
            string url = igUrlIndex == -1 ? qa.RepositoryUrl! : qa.RepositoryUrl!.Substring(0, igUrlIndex);

            url += url.EndsWith('/')
                ? "package.tgz"
                : "/package.tgz";

            if (!url.StartsWith("http"))
            {
                if (url.StartsWith("HL7/fhir/", StringComparison.OrdinalIgnoreCase))
                {
                    url = "https://build.fhir.org/" + url;
                }
                else
                {
                    url = "https://build.fhir.org/ig/" + url;
                }
            }

            // download data
            return await _httpClient.GetByteArrayAsync(url).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves a tag-based package reference based on the provided name and version or tag.
        /// </summary>
        /// <param name="name">        The name of the package.</param>
        /// <param name="versionDiscriminator">The version, tag, or branch of the package.</param>
        /// <param name="qa">          (Optional) The FhirCiQaRecord.</param>
        /// <returns>The package reference.</returns>
        public async ValueTask<PackageReference> GetTagBasedReference(string? name, string? versionDiscriminator, FhirCiQaRecord? qa = null)
        {
            // find our package in the QA listings
            qa ??= await getQaRecord(name, versionDiscriminator);

            if (qa == null)
            {
                return PackageReference.None;
            }

            (string? branchName, bool isDefaultBranch) = GetBranchNameRepoLiteral(qa.RepositoryUrl);

            string tag;
            if ((branchName == null) ||
                (isDefaultBranch && (!versionDiscriminator?.Contains(branchName) ?? true)))
            {
                tag = "current";
            }
            else
            {
                tag = "current$" + branchName;
            }

            return new PackageReference(FhirCiScope, qa.PackageId ?? name!, tag);
        }

        /// <summary>
        /// Retrieves the resolved-version reference for a package based on its name and version or tag.
        /// </summary>
        /// <param name="name">        The name of the package.</param>
        /// <param name="versionDiscriminator">The version, tag, or branch of the package.</param>
        /// <param name="qa">          (Optional) The FhirCiQaRecord.</param>
        /// <returns>The resolved package reference.</returns>
        public async ValueTask<PackageReference> GetResolvedReference(string? name, string? versionDiscriminator, FhirCiQaRecord? qa = null)
        {
            // find our package in the QA listings
            qa ??= await getQaRecord(name, versionDiscriminator);

            if (qa == null)
            {
                return PackageReference.None;
            }

            string version = buildVersionString(qa);

            return new PackageReference(FhirCiScope, qa.PackageId ?? name!, version);
        }

        /// <summary>
        /// Retrieves the tagged and resolved package references for a given package name and version or tag.
        /// </summary>
        /// <param name="name">The name of the package.</param>
        /// <param name="versionDiscriminator">The version, tag, or branch of the package.</param>
        /// <returns>A tuple containing the tagged and resolved package references.</returns>
        public async ValueTask<(PackageReference tagged, PackageReference resolved)> GetReferences(string name, string? versionDiscriminator)
        {
            // find our package in the QA listings
            FhirCiQaRecord? qa = await getQaRecord(name, versionDiscriminator);

            return (await GetTagBasedReference(name, versionDiscriminator, qa), await GetResolvedReference(name, versionDiscriminator, qa));
        }

        /// <summary>
        /// Retrieve a list of all versions of a package id on the build server.
        /// </summary>
        /// <param name="name">Package name.</param>
        /// <returns>List of versions.</returns>
        public async Task<Versions?> GetVersions(string name)
        {
            PackageListing? listing = await DownloadListingAsync(name);
            if (listing == null)
            {
                return new();
            }

            Versions v = listing.ToVersions();

            if (v.Items.Count != listing.Versions!.Count)
            {
                throw new Exception("Version count mismatch!");
            }

            return v;

            //return listing is null ? new Versions() : listing.ToVersions();
        }

        /// <summary>
        /// Download a package from the source.
        /// </summary>
        /// <param name="reference">Package reference of the package to be downloaded.</param>
        /// <returns>Package content as byte array.</returns>
        public async Task<byte[]> GetPackage(PackageReference reference)
        {
            return await downloadPackage(reference);
        }

        /// <summary>
        /// Checks if a package is outdated by comparing its build date with the build date from the QA listings.
        /// </summary>
        /// <param name="reference">The package reference.</param>
        /// <param name="cache">The package cache.</param>
        /// <returns>True if the package is outdated, false if it is up to date, or null if the package is not found in the QA listings.</returns>
        public async Task<bool?> IsPackageOutdated(PackageReference reference, IPackageCache cache)
        {
            if (cache is not DiskPackageCache diskCache)
            {
                throw new Exception("FhirCiClient requires a DiskPackageCache for IsPackageOutdated");
            }

            // find our package in the QA listings
            FhirCiQaRecord? qa = await getQaRecord(reference.Name, reference.Version);

            if (qa == null)
            {
                return null;
            }

            // make sure we have the tag and resolved version references for comparison
            PackageReference taggedReference = await GetTagBasedReference(reference.Name, reference.Version, qa);
            PackageReference resolvedReference = await GetResolvedReference(reference.Name, reference.Version, qa);

            // if either fails, we cannot proceed
            if ((taggedReference == PackageReference.None) ||
                (resolvedReference == PackageReference.None))
            {
                return null;
            }

            // check for a local copy - CI builds are installed by tag
            bool isInstalled = await cache.IsInstalled(taggedReference);

            if (!isInstalled)
            {
                return true;
            }

            // get the manifest from the local copy
            PackageManifest? installedManifest = await diskCache.ReadManifestEx(taggedReference);

            if (installedManifest == null)
            {
                return true;
            }

            // compare the build date from QA and the installed manifest
            return (installedManifest.Date ?? DateTimeOffset.MinValue) < (qa.BuildDateIso ?? qa.BuildDate ?? DateTimeOffset.MaxValue);
        }

        /// <summary>
        /// Installs or updates a package from the FHIR CI server.
        /// </summary>
        /// <param name="reference">The package reference to install or update.</param>
        /// <param name="cache">The package cache.</param>
        public async Task InstallOrUpdate(PackageReference reference, _ForPackages.DiskPackageCache cache)
        {
            if (cache is not DiskPackageCache diskCache)
            {
                throw new Exception("FhirCiClient requires a DiskPackageCache for IsPackageOutdated");
            }

            // find our package in the QA listings
            FhirCiQaRecord? qa = await getQaRecord(reference.Name, reference.Version);

            if (qa == null)
            {
                throw new Exception($"Could not resolve {reference.Moniker} on the CI server");
            }

            // make sure we have the tag and resolved version references for comparison
            PackageReference taggedReference = await GetTagBasedReference(reference.Name, reference.Version, qa);
            PackageReference resolvedReference = await GetResolvedReference(reference.Name, reference.Version, qa);

            // if either fails, we cannot proceed
            if ((taggedReference == PackageReference.None) ||
                (resolvedReference == PackageReference.None))
            {
                throw new Exception($"Could not resolve version information for {reference.Moniker} on the CI server");
            }

            // check for a local copy - CI builds are installed by tag
            bool isInstalled = await cache.IsInstalled(taggedReference);

            // get the manifest from the local copy
            PackageManifest? installedManifest = isInstalled ? await diskCache.ReadManifestEx(taggedReference) : null;

            // compare the build date from QA and the installed manifest
            bool shouldDownload = (installedManifest?.Date ?? DateTimeOffset.MinValue) < (qa.BuildDateIso ?? qa.BuildDate ?? DateTimeOffset.MaxValue);

            if (!shouldDownload)
            {
                return;
            }

            if (isInstalled)
            {
                // need to delete the existing content
                await cache.Delete(taggedReference);
            }

            // download happens via the resolved version
            byte[] data = await downloadPackage(resolvedReference, qa);

            // install happens via the tagged version
            await cache.Install(taggedReference, data);
        }

        #region IDisposable

        private bool _disposed;

        /// <inheritdoc/>
        void IDisposable.Dispose() => Dispose(true);

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="FhirCiClient"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // [WMR 20181102] HttpClient will dispose internal HttpClientHandler/WebRequestHandler
                    _httpClient?.Dispose();
                }

                // release any unmanaged objects
                // set the object references to null
                _disposed = true;
            }
        }

        #endregion
    }
}

#nullable restore
