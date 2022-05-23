// Minimum TypeScript Version: 3.7
// FHIR Primitive: base64Binary

import * as fhir from '../fhir.js';

/**
 * A stream of bytes, base64 encoded
 */
export interface FhirBase64BinaryArgs extends fhir.FhirPrimitiveArgs {
  /**
   * A stream of bytes, base64 encoded
   */
  value?:FhirBase64Binary|string|undefined|null;
}

/**
 * A stream of bytes, base64 encoded
 */
export class FhirBase64Binary extends fhir.FhirPrimitive {
  /**
   * Mapping of this datatype to a FHIR equivalent
   */
  public static override readonly _fts_dataType:string = 'Base64Binary';
  /**
   * Mapping of this datatype to a JSON equivalent
   */
  public static override readonly _fts_jsonType:string = 'string';
  // published regex: (\s*([0-9a-zA-Z\+/=]){4}\s*)+
  public static override readonly _fts_regex:RegExp = /^(\s*([0-9a-zA-Z\+/=]){4}\s*)+$/
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
  public override doModelValidation():fhir.FtsIssue[] {
    let issues:fhir.FtsIssue[] = super.doModelValidation();
    if ((this.value) && (!FhirBase64Binary._fts_regex.test(this.value))) {
      issues.push({ severity: 'error', code: 'invalid',  diagnostics: 'Invalid value in primitive type base64Binary', });
    }
    return issues;
  }
}
