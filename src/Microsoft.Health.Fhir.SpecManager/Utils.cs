// <copyright file="Utils.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager
{
    /// <summary>Utilities (temp home until they have better ones).</summary>
    public abstract class Utils
    {
        /// <summary>Type from fhir type.</summary>
        ///
        /// <param name="fhirType">Type of the fhir.</param>
        ///
        /// <returns>A string.</returns>
        public static string TypeFromFhirType(string fhirType)
        {
            switch (fhirType)
            {
                case "http://hl7.org/fhirpath/System.String":
                    return "string";

                case "http://hl7.org/fhirpath/System.Boolean":
                    return "boolean";

                case "http://hl7.org/fhirpath/System.Date":
                    return "date";

                case "http://hl7.org/fhirpath/System.DateTime":
                    return "dateTime";

                case "http://hl7.org/fhirpath/System.Decimal":
                    return "decimal";

                case "http://hl7.org/fhirpath/System.Integer":
                    return "int";

                case "http://hl7.org/fhirpath/System.Time":
                    return "time";

                default:
                    return fhirType;
            }
        }

        /// <summary>Type from XML type.</summary>
        /// <param name="xmlType">Type of the XML.</param>
        /// <returns>A string.</returns>
        public static string TypeFromXmlType(string xmlType)
        {
            switch (xmlType)
            {
                case "xsd:token":
                case "xs:token":
                    return "code";

                case "xsd:base64Binary":
                case "base64Binary":
                    return "base64Binary";

                case "xsd:string":
                case "xs:string":
                case "xs:string+":
                case "xhtml:div":
                    return "string";

                case "xsd:int":
                    return "int";

                case "xsd:positiveInteger":
                case "xs:positiveInteger":
                    return "positiveInt";

                case "xsd:nonNegativeInteger":
                case "xs:nonNegativeInteger":
                    return "unsignedInt";

                case "xs:anyURI+":
                case "xsd:anyURI":
                case "xs:anyURI":
                case "anyURI":
                    return "uri";

                case "xsd:gYear OR xsd:gYearMonth OR xsd:date":
                case "xs:gYear, xs:gYearMonth, xs:date":
                case "xsd:date":
                    return "date";

                case "xsd:gYear OR xsd:gYearMonth OR xsd:date OR xsd:dateTime":
                case "xs:gYear, xs:gYearMonth, xs:date, xs:dateTime":
                case "xsd:dateTime":
                    return "dateTime";

                case "xsd:time":
                case "time":
                    return "time";

                case "xsd:boolean":
                    return "boolean";

                case "xsd:decimal":
                    return "decimal";

                default:
                    return xmlType;
            }
        }
    }
}
