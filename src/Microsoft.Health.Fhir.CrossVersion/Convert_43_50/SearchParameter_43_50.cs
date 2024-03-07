// <copyright file="SearchParameter.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_43_50;

public class SearchParameter_43_50 : ICrossVersionProcessor<SearchParameter>, ICrossVersionExtractor<SearchParameter>
{
	private Converter_43_50 _converter;
	internal SearchParameter_43_50(Converter_43_50 converter)
	{
		_converter = converter;
	}

	public SearchParameter Extract(ISourceNode node)
	{
		SearchParameter v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, SearchParameter current)
	{
		switch (node.Name)
		{
			case "url":
				current.UrlElement = new FhirUri(node.Text);
				break;

			case "_url":
				_converter._element.Process(node, current.UrlElement);
				break;

			case "version":
				current.VersionElement = new FhirString(node.Text);
				break;

			case "_version":
				_converter._element.Process(node, current.VersionElement);
				break;

			case "name":
				current.NameElement = new FhirString(node.Text);
				break;

			case "_name":
				_converter._element.Process(node, current.NameElement);
				break;

			case "derivedFrom":
				current.DerivedFromElement = new Canonical(node.Text);
				break;

			case "_derivedFrom":
				_converter._element.Process(node, current.DerivedFromElement);
				break;

			case "status":
				current.StatusElement = new Code<PublicationStatus>(Hl7.Fhir.Utility.EnumUtility.ParseLiteral<PublicationStatus>(node.Text));
				break;

			case "_status":
				_converter._element.Process(node, current.StatusElement);
				break;

			case "experimental":
				current.ExperimentalElement = new FhirBoolean(_converter._primitive.GetBoolOpt(node));
				break;

			case "_experimental":
				_converter._element.Process(node, current.ExperimentalElement);
				break;

			case "date":
				current.DateElement = new FhirDateTime(node.Text);
				break;

			case "_date":
				_converter._element.Process(node, current.DateElement);
				break;

			case "publisher":
				current.PublisherElement = new FhirString(node.Text);
				break;

			case "_publisher":
				_converter._element.Process(node, current.PublisherElement);
				break;

			case "contact":
				current.Contact.Add(_converter._contactDetail.Extract(node));
				break;

			case "description":
				current.DescriptionElement = new Markdown(node.Text);
				break;

			case "_description":
				_converter._element.Process(node, current.DescriptionElement);
				break;

			case "useContext":
				current.UseContext.Add(_converter._usageContext.Extract(node));
				break;

			case "jurisdiction":
				current.Jurisdiction.Add(_converter._codeableConcept.Extract(node));
				break;

			case "purpose":
				current.PurposeElement = new Markdown(node.Text);
				break;

			case "_purpose":
				_converter._element.Process(node, current.PurposeElement);
				break;

			case "code":
				current.CodeElement = new Code(node.Text);
				break;

			case "_code":
				_converter._element.Process(node, current.CodeElement);
				break;

			case "base":
				current.BaseElement.Add(new Code<VersionIndependentResourceTypesAll>(Hl7.Fhir.Utility.EnumUtility.ParseLiteral<VersionIndependentResourceTypesAll>(node.Text)));
				break;

			case "type":
				current.TypeElement = new Code<SearchParamType>(Hl7.Fhir.Utility.EnumUtility.ParseLiteral<SearchParamType>(node.Text));
				break;

			case "_type":
				_converter._element.Process(node, current.TypeElement);
				break;

			case "expression":
				current.ExpressionElement = new FhirString(node.Text);
				break;

			case "_expression":
				_converter._element.Process(node, current.ExpressionElement);
				break;

			case "xpath":
				// element SearchParameter.xpath has been removed in the target spec
				break;

			case "xpathUsage":
				// element SearchParameter.xpathUsage has been removed in the target spec
				break;

			case "target":
				current.TargetElement.Add(new Code<VersionIndependentResourceTypesAll>(Hl7.Fhir.Utility.EnumUtility.ParseLiteral<VersionIndependentResourceTypesAll>(node.Text)));
				break;

			case "multipleOr":
				current.MultipleOrElement = new FhirBoolean(_converter._primitive.GetBoolOpt(node));
				break;

			case "_multipleOr":
				_converter._element.Process(node, current.MultipleOrElement);
				break;

			case "multipleAnd":
				current.MultipleAndElement = new FhirBoolean(_converter._primitive.GetBoolOpt(node));
				break;

			case "_multipleAnd":
				_converter._element.Process(node, current.MultipleAndElement);
				break;

			case "comparator":
				current.ComparatorElement.Add(new Code<SearchComparator>(Hl7.Fhir.Utility.EnumUtility.ParseLiteral<SearchComparator>(node.Text)));
				break;

			case "modifier":
				current.ModifierElement.Add(new Code<SearchModifierCode>(Hl7.Fhir.Utility.EnumUtility.ParseLiteral<SearchModifierCode>(node.Text)));
				break;

			case "chain":
				current.ChainElement.Add(new FhirString(node.Text));
				break;

			case "component":
				current.Component.Add(Extract43SearchParameterComponentComponent(node));
				break;

			// process inherited elements
			default:
				_converter._domainResource.Process(node, current);
				break;

		}
	}

	private SearchParameter.ComponentComponent Extract43SearchParameterComponentComponent(ISourceNode parent)
	{
		SearchParameter.ComponentComponent current = new();

        foreach (ISourceNode node in parent.Children())
        {
            switch (node.Name)
            {
                case "definition":
                    current.DefinitionElement = new Canonical(node.Text);
                    break;

                case "_definition":
                    _converter._element.Process(node, current.DefinitionElement);
                    break;

                case "expression":
                    current.ExpressionElement = new FhirString(node.Text);
                    break;

                case "_expression":
                    _converter._element.Process(node, current.ExpressionElement);
                    break;

                // process inherited elements
                default:
                    _converter._backboneElement.Process(node, current);
                    break;
            }
		}

		return current;
	}
}
