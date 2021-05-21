// <auto-generated />
// Built from: hl7.fhir.r4.core version: 4.0.1
  // Option: "NAMESPACE" = "fhirCsR4"

using fhirCsR4.Models;

namespace fhirCsR4.ValueSets
{
  /// <summary>
  /// Example value set for clinical impression prognosis.
  /// </summary>
  public static class ClinicalimpressionPrognosisCodes
  {
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding GoodPrognosis = new Coding
    {
      Code = "170968001",
      Display = "Good prognosis",
      System = "http://snomed.info/sct"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding PoorPrognosis = new Coding
    {
      Code = "170969009",
      Display = "Poor prognosis",
      System = "http://snomed.info/sct"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding PrognosisUncertain = new Coding
    {
      Code = "170970005",
      Display = "Prognosis uncertain",
      System = "http://snomed.info/sct"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding ConditionalPrognosis = new Coding
    {
      Code = "60484009",
      Display = "Conditional prognosis",
      System = "http://snomed.info/sct"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding FairPrognosis = new Coding
    {
      Code = "65872000",
      Display = "Fair prognosis",
      System = "http://snomed.info/sct"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding GuardedPrognosis = new Coding
    {
      Code = "67334001",
      Display = "Guarded prognosis",
      System = "http://snomed.info/sct"
    };
  };
}
