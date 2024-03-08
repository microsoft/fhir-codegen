// <copyright file="Quantity.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_20_50;

public class Quantity_20_50 : ICrossVersionProcessor<Quantity>, ICrossVersionExtractor<Quantity>
{
	private Converter_20_50 _converter;
	internal Quantity_20_50(Converter_20_50 converter)
	{
		_converter = converter;
	}

	public Quantity Extract(ISourceNode node)
	{
		Quantity v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, Quantity current)
	{
		switch (node.Name)
		{
			case "value":
				current.ValueElement = new FhirDecimal(_converter._primitive.GetDecimalOpt(node));
				break;

			case "_value":
				_converter._element.Process(node, current.ValueElement);
				break;

			case "comparator":
				current.Comparator = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<Quantity.QuantityComparator>(node.Text);
				break;

			case "_comparator":
				_converter._element.Process(node, current.ComparatorElement);
				break;

			case "unit":
				current.UnitElement = new FhirString(node.Text);
				break;

			case "_unit":
				_converter._element.Process(node, current.UnitElement);
				break;

			case "system":
				current.SystemElement = new FhirUri(node.Text);
				break;

			case "_system":
				_converter._element.Process(node, current.SystemElement);
				break;

			case "code":
				current.CodeElement = new Code(node.Text);
				break;

			case "_code":
				_converter._element.Process(node, current.CodeElement);
				break;

			// process inherited elements
			default:
				_converter._element.Process(node, current);
				break;

		}
	}
}
