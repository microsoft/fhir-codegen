// <copyright file="Converter_20_50.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Microsoft.Health.Fhir.CrossVersion.Convert_20_50;

namespace Microsoft.Health.Fhir.CrossVersion;

public class Converter_20_50
{
    internal PrimitiveExtractor _primitive;
    internal Address_20_50 _address;
    internal Age_20_50 _age;
    internal Annotation_20_50 _annotation;
    internal Attachment_20_50 _attachment;
    internal BackboneElement_20_50 _backboneElement;
    internal CapabilityStatement_20_50 _capabilityStatement;
    internal CodeableConcept_20_50 _codeableConcept;
    internal CodeSystem_20_50 _codeSystem;
    internal Coding_20_50 _coding;
    internal CompartmentDefinition_20_50 _compartmentDefinition;
    internal ConceptMap_20_50 _conceptMap;
    internal ContactDetail_20_50 _contactDetail;
    internal ContactPoint_20_50 _contactPoint;
    internal Contributor_20_50 _contributor;
    internal Count_20_50 _count;
    internal DataRequirement_20_50 _dataRequirement;
    internal Distance_20_50 _distance;
    internal DomainResource_20_50 _domainResource;
    internal Dosage_20_50 _dosage;
    internal Duration_20_50 _duration;
    internal Element_20_50 _element;
    internal ElementDefinition_20_50 _elementDefinition;
    internal Extension_20_50 _extension;
    internal HumanName_20_50 _humanName;
    internal Identifier_20_50 _identifier;
    internal ImplementationGuide_20_50 _implementationGuide;
    internal Meta_20_50 _meta;
    internal Money_20_50 _money;
    internal Narrative_20_50 _narrative;
    internal OperationDefinition_20_50 _operationDefinition;
    internal ParameterDefinition_20_50 _parameterDefinition;
    internal Period_20_50 _period;
    internal Quantity_20_50 _quantity;
    internal Range_20_50 _range;
    internal Ratio_20_50 _ratio;
    internal Reference_20_50 _reference;
    internal RelatedArtifact_20_50 _relatedArtifact;
    internal Resource_20_50 _resource;
    internal SampledData_20_50 _sampledData;
    internal SearchParameter_20_50 _searchParameter;
    internal Signature_20_50 _signature;
    internal StructureDefinition_20_50 _structureDefinition;
    internal Timing_20_50 _timing;
    internal TriggerDefinition_20_50 _triggerDefinition;
    internal UsageContext_20_50 _usageContext;
    internal ValueSet_20_50 _valueSet;

    public Converter_20_50()
    {
        _primitive = new();
        _address = new(this);
        _age = new(this);
        _annotation = new(this);
        _attachment = new(this);
        _backboneElement = new(this);
        _capabilityStatement = new(this);
        _codeableConcept = new(this);
        _codeSystem = new(this);
        _coding = new(this);
        _compartmentDefinition = new(this);
        _conceptMap = new(this);
        _contactDetail = new(this);
        _contactPoint = new(this);
        _contributor = new(this);
        _count = new(this);
        _dataRequirement = new(this);
        _distance = new(this);
        _domainResource = new(this);
        _dosage = new(this);
        _duration = new(this);
        _element = new(this);
        _elementDefinition = new(this);
        _extension = new(this);
        _humanName = new(this);
        _identifier = new(this);
        _implementationGuide = new(this);
        _meta = new(this);
        _money = new(this);
        _narrative = new(this);
        _operationDefinition = new(this);
        _parameterDefinition = new(this);
        _period = new(this);
        _quantity = new(this);
        _range = new(this);
        _ratio = new(this);
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
