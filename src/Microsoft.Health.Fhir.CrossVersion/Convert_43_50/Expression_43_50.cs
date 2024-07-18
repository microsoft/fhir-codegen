// <copyright file="Expression.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_43_50;

public class Expression_43_50 : ICrossVersionProcessor<Expression>, ICrossVersionExtractor<Expression>
{
	private Converter_43_50 _converter;
	internal Expression_43_50(Converter_43_50 converter)
	{
		_converter = converter;
	}

	public Expression Extract(ISourceNode node)
	{
		Expression v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, Expression current)
	{
		switch (node.Name)
		{
			case "description":
				current.DescriptionElement = new FhirString(node.Text);
				break;

			case "_description":
				_converter._element.Process(node, current.DescriptionElement);
				break;

			case "name":
				current.NameElement = new Code(node.Text);
				break;

			case "_name":
				_converter._element.Process(node, current.NameElement);
				break;

			case "language":
				current.LanguageElement = new Code(node.Text);
				break;

			case "_language":
				_converter._element.Process(node, current.LanguageElement);
				break;

			case "expression":
				current.ExpressionElement = new FhirString(node.Text);
				break;

			case "_expression":
				_converter._element.Process(node, current.ExpressionElement);
				break;

			case "reference":
				current.ReferenceElement = new FhirUri(node.Text);
				break;

			case "_reference":
				_converter._element.Process(node, current.ReferenceElement);
				break;

			// process inherited elements
			default:
				_converter._element.Process(node, current);
				break;

		}
	}
}
