// <copyright file="ElementReader.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Security.Cryptography;
using System.Text.Json;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGen.CrossVersion.Common;

namespace Microsoft.Health.Fhir.CodeGen.CrossVersion.R4B;

public class ElementReader : BaseReader<Element>
{
    private static ElementReader _instance = new();
    public static ElementReader Instance => _instance;

    override public void ReadProperty(ref Utf8JsonReader reader, JsonSerializerOptions options, Element o, string propertyName)
    {
        switch (propertyName)
        {
            case "extension":
                if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
                {
                    throw new JsonException();
                }

                while (reader.TokenType != JsonTokenType.EndArray)
                {
                    Extension ext = ExtensionReader.Instance.Parse(ref reader, options);
                    o.Extension.Add(ext);

                    if (!reader.Read())
                    {
                        throw new JsonException();
                    }
                }
                break;

            case "id":
                o.ElementId = reader.GetString();
                break;
        }
    }

    public DataType ParseExtended(ref Utf8JsonReader reader, JsonSerializerOptions options, DataType dt)
    {
        string propertyName;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return dt;
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                propertyName = reader.GetString() ?? string.Empty;
                if (string.IsNullOrEmpty(propertyName))
                {
                    continue;
                }

                reader.Read();
                ReadProperty(ref reader, options, dt, propertyName);
            }
        }

        throw new JsonException();
    }
}
