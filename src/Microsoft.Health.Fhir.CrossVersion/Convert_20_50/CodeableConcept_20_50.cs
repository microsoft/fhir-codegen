// <copyright file="CodeableConcept.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_20_50;

public class CodeableConcept_20_50 : ICrossVersionProcessor<CodeableConcept>, ICrossVersionExtractor<CodeableConcept>
{
	private Converter_20_50 _converter;
	internal CodeableConcept_20_50(Converter_20_50 converter)
	{
		_converter = converter;
	}

	public CodeableConcept Extract(ISourceNode node)
	{
		CodeableConcept v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, CodeableConcept current)
	{
		switch (node.Name)
		{
			case "coding":
				current.Coding.Add(_converter._coding.Extract(node));
				break;

			case "text":
				current.TextElement = new FhirString(node.Text);
				break;

			case "_text":
				_converter._element.Process(node, current.TextElement);
				break;

			// process inherited elements
			default:
				_converter._element.Process(node, current);
				break;

		}
	}
}
