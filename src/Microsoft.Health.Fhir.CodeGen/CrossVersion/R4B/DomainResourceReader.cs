// <copyright file="DomainResourceConverter.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using System.Text.Json;
using System.Text.Json.Serialization;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGen.CrossVersion.Common;
using static System.Net.Mime.MediaTypeNames;

namespace Microsoft.Health.Fhir.CodeGen.CrossVersion.R4B;

public class DomainResourceReader : BaseReader<DomainResource>
{
    private static DomainResourceReader _instance = new();
    public static DomainResourceReader Instance => _instance;

    override public void ReadProperty(ref Utf8JsonReader reader, JsonSerializerOptions options, DomainResource o, string propertyName)
    {
        switch (propertyName)
        {
            //case "contained":
            //    if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
            //    {
            //        throw new JsonException();
            //    }

            //    o.Contained = new List<Resource>();

            //    while (reader.TokenType != JsonTokenType.EndArray)
            //    {
            //        fhirCsR4B.Models.Resource resource = JsonSerializer.Deserialize<fhirCsR4B.Models.Resource>(ref reader, options);
            //        Contained.Add(resource);

            //        if (!reader.Read())
            //        {
            //            throw new JsonException();
            //        }
            //    }

            //    if (Contained.Count == 0)
            //    {
            //        Contained = null;
            //    }

            //    break;

            //case "extension":
            //    if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
            //    {
            //        throw new JsonException();
            //    }

            //    Extension = new List<Extension>();

            //    while (reader.TokenType != JsonTokenType.EndArray)
            //    {
            //        fhirCsR4B.Models.Extension objExtension = new fhirCsR4B.Models.Extension();
            //        objExtension.DeserializeJson(ref reader, options);
            //        Extension.Add(objExtension);

            //        if (!reader.Read())
            //        {
            //            throw new JsonException();
            //        }
            //    }

            //    if (Extension.Count == 0)
            //    {
            //        Extension = null;
            //    }

            //    break;

            //case "modifierExtension":
            //    if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
            //    {
            //        throw new JsonException();
            //    }

            //    ModifierExtension = new List<Extension>();

            //    while (reader.TokenType != JsonTokenType.EndArray)
            //    {
            //        fhirCsR4B.Models.Extension objModifierExtension = new fhirCsR4B.Models.Extension();
            //        objModifierExtension.DeserializeJson(ref reader, options);
            //        ModifierExtension.Add(objModifierExtension);

            //        if (!reader.Read())
            //        {
            //            throw new JsonException();
            //        }
            //    }

            //    if (ModifierExtension.Count == 0)
            //    {
            //        ModifierExtension = null;
            //    }

            //    break;

            //case "text":
            //    Text = new fhirCsR4B.Models.Narrative();
            //    Text.DeserializeJson(ref reader, options);
            //    break;

            default:
                ResourceReader.Instance.ReadProperty(ref reader, options, o, propertyName);
                break;
        }
    }

}
