// Minimum TypeScript Version: 3.7

export interface FhirBaseArgs { }

export class FhirBase {
  protected readonly __dataType:string='Base';
  constructor() { }

  /**
   * Convert a class-structured model into JSON
   * @returns JSON-compatible version of this object
   */
  public toJSON() {
    let c:any = {};
  
    for (const key in this) {
      if ((this[key] === undefined) || 
          (this[key] === null) ||
          (this[key] === '') ||
          (this[key] === NaN)) {
        continue;
      }
  
      if (key.startsWith('__')) {
        continue;
      }
  
      let dKey:string = key + (this['__' + key + 'IsChoice'] ? (this[key]['__dataType'] ?? '') : '');
  
      if (Array.isArray(this[key])) {
        if (this[key].length === 0) {
          continue;
        }
  
        if (this[key][0]['__isPrimitive']) {
          const eName:string = '_' + dKey;
          let foundAnyVal:boolean = false;
          let foundAnyExt:boolean = false;
          c[dKey] = [];
          c[eName] = [];
          this[key].forEach((av:any) => {
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
                (ao as any)['extension'].push(fhirToJson(e));
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
        } else if (this[key][0]['__dataType']) {
          c[dKey] = [];
          this[key].forEach((v:any) => {
            c[dKey].push(fhirToJson(v));
          });
        } else {
          c[dKey] = this[key];
        }
      } else {
        if (this[key]['__isPrimitive']) {
          if (this[key]['value']) { c[dKey] = this[key].valueOf(); }
  
          const eName:string = '_' + dKey;
          c[eName] = {};
          if (this[key]['id']) { c[eName]['id'] = this[key]['id']; }
          if (this[key]['extension']) {
            c[eName]['extension'] = [];
            this[key]['extension'].forEach((e:any) => {
              c[eName]['extension'].push(fhirToJson(e));
            });
          }
  
          if (Object.keys(c[eName]).length === 0) { delete c[eName]; }
        } else if (this[key]['__dataType']) {
          c[dKey] = fhirToJson(this[key]);
        } else {
          c[dKey] = this[key];
        }
      }
    }
  
    return c;
  }
}