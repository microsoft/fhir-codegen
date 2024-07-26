module FHIR

  ##
  # A formal computable definition of a graph of resources - that is, a coherent set of resources that form a graph by following references. The Graph Definition resource defines a set and makes rules about the set.
  class GraphDefinition < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['context-quantity', 'context-type-quantity', 'context-type-value', 'context-type', 'context', 'date', 'description', 'jurisdiction', 'name', 'publisher', 'start', 'status', 'url', 'version']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'GraphDefinition.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'GraphDefinition.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'GraphDefinition.implicitRules',
        'min'=>0,
        'max'=>1
      },
      ##
      # Language of the resource content
      # The base language in which the resource is written.
      # Language is provided to support indexing and accessibility (typically, services such as text to speech use the language tag). The html language tag in the narrative applies  to the narrative. The language tag on the resource may be used to specify the language of other presentations generated from the data in the resource. Not all the content has to be in the base language. The Resource.language should not be assumed to apply to the narrative automatically. If a language is specified, it should it also be specified on the div element in the html (see rules in HTML5 for information about the relationship between xml:lang and the html lang attribute).
      'language' => {
        'valid_codes'=>{
          'urn:ietf:bcp:47'=>[ 'ar', 'bn', 'cs', 'da', 'de', 'de-AT', 'de-CH', 'de-DE', 'el', 'en', 'en-AU', 'en-CA', 'en-GB', 'en-IN', 'en-NZ', 'en-SG', 'en-US', 'es', 'es-AR', 'es-ES', 'es-UY', 'fi', 'fr', 'fr-BE', 'fr-CH', 'fr-FR', 'fy', 'fy-NL', 'hi', 'hr', 'it', 'it-CH', 'it-IT', 'ja', 'ko', 'nl', 'nl-BE', 'nl-NL', 'no', 'no-NO', 'pa', 'pl', 'pt', 'pt-BR', 'ru', 'ru-RU', 'sr', 'sr-RS', 'sv', 'sv-SE', 'te', 'zh', 'zh-CN', 'zh-HK', 'zh-SG', 'zh-TW' ]
        },
        'type'=>'code',
        'path'=>'GraphDefinition.language',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/languages'}
      },
      ##
      # Text summary of the resource, for human interpretation
      # A human-readable narrative that contains a summary of the resource and can be used to represent the content of the resource to a human. The narrative need not encode all the structured data, but is required to contain sufficient detail to make it "clinically safe" for a human to just read the narrative. Resource definitions may define what content should be represented in the narrative to ensure clinical safety.
      # Contained resources do not have narrative. Resources that are not contained SHOULD have a narrative. In some cases, a resource may only have text with little or no additional discrete data (as long as all minOccurs=1 elements are satisfied).  This may be necessary for data from legacy systems where information is captured as a "text blob" or where text is additionally entered raw or narrated and encoded information is added later.
      'text' => {
        'type'=>'Narrative',
        'path'=>'GraphDefinition.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'GraphDefinition.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'GraphDefinition.extension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Extensions that cannot be ignored
      # May be used to represent additional information that is not part of the basic definition of the resource and that modifies the understanding of the element that contains it and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer is allowed to define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
      # 
      # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'modifierExtension' => {
        'type'=>'Extension',
        'path'=>'GraphDefinition.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Canonical identifier for this graph definition, represented as a URI (globally unique)
      # An absolute URI that is used to identify this graph definition when it is referenced in a specification, model, design or an instance; also called its canonical identifier. This SHOULD be globally unique and SHOULD be a literal address at which at which an authoritative instance of this graph definition is (or will be) published. This URL can be the target of a canonical reference. It SHALL remain the same when the graph definition is stored on different servers.
      # Can be a urn:uuid: or a urn:oid: but real http: addresses are preferred.  Multiple instances may share the same URL if they have a distinct version.
      # 
      # The determination of when to create a new version of a resource (same url, new version) vs. defining a new artifact is up to the author.  Considerations for making this decision are found in [Technical and Business Versions](resource.html#versions). 
      # 
      # In some cases, the resource can no longer be found at the stated url, but the url itself cannot change. Implementations can use the [meta.source](resource.html#meta) element to indicate where the current master source of the resource can be found.
      'url' => {
        'type'=>'uri',
        'path'=>'GraphDefinition.url',
        'min'=>0,
        'max'=>1
      },
      ##
      # Business version of the graph definition
      # The identifier that is used to identify this version of the graph definition when it is referenced in a specification, model, design or instance. This is an arbitrary value managed by the graph definition author and is not expected to be globally unique. For example, it might be a timestamp (e.g. yyyymmdd) if a managed version is not available. There is also no expectation that versions can be placed in a lexicographical sequence.
      # There may be different graph definition instances that have the same identifier but different versions.  The version can be appended to the url in a reference to allow a reference to a particular business version of the graph definition with the format [url]|[version].
      'version' => {
        'type'=>'string',
        'path'=>'GraphDefinition.version',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name for this graph definition (computer friendly)
      # A natural language name identifying the graph definition. This name should be usable as an identifier for the module by machine processing applications such as code generation.
      # The name is not expected to be globally unique. The name should be a simple alphanumeric type name to ensure that it is machine-processing friendly.
      'name' => {
        'type'=>'string',
        'path'=>'GraphDefinition.name',
        'min'=>1,
        'max'=>1
      },
      ##
      # draft | active | retired | unknown
      # The status of this graph definition. Enables tracking the life-cycle of the content.
      # Allows filtering of graph definitions that are appropriate for use versus not.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/publication-status'=>[ 'draft', 'active', 'retired', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'GraphDefinition.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/publication-status'}
      },
      ##
      # For testing purposes, not real usage
      # A Boolean value to indicate that this graph definition is authored for testing purposes (or education/evaluation/marketing) and is not intended to be used for genuine usage.
      # Allows filtering of graph definitions that are appropriate for use versus not.
      'experimental' => {
        'type'=>'boolean',
        'path'=>'GraphDefinition.experimental',
        'min'=>0,
        'max'=>1
      },
      ##
      # Date last changed
      # The date  (and optionally time) when the graph definition was published. The date must change when the business version changes and it must change if the status code changes. In addition, it should change when the substantive content of the graph definition changes.
      # Note that this is not the same as the resource last-modified-date, since the resource may be a secondary representation of the graph definition. Additional specific dates may be added as extensions or be found by consulting Provenances associated with past versions of the resource.
      'date' => {
        'type'=>'dateTime',
        'path'=>'GraphDefinition.date',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name of the publisher (organization or individual)
      # The name of the organization or individual that published the graph definition.
      # Usually an organization but may be an individual. The publisher (or steward) of the graph definition is the organization or individual primarily responsible for the maintenance and upkeep of the graph definition. This is not necessarily the same individual or organization that developed and initially authored the content. The publisher is the primary point of contact for questions or issues with the graph definition. This item SHOULD be populated unless the information is available from context.
      'publisher' => {
        'type'=>'string',
        'path'=>'GraphDefinition.publisher',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contact details for the publisher
      # Contact details to assist a user in finding and communicating with the publisher.
      # May be a web site, an email address, a telephone number, etc.
      'contact' => {
        'type'=>'ContactDetail',
        'path'=>'GraphDefinition.contact',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Natural language description of the graph definition
      # A free text natural language description of the graph definition from a consumer's perspective.
      # This description can be used to capture details such as why the graph definition was built, comments about misuse, instructions for clinical use and interpretation, literature references, examples from the paper world, etc. It is not a rendering of the graph definition as conveyed in the 'text' field of the resource itself. This item SHOULD be populated unless the information is available from context (e.g. the language of the graph definition is presumed to be the predominant language in the place the graph definition was created).
      'description' => {
        'type'=>'markdown',
        'path'=>'GraphDefinition.description',
        'min'=>0,
        'max'=>1
      },
      ##
      # The context that the content is intended to support
      # The content was developed with a focus and intent of supporting the contexts that are listed. These contexts may be general categories (gender, age, ...) or may be references to specific programs (insurance plans, studies, ...) and may be used to assist with indexing and searching for appropriate graph definition instances.
      # When multiple useContexts are specified, there is no expectation that all or any of the contexts apply.
      'useContext' => {
        'type'=>'UsageContext',
        'path'=>'GraphDefinition.useContext',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Intended jurisdiction for graph definition (if applicable)
      # A legal or geographic region in which the graph definition is intended to be used.
      # It may be possible for the graph definition to be used in jurisdictions other than those for which it was originally designed or intended.
      'jurisdiction' => {
        'type'=>'CodeableConcept',
        'path'=>'GraphDefinition.jurisdiction',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Why this graph definition is defined
      # Explanation of why this graph definition is needed and why it has been designed as it has.
      # This element does not describe the usage of the graph definition. Instead, it provides traceability of ''why'' the resource is either needed or ''why'' it is defined as it is.  This may be used to point to source materials or specifications that drove the structure of this graph definition.
      'purpose' => {
        'type'=>'markdown',
        'path'=>'GraphDefinition.purpose',
        'min'=>0,
        'max'=>1
      },
      ##
      # Type of resource at which the graph starts
      # The type of FHIR resource at which instances of this graph start.
      'start' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/resource-types'=>[ 'Account', 'ActivityDefinition', 'AdverseEvent', 'AllergyIntolerance', 'Appointment', 'AppointmentResponse', 'AuditEvent', 'Basic', 'Binary', 'BiologicallyDerivedProduct', 'BodyStructure', 'Bundle', 'CapabilityStatement', 'CarePlan', 'CareTeam', 'CatalogEntry', 'ChargeItem', 'ChargeItemDefinition', 'Claim', 'ClaimResponse', 'ClinicalImpression', 'CodeSystem', 'Communication', 'CommunicationRequest', 'CompartmentDefinition', 'Composition', 'ConceptMap', 'Condition', 'Consent', 'Contract', 'Coverage', 'CoverageEligibilityRequest', 'CoverageEligibilityResponse', 'DetectedIssue', 'Device', 'DeviceDefinition', 'DeviceMetric', 'DeviceRequest', 'DeviceUseStatement', 'DiagnosticReport', 'DocumentManifest', 'DocumentReference', 'DomainResource', 'EffectEvidenceSynthesis', 'Encounter', 'Endpoint', 'EnrollmentRequest', 'EnrollmentResponse', 'EpisodeOfCare', 'EventDefinition', 'Evidence', 'EvidenceVariable', 'ExampleScenario', 'ExplanationOfBenefit', 'FamilyMemberHistory', 'Flag', 'Goal', 'GraphDefinition', 'Group', 'GuidanceResponse', 'HealthcareService', 'ImagingStudy', 'Immunization', 'ImmunizationEvaluation', 'ImmunizationRecommendation', 'ImplementationGuide', 'InsurancePlan', 'Invoice', 'Library', 'Linkage', 'List', 'Location', 'Measure', 'MeasureReport', 'Media', 'Medication', 'MedicationAdministration', 'MedicationDispense', 'MedicationKnowledge', 'MedicationRequest', 'MedicationStatement', 'MedicinalProduct', 'MedicinalProductAuthorization', 'MedicinalProductContraindication', 'MedicinalProductIndication', 'MedicinalProductIngredient', 'MedicinalProductInteraction', 'MedicinalProductManufactured', 'MedicinalProductPackaged', 'MedicinalProductPharmaceutical', 'MedicinalProductUndesirableEffect', 'MessageDefinition', 'MessageHeader', 'MolecularSequence', 'NamingSystem', 'NutritionOrder', 'Observation', 'ObservationDefinition', 'OperationDefinition', 'OperationOutcome', 'Organization', 'OrganizationAffiliation', 'Parameters', 'Patient', 'PaymentNotice', 'PaymentReconciliation', 'Person', 'PlanDefinition', 'Practitioner', 'PractitionerRole', 'Procedure', 'Provenance', 'Questionnaire', 'QuestionnaireResponse', 'RelatedPerson', 'RequestGroup', 'ResearchDefinition', 'ResearchElementDefinition', 'ResearchStudy', 'ResearchSubject', 'Resource', 'RiskAssessment', 'RiskEvidenceSynthesis', 'Schedule', 'SearchParameter', 'ServiceRequest', 'Slot', 'Specimen', 'SpecimenDefinition', 'StructureDefinition', 'StructureMap', 'Subscription', 'Substance', 'SubstanceNucleicAcid', 'SubstancePolymer', 'SubstanceProtein', 'SubstanceReferenceInformation', 'SubstanceSourceMaterial', 'SubstanceSpecification', 'SupplyDelivery', 'SupplyRequest', 'Task', 'TerminologyCapabilities', 'TestReport', 'TestScript', 'ValueSet', 'VerificationResult', 'VisionPrescription' ]
        },
        'type'=>'code',
        'path'=>'GraphDefinition.start',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/resource-types'}
      },
      ##
      # Profile on base resource
      # The profile that describes the use of the base resource.
      # The code does not include the '$' prefix that is always included in the URL when the operation is invoked.
      'profile' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/StructureDefinition'],
        'type'=>'canonical',
        'path'=>'GraphDefinition.profile',
        'min'=>0,
        'max'=>1
      },
      ##
      # Links this graph makes rules about.
      'link' => {
        'type'=>'GraphDefinition::Link',
        'path'=>'GraphDefinition.link',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Links this graph makes rules about.
    class Link < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Link.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Link.extension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Extensions that cannot be ignored even if unrecognized
        # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
        # 
        # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'modifierExtension' => {
          'type'=>'Extension',
          'path'=>'Link.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Path in the resource that contains the link
        # A FHIR expression that identifies one of FHIR References to other resources.
        # The path expression cannot contain a resolve() function. If there is no path, the link is a reverse lookup, using target.params. If the path is "*" then this means all references in the resource.
        'path' => {
          'type'=>'string',
          'path'=>'Link.path',
          'min'=>0,
          'max'=>1
        },
        ##
        # Which slice (if profiled).
        'sliceName' => {
          'type'=>'string',
          'path'=>'Link.sliceName',
          'min'=>0,
          'max'=>1
        },
        ##
        # Minimum occurrences for this link.
        'min' => {
          'type'=>'integer',
          'path'=>'Link.min',
          'min'=>0,
          'max'=>1
        },
        ##
        # Maximum occurrences for this link.
        'max' => {
          'type'=>'string',
          'path'=>'Link.max',
          'min'=>0,
          'max'=>1
        },
        ##
        # Why this link is specified
        # Information about why this link is of interest in this graph definition.
        'description' => {
          'type'=>'string',
          'path'=>'Link.description',
          'min'=>0,
          'max'=>1
        },
        ##
        # Potential target for the link.
        'target' => {
          'type'=>'GraphDefinition::Link::Target',
          'path'=>'Link.target',
          'min'=>0,
          'max'=>Float::INFINITY
        }
      }

      ##
      # Potential target for the link.
      class Target < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'id',
            'path'=>'Target.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Target.extension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Extensions that cannot be ignored even if unrecognized
          # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
          # 
          # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'modifierExtension' => {
            'type'=>'Extension',
            'path'=>'Target.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Type of resource this link refers to.
          'type' => {
            'valid_codes'=>{
              'http://hl7.org/fhir/resource-types'=>[ 'Account', 'ActivityDefinition', 'AdverseEvent', 'AllergyIntolerance', 'Appointment', 'AppointmentResponse', 'AuditEvent', 'Basic', 'Binary', 'BiologicallyDerivedProduct', 'BodyStructure', 'Bundle', 'CapabilityStatement', 'CarePlan', 'CareTeam', 'CatalogEntry', 'ChargeItem', 'ChargeItemDefinition', 'Claim', 'ClaimResponse', 'ClinicalImpression', 'CodeSystem', 'Communication', 'CommunicationRequest', 'CompartmentDefinition', 'Composition', 'ConceptMap', 'Condition', 'Consent', 'Contract', 'Coverage', 'CoverageEligibilityRequest', 'CoverageEligibilityResponse', 'DetectedIssue', 'Device', 'DeviceDefinition', 'DeviceMetric', 'DeviceRequest', 'DeviceUseStatement', 'DiagnosticReport', 'DocumentManifest', 'DocumentReference', 'DomainResource', 'EffectEvidenceSynthesis', 'Encounter', 'Endpoint', 'EnrollmentRequest', 'EnrollmentResponse', 'EpisodeOfCare', 'EventDefinition', 'Evidence', 'EvidenceVariable', 'ExampleScenario', 'ExplanationOfBenefit', 'FamilyMemberHistory', 'Flag', 'Goal', 'GraphDefinition', 'Group', 'GuidanceResponse', 'HealthcareService', 'ImagingStudy', 'Immunization', 'ImmunizationEvaluation', 'ImmunizationRecommendation', 'ImplementationGuide', 'InsurancePlan', 'Invoice', 'Library', 'Linkage', 'List', 'Location', 'Measure', 'MeasureReport', 'Media', 'Medication', 'MedicationAdministration', 'MedicationDispense', 'MedicationKnowledge', 'MedicationRequest', 'MedicationStatement', 'MedicinalProduct', 'MedicinalProductAuthorization', 'MedicinalProductContraindication', 'MedicinalProductIndication', 'MedicinalProductIngredient', 'MedicinalProductInteraction', 'MedicinalProductManufactured', 'MedicinalProductPackaged', 'MedicinalProductPharmaceutical', 'MedicinalProductUndesirableEffect', 'MessageDefinition', 'MessageHeader', 'MolecularSequence', 'NamingSystem', 'NutritionOrder', 'Observation', 'ObservationDefinition', 'OperationDefinition', 'OperationOutcome', 'Organization', 'OrganizationAffiliation', 'Parameters', 'Patient', 'PaymentNotice', 'PaymentReconciliation', 'Person', 'PlanDefinition', 'Practitioner', 'PractitionerRole', 'Procedure', 'Provenance', 'Questionnaire', 'QuestionnaireResponse', 'RelatedPerson', 'RequestGroup', 'ResearchDefinition', 'ResearchElementDefinition', 'ResearchStudy', 'ResearchSubject', 'Resource', 'RiskAssessment', 'RiskEvidenceSynthesis', 'Schedule', 'SearchParameter', 'ServiceRequest', 'Slot', 'Specimen', 'SpecimenDefinition', 'StructureDefinition', 'StructureMap', 'Subscription', 'Substance', 'SubstanceNucleicAcid', 'SubstancePolymer', 'SubstanceProtein', 'SubstanceReferenceInformation', 'SubstanceSourceMaterial', 'SubstanceSpecification', 'SupplyDelivery', 'SupplyRequest', 'Task', 'TerminologyCapabilities', 'TestReport', 'TestScript', 'ValueSet', 'VerificationResult', 'VisionPrescription' ]
            },
            'type'=>'code',
            'path'=>'Target.type',
            'min'=>1,
            'max'=>1,
            'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/resource-types'}
          },
          ##
          # Criteria for reverse lookup
          # A set of parameters to look up.
          # At least one of the parameters must have the value {ref} which identifies the focus resource.
          'params' => {
            'type'=>'string',
            'path'=>'Target.params',
            'min'=>0,
            'max'=>1
          },
          ##
          # Profile for the target resource.
          'profile' => {
            'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/StructureDefinition'],
            'type'=>'canonical',
            'path'=>'Target.profile',
            'min'=>0,
            'max'=>1
          },
          ##
          # Compartment Consistency Rules.
          'compartment' => {
            'type'=>'GraphDefinition::Link::Target::Compartment',
            'path'=>'Target.compartment',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Additional links from target resource.
          'link' => {
            'type'=>'GraphDefinition::Link',
            'path'=>'Target.link',
            'min'=>0,
            'max'=>Float::INFINITY
          }
        }

        ##
        # Compartment Consistency Rules.
        class Compartment < FHIR::Model
          include FHIR::Hashable
          include FHIR::Json
          include FHIR::Xml

          METADATA = {
            ##
            # Unique id for inter-element referencing
            # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
            'id' => {
              'type'=>'id',
              'path'=>'Compartment.id',
              'min'=>0,
              'max'=>1
            },
            ##
            # Additional content defined by implementations
            # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
            # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
            'extension' => {
              'type'=>'Extension',
              'path'=>'Compartment.extension',
              'min'=>0,
              'max'=>Float::INFINITY
            },
            ##
            # Extensions that cannot be ignored even if unrecognized
            # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
            # 
            # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
            # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
            'modifierExtension' => {
              'type'=>'Extension',
              'path'=>'Compartment.modifierExtension',
              'min'=>0,
              'max'=>Float::INFINITY
            },
            ##
            # condition | requirement
            # Defines how the compartment rule is used - whether it it is used to test whether resources are subject to the rule, or whether it is a rule that must be followed.
            # All conditional rules are evaluated; if they are true, then the rules are evaluated.
            'use' => {
              'valid_codes'=>{
                'http://hl7.org/fhir/graph-compartment-use'=>[ 'condition', 'requirement' ]
              },
              'type'=>'code',
              'path'=>'Compartment.use',
              'min'=>1,
              'max'=>1,
              'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/graph-compartment-use'}
            },
            ##
            # Patient | Encounter | RelatedPerson | Practitioner | Device
            # Identifies the compartment.
            'code' => {
              'valid_codes'=>{
                'http://hl7.org/fhir/compartment-type'=>[ 'Patient', 'Encounter', 'RelatedPerson', 'Practitioner', 'Device' ]
              },
              'type'=>'code',
              'path'=>'Compartment.code',
              'min'=>1,
              'max'=>1,
              'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/compartment-type'}
            },
            ##
            # identical | matching | different | custom
            # identical | matching | different | no-rule | custom.
            'rule' => {
              'valid_codes'=>{
                'http://hl7.org/fhir/graph-compartment-rule'=>[ 'identical', 'matching', 'different', 'custom' ]
              },
              'type'=>'code',
              'path'=>'Compartment.rule',
              'min'=>1,
              'max'=>1,
              'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/graph-compartment-rule'}
            },
            ##
            # Custom rule, as a FHIRPath expression.
            'expression' => {
              'type'=>'string',
              'path'=>'Compartment.expression',
              'min'=>0,
              'max'=>1
            },
            ##
            # Documentation for FHIRPath expression.
            'description' => {
              'type'=>'string',
              'path'=>'Compartment.description',
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
          # Extensions that cannot be ignored even if unrecognized
          # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
          # 
          # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          attr_accessor :modifierExtension              # 0-* [ Extension ]
          ##
          # condition | requirement
          # Defines how the compartment rule is used - whether it it is used to test whether resources are subject to the rule, or whether it is a rule that must be followed.
          # All conditional rules are evaluated; if they are true, then the rules are evaluated.
          attr_accessor :use                            # 1-1 code
          ##
          # Patient | Encounter | RelatedPerson | Practitioner | Device
          # Identifies the compartment.
          attr_accessor :code                           # 1-1 code
          ##
          # identical | matching | different | custom
          # identical | matching | different | no-rule | custom.
          attr_accessor :rule                           # 1-1 code
          ##
          # Custom rule, as a FHIRPath expression.
          attr_accessor :expression                     # 0-1 string
          ##
          # Documentation for FHIRPath expression.
          attr_accessor :description                    # 0-1 string
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
        # Extensions that cannot be ignored even if unrecognized
        # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
        # 
        # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        attr_accessor :modifierExtension              # 0-* [ Extension ]
        ##
        # Type of resource this link refers to.
        attr_accessor :type                           # 1-1 code
        ##
        # Criteria for reverse lookup
        # A set of parameters to look up.
        # At least one of the parameters must have the value {ref} which identifies the focus resource.
        attr_accessor :params                         # 0-1 string
        ##
        # Profile for the target resource.
        attr_accessor :profile                        # 0-1 canonical(http://hl7.org/fhir/StructureDefinition/StructureDefinition)
        ##
        # Compartment Consistency Rules.
        attr_accessor :compartment                    # 0-* [ GraphDefinition::Link::Target::Compartment ]
        ##
        # Additional links from target resource.
        attr_accessor :link                           # 0-* [ GraphDefinition::Link ]
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
      # Extensions that cannot be ignored even if unrecognized
      # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
      # 
      # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      attr_accessor :modifierExtension              # 0-* [ Extension ]
      ##
      # Path in the resource that contains the link
      # A FHIR expression that identifies one of FHIR References to other resources.
      # The path expression cannot contain a resolve() function. If there is no path, the link is a reverse lookup, using target.params. If the path is "*" then this means all references in the resource.
      attr_accessor :path                           # 0-1 string
      ##
      # Which slice (if profiled).
      attr_accessor :sliceName                      # 0-1 string
      ##
      # Minimum occurrences for this link.
      attr_accessor :min                            # 0-1 integer
      ##
      # Maximum occurrences for this link.
      attr_accessor :max                            # 0-1 string
      ##
      # Why this link is specified
      # Information about why this link is of interest in this graph definition.
      attr_accessor :description                    # 0-1 string
      ##
      # Potential target for the link.
      attr_accessor :target                         # 0-* [ GraphDefinition::Link::Target ]
    end
    ##
    # Logical id of this artifact
    # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
    # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
    attr_accessor :id                             # 0-1 id
    ##
    # Metadata about the resource
    # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
    attr_accessor :meta                           # 0-1 Meta
    ##
    # A set of rules under which this content was created
    # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
    # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
    attr_accessor :implicitRules                  # 0-1 uri
    ##
    # Language of the resource content
    # The base language in which the resource is written.
    # Language is provided to support indexing and accessibility (typically, services such as text to speech use the language tag). The html language tag in the narrative applies  to the narrative. The language tag on the resource may be used to specify the language of other presentations generated from the data in the resource. Not all the content has to be in the base language. The Resource.language should not be assumed to apply to the narrative automatically. If a language is specified, it should it also be specified on the div element in the html (see rules in HTML5 for information about the relationship between xml:lang and the html lang attribute).
    attr_accessor :language                       # 0-1 code
    ##
    # Text summary of the resource, for human interpretation
    # A human-readable narrative that contains a summary of the resource and can be used to represent the content of the resource to a human. The narrative need not encode all the structured data, but is required to contain sufficient detail to make it "clinically safe" for a human to just read the narrative. Resource definitions may define what content should be represented in the narrative to ensure clinical safety.
    # Contained resources do not have narrative. Resources that are not contained SHOULD have a narrative. In some cases, a resource may only have text with little or no additional discrete data (as long as all minOccurs=1 elements are satisfied).  This may be necessary for data from legacy systems where information is captured as a "text blob" or where text is additionally entered raw or narrated and encoded information is added later.
    attr_accessor :text                           # 0-1 Narrative
    ##
    # Contained, inline Resources
    # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
    # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
    attr_accessor :contained                      # 0-* [ Resource ]
    ##
    # Additional content defined by implementations
    # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
    # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
    attr_accessor :extension                      # 0-* [ Extension ]
    ##
    # Extensions that cannot be ignored
    # May be used to represent additional information that is not part of the basic definition of the resource and that modifies the understanding of the element that contains it and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer is allowed to define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
    # 
    # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
    # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
    attr_accessor :modifierExtension              # 0-* [ Extension ]
    ##
    # Canonical identifier for this graph definition, represented as a URI (globally unique)
    # An absolute URI that is used to identify this graph definition when it is referenced in a specification, model, design or an instance; also called its canonical identifier. This SHOULD be globally unique and SHOULD be a literal address at which at which an authoritative instance of this graph definition is (or will be) published. This URL can be the target of a canonical reference. It SHALL remain the same when the graph definition is stored on different servers.
    # Can be a urn:uuid: or a urn:oid: but real http: addresses are preferred.  Multiple instances may share the same URL if they have a distinct version.
    # 
    # The determination of when to create a new version of a resource (same url, new version) vs. defining a new artifact is up to the author.  Considerations for making this decision are found in [Technical and Business Versions](resource.html#versions). 
    # 
    # In some cases, the resource can no longer be found at the stated url, but the url itself cannot change. Implementations can use the [meta.source](resource.html#meta) element to indicate where the current master source of the resource can be found.
    attr_accessor :url                            # 0-1 uri
    ##
    # Business version of the graph definition
    # The identifier that is used to identify this version of the graph definition when it is referenced in a specification, model, design or instance. This is an arbitrary value managed by the graph definition author and is not expected to be globally unique. For example, it might be a timestamp (e.g. yyyymmdd) if a managed version is not available. There is also no expectation that versions can be placed in a lexicographical sequence.
    # There may be different graph definition instances that have the same identifier but different versions.  The version can be appended to the url in a reference to allow a reference to a particular business version of the graph definition with the format [url]|[version].
    attr_accessor :version                        # 0-1 string
    ##
    # Name for this graph definition (computer friendly)
    # A natural language name identifying the graph definition. This name should be usable as an identifier for the module by machine processing applications such as code generation.
    # The name is not expected to be globally unique. The name should be a simple alphanumeric type name to ensure that it is machine-processing friendly.
    attr_accessor :name                           # 1-1 string
    ##
    # draft | active | retired | unknown
    # The status of this graph definition. Enables tracking the life-cycle of the content.
    # Allows filtering of graph definitions that are appropriate for use versus not.
    attr_accessor :status                         # 1-1 code
    ##
    # For testing purposes, not real usage
    # A Boolean value to indicate that this graph definition is authored for testing purposes (or education/evaluation/marketing) and is not intended to be used for genuine usage.
    # Allows filtering of graph definitions that are appropriate for use versus not.
    attr_accessor :experimental                   # 0-1 boolean
    ##
    # Date last changed
    # The date  (and optionally time) when the graph definition was published. The date must change when the business version changes and it must change if the status code changes. In addition, it should change when the substantive content of the graph definition changes.
    # Note that this is not the same as the resource last-modified-date, since the resource may be a secondary representation of the graph definition. Additional specific dates may be added as extensions or be found by consulting Provenances associated with past versions of the resource.
    attr_accessor :date                           # 0-1 dateTime
    ##
    # Name of the publisher (organization or individual)
    # The name of the organization or individual that published the graph definition.
    # Usually an organization but may be an individual. The publisher (or steward) of the graph definition is the organization or individual primarily responsible for the maintenance and upkeep of the graph definition. This is not necessarily the same individual or organization that developed and initially authored the content. The publisher is the primary point of contact for questions or issues with the graph definition. This item SHOULD be populated unless the information is available from context.
    attr_accessor :publisher                      # 0-1 string
    ##
    # Contact details for the publisher
    # Contact details to assist a user in finding and communicating with the publisher.
    # May be a web site, an email address, a telephone number, etc.
    attr_accessor :contact                        # 0-* [ ContactDetail ]
    ##
    # Natural language description of the graph definition
    # A free text natural language description of the graph definition from a consumer's perspective.
    # This description can be used to capture details such as why the graph definition was built, comments about misuse, instructions for clinical use and interpretation, literature references, examples from the paper world, etc. It is not a rendering of the graph definition as conveyed in the 'text' field of the resource itself. This item SHOULD be populated unless the information is available from context (e.g. the language of the graph definition is presumed to be the predominant language in the place the graph definition was created).
    attr_accessor :description                    # 0-1 markdown
    ##
    # The context that the content is intended to support
    # The content was developed with a focus and intent of supporting the contexts that are listed. These contexts may be general categories (gender, age, ...) or may be references to specific programs (insurance plans, studies, ...) and may be used to assist with indexing and searching for appropriate graph definition instances.
    # When multiple useContexts are specified, there is no expectation that all or any of the contexts apply.
    attr_accessor :useContext                     # 0-* [ UsageContext ]
    ##
    # Intended jurisdiction for graph definition (if applicable)
    # A legal or geographic region in which the graph definition is intended to be used.
    # It may be possible for the graph definition to be used in jurisdictions other than those for which it was originally designed or intended.
    attr_accessor :jurisdiction                   # 0-* [ CodeableConcept ]
    ##
    # Why this graph definition is defined
    # Explanation of why this graph definition is needed and why it has been designed as it has.
    # This element does not describe the usage of the graph definition. Instead, it provides traceability of ''why'' the resource is either needed or ''why'' it is defined as it is.  This may be used to point to source materials or specifications that drove the structure of this graph definition.
    attr_accessor :purpose                        # 0-1 markdown
    ##
    # Type of resource at which the graph starts
    # The type of FHIR resource at which instances of this graph start.
    attr_accessor :start                          # 1-1 code
    ##
    # Profile on base resource
    # The profile that describes the use of the base resource.
    # The code does not include the '$' prefix that is always included in the URL when the operation is invoked.
    attr_accessor :profile                        # 0-1 canonical(http://hl7.org/fhir/StructureDefinition/StructureDefinition)
    ##
    # Links this graph makes rules about.
    attr_accessor :link                           # 0-* [ GraphDefinition::Link ]

    def resourceType
      'GraphDefinition'
    end
  end
end
