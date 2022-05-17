// Minimum TypeScript Version: 3.7
// FHIR Primitive: oid

import * as fhir from '../fhir.js';

import { IssueTypeCodes } from '../fhirValueSets/IssueTypeCodes.js';
import { IssueSeverityCodes } from '../fhirValueSets/IssueSeverityCodes.js';

/**
 * RFC 3001. See also ISO/IEC 8824:1990 €
 */
export interface FhirOidArgs extends fhir.FhirStringArgs {
  /**
   * RFC 3001. See also ISO/IEC 8824:1990 €
   */
  value?:FhirOid|string|undefined;
}

/**
 * RFC 3001. See also ISO/IEC 8824:1990 €
 */
export class FhirOid extends fhir.FhirString {
  protected static readonly _fts_dataType:string = 'Oid';
  protected static readonly _fts_jsonType:string = 'string';
  // published regex: urn:oid:[0-2](\.(0|[1-9][0-9]*))+
  static readonly _fts_regex:RegExp = /^urn:oid:[0-2](\.(0|[1-9][0-9]*))+$/
  /**
   * A oid value, represented as a JS string
   */
  declare value?:string|null|undefined;
  /**
     * Create a FhirOid
     * @param value RFC 3001. See also ISO/IEC 8824:1990 €
     * @param id Unique id for inter-element referencing (uncommon on primitives)
     * @param extension Additional content defined by implementations
     * @param options Options to pass to extension constructors
  */
  constructor(source:Partial<FhirOidArgs> = {}, options:fhir.FhirConstructorOptions = { } ) {
    super(source, options);
  }
  /**
   * Function to perform basic model validation (e.g., check if required elements are present).
   */
  public override doModelValidation():fhir.OperationOutcome {
    var outcome:fhir.OperationOutcome = super.doModelValidation();
    if ((this.value) && (!FhirOid._fts_regex.test(this.value))) {
      outcome.issue!.push(new fhir.OperationOutcomeIssue({ severity: IssueSeverityCodes.Error, code: IssueTypeCodes.InvalidContent,  diagnostics: "Invalid value in primitive type oid", }));
    }
    return outcome;
  }
}
