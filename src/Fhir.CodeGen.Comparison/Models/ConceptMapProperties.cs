using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;

namespace Fhir.CodeGen.Comparison.Models;

public static class ConceptMapProperties
{
    public static string UrlValueSetConceptMapRelationship = "http://hl7.org/fhir/ValueSet/concept-map-relationship";
    public static string UrlCodeSystemConceptMapProperties = "http://hl7.org/fhir/uv/xver/CodeSystem/ConceptMapProperties";

    public static string PropertyCodeConceptDomainRelationship = "concept-domain-relationship";
    public static string PropertyCodeValueDomainRelationship = "value-domain-relationship";
    //public static string PropertyCodeInputMapUrl = "input-map-url";
    //public static string PropertyCodeIdR2 = "id-r2";
    //public static string PropertyCodeIdR3 = "id-r3";
    //public static string PropertyCodeIdR4 = "id-r4";
    //public static string PropertyCodeIdR4B = "id-r4b";
    //public static string PropertyCodeIdR5 = "id-r5";
    //public static string PropertyCodeIdR6 = "id-r6";


    public static CodeSystem CodeSystemConceptMapProperties => new()
    {
        Id = "ConceptMapProperties",
        Url = UrlCodeSystemConceptMapProperties,
        Name = "ConceptMapProperties",
        Title = "Concept Map Properties",
        Status = PublicationStatus.Active,
        Description = new Markdown("Cross-version-specific properties that can be used in ConceptMaps to describe comparitive relationships."),
        Content = CodeSystemContentMode.Complete,
        Concept = new List<CodeSystem.ConceptDefinitionComponent>()
        {
            new()
            {
                Code = new(PropertyCodeConceptDomainRelationship),
                Display = "Concept Domain Relationship",
                Definition = "The relationship of the conceptual domain between the mapping source and target."
            },
            new()
            {
                Code = new(PropertyCodeValueDomainRelationship),
                Display = "Value Domain Relationship",
                Definition = "The relationship of the value domain between the mapping source and target."
            },
            //new()
            //{
            //    Code = new(PropertyCodeInputMapUrl),
            //    Display = "FHIR Cross-Version Source Input URL (if different)",
            //    Definition = "The canonical URL of the source ConceptMap if it is different from the current ConceptMap."
            //},
            //new()
            //{
            //    Code = new(PropertyCodeIdR2),
            //    Display = "R2 Mapping Artifact Id",
            //    Definition = "The Id of the artifact in FHIR DSTU2 this mapping integrates."
            //},
            //new()
            //{
            //    Code = new(PropertyCodeIdR3),
            //    Display = "R3 Mapping Artifact Id",
            //    Definition = "The Id of the artifact in FHIR STU3 this mapping integrates."
            //},
            //new()
            //{
            //    Code = new(PropertyCodeIdR4),
            //    Display = "R4 Mapping Artifact Id",
            //    Definition = "The Id of the artifact in FHIR R4 this mapping integrates."
            //},
            //new()
            //{
            //    Code = new(PropertyCodeIdR4B),
            //    Display = "R4B Mapping Artifact Id",
            //    Definition = "The Id of the artifact in FHIR R4B this mapping integrates."
            //},
            //new()
            //{
            //    Code = new(PropertyCodeIdR5),
            //    Display = "R5 Mapping Artifact Id",
            //    Definition = "The Id of the artifact in FHIR R5 this mapping integrates."
            //},
            //new()
            //{
            //    Code = new(PropertyCodeIdR6),
            //    Display = "R6 Mapping Artifact Id",
            //    Definition = "The Id of the artifact in FHIR R6 this mapping integrates."
            //},
        },
    };

    public static ConceptMap.PropertyComponent PropConceptDomain => new()
    {
        Code = PropertyCodeConceptDomainRelationship,
        Uri = UrlCodeSystemConceptMapProperties,
        Description = "The relationship of the conceptual domain between the mapping source and target.",
        Type = ConceptMap.ConceptMapPropertyType.Code,
        System = UrlValueSetConceptMapRelationship,
    };

    public static ConceptMap.PropertyComponent PropValueDomain => new()
    {
        Code = PropertyCodeValueDomainRelationship,
        Uri = UrlCodeSystemConceptMapProperties,
        Description = "The relationship of the value domain between the mapping source and target.",
        Type = ConceptMap.ConceptMapPropertyType.Code,
        System = UrlValueSetConceptMapRelationship,
    };

    //public static ConceptMap.PropertyComponent PropInputMapUrl => new()
    //{
    //    Code = PropertyCodeInputMapUrl,
    //    Uri = UrlCodeSystemConceptMapProperties,
    //    Description = "The canonical URL of the source ConceptMap if it is different from the current ConceptMap.",
    //    Type = ConceptMap.ConceptMapPropertyType.String,
    //};

    //public static ConceptMap.PropertyComponent PropIdR2 => new()
    //{
    //    Code = PropertyCodeIdR2,
    //    Uri = UrlCodeSystemConceptMapProperties,
    //    Description = "The Id of the artifact in FHIR DSTU2 this mapping integrates.",
    //    Type = ConceptMap.ConceptMapPropertyType.String,
    //};

    //public static ConceptMap.PropertyComponent PropIdR3 => new()
    //{
    //    Code = PropertyCodeIdR3,
    //    Uri = UrlCodeSystemConceptMapProperties,
    //    Description = "The Id of the artifact in FHIR STU3 this mapping integrates.",
    //    Type = ConceptMap.ConceptMapPropertyType.String,
    //};

    //public static ConceptMap.PropertyComponent PropIdR4 => new()
    //{
    //    Code = PropertyCodeIdR4,
    //    Uri = UrlCodeSystemConceptMapProperties,
    //    Description = "The Id of the artifact in FHIR R4 this mapping integrates.",
    //    Type = ConceptMap.ConceptMapPropertyType.String,
    //};

    //public static ConceptMap.PropertyComponent PropIdR4B => new()
    //{
    //    Code = PropertyCodeIdR4B,
    //    Uri = UrlCodeSystemConceptMapProperties,
    //    Description = "The Id of the artifact in FHIR R4B this mapping integrates.",
    //    Type = ConceptMap.ConceptMapPropertyType.String,
    //};

    //public static ConceptMap.PropertyComponent PropIdR5 => new()
    //{
    //    Code = PropertyCodeIdR5,
    //    Uri = UrlCodeSystemConceptMapProperties,
    //    Description = "The Id of the artifact in FHIR R5 this mapping integrates.",
    //    Type = ConceptMap.ConceptMapPropertyType.String,
    //};

    //public static ConceptMap.PropertyComponent PropIdR6 => new()
    //{
    //    Code = PropertyCodeIdR6,
    //    Uri = UrlCodeSystemConceptMapProperties,
    //    Description = "The Id of the artifact in FHIR R6 this mapping integrates.",
    //    Type = ConceptMap.ConceptMapPropertyType.String,
    //};
}
