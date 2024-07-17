// <copyright file="SampledData.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_43_50;

public class SampledData_43_50 : ICrossVersionProcessor<SampledData>, ICrossVersionExtractor<SampledData>
{
	private Converter_43_50 _converter;
	internal SampledData_43_50(Converter_43_50 converter)
	{
		_converter = converter;
	}

	public SampledData Extract(ISourceNode node)
	{
		SampledData v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, SampledData current)
	{
		switch (node.Name)
		{
			case "origin":
				current.Origin = _converter._quantity.Extract(node);
				break;

			case "period":
				// element SampledData.period has been removed in the target spec
				break;

			case "factor":
				current.FactorElement = new FhirDecimal(_converter._primitive.GetDecimalOpt(node));
				break;

			case "_factor":
				_converter._element.Process(node, current.FactorElement);
				break;

			case "lowerLimit":
				current.LowerLimitElement = new FhirDecimal(_converter._primitive.GetDecimalOpt(node));
				break;

			case "_lowerLimit":
				_converter._element.Process(node, current.LowerLimitElement);
				break;

			case "upperLimit":
				current.UpperLimitElement = new FhirDecimal(_converter._primitive.GetDecimalOpt(node));
				break;

			case "_upperLimit":
				_converter._element.Process(node, current.UpperLimitElement);
				break;

			case "dimensions":
				current.DimensionsElement = new PositiveInt(_converter._primitive.GetInt(node));
				break;

			case "_dimensions":
				_converter._element.Process(node, current.DimensionsElement);
				break;

			case "data":
				current.DataElement = new FhirString(node.Text);
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
