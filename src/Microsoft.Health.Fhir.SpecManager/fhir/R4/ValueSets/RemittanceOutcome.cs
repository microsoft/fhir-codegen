// <auto-generated />
// Built from: hl7.fhir.r4.core version: 4.0.1
  // Option: "NAMESPACE" = "fhirCsR4"

using fhirCsR4.Models;

namespace fhirCsR4.ValueSets
{
  /// <summary>
  /// This value set includes Claim Processing Outcome codes.
  /// </summary>
  public static class RemittanceOutcomeCodes
  {
    /// <summary>
    /// The processing has completed without errors
    /// </summary>
    public static readonly Coding ProcessingComplete = new Coding
    {
      Code = "complete",
      Display = "Processing Complete",
      System = "http://hl7.org/fhir/remittance-outcome"
    };
    /// <summary>
    /// One or more errors have been detected in the Claim
    /// </summary>
    public static readonly Coding Error = new Coding
    {
      Code = "error",
      Display = "Error",
      System = "http://hl7.org/fhir/remittance-outcome"
    };
    /// <summary>
    /// No errors have been detected in the Claim and some of the adjudication has been performed.
    /// </summary>
    public static readonly Coding PartialProcessing = new Coding
    {
      Code = "partial",
      Display = "Partial Processing",
      System = "http://hl7.org/fhir/remittance-outcome"
    };
    /// <summary>
    /// The Claim/Pre-authorization/Pre-determination has been received but processing has not begun.
    /// </summary>
    public static readonly Coding Queued = new Coding
    {
      Code = "queued",
      Display = "Queued",
      System = "http://hl7.org/fhir/remittance-outcome"
    };
  };
}
