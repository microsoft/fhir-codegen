module FHIR

  ##
  # Base StructureDefinition for ParameterDefinition Type: The parameters to the module. This collection specifies both the input and output parameters. Input parameters are provided by the caller as part of the $evaluate operation. Output parameters are included in the GuidanceResponse.
  class ParameterDefinition < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = []
    METADATA = {
      ##
      # Unique id for inter-element referencing
      # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
      'id' => {
        'type'=>'id',
        'path'=>'ParameterDefinition.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'ParameterDefinition.extension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Name used to access the parameter value
      # The name of the parameter used to allow access to the value of the parameter in evaluation contexts.
      'name' => {
        'type'=>'code',
        'path'=>'ParameterDefinition.name',
        'min'=>0,
        'max'=>1
      },
      ##
      # in | out
      # Whether the parameter is input or output for the module.
      'use' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/operation-parameter-use'=>[ 'in', 'out' ]
        },
        'type'=>'code',
        'path'=>'ParameterDefinition.use',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/operation-parameter-use'}
      },
      ##
      # Minimum cardinality
      # The minimum number of times this parameter SHALL appear in the request or response.
      'min' => {
        'type'=>'integer',
        'path'=>'ParameterDefinition.min',
        'min'=>0,
        'max'=>1
      },
      ##
      # Maximum cardinality (a number of *)
      # The maximum number of times this element is permitted to appear in the request or response.
      'max' => {
        'type'=>'string',
        'path'=>'ParameterDefinition.max',
        'min'=>0,
        'max'=>1
      },
      ##
      # A brief description of the parameter
      # A brief discussion of what the parameter is for and how it is used by the module.
      'documentation' => {
        'type'=>'string',
        'path'=>'ParameterDefinition.documentation',
        'min'=>0,
        'max'=>1
      },
      ##
      # What type of value
      # The type of the parameter.
      'type' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/abstract-types'=>[ 'Type', 'Any' ],
          'http://hl7.org/fhir/data-types'=>[ 'Address', 'Age', 'Annotation', 'Attachment', 'BackboneElement', 'CodeableConcept', 'Coding', 'ContactDetail', 'ContactPoint', 'Contributor', 'Count', 'DataRequirement', 'Distance', 'Dosage', 'Duration', 'Element', 'ElementDefinition', 'Expression', 'Extension', 'HumanName', 'Identifier', 'MarketingStatus', 'Meta', 'Money', 'MoneyQuantity', 'Narrative', 'ParameterDefinition', 'Period', 'Population', 'ProdCharacteristic', 'ProductShelfLife', 'Quantity', 'Range', 'Ratio', 'Reference', 'RelatedArtifact', 'SampledData', 'Signature', 'SimpleQuantity', 'SubstanceAmount', 'Timing', 'TriggerDefinition', 'UsageContext', 'base64Binary', 'boolean', 'canonical', 'code', 'date', 'dateTime', 'decimal', 'id', 'instant', 'integer', 'markdown', 'oid', 'positiveInt', 'string', 'time', 'unsignedInt', 'uri', 'url', 'uuid', 'xhtml' ],
          'http://hl7.org/fhir/resource-types'=>[ 'Account', 'ActivityDefinition', 'AdverseEvent', 'AllergyIntolerance', 'Appointment', 'AppointmentResponse', 'AuditEvent', 'Basic', 'Binary', 'BiologicallyDerivedProduct', 'BodyStructure', 'Bundle', 'CapabilityStatement', 'CarePlan', 'CareTeam', 'CatalogEntry', 'ChargeItem', 'ChargeItemDefinition', 'Claim', 'ClaimResponse', 'ClinicalImpression', 'CodeSystem', 'Communication', 'CommunicationRequest', 'CompartmentDefinition', 'Composition', 'ConceptMap', 'Condition', 'Consent', 'Contract', 'Coverage', 'CoverageEligibilityRequest', 'CoverageEligibilityResponse', 'DetectedIssue', 'Device', 'DeviceDefinition', 'DeviceMetric', 'DeviceRequest', 'DeviceUseStatement', 'DiagnosticReport', 'DocumentManifest', 'DocumentReference', 'DomainResource', 'EffectEvidenceSynthesis', 'Encounter', 'Endpoint', 'EnrollmentRequest', 'EnrollmentResponse', 'EpisodeOfCare', 'EventDefinition', 'Evidence', 'EvidenceVariable', 'ExampleScenario', 'ExplanationOfBenefit', 'FamilyMemberHistory', 'Flag', 'Goal', 'GraphDefinition', 'Group', 'GuidanceResponse', 'HealthcareService', 'ImagingStudy', 'Immunization', 'ImmunizationEvaluation', 'ImmunizationRecommendation', 'ImplementationGuide', 'InsurancePlan', 'Invoice', 'Library', 'Linkage', 'List', 'Location', 'Measure', 'MeasureReport', 'Media', 'Medication', 'MedicationAdministration', 'MedicationDispense', 'MedicationKnowledge', 'MedicationRequest', 'MedicationStatement', 'MedicinalProduct', 'MedicinalProductAuthorization', 'MedicinalProductContraindication', 'MedicinalProductIndication', 'MedicinalProductIngredient', 'MedicinalProductInteraction', 'MedicinalProductManufactured', 'MedicinalProductPackaged', 'MedicinalProductPharmaceutical', 'MedicinalProductUndesirableEffect', 'MessageDefinition', 'MessageHeader', 'MolecularSequence', 'NamingSystem', 'NutritionOrder', 'Observation', 'ObservationDefinition', 'OperationDefinition', 'OperationOutcome', 'Organization', 'OrganizationAffiliation', 'Parameters', 'Patient', 'PaymentNotice', 'PaymentReconciliation', 'Person', 'PlanDefinition', 'Practitioner', 'PractitionerRole', 'Procedure', 'Provenance', 'Questionnaire', 'QuestionnaireResponse', 'RelatedPerson', 'RequestGroup', 'ResearchDefinition', 'ResearchElementDefinition', 'ResearchStudy', 'ResearchSubject', 'Resource', 'RiskAssessment', 'RiskEvidenceSynthesis', 'Schedule', 'SearchParameter', 'ServiceRequest', 'Slot', 'Specimen', 'SpecimenDefinition', 'StructureDefinition', 'StructureMap', 'Subscription', 'Substance', 'SubstanceNucleicAcid', 'SubstancePolymer', 'SubstanceProtein', 'SubstanceReferenceInformation', 'SubstanceSourceMaterial', 'SubstanceSpecification', 'SupplyDelivery', 'SupplyRequest', 'Task', 'TerminologyCapabilities', 'TestReport', 'TestScript', 'ValueSet', 'VerificationResult', 'VisionPrescription' ]
        },
        'type'=>'code',
        'path'=>'ParameterDefinition.type',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/all-types'}
      },
      ##
      # What profile the value is expected to be
      # If specified, this indicates a profile that the input data must conform to, or that the output data will conform to.
      'profile' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/StructureDefinition'],
        'type'=>'canonical',
        'path'=>'ParameterDefinition.profile',
        'min'=>0,
        'max'=>1
      }
    }
    ##
    # Unique id for inter-element referencing
    # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
    attr_accessor :id                             # 0-1 id
    ##
    # Additional content defined by implementations
    # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
    # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
    attr_accessor :extension                      # 0-* [ Extension ]
    ##
    # Name used to access the parameter value
    # The name of the parameter used to allow access to the value of the parameter in evaluation contexts.
    attr_accessor :name                           # 0-1 code
    ##
    # in | out
    # Whether the parameter is input or output for the module.
    attr_accessor :use                            # 1-1 code
    ##
    # Minimum cardinality
    # The minimum number of times this parameter SHALL appear in the request or response.
    attr_accessor :min                            # 0-1 integer
    ##
    # Maximum cardinality (a number of *)
    # The maximum number of times this element is permitted to appear in the request or response.
    attr_accessor :max                            # 0-1 string
    ##
    # A brief description of the parameter
    # A brief discussion of what the parameter is for and how it is used by the module.
    attr_accessor :documentation                  # 0-1 string
    ##
    # What type of value
    # The type of the parameter.
    attr_accessor :type                           # 1-1 code
    ##
    # What profile the value is expected to be
    # If specified, this indicates a profile that the input data must conform to, or that the output data will conform to.
    attr_accessor :profile                        # 0-1 canonical(http://hl7.org/fhir/StructureDefinition/StructureDefinition)
  end
end
