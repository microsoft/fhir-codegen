// <copyright file="ConceptMap.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_43_50;

public class ConceptMap_43_50 : ICrossVersionProcessor<ConceptMap>, ICrossVersionExtractor<ConceptMap>
{
	private Converter_43_50 _converter;
	internal ConceptMap_43_50(Converter_43_50 converter)
	{
		_converter = converter;
	}

	public ConceptMap Extract(ISourceNode node)
	{
		ConceptMap v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, ConceptMap current)
	{
		switch (node.Name)
		{
			case "url":
				current.UrlElement = new FhirUri(node.Text);
				break;

			case "_url":
				_converter._element.Process(node, current.UrlElement);
				break;

			case "identifier":
				current.Identifier.Add(_converter._identifier.Extract(node));
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

			case "purpose":
				current.PurposeElement = new Markdown(node.Text);
				break;

			case "_purpose":
				_converter._element.Process(node, current.PurposeElement);
				break;

			case "copyright":
				current.CopyrightElement = new Markdown(node.Text);
				break;

			case "_copyright":
				_converter._element.Process(node, current.CopyrightElement);
				break;

			case "sourceUri":
				current.SourceScope = new FhirUri(node.Text);
				break;

			case "sourceCanonical":
				current.SourceScope = new Canonical(node.Text);
				break;

			case "targetUri":
				current.TargetScope = new FhirUri(node.Text);
				break;

			case "targetCanonical":
				current.TargetScope = new Canonical(node.Text);
				break;

			case "group":
				current.Group.Add(Extract43ConceptMapGroupComponent(node));
				break;

			// process inherited elements
			default:
				_converter._domainResource.Process(node, current);
				break;

		}
	}

	private ConceptMap.GroupComponent Extract43ConceptMapGroupComponent(ISourceNode parent)
	{
		ConceptMap.GroupComponent current = new();

        foreach (ISourceNode node in parent.Children())
        {
            switch (node.Name)
            {
                case "source":
                    current.SourceElement = new Canonical(node.Text);
                    break;

                case "_source":
                    _converter._element.Process(node, current.SourceElement);
                    break;

                case "sourceVersion":
                    // element ConceptMap.group.sourceVersion has been removed in the target spec
                    break;

                case "target":
                    current.TargetElement = new Canonical(node.Text);
                    break;

                case "_target":
                    _converter._element.Process(node, current.TargetElement);
                    break;

                case "targetVersion":
                    // element ConceptMap.group.targetVersion has been removed in the target spec
                    break;

                case "element":
                    current.Element.Add(Extract43ConceptMapSourceElementComponent(node));
                    break;

                case "unmapped":
                    current.Unmapped = Extract43ConceptMapUnmappedComponent(node);
                    break;

                // process inherited elements
                default:
                    _converter._backboneElement.Process(node, current);
                    break;
            }
		}

		return current;
	}

	private ConceptMap.SourceElementComponent Extract43ConceptMapSourceElementComponent(ISourceNode parent)
	{
		ConceptMap.SourceElementComponent current = new();

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

                case "display":
                    current.DisplayElement = new FhirString(node.Text);
                    break;

                case "_display":
                    _converter._element.Process(node, current.DisplayElement);
                    break;

                case "target":
                    current.Target.Add(Extract43ConceptMapTargetElementComponent(node));
                    break;

                // process inherited elements
                default:
                    _converter._backboneElement.Process(node, current);
                    break;
            }
		}

		return current;
	}

	private ConceptMap.TargetElementComponent Extract43ConceptMapTargetElementComponent(ISourceNode parent)
	{
		ConceptMap.TargetElementComponent current = new();

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

                case "display":
                    current.DisplayElement = new FhirString(node.Text);
                    break;

                case "_display":
                    _converter._element.Process(node, current.DisplayElement);
                    break;

                case "equivalence":
                    // element ConceptMap.group.element.target.equivalence has been removed in the target spec
                    break;

                case "comment":
                    current.CommentElement = new FhirString(node.Text);
                    break;

                case "_comment":
                    _converter._element.Process(node, current.CommentElement);
                    break;

                case "dependsOn":
                    current.DependsOn.Add(Extract43ConceptMapOtherElementComponent(node));
                    break;

                case "product":
                    current.Product.Add(Extract43ConceptMapOtherElementComponent(node));
                    break;

                // process inherited elements
                default:
                    _converter._backboneElement.Process(node, current);
                    break;
            }
		}

		return current;
	}

	private ConceptMap.OtherElementComponent Extract43ConceptMapOtherElementComponent(ISourceNode parent)
	{
		ConceptMap.OtherElementComponent current = new();

        foreach (ISourceNode node in parent.Children())
        {
            switch (node.Name)
            {
                case "property":
                    // element ConceptMap.group.element.target.dependsOn.property has been removed in the target spec
                    break;

                case "system":
                    // element ConceptMap.group.element.target.dependsOn.system has been removed in the target spec
                    break;

                case "value":
                    // element ConceptMap.group.element.target.dependsOn.value has been removed in the target spec
                    break;

                case "display":
                    // element ConceptMap.group.element.target.dependsOn.display has been removed in the target spec
                    break;

                // process inherited elements
                default:
                    _converter._backboneElement.Process(node, current);
                    break;
            }
		}

		return current;
	}

	private ConceptMap.UnmappedComponent Extract43ConceptMapUnmappedComponent(ISourceNode parent)
	{
		ConceptMap.UnmappedComponent current = new();

        foreach (ISourceNode node in parent.Children())
        {
            switch (node.Name)
            {
                case "mode":
                    current.Mode = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<ConceptMap.ConceptMapGroupUnmappedMode>(node.Text);
                    break;

                case "_mode":
                    _converter._element.Process(node, current.ModeElement);
                    break;

                case "code":
                    current.CodeElement = new Code(node.Text);
                    break;

                case "_code":
                    _converter._element.Process(node, current.CodeElement);
                    break;

                case "display":
                    current.DisplayElement = new FhirString(node.Text);
                    break;

                case "_display":
                    _converter._element.Process(node, current.DisplayElement);
                    break;

                case "url":
                    // element ConceptMap.group.unmapped.url has been removed in the target spec
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
