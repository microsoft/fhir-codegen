// <auto-generated />
// Built from: hl7.fhir.r4.core version: 4.0.1

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Fhir.R4.Serialization;

namespace Fhir.R4.Models
{
  /// <summary>
  /// Vaccine date recommendations.  For example, earliest date to administer, latest date to administer, etc.
  /// </summary>
  [JsonConverter(typeof(Fhir.R4.Serialization.JsonStreamComponentConverter<ImmunizationRecommendationRecommendationDateCriterion>))]
  public class ImmunizationRecommendationRecommendationDateCriterion : BackboneElement,  IFhirJsonSerializable {
    /// <summary>
    /// Date classification of recommendation.  For example, earliest date to give, latest date to give, etc.
    /// </summary>
    public CodeableConcept Code { get; set; }
    /// <summary>
    /// The date whose meaning is specified by dateCriterion.code.
    /// </summary>
    public string Value { get; set; }
    /// <summary>
    /// Extension container element for Value
    /// </summary>
    public Element _Value { get; set; }
    /// <summary>
    /// Serialize to a JSON object
    /// </summary>
    public new void SerializeJson(Utf8JsonWriter writer, JsonSerializerOptions options, bool includeStartObject = true)
    {
      if (includeStartObject)
      {
        writer.WriteStartObject();
      }

      ((Fhir.R4.Models.BackboneElement)this).SerializeJson(writer, options, false);

      if (Code != null)
      {
        writer.WritePropertyName("code");
        Code.SerializeJson(writer, options);
      }

      if (!string.IsNullOrEmpty(Value))
      {
        writer.WriteString("value", (string)Value!);
      }

      if (_Value != null)
      {
        writer.WritePropertyName("_value");
        _Value.SerializeJson(writer, options);
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
          Code = new Fhir.R4.Models.CodeableConcept();
          Code.DeserializeJson(ref reader, options);
          break;

        case "value":
          Value = reader.GetString();
          break;

        case "_value":
          _Value = new Fhir.R4.Models.Element();
          _Value.DeserializeJson(ref reader, options);
          break;

        default:
          ((Fhir.R4.Models.BackboneElement)this).DeserializeJsonProperty(ref reader, options, propertyName);
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
  /// Vaccine administration recommendations.
  /// </summary>
  [JsonConverter(typeof(Fhir.R4.Serialization.JsonStreamComponentConverter<ImmunizationRecommendationRecommendation>))]
  public class ImmunizationRecommendationRecommendation : BackboneElement,  IFhirJsonSerializable {
    /// <summary>
    /// Vaccine(s) which should not be used to fulfill the recommendation.
    /// </summary>
    public List<CodeableConcept> ContraindicatedVaccineCode { get; set; }
    /// <summary>
    /// Vaccine date recommendations.  For example, earliest date to administer, latest date to administer, etc.
    /// </summary>
    public List<ImmunizationRecommendationRecommendationDateCriterion> DateCriterion { get; set; }
    /// <summary>
    /// Contains the description about the protocol under which the vaccine was administered.
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// Extension container element for Description
    /// </summary>
    public Element _Description { get; set; }
    /// <summary>
    /// The use of an integer is prefered if known. A string should only be used in cases where an interger is not available (such as when documenting a recurring booster dose).
    /// </summary>
    public uint? DoseNumberPositiveInt { get; set; }
    /// <summary>
    /// The use of an integer is prefered if known. A string should only be used in cases where an interger is not available (such as when documenting a recurring booster dose).
    /// </summary>
    public string DoseNumberString { get; set; }
    /// <summary>
    /// Extension container element for DoseNumberString
    /// </summary>
    public Element _DoseNumberString { get; set; }
    /// <summary>
    /// The reason for the assigned forecast status.
    /// </summary>
    public List<CodeableConcept> ForecastReason { get; set; }
    /// <summary>
    /// Indicates the patient status with respect to the path to immunity for the target disease.
    /// </summary>
    public CodeableConcept ForecastStatus { get; set; }
    /// <summary>
    /// One possible path to achieve presumed immunity against a disease - within the context of an authority.
    /// </summary>
    public string Series { get; set; }
    /// <summary>
    /// Extension container element for Series
    /// </summary>
    public Element _Series { get; set; }
    /// <summary>
    /// The use of an integer is prefered if known. A string should only be used in cases where an interger is not available (such as when documenting a recurring booster dose).
    /// </summary>
    public uint? SeriesDosesPositiveInt { get; set; }
    /// <summary>
    /// The use of an integer is prefered if known. A string should only be used in cases where an interger is not available (such as when documenting a recurring booster dose).
    /// </summary>
    public string SeriesDosesString { get; set; }
    /// <summary>
    /// Extension container element for SeriesDosesString
    /// </summary>
    public Element _SeriesDosesString { get; set; }
    /// <summary>
    /// Immunization event history and/or evaluation that supports the status and recommendation.
    /// </summary>
    public List<Reference> SupportingImmunization { get; set; }
    /// <summary>
    /// Patient Information that supports the status and recommendation.  This includes patient observations, adverse reactions and allergy/intolerance information.
    /// </summary>
    public List<Reference> SupportingPatientInformation { get; set; }
    /// <summary>
    /// The targeted disease for the recommendation.
    /// </summary>
    public CodeableConcept TargetDisease { get; set; }
    /// <summary>
    /// Vaccine(s) or vaccine group that pertain to the recommendation.
    /// </summary>
    public List<CodeableConcept> VaccineCode { get; set; }
    /// <summary>
    /// Serialize to a JSON object
    /// </summary>
    public new void SerializeJson(Utf8JsonWriter writer, JsonSerializerOptions options, bool includeStartObject = true)
    {
      if (includeStartObject)
      {
        writer.WriteStartObject();
      }

      ((Fhir.R4.Models.BackboneElement)this).SerializeJson(writer, options, false);

      if ((VaccineCode != null) && (VaccineCode.Count != 0))
      {
        writer.WritePropertyName("vaccineCode");
        writer.WriteStartArray();

        foreach (CodeableConcept valVaccineCode in VaccineCode)
        {
          valVaccineCode.SerializeJson(writer, options, true);
        }

        writer.WriteEndArray();
      }

      if (TargetDisease != null)
      {
        writer.WritePropertyName("targetDisease");
        TargetDisease.SerializeJson(writer, options);
      }

      if ((ContraindicatedVaccineCode != null) && (ContraindicatedVaccineCode.Count != 0))
      {
        writer.WritePropertyName("contraindicatedVaccineCode");
        writer.WriteStartArray();

        foreach (CodeableConcept valContraindicatedVaccineCode in ContraindicatedVaccineCode)
        {
          valContraindicatedVaccineCode.SerializeJson(writer, options, true);
        }

        writer.WriteEndArray();
      }

      if (ForecastStatus != null)
      {
        writer.WritePropertyName("forecastStatus");
        ForecastStatus.SerializeJson(writer, options);
      }

      if ((ForecastReason != null) && (ForecastReason.Count != 0))
      {
        writer.WritePropertyName("forecastReason");
        writer.WriteStartArray();

        foreach (CodeableConcept valForecastReason in ForecastReason)
        {
          valForecastReason.SerializeJson(writer, options, true);
        }

        writer.WriteEndArray();
      }

      if ((DateCriterion != null) && (DateCriterion.Count != 0))
      {
        writer.WritePropertyName("dateCriterion");
        writer.WriteStartArray();

        foreach (ImmunizationRecommendationRecommendationDateCriterion valDateCriterion in DateCriterion)
        {
          valDateCriterion.SerializeJson(writer, options, true);
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

      if (!string.IsNullOrEmpty(Series))
      {
        writer.WriteString("series", (string)Series!);
      }

      if (_Series != null)
      {
        writer.WritePropertyName("_series");
        _Series.SerializeJson(writer, options);
      }

      if (DoseNumberPositiveInt != null)
      {
        writer.WriteNumber("doseNumberPositiveInt", (uint)DoseNumberPositiveInt!);
      }

      if (!string.IsNullOrEmpty(DoseNumberString))
      {
        writer.WriteString("doseNumberString", (string)DoseNumberString!);
      }

      if (_DoseNumberString != null)
      {
        writer.WritePropertyName("_doseNumberString");
        _DoseNumberString.SerializeJson(writer, options);
      }

      if (SeriesDosesPositiveInt != null)
      {
        writer.WriteNumber("seriesDosesPositiveInt", (uint)SeriesDosesPositiveInt!);
      }

      if (!string.IsNullOrEmpty(SeriesDosesString))
      {
        writer.WriteString("seriesDosesString", (string)SeriesDosesString!);
      }

      if (_SeriesDosesString != null)
      {
        writer.WritePropertyName("_seriesDosesString");
        _SeriesDosesString.SerializeJson(writer, options);
      }

      if ((SupportingImmunization != null) && (SupportingImmunization.Count != 0))
      {
        writer.WritePropertyName("supportingImmunization");
        writer.WriteStartArray();

        foreach (Reference valSupportingImmunization in SupportingImmunization)
        {
          valSupportingImmunization.SerializeJson(writer, options, true);
        }

        writer.WriteEndArray();
      }

      if ((SupportingPatientInformation != null) && (SupportingPatientInformation.Count != 0))
      {
        writer.WritePropertyName("supportingPatientInformation");
        writer.WriteStartArray();

        foreach (Reference valSupportingPatientInformation in SupportingPatientInformation)
        {
          valSupportingPatientInformation.SerializeJson(writer, options, true);
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
        case "contraindicatedVaccineCode":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          ContraindicatedVaccineCode = new List<CodeableConcept>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Fhir.R4.Models.CodeableConcept objContraindicatedVaccineCode = new Fhir.R4.Models.CodeableConcept();
            objContraindicatedVaccineCode.DeserializeJson(ref reader, options);
            ContraindicatedVaccineCode.Add(objContraindicatedVaccineCode);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (ContraindicatedVaccineCode.Count == 0)
          {
            ContraindicatedVaccineCode = null;
          }

          break;

        case "dateCriterion":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          DateCriterion = new List<ImmunizationRecommendationRecommendationDateCriterion>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Fhir.R4.Models.ImmunizationRecommendationRecommendationDateCriterion objDateCriterion = new Fhir.R4.Models.ImmunizationRecommendationRecommendationDateCriterion();
            objDateCriterion.DeserializeJson(ref reader, options);
            DateCriterion.Add(objDateCriterion);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (DateCriterion.Count == 0)
          {
            DateCriterion = null;
          }

          break;

        case "description":
          Description = reader.GetString();
          break;

        case "_description":
          _Description = new Fhir.R4.Models.Element();
          _Description.DeserializeJson(ref reader, options);
          break;

        case "doseNumberPositiveInt":
          DoseNumberPositiveInt = reader.GetUInt32();
          break;

        case "doseNumberString":
          DoseNumberString = reader.GetString();
          break;

        case "_doseNumberString":
          _DoseNumberString = new Fhir.R4.Models.Element();
          _DoseNumberString.DeserializeJson(ref reader, options);
          break;

        case "forecastReason":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          ForecastReason = new List<CodeableConcept>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Fhir.R4.Models.CodeableConcept objForecastReason = new Fhir.R4.Models.CodeableConcept();
            objForecastReason.DeserializeJson(ref reader, options);
            ForecastReason.Add(objForecastReason);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (ForecastReason.Count == 0)
          {
            ForecastReason = null;
          }

          break;

        case "forecastStatus":
          ForecastStatus = new Fhir.R4.Models.CodeableConcept();
          ForecastStatus.DeserializeJson(ref reader, options);
          break;

        case "series":
          Series = reader.GetString();
          break;

        case "_series":
          _Series = new Fhir.R4.Models.Element();
          _Series.DeserializeJson(ref reader, options);
          break;

        case "seriesDosesPositiveInt":
          SeriesDosesPositiveInt = reader.GetUInt32();
          break;

        case "seriesDosesString":
          SeriesDosesString = reader.GetString();
          break;

        case "_seriesDosesString":
          _SeriesDosesString = new Fhir.R4.Models.Element();
          _SeriesDosesString.DeserializeJson(ref reader, options);
          break;

        case "supportingImmunization":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          SupportingImmunization = new List<Reference>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Fhir.R4.Models.Reference objSupportingImmunization = new Fhir.R4.Models.Reference();
            objSupportingImmunization.DeserializeJson(ref reader, options);
            SupportingImmunization.Add(objSupportingImmunization);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (SupportingImmunization.Count == 0)
          {
            SupportingImmunization = null;
          }

          break;

        case "supportingPatientInformation":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          SupportingPatientInformation = new List<Reference>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Fhir.R4.Models.Reference objSupportingPatientInformation = new Fhir.R4.Models.Reference();
            objSupportingPatientInformation.DeserializeJson(ref reader, options);
            SupportingPatientInformation.Add(objSupportingPatientInformation);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (SupportingPatientInformation.Count == 0)
          {
            SupportingPatientInformation = null;
          }

          break;

        case "targetDisease":
          TargetDisease = new Fhir.R4.Models.CodeableConcept();
          TargetDisease.DeserializeJson(ref reader, options);
          break;

        case "vaccineCode":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          VaccineCode = new List<CodeableConcept>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Fhir.R4.Models.CodeableConcept objVaccineCode = new Fhir.R4.Models.CodeableConcept();
            objVaccineCode.DeserializeJson(ref reader, options);
            VaccineCode.Add(objVaccineCode);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (VaccineCode.Count == 0)
          {
            VaccineCode = null;
          }

          break;

        default:
          ((Fhir.R4.Models.BackboneElement)this).DeserializeJsonProperty(ref reader, options, propertyName);
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
  /// A patient's point-in-time set of recommendations (i.e. forecasting) according to a published schedule with optional supporting justification.
  /// </summary>
  [JsonConverter(typeof(Fhir.R4.Serialization.JsonStreamComponentConverter<ImmunizationRecommendation>))]
  public class ImmunizationRecommendation : DomainResource,  IFhirJsonSerializable {
    /// <summary>
    /// Resource Type Name
    /// </summary>
    public string ResourceType => "ImmunizationRecommendation";
    /// <summary>
    /// Indicates the authority who published the protocol (e.g. ACIP).
    /// </summary>
    public Reference Authority { get; set; }
    /// <summary>
    /// The date the immunization recommendation(s) were created.
    /// </summary>
    public string Date { get; set; }
    /// <summary>
    /// Extension container element for Date
    /// </summary>
    public Element _Date { get; set; }
    /// <summary>
    /// A unique identifier assigned to this particular recommendation record.
    /// </summary>
    public List<Identifier> Identifier { get; set; }
    /// <summary>
    /// The patient the recommendation(s) are for.
    /// </summary>
    public Reference Patient { get; set; }
    /// <summary>
    /// Vaccine administration recommendations.
    /// </summary>
    public List<ImmunizationRecommendationRecommendation> Recommendation { get; set; }
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


      ((Fhir.R4.Models.DomainResource)this).SerializeJson(writer, options, false);

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

      if (Patient != null)
      {
        writer.WritePropertyName("patient");
        Patient.SerializeJson(writer, options);
      }

      if (!string.IsNullOrEmpty(Date))
      {
        writer.WriteString("date", (string)Date!);
      }

      if (_Date != null)
      {
        writer.WritePropertyName("_date");
        _Date.SerializeJson(writer, options);
      }

      if (Authority != null)
      {
        writer.WritePropertyName("authority");
        Authority.SerializeJson(writer, options);
      }

      if ((Recommendation != null) && (Recommendation.Count != 0))
      {
        writer.WritePropertyName("recommendation");
        writer.WriteStartArray();

        foreach (ImmunizationRecommendationRecommendation valRecommendation in Recommendation)
        {
          valRecommendation.SerializeJson(writer, options, true);
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
        case "authority":
          Authority = new Fhir.R4.Models.Reference();
          Authority.DeserializeJson(ref reader, options);
          break;

        case "date":
          Date = reader.GetString();
          break;

        case "_date":
          _Date = new Fhir.R4.Models.Element();
          _Date.DeserializeJson(ref reader, options);
          break;

        case "identifier":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Identifier = new List<Identifier>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Fhir.R4.Models.Identifier objIdentifier = new Fhir.R4.Models.Identifier();
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

        case "patient":
          Patient = new Fhir.R4.Models.Reference();
          Patient.DeserializeJson(ref reader, options);
          break;

        case "recommendation":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Recommendation = new List<ImmunizationRecommendationRecommendation>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Fhir.R4.Models.ImmunizationRecommendationRecommendation objRecommendation = new Fhir.R4.Models.ImmunizationRecommendationRecommendation();
            objRecommendation.DeserializeJson(ref reader, options);
            Recommendation.Add(objRecommendation);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (Recommendation.Count == 0)
          {
            Recommendation = null;
          }

          break;

        default:
          ((Fhir.R4.Models.DomainResource)this).DeserializeJsonProperty(ref reader, options, propertyName);
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
