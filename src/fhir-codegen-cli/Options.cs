using CommandLine;

namespace fhir_codegen_cli
{
    public class Options
    {
        #region Input Options . . .

        [Option("npm-directory", Default = "", Required =true, HelpText = "Directory where NPM packages are located")]
        public string npmDirectory { get; set; }

        [Option("load-v2", Default = true, HelpText = "Load the V2 (DSTU 2) definitions")]
        public bool loadV2 { get; set; }

        [Option("load-v3", Default = true, HelpText = "Load the V3 (STU 3) definitions")]
        public bool loadV3 { get; set; }

        [Option("load-v4", Default = true, HelpText = "Load the V4 (R4) definitions")]
        public bool loadV4 { get; set; }

        #endregion Input Options . . .
    }
}
