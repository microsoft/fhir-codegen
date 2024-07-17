// <copyright file="CompartmentDefinition.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_20_50;

public class CompartmentDefinition_20_50 : ICrossVersionProcessor<CompartmentDefinition>, ICrossVersionExtractor<CompartmentDefinition>
{
	private Converter_20_50 _converter;
	internal CompartmentDefinition_20_50(Converter_20_50 converter)
	{
		_converter = converter;
	}

	public CompartmentDefinition Extract(ISourceNode node)
	{
		CompartmentDefinition v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, CompartmentDefinition current)
	{
		switch (node.Name)
		{
			case "url":
				current.UrlElement = new FhirUri(node.Text);
				break;

			case "_url":
				_converter._element.Process(node, current.UrlElement);
				break;

			case "name":
				current.NameElement = new FhirString(node.Text);
				break;

			case "_name":
				_converter._element.Process(node, current.NameElement);
				break;

			case "title":
				current.TitleElement = new FhirString(node.Text);
				break;

			case "_title":
				_converter._element.Process(node, current.TitleElement);
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

			case "purpose":
				current.PurposeElement = new Markdown(node.Text);
				break;

			case "_purpose":
				_converter._element.Process(node, current.PurposeElement);
				break;

			case "useContext":
				current.UseContext.Add(_converter._usageContext.Extract(node));
				break;

			case "jurisdiction":
				// element CompartmentDefinition.jurisdiction has been removed in the target spec
				break;

			case "code":
				current.CodeElement = new Code<CompartmentType>(Hl7.Fhir.Utility.EnumUtility.ParseLiteral<CompartmentType>(node.Text));
				break;

			case "_code":
				_converter._element.Process(node, current.CodeElement);
				break;

			case "search":
				current.SearchElement = new FhirBoolean(_converter._primitive.GetBool(node));
				break;

			case "_search":
				_converter._element.Process(node, current.SearchElement);
				break;

			case "resource":
				current.Resource.Add(Extract20CompartmentDefinitionResourceComponent(node));
				break;

			// process inherited elements
			default:
				_converter._domainResource.Process(node, current);
				break;

		}
	}

	private CompartmentDefinition.ResourceComponent Extract20CompartmentDefinitionResourceComponent(ISourceNode parent)
	{
		CompartmentDefinition.ResourceComponent current = new();

		foreach (ISourceNode node in parent.Children())
		{
			switch (node.Name)
			{
				case "code":
					current.CodeElement = new Code<ResourceType>(Hl7.Fhir.Utility.EnumUtility.ParseLiteral<ResourceType>(node.Text));
					break;

				case "_code":
					_converter._element.Process(node, current.CodeElement);
					break;

				case "param":
					current.ParamElement.Add(new FhirString(node.Text));
					break;

				case "documentation":
					current.DocumentationElement = new FhirString(node.Text);
					break;

				case "_documentation":
					_converter._element.Process(node, current.DocumentationElement);
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
