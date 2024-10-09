# fhir-codegen

A .Net application, library, and related utilities to work with FHIR specifications.

# Documentation

Detailed documentation can be found on the [documentation site](https://microsoft.github.io/fhir-codegen/).

# Projects in this Repository

Project source code is hosted on [GitHub](https://github.com/microsoft/fhir-codegen).

All projects are currently built on .Net 8.0 and tested on multiple platforms, though Windows is the primary development platform. The
.Net 8.0 SDK and runtimes are available for free at: https://dotnet.microsoft.com/en-us/download .

## Microsoft.Health.Fhir.CodeGenCommon

This project is a class library that contains the common (normalized) models used in these projects.  The project is lightweight with
minimal dependencies and used to ensure that additional projects and external development have access to models.

Detailed information about the models can be found in the [Common Models Documentation](https://microsoft.github.io/fhir-codegen/api/Microsoft.Health.Fhir.CodeGenCommon.Models.html).

## Microsoft.Health.Fhir.CrossVersion

This project is a class library that provides *basic* support for loading specifications from multiple versions of FHIR. The library is scoped
to the needs of this project and is not intended to be a full FHIR library. It is incomplete in support, though we do plan on expanding in the
future.

## Microsoft.Health.Fhir.MappingLanguage

This project is a class library that provides support for working with the FHIR Mapping Language (FML). Today, content is focused on parsing FML
and providing useful abstractions for consumption.

## Microsoft.Health.Fhir.CodeGen

This project is a class library that contains the logic used to process FHIR specifications.  For example, this project contains the code to:
* download packages,
* resolve canonical URLs,
* normalize different versions of FHIR,
* read metadata (CapabilityStatement or Conformance) from FHIR servers,
* compare packages or artifacts,
* convert normalized models into language-specific models,
* etc.

More information about this project can be found in the [API Documentation](https://microsoft.github.io/fhir-codegen/api/index.html).

### Export Languages

The projects in this repository can be used to translate FHIR packages (core or IG) into other forms - e.g., programming language definitions
(C#, TypeScript, Ruby, etc.), other language definitions (FHIR Shorthand, OpenAPI, etc.), or data formats (Info text, etc.).

More information about current languages can be found on the [Export Languages Page](https://microsoft.github.io/fhir-codegen/articles/languages.html). 
Information about adding new languages can be found on the [Extending Page](https://microsoft.github.io/fhir-codegen/articles/extending.html).


## fhir-codegen

This project is a command-line application that can be used to perform export operations (e.g., for CI Pipelines).  Generally, it can:
* manage the FHIR Package Cache (`~/.fhir`) - add/update packages
* perform exports / transforms of FHIR packages
* compare specification packages

To run this project from a command line:
* `dotnet run --project src/fhir-codegen-cli/fhir-codegen-cli.csproj -- [command] [options]`
or you can build a release version to run:
* `dotnet build -c Release`
* `./src/fhir-codegen/bin/Release/net8.0/fhir-codegen-cli.exe`

```
Description:
  A utility for processing FHIR packages into other formats/languages.

Usage:
  fhir-codegen [command] [options]

Options:
  --fhir-cache <fhir-cache>                                           Location of the FHIR cache (none specified defaults to user .fhir directory). []
  --use-official-registries                                           Use official FHIR registries to resolve packages. []
  --additional-fhir-registry-urls <additional-fhir-registry-urls>     Additional FHIR registry URLs to use. []
  --additional-npm-registry-urls <additional-npm-registry-urls>       Additional NPM registry URLs to use. []
  --output-dir, --output-directory, --output-path <output-directory>  File or directory to write output. []
  --output-filename <output-filename>                                 Filename to write output. []
  -p, --load-package, --package <load-package>                        Package to load, either as directive ([name]#[version/literal]) or URL. []
  --auto-load-expansions                                              When loading core packages, load the expansions packages automatically. []
  --resolve-dependencies                                              Resolve package dependencies. []
  --load-structures                                                   Types of FHIR structures to load. []
    opt: CapabilityStatement
    opt: CodeSystem
    opt: Compartment
    opt: ComplexType
    opt: ConceptMap
    opt: Extension
    opt: ImplementationGuide
    opt: Interface
    opt: LogicalModel
    opt: NamingSystem
    opt: Operation
    opt: PrimitiveType
    opt: Profile
    opt: Resource
    opt: SearchParameter
    opt: StructureMap
    opt: Unknown
    opt: ValueSet

  --export-keys <export-keys>                                         Keys of FHIR structures to export (e.g., Patient), empty means all. []
  --load-canonical-examples                                           Load canonical examples from packages. []
  --offline                                                           Offline mode (will not download missing packages). []
  --fhir-version <fhir-version>                                       FHIR version to use. []
  --version                                                           Show version information
  -?, -h, --help                                                      Show help and usage information

Commands:
  generate     Generate output from a FHIR package and exit.
  compare      Compare two sets of packages.
  gui          Launch the GUI (experimental).
```

More information about this project can be found in the [Command Line Documentation](https://microsoft.github.io/fhir-codegen/articles/cli.html).

### Examples

* Download and parse FHIR R4 (latest published version) into the user FHIR cache, then build a TypeScript file in the current directory
  * `fhir-codegen generate TypeScript -p hl7.fhir.r4.core --output-path ./R4.ts`

* Download and parse FHIR R4 (latest published version) into the user FHIR cache, then build a TypeScript file in the current directory, restricted to just the Resources: Patient, Encounter, and Observation
  * `fhir-codegen generate TypeScript -p hl7.fhir.r4.core --output-path ./R4.ts --export-keys Patient|Encounter|Observation`

* Download and parse FHIR R4 (latest published version) into the user FHIR cache, reach out the Firely Test Server, and build a set of OpenAPI definitions
  * `generate OpenApi --fhir-server-url http://server.fire.ly/r4 -p hl7.fhir.r4.core#4.0.1 --output-path ./FS-OpenApi-R4 --include-experimental --schema-level names --metadata true --multi-file true --single-responses false --resolve-server-canonicals false --resolve-external-canonicals false --basic-scopes-only true`

# Build and Running From OCI (i.e. Docker) Images

If you do not have .NET installed and wish to use the command-line utilities, you may instead build an OCI container to run it for you. The included [Dockerfile](./Dockerfile) has been tested with Docker Desktop but should work also with `buildah` and container runtimes such as `podman` and `cri-o`. To build your own image for your native CPU architecture:

  `docker build -t <whatever>/fhir-codegen:latest .`

Alternatively, to build and push for multiple CPU architectures,

  `docker buildx build --platform linux/arm64,linux/amd64 --push -t <whatever>/fhir-codegen:latest .`

The image is configured to run the CLI as its default entry point. To generate FHIR 4.0.1 TypeScript interfaces to a file on your _host_ computer's ./out directory, for example, simply pass the arguments as input to the container runtime, like so:

  ``docker run -it --rm -v `pwd`/out:/out <whatever>/fhir-codegen:latest --load-r4 4.0.1 --language TypeScript --output-path /out``

The above command binds an ./out directory (within your current working directory) to the output directory of fhir-codegen-cli running within the container, so you output will persist after the container exits and is removed. 

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
