  /**
   * Fluent-style function to add codings
   * @param coding 
   * @returns this CodeableConcept, with the additional coding
   */
   public addCoding(coding:fhir.CodingArgs):CodeableConcept {
    this.coding.push(new fhir.Coding(coding));
    return this;
  }

  /**
   * Fluent-style function to remove ALL instances of codings with a system and/or code
   * @param system 
   * @param code 
   * @returns this CodeableConcept, updated without the requesting coding
   */
  public removeCoding(system:fhir.FhirString|string, code:fhir.FhirString|string):CodeableConcept {
    if (this.coding.length === 0) {
      return this;
    }
    const s:string = (typeof system === 'string') ? system : (system as fhir.FhirString).value ?? '';
    const c:string = (typeof code === 'string') ? code : (code as fhir.FhirString).value ?? '';
    let clean:fhir.Coding[] = this.coding.filter((coding) => (((s === '') || (coding.system?.value === s)) && ((c === '') || (coding.code?.value === c)))) ?? [];
    this.coding = clean;
    return this;
  }

  /**
   * Find the first coding with a matching system and/or code
   * @param system 
   * @param code 
   * @returns A matching Coding or undefined.
   */
  public findCoding(system:fhir.FhirString|string, code:fhir.FhirString|string):fhir.Coding|undefined {
    if (this.coding.length === 0) {
      return undefined;
    }
    const s:string = (typeof system === 'string') ? system : (system as fhir.FhirString).value ?? '';
    const c:string = (typeof code === 'string') ? code : (code as fhir.FhirString).value ?? '';
    return this.coding.find((coding) => (((s === '') || (coding.system?.value === s)) && ((c === '') || (coding.code?.value === c))));
  }

  /**
   * Find all instances of an extension with a matching URL, optionally recurse into extension.extension.
   * @param url URL to search for
   * @param searchNested If the search should nest into extensions
   * @returns A new array of FHIR Extensions, with just the desired extensions
   */
  public filterCodings(system:fhir.FhirString|string, code:fhir.FhirString|string):fhir.Coding[] {
    if (this.coding.length === 0) {
      return [];
    }
    const s:string = (typeof system === 'string') ? system : (system as fhir.FhirString).value ?? '';
    const c:string = (typeof code === 'string') ? code : (code as fhir.FhirString).value ?? '';
    return this.coding.filter((coding) => (((s === '') || (coding.system?.value === s)) && ((c === '') || (coding.code?.value === c))));
  }

  /**
   * Test whether this CodeableConcept contains a specific coding.
   * @param system System to search for, empty string will match any system.
   * @param code Code to search for, empty string will match any code.
   * @returns True if this concept contains the specified coding, false if it does not.
   */
  public hasCoding(system:fhir.FhirString|string, code:fhir.FhirString|string):boolean {
    if (this.coding.length === 0) {
      return false;
    }
    
    const s:string = (typeof system === 'string') ? system : (system as fhir.FhirString).value ?? '';
    const c:string = (typeof code === 'string') ? code : (code as fhir.FhirString).value ?? '';

    if ((s) && (c)) {
      return (this.coding.find((coding) => (coding.system?.value === s) && (coding.code?.value === c)) !== undefined);
    }

    if (s) {
      return (this.coding.find((coding) => (coding.system?.value === s)) !== undefined);
    }

    if (c) {
      return (this.coding.find((coding) => coding.code?.value === c) !== undefined);
    }

    return false;
  }

  /**
   * Test whether this CodeableConcept contains a specific coding.
   * @param system System to search for, empty string will match any system.
   * @param code Code to search for, empty string will match any code.
   * @returns True if this concept contains the specified coding, false if it does not.
   */
  public hasCodingFromObject(codings:any):boolean {
    if (this.coding.length === 0) {
      return false;
    }

    for (const obj of Object.values(codings)) {
      if (!obj) { continue; }
      const system:string = (obj as any)['system'] ?? '';
      const code:string = (obj as any)['code'] ?? '';

      if (this.hasCoding(system, code)) {
        return true;
      }
    }

    return false;
  }

  /**
   * Test whether this CodeableConcept contains a specific coding.
   * @param system System to search for, empty string will match any system.
   * @param code Code to search for, empty string will match any code.
   * @returns True if this concept contains the specified coding, false if it does not.
   */
  public hasCodingFromValidationObj(vsValidation:Readonly<string[]>):boolean {
    if (this.coding.length === 0) {
      return false;
    }

    for (const coding of this.coding) {
      const sc:string = (coding.system?.value ?? '') + '|' + (coding.code?.value ?? '');
      const c:string = coding.code?.value ?? '';

      if (vsValidation.find((v) => (v === sc) || (v === c))) {
        return true;
      }
    }

    return false;
  }

  /**
   * Test whether this CodeableConcept contains a specific coding.
   * @param system System to search for, empty string will match any system.
   * @param code Code to search for, empty string will match any code.
   * @returns True if this concept contains the specified coding, false if it does not.
   */
  public hasCodingFromValidationHash(vsValidation:Readonly<number[]>):boolean {
    if (this.coding.length === 0) {
      return false;
    }

    for (const coding of this.coding) {
      const sc:number = fhir.FhirBase._hash52_1a_fast((coding.system?.value ?? '') + '|' + (coding.code?.value ?? ''));
      const c:number = fhir.FhirBase._hash52_1a_fast(coding.code?.value ?? '');

      if (vsValidation.find((v) => (v === sc) || (v === c))) {
        return true;
      }
    }

    return false;
  }
  