// <auto-generated />
// Built from: hl7.fhir.r5.core version: 4.6.0
  // Option: "NAMESPACE" = "fhirCsR5"

using fhirCsR5.Models;

namespace fhirCsR5.ValueSets
{
  /// <summary>
  /// Type for quality report.
  /// </summary>
  public static class QualityTypeCodes
  {
    /// <summary>
    /// INDEL Comparison.
    /// </summary>
    public static readonly Coding INDELComparison = new Coding
    {
      Code = "indel",
      Display = "INDEL Comparison",
      System = "http://hl7.org/fhir/quality-type"
    };
    /// <summary>
    /// SNP Comparison.
    /// </summary>
    public static readonly Coding SNPComparison = new Coding
    {
      Code = "snp",
      Display = "SNP Comparison",
      System = "http://hl7.org/fhir/quality-type"
    };
    /// <summary>
    /// UNKNOWN Comparison.
    /// </summary>
    public static readonly Coding UNKNOWNComparison = new Coding
    {
      Code = "unknown",
      Display = "UNKNOWN Comparison",
      System = "http://hl7.org/fhir/quality-type"
    };
  };
}
