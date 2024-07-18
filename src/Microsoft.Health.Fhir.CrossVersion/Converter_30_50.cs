// <copyright file="Converter_30_50.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Microsoft.Health.Fhir.CrossVersion.Convert_30_50;

namespace Microsoft.Health.Fhir.CrossVersion;

public class Converter_30_50
{
    internal PrimitiveExtractor _primitive;
    internal Address_30_50 _address;
    internal Age_30_50 _age;
    internal Annotation_30_50 _annotation;
    internal Attachment_30_50 _attachment;
    internal BackboneElement_30_50 _backboneElement;
    internal CapabilityStatement_30_50 _capabilityStatement;
    internal CodeableConcept_30_50 _codeableConcept;
    internal CodeSystem_30_50 _codeSystem;
    internal Coding_30_50 _coding;
    internal CompartmentDefinition_30_50 _compartmentDefinition;
    internal ConceptMap_30_50 _conceptMap;
    internal ContactDetail_30_50 _contactDetail;
    internal ContactPoint_30_50 _contactPoint;
    internal Contributor_30_50 _contributor;
    internal Count_30_50 _count;
    internal DataRequirement_30_50 _dataRequirement;
    internal Distance_30_50 _distance;
    internal DomainResource_30_50 _domainResource;
    internal Dosage_30_50 _dosage;
    internal Duration_30_50 _duration;
    internal Element_30_50 _element;
    internal ElementDefinition_30_50 _elementDefinition;
    internal Extension_30_50 _extension;
    internal HumanName_30_50 _humanName;
    internal Identifier_30_50 _identifier;
    internal ImplementationGuide_30_50 _implementationGuide;
    internal Meta_30_50 _meta;
    internal Money_30_50 _money;
    internal Narrative_30_50 _narrative;
    internal OperationDefinition_30_50 _operationDefinition;
    internal ParameterDefinition_30_50 _parameterDefinition;
    internal Period_30_50 _period;
    internal Quantity_30_50 _quantity;
    internal Range_30_50 _range;
    internal Ratio_30_50 _ratio;
    internal Reference_30_50 _reference;
    internal RelatedArtifact_30_50 _relatedArtifact;
    internal Resource_30_50 _resource;
    internal SampledData_30_50 _sampledData;
    internal SearchParameter_30_50 _searchParameter;
    internal Signature_30_50 _signature;
    internal StructureDefinition_30_50 _structureDefinition;
    internal Timing_30_50 _timing;
    internal TriggerDefinition_30_50 _triggerDefinition;
    internal UsageContext_30_50 _usageContext;
    internal ValueSet_30_50 _valueSet;

    public Converter_30_50()
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
    public Hl7.Fhir.Model.Resource Convert(ISourceNode r3)
    {
        return _resource.Extract(r3);
    }
}
