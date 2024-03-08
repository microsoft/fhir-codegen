// <copyright file="RelatedArtifact.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_30_50;

public class RelatedArtifact_30_50 : ICrossVersionProcessor<RelatedArtifact>, ICrossVersionExtractor<RelatedArtifact>
{
	private Converter_30_50 _converter;
	internal RelatedArtifact_30_50(Converter_30_50 converter)
	{
		_converter = converter;
	}

	public RelatedArtifact Extract(ISourceNode node)
	{
		RelatedArtifact v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, RelatedArtifact current)
	{
		switch (node.Name)
		{
			case "type":
				current.Type = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<RelatedArtifact.RelatedArtifactType>(node.Text);
				break;

			case "_type":
				_converter._element.Process(node, current.TypeElement);
				break;

			case "display":
				current.DisplayElement = new FhirString(node.Text);
				break;

			case "_display":
				_converter._element.Process(node, current.DisplayElement);
				break;

			case "citation":
				current.CitationElement = new Markdown(node.Text);
				break;

			case "_citation":
				_converter._element.Process(node, current.CitationElement);
				break;

			case "url":
				// element RelatedArtifact.url has been removed in the target spec
				break;

			case "document":
				current.Document = _converter._attachment.Extract(node);
				break;

			case "resource":
				current.ResourceElement = new Canonical(node.Text);
				break;

			// process inherited elements
			default:
				_converter._element.Process(node, current);
				break;

		}
	}
}
