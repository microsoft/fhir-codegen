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
  /// An error, warning or information message that results from a system action.
  /// </summary>
  [JsonConverter(typeof(fhirCsR2.Serialization.JsonStreamComponentConverter<OperationOutcomeIssue>))]
  public class OperationOutcomeIssue : BackboneElement,  IFhirJsonSerializable {
    /// <summary>
    /// Expresses the issue in a human and computer-friendly way, allowing the requesting system to behave differently based on the type of issue.
    /// </summary>
    public string Code { get; set; }
    /// <summary>
    /// Extension container element for Code
    /// </summary>
    public Element _Code { get; set; }
    /// <summary>
    /// Additional details about the error. This may be a text description of the error, or a system code that identifies the error.
    /// </summary>
    public CodeableConcept Details { get; set; }
    /// <summary>
    /// Additional diagnostic information about the issue.  Typically, this may be a description of how a value is erroneous, or a stack dump to help trace the issue.
    /// </summary>
    public string Diagnostics { get; set; }
    /// <summary>
    /// Extension container element for Diagnostics
    /// </summary>
    public Element _Diagnostics { get; set; }
    /// <summary>
    /// Allows systems to highlight or otherwise guide users to elements implicated in issues to allow them to be fixed more easily.
    /// </summary>
    public List<string> Location { get; set; }
    /// <summary>
    /// Extension container element for Location
    /// </summary>
    public List<Element> _Location { get; set; }
    /// <summary>
    /// Indicates how relevant the issue is to the overall success of the action.
    /// </summary>
    public string Severity { get; set; }
    /// <summary>
    /// Extension container element for Severity
    /// </summary>
    public Element _Severity { get; set; }
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

      if (!string.IsNullOrEmpty(Severity))
      {
        writer.WriteString("severity", (string)Severity!);
      }

      if (_Severity != null)
      {
        writer.WritePropertyName("_severity");
        _Severity.SerializeJson(writer, options);
      }

      if (!string.IsNullOrEmpty(Code))
      {
        writer.WriteString("code", (string)Code!);
      }

      if (_Code != null)
      {
        writer.WritePropertyName("_code");
        _Code.SerializeJson(writer, options);
      }

      if (Details != null)
      {
        writer.WritePropertyName("details");
        Details.SerializeJson(writer, options);
      }

      if (!string.IsNullOrEmpty(Diagnostics))
      {
        writer.WriteString("diagnostics", (string)Diagnostics!);
      }

      if (_Diagnostics != null)
      {
        writer.WritePropertyName("_diagnostics");
        _Diagnostics.SerializeJson(writer, options);
      }

      if ((Location != null) && (Location.Count != 0))
      {
        writer.WritePropertyName("location");
        writer.WriteStartArray();

        foreach (string valLocation in Location)
        {
          writer.WriteStringValue(valLocation);
        }

        writer.WriteEndArray();
      }

      if ((_Location != null) && (_Location.Count != 0))
      {
        writer.WritePropertyName("_location");
        writer.WriteStartArray();

        foreach (Element val_Location in _Location)
        {
          val_Location.SerializeJson(writer, options, true);
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
        case "code":
          Code = reader.GetString();
          break;

        case "_code":
          _Code = new fhirCsR2.Models.Element();
          _Code.DeserializeJson(ref reader, options);
          break;

        case "details":
          Details = new fhirCsR2.Models.CodeableConcept();
          Details.DeserializeJson(ref reader, options);
          break;

        case "diagnostics":
          Diagnostics = reader.GetString();
          break;

        case "_diagnostics":
          _Diagnostics = new fhirCsR2.Models.Element();
          _Diagnostics.DeserializeJson(ref reader, options);
          break;

        case "location":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Location = new List<string>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Location.Add(reader.GetString());

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (Location.Count == 0)
          {
            Location = null;
          }

          break;

        case "_location":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          _Location = new List<Element>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            fhirCsR2.Models.Element obj_Location = new fhirCsR2.Models.Element();
            obj_Location.DeserializeJson(ref reader, options);
            _Location.Add(obj_Location);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (_Location.Count == 0)
          {
            _Location = null;
          }

          break;

        case "severity":
          Severity = reader.GetString();
          break;

        case "_severity":
          _Severity = new fhirCsR2.Models.Element();
          _Severity.DeserializeJson(ref reader, options);
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
  /// Code Values for the OperationOutcome.issue.severity field
  /// </summary>
  public static class OperationOutcomeIssueSeverityCodes {
    public const string FATAL = "fatal";
    public const string ERROR = "error";
    public const string WARNING = "warning";
    public const string INFORMATION = "information";
    public static HashSet<string> Values = new HashSet<string>() {
      "fatal",
      "error",
      "warning",
      "information",
    };
  }
  /// <summary>
  /// A collection of error, warning or information messages that result from a system action.
  /// </summary>
  [JsonConverter(typeof(fhirCsR2.Serialization.JsonStreamComponentConverter<OperationOutcome>))]
  public class OperationOutcome : DomainResource,  IFhirJsonSerializable {
    /// <summary>
    /// Resource Type Name
    /// </summary>
    public override string ResourceType => "OperationOutcome";
    /// <summary>
    /// An error, warning or information message that results from a system action.
    /// </summary>
    public List<OperationOutcomeIssue> Issue { get; set; }
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

      if ((Issue != null) && (Issue.Count != 0))
      {
        writer.WritePropertyName("issue");
        writer.WriteStartArray();

        foreach (OperationOutcomeIssue valIssue in Issue)
        {
          valIssue.SerializeJson(writer, options, true);
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
        case "issue":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Issue = new List<OperationOutcomeIssue>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            fhirCsR2.Models.OperationOutcomeIssue objIssue = new fhirCsR2.Models.OperationOutcomeIssue();
            objIssue.DeserializeJson(ref reader, options);
            Issue.Add(objIssue);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (Issue.Count == 0)
          {
            Issue = null;
          }

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