// <copyright file="FhirTypeUtils.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Xml.Linq;
using System;

namespace Microsoft.Health.Fhir.CodeGenCommon.Refactor;

/// <summary>FHIR type-related utilities.</summary>
public static class FhirTypeUtils
{
    /// <summary>Values that represent search magic parameters.</summary>
    public enum SearchParameterGrouping
    {
        /// <summary>An enum constant representing all resource option.</summary>
        Global,

        /// <summary>An enum constant representing the search result option.</summary>
        Result,

        /// <summary>An enum constant representing all interaction option.</summary>
        Interaction,
    }


    /// <summary>Attempts to get name and URL.</summary>
    /// <param name="value">The value.</param>
    /// <param name="name"> [out] The name.</param>
    /// <param name="url">  [out] URL of the resource.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public static bool TryGetNameAndUrl(string value, out string name, out string url)
    {
        if (string.IsNullOrEmpty(value))
        {
            name = string.Empty;
            url = string.Empty;
            return false;
        }

        int lastSlash = value.LastIndexOf('/');
        if (lastSlash == -1)
        {
            name = value;
            url = "http://hl7.org/fhir/StructureDefinition/" + value;
        }
        else
        {
            name = value.Substring(lastSlash + 1);
            url = value;
        }

        return true;
    }

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
                fhirType = "string";
                break;

            case "http://hl7.org/fhirpath/System.Boolean":
                fhirType = "boolean";
                break;

            case "http://hl7.org/fhirpath/System.Date":
                fhirType = "date";
                break;

            case "http://hl7.org/fhirpath/System.DateTime":
                fhirType = "dateTime";
                break;

            case "http://hl7.org/fhirpath/System.Decimal":
                fhirType = "decimal";
                break;

            case "http://hl7.org/fhirpath/System.Integer":
                fhirType = "int";
                break;

            case "http://hl7.org/fhirpath/System.Time":
                fhirType = "time";
                break;

            default:
                break;
        }

        return !string.IsNullOrEmpty(fhirType);
    }

}
