// <auto-generated />
// Built from: hl7.fhir.r5.core version: 4.6.0
  // Option: "NAMESPACE" = "fhirCsR5"

using fhirCsR5.Models;

namespace fhirCsR5.ValueSets
{
  /// <summary>
  /// Possible types of subjects.
  /// </summary>
  public static class SubjectTypeCodes
  {
    /// <summary>
    /// A type of a manufactured item that is used in the provision of healthcare without being substantially changed through that activity. The device may be a medical or non-medical device.
    /// </summary>
    public static readonly Coding Device = new Coding
    {
      Code = "Device",
      Display = "Device",
      System = "http://hl7.org/fhir/resource-types"
    };
    /// <summary>
    /// Details and position information for a physical place where services are provided and resources and participants may be stored, found, contained, or accommodated.
    /// </summary>
    public static readonly Coding Location = new Coding
    {
      Code = "Location",
      Display = "Location",
      System = "http://hl7.org/fhir/resource-types"
    };
    /// <summary>
    /// A formally or informally recognized grouping of people or organizations formed for the purpose of achieving some form of collective action.  Includes companies, institutions, corporations, departments, community groups, healthcare practice groups, payer/insurer, etc.
    /// </summary>
    public static readonly Coding Organization = new Coding
    {
      Code = "Organization",
      Display = "Organization",
      System = "http://hl7.org/fhir/resource-types"
    };
    /// <summary>
    /// Demographics and other administrative information about an individual or animal receiving care or other health-related services.
    /// </summary>
    public static readonly Coding Patient = new Coding
    {
      Code = "Patient",
      Display = "Patient",
      System = "http://hl7.org/fhir/resource-types"
    };
    /// <summary>
    /// A person who is directly or indirectly involved in the provisioning of healthcare.
    /// </summary>
    public static readonly Coding Practitioner = new Coding
    {
      Code = "Practitioner",
      Display = "Practitioner",
      System = "http://hl7.org/fhir/resource-types"
    };
  };
}
