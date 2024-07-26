// <copyright file="LangOpenApi.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.CommandLine;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using static Microsoft.Health.Fhir.CodeGen.Language.OpenApi.OpenApiCommon;

#if NETSTANDARD2_0
using Microsoft.Health.Fhir.CodeGen.Polyfill;
#endif

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

        if (config.ServerCapabilities != null)
        {
            string fileId = string.IsNullOrEmpty(config.OutputFilename)
                ? definitions.FhirSequence.ToRLiteral()
                : config.OutputFilename;

            if (config.MultiFile)
            {
                WriteAsMultiFile(config, doc, fileId);
            }
            else
            {
                WriteAsSingleFile(config, doc, fileId);
            }
        }
        else
        {
            foreach (var cs in definitions.CapabilityStatementsByUrl.Values)
            {
                string fileId = cs.Id;

                if (config.MultiFile)
                {
                    WriteAsMultiFile(config, doc, fileId);
                }
                else
                {
                    WriteAsSingleFile(config, doc, fileId);
                }
            }
        }
    }


    /// <summary>Writes as multi file.</summary>
    /// <param name="exportDirectory">Directory to write files.</param>
    /// <param name="config"> Options for controlling the open API.</param>
    /// <param name="completeDoc">            The document.</param>
    /// <param name="fileId">         Identifier for the file.</param>
    private void WriteAsMultiFile(
        OpenApiOptions config,
        OpenApiDocument completeDoc,
        string fileId)
    {
        Dictionary<string, OpenApiDocument> docsByPrefix = new();

        // traverse the paths to discover our root keys (mostly just resources)
        foreach ((string apiPath, OpenApiPathItem pathItem) in completeDoc.Paths)
        {
            string pathKey;
            string titleSuffix;

            if (apiPath.Equals("/", StringComparison.Ordinal) ||
                apiPath.Substring(0, 2).Equals("/_", StringComparison.Ordinal) ||
                apiPath.Substring(0, 2).Equals("/$", StringComparison.Ordinal) ||
                char.IsLower(apiPath[1]))
            {
                pathKey = "_SystemOperations";
                titleSuffix = "System Operations";
            }
            else
            {
                pathKey = apiPath.Split('/')[1];
                titleSuffix = pathKey;
            }

            if (!docsByPrefix.ContainsKey(pathKey))
            {
                OpenApiDocument doc = new();
                doc.Info = new OpenApiInfo(completeDoc.Info);
                doc.Info.Title += " - " + titleSuffix;

                doc.Components = new();
                doc.Components.Parameters = new Dictionary<string, OpenApiParameter>();
                doc.Components.Schemas = new Dictionary<string, OpenApiSchema>();
                doc.Components.SecuritySchemes = completeDoc.Components.SecuritySchemes;
                doc.SecurityRequirements = completeDoc.SecurityRequirements;
                doc.Components.Extensions = completeDoc.Components.Extensions;
                doc.Extensions = completeDoc.Extensions;

                doc.Paths = new();
                doc.Tags = new List<OpenApiTag>();

                doc.Servers = completeDoc.Servers.ToList();

                docsByPrefix.Add(pathKey, doc);
            }

            docsByPrefix[pathKey].Paths.Add(apiPath, new OpenApiPathItem(pathItem));
        }

        Dictionary<string, OpenApiTag> sourceTags = completeDoc.Tags.ToDictionary(t => t.Name);

        // traverse each partial document, resolve missing references and write
        foreach ((string pathKey, OpenApiDocument doc) in docsByPrefix)
        {
            ResolveContainedRefs(completeDoc, doc, sourceTags);

            WriteAsSingleFile(config, doc, fileId + "_" + pathKey);
        }
    }

    /// <summary>Add any used parameters missing from the target.</summary>
    /// <param name="newParameters">Options for controlling the new.</param>
    /// <param name="source">       Another instance to copy.</param>
    /// <param name="target">       Target for the.</param>
    private static void MaybeAddParameters(IList<OpenApiParameter> newParameters, OpenApiDocument source, OpenApiDocument target)
    {
        foreach (OpenApiParameter targetParam in newParameters)
        {
            // only need to resolve references, full parameters were copied
            if (!string.IsNullOrEmpty(targetParam.Reference?.Id ?? null) &&
                !target.Components.Parameters.ContainsKey(targetParam.Reference!.Id))
            {
                target.Components.Parameters.Add(
                    targetParam.Reference.Id,
                    source.Components.Parameters[targetParam.Reference.Id]);
            }
        }
    }

    /// <summary>Copies the nested definitions.</summary>
    /// <param name="source">Another instance to copy.</param>
    /// <param name="target">Target for the.</param>
    private void ResolveContainedRefs(
        OpenApiDocument source,
        OpenApiDocument target,
        Dictionary<string, OpenApiTag> sourceTags)
    {
        HashSet<string> usedTags = new();

        foreach ((string targetPathKey, OpenApiPathItem targetPath) in target.Paths)
        {
            MaybeAddParameters(targetPath.Parameters, source, target);

            foreach ((OperationType targetOpKey, OpenApiOperation targetOp) in targetPath.Operations)
            {
                foreach (OpenApiTag targetTag in targetOp.Tags)
                {
                    // only need to resolve references, actual tags were copied
                    if (string.IsNullOrEmpty(targetTag.Reference?.Id ?? null) ||
                        usedTags.Contains(targetTag.Reference!.Id))
                    {
                        continue;
                    }

                    if (sourceTags.TryGetValue(targetTag.Reference.Id, out OpenApiTag? sourceTag))
                    {
                        target.Tags.Add(sourceTag);
                        usedTags.Add(targetTag.Reference.Id);
                    }
                    else
                    {
                        target.Tags.Add(new OpenApiTag(targetTag));
                        usedTags.Add(targetTag.Reference.Id);
                    }
                }

                MaybeAddParameters(targetOp.Parameters, source, target);

                foreach (OpenApiMediaType targetMedia in targetOp.RequestBody?.Content?.Values ?? Array.Empty<OpenApiMediaType>())
                {
                    // only process references, the rest were copied
                    if (string.IsNullOrEmpty(targetMedia.Schema?.Reference?.Id ?? null) ||
                        target.Components.Schemas.ContainsKey(targetMedia.Schema!.Reference.Id))
                    {
                        continue;
                    }

                    CopySchemaRecursive(source, target, targetMedia.Schema.Reference.Id);
                }

                foreach (OpenApiResponse targetResponse in targetOp.Responses.Values)
                {
                    foreach (OpenApiMediaType targetMedia in targetResponse.Content.Values)
                    {
                        // only process references, the rest were copied
                        if (string.IsNullOrEmpty(targetMedia.Schema?.Reference?.Id ?? null) ||
                            target.Components.Schemas.ContainsKey(targetMedia.Schema!.Reference.Id))
                        {
                            continue;
                        }

                        CopySchemaRecursive(source, target, targetMedia.Schema.Reference.Id);
                    }
                }
            }
        }
    }

    /// <summary>Copies the schema recursive.</summary>
    /// <param name="source">Another instance to copy.</param>
    /// <param name="target">Target for the.</param>
    /// <param name="key">   The key.</param>
    private void CopySchemaRecursive(
        OpenApiDocument source,
        OpenApiDocument target,
        string key)
    {
        if (target.Components.Schemas.ContainsKey(key))
        {
            return;
        }

        target.Components.Schemas.Add(key, source.Components.Schemas[key]);

        foreach (OpenApiSchema s in target.Components.Schemas[key].AllOf ?? Array.Empty<OpenApiSchema>())
        {
            //ts.AllOf.Add(s);

            if (string.IsNullOrEmpty(s.Reference?.Id ?? null) ||
                target.Components.Schemas.ContainsKey(s.Reference!.Id))
            {
                continue;
            }

            CopySchemaRecursive(source, target, s.Reference.Id);
        }

        // check properties
        foreach (OpenApiSchema s in target.Components.Schemas[key].Properties?.Values ?? Array.Empty<OpenApiSchema>())
        {
            if (string.IsNullOrEmpty(s.Reference?.Id ?? null) ||
                target.Components.Schemas.ContainsKey(s.Reference!.Id))
            {
                continue;
            }

            CopySchemaRecursive(source, target, s.Reference.Id);
        }
    }

    /// <summary>Writes as single file.</summary>
    /// <param name="config">The configuration.</param>
    /// <param name="doc">   The document.</param>
    /// <param name="fileId">(Optional) Identifier for the file.</param>
    private static void WriteAsSingleFile(
        OpenApiOptions config,
        OpenApiDocument doc,
        string fileId)
    {
        string filename = Path.Combine(config.OutputDirectory, $"{_languageName}_{fileId}.{config.FileFormat.ToString().ToLowerInvariant()}");

        using StreamWriter sw = config.WriteStream == null
            ? new StreamWriter(new FileStream(filename, FileMode.Create))
            : new StreamWriter(config.WriteStream, System.Text.Encoding.UTF8, 512 * 1024, true);

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

        sw.Flush();

        if (config.WriteStream == null)
        {
            sw.Close();
        }
    }
}
