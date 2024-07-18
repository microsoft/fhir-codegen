// <copyright file="ContactPoint.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_20_50;

public class ContactPoint_20_50 : ICrossVersionProcessor<ContactPoint>, ICrossVersionExtractor<ContactPoint>
{
	private Converter_20_50 _converter;
	internal ContactPoint_20_50(Converter_20_50 converter)
	{
		_converter = converter;
	}

	public ContactPoint Extract(ISourceNode node)
	{
		ContactPoint v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, ContactPoint current)
	{
		switch (node.Name)
		{
			case "system":
				current.System = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<ContactPoint.ContactPointSystem>(node.Text);
				break;

			case "_system":
				_converter._element.Process(node, current.SystemElement);
				break;

			case "value":
				current.ValueElement = new FhirString(node.Text);
				break;

			case "_value":
				_converter._element.Process(node, current.ValueElement);
				break;

			case "use":
				current.Use = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<ContactPoint.ContactPointUse>(node.Text);
				break;

			case "_use":
				_converter._element.Process(node, current.UseElement);
				break;

			case "rank":
				current.RankElement = new PositiveInt(_converter._primitive.GetIntOpt(node));
				break;

			case "_rank":
				_converter._element.Process(node, current.RankElement);
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
