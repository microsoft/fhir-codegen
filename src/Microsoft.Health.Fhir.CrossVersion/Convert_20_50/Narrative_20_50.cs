// <copyright file="Narrative.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_20_50;

public class Narrative_20_50 : ICrossVersionProcessor<Narrative>, ICrossVersionExtractor<Narrative>
{
	private Converter_20_50 _converter;
	internal Narrative_20_50(Converter_20_50 converter)
	{
		_converter = converter;
	}

	public Narrative Extract(ISourceNode node)
	{
		Narrative v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, Narrative current)
	{
		switch (node.Name)
		{
			case "status":
				current.Status = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<Narrative.NarrativeStatus>(node.Text);
				break;

			case "_status":
				_converter._element.Process(node, current.StatusElement);
				break;

			case "div":
				current.Div = node.Text;
				break;

			// process inherited elements
			default:
				_converter._element.Process(node, current);
				break;

		}
	}
}
