// <copyright file="CodeableReference.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_43_50;

public class CodeableReference_43_50 : ICrossVersionProcessor<CodeableReference>, ICrossVersionExtractor<CodeableReference>
{
	private Converter_43_50 _converter;
	internal CodeableReference_43_50(Converter_43_50 converter)
	{
		_converter = converter;
	}

	public CodeableReference Extract(ISourceNode node)
	{
		CodeableReference v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, CodeableReference current)
	{
		switch (node.Name)
		{
			case "concept":
				current.Concept = _converter._codeableConcept.Extract(node);
				break;

			case "reference":
				current.Reference = new ResourceReference(node.Text);
				break;

			// process inherited elements
			default:
				_converter._element.Process(node, current);
				break;

		}
	}
}
