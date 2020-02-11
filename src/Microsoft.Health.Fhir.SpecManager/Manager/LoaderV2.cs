using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Health.Fhir.SpecManager.Models;
using fhir_2 = Microsoft.Health.Fhir.SpecManager.fhir.v2;

namespace Microsoft.Health.Fhir.SpecManager.Manager
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>A class to Load a FHIR v4 (R4) Specification</summary>
    ///
    /// <remarks>Gino Canessa, 2/3/2020.</remarks>
    ///-------------------------------------------------------------------------------------------------

    public class LoaderV2
    {
        #region Class Variables . . .

        /// <summary>Filenames to exclude when loading a package.</summary>
        private static HashSet<string> _packageExclusions;

        #endregion Class Variables . . .

        #region Instance Variables . . .

        #endregion Instance Variables . . .

        #region Constructors . . .

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Static constructor.</summary>
        ///
        /// <remarks>Gino Canessa, 2/3/2020.</remarks>
        ///-------------------------------------------------------------------------------------------------

        static LoaderV2()
        {
            _packageExclusions = new HashSet<string>()
            {
                ".index.json",
                "package.json"
            };
        }

        #endregion Constructors . . .

        #region Class Interface . . .

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Function to load a DSTU2 spec NPM into an InfoV2 structure.</summary>
        ///
        /// <remarks>Gino Canessa, 2/3/2020.</remarks>
        ///
        /// <param name="npmDirectory">Pathname of the npm directory.</param>
        /// <param name="fhirDict">      [out] The FHIR dictionary</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        ///-------------------------------------------------------------------------------------------------

        public static bool LoadFhirV2(string npmDirectory, out InfoV2 fhirInfo)
        {
            fhirInfo = null;

            // **** build the path to the package ****

            string dirV2 = Path.Combine(
                npmDirectory,
                "hl7.fhir.r2.core",
                "package"
                );

            // **** make sure the directory exists ****

            if (!Directory.Exists(dirV2))
            {
                Console.WriteLine($"LoadFhirV2 failed, cannot find v2 directory: {dirV2}");
                return false;
            }

            // **** load package info ****

            if (!FhirPackageInfo.TryLoadPackageInfo(dirV2, out FhirPackageInfo packageInfo))
            {
                Console.WriteLine($"Failed to load v2 package info, dir: {dirV2}");
                return false;
            }

            // **** tell the user what's going on ****

            Console.WriteLine($"Found: {packageInfo.Name} version: {packageInfo.Version}");

            // **** far enough along to create our info structure ****

            fhirInfo = new InfoV2();

            // **** get the files in this directory ****

            string[] files = Directory.GetFiles(dirV2, "*.json", SearchOption.TopDirectoryOnly);

            // **** grab a converter for polymorphic deserialization ****

            fhir_2.ResourceConverter converter = new fhir_2.ResourceConverter();

            // **** traverse the files ****

            foreach (string filename in files)
            {
                // **** check for file exclusion ****

                if (_packageExclusions.Contains(Path.GetFileName(filename)))
                {
                    // **** skip this file ****

                    continue;
                }

                // **** parse the name into parts we want ****

                string shortName = Path.GetFileNameWithoutExtension(filename);
                string resourceHint = shortName.Split('-')[0];
                string resourceName = shortName.Substring(resourceHint.Length + 1);

                // **** attempt to load this file ****

                try
                {
                    Console.Write($"v2: {shortName,-85}\r");

                    // **** read the file ****

                    string contents = File.ReadAllText(filename);

                    // **** parse the file into something v3 (note: var is ~10% faster than dynamic here) ****

                    var obj = JsonConvert.DeserializeObject<fhir_2.Resource>(
                        contents,
                        converter
                        );

                    // **** check for excluded type ****

                    if (InfoV2.IsResourceTypeExcluded(resourceHint))
                    {
                        // **** skip ****

                        continue;
                    }

                    // **** check for a dictionary for this type ****

                    if (!InfoV2.IsResourceTypeKnown(resourceHint))
                    {
                        // **** type not found ****

                        Console.WriteLine($"\nUnhandled type: {shortName}, parsed to:{obj.GetType().Name}");
                        return false;
                    }

                    // **** act depending on type ****

                    switch (obj)
                    {
                        case fhir_2.ConceptMap conceptMap:

                            // **** validate it parsed to what it should ****

                            if (!resourceHint.Equals("ConceptMap", StringComparison.Ordinal))
                            {
                                Console.WriteLine($"Wrong type! {shortName} parsed as {conceptMap.ResourceType,-80}");
                                return false;
                            }

                            // **** add to the correct dictionary ****

                            fhirInfo.ConceptMaps.Add(conceptMap.Id, conceptMap);

                            break;

                        case fhir_2.Conformance conformance:

                            // **** validate it parsed to what it should ****

                            if (!resourceHint.Equals("Conformance", StringComparison.Ordinal))
                            {
                                Console.WriteLine($"Wrong type! {shortName} parsed as {conformance.ResourceType,-80}");
                                return false;
                            }

                            // **** add to the correct dictionary ****

                            fhirInfo.Conformances.Add(conformance.Id, conformance);

                            break;

                        case fhir_2.NamingSystem namingSystem:

                            // **** validate it parsed to what it should ****

                            if (!resourceHint.Equals("NamingSystem", StringComparison.Ordinal))
                            {
                                Console.WriteLine($"Wrong type! {shortName} parsed as {namingSystem.ResourceType,-80}");
                                return false;
                            }

                            // **** add to the correct dictionary ****

                            fhirInfo.NamingSystems.Add(namingSystem.Id, namingSystem);

                            break;

                        case fhir_2.OperationDefinition operationDefinition:

                            // **** validate it parsed to what it should ****

                            if (!resourceHint.Equals("OperationDefinition", StringComparison.Ordinal))
                            {
                                Console.WriteLine($"Wrong type! {shortName} parsed as {operationDefinition.ResourceType,-80}");
                                return false;
                            }

                            // **** add to the correct dictionary ****

                            fhirInfo.OperationDefinitions.Add(operationDefinition.Id, operationDefinition);

                            break;

                        case fhir_2.SearchParameter searchParameter:

                            // **** validate it parsed to what it should ****

                            if (!resourceHint.Equals("SearchParameter", StringComparison.Ordinal))
                            {
                                Console.WriteLine($"Wrong type! {shortName} parsed as {searchParameter.ResourceType,-80}");
                                return false;
                            }

                            // **** add to the correct dictionary ****

                            fhirInfo.SearchParameters.Add(searchParameter.Id, searchParameter);

                            break;

                        case fhir_2.StructureDefinition structureDefinition:

                            // **** validate it parsed to what it should ****

                            if (!resourceHint.Equals("StructureDefinition", StringComparison.Ordinal))
                            {
                                Console.WriteLine($"Wrong type! {shortName} parsed as {structureDefinition.ResourceType,-80}");
                                return false;
                            }

                            // **** add to the correct dictionary ****

                            fhirInfo.StructureDefinitions.Add(structureDefinition.Id, structureDefinition);

                            break;

                        case fhir_2.ValueSet valueSet:

                            // **** validate it parsed to what it should ****

                            if (!resourceHint.Equals("ValueSet", StringComparison.Ordinal))
                            {
                                Console.WriteLine($"Wrong type! {shortName} parsed as {valueSet.ResourceType,-80}");
                                return false;
                            }

                            // **** add to the correct dictionary ****

                            fhirInfo.ValueSets.Add(valueSet.Id, valueSet);

                            break;

                        default:
                            Console.WriteLine("");
                            Console.WriteLine($"Unhandled type: {shortName}:{obj.GetType().Name}");
                            return false;
                            //break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("");
                    Console.WriteLine($"Failed to parse file: {filename}: {ex.Message}");
                    return false;
                }
            }

            // **** make sure we cleared the last line ****

            Console.WriteLine($"Loaded and Parsed FHIR DSTU2!{new string(' ', 100)}");

            // **** still here means success ****

            return true;
        }

        #endregion Class Interface . . .

        #region Instance Interface . . .

        #endregion Instance Interface . . .

        #region Internal Functions . . .

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Creates the Resource dictionary needed to load FHIR R4.</summary>
        ///
        /// <remarks>Gino Canessa, 2/3/2020.</remarks>
        ///
        /// <param name="dict">[out] The dictionary.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        ///-------------------------------------------------------------------------------------------------

        //private static bool CreateDictV4(out Dictionary<string, dynamic> dict)
        //{
        //    // **** make a new object ****

        //    dict = new Dictionary<string, dynamic>();

        //    // **** add types we should parse ****

        //    dict.Add("CapabilityStatement", new Dictionary<string, fhir_4.CapabilityStatement>());
        //    dict.Add("CodeSystem", new Dictionary<string, fhir_4.CodeSystem>());
        //    dict.Add("CompartmentDefinition", new Dictionary<string, fhir_4.CompartmentDefinition>());
        //    dict.Add("ConceptMap", new Dictionary<string, fhir_4.ConceptMap>());
        //    dict.Add("NamingSystem", new Dictionary<string, fhir_4.NamingSystem>());
        //    dict.Add("OperationDefinition", new Dictionary<string, fhir_4.OperationDefinition>());
        //    dict.Add("SearchParameter", new Dictionary<string, fhir_4.SearchParameter>());
        //    dict.Add("StructureDefinition", new Dictionary<string, fhir_4.StructureDefinition>());
        //    dict.Add("StructureMap", new Dictionary<string, fhir_4.StructureMap>());
        //    dict.Add("ValueSet", new Dictionary<string, fhir_4.ValueSet>());

        //    return true;
        //}

        #endregion Internal Functions . . .

    }
}
