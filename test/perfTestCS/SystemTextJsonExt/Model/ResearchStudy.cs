// <auto-generated/>
// Contents of: hl7.fhir.r4.core version: 4.0.1

using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Hl7.Fhir.Model;
using Hl7.Fhir.Model.JsonExtensions;
using Hl7.Fhir.Serialization;

/*
  Copyright (c) 2011+, HL7, Inc.
  All rights reserved.
  
  Redistribution and use in source and binary forms, with or without modification, 
  are permitted provided that the following conditions are met:
  
   * Redistributions of source code must retain the above copyright notice, this 
     list of conditions and the following disclaimer.
   * Redistributions in binary form must reproduce the above copyright notice, 
     this list of conditions and the following disclaimer in the documentation 
     and/or other materials provided with the distribution.
   * Neither the name of HL7 nor the names of its contributors may be used to 
     endorse or promote products derived from this software without specific 
     prior written permission.
  
  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
  IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
  INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT 
  NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR 
  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
  WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
  ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
  POSSIBILITY OF SUCH DAMAGE.
  
*/

namespace Hl7.Fhir.Model.JsonExtensions
{
  /// <summary>
  /// JSON Serialization Extensions for ResearchStudy
  /// </summary>
  public static class ResearchStudyJsonExtensions
  {
    /// <summary>
    /// Serialize a FHIR ResearchStudy into JSON
    /// </summary>
    public static void SerializeJson(this ResearchStudy current, Utf8JsonWriter writer, JsonSerializerOptions options, bool includeStartObject = true)
    {
      if (includeStartObject) { writer.WriteStartObject(); }
      writer.WriteString("resourceType","ResearchStudy");
      // Complex: ResearchStudy, Export: ResearchStudy, Base: DomainResource (DomainResource)
      ((Hl7.Fhir.Model.DomainResource)current).SerializeJson(writer, options, false);

      if ((current.Identifier != null) && (current.Identifier.Count != 0))
      {
        writer.WritePropertyName("identifier");
        writer.WriteStartArray();
        foreach (Identifier val in current.Identifier)
        {
          val.SerializeJson(writer, options, true);
        }
        writer.WriteEndArray();
      }

      if ((current.TitleElement != null) && (current.TitleElement.Value != null))
      {
        writer.WriteString("title",current.TitleElement.Value);
      }

      if ((current.Protocol != null) && (current.Protocol.Count != 0))
      {
        writer.WritePropertyName("protocol");
        writer.WriteStartArray();
        foreach (ResourceReference val in current.Protocol)
        {
          val.SerializeJson(writer, options, true);
        }
        writer.WriteEndArray();
      }

      if ((current.PartOf != null) && (current.PartOf.Count != 0))
      {
        writer.WritePropertyName("partOf");
        writer.WriteStartArray();
        foreach (ResourceReference val in current.PartOf)
        {
          val.SerializeJson(writer, options, true);
        }
        writer.WriteEndArray();
      }

      writer.WriteString("status",Hl7.Fhir.Utility.EnumUtility.GetLiteral(current.StatusElement.Value));

      if (current.PrimaryPurposeType != null)
      {
        writer.WritePropertyName("primaryPurposeType");
        current.PrimaryPurposeType.SerializeJson(writer, options);
      }

      if (current.Phase != null)
      {
        writer.WritePropertyName("phase");
        current.Phase.SerializeJson(writer, options);
      }

      if ((current.Category != null) && (current.Category.Count != 0))
      {
        writer.WritePropertyName("category");
        writer.WriteStartArray();
        foreach (CodeableConcept val in current.Category)
        {
          val.SerializeJson(writer, options, true);
        }
        writer.WriteEndArray();
      }

      if ((current.Focus != null) && (current.Focus.Count != 0))
      {
        writer.WritePropertyName("focus");
        writer.WriteStartArray();
        foreach (CodeableConcept val in current.Focus)
        {
          val.SerializeJson(writer, options, true);
        }
        writer.WriteEndArray();
      }

      if ((current.Condition != null) && (current.Condition.Count != 0))
      {
        writer.WritePropertyName("condition");
        writer.WriteStartArray();
        foreach (CodeableConcept val in current.Condition)
        {
          val.SerializeJson(writer, options, true);
        }
        writer.WriteEndArray();
      }

      if ((current.Contact != null) && (current.Contact.Count != 0))
      {
        writer.WritePropertyName("contact");
        writer.WriteStartArray();
        foreach (ContactDetail val in current.Contact)
        {
          val.SerializeJson(writer, options, true);
        }
        writer.WriteEndArray();
      }

      if ((current.RelatedArtifact != null) && (current.RelatedArtifact.Count != 0))
      {
        writer.WritePropertyName("relatedArtifact");
        writer.WriteStartArray();
        foreach (RelatedArtifact val in current.RelatedArtifact)
        {
          val.SerializeJson(writer, options, true);
        }
        writer.WriteEndArray();
      }

      if ((current.Keyword != null) && (current.Keyword.Count != 0))
      {
        writer.WritePropertyName("keyword");
        writer.WriteStartArray();
        foreach (CodeableConcept val in current.Keyword)
        {
          val.SerializeJson(writer, options, true);
        }
        writer.WriteEndArray();
      }

      if ((current.Location != null) && (current.Location.Count != 0))
      {
        writer.WritePropertyName("location");
        writer.WriteStartArray();
        foreach (CodeableConcept val in current.Location)
        {
          val.SerializeJson(writer, options, true);
        }
        writer.WriteEndArray();
      }

      if ((current.Description != null) && (current.Description.Value != null))
      {
        writer.WriteString("description",current.Description.Value);
      }

      if ((current.Enrollment != null) && (current.Enrollment.Count != 0))
      {
        writer.WritePropertyName("enrollment");
        writer.WriteStartArray();
        foreach (ResourceReference val in current.Enrollment)
        {
          val.SerializeJson(writer, options, true);
        }
        writer.WriteEndArray();
      }

      if (current.Period != null)
      {
        writer.WritePropertyName("period");
        current.Period.SerializeJson(writer, options);
      }

      if (current.Sponsor != null)
      {
        writer.WritePropertyName("sponsor");
        current.Sponsor.SerializeJson(writer, options);
      }

      if (current.PrincipalInvestigator != null)
      {
        writer.WritePropertyName("principalInvestigator");
        current.PrincipalInvestigator.SerializeJson(writer, options);
      }

      if ((current.Site != null) && (current.Site.Count != 0))
      {
        writer.WritePropertyName("site");
        writer.WriteStartArray();
        foreach (ResourceReference val in current.Site)
        {
          val.SerializeJson(writer, options, true);
        }
        writer.WriteEndArray();
      }

      if (current.ReasonStopped != null)
      {
        writer.WritePropertyName("reasonStopped");
        current.ReasonStopped.SerializeJson(writer, options);
      }

      if ((current.Note != null) && (current.Note.Count != 0))
      {
        writer.WritePropertyName("note");
        writer.WriteStartArray();
        foreach (Annotation val in current.Note)
        {
          val.SerializeJson(writer, options, true);
        }
        writer.WriteEndArray();
      }

      if ((current.Arm != null) && (current.Arm.Count != 0))
      {
        writer.WritePropertyName("arm");
        writer.WriteStartArray();
        foreach (ResearchStudy.ArmComponent val in current.Arm)
        {
          val.SerializeJson(writer, options, true);
        }
        writer.WriteEndArray();
      }

      if ((current.Objective != null) && (current.Objective.Count != 0))
      {
        writer.WritePropertyName("objective");
        writer.WriteStartArray();
        foreach (ResearchStudy.ObjectiveComponent val in current.Objective)
        {
          val.SerializeJson(writer, options, true);
        }
        writer.WriteEndArray();
      }

      if (includeStartObject) { writer.WriteEndObject(); }
    }

    /// <summary>
    /// Deserialize JSON into a FHIR ResearchStudy
    /// </summary>
    public static void DeserializeJson(this ResearchStudy current, ref Utf8JsonReader reader, JsonSerializerOptions options)
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
          current.DeserializeJsonProperty(ref reader, options, propertyName);
        }
      }

      throw new JsonException();
    }

    /// <summary>
    /// Deserialize JSON into a FHIR ResearchStudy
    /// </summary>
    public static void DeserializeJsonProperty(this ResearchStudy current, ref Utf8JsonReader reader, JsonSerializerOptions options, string propertyName)
    {
      switch (propertyName)
      {
        case "identifier":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          current.Identifier = new List<Identifier>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Hl7.Fhir.Model.Identifier v_Identifier = new Hl7.Fhir.Model.Identifier();
            v_Identifier.DeserializeJson(ref reader, options);
            current.Identifier.Add(v_Identifier);

            if (!reader.Read())
            {
              throw new JsonException();
            }
            if (reader.TokenType == JsonTokenType.EndObject) { reader.Read(); }
          }

          if (current.Identifier.Count == 0)
          {
            current.Identifier = null;
          }
          break;

        case "title":
          current.TitleElement = new FhirString(reader.GetString());
          break;

        case "protocol":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          current.Protocol = new List<ResourceReference>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Hl7.Fhir.Model.ResourceReference v_Protocol = new Hl7.Fhir.Model.ResourceReference();
            v_Protocol.DeserializeJson(ref reader, options);
            current.Protocol.Add(v_Protocol);

            if (!reader.Read())
            {
              throw new JsonException();
            }
            if (reader.TokenType == JsonTokenType.EndObject) { reader.Read(); }
          }

          if (current.Protocol.Count == 0)
          {
            current.Protocol = null;
          }
          break;

        case "partOf":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          current.PartOf = new List<ResourceReference>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Hl7.Fhir.Model.ResourceReference v_PartOf = new Hl7.Fhir.Model.ResourceReference();
            v_PartOf.DeserializeJson(ref reader, options);
            current.PartOf.Add(v_PartOf);

            if (!reader.Read())
            {
              throw new JsonException();
            }
            if (reader.TokenType == JsonTokenType.EndObject) { reader.Read(); }
          }

          if (current.PartOf.Count == 0)
          {
            current.PartOf = null;
          }
          break;

        case "status":
          current.StatusElement =new Code<Hl7.Fhir.Model.ResearchStudy.ResearchStudyStatus>(Hl7.Fhir.Utility.EnumUtility.ParseLiteral<Hl7.Fhir.Model.ResearchStudy.ResearchStudyStatus>(reader.GetString()));
          break;

        case "primaryPurposeType":
          current.PrimaryPurposeType = new Hl7.Fhir.Model.CodeableConcept();
          current.PrimaryPurposeType.DeserializeJson(ref reader, options);
          break;

        case "phase":
          current.Phase = new Hl7.Fhir.Model.CodeableConcept();
          current.Phase.DeserializeJson(ref reader, options);
          break;

        case "category":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          current.Category = new List<CodeableConcept>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Hl7.Fhir.Model.CodeableConcept v_Category = new Hl7.Fhir.Model.CodeableConcept();
            v_Category.DeserializeJson(ref reader, options);
            current.Category.Add(v_Category);

            if (!reader.Read())
            {
              throw new JsonException();
            }
            if (reader.TokenType == JsonTokenType.EndObject) { reader.Read(); }
          }

          if (current.Category.Count == 0)
          {
            current.Category = null;
          }
          break;

        case "focus":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          current.Focus = new List<CodeableConcept>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Hl7.Fhir.Model.CodeableConcept v_Focus = new Hl7.Fhir.Model.CodeableConcept();
            v_Focus.DeserializeJson(ref reader, options);
            current.Focus.Add(v_Focus);

            if (!reader.Read())
            {
              throw new JsonException();
            }
            if (reader.TokenType == JsonTokenType.EndObject) { reader.Read(); }
          }

          if (current.Focus.Count == 0)
          {
            current.Focus = null;
          }
          break;

        case "condition":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          current.Condition = new List<CodeableConcept>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Hl7.Fhir.Model.CodeableConcept v_Condition = new Hl7.Fhir.Model.CodeableConcept();
            v_Condition.DeserializeJson(ref reader, options);
            current.Condition.Add(v_Condition);

            if (!reader.Read())
            {
              throw new JsonException();
            }
            if (reader.TokenType == JsonTokenType.EndObject) { reader.Read(); }
          }

          if (current.Condition.Count == 0)
          {
            current.Condition = null;
          }
          break;

        case "contact":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          current.Contact = new List<ContactDetail>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Hl7.Fhir.Model.ContactDetail v_Contact = new Hl7.Fhir.Model.ContactDetail();
            v_Contact.DeserializeJson(ref reader, options);
            current.Contact.Add(v_Contact);

            if (!reader.Read())
            {
              throw new JsonException();
            }
            if (reader.TokenType == JsonTokenType.EndObject) { reader.Read(); }
          }

          if (current.Contact.Count == 0)
          {
            current.Contact = null;
          }
          break;

        case "relatedArtifact":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          current.RelatedArtifact = new List<RelatedArtifact>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Hl7.Fhir.Model.RelatedArtifact v_RelatedArtifact = new Hl7.Fhir.Model.RelatedArtifact();
            v_RelatedArtifact.DeserializeJson(ref reader, options);
            current.RelatedArtifact.Add(v_RelatedArtifact);

            if (!reader.Read())
            {
              throw new JsonException();
            }
            if (reader.TokenType == JsonTokenType.EndObject) { reader.Read(); }
          }

          if (current.RelatedArtifact.Count == 0)
          {
            current.RelatedArtifact = null;
          }
          break;

        case "keyword":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          current.Keyword = new List<CodeableConcept>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Hl7.Fhir.Model.CodeableConcept v_Keyword = new Hl7.Fhir.Model.CodeableConcept();
            v_Keyword.DeserializeJson(ref reader, options);
            current.Keyword.Add(v_Keyword);

            if (!reader.Read())
            {
              throw new JsonException();
            }
            if (reader.TokenType == JsonTokenType.EndObject) { reader.Read(); }
          }

          if (current.Keyword.Count == 0)
          {
            current.Keyword = null;
          }
          break;

        case "location":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          current.Location = new List<CodeableConcept>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Hl7.Fhir.Model.CodeableConcept v_Location = new Hl7.Fhir.Model.CodeableConcept();
            v_Location.DeserializeJson(ref reader, options);
            current.Location.Add(v_Location);

            if (!reader.Read())
            {
              throw new JsonException();
            }
            if (reader.TokenType == JsonTokenType.EndObject) { reader.Read(); }
          }

          if (current.Location.Count == 0)
          {
            current.Location = null;
          }
          break;

        case "description":
          current.Description = new Markdown(reader.GetString());
          break;

        case "enrollment":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          current.Enrollment = new List<ResourceReference>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Hl7.Fhir.Model.ResourceReference v_Enrollment = new Hl7.Fhir.Model.ResourceReference();
            v_Enrollment.DeserializeJson(ref reader, options);
            current.Enrollment.Add(v_Enrollment);

            if (!reader.Read())
            {
              throw new JsonException();
            }
            if (reader.TokenType == JsonTokenType.EndObject) { reader.Read(); }
          }

          if (current.Enrollment.Count == 0)
          {
            current.Enrollment = null;
          }
          break;

        case "period":
          current.Period = new Hl7.Fhir.Model.Period();
          current.Period.DeserializeJson(ref reader, options);
          break;

        case "sponsor":
          current.Sponsor = new Hl7.Fhir.Model.ResourceReference();
          current.Sponsor.DeserializeJson(ref reader, options);
          break;

        case "principalInvestigator":
          current.PrincipalInvestigator = new Hl7.Fhir.Model.ResourceReference();
          current.PrincipalInvestigator.DeserializeJson(ref reader, options);
          break;

        case "site":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          current.Site = new List<ResourceReference>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Hl7.Fhir.Model.ResourceReference v_Site = new Hl7.Fhir.Model.ResourceReference();
            v_Site.DeserializeJson(ref reader, options);
            current.Site.Add(v_Site);

            if (!reader.Read())
            {
              throw new JsonException();
            }
            if (reader.TokenType == JsonTokenType.EndObject) { reader.Read(); }
          }

          if (current.Site.Count == 0)
          {
            current.Site = null;
          }
          break;

        case "reasonStopped":
          current.ReasonStopped = new Hl7.Fhir.Model.CodeableConcept();
          current.ReasonStopped.DeserializeJson(ref reader, options);
          break;

        case "note":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          current.Note = new List<Annotation>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Hl7.Fhir.Model.Annotation v_Note = new Hl7.Fhir.Model.Annotation();
            v_Note.DeserializeJson(ref reader, options);
            current.Note.Add(v_Note);

            if (!reader.Read())
            {
              throw new JsonException();
            }
            if (reader.TokenType == JsonTokenType.EndObject) { reader.Read(); }
          }

          if (current.Note.Count == 0)
          {
            current.Note = null;
          }
          break;

        case "arm":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          current.Arm = new List<ResearchStudy.ArmComponent>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Hl7.Fhir.Model.ResearchStudy.ArmComponent v_Arm = new Hl7.Fhir.Model.ResearchStudy.ArmComponent();
            v_Arm.DeserializeJson(ref reader, options);
            current.Arm.Add(v_Arm);

            if (!reader.Read())
            {
              throw new JsonException();
            }
            if (reader.TokenType == JsonTokenType.EndObject) { reader.Read(); }
          }

          if (current.Arm.Count == 0)
          {
            current.Arm = null;
          }
          break;

        case "objective":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          current.Objective = new List<ResearchStudy.ObjectiveComponent>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Hl7.Fhir.Model.ResearchStudy.ObjectiveComponent v_Objective = new Hl7.Fhir.Model.ResearchStudy.ObjectiveComponent();
            v_Objective.DeserializeJson(ref reader, options);
            current.Objective.Add(v_Objective);

            if (!reader.Read())
            {
              throw new JsonException();
            }
            if (reader.TokenType == JsonTokenType.EndObject) { reader.Read(); }
          }

          if (current.Objective.Count == 0)
          {
            current.Objective = null;
          }
          break;

        // Complex: ResearchStudy, Export: ResearchStudy, Base: DomainResource
        default:
          ((Hl7.Fhir.Model.DomainResource)current).DeserializeJsonProperty(ref reader, options, propertyName);
          break;
      }
    }

    /// <summary>
    /// Serialize a FHIR ResearchStudy#Arm into JSON
    /// </summary>
    public static void SerializeJson(this ResearchStudy.ArmComponent current, Utf8JsonWriter writer, JsonSerializerOptions options, bool includeStartObject = true)
    {
      if (includeStartObject) { writer.WriteStartObject(); }
      // Component: ResearchStudy#Arm, Export: ArmComponent, Base: BackboneElement (BackboneElement)
      ((Hl7.Fhir.Model.BackboneElement)current).SerializeJson(writer, options, false);

      writer.WriteString("name",current.NameElement.Value);

      if (current.Type != null)
      {
        writer.WritePropertyName("type");
        current.Type.SerializeJson(writer, options);
      }

      if ((current.DescriptionElement != null) && (current.DescriptionElement.Value != null))
      {
        writer.WriteString("description",current.DescriptionElement.Value);
      }

      if (includeStartObject) { writer.WriteEndObject(); }
    }

    /// <summary>
    /// Deserialize JSON into a FHIR ResearchStudy#Arm
    /// </summary>
    public static void DeserializeJson(this ResearchStudy.ArmComponent current, ref Utf8JsonReader reader, JsonSerializerOptions options)
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
          current.DeserializeJsonProperty(ref reader, options, propertyName);
        }
      }

      throw new JsonException();
    }

    /// <summary>
    /// Deserialize JSON into a FHIR ResearchStudy#Arm
    /// </summary>
    public static void DeserializeJsonProperty(this ResearchStudy.ArmComponent current, ref Utf8JsonReader reader, JsonSerializerOptions options, string propertyName)
    {
      switch (propertyName)
      {
        case "name":
          current.NameElement = new FhirString(reader.GetString());
          break;

        case "type":
          current.Type = new Hl7.Fhir.Model.CodeableConcept();
          current.Type.DeserializeJson(ref reader, options);
          break;

        case "description":
          current.DescriptionElement = new FhirString(reader.GetString());
          break;

        // Complex: arm, Export: ArmComponent, Base: BackboneElement
        default:
          ((Hl7.Fhir.Model.BackboneElement)current).DeserializeJsonProperty(ref reader, options, propertyName);
          break;
      }
    }

    /// <summary>
    /// Serialize a FHIR ResearchStudy#Objective into JSON
    /// </summary>
    public static void SerializeJson(this ResearchStudy.ObjectiveComponent current, Utf8JsonWriter writer, JsonSerializerOptions options, bool includeStartObject = true)
    {
      if (includeStartObject) { writer.WriteStartObject(); }
      // Component: ResearchStudy#Objective, Export: ObjectiveComponent, Base: BackboneElement (BackboneElement)
      ((Hl7.Fhir.Model.BackboneElement)current).SerializeJson(writer, options, false);

      if ((current.NameElement != null) && (current.NameElement.Value != null))
      {
        writer.WriteString("name",current.NameElement.Value);
      }

      if (current.Type != null)
      {
        writer.WritePropertyName("type");
        current.Type.SerializeJson(writer, options);
      }

      if (includeStartObject) { writer.WriteEndObject(); }
    }

    /// <summary>
    /// Deserialize JSON into a FHIR ResearchStudy#Objective
    /// </summary>
    public static void DeserializeJson(this ResearchStudy.ObjectiveComponent current, ref Utf8JsonReader reader, JsonSerializerOptions options)
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
          current.DeserializeJsonProperty(ref reader, options, propertyName);
        }
      }

      throw new JsonException();
    }

    /// <summary>
    /// Deserialize JSON into a FHIR ResearchStudy#Objective
    /// </summary>
    public static void DeserializeJsonProperty(this ResearchStudy.ObjectiveComponent current, ref Utf8JsonReader reader, JsonSerializerOptions options, string propertyName)
    {
      switch (propertyName)
      {
        case "name":
          current.NameElement = new FhirString(reader.GetString());
          break;

        case "type":
          current.Type = new Hl7.Fhir.Model.CodeableConcept();
          current.Type.DeserializeJson(ref reader, options);
          break;

        // Complex: objective, Export: ObjectiveComponent, Base: BackboneElement
        default:
          ((Hl7.Fhir.Model.BackboneElement)current).DeserializeJsonProperty(ref reader, options, propertyName);
          break;
      }
    }

    /// <summary>
    /// Resource converter to support Sytem.Text.Json interop.
    /// </summary>
    public class ResearchStudyJsonConverter : JsonConverter<ResearchStudy>
    {
      /// <summary>
      /// Determines whether the specified type can be converted.
      /// </summary>
      public override bool CanConvert(Type objectType) =>
        typeof(ResearchStudy).IsAssignableFrom(objectType);

      /// <summary>
      /// Writes a specified value as JSON.
      /// </summary>
      public override void Write(Utf8JsonWriter writer, ResearchStudy value, JsonSerializerOptions options)
      {
        value.SerializeJson(writer, options, true);
        writer.Flush();
      }
      /// <summary>
      /// Reads and converts the JSON to a typed object.
      /// </summary>
      public override ResearchStudy Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
      {
        ResearchStudy target = new ResearchStudy();
        target.DeserializeJson(ref reader, options);
        return target;
      }
    }
  }

}

// end of file