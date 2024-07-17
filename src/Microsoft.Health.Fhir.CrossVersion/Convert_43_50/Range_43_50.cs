// <copyright file="Range.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_43_50;

public class Range_43_50 : ICrossVersionProcessor<Hl7.Fhir.Model.Range>, ICrossVersionExtractor<Hl7.Fhir.Model.Range>
{
	private Converter_43_50 _converter;
	internal Range_43_50(Converter_43_50 converter)
	{
		_converter = converter;
	}

	public Hl7.Fhir.Model.Range Extract(ISourceNode node)
	{
        Hl7.Fhir.Model.Range v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, Hl7.Fhir.Model.Range current)
	{
		switch (node.Name)
		{
			case "low":
				current.Low = _converter._quantity.Extract(node);
				break;

			case "high":
				current.High = _converter._quantity.Extract(node);
				break;

			// process inherited elements
			default:
				_converter._element.Process(node, current);
				break;

		}
	}
}
