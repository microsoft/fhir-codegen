// <copyright file="ProductShelfLife.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_43_50;

public class ProductShelfLife_43_50 : ICrossVersionProcessor<ProductShelfLife>, ICrossVersionExtractor<ProductShelfLife>
{
	private Converter_43_50 _converter;
	internal ProductShelfLife_43_50(Converter_43_50 converter)
	{
		_converter = converter;
	}

	public ProductShelfLife Extract(ISourceNode node)
	{
		ProductShelfLife v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, ProductShelfLife current)
	{
		switch (node.Name)
		{
			case "identifier":
				// element ProductShelfLife.identifier has been removed in the target spec
				break;

			case "type":
				current.Type = _converter._codeableConcept.Extract(node);
				break;

			case "period":
				// element ProductShelfLife.period has been removed in the target spec
				break;

			case "specialPrecautionsForStorage":
				current.SpecialPrecautionsForStorage.Add(_converter._codeableConcept.Extract(node));
				break;

			// process inherited elements
			default:
				_converter._element.Process(node, current);
				break;

		}
	}
}
