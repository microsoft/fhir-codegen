// <copyright file="Program.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Health.Fhir.SpecManager.Manager;
using Microsoft.Health.Fhir.SpecManager.Models;

namespace FhirCodegenCli
{
    /// <summary>FHIR CodeGen CLI.</summary>
    public static class Program
    {
        /// <summary>Main entry-point for this application.</summary>
        /// <param name="fhirSpecDirectory">The full path to the directory where FHIR specifications are.</param>
        /// <param name="verbose">          Show verbose output.</param>
        /// <param name="offlineMode">      Offline mode (will not download missing specs).</param>
        /// <param name="loadR2">           Whether FHIR R2 should be loaded.</param>
        /// <param name="loadR3">           Whether FHIR R3 should be loaded.</param>
        /// <param name="loadR4">           Whether FHIR R4 should be loaded.</param>
        public static void Main(
            string fhirSpecDirectory,
            bool verbose = false,
            bool offlineMode = false,
            bool loadR2 = false,
            bool loadR3 = false,
            bool loadR4 = false)
        {
            // start timing
            Stopwatch timingWatch = Stopwatch.StartNew();

            // process
            Process(
                fhirSpecDirectory,
                offlineMode,
                loadR2,
                out FhirVersionInfo r2,
                loadR3,
                out FhirVersionInfo r3,
                loadR4,
                out FhirVersionInfo r4);

            // done loading
            long loadMS = timingWatch.ElapsedMilliseconds;

            if ((verbose == true) && (r2 != null))
            {
                DumpFhirVersion(r2);
            }

            if ((verbose == true) && (r3 != null))
            {
                DumpFhirVersion(r3);
            }

            if ((verbose == true) && (r4 != null))
            {
                DumpFhirVersion(r4);
            }

            // done
            long totalMS = timingWatch.ElapsedMilliseconds;

            Console.WriteLine($"Done! Loading: {loadMS / 1000.0}s, Total: {totalMS / 1000.0}s");
        }

        /// <summary>Main processing function.</summary>
        /// <param name="fhirSpecDirectory">The full path to the directory where FHIR specifications are.</param>
        /// <param name="offlineMode">      Offline mode (will not download missing specs).</param>
        /// <param name="loadR2">           Whether FHIR R2 should be loaded.</param>
        /// <param name="r2">               [out] The FhirVersionInfo for R2 (if loaded).</param>
        /// <param name="loadR3">           Whether FHIR R3 should be loaded.</param>
        /// <param name="r3">               [out] The FhirVersionInfo for R3 (if loaded).</param>
        /// <param name="loadR4">           Whether FHIR R4 should be loaded.</param>
        /// <param name="r4">               [out] The FhirVersionInfo for R4 (if loaded).</param>
        public static void Process(
            string fhirSpecDirectory,
            bool offlineMode,
            bool loadR2,
            out FhirVersionInfo r2,
            bool loadR3,
            out FhirVersionInfo r3,
            bool loadR4,
            out FhirVersionInfo r4)
        {
            // initialize the FHIR version manager with our requested directory
            FhirManager.Init(fhirSpecDirectory);

            r2 = null;
            r3 = null;
            r4 = null;

            if (loadR2)
            {
                try
                {
                    r2 = FhirManager.Current.LoadPublished(2, offlineMode);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Loading R2 failed: {ex}");
                    throw;
                }
            }

            if (loadR3)
            {
                try
                {
                    r3 = FhirManager.Current.LoadPublished(3, offlineMode);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Loading R3 failed: {ex}");
                    throw;
                }
            }

            if (loadR4)
            {
                try
                {
                    r4 = FhirManager.Current.LoadPublished(4, offlineMode);
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

            foreach (FhirPrimitive primitive in info.PrimitiveTypes.Values)
            {
                Console.WriteLine($"- {primitive.Name}: {primitive.BaseTypeName}");

                // check for extensions
                if (primitive.Extensions != null)
                {
                    DumpExtensions(primitive.Extensions.Values, 0);
                }
            }

            // dump complex types
            Console.WriteLine($"complex types: {info.ComplexTypes.Count}");
            DumpComplexDict(info.ComplexTypes);

            // dump resources
            Console.WriteLine($"resources: {info.Resources.Count}");
            DumpComplexDict(info.Resources);

            // dump server level operations
            Console.WriteLine($"system operations: {info.SystemOperations.Count}");
            DumpOperations(info.SystemOperations.Values, 0, true);

            // dump magic search parameters - all resource parameters
            Console.WriteLine($"all resource parameters: {info.AllResourceParameters.Count}");
            DumpSearchParameters(info.AllResourceParameters.Values, 0);

            // dump magic search parameters - search result parameters
            Console.WriteLine($"search result parameters: {info.SearchResultParameters.Count}");
            DumpSearchParameters(info.SearchResultParameters.Values, 0);

            // dump magic search parameters - search result parameters
            Console.WriteLine($"all interaction parameters: {info.AllInteractionParameters.Count}");
            DumpSearchParameters(info.AllInteractionParameters.Values, 0);
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
            foreach (FhirElement element in complex.Elements.Values.OrderBy(s => s.FieldOrder))
            {
                string max = (element.CardinalityMax == null) ? "*" : element.CardinalityMax.ToString();

                string propertyType = string.Empty;

                if (element.ElementTypes != null)
                {
                    foreach (FhirElementType elementType in element.ElementTypes.Values)
                    {
                        string joiner = string.IsNullOrEmpty(propertyType) ? string.Empty : "|";

                        string profiles = string.Empty;
                        if ((elementType.Profiles != null) && (elementType.Profiles.Count > 0))
                        {
                            profiles = "(" + string.Join('|', elementType.Profiles.Values) + ")";
                        }

                        propertyType = $"{propertyType}{joiner}{elementType.Code}{profiles}";
                    }
                }

                if (string.IsNullOrEmpty(propertyType))
                {
                    propertyType = element.BaseTypeName;
                }

                Console.WriteLine($"{new string(' ', indentation + 2)}- {element.Name}" +
                    $"[{element.CardinalityMin}..{max}]" +
                    $": {propertyType}");

                // check for extensions
                if (element.Extensions != null)
                {
                    DumpExtensions(element.Extensions.Values, indentation + 2);
                }

                // check for an inline component definition
                if (complex.Components.ContainsKey(element.Path))
                {
                    // recurse into this definition
                    DumpComplexElement(complex.Components[element.Path], indentation + 2);
                }
            }

            // check for extensions
            if (complex.Extensions != null)
            {
                DumpExtensions(complex.Extensions.Values, indentation);
            }

            // dump search parameters
            if (complex.SearchParameters != null)
            {
                DumpSearchParameters(complex.SearchParameters.Values, indentation);
            }

            // dump type operations
            if (complex.TypeOperations != null)
            {
                DumpOperations(complex.TypeOperations.Values, indentation, true);
            }

            // dump instance operations
            if (complex.InstanceOperations != null)
            {
                DumpOperations(complex.InstanceOperations.Values, indentation, false);
            }
        }

        private static void DumpExtensions(IEnumerable<FhirComplex> extensions, int indentation)
        {
            foreach (FhirComplex extension in extensions)
            {
                string typesAndProfiles = string.Empty;
                string joiner = string.Empty;

                //foreach (KeyValuePair<string, List<string>> kvp in extension.AllowedTypesAndProfiles)
                //{
                //    string profiles = string.Join('|', kvp.Value);

                //    if (string.IsNullOrEmpty(typesAndProfiles))
                //    {
                //        joiner = string.Empty;
                //    }
                //    else
                //    {
                //        joiner = "|";
                //    }

                //    if (string.IsNullOrEmpty(profiles))
                //    {
                //        typesAndProfiles = $"{typesAndProfiles}{joiner}{kvp.Key}";
                //    }
                //    else
                //    {
                //        typesAndProfiles = $"{typesAndProfiles}{joiner}{kvp.Key}({profiles})";
                //    }
                //}

                Console.WriteLine($"{new string(' ', indentation + 2)}" +
                    $"+{extension.URL}: {typesAndProfiles}");
            }
        }

        /// <summary>Dumps a search parameters.</summary>
        /// <param name="parameters"> Options for controlling the operation.</param>
        /// <param name="indentation">The indentation.</param>
        private static void DumpSearchParameters(IEnumerable<FhirSearchParam> parameters, int indentation)
        {
            foreach (FhirSearchParam searchParam in parameters)
            {
                Console.WriteLine($"{new string(' ', indentation + 2)}" +
                    $"?{searchParam.Code}" +
                    $"={searchParam.ValueType}");
            }
        }

        /// <summary>Dumps the operations.</summary>
        /// <param name="operations"> The operations.</param>
        /// <param name="indentation">The indentation.</param>
        /// <param name="isTypeLevel">True if is type level, false if not.</param>
        private static void DumpOperations(IEnumerable<FhirOperation> operations, int indentation, bool isTypeLevel)
        {
            foreach (FhirOperation operation in operations)
            {
                if (isTypeLevel)
                {
                    Console.WriteLine($"{new string(' ', indentation + 2)}${operation.Code}");
                }
                else
                {
                    Console.WriteLine($"{new string(' ', indentation + 2)}/{{id}}${operation.Code}");
                }

                if (operation.Parameters != null)
                {
                    foreach (FhirParameter parameter in operation.Parameters.OrderBy(p => p.FieldOrder))
                    {
                        string max = (parameter.Max == null) ? "*" : parameter.Max.ToString();
                        Console.WriteLine($"{new string(' ', indentation + 4)}" +
                            $"{parameter.Use}: {parameter.Name} ({parameter.Min}-{max})");
                    }
                }
            }
        }
    }
}
