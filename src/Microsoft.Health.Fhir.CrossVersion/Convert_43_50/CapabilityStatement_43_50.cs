// <copyright file="CapabilityStatement.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_43_50;

public class CapabilityStatement_43_50 : ICrossVersionProcessor<CapabilityStatement>, ICrossVersionExtractor<CapabilityStatement>
{
	private Converter_43_50 _converter;
	internal CapabilityStatement_43_50(Converter_43_50 converter)
	{
		_converter = converter;
	}

	public CapabilityStatement Extract(ISourceNode node)
	{
		CapabilityStatement v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, CapabilityStatement current)
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

			case "kind":
				current.KindElement = new Code<CapabilityStatementKind>(Hl7.Fhir.Utility.EnumUtility.ParseLiteral<CapabilityStatementKind>(node.Text));
				break;

			case "_kind":
				_converter._element.Process(node, current.KindElement);
				break;

			case "instantiates":
				current.InstantiatesElement.Add(new Canonical(node.Text));
				break;

			case "imports":
				current.ImportsElement.Add(new Canonical(node.Text));
				break;

			case "software":
				current.Software = Extract43CapabilityStatementSoftwareComponent(node);
				break;

			case "implementation":
				current.Implementation = Extract43CapabilityStatementImplementationComponent(node);
				break;

			case "fhirVersion":
				current.FhirVersionElement = new Code<FHIRVersion>(Hl7.Fhir.Utility.EnumUtility.ParseLiteral<FHIRVersion>(node.Text));
				break;

			case "_fhirVersion":
				_converter._element.Process(node, current.FhirVersionElement);
				break;

			case "format":
				current.FormatElement.Add(new Code(node.Text));
				break;

			case "patchFormat":
				current.PatchFormatElement.Add(new Code(node.Text));
				break;

			case "implementationGuide":
				current.ImplementationGuideElement.Add(new Canonical(node.Text));
				break;

			case "rest":
				current.Rest.Add(Extract43CapabilityStatementRestComponent(node));
				break;

			case "messaging":
				current.Messaging.Add(Extract43CapabilityStatementMessagingComponent(node));
				break;

			case "document":
				current.Document.Add(Extract43CapabilityStatementDocumentComponent(node));
				break;

			// process inherited elements
			default:
				_converter._domainResource.Process(node, current);
				break;

		}
	}

	private CapabilityStatement.SoftwareComponent Extract43CapabilityStatementSoftwareComponent(ISourceNode parent)
	{
		CapabilityStatement.SoftwareComponent current = new();

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

                case "version":
                    current.VersionElement = new FhirString(node.Text);
                    break;

                case "_version":
                    _converter._element.Process(node, current.VersionElement);
                    break;

                case "releaseDate":
                    current.ReleaseDateElement = new FhirDateTime(node.Text);
                    break;

                case "_releaseDate":
                    _converter._element.Process(node, current.ReleaseDateElement);
                    break;

                // process inherited elements
                default:
                    _converter._backboneElement.Process(node, current);
                    break;

            }
        }

		return current;
	}

	private CapabilityStatement.ImplementationComponent Extract43CapabilityStatementImplementationComponent(ISourceNode parent)
	{
		CapabilityStatement.ImplementationComponent current = new();

        foreach (ISourceNode node in parent.Children())
        {
            switch (node.Name)
            {
                case "description":
                    current.DescriptionElement = new Markdown(node.Text);
                    break;

                case "_description":
                    _converter._element.Process(node, current.DescriptionElement);
                    break;

                case "url":
                    current.UrlElement = new FhirUrl(node.Text);
                    break;

                case "_url":
                    _converter._element.Process(node, current.UrlElement);
                    break;

                case "custodian":
                    current.Custodian = new ResourceReference(node.Text);
                    break;

                // process inherited elements
                default:
                    _converter._backboneElement.Process(node, current);
                    break;

            }
        }

		return current;
	}

	private CapabilityStatement.RestComponent Extract43CapabilityStatementRestComponent(ISourceNode parent)
	{
		CapabilityStatement.RestComponent current = new();

        foreach (ISourceNode node in parent.Children())
        {
            switch (node.Name)
            {
                case "mode":
                    current.Mode = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<CapabilityStatement.RestfulCapabilityMode>(node.Text);
                    break;

                case "_mode":
                    _converter._element.Process(node, current.ModeElement);
                    break;

                case "documentation":
                    current.DocumentationElement = new Markdown(node.Text);
                    break;

                case "_documentation":
                    _converter._element.Process(node, current.DocumentationElement);
                    break;

                case "security":
                    current.Security = Extract43CapabilityStatementSecurityComponent(node);
                    break;

                case "resource":
                    current.Resource.Add(Extract43CapabilityStatementResourceComponent(node));
                    break;

                case "interaction":
                    current.Interaction.Add(Extract43CapabilityStatementSystemInteractionComponent(node));
                    break;

                case "searchParam":
                    current.SearchParam.Add(Extract43CapabilityStatementSearchParamComponent(node));
                    break;

                case "operation":
                    current.Operation.Add(Extract43CapabilityStatementOperationComponent(node));
                    break;

                case "compartment":
                    current.CompartmentElement.Add(new Canonical(node.Text));
                    break;

                // process inherited elements
                default:
                    _converter._backboneElement.Process(node, current);
                    break;
            }
        }

		return current;
	}

	private CapabilityStatement.SecurityComponent Extract43CapabilityStatementSecurityComponent(ISourceNode parent)
	{
		CapabilityStatement.SecurityComponent current = new();

        foreach (ISourceNode node in parent.Children())
        {
            switch (node.Name)
            {
                case "cors":
                    current.CorsElement = new FhirBoolean(_converter._primitive.GetBoolOpt(node));
                    break;

                case "_cors":
                    _converter._element.Process(node, current.CorsElement);
                    break;

                case "service":
                    current.Service.Add(_converter._codeableConcept.Extract(node));
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

	private CapabilityStatement.ResourceComponent Extract43CapabilityStatementResourceComponent(ISourceNode parent)
	{
		CapabilityStatement.ResourceComponent current = new();

        foreach (ISourceNode node in parent.Children())
        {
            switch (node.Name)
            {
                case "type":
                    current.TypeElement = new Code(node.Text);
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

                case "supportedProfile":
                    current.SupportedProfileElement.Add(new Canonical(node.Text));
                    break;

                case "documentation":
                    current.DocumentationElement = new Markdown(node.Text);
                    break;

                case "_documentation":
                    _converter._element.Process(node, current.DocumentationElement);
                    break;

                case "interaction":
                    current.Interaction.Add(Extract43CapabilityStatementResourceInteractionComponent(node));
                    break;

                case "versioning":
                    current.Versioning = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<CapabilityStatement.ResourceVersionPolicy>(node.Text);
                    break;

                case "_versioning":
                    _converter._element.Process(node, current.VersioningElement);
                    break;

                case "readHistory":
                    current.ReadHistoryElement = new FhirBoolean(_converter._primitive.GetBoolOpt(node));
                    break;

                case "_readHistory":
                    _converter._element.Process(node, current.ReadHistoryElement);
                    break;

                case "updateCreate":
                    current.UpdateCreateElement = new FhirBoolean(_converter._primitive.GetBoolOpt(node));
                    break;

                case "_updateCreate":
                    _converter._element.Process(node, current.UpdateCreateElement);
                    break;

                case "conditionalCreate":
                    current.ConditionalCreateElement = new FhirBoolean(_converter._primitive.GetBoolOpt(node));
                    break;

                case "_conditionalCreate":
                    _converter._element.Process(node, current.ConditionalCreateElement);
                    break;

                case "conditionalRead":
                    current.ConditionalRead = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<CapabilityStatement.ConditionalReadStatus>(node.Text);
                    break;

                case "_conditionalRead":
                    _converter._element.Process(node, current.ConditionalReadElement);
                    break;

                case "conditionalUpdate":
                    current.ConditionalUpdateElement = new FhirBoolean(_converter._primitive.GetBoolOpt(node));
                    break;

                case "_conditionalUpdate":
                    _converter._element.Process(node, current.ConditionalUpdateElement);
                    break;

                case "conditionalDelete":
                    current.ConditionalDelete = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<CapabilityStatement.ConditionalDeleteStatus>(node.Text);
                    break;

                case "_conditionalDelete":
                    _converter._element.Process(node, current.ConditionalDeleteElement);
                    break;

                case "referencePolicy":
                    current.ReferencePolicyElement.Add(new Code<CapabilityStatement.ReferenceHandlingPolicy>(Hl7.Fhir.Utility.EnumUtility.ParseLiteral<CapabilityStatement.ReferenceHandlingPolicy>(node.Text)));
                    break;

                case "searchInclude":
                    current.SearchIncludeElement.Add(new FhirString(node.Text));
                    break;

                case "searchRevInclude":
                    current.SearchRevIncludeElement.Add(new FhirString(node.Text));
                    break;

                case "searchParam":
                    current.SearchParam.Add(Extract43CapabilityStatementSearchParamComponent(node));
                    break;

                case "operation":
                    current.Operation.Add(Extract43CapabilityStatementOperationComponent(node));
                    break;

                // process inherited elements
                default:
                    _converter._backboneElement.Process(node, current);
                    break;
            }
		}

		return current;
	}

	private CapabilityStatement.ResourceInteractionComponent Extract43CapabilityStatementResourceInteractionComponent(ISourceNode parent)
	{
		CapabilityStatement.ResourceInteractionComponent current = new();

        foreach (ISourceNode node in parent.Children())
        {
            switch (node.Name)
            {
                case "code":
                    current.Code = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<CapabilityStatement.TypeRestfulInteraction>(node.Text);
                    break;

                case "_code":
                    _converter._element.Process(node, current.CodeElement);
                    break;

                case "documentation":
                    current.DocumentationElement = new Markdown(node.Text);
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

	private CapabilityStatement.SearchParamComponent Extract43CapabilityStatementSearchParamComponent(ISourceNode parent)
	{
		CapabilityStatement.SearchParamComponent current = new();

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

                case "definition":
                    current.DefinitionElement = new Canonical(node.Text);
                    break;

                case "_definition":
                    _converter._element.Process(node, current.DefinitionElement);
                    break;

                case "type":
                    current.TypeElement = new Code<SearchParamType>(Hl7.Fhir.Utility.EnumUtility.ParseLiteral<SearchParamType>(node.Text));
                    break;

                case "_type":
                    _converter._element.Process(node, current.TypeElement);
                    break;

                case "documentation":
                    current.DocumentationElement = new Markdown(node.Text);
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

	private CapabilityStatement.OperationComponent Extract43CapabilityStatementOperationComponent(ISourceNode parent)
	{
		CapabilityStatement.OperationComponent current = new();

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

                case "definition":
                    current.DefinitionElement = new Canonical(node.Text);
                    break;

                case "_definition":
                    _converter._element.Process(node, current.DefinitionElement);
                    break;

                case "documentation":
                    current.DocumentationElement = new Markdown(node.Text);
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

	private CapabilityStatement.SystemInteractionComponent Extract43CapabilityStatementSystemInteractionComponent(ISourceNode parent)
	{
		CapabilityStatement.SystemInteractionComponent current = new();

        foreach (ISourceNode node in parent.Children())
        {
            switch (node.Name)
            {
                case "code":
                    current.Code = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<CapabilityStatement.SystemRestfulInteraction>(node.Text);
                    break;

                case "_code":
                    _converter._element.Process(node, current.CodeElement);
                    break;

                case "documentation":
                    current.DocumentationElement = new Markdown(node.Text);
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

	private CapabilityStatement.MessagingComponent Extract43CapabilityStatementMessagingComponent(ISourceNode parent)
	{
		CapabilityStatement.MessagingComponent current = new();

        foreach (ISourceNode node in parent.Children())
        {
            switch (node.Name)
            {
                case "endpoint":
                    current.Endpoint.Add(Extract43CapabilityStatementEndpointComponent(node));
                    break;

                case "reliableCache":
                    current.ReliableCacheElement = new UnsignedInt(_converter._primitive.GetIntOpt(node));
                    break;

                case "_reliableCache":
                    _converter._element.Process(node, current.ReliableCacheElement);
                    break;

                case "documentation":
                    current.DocumentationElement = new Markdown(node.Text);
                    break;

                case "_documentation":
                    _converter._element.Process(node, current.DocumentationElement);
                    break;

                case "supportedMessage":
                    current.SupportedMessage.Add(Extract43CapabilityStatementSupportedMessageComponent(node));
                    break;

                // process inherited elements
                default:
                    _converter._backboneElement.Process(node, current);
                    break;
            }
		}

		return current;
	}

	private CapabilityStatement.EndpointComponent Extract43CapabilityStatementEndpointComponent(ISourceNode parent)
	{
		CapabilityStatement.EndpointComponent current = new();

        foreach (ISourceNode node in parent.Children())
        {
            switch (node.Name)
            {
                case "protocol":
                    current.Protocol = _converter._coding.Extract(node);
                    break;

                case "address":
                    current.AddressElement = new FhirUrl(node.Text);
                    break;

                case "_address":
                    _converter._element.Process(node, current.AddressElement);
                    break;

                // process inherited elements
                default:
                    _converter._backboneElement.Process(node, current);
                    break;
            }
		}

		return current;
	}

	private CapabilityStatement.SupportedMessageComponent Extract43CapabilityStatementSupportedMessageComponent(ISourceNode parent)
	{
		CapabilityStatement.SupportedMessageComponent current = new();

        foreach (ISourceNode node in parent.Children())
        {
            switch (node.Name)
            {
                case "mode":
                    current.Mode = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<CapabilityStatement.EventCapabilityMode>(node.Text);
                    break;

                case "_mode":
                    _converter._element.Process(node, current.ModeElement);
                    break;

                case "definition":
                    current.DefinitionElement = new Canonical(node.Text);
                    break;

                case "_definition":
                    _converter._element.Process(node, current.DefinitionElement);
                    break;

                // process inherited elements
                default:
                    _converter._backboneElement.Process(node, current);
                    break;
            }
		}

		return current;
	}

	private CapabilityStatement.DocumentComponent Extract43CapabilityStatementDocumentComponent(ISourceNode parent)
	{
		CapabilityStatement.DocumentComponent current = new();

        foreach (ISourceNode node in parent.Children())
        {
            switch (node.Name)
            {
                case "mode":
                    current.Mode = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<CapabilityStatement.DocumentMode>(node.Text);
                    break;

                case "_mode":
                    _converter._element.Process(node, current.ModeElement);
                    break;

                case "documentation":
                    current.DocumentationElement = new Markdown(node.Text);
                    break;

                case "_documentation":
                    _converter._element.Process(node, current.DocumentationElement);
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
}
