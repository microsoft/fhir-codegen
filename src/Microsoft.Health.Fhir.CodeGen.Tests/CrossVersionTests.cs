// <copyright file="FhirPackageTests.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Xml.Linq;
using FluentAssertions;
using Microsoft.Health.Fhir.CodeGen.CompareTool;
using Microsoft.Health.Fhir.CodeGen.Loader;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using Microsoft.Health.Fhir.MappingLanguage;
using Microsoft.Health.Fhir.PackageManager;
using Microsoft.Health.Fhir.PackageManager.Models;
using Xunit.Abstractions;
using Xunit.Sdk;
using static Microsoft.Health.Fhir.CodeGen.CompareTool.CrossVersionMapCollection;

namespace Microsoft.Health.Fhir.CodeGen.Tests;

public class CrossVersionTests
{
    public CrossVersionTests(ITestOutputHelper outputWriter)
    {
        Console.SetOut(new TestWriter(outputWriter));
    }

    [Theory(DisplayName = "TestLoadingFml")]
    [InlineData("R2toR3", "2to3")]
    [InlineData("R3toR2", "3to2")]
    [InlineData("R3toR4", "3to4")]
    [InlineData("R4toR3", "4to3")]
    [InlineData("R4toR5", "4to5")]
    [InlineData("R5toR4", "5to4")]
    [InlineData("R4BtoR5", "4Bto5")]
    [InlineData("R5toR4B", "5to4B")]
    public void TestLoadingFml(string path, string versionToVersion)
    {
        string prefixPath = @"C:\git\fhir-cross-version\input\";
        int versionToVersionLen = versionToVersion.Length;

        // files have different styles in each directory, but we want all FML files anyway
        string[] files = Directory.GetFiles(prefixPath+path, $"*.fml", SearchOption.TopDirectoryOnly);

        FhirMappingLanguage content = new();

        Dictionary<string, Dictionary<string, FmlTargetInfo>> fmlPathLookup = [];

        int errorCount = 0;
        foreach (string filename in files)
        {
            string fmlContent = File.ReadAllText(filename);

            if (!content.TryParse(fmlContent, out FhirStructureMap? fml))
            {
                Console.WriteLine($"Error loading {filename}: could not parse");
                errorCount++;
                continue;
            }

            fml.Should().NotBeNull();

            // extract the name root
            string name;

            if (fml.MetadataByPath.TryGetValue("name", out MetadataDeclaration? nameMeta))
            {
                name = nameMeta.Literal?.ValueAsString ?? throw new Exception($"Cross-version structure maps require a metadata name property: {filename}");
            }
            else
            {
                name = Path.GetFileNameWithoutExtension(filename);
            }

            if (name.EndsWith(versionToVersion, StringComparison.OrdinalIgnoreCase))
            {
                name = name[..^versionToVersionLen];
            }

            if (name.Equals("primitives", StringComparison.OrdinalIgnoreCase))
            {
                // skip primitive type map - we have that information internally already
                continue;
            }

            try
            {
            CrossVersionMapCollection.ProcessCrossVersionFml(name, fml, fmlPathLookup);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Processing {filename}: {ex.Message}");
                errorCount++;
            }

            //ProcessCrossVersionFml(string name, FhirStructureMap fml, Dictionary<string, List<GroupExpression>> fmlPathLookup)
        }
        errorCount.Should().Be(0, "Should be no parsing/processing errors");
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
    StringBuilder cache = new();
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
