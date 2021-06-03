// <auto-generated />
// Built from: hl7.fhir.r4.core version: 4.0.1
  // Option: "NAMESPACE" = "fhirCsR4"

using fhirCsR4.Models;

namespace fhirCsR4.ValueSets
{
  /// <summary>
  /// Indicates the potential degree of impact of the identified issue on the patient.
  /// </summary>
  public static class DetectedissueSeverityCodes
  {
    /// <summary>
    /// Indicates the issue may be life-threatening or has the potential to cause permanent injury.
    /// </summary>
    public static readonly Coding High = new Coding
    {
      Code = "high",
      Display = "High",
      System = "http://hl7.org/fhir/detectedissue-severity"
    };
    /// <summary>
    /// Indicates the issue may result in some adverse consequences but is unlikely to substantially affect the situation of the subject.
    /// </summary>
    public static readonly Coding Low = new Coding
    {
      Code = "low",
      Display = "Low",
      System = "http://hl7.org/fhir/detectedissue-severity"
    };
    /// <summary>
    /// Indicates the issue may result in noticeable adverse consequences but is unlikely to be life-threatening or cause permanent injury.
    /// </summary>
    public static readonly Coding Moderate = new Coding
    {
      Code = "moderate",
      Display = "Moderate",
      System = "http://hl7.org/fhir/detectedissue-severity"
    };
  };
}
