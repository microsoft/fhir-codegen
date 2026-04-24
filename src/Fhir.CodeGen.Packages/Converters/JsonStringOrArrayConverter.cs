using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fhir.CodeGen.Packages.Converters;

public sealed class JsonStringOrArrayConverter : JsonConverter<IReadOnlyList<string>?>
{
    private readonly char[] _splitChars = [',', ';', ' ', '\n', '\t'];

    public override IReadOnlyList<string>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string? singleValue = reader.GetString();
            if (string.IsNullOrEmpty(singleValue))
            {
                return null;
            }

            // check for an encoded array
            if (singleValue.StartsWith('[') && singleValue.EndsWith(']'))
            {
                string[] parts = singleValue[1..^1].Split(_splitChars, StringSplitOptions.RemoveEmptyEntries);
                return parts;
            }

            return [ singleValue ];
        }
        else if (reader.TokenType == JsonTokenType.StartArray)
        {
            List<string> values = [];
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    return values;
                }
                if (reader.TokenType == JsonTokenType.String)
                {
                    values.Add(reader.GetString() ?? string.Empty);
                }
            }
        }
        return null;
    }

    public override void Write(Utf8JsonWriter writer, IReadOnlyList<string>? value, JsonSerializerOptions options)
    {
        if (value == null || value.Count == 0)
        {
            writer.WriteNullValue();
        }
        else if (value.Count == 1)
        {
            writer.WriteStringValue(value[0]);
        }
        else
        {
            writer.WriteStartArray();
            foreach (var item in value)
            {
                writer.WriteStringValue(item);
            }
            writer.WriteEndArray();
        }
    }
}
