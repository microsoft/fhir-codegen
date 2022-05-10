/**
 * Function to remove invalid element values from serialization
 * @param obj 
 * @returns 
 */
 function fhirToJson(obj:any) {
  let c:any = {};

  for (const key in obj) {
    if ((obj[key] === undefined) || 
        (obj[key] === null) ||
        (obj[key] === '') ||
        (obj[key] === NaN)) {
      continue;
    }

    if (key.startsWith('__')) {
      continue;
    }

    if (Array.isArray(obj[key])) {
      if (obj[key].length === 0) {
        continue;
      }

      if (obj[key][0]['__isPrimitive']) {
        const eName:string = '_' + key;
        let foundAnyVal:boolean = false;
        let foundAnyExt:boolean = false;
        c[key] = [];
        c[eName] = [];
        obj[key].forEach((av:any) => {
          let addElement:boolean = false;
          if ((av['value'] !== undefined) && (av['value'] !== null)) { 
            c[key].push(av.valueOf()); 
            foundAnyVal = true;
            addElement = true;
          } else { 
            c[key].push(null);
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
            c[key].pop();
            c[eName].pop();
          }
        });

        if (!foundAnyVal) { delete c[key]; }
        if (!foundAnyExt) { delete c[eName]; }
      } else if (obj[key][0]['__dataType']) {
        c[key] = [];
        obj[key].forEach((v:any) => {
          c[key].push(fhirToJson(v));
        });
      } else {
        c[key] = obj[key];
      }
    } else {
      if (obj[key]['__isPrimitive']) {
        if (obj[key]['value']) { c[key] = obj[key].valueOf(); }

        const eName:string = '_' + key;
        c[eName] = {};
        if (obj[key]['id']) { c[eName]['id'] = obj[key]['id']; }
        if (obj[key]['extension']) {
          c[eName]['extension'] = [];
          obj[key]['extension'].forEach((e:any) => {
            c[eName]['extension'].push(fhirToJson(e));
          });
        }

        if (Object.keys(c[eName]).length === 0) { delete c[eName]; }
      } else if (obj[key]['__dataType']) {
        c[key] = fhirToJson(obj[key]);
      } else {
        c[key] = obj[key];
      }
    }
  }

  return c;
}
  