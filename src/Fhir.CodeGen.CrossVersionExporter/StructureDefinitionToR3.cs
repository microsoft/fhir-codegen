extern alias coreR3;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Utility;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Fhir.CodeGen.CrossVersionExporter;

public class StructureDefinitionToR3
{
    public string ToJson(StructureDefinition sd5, SerializerSettings? settings = null)
    {
        // TODO: contained resources are not handled

        coreR3::Hl7.Fhir.Model.StructureDefinition sd3 = new()
        {
            Id = sd5.Id,
            Meta = sd5.Meta is null
                ? null
                : new()
                {
                    VersionId = sd5.Meta.VersionId,
                    LastUpdated = sd5.Meta.LastUpdated,
                    Source = sd5.Meta.Source,
                    ProfileElement = sd5.Meta.ProfileElement.DeepCopy().ToList(),
                    Security = sd5.Meta.Security.DeepCopy().ToList(),
                    Tag = sd5.Meta.Tag.DeepCopy().ToList(),
                    Extension = sd5.Meta.Extension.DeepCopy().ToList(),
                },
            ImplicitRules = sd5.ImplicitRules,
            Language = sd5.Language,
            Text = sd5.Text,
            Extension = sd5.Extension.DeepCopy().ToList(),
            ModifierExtension = sd5.ModifierExtension.DeepCopy().ToList(),
            Url = sd5.Url,
            Identifier = sd5.Identifier.Count > 0 ? sd5.Identifier.DeepCopy().ToList() : null,
            Version = sd5.Version,
            Name = sd5.Name,
            Title = sd5.Title,
            Status = sd5.Status,
            Experimental = sd5.Experimental,
            Date = sd5.Date,
            Publisher = sd5.Publisher,
            Contact = sd5.Contact.DeepCopy().ToList(),
            Description = sd5.Description,
            UseContext = sd5.UseContext.DeepCopy().ToList(),
            Jurisdiction = sd5.Jurisdiction.DeepCopy().ToList(),
            Purpose = sd5.Purpose,
            Copyright = sd5.Copyright,
            Keyword = sd5.Keyword.DeepCopy().ToList(),
            FhirVersion = sd5.FhirVersion is null ? null : EnumUtility.GetLiteral(sd5.FhirVersion),
            Mapping = r5MappingToR3(sd5.Mapping),
            Kind = r5KindToR3(sd5.Kind),
            Abstract = sd5.Abstract,
            ContextInvariant = sd5.ContextInvariant,
            Type = sd5.Type,
            BaseDefinition = sd5.BaseDefinition,
            Derivation = r5DerivationToR3(sd5.Derivation),
        };

        // process contexts
        foreach (StructureDefinition.ContextComponent c5 in sd5.Context)
        {
            switch (c5.Type)
            {
                case StructureDefinition.ExtensionContextType.Fhirpath:
                    // skip - cannot represent
                    break;

                case StructureDefinition.ExtensionContextType.Element:
                    sd3.ContextType = coreR3::Hl7.Fhir.Model.StructureDefinition.ExtensionContext.Resource;
                    sd3.ContextElement.Add(new FhirString(c5.Expression));
                    break;

                case StructureDefinition.ExtensionContextType.Extension:
                    sd3.ContextType = coreR3::Hl7.Fhir.Model.StructureDefinition.ExtensionContext.Extension;
                    sd3.ContextElement.Add(new FhirString(c5.Expression));
                    break;
            }
        }

        // process snapshot
        if (sd5.Snapshot?.Element.Count > 0)
        {
            sd3.Snapshot = new();
            sd3.Snapshot.Element = r5ElementsToR3(sd5.Snapshot.Element);
        }

        // process differential
        if (sd5.Differential?.Element.Count > 0)
        {
            sd3.Differential = new();
            sd3.Differential.Element = r5ElementsToR3(sd5.Differential.Element);
        }

        var s = new coreR3::Hl7.Fhir.Serialization.FhirJsonSerializer(settings);
        return s.SerializeToString(sd3);
    }

    private List<coreR3::Hl7.Fhir.Model.ElementDefinition> r5ElementsToR3(List<ElementDefinition> el5)
    {
        List<coreR3::Hl7.Fhir.Model.ElementDefinition> el3 = [];

        foreach (ElementDefinition e5 in el5)
        {
            el3.Add(r5ElementToR3(e5));
        }

        return el3;
    }

    private coreR3::Hl7.Fhir.Model.ElementDefinition r5ElementToR3(ElementDefinition e5)
    {
        // TODO: additional bindings are ignored

        coreR3::Hl7.Fhir.Model.ElementDefinition e3 = new()
        {
            ElementId = e5.ElementId,
            Extension = e5.Extension.DeepCopy().ToList(),
            Path = e5.Path,
            Representation = e5.Representation.Select(r => r5RepresentationToR3(r)),
            SliceName = e5.SliceName,
            Label = e5.Label,
            Code = e5.Code.DeepCopy().ToList(),
            Slicing = e5.Slicing is null ? null : new coreR3::Hl7.Fhir.Model.ElementDefinition.SlicingComponent()
            {
                Discriminator = e5.Slicing.Discriminator.Select(d5 => new coreR3::Hl7.Fhir.Model.ElementDefinition.DiscriminatorComponent()
                {
                    ElementId = d5.ElementId,
                    Extension = d5.Extension.DeepCopy().ToList(),
                    Type = r5DiscriminatorTypeToR3(d5.Type),
                    Path = d5.Path,
                }).ToList(),
                Description = e5.Slicing.Description,
                Ordered = e5.Slicing.Ordered,
                Rules = r5SlicingRulesToR3(e5.Slicing.Rules),
            },
            Short = e5.Short,
            Definition = e5.Definition,
            Comment = e5.Comment,
            Requirements = e5.Requirements,
            Alias = e5.Alias,
            Min = e5.Min,
            Max = e5.Max,
            Base = e5.Base is null ? null : new coreR3::Hl7.Fhir.Model.ElementDefinition.BaseComponent()
            {
                ElementId = e5.Base.ElementId,
                Extension = e5.Base.Extension.DeepCopy().ToList(),
                Path = e5.Base.Path,
                Min = e5.Base.Min,
                Max = e5.Base.Max,
            },
            ContentReference = e5.ContentReference,
            Type = r5EdTypeToR3(e5.Type),
            DefaultValue = e5.DefaultValue,
            MeaningWhenMissing = e5.MeaningWhenMissing,
            OrderMeaning = e5.OrderMeaning,
            Fixed = e5.Fixed,
            Pattern = e5.Pattern,
            Example = e5.Example.Select(ex5 => new coreR3::Hl7.Fhir.Model.ElementDefinition.ExampleComponent()
            {
                ElementId = ex5.ElementId,
                Extension = ex5.Extension.DeepCopy().ToList(),
                Label = ex5.Label,
                Value = ex5.Value,
            }).ToList(),
            MinValue = e5.MinValue,
            MaxValue = e5.MaxValue,
            MaxLength = e5.MaxLength,
            Condition = e5.Condition,
            Constraint = e5.Constraint.Select(c5 => new coreR3::Hl7.Fhir.Model.ElementDefinition.ConstraintComponent()
            {
                ElementId = c5.ElementId,
                Extension = c5.Extension.DeepCopy().ToList(),
                Key = c5.Key,
                Requirements = c5.Requirements,
                Severity = r5ConstraintSeverityToR3(c5.Severity),
                Human = c5.Human,
                Expression = c5.Expression,
                Source = c5.Source,
            }).ToList(),
            MustSupport = e5.MustSupport,
            IsModifier = e5.IsModifier,
            IsSummary = e5.IsSummary,
            Binding = e5.Binding is null ? null : new coreR3::Hl7.Fhir.Model.ElementDefinition.ElementDefinitionBindingComponent()
            {
                ElementId = e5.Binding.ElementId,
                Extension = e5.Binding.Extension.DeepCopy().ToList(),
                Strength = r5BindingStrengthToR3(e5.Binding.Strength),
                Description = e5.Binding.Description,
                ValueSet = e5.Binding.ValueSet is null ? null! : new FhirUri(e5.Binding.ValueSet),
            },
            Mapping = e5.Mapping.Select(m5 => new coreR3::Hl7.Fhir.Model.ElementDefinition.MappingComponent()
            {
                ElementId = m5.ElementId,
                Extension = m5.Extension.DeepCopy().ToList(),
                Identity = m5.Identity,
                Language = m5.Language,
                Map = m5.Map,
                Comment = m5.Comment,
            }).ToList(),
        };

        // add modifier extensions
        if (e5.ModifierExtension.Count > 0)
        {
            e3.Extension.AddRange(e5.ModifierExtension.DeepCopy());
        }

        return e3;
    }

    private coreR3::Hl7.Fhir.Model.BindingStrength? r5BindingStrengthToR3(BindingStrength? s) => s switch
    {
        BindingStrength.Required => coreR3::Hl7.Fhir.Model.BindingStrength.Required,
        BindingStrength.Extensible => coreR3::Hl7.Fhir.Model.BindingStrength.Extensible,
        BindingStrength.Preferred => coreR3::Hl7.Fhir.Model.BindingStrength.Preferred,
        BindingStrength.Example => coreR3::Hl7.Fhir.Model.BindingStrength.Example,
        _ => null,
    };

    private coreR3::Hl7.Fhir.Model.ElementDefinition.ConstraintSeverity? r5ConstraintSeverityToR3(ConstraintSeverity? c5) => c5 switch
    {
        ConstraintSeverity.Error => coreR3::Hl7.Fhir.Model.ElementDefinition.ConstraintSeverity.Error,
        ConstraintSeverity.Warning => coreR3::Hl7.Fhir.Model.ElementDefinition.ConstraintSeverity.Warning,
        _ => null,
    };

    private List<coreR3::Hl7.Fhir.Model.ElementDefinition.TypeRefComponent> r5EdTypeToR3(List<ElementDefinition.TypeRefComponent> trl5)
    {
        List<coreR3::Hl7.Fhir.Model.ElementDefinition.TypeRefComponent> trl3 = [];

        foreach (ElementDefinition.TypeRefComponent tr5 in trl5)
        {
            if (tr5.ProfileElement.Count > 1)
            {
                foreach (Canonical profile in tr5.ProfileElement)
                {
                    coreR3::Hl7.Fhir.Model.ElementDefinition.TypeRefComponent tr3 = new()
                    {
                        ElementId = tr5.ElementId,
                        Extension = tr5.Extension.DeepCopy().ToList(),
                        Code = tr5.Code,
                        Profile = profile,
                        Aggregation = tr5.Aggregation.Select(a => r5AggregationModeToR3(a)),
                        Versioning = r5ReferenceVersionRulesToR3(tr5.Versioning),
                    };

                    trl3.Add(tr3);
                }
            }
            else if (tr5.TargetProfileElement.Count > 1)
            {
                foreach (Canonical target in tr5.TargetProfileElement)
                {
                    coreR3::Hl7.Fhir.Model.ElementDefinition.TypeRefComponent tr3 = new()
                    {
                        ElementId = tr5.ElementId,
                        Extension = tr5.Extension.DeepCopy().ToList(),
                        Code = tr5.Code,
                        TargetProfile = target,
                        Aggregation = tr5.Aggregation.Select(a => r5AggregationModeToR3(a)),
                        Versioning = r5ReferenceVersionRulesToR3(tr5.Versioning),
                    };

                    trl3.Add(tr3);
                }
            }
            else
            {
                coreR3::Hl7.Fhir.Model.ElementDefinition.TypeRefComponent tr3 = new()
                {
                    ElementId = tr5.ElementId,
                    Extension = tr5.Extension.DeepCopy().ToList(),
                    Code = tr5.Code,
                    ProfileElement = tr5.ProfileElement.Count > 0 ? new FhirUri(tr5.ProfileElement[0]) : null,
                    TargetProfileElement = tr5.TargetProfileElement.Count > 0 ? new FhirUri(tr5.TargetProfileElement[0]) : null,
                    Aggregation = tr5.Aggregation.Select(a => r5AggregationModeToR3(a)),
                    Versioning = r5ReferenceVersionRulesToR3(tr5.Versioning),
                };

                trl3.Add(tr3);
            }
        }

        return trl3;
    }

    private coreR3::Hl7.Fhir.Model.ElementDefinition.ReferenceVersionRules? r5ReferenceVersionRulesToR3(ElementDefinition.ReferenceVersionRules? r) => r switch
    {
        ElementDefinition.ReferenceVersionRules.Either => coreR3::Hl7.Fhir.Model.ElementDefinition.ReferenceVersionRules.Either,
        ElementDefinition.ReferenceVersionRules.Independent => coreR3::Hl7.Fhir.Model.ElementDefinition.ReferenceVersionRules.Independent,
        ElementDefinition.ReferenceVersionRules.Specific => coreR3::Hl7.Fhir.Model.ElementDefinition.ReferenceVersionRules.Specific,
        _ => null,
    };

    private coreR3::Hl7.Fhir.Model.ElementDefinition.AggregationMode? r5AggregationModeToR3(ElementDefinition.AggregationMode? a) => a switch
    {
        ElementDefinition.AggregationMode.Contained => coreR3::Hl7.Fhir.Model.ElementDefinition.AggregationMode.Contained,
        ElementDefinition.AggregationMode.Referenced => coreR3::Hl7.Fhir.Model.ElementDefinition.AggregationMode.Referenced,
        ElementDefinition.AggregationMode.Bundled => coreR3::Hl7.Fhir.Model.ElementDefinition.AggregationMode.Bundled,
        _ => null,
    };

    private coreR3::Hl7.Fhir.Model.ElementDefinition.SlicingRules? r5SlicingRulesToR3(ElementDefinition.SlicingRules? r) => r switch
    {
        ElementDefinition.SlicingRules.Closed => coreR3::Hl7.Fhir.Model.ElementDefinition.SlicingRules.Closed,
        ElementDefinition.SlicingRules.Open => coreR3::Hl7.Fhir.Model.ElementDefinition.SlicingRules.Open,
        ElementDefinition.SlicingRules.OpenAtEnd => coreR3::Hl7.Fhir.Model.ElementDefinition.SlicingRules.OpenAtEnd,
        _ => null,
    };

    private coreR3::Hl7.Fhir.Model.ElementDefinition.DiscriminatorType? r5DiscriminatorTypeToR3(ElementDefinition.DiscriminatorType? t) => t switch
    {
        ElementDefinition.DiscriminatorType.Value => coreR3::Hl7.Fhir.Model.ElementDefinition.DiscriminatorType.Value,
        ElementDefinition.DiscriminatorType.Exists => coreR3::Hl7.Fhir.Model.ElementDefinition.DiscriminatorType.Exists,
        ElementDefinition.DiscriminatorType.Pattern => coreR3::Hl7.Fhir.Model.ElementDefinition.DiscriminatorType.Pattern,
        ElementDefinition.DiscriminatorType.Type => coreR3::Hl7.Fhir.Model.ElementDefinition.DiscriminatorType.Type,
        ElementDefinition.DiscriminatorType.Profile => coreR3::Hl7.Fhir.Model.ElementDefinition.DiscriminatorType.Profile,
        ElementDefinition.DiscriminatorType.Position => null,
        _ => null,
    };

    private coreR3::Hl7.Fhir.Model.ElementDefinition.PropertyRepresentation? r5RepresentationToR3(ElementDefinition.PropertyRepresentation? r) => r switch
    {
        ElementDefinition.PropertyRepresentation.XmlAttr => coreR3::Hl7.Fhir.Model.ElementDefinition.PropertyRepresentation.XmlAttr,
        ElementDefinition.PropertyRepresentation.XmlText => coreR3::Hl7.Fhir.Model.ElementDefinition.PropertyRepresentation.XmlText,
        ElementDefinition.PropertyRepresentation.TypeAttr => coreR3::Hl7.Fhir.Model.ElementDefinition.PropertyRepresentation.TypeAttr,
        ElementDefinition.PropertyRepresentation.CdaText => coreR3::Hl7.Fhir.Model.ElementDefinition.PropertyRepresentation.CdaText,
        ElementDefinition.PropertyRepresentation.Xhtml => coreR3::Hl7.Fhir.Model.ElementDefinition.PropertyRepresentation.Xhtml,
        _ => null,
    };

    private coreR3::Hl7.Fhir.Model.StructureDefinition.TypeDerivationRule? r5DerivationToR3(StructureDefinition.TypeDerivationRule? d) => d switch
    {
        StructureDefinition.TypeDerivationRule.Specialization => coreR3::Hl7.Fhir.Model.StructureDefinition.TypeDerivationRule.Specialization,
        StructureDefinition.TypeDerivationRule.Constraint => coreR3::Hl7.Fhir.Model.StructureDefinition.TypeDerivationRule.Constraint,
        _ => null,
    };

    private coreR3::Hl7.Fhir.Model.StructureDefinition.StructureDefinitionKind? r5KindToR3(StructureDefinition.StructureDefinitionKind? k) => k switch
    {
        StructureDefinition.StructureDefinitionKind.PrimitiveType => coreR3::Hl7.Fhir.Model.StructureDefinition.StructureDefinitionKind.PrimitiveType,
        StructureDefinition.StructureDefinitionKind.ComplexType => coreR3::Hl7.Fhir.Model.StructureDefinition.StructureDefinitionKind.ComplexType,
        StructureDefinition.StructureDefinitionKind.Resource => coreR3::Hl7.Fhir.Model.StructureDefinition.StructureDefinitionKind.Resource,
        StructureDefinition.StructureDefinitionKind.Logical => coreR3::Hl7.Fhir.Model.StructureDefinition.StructureDefinitionKind.Logical,
        _ => null,
    };

    private List<coreR3::Hl7.Fhir.Model.StructureDefinition.MappingComponent> r5MappingToR3(List<StructureDefinition.MappingComponent> ml5)
    {
        List<coreR3::Hl7.Fhir.Model.StructureDefinition.MappingComponent> ml3 = [];

        foreach (StructureDefinition.MappingComponent m5 in ml5)
        {
            coreR3::Hl7.Fhir.Model.StructureDefinition.MappingComponent m3 = new()
            {
                ElementId = m5.ElementId,
                Extension = m5.Extension.DeepCopy().ToList(),
                ModifierExtension = m5.ModifierExtension.DeepCopy().ToList(),
                Identity = m5.Identity,
                Uri = m5.Uri,
                Name = m5.Name,
                Comment = m5.Comment,
            };

            ml3.Add(m3);
        }

        return ml3;
    }
}
