RuleSet: CSCodePathProperty
* property[+].code          = #cql-primary-code-path
* property[=].type          = #Coding
* property[=].description   = "The primary code path for the CQL representation of this FHIR type."

RuleSet: ConceptWithCodePath(typeCode, appliedSystem, primaryCodePath)
* concept[+].code                           = {typeCode}
* concept[=].property[+].code               = #cql-primary-code-path
* concept[=].property[=].valueCoding.system = {appliedSystem}
* concept[=].property[=].valueCoding.code   = {primaryCodePath}

RuleSet: CSUsageContext(packageDirective)
* useContext[+].code.system = "http://ginoc.io/cql/UsageContext"
* useContext[=].code.code   = #fhirPackageDirective
* useContext[=].valueCodeableConcept[+].coding[+].code  = {packageDirective}

Instance:       CqlFhirCodePathsR4
InstanceOf:     CodeSystem
Title:          "CQL Primary Code Paths for FHIR R4 Resources"
Description:    "Supplement to R4 FHIR-Types for CQL primary code path properties."
Usage:          #definition
* id                = "cql-fhir-primary-code-paths-r4"
* url               = "http://ginoc.io/cql/cql-fhir-primary-code-paths-r4"
* status            = #active
* content           = #supplement
* supplements       = "http://hl7.org/fhir/fhir-types|4.0.1"
* insert CSUsageContext(#hl7.fhir.r4.core@4.0.1)
* insert CSCodePathProperty
* insert ConceptWithCodePath(#Account,                  "http://hl7.org/fhir/StructureDefinition/Account",                  #type)
* insert ConceptWithCodePath(#ActivityDefinition,       "http://hl7.org/fhir/StructureDefinition/ActivityDefinition",       #topic)
* insert ConceptWithCodePath(#AdverseEvent,             "http://hl7.org/fhir/StructureDefinition/AdverseEvent",             #event)
* insert ConceptWithCodePath(#AllergyIntolerance,       "http://hl7.org/fhir/StructureDefinition/AllergyIntolerance",       #code)
* insert ConceptWithCodePath(#Appointment,              "http://hl7.org/fhir/StructureDefinition/Appointment",              #serviceType)
* insert ConceptWithCodePath(#Basic,                    "http://hl7.org/fhir/StructureDefinition/Basic",                    #code)
* insert ConceptWithCodePath(#BodyStructure,            "http://hl7.org/fhir/StructureDefinition/BodyStructure",            #location)
* insert ConceptWithCodePath(#CarePlan,                 "http://hl7.org/fhir/StructureDefinition/CarePlan",                 #category)
* insert ConceptWithCodePath(#CareTeam,                 "http://hl7.org/fhir/StructureDefinition/CareTeam",                 #category)
* insert ConceptWithCodePath(#ChargeItemDefinition,     "http://hl7.org/fhir/StructureDefinition/ChargeItemDefinition",     #code)
* insert ConceptWithCodePath(#Claim,                    "http://hl7.org/fhir/StructureDefinition/Claim",                    #type)
* insert ConceptWithCodePath(#ClinicalImpression,       "http://hl7.org/fhir/StructureDefinition/ClinicalImpression",       #code)
* insert ConceptWithCodePath(#Communication,            "http://hl7.org/fhir/StructureDefinition/Communication",            #reasonCode)
* insert ConceptWithCodePath(#CommunicationRequest,     "http://hl7.org/fhir/StructureDefinition/CommunicationRequest",     #reasonCode)
* insert ConceptWithCodePath(#Composition,              "http://hl7.org/fhir/StructureDefinition/Composition",              #type)
* insert ConceptWithCodePath(#Condition,                "http://hl7.org/fhir/StructureDefinition/Condition",                #code)
* insert ConceptWithCodePath(#Consent,                  "http://hl7.org/fhir/StructureDefinition/Consent",                  #category)
* insert ConceptWithCodePath(#Coverage,                 "http://hl7.org/fhir/StructureDefinition/Coverage",                 #type)
* insert ConceptWithCodePath(#DetectedIssue,            "http://hl7.org/fhir/StructureDefinition/DetectedIssue",            #code)
* insert ConceptWithCodePath(#Device,                   "http://hl7.org/fhir/StructureDefinition/Device",                   #type)
* insert ConceptWithCodePath(#DeviceMetric,             "http://hl7.org/fhir/StructureDefinition/DeviceMetric",             #type)
* insert ConceptWithCodePath(#DeviceRequest,            "http://hl7.org/fhir/StructureDefinition/DeviceRequest",            #code)
* insert ConceptWithCodePath(#DeviceUseStatement,       "http://hl7.org/fhir/StructureDefinition/DeviceUseStatement",       #device.code)
* insert ConceptWithCodePath(#DiagnosticReport,         "http://hl7.org/fhir/StructureDefinition/DiagnosticReport",         #code)
* insert ConceptWithCodePath(#Encounter,                "http://hl7.org/fhir/StructureDefinition/Encounter",                #type)
* insert ConceptWithCodePath(#EpisodeOfCare,            "http://hl7.org/fhir/StructureDefinition/EpisodeOfCare",            #type)
* insert ConceptWithCodePath(#ExplanationOfBenefit,     "http://hl7.org/fhir/StructureDefinition/ExplanationOfBenefit",     #type)
* insert ConceptWithCodePath(#Flag,                     "http://hl7.org/fhir/StructureDefinition/Flag",                     #code)
* insert ConceptWithCodePath(#Goal,                     "http://hl7.org/fhir/StructureDefinition/Goal",                     #category)
* insert ConceptWithCodePath(#GuidanceResponse,         "http://hl7.org/fhir/StructureDefinition/GuidanceResponse",         #module)
* insert ConceptWithCodePath(#HealthcareService,        "http://hl7.org/fhir/StructureDefinition/HealthcareService",        #type)
* insert ConceptWithCodePath(#Immunization,             "http://hl7.org/fhir/StructureDefinition/Immunization",             #vaccineCode)
* insert ConceptWithCodePath(#Library,                  "http://hl7.org/fhir/StructureDefinition/Library",                  #topic)
* insert ConceptWithCodePath(#Location,                 "http://hl7.org/fhir/StructureDefinition/Location",                 #type)
* insert ConceptWithCodePath(#Measure,                  "http://hl7.org/fhir/StructureDefinition/Measure",                  #topic)
* insert ConceptWithCodePath(#MeasureReport,            "http://hl7.org/fhir/StructureDefinition/MeasureReport",            #type)
* insert ConceptWithCodePath(#Medication,               "http://hl7.org/fhir/StructureDefinition/Medication",               #code)
* insert ConceptWithCodePath(#MedicationAdministration, "http://hl7.org/fhir/StructureDefinition/MedicationAdministration", #medication)
* insert ConceptWithCodePath(#MedicationDispense,       "http://hl7.org/fhir/StructureDefinition/MedicationDispense",       #medication)
* insert ConceptWithCodePath(#MedicationRequest,        "http://hl7.org/fhir/StructureDefinition/MedicationRequest",        #medication)
* insert ConceptWithCodePath(#MedicationStatement,      "http://hl7.org/fhir/StructureDefinition/MedicationStatement",      #medication)
* insert ConceptWithCodePath(#MessageDefinition,        "http://hl7.org/fhir/StructureDefinition/MessageDefinition",        #event)
* insert ConceptWithCodePath(#Observation,              "http://hl7.org/fhir/StructureDefinition/Observation",              #code)
* insert ConceptWithCodePath(#OperationOutcome,         "http://hl7.org/fhir/StructureDefinition/OperationOutcome",         #issue.code)
* insert ConceptWithCodePath(#PractitionerRole,         "http://hl7.org/fhir/StructureDefinition/PractitionerRole",         #code)
* insert ConceptWithCodePath(#Procedure,                "http://hl7.org/fhir/StructureDefinition/Procedure",                #code)
* insert ConceptWithCodePath(#Questionnaire,            "http://hl7.org/fhir/StructureDefinition/Questionnaire",            #name)
* insert ConceptWithCodePath(#RelatedPerson,            "http://hl7.org/fhir/StructureDefinition/RelatedPerson",            #relationship)
* insert ConceptWithCodePath(#RiskAssessment,           "http://hl7.org/fhir/StructureDefinition/RiskAssessment",           #code)
* insert ConceptWithCodePath(#SearchParameter,          "http://hl7.org/fhir/StructureDefinition/SearchParameter",          #target)
* insert ConceptWithCodePath(#ServiceRequest,           "http://hl7.org/fhir/StructureDefinition/ServiceRequest",           #code)
* insert ConceptWithCodePath(#Specimen,                 "http://hl7.org/fhir/StructureDefinition/Specimen",                 #type)
* insert ConceptWithCodePath(#Substance,                "http://hl7.org/fhir/StructureDefinition/Substance",                #code)
* insert ConceptWithCodePath(#SupplyDelivery,           "http://hl7.org/fhir/StructureDefinition/SupplyDelivery",           #type)
* insert ConceptWithCodePath(#SupplyRequest,            "http://hl7.org/fhir/StructureDefinition/SupplyRequest",            #category)
* insert ConceptWithCodePath(#Task,                     "http://hl7.org/fhir/StructureDefinition/Task",                     #code)
