# Adding new langauges

The system is designed to allow developers to add additional languages to be exported.

Language files must be added to the `src/Microsoft.Health.Fhir.SpecManager/Langauge` directory, and implement the [ILanguage](/api/Microsoft.Health.Fhir.SpecManager.Language.ILanguage.html) interface present in the same directory.

The library loads and parses all FHIR versions into a consistent local model.
|Class|Description|How to Access|
|-----|-----------|-------------|
|FhirVersionInfo|All known information and structures for a specific FHIR release.|Passed to ILange.Export as `info`|

