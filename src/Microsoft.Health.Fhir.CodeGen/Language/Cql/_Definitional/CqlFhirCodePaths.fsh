RuleSet: Context(packageId)
* parameter[+].name = "context"
* parameter[=].part[+].name         = "packageId"
* parameter[=].part[=].valueString  = {packageId}

RuleSet: ContextWithVersion(packageId, packageVersion)
* parameter[+].name = "context"
* parameter[=].part[+].name         = "packageId"
* parameter[=].part[=].valueString  = {packageId}
* parameter[=].part[+].name         = "packageVersion"
* parameter[=].part[=].valueString  = {packageVersion}

RuleSet: Supersedes(packageId)
* parameter[+].name = "supersedes"
* parameter[=].part[+].name         = "packageId"
* parameter[=].part[=].valueString  = {packageId}

RuleSet: SupersedesWithVersion(packageId, packageVersion)
* parameter[+].name = "supersedes"
* parameter[=].part[+].name         = "packageId"
* parameter[=].part[=].valueString  = {packageId}
* parameter[=].part[+].name         = "packageVersion"
* parameter[=].part[=].valueString  = {packageVersion}

RuleSet: CodePath(canonical, path)
* parameter[+].name = {canonical}
* parameter[=].part[+].name             = "primaryCodePath"
* parameter[=].part[=].valueString      = {path}


Instance:       CqlFhirPropertiesR4
InstanceOf:     Parameters
Title:          "CQL Primary Code Paths for FHIR R4 Resources"
Description:    "CQL primary code path properties for core FHIR R4 resources."
Usage:          #definition
* id = "cql-fhir-properties-r4"
* insert ContextWithVersion("hl7.fhir.r4.core", "4.0.1")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Account",                  "type")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/ActivityDefinition",       "topic")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/AdverseEvent",             "event")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/AllergyIntolerance",       "code")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Appointment",              "serviceType")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Basic",                    "code")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/BodyStructure",            "location")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/CarePlan",                 "category")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/CareTeam",                 "category")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/ChargeItemDefinition",     "code")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Claim",                    "type")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/ClinicalImpression",       "code")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Communication",            "reasonCode")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/CommunicationRequest",     "reasonCode")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Composition",              "type")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Condition",                "code")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Consent",                  "category")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Coverage",                 "type")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/DetectedIssue",            "code")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Device",                   "type")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/DeviceMetric",             "type")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/DeviceRequest",            "code")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/DeviceUseStatement",       "device.code")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/DiagnosticReport",         "code")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Encounter",                "type")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/EpisodeOfCare",            "type")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/ExplanationOfBenefit",     "type")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Flag",                     "code")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Goal",                     "category")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/GuidanceResponse",         "module")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/HealthcareService",        "type")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Immunization",             "vaccineCode")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Library",                  "topic")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Location",                 "type")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Measure",                  "topic")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/MeasureReport",            "type")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Medication",               "code")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/MedicationAdministration", "medication")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/MedicationDispense",       "medication")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/MedicationRequest",        "medication")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/MedicationStatement",      "medication")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/MessageDefinition",        "event")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Observation",              "code")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/OperationOutcome",         "issue.code")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/PractitionerRole",         "code")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Procedure",                "code")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Questionnaire",            "name")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/RelatedPerson",            "relationship")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/RiskAssessment",           "code")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/SearchParameter",          "target")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/ServiceRequest",           "code")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Specimen",                 "type")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Substance",                "code")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/SupplyDelivery",           "type")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/SupplyRequest",            "category")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Task",                     "code")


Instance:       CqlFhirPropertiesUsCore
InstanceOf:     Parameters
Title:          "CQL Primary Code Paths for US Core"
Description:    "CQL primary code path properties for US Core profiles and changes for R4 Core resources."
Usage:          #definition
* id = "cql-fhir-properties-us-core"
* insert Context("hl7.fhir.us.core")
* insert SupersedesWithVersion("hl7.fhir.r4.core", "4.0.1")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/ActivityDefinition",         "topic")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/AdverseEvent",               "type")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/AllergyIntolerance",         "code")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Appointment",                "serviceType")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Basic",                      "code")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/CarePlan",                   "category")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/CareTeam",                   "category")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/ChargeItemDefinition",       "code")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Claim",                      "type")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/ClinicalImpression",         "code")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Communication",              "category")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/CommunicationRequest",       "category")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Composition",                "type")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Condition",                  "code")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Consent",                    "category")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Coverage",                   "type")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/DetectedIssue",              "category")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Device",                     "type")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/DeviceMetric",               "type")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/DeviceRequest",              "codeCodeableConcept")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/DeviceUseStatement",         "device.code")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/DiagnosticReport",           "code")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Encounter",                  "type")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/EpisodeOfCare",              "type")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/ExplanationOfBenefit",       "type")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Flag",                       "code")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Goal",                       "category")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/GuidanceResponse",           "module")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/HealthcareService",          "type")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Immunization",               "vaccineCode")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Library",                    "topic")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Measure",                    "topic")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/MeasureReport",              "measure.topic")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Medication",                 "code")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/MedicationAdministration",   "medication")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/MedicationDispense",         "medication")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/MedicationRequest",          "medication")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/MedicationStatement",        "medication")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/MessageDefinition",          "event")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Observation",                "code")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/OperationOutcome",           "issue.code")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Procedure",                  "code")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/ProcedureRequest",           "code")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Questionnaire",              "name")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/ReferralRequest",            "type")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/RiskAssessment",             "code")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/SearchParameter",            "target")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Sequence",                   "type")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Specimen",                   "type")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Substance",                  "code")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/SupplyDelivery",             "type")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/SupplyRequest",              "category")
* insert CodePath("http://hl7.org/fhir/StructureDefinition/Task",                       "code")


Instance:       CqlFhirCodePathsQICore
InstanceOf:     Parameters
Title:          "CQL Primary Code Paths for QI Core"
Description:    "CQL primary code path properties for QI Core profiles and changes for R4 Core resources and US Core Profiles."
Usage:          #definition
* id = "cql-fhir-properties-qi-core"
* insert Context("hl7.fhir.us.qicore")
* insert Supersedes("hl7.fhir.us.core")
* insert SupersedesWithVersion("hl7.fhir.r4.core", "4.0.1")

                // put("Practitioner", ""); // Not clear what the primary code path should be...
                // put("QuestionnaireResponse", ""); // Not clear what the primary code path should be...
                // put("USCoreLaboratoryResultObservationProfile", "code"); // v4.1.1

                put("ActivityDefinition", "topic");
                put("AdverseEvent", "event");
                put("AllergyIntolerance", "code");
                put("Appointment", "serviceType");
                put("Basic", "code");
                put("BodyStructure", "location");
                put("CarePlan", "category");
                put("CareTeam", "category");
                put("ChargeItemDefinition", "code");
                put("Claim", "type");
                put("ClinicalImpression", "code");
                put("Communication", "topic");
                put("CommunicationNotDone", "topic");
                put("CommunicationRequest", "category");
                put("Composition", "type");
                put("Condition", "code");
                put("ConditionEncounterDiagnosis", "code");
                put("ConditionProblemsHealthConcerns", "code");
                put("Consent", "category");
                put("Coverage", "type");
                put("DetectedIssue", "category");
                put("Device", "type");
                put("DeviceMetric", "type");
                put("DeviceRequest", "code");
                put("DeviceNotRequested", "code");
                put("DeviceUseStatement", "device.type");
                put("DiagnosticReport", "code");
                put("DiagnosticReportLab", "code");
                put("DiagnosticReportNote", "code");
                put("Encounter", "type");
                put("EpisodeOfCare", "type");
                put("ExplanationOfBenefit", "type");
                put("Flag", "code");
                put("FamilyMemberHistory", "relationship");
                put("Goal", "category");
                put("GuidanceResponse", "module");
                put("HealthcareService", "type");
                put("ImagingStudy", "procedureCode");
                put("Immunization", "vaccineCode");
                put("ImmunizationEvaluation", "targetDisease");
                put("ImmunizationNotDone", "vaccineCode");
                put("ImmunizationRecommendation", "recommendation.vaccineCode");
                put("LaboratoryResultObservation", "code");
                put("Location", "type");
                put("Library", "topic");
                put("Measure", "topic");
                put("MeasureReport", "measure.topic");
                put("Medication", "code");
                put("MedicationAdministration", "medication");
                put("MedicationAdministrationNotDone", "medication");
                put("MedicationDispense", "medication");
                put("MedicationDispenseNotDone", "medication");
                put("MedicationDispenseDeclined", "medication");
                put("MedicationRequest", "medication");
                put("MedicationNotRequested", "medication");
                put("MedicationStatement", "medication");
                put("MessageDefinition", "event");
                put("Observation", "code");
                put("ObservationClinicalTestResult", "code");
                put("ObservationImagingResult", "code");
                put("ObservationSurvey", "code");
                put("ObservationNotDone", "code"); // v4.1.1
                put("ObservationCancelled", "code"); // v5.0.0
                put("OperationOutcome", "issue.code");
                put("Organization", "type");
                put("PractitionerRole", "code");
                put("Procedure", "code");
                put("ProcedureNotDone", "code");
                put("ProcedureRequest", "code");
                put("Questionnaire", "name");
                put("ServiceRequest", "code");
                put("ServiceNotRequested", "code");
                put("RelatedPerson", "relationship");
                put("RiskAssessment", "code");
                put("SearchParameter", "target");
                put("Sequence", "type");
                put("Specimen", "type");
                put("Substance", "code");
                put("SupplyDelivery", "type");
                put("SupplyRequest", "category");
                put("Task", "code");
                put("TaskNotDone", "code");
                put("TaskRejected", "code");
                put("USCoreImplantableDeviceProfile", "type");
                put("USCorePediatricBMIforAgeObservationProfile", "code");
                put("USCorePediatricWeightForHeightObservationProfile", "code");
                put("USCoreObservationSexualOrientationProfile", "code"); // v5.0.0
                put("USCoreObservationSocialHistoryProfile", "code"); // v5.0.0
                put("USCoreObservationSDOHAssessment", "code"); // v5.0.0
                put("USCorePediatricHeadOccipitalFrontalCircumferencePercentileProfile", "code"); // v5.0.0
                put("USCorePulseOximetryProfile", "code");
                put("USCoreSmokingStatusProfile", "code");
                put("observation-bmi", "code");
                put("USCoreBMIProfile", "code");
                put("observation-bodyheight", "code");
                put("USCoreBodyHeightProfile", "code");
                put("observation-bodytemp", "code");
                put("USCoreBodyTemperatureProfile", "code");
                put("observation-bodyweight", "code");
                put("USCoreBodyWeightProfile", "code");
                put("observation-bp", "code");
                put("USCoreBloodPressureProfile", "code");
                put("observation-headcircum", "code");
                put("USCoreHeadCircumferenceProfile", "code");
                put("observation-heartrate", "code");
                put("USCoreHeartRateProfile", "code");
                put("observation-oxygensat", "code");
                put("USCoreOxygenSaturationProfile", "code");
                put("observation-resprate", "code");
                put("USCoreRespiratoryRateProfile", "code");
                put("observation-vitalspanel", "code");
                put("USCoreVitalSignsProfile", "code");
