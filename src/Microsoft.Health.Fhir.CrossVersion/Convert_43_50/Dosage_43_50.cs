// <copyright file="Dosage.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_43_50;

public class Dosage_43_50 : ICrossVersionProcessor<Dosage>, ICrossVersionExtractor<Dosage>
{
	private Converter_43_50 _converter;
	internal Dosage_43_50(Converter_43_50 converter)
	{
		_converter = converter;
	}

	public Dosage Extract(ISourceNode node)
	{
		Dosage v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, Dosage current)
	{
		switch (node.Name)
		{
			case "sequence":
				current.SequenceElement = new Integer(_converter._primitive.GetIntOpt(node));
				break;

			case "_sequence":
				_converter._element.Process(node, current.SequenceElement);
				break;

			case "text":
				current.TextElement = new FhirString(node.Text);
				break;

			case "_text":
				_converter._element.Process(node, current.TextElement);
				break;

			case "additionalInstruction":
				current.AdditionalInstruction.Add(_converter._codeableConcept.Extract(node));
				break;

			case "patientInstruction":
				current.PatientInstructionElement = new FhirString(node.Text);
				break;

			case "_patientInstruction":
				_converter._element.Process(node, current.PatientInstructionElement);
				break;

			case "timing":
				current.Timing = _converter._timing.Extract(node);
				break;

			case "asNeededBoolean":
				// element Dosage.asNeeded[x] has been removed in the target spec
				break;

			case "asNeededCodeableConcept":
				// element Dosage.asNeeded[x] has been removed in the target spec
				break;

			case "site":
				current.Site = _converter._codeableConcept.Extract(node);
				break;

			case "route":
				current.Route = _converter._codeableConcept.Extract(node);
				break;

			case "method":
				current.Method = _converter._codeableConcept.Extract(node);
				break;

			case "doseAndRate":
				current.DoseAndRate.Add(Extract43DosageDoseAndRateComponent(node));
				break;

			case "maxDosePerPeriod":
				current.MaxDosePerPeriod.Add(_converter._ratio.Extract(node));
				break;

			case "maxDosePerAdministration":
				current.MaxDosePerAdministration = _converter._quantity.Extract(node);
				break;

			case "maxDosePerLifetime":
				current.MaxDosePerLifetime = _converter._quantity.Extract(node);
				break;

			// process inherited elements
			default:
				_converter._element.Process(node, current);
				break;

		}
	}

	private Dosage.DoseAndRateComponent Extract43DosageDoseAndRateComponent(ISourceNode parent)
	{
		Dosage.DoseAndRateComponent current = new();

        foreach (ISourceNode node in parent.Children())
        {
            switch (node.Name)
            {
                case "type":
                    current.Type = _converter._codeableConcept.Extract(node);
                    break;

                case "doseRange":
                    current.Dose = _converter._range.Extract(node);
                    break;

                case "doseQuantity":
                    current.Dose = _converter._quantity.Extract(node);
                    break;

                case "rateRatio":
                    current.Rate = _converter._ratio.Extract(node);
                    break;

                case "rateRange":
                    current.Rate = _converter._range.Extract(node);
                    break;

                case "rateQuantity":
                    current.Rate = _converter._quantity.Extract(node);
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
