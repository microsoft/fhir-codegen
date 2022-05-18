// Minimum TypeScript Version: 3.7

import { OperationOutcome } from './OperationOutcome.js';

export interface FhirConstructorOptions {
  /** If instantiated objects should allow non-FHIR defined properties */
  allowUnknownElements?: boolean|undefined;
}

export interface FhirBaseArgs { }

export class FhirBase {
  /**
   * Mapping of this datatype to a FHIR equivalent
   */
  public readonly _fts_dataType:string='Base';

  /** Default constructor */
  constructor(source:Partial<FhirBaseArgs> = {}, options:FhirConstructorOptions = {}) {
    if (options.allowUnknownElements === true) { Object.assign(this, source); }
  }

  /**
   * Function to perform basic model validation (e.g., check if required elements are present).
   */
   public doModelValidation():OperationOutcome {
    var outcome:OperationOutcome = new OperationOutcome({issue:[]});
    return outcome;
  }

  /**
   * Convert a class-structured model into JSON
   * @returns JSON-compatible version of this object
   */
  public toJSON() {
    let c:any = {};
  
    for (const key in this) {
      if (((this as any)[key] === undefined) || 
          ((this as any)[key] === null) ||
          ((this as any)[key] === '') ||
          ((this as any)[key] === NaN)) {
        continue;
      }
  
      if (key.startsWith('_fts_')) {
        continue;
      }
  
      let dKey:string = key + ((this as any)['_fts_' + key + 'IsChoice'] ? ((this as any)[key]['_fts_dataType'] ?? '') : '');
  
      if (Array.isArray(this[key])) {
        if ((this as any)[key].length === 0) {
          continue;
        }
  
        if ((this as any)[key][0]['_fts_isPrimitive']) {
          const eName:string = '_' + dKey;
          let foundAnyVal:boolean = false;
          let foundAnyExt:boolean = false;
          c[dKey] = [];
          c[eName] = [];
          (this as any)[key].forEach((av:any) => {
            let addElement:boolean = false;
            if ((av['value'] !== undefined) && (av['value'] !== null)) { 
              c[dKey].push(av.valueOf()); 
              foundAnyVal = true;
              addElement = true;
            } else { 
              c[dKey].push(null);
            }
  
            let ao:object = {};
            if (av.id) { (ao as any)['id'] = av.id; }
            if (av.extension) {
              (ao as any)['extension'] = [];
              av.extension.forEach((e:any) => {
                (ao as any)['extension'].push(e.toJSON());
              });
            }
  
            if (Object.keys(ao).length !== 0) { 
              c[eName].push(ao);
              foundAnyExt = true;
              addElement = true;
            } else {
              c[eName].push(null);
            }
  
            if (!addElement) {
              c[dKey].pop();
              c[eName].pop();
            }
          });
  
          if (!foundAnyVal) { delete c[dKey]; }
          if (!foundAnyExt) { delete c[eName]; }
        } else if ((this as any)[key][0]['_fts_dataType']) {
          c[dKey] = [];
          (this as any)[key].forEach((v:any) => {
            c[dKey].push(v.toJSON());
          });
        } else {
          c[dKey] = this[key];
        }
      } else {
        if ((this as any)[key]['_fts_isPrimitive']) {
          if ((this as any)[key]['value']) { c[dKey] = (this as any)[key].valueOf(); }
  
          const eName:string = '_' + dKey;
          c[eName] = {};
          if ((this as any)[key]['id']) { c[eName]['id'] = (this as any)[key]['id']; }
          if ((this as any)[key]['extension']) {
            c[eName]['extension'] = [];
            (this as any)[key]['extension'].forEach((e:any) => {
              c[eName]['extension'].push(e.toJSON());
            });
          }
  
          if (Object.keys(c[eName]).length === 0) { delete c[eName]; }
        } else if ((this as any)[key]['_fts_dataType']) {
          c[dKey] = (this as any)[key].toJSON();
        } else {
          c[dKey] = (this as any)[key];
        }
      }
    }
  
    return c;
  }
}