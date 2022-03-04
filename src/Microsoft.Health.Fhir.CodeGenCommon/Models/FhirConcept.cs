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
    /// <summary>The properties and values.</summary>
    private HashSet<string> _propertiesAndValues = null;

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

    /// <summary>Initializes a new instance of the <see cref="FhirConcept"/> class.</summary>
    /// <param name="system">         The system.</param>
    /// <param name="code">           The code.</param>
    /// <param name="display">        The display.</param>
    /// <param name="version">        The version.</param>
    /// <param name="definition">     The definition.</param>
    /// <param name="systemLocalName">The name of the system.</param>
    /// <param name="properties">     The properties.</param>
    public FhirConcept(
        string system,
        string code,
        string display,
        string version,
        string definition,
        string systemLocalName,
        List<KeyValuePair<string, string>> properties)
        : this(
            system,
            code,
            display,
            version,
            definition,
            systemLocalName)
    {
        if ((properties != null) && (properties.Count > 0))
        {
            _propertiesAndValues = new HashSet<string>();

            foreach (KeyValuePair<string, string> kvp in properties)
            {
                string combined = kvp.Key + ":" + kvp.Value;

                if (!_propertiesAndValues.Contains(combined))
                {
                    _propertiesAndValues.Add(combined);
                }
            }
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

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    public object Clone()
    {
        return new FhirConcept(System, Code, Display, Version, Definition, SystemLocalName);
    }

    /// <summary>Matches properties.</summary>
    /// <param name="properties">The properties.</param>
    /// <returns>True if matches properties, false if not.</returns>
    public bool MatchesProperties(List<KeyValuePair<string, string>> properties)
    {
        if (properties == null)
        {
            return true;
        }

        if (_propertiesAndValues == null)
        {
            return false;
        }

        foreach (KeyValuePair<string, string> kvp in properties)
        {
            string combined = kvp.Key + ":" + kvp.Value;

            if (!_propertiesAndValues.Contains(combined))
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
        if (_propertiesAndValues == null)
        {
            return false;
        }

        string combined = propertyName + ":" + propertyValue;

        return _propertiesAndValues.Contains(combined);
    }
}
