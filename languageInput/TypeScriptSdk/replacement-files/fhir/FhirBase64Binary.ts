// Minimum TypeScript Version: 3.7
// FHIR Primitive: base64Binary

import * as fhir from '../fhir.js';

import { IssueTypeCodes } from '../fhirValueSets/IssueTypeCodes.js';
import { IssueSeverityCodes } from '../fhirValueSets/IssueSeverityCodes.js';

/**
 * A stream of bytes, base64 encoded
 */
export interface FhirBase64BinaryArgs extends fhir.FhirStringArgs {
  /**
   * A stream of bytes, base64 encoded
   */
  value?:FhirBase64Binary|string|undefined;
}

/**
 * A stream of bytes, base64 encoded
 */
export class FhirBase64Binary extends fhir.FhirString {
  /**
   * Mapping of this datatype to a FHIR equivalent
   */
  public static readonly _fts_dataType:string = 'Base64Binary';
  /**
   * Mapping of this datatype to a JSON equivalent
   */
  public static readonly _fts_jsonType:string = 'string';
  // published regex: (\s*([0-9a-zA-Z\+/=]){4}\s*)+
  public static readonly _fts_regex:RegExp = /^(\s*([0-9a-zA-Z\+/=]){4}\s*)+$/
  /**
   * A base64Binary value, represented as a JS string
   */
  declare value?:string|null|undefined;
  /**
     * Create a FhirBase64Binary
     * @param value A stream of bytes, base64 encoded
     * @param id Unique id for inter-element referencing (uncommon on primitives)
     * @param extension Additional content defined by implementations
     * @param options Options to pass to extension constructors
  */
  constructor(source:Partial<FhirBase64BinaryArgs> = {}, options:fhir.FhirConstructorOptions = { } ) {
    super(source, options);
  }
  /**
   * Function to perform basic model validation (e.g., check if required elements are present).
   */
  public override doModelValidation():fhir.OperationOutcome {
    var outcome:fhir.OperationOutcome = super.doModelValidation();
    if ((this.value) && (!FhirBase64Binary._fts_regex.test(this.value))) {
      outcome.issue!.push(new fhir.OperationOutcomeIssue({ severity: IssueSeverityCodes.Error, code: IssueTypeCodes.InvalidContent,  diagnostics: "Invalid value in primitive type base64Binary", }));
    }
    return outcome;
  }
}
