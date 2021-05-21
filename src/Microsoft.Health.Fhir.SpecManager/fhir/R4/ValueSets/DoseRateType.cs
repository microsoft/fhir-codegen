// <auto-generated />
// Built from: hl7.fhir.r4.core version: 4.0.1
  // Option: "NAMESPACE" = "fhirCsR4"

using fhirCsR4.Models;

namespace fhirCsR4.ValueSets
{
  /// <summary>
  /// The kind of dose or rate specified.
  /// </summary>
  public static class DoseRateTypeCodes
  {
    /// <summary>
    /// The dose specified is calculated by the prescriber or the system.
    /// </summary>
    public static readonly Coding Calculated = new Coding
    {
      Code = "calculated",
      Display = "Calculated",
      System = "http://terminology.hl7.org/CodeSystem/dose-rate-type"
    };
    /// <summary>
    /// The dose specified is as ordered by the prescriber.
    /// </summary>
    public static readonly Coding Ordered = new Coding
    {
      Code = "ordered",
      Display = "Ordered",
      System = "http://terminology.hl7.org/CodeSystem/dose-rate-type"
    };
  };
}
