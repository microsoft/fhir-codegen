// <auto-generated />
// Built from: hl7.fhir.r5.core version: 4.6.0
  // Option: "NAMESPACE" = "fhirCsR5"

using fhirCsR5.Models;

namespace fhirCsR5.ValueSets
{
  /// <summary>
  /// Concepts for how a measure report consumer and receiver coordinate data exchange updates. The choices are snapshot or incremental updates
  /// </summary>
  public static class SubmitDataUpdateTypeCodes
  {
    /// <summary>
    /// In contrast to the Snapshot Update, the FHIR Parameters resource used in a Submit Data or the Collect Data scenario contains only the new and updated DEQM and QI Core Profiles since the last transaction. If the Consumer supports incremental updates, the contents of the updated payload updates the previous payload data.
    /// </summary>
    public static readonly Coding Incremental = new Coding
    {
      Code = "incremental",
      Display = "Incremental",
      System = "http://hl7.org/fhir/CodeSystem/submit-data-update-type"
    };
    /// <summary>
    /// In contrast to the Incremental Update, the FHIR Parameters resource used in a Submit Data or the Collect Data scenario contains all the DEQM and QI Core Profiles for each transaction.  If the Consumer supports snapshot updates, the contents of the updated payload entirely replaces the previous payload
    /// </summary>
    public static readonly Coding Snapshot = new Coding
    {
      Code = "snapshot",
      Display = "Snapshot",
      System = "http://hl7.org/fhir/CodeSystem/submit-data-update-type"
    };
  };
}
