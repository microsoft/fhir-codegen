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
        /// <value>The identifier.</value>
        [JsonProperty(PropertyName = "_id")]
        internal string Id { get; }

        /// <summary>Gets the name.</summary>
        /// <value>The name.</value>
        internal string Name { get; }

        /// <summary>Gets the description.</summary>
        /// <value>The description.</value>
        internal string Description { get; }

        /// <summary>Gets the distribution tags.</summary>
        /// <value>The distribution tags.</value>
        [JsonProperty(PropertyName ="disttags")]
        internal Dictionary<string, string> DistributionTags { get; }

        /// <summary>Gets the versions.</summary>
        /// <value>The versions.</value>
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
            /// <param name="url">        The URL.</param>
            /// <param name="unlisted">   The unlisted.</param>
            internal VersionInfo(
                string name,
                string version,
                string description,
                Uri url,
                string unlisted)
            {
                Name = name;
                Version = version;
                Description = description;
                URL = url;
                Unlisted = unlisted;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="VersionInfo"/> class.
            /// </summary>
            /// <param name="name">       The name.</param>
            /// <param name="version">    The version.</param>
            /// <param name="description">The description.</param>
            /// <param name="url">        The URL.</param>
            /// <param name="unlisted">   The unlisted.</param>
            [JsonConstructor]
            internal VersionInfo(
                string name,
                string version,
                string description,
                string url,
                string unlisted)
                : this(
                    name,
                    version,
                    description,
                    new Uri(url),
                    unlisted)
            {
            }

            /// <summary>Gets the name.</summary>
            /// <value>The name.</value>
            internal string Name { get; }

            /// <summary>Gets the version.</summary>
            /// <value>The version.</value>
            internal string Version { get; }

            /// <summary>Gets the description.</summary>
            /// <value>The description.</value>
            internal string Description { get; }

            /// <summary>Gets URL of the document.</summary>
            /// <value>The URL.</value>
            internal Uri URL { get; }

            /// <summary>Gets the unlisted.</summary>
            /// <value>The unlisted.</value>
            internal string Unlisted { get; }
        }
    }
}
