// Minimum TypeScript Version: 3.7
// FHIR Primitive: url

import * as fhir from '../fhir.js';

/**
 * A URI that is a literal reference
 */
export interface FhirUrlArgs extends fhir.FhirPrimitiveArgs {
  /**
   * A URI that is a literal reference
   */
  value?:FhirUrl|string|undefined;
}

/**
 * A URI that is a literal reference
 */
export class FhirUrl extends fhir.FhirPrimitive {
  /**
   * Mapping of this datatype to a FHIR equivalent
   */
  public static override readonly _fts_dataType:string = 'Url';
  /**
   * Mapping of this datatype to a JSON equivalent
   */
  public static override readonly _fts_jsonType:string = 'string';
  // published regex: \S*
  public static override readonly _fts_regex:RegExp = /^\S*$/
  /**
   * A url value, represented as a JS string
   */
  declare value?:string|null|undefined;
  /**
     * Create a FhirUrl
     * @param value A URI that is a literal reference
     * @param id Unique id for inter-element referencing (uncommon on primitives)
     * @param extension Additional content defined by implementations
     * @param options Options to pass to extension constructors
  */
  constructor(source:Partial<FhirUrlArgs> = {}, options:fhir.FhirConstructorOptions = { } ) {
    super(source, options);
  }
  /**
   * Function to perform basic model validation (e.g., check if required elements are present).
   */
  public override doModelValidation():fhir.FtsIssue[] {
    let issues:fhir.FtsIssue[] = super.doModelValidation();
    if ((this.value) && (!FhirUrl._fts_regex.test(this.value))) {
      issues.push({ severity: 'error', code: 'invalid', diagnostics: 'Invalid value in primitive type url', });
    }
    return issues;
  }
}
