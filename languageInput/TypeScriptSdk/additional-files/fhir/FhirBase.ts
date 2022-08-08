// Minimum TypeScript Version: 3.7

export interface FhirConstructorOptions {
  /** If instantiated objects should allow non-FHIR defined properties */
  allowUnknownElements?: boolean|undefined;
}

/**
 * Internal Element - equivalent to a FHIR JSON Element (Complex DataType), without extensions
 */
export interface FtsElement {
  /**
   * Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
   */
   id?: string|undefined;
}

/**
 * Internal coding - equivalent to a FHIR JSON Coding (Complex DataType), without extensions
 */
export interface FtsCoding extends FtsElement {
  /**
   * The URI may be an OID (urn:oid:...) or a UUID (urn:uuid:...).  OIDs and UUIDs SHALL be references to the HL7 OID registry. Otherwise, the URI should come from HL7's list of FHIR defined special URIs or it should reference to some definition that establishes the system clearly and unambiguously.
   */
   system?: string|undefined;
   /**
    * Where the terminology does not clearly define what string should be used to identify code system versions, the recommendation is to use the date (expressed in FHIR date format) on which that version was officially published as the version date.
    */
   version?: string|undefined;
   /**
    * A symbol in syntax defined by the system. The symbol may be a predefined code or an expression in a syntax defined by the coding system (e.g. post-coordination).
    */
   code?: string|undefined;
   /**
    * A representation of the meaning of the code in the system, following the rules of the system.
    */
   display?: string|undefined;
   /**
    * Amongst a set of alternatives, a directly chosen code is the most appropriate starting point for new translations. There is some ambiguity about what exactly 'directly chosen' implies, and trading partner agreement may be needed to clarify the use of this element and its consequences more completely.
    */
   userSelected?: boolean|undefined;
}

/**
 * Internal CodeableConcept - equivalent to a FHIR JSON CodeableConcept (Complex DataType), without extensions
 */
 export interface FtsCodeableConcept extends FtsElement { 
  /**
   * Codes may be defined very casually in enumerations, or code lists, up to very formal definitions such as SNOMED CT - see the HL7 v3 Core Principles for more information.  Ordering of codings is undefined and SHALL NOT be used to infer meaning. Generally, at most only one of the coding values will be labeled as UserSelected = true.
   */
  coding?: FtsCoding[]|undefined;
  /**
   * Very often the text is the same as a displayName of one of the codings.
   */
  text?: string|undefined;
}

/**
 * Internal OperationOutcomeIssue - equivalent to OperationOutcome.issue (Backbone Element), without extensions
 */
export interface FtsIssue extends FtsElement {
  /**
   * This is labeled as "Is Modifier" because applications should not confuse hints and warnings with errors.
   */
   severity: 'error'|'fatal'|'information'|'warning'|null;
   /**
    * Describes the type of the issue. The system that creates an OperationOutcome SHALL choose the most applicable code from the IssueType value set, and may additional provide its own code for the error in the details element.
    */
   code: 'business-rule'|'code-invalid'|'conflict'|'deleted'|'duplicate'|'exception'|'expired'|'extension'|'forbidden'|'incomplete'|'informational'|'invalid'|'invariant'|'lock-error'|'login'|'multiple-matches'|'no-store'|'not-found'|'not-supported'|'processing'|'required'|'security'|'structure'|'suppressed'|'throttled'|'timeout'|'too-costly'|'too-long'|'transient'|'unknown'|'value'|null;
   /**
    * A human readable description of the error issue SHOULD be placed in details.text.
    */
   details?: FtsCodeableConcept|undefined;
   /**
    * This may be a description of how a value is erroneous, a stack dump to help trace the issue or other troubleshooting information.
    */
   diagnostics?: string|undefined;
   /**
    * The root of the FHIRPath is the resource or bundle that generated OperationOutcome.  Each FHIRPath SHALL resolve to a single node.
    */
   expression?: string[]|undefined;
 }

export interface FhirBaseArgs { }

export class FhirBase {
  /**
   * Mapping of this datatype to a FHIR equivalent
   */
  public static readonly _fts_dataType:string='Base';
  public static readonly _fts_regex:RegExp = /.?/;

  /** Default constructor */
  constructor(source:Partial<FhirBaseArgs> = {}, options:FhirConstructorOptions = {}) {
    if (options.allowUnknownElements === true) { Object.assign(this, source); }
  }

  /**
   * Function to perform basic model validation (e.g., check if required elements are present).
   */
   public doModelValidation(_exp:string = ''):FtsIssue[] {
    return [];
  }

  /**
   * FNV-1a implementation from https://github.com/tjwebb/fnv-plus
   * @param str 
   * @returns 
   */
  public static _hash52_1a_fast(str:string):number{
		var i,l=str.length-3,t0=0,v0=0x2325,t1=0,v1=0x8422,t2=0,v2=0x9ce4,t3=0,v3=0xcbf2;

		for (i = 0; i < l;) {
			v0^=str.charCodeAt(i++);
			t0=v0*435;t1=v1*435;t2=v2*435;t3=v3*435;
			t2+=v0<<8;t3+=v1<<8;
			t1+=t0>>>16;v0=t0&65535;t2+=t1>>>16;v1=t1&65535;v3=(t3+(t2>>>16))&65535;v2=t2&65535;
			v0^=str.charCodeAt(i++);
			t0=v0*435;t1=v1*435;t2=v2*435;t3=v3*435;
			t2+=v0<<8;t3+=v1<<8;
			t1+=t0>>>16;v0=t0&65535;t2+=t1>>>16;v1=t1&65535;v3=(t3+(t2>>>16))&65535;v2=t2&65535;
			v0^=str.charCodeAt(i++);
			t0=v0*435;t1=v1*435;t2=v2*435;t3=v3*435;
			t2+=v0<<8;t3+=v1<<8;
			t1+=t0>>>16;v0=t0&65535;t2+=t1>>>16;v1=t1&65535;v3=(t3+(t2>>>16))&65535;v2=t2&65535;
			v0^=str.charCodeAt(i++);
			t0=v0*435;t1=v1*435;t2=v2*435;t3=v3*435;
			t2+=v0<<8;t3+=v1<<8;
			t1+=t0>>>16;v0=t0&65535;t2+=t1>>>16;v1=t1&65535;v3=(t3+(t2>>>16))&65535;v2=t2&65535;
		}

		while(i<l+3){
			v0^=str.charCodeAt(i++);
			t0=v0*435;t1=v1*435;t2=v2*435;t3=v3*435;
			t2+=v0<<8;t3+=v1<<8;
			t1+=t0>>>16;v0=t0&65535;t2+=t1>>>16;v1=t1&65535;v3=(t3+(t2>>>16))&65535;v2=t2&65535;
		}

		return (v3&15) * 281474976710656 + v2 * 4294967296 + v1 * 65536 + (v0^(v3>>4));
	}

  /**
   * Validate an Optional Scalar element 
   * @param p 
   * @param exp 
   * @returns 
   */
   public vOS(p:Readonly<string>, exp:Readonly<string>):FtsIssue[] {
    if (((this as any)[p] === undefined) || 
        ((this as any)[p] === null) ||
        ((this as any)[p] === '') ||
        (Number.isNaN((this as any)[p]))) {
      return [];
    }

    if (Array.isArray((this as any)[p])) {
      return [{
        severity: 'error',
        code: 'structure',
        details: {text: `Found array in scalar property ${p} (${exp})`},
        expression: [exp],
      }];
    }

    return (this as any)[p].doModelValidation(exp+'.'+p);
  }

  /**
   * Validate an Optional Scalar element bound to a Value set
   * @param p 
   * @param exp 
   * @param vsN 
   * @param vsV 
   * @param vsS 
   * @returns 
   */
  public vOSV(
    p:Readonly<string>,
    exp:Readonly<string>,
    vsN:string,
    vsV:Readonly<string[]>,
    vsS:Readonly<string>):FtsIssue[] {

    if (((this as any)[p] === undefined) || 
        ((this as any)[p] === null) ||
        ((this as any)[p] === '') ||
        (Number.isNaN((this as any)[p]))) {
      return [];
    }

    if (Array.isArray((this as any)[p])) {
      return [{
        severity: 'error',
        code: 'structure',
        details: {text: `Found array in scalar property ${p} (${exp})`},
        expression: [exp],
      }];
    }

    let iss:FtsIssue[] = [];
    iss.push(...(this as any)[p].doModelValidation(exp+'.'+p));
    if (!(this as any)[p].hasCodingFromValidationObj(vsV)) {
      iss.push({
        severity: (vsS === 'r') ? 'error' : 'information', 
        code:'code-invalid', 
        details:{text:`${p} (${exp}) does not contain code from bound value set ${vsN}`},
        expression: [exp],
      });
    }

    return iss;
  }

  /**
   * Validate an Optional Array element
   * @param p 
   * @param exp 
   * @returns 
   */
  public vOA(p:Readonly<string>, exp:Readonly<string>):FtsIssue[] {
    if (((this as any)[p] === undefined) || 
        ((this as any)[p] === null) ||
        ((this as any)[p] === '') ||
        (Number.isNaN((this as any)[p]))) {
      return [];
    }

    if (!Array.isArray((this as any)[p])) {
      return [{
        severity: 'error',
        code: 'structure',
        details: {text: `Found scalar in array property ${p} (${exp})`}
      }];
    }

    let iss:FtsIssue[] = [];
    (this as any)[p].forEach((x:any,i:number) => {iss.push(...x.doModelValidation(`${exp}.${p}[${i}]`))} );

    return iss;
  }

  /**
   * Validate an Optional Array element bound to a Value set
   * @param p 
   * @param exp 
   * @param vsN 
   * @param vsV 
   * @param vsS 
   * @returns 
   */
  public vOAV(
    p:Readonly<string>,
    exp:Readonly<string>,
    vsN:string,
    vsV:Readonly<string[]>,
    vsS:Readonly<string>):FtsIssue[] {
    let iss:FtsIssue[] = [];

    if (((this as any)[p] === undefined) || 
        ((this as any)[p] === null) ||
        ((this as any)[p] === '') ||
        (Number.isNaN((this as any)[p]))) {
      return [];
    }

    if (!Array.isArray((this as any)[p])) {
      return [{
        severity: 'error',
        code: 'structure',
        details: {text: `Found scalar in array property ${p} (${exp})`},
        expression: [exp],
      }];
    }

    (this as any)[p].forEach((x:any,i:number) => {
      iss.push(...x.doModelValidation(`${exp}.${p}[${i}]`));
      if (!x.hasCodingFromValidationObj(vsV)) {
        iss.push({
          severity: (vsS === 'r') ? 'error' : 'information', 
          code:'code-invalid', 
          details:{text:`${p} (${exp}) does not contain code from bound value set ${vsN}`},
          expression: [exp],
        });
      }
    });

    return iss;
  }

  /**
   * Validate a Required Scalar element
   * @param p 
   * @param exp 
   * @returns 
   */
  public vRS(p:Readonly<string>, exp:Readonly<string>):FtsIssue[] {
    if (((this as any)[p] === undefined) || 
        ((this as any)[p] === null) ||
        ((this as any)[p] === '') ||
        (Number.isNaN((this as any)[p]))) {
      return [{
        severity: 'error',
        code: 'required',
        details: {text: `Missing required property '${p}', ${exp}`},
        expression: [exp],
      }];
    }

    if (Array.isArray((this as any)[p])) {
      return [{
        severity: 'error',
        code: 'structure',
        details: {text: `Found array in scalar property ${p} (${exp})`},
        expression: [exp],
      }];
    }

    if ((this as any)[p].length === 0) {
      return [{
        severity: 'error',
        code: 'required',
        details: {text: `Missing required property '${p}', ${exp}`},
        expression: [exp],
      }];
    }

    return (this as any)[p].doModelValidation(exp+'.'+p);
  }

  /**
   * Validate a Required Scalar element bound to a Value set
   * @param p 
   * @param exp 
   * @param vsN 
   * @param vsV 
   * @param vsS 
   * @returns 
   */
  public vRSV(
    p:Readonly<string>,
    exp:Readonly<string>,
    vsN:string,
    vsV:Readonly<string[]>,
    vsS:Readonly<string>):FtsIssue[] {

    if (((this as any)[p] === undefined) || 
        ((this as any)[p] === null) ||
        ((this as any)[p] === '') ||
        (Number.isNaN((this as any)[p]))) {
      return [{
        severity: 'error',
        code: 'required',
        details: {text: `Missing required property '${p}', ${exp}`},
        expression: [exp],
      }];
    }

    if (Array.isArray((this as any)[p])) {
      return [{
        severity: 'error',
        code: 'structure',
        details: {text: `Found array in scalar property ${p} (${exp})`},
        expression: [exp],
      }];
    }

    let iss:FtsIssue[] = [];
    iss.push(...(this as any)[p].doModelValidation(exp+'.'+p));
    if (!(this as any)[p].hasCodingFromValidationObj(vsV)) {
      iss.push({
        severity: (vsS === 'r') ? 'error' : 'information', 
        code:'code-invalid', 
        details:{text:`${p} (${exp}) does not contain code from bound value set ${vsN}`},
        expression: [exp],
      });
    }
    return iss;
  }
  
  /**
   * Validate a Required Array element
   * @param p 
   * @param exp 
   * @returns 
   */
  public vRA(p:Readonly<string>, exp:Readonly<string>):FtsIssue[] {
    if (((this as any)[p] === undefined) || 
        ((this as any)[p] === null) ||
        ((this as any)[p] === '') ||
        (Number.isNaN((this as any)[p]))) {
      return [{
        severity: 'error',
        code: 'required',
        details: {text: `Missing required property '${p}', ${exp}`},
        expression: [exp],
      }];
    }

    if (!Array.isArray((this as any)[p])) {
      return [{
        severity: 'error',
        code: 'structure',
        details: {text: `Found scalar in array property ${p} (${exp})`},
        expression: [exp],
      }];
    }

    if ((this as any)[p].length === 0) {
      return [{
        severity: 'error',
        code: 'required',
        details: {text: `Missing required property '${p}', ${exp}`},
        expression: [exp],
      }];
    }

    let iss:FtsIssue[] = [];
    (this as any)[p].forEach((x:any,i:number) => {iss.push(...x.doModelValidation(`${exp}.${p}[${i}]`))} );
    return iss;
  }

  /**
   * Validate a Required Array element bound to a Value set
   * @param p 
   * @param exp 
   * @param vsN 
   * @param vsV 
   * @param vsS 
   * @returns 
   */
  public vRAV(
    p:Readonly<string>,
    exp:Readonly<string>,
    vsN:string,
    vsV:Readonly<string[]>,
    vsS:Readonly<string>):FtsIssue[] {

    if (((this as any)[p] === undefined) || 
        ((this as any)[p] === null) ||
        ((this as any)[p] === '') ||
        (Number.isNaN((this as any)[p]))) {
      return [{
        severity: 'error',
        code: 'required',
        details: {text: `Missing required property '${p}', ${exp}`},
        expression: [exp],
      }];
    }

    if (!Array.isArray((this as any)[p])) {
      return [{
        severity: 'error',
        code: 'structure',
        details: {text: `Found scalar in array property ${p} (${exp})`},
        expression: [exp],
      }];
    }

    if ((this as any)[p].length === 0) {
      return [{
        severity: 'error',
        code: 'required',
        details: {text: `Missing required property '${p}', ${exp}`},
        expression: [exp],
      }];
    }

    let iss:FtsIssue[] = [];
    (this as any)[p].forEach((x:any,i:number) => {
      iss.push(...x.doModelValidation(`${exp}.${p}[${i}]`));
      if (!x.hasCodingFromValidationObj(vsV)) {
        iss.push({
          severity: (vsS === 'r') ? 'error' : 'information', 
          code:'code-invalid', 
          details:{text:`${p} (${exp}) does not contain code from bound value set ${vsN}`},
          expression: [exp],
        });
      }
    });
    return iss;
  }

  /**
   * Function to strip invalid element values for serialization.
   */
   public toJSON() {
    let c:any = {};
  
    for (const key in (this as any)) {
      if (((this as any)[key] === undefined) || 
          ((this as any)[key] === null) ||
          ((this as any)[key] === '') ||
          (Number.isNaN((this as any)[key]))) {
        continue;
      }

      let isArray:boolean = Array.isArray((this as any)[key]);

      if (isArray && (this as any)[key].length === 0) {
        continue;
      }

      let ftsDt:string = (isArray ? (this as any)[key][0].constructor._fts_dataType : (this as any)[key].constructor._fts_dataType) ?? '';
      let isChoice:boolean = (this as any).constructor['_fts_' + key + 'IsChoice'] ?? false;
      let isPrimitive = (isArray ? (this as any)[key][0].constructor['_fts_isPrimitive'] : (this as any)[key].constructor['_fts_isPrimitive']) ?? false;
      let dKey:string = key + (isChoice ? ftsDt : '');
  
      if (isArray) {
        if (isPrimitive) {
          const eName:string = '_' + dKey;
          let foundAnyVal:boolean = false;
          let foundAnyExt:boolean = false;
          c[dKey] = [];
          c[eName] = [];
          (this as any)[key].forEach((av:any) => {
            let addElement:boolean = false;
            if ((av.value !== undefined) && (av.value !== null)) { 
              c[dKey].push(av.value); 
              foundAnyVal = true;
              addElement = true;
            } else { 
              c[dKey].push(null);
            }
  
            let ao:object = {};
            if (av.id) { 
              (ao as any)['id'] = av.id;
              foundAnyExt = true;
            }
            if ((av.extension) && (av.extension.length !== 0)) {
              (ao as any)['extension'] = [];
              av.extension.forEach((e:any) => {
                (ao as any)['extension'].push(e);
              });
              foundAnyExt = true;
            }
  
            if (foundAnyExt) { 
              c[eName].push(ao);
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
        } else if (ftsDt) {
          c[dKey] = [];
          (this as any)[key].forEach((v:any) => {
            // c[dKey].push(v.toJSON());
            c[dKey].push(v);
          });
        } else {
          c[dKey] = (this as any)[key];
        }
      } else if (isPrimitive) {
        if (((this as any)[key].value !== undefined) && ((this as any)[key].value !== null)) { c[dKey] = (this as any)[key].value; }

        const eName:string = '_' + dKey;
        c[eName] = {};
        if ((this as any)[key]['id']) { c[eName]['id'] = (this as any)[key]['id']; }
        if (((this as any)[key]['extension']) && ((this as any)[key]['extension'].length !== 0)) {
          c[eName]['extension'] = [];
          (this as any)[key]['extension'].forEach((e:any) => {
            c[eName]['extension'].push(e);
          });
        }

        if (Object.keys(c[eName]).length === 0) { delete c[eName]; }
      } else if (ftsDt) {
        c[dKey] = (this as any)[key];
      } else {
        c[dKey] = (this as any)[key];
      }
    }
  
    return c;
  }
}
