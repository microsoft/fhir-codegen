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

    private Dictionary<string, HashSet<string>> _relatedPaths;

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

        _relatedPaths = [];
    }

    private record class FmlSymbolResolutionRecord
    {
        public required string Identifier { get; init; }
        public required string? Alias { get; init; }
        public required string? TypeIdentifier { get; init; }
        public required string ResolvedPath { get; init; }
        public required DbStructureDefinition Structure { get; init; }
        public required DbElement? Element { get; init; }
    }


    public void ProcessElementRelationships()
    {
        Dictionary<string, FmlSymbolResolutionRecord> structures = [];

        // process the initial structure declarations to get the aliases
        foreach ((string url, StructureDeclaration fmlStructure) in _fml.StructuresByUrl)
        {
            // extract the FHIR structure type from the URL
            string fhirStructureType = url.Split('/')[^1];

            while (!char.IsAsciiLetter(fhirStructureType[^1]))
            {
                fhirStructureType = fhirStructureType[..^1];

                if (fhirStructureType.Length == 0)
                {
                    throw new Exception($"Failed to parse FHIR structure type from URL: {url} in {_name} ({_fmlFilename})");
                }
            }

            // ensure the type exists in the correct package
            if (fmlStructure.ModelMode == StructureMap.StructureMapModelMode.Source)
            {
                DbStructureDefinition? sd = DbStructureDefinition.SelectSingle(
                    _db,
                    FhirPackageKey: _sourcePackage.Key,
                    Id: fhirStructureType);

                if (sd is null)
                {
                    throw new Exception($"Failed to resolve FML source structure: {fhirStructureType} in {_name} ({_fmlFilename})");
                }

                DbElement? ed = DbElement.SelectSingle(
                    _db,
                    FhirPackageKey: _sourcePackage.Key,
                    Id: fhirStructureType);

                if (fmlStructure.Alias is not null)
                {
                    structures[fmlStructure.Alias] = new()
                    {
                        Identifier = fmlStructure.Alias,
                        Alias = fmlStructure.ModelModeLiteral,
                        TypeIdentifier = fhirStructureType,
                        ResolvedPath = sd.Id,
                        Structure = sd,
                        Element = ed,
                    };
                }

                structures[fmlStructure.ModelModeLiteral] = new()
                {
                    Identifier = fmlStructure.ModelModeLiteral,
                    Alias = fmlStructure.Alias,
                    TypeIdentifier = fhirStructureType,
                    ResolvedPath = sd.Id,
                    Structure = sd,
                    Element = ed,
                };
            }

            if (fmlStructure.ModelMode == StructureMap.StructureMapModelMode.Target)
            {
                DbStructureDefinition? sd = DbStructureDefinition.SelectSingle(
                    _db,
                    FhirPackageKey: _targetPackage.Key,
                    Id: fhirStructureType);

                if (sd is null)
                {
                    throw new Exception($"Failed to resolve FML target structure: {fhirStructureType} in {_name} ({_fmlFilename})");
                }

                DbElement? ed = DbElement.SelectSingle(
                    _db,
                    FhirPackageKey: _targetPackage.Key,
                    Id: fhirStructureType);

                if (fmlStructure.Alias is not null)
                {
                    structures[fmlStructure.Alias] = new()
                    {
                        Identifier = fmlStructure.Alias,
                        Alias = fmlStructure.ModelModeLiteral,
                        TypeIdentifier = fhirStructureType,
                        ResolvedPath = sd.Id,
                        Structure = sd,
                        Element = ed,
                    };
                }

                structures[fmlStructure.ModelModeLiteral] = new()
                {
                    Identifier = fmlStructure.ModelModeLiteral,
                    Alias = fmlStructure.Alias,
                    TypeIdentifier = fhirStructureType,
                    ResolvedPath = sd.Id,
                    Structure = sd,
                    Element = ed,
                };
            }
        }

        // process each of the groups in the FML to extract prefixPath maps
        foreach ((string groupName, GroupDeclaration group) in _fml.GroupsByName)
        {
            Dictionary<string, FmlSymbolResolutionRecord> sourceSymbols = [];
            Dictionary<string, FmlSymbolResolutionRecord> targetSymbols = [];
            List<FmlSymbolResolutionRecord> parameters = [];

            // check for groups that process structures directly (no other variables added yet)
            foreach (GroupParameter gp in group.Parameters)
            {
                // we only care about root-level groups here, which require type identifiers
                if (gp.TypeIdentifier is null)
                {
                    continue;
                }

                // at top level, all arugments need to be from structures
                switch (gp.InputMode)
                {
                    case StructureMap.StructureMapInputMode.Source:
                        {
                            if (resolveFmlSymbol(gp.Identifier, null, gp.TypeIdentifier, structures, gp.InputMode.Value)
                                is FmlSymbolResolutionRecord r)
                            {
                                r = r with
                                {
                                    Identifier = gp.Identifier,
                                };

                                sourceSymbols[gp.Identifier] = r;
                                //parameters.Add(r);
                            }
                        }
                        break;

                    case StructureMap.StructureMapInputMode.Target:
                        {
                            if (resolveFmlSymbol(gp.Identifier, null, gp.TypeIdentifier, structures, gp.InputMode.Value)
                                is FmlSymbolResolutionRecord r)
                            {
                                r = r with
                                {
                                    Identifier = gp.Identifier,
                                };

                                targetSymbols[gp.Identifier] = r;
                                //parameters.Add(r);
                            }
                        }
                        break;
                }
            }

            if ((sourceSymbols.Count == 0) ||
                (targetSymbols.Count == 0))
            {
                continue;
            }

            processFmlGroup(
                group,
                sourceSymbols,
                targetSymbols,
                []);
        }

        reconcileElementMapFmlPathsInDb();
    }

    private FmlSymbolResolutionRecord? resolveFmlSymbol(
        string literal,
        string? alias,
        string? typeIdentifier,
        Dictionary<string, FmlSymbolResolutionRecord> symbolTable,
        Hl7.Fhir.Model.StructureMap.StructureMapInputMode mode)
    {
        int packageKey = mode == StructureMap.StructureMapInputMode.Source
            ? _sourcePackage.Key
            : _targetPackage.Key;

        // check for a type identifier
        if (typeIdentifier is not null)
        {
            if (symbolTable.TryGetValue(typeIdentifier, out FmlSymbolResolutionRecord? typeRec))
            {
                return typeRec;
            }

            // try to resolve the type from the database
            DbStructureDefinition? sd = DbStructureDefinition.SelectSingle(
                _db,
                FhirPackageKey: packageKey,
                Id: typeIdentifier);
            if (sd is not null)
            {
                DbElement? ed = DbElement.SelectSingle(
                    _db,
                    FhirPackageKey: packageKey,
                    Id: typeIdentifier);

                return new()
                {
                    Identifier = literal,
                    Alias = alias,
                    TypeIdentifier = typeIdentifier,
                    ResolvedPath = sd.Id,
                    Structure = sd,
                    Element = ed,
                };
            }
        }

        // check for an alias
        if ((alias is not null) &&
            symbolTable.TryGetValue(alias, out FmlSymbolResolutionRecord? aliasRec))
        {
            return aliasRec;
        }

        string[] components = literal.Split('.');

        if (symbolTable.TryGetValue(components[0], out FmlSymbolResolutionRecord? prefixRec))
        {
            string path = string.Join('.', [prefixRec.ResolvedPath, .. components[1..]]);

            // try to resolve from the database
            DbElement? ed = DbElement.SelectSingle(
                _db,
                FhirPackageKey: packageKey,
                Id: path);

            // if not found, check with the choice type literal appended
            ed ??= DbElement.SelectSingle(
                _db,
                FhirPackageKey: packageKey,
                Id: path + "[x]");

            if (ed is not null)
            {
                DbStructureDefinition? sd = DbStructureDefinition.SelectSingle(
                    _db,
                    FhirPackageKey: packageKey,
                    Key: ed.StructureKey);
                if (sd is not null)
                {
                    return new()
                    {
                        Identifier = literal,
                        Alias = alias,
                        TypeIdentifier = typeIdentifier,
                        ResolvedPath = ed.Path,
                        Structure = sd,
                        Element = ed,
                    };
                }
            }
        }

        // iterate up to see if we can substitute something in the chain
        if (prefixRec is not null)
        {
            string[] testComponents = [..prefixRec.ResolvedPath.Split('.'), ..components[1..]];

            for (int splitIndex = testComponents.Length - 1; splitIndex > 0; splitIndex--)
            {
                string prefixPath = string.Join('.', testComponents[0..splitIndex]);

                // try to resolve from the database
                DbElement? prefixEd = DbElement.SelectSingle(
                    _db,
                    FhirPackageKey: packageKey,
                    Id: prefixPath);

                // if not found, check with the choice type literal appended
                prefixEd ??= DbElement.SelectSingle(
                    _db,
                    FhirPackageKey: packageKey,
                    Id: prefixPath + "[x]");

                if (prefixEd is null)
                {
                    continue;
                }

                // get the list of types for this element
                List<DbElementType> elementTypes = DbElementType.SelectList(
                    _db,
                    ElementKey: prefixEd.Key);

                foreach (DbElementType et in elementTypes)
                {
                    DbStructureDefinition? prefixTypeSd = DbStructureDefinition.SelectSingle(
                        _db,
                        FhirPackageKey: packageKey,
                        Id: et.TypeName ?? et.Literal);
                    if (prefixTypeSd is null)
                    {
                        continue;
                    }

                    // check to see if we can resolve the rest of the element from this root
                    string postfixPath = string.Join('.', [prefixTypeSd.Id, ..testComponents[splitIndex..]]);

                    // try to resolve from the database
                    DbElement? postfixEd = DbElement.SelectSingle(
                        _db,
                        FhirPackageKey: packageKey,
                        Id: postfixPath);

                    // if not found, check with the choice type literal appended
                    postfixEd ??= DbElement.SelectSingle(
                        _db,
                        FhirPackageKey: packageKey,
                        Id: postfixPath + "[x]");

                    if (postfixEd is null)
                    {
                        continue;
                    }

                    // since we know it is valid, we can return something - but we do not want to nest into types since that destroys the maps
                    return new()
                    {
                        Identifier = literal,
                        Alias = alias,
                        TypeIdentifier = typeIdentifier,
                        ResolvedPath = string.Join('.', [prefixRec.ResolvedPath, .. components[1..]]),
                        Structure = prefixRec.Structure,
                        Element = null,
                    };
                }
            }
        }


        // likely nested into a datatype, can ignore for now
        return null;
    }

    private void processFmlGroup(
        GroupDeclaration group,
        Dictionary<string, FmlSymbolResolutionRecord> sourceSymbols,
        Dictionary<string, FmlSymbolResolutionRecord> targetSymbols,
        List<FmlSymbolResolutionRecord> parameters,
        HashSet<string>? groupCallStack = null)
    {
        groupCallStack ??= [];
        if (!groupCallStack.Add(group.Name))
        {
            return;
        }

        // explicit parameters always override discovery
        if (parameters.Count > 0)
        {
            sourceSymbols.Clear();
            targetSymbols.Clear();

            // match parameters to the correct dictionary and symbol
            if (parameters.Count != group.Parameters.Count)
            {
                // if we do not have the correct number of parameters, we nested into a type - ignore for now
                // unwind the stack
                groupCallStack.Remove(group.Name);
                return;
            }

            for (int i = 0; i < parameters.Count; i++)
            {
                GroupParameter gp = group.Parameters[i];
                FmlSymbolResolutionRecord p = parameters[i];

                switch (gp.InputMode)
                {
                    case StructureMap.StructureMapInputMode.Source:
                        {
                            sourceSymbols[gp.Identifier] = p with
                            {
                                Identifier = gp.Identifier,
                                Alias = null,
                                TypeIdentifier = gp.TypeIdentifier,
                            };
                        }
                        break;

                    case StructureMap.StructureMapInputMode.Target:
                        {
                            targetSymbols[gp.Identifier] = p with
                            {
                                Identifier = gp.Identifier,
                                Alias = null,
                                TypeIdentifier = gp.TypeIdentifier,
                            };
                        }
                        break;
                }
            }
        }

        // travers each expression in the group
        foreach (GroupExpression expression in group.Expressions)
        {
            if (expression.SimpleCopyExpression is not null)
            {
                try
                {
                    processSimpleCopyExpression(
                        group,
                        expression.SimpleCopyExpression,
                        sourceSymbols,
                        targetSymbols);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($" <<< processSimpleCopyExpression for {_fmlFilename} Line: {expression.Line} caught: {ex.Message}");
                }
                continue;
            }

            if (expression.MappingExpression is not null)
            {
                try
                {
                    processComplexMappingExpression(
                        group,
                        expression.MappingExpression,
                        sourceSymbols,
                        targetSymbols,
                        groupCallStack);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($" <<< processComplexMappingExpression for {_fmlFilename} Line: {expression.Line} caught: {ex.Message}");
                }

                continue;
            }
        }

        // unwind the stack
        groupCallStack.Remove(group.Name);
    }

    private void processComplexMappingExpression(
        GroupDeclaration group,
        FmlGroupExpression expression,
        Dictionary<string, FmlSymbolResolutionRecord> sourceSymbols,
        Dictionary<string, FmlSymbolResolutionRecord> targetSymbols,
        HashSet<string> groupCallStack)
    {
        Dictionary<string, FmlSymbolResolutionRecord> localSourceSymbols = [];
        Dictionary<string, FmlSymbolResolutionRecord> localTargetSymbols = [];

        FmlSymbolResolutionRecord? lastResolvedSource = null;
        FmlSymbolResolutionRecord? lastResolvedTarget = null;

        foreach (FmlExpressionSource expressionSource in expression.Sources)
        {
            // check for odd expressions we do not want to handle
            if (string.IsNullOrEmpty(expressionSource.Identifier) ||
                !expressionSource.Identifier.Contains('.'))
            {
                continue;
            }

            FmlSymbolResolutionRecord? resolved = resolveFmlSymbol(
                expressionSource.Identifier,
                expressionSource.Alias,
                expressionSource.TypeIdentifier,
                sourceSymbols,
                StructureMap.StructureMapInputMode.Source);

            resolved ??= resolveFmlSymbol(
                expressionSource.Identifier,
                expressionSource.Alias,
                expressionSource.TypeIdentifier,
                localSourceSymbols,
                StructureMap.StructureMapInputMode.Source);

            if (resolved is null)
            {
                continue;
            }

            if (resolved.Alias is not null)
            {
                localSourceSymbols[resolved.Alias] = resolved;
            }
            else
            {
                localSourceSymbols[resolved.Identifier] = resolved;
            }

            lastResolvedSource = resolved;
        }

        foreach (FmlExpressionTarget expressionTarget in expression.Targets)
        {
            // check for odd expressions we do not want to handle
            if (string.IsNullOrEmpty(expressionTarget.Identifier) ||
                !expressionTarget.Identifier.Contains('.'))
            {
                continue;
            }

            FmlSymbolResolutionRecord? resolved = resolveFmlSymbol(
                expressionTarget.Identifier,
                expressionTarget.Alias,
                null,
                targetSymbols,
                StructureMap.StructureMapInputMode.Target);

            resolved ??= resolveFmlSymbol(
                expressionTarget.Identifier,
                expressionTarget.Alias,
                null,
                localTargetSymbols,
                StructureMap.StructureMapInputMode.Target);

            if (resolved is null)
            {
                continue;
            }

            if (resolved.Alias is not null)
            {
                localTargetSymbols[resolved.Alias] = resolved;
            }
            else
            {
                localTargetSymbols[resolved.Identifier] = resolved;
            }

            lastResolvedTarget = resolved;
        }

        if ((lastResolvedSource is null) ||
            (lastResolvedTarget is null))
        {
            // either the source or the target is unresolved, we can ignore for now
            return;
        }

        if (localSourceSymbols.Count == 1)
        {
            if (!_relatedPaths.TryGetValue(lastResolvedSource.ResolvedPath, out HashSet<string>? relatedTargetPaths))
            {
                relatedTargetPaths = [];
                _relatedPaths.Add(lastResolvedSource.ResolvedPath, relatedTargetPaths);
            }

            relatedTargetPaths.Add(lastResolvedTarget.ResolvedPath);
        }
        else
        {
            throw new Exception("Is this possible?");
        }

        // check for dependent expression
        if (expression.DependentExpression is not null)
        {
            // iterate over the invocations
            foreach (FmlInvocation dependentInvocation in expression.DependentExpression.Invocations)
            {
                string name = dependentInvocation.Identifier;

                if (_fml.GroupsByName.TryGetValue(name, out GroupDeclaration? dependentGroup))
                {
                    List<FmlSymbolResolutionRecord> dependentParams = [];
                    foreach (FmlInvocationParam invocationParam in dependentInvocation.Parameters)
                    {
                        if (invocationParam.Identifier is null)
                        {
                            continue;
                        }

                        FmlSymbolResolutionRecord? resolvedParam = null;

                        if (localSourceSymbols.TryGetValue(invocationParam.Identifier, out resolvedParam))
                        {
                            dependentParams.Add(resolvedParam);
                            continue;
                        }

                        if (localTargetSymbols.TryGetValue(invocationParam.Identifier, out resolvedParam))
                        {
                            dependentParams.Add(resolvedParam);
                            continue;
                        }

                        if (sourceSymbols.TryGetValue(invocationParam.Identifier, out resolvedParam))
                        {
                            dependentParams.Add(resolvedParam);
                            continue;
                        }

                        if (targetSymbols.TryGetValue(invocationParam.Identifier, out resolvedParam))
                        {
                            dependentParams.Add(resolvedParam);
                            continue;
                        }

                        // if we failed to resolve, it is something that nested into a type - ignore for now
                        continue;
                    }

                    processFmlGroup(
                        dependentGroup,
                        [],
                        [],
                        dependentParams,
                        groupCallStack);
                }

            }
        }

    }

    private void processSimpleCopyExpression(
        GroupDeclaration group,
        MapSimpleCopyExpression expression,
        Dictionary<string, FmlSymbolResolutionRecord> sourceSymbols,
        Dictionary<string, FmlSymbolResolutionRecord> targetSymbols)
    {
        if (resolveFmlSymbol(expression.Source, null, null, sourceSymbols, StructureMap.StructureMapInputMode.Source)
            is not FmlSymbolResolutionRecord sourceResovled)
        {
            _logger.LogInformation(
                $" <<< processSimpleCopyExpression for {_fmlFilename} Line: {expression.Line}" +
                $" Failed to resolve source `{expression.Source}` in group {group.Name}");
            return;
        }

        if (resolveFmlSymbol(expression.Target, null, null, targetSymbols, StructureMap.StructureMapInputMode.Target)
            is not FmlSymbolResolutionRecord targetResolved)
        {
            _logger.LogInformation(
                $" <<< processSimpleCopyExpression for {_fmlFilename} Line: {expression.Line}" +
                $"Failed to resolve target `{expression.Target}` in group {group.Name}");
            return;
        }

        if (!_relatedPaths.TryGetValue(sourceResovled.ResolvedPath, out HashSet<string>? relatedTargetPaths))
        {
            relatedTargetPaths = [];
            _relatedPaths.Add(sourceResovled.ResolvedPath, relatedTargetPaths);
        }

        relatedTargetPaths.Add(targetResolved.ResolvedPath);
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

    private void reconcileElementMapFmlPathsInDb()
    {
        List<DbStructureMappingRecord> sdMappingRecsToAdd = [];
        List<DbElementMappingRecord> edMappingRecsToAdd = [];

        // look for elements that target other elements
        foreach ((string sourcePath, HashSet<string> targetPaths) in _relatedPaths)
        {
            // skip items with no targets
            if (targetPaths.Count == 0)
            {
                continue;
            }

            // set the default relationship based on the number of targets
            ConceptMap.ConceptMapRelationship initialRelationship = targetPaths.Count == 1
                ? ConceptMap.ConceptMapRelationship.Equivalent
                : ConceptMap.ConceptMapRelationship.SourceIsBroaderThanTarget;

            foreach (string targetPath in targetPaths)
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

                // make sure we have a target structure
                if (DbStructureDefinition.SelectSingle(_db, FhirPackageKey: _targetPackage.Key, Id: targetTypeName)
                    is not DbStructureDefinition targetSd)
                {
                    _logger.LogError($"Could not resolve target structure {targetTypeName} for {targetPath} in {_sourcePackage.ShortName}to{_targetPackage.ShortName}");
                    continue;
                }

                // try to get a source element
                DbElement? sourceEd = DbElement.SelectSingle(_db, FhirPackageKey: _sourcePackage.Key, Id: sourcePath);
                sourceEd ??= DbElement.SelectSingle(_db, FhirPackageKey: _sourcePackage.Key, Id: sourcePath + "[x]");

                //// make sure the source element exists
                //if (sourceEd is null)
                //{
                //    _logger.LogWarning($"Note: could not resolve source path {sourcePath} for {sourceTypeName} in {_sourcePackage.ShortName}to{_targetPackage.ShortName}");
                //}

                //// skip elements that have child elements - will either be picked up by dependent groups or are not relevant
                //if (sourceEd?.ChildElementCount > 0)
                //{
                //    continue;
                //}

                // try to get a target element
                DbElement? targetEd = DbElement.SelectSingle(_db, FhirPackageKey: _targetPackage.Key, Id: targetPath);
                targetEd ??= DbElement.SelectSingle(_db, FhirPackageKey: _targetPackage.Key, Id: targetPath + "[x]");
                // make sure the target element exists
                //if (targetEd is null)
                //{
                //    _logger.LogWarning($"Note: Could not resolve target path {targetPath} for {targetTypeName} in {_sourcePackage.ShortName}to{_targetPackage.ShortName}");
                //}

                //// skip elements that have child elements - will either be picked up by dependent groups or are not relevant
                //if (targetEd?.ChildElementCount > 0)
                //{
                //    continue;
                //}

                DbStructureMappingRecord structureMappingRec;

                // check to see if there are existing mapping records for the structures
                List<DbStructureMappingRecord> sdMappingRecords = DbStructureMappingRecord.SelectList(
                    _db,
                    SourceFhirPackageKey: _sourcePackage.Key,
                    SourceStructureKey: sourceSd.Key,
                    TargetFhirPackageKey: _targetPackage.Key,
                    TargetStructureKey: targetSd.Key);

                // if there are no records, we need to create one
                if (sdMappingRecords.Count == 0)
                {
                    (string idLong, string idShort) = XVerProcessor.GenerateArtifactId(
                        _sourcePackage.ShortName,
                        sourceSd.Id,
                        _targetPackage.ShortName,
                        targetSd.Id);

                    // check for maps to other targets and no-maps
                    int otherStructureMapCount = DbStructureMappingRecord.SelectCount(
                        _db,
                        SourceFhirPackageKey: _sourcePackage.Key,
                        SourceStructureKey: sourceSd.Key,
                        TargetFhirPackageKey: _targetPackage.Key);

                    // if there are *other* maps for this source structure, assume the FML is overly-ambitious and log a warning
                    if (otherStructureMapCount > 0)
                    {
                        // if we do not have a structure mapping record by this point in processing, assume the FML is overly-ambitious
                        _logger.LogWarning(
                            $"FML wants to create {idLong}, but" +
                            $" {_sourcePackage.ShortName}:{sourceSd.Name} has relationships for" +
                            $" {_targetPackage.ShortName} that do not include {targetSd.Name}.");

                        continue;
                    }

                    structureMappingRec = new()
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
                        FmlFilename = Path.GetFileName(_fmlFilename),

                        ExplicitNoMap = false,
                        Relationship = initialRelationship,

                        OriginatingConceptMapUrlsLiteral = null,
                        IdLong = idLong,
                        IdShort = idShort,
                        Url = $"http://hl7.org/fhir/{_sourcePackage.FhirVersionShort}/ConceptMap/{idLong}",
                        Name = FhirSanitizationUtils.ReformatIdForName(idLong),
                        Title = $"Concept Map of FHIR {_sourcePackage.ShortName} resource {sourceSd.Name} to FHIR {_targetPackage.ShortName}"
                    };

                    sdMappingRecsToAdd.Add(structureMappingRec);
                    sdMappingRecords.Add(structureMappingRec);
                }
                else
                {
                    structureMappingRec = sdMappingRecords[0];

                    if (structureMappingRec.FmlFilename != Path.GetFileName(_fmlFilename))
                    {
                        structureMappingRec.FmlExists = true;
                        structureMappingRec.FmlUrl = _fmlUrl;
                        structureMappingRec.FmlFilename = Path.GetFileName(_fmlFilename);

                        structureMappingRec.Update(_db);
                    }
                }

                // check to see if there are existing element mapping records
                List<DbElementMappingRecord> edMappingRecords;

                if ((sourceEd is not null) && (targetEd is not null))
                {
                    // check for explicit matches
                    edMappingRecords = DbElementMappingRecord.SelectList(
                        _db,
                        StructureMappingKey: structureMappingRec.Key,
                        SourceElementKey: sourceEd.Key,
                        TargetElementKey: targetEd.Key);

                    // add no-map records
                    edMappingRecords.AddRange(DbElementMappingRecord.SelectList(
                        _db,
                        SourceFhirPackageKey: _sourcePackage.Key,
                        StructureMappingKey: structureMappingRec.Key,
                        TargetFhirPackageKey: _targetPackage.Key,
                        SourceElementKey: sourceEd.Key,
                        TargetElementIdIsNull: true));
                }
                else
                {
                    // check for path-based matches
                    edMappingRecords = DbElementMappingRecord.SelectList(
                        _db,
                        SourceFhirPackageKey: _sourcePackage.Key,
                        TargetFhirPackageKey: _targetPackage.Key,
                        SourceElementId: sourceEd?.Id ?? sourcePath,
                        TargetElementId: targetEd?.Id ?? targetPath);

                    // add no-map records
                    edMappingRecords.AddRange(DbElementMappingRecord.SelectList(
                        _db,
                        SourceFhirPackageKey: _sourcePackage.Key,
                        StructureMappingKey: structureMappingRec.Key,
                        SourceElementId: sourceEd?.Id ?? sourcePath,
                        TargetFhirPackageKey: _targetPackage.Key,
                        TargetElementIdIsNull: true));
                }

                // if there are no records, we need to create one
                if (edMappingRecords.Count == 0)
                {
                    DbElementMappingRecord edMappingRec = new()
                    {
                        Key = DbElementMappingRecord.GetIndex(),
                        StructureMappingKey = structureMappingRec.Key,
                        SourceFhirPackageKey = _sourcePackage.Key,
                        SourceElementKey = sourceEd?.Key,
                        SourceElementId = sourceEd?.Id ?? sourcePath,
                        TargetFhirPackageKey = _targetPackage.Key,
                        TargetElementKey = targetEd?.Key,
                        TargetElementId = targetEd?.Id ?? targetPath,

                        ElementKeys = getKeyArray(_sourcePackage, _targetPackage, sourceEd?.Key, targetEd?.Key),

                        OriginatingConceptMapUrlsLiteral = null,
                        OriginatingFmlUrlsLiteral = _fmlUrl,

                        ExplicitNoMap = false,
                        Relationship = null,
                    };
                    edMappingRecsToAdd.Add(edMappingRec);
                }
            }
        }

        if (sdMappingRecsToAdd.Count > 0 || edMappingRecsToAdd.Count > 0)
        {
            sdMappingRecsToAdd.Insert(_db, ignoreDuplicates: true, insertPrimaryKey: true);
            edMappingRecsToAdd.Insert(_db, ignoreDuplicates: true, insertPrimaryKey: true);

            _logger.LogInformation($"Maps added from FML for {_name}: {sdMappingRecsToAdd.Count} structures, {edMappingRecsToAdd.Count} elements");
        }
    }

    private int?[] getKeyArray(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        int? sourceKey,
        int? targetKey)
    {
        int?[] keyArray = [null, null, null, null, null, null];
        keyArray[sourcePackage.PackageArrayIndex] = sourceKey;
        keyArray[targetPackage.PackageArrayIndex] = targetKey;
        return keyArray;
    }

}
