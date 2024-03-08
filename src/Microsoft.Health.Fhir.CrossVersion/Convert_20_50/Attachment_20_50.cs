// <copyright file="Attachment.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_20_50;

public class Attachment_20_50 : ICrossVersionProcessor<Attachment>, ICrossVersionExtractor<Attachment>
{
	private Converter_20_50 _converter;
	internal Attachment_20_50(Converter_20_50 converter)
	{
		_converter = converter;
	}

	public Attachment Extract(ISourceNode node)
	{
		Attachment v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, Attachment current)
	{
		switch (node.Name)
		{
			case "contentType":
				current.ContentTypeElement = new Code(node.Text);
				break;

			case "_contentType":
				_converter._element.Process(node, current.ContentTypeElement);
				break;

			case "language":
				current.LanguageElement = new Code(node.Text);
				break;

			case "_language":
				_converter._element.Process(node, current.LanguageElement);
				break;

			case "data":
				current.DataElement = new Base64Binary(_converter._primitive.GetByteArrayOpt(node));
				break;

			case "_data":
				_converter._element.Process(node, current.DataElement);
				break;

			case "url":
				current.UrlElement = new FhirUrl(node.Text);
				break;

			case "size":
				current.SizeElement = new Integer64(_converter._primitive.GetLongOpt(node));
				break;

			case "hash":
				current.HashElement = new Base64Binary(_converter._primitive.GetByteArrayOpt(node));
				break;

			case "_hash":
				_converter._element.Process(node, current.HashElement);
				break;

			case "title":
				current.TitleElement = new FhirString(node.Text);
				break;

			case "_title":
				_converter._element.Process(node, current.TitleElement);
				break;

			case "creation":
				current.CreationElement = new FhirDateTime(node.Text);
				break;

			case "_creation":
				_converter._element.Process(node, current.CreationElement);
				break;

			// process inherited elements
			default:
				_converter._element.Process(node, current);
				break;

		}
	}
}
