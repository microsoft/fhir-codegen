# Testing

Running `dotnet run -p src/fhir-codegen-test-cli/fhir-codegen-test-cli.csproj` launches a full build and test.

It will generate updated CSharpBasic and TypeScript files for FHIR Versions DSTU2, STU3, R4, and R5 (May 2020).  It will then run each through a build process (requires `dotnet` and `tsc`) to validate there are no syntax errors in any of the generated files.

Note that this test takes several minutes to run.

## Usage
```
fhir-codegen-test-cli:
  The FHIR CodeGen Test CLI.

Usage:
  fhir-codegen-test-cli [options]

Options:
  --repo-root-path <repo-root-path>    The path to the repository root (if not CWD).
  --verbose                            True to display all output
                                         (default: false)
  --fixed-format-statistics            True to output *only* test run statistics:
                                         #run[tab]#passed[tab]#failed[tab]#skipped
                                         (default: false)
  --errors-to-std-error                True to write errors to stderr instead of stdout.
                                         (default: False)
  --version                            Show version information
  -?, -h, --help                       Show help and usage information
```
