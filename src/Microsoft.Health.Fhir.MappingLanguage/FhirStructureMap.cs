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
    StructureMap = FmlMappingParser.RULE_structureMap, // 0
    ConceptMapDeclaration = FmlMappingParser.RULE_conceptMapDeclaration, // 1,
    ConceptMapPrefix = FmlMappingParser.RULE_conceptMapPrefix, // 2,
    ConceptMapCodeMap = FmlMappingParser.RULE_conceptMapCodeMap, // 3,
    ConceptMapSource = FmlMappingParser.RULE_conceptMapSource, // 4,
    ConceptMapTarget = FmlMappingParser.RULE_conceptMapTarget, // 5,
    Code = FmlMappingParser.RULE_code, // 6,
    MapDeclaration = FmlMappingParser.RULE_mapDeclaration, // 7,
    MetadataDeclaration = FmlMappingParser.RULE_metadataDeclaration, // 8,
    MarkdownLiteral = FmlMappingParser.RULE_markdownLiteral, // 9,
    Url = FmlMappingParser.RULE_url, // 10,
    Identifier = FmlMappingParser.RULE_identifier, // 11,
    StructureDeclaration = FmlMappingParser.RULE_structureDeclaration, // 12,
    ConstantDeclaration = FmlMappingParser.RULE_constantDeclaration, // 13,
    GroupDeclaration = FmlMappingParser.RULE_groupDeclaration, // 14,
    Parameters = FmlMappingParser.RULE_parameters, // 15,
    Parameter = FmlMappingParser.RULE_parameter, // 16,
    GroupExpressions = FmlMappingParser.RULE_groupExpressions, // 17,
    TypeMode = FmlMappingParser.RULE_typeMode, // 18,
    Extends = FmlMappingParser.RULE_extends, // 19,
    TypeIdentifier = FmlMappingParser.RULE_typeIdentifier, // 20,
    Expression = FmlMappingParser.RULE_expression, // 21,
    MapExpression = FmlMappingParser.RULE_mapExpression, // 22,
    MapExpressionName = FmlMappingParser.RULE_mapExpressionName, // 23,
    MapExpressionSource = FmlMappingParser.RULE_mapExpressionSource, // 24,
    MapExpressionTarget = FmlMappingParser.RULE_mapExpressionTarget, // 25,
    SourceCardinality = FmlMappingParser.RULE_sourceCardinality, // 26,
    UpperBound = FmlMappingParser.RULE_upperBound, // 27,
    QualifiedIdentifier = FmlMappingParser.RULE_qualifiedIdentifier, // 28,
    SourceDefault = FmlMappingParser.RULE_sourceDefault, // 29,
    Alias = FmlMappingParser.RULE_alias, // 30,
    WhereClause = FmlMappingParser.RULE_whereClause, // 31,
    CheckClause = FmlMappingParser.RULE_checkClause, // 32,
    Log = FmlMappingParser.RULE_log, // 33,
    DependentExpression = FmlMappingParser.RULE_dependentExpression, // 34,
    ImportDeclaration = FmlMappingParser.RULE_importDeclaration, // 35,
    MapLineTarget = FmlMappingParser.RULE_mapLineTarget, // 36,
    Transform = FmlMappingParser.RULE_transform, // 37,
    Invocation = FmlMappingParser.RULE_invocation, // 38,
    ParamList = FmlMappingParser.RULE_paramList, // 39,
    Param = FmlMappingParser.RULE_param, // 40,
    FpExpression = FmlMappingParser.RULE_fpExpression, // 41,
    FpTerm = FmlMappingParser.RULE_fpTerm, // 42,
    FpInvocation = FmlMappingParser.RULE_fpInvocation, // 43,
    FpExternalConstant = FmlMappingParser.RULE_fpExternalConstant, // 44,
    FpFunction = FmlMappingParser.RULE_fpFunction, // 45,
    FpParamList = FmlMappingParser.RULE_fpParamList, // 46,
    FpTypeSpecifier = FmlMappingParser.RULE_fpTypeSpecifier, // 47,
    Constant = FmlMappingParser.RULE_constant, // 48,
    Literal = FmlMappingParser.RULE_literal, // 49,
    FpQuantity = FmlMappingParser.RULE_fpQuantity, // 50,
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
    Conceptmap = FmlMappingParser.T__0,
    OpenCurlyBracket = FmlMappingParser.T__1,
    CloseCurlyBracket = FmlMappingParser.T__2,
    Prefix = FmlMappingParser.T__3,
    Equals = FmlMappingParser.T__4,
    Minus = FmlMappingParser.T__5,
    Colon = FmlMappingParser.T__6,
    Map = FmlMappingParser.T__7,
    Uses = FmlMappingParser.T__8,
    Alias = FmlMappingParser.T__9,
    As = FmlMappingParser.T__10,
    Source = FmlMappingParser.T__11,
    Queried = FmlMappingParser.T__12,
    Target = FmlMappingParser.T__13,
    Produced = FmlMappingParser.T__14,
    Let = FmlMappingParser.T__15,
    Semicolon = FmlMappingParser.T__16,
    Group = FmlMappingParser.T__17,
    OpenParenthesis = FmlMappingParser.T__18,
    Comma = FmlMappingParser.T__19,
    CloseParenthesis = FmlMappingParser.T__20,
    DoubleLessThan = FmlMappingParser.T__21,
    Types = FmlMappingParser.T__22,
    TypePlus = FmlMappingParser.T__23,
    DoubleGreaterThan = FmlMappingParser.T__24,
    Extends = FmlMappingParser.T__25,
    Arrow = FmlMappingParser.T__26,
    First = FmlMappingParser.T__27,
    NotFirst = FmlMappingParser.T__28,
    Last = FmlMappingParser.T__29,
    NotLast = FmlMappingParser.T__30,
    OnlyOne = FmlMappingParser.T__31,
    DoubleDot = FmlMappingParser.T__32,
    Asterisk = FmlMappingParser.T__33,
    Imports = FmlMappingParser.T__34,
    Where = FmlMappingParser.T__35,
    Check = FmlMappingParser.T__36,
    Div = FmlMappingParser.T__37,
    Contains = FmlMappingParser.T__38,
    Is = FmlMappingParser.T__39,
    Dot = FmlMappingParser.T__40,
    Default = FmlMappingParser.T__41,
    Log = FmlMappingParser.T__42,
    Then = FmlMappingParser.T__43,
    Share = FmlMappingParser.T__44,
    Single = FmlMappingParser.T__45,
    OpenSquareBracket = FmlMappingParser.T__46,
    CloseSquareBracket = FmlMappingParser.T__47,
    Plus = FmlMappingParser.T__48,
    Slash = FmlMappingParser.T__49,
    Mod = FmlMappingParser.T__50,
    Ampersand = FmlMappingParser.T__51,
    Pipe = FmlMappingParser.T__52,
    LessOrEqual = FmlMappingParser.T__53,
    LessThan = FmlMappingParser.T__54,
    GreaterThan = FmlMappingParser.T__55,
    GreaterOrEqual = FmlMappingParser.T__56,
    Tilde = FmlMappingParser.T__57,
    NotEqual = FmlMappingParser.T__58,
    NotEquivalent = FmlMappingParser.T__59,
    In = FmlMappingParser.T__60,
    And = FmlMappingParser.T__61,
    Or = FmlMappingParser.T__62,
    Xor = FmlMappingParser.T__63,
    Implies = FmlMappingParser.T__64,
    DollarThis = FmlMappingParser.T__65,
    DollarIndex = FmlMappingParser.T__66,
    DollarTotal = FmlMappingParser.T__67,
    Percent = FmlMappingParser.T__68,
    Year = FmlMappingParser.T__69,
    Month = FmlMappingParser.T__70,
    Week = FmlMappingParser.T__71,
    Day = FmlMappingParser.T__72,
    Hour = FmlMappingParser.T__73,
    Minute = FmlMappingParser.T__74,
    Second = FmlMappingParser.T__75,
    Millisecond = FmlMappingParser.T__76,
    Years = FmlMappingParser.T__77,
    Months = FmlMappingParser.T__78,
    Weeks = FmlMappingParser.T__79,
    Days = FmlMappingParser.T__80,
    Hours = FmlMappingParser.T__81,
    Minutes = FmlMappingParser.T__82,
    Seconds = FmlMappingParser.T__83,
    Milliseconds = FmlMappingParser.T__84,
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
}

public enum FmlPolarityCodes : int
{
    // '+' : 84 + 1
    Positive = FmlTokenTypeCodes.Plus,

    // '-' : 85 + 1
    Negative = FmlTokenTypeCodes.Minus,
}

public enum FmlMultiplicativeOpCodes : int
{
    // '*': 74 + 1
    Multiply = FmlTokenTypeCodes.Asterisk,

    // '/': 86 + 1
    Divide = FmlTokenTypeCodes.Slash,

    // 'div': 87 + 1
    Div = FmlTokenTypeCodes.Div,

    // 'mod': 88 + 1
    Modulo = FmlTokenTypeCodes.Mod,
}

public enum FmlUnionOpCodes : int
{
    // '|': 91 + 1
    Union = FmlTokenTypeCodes.Pipe,
}

public enum FmlAdditiveOpCodes : int
{
    // '+': 84 + 1
    Add = FmlTokenTypeCodes.Plus,

    // '&': 89 + 1
    And = FmlTokenTypeCodes.Ampersand,

    // '-': 85 + 1
    Subtract = FmlTokenTypeCodes.Minus,
}

public enum FmlInequalityOpCodes : int
{
    // '<': 93 + 1
    LessThan = FmlTokenTypeCodes.LessThan,

    // '<=': 92 + 1
    LessThanOrEqual = FmlTokenTypeCodes.LessOrEqual,

    // '>': 94 + 1
    GreaterThan = FmlTokenTypeCodes.GreaterThan,

    // '>=': 95 + 1
    GreaterThanOrEqual = FmlTokenTypeCodes.GreaterOrEqual,
}

public enum FmlEqualityOpCodes : int
{
    // '=': 55 + 1
    Equal = FmlTokenTypeCodes.Equals,

    // '~': 96 + 1
    Equivalent = FmlTokenTypeCodes.Tilde,

    // '!=': 97 + 1
    NotEqual = FmlTokenTypeCodes.NotEqual,

    // '!~': 98 + 1
    NotEquivalent = FmlTokenTypeCodes.NotEquivalent,
}

public enum FmlMembershipOpCodes : int
{
    // 'in': 99 + 1
    In = FmlTokenTypeCodes.In,

    // 'contains': 100 + 1
    Contains = FmlTokenTypeCodes.Contains,
}

public enum FmlAndOpCodes : int
{
    // 'and': 101 + 1
    And = FmlTokenTypeCodes.And,
}

public enum FmlOrOpCodes : int
{
    // 'or': 102 + 1
    Or = FmlTokenTypeCodes.Or,

    // 'xor': 103 + 1
    Xor = FmlTokenTypeCodes.Xor,
}

public enum FmlImpliesOpCodes : int
{
    // 'implies': 104 + 1
    Implies = FmlTokenTypeCodes.Implies,
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

public record class EmbeddedConceptMapDeclaration : FmlNode
{
    public required string Url { get; init; }
    public required Dictionary<string, EmbeddedConceptMapPrefix> Prefixes { get; init; }
    public required List<EmbeddedConceptMapCodeMap> CodeMaps { get; init; }
}

public record class EmbeddedConceptMapPrefix : FmlNode
{
    public required string Prefix { get; init; }
    public required string Url { get; init; }
}

public record class EmbeddedConceptMapCodeMap : FmlNode
{
    public required string SourcePrefix { get; init; }
    public required string SourceCode { get; init; }
    public required string TargetPrefix { get; init; }
    public required string TargetCode { get; init; }
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
    public required FpExpression? fpExpression { get; init; }
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
    public required Dictionary<string, EmbeddedConceptMapDeclaration> EmbeddedConceptMapsByUrl { get; init; }
    public MapDeclaration? MapDirective { get; init; }
    public required Dictionary<string, StructureDeclaration> StructuresByUrl { get; init; }
    public required Dictionary<string, ImportDeclaration> ImportsByUrl { get; init; }
    public required Dictionary<string, ConstantDeclaration> ConstantsByName { get; init; }
    public required Dictionary<string, GroupDeclaration> GroupsByName { get; init; }
}

