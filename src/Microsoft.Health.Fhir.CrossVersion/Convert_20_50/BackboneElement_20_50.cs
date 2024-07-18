// <copyright file="BackboneElement.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_20_50;

public class BackboneElement_20_50 : ICrossVersionProcessor<BackboneElement>
{
	private Converter_20_50 _converter;
	internal BackboneElement_20_50(Converter_20_50 converter)
	{
		_converter = converter;
	}

	public void Process(ISourceNode node, BackboneElement current)
	{
		switch (node.Name)
		{
			case "modifierExtension":
				current.ModifierExtension.Add(_converter._extension.Extract(node));
				break;

			// process inherited elements
			default:
				_converter._element.Process(node, current);
				break;

		}
	}
}
