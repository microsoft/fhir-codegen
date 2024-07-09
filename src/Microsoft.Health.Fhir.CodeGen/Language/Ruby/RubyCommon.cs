// <copyright file="RubyCommon.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Health.Fhir.CodeGen.Language.Ruby;

public static class RubyCommon
{
    public static HashSet<string> ReservedWords =
    [
        "__ENCODING__",
        "__LINE__",
        "__FILE__",
        "BEGIN",
        "END",
        "alias",
        "and",
        "begin",
        "break",
        "case",
        "class",
        "def",
        "defined?",
        "do",
        "else",
        "elseif",
        "end",
        "ensure",
        "false",
        "for",
        "if",
        "in",
        "module",
        "next",
        "nil",
        "not",
        "or",
        "redo",
        "rescue",
        "retry",
        "return",
        "self",
        "super",
        "then",
        "true",
        "undef",
        "unless",
        "until",
        "when",
        "while",
        "yield",
        "method",           // note this is not actually a reserved word, but treated as one in the output
    ];

    public static void WriteRubyComment(
        this ExportStreamWriter writer,
        string value)
    {
        writer.WriteLineIndented("##");
        foreach (string line in value.Split('\n'))
        {
            writer.WriteLineIndented($"# {line}");
        }
    }
}
