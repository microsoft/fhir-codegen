using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Health.Fhir.SpecManager.Models;
using fhir_3 = Microsoft.Health.Fhir.SpecManager.fhir.v3;

namespace Microsoft.Health.Fhir.SpecManager.Manager
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>A class to Load a FHIR v4 (R4) Specification</summary>
    ///
    /// <remarks>Gino Canessa, 2/3/2020.</remarks>
    ///-------------------------------------------------------------------------------------------------

    public class LoaderV3
    {
        #region Class Constants . . .

        /// <summary>Name of the package release.</summary>
        public const string PackageReleaseName = "STU3";
        /// <summary>Name of the package.</summary>
        public const string PackageName = "hl7.fhir.r3.core";
        /// <summary>The path based on manual downloading of the package.</summary>
        public const string PathManual = "hl7.fhir.r3.core/package";
        /// <summary>The path based on installing the package using NPM.</summary>
        public const string PathNpm = "node_modules/hl7.fhir.r3.core";

        #endregion Class Constants . . .
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

        static LoaderV3()
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
        /// <summary>Searches for the v3 package.</summary>
        ///
        /// <remarks>Gino Canessa, 2/12/2020.</remarks>
        ///
        /// <param name="npmDirectory">    Pathname of the npm directory.</param>
        /// <param name="versionDirectory">[out] Pathname of the version directory.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        ///-------------------------------------------------------------------------------------------------

        public static bool TryFindPackage(string npmDirectory, out string versionDirectory)
        {
            versionDirectory = null;

            // **** check for manual download first ****

            string packageDir = Path.Combine(npmDirectory, PathManual);

            if (!Directory.Exists(packageDir))
            {
                // **** check for npm install directory ****

                packageDir = Path.Combine(npmDirectory, PathNpm);

                if (!Directory.Exists(packageDir))
                {
                    Console.WriteLine($"TryFindPackage <<< cannot find v3 package ({PathManual} or {PathNpm})!");
                    return false;
                }
            }

            // **** set our directory ****

            versionDirectory = packageDir;
            return true;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Function to load an STU3 spec NPM into an InfoV3 structure.</summary>
        ///
        /// <remarks>Gino Canessa, 2/3/2020.</remarks>
        ///
        /// <param name="npmDirectory">Pathname of the npm directory.</param>
        /// <param name="fhirInfo">    [out] The FHIR dictionary.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        ///-------------------------------------------------------------------------------------------------

        public static bool LoadPackage(string npmDirectory, out InfoV3 fhirInfo)
        {
            fhirInfo = null;

            // **** find the package ****

            if (!TryFindPackage(npmDirectory, out string packageDir))
            {
                Console.WriteLine($"LoadPackage <<< cannot find v3 package!");
                return false;
            }

            // **** load package info ****

            if (!FhirPackageInfo.TryLoadPackageInfo(packageDir, out FhirPackageInfo packageInfo))
            {
                Console.WriteLine($"LoadPackage <<< Failed to load v3 package info, dir: {packageDir}");
                return false;
            }

            // **** tell the user what's going on ****

            Console.WriteLine($"LoadPackage <<< Found: {packageInfo.Name} version: {packageInfo.Version}");

            // **** far enough along to create our info structure ****

            fhirInfo = new InfoV3();

            // **** get the files in this directory ****

            string[] files = Directory.GetFiles(packageDir, "*.json", SearchOption.TopDirectoryOnly);

            // **** grab a converter for polymorphic deserialization ****

            fhir_3.ResourceConverter converter = new fhir_3.ResourceConverter();

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
                    Console.Write($"v3: {shortName,-85}\r");

                    // **** read the file ****

                    string contents = File.ReadAllText(filename);

                    // **** parse the file into something v3 (note: var is ~10% faster than dynamic here) ****

                    var obj = JsonConvert.DeserializeObject<fhir_3.Resource>(
                        contents,
                        converter
                        );

                    // **** check for excluded type ****

                    if (InfoV3.IsResourceTypeExcluded(resourceHint))
                    {
                        // **** skip ****

                        continue;
                    }

                    // **** check for a dictionary for this type ****

                    if (!InfoV3.IsResourceTypeKnown(resourceHint))
                    {
                        // **** type not found ****

                        Console.WriteLine($"\nLoadFhirV3 <<< Unhandled type: {shortName}, parsed to:{obj.GetType().Name}");
                        return false;
                    }

                    // **** act depending on type ****

                    switch (obj)
                    {
                        case fhir_3.CapabilityStatement capabilityStatement:

                            // **** validate it parsed to what it should ****

                            if (!resourceHint.Equals("CapabilityStatement", StringComparison.Ordinal))
                            {
                                Console.WriteLine($"LoadPackage <<< Wrong type! {shortName} parsed as {capabilityStatement.ResourceType,-80}");
                                return false;
                            }

                            // **** add to the correct dictionary ****

                            fhirInfo.Capabilities.Add(capabilityStatement.Id, capabilityStatement);

                            break;

                        case fhir_3.CodeSystem codeSystem:

                            // **** validate it parsed to what it should ****

                            if (!resourceHint.Equals("CodeSystem", StringComparison.Ordinal))
                            {
                                Console.WriteLine($"LoadPackage <<< Wrong type! {shortName} parsed as {codeSystem.ResourceType,-80}");
                                return false;
                            }

                            // **** add to the correct dictionary ****

                            fhirInfo.CodeSystems.Add(codeSystem.Id, codeSystem);

                            break;

                        case fhir_3.CompartmentDefinition compartmentDefinition:

                            // **** validate it parsed to what it should ****

                            if (!resourceHint.Equals("CompartmentDefinition", StringComparison.Ordinal))
                            {
                                Console.WriteLine($"LoadPackage <<< Wrong type! {shortName} parsed as {compartmentDefinition.ResourceType,-80}");
                                return false;
                            }

                            // **** add to the correct dictionary ****

                            fhirInfo.CompartmentDefinitions.Add(compartmentDefinition.Id, compartmentDefinition);

                            break;

                        case fhir_3.ConceptMap conceptMap:

                            // **** validate it parsed to what it should ****

                            if (!resourceHint.Equals("ConceptMap", StringComparison.Ordinal))
                            {
                                Console.WriteLine($"LoadPackage <<< Wrong type! {shortName} parsed as {conceptMap.ResourceType,-80}");
                                return false;
                            }

                            // **** add to the correct dictionary ****

                            fhirInfo.ConceptMaps.Add(conceptMap.Id, conceptMap);

                            break;

                            // TODO: Add support for ImplementationGuide

                        case fhir_3.NamingSystem namingSystem:

                            // **** validate it parsed to what it should ****

                            if (!resourceHint.Equals("NamingSystem", StringComparison.Ordinal))
                            {
                                Console.WriteLine($"LoadPackage <<< Wrong type! {shortName} parsed as {namingSystem.ResourceType,-80}");
                                return false;
                            }

                            // **** add to the correct dictionary ****

                            fhirInfo.NamingSystems.Add(namingSystem.Id, namingSystem);

                            break;

                        case fhir_3.OperationDefinition operationDefinition:

                            // **** validate it parsed to what it should ****

                            if (!resourceHint.Equals("OperationDefinition", StringComparison.Ordinal))
                            {
                                Console.WriteLine($"LoadPackage <<< Wrong type! {shortName} parsed as {operationDefinition.ResourceType,-80}");
                                return false;
                            }

                            // **** add to the correct dictionary ****

                            fhirInfo.OperationDefinitions.Add(operationDefinition.Id, operationDefinition);

                            break;

                        case fhir_3.SearchParameter searchParameter:

                            // **** validate it parsed to what it should ****

                            if (!resourceHint.Equals("SearchParameter", StringComparison.Ordinal))
                            {
                                Console.WriteLine($"LoadPackage <<< Wrong type! {shortName} parsed as {searchParameter.ResourceType,-80}");
                                return false;
                            }

                            // **** add to the correct dictionary ****

                            fhirInfo.SearchParameters.Add(searchParameter.Id, searchParameter);

                            break;

                        case fhir_3.StructureDefinition structureDefinition:

                            // **** validate it parsed to what it should ****

                            if (!resourceHint.Equals("StructureDefinition", StringComparison.Ordinal))
                            {
                                Console.WriteLine($"LoadPackage <<< Wrong type! {shortName} parsed as {structureDefinition.ResourceType,-80}");
                                return false;
                            }

                            // **** add to the correct dictionary ****

                            fhirInfo.StructureDefinitions.Add(structureDefinition.Id, structureDefinition);

                            break;

                        case fhir_3.StructureMap structureMap:

                            // **** validate it parsed to what it should ****

                            if (!resourceHint.Equals("StructureMap", StringComparison.Ordinal))
                            {
                                Console.WriteLine($"LoadPackage <<< Wrong type! {shortName} parsed as {structureMap.ResourceType,-80}");
                                return false;
                            }

                            // **** add to the correct dictionary ****

                            fhirInfo.StructureMaps.Add(structureMap.Id, structureMap);

                            break;

                        case fhir_3.ValueSet valueSet:

                            // **** validate it parsed to what it should ****

                            if (!resourceHint.Equals("ValueSet", StringComparison.Ordinal))
                            {
                                Console.WriteLine($"LoadPackage <<< Wrong type! {shortName} parsed as {valueSet.ResourceType,-80}");
                                return false;
                            }

                            // **** add to the correct dictionary ****

                            fhirInfo.ValueSets.Add(valueSet.Id, valueSet);

                            break;

                        default:
                            Console.WriteLine("");
                            Console.WriteLine($"LoadPackage <<< Unhandled type: {shortName}:{obj.GetType().Name}");
                            return false;
                            //break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("");
                    Console.WriteLine($"LoadPackage <<< Failed to parse file: {filename}: {ex.Message}");
                    return false;
                }
            }

            // **** make sure we cleared the last line ****

            Console.WriteLine($"LoadPackage <<< Loaded and Parsed FHIR STU3!{new string(' ', 100)}");

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
