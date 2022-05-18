// Minimum TypeScript Version: 3.7
// FHIR Primitive: uuid

import * as fhir from '../fhir.js';

import { IssueTypeCodes } from '../fhirValueSets/IssueTypeCodes.js';
import { IssueSeverityCodes } from '../fhirValueSets/IssueSeverityCodes.js';

/**
 * See The Open Group, CDE 1.1 Remote Procedure Call specification, Appendix A.
 */
export interface FhirUuidArgs extends fhir.FhirUriArgs {
  /**
   * See The Open Group, CDE 1.1 Remote Procedure Call specification, Appendix A.
   */
  value?:FhirUuid|string|undefined;
}

/**
 * See The Open Group, CDE 1.1 Remote Procedure Call specification, Appendix A.
 */
export class FhirUuid extends fhir.FhirUri {
  /**
   * Mapping of this datatype to a FHIR equivalent
   */
  public static readonly _fts_dataType:string = 'Uuid';
  /**
   * Mapping of this datatype to a JSON equivalent
   */
  public static readonly _fts_jsonType:string = 'string';
  // published regex: urn:uuid:[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}
  public static readonly _fts_regex:RegExp = /^urn:uuid:[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/
  /**
   * A uuid value, represented as a JS string
   */
  declare value?:string|null|undefined;
  /**
     * Create a FhirUuid
     * @param value See The Open Group, CDE 1.1 Remote Procedure Call specification, Appendix A.
     * @param id Unique id for inter-element referencing (uncommon on primitives)
     * @param extension Additional content defined by implementations
     * @param options Options to pass to extension constructors
  */
  constructor(source:Partial<FhirUuidArgs> = {}, options:fhir.FhirConstructorOptions = { } ) {
    super(source, options);
  }
  /**
   * Function to perform basic model validation (e.g., check if required elements are present).
   */
  public override doModelValidation():fhir.OperationOutcome {
    var outcome:fhir.OperationOutcome = super.doModelValidation();
    if ((this.value) && (!FhirUuid._fts_regex.test(this.value))) {
      outcome.issue!.push(new fhir.OperationOutcomeIssue({ severity: IssueSeverityCodes.Error, code: IssueTypeCodes.InvalidContent,  diagnostics: "Invalid value in primitive type uuid", }));
    }
    return outcome;
  }
}
