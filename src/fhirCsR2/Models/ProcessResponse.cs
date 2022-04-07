// <auto-generated />
// Built from: hl7.fhir.r2.core version: 1.0.2
  // Option: "NAMESPACE" = "fhirCsR2"

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using fhirCsR2.Serialization;

namespace fhirCsR2.Models
{
  /// <summary>
  /// Suite of processing note or additional requirements is the processing has been held.
  /// </summary>
  [JsonConverter(typeof(fhirCsR2.Serialization.JsonStreamComponentConverter<ProcessResponseNotes>))]
  public class ProcessResponseNotes : BackboneElement,  IFhirJsonSerializable {
    /// <summary>
    /// The note text.
    /// </summary>
    public string Text { get; set; }
    /// <summary>
    /// Extension container element for Text
    /// </summary>
    public Element _Text { get; set; }
    /// <summary>
    /// The note purpose: Print/Display.
    /// </summary>
    public Coding Type { get; set; }
    /// <summary>
    /// Serialize to a JSON object
    /// </summary>
    public new void SerializeJson(Utf8JsonWriter writer, JsonSerializerOptions options, bool includeStartObject = true)
    {
      if (includeStartObject)
      {
        writer.WriteStartObject();
      }
      ((fhirCsR2.Models.BackboneElement)this).SerializeJson(writer, options, false);

      if (Type != null)
      {
        writer.WritePropertyName("type");
        Type.SerializeJson(writer, options);
      }

      if (!string.IsNullOrEmpty(Text))
      {
        writer.WriteString("text", (string)Text!);
      }

      if (_Text != null)
      {
        writer.WritePropertyName("_text");
        _Text.SerializeJson(writer, options);
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
        case "text":
          Text = reader.GetString();
          break;

        case "_text":
          _Text = new fhirCsR2.Models.Element();
          _Text.DeserializeJson(ref reader, options);
          break;

        case "type":
          Type = new fhirCsR2.Models.Coding();
          Type.DeserializeJson(ref reader, options);
          break;

        default:
          ((fhirCsR2.Models.BackboneElement)this).DeserializeJsonProperty(ref reader, options, propertyName);
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
  /// <summary>
  /// This resource provides processing status, errors and notes from the processing of a resource.
  /// </summary>
  [JsonConverter(typeof(fhirCsR2.Serialization.JsonStreamComponentConverter<ProcessResponse>))]
  public class ProcessResponse : DomainResource,  IFhirJsonSerializable {
    /// <summary>
    /// Resource Type Name
    /// </summary>
    public override string ResourceType => "ProcessResponse";
    /// <summary>
    /// The date when the enclosed suite of services were performed or completed.
    /// </summary>
    public string Created { get; set; }
    /// <summary>
    /// Extension container element for Created
    /// </summary>
    public Element _Created { get; set; }
    /// <summary>
    /// A description of the status of the adjudication or processing.
    /// </summary>
    public string Disposition { get; set; }
    /// <summary>
    /// Extension container element for Disposition
    /// </summary>
    public Element _Disposition { get; set; }
    /// <summary>
    /// Processing errors.
    /// </summary>
    public List<Coding> Error { get; set; }
    /// <summary>
    /// The form to be used for printing the content.
    /// </summary>
    public Coding Form { get; set; }
    /// <summary>
    /// The Response business identifier.
    /// </summary>
    public List<Identifier> Identifier { get; set; }
    /// <summary>
    /// Suite of processing note or additional requirements is the processing has been held.
    /// </summary>
    public List<ProcessResponseNotes> Notes { get; set; }
    /// <summary>
    /// The organization who produced this adjudicated response.
    /// </summary>
    public Reference Organization { get; set; }
    /// <summary>
    /// Knowledge of the original version can inform the processing of this instance so that information which is processable by the originating system may be generated.
    /// </summary>
    public Coding OriginalRuleset { get; set; }
    /// <summary>
    /// Transaction status: error, complete, held.
    /// </summary>
    public Coding Outcome { get; set; }
    /// <summary>
    /// Original request resource reference.
    /// </summary>
    public Reference Request { get; set; }
    /// <summary>
    /// The organization which is responsible for the services rendered to the patient.
    /// </summary>
    public Reference RequestOrganization { get; set; }
    /// <summary>
    /// The practitioner who is responsible for the services rendered to the patient.
    /// </summary>
    public Reference RequestProvider { get; set; }
    /// <summary>
    /// The version of the style of resource contents. This should be mapped to the allowable profiles for this and supporting resources.
    /// </summary>
    public Coding Ruleset { get; set; }
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


      ((fhirCsR2.Models.DomainResource)this).SerializeJson(writer, options, false);

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

      if (Request != null)
      {
        writer.WritePropertyName("request");
        Request.SerializeJson(writer, options);
      }

      if (Outcome != null)
      {
        writer.WritePropertyName("outcome");
        Outcome.SerializeJson(writer, options);
      }

      if (!string.IsNullOrEmpty(Disposition))
      {
        writer.WriteString("disposition", (string)Disposition!);
      }

      if (_Disposition != null)
      {
        writer.WritePropertyName("_disposition");
        _Disposition.SerializeJson(writer, options);
      }

      if (Ruleset != null)
      {
        writer.WritePropertyName("ruleset");
        Ruleset.SerializeJson(writer, options);
      }

      if (OriginalRuleset != null)
      {
        writer.WritePropertyName("originalRuleset");
        OriginalRuleset.SerializeJson(writer, options);
      }

      if (!string.IsNullOrEmpty(Created))
      {
        writer.WriteString("created", (string)Created!);
      }

      if (_Created != null)
      {
        writer.WritePropertyName("_created");
        _Created.SerializeJson(writer, options);
      }

      if (Organization != null)
      {
        writer.WritePropertyName("organization");
        Organization.SerializeJson(writer, options);
      }

      if (RequestProvider != null)
      {
        writer.WritePropertyName("requestProvider");
        RequestProvider.SerializeJson(writer, options);
      }

      if (RequestOrganization != null)
      {
        writer.WritePropertyName("requestOrganization");
        RequestOrganization.SerializeJson(writer, options);
      }

      if (Form != null)
      {
        writer.WritePropertyName("form");
        Form.SerializeJson(writer, options);
      }

      if ((Notes != null) && (Notes.Count != 0))
      {
        writer.WritePropertyName("notes");
        writer.WriteStartArray();

        foreach (ProcessResponseNotes valNotes in Notes)
        {
          valNotes.SerializeJson(writer, options, true);
        }

        writer.WriteEndArray();
      }

      if ((Error != null) && (Error.Count != 0))
      {
        writer.WritePropertyName("error");
        writer.WriteStartArray();

        foreach (Coding valError in Error)
        {
          valError.SerializeJson(writer, options, true);
        }

        writer.WriteEndArray();
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
        case "created":
          Created = reader.GetString();
          break;

        case "_created":
          _Created = new fhirCsR2.Models.Element();
          _Created.DeserializeJson(ref reader, options);
          break;

        case "disposition":
          Disposition = reader.GetString();
          break;

        case "_disposition":
          _Disposition = new fhirCsR2.Models.Element();
          _Disposition.DeserializeJson(ref reader, options);
          break;

        case "error":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Error = new List<Coding>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            fhirCsR2.Models.Coding objError = new fhirCsR2.Models.Coding();
            objError.DeserializeJson(ref reader, options);
            Error.Add(objError);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (Error.Count == 0)
          {
            Error = null;
          }

          break;

        case "form":
          Form = new fhirCsR2.Models.Coding();
          Form.DeserializeJson(ref reader, options);
          break;

        case "identifier":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Identifier = new List<Identifier>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            fhirCsR2.Models.Identifier objIdentifier = new fhirCsR2.Models.Identifier();
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

        case "notes":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Notes = new List<ProcessResponseNotes>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            fhirCsR2.Models.ProcessResponseNotes objNotes = new fhirCsR2.Models.ProcessResponseNotes();
            objNotes.DeserializeJson(ref reader, options);
            Notes.Add(objNotes);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (Notes.Count == 0)
          {
            Notes = null;
          }

          break;

        case "organization":
          Organization = new fhirCsR2.Models.Reference();
          Organization.DeserializeJson(ref reader, options);
          break;

        case "originalRuleset":
          OriginalRuleset = new fhirCsR2.Models.Coding();
          OriginalRuleset.DeserializeJson(ref reader, options);
          break;

        case "outcome":
          Outcome = new fhirCsR2.Models.Coding();
          Outcome.DeserializeJson(ref reader, options);
          break;

        case "request":
          Request = new fhirCsR2.Models.Reference();
          Request.DeserializeJson(ref reader, options);
          break;

        case "requestOrganization":
          RequestOrganization = new fhirCsR2.Models.Reference();
          RequestOrganization.DeserializeJson(ref reader, options);
          break;

        case "requestProvider":
          RequestProvider = new fhirCsR2.Models.Reference();
          RequestProvider.DeserializeJson(ref reader, options);
          break;

        case "ruleset":
          Ruleset = new fhirCsR2.Models.Coding();
          Ruleset.DeserializeJson(ref reader, options);
          break;

        default:
          ((fhirCsR2.Models.DomainResource)this).DeserializeJsonProperty(ref reader, options, propertyName);
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