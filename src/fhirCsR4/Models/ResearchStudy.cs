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
  /// Describes an expected sequence of events for one of the participants of a study.  E.g. Exposure to drug A, wash-out, exposure to drug B, wash-out, follow-up.
  /// </summary>
  [JsonConverter(typeof(fhirCsR4.Serialization.JsonStreamComponentConverter<ResearchStudyArm>))]
  public class ResearchStudyArm : BackboneElement,  IFhirJsonSerializable {
    /// <summary>
    /// A succinct description of the path through the study that would be followed by a subject adhering to this arm.
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// Extension container element for Description
    /// </summary>
    public Element _Description { get; set; }
    /// <summary>
    /// Unique, human-readable label for this arm of the study.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Extension container element for Name
    /// </summary>
    public Element _Name { get; set; }
    /// <summary>
    /// Categorization of study arm, e.g. experimental, active comparator, placebo comparater.
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
      ((fhirCsR4.Models.BackboneElement)this).SerializeJson(writer, options, false);

      if (!string.IsNullOrEmpty(Name))
      {
        writer.WriteString("name", (string)Name!);
      }

      if (_Name != null)
      {
        writer.WritePropertyName("_name");
        _Name.SerializeJson(writer, options);
      }

      if (Type != null)
      {
        writer.WritePropertyName("type");
        Type.SerializeJson(writer, options);
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
        case "description":
          Description = reader.GetString();
          break;

        case "_description":
          _Description = new fhirCsR4.Models.Element();
          _Description.DeserializeJson(ref reader, options);
          break;

        case "name":
          Name = reader.GetString();
          break;

        case "_name":
          _Name = new fhirCsR4.Models.Element();
          _Name.DeserializeJson(ref reader, options);
          break;

        case "type":
          Type = new fhirCsR4.Models.CodeableConcept();
          Type.DeserializeJson(ref reader, options);
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
  /// <summary>
  /// A goal that the study is aiming to achieve in terms of a scientific question to be answered by the analysis of data collected during the study.
  /// </summary>
  [JsonConverter(typeof(fhirCsR4.Serialization.JsonStreamComponentConverter<ResearchStudyObjective>))]
  public class ResearchStudyObjective : BackboneElement,  IFhirJsonSerializable {
    /// <summary>
    /// Unique, human-readable label for this objective of the study.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Extension container element for Name
    /// </summary>
    public Element _Name { get; set; }
    /// <summary>
    /// The kind of study objective.
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
      ((fhirCsR4.Models.BackboneElement)this).SerializeJson(writer, options, false);

      if (!string.IsNullOrEmpty(Name))
      {
        writer.WriteString("name", (string)Name!);
      }

      if (_Name != null)
      {
        writer.WritePropertyName("_name");
        _Name.SerializeJson(writer, options);
      }

      if (Type != null)
      {
        writer.WritePropertyName("type");
        Type.SerializeJson(writer, options);
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
        case "name":
          Name = reader.GetString();
          break;

        case "_name":
          _Name = new fhirCsR4.Models.Element();
          _Name.DeserializeJson(ref reader, options);
          break;

        case "type":
          Type = new fhirCsR4.Models.CodeableConcept();
          Type.DeserializeJson(ref reader, options);
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
  /// <summary>
  /// A process where a researcher or organization plans and then executes a series of steps intended to increase the field of healthcare-related knowledge.  This includes studies of safety, efficacy, comparative effectiveness and other information about medications, devices, therapies and other interventional and investigative techniques.  A ResearchStudy involves the gathering of information about human or animal subjects.
  /// </summary>
  [JsonConverter(typeof(fhirCsR4.Serialization.JsonStreamComponentConverter<ResearchStudy>))]
  public class ResearchStudy : DomainResource,  IFhirJsonSerializable {
    /// <summary>
    /// Resource Type Name
    /// </summary>
    public override string ResourceType => "ResearchStudy";
    /// <summary>
    /// Describes an expected sequence of events for one of the participants of a study.  E.g. Exposure to drug A, wash-out, exposure to drug B, wash-out, follow-up.
    /// </summary>
    public List<ResearchStudyArm> Arm { get; set; }
    /// <summary>
    /// Codes categorizing the type of study such as investigational vs. observational, type of blinding, type of randomization, safety vs. efficacy, etc.
    /// </summary>
    public List<CodeableConcept> Category { get; set; }
    /// <summary>
    /// The condition that is the focus of the study.  For example, In a study to examine risk factors for Lupus, might have as an inclusion criterion "healthy volunteer", but the target condition code would be a Lupus SNOMED code.
    /// </summary>
    public List<CodeableConcept> Condition { get; set; }
    /// <summary>
    /// Contact details to assist a user in learning more about or engaging with the study.
    /// </summary>
    public List<ContactDetail> Contact { get; set; }
    /// <summary>
    /// A full description of how the study is being conducted.
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// Extension container element for Description
    /// </summary>
    public Element _Description { get; set; }
    /// <summary>
    /// The Group referenced should not generally enumerate specific subjects.  Subjects will be linked to the study using the ResearchSubject resource.
    /// </summary>
    public List<Reference> Enrollment { get; set; }
    /// <summary>
    /// The medication(s), food(s), therapy(ies), device(s) or other concerns or interventions that the study is seeking to gain more information about.
    /// </summary>
    public List<CodeableConcept> Focus { get; set; }
    /// <summary>
    /// Identifiers assigned to this research study by the sponsor or other systems.
    /// </summary>
    public List<Identifier> Identifier { get; set; }
    /// <summary>
    /// Key terms to aid in searching for or filtering the study.
    /// </summary>
    public List<CodeableConcept> Keyword { get; set; }
    /// <summary>
    /// Indicates a country, state or other region where the study is taking place.
    /// </summary>
    public List<CodeableConcept> Location { get; set; }
    /// <summary>
    /// Comments made about the study by the performer, subject or other participants.
    /// </summary>
    public List<Annotation> Note { get; set; }
    /// <summary>
    /// A goal that the study is aiming to achieve in terms of a scientific question to be answered by the analysis of data collected during the study.
    /// </summary>
    public List<ResearchStudyObjective> Objective { get; set; }
    /// <summary>
    /// A larger research study of which this particular study is a component or step.
    /// </summary>
    public List<Reference> PartOf { get; set; }
    /// <summary>
    /// Identifies the start date and the expected (or actual, depending on status) end date for the study.
    /// </summary>
    public Period Period { get; set; }
    /// <summary>
    /// The stage in the progression of a therapy from initial experimental use in humans in clinical trials to post-market evaluation.
    /// </summary>
    public CodeableConcept Phase { get; set; }
    /// <summary>
    /// The type of study based upon the intent of the study's activities. A classification of the intent of the study.
    /// </summary>
    public CodeableConcept PrimaryPurposeType { get; set; }
    /// <summary>
    /// A researcher in a study who oversees multiple aspects of the study, such as concept development, protocol writing, protocol submission for IRB approval, participant recruitment, informed consent, data collection, analysis, interpretation and presentation.
    /// </summary>
    public Reference PrincipalInvestigator { get; set; }
    /// <summary>
    /// The set of steps expected to be performed as part of the execution of the study.
    /// </summary>
    public List<Reference> Protocol { get; set; }
    /// <summary>
    /// A description and/or code explaining the premature termination of the study.
    /// </summary>
    public CodeableConcept ReasonStopped { get; set; }
    /// <summary>
    /// Citations, references and other related documents.
    /// </summary>
    public List<RelatedArtifact> RelatedArtifact { get; set; }
    /// <summary>
    /// A facility in which study activities are conducted.
    /// </summary>
    public List<Reference> Site { get; set; }
    /// <summary>
    /// An organization that initiates the investigation and is legally responsible for the study.
    /// </summary>
    public Reference Sponsor { get; set; }
    /// <summary>
    /// The current state of the study.
    /// </summary>
    public string Status { get; set; }
    /// <summary>
    /// Extension container element for Status
    /// </summary>
    public Element _Status { get; set; }
    /// <summary>
    /// A short, descriptive user-friendly label for the study.
    /// </summary>
    public string Title { get; set; }
    /// <summary>
    /// Extension container element for Title
    /// </summary>
    public Element _Title { get; set; }
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


      ((fhirCsR4.Models.DomainResource)this).SerializeJson(writer, options, false);

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

      if (!string.IsNullOrEmpty(Title))
      {
        writer.WriteString("title", (string)Title!);
      }

      if (_Title != null)
      {
        writer.WritePropertyName("_title");
        _Title.SerializeJson(writer, options);
      }

      if ((Protocol != null) && (Protocol.Count != 0))
      {
        writer.WritePropertyName("protocol");
        writer.WriteStartArray();

        foreach (Reference valProtocol in Protocol)
        {
          valProtocol.SerializeJson(writer, options, true);
        }

        writer.WriteEndArray();
      }

      if ((PartOf != null) && (PartOf.Count != 0))
      {
        writer.WritePropertyName("partOf");
        writer.WriteStartArray();

        foreach (Reference valPartOf in PartOf)
        {
          valPartOf.SerializeJson(writer, options, true);
        }

        writer.WriteEndArray();
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

      if (PrimaryPurposeType != null)
      {
        writer.WritePropertyName("primaryPurposeType");
        PrimaryPurposeType.SerializeJson(writer, options);
      }

      if (Phase != null)
      {
        writer.WritePropertyName("phase");
        Phase.SerializeJson(writer, options);
      }

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

      if ((Focus != null) && (Focus.Count != 0))
      {
        writer.WritePropertyName("focus");
        writer.WriteStartArray();

        foreach (CodeableConcept valFocus in Focus)
        {
          valFocus.SerializeJson(writer, options, true);
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

      if ((Contact != null) && (Contact.Count != 0))
      {
        writer.WritePropertyName("contact");
        writer.WriteStartArray();

        foreach (ContactDetail valContact in Contact)
        {
          valContact.SerializeJson(writer, options, true);
        }

        writer.WriteEndArray();
      }

      if ((RelatedArtifact != null) && (RelatedArtifact.Count != 0))
      {
        writer.WritePropertyName("relatedArtifact");
        writer.WriteStartArray();

        foreach (RelatedArtifact valRelatedArtifact in RelatedArtifact)
        {
          valRelatedArtifact.SerializeJson(writer, options, true);
        }

        writer.WriteEndArray();
      }

      if ((Keyword != null) && (Keyword.Count != 0))
      {
        writer.WritePropertyName("keyword");
        writer.WriteStartArray();

        foreach (CodeableConcept valKeyword in Keyword)
        {
          valKeyword.SerializeJson(writer, options, true);
        }

        writer.WriteEndArray();
      }

      if ((Location != null) && (Location.Count != 0))
      {
        writer.WritePropertyName("location");
        writer.WriteStartArray();

        foreach (CodeableConcept valLocation in Location)
        {
          valLocation.SerializeJson(writer, options, true);
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

      if ((Enrollment != null) && (Enrollment.Count != 0))
      {
        writer.WritePropertyName("enrollment");
        writer.WriteStartArray();

        foreach (Reference valEnrollment in Enrollment)
        {
          valEnrollment.SerializeJson(writer, options, true);
        }

        writer.WriteEndArray();
      }

      if (Period != null)
      {
        writer.WritePropertyName("period");
        Period.SerializeJson(writer, options);
      }

      if (Sponsor != null)
      {
        writer.WritePropertyName("sponsor");
        Sponsor.SerializeJson(writer, options);
      }

      if (PrincipalInvestigator != null)
      {
        writer.WritePropertyName("principalInvestigator");
        PrincipalInvestigator.SerializeJson(writer, options);
      }

      if ((Site != null) && (Site.Count != 0))
      {
        writer.WritePropertyName("site");
        writer.WriteStartArray();

        foreach (Reference valSite in Site)
        {
          valSite.SerializeJson(writer, options, true);
        }

        writer.WriteEndArray();
      }

      if (ReasonStopped != null)
      {
        writer.WritePropertyName("reasonStopped");
        ReasonStopped.SerializeJson(writer, options);
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

      if ((Arm != null) && (Arm.Count != 0))
      {
        writer.WritePropertyName("arm");
        writer.WriteStartArray();

        foreach (ResearchStudyArm valArm in Arm)
        {
          valArm.SerializeJson(writer, options, true);
        }

        writer.WriteEndArray();
      }

      if ((Objective != null) && (Objective.Count != 0))
      {
        writer.WritePropertyName("objective");
        writer.WriteStartArray();

        foreach (ResearchStudyObjective valObjective in Objective)
        {
          valObjective.SerializeJson(writer, options, true);
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
        case "arm":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Arm = new List<ResearchStudyArm>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            fhirCsR4.Models.ResearchStudyArm objArm = new fhirCsR4.Models.ResearchStudyArm();
            objArm.DeserializeJson(ref reader, options);
            Arm.Add(objArm);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (Arm.Count == 0)
          {
            Arm = null;
          }

          break;

        case "category":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Category = new List<CodeableConcept>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            fhirCsR4.Models.CodeableConcept objCategory = new fhirCsR4.Models.CodeableConcept();
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

        case "condition":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Condition = new List<CodeableConcept>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            fhirCsR4.Models.CodeableConcept objCondition = new fhirCsR4.Models.CodeableConcept();
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

        case "contact":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Contact = new List<ContactDetail>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            fhirCsR4.Models.ContactDetail objContact = new fhirCsR4.Models.ContactDetail();
            objContact.DeserializeJson(ref reader, options);
            Contact.Add(objContact);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (Contact.Count == 0)
          {
            Contact = null;
          }

          break;

        case "description":
          Description = reader.GetString();
          break;

        case "_description":
          _Description = new fhirCsR4.Models.Element();
          _Description.DeserializeJson(ref reader, options);
          break;

        case "enrollment":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Enrollment = new List<Reference>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            fhirCsR4.Models.Reference objEnrollment = new fhirCsR4.Models.Reference();
            objEnrollment.DeserializeJson(ref reader, options);
            Enrollment.Add(objEnrollment);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (Enrollment.Count == 0)
          {
            Enrollment = null;
          }

          break;

        case "focus":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Focus = new List<CodeableConcept>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            fhirCsR4.Models.CodeableConcept objFocus = new fhirCsR4.Models.CodeableConcept();
            objFocus.DeserializeJson(ref reader, options);
            Focus.Add(objFocus);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (Focus.Count == 0)
          {
            Focus = null;
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
            fhirCsR4.Models.Identifier objIdentifier = new fhirCsR4.Models.Identifier();
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

        case "keyword":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Keyword = new List<CodeableConcept>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            fhirCsR4.Models.CodeableConcept objKeyword = new fhirCsR4.Models.CodeableConcept();
            objKeyword.DeserializeJson(ref reader, options);
            Keyword.Add(objKeyword);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (Keyword.Count == 0)
          {
            Keyword = null;
          }

          break;

        case "location":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Location = new List<CodeableConcept>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            fhirCsR4.Models.CodeableConcept objLocation = new fhirCsR4.Models.CodeableConcept();
            objLocation.DeserializeJson(ref reader, options);
            Location.Add(objLocation);

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

        case "note":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Note = new List<Annotation>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            fhirCsR4.Models.Annotation objNote = new fhirCsR4.Models.Annotation();
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

        case "objective":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Objective = new List<ResearchStudyObjective>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            fhirCsR4.Models.ResearchStudyObjective objObjective = new fhirCsR4.Models.ResearchStudyObjective();
            objObjective.DeserializeJson(ref reader, options);
            Objective.Add(objObjective);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (Objective.Count == 0)
          {
            Objective = null;
          }

          break;

        case "partOf":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          PartOf = new List<Reference>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            fhirCsR4.Models.Reference objPartOf = new fhirCsR4.Models.Reference();
            objPartOf.DeserializeJson(ref reader, options);
            PartOf.Add(objPartOf);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (PartOf.Count == 0)
          {
            PartOf = null;
          }

          break;

        case "period":
          Period = new fhirCsR4.Models.Period();
          Period.DeserializeJson(ref reader, options);
          break;

        case "phase":
          Phase = new fhirCsR4.Models.CodeableConcept();
          Phase.DeserializeJson(ref reader, options);
          break;

        case "primaryPurposeType":
          PrimaryPurposeType = new fhirCsR4.Models.CodeableConcept();
          PrimaryPurposeType.DeserializeJson(ref reader, options);
          break;

        case "principalInvestigator":
          PrincipalInvestigator = new fhirCsR4.Models.Reference();
          PrincipalInvestigator.DeserializeJson(ref reader, options);
          break;

        case "protocol":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Protocol = new List<Reference>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            fhirCsR4.Models.Reference objProtocol = new fhirCsR4.Models.Reference();
            objProtocol.DeserializeJson(ref reader, options);
            Protocol.Add(objProtocol);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (Protocol.Count == 0)
          {
            Protocol = null;
          }

          break;

        case "reasonStopped":
          ReasonStopped = new fhirCsR4.Models.CodeableConcept();
          ReasonStopped.DeserializeJson(ref reader, options);
          break;

        case "relatedArtifact":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          RelatedArtifact = new List<RelatedArtifact>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            fhirCsR4.Models.RelatedArtifact objRelatedArtifact = new fhirCsR4.Models.RelatedArtifact();
            objRelatedArtifact.DeserializeJson(ref reader, options);
            RelatedArtifact.Add(objRelatedArtifact);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (RelatedArtifact.Count == 0)
          {
            RelatedArtifact = null;
          }

          break;

        case "site":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Site = new List<Reference>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            fhirCsR4.Models.Reference objSite = new fhirCsR4.Models.Reference();
            objSite.DeserializeJson(ref reader, options);
            Site.Add(objSite);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (Site.Count == 0)
          {
            Site = null;
          }

          break;

        case "sponsor":
          Sponsor = new fhirCsR4.Models.Reference();
          Sponsor.DeserializeJson(ref reader, options);
          break;

        case "status":
          Status = reader.GetString();
          break;

        case "_status":
          _Status = new fhirCsR4.Models.Element();
          _Status.DeserializeJson(ref reader, options);
          break;

        case "title":
          Title = reader.GetString();
          break;

        case "_title":
          _Title = new fhirCsR4.Models.Element();
          _Title.DeserializeJson(ref reader, options);
          break;

        default:
          ((fhirCsR4.Models.DomainResource)this).DeserializeJsonProperty(ref reader, options, propertyName);
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
  /// Code Values for the ResearchStudy.status field
  /// </summary>
  public static class ResearchStudyStatusCodes {
    public const string ACTIVE = "active";
    public const string ADMINISTRATIVELY_COMPLETED = "administratively-completed";
    public const string APPROVED = "approved";
    public const string CLOSED_TO_ACCRUAL = "closed-to-accrual";
    public const string CLOSED_TO_ACCRUAL_AND_INTERVENTION = "closed-to-accrual-and-intervention";
    public const string COMPLETED = "completed";
    public const string DISAPPROVED = "disapproved";
    public const string IN_REVIEW = "in-review";
    public const string TEMPORARILY_CLOSED_TO_ACCRUAL = "temporarily-closed-to-accrual";
    public const string TEMPORARILY_CLOSED_TO_ACCRUAL_AND_INTERVENTION = "temporarily-closed-to-accrual-and-intervention";
    public const string WITHDRAWN = "withdrawn";
    public static HashSet<string> Values = new HashSet<string>() {
      "active",
      "administratively-completed",
      "approved",
      "closed-to-accrual",
      "closed-to-accrual-and-intervention",
      "completed",
      "disapproved",
      "in-review",
      "temporarily-closed-to-accrual",
      "temporarily-closed-to-accrual-and-intervention",
      "withdrawn",
    };
  }
}