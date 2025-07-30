using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.Comparison.CompareTool;
using Microsoft.Health.Fhir.CodeGen.Configuration;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;
using Microsoft.Health.Fhir.CodeGen.Language;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using Microsoft.Health.Fhir.CodeGenCommon.Utils;
using System.CommandLine;
using System.Linq;
using System.Data.Common;
using System.Collections.Concurrent;
using Microsoft.Health.Fhir.CodeGenCommon.Models;
using Microsoft.Health.Fhir.Comparison.Models;
using System.Xml.Linq;
using System.Data;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Specification.Snapshot;
using static System.Net.Mime.MediaTypeNames;
using Hl7.FhirPath.Sprache;
using Tasks = System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Formats.Tar;

namespace Microsoft.Health.Fhir.Comparison.XVer;

public partial class XVerProcessor
{

    /// <summary>
    /// Creates a compressed .tgz (tar.gz) archive from the specified source directory.
    /// </summary>
    /// <param name="sourceDirectory">The directory to archive and compress.</param>
    /// <param name="outputTgzFile">The path to the output .tgz file.</param>
    /// <remarks>
    /// This method creates a tar archive of the specified directory and compresses it using GZip.
    /// If an error occurs during the process, a message is written to the console.
    /// </remarks>
    private static void createTgzFromDirectory(string sourceDirectory, string outputTgzFile)
    {
        try
        {
            // Compress the tar file into a .tgz file
            using (FileStream tgzFileStream = File.Create(outputTgzFile))
            using (GZipStream gzipStream = new GZipStream(tgzFileStream, CompressionLevel.Optimal))
            {
                TarFile.CreateFromDirectory(sourceDirectory, gzipStream, false);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to create tgz {outputTgzFile} from source {sourceDirectory}: {ex.Message}");
        }
    }

    /// <summary>
    /// Writes the ImplementationGuide, manifest, index, and package.json files for each validation package.
    /// </summary>
    /// <param name="packageSupports">The list of package support objects representing each FHIR package.</param>
    /// <param name="allPackageIndexInfos">The list of all cross-version package index information objects.</param>
    /// <param name="fhirDir">The root directory where FHIR artifacts are written.</param>
    private void writeXverValidationPackageSupportFiles(
        List<PackageXverSupport> packageSupports,
        List<XverPackageIndexInfo> allPackageIndexInfos,
        string fhirDir)
    {
        // iterate over the support packages
        foreach (PackageXverSupport packageSupport in packageSupports)
        {
            string packageId = getPackageId(null, packageSupport.Package);
            string dir = createExportPackageDir(fhirDir, null, packageSupport.Package);

            dir = Path.Combine(dir, "package");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            List<(string packageId, string packageVersion)> internalDependencies = [];
            foreach (PackageXverSupport sourcePackage in packageSupports)
            {
                if (sourcePackage.Package.Key == packageSupport.Package.Key)
                {
                    continue;
                }

                internalDependencies.Add((
                    getPackageId(sourcePackage.Package, packageSupport.Package),
                    _crossDefinitionVersion));
            }

            // get the list of index informations that *target* this version
            List<XverPackageIndexInfo> packageIndexInfos = allPackageIndexInfos.Where(ii => ii.TargetPackageSupport.Package.Key == packageSupport.Package.Key).ToList();

            // build and write the ImplementationGuide resource for the combination package (single source and target)
            {
                string igJson;

                if (packageSupport.Package.FhirVersionShort.StartsWith('4'))
                {
                    igJson = getIgJsonR4(packageSupport.Package, packageId, internalDependencies, packageIndexInfos);
                }
                else if (packageSupport.Package.FhirVersionShort.StartsWith('5'))
                {
                    igJson = getIgJsonR5(packageSupport.Package, packageId, internalDependencies, packageIndexInfos);
                }
                else
                {
                    // TODO: Implment DSTU2 and STU3
                    continue;
                }

                string filename = $"ImplementationGuide-{packageId}.json";
                File.WriteAllText(Path.Combine(dir, filename), igJson);
            }

            // build and write the package.manifest.json file
            {
                string pmJson = $$$"""
                    {
                      "version" : "{{{_crossDefinitionVersion}}}",
                      "fhirVersion" : ["{{{packageSupport.Package.PackageVersion}}}"],
                      "date" : "{{{DateTime.Now.ToString("yyyyMMddHHmmss")}}}",
                      "name" : "{{{packageId}}}",
                      "jurisdiction" : "http://unstats.un.org/unsd/methods/m49/m49.htm#001"
                    }
                    """;

                string filename = "package.manifest.json";
                File.WriteAllText(Path.Combine(dir, filename), pmJson);
            }

            // build and write the .index.json file
            {
                string indexJson = getIndexJson(packageSupport.Package, packageId, internalDependencies, packageIndexInfos);
                string filename = ".index.json";
                File.WriteAllText(Path.Combine(dir, filename), indexJson);
            }

            // build and write the package.json file
            {
                string additionalDependencies = internalDependencies.Count == 0
                    ? string.Empty
                    : (", " + string.Join(", ", internalDependencies.Select(pi => $"\"{pi.packageId}\" : \"{pi.packageVersion}\"")));

                string packageJson = $$$"""
                    {
                        "name" : "{{{packageId}}}",
                        "version" : "{{{_crossDefinitionVersion}}}",
                        "tools-version" : 3,
                        "type" : "IG",
                        "date" : "{{{DateTime.Now.ToString("yyyyMMddHHmmss")}}}",
                        "license" : "CC0-1.0",
                        "canonical" : "http://hl7.org/fhir/uv/xver",
                        "notForPublication" : true,
                        "url" : "http://hl7.org/fhir/uv/xver",
                        "title" : "XVer-{{{packageSupport.Package.ShortName}}}",
                        "description" : "All Cross Version Extensions for FHIR {{{packageSupport.Package.ShortName}}}",
                        "fhirVersions" : ["{{{packageSupport.Package.PackageVersion}}}"],
                        "dependencies" : {
                            "{{{packageSupport.Package.PackageId}}}" : "{{{packageSupport.Package.PackageVersion}}}",
                            "hl7.terminology.{{{packageSupport.Package.ShortName.ToLowerInvariant()}}}" : "6.3.0",
                            "hl7.fhir.uv.extensions.{{{packageSupport.Package.ShortName.ToLowerInvariant()}}}" : "5.2.0",
                            "hl7.fhir.uv.tools.{{{packageSupport.Package.ShortName.ToLowerInvariant()}}}" : "current"
                            {{{additionalDependencies}}}
                        },
                        "author" : "HL7 International / FHIR Infrastructure",
                        "maintainers" : [
                            {
                                "name" : "HL7 International / FHIR Infrastructure",
                                "url" : "http://www.hl7.org/Special/committees/fiwg"
                            }
                        ],
                        "directories" : {
                            "lib" : "package",
                            "doc" : "doc"
                        },
                        "jurisdiction" : "http://unstats.un.org/unsd/methods/m49/m49.htm#001"
                    }
                    """;

                string filename = "package.json";
                File.WriteAllText(Path.Combine(dir, filename), packageJson);
            }
        }
    }


    /// <summary>
    /// Writes ImplementationGuide, manifest, index, and package.json files for each single source-target package combination.
    /// </summary>
    /// <param name="packageSupports">The list of package support objects representing each FHIR package.</param>
    /// <param name="focusPackageIndex">The index of the source package in the packageSupports list.</param>
    /// <param name="xverValueSets">The dictionary of cross-version ValueSets, keyed by (source ValueSet key, target package id).</param>
    /// <param name="xverExtensions">The dictionary of cross-version StructureDefinitions (extensions), keyed by (source element key, target package id).</param>
    /// <param name="fhirDir">The root directory where FHIR artifacts are written.</param>
    /// <returns>A list of <see cref="XverPackageIndexInfo"/> objects containing index information for each source-target package combination.</returns>
    private List<XverPackageIndexInfo> writeXverSinglePackageSupportFiles(
        List<PackageXverSupport> packageSupports,
        int focusPackageIndex,
        Dictionary<(int sourceVsKey, int targetPackageId), ValueSet> xverValueSets,
        Dictionary<(int sourceElementKey, int targetPackageId), (StructureDefinition, DbExtensionSubstitution?)> xverExtensions,
        string fhirDir)
    {
        List<XverPackageIndexInfo> infos = [];

        DbFhirPackage sourcePackage = packageSupports[focusPackageIndex].Package;

        foreach (PackageXverSupport targetSupport in packageSupports)
        {
            if (targetSupport.Package.Key == sourcePackage.Key)
            {
                continue;
            }

            string packageId = getPackageId(sourcePackage, targetSupport.Package);

            XverPackageIndexInfo indexInfo = new()
            {
                SourcePackageSupport = packageSupports[focusPackageIndex],
                TargetPackageSupport = targetSupport,
                PackageId = packageId,
            };

            infos.Add(indexInfo);

            // build and write the ImplementationGuide resource for the combination package (single source and target)
            {
                string igJson;

                if (targetSupport.Package.FhirVersionShort.StartsWith('4'))
                {
                    igJson = getIgJsonR4(sourcePackage, targetSupport.Package, xverValueSets, xverExtensions, indexInfo);
                }
                else if (targetSupport.Package.FhirVersionShort.StartsWith('5'))
                {
                    igJson = getIgJsonR5(sourcePackage, targetSupport.Package, xverValueSets, xverExtensions, indexInfo);
                }
                else
                {
                    // TODO: Implment DSTU2 and STU3
                    continue;
                }

                string filename = $"ImplementationGuide-{packageId}.json";
                File.WriteAllText(Path.Combine(fhirDir, packageId, "package", filename), igJson);
            }

            // build and write the package.manifest.json file
            {
                string pmJson = $$$"""
                    {
                      "version" : "{{{_crossDefinitionVersion}}}",
                      "fhirVersion" : ["{{{targetSupport.Package.PackageVersion}}}"],
                      "date" : "{{{DateTime.Now.ToString("yyyyMMddHHmmss")}}}",
                      "name" : "{{{packageId}}}",
                      "jurisdiction" : "http://unstats.un.org/unsd/methods/m49/m49.htm#001"
                    }
                    """;

                string filename = "package.manifest.json";
                File.WriteAllText(Path.Combine(fhirDir, packageId, "package", filename), pmJson);
            }

            // build and write the .index.json file
            {
                string indexJson = getIndexJson(sourcePackage, targetSupport.Package, xverValueSets, xverExtensions, indexInfo);
                string filename = ".index.json";
                File.WriteAllText(Path.Combine(fhirDir, packageId, "package", filename), indexJson);
            }

            // build and write the package.json file
            {
                string packageJson = $$$"""
                    {
                        "name" : "{{{packageId}}}",
                        "version" : "{{{_crossDefinitionVersion}}}",
                        "tools-version" : 3,
                        "type" : "IG",
                        "date" : "{{{DateTime.Now.ToString("yyyyMMddHHmmss")}}}",
                        "license" : "CC0-1.0",
                        "canonical" : "http://hl7.org/fhir/uv/xver",
                        "notForPublication" : true,
                        "url" : "http://hl7.org/fhir/uv/xver",
                        "title" : "XVer-{{{sourcePackage.ShortName}}}-{{{targetSupport.Package.ShortName}}}",
                        "description" : "Cross Version Extensions for using FHIR {{{sourcePackage.ShortName}}} in FHIR {{{targetSupport.Package.ShortName}}}",
                        "fhirVersions" : ["{{{targetSupport.Package.PackageVersion}}}"],
                        "dependencies" : {
                            "{{{targetSupport.Package.PackageId}}}" : "{{{targetSupport.Package.PackageVersion}}}",
                            "hl7.terminology.{{{targetSupport.Package.ShortName.ToLowerInvariant()}}}" : "6.3.0",
                            "hl7.fhir.uv.extensions.{{{targetSupport.Package.ShortName.ToLowerInvariant()}}}" : "5.2.0",
                            "hl7.fhir.uv.tools.{{{targetSupport.Package.ShortName.ToLowerInvariant()}}}" : "current"
                        },
                        "author" : "HL7 International / FHIR Infrastructure",
                        "maintainers" : [
                            {
                                "name" : "HL7 International / FHIR Infrastructure",
                                "url" : "http://www.hl7.org/Special/committees/fiwg"
                            }
                        ],
                        "directories" : {
                            "lib" : "package",
                            "doc" : "doc"
                        },
                        "jurisdiction" : "http://unstats.un.org/unsd/methods/m49/m49.htm#001"
                    }
                    """;

                string filename = "package.json";
                File.WriteAllText(Path.Combine(fhirDir, packageId, "package", filename), packageJson);
            }
        }

        return infos;
    }


    /// <summary>
    /// Generates the .index.json content for a cross-version package, listing all FHIR package contents
    /// defined for a specific source-target package combination.
    /// </summary>
    /// <param name="sourcePackage">The source <see cref="DbFhirPackage"/> for the cross-version package.</param>
    /// <param name="targetPackage">The target <see cref="DbFhirPackage"/> for the cross-version package.</param>
    /// <param name="xverValueSets">A dictionary of cross-version <see cref="ValueSet"/>s, keyed by (source ValueSet key, target package id).</param>
    /// <param name="xverExtensions">A dictionary of cross-version <see cref="StructureDefinition"/>s (extensions), keyed by (source element key, target package id).</param>
    /// <param name="indexInfo">The <see cref="XverPackageIndexInfo"/> object to populate with index entries.</param>
    /// <returns>A JSON string representing the .index.json file for the cross-version package.</returns>
    private string getIndexJson(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        Dictionary<(int sourceVsKey, int targetPackageId), ValueSet> xverValueSets,
        Dictionary<(int sourceElementKey, int targetPackageId), (StructureDefinition, DbExtensionSubstitution?)> xverExtensions,
        XverPackageIndexInfo indexInfo)
    {
        // build the list of structures we are defining
        foreach (((int sourceElementKey, int targetPackageId), (StructureDefinition sd, DbExtensionSubstitution? extensionSubstitution)) in xverExtensions)
        {
            if (targetPackageId != targetPackage.Key)
            {
                continue;
            }

            if (extensionSubstitution != null)
            {
                continue;
            }

            indexInfo.IndexStructureJsons.Add($$$"""
                {
                    "filename" : "StructureDefinition-{{{sd.Id}}}.json",
                    "resourceType" : "StructureDefinition",
                    "id" : "{{{sd.Id}}}",
                    "url" : "{{{sd.Url}}}",
                    "version" : "{{{_crossDefinitionVersion}}}",
                    "kind" : "complex-type",
                    "type" : "Extension",
                    "derivation" : "constraint"
                }
                """);
        }

        // build the list of value sets we are defining
        foreach (((int sourceElementKey, int targetPackageId), ValueSet vs) in xverValueSets)
        {
            if (targetPackageId != targetPackage.Key)
            {
                continue;
            }

            indexInfo.IndexValueSetJsons.Add($$$"""
                {
                    "filename" : "ValueSet-{{{vs.Id}}}.json",
                    "resourceType" : "ValueSet",
                    "id" : "{{{vs.Id}}}",
                    "url" : "{{{vs.Url}}}",
                    "version" : "{{{_crossDefinitionVersion}}}"
                }
                """);
        }

        string indexJson = $$$"""
            {
                "index-version" : 2,
                "files" : [
                    {
                        "filename" : "ImplementationGuide-{{{indexInfo.PackageId}}}.json",
                        "resourceType" : "ImplementationGuide",
                        "id" : "{{{indexInfo.PackageId}}}",
                        "url" : "http://hl7.org/fhir/uv/xver/ImplementationGuide/{{{indexInfo.PackageId}}}",
                        "version" : "{{{_crossDefinitionVersion}}}"
                    },
                    {{{string.Join(", ", indexInfo.IndexStructureJsons)}}},
                    {{{string.Join(", ", indexInfo.IndexValueSetJsons)}}}
                ]
            }
            """;

        return indexJson;
    }



    /// <summary>
    /// Generates the .index.json content for a cross-version package, listing all FHIR package contents
    /// defined for a specific source-target package combination.
    /// </summary>
    /// <param name="package">The <see cref="DbFhirPackage"/> representing the package for which the index is generated.</param>
    /// <param name="packageId">The unique package identifier for this cross-version package.</param>
    /// <param name="internalDependencies">A list of internal package dependencies, each as a tuple of package ID and version.</param>
    /// <param name="targetInfos">A list of <see cref="XverPackageIndexInfo"/> objects containing index information for each target package.</param>
    /// <returns>A JSON string representing the .index.json file for the cross-version package.</returns>

    private string getIndexJson(
        DbFhirPackage package,
        string packageId,
        List<(string packageId, string packageVersion)> internalDependencies,
        List<XverPackageIndexInfo> targetInfos)
    {
        if (internalDependencies.Count == 0)
        {
            return $$$"""
            {
                "index-version" : 2,
                "files" : [
                    {
                        "filename" : "ImplementationGuide-{{{packageId}}}.json",
                        "resourceType" : "ImplementationGuide",
                        "id" : "{{{packageId}}}",
                        "url" : "http://hl7.org/fhir/uv/xver/ImplementationGuide/{{{packageId}}}",
                        "version" : "{{{_crossDefinitionVersion}}}"
                    }
                ]
            }
            """;
        }

        return $$$"""
            {
                "index-version" : 2,
                "files" : [
                    {
                        "filename" : "ImplementationGuide-{{{packageId}}}.json",
                        "resourceType" : "ImplementationGuide",
                        "id" : "{{{packageId}}}",
                        "url" : "http://hl7.org/fhir/uv/xver/ImplementationGuide/{{{packageId}}}",
                        "version" : "{{{_crossDefinitionVersion}}}"
                    },
                    {{{string.Join(", ", targetInfos.SelectMany(ii => ii.IndexStructureJsons))}}},
                    {{{string.Join(", ", targetInfos.SelectMany(ii => ii.IndexValueSetJsons))}}}
                ]
            }
            """;
    }

    private string getIgJsonR5(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        Dictionary<(int sourceVsKey, int targetPackageId), ValueSet> xverValueSets,
        Dictionary<(int sourceElementKey, int targetPackageId), (StructureDefinition, DbExtensionSubstitution?)> xverExtensions,
        XverPackageIndexInfo indexInfo)
    {
        ImplementationGuide ig = new()
        {
            Id = "ImplementationGuide-" + indexInfo.PackageId,
            Extension = [
                new()
                {
                    Url = "http://hl7.org/fhir/StructureDefinition/structuredefinition-standards-status",
                    Value = new Code("trial-use"),
                },
                new()
                {
                    Url = "http://hl7.org/fhir/StructureDefinition/structuredefinition-wg",
                    Value = new Code("fhir"),
                }
            ],
            Url = $"http://hl7.org/fhir/uv/xver/ImplementationGuide/{indexInfo.PackageId}",
            Version = _crossDefinitionVersion,
            Name = $"XVer_{sourcePackage.ShortName.ToLowerInvariant()}_{targetPackage.ShortName.ToLowerInvariant()}",
            Title = $"XVer-{sourcePackage.ShortName}-{targetPackage.ShortName}",
            Status = PublicationStatus.Active,
            Date = "2025-05-19T00:00:00+00:00",
            Publisher = "HL7 International / FHIR Infrastructure",
            Contact = [
                new()
                {
                    Name = "HL7 International / FHIR Infrastructure",
                    Telecom = [
                        new()
                        {
                            System = ContactPoint.ContactPointSystem.Url,
                            Value = "http://www.hl7.org/Special/committees/fiwg",
                        },
                    ],
                }
            ],
            Description = $"Cross Version Extensions for using FHIR {sourcePackage.ShortName} in FHIR {targetPackage.ShortName}",
            Jurisdiction = [
                new()
                {
                    Coding = [
                        new()
                        {
                            System = "http://unstats.un.org/unsd/methods/m49/m49.htm",
                            Code = "001",
                            Display = "World",
                        }
                    ],
                }
            ],
            PackageId = indexInfo.PackageId,
            License = ImplementationGuide.SPDXLicense.CC01_0,
            FhirVersion = [FHIRVersion.N5_0_0],
            DependsOn = [
                new()
                {
                    ElementId = "hl7tx",
                    Uri = "http://terminology.hl7.org/ImplementationGuide/hl7.terminology",
                    PackageId = "hl7.terminology.r5",
                    Version = "6.3.0",
                    Extension = [
                        new()
                        {
                            Url = "http://hl7.org/fhir/tools/StructureDefinition/implementationguide-dependency-comment",
                            Value = new Markdown("Automatically added as a dependency - all IGs depend on HL7 Terminology"),
                        },
                    ],
                },
                new()
                {
                    ElementId = "hl7_fhir_uv_extensions",
                    Uri = "http://hl7.org/fhir/extensions/ImplementationGuide/hl7.fhir.uv.extensions",
                    PackageId = "hl7.fhir.uv.extensions.r5",
                    Version = "5.2.0",
                },
                new()
                {
                    ElementId = "hl7_fhir_uv_tools",
                    Uri = "http://hl7.org/fhir/tools/ImplementationGuide/hl7.fhir.uv.tools",
                    PackageId = "hl7.fhir.uv.tools.r5",
                    Version = "current",
                },
            ],
            Definition = new()
            {
                Resource = [],
            }
        };

        // add our structures
        foreach (((int sourceElementKey, int targetPackageId), (StructureDefinition sd, DbExtensionSubstitution? extensionSubstitution)) in xverExtensions)
        {
            if (targetPackageId != targetPackage.Key)
            {
                continue;
            }

            if (extensionSubstitution != null)
            {
                continue;
            }

            indexInfo.IgStructures.Add(new()
            {
                Reference = new ResourceReference($"StructureDefinition/{sd.Id}"),
                Name = sd.Name,
                Description = sd.Description,
                Extension = [
                    new()
                    {
                        Url = "http://hl7.org/fhir/tools/StructureDefinition/resource-information",
                        Value = new FhirString("StructureDefinition:extension"),
                    },
                ],
            });
        }

        ig.Definition.Resource.AddRange(indexInfo.IgStructures);

        // add our value sets
        foreach (((int sourceElementKey, int targetPackageId), ValueSet vs) in xverValueSets)
        {
            if (targetPackageId != targetPackage.Key)
            {
                continue;
            }

            indexInfo.IgValueSets.Add(new()
            {
                Reference = new ResourceReference($"StructureDefinition/{vs.Id}"),
                Name = vs.Name,
                Description = vs.Description,
                Extension = [
                    new()
                    {
                        Url = "http://hl7.org/fhir/tools/StructureDefinition/resource-information",
                        Value = new FhirString("ValueSet"),
                    },
                ],
            });
        }

        ig.Definition.Resource.AddRange(indexInfo.IgValueSets);

        return ig.ToJson(new FhirJsonSerializationSettings() { Pretty = true });
    }


    private string getIgJsonR5(
        DbFhirPackage package,
        string packageId,
        List<(string packageId, string packageVersion)> internalDependencies,
        List<XverPackageIndexInfo> targetInfos)
    {
        ImplementationGuide ig = new()
        {
            Id = "ImplementationGuide-" + packageId,
            Extension = [
                new()
                {
                    Url = "http://hl7.org/fhir/StructureDefinition/structuredefinition-standards-status",
                    Value = new Code("trial-use"),
                },
                new()
                {
                    Url = "http://hl7.org/fhir/StructureDefinition/structuredefinition-wg",
                    Value = new Code("fhir"),
                }
            ],
            Url = $"http://hl7.org/fhir/uv/xver/ImplementationGuide/{packageId}",
            Version = _crossDefinitionVersion,
            Name = $"XVer_{package.ShortName.ToLowerInvariant()}",
            Title = $"XVer-{package.ShortName}",
            Status = PublicationStatus.Active,
            Date = "2025-05-19T00:00:00+00:00",
            Publisher = "HL7 International / FHIR Infrastructure",
            Contact = [
                new()
                {
                    Name = "HL7 International / FHIR Infrastructure",
                    Telecom = [
                        new()
                        {
                            System = ContactPoint.ContactPointSystem.Url,
                            Value = "http://www.hl7.org/Special/committees/fiwg",
                        },
                    ],
                }
            ],
            Description = $"All Cross Version Extensions for FHIR {package.ShortName}",
            Jurisdiction = [
                new()
                {
                    Coding = [
                        new()
                        {
                            System = "http://unstats.un.org/unsd/methods/m49/m49.htm",
                            Code = "001",
                            Display = "World",
                        }
                    ],
                }
            ],
            PackageId = packageId,
            License = ImplementationGuide.SPDXLicense.CC01_0,
            FhirVersion = [FHIRVersion.N5_0_0],
            DependsOn = [
                new()
                {
                    ElementId = "hl7tx",
                    Uri = "http://terminology.hl7.org/ImplementationGuide/hl7.terminology",
                    PackageId = "hl7.terminology.r5",
                    Version = "6.3.0",
                    Extension = [
                        new()
                        {
                            Url = "http://hl7.org/fhir/tools/StructureDefinition/implementationguide-dependency-comment",
                            Value = new Markdown("Automatically added as a dependency - all IGs depend on HL7 Terminology"),
                        },
                    ],
                },
                new()
                {
                    ElementId = "hl7_fhir_uv_extensions",
                    Uri = "http://hl7.org/fhir/extensions/ImplementationGuide/hl7.fhir.uv.extensions",
                    PackageId = "hl7.fhir.uv.extensions.r5",
                    Version = "5.2.0",
                },
                new()
                {
                    ElementId = "hl7_fhir_uv_tools",
                    Uri = "http://hl7.org/fhir/tools/ImplementationGuide/hl7.fhir.uv.tools",
                    PackageId = "hl7.fhir.uv.tools.r5",
                    Version = "current",
                },
            ],
            Definition = new()
            {
                Resource = [],
            }
        };

        if (internalDependencies.Count == 0)
        {
            ig.Definition = new() { Resource = [] };
            ig.Definition.Resource.AddRange(targetInfos.SelectMany(ii => ii.IgStructures));
            ig.Definition.Resource.AddRange(targetInfos.SelectMany(ii => ii.IgValueSets));
        }
        else
        {
            foreach ((string depPackageId, string depPackageVersion) in internalDependencies)
            {
                ig.DependsOn.Add(new()
                {
                    ElementId = depPackageId.Replace('.', '_'),
                    Uri = $"http://hl7.org/fhir/uv/xver/ImplementationGuide/{depPackageId}",
                    PackageId = depPackageId,
                    Version = depPackageVersion
                });
            }
        }

        return ig.ToJson(new FhirJsonSerializationSettings() { Pretty = true });
    }



    private string getIgJsonR4(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        Dictionary<(int sourceVsKey, int targetPackageId), ValueSet> xverValueSets,
        Dictionary<(int sourceElementKey, int targetPackageId), (StructureDefinition, DbExtensionSubstitution?)> xverExtensions,
        XverPackageIndexInfo indexInfo)
    {
        // build the list of structures we are defining
        foreach (((int sourceElementKey, int targetPackageId), (StructureDefinition sd, DbExtensionSubstitution? extensionSubstitution)) in xverExtensions)
        {
            if (targetPackageId != targetPackage.Key)
            {
                continue;
            }

            if (extensionSubstitution != null)
            {
                continue;
            }

            indexInfo.IgStructureJsons.Add($$$"""
                {
                    "extension" : [{
                        "url" : "http://hl7.org/fhir/tools/StructureDefinition/resource-information",
                        "valueString" : "StructureDefinition:extension"
                    }],
                    "reference" : {
                        "reference" : "StructureDefinition/{{{sd.Id}}}"
                    },
                    "name" : "{{{sd.Name}}}",
                    "description" : "{{{sd.Description}}}"
                }
                """);
        }

        // build the list of value sets we are defining
        foreach (((int sourceElementKey, int targetPackageId), ValueSet vs) in xverValueSets)
        {
            if (targetPackageId != targetPackage.Key)
            {
                continue;
            }

            indexInfo.IgValueSetJsons.Add($$$"""
                {
                    "extension" : [{
                        "url" : "http://hl7.org/fhir/tools/StructureDefinition/resource-information",
                        "valueString" : "ValueSet"
                    }],
                    "reference" : {
                        "reference" : "ValueSet/{{{vs.Id}}}"
                    },
                    "name" : "{{{vs.Name}}}",
                    "description" : "{{{vs.Description}}}"
                }
                """);
        }

        string igJson = $$$"""
            {
              "resourceType" : "ImplementationGuide",
              "id" : "ImplementationGiude-{{{indexInfo.PackageId}}}",
              "extension" : [{
                "url" : "http://hl7.org/fhir/StructureDefinition/structuredefinition-standards-status",
                "valueCode" : "trial-use"
              },
              {
                "url" : "http://hl7.org/fhir/StructureDefinition/structuredefinition-wg",
                "valueCode" : "fhir"
              }],
              "url" : "http://hl7.org/fhir/uv/xver/ImplementationGuide/{{{indexInfo.PackageId}}}",
              "version" : "{{{_crossDefinitionVersion}}}",
              "name" : "XVer_{{{sourcePackage.ShortName.ToLowerInvariant()}}}_{{{targetPackage.ShortName.ToLowerInvariant()}}}",
              "title" : "XVer-{{{sourcePackage.ShortName}}}-{{{targetPackage.ShortName}}}",
              "status" : "active",
              "date" : "2025-05-19T00:00:00+00:00",
              "publisher" : "HL7 International / FHIR Infrastructure",
              "contact" : [{
                "name" : "HL7 International / FHIR Infrastructure",
                "telecom" : [{
                  "system" : "url",
                  "value" : "http://www.hl7.org/Special/committees/fiwg"
                }]
              }],
              "description" : "Cross Version Extensions for using FHIR {{{sourcePackage.ShortName}}} in FHIR {{{targetPackage.ShortName}}}",
              "jurisdiction" : [{
                "coding" : [{
                  "system" : "http://unstats.un.org/unsd/methods/m49/m49.htm",
                  "code" : "001",
                  "display" : "World"
                }]
              }],
              "packageId" : "{{{indexInfo.PackageId}}}",
              "license" : "CC0-1.0",
              "fhirVersion" : ["{{{targetPackage.PackageVersion}}}"],
              "dependsOn" : [{
                "id" : "hl7tx",
                "extension" : [{
                  "url" : "http://hl7.org/fhir/tools/StructureDefinition/implementationguide-dependency-comment",
                  "valueMarkdown" : "Automatically added as a dependency - all IGs depend on HL7 Terminology"
                }],
                "uri" : "http://terminology.hl7.org/ImplementationGuide/hl7.terminology",
                "packageId" : "hl7.terminology.{{{targetPackage.ShortName.ToLowerInvariant()}}}",
                "version" : "6.3.0"
              },
              {
                "id" : "hl7_fhir_uv_extensions",
                "uri" : "http://hl7.org/fhir/extensions/ImplementationGuide/hl7.fhir.uv.extensions",
                "packageId" : "hl7.fhir.uv.extensions.{{{targetPackage.ShortName.ToLowerInvariant()}}}",
                "version" : "5.2.0"
              },
              {
                "id" : "hl7_fhir_uv_tools",
                "uri" : "http://hl7.org/fhir/tools/ImplementationGuide/hl7.fhir.uv.tools",
                "packageId" : "hl7.fhir.uv.tools.{{{targetPackage.ShortName.ToLowerInvariant()}}}",
                "version" : "current"
              }],
              "definition" : {
                "resource" : [
                {{{string.Join(", ", indexInfo.IgStructureJsons)}}},
                {{{string.Join(", ", indexInfo.IgValueSetJsons)}}}]
              }
            }
            """;

        return igJson;
    }

    private string getIgJsonR4(
        DbFhirPackage package,
        string packageId,
        List<(string packageId, string packageVersion)> internalDependencies,
        List<XverPackageIndexInfo> targetInfos)
    {
        string additionalDependencies;
        string resources;

        if (internalDependencies.Count == 0)
        {
            additionalDependencies = string.Empty;
            resources = $$$"""
                ,
                  "definition" : {
                    "resource" : [
                    {{{string.Join(", ", targetInfos.SelectMany(ii => ii.IgStructureJsons))}}},
                    {{{string.Join(", ", targetInfos.SelectMany(ii => ii.IgValueSetJsons))}}}]
                  }
                """;
        }
        else
        {
            additionalDependencies = "," + string.Join(
                ",",
                internalDependencies.Select(pi => $$$"""
                    {
                        "id" : "{{{pi.packageId.Replace('.', '_')}}}",
                        "uri" : "http://hl7.org/fhir/uv/xver/ImplementationGuide/{{{packageId}}}",
                        "packageId" : "{{{pi.packageId}}}",
                        "version" : "{{{pi.packageVersion}}}"
                    }
                """)
                );
            resources = string.Empty;
        }

        string igJson = $$$"""
            {
              "resourceType" : "ImplementationGuide",
              "id" : "{{{packageId}}}",
              "extension" : [{
                "url" : "http://hl7.org/fhir/StructureDefinition/structuredefinition-standards-status",
                "valueCode" : "trial-use"
              },
              {
                "url" : "http://hl7.org/fhir/StructureDefinition/structuredefinition-wg",
                "valueCode" : "fhir"
              }],
              "url" : "http://hl7.org/fhir/uv/xver/ImplementationGuide/{{{packageId}}}",
              "version" : "{{{_crossDefinitionVersion}}}",
              "name" : "XVer_{{{package.ShortName.ToLowerInvariant()}}}",
              "title" : "XVer-{{{package.ShortName}}}",
              "status" : "active",
              "date" : "2025-05-19T00:00:00+00:00",
              "publisher" : "HL7 International / FHIR Infrastructure",
              "contact" : [{
                "name" : "HL7 International / FHIR Infrastructure",
                "telecom" : [{
                  "system" : "url",
                  "value" : "http://www.hl7.org/Special/committees/fiwg"
                }]
              }],
              "description" : "All Cross Version Extensions for for FHIR {{{package.ShortName}}}",
              "jurisdiction" : [{
                "coding" : [{
                  "system" : "http://unstats.un.org/unsd/methods/m49/m49.htm",
                  "code" : "001",
                  "display" : "World"
                }]
              }],
              "packageId" : "{{{packageId}}}",
              "license" : "CC0-1.0",
              "fhirVersion" : ["{{{package.PackageVersion}}}"],
              "dependsOn" : [{
                "id" : "hl7tx",
                "extension" : [{
                  "url" : "http://hl7.org/fhir/tools/StructureDefinition/implementationguide-dependency-comment",
                  "valueMarkdown" : "Automatically added as a dependency - all IGs depend on HL7 Terminology"
                }],
                "uri" : "http://terminology.hl7.org/ImplementationGuide/hl7.terminology",
                "packageId" : "hl7.terminology.{{{package.ShortName.ToLowerInvariant()}}}",
                "version" : "6.3.0"
              },
              {
                "id" : "hl7_fhir_uv_extensions",
                "uri" : "http://hl7.org/fhir/extensions/ImplementationGuide/hl7.fhir.uv.extensions",
                "packageId" : "hl7.fhir.uv.extensions.{{{package.ShortName.ToLowerInvariant()}}}",
                "version" : "5.2.0"
              },
              {
                "id" : "hl7_fhir_uv_tools",
                "uri" : "http://hl7.org/fhir/tools/ImplementationGuide/hl7.fhir.uv.tools",
                "packageId" : "hl7.fhir.uv.tools.{{{package.ShortName.ToLowerInvariant()}}}",
                "version" : "current"
              }{{{additionalDependencies}}}
              ]{{{resources}}}
            }
            """;

        return igJson;
    }

}
