using Microsoft.Health.Fhir.SpecManager.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using fhir_2 = Microsoft.Health.Fhir.SpecManager.fhir.v2;
using fhir_3 = Microsoft.Health.Fhir.SpecManager.fhir.v3;
using fhir_4 = Microsoft.Health.Fhir.SpecManager.fhir.v4;
using fhir_5 = Microsoft.Health.Fhir.SpecManager.fhir.v4;

namespace Microsoft.Health.Fhir.SpecManager.Manager
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>Information about a FHIR release.</summary>
    ///
    /// <remarks>Gino Canessa, 2/14/2020.</remarks>
    ///-------------------------------------------------------------------------------------------------

    public class FhirVersionInfo
    {
        #region Class Constants . . .

        private const string _urlJsonType = "http://hl7.org/fhir/StructureDefinition/structuredefinition-json-type";
        private const string _urlXmlType = "http://hl7.org/fhir/StructureDefinition/structuredefinition-xml-type";

        #endregion Class Constants . . .

        #region Class Variables . . .

        private static HashSet<int> _knownVersionNumbers;

        private static Dictionary<int, HashSet<string>> _versionResourcesToProcess;
        private static Dictionary<int, HashSet<string>> _versionResourcesToIgnore;
        private static Dictionary<int, HashSet<string>> _versionFilesToIgnore;
        
        #endregion Class Variables . . .

        #region Instance Variables . . .

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the major version.</summary>
        ///
        /// <value>The major version.</value>
        ///-------------------------------------------------------------------------------------------------

        public int MajorVersion { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the name of the package release.</summary>
        ///
        /// <value>The name of the package release.</value>
        ///-------------------------------------------------------------------------------------------------

        public string ReleaseName { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the name of the package.</summary>
        ///
        /// <value>The name of the package.</value>
        ///-------------------------------------------------------------------------------------------------

        public string PackageName { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the version string.</summary>
        ///
        /// <value>The version string.</value>
        ///-------------------------------------------------------------------------------------------------

        public string VersionString { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets a value indicating whether this object is development build.</summary>
        ///
        /// <value>True if this object is development build, false if not.</value>
        ///-------------------------------------------------------------------------------------------------

        public bool IsDevBuild { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the development branch.</summary>
        ///
        /// <value>The development branch.</value>
        ///-------------------------------------------------------------------------------------------------

        public string DevBranch { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets a value indicating whether this object is local build.</summary>
        ///
        /// <value>True if this object is local build, false if not.</value>
        ///-------------------------------------------------------------------------------------------------

        public bool IsLocalBuild { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the pathname of the local directory.</summary>
        ///
        /// <value>The pathname of the local directory.</value>
        ///-------------------------------------------------------------------------------------------------

        public string LocalDirectory { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets a value indicating whether this object is on disk.</summary>
        ///
        /// <value>True if available, false if not.</value>
        ///-------------------------------------------------------------------------------------------------

        public bool IsOnDisk { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the Date/Time of the last downloaded.</summary>
        ///
        /// <value>The last downloaded.</value>
        ///-------------------------------------------------------------------------------------------------

        public DateTime? LastDownloaded { get; set; }

        /// <summary>The JSON converter for polymorphic deserialization of this version of FHIR.</summary>
        private JsonConverter _jsonConverter;

        Dictionary<string, FhirSimpleType> _simpleTypes;
        Dictionary<string, FhirComplexType> _complexTypes;
        Dictionary<string, FhirResource> _resources;
        Dictionary<string, FhirCapability> _capabilities;
        
        #endregion Instance Variables . . .

        #region Constructors . . .

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Static constructor.</summary>
        ///
        /// <remarks>Gino Canessa, 2/18/2020.</remarks>
        ///-------------------------------------------------------------------------------------------------

        static FhirVersionInfo()
        {
            _knownVersionNumbers = new HashSet<int>()
            {
                2,
                3,
                4,
                5
            };

            _versionResourcesToProcess = new Dictionary<int, HashSet<string>>()
            {
                {
                    2,
                    new HashSet<string>()
                    {
                        "Conformance",
                        "NamingSystem",
                        "OperationDefinition",
                        "SearchParameter",
                        "StructureDefinition",
                        "ValueSet",
                    }
                },
                {
                    3,
                    new HashSet<string>()
                    {
                        "CapabilityStatement",
                        "CodeSystem",
                        "NamingSystem",
                        "OperationDefinition",
                        "SearchParameter",
                        "StructureDefinition",
                        "ValueSet",
                    }
                },
                {
                    4,
                    new HashSet<string>()
                    {
                        "CapabilityStatement",
                        "CodeSystem",
                        "NamingSystem",
                        "OperationDefinition",
                        "SearchParameter",
                        "StructureDefinition",
                        "ValueSet",
                    }
                },
                {
                    5,
                    new HashSet<string>()
                    {
                        "CapabilityStatement",
                        "CodeSystem",
                        "NamingSystem",
                        "OperationDefinition",
                        "SearchParameter",
                        "StructureDefinition",
                        "ValueSet",
                    }
                },
            };

            _versionResourcesToIgnore = new Dictionary<int, HashSet<string>>()
            {
                {
                    2,
                    new HashSet<string>()
                    {
                        "ConceptMap",
                        "ImplementationGuide",
                    }
                },
                {
                    3,
                    new HashSet<string>()
                    {
                        "CompartmentDefinition",
                        "ConceptMap",
                        "ImplementationGuide",
                        "StructureMap",
                    }
                },
                {
                    4,
                    new HashSet<string>()
                    {
                        "CompartmentDefinition",
                        "ConceptMap",
                        "StructureMap",
                    }
                },
                {
                    5,
                    new HashSet<string>()
                    {
                        "CompartmentDefinition",
                        "ConceptMap",
                        "StructureMap",
                    }
                },
            };

            _versionFilesToIgnore = new Dictionary<int, HashSet<string>>()
            {
                {
                    2,
                    new HashSet<string>()
                    {
                        ".index.json",
                        "package.json"
                    }
                },
                {
                    3,
                    new HashSet<string>()
                    {
                        ".index.json",
                        "package.json"
                    }
                },
                {
                    4,
                    new HashSet<string>()
                    {
                        ".index.json",
                        "package.json"
                    }
                },
                {
                    5,
                    new HashSet<string>()
                    {
                        ".index.json",
                        "package.json"
                    }
                },
            };
        }

        public FhirVersionInfo(int majorVersion)
        {
            if (!_knownVersionNumbers.Contains(majorVersion))
            {
                throw new Exception($"Invalid FHIR major version: {majorVersion}!");
            }

            // **** copy required fields ****

            MajorVersion = majorVersion;

            // **** create our JSON converter ****

            switch (majorVersion)
            {
                case 2: _jsonConverter = new fhir_2.ResourceConverter(); break;
                case 3: _jsonConverter = new fhir_3.ResourceConverter(); break;
                case 4: _jsonConverter = new fhir_4.ResourceConverter(); break;
                case 5: _jsonConverter = new fhir_5.ResourceConverter(); break;
            }

            // **** create our info dictionaries ****

            _simpleTypes = new Dictionary<string, FhirSimpleType>();
            _complexTypes = new Dictionary<string, FhirComplexType>();
            _resources = new Dictionary<string, FhirResource>();
            _capabilities = new Dictionary<string, FhirCapability>();
        }

        #endregion Constructors . . .

        #region Class Interface . . .

        #endregion Class Interface . . .

        #region Instance Interface . . .

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Determine if we should process resource.</summary>
        ///
        /// <remarks>Gino Canessa, 2/18/2020.</remarks>
        ///
        /// <param name="resourceName"> Name of the resource.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        ///-------------------------------------------------------------------------------------------------

        public bool ShouldProcessResource(string resourceName)
        {
            if (_versionResourcesToProcess[MajorVersion].Contains(resourceName))
            {
                return true;
            }

            return false;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Determine if we should ignore resource.</summary>
        ///
        /// <remarks>Gino Canessa, 2/18/2020.</remarks>
        ///
        /// <param name="resourceName"> Name of the resource.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        ///-------------------------------------------------------------------------------------------------

        public bool ShouldIgnoreResource(string resourceName)
        {
            if (_versionResourcesToIgnore[MajorVersion].Contains(resourceName))
            {
                return true;
            }

            return false;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Determine if we should skip file.</summary>
        ///
        /// <remarks>Gino Canessa, 2/18/2020.</remarks>
        ///
        /// <param name="filename">Filename of the file.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        ///-------------------------------------------------------------------------------------------------

        public bool ShouldSkipFile(string filename)
        {
            if (_versionFilesToIgnore[MajorVersion].Contains(filename))
            {
                return true;
            }

            return false;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Attempts to parse resource an object from the given string.</summary>
        ///
        /// <remarks>Gino Canessa, 2/18/2020.</remarks>
        ///
        /// <param name="json">The JSON.</param>
        /// <param name="obj"> [out] The object.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        ///-------------------------------------------------------------------------------------------------

        public bool TryParseResource(string json, out object obj)
        {
            try
            {
                // **** act depending on FHIR version ****
                switch (MajorVersion)
                {
                    case 2:
                        obj = JsonConvert.DeserializeObject<fhir_2.Resource>(json, _jsonConverter);
                        return true;
                        //break;
                    case 3:
                        obj = JsonConvert.DeserializeObject<fhir_3.Resource>(json, _jsonConverter);
                        return true;
                        //break;
                    case 4:
                        obj = JsonConvert.DeserializeObject<fhir_4.Resource>(json, _jsonConverter);
                        return true;
                        //break;
                    case 5:
                        obj = JsonConvert.DeserializeObject<fhir_5.Resource>(json, _jsonConverter);
                        return true;
                        //break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TryParseResource <<< exception: \n{ex}\n------------------------------------");
            }

            obj = null;
            return false;
        }

        public bool ProcessResource(object obj)
        {
            switch (obj)
            {
                case fhir_2.Conformance conformance:
                    // **** ignore for now ****

                    return true;
                    //break;

                case fhir_2.NamingSystem namingSystem:
                    // **** ignore for now ****

                    return true;
                //break;

                case fhir_2.OperationDefinition operationDefinition:
                    // **** ignore for now ****

                    return true;
                //break;

                case fhir_2.SearchParameter searchParameter:
                    // **** ignore for now ****

                    return true;
                //break;

                case fhir_2.StructureDefinition structureDefinition:
                    return ProcessStructureDefV2(structureDefinition);
                    //break;

                case fhir_2.ValueSet valueSet:
                    // **** ignore for now ****

                    return true;
                    //break;
            }

            // **** unprocessed ****

            return false;
        }

        #endregion Instance Interface . . .

        #region Internal Functions . . .


        #endregion Internal Functions . . .

        #region V2 Processing. . .

        private bool ProcessStructureDefV2(fhir_2.StructureDefinition sd)
        {
            // **** ignore retired ****

            if (sd.Status.Equals("retired", StringComparison.Ordinal))
            {
                return true;
            }

            // **** act depending on kind ****

            switch (sd.Kind)
            {
                case "datatype":
                    // **** 4 elements is for a simple type, more is for a complex one ****

                    if (sd.Snapshot.Element.Length > 4)
                    {
                        return ProcessDataTypeComplexV2(sd);
                    }

                    return ProcessDataTypeSimpleV2(sd);
                    //break;

                case "resource":
                    break;

                case "logical":
                    // **** ignore logical ****

                    return true;
                    //break;
            }

            // **** here means success ****

            return true;
        }
        
        private bool ProcessDataTypeSimpleV2(fhir_2.StructureDefinition sd)
        {
            // **** create a new Simple Type object ****

            FhirSimpleType simple = new FhirSimpleType()
            {
                Name = sd.Name,
                NameCapitalized = string.Concat(sd.Name.Substring(0, 1).ToUpper(), sd.Name.Substring(1)),
                StandardStatus = sd.Status,
                ShortDescription = sd.Description,
                Definition = sd.Requirements,
            };

            // **** figure out the type ****

            foreach (fhir_2.ElementDefinition element in sd.Snapshot.Element)
            {
                // **** check for {type}.value to find where the actual value is defined ****

                if (element.Path.Equals($"{sd.Name}.value", StringComparison.Ordinal))
                {
                    // **** figure out the type ****

                    foreach (fhir_2.ElementDefinitionType edType in element.Type)
                    {
                        // **** check for a specified type ****

                        if (!string.IsNullOrEmpty(edType.Code))
                        {
                            // **** use this type ****

                            simple.BaseTypeName = edType.Code;

                            // **** done searching ****

                            break;
                        }

                        // **** use an extension-defined type ****

                        foreach (fhir_2.Extension ext in edType._Code.Extension)
                        {
                            if (ext.Url.Equals(_urlJsonType, StringComparison.Ordinal))
                            {
                                // *** use this type ****

                                simple.BaseTypeName = ext.ValueString;
                                
                                // **** stop looking ****

                                break;

                            }
                        }
                    }

                    // **** stop looking ****

                    break;
                }
            }

            // **** make sure we have a type ****

            if (string.IsNullOrEmpty(simple.BaseTypeName))
            {
                Console.WriteLine($"ProcessDataTypeSimpleV2 <<<" +
                    $" Could not determine base type for {sd.Name}");
                return false;
            }

            // **** add to our dictionary of simple types ****

            _simpleTypes[sd.Name] = simple;

            // **** success ****

            return true;
        }

        private bool ProcessDataTypeComplexV2(fhir_2.StructureDefinition sd)
        {
            // **** create a new Complex Type object ****

            FhirComplexType complex = new FhirComplexType()
            {
                Name = sd.Name,
                NameCapitalized = string.Concat(sd.Name.Substring(0, 1).ToUpper(), sd.Name.Substring(1)),
                StandardStatus = sd.Status,
                ShortDescription = sd.Description,
                Definition = sd.Requirements,
            };

            // **** figure out the basea type ****

            foreach (fhir_2.ElementDefinition element in sd.Snapshot.Element)
            {
                // **** check for {type}.value to find where the actual value is defined ****

                if (element.Path.Equals(sd.Name, StringComparison.Ordinal))
                {
                    // **** figure out the type ****

                    foreach (fhir_2.ElementDefinitionType edType in element.Type)
                    {
                        // **** check for a specified type ****

                        if (!string.IsNullOrEmpty(edType.Code))
                        {
                            // **** use this type ****

                            complex.BaseTypeName = edType.Code;

                            // **** done searching ****

                            break;
                        }
                        
                        // **** use an extension-defined type ****

                        foreach (fhir_2.Extension ext in edType._Code.Extension)
                        {
                            if (ext.Url.Equals(_urlJsonType, StringComparison.Ordinal))
                            {
                                // **** use this type ****

                                complex.BaseTypeName = ext.ValueString;
                                
                                // **** stop looking ****

                                break;
                            }
                        }
                    }
                
                    // **** stop looking ****

                    break;
                }
            }

            // **** make sure we have a type ****

            if (string.IsNullOrEmpty(complex.BaseTypeName))
            {
                Console.WriteLine($"ProcessDataTypeComplexV2 <<<" +
                    $" Could not determine base type for {sd.Name}");
                return false;
            }

            // **** look for properties on this type ****



            // **** add to our dictionary of complex types ****

            _complexTypes[sd.Name] = complex;

            // **** success ****

            return true;
        }

        #endregion V2 Processing. . .

    }
}
