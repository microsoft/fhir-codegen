using Microsoft.Health.Fhir.SpecManager.Converters;
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

        public const string UrlJsonType = "http://hl7.org/fhir/StructureDefinition/structuredefinition-json-type";
        public const string UrlXmlType = "http://hl7.org/fhir/StructureDefinition/structuredefinition-xml-type";
        public const string UrlFhirType = "http://hl7.org/fhir/StructureDefinition/structuredefinition-fhir-type";

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

        private IFhirConverter _fhirConverter;
        private Dictionary<string, FhirSimpleType> _simpleTypes;
        private Dictionary<string, FhirComplexType> _complexTypes;
        private Dictionary<string, FhirResource> _resources;
        private Dictionary<string, FhirCapability> _capabilities;

        public Dictionary<string, FhirSimpleType> SimpleTypes { get => _simpleTypes; set => _simpleTypes = value; }
        public Dictionary<string, FhirComplexType> ComplexTypes { get => _complexTypes; set => _complexTypes = value; }
        public Dictionary<string, FhirResource> Resources { get => _resources; set => _resources = value; }
        public Dictionary<string, FhirCapability> Capabilities { get => _capabilities; set => _capabilities = value; }

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
                        "StructureDefinition",
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
                        "Conformance",
                        "NamingSystem",
                        "OperationDefinition",
                        "SearchParameter",
                        "ValueSet",

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
                case 2:
                    _fhirConverter = new FromV2();
                    break;
                case 3:
                    _fhirConverter = null;
                    break;
                case 4:
                    _fhirConverter = null;
                    break;
                case 5:
                    _fhirConverter = null;
                    break;
            }

            // **** create our info dictionaries ****

            SimpleTypes = new Dictionary<string, FhirSimpleType>();
            ComplexTypes = new Dictionary<string, FhirComplexType>();
            Resources = new Dictionary<string, FhirResource>();
            Capabilities = new Dictionary<string, FhirCapability>();
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
            return _fhirConverter.TryParseResource(json, out obj);
        }

        public bool TryProcessResource(object obj)
        {
            return _fhirConverter.TryProcessResource(
                obj,
                ref _simpleTypes,
                ref _complexTypes,
                ref _resources
                );
        }

        #endregion Instance Interface . . .

        #region Internal Functions . . .

        #endregion Internal Functions . . .

    }
}
