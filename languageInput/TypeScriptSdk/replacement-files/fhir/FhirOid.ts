// Minimum TypeScript Version: 3.7
// FHIR Primitive: oid

import * as fhir from '../fhir.js';

/**
 * RFC 3001. See also ISO/IEC 8824:1990 €
 */
export interface FhirOidArgs extends fhir.FhirPrimitiveArgs {
  /**
   * RFC 3001. See also ISO/IEC 8824:1990 €
   */
  value?:FhirOid|string|undefined|null;
}

/**
 * RFC 3001. See also ISO/IEC 8824:1990 €
 */
export class FhirOid extends fhir.FhirPrimitive {
  /**
   * Mapping of this datatype to a FHIR equivalent
   */
  public static override readonly _fts_dataType:string = 'Oid';
  /**
   * Mapping of this datatype to a FHIR equivalent
   */
  public static override readonly _fts_jsonType:string = 'string';
  // published regex: urn:oid:[0-2](\.(0|[1-9][0-9]*))+
  public static override readonly _fts_regex:RegExp = /^urn:oid:[0-2](\.(0|[1-9][0-9]*))+$/
  /**
   * A oid value, represented as a JS string
   */
  declare value?:string|null|undefined;
  /**
     * Create a FhirOid
     * @param value RFC 3001. See also ISO/IEC 8824:1990 €
     * @param id Unique id for inter-element referencing (uncommon on primitives)
     * @param extension Additional content defined by implementations
     * @param options Options to pass to extension constructors
  */
  constructor(source:Partial<FhirOidArgs> = {}, options:fhir.FhirConstructorOptions = { } ) {
    super(source, options);
  }
  /**
   * Function to perform basic model validation (e.g., check if required elements are present).
   */
  public override doModelValidation():fhir.FtsIssue[] {
    let issues:fhir.FtsIssue[] = super.doModelValidation();
    if ((this.value) && (!FhirOid._fts_regex.test(this.value))) {
      issues.push({ severity: 'error', code: 'invalid', diagnostics: 'Invalid value in primitive type oid', });
    }
    return issues;
  }
}
