// Minimum TypeScript Version: 3.7
// FHIR Primitive: uri

import * as fhir from '../fhir.js';

import { IssueTypeCodes } from '../fhirValueSets/IssueTypeCodes.js';
import { IssueSeverityCodes } from '../fhirValueSets/IssueSeverityCodes.js';

/**
 * see http://en.wikipedia.org/wiki/Uniform_resource_identifier
 */
export interface FhirUriArgs extends fhir.FhirStringArgs {
  /**
   * see http://en.wikipedia.org/wiki/Uniform_resource_identifier
   */
  value?:FhirUri|string|undefined;
}

/**
 * see http://en.wikipedia.org/wiki/Uniform_resource_identifier
 */
export class FhirUri extends fhir.FhirString {
  /**
   * Mapping of this datatype to a FHIR equivalent
   */
  public static readonly _fts_dataType:string = 'Uri';
  /**
   * Mapping of this datatype to a JSON equivalent
   */
  public static readonly _fts_jsonType:string = 'string';
  // published regex: \S*
  public static readonly _fts_regex:RegExp = /^\S*$/
  /**
   * A uri value, represented as a JS string
   */
  declare value?:string|null|undefined;
  /**
     * Create a FhirUri
     * @param value see http://en.wikipedia.org/wiki/Uniform_resource_identifier
     * @param id Unique id for inter-element referencing (uncommon on primitives)
     * @param extension Additional content defined by implementations
     * @param options Options to pass to extension constructors
  */
  constructor(source:Partial<FhirUriArgs> = {}, options:fhir.FhirConstructorOptions = { } ) {
    super(source, options);
  }
  /**
   * Function to perform basic model validation (e.g., check if required elements are present).
   */
  public override doModelValidation():fhir.OperationOutcome {
    var outcome:fhir.OperationOutcome = super.doModelValidation();
    if ((this.value) && (!FhirUri._fts_regex.test(this.value))) {
      outcome.issue!.push(new fhir.OperationOutcomeIssue({ severity: IssueSeverityCodes.Error, code: IssueTypeCodes.InvalidContent,  diagnostics: "Invalid value in primitive type uri", }));
    }
    return outcome;
  }
}
