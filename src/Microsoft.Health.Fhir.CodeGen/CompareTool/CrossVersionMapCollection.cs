// <copyright file="CrossVersionMapCollection.cs" company="Microsoft Corporation">
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
    //private Dictionary<string, ConceptMap> _elementConceptMaps = [];

    private Dictionary<string, FhirStructureMap> _fmlByCompositeName = [];

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
            return TryLoadOfficialConceptMaps(path) && TryLoadOfficialStructureMaps(path);
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
            throw new DirectoryNotFoundException($"Could not find fhir-cross-version/input/{_sourceToTargetWithR} directory: {path}");
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

    public static async Task<OperationOutcome> VerifyFmlDataTypes(FhirStructureMap fml, Func<string, string?, Task<StructureDefinition?>> resolveMapUseCrossVersionType, Func<string, IEnumerable<FhirStructureMap>> resolveMaps,
        IAsyncResourceResolver sourceResolver, IAsyncResourceResolver targetResolver, Dictionary<string, GroupDeclaration> namedGroups, Dictionary<string, GroupDeclaration> typedGroups)
    {
        Console.WriteLine($"Validating map {fml.MapDirective?.Url ?? fml.MetadataByPath["url"]?.Literal?.ValueAsString}");

        // scan the uses types
        Dictionary<string, StructureDefinition?> _aliasedTypes = new ();
        foreach (var use in fml.StructuresByUrl)
        {
            // Console.WriteLine($"Use {use.Key} as {use.Value?.Alias}");
            var sd = await resolveMapUseCrossVersionType(use.Key.Trim('\"'), use.Value?.Alias);
            if (use.Value?.Alias != null)
                _aliasedTypes.Add(use.Value.Alias, sd);
            else if (sd != null && sd.Name != null)
                _aliasedTypes.Add(use.Value?.Alias ?? sd.Name, sd);
        }

        //// scan the imports
        //// pointless since everything is brought in through the wildcard stuff anyway!
        //foreach (var import in fml.ImportsByUrl.Keys)
        //{
        //    Console.WriteLine($"Import {import}");
        //    var maps = resolveMaps(import);
        //}

        // scan all the groups
        OperationOutcome outcome = new OperationOutcome();
        foreach (var group in fml.GroupsByName.Values)
        {
            var results = VerifyFmlDataTypesForGroup(fml, group, _aliasedTypes, resolveMaps, sourceResolver, targetResolver, namedGroups, typedGroups);
            outcome.Issue.AddRange(results);
        }
        Console.WriteLine();
        return outcome;
    }

    public class PropertyOrTypeDetails
    {
        public IAsyncResourceResolver Resolver { get; init; }
        public ElementDefinitionNavigator Element { get; init; }

        public override string ToString()
        {
            // return $"{Definition.Url}|{Definition.Version} # {Element.Path} ({String.Join(",", Element.Current.Type.Select(t => t.Code))})";
            return $"{Element.Path}|{Element.StructureDefinition.Version} ({String.Join(",", Element.Current.Type.Select(t => t.Code))})";
        }
    }

    public static List<OperationOutcome.IssueComponent> VerifyFmlDataTypesForGroup(
        FhirStructureMap fml,
        GroupDeclaration group,
        Dictionary<string, StructureDefinition?> _aliasedTypes,
        Func<string, IEnumerable<FhirStructureMap>> resolveMaps,
        IAsyncResourceResolver sourceResolver, IAsyncResourceResolver targetResolver, Dictionary<string, GroupDeclaration> namedGroups, Dictionary<string, GroupDeclaration> typedGroups)
    {
        List<OperationOutcome.IssueComponent> issues = new List<OperationOutcome.IssueComponent>();
        Console.Write($"  {group.Name}(");

        // Check the types in the group parameters
        Dictionary<string, PropertyOrTypeDetails?> parameterTypesByName = new ();
        foreach (var gp in group.Parameters)
        {
            if (gp != group.Parameters.First())
                Console.Write(", ");
            PropertyOrTypeDetails? tp = null;
            string? type = gp.TypeIdentifier;
            // lookup the type in the aliases
            var resolver = gp.InputMode == StructureMap.StructureMapInputMode.Source ? sourceResolver : targetResolver;
            if (type != null)
            {
                if (!type.Contains('/') && _aliasedTypes.ContainsKey(type))
                {
                    var sd = _aliasedTypes[type];
                    if (sd != null)
                    {
                        var sw = new StructureDefinitionWalker(sd, resolver);
                        tp = new PropertyOrTypeDetails
                        {
                            Resolver = resolver,
                            Element = sw.Current
                        };
                        type = $"{sd.Url}|{sd.Version}";
                        gp.ParameterElementDefinition = sw.Current;
                    }
                }
                else if (type != "string")
                {
                    string msg = $"Group {group.Name} parameter {gp.Identifier} at @{gp.Line}:{gp.Column} has no type `{gp.TypeIdentifier}`";
                    ReportIssue(issues, msg, OperationOutcome.IssueType.NotFound);
                }
            }
            else if (gp.ParameterElementDefinition != null)
            {
                tp = new PropertyOrTypeDetails
                {
                    Resolver = resolver,
                    Element = gp.ParameterElementDefinition
                };
            }
            parameterTypesByName.Add(gp.Identifier, tp);
            Console.Write($" {gp.Identifier}");
            if (gp.ParameterElementDefinition != null)
                Console.Write($" : {gp.ParameterElementDefinition.DebugString()}");
            else
                Console.Write($" : {type ?? "?"}");
        }
        Console.Write(" )\n  {\n");

        // Check if the group extends any other groups
        if (group.ExtendsIdentifier != null)
        {
            // Check that the parameter values are compatible...
        }

        // Now scan for dependencies in rules
        foreach(var rule in group.Expressions)
        {
            VerifyFmlGroupRule("     ", fml, group, _aliasedTypes, sourceResolver, targetResolver, namedGroups, typedGroups, issues, parameterTypesByName, rule);
        }
        Console.WriteLine("  }");
        return issues;
    }

    private static void VerifyFmlGroupRule(string prefix, FhirStructureMap fml, GroupDeclaration group, Dictionary<string, StructureDefinition?> _aliasedTypes, IAsyncResourceResolver sourceResolver, IAsyncResourceResolver targetResolver, Dictionary<string, GroupDeclaration> namedGroups, Dictionary<string, GroupDeclaration> typedGroups, List<OperationOutcome.IssueComponent> issues, Dictionary<string, PropertyOrTypeDetails?> parameterTypesByName, GroupExpression rule)
    {
        Console.Write(prefix);

        // deduce the datatypes for the variables
        Dictionary<string, PropertyOrTypeDetails?> parameterTypesByNameForRule = parameterTypesByName.ShallowCopy();
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
                    tpV = ResolveIdentifierType(source.Identifier, parameterTypesByNameForRule, source, issues);
                }
                catch (ApplicationException e)
                {
                    string msg = $"Can't resolve type of source identifier `{source.Identifier}`: {e.Message}";
                    ReportIssue(issues, msg, OperationOutcome.IssueType.Exception);
                }

                if (!string.IsNullOrEmpty(source.TypeIdentifier))
                {
                    // Cast down to this type
                    Console.Write($".ofType({source.TypeIdentifier})");
                    string typeName = source.TypeIdentifier;
                    if (!typeName.Contains(':')) // assume this is a FHIR type
                        typeName = "http://hl7.org/fhir/StructureDefinition/" + typeName;
                    var sdCastType = sourceResolver.FindStructureDefinitionAsync(typeName).WaitResult();
                    if (sdCastType == null)
                    {
                        string msg = $"Unable to resolve type cast `{typeName}` in {group.Name} at @{source.Line}:{source.Column}";
                        ReportIssue(issues, msg, OperationOutcome.IssueType.Duplicate);
                    }
                    else
                    {
                        var sw = new StructureDefinitionWalker(sdCastType, sourceResolver);
                        tpV = new PropertyOrTypeDetails
                        {
                            Resolver = sourceResolver,
                            Element = sw.Current
                        };
                    }
                }

                // Cardinality constraints
                if (!string.IsNullOrEmpty(source.Cardinality))
                {
                    Console.Write($"[{source.Cardinality}]");
                }

                // Source Default value
                if (source.DefaultExpression != null)
                {
                    Console.Write($" DEFAULT({source.DefaultExpression.RawText})");
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
                        tpV = ResolveIdentifierType(target.Identifier, parameterTypesByNameForRule, target, issues);
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
                    tpV = VerifyInvocation(group, issues, parameterTypesByNameForRule, target.Invocation, targetResolver);
                }

                if (target.Transform != null)
                {
                    Console.Write(" = ");
                    PropertyOrTypeDetails? transformedSourceV = null;
                    if (target.Transform.Literal != null)
                    {
                        var literal = target.Transform.Literal;
                        Console.Write($" {literal.RawText}");
                        transformedSourceV = ResolveLiteralDataType(group, sourceResolver, issues, literal);
                    }
                    if (!string.IsNullOrEmpty(target.Transform.Identifier))
                    {
                        Console.Write($" {target.Transform.Identifier}");
                        try
                        {
                            transformedSourceV = ResolveIdentifierType(target.Transform.Identifier, parameterTypesByNameForRule, target.Transform, issues);
                        }
                        catch (ApplicationException e)
                        {
                            string msg = $"Can't resolve type of target transform identifier `{target.Transform.Identifier}`: {e.Message}";
                            ReportIssue(issues, msg, OperationOutcome.IssueType.Exception);
                        }

                    }
                    if (target.Transform.Invocation != null)
                        transformedSourceV = VerifyInvocation(group, issues, parameterTypesByNameForRule, target.Transform.Invocation, targetResolver);

                    if (target.Transform.fpExpression != null)
                    {
                        // Need to process this!
                        Console.Write($"( {target.Transform.fpExpression.RawText} )");
                    }

                    if (transformedSourceV?.Element == null && target.Transform.fpExpression == null) // skipping error for FP expression as that's known
                    {
                        string msg = $"No type derived for transform `{target.Transform.RawText}` at @{target.Transform.Line}:{target.Transform.Column} ";
                        ReportIssue(issues, msg, OperationOutcome.IssueType.Exception);
                    }

                    Console.Write($" : {transformedSourceV?.Element?.DebugString() ?? "?"}");

                    // Validate that the datatype is compatible
                    VerifyMapBetweenDatatypes(typedGroups, issues, target.Transform, transformedSourceV, tpV);
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
                sourceV = ResolveIdentifierType(rule.SimpleCopyExpression.Source, parameterTypesByNameForRule, rule.SimpleCopyExpression, issues);
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
                targetV = ResolveIdentifierType(rule.SimpleCopyExpression.Target, parameterTypesByNameForRule, rule.SimpleCopyExpression, issues);
                Console.Write($"{rule.SimpleCopyExpression.Target} : {targetV?.Element?.DebugString() ?? "?"}");
            }
            catch (ApplicationException ex)
            {
                string msg = $"Can't resolve simple target `{rule.SimpleCopyExpression.Target}`: {ex.Message}";
                ReportIssue(issues, msg, OperationOutcome.IssueType.Exception);
            }

            // Verify that there exists a map that goes between these types
            VerifyMapBetweenDatatypes(typedGroups, issues, rule.SimpleCopyExpression, sourceV, targetV);
        }

        // Scan any dependent group calls
        var de = rule.MappingExpression?.DependentExpression;
        if (de != null)
        {
            foreach (var i in de.Invocations)
            {
                Console.Write("\n        "+prefix);
                Console.Write($" then {i.Identifier}( ");
                if (!fml.GroupsByName.ContainsKey(i.Identifier) && !namedGroups.ContainsKey(i.Identifier))
                {
                    Console.WriteLine($"... )");
                    string msg = $"Calling non existent dependent group {i.Identifier} at @{i.Line}:{i.Column}";
                    ReportIssue(issues, msg, OperationOutcome.IssueType.NotFound);
                }
                else
                {
                    var dg = fml.GroupsByName.ContainsKey(i.Identifier) ? fml.GroupsByName[i.Identifier] : namedGroups[i.Identifier];
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
                                            else
                                            {
                                                if (gp.ParameterElementDefinition.ToString() != gpv.Element.ToString())
                                                {
                                                    string msg = $"Mismatched type `{cp.Identifier ?? cp.Literal?.ValueAsString}` calling dependent group {i.Identifier} at @{cp.Literal?.Line}:{cp.Literal?.Column} - {gp.ParameterElementDefinition.DebugString()} != {gpv.Element.DebugString()}";
                                                    ReportIssue(issues, msg, OperationOutcome.IssueType.Conflict);
                                                }
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
                    VerifyFmlGroupRule(prefix + "     ", fml, group, _aliasedTypes, sourceResolver, targetResolver, namedGroups, typedGroups, issues, parameterTypesByNameForRule, childRule);
                }
            }
        }
        else
        {
            Console.WriteLine();
        }
    }

    private static void VerifyMapBetweenDatatypes(Dictionary<string, GroupDeclaration> typedGroups, List<OperationOutcome.IssueComponent> issues, FmlNode node, PropertyOrTypeDetails? sourceV, PropertyOrTypeDetails? targetV)
    {
        if (sourceV != null && targetV != null)
        {
            IEnumerable<string> sourceTypeNames = GetTypeNames(sourceV);
            IEnumerable<string> targetTypeNames = GetTypeNames(targetV);
            // Just need to ensure that there exists a target type for each source that exists
            foreach (var stn in sourceTypeNames)
            {
                if (targetTypeNames.Contains(stn))
                    continue;

                IEnumerable<string> lookupsForSource;
                if (stn.StartsWith("http://hl7.org/fhirpath/"))
                {
                    bool sameTypeFound = false;
                    lookupsForSource = targetTypeNames.Select(ttn =>
                    {
                        int versionSeperatorIndex = ttn.IndexOf('|');
                        var versionLessSourceType = versionSeperatorIndex > 0 ? ttn.Substring(0, versionSeperatorIndex) : ttn;
                        if (FhirToFhirPathDataTypeMappings.TryGetValue(versionLessSourceType, out var fpTN) && !ttn.StartsWith("http://hl7.org/fhirpath/"))
                        {
                            if (stn == fpTN)
                                sameTypeFound = true;
                            return $"{stn} -> {fpTN}";
                        }
                        return $"{stn} -> {ttn}";
                    }).ToArray();
                    if (sameTypeFound)
                        continue;
                }
                else
                {
                    int versionSeperatorIndex = stn.IndexOf('|');
                    var versionLessSourceType = versionSeperatorIndex > 0 ? stn.Substring(0, versionSeperatorIndex) : stn;
                    if (FhirToFhirPathDataTypeMappings.TryGetValue(versionLessSourceType, out var fpTN) && targetTypeNames.Any(ttn => ttn.StartsWith("http://hl7.org/fhirpath/")))
                    {
                        if (targetTypeNames.Contains(fpTN))
                            continue;
                        lookupsForSource = targetTypeNames.Where(ttn => !ttn.StartsWith("http://hl7.org/fhirpath/")).Select(ttn => $"{stn} -> {ttn}");

                        lookupsForSource = lookupsForSource.Union(
                            targetTypeNames.Where(ttn => ttn.StartsWith("http://hl7.org/fhirpath/")).Select(ttn => $"{fpTN} -> {ttn}")
                            );
                    }
                    else
                    {
                        lookupsForSource = targetTypeNames.Select(ttn => $"{stn} -> {ttn}");
                    }
                }

                if (!lookupsForSource.Any(mapLookup => typedGroups.ContainsKey(mapLookup)))
                {
                    string msg = $"There is no target type for mapping from {stn} detected @{node.Line}:{node.Column}";
                    msg += "\n        " + string.Join("\n        ", lookupsForSource);
                    ReportIssue(issues, msg, OperationOutcome.IssueType.Exception);
                }
            }
        }
    }

    private static Dictionary<string, string> FhirToFhirPathDataTypeMappings = new Dictionary<string, string>(
        [
            new ("http://hl7.org/fhir/StructureDefinition/boolean", "http://hl7.org/fhirpath/System.Boolean"),
            new ("http://hl7.org/fhir/StructureDefinition/string", "http://hl7.org/fhirpath/System.String"),
            new ("http://hl7.org/fhir/StructureDefinition/uri", "http://hl7.org/fhirpath/System.String"),
            new ("http://hl7.org/fhir/StructureDefinition/code", "http://hl7.org/fhirpath/System.String"),
            new ("http://hl7.org/fhir/StructureDefinition/oid", "http://hl7.org/fhirpath/System.String"),
            new ("http://hl7.org/fhir/StructureDefinition/id", "http://hl7.org/fhirpath/System.String"),
            new ("http://hl7.org/fhir/StructureDefinition/uuid", "http://hl7.org/fhirpath/System.String"),
            new ("http://hl7.org/fhir/StructureDefinition/markdown", "http://hl7.org/fhirpath/System.String"),
            new ("http://hl7.org/fhir/StructureDefinition/base64Binary", "http://hl7.org/fhirpath/System.String"),
            new ("http://hl7.org/fhir/StructureDefinition/integer", "http://hl7.org/fhirpath/System.Integer"),
            new ("http://hl7.org/fhir/StructureDefinition/unsignedInt", "http://hl7.org/fhirpath/System.Integer"),
            new ("http://hl7.org/fhir/StructureDefinition/positiveInt", "http://hl7.org/fhirpath/System.Integer"),
            new ("http://hl7.org/fhir/StructureDefinition/integer64", "http://hl7.org/fhirpath/System.Long"),
            new ("http://hl7.org/fhir/StructureDefinition/decimal", "http://hl7.org/fhirpath/System.Decimal"),
            new ("http://hl7.org/fhir/StructureDefinition/date", "http://hl7.org/fhirpath/System.DateTime"),
            new ("http://hl7.org/fhir/StructureDefinition/dateTime", "http://hl7.org/fhirpath/System.DateTime"),
            new ("http://hl7.org/fhir/StructureDefinition/instant", "http://hl7.org/fhirpath/System.DateTime"),
            new ("http://hl7.org/fhir/StructureDefinition/time", "http://hl7.org/fhirpath/System.Time"),
            new ("http://hl7.org/fhir/StructureDefinition/Quantity", "http://hl7.org/fhirpath/System.Quantity"),
        ]);

    private static IEnumerable<string> GetTypeNames(PropertyOrTypeDetails ptd)
    {
        List<string> typeNames = new List<string>();

        // simple root case, we're not in a child property
        if (!ptd.Element.Path.Contains("."))
        {
            typeNames.Add($"{ptd.Element.StructureDefinition.Url}|{ptd.Element.StructureDefinition.Version}");
        }
        else
        {
            foreach (var t in ptd.Element.Current.Type)
            {
                var typeName = t.Code;
                if (typeName != null)
                {
                    if (!typeName.Contains(':')) // assume this is a FHIR type
                        typeName = "http://hl7.org/fhir/StructureDefinition/" + typeName;
                    if (typeName.StartsWith("http://hl7.org/fhirpath/System."))
                    {
                        if (!typeNames.Contains(typeName))
                            typeNames.Add(typeName);
                    }
                    else
                    {
                        var sdType = ptd.Resolver.FindStructureDefinitionAsync(typeName).WaitResult();
                        if (sdType != null)
                        {
                            var tn = $"{sdType.Url}|{sdType.Version}";
                            if (!typeNames.Contains(tn))
                                typeNames.Add(tn);
                        }
                    }
                }
                else
                {
                    var tn = t.Code + "|" + ptd.Element.StructureDefinition.Version;
                    if (!typeNames.Contains(tn))
                        typeNames.Add(tn);
                }
            }
        }
        return typeNames;
    }

    private static PropertyOrTypeDetails? ResolveLiteralDataType(GroupDeclaration group, IAsyncResourceResolver sourceResolver, List<OperationOutcome.IssueComponent> issues, LiteralValue literal)
    {
        if (literal.TokenType == FmlTokenTypeCodes.Id)
            ReportIssue(issues, $"Invalid token type `{literal.TokenType}` parsing literal - likely should have been a target.transform.identifier @{literal.Line}:{literal.Column}", OperationOutcome.IssueType.Invalid);
        // todo: resolve this type
        string? typeName = null;
        switch (literal.TokenType)
        {
            case FmlTokenTypeCodes.SingleQuotedString: typeName = "string"; break;
            case FmlTokenTypeCodes.DoubleQuotedString: typeName = "string"; break;
            case FmlTokenTypeCodes.Bool: typeName = "boolean"; break;
            case FmlTokenTypeCodes.Date: typeName = "date"; break;
            case FmlTokenTypeCodes.DateTime: typeName = "dateTime"; break;
            case FmlTokenTypeCodes.Time: typeName = "time"; break;
            case FmlTokenTypeCodes.Integer: typeName = "integer"; break;
            case FmlTokenTypeCodes.LongInteger: typeName = "integer64"; break;
            case FmlTokenTypeCodes.Decimal: typeName = "decimal"; break;
                // quantity?
        }
        return ResolveDataTypeFromName(group, sourceResolver, issues, literal, typeName);
    }

    private static PropertyOrTypeDetails? ResolveDataTypeFromName(GroupDeclaration group, IAsyncResourceResolver resolver, List<OperationOutcome.IssueComponent> issues, FmlNode literal, string? typeName)
    {
        if (typeName != null)
        {
            if (!typeName.Contains(':')) // assume this is a FHIR type
                typeName = "http://hl7.org/fhir/StructureDefinition/" + typeName;
            var sdCastType = resolver.FindStructureDefinitionAsync(typeName).WaitResult();
            if (sdCastType == null)
            {
                string msg = $"Unable to resolve type `{typeName}` in {group.Name} at @{literal.Line}:{literal.Column}";
                ReportIssue(issues, msg, OperationOutcome.IssueType.Duplicate);
            }
            else
            {
                var sw = new StructureDefinitionWalker(sdCastType, resolver);
                return new PropertyOrTypeDetails
                {
                    Resolver = resolver,
                    Element = sw.Current
                };
            }
        }

        return null;
    }

    private static PropertyOrTypeDetails? VerifyInvocation(GroupDeclaration group, List<OperationOutcome.IssueComponent> issues, Dictionary<string, PropertyOrTypeDetails?> parameterTypesByNameForRule, FmlInvocation invocation, IAsyncResourceResolver targetResolver)
    {
        // deduce the return type of the invocation
        Console.Write($" {invocation.Identifier}(");
        PropertyOrTypeDetails? parameterTypeForFirstParam = null;
        foreach (var p in invocation.Parameters)
        {
            if (p != invocation.Parameters.First())
                Console.Write(",");

            PropertyOrTypeDetails? parameterTypeV = null;
            if (!string.IsNullOrEmpty(p.Identifier))
            {
                Console.Write($"{p.Identifier}");
                try
                {
                    parameterTypeV = ResolveIdentifierType(p.Identifier, parameterTypesByNameForRule, p, issues); // is this the correct place?
                    p.ParameterElementDefinition = parameterTypeV?.Element;
                }
                catch (ApplicationException e)
                {
                    string msg = $"Can't resolve type of parameter identifier `{p.Identifier}`: {e.Message}";
                    ReportIssue(issues, msg, OperationOutcome.IssueType.Exception);
                }
            }
            if (p.Literal != null)
            {
                // TODO: resolve the type of the literal
                Console.Write($" {p.Literal.RawText}");
                parameterTypeV = ResolveLiteralDataType(group, targetResolver, issues, p.Literal);
            }
            if (p == invocation.Parameters.First())
                parameterTypeForFirstParam = parameterTypeV;
            Console.Write($" : {parameterTypeV?.Element?.DebugString() ?? "?"}");
        }
        Console.Write(" )");
        switch (invocation.Identifier)
        {
            case "create":
                // Check that the first parameter is a string, and that it has resolved properly.
                var literal = invocation.Parameters.FirstOrDefault()?.Literal;
                if (literal != null)
                {
                    string typeName = literal.ValueAsString;
                    return ResolveDataTypeFromName(group, targetResolver, issues, literal, typeName);
                }
                else
                {
                    string msg = $"No type parameter for create at @{invocation.Line}:{invocation.Column}";
                    ReportIssue(issues, msg, OperationOutcome.IssueType.Duplicate);
                }
                break;

            case "copy":
                return parameterTypeForFirstParam;

            case "truncate":
                return ResolveDataTypeFromName(group, targetResolver, issues, invocation, "string");

            case "escape":
                return ResolveDataTypeFromName(group, targetResolver, issues, invocation, "string");

            // cast?

            case "append":
                return ResolveDataTypeFromName(group, targetResolver, issues, invocation, "string");

            case "translate":
                // Check that the parameters are correct for the translate operation
                if (invocation.Parameters.Count < 2)
                {
                    string msg = $"Translate missing transform parameters at @{invocation.Line}:{invocation.Column}";
                    ReportIssue(issues, msg, OperationOutcome.IssueType.Duplicate);
                }
                // This will just return the datatype of the first parameter (as it's meant to just convert that value with the definition in the second parameter)
                return parameterTypeForFirstParam;

            case "reference":
                return ResolveDataTypeFromName(group, targetResolver, issues, invocation, "string");

            // dateOp?

            case "uuid":
                return ResolveDataTypeFromName(group, targetResolver, issues, invocation, "uuid");

            case "pointer":
                return ResolveDataTypeFromName(group, targetResolver, issues, invocation, "string");

            // case "evaluate": // FHIRpath expression - wish I could use the Fhirpath static validator here

            case "cc":
                return ResolveDataTypeFromName(group, targetResolver, issues, invocation, "CodeableConcept");

            case "c":
                return ResolveDataTypeFromName(group, targetResolver, issues, invocation, "Coding");

            case "qty":
                return ResolveDataTypeFromName(group, targetResolver, issues, invocation, "Quantity");

            case "id":
                return ResolveDataTypeFromName(group, targetResolver, issues, invocation, "Identifier");

            case "cp":
                return ResolveDataTypeFromName(group, targetResolver, issues, invocation, "ContactPoint");

            default:
                string msgUnhandled = $"Invocation of `{invocation.Identifier}` not handled yet at @{invocation.Line}:{invocation.Column}";
                ReportIssue(issues, msgUnhandled, OperationOutcome.IssueType.Duplicate);
                break;
        }

        // unknown return type detected
        return null;
    }

    private static void ReportIssue(List<OperationOutcome.IssueComponent> issues, string message, OperationOutcome.IssueType code, OperationOutcome.IssueSeverity severity = OperationOutcome.IssueSeverity.Error)
    {
        issues.Add(new OperationOutcome.IssueComponent()
        {
            Code = code,
            Severity = severity,
            Details = new CodeableConcept() { Text = message }
        });
        Console.WriteLine("\nError: " + message);
    }

    private static PropertyOrTypeDetails? ResolveIdentifierType(string identifier, Dictionary<string, PropertyOrTypeDetails?> parameterTypesByNameForRule, FmlNode sourceOrTargetNode, List<OperationOutcome.IssueComponent> issues)
    {
        IEnumerable<string> parts = identifier.Split('.');
        // Get the base type for this variable
        if (parameterTypesByNameForRule.TryGetValue(parts.First(), out PropertyOrTypeDetails? tp))
        {
            if (tp != null)
            {
                var childProps = parts.Skip(1);
                while (childProps.Any())
                {
                    var sw = new StructureDefinitionWalker(tp.Element, tp.Resolver);
                    try
                    {
                        var node = sw.Child(childProps.First());
                        if (node != null)
                        {
                            if (node.Current.Current.ContentReference != null)
                            {
                                // Need to walk into the node further
                                if (StructureDefinitionWalker.TryFollowContentReference(node.Current, s => tp.Resolver.FindStructureDefinitionAsync(s).WaitResult(), out var r))
                                {
                                    node = new StructureDefinitionWalker(r, tp.Resolver);
                                }
                            }
                            tp = new PropertyOrTypeDetails
                            {
                                Resolver = tp.Resolver,
                                Element = node.Current
                            };
                        }
                    }
                    catch (Exception ex)
                    {
                        // if (ex is StructureDefinitionWalkerException swe && !sw.Current.Elements.Any(e => e.Path == sw.Current.Path + "." + childProps.First() && e.IsPrimitiveValueConstraint()))
                            throw new ApplicationException($"{ex.Message} @{sourceOrTargetNode.Line}:{sourceOrTargetNode.Column}", ex);
                    }

                    childProps = childProps.Skip(1);
                }
                return tp;
            }
            return null;
        }
        throw new ApplicationException($"Identifier `{parts.First()}` is not in scope @{sourceOrTargetNode.Line}:{sourceOrTargetNode.Column}");
    }

    private static void ProcessCrossVersionGroup(
        FhirStructureMap fml,
        string sourcePrefix,
        string targetPrefix,
        GroupDeclaration group,
        Dictionary<string, Dictionary<string, FmlTargetInfo>> fmlPathLookup)
    {
        string groupSourceVar = string.Empty;
        string groupTargetVar = string.Empty;

        // TODO: we should be better about detecting this. Either track the call tree during a recursive descent or
        // determine the behavior based off of element definition named references.

        // Skip (re-)processing some known recursive points
        if (sourcePrefix == "QuestionnaireResponse.item.item" || targetPrefix == "QuestionnaireResponse.item.item"
            || sourcePrefix == "Questionnaire.item.item" || targetPrefix == "Questionnaire.item.item"
            || sourcePrefix == "QuestionnaireResponse.item.answer.item.answer" || targetPrefix == "QuestionnaireResponse.item.answer.item.answer"
            || sourcePrefix == "GraphDefinition.link.target.link"
            )
            return;

        if (sourcePrefix.Length > 2048 || targetPrefix.Length > 2048)
        {
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
                string sourceName = exp.SimpleCopyExpression.Source.StartsWith(groupSourceVar, StringComparison.Ordinal)
                    ? exp.SimpleCopyExpression.Source[(groupSourceVarLen + 1)..]
                    : exp.SimpleCopyExpression.Source;

                string targetName = exp.SimpleCopyExpression.Target.StartsWith(groupTargetVar, StringComparison.Ordinal)
                    ? exp.SimpleCopyExpression.Target[(groupTargetVarLen + 1)..]
                    : exp.SimpleCopyExpression.Target;

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
                    string sourceName = source.Identifier.StartsWith(groupSourceVar, StringComparison.Ordinal) && source.Identifier.Length > groupSourceVarLen
                        ? source.Identifier[(groupSourceVarLen + 1)..]
                        : source.Identifier;

                    // add our current name prefix
                    sourceName = $"{sourcePrefix}.{sourceName}";

                    if (!fmlPathLookup.TryGetValue(sourceName, out Dictionary<string, FmlTargetInfo>? expressionsByTarget))
                    {
                        expressionsByTarget = [];
                        fmlPathLookup.Add(sourceName, expressionsByTarget);
                    }

                    foreach (FmlExpressionTarget target in exp.MappingExpression.Targets)
                    {
                        string targetName = target.Identifier?.StartsWith(groupTargetVar, StringComparison.Ordinal) == true && target.Identifier.Length > groupTargetVarLen
                            ? target.Identifier[(groupTargetVarLen + 1)..]
                            : target.Identifier ?? string.Empty;

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
                                    ProcessCrossVersionGroup(fml, sourceName, targetName, dependentGroup, fmlPathLookup);
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
                    Code = tm.Target?.Path ?? path,
                    Display = tm.Target?.Short ?? string.Empty,
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
}

internal static class ElementDefinitionNavigatorExtensions
{
    public static string DebugString(this ElementDefinitionNavigator Element, bool includeTypes = false)
    {
        // return $"{Definition.Url}|{Definition.Version} # {Element.Path} ({String.Join(",", Element.Current.Type.Select(t => t.Code))})";
        if (includeTypes)
            return $"{Element.Path}|{Element.StructureDefinition.Version} ({String.Join(",", Element.Current.Type.Select(t => t.Code))})";
        return $"{Element.Path}|{Element.StructureDefinition.Version ?? Element.StructureDefinition.FhirVersion.GetLiteral()}";
    }
}
