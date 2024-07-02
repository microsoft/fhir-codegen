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
    public static void WriteIndentedComment(
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
