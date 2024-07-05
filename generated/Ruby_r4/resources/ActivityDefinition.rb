module FHIR

  ##
  # This resource allows for the definition of some activity to be performed, independent of a particular patient, practitioner, or other performance context.
  class ActivityDefinition < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['composed-of', 'context-quantity', 'context-type-quantity', 'context-type-value', 'context-type', 'context', 'date', 'depends-on', 'derived-from', 'description', 'effective', 'identifier', 'jurisdiction', 'name', 'predecessor', 'publisher', 'status', 'successor', 'title', 'topic', 'url', 'version']
    MULTIPLE_TYPES = {
      'subject[x]' => ['CodeableConcept', 'Reference'],
      'timing[x]' => ['Age', 'dateTime', 'Duration', 'Period', 'Range', 'Timing'],
      'product[x]' => ['CodeableConcept', 'Reference']
    }
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'ActivityDefinition.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'ActivityDefinition.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'ActivityDefinition.implicitRules',
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
        'path'=>'ActivityDefinition.language',
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
        'path'=>'ActivityDefinition.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'ActivityDefinition.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'ActivityDefinition.extension',
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
        'path'=>'ActivityDefinition.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Canonical identifier for this activity definition, represented as a URI (globally unique)
      # An absolute URI that is used to identify this activity definition when it is referenced in a specification, model, design or an instance; also called its canonical identifier. This SHOULD be globally unique and SHOULD be a literal address at which at which an authoritative instance of this activity definition is (or will be) published. This URL can be the target of a canonical reference. It SHALL remain the same when the activity definition is stored on different servers.
      # Can be a urn:uuid: or a urn:oid: but real http: addresses are preferred.  Multiple instances may share the same URL if they have a distinct version.
      # 
      # The determination of when to create a new version of a resource (same url, new version) vs. defining a new artifact is up to the author.  Considerations for making this decision are found in [Technical and Business Versions](resource.html#versions). 
      # 
      # In some cases, the resource can no longer be found at the stated url, but the url itself cannot change. Implementations can use the [meta.source](resource.html#meta) element to indicate where the current master source of the resource can be found.
      'url' => {
        'type'=>'uri',
        'path'=>'ActivityDefinition.url',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional identifier for the activity definition
      # A formal identifier that is used to identify this activity definition when it is represented in other formats, or referenced in a specification, model, design or an instance.
      # Typically, this is used for identifiers that can go in an HL7 V3 II (instance identifier) data type, and can then identify this activity definition outside of FHIR, where it is not possible to use the logical URI.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'ActivityDefinition.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Business version of the activity definition
      # The identifier that is used to identify this version of the activity definition when it is referenced in a specification, model, design or instance. This is an arbitrary value managed by the activity definition author and is not expected to be globally unique. For example, it might be a timestamp (e.g. yyyymmdd) if a managed version is not available. There is also no expectation that versions can be placed in a lexicographical sequence. To provide a version consistent with the Decision Support Service specification, use the format Major.Minor.Revision (e.g. 1.0.0). For more information on versioning knowledge assets, refer to the Decision Support Service specification. Note that a version is required for non-experimental active assets.
      # There may be different activity definition instances that have the same identifier but different versions.  The version can be appended to the url in a reference to allow a reference to a particular business version of the activity definition with the format [url]|[version].
      'version' => {
        'type'=>'string',
        'path'=>'ActivityDefinition.version',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name for this activity definition (computer friendly)
      # A natural language name identifying the activity definition. This name should be usable as an identifier for the module by machine processing applications such as code generation.
      # The name is not expected to be globally unique. The name should be a simple alphanumeric type name to ensure that it is machine-processing friendly.
      'name' => {
        'type'=>'string',
        'path'=>'ActivityDefinition.name',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name for this activity definition (human friendly)
      # A short, descriptive, user-friendly title for the activity definition.
      # This name does not need to be machine-processing friendly and may contain punctuation, white-space, etc.
      'title' => {
        'type'=>'string',
        'path'=>'ActivityDefinition.title',
        'min'=>0,
        'max'=>1
      },
      ##
      # Subordinate title of the activity definition
      # An explanatory or alternate title for the activity definition giving additional information about its content.
      'subtitle' => {
        'type'=>'string',
        'path'=>'ActivityDefinition.subtitle',
        'min'=>0,
        'max'=>1
      },
      ##
      # draft | active | retired | unknown
      # The status of this activity definition. Enables tracking the life-cycle of the content.
      # Allows filtering of activity definitions that are appropriate for use versus not.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/publication-status'=>[ 'draft', 'active', 'retired', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'ActivityDefinition.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/publication-status'}
      },
      ##
      # For testing purposes, not real usage
      # A Boolean value to indicate that this activity definition is authored for testing purposes (or education/evaluation/marketing) and is not intended to be used for genuine usage.
      # Allows filtering of activity definitions that are appropriate for use versus not.
      'experimental' => {
        'type'=>'boolean',
        'path'=>'ActivityDefinition.experimental',
        'min'=>0,
        'max'=>1
      },
      ##
      # Type of individual the activity definition is intended for
      # A code or group definition that describes the intended subject of the activity being defined.
      'subjectCodeableConcept' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/resource-types'=>[ 'Patient', 'Practitioner', 'Organization', 'Location', 'Device' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'ActivityDefinition.subject[x]',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/subject-type'}
      },
      ##
      # Type of individual the activity definition is intended for
      # A code or group definition that describes the intended subject of the activity being defined.
      'subjectReference' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/resource-types'=>[ 'Patient', 'Practitioner', 'Organization', 'Location', 'Device' ]
        },
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Group'],
        'type'=>'Reference',
        'path'=>'ActivityDefinition.subject[x]',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/subject-type'}
      },
      ##
      # Date last changed
      # The date  (and optionally time) when the activity definition was published. The date must change when the business version changes and it must change if the status code changes. In addition, it should change when the substantive content of the activity definition changes.
      # Note that this is not the same as the resource last-modified-date, since the resource may be a secondary representation of the activity definition. Additional specific dates may be added as extensions or be found by consulting Provenances associated with past versions of the resource.
      'date' => {
        'type'=>'dateTime',
        'path'=>'ActivityDefinition.date',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name of the publisher (organization or individual)
      # The name of the organization or individual that published the activity definition.
      # Usually an organization but may be an individual. The publisher (or steward) of the activity definition is the organization or individual primarily responsible for the maintenance and upkeep of the activity definition. This is not necessarily the same individual or organization that developed and initially authored the content. The publisher is the primary point of contact for questions or issues with the activity definition. This item SHOULD be populated unless the information is available from context.
      'publisher' => {
        'type'=>'string',
        'path'=>'ActivityDefinition.publisher',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contact details for the publisher
      # Contact details to assist a user in finding and communicating with the publisher.
      # May be a web site, an email address, a telephone number, etc.
      'contact' => {
        'type'=>'ContactDetail',
        'path'=>'ActivityDefinition.contact',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Natural language description of the activity definition
      # A free text natural language description of the activity definition from a consumer's perspective.
      # This description can be used to capture details such as why the activity definition was built, comments about misuse, instructions for clinical use and interpretation, literature references, examples from the paper world, etc. It is not a rendering of the activity definition as conveyed in the 'text' field of the resource itself. This item SHOULD be populated unless the information is available from context (e.g. the language of the activity definition is presumed to be the predominant language in the place the activity definition was created).
      'description' => {
        'type'=>'markdown',
        'path'=>'ActivityDefinition.description',
        'min'=>0,
        'max'=>1
      },
      ##
      # The context that the content is intended to support
      # The content was developed with a focus and intent of supporting the contexts that are listed. These contexts may be general categories (gender, age, ...) or may be references to specific programs (insurance plans, studies, ...) and may be used to assist with indexing and searching for appropriate activity definition instances.
      # When multiple useContexts are specified, there is no expectation that all or any of the contexts apply.
      'useContext' => {
        'type'=>'UsageContext',
        'path'=>'ActivityDefinition.useContext',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Intended jurisdiction for activity definition (if applicable)
      # A legal or geographic region in which the activity definition is intended to be used.
      # It may be possible for the activity definition to be used in jurisdictions other than those for which it was originally designed or intended.
      'jurisdiction' => {
        'type'=>'CodeableConcept',
        'path'=>'ActivityDefinition.jurisdiction',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Why this activity definition is defined
      # Explanation of why this activity definition is needed and why it has been designed as it has.
      # This element does not describe the usage of the activity definition. Instead, it provides traceability of ''why'' the resource is either needed or ''why'' it is defined as it is.  This may be used to point to source materials or specifications that drove the structure of this activity definition.
      'purpose' => {
        'type'=>'markdown',
        'path'=>'ActivityDefinition.purpose',
        'min'=>0,
        'max'=>1
      },
      ##
      # Describes the clinical usage of the activity definition
      # A detailed description of how the activity definition is used from a clinical perspective.
      'usage' => {
        'type'=>'string',
        'path'=>'ActivityDefinition.usage',
        'min'=>0,
        'max'=>1
      },
      ##
      # Use and/or publishing restrictions
      # A copyright statement relating to the activity definition and/or its contents. Copyright statements are generally legal restrictions on the use and publishing of the activity definition.
      'copyright' => {
        'type'=>'markdown',
        'path'=>'ActivityDefinition.copyright',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the activity definition was approved by publisher
      # The date on which the resource content was approved by the publisher. Approval happens once when the content is officially approved for usage.
      # The 'date' element may be more recent than the approval date because of minor changes or editorial corrections.
      'approvalDate' => {
        'type'=>'date',
        'path'=>'ActivityDefinition.approvalDate',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the activity definition was last reviewed
      # The date on which the resource content was last reviewed. Review happens periodically after approval but does not change the original approval date.
      # If specified, this date follows the original approval date.
      'lastReviewDate' => {
        'type'=>'date',
        'path'=>'ActivityDefinition.lastReviewDate',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the activity definition is expected to be used
      # The period during which the activity definition content was or is planned to be in active use.
      # The effective period for a activity definition  determines when the content is applicable for usage and is independent of publication and review dates. For example, a measure intended to be used for the year 2016 might be published in 2015.
      'effectivePeriod' => {
        'type'=>'Period',
        'path'=>'ActivityDefinition.effectivePeriod',
        'min'=>0,
        'max'=>1
      },
      ##
      # E.g. Education, Treatment, Assessment, etc.
      # Descriptive topics related to the content of the activity. Topics provide a high-level categorization of the activity that can be useful for filtering and searching.
      'topic' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/definition-topic'=>[ 'treatment', 'education', 'assessment' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'ActivityDefinition.topic',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/definition-topic'}
      },
      ##
      # Who authored the content
      # An individiual or organization primarily involved in the creation and maintenance of the content.
      'author' => {
        'type'=>'ContactDetail',
        'path'=>'ActivityDefinition.author',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Who edited the content
      # An individual or organization primarily responsible for internal coherence of the content.
      'editor' => {
        'type'=>'ContactDetail',
        'path'=>'ActivityDefinition.editor',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Who reviewed the content
      # An individual or organization primarily responsible for review of some aspect of the content.
      'reviewer' => {
        'type'=>'ContactDetail',
        'path'=>'ActivityDefinition.reviewer',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Who endorsed the content
      # An individual or organization responsible for officially endorsing the content for use in some setting.
      'endorser' => {
        'type'=>'ContactDetail',
        'path'=>'ActivityDefinition.endorser',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional documentation, citations, etc.
      # Related artifacts such as additional documentation, justification, or bibliographic references.
      # Each related artifact is either an attachment, or a reference to another resource, but not both.
      'relatedArtifact' => {
        'type'=>'RelatedArtifact',
        'path'=>'ActivityDefinition.relatedArtifact',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Logic used by the activity definition
      # A reference to a Library resource containing any formal logic used by the activity definition.
      'library' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Library'],
        'type'=>'canonical',
        'path'=>'ActivityDefinition.library',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Kind of resource
      # A description of the kind of resource the activity definition is representing. For example, a MedicationRequest, a ServiceRequest, or a CommunicationRequest. Typically, but not always, this is a Request resource.
      # May determine what types of extensions are permitted.
      'kind' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/request-resource-types'=>[ 'Appointment', 'AppointmentResponse', 'CarePlan', 'Claim', 'CommunicationRequest', 'Contract', 'DeviceRequest', 'EnrollmentRequest', 'ImmunizationRecommendation', 'MedicationRequest', 'NutritionOrder', 'ServiceRequest', 'SupplyRequest', 'Task', 'VisionPrescription' ]
        },
        'type'=>'code',
        'path'=>'ActivityDefinition.kind',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/request-resource-types'}
      },
      ##
      # What profile the resource needs to conform to
      # A profile to which the target of the activity definition is expected to conform.
      'profile' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/StructureDefinition'],
        'type'=>'canonical',
        'path'=>'ActivityDefinition.profile',
        'min'=>0,
        'max'=>1
      },
      ##
      # Detail type of activity
      # Detailed description of the type of activity; e.g. What lab test, what procedure, what kind of encounter.
      # Tends to be less relevant for activities involving particular products.
      'code' => {
        'type'=>'CodeableConcept',
        'path'=>'ActivityDefinition.code',
        'min'=>0,
        'max'=>1
      },
      ##
      # proposal | plan | directive | order | original-order | reflex-order | filler-order | instance-order | option
      # Indicates the level of authority/intentionality associated with the activity and where the request should fit into the workflow chain.
      'intent' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/request-intent'=>[ 'proposal', 'plan', 'directive', 'order', 'original-order', 'reflex-order', 'filler-order', 'instance-order', 'option' ]
        },
        'type'=>'code',
        'path'=>'ActivityDefinition.intent',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/request-intent'}
      },
      ##
      # routine | urgent | asap | stat
      # Indicates how quickly the activity  should be addressed with respect to other requests.
      'priority' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/request-priority'=>[ 'routine', 'urgent', 'asap', 'stat' ]
        },
        'type'=>'code',
        'path'=>'ActivityDefinition.priority',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/request-priority'}
      },
      ##
      # True if the activity should not be performed
      # Set this to true if the definition is to indicate that a particular activity should NOT be performed. If true, this element should be interpreted to reinforce a negative coding. For example NPO as a code with a doNotPerform of true would still indicate to NOT perform the action.
      # This element is not intended to be used to communicate a decision support response to cancel an order in progress. That should be done with the "remove" type of a PlanDefinition or RequestGroup.
      'doNotPerform' => {
        'type'=>'boolean',
        'path'=>'ActivityDefinition.doNotPerform',
        'min'=>0,
        'max'=>1
      },
      ##
      # When activity is to occur
      # The period, timing or frequency upon which the described activity is to occur.
      'timingAge' => {
        'type'=>'Age',
        'path'=>'ActivityDefinition.timing[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # When activity is to occur
      # The period, timing or frequency upon which the described activity is to occur.
      'timingDateTime' => {
        'type'=>'DateTime',
        'path'=>'ActivityDefinition.timing[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # When activity is to occur
      # The period, timing or frequency upon which the described activity is to occur.
      'timingDuration' => {
        'type'=>'Duration',
        'path'=>'ActivityDefinition.timing[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # When activity is to occur
      # The period, timing or frequency upon which the described activity is to occur.
      'timingPeriod' => {
        'type'=>'Period',
        'path'=>'ActivityDefinition.timing[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # When activity is to occur
      # The period, timing or frequency upon which the described activity is to occur.
      'timingRange' => {
        'type'=>'Range',
        'path'=>'ActivityDefinition.timing[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # When activity is to occur
      # The period, timing or frequency upon which the described activity is to occur.
      'timingTiming' => {
        'type'=>'Timing',
        'path'=>'ActivityDefinition.timing[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Where it should happen
      # Identifies the facility where the activity will occur; e.g. home, hospital, specific clinic, etc.
      # May reference a specific clinical location or may just identify a type of location.
      'location' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Location'],
        'type'=>'Reference',
        'path'=>'ActivityDefinition.location',
        'min'=>0,
        'max'=>1
      },
      ##
      # Who should participate in the action
      # Indicates who should participate in performing the action described.
      'participant' => {
        'type'=>'ActivityDefinition::Participant',
        'path'=>'ActivityDefinition.participant',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # What's administered/supplied
      # Identifies the food, drug or other product being consumed or supplied in the activity.
      'productCodeableConcept' => {
        'type'=>'CodeableConcept',
        'path'=>'ActivityDefinition.product[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # What's administered/supplied
      # Identifies the food, drug or other product being consumed or supplied in the activity.
      'productReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Medication', 'http://hl7.org/fhir/StructureDefinition/Substance'],
        'type'=>'Reference',
        'path'=>'ActivityDefinition.product[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # How much is administered/consumed/supplied
      # Identifies the quantity expected to be consumed at once (per dose, per meal, etc.).
      'quantity' => {
        'type'=>'Quantity',
        'path'=>'ActivityDefinition.quantity',
        'min'=>0,
        'max'=>1
      },
      ##
      # Detailed dosage instructions
      # Provides detailed dosage instructions in the same way that they are described for MedicationRequest resources.
      # If a dosage instruction is used, the definition should not specify timing or quantity.
      'dosage' => {
        'type'=>'Dosage',
        'path'=>'ActivityDefinition.dosage',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # What part of body to perform on
      # Indicates the sites on the subject's body where the procedure should be performed (I.e. the target sites).
      # Only used if not implicit in the code found in ServiceRequest.type.
      'bodySite' => {
        'type'=>'CodeableConcept',
        'path'=>'ActivityDefinition.bodySite',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # What specimens are required to perform this action
      # Defines specimen requirements for the action to be performed, such as required specimens for a lab test.
      'specimenRequirement' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/SpecimenDefinition'],
        'type'=>'Reference',
        'path'=>'ActivityDefinition.specimenRequirement',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # What observations are required to perform this action
      # Defines observation requirements for the action to be performed, such as body weight or surface area.
      'observationRequirement' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ObservationDefinition'],
        'type'=>'Reference',
        'path'=>'ActivityDefinition.observationRequirement',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # What observations must be produced by this action
      # Defines the observations that are expected to be produced by the action.
      'observationResultRequirement' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ObservationDefinition'],
        'type'=>'Reference',
        'path'=>'ActivityDefinition.observationResultRequirement',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Transform to apply the template
      # A reference to a StructureMap resource that defines a transform that can be executed to produce the intent resource using the ActivityDefinition instance as the input.
      # Note that if both a transform and dynamic values are specified, the dynamic values will be applied to the result of the transform.
      'transform' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/StructureMap'],
        'type'=>'canonical',
        'path'=>'ActivityDefinition.transform',
        'min'=>0,
        'max'=>1
      },
      ##
      # Dynamic aspects of the definition
      # Dynamic values that will be evaluated to produce values for elements of the resulting resource. For example, if the dosage of a medication must be computed based on the patient's weight, a dynamic value would be used to specify an expression that calculated the weight, and the path on the request resource that would contain the result.
      # Dynamic values are applied in the order in which they are defined in the ActivityDefinition. Note that if both a transform and dynamic values are specified, the dynamic values will be applied to the result of the transform.
      'dynamicValue' => {
        'type'=>'ActivityDefinition::DynamicValue',
        'path'=>'ActivityDefinition.dynamicValue',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Who should participate in the action
    # Indicates who should participate in performing the action described.
    class Participant < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Participant.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Participant.extension',
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
          'path'=>'Participant.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # patient | practitioner | related-person | device
        # The type of participant in the action.
        'type' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/action-participant-type'=>[ 'patient', 'practitioner', 'related-person', 'device' ]
          },
          'type'=>'code',
          'path'=>'Participant.type',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/action-participant-type'}
        },
        ##
        # E.g. Nurse, Surgeon, Parent, etc.
        # The role the participant should play in performing the described action.
        'role' => {
          'type'=>'CodeableConcept',
          'path'=>'Participant.role',
          'min'=>0,
          'max'=>1
        }
      }
      ##
      # Unique id for inter-element referencing
      # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
      attr_accessor :id                             # 0-1 string
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
      # patient | practitioner | related-person | device
      # The type of participant in the action.
      attr_accessor :type                           # 1-1 code
      ##
      # E.g. Nurse, Surgeon, Parent, etc.
      # The role the participant should play in performing the described action.
      attr_accessor :role                           # 0-1 CodeableConcept
    end

    ##
    # Dynamic aspects of the definition
    # Dynamic values that will be evaluated to produce values for elements of the resulting resource. For example, if the dosage of a medication must be computed based on the patient's weight, a dynamic value would be used to specify an expression that calculated the weight, and the path on the request resource that would contain the result.
    # Dynamic values are applied in the order in which they are defined in the ActivityDefinition. Note that if both a transform and dynamic values are specified, the dynamic values will be applied to the result of the transform.
    class DynamicValue < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'DynamicValue.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'DynamicValue.extension',
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
          'path'=>'DynamicValue.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # The path to the element to be set dynamically
        # The path to the element to be customized. This is the path on the resource that will hold the result of the calculation defined by the expression. The specified path SHALL be a FHIRPath resolveable on the specified target type of the ActivityDefinition, and SHALL consist only of identifiers, constant indexers, and a restricted subset of functions. The path is allowed to contain qualifiers (.) to traverse sub-elements, as well as indexers ([x]) to traverse multiple-cardinality sub-elements (see the [Simple FHIRPath Profile](fhirpath.html#simple) for full details).
        # The path attribute contains a [Simple FHIRPath Subset](fhirpath.html#simple) that allows path traversal, but not calculation.
        'path' => {
          'type'=>'string',
          'path'=>'DynamicValue.path',
          'min'=>1,
          'max'=>1
        },
        ##
        # An expression that provides the dynamic value for the customization
        # An expression specifying the value of the customized element.
        # The expression may be inlined, or may be a reference to a named expression within a logic library referenced by the library element.
        'expression' => {
          'type'=>'Expression',
          'path'=>'DynamicValue.expression',
          'min'=>1,
          'max'=>1
        }
      }
      ##
      # Unique id for inter-element referencing
      # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
      attr_accessor :id                             # 0-1 string
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
      # The path to the element to be set dynamically
      # The path to the element to be customized. This is the path on the resource that will hold the result of the calculation defined by the expression. The specified path SHALL be a FHIRPath resolveable on the specified target type of the ActivityDefinition, and SHALL consist only of identifiers, constant indexers, and a restricted subset of functions. The path is allowed to contain qualifiers (.) to traverse sub-elements, as well as indexers ([x]) to traverse multiple-cardinality sub-elements (see the [Simple FHIRPath Profile](fhirpath.html#simple) for full details).
      # The path attribute contains a [Simple FHIRPath Subset](fhirpath.html#simple) that allows path traversal, but not calculation.
      attr_accessor :path                           # 1-1 string
      ##
      # An expression that provides the dynamic value for the customization
      # An expression specifying the value of the customized element.
      # The expression may be inlined, or may be a reference to a named expression within a logic library referenced by the library element.
      attr_accessor :expression                     # 1-1 Expression
    end
    ##
    # Logical id of this artifact
    # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
    # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
    attr_accessor :id                             # 0-1 string
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
    # Canonical identifier for this activity definition, represented as a URI (globally unique)
    # An absolute URI that is used to identify this activity definition when it is referenced in a specification, model, design or an instance; also called its canonical identifier. This SHOULD be globally unique and SHOULD be a literal address at which at which an authoritative instance of this activity definition is (or will be) published. This URL can be the target of a canonical reference. It SHALL remain the same when the activity definition is stored on different servers.
    # Can be a urn:uuid: or a urn:oid: but real http: addresses are preferred.  Multiple instances may share the same URL if they have a distinct version.
    # 
    # The determination of when to create a new version of a resource (same url, new version) vs. defining a new artifact is up to the author.  Considerations for making this decision are found in [Technical and Business Versions](resource.html#versions). 
    # 
    # In some cases, the resource can no longer be found at the stated url, but the url itself cannot change. Implementations can use the [meta.source](resource.html#meta) element to indicate where the current master source of the resource can be found.
    attr_accessor :url                            # 0-1 uri
    ##
    # Additional identifier for the activity definition
    # A formal identifier that is used to identify this activity definition when it is represented in other formats, or referenced in a specification, model, design or an instance.
    # Typically, this is used for identifiers that can go in an HL7 V3 II (instance identifier) data type, and can then identify this activity definition outside of FHIR, where it is not possible to use the logical URI.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Business version of the activity definition
    # The identifier that is used to identify this version of the activity definition when it is referenced in a specification, model, design or instance. This is an arbitrary value managed by the activity definition author and is not expected to be globally unique. For example, it might be a timestamp (e.g. yyyymmdd) if a managed version is not available. There is also no expectation that versions can be placed in a lexicographical sequence. To provide a version consistent with the Decision Support Service specification, use the format Major.Minor.Revision (e.g. 1.0.0). For more information on versioning knowledge assets, refer to the Decision Support Service specification. Note that a version is required for non-experimental active assets.
    # There may be different activity definition instances that have the same identifier but different versions.  The version can be appended to the url in a reference to allow a reference to a particular business version of the activity definition with the format [url]|[version].
    attr_accessor :version                        # 0-1 string
    ##
    # Name for this activity definition (computer friendly)
    # A natural language name identifying the activity definition. This name should be usable as an identifier for the module by machine processing applications such as code generation.
    # The name is not expected to be globally unique. The name should be a simple alphanumeric type name to ensure that it is machine-processing friendly.
    attr_accessor :name                           # 0-1 string
    ##
    # Name for this activity definition (human friendly)
    # A short, descriptive, user-friendly title for the activity definition.
    # This name does not need to be machine-processing friendly and may contain punctuation, white-space, etc.
    attr_accessor :title                          # 0-1 string
    ##
    # Subordinate title of the activity definition
    # An explanatory or alternate title for the activity definition giving additional information about its content.
    attr_accessor :subtitle                       # 0-1 string
    ##
    # draft | active | retired | unknown
    # The status of this activity definition. Enables tracking the life-cycle of the content.
    # Allows filtering of activity definitions that are appropriate for use versus not.
    attr_accessor :status                         # 1-1 code
    ##
    # For testing purposes, not real usage
    # A Boolean value to indicate that this activity definition is authored for testing purposes (or education/evaluation/marketing) and is not intended to be used for genuine usage.
    # Allows filtering of activity definitions that are appropriate for use versus not.
    attr_accessor :experimental                   # 0-1 boolean
    ##
    # Type of individual the activity definition is intended for
    # A code or group definition that describes the intended subject of the activity being defined.
    attr_accessor :subjectCodeableConcept         # 0-1 CodeableConcept
    ##
    # Type of individual the activity definition is intended for
    # A code or group definition that describes the intended subject of the activity being defined.
    attr_accessor :subjectReference               # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Group)
    ##
    # Date last changed
    # The date  (and optionally time) when the activity definition was published. The date must change when the business version changes and it must change if the status code changes. In addition, it should change when the substantive content of the activity definition changes.
    # Note that this is not the same as the resource last-modified-date, since the resource may be a secondary representation of the activity definition. Additional specific dates may be added as extensions or be found by consulting Provenances associated with past versions of the resource.
    attr_accessor :date                           # 0-1 dateTime
    ##
    # Name of the publisher (organization or individual)
    # The name of the organization or individual that published the activity definition.
    # Usually an organization but may be an individual. The publisher (or steward) of the activity definition is the organization or individual primarily responsible for the maintenance and upkeep of the activity definition. This is not necessarily the same individual or organization that developed and initially authored the content. The publisher is the primary point of contact for questions or issues with the activity definition. This item SHOULD be populated unless the information is available from context.
    attr_accessor :publisher                      # 0-1 string
    ##
    # Contact details for the publisher
    # Contact details to assist a user in finding and communicating with the publisher.
    # May be a web site, an email address, a telephone number, etc.
    attr_accessor :contact                        # 0-* [ ContactDetail ]
    ##
    # Natural language description of the activity definition
    # A free text natural language description of the activity definition from a consumer's perspective.
    # This description can be used to capture details such as why the activity definition was built, comments about misuse, instructions for clinical use and interpretation, literature references, examples from the paper world, etc. It is not a rendering of the activity definition as conveyed in the 'text' field of the resource itself. This item SHOULD be populated unless the information is available from context (e.g. the language of the activity definition is presumed to be the predominant language in the place the activity definition was created).
    attr_accessor :description                    # 0-1 markdown
    ##
    # The context that the content is intended to support
    # The content was developed with a focus and intent of supporting the contexts that are listed. These contexts may be general categories (gender, age, ...) or may be references to specific programs (insurance plans, studies, ...) and may be used to assist with indexing and searching for appropriate activity definition instances.
    # When multiple useContexts are specified, there is no expectation that all or any of the contexts apply.
    attr_accessor :useContext                     # 0-* [ UsageContext ]
    ##
    # Intended jurisdiction for activity definition (if applicable)
    # A legal or geographic region in which the activity definition is intended to be used.
    # It may be possible for the activity definition to be used in jurisdictions other than those for which it was originally designed or intended.
    attr_accessor :jurisdiction                   # 0-* [ CodeableConcept ]
    ##
    # Why this activity definition is defined
    # Explanation of why this activity definition is needed and why it has been designed as it has.
    # This element does not describe the usage of the activity definition. Instead, it provides traceability of ''why'' the resource is either needed or ''why'' it is defined as it is.  This may be used to point to source materials or specifications that drove the structure of this activity definition.
    attr_accessor :purpose                        # 0-1 markdown
    ##
    # Describes the clinical usage of the activity definition
    # A detailed description of how the activity definition is used from a clinical perspective.
    attr_accessor :usage                          # 0-1 string
    ##
    # Use and/or publishing restrictions
    # A copyright statement relating to the activity definition and/or its contents. Copyright statements are generally legal restrictions on the use and publishing of the activity definition.
    attr_accessor :copyright                      # 0-1 markdown
    ##
    # When the activity definition was approved by publisher
    # The date on which the resource content was approved by the publisher. Approval happens once when the content is officially approved for usage.
    # The 'date' element may be more recent than the approval date because of minor changes or editorial corrections.
    attr_accessor :approvalDate                   # 0-1 date
    ##
    # When the activity definition was last reviewed
    # The date on which the resource content was last reviewed. Review happens periodically after approval but does not change the original approval date.
    # If specified, this date follows the original approval date.
    attr_accessor :lastReviewDate                 # 0-1 date
    ##
    # When the activity definition is expected to be used
    # The period during which the activity definition content was or is planned to be in active use.
    # The effective period for a activity definition  determines when the content is applicable for usage and is independent of publication and review dates. For example, a measure intended to be used for the year 2016 might be published in 2015.
    attr_accessor :effectivePeriod                # 0-1 Period
    ##
    # E.g. Education, Treatment, Assessment, etc.
    # Descriptive topics related to the content of the activity. Topics provide a high-level categorization of the activity that can be useful for filtering and searching.
    attr_accessor :topic                          # 0-* [ CodeableConcept ]
    ##
    # Who authored the content
    # An individiual or organization primarily involved in the creation and maintenance of the content.
    attr_accessor :author                         # 0-* [ ContactDetail ]
    ##
    # Who edited the content
    # An individual or organization primarily responsible for internal coherence of the content.
    attr_accessor :editor                         # 0-* [ ContactDetail ]
    ##
    # Who reviewed the content
    # An individual or organization primarily responsible for review of some aspect of the content.
    attr_accessor :reviewer                       # 0-* [ ContactDetail ]
    ##
    # Who endorsed the content
    # An individual or organization responsible for officially endorsing the content for use in some setting.
    attr_accessor :endorser                       # 0-* [ ContactDetail ]
    ##
    # Additional documentation, citations, etc.
    # Related artifacts such as additional documentation, justification, or bibliographic references.
    # Each related artifact is either an attachment, or a reference to another resource, but not both.
    attr_accessor :relatedArtifact                # 0-* [ RelatedArtifact ]
    ##
    # Logic used by the activity definition
    # A reference to a Library resource containing any formal logic used by the activity definition.
    attr_accessor :library                        # 0-* [ canonical(http://hl7.org/fhir/StructureDefinition/Library) ]
    ##
    # Kind of resource
    # A description of the kind of resource the activity definition is representing. For example, a MedicationRequest, a ServiceRequest, or a CommunicationRequest. Typically, but not always, this is a Request resource.
    # May determine what types of extensions are permitted.
    attr_accessor :kind                           # 0-1 code
    ##
    # What profile the resource needs to conform to
    # A profile to which the target of the activity definition is expected to conform.
    attr_accessor :profile                        # 0-1 canonical(http://hl7.org/fhir/StructureDefinition/StructureDefinition)
    ##
    # Detail type of activity
    # Detailed description of the type of activity; e.g. What lab test, what procedure, what kind of encounter.
    # Tends to be less relevant for activities involving particular products.
    attr_accessor :code                           # 0-1 CodeableConcept
    ##
    # proposal | plan | directive | order | original-order | reflex-order | filler-order | instance-order | option
    # Indicates the level of authority/intentionality associated with the activity and where the request should fit into the workflow chain.
    attr_accessor :intent                         # 0-1 code
    ##
    # routine | urgent | asap | stat
    # Indicates how quickly the activity  should be addressed with respect to other requests.
    attr_accessor :priority                       # 0-1 code
    ##
    # True if the activity should not be performed
    # Set this to true if the definition is to indicate that a particular activity should NOT be performed. If true, this element should be interpreted to reinforce a negative coding. For example NPO as a code with a doNotPerform of true would still indicate to NOT perform the action.
    # This element is not intended to be used to communicate a decision support response to cancel an order in progress. That should be done with the "remove" type of a PlanDefinition or RequestGroup.
    attr_accessor :doNotPerform                   # 0-1 boolean
    ##
    # When activity is to occur
    # The period, timing or frequency upon which the described activity is to occur.
    attr_accessor :timingAge                      # 0-1 Age
    ##
    # When activity is to occur
    # The period, timing or frequency upon which the described activity is to occur.
    attr_accessor :timingDateTime                 # 0-1 DateTime
    ##
    # When activity is to occur
    # The period, timing or frequency upon which the described activity is to occur.
    attr_accessor :timingDuration                 # 0-1 Duration
    ##
    # When activity is to occur
    # The period, timing or frequency upon which the described activity is to occur.
    attr_accessor :timingPeriod                   # 0-1 Period
    ##
    # When activity is to occur
    # The period, timing or frequency upon which the described activity is to occur.
    attr_accessor :timingRange                    # 0-1 Range
    ##
    # When activity is to occur
    # The period, timing or frequency upon which the described activity is to occur.
    attr_accessor :timingTiming                   # 0-1 Timing
    ##
    # Where it should happen
    # Identifies the facility where the activity will occur; e.g. home, hospital, specific clinic, etc.
    # May reference a specific clinical location or may just identify a type of location.
    attr_accessor :location                       # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Location)
    ##
    # Who should participate in the action
    # Indicates who should participate in performing the action described.
    attr_accessor :participant                    # 0-* [ ActivityDefinition::Participant ]
    ##
    # What's administered/supplied
    # Identifies the food, drug or other product being consumed or supplied in the activity.
    attr_accessor :productCodeableConcept         # 0-1 CodeableConcept
    ##
    # What's administered/supplied
    # Identifies the food, drug or other product being consumed or supplied in the activity.
    attr_accessor :productReference               # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Medication|http://hl7.org/fhir/StructureDefinition/Substance)
    ##
    # How much is administered/consumed/supplied
    # Identifies the quantity expected to be consumed at once (per dose, per meal, etc.).
    attr_accessor :quantity                       # 0-1 Quantity
    ##
    # Detailed dosage instructions
    # Provides detailed dosage instructions in the same way that they are described for MedicationRequest resources.
    # If a dosage instruction is used, the definition should not specify timing or quantity.
    attr_accessor :dosage                         # 0-* [ Dosage ]
    ##
    # What part of body to perform on
    # Indicates the sites on the subject's body where the procedure should be performed (I.e. the target sites).
    # Only used if not implicit in the code found in ServiceRequest.type.
    attr_accessor :bodySite                       # 0-* [ CodeableConcept ]
    ##
    # What specimens are required to perform this action
    # Defines specimen requirements for the action to be performed, such as required specimens for a lab test.
    attr_accessor :specimenRequirement            # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/SpecimenDefinition) ]
    ##
    # What observations are required to perform this action
    # Defines observation requirements for the action to be performed, such as body weight or surface area.
    attr_accessor :observationRequirement         # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/ObservationDefinition) ]
    ##
    # What observations must be produced by this action
    # Defines the observations that are expected to be produced by the action.
    attr_accessor :observationResultRequirement   # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/ObservationDefinition) ]
    ##
    # Transform to apply the template
    # A reference to a StructureMap resource that defines a transform that can be executed to produce the intent resource using the ActivityDefinition instance as the input.
    # Note that if both a transform and dynamic values are specified, the dynamic values will be applied to the result of the transform.
    attr_accessor :transform                      # 0-1 canonical(http://hl7.org/fhir/StructureDefinition/StructureMap)
    ##
    # Dynamic aspects of the definition
    # Dynamic values that will be evaluated to produce values for elements of the resulting resource. For example, if the dosage of a medication must be computed based on the patient's weight, a dynamic value would be used to specify an expression that calculated the weight, and the path on the request resource that would contain the result.
    # Dynamic values are applied in the order in which they are defined in the ActivityDefinition. Note that if both a transform and dynamic values are specified, the dynamic values will be applied to the result of the transform.
    attr_accessor :dynamicValue                   # 0-* [ ActivityDefinition::DynamicValue ]

    def resourceType
      'ActivityDefinition'
    end
  end
end
