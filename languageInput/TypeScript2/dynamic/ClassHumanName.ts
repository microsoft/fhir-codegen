
/**
 * Convert a HumanName into a displayable string
 */
toDisplay(familyFirst:boolean = true, includeAnnotations:boolean = false):string {
  if ((this.text) && (this.text.length > 0)) {
    return this.text;
  }

  var val:string = '';

  if (familyFirst) {
    if (this.family) {
      val = this.family;
    }

    if (this.given) {
      val += (val.length > 0 ? ', ' : '') + this.given.join(' ');
    }

    if (includeAnnotations) {
      if (this.suffix) {
        val += (val.length > 0 ? ', ' : '') + this.suffix.join(', ');
      }

      if (this.prefix) {
        val += (val.length > 0 ? ', ' : '') + this.prefix.join(', ');
      }
    }

    return val;
  }

  if ((includeAnnotations) && (this.prefix)) {
    val += this.prefix.join(', ');
  }

  if (this.given) {
    val = (val.length > 0 ? ' ' : '') + this.given.join(' ');
  }
  if (this.family) {
    val += (val.length > 0 ? ' ' : '') + this.family;
  }

  if ((includeAnnotations) && (this.suffix)) {
    val += (val.length > 0 ? ', ' : '') + this.suffix.join(', ');
  }

  return val;
}
