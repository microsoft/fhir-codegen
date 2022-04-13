// <copyright file="FhirCodeSystem.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>A fhir code system.</summary>
public class FhirCodeSystem
{
    /// <summary>A code system filter.</summary>
    public readonly record struct FilterDefinition(
        string Code,
        string Description,
        IEnumerable<string> Operator,
        string Value);

    /// <summary>Values that represent property type enums.</summary>
    public enum PropertyTypeEnum
    {
        Code,
        Coding,
        String,
        Integer,
        Boolean,
        DateTime,
        Decimal,
    }

    /// <summary>Property type from value.</summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when one or more arguments are outside the
    ///  required range.</exception>
    /// <param name="value">The value.</param>
    /// <returns>A PropertyTypeEnum.</returns>
    public static PropertyTypeEnum PropertyTypeFromValue(string value)
    {
        switch (value.ToLowerInvariant())
        {
            case "code":     return PropertyTypeEnum.Code;
            case "coding":   return PropertyTypeEnum.Coding;
            case "string":   return PropertyTypeEnum.String;
            case "integer":  return PropertyTypeEnum.Integer;
            case "boolean":  return PropertyTypeEnum.Boolean;
            case "datetime": return PropertyTypeEnum.DateTime;
            case "decimal":  return PropertyTypeEnum.Decimal;
        }

        throw new ArgumentOutOfRangeException($"Invalid PropertyTypeEnum source: {value}");
    }

    /// <summary>A code system property.</summary>
    public readonly record struct PropertyDefinition(
        string Code,
        string PropUri,
        string Description,
        PropertyTypeEnum PropType);

    private readonly FhirConceptTreeNode _rootConcept;
    private readonly Dictionary<string, FhirConceptTreeNode> _conceptLookup;
    private readonly Dictionary<string, FilterDefinition> _filters;
    private readonly Dictionary<string, PropertyDefinition> _properties;

    /// <summary>Initializes a new instance of the <see cref="FhirCodeSystem"/> class.</summary>
    /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
    /// <param name="name">          The name.</param>
    /// <param name="id">            The identifier.</param>
    /// <param name="version">       The version.</param>
    /// <param name="title">         The title.</param>
    /// <param name="url">           The URL.</param>
    /// <param name="standardStatus">The standard status.</param>
    /// <param name="description">   The description.</param>
    /// <param name="content">       The content.</param>
    /// <param name="rootConcept">   The root concept.</param>
    /// <param name="conceptLookup"> The concept lookup.</param>
    /// <param name="filters">       The filters.</param>
    /// <param name="properties">    The properties.</param>
    public FhirCodeSystem(
        string name,
        string id,
        string version,
        string title,
        string url,
        string standardStatus,
        string description,
        string content,
        FhirConceptTreeNode rootConcept,
        Dictionary<string, FhirConceptTreeNode> conceptLookup,
        Dictionary<string, FilterDefinition> filters,
        Dictionary<string, PropertyDefinition> properties)
    {
        if (url == null)
        {
            throw new ArgumentNullException(nameof(url));
        }

        Name = name;
        Id = id;
        Version = version;
        Title = title;
        URL = url;
        StandardStatus = standardStatus;
        Description = description;
        Content = content;
        _rootConcept = rootConcept;
        _conceptLookup = conceptLookup;
        _filters = filters;
        _properties = properties;
    }

    /// <summary>Gets the name.</summary>
    /// <value>The name.</value>
    public string Name { get; }

    /// <summary>Gets the identifier.</summary>
    /// <value>The identifier.</value>
    public string Id { get; }

    /// <summary>Gets the version.</summary>
    /// <value>The version.</value>
    public string Version { get; }

    /// <summary>Gets the title.</summary>
    /// <value>The title.</value>
    public string Title { get; }

    /// <summary>Gets URL of the document.</summary>
    /// <value>The URL.</value>
    public string URL { get; }

    /// <summary>Gets the standard status.</summary>
    /// <value>The standard status.</value>
    public string StandardStatus { get; }

    /// <summary>Gets the description.</summary>
    /// <value>The description.</value>
    public string Description { get; }

    /// <summary>Gets the content.</summary>
    /// <value>The content.</value>
    public string Content { get; }

    /// <summary>Gets the root concept.</summary>
    /// <value>The root concept.</value>
    public FhirConceptTreeNode RootConcept => _rootConcept;

    /// <summary>Gets the concepts (by code).</summary>
    /// <value>The concepts (by code).</value>
    public Dictionary<string, FhirConceptTreeNode> ConceptLookup => _conceptLookup;

    /// <summary>Gets the filters.</summary>
    public Dictionary<string, FilterDefinition> Filters => _filters;

    /// <summary>Gets the properties.</summary>
    public Dictionary<string, PropertyDefinition> Properties => _properties;

    /// <summary>Indexer to get slices based on name.</summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when one or more arguments are outside the
    ///  required range.</exception>
    /// <param name="code">The code.</param>
    /// <returns>The indexed item.</returns>
    public FhirConceptTreeNode this[string code]
    {
        get
        {
            if (!_conceptLookup.ContainsKey(code))
            {
                throw new ArgumentOutOfRangeException(nameof(code));
            }

            return _conceptLookup[code];
        }
    }

    /// <summary>Query if this system contains a concept, specified by code.</summary>
    /// <param name="code">The code.</param>
    /// <returns>True if this system has the concept, false if it does not.</returns>
    public bool ContainsConcept(string code)
    {
        return _conceptLookup.ContainsKey(code);
    }
}
