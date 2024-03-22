// <copyright file="LangOpenApi.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.CommandLine;
using Microsoft.Health.Fhir.CodeGen.Models;
using static Microsoft.Health.Fhir.CodeGen.Lanugage.OpenApi.OpenApiCommon;

namespace Microsoft.Health.Fhir.CodeGen.Lanugage.OpenApi;

/// <summary>Class used to export OpenAPI definitions.</summary>
public class LangOpenApi : ILanguage
{
    /// <summary>Gets the language name.</summary>
    public string Name => "OpenApi";

    public Type ConfigType => typeof(OpenApiOptions);

    /// <summary>Gets the FHIR primitive type map.</summary>
    public Dictionary<string, string> FhirPrimitiveTypeMap => _primitiveTypeMap;

    /// <summary>Gets a value indicating whether this language is idempotent.</summary>
    public bool IsIdempotent => true;

    public void Export(object config, DefinitionCollection definitions) { }
}
