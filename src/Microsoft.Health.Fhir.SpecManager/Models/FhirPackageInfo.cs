// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    /// <summary>Information about the fhir package.</summary>
    public class FhirPackageInfo
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        /// -------------------------------------------------------------------------------------------------
        public string Name { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the version.</summary>
        ///
        /// <value>The version.</value>
        /// -------------------------------------------------------------------------------------------------
        public string Version { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets a list of fhir versions.</summary>
        ///
        /// <value>A list of fhir versions.</value>
        /// -------------------------------------------------------------------------------------------------
        [JsonProperty(PropertyName = "fhir-version-list")]
        public string[] FhirVersionList { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the fhir versions.</summary>
        ///
        /// <value>The fhir versions.</value>
        /// -------------------------------------------------------------------------------------------------
        [JsonProperty(PropertyName = "fhirVersions")]
        public string[] FhirVersions { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the type of the package.</summary>
        ///
        /// <value>The type of the package.</value>
        /// -------------------------------------------------------------------------------------------------
        [JsonProperty(PropertyName = "type")]
        public string PackageType { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the tools version.</summary>
        ///
        /// <value>The tools version.</value>
        /// -------------------------------------------------------------------------------------------------
        [JsonProperty(PropertyName = "tools-version")]
        public decimal ToolsVersion { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the canonical.</summary>
        ///
        /// <value>The canonical.</value>
        /// -------------------------------------------------------------------------------------------------
        public string Canonical { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the homepage.</summary>
        ///
        /// <value>The homepage.</value>
        /// -------------------------------------------------------------------------------------------------
        public string Homepage { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets URL of the document.</summary>
        ///
        /// <value>The URL.</value>
        /// -------------------------------------------------------------------------------------------------
        public string URL { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the title.</summary>
        ///
        /// <value>The title.</value>
        /// -------------------------------------------------------------------------------------------------
        public string Title { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the description.</summary>
        ///
        /// <value>The description.</value>
        /// -------------------------------------------------------------------------------------------------
        public string Description { get; set; }

        /// <summary>Information about the dependency.</summary>
        public class DependencyInfo
        {
            /// -------------------------------------------------------------------------------------------------
            /// <summary>Gets or sets the name.</summary>
            ///
            /// <value>The name.</value>
            /// -------------------------------------------------------------------------------------------------

            public string Name { get; set; }

            /// -------------------------------------------------------------------------------------------------
            /// <summary>Gets or sets the version.</summary>
            ///
            /// <value>The version.</value>
            /// -------------------------------------------------------------------------------------------------

            public string Version { get; set; }
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the dependencies.</summary>
        ///
        /// <value>The dependencies.</value>
        /// -------------------------------------------------------------------------------------------------
        public DependencyInfo[] Dependencies { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the keywords.</summary>
        ///
        /// <value>The keywords.</value>
        /// -------------------------------------------------------------------------------------------------
        public string[] Keywords { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the author.</summary>
        ///
        /// <value>The author.</value>
        /// -------------------------------------------------------------------------------------------------
        public string Author { get; set; }

        /// <summary>Information about the maintainer.</summary>
        public class MaintainerInfo
        {
            /// -------------------------------------------------------------------------------------------------
            /// <summary>Gets or sets the name.</summary>
            ///
            /// <value>The name.</value>
            /// -------------------------------------------------------------------------------------------------
            public string Name { get; set; }

            /// -------------------------------------------------------------------------------------------------
            /// <summary>Gets or sets the email.</summary>
            ///
            /// <value>The email.</value>
            /// -------------------------------------------------------------------------------------------------
            public string Email { get; set; }
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the maintainers.</summary>
        ///
        /// <value>The maintainers.</value>
        /// -------------------------------------------------------------------------------------------------
        public MaintainerInfo[] Maintainers { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the license.</summary>
        ///
        /// <value>The license.</value>
        /// -------------------------------------------------------------------------------------------------
        public string License { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>Attempts to load FHIR NPM package information from the given directory.</summary>
        ///
        /// <param name="packageDirectory">Pathname of the package directory.</param>
        /// <param name="packageInfo">     [out] Information describing the package.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        /// -------------------------------------------------------------------------------------------------
        public static bool TryLoadPackageInfo(string packageDirectory, out FhirPackageInfo packageInfo)
        {
            packageInfo = null;

            // build the path to our file
            string packageFilename = Path.Combine(packageDirectory, "package.json");

            // make sure our file exists
            if (!File.Exists(packageFilename))
            {
                Console.WriteLine($"Package file not found! {packageFilename}");
                return false;
            }

            try
            {
                // load the file
                string packageContents = File.ReadAllText(packageFilename);

                // attempt to parse
                packageInfo = JsonConvert.DeserializeObject<FhirPackageInfo>(packageContents);

                // here means success
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Parsing package.json failed: {ex.Message}");
            }

            // still here means failure
            return false;
        }
    }
}
