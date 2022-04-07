// <auto-generated />
// Built from: hl7.fhir.r4b.core version: 4.3.0-snapshot1
  // Option: "NAMESPACE" = "fhirCsR4B"

using fhirCsR4B.Models;

namespace fhirCsR4B.ValueSets
{
  /// <summary>
  /// Codes that convey the current status of the research study.
  /// </summary>
  public static class ResearchStudyStatusCodes
  {
    /// <summary>
    /// Study is opened for accrual.
    /// </summary>
    public static readonly Coding Active = new Coding
    {
      Code = "active",
      Display = "Active",
      System = "http://hl7.org/fhir/research-study-status"
    };
    /// <summary>
    /// Study is completed prematurely and will not resume; patients are no longer examined nor treated.
    /// </summary>
    public static readonly Coding AdministrativelyCompleted = new Coding
    {
      Code = "administratively-completed",
      Display = "Administratively Completed",
      System = "http://hl7.org/fhir/research-study-status"
    };
    /// <summary>
    /// Protocol is approved by the review board.
    /// </summary>
    public static readonly Coding Approved = new Coding
    {
      Code = "approved",
      Display = "Approved",
      System = "http://hl7.org/fhir/research-study-status"
    };
    /// <summary>
    /// Study is closed for accrual; patients can be examined and treated.
    /// </summary>
    public static readonly Coding ClosedToAccrual = new Coding
    {
      Code = "closed-to-accrual",
      Display = "Closed to Accrual",
      System = "http://hl7.org/fhir/research-study-status"
    };
    /// <summary>
    /// Study is closed to accrual and intervention, i.e. the study is closed to enrollment, all study subjects have completed treatment or intervention but are still being followed according to the primary objective of the study.
    /// </summary>
    public static readonly Coding ClosedToAccrualAndIntervention = new Coding
    {
      Code = "closed-to-accrual-and-intervention",
      Display = "Closed to Accrual and Intervention",
      System = "http://hl7.org/fhir/research-study-status"
    };
    /// <summary>
    /// Study is closed to accrual and intervention, i.e. the study is closed to enrollment, all study subjects have completed treatment
    /// or intervention but are still being followed according to the primary objective of the study.
    /// </summary>
    public static readonly Coding Completed = new Coding
    {
      Code = "completed",
      Display = "Completed",
      System = "http://hl7.org/fhir/research-study-status"
    };
    /// <summary>
    /// Protocol was disapproved by the review board.
    /// </summary>
    public static readonly Coding Disapproved = new Coding
    {
      Code = "disapproved",
      Display = "Disapproved",
      System = "http://hl7.org/fhir/research-study-status"
    };
    /// <summary>
    /// Protocol is submitted to the review board for approval.
    /// </summary>
    public static readonly Coding InReview = new Coding
    {
      Code = "in-review",
      Display = "In Review",
      System = "http://hl7.org/fhir/research-study-status"
    };
    /// <summary>
    /// Study is temporarily closed for accrual; can be potentially resumed in the future; patients can be examined and treated.
    /// </summary>
    public static readonly Coding TemporarilyClosedToAccrual = new Coding
    {
      Code = "temporarily-closed-to-accrual",
      Display = "Temporarily Closed to Accrual",
      System = "http://hl7.org/fhir/research-study-status"
    };
    /// <summary>
    /// Study is temporarily closed for accrual and intervention and potentially can be resumed in the future.
    /// </summary>
    public static readonly Coding TemporarilyClosedToAccrualAndIntervention = new Coding
    {
      Code = "temporarily-closed-to-accrual-and-intervention",
      Display = "Temporarily Closed to Accrual and Intervention",
      System = "http://hl7.org/fhir/research-study-status"
    };
    /// <summary>
    /// Protocol was withdrawn by the lead organization.
    /// </summary>
    public static readonly Coding Withdrawn = new Coding
    {
      Code = "withdrawn",
      Display = "Withdrawn",
      System = "http://hl7.org/fhir/research-study-status"
    };

    /// <summary>
    /// Literal for code: Active
    /// </summary>
    public const string LiteralActive = "active";

    /// <summary>
    /// Literal for code: ResearchStudyStatusActive
    /// </summary>
    public const string LiteralResearchStudyStatusActive = "http://hl7.org/fhir/research-study-status#active";

    /// <summary>
    /// Literal for code: AdministrativelyCompleted
    /// </summary>
    public const string LiteralAdministrativelyCompleted = "administratively-completed";

    /// <summary>
    /// Literal for code: ResearchStudyStatusAdministrativelyCompleted
    /// </summary>
    public const string LiteralResearchStudyStatusAdministrativelyCompleted = "http://hl7.org/fhir/research-study-status#administratively-completed";

    /// <summary>
    /// Literal for code: Approved
    /// </summary>
    public const string LiteralApproved = "approved";

    /// <summary>
    /// Literal for code: ResearchStudyStatusApproved
    /// </summary>
    public const string LiteralResearchStudyStatusApproved = "http://hl7.org/fhir/research-study-status#approved";

    /// <summary>
    /// Literal for code: ClosedToAccrual
    /// </summary>
    public const string LiteralClosedToAccrual = "closed-to-accrual";

    /// <summary>
    /// Literal for code: ResearchStudyStatusClosedToAccrual
    /// </summary>
    public const string LiteralResearchStudyStatusClosedToAccrual = "http://hl7.org/fhir/research-study-status#closed-to-accrual";

    /// <summary>
    /// Literal for code: ClosedToAccrualAndIntervention
    /// </summary>
    public const string LiteralClosedToAccrualAndIntervention = "closed-to-accrual-and-intervention";

    /// <summary>
    /// Literal for code: ResearchStudyStatusClosedToAccrualAndIntervention
    /// </summary>
    public const string LiteralResearchStudyStatusClosedToAccrualAndIntervention = "http://hl7.org/fhir/research-study-status#closed-to-accrual-and-intervention";

    /// <summary>
    /// Literal for code: Completed
    /// </summary>
    public const string LiteralCompleted = "completed";

    /// <summary>
    /// Literal for code: ResearchStudyStatusCompleted
    /// </summary>
    public const string LiteralResearchStudyStatusCompleted = "http://hl7.org/fhir/research-study-status#completed";

    /// <summary>
    /// Literal for code: Disapproved
    /// </summary>
    public const string LiteralDisapproved = "disapproved";

    /// <summary>
    /// Literal for code: ResearchStudyStatusDisapproved
    /// </summary>
    public const string LiteralResearchStudyStatusDisapproved = "http://hl7.org/fhir/research-study-status#disapproved";

    /// <summary>
    /// Literal for code: InReview
    /// </summary>
    public const string LiteralInReview = "in-review";

    /// <summary>
    /// Literal for code: ResearchStudyStatusInReview
    /// </summary>
    public const string LiteralResearchStudyStatusInReview = "http://hl7.org/fhir/research-study-status#in-review";

    /// <summary>
    /// Literal for code: TemporarilyClosedToAccrual
    /// </summary>
    public const string LiteralTemporarilyClosedToAccrual = "temporarily-closed-to-accrual";

    /// <summary>
    /// Literal for code: ResearchStudyStatusTemporarilyClosedToAccrual
    /// </summary>
    public const string LiteralResearchStudyStatusTemporarilyClosedToAccrual = "http://hl7.org/fhir/research-study-status#temporarily-closed-to-accrual";

    /// <summary>
    /// Literal for code: TemporarilyClosedToAccrualAndIntervention
    /// </summary>
    public const string LiteralTemporarilyClosedToAccrualAndIntervention = "temporarily-closed-to-accrual-and-intervention";

    /// <summary>
    /// Literal for code: ResearchStudyStatusTemporarilyClosedToAccrualAndIntervention
    /// </summary>
    public const string LiteralResearchStudyStatusTemporarilyClosedToAccrualAndIntervention = "http://hl7.org/fhir/research-study-status#temporarily-closed-to-accrual-and-intervention";

    /// <summary>
    /// Literal for code: Withdrawn
    /// </summary>
    public const string LiteralWithdrawn = "withdrawn";

    /// <summary>
    /// Literal for code: ResearchStudyStatusWithdrawn
    /// </summary>
    public const string LiteralResearchStudyStatusWithdrawn = "http://hl7.org/fhir/research-study-status#withdrawn";

    /// <summary>
    /// Dictionary for looking up ResearchStudyStatus Codings based on Codes
    /// </summary>
    public static Dictionary<string, Coding> Values = new Dictionary<string, Coding>() {
      { "active", Active }, 
      { "http://hl7.org/fhir/research-study-status#active", Active }, 
      { "administratively-completed", AdministrativelyCompleted }, 
      { "http://hl7.org/fhir/research-study-status#administratively-completed", AdministrativelyCompleted }, 
      { "approved", Approved }, 
      { "http://hl7.org/fhir/research-study-status#approved", Approved }, 
      { "closed-to-accrual", ClosedToAccrual }, 
      { "http://hl7.org/fhir/research-study-status#closed-to-accrual", ClosedToAccrual }, 
      { "closed-to-accrual-and-intervention", ClosedToAccrualAndIntervention }, 
      { "http://hl7.org/fhir/research-study-status#closed-to-accrual-and-intervention", ClosedToAccrualAndIntervention }, 
      { "completed", Completed }, 
      { "http://hl7.org/fhir/research-study-status#completed", Completed }, 
      { "disapproved", Disapproved }, 
      { "http://hl7.org/fhir/research-study-status#disapproved", Disapproved }, 
      { "in-review", InReview }, 
      { "http://hl7.org/fhir/research-study-status#in-review", InReview }, 
      { "temporarily-closed-to-accrual", TemporarilyClosedToAccrual }, 
      { "http://hl7.org/fhir/research-study-status#temporarily-closed-to-accrual", TemporarilyClosedToAccrual }, 
      { "temporarily-closed-to-accrual-and-intervention", TemporarilyClosedToAccrualAndIntervention }, 
      { "http://hl7.org/fhir/research-study-status#temporarily-closed-to-accrual-and-intervention", TemporarilyClosedToAccrualAndIntervention }, 
      { "withdrawn", Withdrawn }, 
      { "http://hl7.org/fhir/research-study-status#withdrawn", Withdrawn }, 
    };
  };
}