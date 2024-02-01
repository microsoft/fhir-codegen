// <copyright file="LangInfo.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.CommandLine;

namespace Microsoft.Health.Fhir.CodeGen.Lanugage.Info;

/// <summary>Class used to export package/specification information.</summary>
public class LangInfo : ILanguage
{
    /// <summary>Values that represent Information formats.</summary>
    public enum InfoFormat
    {
        Text,
        JSON,
    }

    /// <summary>Gets the language name.</summary>
    public string Name => "Info";

    /// <summary>Dictionary mapping FHIR primitive types to language equivalents.</summary>
    private static readonly Dictionary<string, string> _primitiveTypeMap = new()
        {
            { "base", "base" },
            { "base64Binary", "base64Binary" },
            { "boolean", "boolean" },
            { "canonical", "canonical" },
            { "code", "code" },
            { "date", "date" },
            { "dateTime", "dateTime" },
            { "decimal", "decimal" },
            { "id", "id" },
            { "instant", "instant" },
            { "integer", "integer" },
            { "integer64", "integer64" },
            { "markdown", "markdown" },
            { "oid", "oid" },
            { "positiveInt", "positiveInt" },
            { "string", "string" },
            { "time", "time" },
            { "unsignedInt", "unsignedInt" },
            { "uri", "uri" },
            { "url", "url" },
            { "uuid", "uuid" },
            { "xhtml", "xhtml" },
        };

    /// <summary>Gets the FHIR primitive type map.</summary>
    public Dictionary<string, string> FhirPrimitiveTypeMap => _primitiveTypeMap;

    /// <summary>Gets options for controlling the language.</summary>
    public Dictionary<string, Option> LanguageOptions => new()
    {
        { "format", new Option<InfoFormat>(
            name: "--file-format",
            getDefaultValue: () => InfoFormat.Text,
            "Export file format.")
        },
    };
}
