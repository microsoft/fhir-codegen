using CommandLine;

namespace fhir_codegen_cli
{
    public class Options
    {
        #region Input Options . . .

        [Option("npm-directory", Default = "", Required =true, HelpText = "Directory where NPM packages are located")]
        public string NpmDirectory { get; set; }

        [Option("install-npms", Default = false, HelpText = "Download NPM packages using 'npm install'")]
        public bool InstallNpms { get; set; }

        [Option("load-v2", Default = true, HelpText = "Load the V2 (DSTU 2) definitions")]
        public bool LoadV2 { get; set; }

        [Option("load-v3", Default = true, HelpText = "Load the V3 (STU 3) definitions")]
        public bool LoadV3 { get; set; }

        [Option("load-v4", Default = true, HelpText = "Load the V4 (R4) definitions")]
        public bool LoadV4 { get; set; }

        #endregion Input Options . . .
    }
}
