// <auto-generated />
// Built from: hl7.fhir.r5.core version: 5.0.0-snapshot1
  // Option: "NAMESPACE" = "fhirCsR5"

using fhirCsR5.Models;

namespace fhirCsR5.ValueSets
{
  /// <summary>
  /// The function performed by the participant for the action.
  /// </summary>
  public static class ActionParticipantFunctionCodes
  {
    /// <summary>
    /// The participant is the author of the result of the action.
    /// </summary>
    public static readonly Coding Author = new Coding
    {
      Code = "author",
      Display = "Author",
      System = "http://hl7.org/fhir/action-participant-function"
    };
    /// <summary>
    /// The participant is the performer of the action.
    /// </summary>
    public static readonly Coding Performer = new Coding
    {
      Code = "performer",
      Display = "Performer",
      System = "http://hl7.org/fhir/action-participant-function"
    };
    /// <summary>
    /// The participant is reviewing the result of the action.
    /// </summary>
    public static readonly Coding Reviewer = new Coding
    {
      Code = "reviewer",
      Display = "Reviewer",
      System = "http://hl7.org/fhir/action-participant-function"
    };
    /// <summary>
    /// The participant is a witness to the action being performed.
    /// </summary>
    public static readonly Coding Witness = new Coding
    {
      Code = "witness",
      Display = "Witness",
      System = "http://hl7.org/fhir/action-participant-function"
    };

    /// <summary>
    /// Literal for code: Author
    /// </summary>
    public const string LiteralAuthor = "author";

    /// <summary>
    /// Literal for code: ActionParticipantFunctionAuthor
    /// </summary>
    public const string LiteralActionParticipantFunctionAuthor = "http://hl7.org/fhir/action-participant-function#author";

    /// <summary>
    /// Literal for code: Performer
    /// </summary>
    public const string LiteralPerformer = "performer";

    /// <summary>
    /// Literal for code: ActionParticipantFunctionPerformer
    /// </summary>
    public const string LiteralActionParticipantFunctionPerformer = "http://hl7.org/fhir/action-participant-function#performer";

    /// <summary>
    /// Literal for code: Reviewer
    /// </summary>
    public const string LiteralReviewer = "reviewer";

    /// <summary>
    /// Literal for code: ActionParticipantFunctionReviewer
    /// </summary>
    public const string LiteralActionParticipantFunctionReviewer = "http://hl7.org/fhir/action-participant-function#reviewer";

    /// <summary>
    /// Literal for code: Witness
    /// </summary>
    public const string LiteralWitness = "witness";

    /// <summary>
    /// Literal for code: ActionParticipantFunctionWitness
    /// </summary>
    public const string LiteralActionParticipantFunctionWitness = "http://hl7.org/fhir/action-participant-function#witness";

    /// <summary>
    /// Dictionary for looking up ActionParticipantFunction Codings based on Codes
    /// </summary>
    public static Dictionary<string, Coding> Values = new Dictionary<string, Coding>() {
      { "author", Author }, 
      { "http://hl7.org/fhir/action-participant-function#author", Author }, 
      { "performer", Performer }, 
      { "http://hl7.org/fhir/action-participant-function#performer", Performer }, 
      { "reviewer", Reviewer }, 
      { "http://hl7.org/fhir/action-participant-function#reviewer", Reviewer }, 
      { "witness", Witness }, 
      { "http://hl7.org/fhir/action-participant-function#witness", Witness }, 
    };
  };
}