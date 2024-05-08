// <copyright file="FMLTests.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using FluentAssertions;
using Microsoft.Health.Fhir.MappingLanguage.Tests.Extensions;
using Xunit.Abstractions;

namespace Microsoft.Health.Fhir.MappingLanguage.Tests;

public class FMLTests
{
    [Theory]
    [FileData("data/Encounter4Bto5.fml")]
    internal void TestParseEncounter4Bto5(string content)
    {
        FhirMappingLanguage fml = new();

        bool success = fml.TryParse(content, out Hl7.Fhir.Model.StructureMap? sm);

        success.Should().BeTrue();
        if (!success)
        {
            return;
        }

        sm.Should().NotBeNull();
        if (sm is null)
        {
            return;
        }

        sm.Url.Should().Be("http://hl7.org/fhir/uv/xver/StructureMap/Encounter4Bto5");
        sm.Id.Should().Be("Encounter4Bto5");
        sm.Name.Should().Be("Encounter4Bto5");
        sm.Title.Should().Be("Encounter Transforms: R4B to R5");
        sm.Status.Should().Be(Hl7.Fhir.Model.PublicationStatus.Active);

        sm.Structure.Count.Should().Be(2);
        sm.Structure[0].Url.Should().Be("http://hl7.org/fhir/4.3/Encounter");
        sm.Structure[0].Alias.Should().Be("EncounterR4B");
        sm.Structure[0].Mode.Should().Be(Hl7.Fhir.Model.StructureMap.StructureMapModelMode.Source);
        sm.Structure[1].Url.Should().Be("http://hl7.org/fhir/5.0/Encounter");
        sm.Structure[1].Alias.Should().Be("EncounterR5");
        sm.Structure[1].Mode.Should().Be(Hl7.Fhir.Model.StructureMap.StructureMapModelMode.Target);

        sm.Import.Count().Should().Be(1);
        sm.Import.First().Should().Be("http://hl7.org/fhir/uv/xver/StructureMap/*4Bto5");

        sm.Group.Count.Should().Be(5);

        Hl7.Fhir.Model.StructureMap.GroupComponent group = sm.Group[0];
        group.Name.Should().Be("Encounter");
        group.Input.Count.Should().Be(2);
        group.Input[0].Mode.Should().Be(Hl7.Fhir.Model.StructureMap.StructureMapInputMode.Source);
        group.Input[0].Name.Should().Be("src");
        group.Input[0].Type.Should().Be("EncounterR4B");
        group.Input[1].Mode.Should().Be(Hl7.Fhir.Model.StructureMap.StructureMapInputMode.Target);
        group.Input[1].Name.Should().Be("tgt");
        group.Input[1].Type.Should().Be("EncounterR5");
        group.Extends.Should().Be("DomainResource");
        group.TypeMode.Should().Be(Hl7.Fhir.Model.StructureMap.StructureMapGroupTypeMode.TypeAndTypes);

        group.Rule.Count.Should().Be(19);

        Hl7.Fhir.Model.StructureMap.RuleComponent rule = group.Rule[0];
        rule.Name.Should().Be("identifier");
        rule.Source.Count.Should().Be(1);
        rule.Source[0].Context.Should().Be("src");
        rule.Source[0].Element.Should().Be("identifier");
        rule.Source[0].Variable.Should().Be("vvv");
        rule.Target.Count.Should().Be(1);
        rule.Target[0].Context.Should().Be("tgt");
        rule.Target[0].Element.Should().Be("identifier");
        rule.Target[0].Variable.Should().Be("vvv");
        rule.Target[0].Transform.Should().Be(Hl7.Fhir.Model.StructureMap.StructureMapTransform.Create);

        rule = group.Rule[1];
        rule.Name.Should().Be("status");
        rule.Source.Count.Should().Be(1);
        rule.Source[0].Context.Should().Be("src");
        rule.Source[0].Element.Should().Be("status");
        rule.Source[0].Variable.Should().Be("v");
        rule.Target.Count.Should().Be(1);
        rule.Target[0].Context.Should().Be("tgt");
        rule.Target[0].Element.Should().Be("status");
        rule.Target[0].Transform.Should().Be(Hl7.Fhir.Model.StructureMap.StructureMapTransform.Translate);
        rule.Target[0].Parameter.Count.Should().Be(3);
        rule.Target[0].Parameter[0].Value.Should().BeOfType<Hl7.Fhir.Model.Id>();
        rule.Target[0].Parameter[0].Value.ToString().Should().Be("v");
        rule.Target[0].Parameter[1].Value.Should().BeOfType<Hl7.Fhir.Model.FhirString>();
        rule.Target[0].Parameter[1].Value.ToString().Should().Be("http://hl7.org/fhir/uv/xver/ConceptMap/enc.status-4bto5");
        rule.Target[0].Parameter[2].Value.Should().BeOfType<Hl7.Fhir.Model.FhirString>();
        rule.Target[0].Parameter[2].Value.ToString().Should().Be("code");


        rule = group.Rule[2];
        rule.Name.Should().Be("class");
        rule.Source.Count.Should().Be(1);
        rule.Source[0].Context.Should().Be("src");
        rule.Source[0].Element.Should().Be("class");
        rule.Source[0].Variable.Should().Be("s");
        rule.Target.Count.Should().Be(2);
        rule.Target[0].Context.Should().Be("tgt");
        rule.Target[0].Element.Should().Be("class");
        rule.Source[0].Variable.Should().Be("s");
        rule.Target[0].Transform.Should().Be(Hl7.Fhir.Model.StructureMap.StructureMapTransform.Create);
        rule.Target[0].Parameter.Count.Should().Be(1);
        rule.Target[0].Parameter[0].Value.Should().BeOfType<Hl7.Fhir.Model.FhirString>();
        rule.Target[0].Parameter[0].Value.ToString().Should().Be("CodeableConcept");
        rule.Target[1].Context.Should().Be("t");
        rule.Target[1].Element.Should().Be("coding");
        rule.Target[1].Variable.Should().Be("tc");


        group = sm.Group[1];
        group.Name.Should().Be("EncounterParticipant");
        group.Input.Count.Should().Be(2);
        group.Input[0].Mode.Should().Be(Hl7.Fhir.Model.StructureMap.StructureMapInputMode.Source);
        group.Input[0].Name.Should().Be("src");
        group.Input[0].Type.Should().BeNullOrEmpty();
        group.Input[1].Mode.Should().Be(Hl7.Fhir.Model.StructureMap.StructureMapInputMode.Target);
        group.Input[1].Name.Should().Be("tgt");
        group.Input[0].Type.Should().BeNullOrEmpty();
        group.Extends.Should().Be("BackboneElement");
        group.TypeMode.Should().BeNull();

        group = sm.Group[2];
        group.Name.Should().Be("EncounterDiagnosis");
        group.Input.Count.Should().Be(2);
        group.Input[0].Mode.Should().Be(Hl7.Fhir.Model.StructureMap.StructureMapInputMode.Source);
        group.Input[0].Name.Should().Be("src");
        group.Input[0].Type.Should().BeNullOrEmpty();
        group.Input[1].Mode.Should().Be(Hl7.Fhir.Model.StructureMap.StructureMapInputMode.Target);
        group.Input[1].Name.Should().Be("tgt");
        group.Input[0].Type.Should().BeNullOrEmpty();
        group.Extends.Should().Be("BackboneElement");
        group.TypeMode.Should().BeNull();

        group = sm.Group[3];
        group.Name.Should().Be("EncounterAdmission");
        group.Input.Count.Should().Be(2);
        group.Input[0].Mode.Should().Be(Hl7.Fhir.Model.StructureMap.StructureMapInputMode.Source);
        group.Input[0].Name.Should().Be("src");
        group.Input[0].Type.Should().BeNullOrEmpty();
        group.Input[1].Mode.Should().Be(Hl7.Fhir.Model.StructureMap.StructureMapInputMode.Target);
        group.Input[1].Name.Should().Be("tgt");
        group.Input[0].Type.Should().BeNullOrEmpty();
        group.Extends.Should().Be("BackboneElement");
        group.TypeMode.Should().BeNull();

        group = sm.Group[4];
        group.Name.Should().Be("EncounterLocation");
        group.Input.Count.Should().Be(2);
        group.Input[0].Mode.Should().Be(Hl7.Fhir.Model.StructureMap.StructureMapInputMode.Source);
        group.Input[0].Name.Should().Be("src");
        group.Input[0].Type.Should().BeNullOrEmpty();
        group.Input[1].Mode.Should().Be(Hl7.Fhir.Model.StructureMap.StructureMapInputMode.Target);
        group.Input[1].Name.Should().Be("tgt");
        group.Input[0].Type.Should().BeNullOrEmpty();
        group.Extends.Should().Be("BackboneElement");
        group.TypeMode.Should().BeNull();

    }
}
