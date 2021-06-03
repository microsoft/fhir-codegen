// <auto-generated />
// Built from: hl7.fhir.r5.core version: 4.6.0
  // Option: "NAMESPACE" = "fhirCsR5"

using fhirCsR5.Models;

namespace fhirCsR5.ValueSets
{
  /// <summary>
  /// This example FHIR value set is comprised of example Actor Type codes, which can be used to value FHIR agents, actors, and other role         elements such as those specified in financial transactions. The FHIR Actor value set is based on    DICOM Audit Message, C402;   ASTM Standard, E1762-95 [2013]; selected codes and          derived actor roles from HL7 RoleClass OID 2.16.840.1.113883.5.110;    HL7 Role Code 2.16.840.1.113883.5.111, including AgentRoleType;          HL7 ParticipationType OID: 2.16.840.1.113883.5.90; and    HL7 ParticipationFunction codes OID: 2.16.840.1.113883.5.88.           This value set includes, by reference, role codes from external code systems: NUCC Health Care Provider Taxonomy OID: 2.16.840.1.113883.6.101;          North American Industry Classification System [NAICS]OID: 2.16.840.1.113883.6.85; IndustryClassificationSystem 2.16.840.1.113883.1.11.16039;          and US Census Occupation Code OID: 2.16.840.1.113883.6.243 for relevant recipient or custodian codes not included in this value set.            If no source is indicated in the definition comments, then these are example FHIR codes.          It can be extended with appropriate roles described by SNOMED as well as those described in the HL7 Role Based Access Control Catalog and the          HL7 Healthcare (Security and Privacy) Access Control Catalog.            In Role-Based Access Control (RBAC), permissions are operations on an object that a user wishes to access. Permissions are grouped into roles.          A role characterizes the functions a user is allowed to perform. Roles are assigned to users. If the user's role has the appropriate permissions          to access an object, then that user is granted access to the object. FHIR readily enables RBAC, as FHIR Resources are object types and the CRUDE          events (the FHIR equivalent to permissions in the RBAC scheme) are operations on those objects.          In Attribute-Based Access Control (ABAC), a user requests to perform operations on objects. That user's access request is granted or denied          based on a set of access control policies that are specified in terms of attributes and conditions. FHIR readily enables ABAC, as instances of          a Resource in FHIR (again, Resources are object types) can have attributes associated with them. These attributes include security tags,          environment conditions, and a host of user and object characteristics, which are the same attributes as those used in ABAC. Attributes help          define the access control policies that determine the operations a user may perform on a Resource (in FHIR) or object (in ABAC). For example,          a tag (or attribute) may specify that the identified Resource (object) is not to be further disclosed without explicit consent from the patient.
  /// </summary>
  public static class SecurityRoleTypeCodes
  {
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding CitizenRoleType = new Coding
    {
      Code = "_CitizenRoleType",
      Display = "CitizenRoleType",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleCode"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding AFFL = new Coding
    {
      Code = "AFFL",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleClass"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding AGNT = new Coding
    {
      Code = "AGNT",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleClass"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding AMENDER = new Coding
    {
      Code = "AMENDER",
      System = "http://terminology.hl7.org/CodeSystem/contractsignertypecodes"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding ASSIGNED = new Coding
    {
      Code = "ASSIGNED",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleClass"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding AUCG = new Coding
    {
      Code = "AUCG",
      System = "http://terminology.hl7.org/CodeSystem/v3-ParticipationFunction"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding AULR = new Coding
    {
      Code = "AULR",
      System = "http://terminology.hl7.org/CodeSystem/v3-ParticipationFunction"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding AUT = new Coding
    {
      Code = "AUT",
      System = "http://terminology.hl7.org/CodeSystem/v3-ParticipationType"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding AuthorizationServer = new Coding
    {
      Code = "authserver",
      Display = "Authorization Server",
      System = "http://terminology.hl7.org/CodeSystem/extra-security-role-type"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding AUTM = new Coding
    {
      Code = "AUTM",
      System = "http://terminology.hl7.org/CodeSystem/v3-ParticipationFunction"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding AUWA = new Coding
    {
      Code = "AUWA",
      System = "http://terminology.hl7.org/CodeSystem/v3-ParticipationFunction"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding AsylumSeeker = new Coding
    {
      Code = "CAS",
      Display = "asylum seeker",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleCode"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding SingleMinorAsylumSeeker = new Coding
    {
      Code = "CASM",
      Display = "single minor asylum seeker",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleCode"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding CLAIM = new Coding
    {
      Code = "CLAIM",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleClass"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding CLASSIFIER = new Coding
    {
      Code = "CLASSIFIER",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleCode"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding National = new Coding
    {
      Code = "CN",
      Display = "national",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleCode"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding NonCountryMemberWithoutResidencePermit = new Coding
    {
      Code = "CNRP",
      Display = "non-country member without residence permit",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleCode"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding NonCountryMemberMinorWithoutResidencePermit = new Coding
    {
      Code = "CNRPM",
      Display = "non-country member minor without residence permit",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleCode"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding COAUTH = new Coding
    {
      Code = "COAUTH",
      System = "http://terminology.hl7.org/CodeSystem/contractsignertypecodes"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding CONSENTER = new Coding
    {
      Code = "CONSENTER",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleCode"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding CONSWIT = new Coding
    {
      Code = "CONSWIT",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleCode"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding CONT = new Coding
    {
      Code = "CONT",
      System = "http://terminology.hl7.org/CodeSystem/contractsignertypecodes"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding COPART = new Coding
    {
      Code = "COPART",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleCode"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding COVPTY = new Coding
    {
      Code = "COVPTY",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleClass"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding PermitCardApplicant = new Coding
    {
      Code = "CPCA",
      Display = "permit card applicant",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleCode"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding NonCountryMemberWithResidencePermit = new Coding
    {
      Code = "CRP",
      Display = "non-country member with residence permit",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleCode"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding NonCountryMemberMinorWithResidencePermit = new Coding
    {
      Code = "CRPM",
      Display = "non-country member minor with residence permit",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleCode"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding CST = new Coding
    {
      Code = "CST",
      System = "http://terminology.hl7.org/CodeSystem/v3-ParticipationType"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding DataCollector = new Coding
    {
      Code = "datacollector",
      Display = "Data Collector",
      System = "http://terminology.hl7.org/CodeSystem/extra-security-role-type"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding DataProcessor = new Coding
    {
      Code = "dataprocessor",
      Display = "Data Processor",
      System = "http://terminology.hl7.org/CodeSystem/extra-security-role-type"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding DataSubject = new Coding
    {
      Code = "datasubject",
      Display = "Data Subject",
      System = "http://terminology.hl7.org/CodeSystem/extra-security-role-type"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding DECLASSIFIER = new Coding
    {
      Code = "DECLASSIFIER",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleCode"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding DELEGATEE = new Coding
    {
      Code = "DELEGATEE",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleCode"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding DELEGATOR = new Coding
    {
      Code = "DELEGATOR",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleCode"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding DEPEN = new Coding
    {
      Code = "DEPEN",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleClass"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding DOWNGRDER = new Coding
    {
      Code = "DOWNGRDER",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleCode"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding DPOWATT = new Coding
    {
      Code = "DPOWATT",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleCode"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding ECON = new Coding
    {
      Code = "ECON",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleClass"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding EMP = new Coding
    {
      Code = "EMP",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleClass"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding EVTWIT = new Coding
    {
      Code = "EVTWIT",
      System = "http://terminology.hl7.org/CodeSystem/contractsignertypecodes"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding EXCEST = new Coding
    {
      Code = "EXCEST",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleCode"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding GRANTEE = new Coding
    {
      Code = "GRANTEE",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleCode"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding GRANTOR = new Coding
    {
      Code = "GRANTOR",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleCode"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding GT = new Coding
    {
      Code = "GT",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleCode"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding GUADLTM = new Coding
    {
      Code = "GUADLTM",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleCode"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding GUARD = new Coding
    {
      Code = "GUARD",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleClass"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding HPOWATT = new Coding
    {
      Code = "HPOWATT",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleCode"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding HumanUser = new Coding
    {
      Code = "humanuser",
      Display = "Human User",
      System = "http://terminology.hl7.org/CodeSystem/extra-security-role-type"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding INF = new Coding
    {
      Code = "INF",
      System = "http://terminology.hl7.org/CodeSystem/v3-ParticipationType"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding INTPRTER = new Coding
    {
      Code = "INTPRTER",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleCode"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding INVSBJ = new Coding
    {
      Code = "INVSBJ",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleClass"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding IRCP = new Coding
    {
      Code = "IRCP",
      System = "http://terminology.hl7.org/CodeSystem/v3-ParticipationType"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding LA = new Coding
    {
      Code = "LA",
      System = "http://terminology.hl7.org/CodeSystem/v3-ParticipationType"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding NAMED = new Coding
    {
      Code = "NAMED",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleClass"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding NOK = new Coding
    {
      Code = "NOK",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleClass"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding NOT = new Coding
    {
      Code = "NOT",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleClass"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding PAT = new Coding
    {
      Code = "PAT",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleClass"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding POWATT = new Coding
    {
      Code = "POWATT",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleCode"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding PRIMAUTH = new Coding
    {
      Code = "PRIMAUTH",
      System = "http://terminology.hl7.org/CodeSystem/contractsignertypecodes"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding PROMSK = new Coding
    {
      Code = "PROMSK",
      System = "http://terminology.hl7.org/CodeSystem/v3-ParticipationFunction"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding PROV = new Coding
    {
      Code = "PROV",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleClass"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding RESPRSN = new Coding
    {
      Code = "RESPRSN",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleCode"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding REVIEWER = new Coding
    {
      Code = "REVIEWER",
      System = "http://terminology.hl7.org/CodeSystem/contractsignertypecodes"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding SOURCE = new Coding
    {
      Code = "SOURCE",
      System = "http://terminology.hl7.org/CodeSystem/contractsignertypecodes"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding SPOWATT = new Coding
    {
      Code = "SPOWATT",
      System = "http://terminology.hl7.org/CodeSystem/v3-RoleCode"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding TRANS = new Coding
    {
      Code = "TRANS",
      System = "http://terminology.hl7.org/CodeSystem/contractsignertypecodes"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding TRC = new Coding
    {
      Code = "TRC",
      System = "http://terminology.hl7.org/CodeSystem/v3-ParticipationType"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding VALID = new Coding
    {
      Code = "VALID",
      System = "http://terminology.hl7.org/CodeSystem/contractsignertypecodes"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding VERF = new Coding
    {
      Code = "VERF",
      System = "http://terminology.hl7.org/CodeSystem/contractsignertypecodes"
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly Coding WIT = new Coding
    {
      Code = "WIT",
      System = "http://terminology.hl7.org/CodeSystem/v3-ParticipationType"
    };
  };
}
