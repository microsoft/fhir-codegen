// <copyright file="ValueSet.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_20_50;

public class ValueSet_20_50 : ICrossVersionProcessor<ValueSet>, ICrossVersionExtractor<ValueSet>
{
	private Converter_20_50 _converter;
	internal ValueSet_20_50(Converter_20_50 converter)
	{
		_converter = converter;
	}

	public ValueSet Extract(ISourceNode node)
	{
		ValueSet v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, ValueSet current)
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

			case "immutable":
				current.ImmutableElement = new FhirBoolean(_converter._primitive.GetBoolOpt(node));
				break;

			case "_immutable":
				_converter._element.Process(node, current.ImmutableElement);
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

			case "extensible":
				// element ValueSet.extensible has been removed in the target spec
				break;

			case "compose":
				current.Compose = Extract20ValueSetComposeComponent(node);
				break;

			case "expansion":
				current.Expansion = Extract20ValueSetExpansionComponent(node);
				break;

			// process inherited elements
			default:
				_converter._domainResource.Process(node, current);
				break;

		}
	}

	private ValueSet.ComposeComponent Extract20ValueSetComposeComponent(ISourceNode parent)
	{
		ValueSet.ComposeComponent current = new();

		foreach (ISourceNode node in parent.Children())
		{
			switch (node.Name)
			{
				case "lockedDate":
					current.LockedDateElement = new Date(node.Text);
					break;

				case "_lockedDate":
					_converter._element.Process(node, current.LockedDateElement);
					break;

				case "inactive":
					current.InactiveElement = new FhirBoolean(_converter._primitive.GetBoolOpt(node));
					break;

				case "_inactive":
					_converter._element.Process(node, current.InactiveElement);
					break;

				case "include":
					current.Include.Add(Extract20ValueSetConceptSetComponent(node));
					break;

				case "exclude":
					current.Exclude.Add(Extract20ValueSetConceptSetComponent(node));
					break;

				// process inherited elements
				default:
					_converter._backboneElement.Process(node, current);
					break;

			}
		}

		return current;
	}

	private ValueSet.ConceptSetComponent Extract20ValueSetConceptSetComponent(ISourceNode parent)
	{
		ValueSet.ConceptSetComponent current = new();

		foreach (ISourceNode node in parent.Children())
		{
			switch (node.Name)
			{
				case "system":
					current.SystemElement = new FhirUri(node.Text);
					break;

				case "_system":
					_converter._element.Process(node, current.SystemElement);
					break;

				case "version":
					current.VersionElement = new FhirString(node.Text);
					break;

				case "_version":
					_converter._element.Process(node, current.VersionElement);
					break;

				case "concept":
					current.Concept.Add(Extract20ValueSetConceptReferenceComponent(node));
					break;

				case "filter":
					current.Filter.Add(Extract20ValueSetFilterComponent(node));
					break;

				case "valueSet":
					current.ValueSetElement.Add(new Canonical(node.Text));
					break;

				// process inherited elements
				default:
					_converter._backboneElement.Process(node, current);
					break;

			}
		}

		return current;
	}

	private ValueSet.ConceptReferenceComponent Extract20ValueSetConceptReferenceComponent(ISourceNode parent)
	{
		ValueSet.ConceptReferenceComponent current = new();

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

				case "designation":
					current.Designation.Add(Extract20ValueSetDesignationComponent(node));
					break;

				// process inherited elements
				default:
					_converter._backboneElement.Process(node, current);
					break;

			}
		}

		return current;
	}

	private ValueSet.DesignationComponent Extract20ValueSetDesignationComponent(ISourceNode parent)
	{
		ValueSet.DesignationComponent current = new();

		foreach (ISourceNode node in parent.Children())
		{
			switch (node.Name)
			{
				case "language":
					current.LanguageElement = new Code(node.Text);
					break;

				case "_language":
					_converter._element.Process(node, current.LanguageElement);
					break;

				case "use":
					current.Use = _converter._coding.Extract(node);
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

	private ValueSet.FilterComponent Extract20ValueSetFilterComponent(ISourceNode parent)
	{
		ValueSet.FilterComponent current = new();

		foreach (ISourceNode node in parent.Children())
		{
			switch (node.Name)
			{
				case "property":
					current.PropertyElement = new Code(node.Text);
					break;

				case "_property":
					_converter._element.Process(node, current.PropertyElement);
					break;

				case "op":
					current.OpElement = new Code<FilterOperator>(Hl7.Fhir.Utility.EnumUtility.ParseLiteral<FilterOperator>(node.Text));
					break;

				case "_op":
					_converter._element.Process(node, current.OpElement);
					break;

				case "value":
					current.ValueElement = new FhirString(node.Text);
					break;

				// process inherited elements
				default:
					_converter._backboneElement.Process(node, current);
					break;

			}
		}

		return current;
	}

	private ValueSet.ExpansionComponent Extract20ValueSetExpansionComponent(ISourceNode parent)
	{
		ValueSet.ExpansionComponent current = new();

		foreach (ISourceNode node in parent.Children())
		{
			switch (node.Name)
			{
				case "identifier":
					current.IdentifierElement = new FhirUri(node.Text);
					break;

				case "_identifier":
					_converter._element.Process(node, current.IdentifierElement);
					break;

				case "timestamp":
					current.TimestampElement = new FhirDateTime(node.Text);
					break;

				case "_timestamp":
					_converter._element.Process(node, current.TimestampElement);
					break;

				case "total":
					current.TotalElement = new Integer(_converter._primitive.GetIntOpt(node));
					break;

				case "_total":
					_converter._element.Process(node, current.TotalElement);
					break;

				case "offset":
					current.OffsetElement = new Integer(_converter._primitive.GetIntOpt(node));
					break;

				case "_offset":
					_converter._element.Process(node, current.OffsetElement);
					break;

				case "parameter":
					current.Parameter.Add(Extract20ValueSetParameterComponent(node));
					break;

				case "contains":
					current.Contains.Add(Extract20ValueSetContainsComponent(node));
					break;

				// process inherited elements
				default:
					_converter._backboneElement.Process(node, current);
					break;

			}
		}

		return current;
	}

	private ValueSet.ParameterComponent Extract20ValueSetParameterComponent(ISourceNode parent)
	{
		ValueSet.ParameterComponent current = new();

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

				case "valueString":
					current.Value = new FhirString(node.Text);
					break;

				case "_valueString":
					_converter._element.Process(node, current.Value);
					break;

				case "valueBoolean":
					current.Value = new FhirBoolean(_converter._primitive.GetBoolOpt(node));
					break;

				case "_valueBoolean":
					_converter._element.Process(node, current.Value);
					break;

				case "valueInteger":
					current.Value = new Integer(_converter._primitive.GetIntOpt(node));
					break;

				case "_valueInteger":
					_converter._element.Process(node, current.Value);
					break;

				case "valueDecimal":
					current.Value = new FhirDecimal(_converter._primitive.GetDecimalOpt(node));
					break;

				case "_valueDecimal":
					_converter._element.Process(node, current.Value);
					break;

				case "valueUri":
					current.Value = new FhirUri(node.Text);
					break;

				case "_valueUri":
					_converter._element.Process(node, current.Value);
					break;

				case "valueCode":
					current.Value = new Code(node.Text);
					break;

				case "_valueCode":
					_converter._element.Process(node, current.Value);
					break;

				// process inherited elements
				default:
					_converter._backboneElement.Process(node, current);
					break;

			}
		}

		return current;
	}

	private ValueSet.ContainsComponent Extract20ValueSetContainsComponent(ISourceNode parent)
	{
		ValueSet.ContainsComponent current = new();

		foreach (ISourceNode node in parent.Children())
		{
			switch (node.Name)
			{
				case "system":
					current.SystemElement = new FhirUri(node.Text);
					break;

				case "_system":
					_converter._element.Process(node, current.SystemElement);
					break;

				case "abstract":
					current.AbstractElement = new FhirBoolean(_converter._primitive.GetBoolOpt(node));
					break;

				case "_abstract":
					_converter._element.Process(node, current.AbstractElement);
					break;

				case "inactive":
					current.InactiveElement = new FhirBoolean(_converter._primitive.GetBoolOpt(node));
					break;

				case "_inactive":
					_converter._element.Process(node, current.InactiveElement);
					break;

				case "version":
					current.VersionElement = new FhirString(node.Text);
					break;

				case "_version":
					_converter._element.Process(node, current.VersionElement);
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

				case "designation":
					current.Designation.Add(Extract20ValueSetDesignationComponent(node));
					break;

				case "contains":
					current.Contains.Add(Extract20ValueSetContainsComponent(node));
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
