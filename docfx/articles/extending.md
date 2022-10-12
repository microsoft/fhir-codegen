# Adding New Languages

The system is designed to allow developers to add additional languages to be exported.

Language files must be added to the `src/Microsoft.Health.Fhir.SpecManager/Language` directory, and implement the [ILanguage](/fhir-codegen/api/Microsoft.Health.Fhir.SpecManager.Language.ILanguage.html) interface present in the same directory.

The library loads and parses all FHIR versions into a consistent local model.

During export, the interface function [Export](/fhir-codegen/api/Microsoft.Health.Fhir.SpecManager.Language.ILanguage.html#methods) is called, with a [IPackageExportable](/fhir-codegen/api/Microsoft.Health.Fhir.SpecManager.Manager.IPackageExportable.html) already set to all user options (e.g., the requested version of FHIR has been loaded, filtering has been applied, etc.).

The `Export` function may write as many files as desired in the `exportDirectory`.  The exporter will move those files into the user-requested directory, and will bundle them together in an archive if necessary (relative paths are preserved).

Existing language implementations can be used to serve as templates for export.  Specifically:
* [Info](/fhir-codegen/api/Microsoft.Health.Fhir.SpecManager.Language.Info.html) is a good 'single-file' export language that covers many types of artifacts.
* [TypeScriptSdk](/fhir-codegen/api/Microsoft.Health.Fhir.SpecManager.Language.TypeScriptSdk.html) is a good example of an 'advanced' configuration that performs a conversion from the generic models into language-specific objects before exporting.

More information about all of the implemented language modules can be found on the [Export Languages Page](languages.md).