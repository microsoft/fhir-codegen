// <copyright file="TriggerDefinition.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_30_50;

public class TriggerDefinition_30_50 : ICrossVersionProcessor<TriggerDefinition>, ICrossVersionExtractor<TriggerDefinition>
{
	private Converter_30_50 _converter;
	internal TriggerDefinition_30_50(Converter_30_50 converter)
	{
		_converter = converter;
	}

	public TriggerDefinition Extract(ISourceNode node)
	{
		TriggerDefinition v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, TriggerDefinition current)
	{
		switch (node.Name)
		{
			case "type":
				current.Type = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<TriggerDefinition.TriggerType>(node.Text);
				break;

			case "_type":
				_converter._element.Process(node, current.TypeElement);
				break;

			case "eventName":
				// element TriggerDefinition.eventName has been removed in the target spec
				break;

			case "eventTimingTiming":
				// element TriggerDefinition.eventTiming[x] has been removed in the target spec
				break;

			case "eventTimingReference":
				// element TriggerDefinition.eventTiming[x] has been removed in the target spec
				break;

			case "eventTimingDate":
				// element TriggerDefinition.eventTiming[x] has been removed in the target spec
				break;

			case "eventTimingDateTime":
				// element TriggerDefinition.eventTiming[x] has been removed in the target spec
				break;

			case "eventData":
				// element TriggerDefinition.eventData has been removed in the target spec
				break;

			// process inherited elements
			default:
				_converter._element.Process(node, current);
				break;

		}
	}
}
