// <copyright file="FhirCapOperation.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Xml.Linq;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>A FHIR Operation, as listed in a CapabilityStatement.</summary>
public class FhirCapOperation : ICloneable
{
    private List<string> _additionalDefinitions;

    /// <summary>Initializes a new instance of the <see cref="FhirCapOperation"/> class.</summary>
    /// <param name="name">               The name.</param>
    /// <param name="definitionCanonical">The definition canonical.</param>
    /// <param name="documentation">      The documentation.</param>
    /// <param name="expectation">        The conformance expectation.</param>
    public FhirCapOperation(
        string name,
        string definitionCanonical,
        string documentation,
        string expectation)
    {
        Name = name;
        DefinitionCanonical = definitionCanonical;
        Documentation = documentation;
        _additionalDefinitions = null;
        ExpectationLiteral = expectation;
        if (expectation.TryFhirEnum<FhirCapabiltyStatement.ExpectationCodes>(out object expect))
        {
            Expectation = (FhirCapabiltyStatement.ExpectationCodes)expect;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirCapOperation"/> class.
    /// </summary>
    /// <param name="source">Source for the.</param>
    public FhirCapOperation(FhirCapOperation source)
    {
        Name = source.Name;
        DefinitionCanonical = source.DefinitionCanonical;
        Documentation = source.Documentation;
        _additionalDefinitions = source._additionalDefinitions?.Select(s => s).ToList() ?? null;
        ExpectationLiteral = source.ExpectationLiteral;
        Expectation = source.Expectation;
    }

    /// <summary>Gets the name.</summary>
    public string Name { get; }

    /// <summary>Gets the definition canonical.</summary>
    public string DefinitionCanonical { get; }

    /// <summary>Gets the documentation.</summary>
    public string Documentation { get; }

    /// <summary>Gets the additional definitions.</summary>
    public List<string> AdditionalDefinitions { get; }

    /// <summary>Gets the conformance expectation literal.</summary>
    public string ExpectationLiteral { get; }

    /// <summary>Gets the conformance expectation.</summary>
    public FhirCapabiltyStatement.ExpectationCodes? Expectation { get; }

    /// <summary>Adds a definition.</summary>
    /// <param name="definitionCanonical">The definition canonical.</param>
    public void AddDefinition(string definitionCanonical)
    {
        if (_additionalDefinitions == null)
        {
            _additionalDefinitions = new List<string>();
        }

        _additionalDefinitions.Add(definitionCanonical);
    }

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    public object Clone()
    {
        return new FhirCapOperation(this);
    }
}
