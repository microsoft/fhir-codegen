// Minimum TypeScript Version: 3.7
// FHIR Primitive: decimal

import * as fhir from '../fhir.js';

/**
 * Do not use an IEEE type floating point type, instead use something that works like a true decimal, with inbuilt precision (e.g. Java BigInteger)
 */
export interface FhirDecimalArgs extends fhir.FhirPrimitiveArgs {
  /**
   * Do not use an IEEE type floating point type, instead use something that works like a true decimal, with inbuilt precision (e.g. Java BigInteger)
   */
  value?:FhirDecimal|number|undefined;
}

/**
 * Do not use an IEEE type floating point type, instead use something that works like a true decimal, with inbuilt precision (e.g. Java BigInteger)
 */
export class FhirDecimal extends fhir.FhirPrimitive {
  /**
   * Mapping of this datatype to a FHIR equivalent
   */
  public static override readonly _fts_dataType:string = 'Decimal';
  /**
   * Mapping of this datatype to a JSON equivalent
   */
  public static override readonly _fts_jsonType:string = 'number';
  // published regex: -?(0|[1-9][0-9]*)(\.[0-9]+)?([eE][+-]?[0-9]+)?
  public static override readonly _fts_regex:RegExp = /^-?(0|[1-9][0-9]*)(\.[0-9]+)?([eE][+-]?[0-9]+)?$/
  /**
   * A decimal value, represented as a JS number
   */
  declare value?:number|null|undefined;
  /**
     * Create a FhirDecimal
     * @param value Do not use an IEEE type floating point type, instead use something that works like a true decimal, with inbuilt precision (e.g. Java BigInteger)
     * @param id Unique id for inter-element referencing (uncommon on primitives)
     * @param extension Additional content defined by implementations
     * @param options Options to pass to extension constructors
  */
  constructor(source:Partial<FhirDecimalArgs> = {}, options:fhir.FhirConstructorOptions = { } ) {
    super(source, options);
  }
  /**
   * Function to perform basic model validation (e.g., check if required elements are present).
   */
  public override doModelValidation():fhir.FtsIssue[] {
    let issues:fhir.FtsIssue[] = super.doModelValidation();
    if ((this.value) && (!FhirDecimal._fts_regex.test(this.value.toString()))) {
      issues.push({ severity: 'error', code: 'invalid',  diagnostics: 'Invalid value in primitive type decimal', });
    }
    return issues;
  }
}
