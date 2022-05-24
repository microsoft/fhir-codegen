  /**
   * Fluent-style function to add codings
   * @param coding 
   * @returns 
   */
   public addCoding(coding:fhir.CodingArgs):CodeableConcept {
    this.coding.push(new fhir.Coding(coding));
    return this;
  }
