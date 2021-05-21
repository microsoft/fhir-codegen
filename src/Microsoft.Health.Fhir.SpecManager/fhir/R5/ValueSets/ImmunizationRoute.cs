// <auto-generated />
// Built from: hl7.fhir.r5.core version: 4.6.0
  // Option: "NAMESPACE" = "fhirCsR5"

using fhirCsR5.Models;

namespace fhirCsR5.ValueSets
{
  /// <summary>
  /// The value set to instantiate this attribute should be drawn from a terminologically robust code system that consists of or contains concepts to support describing the administrative routes used during vaccination. This value set is provided as a suggestive example.
  /// </summary>
  public static class ImmunizationRouteCodes
  {
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding InjectionIntradermal = new Coding
    {
      Code = "IDINJ",
      Display = "Injection, intradermal",
      System = "http://terminology.hl7.org/CodeSystem/v3-RouteOfAdministration"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding InjectionIntramuscular = new Coding
    {
      Code = "IM",
      Display = "Injection, intramuscular",
      System = "http://terminology.hl7.org/CodeSystem/v3-RouteOfAdministration"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding InjectionIntravenous = new Coding
    {
      Code = "IVINJ",
      Display = "Injection, intravenous",
      System = "http://terminology.hl7.org/CodeSystem/v3-RouteOfAdministration"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding InhalationNasal = new Coding
    {
      Code = "NASINHLC",
      Display = "Inhalation, nasal",
      System = "http://terminology.hl7.org/CodeSystem/v3-RouteOfAdministration"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding SwallowOral = new Coding
    {
      Code = "PO",
      Display = "Swallow, oral",
      System = "http://terminology.hl7.org/CodeSystem/v3-RouteOfAdministration"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding InjectionSubcutaneous = new Coding
    {
      Code = "SQ",
      Display = "Injection, subcutaneous",
      System = "http://terminology.hl7.org/CodeSystem/v3-RouteOfAdministration"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding Transdermal = new Coding
    {
      Code = "TRNSDERM",
      Display = "Transdermal",
      System = "http://terminology.hl7.org/CodeSystem/v3-RouteOfAdministration"
    };
  };
}
