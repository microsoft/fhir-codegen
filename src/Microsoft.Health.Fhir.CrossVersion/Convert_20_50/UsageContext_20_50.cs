// <copyright file="UsageContext.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_20_50;

public class UsageContext_20_50 : ICrossVersionProcessor<UsageContext>, ICrossVersionExtractor<UsageContext>
{
	private Converter_20_50 _converter;
	internal UsageContext_20_50(Converter_20_50 converter)
	{
		_converter = converter;
	}

	public UsageContext Extract(ISourceNode node)
	{
		UsageContext v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, UsageContext current)
	{
		switch (node.Name)
		{
			case "code":
				current.Code = _converter._coding.Extract(node);
				break;

			case "valueCodeableConcept":
				current.Value = _converter._codeableConcept.Extract(node);
				break;

			case "valueQuantity":
				current.Value = _converter._quantity.Extract(node);
				break;

			case "valueRange":
				current.Value = _converter._range.Extract(node);
				break;

			// process inherited elements
			default:
				_converter._element.Process(node, current);
				break;

		}
	}
}
