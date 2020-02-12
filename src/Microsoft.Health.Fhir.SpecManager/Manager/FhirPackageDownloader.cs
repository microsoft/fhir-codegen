using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using Microsoft.Health.Fhir.SpecManager.Models;
using Newtonsoft.Json;

namespace Microsoft.Health.Fhir.SpecManager.Manager
{
    public class FhirPackageDownloader
    {
        #region Class Constants . . .

        public const string PublishedFhirUrl = "http://hl7.org/fhir/";
        public const string BuildFhirUrl = "http://build.fhir.org/";

        #endregion Class Constants . . .

        #region Class Variables . . .

        private static HttpClient _httpClient;

        #endregion Class Variables . . .

        #region Instance Variables . . .

        #endregion Instance Variables . . .

        #region Constructors . . .

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Static constructor.</summary>
        ///
        /// <remarks>Gino Canessa, 2/12/2020.</remarks>
        ///-------------------------------------------------------------------------------------------------

        static FhirPackageDownloader()
        {
            // **** create our http client ****

            _httpClient = new HttpClient();
        }

        #endregion Constructors . . .

        #region Class Interface . . .

        #endregion Class Interface . . .

        #region Instance Interface . . .

        public bool DownloadPublishedPackage(string releaseName, string packageName, string npmDirectory)
        {
            try
            {
                string url = $"{PublishedFhirUrl}{releaseName}/{packageName}.tgz";

                string directory = Path.Combine(npmDirectory, packageName);

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // **** start our download ****

                Stream fileStream = _httpClient.GetStreamAsync(url).Result;

                // **** extract to the npm directory ****

                Stream gzipStream = new GZipInputStream(fileStream);

                // ***** grab the tar archive ****

                TarArchive tar = TarArchive.CreateInputTarArchive(gzipStream);

                // **** extract ****

                tar.ExtractContents(directory);

                // **** clean up ****

                tar.Close();
                gzipStream.Close();
                fileStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }


            return true;
        }

        public bool DownloadBuildPackage()
        {
            return false;
        }

        #endregion Instance Interface . . .

        #region Internal Functions . . .

        #endregion Internal Functions . . .

    }
}
