// <auto-generated />
// Built from: hl7.fhir.r4.core version: 4.0.1
  // Option: "NAMESPACE" = "fhirCsR4"

using fhirCsR4.Models;

namespace fhirCsR4.ValueSets
{
  /// <summary>
  /// How to interpret the context.
  /// </summary>
  public static class MapContextTypeCodes
  {
    /// <summary>
    /// The context specifies a type.
    /// </summary>
    public static readonly Coding Type = new Coding
    {
      Code = "type",
      Display = "Type",
      System = "http://hl7.org/fhir/map-context-type"
    };
    /// <summary>
    /// The context specifies a variable.
    /// </summary>
    public static readonly Coding Variable = new Coding
    {
      Code = "variable",
      Display = "Variable",
      System = "http://hl7.org/fhir/map-context-type"
    };
  };
}
