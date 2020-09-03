# CLI Usage

```dos
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
                                                    primitive|complex|resource|interaction|enum
                                                    (default: all types)
  --version                                      Show version information
  -?, -h, --help                                 Show help and usage information
```

## Language Information

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

## CLI Examples

* Download and parse FHIR R4 (latest published version) into ./fhir, then build a TypeScript file in the current directory
  * `fhir-codegen-cli --load-r4 latest --fhir-spec-directory ./fhir --language TypeScript --output-path ./R4.ts`

* Download and parse FHIR R4 (latest published version) into ./fhir, then build a TypeScript file in the current directory, restricted to just the Resources: Patient, Encounter, and Observation
  * `fhir-codegen-cli --load-r4 latest --fhir-spec-directory ./fhir --language TypeScript --output-path ./R4.ts --export-keys Patient|Encounter|Observation`

* Download and parse the latest published version of each FHIR release into ./fhir, then build a C# file for each in ./cs
  * `fhir-codegen-cli --load-r2 latest --load-r3 latest --load-r4 latest --load-r5 latest --fhir-spec-directory ./fhir --language CSharpBasic --output-path ./cs`

* Download and parse FHIR R4 (latest published version) into ./fhir, then build a C# file in the current directory using the namespace: MyOrg.MyProject.Fhir
  * `fhir-codegen-cli --load-r4 latest --fhir-spec-directory ./fhir --language CSharpBasic --output-path ./cs/R4.cs --language-options CSharpBasic|namespace=MyOrg.MyProject.Fhir`
