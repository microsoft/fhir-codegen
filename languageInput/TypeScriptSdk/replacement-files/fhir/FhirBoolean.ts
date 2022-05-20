// Minimum TypeScript Version: 3.7
// FHIR Primitive: boolean

import * as fhir from '../fhir.js';

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
  public static override readonly _fts_dataType:string = 'Boolean';
  /**
   * Mapping of this datatype to a JSON equivalent
   */
  public static override readonly _fts_jsonType:string = 'boolean';
  // published regex: true|false
  public static override readonly _fts_regex:RegExp = /^true|false$/
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
  public override doModelValidation():fhir.FtsIssue[] {
    let issues:fhir.FtsIssue[] = super.doModelValidation();
    if ((this.value) && (!FhirBoolean._fts_regex.test(this.value.toString()))) {
      issues.push({ severity: 'error', code: 'invalid',  diagnostics: 'Invalid value in primitive type boolean', });
    }
    return issues;
  }
  /**
   * Returns the primitive value of the specified object.
   */
  public override valueOf():boolean { return (this.value ?? false); }
}
