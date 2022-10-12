
/**
 * Access a bundle.entry[].resource as a typed resource
 */
  resourceAs<BundeContentType = fhir.IFhirResource>(): BundeContentType|unknown {
    return this.resource as unknown as BundeContentType;
  }
