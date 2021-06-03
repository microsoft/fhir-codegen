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
  /// The marketing status describes the date when a medicinal product is actually put on the market or the date as of which it is no longer available.
  /// </summary>
  [JsonConverter(typeof(fhirCsR4.Serialization.JsonStreamComponentConverter<ProdCharacteristic>))]
  public class ProdCharacteristic : BackboneElement,  IFhirJsonSerializable {
    /// <summary>
    /// Where applicable, the color can be specified An appropriate controlled vocabulary shall be used The term and the term identifier shall be used.
    /// </summary>
    public List<string> Color { get; set; }
    /// <summary>
    /// Extension container element for Color
    /// </summary>
    public List<Element> _Color { get; set; }
    /// <summary>
    /// Where applicable, the depth can be specified using a numerical value and its unit of measurement The unit of measurement shall be specified in accordance with ISO 11240 and the resulting terminology The symbol and the symbol identifier shall be used.
    /// </summary>
    public Quantity Depth { get; set; }
    /// <summary>
    /// Where applicable, the external diameter can be specified using a numerical value and its unit of measurement The unit of measurement shall be specified in accordance with ISO 11240 and the resulting terminology The symbol and the symbol identifier shall be used.
    /// </summary>
    public Quantity ExternalDiameter { get; set; }
    /// <summary>
    /// Where applicable, the height can be specified using a numerical value and its unit of measurement The unit of measurement shall be specified in accordance with ISO 11240 and the resulting terminology The symbol and the symbol identifier shall be used.
    /// </summary>
    public Quantity Height { get; set; }
    /// <summary>
    /// Where applicable, the image can be provided The format of the image attachment shall be specified by regional implementations.
    /// </summary>
    public List<Attachment> Image { get; set; }
    /// <summary>
    /// Where applicable, the imprint can be specified as text.
    /// </summary>
    public List<string> Imprint { get; set; }
    /// <summary>
    /// Extension container element for Imprint
    /// </summary>
    public List<Element> _Imprint { get; set; }
    /// <summary>
    /// Where applicable, the nominal volume can be specified using a numerical value and its unit of measurement The unit of measurement shall be specified in accordance with ISO 11240 and the resulting terminology The symbol and the symbol identifier shall be used.
    /// </summary>
    public Quantity NominalVolume { get; set; }
    /// <summary>
    /// Where applicable, the scoring can be specified An appropriate controlled vocabulary shall be used The term and the term identifier shall be used.
    /// </summary>
    public CodeableConcept Scoring { get; set; }
    /// <summary>
    /// Where applicable, the shape can be specified An appropriate controlled vocabulary shall be used The term and the term identifier shall be used.
    /// </summary>
    public string Shape { get; set; }
    /// <summary>
    /// Extension container element for Shape
    /// </summary>
    public Element _Shape { get; set; }
    /// <summary>
    /// Where applicable, the weight can be specified using a numerical value and its unit of measurement The unit of measurement shall be specified in accordance with ISO 11240 and the resulting terminology The symbol and the symbol identifier shall be used.
    /// </summary>
    public Quantity Weight { get; set; }
    /// <summary>
    /// Where applicable, the width can be specified using a numerical value and its unit of measurement The unit of measurement shall be specified in accordance with ISO 11240 and the resulting terminology The symbol and the symbol identifier shall be used.
    /// </summary>
    public Quantity Width { get; set; }
    /// <summary>
    /// Serialize to a JSON object
    /// </summary>
    public new void SerializeJson(Utf8JsonWriter writer, JsonSerializerOptions options, bool includeStartObject = true)
    {
      if (includeStartObject)
      {
        writer.WriteStartObject();
      }
      ((fhirCsR4.Models.BackboneElement)this).SerializeJson(writer, options, false);

      if (Height != null)
      {
        writer.WritePropertyName("height");
        Height.SerializeJson(writer, options);
      }

      if (Width != null)
      {
        writer.WritePropertyName("width");
        Width.SerializeJson(writer, options);
      }

      if (Depth != null)
      {
        writer.WritePropertyName("depth");
        Depth.SerializeJson(writer, options);
      }

      if (Weight != null)
      {
        writer.WritePropertyName("weight");
        Weight.SerializeJson(writer, options);
      }

      if (NominalVolume != null)
      {
        writer.WritePropertyName("nominalVolume");
        NominalVolume.SerializeJson(writer, options);
      }

      if (ExternalDiameter != null)
      {
        writer.WritePropertyName("externalDiameter");
        ExternalDiameter.SerializeJson(writer, options);
      }

      if (!string.IsNullOrEmpty(Shape))
      {
        writer.WriteString("shape", (string)Shape!);
      }

      if (_Shape != null)
      {
        writer.WritePropertyName("_shape");
        _Shape.SerializeJson(writer, options);
      }

      if ((Color != null) && (Color.Count != 0))
      {
        writer.WritePropertyName("color");
        writer.WriteStartArray();

        foreach (string valColor in Color)
        {
          writer.WriteStringValue(valColor);
        }

        writer.WriteEndArray();
      }

      if ((_Color != null) && (_Color.Count != 0))
      {
        writer.WritePropertyName("_color");
        writer.WriteStartArray();

        foreach (Element val_Color in _Color)
        {
          val_Color.SerializeJson(writer, options, true);
        }

        writer.WriteEndArray();
      }

      if ((Imprint != null) && (Imprint.Count != 0))
      {
        writer.WritePropertyName("imprint");
        writer.WriteStartArray();

        foreach (string valImprint in Imprint)
        {
          writer.WriteStringValue(valImprint);
        }

        writer.WriteEndArray();
      }

      if ((_Imprint != null) && (_Imprint.Count != 0))
      {
        writer.WritePropertyName("_imprint");
        writer.WriteStartArray();

        foreach (Element val_Imprint in _Imprint)
        {
          val_Imprint.SerializeJson(writer, options, true);
        }

        writer.WriteEndArray();
      }

      if ((Image != null) && (Image.Count != 0))
      {
        writer.WritePropertyName("image");
        writer.WriteStartArray();

        foreach (Attachment valImage in Image)
        {
          valImage.SerializeJson(writer, options, true);
        }

        writer.WriteEndArray();
      }

      if (Scoring != null)
      {
        writer.WritePropertyName("scoring");
        Scoring.SerializeJson(writer, options);
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
        case "color":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Color = new List<string>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Color.Add(reader.GetString());

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (Color.Count == 0)
          {
            Color = null;
          }

          break;

        case "_color":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          _Color = new List<Element>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            fhirCsR4.Models.Element obj_Color = new fhirCsR4.Models.Element();
            obj_Color.DeserializeJson(ref reader, options);
            _Color.Add(obj_Color);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (_Color.Count == 0)
          {
            _Color = null;
          }

          break;

        case "depth":
          Depth = new fhirCsR4.Models.Quantity();
          Depth.DeserializeJson(ref reader, options);
          break;

        case "externalDiameter":
          ExternalDiameter = new fhirCsR4.Models.Quantity();
          ExternalDiameter.DeserializeJson(ref reader, options);
          break;

        case "height":
          Height = new fhirCsR4.Models.Quantity();
          Height.DeserializeJson(ref reader, options);
          break;

        case "image":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Image = new List<Attachment>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            fhirCsR4.Models.Attachment objImage = new fhirCsR4.Models.Attachment();
            objImage.DeserializeJson(ref reader, options);
            Image.Add(objImage);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (Image.Count == 0)
          {
            Image = null;
          }

          break;

        case "imprint":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Imprint = new List<string>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Imprint.Add(reader.GetString());

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (Imprint.Count == 0)
          {
            Imprint = null;
          }

          break;

        case "_imprint":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          _Imprint = new List<Element>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            fhirCsR4.Models.Element obj_Imprint = new fhirCsR4.Models.Element();
            obj_Imprint.DeserializeJson(ref reader, options);
            _Imprint.Add(obj_Imprint);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (_Imprint.Count == 0)
          {
            _Imprint = null;
          }

          break;

        case "nominalVolume":
          NominalVolume = new fhirCsR4.Models.Quantity();
          NominalVolume.DeserializeJson(ref reader, options);
          break;

        case "scoring":
          Scoring = new fhirCsR4.Models.CodeableConcept();
          Scoring.DeserializeJson(ref reader, options);
          break;

        case "shape":
          Shape = reader.GetString();
          break;

        case "_shape":
          _Shape = new fhirCsR4.Models.Element();
          _Shape.DeserializeJson(ref reader, options);
          break;

        case "weight":
          Weight = new fhirCsR4.Models.Quantity();
          Weight.DeserializeJson(ref reader, options);
          break;

        case "width":
          Width = new fhirCsR4.Models.Quantity();
          Width.DeserializeJson(ref reader, options);
          break;

        default:
          ((fhirCsR4.Models.BackboneElement)this).DeserializeJsonProperty(ref reader, options, propertyName);
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
