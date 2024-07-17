// <copyright file="ImplementationGuide.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_43_50;

public class ImplementationGuide_43_50 : ICrossVersionProcessor<ImplementationGuide>, ICrossVersionExtractor<ImplementationGuide>
{
	private Converter_43_50 _converter;
	internal ImplementationGuide_43_50(Converter_43_50 converter)
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

			case "packageId":
				current.PackageIdElement = new Id(node.Text);
				break;

			case "_packageId":
				_converter._element.Process(node, current.PackageIdElement);
				break;

			case "license":
				current.License = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<ImplementationGuide.SPDXLicense>(node.Text);
				break;

			case "_license":
				_converter._element.Process(node, current.LicenseElement);
				break;

			case "fhirVersion":
				current.FhirVersionElement.Add(new Code<FHIRVersion>(Hl7.Fhir.Utility.EnumUtility.ParseLiteral<FHIRVersion>(node.Text)));
				break;

			case "dependsOn":
				current.DependsOn.Add(Extract43ImplementationGuideDependsOnComponent(node));
				break;

			case "global":
				current.Global.Add(Extract43ImplementationGuideGlobalComponent(node));
				break;

			case "definition":
				current.Definition = Extract43ImplementationGuideDefinitionComponent(node);
				break;

			case "manifest":
				current.Manifest = Extract43ImplementationGuideManifestComponent(node);
				break;

			// process inherited elements
			default:
				_converter._domainResource.Process(node, current);
				break;

		}
	}

	private ImplementationGuide.DependsOnComponent Extract43ImplementationGuideDependsOnComponent(ISourceNode parent)
	{
		ImplementationGuide.DependsOnComponent current = new();

        foreach (ISourceNode node in parent.Children())
        {
            switch (node.Name)
            {
                case "uri":
                    current.UriElement = new Canonical(node.Text);
                    break;

                case "_uri":
                    _converter._element.Process(node, current.UriElement);
                    break;

                case "packageId":
                    current.PackageIdElement = new Id(node.Text);
                    break;

                case "_packageId":
                    _converter._element.Process(node, current.PackageIdElement);
                    break;

                case "version":
                    current.VersionElement = new FhirString(node.Text);
                    break;

                case "_version":
                    _converter._element.Process(node, current.VersionElement);
                    break;

                // process inherited elements
                default:
                    _converter._backboneElement.Process(node, current);
                    break;
            }
		}

		return current;
	}

	private ImplementationGuide.GlobalComponent Extract43ImplementationGuideGlobalComponent(ISourceNode parent)
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

                case "_profile":
                    _converter._element.Process(node, current.ProfileElement);
                    break;

                // process inherited elements
                default:
                    _converter._backboneElement.Process(node, current);
                    break;
            }
		}

		return current;
	}

	private ImplementationGuide.DefinitionComponent Extract43ImplementationGuideDefinitionComponent(ISourceNode parent)
	{
		ImplementationGuide.DefinitionComponent current = new();

        foreach (ISourceNode node in parent.Children())
        {
            switch (node.Name)
            {
                case "grouping":
                    current.Grouping.Add(Extract43ImplementationGuideGroupingComponent(node));
                    break;

                case "resource":
                    current.Resource.Add(Extract43ImplementationGuideResourceComponent(node));
                    break;

                case "page":
                    current.Page = Extract43ImplementationGuidePageComponent(node);
                    break;

                case "parameter":
                    current.Parameter.Add(Extract43ImplementationGuideParameterComponent(node));
                    break;

                case "template":
                    current.Template.Add(Extract43ImplementationGuideTemplateComponent(node));
                    break;

                // process inherited elements
                default:
                    _converter._backboneElement.Process(node, current);
                    break;
            }
		}

		return current;
	}

	private ImplementationGuide.GroupingComponent Extract43ImplementationGuideGroupingComponent(ISourceNode parent)
	{
		ImplementationGuide.GroupingComponent current = new();

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
                    current.DescriptionElement = new Markdown(node.Text);
                    break;

                case "_description":
                    _converter._element.Process(node, current.DescriptionElement);
                    break;

                // process inherited elements
                default:
                    _converter._backboneElement.Process(node, current);
                    break;
            }
		}

		return current;
	}

	private ImplementationGuide.ResourceComponent Extract43ImplementationGuideResourceComponent(ISourceNode parent)
	{
		ImplementationGuide.ResourceComponent current = new();

        foreach (ISourceNode node in parent.Children())
        {
            switch (node.Name)
            {
                case "reference":
                    current.Reference = new ResourceReference(node.Text);
                    break;

                case "fhirVersion":
                    current.FhirVersionElement.Add(new Code<FHIRVersion>(Hl7.Fhir.Utility.EnumUtility.ParseLiteral<FHIRVersion>(node.Text)));
                    break;

                case "name":
                    current.NameElement = new FhirString(node.Text);
                    break;

                case "_name":
                    _converter._element.Process(node, current.NameElement);
                    break;

                case "description":
                    current.DescriptionElement = new Markdown(node.Text);
                    break;

                case "_description":
                    _converter._element.Process(node, current.DescriptionElement);
                    break;

                case "exampleBoolean":
                    // element ImplementationGuide.definition.resource.example[x] has been removed in the target spec
                    break;

                case "exampleCanonical":
                    // element ImplementationGuide.definition.resource.example[x] has been removed in the target spec
                    break;

                case "groupingId":
                    current.GroupingIdElement = new Id(node.Text);
                    break;

                case "_groupingId":
                    _converter._element.Process(node, current.GroupingIdElement);
                    break;

                // process inherited elements
                default:
                    _converter._backboneElement.Process(node, current);
                    break;
            }
		}

		return current;
	}

	private ImplementationGuide.PageComponent Extract43ImplementationGuidePageComponent(ISourceNode parent)
	{
		ImplementationGuide.PageComponent current = new();

        foreach (ISourceNode node in parent.Children())
        {
            switch (node.Name)
            {
                case "nameUrl":
                    // element ImplementationGuide.definition.page.name[x] has been removed in the target spec
                    break;

                case "nameReference":
                    // element ImplementationGuide.definition.page.name[x] has been removed in the target spec
                    break;

                case "title":
                    current.TitleElement = new FhirString(node.Text);
                    break;

                case "_title":
                    _converter._element.Process(node, current.TitleElement);
                    break;

                case "generation":
                    current.Generation = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<ImplementationGuide.GuidePageGeneration>(node.Text);
                    break;

                case "_generation":
                    _converter._element.Process(node, current.GenerationElement);
                    break;

                case "page":
                    current.Page.Add(Extract43ImplementationGuidePageComponent(node));
                    break;

                // process inherited elements
                default:
                    _converter._backboneElement.Process(node, current);
                    break;
            }
		}

		return current;
	}

	private ImplementationGuide.ParameterComponent Extract43ImplementationGuideParameterComponent(ISourceNode parent)
	{
		ImplementationGuide.ParameterComponent current = new();

        foreach (ISourceNode node in parent.Children())
        {
            switch (node.Name)
            {
                case "code":
                    current.Code = new Coding("http://hl7.org/fhir/guide-parameter-code", node.Text);
                    break;

                case "_code":
                    _converter._element.Process(node, current.Code);
                    break;

                case "value":
                    current.ValueElement = new FhirString(node.Text);
                    break;

                case "_value":
                    _converter._element.Process(node, current.ValueElement);
                    break;

                // process inherited elements
                default:
                    _converter._backboneElement.Process(node, current);
                    break;
            }
		}

		return current;
	}

	private ImplementationGuide.TemplateComponent Extract43ImplementationGuideTemplateComponent(ISourceNode parent)
	{
		ImplementationGuide.TemplateComponent current = new();

        foreach (ISourceNode node in parent.Children())
        {
            switch (node.Name)
            {
                case "code":
                    current.CodeElement = new Code(node.Text);
                    break;

                case "_code":
                    _converter._element.Process(node, current.CodeElement);
                    break;

                case "source":
                    current.SourceElement = new FhirString(node.Text);
                    break;

                case "_source":
                    _converter._element.Process(node, current.SourceElement);
                    break;

                case "scope":
                    current.ScopeElement = new FhirString(node.Text);
                    break;

                case "_scope":
                    _converter._element.Process(node, current.ScopeElement);
                    break;

                // process inherited elements
                default:
                    _converter._backboneElement.Process(node, current);
                    break;
            }
		}

		return current;
	}

	private ImplementationGuide.ManifestComponent Extract43ImplementationGuideManifestComponent(ISourceNode parent)
	{
		ImplementationGuide.ManifestComponent current = new();

        foreach (ISourceNode node in parent.Children())
        {
            switch (node.Name)
            {
                case "rendering":
                    current.RenderingElement = new FhirUrl(node.Text);
                    break;

                case "_rendering":
                    _converter._element.Process(node, current.RenderingElement);
                    break;

                case "resource":
                    current.Resource.Add(Extract43ImplementationGuideManifestResourceComponent(node));
                    break;

                case "page":
                    current.Page.Add(Extract43ImplementationGuideManifestPageComponent(node));
                    break;

                case "image":
                    current.ImageElement.Add(new FhirString(node.Text));
                    break;

                case "other":
                    current.OtherElement.Add(new FhirString(node.Text));
                    break;

                // process inherited elements
                default:
                    _converter._backboneElement.Process(node, current);
                    break;
            }
		}

		return current;
	}

	private ImplementationGuide.ManifestResourceComponent Extract43ImplementationGuideManifestResourceComponent(ISourceNode parent)
	{
		ImplementationGuide.ManifestResourceComponent current = new();

        foreach (ISourceNode node in parent.Children())
        {
            switch (node.Name)
            {
                case "reference":
                    current.Reference = new ResourceReference(node.Text);
                    break;

                case "exampleBoolean":
                    // element ImplementationGuide.manifest.resource.example[x] has been removed in the target spec
                    break;

                case "exampleCanonical":
                    // element ImplementationGuide.manifest.resource.example[x] has been removed in the target spec
                    break;

                case "relativePath":
                    current.RelativePathElement = new FhirUrl(node.Text);
                    break;

                case "_relativePath":
                    _converter._element.Process(node, current.RelativePathElement);
                    break;

                // process inherited elements
                default:
                    _converter._backboneElement.Process(node, current);
                    break;
            }
		}

		return current;
	}

	private ImplementationGuide.ManifestPageComponent Extract43ImplementationGuideManifestPageComponent(ISourceNode parent)
	{
		ImplementationGuide.ManifestPageComponent current = new();

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

                case "title":
                    current.TitleElement = new FhirString(node.Text);
                    break;

                case "_title":
                    _converter._element.Process(node, current.TitleElement);
                    break;

                case "anchor":
                    current.AnchorElement.Add(new FhirString(node.Text));
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
