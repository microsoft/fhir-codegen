// <copyright file="ContactDetail.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_43_50;

public class ContactDetail_43_50 : ICrossVersionProcessor<ContactDetail>, ICrossVersionExtractor<ContactDetail>
{
	private Converter_43_50 _converter;
	internal ContactDetail_43_50(Converter_43_50 converter)
	{
		_converter = converter;
	}

	public ContactDetail Extract(ISourceNode node)
	{
		ContactDetail v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, ContactDetail current)
	{
		switch (node.Name)
		{
			case "name":
				current.NameElement = new FhirString(node.Text);
				break;

			case "_name":
				_converter._element.Process(node, current.NameElement);
				break;

			case "telecom":
				current.Telecom.Add(_converter._contactPoint.Extract(node));
				break;

			// process inherited elements
			default:
				_converter._element.Process(node, current);
				break;

		}
	}
}
