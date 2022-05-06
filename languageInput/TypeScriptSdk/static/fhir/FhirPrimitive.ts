// Minimum TypeScript Version: 3.7

import * as fhir from '../fhir.js';

export type IFhirPrimitive = fhir.FhirBase & {
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
}

export class FhirPrimitive extends fhir.FhirBase implements IFhirPrimitive {
  // readonly typeName:string = 'FhirPrimitive';

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
    this.value = value;
    if (id) { this.id = id; }
    if ((extension) && (extension.length > 0)) {
      this.extension = [];
      extension!.forEach((e) => {
        if (e === null) { this.extension!.push(null); }
        else { this.extension!.push(new fhir.Extension(e, options)); }
      });
    }
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