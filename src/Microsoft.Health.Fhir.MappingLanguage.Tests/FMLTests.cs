// <copyright file="FMLTests.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.ComponentModel;
using System.IO;
using System.Text;
using Shouldly;
using Hl7.Fhir.Utility;
using Microsoft.Health.Fhir.MappingLanguage.Tests.Extensions;
using Xunit.Abstractions;

namespace Microsoft.Health.Fhir.MappingLanguage.Tests;

public class FMLTests
{
    public FMLTests(ITestOutputHelper outputWriter)
    {
        Console.SetOut(new TestWriter(outputWriter));
    }

    [Fact]
    [Trait("Category", "FML")]
    internal void TestBuildingLiteralEnums()
    {
        // Output what should be in the enum to the console (test output)
        List<string> lines = AntlrUtils.BuildLiteralEnums();
        Console.WriteLine(String.Join("\n", lines));

        // Now check that the actual code matches what it should be
        lines.ShouldNotBeNullOrEmpty();

        var evs = typeof(MappingLanguage.FmlTokenTypeCodes).GetEnumValues();
        Assert.Equal(evs.Length, lines.Count);
        foreach (var ev in evs)
        {
            var line = lines.FirstOrDefault(v => v.StartsWith($"{ev} = "));
            if (line != null)
                lines.Remove(line);
            else
                Console.WriteLine($"Missing: {ev}");
        }
        Assert.Empty(lines); // should be no lines left over

        // Also test the other enum
        var rcs = typeof(MappingLanguage.FmlRuleCodes).GetEnumValues();
        FmlMappingParser.ruleNames.Length.ShouldBe(rcs.Length);
        bool hasIssues = false;
        foreach (var rc in rcs)
        {
            if (!FmlMappingParser.ruleNames.Contains($"{rc}", StringComparer.OrdinalIgnoreCase))
            {
                Console.WriteLine($"FmlMappingParser.ruleNames: {rc} is not in the enum");
                hasIssues = true;
            }
        }
        Assert.False(hasIssues);
    }

    [Fact]
    [Trait("Category", "FML")]
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
    src.source -> tgt.source;

//    s.header as s1 ->  tgt.parameter as t,  t.name = (%s1.substring(0, %s1.indexOf(': '))),  t.value = (%s1.substring(%s1.indexOf(': ') + 1));

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

        success.ShouldBeTrue();
        if (!success)
        {
            return;
        }

        map.ShouldNotBeNull();
        if (map == null)
        {
            return;
        }

        map.MetadataByPath.Count.ShouldBe(9);
        map.MetadataByPath["url"].Literal!.ValueAsString.ShouldBe("http://example.org/fhir/StructureDefinition/test");
        map.MetadataByPath["id"].Literal!.ValueAsString.ShouldBe("Fml4to5");
        map.MetadataByPath["name"].Literal!.ValueAsString.ShouldBe("FhirMarkup4to5");
        map.MetadataByPath["title"].Literal!.ValueAsString.ShouldBe("Test FML file to exercise core parsing");
        map.MetadataByPath["status"].Literal!.ValueAsString.ShouldBe("draft");
        map.MetadataByPath["description"].MarkdownValue!.ShouldContain("This was challenging to code into the grammar.");
        map.MetadataByPath["description"].MarkdownValue!.ShouldContain("It should all be working now though");
        map.MetadataByPath["jurisdiction"].Literal.ShouldBeNull();
        map.MetadataByPath["jurisdiction.coding"].Literal.ShouldBeNull();
        map.MetadataByPath["jurisdiction.coding.code"].Literal!.ValueAsString.ShouldBe("AQ");
        //map.MetadataByPath["jurisdiction.coding.code"].InlineComment.ShouldBe("set a jurisdiction code");


        //sm.Url.ShouldBe("http://example.org/fhir/StructureDefinition/test");
        //sm.Id.ShouldBe("Fml4to5");
        //sm.Name.ShouldBe("FhirMarkup4to5");
        //sm.Title.ShouldBe("Test FML file to exercise core parsing");
        //sm.Status.ShouldBe(Hl7.Fhir.Model.PublicationStatus.Draft);
        //sm.Description.ShouldBe("This was challenging to code into the grammar.\nIt should all be working now though\n");
        //sm.Jurisdiction.Count.ShouldBe(1);
        //sm.Jurisdiction[0].Coding.Count.ShouldBe(1);
        //sm.Jurisdiction[0].Coding[0].Code.ShouldBe("AQ");
    }

    [Fact]
    [Trait("Category", "FML")]
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

        success.ShouldBeTrue();
        if (!success)
        {
            return;
        }

        map.ShouldNotBeNull();
        if (map == null)
        {
            return;
        }

        map.MetadataByPath.Count.ShouldBe(5);
        //map.MetadataByPath["url"].Literal!.ValueAsString.ShouldBe("http://example.org/fhir/StructureDefinition/test");
        //map.MetadataByPath["id"].Literal!.ValueAsString.ShouldBe("Fml4to5");
        //map.MetadataByPath["name"].Literal!.ValueAsString.ShouldBe("FhirMarkup4to5");
        //map.MetadataByPath["title"].Literal!.ValueAsString.ShouldBe("Test FML file to exercise core parsing");
        //map.MetadataByPath["status"].Literal!.ValueAsString.ShouldBe("draft");
        //map.MetadataByPath["description"].MarkdownValue!.ShouldBe("This was challenging to code into the grammar.\r\nIt should all be working now though");
        //map.MetadataByPath["jurisdiction"].Literal.ShouldBeNull();
        //map.MetadataByPath["jurisdiction.coding"].Literal.ShouldBeNull();
        //map.MetadataByPath["jurisdiction.coding.code"].Literal!.ValueAsString.ShouldBe("AQ");
    }
    //[Theory]
    //[FileData("data/Encounter4Bto5.fml")]
    //internal void TestParseEncounter4Bto5(string content)
    //{
    //    FhirMappingLanguage fml = new();

    //    bool success = fml.TryParse(content, out Hl7.Fhir.Model.StructureMap? sm);

    //    success.ShouldBeTrue();
    //    if (!success)
    //    {
    //        return;
    //    }

    //    sm.ShouldNotBeNull();
    //    if (sm == null)
    //    {
    //        return;
    //    }

    //    sm.Url.ShouldBe("http://hl7.org/fhir/uv/xver/StructureMap/Encounter4Bto5");
    //    sm.Id.ShouldBe("Encounter4Bto5");
    //    sm.Name.ShouldBe("Encounter4Bto5");
    //    sm.Title.ShouldBe("Encounter Transforms: R4B to R5");
    //    sm.Status.ShouldBe(Hl7.Fhir.Model.PublicationStatus.Active);

    //    sm.Structure.Count.ShouldBe(2);
    //    sm.Structure[0].Url.ShouldBe("http://hl7.org/fhir/4.3/Encounter");
    //    sm.Structure[0].Alias.ShouldBe("EncounterR4B");
    //    sm.Structure[0].Mode.ShouldBe(Hl7.Fhir.Model.StructureMap.StructureMapModelMode.Source);
    //    sm.Structure[1].Url.ShouldBe("http://hl7.org/fhir/5.0/Encounter");
    //    sm.Structure[1].Alias.ShouldBe("EncounterR5");
    //    sm.Structure[1].Mode.ShouldBe(Hl7.Fhir.Model.StructureMap.StructureMapModelMode.Target);

    //    sm.Import.Count().ShouldBe(1);
    //    sm.Import.First().ShouldBe("http://hl7.org/fhir/uv/xver/StructureMap/*4Bto5");

    //    sm.Group.Count.ShouldBe(5);

    //    Hl7.Fhir.Model.StructureMap.GroupComponent group = sm.Group[0];
    //    group.Name.ShouldBe("Encounter");
    //    group.Input.Count.ShouldBe(2);
    //    group.Input[0].Mode.ShouldBe(Hl7.Fhir.Model.StructureMap.StructureMapInputMode.Source);
    //    group.Input[0].Name.ShouldBe("src");
    //    group.Input[0].Type.ShouldBe("EncounterR4B");
    //    group.Input[1].Mode.ShouldBe(Hl7.Fhir.Model.StructureMap.StructureMapInputMode.Target);
    //    group.Input[1].Name.ShouldBe("tgt");
    //    group.Input[1].Type.ShouldBe("EncounterR5");
    //    group.Extends.ShouldBe("DomainResource");
    //    group.TypeMode.ShouldBe(Hl7.Fhir.Model.StructureMap.StructureMapGroupTypeMode.TypeAndTypes);

    //    group.Rule.Count.ShouldBe(19);

    //    Hl7.Fhir.Model.StructureMap.RuleComponent rule = group.Rule[0];
    //    rule.Name.ShouldBe("identifier");
    //    rule.Source.Count.ShouldBe(1);
    //    rule.Source[0].Context.ShouldBe("src");
    //    rule.Source[0].Element.ShouldBe("identifier");
    //    rule.Source[0].Variable.ShouldBe("vvv");
    //    rule.Target.Count.ShouldBe(1);
    //    rule.Target[0].Context.ShouldBe("tgt");
    //    rule.Target[0].Element.ShouldBe("identifier");
    //    rule.Target[0].Variable.ShouldBe("vvv");
    //    rule.Target[0].Transform.ShouldBe(Hl7.Fhir.Model.StructureMap.StructureMapTransform.Create);

    //    rule = group.Rule[1];
    //    rule.Name.ShouldBe("status");
    //    rule.Source.Count.ShouldBe(1);
    //    rule.Source[0].Context.ShouldBe("src");
    //    rule.Source[0].Element.ShouldBe("status");
    //    rule.Source[0].Variable.ShouldBe("v");
    //    rule.Target.Count.ShouldBe(1);
    //    rule.Target[0].Context.ShouldBe("tgt");
    //    rule.Target[0].Element.ShouldBe("status");
    //    rule.Target[0].Transform.ShouldBe(Hl7.Fhir.Model.StructureMap.StructureMapTransform.Translate);
    //    rule.Target[0].Parameter.Count.ShouldBe(3);
    //    rule.Target[0].Parameter[0].Value.ShouldBeOfType<Hl7.Fhir.Model.Id>();
    //    rule.Target[0].Parameter[0].Value.ToString().ShouldBe("v");
    //    rule.Target[0].Parameter[1].Value.ShouldBeOfType<Hl7.Fhir.Model.FhirString>();
    //    rule.Target[0].Parameter[1].Value.ToString().ShouldBe("http://hl7.org/fhir/uv/xver/ConceptMap/enc.status-4bto5");
    //    rule.Target[0].Parameter[2].Value.ShouldBeOfType<Hl7.Fhir.Model.FhirString>();
    //    rule.Target[0].Parameter[2].Value.ToString().ShouldBe("code");


    //    rule = group.Rule[2];
    //    rule.Name.ShouldBe("class");
    //    rule.Source.Count.ShouldBe(1);
    //    rule.Source[0].Context.ShouldBe("src");
    //    rule.Source[0].Element.ShouldBe("class");
    //    rule.Source[0].Variable.ShouldBe("s");
    //    rule.Target.Count.ShouldBe(2);
    //    rule.Target[0].Context.ShouldBe("tgt");
    //    rule.Target[0].Element.ShouldBe("class");
    //    rule.Source[0].Variable.ShouldBe("s");
    //    rule.Target[0].Transform.ShouldBe(Hl7.Fhir.Model.StructureMap.StructureMapTransform.Create);
    //    rule.Target[0].Parameter.Count.ShouldBe(1);
    //    rule.Target[0].Parameter[0].Value.ShouldBeOfType<Hl7.Fhir.Model.FhirString>();
    //    rule.Target[0].Parameter[0].Value.ToString().ShouldBe("CodeableConcept");
    //    rule.Target[1].Context.ShouldBe("t");
    //    rule.Target[1].Element.ShouldBe("coding");
    //    rule.Target[1].Variable.ShouldBe("tc");


    //    group = sm.Group[1];
    //    group.Name.ShouldBe("EncounterParticipant");
    //    group.Input.Count.ShouldBe(2);
    //    group.Input[0].Mode.ShouldBe(Hl7.Fhir.Model.StructureMap.StructureMapInputMode.Source);
    //    group.Input[0].Name.ShouldBe("src");
    //    group.Input[0].Type.ShouldBeNullOrEmpty();
    //    group.Input[1].Mode.ShouldBe(Hl7.Fhir.Model.StructureMap.StructureMapInputMode.Target);
    //    group.Input[1].Name.ShouldBe("tgt");
    //    group.Input[0].Type.ShouldBeNullOrEmpty();
    //    group.Extends.ShouldBe("BackboneElement");
    //    group.TypeMode.ShouldBeNull();

    //    group = sm.Group[2];
    //    group.Name.ShouldBe("EncounterDiagnosis");
    //    group.Input.Count.ShouldBe(2);
    //    group.Input[0].Mode.ShouldBe(Hl7.Fhir.Model.StructureMap.StructureMapInputMode.Source);
    //    group.Input[0].Name.ShouldBe("src");
    //    group.Input[0].Type.ShouldBeNullOrEmpty();
    //    group.Input[1].Mode.ShouldBe(Hl7.Fhir.Model.StructureMap.StructureMapInputMode.Target);
    //    group.Input[1].Name.ShouldBe("tgt");
    //    group.Input[0].Type.ShouldBeNullOrEmpty();
    //    group.Extends.ShouldBe("BackboneElement");
    //    group.TypeMode.ShouldBeNull();

    //    group = sm.Group[3];
    //    group.Name.ShouldBe("EncounterAdmission");
    //    group.Input.Count.ShouldBe(2);
    //    group.Input[0].Mode.ShouldBe(Hl7.Fhir.Model.StructureMap.StructureMapInputMode.Source);
    //    group.Input[0].Name.ShouldBe("src");
    //    group.Input[0].Type.ShouldBeNullOrEmpty();
    //    group.Input[1].Mode.ShouldBe(Hl7.Fhir.Model.StructureMap.StructureMapInputMode.Target);
    //    group.Input[1].Name.ShouldBe("tgt");
    //    group.Input[0].Type.ShouldBeNullOrEmpty();
    //    group.Extends.ShouldBe("BackboneElement");
    //    group.TypeMode.ShouldBeNull();

    //    group = sm.Group[4];
    //    group.Name.ShouldBe("EncounterLocation");
    //    group.Input.Count.ShouldBe(2);
    //    group.Input[0].Mode.ShouldBe(Hl7.Fhir.Model.StructureMap.StructureMapInputMode.Source);
    //    group.Input[0].Name.ShouldBe("src");
    //    group.Input[0].Type.ShouldBeNullOrEmpty();
    //    group.Input[1].Mode.ShouldBe(Hl7.Fhir.Model.StructureMap.StructureMapInputMode.Target);
    //    group.Input[1].Name.ShouldBe("tgt");
    //    group.Input[0].Type.ShouldBeNullOrEmpty();
    //    group.Extends.ShouldBe("BackboneElement");
    //    group.TypeMode.ShouldBeNull();

    //}

    [Fact]
    [Trait("Category", "FML")]
    internal void FmlParseDefaultValue()
    {
        string content = """"
/// url = "http://example.org/fhir/StructureDefinition/test"
/// id = "Fml4to5"
group Encounter(source src, target tgt) {
    src.source default (24) -> tgt.source;
    src.source default "24" -> tgt.source;
}
"""";
        FhirMappingLanguage fml = new();

        bool success = fml.TryParse(content, out FhirStructureMap? map);

        success.ShouldBeTrue();
        if (!success)
        {
            return;
        }

        map.ShouldNotBeNull();
        if (map == null)
        {
            return;
        }

        map.GroupsByName.Count.ShouldBe(1);
        List<GroupExpression> rules = map.GroupsByName.Values.First().Expressions;
        rules.Count.ShouldBe(2);
        rules[0].MappingExpression!.Sources.First().DefaultExpression!.RawText.ShouldBe("24");
        rules[1].MappingExpression!.Sources.First().DefaultValue.ShouldBe("24");
    }
}
public class TestWriter : TextWriter
{
    public ITestOutputHelper OutputWriter { get; }

    public override Encoding Encoding => Encoding.ASCII;

    public TestWriter(ITestOutputHelper outputWriter)
    {
        OutputWriter = outputWriter;
    }
    private StringBuilder cache = new();
    public override void Write(char value)
    {
        if (value == '\n')
        {
            OutputWriter.WriteLine(cache.ToString());
            cache.Clear();
        }
        else
        {
            cache.Append(value);
        }
    }
    public override void Flush()
    {
        if (cache.Length == 0) return;
        OutputWriter.WriteLine(cache.ToString());
        cache.Clear();
    }
}
