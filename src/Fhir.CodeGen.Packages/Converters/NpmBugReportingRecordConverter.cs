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
/// Deserializes the npm 'bugs' field which may be:
///   "string"
///   { "url"?: "...", "email"?: "..." }
/// Always returns a list of NpmBugReportingRecord (or null).
/// Serialization is intentionally not supported.
/// </summary>
public sealed class NpmBugReportingRecordConverter : JsonConverter<IReadOnlyList<NpmBugReportingRecord>?>
{
    public override IReadOnlyList<NpmBugReportingRecord>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Null => null,
            JsonTokenType.String => new List<NpmBugReportingRecord> { FromString(ref reader) },
            JsonTokenType.StartObject => new List<NpmBugReportingRecord> { FromObject(ref reader) },
            JsonTokenType.StartArray => FromArray(ref reader),
            _ => throw new JsonException("Invalid token for bug-reporting field.")
        };
    }

    public override void Write(Utf8JsonWriter writer, IReadOnlyList<NpmBugReportingRecord>? value, JsonSerializerOptions options)
        => throw new NotSupportedException("Serialization of bug-reporting is not supported (read-only).");

    private static NpmBugReportingRecord FromString(ref Utf8JsonReader reader)
        => new() { Url = reader.GetString() };

    private static NpmBugReportingRecord FromObject(ref Utf8JsonReader reader)
    {
        string? email = null;
        string? url = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Expected property name in bug-reporting object.");

            string prop = reader.GetString() ?? string.Empty;
            if (!reader.Read())
                throw new JsonException("Unexpected end of object in bug-reporting entry.");

            switch (prop.ToLowerInvariant())
            {
                case "email":
                    {
                        if (reader.TokenType == JsonTokenType.String)
                        {
                            email = reader.GetString();
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

        return new NpmBugReportingRecord
        {
            Email = email,
            Url = url,
        };
    }

    private static IReadOnlyList<NpmBugReportingRecord> FromArray(ref Utf8JsonReader reader)
    {
        List<NpmBugReportingRecord> list = [];
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            list.Add(reader.TokenType switch
            {
                JsonTokenType.String => FromString(ref reader),
                JsonTokenType.StartObject => FromObject(ref reader),
                _ => throw new JsonException("Array bug-reporting entries must be string or object.")
            });
        }
        return list;
    }
}
