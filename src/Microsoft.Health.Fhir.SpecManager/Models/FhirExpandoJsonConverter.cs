// <copyright file="FhirExpandoJsonConverter.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Microsoft.Health.Fhir.SpecManager.Models;

/// <summary>A FHIR expando JSON converter.</summary>
public class FhirExpandoJsonConverter : JsonConverter<FhirExpando>
{
    /// <summary>Reads and converts the JSON to type <typeparamref name="T" />.</summary>
    /// <exception cref="JsonException">Thrown when a JSON error condition occurs.</exception>
    /// <param name="reader">       [in,out] The reader.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">      An object that specifies serialization options to use.</param>
    /// <returns>The converted value.</returns>
    public override FhirExpando Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException($"JsonTokenType was of type {reader.TokenType}, only objects are supported");
        }

        FhirExpando eo = new();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return eo;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException("JsonTokenType was not PropertyName");
            }

            string propertyName = reader.GetString() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new JsonException("Failed to get property name");
            }

            reader.Read();

            eo.TryAdd(propertyName, ExtractValue(ref reader, options));
        }

        return eo;
    }

    /// <summary>Writes a specified value as JSON.</summary>
    /// <param name="writer"> The writer to write to.</param>
    /// <param name="value">  The value to convert to JSON.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    public override void Write(Utf8JsonWriter writer, FhirExpando value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (IDictionary<string, object?>)value, options);
    }

    /// <summary>Extracts the value.</summary>
    /// <exception cref="JsonException">Thrown when a JSON error condition occurs.</exception>
    /// <param name="reader"> [in,out] The reader.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    /// <returns>The extracted value.</returns>
    private object? ExtractValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                return reader.GetString();

            case JsonTokenType.False:
                return false;

            case JsonTokenType.True:
                return true;

            case JsonTokenType.Null:
                return null;

            case JsonTokenType.Number:
                if (reader.TryGetInt32(out int vi))
                {
                    return vi;
                }

                if (reader.TryGetInt64(out long vl))
                {
                    return vl;
                }

                return reader.GetDecimal();

            case JsonTokenType.StartObject:
                return Read(ref reader, typeof(FhirExpando), options);

            case JsonTokenType.StartArray:
                List<object?> list = new();
                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    list.Add(ExtractValue(ref reader, options));
                }
                return list;

            default:
                throw new JsonException($"'{reader.TokenType}' is not supported");
        }
    }
}
