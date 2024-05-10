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
using Newtonsoft.Json.Linq;
using System.Reflection;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using System.Globalization;

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

    public override object VisitMetadataDeclaration([NotNull] MetadataDeclarationContext ctx)
    {
        Console.WriteLine(
          $"Meta {ctx.qualifiedIdentifier()?.GetText()}:" +
          $" {ctx.literal()?.GetText() ?? ctx.constant()?.GetText() ?? ctx.markdownLiteral()?.GetText()}" +
          $" comment: {ctx.INLINE_COMMENT()?.GetText()}");

        string elementPath = ctx.qualifiedIdentifier()?.GetText() ?? string.Empty;

        if (string.IsNullOrEmpty(elementPath))
        {
            return base.VisitMetadataDeclaration(ctx);
        }

        ParserRuleContext? c = (ParserRuleContext?)ctx.literal() ??
            (ParserRuleContext?)ctx.constant() ??
            (ParserRuleContext?)ctx.markdownLiteral() ??
            null;

        if (c is null)
        {
            return base.VisitMetadataDeclaration(ctx);
        }

        // need to get the correct type here (e.g., bool)
        // need to add utility function to strip quotes from types like DOUBLE_QUOTED_STRING
        //string value = ctx.literal()?.GetText() ?? ctx.constant()?.GetText() ?? ctx.markdownLiteral()?.GetText() ?? string.Empty;
        dynamic? value = GetValue(c);

        if (value is null)
        {
            return base.VisitMetadataDeclaration(ctx);
        }

        object? obj = _map!;

        string[] pathComponents = elementPath.Split('.');
        for(int i = 0; i < pathComponents.Length; i++)
        {
            if (obj is null)
            {
                return base.VisitMetadataDeclaration(ctx);
            }

            // TODO(ginoc): need to actually lookup the Firely name in case it is different
            string pathComponent = pathComponents[i].ToPascalCase();
            PropertyInfo? property = obj.GetType().GetProperty(pathComponent, BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
            {
                return base.VisitMetadataDeclaration(ctx);
            }

            if (i == pathComponents.Length - 1)
            {
                try
                {
                    if (property.PropertyType.IsAssignableFrom(value.GetType()))
                    {
                        // set the value
                        property.SetValue(obj, value);
                    }
                    else if (property.PropertyType.IsGenericType &&
                        (property.PropertyType.GetGenericArguments().FirstOrDefault() is Type t) &&
                        t.IsEnum)
                    {
                        property.SetValue(obj, Hl7.Fhir.Utility.EnumUtility.ParseLiteral(value, t));
                    }

                    break;
                }
                catch
                {
                    break;
                }
            }

            obj = property.GetValue(obj);
        }

        return base.VisitMetadataDeclaration(ctx);
    }



    //      NULL_LITERAL = 85, BOOL = 86, DATE = 87, 
    //DATE_TIME = 88, TIME = 89, LONG_INTEGER = 90, DECIMAL = 91, INTEGER = 92, ID = 93, 
    //IDENTIFIER = 94, DELIMITED_IDENTIFIER = 95, SINGLE_QUOTED_STRING = 96, DOUBLE_QUOTED_STRING = 97, 
    //TRIPLE_QUOTED_STRING_LITERAL = 98, WS = 99, C_STYLE_COMMENT = 100, METADATA_PREFIX = 101, 
    //LINE_COMMENT = 102, INLINE_COMMENT = 103

    private static string? GetString(ParserRuleContext c) => c.Stop.Type switch
    {
        NULL_LITERAL => null,
        BOOL => c.GetText() ,
        DATE => c.Stop.Text.StartsWith('@') ? c.Stop.Text[1..] : c.Stop.Text,
        DATE_TIME => c.Stop.Text.StartsWith('@') ? c.Stop.Text : c.Stop.Text,
        TIME => c.Stop.Text.StartsWith('@') ? c.Stop.Text : c.Stop.Text,
        LONG_INTEGER => c.GetText(),
        DECIMAL => c.GetText(),
        INTEGER => c.GetText(),
        ID => c.GetText(),
        IDENTIFIER => c.Stop.Text.Length > 1 && c.Stop.Text[0] == '\'' && c.Stop.Text[^1] == '\'' ? c.Stop.Text[1..^1] : c.Stop.Text,
        DELIMITED_IDENTIFIER => c.Stop.Text[1..^1],
        SINGLE_QUOTED_STRING => c.Stop.Text[1..^1],
        DOUBLE_QUOTED_STRING => c.Stop.Text[1..^1],
        C_STYLE_COMMENT => c.Stop.Text.Length > 4 ? c.Stop.Text[2..^2] : c.Stop.Text,
        LINE_COMMENT => c.Stop.Text.Length > 2 ? c.Stop.Text[2..] : c.Stop.Text,
        _ => null,
    };

    private static dynamic? GetValue(ParserRuleContext c) => c.Stop.Type switch
    {
        NULL_LITERAL => null,
        BOOL => c.Stop.Text == "true" ? true : false,
        DATE => c.Stop.Text.StartsWith('@') ? (TryParseDateString(c.Stop.Text[1..], out DateTimeOffset s, out _) ? s : null) : (TryParseDateString(c.Stop.Text, out s, out _) ? s : null),
        DATE_TIME => c.Stop.Text.StartsWith('@') ? (TryParseDateString(c.Stop.Text[1..], out DateTimeOffset s, out _) ? s : null) : (TryParseDateString(c.Stop.Text, out s, out _) ? s : null),
        TIME => c.Stop.Text.StartsWith('@') ? (TryParseDateString(c.Stop.Text[1..], out DateTimeOffset s, out _) ? s : null) : (TryParseDateString(c.Stop.Text, out s, out _) ? s : null),
        LONG_INTEGER => long.TryParse(c.Stop.Text, out long value) ? value : null,
        DECIMAL => decimal.TryParse(c.Stop.Text, out decimal value) ? value : null,
        INTEGER => int.TryParse(c.Stop.Text, out int value) ? value : null,
        ID => c.Stop.Text,
        IDENTIFIER => c.Stop.Text.Length > 1 && c.Stop.Text[0] == '\'' && c.Stop.Text[^1] == '\'' ? c.Stop.Text[1..^1] : c.Stop.Text,
        DELIMITED_IDENTIFIER => c.Stop.Text[1..^1],
        SINGLE_QUOTED_STRING => c.Stop.Text[1..^1],
        DOUBLE_QUOTED_STRING => c.Stop.Text[1..^1],
        C_STYLE_COMMENT => c.Stop.Text.Length > 4 ? c.Stop.Text[2..^2] : c.Stop.Text,
        LINE_COMMENT => c.Stop.Text.Length > 2 ? c.Stop.Text[2..] : c.Stop.Text,
        _ => null,
    };

    private static DataType? GetFhirValue(ParserRuleContext c) => c.Stop.Type switch
    {
        NULL_LITERAL => null,
        BOOL => c.Stop.Text == "true" ? new FhirBoolean(true) : new FhirBoolean(false),
        DATE => c.Stop.Text.StartsWith('@') ? new FhirDateTime(c.Stop.Text[1..]) : new FhirDateTime(c.Stop.Text),
        DATE_TIME => c.Stop.Text.StartsWith('@') ? new FhirDateTime(c.Stop.Text) : new FhirDateTime(c.Stop.Text),
        TIME => c.Stop.Text.StartsWith('@') ? new Time(c.Stop.Text) : new Time(c.Stop.Text),
        LONG_INTEGER => long.TryParse(c.Stop.Text, out long value) ? new Integer64(value) : null,
        DECIMAL => decimal.TryParse(c.Stop.Text, out decimal value) ? new FhirDecimal(value) : null,
        INTEGER => int.TryParse(c.Stop.Text, out int value) ? new Integer64(value) : null,
        ID => new Id(c.Stop.Text),
        IDENTIFIER => c.Stop.Text.Length > 1 && c.Stop.Text[0] == '\'' && c.Stop.Text[^1] == '\'' ? new FhirString(c.Stop.Text[1..^1]) : new FhirString(c.Stop.Text),
        DELIMITED_IDENTIFIER => new FhirString(c.Stop.Text[1..^1]),
        SINGLE_QUOTED_STRING => new FhirString(c.Stop.Text[1..^1]),
        DOUBLE_QUOTED_STRING => new FhirString(c.Stop.Text[1..^1]),
        C_STYLE_COMMENT => c.Stop.Text.Length > 4 ? new FhirString(c.Stop.Text[2..^2]) : new FhirString(c.Stop.Text),
        LINE_COMMENT => c.Stop.Text.Length > 2 ? new FhirString(c.Stop.Text[2..]) : new FhirString(c.Stop.Text),
        _ => null,
    };

    public static bool TryParseDateString(string dateString, out DateTimeOffset start, out DateTimeOffset end)
    {
        if (string.IsNullOrEmpty(dateString))
        {
            start = DateTimeOffset.MinValue;
            end = DateTimeOffset.MaxValue;
            return false;
        }

        // need to check for just year because DateTime refuses to parse that
        if (dateString.Length == 4)
        {
            start = new DateTimeOffset(int.Parse(dateString), 1, 1, 0, 0, 0, TimeSpan.Zero);
            end = start.AddYears(1).AddTicks(-1);
            return true;
        }

        // note that we are using DateTime and converting to DateTimeOffset to work through TZ stuff without manually parsing each format precision
        if (!DateTime.TryParse(dateString, null, DateTimeStyles.RoundtripKind, out DateTime dt))
        {
            Console.WriteLine($"Failed to parse date: {dateString}");
            start = DateTimeOffset.MinValue;
            end = DateTimeOffset.MaxValue;
            return false;
        }

        start = new DateTimeOffset(dt, TimeSpan.Zero);

        switch (dateString.Length)
        {
            // YYYY
            case 4:
                end = start.AddYears(1).AddTicks(-1);
                break;

            // YYYY-MM
            case 7:
                end = start.AddMonths(1).AddTicks(-1);
                break;

            // YYYY-MM-DD
            case 10:
                end = start.AddDays(1).AddTicks(-1);
                break;

            // Note: this is not defined as valid, but wanted to support it
            // YYYY-MM-DDThh
            case 13:
                end = start.AddHours(1).AddTicks(-1);
                break;

            // YYYY-MM-DDThh:mm
            case 16:
                end = start.AddMinutes(1).AddTicks(-1);
                break;

            // Note: servers are allowed to ignore fractional seconds - I am chosing to do so.

            // YYYY-MM-DDThh:mm:ss
            case 19:
            // YYYY-MM-DDThh:mm:ssZ
            case 20:
            // YYYY-MM-DDThh:mm:ss+zz
            // YYYY-MM-DDThh:mm:ss.fZ
            case 22:
            // YYYY-MM-DDThh:mm:ss.ffZ
            case 23:
            // YYYY-MM-DDThh:mm:ss.fffZ
            case 24:
            // YYYY-MM-DDThh:mm:ss+zz:zz
            // YYYY-MM-DDThh:mm:ss.ffffZ
            case 25:
            // YYYY-MM-DDThh:mm:ss.f+zz:zz
            case 27:
            // YYYY-MM-DDThh:mm:ss.ff+zz:zz
            case 28:
            // YYYY-MM-DDThh:mm:ss.fff+zz:zz
            case 29:
            // YYYY-MM-DDThh:mm:ss.ffff+zz:zz
            case 30:
                end = start.AddSeconds(1).AddTicks(-1);
                break;

            default:
                Console.WriteLine($"Invalid date format: {dateString}");
                start = DateTimeOffset.MinValue;
                end = DateTimeOffset.MaxValue;
                return false;
        }

        return true;
    }

    public override object VisitMapDeclaration([NotNull] MapDeclarationContext ctx)
    {
        //Console.WriteLine($"    Map URL: {ctx.url()?.GetText()}");
        //Console.WriteLine($"Map Id/Name: {ctx.identifier()?.GetText()}");

        // markdown type - join with newlines
        string comment = string.Join('\n', ctx.LINE_COMMENT()?.Select(c => c.GetText()[2..]) ?? Enumerable.Empty<string>());
        _map!.Description = comment;

        _map!.Url = ctx.url().GetText();
        _map!.Id = ctx.identifier().GetText();

        return base.VisitMapDeclaration(ctx);
    }

    //private static string StringFromNode()
    //{

    //}



    //public override object VisitStructureDeclaration([NotNull] StructureDeclarationContext ctx)
    //{
    //    StructureMap.StructureComponent sc = new();

    //    // string type - join with spaces
    //    string comment = string.Join(' ', ctx.LINE_COMMENT()?.Select(c => c.GetText()[2..]) ?? Enumerable.Empty<string>());
    //    sc.Documentation = comment;

    //    if ((ctx.url() is UrlContext urlCtx) &&
    //        TryGetStringValue(urlCtx, out string? url))
    //    {
    //        sc.Url = url;
    //    }

    //    if (ctx.structureAlias() is StructureAliasContext aliasCtx)
    //    {
    //        sc.Alias = ExtractString(aliasCtx);
    //    }

    //    if ((ctx.modelMode() is ModelModeContext modelModeCtx) &&
    //        (ExtractString(modelModeCtx) is string modelMode))
    //    {
    //        sc.Mode = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<StructureMap.StructureMapModelMode>(modelMode);
    //    }

    //    _map!.Structure.Add(sc);

    //    return base.VisitStructureDeclaration(ctx);
    //}

    //public override object VisitImports([NotNull] ImportsContext ctx)
    //{
    //    if (ExtractString(ctx) is string import)
    //    {
    //        // markdown type - join with newlines
    //        string comment = string.Join('\n', ctx.LINE_COMMENT()?.Select(c => c.GetText()[2..]) ?? Enumerable.Empty<string>());

    //        Canonical ci = new Canonical(import);
    //        if (!string.IsNullOrEmpty(comment))
    //        {
    //            ci.Extension.Add(new Extension(CommonDefinitions.ExtUrlComment, new Markdown(comment)));
    //        }

    //        _map!.ImportElement.Add(ci);
    //    }

    //    return base.VisitImports(ctx);
    //}

    //public override object VisitConst([NotNull] ConstContext ctx)
    //{
    //    string id = ctx.ID().GetText();
    //    string fp = ctx.fhirPath().GetText();

    //    if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(fp))
    //    {
    //        StructureMap.ConstComponent cc = new();

    //        cc.Name = id;
    //        cc.Value = fp;

    //        // markdown type - join with newlines
    //        string comment = string.Join('\n', ctx.LINE_COMMENT()?.Select(c => c.GetText()[2..]) ?? Enumerable.Empty<string>());
    //        if (!string.IsNullOrEmpty(comment))
    //        {
    //            cc.Extension.Add(new Extension(CommonDefinitions.ExtUrlComment, new Markdown(comment)));
    //        }

    //        _map!.Const.Add(cc);
    //    }

    //    return base.VisitConst(ctx);
    //}

    //StructureMap.GroupComponent? _currentGroup = null;
    //List<StructureMap.RuleComponent> _currentRules = [];

    //public override object VisitGroup([NotNull] GroupContext ctx)
    //{
    //    // create a new group - note that the subsequent Rule visitor will add to this group
    //    _currentGroup = new();
    //    _currentRules = [];

    //    // string type - join with spaces
    //    string comment = string.Join(' ', ctx.LINE_COMMENT()?.Select(c => c.GetText()[2..]) ?? Enumerable.Empty<string>());
    //    _currentGroup.Documentation = comment;

    //    _currentGroup.Name = ctx.ID()?.GetText();

    //    foreach (ParameterContext parameterCtx in ctx.parameters()?.parameter() ?? Array.Empty<ParameterContext>())
    //    {
    //        StructureMap.InputComponent inputParam = new();

    //        inputParam.Mode = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<StructureMap.StructureMapInputMode>(parameterCtx.inputMode()?.GetText());
    //        inputParam.Name = parameterCtx.ID()?.GetText();
    //        inputParam.Type = parameterCtx.type() is TypeContext typeCtx ? ExtractString(typeCtx) : null;

    //        _currentGroup.Input.Add(inputParam);
    //    }

    //    _currentGroup.Extends = ctx.extends() is ExtendsContext extendsCtx ? extendsCtx.ID()?.GetText() : null;

    //    if (ctx.typeMode() is TypeModeContext typeModeCtx)
    //    {
    //        string tm = typeModeCtx.GetText();
    //        switch (tm)
    //        {
    //            case "type+":
    //            case "<<type+>>":
    //            case "type-and-types":
    //                _currentGroup.TypeMode = StructureMap.StructureMapGroupTypeMode.TypeAndTypes;
    //                break;

    //            case "type":
    //            case "<<type>>":
    //                _currentGroup.TypeMode = StructureMap.StructureMapGroupTypeMode.Types;
    //                break;
    //        }
    //    }

    //    _map!.Group.Add(_currentGroup);

    //    return base.VisitGroup(ctx);
    //}

    //public override object VisitRule([NotNull] FmlMappingParser.RuleContext ctx)
    //{
    //    if (_currentGroup is null)
    //    {
    //        throw new Exception("Rule found outside of a group");
    //    }

    //    StructureMap.RuleComponent rule = new();

    //    // string type - join with spaces
    //    string comment = string.Join(' ', ctx.LINE_COMMENT()?.Select(c => c.GetText()[2..]) ?? Enumerable.Empty<string>());
    //    rule.Documentation = comment;

    //    rule.Name = ctx.ruleName()?.ID()?.GetText() ?? string.Empty;

    //    foreach (RuleSourceContext ruleSourceCtx in ctx.ruleSources()?.children ?? Array.Empty<RuleSourceContext>())
    //    {
    //        StructureMap.SourceComponent rs = new();

    //        // sort out the actual source
    //        string[] sourceComponents = ruleSourceCtx.ruleContext()?.identifier()?.Select(i => i.GetText()).ToArray() ?? [];

    //        rs.Context = sourceComponents[0];
    //        rs.Element = sourceComponents.Length > 1 ? string.Join('.', sourceComponents[1..]) : sourceComponents.FirstOrDefault() ?? string.Empty;

    //        // if there is no rule name, we want to use the element name
    //        if (string.IsNullOrEmpty(rule.Name))
    //        {
    //            rule.Name = rs.Element;
    //        }

    //        if (ruleSourceCtx.sourceType()?.identifier()?.GetText() is string sourceType)
    //        {
    //            rs.Type = sourceType;
    //        }

    //        if (ruleSourceCtx.sourceCardinality() is SourceCardinalityContext sourceCardinalityCtx)
    //        {
    //            if (sourceCardinalityCtx.INTEGER()?.Payload is int min)
    //            {
    //                rs.Min = min;
    //            }

    //            if (sourceCardinalityCtx.upperBound()?.GetText() is string max)
    //            {
    //                rs.Max = max;
    //            }
    //        }

    //        if (ruleSourceCtx.sourceDefault()?.fhirPath()?.GetText() is string defaultValue)
    //        {
    //            rs.DefaultValue = defaultValue;
    //        }

    //        if (ruleSourceCtx.sourceListMode()?.GetText() is string listMode)
    //        {
    //            rs.ListMode = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<StructureMap.StructureMapSourceListMode>(listMode);
    //        }

    //        if (ruleSourceCtx.alias()?.identifier()?.GetText() is string alias)
    //        {
    //            rs.Variable = alias;
    //        }
    //        else
    //        {
    //            rs.Variable = _unspecifiedVarName;
    //        }

    //        if (ruleSourceCtx.whereClause()?.fhirPath()?.GetText() is string whereClause)
    //        {
    //            rs.Condition = whereClause;
    //        }

    //        if (ruleSourceCtx.checkClause()?.fhirPath()?.GetText() is string checkClause)
    //        {
    //            rs.Check = checkClause;
    //        }

    //        if (ruleSourceCtx.log()?.fhirPath()?.GetText() is string log)
    //        {
    //            rs.LogMessage = log;
    //        }

    //        rule.Source.Add(rs);
    //    }

    //    foreach (IParseTree ctxPt in ctx.ruleTargets()?.children ?? Array.Empty<RuleTargetContext>())
    //    {
    //        if (ctxPt is not RuleTargetContext ruleTargetCtx)
    //        {
    //            continue;
    //        }

    //        StructureMap.TargetComponent rt = new();

    //        string[] targetComponents = ruleTargetCtx.ruleContext()?.identifier()?.Select(i => i.GetText()).ToArray() ?? [];

    //        rt.Context = targetComponents[0];
    //        rt.Element = targetComponents.Length > 1 ? string.Join('.', targetComponents[1..]) : targetComponents.FirstOrDefault() ?? string.Empty;

    //        if (ruleTargetCtx.transform() is TransformContext transformCtx)
    //        {
    //            if (transformCtx.ruleContext() is RuleContextContext ruleCtx)
    //            {
    //                Console.WriteLine($"transform context rule context: {ruleCtx.GetText()}");
    //            }

    //            //transformCtx.ruleContext()
    //            //null
    //            //transformCtx.invocation().identifier().GetText()
    //            //"translate"

    //            if (transformCtx.invocation() is InvocationContext invocationCtx)
    //            {
    //                rt.Transform = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<StructureMap.StructureMapTransform>(invocationCtx.identifier().GetText());

    //                foreach (ParamContext paramCtx in invocationCtx.paramList()?.param() ?? Enumerable.Empty<ParamContext>())
    //                {
    //                    StructureMap.ParameterComponent param = new();

    //                    // TODO(ginoc): we should use the type from the lexer instead of parse-based guessing here

    //                    param.Value = ParseValueForParam(paramCtx.GetText());

    //                    rt.Parameter.Add(param);
    //                }
    //            }
    //        }
    //        else
    //        {
    //            rt.Transform = StructureMap.StructureMapTransform.Create;
    //        }

    //        if (ruleTargetCtx.alias()?.identifier()?.GetText() is string alias)
    //        {
    //            rt.Variable = alias;
    //        }
    //        else
    //        {
    //            rt.Variable = _unspecifiedVarName;
    //        }

    //        rule.Target.Add(rt);
    //    }

    //    // check for dependent rules
    //    foreach (InvocationContext dependentCtx in ctx.dependent()?.invocation() ?? Enumerable.Empty<InvocationContext>())
    //    {
    //        StructureMap.DependentComponent dc = new();

    //        dc.Name = dependentCtx.identifier()?.GetText();

    //        foreach (ParamContext paramCtx in dependentCtx.paramList()?.param() ?? Enumerable.Empty<ParamContext>())
    //        {
    //            StructureMap.ParameterComponent param = new();

    //            param.Value = new FhirString(paramCtx.GetText());

    //            dc.Parameter.Add(param);
    //        }

    //        rule.Dependent.Add(dc);
    //    }

    //    // if we have nested into a rule, we need to add it there
    //    if (_currentRules.Count != 0)
    //    {
    //        _currentRules[^1].Rule.Add(rule);
    //    }
    //    else
    //    {
    //        _currentGroup.Rule.Add(rule);
    //    }

    //    return base.VisitRule(ctx);
    //}

    //private static DataType? ParseValueForParam(string value)
    //{
    //    if (string.IsNullOrEmpty(value))
    //    {
    //        return null;
    //    }

    //    switch (value[0])
    //    {
    //        case '\'':
    //        case '"':
    //            return new FhirString(value[1..^1]);
    //        case '@':
    //        case '%':
    //        //case '-':
    //        //case '+':
    //            return new FhirString(value);

    //        case 't':
    //            return value.Equals("true", StringComparison.Ordinal) ? new FhirBoolean(true) : new Id(value);

    //        case 'f':
    //            return value.Equals("false", StringComparison.Ordinal) ? new FhirBoolean(true) : new Id(value);

    //        case '{':
    //            return (value.Length == 2 && value[1] == '}') ? new FhirString(value) : new Id(value);
    //    }

    //    if (int.TryParse(value, out var intVal))
    //    {
    //        return new Integer(intVal);
    //    }

    //    if (decimal.TryParse(value, out decimal decimalVal))
    //    {
    //        return new FhirDecimal(decimalVal);
    //    }

    //    return new Id(value);
    //}


    //private static bool TryGetStringValue(StringValueContext ctx, [CA.NotNullWhen(true)] out string? value)
    //{
    //    // get to the terminal node we are interested in
    //    if ((ctx.ChildCount == 0) ||
    //        (ctx.children[0] is not ITerminalNode node))
    //    {
    //        value = null;
    //        return false;
    //    }

    //    switch (node.Symbol.Type)
    //    {
    //        case STRING:
    //            value = node.Symbol.Text;
    //            return true;

    //        case DOUBLE_QUOTED_STRING:
    //            value = node.Symbol.Text[1..^1];
    //            return true;

    //        default:
    //            value = null;
    //            return false;
    //    }
    //}

    //private static bool TryGetStringValue(IdentifierContext ctx, [CA.NotNullWhen(true)] out string? value)
    //{
    //    // get to the terminal node we are interested in
    //    if ((ctx.ChildCount == 0) ||
    //        (ctx.children[0] is not ITerminalNode node))
    //    {
    //        value = null;
    //        return false;
    //    }

    //    switch (node.Symbol.Type)
    //    {
    //        case ID:
    //        case IDENTIFIER:
    //            value = node.Symbol.Text;
    //            return true;

    //        case DELIMITED_IDENTIFIER:
    //            value = node.Symbol.Text[1..^1];
    //            return true;

    //        default:
    //            value = null;
    //            return false;
    //    }
    //}

    //private static bool TryGetStringValue(UrlContext ctx, [CA.NotNullWhen(true)] out string? value)
    //{
    //    // get to the terminal node we are interested in
    //    if ((ctx.ChildCount == 0) ||
    //        (ctx.children[0] is not ITerminalNode node))
    //    {
    //        value = null;
    //        return false;
    //    }

    //    switch (node.Symbol.Type)
    //    {
    //        case STRING:
    //            value = node.Symbol.Text;
    //            return true;

    //        case DOUBLE_QUOTED_STRING:
    //            value = node.Symbol.Text[1..^1];
    //            return true;

    //        default:
    //            value = null;
    //            return false;
    //    }
    //}

    //private static string ExtractString(ParserRuleContext ctx)
    //{
    //    if (ctx.ChildCount == 0)
    //    {
    //        return ctx.GetText();
    //    }

    //    if ((ctx.GetRuleContext<StringValueContext>(0) is StringValueContext stringValueCtx) &&
    //        TryGetStringValue(stringValueCtx, out string? value))
    //    {
    //        return value;
    //    }

    //    if ((ctx.GetRuleContext<IdentifierContext>(0) is IdentifierContext identifierCtx) &&
    //        TryGetStringValue(identifierCtx, out value))
    //    {
    //        return value;
    //    }

    //    if ((ctx.GetRuleContext<UrlContext>(0) is UrlContext urlCtx) &&
    //        TryGetStringValue(urlCtx, out value))
    //    {
    //        return value;
    //    }

    //    return ctx.GetText();
    //}
}
