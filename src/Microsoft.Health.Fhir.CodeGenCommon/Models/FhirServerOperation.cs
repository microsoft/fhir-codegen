// <copyright file="FhirServerOperation.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>A FHIR server operation.</summary>
public class FhirServerOperation
{
    private List<string> _additionalDefinitions;

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirServerOperation"/> class.
    /// </summary>
    /// <param name="name">               The name.</param>
    /// <param name="definitionCanonical">The definition canonical.</param>
    /// <param name="documentation">      The documentation.</param>
    public FhirServerOperation(string name, string definitionCanonical, string documentation)
    {
        Name = name;
        DefinitionCanonical = definitionCanonical;
        Documentation = documentation;
        _additionalDefinitions = null;
    }

    /// <summary>Gets the name.</summary>
    public string Name { get; }

    /// <summary>Gets the definition canonical.</summary>
    public string DefinitionCanonical { get; }

    /// <summary>Gets the documentation.</summary>
    public string Documentation { get; }

    /// <summary>Gets the additional definitions.</summary>
    public List<string> AdditionalDefinitions { get; }

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
        return new FhirServerOperation(
            Name,
            DefinitionCanonical,
            Documentation);
    }
}
