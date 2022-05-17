// Minimum TypeScript Version: 3.7
// FHIR Primitive: id

import * as fhir from '../fhir.js';

import { IssueTypeCodes } from '../fhirValueSets/IssueTypeCodes.js';
import { IssueSeverityCodes } from '../fhirValueSets/IssueSeverityCodes.js';

/**
 * RFC 4122
 */
export interface FhirIdArgs extends fhir.FhirStringArgs {
  /**
   * RFC 4122
   */
  value?:FhirId|string|undefined;
}

/**
 * RFC 4122
 */
export class FhirId extends fhir.FhirString {
  protected static readonly _fts_dataType:string = 'Id';
  protected static readonly _fts_jsonType:string = 'string';
  // published regex: [A-Za-z0-9\-\.]{1,64}
  static readonly _fts_regex:RegExp = /^[A-Za-z0-9\-\.]{1,64}$/
  /**
   * A id value, represented as a JS string
   */
  declare value?:string|null|undefined;
  /**
     * Create a FhirId
     * @param value RFC 4122
     * @param id Unique id for inter-element referencing (uncommon on primitives)
     * @param extension Additional content defined by implementations
     * @param options Options to pass to extension constructors
  */
  constructor(source:Partial<FhirIdArgs> = {}, options:fhir.FhirConstructorOptions = { } ) {
    super(source, options);
  }
  /**
   * Function to perform basic model validation (e.g., check if required elements are present).
   */
  public override doModelValidation():fhir.OperationOutcome {
    var outcome:fhir.OperationOutcome = super.doModelValidation();
    if ((this.value) && (!FhirId._fts_regex.test(this.value))) {
      outcome.issue!.push(new fhir.OperationOutcomeIssue({ severity: IssueSeverityCodes.Error, code: IssueTypeCodes.InvalidContent,  diagnostics: "Invalid value in primitive type id", }));
    }
    return outcome;
  }
}
