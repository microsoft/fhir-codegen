using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Fhir.CodeGen.Packages.Models;
using Hl7.FhirPath.Sprache;

namespace Fhir.CodeGen.Packages.Converters;

/// <summary>
/// Custom converter to handle the various representations NpmPersonRecord:
///     "name"
///     "name &lt;email&gt;"
///     "&lt;email&gt;"
///     "name &lt;email&gt; (url)"
///     "&lt;email&gt; (url)"
///     "(url)"
///     "name (url)"
///     { "name"?: "...", "email"?: "...", "url"?: "...", "username"?: "...", "_avatar"?: "..." }
///     [ "string" | { ... }, ... ]
/// Always returns a list of NpmPersonRecord (or null).
/// Serialization is intentionally not supported.
/// </summary>
internal sealed class NpmPersonRecordConverter : JsonConverter<IReadOnlyList<NpmPersonRecord>>
{
    private const char _emailStartLiteral = '<';
    private const char _emailEndLiteral = '>';
    private const char _urlStartLiteral = '(';
    private const char _urlEndLiteral = ')';

    public override IReadOnlyList<NpmPersonRecord>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Null => null,
            JsonTokenType.String => new List<NpmPersonRecord> { FromString(ref reader) },
            JsonTokenType.StartObject => new List<NpmPersonRecord> { FromObject(ref reader) },
            JsonTokenType.StartArray => FromArray(ref reader),
            _ => throw new JsonException("Invalid token for person field.")
        };
    }

    public override void Write(Utf8JsonWriter writer, IReadOnlyList<NpmPersonRecord>? value, JsonSerializerOptions options)
        => throw new NotSupportedException("Serialization of person is not supported (read-only).");

    private static NpmPersonRecord FromString(ref Utf8JsonReader reader)
    {
        string? raw = reader.GetString();
        if (string.IsNullOrWhiteSpace(raw))
        {
            return new()
            {
                Name = string.Empty,
            };
        }

        ReadOnlySpan<char> span = raw.AsSpan().Trim();

        int emailStart = span.IndexOf(_emailStartLiteral);
        int emailEnd = emailStart >= 0 ? span.Slice(emailStart + 1).IndexOf(_emailEndLiteral) : -1;
        int urlStart = span.IndexOf(_urlStartLiteral);
        int urlEnd = urlStart >= 0 ? span.Slice(urlStart + 1).IndexOf(_urlEndLiteral) : -1;
        int nameEnd = span.Length;

        if (emailStart >= 0)
        {
            nameEnd = emailStart;
        }
        else if (urlStart >= 0)
        {
            nameEnd = urlStart;
        }

        return new()
        {
            Username = null,
            Name = (nameEnd > 0) ? span.Slice(0, nameEnd).Trim().ToString() : string.Empty,
            Email = (emailStart >= 0 && emailEnd >= 0) ? span.Slice(emailStart + 1, emailEnd).Trim().ToString() : null,
            Url = (urlStart >= 0 && urlEnd >= 0) ? span.Slice(urlStart + 1, urlEnd).Trim().ToString() : null,
        };
    }

    private static NpmPersonRecord FromObject(ref Utf8JsonReader reader)
    {
        string? username = null;
        string? name = null;
        string? email = null;
        string? url = null;
        string? avatar = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Expected property name in person object.");

            string prop = reader.GetString() ?? string.Empty;
            if (!reader.Read())
                throw new JsonException("Unexpected end of object in person entry.");

            switch (prop.ToLowerInvariant())
            {
                case "author":
                    {
                        if (reader.TokenType == JsonTokenType.String)
                        {
                            // author is a full string, parse it but keep going so we don't have to deal with unwinding the reader
                            NpmPersonRecord author = FromString(ref reader);

                            username = author.Username;
                            name = author.Name;
                            email = author.Email;
                            url = author.Url;
                            avatar = author.Avatar;
                        }
                        else if (reader.TokenType != JsonTokenType.Null)
                        {
                            reader.TrySkip();
                        }
                    }
                    break;

                case "username":
                    {
                        if (reader.TokenType == JsonTokenType.String)
                        {
                            username = reader.GetString();
                        }
                        else if (reader.TokenType != JsonTokenType.Null)
                        {
                            reader.TrySkip();
                        }
                    }
                    break;

                case "name":
                    {
                        if (reader.TokenType == JsonTokenType.String)
                        {
                            name = reader.GetString();
                        }
                        else if (reader.TokenType != JsonTokenType.Null)
                        {
                            reader.TrySkip();
                        }
                    }
                    break;

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

                case "_avatar":
                    {
                        if (reader.TokenType == JsonTokenType.String)
                        {
                            avatar = reader.GetString();
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

        return new()
        {
            Username = username,
            Name = name ?? string.Empty,
            Email = email,
            Url = url,
            Avatar = avatar,
        };
    }

    private static IReadOnlyList<NpmPersonRecord> FromArray(ref Utf8JsonReader reader)
    {
        List<NpmPersonRecord> list = [];
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            list.Add(reader.TokenType switch
            {
                JsonTokenType.String => FromString(ref reader),
                JsonTokenType.StartObject => FromObject(ref reader),
                _ => throw new JsonException("Array person entries must be string or object.")
            });
        }
        return list;
    }



}
