// <auto-generated />
// Built from: hl7.fhir.r2.core version: 1.0.2
  // Option: "NAMESPACE" = "fhirCsR2"

using fhirCsR2.Models;

namespace fhirCsR2.ValueSets
{
  /// <summary>
  /// Complete, proposed, exploratory, other.
  /// </summary>
  public static class ClaimUseLinkCodes
  {
    /// <summary>
    /// The treatment is complete and this represents a Claim for the services.
    /// </summary>
    public static readonly Coding Complete = new Coding
    {
      Code = "complete",
      Display = "Complete",
      System = "http://hl7.org/fhir/claim-use-link"
    };
    /// <summary>
    /// The treatment is proposed and this represents a Pre-determination for the services.
    /// </summary>
    public static readonly Coding Exploratory = new Coding
    {
      Code = "exploratory",
      Display = "Exploratory",
      System = "http://hl7.org/fhir/claim-use-link"
    };
    /// <summary>
    /// A locally defined or otherwise resolved status.
    /// </summary>
    public static readonly Coding Other = new Coding
    {
      Code = "other",
      Display = "Other",
      System = "http://hl7.org/fhir/claim-use-link"
    };
    /// <summary>
    /// The treatment is proposed and this represents a Pre-authorization for the services.
    /// </summary>
    public static readonly Coding Proposed = new Coding
    {
      Code = "proposed",
      Display = "Proposed",
      System = "http://hl7.org/fhir/claim-use-link"
    };

    /// <summary>
    /// Literal for code: Complete
    /// </summary>
    public const string LiteralComplete = "complete";

    /// <summary>
    /// Literal for code: ClaimUseLinkComplete
    /// </summary>
    public const string LiteralClaimUseLinkComplete = "http://hl7.org/fhir/claim-use-link#complete";

    /// <summary>
    /// Literal for code: Exploratory
    /// </summary>
    public const string LiteralExploratory = "exploratory";

    /// <summary>
    /// Literal for code: ClaimUseLinkExploratory
    /// </summary>
    public const string LiteralClaimUseLinkExploratory = "http://hl7.org/fhir/claim-use-link#exploratory";

    /// <summary>
    /// Literal for code: Other
    /// </summary>
    public const string LiteralOther = "other";

    /// <summary>
    /// Literal for code: ClaimUseLinkOther
    /// </summary>
    public const string LiteralClaimUseLinkOther = "http://hl7.org/fhir/claim-use-link#other";

    /// <summary>
    /// Literal for code: Proposed
    /// </summary>
    public const string LiteralProposed = "proposed";

    /// <summary>
    /// Literal for code: ClaimUseLinkProposed
    /// </summary>
    public const string LiteralClaimUseLinkProposed = "http://hl7.org/fhir/claim-use-link#proposed";

    /// <summary>
    /// Dictionary for looking up ClaimUseLink Codings based on Codes
    /// </summary>
    public static Dictionary<string, Coding> Values = new Dictionary<string, Coding>() {
      { "complete", Complete }, 
      { "http://hl7.org/fhir/claim-use-link#complete", Complete }, 
      { "exploratory", Exploratory }, 
      { "http://hl7.org/fhir/claim-use-link#exploratory", Exploratory }, 
      { "other", Other }, 
      { "http://hl7.org/fhir/claim-use-link#other", Other }, 
      { "proposed", Proposed }, 
      { "http://hl7.org/fhir/claim-use-link#proposed", Proposed }, 
    };
  };
}