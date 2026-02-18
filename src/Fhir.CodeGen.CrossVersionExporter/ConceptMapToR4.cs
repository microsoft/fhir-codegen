extern alias coreR4;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Utility;
using static Fhir.CodeGen.CrossVersionExporter.Utilities;

namespace Fhir.CodeGen.CrossVersionExporter;

public class ConceptMapToR4
{
    public string ToJson(ConceptMap cm5, SerializerSettings? settings = null)
    {
        // TODO: contained resources are not handled

        coreR4::Hl7.Fhir.Model.ConceptMap cm4 = new()
        {
            Id = cm5.Id,
            Meta = cm5.Meta is null 
                ? null
                : new()
                {
                    VersionId = cm5.Meta.VersionId,
                    LastUpdated = cm5.Meta.LastUpdated,
                    Source = cm5.Meta.Source,
                    ProfileElement = cm5.Meta.ProfileElement.DeepCopy().ToList(),
                    Security = cm5.Meta.Security.DeepCopy().ToList(),
                    Tag = cm5.Meta.Tag.DeepCopy().ToList(),
                    Extension = cm5.Meta.Extension.DeepCopy().ToList(),
                },
            ImplicitRules = cm5.ImplicitRules,
            Language = cm5.Language,
            Text = cm5.Text,
            Extension = cm5.Extension.DeepCopy().ToList(),
            ModifierExtension = cm5.ModifierExtension.DeepCopy().ToList(),
            Url = cm5.Url,
            Identifier = cm5.Identifier.Count > 0 ? (Identifier)cm5.Identifier[0].DeepCopy() : null,
            Version = cm5.Version,
            Name = cm5.Name,
            Title = cm5.Title,
            Status = cm5.Status,
            Experimental = cm5.Experimental,
            Date = cm5.Date,
            Publisher = cm5.Publisher,
            Contact = cm5.Contact.DeepCopy().ToList(),
            Description = cm5.Description,
            UseContext = cm5.UseContext.DeepCopy().ToList(),
            Jurisdiction = cm5.Jurisdiction.DeepCopy().ToList(),
            Purpose = cm5.Purpose,
            Copyright = cm5.Copyright,
            Source = cm5.SourceScope,
            Target = cm5.TargetScope,
        };

        foreach (ConceptMap.GroupComponent g5 in cm5.Group)
        {
            coreR4::Hl7.Fhir.Model.ConceptMap.GroupComponent g4 = new()
            {
                Source = g5.SourceElement?.Uri,
                SourceVersion = g5.SourceElement?.Version,
                Target = g5.TargetElement?.Uri,
                TargetVersion = g5.TargetElement?.Version,
            };

            cm4.Group.Add(g4);

            foreach (ConceptMap.SourceElementComponent s5 in g5.Element)
            {
                coreR4::Hl7.Fhir.Model.ConceptMap.SourceElementComponent s4 = new()
                {
                    Code = s5.Code,
                    Display = s5.Display,
                };
                g4.Element.Add(s4);

                if (s5.NoMap == true)
                {
                    s4.Target.Add(new()
                    {
                        Equivalence = coreR4::Hl7.Fhir.Model.ConceptMapEquivalence.Unmatched,
                    });
                }

                foreach (ConceptMap.TargetElementComponent? t5 in s5.Target)
                {
                    coreR4::Hl7.Fhir.Model.ConceptMap.TargetElementComponent t4 = new()
                    {
                        Code = t5.Code,
                        Display = t5.Display,
                        Equivalence = r5RelationshipToR4Equivalence(t5.Relationship!.Value),
                        Comment = t5.Comment,
                    };
                    s4.Target.Add(t4);

                    // TODO: property, dependsOn, and product are skipped for now
                }
            }

            if (g5.Unmapped is not null)
            {
                coreR4::Hl7.Fhir.Model.ConceptMap.UnmappedComponent u4 = new()
                {
                    Mode = r5UnmappedModeToR4(g5.Unmapped.Mode!.Value),
                    Code = g5.Unmapped.Code,
                    Display = g5.Unmapped.Display,
                    Url = g5.Unmapped.OtherMap,
                };
            }
        }

        var s = new coreR4::Hl7.Fhir.Serialization.FhirJsonSerializer(settings);
        return s.SerializeToString(cm4);
    }

    private coreR4::Hl7.Fhir.Model.ConceptMap.ConceptMapGroupUnmappedMode r5UnmappedModeToR4(ConceptMap.ConceptMapGroupUnmappedMode m) => m switch
    {
        ConceptMap.ConceptMapGroupUnmappedMode.UseSourceCode => coreR4::Hl7.Fhir.Model.ConceptMap.ConceptMapGroupUnmappedMode.Provided,
        ConceptMap.ConceptMapGroupUnmappedMode.Fixed => coreR4::Hl7.Fhir.Model.ConceptMap.ConceptMapGroupUnmappedMode.Fixed,
        ConceptMap.ConceptMapGroupUnmappedMode.OtherMap => coreR4::Hl7.Fhir.Model.ConceptMap.ConceptMapGroupUnmappedMode.OtherMap,
        _ => coreR4::Hl7.Fhir.Model.ConceptMap.ConceptMapGroupUnmappedMode.OtherMap,
    };

    private coreR4::Hl7.Fhir.Model.ConceptMapEquivalence r5RelationshipToR4Equivalence(ConceptMap.ConceptMapRelationship r) => r switch
    {
        ConceptMap.ConceptMapRelationship.RelatedTo => coreR4::Hl7.Fhir.Model.ConceptMapEquivalence.Relatedto,
        ConceptMap.ConceptMapRelationship.Equivalent => coreR4::Hl7.Fhir.Model.ConceptMapEquivalence.Equivalent,
        ConceptMap.ConceptMapRelationship.SourceIsNarrowerThanTarget => coreR4::Hl7.Fhir.Model.ConceptMapEquivalence.Narrower,
        ConceptMap.ConceptMapRelationship.SourceIsBroaderThanTarget => coreR4::Hl7.Fhir.Model.ConceptMapEquivalence.Wider,
        ConceptMap.ConceptMapRelationship.NotRelatedTo => coreR4::Hl7.Fhir.Model.ConceptMapEquivalence.Disjoint,
        _ => coreR4::Hl7.Fhir.Model.ConceptMapEquivalence.Unmatched,
    };
}
