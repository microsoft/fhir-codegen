// <copyright file="Signature.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_43_50;

public class Signature_43_50 : ICrossVersionProcessor<Signature>, ICrossVersionExtractor<Signature>
{
	private Converter_43_50 _converter;
	internal Signature_43_50(Converter_43_50 converter)
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

			case "who":
				current.Who = new ResourceReference(node.Text);
				break;

			case "onBehalfOf":
				current.OnBehalfOf = new ResourceReference(node.Text);
				break;

			case "targetFormat":
				current.TargetFormatElement = new Code(node.Text);
				break;

			case "_targetFormat":
				_converter._element.Process(node, current.TargetFormatElement);
				break;

			case "sigFormat":
				current.SigFormatElement = new Code(node.Text);
				break;

			case "_sigFormat":
				_converter._element.Process(node, current.SigFormatElement);
				break;

			case "data":
				current.DataElement = new Base64Binary(_converter._primitive.GetByteArrayOpt(node));
				break;

			case "_data":
				_converter._element.Process(node, current.DataElement);
				break;

			// process inherited elements
			default:
				_converter._element.Process(node, current);
				break;

		}
	}
}
