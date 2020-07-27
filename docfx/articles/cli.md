# CLI Usage

```dos
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

## Language Information

  * Info
    
    Basic text output of a version of FHIR for information and testing.

  * CSharpBasic

    Basic C# language bindings, useful for prototyping and small projects.  Exported classes are able to serialize to and parse from FHIR JSON.

  * TypeScript

    Basic TypeScript bindings, useful for prototyping and small projects.  Exported classes are able to serialize to and parse from FHIR JSON.

  * CSharpFirely **EXPERIMENTAL**

    Export base C# classes needed for the Firely-maintained C# API ([FHIR-Net-API](https://github.com/FirelyTeam/fhir-net-api/)).


## CLI Examples

* Download and parse FHIR R4 (latest published version) into ./fhir, then build a TypeScript file in the current directory
  * `fhir-codegen-cli --load-r4 latest --fhir-spec-directory ./fhir --language TypeScript --output-path ./R4.ts`

* Download and parse FHIR R4 (latest published version) into ./fhir, then build a TypeScript file in the current directory, restricted to just the Resources: Patient, Encounter, and Observation
  * `fhir-codegen-cli --load-r4 latest --fhir-spec-directory ./fhir --language TypeScript --output-path ./R4.ts --export-keys Patient|Encounter|Observation`

* Download and parse the latest published version of each FHIR release into ./fhir, then build a C# file for each in ./cs
  * `fhir-codegen-cli --load-r2 latest --load-r3 latest --load-r4 latest --load-r5 latest --fhir-spec-directory ./fhir --language CSharpBasic --output-path ./cs`

* Download and parse FHIR R4 (latest published version) into ./fhir, then build a C# file in the current directory using the namespace: MyOrg.MyProject.Fhir
  * `fhir-codegen-cli --load-r4 latest --fhir-spec-directory ./fhir --language CSharpBasic --output-path ./cs/R4.cs --language-options CSharpBasic|namespace=MyOrg.MyProject.Fhir`
