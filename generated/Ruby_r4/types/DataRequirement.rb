module FHIR

  ##
  # Base StructureDefinition for DataRequirement Type: Describes a required data item for evaluation in terms of the type of data, and optional code or date-based filters of the data.
  class DataRequirement < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = []
    MULTIPLE_TYPES = {
      'subject[x]' => ['CodeableConcept', 'Reference']
    }
    METADATA = {
      ##
      # Unique id for inter-element referencing
      # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
      'id' => {
        'type'=>'id',
        'path'=>'DataRequirement.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'DataRequirement.extension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The type of the required data, specified as the type name of a resource. For profiles, this value is set to the type of the base resource of the profile.
      'type' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/abstract-types'=>[ 'Type', 'Any' ],
          'http://hl7.org/fhir/data-types'=>[ 'Address', 'Age', 'Annotation', 'Attachment', 'BackboneElement', 'CodeableConcept', 'Coding', 'ContactDetail', 'ContactPoint', 'Contributor', 'Count', 'DataRequirement', 'Distance', 'Dosage', 'Duration', 'Element', 'ElementDefinition', 'Expression', 'Extension', 'HumanName', 'Identifier', 'MarketingStatus', 'Meta', 'Money', 'MoneyQuantity', 'Narrative', 'ParameterDefinition', 'Period', 'Population', 'ProdCharacteristic', 'ProductShelfLife', 'Quantity', 'Range', 'Ratio', 'Reference', 'RelatedArtifact', 'SampledData', 'Signature', 'SimpleQuantity', 'SubstanceAmount', 'Timing', 'TriggerDefinition', 'UsageContext', 'base64Binary', 'boolean', 'canonical', 'code', 'date', 'dateTime', 'decimal', 'id', 'instant', 'integer', 'markdown', 'oid', 'positiveInt', 'string', 'time', 'unsignedInt', 'uri', 'url', 'uuid', 'xhtml' ],
          'http://hl7.org/fhir/resource-types'=>[ 'Account', 'ActivityDefinition', 'AdverseEvent', 'AllergyIntolerance', 'Appointment', 'AppointmentResponse', 'AuditEvent', 'Basic', 'Binary', 'BiologicallyDerivedProduct', 'BodyStructure', 'Bundle', 'CapabilityStatement', 'CarePlan', 'CareTeam', 'CatalogEntry', 'ChargeItem', 'ChargeItemDefinition', 'Claim', 'ClaimResponse', 'ClinicalImpression', 'CodeSystem', 'Communication', 'CommunicationRequest', 'CompartmentDefinition', 'Composition', 'ConceptMap', 'Condition', 'Consent', 'Contract', 'Coverage', 'CoverageEligibilityRequest', 'CoverageEligibilityResponse', 'DetectedIssue', 'Device', 'DeviceDefinition', 'DeviceMetric', 'DeviceRequest', 'DeviceUseStatement', 'DiagnosticReport', 'DocumentManifest', 'DocumentReference', 'DomainResource', 'EffectEvidenceSynthesis', 'Encounter', 'Endpoint', 'EnrollmentRequest', 'EnrollmentResponse', 'EpisodeOfCare', 'EventDefinition', 'Evidence', 'EvidenceVariable', 'ExampleScenario', 'ExplanationOfBenefit', 'FamilyMemberHistory', 'Flag', 'Goal', 'GraphDefinition', 'Group', 'GuidanceResponse', 'HealthcareService', 'ImagingStudy', 'Immunization', 'ImmunizationEvaluation', 'ImmunizationRecommendation', 'ImplementationGuide', 'InsurancePlan', 'Invoice', 'Library', 'Linkage', 'List', 'Location', 'Measure', 'MeasureReport', 'Media', 'Medication', 'MedicationAdministration', 'MedicationDispense', 'MedicationKnowledge', 'MedicationRequest', 'MedicationStatement', 'MedicinalProduct', 'MedicinalProductAuthorization', 'MedicinalProductContraindication', 'MedicinalProductIndication', 'MedicinalProductIngredient', 'MedicinalProductInteraction', 'MedicinalProductManufactured', 'MedicinalProductPackaged', 'MedicinalProductPharmaceutical', 'MedicinalProductUndesirableEffect', 'MessageDefinition', 'MessageHeader', 'MolecularSequence', 'NamingSystem', 'NutritionOrder', 'Observation', 'ObservationDefinition', 'OperationDefinition', 'OperationOutcome', 'Organization', 'OrganizationAffiliation', 'Parameters', 'Patient', 'PaymentNotice', 'PaymentReconciliation', 'Person', 'PlanDefinition', 'Practitioner', 'PractitionerRole', 'Procedure', 'Provenance', 'Questionnaire', 'QuestionnaireResponse', 'RelatedPerson', 'RequestGroup', 'ResearchDefinition', 'ResearchElementDefinition', 'ResearchStudy', 'ResearchSubject', 'Resource', 'RiskAssessment', 'RiskEvidenceSynthesis', 'Schedule', 'SearchParameter', 'ServiceRequest', 'Slot', 'Specimen', 'SpecimenDefinition', 'StructureDefinition', 'StructureMap', 'Subscription', 'Substance', 'SubstanceNucleicAcid', 'SubstancePolymer', 'SubstanceProtein', 'SubstanceReferenceInformation', 'SubstanceSourceMaterial', 'SubstanceSpecification', 'SupplyDelivery', 'SupplyRequest', 'Task', 'TerminologyCapabilities', 'TestReport', 'TestScript', 'ValueSet', 'VerificationResult', 'VisionPrescription' ]
        },
        'type'=>'code',
        'path'=>'DataRequirement.type',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/all-types'}
      },
      ##
      # The profile of the required data, specified as the uri of the profile definition.
      'profile' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/StructureDefinition'],
        'type'=>'canonical',
        'path'=>'DataRequirement.profile',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # E.g. Patient, Practitioner, RelatedPerson, Organization, Location, Device
      # The intended subjects of the data requirement. If this element is not provided, a Patient subject is assumed.
      # The subject of a data requirement is critical, as the data being specified is determined with respect to a particular subject. This corresponds roughly to the notion of a Compartment in that it limits what data is available based on its relationship to the subject. In CQL, this corresponds to the context declaration.
      'subjectCodeableConcept' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/resource-types'=>[ 'Patient', 'Practitioner', 'Organization', 'Location', 'Device' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'DataRequirement.subject[x]',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/subject-type'}
      },
      ##
      # E.g. Patient, Practitioner, RelatedPerson, Organization, Location, Device
      # The intended subjects of the data requirement. If this element is not provided, a Patient subject is assumed.
      # The subject of a data requirement is critical, as the data being specified is determined with respect to a particular subject. This corresponds roughly to the notion of a Compartment in that it limits what data is available based on its relationship to the subject. In CQL, this corresponds to the context declaration.
      'subjectReference' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/resource-types'=>[ 'Patient', 'Practitioner', 'Organization', 'Location', 'Device' ]
        },
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Group'],
        'type'=>'Reference',
        'path'=>'DataRequirement.subject[x]',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/subject-type'}
      },
      ##
      # Indicates specific structure elements that are referenced by the knowledge module
      # Indicates that specific elements of the type are referenced by the knowledge module and must be supported by the consumer in order to obtain an effective evaluation. This does not mean that a value is required for this element, only that the consuming system must understand the element and be able to provide values for it if they are available. 
      # 
      # The value of mustSupport SHALL be a FHIRPath resolveable on the type of the DataRequirement. The path SHALL consist only of identifiers, constant indexers, and .resolve() (see the [Simple FHIRPath Profile](fhirpath.html#simple) for full details).
      'mustSupport' => {
        'type'=>'string',
        'path'=>'DataRequirement.mustSupport',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # What codes are expected
      # Code filters specify additional constraints on the data, specifying the value set of interest for a particular element of the data. Each code filter defines an additional constraint on the data, i.e. code filters are AND'ed, not OR'ed.
      'codeFilter' => {
        'type'=>'DataRequirement::CodeFilter',
        'path'=>'DataRequirement.codeFilter',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # What dates/date ranges are expected
      # Date filters specify additional constraints on the data in terms of the applicable date range for specific elements. Each date filter specifies an additional constraint on the data, i.e. date filters are AND'ed, not OR'ed.
      'dateFilter' => {
        'type'=>'DataRequirement::DateFilter',
        'path'=>'DataRequirement.dateFilter',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Number of results
      # Specifies a maximum number of results that are required (uses the _count search parameter).
      # This element can be used in combination with the sort element to specify quota requirements such as "the most recent 5" or "the highest 5".
      'limit' => {
        'type'=>'positiveInt',
        'path'=>'DataRequirement.limit',
        'min'=>0,
        'max'=>1
      },
      ##
      # Order of the results
      # Specifies the order of the results to be returned.
      # This element can be used in combination with the sort element to specify quota requirements such as "the most recent 5" or "the highest 5". When multiple sorts are specified, they are applied in the order they appear in the resource.
      'sort' => {
        'type'=>'DataRequirement::Sort',
        'path'=>'DataRequirement.sort',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # What codes are expected
    # Code filters specify additional constraints on the data, specifying the value set of interest for a particular element of the data. Each code filter defines an additional constraint on the data, i.e. code filters are AND'ed, not OR'ed.
    class CodeFilter < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'CodeFilter.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'CodeFilter.extension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # A code-valued attribute to filter on
        # The code-valued attribute of the filter. The specified path SHALL be a FHIRPath resolveable on the specified type of the DataRequirement, and SHALL consist only of identifiers, constant indexers, and .resolve(). The path is allowed to contain qualifiers (.) to traverse sub-elements, as well as indexers ([x]) to traverse multiple-cardinality sub-elements (see the [Simple FHIRPath Profile](fhirpath.html#simple) for full details). Note that the index must be an integer constant. The path must resolve to an element of type code, Coding, or CodeableConcept.
        # The path attribute contains a [Simple FHIRPath Subset](fhirpath.html#simple) that allows path traversal, but not calculation.
        'path' => {
          'type'=>'string',
          'path'=>'CodeFilter.path',
          'min'=>0,
          'max'=>1
        },
        ##
        # A coded (token) parameter to search on
        # A token parameter that refers to a search parameter defined on the specified type of the DataRequirement, and which searches on elements of type code, Coding, or CodeableConcept.
        'searchParam' => {
          'type'=>'string',
          'path'=>'CodeFilter.searchParam',
          'min'=>0,
          'max'=>1
        },
        ##
        # Valueset for the filter
        # The valueset for the code filter. The valueSet and code elements are additive. If valueSet is specified, the filter will return only those data items for which the value of the code-valued element specified in the path is a member of the specified valueset.
        'valueSet' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ValueSet'],
          'type'=>'canonical',
          'path'=>'CodeFilter.valueSet',
          'min'=>0,
          'max'=>1
        },
        ##
        # What code is expected
        # The codes for the code filter. If values are given, the filter will return only those data items for which the code-valued attribute specified by the path has a value that is one of the specified codes. If codes are specified in addition to a value set, the filter returns items matching a code in the value set or one of the specified codes.
        'code' => {
          'type'=>'Coding',
          'path'=>'CodeFilter.code',
          'min'=>0,
          'max'=>Float::INFINITY
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
      # A code-valued attribute to filter on
      # The code-valued attribute of the filter. The specified path SHALL be a FHIRPath resolveable on the specified type of the DataRequirement, and SHALL consist only of identifiers, constant indexers, and .resolve(). The path is allowed to contain qualifiers (.) to traverse sub-elements, as well as indexers ([x]) to traverse multiple-cardinality sub-elements (see the [Simple FHIRPath Profile](fhirpath.html#simple) for full details). Note that the index must be an integer constant. The path must resolve to an element of type code, Coding, or CodeableConcept.
      # The path attribute contains a [Simple FHIRPath Subset](fhirpath.html#simple) that allows path traversal, but not calculation.
      attr_accessor :path                           # 0-1 string
      ##
      # A coded (token) parameter to search on
      # A token parameter that refers to a search parameter defined on the specified type of the DataRequirement, and which searches on elements of type code, Coding, or CodeableConcept.
      attr_accessor :searchParam                    # 0-1 string
      ##
      # Valueset for the filter
      # The valueset for the code filter. The valueSet and code elements are additive. If valueSet is specified, the filter will return only those data items for which the value of the code-valued element specified in the path is a member of the specified valueset.
      attr_accessor :valueSet                       # 0-1 canonical(http://hl7.org/fhir/StructureDefinition/ValueSet)
      ##
      # What code is expected
      # The codes for the code filter. If values are given, the filter will return only those data items for which the code-valued attribute specified by the path has a value that is one of the specified codes. If codes are specified in addition to a value set, the filter returns items matching a code in the value set or one of the specified codes.
      attr_accessor :code                           # 0-* [ Coding ]
    end

    ##
    # What dates/date ranges are expected
    # Date filters specify additional constraints on the data in terms of the applicable date range for specific elements. Each date filter specifies an additional constraint on the data, i.e. date filters are AND'ed, not OR'ed.
    class DateFilter < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'value[x]' => ['dateTime', 'Duration', 'Period']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'DateFilter.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'DateFilter.extension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # A date-valued attribute to filter on
        # The date-valued attribute of the filter. The specified path SHALL be a FHIRPath resolveable on the specified type of the DataRequirement, and SHALL consist only of identifiers, constant indexers, and .resolve(). The path is allowed to contain qualifiers (.) to traverse sub-elements, as well as indexers ([x]) to traverse multiple-cardinality sub-elements (see the [Simple FHIRPath Profile](fhirpath.html#simple) for full details). Note that the index must be an integer constant. The path must resolve to an element of type date, dateTime, Period, Schedule, or Timing.
        # The path attribute contains a [Simple FHIR Subset](fhirpath.html#simple) that allows path traversal, but not calculation.
        'path' => {
          'type'=>'string',
          'path'=>'DateFilter.path',
          'min'=>0,
          'max'=>1
        },
        ##
        # A date valued parameter to search on
        # A date parameter that refers to a search parameter defined on the specified type of the DataRequirement, and which searches on elements of type date, dateTime, Period, Schedule, or Timing.
        'searchParam' => {
          'type'=>'string',
          'path'=>'DateFilter.searchParam',
          'min'=>0,
          'max'=>1
        },
        ##
        # The value of the filter, as a Period, DateTime, or Duration value
        # The value of the filter. If period is specified, the filter will return only those data items that fall within the bounds determined by the Period, inclusive of the period boundaries. If dateTime is specified, the filter will return only those data items that are equal to the specified dateTime. If a Duration is specified, the filter will return only those data items that fall within Duration before now.
        'valueDateTime' => {
          'type'=>'DateTime',
          'path'=>'DateFilter.value[x]',
          'min'=>0,
          'max'=>1
        }
        ##
        # The value of the filter, as a Period, DateTime, or Duration value
        # The value of the filter. If period is specified, the filter will return only those data items that fall within the bounds determined by the Period, inclusive of the period boundaries. If dateTime is specified, the filter will return only those data items that are equal to the specified dateTime. If a Duration is specified, the filter will return only those data items that fall within Duration before now.
        'valueDuration' => {
          'type'=>'Duration',
          'path'=>'DateFilter.value[x]',
          'min'=>0,
          'max'=>1
        }
        ##
        # The value of the filter, as a Period, DateTime, or Duration value
        # The value of the filter. If period is specified, the filter will return only those data items that fall within the bounds determined by the Period, inclusive of the period boundaries. If dateTime is specified, the filter will return only those data items that are equal to the specified dateTime. If a Duration is specified, the filter will return only those data items that fall within Duration before now.
        'valuePeriod' => {
          'type'=>'Period',
          'path'=>'DateFilter.value[x]',
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
      # A date-valued attribute to filter on
      # The date-valued attribute of the filter. The specified path SHALL be a FHIRPath resolveable on the specified type of the DataRequirement, and SHALL consist only of identifiers, constant indexers, and .resolve(). The path is allowed to contain qualifiers (.) to traverse sub-elements, as well as indexers ([x]) to traverse multiple-cardinality sub-elements (see the [Simple FHIRPath Profile](fhirpath.html#simple) for full details). Note that the index must be an integer constant. The path must resolve to an element of type date, dateTime, Period, Schedule, or Timing.
      # The path attribute contains a [Simple FHIR Subset](fhirpath.html#simple) that allows path traversal, but not calculation.
      attr_accessor :path                           # 0-1 string
      ##
      # A date valued parameter to search on
      # A date parameter that refers to a search parameter defined on the specified type of the DataRequirement, and which searches on elements of type date, dateTime, Period, Schedule, or Timing.
      attr_accessor :searchParam                    # 0-1 string
      ##
      # The value of the filter, as a Period, DateTime, or Duration value
      # The value of the filter. If period is specified, the filter will return only those data items that fall within the bounds determined by the Period, inclusive of the period boundaries. If dateTime is specified, the filter will return only those data items that are equal to the specified dateTime. If a Duration is specified, the filter will return only those data items that fall within Duration before now.
      attr_accessor :valueDateTime                  # 0-1 DateTime
      ##
      # The value of the filter, as a Period, DateTime, or Duration value
      # The value of the filter. If period is specified, the filter will return only those data items that fall within the bounds determined by the Period, inclusive of the period boundaries. If dateTime is specified, the filter will return only those data items that are equal to the specified dateTime. If a Duration is specified, the filter will return only those data items that fall within Duration before now.
      attr_accessor :valueDuration                  # 0-1 Duration
      ##
      # The value of the filter, as a Period, DateTime, or Duration value
      # The value of the filter. If period is specified, the filter will return only those data items that fall within the bounds determined by the Period, inclusive of the period boundaries. If dateTime is specified, the filter will return only those data items that are equal to the specified dateTime. If a Duration is specified, the filter will return only those data items that fall within Duration before now.
      attr_accessor :valuePeriod                    # 0-1 Period
    end

    ##
    # Order of the results
    # Specifies the order of the results to be returned.
    # This element can be used in combination with the sort element to specify quota requirements such as "the most recent 5" or "the highest 5". When multiple sorts are specified, they are applied in the order they appear in the resource.
    class Sort < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Sort.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Sort.extension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # The name of the attribute to perform the sort
        # The attribute of the sort. The specified path must be resolvable from the type of the required data. The path is allowed to contain qualifiers (.) to traverse sub-elements, as well as indexers ([x]) to traverse multiple-cardinality sub-elements. Note that the index must be an integer constant.
        'path' => {
          'type'=>'string',
          'path'=>'Sort.path',
          'min'=>1,
          'max'=>1
        },
        ##
        # ascending | descending
        # The direction of the sort, ascending or descending.
        'direction' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/sort-direction'=>[ 'ascending', 'descending' ]
          },
          'type'=>'code',
          'path'=>'Sort.direction',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/sort-direction'}
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
      # The name of the attribute to perform the sort
      # The attribute of the sort. The specified path must be resolvable from the type of the required data. The path is allowed to contain qualifiers (.) to traverse sub-elements, as well as indexers ([x]) to traverse multiple-cardinality sub-elements. Note that the index must be an integer constant.
      attr_accessor :path                           # 1-1 string
      ##
      # ascending | descending
      # The direction of the sort, ascending or descending.
      attr_accessor :direction                      # 1-1 code
    end
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
    # The type of the required data, specified as the type name of a resource. For profiles, this value is set to the type of the base resource of the profile.
    attr_accessor :type                           # 1-1 code
    ##
    # The profile of the required data, specified as the uri of the profile definition.
    attr_accessor :profile                        # 0-* [ canonical(http://hl7.org/fhir/StructureDefinition/StructureDefinition) ]
    ##
    # E.g. Patient, Practitioner, RelatedPerson, Organization, Location, Device
    # The intended subjects of the data requirement. If this element is not provided, a Patient subject is assumed.
    # The subject of a data requirement is critical, as the data being specified is determined with respect to a particular subject. This corresponds roughly to the notion of a Compartment in that it limits what data is available based on its relationship to the subject. In CQL, this corresponds to the context declaration.
    attr_accessor :subjectCodeableConcept         # 0-1 CodeableConcept
    ##
    # E.g. Patient, Practitioner, RelatedPerson, Organization, Location, Device
    # The intended subjects of the data requirement. If this element is not provided, a Patient subject is assumed.
    # The subject of a data requirement is critical, as the data being specified is determined with respect to a particular subject. This corresponds roughly to the notion of a Compartment in that it limits what data is available based on its relationship to the subject. In CQL, this corresponds to the context declaration.
    attr_accessor :subjectReference               # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Group)
    ##
    # Indicates specific structure elements that are referenced by the knowledge module
    # Indicates that specific elements of the type are referenced by the knowledge module and must be supported by the consumer in order to obtain an effective evaluation. This does not mean that a value is required for this element, only that the consuming system must understand the element and be able to provide values for it if they are available. 
    # 
    # The value of mustSupport SHALL be a FHIRPath resolveable on the type of the DataRequirement. The path SHALL consist only of identifiers, constant indexers, and .resolve() (see the [Simple FHIRPath Profile](fhirpath.html#simple) for full details).
    attr_accessor :mustSupport                    # 0-* [ string ]
    ##
    # What codes are expected
    # Code filters specify additional constraints on the data, specifying the value set of interest for a particular element of the data. Each code filter defines an additional constraint on the data, i.e. code filters are AND'ed, not OR'ed.
    attr_accessor :codeFilter                     # 0-* [ DataRequirement::CodeFilter ]
    ##
    # What dates/date ranges are expected
    # Date filters specify additional constraints on the data in terms of the applicable date range for specific elements. Each date filter specifies an additional constraint on the data, i.e. date filters are AND'ed, not OR'ed.
    attr_accessor :dateFilter                     # 0-* [ DataRequirement::DateFilter ]
    ##
    # Number of results
    # Specifies a maximum number of results that are required (uses the _count search parameter).
    # This element can be used in combination with the sort element to specify quota requirements such as "the most recent 5" or "the highest 5".
    attr_accessor :limit                          # 0-1 positiveInt
    ##
    # Order of the results
    # Specifies the order of the results to be returned.
    # This element can be used in combination with the sort element to specify quota requirements such as "the most recent 5" or "the highest 5". When multiple sorts are specified, they are applied in the order they appear in the resource.
    attr_accessor :sort                           # 0-* [ DataRequirement::Sort ]
  end
end
