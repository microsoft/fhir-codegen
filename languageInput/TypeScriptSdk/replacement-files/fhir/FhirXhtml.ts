// Minimum TypeScript Version: 3.7
// FHIR Primitive: xhtml

import * as fhir from '../fhir.js';

/**
 * XHTML
 */
export interface FhirXhtmlArgs extends fhir.FhirPrimitiveArgs {
  /**
   * XHTML
   */
  value?:FhirXhtml|string|undefined;
}

/**
 * XHTML
 */
export class FhirXhtml extends fhir.FhirPrimitive {
  /**
   * Mapping of this datatype to a FHIR equivalent
   */
  public static override readonly _fts_dataType:string = 'Xhtml';
  /**
   * Mapping of this datatype to a JSON equivalent
   */
  public static override readonly _fts_jsonType:string = 'string';
  /**
   * A xhtml value, represented as a JS string
   */
  declare value?:string|null|undefined;
  /**
     * Create a FhirXhtml
     * @param value XHTML
     * @param id Unique id for inter-element referencing (uncommon on primitives)
     * @param extension Additional content defined by implementations
     * @param options Options to pass to extension constructors
  */
  constructor(source:Partial<FhirXhtmlArgs> = {}, options:fhir.FhirConstructorOptions = { } ) {
    super(source, options);
  }
  /**
   * Function to perform basic model validation (e.g., check if required elements are present).
   */
  public override doModelValidation():fhir.FtsIssue[] {
    let issues:fhir.FtsIssue[] = super.doModelValidation();
    return issues;
  }
}
