// <copyright file="ExpressionDefinitionsFunction.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.MappingLanguage.Expression;

/// <summary>A function code extensions.</summary>
public static class FunctionCodeExtensions
{
    /// <summary>A string extension method that converts a literal to a function code.</summary>
    /// <param name="literal">The literal to act on.</param>
    /// <returns>Literal as an ExpressionNode.FunctionCodes?</returns>
    public static ExpressionNode.FunctionCodes? ToFunctionCode(this string literal) => ExpressionNode.ToFunctionCode(literal);

    /// <summary>
    /// The ExpressionNode.FunctionCodes extension method that converts a functionCode to a
    /// literal.
    /// </summary>
    /// <param name="functionCode">The function code.</param>
    /// <returns>FunctionCode as a string.</returns>
    public static string ToString(this ExpressionNode.FunctionCodes functionCode) => ExpressionNode.ToLiteral(functionCode);
}

/// <summary>Expression function definitions.</summary>
public partial class ExpressionNode
{
    /// <summary>Values that represent function codes.</summary>
    public enum FunctionCodes
    {
        Custom,

        // FHIRPath functions
        Empty, Not, Exists, SubsetOf, SupersetOf, IsDistinct, Distinct, Count, Where, Select, All, Repeat, Aggregate, Item /*implicit from name[]*/, As, Is, Single,
        First, Last, Tail, Skip, Take, Union, Combine, Intersect, Exclude, Iif, Upper, Lower, ToChars, IndexOf, Substring, StartsWith, EndsWith, Matches, MatchesFull, ReplaceMatches, Contains, Replace, Length,
        Children, Descendants, MemberOf, Trace, Check, Today, Now, Resolve, Extension, AllFalse, AnyFalse, AllTrue, AnyTrue,
        HasValue, OfType, Type, ConvertsToBoolean, ConvertsToInteger, ConvertsToString, ConvertsToDecimal, ConvertsToQuantity, ConvertsToDateTime, ConvertsToDate, ConvertsToTime, ToBoolean, ToInteger, ToString, ToDecimal, ToQuantity, ToDateTime, ToTime, ConformsTo,
        Round, Sqrt, Abs, Ceiling, Exp, Floor, Ln, Log, Power, Truncate,

        // R3 functions
        Encode, Decode, Escape, Unescape, Trim, Split, Join, LowBoundary, HighBoundary, Precision,

        // HAPI Local extensions to FHIRPath
        HtmlChecks1, HtmlChecks2, AliasAs, Alias, Comparable, HasTemplateIdOf,
    }

    /// <summary>
    /// Converts the given literal to its corresponding <see cref="FunctionCodes"/>.
    /// </summary>
    /// <param name="literal">The literal to convert.</param>
    /// <returns>The corresponding <see cref="FunctionCodes"/> if the conversion is successful; otherwise, null.</returns>
    public static FunctionCodes? ToFunctionCode(string literal) => literal switch
    {
        "empty" => FunctionCodes.Empty,
        "not" => FunctionCodes.Not,
        "exists" => FunctionCodes.Exists,
        "subsetOf" => FunctionCodes.SubsetOf,
        "supersetOf" => FunctionCodes.SupersetOf,
        "isDistinct" => FunctionCodes.IsDistinct,
        "distinct" => FunctionCodes.Distinct,
        "count" => FunctionCodes.Count,
        "where" => FunctionCodes.Where,
        "select" => FunctionCodes.Select,
        "all" => FunctionCodes.All,
        "repeat" => FunctionCodes.Repeat,
        "aggregate" => FunctionCodes.Aggregate,
        "item" => FunctionCodes.Item,
        "as" => FunctionCodes.As,
        "is" => FunctionCodes.Is,
        "single" => FunctionCodes.Single,
        "first" => FunctionCodes.First,
        "last" => FunctionCodes.Last,
        "tail" => FunctionCodes.Tail,
        "skip" => FunctionCodes.Skip,
        "take" => FunctionCodes.Take,
        "union" => FunctionCodes.Union,
        "combine" => FunctionCodes.Combine,
        "intersect" => FunctionCodes.Intersect,
        "exclude" => FunctionCodes.Exclude,
        "iif" => FunctionCodes.Iif,
        "upper" => FunctionCodes.Upper,
        "lower" => FunctionCodes.Lower,
        "toChars" => FunctionCodes.ToChars,
        "indexOf" => FunctionCodes.IndexOf,
        "substring" => FunctionCodes.Substring,
        "startsWith" => FunctionCodes.StartsWith,
        "endsWith" => FunctionCodes.EndsWith,
        "matches" => FunctionCodes.Matches,
        "matchesFull" => FunctionCodes.MatchesFull,
        "replaceMatches" => FunctionCodes.ReplaceMatches,
        "contains" => FunctionCodes.Contains,
        "replace" => FunctionCodes.Replace,
        "length" => FunctionCodes.Length,
        "children" => FunctionCodes.Children,
        "descendants" => FunctionCodes.Descendants,
        "memberOf" => FunctionCodes.MemberOf,
        "trace" => FunctionCodes.Trace,
        "check" => FunctionCodes.Check,
        "today" => FunctionCodes.Today,
        "now" => FunctionCodes.Now,
        "resolve" => FunctionCodes.Resolve,
        "extension" => FunctionCodes.Extension,
        "allFalse" => FunctionCodes.AllFalse,
        "anyFalse" => FunctionCodes.AnyFalse,
        "allTrue" => FunctionCodes.AllTrue,
        "anyTrue" => FunctionCodes.AnyTrue,
        "hasValue" => FunctionCodes.HasValue,
        "ofType" => FunctionCodes.OfType,
        "type" => FunctionCodes.Type,
        "convertsToBoolean" => FunctionCodes.ConvertsToBoolean,
        "convertsToInteger" => FunctionCodes.ConvertsToInteger,
        "convertsToString" => FunctionCodes.ConvertsToString,
        "convertsToDecimal" => FunctionCodes.ConvertsToDecimal,
        "convertsToQuantity" => FunctionCodes.ConvertsToQuantity,
        "convertsToDateTime" => FunctionCodes.ConvertsToDateTime,
        "convertsToDate" => FunctionCodes.ConvertsToDate,
        "convertsToTime" => FunctionCodes.ConvertsToTime,
        "toBoolean" => FunctionCodes.ToBoolean,
        "toInteger" => FunctionCodes.ToInteger,
        "toString" => FunctionCodes.ToString,
        "toDecimal" => FunctionCodes.ToDecimal,
        "toQuantity" => FunctionCodes.ToQuantity,
        "toDateTime" => FunctionCodes.ToDateTime,
        "toTime" => FunctionCodes.ToTime,
        "conformsTo" => FunctionCodes.ConformsTo,
        "round" => FunctionCodes.Round,
        "sqrt" => FunctionCodes.Sqrt,
        "abs" => FunctionCodes.Abs,
        "ceiling" => FunctionCodes.Ceiling,
        "exp" => FunctionCodes.Exp,
        "floor" => FunctionCodes.Floor,
        "ln" => FunctionCodes.Ln,
        "log" => FunctionCodes.Log,
        "power" => FunctionCodes.Power,
        "truncate" => FunctionCodes.Truncate,
        "encode" => FunctionCodes.Encode,
        "decode" => FunctionCodes.Decode,
        "escape" => FunctionCodes.Escape,
        "unescape" => FunctionCodes.Unescape,
        "trim" => FunctionCodes.Trim,
        "split" => FunctionCodes.Split,
        "join" => FunctionCodes.Join,
        "lowBoundary" => FunctionCodes.LowBoundary,
        "highBoundary" => FunctionCodes.HighBoundary,
        "precision" => FunctionCodes.Precision,
        "htmlChecks1" => FunctionCodes.HtmlChecks1,
        "htmlChecks2" => FunctionCodes.HtmlChecks2,
        "aliasAs" => FunctionCodes.AliasAs,
        "alias" => FunctionCodes.Alias,
        "comparable" => FunctionCodes.Comparable,
        "hasTemplateIdOf" => FunctionCodes.HasTemplateIdOf,
        _ => null
    };

    /// <summary>
    /// Converts the given <see cref="FunctionCodes"/> to its corresponding literal representation.
    /// </summary>
    /// <param name="code">The <see cref="FunctionCodes"/> to convert.</param>
    /// <returns>The literal representation of the <see cref="FunctionCodes"/>.</returns>
    public static string ToLiteral(FunctionCodes code) => code switch
    {
        FunctionCodes.Empty => "empty",
        FunctionCodes.Not => "not",
        FunctionCodes.Exists => "exists",
        FunctionCodes.SubsetOf => "subsetOf",
        FunctionCodes.SupersetOf => "supersetOf",
        FunctionCodes.IsDistinct => "isDistinct",
        FunctionCodes.Distinct => "distinct",
        FunctionCodes.Count => "count",
        FunctionCodes.Where => "where",
        FunctionCodes.Select => "select",
        FunctionCodes.All => "all",
        FunctionCodes.Repeat => "repeat",
        FunctionCodes.Aggregate => "aggregate",
        FunctionCodes.Item => "item",
        FunctionCodes.As => "as",
        FunctionCodes.Is => "is",
        FunctionCodes.Single => "single",
        FunctionCodes.First => "first",
        FunctionCodes.Last => "last",
        FunctionCodes.Tail => "tail",
        FunctionCodes.Skip => "skip",
        FunctionCodes.Take => "take",
        FunctionCodes.Union => "union",
        FunctionCodes.Combine => "combine",
        FunctionCodes.Intersect => "intersect",
        FunctionCodes.Exclude => "exclude",
        FunctionCodes.Iif => "iif",
        FunctionCodes.Upper => "upper",
        FunctionCodes.Lower => "lower",
        FunctionCodes.ToChars => "toChars",
        FunctionCodes.IndexOf => "indexOf",
        FunctionCodes.Substring => "substring",
        FunctionCodes.StartsWith => "startsWith",
        FunctionCodes.EndsWith => "endsWith",
        FunctionCodes.Matches => "matches",
        FunctionCodes.MatchesFull => "matchesFull",
        FunctionCodes.ReplaceMatches => "replaceMatches",
        FunctionCodes.Contains => "contains",
        FunctionCodes.Replace => "replace",
        FunctionCodes.Length => "length",
        FunctionCodes.Children => "children",
        FunctionCodes.Descendants => "descendants",
        FunctionCodes.MemberOf => "memberOf",
        FunctionCodes.Trace => "trace",
        FunctionCodes.Check => "check",
        FunctionCodes.Today => "today",
        FunctionCodes.Now => "now",
        FunctionCodes.Resolve => "resolve",
        FunctionCodes.Extension => "extension",
        FunctionCodes.AllFalse => "allFalse",
        FunctionCodes.AnyFalse => "anyFalse",
        FunctionCodes.AllTrue => "allTrue",
        FunctionCodes.AnyTrue => "anyTrue",
        FunctionCodes.HasValue => "hasValue",
        FunctionCodes.OfType => "ofType",
        FunctionCodes.Type => "type",
        FunctionCodes.ConvertsToBoolean => "convertsToBoolean",
        FunctionCodes.ConvertsToInteger => "convertsToInteger",
        FunctionCodes.ConvertsToString => "convertsToString",
        FunctionCodes.ConvertsToDecimal => "convertsToDecimal",
        FunctionCodes.ConvertsToQuantity => "convertsToQuantity",
        FunctionCodes.ConvertsToDateTime => "convertsToDateTime",
        FunctionCodes.ConvertsToDate => "convertsToDate",
        FunctionCodes.ConvertsToTime => "convertsToTime",
        FunctionCodes.ToBoolean => "toBoolean",
        FunctionCodes.ToInteger => "toInteger",
        FunctionCodes.ToString => "toString",
        FunctionCodes.ToDecimal => "toDecimal",
        FunctionCodes.ToQuantity => "toQuantity",
        FunctionCodes.ToDateTime => "toDateTime",
        FunctionCodes.ToTime => "toTime",
        FunctionCodes.ConformsTo => "conformsTo",
        FunctionCodes.Round => "round",
        FunctionCodes.Sqrt => "sqrt",
        FunctionCodes.Abs => "abs",
        FunctionCodes.Ceiling => "ceiling",
        FunctionCodes.Exp => "exp",
        FunctionCodes.Floor => "floor",
        FunctionCodes.Ln => "ln",
        FunctionCodes.Log => "log",
        FunctionCodes.Power => "power",
        FunctionCodes.Truncate => "truncate",
        FunctionCodes.Encode => "encode",
        FunctionCodes.Decode => "decode",
        FunctionCodes.Escape => "escape",
        FunctionCodes.Unescape => "unescape",
        FunctionCodes.Trim => "trim",
        FunctionCodes.Split => "split",
        FunctionCodes.Join => "join",
        FunctionCodes.LowBoundary => "lowBoundary",
        FunctionCodes.HighBoundary => "highBoundary",
        FunctionCodes.Precision => "precision",
        FunctionCodes.HtmlChecks1 => "htmlChecks1",
        FunctionCodes.HtmlChecks2 => "htmlChecks2",
        FunctionCodes.AliasAs => "aliasAs",
        FunctionCodes.Alias => "alias",
        FunctionCodes.Comparable => "comparable",
        FunctionCodes.HasTemplateIdOf => "hasTemplateIdOf",
        _ => "??",
    };
}
