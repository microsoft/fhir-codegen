// <auto-generated />
// Built from: hl7.fhir.r4.core version: 4.0.1
  // Option: "NAMESPACE" = "fhirCsR4"

using fhirCsR4.Models;

namespace fhirCsR4.ValueSets
{
  /// <summary>
  /// Mode for this instance of data.
  /// </summary>
  public static class MapInputModeCodes
  {
    /// <summary>
    /// Names an input instance used a source for mapping.
    /// </summary>
    public static readonly Coding SourceInstance = new Coding
    {
      Code = "source",
      Display = "Source Instance",
      System = "http://hl7.org/fhir/map-input-mode"
    };
    /// <summary>
    /// Names an instance that is being populated.
    /// </summary>
    public static readonly Coding TargetInstance = new Coding
    {
      Code = "target",
      Display = "Target Instance",
      System = "http://hl7.org/fhir/map-input-mode"
    };
  };
}
