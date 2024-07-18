// <copyright file="DomainResource.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_30_50;

public class DomainResource_30_50 : ICrossVersionProcessor<DomainResource>
{
	private Converter_30_50 _converter;
	internal DomainResource_30_50(Converter_30_50 converter)
	{
		_converter = converter;
	}

	public void Process(ISourceNode node, DomainResource current)
	{
		switch (node.Name)
		{
			case "text":
				current.Text = _converter._narrative.Extract(node);
				break;

			case "contained":
				current.Contained.Add(_converter._resource.Extract(node));
				break;

			case "extension":
				current.Extension.Add(_converter._extension.Extract(node));
				break;

			case "modifierExtension":
				current.ModifierExtension.Add(_converter._extension.Extract(node));
				break;

			// process inherited elements
			default:
				_converter._resource.Process(node, current);
				break;

		}
	}
}
