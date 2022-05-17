// Minimum TypeScript Version: 3.7
// FHIR Primitive: canonical

import * as fhir from '../fhir.js';

import { IssueTypeCodes } from '../fhirValueSets/IssueTypeCodes.js';
import { IssueSeverityCodes } from '../fhirValueSets/IssueSeverityCodes.js';

/**
 * see [Canonical References](references.html#canonical)
 */
export interface FhirCanonicalArgs extends fhir.FhirStringArgs {
  /**
   * see [Canonical References](references.html#canonical)
   */
  value?:FhirCanonical|string|undefined;
}

/**
 * see [Canonical References](references.html#canonical)
 */
export class FhirCanonical extends fhir.FhirString {
  protected static readonly _fts_dataType:string = 'Canonical';
  protected static readonly _fts_jsonType:string = 'string';
  // published regex: \S*
  static readonly _fts_regex:RegExp = /^\S*$/
  /**
   * A canonical value, represented as a JS string
   */
  declare value?:string|null|undefined;
  /**
     * Create a FhirCanonical
     * @param value see [Canonical References](references.html#canonical)
     * @param id Unique id for inter-element referencing (uncommon on primitives)
     * @param extension Additional content defined by implementations
     * @param options Options to pass to extension constructors
  */
  constructor(source:Partial<FhirCanonicalArgs> = {}, options:fhir.FhirConstructorOptions = { } ) {
    super(source, options);
  }
  /**
   * Function to perform basic model validation (e.g., check if required elements are present).
   */
  public override doModelValidation():fhir.OperationOutcome {
    var outcome:fhir.OperationOutcome = super.doModelValidation();
    if ((this.value) && (!FhirCanonical._fts_regex.test(this.value))) {
      outcome.issue!.push(new fhir.OperationOutcomeIssue({ severity: IssueSeverityCodes.Error, code: IssueTypeCodes.InvalidContent,  diagnostics: "Invalid value in primitive type canonical", }));
    }
    return outcome;
  }
}
