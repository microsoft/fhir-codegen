// <copyright file="BaseReader.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Formats.Asn1;
using System.Text.Json;

namespace Microsoft.Health.Fhir.CodeGen.CrossVersion.R4B;

public class BaseReader<TBase> where TBase : Hl7.Fhir.Model.Base
{
    /// <summary>Reads a property.</summary>
    /// <param name="reader">      [in,out] The reader.</param>
    /// <param name="options">     Options for controlling the operation.</param>
    /// <param name="o">           A TBase to process.</param>
    /// <param name="propertyName">Name of the property.</param>
    public virtual void ReadProperty(ref Utf8JsonReader reader, JsonSerializerOptions options, TBase o, string propertyName) { }
}

public class BaseParser<TBase> : BaseReader<TBase> where TBase : Hl7.Fhir.Model.Base, new()
{
    /// <summary>Parses.</summary>
    /// <exception cref="JsonException">Thrown when a JSON error condition occurs.</exception>
    /// <param name="reader"> [in,out] The reader.</param>
    /// <param name="options">Options for controlling the operation.</param>
    /// <returns>A TBase.</returns>
    public virtual TBase Parse(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        TBase o = new();

        string propertyName;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return o;
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                propertyName = reader.GetString() ?? string.Empty;
                if (string.IsNullOrEmpty(propertyName))
                {
                    continue;
                }

                reader.Read();
                ReadProperty(ref reader, options, o, propertyName);
            }
        }

        throw new JsonException();
    }

}
