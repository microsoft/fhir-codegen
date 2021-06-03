// <auto-generated />
// Built from: hl7.fhir.r5.core version: 4.6.0
  // Option: "NAMESPACE" = "fhirCsR5"

using fhirCsR5.Models;

namespace fhirCsR5.ValueSets
{
  /// <summary>
  /// Used to code the producer or rule for creating the display string.
  /// </summary>
  public static class ContributorSummarySourceCodes
  {
    /// <summary>
    /// Data copied by human from article text.
    /// </summary>
    public static readonly Coding CopiedFromArticle = new Coding
    {
      Code = "article-copy",
      Display = "Copied from article",
      System = "http://terminology.hl7.org/CodeSystem/contributor-summary-source"
    };
    /// <summary>
    /// Data copied by machine from citation manager data.
    /// </summary>
    public static readonly Coding ReportedByCitationManager = new Coding
    {
      Code = "citation-manager",
      Display = "Reported by citation manager",
      System = "http://terminology.hl7.org/CodeSystem/contributor-summary-source"
    };
    /// <summary>
    /// Custom format (may be described in text note).
    /// </summary>
    public static readonly Coding CustomFormat = new Coding
    {
      Code = "custom",
      Display = "custom format",
      System = "http://terminology.hl7.org/CodeSystem/contributor-summary-source"
    };
    /// <summary>
    /// Data copied by machine from publisher data.
    /// </summary>
    public static readonly Coding PublisherProvided = new Coding
    {
      Code = "publisher-data",
      Display = "Publisher provided",
      System = "http://terminology.hl7.org/CodeSystem/contributor-summary-source"
    };
  };
}
