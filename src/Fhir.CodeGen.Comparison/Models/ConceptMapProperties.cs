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

}
