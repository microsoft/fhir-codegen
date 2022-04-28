/**
 * Function to remove invalid element values from serialization
 * @param obj 
 * @returns 
 */
function fhirToJson(obj:any) {
    let c:any = {...obj};
  
    for (const key in c) {
      if ((c[key] === undefined) || 
          (c[key] === null) ||
          (c[key] === '') ||
          (c[key] === NaN) ||
          (Array.isArray(c[key]) && (c[key].length === 0)) ||
          ((typeof c[key] === 'object') && (Object.keys(c[key]).length === 0))) {
        delete c[key];
      }
    }
  
    return c;
  }
  