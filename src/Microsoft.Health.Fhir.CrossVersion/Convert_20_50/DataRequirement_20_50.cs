// <copyright file="DataRequirement.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_20_50;

public class DataRequirement_20_50 : ICrossVersionProcessor<DataRequirement>, ICrossVersionExtractor<DataRequirement>
{
	private Converter_20_50 _converter;
	internal DataRequirement_20_50(Converter_20_50 converter)
	{
		_converter = converter;
	}

	public DataRequirement Extract(ISourceNode node)
	{
		DataRequirement v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, DataRequirement current)
	{
		switch (node.Name)
		{
			case "type":
				current.TypeElement = new Code<FHIRAllTypes>(Hl7.Fhir.Utility.EnumUtility.ParseLiteral<FHIRAllTypes>(node.Text));
				break;

			case "_type":
				_converter._element.Process(node, current.TypeElement);
				break;

			case "profile":
				current.ProfileElement.Add(new Canonical(node.Text));
				break;

			case "mustSupport":
				current.MustSupportElement.Add(new FhirString(node.Text));
				break;

			case "codeFilter":
				current.CodeFilter.Add(Extract20DataRequirementCodeFilterComponent(node));
				break;

			case "dateFilter":
				current.DateFilter.Add(Extract20DataRequirementDateFilterComponent(node));
				break;

			// process inherited elements
			default:
				_converter._element.Process(node, current);
				break;

		}
	}

	private DataRequirement.CodeFilterComponent Extract20DataRequirementCodeFilterComponent(ISourceNode parent)
	{
		DataRequirement.CodeFilterComponent current = new();

		foreach (ISourceNode node in parent.Children())
		{
			switch (node.Name)
			{
				case "path":
					current.PathElement = new FhirString(node.Text);
					break;

				case "_path":
					_converter._element.Process(node, current.PathElement);
					break;

				case "valueSetString":
					// element DataRequirement.codeFilter.valueSet[x] has been removed in the target spec
					break;

				case "valueSetReference":
					// element DataRequirement.codeFilter.valueSet[x] has been removed in the target spec
					break;

				case "valueCode":
					// element DataRequirement.codeFilter.valueCode has been removed in the target spec
					break;

				case "valueCoding":
					// element DataRequirement.codeFilter.valueCoding has been removed in the target spec
					break;

				case "valueCodeableConcept":
					// element DataRequirement.codeFilter.valueCodeableConcept has been removed in the target spec
					break;

				// process inherited elements
				default:
					_converter._element.Process(node, current);
					break;

			}
		}

		return current;
	}

	private DataRequirement.DateFilterComponent Extract20DataRequirementDateFilterComponent(ISourceNode parent)
	{
		DataRequirement.DateFilterComponent current = new();

		foreach (ISourceNode node in parent.Children())
		{
			switch (node.Name)
			{
				case "path":
					current.PathElement = new FhirString(node.Text);
					break;

				case "_path":
					_converter._element.Process(node, current.PathElement);
					break;

				case "valueDateTime":
					current.Value = new FhirDateTime(node.Text);
					break;

				case "_valueDateTime":
					_converter._element.Process(node, current.Value);
					break;

				case "valuePeriod":
					current.Value = _converter._period.Extract(node);
					break;

				case "valueDuration":
					current.Value = _converter._duration.Extract(node);
					break;

				// process inherited elements
				default:
					_converter._element.Process(node, current);
					break;

			}
		}

		return current;
	}
}
