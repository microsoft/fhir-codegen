// Minimum TypeScript Version: 3.7
// FHIR Primitive: unsignedInt

import * as fhir from '../fhir.js';

import { IssueTypeCodes } from '../fhirValueSets/IssueTypeCodes.js';
import { IssueSeverityCodes } from '../fhirValueSets/IssueSeverityCodes.js';

/**
 * An integer with a value that is not negative (e.g. &gt;= 0)
 */
export interface FhirUnsignedIntArgs extends fhir.FhirIntegerArgs {
  /**
   * An integer with a value that is not negative (e.g. &gt;= 0)
   */
  value?:FhirUnsignedInt|number|undefined;
}

/**
 * An integer with a value that is not negative (e.g. &gt;= 0)
 */
export class FhirUnsignedInt extends fhir.FhirInteger {
  /**
   * Mapping of this datatype to a FHIR equivalent
   */
  public static readonly _fts_dataType:string = 'UnsignedInt';
  /**
   * Mapping of this datatype to a JSON equivalent
   */
  public static readonly _fts_jsonType:string = 'number';
  // published regex: [0]|([1-9][0-9]*)
  public static readonly _fts_regex:RegExp = /^[0]|([1-9][0-9]*)$/
  /**
   * A unsignedInt value, represented as a JS number
   */
  declare value?:number|null|undefined;
  /**
     * Create a FhirUnsignedInt
     * @param value An integer with a value that is not negative (e.g. >= 0)
     * @param id Unique id for inter-element referencing (uncommon on primitives)
     * @param extension Additional content defined by implementations
     * @param options Options to pass to extension constructors
  */
  constructor(source:Partial<FhirUnsignedIntArgs> = {}, options:fhir.FhirConstructorOptions = { } ) {
    super(source, options);
  }
  /**
   * Function to perform basic model validation (e.g., check if required elements are present).
   */
  public override doModelValidation():fhir.OperationOutcome {
    var outcome:fhir.OperationOutcome = super.doModelValidation();
    if ((this.value) && (!FhirUnsignedInt._fts_regex.test(this.value.toString()))) {
      outcome.issue!.push(new fhir.OperationOutcomeIssue({ severity: IssueSeverityCodes.Error, code: IssueTypeCodes.InvalidContent,  diagnostics: "Invalid value in primitive type unsignedInt", }));
    }
    return outcome;
  }
}
