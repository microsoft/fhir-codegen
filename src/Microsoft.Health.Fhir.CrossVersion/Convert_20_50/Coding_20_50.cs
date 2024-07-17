// <copyright file="Coding.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_20_50;

public class Coding_20_50 : ICrossVersionProcessor<Coding>, ICrossVersionExtractor<Coding>
{
	private Converter_20_50 _converter;
	internal Coding_20_50(Converter_20_50 converter)
	{
		_converter = converter;
	}

	public Coding Extract(ISourceNode node)
	{
		Coding v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, Coding current)
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

			case "userSelected":
				current.UserSelectedElement = new FhirBoolean(_converter._primitive.GetBoolOpt(node));
				break;

			case "_userSelected":
				_converter._element.Process(node, current.UserSelectedElement);
				break;

			// process inherited elements
			default:
				_converter._element.Process(node, current);
				break;

		}
	}
}
