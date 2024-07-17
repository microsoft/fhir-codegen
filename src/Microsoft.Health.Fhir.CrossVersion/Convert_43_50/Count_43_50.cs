// <copyright file="Count.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_43_50;

public class Count_43_50 : ICrossVersionProcessor<Count>, ICrossVersionExtractor<Count>
{
	private Converter_43_50 _converter;
	internal Count_43_50(Converter_43_50 converter)
	{
		_converter = converter;
	}

	public Count Extract(ISourceNode node)
	{
		Count v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, Count current)
	{
		switch (node.Name)
		{
			// process inherited elements
			default:
				_converter._quantity.Process(node, current);
				break;

		}
	}
}
