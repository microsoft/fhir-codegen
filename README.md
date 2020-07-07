# fhir-codegen

A .Net Core library and utility to generate various computer language outputs from FHIR specifications.

# Usage (CLI)

```
Usage:
  fhir-codegen-cli [options]

Options:
  --fhir-spec-directory <fhir-spec-directory>    The full path to the directory where FHIR 
                                                   specifications are downloaded and cached.
  --output-path <output-path>                    File or directory to write output.
  --verbose                                      Show verbose output.
  --offline-mode                                 Offline mode.
                                                   (will not download missing specs).
  --language <language>                          Name of the language to export.
                                                   (default: Info|TypeScript|CSharpBasic).
  --export-keys <export-keys>                    '|' separated list of items to export.
                                                   (not present to export everything)
  --load-r2 <load-r2>                            If FHIR R2 should be loaded, which version.
                                                   (e.g., 1.0.2 or latest)
  --load-r3 <load-r3>                            If FHIR R3 should be loaded, which version.
                                                   (e.g., 3.0.2 or latest)
  --load-r4 <load-r4>                            If FHIR R4 should be loaded, which version.
                                                   (e.g., 4.0.1 or latest)
  --load-r5 <load-r5>                            If FHIR R5 should be loaded, which version.
                                                   (e.g., 4.4.0 or latest)
  --language-options <language-options>          Language specific options, see documentation
                                                   for more details.
                                                   (e.g., CSharpBasic|namespace=myorg.fhir)
  --version                                      Show version information.
  -?, -h, --help                                 Show help and usage information.
```

## Examples

* Download and parse FHIR R4 (latest published version) into ./fhir, then build a TypeScript file in the current directory
  * `fhir-codegen-cli --load-r4 latest --fhir-spec-directory ./fhir --language TypeScript --output-file ./R4.ts`

* Download and parse FHIR R4 (latest published version) into ./fhir, then build a TypeScript file in the current directory, restricted to just the Resources: Patient, Encounter, and Observation
  * `fhir-codegen-cli --load-r4 latest --fhir-spec-directory ./fhir --language TypeScript --output-file ./R4.ts --export-keys Patient|Encounter|Observation`

* Download and parse the latest published version of each FHIR release into ./fhir, then build a C# file for each in ./cs
  * `fhir-codegen-cli --load-r2 latest --load-r3 latest --load-r4 latest --load-r5 latest --fhir-spec-directory ./fhir --language CSharpBasic --output-file ./cs`

* Download and parse FHIR R4 (latest published version) into ./fhir, then build a C# file in the current directory using the namespace: MyOrg.MyProject.Fhir
  * `fhir-codegen-cli --load-r4 latest --fhir-spec-directory ./fhir --language CSharpBasic --output-file ./cs/R4.cs --language-options CSharpBasic|namespace=MyOrg.MyProject.Fhir`

## Testing

Running `dotnet run -p src/fhir-codegen-test-cli/fhir-codegen-test-cli.csproj` launches a full build and test.

It will generate updated CSharpBasic and TypeScript files for FHIR Versions DSTU2, STU3, R4, and R5 (May 2020).  It will then run each through a build process (requires `dotnet` and `tsc`) to validate there are no syntax errors in any of the generated files.

Note that this test takes several minutes to run, and more tests will be added in the future.

## Langauge Information

  * Info
    
    Basic text output of a version of FHIR for information and testing.

  * CSharpBasic

    Basic C# language bindings, useful for prototyping and small projects.  Exported classes are able to serialize to and parse from FHIR JSON.

  * TypeScript

    Basic TypeScript bindings, useful for prototyping and small projects.  Exported classes are able to serialize to and parse from FHIR JSON.

  * CSharpFirely **EXPERIMENTAL**

    Export base C# classes needed for the Firely-maintained C# API ([FHIR-Net-API](https://github.com/FirelyTeam/fhir-net-api/)).

# Pre-Generated Files

The `generated` directory has static outputs for each of the supported versions of FHIR, in each of the supported languages.

# Additional Documentation

The system is designed to allow developers to add additional languages to be exported.  For more information, please see the [documentation](http://microsoft.github.io/fhir-codegen/).

# Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
