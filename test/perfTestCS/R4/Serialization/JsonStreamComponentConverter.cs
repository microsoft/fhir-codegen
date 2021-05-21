// <auto-generated />
// Built from: hl7.fhir.r4.core version: 4.0.1

using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Fhir.R4.Models;

namespace Fhir.R4.Serialization
{
  /// <summary>
  /// Common converter to support deserialization of non-resource FHIR components.
  /// </summary>
  public class JsonStreamComponentConverter<T> : JsonConverter<T>
    where T : IFhirJsonSerializable, new()
  {
    /// <summary>
    /// Determines whether the specified type can be converted.
    /// </summary>
    public override bool CanConvert(Type objectType) =>
      typeof(T).IsAssignableFrom(objectType);

    /// <summary>
    /// Writes a specified value as JSON.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, T component, JsonSerializerOptions options)
    {
      component.SerializeJson(writer, options, true);
      writer.Flush();
    }
    /// <summary>
    /// Reads and converts the JSON to a typed object.
    /// </summary>
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
      return ComponentRead(ref reader, typeToConvert, options);
    }
    /// <summary>
    /// Read override to handle reading of components (to allow for open reader).
    /// </summary>
    public static T ComponentRead(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
      if (reader.TokenType != JsonTokenType.StartObject)
      {
        throw new JsonException();
      }

      IFhirJsonSerializable target = new T();
      target.DeserializeJson(ref reader, options);

      return (T)target;
    }
  }
}
