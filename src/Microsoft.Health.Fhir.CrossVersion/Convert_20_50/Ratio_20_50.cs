// <copyright file="Ratio.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_20_50;

public class Ratio_20_50 : ICrossVersionProcessor<Ratio>, ICrossVersionExtractor<Ratio>
{
	private Converter_20_50 _converter;
	internal Ratio_20_50(Converter_20_50 converter)
	{
		_converter = converter;
	}

	public Ratio Extract(ISourceNode node)
	{
		Ratio v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, Ratio current)
	{
		switch (node.Name)
		{
			case "numerator":
				current.Numerator = _converter._quantity.Extract(node);
				break;

			case "denominator":
				current.Denominator = _converter._quantity.Extract(node);
				break;

			// process inherited elements
			default:
				_converter._element.Process(node, current);
				break;

		}
	}
}
