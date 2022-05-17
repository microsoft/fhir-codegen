
/**
 * Access a bundle.entry[].resource as a typed resource
 */
  resourceAs<BundeContentType = fhir.FhirResource>(): BundeContentType|unknown {
    return this.resource as unknown as BundeContentType;
  }
