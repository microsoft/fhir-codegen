// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;

namespace Microsoft.Health.Fhir.SpecManager.Manager
{
    /// <summary>A fhir package downloader.</summary>
    public static class FhirPackageDownloader
    {
        public const string PublishedFhirUrl = "http://hl7.org/fhir/";
        public const string BuildFhirUrl = "http://build.fhir.org/";

        /// <summary>The HTTP client.</summary>
        private static HttpClient _httpClient = new HttpClient();

        /// -------------------------------------------------------------------------------------------------
        /// <summary>Downloads a published FHIR package.</summary>
        ///
        /// <param name="releaseName"> Name of the release.</param>
        /// <param name="packageName"> Name of the package.</param>
        /// <param name="npmDirectory">Pathname of the npm directory.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        /// -------------------------------------------------------------------------------------------------

        public static bool DownloadPublishedPackage(string releaseName, string packageName, string npmDirectory)
        {
            Stream fileStream = null;
            Stream gzipStream = null;
            TarArchive tar = null;

            try
            {
                // build the url to this package
                string url = $"{PublishedFhirUrl}{releaseName}/{packageName}.tgz";

                // build our extraction directory name
                string directory = Path.Combine(npmDirectory, packageName);

                // make sure our destination directory exists
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // start our download as a stream
                fileStream = _httpClient.GetStreamAsync(url).Result;

                // extract to the npm directory
                gzipStream = new GZipInputStream(fileStream);

                // grab the tar archive
                tar = TarArchive.CreateInputTarArchive(gzipStream);

                // extract
                tar.ExtractContents(directory);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DownloadPublishedPackage <<< failed to download package: {releaseName}/{packageName}: {ex.Message}");
                return false;
            }
            finally
            {
                // clean up
                if (tar != null)
                {
                    tar.Close();
                    tar = null;
                }

                if (gzipStream != null)
                {
                    gzipStream.Close();
                    gzipStream = null;
                }

                if (fileStream != null)
                {
                    fileStream.Close();
                    fileStream = null;
                }
            }

            return true;
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>Downloads a package from the Dev build server.</summary>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        /// -------------------------------------------------------------------------------------------------
        public static bool DownloadBuildPackage()
        {
            return false;
        }
    }
}
