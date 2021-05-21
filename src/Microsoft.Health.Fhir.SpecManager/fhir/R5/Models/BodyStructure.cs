// <auto-generated />
// Built from: hl7.fhir.r5.core version: 4.6.0
  // Option: "NAMESPACE" = "fhirCsR5"

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using fhirCsR5.Serialization;

namespace fhirCsR5.Models
{
  /// <summary>
  /// Record details about an anatomical structure.  This resource may be used when a coded concept does not provide the necessary detail needed for the use case.
  /// </summary>
  [JsonConverter(typeof(fhirCsR5.Serialization.JsonStreamComponentConverter<BodyStructure>))]
  public class BodyStructure : DomainResource,  IFhirJsonSerializable {
    /// <summary>
    /// Resource Type Name
    /// </summary>
    public string ResourceType => "BodyStructure";
    /// <summary>
    /// This element is labeled as a modifier because it may be used to mark that the resource was created in error.
    /// </summary>
    public bool? Active { get; set; }
    /// <summary>
    /// This description could include any visual markings used to orientate the viewer e.g. external reference points, special sutures, ink markings.
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// Extension container element for Description
    /// </summary>
    public Element _Description { get; set; }
    /// <summary>
    /// Identifier for this instance of the anatomical structure.
    /// </summary>
    public List<Identifier> Identifier { get; set; }
    /// <summary>
    /// Image or images used to identify a location.
    /// </summary>
    public List<Attachment> Image { get; set; }
    /// <summary>
    /// The anatomical location or region of the specimen, lesion, or body structure.
    /// </summary>
    public CodeableConcept Location { get; set; }
    /// <summary>
    /// Qualifier to refine the anatomical location.  These include qualifiers for laterality, relative location, directionality, number, and plane.
    /// </summary>
    public List<CodeableConcept> LocationQualifier { get; set; }
    /// <summary>
    /// The minimum cardinality of 0 supports the use case of specifying a location without defining a morphology.
    /// </summary>
    public CodeableConcept Morphology { get; set; }
    /// <summary>
    /// The person to which the body site belongs.
    /// </summary>
    public Reference Patient { get; set; }
    /// <summary>
    /// Serialize to a JSON object
    /// </summary>
    public new void SerializeJson(Utf8JsonWriter writer, JsonSerializerOptions options, bool includeStartObject = true)
    {
      if (includeStartObject)
      {
        writer.WriteStartObject();
      }
      if (!string.IsNullOrEmpty(ResourceType))
      {
        writer.WriteString("resourceType", (string)ResourceType!);
      }


      ((fhirCsR5.Models.DomainResource)this).SerializeJson(writer, options, false);

      if ((Identifier != null) && (Identifier.Count != 0))
      {
        writer.WritePropertyName("identifier");
        writer.WriteStartArray();

        foreach (Identifier valIdentifier in Identifier)
        {
          valIdentifier.SerializeJson(writer, options, true);
        }

        writer.WriteEndArray();
      }

      if (Active != null)
      {
        writer.WriteBoolean("active", (bool)Active!);
      }

      if (Morphology != null)
      {
        writer.WritePropertyName("morphology");
        Morphology.SerializeJson(writer, options);
      }

      if (Location != null)
      {
        writer.WritePropertyName("location");
        Location.SerializeJson(writer, options);
      }

      if ((LocationQualifier != null) && (LocationQualifier.Count != 0))
      {
        writer.WritePropertyName("locationQualifier");
        writer.WriteStartArray();

        foreach (CodeableConcept valLocationQualifier in LocationQualifier)
        {
          valLocationQualifier.SerializeJson(writer, options, true);
        }

        writer.WriteEndArray();
      }

      if (!string.IsNullOrEmpty(Description))
      {
        writer.WriteString("description", (string)Description!);
      }

      if (_Description != null)
      {
        writer.WritePropertyName("_description");
        _Description.SerializeJson(writer, options);
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

      if (Patient != null)
      {
        writer.WritePropertyName("patient");
        Patient.SerializeJson(writer, options);
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
        case "active":
          Active = reader.GetBoolean();
          break;

        case "description":
          Description = reader.GetString();
          break;

        case "_description":
          _Description = new fhirCsR5.Models.Element();
          _Description.DeserializeJson(ref reader, options);
          break;

        case "identifier":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Identifier = new List<Identifier>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            fhirCsR5.Models.Identifier objIdentifier = new fhirCsR5.Models.Identifier();
            objIdentifier.DeserializeJson(ref reader, options);
            Identifier.Add(objIdentifier);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (Identifier.Count == 0)
          {
            Identifier = null;
          }

          break;

        case "image":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Image = new List<Attachment>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            fhirCsR5.Models.Attachment objImage = new fhirCsR5.Models.Attachment();
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

        case "location":
          Location = new fhirCsR5.Models.CodeableConcept();
          Location.DeserializeJson(ref reader, options);
          break;

        case "locationQualifier":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          LocationQualifier = new List<CodeableConcept>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            fhirCsR5.Models.CodeableConcept objLocationQualifier = new fhirCsR5.Models.CodeableConcept();
            objLocationQualifier.DeserializeJson(ref reader, options);
            LocationQualifier.Add(objLocationQualifier);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (LocationQualifier.Count == 0)
          {
            LocationQualifier = null;
          }

          break;

        case "morphology":
          Morphology = new fhirCsR5.Models.CodeableConcept();
          Morphology.DeserializeJson(ref reader, options);
          break;

        case "patient":
          Patient = new fhirCsR5.Models.Reference();
          Patient.DeserializeJson(ref reader, options);
          break;

        default:
          ((fhirCsR5.Models.DomainResource)this).DeserializeJsonProperty(ref reader, options, propertyName);
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
