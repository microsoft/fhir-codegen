// <auto-generated />
// Built from: hl7.fhir.r5.core version: 4.6.0
  // Option: "NAMESPACE" = "fhirCsR5"

using fhirCsR5.Models;

namespace fhirCsR5.ValueSets
{
  /// <summary>
  /// Status of the validation of the target against the primary source
  /// </summary>
  public static class VerificationresultValidationStatusCodes
  {
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding Failed = new Coding
    {
      Code = "failed",
      Display = "Failed",
      System = "http://terminology.hl7.org/CodeSystem/validation-status"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding Successful = new Coding
    {
      Code = "successful",
      Display = "Successful",
      System = "http://terminology.hl7.org/CodeSystem/validation-status"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding Unknown = new Coding
    {
      Code = "unknown",
      Display = "Unknown",
      System = "http://terminology.hl7.org/CodeSystem/validation-status"
    };
  };
}
