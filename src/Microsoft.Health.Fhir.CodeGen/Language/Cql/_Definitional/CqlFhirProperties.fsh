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

RuleSet: ModelProperties(patientClassName, patientBirthDateProperty)
* parameter[+].name = "modelProperties"
* parameter[=].part[+].name         = "patientClassName"
* parameter[=].part[=].valueString  = {patientClassName}
* parameter[=].part[+].name         = "patientBirthDatePropertyName"
* parameter[=].part[=].valueString  = {patientBirthDateProperty}

RuleSet: StructureProperties(canonical, path)
* parameter[+].name = {canonical}
* parameter[=].part[+].name             = "primaryCodePath"
* parameter[=].part[=].valueString      = {path}


Instance:       CqlFhirPropertiesR4
InstanceOf:     Parameters
Title:          "CQL Primary Code Paths for FHIR R4 Resources"
Description:    "CQL primary code path properties for core FHIR R4 resources."
Usage:          #definition
* id = "cql-hl7-fhir-r4-core"
* insert ContextWithVersion("hl7.fhir.r4.core", "4.0.1")
* insert ModelProperties("FHIR.Patient", "birthDate.value")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Account",                  "type")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/ActivityDefinition",       "topic")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/AdverseEvent",             "event")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/AllergyIntolerance",       "code")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Appointment",              "serviceType")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Basic",                    "code")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/BodyStructure",            "location")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/CarePlan",                 "category")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/CareTeam",                 "category")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/ChargeItemDefinition",     "code")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Claim",                    "type")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/ClinicalImpression",       "code")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Communication",            "reasonCode")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/CommunicationRequest",     "reasonCode")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Composition",              "type")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Condition",                "code")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Consent",                  "category")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Coverage",                 "type")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/DetectedIssue",            "code")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Device",                   "type")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/DeviceMetric",             "type")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/DeviceRequest",            "code")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/DeviceUseStatement",       "device.code")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/DiagnosticReport",         "code")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Encounter",                "type")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/EpisodeOfCare",            "type")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/ExplanationOfBenefit",     "type")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Flag",                     "code")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Goal",                     "category")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/GuidanceResponse",         "module")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/HealthcareService",        "type")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Immunization",             "vaccineCode")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Library",                  "topic")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Location",                 "type")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Measure",                  "topic")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/MeasureReport",            "type")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Medication",               "code")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/MedicationAdministration", "medication")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/MedicationDispense",       "medication")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/MedicationRequest",        "medication")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/MedicationStatement",      "medication")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/MessageDefinition",        "event")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Observation",              "code")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/OperationOutcome",         "issue.code")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/PractitionerRole",         "code")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Procedure",                "code")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Questionnaire",            "name")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/RelatedPerson",            "relationship")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/RiskAssessment",           "code")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/SearchParameter",          "target")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/ServiceRequest",           "code")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Specimen",                 "type")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Substance",                "code")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/SupplyDelivery",           "type")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/SupplyRequest",            "category")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Task",                     "code")


Instance:       CqlFhirPropertiesUsCore
InstanceOf:     Parameters
Title:          "CQL Primary Code Paths for US Core"
Description:    "CQL primary code path properties for US Core profiles and changes for R4 Core resources."
Usage:          #definition
* id = "cql-hl7-fhir-us-core"
* insert Context("hl7.fhir.us.core")
* insert SupersedesWithVersion("hl7.fhir.r4.core", "4.0.1")
* insert ModelProperties("PatientProfile", "birthDate")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/ActivityDefinition",         "topic")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/AdverseEvent",               "type")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/AllergyIntolerance",         "code")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Appointment",                "serviceType")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Basic",                      "code")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/CarePlan",                   "category")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/CareTeam",                   "category")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/ChargeItemDefinition",       "code")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Claim",                      "type")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/ClinicalImpression",         "code")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Communication",              "category")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/CommunicationRequest",       "category")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Composition",                "type")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Condition",                  "code")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Consent",                    "category")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Coverage",                   "type")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/DetectedIssue",              "category")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Device",                     "type")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/DeviceMetric",               "type")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/DeviceRequest",              "codeCodeableConcept")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/DeviceUseStatement",         "device.code")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/DiagnosticReport",           "code")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Encounter",                  "type")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/EpisodeOfCare",              "type")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/ExplanationOfBenefit",       "type")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Flag",                       "code")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Goal",                       "category")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/GuidanceResponse",           "module")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/HealthcareService",          "type")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Immunization",               "vaccineCode")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Library",                    "topic")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Measure",                    "topic")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/MeasureReport",              "measure.topic")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Medication",                 "code")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/MedicationAdministration",   "medication")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/MedicationDispense",         "medication")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/MedicationRequest",          "medication")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/MedicationStatement",        "medication")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/MessageDefinition",          "event")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Observation",                "code")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/OperationOutcome",           "issue.code")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Procedure",                  "code")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/ProcedureRequest",           "code")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Questionnaire",              "name")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/ReferralRequest",            "type")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/RiskAssessment",             "code")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/SearchParameter",            "target")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Sequence",                   "type")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Specimen",                   "type")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Substance",                  "code")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/SupplyDelivery",             "type")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/SupplyRequest",              "category")
* insert StructureProperties("http://hl7.org/fhir/StructureDefinition/Task",                       "code")


Instance:       CqlFhirCodePathsQICore
InstanceOf:     Parameters
Title:          "CQL Primary Code Paths for QI Core"
Description:    "CQL primary code path properties for QI Core profiles and changes for R4 Core resources and US Core Profiles."
Usage:          #definition
* id = "cql-hl7-fhir-us-qicore"
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
