// <copyright file="Contributor.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_20_50;

public class Contributor_20_50 : ICrossVersionProcessor<Contributor>, ICrossVersionExtractor<Contributor>
{
	private Converter_20_50 _converter;
	internal Contributor_20_50(Converter_20_50 converter)
	{
		_converter = converter;
	}

	public Contributor Extract(ISourceNode node)
	{
		Contributor v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, Contributor current)
	{
		switch (node.Name)
		{
			case "type":
				current.Type = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<Contributor.ContributorType>(node.Text);
				break;

			case "_type":
				_converter._element.Process(node, current.TypeElement);
				break;

			case "name":
				current.NameElement = new FhirString(node.Text);
				break;

			case "_name":
				_converter._element.Process(node, current.NameElement);
				break;

			case "contact":
				current.Contact.Add(_converter._contactDetail.Extract(node));
				break;

			// process inherited elements
			default:
				_converter._element.Process(node, current);
				break;

		}
	}
}
