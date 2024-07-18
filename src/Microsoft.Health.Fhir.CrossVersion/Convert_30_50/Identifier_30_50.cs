// <copyright file="Identifier.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_30_50;

public class Identifier_30_50 : ICrossVersionProcessor<Identifier>, ICrossVersionExtractor<Identifier>
{
	private Converter_30_50 _converter;
	internal Identifier_30_50(Converter_30_50 converter)
	{
		_converter = converter;
	}

	public Identifier Extract(ISourceNode node)
	{
		Identifier v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, Identifier current)
	{
		switch (node.Name)
		{
			case "use":
				current.Use = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<Identifier.IdentifierUse>(node.Text);
				break;

			case "_use":
				_converter._element.Process(node, current.UseElement);
				break;

			case "type":
				current.Type = _converter._codeableConcept.Extract(node);
				break;

			case "system":
				current.SystemElement = new FhirUri(node.Text);
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

			case "period":
				current.Period = _converter._period.Extract(node);
				break;

			case "assigner":
				current.Assigner = new ResourceReference(node.Text);
				break;

			// process inherited elements
			default:
				_converter._element.Process(node, current);
				break;

		}
	}
}
