using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Utility;

namespace Microsoft.Health.Fhir.Comparison.Extensions;

public static class TestExtensionsNotUsed
{
    //private static Hl7.Fhir.Serialization.FhirJsonSerializer _fhirSerializer = new Hl7.Fhir.Serialization.FhirJsonSerializer(new Hl7.Fhir.Serialization.SerializerSettings()
    //{
    //    Pretty = false,
    //});

    //private static Hl7.Fhir.Serialization.FhirJsonParser _fhirParser = new Hl7.Fhir.Serialization.FhirJsonParser(new Hl7.Fhir.Serialization.ParserSettings()
    //{
    //});

    private static JsonSerializerOptions _options = new JsonSerializerOptions()
        .ForFhir(ModelInfo.ModelInspector)
        .UsingMode(DeserializerModes.Ostrich)
        .Compact();

    public static bool TrySerializeForDb<T>(T instance, [NotNullWhen(true)]out string? json) where T : Hl7.Fhir.Model.Base
    {
        if (instance == null)
        {
            json = null;
            return false;
        }

        // Serialize the FHIR model instance to a JSON string
        json = JsonSerializer.Serialize(instance, _options);
        return true;
    }

    public static bool TrySerializeForDb<T>(List<T> instances, [NotNullWhen(true)] out string? json) where T : Hl7.Fhir.Model.Base
    {
        if ((instances == null) || (instances.Count == 0))
        {
            json = null;
            return false;
        }

        // Serialize the FHIR model instance to a JSON string
        json = JsonSerializer.Serialize(instances, _options);
        return true;
    }

    public static T? ParseFromDb<T>(string json) where T : Hl7.Fhir.Model.Base
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        // Parse the JSON string into a FHIR model instance of type T
        return JsonSerializer.Deserialize<T>(json, _options);
    }

    public static List<T> ParseArrayFromDb<T>(string json) where T : Hl7.Fhir.Model.Base
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<T>();
        }

        return JsonSerializer.Deserialize<List<T>>(json, _options) ?? [];
    }


    //public static string? SerializeForDb(Hl7.Fhir.Model.Base instance, bool isNullable = false)
    //{
    //    if (instance == null)
    //    {
    //        return isNullable ? null : string.Empty;
    //    }

    //    // Serialize the FHIR model instance to a JSON string
    //    return _fhirSerializer.SerializeToString(instance);
    //}

    //public static string? SerializeArrayForDb(IEnumerable<Hl7.Fhir.Model.Base> instances, bool isNullable = false)
    //{
    //    if (instances == null || !instances.Any())
    //    {
    //        return isNullable ? null : string.Empty;
    //    }

    //    // Serialize each instance individually and combine them into a JSON array
    //    IEnumerable<string> serializedInstances = instances.Select(instance => _fhirSerializer.SerializeToString(instance));
    //    return $"[{string.Join(",", serializedInstances)}]";
    //}
    //public static string? SerializeArrayForDb(IEnumerable<Hl7.Fhir.Model.Base> instances, bool isNullable = false)
    //{
    //    if (instances == null || !instances.Any())
    //    {
    //        return isNullable ? null : string.Empty;
    //    }

    //    // Serialize each instance individually and combine them into a JSON array
    //    IEnumerable<string> serializedInstances = instances.Select(instance => _fhirSerializer.SerializeToString(instance));
    //    return $"[{string.Join(",", serializedInstances)}]";
    //}

    //public static T? ParseFromDb<T>(string json) where T : Hl7.Fhir.Model.Base
    //{
    //    if (string.IsNullOrWhiteSpace(json))
    //    {
    //        return null;
    //    }

    //    // Parse the JSON string into a FHIR model instance of type T
    //    return _fhirParser.Parse<T>(json);
    //}

    //public static Hl7.Fhir.Model.Base? ParseFromDb(string json, Type dataType)
    //{
    //    if (string.IsNullOrWhiteSpace(json))
    //    {
    //        return null;
    //    }

    //    // Parse the JSON string into a FHIR model instance of type dataType
    //    return _fhirParser.Parse(json, dataType);
    //}

    //public static List<T> ParseArrayFromDb2<T>(string json) where T : Hl7.Fhir.Model.Base
    //{
    //    if (string.IsNullOrWhiteSpace(json))
    //    {
    //        return new List<T>();
    //    }

    //    // Parse the JSON array string into individual JSON objects
    //    var jsonArray = System.Text.Json.JsonDocument.Parse(json).RootElement.EnumerateArray();

    //    // Deserialize each JSON object into a FHIR model instance of type T
    //    return jsonArray.Select(element => _fhirParser.Parse<T>(element.GetRawText())).ToList();
    //}

    //public static List<Hl7.Fhir.Model.Base> ParseArrayFromDb(string json, Type dataType)
    //{
    //    if (string.IsNullOrWhiteSpace(json))
    //    {
    //        return new List<Hl7.Fhir.Model.Base>();
    //    }

    //    // Parse the JSON array string into individual JSON objects
    //    var jsonArray = System.Text.Json.JsonDocument.Parse(json).RootElement.EnumerateArray();

    //    // Deserialize each JSON object into a FHIR model instance of type T
    //    return jsonArray.Select(element => _fhirParser.Parse(element.GetRawText(), dataType)).ToList();
    //}
}
