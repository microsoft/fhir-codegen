using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Fhir.CodeGen.Packages.Models;

namespace Fhir.CodeGen.Packages.Converters;


/// <summary>
/// Need a custom converter to handle the different property name casing and variants.
/// </summary>
internal sealed class RegistryCatalogJsonConverter : JsonConverter<RegistryCatalogRecord>
{
    public override RegistryCatalogRecord? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        bool upperCaseNames = false;

        string? name = null;
        string? description = null;
        string? fhirVersion = null;
        DateTime? publicationDate = null;
        string? version = null;
        int? resourceCount = null;
        string? canonical = null;
        string? kind = null;
        string? url = null;
        string? scope = null;
        List<string>? keywords = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();

            string propName = reader.GetString()!;
            reader.Read(); // move to value

            if (propName.Length == 0)
            {
                continue;
            }

            if (char.IsUpper(propName[0]))
            {
                upperCaseNames = true;
            }

            // Normalize by simple case-insensitive comparison or explicit switch on known variants
            switch (propName)
            {
                case "Name":
                case "name":
                    name = reader.TokenType == JsonTokenType.Null ? null : reader.GetString();
                    break;

                case "Description":
                case "description":
                    description = reader.TokenType == JsonTokenType.Null ? null : reader.GetString();
                    break;

                case "FhirVersion":
                case "fhirVersion":
                case "FHIRVersion":
                case "fhirversion":
                    fhirVersion = reader.TokenType == JsonTokenType.Null ? null : reader.GetString();
                    break;

                case "Date":
                case "date":
                case "PublicationDate":
                case "publicationDate":
                    publicationDate = reader.TokenType == JsonTokenType.Null ? null : reader.GetDateTime();
                    break;

                case "Version":
                case "version":
                    version = reader.TokenType == JsonTokenType.Null ? null : reader.GetString();
                    break;

                case "Count":
                case "count":
                    if (reader.TokenType == JsonTokenType.String)
                    {
                        string? countStr = reader.GetString();
                        if (int.TryParse(countStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out int count))
                        {
                            resourceCount = count;
                        }
                    }
                    else if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out int count))
                    {
                        resourceCount = count;
                    }
                    else
                    {
                        resourceCount = null;
                    }
                    break;

                case "Canonical":
                case "canonical":
                    canonical = reader.TokenType == JsonTokenType.Null ? null : reader.GetString();
                    break;

                case "Kind":
                case "kind":
                    kind = reader.TokenType == JsonTokenType.Null ? null : reader.GetString();
                    break;

                case "Url":
                case "url":
                    url = reader.TokenType == JsonTokenType.Null ? null : reader.GetString();
                    break;

                case "Scope":
                case "scope":
                    scope = reader.TokenType == JsonTokenType.Null ? null : reader.GetString();
                    break;

                case "Keywords":
                case "keywords":
                    if (reader.TokenType == JsonTokenType.StartArray)
                    {
                        keywords = new List<string>();
                        while (reader.Read())
                        {
                            if (reader.TokenType == JsonTokenType.EndArray)
                                break;

                            if (reader.TokenType == JsonTokenType.String)
                            {
                                keywords.Add(reader.GetString()!);
                            }
                        }
                    }
                    else if (reader.TokenType == JsonTokenType.String)
                    {
                        keywords = [reader.GetString() ?? string.Empty];
                    }

                    break;

                default:
                    // Skip unknown property
                    if (reader.TokenType is JsonTokenType.StartObject or JsonTokenType.StartArray)
                    {
                        using var _ = JsonDocument.ParseValue(ref reader); // consume complex value
                    }
                    // primitive already consumed
                    break;
            }
        }

        return new RegistryCatalogRecord
        {
            Name = name,
            Description = description,
            FhirVersion = fhirVersion,
            PublicationDate = publicationDate,
            Version = version,
            ResourceCount = resourceCount,
            Canonical = canonical,
            Kind = kind,
            Url = url,
            UpperCaseNames = upperCaseNames,
        };
    }

    public override void Write(Utf8JsonWriter writer, RegistryCatalogRecord value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        if (value.UpperCaseNames == true)
        {
            if (value.Name is not null) writer.WriteString("Name", value.Name);
            if (value.Description is not null) writer.WriteString("Description", value.Description);
            if (value.FhirVersion is not null) writer.WriteString("FhirVersion", value.FhirVersion);
            if (value.PublicationDate is not null) writer.WriteString("PublicationDate", value.PublicationDate?.ToString("s", CultureInfo.InvariantCulture));
            if (value.Version is not null) writer.WriteString("Version", value.Version);
            if (value.ResourceCount is not null) writer.WriteNumber("Count", value.ResourceCount.Value);
            if (value.Canonical is not null) writer.WriteString("Canonical", value.Canonical);
            if (value.Kind is not null) writer.WriteString("Kind", value.Kind);
            if (value.Url is not null) writer.WriteString("Url", value.Url);
            writer.WriteEndObject();

            return;
        }

        if (value.Name is not null) writer.WriteString("name", value.Name);
        if (value.Description is not null) writer.WriteString("description", value.Description);
        if (value.FhirVersion is not null) writer.WriteString("fhirVersion", value.FhirVersion);
        if (value.PublicationDate is not null) writer.WriteString("publicationDate", value.PublicationDate?.ToString("s", CultureInfo.InvariantCulture));
        if (value.Version is not null) writer.WriteString("version", value.Version);
        if (value.ResourceCount is not null) writer.WriteNumber("count", value.ResourceCount.Value);
        if (value.Canonical is not null) writer.WriteString("canonical", value.Canonical);
        if (value.Kind is not null) writer.WriteString("kind", value.Kind);
        if (value.Url is not null) writer.WriteString("url", value.Url);
        writer.WriteEndObject();
    }
}
