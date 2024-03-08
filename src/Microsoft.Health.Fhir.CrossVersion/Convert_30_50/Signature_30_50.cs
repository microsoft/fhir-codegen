// <copyright file="Signature.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_30_50;

public class Signature_30_50 : ICrossVersionProcessor<Signature>, ICrossVersionExtractor<Signature>
{
	private Converter_30_50 _converter;
	internal Signature_30_50(Converter_30_50 converter)
	{
		_converter = converter;
	}

	public Signature Extract(ISourceNode node)
	{
		Signature v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, Signature current)
	{
		switch (node.Name)
		{
			case "type":
				current.Type.Add(_converter._coding.Extract(node));
				break;

			case "when":
				current.WhenElement = new Instant(new FhirDateTime(node.Text).ToDateTimeOffset(TimeSpan.Zero));
				break;

			case "_when":
				_converter._element.Process(node, current.WhenElement);
				break;

			case "whoUri":
				// element Signature.who[x] has been removed in the target spec
				break;

			case "whoReference":
				// element Signature.who[x] has been removed in the target spec
				break;

			case "onBehalfOfUri":
				// element Signature.onBehalfOf[x] has been removed in the target spec
				break;

			case "onBehalfOfReference":
				// element Signature.onBehalfOf[x] has been removed in the target spec
				break;

			case "contentType":
				// element Signature.contentType has been removed in the target spec
				break;

			case "blob":
				// element Signature.blob has been removed in the target spec
				break;

			// process inherited elements
			default:
				_converter._element.Process(node, current);
				break;

		}
	}
}
