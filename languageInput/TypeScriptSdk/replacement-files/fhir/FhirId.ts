// Minimum TypeScript Version: 3.7
// FHIR Primitive: id

import * as fhir from '../fhir.js';

/**
 * RFC 4122
 */
export interface FhirIdArgs extends fhir.FhirPrimitiveArgs {
  /**
   * RFC 4122
   */
  value?:FhirId|string|undefined|null;
}

/**
 * RFC 4122
 */
export class FhirId extends fhir.FhirPrimitive {
  /**
   * Mapping of this datatype to a FHIR equivalent
   */
  public static override readonly _fts_dataType:string = 'Id';
  /**
   * Mapping of this datatype to a JSON equivalent
   */
  public static override readonly _fts_jsonType:string = 'string';
  // published regex: [A-Za-z0-9\-\.]{1,64}
  public static override readonly _fts_regex:RegExp = /^[A-Za-z0-9\-\.]{1,64}$/
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
  public override doModelValidation():fhir.FtsIssue[] {
    let issues:fhir.FtsIssue[] = super.doModelValidation();
    if ((this.value) && ((typeof this.value !== 'string') || (!FhirId._fts_regex.test(this.value)))) {
      issues.push({ severity: 'error', code: 'invalid', diagnostics: 'Invalid value in primitive type id', });
    }
    return issues;
  }
}
