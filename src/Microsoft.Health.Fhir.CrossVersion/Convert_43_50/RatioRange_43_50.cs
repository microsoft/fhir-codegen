// <copyright file="RatioRange.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_43_50;

public class RatioRange_43_50 : ICrossVersionProcessor<RatioRange>, ICrossVersionExtractor<RatioRange>
{
	private Converter_43_50 _converter;
	internal RatioRange_43_50(Converter_43_50 converter)
	{
		_converter = converter;
	}

	public RatioRange Extract(ISourceNode node)
	{
		RatioRange v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, RatioRange current)
	{
		switch (node.Name)
		{
			case "lowNumerator":
				current.LowNumerator = _converter._quantity.Extract(node);
				break;

			case "highNumerator":
				current.HighNumerator = _converter._quantity.Extract(node);
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
