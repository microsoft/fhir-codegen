# Adding new langauges

The system is designed to allow developers to add additional languages to be exported.

Language files must be added to the `src/Microsoft.Health.Fhir.SpecManager/Langauge` directory, and implement the [ILanguage](/fhir-codegen/api/Microsoft.Health.Fhir.SpecManager.Language.ILanguage.html) interface present in the same directory.

The library loads and parses all FHIR versions into a consistent local model.

During export, the interface function `Export` is called, with a `FhirVersionInfo` already set to all user options (e.g., the requested version of FHIR has been loaded, filtering has been applied, etc.).

The `Export` function may write as many files as desired in the `exportDirectory`.  The exporter will move those files into the user-requested directory, and will bundle them together in an archive if necessary (relative paths are preserved).

The [Info](/fhir-codegen/api/Microsoft.Health.Fhir.SpecManager.Language.Info.html), [CSharpBasic](/fhir-codegen/api/Microsoft.Health.Fhir.SpecManager.Language.CSharpBasic.html), and [TypeScript](/fhir-codegen/api/Microsoft.Health.Fhir.SpecManager.Language.TypeScript.html) languages are all available as references on how to traverse the various structures.
