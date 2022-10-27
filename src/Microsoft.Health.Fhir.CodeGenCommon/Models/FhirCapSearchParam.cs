// <copyright file="FhirCapSearchParam.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.CodeGenCommon.Extensions;

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>A FHIR search parameter record from a CapabilityStatement.</summary>
public class FhirCapSearchParam : ICloneable
{
    /// <summary>Initializes a new instance of the <see cref="FhirCapSearchParam"/> class.</summary>
    /// <param name="name">               The name.</param>
    /// <param name="definitionCanonical">The definition canonical.</param>
    /// <param name="parameterType">      The type of the parameter.</param>
    /// <param name="documentation">      The documentation.</param>
    /// <param name="expectation">        The conformance expectation.</param>
    public FhirCapSearchParam(
        string name,
        string definitionCanonical,
        string parameterType,
        string documentation,
        string expectation)
    {
        Name = name;
        DefinitionCanonical = definitionCanonical;
        ParameterType = parameterType.ToFhirEnum<SearchParameterType>();
        Documentation = documentation;
        ExpectationLiteral = expectation;
        if (expectation.TryFhirEnum<FhirCapabiltyStatement.ExpectationCodes>(out object expect))
        {
            Expectation = (FhirCapabiltyStatement.ExpectationCodes)expect;
        }
    }

    /// <summary>Initializes a new instance of the <see cref="FhirCapSearchParam"/> class.</summary>
    /// <param name="source">Source to copy.</param>
    public FhirCapSearchParam(FhirCapSearchParam source)
    {
        Name = source.Name;
        DefinitionCanonical = source.DefinitionCanonical;
        ParameterType = source.ParameterType;
        Documentation = source.Documentation;
        ExpectationLiteral = source.ExpectationLiteral;
        Expectation = source.Expectation;
    }

    /// <summary>
    /// Values that represent the type of value a search parameter refers to, and how the content is
    /// interpreted.
    /// </summary>
    public enum SearchParameterType
    {
        /// <summary>Search parameter SHALL be a number (a whole number, or a decimal).</summary>
        [FhirLiteral("number")]
        Number,

        /// <summary>Search parameter is on a date/time. The date format is the standard XML format, though other formats may be supported.</summary>
        [FhirLiteral("date")]
        Date,

        /// <summary>Search parameter is a simple string, like a name part. Search is case-insensitive and accent-insensitive. May match just the start of a string. String parameters may contain spaces.</summary>
        [FhirLiteral("string")]
#pragma warning disable CA1720 // Identifier contains type name
        String,
#pragma warning restore CA1720 // Identifier contains type name

        /// <summary>Search parameter on a coded element or identifier. May be used to search through the text, display, code and code/codesystem (for codes) and label, system and key (for identifier). Its value is either a string or a pair of namespace and value, separated by a "|", depending on the modifier used.</summary>
        [FhirLiteral("token")]
        Token,

        /// <summary>A reference to another resource (Reference or canonical).</summary>
        [FhirLiteral("reference")]
        Reference,

        /// <summary>A composite search parameter that combines a search on two values together.</summary>
        [FhirLiteral("composite")]
        Composite,

        /// <summary>A search parameter that searches on a quantity.</summary>
        [FhirLiteral("quantity")]
        Quantity,

        /// <summary>A search parameter that searches on a URI (RFC 3986).</summary>
        [FhirLiteral("uri")]
        Uri,

        /// <summary>Special logic applies to this parameter per the description of the search parameter.</summary>
        [FhirLiteral("special")]
        Special,
    }

    /// <summary>Gets the name.</summary>
    public string Name { get; }

    /// <summary>Gets the definition canonical.</summary>
    public string DefinitionCanonical { get; }

    /// <summary>Gets the type of the parameter.</summary>
    public SearchParameterType ParameterType { get; }

    /// <summary>Gets the documentation.</summary>
    public string Documentation { get; }

    /// <summary>Gets the conformance expectation literal.</summary>
    public string ExpectationLiteral { get; }

    /// <summary>Gets the conformance expectation.</summary>
    public FhirCapabiltyStatement.ExpectationCodes? Expectation { get; }

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    public object Clone()
    {
        return new FhirCapSearchParam(this);
    }
}
