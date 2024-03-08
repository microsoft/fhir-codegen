// <copyright file="Period.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_30_50;

public class Period_30_50 : ICrossVersionProcessor<Period>, ICrossVersionExtractor<Period>
{
	private Converter_30_50 _converter;
	internal Period_30_50(Converter_30_50 converter)
	{
		_converter = converter;
	}

	public Period Extract(ISourceNode node)
	{
		Period v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, Period current)
	{
		switch (node.Name)
		{
			case "start":
				current.StartElement = new FhirDateTime(node.Text);
				break;

			case "_start":
				_converter._element.Process(node, current.StartElement);
				break;

			case "end":
				current.EndElement = new FhirDateTime(node.Text);
				break;

			case "_end":
				_converter._element.Process(node, current.EndElement);
				break;

			// process inherited elements
			default:
				_converter._element.Process(node, current);
				break;

		}
	}
}
