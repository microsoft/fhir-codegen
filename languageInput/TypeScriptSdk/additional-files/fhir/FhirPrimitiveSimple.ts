// Minimum TypeScript Version: 3.7

import * as fhir from '../fhir.js';

export interface FhirPrimitiveSimpleArgs {
  /**
   * Value of the primitive - constrained by decendant classes.
   */
   value?:any|null|undefined;
 }

export class FhirPrimitiveSimple extends fhir.FhirBase  {
  public static readonly _fts_isPrimitive:boolean = true;
  /**
   * Mapping of this datatype to a FHIR equivalent
   */
  public static override readonly _fts_dataType:string='PrimitiveType';
  public static readonly _fts_jsonType:string='any';

  /**
   * Value of the primitive - constrained by decendant classes.
   */
  value?:boolean|number|bigint|string|null|undefined;

  /**
   * Constructor for FHIR primitive type elements that do not allow extended properties
   * @param source 
   * @param options 
   */
  constructor(source:Partial<FhirPrimitiveSimpleArgs> = {}, options:fhir.FhirConstructorOptions = {}) {
    super({}, options);
    this.value = source?.value ?? undefined;
  }

  /**
   * Function to perform basic model validation (e.g., check if required elements are present).
   */
  public override doModelValidation(exp:string = ''):fhir.FtsIssue[] {
    let iss:fhir.FtsIssue[] = super.doModelValidation(exp);
    if (this.value === undefined) {
      iss.push({
        severity: 'error',
        code: 'required',
        details: { text: 'Simple Primitive values must have a value.' },
        expression: [exp],
      });
    }
    return iss;
  }
}