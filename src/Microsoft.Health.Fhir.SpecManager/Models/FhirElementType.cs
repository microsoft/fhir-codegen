// <copyright file="FhirElementType.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    /// <summary>A fhir element type.</summary>
    public class FhirElementType
    {
        private readonly Dictionary<string, FhirElementProfile> _profiles;
        private readonly Uri _baseElementTypeUri = new Uri("http://hl7.org/fhir/StructureDefinition");

        /// <summary>Initializes a new instance of the <see cref="FhirElementType"/> class.</summary>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <param name="code">The code.</param>
        public FhirElementType(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            if (IsXmlType(code, out string xmlFhirType))
            {
                code = xmlFhirType;
            }

            if (IsFhirPathType(code, out string fhirType))
            {
                code = fhirType;
            }

            // check for no slashes - implied relative StructureDefinition url
            int lastSlash = code.LastIndexOf('/');
            if (lastSlash == -1)
            {
                Name = code;
                URL = new Uri(_baseElementTypeUri, code);
            }
            else
            {
                Name = code.Substring(lastSlash + 1);
                URL = new Uri(code);
            }

            Type = Name;

            _profiles = new Dictionary<string, FhirElementProfile>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FhirElementType"/> class.
        /// </summary>
        /// <param name="code">    The code.</param>
        /// <param name="profiles">The profiles.</param>
        public FhirElementType(
            string code,
            IEnumerable<string> profiles)
        {
            // TODO: chain initializers properly
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            if (IsXmlType(code, out string xmlFhirType))
            {
                code = xmlFhirType;
            }

            if (IsFhirPathType(code, out string fhirType))
            {
                code = fhirType;
            }

            // check for no slashes - implied relative StructureDefinition url
            int lastSlash = code.LastIndexOf('/');
            if (lastSlash == -1)
            {
                Name = code;
                URL = new Uri(_baseElementTypeUri, code);
            }
            else
            {
                Name = code.Substring(lastSlash + 1);
                URL = new Uri(code);
            }

            Type = Name;
            _profiles = new Dictionary<string, FhirElementProfile>();

            if (profiles != null)
            {
                foreach (string profileUrl in profiles)
                {
                    if (string.IsNullOrEmpty(profileUrl))
                    {
                        continue;
                    }

                    FhirElementProfile profile = new FhirElementProfile(new Uri(profileUrl));

                    if (_profiles.ContainsKey(profile.Name))
                    {
                        continue;
                    }

                    _profiles.Add(profile.Name, profile);
                }
            }
        }

        /// <summary>Initializes a new instance of the <see cref="FhirElementType"/> class.</summary>
        /// <param name="name">    The code.</param>
        /// <param name="url">     The URL.</param>
        /// <param name="profiles">The profiles.</param>
        private FhirElementType(string name, string type, Uri url, Dictionary<string, FhirElementProfile> profiles)
        {
            Name = name;
            Type = type;
            URL = url;
            _profiles = profiles;
        }

        /// <summary>Gets the name.</summary>
        /// <value>The name.</value>
        public string Name { get; }

        /// <summary>Gets the type.</summary>
        /// <value>The type.</value>
        public string Type { get; }

        /// <summary>Gets URL of the document.</summary>
        /// <value>The URL.</value>
        public Uri URL { get; }

        /// <summary>Gets the profiles.</summary>
        /// <value>The profiles.</value>
        public Dictionary<string, FhirElementProfile> Profiles => _profiles;

        /// <summary>Adds a profile.</summary>
        /// <param name="profileUrl">The profile url.</param>
        internal void AddProfile(string profileUrl)
        {
            FhirElementProfile profile = new FhirElementProfile(new Uri(profileUrl));

            if (_profiles.ContainsKey(profile.Name))
            {
                return;
            }

            _profiles.Add(profile.Name, profile);
        }

        /// <summary>Deep copy.</summary>
        /// <param name="primitiveTypeMap">The primitive type map.</param>
        /// <returns>A FhirElementType.</returns>
        public FhirElementType DeepCopy(
            Dictionary<string, string> primitiveTypeMap)
        {
            Dictionary<string, FhirElementProfile> profiles = new Dictionary<string, FhirElementProfile>();

            foreach (KeyValuePair<string, FhirElementProfile> kvp in _profiles)
            {
                profiles.Add(kvp.Key, kvp.Value.DeepCopy());
            }

            string type = Type ?? Name;

            if ((primitiveTypeMap != null) && primitiveTypeMap.ContainsKey(type))
            {
                type = primitiveTypeMap[type];
            }

            return new FhirElementType(Name, type, URL, profiles);
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
}
