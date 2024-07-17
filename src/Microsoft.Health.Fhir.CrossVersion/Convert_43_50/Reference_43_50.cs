// <copyright file="Reference.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_43_50;

public class Reference_43_50 : ICrossVersionProcessor<ResourceReference>, ICrossVersionExtractor<ResourceReference>
{
	private Converter_43_50 _converter;
	internal Reference_43_50(Converter_43_50 converter)
	{
		_converter = converter;
	}

	public ResourceReference Extract(ISourceNode node)
	{
        ResourceReference v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, ResourceReference current)
	{
		switch (node.Name)
		{
			case "reference":
				current.ReferenceElement = new FhirString(node.Text);
				break;

			case "_reference":
				_converter._element.Process(node, current.ReferenceElement);
				break;

			case "type":
				current.TypeElement = new FhirUri(node.Text);
				break;

			case "_type":
				_converter._element.Process(node, current.TypeElement);
				break;

			case "identifier":
				current.Identifier = _converter._identifier.Extract(node);
				break;

			case "display":
				current.DisplayElement = new FhirString(node.Text);
				break;

			case "_display":
				_converter._element.Process(node, current.DisplayElement);
				break;

			// process inherited elements
			default:
				_converter._element.Process(node, current);
				break;

		}
	}
}
