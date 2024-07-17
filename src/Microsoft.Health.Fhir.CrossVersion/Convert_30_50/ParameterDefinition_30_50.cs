// <copyright file="ParameterDefinition.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_30_50;

public class ParameterDefinition_30_50 : ICrossVersionProcessor<ParameterDefinition>, ICrossVersionExtractor<ParameterDefinition>
{
	private Converter_30_50 _converter;
	internal ParameterDefinition_30_50(Converter_30_50 converter)
	{
		_converter = converter;
	}

	public ParameterDefinition Extract(ISourceNode node)
	{
		ParameterDefinition v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, ParameterDefinition current)
	{
		switch (node.Name)
		{
			case "name":
				current.NameElement = new Code(node.Text);
				break;

			case "_name":
				_converter._element.Process(node, current.NameElement);
				break;

			case "use":
				current.UseElement = new Code<OperationParameterUse>(Hl7.Fhir.Utility.EnumUtility.ParseLiteral<OperationParameterUse>(node.Text));
				break;

			case "_use":
				_converter._element.Process(node, current.UseElement);
				break;

			case "min":
				current.MinElement = new Integer(_converter._primitive.GetIntOpt(node));
				break;

			case "_min":
				_converter._element.Process(node, current.MinElement);
				break;

			case "max":
				current.MaxElement = new FhirString(node.Text);
				break;

			case "_max":
				_converter._element.Process(node, current.MaxElement);
				break;

			case "documentation":
				current.DocumentationElement = new FhirString(node.Text);
				break;

			case "_documentation":
				_converter._element.Process(node, current.DocumentationElement);
				break;

			case "type":
				current.TypeElement = new Code<FHIRAllTypes>(Hl7.Fhir.Utility.EnumUtility.ParseLiteral<FHIRAllTypes>(node.Text));
				break;

			case "_type":
				_converter._element.Process(node, current.TypeElement);
				break;

			case "profile":
				current.ProfileElement = new Canonical(node.Text);
				break;

			// process inherited elements
			default:
				_converter._element.Process(node, current);
				break;

		}
	}
}
