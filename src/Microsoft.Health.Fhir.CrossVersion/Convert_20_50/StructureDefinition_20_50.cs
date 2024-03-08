// <copyright file="StructureDefinition.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_20_50;

public class StructureDefinition_20_50 : ICrossVersionProcessor<StructureDefinition>, ICrossVersionExtractor<StructureDefinition>
{
    private Converter_20_50 _converter;
    internal StructureDefinition_20_50(Converter_20_50 converter)
    {
        _converter = converter;
    }

    string _lastKindLiteral = string.Empty;
    //string _lasConstrainedType = string.Empty;

    public StructureDefinition Extract(ISourceNode node)
    {
        StructureDefinition v = new();

        _lastKindLiteral = string.Empty;
        //_lasConstrainedType = string.Empty;

        foreach (ISourceNode child in node.Children())
        {
            Process(child, v);
        }

        v.Derivation = string.IsNullOrEmpty(v.Type) ? StructureDefinition.TypeDerivationRule.Specialization : StructureDefinition.TypeDerivationRule.Constraint;

        // determine the kind - need to map odd cases from DSTU2 to later element after other processing
        switch (_lastKindLiteral)
        {
            case "datatype":
                {
                    if (v.Type?.Equals("Extension", StringComparison.Ordinal) ?? false)
                    {
                        v.Kind = StructureDefinition.StructureDefinitionKind.Resource;
                    }
                    // leading lower case is primitive
                    else if (char.IsLower(v.Name[0]))
                    {
                        v.Kind = StructureDefinition.StructureDefinitionKind.PrimitiveType;

                        if (v.Type == null)
                        {
                            v.Type = v.Id;
                        }
                    }
                    else
                    {
                        v.Kind = StructureDefinition.StructureDefinitionKind.ComplexType;

                        if (v.Type == null)
                        {
                            if (!string.IsNullOrEmpty(v.BaseDefinition))
                            {
                                v.Type = v.BaseDefinition.Split('/').Last();
                            }
                            else
                            {
                                v.Type = v.Id;
                            }
                        }
                    }
                }
                break;

            case "resource":
                {
                    v.Kind = StructureDefinition.StructureDefinitionKind.Resource;

                    if (v.Type == null)
                    {
                        v.Type = v.Id;
                    }
                }
                break;

            case "logical":
                v.Kind = StructureDefinition.StructureDefinitionKind.Logical;
                break;
        }

        // check for a title in the name
        if (v.Name.Contains(' '))
        {
            v.Title = v.Name;
            v.Name = string.Join(string.Empty, v.Name.Split(' ', '-', StringSplitOptions.RemoveEmptyEntries).Select(n => n = char.ToUpperInvariant(n[0]) + n.Substring(1)));
        }

        //// if they are not the same, the name is actually the title
        //if (!v.Name?.Equals(v.Id, StringComparison.Ordinal) ?? false)
        //{
        //    v.Title = v.Name;
        //    v.Name = v.Id;
        //}

        return v;
    }

    public void Process(ISourceNode node, StructureDefinition current)
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
                //current.TitleElement = new FhirString(node.Text);
                current.NameElement = new FhirString(node.Text);
                break;

            case "_name":
                //_converter._element.Process(node, current.TitleElement);
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

            case "keyword":
                current.Keyword.Add(_converter._coding.Extract(node));
                break;

            case "fhirVersion":
                current.FhirVersionElement = new Code<FHIRVersion>(Hl7.Fhir.Utility.EnumUtility.ParseLiteral<FHIRVersion>(node.Text));
                break;

            case "mapping":
                current.Mapping.Add(Extract20StructureDefinitionMappingComponent(node));
                break;

            case "kind":
                _lastKindLiteral = node.Text;
                break;

            case "_kind":
                _converter._element.Process(node, current.KindElement);
                break;

            case "abstract":
                current.AbstractElement = new FhirBoolean(_converter._primitive.GetBool(node));
                break;

            case "_abstract":
                _converter._element.Process(node, current.AbstractElement);
                break;

            case "contextType":
                // element StructureDefinition.contextType has been removed in the target spec
                //contextTypeLiteral = node.Text;
                break;

            case "context":
                // element StructureDefinition.context has been changed in the target spec too much for first pass conversion
                {
                    current.Context.Add(new StructureDefinition.ContextComponent()
                    {
                        TypeElement = new Code<StructureDefinition.ExtensionContextType>(StructureDefinition.ExtensionContextType.Element),
                        ExpressionElement = new FhirString(node.Text)
                    });


                    //foreach (ISourceNode item in node.Children())
                    //{
                    //    current.Context.Add(new StructureDefinition.ContextComponent()
                    //    {
                    //        TypeElement = new Code<StructureDefinition.ExtensionContextType>(StructureDefinition.ExtensionContextType.Element),
                    //        ExpressionElement = new FhirString(item.Text)
                    //    });
                    //}
                }
                break;

            case "contextInvariant":
                current.ContextInvariantElement.Add(new FhirString(node.Text));
                break;

            case "type":
            case "constrainedType":
                current.TypeElement = new FhirUri(node.Text);
                break;

            case "base":
            case "baseDefinition":
                current.BaseDefinitionElement = new Canonical(node.Text);
                break;

            case "derivation":
                current.Derivation = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<StructureDefinition.TypeDerivationRule>(node.Text);
                break;

            case "_derivation":
                _converter._element.Process(node, current.DerivationElement);
                break;

            case "snapshot":
                current.Snapshot = Extract20StructureDefinitionSnapshotComponent(node);
                break;

            case "differential":
                current.Differential = Extract20StructureDefinitionDifferentialComponent(node);
                break;

            // process inherited elements
            default:
                _converter._domainResource.Process(node, current);
                break;

        }
    }


	private StructureDefinition.MappingComponent Extract20StructureDefinitionMappingComponent(ISourceNode parent)
	{
		StructureDefinition.MappingComponent current = new();

		foreach (ISourceNode node in parent.Children())
		{
			switch (node.Name)
			{
				case "identity":
					current.IdentityElement = new Id(node.Text);
					break;

				case "_identity":
					_converter._element.Process(node, current.IdentityElement);
					break;

				case "uri":
					current.UriElement = new FhirUri(node.Text);
					break;

				case "_uri":
					_converter._element.Process(node, current.UriElement);
					break;

				case "name":
					current.NameElement = new FhirString(node.Text);
					break;

				case "_name":
					_converter._element.Process(node, current.NameElement);
					break;

				case "comment":
					current.CommentElement = new FhirString(node.Text);
					break;

				case "_comment":
					_converter._element.Process(node, current.CommentElement);
					break;

				// process inherited elements
				default:
					_converter._backboneElement.Process(node, current);
					break;

			}
		}

		return current;
	}

	private StructureDefinition.SnapshotComponent Extract20StructureDefinitionSnapshotComponent(ISourceNode parent)
	{
		StructureDefinition.SnapshotComponent current = new();

		foreach (ISourceNode node in parent.Children())
		{
			switch (node.Name)
			{
				case "element":
					current.Element.Add(_converter._elementDefinition.Extract(node));
					break;

				// process inherited elements
				default:
					_converter._backboneElement.Process(node, current);
					break;

			}
		}

		return current;
	}

	private StructureDefinition.DifferentialComponent Extract20StructureDefinitionDifferentialComponent(ISourceNode parent)
	{
		StructureDefinition.DifferentialComponent current = new();

		foreach (ISourceNode node in parent.Children())
		{
			switch (node.Name)
			{
				case "element":
					current.Element.Add(_converter._elementDefinition.Extract(node));
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
