module FHIR
  PRIMITIVES = {
    ##
    # Base StructureDefinition for base64Binary Type: A stream of bytes
    'base64Binary' => {'type'=>'string', 'regex'=>'(\\s*([0-9a-zA-Z\\+/=]){4}\\s*)+' },
    ##
    # Base StructureDefinition for boolean Type: Value of "true" or "false"
    'boolean' => {'type'=>'boolean', 'regex'=>'true|false' },
    ##
    # Base StructureDefinition for canonical type: A URI that is a reference to a canonical URL on a FHIR resource
    'canonical' => {'type'=>'string', 'regex'=>'\\S*' },
    ##
    # Base StructureDefinition for code type: A string which has at least one character and no leading or trailing whitespace and where there is no whitespace other than single spaces in the contents
    'code' => {'type'=>'string', 'regex'=>'[^\\s]+(\\s[^\\s]+)*' },
    ##
    # Base StructureDefinition for date Type: A date or partial date (e.g. just year or year + month). There is no time zone. The format is a union of the schema types gYear, gYearMonth and date.  Dates SHALL be valid dates.
    'date' => {'type'=>'date', 'regex'=>'([0-9]([0-9]([0-9][1-9]|[1-9]0)|[1-9]00)|[1-9]000)(-(0[1-9]|1[0-2])(-(0[1-9]|[1-2][0-9]|3[0-1]))?)?' },
    ##
    # Base StructureDefinition for dateTime Type: A date, date-time or partial date (e.g. just year or year + month).  If hours and minutes are specified, a time zone SHALL be populated. The format is a union of the schema types gYear, gYearMonth, date and dateTime. Seconds must be provided due to schema type constraints but may be zero-filled and may be ignored.                 Dates SHALL be valid dates.
    'dateTime' => {'type'=>'datetime', 'regex'=>'([0-9]([0-9]([0-9][1-9]|[1-9]0)|[1-9]00)|[1-9]000)(-(0[1-9]|1[0-2])(-(0[1-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:([0-5][0-9]|60)(\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?' },
    ##
    # Base StructureDefinition for decimal Type: A rational number with implicit precision
    'decimal' => {'type'=>'decimal', 'regex'=>'-?(0|[1-9][0-9]*)(\\.[0-9]+)?([eE][+-]?[0-9]+)?' },
    ##
    # Base StructureDefinition for id type: Any combination of letters, numerals, "-" and ".", with a length limit of 64 characters.  (This might be an integer, an unprefixed OID, UUID or any other identifier pattern that meets these constraints.)  Ids are case-insensitive.
    'id' => {'type'=>'string', 'regex'=>'[A-Za-z0-9\\-\\.]{1,64}' },
    ##
    # Base StructureDefinition for instant Type: An instant in time - known at least to the second
    'instant' => {'type'=>'datetime', 'regex'=>'([0-9]([0-9]([0-9][1-9]|[1-9]0)|[1-9]00)|[1-9]000)-(0[1-9]|1[0-2])-(0[1-9]|[1-2][0-9]|3[0-1])T([01][0-9]|2[0-3]):[0-5][0-9]:([0-5][0-9]|60)(\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00))' },
    ##
    # Base StructureDefinition for integer Type: A whole number
    'integer' => {'type'=>'integer', 'regex'=>'-?([0]|([1-9][0-9]*))' },
    ##
    # Base StructureDefinition for markdown type: A string that may contain Github Flavored Markdown syntax for optional processing by a mark down presentation engine
    'markdown' => {'type'=>'string', 'regex'=>'[ \\r\\n\\t\\S]+' },
    ##
    # Base StructureDefinition for oid type: An OID represented as a URI
    'oid' => {'type'=>'string', 'regex'=>'urn:oid:[0-2](\\.(0|[1-9][0-9]*))+' },
    ##
    # Base StructureDefinition for positiveInt type: An integer with a value that is positive (e.g. >0)
    'positiveInt' => {'type'=>'string', 'regex'=>'[1-9][0-9]*' },
    ##
    # Base StructureDefinition for string Type: A sequence of Unicode characters
    'string' => {'type'=>'string', 'regex'=>'[ \\r\\n\\t\\S]+' },
    ##
    # Base StructureDefinition for time Type: A time during the day, with no date specified
    'time' => {'type'=>'time', 'regex'=>'([01][0-9]|2[0-3]):[0-5][0-9]:([0-5][0-9]|60)(\\.[0-9]+)?' },
    ##
    # Base StructureDefinition for unsignedInt type: An integer with a value that is not negative (e.g. >= 0)
    'unsignedInt' => {'type'=>'string', 'regex'=>'[0]|([1-9][0-9]*)' },
    ##
    # Base StructureDefinition for uri Type: String of characters used to identify a name or a resource
    'uri' => {'type'=>'string', 'regex'=>'\\S*' },
    ##
    # Base StructureDefinition for url type: A URI that is a literal reference
    'url' => {'type'=>'string', 'regex'=>'\\S*' },
    ##
    # Base StructureDefinition for uuid type: A UUID, represented as a URI
    'uuid' => {'type'=>'string', 'regex'=>'urn:uuid:[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}' },
    ##
    # Base StructureDefinition for xhtml Type
    'xhtml' => {'type'=>'string' },
  }
  TYPES = [ 'Element', 'BackboneElement', 'Address', 'Age', 'Annotation', 'Attachment', 'CodeableConcept', 'Coding', 'ContactDetail', 'ContactPoint', 'Contributor', 'Count', 'DataRequirement', 'Distance', 'Dosage', 'Duration', 'ElementDefinition', 'Expression', 'Extension', 'HumanName', 'Identifier', 'MarketingStatus', 'Meta', 'Money', 'Narrative', 'ParameterDefinition', 'Period', 'Population', 'ProdCharacteristic', 'ProductShelfLife', 'Quantity', 'Range', 'Ratio', 'Reference', 'RelatedArtifact', 'SampledData', 'Signature', 'SubstanceAmount', 'Timing', 'TriggerDefinition', 'UsageContext' ]
  RESOURCES = [ 'Resource', 'Account', 'ActivityDefinition', 'AdverseEvent', 'AllergyIntolerance', 'Appointment', 'AppointmentResponse', 'AuditEvent', 'Basic', 'Binary', 'BiologicallyDerivedProduct', 'BodyStructure', 'Bundle', 'CapabilityStatement', 'CarePlan', 'CareTeam', 'CatalogEntry', 'ChargeItem', 'ChargeItemDefinition', 'Claim', 'ClaimResponse', 'ClinicalImpression', 'CodeSystem', 'Communication', 'CommunicationRequest', 'CompartmentDefinition', 'Composition', 'ConceptMap', 'Condition', 'Consent', 'Contract', 'Coverage', 'CoverageEligibilityRequest', 'CoverageEligibilityResponse', 'DetectedIssue', 'Device', 'DeviceDefinition', 'DeviceMetric', 'DeviceRequest', 'DeviceUseStatement', 'DiagnosticReport', 'DocumentManifest', 'DocumentReference', 'DomainResource', 'EffectEvidenceSynthesis', 'Encounter', 'Endpoint', 'EnrollmentRequest', 'EnrollmentResponse', 'EpisodeOfCare', 'EventDefinition', 'Evidence', 'EvidenceVariable', 'ExampleScenario', 'ExplanationOfBenefit', 'FamilyMemberHistory', 'Flag', 'Goal', 'GraphDefinition', 'Group', 'GuidanceResponse', 'HealthcareService', 'ImagingStudy', 'Immunization', 'ImmunizationEvaluation', 'ImmunizationRecommendation', 'ImplementationGuide', 'InsurancePlan', 'Invoice', 'Library', 'Linkage', 'List', 'Location', 'Measure', 'MeasureReport', 'Media', 'Medication', 'MedicationAdministration', 'MedicationDispense', 'MedicationKnowledge', 'MedicationRequest', 'MedicationStatement', 'MedicinalProduct', 'MedicinalProductAuthorization', 'MedicinalProductContraindication', 'MedicinalProductIndication', 'MedicinalProductIngredient', 'MedicinalProductInteraction', 'MedicinalProductManufactured', 'MedicinalProductPackaged', 'MedicinalProductPharmaceutical', 'MedicinalProductUndesirableEffect', 'MessageDefinition', 'MessageHeader', 'MolecularSequence', 'NamingSystem', 'NutritionOrder', 'Observation', 'ObservationDefinition', 'OperationDefinition', 'OperationOutcome', 'Organization', 'OrganizationAffiliation', 'Parameters', 'Patient', 'PaymentNotice', 'PaymentReconciliation', 'Person', 'PlanDefinition', 'Practitioner', 'PractitionerRole', 'Procedure', 'Provenance', 'Questionnaire', 'QuestionnaireResponse', 'RelatedPerson', 'RequestGroup', 'ResearchDefinition', 'ResearchElementDefinition', 'ResearchStudy', 'ResearchSubject', 'RiskAssessment', 'RiskEvidenceSynthesis', 'Schedule', 'SearchParameter', 'ServiceRequest', 'Slot', 'Specimen', 'SpecimenDefinition', 'StructureDefinition', 'StructureMap', 'Subscription', 'Substance', 'SubstanceNucleicAcid', 'SubstancePolymer', 'SubstanceProtein', 'SubstanceReferenceInformation', 'SubstanceSourceMaterial', 'SubstanceSpecification', 'SupplyDelivery', 'SupplyRequest', 'Task', 'TerminologyCapabilities', 'TestReport', 'TestScript', 'ValueSet', 'VerificationResult', 'VisionPrescription' ]
end
