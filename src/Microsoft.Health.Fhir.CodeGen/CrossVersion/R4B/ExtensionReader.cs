// <copyright file="ExtensionReader.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Text.Json;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGen.CrossVersion.Common;

namespace Microsoft.Health.Fhir.CodeGen.CrossVersion.R4B;

public class ExtensionReader : BaseParser<Extension>
{
    private static ExtensionReader _instance = new();
    public static ExtensionReader Instance => _instance;

    override public void ReadProperty(ref Utf8JsonReader reader, JsonSerializerOptions options, Extension o, string propertyName)
    {
        if (propertyName.StartsWith('_'))
        {
            o.Value = ElementReader.Instance.ParseExtended(ref reader, options, o.Value);
            return;
        }

        switch (propertyName)
        {
            case "url":
                o.Url = reader.GetString();
                break;

            case "valueBase64Binary":
                {
                    if ((o.Value != null) && (o.Value is Base64Binary oV))
                    {
                        oV.Value = reader.GetBytesFromBase64();
                    }
                    else
                    {
                        o.Value = new Base64Binary(reader.GetBytesFromBase64());
                    }
                }
                break;

            case "valueBoolean":
                {
                    if ((o.Value != null) && (o.Value is FhirBoolean oV))
                    {
                        oV.Value = reader.GetBoolean();
                    }
                    else
                    {
                        o.Value = new FhirBoolean(reader.GetBoolean());
                    }
                }
                break;

            case "valueCanonical":
                {
                    if ((o.Value != null) && (o.Value is Canonical oV))
                    {
                        oV.Value = reader.GetString();
                    }
                    else
                    {
                        o.Value = new Canonical(reader.GetString());
                    }
                }
                break;

            case "valueCode":
                {
                    if ((o.Value != null) && (o.Value is Code oV))
                    {
                        oV.Value = reader.GetString();
                    }
                    else
                    {
                        o.Value = new Code(reader.GetString());
                    }
                }
                break;

            case "valueDate":
                {
                    (object? result, Hl7.Fhir.Serialization.FhirJsonException? error) = PrimitiveReader.DeserializePrimitiveValue(ref reader, typeof(DateTimeOffset), typeof(Date));
                    if ((error == null) && (result != null))
                    {
                        if ((o.Value != null) && (o.Value is Date oV))
                        {
                            oV.Value = ((Date)result).Value;
                        }
                        else
                        {
                            o.Value = (Date)result;
                        }
                    }
                }
                break;

            case "valueDateTime":
                {
                    (object? result, Hl7.Fhir.Serialization.FhirJsonException? error) = PrimitiveReader.DeserializePrimitiveValue(ref reader, typeof(DateTimeOffset), typeof(FhirDateTime));
                    if ((error == null) && (result != null))
                    {
                        if ((o.Value != null) && (o.Value is FhirDateTime oV))
                        {
                            oV.Value = ((FhirDateTime)result).Value;
                        }
                        else
                        {
                            o.Value = (FhirDateTime)result;
                        }
                    }
                }
                break;

            //case "valueDecimal":
            //    o.Value = new FhirDecimal(reader.GetDecimal());
            //    break;

            //case "valueId":
            //    o.Value = new Id(reader.GetString());
            //    break;

            //case "valueInstant":
            //    ValueInstant = reader.GetString();
            //    break;

            //case "valueInteger":
            //    ValueInteger = reader.GetInt32();
            //    break;

            //case "valueMarkdown":
            //    ValueMarkdown = reader.GetString();
            //    break;

            //case "valueOid":
            //    ValueOid = reader.GetString();
            //    break;

            //case "valuePositiveInt":
            //    ValuePositiveInt = reader.GetUInt32();
            //    break;

            //case "valueString":
            //    ValueString = reader.GetString();
            //    break;

            //case "valueTime":
            //    ValueTime = reader.GetString();
            //    break;

            //case "valueUnsignedInt":
            //    ValueUnsignedInt = reader.GetUInt32();
            //    break;

            //case "valueUri":
            //    ValueUri = reader.GetString();
            //    break;

            //case "valueUrl":
            //    ValueUrl = reader.GetString();
            //    break;

            //case "valueUuid":
            //    ValueUuid = reader.GetGuid();
            //    break;

            //case "valueAddress":
            //    ValueAddress = new fhirCsR4B.Models.Address();
            //    ValueAddress.DeserializeJson(ref reader, options);
            //    break;

            //case "valueAge":
            //    ValueAge = new fhirCsR4B.Models.Age();
            //    ValueAge.DeserializeJson(ref reader, options);
            //    break;

            //case "valueAnnotation":
            //    ValueAnnotation = new fhirCsR4B.Models.Annotation();
            //    ValueAnnotation.DeserializeJson(ref reader, options);
            //    break;

            //case "valueAttachment":
            //    ValueAttachment = new fhirCsR4B.Models.Attachment();
            //    ValueAttachment.DeserializeJson(ref reader, options);
            //    break;

            //case "valueCodeableConcept":
            //    ValueCodeableConcept = new fhirCsR4B.Models.CodeableConcept();
            //    ValueCodeableConcept.DeserializeJson(ref reader, options);
            //    break;

            //case "valueCodeableReference":
            //    ValueCodeableReference = new fhirCsR4B.Models.CodeableReference();
            //    ValueCodeableReference.DeserializeJson(ref reader, options);
            //    break;

            //case "valueCoding":
            //    ValueCoding = new fhirCsR4B.Models.Coding();
            //    ValueCoding.DeserializeJson(ref reader, options);
            //    break;

            //case "valueContactPoint":
            //    ValueContactPoint = new fhirCsR4B.Models.ContactPoint();
            //    ValueContactPoint.DeserializeJson(ref reader, options);
            //    break;

            //case "valueCount":
            //    ValueCount = new fhirCsR4B.Models.Count();
            //    ValueCount.DeserializeJson(ref reader, options);
            //    break;

            //case "valueDistance":
            //    ValueDistance = new fhirCsR4B.Models.Distance();
            //    ValueDistance.DeserializeJson(ref reader, options);
            //    break;

            //case "valueDuration":
            //    ValueDuration = new fhirCsR4B.Models.Duration();
            //    ValueDuration.DeserializeJson(ref reader, options);
            //    break;

            //case "valueHumanName":
            //    ValueHumanName = new fhirCsR4B.Models.HumanName();
            //    ValueHumanName.DeserializeJson(ref reader, options);
            //    break;

            //case "valueIdentifier":
            //    ValueIdentifier = new fhirCsR4B.Models.Identifier();
            //    ValueIdentifier.DeserializeJson(ref reader, options);
            //    break;

            //case "valueMoney":
            //    ValueMoney = new fhirCsR4B.Models.Money();
            //    ValueMoney.DeserializeJson(ref reader, options);
            //    break;

            //case "valuePeriod":
            //    ValuePeriod = new fhirCsR4B.Models.Period();
            //    ValuePeriod.DeserializeJson(ref reader, options);
            //    break;

            //case "valueQuantity":
            //    ValueQuantity = new fhirCsR4B.Models.Quantity();
            //    ValueQuantity.DeserializeJson(ref reader, options);
            //    break;

            //case "valueRange":
            //    ValueRange = new fhirCsR4B.Models.Range();
            //    ValueRange.DeserializeJson(ref reader, options);
            //    break;

            //case "valueRatio":
            //    ValueRatio = new fhirCsR4B.Models.Ratio();
            //    ValueRatio.DeserializeJson(ref reader, options);
            //    break;

            //case "valueRatioRange":
            //    ValueRatioRange = new fhirCsR4B.Models.RatioRange();
            //    ValueRatioRange.DeserializeJson(ref reader, options);
            //    break;

            //case "valueReference":
            //    ValueReference = new fhirCsR4B.Models.Reference();
            //    ValueReference.DeserializeJson(ref reader, options);
            //    break;

            //case "valueSampledData":
            //    ValueSampledData = new fhirCsR4B.Models.SampledData();
            //    ValueSampledData.DeserializeJson(ref reader, options);
            //    break;

            //case "valueSignature":
            //    ValueSignature = new fhirCsR4B.Models.Signature();
            //    ValueSignature.DeserializeJson(ref reader, options);
            //    break;

            //case "valueTiming":
            //    ValueTiming = new fhirCsR4B.Models.Timing();
            //    ValueTiming.DeserializeJson(ref reader, options);
            //    break;

            //case "valueContactDetail":
            //    ValueContactDetail = new fhirCsR4B.Models.ContactDetail();
            //    ValueContactDetail.DeserializeJson(ref reader, options);
            //    break;

            //case "valueContributor":
            //    ValueContributor = new fhirCsR4B.Models.Contributor();
            //    ValueContributor.DeserializeJson(ref reader, options);
            //    break;

            //case "valueDataRequirement":
            //    ValueDataRequirement = new fhirCsR4B.Models.DataRequirement();
            //    ValueDataRequirement.DeserializeJson(ref reader, options);
            //    break;

            //case "valueExpression":
            //    ValueExpression = new fhirCsR4B.Models.Expression();
            //    ValueExpression.DeserializeJson(ref reader, options);
            //    break;

            //case "valueParameterDefinition":
            //    ValueParameterDefinition = new fhirCsR4B.Models.ParameterDefinition();
            //    ValueParameterDefinition.DeserializeJson(ref reader, options);
            //    break;

            //case "valueRelatedArtifact":
            //    ValueRelatedArtifact = new fhirCsR4B.Models.RelatedArtifact();
            //    ValueRelatedArtifact.DeserializeJson(ref reader, options);
            //    break;

            //case "valueTriggerDefinition":
            //    ValueTriggerDefinition = new fhirCsR4B.Models.TriggerDefinition();
            //    ValueTriggerDefinition.DeserializeJson(ref reader, options);
            //    break;

            //case "valueUsageContext":
            //    ValueUsageContext = new fhirCsR4B.Models.UsageContext();
            //    ValueUsageContext.DeserializeJson(ref reader, options);
            //    break;

            //case "valueDosage":
            //    ValueDosage = new fhirCsR4B.Models.Dosage();
            //    ValueDosage.DeserializeJson(ref reader, options);
            //    break;

            //case "valueMeta":
            //    ValueMeta = new fhirCsR4B.Models.Meta();
            //    ValueMeta.DeserializeJson(ref reader, options);
            //    break;

            default:
                ElementReader.Instance.ReadProperty(ref reader, options, o, propertyName);
                break;
        }
    }
}
