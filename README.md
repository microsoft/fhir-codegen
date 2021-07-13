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
  --offline-mode                                 Offline mode .
                                                    (will not download missing specs)
  --language <language>                          Name of the language to export.
                                                    (default: Info|TypeScript|CSharpBasic)
  --export-keys <export-keys>                    '|' separated list of items to export.
                                                    (not present to export everything)
  --load-r2 <load-r2>                            If FHIR R2 should be loaded, which version.
                                                    (e.g., 1.0.2 or latest)
  --load-r3 <load-r3>                            If FHIR R3 should be loaded, which version.
                                                    (e.g., 3.0.2 or latest)
  --load-r4 <load-r4>                            If FHIR R4 should be loaded, which version.
                                                    (e.g., 4.0.1 or latest).
  --load-r5 <load-r5>                            If FHIR R5 should be loaded, which version.
                                                    (e.g., 4.4.0 or latest)
  --language-help                                Show supported languages and their options.
  --language-options <language-options>          Language specific options, see documentation
                                                   for more details.
                                                   (e.g., CSharpBasic|namespace=myorg.fhir)
  --official-expansions-only                     True to restrict value-sets exported to only
                                                    official expansions.
                                                    (default: false)
  --fhir-server-url <fhir-server-url>            FHIR Server URL to pull a CapabilityStatement
                                                    (or Conformance) from. Only supports
                                                    application/fhir+json.
  --include-experimental                         If the output should include structures
                                                    marked experimental.
                                                    (default: false)
  --export-types                                 Which FHIR classes types to export:
                                                    primitive|complex|resource|interaction|enum.
                                                    (default: all types)
  --version                                      Show version information
  -?, -h, --help                                 Show help and usage information
```

## Examples

* Download and parse FHIR R4 (latest published version) into ./fhir, then build a TypeScript file in the current directory
  * `fhir-codegen-cli --load-r4 latest --fhir-spec-directory ./fhir --language TypeScript --output-path ./R4.ts`

* Download and parse FHIR R4 (latest published version) into ./fhir, then build a TypeScript file in the current directory, restricted to just the Resources: Patient, Encounter, and Observation
  * `fhir-codegen-cli --load-r4 latest --fhir-spec-directory ./fhir --language TypeScript --output-path ./R4.ts --export-keys Patient|Encounter|Observation`

* Download and parse the latest published version of each FHIR release into ./fhir, then build a C# file for each in ./cs
  * `fhir-codegen-cli --load-r2 latest --load-r3 latest --load-r4 latest --load-r5 latest --fhir-spec-directory ./fhir --language CSharpBasic --output-path ./cs`

* Download and parse FHIR R4 (latest published version) into ./fhir, then build a C# file in the current directory using the namespace: MyOrg.MyProject.Fhir
  * `fhir-codegen-cli --load-r4 latest --fhir-spec-directory ./fhir --language CSharpBasic --output-path ./cs/R4.cs --language-options CSharpBasic|namespace=MyOrg.MyProject.Fhir`

## Langauge Information

### CSharpBasic

Basic C# language bindings, useful for prototyping and small projects.  Exported classes are able to serialize to and parse from FHIR JSON.

| Language Option | Description | Default |
| --------- | ----------- | ------- |
| namespace | Namespace to use when exporting C# files. | fhir |

### CSharpFirely1

Export base C# classes needed for the Firely-maintained C# API ([FHIR-Net-API](https://github.com/FirelyTeam/fhir-net-api/)), v1.

| Language Option | Description | Default |
| --------- | ----------- | ------- |
| DetailedHeader | If the generator should include the user and date/time information in the header (true\|false). | true |

### CSharpFirely2

Export base C# classes needed for the Firely-maintained C# API ([FHIR-Net-API](https://github.com/FirelyTeam/fhir-net-api/)), v2.

### Cytoscape **EXPERIMENTAL**

Export a [cytoscape](https://js.cytoscape.org/) data file (JSON).

### Info

Basic text output of a version of FHIR for information and testing.

### OpenAPI **EXPERIMENTAL**

Export an OpenAPI 2 or 3 JSON version of the standard.

| Language Option | Description | Default |
| --------- | ----------- | ------- |
| BundleOperations | If the generator should include /Bundle, etc. (true\|false). | true |
| Descriptions | If properties should include descriptions (true\|false). | true |
| DescriptionMaxLen | Maximum length of descriptions, if being validated.  | 60 |
| DescriptionValidation | If descriptions are required and should be validated (false\|true). | false |
| ExpandProfiles | If types should expand based on allowed profiles (true\|false). | true |
| FhirJson | If paths should explicitly support FHIR+JSON (true\|false). | true |
| FhirXml | If paths should explicitly support FHIR+XML (false\|true). | false |
| History | If _history GET operations should be included (false\|true) | false |
| MaxRecurisions | Maximum depth to expand recursions. | 0 |
| Metadata | If the JSON should include a link to /metadata (false\|true). | false |
| Minify | If the output JSON should be minified (false\|true). | false |
| OpenApiVersion | Open API version to use (2, 3). | 2 |
| OperationCase | Case of the first letter of Operation IDs (upper\|lower). | upper |
| ReadOnly | If the output should only contain GET operations (false\|true). | false |
| RemoveUncommonFields | If the generator should remove some uncommon fields (false\|true) | false |
| Schemas | If schemas should be included (true\|false). | true |
| SchemasInline | If the output should inline all schemas (no inheritance) (false\|true). | false |
| SingleResponses | If operations should only include a single response (false\|true). | false |
| Summaries | If responses should include summaries (true\|false). | true |
| Title | Title to use in the Info section. | `FHIR {FHIR Release Name}:{FHIR Version String} |
| WriteOnly | If the output should only contain POST/PUT/DELETE operations (false\|true). | false |

### TypeScript

Basic TypeScript bindings, useful for prototyping and small projects.  Exported classes are able to serialize to and parse from FHIR JSON.

# Developing/Building

`dotnet` required. [Cross platform installation instructions](https://dotnet.microsoft.com/download).

Build with `dotnet build`. Run the cli project with `dotnet run -p src/fhir-codegen-cli`.

# Testing

Running `dotnet run -p src/fhir-codegen-test-cli/fhir-codegen-test-cli.csproj` launches a full build and test.

It will generate updated CSharpBasic and TypeScript files for FHIR Versions DSTU2, STU3, R4, and R5 (May 2020).  It will then run each through a build process (requires `dotnet` for C# and `tsc` for TypeScript) to validate there are no syntax errors in any of the generated files.

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

## Requirements

In order to run TypeScript tests, the system must be able to find the 'tsc' (TypeScript compile) command.  Note that it must be installed and accessible by the test application (e.g., `npm install -g typescript`).

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
