// Minimum TypeScript Version: 3.7
// FHIR Primitive: boolean

import * as fhir from '../fhir.js';

import { IssueTypeCodes } from '../fhirValueSets/IssueTypeCodes.js';
import { IssueSeverityCodes } from '../fhirValueSets/IssueSeverityCodes.js';

/**
 * Value of "true" or "false"
 */
export interface FhirBooleanArgs extends fhir.FhirPrimitiveArgs {
  /**
   * Value of "true" or "false"
   */
  value?:FhirBoolean|boolean|undefined;
}

/**
 * Value of "true" or "false"
 */
export class FhirBoolean extends fhir.FhirPrimitive {
  /**
   * Mapping of this datatype to a FHIR equivalent
   */
  public static readonly _fts_dataType:string = 'Boolean';
  /**
   * Mapping of this datatype to a JSON equivalent
   */
  public static readonly _fts_jsonType:string = 'boolean';
  // published regex: true|false
  public static readonly _fts_regex:RegExp = /^true|false$/
  /**
   * A boolean value, represented as a JS boolean
   */
  declare value?:boolean|null|undefined;
  /**
     * Create a FhirBoolean
     * @param value Value of "true" or "false"
     * @param id Unique id for inter-element referencing (uncommon on primitives)
     * @param extension Additional content defined by implementations
     * @param options Options to pass to extension constructors
  */
  constructor(source:Partial<FhirBooleanArgs> = {}, options:fhir.FhirConstructorOptions = { } ) {
    super(source, options);
  }
  /**
   * Function to perform basic model validation (e.g., check if required elements are present).
   */
  public override doModelValidation():fhir.OperationOutcome {
    var outcome:fhir.OperationOutcome = super.doModelValidation();
    if ((this.value) && (!FhirBoolean._fts_regex.test(this.value.toString()))) {
      outcome.issue!.push(new fhir.OperationOutcomeIssue({ severity: IssueSeverityCodes.Error, code: IssueTypeCodes.InvalidContent,  diagnostics: "Invalid value in primitive type boolean", }));
    }
    return outcome;
  }
  /**
   * Returns the primitive value of the specified object.
   */
  public valueOf():boolean { return (this.value ?? false); }
}
