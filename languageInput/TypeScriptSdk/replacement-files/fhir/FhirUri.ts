// Minimum TypeScript Version: 3.7
// FHIR Primitive: uri

import * as fhir from '../fhir.js';

/**
 * see http://en.wikipedia.org/wiki/Uniform_resource_identifier
 */
export interface FhirUriArgs extends fhir.FhirPrimitiveArgs {
  /**
   * see http://en.wikipedia.org/wiki/Uniform_resource_identifier
   */
  value?:FhirUri|string|undefined|null;
}

/**
 * see http://en.wikipedia.org/wiki/Uniform_resource_identifier
 */
export class FhirUri extends fhir.FhirPrimitive {
  /**
   * Mapping of this datatype to a FHIR equivalent
   */
  public static override readonly _fts_dataType:string = 'Uri';
  /**
   * Mapping of this datatype to a JSON equivalent
   */
  public static override readonly _fts_jsonType:string = 'string';
  // published regex: \S*
  public static override readonly _fts_regex:RegExp = /^\S*$/
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
  public override doModelValidation():fhir.FtsIssue[] {
    let issues:fhir.FtsIssue[] = super.doModelValidation();
    if ((this.value) && ((typeof this.value !== 'string') || (!FhirUri._fts_regex.test(this.value)))) {
      issues.push({ severity: 'error', code: 'invalid', diagnostics: 'Invalid value in primitive type uri', });
    }
    return issues;
  }
}
