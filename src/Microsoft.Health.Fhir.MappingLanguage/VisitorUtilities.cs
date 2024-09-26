// <copyright file="VisitorUtilities.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Hl7.Fhir.Model;
using Newtonsoft.Json.Linq;
using static FmlMappingParser;

namespace Microsoft.Health.Fhir.MappingLanguage;

internal static class VisitorUtilities
{
    internal static List<string> GetPrefixComments(ParserRuleContext ctx, Dictionary<int, ParsedCommentNode> parsedComments)
    {
        List<string> comments = [];

        int nextLoc = ctx.Start.StartIndex;

        while (parsedComments.TryGetValue(nextLoc, out ParsedCommentNode? comment))
        {
            comments.Add(comment.NodeText);
            //nextLoc = nextLoc == comment.StartIndex - 1 ? -1 : comment.StartIndex - 1;
            nextLoc = nextLoc == comment.LastWsStartIndex - 1 ? -1 : comment.LastWsStartIndex - 1;
        }

        return comments;
    }

    internal static List<string> GetPostfixComments(ParserRuleContext ctx, Dictionary<int, ParsedCommentNode> parsedComments)
    {
        List<string> comments = [];

        int nextLoc = ctx.Stop.StopIndex + 1;

        while (parsedComments.TryGetValue(nextLoc, out ParsedCommentNode? comment) && (comment.LastWsHasNewline == false))
        {
            comments.Add(comment.NodeText);
            nextLoc = nextLoc == comment.StopIndex + 1 ? -1 : comment.StopIndex + 1;
        }

        return comments;
    }

    internal static (string value, FmlTokenTypeCodes matchedToken)? GetGrammarLiteral(ParserRuleContext c, params int[] literalsToMatch)
    {
        HashSet<int> hash = new HashSet<int>(literalsToMatch);

        if (c.children == null) return null;

        // first pass just check terminal nodes
        foreach (IParseTree child in c.children)
        {
            if (child is not ITerminalNode tn)
            {
                continue;
            }

            if (hash.Contains(tn.Symbol.Type))
            {
                return (GetString(tn), (FmlTokenTypeCodes)tn.Symbol.Type);
            }
        }

        // if we are still here, recurse into non-terminal nodes
        foreach (IParseTree child in c.children)
        {
            if (child is not ParserRuleContext cc)
            {
                continue;
            }

            (string v, FmlTokenTypeCodes tt)? res = GetGrammarLiteral(cc, literalsToMatch);

            if (res != null)
            {
                return res;
            }
        }

        return null;
    }

    internal static LiteralValue? GetLiteral(ParserRuleContext? c, Dictionary<int, ParsedCommentNode>? parsedComments = null)
    {
        if (c == null)
        {
            return null;
        }

        return new LiteralValue()
        {
            ValueAsString = GetString(c) ?? string.Empty,
            Value = GetValue(c),
            FhirValue = GetFhirValue(c),
            TokenType = (FmlTokenTypeCodes)c.Stop.Type,

            RawText = c.GetText() ?? string.Empty,
            PrefixComments = parsedComments == null ? [] : GetPrefixComments(c, parsedComments),
            PostfixComments = parsedComments == null ? [] : GetPostfixComments(c, parsedComments),
            Line = c.Start.Line,
            Column = c.Start.Column,
            StartIndex = c.Start.StartIndex,
            StopIndex = c.Stop.StopIndex,
        };
    }

    internal static string? GetString(ParserRuleContext? c) => c?.Stop.Type switch
    {
        null => null,
        NULL_LITERAL => null,
        BOOL => c.GetText(),
        DATE => c.Stop.Text.StartsWith("@") ? c.Stop.Text[1..] : c.Stop.Text,
        DATE_TIME => c.Stop.Text.StartsWith("@") ? c.Stop.Text : c.Stop.Text,
        TIME => c.Stop.Text.StartsWith("@") ? c.Stop.Text : c.Stop.Text,
        LONG_INTEGER => c.GetText(),
        DECIMAL => c.GetText(),
        INTEGER => c.GetText(),
        ID => c.GetText(),
        IDENTIFIER => c.Stop.Text.Length > 1 && c.Stop.Text[0] == '\'' && c.Stop.Text[^1] == '\'' ? c.Stop.Text[1..^1] : c.Stop.Text,
        DELIMITED_IDENTIFIER => c.Stop.Text[1..^1],
        SINGLE_QUOTED_STRING => c.Stop.Text[1..^1],
        DOUBLE_QUOTED_STRING => c.Stop.Text[1..^1],
        BLOCK_COMMENT => c.Stop.Text.Length > 4 ? c.Stop.Text[2..^2].Trim() : c.Stop.Text.Trim(),
        LINE_COMMENT => c.Stop.Text.Length > 2 ? c.Stop.Text[2..].Trim() : c.Stop.Text.Trim(),
        TRIPLE_QUOTED_STRING_LITERAL => c.Stop.Text.Trim().Length > 5 ? c.Stop.Text.Trim()[3..^3].Trim() : c.Stop.Text.Trim(),
        _ => c.GetText().Trim(),
    };

    internal static string GetString(ITerminalNode? tn) => tn?.Symbol.Type switch
    {
        null => string.Empty,
        NULL_LITERAL => string.Empty,
        BOOL => tn.GetText(),
        DATE => tn.Symbol.Text.StartsWith("@") ? tn.Symbol.Text[1..] : tn.Symbol.Text,
        DATE_TIME => tn.Symbol.Text.StartsWith("@") ? tn.Symbol.Text : tn.Symbol.Text,
        TIME => tn.Symbol.Text.StartsWith("@") ? tn.Symbol.Text : tn.Symbol.Text,
        LONG_INTEGER => tn.GetText(),
        DECIMAL => tn.GetText(),
        INTEGER => tn.GetText(),
        ID => tn.GetText(),
        IDENTIFIER => tn.Symbol.Text.Length > 1 && tn.Symbol.Text[0] == '\'' && tn.Symbol.Text[^1] == '\'' ? tn.Symbol.Text[1..^1] : tn.Symbol.Text,
        DELIMITED_IDENTIFIER => tn.Symbol.Text[1..^1],
        SINGLE_QUOTED_STRING => tn.Symbol.Text[1..^1],
        DOUBLE_QUOTED_STRING => tn.Symbol.Text[1..^1],
        BLOCK_COMMENT => tn.Symbol.Text.Length > 4 ? tn.Symbol.Text[2..^2].Trim() : tn.Symbol.Text.Trim(),
        LINE_COMMENT => tn.Symbol.Text.Length > 2 ? tn.Symbol.Text[2..].Trim() : tn.Symbol.Text.Trim(),
        TRIPLE_QUOTED_STRING_LITERAL => tn.Symbol.Text.Trim().Length > 5 ? tn.Symbol.Text.Trim()[3..^3].Trim() : tn.Symbol.Text.Trim(),
        _ => tn?.Symbol.Text.Trim() ?? string.Empty,
    };

    internal static string GetString(IToken? tn) => tn?.Type switch
    {
        null => string.Empty,
        NULL_LITERAL => string.Empty,
        BOOL => tn.Text,
        DATE => tn.Text.StartsWith("@") ? tn.Text[1..] : tn.Text,
        DATE_TIME => tn.Text.StartsWith("@") ? tn.Text : tn.Text,
        TIME => tn.Text.StartsWith("@") ? tn.Text : tn.Text,
        LONG_INTEGER => tn.Text,
        DECIMAL => tn.Text,
        INTEGER => tn.Text,
        ID => tn.Text,
        IDENTIFIER => tn.Text.Length > 1 && tn.Text[0] == '\'' && tn.Text[^1] == '\'' ? tn.Text[1..^1] : tn.Text,
        DELIMITED_IDENTIFIER => tn.Text[1..^1],
        SINGLE_QUOTED_STRING => tn.Text[1..^1],
        DOUBLE_QUOTED_STRING => tn.Text[1..^1],
        BLOCK_COMMENT => tn.Text.Length > 4 ? tn.Text[2..^2].Trim() : tn.Text.Trim(),
        LINE_COMMENT => tn.Text.Length > 2 ? tn.Text[2..].Trim() : tn.Text.Trim(),
        TRIPLE_QUOTED_STRING_LITERAL => tn.Text.Trim().Length > 5 ? tn.Text.Trim()[3..^3].Trim() : tn.Text.Trim(),
        _ => tn?.Text ?? string.Empty,
    };

    internal static dynamic? GetValue(ParserRuleContext? c) => c?.Stop.Type switch
    {
        null => null,
        NULL_LITERAL => null,
        BOOL => c.Stop.Text == "true" ? true : false,
        DATE => TryParseDateString(c.Stop.Text, out DateTimeOffset value) ? value : null,
        DATE_TIME => TryParseDateString(c.Stop.Text, out DateTimeOffset value) ? value : null,
        TIME => TryParseDateString(c.Stop.Text, out DateTimeOffset value) ? value : null,
        LONG_INTEGER => long.TryParse(c.Stop.Text, out long value) ? value : null,
        DECIMAL => decimal.TryParse(c.Stop.Text, out decimal value) ? value : null,
        INTEGER => int.TryParse(c.Stop.Text, out int value) ? value : null,
        ID => c.Stop.Text,
        IDENTIFIER => c.Stop.Text.Length > 1 && c.Stop.Text[0] == '\'' && c.Stop.Text[^1] == '\'' ? c.Stop.Text[1..^1] : c.Stop.Text,
        DELIMITED_IDENTIFIER => c.Stop.Text[1..^1],
        SINGLE_QUOTED_STRING => c.Stop.Text[1..^1],
        DOUBLE_QUOTED_STRING => c.Stop.Text[1..^1],
        BLOCK_COMMENT => c.Stop.Text.Length > 4 ? c.Stop.Text[2..^2].Trim() : c.Stop.Text.Trim(),
        LINE_COMMENT => c.Stop.Text.Length > 2 ? c.Stop.Text[2..].Trim() : c.Stop.Text.Trim(),
        TRIPLE_QUOTED_STRING_LITERAL => c.Stop.Text.Trim().Length > 5 ? c.Stop.Text.Trim()[3..^3].Trim() : c.Stop.Text.Trim(),
        _ => c?.Stop.Text.Trim(),
    };

    internal static Hl7.Fhir.Model.DataType? GetFhirValue(ParserRuleContext? c) => c?.Stop.Type switch
    {
        null => null,
        NULL_LITERAL => null,
        BOOL => c.Stop.Text == "true" ? new FhirBoolean(true) : new FhirBoolean(false),
        DATE => c.Stop.Text.StartsWith("@") ? new FhirDateTime(c.Stop.Text[1..]) : new FhirDateTime(c.Stop.Text),
        DATE_TIME => c.Stop.Text.StartsWith("@") ? new FhirDateTime(c.Stop.Text) : new FhirDateTime(c.Stop.Text),
        TIME => c.Stop.Text.StartsWith("@") ? new Time(c.Stop.Text) : new Time(c.Stop.Text),
        LONG_INTEGER => long.TryParse(c.Stop.Text, out long value) ? new Integer64(value) : null,
        DECIMAL => decimal.TryParse(c.Stop.Text, out decimal value) ? new FhirDecimal(value) : null,
        INTEGER => int.TryParse(c.Stop.Text, out int value) ? new Integer64(value) : null,
        ID => new Id(c.Stop.Text),
        IDENTIFIER => c.Stop.Text.Length > 1 && c.Stop.Text[0] == '\'' && c.Stop.Text[^1] == '\'' ? new FhirString(c.Stop.Text[1..^1]) : new FhirString(c.Stop.Text),
        DELIMITED_IDENTIFIER => new FhirString(c.Stop.Text[1..^1]),
        SINGLE_QUOTED_STRING => new FhirString(c.Stop.Text[1..^1]),
        DOUBLE_QUOTED_STRING => new FhirString(c.Stop.Text[1..^1]),
        BLOCK_COMMENT => c.Stop.Text.Length > 4 ? new FhirString(c.Stop.Text[2..^2].Trim()) : new FhirString(c.Stop.Text.Trim()),
        LINE_COMMENT => c.Stop.Text.Length > 2 ? new FhirString(c.Stop.Text[2..].Trim()) : new FhirString(c.Stop.Text.Trim()),
        TRIPLE_QUOTED_STRING_LITERAL => c.Stop.Text.Trim().Length > 5 ? new Markdown(c.Stop.Text.Trim()[3..^3].Trim()) : new Markdown(c.Stop.Text.Trim()),
        _ => string.IsNullOrEmpty(c?.Stop.Text) ? null : new FhirString(c!.Stop.Text),
    };

    internal static T? GetEnum<T>(string? value)
        where T : Enum
    {
        if (string.IsNullOrEmpty(value))
        {
            return default;
        }

        return (T?)Hl7.Fhir.Utility.EnumUtility.ParseLiteral(value, typeof(T));
    }

    internal static bool TryParseDateString(string dateString, out DateTimeOffset dto)
    {
        if (string.IsNullOrEmpty(dateString))
        {
            dto = DateTimeOffset.MinValue;
            return false;
        }

        if (dateString.StartsWith("@"))
        {
            dateString = dateString[1..];
        }

        // need to check for just year because DateTime refuses to parse that
        if (dateString.Length == 4)
        {
            dto = new DateTimeOffset(int.Parse(dateString), 1, 1, 0, 0, 0, TimeSpan.Zero);
            return true;
        }

        // note that we are using DateTime and converting to DateTimeOffset to work through TZ stuff without manually parsing each format precision
        if (!DateTime.TryParse(dateString, null, DateTimeStyles.RoundtripKind, out DateTime dt))
        {
            Console.WriteLine($"Failed to parse date: {dateString}");
            dto = DateTimeOffset.MinValue;
            return false;
        }

        dto = new DateTimeOffset(dt, TimeSpan.Zero);

        return true;
    }
}
