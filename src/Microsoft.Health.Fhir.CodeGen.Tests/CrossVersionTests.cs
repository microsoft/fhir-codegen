// <copyright file="FhirPackageTests.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Text;
using FluentAssertions;
using Hl7.Fhir.Model;
using Hl7.Fhir.Specification.Source;
using Microsoft.Health.Fhir.CodeGen.CompareTool;
using Microsoft.Health.Fhir.CodeGen.Tests.Extensions;
using Microsoft.Health.Fhir.MappingLanguage;
using Xunit.Abstractions;

namespace Microsoft.Health.Fhir.CodeGen.Tests;

public class CrossVersionTests
{
    public CrossVersionTests(ITestOutputHelper outputWriter)
    {
        Console.SetOut(new TestWriter(outputWriter));
    }

    private static string FindRelativeDir(string path)
    {
        return DirectoryContentsAttribute.FindRelativeDir(string.Empty, path, true);
    }

    [Theory(DisplayName = "TestLoadingFml")]
    [InlineData("fhir-cross-version/input/R2toR3", "2to3")]
    [InlineData("fhir-cross-version/input/R3toR2", "3to2")]
    [InlineData("fhir-cross-version/input/R3toR4", "3to4")]
    [InlineData("fhir-cross-version/input/R4toR3", "4to3")]
    [InlineData("fhir-cross-version/input/R4toR5", "4to5")]
    [InlineData("fhir-cross-version/input/R5toR4", "5to4")]
    [InlineData("fhir-cross-version/input/R4BtoR5", "4Bto5")]
    [InlineData("fhir-cross-version/input/R5toR4B", "5to4B")]
    public void TestLoadingFml(string path, string versionToVersion)
    {
        //string prefixPath = @"C:\git\fhir-cross-version\input\";
        int versionToVersionLen = versionToVersion.Length;

        // files have different styles in each directory, but we want all FML files anyway
        string[] files = Directory.GetFiles(FindRelativeDir(path), $"*.fml", SearchOption.TopDirectoryOnly);

        FhirMappingLanguage content = new();

        Dictionary<string, Dictionary<string, CrossVersionMapCollection.FmlTargetInfo>> fmlPathLookup = [];

        int errorCount = 0;
        foreach (string filename in files)
        {
            string fmlContent = File.ReadAllText(filename);

            Console.WriteLine($"Parsing {filename}");
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

    IAsyncResourceResolver OnlyVersion(CrossVersionResolver resolver, string version)
    {
        switch (version)
        {
            case "2": return resolver.OnlyDstu2;
            case "3": return resolver.OnlyStu3;
            case "4": return resolver.OnlyR4;
            case "4B": return resolver.OnlyR4B;
            case "5": return resolver.OnlyR5;
        }
        return null!;
    }

    [Theory(DisplayName = "ValidateCrossVersionMaps")]
    [InlineData("fhir-cross-version/input/R2toR3", "2to3")]
    [InlineData("fhir-cross-version/input/R3toR2", "3to2")]
    [InlineData("fhir-cross-version/input/R3toR4", "3to4")]
    [InlineData("fhir-cross-version/input/R4toR3", "4to3")]
    [InlineData("fhir-cross-version/input/R4toR5", "4to5")]
    [InlineData("fhir-cross-version/input/R5toR4", "5to4")]
    [InlineData("fhir-cross-version/input/R4BtoR5", "4Bto5")]
    [InlineData("fhir-cross-version/input/R5toR4B", "5to4B")]
    public async System.Threading.Tasks.Task ValidateCrossVersionMaps(string path, string versionToVersion)
    {
        int versionToVersionLen = versionToVersion.Length;
        int errorCount = 0;

        // files have different styles in each directory, but we want all FML files anyway
        string[] files = Directory.GetFiles(FindRelativeDir(path), $"*.fml", SearchOption.TopDirectoryOnly);

        FhirMappingLanguage content = new();

        Dictionary<string, Dictionary<string, CrossVersionMapCollection.FmlTargetInfo>> fmlPathLookup = [];

        var cvr = new CrossVersionResolver();
        var versions = versionToVersion.Split("to");
        await cvr.Initialize(versions);
        CachedResolver source = new CachedResolver(cvr);
        source.Load += Source_Load;

        var sourceResolver = new CachedResolver(new MultiResolver(OnlyVersion(cvr, versions[0]), cvr));
        sourceResolver.Load += Source_Load;

        var targetResolver = new CachedResolver(new MultiResolver(OnlyVersion(cvr, versions[1]), cvr));
        targetResolver.Load += Source_Load;

        async Task<StructureDefinition?> resolveMapUseCrossVersionType(string url, string? alias)
        {
            if (await source.ResolveByCanonicalUriAsync(url) is StructureDefinition sd)
            {
                // Console.WriteLine(" - yup");
                return sd;
            }
            Console.WriteLine($"\nError: Resolving Type {url} as {alias} was not found");
            errorCount++;
            return null;
        }
        IEnumerable<FhirStructureMap> resolveMaps(string url)
        {
            Console.WriteLine($"Resolving Maps {url}");
            return new List<FhirStructureMap>();
        }

        List<FhirStructureMap> allMaps = new List<FhirStructureMap>();
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

            try
            {
                CrossVersionMapCollection.ProcessCrossVersionFml(name, fml, fmlPathLookup);
                allMaps.Add(fml);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Processing {filename}: {ex.Message}");
                errorCount++;
            }
        }
        errorCount.Should().Be(0, "Should be no parsing/processing errors");

        // Prepare a cache of the TYPE based map groups
        Dictionary<string, GroupDeclaration> namedGroups = new Dictionary<string, GroupDeclaration>();
        Dictionary<string, GroupDeclaration?> typedGroups = new Dictionary<string, GroupDeclaration?>();

        // With a default set of maps from the fhir types to fhirpath primitives
        typedGroups.Add("http://hl7.org/fhirpath/System.Date -> http://hl7.org/fhirpath/System.DateTime", null);
        typedGroups.Add("http://hl7.org/fhirpath/System.DateTime -> http://hl7.org/fhirpath/System.Date", null);
        typedGroups.Add("http://hl7.org/fhirpath/System.String -> http://hl7.org/fhirpath/System.Integer", null);
        typedGroups.Add("http://hl7.org/fhirpath/System.Integer -> http://hl7.org/fhirpath/System.String", null);
        typedGroups.Add("http://hl7.org/fhirpath/System.Integer -> http://hl7.org/fhirpath/System.Decimal", null);

        foreach (var fml in allMaps)
        {
            foreach (var group in fml.GroupsByName.Values)
            {
                // Console.WriteLine($"{group.TypeMode} {group.Name}");
                if (!namedGroups.ContainsKey(group.Name))
                    namedGroups.Add(group.Name, group);
                else
                {
                    Console.WriteLine($"Error: Duplicate group name: {group.Name}");
                    errorCount++;
                }

                if (group.TypeMode == StructureMap.StructureMapGroupTypeMode.TypeAndTypes
                    || group.TypeMode == StructureMap.StructureMapGroupTypeMode.Types)
                {
                    Console.Write($"{group.TypeMode} {group.Name}");

                    // Check that all the parameters have type declarations
                    Dictionary<string, StructureDefinition?> aliasedTypes = new();
                    foreach (var use in fml.StructuresByUrl)
                    {
                        // Console.WriteLine($"Use {use.Key} as {use.Value?.Alias}");
                        var sd = await resolveMapUseCrossVersionType(use.Key.Trim('\"'), use.Value?.Alias);
                        if (use.Value?.Alias != null)
                            aliasedTypes.Add(use.Value.Alias, sd);
                        else if (sd != null && sd.Name != null)
                            aliasedTypes.Add(use.Value?.Alias ?? sd.Name, sd);
                    }
                    string? typeMapping = null;
                    foreach (var gp in group.Parameters)
                    {
                        if (string.IsNullOrEmpty(gp.TypeIdentifier))
                        {
                            Console.WriteLine($"\n    * No type provided for parameter `{gp.Identifier}`");
                            errorCount++;
                        }
                        else
                        {
                            string? type = gp.TypeIdentifier;
                            // lookup the type in the aliases
                            var resolver = gp.InputMode == StructureMap.StructureMapInputMode.Source ? sourceResolver : targetResolver;
                            if (type != null)
                            {
                                if (!type.Contains('/') && aliasedTypes.ContainsKey(type))
                                {
                                    var sd = aliasedTypes[type];
                                    if (sd != null)
                                    {
                                        var sw = new StructureDefinitionWalker(sd, resolver);
                                        type = $"{sd.Url}|{sd.Version}";
                                        gp.ParameterElementDefinition = sw.Current;
                                    }
                                }
                                else if (type != "string")
                                {
                                    Console.WriteLine($"\nError: Group {group.Name} parameter {gp.Identifier} at @{gp.Line}:{gp.Column} has no type `{gp.TypeIdentifier}`");
                                    errorCount++;
                                }
                            }
                            if (!string.IsNullOrEmpty(typeMapping))
                                typeMapping += " -> ";
                            typeMapping += type;
                        }
                    }
                    Console.Write($"\t\t{typeMapping}");
                    Console.Write("\n");

                    if (typedGroups.ContainsKey(typeMapping))
                    {
                        var eg = typedGroups[typeMapping];
                        Console.WriteLine($"    Error: Group {group.Name} duplicates the type mappings declared in group {eg.Name}");
                        errorCount++;
                    }
                    else
                    {
                        typedGroups.Add(typeMapping, group);
                    }
                }
                else
                {
                    // Console.WriteLine($"skipping {group.TypeMode} {group.Name}");
                }
            }
        }

        // Now scan all these maps
        foreach (var fml in allMaps)
        {
            try
            {
                var outcome = await CrossVersionMapCollection.VerifyFmlDataTypes(fml, resolveMapUseCrossVersionType, resolveMaps, sourceResolver, targetResolver, namedGroups, typedGroups);
                if (!outcome.Success)
                    errorCount++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: Processing {fml.MetadataByPath["name"].Literal?.ValueAsString}: {ex.Message}");
                errorCount++;
            }

            //ProcessCrossVersionFml(string name, FhirStructureMap fml, Dictionary<string, List<GroupExpression>> fmlPathLookup)
        }
        errorCount.Should().Be(0, "fml errors");
    }

    private void Source_Load(object sender, CachedResolver.LoadResourceEventArgs e)
    {
        if (e.Resource is IVersionableConformanceResource cr)
        {
            // Console.WriteLine($"{e.Url} {cr.Name} | {cr.Version}");
        }
    }

    //[Xunit.Fact(DisplayName = "PrepareCrossVersionStructureDefinitionCache")]
    //public async System.Threading.Tasks.Task PrepareCrossVersionStructureDefinitionCache()
    //{
    //    // Download the cross version packages zip file
    //    // http://fhir.org/packages/xver-packages.zip
    //    string crossVersionPackages = Path.Combine(
    //        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    //        "FhirCrossVersionTests");
    //    if (!Directory.Exists(crossVersionPackages))
    //        Directory.CreateDirectory(crossVersionPackages);

    //    Console.ForegroundColor = ConsoleColor.Green;
    //    Console.WriteLine($"Cross version cache located in {crossVersionPackages}");
    //    Console.ResetColor();

    //    string crossVersionPackagesZipFile = Path.Combine(crossVersionPackages, "xver-packages.zip");

    //    if (!File.Exists(crossVersionPackagesZipFile))
    //    {
    //        Console.WriteLine($"Downloading http://fhir.org/packages/xver-packages.zip");
    //        HttpClient server = new HttpClient();
    //        var stream = await server.GetStreamAsync("http://fhir.org/packages/xver-packages.zip");
    //        using (var outStream = File.OpenWrite(crossVersionPackagesZipFile))
    //        {
    //            await stream.CopyToAsync(outStream);
    //            await outStream.FlushAsync();
    //        }
    //    }

    //    using (var zipStream = File.OpenRead(crossVersionPackagesZipFile))
    //    {
    //        ZipArchive archive = new ZipArchive(zipStream);
    //        foreach (var item in archive.Entries)
    //        {
    //            if ((item.Name.EndsWith(".as.r5.tgz") || item.Name.EndsWith("hl7.fhir.r5.core.tgz")) && !item.Name.StartsWith("."))
    //            {
    //                Console.Write($"Verifying cache for {item.Name}");
    //                var path = Path.Combine(
    //                    crossVersionPackages,
    //                    item.Name.Split('.').Skip(2).First());
    //                if (!Directory.Exists(path))
    //                {
    //                    Console.WriteLine($"\r\n    Extracting for {item.Name.Split('.').Skip(2).First()}");
    //                    Directory.CreateDirectory(path);

    //                    // Now extract this package into this folder
    //                    using (var tarStream = new GZipStream(item.Open(), CompressionMode.Decompress))
    //                    {
    //                        TarReader r = new TarReader(tarStream);
    //                        var a = await r.GetNextEntryAsync();
    //                        while (a != null)
    //                        {
    //                            if (!a.Name.StartsWith("package/other/")
    //                                && !a.Name.StartsWith("package/openapi/")
    //                                && !a.Name.StartsWith("package/xml/")
    //                                && a.Name != "package/.index.json"
    //                                && a.Name.Contains("/StructureDefinition-"))
    //                            {
    //                                // Console.WriteLine($"{a.Name}");
    //                                await a.ExtractToFileAsync(Path.Combine(path, a.Name.Replace("package/", "")), true);
    //                            }
    //                            a = await r.GetNextEntryAsync();
    //                        }
    //                    }
    //                }
    //                else
    //                {
    //                    Console.WriteLine($" - current");
    //                }
    //            }
    //        }
    //    }
    //}

}

public class TestWriter : TextWriter
{
    public ITestOutputHelper OutputWriter { get; }

    public override System.Text.Encoding Encoding => System.Text.Encoding.ASCII;

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
