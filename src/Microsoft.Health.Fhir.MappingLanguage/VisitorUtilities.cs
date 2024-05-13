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
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Hl7.Fhir.Model;
using Newtonsoft.Json.Linq;
using static FmlMappingParser;

namespace Microsoft.Health.Fhir.MappingLanguage;

internal static class VisitorUtilities
{
    internal static LiteralValue? GetLiteral(ParserRuleContext? c)
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
        };
    }

    internal static string? GetString(ParserRuleContext? c) => c?.Stop.Type switch
    {
        NULL_LITERAL => null,
        BOOL => c.GetText(),
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
        TRIPLE_QUOTED_STRING_LITERAL => c.Stop.Text.Trim().Length > 5 ? c.Stop.Text.Trim()[3..^3].Trim() : c.Stop.Text.Trim(),
        _ => null,
    };

    internal static string GetString(ITerminalNode? tn) => tn?.Symbol.Type switch
    {
        NULL_LITERAL => string.Empty,
        BOOL => tn.GetText(),
        DATE => tn.Symbol.Text.StartsWith('@') ? tn.Symbol.Text[1..] : tn.Symbol.Text,
        DATE_TIME => tn.Symbol.Text.StartsWith('@') ? tn.Symbol.Text : tn.Symbol.Text,
        TIME => tn.Symbol.Text.StartsWith('@') ? tn.Symbol.Text : tn.Symbol.Text,
        LONG_INTEGER => tn.GetText(),
        DECIMAL => tn.GetText(),
        INTEGER => tn.GetText(),
        ID => tn.GetText(),
        IDENTIFIER => tn.Symbol.Text.Length > 1 && tn.Symbol.Text[0] == '\'' && tn.Symbol.Text[^1] == '\'' ? tn.Symbol.Text[1..^1] : tn.Symbol.Text,
        DELIMITED_IDENTIFIER => tn.Symbol.Text[1..^1],
        SINGLE_QUOTED_STRING => tn.Symbol.Text[1..^1],
        DOUBLE_QUOTED_STRING => tn.Symbol.Text[1..^1],
        C_STYLE_COMMENT => tn.Symbol.Text.Length > 4 ? tn.Symbol.Text[2..^2] : tn.Symbol.Text,
        LINE_COMMENT => tn.Symbol.Text.Length > 2 ? tn.Symbol.Text[2..] : tn.Symbol.Text,
        TRIPLE_QUOTED_STRING_LITERAL => tn.Symbol.Text.Trim().Length > 5 ? tn.Symbol.Text.Trim()[3..^3].Trim() : tn.Symbol.Text.Trim(),
        _ => string.Empty,
    };


    internal static dynamic? GetValue(ParserRuleContext c) => c.Stop.Type switch
    {
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
        C_STYLE_COMMENT => c.Stop.Text.Length > 4 ? c.Stop.Text[2..^2] : c.Stop.Text,
        LINE_COMMENT => c.Stop.Text.Length > 2 ? c.Stop.Text[2..] : c.Stop.Text,
        TRIPLE_QUOTED_STRING_LITERAL => c.Stop.Text.Trim().Length > 5 ? c.Stop.Text.Trim()[3..^3].Trim() : c.Stop.Text.Trim(),
        _ => null,
    };

    internal static Hl7.Fhir.Model.DataType? GetFhirValue(ParserRuleContext c) => c.Stop.Type switch
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
        TRIPLE_QUOTED_STRING_LITERAL => c.Stop.Text.Trim().Length > 5 ? new Markdown(c.Stop.Text.Trim()[3..^3].Trim()) : new Markdown(c.Stop.Text.Trim()),
        _ => null,
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

        if (dateString.StartsWith('@'))
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
