// <auto-generated />
// Built from: hl7.fhir.r4.core version: 4.0.1
  // Option: "NAMESPACE" = "fhirCsR4"

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using fhirCsR4.Serialization;

namespace fhirCsR4.Models
{
  /// <summary>
  /// A relationship of two Quantity values - expressed as a numerator and a denominator.
  /// </summary>
  [JsonConverter(typeof(fhirCsR4.Serialization.JsonStreamComponentConverter<Ratio>))]
  public class Ratio : Element,  IFhirJsonSerializable {
    /// <summary>
    /// The value of the denominator.
    /// </summary>
    public Quantity Denominator { get; set; }
    /// <summary>
    /// The value of the numerator.
    /// </summary>
    public Quantity Numerator { get; set; }
    /// <summary>
    /// Serialize to a JSON object
    /// </summary>
    public new void SerializeJson(Utf8JsonWriter writer, JsonSerializerOptions options, bool includeStartObject = true)
    {
      if (includeStartObject)
      {
        writer.WriteStartObject();
      }
      ((fhirCsR4.Models.Element)this).SerializeJson(writer, options, false);

      if (Numerator != null)
      {
        writer.WritePropertyName("numerator");
        Numerator.SerializeJson(writer, options);
      }

      if (Denominator != null)
      {
        writer.WritePropertyName("denominator");
        Denominator.SerializeJson(writer, options);
      }

      if (includeStartObject)
      {
        writer.WriteEndObject();
      }
    }
    /// <summary>
    /// Deserialize a JSON property
    /// </summary>
    public new void DeserializeJsonProperty(ref Utf8JsonReader reader, JsonSerializerOptions options, string propertyName)
    {
      switch (propertyName)
      {
        case "denominator":
          Denominator = new fhirCsR4.Models.Quantity();
          Denominator.DeserializeJson(ref reader, options);
          break;

        case "numerator":
          Numerator = new fhirCsR4.Models.Quantity();
          Numerator.DeserializeJson(ref reader, options);
          break;

        default:
          ((fhirCsR4.Models.Element)this).DeserializeJsonProperty(ref reader, options, propertyName);
          break;
      }
    }

    /// <summary>
    /// Deserialize a JSON object
    /// </summary>
    public new void DeserializeJson(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
      string propertyName;

      while (reader.Read())
      {
        if (reader.TokenType == JsonTokenType.EndObject)
        {
          return;
        }

        if (reader.TokenType == JsonTokenType.PropertyName)
        {
          propertyName = reader.GetString();
          reader.Read();
          this.DeserializeJsonProperty(ref reader, options, propertyName);
        }
      }

      throw new JsonException();
    }
  }
}
