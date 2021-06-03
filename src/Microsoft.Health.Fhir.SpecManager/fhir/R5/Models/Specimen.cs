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
  /// Details concerning the specimen collection.
  /// </summary>
  [JsonConverter(typeof(fhirCsR5.Serialization.JsonStreamComponentConverter<SpecimenCollection>))]
  public class SpecimenCollection : BackboneElement,  IFhirJsonSerializable {
    /// <summary>
    /// If the use case requires  BodySite to be handled as a separate resource instead of an inline coded element (e.g. to identify and track separately)  then use the standard extension [bodySite](extension-bodysite.html).
    /// </summary>
    public CodeableConcept BodySite { get; set; }
    /// <summary>
    /// Time when specimen was collected from subject - the physiologically relevant time.
    /// </summary>
    public string CollectedDateTime { get; set; }
    /// <summary>
    /// Extension container element for CollectedDateTime
    /// </summary>
    public Element _CollectedDateTime { get; set; }
    /// <summary>
    /// Time when specimen was collected from subject - the physiologically relevant time.
    /// </summary>
    public Period CollectedPeriod { get; set; }
    /// <summary>
    /// Person who collected the specimen.
    /// </summary>
    public Reference Collector { get; set; }
    /// <summary>
    /// The span of time over which the collection of a specimen occurred.
    /// </summary>
    public Duration Duration { get; set; }
    /// <summary>
    /// Representing fasting status using this element is preferred to representing it with an observation using a 'pre-coordinated code'  such as  LOINC 2005-7 (Calcium [Moles/​time] in 2 hour Urine --12 hours fasting), or  using  a component observation ` such as `Observation.component code`  = LOINC 49541-6 (Fasting status - Reported).
    /// </summary>
    public CodeableConcept FastingStatusCodeableConcept { get; set; }
    /// <summary>
    /// Representing fasting status using this element is preferred to representing it with an observation using a 'pre-coordinated code'  such as  LOINC 2005-7 (Calcium [Moles/​time] in 2 hour Urine --12 hours fasting), or  using  a component observation ` such as `Observation.component code`  = LOINC 49541-6 (Fasting status - Reported).
    /// </summary>
    public Duration FastingStatusDuration { get; set; }
    /// <summary>
    /// A coded value specifying the technique that is used to perform the procedure.
    /// </summary>
    public CodeableConcept Method { get; set; }
    /// <summary>
    /// The quantity of specimen collected; for instance the volume of a blood sample, or the physical measurement of an anatomic pathology sample.
    /// </summary>
    public Quantity Quantity { get; set; }
    /// <summary>
    /// Serialize to a JSON object
    /// </summary>
    public new void SerializeJson(Utf8JsonWriter writer, JsonSerializerOptions options, bool includeStartObject = true)
    {
      if (includeStartObject)
      {
        writer.WriteStartObject();
      }
      ((fhirCsR5.Models.BackboneElement)this).SerializeJson(writer, options, false);

      if (Collector != null)
      {
        writer.WritePropertyName("collector");
        Collector.SerializeJson(writer, options);
      }

      if (!string.IsNullOrEmpty(CollectedDateTime))
      {
        writer.WriteString("collectedDateTime", (string)CollectedDateTime!);
      }

      if (_CollectedDateTime != null)
      {
        writer.WritePropertyName("_collectedDateTime");
        _CollectedDateTime.SerializeJson(writer, options);
      }

      if (CollectedPeriod != null)
      {
        writer.WritePropertyName("collectedPeriod");
        CollectedPeriod.SerializeJson(writer, options);
      }

      if (Duration != null)
      {
        writer.WritePropertyName("duration");
        Duration.SerializeJson(writer, options);
      }

      if (Quantity != null)
      {
        writer.WritePropertyName("quantity");
        Quantity.SerializeJson(writer, options);
      }

      if (Method != null)
      {
        writer.WritePropertyName("method");
        Method.SerializeJson(writer, options);
      }

      if (BodySite != null)
      {
        writer.WritePropertyName("bodySite");
        BodySite.SerializeJson(writer, options);
      }

      if (FastingStatusCodeableConcept != null)
      {
        writer.WritePropertyName("fastingStatusCodeableConcept");
        FastingStatusCodeableConcept.SerializeJson(writer, options);
      }

      if (FastingStatusDuration != null)
      {
        writer.WritePropertyName("fastingStatusDuration");
        FastingStatusDuration.SerializeJson(writer, options);
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
        case "bodySite":
          BodySite = new fhirCsR5.Models.CodeableConcept();
          BodySite.DeserializeJson(ref reader, options);
          break;

        case "collectedDateTime":
          CollectedDateTime = reader.GetString();
          break;

        case "_collectedDateTime":
          _CollectedDateTime = new fhirCsR5.Models.Element();
          _CollectedDateTime.DeserializeJson(ref reader, options);
          break;

        case "collectedPeriod":
          CollectedPeriod = new fhirCsR5.Models.Period();
          CollectedPeriod.DeserializeJson(ref reader, options);
          break;

        case "collector":
          Collector = new fhirCsR5.Models.Reference();
          Collector.DeserializeJson(ref reader, options);
          break;

        case "duration":
          Duration = new fhirCsR5.Models.Duration();
          Duration.DeserializeJson(ref reader, options);
          break;

        case "fastingStatusCodeableConcept":
          FastingStatusCodeableConcept = new fhirCsR5.Models.CodeableConcept();
          FastingStatusCodeableConcept.DeserializeJson(ref reader, options);
          break;

        case "fastingStatusDuration":
          FastingStatusDuration = new fhirCsR5.Models.Duration();
          FastingStatusDuration.DeserializeJson(ref reader, options);
          break;

        case "method":
          Method = new fhirCsR5.Models.CodeableConcept();
          Method.DeserializeJson(ref reader, options);
          break;

        case "quantity":
          Quantity = new fhirCsR5.Models.Quantity();
          Quantity.DeserializeJson(ref reader, options);
          break;

        default:
          ((fhirCsR5.Models.BackboneElement)this).DeserializeJsonProperty(ref reader, options, propertyName);
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
  /// Details concerning processing and processing steps for the specimen.
  /// </summary>
  [JsonConverter(typeof(fhirCsR5.Serialization.JsonStreamComponentConverter<SpecimenProcessing>))]
  public class SpecimenProcessing : BackboneElement,  IFhirJsonSerializable {
    /// <summary>
    /// Material used in the processing step.
    /// </summary>
    public List<Reference> Additive { get; set; }
    /// <summary>
    /// Textual description of procedure.
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// Extension container element for Description
    /// </summary>
    public Element _Description { get; set; }
    /// <summary>
    /// A coded value specifying the procedure used to process the specimen.
    /// </summary>
    public CodeableConcept Procedure { get; set; }
    /// <summary>
    /// A record of the time or period when the specimen processing occurred.  For example the time of sample fixation or the period of time the sample was in formalin.
    /// </summary>
    public string TimeDateTime { get; set; }
    /// <summary>
    /// Extension container element for TimeDateTime
    /// </summary>
    public Element _TimeDateTime { get; set; }
    /// <summary>
    /// A record of the time or period when the specimen processing occurred.  For example the time of sample fixation or the period of time the sample was in formalin.
    /// </summary>
    public Period TimePeriod { get; set; }
    /// <summary>
    /// Serialize to a JSON object
    /// </summary>
    public new void SerializeJson(Utf8JsonWriter writer, JsonSerializerOptions options, bool includeStartObject = true)
    {
      if (includeStartObject)
      {
        writer.WriteStartObject();
      }
      ((fhirCsR5.Models.BackboneElement)this).SerializeJson(writer, options, false);

      if (!string.IsNullOrEmpty(Description))
      {
        writer.WriteString("description", (string)Description!);
      }

      if (_Description != null)
      {
        writer.WritePropertyName("_description");
        _Description.SerializeJson(writer, options);
      }

      if (Procedure != null)
      {
        writer.WritePropertyName("procedure");
        Procedure.SerializeJson(writer, options);
      }

      if ((Additive != null) && (Additive.Count != 0))
      {
        writer.WritePropertyName("additive");
        writer.WriteStartArray();

        foreach (Reference valAdditive in Additive)
        {
          valAdditive.SerializeJson(writer, options, true);
        }

        writer.WriteEndArray();
      }

      if (!string.IsNullOrEmpty(TimeDateTime))
      {
        writer.WriteString("timeDateTime", (string)TimeDateTime!);
      }

      if (_TimeDateTime != null)
      {
        writer.WritePropertyName("_timeDateTime");
        _TimeDateTime.SerializeJson(writer, options);
      }

      if (TimePeriod != null)
      {
        writer.WritePropertyName("timePeriod");
        TimePeriod.SerializeJson(writer, options);
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
        case "additive":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Additive = new List<Reference>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            fhirCsR5.Models.Reference objAdditive = new fhirCsR5.Models.Reference();
            objAdditive.DeserializeJson(ref reader, options);
            Additive.Add(objAdditive);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (Additive.Count == 0)
          {
            Additive = null;
          }

          break;

        case "description":
          Description = reader.GetString();
          break;

        case "_description":
          _Description = new fhirCsR5.Models.Element();
          _Description.DeserializeJson(ref reader, options);
          break;

        case "procedure":
          Procedure = new fhirCsR5.Models.CodeableConcept();
          Procedure.DeserializeJson(ref reader, options);
          break;

        case "timeDateTime":
          TimeDateTime = reader.GetString();
          break;

        case "_timeDateTime":
          _TimeDateTime = new fhirCsR5.Models.Element();
          _TimeDateTime.DeserializeJson(ref reader, options);
          break;

        case "timePeriod":
          TimePeriod = new fhirCsR5.Models.Period();
          TimePeriod.DeserializeJson(ref reader, options);
          break;

        default:
          ((fhirCsR5.Models.BackboneElement)this).DeserializeJsonProperty(ref reader, options, propertyName);
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
  /// The container holding the specimen.  The recursive nature of containers; i.e. blood in tube in tray in rack is not addressed here.
  /// </summary>
  [JsonConverter(typeof(fhirCsR5.Serialization.JsonStreamComponentConverter<SpecimenContainer>))]
  public class SpecimenContainer : BackboneElement,  IFhirJsonSerializable {
    /// <summary>
    /// Introduced substance to preserve, maintain or enhance the specimen. Examples: Formalin, Citrate, EDTA.
    /// </summary>
    public CodeableConcept AdditiveCodeableConcept { get; set; }
    /// <summary>
    /// Introduced substance to preserve, maintain or enhance the specimen. Examples: Formalin, Citrate, EDTA.
    /// </summary>
    public Reference AdditiveReference { get; set; }
    /// <summary>
    /// The capacity (volume or other measure) the container may contain.
    /// </summary>
    public Quantity Capacity { get; set; }
    /// <summary>
    /// Textual description of the container.
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// Extension container element for Description
    /// </summary>
    public Element _Description { get; set; }
    /// <summary>
    /// Id for container. There may be multiple; a manufacturer's bar code, lab assigned identifier, etc. The container ID may differ from the specimen id in some circumstances.
    /// </summary>
    public List<Identifier> Identifier { get; set; }
    /// <summary>
    /// The quantity of specimen in the container; may be volume, dimensions, or other appropriate measurements, depending on the specimen type.
    /// </summary>
    public Quantity SpecimenQuantity { get; set; }
    /// <summary>
    /// The type of container associated with the specimen (e.g. slide, aliquot, etc.).
    /// </summary>
    public CodeableConcept Type { get; set; }
    /// <summary>
    /// Serialize to a JSON object
    /// </summary>
    public new void SerializeJson(Utf8JsonWriter writer, JsonSerializerOptions options, bool includeStartObject = true)
    {
      if (includeStartObject)
      {
        writer.WriteStartObject();
      }
      ((fhirCsR5.Models.BackboneElement)this).SerializeJson(writer, options, false);

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

      if (!string.IsNullOrEmpty(Description))
      {
        writer.WriteString("description", (string)Description!);
      }

      if (_Description != null)
      {
        writer.WritePropertyName("_description");
        _Description.SerializeJson(writer, options);
      }

      if (Type != null)
      {
        writer.WritePropertyName("type");
        Type.SerializeJson(writer, options);
      }

      if (Capacity != null)
      {
        writer.WritePropertyName("capacity");
        Capacity.SerializeJson(writer, options);
      }

      if (SpecimenQuantity != null)
      {
        writer.WritePropertyName("specimenQuantity");
        SpecimenQuantity.SerializeJson(writer, options);
      }

      if (AdditiveCodeableConcept != null)
      {
        writer.WritePropertyName("additiveCodeableConcept");
        AdditiveCodeableConcept.SerializeJson(writer, options);
      }

      if (AdditiveReference != null)
      {
        writer.WritePropertyName("additiveReference");
        AdditiveReference.SerializeJson(writer, options);
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
        case "additiveCodeableConcept":
          AdditiveCodeableConcept = new fhirCsR5.Models.CodeableConcept();
          AdditiveCodeableConcept.DeserializeJson(ref reader, options);
          break;

        case "additiveReference":
          AdditiveReference = new fhirCsR5.Models.Reference();
          AdditiveReference.DeserializeJson(ref reader, options);
          break;

        case "capacity":
          Capacity = new fhirCsR5.Models.Quantity();
          Capacity.DeserializeJson(ref reader, options);
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

        case "specimenQuantity":
          SpecimenQuantity = new fhirCsR5.Models.Quantity();
          SpecimenQuantity.DeserializeJson(ref reader, options);
          break;

        case "type":
          Type = new fhirCsR5.Models.CodeableConcept();
          Type.DeserializeJson(ref reader, options);
          break;

        default:
          ((fhirCsR5.Models.BackboneElement)this).DeserializeJsonProperty(ref reader, options, propertyName);
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
  /// A sample to be used for analysis.
  /// </summary>
  [JsonConverter(typeof(fhirCsR5.Serialization.JsonStreamComponentConverter<Specimen>))]
  public class Specimen : DomainResource,  IFhirJsonSerializable {
    /// <summary>
    /// Resource Type Name
    /// </summary>
    public string ResourceType => "Specimen";
    /// <summary>
    /// The identifier assigned by the lab when accessioning specimen(s). This is not necessarily the same as the specimen identifier, depending on local lab procedures.
    /// </summary>
    public Identifier AccessionIdentifier { get; set; }
    /// <summary>
    /// Details concerning the specimen collection.
    /// </summary>
    public SpecimenCollection Collection { get; set; }
    /// <summary>
    /// Specimen condition is an observation made about the specimen.  It's a point-in-time assessment.  It can be used to assess its quality or appropriateness for a specific test.
    /// </summary>
    public List<CodeableConcept> Condition { get; set; }
    /// <summary>
    /// The container holding the specimen.  The recursive nature of containers; i.e. blood in tube in tray in rack is not addressed here.
    /// </summary>
    public List<SpecimenContainer> Container { get; set; }
    /// <summary>
    /// Id for specimen.
    /// </summary>
    public List<Identifier> Identifier { get; set; }
    /// <summary>
    /// To communicate any details or issues about the specimen or during the specimen collection. (for example: broken vial, sent with patient, frozen).
    /// </summary>
    public List<Annotation> Note { get; set; }
    /// <summary>
    /// The parent specimen could be the source from which the current specimen is derived by some processing step (e.g. an aliquot or isolate or extracted nucleic acids from clinical samples) or one of many specimens that were combined to create a pooled sample.
    /// </summary>
    public List<Reference> Parent { get; set; }
    /// <summary>
    /// Details concerning processing and processing steps for the specimen.
    /// </summary>
    public List<SpecimenProcessing> Processing { get; set; }
    /// <summary>
    /// Time when specimen was received for processing or testing.
    /// </summary>
    public string ReceivedTime { get; set; }
    /// <summary>
    /// Extension container element for ReceivedTime
    /// </summary>
    public Element _ReceivedTime { get; set; }
    /// <summary>
    /// The request may be explicit or implied such with a ServiceRequest that requires a blood draw.
    /// </summary>
    public List<Reference> Request { get; set; }
    /// <summary>
    /// This element is labeled as a modifier because the status contains codes that mark the resource as not currently valid.
    /// </summary>
    public string Status { get; set; }
    /// <summary>
    /// Extension container element for Status
    /// </summary>
    public Element _Status { get; set; }
    /// <summary>
    /// Where the specimen came from. This may be from patient(s), from a location (e.g., the source of an environmental sample), or a sampling of a substance or a device.
    /// </summary>
    public Reference Subject { get; set; }
    /// <summary>
    /// The type can change the way that a specimen is handled and drives what kind of analyses can properly be performed on the specimen. It is frequently used in diagnostic work flow decision making systems.
    /// </summary>
    public CodeableConcept Type { get; set; }
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

      if (AccessionIdentifier != null)
      {
        writer.WritePropertyName("accessionIdentifier");
        AccessionIdentifier.SerializeJson(writer, options);
      }

      if (!string.IsNullOrEmpty(Status))
      {
        writer.WriteString("status", (string)Status!);
      }

      if (_Status != null)
      {
        writer.WritePropertyName("_status");
        _Status.SerializeJson(writer, options);
      }

      if (Type != null)
      {
        writer.WritePropertyName("type");
        Type.SerializeJson(writer, options);
      }

      if (Subject != null)
      {
        writer.WritePropertyName("subject");
        Subject.SerializeJson(writer, options);
      }

      if (!string.IsNullOrEmpty(ReceivedTime))
      {
        writer.WriteString("receivedTime", (string)ReceivedTime!);
      }

      if (_ReceivedTime != null)
      {
        writer.WritePropertyName("_receivedTime");
        _ReceivedTime.SerializeJson(writer, options);
      }

      if ((Parent != null) && (Parent.Count != 0))
      {
        writer.WritePropertyName("parent");
        writer.WriteStartArray();

        foreach (Reference valParent in Parent)
        {
          valParent.SerializeJson(writer, options, true);
        }

        writer.WriteEndArray();
      }

      if ((Request != null) && (Request.Count != 0))
      {
        writer.WritePropertyName("request");
        writer.WriteStartArray();

        foreach (Reference valRequest in Request)
        {
          valRequest.SerializeJson(writer, options, true);
        }

        writer.WriteEndArray();
      }

      if (Collection != null)
      {
        writer.WritePropertyName("collection");
        Collection.SerializeJson(writer, options);
      }

      if ((Processing != null) && (Processing.Count != 0))
      {
        writer.WritePropertyName("processing");
        writer.WriteStartArray();

        foreach (SpecimenProcessing valProcessing in Processing)
        {
          valProcessing.SerializeJson(writer, options, true);
        }

        writer.WriteEndArray();
      }

      if ((Container != null) && (Container.Count != 0))
      {
        writer.WritePropertyName("container");
        writer.WriteStartArray();

        foreach (SpecimenContainer valContainer in Container)
        {
          valContainer.SerializeJson(writer, options, true);
        }

        writer.WriteEndArray();
      }

      if ((Condition != null) && (Condition.Count != 0))
      {
        writer.WritePropertyName("condition");
        writer.WriteStartArray();

        foreach (CodeableConcept valCondition in Condition)
        {
          valCondition.SerializeJson(writer, options, true);
        }

        writer.WriteEndArray();
      }

      if ((Note != null) && (Note.Count != 0))
      {
        writer.WritePropertyName("note");
        writer.WriteStartArray();

        foreach (Annotation valNote in Note)
        {
          valNote.SerializeJson(writer, options, true);
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
        case "accessionIdentifier":
          AccessionIdentifier = new fhirCsR5.Models.Identifier();
          AccessionIdentifier.DeserializeJson(ref reader, options);
          break;

        case "collection":
          Collection = new fhirCsR5.Models.SpecimenCollection();
          Collection.DeserializeJson(ref reader, options);
          break;

        case "condition":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Condition = new List<CodeableConcept>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            fhirCsR5.Models.CodeableConcept objCondition = new fhirCsR5.Models.CodeableConcept();
            objCondition.DeserializeJson(ref reader, options);
            Condition.Add(objCondition);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (Condition.Count == 0)
          {
            Condition = null;
          }

          break;

        case "container":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Container = new List<SpecimenContainer>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            fhirCsR5.Models.SpecimenContainer objContainer = new fhirCsR5.Models.SpecimenContainer();
            objContainer.DeserializeJson(ref reader, options);
            Container.Add(objContainer);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (Container.Count == 0)
          {
            Container = null;
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

        case "note":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Note = new List<Annotation>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            fhirCsR5.Models.Annotation objNote = new fhirCsR5.Models.Annotation();
            objNote.DeserializeJson(ref reader, options);
            Note.Add(objNote);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (Note.Count == 0)
          {
            Note = null;
          }

          break;

        case "parent":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Parent = new List<Reference>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            fhirCsR5.Models.Reference objParent = new fhirCsR5.Models.Reference();
            objParent.DeserializeJson(ref reader, options);
            Parent.Add(objParent);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (Parent.Count == 0)
          {
            Parent = null;
          }

          break;

        case "processing":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Processing = new List<SpecimenProcessing>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            fhirCsR5.Models.SpecimenProcessing objProcessing = new fhirCsR5.Models.SpecimenProcessing();
            objProcessing.DeserializeJson(ref reader, options);
            Processing.Add(objProcessing);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (Processing.Count == 0)
          {
            Processing = null;
          }

          break;

        case "receivedTime":
          ReceivedTime = reader.GetString();
          break;

        case "_receivedTime":
          _ReceivedTime = new fhirCsR5.Models.Element();
          _ReceivedTime.DeserializeJson(ref reader, options);
          break;

        case "request":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Request = new List<Reference>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            fhirCsR5.Models.Reference objRequest = new fhirCsR5.Models.Reference();
            objRequest.DeserializeJson(ref reader, options);
            Request.Add(objRequest);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (Request.Count == 0)
          {
            Request = null;
          }

          break;

        case "status":
          Status = reader.GetString();
          break;

        case "_status":
          _Status = new fhirCsR5.Models.Element();
          _Status.DeserializeJson(ref reader, options);
          break;

        case "subject":
          Subject = new fhirCsR5.Models.Reference();
          Subject.DeserializeJson(ref reader, options);
          break;

        case "type":
          Type = new fhirCsR5.Models.CodeableConcept();
          Type.DeserializeJson(ref reader, options);
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
  /// <summary>
  /// Code Values for the Specimen.status field
  /// </summary>
  public static class SpecimenStatusCodes {
    public const string AVAILABLE = "available";
    public const string UNAVAILABLE = "unavailable";
    public const string UNSATISFACTORY = "unsatisfactory";
    public const string ENTERED_IN_ERROR = "entered-in-error";
  }
}
