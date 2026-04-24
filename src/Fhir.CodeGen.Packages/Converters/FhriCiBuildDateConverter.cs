using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fhir.CodeGen.Packages.Converters;

public sealed class FhriCiBuildDateConverter : JsonConverter<DateTimeOffset?>
{
    private const string Format = "ddd, dd MMM, yyyy HH':'mm':'ss zzz";

    public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string for manifest date, found {reader.TokenType}.");
        }

        string? dateString = reader.GetString();

        if (string.IsNullOrWhiteSpace(dateString))
        {
            return null;
        }

        if (DateTimeOffset.TryParseExact(
                dateString,
                Format,
                CultureInfo.InvariantCulture.DateTimeFormat,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out DateTimeOffset dto))
        {
            return dto;
        }

        // Match prior behavior: silently fall back to null if it does not parse
        return null;
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        // Emit in UTC using the required compact form
        writer.WriteStringValue(value.Value.ToUniversalTime().ToString(Format, CultureInfo.InvariantCulture));
    }
}
