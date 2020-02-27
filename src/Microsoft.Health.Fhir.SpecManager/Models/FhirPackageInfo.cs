// -------------------------------------------------------------------------------------------------
// <copyright file="FhirPackageInfo.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
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
        /// <summary>
        /// Initializes a new instance of the <see cref="FhirPackageInfo"/> class.
        /// </summary>
        /// <param name="name">           The name.</param>
        /// <param name="version">        The version.</param>
        /// <param name="fhirVersionList">A list of fhir versions.</param>
        /// <param name="fhirVersions">   The fhir versions.</param>
        /// <param name="packageType">    The type of the package.</param>
        /// <param name="toolsVersion">   The tools version.</param>
        /// <param name="canonical">      The canonical.</param>
        /// <param name="homepage">       The homepage.</param>
        /// <param name="url">            The URL.</param>
        /// <param name="title">          The title.</param>
        /// <param name="description">    The description.</param>
        /// <param name="keywords">       The keywords.</param>
        /// <param name="author">         The author.</param>
        /// <param name="license">        The license.</param>
        [JsonConstructor]
        internal FhirPackageInfo(
            string name,
            string version,
            List<string> fhirVersionList,
            List<string> fhirVersions,
            string packageType,
            decimal toolsVersion,
            string canonical,
            string homepage,
            Uri url,
            string title,
            string description,
            List<string> keywords,
            string author,
            string license)
        {
            Name = name;
            Version = version;
            FhirVersionList = fhirVersionList;
            FhirVersions = fhirVersions;
            PackageType = packageType;
            ToolsVersion = toolsVersion;
            Canonical = canonical;
            Homepage = homepage;
            URL = url;
            Title = title;
            Description = description;
            Keywords = keywords;
            Author = author;
            License = license;
        }

        /// <summary>Gets the name.</summary>
        /// <value>The name.</value>
        public string Name { get; }

        /// <summary>Gets the version.</summary>
        /// <value>The version.</value>
        public string Version { get; }

        /// <summary>Gets a list of fhir versions.</summary>
        /// <value>A list of fhir versions.</value>
        [JsonProperty(PropertyName = "fhir-version-list")]
        public List<string> FhirVersionList { get; }

        /// <summary>Gets the fhir versions.</summary>
        /// <value>The fhir versions.</value>
        [JsonProperty(PropertyName = "fhirVersions")]
        public List<string> FhirVersions { get; }

        /// <summary>Gets the type of the package.</summary>
        /// <value>The type of the package.</value>
        [JsonProperty(PropertyName = "type")]
        public string PackageType { get; }

        /// <summary>Gets the tools version.</summary>
        /// <value>The tools version.</value>
        [JsonProperty(PropertyName = "tools-version")]
        public decimal ToolsVersion { get; }

        /// <summary>Gets the canonical.</summary>
        /// <value>The canonical.</value>
        public string Canonical { get; }

        /// <summary>Gets the homepage.</summary>
        /// <value>The homepage.</value>
        public string Homepage { get; }

        /// <summary>Gets URL of the document.</summary>
        /// <value>The URL.</value>
        public Uri URL { get; }

        /// <summary>Gets the title.</summary>
        /// <value>The title.</value>
        public string Title { get; }

        /// <summary>Gets the description.</summary>
        /// <value>The description.</value>
        public string Description { get; }

        /// <summary>Gets the keywords.</summary>
        /// <value>The keywords.</value>
        public List<string> Keywords { get; }

        /// <summary>Gets the author.</summary>
        /// <value>The author.</value>
        public string Author { get; }

        /// <summary>Gets the license.</summary>
        /// <value>The license.</value>
        public string License { get; }

        /// <summary>Attempts to load FHIR NPM package information from the given directory.</summary>
        /// <param name="packageDirectory">Pathname of the package directory.</param>
        /// <param name="packageInfo">     [out] Information describing the package.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
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
                throw;
            }
        }
    }
}
