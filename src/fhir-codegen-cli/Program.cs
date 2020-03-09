// <copyright file="Program.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Health.Fhir.SpecManager.fhir.r2;
using Microsoft.Health.Fhir.SpecManager.Manager;
using Microsoft.Health.Fhir.SpecManager.Models;

namespace FhirCodegenCli
{
    /// <summary>FHIR CodeGen CLI.</summary>
    public static class Program
    {
        /// <summary>Main entry-point for this application.</summary>
        /// <param name="fhirSpecDirectory">The full path to the directory where FHIR specifications are.</param>
        /// <param name="outputFile">       An array of command-line argument strings.</param>
        /// <param name="verbose">          Show verbose output.</param>
        /// <param name="offlineMode">      Offline mode (will not download missing specs).</param>
        /// <param name="loadR2">           If FHIR R2 should be loaded, which version (e.g., 1.0.2 or latest).</param>
        /// <param name="loadR3">           If FHIR R3 should be loaded, which version (e.g., 3.0.2 or latest).</param>
        /// <param name="loadR4">           If FHIR R4 should be loaded, which version (e.g., 4.0.1 or latest).</param>
        public static void Main(
            string fhirSpecDirectory,
            string outputFile = "",
            bool verbose = false,
            bool offlineMode = false,
            string loadR2 = "",
            string loadR3 = "",
            string loadR4 = "")
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

            if (string.IsNullOrEmpty(outputFile))
            {
                if ((verbose == true) && (r2 != null))
                {
                    DumpFhirVersion(Console.Out, r2);
                }

                if ((verbose == true) && (r3 != null))
                {
                    DumpFhirVersion(Console.Out, r3);
                }

                if ((verbose == true) && (r4 != null))
                {
                    DumpFhirVersion(Console.Out, r4);
                }
            }

            if (!string.IsNullOrEmpty(outputFile))
            {
                if (!Directory.Exists(Path.GetDirectoryName(outputFile)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(outputFile));
                }

                using (StreamWriter writer = new StreamWriter(new FileStream(outputFile, FileMode.Create)))
                {
                    if (r2 != null)
                    {
                        DumpFhirVersion(writer, r2);
                    }

                    if (r3 != null)
                    {
                        DumpFhirVersion(writer, r3);
                    }

                    if (r4 != null)
                    {
                        DumpFhirVersion(writer, r4);
                    }
                }
            }

            // done
            long totalMS = timingWatch.ElapsedMilliseconds;

            Console.WriteLine($"Done! Loading: {loadMS / 1000.0}s, Total: {totalMS / 1000.0}s");
        }

        /// <summary>Main processing function.</summary>
        /// <param name="fhirSpecDirectory">The full path to the directory where FHIR specifications are.</param>
        /// <param name="offlineMode">      Offline mode (will not download missing specs).</param>
        /// <param name="loadR2">           If FHIR R2 should be loaded, which version (e.g., 1.0.2 or latest).</param>
        /// <param name="r2">               [out] The FhirVersionInfo for R2 (if loaded).</param>
        /// <param name="loadR3">           If FHIR R3 should be loaded, which version (e.g., 3.0.2 or latest).</param>
        /// <param name="r3">               [out] The FhirVersionInfo for R3 (if loaded).</param>
        /// <param name="loadR4">           If FHIR R4 should be loaded, which version (e.g., 4.0.1 or latest).</param>
        /// <param name="r4">               [out] The FhirVersionInfo for R4 (if loaded).</param>
        public static void Process(
            string fhirSpecDirectory,
            bool offlineMode,
            string loadR2,
            out FhirVersionInfo r2,
            string loadR3,
            out FhirVersionInfo r3,
            string loadR4,
            out FhirVersionInfo r4)
        {
            // initialize the FHIR version manager with our requested directory
            FhirManager.Init(fhirSpecDirectory);

            r2 = null;
            r3 = null;
            r4 = null;

            if (!string.IsNullOrEmpty(loadR2))
            {
                try
                {
                    r2 = FhirManager.Current.LoadPublished(2, loadR2, offlineMode);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Loading R2 ({loadR2}) failed: {ex}");
                    throw;
                }
            }

            if (!string.IsNullOrEmpty(loadR3))
            {
                try
                {
                    r3 = FhirManager.Current.LoadPublished(3, loadR3, offlineMode);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Loading R3 ({loadR3}) failed: {ex}");
                    throw;
                }
            }

            if (!string.IsNullOrEmpty(loadR4))
            {
                try
                {
                    r4 = FhirManager.Current.LoadPublished(4, loadR4, offlineMode);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Loading R4 ({loadR4}) failed: {ex}");
                    throw;
                }
            }
        }

        /// <summary>Dumps information about a FHIR version to the console.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="info">  The information.</param>
        private static void DumpFhirVersion(TextWriter writer, FhirVersionInfo info)
        {
            // tell the user what's going on
            writer.WriteLine($"Contents of: {info.PackageName} version: {info.VersionString}");

            // dump primitive types
            writer.WriteLine($"primitive types: {info.PrimitiveTypes.Count}");

            foreach (FhirPrimitive primitive in info.PrimitiveTypes.Values)
            {
                writer.WriteLine($"- {primitive.Name}: {primitive.BaseTypeName}");

                // check for extensions
                if (primitive.Extensions != null)
                {
                    DumpExtensions(writer, primitive.Extensions.Values, 0);
                }
            }

            // dump complex types
            writer.WriteLine($"complex types: {info.ComplexTypes.Count}");
            DumpComplexDict(writer, info.ComplexTypes);

            // dump resources
            writer.WriteLine($"resources: {info.Resources.Count}");
            DumpComplexDict(writer, info.Resources);

            // dump server level operations
            writer.WriteLine($"system operations: {info.SystemOperations.Count}");
            DumpOperations(writer, info.SystemOperations.Values, 0, true);

            // dump magic search parameters - all resource parameters
            writer.WriteLine($"all resource parameters: {info.AllResourceParameters.Count}");
            DumpSearchParameters(writer, info.AllResourceParameters.Values, 0);

            // dump magic search parameters - search result parameters
            writer.WriteLine($"search result parameters: {info.SearchResultParameters.Count}");
            DumpSearchParameters(writer, info.SearchResultParameters.Values, 0);

            // dump magic search parameters - search result parameters
            writer.WriteLine($"all interaction parameters: {info.AllInteractionParameters.Count}");
            DumpSearchParameters(writer, info.AllInteractionParameters.Values, 0);
        }

        /// <summary>Dumps a complex structure (complex type/resource and properties).</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="dict">  The dictionary.</param>
        private static void DumpComplexDict(TextWriter writer, Dictionary<string, FhirComplex> dict)
        {
            foreach (KeyValuePair<string, FhirComplex> kvp in dict)
            {
                DumpComplex(writer, kvp.Value);
            }
        }

        /// <summary>Dumps a complex element.</summary>
        /// <param name="writer">     The writer.</param>
        /// <param name="complex">    The complex.</param>
        /// <param name="indentation">(Optional) The indentation.</param>
        private static void DumpComplex(TextWriter writer, FhirComplex complex, int indentation = 0)
        {
            // write this type's line, if it's a root element
            // (sub-properties are written with cardinality in the prior loop)
            if (indentation == 0)
            {
                writer.WriteLine($"{new string(' ', indentation)}- {complex.Name}: {complex.BaseTypeName}");
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

                writer.WriteLine($"{new string(' ', indentation + 2)}- {element.Name}" +
                    $"[{element.CardinalityMin}..{max}]" +
                    $": {propertyType}");

                // check for extensions
                if (element.Extensions != null)
                {
                    DumpExtensions(writer, element.Extensions.Values, indentation + 2);
                }

                // check for an inline component definition
                if (complex.Components.ContainsKey(element.Path))
                {
                    // recurse into this definition
                    DumpComplex(writer, complex.Components[element.Path], indentation + 2);
                }

                // check for slices
                if (element.Slicing != null)
                {
                    foreach (FhirSlicing slicing in element.Slicing.Values)
                    {
                        writer.WriteLine($"{new string(' ', indentation + 4)}: {slicing.DefinedByUrl} ({slicing.SlicingRules})");
                        foreach (FhirComplex slice in slicing.Slices.Values)
                        {
                            writer.WriteLine($"{new string(' ', indentation + 4)}: {element.Name}:{slice.Name}");
                            DumpComplex(writer, slice, indentation + 4);
                        }
                    }
                }
            }

            // check for extensions
            if (complex.Extensions != null)
            {
                DumpExtensions(writer, complex.Extensions.Values, indentation);
            }

            // dump search parameters
            if (complex.SearchParameters != null)
            {
                DumpSearchParameters(writer, complex.SearchParameters.Values, indentation);
            }

            // dump type operations
            if (complex.TypeOperations != null)
            {
                DumpOperations(writer, complex.TypeOperations.Values, indentation, true);
            }

            // dump instance operations
            if (complex.InstanceOperations != null)
            {
                DumpOperations(writer, complex.InstanceOperations.Values, indentation, false);
            }
        }

        /// <summary>Dumps the extensions.</summary>
        /// <param name="writer">     The writer.</param>
        /// <param name="extensions"> The extensions.</param>
        /// <param name="indentation">The indentation.</param>
        private static void DumpExtensions(TextWriter writer, IEnumerable<FhirComplex> extensions, int indentation)
        {
            foreach (FhirComplex extension in extensions)
            {
                DumpExtension(writer, extension, indentation);
            }
        }

        /// <summary>Dumps an extension.</summary>
        /// <param name="writer">     The writer.</param>
        /// <param name="extension">  The extension.</param>
        /// <param name="indentation">The indentation.</param>
        private static void DumpExtension(TextWriter writer, FhirComplex extension, int indentation)
        {
            writer.WriteLine($"{new string(' ', indentation + 2)}+{extension.URL}");

            if (extension.Elements.Count > 0)
            {
                DumpComplex(writer, extension, indentation + 2);
            }
        }

        /// <summary>Dumps a search parameters.</summary>
        /// <param name="writer">     The writer.</param>
        /// <param name="parameters"> Options for controlling the operation.</param>
        /// <param name="indentation">The indentation.</param>
        private static void DumpSearchParameters(
            TextWriter writer,
            IEnumerable<FhirSearchParam> parameters,
            int indentation)
        {
            foreach (FhirSearchParam searchParam in parameters)
            {
                writer.WriteLine($"{new string(' ', indentation + 2)}" +
                    $"?{searchParam.Code}" +
                    $"={searchParam.ValueType}");
            }
        }

        /// <summary>Dumps the operations.</summary>
        /// <param name="writer">     The writer.</param>
        /// <param name="operations"> The operations.</param>
        /// <param name="indentation">The indentation.</param>
        /// <param name="isTypeLevel">True if is type level, false if not.</param>
        private static void DumpOperations(
            TextWriter writer,
            IEnumerable<FhirOperation> operations,
            int indentation,
            bool isTypeLevel)
        {
            foreach (FhirOperation operation in operations)
            {
                if (isTypeLevel)
                {
                    writer.WriteLine($"{new string(' ', indentation + 2)}${operation.Code}");
                }
                else
                {
                    writer.WriteLine($"{new string(' ', indentation + 2)}/{{id}}${operation.Code}");
                }

                if (operation.Parameters != null)
                {
                    foreach (FhirParameter parameter in operation.Parameters.OrderBy(p => p.FieldOrder))
                    {
                        string max = (parameter.Max == null) ? "*" : parameter.Max.ToString();
                        writer.WriteLine($"{new string(' ', indentation + 4)}" +
                            $"{parameter.Use}: {parameter.Name} ({parameter.Min}-{max})");
                    }
                }
            }
        }
    }
}
