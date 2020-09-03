# API Documentation

This section contains source code information for:

## Executable: [FhirCodegenCli](FhirCodegenCli.html)

A command-line tool for downloading, parsing, and exporting various versions of the FHIR specifications to other computer languages.

## Executable: [FhirCodegenTestCli](FhirCodegenTestCli.html)

A command-line tool for exercising the code generation functionality.

## Library: [Microsoft.Health.Fhir.SpecManager](Microsoft.Health.Fhir.SpecManager.html)

Main library to handling downloading, parsing, and exporting various versions of the FHIR specifications.

Parent namespace for all library functionality.

## Library Namespace: [Microsoft.Health.Fhir.SpecManager.Converters](Microsoft.Health.Fhir.SpecManager.Converters.html)

Classes to convert from various FHIR versions into the internal structures used in the library.

Each version-specific loader implements `IFhirConverter`.

## Library Namespaces for Specific FHIR Versions

* [Microsoft.Health.Fhir.SpecManager.fhir.r2](Microsoft.Health.Fhir.SpecManager.fhir.r2.html)

  Classes for parsing FHIR DSTU2 (R2) JSON files.  Validated against version 1.0.2.

* [Microsoft.Health.Fhir.SpecManager.fhir.r3](Microsoft.Health.Fhir.SpecManager.fhir.r3.html)

  Classes for parsing FHIR STU3 (R3) JSON files.  Validated against version 3.0.2.

* [Microsoft.Health.Fhir.SpecManager.fhir.r4](Microsoft.Health.Fhir.SpecManager.fhir.r4.html)

  Classes for parsing FHIR R4 JSON files.  Validated against version 4.0.1.

* [Microsoft.Health.Fhir.SpecManager.fhir.r5](Microsoft.Health.Fhir.SpecManager.fhir.r5.html)

  Classes for parsing FHIR R5 JSON files.  Validated against version 4.5.0 (September 2020) and 4.4.0 (May 2020).

## Library Namespace: [Microsoft.Health.Fhir.SpecManager.Language](Microsoft.Health.Fhir.SpecManager.Language.html)

Classes used to export different languages from a loaded version of FHIR.

* Interface [ILanguage](Microsoft.Health.Fhir.SpecManager.Language.ILanguage.html)

  Interface used to define a language available for export.

* Class [ExportStreamWriter](Microsoft.Health.Fhir.SpecManager.Language.ExportStreamWriter.html)

  Extended `System.IO.StreamWriter` used for internally-defined languages.  Includes convenience functions to ease exporting code files.

* Class [LanguageHelper](Microsoft.Health.Fhir.SpecManager.Language.LanguageHelper.html)

  Utilities for working with Languages for export.

* Internally Defined Langauges
  * [CSharpBasic](Microsoft.Health.Fhir.SpecManager.Language.CSharpBasic.html)

    Basic C# language bindings, useful for prototyping and small projects.  Exported classes are able to serialize to and parse from FHIR JSON.

  * [CSharpFirely1](Microsoft.Health.Fhir.SpecManager.Language.CSharpFirely1.html)

    Export base C# classes needed for the Firely-maintained C# API ([FHIR-Net-API](https://github.com/FirelyTeam/fhir-net-api/)). Version 1.x, compatible with legacy T4 templates.

  * [CSharpFirely2](Microsoft.Health.Fhir.SpecManager.Language.CSharpFirely2.html)

    Export base C# classes needed for the Firely-maintained C# API ([FHIR-Net-API](https://github.com/FirelyTeam/fhir-net-api/)). Version 2.x, new development.

  * [Cytoscape](Microsoft.Health.Fhir.SpecManager.Language.Cytoscape.html) **EXPERIMENTAL**
    
    Export a [cytoscape](https://js.cytoscape.org/) data file (JSON).

  * [Info](Microsoft.Health.Fhir.SpecManager.Language.Info.html)
    
    Basic text output of a version of FHIR for information and testing.

  * [OpenAPI](Microsoft.Health.Fhir.SpecManager.Language.LangOpenApi.html) **EXPERIMENTAL**

    Export an OpenAPI version 2 or 3 JSON document for the selected options.

  * [TypeScript](Microsoft.Health.Fhir.SpecManager.Language.TypeScript.html)

    Basic TypeScript bindings, useful for prototyping and small projects.  Exported classes are able to serialize to and parse from FHIR JSON.

## Library Namespace: [Microsoft.Health.Fhir.SpecManager.Manager](Microsoft.Health.Fhir.SpecManager.Manager.html)

Classes used to load and manage FHIR versions.  Includes functionality to connect to a FHIR server and pull metadata.

## Library Namespace: [Microsoft.Health.Fhir.SpecManager.Models](Microsoft.Health.Fhir.SpecManager.Models.html)

Model classes used in the library.