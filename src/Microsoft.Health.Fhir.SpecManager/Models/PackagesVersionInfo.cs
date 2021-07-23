// <copyright file="PackagesVersionInfo.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    /// <summary>Information about the package version.</summary>
    internal class PackagesVersionInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PackagesVersionInfo"/> class.
        /// </summary>
        /// <param name="id">              The identifier.</param>
        /// <param name="name">            The name.</param>
        /// <param name="description">     The description.</param>
        /// <param name="distributionTags">The distribution tags.</param>
        /// <param name="versions">        The versions.</param>
        [JsonConstructor]
        internal PackagesVersionInfo(
            string id,
            string name,
            string description,
            Dictionary<string, string> distributionTags,
            Dictionary<string, VersionInfo> versions)
        {
            Id = id;
            Name = name;
            Description = description;
            DistributionTags = distributionTags;
            Versions = versions;
        }

        /// <summary>Gets the identifier.</summary>
        [JsonProperty(PropertyName = "_id")]
        internal string Id { get; }

        /// <summary>Gets the name.</summary>
        [JsonProperty(PropertyName = "name")]
        internal string Name { get; }

        /// <summary>Gets the description.</summary>
        [JsonProperty(PropertyName = "description")]
        internal string Description { get; }

        /// <summary>Gets the distribution tags.</summary>
        [JsonProperty(PropertyName ="dist-tags")]
        internal Dictionary<string, string> DistributionTags { get; }

        /// <summary>Gets the versions.</summary>
        [JsonProperty(PropertyName = "versions")]
        internal Dictionary<string, VersionInfo> Versions { get; }

        /// <summary>Information about the version.</summary>
        internal class VersionInfo
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="VersionInfo"/> class.
            /// </summary>
            /// <param name="name">       The name.</param>
            /// <param name="version">    The version.</param>
            /// <param name="description">The description.</param>
            /// <param name="dist">       The distribution info.</param>
            /// <param name="fhirVersion">The FHIR version.</param>
            /// <param name="url">        The URL.</param>
            /// <param name="unlisted">   The unlisted.</param>
            internal VersionInfo(
                string name,
                string version,
                string description,
                DistributionInfo dist,
                string fhirVersion,
                Uri url,
                string unlisted)
            {
                Name = name;
                Version = version;
                Description = description;
                Distribution = dist;
                FhirVersion = fhirVersion;
                URL = url;
                Unlisted = unlisted;
            }

            /// <summary>Initializes a new instance of the <see cref="VersionInfo"/> class.</summary>
            /// <param name="name">       The name.</param>
            /// <param name="version">    The version.</param>
            /// <param name="description">The description.</param>
            /// <param name="dist">       The distribution info.</param>
            /// <param name="fhirVersion">The FHIR version.</param>
            /// <param name="url">        The URL.</param>
            /// <param name="unlisted">   The unlisted.</param>
            [JsonConstructor]
            internal VersionInfo(
                string name,
                string version,
                string description,
                DistributionInfo dist,
                string fhirVersion,
                string url,
                string unlisted)
                : this(
                    name,
                    version,
                    description,
                    dist,
                    fhirVersion,
                    new Uri(url),
                    unlisted)
            {
            }

            /// <summary>Gets the name.</summary>
            [JsonProperty(PropertyName = "name")]
            internal string Name { get; }

            /// <summary>Gets the version.</summary>
            [JsonProperty(PropertyName = "version")]
            internal string Version { get; }

            /// <summary>Gets the description.</summary>
            [JsonProperty(PropertyName = "description")]
            internal string Description { get; }

            /// <summary>Gets URL of the document.</summary>
            [JsonProperty(PropertyName = "url")]
            internal Uri URL { get; }

            /// <summary>Gets the distribution.</summary>
            [JsonProperty(PropertyName = "dist")]
            internal DistributionInfo Distribution { get; }

            /// <summary>Gets the FHIR version.</summary>
            [JsonProperty(PropertyName = "fhirVersion")]
            internal string FhirVersion { get; }

            /// <summary>Gets the unlisted.</summary>
            [JsonProperty(PropertyName = "unlisted")]
            internal string Unlisted { get; }

            /// <summary>Information about the distribution.</summary>
            internal class DistributionInfo
            {
                /// <summary>Gets the hash sha.</summary>
                [JsonProperty(PropertyName = "shasum")]
                internal string HashSHA { get; }

                /// <summary>Gets URL of the tarball.</summary>
                [JsonProperty(PropertyName = "tarball")]
                internal string TarballUrl { get; }
            }
        }
    }
}
