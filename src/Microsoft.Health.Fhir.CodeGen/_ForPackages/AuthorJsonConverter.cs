using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Microsoft.Health.Fhir.CodeGen._ForPackages
{
    /// <summary>
    /// Only does something custom for the author element, otherwise just do the regular serialization.
    /// </summary>
    internal class AuthorJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(AuthorInfo) || objectType == typeof(string);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {

            if (reader.TokenType == JsonToken.StartObject)
            {

                if (objectType == typeof(AuthorInfo))
                {
                    // Use DummyDictionary to fool JsonSerializer into not using this converter recursively
                    var author = serializer.Deserialize<DummyDictionary>(reader);
                    return author;
                }
            }
            else if (reader.TokenType == JsonToken.String && reader.Path == "author")
            {
                if (reader.Value?.ToString() is { } value)
                {
                    var author = AuthorSerializer.Deserialize(value);
                    return author;
                }
            }
            return serializer.Deserialize(reader);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is AuthorInfo author)
            {
                serializer.Serialize(writer, author.ParsedFromString ? AuthorSerializer.Serialize(author) : value);
            }
            else
            {
                serializer.Serialize(writer, value);
            }
        }

        /// <summary>
        /// Dummy to fool JsonSerializer into not using this converter recursively
        /// </summary>
        private class DummyDictionary : AuthorInfo { }
    }
}
