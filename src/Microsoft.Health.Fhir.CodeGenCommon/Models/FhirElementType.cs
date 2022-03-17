// <copyright file="FhirElementType.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>A fhir element type.</summary>
public class FhirElementType
{
    private readonly Dictionary<string, FhirElementProfile> _targetProfiles;
    private readonly Dictionary<string, FhirElementProfile> _typeProfiles;
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

        _targetProfiles = new Dictionary<string, FhirElementProfile>();
        _typeProfiles = new Dictionary<string, FhirElementProfile>();
    }

    /// <summary>Initializes a new instance of the <see cref="FhirElementType"/> class.</summary>
    /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
    /// <param name="code">          The code.</param>
    /// <param name="targetProfiles">The target profiles.</param>
    /// <param name="typeProfiles">  The type profiles.</param>
    public FhirElementType(
        string code,
        IEnumerable<string> targetProfiles,
        IEnumerable<string> typeProfiles)
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

        _targetProfiles = new Dictionary<string, FhirElementProfile>();
        if (targetProfiles != null)
        {
            foreach (string profileUrl in targetProfiles)
            {
                if (string.IsNullOrEmpty(profileUrl))
                {
                    continue;
                }

                FhirElementProfile profile = new FhirElementProfile(new Uri(profileUrl));

                if (_targetProfiles.ContainsKey(profile.Name))
                {
                    continue;
                }

                _targetProfiles.Add(profile.Name, profile);
            }
        }

        _typeProfiles = new Dictionary<string, FhirElementProfile>();
        if (typeProfiles != null)
        {
            foreach (string profileUrl in typeProfiles)
            {
                if (string.IsNullOrEmpty(profileUrl))
                {
                    continue;
                }

                FhirElementProfile profile = new FhirElementProfile(new Uri(profileUrl));

                if (_typeProfiles.ContainsKey(profile.Name))
                {
                    continue;
                }

                _typeProfiles.Add(profile.Name, profile);
            }
        }
    }

    /// <summary>Initializes a new instance of the <see cref="FhirElementType"/> class.</summary>
    /// <param name="name">          The code.</param>
    /// <param name="type">          The type.</param>
    /// <param name="url">           The URL.</param>
    /// <param name="profiles">      The target profiles.</param>
    /// <param name="typeProfiles">  The type profiles.</param>
    [System.Text.Json.Serialization.JsonConstructor]
    public FhirElementType(
        string name,
        string type,
        Uri url,
        Dictionary<string, FhirElementProfile> profiles,
        Dictionary<string, FhirElementProfile> typeProfiles)
    {
        Name = name;
        Type = type;
        URL = url;
        _targetProfiles = profiles;
        _typeProfiles = typeProfiles;
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
    public Dictionary<string, FhirElementProfile> Profiles => _targetProfiles;

    /// <summary>Gets the type profiles.</summary>
    public Dictionary<string, FhirElementProfile> TypeProfiles => _typeProfiles;

    /// <summary>Adds a TARGET profile.</summary>
    /// <param name="profileUrl">The profile url.</param>
    public void AddProfile(string profileUrl)
    {
        FhirElementProfile profile = new FhirElementProfile(new Uri(profileUrl));

        if (_targetProfiles.ContainsKey(profile.Name))
        {
            return;
        }

        _targetProfiles.Add(profile.Name, profile);
    }

    /// <summary>Adds a type profile.</summary>
    /// <param name="profileUrl">The profile url.</param>
    public void AddTypeProfile(string profileUrl)
    {
        FhirElementProfile profile = new FhirElementProfile(new Uri(profileUrl));

        if (_typeProfiles.ContainsKey(profile.Name))
        {
            return;
        }

        _typeProfiles.Add(profile.Name, profile);
    }

    /// <summary>Deep copy.</summary>
    /// <param name="primitiveTypeMap">The primitive type map.</param>
    /// <returns>A FhirElementType.</returns>
    public FhirElementType DeepCopy(
        Dictionary<string, string> primitiveTypeMap)
    {
        Dictionary<string, FhirElementProfile> targetProfiles = _targetProfiles.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.DeepCopy());

        Dictionary<string, FhirElementProfile> typeProfiles = _typeProfiles.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.DeepCopy());

        string type = Type ?? Name;

        if ((primitiveTypeMap != null) && primitiveTypeMap.ContainsKey(type))
        {
            type = primitiveTypeMap[type];
        }

        return new FhirElementType(Name, type, URL, targetProfiles, typeProfiles);
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
