using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Utils;
using Microsoft.Health.Fhir.Comparison.Models;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;

namespace Microsoft.Health.Fhir.Comparison.Extensions;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// </remarks>
public static class DbStringFhirConverterExtensions
{
    // public static string ToJson<T>(this List<T> sourceList, FhirJsonSerializationSettings? settings = null)
    //     where T : Hl7.Fhir.Model.Base
    // {
    //     settings ??= new() { Pretty = true };
    //     return "[" + sourceList.Select(s => s.ToJson(settings)) + "]";
    // }

    // public static string? ToDbJson<T>(this List<T> values)
    //     where T : Hl7.Fhir.Model.Base
    //     => values.Count == 0
    //     ? null
    //     : JsonSerializer.Serialize(values);

    // public static string? ToDbJson<T>(this T? value)
    //     where T : Hl7.Fhir.Model.Base
    //     => value == null
    //     ? null
    //     : JsonSerializer.Serialize(value);

    // public static List<T> FromDbJsonAsList<T>(this string? val)
    //     where T : Hl7.Fhir.Model.Base
    //     => val == null
    //     ? []
    //     : JsonSerializer.Deserialize<List<T>>(val) ?? [];

    // public static T? FromDbJson<T>(this string? val)
    //     where T : Hl7.Fhir.Model.Base
    //     => val == null
    //     ? null
    //     : JsonSerializer.Deserialize<T>(val);
}