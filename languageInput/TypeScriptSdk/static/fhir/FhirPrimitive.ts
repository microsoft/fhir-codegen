// Minimum TypeScript Version: 3.7

import * as fhir from '../fhir.js';

import { IssueSeverityValueSetEnum } from '../fhirValueSets/IssueSeverityValueSet.js';
import { IssueTypeValueSetEnum } from '../fhirValueSets/IssueTypeValueSet.js';

export class FhirPrimitive extends fhir.FhirBase  {
  readonly __dataType:string='PrimitiveType';
  readonly __jsonType:string='any';

  /**
   * Value of the primitive - is constrained by decendant classes.
   */
  value:boolean|number|bigint|string|null;

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
  constructor(value?:any|null, id?:string, extension?:(fhir.Extension|null)[], options:fhir.FhirConstructorOptions = { }) {
    super();
    if ((value) && (value['__dataType'])) {
      this.value = value.value ?? null;
      this.id = value.id ?? undefined;
      if ((value.extension) && (value.extension.length > 0)) {
        this.extension = [];
        value.extension!.forEach((e) => {
          if (e === null) { this.extension!.push(null); }
          else { this.extension!.push(new fhir.Extension(e, options)); }
        });
      }
    } else {
      this.value = value;
    }

    if (id) { this.id = id; }
    if ((extension) && (extension.length > 0)) {
      if (!this.extension) { this.extension = []; }
      extension!.forEach((e) => {
        if (e === null) { this.extension!.push(null); }
        else { this.extension!.push(new fhir.Extension(e, options)); }
      });
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