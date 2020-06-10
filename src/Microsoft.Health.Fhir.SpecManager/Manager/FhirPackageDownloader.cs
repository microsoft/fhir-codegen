// -------------------------------------------------------------------------------------------------
// <copyright file="FhirPackageDownloader.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using Newtonsoft.Json;

namespace Microsoft.Health.Fhir.SpecManager.Manager
{
    /// <summary>A fhir package downloader.</summary>
    public static class FhirPackageDownloader
    {
        /// <summary>Base URL for published versions of FHIR.</summary>
        public const string PublishedDownloadUrlBase = "http://hl7.org/fhir/";

        /// <summary>Base URL for developer build versions of FHIR.</summary>
        public const string BuildDownloadUrlBase = "http://build.fhir.org/";

        /// <summary>Base URL for FHIR package downloads.</summary>
        public static readonly Uri PackageDownloadUriBase = new Uri("http://packages.fhir.org/");

        /// <summary>The HTTP client.</summary>
        private static HttpClient _httpClient = new HttpClient();

        /// <summary>Downloads a published FHIR package.</summary>
        /// <exception cref="FileNotFoundException">Thrown when the requested file is not present.</exception>
        /// <exception cref="InvalidDataException"> Thrown when an Invalid Data error condition occurs.</exception>
        /// <param name="releaseName">      The release name (e.g., R4, DSTU2).</param>
        /// <param name="packageName">      Name of the package.</param>
        /// <param name="version">          The version string (e.g., 4.0.1).</param>
        /// <param name="fhirSpecDirectory">Pathname of the FHIR spec directory.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        public static bool DownloadPackage(
            string releaseName,
            string packageName,
            string version,
            string fhirSpecDirectory)
        {
            Uri infoUri = new Uri(PackageDownloadUriBase, packageName);

            // check the package server for version information
            HttpResponseMessage response = _httpClient.GetAsync(infoUri).Result;

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed to GET package info: {response.StatusCode}");
                return false;
            }

            string versionInfoJson = response.Content.ReadAsStringAsync().Result;

            // deserialize our version info
            Models.PackagesVersionInfo info = JsonConvert.DeserializeObject<Models.PackagesVersionInfo>(versionInfoJson);

            // make sure we match
            if (info.Name != packageName)
            {
                throw new FileNotFoundException($"Package not found: {packageName}");
            }

            if (info.Versions == null)
            {
                throw new InvalidDataException($"Version information not found for package: {packageName}");
            }

            // find the correct download url for this version
            if (!info.Versions.ContainsKey(version))
            {
                throw new InvalidDataException($"Version {version} not found in package {packageName}");
            }

            // download and extract our package
            return DownloadAndExtract(info.Versions[version].URL, packageName, version, fhirSpecDirectory);
        }

        /// <summary>Downloads a published FHIR package.</summary>
        /// <param name="releaseName">      The release name (e.g., R4, DSTU2).</param>
        /// <param name="packageName">      Name of the package.</param>
        /// <param name="version">          The version string (e.g., 4.0.1).</param>
        /// <param name="fhirSpecDirectory">Pathname of the FHIR spec directory.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        public static bool DownloadPublished(
            string releaseName,
            string packageName,
            string version,
            string fhirSpecDirectory)
        {
            // build the url to this package
            string url = $"{PublishedDownloadUrlBase}{releaseName}/{packageName}.tgz";

            // download and extract our package
            return DownloadAndExtract(new Uri(url), packageName, version, fhirSpecDirectory);
        }

        /// <summary>Downloads the and extract.</summary>
        /// <param name="uri">          URI of the resource.</param>
        /// <param name="packageName">  Name of the package.</param>
        /// <param name="version">      The version.</param>
        /// <param name="specDirectory">Pathname of the specifier directory.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        private static bool DownloadAndExtract(
            Uri uri,
            string packageName,
            string version,
            string specDirectory)
        {
            Stream fileStream = null;
            Stream gzipStream = null;
            TarArchive tar = null;

            try
            {
                // build our extraction directory name
                string directory = Path.Combine(specDirectory, $"{packageName}-{version}");

                // make sure our destination directory exists
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // start our download as a stream
                fileStream = _httpClient.GetStreamAsync(uri).Result;

                // extract to the npm directory
                gzipStream = new GZipInputStream(fileStream);

                // grab the tar archive
                tar = TarArchive.CreateInputTarArchive(gzipStream);

                // extract
                tar.ExtractContents(directory);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DownloadPublishedPackage <<< failed to download package: {packageName}-{version}: {ex.Message}");
                throw;
            }
            finally
            {
                // clean up
                if (tar != null)
                {
                    tar.Close();
                }

                if (gzipStream != null)
                {
                    gzipStream.Close();
                }

                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }

        /// <summary>Downloads a package from the Dev build server.</summary>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        public static bool DownloadBuildPackage()
        {
            return false;
        }
    }
}
