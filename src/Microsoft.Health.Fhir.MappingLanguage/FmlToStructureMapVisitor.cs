// <copyright file="FmlToStructureMapVisitor.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CA = System.Diagnostics.CodeAnalysis;

using static FmlMappingParser;
using Antlr4.Runtime;
using Microsoft.Health.Fhir.CodeGenCommon.FhirExtensions;
using Hl7.Fhir.Model;
using System.Diagnostics.Metrics;

namespace Microsoft.Health.Fhir.MappingLanguage;

public class FmlToStructureMapVisitor : FmlMappingBaseVisitor<object>
{
    /// <summary>
    /// (Immutable) Variable name when none is present - not sure why this is necessary.
    /// </summary>
    /// <remarks>
    /// https://github.com/hapifhir/org.hl7.fhir.core/blob/master/org.hl7.fhir.r5/src/main/java/org/hl7/fhir/r5/utils/structuremap/StructureMapUtilities.java
    /// </remarks>
    private const string _unspecifiedVarName = "vvv";


    private StructureMap? _map = null;

    public StructureMap? ParsedStructureMap => _map;

    public override object VisitStructureMap([NotNull] StructureMapContext ctx)
    {
        _map = new();

        return base.VisitStructureMap(ctx);
    }

    public override object VisitHeader([NotNull] HeaderContext ctx)
    {
        // old header format id
        if (ctx.mapId()?.FirstOrDefault() is MapIdContext mapIdCtx)
        {
            // markdown type - join with newlines
            string comment = string.Join('\n', mapIdCtx.LINE_COMMENT()?.Select(c => c.GetText()[2..]) ?? Enumerable.Empty<string>());
            _map!.Description = comment;

            _map!.Url = ExtractString(mapIdCtx.url());
            _map!.Id = ExtractString(mapIdCtx.identifier());
        }

        // new header format url
        if (ctx.mapUrl()?.FirstOrDefault() is MapUrlContext mapUrlCtx)
        {
            _map!.Url = ExtractString(mapUrlCtx);
        }

        // new header format name
        if ((ctx.mapName()?.FirstOrDefault() is MapNameContext mapNameCtx) &&
            ExtractString(mapNameCtx) is string mapName)
        {
            _map!.Id = mapName;
            _map!.Name = mapName;
        }

        // new header format title
        if (ctx.mapTitle()?.FirstOrDefault() is MapTitleContext mapTitleCtx)
        {
            _map!.Title = ExtractString(mapTitleCtx);
        }

        // new header format status
        if ((ctx.mapStatus()?.FirstOrDefault() is MapStatusContext mapStatusCtx) &&
            (ExtractString(mapStatusCtx) is string mapStatus))
        {
            _map!.Status = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<PublicationStatus>(mapStatus);
        }

        // new header format description
        if (ctx.mapDescription()?.FirstOrDefault() is MapDescriptionContext mapDescriptionCtx)
        {
            _map!.Description = ExtractString(mapDescriptionCtx);
        }


        return base.VisitHeader(ctx);
    }

    public override object VisitStructure([NotNull] StructureContext ctx)
    {
        StructureMap.StructureComponent sc = new();

        // string type - join with spaces
        string comment = string.Join(' ', ctx.LINE_COMMENT()?.Select(c => c.GetText()[2..]) ?? Enumerable.Empty<string>());
        sc.Documentation = comment;

        if ((ctx.url() is UrlContext urlCtx) &&
            TryGetStringValue(urlCtx, out string? url))
        {
            sc.Url = url;
        }

        if (ctx.structureAlias() is StructureAliasContext aliasCtx)
        {
            sc.Alias = ExtractString(aliasCtx);
        }

        if ((ctx.modelMode() is ModelModeContext modelModeCtx) &&
            (ExtractString(modelModeCtx) is string modelMode))
        {
            sc.Mode = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<StructureMap.StructureMapModelMode>(modelMode);
        }

        _map!.Structure.Add(sc);

        return base.VisitStructure(ctx);
    }

    public override object VisitImports([NotNull] ImportsContext ctx)
    {
        if (ExtractString(ctx) is string import)
        {
            // markdown type - join with newlines
            string comment = string.Join('\n', ctx.LINE_COMMENT()?.Select(c => c.GetText()[2..]) ?? Enumerable.Empty<string>());

            Canonical ci = new Canonical(import);
            if (!string.IsNullOrEmpty(comment))
            {
                ci.Extension.Add(new Extension(CommonDefinitions.ExtUrlComment, new Markdown(comment)));
            }

            _map!.ImportElement.Add(ci);
        }

        return base.VisitImports(ctx);
    }

    public override object VisitConst([NotNull] ConstContext ctx)
    {
        string id = ctx.ID().GetText();
        string fp = ctx.fhirPath().GetText();

        if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(fp))
        {
            StructureMap.ConstComponent cc = new();

            cc.Name = id;
            cc.Value = fp;

            // markdown type - join with newlines
            string comment = string.Join('\n', ctx.LINE_COMMENT()?.Select(c => c.GetText()[2..]) ?? Enumerable.Empty<string>());
            if (!string.IsNullOrEmpty(comment))
            {
                cc.Extension.Add(new Extension(CommonDefinitions.ExtUrlComment, new Markdown(comment)));
            }

            _map!.Const.Add(cc);
        }

        return base.VisitConst(ctx);
    }

    StructureMap.GroupComponent? _currentGroup = null;
    List<StructureMap.RuleComponent> _currentRules = [];

    public override object VisitGroup([NotNull] GroupContext ctx)
    {
        // create a new group - note that the subsequent Rule visitor will add to this group
        _currentGroup = new();
        _currentRules = [];

        // string type - join with spaces
        string comment = string.Join(' ', ctx.LINE_COMMENT()?.Select(c => c.GetText()[2..]) ?? Enumerable.Empty<string>());
        _currentGroup.Documentation = comment;

        _currentGroup.Name = ctx.ID()?.GetText();

        foreach (ParameterContext parameterCtx in ctx.parameters()?.parameter() ?? Array.Empty<ParameterContext>())
        {
            StructureMap.InputComponent inputParam = new();

            inputParam.Mode = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<StructureMap.StructureMapInputMode>(parameterCtx.inputMode()?.GetText());
            inputParam.Name = parameterCtx.ID()?.GetText();
            inputParam.Type = parameterCtx.type() is TypeContext typeCtx ? ExtractString(typeCtx) : null;

            _currentGroup.Input.Add(inputParam);
        }

        _currentGroup.Extends = ctx.extends() is ExtendsContext extendsCtx ? extendsCtx.ID()?.GetText() : null;

        if (ctx.typeMode() is TypeModeContext typeModeCtx)
        {
            string tm = typeModeCtx.GetText();
            switch (tm)
            {
                case "type+":
                case "<<type+>>":
                case "type-and-types":
                    _currentGroup.TypeMode = StructureMap.StructureMapGroupTypeMode.TypeAndTypes;
                    break;

                case "type":
                case "<<type>>":
                    _currentGroup.TypeMode = StructureMap.StructureMapGroupTypeMode.Types;
                    break;
            }
        }

        _map!.Group.Add(_currentGroup);

        return base.VisitGroup(ctx);
    }

    public override object VisitRule([NotNull] FmlMappingParser.RuleContext ctx)
    {
        if (_currentGroup is null)
        {
            throw new Exception("Rule found outside of a group");
        }

        StructureMap.RuleComponent rule = new();

        // string type - join with spaces
        string comment = string.Join(' ', ctx.LINE_COMMENT()?.Select(c => c.GetText()[2..]) ?? Enumerable.Empty<string>());
        rule.Documentation = comment;

        rule.Name = ctx.ruleName()?.ID()?.GetText() ?? string.Empty;

        foreach (RuleSourceContext ruleSourceCtx in ctx.ruleSources()?.children ?? Array.Empty<RuleSourceContext>())
        {
            StructureMap.SourceComponent rs = new();

            // sort out the actual source
            string[] sourceComponents = ruleSourceCtx.ruleContext()?.identifier()?.Select(i => i.GetText()).ToArray() ?? [];

            rs.Context = sourceComponents[0];
            rs.Element = sourceComponents.Length > 1 ? string.Join('.', sourceComponents[1..]) : sourceComponents.FirstOrDefault() ?? string.Empty;

            // if there is no rule name, we want to use the element name
            if (string.IsNullOrEmpty(rule.Name))
            {
                rule.Name = rs.Element;
            }

            if (ruleSourceCtx.sourceType()?.identifier()?.GetText() is string sourceType)
            {
                rs.Type = sourceType;
            }

            if (ruleSourceCtx.sourceCardinality() is SourceCardinalityContext sourceCardinalityCtx)
            {
                if (sourceCardinalityCtx.INTEGER()?.Payload is int min)
                {
                    rs.Min = min;
                }

                if (sourceCardinalityCtx.upperBound()?.GetText() is string max)
                {
                    rs.Max = max;
                }
            }

            if (ruleSourceCtx.sourceDefault()?.fhirPath()?.GetText() is string defaultValue)
            {
                rs.DefaultValue = defaultValue;
            }

            if (ruleSourceCtx.sourceListMode()?.GetText() is string listMode)
            {
                rs.ListMode = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<StructureMap.StructureMapSourceListMode>(listMode);
            }

            if (ruleSourceCtx.alias()?.identifier()?.GetText() is string alias)
            {
                rs.Variable = alias;
            }
            else
            {
                rs.Variable = _unspecifiedVarName;
            }

            if (ruleSourceCtx.whereClause()?.fhirPath()?.GetText() is string whereClause)
            {
                rs.Condition = whereClause;
            }

            if (ruleSourceCtx.checkClause()?.fhirPath()?.GetText() is string checkClause)
            {
                rs.Check = checkClause;
            }

            if (ruleSourceCtx.log()?.fhirPath()?.GetText() is string log)
            {
                rs.LogMessage = log;
            }

            rule.Source.Add(rs);
        }

        foreach (IParseTree ctxPt in ctx.ruleTargets()?.children ?? Array.Empty<RuleTargetContext>())
        {
            if (ctxPt is not RuleTargetContext ruleTargetCtx)
            {
                continue;
            }

            StructureMap.TargetComponent rt = new();

            string[] targetComponents = ruleTargetCtx.ruleContext()?.identifier()?.Select(i => i.GetText()).ToArray() ?? [];

            rt.Context = targetComponents[0];
            rt.Element = targetComponents.Length > 1 ? string.Join('.', targetComponents[1..]) : targetComponents.FirstOrDefault() ?? string.Empty;

            if (ruleTargetCtx.transform() is TransformContext transformCtx)
            {
                if (transformCtx.ruleContext() is RuleContextContext ruleCtx)
                {
                    Console.WriteLine($"transform context rule context: {ruleCtx.GetText()}");
                }

                //transformCtx.ruleContext()
                //null
                //transformCtx.invocation().identifier().GetText()
                //"translate"

                if (transformCtx.invocation() is InvocationContext invocationCtx)
                {
                    rt.Transform = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<StructureMap.StructureMapTransform>(invocationCtx.identifier().GetText());

                    foreach (ParamContext paramCtx in invocationCtx.paramList()?.param() ?? Enumerable.Empty<ParamContext>())
                    {
                        StructureMap.ParameterComponent param = new();

                        param.Value = ParseValueForParam(paramCtx.GetText());

                        rt.Parameter.Add(param);
                    }
                }
            }
            else
            {
                rt.Transform = StructureMap.StructureMapTransform.Create;
            }

            if (ruleTargetCtx.alias()?.identifier()?.GetText() is string alias)
            {
                rt.Variable = alias;
            }
            else
            {
                rt.Variable = _unspecifiedVarName;
            }

            rule.Target.Add(rt);
        }

        // check for dependent rules
        foreach (InvocationContext dependentCtx in ctx.dependent()?.invocation() ?? Enumerable.Empty<InvocationContext>())
        {
            StructureMap.DependentComponent dc = new();

            dc.Name = dependentCtx.identifier()?.GetText();

            foreach (ParamContext paramCtx in dependentCtx.paramList()?.param() ?? Enumerable.Empty<ParamContext>())
            {
                StructureMap.ParameterComponent param = new();

                param.Value = new FhirString(paramCtx.GetText());

                dc.Parameter.Add(param);
            }

            rule.Dependent.Add(dc);
        }

        // if we have nested into a rule, we need to add it there
        if (_currentRules.Count != 0)
        {
            _currentRules[^1].Rule.Add(rule);
        }
        else
        {
            _currentGroup.Rule.Add(rule);
        }

        return base.VisitRule(ctx);
    }

    private static DataType? ParseValueForParam(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        switch (value[0])
        {
            case '\'':
            case '"':
                return new FhirString(value[1..^1]);
            case '@':
            case '%':
            //case '-':
            //case '+':
                return new FhirString(value);

            case 't':
                return value.Equals("true", StringComparison.Ordinal) ? new FhirBoolean(true) : new Id(value);

            case 'f':
                return value.Equals("false", StringComparison.Ordinal) ? new FhirBoolean(true) : new Id(value);

            case '{':
                return (value.Length == 2 && value[1] == '}') ? new FhirString(value) : new Id(value);
        }

        if (int.TryParse(value, out var intVal))
        {
            return new Integer(intVal);
        }

        if (decimal.TryParse(value, out decimal decimalVal))
        {
            return new FhirDecimal(decimalVal);
        }

        return new Id(value);
    }


    private static bool TryGetStringValue(StringValueContext ctx, [CA.NotNullWhen(true)] out string? value)
    {
        // get to the terminal node we are interested in
        if ((ctx.ChildCount == 0) ||
            (ctx.children[0] is not ITerminalNode node))
        {
            value = null;
            return false;
        }

        switch (node.Symbol.Type)
        {
            case STRING:
                value = node.Symbol.Text;
                return true;

            case DOUBLE_QUOTED_STRING:
                value = node.Symbol.Text[1..^1];
                return true;

            default:
                value = null;
                return false;
        }
    }

    private static bool TryGetStringValue(IdentifierContext ctx, [CA.NotNullWhen(true)] out string? value)
    {
        // get to the terminal node we are interested in
        if ((ctx.ChildCount == 0) ||
            (ctx.children[0] is not ITerminalNode node))
        {
            value = null;
            return false;
        }

        switch (node.Symbol.Type)
        {
            case ID:
            case IDENTIFIER:
                value = node.Symbol.Text;
                return true;

            case DELIMITED_IDENTIFIER:
                value = node.Symbol.Text[1..^1];
                return true;

            default:
                value = null;
                return false;
        }
    }

    private static bool TryGetStringValue(UrlContext ctx, [CA.NotNullWhen(true)] out string? value)
    {
        // get to the terminal node we are interested in
        if ((ctx.ChildCount == 0) ||
            (ctx.children[0] is not ITerminalNode node))
        {
            value = null;
            return false;
        }

        switch (node.Symbol.Type)
        {
            case STRING:
                value = node.Symbol.Text;
                return true;

            case DOUBLE_QUOTED_STRING:
                value = node.Symbol.Text[1..^1];
                return true;

            default:
                value = null;
                return false;
        }
    }

    private static string ExtractString(ParserRuleContext ctx)
    {
        if (ctx.ChildCount == 0)
        {
            return ctx.GetText();
        }

        if ((ctx.GetRuleContext<StringValueContext>(0) is StringValueContext stringValueCtx) &&
            TryGetStringValue(stringValueCtx, out string? value))
        {
            return value;
        }

        if ((ctx.GetRuleContext<IdentifierContext>(0) is IdentifierContext identifierCtx) &&
            TryGetStringValue(identifierCtx, out value))
        {
            return value;
        }

        if ((ctx.GetRuleContext<UrlContext>(0) is UrlContext urlCtx) &&
            TryGetStringValue(urlCtx, out value))
        {
            return value;
        }

        return ctx.GetText();
    }
}
