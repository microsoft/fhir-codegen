// <copyright file="Converter_43_50.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Microsoft.Health.Fhir.CrossVersion.Convert_43_50;

namespace Microsoft.Health.Fhir.CrossVersion;

public class Converter_43_50
{
    internal PrimitiveExtractor _primitive;
    internal Address_43_50 _address;
    internal Age_43_50 _age;
    internal Annotation_43_50 _annotation;
    internal Attachment_43_50 _attachment;
    internal BackboneElement_43_50 _backboneElement;
    internal CapabilityStatement_43_50 _capabilityStatement;
    internal CodeableConcept_43_50 _codeableConcept;
    internal CodeableReference_43_50 _codeableReference;
    internal CodeSystem_43_50 _codeSystem;
    internal Coding_43_50 _coding;
    internal CompartmentDefinition_43_50 _compartmentDefinition;
    internal ConceptMap_43_50 _conceptMap;
    internal ContactDetail_43_50 _contactDetail;
    internal ContactPoint_43_50 _contactPoint;
    internal Contributor_43_50 _contributor;
    internal Count_43_50 _count;
    internal DataRequirement_43_50 _dataRequirement;
    internal DataType_43_50 _dataType;
    internal Distance_43_50 _distance;
    internal DomainResource_43_50 _domainResource;
    internal Dosage_43_50 _dosage;
    internal Duration_43_50 _duration;
    internal Element_43_50 _element;
    internal ElementDefinition_43_50 _elementDefinition;
    internal Expression_43_50 _expression;
    internal Extension_43_50 _extension;
    internal HumanName_43_50 _humanName;
    internal Identifier_43_50 _identifier;
    internal ImplementationGuide_43_50 _implementationGuide;
    internal MarketingStatus_43_50 _marketingStatus;
    internal Meta_43_50 _meta;
    internal Money_43_50 _money;
    internal Narrative_43_50 _narrative;
    internal OperationDefinition_43_50 _operationDefinition;
    internal ParameterDefinition_43_50 _parameterDefinition;
    internal Period_43_50 _period;
    internal ProductShelfLife_43_50 _productShelfLife;
    internal Quantity_43_50 _quantity;
    internal Range_43_50 _range;
    internal Ratio_43_50 _ratio;
    internal RatioRange_43_50 _ratioRange;
    internal Reference_43_50 _reference;
    internal RelatedArtifact_43_50 _relatedArtifact;
    internal Resource_43_50 _resource;
    internal SampledData_43_50 _sampledData;
    internal SearchParameter_43_50 _searchParameter;
    internal Signature_43_50 _signature;
    internal StructureDefinition_43_50 _structureDefinition;
    internal Timing_43_50 _timing;
    internal TriggerDefinition_43_50 _triggerDefinition;
    internal UsageContext_43_50 _usageContext;
    internal ValueSet_43_50 _valueSet;

    public Converter_43_50()
    {
        _primitive = new();
        _address = new(this);
        _age = new(this);
        _annotation = new(this);
        _attachment = new(this);
        _backboneElement = new(this);
        _capabilityStatement = new(this);
        _codeableConcept = new(this);
        _codeableReference = new(this);
        _codeSystem = new(this);
        _coding = new(this);
        _compartmentDefinition = new(this);
        _conceptMap = new(this);
        _contactDetail = new(this);
        _contactPoint = new(this);
        _contributor = new(this);
        _count = new(this);
        _dataRequirement = new(this);
        _dataType = new(this);
        _distance = new(this);
        _domainResource = new(this);
        _dosage = new(this);
        _duration = new(this);
        _element = new(this);
        _elementDefinition = new(this);
        _expression = new(this);
        _extension = new(this);
        _humanName = new(this);
        _identifier = new(this);
        _implementationGuide = new(this);
        _marketingStatus = new(this);
        _meta = new(this);
        _money = new(this);
        _narrative = new(this);
        _operationDefinition = new(this);
        _parameterDefinition = new(this);
        _period = new(this);
        _productShelfLife = new(this);
        _quantity = new(this);
        _range = new(this);
        _ratio = new(this);
        _ratioRange = new(this);
        _reference = new(this);
        _relatedArtifact = new(this);
        _resource = new(this);
        _sampledData = new(this);
        _searchParameter = new(this);
        _signature = new(this);
        _structureDefinition = new(this);
        _timing = new(this);
        _triggerDefinition = new(this);
        _usageContext = new(this);
        _valueSet = new(this);
    }
    public Hl7.Fhir.Model.Resource Convert(ISourceNode r4b)
    {
        return _resource.Extract(r4b);
    }
}
