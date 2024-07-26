module FHIR

  ##
  # A structured set of questions intended to guide the collection of answers from end-users. Questionnaires provide detailed control over order, presentation, phraseology and grouping to allow coherent, consistent data collection.
  # To support structured, hierarchical registration of data gathered using digital forms and other questionnaires.  Questionnaires provide greater control over presentation and allow capture of data in a domain-independent way (i.e. capturing information that would otherwise require multiple distinct types of resources).
  class Questionnaire < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['code', 'context-quantity', 'context-type-quantity', 'context-type-value', 'context-type', 'context', 'date', 'definition', 'description', 'effective', 'identifier', 'jurisdiction', 'name', 'publisher', 'status', 'subject-type', 'title', 'url', 'version']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'Questionnaire.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'Questionnaire.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'Questionnaire.implicitRules',
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
        'path'=>'Questionnaire.language',
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
        'path'=>'Questionnaire.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'Questionnaire.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Questionnaire.extension',
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
        'path'=>'Questionnaire.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Canonical identifier for this questionnaire, represented as a URI (globally unique)
      # An absolute URI that is used to identify this questionnaire when it is referenced in a specification, model, design or an instance; also called its canonical identifier. This SHOULD be globally unique and SHOULD be a literal address at which at which an authoritative instance of this questionnaire is (or will be) published. This URL can be the target of a canonical reference. It SHALL remain the same when the questionnaire is stored on different servers.
      # The name of the referenced questionnaire can be conveyed using the http://hl7.org/fhir/StructureDefinition/display extension.
      'url' => {
        'type'=>'uri',
        'path'=>'Questionnaire.url',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional identifier for the questionnaire
      # A formal identifier that is used to identify this questionnaire when it is represented in other formats, or referenced in a specification, model, design or an instance.
      # Typically, this is used for identifiers that can go in an HL7 V3 II (instance identifier) data type, and can then identify this questionnaire outside of FHIR, where it is not possible to use the logical URI.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'Questionnaire.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Business version of the questionnaire
      # The identifier that is used to identify this version of the questionnaire when it is referenced in a specification, model, design or instance. This is an arbitrary value managed by the questionnaire author and is not expected to be globally unique. For example, it might be a timestamp (e.g. yyyymmdd) if a managed version is not available. There is also no expectation that versions can be placed in a lexicographical sequence.
      # There may be different questionnaire instances that have the same identifier but different versions.  The version can be appended to the url in a reference to allow a reference to a particular business version of the questionnaire with the format [url]|[version].
      'version' => {
        'type'=>'string',
        'path'=>'Questionnaire.version',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name for this questionnaire (computer friendly)
      # A natural language name identifying the questionnaire. This name should be usable as an identifier for the module by machine processing applications such as code generation.
      # The name is not expected to be globally unique. The name should be a simple alphanumeric type name to ensure that it is machine-processing friendly.
      'name' => {
        'type'=>'string',
        'path'=>'Questionnaire.name',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name for this questionnaire (human friendly)
      # A short, descriptive, user-friendly title for the questionnaire.
      # This name does not need to be machine-processing friendly and may contain punctuation, white-space, etc.
      'title' => {
        'type'=>'string',
        'path'=>'Questionnaire.title',
        'min'=>0,
        'max'=>1
      },
      ##
      # Instantiates protocol or definition
      # The URL of a Questionnaire that this Questionnaire is based on.
      'derivedFrom' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Questionnaire'],
        'type'=>'canonical',
        'path'=>'Questionnaire.derivedFrom',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # draft | active | retired | unknown
      # The status of this questionnaire. Enables tracking the life-cycle of the content.
      # Allows filtering of questionnaires that are appropriate for use versus not.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/publication-status'=>[ 'draft', 'active', 'retired', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'Questionnaire.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/publication-status'}
      },
      ##
      # For testing purposes, not real usage
      # A Boolean value to indicate that this questionnaire is authored for testing purposes (or education/evaluation/marketing) and is not intended to be used for genuine usage.
      # Allows filtering of questionnaires that are appropriate for use versus not.
      'experimental' => {
        'type'=>'boolean',
        'path'=>'Questionnaire.experimental',
        'min'=>0,
        'max'=>1
      },
      ##
      # Resource that can be subject of QuestionnaireResponse
      # The types of subjects that can be the subject of responses created for the questionnaire.
      # If none are specified, then the subject is unlimited.
      'subjectType' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/resource-types'=>[ 'Account', 'ActivityDefinition', 'AdverseEvent', 'AllergyIntolerance', 'Appointment', 'AppointmentResponse', 'AuditEvent', 'Basic', 'Binary', 'BiologicallyDerivedProduct', 'BodyStructure', 'Bundle', 'CapabilityStatement', 'CarePlan', 'CareTeam', 'CatalogEntry', 'ChargeItem', 'ChargeItemDefinition', 'Claim', 'ClaimResponse', 'ClinicalImpression', 'CodeSystem', 'Communication', 'CommunicationRequest', 'CompartmentDefinition', 'Composition', 'ConceptMap', 'Condition', 'Consent', 'Contract', 'Coverage', 'CoverageEligibilityRequest', 'CoverageEligibilityResponse', 'DetectedIssue', 'Device', 'DeviceDefinition', 'DeviceMetric', 'DeviceRequest', 'DeviceUseStatement', 'DiagnosticReport', 'DocumentManifest', 'DocumentReference', 'DomainResource', 'EffectEvidenceSynthesis', 'Encounter', 'Endpoint', 'EnrollmentRequest', 'EnrollmentResponse', 'EpisodeOfCare', 'EventDefinition', 'Evidence', 'EvidenceVariable', 'ExampleScenario', 'ExplanationOfBenefit', 'FamilyMemberHistory', 'Flag', 'Goal', 'GraphDefinition', 'Group', 'GuidanceResponse', 'HealthcareService', 'ImagingStudy', 'Immunization', 'ImmunizationEvaluation', 'ImmunizationRecommendation', 'ImplementationGuide', 'InsurancePlan', 'Invoice', 'Library', 'Linkage', 'List', 'Location', 'Measure', 'MeasureReport', 'Media', 'Medication', 'MedicationAdministration', 'MedicationDispense', 'MedicationKnowledge', 'MedicationRequest', 'MedicationStatement', 'MedicinalProduct', 'MedicinalProductAuthorization', 'MedicinalProductContraindication', 'MedicinalProductIndication', 'MedicinalProductIngredient', 'MedicinalProductInteraction', 'MedicinalProductManufactured', 'MedicinalProductPackaged', 'MedicinalProductPharmaceutical', 'MedicinalProductUndesirableEffect', 'MessageDefinition', 'MessageHeader', 'MolecularSequence', 'NamingSystem', 'NutritionOrder', 'Observation', 'ObservationDefinition', 'OperationDefinition', 'OperationOutcome', 'Organization', 'OrganizationAffiliation', 'Parameters', 'Patient', 'PaymentNotice', 'PaymentReconciliation', 'Person', 'PlanDefinition', 'Practitioner', 'PractitionerRole', 'Procedure', 'Provenance', 'Questionnaire', 'QuestionnaireResponse', 'RelatedPerson', 'RequestGroup', 'ResearchDefinition', 'ResearchElementDefinition', 'ResearchStudy', 'ResearchSubject', 'Resource', 'RiskAssessment', 'RiskEvidenceSynthesis', 'Schedule', 'SearchParameter', 'ServiceRequest', 'Slot', 'Specimen', 'SpecimenDefinition', 'StructureDefinition', 'StructureMap', 'Subscription', 'Substance', 'SubstanceNucleicAcid', 'SubstancePolymer', 'SubstanceProtein', 'SubstanceReferenceInformation', 'SubstanceSourceMaterial', 'SubstanceSpecification', 'SupplyDelivery', 'SupplyRequest', 'Task', 'TerminologyCapabilities', 'TestReport', 'TestScript', 'ValueSet', 'VerificationResult', 'VisionPrescription' ]
        },
        'type'=>'code',
        'path'=>'Questionnaire.subjectType',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/resource-types'}
      },
      ##
      # Date last changed
      # The date  (and optionally time) when the questionnaire was published. The date must change when the business version changes and it must change if the status code changes. In addition, it should change when the substantive content of the questionnaire changes.
      # Note that this is not the same as the resource last-modified-date, since the resource may be a secondary representation of the questionnaire. Additional specific dates may be added as extensions or be found by consulting Provenances associated with past versions of the resource.
      'date' => {
        'type'=>'dateTime',
        'path'=>'Questionnaire.date',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name of the publisher (organization or individual)
      # The name of the organization or individual that published the questionnaire.
      # Usually an organization but may be an individual. The publisher (or steward) of the questionnaire is the organization or individual primarily responsible for the maintenance and upkeep of the questionnaire. This is not necessarily the same individual or organization that developed and initially authored the content. The publisher is the primary point of contact for questions or issues with the questionnaire. This item SHOULD be populated unless the information is available from context.
      'publisher' => {
        'type'=>'string',
        'path'=>'Questionnaire.publisher',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contact details for the publisher
      # Contact details to assist a user in finding and communicating with the publisher.
      # May be a web site, an email address, a telephone number, etc.
      'contact' => {
        'type'=>'ContactDetail',
        'path'=>'Questionnaire.contact',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Natural language description of the questionnaire
      # A free text natural language description of the questionnaire from a consumer's perspective.
      # This description can be used to capture details such as why the questionnaire was built, comments about misuse, instructions for clinical use and interpretation, literature references, examples from the paper world, etc. It is not a rendering of the questionnaire as conveyed in the 'text' field of the resource itself. This item SHOULD be populated unless the information is available from context (e.g. the language of the questionnaire is presumed to be the predominant language in the place the questionnaire was created).
      'description' => {
        'type'=>'markdown',
        'path'=>'Questionnaire.description',
        'min'=>0,
        'max'=>1
      },
      ##
      # The context that the content is intended to support
      # The content was developed with a focus and intent of supporting the contexts that are listed. These contexts may be general categories (gender, age, ...) or may be references to specific programs (insurance plans, studies, ...) and may be used to assist with indexing and searching for appropriate questionnaire instances.
      # When multiple useContexts are specified, there is no expectation that all or any of the contexts apply.
      'useContext' => {
        'type'=>'UsageContext',
        'path'=>'Questionnaire.useContext',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Intended jurisdiction for questionnaire (if applicable)
      # A legal or geographic region in which the questionnaire is intended to be used.
      # It may be possible for the questionnaire to be used in jurisdictions other than those for which it was originally designed or intended.
      'jurisdiction' => {
        'type'=>'CodeableConcept',
        'path'=>'Questionnaire.jurisdiction',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Why this questionnaire is defined
      # Explanation of why this questionnaire is needed and why it has been designed as it has.
      # This element does not describe the usage of the questionnaire. Instead, it provides traceability of ''why'' the resource is either needed or ''why'' it is defined as it is.  This may be used to point to source materials or specifications that drove the structure of this questionnaire.
      'purpose' => {
        'type'=>'markdown',
        'path'=>'Questionnaire.purpose',
        'min'=>0,
        'max'=>1
      },
      ##
      # Use and/or publishing restrictions
      # A copyright statement relating to the questionnaire and/or its contents. Copyright statements are generally legal restrictions on the use and publishing of the questionnaire.
      'copyright' => {
        'type'=>'markdown',
        'path'=>'Questionnaire.copyright',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the questionnaire was approved by publisher
      # The date on which the resource content was approved by the publisher. Approval happens once when the content is officially approved for usage.
      # The 'date' element may be more recent than the approval date because of minor changes or editorial corrections.
      'approvalDate' => {
        'type'=>'date',
        'path'=>'Questionnaire.approvalDate',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the questionnaire was last reviewed
      # The date on which the resource content was last reviewed. Review happens periodically after approval but does not change the original approval date.
      # If specified, this date follows the original approval date.
      'lastReviewDate' => {
        'type'=>'date',
        'path'=>'Questionnaire.lastReviewDate',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the questionnaire is expected to be used
      # The period during which the questionnaire content was or is planned to be in active use.
      # The effective period for a questionnaire  determines when the content is applicable for usage and is independent of publication and review dates. For example, a measure intended to be used for the year 2016 might be published in 2015.
      'effectivePeriod' => {
        'type'=>'Period',
        'path'=>'Questionnaire.effectivePeriod',
        'min'=>0,
        'max'=>1
      },
      ##
      # Concept that represents the overall questionnaire
      # An identifier for this question or group of questions in a particular terminology such as LOINC.
      'code' => {
        'type'=>'Coding',
        'path'=>'Questionnaire.code',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Questions and sections within the Questionnaire
      # A particular question, question grouping or display text that is part of the questionnaire.
      # The content of the questionnaire is constructed from an ordered, hierarchical collection of items.
      'item' => {
        'type'=>'Questionnaire::Item',
        'path'=>'Questionnaire.item',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Questions and sections within the Questionnaire
    # A particular question, question grouping or display text that is part of the questionnaire.
    # The content of the questionnaire is constructed from an ordered, hierarchical collection of items.
    class Item < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Item.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Item.extension',
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
          'path'=>'Item.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Unique id for item in questionnaire
        # An identifier that is unique within the Questionnaire allowing linkage to the equivalent item in a QuestionnaireResponse resource.
        # This ''can'' be a meaningful identifier (e.g. a LOINC code) but is not intended to have any meaning.  GUIDs or sequential numbers are appropriate here.
        'linkId' => {
          'type'=>'string',
          'path'=>'Item.linkId',
          'min'=>1,
          'max'=>1
        },
        ##
        # ElementDefinition - details for the item
        # This element is a URI that refers to an [ElementDefinition](elementdefinition.html) that provides information about this item, including information that might otherwise be included in the instance of the Questionnaire resource. A detailed description of the construction of the URI is shown in Comments, below. If this element is present then the following element values MAY be derived from the Element Definition if the corresponding elements of this Questionnaire resource instance have no value:
        # 
        # * code (ElementDefinition.code) 
        # * type (ElementDefinition.type) 
        # * required (ElementDefinition.min) 
        # * repeats (ElementDefinition.max) 
        # * maxLength (ElementDefinition.maxLength) 
        # * answerValueSet (ElementDefinition.binding)
        # * options (ElementDefinition.binding).
        # The uri refers to an ElementDefinition in a [StructureDefinition](structuredefinition.html#) and always starts with the [canonical URL](references.html#canonical) for the target resource. When referring to a StructureDefinition, a fragment identifier is used to specify the element definition by its id [Element.id](element-definitions.html#Element.id). E.g. http://hl7.org/fhir/StructureDefinition/Observation#Observation.value[x]. In the absence of a fragment identifier, the first/root element definition in the target is the matching element definition.
        'definition' => {
          'type'=>'uri',
          'path'=>'Item.definition',
          'min'=>0,
          'max'=>1
        },
        ##
        # Corresponding concept for this item in a terminology
        # A terminology code that corresponds to this group or question (e.g. a code from LOINC, which defines many questions and answers).
        # The value may come from the ElementDefinition referred to by .definition.
        'code' => {
          'type'=>'Coding',
          'path'=>'Item.code',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # E.g. "1(a)", "2.5.3"
        # A short label for a particular group, question or set of display text within the questionnaire used for reference by the individual completing the questionnaire.
        # These are generally unique within a questionnaire, though this is not guaranteed. Some questionnaires may have multiple questions with the same label with logic to control which gets exposed.  Typically, these won't be used for "display" items, though such use is not prohibited.  Systems SHOULD NOT generate their own prefixes if prefixes are defined for any items within a Questionnaire.
        'prefix' => {
          'type'=>'string',
          'path'=>'Item.prefix',
          'min'=>0,
          'max'=>1
        },
        ##
        # Primary text for the item
        # The name of a section, the text of a question or text content for a display item.
        # When using this element to represent the name of a section, use group type item and also make sure to limit the text element to a short string suitable for display as a section heading.  Group item instructions should be included as a display type item within the group.
        'text' => {
          'type'=>'string',
          'path'=>'Item.text',
          'min'=>0,
          'max'=>1
        },
        ##
        # group | display | boolean | decimal | integer | date | dateTime +
        # The type of questionnaire item this is - whether text for display, a grouping of other items or a particular type of data to be captured (string, integer, coded choice, etc.).
        # Additional constraints on the type of answer can be conveyed by extensions. The value may come from the ElementDefinition referred to by .definition.
        'type' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/item-type'=>[ 'group', 'display', 'boolean', 'decimal', 'integer', 'date', 'dateTime', 'time', 'string', 'text', 'url', 'choice', 'open-choice', 'attachment', 'reference', 'quantity' ]
          },
          'type'=>'code',
          'path'=>'Item.type',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/item-type'}
        },
        ##
        # Only allow data when
        # A constraint indicating that this item should only be enabled (displayed/allow answers to be captured) when the specified condition is true.
        # If multiple repetitions of this extension are present, the item should be enabled when the condition for *any* of the repetitions is true.  I.e. treat "enableWhen"s as being joined by an "or" clause.  This element is a modifier because if enableWhen is present for an item, "required" is ignored unless one of the enableWhen conditions is met. When an item is disabled, all of its descendants are disabled, regardless of what their own enableWhen logic might evaluate to.
        'enableWhen' => {
          'type'=>'Questionnaire::Item::EnableWhen',
          'path'=>'Item.enableWhen',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # all | any
        # Controls how multiple enableWhen values are interpreted -  whether all or any must be true.
        # This element must be specified if more than one enableWhen value is provided.
        'enableBehavior' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/questionnaire-enable-behavior'=>[ 'all', 'any' ]
          },
          'type'=>'code',
          'path'=>'Item.enableBehavior',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/questionnaire-enable-behavior'}
        },
        ##
        # Whether the item must be included in data results
        # An indication, if true, that the item must be present in a "completed" QuestionnaireResponse.  If false, the item may be skipped when answering the questionnaire.
        # Questionnaire.item.required only has meaning for elements that are conditionally enabled with enableWhen if the condition evaluates to true.  If an item that contains other items is marked as required, that does not automatically make the contained elements required (though required groups must contain at least one child element). The value may come from the ElementDefinition referred to by .definition.
        'required' => {
          'type'=>'boolean',
          'path'=>'Item.required',
          'min'=>0,
          'max'=>1
        },
        ##
        # Whether the item may repeat
        # An indication, if true, that the item may occur multiple times in the response, collecting multiple answers for questions or multiple sets of answers for groups.
        # If a question is marked as repeats=true, then multiple answers can be provided for the question in the corresponding QuestionnaireResponse.  When rendering the questionnaire, it is up to the rendering software whether to render the question text for each answer repetition (i.e. "repeat the question") or to simply allow entry/selection of multiple answers for the question (repeat the answers).  Which is most appropriate visually may depend on the type of answer as well as whether there are nested items.
        # 
        # The resulting QuestionnaireResponse will be populated the same way regardless of rendering - one 'question' item with multiple answer values.
        # 
        #  The value may come from the ElementDefinition referred to by .definition.
        'repeats' => {
          'type'=>'boolean',
          'path'=>'Item.repeats',
          'min'=>0,
          'max'=>1
        },
        ##
        # Don't allow human editing
        # An indication, when true, that the value cannot be changed by a human respondent to the Questionnaire.
        # The value of readOnly elements can be established by asserting  extensions for defaultValues, linkages that support pre-population and/or extensions that support calculation based on other answers.
        'readOnly' => {
          'type'=>'boolean',
          'path'=>'Item.readOnly',
          'min'=>0,
          'max'=>1
        },
        ##
        # No more than this many characters
        # The maximum number of characters that are permitted in the answer to be considered a "valid" QuestionnaireResponse.
        # For base64binary, reflects the number of characters representing the encoded data, not the number of bytes of the binary data. The value may come from the ElementDefinition referred to by .definition.
        'maxLength' => {
          'type'=>'integer',
          'path'=>'Item.maxLength',
          'min'=>0,
          'max'=>1
        },
        ##
        # Valueset containing permitted answers
        # A reference to a value set containing a list of codes representing permitted answers for a "choice" or "open-choice" question.
        # LOINC defines many useful value sets for questionnaire responses. See [LOINC Answer Lists](loinc.html#alist). The value may come from the ElementDefinition referred to by .definition.
        'answerValueSet' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ValueSet'],
          'type'=>'canonical',
          'path'=>'Item.answerValueSet',
          'min'=>0,
          'max'=>1
        },
        ##
        # Permitted answer
        # One of the permitted answers for a "choice" or "open-choice" question.
        # This element can be used when the value set machinery of answerValueSet is deemed too cumbersome or when there's a need to capture possible answers that are not codes.
        'answerOption' => {
          'type'=>'Questionnaire::Item::AnswerOption',
          'path'=>'Item.answerOption',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Initial value(s) when item is first rendered
        # One or more values that should be pre-populated in the answer when initially rendering the questionnaire for user input.
        # The user is allowed to change the value and override the default (unless marked as read-only). If the user doesn't change the value, then this initial value will be persisted when the QuestionnaireResponse is initially created.  Note that initial values can influence results.  The data type of initial[x] must agree with the item.type, and only repeating items can have more then one initial value.
        'initial' => {
          'type'=>'Questionnaire::Item::Initial',
          'path'=>'Item.initial',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Nested questionnaire items
        # Text, questions and other groups to be nested beneath a question or group.
        # There is no specified limit to the depth of nesting.  However, Questionnaire authors are encouraged to consider the impact on the user and user interface of overly deep nesting.
        'item' => {
          'type'=>'Questionnaire::Item',
          'path'=>'Item.item',
          'min'=>0,
          'max'=>Float::INFINITY
        }
      }

      ##
      # Only allow data when
      # A constraint indicating that this item should only be enabled (displayed/allow answers to be captured) when the specified condition is true.
      # If multiple repetitions of this extension are present, the item should be enabled when the condition for *any* of the repetitions is true.  I.e. treat "enableWhen"s as being joined by an "or" clause.  This element is a modifier because if enableWhen is present for an item, "required" is ignored unless one of the enableWhen conditions is met. When an item is disabled, all of its descendants are disabled, regardless of what their own enableWhen logic might evaluate to.
      class EnableWhen < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        MULTIPLE_TYPES = {
          'answer[x]' => ['boolean', 'Coding', 'date', 'dateTime', 'decimal', 'integer', 'Quantity', 'Reference', 'string', 'time']
        }
        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'id',
            'path'=>'EnableWhen.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'EnableWhen.extension',
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
            'path'=>'EnableWhen.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Question that determines whether item is enabled
          # The linkId for the question whose answer (or lack of answer) governs whether this item is enabled.
          # If multiple question occurrences are present for the same question (same linkId), then this refers to the nearest question occurrence reachable by tracing first the "ancestor" axis and then the "preceding" axis and then the "following" axis.
          'question' => {
            'type'=>'string',
            'path'=>'EnableWhen.question',
            'min'=>1,
            'max'=>1
          },
          ##
          # exists | = | != | > | < | >= | <=
          # Specifies the criteria by which the question is enabled.
          'operator' => {
            'valid_codes'=>{
              'http://hl7.org/fhir/questionnaire-enable-operator'=>[ 'exists', '=', '!=', '>', '<', '>=', '<=' ]
            },
            'type'=>'code',
            'path'=>'EnableWhen.operator',
            'min'=>1,
            'max'=>1,
            'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/questionnaire-enable-operator'}
          },
          ##
          # Value for question comparison based on operator
          # A value that the referenced question is tested using the specified operator in order for the item to be enabled.
          'answerBoolean' => {
            'type'=>'Boolean',
            'path'=>'EnableWhen.answer[x]',
            'min'=>1,
            'max'=>1
          }
          ##
          # Value for question comparison based on operator
          # A value that the referenced question is tested using the specified operator in order for the item to be enabled.
          'answerCoding' => {
            'type'=>'Coding',
            'path'=>'EnableWhen.answer[x]',
            'min'=>1,
            'max'=>1
          }
          ##
          # Value for question comparison based on operator
          # A value that the referenced question is tested using the specified operator in order for the item to be enabled.
          'answerDate' => {
            'type'=>'Date',
            'path'=>'EnableWhen.answer[x]',
            'min'=>1,
            'max'=>1
          }
          ##
          # Value for question comparison based on operator
          # A value that the referenced question is tested using the specified operator in order for the item to be enabled.
          'answerDateTime' => {
            'type'=>'DateTime',
            'path'=>'EnableWhen.answer[x]',
            'min'=>1,
            'max'=>1
          }
          ##
          # Value for question comparison based on operator
          # A value that the referenced question is tested using the specified operator in order for the item to be enabled.
          'answerDecimal' => {
            'type'=>'Decimal',
            'path'=>'EnableWhen.answer[x]',
            'min'=>1,
            'max'=>1
          }
          ##
          # Value for question comparison based on operator
          # A value that the referenced question is tested using the specified operator in order for the item to be enabled.
          'answerInteger' => {
            'type'=>'Integer',
            'path'=>'EnableWhen.answer[x]',
            'min'=>1,
            'max'=>1
          }
          ##
          # Value for question comparison based on operator
          # A value that the referenced question is tested using the specified operator in order for the item to be enabled.
          'answerQuantity' => {
            'type'=>'Quantity',
            'path'=>'EnableWhen.answer[x]',
            'min'=>1,
            'max'=>1
          }
          ##
          # Value for question comparison based on operator
          # A value that the referenced question is tested using the specified operator in order for the item to be enabled.
          'answerReference' => {
            'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
            'type'=>'Reference',
            'path'=>'EnableWhen.answer[x]',
            'min'=>1,
            'max'=>1
          }
          ##
          # Value for question comparison based on operator
          # A value that the referenced question is tested using the specified operator in order for the item to be enabled.
          'answerString' => {
            'type'=>'String',
            'path'=>'EnableWhen.answer[x]',
            'min'=>1,
            'max'=>1
          }
          ##
          # Value for question comparison based on operator
          # A value that the referenced question is tested using the specified operator in order for the item to be enabled.
          'answerTime' => {
            'type'=>'Time',
            'path'=>'EnableWhen.answer[x]',
            'min'=>1,
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
        # Question that determines whether item is enabled
        # The linkId for the question whose answer (or lack of answer) governs whether this item is enabled.
        # If multiple question occurrences are present for the same question (same linkId), then this refers to the nearest question occurrence reachable by tracing first the "ancestor" axis and then the "preceding" axis and then the "following" axis.
        attr_accessor :question                       # 1-1 string
        ##
        # exists | = | != | > | < | >= | <=
        # Specifies the criteria by which the question is enabled.
        attr_accessor :operator                       # 1-1 code
        ##
        # Value for question comparison based on operator
        # A value that the referenced question is tested using the specified operator in order for the item to be enabled.
        attr_accessor :answerBoolean                  # 1-1 Boolean
        ##
        # Value for question comparison based on operator
        # A value that the referenced question is tested using the specified operator in order for the item to be enabled.
        attr_accessor :answerCoding                   # 1-1 Coding
        ##
        # Value for question comparison based on operator
        # A value that the referenced question is tested using the specified operator in order for the item to be enabled.
        attr_accessor :answerDate                     # 1-1 Date
        ##
        # Value for question comparison based on operator
        # A value that the referenced question is tested using the specified operator in order for the item to be enabled.
        attr_accessor :answerDateTime                 # 1-1 DateTime
        ##
        # Value for question comparison based on operator
        # A value that the referenced question is tested using the specified operator in order for the item to be enabled.
        attr_accessor :answerDecimal                  # 1-1 Decimal
        ##
        # Value for question comparison based on operator
        # A value that the referenced question is tested using the specified operator in order for the item to be enabled.
        attr_accessor :answerInteger                  # 1-1 Integer
        ##
        # Value for question comparison based on operator
        # A value that the referenced question is tested using the specified operator in order for the item to be enabled.
        attr_accessor :answerQuantity                 # 1-1 Quantity
        ##
        # Value for question comparison based on operator
        # A value that the referenced question is tested using the specified operator in order for the item to be enabled.
        attr_accessor :answerReference                # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Resource)
        ##
        # Value for question comparison based on operator
        # A value that the referenced question is tested using the specified operator in order for the item to be enabled.
        attr_accessor :answerString                   # 1-1 String
        ##
        # Value for question comparison based on operator
        # A value that the referenced question is tested using the specified operator in order for the item to be enabled.
        attr_accessor :answerTime                     # 1-1 Time
      end

      ##
      # Permitted answer
      # One of the permitted answers for a "choice" or "open-choice" question.
      # This element can be used when the value set machinery of answerValueSet is deemed too cumbersome or when there's a need to capture possible answers that are not codes.
      class AnswerOption < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        MULTIPLE_TYPES = {
          'value[x]' => ['Coding', 'date', 'integer', 'Reference', 'string', 'time']
        }
        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'id',
            'path'=>'AnswerOption.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'AnswerOption.extension',
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
            'path'=>'AnswerOption.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Answer value
          # A potential answer that's allowed as the answer to this question.
          # The data type of the value must agree with the item.type.
          'valueCoding' => {
            'type'=>'Coding',
            'path'=>'AnswerOption.value[x]',
            'min'=>1,
            'max'=>1
          },
          ##
          # Answer value
          # A potential answer that's allowed as the answer to this question.
          # The data type of the value must agree with the item.type.
          'valueDate' => {
            'type'=>'Date',
            'path'=>'AnswerOption.value[x]',
            'min'=>1,
            'max'=>1
          },
          ##
          # Answer value
          # A potential answer that's allowed as the answer to this question.
          # The data type of the value must agree with the item.type.
          'valueInteger' => {
            'type'=>'Integer',
            'path'=>'AnswerOption.value[x]',
            'min'=>1,
            'max'=>1
          },
          ##
          # Answer value
          # A potential answer that's allowed as the answer to this question.
          # The data type of the value must agree with the item.type.
          'valueReference' => {
            'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
            'type'=>'Reference',
            'path'=>'AnswerOption.value[x]',
            'min'=>1,
            'max'=>1
          },
          ##
          # Answer value
          # A potential answer that's allowed as the answer to this question.
          # The data type of the value must agree with the item.type.
          'valueString' => {
            'type'=>'String',
            'path'=>'AnswerOption.value[x]',
            'min'=>1,
            'max'=>1
          },
          ##
          # Answer value
          # A potential answer that's allowed as the answer to this question.
          # The data type of the value must agree with the item.type.
          'valueTime' => {
            'type'=>'Time',
            'path'=>'AnswerOption.value[x]',
            'min'=>1,
            'max'=>1
          },
          ##
          # Whether option is selected by default
          # Indicates whether the answer value is selected when the list of possible answers is initially shown.
          # Use this instead of initial[v] if answerValueSet is present.
          'initialSelected' => {
            'type'=>'boolean',
            'path'=>'AnswerOption.initialSelected',
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
        # Answer value
        # A potential answer that's allowed as the answer to this question.
        # The data type of the value must agree with the item.type.
        attr_accessor :valueCoding                    # 1-1 Coding
        ##
        # Answer value
        # A potential answer that's allowed as the answer to this question.
        # The data type of the value must agree with the item.type.
        attr_accessor :valueDate                      # 1-1 Date
        ##
        # Answer value
        # A potential answer that's allowed as the answer to this question.
        # The data type of the value must agree with the item.type.
        attr_accessor :valueInteger                   # 1-1 Integer
        ##
        # Answer value
        # A potential answer that's allowed as the answer to this question.
        # The data type of the value must agree with the item.type.
        attr_accessor :valueReference                 # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Resource)
        ##
        # Answer value
        # A potential answer that's allowed as the answer to this question.
        # The data type of the value must agree with the item.type.
        attr_accessor :valueString                    # 1-1 String
        ##
        # Answer value
        # A potential answer that's allowed as the answer to this question.
        # The data type of the value must agree with the item.type.
        attr_accessor :valueTime                      # 1-1 Time
        ##
        # Whether option is selected by default
        # Indicates whether the answer value is selected when the list of possible answers is initially shown.
        # Use this instead of initial[v] if answerValueSet is present.
        attr_accessor :initialSelected                # 0-1 boolean
      end

      ##
      # Initial value(s) when item is first rendered
      # One or more values that should be pre-populated in the answer when initially rendering the questionnaire for user input.
      # The user is allowed to change the value and override the default (unless marked as read-only). If the user doesn't change the value, then this initial value will be persisted when the QuestionnaireResponse is initially created.  Note that initial values can influence results.  The data type of initial[x] must agree with the item.type, and only repeating items can have more then one initial value.
      class Initial < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        MULTIPLE_TYPES = {
          'value[x]' => ['Attachment', 'boolean', 'Coding', 'date', 'dateTime', 'decimal', 'integer', 'Quantity', 'Reference', 'string', 'time', 'uri']
        }
        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'id',
            'path'=>'Initial.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Initial.extension',
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
            'path'=>'Initial.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Actual value for initializing the question
          # The actual value to for an initial answer.
          # The type of the initial value must be consistent with the type of the item.
          'valueAttachment' => {
            'type'=>'Attachment',
            'path'=>'Initial.value[x]',
            'min'=>1,
            'max'=>1
          }
          ##
          # Actual value for initializing the question
          # The actual value to for an initial answer.
          # The type of the initial value must be consistent with the type of the item.
          'valueBoolean' => {
            'type'=>'Boolean',
            'path'=>'Initial.value[x]',
            'min'=>1,
            'max'=>1
          }
          ##
          # Actual value for initializing the question
          # The actual value to for an initial answer.
          # The type of the initial value must be consistent with the type of the item.
          'valueCoding' => {
            'type'=>'Coding',
            'path'=>'Initial.value[x]',
            'min'=>1,
            'max'=>1
          }
          ##
          # Actual value for initializing the question
          # The actual value to for an initial answer.
          # The type of the initial value must be consistent with the type of the item.
          'valueDate' => {
            'type'=>'Date',
            'path'=>'Initial.value[x]',
            'min'=>1,
            'max'=>1
          }
          ##
          # Actual value for initializing the question
          # The actual value to for an initial answer.
          # The type of the initial value must be consistent with the type of the item.
          'valueDateTime' => {
            'type'=>'DateTime',
            'path'=>'Initial.value[x]',
            'min'=>1,
            'max'=>1
          }
          ##
          # Actual value for initializing the question
          # The actual value to for an initial answer.
          # The type of the initial value must be consistent with the type of the item.
          'valueDecimal' => {
            'type'=>'Decimal',
            'path'=>'Initial.value[x]',
            'min'=>1,
            'max'=>1
          }
          ##
          # Actual value for initializing the question
          # The actual value to for an initial answer.
          # The type of the initial value must be consistent with the type of the item.
          'valueInteger' => {
            'type'=>'Integer',
            'path'=>'Initial.value[x]',
            'min'=>1,
            'max'=>1
          }
          ##
          # Actual value for initializing the question
          # The actual value to for an initial answer.
          # The type of the initial value must be consistent with the type of the item.
          'valueQuantity' => {
            'type'=>'Quantity',
            'path'=>'Initial.value[x]',
            'min'=>1,
            'max'=>1
          }
          ##
          # Actual value for initializing the question
          # The actual value to for an initial answer.
          # The type of the initial value must be consistent with the type of the item.
          'valueReference' => {
            'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
            'type'=>'Reference',
            'path'=>'Initial.value[x]',
            'min'=>1,
            'max'=>1
          }
          ##
          # Actual value for initializing the question
          # The actual value to for an initial answer.
          # The type of the initial value must be consistent with the type of the item.
          'valueString' => {
            'type'=>'String',
            'path'=>'Initial.value[x]',
            'min'=>1,
            'max'=>1
          }
          ##
          # Actual value for initializing the question
          # The actual value to for an initial answer.
          # The type of the initial value must be consistent with the type of the item.
          'valueTime' => {
            'type'=>'Time',
            'path'=>'Initial.value[x]',
            'min'=>1,
            'max'=>1
          }
          ##
          # Actual value for initializing the question
          # The actual value to for an initial answer.
          # The type of the initial value must be consistent with the type of the item.
          'valueUri' => {
            'type'=>'Uri',
            'path'=>'Initial.value[x]',
            'min'=>1,
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
        # Actual value for initializing the question
        # The actual value to for an initial answer.
        # The type of the initial value must be consistent with the type of the item.
        attr_accessor :valueAttachment                # 1-1 Attachment
        ##
        # Actual value for initializing the question
        # The actual value to for an initial answer.
        # The type of the initial value must be consistent with the type of the item.
        attr_accessor :valueBoolean                   # 1-1 Boolean
        ##
        # Actual value for initializing the question
        # The actual value to for an initial answer.
        # The type of the initial value must be consistent with the type of the item.
        attr_accessor :valueCoding                    # 1-1 Coding
        ##
        # Actual value for initializing the question
        # The actual value to for an initial answer.
        # The type of the initial value must be consistent with the type of the item.
        attr_accessor :valueDate                      # 1-1 Date
        ##
        # Actual value for initializing the question
        # The actual value to for an initial answer.
        # The type of the initial value must be consistent with the type of the item.
        attr_accessor :valueDateTime                  # 1-1 DateTime
        ##
        # Actual value for initializing the question
        # The actual value to for an initial answer.
        # The type of the initial value must be consistent with the type of the item.
        attr_accessor :valueDecimal                   # 1-1 Decimal
        ##
        # Actual value for initializing the question
        # The actual value to for an initial answer.
        # The type of the initial value must be consistent with the type of the item.
        attr_accessor :valueInteger                   # 1-1 Integer
        ##
        # Actual value for initializing the question
        # The actual value to for an initial answer.
        # The type of the initial value must be consistent with the type of the item.
        attr_accessor :valueQuantity                  # 1-1 Quantity
        ##
        # Actual value for initializing the question
        # The actual value to for an initial answer.
        # The type of the initial value must be consistent with the type of the item.
        attr_accessor :valueReference                 # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Resource)
        ##
        # Actual value for initializing the question
        # The actual value to for an initial answer.
        # The type of the initial value must be consistent with the type of the item.
        attr_accessor :valueString                    # 1-1 String
        ##
        # Actual value for initializing the question
        # The actual value to for an initial answer.
        # The type of the initial value must be consistent with the type of the item.
        attr_accessor :valueTime                      # 1-1 Time
        ##
        # Actual value for initializing the question
        # The actual value to for an initial answer.
        # The type of the initial value must be consistent with the type of the item.
        attr_accessor :valueUri                       # 1-1 Uri
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
      # Unique id for item in questionnaire
      # An identifier that is unique within the Questionnaire allowing linkage to the equivalent item in a QuestionnaireResponse resource.
      # This ''can'' be a meaningful identifier (e.g. a LOINC code) but is not intended to have any meaning.  GUIDs or sequential numbers are appropriate here.
      attr_accessor :linkId                         # 1-1 string
      ##
      # ElementDefinition - details for the item
      # This element is a URI that refers to an [ElementDefinition](elementdefinition.html) that provides information about this item, including information that might otherwise be included in the instance of the Questionnaire resource. A detailed description of the construction of the URI is shown in Comments, below. If this element is present then the following element values MAY be derived from the Element Definition if the corresponding elements of this Questionnaire resource instance have no value:
      # 
      # * code (ElementDefinition.code) 
      # * type (ElementDefinition.type) 
      # * required (ElementDefinition.min) 
      # * repeats (ElementDefinition.max) 
      # * maxLength (ElementDefinition.maxLength) 
      # * answerValueSet (ElementDefinition.binding)
      # * options (ElementDefinition.binding).
      # The uri refers to an ElementDefinition in a [StructureDefinition](structuredefinition.html#) and always starts with the [canonical URL](references.html#canonical) for the target resource. When referring to a StructureDefinition, a fragment identifier is used to specify the element definition by its id [Element.id](element-definitions.html#Element.id). E.g. http://hl7.org/fhir/StructureDefinition/Observation#Observation.value[x]. In the absence of a fragment identifier, the first/root element definition in the target is the matching element definition.
      attr_accessor :definition                     # 0-1 uri
      ##
      # Corresponding concept for this item in a terminology
      # A terminology code that corresponds to this group or question (e.g. a code from LOINC, which defines many questions and answers).
      # The value may come from the ElementDefinition referred to by .definition.
      attr_accessor :code                           # 0-* [ Coding ]
      ##
      # E.g. "1(a)", "2.5.3"
      # A short label for a particular group, question or set of display text within the questionnaire used for reference by the individual completing the questionnaire.
      # These are generally unique within a questionnaire, though this is not guaranteed. Some questionnaires may have multiple questions with the same label with logic to control which gets exposed.  Typically, these won't be used for "display" items, though such use is not prohibited.  Systems SHOULD NOT generate their own prefixes if prefixes are defined for any items within a Questionnaire.
      attr_accessor :prefix                         # 0-1 string
      ##
      # Primary text for the item
      # The name of a section, the text of a question or text content for a display item.
      # When using this element to represent the name of a section, use group type item and also make sure to limit the text element to a short string suitable for display as a section heading.  Group item instructions should be included as a display type item within the group.
      attr_accessor :text                           # 0-1 string
      ##
      # group | display | boolean | decimal | integer | date | dateTime +
      # The type of questionnaire item this is - whether text for display, a grouping of other items or a particular type of data to be captured (string, integer, coded choice, etc.).
      # Additional constraints on the type of answer can be conveyed by extensions. The value may come from the ElementDefinition referred to by .definition.
      attr_accessor :type                           # 1-1 code
      ##
      # Only allow data when
      # A constraint indicating that this item should only be enabled (displayed/allow answers to be captured) when the specified condition is true.
      # If multiple repetitions of this extension are present, the item should be enabled when the condition for *any* of the repetitions is true.  I.e. treat "enableWhen"s as being joined by an "or" clause.  This element is a modifier because if enableWhen is present for an item, "required" is ignored unless one of the enableWhen conditions is met. When an item is disabled, all of its descendants are disabled, regardless of what their own enableWhen logic might evaluate to.
      attr_accessor :enableWhen                     # 0-* [ Questionnaire::Item::EnableWhen ]
      ##
      # all | any
      # Controls how multiple enableWhen values are interpreted -  whether all or any must be true.
      # This element must be specified if more than one enableWhen value is provided.
      attr_accessor :enableBehavior                 # 0-1 code
      ##
      # Whether the item must be included in data results
      # An indication, if true, that the item must be present in a "completed" QuestionnaireResponse.  If false, the item may be skipped when answering the questionnaire.
      # Questionnaire.item.required only has meaning for elements that are conditionally enabled with enableWhen if the condition evaluates to true.  If an item that contains other items is marked as required, that does not automatically make the contained elements required (though required groups must contain at least one child element). The value may come from the ElementDefinition referred to by .definition.
      attr_accessor :required                       # 0-1 boolean
      ##
      # Whether the item may repeat
      # An indication, if true, that the item may occur multiple times in the response, collecting multiple answers for questions or multiple sets of answers for groups.
      # If a question is marked as repeats=true, then multiple answers can be provided for the question in the corresponding QuestionnaireResponse.  When rendering the questionnaire, it is up to the rendering software whether to render the question text for each answer repetition (i.e. "repeat the question") or to simply allow entry/selection of multiple answers for the question (repeat the answers).  Which is most appropriate visually may depend on the type of answer as well as whether there are nested items.
      # 
      # The resulting QuestionnaireResponse will be populated the same way regardless of rendering - one 'question' item with multiple answer values.
      # 
      #  The value may come from the ElementDefinition referred to by .definition.
      attr_accessor :repeats                        # 0-1 boolean
      ##
      # Don't allow human editing
      # An indication, when true, that the value cannot be changed by a human respondent to the Questionnaire.
      # The value of readOnly elements can be established by asserting  extensions for defaultValues, linkages that support pre-population and/or extensions that support calculation based on other answers.
      attr_accessor :readOnly                       # 0-1 boolean
      ##
      # No more than this many characters
      # The maximum number of characters that are permitted in the answer to be considered a "valid" QuestionnaireResponse.
      # For base64binary, reflects the number of characters representing the encoded data, not the number of bytes of the binary data. The value may come from the ElementDefinition referred to by .definition.
      attr_accessor :maxLength                      # 0-1 integer
      ##
      # Valueset containing permitted answers
      # A reference to a value set containing a list of codes representing permitted answers for a "choice" or "open-choice" question.
      # LOINC defines many useful value sets for questionnaire responses. See [LOINC Answer Lists](loinc.html#alist). The value may come from the ElementDefinition referred to by .definition.
      attr_accessor :answerValueSet                 # 0-1 canonical(http://hl7.org/fhir/StructureDefinition/ValueSet)
      ##
      # Permitted answer
      # One of the permitted answers for a "choice" or "open-choice" question.
      # This element can be used when the value set machinery of answerValueSet is deemed too cumbersome or when there's a need to capture possible answers that are not codes.
      attr_accessor :answerOption                   # 0-* [ Questionnaire::Item::AnswerOption ]
      ##
      # Initial value(s) when item is first rendered
      # One or more values that should be pre-populated in the answer when initially rendering the questionnaire for user input.
      # The user is allowed to change the value and override the default (unless marked as read-only). If the user doesn't change the value, then this initial value will be persisted when the QuestionnaireResponse is initially created.  Note that initial values can influence results.  The data type of initial[x] must agree with the item.type, and only repeating items can have more then one initial value.
      attr_accessor :initial                        # 0-* [ Questionnaire::Item::Initial ]
      ##
      # Nested questionnaire items
      # Text, questions and other groups to be nested beneath a question or group.
      # There is no specified limit to the depth of nesting.  However, Questionnaire authors are encouraged to consider the impact on the user and user interface of overly deep nesting.
      attr_accessor :item                           # 0-* [ Questionnaire::Item ]
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
    # Canonical identifier for this questionnaire, represented as a URI (globally unique)
    # An absolute URI that is used to identify this questionnaire when it is referenced in a specification, model, design or an instance; also called its canonical identifier. This SHOULD be globally unique and SHOULD be a literal address at which at which an authoritative instance of this questionnaire is (or will be) published. This URL can be the target of a canonical reference. It SHALL remain the same when the questionnaire is stored on different servers.
    # The name of the referenced questionnaire can be conveyed using the http://hl7.org/fhir/StructureDefinition/display extension.
    attr_accessor :url                            # 0-1 uri
    ##
    # Additional identifier for the questionnaire
    # A formal identifier that is used to identify this questionnaire when it is represented in other formats, or referenced in a specification, model, design or an instance.
    # Typically, this is used for identifiers that can go in an HL7 V3 II (instance identifier) data type, and can then identify this questionnaire outside of FHIR, where it is not possible to use the logical URI.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Business version of the questionnaire
    # The identifier that is used to identify this version of the questionnaire when it is referenced in a specification, model, design or instance. This is an arbitrary value managed by the questionnaire author and is not expected to be globally unique. For example, it might be a timestamp (e.g. yyyymmdd) if a managed version is not available. There is also no expectation that versions can be placed in a lexicographical sequence.
    # There may be different questionnaire instances that have the same identifier but different versions.  The version can be appended to the url in a reference to allow a reference to a particular business version of the questionnaire with the format [url]|[version].
    attr_accessor :version                        # 0-1 string
    ##
    # Name for this questionnaire (computer friendly)
    # A natural language name identifying the questionnaire. This name should be usable as an identifier for the module by machine processing applications such as code generation.
    # The name is not expected to be globally unique. The name should be a simple alphanumeric type name to ensure that it is machine-processing friendly.
    attr_accessor :name                           # 0-1 string
    ##
    # Name for this questionnaire (human friendly)
    # A short, descriptive, user-friendly title for the questionnaire.
    # This name does not need to be machine-processing friendly and may contain punctuation, white-space, etc.
    attr_accessor :title                          # 0-1 string
    ##
    # Instantiates protocol or definition
    # The URL of a Questionnaire that this Questionnaire is based on.
    attr_accessor :derivedFrom                    # 0-* [ canonical(http://hl7.org/fhir/StructureDefinition/Questionnaire) ]
    ##
    # draft | active | retired | unknown
    # The status of this questionnaire. Enables tracking the life-cycle of the content.
    # Allows filtering of questionnaires that are appropriate for use versus not.
    attr_accessor :status                         # 1-1 code
    ##
    # For testing purposes, not real usage
    # A Boolean value to indicate that this questionnaire is authored for testing purposes (or education/evaluation/marketing) and is not intended to be used for genuine usage.
    # Allows filtering of questionnaires that are appropriate for use versus not.
    attr_accessor :experimental                   # 0-1 boolean
    ##
    # Resource that can be subject of QuestionnaireResponse
    # The types of subjects that can be the subject of responses created for the questionnaire.
    # If none are specified, then the subject is unlimited.
    attr_accessor :subjectType                    # 0-* [ code ]
    ##
    # Date last changed
    # The date  (and optionally time) when the questionnaire was published. The date must change when the business version changes and it must change if the status code changes. In addition, it should change when the substantive content of the questionnaire changes.
    # Note that this is not the same as the resource last-modified-date, since the resource may be a secondary representation of the questionnaire. Additional specific dates may be added as extensions or be found by consulting Provenances associated with past versions of the resource.
    attr_accessor :date                           # 0-1 dateTime
    ##
    # Name of the publisher (organization or individual)
    # The name of the organization or individual that published the questionnaire.
    # Usually an organization but may be an individual. The publisher (or steward) of the questionnaire is the organization or individual primarily responsible for the maintenance and upkeep of the questionnaire. This is not necessarily the same individual or organization that developed and initially authored the content. The publisher is the primary point of contact for questions or issues with the questionnaire. This item SHOULD be populated unless the information is available from context.
    attr_accessor :publisher                      # 0-1 string
    ##
    # Contact details for the publisher
    # Contact details to assist a user in finding and communicating with the publisher.
    # May be a web site, an email address, a telephone number, etc.
    attr_accessor :contact                        # 0-* [ ContactDetail ]
    ##
    # Natural language description of the questionnaire
    # A free text natural language description of the questionnaire from a consumer's perspective.
    # This description can be used to capture details such as why the questionnaire was built, comments about misuse, instructions for clinical use and interpretation, literature references, examples from the paper world, etc. It is not a rendering of the questionnaire as conveyed in the 'text' field of the resource itself. This item SHOULD be populated unless the information is available from context (e.g. the language of the questionnaire is presumed to be the predominant language in the place the questionnaire was created).
    attr_accessor :description                    # 0-1 markdown
    ##
    # The context that the content is intended to support
    # The content was developed with a focus and intent of supporting the contexts that are listed. These contexts may be general categories (gender, age, ...) or may be references to specific programs (insurance plans, studies, ...) and may be used to assist with indexing and searching for appropriate questionnaire instances.
    # When multiple useContexts are specified, there is no expectation that all or any of the contexts apply.
    attr_accessor :useContext                     # 0-* [ UsageContext ]
    ##
    # Intended jurisdiction for questionnaire (if applicable)
    # A legal or geographic region in which the questionnaire is intended to be used.
    # It may be possible for the questionnaire to be used in jurisdictions other than those for which it was originally designed or intended.
    attr_accessor :jurisdiction                   # 0-* [ CodeableConcept ]
    ##
    # Why this questionnaire is defined
    # Explanation of why this questionnaire is needed and why it has been designed as it has.
    # This element does not describe the usage of the questionnaire. Instead, it provides traceability of ''why'' the resource is either needed or ''why'' it is defined as it is.  This may be used to point to source materials or specifications that drove the structure of this questionnaire.
    attr_accessor :purpose                        # 0-1 markdown
    ##
    # Use and/or publishing restrictions
    # A copyright statement relating to the questionnaire and/or its contents. Copyright statements are generally legal restrictions on the use and publishing of the questionnaire.
    attr_accessor :copyright                      # 0-1 markdown
    ##
    # When the questionnaire was approved by publisher
    # The date on which the resource content was approved by the publisher. Approval happens once when the content is officially approved for usage.
    # The 'date' element may be more recent than the approval date because of minor changes or editorial corrections.
    attr_accessor :approvalDate                   # 0-1 date
    ##
    # When the questionnaire was last reviewed
    # The date on which the resource content was last reviewed. Review happens periodically after approval but does not change the original approval date.
    # If specified, this date follows the original approval date.
    attr_accessor :lastReviewDate                 # 0-1 date
    ##
    # When the questionnaire is expected to be used
    # The period during which the questionnaire content was or is planned to be in active use.
    # The effective period for a questionnaire  determines when the content is applicable for usage and is independent of publication and review dates. For example, a measure intended to be used for the year 2016 might be published in 2015.
    attr_accessor :effectivePeriod                # 0-1 Period
    ##
    # Concept that represents the overall questionnaire
    # An identifier for this question or group of questions in a particular terminology such as LOINC.
    attr_accessor :code                           # 0-* [ Coding ]
    ##
    # Questions and sections within the Questionnaire
    # A particular question, question grouping or display text that is part of the questionnaire.
    # The content of the questionnaire is constructed from an ordered, hierarchical collection of items.
    attr_accessor :item                           # 0-* [ Questionnaire::Item ]

    def resourceType
      'Questionnaire'
    end
  end
end
