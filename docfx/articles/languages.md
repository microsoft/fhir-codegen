# Export Language Information

While each 'export language' is unique, there are several definitions that have evolved over time and kept prior versions (for existing functionality).  This page groups languages for context.

## C\# Basic Models

These languages are used to generate 'basic' FHIR models in C\#.  That is to say, classes that represent the FHIR+JSON structures and can round-trip in FHIR+JSON.  Initially, these classes were used to bootstrap this project, though only the definitions from DSTU2 are still used for this.

Today, these exports are used as part of the 'generated' definitions to ensure that parsing and export remain consistent in a compilable (testable) format, as well as for prototyping changes to the core FHIR specifications.

### CSharpBasic

Initial basic C# language bindings, useful for prototyping and small projects.  Exported classes are able to serialize to and parse from FHIR+JSON.  This language exports a single file containing all exported artifacts.

| Language Option | Description | Default | Allowed |
| --------- | ----------- | ------- | ------- |
| namespace | Namespace to use when exporting C\# files. | `fhir` | C\# namespace literal strings |


### CSharp2

Second version of basic C# language bindings, useful for prototyping and small projects.  Exported classes are able to serialize to and parse from FHIR+JSON using `System.Text.Json`.  This language exports a directory structure with individual files for each artifact (e.g., resource, value set, etc.).

| Language Option | Description | Default | Allowed |
| --------- | ----------- | ------- | ------- |
| access-modifier | Access modifier for exported elements. | `public` | `public` \| `internal` \| `private` |
| namespace | Namespace to use when exporting C# files. | `Fhir.R{VersionNumber}` | C# namespace literal strings |

## Firely C\#

These language definitions are used to generate the 'generated' portions of the [Firely .Net SDK](https://fire.ly/products/firely-net-sdk/).

Note that changes to these modules must be submitted or approved by Firely SDK maintainers.

### CSharpFirely1

Export base C# classes needed for the Firely-maintained C# API ([FHIR-Net-API](https://github.com/FirelyTeam/fhir-net-api/)), v1.

| Language Option | Description | Default | Allowed |
| --------- | ----------- | ------- | ------- |
| DetailedHeader | If the generator should include the user and date/time information in the header. | `true` | `true` \| `false` |

### CSharpFirely2

Export base C# classes needed for the Firely-maintained C# API ([FHIR-Net-API](https://github.com/FirelyTeam/fhir-net-api/)), v2.

| Language Option | Description | Default | Allowed |
| --------- | ----------- | ------- | ------- |
| subset | Which subset of language exports to make. | `main` | `all` \| `common` \| `main` |
| w5 | If output should include 5 W's mappings. | `true` | `true` \| `false` |

#### Subsets

Note that the `subset` option is used to control which type of objects are exported:
* `all` is used to export all package artifacts.
* `common` is used to export artifacts which have been promoted to the [common](https://github.com/FirelyTeam/firely-net-common/) Firely SDK package.
* `main` is used to export version-specific artifacts, a complement to `common`.

### CSharpFirelyIG

Prototype module for generating Implementation Guide-specific additions to the Firely Net SDK.

## Info
    
This language definition produces a text output of package contents.  This is used as part of the 'generated' file set to ensure that parsing and generation remain consistent.  These files are also useful as a method to quickly find information from a package.

## TypeScript

TypeScript generation includes a few different language export modules with different goals.  Each is detailed below.

### TypeScript

This language exports a single TypeScript file, useful for prototyping and small projects.  Exported classes are able to serialize to and parse from FHIR+JSON.

This language is kept stable as it is used to generate the definitions used in [DefinitelyTyped](https://github.com/DefinitelyTyped/DefinitelyTyped/tree/master/types/fhir) (`@types/fhir`).  Post-processing of this file into the DTS used is done by Vermonster's [fhir-dts-generator](https://github.com/Vermonster/fhir-dts-generator) project.

Note that changes to this module must be submitted or approved by Vermonster.

### TypeScript2

This is the second (internal) iteration of TypeScript exports.  It generates a single file per artifact (e.g., resource, value-set, etc.).  It is a prototype for moving from 'JSON-definition' to 'SDK' territory.

| Language Option | Description | Default | Allowed |
| --------- | ----------- | ------- | ------- |
| namespace | Namespace to use when exporting C# files. | `Fhir.R{VersionNumber}` | C# namespace literal strings |

### TypeScriptSdk

This module is used to generate the [fhir-typescript](https://github.com/fhir-typescript/fhir-typescript) SDK (beta).  While serialization and parsing are FHIR+JSON compatible, the class definitions provide a smoother developer experience (e.g., choice types are a single property with multiple allowed types).

Note that this is the first export module to separate language-specific generation from the file writing.  E.g., the class in `Language/TypeScriptSdk/ModelBuilder.cs` is used to convert the internal normalized but generic models (e.g., `FhirElement`) into TypeScript-specific models (e.g., `TypeScriptSdk.ModelBuilder.ExportElement`).  This style of module is generally preferred moving forward, as it allows for more flexibility in exports (e.g., being able to internalize the models for FHIR R4 when exporting an IG built on that version of FHIR).

Note that changes to this module must be submitted or approved by 'fhir-typescript' maintainers.

## Cytoscape

 **EXPERIMENTAL**

Export a [cytoscape](https://js.cytoscape.org/) data file (JSON).

## OpenAPI

 **EXPERIMENTAL**

This module is used to export [OpenAPI](https://www.openapis.org/) definitions.  Tool-chains that consume OpenAPI definitions are varied and often have quite specific requirements.  To be more widely useful, this module defines *many* options to modify the exported definitions.

Note that this module is still considered experimental/beta at this time.  Note that this module is expected to become 'stable' relatively Soon&trade; (2022-2023).

| Language Option | Description | Default | Allowed |
| --------- | ----------- | ------- | ------- |
| SchemaLevel | How much schema to include. | `detailed` | `minimal` \| `names` \| `detailed` |
| FhirJson | If paths should explicitly support FHIR+JSON. | `true` | `true` \| `false` |
| FhirXml | If paths should explicitly support FHIR+XML. | `false` | `true` \| `false` |
| PatchJson | If PATCH operations should explicitly support json-patch. | `false` | `true` \| `false` |
| PatchXml | If PATCH operations should explicitly support XML-patch. | `false` | `true` \| `false` |
| PatchFhir | If PATCH operations should explicitly support FHIR types. | `true` | `true` \| `false` |
| SearchSupport | Supported search methods. | `both` | `both` \| `get` \| `post` \| `none` |
| SearchPostParams | Where search params should appear in post-based search. | `body` | `body` \| `query` \| `both` \| `none` |
| History | If _history GET operations should be included. | `false` | `true` \| `false` |
| Metadata | If the JSON should include a link to /metadata. | `true` | `true` \| `false` |
| BundleOperations | If the generator should include /Bundle, etc.. | `true` | `true` \| `false` |
| ReadOnly | If the output should only contain GET operations. | `false` | `true` \| `false` |
| WriteOnly | If the output should only contain POST/PUT/DELETE operations. | `false` | `true` \| `false` |
| Descriptions | If properties should include descriptions. | `true` | `true` \| `false` |
| DescriptionMaxLen | Maximum length of descriptions, if being validated. | `60` | Positive Integer values |
| DescriptionValidation | If descriptions are required and should be validated. | `false` | `true` \| `false` |
| ExpandProfiles | If types should expand based on allowed profiles. | `true` | `true` \| `false` |
| ExpandReferences | If types should expand through references. | `true` | `true` \| `false` |
| MaxRecursions | Maximum depth to expand recursions. | `0` | Positive Integer values |
| Minify | If the output JSON should be minified. | `false` | `true` \| `false` |
| OpenApiVersion | Open API version to use. | `2` | `2` \| `3` |
| OperationCase | Case of the first letter of Operation IDs. | `upper` | `upper` \| `lower` |
| RemoveUncommonFields | If the generator should remove some uncommon fields. | `false` | `true` \| `false` |
| Schemas | If schemas should be included. | `true` | `true` \| `false` |
| SchemasInline | If the output should inline all schemas (no inheritance). | `false` | `true` \| `false` |
| SingleResponses | If operations should only include a single response. | `false` | `true` \| `false` |
| Summaries | If responses should include summaries. | `true` | `true` \| `false` |
| Title | Title to use in the Info section. | |