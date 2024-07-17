// <copyright file="ElementDefinition.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_43_50;

public class ElementDefinition_43_50 : ICrossVersionProcessor<ElementDefinition>, ICrossVersionExtractor<ElementDefinition>
{
	private Converter_43_50 _converter;
	internal ElementDefinition_43_50(Converter_43_50 converter)
	{
		_converter = converter;
	}

	public ElementDefinition Extract(ISourceNode node)
	{
		ElementDefinition v = new();
		foreach (ISourceNode child in node.Children())
		{
			Process(child, v);
		}

		return v;
	}

	public void Process(ISourceNode node, ElementDefinition current)
	{
		switch (node.Name)
		{
			case "path":
				current.PathElement = new FhirString(node.Text);
				break;

			case "_path":
				_converter._element.Process(node, current.PathElement);
				break;

			case "representation":
				current.RepresentationElement.Add(new Code<ElementDefinition.PropertyRepresentation>(Hl7.Fhir.Utility.EnumUtility.ParseLiteral<ElementDefinition.PropertyRepresentation>(node.Text)));
				break;

			case "sliceName":
				current.SliceNameElement = new FhirString(node.Text);
				break;

			case "_sliceName":
				_converter._element.Process(node, current.SliceNameElement);
				break;

			case "sliceIsConstraining":
				current.SliceIsConstrainingElement = new FhirBoolean(_converter._primitive.GetBoolOpt(node));
				break;

			case "_sliceIsConstraining":
				_converter._element.Process(node, current.SliceIsConstrainingElement);
				break;

			case "label":
				current.LabelElement = new FhirString(node.Text);
				break;

			case "_label":
				_converter._element.Process(node, current.LabelElement);
				break;

			case "code":
				current.Code.Add(_converter._coding.Extract(node));
				break;

			case "slicing":
				current.Slicing = Extract43ElementDefinitionSlicingComponent(node);
				break;

			case "short":
				current.ShortElement = new FhirString(node.Text);
				break;

			case "_short":
				_converter._element.Process(node, current.ShortElement);
				break;

			case "definition":
				current.DefinitionElement = new Markdown(node.Text);
				break;

			case "_definition":
				_converter._element.Process(node, current.DefinitionElement);
				break;

			case "comment":
				current.CommentElement = new Markdown(node.Text);
				break;

			case "_comment":
				_converter._element.Process(node, current.CommentElement);
				break;

			case "requirements":
				current.RequirementsElement = new Markdown(node.Text);
				break;

			case "_requirements":
				_converter._element.Process(node, current.RequirementsElement);
				break;

			case "alias":
				current.AliasElement.Add(new FhirString(node.Text));
				break;

			case "min":
				current.MinElement = new UnsignedInt(_converter._primitive.GetIntOpt(node));
				break;

			case "_min":
				_converter._element.Process(node, current.MinElement);
				break;

			case "max":
				current.MaxElement = new FhirString(node.Text);
				break;

			case "_max":
				_converter._element.Process(node, current.MaxElement);
				break;

			case "base":
				current.Base = Extract43ElementDefinitionBaseComponent(node);
				break;

			case "contentReference":
				current.ContentReferenceElement = new FhirUri(node.Text);
				break;

			case "_contentReference":
				_converter._element.Process(node, current.ContentReferenceElement);
				break;

			case "type":
				current.Type.Add(Extract43ElementDefinitionTypeRefComponent(node));
				break;

			case "defaultValueBase64Binary":
				current.DefaultValue = new Base64Binary(_converter._primitive.GetByteArrayOpt(node));
				break;

			case "_defaultValueBase64Binary":
				_converter._element.Process(node, current.DefaultValue);
				break;

			case "defaultValueBoolean":
				current.DefaultValue = new FhirBoolean(_converter._primitive.GetBoolOpt(node));
				break;

			case "_defaultValueBoolean":
				_converter._element.Process(node, current.DefaultValue);
				break;

			case "defaultValueCanonical":
				current.DefaultValue = new Canonical(node.Text);
				break;

			case "_defaultValueCanonical":
				_converter._element.Process(node, current.DefaultValue);
				break;

			case "defaultValueCode":
				current.DefaultValue = new Code(node.Text);
				break;

			case "_defaultValueCode":
				_converter._element.Process(node, current.DefaultValue);
				break;

			case "defaultValueDate":
				current.DefaultValue = new Date(node.Text);
				break;

			case "_defaultValueDate":
				_converter._element.Process(node, current.DefaultValue);
				break;

			case "defaultValueDateTime":
				current.DefaultValue = new FhirDateTime(node.Text);
				break;

			case "_defaultValueDateTime":
				_converter._element.Process(node, current.DefaultValue);
				break;

			case "defaultValueDecimal":
				current.DefaultValue = new FhirDecimal(_converter._primitive.GetDecimalOpt(node));
				break;

			case "_defaultValueDecimal":
				_converter._element.Process(node, current.DefaultValue);
				break;

			case "defaultValueId":
				current.DefaultValue = new Id(node.Text);
				break;

			case "_defaultValueId":
				_converter._element.Process(node, current.DefaultValue);
				break;

			case "defaultValueInstant":
				current.DefaultValue = new Instant(new FhirDateTime(node.Text).ToDateTimeOffset(TimeSpan.Zero));
				break;

			case "_defaultValueInstant":
				_converter._element.Process(node, current.DefaultValue);
				break;

			case "defaultValueInteger":
				current.DefaultValue = new Integer(_converter._primitive.GetIntOpt(node));
				break;

			case "_defaultValueInteger":
				_converter._element.Process(node, current.DefaultValue);
				break;

			case "defaultValueMarkdown":
				current.DefaultValue = new Markdown(node.Text);
				break;

			case "_defaultValueMarkdown":
				_converter._element.Process(node, current.DefaultValue);
				break;

			case "defaultValueOid":
				current.DefaultValue = new Oid(node.Text);
				break;

			case "_defaultValueOid":
				_converter._element.Process(node, current.DefaultValue);
				break;

			case "defaultValuePositiveInt":
				current.DefaultValue = new PositiveInt(_converter._primitive.GetIntOpt(node));
				break;

			case "_defaultValuePositiveInt":
				_converter._element.Process(node, current.DefaultValue);
				break;

			case "defaultValueString":
				current.DefaultValue = new FhirString(node.Text);
				break;

			case "_defaultValueString":
				_converter._element.Process(node, current.DefaultValue);
				break;

			case "defaultValueTime":
				current.DefaultValue = new Time(node.Text);
				break;

			case "_defaultValueTime":
				_converter._element.Process(node, current.DefaultValue);
				break;

			case "defaultValueUnsignedInt":
				current.DefaultValue = new UnsignedInt(_converter._primitive.GetIntOpt(node));
				break;

			case "_defaultValueUnsignedInt":
				_converter._element.Process(node, current.DefaultValue);
				break;

			case "defaultValueUri":
				current.DefaultValue = new FhirUri(node.Text);
				break;

			case "_defaultValueUri":
				_converter._element.Process(node, current.DefaultValue);
				break;

			case "defaultValueUrl":
				current.DefaultValue = new FhirUrl(node.Text);
				break;

			case "_defaultValueUrl":
				_converter._element.Process(node, current.DefaultValue);
				break;

			case "defaultValueUuid":
				current.DefaultValue = new Uuid(node.Text);
				break;

			case "_defaultValueUuid":
				_converter._element.Process(node, current.DefaultValue);
				break;

			case "defaultValueAddress":
				current.DefaultValue = _converter._address.Extract(node);
				break;

			case "defaultValueAge":
				current.DefaultValue = _converter._age.Extract(node);
				break;

			case "defaultValueAnnotation":
				current.DefaultValue = _converter._annotation.Extract(node);
				break;

			case "defaultValueAttachment":
				current.DefaultValue = _converter._attachment.Extract(node);
				break;

			case "defaultValueCodeableConcept":
				current.DefaultValue = _converter._codeableConcept.Extract(node);
				break;

			case "defaultValueCodeableReference":
				current.DefaultValue = _converter._codeableReference.Extract(node);
				break;

			case "defaultValueCoding":
				current.DefaultValue = _converter._coding.Extract(node);
				break;

			case "defaultValueContactPoint":
				current.DefaultValue = _converter._contactPoint.Extract(node);
				break;

			case "defaultValueCount":
				current.DefaultValue = _converter._count.Extract(node);
				break;

			case "defaultValueDistance":
				current.DefaultValue = _converter._distance.Extract(node);
				break;

			case "defaultValueDuration":
				current.DefaultValue = _converter._duration.Extract(node);
				break;

			case "defaultValueHumanName":
				current.DefaultValue = _converter._humanName.Extract(node);
				break;

			case "defaultValueIdentifier":
				current.DefaultValue = _converter._identifier.Extract(node);
				break;

			case "defaultValueMoney":
				current.DefaultValue = _converter._money.Extract(node);
				break;

			case "defaultValuePeriod":
				current.DefaultValue = _converter._period.Extract(node);
				break;

			case "defaultValueQuantity":
				current.DefaultValue = _converter._quantity.Extract(node);
				break;

			case "defaultValueRange":
				current.DefaultValue = _converter._range.Extract(node);
				break;

			case "defaultValueRatio":
				current.DefaultValue = _converter._ratio.Extract(node);
				break;

			case "defaultValueRatioRange":
				current.DefaultValue = _converter._ratioRange.Extract(node);
				break;

			case "defaultValueReference":
				current.DefaultValue = new ResourceReference(node.Text);
				break;

			case "defaultValueSampledData":
				current.DefaultValue = _converter._sampledData.Extract(node);
				break;

			case "defaultValueSignature":
				current.DefaultValue = _converter._signature.Extract(node);
				break;

			case "defaultValueTiming":
				current.DefaultValue = _converter._timing.Extract(node);
				break;

			case "defaultValueContactDetail":
				current.DefaultValue = _converter._contactDetail.Extract(node);
				break;

			case "defaultValueContributor":
				current.DefaultValue = _converter._contributor.Extract(node);
				break;

			case "defaultValueDataRequirement":
				current.DefaultValue = _converter._dataRequirement.Extract(node);
				break;

			case "defaultValueExpression":
				current.DefaultValue = _converter._expression.Extract(node);
				break;

			case "defaultValueParameterDefinition":
				current.DefaultValue = _converter._parameterDefinition.Extract(node);
				break;

			case "defaultValueRelatedArtifact":
				current.DefaultValue = _converter._relatedArtifact.Extract(node);
				break;

			case "defaultValueTriggerDefinition":
				current.DefaultValue = _converter._triggerDefinition.Extract(node);
				break;

			case "defaultValueUsageContext":
				current.DefaultValue = _converter._usageContext.Extract(node);
				break;

			case "defaultValueDosage":
				current.DefaultValue = _converter._dosage.Extract(node);
				break;

			case "meaningWhenMissing":
				current.MeaningWhenMissingElement = new Markdown(node.Text);
				break;

			case "_meaningWhenMissing":
				_converter._element.Process(node, current.MeaningWhenMissingElement);
				break;

			case "orderMeaning":
				current.OrderMeaningElement = new FhirString(node.Text);
				break;

			case "_orderMeaning":
				_converter._element.Process(node, current.OrderMeaningElement);
				break;

			case "fixedBase64Binary":
				current.Fixed = new Base64Binary(_converter._primitive.GetByteArrayOpt(node));
				break;

			case "_fixedBase64Binary":
				_converter._element.Process(node, current.Fixed);
				break;

			case "fixedBoolean":
				current.Fixed = new FhirBoolean(_converter._primitive.GetBoolOpt(node));
				break;

			case "_fixedBoolean":
				_converter._element.Process(node, current.Fixed);
				break;

			case "fixedCanonical":
				current.Fixed = new Canonical(node.Text);
				break;

			case "_fixedCanonical":
				_converter._element.Process(node, current.Fixed);
				break;

			case "fixedCode":
				current.Fixed = new Code(node.Text);
				break;

			case "_fixedCode":
				_converter._element.Process(node, current.Fixed);
				break;

			case "fixedDate":
				current.Fixed = new Date(node.Text);
				break;

			case "_fixedDate":
				_converter._element.Process(node, current.Fixed);
				break;

			case "fixedDateTime":
				current.Fixed = new FhirDateTime(node.Text);
				break;

			case "_fixedDateTime":
				_converter._element.Process(node, current.Fixed);
				break;

			case "fixedDecimal":
				current.Fixed = new FhirDecimal(_converter._primitive.GetDecimalOpt(node));
				break;

			case "_fixedDecimal":
				_converter._element.Process(node, current.Fixed);
				break;

			case "fixedId":
				current.Fixed = new Id(node.Text);
				break;

			case "_fixedId":
				_converter._element.Process(node, current.Fixed);
				break;

			case "fixedInstant":
				current.Fixed = new Instant(new FhirDateTime(node.Text).ToDateTimeOffset(TimeSpan.Zero));
				break;

			case "_fixedInstant":
				_converter._element.Process(node, current.Fixed);
				break;

			case "fixedInteger":
				current.Fixed = new Integer(_converter._primitive.GetIntOpt(node));
				break;

			case "_fixedInteger":
				_converter._element.Process(node, current.Fixed);
				break;

			case "fixedMarkdown":
				current.Fixed = new Markdown(node.Text);
				break;

			case "_fixedMarkdown":
				_converter._element.Process(node, current.Fixed);
				break;

			case "fixedOid":
				current.Fixed = new Oid(node.Text);
				break;

			case "_fixedOid":
				_converter._element.Process(node, current.Fixed);
				break;

			case "fixedPositiveInt":
				current.Fixed = new PositiveInt(_converter._primitive.GetIntOpt(node));
				break;

			case "_fixedPositiveInt":
				_converter._element.Process(node, current.Fixed);
				break;

			case "fixedString":
				current.Fixed = new FhirString(node.Text);
				break;

			case "_fixedString":
				_converter._element.Process(node, current.Fixed);
				break;

			case "fixedTime":
				current.Fixed = new Time(node.Text);
				break;

			case "_fixedTime":
				_converter._element.Process(node, current.Fixed);
				break;

			case "fixedUnsignedInt":
				current.Fixed = new UnsignedInt(_converter._primitive.GetIntOpt(node));
				break;

			case "_fixedUnsignedInt":
				_converter._element.Process(node, current.Fixed);
				break;

			case "fixedUri":
				current.Fixed = new FhirUri(node.Text);
				break;

			case "_fixedUri":
				_converter._element.Process(node, current.Fixed);
				break;

			case "fixedUrl":
				current.Fixed = new FhirUrl(node.Text);
				break;

			case "_fixedUrl":
				_converter._element.Process(node, current.Fixed);
				break;

			case "fixedUuid":
				current.Fixed = new Uuid(node.Text);
				break;

			case "_fixedUuid":
				_converter._element.Process(node, current.Fixed);
				break;

			case "fixedAddress":
				current.Fixed = _converter._address.Extract(node);
				break;

			case "fixedAge":
				current.Fixed = _converter._age.Extract(node);
				break;

			case "fixedAnnotation":
				current.Fixed = _converter._annotation.Extract(node);
				break;

			case "fixedAttachment":
				current.Fixed = _converter._attachment.Extract(node);
				break;

			case "fixedCodeableConcept":
				current.Fixed = _converter._codeableConcept.Extract(node);
				break;

			case "fixedCodeableReference":
				current.Fixed = _converter._codeableReference.Extract(node);
				break;

			case "fixedCoding":
				current.Fixed = _converter._coding.Extract(node);
				break;

			case "fixedContactPoint":
				current.Fixed = _converter._contactPoint.Extract(node);
				break;

			case "fixedCount":
				current.Fixed = _converter._count.Extract(node);
				break;

			case "fixedDistance":
				current.Fixed = _converter._distance.Extract(node);
				break;

			case "fixedDuration":
				current.Fixed = _converter._duration.Extract(node);
				break;

			case "fixedHumanName":
				current.Fixed = _converter._humanName.Extract(node);
				break;

			case "fixedIdentifier":
				current.Fixed = _converter._identifier.Extract(node);
				break;

			case "fixedMoney":
				current.Fixed = _converter._money.Extract(node);
				break;

			case "fixedPeriod":
				current.Fixed = _converter._period.Extract(node);
				break;

			case "fixedQuantity":
				current.Fixed = _converter._quantity.Extract(node);
				break;

			case "fixedRange":
				current.Fixed = _converter._range.Extract(node);
				break;

			case "fixedRatio":
				current.Fixed = _converter._ratio.Extract(node);
				break;

			case "fixedRatioRange":
				current.Fixed = _converter._ratioRange.Extract(node);
				break;

			case "fixedReference":
				current.Fixed = new ResourceReference(node.Text);
				break;

			case "fixedSampledData":
				current.Fixed = _converter._sampledData.Extract(node);
				break;

			case "fixedSignature":
				current.Fixed = _converter._signature.Extract(node);
				break;

			case "fixedTiming":
				current.Fixed = _converter._timing.Extract(node);
				break;

			case "fixedContactDetail":
				current.Fixed = _converter._contactDetail.Extract(node);
				break;

			case "fixedContributor":
				current.Fixed = _converter._contributor.Extract(node);
				break;

			case "fixedDataRequirement":
				current.Fixed = _converter._dataRequirement.Extract(node);
				break;

			case "fixedExpression":
				current.Fixed = _converter._expression.Extract(node);
				break;

			case "fixedParameterDefinition":
				current.Fixed = _converter._parameterDefinition.Extract(node);
				break;

			case "fixedRelatedArtifact":
				current.Fixed = _converter._relatedArtifact.Extract(node);
				break;

			case "fixedTriggerDefinition":
				current.Fixed = _converter._triggerDefinition.Extract(node);
				break;

			case "fixedUsageContext":
				current.Fixed = _converter._usageContext.Extract(node);
				break;

			case "fixedDosage":
				current.Fixed = _converter._dosage.Extract(node);
				break;

			case "patternBase64Binary":
				current.Pattern = new Base64Binary(_converter._primitive.GetByteArrayOpt(node));
				break;

			case "_patternBase64Binary":
				_converter._element.Process(node, current.Pattern);
				break;

			case "patternBoolean":
				current.Pattern = new FhirBoolean(_converter._primitive.GetBoolOpt(node));
				break;

			case "_patternBoolean":
				_converter._element.Process(node, current.Pattern);
				break;

			case "patternCanonical":
				current.Pattern = new Canonical(node.Text);
				break;

			case "_patternCanonical":
				_converter._element.Process(node, current.Pattern);
				break;

			case "patternCode":
				current.Pattern = new Code(node.Text);
				break;

			case "_patternCode":
				_converter._element.Process(node, current.Pattern);
				break;

			case "patternDate":
				current.Pattern = new Date(node.Text);
				break;

			case "_patternDate":
				_converter._element.Process(node, current.Pattern);
				break;

			case "patternDateTime":
				current.Pattern = new FhirDateTime(node.Text);
				break;

			case "_patternDateTime":
				_converter._element.Process(node, current.Pattern);
				break;

			case "patternDecimal":
				current.Pattern = new FhirDecimal(_converter._primitive.GetDecimalOpt(node));
				break;

			case "_patternDecimal":
				_converter._element.Process(node, current.Pattern);
				break;

			case "patternId":
				current.Pattern = new Id(node.Text);
				break;

			case "_patternId":
				_converter._element.Process(node, current.Pattern);
				break;

			case "patternInstant":
				current.Pattern = new Instant(new FhirDateTime(node.Text).ToDateTimeOffset(TimeSpan.Zero));
				break;

			case "_patternInstant":
				_converter._element.Process(node, current.Pattern);
				break;

			case "patternInteger":
				current.Pattern = new Integer(_converter._primitive.GetIntOpt(node));
				break;

			case "_patternInteger":
				_converter._element.Process(node, current.Pattern);
				break;

			case "patternMarkdown":
				current.Pattern = new Markdown(node.Text);
				break;

			case "_patternMarkdown":
				_converter._element.Process(node, current.Pattern);
				break;

			case "patternOid":
				current.Pattern = new Oid(node.Text);
				break;

			case "_patternOid":
				_converter._element.Process(node, current.Pattern);
				break;

			case "patternPositiveInt":
				current.Pattern = new PositiveInt(_converter._primitive.GetIntOpt(node));
				break;

			case "_patternPositiveInt":
				_converter._element.Process(node, current.Pattern);
				break;

			case "patternString":
				current.Pattern = new FhirString(node.Text);
				break;

			case "_patternString":
				_converter._element.Process(node, current.Pattern);
				break;

			case "patternTime":
				current.Pattern = new Time(node.Text);
				break;

			case "_patternTime":
				_converter._element.Process(node, current.Pattern);
				break;

			case "patternUnsignedInt":
				current.Pattern = new UnsignedInt(_converter._primitive.GetIntOpt(node));
				break;

			case "_patternUnsignedInt":
				_converter._element.Process(node, current.Pattern);
				break;

			case "patternUri":
				current.Pattern = new FhirUri(node.Text);
				break;

			case "_patternUri":
				_converter._element.Process(node, current.Pattern);
				break;

			case "patternUrl":
				current.Pattern = new FhirUrl(node.Text);
				break;

			case "_patternUrl":
				_converter._element.Process(node, current.Pattern);
				break;

			case "patternUuid":
				current.Pattern = new Uuid(node.Text);
				break;

			case "_patternUuid":
				_converter._element.Process(node, current.Pattern);
				break;

			case "patternAddress":
				current.Pattern = _converter._address.Extract(node);
				break;

			case "patternAge":
				current.Pattern = _converter._age.Extract(node);
				break;

			case "patternAnnotation":
				current.Pattern = _converter._annotation.Extract(node);
				break;

			case "patternAttachment":
				current.Pattern = _converter._attachment.Extract(node);
				break;

			case "patternCodeableConcept":
				current.Pattern = _converter._codeableConcept.Extract(node);
				break;

			case "patternCodeableReference":
				current.Pattern = _converter._codeableReference.Extract(node);
				break;

			case "patternCoding":
				current.Pattern = _converter._coding.Extract(node);
				break;

			case "patternContactPoint":
				current.Pattern = _converter._contactPoint.Extract(node);
				break;

			case "patternCount":
				current.Pattern = _converter._count.Extract(node);
				break;

			case "patternDistance":
				current.Pattern = _converter._distance.Extract(node);
				break;

			case "patternDuration":
				current.Pattern = _converter._duration.Extract(node);
				break;

			case "patternHumanName":
				current.Pattern = _converter._humanName.Extract(node);
				break;

			case "patternIdentifier":
				current.Pattern = _converter._identifier.Extract(node);
				break;

			case "patternMoney":
				current.Pattern = _converter._money.Extract(node);
				break;

			case "patternPeriod":
				current.Pattern = _converter._period.Extract(node);
				break;

			case "patternQuantity":
				current.Pattern = _converter._quantity.Extract(node);
				break;

			case "patternRange":
				current.Pattern = _converter._range.Extract(node);
				break;

			case "patternRatio":
				current.Pattern = _converter._ratio.Extract(node);
				break;

			case "patternRatioRange":
				current.Pattern = _converter._ratioRange.Extract(node);
				break;

			case "patternReference":
				current.Pattern = new ResourceReference(node.Text);
				break;

			case "patternSampledData":
				current.Pattern = _converter._sampledData.Extract(node);
				break;

			case "patternSignature":
				current.Pattern = _converter._signature.Extract(node);
				break;

			case "patternTiming":
				current.Pattern = _converter._timing.Extract(node);
				break;

			case "patternContactDetail":
				current.Pattern = _converter._contactDetail.Extract(node);
				break;

			case "patternContributor":
				current.Pattern = _converter._contributor.Extract(node);
				break;

			case "patternDataRequirement":
				current.Pattern = _converter._dataRequirement.Extract(node);
				break;

			case "patternExpression":
				current.Pattern = _converter._expression.Extract(node);
				break;

			case "patternParameterDefinition":
				current.Pattern = _converter._parameterDefinition.Extract(node);
				break;

			case "patternRelatedArtifact":
				current.Pattern = _converter._relatedArtifact.Extract(node);
				break;

			case "patternTriggerDefinition":
				current.Pattern = _converter._triggerDefinition.Extract(node);
				break;

			case "patternUsageContext":
				current.Pattern = _converter._usageContext.Extract(node);
				break;

			case "patternDosage":
				current.Pattern = _converter._dosage.Extract(node);
				break;

			case "example":
				current.Example.Add(Extract43ElementDefinitionExampleComponent(node));
				break;

			case "minValueDate":
				current.MinValue = new Date(node.Text);
				break;

			case "_minValueDate":
				_converter._element.Process(node, current.MinValue);
				break;

			case "minValueDateTime":
				current.MinValue = new FhirDateTime(node.Text);
				break;

			case "_minValueDateTime":
				_converter._element.Process(node, current.MinValue);
				break;

			case "minValueInstant":
				current.MinValue = new Instant(new FhirDateTime(node.Text).ToDateTimeOffset(TimeSpan.Zero));
				break;

			case "_minValueInstant":
				_converter._element.Process(node, current.MinValue);
				break;

			case "minValueTime":
				current.MinValue = new Time(node.Text);
				break;

			case "_minValueTime":
				_converter._element.Process(node, current.MinValue);
				break;

			case "minValueDecimal":
				current.MinValue = new FhirDecimal(_converter._primitive.GetDecimalOpt(node));
				break;

			case "_minValueDecimal":
				_converter._element.Process(node, current.MinValue);
				break;

			case "minValueInteger":
				current.MinValue = new Integer(_converter._primitive.GetIntOpt(node));
				break;

			case "_minValueInteger":
				_converter._element.Process(node, current.MinValue);
				break;

			case "minValuePositiveInt":
				current.MinValue = new PositiveInt(_converter._primitive.GetIntOpt(node));
				break;

			case "_minValuePositiveInt":
				_converter._element.Process(node, current.MinValue);
				break;

			case "minValueUnsignedInt":
				current.MinValue = new UnsignedInt(_converter._primitive.GetIntOpt(node));
				break;

			case "_minValueUnsignedInt":
				_converter._element.Process(node, current.MinValue);
				break;

			case "minValueQuantity":
				current.MinValue = _converter._quantity.Extract(node);
				break;

			case "maxValueDate":
				current.MaxValue = new Date(node.Text);
				break;

			case "_maxValueDate":
				_converter._element.Process(node, current.MaxValue);
				break;

			case "maxValueDateTime":
				current.MaxValue = new FhirDateTime(node.Text);
				break;

			case "_maxValueDateTime":
				_converter._element.Process(node, current.MaxValue);
				break;

			case "maxValueInstant":
				current.MaxValue = new Instant(new FhirDateTime(node.Text).ToDateTimeOffset(TimeSpan.Zero));
				break;

			case "_maxValueInstant":
				_converter._element.Process(node, current.MaxValue);
				break;

			case "maxValueTime":
				current.MaxValue = new Time(node.Text);
				break;

			case "_maxValueTime":
				_converter._element.Process(node, current.MaxValue);
				break;

			case "maxValueDecimal":
				current.MaxValue = new FhirDecimal(_converter._primitive.GetDecimalOpt(node));
				break;

			case "_maxValueDecimal":
				_converter._element.Process(node, current.MaxValue);
				break;

			case "maxValueInteger":
				current.MaxValue = new Integer(_converter._primitive.GetIntOpt(node));
				break;

			case "_maxValueInteger":
				_converter._element.Process(node, current.MaxValue);
				break;

			case "maxValuePositiveInt":
				current.MaxValue = new PositiveInt(_converter._primitive.GetIntOpt(node));
				break;

			case "_maxValuePositiveInt":
				_converter._element.Process(node, current.MaxValue);
				break;

			case "maxValueUnsignedInt":
				current.MaxValue = new UnsignedInt(_converter._primitive.GetIntOpt(node));
				break;

			case "_maxValueUnsignedInt":
				_converter._element.Process(node, current.MaxValue);
				break;

			case "maxValueQuantity":
				current.MaxValue = _converter._quantity.Extract(node);
				break;

			case "maxLength":
				current.MaxLengthElement = new Integer(_converter._primitive.GetIntOpt(node));
				break;

			case "_maxLength":
				_converter._element.Process(node, current.MaxLengthElement);
				break;

			case "condition":
				current.ConditionElement.Add(new Id(node.Text));
				break;

			case "constraint":
				current.Constraint.Add(Extract43ElementDefinitionConstraintComponent(node));
				break;

			case "mustSupport":
				current.MustSupportElement = new FhirBoolean(_converter._primitive.GetBoolOpt(node));
				break;

			case "_mustSupport":
				_converter._element.Process(node, current.MustSupportElement);
				break;

			case "isModifier":
				current.IsModifierElement = new FhirBoolean(_converter._primitive.GetBoolOpt(node));
				break;

			case "_isModifier":
				_converter._element.Process(node, current.IsModifierElement);
				break;

			case "isModifierReason":
				current.IsModifierReasonElement = new FhirString(node.Text);
				break;

			case "_isModifierReason":
				_converter._element.Process(node, current.IsModifierReasonElement);
				break;

			case "isSummary":
				current.IsSummaryElement = new FhirBoolean(_converter._primitive.GetBoolOpt(node));
				break;

			case "_isSummary":
				_converter._element.Process(node, current.IsSummaryElement);
				break;

			case "binding":
				current.Binding = Extract43ElementDefinitionBindingComponent(node);
				break;

			case "mapping":
				current.Mapping.Add(Extract43ElementDefinitionMappingComponent(node));
				break;

			// process inherited elements
			default:
				_converter._element.Process(node, current);
				break;

		}
	}

	private ElementDefinition.SlicingComponent Extract43ElementDefinitionSlicingComponent(ISourceNode parent)
	{
		ElementDefinition.SlicingComponent current = new();

        foreach (ISourceNode node in parent.Children())
        {
            switch (node.Name)
            {
                case "discriminator":
                    current.Discriminator.Add(Extract43ElementDefinitionDiscriminatorComponent(node));
                    break;

                case "description":
                    current.DescriptionElement = new FhirString(node.Text);
                    break;

                case "_description":
                    _converter._element.Process(node, current.DescriptionElement);
                    break;

                case "ordered":
                    current.OrderedElement = new FhirBoolean(_converter._primitive.GetBoolOpt(node));
                    break;

                case "_ordered":
                    _converter._element.Process(node, current.OrderedElement);
                    break;

                case "rules":
                    current.Rules = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<ElementDefinition.SlicingRules>(node.Text);
                    break;

                case "_rules":
                    _converter._element.Process(node, current.RulesElement);
                    break;

                // process inherited elements
                default:
                    _converter._element.Process(node, current);
                    break;
            }
		}

		return current;
	}

	private ElementDefinition.DiscriminatorComponent Extract43ElementDefinitionDiscriminatorComponent(ISourceNode parent)
	{
		ElementDefinition.DiscriminatorComponent current = new();

        foreach (ISourceNode node in parent.Children())
        {
            switch (node.Name)
            {
                case "type":
                    current.Type = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<ElementDefinition.DiscriminatorType>(node.Text);
                    break;

                case "_type":
                    _converter._element.Process(node, current.TypeElement);
                    break;

                case "path":
                    current.PathElement = new FhirString(node.Text);
                    break;

                case "_path":
                    _converter._element.Process(node, current.PathElement);
                    break;

                // process inherited elements
                default:
                    _converter._element.Process(node, current);
                    break;
            }
		}

		return current;
	}

	private ElementDefinition.BaseComponent Extract43ElementDefinitionBaseComponent(ISourceNode parent)
	{
		ElementDefinition.BaseComponent current = new();

        foreach (ISourceNode node in parent.Children())
        {
            switch (node.Name)
            {
                case "path":
                    current.PathElement = new FhirString(node.Text);
                    break;

                case "_path":
                    _converter._element.Process(node, current.PathElement);
                    break;

                case "min":
                    current.MinElement = new UnsignedInt(_converter._primitive.GetInt(node));
                    break;

                case "_min":
                    _converter._element.Process(node, current.MinElement);
                    break;

                case "max":
                    current.MaxElement = new FhirString(node.Text);
                    break;

                case "_max":
                    _converter._element.Process(node, current.MaxElement);
                    break;

                // process inherited elements
                default:
                    _converter._element.Process(node, current);
                    break;
            }
		}

		return current;
	}

	private ElementDefinition.TypeRefComponent Extract43ElementDefinitionTypeRefComponent(ISourceNode parent)
	{
		ElementDefinition.TypeRefComponent current = new();

        foreach (ISourceNode node in parent.Children())
        {
            switch (node.Name)
            {
                case "code":
                    current.CodeElement = new FhirUri(node.Text);
                    break;

                case "_code":
                    _converter._element.Process(node, current.CodeElement);
                    break;

                case "profile":
                    current.ProfileElement.Add(new Canonical(node.Text));
                    break;

                case "targetProfile":
                    current.TargetProfileElement.Add(new Canonical(node.Text));
                    break;

                case "aggregation":
                    current.AggregationElement.Add(new Code<ElementDefinition.AggregationMode>(Hl7.Fhir.Utility.EnumUtility.ParseLiteral<ElementDefinition.AggregationMode>(node.Text)));
                    break;

                case "versioning":
                    current.Versioning = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<ElementDefinition.ReferenceVersionRules>(node.Text);
                    break;

                case "_versioning":
                    _converter._element.Process(node, current.VersioningElement);
                    break;

                // process inherited elements
                default:
                    _converter._element.Process(node, current);
                    break;
            }
		}

		return current;
	}

	private ElementDefinition.ExampleComponent Extract43ElementDefinitionExampleComponent(ISourceNode parent)
	{
		ElementDefinition.ExampleComponent current = new();

        foreach (ISourceNode node in parent.Children())
        {
            switch (node.Name)
            {
                case "label":
                    current.LabelElement = new FhirString(node.Text);
                    break;

                case "_label":
                    _converter._element.Process(node, current.LabelElement);
                    break;

                case "valueBase64Binary":
                    current.Value = new Base64Binary(_converter._primitive.GetByteArray(node));
                    break;

                case "_valueBase64Binary":
                    _converter._element.Process(node, current.Value);
                    break;

                case "valueBoolean":
                    current.Value = new FhirBoolean(_converter._primitive.GetBool(node));
                    break;

                case "_valueBoolean":
                    _converter._element.Process(node, current.Value);
                    break;

                case "valueCanonical":
                    current.Value = new Canonical(node.Text);
                    break;

                case "_valueCanonical":
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
                    current.Value = new FhirDecimal(_converter._primitive.GetDecimal(node));
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
                    current.Value = new Integer(_converter._primitive.GetInt(node));
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
                    current.Value = new PositiveInt(_converter._primitive.GetInt(node));
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
                    current.Value = new UnsignedInt(_converter._primitive.GetInt(node));
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

                case "valueUrl":
                    current.Value = new FhirUrl(node.Text);
                    break;

                case "_valueUrl":
                    _converter._element.Process(node, current.Value);
                    break;

                case "valueUuid":
                    current.Value = new Uuid(node.Text);
                    break;

                case "_valueUuid":
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

                case "valueCodeableReference":
                    current.Value = _converter._codeableReference.Extract(node);
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

                case "valueRatioRange":
                    current.Value = _converter._ratioRange.Extract(node);
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

                case "valueContactDetail":
                    current.Value = _converter._contactDetail.Extract(node);
                    break;

                case "valueContributor":
                    current.Value = _converter._contributor.Extract(node);
                    break;

                case "valueDataRequirement":
                    current.Value = _converter._dataRequirement.Extract(node);
                    break;

                case "valueExpression":
                    current.Value = _converter._expression.Extract(node);
                    break;

                case "valueParameterDefinition":
                    current.Value = _converter._parameterDefinition.Extract(node);
                    break;

                case "valueRelatedArtifact":
                    current.Value = _converter._relatedArtifact.Extract(node);
                    break;

                case "valueTriggerDefinition":
                    current.Value = _converter._triggerDefinition.Extract(node);
                    break;

                case "valueUsageContext":
                    current.Value = _converter._usageContext.Extract(node);
                    break;

                case "valueDosage":
                    current.Value = _converter._dosage.Extract(node);
                    break;

                // process inherited elements
                default:
                    _converter._element.Process(node, current);
                    break;
            }
		}

		return current;
	}

	private ElementDefinition.ConstraintComponent Extract43ElementDefinitionConstraintComponent(ISourceNode parent)
	{
		ElementDefinition.ConstraintComponent current = new();

        foreach (ISourceNode node in parent.Children())
        {
            switch (node.Name)
            {
                case "key":
                    current.KeyElement = new Id(node.Text);
                    break;

                case "_key":
                    _converter._element.Process(node, current.KeyElement);
                    break;

                case "requirements":
                    current.RequirementsElement = new Markdown(node.Text);
                    break;

                case "_requirements":
                    _converter._element.Process(node, current.RequirementsElement);
                    break;

                case "severity":
                    current.Severity = Hl7.Fhir.Utility.EnumUtility.ParseLiteral<ConstraintSeverity>(node.Text);
                    break;

                case "_severity":
                    _converter._element.Process(node, current.SeverityElement);
                    break;

                case "human":
                    current.HumanElement = new FhirString(node.Text);
                    break;

                case "_human":
                    _converter._element.Process(node, current.HumanElement);
                    break;

                case "expression":
                    current.ExpressionElement = new FhirString(node.Text);
                    break;

                case "_expression":
                    _converter._element.Process(node, current.ExpressionElement);
                    break;

                case "xpath":
                    // element ElementDefinition.constraint.xpath has been removed in the target spec
                    break;

                case "source":
                    current.SourceElement = new Canonical(node.Text);
                    break;

                case "_source":
                    _converter._element.Process(node, current.SourceElement);
                    break;

                // process inherited elements
                default:
                    _converter._element.Process(node, current);
                    break;
            }
		}

		return current;
	}

	private ElementDefinition.ElementDefinitionBindingComponent Extract43ElementDefinitionBindingComponent(ISourceNode parent)
	{
		ElementDefinition.ElementDefinitionBindingComponent current = new();

        foreach (ISourceNode node in parent.Children())
        {
            switch (node.Name)
            {
                case "strength":
                    current.StrengthElement = new Code<BindingStrength>(Hl7.Fhir.Utility.EnumUtility.ParseLiteral<BindingStrength>(node.Text));
                    break;

                case "_strength":
                    _converter._element.Process(node, current.StrengthElement);
                    break;

                case "description":
                    current.DescriptionElement = new Markdown(node.Text);
                    break;

                case "_description":
                    _converter._element.Process(node, current.DescriptionElement);
                    break;

                case "valueSet":
                    current.ValueSetElement = new Canonical(node.Text);
                    break;

                case "_valueSet":
                    _converter._element.Process(node, current.ValueSetElement);
                    break;

                // process inherited elements
                default:
                    _converter._element.Process(node, current);
                    break;
            }
		}

		return current;
	}

	private ElementDefinition.MappingComponent Extract43ElementDefinitionMappingComponent(ISourceNode parent)
	{
		ElementDefinition.MappingComponent current = new();

        foreach (ISourceNode node in parent.Children())
        {
            switch (node.Name)
            {
                case "identity":
                    current.IdentityElement = new Id(node.Text);
                    break;

                case "_identity":
                    _converter._element.Process(node, current.IdentityElement);
                    break;

                case "language":
                    current.LanguageElement = new Code(node.Text);
                    break;

                case "_language":
                    _converter._element.Process(node, current.LanguageElement);
                    break;

                case "map":
                    current.MapElement = new FhirString(node.Text);
                    break;

                case "_map":
                    _converter._element.Process(node, current.MapElement);
                    break;

                case "comment":
                    current.CommentElement = new Markdown(node.Text);
                    break;

                case "_comment":
                    _converter._element.Process(node, current.CommentElement);
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
