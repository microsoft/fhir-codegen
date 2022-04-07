// <auto-generated />
// Built from: hl7.fhir.r4.core version: 4.0.1
  // Option: "NAMESPACE" = "fhirCsR4"

using fhirCsR4.Models;

namespace fhirCsR4.ValueSets
{
  /// <summary>
  /// How resource references can be aggregated.
  /// </summary>
  public static class ResourceAggregationModeCodes
  {
    /// <summary>
    /// The resource the reference points to will be found in the same bundle as the resource that includes the reference.
    /// </summary>
    public static readonly Coding Bundled = new Coding
    {
      Code = "bundled",
      Display = "Bundled",
      System = "http://hl7.org/fhir/resource-aggregation-mode"
    };
    /// <summary>
    /// The reference is a local reference to a contained resource.
    /// </summary>
    public static readonly Coding Contained = new Coding
    {
      Code = "contained",
      Display = "Contained",
      System = "http://hl7.org/fhir/resource-aggregation-mode"
    };
    /// <summary>
    /// The reference to a resource that has to be resolved externally to the resource that includes the reference.
    /// </summary>
    public static readonly Coding Referenced = new Coding
    {
      Code = "referenced",
      Display = "Referenced",
      System = "http://hl7.org/fhir/resource-aggregation-mode"
    };

    /// <summary>
    /// Literal for code: Bundled
    /// </summary>
    public const string LiteralBundled = "bundled";

    /// <summary>
    /// Literal for code: ResourceAggregationModeBundled
    /// </summary>
    public const string LiteralResourceAggregationModeBundled = "http://hl7.org/fhir/resource-aggregation-mode#bundled";

    /// <summary>
    /// Literal for code: Contained
    /// </summary>
    public const string LiteralContained = "contained";

    /// <summary>
    /// Literal for code: ResourceAggregationModeContained
    /// </summary>
    public const string LiteralResourceAggregationModeContained = "http://hl7.org/fhir/resource-aggregation-mode#contained";

    /// <summary>
    /// Literal for code: Referenced
    /// </summary>
    public const string LiteralReferenced = "referenced";

    /// <summary>
    /// Literal for code: ResourceAggregationModeReferenced
    /// </summary>
    public const string LiteralResourceAggregationModeReferenced = "http://hl7.org/fhir/resource-aggregation-mode#referenced";

    /// <summary>
    /// Dictionary for looking up ResourceAggregationMode Codings based on Codes
    /// </summary>
    public static Dictionary<string, Coding> Values = new Dictionary<string, Coding>() {
      { "bundled", Bundled }, 
      { "http://hl7.org/fhir/resource-aggregation-mode#bundled", Bundled }, 
      { "contained", Contained }, 
      { "http://hl7.org/fhir/resource-aggregation-mode#contained", Contained }, 
      { "referenced", Referenced }, 
      { "http://hl7.org/fhir/resource-aggregation-mode#referenced", Referenced }, 
    };
  };
}