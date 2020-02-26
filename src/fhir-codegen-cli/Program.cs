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
        /// <summary>Main entry-point for this application.</summary>
        /// <param name="args">An array of command-line argument strings.</param>
        static void Main(string[] args)
        {
            bool success = false;

            // start timing

            Stopwatch timingWatch = Stopwatch.StartNew();

            // process based on command line arguments

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(options => {
                    success = Process(options);
                })
                .WithNotParsed(errors => { Console.WriteLine("Invalid arguments"); });

            // done

            long elapsedMs = timingWatch.ElapsedMilliseconds;

            Console.WriteLine($"Finished {success}: {elapsedMs / 1000.0} s");
        }

        /// <summary>Main processing function.</summary>
        /// <param name="options">Options for controlling the operation.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        static bool Process(Options options)
        {
            // initialize the FHIR version manager with our requested directory
            FhirManager.Init(options.NpmDirectory);

            if (options.LoadR2)
            {
                if (!FhirManager.Current.LoadPublished(2, out FhirVersionInfo r2))
                {
                    Console.WriteLine("Loading R2 failed!");
                    return false;
                }

                // tell the user what's going on
                if (options.Verbose)
                {
                    DumpFhirVersion(r2);
                }
            }

            if (options.LoadR3)
            {
                if (!FhirManager.Current.LoadPublished(3, out FhirVersionInfo r3))
                {
                    Console.WriteLine("Loading R3 failed!");
                    return false;
                }

                // tell the user what's going on
                if (options.Verbose)
                {
                    DumpFhirVersion(r3);
                }
            }

            if (options.LoadR4)
            {
                if (!FhirManager.Current.LoadPublished(4, out FhirVersionInfo r4))
                {
                    Console.WriteLine("Loading R4 failed!");
                    return false;
                }

                // tell the user what's going on
                if (options.Verbose)
                {
                    DumpFhirVersion(r4);
                }
            }

            return true;
        }

        /// <summary>Dumps information about a FHIR version to the console.</summary>
        /// <param name="info">The information.</param>
        static void DumpFhirVersion(FhirVersionInfo info)
        {
            // tell the user what's going on

            Console.WriteLine($"Found: {info.PackageName} version: {info.VersionString}");

            // dump primitive types

            Console.WriteLine($"primitive types: {info.PrimitiveTypes.Count}");

            foreach (KeyValuePair<string, FhirPrimitive> kvp in info.PrimitiveTypes)
            {
                Console.WriteLine($"- {kvp.Key}: {kvp.Value.BaseTypeName}");
            }

            // dump complex types
            Console.WriteLine($"complex types: {info.ComplexTypes.Count}");
            DumpComplex(info.ComplexTypes);

            //// dump resources
            //Console.WriteLine($"resources: {info.Resources.Count}");
            //DumpComplex(info.Resources);
        }

        /// <summary>Dumps a complex structure (complex type/resource and properties)</summary>
        /// <param name="dict">The dictionary.</param>
        private static void DumpComplex(Dictionary<string, FhirComplex> dict)
        {
            foreach (KeyValuePair<string, FhirComplex> kvp in dict)
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
