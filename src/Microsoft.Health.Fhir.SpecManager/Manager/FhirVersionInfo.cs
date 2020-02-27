// -------------------------------------------------------------------------------------------------
// <copyright file="FhirVersionInfo.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Health.Fhir.SpecManager.Converters;
using Microsoft.Health.Fhir.SpecManager.Models;

namespace Microsoft.Health.Fhir.SpecManager.Manager
{
    /// <summary>Information about a FHIR release.</summary>
    public class FhirVersionInfo
    {
        /// <summary>Extension URL for JSON type information.</summary>
        public const string UrlJsonType = "http://hl7.org/fhir/StructureDefinition/structuredefinition-json-type";

        /// <summary>Extension URL for XML type information.</summary>
        public const string UrlXmlType = "http://hl7.org/fhir/StructureDefinition/structuredefinition-xml-type";

        /// <summary>Extension URL for FHIR type information (added R4).</summary>
        public const string UrlFhirType = "http://hl7.org/fhir/StructureDefinition/structuredefinition-fhir-type";

        /// <summary>The known version numbers (for fast checking on version load requests).</summary>
        private static HashSet<int> _knownVersionNumbers = new HashSet<int>()
        {
            2,
            3,
            4,
            5,
        };

        /// <summary>Types of resources to process, by FHIR version.</summary>
        private static Dictionary<int, HashSet<string>> _versionResourcesToProcess = new Dictionary<int, HashSet<string>>()
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

        /// <summary>Types of resources to ignore, by FHIR version.</summary>
        private static Dictionary<int, HashSet<string>> _versionResourcesToIgnore = new Dictionary<int, HashSet<string>>()
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

        private static HashSet<string> _npmFilesToIgnore = new HashSet<string>()
        {
            ".index.json",
            "package.json",
        };

        private IFhirConverter _fhirConverter;
        private Dictionary<string, FhirPrimitive> _primitiveTypes;
        private Dictionary<string, FhirComplex> _complexTypes;
        private Dictionary<string, FhirComplex> _resources;
        private Dictionary<string, FhirCapability> _capabilities;

        /// <summary>
        /// Initializes a new instance of the <see cref="FhirVersionInfo"/> class.
        /// Require major version (release #) to validate it is supported.
        /// </summary>
        ///
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        ///
        /// <param name="majorVersion">The major version.</param>
        public FhirVersionInfo(int majorVersion)
        {
            if (!_knownVersionNumbers.Contains(majorVersion))
            {
                throw new Exception($"Invalid FHIR major version: {majorVersion}!");
            }

            // copy required fields
            MajorVersion = majorVersion;

            // create our JSON converter
            switch (majorVersion)
            {
                case 2:
                    _fhirConverter = new FromR2();
                    break;
                case 3:
                    _fhirConverter = new FromR3();
                    break;
                case 4:
                    _fhirConverter = new FromR4();
                    break;
                case 5:
                    _fhirConverter = null;
                    break;
            }

            // create our info dictionaries
            PrimitiveTypes = new Dictionary<string, FhirPrimitive>();
            ComplexTypes = new Dictionary<string, FhirComplex>();
            Resources = new Dictionary<string, FhirComplex>();
            Capabilities = new Dictionary<string, FhirCapability>();
        }

        /// <summary>Gets or sets the major version.</summary>
        ///
        /// <value>The major version.</value>
        public int MajorVersion { get; set; }

        /// <summary>Gets or sets the name of the package release.</summary>
        ///
        /// <value>The name of the package release.</value>
        public string ReleaseName { get; set; }

        /// <summary>Gets or sets the name of the package.</summary>
        ///
        /// <value>The name of the package.</value>
        public string PackageName { get; set; }

        /// <summary>Gets or sets the version string.</summary>
        ///
        /// <value>The version string.</value>
        public string VersionString { get; set; }

        /// <summary>Gets or sets a value indicating whether this object is development build.</summary>
        ///
        /// <value>True if this object is development build, false if not.</value>
        public bool IsDevBuild { get; set; }

        /// <summary>Gets or sets the development branch.</summary>
        ///
        /// <value>The development branch.</value>
        public string DevBranch { get; set; }

        /// <summary>Gets or sets a value indicating whether this object is local build.</summary>
        ///
        /// <value>True if this object is local build, false if not.</value>
        public bool IsLocalBuild { get; set; }

        /// <summary>Gets or sets the pathname of the local directory.</summary>
        ///
        /// <value>The pathname of the local directory.</value>
        public string LocalDirectory { get; set; }

        /// <summary>Gets or sets a value indicating whether this object is on disk.</summary>
        ///
        /// <value>True if available, false if not.</value>
        public bool IsOnDisk { get; set; }

        /// <summary>Gets or sets the Date/Time of the last downloaded.</summary>
        ///
        /// <value>The last downloaded.</value>
        public DateTime? LastDownloaded { get; set; }

        /// <summary>Gets or sets a dictionary with the known primitive types for this version of FHIR.</summary>
        ///
        /// <value>A dictionary of the primitive types.</value>
        public Dictionary<string, FhirPrimitive> PrimitiveTypes { get => _primitiveTypes; set => _primitiveTypes = value; }

        /// <summary>Gets or sets a dictionary with the known complex types for this version of FHIR.</summary>
        ///
        /// <value>A dictionary of the complex types.</value>
        public Dictionary<string, FhirComplex> ComplexTypes { get => _complexTypes; set => _complexTypes = value; }

        /// <summary>Gets or sets a dictionary with the known resources for this version of FHIR.</summary>
        ///
        /// <value>A dictionary of the resources.</value>
        public Dictionary<string, FhirComplex> Resources { get => _resources; set => _resources = value; }

        /// <summary>Gets or sets the capabilities.</summary>
        ///
        /// <value>The capabilities.</value>
        public Dictionary<string, FhirCapability> Capabilities { get => _capabilities; set => _capabilities = value; }

        /// <summary>Determine if we should process resource.</summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        public bool ShouldProcessResource(string resourceName)
        {
            if (_versionResourcesToProcess[MajorVersion].Contains(resourceName))
            {
                return true;
            }

            return false;
        }

        /// <summary>Determine if we should ignore resource.</summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        public bool ShouldIgnoreResource(string resourceName)
        {
            if (_versionResourcesToIgnore[MajorVersion].Contains(resourceName))
            {
                return true;
            }

            return false;
        }

        /// <summary>Determine if we should skip file.</summary>
        /// <param name="filename">Filename of the file.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        public static bool ShouldSkipFile(string filename)
        {
            if (_npmFilesToIgnore.Contains(filename))
            {
                return true;
            }

            return false;
        }

        /// <summary>Attempts to parse resource an object from the given string.</summary>
        /// <param name="json">    The JSON.</param>
        /// <param name="resource">[out] The resource object.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        public bool TryParseResource(string json, out object resource)
        {
            return _fhirConverter.TryParseResource(json, out resource);
        }

        /// <summary>Attempts to process resource.</summary>
        /// <param name="resource">[out] The resource object.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        public bool TryProcessResource(object resource)
        {
            return _fhirConverter.TryProcessResource(
                resource,
                ref _primitiveTypes,
                ref _complexTypes,
                ref _resources);
        }
    }
}
