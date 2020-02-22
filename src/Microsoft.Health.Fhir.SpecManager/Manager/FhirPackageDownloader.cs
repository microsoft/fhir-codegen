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
    public class FhirPackageDownloader
    {
        public const string PublishedFhirUrl = "http://hl7.org/fhir/";
        public const string BuildFhirUrl = "http://build.fhir.org/";

        private static HttpClient _httpClient;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Static constructor.</summary>
        ///-------------------------------------------------------------------------------------------------

        static FhirPackageDownloader()
        {
            // **** create our http client ****

            _httpClient = new HttpClient();
        }

        public bool DownloadPublishedPackage(string releaseName, string packageName, string npmDirectory)
        {
            Stream fileStream = null;
            Stream gzipStream = null;
            TarArchive tar = null;

            try
            {
                // **** build the url to this package ****

                string url = $"{PublishedFhirUrl}{releaseName}/{packageName}.tgz";

                // **** build our extraction directory name ****

                string directory = Path.Combine(npmDirectory, packageName);

                // **** make sure our destination directory exists ****

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // **** start our download as a stream ****

                fileStream = _httpClient.GetStreamAsync(url).Result;

                // **** extract to the npm directory ****

                gzipStream = new GZipInputStream(fileStream);

                // ***** grab the tar archive ****

                tar = TarArchive.CreateInputTarArchive(gzipStream);

                // **** extract ****

                tar.ExtractContents(directory);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DownloadPublishedPackage <<< failed to download package: {releaseName}/{packageName}: {ex.Message}");
                return false;
            }
            finally
            {
                // **** clean up ****

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

        public bool DownloadBuildPackage()
        {
            return false;
        }
    }
}
