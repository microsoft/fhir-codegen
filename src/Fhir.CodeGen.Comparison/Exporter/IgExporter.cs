using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Fhir.CodeGen.Common.Packaging;
using Fhir.CodeGen.Common.Utils;
using Fhir.CodeGen.Comparison.Models;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Logging;
using Octokit;
using static Fhir.CodeGen.Packages.Models.PackageIndex;

namespace Fhir.CodeGen.Comparison.Exporter;

public class IgExporter
{
    public class XVerExportTrackingRecord
    {
        public List<XVerIgExportTrackingRecord> XVerIgs { get; set; } = [];
        public List<ValidationIgExportTrackingRecord> ValidationIgs { get; set; } = [];
    }

    public class XVerIgExportTrackingRecord
    {
        public required FhirPackageComparisonPair PackagePair { get; set; }
        public required string PackageId { get; set; }
        public string PackageUrl =>
            $"http://hl7.org/fhir/uv/xver/ImplementationGuide/{PackageId}";

        public string? IgRootDir { get; set; } = null;
        public string? InputDir { get; set; } = null;
        public string? IncludesDir { get; set; } = null;
        public string? PageContentDir { get; set; } = null;

        public XVerIgFileRecord? IgIndexFile { get; set; } = null;

        public string? VocabularyDir { get; set; } = null;
        public List<XVerIgFileRecord> CodeSystemFiles { get; set; } = [];
        public List<XVerIgFileRecord> ValueSetFiles { get; set; } = [];
        public string? VocabMapDir { get; set; } = null;
        public List<XVerIgFileRecord> VsConceptMapFiles { get; set; } = [];

        public string? ExtensionDir { get; set; } = null;
        public List<XVerIgFileRecord> ExtensionFiles { get; set; } = [];

        public string? ProfileDir { get; set; } = null;
        public List<XVerIgFileRecord> ProfileFiles { get; set; } = [];

        public string? ResourceMapDir { get; set; } = null;
        public List<XVerIgFileRecord> ResourceMapFiles { get; set; } = [];
        public string? ElementMapDir { get; set; } = null;
        public List<XVerIgFileRecord> ElementMapFiles { get; set; } = [];

        public PackageContents AsPackageContents()
        {
            if (IgIndexFile is null)
            {
                throw new Exception("IG Index file is required to create PackageContents.");
            }

            List<PackageContents.PackageFile> files = [
                IgIndexFile.AsPackageFile(),
                ];

            files.AddRange(CodeSystemFiles.Select(f => f.AsPackageFile()));
            files.AddRange(ValueSetFiles.Select(f => f.AsPackageFile()));
            files.AddRange(VsConceptMapFiles.Select(f => f.AsPackageFile()));
            files.AddRange(ExtensionFiles.Select(f => f.AsPackageFile()));
            files.AddRange(ProfileFiles.Select(f => f.AsPackageFile()));
            files.AddRange(ResourceMapFiles.Select(f => f.AsPackageFile()));
            files.AddRange(ElementMapFiles.Select(f => f.AsPackageFile()));

            return new PackageContents()
            {
                IndexVersion = 2,
                Files = files,
            };
        }
    }

    public class ValidationIgExportTrackingRecord
    {
        public required DbFhirPackage Package { get; set; }
        public required string PackageId { get; set; }

        public string? IgRootDir { get; set; } = null;
        public string? InputDir { get; set; } = null;
        public string? IncludesDir { get; set; } = null;
        public string? PageContentDir { get; set; } = null;

        public XVerIgFileRecord? IgIndexFile { get; set; } = null;


        public PackageContents AsPackageContents()
        {
            if (IgIndexFile is null)
            {
                throw new Exception("IG Index file is required to create PackageContents.");
            }

            return new PackageContents()
            {
                IndexVersion = 2,
                Files = [ IgIndexFile.AsPackageFile() ],
            };
        }
    }

    public record class XVerIgFileRecord
    {
        [JsonPropertyName("filename")]
        public required string FileName { get; init; }

        [JsonIgnore]
        public required string FileNameWithoutExtension { get; init; }

        [JsonIgnore]
        public required bool IsPageContentFile { get; init; }

        [JsonIgnore]
        public required string Name { get; init; }

        [JsonPropertyName("id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public required string? Id { get; init; }

        [JsonPropertyName("url")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public required string? Url { get; init; }

        [JsonPropertyName("resourceType")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public required string? ResourceType { get; init; }

        /// <summary>
        /// Resource `version` value, if applicable *and* different from the IG itself (e.g., CodeSystem.version).
        /// </summary>
        [JsonPropertyName("version")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Version { get; init; } = null;

        [JsonIgnore]
        public required string Description { get; init; }

        [JsonIgnore]
        public string? GroupingId { get; init; } = null;

        [JsonIgnore]
        public bool? IsExample { get; init; } = null;

        [JsonIgnore]
        public List<string>? Profiles { get; init; } = null;

        /// <summary>
        /// Resource `kind` value, if applicable (e.g., CodeSystem.kind).
        /// </summary>
        [JsonPropertyName("kind")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? KindValue { get; init; } = null;

        /// <summary>
        /// Resource `type` value, if applicable (e.g., StructureDefinition.type).
        /// </summary>
        [JsonPropertyName("type")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TypeValue { get; init; } = null;

        /// <summary>
        /// Resource `derivation` value, if applicable (e.g., StructureDefinition.derivation).
        /// </summary>
        [JsonPropertyName("derivation")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? DerivationValue { get; init; } = null;

        /// <summary>
        /// Resource `valueSet` value, if applicable (e.g., CodeSystem.valueSet).
        /// </summary>
        [JsonPropertyName("valueSet")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ValueSetValue { get; init; } = null;

        [JsonPropertyName("hasSnapshot")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? HasSnapshot { get; init; } = null;

        [JsonPropertyName("hasExpansion")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? HasExpansion { get; init; } = null;

        public PackageContents.PackageFile AsPackageFile() => new()
        {
            FileName = FileName,
            ResourceType = ResourceType,
            Id = Id,
            Url = Url,
            Version = Version,
            Kind = KindValue,
            Type = TypeValue,
            Derivation = DerivationValue,
        };
    }

    private readonly XVerExporter _exporter;

    private readonly IDbConnection _db;

    private ILoggerFactory _loggerFactory;
    private ILogger _logger;

    private List<DbFhirPackage> _packages = [];

    private string _outputPath;
    private string? _crossVersionSourcePath;

    private static Dictionary<string, string> _publisherScripts = [];
    private static Lock _publisherScriptsLock = new();


    public IgExporter(
        IDbConnection db,
        ILoggerFactory loggerFactory,
        string outputPath,
        string? crossVersionSourcePath,
        XVerExporter exporter)

    {
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<IgExporter>();

        _db = db;

        _crossVersionSourcePath = crossVersionSourcePath;
        _outputPath = outputPath;
        if (!Directory.Exists(_outputPath))
        {
            Directory.CreateDirectory(_outputPath);
        }

        _exporter = exporter;
    }

    public XVerExportTrackingRecord CreateInitialXVerIgs(
        bool includeScripts = true,
        int? maxStepSize = null,
        HashSet<(FhirReleases.FhirSequenceCodes s, FhirReleases.FhirSequenceCodes t)>? specificPairs = null)
    {
        XVerExportTrackingRecord tr = new();

        // get the list of packages
        _packages = DbFhirPackage.SelectList(_db, orderByProperties: [nameof(DbFhirPackage.PackageVersion)]);

        List<FhirPackageComparisonPair> packagePairs = [];
        maxStepSize ??= _packages.Count - 1;

        // we want to process closer versions first, so we do a stepped approach
        for (int stepSize = 1; stepSize <= maxStepSize; stepSize++)
        {
            for (int i = 0; i < _packages.Count - stepSize; i++)
            {
                DbFhirPackage sourcePackage = _packages[i];
                DbFhirPackage targetPackage = _packages[i + stepSize];

                if ((specificPairs is null) ||
                    specificPairs.Contains((sourcePackage.DefinitionFhirSequence, targetPackage.DefinitionFhirSequence)))
                {
                    packagePairs.Add(new(sourcePackage, targetPackage));
                }


                if ((specificPairs is null) ||
                    specificPairs.Contains((targetPackage.DefinitionFhirSequence, sourcePackage.DefinitionFhirSequence)))
                {
                    packagePairs.Add(new(targetPackage, sourcePackage));
                }
            }
        }

        // iterate over our pairs in the order we built them
        foreach (FhirPackageComparisonPair packagePair in packagePairs)
        {
            tr.XVerIgs.Add(createInitialXVerIg(packagePair, includeScripts));
        }

        HashSet<FhirReleases.FhirSequenceCodes>? targetFhirVersions = null;
        if (specificPairs is not null)
        {
            targetFhirVersions = [];
            foreach ((FhirReleases.FhirSequenceCodes s, FhirReleases.FhirSequenceCodes t) in specificPairs)
            {
                targetFhirVersions.Add(t);
            }
        }

        // iterate over our packages for validation Igs
        foreach (DbFhirPackage package in _packages)
        {
            if ((targetFhirVersions is not null) &&
                !targetFhirVersions.Contains(package.DefinitionFhirSequence))
            {
                continue;
            }

            tr.ValidationIgs.Add(createInitialValidationIg(package, includeScripts));
        }

        return tr;
    }

    private static string getXVerPackageId(FhirPackageComparisonPair pair) =>
        $"hl7.fhir.uv.xver-{pair.SourcePackageShortName.ToLowerInvariant()}.{pair.TargetPackageShortName.ToLowerInvariant()}";

    private static string getValidationPackageId(DbFhirPackage package) =>
        $"hl7.fhir.uv.xver.{package.ShortName.ToLowerInvariant()}";

    private XVerIgExportTrackingRecord createInitialXVerIg(
        FhirPackageComparisonPair packagePair,
        bool includeScripts = true)
    {
        string packageId = getXVerPackageId(packagePair);

        DbFhirPackage sourcePackage = packagePair.SourcePackage;
        DbFhirPackage targetPackage = packagePair.TargetPackage;

        XVerIgExportTrackingRecord igTr = new()
        {
            PackageId = packageId,
            PackagePair = packagePair,
        };

        igTr.IgIndexFile ??= new()
        {
            FileName = $"ImplementationGuide-{packageId}.json",
            FileNameWithoutExtension = $"ImplementationGuide-{packageId}",
            IsPageContentFile = false,
            Name = FhirSanitizationUtils.ReformatIdForName(packageId),
            Id = packageId,
            Url = $"http://hl7.org/fhir/uv/xver/ImplementationGuide/{packageId}",
            ResourceType = "ImplementationGuide",
            Version = _exporter._crossDefinitionVersion,
            Description =
                $"FHIR Cross-Version Extensions package to access" +
                $" FHIR {targetPackage.DefinitionFhirSequence} artifacts while using" +
                $" FHIR {sourcePackage.DefinitionFhirSequence}",
        };

        createXVerIgDirectories(igTr);
        copyIgSourceContent(igTr.InputDir!);

        if (includeScripts &&
            (igTr.IgRootDir is not null))
        {
            writeScriptFiles(igTr.IgRootDir);
        }

        return igTr;
    }

    private ValidationIgExportTrackingRecord createInitialValidationIg(
        DbFhirPackage package,
        bool includeScripts = true)
    {
        string packageId = getValidationPackageId(package);

        ValidationIgExportTrackingRecord vTr = new()
        {
            PackageId = packageId,
            Package = package,
        };

        vTr.IgIndexFile ??= new()
        {
            FileName = $"ImplementationGuide-{packageId}.json",
            FileNameWithoutExtension = $"ImplementationGuide-{packageId}",
            IsPageContentFile = false,
            Name = FhirSanitizationUtils.ReformatIdForName(packageId),
            Id = packageId,
            Url = $"http://hl7.org/fhir/uv/xver/ImplementationGuide/{packageId}",
            ResourceType = "ImplementationGuide",
            Version = _exporter._crossDefinitionVersion,
            Description =
                $"FHIR Cross-Version Extensions validation package that contains the contents" +
                $" from all other FHIR versions to use in" +
                $" FHIR {package.DefinitionFhirSequence}",
        };

        createValidationIgDirectories(vTr);
        copyIgSourceContent(vTr.InputDir!);

        if (includeScripts &&
            (vTr.IgRootDir is not null))
        {
            writeScriptFiles(vTr.IgRootDir);
        }

        return vTr;
    }

    private void writeScriptFiles(string igRootDir)
    {
        Dictionary<string, string> scriptFiles = getCurrentPublisherScripts();
        _logger.LogInformation($"Writing {scriptFiles.Comparer} files from GitHub repository to {igRootDir}");

        foreach ((string filePath, string fileContent) in scriptFiles)
        {
            try
            {
                string fullPath = Path.Combine(igRootDir, filePath);
                string? directory = Path.GetDirectoryName(fullPath);

                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(fullPath, fileContent);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Failed to write GitHub file: {filePath}");
            }
        }
    }

    /// <summary>
    /// Determines whether a file should be skipped when copying from the GitHub repository.
    /// </summary>
    /// <param name="filePath">The relative file path from the repository root.</param>
    /// <returns>True if the file should be skipped, false otherwise.</returns>
    private static bool shouldSkipScriptRepoFile(string filePath)
    {
        // Skip hidden files and directories
        if (filePath.StartsWith(".") || filePath.Contains("/."))
        {
            return true;
        }

        // skip repository README
        if (filePath.Equals("README.md", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        // Skip common binary file extensions
        string[] skipExtensions = { ".exe", ".dll", ".bin", ".jar", ".zip", ".tar", ".gz", ".7z",
                                   ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".ico", ".svg",
                                   ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx" };

        string extension = Path.GetExtension(filePath).ToLowerInvariant();
        if (skipExtensions.Contains(extension))
        {
            return true;
        }

        // Skip node_modules and other common directories
        string[] skipDirectories = { "node_modules/", ".git/", ".vs/", ".vscode/", "bin/", "obj/" };
        foreach (string skipDir in skipDirectories)
        {
            if (filePath.Contains(skipDir))
            {
                return true;
            }
        }

        // Skip very large files (> 1MB) - these are likely not useful for IG publishing
        // Note: We can't check file size here since we only have the path, 
        // but we'll handle this in the fetch method

        return false;
    }


    /// <summary>
    /// Recursively gets all contents from a GitHub repository.
    /// </summary>
    private List<RepositoryContent> getRepositoryContentsRecursive(
        GitHubClient client,
        string owner,
        string repo,
        string? path = null)
    {
        List<RepositoryContent> allContents = [];

        try
        {
            IReadOnlyList<RepositoryContent> contents = path == null
                ? client.Repository.Content.GetAllContents(owner, repo).Result
                : client.Repository.Content.GetAllContents(owner, repo, path).Result;

            foreach (var content in contents)
            {
                if (content.Type == ContentType.File)
                {
                    allContents.Add(content);
                }
                else if (content.Type == ContentType.Dir && !shouldSkipScriptRepoFile(content.Path))
                {
                    // Recursively get directory contents
                    List<RepositoryContent> subContents = getRepositoryContentsRecursive(client, owner, repo, content.Path);
                    allContents.AddRange(subContents);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get repository contents for path: {Path}", path);
        }

        return allContents;
    }

    /// <summary>
    /// Fetches all files from the HL7 ig-publisher-scripts GitHub repository.
    /// </summary>
    /// <returns>A dictionary where keys are relative file paths and values are file contents.</returns>
    private Dictionary<string, string> getCurrentPublisherScripts()
    {
        const string owner = "HL7";
        const string repo = "ig-publisher-scripts";

        lock (_publisherScriptsLock)
        {
            if (_publisherScripts.Count > 0)
            {
                _logger.LogInformation("Using cached publisher scripts from GitHub repository: {Owner}/{Repo}", owner, repo);
                return _publisherScripts;
            }

            try
            {
                _logger.LogInformation("Fetching files from GitHub repository: {Owner}/{Repo}", owner, repo);

                GitHubClient client = new(new ProductHeaderValue("fhir-codegen"));

                // Get the repository contents recursively
                List<RepositoryContent> contents = getRepositoryContentsRecursive(client, owner, repo);

                foreach (RepositoryContent content in contents)
                {
                    if (content.Type == ContentType.File && !shouldSkipScriptRepoFile(content.Path))
                    {
                        try
                        {
                            // Skip files larger than 1MB
                            if (content.Size > 1024 * 1024)
                            {
                                _logger.LogDebug("Skipping large file: {Path} ({Size} bytes)", content.Path, content.Size);
                                continue;
                            }

                            // Get the file content
                            byte[] fileContent = client.Repository.Content.GetRawContent(owner, repo, content.Path).Result;
                            string contentString = System.Text.Encoding.UTF8.GetString(fileContent);

                            _publisherScripts[content.Path] = contentString;
                            _logger.LogDebug("Fetched file: {Path}", content.Path);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to fetch file content for: {Path}", content.Path);
                        }
                    }
                    else if (content.Type == ContentType.File)
                    {
                        _logger.LogDebug("Skipping file: {Path}", content.Path);
                    }
                }

                _logger.LogInformation("Successfully fetched {Count} files from GitHub repository", _publisherScripts.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch files from GitHub repository: {Owner}/{Repo}", owner, repo);
            }
        }

        return _publisherScripts;
    }


    private void copyIgSourceContent(string igInputDir)
    {
        // check for a source path we can copy from
        if (string.IsNullOrEmpty(_crossVersionSourcePath))
        {
            return;
        }

        if (!Directory.Exists(_crossVersionSourcePath))
        {
            _logger.LogWarning($"Cross-version IG source path '{_crossVersionSourcePath}' does not exist; skipping copy of IG source content");
            return;
        }

        string igSourceDir = Path.Combine(_crossVersionSourcePath, "input", "ig-source");
        if (!Directory.Exists(igSourceDir))
        {
            _logger.LogWarning($"Cross-version IG source content path '{igSourceDir}' does not exist; skipping copy of IG source content");
            return;
        }

        // copy the contents of this directory into the target input directory
        recursiveCopyContents(igSourceDir, igInputDir);
    }

    private void recursiveCopyContents(string sourceDir, string targetDir)
    {
        // copy all files in this directory
        foreach (string sourceFilePath in Directory.GetFiles(sourceDir))
        {
            string fileName = Path.GetFileName(sourceFilePath);
            string targetFilePath = Path.Combine(targetDir, fileName);
            File.Copy(sourceFilePath, targetFilePath, overwrite: true);
        }

        // recurse into sub-directories
        foreach (string sourceSubDir in Directory.GetDirectories(sourceDir))
        {
            string dirName = Path.GetFileName(sourceSubDir);
            string targetSubDir = Path.Combine(targetDir, dirName);
            if (!Directory.Exists(targetSubDir))
            {
                Directory.CreateDirectory(targetSubDir);
            }
            recursiveCopyContents(sourceSubDir, targetSubDir);
        }
    }

    private void createValidationIgDirectories(ValidationIgExportTrackingRecord vTr)
    {
        vTr.IgRootDir = Path.Combine(_outputPath, vTr.PackageId);
        if (!Directory.Exists(vTr.IgRootDir))
        {
            Directory.CreateDirectory(vTr.IgRootDir);
        }

        vTr.InputDir = Path.Combine(vTr.IgRootDir, "input");
        if (!Directory.Exists(vTr.InputDir))
        {
            Directory.CreateDirectory(vTr.InputDir);
        }

        vTr.IncludesDir = Path.Combine(vTr.InputDir, "includes");
        if (!Directory.Exists(vTr.IncludesDir))
        {
            Directory.CreateDirectory(vTr.IncludesDir);
        }

        vTr.PageContentDir = Path.Combine(vTr.InputDir, "pagecontent");
        if (!Directory.Exists(vTr.PageContentDir))
        {
            Directory.CreateDirectory(vTr.PageContentDir);
        }
    }

    private void createXVerIgDirectories(XVerIgExportTrackingRecord igTr)
    {
        igTr.IgRootDir = Path.Combine(_outputPath, igTr.PackageId);
        if (Directory.Exists(igTr.IgRootDir))
        {
            Directory.Delete(igTr.IgRootDir, recursive: true);
        }
        Directory.CreateDirectory(igTr.IgRootDir);

        igTr.InputDir = Path.Combine(igTr.IgRootDir, "input");
        if (!Directory.Exists(igTr.InputDir))
        {
            Directory.CreateDirectory(igTr.InputDir);
        }

        igTr.IncludesDir = Path.Combine(igTr.InputDir, "includes");
        if (!Directory.Exists(igTr.IncludesDir))
        {
            Directory.CreateDirectory(igTr.IncludesDir);
        }

        igTr.PageContentDir = Path.Combine(igTr.InputDir, "pagecontent");
        if (!Directory.Exists(igTr.PageContentDir))
        {
            Directory.CreateDirectory(igTr.PageContentDir);
        }

        igTr.VocabularyDir = Path.Combine(igTr.InputDir, "vocabulary");
        if (!Directory.Exists(igTr.VocabularyDir))
        {
            Directory.CreateDirectory(igTr.VocabularyDir);
        }

        igTr.VocabMapDir = Path.Combine(igTr.InputDir, "vocabularymaps");
        if (!Directory.Exists(igTr.VocabMapDir))
        {
            Directory.CreateDirectory(igTr.VocabMapDir);
        }

        igTr.ExtensionDir = Path.Combine(igTr.InputDir, "extensions");
        if (!Directory.Exists(igTr.ExtensionDir))
        {
            Directory.CreateDirectory(igTr.ExtensionDir);
        }

        igTr.ProfileDir = Path.Combine(igTr.InputDir, "profiles");
        if (!Directory.Exists(igTr.ProfileDir))
        {
            Directory.CreateDirectory(igTr.ProfileDir);
        }

        igTr.ResourceMapDir = Path.Combine(igTr.InputDir, "resourcemaps");
        if (!Directory.Exists(igTr.ResourceMapDir))
        {
            Directory.CreateDirectory(igTr.ResourceMapDir);
        }

        igTr.ElementMapDir = Path.Combine(igTr.InputDir, "elementmaps");
        if (!Directory.Exists(igTr.ElementMapDir))
        {
            Directory.CreateDirectory(igTr.ElementMapDir);
        }
    }
}
