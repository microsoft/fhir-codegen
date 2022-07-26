// Minimum TypeScript Version: 3.7

import * as fhir from '../fhir.js';

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
    * Unique id for inter-element referencing
    */
  id?:string|undefined;

  /**
    * Additional content defined by implementations
    */
  extension:fhir.Extension[] = [];

  /**
   * Constructor for FHIR primitive type elements
   * @param value 
   * @param id 
   * @param extension 
   * @param options 
   */
  constructor(source:Partial<FhirPrimitiveArgs> = {}, options:fhir.FhirConstructorOptions = {}) {
    super({}, options);
    if (source) {
      if ((source.value !== undefined) && (source.value.constructor) && (source.value.constructor['_fts_dataType'])) {
        this.value = source.value.value ?? null;
        this.id = source.value.id ?? undefined;
        if ((source.value.extension) && (source.value.extension.length > 0)) {
          this.extension = [];
          source.value.extension!.forEach((e:any) => {
            if (e) { this.extension!.push(new fhir.Extension(e, options)); }
          });
        }
      } else if (source.value !== undefined) {
        this.value = source.value;
      }
  
      if (source.id) { this.id = source.id; }
      if ((source.extension) && (source.extension.length > 0)) {
        if (!this.extension) { this.extension = []; }
        source.extension!.forEach((e:any) => {
          if (e) { this.extension!.push(new fhir.Extension(e, options)); }
        });
      }
    } else {
      this.extension = [];
    }
  }

  /**
   * Function to perform basic model validation (e.g., check if required elements are present).
   */
  public override doModelValidation(exp:string = ''):fhir.FtsIssue[] {
    let iss:fhir.FtsIssue[] = super.doModelValidation(exp);
    if ((this.value === undefined) && (!this.id) && ((!this.extension) || (this.extension.length === 0))) {
      iss.push({
        severity: 'error',
        code: 'required',
        details: { text: 'Primitive values must have a value, id, or extensions.' },
        expression: [exp],
      });
    }
    return iss;
  }

  /**
   * Fluent-style function to add extended primitive properties
   * @param source 
   */
   public addExtendedProperties(source:fhir.FhirElementArgs = {}):fhir.FhirPrimitive {
    if (source.id) { this.id = source.id.toString(); }
    if (source.extension) {
      source.extension.forEach((x) => {
        this.extension.push(new fhir.Extension(x));
      });
    }
    return this;
  }

  /**
   * Fluent-style function to add extensions
   * @param ext
   * @returns 
   */
   public addExtension(ext:fhir.ExtensionArgs):fhir.FhirPrimitive {
    this.extension.push(new fhir.Extension(ext));
    return this;
  }
  
  /**
   * Remove ALL instances of extensions with a matching URL, optionally recurse into extension.extension.
   * @param url URL of extensions to remove
   * @param searchNested If the removal should search for nested extensions
   */
  public removeExtensions(url:fhir.FhirString|string, searchNested:boolean = false):fhir.FhirPrimitive {
    if (this.extension.length === 0) {
      return this;
    }
    const matchUrl:string = (typeof url === 'string') ? url : (url as fhir.FhirString).value ?? '';
    let clean:fhir.Extension[] = this.extension.filter((ext) => (ext?.url?.value ?? '') !== matchUrl) ?? [];
    if (searchNested) {
      for (let i:number = 0; i < clean.length; i++) {
        clean[i] = clean[i].removeExtensions(matchUrl, searchNested) as fhir.Extension;
      }
    }
    this.extension = clean;
    return this;
  }

  /**
   * Find the first instance of an extension with a matching URL, optionally recurse into extension.extension.
   * @param url URL to search for
   * @param searchNested If the search should nest into extensions
   * @returns The FHIR Extension if found, or undefined.
   */
  public findExtension(url:fhir.FhirString|string, searchNested:boolean = false):fhir.Extension|undefined {
    if (this.extension.length === 0) {
      return undefined;
    }
    const matchUrl:string = (typeof url === 'string') ? url : (url as fhir.FhirString).value ?? '';
    if (searchNested) {
      const flat:fhir.Extension[] = FhirPrimitive.recurseForExtension(matchUrl, this.extension);
      if (flat.length === 0) {
        return undefined;
      }
      return flat[0];
    }
    return this.extension.find((ext) => (ext?.url?.value === matchUrl));
  }

  /**
   * Find all instances of an extension with a matching URL, optionally recurse into extension.extension.
   * @param url URL to search for
   * @param searchNested If the search should nest into extensions
   * @returns A new array of FHIR Extensions, with just the desired extensions
   */
  public filterExtensions(url:fhir.FhirString|string, searchNested:boolean = false):fhir.Extension[] {
    if (this.extension.length === 0) {
      return [];
    }
    const matchUrl:string = (typeof url === 'string') ? url : (url as fhir.FhirString).value ?? '';
    if (searchNested) {
      return FhirPrimitive.recurseForExtension(matchUrl, this.extension);
    }
    return this.extension.filter((ext) => (ext?.url?.value === matchUrl))
  }

  /**
   * Internal recursive search function
   * @param url 
   * @param exts 
   * @returns A new array (flat) of matching extensions
   */
  private static recurseForExtension(url:string, exts:fhir.Extension[]):fhir.Extension[] {
    if ((!exts) || (exts.length === 0)) {
      return [];
    }
    let val:fhir.Extension[] = [];
    exts.forEach(
      (ext) => {
        if (ext && ext.url && (ext.url.value === url)) {
          val.push(ext);
          return;
        }
        if (ext && ext.extension && (ext.extension.length > 0)) {
          val.push(...this.recurseForExtension(url, ext.extension));
          return;
        }
        return;
      },
      []);
    return val;
  }
}