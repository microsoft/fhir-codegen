using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Fhir.CodeGen.Common.FhirExtensions;
using Fhir.CodeGen.Common.Utils;
using Fhir.CodeGen.Comparison.Models;
using Fhir.CodeGen.Comparison.XVer;
using Fhir.CodeGen.Lib.FhirExtensions;
using Fhir.CodeGen.MappingLanguage;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Logging;
using static Fhir.CodeGen.Comparison.CompareTool.CrossVersionMapCollection;

namespace Fhir.CodeGen.Comparison.CompareTool;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// TODO: This is mostly pulled from CrossVersionMapCollection and needs to be refactored.
/// </remarks>
public class FmlDbProcessor
{
    private IDbConnection _db;
    private DbFhirPackage _sourcePackage;
    private DbFhirPackage _targetPackage;
    private string _name;
    private FhirStructureMap _fml;
    private string _fmlFilename;
    private string? _fmlUrl;

    private ILoggerFactory? _loggerFactory;
    private ILogger _logger;

    public FmlDbProcessor(
        IDbConnection db,
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        string filename,
        string name,
        FhirStructureMap fml,
        ILoggerFactory? loggerFactory = null)
    {
        _db = db;
        _sourcePackage = sourcePackage;
        _targetPackage = targetPackage;
        _name = name;
        _fml = fml;
        _fmlFilename = filename;

        _loggerFactory = loggerFactory;
        _logger = loggerFactory?.CreateLogger<FmlDbProcessor>()
            ?? LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<FmlDbProcessor>();

        _fmlUrl = fml.MetadataByPath.ContainsKey("url")
            ? fml.MetadataByPath["url"]?.Literal?.ValueAsString
            : null;
    }

    public void ProcessElementRelationships()
    {
        Dictionary<string, Dictionary<string, CrossVersionMapCollection.FmlTargetInfo>> fmlPathLookup = [];
        fmlPathLookup.Clear();

        // process each of the groups in the FML to extract path maps
        foreach ((string groupName, GroupDeclaration group) in _fml.GroupsByName)
        {
            // process root groups (recurses into dependent groups)
            if (_name.Contains(groupName))
            {
                System.Diagnostics.Debug.Fail("Pick up here.");
                processCrossVersionGroup(groupName, groupName, group, fmlPathLookup);
            }
        }

        reconcileElementMapFmlPathsInDb(fmlPathLookup);
    }

    private void processFmlGroup()
    {

    }

    [Obsolete]
    private void processCrossVersionGroup(
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
            _logger.LogWarning($"{_fml.MapDirective?.Url ?? _fml.MetadataByPath["url"]?.Literal?.Value} {group.Name} Path likely in a recursive loop {sourcePrefix} -> {targetPrefix}");
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
                                if (fnName != group.Name && _fml.GroupsByName.TryGetValue(fnName, out GroupDeclaration? dependentGroup))
                                {
                                    if (dependentGroupCallStack == null || !dependentGroupCallStack.Contains(fnName))
                                    {
                                        var newStack = dependentGroupCallStack?.Append(fnName).ToArray() ?? [fnName];
                                        processCrossVersionGroup(sourceName, targetName, dependentGroup, fmlPathLookup, newStack);
                                    }
                                }
                            }
                        }
                    }

                }

                continue;
            }
        }
    }

    private void reconcileElementMapFmlPathsInDb(
        Dictionary<string, Dictionary<string, FmlTargetInfo>> fmlPathLookup)
    {
        List<DbStructureMappingRecord> sdMappingRecsToAdd = [];
        List<DbElementMappingRecord> edMappingRecsToAdd = [];

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
                if (DbStructureDefinition.SelectSingle(_db, FhirPackageKey: _sourcePackage.Key, Id: sourceTypeName)
                    is not DbStructureDefinition sourceSd)
                {
                    _logger.LogError($"Could not resolve source type {sourceTypeName} for {sourcePath} in {_sourcePackage.ShortName}to{_targetPackage.ShortName}");
                    continue;
                }

                DbElement? sourceEd = DbElement.SelectSingle(_db, FhirPackageKey: _sourcePackage.Key, Path: sourcePath);
                // make sure the source element exists
                if (sourceEd is null)
                {
                    _logger.LogWarning($"Note: could not resolve source path {sourcePath} for {sourceTypeName} in {_sourcePackage.ShortName}to{_targetPackage.ShortName}");
                }

                // skip elements that have child elements - will either be picked up by dependent groups or are not relevant
                if (sourceEd?.ChildElementCount > 0)
                {
                    continue;
                }

                // make sure we have a target structure
                if (DbStructureDefinition.SelectSingle(_db, FhirPackageKey: _targetPackage.Key, Id: targetTypeName)
                    is not DbStructureDefinition targetSd)
                {
                    _logger.LogError($"Could not resolve target type {targetTypeName} for {targetPath} in {_sourcePackage.ShortName}to{_targetPackage.ShortName}");
                    continue;
                }

                DbElement? targetEd = DbElement.SelectSingle(_db, FhirPackageKey: _targetPackage.Key, Path: targetPath);
                // make sure the target element exists
                if (targetEd is null)
                {
                    _logger.LogWarning($"Note: Could not resolve target path {targetPath} for {targetTypeName} in {_sourcePackage.ShortName}to{_targetPackage.ShortName}");
                }

                // skip elements that have child elements - will either be picked up by dependent groups or are not relevant
                if (targetEd?.ChildElementCount > 0)
                {
                    continue;
                }

                // check to see if there are existing mapping records for the structures
                List<DbStructureMappingRecord> sdMappingRecords = DbStructureMappingRecord.SelectList(
                    _db,
                    SourceStructureKey: sourceSd.Key,
                    TargetStructureKey: targetSd.Key);

                // if there are no records, we need to create one
                if (sdMappingRecords.Count == 0)
                {
                    (string idLong, string idShort) = XVerProcessor.GenerateArtifactId(
                        _sourcePackage.ShortName,
                        sourceSd.Id,
                        _targetPackage.ShortName,
                        targetSd.Id);

                    DbStructureMappingRecord sdMappingRec = new()
                    {
                        Key = DbStructureMappingRecord.GetIndex(),
                        SourceFhirPackageKey = _sourcePackage.Key,
                        SourceStructureKey = sourceSd.Key,
                        SourceStructureId = sourceSd.Id,

                        TargetFhirPackageKey = _targetPackage.Key,
                        TargetStructureKey = targetSd.Key,
                        TargetStructureId = targetSd.Id,

                        FmlExists = true,
                        FmlUrl = _fmlUrl,
                        FmlFilename = _fmlFilename,

                        Relationship = initialRelationship,
                        ConceptDomainRelationship = initialRelationship,
                        ValueDomainRelationship = null,
                        ComputedRelationship = null,

                        OriginatingConceptMapUrlsLiteral = null,
                        IdLong = idLong,
                        IdShort = idShort,
                        Url = $"http://hl7.org/fhir/{_sourcePackage.FhirVersionShort}/ConceptMap/{idLong}",
                        Name = FhirSanitizationUtils.ReformatIdForName(idLong),
                        Title = $"Concept Map of FHIR {_sourcePackage.ShortName} resource {sourceSd.Name} to FHIR {_targetPackage.ShortName}"
                    };

                    sdMappingRecsToAdd.Add(sdMappingRec);
                    sdMappingRecords.Add(sdMappingRec);
                }

                DbStructureMappingRecord relevantMap = sdMappingRecords[0];

                // check to see if there are existing element mapping records
                List<DbElementMappingRecord> edMappingRecords = sourceEd is null
                    ? []
                    : DbElementMappingRecord.SelectList(
                        _db,
                        SourceElementKey: sourceEd.Key,
                        TargetElementKey: targetEd?.Key);

                // if there are no records, we need to create one
                if (edMappingRecords.Count == 0)
                {
                    DbElementMappingRecord edMappingRec = new()
                    {
                        Key = DbElementMappingRecord.GetIndex(),
                        ResourceMapKey = relevantMap.Key,
                        SourceFhirPackageKey = _sourcePackage.Key,
                        SourceElementKey = sourceEd?.Key,
                        SourceElementId = sourceEd?.Id ?? sourcePath,
                        TargetFhirPackageKey = _targetPackage.Key,
                        TargetElementKey = targetEd?.Key,
                        TargetElementId = targetEd?.Id ?? targetPath,

                        OriginatingConceptMapUrlsLiteral = null,

                        Relationship = null,
                        ConceptDomainRelationship = null,
                        ValueDomainRelationship = null,
                        ComputedRelationship = null,

                        ElementTypeChange = null,
                        TypesAddedLiteral = null,
                        TypesRemovedLiteral = null,
                        TypesIdenticalLiteral = null,
                        TypesMappedLiteral = null,

                        ReferenceTargetChange = null,
                        ReferenceTargetsAddedLiteral = null,
                        ReferenceTargetsRemovedLiteral = null,
                        ReferenceTargetsIdenticalLiteral = null,
                        ReferenceTargetsMappedLiteral = null,

                        BindingStrengthChange = null,
                        BindingBecameRequired = null,
                        BindingNoLongerRequired = null,
                        BindingTargetChange = null,
                        BoundValueSetMapKey = null,

                        MaxCardinalityChange = null,
                        BecameProhibited = null,
                        BecameMandatory = null,
                        BecameOptional = null,
                        BecameArray = null,
                        BecameScalar = null,
                    };
                    edMappingRecsToAdd.Add(edMappingRec);
                }
            }
        }

        if (sdMappingRecsToAdd.Count > 0 || edMappingRecsToAdd.Count > 0)
        {
            _logger.LogInformation($"Maps added from FML for {_name}: {sdMappingRecsToAdd.Count} structures, {edMappingRecsToAdd.Count} elements");
        }

        sdMappingRecsToAdd.Insert(_db, ignoreDuplicates: true, insertPrimaryKey: true);
        edMappingRecsToAdd.Insert(_db, ignoreDuplicates: true, insertPrimaryKey: true);
    }

}
