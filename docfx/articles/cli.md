# Command Line Interface

The command-line-interface (CLI) project is intended as a utility suitable for both manual and automated (e.g., CI Pipeline) use.

The CLI is focused on language-export.  It includes options to specify inputs (e.g., which FHIR packages to download/parse, if additional filters should be used, etc.) and outputs (e.g., export language, destination directory, etc.).

## Options

The CLI self-reports all options with descriptions:

```dos
Usage:
  fhir-codegen-cli [options]

Options:
  -o, --output-path <output-path>                File or directory to write output.

  --package-directory <package-directory>        The path to a local directory for FHIR packages,
                                                 if different than the default 
                                                 FHIR cache (~/.fhir); e.g., (.../fhirPackages)).

  -l, --language <language>                      Name of the language to export (default: 
                                                 Info|TypeScript|CSharpBasic).
  --language-help                                Display languages and their options. 
                                                 [default: False]
  --language-options, --opts <language-options>  Language specific options, see documentation for
                                                 more details. Example: 
                                                 Lang1|opt=a|opt2=b|Lang2|opt=tt|opt3=oo.
  --language-input-dir <language-input-dir>      The full path to a local directory to pass 
                                                 additional content to languages.

  --offline-mode                                 Offline mode (will not download packages).
                                                 [default: False]
  -k, --export-keys <export-keys>                '|' separated list of items to export (not present
                                                 to export everything).
  --official-expansions-only                     Set to restrict value-sets to only official
                                                 expansions. [default: False]
  --experimental, --include-experimental         If the output should include structures marked
                                                 experimental. [default: False]
  --export-types, --types <export-types>         Types of FHIR structures to export
                                                 (primitive|complex|resource|interaction|enum),
                                                 default is all.
  --extension-support <extension-support>        The level of extensions to include (none|
                                                 official|officialNonPrimitive|nonPrimitive|all),
                                                 default is nonPrimitive.
  --fhir-server-url, --server <fhir-server-url>  FHIR Server URL to pull a CapabilityStatement 
                                                 or Conformance from. The server
                                                 must provide application/fhir+json support.

  -p, --packages <packages>                      '|' separated list of packages, with or without
                                                 version numbers, e.g.,
                                                 hl7.fhir.r4.core#4.0.1|hl7.fhir.us.core#latest.
  --load-DSTU2, --load-r2 <load-DSTU2>           If FHIR DSTU2 should be loaded, which version
                                                 (e.g., 1.0.2 or latest)
  --load-r3, --load-STU3 <load-STU3>             If FHIR STU3 should be loaded, which version
                                                 (e.g., 3.0.2 or latest)
  --load-r4 <load-r4>                            If FHIR R4 should be loaded, which version
                                                 (e.g., 4.0.1 or latest)
  --load-r4b <load-r4b>                          If FHIR R4B should be loaded, which version
                                                 (e.g., 4.3.0 or latest)
  --load-r5 <load-r5>                            If FHIR R5 should be loaded, which version
                                                 (e.g., 5.0.0-ballot or latest)
  --ci-branch <ci-branch>                        If loading from the CI server, the name of the 
                                                 branch to use.

  -v, --verbose                                  Show verbose output. [default: False]
  --version                                      Show version information
  -?, -h, --help                                 Show help and usage information
```

## Package Source

By default, the CLI will use the user's FHIR cache directory (`~/.fhir`).  This directory is common to many tools in the FHIR ecosystem, such as the publication tooling (e.g., IG Publisher) and package-based tools from other companies (e.g., Firely).

If a different directory is desired (e.g., CI Builds in a docker image, 'clean' builds from a specific location, etc.), the `--package-directory` option can be used.

## Specifying Packages

Initially, the CLI only supported loading FHIR Core specifications (e.g., FHIR R4).  In order to maintain backwards compatibility, explicit options to load core specifications are still supported (e.g., `--load-r4 latest`), though users should consider changing to the package directive format (e.g., `--packages hl7.fhir.r4.core#latest`).

## Languages

fhir-codegen supports several export languages, each with their own configuration (options), additional files, and artifact support.  Since the language support is common to all projects in the repository, more information can be found on the [Languages Page](languages.md)

## Filtering

Several options relate to filtering the artifacts included in a package, e.g., `--export-keys` is used to provide an explicit list of artifacts that should be allowed; `--server` is used to filter packages based on the capabilities of an existing FHIR server; etc..

These filters are applied after packages are loaded.  For example, a user may want to generate *only* definitions for FHIR R4 Patient, Encounter, Observation, and Bundle resources.  A user can specify to load the FHIR R4 specification and use the option `--export-keys Patient|Encounter|Observation|Bundle` to filter the specification.  Note that the filtering is "smart" in that all definitions required by those resources (e.g., data types, etc.) will still be included.

Note that some languages will provide 'object' stubs for elements that cannot be defined, while others may exclude them entirely.

## Examples

* Download and parse FHIR R4 (latest published version) into the user FHIR cache, then build a TypeScript file in the current directory
  * `fhir-codegen-cli -p hl7.fhir.r4#latest --language TypeScript --output-path ./R4.ts`

* Download and parse FHIR R4 (latest published version) into the user FHIR cache, then build a TypeScript file in the current directory, restricted to just the Resources: Patient, Encounter, and Observation
  * `fhir-codegen-cli --load-r4 latest --language TypeScript --output-path ./R4.ts --export-keys Patient|Encounter|Observation`

* Download and parse the latest published version of each FHIR release into the user FHIR cache, then build a C# file for each in ./cs
  * `fhir-codegen-cli --load-r2 latest --load-r3 latest --load-r4 latest --load-r5 latest --language CSharpBasic --output-path ./cs`

* Download and parse FHIR R4 (latest published version) into the user FHIR cache, then build a C# file in the current directory using the namespace: MyOrg.MyProject.Fhir
  * `fhir-codegen-cli -p hl7.fhir.r4#latest --language CSharpBasic --output-path ./cs/R4.cs --language-options CSharpBasic|namespace=MyOrg.MyProject.Fhir`
