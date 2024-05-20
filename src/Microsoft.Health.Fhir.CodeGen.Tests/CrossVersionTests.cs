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
    [Theory]
    [InlineData("C:\\git\\fhir-cross-version\\input\\R2toR3", "2to3")]
    [InlineData("C:\\git\\fhir-cross-version\\input\\R3toR2", "3to2")]
    [InlineData("C:\\git\\fhir-cross-version\\input\\R3toR4", "3to4")]
    [InlineData("C:\\git\\fhir-cross-version\\input\\R4toR3", "4to3")]
    [InlineData("C:\\git\\fhir-cross-version\\input\\R4toR5", "4to5")]
    [InlineData("C:\\git\\fhir-cross-version\\input\\R5toR4", "5to4")]
    [InlineData("C:\\git\\fhir-cross-version\\input\\R4BtoR5", "4Bto5")]
    [InlineData("C:\\git\\fhir-cross-version\\input\\R5toR4B", "5to4B")]
    public void TestLoadingFml(string path, string versionToVersion)
    {
        int versionToVersionLen = versionToVersion.Length;

        // files have different styles in each directory, but we want all FML files anyway
        string[] files = Directory.GetFiles(path, $"*.fml", SearchOption.TopDirectoryOnly);

        FhirMappingLanguage content = new();

        Dictionary<string, Dictionary<string, FmlTargetInfo>> fmlPathLookup = [];

        foreach (string filename in files)
        {
            string fmlContent = File.ReadAllText(filename);

            if (!content.TryParse(fmlContent, out FhirStructureMap? fml))
            {
                Console.WriteLine($"Error loading {filename}: could not parse");
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

            CrossVersionMapCollection.ProcessCrossVersionFml(name, fml, fmlPathLookup);

            //ProcessCrossVersionFml(string name, FhirStructureMap fml, Dictionary<string, List<GroupExpression>> fmlPathLookup)
        }
    }
}
