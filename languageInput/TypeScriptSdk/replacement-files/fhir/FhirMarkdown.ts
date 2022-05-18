// Minimum TypeScript Version: 3.7
// FHIR Primitive: markdown

import * as fhir from '../fhir.js';

import { IssueTypeCodes } from '../fhirValueSets/IssueTypeCodes.js';
import { IssueSeverityCodes } from '../fhirValueSets/IssueSeverityCodes.js';

/**
 * Systems are not required to have markdown support, so the text should be readable without markdown processing. The markdown syntax is GFM - see https://github.github.com/gfm/
 */
export interface FhirMarkdownArgs extends fhir.FhirStringArgs {
  /**
   * Systems are not required to have markdown support, so the text should be readable without markdown processing. The markdown syntax is GFM - see https://github.github.com/gfm/
   */
  value?:FhirMarkdown|string|undefined;
}

/**
 * Systems are not required to have markdown support, so the text should be readable without markdown processing. The markdown syntax is GFM - see https://github.github.com/gfm/
 */
export class FhirMarkdown extends fhir.FhirString {
  /**
   * Mapping of this datatype to a FHIR equivalent
   */
  public static readonly _fts_dataType:string = 'Markdown';
  /**
   * Mapping of this datatype to a JSON equivalent
   */
  public static readonly _fts_jsonType:string = 'string';
  // published regex: [ \r\n\t\S]+
  public static readonly _fts_regex:RegExp = /^[ \r\n\t\S]+$/
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
  public override doModelValidation():fhir.OperationOutcome {
    var outcome:fhir.OperationOutcome = super.doModelValidation();
    if ((this.value) && (!FhirMarkdown._fts_regex.test(this.value))) {
      outcome.issue!.push(new fhir.OperationOutcomeIssue({ severity: IssueSeverityCodes.Error, code: IssueTypeCodes.InvalidContent,  diagnostics: "Invalid value in primitive type markdown", }));
    }
    return outcome;
  }
}
