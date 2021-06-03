// <auto-generated />
// Built from: hl7.fhir.r4.core version: 4.0.1
  // Option: "NAMESPACE" = "fhirCsR4"

using fhirCsR4.Models;

namespace fhirCsR4.ValueSets
{
  /// <summary>
  /// Operations supported by REST at the type or instance level.
  /// </summary>
  public static class TypeRestfulInteractionCodes
  {
    /// <summary>
    /// Create a new resource with a server assigned id.
    /// </summary>
    public static readonly Coding Create = new Coding
    {
      Code = "create",
      Display = "create",
      System = "http://hl7.org/fhir/restful-interaction"
    };
    /// <summary>
    /// Delete a resource.
    /// </summary>
    public static readonly Coding Delete = new Coding
    {
      Code = "delete",
      Display = "delete",
      System = "http://hl7.org/fhir/restful-interaction"
    };
    /// <summary>
    /// Retrieve the change history for a particular resource.
    /// </summary>
    public static readonly Coding HistoryInstance = new Coding
    {
      Code = "history-instance",
      Display = "history-instance",
      System = "http://hl7.org/fhir/restful-interaction"
    };
    /// <summary>
    /// Retrieve the change history for all resources of a particular type.
    /// </summary>
    public static readonly Coding HistoryType = new Coding
    {
      Code = "history-type",
      Display = "history-type",
      System = "http://hl7.org/fhir/restful-interaction"
    };
    /// <summary>
    /// Update an existing resource by posting a set of changes to it.
    /// </summary>
    public static readonly Coding Patch = new Coding
    {
      Code = "patch",
      Display = "patch",
      System = "http://hl7.org/fhir/restful-interaction"
    };
    /// <summary>
    /// Read the current state of the resource.
    /// </summary>
    public static readonly Coding Read = new Coding
    {
      Code = "read",
      Display = "read",
      System = "http://hl7.org/fhir/restful-interaction"
    };
    /// <summary>
    /// Search all resources of the specified type based on some filter criteria.
    /// </summary>
    public static readonly Coding SearchType = new Coding
    {
      Code = "search-type",
      Display = "search-type",
      System = "http://hl7.org/fhir/restful-interaction"
    };
    /// <summary>
    /// Update an existing resource by its id (or create it if it is new).
    /// </summary>
    public static readonly Coding Update = new Coding
    {
      Code = "update",
      Display = "update",
      System = "http://hl7.org/fhir/restful-interaction"
    };
    /// <summary>
    /// Read the state of a specific version of the resource.
    /// </summary>
    public static readonly Coding Vread = new Coding
    {
      Code = "vread",
      Display = "vread",
      System = "http://hl7.org/fhir/restful-interaction"
    };
  };
}
