# fhir-codegen

A .Net library and related utilities to work with FHIR specifications.

# Documentation

Detailed documentation can be found on the [documentation site](https://microsoft.github.io/fhir-codegen/).

# Projects in this Repository

Project source code is hosted on [GitHub](https://github.com/microsoft/fhir-codegen).

All projects are currently built on .Net 6.0 and tested on multiple platforms, though Windows is the primary development platform.  The .Net 6.0 SDK and runtimes are available for free at: https://dotnet.microsoft.com/en-us/download .

## Microsoft.Health.Fhir.CodeGenCommon

This project is a class library that contains the common (normalized) models used in these projects.  The project is lightweight and used to ensure that additional projects (e.g., a WASM UI) have access to all the models.

Detailed information about the models can be found in the [Common Models Documentation](https://microsoft.github.io/fhir-codegen/api/Microsoft.Health.Fhir.CodeGenCommon.Models.html).

## Microsoft.Health.Fhir.SpecManager

This project is a class library that contains the logic used in the various projects.  For example, this project contains the code to:
* download packages,
* resolve canonical URLs,
* normalize different versions of FHIR,
* read metadata (CapabilityStatement or Conformance) from FHIR servers,
* compare packages or artifacts,
* convert normalized models into language-specific models,
* etc.

More information about this project can be found in the [API Documentation](https://microsoft.github.io/fhir-codegen/api/index.html).

### Export Languages

The projects in this repository can be used to translate FHIR packages (core or IG) into other forms - e.g., programming language definitions (C#, TypeScript, etc.), or data files for consumption (Info, Cytoscape, etc.).

More information about current languages can be found on the [Export Languages Page](https://microsoft.github.io/fhir-codegen/articles/languages.html).  Information about adding new languages can be found on the [Extending Page](https://microsoft.github.io/fhir-codegen/articles/extending.html).


## FhirCodeGenBlazor

This project is a server-side Blazor application that can be used to interact with the code-generation library.  Generally, it can:
* manage the FHIR Package Cache (`~/.fhir`) - add/update/remove packages
* browse package artifacts
* search across element information (e.g., resource/logical models elements)
* compare packages or artifacts (Diff Tool)
* perform exports
* etc.

To run this project from a command line:
* `dotnet run --project src/FhirCodeGenBlazor/FhirCodeGenBlazor.csproj`
or, you can build a release version to run:
* `dotnet build src/FhirCodeGenBlazor/FhirCodeGenBlazor.csproj -c Release`
* `dotnet ./src/FhirCodeGenBlazor/bin/Release/net6.0/FhirCodeGenBlazor.dll`

More information about this project can be found in the [API Documentation](https://microsoft.github.io/fhir-codegen/api/index.html).

More information about this project can be found in the [Blazor UI Documentation](https://microsoft.github.io/fhir-codegen/articles/blazorui.html).


## fhir-codegen-cli

This project is a command-line application that can be used to perform export operations (e.g., for CI Pipelines).  Generally, it can:
* manage the FHIR Package Cache (`~/.fhir`) - add/update packages
* perform exports / transforms of FHIR packages

To run this project from a command line:
* `dotnet run --project src/fhir-codegen-cli/fhir-codegen-cli.csproj -- [options]`
or, you can build a release version to run:
* `dotnet build src/fhir-codegen-cli/fhir-codegen-cli.csproj -c Release`
* `dotnet ./src/fhir-codegen-cli/bin/Release/net6.0/fhir-codegen-cli.dll`

```
Usage:
  fhir-codegen-cli [options]

Options:
  -o, --output-path <output-path>                File or directory to write output.
  --package-directory <package-directory>        The path to a local directory for FHIR packages, if different than the default 
                                                 FHIR cache (~/.fhir); e.g., (.../fhirPackages)).
  -l, --language <language>                      Name of the language to export (default: Info|TypeScript|CSharpBasic).
  --language-help                                Display languages and their options. [default: False]
  --language-options, --opts <language-options>  Language specific options, see documentation for more details. Example: 
                                                 Lang1|opt=a|opt2=b|Lang2|opt=tt|opt3=oo.
  --language-input-dir <language-input-dir>      The full path to a local directory to pass additional content to languages.
  --offline-mode                                 Offline mode (will not download missing packages). [default: False]
  -k, --export-keys <export-keys>                '|' separated list of items to export (not present to export everything).
  --official-expansions-only                     Set to restrict value-sets to only official expansions. [default: False]
  --experimental, --include-experimental         If the output should include structures marked experimental. [default: False]
  --export-types, --types <export-types>         Types of FHIR structures to export (primitive|complex|resource|interaction|enum),
                                                 default is all.
  --extension-support <extension-support>        The level of extensions to include (none|official|officialNonPrimitive|nonPrimitive|all),
                                                 default is nonPrimitive.
  --fhir-server-url, --server <fhir-server-url>  FHIR Server URL to pull a CapabilityStatement or Conformance from. The server
                                                 must provide application/fhir+json support.
  -p, --packages <packages>                      '|' separated list of packages, with or without version numbers,
                                                 e.g., hl7.fhir.r4.core#4.0.1|hl7.fhir.us.core#latest.
  --load-DSTU2, --load-r2 <load-DSTU2>           If FHIR DSTU2 should be loaded, which version (e.g., 1.0.2 or latest)
  --load-r3, --load-STU3 <load-STU3>             If FHIR STU3 should be loaded, which version (e.g., 3.0.2 or latest)
  --load-r4 <load-r4>                            If FHIR R4 should be loaded, which version (e.g., 4.0.1 or latest)
  --load-r4b <load-r4b>                          If FHIR R4B should be loaded, which version (e.g., 4.3.0 or latest)
  --load-r5 <load-r5>                            If FHIR R5 should be loaded, which version (e.g., 5.0.0-ballot or latest)
  --ci-branch <ci-branch>                        If loading from the CI server, the name of the branch to use.
  -v, --verbose                                  Show verbose output. [default: False]
  --version                                      Show version information
  -?, -h, --help                                 Show help and usage information
```

More information about this project can be found in the [Command Line Documentation](https://microsoft.github.io/fhir-codegen/articles/cli.html).

### Examples

* Download and parse FHIR R4 (latest published version) into the user FHIR cache, then build a TypeScript file in the current directory
  * `fhir-codegen-cli -p hl7.fhir.r4#latest --language TypeScript --output-path ./R4.ts`

* Download and parse FHIR R4 (latest published version) into the user FHIR cache, then build a TypeScript file in the current directory, restricted to just the Resources: Patient, Encounter, and Observation
  * `fhir-codegen-cli --load-r4 latest --language TypeScript --output-path ./R4.ts --export-keys Patient|Encounter|Observation`

* Download and parse the latest published version of each FHIR release into the user FHIR cache, then build a C# file for each in ./cs
  * `fhir-codegen-cli --load-r2 latest --load-r3 latest --load-r4 latest --load-r5 latest --language CSharpBasic --output-path ./cs`

* Download and parse FHIR R4 (latest published version) into the user FHIR cache, then build a C# file in the current directory using the namespace: MyOrg.MyProject.Fhir
  * `fhir-codegen-cli -p hl7.fhir.r4#latest --language CSharpBasic --output-path ./cs/R4.cs --language-options CSharpBasic|namespace=MyOrg.MyProject.Fhir`

## fhir-codegen-test-cli

This project is a **minimal** test harness for generated TypeScript and CSharp code.  It can be used to ensure that the core parsing and generation is working properly, and potentially as an example for other language outputs.

To run this project from a command line:
* `dotnet run --project src/fhir-codegen-test-cli/fhir-codegen-test-cli.csproj -- [options]`

It will use generated CSharpBasic and TypeScript files for FHIR Versions DSTU2, STU3, R4, and R5.  It will then run each through a build process (requires `dotnet` for C# and `tsc` for TypeScript) to validate there are no syntax errors in any of the generated files.

Note that this test takes several minutes to run.

## Usage
```
fhir-codegen-test-cli:
  The FHIR CodeGen Test CLI.

Usage:
  fhir-codegen-test-cli [options]

Options:
  --repo-root-path <repo-root-path>    The path to the repository root (if not CWD).
  --verbose                            True to display all output (default: false)
  --fixed-format-statistics            True to output *only* test run statistics:
                                         #run[tab]#passed[tab]#failed[tab]#skipped
                                         (default: false)
  --errors-to-std-error                True to write errors to stderr instead of stdout.
                                         (default: False)
  --version                            Show version information
  -?, -h, --help                       Show help and usage information
```

## Requirements

In order to run TypeScript tests, the system must be able to find the 'tsc' (TypeScript compile) command.  Note that it must be installed and accessible by the test application (e.g., `npm install -g typescript`).

## fhirCsR2

This is a library project used to isolate the FHIR DSTU2 definitions.  All other versions of FHIR are dynamically parsed, but there has not been justification to port this forward - no technical corrections for DSTU2 are expected, so this is *mostly* considered legacy.

# Pre-Generated Files

The `generated` directory has static outputs for each of the supported versions of FHIR, in some of the supported languages.  These files are used to validate changes to the core loading and parsing, but may be useful otherwise.


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

# Trademarks

FHIR&reg; is the registered trademark of HL7 and is used under Community Project guidelines.  This project is not affiliated with, or approved or sponsored by, HL7.