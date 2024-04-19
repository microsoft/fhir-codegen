// <copyright file="LangOpenApi.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.CommandLine;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using static Microsoft.Health.Fhir.CodeGen.Language.OpenApi.OpenApiCommon;

namespace Microsoft.Health.Fhir.CodeGen.Language.OpenApi;

/// <summary>Class used to export OpenAPI definitions.</summary>
public class LangOpenApi : ILanguage
{
    private const string _languageName = "OpenApi";

    /// <summary>Gets the language name.</summary>
    public string Name => _languageName;

    public Type ConfigType => typeof(OpenApiOptions);

    /// <summary>Gets the FHIR primitive type map.</summary>
    public Dictionary<string, string> FhirPrimitiveTypeMap => _primitiveTypeMap;

    /// <summary>Gets a value indicating whether this language is idempotent.</summary>
    public bool IsIdempotent => true;

    public void Export(object untypedConfig, DefinitionCollection definitions)
    {
        if (untypedConfig is not OpenApiOptions config)
        {
            throw new ArgumentException("Invalid configuration type");
        }

        ModelBuilder modelBuilder = new(definitions, config);

        OpenApiDocument doc = modelBuilder.Build();

        if (config.MultiFile)
        {
            //WriteAsMultiFile(config, doc);
        }
        else
        {
            WriteAsSingleFile(config, doc);
        }
    }

    /// <summary>Writes as single file.</summary>
    /// <param name="config">The configuration.</param>
    /// <param name="doc">   The document.</param>
    /// <param name="fileId">(Optional) Identifier for the file.</param>
    private static void WriteAsSingleFile(
        OpenApiOptions config,
        OpenApiDocument doc,
        string fileId = "")
    {
        string filename = string.IsNullOrEmpty(fileId)
            ? Path.Combine(config.OutputDirectory, $"{_languageName}.{config.FileFormat.ToString().ToLowerInvariant()}")
            : Path.Combine(config.OutputDirectory, $"{_languageName}_{fileId}.{config.FileFormat.ToString().ToLowerInvariant()}");

        //using Stream stream = config.WriteStream ?? new FileStream(filename, FileMode.Create);
        using StreamWriter sw = config.WriteStream == null
            ? new StreamWriter(new FileStream(filename, FileMode.Create), leaveOpen: false)
            : new StreamWriter(config.WriteStream, leaveOpen: config.WriteStream != null);

        IOpenApiWriter writer = config.FileFormat switch
        {
            OaFileFormat.YAML => new OpenApiYamlWriter(sw),
            _ => new OpenApiJsonWriter(sw, new OpenApiJsonWriterSettings() { Terse = config.Minify }),
        };

        doc.Serialize(
            writer,
            config.OpenApiVersion == OaVersion.v2
                ? Microsoft.OpenApi.OpenApiSpecVersion.OpenApi2_0
                : Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0);
    }
}
