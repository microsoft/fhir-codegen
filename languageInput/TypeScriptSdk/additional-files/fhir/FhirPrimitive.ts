// Minimum TypeScript Version: 3.7

import * as fhir from '../fhir.js';

import { IssueSeverityValueSetEnum, IssueTypeValueSetEnum } from '../valueSetEnums.js';

export interface FhirPrimitiveArgs {
  /**
   * Value of the primitive - constrained by decendant classes.
   */
   value?:any|null|undefined;

   /**
     * Unique id for inter-element referencing
     */
   id?:string|undefined;
 
   /**
     * Additional content defined by implementations
     */
   extension?:(fhir.Extension|null)[]|undefined;
 }

export class FhirPrimitive extends fhir.FhirBase  {
  protected readonly _fts_isPrimitive:boolean = true;
  protected readonly _fts_dataType:string='PrimitiveType';
  protected readonly _fts_jsonType:string='any';

  /**
   * Value of the primitive - constrained by decendant classes.
   */
  value?:boolean|number|bigint|string|null|undefined;

  /**
    * Unique id for inter-element referencing
    */
  id?:string|undefined;

  /**
    * Additional content defined by implementations
    */
  extension?:(fhir.Extension|null)[]|undefined;

  /**
   * Constructor for FHIR primitive type elements
   * @param value 
   * @param id 
   * @param extension 
   * @param options 
   */
  constructor(source:Partial<FhirPrimitiveArgs> = {}, options:fhir.FhirConstructorOptions = {}) {
    super();
    if (source) {
      if ((source.value) && (source.value['_fts_dataType'])) {
        this.value = source.value.value ?? null;
        this.id = source.value.id ?? undefined;
        if ((source.value.extension) && (source.value.extension.length > 0)) {
          this.extension = [];
          source.value.extension!.forEach((e:any) => {
            if (e === null) { this.extension!.push(null); }
            else { this.extension!.push(new fhir.Extension(e, options)); }
          });
        }
      } else if (source.value) {
        this.value = source.value;
      }
  
      if (source.id) { this.id = source.id; }
      if ((source.extension) && (source.extension.length > 0)) {
        if (!this.extension) { this.extension = []; }
        source.extension!.forEach((e:any) => {
          if (e === null) { this.extension!.push(null); }
          else { this.extension!.push(new fhir.Extension(e, options)); }
        });
      }
    }
  }

  /**
   * Function to perform basic model validation (e.g., check if required elements are present).
   */
  public doModelValidation():fhir.OperationOutcome {
    var outcome:fhir.OperationOutcome = new fhir.OperationOutcome({issue:[]});
    if ((!this.value) && (!this.id) && ((!this.extension) || (this.extension.length === 0))) {
      outcome.issue!.push(new fhir.OperationOutcomeIssue({
        severity: IssueSeverityValueSetEnum.Error,
        code: IssueTypeValueSetEnum.RequiredElementMissing,
        diagnostics: "Primitive values must have a value, id, or extensions.",
      }));
    }
    return outcome;
  }

  /**
   * Add an extension with the desired URL and FHIR value
   * @param url 
   * @param value 
   */
  addExtension(url:string, value:fhir.FhirBase) {
    if (this.extension === undefined) {
      this.extension = [];
    }

    this.extension.push(fhir.Extension.fromValue(url, value));
  }
}