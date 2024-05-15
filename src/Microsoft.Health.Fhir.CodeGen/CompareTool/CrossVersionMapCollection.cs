// <copyright file="CrossVersionMapCollection.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.CodeGen.Loader;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.FhirExtensions;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using Microsoft.Health.Fhir.PackageManager;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;

namespace Microsoft.Health.Fhir.CodeGen.CompareTool;

public class CrossVersionMapCollection
{
    private const string _canonicalRootCrossVersion = "http://hl7.org/fhir/uv/xver/";
    private const string _canonicalRootHl7 = "http://hl7.org/fhir/";
    private const string _canonicalRootCi = "http://build.fhir.org/";
    private const string _canonicalRootTHO = "http://terminology.hl7.org/";

    public enum CrossVersionMapTypeCodes
    {
        ValueSetConcepts,
        DataTypeConcepts,
        ResourceTypeConcepts,
        ComplexTypeElementConcepts,
        ResourceElementConcepts,
    }

    private PackageLoader _loader = null!;

    private IFhirPackageClient _cache = null!;

    private DefinitionCollection _source;
    private DefinitionCollection _target;
    private DefinitionCollection _dc = null!;
    
    private string _mapCanonical = string.Empty;

    private FhirReleases.FhirSequenceCodes _sourceFhirSequence;
    private string _sourcePackageCanonical;
    private string _sourceRLiteral;
    private string _sourceShortVersionUrlSegment;
    private string _sourcePackageVersion;

    private FhirReleases.FhirSequenceCodes _targetFhirSequence;
    private string _targetPackageCanonical;
    private string _targetRLiteral;
    private string _targetShortVersionUrlSegment;
    private string _targetPackageVersion;

    private string _sourceToTargetWithR;
    private int _sourceToTargetWithRLen;
    private string _sourceToTargetNoR;
    private int _sourceToTargetNoRLen;

    private Dictionary<string, string> _urlMap = [];

    // dictionary with keys of source value set urls and values of each target vs there is a map for
    private Dictionary<string, List<string>> _vsUrlsWithMaps = [];

    private ConceptMap? _dataTypeMap = null;
    private ConceptMap? _resourceTypeMap = null;
    private Dictionary<string, ConceptMap> _elementConceptMaps = [];


    public CrossVersionMapCollection(
        IFhirPackageClient cache,
        DefinitionCollection source,
        DefinitionCollection target)
    {
        _cache = cache;
        _loader = new(_cache, new()
        {
            JsonModel = LoaderOptions.JsonDeserializationModel.SystemTextJson,
            AutoLoadExpansions = false,
            ResolvePackageDependencies = false,
        });

        _source = source;

        _sourcePackageCanonical = source.MainPackageCanonical;
        _sourceFhirSequence = source.FhirSequence;
        _sourceRLiteral = source.FhirSequence.ToRLiteral();
        _sourceShortVersionUrlSegment = "/" + source.FhirSequence.ToShortVersion() + "/";
        _sourcePackageVersion = source.FhirVersionLiteral;

        _target = target;

        _targetPackageCanonical = target.MainPackageCanonical;
        _targetFhirSequence = target.FhirSequence;
        _targetRLiteral = target.FhirSequence.ToRLiteral();
        _targetShortVersionUrlSegment = "/" + target.FhirSequence.ToShortVersion() + "/";
        _targetPackageVersion = target.FhirVersionLiteral;

        _sourceToTargetWithR = $"{_sourceRLiteral}to{_targetRLiteral}";
        _sourceToTargetWithRLen = _sourceToTargetWithR.Length;
        _sourceToTargetNoR = $"{_sourceRLiteral[1..]}to{_targetRLiteral[1..]}";
        _sourceToTargetNoRLen = _sourceToTargetNoR.Length;

        //_mapCanonical = $"http://hl7.org/fhir/uv/xver/{_leftRLiteral.ToLowerInvariant()}-{_rightRLiteral.ToLowerInvariant()}";
        _mapCanonical = BuildUrl("{0}{3}-{5}", _canonicalRootCrossVersion);

        // create our maps collection
        _dc = new()
        {
            Name = "FHIR Cross Version Maps",
            FhirVersion = FHIRVersion.N5_0_0,
            FhirVersionLiteral = "5.0.0",
            FhirSequence = FhirReleases.FhirSequenceCodes.R5,
            MainPackageId = $"hl7.fhir.uv.xver.{_sourceRLiteral.ToLowerInvariant()}-{_targetRLiteral.ToLowerInvariant()}",
            MainPackageVersion = "0.0.1",
            MainPackageCanonical = _mapCanonical,
        };
    }

    public bool PathHasFhirCrossVersionOfficial(string path)
    {
        if (!Directory.Exists(path))
        {
            return false;
        }

        string igFilePath = Path.Combine(path, "ig.ini");

        if (!File.Exists(igFilePath))
        {
            return false;
        }

        if (File.ReadAllText(igFilePath).Contains("xver-ig.xml"))
        {
            return true;
        }

        return false;
    }

    public bool PathHasFhirCrossVersionSource(string path)
    {
        if (!Directory.Exists(path))
        {
            return false;
        }

        string toolInfoPath = Path.Combine(path, "tool-info.txt");

        if (!File.Exists(toolInfoPath))
        {
            return false;
        }

        if (File.ReadAllText(toolInfoPath) == "fhir-cross-version-source#v2")
        {
            return true;
        }

        return false;
    }

    public bool TryLoadConceptMaps(string path)
    {
        bool isOfficial = PathHasFhirCrossVersionOfficial(path);
        bool isSource = PathHasFhirCrossVersionSource(path);

        if (isOfficial && isSource)
        {
            throw new Exception($"Cannot determine style of maps to load from {path}!");
        }

        if (isOfficial)
        {
            return TryLoadOfficialConceptMaps(path);
        }

        if (isSource)
        {
            return TryLoadSourceConceptMaps(path);
        }

        return false;
    }

    private bool TryLoadSourceConceptMaps(string crossRepoPath)
    {
        Console.WriteLine($"Loading fhir-cross-version-source concept maps for conversion from {_sourceRLiteral} to {_targetRLiteral}...");

        if (!TryLoadSourceConceptMaps(crossRepoPath, CrossVersionMapTypeCodes.ValueSetConcepts))
        {
            throw new Exception($"Failed to load fhir-cross-version-source value set concept maps");
        }

        if (!TryLoadSourceConceptMaps(crossRepoPath, CrossVersionMapTypeCodes.DataTypeConcepts))
        {
            throw new Exception($"Failed to load fhir-cross-version-source data type concept map");
        }

        if (!TryLoadSourceConceptMaps(crossRepoPath, CrossVersionMapTypeCodes.ResourceTypeConcepts))
        {
            throw new Exception($"Failed to load fhir-cross-version-source resource type concept map");
        }

        if (!TryLoadSourceConceptMaps(crossRepoPath, CrossVersionMapTypeCodes.ComplexTypeElementConcepts))
        {
            throw new Exception($"Failed to load fhir-cross-version-source complex type element concept maps");
        }

        if (!TryLoadSourceConceptMaps(crossRepoPath, CrossVersionMapTypeCodes.ResourceElementConcepts))
        {
            throw new Exception($"Failed to load fhir-cross-version-source resource element concept maps");
        }

        return true;
    }

    private bool TryLoadOfficialConceptMaps(string fhirCrossRepoPath)
    {
        Console.WriteLine($"Loading fhir-cross-version concept maps for conversion from {_sourceRLiteral} to {_targetRLiteral}...");

        if (!TryLoadOfficialConceptMaps(fhirCrossRepoPath, "codes"))
        {
            throw new Exception($"Failed to load fhir-cross-version code concept maps");
        }

        if (!TryLoadOfficialConceptMaps(fhirCrossRepoPath, "types"))
        {
            throw new Exception($"Failed to load fhir-cross-version type concept maps");
        }

        if (!TryLoadOfficialConceptMaps(fhirCrossRepoPath, "resources"))
        {
            throw new Exception($"Failed to load fhir-cross-version resource concept maps");
        }

        if (!TryLoadOfficialConceptMaps(fhirCrossRepoPath, "elements"))
        {
            throw new Exception($"Failed to load fhir-cross-version element concept maps");
        }

        return true;

    }

    private bool TryLoadOfficialConceptMaps(string fhirCrossRepoPath, string key)
    {
        string path = Path.Combine(fhirCrossRepoPath, "input", key);
        if (!Directory.Exists(path))
        {
            throw new DirectoryNotFoundException($"Could not find fhir-cross-version/input/{key} directory: {path}");
        }

        // files appear similar to ConceptMap-types-4bto5.json
        string[] files = Directory.GetFiles(path, $"ConceptMap*-{_sourceToTargetNoR}.json", SearchOption.TopDirectoryOnly);

        foreach (string filename in files)
        {
            try
            {
                object? loaded = _loader.ParseContentsSystemTextStream("fhir+json", filename, typeof(ConceptMap));
                if (loaded is not ConceptMap cm)
                {
                    Console.WriteLine($"Error loading {filename}: could not parse as ConceptMap");
                    continue;
                }

                // fix urls so we can find things
                switch (key)
                {
                    case "codes":
                        {
                            if (cm.Group.Count == 0)
                            {
                                throw new Exception($"Invalid concept map {filename}: expected 1 group, found {cm.Group.Count}");
                            }

                            string sourceScopeUrl = string.Empty;
                            string targetScopeUrl = string.Empty;

                            // try to look up the actual value set URLs based on the elements
                            if (cm.SourceScope is FhirUri originalSourceUri)
                            {
                                string elementPath = originalSourceUri.Value.Split('#')[^1];
                                if (_source.TryFindElementByPath(elementPath, out StructureDefinition? sd, out ElementDefinition? ed))
                                {
                                    sourceScopeUrl = ed.Binding?.ValueSet ?? string.Empty;
                                }
                            }

                            if (cm.TargetScope is FhirUri originalTargetUri)
                            {
                                string elementPath = originalTargetUri.Value.Split('#')[^1];
                                if (_target.TryFindElementByPath(elementPath, out StructureDefinition? sd, out ElementDefinition? ed))
                                {
                                    targetScopeUrl = ed.Binding?.ValueSet ?? string.Empty;
                                }
                            }

                            string unversionedSourceUrl = sourceScopeUrl.Contains('|') ? sourceScopeUrl.Split('|')[0] : sourceScopeUrl;
                            string unversionedTargetUrl = targetScopeUrl.Contains('|') ? targetScopeUrl.Split('|')[0] : targetScopeUrl;

                            if (string.IsNullOrEmpty(sourceScopeUrl) || string.IsNullOrEmpty(targetScopeUrl))
                            {
                                throw new Exception($"Cannot resolve scope references in {filename}");
                            }

                            // check for exclusions
                            if (PackageComparer._exclusionSet.Contains(sourceScopeUrl))
                            {
                                continue;
                            }

                            string leftName = NameFromUrl(sourceScopeUrl);
                            string rightName = NameFromUrl(targetScopeUrl);

                            string originalMapUrl = cm.Url;

                            string localConceptMapId = $"{_sourceRLiteral}-{leftName}-{_targetRLiteral}-{rightName}";
                            string localUrl = BuildUrl("{0}/{1}/{2}", _mapCanonical, name: localConceptMapId, resourceType: "ConceptMap");

                            // if we have a mapping for this value set pair already, just track the source name
                            if (_dc.ConceptMapsByUrl.TryGetValue(localUrl, out ConceptMap? existing))
                            {
                                // add this local URL to our index
                                _urlMap.Add(originalMapUrl, localUrl);

                                // keep track of the original URL we can round-trip back
                                existing.AddExtension(CommonDefinitions.ExtUrlConceptMapAdditionalUrls, new FhirUrl(originalMapUrl));

                                // nothing else to do with this map
                                continue;
                            }

                            // update our info
                            cm.Id = localConceptMapId;
                            cm.Url = localUrl;
                            cm.Name = localConceptMapId;
                            cm.Title = GetConceptMapTitle(leftName, rightName);
                            cm.AddExtension(CommonDefinitions.ExtUrlConceptMapAdditionalUrls, new FhirUrl(originalMapUrl));

                            // try to manufacture correct value set URLs based on what we have
                            cm.SourceScope = new Canonical(unversionedSourceUrl + "|" + _sourcePackageVersion);
                            cm.TargetScope = new Canonical(unversionedTargetUrl + "|" + _targetPackageVersion);

                            foreach (ConceptMap.GroupComponent group in cm.Group)
                            {
                                // fix the source and target value set URLs if they have had versions inserted

                                if (group.Source.Contains(_sourceShortVersionUrlSegment, StringComparison.Ordinal))
                                {
                                    group.Source = group.Source.Replace(_sourceShortVersionUrlSegment, "/");
                                }

                                if (group.Target.Contains(_targetShortVersionUrlSegment, StringComparison.Ordinal))
                                {
                                    group.Target = group.Target.Replace(_targetShortVersionUrlSegment, "/");
                                }
                            }

                            // add to our listing of value set maps
                            if (_vsUrlsWithMaps.TryGetValue(unversionedSourceUrl, out List<string>? rightList))
                            {
                                rightList.Add(unversionedTargetUrl);
                            }
                            else
                            {
                                _vsUrlsWithMaps.Add(unversionedSourceUrl, [targetScopeUrl]);
                            }

                        }
                        break;

                    case "types":
                        {
                            if (cm.Group.Count != 1)
                            {
                                throw new Exception($"Invalid concept map {filename}: expected 1 group, found {cm.Group.Count}");
                            }

                            string officialUrl = cm.Url;

                            string localConceptMapId = $"{_sourceRLiteral}-types-{_targetRLiteral}";
                            string localUrl = BuildUrl("{0}/{1}/{2}", _mapCanonical, name: localConceptMapId, resourceType: "ConceptMap");

                            // update our info
                            cm.Id = localConceptMapId;
                            cm.Url = localUrl;
                            cm.Name = localConceptMapId;
                            cm.Title = GetConceptMapTitle("data type");

                            string sourceLocalUrl = BuildUrl("{0}/{1}/{2}", _sourcePackageCanonical, "ValueSet", "data-types");
                            string targetLocalUrl = BuildUrl("{0}/{1}/{2}", _targetPackageCanonical, "ValueSet", "data-types");

                            cm.SourceScope = new Canonical($"{sourceLocalUrl}|{_sourcePackageVersion}");
                            cm.TargetScope = new Canonical($"{targetLocalUrl}|{_targetPackageVersion}");

                            cm.Group[0].Source = sourceLocalUrl;
                            cm.Group[0].Target = targetLocalUrl;

                            _dataTypeMap = cm;
                        }
                        break;

                    case "resources":
                        {
                            if (cm.Group.Count != 1)
                            {
                                throw new Exception($"Invalid concept map {filename}: expected 1 group, found {cm.Group.Count}");
                            }

                            string officialUrl = cm.Url;

                            string localConceptMapId = $"{_sourceRLiteral}-resources-{_targetRLiteral}";
                            string localUrl = BuildUrl("{0}/{1}/{2}", _mapCanonical, name: localConceptMapId, resourceType: "ConceptMap");

                            // update our info
                            cm.Id = localConceptMapId;
                            cm.Url = localUrl;
                            cm.Name = localConceptMapId;
                            cm.Title = GetConceptMapTitle("resource type");

                            string sourceLocalUrl = BuildUrl("{0}/{1}/{2}", _sourcePackageCanonical, "ValueSet", "resources");
                            string targetLocalUrl = BuildUrl("{0}/{1}/{2}", _targetPackageCanonical, "ValueSet", "resources");

                            cm.SourceScope = new Canonical($"{sourceLocalUrl}|{_sourcePackageVersion}");
                            cm.TargetScope = new Canonical($"{targetLocalUrl}|{_targetPackageVersion}");

                            cm.Group[0].Source = sourceLocalUrl;
                            cm.Group[0].Target = targetLocalUrl;

                            _resourceTypeMap = cm;
                        }
                        break;

                    case "elements":
                        {
                            if (cm.Group.Count != 1)
                            {
                                throw new Exception($"Invalid concept map {filename}: expected 1 group, found {cm.Group.Count}");
                            }

                            string officialUrl = cm.Url;

                            string localConceptMapId = $"{_sourceRLiteral}-elements-{_targetRLiteral}";
                            string localUrl = BuildUrl("{0}/{1}/{2}", _mapCanonical, name: localConceptMapId, resourceType: "ConceptMap");

                            // update our info
                            cm.Id = localConceptMapId;
                            cm.Url = localUrl;
                            cm.Name = localConceptMapId;
                            cm.Title = GetConceptMapTitle("element");

                            string sourceLocalUrl = BuildUrl("{0}/{1}/{2}", _sourcePackageCanonical, "ValueSet", "elements");
                            string targetLocalUrl = BuildUrl("{0}/{1}/{2}", _targetPackageCanonical, "ValueSet", "elements");

                            cm.SourceScope = new Canonical($"{sourceLocalUrl}|{_sourcePackageVersion}");
                            cm.TargetScope = new Canonical($"{targetLocalUrl}|{_targetPackageVersion}");

                            cm.Group[0].Source = sourceLocalUrl;
                            cm.Group[0].Target = targetLocalUrl;

                            // traverse this element map for all elements and build individual maps
                            Dictionary<string, List<ConceptMap.SourceElementComponent>> typeElements = [];
                            foreach (ConceptMap.SourceElementComponent sec in cm.Group[0].Element)
                            {
                                string typeName = sec.Code.Split('.')[0];

                                if (typeElements.TryGetValue(typeName, out List<ConceptMap.SourceElementComponent>? elements))
                                {
                                    elements.Add(sec);
                                }
                                else
                                {
                                    typeElements.Add(typeName, [sec]);
                                }
                            }

                            // traverse our extracted elements and build new concept maps
                            foreach ((string typeName, List<ConceptMap.SourceElementComponent> elements) in typeElements)
                            {
                                if (elements.Count == 0)
                                {
                                    continue;
                                }

                                string elementMapId = $"{_sourceRLiteral}-{typeName}-{_targetRLiteral}-{typeName}";
                                string elementMapUrl = BuildUrl("{0}/{1}/{2}", _mapCanonical, name: elementMapId, resourceType: "ConceptMap");

                                string elementSourceUrl = BuildUrl("{0}/{1}/{2}", _sourcePackageCanonical, "StructureDefinition", typeName);
                                string elementTargetUrl = elements[0].Target.Count != 0
                                    ? BuildUrl("{0}/{1}/{2}", _targetPackageCanonical, "StructureDefinition", elements[0].Target[0].Code.Split('.')[0])
                                    : BuildUrl("{0}/{1}/{2}", _targetPackageCanonical, "StructureDefinition", typeName);

                                ConceptMap elementMap = new()
                                {
                                    Id = elementMapId,
                                    Url = elementMapUrl,
                                    Name = elementMapId,
                                    Title = GetConceptMapTitle(typeName),
                                    SourceScope = new Canonical($"{elementSourceUrl}|{_sourcePackageVersion}"),
                                    TargetScope = new Canonical($"{elementTargetUrl}|{_targetPackageVersion}"),
                                    Group = [new ConceptMap.GroupComponent
                                    {
                                        Source = elementSourceUrl,
                                        Target = elementTargetUrl,
                                        Element = elements,
                                    }],
                                };

                                _elementConceptMaps.Add(typeName, elementMap);
                            }
                        }
                        break;
                }

                // add this to our maps
                _dc.AddConceptMap(cm, _dc.MainPackageId, _dc.MainPackageVersion);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading {filename}: {ex.Message}");
            }
        }

        return true;
    }

    private string FilenameForMap(CrossVersionMapTypeCodes mapType, string name = "", string targetName = "") => mapType switch
    {
        CrossVersionMapTypeCodes.ValueSetConcepts => $"ConceptMap-{_sourceRLiteral}-{name}-{_targetRLiteral}.json",
        CrossVersionMapTypeCodes.DataTypeConcepts => $"ConceptMap-{_sourceRLiteral}-data-types-{_targetRLiteral}.json",
        CrossVersionMapTypeCodes.ResourceTypeConcepts => $"ConceptMap-{_sourceRLiteral}-resource-types-{_targetRLiteral}.json",
        CrossVersionMapTypeCodes.ComplexTypeElementConcepts => $"ConceptMap-{_sourceRLiteral}-{name}-{_targetRLiteral}.json",
        CrossVersionMapTypeCodes.ResourceElementConcepts => string.IsNullOrEmpty(targetName)
        ? $"ConceptMap-{_sourceRLiteral}-{name}-{_targetRLiteral}.json"
        : $"ConceptMap-{_sourceRLiteral}-{name}-{_targetRLiteral}-{targetName}.json",
        _ => throw new ArgumentException($"Unknown map type: {mapType}"),
    };

    private string RelativePathForMap(CrossVersionMapTypeCodes mapType, string sourcePath) => mapType switch
    {
        CrossVersionMapTypeCodes.ValueSetConcepts => Path.Combine(sourcePath, "ValueSets"),
        CrossVersionMapTypeCodes.ComplexTypeElementConcepts => Path.Combine(sourcePath, "ComplexTypes"),
        CrossVersionMapTypeCodes.ResourceElementConcepts => Path.Combine(sourcePath, "Resources"),
        _ => sourcePath,
    };

    private bool TryLoadSourceConceptMaps(string crossRepoPath, CrossVersionMapTypeCodes mapType)
    {
        string rootPath = Path.Combine(crossRepoPath, $"{_sourceRLiteral}_{_targetRLiteral}", "maps");
        if (!Directory.Exists(rootPath))
        {
            throw new DirectoryNotFoundException($"Could not find {_sourceRLiteral}_{_targetRLiteral}/maps directory: {rootPath}");
        }

        string filenameFilter = FilenameForMap(mapType, "*");
        string sourcePath = RelativePathForMap(mapType, rootPath);

        string[] files = Directory.GetFiles(sourcePath, filenameFilter, SearchOption.TopDirectoryOnly);

        foreach (string filename in files)
        {
            try
            {
                object? loaded = _loader.ParseContentsSystemTextStream("fhir+json", filename, typeof(ConceptMap));
                if (loaded is not ConceptMap cm)
                {
                    Console.WriteLine($"Error loading {filename}: could not parse as ConceptMap");
                    continue;
                }

                // fix urls so we can find things
                switch (mapType)
                {
                    case CrossVersionMapTypeCodes.ValueSetConcepts:
                        {
                            if (cm.Group.Count != 1)
                            {
                                throw new Exception($"Invalid concept map {filename}: expected 1 group, found {cm.Group.Count}");
                            }

                            if (_dc!.ConceptMapsByUrl.ContainsKey(cm.Url))
                            {
                                Console.WriteLine($"Skipping repeat load of {cm.Url}...");
                                continue;
                            }

                            string sourceLocalUrl = cm.Group[0].Source;
                            string targetLocalUrl = cm.Group[0].Target;

                            // add to our listing of value set maps
                            if (_vsUrlsWithMaps.TryGetValue(sourceLocalUrl, out List<string>? rightList))
                            {
                                rightList.Add(targetLocalUrl);
                            }
                            else
                            {
                                _vsUrlsWithMaps.Add(sourceLocalUrl, [targetLocalUrl]);
                            }

                            // add any known element-based name maps into our lookup table
                            foreach (Extension ext in cm.GetExtensions(CommonDefinitions.ExtUrlConceptMapAdditionalUrls))
                            {
                                if (ext.Value is FhirUrl fhirUrl)
                                {
                                    // use the official URL as the key
                                    _urlMap.Add(fhirUrl.Value, cm.Url);
                                }
                            }
                        }
                        break;

                    case CrossVersionMapTypeCodes.DataTypeConcepts:
                        {
                            _dataTypeMap = cm;
                        }
                        break;

                    case CrossVersionMapTypeCodes.ResourceTypeConcepts:
                        {
                            _resourceTypeMap = cm;
                        }
                        break;

                    case CrossVersionMapTypeCodes.ComplexTypeElementConcepts:
                        {
                            string leftName;
                            if (cm.SourceScope is Canonical leftCanonical)
                            {
                                leftName = leftCanonical.Uri?.Split(['/', '#'])[^1] ?? throw new Exception($"Invalid left canonical in {cm.Url}");
                            }
                            else
                            {
                                throw new Exception($"Invalid left canonical in {cm.Url}");
                            }

                            _elementConceptMaps.Add(leftName, cm);
                        }
                        break;

                    case CrossVersionMapTypeCodes.ResourceElementConcepts:
                        {
                            string leftName;
                            if (cm.SourceScope is Canonical leftCanonical)
                            {
                                leftName = leftCanonical.Uri?.Split(['/', '#'])[^1] ?? throw new Exception($"Invalid left canonical in {cm.Url}");
                            }
                            else
                            {
                                throw new Exception($"Invalid left canonical in {cm.Url}");
                            }

                            _elementConceptMaps.Add(leftName, cm);
                        }
                        break;
                }

                // add this to our maps
                _dc!.AddConceptMap(cm, _dc.MainPackageId, _dc.MainPackageVersion);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading {filename}: {ex.Message}");
            }
        }

        return true;
    }


    public ConceptMap? GetSourceValueSetConceptMap(
        ValueSetComparison vsc)
    {
        // we cannot write maps that do not have both a source and a target
        if (string.IsNullOrEmpty(vsc.Source.Url) || string.IsNullOrEmpty(vsc.Target.Url))
        {
            return null;
        }

        if (vsc.ConceptComparisons.Count == 0)
        {
            throw new Exception("Cannot process a comparison with no mappings!");
        }

        string localConceptMapId = $"{_sourceRLiteral}-{vsc.Source.NamePascal}-{_targetRLiteral}-{vsc.Target.NamePascal}";
        string localUrl = BuildUrl("{0}/{1}/{2}", _mapCanonical, name: localConceptMapId, resourceType: "ConceptMap");

        string sourceUrl = vsc.Source.Url;
        string targetUrl = vsc.Target.Url;

        string sourceCanonical = $"{sourceUrl}|{_sourcePackageVersion}";
        string targetCanonical = $"{targetUrl}|{_targetPackageVersion}";

        // check to see if we need to create a new concept map
        if (!_dc.ConceptMapsByUrl.TryGetValue(localUrl, out ConceptMap? cm))
        {
            cm = new();

            // update our info
            cm.Id = localConceptMapId;
            cm.Url = localUrl;
            cm.Name = localConceptMapId;
            cm.Title = GetConceptMapTitle(vsc.Source.Name);

            cm.SourceScope = new Canonical(sourceCanonical);
            cm.TargetScope = new Canonical(targetCanonical);
        }

        Dictionary<string, ConceptMap.GroupComponent> groups = [];

        string primaryTargetSystem = vsc.ConceptComparisons.Values
            .SelectMany(cc => cc.TargetMappings.Select(t => t.Target.System))
            .GroupBy(s => s)
            .OrderByDescending(c => c.Count())
            .FirstOrDefault()?.Key ?? vsc.ConceptComparisons.First().Value.Source.System;

        // traverse concepts that exist in our source
        foreach ((string code, ConceptComparison cc) in vsc.ConceptComparisons.OrderBy(kvp => kvp.Key))
        {
            string sourceSystem = cc.Source.System;
            string noTargetKey = $"{sourceSystem}||{primaryTargetSystem}";

            if (cc.TargetMappings.Count == 0)
            {
                if (!groups.TryGetValue(noTargetKey, out ConceptMap.GroupComponent? noTargetGroup))
                {
                    noTargetGroup = new()
                    {
                        Source = sourceSystem,
                        Target = primaryTargetSystem,
                    };

                    groups.Add(noTargetKey, noTargetGroup);
                }

                noTargetGroup.Element.Add(new()
                {
                    Code = cc.Source.Code,
                    Display = cc.Source.Description,
                    NoMap = true,
                });

                continue;
            }

            Dictionary<string, List<ConceptMap.TargetElementComponent>> elementTargetsBySystem = [];

            foreach (ConceptComparisonDetails targetMapping in cc.TargetMappings)
            {
                string targetSystem = targetMapping.Target.System;
                string key = string.IsNullOrEmpty(targetSystem)
                    ? noTargetKey
                    : $"{sourceSystem}||{targetSystem}";

                if (!elementTargetsBySystem.TryGetValue(key, out List<ConceptMap.TargetElementComponent>? elementTargets))
                {
                    elementTargets = [];
                    elementTargetsBySystem.Add(key, elementTargets);
                }

                elementTargets.Add(new()
                {
                    Code = targetMapping.Target.Code,
                    Display = targetMapping.Target.Description,
                    Relationship = targetMapping.Relationship,
                    Comment = targetMapping.Message,
                });
            }

            foreach ((string key, List<ConceptMap.TargetElementComponent> targets) in elementTargetsBySystem)
            {
                if (!groups.TryGetValue(key, out ConceptMap.GroupComponent? group))
                {
                    string[] components = key.Split("||");

                    group = new()
                    {
                        Source = components[0],
                        Target = components[1],
                    };

                    groups.Add(key, group);
                }

                group.Element.Add(new()
                {
                    Code = cc.Source.Code,
                    Display = cc.Source.Description,
                    Target = targets,
                });
            }
        }

        // check for a value set that could not map anything
        if (groups.Count == 0)
        {
            Console.WriteLine($"Not updating ConceptMap for {vsc.CompositeName} - no concepts are mapped to target!");
            return null;
        }

        cm.Group.Clear();
        cm.Group.AddRange(groups.Values);

        return cm;
    }

    //private CMR ApplyRelationship(CMR existing, CMR? change) => existing switch
    //{
    //    CMR.Equivalent => change ?? CMR.Equivalent,
    //    CMR.RelatedTo => (change == CMR.NotRelatedTo) ? CMR.NotRelatedTo : existing,
    //    CMR.SourceIsNarrowerThanTarget => (change == CMR.SourceIsNarrowerThanTarget || change == CMR.Equivalent)
    //        ? CMR.SourceIsNarrowerThanTarget : CMR.RelatedTo,
    //    CMR.SourceIsBroaderThanTarget => (change == CMR.SourceIsBroaderThanTarget || change == CMR.Equivalent)
    //        ? CMR.SourceIsBroaderThanTarget : CMR.RelatedTo,
    //    CMR.NotRelatedTo => change ?? existing,
    //    _ => change ?? existing,
    //};


    public ConceptMap? GetSourceDataTypesConceptMap(
        Dictionary<string, List<PrimitiveTypeComparison>> primitiveTypes,
        Dictionary<string, List<StructureComparison>> complexTypes)
    {
        string localConceptMapId = $"{_sourceRLiteral}-types-{_targetRLiteral}";
        string localUrl = BuildUrl("{0}/{1}/{2}", _mapCanonical, name: localConceptMapId, resourceType: "ConceptMap");

        string sourceUrl = BuildUrl("{0}/{1}/{2}", _sourcePackageCanonical, "ValueSet", "data-types");
        string targetUrl = BuildUrl("{0}/{1}/{2}", _targetPackageCanonical, "ValueSet", "data-types");

        string sourceCanonical = $"{sourceUrl}|{_sourcePackageVersion}";
        string targetCanonical = $"{targetUrl}|{_targetPackageVersion}";

        // check to see if we need to create a new concept map
        if (!_dc.ConceptMapsByUrl.TryGetValue(localUrl, out ConceptMap? cm))
        {
            cm = new();

            // update our info
            cm.Id = localConceptMapId;
            cm.Url = localUrl;
            cm.Name = localConceptMapId;
            cm.Title = GetConceptMapTitle("data types");

            cm.SourceScope = new Canonical(sourceCanonical);
            cm.TargetScope = new Canonical(targetCanonical);
        }

        ConceptMap.GroupComponent group = new();

        group.Source = sourceUrl;
        group.Target = targetUrl;

        // traverse primitive types
        foreach ((string key, List<PrimitiveTypeComparison> comparisons) in primitiveTypes.OrderBy(kvp => kvp.Key))
        {
            if (comparisons.Count == 0)
            {
                group.Element.Add(new()
                {
                    Code = key,
                    Display = $"Primitive type {_sourceRLiteral} `{key}` does not exist in {_targetRLiteral} and has no mappings.",
                    NoMap = true,
                });

                continue;
            }

            List<ConceptMap.TargetElementComponent> targets = [];

            foreach (PrimitiveTypeComparison c in comparisons.OrderBy(c => c.TargetTypeLiteral))
            {
                if (c.Target == null)
                {
                    Console.WriteLine($"Not adding {c.SourceTypeLiteral} - no target exists");
                    continue;
                }

                targets.Add(new ConceptMap.TargetElementComponent()
                {
                    Code = c.TargetTypeLiteral,
                    Display = c.Target.Description,
                    Relationship = c.Relationship,
                    Comment = c.Message,
                });
            }

            if (targets.Count == 0)
            {
                group.Element.Add(new()
                {
                    Code = key,
                    Display = $"Primitive type {_sourceRLiteral} `{key}` does not exist in {_targetRLiteral} and has no mappings.",
                    NoMap = true,
                });

                continue;
            }

            group.Element.Add(new()
            {
                Code = key,
                Display = $"Primitive type {_sourceRLiteral} `{key}` maps to {string.Join(" and ", targets.Select(tm => $"`{tm.Code}`"))}.",
                Target = targets,
            });
        }

        // traverse complex types
        foreach ((string key, List<StructureComparison> comparisons) in complexTypes.OrderBy(kvp => kvp.Key))
        {
            if (comparisons.Count == 0)
            {
                group.Element.Add(new()
                {
                    Code = key,
                    Display = $"Complex type {_sourceRLiteral} `{key}` does not exist in {_targetRLiteral} and has no mappings.",
                    NoMap = true,
                });

                continue;
            }

            List<ConceptMap.TargetElementComponent> targets = [];

            foreach (StructureComparison c in comparisons.OrderBy(c => c.Source.Name))
            {
                if (c.Target == null)
                {
                    Console.WriteLine($"Not adding {c.Source.Name} - no target exists");
                    continue;
                }

                targets.Add(new ConceptMap.TargetElementComponent()
                {
                    Code = c.Target.Name,
                    Display = c.Target.Description,
                    Relationship = c.Relationship,
                    Comment = c.Message,
                });
            }

            if (targets.Count == 0)
            {
                group.Element.Add(new()
                {
                    Code = key,
                    Display = $"Complex type {_sourceRLiteral} `{key}` does not exist in {_targetRLiteral} and has no mappings.",
                    NoMap = true,
                });

                continue;
            }

            group.Element.Add(new()
            {
                Code = key,
                Display = $"Complex type {_sourceRLiteral} `{key}` maps to {string.Join(" and ", targets.Select(tm => $"`{tm.Code}`"))}.",
                Target = targets,
            });

        }

        cm.Group.Add(group);

        return cm;
    }


    public ConceptMap? GetSourceResourceTypeConceptMap(
        Dictionary<string, List<StructureComparison>> resources)
    {
        string localConceptMapId = $"{_sourceRLiteral}-resources-{_targetRLiteral}";
        string localUrl = BuildUrl("{0}/{1}/{2}", _mapCanonical, name: localConceptMapId, resourceType: "ConceptMap");

        string sourceUrl = BuildUrl("{0}/{1}/{2}", _sourcePackageCanonical, "ValueSet", "resources");
        string targetUrl = BuildUrl("{0}/{1}/{2}", _targetPackageCanonical, "ValueSet", "resources");

        string sourceCanonical = $"{sourceUrl}|{_sourcePackageVersion}";
        string targetCanonical = $"{targetUrl}|{_targetPackageVersion}";

        // check to see if we need to create a new concept map
        if (!_dc.ConceptMapsByUrl.TryGetValue(localUrl, out ConceptMap? cm))
        {
            cm = new();

            // update our info
            cm.Id = localConceptMapId;
            cm.Url = localUrl;
            cm.Name = localConceptMapId;
            cm.Title = GetConceptMapTitle("resource types");

            cm.SourceScope = new Canonical(sourceCanonical);
            cm.TargetScope = new Canonical(targetCanonical);
        }

        ConceptMap.GroupComponent group = new();

        group.Source = sourceUrl;
        group.Target = targetUrl;

        // traverse resources types
        foreach ((string key, List<StructureComparison> comparisons) in resources.OrderBy(kvp => kvp.Key))
        {
            if (comparisons.Count == 0)
            {
                group.Element.Add(new()
                {
                    Code = key,
                    Display = $"Resource {_sourceRLiteral} `{key}` does not exist in {_targetRLiteral} and has no mappings.",
                    NoMap = true,
                });

                continue;
            }

            List<ConceptMap.TargetElementComponent> targets = [];

            foreach (StructureComparison c in comparisons.OrderBy(c => c.Source.Name))
            {
                if (c.Target == null)
                {
                    Console.WriteLine($"Not adding {c.Source.Name} - no target exists");
                    continue;
                }

                targets.Add(new ConceptMap.TargetElementComponent()
                {
                    Code = c.Target.Name,
                    Display = c.Target.Description,
                    Relationship = c.Relationship,
                    Comment = c.Message,
                });
            }

            if (targets.Count == 0)
            {
                group.Element.Add(new()
                {
                    Code = key,
                    Display = $"Resource {_sourceRLiteral} `{key}` does not exist in {_targetRLiteral} and has no mappings.",
                    NoMap = true,
                });

                continue;
            }

            group.Element.Add(new()
            {
                Code = key,
                Display = $"Resource {_sourceRLiteral} `{key}` maps to {string.Join(" and ", targets.Select(tm => $"`{tm.Code}`"))}.",
                Target = targets,
            });
        }

        cm.Group.Add(group);

        return cm;
    }


    public ConceptMap? TryGetSourceStructureElementConceptMap(
        StructureComparison cRec)
    {
        if (cRec.Target == null)
        {
            Console.WriteLine($"Not writing {cRec.Source.Name} - no target exists");
            return null;
        }

        string sourceName = cRec.Source.Name;
        string targetName = cRec.Target.Name;

        string localConceptMapId = cRec.CompositeName;          // $"{_sourceRLiteral}-{sourceName}-{_targetRLiteral}";
        string localUrl = BuildUrl("{0}/{1}/{2}", _mapCanonical, name: localConceptMapId, resourceType: "ConceptMap");

        string sourceUrl = cRec.Source.Url;
        string targetUrl = cRec.Target.Url;

        string sourceCanonical = $"{sourceUrl}|{_sourcePackageVersion}";
        string targetCanonical = $"{targetUrl}|{_targetPackageVersion}";

        // check to see if we need to create a new concept map
        if (!_dc.ConceptMapsByUrl.TryGetValue(localUrl, out ConceptMap? cm))
        {
            cm = new();

            // update our info
            cm.Id = localConceptMapId;
            cm.Url = localUrl;
            cm.Name = localConceptMapId;
            cm.Title = GetConceptMapTitle(sourceName);

            cm.SourceScope = new Canonical(sourceCanonical);
            cm.TargetScope = new Canonical(targetCanonical);
        }

        ConceptMap.GroupComponent group = new();

        group.Source = cRec.Source.Url;
        group.Target = cRec.Target.Url;

        // traverse elements that exist in our source
        foreach ((string path, ElementComparison elementComparison) in cRec.ElementComparisons.OrderBy(kvp => kvp.Key))
        {
            // put an entry with no map if there is no target
            if (elementComparison.TargetMappings.Count == 0)
            {
                group.Element.Add(new()
                {
                    Code = path,
                    NoMap = true,
                    Display = elementComparison.Source.Short,
                });

                continue;
            }

            group.Element.Add(new()
            {
                Code = path,
                Display = elementComparison.Source.Short,
                Target = elementComparison.TargetMappings.Select(tm => new ConceptMap.TargetElementComponent()
                {
                    Code = tm.Target.Path,
                    Display = tm.Target.Short,
                    Relationship = elementComparison.Relationship,
                    Comment = elementComparison.Message,
                }).ToList(),
            });
        }

        // check for a value set that could not map anything
        if (group.Element.Count == 0)
        {
            Console.WriteLine($"Not writing ConceptMap for {cRec.CompositeName} - no concepts are mapped to target!");
            return null;
        }

        cm.Group.Add(group);

        return cm;
    }

    private string GetConceptMapTitle(string name) => $"Concept map to convert a FHIR {_sourceRLiteral} {name} into FHIR {_targetRLiteral}";

    private string GetConceptMapTitle(string leftName, string rightName) => $"Concept map to convert a FHIR {_sourceRLiteral} {leftName} into a FHIR {_targetRLiteral} {rightName}";

    /// <summary>Applies the canonical format.</summary>
    /// <param name="formatString">The format string.</param>
    /// <param name="canonical">   The canonical.</param>
    /// <param name="name">        The name.</param>
    /// <returns>A string.</returns>
    /// <remarks>
    /// 0 = canonical, 1 = resourceType, 2 = name, 3 = leftRLiteral, 4 = reserved, 5 = rightRLiteral, 6 = reserved
    /// </remarks>
    private string BuildUrl(string formatString, string canonical, string resourceType = "", string name = "") =>
        string.Format(formatString, canonical, resourceType, name, _sourceRLiteral, "", _targetRLiteral, "");

    private string RemoveLeftToRight(string value)
    {
        if (value.EndsWith(_sourceToTargetNoR, StringComparison.Ordinal))
        {
            if (value[^(_sourceToTargetNoRLen + 1)] == '-')
            {
                return value[..^(_sourceToTargetNoRLen + 1)];
            }

            return value[..^_sourceToTargetNoR.Length];
        }

        if (value.EndsWith(_sourceToTargetWithR, StringComparison.OrdinalIgnoreCase))
        {
            if (value[^(_sourceToTargetWithRLen + 1)] == '-')
            {
                return value[..^(_sourceToTargetWithRLen + 1)];
            }

            return value[..^_sourceToTargetWithR.Length];
        }

        return value;
    }

    public string NameFromUrl(string url) => RemoveLeftToRight(url.Split('/', '#')[^1].Split('|')[0]).ToPascalCase();

    public HashSet<string> GetAllReferencedValueSetUrls()
    {
        HashSet<string> set = [];

        foreach ((string left, List<string> rights) in _vsUrlsWithMaps)
        {
            set.Add(left);
            foreach (string right in rights)
            {
                set.Add(right);
            }
        }

        return set;
    }

    public List<string> GetMapTargetsForVs(string sourceUrl)
    {
        if (_vsUrlsWithMaps.TryGetValue(sourceUrl, out List<string>? targets))
        {
            return targets;
        }
        else if (sourceUrl.Contains('|') &&
                 _vsUrlsWithMaps.TryGetValue(sourceUrl.Split('|')[0], out targets))
        {
            return targets;
        }

        return [];
    }

    public List<ConceptMap> GetMapsForSource(string sourceUrl) => _dc.ConceptMapsForSource(sourceUrl);

    public bool TryGetMapsForSource(string sourceUrl, [NotNullWhen(true)] out List<ConceptMap>? maps)
    {
        maps = _dc.ConceptMapsForSource(sourceUrl);
        return maps.Count > 0;
    }

    public ConceptMap? DataTypeMap => _dataTypeMap;

    public ConceptMap? ResourceTypeMap => _resourceTypeMap;

    public ConceptMap? ElementTypeMap(string sdName) => _elementConceptMaps.TryGetValue(sdName, out ConceptMap? cm) ? cm : null;
}
