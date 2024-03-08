// <copyright file="ImplementationGuide.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_20_50;

public class ImplementationGuide_20_50 : ICrossVersionProcessor<ImplementationGuide>, ICrossVersionExtractor<ImplementationGuide>
{
	private Converter_20_50 _converter;
	internal ImplementationGuide_20_50(Converter_20_50 converter)
	{
		_converter = converter;
	}

	public ImplementationGuide Extract(ISourceNode node)
	{
		ImplementationGuide v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, ImplementationGuide current)
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

			case "useContext":
				current.UseContext.Add(_converter._usageContext.Extract(node));
				break;

			case "jurisdiction":
				current.Jurisdiction.Add(_converter._codeableConcept.Extract(node));
				break;

			case "copyright":
				current.CopyrightElement = new Markdown(node.Text);
				break;

			case "_copyright":
				_converter._element.Process(node, current.CopyrightElement);
				break;

			case "fhirVersion":
                current.FhirVersionElement.Add(new Code<FHIRVersion>(Hl7.Fhir.Utility.EnumUtility.ParseLiteral<FHIRVersion>(node.Text)));
                break;

			case "dependency":
				// element ImplementationGuide.dependency has been removed in the target spec
				break;

			case "package":
				// element ImplementationGuide.package has been removed in the target spec
				break;

			case "global":
				current.Global.Add(Extract20ImplementationGuideGlobalComponent(node));
				break;

			case "binary":
				// element ImplementationGuide.binary has been removed in the target spec
				break;

			case "page":
				// element ImplementationGuide.page has been removed in the target spec
				break;

			// process inherited elements
			default:
				_converter._domainResource.Process(node, current);
				break;

		}
	}
	/* skipping - element has been removed

	private ImplementationGuide.DependencyComponent Extract20ImplementationGuideDependencyComponent(ISourceNode parent)
	{
		ImplementationGuide.DependencyComponent current = new();

		foreach (ISourceNode node in parent.Children())
		{
			switch (node.Name)
			{
				case "type":
					current.Type = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<ImplementationGuide.GuideDependencyType>(node.Text);
					break;

				case "_type":
					_converter._element.Process(node, current.TypeElement);
					break;

				case "uri":
					current.UriElement = new FhirUri(node.Text);
					break;

				case "_uri":
					_converter._element.Process(node, current.UriElement);
					break;

				// process inherited elements
				default:
					_converter._backboneElement.Process(node, current);
					break;

			}
		}

		return current;
	}
	*/
	/* skipping - element has been removed

	private ImplementationGuide.PackageComponent Extract20ImplementationGuidePackageComponent(ISourceNode parent)
	{
		ImplementationGuide.PackageComponent current = new();

		foreach (ISourceNode node in parent.Children())
		{
			switch (node.Name)
			{
				case "name":
					current.NameElement = new FhirString(node.Text);
					break;

				case "_name":
					_converter._element.Process(node, current.NameElement);
					break;

				case "description":
					current.DescriptionElement = new FhirString(node.Text);
					break;

				case "_description":
					_converter._element.Process(node, current.DescriptionElement);
					break;

				case "resource":
					current.Resource.Add(Extract20ImplementationGuideResourceComponent(node));
					break;

				// process inherited elements
				default:
					_converter._backboneElement.Process(node, current);
					break;

			}
		}

		return current;
	}

	private ImplementationGuide.ResourceComponent Extract20ImplementationGuideResourceComponent(ISourceNode parent)
	{
		ImplementationGuide.ResourceComponent current = new();

		foreach (ISourceNode node in parent.Children())
		{
			switch (node.Name)
			{
				case "example":
					current.ExampleElement = new FhirBoolean(_converter._primitive.GetBool(node));
					break;

				case "_example":
					_converter._element.Process(node, current.ExampleElement);
					break;

				case "name":
					current.NameElement = new FhirString(node.Text);
					break;

				case "_name":
					_converter._element.Process(node, current.NameElement);
					break;

				case "description":
					current.DescriptionElement = new FhirString(node.Text);
					break;

				case "_description":
					_converter._element.Process(node, current.DescriptionElement);
					break;

				case "acronym":
					current.AcronymElement = new FhirString(node.Text);
					break;

				case "_acronym":
					_converter._element.Process(node, current.AcronymElement);
					break;

				case "sourceUri":
					current.Source = new FhirUri(node.Text);
					break;

				case "_sourceUri":
					_converter._element.Process(node, current.Source);
					break;

				case "sourceReference":
					current.Source = new ResourceReference(node.Text);
					break;

				case "exampleFor":
					current.ExampleForElement = new ResourceReference(node.Text);
					break;

				// process inherited elements
				default:
					_converter._backboneElement.Process(node, current);
					break;

			}
		}

		return current;
	}
	*/

	private ImplementationGuide.GlobalComponent Extract20ImplementationGuideGlobalComponent(ISourceNode parent)
	{
		ImplementationGuide.GlobalComponent current = new();

		foreach (ISourceNode node in parent.Children())
		{
			switch (node.Name)
			{
				case "type":
					current.TypeElement = new Code<ResourceType>(Hl7.Fhir.Utility.EnumUtility.ParseLiteral<ResourceType>(node.Text));
					break;

				case "_type":
					_converter._element.Process(node, current.TypeElement);
					break;

				case "profile":
					current.ProfileElement = new Canonical(node.Text);
					break;

				// process inherited elements
				default:
					_converter._backboneElement.Process(node, current);
					break;

			}
		}

		return current;
	}
	/* skipping - element has been removed

	private ImplementationGuide.PageComponent Extract20ImplementationGuidePageComponent(ISourceNode parent)
	{
		ImplementationGuide.PageComponent current = new();

		foreach (ISourceNode node in parent.Children())
		{
			switch (node.Name)
			{
				case "source":
					current.SourceElement = new FhirUri(node.Text);
					break;

				case "_source":
					_converter._element.Process(node, current.SourceElement);
					break;

				case "title":
					current.TitleElement = new FhirString(node.Text);
					break;

				case "_title":
					_converter._element.Process(node, current.TitleElement);
					break;

				case "kind":
					current.Kind = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<ImplementationGuide.GuidePageKind>(node.Text);
					break;

				case "_kind":
					_converter._element.Process(node, current.KindElement);
					break;

				case "type":
					current.TypeElement.Add(new Code<VersionIndependentResourceTypesAll>(Hl7.Fhir.Utility.EnumUtility.ParseLiteral<VersionIndependentResourceTypesAll>(node.Text)));
					break;

				case "package":
					current.PackageElement.Add(new FhirString(node.Text));
					break;

				case "format":
					current.FormatElement = new Code(node.Text);
					break;

				case "_format":
					_converter._element.Process(node, current.FormatElement);
					break;

				case "page":
					current.Page.Add(Extract20ImplementationGuidePageComponent(node));
					break;

				// process inherited elements
				default:
					_converter._backboneElement.Process(node, current);
					break;

			}
		}

		return current;
	}
	*/
}
