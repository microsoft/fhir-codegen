// <copyright file="MetaReader.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.Language.Debugging;
using Hl7.Fhir.Model;
using Hl7.Fhir.Support;
using Microsoft.Health.Fhir.CodeGen.CrossVersion.Common;
using System;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Microsoft.Health.Fhir.CodeGen.CrossVersion.R4B;

public class MetaReader : BaseParser<Meta>
{
    private static MetaReader _instance = new();
    public static MetaReader Instance => _instance;

    override public void ReadProperty(ref Utf8JsonReader reader, JsonSerializerOptions options, Meta o, string propertyName)
    {
        switch (propertyName)
        {
            case "lastUpdated":
                {
                    (object? result, Hl7.Fhir.Serialization.FhirJsonException? error) = PrimitiveReader.DeserializePrimitiveValue(ref reader, typeof(DateTimeOffset), typeof(FhirDateTime));
                    if (error == null)
                    {
                        o.LastUpdatedElement.ObjectValue = result;
                    }
                }

                break;

            //case "_lastUpdated":
            //    _LastUpdated = new fhirCsR4B.Models.Element();
            //    _LastUpdated.DeserializeJson(ref reader, options);
            //    break;

            //case "profile":
            //    if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
            //    {
            //        throw new JsonException();
            //    }

            //    Profile = new List<string>();

            //    while (reader.TokenType != JsonTokenType.EndArray)
            //    {
            //        Profile.Add(reader.GetString());

            //        if (!reader.Read())
            //        {
            //            throw new JsonException();
            //        }
            //    }

            //    if (Profile.Count == 0)
            //    {
            //        Profile = null;
            //    }

            //    break;

            //case "_profile":
            //    if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
            //    {
            //        throw new JsonException();
            //    }

            //    _Profile = new List<Element>();

            //    while (reader.TokenType != JsonTokenType.EndArray)
            //    {
            //        fhirCsR4B.Models.Element obj_Profile = new fhirCsR4B.Models.Element();
            //        obj_Profile.DeserializeJson(ref reader, options);
            //        _Profile.Add(obj_Profile);

            //        if (!reader.Read())
            //        {
            //            throw new JsonException();
            //        }
            //    }

            //    if (_Profile.Count == 0)
            //    {
            //        _Profile = null;
            //    }

            //    break;

            //case "security":
            //    if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
            //    {
            //        throw new JsonException();
            //    }

            //    Security = new List<Coding>();

            //    while (reader.TokenType != JsonTokenType.EndArray)
            //    {
            //        fhirCsR4B.Models.Coding objSecurity = new fhirCsR4B.Models.Coding();
            //        objSecurity.DeserializeJson(ref reader, options);
            //        Security.Add(objSecurity);

            //        if (!reader.Read())
            //        {
            //            throw new JsonException();
            //        }
            //    }

            //    if (Security.Count == 0)
            //    {
            //        Security = null;
            //    }

            //    break;

            //case "source":
            //    Source = reader.GetString();
            //    break;

            //case "_source":
            //    _Source = new fhirCsR4B.Models.Element();
            //    _Source.DeserializeJson(ref reader, options);
            //    break;

            //case "tag":
            //    if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
            //    {
            //        throw new JsonException();
            //    }

            //    Tag = new List<Coding>();

            //    while (reader.TokenType != JsonTokenType.EndArray)
            //    {
            //        fhirCsR4B.Models.Coding objTag = new fhirCsR4B.Models.Coding();
            //        objTag.DeserializeJson(ref reader, options);
            //        Tag.Add(objTag);

            //        if (!reader.Read())
            //        {
            //            throw new JsonException();
            //        }
            //    }

            //    if (Tag.Count == 0)
            //    {
            //        Tag = null;
            //    }

            //    break;

            case "versionId":
                o.VersionId = reader.GetString();
                break;

            case "_versionId":
                o.VersionIdElement = (Id)ElementReader.Instance.ParseExtended(ref reader, options, o.VersionIdElement);
                break;

            default:
                ElementReader.Instance.ReadProperty(ref reader, options, o, propertyName);
                break;
        }
    }

}
