// <copyright file="FhirSearchParam.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>A fhir search parameter.</summary>
public class FhirSearchParam : ICloneable
{
    /// <summary>Initializes a new instance of the <see cref="FhirSearchParam"/> class.</summary>
    /// <param name="id">               The identifier.</param>
    /// <param name="url">              The URL.</param>
    /// <param name="version">          The version.</param>
    /// <param name="name">             The name.</param>
    /// <param name="description">      The description.</param>
    /// <param name="definition">       The purpose.</param>
    /// <param name="code">             The code.</param>
    /// <param name="resourceTypes">    The type of the resource.</param>
    /// <param name="targets">          The targets.</param>
    /// <param name="valueType">        The type of the value.</param>
    /// <param name="publicationStatus">The publication status.</param>
    /// <param name="standardStatus">   The standard status.</param>
    /// <param name="fmmLevel">         The fmm level.</param>
    /// <param name="isExperimental">   True if is experimental, false if not.</param>
    /// <param name="xpath">            The xpath.</param>
    /// <param name="xpathUsage">       The xpath usage.</param>
    /// <param name="expression">       The expression.</param>
    [System.Text.Json.Serialization.JsonConstructor]
    public FhirSearchParam(
        string id,
        Uri url,
        string version,
        string name,
        string description,
        string definition,
        string code,
        List<string> resourceTypes,
        List<string> targets,
        string valueType,
        string publicationStatus,
        string standardStatus,
        int? fmmLevel,
        bool isExperimental,
        string xpath,
        string xpathUsage,
        string expression,
        List<FhirSearchParamComponent> components = null)
    {
        Id = id;
        Version = version;
        Name = name;
        Description = description ?? string.Empty;
        Purpose = definition;
        Code = code;
        ValueType = valueType;
        URL = url;
        PublicationStatus = publicationStatus;
        StandardStatus = standardStatus;
        FhirMaturityLevel = fmmLevel;
        IsExperimental = isExperimental;
        XPath = xpath ?? string.Empty;
        XPathUsage = xpathUsage;
        Expression = expression ?? string.Empty;

        if (resourceTypes != null)
        {
            ResourceTypes = resourceTypes.Select(s => (string)s.Clone()).ToList();
        }

        if (targets != null)
        {
            Targets = targets.Select(s => (string)s.Clone()).ToList();
        }

        Components = components?.Select(c => (FhirSearchParamComponent)c.Clone()).ToList();
    }

    /// <summary>Values that represent search magic parameters.</summary>
    public enum ParameterGrouping
    {
        /// <summary>An enum constant representing all resource option.</summary>
        Global,

        /// <summary>An enum constant representing the search result option.</summary>
        Result,

        /// <summary>An enum constant representing all interaction option.</summary>
        Interaction,
    }

    /// <summary>Gets the identifier.</summary>
    /// <value>The identifier.</value>
    public string Id { get; }

    /// <summary>Gets URL of the document.</summary>
    /// <value>The URL.</value>
    public Uri URL { get; }

    /// <summary>Gets the version.</summary>
    /// <value>The version.</value>
    public string Version { get; }

    /// <summary>Gets the name.</summary>
    /// <value>The name.</value>
    public string Name { get; }

    /// <summary>Gets the purpose.</summary>
    /// <value>The purpose.</value>
    public string Purpose { get; }

    /// <summary>Gets the code.</summary>
    /// <value>The code.</value>
    public string Code { get; }

    /// <summary>Gets the description.</summary>
    /// <value>The description.</value>
    public string Description { get; }

    /// <summary>Gets the type of the resource.</summary>
    /// <value>The type of the resource.</value>
    public List<string> ResourceTypes { get; }

    /// <summary>Gets the resource (e.g., reference) targets.</summary>
    public List<string> Targets { get; }

    /// <summary>Gets the type of the value.</summary>
    /// <value>The type of the value.</value>
    public string ValueType { get; }

    /// <summary>Gets the standard status.</summary>
    /// <summary>Gets the publication status.</summary>
    public string PublicationStatus { get; }

    /// <summary>
    /// Gets status of this type in the standards process
    /// see: http://hl7.org/fhir/valueset-standards-status.html.
    /// </summary>
    /// <value>The standard status.</value>
    public string StandardStatus { get; }

    /// <summary>Gets the FHIR maturity level.</summary>
    public int? FhirMaturityLevel { get; }

    /// <summary>Gets a value indicating whether this object is experimental.</summary>
    /// <value>True if this object is experimental, false if not.</value>
    public bool IsExperimental { get; }

    /// <summary>Gets the XPath specification for this search parameter.</summary>
    public string XPath { get; }

    /// <summary>Gets the XPath usage information.</summary>
    public string XPathUsage { get; }

    /// <summary>Gets the expression.</summary>
    public string Expression { get; }

    /// <summary>Gets the components.</summary>
    public List<FhirSearchParamComponent> Components { get; }

    /// <summary>Deep copy.</summary>
    /// <returns>A FhirSearchParam.</returns>
    public object Clone()
    {
        return new FhirSearchParam(
            Id,
            URL,
            Version,
            Name,
            Description,
            Purpose,
            Code,
            ResourceTypes,
            Targets,
            ValueType,
            PublicationStatus,
            StandardStatus,
            FhirMaturityLevel,
            IsExperimental,
            XPath,
            XPathUsage,
            Expression,
            Components?.Select(c => (FhirSearchParamComponent)c.Clone()).ToList()
        );
    }
}
