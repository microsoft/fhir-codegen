// <copyright file="FhirTypeUtils.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics.CodeAnalysis;


#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#elif NETSTANDARD2_0
using Microsoft.Health.Fhir.CodeGenCommon.Polyfill;
#endif

namespace Microsoft.Health.Fhir.CodeGenCommon.Utils;

public static class FhirTypeUtils
{
    public readonly record struct FhirPrimitiveInfoRec(
        string FhirType,
        string FhirPathType,
        string JsonType,
        string XmlType,
        string? Regex);

    /// <summary>(Immutable) The FHIR sequence map.</summary>
    private static readonly FrozenDictionary<string, FhirPrimitiveInfoRec> _fhirPrimitiveInfo = new Dictionary<string, FhirPrimitiveInfoRec>()
    {
        { "base64Binary", new("base64Binary", "http://hl7.org/fhirpath/System.String", "string", "xs:base64Binary", "(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=)?") },
        { "boolean", new("boolean", "http://hl7.org/fhirpath/System.Boolean", "boolean", "xs:boolean", "true|false") },
        { "canonical", new("canonical", "http://hl7.org/fhirpath/System.String", "string", "xs:anyURI", "\\S*") },
        { "code", new("code", "http://hl7.org/fhirpath/System.String", "string", "xs:token", "[^\\s]+( [^\\s]+)*") },
        { "date", new("date", "http://hl7.org/fhirpath/System.Date", "string", "xs:date, xs:gYearMonth, xs:gYear", "([0-9]([0-9]([0-9][1-9]|[1-9]0)|[1-9]00)|[1-9]000)(-(0[1-9]|1[0-2])(-(0[1-9]|[1-2][0-9]|3[0-1]))?)?") },
        { "dateTime", new("dateTime", "http://hl7.org/fhirpath/System.DateTime", "string", "xs:dateTime, xs:date, xs:gYearMonth, xs:gYear", "([0-9]([0-9]([0-9][1-9]|[1-9]0)|[1-9]00)|[1-9]000)(-(0[1-9]|1[0-2])(-(0[1-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:([0-5][0-9]|60)(\\.[0-9]{1,9})?)?)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)?)?)?") },
        { "decimal", new("decimal", "http://hl7.org/fhirpath/System.Decimal", "number", "xs:decimal, xs:double", "-?(0|[1-9][0-9]{0,17})(\\.[0-9]{1,17})?([eE][+-]?[0-9]{1,9}})?") },
        { "id", new("id", "http://hl7.org/fhirpath/System.String", "string", "xs:string", "[A-Za-z0-9\\-\\.]{1,64}") },
        { "instant", new("instant", "http://hl7.org/fhirpath/System.DateTime", "string", "xs:dateTime", "([0-9]([0-9]([0-9][1-9]|[1-9]0)|[1-9]00)|[1-9]000)-(0[1-9]|1[0-2])-(0[1-9]|[1-2][0-9]|3[0-1])T([01][0-9]|2[0-3]):[0-5][0-9]:([0-5][0-9]|60)(\\.[0-9]{1,9})?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00))") },
        { "integer", new("integer", "http://hl7.org/fhirpath/System.Integer", "number", "xs:int", "[0]|[-+]?[1-9][0-9]*") },
        { "integer64", new("integer64", "http://hl7.org/fhirpath/System.Integer", "string", "xs:long", "[0]|[-+]?[1-9][0-9]*") },
        { "markdown", new("markdown", "http://hl7.org/fhirpath/System.String", "string", "xs:string", "^[\\s\\S]+$") },
        { "oid", new("oid", "http://hl7.org/fhirpath/System.String", "string", "xs:anyURI", "urn:oid:[0-2](\\.(0|[1-9][0-9]*))+") },
        { "positiveInt", new("positiveInt", "http://hl7.org/fhirpath/System.Integer", "number", "xs:positiveInteger", "[1-9][0-9]*") },
        { "string", new("string", "http://hl7.org/fhirpath/System.String", "string", "xs:string", "^[\\s\\S]+$") },
        { "time", new("time", "http://hl7.org/fhirpath/System.Time", "string", "xs:time", "([01][0-9]|2[0-3]):[0-5][0-9]:([0-5][0-9]|60)(\\.[0-9]{1,9})?") },
        { "unsignedInt", new("unsignedInt", "http://hl7.org/fhirpath/System.Integer", "number", "xs:nonNegativeInteger", "[0]|([1-9][0-9]*)") },
        { "uri", new("uri", "http://hl7.org/fhirpath/System.String", "string", "xs:anyURI", "\\S*") },
        { "url", new("url", "http://hl7.org/fhirpath/System.String", "string", "xs:anyURI", "\\S*") },
        { "uuid", new("uuid", "http://hl7.org/fhirpath/System.String", "string", "xs:anyURI", "urn:uuid:[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}") },
        { "xhtml", new("xhtml", "http://hl7.org/fhirpath/System.String", "string", "xs:string, xhtml:div", null) },
    }.ToFrozenDictionary();

    /// <summary>
    /// Attempts to get primitive information a FhirPrimitiveInfoRec from the given string.
    /// </summary>
    /// <param name="fhirType">Type in FHIR.</param>
    /// <param name="rec">     [out] The record.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public static bool TryGetPrimitiveInfo(string fhirType, [NotNullWhen(true)] out FhirPrimitiveInfoRec rec) => _fhirPrimitiveInfo.TryGetValue(fhirType, out rec);

    /// <summary>Type from XML type.</summary>
    /// <param name="xmlType"> Type of the XML.</param>
    /// <param name="fhirType">[out] Type in FHIR.</param>
    /// <returns>A string.</returns>
    public static bool IsXmlType(string xmlType, out string fhirType)
    {
        fhirType = string.Empty;

        switch (xmlType)
        {
            case "xsd:token":
            case "xs:token":
                fhirType = "code";
                break;

            case "xsd:base64Binary":
            case "base64Binary":
                fhirType = "base64Binary";
                break;

            case "xsd:string":
            case "xs:string":
            case "xs:string+":
            case "xhtml:div":
                fhirType = "string";
                break;

            case "xsd:int":
                fhirType = "int";
                break;

            case "xsd:positiveInteger":
            case "xs:positiveInteger":
                fhirType = "positiveInt";
                break;

            case "xsd:nonNegativeInteger":
            case "xs:nonNegativeInteger":
                fhirType = "unsignedInt";
                break;

            case "xs:anyURI+":
            case "xsd:anyURI":
            case "xs:anyURI":
            case "anyURI":
                fhirType = "uri";
                break;

            case "xsd:gYear OR xsd:gYearMonth OR xsd:date":
            case "xs:gYear, xs:gYearMonth, xs:date":
            case "xsd:date":
                fhirType = "date";
                break;

            case "xsd:gYear OR xsd:gYearMonth OR xsd:date OR xsd:dateTime":
            case "xs:gYear, xs:gYearMonth, xs:date, xs:dateTime":
            case "xsd:dateTime":
                fhirType = "dateTime";
                break;

            case "xsd:time":
            case "time":
                fhirType = "time";
                break;

            case "xsd:boolean":
                fhirType = "boolean";
                break;

            case "xsd:decimal":
                fhirType = "decimal";
                break;

            default:
                break;
        }

        return !string.IsNullOrEmpty(fhirType);
    }

    /// <summary>Query if 'xmlType' is XML base type.</summary>
    /// <param name="xmlType"> Type of the XML.</param>
    /// <param name="fhirType">[out] Type in FHIR.</param>
    /// <returns>True if XML base type, false if not.</returns>
    public static bool IsXmlBaseType(string xmlType, out string fhirType)
    {
        fhirType = string.Empty;

        switch (xmlType)
        {
            case "xsd:token":
            case "xs:token":
                fhirType = "code";
                break;

            case "xsd:base64Binary":
            case "base64Binary":
            case "xs:anyURI+":
            case "xsd:anyURI":
            case "xs:anyURI":
            case "anyURI":
            case "xsd:string":
            case "xs:string":
            case "xs:string+":
            case "xhtml:div":
                fhirType = "string";
                break;

            case "xsd:int":
            case "xsd:positiveInteger":
            case "xs:positiveInteger":
            case "xsd:nonNegativeInteger":
            case "xs:nonNegativeInteger":
                fhirType = "int";
                break;

            case "xsd:gYear OR xsd:gYearMonth OR xsd:date":
            case "xs:gYear, xs:gYearMonth, xs:date":
            case "xsd:date":
                fhirType = "date";
                break;

            case "xsd:gYear OR xsd:gYearMonth OR xsd:date OR xsd:dateTime":
            case "xs:gYear, xs:gYearMonth, xs:date, xs:dateTime":
            case "xsd:dateTime":
                fhirType = "dateTime";
                break;

            case "xsd:time":
            case "time":
                fhirType = "time";
                break;

            case "xsd:boolean":
                fhirType = "boolean";
                break;

            case "xsd:decimal":
                fhirType = "decimal";
                break;

            default:
                break;
        }

        return !string.IsNullOrEmpty(fhirType);
    }

    /// <summary>FHIR type.</summary>
    /// <param name="literal">The literal.</param>
    /// <returns>A string.</returns>
    public static string SystemToFhirType(string literal) => literal switch
    {
        "http://hl7.org/fhirpath/System.String" => "string",
        "http://hl7.org/fhirpath/String" => "string",
        "http://hl7.org/fhirpath/System.Boolean" => "boolean",
        "http://hl7.org/fhirpath/bool" => "boolean",
        "http://hl7.org/fhirpath/System.Date" => "date",
        "http://hl7.org/fhirpath/System.DateTime" => "dateTime",
        "http://hl7.org/fhirpath/DateTime" => "dateTime",
        "http://hl7.org/fhirpath/System.Decimal" => "decimal",
        "http://hl7.org/fhirpath/Decimal" => "decimal",
        "http://hl7.org/fhirpath/System.Integer" => "int",
        "http://hl7.org/fhirpath/Integer" => "int",
        "http://hl7.org/fhirpath/System.Time" => "time",
        "http://hl7.org/fhirpath/Time" => "time",
        "http://hl7.org/fhirpath/Quantity" => "Quantity",
        "http://hl7.org/fhirpath/SimpleTypeInfo" => "SimpleType",
        "http://hl7.org/fhirpath/ClassInfo" => "Class",
        _ => string.Empty
    };

    /// <summary>Check if a type is listed in FHIRPath notation, and return the FHIR type if it is.</summary>
    /// <param name="fhirPathType">Type in FHIRPath.</param>
    /// <param name="fhirType">    [out] Type in FHIR.</param>
    /// <returns>A string.</returns>
    public static bool IsFhirPathType(string fhirPathType, out string fhirType)
    {
        fhirType = string.Empty;

        switch (fhirPathType)
        {
            case "http://hl7.org/fhirpath/System.String":
            case "http://hl7.org/fhirpath/String":
                fhirType = "string";
                break;

            case "http://hl7.org/fhirpath/System.Boolean":
            case "http://hl7.org/fhirpath/bool":
                fhirType = "boolean";
                break;

            case "http://hl7.org/fhirpath/System.Date":
                fhirType = "date";
                break;

            case "http://hl7.org/fhirpath/System.DateTime":
            case "http://hl7.org/fhirpath/DateTime":
                fhirType = "dateTime";
                break;

            case "http://hl7.org/fhirpath/System.Decimal":
            case "http://hl7.org/fhirpath/Decimal":
                fhirType = "decimal";
                break;

            case "http://hl7.org/fhirpath/System.Integer":
            case "http://hl7.org/fhirpath/Integer":
                fhirType = "int";
                break;

            case "http://hl7.org/fhirpath/System.Time":
            case "http://hl7.org/fhirpath/Time":
                fhirType = "time";
                break;

            case "http://hl7.org/fhirpath/Quantity":
                fhirType = "Quantity";
                break;

            case "http://hl7.org/fhirpath/SimpleTypeInfo":
                fhirType = "SimpleType";
                break;

            case "http://hl7.org/fhirpath/ClassInfo":
                fhirType = "Class";
                break;

            default:
                break;
        }

        return !string.IsNullOrEmpty(fhirType);
    }

}
