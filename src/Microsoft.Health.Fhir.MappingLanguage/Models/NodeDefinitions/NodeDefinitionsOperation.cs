// <copyright file="ExpressionDefinitionsOperation.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.MappingLanguage.Expression;

public static class OperationCodeExtensions
{
    /// <summary>
    /// A string extension method that converts a literal to an operation code.
    /// </summary>
    /// <param name="literal">The literal to act on.</param>
    /// <returns>Literal as an ExpressionNode.OperationCodes?</returns>
    public static ExpressionNode.OperationCodes? ToOperationCode(this string literal) => ExpressionNode.ToOperationCode(literal);

    /// <summary>
    /// The ExpressionNode.OperationCodes extension method that converts an operationCode to a
    /// literal.
    /// </summary>
    /// <param name="operationCode">The operation code.</param>
    /// <returns>OperationCode as a string.</returns>
    public static string ToString(this ExpressionNode.OperationCodes operationCode) => ExpressionNode.ToLiteral(operationCode);
}

/// <content>An expression node.</content>
public partial class ExpressionNode
{
    /// <summary>Values that represent operation codes.</summary>
    public enum OperationCodes
    {
        And,
        As,
        Concatenate,
        Contains,
        Div,
        DivideBy,
        Equals,
        Equivalent,
        Greater,
        GreaterOrEqual,
        Implies,
        In,
        Is,
        LessOrEqual,
        LessThan,
        MemberOf,
        Minus,
        Mod,
        NotEquals,
        NotEquivalent,
        Or,
        Plus,
        Times,
        Union,
        Xor,
    }

    /// <summary>Converts a literal to an operation code.</summary>
    /// <param name="literal">The literal.</param>
    /// <returns>Literal as an OperationCodes?</returns>
    public static OperationCodes? ToOperationCode(string literal) => literal.ToLower() switch
    {
        "and" => OperationCodes.And,
        "as" => OperationCodes.As,
        "&" => OperationCodes.Concatenate,
        "contains" => OperationCodes.Contains,
        "div" => OperationCodes.Div,
        "/" => OperationCodes.DivideBy,
        "=" => OperationCodes.Equals,
        "~" => OperationCodes.Equivalent,
        ">" => OperationCodes.Greater,
        ">=" => OperationCodes.GreaterOrEqual,
        "implies" => OperationCodes.Implies,
        "in" => OperationCodes.In,
        "is" => OperationCodes.Is,
        "<=" => OperationCodes.LessOrEqual,
        "<" => OperationCodes.LessThan,
        "memberOf" => OperationCodes.MemberOf,
        "-" => OperationCodes.Minus,
        "mod" => OperationCodes.Mod,
        "!=" => OperationCodes.NotEquals,
        "!~" => OperationCodes.NotEquivalent,
        "or" => OperationCodes.Or,
        "+" => OperationCodes.Plus,
        "*" => OperationCodes.Times,
        "|" => OperationCodes.Union,
        "xor" => OperationCodes.Xor,
        _ => null,
    };

    /// <summary>
    /// Converts the given <see cref="OperationCodes"/> to its corresponding literal representation.
    /// </summary>
    /// <param name="operationCode">The operation code.</param>
    /// <returns>The literal representation of the <see cref="FunctionCodes"/>.</returns>
    public static string ToLiteral(OperationCodes operationCode) => operationCode switch
    {
        OperationCodes.And => "and",
        OperationCodes.As => "as",
        OperationCodes.Concatenate => "&",
        OperationCodes.Contains => "contains",
        OperationCodes.Div => "div",
        OperationCodes.DivideBy => "/",
        OperationCodes.Equals => "=",
        OperationCodes.Equivalent => "~",
        OperationCodes.Greater => ">",
        OperationCodes.GreaterOrEqual => ">=",
        OperationCodes.Implies => "implies",
        OperationCodes.In => "in",
        OperationCodes.Is => "is",
        OperationCodes.LessOrEqual => "<=",
        OperationCodes.LessThan => "<",
        OperationCodes.MemberOf => "memberOf",
        OperationCodes.Minus => "-",
        OperationCodes.Mod => "mod",
        OperationCodes.NotEquals => "!=",
        OperationCodes.NotEquivalent => "!~",
        OperationCodes.Or => "or",
        OperationCodes.Plus => "+",
        OperationCodes.Times => "*",
        OperationCodes.Union => "|",
        OperationCodes.Xor => "xor",
        _ => string.Empty,      // throw new ArgumentOutOfRangeException(nameof(operationCode), operationCode, null),
    };

}
