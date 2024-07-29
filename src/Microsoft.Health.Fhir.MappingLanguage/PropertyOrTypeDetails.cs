// <copyright file="CrossVersionMapCollection.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.Specification.Navigation;
using Hl7.Fhir.Specification.Source;
using Hl7.Fhir.Utility;
using Hl7.FhirPath;

namespace Microsoft.Health.Fhir.MappingLanguage;

public class PropertyOrTypeDetails
{
    public PropertyOrTypeDetails(string propertyPath, ElementDefinitionNavigator element, IAsyncResourceResolver resolver)
    {
        PropertyPath = propertyPath;
        Element = element;
        Resolver = resolver;
    }

    public string PropertyPath { get; private set; }
    public IAsyncResourceResolver Resolver { get; private set; }
    public ElementDefinitionNavigator Element { get; private set; }

    public override string ToString()
    {
        // return $"{Definition.Url}|{Definition.Version} # {Element.Path} ({String.Join(",", Element.Current.Type.Select(t => t.Code))})";
        return $"{Element.Path}|{Element.StructureDefinition.Version} ({String.Join(",", Element.Current.Type.Select(t => t.Code))})";
    }
}
