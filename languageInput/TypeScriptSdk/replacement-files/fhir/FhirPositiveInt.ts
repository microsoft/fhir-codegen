// Minimum TypeScript Version: 3.7
// FHIR Primitive: positiveInt

import * as fhir from '../fhir.js';

/**
 * An integer with a value that is positive (e.g. &gt;0)
 */
export interface FhirPositiveIntArgs extends fhir.FhirPrimitiveArgs {
  /**
   * An integer with a value that is positive (e.g. &gt;0)
   */
  value?:FhirPositiveInt|number|undefined|null;
}

/**
 * An integer with a value that is positive (e.g. &gt;0)
 */
export class FhirPositiveInt extends fhir.FhirPrimitive {
  /**
   * Mapping of this datatype to a FHIR equivalent
   */
  public static override readonly _fts_dataType:string = 'PositiveInt';
  /**
   * Mapping of this datatype to a JSON equivalent
   */
  public static override readonly _fts_jsonType:string = 'number';
  // published regex: [1-9][0-9]*
  public static override readonly _fts_regex:RegExp = /^[1-9][0-9]*$/
  /**
   * A positiveInt value, represented as a JS number
   */
  declare value?:number|null|undefined;
  /**
     * Create a FhirPositiveInt
     * @param value An integer with a value that is positive (e.g. >0)
     * @param id Unique id for inter-element referencing (uncommon on primitives)
     * @param extension Additional content defined by implementations
     * @param options Options to pass to extension constructors
  */
  constructor(source:Partial<FhirPositiveIntArgs> = {}, options:fhir.FhirConstructorOptions = { } ) {
    super(source, options);
  }
  /**
   * Function to perform basic model validation (e.g., check if required elements are present).
   */
  public override doModelValidation(expression:string = ''):fhir.FtsIssue[] {
    let issues:fhir.FtsIssue[] = super.doModelValidation(expression);
    if ((this.value !== undefined) && (this.value !== null) && ((typeof this.value !== 'number') || (!Number.isInteger(this.value)) || (this.value <= 0) || (!FhirPositiveInt._fts_regex.test(this.value.toString())))) {
      issues.push({ severity: 'error', code: 'invalid', diagnostics: 'Invalid value in primitive type positiveInt', expression: [expression]});
    }
    return issues;
  }
  /**
   * Returns a string representation of an object.
   * @param radix Specifies a radix for converting numeric values to strings. This value is only used for numbers.
   */
   public override toString(radix?:number):string { return (this.value ?? NaN).toString(radix); }
   /**
    * Returns a string representing a number in fixed-point notation.
    * @param fractionDigits Number of digits after the decimal point. Must be in the range 0 - 20, inclusive.
    */
   public toFixed(fractionDigits?:number):string { return (this.value ?? NaN).toFixed(fractionDigits); }
   /**
    * Returns a string containing a number represented in exponential notation.
    * @param fractionDigits Number of digits after the decimal point. Must be in the range 0 - 20, inclusive.
    */
   public toExponential(fractionDigits?:number):string { return (this.value ?? NaN).toExponential(fractionDigits); }
   /**
    * Returns a string containing a number represented either in exponential or fixed-point notation with a specified number of digits.
    * @param precision Number of significant digits. Must be in the range 1 - 21, inclusive.
    */
   public toPrecision(precision?:number):string { return (this.value ?? NaN).toPrecision(precision); }
   /**
    * Returns the primitive value of the specified object.
    */
   public override valueOf():number { return (this.value ?? NaN); }
}
