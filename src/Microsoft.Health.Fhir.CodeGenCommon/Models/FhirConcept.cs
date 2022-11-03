// <copyright file="FhirConcept.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>A fhir triplet.</summary>
public class FhirConcept : ICloneable
{
    private Dictionary<string, List<object>> _properties = new();

    /// <summary>The properties and values.</summary>
    private HashSet<string> _propertyKeyValueHash = new();

    /// <summary>Initializes a new instance of the <see cref="FhirConcept"/> class.</summary>
    /// <param name="system">    The system.</param>
    /// <param name="code">      The code.</param>
    /// <param name="display">   The display.</param>
    /// <param name="version">   The version.</param>
    /// <param name="definition">The definition.</param>
    /// <param name="systemLocalName">The name of the system.</param>
    public FhirConcept(
        string system,
        string code,
        string display,
        string version,
        string definition,
        string systemLocalName)
    {
        System = system ?? string.Empty;
        Code = code ?? string.Empty;
        Display = display ?? string.Empty;
        Version = version ?? string.Empty;
        Definition = definition ?? string.Empty;

        if (string.IsNullOrEmpty(systemLocalName))
        {
            if (string.IsNullOrEmpty(system))
            {
                SystemLocalName = string.Empty;
            }
            else if (system.StartsWith("http://hl7.org/fhir/", StringComparison.Ordinal))
            {
                string name = system.Substring(20);
                SystemLocalName = FhirUtils.SanitizeForProperty(name, null);
            }
            else if (system.StartsWith("http://terminology.hl7.org/CodeSystem/", StringComparison.Ordinal))
            {
                string name = system.Substring(38);
                SystemLocalName = FhirUtils.SanitizeForProperty(name, null);
            }
        }
        else
        {
            SystemLocalName = FhirUtils.SanitizeForProperty(systemLocalName, null);
        }
    }

    /// <summary>
    /// Initializes a new instance of the Microsoft.Health.Fhir.CodeGenCommon.Models.FhirConcept
    /// class.
    /// </summary>
    /// <param name="system">              The system.</param>
    /// <param name="code">                The code.</param>
    /// <param name="display">             The display.</param>
    /// <param name="version">             The version.</param>
    /// <param name="definition">          The definition.</param>
    /// <param name="systemLocalName">     The name of the system.</param>
    /// <param name="properties">          The properties.</param>
    /// <param name="propertyKeyValueHash">The properties and values.</param>
    public FhirConcept(
        string system,
        string code,
        string display,
        string version,
        string definition,
        string systemLocalName,
        Dictionary<string, List<object>> properties,
        HashSet<string> propertyKeyValueHash)
        :this(
             system,
             code,
             display,
             version,
             definition,
             systemLocalName)
    {
        foreach ((string propCode, List<object> propValues) in properties)
        {
            _properties.Add(propCode, propValues.Select(obj => obj).ToList());
        }

        foreach (string hash in propertyKeyValueHash)
        {
            _propertyKeyValueHash.Add(hash);
        }
    }

    /// <summary>Initializes a new instance of the <see cref="FhirConcept"/> class.</summary>
    /// <param name="system">    The system.</param>
    /// <param name="code">      The code.</param>
    /// <param name="display">   The display.</param>
    /// <param name="version">   The version.</param>
    /// <param name="definition">The definition.</param>
    public FhirConcept(
        string system,
        string code,
        string display,
        string version,
        string definition)
        : this(
            system,
            code,
            display,
            version,
            definition,
            string.Empty)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="FhirConcept"/> class.</summary>
    /// <param name="system"> The system.</param>
    /// <param name="code">   The code.</param>
    /// <param name="display">The display.</param>
    /// <param name="version">The version.</param>
    public FhirConcept(
        string system,
        string code,
        string display,
        string version)
        : this(
            system,
            code,
            display,
            version,
            string.Empty,
            string.Empty)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirConcept"/> class.
    /// </summary>
    /// <param name="system"> The system.</param>
    /// <param name="code">   The code.</param>
    /// <param name="display">The display.</param>
    public FhirConcept(
        string system,
        string code,
        string display)
        : this(
            system,
            code,
            display,
            string.Empty,
            string.Empty,
            string.Empty)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="FhirConcept"/> class.</summary>
    /// <param name="system">The system.</param>
    /// <param name="code">  The code.</param>
    public FhirConcept(
        string system,
        string code)
        : this(
            system,
            code,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirConcept"/> class.
    /// </summary>
    /// <param name="source">Source for the.</param>
    public FhirConcept(FhirConcept source)
    {
        System = source.System;
        Code = source.Code;
        Display = source.Display;
        Version = source.Version;
        Definition = source.Definition;
        SystemLocalName = source.SystemLocalName;

        foreach ((string propCode, List<object> propValues) in source._properties)
        {
            _properties.Add(propCode, propValues.Select(obj => obj).ToList());
        }

        foreach (string hash in source._propertyKeyValueHash)
        {
            _propertyKeyValueHash.Add(hash);
        }
    }

    /// <summary>Initializes a new instance of the <see cref="FhirConcept"/> class.</summary>
    private FhirConcept()
        : this(
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty)
    {
    }

    /// <summary>Gets the system.</summary>
    /// <value>The system.</value>
    public string System { get; }

    /// <summary>Gets the name of the system.</summary>
    /// <value>The name of the system.</value>
    public string SystemLocalName { get; }

    /// <summary>Gets the code.</summary>
    /// <value>The code.</value>
    public string Code { get; }

    /// <summary>Gets the display.</summary>
    /// <value>The display.</value>
    public string Display { get; }

    /// <summary>Gets the version.</summary>
    /// <value>The version.</value>
    public string Version { get; }

    /// <summary>Gets the definition.</summary>
    /// <value>The definition.</value>
    public string Definition { get; }

    /// <summary>System and code.</summary>
    /// <returns>A string.</returns>
    public string SystemAndCode() => $"{System}#{Code}";

    /// <summary>Code and system.</summary>
    /// <returns>A string.</returns>
    public string CodeAndSystem() => $"{Code} {System}";

    /// <summary>Gets the key.</summary>
    /// <returns>A string.</returns>
    public string Key() => $"{System}#{Code}";

    /// <summary>Gets the defined concept properties.</summary>
    public Dictionary<string, List<object>> Properties => _properties;

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    public object Clone()
    {
        return new FhirConcept(this);
    }

    /// <summary>Adds a property.</summary>
    /// <param name="code">              The code.</param>
    /// <param name="value">             The value.</param>
    /// <param name="canonicalizedValue">The canonicalized version of the value (for matching).</param>
    public void AddProperty(
        string code,
        object value,
        string canonicalizedValue)
    {
        if (!_properties.ContainsKey(code))
        {
            _properties.Add(code, new());
        }

        _properties[code].Add(value);
        _propertyKeyValueHash.Add(code + ":" + canonicalizedValue);
    }

    /// <summary>Matches properties.</summary>
    /// <param name="propertyHashes">The properties.</param>
    /// <returns>True if matches properties, false if not.</returns>
    public bool MatchesProperties(List<string> propertyHashes)
    {
        if ((propertyHashes == null) || (!propertyHashes.Any()))
        {
            return true;
        }

        if ((_propertyKeyValueHash == null) || (!_propertyKeyValueHash.Any()))
        {
            return false;
        }

        foreach (string hash in propertyHashes)
        {
            if (!_propertyKeyValueHash.Contains(hash))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Query if this Concept has a property with the specified value.</summary>
    /// <param name="propertyName"> Name of the property.</param>
    /// <param name="propertyValue">The property value.</param>
    /// <returns>True if this concept matches, false if not.</returns>
    public bool HasProperty(string propertyName, string propertyValue)
    {
        if (_propertyKeyValueHash == null)
        {
            return false;
        }

        string combined = propertyName + ":" + propertyValue;

        return _propertyKeyValueHash.Contains(combined);
    }

    /// <summary>Gets property hash.</summary>
    /// <param name="valueSystem"> The value system.</param>
    /// <param name="valueCode">   The value code.</param>
    /// <param name="valueVersion">The value version.</param>
    /// <returns>The property hash.</returns>
    public static string GetCanonical(
        string valueSystem,
        string valueCode,
        string valueVersion)
    {
        string value = string.Empty;

        if (!string.IsNullOrEmpty(valueSystem))
        {
            value = value + valueSystem;
        }

        if (!string.IsNullOrEmpty(valueCode))
        {
            if (string.IsNullOrEmpty(value))
            {
                value = valueCode;
            }
            else
            {
                value = value + "#" + valueCode;
            }
        }

        if (!string.IsNullOrEmpty(valueVersion))
        {
            if (string.IsNullOrEmpty(value))
            {
                value = valueVersion;
            }
            else
            {
                value = value + "|" + valueVersion;
            }
        }

        return value;
    }
}
