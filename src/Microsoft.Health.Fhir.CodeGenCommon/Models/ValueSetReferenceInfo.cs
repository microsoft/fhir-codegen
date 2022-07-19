// <copyright file="ValueSetReferenceInfo.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>Information about the value set reference.</summary>
public class ValueSetReferenceInfo
{
    private List<string> _paths;
    private FhirElement.ElementDefinitionBindingStrength _strongestBinding;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueSetReferenceInfo"/> class.
    /// </summary>
    public ValueSetReferenceInfo()
    {
        _paths = new List<string>();
        _strongestBinding = FhirElement.ElementDefinitionBindingStrength.Example;
    }

    /// <summary>Gets the paths.</summary>
    public List<string> Paths => _paths;

    /// <summary>Gets the strongest binding.</summary>
    public FhirElement.ElementDefinitionBindingStrength StrongestBinding => _strongestBinding;

    /// <summary>Adds a path and checks for changes to strongest binding level.</summary>
    /// <param name="elementPath">Full pathname of the file.</param>
    /// <param name="strength">   The strength of the value set binding to the given element.</param>
    public void AddPath(
        string elementPath,
        FhirElement.ElementDefinitionBindingStrength? strength)
    {
        _paths.Add(elementPath);

        if (strength == null)
        {
            return;
        }

        if (strength < _strongestBinding)
        {
            _strongestBinding = (FhirElement.ElementDefinitionBindingStrength)strength;
        }
    }
}
