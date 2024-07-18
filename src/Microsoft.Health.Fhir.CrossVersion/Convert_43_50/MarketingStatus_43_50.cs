// <copyright file="MarketingStatus.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_43_50;

public class MarketingStatus_43_50 : ICrossVersionProcessor<MarketingStatus>, ICrossVersionExtractor<MarketingStatus>
{
	private Converter_43_50 _converter;
	internal MarketingStatus_43_50(Converter_43_50 converter)
	{
		_converter = converter;
	}

	public MarketingStatus Extract(ISourceNode node)
	{
		MarketingStatus v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, MarketingStatus current)
	{
		switch (node.Name)
		{
			case "country":
				current.Country = _converter._codeableConcept.Extract(node);
				break;

			case "jurisdiction":
				current.Jurisdiction = _converter._codeableConcept.Extract(node);
				break;

			case "status":
				current.Status = _converter._codeableConcept.Extract(node);
				break;

			case "dateRange":
				current.DateRange = _converter._period.Extract(node);
				break;

			case "restoreDate":
				current.RestoreDateElement = new FhirDateTime(node.Text);
				break;

			case "_restoreDate":
				_converter._element.Process(node, current.RestoreDateElement);
				break;

			// process inherited elements
			default:
				_converter._element.Process(node, current);
				break;

		}
	}
}
