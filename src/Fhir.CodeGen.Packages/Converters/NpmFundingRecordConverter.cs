using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Fhir.CodeGen.Packages.Models;

namespace Fhir.CodeGen.Packages.Converters;

/// <summary>
/// Deserializes the npm 'funding' field which may be:
///   "string"
///   { "type"?: "...", "url"?: "..." }
///   [ "string" | { ... }, ... ]
/// Always returns a list of NpmFundingRecord (or null).
/// Serialization is intentionally not supported.
/// </summary>
public sealed class NpmFundingRecordConverter : JsonConverter<IReadOnlyList<NpmFundingRecord>?>
{
    public override IReadOnlyList<NpmFundingRecord>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Null => null,
            JsonTokenType.String => new List<NpmFundingRecord> { FromString(ref reader) },
            JsonTokenType.StartObject => new List<NpmFundingRecord> { FromObject(ref reader) },
            JsonTokenType.StartArray => FromArray(ref reader),
            _ => throw new JsonException("Invalid token for funding field.")
        };
    }

    public override void Write(Utf8JsonWriter writer, IReadOnlyList<NpmFundingRecord>? value, JsonSerializerOptions options)
        => throw new NotSupportedException("Serialization of funding is not supported (read-only).");

    private static NpmFundingRecord FromString(ref Utf8JsonReader reader)
        => new() { Url = reader.GetString() };

    private static NpmFundingRecord FromObject(ref Utf8JsonReader reader)
    {
        string? type = null;
        string? url = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Expected property name in funding object.");

            string prop = reader.GetString() ?? string.Empty;
            if (!reader.Read())
                throw new JsonException("Unexpected end of object in funding entry.");

            switch (prop.ToLowerInvariant())
            {
                case "type":
                    {
                        if (reader.TokenType == JsonTokenType.String)
                        {
                            type = reader.GetString();
                        }
                        else if (reader.TokenType != JsonTokenType.Null)
                        {
                            reader.TrySkip();
                        }
                    }
                    break;

                case "url":
                    {
                        if (reader.TokenType == JsonTokenType.String)
                        {
                            url = reader.GetString();
                        }
                        else if (reader.TokenType != JsonTokenType.Null)
                        {
                            reader.TrySkip();
                        }
                    }
                    break;

                default:
                    reader.TrySkip();
                    break;
            }
        }

        return new NpmFundingRecord { Type = type, Url = url };
    }

    private static IReadOnlyList<NpmFundingRecord> FromArray(ref Utf8JsonReader reader)
    {
        List<NpmFundingRecord> list = [];
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            list.Add(reader.TokenType switch
            {
                JsonTokenType.String => FromString(ref reader),
                JsonTokenType.StartObject => FromObject(ref reader),
                _ => throw new JsonException("Array funding entries must be string or object.")
            });
        }
        return list;
    }
}
