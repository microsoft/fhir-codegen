// <auto-generated />
// Built from: hl7.fhir.r3.core version: 3.0.2
  // Option: "NAMESPACE" = "fhirCsR3"

using fhirCsR3.Models;

namespace fhirCsR3.ValueSets
{
  /// <summary>
  /// A coded concept indicating the current status of a the Device Usage
  /// </summary>
  public static class DeviceStatementStatusCodes
  {
    /// <summary>
    /// The device is still being used.
    /// </summary>
    public static readonly Coding Active = new Coding
    {
      Code = "active",
      Display = "Active",
      System = "http://hl7.org/fhir/device-statement-status"
    };
    /// <summary>
    /// The device is no longer being used.
    /// </summary>
    public static readonly Coding Completed = new Coding
    {
      Code = "completed",
      Display = "Completed",
      System = "http://hl7.org/fhir/device-statement-status"
    };
    /// <summary>
    /// The statement was recorded incorrectly.
    /// </summary>
    public static readonly Coding EnteredInError = new Coding
    {
      Code = "entered-in-error",
      Display = "Entered in Error",
      System = "http://hl7.org/fhir/device-statement-status"
    };
    /// <summary>
    /// The device may be used at some time in the future.
    /// </summary>
    public static readonly Coding Intended = new Coding
    {
      Code = "intended",
      Display = "Intended",
      System = "http://hl7.org/fhir/device-statement-status"
    };
    /// <summary>
    /// Actions implied by the statement have been temporarily halted, but are expected to continue later. May also be called "suspended".
    /// </summary>
    public static readonly Coding OnHold = new Coding
    {
      Code = "on-hold",
      Display = "On Hold",
      System = "http://hl7.org/fhir/device-statement-status"
    };
    /// <summary>
    /// Actions implied by the statement have been permanently halted, before all of them occurred.
    /// </summary>
    public static readonly Coding Stopped = new Coding
    {
      Code = "stopped",
      Display = "Stopped",
      System = "http://hl7.org/fhir/device-statement-status"
    };

    /// <summary>
    /// Literal for code: Active
    /// </summary>
    public const string LiteralActive = "active";

    /// <summary>
    /// Literal for code: DeviceStatementStatusActive
    /// </summary>
    public const string LiteralDeviceStatementStatusActive = "http://hl7.org/fhir/device-statement-status#active";

    /// <summary>
    /// Literal for code: Completed
    /// </summary>
    public const string LiteralCompleted = "completed";

    /// <summary>
    /// Literal for code: DeviceStatementStatusCompleted
    /// </summary>
    public const string LiteralDeviceStatementStatusCompleted = "http://hl7.org/fhir/device-statement-status#completed";

    /// <summary>
    /// Literal for code: EnteredInError
    /// </summary>
    public const string LiteralEnteredInError = "entered-in-error";

    /// <summary>
    /// Literal for code: DeviceStatementStatusEnteredInError
    /// </summary>
    public const string LiteralDeviceStatementStatusEnteredInError = "http://hl7.org/fhir/device-statement-status#entered-in-error";

    /// <summary>
    /// Literal for code: Intended
    /// </summary>
    public const string LiteralIntended = "intended";

    /// <summary>
    /// Literal for code: DeviceStatementStatusIntended
    /// </summary>
    public const string LiteralDeviceStatementStatusIntended = "http://hl7.org/fhir/device-statement-status#intended";

    /// <summary>
    /// Literal for code: OnHold
    /// </summary>
    public const string LiteralOnHold = "on-hold";

    /// <summary>
    /// Literal for code: DeviceStatementStatusOnHold
    /// </summary>
    public const string LiteralDeviceStatementStatusOnHold = "http://hl7.org/fhir/device-statement-status#on-hold";

    /// <summary>
    /// Literal for code: Stopped
    /// </summary>
    public const string LiteralStopped = "stopped";

    /// <summary>
    /// Literal for code: DeviceStatementStatusStopped
    /// </summary>
    public const string LiteralDeviceStatementStatusStopped = "http://hl7.org/fhir/device-statement-status#stopped";

    /// <summary>
    /// Dictionary for looking up DeviceStatementStatus Codings based on Codes
    /// </summary>
    public static Dictionary<string, Coding> Values = new Dictionary<string, Coding>() {
      { "active", Active }, 
      { "http://hl7.org/fhir/device-statement-status#active", Active }, 
      { "completed", Completed }, 
      { "http://hl7.org/fhir/device-statement-status#completed", Completed }, 
      { "entered-in-error", EnteredInError }, 
      { "http://hl7.org/fhir/device-statement-status#entered-in-error", EnteredInError }, 
      { "intended", Intended }, 
      { "http://hl7.org/fhir/device-statement-status#intended", Intended }, 
      { "on-hold", OnHold }, 
      { "http://hl7.org/fhir/device-statement-status#on-hold", OnHold }, 
      { "stopped", Stopped }, 
      { "http://hl7.org/fhir/device-statement-status#stopped", Stopped }, 
    };
  };
}