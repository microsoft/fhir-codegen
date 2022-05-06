
  /**
   * Create an extension object with a specified URL and FhirBase value
   * @param url 
   * @param value 
   * @returns 
   */
  static fromValue(url:string, value:fhir.FhirBase):Extension {
    let ext:Extension = new Extension({url: url});
    
    if (!value) {
      return ext;
    }

    const vName = 'value' + value.constructor.name;
    ext[vName] = value;
  }
