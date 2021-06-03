// <auto-generated />
// Built from: hl7.fhir.r4.core version: 4.0.1
  // Option: "NAMESPACE" = "fhirCsR4"

using fhirCsR4.Models;

namespace fhirCsR4.ValueSets
{
  /// <summary>
  /// The type of method used to execute a subscription.
  /// </summary>
  public static class SubscriptionChannelTypeCodes
  {
    /// <summary>
    /// The channel is executed by sending an email to the email addressed in the URI (which must be a mailto:).
    /// </summary>
    public static readonly Coding Email = new Coding
    {
      Code = "email",
      Display = "Email",
      System = "http://hl7.org/fhir/subscription-channel-type"
    };
    /// <summary>
    /// The channel is executed by sending a message (e.g. a Bundle with a MessageHeader resource etc.) to the application identified in the URI.
    /// </summary>
    public static readonly Coding Message = new Coding
    {
      Code = "message",
      Display = "Message",
      System = "http://hl7.org/fhir/subscription-channel-type"
    };
    /// <summary>
    /// The channel is executed by making a post to the URI. If a payload is included, the URL is interpreted as the service base, and an update (PUT) is made.
    /// </summary>
    public static readonly Coding RestHook = new Coding
    {
      Code = "rest-hook",
      Display = "Rest Hook",
      System = "http://hl7.org/fhir/subscription-channel-type"
    };
    /// <summary>
    /// The channel is executed by sending an SMS message to the phone number identified in the URL (tel:).
    /// </summary>
    public static readonly Coding SMS = new Coding
    {
      Code = "sms",
      Display = "SMS",
      System = "http://hl7.org/fhir/subscription-channel-type"
    };
    /// <summary>
    /// The channel is executed by sending a packet across a web socket connection maintained by the client. The URL identifies the websocket, and the client binds to this URL.
    /// </summary>
    public static readonly Coding Websocket = new Coding
    {
      Code = "websocket",
      Display = "Websocket",
      System = "http://hl7.org/fhir/subscription-channel-type"
    };
  };
}
