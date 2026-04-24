extern alias coreR3;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace Fhir.CodeGen.CrossVersionExporter;

public class ValueSetToR3
{
    public string ToJson(ValueSet vs5, SerializerSettings? settings = null)
    {
        // TODO: contained resources are not handled

        coreR3::Hl7.Fhir.Model.ValueSet vs3 = new()
        {
            Id = vs5.Id,
            Meta = vs5.Meta is null
                ? null
                : new()
                {
                    VersionId = vs5.Meta.VersionId,
                    LastUpdated = vs5.Meta.LastUpdated,
                    Source = vs5.Meta.Source,
                    ProfileElement = vs5.Meta.ProfileElement.DeepCopy().ToList(),
                    Security = vs5.Meta.Security.DeepCopy().ToList(),
                    Tag = vs5.Meta.Tag.DeepCopy().ToList(),
                    Extension = vs5.Meta.Extension.DeepCopy().ToList(),
                },
            ImplicitRules = vs5.ImplicitRules,
            Language = vs5.Language,
            Text = vs5.Text,
            Extension = vs5.Extension.DeepCopy().ToList(),
            ModifierExtension = vs5.ModifierExtension.DeepCopy().ToList(),
            Url = vs5.Url,
            Identifier = vs5.Identifier.DeepCopy().ToList(),
            Version = vs5.Version,
            Name = vs5.Name,
            Title = vs5.Title,
            Status = vs5.Status,
            Experimental = vs5.Experimental,
            Date = vs5.Date,
            Publisher = vs5.Publisher,
            Contact = vs5.Contact.DeepCopy().ToList(),
            Description = vs5.Description,
            UseContext = vs5.UseContext.DeepCopy().ToList(),
            Jurisdiction = vs5.Jurisdiction.DeepCopy().ToList(),
            Immutable = vs5.Immutable,
            Purpose = vs5.Purpose,
            Copyright = vs5.Copyright,
            Compose = r5ComposeToR3(vs5.Compose),
            Expansion = vs5.Expansion is null
                ? null
                : new()
                {
                    ElementId = vs5.Expansion.ElementId,
                    Extension = vs5.Expansion.Extension,
                    Identifier = vs5.Expansion.Identifier,
                    Timestamp = vs5.Expansion.Timestamp,
                    Total = vs5.Expansion.Total,
                    Offset = vs5.Expansion.Offset,
                    Parameter = vs5.Expansion.Parameter.Select(p5 => new coreR3::Hl7.Fhir.Model.ValueSet.ParameterComponent()
                    {
                        ElementId = p5.ElementId,
                        Extension = p5.Extension,
                        Name = p5.Name,
                        Value = p5.Value,
                    }).ToList(),
                    Contains = vs5.Expansion.Contains.Select(c5 => r5ContainsToR3(c5)).ToList(),
                },
        };

        var s = new coreR3::Hl7.Fhir.Serialization.FhirJsonSerializer(settings);
        return s.SerializeToString(vs3);
    }

    private coreR3::Hl7.Fhir.Model.ValueSet.ContainsComponent r5ContainsToR3(ValueSet.ContainsComponent c5)
    {
        coreR3::Hl7.Fhir.Model.ValueSet.ContainsComponent c3 = new()
        {
            ElementId = c5.ElementId,
            Extension = c5.Extension,
            System = c5.System,
            Abstract = c5.Abstract,
            Inactive = c5.Inactive,
            Version = c5.Version,
            Code = c5.Code,
            Display = c5.Display,
            Designation = c5.Designation.Select(d5 => new coreR3::Hl7.Fhir.Model.ValueSet.DesignationComponent()
            {
                ElementId = d5.ElementId,
                Extension = d5.Extension,
                Language = d5.Language,
                Use = d5.Use,
                Value = d5.Value,
            }).ToList(),
            Contains = c5.Contains.Select(c5c => r5ContainsToR3(c5c)).ToList(),
        };
        return c3;
    }

    private coreR3::Hl7.Fhir.Model.ValueSet.ComposeComponent? r5ComposeToR3(ValueSet.ComposeComponent? c5)
    {
        if (c5 is null)
        {
            return null;
        }

        coreR3::Hl7.Fhir.Model.ValueSet.ComposeComponent c3 = new()
        {
            ElementId = c5.ElementId,
            Extension = c5.Extension,
            LockedDate = c5.LockedDate,
            Inactive = c5.Inactive,
            Include = c5.Include.Select(i5 => r5ConceptSetToR3(i5)).ToList(),
            Exclude = c5.Exclude.Select(e5 => r5ConceptSetToR3(e5)).ToList(),
        };

        return c3;
    }

    private coreR3::Hl7.Fhir.Model.ValueSet.ConceptSetComponent r5ConceptSetToR3(ValueSet.ConceptSetComponent c5)
    {
        coreR3::Hl7.Fhir.Model.ValueSet.ConceptSetComponent c3 = new()
        {
            ElementId = c5.ElementId,
            Extension = c5.Extension,
            System = c5.System,
            Version = c5.Version,
            Concept = c5.Concept.Select(concept5 => new coreR3::Hl7.Fhir.Model.ValueSet.ConceptReferenceComponent()
            {
                ElementId = concept5.ElementId,
                Extension = concept5.Extension,
                Code = concept5.Code,
                Display = concept5.Display,
                Designation = concept5.Designation.Select(d5 => new coreR3::Hl7.Fhir.Model.ValueSet.DesignationComponent()
                {
                    ElementId = d5.ElementId,
                    Extension = d5.Extension,
                    Language = d5.Language,
                    Use = d5.Use,
                    Value = d5.Value,
                }).ToList(),
            }).ToList(),
            Filter = c5.Filter.Select(f5 => new coreR3::Hl7.Fhir.Model.ValueSet.FilterComponent()
            {
                ElementId = f5.ElementId,
                Extension = f5.Extension,
                Property = f5.Property,
                Op = f5.Op,
                Value = f5.Value,
            }).ToList(),
            ValueSet = c5.ValueSet.Select(vs5 => vs5).ToList(),
        };

        return c3;
    }
}
