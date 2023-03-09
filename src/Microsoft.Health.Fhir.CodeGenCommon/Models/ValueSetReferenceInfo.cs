// <copyright file="ValueSetReferenceInfo.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>Information about the value set reference.</summary>
public class ValueSetReferenceInfo
{
    private Dictionary<string, FhirElement> _referencingElementsByPath;
    private FhirElement.ElementDefinitionBindingStrength _strongestBinding;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueSetReferenceInfo"/> class.
    /// </summary>
    public ValueSetReferenceInfo()
    {
        _referencingElementsByPath = new();
        _strongestBinding = FhirElement.ElementDefinitionBindingStrength.Example;
    }

    /// <summary>Gets the full pathname of the referencing elements by file.</summary>
    public Dictionary<string, FhirElement> ReferencingElementsByPath => _referencingElementsByPath;

    /// <summary>Gets the strongest binding.</summary>
    public FhirElement.ElementDefinitionBindingStrength StrongestBinding => _strongestBinding;

    /// <summary>Adds a path and checks for changes to strongest binding level.</summary>
    /// <param name="element">The element.</param>
    public void AddPath(FhirElement element)
    {
        if ((element == null) ||
            string.IsNullOrEmpty(element.Path) ||
            (element.ValueSetBindingStrength == null) ||
            _referencingElementsByPath.ContainsKey(element.Path))
        {
            return;
        }

        FhirElement.ElementDefinitionBindingStrength strength = (FhirElement.ElementDefinitionBindingStrength)element.ValueSetBindingStrength!;

        _referencingElementsByPath.Add(element.Path, element);

        if (strength < _strongestBinding)
        {
            _strongestBinding = strength;
        }
    }
}
