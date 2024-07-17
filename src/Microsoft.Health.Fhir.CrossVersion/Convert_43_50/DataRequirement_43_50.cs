// <copyright file="DataRequirement.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_43_50;

public class DataRequirement_43_50 : ICrossVersionProcessor<DataRequirement>, ICrossVersionExtractor<DataRequirement>
{
	private Converter_43_50 _converter;
	internal DataRequirement_43_50(Converter_43_50 converter)
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

			case "subjectCodeableConcept":
				current.Subject = _converter._codeableConcept.Extract(node);
				break;

			case "subjectReference":
				current.Subject = new ResourceReference(node.Text);
				break;

			case "mustSupport":
				current.MustSupportElement.Add(new FhirString(node.Text));
				break;

			case "codeFilter":
				current.CodeFilter.Add(Extract43DataRequirementCodeFilterComponent(node));
				break;

			case "dateFilter":
				current.DateFilter.Add(Extract43DataRequirementDateFilterComponent(node));
				break;

			case "limit":
				current.LimitElement = new PositiveInt(_converter._primitive.GetIntOpt(node));
				break;

			case "_limit":
				_converter._element.Process(node, current.LimitElement);
				break;

			case "sort":
				current.Sort.Add(Extract43DataRequirementSortComponent(node));
				break;

			// process inherited elements
			default:
				_converter._element.Process(node, current);
				break;

		}
	}

	private DataRequirement.CodeFilterComponent Extract43DataRequirementCodeFilterComponent(ISourceNode parent)
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

                case "searchParam":
                    current.SearchParamElement = new FhirString(node.Text);
                    break;

                case "_searchParam":
                    _converter._element.Process(node, current.SearchParamElement);
                    break;

                case "valueSet":
                    current.ValueSetElement = new Canonical(node.Text);
                    break;

                case "_valueSet":
                    _converter._element.Process(node, current.ValueSetElement);
                    break;

                case "code":
                    current.Code.Add(_converter._coding.Extract(node));
                    break;

                // process inherited elements
                default:
                    _converter._element.Process(node, current);
                    break;
            }
		}

		return current;
	}

	private DataRequirement.DateFilterComponent Extract43DataRequirementDateFilterComponent(ISourceNode parent)
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

                case "searchParam":
                    current.SearchParamElement = new FhirString(node.Text);
                    break;

                case "_searchParam":
                    _converter._element.Process(node, current.SearchParamElement);
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

	private DataRequirement.SortComponent Extract43DataRequirementSortComponent(ISourceNode parent)
	{
		DataRequirement.SortComponent current = new();

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

                case "direction":
                    current.Direction = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<DataRequirement.SortDirection>(node.Text);
                    break;

                case "_direction":
                    _converter._element.Process(node, current.DirectionElement);
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
