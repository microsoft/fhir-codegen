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
  /// JSON Serialization Extensions for CoverageEligibilityRequest
  /// </summary>
  public static class CoverageEligibilityRequestJsonExtensions
  {
    /// <summary>
    /// Serialize a FHIR CoverageEligibilityRequest into JSON
    /// </summary>
    public static void SerializeJson(this CoverageEligibilityRequest current, Utf8JsonWriter writer, JsonSerializerOptions options, bool includeStartObject = true)
    {
      if (includeStartObject) { writer.WriteStartObject(); }
      writer.WriteString("resourceType","CoverageEligibilityRequest");
      // Complex: CoverageEligibilityRequest, Export: CoverageEligibilityRequest, Base: DomainResource (DomainResource)
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

      writer.WriteString("status",Hl7.Fhir.Utility.EnumUtility.GetLiteral(current.StatusElement.Value));

      if (current.Priority != null)
      {
        writer.WritePropertyName("priority");
        current.Priority.SerializeJson(writer, options);
      }

      if ((current.PurposeElement != null) && (current.PurposeElement.Count != 0))
      {
        writer.WritePropertyName("purpose");
        writer.WriteStartArray();
        foreach (Code<Hl7.Fhir.Model.CoverageEligibilityRequest.EligibilityRequestPurpose> val in current.PurposeElement)
        {
          writer.WriteStringValue(Hl7.Fhir.Utility.EnumUtility.GetLiteral(val.Value));
        }
        writer.WriteEndArray();
      }

      writer.WritePropertyName("patient");
      current.Patient.SerializeJson(writer, options);

      if (current.Serviced != null)
      {
        switch (current.Serviced)
        {
          case Date v_Date:
            writer.WriteString("servicedDate",v_Date.Value);
            break;
          case Period v_Period:
            writer.WritePropertyName("servicedPeriod");
            v_Period.SerializeJson(writer, options);
            break;
        }
      }
      writer.WriteString("created",current.CreatedElement.Value);

      if (current.Enterer != null)
      {
        writer.WritePropertyName("enterer");
        current.Enterer.SerializeJson(writer, options);
      }

      if (current.Provider != null)
      {
        writer.WritePropertyName("provider");
        current.Provider.SerializeJson(writer, options);
      }

      writer.WritePropertyName("insurer");
      current.Insurer.SerializeJson(writer, options);

      if (current.Facility != null)
      {
        writer.WritePropertyName("facility");
        current.Facility.SerializeJson(writer, options);
      }

      if ((current.SupportingInfo != null) && (current.SupportingInfo.Count != 0))
      {
        writer.WritePropertyName("supportingInfo");
        writer.WriteStartArray();
        foreach (CoverageEligibilityRequest.SupportingInformationComponent val in current.SupportingInfo)
        {
          val.SerializeJson(writer, options, true);
        }
        writer.WriteEndArray();
      }

      if ((current.Insurance != null) && (current.Insurance.Count != 0))
      {
        writer.WritePropertyName("insurance");
        writer.WriteStartArray();
        foreach (CoverageEligibilityRequest.InsuranceComponent val in current.Insurance)
        {
          val.SerializeJson(writer, options, true);
        }
        writer.WriteEndArray();
      }

      if ((current.Item != null) && (current.Item.Count != 0))
      {
        writer.WritePropertyName("item");
        writer.WriteStartArray();
        foreach (CoverageEligibilityRequest.DetailsComponent val in current.Item)
        {
          val.SerializeJson(writer, options, true);
        }
        writer.WriteEndArray();
      }

      if (includeStartObject) { writer.WriteEndObject(); }
    }

    /// <summary>
    /// Deserialize JSON into a FHIR CoverageEligibilityRequest
    /// </summary>
    public static void DeserializeJson(this CoverageEligibilityRequest current, ref Utf8JsonReader reader, JsonSerializerOptions options)
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
    /// Deserialize JSON into a FHIR CoverageEligibilityRequest
    /// </summary>
    public static void DeserializeJsonProperty(this CoverageEligibilityRequest current, ref Utf8JsonReader reader, JsonSerializerOptions options, string propertyName)
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

        case "status":
          current.StatusElement =new Code<Hl7.Fhir.Model.FinancialResourceStatusCodes>(Hl7.Fhir.Utility.EnumUtility.ParseLiteral<Hl7.Fhir.Model.FinancialResourceStatusCodes>(reader.GetString()));
          break;

        case "priority":
          current.Priority = new Hl7.Fhir.Model.CodeableConcept();
          current.Priority.DeserializeJson(ref reader, options);
          break;

        case "purpose":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          current.PurposeElement = new List<Code<Hl7.Fhir.Model.CoverageEligibilityRequest.EligibilityRequestPurpose>>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            current.PurposeElement.Add(new Code<Hl7.Fhir.Model.CoverageEligibilityRequest.EligibilityRequestPurpose>(Hl7.Fhir.Utility.EnumUtility.ParseLiteral<Hl7.Fhir.Model.CoverageEligibilityRequest.EligibilityRequestPurpose>(reader.GetString())));

            if (!reader.Read())
            {
              throw new JsonException();
            }
            if (reader.TokenType == JsonTokenType.EndObject) { reader.Read(); }
          }

          if (current.PurposeElement.Count == 0)
          {
            current.PurposeElement = null;
          }
          break;

        case "patient":
          current.Patient = new Hl7.Fhir.Model.ResourceReference();
          current.Patient.DeserializeJson(ref reader, options);
          break;

        case "servicedDate":
          current.Serviced = new Date(reader.GetString());
          break;

        case "servicedPeriod":
          current.Serviced = new Hl7.Fhir.Model.Period();
          current.Serviced.DeserializeJson(ref reader, options);
          break;

        case "created":
          current.CreatedElement = new FhirDateTime(reader.GetString());
          break;

        case "enterer":
          current.Enterer = new Hl7.Fhir.Model.ResourceReference();
          current.Enterer.DeserializeJson(ref reader, options);
          break;

        case "provider":
          current.Provider = new Hl7.Fhir.Model.ResourceReference();
          current.Provider.DeserializeJson(ref reader, options);
          break;

        case "insurer":
          current.Insurer = new Hl7.Fhir.Model.ResourceReference();
          current.Insurer.DeserializeJson(ref reader, options);
          break;

        case "facility":
          current.Facility = new Hl7.Fhir.Model.ResourceReference();
          current.Facility.DeserializeJson(ref reader, options);
          break;

        case "supportingInfo":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          current.SupportingInfo = new List<CoverageEligibilityRequest.SupportingInformationComponent>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Hl7.Fhir.Model.CoverageEligibilityRequest.SupportingInformationComponent v_SupportingInfo = new Hl7.Fhir.Model.CoverageEligibilityRequest.SupportingInformationComponent();
            v_SupportingInfo.DeserializeJson(ref reader, options);
            current.SupportingInfo.Add(v_SupportingInfo);

            if (!reader.Read())
            {
              throw new JsonException();
            }
            if (reader.TokenType == JsonTokenType.EndObject) { reader.Read(); }
          }

          if (current.SupportingInfo.Count == 0)
          {
            current.SupportingInfo = null;
          }
          break;

        case "insurance":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          current.Insurance = new List<CoverageEligibilityRequest.InsuranceComponent>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Hl7.Fhir.Model.CoverageEligibilityRequest.InsuranceComponent v_Insurance = new Hl7.Fhir.Model.CoverageEligibilityRequest.InsuranceComponent();
            v_Insurance.DeserializeJson(ref reader, options);
            current.Insurance.Add(v_Insurance);

            if (!reader.Read())
            {
              throw new JsonException();
            }
            if (reader.TokenType == JsonTokenType.EndObject) { reader.Read(); }
          }

          if (current.Insurance.Count == 0)
          {
            current.Insurance = null;
          }
          break;

        case "item":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          current.Item = new List<CoverageEligibilityRequest.DetailsComponent>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Hl7.Fhir.Model.CoverageEligibilityRequest.DetailsComponent v_Item = new Hl7.Fhir.Model.CoverageEligibilityRequest.DetailsComponent();
            v_Item.DeserializeJson(ref reader, options);
            current.Item.Add(v_Item);

            if (!reader.Read())
            {
              throw new JsonException();
            }
            if (reader.TokenType == JsonTokenType.EndObject) { reader.Read(); }
          }

          if (current.Item.Count == 0)
          {
            current.Item = null;
          }
          break;

        // Complex: CoverageEligibilityRequest, Export: CoverageEligibilityRequest, Base: DomainResource
        default:
          ((Hl7.Fhir.Model.DomainResource)current).DeserializeJsonProperty(ref reader, options, propertyName);
          break;
      }
    }

    /// <summary>
    /// Serialize a FHIR CoverageEligibilityRequest#SupportingInformation into JSON
    /// </summary>
    public static void SerializeJson(this CoverageEligibilityRequest.SupportingInformationComponent current, Utf8JsonWriter writer, JsonSerializerOptions options, bool includeStartObject = true)
    {
      if (includeStartObject) { writer.WriteStartObject(); }
      // Component: CoverageEligibilityRequest#SupportingInformation, Export: SupportingInformationComponent, Base: BackboneElement (BackboneElement)
      ((Hl7.Fhir.Model.BackboneElement)current).SerializeJson(writer, options, false);

      writer.WriteNumber("sequence",(int)current.SequenceElement.Value);

      writer.WritePropertyName("information");
      current.Information.SerializeJson(writer, options);

      if ((current.AppliesToAllElement != null) && (current.AppliesToAllElement.Value != null))
      {
        writer.WriteBoolean("appliesToAll",(bool)current.AppliesToAllElement.Value);
      }

      if (includeStartObject) { writer.WriteEndObject(); }
    }

    /// <summary>
    /// Deserialize JSON into a FHIR CoverageEligibilityRequest#SupportingInformation
    /// </summary>
    public static void DeserializeJson(this CoverageEligibilityRequest.SupportingInformationComponent current, ref Utf8JsonReader reader, JsonSerializerOptions options)
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
    /// Deserialize JSON into a FHIR CoverageEligibilityRequest#SupportingInformation
    /// </summary>
    public static void DeserializeJsonProperty(this CoverageEligibilityRequest.SupportingInformationComponent current, ref Utf8JsonReader reader, JsonSerializerOptions options, string propertyName)
    {
      switch (propertyName)
      {
        case "sequence":
          current.SequenceElement = new PositiveInt(reader.GetInt32());
          break;

        case "information":
          current.Information = new Hl7.Fhir.Model.ResourceReference();
          current.Information.DeserializeJson(ref reader, options);
          break;

        case "appliesToAll":
          current.AppliesToAllElement = new FhirBoolean(reader.GetBoolean());
          break;

        // Complex: supportingInfo, Export: SupportingInformationComponent, Base: BackboneElement
        default:
          ((Hl7.Fhir.Model.BackboneElement)current).DeserializeJsonProperty(ref reader, options, propertyName);
          break;
      }
    }

    /// <summary>
    /// Serialize a FHIR CoverageEligibilityRequest#Insurance into JSON
    /// </summary>
    public static void SerializeJson(this CoverageEligibilityRequest.InsuranceComponent current, Utf8JsonWriter writer, JsonSerializerOptions options, bool includeStartObject = true)
    {
      if (includeStartObject) { writer.WriteStartObject(); }
      // Component: CoverageEligibilityRequest#Insurance, Export: InsuranceComponent, Base: BackboneElement (BackboneElement)
      ((Hl7.Fhir.Model.BackboneElement)current).SerializeJson(writer, options, false);

      if ((current.FocalElement != null) && (current.FocalElement.Value != null))
      {
        writer.WriteBoolean("focal",(bool)current.FocalElement.Value);
      }

      writer.WritePropertyName("coverage");
      current.Coverage.SerializeJson(writer, options);

      if ((current.BusinessArrangementElement != null) && (current.BusinessArrangementElement.Value != null))
      {
        writer.WriteString("businessArrangement",current.BusinessArrangementElement.Value);
      }

      if (includeStartObject) { writer.WriteEndObject(); }
    }

    /// <summary>
    /// Deserialize JSON into a FHIR CoverageEligibilityRequest#Insurance
    /// </summary>
    public static void DeserializeJson(this CoverageEligibilityRequest.InsuranceComponent current, ref Utf8JsonReader reader, JsonSerializerOptions options)
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
    /// Deserialize JSON into a FHIR CoverageEligibilityRequest#Insurance
    /// </summary>
    public static void DeserializeJsonProperty(this CoverageEligibilityRequest.InsuranceComponent current, ref Utf8JsonReader reader, JsonSerializerOptions options, string propertyName)
    {
      switch (propertyName)
      {
        case "focal":
          current.FocalElement = new FhirBoolean(reader.GetBoolean());
          break;

        case "coverage":
          current.Coverage = new Hl7.Fhir.Model.ResourceReference();
          current.Coverage.DeserializeJson(ref reader, options);
          break;

        case "businessArrangement":
          current.BusinessArrangementElement = new FhirString(reader.GetString());
          break;

        // Complex: insurance, Export: InsuranceComponent, Base: BackboneElement
        default:
          ((Hl7.Fhir.Model.BackboneElement)current).DeserializeJsonProperty(ref reader, options, propertyName);
          break;
      }
    }

    /// <summary>
    /// Serialize a FHIR CoverageEligibilityRequest#Details into JSON
    /// </summary>
    public static void SerializeJson(this CoverageEligibilityRequest.DetailsComponent current, Utf8JsonWriter writer, JsonSerializerOptions options, bool includeStartObject = true)
    {
      if (includeStartObject) { writer.WriteStartObject(); }
      // Component: CoverageEligibilityRequest#Details, Export: DetailsComponent, Base: BackboneElement (BackboneElement)
      ((Hl7.Fhir.Model.BackboneElement)current).SerializeJson(writer, options, false);

      if ((current.SupportingInfoSequenceElement != null) && (current.SupportingInfoSequenceElement.Count != 0))
      {
        writer.WritePropertyName("supportingInfoSequence");
        writer.WriteStartArray();
        foreach (PositiveInt val in current.SupportingInfoSequenceElement)
        {
          writer.WriteNumberValue((int)val.Value);
        }
        writer.WriteEndArray();
      }

      if (current.Category != null)
      {
        writer.WritePropertyName("category");
        current.Category.SerializeJson(writer, options);
      }

      if (current.ProductOrService != null)
      {
        writer.WritePropertyName("productOrService");
        current.ProductOrService.SerializeJson(writer, options);
      }

      if ((current.Modifier != null) && (current.Modifier.Count != 0))
      {
        writer.WritePropertyName("modifier");
        writer.WriteStartArray();
        foreach (CodeableConcept val in current.Modifier)
        {
          val.SerializeJson(writer, options, true);
        }
        writer.WriteEndArray();
      }

      if (current.Provider != null)
      {
        writer.WritePropertyName("provider");
        current.Provider.SerializeJson(writer, options);
      }

      if (current.Quantity != null)
      {
        writer.WritePropertyName("quantity");
        current.Quantity.SerializeJson(writer, options);
      }

      if (current.UnitPrice != null)
      {
        writer.WritePropertyName("unitPrice");
        current.UnitPrice.SerializeJson(writer, options);
      }

      if (current.Facility != null)
      {
        writer.WritePropertyName("facility");
        current.Facility.SerializeJson(writer, options);
      }

      if ((current.Diagnosis != null) && (current.Diagnosis.Count != 0))
      {
        writer.WritePropertyName("diagnosis");
        writer.WriteStartArray();
        foreach (CoverageEligibilityRequest.DiagnosisComponent val in current.Diagnosis)
        {
          val.SerializeJson(writer, options, true);
        }
        writer.WriteEndArray();
      }

      if ((current.Detail != null) && (current.Detail.Count != 0))
      {
        writer.WritePropertyName("detail");
        writer.WriteStartArray();
        foreach (ResourceReference val in current.Detail)
        {
          val.SerializeJson(writer, options, true);
        }
        writer.WriteEndArray();
      }

      if (includeStartObject) { writer.WriteEndObject(); }
    }

    /// <summary>
    /// Deserialize JSON into a FHIR CoverageEligibilityRequest#Details
    /// </summary>
    public static void DeserializeJson(this CoverageEligibilityRequest.DetailsComponent current, ref Utf8JsonReader reader, JsonSerializerOptions options)
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
    /// Deserialize JSON into a FHIR CoverageEligibilityRequest#Details
    /// </summary>
    public static void DeserializeJsonProperty(this CoverageEligibilityRequest.DetailsComponent current, ref Utf8JsonReader reader, JsonSerializerOptions options, string propertyName)
    {
      switch (propertyName)
      {
        case "supportingInfoSequence":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          current.SupportingInfoSequenceElement = new List<PositiveInt>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            current.SupportingInfoSequenceElement.Add(new PositiveInt(reader.GetInt32()));

            if (!reader.Read())
            {
              throw new JsonException();
            }
            if (reader.TokenType == JsonTokenType.EndObject) { reader.Read(); }
          }

          if (current.SupportingInfoSequenceElement.Count == 0)
          {
            current.SupportingInfoSequenceElement = null;
          }
          break;

        case "category":
          current.Category = new Hl7.Fhir.Model.CodeableConcept();
          current.Category.DeserializeJson(ref reader, options);
          break;

        case "productOrService":
          current.ProductOrService = new Hl7.Fhir.Model.CodeableConcept();
          current.ProductOrService.DeserializeJson(ref reader, options);
          break;

        case "modifier":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          current.Modifier = new List<CodeableConcept>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Hl7.Fhir.Model.CodeableConcept v_Modifier = new Hl7.Fhir.Model.CodeableConcept();
            v_Modifier.DeserializeJson(ref reader, options);
            current.Modifier.Add(v_Modifier);

            if (!reader.Read())
            {
              throw new JsonException();
            }
            if (reader.TokenType == JsonTokenType.EndObject) { reader.Read(); }
          }

          if (current.Modifier.Count == 0)
          {
            current.Modifier = null;
          }
          break;

        case "provider":
          current.Provider = new Hl7.Fhir.Model.ResourceReference();
          current.Provider.DeserializeJson(ref reader, options);
          break;

        case "quantity":
          current.Quantity = new Hl7.Fhir.Model.Quantity();
          current.Quantity.DeserializeJson(ref reader, options);
          break;

        case "unitPrice":
          current.UnitPrice = new Hl7.Fhir.Model.Money();
          current.UnitPrice.DeserializeJson(ref reader, options);
          break;

        case "facility":
          current.Facility = new Hl7.Fhir.Model.ResourceReference();
          current.Facility.DeserializeJson(ref reader, options);
          break;

        case "diagnosis":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          current.Diagnosis = new List<CoverageEligibilityRequest.DiagnosisComponent>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Hl7.Fhir.Model.CoverageEligibilityRequest.DiagnosisComponent v_Diagnosis = new Hl7.Fhir.Model.CoverageEligibilityRequest.DiagnosisComponent();
            v_Diagnosis.DeserializeJson(ref reader, options);
            current.Diagnosis.Add(v_Diagnosis);

            if (!reader.Read())
            {
              throw new JsonException();
            }
            if (reader.TokenType == JsonTokenType.EndObject) { reader.Read(); }
          }

          if (current.Diagnosis.Count == 0)
          {
            current.Diagnosis = null;
          }
          break;

        case "detail":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          current.Detail = new List<ResourceReference>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Hl7.Fhir.Model.ResourceReference v_Detail = new Hl7.Fhir.Model.ResourceReference();
            v_Detail.DeserializeJson(ref reader, options);
            current.Detail.Add(v_Detail);

            if (!reader.Read())
            {
              throw new JsonException();
            }
            if (reader.TokenType == JsonTokenType.EndObject) { reader.Read(); }
          }

          if (current.Detail.Count == 0)
          {
            current.Detail = null;
          }
          break;

        // Complex: item, Export: DetailsComponent, Base: BackboneElement
        default:
          ((Hl7.Fhir.Model.BackboneElement)current).DeserializeJsonProperty(ref reader, options, propertyName);
          break;
      }
    }

    /// <summary>
    /// Serialize a FHIR CoverageEligibilityRequest#Diagnosis into JSON
    /// </summary>
    public static void SerializeJson(this CoverageEligibilityRequest.DiagnosisComponent current, Utf8JsonWriter writer, JsonSerializerOptions options, bool includeStartObject = true)
    {
      if (includeStartObject) { writer.WriteStartObject(); }
      // Component: CoverageEligibilityRequest#Diagnosis, Export: DiagnosisComponent, Base: BackboneElement (BackboneElement)
      ((Hl7.Fhir.Model.BackboneElement)current).SerializeJson(writer, options, false);

      if (current.Diagnosis != null)
      {
        switch (current.Diagnosis)
        {
          case CodeableConcept v_CodeableConcept:
            writer.WritePropertyName("diagnosisCodeableConcept");
            v_CodeableConcept.SerializeJson(writer, options);
            break;
          case ResourceReference v_ResourceReference:
            writer.WritePropertyName("diagnosisReference");
            v_ResourceReference.SerializeJson(writer, options);
            break;
        }
      }
      if (includeStartObject) { writer.WriteEndObject(); }
    }

    /// <summary>
    /// Deserialize JSON into a FHIR CoverageEligibilityRequest#Diagnosis
    /// </summary>
    public static void DeserializeJson(this CoverageEligibilityRequest.DiagnosisComponent current, ref Utf8JsonReader reader, JsonSerializerOptions options)
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
    /// Deserialize JSON into a FHIR CoverageEligibilityRequest#Diagnosis
    /// </summary>
    public static void DeserializeJsonProperty(this CoverageEligibilityRequest.DiagnosisComponent current, ref Utf8JsonReader reader, JsonSerializerOptions options, string propertyName)
    {
      switch (propertyName)
      {
        case "diagnosisCodeableConcept":
          current.Diagnosis = new Hl7.Fhir.Model.CodeableConcept();
          current.Diagnosis.DeserializeJson(ref reader, options);
          break;

        case "diagnosisReference":
          current.Diagnosis = new Hl7.Fhir.Model.ResourceReference();
          current.Diagnosis.DeserializeJson(ref reader, options);
          break;

        // Complex: diagnosis, Export: DiagnosisComponent, Base: BackboneElement
        default:
          ((Hl7.Fhir.Model.BackboneElement)current).DeserializeJsonProperty(ref reader, options, propertyName);
          break;
      }
    }

    /// <summary>
    /// Resource converter to support Sytem.Text.Json interop.
    /// </summary>
    public class CoverageEligibilityRequestJsonConverter : JsonConverter<CoverageEligibilityRequest>
    {
      /// <summary>
      /// Determines whether the specified type can be converted.
      /// </summary>
      public override bool CanConvert(Type objectType) =>
        typeof(CoverageEligibilityRequest).IsAssignableFrom(objectType);

      /// <summary>
      /// Writes a specified value as JSON.
      /// </summary>
      public override void Write(Utf8JsonWriter writer, CoverageEligibilityRequest value, JsonSerializerOptions options)
      {
        value.SerializeJson(writer, options, true);
        writer.Flush();
      }
      /// <summary>
      /// Reads and converts the JSON to a typed object.
      /// </summary>
      public override CoverageEligibilityRequest Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
      {
        CoverageEligibilityRequest target = new CoverageEligibilityRequest();
        target.DeserializeJson(ref reader, options);
        return target;
      }
    }
  }

}

// end of file