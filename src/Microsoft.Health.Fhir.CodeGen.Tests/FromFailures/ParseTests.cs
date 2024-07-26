// <copyright file="StructureParseTests.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using System.Text.Json;
using FluentAssertions;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;
using Microsoft.Health.Fhir.CodeGen.Tests.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Models;

namespace Microsoft.Health.Fhir.CodeGen.Tests.FromFailures;

/// <summary>A structure parse tests.</summary>
public class ParseTests
{
    ///// <summary>
    ///// Note that the Expansion is WRONG in several publications
    ///// TODO(ginoc): Remove this test when current build is verified correct.
    ///// </summary>
    ///// <param name="json">The JSON.</param>
    //[Theory]
    //[FileData("TestData/R5/expansions/ValueSet-units-of-time.json")]
    //[Trait("Category", "Issues")]
    //[Trait("FhirVersion", "R5")]
    //public void TestParseR5ValueSetUnitsOfTime(string json)
    //{
    //    FhirJsonPocoDeserializer parser = new(new FhirJsonPocoDeserializerSettings()
    //    {
    //        DisableBase64Decoding = false,
    //        Validator = null,
    //    });

    //    // always use lenient parsing
    //    Resource parsed = parser.DeserializeResource(json);

    //    parsed.Should().NotBeNull();
    //    parsed.Should().BeOfType<ValueSet>();

    //    ValueSet vs = (ValueSet)parsed;

    //    vs.Expansion.Should().NotBeNull();
    //    vs.Expansion.Contains.Should().NotBeEmpty();
    //    vs.Expansion.Contains.Count.Should().Be(7);

    //    foreach (ValueSet.ContainsComponent cc in vs.Expansion.Contains)
    //    {
    //        cc.Display.Should().StartWith(cc.Code);
    //    }
    //}

    [Theory]
    [FileData("TestData/R4B/ValueSet-nhin-purposeofuse.json")]
    [Trait("Category", "Issues")]
    [Trait("FhirVersion", "R4B")]
    public void TestParseR4BValueSetNhinPOU(string json)
    {
        Hl7.Fhir.ElementModel.ISourceNode sn = FhirJsonNode.Parse(json);

        Microsoft.Health.Fhir.CrossVersion.Converter_43_50 c = new();

        Resource r = c.Convert(sn);

        r.Should().NotBeNull();
        r.Should().BeOfType<ValueSet>();

        ValueSet vs = (ValueSet)r;

        vs.Id.Should().Be("nhin-purposeofuse");
        vs.Name.Should().Be("NHIN PurposeOfUse");
        vs.Contained.Should().NotBeEmpty();
        vs.Contained[0].Should().BeOfType<ConceptMap>();
        vs.Contained[0].Id.Should().Be("map");

        vs.Experimental.Should().BeFalse();
        vs.DateElement.Should().BeEquivalentTo(new FhirDateTime(2010, 1, 29));
    }

    [Theory]
    [FileData("TestData/R5/StructureDefinition-integer64.json")]
    [Trait("Category", "Issues")]
    [Trait("FhirVersion", "R5")]
    public void TestParseR5StructureInt64(string json)
    {
        FhirJsonPocoDeserializer parser = new(new FhirJsonPocoDeserializerSettings()
        {
            DisableBase64Decoding = false,
            Validator = null,
            OnPrimitiveParseFailed = LocalPrimitiveParseHandler,
        });

        // always use lenient parsing
        Resource parsed = parser.DeserializeResource(json);

        parsed.Should().NotBeNull();
        parsed.Should().BeOfType<StructureDefinition>();

        StructureDefinition sd = (StructureDefinition)parsed;

        sd.cgArtifactClass().Should().Be(FhirArtifactClassEnum.PrimitiveType);
    }

    public (object?, FhirJsonException?) LocalPrimitiveParseHandler(
        ref Utf8JsonReader reader,
        Type targetType,
        object? originalValue,
        FhirJsonException originalException)
    {
        if (targetType == typeof(long))
        {
            if (originalValue is long ol)
            {
                return (ol, null);
            }

            if (originalValue is string s)
            {
                if (long.TryParse(s, out long l))
                {
                    return (l, null);
                }
            }
        }

        return (null, null);
    }
}
