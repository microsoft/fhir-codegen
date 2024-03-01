// <copyright file="ValueSetConverter.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using System.Net.NetworkInformation;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGen.CrossVersion.Common;

namespace Microsoft.Health.Fhir.CodeGen.CrossVersion.R4B;

public class ValueSetReader : BaseParser<ValueSet>
{
    private static ValueSetReader _instance = new();
    public static ValueSetReader Instance => _instance;

    override public void ReadProperty(ref Utf8JsonReader reader, JsonSerializerOptions options, ValueSet o, string propertyName)
    {
        switch (propertyName)
        {
            //case "compose":
            //    o.Compose = new();
            //    Compose.DeserializeJson(ref reader, options);
            //    break;

            //case "contact":
            //    if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
            //    {
            //        throw new JsonException();
            //    }

            //    Contact = new List<ContactDetail>();

            //    while (reader.TokenType != JsonTokenType.EndArray)
            //    {
            //        fhirCsR4B.Models.ContactDetail objContact = new fhirCsR4B.Models.ContactDetail();
            //        objContact.DeserializeJson(ref reader, options);
            //        Contact.Add(objContact);

            //        if (!reader.Read())
            //        {
            //            throw new JsonException();
            //        }
            //    }

            //    if (Contact.Count == 0)
            //    {
            //        Contact = null;
            //    }

            //    break;

            //case "copyright":
            //    Copyright = reader.GetString();
            //    break;

            //case "_copyright":
            //    _Copyright = new fhirCsR4B.Models.Element();
            //    _Copyright.DeserializeJson(ref reader, options);
            //    break;

            //case "date":
            //    Date = reader.GetString();
            //    break;

            //case "_date":
            //    _Date = new fhirCsR4B.Models.Element();
            //    _Date.DeserializeJson(ref reader, options);
            //    break;

            //case "description":
            //    Description = reader.GetString();
            //    break;

            //case "_description":
            //    _Description = new fhirCsR4B.Models.Element();
            //    _Description.DeserializeJson(ref reader, options);
            //    break;

            //case "expansion":
            //    Expansion = new fhirCsR4B.Models.ValueSetExpansion();
            //    Expansion.DeserializeJson(ref reader, options);
            //    break;

            //case "experimental":
            //    Experimental = reader.GetBoolean();
            //    break;

            //case "_experimental":
            //    _Experimental = new fhirCsR4B.Models.Element();
            //    _Experimental.DeserializeJson(ref reader, options);
            //    break;

            //case "identifier":
            //    if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
            //    {
            //        throw new JsonException();
            //    }

            //    Identifier = new List<Identifier>();

            //    while (reader.TokenType != JsonTokenType.EndArray)
            //    {
            //        fhirCsR4B.Models.Identifier objIdentifier = new fhirCsR4B.Models.Identifier();
            //        objIdentifier.DeserializeJson(ref reader, options);
            //        Identifier.Add(objIdentifier);

            //        if (!reader.Read())
            //        {
            //            throw new JsonException();
            //        }
            //    }

            //    if (Identifier.Count == 0)
            //    {
            //        Identifier = null;
            //    }

            //    break;

            //case "immutable":
            //    Immutable = reader.GetBoolean();
            //    break;

            //case "_immutable":
            //    _Immutable = new fhirCsR4B.Models.Element();
            //    _Immutable.DeserializeJson(ref reader, options);
            //    break;

            //case "jurisdiction":
            //    if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
            //    {
            //        throw new JsonException();
            //    }

            //    Jurisdiction = new List<CodeableConcept>();

            //    while (reader.TokenType != JsonTokenType.EndArray)
            //    {
            //        fhirCsR4B.Models.CodeableConcept objJurisdiction = new fhirCsR4B.Models.CodeableConcept();
            //        objJurisdiction.DeserializeJson(ref reader, options);
            //        Jurisdiction.Add(objJurisdiction);

            //        if (!reader.Read())
            //        {
            //            throw new JsonException();
            //        }
            //    }

            //    if (Jurisdiction.Count == 0)
            //    {
            //        Jurisdiction = null;
            //    }

            //    break;

            case "name":
                o.Name = reader.GetString();
                break;

            //case "_name":
            //    _Name = new fhirCsR4B.Models.Element();
            //    _Name.DeserializeJson(ref reader, options);
            //    break;

            //case "publisher":
            //    Publisher = reader.GetString();
            //    break;

            //case "_publisher":
            //    _Publisher = new fhirCsR4B.Models.Element();
            //    _Publisher.DeserializeJson(ref reader, options);
            //    break;

            //case "purpose":
            //    Purpose = reader.GetString();
            //    break;

            //case "_purpose":
            //    _Purpose = new fhirCsR4B.Models.Element();
            //    _Purpose.DeserializeJson(ref reader, options);
            //    break;

            //case "status":
            //    Status = reader.GetString();
            //    break;

            //case "_status":
            //    _Status = new fhirCsR4B.Models.Element();
            //    _Status.DeserializeJson(ref reader, options);
            //    break;

            //case "title":
            //    Title = reader.GetString();
            //    break;

            //case "_title":
            //    _Title = new fhirCsR4B.Models.Element();
            //    _Title.DeserializeJson(ref reader, options);
            //    break;

            case "url":
                o.Url = reader.GetString();
                break;

            //case "_url":
            //    _Url = new fhirCsR4B.Models.Element();
            //    _Url.DeserializeJson(ref reader, options);
            //    break;

            //case "useContext":
            //    if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
            //    {
            //        throw new JsonException();
            //    }

            //    UseContext = new List<UsageContext>();

            //    while (reader.TokenType != JsonTokenType.EndArray)
            //    {
            //        fhirCsR4B.Models.UsageContext objUseContext = new fhirCsR4B.Models.UsageContext();
            //        objUseContext.DeserializeJson(ref reader, options);
            //        UseContext.Add(objUseContext);

            //        if (!reader.Read())
            //        {
            //            throw new JsonException();
            //        }
            //    }

            //    if (UseContext.Count == 0)
            //    {
            //        UseContext = null;
            //    }

            //    break;

            case "version":
                o.Version = reader.GetString();
                break;

            //case "_version":
            //    _Version = new fhirCsR4B.Models.Element();
            //    _Version.DeserializeJson(ref reader, options);
            //    break;

            default:
                DomainResourceReader.Instance.ReadProperty(ref reader, options, o, propertyName);
                break;
        }
    }
}
