// <copyright file="LangOpenApi.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using System.IO;
using Microsoft.Health.Fhir.SpecManager.Manager;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using static Microsoft.Health.Fhir.SpecManager.Language.OpenApi.OpenApiCommon;

namespace Microsoft.Health.Fhir.SpecManager.Language.OpenApi;

public class LangOpenApi : ILanguage
{
    /// <summary>FHIR information we are exporting.</summary>
    private FhirVersionInfo _info;

    /// <summary>Options for controlling the export.</summary>
    private ExporterOptions _options;

    /// <summary>Pathname of the export directory.</summary>
    private string _exportDirectory;

    /// <summary>True to export enums.</summary>
    private bool _exportEnums = true;

    /// <summary>Name of the language.</summary>
    private const string _languageName = "OpenApi";

    /// <summary>The single file export extension - requires directory export.</summary>
    private const string _singleFileExportExtension = null;

    /// <summary>(Immutable) Pathname of the relative export directory.</summary>
    private const string _relativeExportDirectory = "";

    /// <summary>Gets the name of the language.</summary>
    /// <value>The name of the language.</value>
    string ILanguage.LanguageName => _languageName;

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

        string fileExt = openApiOptions.FileFormat.ToString().ToLower();

        if (serverInfo != null)
        {
            ModelBuilder builder = new(info, openApiOptions, options, serverInfo);

            OpenApiDocument doc = builder.Build();

            string filename = Path.Combine(exportDirectory, $"{_languageName}_{FhirPackageCommon.RForSequence(info.FhirSequence)}.{fileExt}");

            using (FileStream stream = new FileStream(filename, FileMode.Create))
            using (StreamWriter sw = new StreamWriter(stream))
            {
                IOpenApiWriter writer;

                switch (openApiOptions.FileFormat)
                {
                    case OaFileFormat.Json:
                    default:
                        writer = new OpenApiJsonWriter(sw, new OpenApiJsonWriterSettings() { Terse = openApiOptions.Minify });
                        break;
                    case OaFileFormat.Yaml:
                        writer = new OpenApiYamlWriter(sw);
                        break;
                }

                doc.Serialize(writer, openApiOptions.OpenApiVersion);
            }
        }
        else
        { 
            foreach (FhirCapabiltyStatement cap in info.CapabilitiesByUrl.Values)
            {
                ModelBuilder builder = new(info, openApiOptions, options, cap);

                OpenApiDocument doc = builder.Build();

                string filename = Path.Combine(exportDirectory, $"{_languageName}_{cap.Id}.{fileExt}");

                using (FileStream stream = new FileStream(filename, FileMode.Create))
                using (StreamWriter sw = new StreamWriter(stream))
                {
                    IOpenApiWriter writer;

                    switch (openApiOptions.FileFormat)
                    {
                        case OaFileFormat.Json:
                        default:
                            writer = new OpenApiJsonWriter(sw, new OpenApiJsonWriterSettings() { Terse = openApiOptions.Minify });
                            break;
                        case OaFileFormat.Yaml:
                            writer = new OpenApiYamlWriter(sw);
                            break;
                    }

                    doc.Serialize(writer, openApiOptions.OpenApiVersion);
                }
            }
        }
    }
}
