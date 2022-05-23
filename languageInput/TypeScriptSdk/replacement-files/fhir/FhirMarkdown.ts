// Minimum TypeScript Version: 3.7
// FHIR Primitive: markdown

import * as fhir from '../fhir.js';

/**
 * Systems are not required to have markdown support, so the text should be readable without markdown processing. The markdown syntax is GFM - see https://github.github.com/gfm/
 */
export interface FhirMarkdownArgs extends fhir.FhirPrimitiveArgs {
  /**
   * Systems are not required to have markdown support, so the text should be readable without markdown processing. The markdown syntax is GFM - see https://github.github.com/gfm/
   */
  value?:FhirMarkdown|string|undefined|null;
}

/**
 * Systems are not required to have markdown support, so the text should be readable without markdown processing. The markdown syntax is GFM - see https://github.github.com/gfm/
 */
export class FhirMarkdown extends fhir.FhirPrimitive {
  /**
   * Mapping of this datatype to a FHIR equivalent
   */
  public static override readonly _fts_dataType:string = 'Markdown';
  /**
   * Mapping of this datatype to a JSON equivalent
   */
  public static override readonly _fts_jsonType:string = 'string';
  // published regex: [ \r\n\t\S]+
  public static override readonly _fts_regex:RegExp = /^[ \r\n\t\S]+$/
  /**
   * A markdown value, represented as a JS string
   */
  declare value?:string|null|undefined;
  /**
     * Create a FhirMarkdown
     * @param value Systems are not required to have markdown support, so the text should be readable without markdown processing. The markdown syntax is GFM - see https://github.github.com/gfm/
     * @param id Unique id for inter-element referencing (uncommon on primitives)
     * @param extension Additional content defined by implementations
     * @param options Options to pass to extension constructors
  */
  constructor(source:Partial<FhirMarkdownArgs> = {}, options:fhir.FhirConstructorOptions = { } ) {
    super(source, options);
  }
  /**
   * Function to perform basic model validation (e.g., check if required elements are present).
   */
  public override doModelValidation():fhir.FtsIssue[] {
    let issues:fhir.FtsIssue[] = super.doModelValidation();
    if ((this.value) && (!FhirMarkdown._fts_regex.test(this.value))) {
      issues.push({ severity: 'error', code: 'invalid',  diagnostics: 'Invalid value in primitive type markdown', });
    }
    return issues;
  }
}
