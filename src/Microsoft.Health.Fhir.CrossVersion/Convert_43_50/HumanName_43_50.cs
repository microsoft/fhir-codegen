// <copyright file="HumanName.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_43_50;

public class HumanName_43_50 : ICrossVersionProcessor<HumanName>, ICrossVersionExtractor<HumanName>
{
	private Converter_43_50 _converter;
	internal HumanName_43_50(Converter_43_50 converter)
	{
		_converter = converter;
	}

	public HumanName Extract(ISourceNode node)
	{
		HumanName v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, HumanName current)
	{
		switch (node.Name)
		{
			case "use":
				current.Use = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<HumanName.NameUse>(node.Text);
				break;

			case "_use":
				_converter._element.Process(node, current.UseElement);
				break;

			case "text":
				current.TextElement = new FhirString(node.Text);
				break;

			case "_text":
				_converter._element.Process(node, current.TextElement);
				break;

			case "family":
				current.FamilyElement = new FhirString(node.Text);
				break;

			case "_family":
				_converter._element.Process(node, current.FamilyElement);
				break;

			case "given":
				current.GivenElement.Add(new FhirString(node.Text));
				break;

			case "prefix":
				current.PrefixElement.Add(new FhirString(node.Text));
				break;

			case "suffix":
				current.SuffixElement.Add(new FhirString(node.Text));
				break;

			case "period":
				current.Period = _converter._period.Extract(node);
				break;

			// process inherited elements
			default:
				_converter._element.Process(node, current);
				break;

		}
	}
}
