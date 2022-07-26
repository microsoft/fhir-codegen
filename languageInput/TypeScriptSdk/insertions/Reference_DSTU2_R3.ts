
  /**
   * Create a reference from an existing resource
   */
   static fromResource(source:fhir.FhirResource, baseUrl:string=''):Reference {
    if (baseUrl.endsWith('/')) {
      return new Reference({
        reference: baseUrl + source.resourceType + '/' + source.id,
      });
    }

    return new Reference({
      reference: ((baseUrl.length > 0) ? (baseUrl + '/') : '') + source.resourceType + '/' + source.id,
    });
  }
