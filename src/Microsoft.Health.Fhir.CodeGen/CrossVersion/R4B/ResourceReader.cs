// <copyright file="ResourceReader.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGen.CrossVersion.Common;
using Microsoft.Health.Fhir.CodeGen.Lanugage;
using System.Security.Cryptography;
using System.Text.Json;

namespace Microsoft.Health.Fhir.CodeGen.CrossVersion.R4B;

public class ResourceReader : BaseReader<Resource>
{
    private static ResourceReader _instance = new();
    public static ResourceReader Instance => _instance;

    override public void ReadProperty(ref Utf8JsonReader reader, JsonSerializerOptions options, Resource r, string propertyName)
    {
        switch (propertyName)
        {
            case "id":
                r.Id = reader.GetString();
                break;

            //case "_id":
            //    JsonSerializer.Deserialize<Element>(ref reader, options);
            //    _Id = new fhirCsR4B.Models.Element();
            //    _Id.DeserializeJson(ref reader, options);
            //    break;

            //case "implicitRules":
            //    r.ImplicitRules = reader.GetString();
            //    break;

            //case "_implicitRules":
            //    _ImplicitRules = new fhirCsR4B.Models.Element();
            //    _ImplicitRules.DeserializeJson(ref reader, options);
            //    break;

            //case "language":
            //    r.Language = reader.GetString();
            //    break;

            //case "_language":
            //    _Language = new fhirCsR4B.Models.Element();
            //    _Language.DeserializeJson(ref reader, options);
            //    break;

            case "meta":
                r.Meta = MetaReader.Instance.Parse(ref reader, options);
                break;

        }
    }
}
