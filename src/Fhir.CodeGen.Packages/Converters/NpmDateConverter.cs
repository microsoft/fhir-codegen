using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fhir.CodeGen.Packages.Converters;

/// <summary>
/// System.Text.Json converter for manifest date values in the format yyyyMMddHHmmss.
/// Reads/writes a DateTime? as that compact UTC string. Returns null for blank or unparsable values.
/// </summary>
public sealed class NpmDateConverter : JsonConverter<DateTime?>
{
    private const string Format = "yyyyMMddHHmmss";

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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

        if (DateTime.TryParseExact(
                dateString,
                Format,
                CultureInfo.InvariantCulture.DateTimeFormat,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out DateTime dt))
        {
            return dt;
        }

        // Match prior behavior: silently fall back to null if it does not parse
        return null;
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
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
