
/**
 * Convert a HumanName into a displayable string
 */

  ResourceAs<BundeContentType = fhir.IFhirResource>(): BundeContentType|unknown {
    return this.resource as unknown as BundeContentType;
  }
