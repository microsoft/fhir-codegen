using CommandLine;

namespace fhir_codegen_cli
{
    public class Options
    {
        [Option("npm-directory", Default = "", Required = true, HelpText = "Directory where NPM packages are located")]
        public string NpmDirectory { get; set; }

        [Option("install-npms", Default = false, HelpText = "Download NPM packages using 'npm install'")]
        public bool InstallNpms { get; set; }

        [Option("load-r2", Default = false, HelpText = "Load the R2 (DSTU 2) definitions")]
        public bool LoadR2 { get; set; }

        [Option("load-r3", Default = false, HelpText = "Load the R3 (STU 3) definitions")]
        public bool LoadR3 { get; set; }

        [Option("load-r4", Default = false, HelpText = "Load the R4 definitions")]
        public bool LoadR4 { get; set; }

        [Option("load-r5", Default = false, HelpText = "Load the R5 definitions")]
        public bool LoadR5 { get; set; }

        [Option('v', "verbose", Default = false, HelpText = "Show verbose output")]
        public bool Verbose { get; set; }

    }
}
