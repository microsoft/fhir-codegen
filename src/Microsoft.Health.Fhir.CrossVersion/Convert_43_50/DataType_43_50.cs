// <copyright file="DataType.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_43_50;

public class DataType_43_50 : ICrossVersionProcessor<DataType>
{
	private Converter_43_50 _converter;
	internal DataType_43_50(Converter_43_50 converter)
	{
		_converter = converter;
	}

	public void Process(ISourceNode node, DataType current)
	{
		switch (node.Name)
		{
			// process inherited elements
			default:
				_converter._element.Process(node, current);
				break;

		}
	}
}
