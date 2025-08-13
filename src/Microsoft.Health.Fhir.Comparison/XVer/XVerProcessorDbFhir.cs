using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Specification.Snapshot;
using Hl7.Fhir.Utility;
using Hl7.FhirPath.Sprache;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.CodeGen.Configuration;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Utils;
using Microsoft.Health.Fhir.Comparison.CompareTool;
using Microsoft.Health.Fhir.Comparison.Extensions;
using Microsoft.Health.Fhir.Comparison.Models;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;
using Tasks = System.Threading.Tasks;

namespace Microsoft.Health.Fhir.Comparison.XVer;

public partial class XVerProcessor
{

    /// <summary>
    /// Represents index information for a cross-version FHIR package, including references to supporting structures and value sets.
    /// </summary>
    private class XverPackageIndexInfo
    {
        /// <summary>
        /// Gets or sets the source package support information.
        /// </summary>
        public required PackageXverSupport SourcePackageSupport { get; set; }

        /// <summary>
        /// Gets or sets the target package support information.
        /// </summary>
        public required PackageXverSupport TargetPackageSupport { get; set; }

        /// <summary>
        /// Gets or sets the unique package identifier for this cross-version package.
        /// </summary>
        public required string PackageId { get; set; }

        /// <summary>
        /// Gets or sets the list of JSON strings representing indexed structure definitions.
        /// </summary>
        public List<string> IndexStructureJsons { get; set; } = [];

        /// <summary>
        /// Gets or sets the list of JSON strings representing indexed value sets.
        /// </summary>
        public List<string> IndexValueSetJsons { get; set; } = [];

        /// <summary>
        /// Gets or sets the list of JSON strings for ImplementationGuide structure resources.
        /// </summary>
        public List<string> IgStructureJsons { get; set; } = [];

        /// <summary>
        /// Gets or sets the list of JSON strings for ImplementationGuide value set resources.
        /// </summary>
        public List<string> IgValueSetJsons { get; set; } = [];

        /// <summary>
        /// Gets or sets the list of ImplementationGuide structure resource components.
        /// </summary>
        public List<ImplementationGuide.ResourceComponent> IgStructures { get; set; } = [];

        /// <summary>
        /// Gets or sets the list of ImplementationGuide value set resource components.
        /// </summary>
        public List<ImplementationGuide.ResourceComponent> IgValueSets { get; set; } = [];
    }

    private static Dictionary<string, string> _publisherScripts = [];
    private static Lock _publisherScriptsLock = new();

    /// <summary>
    /// Generates cross-version FHIR artifacts from the loaded database, including ValueSets, StructureDefinitions, and ImplementationGuides.
    /// </summary>
    /// <param name="version">Optional artifact version to use; if null, uses the configured artifact version.</param>
    /// <param name="outputDir">Optional output directory; if null, uses the configured map source path.</param>
    /// <exception cref="Exception">
    /// Thrown if the database is not loaded or if the output directory is not specified.
    /// </exception>
    public void WriteFhirFromDatabase(string? version = null, string? outputDir = null)
    {
        // check for no database
        if (_db == null)
        {
            throw new Exception("Cannot generate FHIR artifacts without a loaded database!");
        }

        outputDir ??= _config.CrossVersionMapSourcePath;

        // check for no output location
        if (string.IsNullOrEmpty(outputDir))
        {
            throw new Exception("Cannot write FHIR artifacts without output or map source folder!");
        }

        string fhirDir = Path.Combine(outputDir, "fhir");
        if (Directory.Exists(fhirDir))
        {
            Directory.Delete(fhirDir, true);
        }

        Directory.CreateDirectory(fhirDir);

        if (string.IsNullOrEmpty(version))
        {
            _crossDefinitionVersion = _config.XverArtifactVersion;
        }
        else
        {
            _crossDefinitionVersion = version;
        }

        _logger.LogInformation($"Writing cross-version FHIR artifacts to {fhirDir} with version {_crossDefinitionVersion}");

        // grab the FHIR Packages we are processing
        List<DbFhirPackage> packages = DbFhirPackage.SelectList(_db.DbConnection, orderByProperties: [nameof(DbFhirPackage.ShortName)]);
        List<DbFhirPackageComparisonPair> packageComparisonPairs = DbFhirPackageComparisonPair.SelectList(
            _db.DbConnection,
            orderByProperties: [nameof(DbFhirPackageComparisonPair.SourcePackageKey), nameof(DbFhirPackageComparisonPair.TargetPackageKey)]);

        ConcurrentDictionary<int, string> differentialVsBySourceKey = [];

        Dictionary<int, HashSet<string>> basicElementPathsByPackageKey = [];
        Dictionary<(int, string), List<List<XverOutcome>>> xverOutcomes = [];

        List<PackageXverSupport> packageSupports = [];

        List<XverPackageIndexInfo> allIndexInfos = [];

        // iterate over the packages to build the Basic resource element paths
        foreach ((DbFhirPackage package, int index) in packages.Select((p, i) => (p, i)))
        {
            // need to create a definition collection with the matching core package so that we can build everything
            string packageDirective = $"{package.PackageId}#{package.PackageVersion}";

            // create a loader because these are all different FHIR core versions
            using CodeGen.Loader.PackageLoader loader = new(_config, new()
            {
                JsonModel = CodeGen.Loader.LoaderOptions.JsonDeserializationModel.SystemTextJson,
            });

            //// need to load the CodeSystems from each package
            //DefinitionCollection codeSystemCollection = loader.LoadPackages([packageDirective], loadFilterOverride: [FhirArtifactClassEnum.CodeSystem]).Result
            //    ?? throw new Exception($"Failed to load CodeSystems for export of package: {packageDirective}");

            //DefinitionCollection coreDc = loader.LoadPackages([packageDirective]).Result
            //    ?? throw new Exception($"Could not load package: {packageDirective}");

            PackageXverSupport packageSupport = new()
            {
                PackageIndex = index,
                Package = package,
                BasicElements = [],
                //CodeSystemDc = codeSystemCollection,
                //CoreDC = coreDc,
                //SnapshotGenerator = new(coreDc),
            };

            packageSupports.Add(packageSupport);

            // check for a basic structure
            DbStructureDefinition? basicResource = DbStructureDefinition.SelectSingle(
                _db.DbConnection,
                FhirPackageKey: package.Key,
                Name: "Basic",
                ArtifactClass: FhirArtifactClassEnum.Resource);

            if (basicResource != null)
            {
                // get the elements for this structure
                List<DbElement> basicElements = DbElement.SelectList(
                    _db.DbConnection,
                    StructureKey: basicResource.Key);

                // iterate over the elements
                foreach (DbElement element in basicElements)
                {
                    // skip root and elements with empty paths
                    if ((element.ResourceFieldOrder == 0) ||
                        string.IsNullOrEmpty(element.Path))
                    {
                        continue;
                    }

                    // add the path to the dictionary, but strip "Basic" from the front
                    packageSupport.BasicElements.Add(element.Path.Substring(5));
                }
            }

            // check for an extension structure
            DbStructureDefinition? extensionStructure = DbStructureDefinition.SelectSingle(
                _db.DbConnection,
                FhirPackageKey: package.Key,
                Name: "Extension",
                ArtifactClass: FhirArtifactClassEnum.ComplexType);

            if (extensionStructure != null)
            {
                // check for the value[x] element
                DbElement? extValueElement = DbElement.SelectSingle(
                    _db.DbConnection,
                    FhirPackageKey: package.Key,
                    StructureKey: extensionStructure.Key,
                    Id: "Extension.value[x]");

                if (extValueElement != null)
                {
                    // get the types for this element
                    List<DbElementType> extValueTypes = DbElementType.SelectList(
                        _db.DbConnection,
                        ElementKey: extValueElement.Key);

                    // iterate over the types
                    foreach (DbElementType extValueType in extValueTypes)
                    {
                        if (!string.IsNullOrEmpty(extValueType.TypeName))
                        {
                            packageSupport.AllowedExtensionTypes.Add(extValueType.TypeName);
                        }
                    }
                }
            }
        }

        // iterate over the list of packages
        for (int focusPackageIndex = 0; focusPackageIndex < packages.Count; focusPackageIndex++)
        {
            // ignore DSTU2 for now
            //if (packageSupports[focusPackageIndex].Package.DefinitionFhirSequence == FhirReleases.FhirSequenceCodes.DSTU2)
            //{
            //    continue;
            //}

            //if (focusPackageIndex != packages.Count - 1)
            //{
            //    continue;
            //}

            _logger.LogInformation($"Processing package {focusPackageIndex + 1} of {packages.Count}: {packages[focusPackageIndex].ShortName}");

            Dictionary<(int sourceVsKey, int targetPackageId), ValueSet> xverValueSets = buildXverValueSets(packages, focusPackageIndex);

            writeXverValueSets(packages, focusPackageIndex, xverValueSets, fhirDir);

            Dictionary<(int sourceElementKey, int targetPackageId), (StructureDefinition, DbExtensionSubstitution?)> xverExtensions = [];
            Dictionary<(int sourceStructureKey, int targetPackageId), StructureDefinition> xverProfiles = [];

            // build the structure graph so that we can resolve targets in references
            //Dictionary<int, DbGraphSd> sdGraphs = [];
            //addStructureGraphs(packages[focusPackageIndex].Key, FhirArtifactClassEnum.ComplexType, packages, sdGraphs);
            //addStructureGraphs(packages[focusPackageIndex].Key, FhirArtifactClassEnum.Resource, packages, sdGraphs);

            buildXverStructures(packageSupports, focusPackageIndex, xverValueSets, xverExtensions, xverProfiles, xverOutcomes, FhirArtifactClassEnum.ComplexType);
            buildXverStructures(packageSupports, focusPackageIndex, xverValueSets, xverExtensions, xverProfiles, xverOutcomes, FhirArtifactClassEnum.Resource);

            writeXverStructures(packageSupports, focusPackageIndex, xverExtensions, xverProfiles, fhirDir);

            if (!_config.XverExportForPublisher)
            {
                List<XverPackageIndexInfo> focusedIndexInfos = writeXverSinglePackageSupportFiles(
                    packageSupports,
                    focusPackageIndex,
                    xverValueSets,
                    xverExtensions,
                    fhirDir);
                allIndexInfos.AddRange(focusedIndexInfos);
            }

            // 2025.08.12 - need to copy CodeSystems directly - serializing/parsing is problematic for portions
#if !XVER_CS_DISABLED
            // write the source code systems for this package
            writeXverSourceCodeSystemsFromDb(packages, focusPackageIndex, fhirDir);
#else
            // copy the source code systems into the target package directories
            writeXverSourceCodeSystems(packageSupports, focusPackageIndex, fhirDir);
#endif
        }

        // write all of our outcome lists
        writeXverOutcomes(
            packageSupports,
            xverOutcomes,
            outputDir,
            out Dictionary<string, List<(string structureName, string filename)>> packageMdList);

        // write our combined package support files
        if (_config.XverExportForPublisher)
        {
            // write the individual package support files
            for (int focusPackageIndex = 0; focusPackageIndex < packages.Count; focusPackageIndex++)
            {
                // write the publisher config files
                writePublisherSinglePackageConfig(packageSupports, focusPackageIndex, fhirDir, packageMdList);
            }

            // write the validation package support files
            writePublisherValidationPackageConfig(packageSupports, allIndexInfos, fhirDir, packageMdList);
        }
        else
        {
            writeXverValidationPackageSupportFiles(packageSupports, allIndexInfos, fhirDir);
        }

        if ((_config.XverExportForPublisher == false) &&
            (_config.XverGenerateNpms == true))
        {
            // make the make package tgz files
            foreach (DbFhirPackage focusPackage in packages)
            {
                // TODO: until verified, only write R4 and later packages
                if ((focusPackage.ShortName == "R2") ||
                    (focusPackage.ShortName == "R3"))
                {
                    continue;
                }

                string validationPackageId = $"hl7.fhir.uv.xver.{focusPackage.ShortName.ToLowerInvariant()}";

                // create the validation package
                createTgzFromDirectory(
                    Path.Combine(fhirDir, focusPackage.ShortName),
                    Path.Combine(fhirDir, $"{validationPackageId}.{_crossDefinitionVersion}.tgz"));

                // look for all combination packages, using the focus as the target
                foreach (DbFhirPackage sourcePackage in packages)
                {
                    if (sourcePackage.Key == focusPackage.Key)
                    {
                        continue;
                    }

                    string packageId = $"hl7.fhir.uv.xver-{sourcePackage.ShortName.ToLowerInvariant()}.{focusPackage.ShortName.ToLowerInvariant()}";

                    // create the validation package
                    createTgzFromDirectory(
                        Path.Combine(fhirDir, $"{sourcePackage.ShortName}-for-{focusPackage.ShortName}"),
                        Path.Combine(fhirDir, $"{packageId}.{_crossDefinitionVersion}.tgz"));
                }
            }
        }
    }

    private void addStructureGraphs(
        int fhirPackageKey,
        FhirArtifactClassEnum artifactClass,
        List<DbFhirPackage> packages,
        Dictionary<int, DbGraphSd> sdGraphs)
    {
        List<DbStructureDefinition> structures = DbStructureDefinition.SelectList(
            _db!.DbConnection,
            FhirPackageKey: fhirPackageKey,
            ArtifactClass: artifactClass);

        foreach (DbStructureDefinition sd in structures)
        {
            if (sdGraphs.ContainsKey(sd.Key))
            {
                continue;
            }

            // build a graph for this structure
            DbGraphSd sdGraph = new()
            {
                DB = _db!.DbConnection,
                Packages = packages,
                KeySd = sd,
            };

            sdGraphs.Add(sd.Key, sdGraph);
        }
    }

    private string? getLocalPackageDirectory(string directive)
    {
        if (string.IsNullOrEmpty(directive))
        {
            return null;
        }

        // check to see if we think this is a directory
        if ((directive.IndexOfAny(Path.GetInvalidPathChars()) == -1) &&
            (directive.Contains('/') || directive.Contains('\\') || directive.Contains('~')))
        {
            // check to see if we can find the directory
            string resolvedDir = CodeGen.Utils.FileSystemUtils.FindRelativeDir(".", directive, false);
            if (!string.IsNullOrEmpty(resolvedDir))
            {
                return resolvedDir;
            }
        }

        if (directive.Contains('@'))
        {
            directive = directive.Replace('@', '#');
        }

        string fhirPackageDir = _config.FhirCacheDirectory ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".fhir", "packages");

        string packageDir = Path.Combine(fhirPackageDir, directive, "package");

        if (Directory.Exists(packageDir))
        {
            return packageDir;
        }

        return null;
    }


    private void writeXverSourceCodeSystems(
        List<PackageXverSupport> packageSupports,
        int focusPackageIndex,
        string fhirDir)
    {
        DbFhirPackage focusPackage = packageSupports[focusPackageIndex].Package;

        // iterate over the target packages
        foreach (PackageXverSupport targetSupport in packageSupports)
        {
            if (focusPackage.Key == targetSupport.Package.Key)
            {
                continue;
            }

            DbFhirPackage targetPackage = targetSupport.Package;

            // make sure we have a code system collection for the target package
            //DefinitionCollection dc = targetSupport.CodeSystemDc ?? targetSupport.CoreDC ?? 

            // try to resolve the source directory
            string? sourceDir = getLocalPackageDirectory(focusPackage.CacheFolderName);

            if (sourceDir == null)
            {
                // nothing to do
                continue;
            }

            string dir = createExportPackageDir(fhirDir, focusPackage, targetPackage);
            dir = _config.XverExportForPublisher
                ? Path.Combine(dir, "input", "vocabulary")
                : Path.Combine(dir, "package");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            // traverse the source directory code systems
            foreach (string sourceFilename in Directory.EnumerateFiles(sourceDir, "CodeSystem-*.json", SearchOption.TopDirectoryOnly))
            {
                // build our target filename
                string targetFilename = Path.Combine(dir, Path.GetFileName(sourceFilename));

                if (File.Exists(targetFilename))
                {
                    // already exists, skip
                    continue;
                }

                // copy the file to the target directory
                try
                {
                    File.Copy(sourceFilename, targetFilename);
                    _logger.LogInformation($"Copied source CodeSystem file {sourceFilename} to {targetFilename}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to copy source CodeSystem file {sourceFilename} to {targetFilename}");
                }
            }
        }
    }


    // 2025.08.12 - need to copy CodeSystems directly - serializing/parsing is problematic for portions
#if !XVER_CS_DISABLED
    private void writeXverSourceCodeSystemsFromDb(
        List<DbFhirPackage> packages,
        int focusPackageIndex,
        string fhirDir)
    {
        DbFhirPackage focusPackage = packages[focusPackageIndex];

        // iterate over the target packages
        foreach (DbFhirPackage targetPackage in packages)
        {
            if (focusPackage.Key == targetPackage.Key)
            {
                continue;
            }

            string dir = createExportPackageDir(fhirDir, focusPackage, targetPackage);
            dir = _config.XverExportForPublisher
                ? Path.Combine(dir, "input", "vocabulary")
                : Path.Combine(dir, "package");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }


            // get the list of code systems in the source package
            List<DbCodeSystem> codeSystems = DbCodeSystem.SelectList(
                _db!.DbConnection,
                FhirPackageKey: focusPackage.Key);

            // iterate over the code systems to them
            foreach (DbCodeSystem dbCs in codeSystems)
            {
                // create the FHIR CodeSystem
                CodeSystem fhirCs = new()
                {
                    Id = dbCs.Id,
                    Url = dbCs.UnversionedUrl,
                    Name = dbCs.Name,
                    Version = dbCs.Version,
                    VersionAlgorithm =
                        (dbCs.VersionAlgorithmString != null)
                        ? new FhirString(dbCs.VersionAlgorithmString)
                        : (dbCs.VersionAlgorithmCoding != null)
                        ? dbCs.VersionAlgorithmCoding
                        : null,
                    Status = dbCs.Status,
                    Title = dbCs.Title,
                    Description = dbCs.Description,
                    Purpose = dbCs.Purpose,
                    Text = dbCs.Narrative,
                    Experimental = dbCs.IsExperimental,
                    DateElement = (dbCs.LastChangedDate != null) ? new FhirDateTime(dbCs.LastChangedDate.Value) : null,
                    Publisher = dbCs.Publisher,
                    Copyright = dbCs.Copyright,
                    CopyrightLabel = dbCs.CopyrightLabel,
                    ApprovalDate = dbCs.ApprovalDate,
                    LastReviewDate = dbCs.LastReviewDate,
                    EffectivePeriod = (dbCs.EffectivePeriodStart != null || dbCs.EffectivePeriodEnd != null)
                        ? new Period()
                        {
                            StartElement = (dbCs.EffectivePeriodStart != null) ? new FhirDateTime(dbCs.EffectivePeriodStart.Value) : null,
                            EndElement = (dbCs.EffectivePeriodEnd != null) ? new FhirDateTime(dbCs.EffectivePeriodEnd.Value) : null,
                        }
                        : null,
                    Topic = dbCs.Topic,
                    RelatedArtifact = dbCs.RelatedArtifacts,
                    Jurisdiction = dbCs.Jurisdictions,
                    UseContext = dbCs.UseContexts,
                    Contact = dbCs.Contacts,
                    Author = dbCs.Authors,
                    Editor = dbCs.Editors,
                    Reviewer = dbCs.Reviewers,
                    CaseSensitive = dbCs.IsCaseSensitive,
                    ValueSet = dbCs.ValueSetVersioned,
                    HierarchyMeaning = dbCs.HierarchyMeaning,
                    Compositional = dbCs.IsCompositional,
                    VersionNeeded = dbCs.VersionNeeded,
                    Content = dbCs.Content,
                    Supplements = dbCs.SupplementsVersioned,
                    Count = dbCs.Count,
                };

                // add standard extensions
                if (dbCs.RootExtensions != null)
                {
                    foreach (Hl7.Fhir.Model.Extension ext in dbCs.RootExtensions)
                    {
                        fhirCs.Extension.Add(ext);
                    }
                }

                // add filters
                List<DbCodeSystemFilter> csFilters = DbCodeSystemFilter.SelectList(
                    _db!.DbConnection,
                    CodeSystemKey: dbCs.Key);

                foreach (DbCodeSystemFilter dbFilter in csFilters)
                {
                    fhirCs.Filter.Add(new CodeSystem.FilterComponent()
                    {
                        Code = dbFilter.Code,
                        Description = dbFilter.Description,
                        Operator = dbFilter.Operators.Split('|').Select(op => EnumUtility.ParseLiteral<FilterOperator>(op, true)).ToList(),
                        Value = dbFilter.Value,
                    });
                }

                // add property definitions
                List<DbCodeSystemPropertyDefinition> csPropertyDefinitions = DbCodeSystemPropertyDefinition.SelectList(
                    _db!.DbConnection,
                    CodeSystemKey: dbCs.Key);

                foreach (DbCodeSystemPropertyDefinition dbPropDef in csPropertyDefinitions)
                {
                    fhirCs.Property.Add(new CodeSystem.PropertyComponent()
                    {
                        Code = dbPropDef.Code,
                        Uri = dbPropDef.Uri,
                        Description = dbPropDef.Description,
                        Type = dbPropDef.Type,
                    });
                }

                // recursively add concepts
                addDbCodeSystemConcepts(fhirCs.Concept, dbCs.Key);

                // write the code system to a file
                string filename = $"CodeSystem-{fhirCs.Id}.json";
                string path = Path.Combine(dir, filename);
                File.WriteAllText(path, fhirCs.ToJson(new FhirJsonSerializationSettings() { Pretty = true }));
            }
        }
    }

    private void addDbCodeSystemConcepts(
        List<CodeSystem.ConceptDefinitionComponent> concepts,
        int dbCsKey,
        int? parentConceptKey = null)
    {
        List<DbCodeSystemConcept> dbConcepts = (parentConceptKey == null)
            ? DbCodeSystemConcept.SelectList(
                _db!.DbConnection,
                CodeSystemKey: dbCsKey,
                ParentConceptKeyIsNull: true,
                orderByProperties: [nameof(DbCodeSystemConcept.FlatOrder)])
            : DbCodeSystemConcept.SelectList(
                _db!.DbConnection,
                CodeSystemKey: dbCsKey,
                ParentConceptKey: parentConceptKey.Value,
                orderByProperties: [nameof(DbCodeSystemConcept.FlatOrder)]);

        foreach (DbCodeSystemConcept dbConcept in dbConcepts)
        {
            // create the concept
            CodeSystem.ConceptDefinitionComponent fhirConcept = new CodeSystem.ConceptDefinitionComponent()
            {
                Code = dbConcept.Code,
                Display = dbConcept.Display,
                Definition = dbConcept.Definition,
                Designation = dbConcept.Designations,
                Property = dbConcept.Properties,
            };

            concepts.Add(fhirConcept);

            // recursively add child concepts
            if (dbConcept.ChildConceptCount != 0)
            {
                addDbCodeSystemConcepts(fhirConcept.Concept, dbCsKey, dbConcept.Key);
            }
        }
    }
#endif

    private static string getPackageId(DbFhirPackage? sourcePackage, DbFhirPackage targetPackage) => sourcePackage == null
        ? $"hl7.fhir.uv.xver.{targetPackage.ShortName.ToLowerInvariant()}"
        : $"hl7.fhir.uv.xver-{sourcePackage.ShortName.ToLowerInvariant()}.{targetPackage.ShortName.ToLowerInvariant()}";

    private void writeXverValueSets(
        List<DbFhirPackage> packages,
        int focusPackageIndex,
        Dictionary<(int sourceVsKey, int targetPackageId), ValueSet> xverValueSets,
        string fhirDir)
    {
        Dictionary<int, DbFhirPackage> packageDict = packages.ToDictionary(p => p.Key);
        DbFhirPackage focusPackage = packages[focusPackageIndex];

        // iterate over the value sets
        foreach (((int sourceVsKey, int targetPackageId), ValueSet vs) in xverValueSets)
        {
            DbFhirPackage targetPackage = packageDict[targetPackageId];

            string dir = createExportPackageDir(fhirDir, focusPackage, targetPackage);

            dir = _config.XverExportForPublisher
                ? Path.Combine(dir, "input", "vocabulary")
                : Path.Combine(dir, "package");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            // write the value set to a file
            string filename = $"ValueSet-{vs.Id}.json";
            string path = Path.Combine(dir, filename);
            File.WriteAllText(path, vs.ToJson(new FhirJsonSerializationSettings() { Pretty = true }));
        }
    }

    private void writeXverStructures(
        List<PackageXverSupport> packageSupports,
        int focusPackageIndex,
        Dictionary<(int sourceElementKey, int targetPackageId), (StructureDefinition, DbExtensionSubstitution?)> xverExtensions,
        Dictionary<(int sourceStructureKey, int targetPackageId), StructureDefinition> xverProfiles,
        string fhirDir)
    {
        Dictionary<int, DbFhirPackage> packageDict = packageSupports.Select(ps => ps.Package).ToDictionary(p => p.Key);
        DbFhirPackage focusPackage = packageSupports[focusPackageIndex].Package;

        ILookup<int, SnapshotGenerator?> generatorsById = packageSupports.ToLookup(ps => ps.Package.Key, ps => ps.SnapshotGenerator);

        Dictionary<int, (string packageId, string packageDir)> packageWriteSupport = [];
        foreach (PackageXverSupport ps in packageSupports)
        {
            if (ps.PackageIndex == focusPackageIndex)
            {
                continue;
            }

            string packageId = getPackageId(focusPackage, ps.Package);
            string dir = createExportPackageDir(fhirDir, focusPackage, ps.Package);

            packageWriteSupport.Add(ps.Package.Key, (packageId, dir));
        }

        // iterate over the extensions
        foreach (((int sourceKey, int targetPackageId), (StructureDefinition sd, DbExtensionSubstitution? extensionSubstitution)) in xverExtensions)
        {
            // if this extension is substituted out, we do not need to write it
            if (extensionSubstitution != null)
            {
                continue;
            }

            if (_config.XverGenerateSnapshots)
            {
                try
                {
                    if (sd.Snapshot == null)
                    {
                        // create a new snapshot
                        sd.Snapshot = new StructureDefinition.SnapshotComponent();
                    }

                    // a valid snapshot will always have at least the root element
                    if (sd.Snapshot.Element.Count == 0)
                    {
                        //sd.Snapshot.Element = packageSupports[targetPackageId].SnapshotGenerator?.GenerateAsync(sd).Result ?? [];
                        sd.Snapshot.Element = generatorsById[targetPackageId]?.FirstOrDefault()?.GenerateAsync(sd).Result ?? [];
                    }
                }
                catch (Exception) { }
            }

            DbFhirPackage targetPackage = packageDict[targetPackageId];
            (string packageId, string dir) = packageWriteSupport[targetPackageId];

            dir = _config.XverExportForPublisher
                ? Path.Combine(dir, "input", "extensions")
                : Path.Combine(dir, "package");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            // write the structure to a file
            string filename = $"StructureDefinition-{sd.Id}.json";

            string path = Path.Combine(dir, filename);
            File.WriteAllText(path, sd.ToJson(new FhirJsonSerializationSettings() { Pretty = true }));
        }

        // iterate over the profiles
        foreach (((int sourceStructureKey, int targetPackageId), StructureDefinition sd) in xverProfiles)
        {
            if (_config.XverGenerateSnapshots)
            {
                try
                {
                    if (sd.Snapshot == null)
                    {
                        // create a new snapshot
                        sd.Snapshot = new StructureDefinition.SnapshotComponent();
                    }

                    // a valid snapshot will always have at least the root element
                    if (sd.Snapshot.Element.Count == 0)
                    {
                        //sd.Snapshot.Element = packageSupports[targetPackageId].SnapshotGenerator?.GenerateAsync(sd).Result ?? [];
                        sd.Snapshot.Element = generatorsById[targetPackageId]?.FirstOrDefault()?.GenerateAsync(sd).Result ?? [];
                    }
                }
                catch (Exception) { }
            }

            DbFhirPackage targetPackage = packageDict[targetPackageId];
            (string packageId, string dir) = packageWriteSupport[targetPackageId];

            dir = _config.XverExportForPublisher
                ? Path.Combine(dir, "input", "profiles")
                : Path.Combine(dir, "package");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            // write the structure to a file
            string filename = $"StructureDefinition-{sd.Id}.json";

            string path = Path.Combine(dir, filename);
            File.WriteAllText(path, sd.ToJson(new FhirJsonSerializationSettings() { Pretty = true }));
        }
    }


    private void buildXverStructures(
        List<PackageXverSupport> packageSupports,
        int sourcePackageIndex,
        Dictionary<(int sourceVsKey, int targetPackageId), ValueSet> xverValueSets,
        Dictionary<(int sourceElementKey, int targetPackageId), (StructureDefinition, DbExtensionSubstitution?)> xverExtensions,
        Dictionary<(int sourceStructureKey, int targetPackageId), StructureDefinition> xverProfiles,
        Dictionary<(int, string), List<List<XverOutcome>>> xverOutcomes,
        FhirArtifactClassEnum artifactClass)
    {
        DbFhirPackage sourcePackage = packageSupports[sourcePackageIndex].Package;

        // resolve the extension types for this version of FHIR
        HashSet<string> allowedExtensionTypes = getAllowedExtensionTypes(sourcePackage.Key);

        // get the list of structures in this version
        List<DbStructureDefinition> structures = DbStructureDefinition.SelectList(
                _db!.DbConnection,
                FhirPackageKey: sourcePackage.Key,
                ArtifactClass: artifactClass);

        // iterate over the structures
        foreach (DbStructureDefinition sd in structures)
        {
            if (!xverOutcomes.ContainsKey((sourcePackageIndex, sd.Name)))
            {
                xverOutcomes[(sourcePackageIndex, sd.Name)] = [];
                for (int i = 0; i < packageSupports.Count; i++)
                {
                    xverOutcomes[(sourcePackageIndex, sd.Name)].Add([]);
                }
            }

            // build a graph for this structure
            DbGraphSd sdGraph = new()
            {
                DB = _db!.DbConnection,
                Packages = packageSupports.Select(ps => ps.Package).ToList(),
                KeySd = sd,
            };

            // build a dictionary of all element projections, by element key
            Dictionary<int, List<DbGraphSd.DbElementRow>> elementProjectionDict = [];
            foreach (DbGraphSd.DbSdRow sdRow in sdGraph.Projection)
            {
                foreach (DbGraphSd.DbElementRow sdElementRow in sdRow.Projection)
                {
                    if (sdElementRow.KeyCell == null)
                    {
                        continue;
                    }

                    if (!elementProjectionDict.TryGetValue(sdElementRow.KeyCell.Element.Key, out List<DbGraphSd.DbElementRow>? elementList))
                    {
                        elementList = [];
                        elementProjectionDict.Add(sdElementRow.KeyCell.Element.Key, elementList);
                    }

                    elementList.Add(sdElementRow);
                }
            }

            List<HashSet<int>> generatedElementKeys = [];
            for (int i = 0; i < packageSupports.Count; i++)
            {
                generatedElementKeys.Add([]);
            }

            List<bool> structureMapsToBasic = [];
            for (int i = 0; i < packageSupports.Count; i++)
            {
                if (sdGraph.Projection.Any(sdRow => sdRow[i] != null))
                {
                    structureMapsToBasic.Add(false);
                    continue;
                }

                structureMapsToBasic.Add(true);
            }

            // iterate over the elements of our structure
            foreach (DbElement element in DbElement.SelectList(_db!.DbConnection, StructureKey: sd.Key, orderByProperties: [nameof(DbElement.ResourceFieldOrder)]))
            {
                // do not build extensions for simple-type elements (e.g., Element.id, Extension.url, etc.)
                if (element.IsSimpleType == true)
                {
                    continue;
                }

                // do not build extensions for extension or base elements
                switch (element.FullCollatedTypeLiteral)
                {
                    case "Extension":
                    case "Base":
                        continue;
                }

                // resolve the projection rows for this element
                List<DbGraphSd.DbElementRow> elementProjection = elementProjectionDict[element.Key];

                bool extensionNeeded = false;

                // work upwards first
                for (int currentIndex = sourcePackageIndex; currentIndex < (packageSupports.Count - 1); currentIndex++)
                {
                    int targetPackageIndex = currentIndex + 1;
                    DbFhirPackage targetPackage = packageSupports[targetPackageIndex].Package;

                    // if we already generated the parent of this element, flag it as added and move on
                    if ((element.ParentElementKey != null) &&
                        generatedElementKeys[targetPackageIndex].Contains(element.ParentElementKey.Value))
                    {
                        // add this element to the generated list
                        generatedElementKeys[targetPackageIndex].Add(element.Key);
                        xverOutcomes[(sourcePackageIndex, sd.Name)][targetPackageIndex].Add(new()
                        {
                            SourcePackageKey = sourcePackage.Key,
                            SourceStructureName = sd.Name,
                            SourceElementId = element.Id,
                            SourceElementFieldOrder = element.ResourceFieldOrder,
                            TargetPackageKey = targetPackage.Key,
                            OutcomeCode = XverOutcomeCodes.UseExtensionFromAncestor,
                            TargetElementId = null,
                            TargetExtensionUrl = null,
                            ReplacementExtensionUrl = null,
                        });
                        continue;
                    }

                    // do not generate if this element is part of a mapped structure and has an equivalent in the target's basic resource definition
                    if (structureMapsToBasic[targetPackageIndex] &&
                        (element.ParentElementKey != null) &&
                        packageSupports[targetPackageIndex].BasicElements.Contains(element.Path.Substring(sd.Name.Length)))
                    {
                        xverOutcomes[(sourcePackageIndex, sd.Name)][targetPackageIndex].Add(new()
                        {
                            SourcePackageKey = sourcePackage.Key,
                            SourceStructureName = sd.Name,
                            SourceElementId = element.Id,
                            SourceElementFieldOrder = element.ResourceFieldOrder,
                            TargetPackageKey = targetPackage.Key,
                            OutcomeCode = XverOutcomeCodes.UseBasicElement,
                            TargetElementId = null,
                            TargetExtensionUrl = null,
                            ReplacementExtensionUrl = null,
                        });
                        continue;
                    }

                    List<DbElementComparison> comparisons = [];

                    // resolve the current column
                    List<DbGraphSd.DbElementCell?> sourceCells = elementProjection
                        .Select(row => row[currentIndex])
                        .ToList();

                    // if we have not hit a need for the extension yet in this direction, test the curent pair
                    if (!extensionNeeded)
                    {
                        // check to see if this element has already been mapped in the previous version
                        if ((currentIndex > sourcePackageIndex) &&
                            generatedElementKeys[currentIndex - 1].Contains(element.Key))
                        {
                            extensionNeeded = true;
                        }
                        // only generate entire structures if there is no mappable structure in the target
                        else if (element.ResourceFieldOrder == 0)
                        {
                            extensionNeeded = structureMapsToBasic[targetPackageIndex];
                        }
                        // if we have no mappings, we need a new extension
                        else if (sourceCells.Count == 0)
                        {
                            extensionNeeded = true;
                        }
                        // if all cells or right projections are null, we need an extension
                        else if (sourceCells.All(cell => cell?.RightCell == null))
                        {
                            extensionNeeded = true;
                        }
                        // need to check aggregate relationship
                        else
                        {
                            // easier to check inverse here
                            extensionNeeded = true;

                            List<DbGraphSd.DbElementCell> matchedCells = sourceCells
                                .Where(c => (c?.RightComparison?.Relationship == CMR.Equivalent) || (c?.RightComparison?.Relationship == CMR.SourceIsNarrowerThanTarget))
                                .Select(c => c!)
                                .ToList();

                            if (matchedCells.Count != 0)
                            {
                                extensionNeeded = false;
                                XverOutcomeCodes oc = matchedCells.Count > 1
                                    ? XverOutcomeCodes.UseOneOfElements
                                    : matchedCells[0].RightCell?.Element.Id == element.Id
                                        ? XverOutcomeCodes.UseElementSameName
                                        : XverOutcomeCodes.UseElementRenamed;

                                xverOutcomes[(sourcePackageIndex, sd.Name)][targetPackageIndex].Add(new()
                                {
                                    SourcePackageKey = sourcePackage.Key,
                                    SourceStructureName = sd.Name,
                                    SourceElementId = element.Id,
                                    SourceElementFieldOrder = element.ResourceFieldOrder,
                                    TargetPackageKey = targetPackage.Key,
                                    OutcomeCode = oc,
                                    TargetElementId = string.Join(',', matchedCells.Select(c => c.RightCell?.Element.Id)),
                                    TargetExtensionUrl = null,
                                    ReplacementExtensionUrl = null,
                                });
                            }
                        }
                    }

                    // if we still do not need an extension, go to next package
                    if (!extensionNeeded)
                    {
                        continue;
                    }

                    // check to see if we have already generated this extension
                    if (xverExtensions.ContainsKey((element.Key, targetPackage.Key)))
                    {
                        continue;
                    }

                    foreach (DbGraphSd.DbElementCell? cell in sourceCells)
                    {
                        if (cell?.RightComparison == null)
                        {
                            continue;
                        }

                        comparisons.Add(cell.RightComparison);
                    }

                    // build an extension for the original source element to target the current target version
                    StructureDefinition? extSd = createExtensionSd(
                        sourcePackageIndex,
                        packageSupports[sourcePackageIndex],
                        targetPackageIndex,
                        packageSupports[targetPackageIndex],
                        sd,
                        element,
                        comparisons,
                        elementProjectionDict,
                        xverValueSets);

                    if (extSd != null)
                    {
                        DbExtensionSubstitution? extSub = DbExtensionSubstitution.SelectSingle(_db!.DbConnection, SourceElementId: element.Id);

                        xverExtensions.Add((element.Key, packageSupports[targetPackageIndex].Package.Key), (extSd, extSub));
                        generatedElementKeys[targetPackageIndex].Add(element.Key);

                        xverOutcomes[(sourcePackageIndex, sd.Name)][targetPackageIndex].Add(new()
                        {
                            SourcePackageKey = sourcePackage.Key,
                            SourceStructureName = sd.Name,
                            SourceElementId = element.Id,
                            SourceElementFieldOrder = element.ResourceFieldOrder,
                            TargetPackageKey = targetPackage.Key,
                            OutcomeCode = XverOutcomeCodes.UseExtension,
                            TargetElementId = null,
                            TargetExtensionUrl = extSd.Url,
                            ReplacementExtensionUrl = extSub?.ReplacementUrl
                        });

                        // check to see if this extension maps to the root of the Basic resource
                        if (extSd.Context.Any(c => c.Expression == "Basic"))
                        {
                            // need to create a profile for this extension
                            StructureDefinition profileSd = createBasicProfileForExtension(sourcePackage, targetPackage, sd, extSd);
                            xverProfiles.Add((sd.Key, targetPackage.Key), profileSd);
                        }
                    }
                }

                extensionNeeded = false;

                // then work downwards
                for (int currentIndex = sourcePackageIndex; currentIndex > 0; currentIndex--)
                {
                    int targetPackageIndex = currentIndex - 1;
                    DbFhirPackage targetPackage = packageSupports[targetPackageIndex].Package;

                    // if we already generated the parent of this element, flag it as added and move on
                    if ((element.ParentElementKey != null) &&
                        generatedElementKeys[targetPackageIndex].Contains(element.ParentElementKey.Value))
                    {
                        // add this element to the generated list
                        generatedElementKeys[targetPackageIndex].Add(element.Key);
                        xverOutcomes[(sourcePackageIndex, sd.Name)][targetPackageIndex].Add(new()
                        {
                            SourcePackageKey = sourcePackage.Key,
                            SourceStructureName = sd.Name,
                            SourceElementId = element.Id,
                            SourceElementFieldOrder = element.ResourceFieldOrder,
                            TargetPackageKey = targetPackage.Key,
                            OutcomeCode = XverOutcomeCodes.UseExtensionFromAncestor,
                            TargetElementId = null,
                            TargetExtensionUrl = null,
                            ReplacementExtensionUrl = null,
                        });
                        continue;
                    }

                    // do not generate if this element is equivalent in the target basic resource
                    if (structureMapsToBasic[targetPackageIndex] &&
                        (element.ParentElementKey != null) &&
                        packageSupports[targetPackageIndex].BasicElements.Contains(element.Path.Substring(sd.Name.Length)))
                    {
                        xverOutcomes[(sourcePackageIndex, sd.Name)][targetPackageIndex].Add(new()
                        {
                            SourcePackageKey = sourcePackage.Key,
                            SourceStructureName = sd.Name,
                            SourceElementId = element.Id,
                            SourceElementFieldOrder = element.ResourceFieldOrder,
                            TargetPackageKey = targetPackage.Key,
                            OutcomeCode = XverOutcomeCodes.UseBasicElement,
                            TargetElementId = null,
                            TargetExtensionUrl = null,
                            ReplacementExtensionUrl = null,
                        });
                        continue;
                    }

                    List<DbElementComparison> comparisons = [];

                    // resolve the current column
                    List<DbGraphSd.DbElementCell?> sourceCells = elementProjection
                        .Select(row => row[currentIndex])
                        .ToList();

                    // if we have not hit a need for the extension yet in this direction, test the curent pair
                    if (!extensionNeeded)
                    {
                        // check to see if this element has been mapped in the previous version
                        if ((currentIndex < sourcePackageIndex) &&
                            generatedElementKeys[currentIndex + 1].Contains(element.Key))
                        {
                            extensionNeeded = true;
                        }
                        // only generate entire structures if there is no mappable structure in the target
                        else if (element.ResourceFieldOrder == 0)
                        {
                            extensionNeeded = structureMapsToBasic[targetPackageIndex];
                        }
                        // if we have no mappings, we need a new extension
                        else if (sourceCells.Count == 0)
                        {
                            extensionNeeded = true;
                        }
                        // if all cells or left projections are null, we need an extension
                        else if (sourceCells.All(cell => cell?.LeftComparison == null))
                        {
                            extensionNeeded = true;
                        }
                        // need to check aggregate relationship
                        else
                        {
                            // easier to check inverse here
                            extensionNeeded = true;

                            List<DbGraphSd.DbElementCell> matchedCells = sourceCells
                                .Where(c => (c?.LeftComparison?.Relationship == CMR.Equivalent) || (c?.LeftComparison?.Relationship == CMR.SourceIsNarrowerThanTarget))
                                .Select(c => c!)
                                .ToList();

                            if (matchedCells.Count != 0)
                            {
                                extensionNeeded = false;
                                XverOutcomeCodes oc = matchedCells.Count > 1
                                    ? XverOutcomeCodes.UseOneOfElements
                                    : matchedCells[0].LeftCell?.Element.Id == element.Id
                                        ? XverOutcomeCodes.UseElementSameName
                                        : XverOutcomeCodes.UseElementRenamed;

                                xverOutcomes[(sourcePackageIndex, sd.Name)][targetPackageIndex].Add(new()
                                {
                                    SourcePackageKey = sourcePackage.Key,
                                    SourceStructureName = sd.Name,
                                    SourceElementId = element.Id,
                                    SourceElementFieldOrder = element.ResourceFieldOrder,
                                    TargetPackageKey = targetPackage.Key,
                                    OutcomeCode = oc,
                                    TargetElementId = string.Join(',', matchedCells.Select(c => c.LeftCell?.Element.Id)),
                                    TargetExtensionUrl = null,
                                    ReplacementExtensionUrl = null,
                                });
                            }
                        }
                    }

                    // if we still do not need an extension, go to next package
                    if (!extensionNeeded)
                    {
                        continue;
                    }

                    // check to see if we have already generated this extension
                    if (xverExtensions.ContainsKey((element.Key, targetPackage.Key)))
                    {
                        continue;
                    }

                    foreach (DbGraphSd.DbElementCell? cell in sourceCells)
                    {
                        if (cell?.LeftComparison == null)
                        {
                            continue;
                        }

                        comparisons.Add(cell.LeftComparison);
                    }

                    // build an extension for the original source element to target the current target version
                    StructureDefinition? extSd = createExtensionSd(
                        sourcePackageIndex,
                        packageSupports[sourcePackageIndex],
                        targetPackageIndex,
                        packageSupports[targetPackageIndex],
                        sd,
                        element,
                        comparisons,
                        elementProjectionDict,
                        xverValueSets);

                    if (extSd != null)
                    {
                        DbExtensionSubstitution? extSub = DbExtensionSubstitution.SelectSingle(_db!.DbConnection, SourceElementId: element.Id);

                        xverExtensions.Add((element.Key, targetPackage.Key), (extSd, extSub));
                        generatedElementKeys[targetPackageIndex].Add(element.Key);

                        xverOutcomes[(sourcePackageIndex, sd.Name)][targetPackageIndex].Add(new()
                        {
                            SourcePackageKey = sourcePackage.Key,
                            SourceStructureName = sd.Name,
                            SourceElementId = element.Id,
                            SourceElementFieldOrder = element.ResourceFieldOrder,
                            TargetPackageKey = targetPackage.Key,
                            OutcomeCode = XverOutcomeCodes.UseExtension,
                            TargetElementId = null,
                            TargetExtensionUrl = extSd.Url,
                            ReplacementExtensionUrl = extSub?.ReplacementUrl,
                        });

                        // check to see if this extension maps to the root of the Basic resource
                        if (extSd.Context.Any(c => c.Expression == "Basic"))
                        {
                            // need to create a profile for this extension
                            StructureDefinition profileSd = createBasicProfileForExtension(sourcePackage, targetPackage, sd, extSd);
                            xverProfiles.Add((sd.Key, targetPackage.Key), profileSd);
                        }
                    }
                }
            }
        }

        return;

        HashSet<string> getAllowedExtensionTypes(int packageKey)
        {
            // resolve the 'extension' structure definition
            DbStructureDefinition? extSd = DbStructureDefinition.SelectSingle(
                _db!.DbConnection,
                FhirPackageKey: packageKey,
                Name: "Extension");

            if (extSd == null)
            {
                return [];
            }

            // get the 'value[x]' element
            DbElement? extValueElement = DbElement.SelectSingle(
                _db!.DbConnection,
                StructureKey: extSd.Key,
                Id: "Extension.value[x]");

            if (extValueElement == null)
            {
                return [];
            }

            // get the types allowed in the Extension.value element
            List<DbElementType> extValueTypes = DbElementType.SelectList(
                _db!.DbConnection,
                ElementKey: extValueElement.Key);

            return new HashSet<string>(extValueTypes.Select(et => et.TypeName!));
        }
    }

    private StructureDefinition createBasicProfileForExtension(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        DbStructureDefinition sourceStructure,
        StructureDefinition extSd)
    {
        // TODO: add profiles for other resource types
        string targetStructureName = "Basic";       // extSd.Context.First().Expression;

        string profileId = $"{sourcePackage.ShortName}-{sourceStructure.Name}-for-{targetPackage.ShortName}";
        StructureDefinition profileSd = new()
        {
            Id = profileId,
            Url = $"http://hl7.org/fhir/{targetPackage.FhirVersionShort}/StructureDefinition/{profileId}",
            Name = FhirSanitizationUtils.ReformatIdForName(profileId),
            Version = _crossDefinitionVersion,
            FhirVersion = EnumUtility.ParseLiteral<FHIRVersion>(targetPackage.PackageVersion) ?? FHIRVersion.N5_0_0,
            DateElement = new FhirDateTime(DateTimeOffset.Now),
            Title = $"Cross-version Profile for {sourcePackage.ShortName}.{sourceStructure.Name} for use in FHIR {targetPackage.ShortName}",
            Description = $"This cross-version profile on the {targetPackage.ShortName}.{targetStructureName} resource can be used to represent a FHIR {sourcePackage.ShortName}.{sourceStructure.Name} resource.",
            Status = PublicationStatus.Active,
            Experimental = false,
            Kind = StructureDefinition.StructureDefinitionKind.Resource,
            Abstract = false,
            Type = targetStructureName,
            BaseDefinition = $"http://hl7.org/fhir/StructureDefinition/{targetStructureName}",
            Derivation = StructureDefinition.TypeDerivationRule.Constraint,
            Differential = new()
            {
                Element = [
                    new ElementDefinition()
                    {
                        ElementId = "Basic.extension",
                        Path = "Basic.extension",
                        Slicing = new()
                        {
                            Discriminator = [
                                new ElementDefinition.DiscriminatorComponent()
                                {
                                    Type = ElementDefinition.DiscriminatorType.Value,
                                    Path = "url",
                                }
                            ],
                            Ordered = false,
                            Rules = ElementDefinition.SlicingRules.Open,
                        },
                        Base = new ElementDefinition.BaseComponent()
                        {
                            Path = "DomainResource.extension",
                            Min = 0,
                            Max = "*",
                        },
                        Min = 1,
                        Max = "*",
                    },
                    new ElementDefinition()
                    {
                        ElementId = $"Basic.extension:{sourceStructure.Id}",
                        Path = "Basic.extension",
                        SliceName = sourceStructure.Id,
                        Short = $"Cross-version extension for {sourceStructure.Name} from {sourcePackage.ShortName} for use in FHIR {targetPackage.ShortName}",
                        Min = 1,
                        Max = "1",
                        Base = new ElementDefinition.BaseComponent()
                        {
                            Path = "DomainResource.extension",
                            Min = 0,
                            Max = "*",
                        },
                        Type = [
                            new ElementDefinition.TypeRefComponent()
                            {
                                Code = "Extension",
                                Profile = [ extSd.Url ],
                            },
                        ],
                    },
                    new ElementDefinition()
                    {
                        ElementId = "Basic.code",
                        Path = "Basic.code",
                        Pattern = new CodeableConcept("http://hl7.org/fhir/fhir-types", sourceStructure.Id),
                        Base = new ElementDefinition.BaseComponent()
                        {
                            Path = "Basic.code",
                            Min = 1,
                            Max = "*",
                        }
                    },
                ],
            },
        };



        return profileSd;
    }

    private StructureDefinition? createExtensionSd(
        int sourceIndex,
        PackageXverSupport sourcePackageSupport,
        int targetIndex,
        PackageXverSupport targetPackageSupport,
        DbStructureDefinition sd,
        DbElement element,
        List<DbElementComparison> relevantComparisons,
        Dictionary<int, List<DbGraphSd.DbElementRow>> elementProjectionDict,
        Dictionary<(int sourceVsKey, int targetPackageId), ValueSet> xverValueSets)
    {
        DbFhirPackage sourcePackage = sourcePackageSupport.Package;
        DbFhirPackage targetPackage = targetPackageSupport.Package;

        //string sdId = $"{focusPackage.ShortName}-{element.Path}-for-{targetPackage.ShortName}";
        string sdId = $"ext-{sourcePackage.ShortName}-{collapsePathForId(element.Path)}";

        bool isRootElement = element.ResourceFieldOrder == 0;
        int elementPathLen = element.Path.Length;

        bool isExtensionOnBasic = false;
        List<DbElement> contextElements = [];
        List<StructureDefinition.ContextComponent> contexts = [];

        // if our source element is a resource or datatype, we can only apply it to the basic resource
        if (isRootElement)
        {
            contexts.Add(new()
            {
                Type = StructureDefinition.ExtensionContextType.Element,
                Expression = "Basic",
            });

            DbElement? basicElement = DbElement.SelectSingle(
                _db!.DbConnection,
                FhirPackageKey: targetPackage.Key,
                Id: "Basic");

            if (basicElement != null)
            {
                // add the basic element to the context elements
                contextElements.Add(basicElement);
                isExtensionOnBasic = true;
            }
        }
        else
        {
            HashSet<string> contextElementPaths = [];

            contextElements = discoverContexts(contextElementPaths, sourceIndex, targetIndex, targetPackageSupport, element, elementProjectionDict);

            if (contextElementPaths.Count != 0)
            {
                foreach (string path in contextElementPaths.Distinct().Order())
                {
                    contexts.Add(new()
                    {
                        Type = StructureDefinition.ExtensionContextType.Element,
                        Expression = path,
                    });
                }
            }
        }

        // fallback to element if we have no contexts
        if (contexts.Count == 0)
        {
            contexts.Add(new()
            {
                Type = StructureDefinition.ExtensionContextType.Element,
                Expression = "Element",
            });
        }

        StructureDefinition extSd = new()
        {
            Id = sdId,
            //Url = $"http://hl7.org/fhir/uv/xver/{focusPackage.FhirVersionShort}/StructureDefinition/extension-{element.Path.Replace("[x]", string.Empty)}",
            Url = $"http://hl7.org/fhir/{sourcePackage.FhirVersionShort}/StructureDefinition/extension-{element.Path.Replace("[x]", string.Empty)}",
            Name = FhirSanitizationUtils.ReformatIdForName(sdId),
            Version = _crossDefinitionVersion,
            FhirVersion = EnumUtility.ParseLiteral<FHIRVersion>(targetPackageSupport!.Package.PackageVersion) ?? FHIRVersion.N5_0_0,
            DateElement = new FhirDateTime(DateTimeOffset.Now),
            Title = $"Cross-version Extension for {sourcePackage.ShortName}.{element.Path} for use in FHIR {targetPackage.ShortName}",
            Description = $"This cross-version extension represents {element.Path} from {sd.VersionedUrl} for use in FHIR {targetPackage.ShortName}.",
            Status = PublicationStatus.Active,
            Experimental = false,
            Kind = StructureDefinition.StructureDefinitionKind.ComplexType,
            Abstract = false,
            Context = contexts,
            Type = "Extension",
            BaseDefinition = "http://hl7.org/fhir/StructureDefinition/Extension",
            Derivation = StructureDefinition.TypeDerivationRule.Constraint,
            Differential = new()
            {
                Element = [],
            },
        };

        Dictionary<int, string> extPathByElementKey = [];

        // add this element to the structure, including the child elements
        ExtElementBuilderRecord? extRec = addElementToExtension(
            extSd,
            "Extension",
            "Extension",
            null,
            sourcePackageSupport,
            targetPackageSupport,
            sd,
            element,
            relevantComparisons,
            xverValueSets,
            contextElements,
            isExtensionOnBasic);

        if (extRec == null)
        {
            // we should not be in a scenario where we do not have any properties
            throw new Exception($"Extension build for {extSd.Id} failed!");
        }

        addExtRecToSd(extSd, extRec, true);

        return extSd;
    }

    private void addExtRecToSd(StructureDefinition sd, ExtElementBuilderRecord extRec, bool addRootElement = false)
    {
        if (addRootElement)
        {
            ElementDefinition rootElement = new()
            {
                ElementId = extRec.ElementId,
                Path = extRec.Path,
                Short = extRec.ShortText,
                Definition = extRec.Definition,
                Comment = extRec.Comment,
                Min = extRec.SourceElement.MinCardinality,
                Max = extRec.SourceElement.MaxCardinalityString,
                Base = new()
                {
                    Path = "Extension",
                    Min = 0,
                    Max = "*",
                },
                IsModifier = extRec.SourceElement.IsModifier,
                IsModifierReason = extRec.SourceElement.IsModifierReason
                    ?? (extRec.SourceElement.IsModifier == true ? $"This extension is a modifier because the target element {extRec.SourceElement.Id} is flagged IsModifier" : null),
            };

            sd.Differential.Element.Add(rootElement);
        }
        else if (extRec.SliceName != null)
        {
            // add the primary slice
            ElementDefinition sliceElement = new()
            {
                ElementId = extRec.ElementId,
                Path = extRec.Path,
                SliceName = extRec.SliceName,
                Short = extRec.ShortText,
                Definition = extRec.Definition,
                Comment = extRec.Comment,
                Min = extRec.SourceElement.MinCardinality,
                Max = extRec.SourceElement.MaxCardinalityString,
                Base = new()
                {
                    Path = "Extension.extension",
                    Min = 0,
                    Max = "*",
                }
            };

            sd.Differential.Element.Add(sliceElement);
        }

        bool hasDatatypeExtension = (extRec.DatatypeSliceElement != null) && (extRec.DatatypeValueElement != null);

        // if there are any extensions, we need to build the slicing element
        if ((extRec.Extensions.Count > 0) ||
            hasDatatypeExtension)
        {
            sd.Differential.Element.Add(new()
            {
                ElementId = extRec.ElementId + ".extension",
                Path = extRec.Path + ".extension",
                Base = new()
                {
                    Path = "Extension.extension",
                    Min = 0,
                    Max = "*",
                },
                Slicing = new()
                {
                    Discriminator = [
                        new() {
                            Type = ElementDefinition.DiscriminatorType.Value,
                            Path = "url",
                        }
                    ],
                    Ordered = false,
                    Rules = ElementDefinition.SlicingRules.Closed,
                },
                Min = extRec.Extensions.Sum(ebr => ebr.SourceElement.MinCardinality),
                Max = "*",
            });
        }

        // recursively add any extensions
        foreach (ExtElementBuilderRecord child in extRec.Extensions)
        {
            addExtRecToSd(sd, child);
        }

        // if we have a datatype slice and value, we need to add them
        if (hasDatatypeExtension)
        {
            sd.Differential.Element.Add(extRec.DatatypeSliceElement);
            sd.Differential.Element.Add(extRec.DatatypeValueElement);
        }

        // add the URL element (always required)
        sd.Differential.Element.Add(new()
        {
            ElementId = extRec.ElementId + ".url",
            Path = extRec.Path + ".url",
            Base = new()
            {
                Path = "Extension.url",
                Min = 1,
                Max = "1",
            },
            Min = 1,
            Max = "1",
            Fixed = new FhirUri(extRec.Url),
        });

        // if there is a value element, we need to add it
        if (extRec.ValueElement != null)
        {
            sd.Differential.Element.Add(extRec.ValueElement);
        }
    }

    private record class ExtElementBuilderRecord
    {
        public required DbElement SourceElement { get; set; }
        public required string? UserMessages { get; set; }
        public required string ShortText { get; set; }
        public required string Definition { get; set; }
        public required string? Comment { get; set; }
        public required string Url { get; set; }
        public required string ElementId { get; set; }
        public required string Path { get; set; }
        public required string? SliceName { get; set; }
        public ElementDefinition? ValueElement { get; set; } = null;
        public List<ExtElementBuilderRecord> Extensions { get; set; } = [];
        public ElementDefinition? DatatypeSliceElement { get; set; } = null;
        public ElementDefinition? DatatypeValueElement { get; set; } = null;
        public List<string> ExtendedDatatypeNames = [];
    }


    private ExtElementBuilderRecord? addElementToExtension(
        StructureDefinition extSd,
        string extElementId,
        string extElementPath,
        string? sliceName,
        PackageXverSupport sourcePackageSupport,
        PackageXverSupport targetPackageSupport,
        DbStructureDefinition sd,
        DbElement element,
        List<DbElementComparison> relevantComparisons,
        Dictionary<(int sourceVsKey, int targetPackageId), ValueSet> xverValueSets,
        List<DbElement>? contextElements,
        bool isExtensionOnBasic)
    {
        // do not build extensions for extension or base elements
        switch (element.FullCollatedTypeLiteral)
        {
            case "Extension":
            case "Base":
                return null;
        }

        // skip id elements, they are part of every element and do not need to be written
        if (extElementId.EndsWith(".extension:id", StringComparison.Ordinal))
        {
            return null;
        }

        // check to see if this element is in the 'basic' resource of this version (do not add)
        if ((isExtensionOnBasic == true) &&
            (sliceName != null) &&
            (element.Path.Length > sd.Name.Length) &&
            targetPackageSupport.BasicElements.Contains(element.Path.Substring(sd.Name.Length)))
        {
            return null;
        }

        (string? edShortText, string? edDefinition, string? edComment) = getTextForExtensionElement(
            element,
            relevantComparisons.Count == 0 ? null : string.Join(' ', relevantComparisons.Select(c => c.UserMessage ?? string.Empty)));

        ExtElementBuilderRecord extBuilderRec = new()
        {
            SourceElement = element,
            UserMessages = relevantComparisons.Count == 0 ? null : string.Join(' ', relevantComparisons.Select(c => c.UserMessage ?? string.Empty)),
            ShortText = edShortText ?? $"Cross-version extension for {element.Name} from {sd.VersionedUrl} for use in FHIR {targetPackageSupport.Package.ShortName}",
            Definition = edDefinition ?? $"This cross-version extension represents {element.Path} from {sd.VersionedUrl} for use in FHIR {targetPackageSupport.Package.ShortName}.",
            Comment = edComment,
            Url = sliceName == null ? extSd.Url : sliceName,
            ElementId = extElementId,
            Path = extElementPath,
            SliceName = sliceName,
        };

        int sourceCol = sourcePackageSupport.PackageIndex;
        int targetCol = targetPackageSupport.PackageIndex;

        // if there are child elements, process them
        if (element.ChildElementCount != 0)
        {
            // iterate over our child elements and add them
            foreach (DbElement childElement in DbElement.SelectList(
                _db!.DbConnection,
                ParentElementKey: element.Key,
                orderByProperties: [nameof(DbElement.ResourceFieldOrder)]))
            {
                // add this child element to the extension
                ExtElementBuilderRecord? child = addElementToExtension(
                    extSd,
                    $"{extElementId}.extension:{childElement.Name}",
                    $"{extElementPath}.extension",
                    childElement.Name,
                    sourcePackageSupport,
                    targetPackageSupport,
                    sd,
                    childElement,
                    relevantComparisons,
                    xverValueSets,
                    null,
                    isExtensionOnBasic);

                if (child != null)
                {
                    extBuilderRec.Extensions.Add(child);
                }
            }

            // don't have to do anything past processing children
            return extBuilderRec;
        }

        ElementDefinition extensionEdValue = new()
        {
            ElementId = extElementId + ".value[x]",
            Path = extElementPath + ".value[x]",
            Short = extBuilderRec.ShortText,
            Definition = extBuilderRec.Definition,
            Comment = extBuilderRec.Comment,
            Base = new()
            {
                Path = "Extension.value[x]",
                Min = 0,
                Max = "1",
            },
            Type = [],
        };

        // check to see if we need to add a binding
        if ((element.ValueSetBindingStrength != null) &&
            (element.BindingValueSet != null))
        {
            string? vsUrl = null;

            if ((element.BindingValueSetKey != null) &&
                xverValueSets.TryGetValue((element.BindingValueSetKey.Value, targetPackageSupport.Package.Key), out ValueSet? vs))
            {
                vsUrl = vs.Url;
            }
            else
            {
                List<(string unversionedUrl, string version)> mappedUrls = _db!.DbConnection.GetMappedValueSetUrls(
                    sourcePackageSupport.Package.Key,
                    element.BindingValueSet,
                    targetPackageSupport.Package.Key);

                if (mappedUrls.Count == 0)
                {
                    // try to get a matching VS
                    DbValueSet? targetVs = DbValueSet.SelectSingle(
                        _db!.DbConnection,
                        FhirPackageKey: targetPackageSupport.Package.Key,
                        UnversionedUrl: element.BindingValueSet);

                    targetVs ??= DbValueSet.SelectSingle(
                        _db!.DbConnection,
                        FhirPackageKey: targetPackageSupport.Package.Key,
                        VersionedUrl: element.BindingValueSet);

                    if (targetVs == null)
                    {
                        DbValueSet? sourceVs = DbValueSet.SelectSingle(
                            _db!.DbConnection,
                            Key: element.BindingValueSetKey);

                        if (sourceVs != null)
                        {
                            targetVs = DbValueSet.SelectSingle(
                                _db!.DbConnection,
                                FhirPackageKey: targetPackageSupport.Package.Key,
                                Id: sourceVs.Id);
                        }
                    }

                    // if we have a target VS, use it
                    if (targetVs != null)
                    {
                        vsUrl = targetVs.UnversionedUrl + "|" + targetVs.Version;
                    }
                    // if this is an example binding, just leave it unbound
                    else if (element.ValueSetBindingStrength == BindingStrength.Extensible)
                    {
                        vsUrl = null;
                    }
                    else
                    {
                        // TODO: use the original binding value set URL - it is likely unexpandable
                        // Note that this will cause publisher warnings, but we do not have a strategy for resolving yet
                        vsUrl = element.BindingValueSet;
                    }
                }
                else
                {
                    vsUrl = mappedUrls[0].unversionedUrl + "|" + mappedUrls[0].version;
                }
            }

            if (vsUrl != null)
            {
                extensionEdValue.Binding = new()
                {
                    Strength = element.ValueSetBindingStrength,
                    Description = element.BindingDescription,
                    ValueSet = vsUrl,
                };
            }
        }

        // build the value types
        List<DbElementType> elementValueTypes = DbElementType.SelectList(
            _db!.DbConnection,
            ElementKey: element.Key);

        // setup a lookup by type name
        ILookup<string, DbElementType> collectedValueTypes = elementValueTypes.ToLookup(t => t.TypeName ?? string.Empty);
        List<string> extAllowedTypes = [];
        Dictionary<string, List<string>> extReplaceableTypes = [];
        List<string> extMappedTypes = [];

        HashSet<string> quantityProfilesMovedToTypes = [];

        // categorize the types based on how we process them
        foreach (string valueTypeName in elementValueTypes.Select(t => t.TypeName ?? string.Empty).Distinct())
        {
            if (targetPackageSupport.AllowedExtensionTypes.Contains(valueTypeName))
            {
                // check for this being the "Quantity" type to do special type profile handling
                if (valueTypeName == "Quantity")
                {
                    List<string> typeProfiles = collectedValueTypes[valueTypeName].Select(t => t.TypeProfile).Where(t => t != null)!.ToList<string>();

                    if (typeProfiles.Count > 0)
                    {
                        // check the profiled types
                        foreach (string typeProfile in typeProfiles)
                        {
                            string tpShort = typeProfile.Split('/')[^1];
                            if (targetPackageSupport.AllowedExtensionTypes.Contains(tpShort))
                            {
                                quantityProfilesMovedToTypes.Add(tpShort);
                                extAllowedTypes.Add(tpShort);
                            }
                        }

                        // skip this quantity if it was only this type
                        if (typeProfiles.Count == quantityProfilesMovedToTypes.Count)
                        {
                            continue;
                        }
                    }
                }

                extAllowedTypes.Add(valueTypeName);
                continue;
            }

            if (FhirTypeMappings.PrimitiveTypeFallbacks.TryGetValue(valueTypeName, out string? replacementType))
            {
                if (!extReplaceableTypes.TryGetValue(replacementType, out List<string>? replaceableTypes))
                {
                    replaceableTypes = [];
                    extReplaceableTypes.Add(replacementType, replaceableTypes);
                }
                extReplaceableTypes[replacementType].Add(valueTypeName);
                continue;
            }

            extMappedTypes.Add(valueTypeName);
        }

        HashSet<string> usedTypes = [];

        // process mapped types (extension before value)
        foreach (string typeName in extMappedTypes)
        {
            addDatatypeExtension(
                extSd,
                element,
                sourcePackageSupport,
                ref extBuilderRec,
                extElementId,           //extElementId + ".value[x].extension",
                extElementPath,         //extElementPath + ".value[x].extension",
                typeName);

            // resolve this structure
            DbStructureDefinition? etSd = DbStructureDefinition.SelectSingle(
                _db!.DbConnection,
                FhirPackageKey: sourcePackageSupport.Package.Key,
                Name: typeName);

            if (etSd == null)
            {
                continue;
            }

            // get the root element of the structure
            DbElement etRootElement = DbElement.SelectSingle(_db!.DbConnection, StructureKey: etSd.Key, ResourceFieldOrder: 0)
                ?? throw new Exception($"Failed to resolve the root element of {etSd.Name} ({etSd.Key})");

            // get the child elements for the structure
            List<DbElement> etElements = DbElement.SelectList(
                _db!.DbConnection,
                StructureKey: etSd.Key,
                ParentElementKey: etRootElement.Key,
                orderByProperties: [nameof(DbElement.ResourceFieldOrder)]);

            // iterate over the elements to add them to the extension
            foreach (DbElement etElement in etElements)
            {
                ExtElementBuilderRecord? childRec = addElementToExtension(
                    extSd,
                    $"{extElementId}.extension:{etElement.Name}",
                    $"{extElementPath}.extension",
                    etElement.Name,
                    sourcePackageSupport,
                    targetPackageSupport,
                    sd,
                    etElement,
                    relevantComparisons,
                    xverValueSets,
                    null,
                    isExtensionOnBasic);

                if (childRec != null)
                {
                    extBuilderRec.Extensions.Add(childRec);
                }
            }
        }

        // process replaced quantity types
        foreach (string typeName in quantityProfilesMovedToTypes)
        {
            if (usedTypes.Contains(typeName))
            {
                continue;
            }

            // add the value element if we are supposed to
            extBuilderRec.ValueElement = extensionEdValue;

            // consolidate profiles
            List<string> typeProfiles = collectedValueTypes.Contains(typeName) ? collectedValueTypes[typeName].Select(t => t.TypeProfile).Where(t => t != null)!.ToList<string>() : [];
            List<string> targetProfiles = collectedValueTypes.Contains(typeName) ? collectedValueTypes[typeName].Select(t => t.TargetProfile).Where(t => t != null)!.ToList<string>() : [];

            // create a new type reference
            ElementDefinition.TypeRefComponent? edValueType = new()
            {
                Code = typeName,
                ProfileElement = typeProfiles.Select(v => new Canonical(v)).ToList(),
                TargetProfileElement = targetProfiles.Select(v => new Canonical(v)).ToList(),
            };

            extensionEdValue.Type.Add(edValueType);

            // check to see if we use the type to contain data from another type too (need extensions)
            if (extReplaceableTypes.TryGetValue(typeName, out List<string>? replaceableTypes))
            {
                // add each of the replaceable types
                foreach (string rt in replaceableTypes)
                {
                    addDatatypeExtension(
                        extSd,
                        element,
                        sourcePackageSupport,
                        ref extBuilderRec,
                        extElementId,           //extElementId + ".value[x].extension",
                        extElementPath,         //extElementPath + ".value[x].extension",
                        rt);
                }
            }

            usedTypes.Add(typeName);
        }

        HashSet<string> contextReferenceTargets = [];
        if (contextElements != null)
        {
            // add the context elements to the extension
            foreach (DbElement contextElement in contextElements)
            {
                // get any reference element types for this element
                List<DbElementType> referenceTypes = DbElementType.SelectList(
                    _db!.DbConnection,
                    ElementKey: contextElement.Key,
                    TypeName: "Reference");

                foreach (DbElementType rt in referenceTypes)
                {
                    // if we have a target profile, add it to the extension
                    if (!string.IsNullOrEmpty(rt.TargetProfile))
                    {
                        contextReferenceTargets.Add(rt.TargetProfile);
                    }
                }
            }
        }

        // process allowed and replaceable types
        foreach (string typeName in extAllowedTypes)
        {
            if (usedTypes.Contains(typeName))
            {
                continue;
            }

            // add the value element if we are supposed to
            extBuilderRec.ValueElement = extensionEdValue;

            // consolidate profiles
            List<string> typeProfiles = collectedValueTypes[typeName].Select(t => t.TypeProfile).Where(t => t != null)!.ToList<string>();
            HashSet<string> targetProfiles = [];   //  collectedValueTypes[typeName].Select(t => t.TargetProfile).Where(t => t != null)!.ToList<string>();

            // build our target profiles - if the target resource is available, we can use that, otherwise use the profile we are creating
            foreach (string? tp in collectedValueTypes[typeName].Select(t => t.TargetProfile))
            {
                if (tp == null)
                {
                    continue;
                }

                // get the mapped structure URLs for the target package
                List<string> mappedUrls = _db!.DbConnection.GetMappedStructureUrls(sourcePackageSupport.Package.Key, tp, targetPackageSupport.Package.Key);

                foreach (string unversionedUrl in mappedUrls)
                {
                    // only add targets that do not exist on the context reference targets
                    if (contextReferenceTargets.Contains(unversionedUrl))
                    {
                        continue;
                    }

                    targetProfiles.Add(unversionedUrl);
                }
            }

            // remove any quantity type profiles that got promoted
            if ((typeName == "Quantity") &&
                (typeProfiles.Count > 0) &&
                (quantityProfilesMovedToTypes.Count > 0))
            {
                List<string> toRemove = typeProfiles.Where(tp => quantityProfilesMovedToTypes.Contains(tp.Split('/')[^1])).ToList();
                foreach (string tr in toRemove)
                {
                    typeProfiles.Remove(tr);
                }
            }

            // create a new type reference
            ElementDefinition.TypeRefComponent? edValueType = new()
            {
                Code = typeName,
                ProfileElement = typeProfiles.Select(v => new Canonical(v)).ToList(),
                TargetProfileElement = targetProfiles.Select(v => new Canonical(v)).ToList(),
            };

            extensionEdValue.Type.Add(edValueType);

            // check to see if we use the type to contain data from another type too (need extensions)
            if (extReplaceableTypes.TryGetValue(typeName, out List<string>? replaceableTypes))
            {
                // add each of the replaceable types
                foreach (string rt in replaceableTypes)
                {
                    addDatatypeExtension(
                        extSd,
                        element,
                        sourcePackageSupport,
                        ref extBuilderRec,
                        extElementId,           //extElementId + ".value[x].extension",
                        extElementPath,         //extElementPath + ".value[x].extension",
                        rt);
                }
            }

            usedTypes.Add(typeName);
        }

        // check for any missed replaceable types
        foreach ((string typeName, List<string> replaceableTypes) in extReplaceableTypes)
        {
            if (usedTypes.Contains(typeName))
            {
                continue;
            }

            // add the value element if we are supposed to
            extBuilderRec.ValueElement = extensionEdValue;

            // create a new type reference
            ElementDefinition.TypeRefComponent? edValueType = new()
            {
                Code = typeName,
            };

            extensionEdValue.Type.Add(edValueType);

            // add each of the replaceable types
            foreach (string rt in replaceableTypes)
            {
                addDatatypeExtension(
                    extSd,
                    element,
                    sourcePackageSupport,
                    ref extBuilderRec,
                    extElementId,           //extElementId + ".value[x].extension",
                    extElementPath,         //extElementPath + ".value[x].extension",
                    rt);
            }

            usedTypes.Add(typeName);
        }

        return extBuilderRec;
    }


    private void addDatatypeExtension(
        StructureDefinition extSd,
        DbElement sourceDbElement,
        PackageXverSupport sourcePackageSupport,
        ref ExtElementBuilderRecord extBuilderRecord,
        string parentId,
        string parentPath,
        string typeName)
    {
        // if we don't have the element already, we need to create the whole set
        if (extBuilderRecord.DatatypeValueElement == null)
        {
            extBuilderRecord.DatatypeSliceElement = new()
            {
                ElementId = parentId + ".extension:_datatype",
                Path = parentPath + ".extension",
                SliceName = "_datatype",
                Short = $"Data type name for {sourceDbElement.Id} from FHIR {sourcePackageSupport.Package.ShortName}",
                Definition = $"Data type name for {sourceDbElement.Id} from FHIR {sourcePackageSupport.Package.ShortName}",
                Min = 0,
                Max = "1",
                Type = [
                    new()
                    {
                        Code = "Extension",
                        Profile = ["http://hl7.org/fhir/StructureDefinition/_datatype"],
                    }
                ],
            };

            extBuilderRecord.DatatypeValueElement = new()
            {
                ElementId = parentId + ".extension:_datatype.value[x]",
                Path = parentPath + ".extension.value[x]",
                Comment = $"Must be: {typeName}",
                Min = 1,
                Max = "1",
                Base = new()
                {
                    Path = "Extension.value[x]",
                    Min = 0,
                    Max = "1",
                },
                Type = [
                    new()
                        {
                            Code = "string",
                        }
                ],
                Fixed = new FhirString(typeName),
            };

            extBuilderRecord.ExtendedDatatypeNames = [typeName];

            // done
            return;
        }

        // need to add this type
        extBuilderRecord.ExtendedDatatypeNames.Add(typeName);
        extBuilderRecord.DatatypeValueElement.Fixed = null;
        extBuilderRecord.DatatypeValueElement.Comment += "|" + typeName;
    }


    private List<DbElement> discoverContexts(
        HashSet<string> contextElementPaths,
        int sourceIndex,
        int targetIndex,
        PackageXverSupport targetPackageSupport,
        DbElement element,
        Dictionary<int, List<DbGraphSd.DbElementRow>> elementProjectionDict)

    {
        List<DbElement> contextElements = [];

        // iterate over the element projection rows
        foreach ((DbGraphSd.DbElementRow elementRow, int elementRowNumber) in elementProjectionDict[element.Key].Select((r, i) => (r, i)))
        {
            // extract the element cell for this target
            DbGraphSd.DbElementCell? eCell = elementRow[targetIndex];

            // if the cell is not null, use the target path from the cell
            if (eCell != null)
            {
                contextElementPaths.Add(eCell.Element.Path);
                continue;
            }

            // need to try and find a parent for a path
            bool addedSomething = false;
            int? parentKey = element.ParentElementKey;
            while (parentKey != null)
            {
                int key = parentKey.Value;
                parentKey = null;

                if (elementProjectionDict.TryGetValue(key, out List<DbGraphSd.DbElementRow>? parentRows))
                {
                    foreach ((DbGraphSd.DbElementRow parentRow, int parentRowNumber) in parentRows.Select((r, i) => (r, i)))
                    {
                        // only match the equivalent row number
                        if (parentRowNumber != elementRowNumber)
                        {
                            continue;
                        }

                        // extract the element cell for this target
                        DbGraphSd.DbElementCell? parentCell = parentRow[targetIndex];
                        if (parentCell != null)
                        {
                            contextElements.Add(parentCell.Element);
                            contextElementPaths.Add(parentCell.Element.Path);
                            addedSomething = true;
                            break;
                        }

                        DbGraphSd.DbElementCell? contextCell = parentRow[sourceIndex];
                        if (contextCell != null)
                        {
                            if (contextCell.Element.ResourceFieldOrder == 0)
                            {
                                // add this as the context
                                contextElements.Add(contextCell.Element);
                                contextElementPaths.Add(contextCell.Element.Path);
                                addedSomething = true;
                                break;
                            }

                            parentKey = contextCell.Element.ParentElementKey;
                            break;
                        }
                    }
                }
            }

            if (!addedSomething)
            {
                // if we can't find anything that matches, see if this structure exists in the target
                string name = element.Path.Split('.')[0];

                if ((DbStructureDefinition.SelectCount(_db!.DbConnection, FhirPackageKey: targetPackageSupport.Package.Key, Id: name) != 0) ||
                    (targetPackageSupport.CoreDC?.ComplexTypesByName.ContainsKey(name) == true) ||
                    (targetPackageSupport.CoreDC?.ResourcesByName.ContainsKey(name) == true))
                {
                    contextElementPaths.Add(name);
                    DbElement? dbElement = DbElement.SelectSingle(
                        _db!.DbConnection,
                        FhirPackageKey: targetPackageSupport.Package.Key,
                        Id: name);
                    if (dbElement != null)
                    {
                        contextElements.Add(dbElement);
                    }
                }

                // if we do not find *anything* that matches, the caller will default by adding Element
            }
        }

        return contextElements;
    }


    private (string? shortText, string? definition, string? comment) getTextForExtensionElement(DbElement ed, string? reason)
    {
        List<string> strings = [];

        if (!string.IsNullOrEmpty(ed.Short))
        {
            strings.Add(ed.Short);
        }

        if (!string.IsNullOrEmpty(ed.Definition) &&
            !ed.Definition.Equals(ed.Short, StringComparison.Ordinal) &&
            !ed.Definition.Equals(ed.Short + ".", StringComparison.Ordinal))
        {
            strings.Add(ed.Definition);
        }

        if (!string.IsNullOrEmpty(reason))
        {
            strings.Add(reason!);
        }

        switch (strings.Count)
        {
            case 0:
                return (null, null, null);

            case 1:
                return (strings[0], null, null);

            case 2:
                return (strings[0], strings[1], null);

            default:
                return (strings[0], strings[1], string.Join("\n", strings.Skip(2)));
        }
    }


    private string collapsePathForId(string path)
    {
        string pathClean = path.Replace("[x]", string.Empty);
        string[] components = pathClean.Replace("[x]", string.Empty).Split('.');
        switch (components.Length)
        {
            case 0:
                return pathClean;

            case 1:
                return pathClean;

            case 2:
                {
                    if (pathClean.Length > 45)
                    {
                        string rName = (components[0].Length > 20)
                            ? new string(components[0].Where(char.IsUpper).ToArray())
                            : components[0];

                        string eName = (components[1].Length > 20)
                            ? $"{components[1][0]}" + new string(components[1].Where(char.IsUpper).ToArray())
                            : components[1];

                        return rName + "." + eName;
                    }

                    return pathClean;
                }

            default:
                {
                    // use the full first and last, and one character from each in-between
                    if (components[0].Length > 20)
                    {
                        components[0] = new string(components[0].Where(char.IsUpper).ToArray());
                    }

                    for (int i = 1; i < components.Length - 1; i++)
                    {
                        if (components[i].Length > 3)
                        {
                            components[i] = $"{components[i][0]}{components[i][1]}";
                        }
                    }

                    if (components.Last().Length > 20)
                    {
                        components[components.Length - 1] = $"{components[components.Length - 1][0]}" + new string(components[0].Where(char.IsUpper).ToArray());
                    }

                    return string.Join('.', components);
                }

        }
    }


    private Dictionary<(int sourceVsKey, int targetPackageId), ValueSet> buildXverValueSets(
        List<DbFhirPackage> packages,
        int sourcePackageIndex)
    {
        DbFhirPackage sourcePackage = packages[sourcePackageIndex];

        Dictionary<(int sourceVsKey, int targetPackageId), ValueSet> xverValueSets = [];

        // get the list of value sets in this version that have a required binding
        List<DbValueSet> valueSets = DbValueSet.SelectList(
            _db!.DbConnection,
            FhirPackageKey: sourcePackage.Key);

        // iterate over the value sets
        foreach (DbValueSet vs in valueSets)
        {
            // skip excluded content and value sets that cannot expand
            if (_exclusionSet.Contains(vs.UnversionedUrl) ||
                _exclusionSet.Contains(vs.VersionedUrl) ||
                (vs.CanExpand == false) ||
                (vs.IsExcluded == true))
            {
                continue;
            }

            // build a graph for this value set
            DbGraphVs vsGraph = new()
            {
                DB = _db!.DbConnection,
                Packages = packages,
                KeyVs = vs,
            };

            // build a dictionary of all concept projections, by concept key
            Dictionary<int, List<DbGraphVs.DbVsConceptRow>> conceptProjectionDict = [];
            foreach (DbGraphVs.DbVsRow vsRow in vsGraph.Projection)
            {
                List<DbGraphVs.DbVsConceptRow> conceptProjections = vsRow.Projection;
                foreach (DbGraphVs.DbVsConceptRow vsConceptRow in conceptProjections)
                {
                    if (vsConceptRow.KeyCell == null)
                    {
                        continue;
                    }

                    if (!conceptProjectionDict.TryGetValue(vsConceptRow.KeyCell.Concept.Key, out List<DbGraphVs.DbVsConceptRow>? conceptList))
                    {
                        conceptList = [];
                        conceptProjectionDict.Add(vsConceptRow.KeyCell.Concept.Key, conceptList);
                    }

                    conceptList.Add(vsConceptRow);
                }
            }

            // build the value sets for this package
            buildXverValueSets(
                packages,
                sourcePackageIndex,
                vs,
                conceptProjectionDict,
                xverValueSets);
        }

        return xverValueSets;
    }

    private void buildXverValueSets(
        List<DbFhirPackage> packages,
        int sourcePackageIndex,
        DbValueSet sourceVs,
        Dictionary<int, List<DbGraphVs.DbVsConceptRow>> conceptProjectionDict,
        Dictionary<(int sourceVsKey, int targetPackageId), ValueSet> xverValueSets,
        HashSet<int>? conceptsWithoutEquivalent = null,
        ValueSet? xverVs = null,
        int currentPackageIndex = -1,
        int targetPackageIndex = -1)
    {
        // check for starting conditions
        if ((currentPackageIndex == -1) ||
            (targetPackageIndex == -1))
        {
            // if we are not the last package, build upwards
            if (sourcePackageIndex < (packages.Count - 1))
            {
                buildXverValueSets(
                    packages,
                    sourcePackageIndex,
                    sourceVs,
                    conceptProjectionDict,
                    xverValueSets,
                    conceptsWithoutEquivalent,
                    xverVs,
                    currentPackageIndex: sourcePackageIndex,
                    targetPackageIndex: sourcePackageIndex + 1);
            }

            // if we are not the first package, build downwards
            if (sourcePackageIndex > 0)
            {
                buildXverValueSets(
                    packages,
                    sourcePackageIndex,
                    sourceVs,
                    conceptProjectionDict,
                    xverValueSets,
                    conceptsWithoutEquivalent,
                    xverVs,
                    currentPackageIndex: sourcePackageIndex,
                    targetPackageIndex: sourcePackageIndex - 1);
            }

            // done
            return;
        }

        bool testingRight = currentPackageIndex < targetPackageIndex;
        bool testingLeft = !testingRight;
        conceptsWithoutEquivalent ??= [];

        DbFhirPackage sourcePackage = packages[sourcePackageIndex];
        DbFhirPackage targetPackage = packages[targetPackageIndex];

        //string sourceDashTarget = $"{focusPackage.ShortName}-{targetPackage.ShortName}";
        string vsId = $"{sourcePackage.ShortName}-{sourceVs.Id}-for-{targetPackage.ShortName}";
        //string vsId = $"{sourceDashTarget}-{sourceVs.Id}";

        ValueSet vs = new()
        {
            Url = $"http://hl7.org/fhir/{sourcePackage.FhirVersionShort}/ValueSet/{vsId}",
            Id = vsId,
            Version = _crossDefinitionVersion,
            Name = FhirSanitizationUtils.ReformatIdForName(vsId),
            Title = $"Cross-version VS for {sourcePackage.ShortName}.{sourceVs.Name} for use in FHIR {targetPackage.ShortName}",
            Status = PublicationStatus.Active,
            Experimental = false,
            DateElement = new FhirDateTime(DateTimeOffset.Now),
            Description = $"This cross-version ValueSet represents concepts from {sourceVs.VersionedUrl} for use in FHIR {targetPackage.ShortName}." +
                    $" Concepts not present here have direct `equivalent` mappings crossing all versions from {sourcePackage.ShortName} to {targetPackage.ShortName}.",
            Compose = new()
            {
                Include = [],
            },
            Expansion = new()
            {
                TimestampElement = new FhirDateTime(DateTimeOffset.Now),
                Contains = [],
            },
        };

        Dictionary<string, ValueSet.ConceptSetComponent> composeIncludes = [];

        // if we have an existing VS, start with the compose and expansion from that one (note that nonEquivalentConceptKeys will already be populated)
        if (xverVs != null)
        {
            vs.Compose = (ValueSet.ComposeComponent)xverVs.Compose.DeepCopy();
            foreach (ValueSet.ConceptSetComponent composeInclude in vs.Compose.Include)
            {
                composeIncludes.Add(composeInclude.System + "|" + composeInclude.Version, composeInclude);
            }

            vs.Expansion = (ValueSet.ExpansionComponent)xverVs.Expansion.DeepCopy();
        }

        // iterate over the projections
        foreach ((int sourceConceptKey, List<DbGraphVs.DbVsConceptRow> conceptProjections) in conceptProjectionDict)
        {
            // skip if we know this concept has already mapped out
            if (conceptsWithoutEquivalent.Contains(sourceConceptKey))
            {
                continue;
            }

            // check to see if we have any equivalent mappings
            if (testingRight &&
                conceptProjections.Any((DbGraphVs.DbVsConceptRow vsConceptRow) => vsConceptRow[currentPackageIndex]?.RightComparison?.Relationship == CMR.Equivalent))
            {
                continue;
            }

            if (testingLeft &&
                conceptProjections.Any((DbGraphVs.DbVsConceptRow vsConceptRow) => vsConceptRow[currentPackageIndex]?.LeftComparison?.Relationship == CMR.Equivalent))
            {
                continue;
            }

            // add this concept as not directly equivalent
            conceptsWithoutEquivalent.Add(sourceConceptKey);

            // check to see if we have this concept
            DbValueSetConcept concept = conceptProjections[0].KeyCell?.Concept ?? throw new Exception($"Failed to resolve concept for {sourceConceptKey} in {sourceVs.Name}!");

            string composeKey = concept.System + "|" + concept.SystemVersion;

            if (!composeIncludes.TryGetValue(composeKey, out ValueSet.ConceptSetComponent? composeInclude))
            {
                // create a new include for this concept
                composeInclude = new()
                {
                    System = concept.System,
                    Version = concept.SystemVersion,
                    Concept = [],
                };
                composeIncludes.Add(composeKey, composeInclude);
                vs.Compose.Include.Add(composeInclude);
            }

            composeInclude.Concept.Add(new()
            {
                Code = concept.Code,
                Display = concept.Display,
            });

            // add this concept to the expansion
            vs.Expansion.Contains.Add(new()
            {
                System = concept.System,
                Version = concept.SystemVersion,
                Code = concept.Code,
                Display = concept.Display,
            });
        }

        // add this value set to the dictionary if it has any concepts
        if (vs.Expansion.Contains.Count > 0)
        {
            xverValueSets.Add((sourceVs.Key, targetPackage.Key), vs);
        }

        // check for continuing to the next package to the right
        if (testingRight &&
            (targetPackageIndex < packages.Count - 1))
        {
            // build the value set for this package
            buildXverValueSets(
                packages,
                sourcePackageIndex,
                sourceVs,
                conceptProjectionDict,
                xverValueSets,
                conceptsWithoutEquivalent,
                vs,
                currentPackageIndex: targetPackageIndex,
                targetPackageIndex: targetPackageIndex + 1);
        }

        // check for continuing to the next package to the left
        if (testingLeft &&
            (targetPackageIndex > 0))
        {
            // build the value set for this package
            buildXverValueSets(
                packages,
                sourcePackageIndex,
                sourceVs,
                conceptProjectionDict,
                xverValueSets,
                conceptsWithoutEquivalent,
                vs,
                currentPackageIndex: targetPackageIndex,
                targetPackageIndex: targetPackageIndex - 1);
        }

        return;
    }



}
