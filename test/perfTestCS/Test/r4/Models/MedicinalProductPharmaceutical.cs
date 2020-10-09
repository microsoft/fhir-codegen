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
  /// Characteristics e.g. a products onset of action.
  /// </summary>
  [JsonConverter(typeof(Fhir.R4.Serialization.JsonStreamComponentConverter<MedicinalProductPharmaceuticalCharacteristics>))]
  public class MedicinalProductPharmaceuticalCharacteristics : BackboneElement,  IFhirJsonSerializable {
    /// <summary>
    /// A coded characteristic.
    /// </summary>
    public CodeableConcept Code { get; set; }
    /// <summary>
    /// The status of characteristic e.g. assigned or pending.
    /// </summary>
    public CodeableConcept Status { get; set; }
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

      if (Status != null)
      {
        writer.WritePropertyName("status");
        Status.SerializeJson(writer, options);
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

        case "status":
          Status = new Fhir.R4.Models.CodeableConcept();
          Status.DeserializeJson(ref reader, options);
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
  /// A species specific time during which consumption of animal product is not appropriate.
  /// </summary>
  [JsonConverter(typeof(Fhir.R4.Serialization.JsonStreamComponentConverter<MedicinalProductPharmaceuticalRouteOfAdministrationTargetSpeciesWithdrawalPeriod>))]
  public class MedicinalProductPharmaceuticalRouteOfAdministrationTargetSpeciesWithdrawalPeriod : BackboneElement,  IFhirJsonSerializable {
    /// <summary>
    /// Extra information about the withdrawal period.
    /// </summary>
    public string SupportingInformation { get; set; }
    /// <summary>
    /// Extension container element for SupportingInformation
    /// </summary>
    public Element _SupportingInformation { get; set; }
    /// <summary>
    /// Coded expression for the type of tissue for which the withdrawal period applues, e.g. meat, milk.
    /// </summary>
    public CodeableConcept Tissue { get; set; }
    /// <summary>
    /// A value for the time.
    /// </summary>
    public Quantity Value { get; set; }
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

      if (Tissue != null)
      {
        writer.WritePropertyName("tissue");
        Tissue.SerializeJson(writer, options);
      }

      if (Value != null)
      {
        writer.WritePropertyName("value");
        Value.SerializeJson(writer, options);
      }

      if (!string.IsNullOrEmpty(SupportingInformation))
      {
        writer.WriteString("supportingInformation", (string)SupportingInformation!);
      }

      if (_SupportingInformation != null)
      {
        writer.WritePropertyName("_supportingInformation");
        _SupportingInformation.SerializeJson(writer, options);
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
        case "supportingInformation":
          SupportingInformation = reader.GetString();
          break;

        case "_supportingInformation":
          _SupportingInformation = new Fhir.R4.Models.Element();
          _SupportingInformation.DeserializeJson(ref reader, options);
          break;

        case "tissue":
          Tissue = new Fhir.R4.Models.CodeableConcept();
          Tissue.DeserializeJson(ref reader, options);
          break;

        case "value":
          Value = new Fhir.R4.Models.Quantity();
          Value.DeserializeJson(ref reader, options);
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
  /// A species for which this route applies.
  /// </summary>
  [JsonConverter(typeof(Fhir.R4.Serialization.JsonStreamComponentConverter<MedicinalProductPharmaceuticalRouteOfAdministrationTargetSpecies>))]
  public class MedicinalProductPharmaceuticalRouteOfAdministrationTargetSpecies : BackboneElement,  IFhirJsonSerializable {
    /// <summary>
    /// Coded expression for the species.
    /// </summary>
    public CodeableConcept Code { get; set; }
    /// <summary>
    /// A species specific time during which consumption of animal product is not appropriate.
    /// </summary>
    public List<MedicinalProductPharmaceuticalRouteOfAdministrationTargetSpeciesWithdrawalPeriod> WithdrawalPeriod { get; set; }
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

      if ((WithdrawalPeriod != null) && (WithdrawalPeriod.Count != 0))
      {
        writer.WritePropertyName("withdrawalPeriod");
        writer.WriteStartArray();

        foreach (MedicinalProductPharmaceuticalRouteOfAdministrationTargetSpeciesWithdrawalPeriod valWithdrawalPeriod in WithdrawalPeriod)
        {
          valWithdrawalPeriod.SerializeJson(writer, options, true);
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
          Code = new Fhir.R4.Models.CodeableConcept();
          Code.DeserializeJson(ref reader, options);
          break;

        case "withdrawalPeriod":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          WithdrawalPeriod = new List<MedicinalProductPharmaceuticalRouteOfAdministrationTargetSpeciesWithdrawalPeriod>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Fhir.R4.Models.MedicinalProductPharmaceuticalRouteOfAdministrationTargetSpeciesWithdrawalPeriod objWithdrawalPeriod = new Fhir.R4.Models.MedicinalProductPharmaceuticalRouteOfAdministrationTargetSpeciesWithdrawalPeriod();
            objWithdrawalPeriod.DeserializeJson(ref reader, options);
            WithdrawalPeriod.Add(objWithdrawalPeriod);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (WithdrawalPeriod.Count == 0)
          {
            WithdrawalPeriod = null;
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
  /// The path by which the pharmaceutical product is taken into or makes contact with the body.
  /// </summary>
  [JsonConverter(typeof(Fhir.R4.Serialization.JsonStreamComponentConverter<MedicinalProductPharmaceuticalRouteOfAdministration>))]
  public class MedicinalProductPharmaceuticalRouteOfAdministration : BackboneElement,  IFhirJsonSerializable {
    /// <summary>
    /// Coded expression for the route.
    /// </summary>
    public CodeableConcept Code { get; set; }
    /// <summary>
    /// The first dose (dose quantity) administered in humans can be specified, for a product under investigation, using a numerical value and its unit of measurement.
    /// </summary>
    public Quantity FirstDose { get; set; }
    /// <summary>
    /// The maximum dose per day (maximum dose quantity to be administered in any one 24-h period) that can be administered as per the protocol referenced in the clinical trial authorisation.
    /// </summary>
    public Quantity MaxDosePerDay { get; set; }
    /// <summary>
    /// The maximum dose per treatment period that can be administered as per the protocol referenced in the clinical trial authorisation.
    /// </summary>
    public Ratio MaxDosePerTreatmentPeriod { get; set; }
    /// <summary>
    /// The maximum single dose that can be administered as per the protocol of a clinical trial can be specified using a numerical value and its unit of measurement.
    /// </summary>
    public Quantity MaxSingleDose { get; set; }
    /// <summary>
    /// The maximum treatment period during which an Investigational Medicinal Product can be administered as per the protocol referenced in the clinical trial authorisation.
    /// </summary>
    public Duration MaxTreatmentPeriod { get; set; }
    /// <summary>
    /// A species for which this route applies.
    /// </summary>
    public List<MedicinalProductPharmaceuticalRouteOfAdministrationTargetSpecies> TargetSpecies { get; set; }
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

      if (FirstDose != null)
      {
        writer.WritePropertyName("firstDose");
        FirstDose.SerializeJson(writer, options);
      }

      if (MaxSingleDose != null)
      {
        writer.WritePropertyName("maxSingleDose");
        MaxSingleDose.SerializeJson(writer, options);
      }

      if (MaxDosePerDay != null)
      {
        writer.WritePropertyName("maxDosePerDay");
        MaxDosePerDay.SerializeJson(writer, options);
      }

      if (MaxDosePerTreatmentPeriod != null)
      {
        writer.WritePropertyName("maxDosePerTreatmentPeriod");
        MaxDosePerTreatmentPeriod.SerializeJson(writer, options);
      }

      if (MaxTreatmentPeriod != null)
      {
        writer.WritePropertyName("maxTreatmentPeriod");
        MaxTreatmentPeriod.SerializeJson(writer, options);
      }

      if ((TargetSpecies != null) && (TargetSpecies.Count != 0))
      {
        writer.WritePropertyName("targetSpecies");
        writer.WriteStartArray();

        foreach (MedicinalProductPharmaceuticalRouteOfAdministrationTargetSpecies valTargetSpecies in TargetSpecies)
        {
          valTargetSpecies.SerializeJson(writer, options, true);
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
          Code = new Fhir.R4.Models.CodeableConcept();
          Code.DeserializeJson(ref reader, options);
          break;

        case "firstDose":
          FirstDose = new Fhir.R4.Models.Quantity();
          FirstDose.DeserializeJson(ref reader, options);
          break;

        case "maxDosePerDay":
          MaxDosePerDay = new Fhir.R4.Models.Quantity();
          MaxDosePerDay.DeserializeJson(ref reader, options);
          break;

        case "maxDosePerTreatmentPeriod":
          MaxDosePerTreatmentPeriod = new Fhir.R4.Models.Ratio();
          MaxDosePerTreatmentPeriod.DeserializeJson(ref reader, options);
          break;

        case "maxSingleDose":
          MaxSingleDose = new Fhir.R4.Models.Quantity();
          MaxSingleDose.DeserializeJson(ref reader, options);
          break;

        case "maxTreatmentPeriod":
          MaxTreatmentPeriod = new Fhir.R4.Models.Duration();
          MaxTreatmentPeriod.DeserializeJson(ref reader, options);
          break;

        case "targetSpecies":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          TargetSpecies = new List<MedicinalProductPharmaceuticalRouteOfAdministrationTargetSpecies>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Fhir.R4.Models.MedicinalProductPharmaceuticalRouteOfAdministrationTargetSpecies objTargetSpecies = new Fhir.R4.Models.MedicinalProductPharmaceuticalRouteOfAdministrationTargetSpecies();
            objTargetSpecies.DeserializeJson(ref reader, options);
            TargetSpecies.Add(objTargetSpecies);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (TargetSpecies.Count == 0)
          {
            TargetSpecies = null;
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
  /// A pharmaceutical product described in terms of its composition and dose form.
  /// </summary>
  [JsonConverter(typeof(Fhir.R4.Serialization.JsonStreamComponentConverter<MedicinalProductPharmaceutical>))]
  public class MedicinalProductPharmaceutical : DomainResource,  IFhirJsonSerializable {
    /// <summary>
    /// Resource Type Name
    /// </summary>
    public string ResourceType => "MedicinalProductPharmaceutical";
    /// <summary>
    /// The administrable dose form, after necessary reconstitution.
    /// </summary>
    public CodeableConcept AdministrableDoseForm { get; set; }
    /// <summary>
    /// Characteristics e.g. a products onset of action.
    /// </summary>
    public List<MedicinalProductPharmaceuticalCharacteristics> Characteristics { get; set; }
    /// <summary>
    /// Accompanying device.
    /// </summary>
    public List<Reference> Device { get; set; }
    /// <summary>
    /// An identifier for the pharmaceutical medicinal product.
    /// </summary>
    public List<Identifier> Identifier { get; set; }
    /// <summary>
    /// Ingredient.
    /// </summary>
    public List<Reference> Ingredient { get; set; }
    /// <summary>
    /// The path by which the pharmaceutical product is taken into or makes contact with the body.
    /// </summary>
    public List<MedicinalProductPharmaceuticalRouteOfAdministration> RouteOfAdministration { get; set; }
    /// <summary>
    /// Todo.
    /// </summary>
    public CodeableConcept UnitOfPresentation { get; set; }
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

      if (AdministrableDoseForm != null)
      {
        writer.WritePropertyName("administrableDoseForm");
        AdministrableDoseForm.SerializeJson(writer, options);
      }

      if (UnitOfPresentation != null)
      {
        writer.WritePropertyName("unitOfPresentation");
        UnitOfPresentation.SerializeJson(writer, options);
      }

      if ((Ingredient != null) && (Ingredient.Count != 0))
      {
        writer.WritePropertyName("ingredient");
        writer.WriteStartArray();

        foreach (Reference valIngredient in Ingredient)
        {
          valIngredient.SerializeJson(writer, options, true);
        }

        writer.WriteEndArray();
      }

      if ((Device != null) && (Device.Count != 0))
      {
        writer.WritePropertyName("device");
        writer.WriteStartArray();

        foreach (Reference valDevice in Device)
        {
          valDevice.SerializeJson(writer, options, true);
        }

        writer.WriteEndArray();
      }

      if ((Characteristics != null) && (Characteristics.Count != 0))
      {
        writer.WritePropertyName("characteristics");
        writer.WriteStartArray();

        foreach (MedicinalProductPharmaceuticalCharacteristics valCharacteristics in Characteristics)
        {
          valCharacteristics.SerializeJson(writer, options, true);
        }

        writer.WriteEndArray();
      }

      if ((RouteOfAdministration != null) && (RouteOfAdministration.Count != 0))
      {
        writer.WritePropertyName("routeOfAdministration");
        writer.WriteStartArray();

        foreach (MedicinalProductPharmaceuticalRouteOfAdministration valRouteOfAdministration in RouteOfAdministration)
        {
          valRouteOfAdministration.SerializeJson(writer, options, true);
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
        case "administrableDoseForm":
          AdministrableDoseForm = new Fhir.R4.Models.CodeableConcept();
          AdministrableDoseForm.DeserializeJson(ref reader, options);
          break;

        case "characteristics":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Characteristics = new List<MedicinalProductPharmaceuticalCharacteristics>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Fhir.R4.Models.MedicinalProductPharmaceuticalCharacteristics objCharacteristics = new Fhir.R4.Models.MedicinalProductPharmaceuticalCharacteristics();
            objCharacteristics.DeserializeJson(ref reader, options);
            Characteristics.Add(objCharacteristics);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (Characteristics.Count == 0)
          {
            Characteristics = null;
          }

          break;

        case "device":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Device = new List<Reference>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Fhir.R4.Models.Reference objDevice = new Fhir.R4.Models.Reference();
            objDevice.DeserializeJson(ref reader, options);
            Device.Add(objDevice);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (Device.Count == 0)
          {
            Device = null;
          }

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

        case "ingredient":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Ingredient = new List<Reference>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Fhir.R4.Models.Reference objIngredient = new Fhir.R4.Models.Reference();
            objIngredient.DeserializeJson(ref reader, options);
            Ingredient.Add(objIngredient);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (Ingredient.Count == 0)
          {
            Ingredient = null;
          }

          break;

        case "routeOfAdministration":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          RouteOfAdministration = new List<MedicinalProductPharmaceuticalRouteOfAdministration>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Fhir.R4.Models.MedicinalProductPharmaceuticalRouteOfAdministration objRouteOfAdministration = new Fhir.R4.Models.MedicinalProductPharmaceuticalRouteOfAdministration();
            objRouteOfAdministration.DeserializeJson(ref reader, options);
            RouteOfAdministration.Add(objRouteOfAdministration);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (RouteOfAdministration.Count == 0)
          {
            RouteOfAdministration = null;
          }

          break;

        case "unitOfPresentation":
          UnitOfPresentation = new Fhir.R4.Models.CodeableConcept();
          UnitOfPresentation.DeserializeJson(ref reader, options);
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
