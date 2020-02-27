// <copyright file="Options.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using CommandLine;

namespace FhirCodegenCli
{
    /// <summary>Command line options for the CLI.</summary>
    public class Options
    {
        /// <summary>Gets or sets the pathname of the npm directory.</summary>
        /// <value>The pathname of the npm directory.</value>
        [Option("npm-directory", Default = "", Required = true, HelpText = "Directory where NPM packages are located")]
        public string NpmDirectory { get; set; }

        /// <summary>Gets or sets a value indicating whether the install npms.</summary>
        /// <value>True if install npms, false if not.</value>
        [Option("install-npms", Default = false, HelpText = "Download NPM packages using 'npm install'")]
        public bool InstallNpms { get; set; }

        /// <summary>Gets or sets a value indicating whether FHIR R2 should be loaded.</summary>
        /// <value>True to load R2, false to not.</value>
        [Option("load-r2", Default = false, HelpText = "Load the R2 (DSTU 2) definitions")]
        public bool LoadR2 { get; set; }

        /// <summary>Gets or sets a value indicating whether FHIR R3 should be loaded.</summary>
        /// <value>True to load R3, false to not.</value>
        [Option("load-r3", Default = false, HelpText = "Load the R3 (STU 3) definitions")]
        public bool LoadR3 { get; set; }

        /// <summary>Gets or sets a value indicating whether FHIR R4 should be loaded.</summary>
        /// <value>True to load R4, false to not.</value>
        [Option("load-r4", Default = false, HelpText = "Load the R4 definitions")]
        public bool LoadR4 { get; set; }

        // /// <summary>Gets or sets a value indicating whether FHIR R5 should be loaded.</summary>
        // /// <value>True if load R5, false to not.</value>
        // [Option("load-r5", Default = false, HelpText = "Load the R5 definitions")]
        // public bool LoadR5 { get; set; }

        /// <summary>Gets or sets a value indicating whether the verbose.</summary>
        /// <value>True if verbose, false if not.</value>
        [Option('v', "verbose", Default = false, HelpText = "Show verbose output")]
        public bool Verbose { get; set; }
    }
}
