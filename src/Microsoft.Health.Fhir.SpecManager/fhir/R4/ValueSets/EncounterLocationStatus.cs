// <auto-generated />
// Built from: hl7.fhir.r4.core version: 4.0.1
  // Option: "NAMESPACE" = "fhirCsR4"

using fhirCsR4.Models;

namespace fhirCsR4.ValueSets
{
  /// <summary>
  /// The status of the location.
  /// </summary>
  public static class EncounterLocationStatusCodes
  {
    /// <summary>
    /// The patient is currently at this location, or was between the period specified.
    /// A system may update these records when the patient leaves the location to either reserved, or completed.
    /// </summary>
    public static readonly Coding Active = new Coding
    {
      Code = "active",
      Display = "Active",
      System = "http://hl7.org/fhir/encounter-location-status"
    };
    /// <summary>
    /// The patient was at this location during the period specified.
    /// Not to be used when the patient is currently at the location.
    /// </summary>
    public static readonly Coding Completed = new Coding
    {
      Code = "completed",
      Display = "Completed",
      System = "http://hl7.org/fhir/encounter-location-status"
    };
    /// <summary>
    /// The patient is planned to be moved to this location at some point in the future.
    /// </summary>
    public static readonly Coding Planned = new Coding
    {
      Code = "planned",
      Display = "Planned",
      System = "http://hl7.org/fhir/encounter-location-status"
    };
    /// <summary>
    /// This location is held empty for this patient.
    /// </summary>
    public static readonly Coding Reserved = new Coding
    {
      Code = "reserved",
      Display = "Reserved",
      System = "http://hl7.org/fhir/encounter-location-status"
    };
  };
}
