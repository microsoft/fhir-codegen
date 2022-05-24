  /**
   * Fluent-style function to add extensions
   * @param ext
   * @returns 
   */
  public addExtension(ext:fhir.ExtensionArgs):fhir.FhirElement {
    this.extension.push(new fhir.Extension(ext));
    return this;
  }
  
  /**
   * Remove ALL instances of extensions with a matching URL, optionally recurse into extension.extension.
   * @param url URL of extensions to remove
   * @param searchNested If the removal should search for nested extensions
   */
  public removeExtensions(url:fhir.FhirString|string, searchNested:boolean = false):fhir.FhirElement {
    if (this.extension.length === 0) {
      return this;
    }
    const matchUrl:string = (typeof url === 'string') ? url : (url as fhir.FhirString).value ?? '';
    let clean:fhir.Extension[] = this.extension.filter((ext) => (ext?.url?.value ?? '') !== matchUrl) ?? [];
    if (searchNested) {
      for (let i:number = 0; i < clean.length; i++) {
        clean[i] = clean[i].removeExtensions(matchUrl, searchNested) as fhir.Extension;
      }
    }
    this.extension = clean;
    return this;
  }

  /**
   * Find the first instance of an extension with a matching URL, optionally recurse into extension.extension.
   * @param url URL to search for
   * @param searchNested If the search should nest into extensions
   * @returns The FHIR Extension if found, or undefined.
   */
  public findExtension(url:fhir.FhirString|string, searchNested:boolean = false):fhir.Extension|undefined {
    if (this.extension.length === 0) {
      return undefined;
    }
    const matchUrl:string = (typeof url === 'string') ? url : (url as fhir.FhirString).value ?? '';
    if (searchNested) {
      const flat:fhir.Extension[] = FhirElement.recurseForExtension(matchUrl, this.extension);
      if (flat.length === 0) {
        return undefined;
      }
      return flat[0];
    }
    return this.extension.find((ext) => (ext?.url?.value === matchUrl));
  }

  /**
   * Find all instances of an extension with a matching URL, optionally recurse into extension.extension.
   * @param url URL to search for
   * @param searchNested If the search should nest into extensions
   * @returns A new array of FHIR Extensions, with just the desired extensions
   */
  public filterExtensions(url:fhir.FhirString|string, searchNested:boolean = false):fhir.Extension[] {
    if (this.extension.length === 0) {
      return [];
    }
    const matchUrl:string = (typeof url === 'string') ? url : (url as fhir.FhirString).value ?? '';
    if (searchNested) {
      return FhirElement.recurseForExtension(matchUrl, this.extension);
    }
    return this.extension.filter((ext) => (ext?.url?.value === matchUrl))
  }

  /**
   * Internal recursive search function
   * @param url 
   * @param exts 
   * @returns A new array (flat) of matching extensions
   */
  private static recurseForExtension(url:string, exts:fhir.Extension[]):fhir.Extension[] {
    if ((!exts) || (exts.length === 0)) {
      return [];
    }
    let val:fhir.Extension[] = [];
    exts.forEach(
      (ext) => {
        if (ext && ext.url && (ext.url.value === url)) {
          val.push(ext);
          return;
        }
        if (ext && ext.extension && (ext.extension.length > 0)) {
          val.push(...this.recurseForExtension(url, ext.extension));
          return;
        }
        return;
      },
      []);
    return val;
  }
