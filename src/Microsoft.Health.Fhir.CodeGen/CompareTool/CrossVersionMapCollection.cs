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

    private DefinitionCollection _left;
    private DefinitionCollection _right;
    private DefinitionCollection _dc = null!;
    
    private string _mapCanonical = string.Empty;

    private FhirReleases.FhirSequenceCodes _leftFhirSequence;
    private string _leftPackageCanonical;
    private string _leftRLiteral;
    private string _leftShortVersionUrlSegment;
    private string _leftPackageVersion;

    private FhirReleases.FhirSequenceCodes _rightFhirSequence;
    private string _rightPackageCanonical;
    private string _rightRLiteral;
    private string _rightShortVersionUrlSegment;
    private string _rightPackageVersion;

    private string _leftToRightWithR;
    private int _leftToRightWithRLen;
    private string _leftToRightNoR;
    private int _leftToRightNoRLen;

    private Dictionary<string, string> _urlMap = [];

    // dictionary with keys of source value set urls and values of each target vs there is a map for
    private Dictionary<string, List<string>> _vsUrlsWithMaps = [];

    private ConceptMap? _dataTypeMap = null;
    private ConceptMap? _resourceTypeMap = null;
    private Dictionary<string, ConceptMap> _elementConceptMaps = [];


    public CrossVersionMapCollection(
        IFhirPackageClient cache,
        DefinitionCollection left,
        DefinitionCollection right)
    {
        _cache = cache;
        _loader = new(_cache, new()
        {
            JsonModel = LoaderOptions.JsonDeserializationModel.SystemTextJson,
            AutoLoadExpansions = false,
            ResolvePackageDependencies = false,
        });

        _left = left;

        _leftPackageCanonical = left.MainPackageCanonical;
        _leftFhirSequence = left.FhirSequence;
        _leftRLiteral = left.FhirSequence.ToRLiteral();
        _leftShortVersionUrlSegment = "/" + left.FhirSequence.ToShortVersion() + "/";
        _leftPackageVersion = left.FhirVersionLiteral;

        _right = right;

        _rightPackageCanonical = right.MainPackageCanonical;
        _rightFhirSequence = right.FhirSequence;
        _rightRLiteral = right.FhirSequence.ToRLiteral();
        _rightShortVersionUrlSegment = "/" + right.FhirSequence.ToShortVersion() + "/";
        _rightPackageVersion = right.FhirVersionLiteral;

        _leftToRightWithR = $"{_leftRLiteral}to{_rightRLiteral}";
        _leftToRightWithRLen = _leftToRightWithR.Length;
        _leftToRightNoR = $"{_leftRLiteral[1..]}to{_rightRLiteral[1..]}";
        _leftToRightNoRLen = _leftToRightNoR.Length;

        //_mapCanonical = $"http://hl7.org/fhir/uv/xver/{_leftRLiteral.ToLowerInvariant()}-{_rightRLiteral.ToLowerInvariant()}";
        _mapCanonical = BuildUrl("{0}{3}-{5}", _canonicalRootCrossVersion);

        // create our maps collection
        _dc = new()
        {
            Name = "FHIR Cross Version Maps",
            FhirVersion = FHIRVersion.N5_0_0,
            FhirVersionLiteral = "5.0.0",
            FhirSequence = FhirReleases.FhirSequenceCodes.R5,
            MainPackageId = $"hl7.fhir.uv.xver.{_leftRLiteral.ToLowerInvariant()}-{_rightRLiteral.ToLowerInvariant()}",
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
        Console.WriteLine($"Loading fhir-cross-version-source concept maps for conversion from {_leftRLiteral} to {_rightRLiteral}...");

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
        Console.WriteLine($"Loading fhir-cross-version concept maps for conversion from {_leftRLiteral} to {_rightRLiteral}...");

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
        string[] files = Directory.GetFiles(path, $"ConceptMap*-{_leftToRightNoR}.json", SearchOption.TopDirectoryOnly);

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
                                if (_left.TryFindElementByPath(elementPath, out StructureDefinition? sd, out ElementDefinition? ed))
                                {
                                    sourceScopeUrl = ed.Binding?.ValueSet ?? string.Empty;
                                }
                            }

                            if (cm.TargetScope is FhirUri originalTargetUri)
                            {
                                string elementPath = originalTargetUri.Value.Split('#')[^1];
                                if (_right.TryFindElementByPath(elementPath, out StructureDefinition? sd, out ElementDefinition? ed))
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

                            string localConceptMapId = $"{_leftRLiteral}-{leftName}-{_rightRLiteral}-{rightName}";
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
                            cm.SourceScope = new Canonical(unversionedSourceUrl + "|" + _leftPackageVersion);
                            cm.TargetScope = new Canonical(unversionedTargetUrl + "|" + _rightPackageVersion);

                            foreach (ConceptMap.GroupComponent group in cm.Group)
                            {
                                // fix the source and target value set URLs if they have had versions inserted

                                if (group.Source.Contains(_leftShortVersionUrlSegment, StringComparison.Ordinal))
                                {
                                    group.Source = group.Source.Replace(_leftShortVersionUrlSegment, "/");
                                }

                                if (group.Target.Contains(_rightShortVersionUrlSegment, StringComparison.Ordinal))
                                {
                                    group.Target = group.Target.Replace(_rightShortVersionUrlSegment, "/");
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

                            string localConceptMapId = $"{_leftRLiteral}-types-{_rightRLiteral}";
                            string localUrl = BuildUrl("{0}/{1}/{2}", _mapCanonical, name: localConceptMapId, resourceType: "ConceptMap");

                            // update our info
                            cm.Id = localConceptMapId;
                            cm.Url = localUrl;
                            cm.Name = localConceptMapId;
                            cm.Title = GetConceptMapTitle("data type");

                            string sourceLocalUrl = BuildUrl("{0}/{1}/{2}", _leftPackageCanonical, "ValueSet", "data-types");
                            string targetLocalUrl = BuildUrl("{0}/{1}/{2}", _rightPackageCanonical, "ValueSet", "data-types");

                            cm.SourceScope = new Canonical($"{sourceLocalUrl}|{_leftPackageVersion}");
                            cm.TargetScope = new Canonical($"{targetLocalUrl}|{_rightPackageVersion}");

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

                            string localConceptMapId = $"{_leftRLiteral}-resources-{_rightRLiteral}";
                            string localUrl = BuildUrl("{0}/{1}/{2}", _mapCanonical, name: localConceptMapId, resourceType: "ConceptMap");

                            // update our info
                            cm.Id = localConceptMapId;
                            cm.Url = localUrl;
                            cm.Name = localConceptMapId;
                            cm.Title = GetConceptMapTitle("resource type");

                            string sourceLocalUrl = BuildUrl("{0}/{1}/{2}", _leftPackageCanonical, "ValueSet", "resources");
                            string targetLocalUrl = BuildUrl("{0}/{1}/{2}", _rightPackageCanonical, "ValueSet", "resources");

                            cm.SourceScope = new Canonical($"{sourceLocalUrl}|{_leftPackageVersion}");
                            cm.TargetScope = new Canonical($"{targetLocalUrl}|{_rightPackageVersion}");

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

                            string localConceptMapId = $"{_leftRLiteral}-elements-{_rightRLiteral}";
                            string localUrl = BuildUrl("{0}/{1}/{2}", _mapCanonical, name: localConceptMapId, resourceType: "ConceptMap");

                            // update our info
                            cm.Id = localConceptMapId;
                            cm.Url = localUrl;
                            cm.Name = localConceptMapId;
                            cm.Title = GetConceptMapTitle("element");

                            string sourceLocalUrl = BuildUrl("{0}/{1}/{2}", _leftPackageCanonical, "ValueSet", "elements");
                            string targetLocalUrl = BuildUrl("{0}/{1}/{2}", _rightPackageCanonical, "ValueSet", "elements");

                            cm.SourceScope = new Canonical($"{sourceLocalUrl}|{_leftPackageVersion}");
                            cm.TargetScope = new Canonical($"{targetLocalUrl}|{_rightPackageVersion}");

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

                                string elementMapId = $"{_leftRLiteral}-{typeName}-{_rightRLiteral}";
                                string elementMapUrl = BuildUrl("{0}/{1}/{2}", _mapCanonical, name: elementMapId, resourceType: "ConceptMap");

                                string elementSourceUrl = BuildUrl("{0}/{1}/{2}", _leftPackageCanonical, "StructureDefinition", typeName);
                                string elementTargetUrl = elements[0].Target.Count != 0
                                    ? BuildUrl("{0}/{1}/{2}", _rightPackageCanonical, "StructureDefinition", elements[0].Target[0].Code.Split('.')[0])
                                    : BuildUrl("{0}/{1}/{2}", _rightPackageCanonical, "StructureDefinition", typeName);

                                ConceptMap elementMap = new()
                                {
                                    Id = elementMapId,
                                    Url = elementMapUrl,
                                    Name = elementMapId,
                                    Title = GetConceptMapTitle(typeName),
                                    SourceScope = new Canonical($"{elementSourceUrl}|{_leftPackageVersion}"),
                                    TargetScope = new Canonical($"{elementTargetUrl}|{_rightPackageVersion}"),
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

    private string FilenameForMap(CrossVersionMapTypeCodes mapType, string name = "") => mapType switch
    {
        CrossVersionMapTypeCodes.ValueSetConcepts => $"ConceptMap-{_leftRLiteral}-{name}-{_rightRLiteral}.json",
        CrossVersionMapTypeCodes.DataTypeConcepts => $"ConceptMap-{_leftRLiteral}-data-types-{_rightRLiteral}.json",
        CrossVersionMapTypeCodes.ResourceTypeConcepts => $"ConceptMap-{_leftRLiteral}-resource-types-{_rightRLiteral}.json",
        CrossVersionMapTypeCodes.ComplexTypeElementConcepts => $"ConceptMap-{_leftRLiteral}-{name}-{_rightRLiteral}.json",
        CrossVersionMapTypeCodes.ResourceElementConcepts => $"ConceptMap-{_leftRLiteral}-{name}-{_rightRLiteral}.json",
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
        string rootPath = Path.Combine(crossRepoPath, $"{_leftRLiteral}_{_rightRLiteral}", "maps");
        if (!Directory.Exists(rootPath))
        {
            throw new DirectoryNotFoundException($"Could not find {_leftRLiteral}_{_rightRLiteral}/maps directory: {rootPath}");
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
        if (string.IsNullOrEmpty(vsc.SourceUrl) || string.IsNullOrEmpty(vsc.TargetUrl))
        {
            return null;
        }

        if (vsc.ConceptComparisons.Count == 0)
        {
            throw new Exception("Cannot process a comparison with no mappings!");
        }

        string localConceptMapId = $"{_leftRLiteral}-{vsc.SourceName}-{_rightRLiteral}-{vsc.TargetName}";
        string localUrl = BuildUrl("{0}/{1}/{2}", _mapCanonical, name: localConceptMapId, resourceType: "ConceptMap");

        string sourceUrl = vsc.SourceUrl;
        string targetUrl = vsc.TargetUrl;

        string sourceCanonical = $"{sourceUrl}|{_leftPackageVersion}";
        string targetCanonical = $"{targetUrl}|{_rightPackageVersion}";

        // check to see if we need to create a new concept map
        if (!_dc.ConceptMapsByUrl.TryGetValue(localUrl, out ConceptMap? cm))
        {
            cm = new();

            // update our info
            cm.Id = localConceptMapId;
            cm.Url = localUrl;
            cm.Name = localConceptMapId;
            cm.Title = GetConceptMapTitle(vsc.SourceName);

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
            string noTargetKey = $"{sourceSystem}-{primaryTargetSystem}";

            if (cc.TargetMappings.Count == 0)
            {
                if (!groups.TryGetValue(noTargetKey, out ConceptMap.GroupComponent? noTargetGroup))
                {
                    noTargetGroup = new()
                    {
                        Source = sourceUrl,
                        Target = targetUrl,
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
                    : $"{sourceSystem}-{targetSystem}";

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

            foreach ((string targetSystem, List<ConceptMap.TargetElementComponent> targets) in elementTargetsBySystem)
            {
                string key = string.IsNullOrEmpty(targetSystem)
                    ? noTargetKey
                    : $"{sourceSystem}-{targetSystem}";

                if (!groups.TryGetValue(key, out ConceptMap.GroupComponent? group))
                {
                    group = new()
                    {
                        Source = sourceUrl,
                        Target = targetUrl,
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

    public ConceptMap? GetSourceDataTypesConceptMap(
        IEnumerable<ComparisonRecord<StructureInfoRec>> primitiveTypes,
        IEnumerable<ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>> complexTypes)
    {
        string localConceptMapId = $"{_leftRLiteral}-types-{_rightRLiteral}";
        string localUrl = BuildUrl("{0}/{1}/{2}", _mapCanonical, name: localConceptMapId, resourceType: "ConceptMap");

        string sourceUrl = BuildUrl("{0}/{1}/{2}", _leftPackageCanonical, "ValueSet", "data-types");
        string targetUrl = BuildUrl("{0}/{1}/{2}", _rightPackageCanonical, "ValueSet", "data-types");

        string sourceCanonical = $"{sourceUrl}|{_leftPackageVersion}";
        string targetCanonical = $"{targetUrl}|{_rightPackageVersion}";

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
        foreach (ComparisonRecord<StructureInfoRec> c in primitiveTypes.Where(ci => ci.KeyInLeft == true))
        {
            // skip primitives that do not have serialization-based conversion info
            if ((c.TypeSerializationInfo == null) || (c.TypeSerializationInfo.Count == 0))
            {
                continue;
            }

            // add mappings based on primitive serialization info
            group.Element.Add(new()
            {
                Code = c.Key,
                Display = c.Left[0].Description,
                Target = c.TypeSerializationInfo.Values.Where(tsi => tsi.Source == c.Key).Select(tsi => new ConceptMap.TargetElementComponent()
                {
                    Code = tsi.Target,
                    Display = c.Right.Where(si => si.Name == tsi.Target).FirstOrDefault()?.Description ?? $"Primitive type {tsi.Target}",
                    Relationship = tsi.Relationship,
                    Comment = tsi.Message,
                }).ToList(),
            });
        }

        // traverse complex types
        foreach (IComparisonRecord<StructureInfoRec> c in complexTypes.Where(cti => cti.KeyInLeft == true).OrderBy(cti => cti.Key))
        {
            // put an entry with no map if there is no target
            if (c.Right.Count == 0)
            {
                group.Element.Add(new()
                {
                    Code = c.Key,
                    NoMap = true,
                    Display = c.Left[0].Description,
                });

                continue;
            }

            // add mappings based on record info
            group.Element.Add(new()
            {
                Code = c.Key,
                Display = c.Left[0].Description,
                Target = c.Right.Select(target => new ConceptMap.TargetElementComponent()
                {
                    Code = target.Name,
                    Display = target.Description,
                    Relationship = c.Relationship,
                    Comment = c.Message,
                }).ToList(),
            });
        }

        cm.Group.Add(group);

        return cm;
    }


    public ConceptMap? GetSourceResourceTypeConceptMap(
        IEnumerable<ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>> resources)
    {
        string localConceptMapId = $"{_leftRLiteral}-resources-{_rightRLiteral}";
        string localUrl = BuildUrl("{0}/{1}/{2}", _mapCanonical, name: localConceptMapId, resourceType: "ConceptMap");

        string sourceUrl = BuildUrl("{0}/{1}/{2}", _leftPackageCanonical, "ValueSet", "resources");
        string targetUrl = BuildUrl("{0}/{1}/{2}", _rightPackageCanonical, "ValueSet", "resources");

        string sourceCanonical = $"{sourceUrl}|{_leftPackageVersion}";
        string targetCanonical = $"{targetUrl}|{_rightPackageVersion}";

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
        foreach (IComparisonRecord<StructureInfoRec> c in resources.Where(cti => cti.KeyInLeft == true).OrderBy(cti => cti.Key))
        {
            // put an entry with no map if there is no target
            if (c.Right.Count == 0)
            {
                group.Element.Add(new()
                {
                    Code = c.Key,
                    NoMap = true,
                    Display = c.Left[0].Description,
                });

                continue;
            }

            // add mappings based on record info
            group.Element.Add(new()
            {
                Code = c.Key,
                Display = c.Left[0].Description,
                Target = c.Right.Select(target => new ConceptMap.TargetElementComponent()
                {
                    Code = target.Name,
                    Display = target.Description,
                    Relationship = c.Relationship,
                    Comment = c.Message,
                }).ToList(),
            });
        }

        cm.Group.Add(group);

        return cm;
    }


    public ConceptMap? TryGetSourceStructureElementConceptMap(
        ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec> cRec)
    {
        // we cannot write maps that do not have both a source and a target
        if ((cRec.Left.Count == 0) || (cRec.Right.Count == 0))
        {
            return null;
        }

        if (cRec.Left.Count > 1)
        {
            throw new Exception($"Cannot build concept map for structures with more than source: {cRec.Key}");
        }

        string leftName = cRec.Left[0].Name;
        string rightName = cRec.Right[0].Name;

        string localConceptMapId = $"{_leftRLiteral}-{leftName}-{_rightRLiteral}";
        string localUrl = BuildUrl("{0}/{1}/{2}", _mapCanonical, name: localConceptMapId, resourceType: "ConceptMap");

        string sourceUrl = cRec.Left[0].Url;
        string targetUrl = cRec.Right[0].Url;

        string sourceCanonical = $"{sourceUrl}|{_leftPackageVersion}";
        string targetCanonical = $"{targetUrl}|{_rightPackageVersion}";

        // check to see if we need to create a new concept map
        if (!_dc.ConceptMapsByUrl.TryGetValue(localUrl, out ConceptMap? cm))
        {
            cm = new();

            // update our info
            cm.Id = localConceptMapId;
            cm.Url = localUrl;
            cm.Name = localConceptMapId;
            cm.Title = GetConceptMapTitle(leftName);

            cm.SourceScope = new Canonical(sourceCanonical);
            cm.TargetScope = new Canonical(targetCanonical);
        }

        ConceptMap.GroupComponent group = new();

        group.Source = cRec.Left[0].Url;
        group.Target = cRec.Right[0].Url;

        // traverse elements that exist in our source
        foreach ((string elementKey, ComparisonRecord<ElementInfoRec, ElementTypeInfoRec> elementComparison) in cRec.Children.Where(kvp => kvp.Value.KeyInLeft == true).OrderBy(kvp => kvp.Key))
        {
            // put an entry with no map if there is no target
            if (elementComparison.Right.Count == 0)
            {
                group.Element.Add(new()
                {
                    Code = elementKey,
                    NoMap = true,
                    Display = elementComparison.Left[0].Short,
                });

                continue;
            }

            group.Element.Add(new()
            {
                Code = elementKey,
                Display = elementComparison.Left[0].Short,
                Target = elementComparison.Right.Select(target => new ConceptMap.TargetElementComponent()
                {
                    Code = target.Path,
                    Display = target.Short,
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

    private string GetConceptMapTitle(string name) => $"Concept map to convert a FHIR {_leftRLiteral} {name} into FHIR {_rightRLiteral}";

    private string GetConceptMapTitle(string leftName, string rightName) => $"Concept map to convert a FHIR {_leftRLiteral} {leftName} into a FHIR {_rightRLiteral} {rightName}";

    /// <summary>Applies the canonical format.</summary>
    /// <param name="formatString">The format string.</param>
    /// <param name="canonical">   The canonical.</param>
    /// <param name="name">        The name.</param>
    /// <returns>A string.</returns>
    /// <remarks>
    /// 0 = canonical, 1 = resourceType, 2 = name, 3 = leftRLiteral, 4 = reserved, 5 = rightRLiteral, 6 = reserved
    /// </remarks>
    private string BuildUrl(string formatString, string canonical, string resourceType = "", string name = "") =>
        string.Format(formatString, canonical, resourceType, name, _leftRLiteral, "", _rightRLiteral, "");

    private string RemoveLeftToRight(string value)
    {
        if (value.EndsWith(_leftToRightNoR, StringComparison.Ordinal))
        {
            if (value[^(_leftToRightNoRLen + 1)] == '-')
            {
                return value[..^(_leftToRightNoRLen + 1)];
            }

            return value[..^_leftToRightNoR.Length];
        }

        if (value.EndsWith(_leftToRightWithR, StringComparison.OrdinalIgnoreCase))
        {
            if (value[^(_leftToRightWithRLen + 1)] == '-')
            {
                return value[..^(_leftToRightWithRLen + 1)];
            }

            return value[..^_leftToRightWithR.Length];
        }

        return value;
    }

    public string NameFromUrl(string url) => RemoveLeftToRight(url.Split('/', '#')[^1].Split('|')[0]).ToPascalCase();

    //public bool TryConvertUrlOfficialToSource(
    //    string officialUrl,
    //    [NotNullWhen(true)] out string? localUrl,
    //    [NotNullWhen(true)] out string? name,
    //    bool appendCanonicalLeft = false,
    //    bool appendCanonicalRight = false,
    //    string resourceTypeIfMissing = "")
    //{
    //    if (_urlMap.TryGetValue(officialUrl, out localUrl))
    //    {
    //        name = RemoveLeftToRight(localUrl.Split('/')[^1]);

    //        if (appendCanonicalLeft)
    //        {
    //            localUrl += $"|{_leftPackageVersion}";
    //        }

    //        if (appendCanonicalRight)
    //        {
    //            localUrl += $"|{_rightPackageVersion}";
    //        }

    //        return true;
    //    }

    //    if (officialUrl.StartsWith("urn:", StringComparison.Ordinal))
    //    {
    //        localUrl = officialUrl;
    //        name = string.Join('-', officialUrl.Split(':')[^2]).ToPascalCase();
    //        return true;
    //    }

    //    int offset;

    //    if (officialUrl.StartsWith(_canonicalRootHl7, StringComparison.Ordinal))
    //    {
    //        offset = 0;
    //    }
    //    else if (officialUrl.StartsWith(_canonicalRootTHO, StringComparison.Ordinal) ||
    //        officialUrl.StartsWith(_canonicalRootCi, StringComparison.Ordinal))
    //    {
    //        offset = 1;
    //    }
    //    else
    //    {
    //        localUrl = officialUrl;
    //        name = officialUrl.ToPascalCase();
    //        return true;
    //    }

    //    string[] components = officialUrl.Split('/', StringSplitOptions.RemoveEmptyEntries);

    //    // name is always last component from here on out
    //    name = components[^1].Split('|')[0].Split('#')[^1];

    //    //if (components.Length < (5 - offset))
    //    //{
    //    //    throw new ArgumentException($"Invalid official URL: {officialUrl}");
    //    //}

    //    // check for a cross-version concept map url - we can only convert the literals of these if we don't have a mapped value
    //    if ((components.Length == (6 - offset)) &&
    //        (components[3-offset] == "uv") &&
    //        (components[5-offset] == "ConceptMap"))
    //    {
    //        name = name switch
    //        {
    //            "elements" => "Elements",
    //            "resources" => "Resources",
    //            "types" => "DataTypes",
    //            _ => name,
    //        };

    //        localUrl = BuildUrl("{0}/{1}/{2}", _mapCanonical, resourceType: "ConceptMap", name: name);

    //        if (appendCanonicalLeft)
    //        {
    //            localUrl += $"|{_leftPackageVersion}";
    //        }

    //        if (appendCanonicalRight)
    //        {
    //            localUrl += $"|{_rightPackageVersion}";
    //        }

    //        return true;
    //    }

    //    // check for root + version + resource type + name
    //    if ((components.Length == (6 - offset)) &&
    //        ((components[3-offset] == _leftShortVersion) || (components[3-offset] == _rightShortVersion)))
    //    {
    //        localUrl = BuildUrl("{0}/{1}/{2}", _mapCanonical, resourceType: components[4-offset], name: name);

    //        if (appendCanonicalLeft)
    //        {
    //            localUrl += $"|{_leftPackageVersion}";
    //        }

    //        if (appendCanonicalRight)
    //        {
    //            localUrl += $"|{_rightPackageVersion}";
    //        }

    //        return true;
    //    }

    //    // check for root + version + name
    //    if ((components.Length == (5 - offset)) &&
    //        ((components[3-offset] == _leftShortVersion) || (components[3-offset] == _rightShortVersion)))
    //    {
    //        if (string.IsNullOrEmpty(resourceTypeIfMissing))
    //        {
    //            localUrl = BuildUrl("{0}/{2}", _mapCanonical, name: name);
    //        }
    //        else
    //        {
    //            localUrl = BuildUrl("{0}/{1}/{2}", _mapCanonical, resourceType: resourceTypeIfMissing, name: name);
    //        }

    //        if (appendCanonicalLeft)
    //        {
    //            localUrl += $"|{_leftPackageVersion}";
    //        }

    //        if (appendCanonicalRight)
    //        {
    //            localUrl += $"|{_rightPackageVersion}";
    //        }

    //        return true;
    //    }

    //    // check for root + resource + name
    //    if (components.Length == (5 - offset))
    //    {
    //        localUrl = BuildUrl("{0}/{1}/{2}", _mapCanonical, resourceType: components[3 - offset], name: name);

    //        if (appendCanonicalLeft)
    //        {
    //            localUrl += $"|{_leftPackageVersion}";
    //        }

    //        if (appendCanonicalRight)
    //        {
    //            localUrl += $"|{_rightPackageVersion}";
    //        }

    //        return true;
    //    }

    //    // check for root + name
    //    if (components.Length == (4 - offset))
    //    {
    //        if (string.IsNullOrEmpty(resourceTypeIfMissing))
    //        {
    //            localUrl = BuildUrl("{0}/{2}", _mapCanonical, name: name);
    //        }
    //        else
    //        {
    //            localUrl = BuildUrl("{0}/{1}/{2}", _mapCanonical, resourceType: resourceTypeIfMissing, name: name);
    //        }

    //        if (appendCanonicalLeft)
    //        {
    //            localUrl += $"|{_leftPackageVersion}";
    //        }

    //        if (appendCanonicalRight)
    //        {
    //            localUrl += $"|{_rightPackageVersion}";
    //        }

    //        return true;
    //    }


    //    // don't know what this url format is, fail and see if there are more patterns to follow

    //    name = null;
    //    localUrl = null;
    //    return false;
    //}

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

    public List<ConceptMap> GetMapsForVs(string sourceVsUrl) => _dc.ConceptMapsForSource(sourceVsUrl);

    public bool TryGetMapsForVs(string sourceVsUrl, [NotNullWhen(true)] out List<ConceptMap>? maps)
    {
        maps = _dc.ConceptMapsForSource(sourceVsUrl);
        return maps.Count > 0;
    }

    public ConceptMap? DataTypeMap => _dataTypeMap;

    public ConceptMap? ResourceTypeMap => _resourceTypeMap;

    public ConceptMap? ElementTypeMap(string sdName) => _elementConceptMaps.TryGetValue(sdName, out ConceptMap? cm) ? cm : null;
}
