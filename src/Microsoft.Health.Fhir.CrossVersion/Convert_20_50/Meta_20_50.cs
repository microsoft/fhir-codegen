// <copyright file="Meta.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_20_50;

public class Meta_20_50 : ICrossVersionProcessor<Meta>, ICrossVersionExtractor<Meta>
{
	private Converter_20_50 _converter;
	internal Meta_20_50(Converter_20_50 converter)
	{
		_converter = converter;
	}

	public Meta Extract(ISourceNode node)
	{
		Meta v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, Meta current)
	{
		switch (node.Name)
		{
			case "versionId":
				current.VersionIdElement = new Id(node.Text);
				break;

			case "_versionId":
				_converter._element.Process(node, current.VersionIdElement);
				break;

			case "lastUpdated":
				current.LastUpdatedElement = new Instant(new FhirDateTime(node.Text).ToDateTimeOffset(TimeSpan.Zero));
				break;

			case "_lastUpdated":
				_converter._element.Process(node, current.LastUpdatedElement);
				break;

			case "profile":
				current.ProfileElement.Add(new FhirUri(node.Text));
				break;

			case "security":
				current.Security.Add(_converter._coding.Extract(node));
				break;

			case "tag":
				current.Tag.Add(_converter._coding.Extract(node));
				break;

			// process inherited elements
			default:
				_converter._element.Process(node, current);
				break;

		}
	}
}
