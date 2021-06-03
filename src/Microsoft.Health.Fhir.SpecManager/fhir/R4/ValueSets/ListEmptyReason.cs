// <auto-generated />
// Built from: hl7.fhir.r4.core version: 4.0.1
  // Option: "NAMESPACE" = "fhirCsR4"

using fhirCsR4.Models;

namespace fhirCsR4.ValueSets
{
  /// <summary>
  /// General reasons for a list to be empty. Reasons are either related to a summary list (i.e. problem or medication list) or to a workflow related list (i.e. consultation list).
  /// </summary>
  public static class ListEmptyReasonCodes
  {
    /// <summary>
    /// This list has now closed or has ceased to be relevant or useful.
    /// </summary>
    public static readonly Coding Closed = new Coding
    {
      Code = "closed",
      Display = "Closed",
      System = "http://terminology.hl7.org/CodeSystem/list-empty-reason"
    };
    /// <summary>
    /// Clinical judgment that there are no known items for this list after reasonable investigation. Note that this a positive statement by a clinical user, and not a default position asserted by a computer system in the lack of other information. Example uses:  * For allergies: the patient or patient's agent/guardian has asserted that he/she is not aware of any allergies (NKA - nil known allergies)  * For medications: the patient or patient's agent/guardian has asserted that the patient is known to be taking no medications  * For diagnoses, problems and procedures: the patient or patient's agent/guardian has asserted that there is no known event to record.
    /// </summary>
    public static readonly Coding NilKnown = new Coding
    {
      Code = "nilknown",
      Display = "Nil Known",
      System = "http://terminology.hl7.org/CodeSystem/list-empty-reason"
    };
    /// <summary>
    /// The investigation to find out whether there are items for this list has not occurred.
    /// </summary>
    public static readonly Coding NotAsked = new Coding
    {
      Code = "notasked",
      Display = "Not Asked",
      System = "http://terminology.hl7.org/CodeSystem/list-empty-reason"
    };
    /// <summary>
    /// The work to populate this list has not yet begun.
    /// </summary>
    public static readonly Coding NotStarted = new Coding
    {
      Code = "notstarted",
      Display = "Not Started",
      System = "http://terminology.hl7.org/CodeSystem/list-empty-reason"
    };
    /// <summary>
    /// Information to populate this list cannot be obtained; e.g. unconscious patient.
    /// </summary>
    public static readonly Coding Unavailable = new Coding
    {
      Code = "unavailable",
      Display = "Unavailable",
      System = "http://terminology.hl7.org/CodeSystem/list-empty-reason"
    };
    /// <summary>
    /// The content of the list was not provided due to privacy or confidentiality concerns. Note that it should not be assumed that this means that the particular information in question was withheld due to its contents - it can also be a policy decision.
    /// </summary>
    public static readonly Coding InformationWithheld = new Coding
    {
      Code = "withheld",
      Display = "Information Withheld",
      System = "http://terminology.hl7.org/CodeSystem/list-empty-reason"
    };
  };
}
