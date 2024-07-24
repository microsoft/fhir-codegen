// <copyright file="LoaderOptions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using System.Text.Json;
using Hl7.Fhir.Serialization;

namespace Microsoft.Health.Fhir.CodeGen.Loader;

/// <summary>
/// Represents the options for the loader.
/// </summary>
public record class LoaderOptions
{
    /// <summary>
    /// Represents the JSON deserialization model.
    /// </summary>
    public enum JsonDeserializationModel
    {
        Default,
        Poco,
        SystemTextJson,
    }

    /// <summary>
    /// Gets or sets the JSON model.
    /// </summary>
    /// <value>The JSON deserialization model.</value>
    public JsonDeserializationModel JsonModel { get; init; } = JsonDeserializationModel.Poco;

    /// <summary>
    /// Gets or sets options for controlling the FHIR JSON.
    /// </summary>
    /// <value>The JSON options for FHIR.</value>
    public JsonSerializerOptions FhirJsonOptions { get; init; } = new JsonSerializerOptions()
    {
        AllowTrailingCommas = true,
    }.ForFhir(new FhirJsonPocoDeserializerSettings()
    {
        DisableBase64Decoding = false,
        Validator = null,
    });

    /// <summary>
    /// Gets or sets the FHIR JSON settings.
    /// </summary>
    /// <value>The FHIR JSON settings.</value>
    public FhirJsonPocoDeserializerSettings FhirJsonSettings { get; init; } = new FhirJsonPocoDeserializerSettings()
    {
        DisableBase64Decoding = false,
        Validator = null,
    };

#if DISABLE_XML
    public object FhirXmlSettings { get; init; } = new();
#else
    /// <summary>
    /// Gets or sets the FHIR XML settings.
    /// </summary>
    /// <value>The FHIR XML settings.</value>
    public FhirXmlPocoDeserializerSettings FhirXmlSettings { get; init; } = new FhirXmlPocoDeserializerSettings()
    {
        DisableBase64Decoding = false,
        Validator = null,
    };
#endif
}
