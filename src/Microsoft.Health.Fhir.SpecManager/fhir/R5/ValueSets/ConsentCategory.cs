// <auto-generated />
// Built from: hl7.fhir.r5.core version: 4.6.0
  // Option: "NAMESPACE" = "fhirCsR5"

using fhirCsR5.Models;

namespace fhirCsR5.ValueSets
{
  /// <summary>
  /// This value set includes sample Consent Directive Type codes, including several consent directive related LOINC codes; HL7 VALUE SET: ActConsentType(2.16.840.1.113883.1.11.19897); examples of US realm consent directive legal descriptions and references to online and/or downloadable forms such as the SSA-827 Authorization to Disclose Information to the Social Security Administration; and other anticipated consent directives related to participation in a clinical trial, medical procedures, reproductive procedures; health care directive (Living Will); advance directive, do not resuscitate (DNR); Physician Orders for Life-Sustaining Treatment (POLST)
  /// </summary>
  public static class ConsentCategoryCodes
  {
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding ActConsentType = new Coding
    {
      Code = "_ActConsentType",
      Display = "ActConsentType",
      System = "http://terminology.hl7.org/CodeSystem/v3-ActCode"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding PrivacyPolicyAcknowledgementDocument = new Coding
    {
      Code = "57016-8",
      Display = "Privacy policy acknowledgement Document",
      System = "http://loinc.org"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding PrivacyPolicyOrganizationDocument = new Coding
    {
      Code = "57017-6",
      Display = "Privacy policy Organization Document ",
      System = "http://loinc.org"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding PatientConsent = new Coding
    {
      Code = "59284-0",
      Display = "Patient Consent ",
      System = "http://loinc.org"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding ReleaseOfInformationConsent = new Coding
    {
      Code = "64292-6",
      Display = "Release of information consent ",
      System = "http://loinc.org"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding AdvanceDirective = new Coding
    {
      Code = "acd",
      Display = "Advance Directive",
      System = "http://terminology.hl7.org/CodeSystem/consentcategorycodes"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding DoNotResuscitate = new Coding
    {
      Code = "dnr",
      Display = "Do Not Resuscitate",
      System = "http://terminology.hl7.org/CodeSystem/consentcategorycodes"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding EmergencyOnly = new Coding
    {
      Code = "emrgonly",
      Display = "Emergency Only",
      System = "http://terminology.hl7.org/CodeSystem/consentcategorycodes"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding HealthCareDirective = new Coding
    {
      Code = "hcd",
      Display = "Health Care Directive",
      System = "http://terminology.hl7.org/CodeSystem/consentcategorycodes"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding InformationCollection = new Coding
    {
      Code = "ICOL",
      Display = "information collection",
      System = "http://terminology.hl7.org/CodeSystem/v3-ActCode"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding InformationDisclosure = new Coding
    {
      Code = "IDSCL",
      Display = "information disclosure",
      System = "http://terminology.hl7.org/CodeSystem/v3-ActCode"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding InformationAccess = new Coding
    {
      Code = "INFA",
      Display = "information access",
      System = "http://terminology.hl7.org/CodeSystem/v3-ActCode"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding AccessOnly = new Coding
    {
      Code = "INFAO",
      Display = "access only",
      System = "http://terminology.hl7.org/CodeSystem/v3-ActCode"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding AccessAndSaveOnly = new Coding
    {
      Code = "INFASO",
      Display = "access and save only",
      System = "http://terminology.hl7.org/CodeSystem/v3-ActCode"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding InformationRedisclosure = new Coding
    {
      Code = "IRDSCL",
      Display = "information redisclosure",
      System = "http://terminology.hl7.org/CodeSystem/v3-ActCode"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding NoticeOfPrivacyPractices = new Coding
    {
      Code = "npp",
      Display = "Notice of Privacy Practices",
      System = "http://terminology.hl7.org/CodeSystem/consentcategorycodes"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding POLST = new Coding
    {
      Code = "polst",
      Display = "POLST",
      System = "http://terminology.hl7.org/CodeSystem/consentcategorycodes"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding ResearchInformationAccess = new Coding
    {
      Code = "research",
      Display = "Research Information Access",
      System = "http://terminology.hl7.org/CodeSystem/consentcategorycodes"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding ResearchInformationAccess_2 = new Coding
    {
      Code = "RESEARCH",
      Display = "research information access",
      System = "http://terminology.hl7.org/CodeSystem/v3-ActCode"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding DeIdentifiedInformationAccess = new Coding
    {
      Code = "rsdid",
      Display = "De-identified Information Access",
      System = "http://terminology.hl7.org/CodeSystem/consentcategorycodes"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding DeIdentifiedInformationAccess_2 = new Coding
    {
      Code = "RSDID",
      Display = "de-identified information access",
      System = "http://terminology.hl7.org/CodeSystem/v3-ActCode"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding ReIdentifiableInformationAccess = new Coding
    {
      Code = "rsreid",
      Display = "Re-identifiable Information Access",
      System = "http://terminology.hl7.org/CodeSystem/consentcategorycodes"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding ReIdentifiableInformationAccess_2 = new Coding
    {
      Code = "RSREID",
      Display = "re-identifiable information access",
      System = "http://terminology.hl7.org/CodeSystem/v3-ActCode"
    };
  };
}
