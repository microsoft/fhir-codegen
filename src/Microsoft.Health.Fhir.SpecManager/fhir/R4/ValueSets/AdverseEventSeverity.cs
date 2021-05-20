// <auto-generated />
// Built from: hl7.fhir.r4.core version: 4.0.1
  // Option: "NAMESPACE" = "fhirCsR4"

using fhirCsR4.Models;

namespace fhirCsR4.ValueSets
{
  /// <summary>
  /// The severity of the adverse event itself, in direct relation to the subject.
  /// </summary>
  public static class AdverseEventSeverityCodes
  {
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding Mild = new Coding
    {
      Code = "mild",
      Display = "Mild",
      System = "http://terminology.hl7.org/CodeSystem/adverse-event-severity"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding Moderate = new Coding
    {
      Code = "moderate",
      Display = "Moderate",
      System = "http://terminology.hl7.org/CodeSystem/adverse-event-severity"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding Severe = new Coding
    {
      Code = "severe",
      Display = "Severe",
      System = "http://terminology.hl7.org/CodeSystem/adverse-event-severity"
    };
  };
}