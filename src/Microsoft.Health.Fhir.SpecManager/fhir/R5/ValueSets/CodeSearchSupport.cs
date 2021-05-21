// <auto-generated />
// Built from: hl7.fhir.r5.core version: 4.6.0
  // Option: "NAMESPACE" = "fhirCsR5"

using fhirCsR5.Models;

namespace fhirCsR5.ValueSets
{
  /// <summary>
  /// The degree to which the server supports the code search parameter on ValueSet, if it is supported.
  /// </summary>
  public static class CodeSearchSupportCodes
  {
    /// <summary>
    /// The search for code on ValueSet only includes all codes based on the expansion of the value set.
    /// </summary>
    public static readonly Coding ImplicitCodes = new Coding
    {
      Code = "all",
      Display = "Implicit Codes",
      System = "http://hl7.org/fhir/code-search-support"
    };
    /// <summary>
    /// The search for code on ValueSet only includes codes explicitly detailed on includes or expansions.
    /// </summary>
    public static readonly Coding ExplicitCodes = new Coding
    {
      Code = "explicit",
      Display = "Explicit Codes",
      System = "http://hl7.org/fhir/code-search-support"
    };
  };
}
