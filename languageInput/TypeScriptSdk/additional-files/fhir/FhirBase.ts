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
   public doModelValidation():FtsIssue[] {
    let issues:FtsIssue[] = [];
    return issues;
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
          ((this as any)[key] === NaN) ||
          ((this as any)[key] === [])) {
        continue;
      }
  
      if (key.startsWith('_fts_')) {
        continue;
      }
  
      let dKey:string = key + ((this as any)['_fts_' + key + 'IsChoice'] ? ((this as any)[key]['_fts_dataType'] ?? '') : '');
  
      if (Array.isArray((this as any)[key])) {
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
          c[dKey] = (this as any)[key];
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