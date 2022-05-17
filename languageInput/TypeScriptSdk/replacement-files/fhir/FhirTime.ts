// Minimum TypeScript Version: 3.7
// FHIR Primitive: time

import * as fhir from '../fhir.js';

import { IssueTypeCodes } from '../fhirValueSets/IssueTypeCodes.js';
import { IssueSeverityCodes } from '../fhirValueSets/IssueSeverityCodes.js';

/**
 * A time during the day, with no date specified
 */
export interface FhirTimeArgs extends fhir.FhirStringArgs {
  /**
   * A time during the day, with no date specified
   */
  value?:FhirTime|string|undefined;
}

/**
 * A time during the day, with no date specified
 */
export class FhirTime extends fhir.FhirString {
  protected static readonly _fts_dataType:string = 'Time';
  protected static readonly _fts_jsonType:string = 'string';
  // published regex: ([01][0-9]|2[0-3]):[0-5][0-9]:([0-5][0-9]|60)(\.[0-9]+)?
  static readonly _fts_regex:RegExp = /^([01][0-9]|2[0-3]):[0-5][0-9]:([0-5][0-9]|60)(\.[0-9]+)?$/
  /**
   * A time value, represented as a JS string
   */
  declare value?:string|null|undefined;
  /**
     * Create a FhirTime
     * @param value A time during the day, with no date specified
     * @param id Unique id for inter-element referencing (uncommon on primitives)
     * @param extension Additional content defined by implementations
     * @param options Options to pass to extension constructors
  */
  constructor(source:Partial<FhirTimeArgs> = {}, options:fhir.FhirConstructorOptions = { } ) {
    super(source, options);
  }
  /**
   * Function to perform basic model validation (e.g., check if required elements are present).
   */
  public override doModelValidation():fhir.OperationOutcome {
    var outcome:fhir.OperationOutcome = super.doModelValidation();
    if ((this.value) && (!FhirTime._fts_regex.test(this.value))) {
      outcome.issue!.push(new fhir.OperationOutcomeIssue({ severity: IssueSeverityCodes.Error, code: IssueTypeCodes.InvalidContent,  diagnostics: "Invalid value in primitive type time", }));
    }
    return outcome;
  }
}
