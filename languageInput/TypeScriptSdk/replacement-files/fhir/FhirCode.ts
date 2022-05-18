// Minimum TypeScript Version: 3.7
// FHIR Primitive: code

import * as fhir from '../fhir.js';

import { IssueTypeCodes } from '../fhirValueSets/IssueTypeCodes.js';
import { IssueSeverityCodes } from '../fhirValueSets/IssueSeverityCodes.js';

/**
 * A string which has at least one character and no leading or trailing whitespace and where there is no whitespace other than single spaces in the contents
 */
export interface FhirCodeArgs extends fhir.FhirStringArgs {
  /**
   * A string which has at least one character and no leading or trailing whitespace and where there is no whitespace other than single spaces in the contents
   */
  value?:FhirCode|string|undefined;
}

/**
 * A string which has at least one character and no leading or trailing whitespace and where there is no whitespace other than single spaces in the contents
 */
export class FhirCode extends fhir.FhirString {
  /**
   * Mapping of this datatype to a FHIR equivalent
   */
  public static readonly _fts_dataType:string = 'Code';
  /**
   * Mapping of this datatype to a JSON equivalent
   */
  public static readonly _fts_jsonType:string = 'string';
  // published regex: [^\s]+(\s[^\s]+)*
  public static readonly _fts_regex:RegExp = /^[^\s]+(\s[^\s]+)*$/
  /**
   * A code value, represented as a JS string
   */
  declare value?:string|null|undefined;
  /**
     * Create a FhirCode
     * @param value A string which has at least one character and no leading or trailing whitespace and where there is no whitespace other than single spaces in the contents
     * @param id Unique id for inter-element referencing (uncommon on primitives)
     * @param extension Additional content defined by implementations
     * @param options Options to pass to extension constructors
  */
  constructor(source:Partial<FhirCodeArgs> = {}, options:fhir.FhirConstructorOptions = { } ) {
    super(source, options);
  }
  /**
   * Function to perform basic model validation (e.g., check if required elements are present).
   */
  public override doModelValidation():fhir.OperationOutcome {
    var outcome:fhir.OperationOutcome = super.doModelValidation();
    if ((this.value) && (!FhirCode._fts_regex.test(this.value))) {
      outcome.issue!.push(new fhir.OperationOutcomeIssue({ severity: IssueSeverityCodes.Error, code: IssueTypeCodes.InvalidContent,  diagnostics: "Invalid value in primitive type code", }));
    }
    return outcome;
  }
}
