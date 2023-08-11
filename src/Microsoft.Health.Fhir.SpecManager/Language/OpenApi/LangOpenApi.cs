// <copyright file="LangOpenApi.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using System.IO;
using Microsoft.Health.Fhir.SpecManager.Manager;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using static Microsoft.Health.Fhir.SpecManager.Language.OpenApi.OpenApiCommon;

namespace Microsoft.Health.Fhir.SpecManager.Language.OpenApi;

/// <summary>Language export for OpenAPI.</summary>
public class LangOpenApi : ILanguage
{
    /// <summary>Name of the language.</summary>
    private const string _languageName = "OpenApi";

    /// <summary>The single file export extension - requires directory export.</summary>
    private const string _singleFileExportExtension = null;

    /// <summary>Gets the name of the language.</summary>
    /// <value>The name of the language.</value>
    string ILanguage.LanguageName => _languageName;

    string ILanguage.Namespace
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    /// <summary>
    /// Gets the single file extension for this language - null or empty indicates a multi-file
    /// export (exporter should copy the contents of the directory).
    /// </summary>
    string ILanguage.SingleFileExportExtension => _singleFileExportExtension;

    /// <summary>Gets the FHIR primitive type map.</summary>
    /// <value>The FHIR primitive type map.</value>
    Dictionary<string, string> ILanguage.FhirPrimitiveTypeMap => _primitiveTypeMap;

    /// <summary>Gets the reserved words.</summary>
    /// <value>The reserved words.</value>
    HashSet<string> ILanguage.ReservedWords => _reservedWords;

    /// <summary>
    /// Gets a list of FHIR class types that the language WILL export, regardless of user choices.
    /// Used to provide information to users.
    /// </summary>
    List<ExporterOptions.FhirExportClassType> ILanguage.RequiredExportClassTypes => new()
    {
        ExporterOptions.FhirExportClassType.PrimitiveType,
        ExporterOptions.FhirExportClassType.ComplexType,
        ExporterOptions.FhirExportClassType.Resource,
        ExporterOptions.FhirExportClassType.Interaction,
    };

    /// <summary>
    /// Gets a list of FHIR class types that the language CAN export, depending on user choices.
    /// </summary>
    List<ExporterOptions.FhirExportClassType> ILanguage.OptionalExportClassTypes => new()
    {
        //ExporterOptions.FhirExportClassType.Profile,
    };

    /// <summary>Gets language-specific options and their descriptions.</summary>
    Dictionary<string, string> ILanguage.LanguageOptions => OpenApiOptions.LanguageOptions;

    void ILanguage.Export(
        FhirVersionInfo info,
        FhirComplex complex,
        Stream outputStream)
        => throw new NotImplementedException();

    /// <summary>Export the passed FHIR version into the specified directory.</summary>
    /// <param name="info">           The information.</param>
    /// <param name="serverInfo">     Information describing the server.</param>
    /// <param name="options">        Options for controlling the operation.</param>
    /// <param name="exportDirectory">Directory to write files.</param>
    void ILanguage.Export(
        FhirVersionInfo info,
        FhirCapabiltyStatement serverInfo,
        ExporterOptions options,
        string exportDirectory)
    {
        OpenApiOptions openApiOptions = new OpenApiOptions(options);

        if (serverInfo != null)
        {
            ModelBuilder builder = new(info, openApiOptions, options, serverInfo);

            OpenApiDocument doc = builder.Build();

            if (openApiOptions.MultiFile)
            {
                WriteAsMultiFile(exportDirectory, openApiOptions, doc, FhirPackageCommon.RForSequence(info.FhirSequence));
            }
            else
            {
                WriteAsSingleFile(exportDirectory, openApiOptions, doc, FhirPackageCommon.RForSequence(info.FhirSequence));
            }
        }
        else
        {
            foreach (FhirCapabiltyStatement cap in info.CapabilitiesByUrl.Values)
            {
                ModelBuilder builder = new(info, openApiOptions, options, cap);

                OpenApiDocument doc = builder.Build();

                if (openApiOptions.MultiFile)
                {
                    WriteAsMultiFile(exportDirectory, openApiOptions, doc, cap.Id);
                }
                else
                {
                    WriteAsSingleFile(exportDirectory, openApiOptions, doc, cap.Id);
                }
            }
        }
    }

    /// <summary>Writes as multi file.</summary>
    /// <param name="exportDirectory">Directory to write files.</param>
    /// <param name="openApiOptions"> Options for controlling the open API.</param>
    /// <param name="completeDoc">            The document.</param>
    /// <param name="fileId">         Identifier for the file.</param>
    private void WriteAsMultiFile(
        string exportDirectory,
        OpenApiOptions openApiOptions,
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

            WriteAsSingleFile(exportDirectory, openApiOptions, doc, fileId + "_" + pathKey);
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
                    !target.Components.Parameters.ContainsKey(targetParam.Reference.Id))
            {
                target.Components.Parameters.Add(
                    targetParam.Reference.Id,
                    source.Components.Parameters[targetParam.Reference.Id]);
            }
        }
    }

    /// <summary>Copies the nested defs.</summary>
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
                        usedTags.Contains(targetTag.Reference.Id))
                    {
                        continue;
                    }

                    target.Tags.Add(sourceTags[targetTag.Reference.Id]);
                    usedTags.Add(targetTag.Reference.Id);
                }

                MaybeAddParameters(targetOp.Parameters, source, target);

                foreach (OpenApiMediaType targetMedia in targetOp.RequestBody?.Content?.Values ?? Array.Empty<OpenApiMediaType>())
                {
                    // only process references, the rest were copied
                    if (string.IsNullOrEmpty(targetMedia.Schema?.Reference?.Id ?? null) ||
                        target.Components.Schemas.ContainsKey(targetMedia.Schema.Reference.Id))
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
                            target.Components.Schemas.ContainsKey(targetMedia.Schema.Reference.Id))
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
                target.Components.Schemas.ContainsKey(s.Reference.Id))
            {
                continue;
            }

            CopySchemaRecursive(source, target, s.Reference.Id);
        }

        // check properties
        foreach (OpenApiSchema s in target.Components.Schemas[key].Properties?.Values ?? Array.Empty<OpenApiSchema>())
        {
            if (string.IsNullOrEmpty(s.Reference?.Id ?? null) ||
                target.Components.Schemas.ContainsKey(s.Reference.Id))
            {
                continue;
            }

            CopySchemaRecursive(source, target, s.Reference.Id);
        }
    }

    /// <summary>Writes an OpenApi Document as single file.</summary>
    /// <param name="exportDirectory">Directory to write files.</param>
    /// <param name="openApiOptions"> Options for controlling the open API.</param>
    /// <param name="doc">            The document.</param>
    /// <param name="fileId">         Identifier for the file.</param>
    private static void WriteAsSingleFile(
        string exportDirectory,
        OpenApiOptions openApiOptions,
        OpenApiDocument doc,
        string fileId)
    {
        string filename = Path.Combine(exportDirectory, $"{_languageName}_{fileId}.{openApiOptions.FileFormat.ToString().ToLowerInvariant()}");

        using FileStream stream = new FileStream(filename, FileMode.Create);
        using StreamWriter sw = new StreamWriter(stream);

        IOpenApiWriter writer = openApiOptions.FileFormat switch
        {
            OaFileFormat.Yaml => new OpenApiYamlWriter(sw),
            _ => new OpenApiJsonWriter(sw, new OpenApiJsonWriterSettings() { Terse = openApiOptions.Minify }),
        };

        doc.Serialize(writer, openApiOptions.OpenApiVersion);

    }
}
