// <auto-generated />
// Built from: hl7.fhir.r5.core version: 4.6.0
  // Option: "NAMESPACE" = "fhirCsR5"

using fhirCsR5.Models;

namespace fhirCsR5.ValueSets
{
  /// <summary>
  /// This is an example set of Content Derivative type codes, which represent the minimal content derived from the basal information source at a specific stage in its lifecycle, which is sufficient to manage that source information, for example, in a repository, registry, processes and workflows, for making access control decisions, and providing query responses.
  /// </summary>
  public static class ContractContentDerivativeCodes
  {
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding ContentRegistration = new Coding
    {
      Code = "registration",
      Display = "Content Registration",
      System = "http://terminology.hl7.org/CodeSystem/contract-content-derivative"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding ContentRetrieval = new Coding
    {
      Code = "retrieval",
      Display = "Content Retrieval",
      System = "http://terminology.hl7.org/CodeSystem/contract-content-derivative"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding ShareableContent = new Coding
    {
      Code = "shareable",
      Display = "Shareable Content",
      System = "http://terminology.hl7.org/CodeSystem/contract-content-derivative"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding ContentStatement = new Coding
    {
      Code = "statement",
      Display = "Content Statement",
      System = "http://terminology.hl7.org/CodeSystem/contract-content-derivative"
    };
  };
}
