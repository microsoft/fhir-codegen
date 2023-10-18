// <copyright file="FhirVersionConverter.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using System.Formats.Asn1;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Microsoft.Health.Fhir.PackageManager.Models;

public class FhirVersionConverter : JsonConverter<string>
{
    public override bool CanConvert(Type objectType)
    {
        return ((objectType == typeof(string)) || objectType.IsAssignableTo(typeof(IEnumerable<string>)));
    }

    /// <summary>Reads and converts the JSON to type <typeparamref name="T" />.</summary>
    /// <param name="reader">       [in,out] The reader.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">      An object that specifies serialization options to use.</param>
    /// <returns>The converted value.</returns>
    public override string Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.None:
                break;
            case JsonTokenType.StartObject:
                break;
            case JsonTokenType.EndObject:
                break;
            case JsonTokenType.StartArray:
                {
                    List<string> result = new();
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndArray)
                        {
                            break;
                        }

                        if (reader.TokenType == JsonTokenType.String)
                        {
                            result.Add(reader.GetString() ?? string.Empty);
                        }
                    }

                    return "[" + string.Join(',', result) + "]";
                }

            case JsonTokenType.EndArray:
                break;
            case JsonTokenType.PropertyName:
                break;
            case JsonTokenType.Comment:
                break;
            case JsonTokenType.String:
                {
                    return reader.GetString() ?? string.Empty;
                }
            case JsonTokenType.Number:
                break;
            case JsonTokenType.True:
                break;
            case JsonTokenType.False:
                break;
            case JsonTokenType.Null:
                break;
            default:
                break;
        }

        return string.Empty;
    }

    /// <summary>Writes a specified value as JSON.</summary>
    /// <param name="writer"> The writer to write to.</param>
    /// <param name="value">  The value to convert to JSON.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    public override void Write(
        Utf8JsonWriter writer,
        string value,
        JsonSerializerOptions options) => writer.WriteStringValue(value);
}
