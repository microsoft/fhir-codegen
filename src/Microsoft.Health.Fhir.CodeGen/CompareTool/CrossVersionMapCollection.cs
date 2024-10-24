﻿// <copyright file="CrossVersionMapCollection.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Specification.Navigation;
using Hl7.Fhir.Specification.Source;
using Hl7.Fhir.Utility;
using Microsoft.Health.Fhir.CodeGen.Loader;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.FhirExtensions;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using Microsoft.Health.Fhir.MappingLanguage;
using Hl7.Fhir.FhirPath.Validator;
using Hl7.Fhir.Introspection;
using Hl7.FhirPath;
using System.Text;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;
using Hl7.Fhir.Language.Debugging;
using Firely.Fhir.Packages;

#if NETSTANDARD2_0
using Microsoft.Health.Fhir.CodeGenCommon.Polyfill;
#endif

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

    private DefinitionCollection _source;
    private DefinitionCollection _target;
    private DefinitionCollection _dc = null!;
    
    private string _mapCanonical = string.Empty;

    private FhirReleases.FhirSequenceCodes _sourceFhirSequence;
    private string _sourcePackageCanonical;
    private string _sourceRLiteral;
    private string _sourceShortVersion;
    private string _sourceShortVersionUrlSegment;
    private string _sourcePackageVersion;

    private FhirReleases.FhirSequenceCodes _targetFhirSequence;
    private string _targetPackageCanonical;
    private string _targetRLiteral;
    private string _targetShortVersion;
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
    //private Dictionary<string, ConceptMap> _elementConceptMaps = [];

    private Dictionary<string, FhirStructureMap> _fmlByCompositeName = [];

    public CrossVersionMapCollection(
        DefinitionCollection source,
        DefinitionCollection target)
    {
        _loader = new(new()
        {
            AutoLoadExpansions = false,
            ResolvePackageDependencies = false,
        }, new()
        {
            JsonModel = LoaderOptions.JsonDeserializationModel.SystemTextJson,
        });

        _source = source;

        _sourcePackageCanonical = source.MainPackageCanonical;
        _sourceFhirSequence = source.FhirSequence;
        _sourceRLiteral = source.FhirSequence.ToRLiteral();
        _sourceShortVersion = source.FhirSequence.ToShortVersion();
        _sourceShortVersionUrlSegment = "/" + _sourceShortVersion + "/";
        _sourcePackageVersion = source.FhirVersionLiteral;

        _target = target;

        _targetPackageCanonical = target.MainPackageCanonical;
        _targetFhirSequence = target.FhirSequence;
        _targetRLiteral = target.FhirSequence.ToRLiteral();
        _targetShortVersion = target.FhirSequence.ToShortVersion();
        _targetShortVersionUrlSegment = "/" + _targetShortVersion + "/";
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

    public bool TryLoadCrossVersionMaps(string path)
    {
        bool isOfficial = PathHasFhirCrossVersionOfficial(path);
        bool isSource = PathHasFhirCrossVersionSource(path);

        if (isOfficial && isSource)
        {
            throw new Exception($"Cannot determine style of maps to load from {path}!");
        }

        if (isOfficial)
        {
            if (TryLoadOfficialConceptMaps(path) && TryLoadOfficialStructureMaps(path))
            {
                ReconcileElementMapWithFML();
                return true;
            }

            return false;
        }

        if (isSource)
        {
            return TryLoadSourceConceptMaps(path);
        }

        return false;
    }

    internal bool TryLoadSourceConceptMaps(string crossRepoPath)
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

    internal bool TryLoadOfficialConceptMaps(string fhirCrossRepoPath)
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

    internal bool TryLoadOfficialStructureMaps(string fhirCrossRepoPath)
    {
        Console.WriteLine($"Loading fhir-cross-version structure maps for conversion from {_sourceRLiteral} to {_targetRLiteral}...");

        string path = Path.Combine(fhirCrossRepoPath, "input", _sourceToTargetWithR);
        if (!Directory.Exists(path))
        {
            Console.WriteLine($"Could not find fhir-cross-version/input/{_sourceToTargetWithR} directory: {path}");
            return false;
            //throw new DirectoryNotFoundException($"Could not find fhir-cross-version/input/{_sourceToTargetWithR} directory: {path}");
        }

        // files have different styles in each directory, but we want all FML files anyway
        string[] files = Directory.GetFiles(path, $"*.fml", SearchOption.TopDirectoryOnly);

        FhirMappingLanguage fmlParser = new();

        foreach (string filename in files)
        {
            try
            {
                string fmlContent = File.ReadAllText(filename);

                if (!fmlParser.TryParse(fmlContent, out FhirStructureMap? fml))
                {
                    Console.WriteLine($"Error loading {filename}: could not parse");
                    continue;
                }

                // extract the name root
                string name;

                if (fml.MetadataByPath.TryGetValue("name", out MetadataDeclaration? nameMeta))
                {
                    name = nameMeta.Literal?.ValueAsString ?? throw new Exception($"Cross-version structure maps require a metadata name property: {filename}");
                }
                else
                {
                    name = Path.GetFileNameWithoutExtension(filename);
                }

                if (name.EndsWith(_sourceToTargetNoR, StringComparison.OrdinalIgnoreCase))
                {
                    name = name[..^_sourceToTargetNoRLen];
                }

                if (name.Equals("primitives", StringComparison.OrdinalIgnoreCase))
                {
                    // skip primitive type map - we have that information internally already
                    continue;
                }

                _fmlByCompositeName.Add($"{_sourceRLiteral}-{name}-{_targetRLiteral}-{name}", fml);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading {filename}: {ex.Message}");
            }
        }

        return true;
    }

    /// <summary>Reconcile element map with information from Structure Maps.</summary>
    internal void ReconcileElementMapWithFML()
    {
        // if we do not have any structure maps, nothing to do
        if (_fmlByCompositeName.Count == 0)
        {
            return;
        }

        //StringBuilder addedMaps = new();

        Dictionary<string, Dictionary<string, FmlTargetInfo>> fmlPathLookup = [];

        // iterate over the FML files to see what element maps we can find
        foreach ((string name, FhirStructureMap fml) in _fmlByCompositeName)
        {
            fmlPathLookup.Clear();

            // process each of the groups in the FML to extract path maps
            foreach ((string groupName, GroupDeclaration group) in fml.GroupsByName)
            {
                // process root groups (recurses into dependent groups)
                if (name.Contains(groupName))
                {
                    ProcessCrossVersionGroup(fml, groupName, groupName, group, fmlPathLookup);
                }
            }

            ReconcileElementMapFmlPaths(name, fmlPathLookup);
        }
    }

    private void ReconcileElementMapFmlPaths(
        string fmlName,
        Dictionary<string, Dictionary<string, FmlTargetInfo>> fmlPathLookup)
    {
        StringBuilder addedMaps = new();

        // look for elements that target other elements
        foreach ((string sourcePath, Dictionary<string, FmlTargetInfo> targets) in fmlPathLookup)
        {
            // skip items with no targets
            if (targets.Count == 0)
            {
                continue;
            }

            // set the default relationship based on the number of targets
            ConceptMap.ConceptMapRelationship initialRelationship = targets.Count == 1
                ? ConceptMap.ConceptMapRelationship.Equivalent
                : ConceptMap.ConceptMapRelationship.SourceIsBroaderThanTarget;

            foreach ((string targetPath, FmlTargetInfo targetInfo) in targets)
            {
                // check for source and target paths being the same
                if (sourcePath == targetPath)
                {
                    continue;
                }

                // grab the source and target type info to check for mappings
                string sourceTypeName = sourcePath.Split('.')[0];
                string targetTypeName = targetPath.Split('.')[0];

                // make sure we have a source structure
                if (!_source.TryGetStructure(sourceTypeName, out StructureDefinition? sourceSd))
                {
                    Console.WriteLine($"Could not resolve source type {sourceTypeName} for {sourcePath}");
                    continue;
                }

                // make sure the source element exists
                if (!sourceSd.cgTryGetElementByPath(sourcePath, out ElementDefinition? sourceEd))
                {
                    Console.WriteLine($"Could not resolve source path {sourcePath} for {sourceTypeName}");
                    continue;
                }

                // skip elements that have child elements - will either be picked up by dependent groups or are not relevant
                if (_source.HasChildElements(sourceEd.Path))
                {
                    continue;
                }

                // make sure we have a target structure
                if (!_target.TryGetStructure(sourceTypeName, out StructureDefinition? targetSd))
                {
                    Console.WriteLine($"Could not resolve source type {targetTypeName} for {targetPath}");
                    continue;
                }

                // make sure the target element exists
                if (!targetSd.cgTryGetElementByPath(targetPath, out ElementDefinition? targetEd))
                {
                    Console.WriteLine($"Could not resolve target path {targetPath} for {targetTypeName}");
                    continue;
                }

                // skip elements that have child elements - will either be picked up by dependent groups or are not relevant
                if (_target.HasChildElements(targetEd.Path))
                {
                    continue;
                }

                //// check for a type map we need to apply
                //if ((_dataTypeMap?.Group.FirstOrDefault()?.Element is List<ConceptMap.SourceElementComponent> values) &&
                //    (values.FirstOrDefault(v => v.Code == sourceType) is ConceptMap.SourceElementComponent sourceEC) &&
                //    sourceEC.Target.Any(t => t.Code != sourceEC.Code))
                //{

                //}

                // we want to do a mapping between these elements, check to see if there are any for the correct target
                List<ConceptMap> maps = _dc.ConceptMapsForSource(sourceSd.Url)
                    .Where(cm => ((cm.TargetScope is Canonical tsc) && (tsc.Uri == targetSd.Url)) || ((cm.TargetScope is FhirUri tsu) && (tsu.Value == targetSd.Url)))
                    .ToList();

                ConceptMap elementMap;

                // check if we need to create a map
                if (maps.Count == 0)
                {
                    elementMap = BuildNewElementMap(sourceTypeName, targetTypeName, null);
                    _dc.AddConceptMap(elementMap, _dc.MainPackageId, _dc.MainPackageVersion);
                }
                else
                {
                    elementMap = maps[0];
                }

                ConceptMap.GroupComponent? group = null;

                // traverse groups looking for a matching definition
                foreach (ConceptMap.GroupComponent cmg in elementMap.Group)
                {
                    // check the source and target
                    if ((cmg.Source != sourceSd.Url) ||
                        (cmg.Target != targetSd.Url))
                    {
                        continue;
                    }

                    group = cmg;
                    break;
                }

                if (group == null)
                {
                    group = new()
                    {
                        SourceElement = new Canonical(sourceSd.Url, sourceSd.Version, null),
                        TargetElement = new Canonical(targetSd.Url, targetSd.Version, null),
                    };

                    elementMap.Group.Add(group);
                }

                ConceptMap.SourceElementComponent? groupElement = null;

                // check to see if this path exists within the group
                foreach (ConceptMap.SourceElementComponent existingGroupElement in group.Element)
                {
                    // check the path
                    if (existingGroupElement.Code != sourceEd.Path)
                    {
                        continue;
                    }

                    groupElement = existingGroupElement;
                    break;
                }

                if (groupElement == null)
                {
                    groupElement = new()
                    {
                        Code = sourceEd.Path,
                        Target = [],
                    };
                    group.Element.Add(groupElement);
                }

                ConceptMap.TargetElementComponent? targetElement = null;

                // check if this target exists
                foreach (ConceptMap.TargetElementComponent te in groupElement.Target)
                {
                    // check the path
                    if (te.Code != targetEd.Path)
                    {
                        continue;
                    }

                    targetElement = te;
                    break;
                }

                if (targetElement == null)
                {
                    // default everything to equivalent in this step - will restrict based on types later
                    targetElement = new()
                    {
                        Code = targetEd.Path,
                        Relationship = ConceptMap.ConceptMapRelationship.Equivalent,    // RelationshipForFmlTarget(targetInfo),
                        Comment = $"Discovered map via FML {fmlName}: {targetInfo.FhirMappingExpression.RawText}",
                    };

                    addedMaps.AppendLine($"FML map: {sourceEd.Path}:{targetEd.Path} - {targetInfo.FhirMappingExpression.RawText}");

                    // ensure the concept map has everything
                    groupElement.Target.Add(targetElement);
                }
            }
        }

        if (addedMaps.Length > 0)
        {
            Console.WriteLine($"Element maps added from FML for {fmlName}:");
            Console.WriteLine(addedMaps.ToString());
        }
    }

    private ConceptMap.ConceptMapRelationship? RelationshipForFmlTarget(FmlTargetInfo ti)
    {
        if (ti.IsSimpleCopy)
        {
            return ConceptMap.ConceptMapRelationship.Equivalent;
        }

        return ConceptMap.ConceptMapRelationship.RelatedTo;
    }

    public record class FmlTargetInfo
    {
        public required GroupExpression FhirMappingExpression { get; init; }
        public bool IsSimpleCopy { get; init; } = false;
        public bool HasTransform { get; init; } = false;
        public string TransformName { get; init; } = string.Empty;
        public string TranslateReference {  get; init; } = string.Empty;
        public string TranslateType {  get; init; } = string.Empty;
        public bool IsComplexTransform { get; init; } = false;
    }

    public static void ProcessCrossVersionFml(string name, FhirStructureMap fml, Dictionary<string, Dictionary<string, FmlTargetInfo>> fmlPathLookup)
    {
        if (fml.GroupsByName.Count == 0)
        {
            throw new Exception($"Cannot process FML for {name} - no groups found!");
        }

        if (!fml.GroupsByName.TryGetValue(name, out GroupDeclaration? group) && !name.Equals("primitives", StringComparison.OrdinalIgnoreCase))
        {
            throw new Exception($"Cannot process FML for {name} - group {name} is not found");
        }

        if (group != null)
        {
            ProcessCrossVersionGroup(fml, name, name, group, fmlPathLookup);
        }
    }

    private static void ReportIssue(List<OperationOutcome.IssueComponent> issues, string message, OperationOutcome.IssueType code, OperationOutcome.IssueSeverity severity = OperationOutcome.IssueSeverity.Error)
    {
        issues.Add(new OperationOutcome.IssueComponent()
        {
            Code = code,
            Severity = severity,
            Details = new CodeableConcept() { Text = message }
        });
        Console.WriteLine($"\n{severity.GetDocumentation()}: {message}");
    }

    private static void ProcessCrossVersionGroup(
        FhirStructureMap fml,
        string sourcePrefix,
        string targetPrefix,
        GroupDeclaration group,
        Dictionary<string, Dictionary<string, FmlTargetInfo>> fmlPathLookup, IEnumerable<string>? dependentGroupCallStack = null)
    {
        string groupSourceVar = string.Empty;
        string groupTargetVar = string.Empty;

        if (sourcePrefix.Length > 2048 || targetPrefix.Length > 2048)
        {
            // A safety check on missing recursive definitions...
            System.Diagnostics.Trace.WriteLine($"{fml.MapDirective?.Url ?? fml.MetadataByPath["url"]?.Literal?.Value} {group.Name} Path likely in a recursive loop {sourcePrefix} -> {targetPrefix}");
            throw new ApplicationException($"Path likely in a recursive loop {sourcePrefix} -> {targetPrefix}");
        }

        // parse out the source and target names from the group
        foreach (GroupParameter gp in group.Parameters)
        {
            switch (gp.InputMode)
            {
                case StructureMap.StructureMapInputMode.Source:
                    groupSourceVar = gp.Identifier;
                    break;

                case StructureMap.StructureMapInputMode.Target:
                    groupTargetVar = gp.Identifier;
                    break;
            }
        }

        if (string.IsNullOrEmpty(groupSourceVar) || string.IsNullOrEmpty(groupTargetVar))
        {
            throw new Exception("Failed to parse group parameters");
        }

        int groupSourceVarLen = groupSourceVar.Length;
        int groupTargetVarLen = groupTargetVar.Length;

        // iterate over expressions in this group
        foreach (GroupExpression exp in group.Expressions)
        {
            if (exp.SimpleCopyExpression != null)
            {
                string sourceName;

                if (exp.SimpleCopyExpression.Source.StartsWith(groupSourceVar, StringComparison.Ordinal))
                {
                    if (exp.SimpleCopyExpression.Source.Length == groupSourceVarLen)
                    {
                        sourceName = string.Empty;
                    }
                    else
                    {
                        // add our current name prefix
                        sourceName = exp.SimpleCopyExpression.Source[(groupSourceVarLen + 1)..];
                    }
                }
                else
                {
                    sourceName = exp.SimpleCopyExpression.Source;
                }

                string targetName;

                if (exp.SimpleCopyExpression.Target == null)
                {
                    targetName = string.Empty;
                }
                else if (exp.SimpleCopyExpression.Target.StartsWith(groupTargetVar, StringComparison.Ordinal))
                {
                    if (exp.SimpleCopyExpression.Target.Length == groupTargetVarLen)
                    {
                        targetName = string.Empty;
                    }
                    else
                    {
                        targetName = exp.SimpleCopyExpression.Target[(groupTargetVarLen + 1)..];
                    }
                }
                else
                {
                    targetName = exp.SimpleCopyExpression.Target;
                }

                // add our current name prefix
                sourceName = $"{sourcePrefix}.{sourceName}";
                targetName = $"{targetPrefix}.{targetName}";

                if (!fmlPathLookup.TryGetValue(sourceName, out Dictionary<string, FmlTargetInfo>? expressionsByTarget))
                {
                    expressionsByTarget = [];
                    fmlPathLookup.Add(sourceName, expressionsByTarget);
                }

                expressionsByTarget.Add(targetName, new()
                {
                    FhirMappingExpression = exp,
                    IsSimpleCopy = true,
                });

                continue;
            }

            if (exp.MappingExpression != null)
            {
                foreach (FmlExpressionSource source in exp.MappingExpression.Sources)
                {
                    string ruleSourcePrefix = source.Identifier.Split('.')[0];

                    if (ruleSourcePrefix != groupSourceVar)
                    {
                        // skip elements that do not start with our matching variable
                        continue;
                    }

                    string sourceName;

                    if (source.Identifier.StartsWith(groupSourceVar, StringComparison.Ordinal))
                    {
                        if (source.Identifier.Length == groupSourceVarLen)
                        {
                            sourceName = string.Empty;
                        }
                        else
                        {
                            // add our current name prefix
                            sourceName = source.Identifier[(groupSourceVarLen + 1)..];
                        }
                    }
                    else
                    {
                        sourceName = source.Identifier;
                    }

                    // add our current name prefix
                    sourceName = $"{sourcePrefix}.{sourceName}";

                    if (!fmlPathLookup.TryGetValue(sourceName, out Dictionary<string, FmlTargetInfo>? expressionsByTarget))
                    {
                        expressionsByTarget = [];
                        fmlPathLookup.Add(sourceName, expressionsByTarget);
                    }

                    foreach (FmlExpressionTarget target in exp.MappingExpression.Targets)
                    {
                        string targetName;

                        if (target.Identifier == null)
                        {
                            targetName = string.Empty;
                        }
                        else if (target.Identifier.StartsWith(groupTargetVar, StringComparison.Ordinal))
                        {
                            if (target.Identifier.Length == groupTargetVarLen)
                            {
                                targetName = string.Empty;
                            }
                            else
                            {
                                targetName = target.Identifier[(groupTargetVarLen + 1)..];
                            }
                        }
                        else
                        {
                            targetName = target.Identifier;
                        }

                        // add our current name prefix
                        targetName = string.IsNullOrEmpty(targetName) ? targetPrefix : $"{targetPrefix}.{targetName}";

                        string transformName = target.Transform?.Invocation?.Identifier ?? string.Empty;

                        switch (transformName)
                        {
                            case "translate":
                                expressionsByTarget[targetName] = new()
                                {
                                    FhirMappingExpression = exp,
                                    HasTransform = target.Transform != null,
                                    TransformName = transformName,
                                    TranslateReference = ((target.Transform?.Invocation?.Parameters.Count ?? 0) > 1)
                                        ? target.Transform!.Invocation!.Parameters[1]!.Literal?.ValueAsString ?? string.Empty
                                        : string.Empty,
                                    TranslateType = ((target.Transform?.Invocation?.Parameters.Count ?? 0) > 2)
                                        ? target.Transform!.Invocation!.Parameters[2]!.Literal?.ValueAsString ?? string.Empty
                                        : string.Empty,
                                };
                                break;

                            default:
                                //if (!string.IsNullOrEmpty(transformName))
                                //{
                                //    Console.Write("");
                                //}
                                expressionsByTarget[targetName] = new()
                                {
                                    FhirMappingExpression = exp,
                                    HasTransform = target.Transform != null,
                                    TransformName = transformName,
                                    IsComplexTransform = !string.IsNullOrEmpty(transformName),
                                };
                                break;
                        }

                        // try to nest into a dependent expression if there is one
                        if (exp.MappingExpression.DependentExpression != null)
                        {
                            foreach (FmlInvocation dependentInvocation in exp.MappingExpression.DependentExpression.Invocations)
                            {
                                string fnName = dependentInvocation.Identifier;
                                if (fnName != group.Name && fml.GroupsByName.TryGetValue(fnName, out GroupDeclaration? dependentGroup))
                                {
                                    if (dependentGroupCallStack == null || !dependentGroupCallStack.Contains(fnName))
                                    {
                                        var newStack = dependentGroupCallStack?.Append(fnName).ToArray() ?? [fnName];
                                        ProcessCrossVersionGroup(fml, sourceName, targetName, dependentGroup, fmlPathLookup, newStack);
                                    }
                                }
                            }
                        }
                    }

                }

                continue;
            }

            Console.Write("");

        }
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
                object? loaded = _loader.ParseContentsSystemTextStream("fhir+json", typeof(ConceptMap), path: filename);
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
                                // skip any empty maps
                                Console.WriteLine($"Invalid concept map {filename}: expected 1 group, found {cm.Group.Count}");
                                continue;
                                //throw new Exception($"Invalid concept map {filename}: expected 1 group, found {cm.Group.Count}");
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

                                // official sources are sometimes incorrect, listing the same code multiple times instead of multiple targets
                                Dictionary<string, ConceptMap.SourceElementComponent> elementDict = [];
                                foreach (ConceptMap.SourceElementComponent element in elements)
                                {
                                    if (elementDict.TryGetValue(element.Code, out ConceptMap.SourceElementComponent? reconciled))
                                    {
                                        // add our targets to this element
                                        reconciled.Target ??= [];
                                        reconciled.Target.AddRange(reconciled.Target);
                                    }
                                    else
                                    {
                                        // add our element to the dictionary
                                        elementDict.Add(element.Code, element);
                                    }
                                }

                                ConceptMap elementMap = BuildNewElementMap(
                                    typeName,
                                    elements[0].Target.Count == 0 ? typeName : elements[0].Target[0].Code.Split('.')[0],
                                    elementDict.Values.OrderBy(se => se.Code).ToList());

                                //_elementConceptMaps.Add(typeName, elementMap);
                                _dc.AddConceptMap(elementMap, _dc.MainPackageId, _dc.MainPackageVersion);
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

    private ConceptMap BuildNewElementMap(string sourceTypeName, string targetTypeName, List<ConceptMap.SourceElementComponent>? elements)
    {
        string elementMapId = $"{_sourceRLiteral}-{sourceTypeName}-{_targetRLiteral}-{targetTypeName}";
        string elementMapUrl = BuildUrl("{0}/{1}/{2}", _mapCanonical, name: elementMapId, resourceType: "ConceptMap");

        string elementSourceUrl = BuildUrl("{0}/{1}/{2}", _sourcePackageCanonical, "StructureDefinition", sourceTypeName);
        string elementTargetUrl = BuildUrl("{0}/{1}/{2}", _targetPackageCanonical, "StructureDefinition", targetTypeName);

        return new()
        {
            Id = elementMapId,
            Url = elementMapUrl,
            Name = elementMapId,
            Title = GetConceptMapTitle(sourceTypeName),
            SourceScope = new Canonical($"{elementSourceUrl}|{_sourcePackageVersion}"),
            TargetScope = new Canonical($"{elementTargetUrl}|{_targetPackageVersion}"),
            Group = [new ConceptMap.GroupComponent
            {
                Source = elementSourceUrl,
                Target = elementTargetUrl,
                Element = elements,
            }],
        };
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
                object? loaded = _loader.ParseContentsSystemTextStream("fhir+json", typeof(ConceptMap), path: filename);
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

                            //_elementConceptMaps.Add(leftName, cm);
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

                            //_elementConceptMaps.Add(leftName, cm);
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
        if (string.IsNullOrEmpty(vsc.Source.Url) || string.IsNullOrEmpty(vsc.Target?.Url))
        {
            return null;
        }

        if (vsc.ConceptComparisons.Count == 0)
        {
            throw new Exception("Cannot process a comparison with no mappings!");
        }

        string localConceptMapId = $"{_sourceRLiteral}-{vsc.Source.NamePascal}-{_targetRLiteral}-{vsc.Target!.NamePascal}";
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
            .SelectMany(cc => cc.TargetMappings.Select(t => t.Target?.System ?? string.Empty))
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
                string targetSystem = targetMapping.Target?.System ?? primaryTargetSystem;
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
                    Code = targetMapping.Target?.Code ?? string.Empty,
                    Display = targetMapping.Target?.Description ?? string.Empty,
                    Relationship = targetMapping.Relationship,
                    Comment = targetMapping.Message,
                });
            }

            foreach ((string key, List<ConceptMap.TargetElementComponent> targets) in elementTargetsBySystem)
            {
                if (!groups.TryGetValue(key, out ConceptMap.GroupComponent? group))
                {
                    int loc = key.IndexOf("||");

                    if (loc == -1)
                    {
                        throw new Exception($"Invalid key: {key}");
                    }

                    group = new()
                    {
                        Source = key[..loc],
                        Target = key[(loc + 2)..],
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
                    //Console.WriteLine($"Not adding {c.SourceTypeLiteral} - no target exists");
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
                    //Console.WriteLine($"Not adding {c.Source.Name} - no target exists");
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
            //Console.WriteLine($"Not writing {cRec.Source.Name} - no target exists");
            return null;
        }

        string sourceName = cRec.Source.Name;
        string targetName = cRec.Target.Name;

        string localConceptMapId = cRec.CompositeName;          // $"{_sourceRLiteral}-{sourceName}-{_targetRLiteral}";
        string localUrl = BuildUrl("{0}/{1}/{2}", _mapCanonical, name: localConceptMapId, resourceType: "ConceptMap");

        // check to see if we need to create a new concept map
        if (!_dc.ConceptMapsByUrl.TryGetValue(localUrl, out ConceptMap? cm))
        {
            cm = BuildNewElementMap(sourceName, targetName, null);
        }

        ConceptMap.GroupComponent? group = null;

        // traverse groups looking for a matching definition
        foreach (ConceptMap.GroupComponent cmg in cm.Group)
        {
            // check the source and target
            if ((cmg.Source != cRec.Source.Url) ||
                (cmg.Target != cRec.Target.Url))
            {
                continue;
            }

            group = cmg;
            break;
        }

        if (group == null)
        {
            group = new()
            {
                SourceElement = new Canonical(cRec.Source.Url, _sourcePackageVersion, null),
                TargetElement = new Canonical(cRec.Target.Url, _targetPackageVersion, null),
            };

            cm.Group.Add(group);
        }

        // build a dictionary of the existing paths
        Dictionary<string, ConceptMap.SourceElementComponent> groupElements = group.Element.ToDictionary(e => e.Code);

        // traverse elements that exist in our source
        foreach ((string path, ElementComparison elementComparison) in cRec.ElementComparisons.OrderBy(kvp => kvp.Key))
        {
            // get or create the source element mapping
            if (!groupElements.TryGetValue(path, out ConceptMap.SourceElementComponent? groupElement))
            {
                groupElement = new()
                {
                    Code = path,
                    Display = elementComparison.Source.Short,
                };

                groupElements.Add(path, groupElement);
            }

            // set no-map if there is no target
            if (elementComparison.TargetMappings.Count == 0)
            {
                groupElement.NoMap = true;
                continue;
            }

            // get or create a target
            groupElement.Target ??= [];

            // just traverse the list since there are typically N<2 elements
            foreach (ElementComparisonDetails tm in elementComparison.TargetMappings)
            {
                string targetPath = tm.Target?.Path ?? path;
                ConceptMap.TargetElementComponent? targetElement = null;

                // check if this target exists
                foreach (ConceptMap.TargetElementComponent te in groupElement.Target)
                {
                    // check the path
                    if (te.Code != targetPath)
                    {
                        continue;
                    }

                    targetElement = te;
                    break;
                }

                if (targetElement == null)
                {
                    targetElement = new()
                    {
                        Code = targetPath,
                    };

                    groupElement.Target.Add(targetElement);
                }

                // ensure the target element has all the properties we want
                targetElement.Display = tm.Target?.Short ?? string.Empty;
                targetElement.Relationship = tm.Relationship;
                targetElement.Comment = tm.Message;
            }
        }

        // reset the group to have the elements we computed
        group.Element = groupElements.Values.OrderBy(se => se.Code).ToList();

        // check for a value set that could not map anything
        if (group.Element.Count == 0)
        {
            //Console.WriteLine($"Not writing ConceptMap for {cRec.CompositeName} - no concepts are mapped to target!");
            return null;
        }

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
    /// 0 = canonical, 1 = resourceType, 2 = name, 3 = leftRLiteral, 4 = leftShortVersion, 5 = rightRLiteral, 6 = rightShortVersion
    /// </remarks>
    private string BuildUrl(string formatString, string canonical, string resourceType = "", string name = "") =>
        string.Format(formatString, canonical, resourceType, name, _sourceRLiteral, _sourceShortVersion, _targetRLiteral, _targetShortVersion);

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

    public void UpdateDataTypeMap(Dictionary<string, List<PrimitiveTypeComparison>> primitiveTypes)
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

        if (cm.Group.Count == 0)
        {
            cm.Group.Add(new()
            {
                Source = sourceUrl,
                Target = targetUrl,
            });
        }

        ConceptMap.GroupComponent group = cm.Group[0];

        HashSet<string> updatedTypes = [];

        // traverse our group and update any primitive we find
        foreach (ConceptMap.SourceElementComponent se in group.Element)
        {
            if (primitiveTypes.TryGetValue(se.Code, out List<PrimitiveTypeComparison>? comparisons))
            {
                updatedTypes.Add(se.Code);

                if (comparisons.Count == 0)
                {
                    se.NoMap = true;
                    se.Display = $"Primitive type {_sourceRLiteral} `{se.Code}` does not exist in {_targetRLiteral} and has no mappings.";
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
                    se.NoMap = true;
                    se.Display = $"Primitive type {_sourceRLiteral} `{se.Code}` does not exist in {_targetRLiteral} and has no mappings.";
                }
                else
                {
                    se.Target = targets;
                }
            }
        }

        // traverse primitive types to add any missing records
        foreach ((string key, List<PrimitiveTypeComparison> comparisons) in primitiveTypes.OrderBy(kvp => kvp.Key))
        {
            if (updatedTypes.Contains(key))
            {
                continue;
            }

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
    }


    public void UpdateDataTypeMap(Dictionary<string, List<StructureComparison>> complexTypes)
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

        if (cm.Group.Count == 0)
        {
            cm.Group.Add(new()
            {
                Source = sourceUrl,
                Target = targetUrl,
            });
        }

        ConceptMap.GroupComponent group = cm.Group[0];

        HashSet<string> updatedTypes = [];

        // traverse our group and update any primitive we find
        foreach (ConceptMap.SourceElementComponent se in group.Element)
        {
            if (complexTypes.TryGetValue(se.Code, out List<StructureComparison>? comparisons))
            {
                updatedTypes.Add(se.Code);

                if (comparisons.Count == 0)
                {
                    se.NoMap = true;
                    se.Display = $"Complex type {_sourceRLiteral} `{se.Code}` does not exist in {_targetRLiteral} and has no mappings.";
                    continue;
                }

                List<ConceptMap.TargetElementComponent> targets = [];

                foreach (StructureComparison c in comparisons.OrderBy(c => c.Target?.Name ?? c.CompositeName))
                {
                    if (c.Target == null)
                    {
                        Console.WriteLine($"Not adding {c.CompositeName} - no target exists");
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
                    se.NoMap = true;
                    se.Display = $"Complex type {_sourceRLiteral} `{se.Code}` does not exist in {_targetRLiteral} and has no mappings.";
                }
                else
                {
                    se.Target = targets;
                }
            }
        }
        
        // traverse complex types to add any missing records
        foreach ((string key, List<StructureComparison> comparisons) in complexTypes.OrderBy(kvp => kvp.Key))
        {
            if (updatedTypes.Contains(key))
            {
                continue;
            }

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

            foreach (StructureComparison c in comparisons.OrderBy(c => c.Target?.Name ?? c.CompositeName))
            {
                if (c.Target == null)
                {
                    Console.WriteLine($"Not adding {c.CompositeName} - no target exists");
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
    }

    public static async Task<OperationOutcome> CheckFmlForMissingProperties(FhirStructureMap fml, CrossVersionCheckOptions options)
    {
        Console.WriteLine($"Checking map {fml.MapDirective?.Url ?? fml.MetadataByPath["url"]?.Literal?.ValueAsString} for missing properties");

        // scan the uses types
        Dictionary<string, StructureDefinition?> _aliasedTypes = new();
        foreach (var use in fml.StructuresByUrl)
        {
            // Console.WriteLine($"Use {use.Key} as {use.Value?.Alias}");
            var sd = await options.resolveMapUseCrossVersionType(use.Key.Trim('\"'), use.Value?.Alias);
            if (use.Value?.Alias != null)
                _aliasedTypes.Add(use.Value.Alias, sd);
            else if (sd != null && sd.Name != null)
                _aliasedTypes.Add(use.Value?.Alias ?? sd.Name, sd);
        }

        // scan all the groups
        OperationOutcome outcome = new OperationOutcome();
        foreach (var group in fml.GroupsByName.Values)
        {
            var results = CheckFmlForMissingPropertiesInGroup(fml, group, _aliasedTypes, options);
            outcome.Issue.AddRange(results);
        }
        Console.WriteLine();
        return outcome;
    }

    private static List<OperationOutcome.IssueComponent> CheckFmlForMissingPropertiesInGroup(
            FhirStructureMap fml,
            GroupDeclaration group,
            Dictionary<string, StructureDefinition?> _aliasedTypes, CrossVersionCheckOptions options
            )
    {
        List<OperationOutcome.IssueComponent> issues = new List<OperationOutcome.IssueComponent>();
        Console.Write($"  {group.Name}(");

        // Check the types in the group parameters
        Dictionary<string, PropertyOrTypeDetails?> parameterTypesByName = new();
        foreach (var gp in group.Parameters)
        {
            if (gp != group.Parameters.First())
                Console.Write(", ");
            PropertyOrTypeDetails? tp = null;
            string? type = gp.TypeIdentifier;
            // lookup the type in the aliases
            var resolver = gp.InputMode == StructureMap.StructureMapInputMode.Source ? options.source.Resolver : options.target.Resolver;
            if (type != null)
            {
                if (!type.Contains('/') && _aliasedTypes.ContainsKey(type))
                {
                    var sd = _aliasedTypes[type];
                    if (sd != null)
                    {
                        var sw = new FmlStructureDefinitionWalker(sd, resolver);
                        tp = new PropertyOrTypeDetails(sw.Current.Path, sw.Current, resolver);
                        type = $"{sd.Url}|{sd.Version}";
                        gp.ParameterElementDefinition = sw.Current;
                    }
                    else
                    {
                        string msg = $"Group {group.Name} parameter {gp.Identifier} type `{gp.TypeIdentifier}` is not imported in a use at @{gp.Line}:{gp.Column}";
                        ReportIssue(issues, msg, OperationOutcome.IssueType.NotFound);
                    }
                }
                else
                {
                    tp = FmlValidator.ResolveDataTypeFromName(group, resolver, issues, gp, type);
                    if (tp != null)
                    {
                        gp.ParameterElementDefinition = tp.Element;
                    }
                    else
                    {
                        string msg = $"Group {group.Name} parameter {gp.Identifier} has no type `{gp.TypeIdentifier}` at @{gp.Line}:{gp.Column}";
                        ReportIssue(issues, msg, OperationOutcome.IssueType.NotFound);
                    }
                }
            }
            else if (gp.ParameterElementDefinition != null)
            {
                tp = new PropertyOrTypeDetails(gp.ParameterElementDefinition.Path, gp.ParameterElementDefinition, resolver);
            }
            parameterTypesByName.Add(gp.Identifier, tp);
            Console.Write($" {gp.Identifier}");
            if (gp.ParameterElementDefinition != null)
                Console.Write($" : {gp.ParameterElementDefinition.DebugString()}");
            else
                Console.Write($" : {type ?? "?"}");
        }
        Console.Write(" )\n  {\n");

        // Call this routine to compare the 2 structures and get some results to check if the map has all this inside!
        var sourceSD = group.Parameters.FirstOrDefault(gp => gp.InputMode == StructureMap.StructureMapInputMode.Source)?.ParameterElementDefinition;
        var targetSD = group.Parameters.FirstOrDefault(gp => gp.InputMode == StructureMap.StructureMapInputMode.Target)?.ParameterElementDefinition;
        StructureComparison? comparison = null;
        if (group.Parameters.Count == 2 && sourceSD?.StructureDefinition != null && targetSD?.StructureDefinition != null)
        {
            var config = new Configuration.ConfigCompare()
            {

            };
            PackageComparer pc = new PackageComparer(config, options.SourcePackage, options.TargetPackage);
            if (pc.TryCompareStructureElements(sourceSD.StructureDefinition, targetSD.StructureDefinition, null, out StructureComparison? directComparison))
            {
                // review the direct comparison
                // Console.WriteLine($"{directComparison.CompositeName}");
                comparison = directComparison;

                // Trim down the element comparisons to only the same level in the type
                if (sourceSD != null && !string.IsNullOrEmpty(sourceSD.Path))
                {
                    var excludeKeys = comparison.ElementComparisons.Keys.Where(k => !k.StartsWith($"{sourceSD.Path}.") || k.Substring(sourceSD.Path.Length + 1).Contains('.') || k.EndsWith(".id") || k.EndsWith(".extension") || k.EndsWith(".modifierExtension")).ToArray();
                    foreach (var key in excludeKeys) // comparison.ElementComparisons.ContainsKey(sourceSD?.Path))
                    {
                        // remove this element
                        comparison.ElementComparisons.Remove(key);
                    }
                    if (!sourceSD.Path.Contains('.'))
                    {
                        // also remove any DomainResource/Resource base level properties
                        string[] excludeProperties = Array.Empty<string>();
                        if (group.ExtendsIdentifier == "Resource" || group.ExtendsIdentifier == "DomainResource")
                        {
                            excludeProperties = new[]
                            {
                                $"{sourceSD.Path}.meta",
                                $"{sourceSD.Path}.implicitRules",
                                $"{sourceSD.Path}.language",
                                $"{sourceSD.Path}.text",
                                $"{sourceSD.Path}.contained"
                            };
                        }

                        if (group.ExtendsIdentifier == "Quantity")
                        {
                            excludeProperties = new[]
                            {
                                $"{sourceSD.Path}.value",
                                $"{sourceSD.Path}.comparator",
                                $"{sourceSD.Path}.unit",
                                $"{sourceSD.Path}.system",
                                $"{sourceSD.Path}.code"
                            };
                        }

                        excludeKeys = comparison.ElementComparisons.Keys.Where(k => excludeProperties.Contains(k)).ToArray();
                        foreach (var key in excludeKeys) // comparison.ElementComparisons.ContainsKey(sourceSD?.Path))
                        {
                            // remove this element
                            comparison.ElementComparisons.Remove(key);
                        }
                    }
                    // Remove any properties in the comparisons that indicate there is no target for the property
                    excludeKeys = comparison.ElementComparisons.Where(ec => !ec.Value.TargetMappings.Any()).Select(kvp => kvp.Key).ToArray();
                    foreach (var key in excludeKeys) // comparison.ElementComparisons.ContainsKey(sourceSD?.Path))
                    {
                        // remove this element
                        comparison.ElementComparisons.Remove(key);
                    }
                }
            }
        }


        // Check if the group extends any other groups
        if (!string.IsNullOrEmpty(group.ExtendsIdentifier))
        {
            // Check that the named group exists
            if (!options.namedGroups.ContainsKey(group.ExtendsIdentifier!))
            {
                string msg = $"Unable to extends group `{group.ExtendsIdentifier}` in {group.Name} at @{group.Line}:{group.Column}";
                ReportIssue(issues, msg, OperationOutcome.IssueType.Duplicate);
            }

            // Check that the parameter values are compatible...

        }

        // Now scan for dependencies in rules
        foreach (var rule in group.Expressions)
        {
            CheckFmlForMissingPropertiesGroupRule("     ", fml, group, _aliasedTypes, options, issues, parameterTypesByName, rule, comparison);
        }

        // now check if there are any properties left over...
        if (comparison != null)
        {
            if (comparison.ElementComparisons.Any())
            {
                Console.Write("Warning: Elements were not mapped for this type\n");
                foreach (var comparisonElement in comparison.ElementComparisons)
                {
                    // Console.Write($"    {comparisonElement.Key} -> {String.Join(", ", comparisonElement.Value.TargetMappings.Select(tm => tm.Target?.Path))} // {comparisonElement.Value.Message}\n");
                    ReportIssue(issues, $"{comparisonElement.Key} -> {String.Join(", ", comparisonElement.Value.TargetMappings.Select(tm => tm.Target?.Path))} // {comparisonElement.Value.Message}", OperationOutcome.IssueType.Incomplete, OperationOutcome.IssueSeverity.Warning);
                    GenerateMapRule(issues, comparisonElement, parameterTypesByName, group);
                }
            }
        }
        Console.WriteLine("  }");

        return issues;
    }

    private static void GenerateMapRule(List<OperationOutcome.IssueComponent> issues, KeyValuePair<string, ElementComparison> comparisonElement, Dictionary<string, PropertyOrTypeDetails?> parameterTypesByName, GroupDeclaration group)
    {
        foreach (var tm in comparisonElement.Value.TargetMappings)
        {
            var sourceProp = $"{parameterTypesByName.FirstOrDefault().Key}.{comparisonElement.Value.Source.Name}";
            var targetProp = $"{parameterTypesByName.Skip(1).FirstOrDefault().Key}.{tm.Target?.Name}";
            try
            {
                var sp = FmlValidator.ResolveIdentifierType(sourceProp, parameterTypesByName, group, issues);
                var tp = FmlValidator.ResolveIdentifierType(targetProp, parameterTypesByName, group, issues);
                if (tm.Relationship == ConceptMap.ConceptMapRelationship.Equivalent && sp != null && tp != null)
                {
                    Console.Write($"{sourceProp} -> {targetProp}; // {sp.Element.DebugString()} -> {tp.Element.DebugString()}\n");
                }
                else
                {
                    Console.Write($"// {sourceProp} -> {targetProp}; // {tm.Relationship}\n");
                }
            }
            catch (Exception)
            {
                Console.Write($"// {sourceProp} -> {targetProp}; // {tm.Relationship} -- Property doesn't resolve\n");
            }
        }
    }

    private static void CheckFmlForMissingPropertiesGroupRule(string prefix, FhirStructureMap fml, GroupDeclaration group, Dictionary<string, StructureDefinition?> _aliasedTypes, CrossVersionCheckOptions options, List<OperationOutcome.IssueComponent> issues, Dictionary<string, PropertyOrTypeDetails?> parameterTypesByName, GroupExpression rule, StructureComparison? comparison)
    {
        Console.Write(prefix);

        // deduce the datatypes for the variables
        Dictionary<string, PropertyOrTypeDetails?> parameterTypesByNameForRule = parameterTypesByName.ShallowCopy();
        PropertyOrTypeDetails? singleSourceVariable = null;
        if (rule.MappingExpression != null)
        {
            foreach (var source in rule.MappingExpression.Sources)
            {
                if (source != rule.MappingExpression.Sources.First())
                    Console.Write(", ");

                Console.Write($"{source.Identifier}");
                PropertyOrTypeDetails? tpV = null;
                try
                {
                    tpV = FmlValidator.ResolveIdentifierType(source.Identifier, parameterTypesByNameForRule, source, issues);
                }
                catch (ApplicationException e)
                {
                    string msg = $"Can't resolve type of source identifier `{source.Identifier}`: {e.Message}";
                    ReportIssue(issues, msg, OperationOutcome.IssueType.Exception);
                }

                // Check that the comparison includes this rule.
                if (comparison != null && tpV?.Element?.Path != null)
                {
                    if (comparison.ElementComparisons.ContainsKey(tpV.PropertyPath))
                    {
                        // remove this element (but probably want to check that it is used correctly too)
                        comparison.ElementComparisons.Remove(tpV.PropertyPath);
                    }

                    if (comparison.ElementComparisons.ContainsKey(tpV.Element.Path))
                    {
                        // remove this element (but probably want to check that it is used correctly too)
                        comparison.ElementComparisons.Remove(tpV.Element.Path);
                    }
                }

                if (!string.IsNullOrEmpty(source.TypeIdentifier))
                {
                    // Cast down to this type
                    Console.Write($".ofType({source.TypeIdentifier})");
                    string typeName = source.TypeIdentifier!;
                    if (!typeName.Contains(':')) // assume this is a FHIR type
                        typeName = "http://hl7.org/fhir/StructureDefinition/" + typeName;
                    var sdCastType = options.source.Resolver.FindStructureDefinitionAsync(typeName).WaitResult();
                    if (sdCastType == null)
                    {
                        string msg = $"Unable to resolve type cast `{typeName}` in {group.Name} at @{source.Line}:{source.Column}";
                        ReportIssue(issues, msg, OperationOutcome.IssueType.Duplicate);
                    }
                    else
                    {
                        var sw = new FmlStructureDefinitionWalker(sdCastType, options.source.Resolver);

                        // Check that the type being attempted is among the types in the actual property
                        if (!tpV?.Element?.Current?.Type.Any(t => t.Code == source.TypeIdentifier) == true)
                        {
                            string msg = $"Type `{typeName}` is not a valid cast for `{source.Identifier}` in {group.Name} at @{source.Line}:{source.Column}";
                            ReportIssue(issues, msg, OperationOutcome.IssueType.Duplicate);
                        }

                        // TODO: @brianpos - not sure if we want to pass through empty here, skip the call, or throw an exception if tpV is null here
                        tpV = new PropertyOrTypeDetails(tpV?.PropertyPath ?? string.Empty, sw.Current, options.source.Resolver);
                    }
                }

                Console.Write($" : {tpV?.Element?.DebugString() ?? "?"}");
                if (source.Alias != null)
                {
                    Console.Write($" as {source.Alias}");
                    if (parameterTypesByNameForRule.ContainsKey(source.Alias))
                    {
                        string msg = $"Duplicate source parameter name `{source.Alias}` in {group.Name} at @{source.Line}:{source.Column}";
                        ReportIssue(issues, msg, OperationOutcome.IssueType.Duplicate);
                    }
                    else
                    {
                        parameterTypesByNameForRule.Add(source.Alias, tpV);
                    }
                }

                if (source == rule.MappingExpression.Sources.First())
                {
                    singleSourceVariable = tpV;
                }
            }

            Console.Write($"  -->  ");

            foreach (var target in rule.MappingExpression.Targets)
            {
                if (target != rule.MappingExpression.Targets.First())
                    Console.Write(", ");

                PropertyOrTypeDetails? tpV = null;
                if (!string.IsNullOrEmpty(target.Identifier))
                {
                    Console.Write($"{target.Identifier}");
                    try
                    {
                        tpV = FmlValidator.ResolveIdentifierType(target.Identifier, parameterTypesByNameForRule, target, issues);
                    }
                    catch (ApplicationException e)
                    {
                        string msg = $"Can't resolve type of target identifier `{target.Identifier}`: {e.Message}";
                        ReportIssue(issues, msg, OperationOutcome.IssueType.Exception);
                    }
                    Console.Write($" : {tpV?.Element?.DebugString() ?? "?"}");
                }
                if (target.Invocation != null)
                {
                    tpV = FmlValidator.VerifyInvocation(group, issues, parameterTypesByNameForRule, target.Invocation, options.target.Resolver);
                }

                if (target.Alias != null)
                {
                    Console.Write($" as {target.Alias}");
                    if (parameterTypesByNameForRule.ContainsKey(target.Alias))
                    {
                        string msg = $"Duplicate target parameter name `{target.Alias}` in {group.Name} at @{target.Line}:{target.Column}";
                        ReportIssue(issues, msg, OperationOutcome.IssueType.Duplicate);
                    }
                    else
                    {
                        parameterTypesByNameForRule.Add(target.Alias, tpV);
                    }
                }
            }
        }

        if (rule.SimpleCopyExpression != null)
        {
            PropertyOrTypeDetails? sourceV = null;
            try
            {
                sourceV = FmlValidator.ResolveIdentifierType(rule.SimpleCopyExpression.Source, parameterTypesByNameForRule, rule.SimpleCopyExpression, issues);
                Console.Write($"{rule.SimpleCopyExpression.Source} : {sourceV?.Element?.DebugString() ?? "?"}");
            }
            catch (ApplicationException ex)
            {
                string msg = $"Can't resolve simple source `{rule.SimpleCopyExpression.Source}`: {ex.Message}";
                ReportIssue(issues, msg, OperationOutcome.IssueType.Exception);
            }

            Console.Write($"  -->  ");

            PropertyOrTypeDetails? targetV = null;
            try
            {
                targetV = FmlValidator.ResolveIdentifierType(rule.SimpleCopyExpression.Target, parameterTypesByNameForRule, rule.SimpleCopyExpression, issues);
                Console.Write($"{rule.SimpleCopyExpression.Target} : {targetV?.Element?.DebugString() ?? "?"}");
            }
            catch (ApplicationException ex)
            {
                string msg = $"Can't resolve simple target `{rule.SimpleCopyExpression.Target}`: {ex.Message}";
                ReportIssue(issues, msg, OperationOutcome.IssueType.Exception);
            }

            // Check that the comparison includes this rule.
            if (comparison != null && sourceV != null)
            {
                // var ec = comparison.ElementComparisons.Values.Where(ec => ec.Source.Path == sourceV.Element?.Path);
                if (comparison.ElementComparisons.ContainsKey(sourceV.PropertyPath))
                {
                    // remove this element (but probably want to check that it is used correctly too)
                    comparison.ElementComparisons.Remove(sourceV.PropertyPath);
                }

                if (comparison.ElementComparisons.ContainsKey(sourceV.Element.Path))
                {
                    // remove this element
                    comparison.ElementComparisons.Remove(sourceV.Element.Path);
                }
            }
        }

        // Scan any dependent group calls
        var de = rule.MappingExpression?.DependentExpression;
        if (de != null)
        {
            foreach (var i in de.Invocations)
            {
                Console.Write("\n        " + prefix);
                Console.Write($" then {i.Identifier}( ");
                if (!fml.GroupsByName.ContainsKey(i.Identifier) && !options.namedGroups.ContainsKey(i.Identifier))
                {
                    Console.WriteLine($"... )");
                    string msg = $"Calling non existent dependent group {i.Identifier} at @{i.Line}:{i.Column}";
                    ReportIssue(issues, msg, OperationOutcome.IssueType.NotFound);
                }
                else
                {
                    var dg = fml.GroupsByName.ContainsKey(i.Identifier) ? fml.GroupsByName[i.Identifier] : options.namedGroups[i.Identifier];
                    // walk the parameters
                    for (int nParam = 0; nParam < dg.Parameters.Count; nParam++)
                    {
                        if (nParam > 0)
                            Console.Write(", ");
                        var gp = dg.Parameters[nParam];
                        string? type = gp.TypeIdentifier;
                        // lookup the type in the aliases
                        if (type != null && !type.Contains('/') && _aliasedTypes.ContainsKey(type))
                        {
                            var sd = _aliasedTypes[type];
                            if (sd != null)
                                type = $"{sd.Url}|{sd.Version}";
                        }

                        // which value should we use
                        if (nParam < i.Parameters.Count)
                        {
                            var cp = i.Parameters[nParam];
                            Console.Write($"{cp.Identifier ?? cp.Literal?.ValueAsString}");
                            // Check in the rule source/target aliases
                            if (rule.MappingExpression != null)
                            {
                                string? variableName = cp.Identifier ?? cp.Literal?.ValueAsString;
                                if (variableName == null)
                                {
                                    string msg = $"No Variable name provided for parameter {i} calling dependent group {i.Identifier} at @{cp.Line}:{cp.Column}";
                                    ReportIssue(issues, msg, OperationOutcome.IssueType.NotFound);
                                }
                                else
                                {
                                    if (!parameterTypesByNameForRule.ContainsKey(variableName))
                                    {
                                        string msg = $"Variable not found `{variableName}` calling dependent group {i.Identifier} at @{cp.Literal?.Line}:{cp.Literal?.Column}";
                                        ReportIssue(issues, msg, OperationOutcome.IssueType.NotFound);
                                    }
                                    else
                                    {
                                        var gpv = parameterTypesByNameForRule[variableName];
                                        type = gpv?.ToString() ?? "??";
                                        // Console.Write($"({type})");
                                        if (gpv != null)
                                        {
                                            if (gp.ParameterElementDefinition == null)
                                            {
                                                gp.ParameterElementDefinition = gpv.Element;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        Console.Write($" {gp.Identifier} : {type ?? gp.ParameterElementDefinition?.Path ?? "?"}");
                    }
                    Console.WriteLine(" )");
                }
            }

            if (de.Expressions.Any())
            {
                Console.Write('\n');
                // process any expressions as a result of any
                foreach (var childRule in de.Expressions)
                {
                    CheckFmlForMissingPropertiesGroupRule(prefix + "     ", fml, group, _aliasedTypes, options, issues, parameterTypesByNameForRule, childRule, comparison);
                }
            }
        }
        else
        {
            Console.WriteLine();
        }
    }
}

public record CrossVersionCheckOptions : ValidateMapOptions
{
    public required DefinitionCollection SourcePackage { get; init; }
    public required DefinitionCollection TargetPackage { get; init; }
}

internal static class ElementDefinitionNavigatorExtensions
{
    public static string DebugString(this ElementDefinitionNavigator Element, bool includeTypes = true)
    {
        // return $"{Definition.Url}|{Definition.Version} # {Element.Path} ({String.Join(",", Element.Current.Type.Select(t => t.Code))})";
        if (includeTypes)
            return $"{Element.Path}|{Element.StructureDefinition.Version} ({String.Join(",", Element.Current.Type.Select(t =>
            {
                if (string.IsNullOrWhiteSpace(t.Code))
                {
                    System.Diagnostics.Trace.WriteLine($"Element {Element.Path}|{Element.StructureDefinition.Version ?? Element.StructureDefinition.FhirVersion.GetLiteral()} has no type data");
                }
                return t.Code;
            }))})";
        return $"{Element.Path}|{Element.StructureDefinition.Version ?? Element.StructureDefinition.FhirVersion.GetLiteral()}";
    }
}
