// <copyright file="Resource.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_43_50;

public class Resource_43_50 : ICrossVersionProcessor<Resource>
{
	private Converter_43_50 _converter;
	internal Resource_43_50(Converter_43_50 converter)
	{
		_converter = converter;
	}

	public Resource Extract(ISourceNode node)
    {
        switch (node.GetResourceTypeIndicator())
        {
            case "CapabilityStatement":
                return _converter._capabilityStatement.Extract(node);

            case "CodeSystem":
                return _converter._codeSystem.Extract(node);

            case "CompartmentDefinition":
                return _converter._compartmentDefinition.Extract(node);

            case "ConceptMap":
                return _converter._conceptMap.Extract(node);

            case "ImplementationGuide":
                return _converter._implementationGuide.Extract(node);

            case "OperationDefinition":
                return _converter._operationDefinition.Extract(node);

            case "SearchParameter":
                return _converter._searchParameter.Extract(node);

            case "StructureDefinition":
                return _converter._structureDefinition.Extract(node);

            case "ValueSet":
                return _converter._valueSet.Extract(node);
        }

        throw new Exception($"Unhandled resource type: {node.GetResourceTypeIndicator()}");
	}

	public void Process(ISourceNode node, Resource current)
	{
		switch (node.Name)
		{
			case "id":
				current.Id = node.Text;
				break;

			case "meta":
				current.Meta = _converter._meta.Extract(node);
				break;

			case "implicitRules":
				current.ImplicitRules = node.Text;
				break;

			case "_implicitRules":
				_converter._element.Process(node, current.ImplicitRulesElement);
				break;

			case "language":
				current.Language = node.Text;
				break;

			case "_language":
				_converter._element.Process(node, current.LanguageElement);
				break;

		}
	}
}
