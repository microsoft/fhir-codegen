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

        /// <summary>Downloads either a Package or Published version of the requested type.</summary>
        /// <param name="releaseName">      The release name (e.g., R4, DSTU2).</param>
        /// <param name="ballotPrefix">     The ballot prefix.</param>
        /// <param name="packageName">      Name of the package.</param>
        /// <param name="version">          The version string (e.g., 4.0.1).</param>
        /// <param name="fhirSpecDirectory">Pathname of the FHIR spec directory.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        public static bool Download(
            string releaseName,
            string ballotPrefix,
            string packageName,
            string version,
            string fhirSpecDirectory)
        {
            bool loaded = false;

            try
            {
                Console.WriteLine($" <<< downloading PACKAGE {packageName}:{version}");

                // download from the package manager
                loaded = FhirPackageDownloader.DownloadPackage(
                    releaseName,
                    packageName,
                    version,
                    fhirSpecDirectory);

                if (loaded)
                {
                    return true;
                }
            }
            catch (HttpRequestException)
            {
                Console.WriteLine($"Failed to download Package: {packageName}:{version}");
            }
            catch (AggregateException)
            {
                Console.WriteLine($"Failed to download Published: {packageName}:{version}");
            }
            catch (System.Threading.Tasks.TaskCanceledException)
            {
                Console.WriteLine($"Failed to download Published: {packageName}:{version}");
            }

            try
            {
                Console.WriteLine($" <<< downloading PUBLISHED {packageName}:{version}");

                // download from publish URL
                loaded = FhirPackageDownloader.DownloadPublished(
                    releaseName,
                    ballotPrefix,
                    packageName,
                    version,
                    fhirSpecDirectory);
            }
            catch (HttpRequestException)
            {
                Console.WriteLine($"Failed to download Published: {packageName}:{version}");
            }
            catch (AggregateException)
            {
                Console.WriteLine($"Failed to download Published: {packageName}:{version}");
            }
            catch (System.Threading.Tasks.TaskCanceledException)
            {
                Console.WriteLine($"Failed to download Published: {packageName}:{version}");
            }

            return loaded;
        }

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
                throw new FileNotFoundException($"Package not found: {releaseName}:{packageName}");
            }

            if (info.Versions == null)
            {
                throw new InvalidDataException($"Version information not found for package: {releaseName}:{packageName}");
            }

            // find the correct download url for this version
            if (!info.Versions.ContainsKey(version))
            {
                throw new InvalidDataException($"Version {version} not found in package {releaseName}:{packageName}");
            }

            // download and extract our package
            return DownloadAndExtract(info.Versions[version].URL, packageName, version, fhirSpecDirectory);
        }

        /// <summary>Downloads a published FHIR package.</summary>
        /// <param name="releaseName">      The release name (e.g., R4, DSTU2).</param>
        /// <param name="ballotPrefix">     The ballot prefix.</param>
        /// <param name="packageName">      Name of the package.</param>
        /// <param name="version">          The version string (e.g., 4.0.1).</param>
        /// <param name="fhirSpecDirectory">Pathname of the FHIR spec directory.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        public static bool DownloadPublished(
            string releaseName,
            string ballotPrefix,
            string packageName,
            string version,
            string fhirSpecDirectory)
        {
            // build the url to this package
            string url;

            if (string.IsNullOrEmpty(ballotPrefix))
            {
                if ((!string.IsNullOrEmpty(version)) && (version.Length > 5))
                {
                    url = $"{PublishedDownloadUrlBase}{version}/{packageName}.tgz";
                }
                else
                {
                    url = $"{PublishedDownloadUrlBase}{releaseName}/{packageName}.tgz";
                }
            }
            else
            {
                url = $"{PublishedDownloadUrlBase}{ballotPrefix}/{packageName}.tgz";
            }

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
            try
            {
                // build our extraction directory name
                string directory = Path.Combine(specDirectory, $"{packageName}-{version}");

                // make sure our destination directory exists
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (Stream fileStream = _httpClient.GetStreamAsync(uri).Result)
                using (Stream gzipStream = new GZipInputStream(fileStream))
                using (TarArchive tar = TarArchive.CreateInputTarArchive(gzipStream))
                {
                    // extract
                    tar.ExtractContents(directory);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DownloadPublishedPackage <<< failed to download package: {packageName}-{version}: {ex.Message}");
                throw;
            }
        }

        /// <summary>Downloads the and extract.</summary>
        /// <exception cref="FileNotFoundException">Thrown when the requested file is not present.</exception>
        /// <param name="sourceDir">    Path to load package files from.</param>
        /// <param name="packageName">  Name of the package.</param>
        /// <param name="version">      The version.</param>
        /// <param name="specDirectory">Pathname of the specifier directory.</param>
        /// <param name="dir">          [out] The dir the package was expanded to.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        internal static bool CopyAndExtract(
            string sourceDir,
            string packageName,
            string version,
            string specDirectory,
            out string dir)
        {
            try
            {
                // build our extraction directory name
                dir = Path.Combine(specDirectory, $"local-{packageName}-{version}");

                // make sure our destination directory exists
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                string sourceFilename = Path.Combine(sourceDir, $"{packageName}.tgz");

                if (!File.Exists(sourceFilename))
                {
                    throw new FileNotFoundException();
                }

                // open the source file
                using (Stream fileStream = new FileStream(sourceFilename, FileMode.Open))
                using (Stream gzipStream = new GZipInputStream(fileStream))
                using (TarArchive tar = TarArchive.CreateInputTarArchive(gzipStream))
                {
                    // extract
                    tar.ExtractContents(dir);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DownloadPublishedPackage <<< failed to download package: {packageName}-{version}: {ex.Message}");
                throw;
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
