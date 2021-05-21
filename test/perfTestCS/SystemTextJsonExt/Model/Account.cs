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
  /// JSON Serialization Extensions for Account
  /// </summary>
  public static class AccountJsonExtensions
  {
    /// <summary>
    /// Serialize a FHIR Account into JSON
    /// </summary>
    public static void SerializeJson(this Account current, Utf8JsonWriter writer, JsonSerializerOptions options, bool includeStartObject = true)
    {
      if (includeStartObject) { writer.WriteStartObject(); }
      writer.WriteString("resourceType","Account");
      // Complex: Account, Export: Account, Base: DomainResource (DomainResource)
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

      if (current.Type != null)
      {
        writer.WritePropertyName("type");
        current.Type.SerializeJson(writer, options);
      }

      if ((current.NameElement != null) && (current.NameElement.Value != null))
      {
        writer.WriteString("name",current.NameElement.Value);
      }

      if ((current.Subject != null) && (current.Subject.Count != 0))
      {
        writer.WritePropertyName("subject");
        writer.WriteStartArray();
        foreach (ResourceReference val in current.Subject)
        {
          val.SerializeJson(writer, options, true);
        }
        writer.WriteEndArray();
      }

      if (current.ServicePeriod != null)
      {
        writer.WritePropertyName("servicePeriod");
        current.ServicePeriod.SerializeJson(writer, options);
      }

      if ((current.Coverage != null) && (current.Coverage.Count != 0))
      {
        writer.WritePropertyName("coverage");
        writer.WriteStartArray();
        foreach (Account.CoverageComponent val in current.Coverage)
        {
          val.SerializeJson(writer, options, true);
        }
        writer.WriteEndArray();
      }

      if (current.Owner != null)
      {
        writer.WritePropertyName("owner");
        current.Owner.SerializeJson(writer, options);
      }

      if ((current.DescriptionElement != null) && (current.DescriptionElement.Value != null))
      {
        writer.WriteString("description",current.DescriptionElement.Value);
      }

      if ((current.Guarantor != null) && (current.Guarantor.Count != 0))
      {
        writer.WritePropertyName("guarantor");
        writer.WriteStartArray();
        foreach (Account.GuarantorComponent val in current.Guarantor)
        {
          val.SerializeJson(writer, options, true);
        }
        writer.WriteEndArray();
      }

      if (current.PartOf != null)
      {
        writer.WritePropertyName("partOf");
        current.PartOf.SerializeJson(writer, options);
      }

      if (includeStartObject) { writer.WriteEndObject(); }
    }

    /// <summary>
    /// Deserialize JSON into a FHIR Account
    /// </summary>
    public static void DeserializeJson(this Account current, ref Utf8JsonReader reader, JsonSerializerOptions options)
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
    /// Deserialize JSON into a FHIR Account
    /// </summary>
    public static void DeserializeJsonProperty(this Account current, ref Utf8JsonReader reader, JsonSerializerOptions options, string propertyName)
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
          current.StatusElement =new Code<Hl7.Fhir.Model.Account.AccountStatus>(Hl7.Fhir.Utility.EnumUtility.ParseLiteral<Hl7.Fhir.Model.Account.AccountStatus>(reader.GetString()));
          break;

        case "type":
          current.Type = new Hl7.Fhir.Model.CodeableConcept();
          current.Type.DeserializeJson(ref reader, options);
          break;

        case "name":
          current.NameElement = new FhirString(reader.GetString());
          break;

        case "subject":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          current.Subject = new List<ResourceReference>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Hl7.Fhir.Model.ResourceReference v_Subject = new Hl7.Fhir.Model.ResourceReference();
            v_Subject.DeserializeJson(ref reader, options);
            current.Subject.Add(v_Subject);

            if (!reader.Read())
            {
              throw new JsonException();
            }
            if (reader.TokenType == JsonTokenType.EndObject) { reader.Read(); }
          }

          if (current.Subject.Count == 0)
          {
            current.Subject = null;
          }
          break;

        case "servicePeriod":
          current.ServicePeriod = new Hl7.Fhir.Model.Period();
          current.ServicePeriod.DeserializeJson(ref reader, options);
          break;

        case "coverage":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          current.Coverage = new List<Account.CoverageComponent>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Hl7.Fhir.Model.Account.CoverageComponent v_Coverage = new Hl7.Fhir.Model.Account.CoverageComponent();
            v_Coverage.DeserializeJson(ref reader, options);
            current.Coverage.Add(v_Coverage);

            if (!reader.Read())
            {
              throw new JsonException();
            }
            if (reader.TokenType == JsonTokenType.EndObject) { reader.Read(); }
          }

          if (current.Coverage.Count == 0)
          {
            current.Coverage = null;
          }
          break;

        case "owner":
          current.Owner = new Hl7.Fhir.Model.ResourceReference();
          current.Owner.DeserializeJson(ref reader, options);
          break;

        case "description":
          current.DescriptionElement = new FhirString(reader.GetString());
          break;

        case "guarantor":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          current.Guarantor = new List<Account.GuarantorComponent>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            Hl7.Fhir.Model.Account.GuarantorComponent v_Guarantor = new Hl7.Fhir.Model.Account.GuarantorComponent();
            v_Guarantor.DeserializeJson(ref reader, options);
            current.Guarantor.Add(v_Guarantor);

            if (!reader.Read())
            {
              throw new JsonException();
            }
            if (reader.TokenType == JsonTokenType.EndObject) { reader.Read(); }
          }

          if (current.Guarantor.Count == 0)
          {
            current.Guarantor = null;
          }
          break;

        case "partOf":
          current.PartOf = new Hl7.Fhir.Model.ResourceReference();
          current.PartOf.DeserializeJson(ref reader, options);
          break;

        // Complex: Account, Export: Account, Base: DomainResource
        default:
          ((Hl7.Fhir.Model.DomainResource)current).DeserializeJsonProperty(ref reader, options, propertyName);
          break;
      }
    }

    /// <summary>
    /// Serialize a FHIR Account#Coverage into JSON
    /// </summary>
    public static void SerializeJson(this Account.CoverageComponent current, Utf8JsonWriter writer, JsonSerializerOptions options, bool includeStartObject = true)
    {
      if (includeStartObject) { writer.WriteStartObject(); }
      // Component: Account#Coverage, Export: CoverageComponent, Base: BackboneElement (BackboneElement)
      ((Hl7.Fhir.Model.BackboneElement)current).SerializeJson(writer, options, false);

      writer.WritePropertyName("coverage");
      current.Coverage.SerializeJson(writer, options);

      if ((current.PriorityElement != null) && (current.PriorityElement.Value != null))
      {
        writer.WriteNumber("priority",(int)current.PriorityElement.Value);
      }

      if (includeStartObject) { writer.WriteEndObject(); }
    }

    /// <summary>
    /// Deserialize JSON into a FHIR Account#Coverage
    /// </summary>
    public static void DeserializeJson(this Account.CoverageComponent current, ref Utf8JsonReader reader, JsonSerializerOptions options)
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
    /// Deserialize JSON into a FHIR Account#Coverage
    /// </summary>
    public static void DeserializeJsonProperty(this Account.CoverageComponent current, ref Utf8JsonReader reader, JsonSerializerOptions options, string propertyName)
    {
      switch (propertyName)
      {
        case "coverage":
          current.Coverage = new Hl7.Fhir.Model.ResourceReference();
          current.Coverage.DeserializeJson(ref reader, options);
          break;

        case "priority":
          current.PriorityElement = new PositiveInt(reader.GetInt32());
          break;

        // Complex: coverage, Export: CoverageComponent, Base: BackboneElement
        default:
          ((Hl7.Fhir.Model.BackboneElement)current).DeserializeJsonProperty(ref reader, options, propertyName);
          break;
      }
    }

    /// <summary>
    /// Serialize a FHIR Account#Guarantor into JSON
    /// </summary>
    public static void SerializeJson(this Account.GuarantorComponent current, Utf8JsonWriter writer, JsonSerializerOptions options, bool includeStartObject = true)
    {
      if (includeStartObject) { writer.WriteStartObject(); }
      // Component: Account#Guarantor, Export: GuarantorComponent, Base: BackboneElement (BackboneElement)
      ((Hl7.Fhir.Model.BackboneElement)current).SerializeJson(writer, options, false);

      writer.WritePropertyName("party");
      current.Party.SerializeJson(writer, options);

      if ((current.OnHoldElement != null) && (current.OnHoldElement.Value != null))
      {
        writer.WriteBoolean("onHold",(bool)current.OnHoldElement.Value);
      }

      if (current.Period != null)
      {
        writer.WritePropertyName("period");
        current.Period.SerializeJson(writer, options);
      }

      if (includeStartObject) { writer.WriteEndObject(); }
    }

    /// <summary>
    /// Deserialize JSON into a FHIR Account#Guarantor
    /// </summary>
    public static void DeserializeJson(this Account.GuarantorComponent current, ref Utf8JsonReader reader, JsonSerializerOptions options)
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
    /// Deserialize JSON into a FHIR Account#Guarantor
    /// </summary>
    public static void DeserializeJsonProperty(this Account.GuarantorComponent current, ref Utf8JsonReader reader, JsonSerializerOptions options, string propertyName)
    {
      switch (propertyName)
      {
        case "party":
          current.Party = new Hl7.Fhir.Model.ResourceReference();
          current.Party.DeserializeJson(ref reader, options);
          break;

        case "onHold":
          current.OnHoldElement = new FhirBoolean(reader.GetBoolean());
          break;

        case "period":
          current.Period = new Hl7.Fhir.Model.Period();
          current.Period.DeserializeJson(ref reader, options);
          break;

        // Complex: guarantor, Export: GuarantorComponent, Base: BackboneElement
        default:
          ((Hl7.Fhir.Model.BackboneElement)current).DeserializeJsonProperty(ref reader, options, propertyName);
          break;
      }
    }

    /// <summary>
    /// Resource converter to support Sytem.Text.Json interop.
    /// </summary>
    public class AccountJsonConverter : JsonConverter<Account>
    {
      /// <summary>
      /// Determines whether the specified type can be converted.
      /// </summary>
      public override bool CanConvert(Type objectType) =>
        typeof(Account).IsAssignableFrom(objectType);

      /// <summary>
      /// Writes a specified value as JSON.
      /// </summary>
      public override void Write(Utf8JsonWriter writer, Account value, JsonSerializerOptions options)
      {
        value.SerializeJson(writer, options, true);
        writer.Flush();
      }
      /// <summary>
      /// Reads and converts the JSON to a typed object.
      /// </summary>
      public override Account Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
      {
        Account target = new Account();
        target.DeserializeJson(ref reader, options);
        return target;
      }
    }
  }

}

// end of file
