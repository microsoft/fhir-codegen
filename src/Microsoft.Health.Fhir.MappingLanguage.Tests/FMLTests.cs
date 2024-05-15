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
    [Fact]
    internal void TestBuildingLiteralEnums()
    {
        List<string> lines = AntlrUtils.BuildLiteralEnums();

        lines.Should().NotBeNullOrEmpty();
    }

    [Fact]
    internal void FmlParseTest01()
    {
        string content = """"
/// url = "http://example.org/fhir/StructureDefinition/test"
/// id = "Fml4to5"
/// name = "FhirMarkup4to5"
/// title = "Test FML file to exercise core parsing"
/// status = "draft"
/// description = """
This was challenging to code into the grammar.
It should all be working now though
"""
/// jurisdiction =
/// jurisdiction.coding = 

/// jurisdiction.coding.code = 'AQ' // set a jurisdiction code

// use R4 Encounter as the source
uses "http://hl7.org/fhir/4.0/Encounter" alias EncounterR4 as source

// use the R5 Encounter as the target
uses "http://hl7.org/fhir/5.0/Encounter" alias EncounterR5 as target

/* the following is used in the conversion maps, but is not actually a canonical */
imports "http://hl7.org/fhir/uv/xver/StructureMap/*4to5"

let constFhirPathFn = combine(3, 4);
let constFhirPathFnParen = (combine(3, 4));
let constFhirPathSpecial = $this.status;
let constStringLit = 'one';
let constInt = 2;
let constDecimal = 3.0;
let constBool = true;
let constDoubleQuoted = "http://example.org";
let constDate = @2024-05-09;
let constNull = {};

// comment before a group
// with a second line 
group Encounter(source src : EncounterR4, target tgt : EncounterR5) extends DomainResource <<type+>> {
    src.identifier -> tgt.identifier;     // basic copy
    // translate code
    src.status as v -> tgt.status = translate(v, 'http://hl7.org/fhir/uv/xver/ConceptMap/enc.status-4to5', 'code');

    // create a CodeableConcept, dependent rule to apply as coding
    src.class as s ->  tgt.class = create('CodeableConcept') as t,  t.coding as tc then Coding(s, tc);

    // nested function copy with dependent rule
    src.participant as s -> tgt.participant as t then EncounterParticipant(s, t);

    // nested function copy with dependent rule, split onto multiple lines
    src.diagnosis as s -> tgt.diagnosis as t then
        EncounterDiagnosis(s, t);
}

/* a multi-line C-Style comment
 * that spans multiple lines
 */
group EncounterParticipant(source src, target tgt) extends BackboneElement {
  src.type -> tgt.type;
  src.period -> tgt.period;
  src.individual -> tgt.actor;
}

group EncounterDiagnosis(source src, target tgt) extends BackboneElement {
  src.condition -> tgt.condition;
  src.use -> tgt.use;
}
"""";
        FhirMappingLanguage fml = new();

        bool success = fml.TryParse(content, out FhirStructureMap? map);

        success.Should().BeTrue();
        if (!success)
        {
            return;
        }

        map.Should().NotBeNull();
        if (map == null)
        {
            return;
        }

        map.MetadataByPath.Count.Should().Be(9);
        map.MetadataByPath["url"].Literal!.ValueAsString.Should().Be("http://example.org/fhir/StructureDefinition/test");
        map.MetadataByPath["id"].Literal!.ValueAsString.Should().Be("Fml4to5");
        map.MetadataByPath["name"].Literal!.ValueAsString.Should().Be("FhirMarkup4to5");
        map.MetadataByPath["title"].Literal!.ValueAsString.Should().Be("Test FML file to exercise core parsing");
        map.MetadataByPath["status"].Literal!.ValueAsString.Should().Be("draft");
        map.MetadataByPath["description"].MarkdownValue!.Should().Be("This was challenging to code into the grammar.\r\nIt should all be working now though");
        map.MetadataByPath["jurisdiction"].Literal.Should().BeNull();
        map.MetadataByPath["jurisdiction.coding"].Literal.Should().BeNull();
        map.MetadataByPath["jurisdiction.coding.code"].Literal!.ValueAsString.Should().Be("AQ");
        //map.MetadataByPath["jurisdiction.coding.code"].InlineComment.Should().Be("set a jurisdiction code");


        //sm.Url.Should().Be("http://example.org/fhir/StructureDefinition/test");
        //sm.Id.Should().Be("Fml4to5");
        //sm.Name.Should().Be("FhirMarkup4to5");
        //sm.Title.Should().Be("Test FML file to exercise core parsing");
        //sm.Status.Should().Be(Hl7.Fhir.Model.PublicationStatus.Draft);
        //sm.Description.Should().Be("This was challenging to code into the grammar.\nIt should all be working now though\n");
        //sm.Jurisdiction.Count.Should().Be(1);
        //sm.Jurisdiction[0].Coding.Count.Should().Be(1);
        //sm.Jurisdiction[0].Coding[0].Code.Should().Be("AQ");
    }

    [Fact]
    internal void FmlCommentParseTest()
    {
        string content = """"
/// id = "Fml4to5" // single inline comment
/// name = "FhirMarkup4to5" /* first of two block comments */ /* second block comment */
/// title = "Comment test"
// line comment before description
/// description = """
This was challenging to code into the grammar.  // not a comment
It should all be working now though             /* also not a comment */
"""
// two lines
// before a directive
/// comment = "This is a comment"

// use R4 Encounter as the source
uses "http://hl7.org/fhir/4.0/Encounter" alias /* inline comment */ EncounterR4 /* here too */ as source

/* a multi-line block comment
 * that spans multiple lines
 */
uses "http://hl7.org/fhir/5.0/Encounter" alias EncounterR5 as target

/* the following is used in the conversion maps, but is not actually a canonical */
imports "http://hl7.org/fhir/uv/xver/StructureMap/*4to5"

group Encounter(source src : EncounterR4, target tgt : EncounterR5) extends DomainResource <<type+>> {
}
"""";
        FhirMappingLanguage fml = new();

        bool success = fml.TryParse(content, out FhirStructureMap? map);

        success.Should().BeTrue();
        if (!success)
        {
            return;
        }

        map.Should().NotBeNull();
        if (map == null)
        {
            return;
        }

        map.MetadataByPath.Count.Should().Be(5);
        //map.MetadataByPath["url"].Literal!.ValueAsString.Should().Be("http://example.org/fhir/StructureDefinition/test");
        //map.MetadataByPath["id"].Literal!.ValueAsString.Should().Be("Fml4to5");
        //map.MetadataByPath["name"].Literal!.ValueAsString.Should().Be("FhirMarkup4to5");
        //map.MetadataByPath["title"].Literal!.ValueAsString.Should().Be("Test FML file to exercise core parsing");
        //map.MetadataByPath["status"].Literal!.ValueAsString.Should().Be("draft");
        //map.MetadataByPath["description"].MarkdownValue!.Should().Be("This was challenging to code into the grammar.\r\nIt should all be working now though");
        //map.MetadataByPath["jurisdiction"].Literal.Should().BeNull();
        //map.MetadataByPath["jurisdiction.coding"].Literal.Should().BeNull();
        //map.MetadataByPath["jurisdiction.coding.code"].Literal!.ValueAsString.Should().Be("AQ");
    }
    //[Theory]
    //[FileData("data/Encounter4Bto5.fml")]
    //internal void TestParseEncounter4Bto5(string content)
    //{
    //    FhirMappingLanguage fml = new();

    //    bool success = fml.TryParse(content, out Hl7.Fhir.Model.StructureMap? sm);

    //    success.Should().BeTrue();
    //    if (!success)
    //    {
    //        return;
    //    }

    //    sm.Should().NotBeNull();
    //    if (sm == null)
    //    {
    //        return;
    //    }

    //    sm.Url.Should().Be("http://hl7.org/fhir/uv/xver/StructureMap/Encounter4Bto5");
    //    sm.Id.Should().Be("Encounter4Bto5");
    //    sm.Name.Should().Be("Encounter4Bto5");
    //    sm.Title.Should().Be("Encounter Transforms: R4B to R5");
    //    sm.Status.Should().Be(Hl7.Fhir.Model.PublicationStatus.Active);

    //    sm.Structure.Count.Should().Be(2);
    //    sm.Structure[0].Url.Should().Be("http://hl7.org/fhir/4.3/Encounter");
    //    sm.Structure[0].Alias.Should().Be("EncounterR4B");
    //    sm.Structure[0].Mode.Should().Be(Hl7.Fhir.Model.StructureMap.StructureMapModelMode.Source);
    //    sm.Structure[1].Url.Should().Be("http://hl7.org/fhir/5.0/Encounter");
    //    sm.Structure[1].Alias.Should().Be("EncounterR5");
    //    sm.Structure[1].Mode.Should().Be(Hl7.Fhir.Model.StructureMap.StructureMapModelMode.Target);

    //    sm.Import.Count().Should().Be(1);
    //    sm.Import.First().Should().Be("http://hl7.org/fhir/uv/xver/StructureMap/*4Bto5");

    //    sm.Group.Count.Should().Be(5);

    //    Hl7.Fhir.Model.StructureMap.GroupComponent group = sm.Group[0];
    //    group.Name.Should().Be("Encounter");
    //    group.Input.Count.Should().Be(2);
    //    group.Input[0].Mode.Should().Be(Hl7.Fhir.Model.StructureMap.StructureMapInputMode.Source);
    //    group.Input[0].Name.Should().Be("src");
    //    group.Input[0].Type.Should().Be("EncounterR4B");
    //    group.Input[1].Mode.Should().Be(Hl7.Fhir.Model.StructureMap.StructureMapInputMode.Target);
    //    group.Input[1].Name.Should().Be("tgt");
    //    group.Input[1].Type.Should().Be("EncounterR5");
    //    group.Extends.Should().Be("DomainResource");
    //    group.TypeMode.Should().Be(Hl7.Fhir.Model.StructureMap.StructureMapGroupTypeMode.TypeAndTypes);

    //    group.Rule.Count.Should().Be(19);

    //    Hl7.Fhir.Model.StructureMap.RuleComponent rule = group.Rule[0];
    //    rule.Name.Should().Be("identifier");
    //    rule.Source.Count.Should().Be(1);
    //    rule.Source[0].Context.Should().Be("src");
    //    rule.Source[0].Element.Should().Be("identifier");
    //    rule.Source[0].Variable.Should().Be("vvv");
    //    rule.Target.Count.Should().Be(1);
    //    rule.Target[0].Context.Should().Be("tgt");
    //    rule.Target[0].Element.Should().Be("identifier");
    //    rule.Target[0].Variable.Should().Be("vvv");
    //    rule.Target[0].Transform.Should().Be(Hl7.Fhir.Model.StructureMap.StructureMapTransform.Create);

    //    rule = group.Rule[1];
    //    rule.Name.Should().Be("status");
    //    rule.Source.Count.Should().Be(1);
    //    rule.Source[0].Context.Should().Be("src");
    //    rule.Source[0].Element.Should().Be("status");
    //    rule.Source[0].Variable.Should().Be("v");
    //    rule.Target.Count.Should().Be(1);
    //    rule.Target[0].Context.Should().Be("tgt");
    //    rule.Target[0].Element.Should().Be("status");
    //    rule.Target[0].Transform.Should().Be(Hl7.Fhir.Model.StructureMap.StructureMapTransform.Translate);
    //    rule.Target[0].Parameter.Count.Should().Be(3);
    //    rule.Target[0].Parameter[0].Value.Should().BeOfType<Hl7.Fhir.Model.Id>();
    //    rule.Target[0].Parameter[0].Value.ToString().Should().Be("v");
    //    rule.Target[0].Parameter[1].Value.Should().BeOfType<Hl7.Fhir.Model.FhirString>();
    //    rule.Target[0].Parameter[1].Value.ToString().Should().Be("http://hl7.org/fhir/uv/xver/ConceptMap/enc.status-4bto5");
    //    rule.Target[0].Parameter[2].Value.Should().BeOfType<Hl7.Fhir.Model.FhirString>();
    //    rule.Target[0].Parameter[2].Value.ToString().Should().Be("code");


    //    rule = group.Rule[2];
    //    rule.Name.Should().Be("class");
    //    rule.Source.Count.Should().Be(1);
    //    rule.Source[0].Context.Should().Be("src");
    //    rule.Source[0].Element.Should().Be("class");
    //    rule.Source[0].Variable.Should().Be("s");
    //    rule.Target.Count.Should().Be(2);
    //    rule.Target[0].Context.Should().Be("tgt");
    //    rule.Target[0].Element.Should().Be("class");
    //    rule.Source[0].Variable.Should().Be("s");
    //    rule.Target[0].Transform.Should().Be(Hl7.Fhir.Model.StructureMap.StructureMapTransform.Create);
    //    rule.Target[0].Parameter.Count.Should().Be(1);
    //    rule.Target[0].Parameter[0].Value.Should().BeOfType<Hl7.Fhir.Model.FhirString>();
    //    rule.Target[0].Parameter[0].Value.ToString().Should().Be("CodeableConcept");
    //    rule.Target[1].Context.Should().Be("t");
    //    rule.Target[1].Element.Should().Be("coding");
    //    rule.Target[1].Variable.Should().Be("tc");


    //    group = sm.Group[1];
    //    group.Name.Should().Be("EncounterParticipant");
    //    group.Input.Count.Should().Be(2);
    //    group.Input[0].Mode.Should().Be(Hl7.Fhir.Model.StructureMap.StructureMapInputMode.Source);
    //    group.Input[0].Name.Should().Be("src");
    //    group.Input[0].Type.Should().BeNullOrEmpty();
    //    group.Input[1].Mode.Should().Be(Hl7.Fhir.Model.StructureMap.StructureMapInputMode.Target);
    //    group.Input[1].Name.Should().Be("tgt");
    //    group.Input[0].Type.Should().BeNullOrEmpty();
    //    group.Extends.Should().Be("BackboneElement");
    //    group.TypeMode.Should().BeNull();

    //    group = sm.Group[2];
    //    group.Name.Should().Be("EncounterDiagnosis");
    //    group.Input.Count.Should().Be(2);
    //    group.Input[0].Mode.Should().Be(Hl7.Fhir.Model.StructureMap.StructureMapInputMode.Source);
    //    group.Input[0].Name.Should().Be("src");
    //    group.Input[0].Type.Should().BeNullOrEmpty();
    //    group.Input[1].Mode.Should().Be(Hl7.Fhir.Model.StructureMap.StructureMapInputMode.Target);
    //    group.Input[1].Name.Should().Be("tgt");
    //    group.Input[0].Type.Should().BeNullOrEmpty();
    //    group.Extends.Should().Be("BackboneElement");
    //    group.TypeMode.Should().BeNull();

    //    group = sm.Group[3];
    //    group.Name.Should().Be("EncounterAdmission");
    //    group.Input.Count.Should().Be(2);
    //    group.Input[0].Mode.Should().Be(Hl7.Fhir.Model.StructureMap.StructureMapInputMode.Source);
    //    group.Input[0].Name.Should().Be("src");
    //    group.Input[0].Type.Should().BeNullOrEmpty();
    //    group.Input[1].Mode.Should().Be(Hl7.Fhir.Model.StructureMap.StructureMapInputMode.Target);
    //    group.Input[1].Name.Should().Be("tgt");
    //    group.Input[0].Type.Should().BeNullOrEmpty();
    //    group.Extends.Should().Be("BackboneElement");
    //    group.TypeMode.Should().BeNull();

    //    group = sm.Group[4];
    //    group.Name.Should().Be("EncounterLocation");
    //    group.Input.Count.Should().Be(2);
    //    group.Input[0].Mode.Should().Be(Hl7.Fhir.Model.StructureMap.StructureMapInputMode.Source);
    //    group.Input[0].Name.Should().Be("src");
    //    group.Input[0].Type.Should().BeNullOrEmpty();
    //    group.Input[1].Mode.Should().Be(Hl7.Fhir.Model.StructureMap.StructureMapInputMode.Target);
    //    group.Input[1].Name.Should().Be("tgt");
    //    group.Input[0].Type.Should().BeNullOrEmpty();
    //    group.Extends.Should().Be("BackboneElement");
    //    group.TypeMode.Should().BeNull();

    //}
}
