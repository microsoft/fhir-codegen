// <copyright file="Address.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_43_50;

public class Address_43_50 : ICrossVersionProcessor<Address>, ICrossVersionExtractor<Address>
{
	private Converter_43_50 _converter;
	internal Address_43_50(Converter_43_50 converter)
	{
		_converter = converter;
	}

	public Address Extract(ISourceNode node)
	{
		Address v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, Address current)
	{
		switch (node.Name)
		{
			case "use":
				current.Use = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<Address.AddressUse>(node.Text);
				break;

			case "_use":
				_converter._element.Process(node, current.UseElement);
				break;

			case "type":
				current.Type = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<Address.AddressType>(node.Text);
				break;

			case "_type":
				_converter._element.Process(node, current.TypeElement);
				break;

			case "text":
				current.TextElement = new FhirString(node.Text);
				break;

			case "_text":
				_converter._element.Process(node, current.TextElement);
				break;

			case "line":
				current.LineElement.Add(new FhirString(node.Text));
				break;

			case "city":
				current.CityElement = new FhirString(node.Text);
				break;

			case "_city":
				_converter._element.Process(node, current.CityElement);
				break;

			case "district":
				current.DistrictElement = new FhirString(node.Text);
				break;

			case "_district":
				_converter._element.Process(node, current.DistrictElement);
				break;

			case "state":
				current.StateElement = new FhirString(node.Text);
				break;

			case "_state":
				_converter._element.Process(node, current.StateElement);
				break;

			case "postalCode":
				current.PostalCodeElement = new FhirString(node.Text);
				break;

			case "_postalCode":
				_converter._element.Process(node, current.PostalCodeElement);
				break;

			case "country":
				current.CountryElement = new FhirString(node.Text);
				break;

			case "_country":
				_converter._element.Process(node, current.CountryElement);
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
