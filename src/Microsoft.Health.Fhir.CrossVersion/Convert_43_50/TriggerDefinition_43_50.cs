// <copyright file="TriggerDefinition.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_43_50;

public class TriggerDefinition_43_50 : ICrossVersionProcessor<TriggerDefinition>, ICrossVersionExtractor<TriggerDefinition>
{
	private Converter_43_50 _converter;
	internal TriggerDefinition_43_50(Converter_43_50 converter)
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

			case "name":
				current.NameElement = new FhirString(node.Text);
				break;

			case "_name":
				_converter._element.Process(node, current.NameElement);
				break;

			case "timingTiming":
				current.Timing = _converter._timing.Extract(node);
				break;

			case "timingReference":
				current.Timing = new ResourceReference(node.Text);
				break;

			case "timingDate":
				current.Timing = new Date(node.Text);
				break;

			case "_timingDate":
				_converter._element.Process(node, current.Timing);
				break;

			case "timingDateTime":
				current.Timing = new FhirDateTime(node.Text);
				break;

			case "_timingDateTime":
				_converter._element.Process(node, current.Timing);
				break;

			case "data":
				current.Data.Add(_converter._dataRequirement.Extract(node));
				break;

			case "condition":
				current.Condition = _converter._expression.Extract(node);
				break;

			// process inherited elements
			default:
				_converter._element.Process(node, current);
				break;

		}
	}
}
