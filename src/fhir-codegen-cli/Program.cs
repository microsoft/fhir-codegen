using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using CommandLine;
using Microsoft.Health.Fhir.SpecManager.Manager;
using Microsoft.Health.Fhir.SpecManager.Models;

namespace fhir_codegen_cli
{
    class Program
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>Main entry-point for this application.</summary>
        ///
        /// <param name="args">An array of command-line argument strings.</param>
        ///-------------------------------------------------------------------------------------------------

        static void Main(string[] args)
        {
            bool success = false;

            // **** start timing ****

            Stopwatch timingWatch = Stopwatch.StartNew();

            // **** process based on command line arguments ****

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(options => {
                    success = Process(options);
                })
                .WithNotParsed(errors => { Console.WriteLine("Invalid arguments"); });

            // **** done ****

            long elapsedMs = timingWatch.ElapsedMilliseconds;

            Console.WriteLine($"Finished {success}: {elapsedMs / 1000.0} s");
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Main processing function</summary>
        ///
        /// <param name="options">Options for controlling the operation.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        ///-------------------------------------------------------------------------------------------------

        static bool Process(Options options)
        {
            // **** initialize the FHIR version manager with our requested directory ****

            FhirManager.Init(options.NpmDirectory);



            //// **** test downloader ****

            //FhirPackageDownloader downloader = new FhirPackageDownloader();
            //downloader.DownloadPublishedPackage(
            //    LoaderV4.PackageReleaseName,
            //    LoaderV4.PackageName,
            //    options.NpmDirectory
            //    );
            //downloader.CheckInstalledVersions(options.NpmDirectory);


            // **** check for loading V2 ****

            if (options.LoadR2)
            {
                if (!FhirManager.Current.LoadPublished(2, out FhirVersionInfo r2))
                {
                    Console.WriteLine("Loading R2 failed!");
                    return false;
                }

                // **** tell the user what's going on ****

                DumpFhirVersion(r2);
            }


            if (options.LoadR3)
            {
                if (!FhirManager.Current.LoadPublished(3, out FhirVersionInfo r3))
                {
                    Console.WriteLine("Loading R3 failed!");
                    return false;
                }

                // **** tell the user what's going on ****

                DumpFhirVersion(r3);
            }

            if (options.LoadR4)
            {
                if (!FhirManager.Current.LoadPublished(4, out FhirVersionInfo r4))
                {
                    Console.WriteLine("Loading R4 failed!");
                    return false;
                }

                // **** tell the user what's going on ****

                DumpFhirVersion(r4);
            }

            return true;

            //// **** check for loading V3 ****

            //if (options.LoadV3)
            //{
            //    if (!LoaderV3.LoadPackage(options.NpmDirectory, out InfoV3 fhirInfoV3))
            //    {
            //        Console.WriteLine("Loading v3 failed!");
            //        return false;
            //    }
            //}

            //// **** check for loading V4 ****

            //if (options.LoadV4)
            //{
            //    if (!LoaderV4.LoadPackage(options.NpmDirectory, out InfoV4 fhirInfoV4))
            //    {
            //        Console.WriteLine("Loading v4 failed!");
            //        return false;
            //    }
            //}

            //// **** still here means success ****

            //return true;
        }

        static void DumpFhirVersion(FhirVersionInfo info)
        {
            // **** tell the user what's going on ****

            Console.WriteLine($"Found: {info.PackageName} version: {info.VersionString}");

            // **** dump simple types ****

            Console.WriteLine($"simple types: {info.SimpleTypes.Count}");

            foreach (KeyValuePair<string, FhirSimpleType> kvp in info.SimpleTypes)
            {
                string primitiveMarker = (kvp.Value.IsPrimitive) ? "*" : "";
                Console.WriteLine($"- {kvp.Key}{primitiveMarker}: {kvp.Value.BaseTypeName}");
            }

            // **** dump complex types ****

            Console.WriteLine($"complex types: {info.ComplexTypes.Count}");
            DumpComplex<FhirComplexType>(info.ComplexTypes);

            // **** dump resources ****

            Console.WriteLine($"resources: {info.Resources.Count}");
            DumpComplex<FhirResource>(info.Resources);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Dumps a complex structure (complex type/resource and properties)</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dict">The dictionary.</param>
        ///-------------------------------------------------------------------------------------------------

        private static void DumpComplex<T>(Dictionary<string, T> dict)
            where T : FhirTypeBase
        {
            foreach (KeyValuePair<string, T> kvp in dict)
            {
                Console.WriteLine($"- {kvp.Key}: {kvp.Value.BaseTypeName}");
                foreach (KeyValuePair<string, FhirProperty> propKvp in kvp.Value.Properties)
                {
                    string max = (propKvp.Value.CardinaltiyMax == null) ? "*" : propKvp.Value.CardinaltiyMax.ToString();

                    string propertyType = propKvp.Value.BaseTypeName;

                    if (propKvp.Value.ExpandedTypes != null)
                    {
                        foreach (string expandedType in propKvp.Value.ExpandedTypes)
                        {
                            if (string.IsNullOrEmpty(propertyType))
                            {
                                propertyType = expandedType;
                                continue;
                            }

                            propertyType = $"{propertyType}|{expandedType}";
                        }
                    }

                    Console.WriteLine($"  - {propKvp.Value.Name}: {propertyType}" +
                        $" ({propKvp.Value.CardinalityMin}" +
                        $".." +
                        $"{max})");
                }
            }
        }

    }
}
