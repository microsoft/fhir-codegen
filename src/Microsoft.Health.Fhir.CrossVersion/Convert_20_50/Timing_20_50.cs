// <copyright file="Timing.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_20_50;

public class Timing_20_50 : ICrossVersionProcessor<Timing>, ICrossVersionExtractor<Timing>
{
	private Converter_20_50 _converter;
	internal Timing_20_50(Converter_20_50 converter)
	{
		_converter = converter;
	}

	public Timing Extract(ISourceNode node)
	{
		Timing v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, Timing current)
	{
		switch (node.Name)
		{
			case "event":
				current.EventElement.Add(new FhirDateTime(node.Text));
				break;

			case "repeat":
				current.Repeat = Extract20TimingRepeatComponent(node);
				break;

			case "code":
				current.Code = _converter._codeableConcept.Extract(node);
				break;

			// process inherited elements
			default:
				_converter._element.Process(node, current);
				break;

		}
	}

	private Timing.RepeatComponent Extract20TimingRepeatComponent(ISourceNode parent)
	{
		Timing.RepeatComponent current = new();

		foreach (ISourceNode node in parent.Children())
		{
			switch (node.Name)
			{
				case "boundsDuration":
					current.Bounds = _converter._duration.Extract(node);
					break;

				case "boundsRange":
					current.Bounds = _converter._range.Extract(node);
					break;

				case "boundsPeriod":
					current.Bounds = _converter._period.Extract(node);
					break;

				case "count":
					current.CountElement = new PositiveInt(_converter._primitive.GetIntOpt(node));
					break;

				case "countMax":
					current.CountMaxElement = new PositiveInt(_converter._primitive.GetIntOpt(node));
					break;

				case "duration":
					current.DurationElement = new FhirDecimal(_converter._primitive.GetDecimalOpt(node));
					break;

				case "_duration":
					_converter._element.Process(node, current.DurationElement);
					break;

				case "durationMax":
					current.DurationMaxElement = new FhirDecimal(_converter._primitive.GetDecimalOpt(node));
					break;

				case "_durationMax":
					_converter._element.Process(node, current.DurationMaxElement);
					break;

				case "durationUnit":
					current.DurationUnit = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<Timing.UnitsOfTime>(node.Text);
					break;

				case "_durationUnit":
					_converter._element.Process(node, current.DurationUnitElement);
					break;

				case "frequency":
					current.FrequencyElement = new PositiveInt(_converter._primitive.GetIntOpt(node));
					break;

				case "frequencyMax":
					current.FrequencyMaxElement = new PositiveInt(_converter._primitive.GetIntOpt(node));
					break;

				case "period":
					current.PeriodElement = new FhirDecimal(_converter._primitive.GetDecimalOpt(node));
					break;

				case "_period":
					_converter._element.Process(node, current.PeriodElement);
					break;

				case "periodMax":
					current.PeriodMaxElement = new FhirDecimal(_converter._primitive.GetDecimalOpt(node));
					break;

				case "_periodMax":
					_converter._element.Process(node, current.PeriodMaxElement);
					break;

				case "periodUnit":
					current.PeriodUnit = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<Timing.UnitsOfTime>(node.Text);
					break;

				case "_periodUnit":
					_converter._element.Process(node, current.PeriodUnitElement);
					break;

				case "dayOfWeek":
					current.DayOfWeekElement.Add(new Code<DaysOfWeek>(Hl7.Fhir.Utility.EnumUtility.ParseLiteral<DaysOfWeek>(node.Text)));
					break;

				case "timeOfDay":
					current.TimeOfDayElement.Add(new Time(node.Text));
					break;

				case "when":
					current.WhenElement.Add(new Code<Timing.EventTiming>(Hl7.Fhir.Utility.EnumUtility.ParseLiteral<Timing.EventTiming>(node.Text)));
					break;

				case "offset":
					current.OffsetElement = new UnsignedInt(_converter._primitive.GetIntOpt(node));
					break;

				case "_offset":
					_converter._element.Process(node, current.OffsetElement);
					break;

				// process inherited elements
				default:
					_converter._element.Process(node, current);
					break;

			}
		}

		return current;
	}
}
