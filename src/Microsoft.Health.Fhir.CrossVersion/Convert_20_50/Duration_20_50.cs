// <copyright file="Duration.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_20_50;

public class Duration_20_50 : ICrossVersionProcessor<Duration>, ICrossVersionExtractor<Duration>
{
	private Converter_20_50 _converter;
	internal Duration_20_50(Converter_20_50 converter)
	{
		_converter = converter;
	}

	public Duration Extract(ISourceNode node)
	{
		Duration v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, Duration current)
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
