// <copyright file="FhirMap.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
using System;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Health.Fhir.MappingLanguage;

/// <summary>Values that represent FML rules - these are entities in the grammar.</summary>
/// <remarks>
/// note that this enum needs to be updated any time the ANTLR grammar is updated
/// these are just the rules from the parser class with the 'RULE_' prefix removed and the first letter capitalized
/// </remarks>
public enum FmlRuleCodes : int
{
    StructureMap = 0,
    MapDeclaration = 1,
    MetadataDeclaration = 2,
    MarkdownLiteral = 3,
    Url = 4,
    Identifier = 5,
    StructureDeclaration = 6,
    StructureAlias = 7,
    ImportDeclaration = 8,
    ConstantDeclaration = 9,
    GroupDeclaration = 10,
    GroupExpressions = 11,
    TypeMode = 12,
    Extends = 13,
    Parameters = 14,
    Parameter = 15,
    TypeIdentifier = 16,
    Expression = 17,
    MapExpression = 18,
    MapExpressionName = 19,
    MapExpressionSource = 20,
    MapExpressionTarget = 21,
    SourceCardinality = 22,
    UpperBound = 23,
    QualifiedIdentifier = 24,
    SourceDefault = 25,
    Alias = 26,
    WhereClause = 27,
    CheckClause = 28,
    Log = 29,
    DependentExpression = 30,
    MapLineTarget = 31,
    Transform = 32,
    Invocation = 33,
    ParamList = 34,
    Param = 35,
    FpExpression = 36,
    FpPolarityLiteral = 37,
    FpMultiplicativeLiteral = 38,
    FpAdditiveLiteral = 39,
    FpTypeAssertionLiteral = 40,
    FpUnionLiteral = 41,
    FpInequalityLiteral = 42,
    FpEqualityLiteral = 43,
    FpMembershipLiteral = 44,
    FpAndLiteral = 45,
    FpOrLiteral = 46,
    FpImpliesLiteral = 47,
    FpTerm = 48,
    FpInvocation = 49,
    FpExternalConstant = 50,
    FpFunction = 51,
    FpParamList = 52,
    FpTypeSpecifier = 53,
    Constant = 54,
    Literal = 55,
    FpQuantity = 56,
    FpUnit = 57,
    FpDateTimePrecision = 58,
    FpPluralDateTimePrecision = 59,
    GroupTypeMode = 60,
    SourceListMode = 61,
    TargetListMode = 62,
    InputMode = 63,
    ModelMode = 64,
}

/// <summary>Values that represent fml token type codes - these are token types from the lexer.</summary>
/// <remarks>
/// note that this enum needs to be updated any time the ANTLR grammar is updated
/// 
/// To sort out types for literals, use AntlrUtils.BuildLiteralEnums() to generate the list of literals and their corresponding token types
/// 
/// </remarks>
public enum FmlTokenTypeCodes : int
{
    Map = FmlMappingParser.T__0,
    Equals = FmlMappingParser.T__1,
    Uses = FmlMappingParser.T__2,
    As = FmlMappingParser.T__3,
    Alias = FmlMappingParser.T__4,
    Imports = FmlMappingParser.T__5,
    Let = FmlMappingParser.T__6,
    Semicolon = FmlMappingParser.T__7,
    Group = FmlMappingParser.T__8,
    OpenCurlyBracket = FmlMappingParser.T__9,
    CloseCurlyBracket = FmlMappingParser.T__10,
    DoubleLessThan = FmlMappingParser.T__11,
    DoubleGreaterThan = FmlMappingParser.T__12,
    Extends = FmlMappingParser.T__13,
    OpenParenthesis = FmlMappingParser.T__14,
    Comma = FmlMappingParser.T__15,
    CloseParenthesis = FmlMappingParser.T__16,
    Colon = FmlMappingParser.T__17,
    GreaterThan = FmlMappingParser.T__18,
    DoubleDot = FmlMappingParser.T__19,
    Asterisk = FmlMappingParser.T__20,
    Dot = FmlMappingParser.T__21,
    EmptySingleQuotedString = FmlMappingParser.T__22,
    Default = FmlMappingParser.T__23,
    Where = FmlMappingParser.T__24,
    Check = FmlMappingParser.T__25,
    Log = FmlMappingParser.T__26,
    Then = FmlMappingParser.T__27,
    OpenSquareBracket = FmlMappingParser.T__28,
    CloseSquareBracket = FmlMappingParser.T__29,
    Plus = FmlMappingParser.T__30,
    Minus = FmlMappingParser.T__31,
    Slash = FmlMappingParser.T__32,
    Div = FmlMappingParser.T__33,
    Mod = FmlMappingParser.T__34,
    Ampersand = FmlMappingParser.T__35,
    Is = FmlMappingParser.T__36,
    Pipe = FmlMappingParser.T__37,
    LessOrEqual = FmlMappingParser.T__38,
    LessThan = FmlMappingParser.T__39,
    Arrow = FmlMappingParser.T__40,
    GreaterOrEqual = FmlMappingParser.T__41,
    Tilde = FmlMappingParser.T__42,
    NotEqual = FmlMappingParser.T__43,
    NotEquivalent = FmlMappingParser.T__44,
    In = FmlMappingParser.T__45,
    Contains = FmlMappingParser.T__46,
    And = FmlMappingParser.T__47,
    Or = FmlMappingParser.T__48,
    Xor = FmlMappingParser.T__49,
    Implies = FmlMappingParser.T__50,
    DollarThis = FmlMappingParser.T__51,
    DollarIndex = FmlMappingParser.T__52,
    DollarTotal = FmlMappingParser.T__53,
    Percent = FmlMappingParser.T__54,
    Year = FmlMappingParser.T__55,
    Month = FmlMappingParser.T__56,
    Week = FmlMappingParser.T__57,
    Day = FmlMappingParser.T__58,
    Hour = FmlMappingParser.T__59,
    Minute = FmlMappingParser.T__60,
    Second = FmlMappingParser.T__61,
    Millisecond = FmlMappingParser.T__62,
    Years = FmlMappingParser.T__63,
    Months = FmlMappingParser.T__64,
    Weeks = FmlMappingParser.T__65,
    Days = FmlMappingParser.T__66,
    Hours = FmlMappingParser.T__67,
    Minutes = FmlMappingParser.T__68,
    Seconds = FmlMappingParser.T__69,
    Milliseconds = FmlMappingParser.T__70,
    Types = FmlMappingParser.T__71,
    TypePlus = FmlMappingParser.T__72,
    First = FmlMappingParser.T__73,
    NotFirst = FmlMappingParser.T__74,
    Last = FmlMappingParser.T__75,
    NotLast = FmlMappingParser.T__76,
    OnlyOne = FmlMappingParser.T__77,
    Share = FmlMappingParser.T__78,
    Single = FmlMappingParser.T__79,
    Source = FmlMappingParser.T__80,
    Target = FmlMappingParser.T__81,
    Queried = FmlMappingParser.T__82,
    Produced = FmlMappingParser.T__83,
    NullLiteral = FmlMappingParser.NULL_LITERAL,
    Bool = FmlMappingParser.BOOL,
    Date = FmlMappingParser.DATE,
    DateTime = FmlMappingParser.DATE_TIME,
    Time = FmlMappingParser.TIME,
    LongInteger = FmlMappingParser.LONG_INTEGER,
    Decimal = FmlMappingParser.DECIMAL,
    Integer = FmlMappingParser.INTEGER,
    Id = FmlMappingParser.ID,
    Identifier = FmlMappingParser.IDENTIFIER,
    DelimitedIdentifier = FmlMappingParser.DELIMITED_IDENTIFIER,
    SingleQuotedString = FmlMappingParser.SINGLE_QUOTED_STRING,
    DoubleQuotedString = FmlMappingParser.DOUBLE_QUOTED_STRING,
    TripleQuotedStringLiteral = FmlMappingParser.TRIPLE_QUOTED_STRING_LITERAL,
    Whitespace = FmlMappingParser.WS,
    BlockComment = FmlMappingParser.BLOCK_COMMENT,
    TripleSlash = FmlMappingParser.METADATA_PREFIX,
    LineComment = FmlMappingParser.LINE_COMMENT,
    //InlineComment = FmlMappingParser.INLINE_COMMENT,
}

public enum FmlPolarityCodes : int
{
    // '+' : 84 + 1
    Positive = 84 + 1,

    // '-' : 85 + 1
    Negative = 85 + 1,
}

public enum FmlMultiplicativeOpCodes : int
{
    // '*': 74 + 1
    Multiply = 74 + 1,

    // '/': 86 + 1
    Divide = 86 + 1,

    // 'div': 87 + 1
    Div = 87 + 1,

    // 'mod': 88 + 1
    Modulo = 88 + 1,
}

public enum FmlUnionOpCodes : int
{
    // '|': 91 + 1
    Union = 91 + 1,
}

public enum FmlAdditiveOpCodes : int
{
    // '+': 84 + 1
    Add = 84 + 1,

    // '&': 89 + 1
    And = 89 + 1,

    // '-': 85 + 1
    Subtract = 85 + 1,
}

public enum FmlInequalityOpCodes : int
{
    // '<': 93 + 1
    LessThan = 93 + 1,

    // '<=': 92 + 1
    LessThanOrEqual = 92 + 1,

    // '>': 94 + 1
    GreaterThan = 94 + 1,

    // '>=': 95 + 1
    GreaterThanOrEqual = 95 + 1,
}

public enum FmlEqualityOpCodes : int
{
    // '=': 55 + 1
    Equal = 55 + 1,

    // '~': 96 + 1
    Equivalent = 96 + 1,

    // '!=': 97 + 1
    NotEqual = 97 + 1,

    // '!~': 98 + 1
    NotEquivalent = 98 + 1,
}

public enum FmlMembershipOpCodes : int
{
    // 'in': 99 + 1
    In = 99 + 1,

    // 'contains': 100 + 1
    Contains = 100 + 1,
}

public enum FmlAndOpCodes : int
{
    // 'and': 101 + 1
    And = 101 + 1,
}

public enum FmlOrOpCodes : int
{
    // 'or': 102 + 1
    Or = 102 + 1,

    // 'xor': 103 + 1
    Xor = 103 + 1,
}

public enum FmlImpliesOpCodes : int
{
    // 'implies': 104 + 1
    Implies = 104 + 1,
}

public record class ParsedCommentNode
{
    public required string NodeText { get; init; }
    public required int Line { get; init; }
    public required int Column { get; init; }
    public required int TokenIndex { get; init; }
    public required int StartIndex { get; init; }
    public required int StopIndex { get; init; }

    public required int LastWsStartIndex { get; init; }
    public required int LastWsStopIndex { get; init; }
    public required bool LastWsHasNewline { get; init; }
}

public record class FmlNode
{
    public required string RawText { get; init; }
    public required List<string> PrefixComments { get; init; } = [];
    public required List<string> PostfixComments { get; init; } = [];

    public required int Line { get; init; }
    public required int Column { get; init; }
    public required int StartIndex { get; init; }
    public required int StopIndex { get; init; }
}

public record class MetadataDeclaration : FmlNode
{
    public required string ElementPath { get; init; }
    public required LiteralValue? Literal { get; init; }
    public required string? MarkdownValue { get; init; }
}

public record class MapDeclaration : FmlNode
{
    public required string Url { get; init; }
    public required string Identifier { get; init; }
    public required FmlTokenTypeCodes IdentifierTokenType { get; init; }
}

public record class StructureDeclaration : FmlNode
{
    public required string Url { get; init; }
    public required string? Alias { get; init; }
    public required string ModelModeLiteral { get; init; }
    public required Hl7.Fhir.Model.StructureMap.StructureMapModelMode? ModelMode { get; init; }
}

public record class ImportDeclaration : FmlNode
{
    public required string Url { get; init; }
}

public record class ConstantDeclaration : FmlNode
{
    public required string Name { get; init; }
    public required FpExpression Value { get; init; }
}

public record class GroupParameter : FmlNode
{
    public required string InputModeLiteral { get; init; }
    public required Hl7.Fhir.Model.StructureMap.StructureMapInputMode? InputMode { get; init; }
    public required string Identifier { get; init; }
    public required string? TypeIdentifier { get; init; }
}

public record class MapSimpleCopyExpression : FmlNode
{
    public required string Source { get; init; }
    public required string Target { get; init; }
}

public record class FmlExpressionSource : FmlNode
{
    public required string Identifier { get; init; }
    public required string? TypeIdentifier { get; init; }
    public required string? Cardinality { get; init; }
    //public required int? CardinalityMin { get; init; }
    //public required int? CardinalityMaxInt { get; init; }
    //public required string? CardinalityMax { get; init; }
    public required FpExpression? DefaultExpression { get; init; }
    public required string? ListModeLiteral { get; init; }
    public required Hl7.Fhir.Model.StructureMap.StructureMapSourceListMode? ListMode { get; init; }
    public required string? Alias { get; init; }
    public required FpExpression? WhereClause { get; init; }
    public required FpExpression? CheckClause { get; init; }
    public required FpExpression? LogExpression { get; init; }
}

public record class FmlInvocationParam : FmlNode
{
    public required LiteralValue? Literal { get; init; }
    public required string? Identifier { get; init; }
}

public record class FmlInvocation : FmlNode
{
    public required string Identifier { get; init; }
    public required List<FmlInvocationParam> Parameters { get; init; }
}

public record class FmlTargetTransform : FmlNode
{
    public required LiteralValue? Literal { get; init; }
    public required string? Identifier { get; init; }
    public required FmlInvocation? Invocation { get; init; }
}

public record class FmlExpressionTarget : FmlNode
{
    public required string Identifier { get; init; }
    public required FmlTargetTransform? Transform { get; init; }
    public required FmlInvocation? Invocation { get; init; }
    public required string? Alias { get; init; }
    public required string? TargetListModeLiteral { get; init; }
    public required Hl7.Fhir.Model.StructureMap.StructureMapTargetListMode? TargetListMode { get; init; }
}

public record class FmlDependentExpression : FmlNode
{
    public required List<FmlInvocation> Invocations { get; init; }
    public required List<GroupExpression> Expressions { get; init; }
}

public record class FmlGroupExpression : FmlNode
{
    public required List<FmlExpressionSource> Sources { get; init; }
    public required List<FmlExpressionTarget> Targets { get; init; }
    public required FmlDependentExpression? DependentExpression { get; init; }
    public required string? Name { get; init; }
}

public record class GroupExpression : FmlNode
{
    public MapSimpleCopyExpression? SimpleCopyExpression { get; init; } = null;
    public FpExpression? FhirPathExpression { get; init; } = null;
    public FmlGroupExpression? MappingExpression { get; init; } = null;
}

public record class GroupDeclaration : FmlNode
{
    public required string Name { get; init; }
    public required List<GroupParameter> Parameters { get; init; }
    public required string? ExtendsIdentifier { get; init; }
    public required string? TypeModeLiteral { get; init; }
    public required Hl7.Fhir.Model.StructureMap.StructureMapGroupTypeMode? TypeMode { get; init; }
    public required List<GroupExpression> Expressions { get; init; }
}

public record class FpFunction : FmlNode
{
    public required string Identifier { get; init; }
    public required FmlTokenTypeCodes IdentifierTokenType { get; init; }
    public required List<FpExpression?> Parameters { get; init; }
}

public record class FpInvocation : FmlNode
{
    public FpFunction? FunctionInvocation { get; init; }
    public string? MemberInvocation { get; init; }
    public FmlTokenTypeCodes? MemberInvocationTokenType { get; init; }
    public bool? ThisInvocation { get; init; }
    public bool? IndexInvocation { get; init; }
    public bool? TotalInvocation { get; init; }
}

public record class LiteralValue : FmlNode
{
    public required string ValueAsString { get; init; }
    public required dynamic? Value { get; init; }
    public required Hl7.Fhir.Model.DataType? FhirValue { get; init; }
    public required FmlTokenTypeCodes TokenType { get; init; }
}

public record class FpTerm : FmlNode
{
    public FpInvocation? InvocationTerm { get; init; }
    public LiteralValue? LiteralTerm { get; init; }
    public string? ExternalConstantTerm { get; init; }
    public FmlTokenTypeCodes? ExternalConstantTokenType { get; init; }
    public FpExpression? ParenthesizedTerm { get; init; }
}

public record class FpInvocationExpression : FmlNode
{
    public required FpExpression Expression { get; init; }
    public required FpInvocation Invocation { get; init; }
}

public record class FpIndexerExpression : FmlNode
{
    public required FpExpression Expression { get; init; }
    public required FpExpression Index { get; init; }
}

public record class FpPolarityExpression : FmlNode
{
    public required FmlPolarityCodes Polarity { get; init; }
    public required string Literal { get; init; }
    public required bool IsPositive { get; init; }
    public required FpExpression Expression { get; init; }

}

public record class FpBinaryExpression<T> : FmlNode
{
    public required FpExpression Left { get; init; }
    public required T Operator { get; init; }
    public required string OperatorLiteral { get; init; }
    public required FpExpression Right { get; init; }
}

public record class FpTypeExpression : FmlNode
{
    public required FpExpression Expression { get; init; }
    public required string TypeAssignmentLiteral { get; init; }
    public required string TypeIdentifier { get; init; }
}


public record class FpExpression : FmlNode
{
    public required string Expression { get; init; }
    public required FmlRuleCodes ExpressionRule { get; init; }
    public FpTerm? TermExpression { get; init; } = null;
    public FpInvocationExpression? InvocationExpression { get; init; } = null;
    public FpIndexerExpression? IndexerExpression { get; init; } = null;
    public FpPolarityExpression? PolarityExpression { get; init; } = null;
    public FpBinaryExpression<FmlMultiplicativeOpCodes>? MultiplicativeExpression { get; init; } = null;
    public FpBinaryExpression<FmlAdditiveOpCodes>? AdditiveExpression { get; init; } = null;
    public FpTypeExpression? TypeExpression { get; init; } = null;
    public FpBinaryExpression<FmlUnionOpCodes>? UnionExpression { get; init; } = null;
    public FpBinaryExpression<FmlInequalityOpCodes>? InequalityExpression { get; init; } = null;
    public FpBinaryExpression<FmlEqualityOpCodes>? EqualityExpression { get; init; } = null;
    public FpBinaryExpression<FmlMembershipOpCodes>? MembershipExpression { get; init; } = null;
    public FpBinaryExpression<FmlAndOpCodes>? AndExpression { get; init; } = null;
    public FpBinaryExpression<FmlOrOpCodes>? OrExpression { get; init; } = null;
    public FpBinaryExpression<FmlImpliesOpCodes>? ImpliesExpression { get; init; } = null;
}

public record class FhirStructureMap
{
    public required Dictionary<string, MetadataDeclaration> MetadataByPath { get; init; }
    public MapDeclaration? MapDirective { get; init; }
    public required Dictionary<string, StructureDeclaration> StructuresByUrl { get; init; }
    public required Dictionary<string, ImportDeclaration> ImportsByUrl { get; init; }
    public required Dictionary<string, ConstantDeclaration> ConstantsByName { get; init; }
    public required Dictionary<string, GroupDeclaration> GroupsByName { get; init; }
}

