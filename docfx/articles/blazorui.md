# FhirCodeGenBlazor

The Blazor project is intended as a developer-friendly interface for exploring FHIR packages and exporting 'language' outputs.

The project is a server-hosted Blazor web UI.  To date, it has only been tested as a single-user context - i.e., this is a UI for a local computer, not a hosted server.

## Index Page

The main index page is a listing of the contents of the user's FHIR Cache.  Individual packages (package id plus version) can be loaded directly, or packages can be added or removed.

Clicking 'Add Package' takes a user to the [Add Package Page](#add-package-page).

Clicking 'Load' on a package directive record loads the package into memory and exposes additional page links: [View Package](#view-package-page), [Package Elements](#package-elements-page), and [Export Package](#export-package-page).

## Add Package Page

The 'Add Package Page' is used to find and download FHIR packages.  Packages can be pulled from package registries (`http://packages.fhir.org/` and `http://packages2.fhir.org`), released web-packages of FHIR core (e.g., `http://hl7.org/fhir/R4/hl7.fhir.r4.core.tgz`), or the CI build server (`http://build.fhir.org`, with various branch and IG conventions).

For release packages, the 'Lookup' option requests the package version manifests from the registry servers and allows downloading specific versions.

For CI Builds, the lookup is used to confirm that a package exists at the requested location.

## View Package Page

The 'View Package Page' is used to explore package artifacts.  It is intended to be considered as a view equivalent to browsing the published definitions, though it does not contain additional information (e.g., HTML pages) and is not considered a 'normative' view into packages.

It does contain additional features for traversing nested definitions, expanding into datatypes (e.g., presenting a datatype element the same way a backbone element is displayed), etc..

## Package Elements Page

The 'Package Elements Page' is used to search over all 'FHIR Elements' within a package (can be filtered by artifact type).

The page includes the ability to download JSON representations of the results, either an array of all FHIR Element Paths, or the normalized element definitions.

### LINQ

One of the mechanisms to search over package elements is a LINQ predicate.  The predicate is evaluated on each (normalized) [FhirElement](/fhir-codegen/api/Microsoft.Health.Fhir.CodeGenCommon.Models.FhirElement.html) in the package, and there is global access to the current-context [FhirVersionInfo](/fhir-codegen/api/Microsoft.Health.Fhir.SpecManager.Manager.FhirVersionInfo.html) as `Info`.

Some examples:
* to search for all elements that have a type of `uri`
```c#
e => (e.ElementTypes?.ContainsKey("uri") ?? false)
```

* to search for all elements that have a type of `reference` and do not have search parameters:
```c#
e => ((e.ElementTypes?.ContainsKey("Reference") ?? false) && (!Info.SearchParametersByUrl.Values.Any(sp => sp.Expression.Contains(e.Path))))
```

* to search for elements that have a `required` binding to a value-set that starts with `http://hl7.org/fhir/ValueSet/request`:
```c#
e => ((e.BindingStrength?.Equals("required") ?? false) && (e.ValueSet?.StartsWith("http://hl7.org/fhir/ValueSet/request") ?? false))
```

## Export Package Page

The 'Export Package Page' is used to perform an export based on a loaded package.  The page allows for selection of the Export Language module, common export parameters, and language-specific options.

Note that the export path is local to the computer running the Blazor UI (server).

## Diff Tool Page

The 'Diff Tool Page' is used to granularly compare either two packages or two artifacts.  The packages must be loaded into memory to be available for selection.

The artifact types and changes detected are constantly being improved.  If you have suggestions or requests, please let us know!
