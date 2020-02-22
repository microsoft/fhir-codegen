using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    public class FhirPackageInfo
    {
        #region Class Variables . . .

        #endregion Class Variables . . .

        #region Instance Variables . . .

        public string Name { get; set; }

        public string Version { get; set; }

        [JsonProperty(PropertyName = "fhir-version-list")]
        public string[] FhirVersionList { get; set; }

        [JsonProperty(PropertyName = "fhirVersions")]
        public string[] FhirVersions { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string PackageType { get; set; }

        [JsonProperty(PropertyName = "tools-version")]
        public decimal ToolsVersion { get; set; }

        public string Canonical { get; set; }
        public string Homepage { get; set; }

        public string URL { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public class DependencyInfo
        {
            public string Name { get; set; }

            public string Version { get; set; }
        }

        public DependencyInfo[] Dependencies { get; set; }

        public string[] Keywords { get; set; }

        public string Author { get; set; }

        public class MaintainerInfo
        {
            public string Name { get; set; }

            public string Email { get; set; }
        }

        public MaintainerInfo[] Maintainers { get; set; }

        public string License { get; set; }

        #endregion Instance Variables . . .

        #region Constructors . . .

        #endregion Constructors . . .

        #region Class Interface . . .

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Attempts to load FHIR NPM package information from the given directory.</summary>
        ///
        /// <param name="packageDirectory">Pathname of the package directory.</param>
        /// <param name="packageInfo">     [out] Information describing the package.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        ///-------------------------------------------------------------------------------------------------

        public static bool TryLoadPackageInfo(string packageDirectory, out FhirPackageInfo packageInfo)
        {
            packageInfo = null;

            // **** build the path to our file ****

            string packageFilename = Path.Combine(packageDirectory, "package.json");

            // **** make sure our file exists ****

            if (!File.Exists(packageFilename))
            {
                Console.WriteLine($"Package file not found! {packageFilename}");
                return false;
            }

            try
            {
                // **** load the file ****

                string packageContents = File.ReadAllText(packageFilename);

                // **** attempt to parse ****

                packageInfo = JsonConvert.DeserializeObject<FhirPackageInfo>(packageContents);

                // **** here means success ****

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Parsing package.json failed: {ex.Message}");
            }

            // **** still here means failure ****

            return false;
        }

        #endregion Class Interface . . .

        #region Instance Interface . . .

        #endregion Instance Interface . . .

        #region Internal Functions . . .

        #endregion Internal Functions . . .



    }
}
