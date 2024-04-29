// <copyright file="Extension.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_30_50;

public class Extension_30_50 : ICrossVersionProcessor<Extension>, ICrossVersionExtractor<Extension>
{
	private Converter_30_50 _converter;
	internal Extension_30_50(Converter_30_50 converter)
	{
		_converter = converter;
	}

	public Extension Extract(ISourceNode node)
	{
		Extension v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, Extension current)
	{
		switch (node.Name)
		{
			case "url":
				current.Url = node.Text;
				break;

			case "valueBase64Binary":
				current.Value = new Base64Binary(_converter._primitive.GetByteArrayOpt(node));
				break;

			case "_valueBase64Binary":
                _converter._element.Process(node, current.Value);
                break;

			case "valueBoolean":
				current.Value = new FhirBoolean(_converter._primitive.GetBoolOpt(node));
				break;

			case "_valueBoolean":
				_converter._element.Process(node, current.Value);
				break;

			case "valueCode":
				current.Value = new Code(node.Text);
				break;

			case "_valueCode":
				_converter._element.Process(node, current.Value);
				break;

			case "valueDate":
				current.Value = new Date(node.Text);
				break;

			case "_valueDate":
				_converter._element.Process(node, current.Value);
				break;

			case "valueDateTime":
				current.Value = new FhirDateTime(node.Text);
				break;

			case "_valueDateTime":
				_converter._element.Process(node, current.Value);
				break;

			case "valueDecimal":
				current.Value = new FhirDecimal(_converter._primitive.GetDecimalOpt(node));
				break;

			case "_valueDecimal":
				_converter._element.Process(node, current.Value);
				break;

			case "valueId":
				current.Value = new Id(node.Text);
				break;

			case "_valueId":
				_converter._element.Process(node, current.Value);
				break;

			case "valueInstant":
				current.Value = new Instant(new FhirDateTime(node.Text).ToDateTimeOffset(TimeSpan.Zero));
				break;

			case "_valueInstant":
				_converter._element.Process(node, current.Value);
				break;

			case "valueInteger":
				current.Value = new Integer(_converter._primitive.GetIntOpt(node));
				break;

			case "_valueInteger":
				_converter._element.Process(node, current.Value);
				break;

			case "valueMarkdown":
				current.Value = new Markdown(node.Text);
				break;

			case "_valueMarkdown":
				_converter._element.Process(node, current.Value);
				break;

			case "valueOid":
				current.Value = new Oid(node.Text);
				break;

			case "_valueOid":
				_converter._element.Process(node, current.Value);
				break;

			case "valuePositiveInt":
				current.Value = new PositiveInt(_converter._primitive.GetIntOpt(node));
				break;

			case "_valuePositiveInt":
				_converter._element.Process(node, current.Value);
				break;

			case "valueString":
				current.Value = new FhirString(node.Text);
				break;

			case "_valueString":
				_converter._element.Process(node, current.Value);
				break;

			case "valueTime":
				current.Value = new Time(node.Text);
				break;

			case "_valueTime":
				_converter._element.Process(node, current.Value);
				break;

			case "valueUnsignedInt":
				current.Value = new UnsignedInt(_converter._primitive.GetIntOpt(node));
				break;

			case "_valueUnsignedInt":
				_converter._element.Process(node, current.Value);
				break;

			case "valueUri":
				current.Value = new FhirUri(node.Text);
				break;

			case "_valueUri":
				_converter._element.Process(node, current.Value);
				break;

			case "valueAddress":
				current.Value = _converter._address.Extract(node);
				break;

			case "valueAge":
				current.Value = _converter._age.Extract(node);
				break;

			case "valueAnnotation":
				current.Value = _converter._annotation.Extract(node);
				break;

			case "valueAttachment":
				current.Value = _converter._attachment.Extract(node);
				break;

			case "valueCodeableConcept":
				current.Value = _converter._codeableConcept.Extract(node);
				break;

			case "valueCoding":
				current.Value = _converter._coding.Extract(node);
				break;

			case "valueContactPoint":
				current.Value = _converter._contactPoint.Extract(node);
				break;

			case "valueCount":
				current.Value = _converter._count.Extract(node);
				break;

			case "valueDistance":
				current.Value = _converter._distance.Extract(node);
				break;

			case "valueDuration":
				current.Value = _converter._duration.Extract(node);
				break;

			case "valueHumanName":
				current.Value = _converter._humanName.Extract(node);
				break;

			case "valueIdentifier":
				current.Value = _converter._identifier.Extract(node);
				break;

			case "valueMoney":
				current.Value = _converter._money.Extract(node);
				break;

			case "valuePeriod":
				current.Value = _converter._period.Extract(node);
				break;

			case "valueQuantity":
				current.Value = _converter._quantity.Extract(node);
				break;

			case "valueRange":
				current.Value = _converter._range.Extract(node);
				break;

			case "valueRatio":
				current.Value = _converter._ratio.Extract(node);
				break;

			case "valueReference":
				current.Value = new ResourceReference(node.Text);
				break;

			case "valueSampledData":
				current.Value = _converter._sampledData.Extract(node);
				break;

			case "valueSignature":
				current.Value = _converter._signature.Extract(node);
				break;

			case "valueTiming":
				current.Value = _converter._timing.Extract(node);
				break;

			case "valueMeta":
				current.Value = _converter._meta.Extract(node);
				break;

			// process inherited elements
			default:
				_converter._element.Process(node, current);
				break;

		}
	}
}
