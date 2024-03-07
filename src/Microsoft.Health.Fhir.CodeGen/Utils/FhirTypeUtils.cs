// <copyright file="FhirTypeUtils.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.CodeGen.Utils;

/// <summary>A FHIR type utilities.</summary>
public static class FhirTypeUtils
{
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
