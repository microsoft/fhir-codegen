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
  /// Characteristics for quantitative results of this observation.
  /// </summary>
  [JsonConverter(typeof(Fhir.R4.Serialization.JsonStreamComponentConverter<ObservationDefinitionQuantitativeDetails>))]
  public class ObservationDefinitionQuantitativeDetails : BackboneElement,  IFhirJsonSerializable {
    /// <summary>
    /// Factor for converting value expressed with SI unit to value expressed with customary unit.
    /// </summary>
    public decimal? ConversionFactor { get; set; }
    /// <summary>
    /// Extension container element for ConversionFactor
    /// </summary>
    public Element _ConversionFactor { get; set; }
    /// <summary>
    /// Customary unit used to report quantitative results of observations conforming to this ObservationDefinition.
    /// </summary>
    public CodeableConcept CustomaryUnit { get; set; }
    /// <summary>
    /// Number of digits after decimal separator when the results of such observations are of type Quantity.
    /// </summary>
    public int? DecimalPrecision { get; set; }
    /// <summary>
    /// SI unit used to report quantitative results of observations conforming to this ObservationDefinition.
    /// </summary>
    public CodeableConcept Unit { get; set; }
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

      if (CustomaryUnit != null)
      {
        writer.WritePropertyName("customaryUnit");
        CustomaryUnit.SerializeJson(writer, options);
      }

      if (Unit != null)
      {
        writer.WritePropertyName("unit");
        Unit.SerializeJson(writer, options);
      }

      if (ConversionFactor != null)
      {
        writer.WriteNumber("conversionFactor", (decimal)ConversionFactor!);
      }

      if (_ConversionFactor != null)
      {
        writer.WritePropertyName("_conversionFactor");
        _ConversionFactor.SerializeJson(writer, options);
      }

      if (DecimalPrecision != null)
      {
        writer.WriteNumber("decimalPrecision", (int)DecimalPrecision!);
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
        case "conversionFactor":
          ConversionFactor = reader.GetDecimal();
          break;

        case "_conversionFactor":
          _ConversionFactor = new Fhir.R4.Models.Element();
          _ConversionFactor.DeserializeJson(ref reader, options);
          break;

        case "customaryUnit":
          CustomaryUnit = new Fhir.R4.Models.CodeableConcept();
          CustomaryUnit.DeserializeJson(ref reader, options);
          break;

        case "decimalPrecision":
          DecimalPrecision = reader.GetInt32();
          break;

        case "unit":
          Unit = new Fhir.R4.Models.CodeableConcept();
          Unit.DeserializeJson(ref reader, options);
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
  /// Multiple  ranges of results qualified by different contexts for ordinal or continuous observations conforming to this ObservationDefinition.
  /// </summary>
  [JsonConverter(typeof(Fhir.R4.Serialization.JsonStreamComponentConverter<ObservationDefinitionQualifiedInterval>))]
  public class ObservationDefinitionQualifiedInterval : BackboneElement,  IFhirJsonSerializable {
    /// <summary>
    /// Some analytes vary greatly over age.
    /// </summary>
    public Range Age { get; set; }
    /// <summary>
    /// If this element is not present then the global population is assumed.
    /// </summary>
    public List<CodeableConcept> AppliesTo { get; set; }
    /// <summary>
    /// The category of interval of values for continuous or ordinal observations conforming to this ObservationDefinition.
    /// </summary>
    public string Category { get; set; }
    /// <summary>
    /// Extension container element for Category
    /// </summary>
    public Element _Category { get; set; }
    /// <summary>
    /// Text based condition for which the reference range is valid.
    /// </summary>
    public string Condition { get; set; }
    /// <summary>
    /// Extension container element for Condition
    /// </summary>
    public Element _Condition { get; set; }
    /// <summary>
    /// Codes to indicate the health context the range applies to. For example, the normal or therapeutic range.
    /// </summary>
    public CodeableConcept Context { get; set; }
    /// <summary>
    /// Sex of the population the range applies to.
    /// </summary>
    public string Gender { get; set; }
    /// <summary>
    /// Extension container element for Gender
    /// </summary>
    public Element _Gender { get; set; }
    /// <summary>
    /// The gestational age to which this reference range is applicable, in the context of pregnancy.
    /// </summary>
    public Range GestationalAge { get; set; }
    /// <summary>
    /// The low and high values determining the interval. There may be only one of the two.
    /// </summary>
    public Range Range { get; set; }
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

      if (!string.IsNullOrEmpty(Category))
      {
        writer.WriteString("category", (string)Category!);
      }

      if (_Category != null)
      {
        writer.WritePropertyName("_category");
        _Category.SerializeJson(writer, options);
      }

      if (Range != null)
      {
        writer.WritePropertyName("range");
        Range.SerializeJson(writer, options);
      }

      if (Context != null)
      {
        writer.WritePropertyName("context");
        Context.SerializeJson(writer, options);
      }

      if ((AppliesTo != null) && (AppliesTo.Count != 0))
      {
        writer.WritePropertyName("appliesTo");
        writer.WriteStartArray();

        foreach (CodeableConcept valAppliesTo in AppliesTo)
        {
          valAppliesTo.SerializeJson(writer, options, true);
        }

        writer.WriteEndArray();
      }

      if (!string.IsNullOrEmpty(Gender))
      {
        writer.WriteString("gender", (string)Gender!);
      }

      if (_Gender != null)
      {
        writer.WritePropertyName("_gender");
        _Gender.SerializeJson(writer, options);
      }

      if (Age != null)
      {
        writer.WritePropertyName("age");
        Age.SerializeJson(writer, options);
      }

      if (GestationalAge != null)
      {
        writer.WritePropertyName("gestationalAge");
        GestationalAge.SerializeJson(writer, options);
      }

      if (!string.IsNullOrEmpty(Condition))
      {
        writer.WriteString("condition", (string)Condition!);
      }

      if (_Condition != null)
      {
        writer.WritePropertyName("_condition");
        _Condition.SerializeJson(writer, options);
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
        case "age":
          Age = new Fhir.R4.Models.Range();
          Age.DeserializeJson(ref reader, options);
          break;

        case "appliesTo":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          AppliesTo = new List<CodeableConcept>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Fhir.R4.Models.CodeableConcept objAppliesTo = new Fhir.R4.Models.CodeableConcept();
            objAppliesTo.DeserializeJson(ref reader, options);
            AppliesTo.Add(objAppliesTo);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (AppliesTo.Count == 0)
          {
            AppliesTo = null;
          }

          break;

        case "category":
          Category = reader.GetString();
          break;

        case "_category":
          _Category = new Fhir.R4.Models.Element();
          _Category.DeserializeJson(ref reader, options);
          break;

        case "condition":
          Condition = reader.GetString();
          break;

        case "_condition":
          _Condition = new Fhir.R4.Models.Element();
          _Condition.DeserializeJson(ref reader, options);
          break;

        case "context":
          Context = new Fhir.R4.Models.CodeableConcept();
          Context.DeserializeJson(ref reader, options);
          break;

        case "gender":
          Gender = reader.GetString();
          break;

        case "_gender":
          _Gender = new Fhir.R4.Models.Element();
          _Gender.DeserializeJson(ref reader, options);
          break;

        case "gestationalAge":
          GestationalAge = new Fhir.R4.Models.Range();
          GestationalAge.DeserializeJson(ref reader, options);
          break;

        case "range":
          Range = new Fhir.R4.Models.Range();
          Range.DeserializeJson(ref reader, options);
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
  /// Set of definitional characteristics for a kind of observation or measurement produced or consumed by an orderable health care service.
  /// </summary>
  [JsonConverter(typeof(Fhir.R4.Serialization.JsonStreamComponentConverter<ObservationDefinition>))]
  public class ObservationDefinition : DomainResource,  IFhirJsonSerializable {
    /// <summary>
    /// Resource Type Name
    /// </summary>
    public string ResourceType => "ObservationDefinition";
    /// <summary>
    /// The set of abnormal coded results for the observation conforming to this ObservationDefinition.
    /// </summary>
    public Reference AbnormalCodedValueSet { get; set; }
    /// <summary>
    /// This element allows various categorization schemes based on the owner’s definition of the category and effectively multiple categories can be used for one instance of ObservationDefinition. The level of granularity is defined by the category concepts in the value set.
    /// </summary>
    public List<CodeableConcept> Category { get; set; }
    /// <summary>
    /// Describes what will be observed. Sometimes this is called the observation "name".
    /// </summary>
    public CodeableConcept Code { get; set; }
    /// <summary>
    /// The set of critical coded results for the observation conforming to this ObservationDefinition.
    /// </summary>
    public Reference CriticalCodedValueSet { get; set; }
    /// <summary>
    /// A unique identifier assigned to this ObservationDefinition artifact.
    /// </summary>
    public List<Identifier> Identifier { get; set; }
    /// <summary>
    /// Only used if not implicit in observation code.
    /// </summary>
    public CodeableConcept Method { get; set; }
    /// <summary>
    /// An example of observation allowing multiple results is "bacteria identified by culture". Conversely, the measurement of a potassium level allows a single result.
    /// </summary>
    public bool? MultipleResultsAllowed { get; set; }
    /// <summary>
    /// The set of normal coded results for the observations conforming to this ObservationDefinition.
    /// </summary>
    public Reference NormalCodedValueSet { get; set; }
    /// <summary>
    /// The data types allowed for the value element of the instance observations conforming to this ObservationDefinition.
    /// </summary>
    public List<string> PermittedDataType { get; set; }
    /// <summary>
    /// Extension container element for PermittedDataType
    /// </summary>
    public List<Element> _PermittedDataType { get; set; }
    /// <summary>
    /// The preferred name to be used when reporting the results of observations conforming to this ObservationDefinition.
    /// </summary>
    public string PreferredReportName { get; set; }
    /// <summary>
    /// Extension container element for PreferredReportName
    /// </summary>
    public Element _PreferredReportName { get; set; }
    /// <summary>
    /// Multiple  ranges of results qualified by different contexts for ordinal or continuous observations conforming to this ObservationDefinition.
    /// </summary>
    public List<ObservationDefinitionQualifiedInterval> QualifiedInterval { get; set; }
    /// <summary>
    /// Characteristics for quantitative results of this observation.
    /// </summary>
    public ObservationDefinitionQuantitativeDetails QuantitativeDetails { get; set; }
    /// <summary>
    /// The set of valid coded results for the observations  conforming to this ObservationDefinition.
    /// </summary>
    public Reference ValidCodedValueSet { get; set; }
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

      if ((Category != null) && (Category.Count != 0))
      {
        writer.WritePropertyName("category");
        writer.WriteStartArray();

        foreach (CodeableConcept valCategory in Category)
        {
          valCategory.SerializeJson(writer, options, true);
        }

        writer.WriteEndArray();
      }

      if (Code != null)
      {
        writer.WritePropertyName("code");
        Code.SerializeJson(writer, options);
      }

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

      if ((PermittedDataType != null) && (PermittedDataType.Count != 0))
      {
        writer.WritePropertyName("permittedDataType");
        writer.WriteStartArray();

        foreach (string valPermittedDataType in PermittedDataType)
        {
          writer.WriteStringValue(valPermittedDataType);
        }

        writer.WriteEndArray();
      }

      if ((_PermittedDataType != null) && (_PermittedDataType.Count != 0))
      {
        writer.WritePropertyName("_permittedDataType");
        writer.WriteStartArray();

        foreach (Element val_PermittedDataType in _PermittedDataType)
        {
          val_PermittedDataType.SerializeJson(writer, options, true);
        }

        writer.WriteEndArray();
      }

      if (MultipleResultsAllowed != null)
      {
        writer.WriteBoolean("multipleResultsAllowed", (bool)MultipleResultsAllowed!);
      }

      if (Method != null)
      {
        writer.WritePropertyName("method");
        Method.SerializeJson(writer, options);
      }

      if (!string.IsNullOrEmpty(PreferredReportName))
      {
        writer.WriteString("preferredReportName", (string)PreferredReportName!);
      }

      if (_PreferredReportName != null)
      {
        writer.WritePropertyName("_preferredReportName");
        _PreferredReportName.SerializeJson(writer, options);
      }

      if (QuantitativeDetails != null)
      {
        writer.WritePropertyName("quantitativeDetails");
        QuantitativeDetails.SerializeJson(writer, options);
      }

      if ((QualifiedInterval != null) && (QualifiedInterval.Count != 0))
      {
        writer.WritePropertyName("qualifiedInterval");
        writer.WriteStartArray();

        foreach (ObservationDefinitionQualifiedInterval valQualifiedInterval in QualifiedInterval)
        {
          valQualifiedInterval.SerializeJson(writer, options, true);
        }

        writer.WriteEndArray();
      }

      if (ValidCodedValueSet != null)
      {
        writer.WritePropertyName("validCodedValueSet");
        ValidCodedValueSet.SerializeJson(writer, options);
      }

      if (NormalCodedValueSet != null)
      {
        writer.WritePropertyName("normalCodedValueSet");
        NormalCodedValueSet.SerializeJson(writer, options);
      }

      if (AbnormalCodedValueSet != null)
      {
        writer.WritePropertyName("abnormalCodedValueSet");
        AbnormalCodedValueSet.SerializeJson(writer, options);
      }

      if (CriticalCodedValueSet != null)
      {
        writer.WritePropertyName("criticalCodedValueSet");
        CriticalCodedValueSet.SerializeJson(writer, options);
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
        case "abnormalCodedValueSet":
          AbnormalCodedValueSet = new Fhir.R4.Models.Reference();
          AbnormalCodedValueSet.DeserializeJson(ref reader, options);
          break;

        case "category":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Category = new List<CodeableConcept>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Fhir.R4.Models.CodeableConcept objCategory = new Fhir.R4.Models.CodeableConcept();
            objCategory.DeserializeJson(ref reader, options);
            Category.Add(objCategory);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (Category.Count == 0)
          {
            Category = null;
          }

          break;

        case "code":
          Code = new Fhir.R4.Models.CodeableConcept();
          Code.DeserializeJson(ref reader, options);
          break;

        case "criticalCodedValueSet":
          CriticalCodedValueSet = new Fhir.R4.Models.Reference();
          CriticalCodedValueSet.DeserializeJson(ref reader, options);
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

        case "method":
          Method = new Fhir.R4.Models.CodeableConcept();
          Method.DeserializeJson(ref reader, options);
          break;

        case "multipleResultsAllowed":
          MultipleResultsAllowed = reader.GetBoolean();
          break;

        case "normalCodedValueSet":
          NormalCodedValueSet = new Fhir.R4.Models.Reference();
          NormalCodedValueSet.DeserializeJson(ref reader, options);
          break;

        case "permittedDataType":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          PermittedDataType = new List<string>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            PermittedDataType.Add(reader.GetString());

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (PermittedDataType.Count == 0)
          {
            PermittedDataType = null;
          }

          break;

        case "_permittedDataType":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          _PermittedDataType = new List<Element>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Fhir.R4.Models.Element obj_PermittedDataType = new Fhir.R4.Models.Element();
            obj_PermittedDataType.DeserializeJson(ref reader, options);
            _PermittedDataType.Add(obj_PermittedDataType);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (_PermittedDataType.Count == 0)
          {
            _PermittedDataType = null;
          }

          break;

        case "preferredReportName":
          PreferredReportName = reader.GetString();
          break;

        case "_preferredReportName":
          _PreferredReportName = new Fhir.R4.Models.Element();
          _PreferredReportName.DeserializeJson(ref reader, options);
          break;

        case "qualifiedInterval":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          QualifiedInterval = new List<ObservationDefinitionQualifiedInterval>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Fhir.R4.Models.ObservationDefinitionQualifiedInterval objQualifiedInterval = new Fhir.R4.Models.ObservationDefinitionQualifiedInterval();
            objQualifiedInterval.DeserializeJson(ref reader, options);
            QualifiedInterval.Add(objQualifiedInterval);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (QualifiedInterval.Count == 0)
          {
            QualifiedInterval = null;
          }

          break;

        case "quantitativeDetails":
          QuantitativeDetails = new Fhir.R4.Models.ObservationDefinitionQuantitativeDetails();
          QuantitativeDetails.DeserializeJson(ref reader, options);
          break;

        case "validCodedValueSet":
          ValidCodedValueSet = new Fhir.R4.Models.Reference();
          ValidCodedValueSet.DeserializeJson(ref reader, options);
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
