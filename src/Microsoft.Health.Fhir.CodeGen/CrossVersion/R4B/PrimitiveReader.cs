// <copyright file="PrimitiveReader.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Reflection.Metadata;
using System.Text.Json;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;

namespace Microsoft.Health.Fhir.CodeGen.CrossVersion.R4B;

/// <summary>A primitive reader.</summary>
public static class PrimitiveReader
{
    private static void Recover(ref Utf8JsonReader reader)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.None:
                return;
            case JsonTokenType.Null:
            case JsonTokenType.Number or JsonTokenType.String:
            case JsonTokenType.True or JsonTokenType.False:
                reader.Read();
                return;
            case JsonTokenType.PropertyName:
                SkipTo(ref reader, JsonTokenType.PropertyName);
                return;
            case JsonTokenType.StartArray:
                SkipTo(ref reader, JsonTokenType.EndArray);
                reader.Read();
                return;
            case JsonTokenType.StartObject:
                SkipTo(ref reader, JsonTokenType.EndObject);
                reader.Read();
                return;
            default:
                throw new InvalidOperationException($"Cannot recover, aborting. Token {reader.TokenType} was unexpected at this point.");
        }
    }

    private static void SkipTo(ref Utf8JsonReader reader, JsonTokenType tt)
    {
        var depth = reader.CurrentDepth;

        while (reader.Read() && reader.CurrentDepth >= depth)
        {
            if (reader.CurrentDepth == depth && reader.TokenType == tt) break;
        }
    }

    /// <summary>Deserialize primitive value.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="reader">          [in,out] The reader.</param>
    /// <param name="implementingType">Type of the implementing.</param>
    /// <param name="fhirType">        Type of the FHIR.</param>
    /// <returns>A Tuple.</returns>
    internal static (object?, FhirJsonException?) DeserializePrimitiveValue(ref Utf8JsonReader reader, Type implementingType, Type? fhirType)
    {
        // Check for unexpected non-value types.
        if (reader.TokenType is JsonTokenType.StartObject or JsonTokenType.StartArray)
        {
            var exception = reader.TokenType == JsonTokenType.StartObject
                ? new FhirJsonException("JSON104", "Expected a primitive value, not a json object.")
                : new FhirJsonException("JSON105", "Expected a primitive value, not the start of an array.");
            Recover(ref reader);
            return (null, exception);
        }

        // Check for value types
        (object? partial, FhirJsonException? error) result = reader.TokenType switch
        {
            JsonTokenType.Null => new(null, new FhirJsonException("JSON109", "Expected a primitive value, not a json null.")),
            JsonTokenType.String when string.IsNullOrEmpty(reader.GetString()) => new(reader.GetString(), new FhirJsonException("JSON127", "Properties cannot be empty strings. Either they are absent, or they are present with at least one character of non-whitespace content.")),
            JsonTokenType.String when implementingType == typeof(string) => new(reader.GetString(), null),
            JsonTokenType.String when implementingType == typeof(byte[]) => readBase64(ref reader),
            JsonTokenType.String when implementingType == typeof(DateTimeOffset) => readDateTimeOffset(ref reader),
            JsonTokenType.String when implementingType.IsEnum => new(reader.GetString(), null),
            JsonTokenType.String when implementingType == typeof(long) => readLong(ref reader, fhirType),
            //JsonTokenType.String when requiredType.IsEnum => readEnum(ref reader, requiredType),
            JsonTokenType.String => unexpectedToken(ref reader, reader.GetString(), implementingType.Name, "string"),
            JsonTokenType.Number => tryGetMatchingNumber(ref reader, implementingType, fhirType),
            JsonTokenType.True or JsonTokenType.False when implementingType == typeof(bool) => new(reader.GetBoolean(), null),
            JsonTokenType.True or JsonTokenType.False => unexpectedToken(ref reader, getRaw(ref reader), implementingType.Name, "boolean"),

            _ =>
                // This would be an internal logic error, since our callers should have made sure we're
                // on the primitive value after the property name (and the Utf8JsonReader would have complained about any
                // other token that one that is a value).
                // EK: I think 'Comment' is the only possible non-expected option here....
                throw new InvalidOperationException($"Unexpected token type {reader.TokenType} while parsing a primitive value. "),
        };

        // Read past the value
        reader.Read();

        return result;

        static string getRaw(ref Utf8JsonReader reader)
        {
            reader.Read();
            return System.Text.Encoding.UTF8.GetString(reader.ValueSpan);
        }

        static (object?, FhirJsonException) unexpectedToken(ref Utf8JsonReader reader, string? value, string expected, string actual) =>
            new(value, new FhirJsonException("JSON110", $"Expecting a {expected}, but found a json {actual} with value '{value}'."));

        static (object?, FhirJsonException?) readBase64(ref Utf8JsonReader reader) =>
            reader.TryGetBytesFromBase64(out var bytesValue) ?
                new(bytesValue, null) :
                new(reader.GetString(), new FhirJsonException("JSON106", "Encountered incorrectly encoded base64 data."));

        static (object?, FhirJsonException?) readDateTimeOffset(ref Utf8JsonReader reader)
        {
            var contents = reader.GetString()!;

            return Hl7.Fhir.ElementModel.Types.DateTime.TryParse(contents, out var parsed) ?
                new(parsed.ToDateTimeOffset(TimeSpan.Zero), null) :
                new(contents, new FhirJsonException("JSON107", $"Literal string '{contents}' cannot be parsed as an instant."));
        }

        static (object?, FhirJsonException?) readLong(ref Utf8JsonReader reader, Type? fhirType)
        {
            // convert string in json to a long.
            var contents = reader.GetString()!;

            return long.TryParse(contents, out var parsed) switch
            {
                true when isInteger64() => new(parsed, null),
                true => new(parsed, new FhirJsonException("JSON123", $"JsonString '{contents}' cannot be parsed as a {typeName()}, because it should be a Json number.")),
                false when isInteger64() => new(contents, new FhirJsonException("JSON122", $"Json string '{contents}' cannot be parsed as a {nameof(Integer64)}.")),
                false => new(contents, new FhirJsonException("JSON123", $"JsonString '{contents}' cannot be parsed as a {typeName()}, because it should be a Json number.")),
            };

            string typeName()
                => fhirType?.Name ?? string.Empty;

            bool isInteger64()
                => fhirType == typeof(Integer64);
        }

        // Validation is now done using POCO validation, so have removed it here.
        // Keep code around in case I make my mind up before publication.
        //static (object?, FhirJsonException?) readEnum(ref Utf8JsonReader reader, Type enumType)
        //{
        //    var contents = reader.GetString()!;
        //    var enumValue = EnumUtility.ParseLiteral(contents, enumType);

        //    return enumValue is not null
        //        ? (contents, null)
        //        : (contents, ERR.CODED_VALUE_NOT_IN_ENUM.With(ref reader, contents, EnumUtility.GetName(enumType)));
        //}

        /// <summary>
        /// This function tries to map from the json-format "generic" number to the kind of numeric type defined in the POCO.
        /// </summary>
        /// <remarks>Reader must be positioned on a number token. This function will not move the reader to the next token.</remarks>
        static (object?, FhirJsonException?) tryGetMatchingNumber(ref Utf8JsonReader reader, Type implementingType, Type? fhirType)
        {
            if (reader.TokenType != JsonTokenType.Number)
                throw new InvalidOperationException($"Cannot read a numeric when reader is on a {reader.TokenType}. ");

            object? value = null;
            bool success;

            if (implementingType == typeof(decimal))
                success = reader.TryGetDecimal(out decimal dec) && (value = dec) is { };
            else if (implementingType == typeof(int))
                success = reader.TryGetInt32(out int i32) && (value = i32) is { };
            else if (implementingType == typeof(uint))
                success = reader.TryGetUInt32(out uint ui32) && (value = ui32) is { };
            else if (implementingType == typeof(long))
                success = reader.TryGetInt64(out long i64) && (value = i64) is { };
            else if (implementingType == typeof(ulong))
                success = reader.TryGetUInt64(out ulong ui64) && (value = ui64) is { };
            else if (implementingType == typeof(float))
                success = reader.TryGetSingle(out float si) && float.IsNormal(si) && (value = si) is { };
            else if (implementingType == typeof(double))
                success = reader.TryGetDouble(out double dbl) && double.IsNormal(dbl) && (value = dbl) is { };
            else
            {
                return unexpectedToken(ref reader, getRaw(ref reader), implementingType.Name, "number");
            }

            // We expected a number, we found a json number, but they don't match (e.g. precision etc)
            if (success)
            {
                return implementingType == typeof(long) && fhirType == typeof(Integer64)
                    ? new(value, new FhirJsonException("JSON123", $"JsonString '{getRaw(ref reader)}' cannot be parsed as a {nameof(Integer64)}, because it should be a Json number."))
                    : new(value, null);
            }
            else
            {
                var rawValue = getRaw(ref reader);
                return new(rawValue, new FhirJsonException("JSON108", $"Json number '{rawValue}' cannot be parsed as a {implementingType.Name}."));
            }
        }

    }
}
