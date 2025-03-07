// <copyright file="CodeSystem.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_20_50;

public class CodeSystem_20_50 : ICrossVersionProcessor<CodeSystem>, ICrossVersionExtractor<CodeSystem>
{
    private const string _standardConceptPropertiesUrl = "http://hl7.org/fhir/concept-properties";
    private const string _propStatus = "status";                    // code
    private const string _propInactive = "inactive";                // boolean
    private const string _propEffectiveDate = "effectiveDate";      // date
    private const string _propDeprecationDate = "deprecationDate";  // date
    private const string _propRetirementDate = "retirementDate";    // date
    private const string _propNotSelectable = "notSelectable";      // boolean
    private const string _propParent = "parent";                    // code
    private const string _propChild = "child";                      // code
    private const string _propPartOf = "partOf";                    // code
    private const string _propSynonym = "synonym";                  // code
    private const string _propComment = "comment";                  // string
    private const string _propItemWeight = "itemWeight";            // decimal

    private static readonly (string, CodeSystem.PropertyType)[] _properties = [
        (_propStatus, CodeSystem.PropertyType.Code),
        (_propInactive, CodeSystem.PropertyType.Boolean),
        (_propEffectiveDate, CodeSystem.PropertyType.DateTime),
        (_propDeprecationDate, CodeSystem.PropertyType.DateTime),
        (_propRetirementDate, CodeSystem.PropertyType.DateTime),
        (_propNotSelectable, CodeSystem.PropertyType.Boolean),
        (_propParent, CodeSystem.PropertyType.Code),
        (_propChild, CodeSystem.PropertyType.Code),
        (_propPartOf, CodeSystem.PropertyType.Code),
        (_propSynonym, CodeSystem.PropertyType.Code),
        (_propComment, CodeSystem.PropertyType.String),
        (_propItemWeight, CodeSystem.PropertyType.Decimal),
    ];


    private Converter_20_50 _converter;
	internal CodeSystem_20_50(Converter_20_50 converter)
	{
		_converter = converter;
	}

	public CodeSystem Extract(ISourceNode node)
	{
		CodeSystem v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

        addStandardConceptProperties(v);

        return v;
	}

    private void addStandardConceptProperties(CodeSystem v)
    {
        ILookup<string, CodeSystem.PropertyComponent> csPropertiesByCode = v.Property.ToLookup(p => p.Code);
        ILookup<string, CodeSystem.PropertyComponent> csPropertiesByUri = v.Property.ToLookup(p => p.Uri);

        foreach ((string prop, CodeSystem.PropertyType type) in _properties)
        {
            if (!csPropertiesByCode.Contains(prop) &&
                !csPropertiesByUri.Contains(_standardConceptPropertiesUrl + "#" + prop))
            {
                v.Property.Add(new CodeSystem.PropertyComponent
                {
                    Code = prop,
                    Uri = _standardConceptPropertiesUrl + "#" + prop,
                    Type = type
                });
            }
        }
    }

    public void Process(ISourceNode node, CodeSystem current)
	{
		switch (node.Name)
		{
			case "system":
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

			case "caseSensitive":
				current.CaseSensitiveElement = new FhirBoolean(_converter._primitive.GetBoolOpt(node));
				break;

			case "_caseSensitive":
				_converter._element.Process(node, current.CaseSensitiveElement);
				break;

			case "valueSet":
				current.ValueSetElement = new Canonical(node.Text);
				break;

			case "hierarchyMeaning":
				current.HierarchyMeaning = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<CodeSystem.CodeSystemHierarchyMeaning>(node.Text);
				break;

			case "_hierarchyMeaning":
				_converter._element.Process(node, current.HierarchyMeaningElement);
				break;

			case "compositional":
				current.CompositionalElement = new FhirBoolean(_converter._primitive.GetBoolOpt(node));
				break;

			case "_compositional":
				_converter._element.Process(node, current.CompositionalElement);
				break;

			case "versionNeeded":
				current.VersionNeededElement = new FhirBoolean(_converter._primitive.GetBoolOpt(node));
				break;

			case "_versionNeeded":
				_converter._element.Process(node, current.VersionNeededElement);
				break;

			case "content":
				current.Content = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<CodeSystemContentMode>(node.Text);
				break;

			case "_content":
				_converter._element.Process(node, current.ContentElement);
				break;

			case "count":
				current.CountElement = new UnsignedInt(_converter._primitive.GetIntOpt(node));
				break;

			case "_count":
				_converter._element.Process(node, current.CountElement);
				break;

			case "filter":
				current.Filter.Add(Extract20CodeSystemFilterComponent(node));
				break;

			case "property":
				current.Property.Add(Extract20CodeSystemPropertyComponent(node));
				break;

			case "concept":
				current.Concept.Add(Extract20CodeSystemConceptDefinitionComponent(node));
				break;

			// process inherited elements
			default:
				_converter._domainResource.Process(node, current);
				break;

		}
	}

	private CodeSystem.FilterComponent Extract20CodeSystemFilterComponent(ISourceNode parent)
	{
		CodeSystem.FilterComponent current = new();

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

				case "description":
					current.DescriptionElement = new FhirString(node.Text);
					break;

				case "_description":
					_converter._element.Process(node, current.DescriptionElement);
					break;

				case "operator":
					current.OperatorElement.Add(new Code<FilterOperator>(Hl7.Fhir.Utility.EnumUtility.ParseLiteral<FilterOperator>(node.Text)));
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

	private CodeSystem.PropertyComponent Extract20CodeSystemPropertyComponent(ISourceNode parent)
	{
		CodeSystem.PropertyComponent current = new();

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

				case "uri":
					current.UriElement = new FhirUri(node.Text);
					break;

				case "_uri":
					_converter._element.Process(node, current.UriElement);
					break;

				case "description":
					current.DescriptionElement = new FhirString(node.Text);
					break;

				case "_description":
					_converter._element.Process(node, current.DescriptionElement);
					break;

				case "type":
					current.Type = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<CodeSystem.PropertyType>(node.Text);
					break;

				case "_type":
					_converter._element.Process(node, current.TypeElement);
					break;

				// process inherited elements
				default:
					_converter._backboneElement.Process(node, current);
					break;

			}
		}

		return current;
	}

	private CodeSystem.ConceptDefinitionComponent Extract20CodeSystemConceptDefinitionComponent(ISourceNode parent)
	{
		CodeSystem.ConceptDefinitionComponent current = new();

		foreach (ISourceNode node in parent.Children())
		{
			switch (node.Name)
			{
                case "abstract":
                    current.Property.Add(new()
                    {
                        Code = "notSelectable",
                        Value = new FhirBoolean(_converter._primitive.GetBoolOpt(node))
                    });
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

				case "definition":
					current.DefinitionElement = new FhirString(node.Text);
					break;

				case "_definition":
					_converter._element.Process(node, current.DefinitionElement);
					break;

				case "designation":
					current.Designation.Add(Extract20CodeSystemDesignationComponent(node));
					break;

				case "property":
					current.Property.Add(Extract20CodeSystemConceptPropertyComponent(node));
					break;

				case "concept":
					current.Concept.Add(Extract20CodeSystemConceptDefinitionComponent(node));
					break;

				// process inherited elements
				default:
					_converter._backboneElement.Process(node, current);
					break;

			}
		}

		return current;
	}

	private CodeSystem.DesignationComponent Extract20CodeSystemDesignationComponent(ISourceNode parent)
	{
		CodeSystem.DesignationComponent current = new();

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

	private CodeSystem.ConceptPropertyComponent Extract20CodeSystemConceptPropertyComponent(ISourceNode parent)
	{
		CodeSystem.ConceptPropertyComponent current = new();

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

				case "valueCode":
					current.Value = new Code(node.Text);
					break;

				case "_valueCode":
					_converter._element.Process(node, current.Value);
					break;

				case "valueCoding":
					current.Value = _converter._coding.Extract(node);
					break;

				case "valueString":
					current.Value = new FhirString(node.Text);
					break;

				case "_valueString":
					_converter._element.Process(node, current.Value);
					break;

				case "valueInteger":
					current.Value = new Integer(_converter._primitive.GetInt(node));
					break;

				case "_valueInteger":
					_converter._element.Process(node, current.Value);
					break;

				case "valueBoolean":
					current.Value = new FhirBoolean(_converter._primitive.GetBool(node));
					break;

				case "_valueBoolean":
					_converter._element.Process(node, current.Value);
					break;

				case "valueDateTime":
					current.Value = new FhirDateTime(node.Text);
					break;

				case "_valueDateTime":
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
}
