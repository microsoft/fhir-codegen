using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;

namespace Microsoft.Health.Fhir.CodeGen._ForPackages
{
    internal class ManifestDateJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(DateTimeOffset)) ||
                   (objectType == typeof(string));
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            string? dateString = reader.Value?.ToString();

            if (string.IsNullOrEmpty(dateString))
            {
                return null;
            }

            if (DateTimeOffset.TryParseExact(
                    dateString,
                    "yyyyMMddHHmmss",
                    CultureInfo.InvariantCulture.DateTimeFormat,
                    DateTimeStyles.None, out DateTimeOffset dto))
            {
                return dto;
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is DateTimeOffset dto)
            {
                serializer.Serialize(writer, dto.ToUniversalTime().ToString("yyyyMMddHHmmss"));
            }
            else if (value != null)
            {
                serializer.Serialize(writer, value);
            }
        }
    }
}
