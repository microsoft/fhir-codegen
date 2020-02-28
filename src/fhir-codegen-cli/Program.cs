// <copyright file="Program.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CommandLine;
using Microsoft.Health.Fhir.SpecManager.Manager;
using Microsoft.Health.Fhir.SpecManager.Models;

namespace FhirCodegenCli
{
    /// <summary>FHIR CodeGen CLI.</summary>
    public static class Program
    {
        /// <summary>Main entry-point for this application.</summary>
        /// <param name="args">An array of command-line argument strings.</param>
        public static void Main(string[] args)
        {
            bool success = false;

            // start timing
            Stopwatch timingWatch = Stopwatch.StartNew();

            // process based on command line arguments
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(options => { Process(options); })
                .WithNotParsed(errors => { Console.WriteLine("Invalid arguments"); });

            // done
            long elapsedMs = timingWatch.ElapsedMilliseconds;

            Console.WriteLine($"Finished {success}: {elapsedMs / 1000.0} s");
        }

        /// <summary>Main processing function.</summary>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <param name="options">Options for controlling the operation.</param>
        public static void Process(Options options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            // initialize the FHIR version manager with our requested directory
            FhirManager.Init(options.NpmDirectory);

            if (options.LoadR2)
            {
                try
                {
                    FhirVersionInfo r2 = FhirManager.Current.LoadPublished(2);

                    if (options.Verbose)
                    {
                        DumpFhirVersion(r2);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Loading R2 failed: {ex}");
                    throw;
                }
            }

            if (options.LoadR3)
            {
                try
                {
                    FhirVersionInfo r3 = FhirManager.Current.LoadPublished(3);

                    if (options.Verbose)
                    {
                        DumpFhirVersion(r3);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Loading R3 failed: {ex}");
                    throw;
                }
            }

            if (options.LoadR4)
            {
                try
                {
                    FhirVersionInfo r4 = FhirManager.Current.LoadPublished(4);

                    if (options.Verbose)
                    {
                        DumpFhirVersion(r4);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Loading R4 failed: {ex}");
                    throw;
                }
            }
        }

        /// <summary>Dumps information about a FHIR version to the console.</summary>
        /// <param name="info">The information.</param>
        private static void DumpFhirVersion(FhirVersionInfo info)
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
            DumpComplexDict(info.ComplexTypes);

            // dump resources
            Console.WriteLine($"resources: {info.Resources.Count}");
            DumpComplexDict(info.Resources);
        }

        /// <summary>Dumps a complex structure (complex type/resource and properties).</summary>
        /// <param name="dict">The dictionary.</param>
        private static void DumpComplexDict(Dictionary<string, FhirComplex> dict)
        {
            foreach (KeyValuePair<string, FhirComplex> kvp in dict)
            {
                DumpComplexElement(kvp.Value);
            }
        }

        /// <summary>Dumps a complex element.</summary>
        /// <param name="complex">    The complex.</param>
        /// <param name="indentation">(Optional) The indentation.</param>
        private static void DumpComplexElement(FhirComplex complex, int indentation = 0)
        {
            // write this type's line, if it's a root element
            // (sub-properties are written with cardinality in the prior loop)
            if (indentation == 0)
            {
                Console.WriteLine($"{new string(' ', indentation)}- {complex.Name}: {complex.BaseTypeName}");
            }

            // traverse properties for this type
            foreach (KeyValuePair<string, FhirProperty> kvp in complex.Properties.OrderBy(s => s.Value.FieldOrder))
            {
                string max = (kvp.Value.CardinalityMax == null) ? "*" : kvp.Value.CardinalityMax.ToString();

                string propertyType = kvp.Value.BaseTypeName;

                if (kvp.Value.ChoiceTypes != null)
                {
                    foreach (string choiceType in kvp.Value.ChoiceTypes)
                    {
                        if (string.IsNullOrEmpty(propertyType))
                        {
                            propertyType = choiceType;
                            continue;
                        }

                        propertyType = $"{propertyType}|{choiceType}";
                    }
                }

                string profiles = string.Empty;
                if (kvp.Value.TargetProfiles != null)
                {
                    foreach (string profile in kvp.Value.TargetProfiles)
                    {
                        if (string.IsNullOrEmpty(profiles))
                        {
                            profiles = profile;
                            continue;
                        }

                        profiles = $"{profiles}|{profile}";
                    }
                }

                if (!string.IsNullOrEmpty(profiles))
                {
                    propertyType = $"{propertyType}({profiles})";
                }

                Console.WriteLine($"{new string(' ', indentation + 2)}- {kvp.Value.Name}: {propertyType}" +
                    $" ({kvp.Value.CardinalityMin}" +
                    $".." +
                    $"{max})");

                // check for an inline component definition
                if (complex.Components.ContainsKey(kvp.Value.Path))
                {
                    // recurse into this definition
                    DumpComplexElement(complex.Components[kvp.Value.Path], indentation + 2);
                }
            }

            // dump search parameters
            if (complex.SearchParameters != null)
            {
                foreach (FhirSearchParam searchParam in complex.SearchParameters.Values)
                {
                    Console.WriteLine($"{new string(' ', indentation + 2)}?{searchParam.Code} ({searchParam.ValueType})");
                }
            }
        }
    }
}
