
  /**
   * Create a reference from an existing resource
   */
   static fromResource(source:fhirInterfaces.IResource, baseUrl:string=''):Reference {
    if (baseUrl.endsWith('/')) {
      return Reference.FactoryCreate({
        type: source.resourceType,
        reference: baseUrl + source.resourceType + '/' + source.id,
      });
    }

    return Reference.FactoryCreate({
      type: source.resourceType,
      reference: ((baseUrl.length > 0) ? (baseUrl + '/') : '') + source.resourceType + '/' + source.id,
    });
  }
