module FHIR

  ##
  # This resource allows for the definition of various types of plans as a sharable, consumable, and executable artifact. The resource is general enough to support the description of a broad range of clinical artifacts such as clinical decision support rules, order sets and protocols.
  class PlanDefinition < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['composed-of', 'context-quantity', 'context-type-quantity', 'context-type-value', 'context-type', 'context', 'date', 'definition', 'depends-on', 'derived-from', 'description', 'effective', 'identifier', 'jurisdiction', 'name', 'predecessor', 'publisher', 'status', 'successor', 'title', 'topic', 'type', 'url', 'version']
    MULTIPLE_TYPES = {
      'subject[x]' => ['CodeableConcept', 'Reference']
    }
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'PlanDefinition.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'PlanDefinition.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'PlanDefinition.implicitRules',
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
        'path'=>'PlanDefinition.language',
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
        'path'=>'PlanDefinition.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'PlanDefinition.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'PlanDefinition.extension',
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
        'path'=>'PlanDefinition.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Canonical identifier for this plan definition, represented as a URI (globally unique)
      # An absolute URI that is used to identify this plan definition when it is referenced in a specification, model, design or an instance; also called its canonical identifier. This SHOULD be globally unique and SHOULD be a literal address at which at which an authoritative instance of this plan definition is (or will be) published. This URL can be the target of a canonical reference. It SHALL remain the same when the plan definition is stored on different servers.
      # Can be a urn:uuid: or a urn:oid: but real http: addresses are preferred.  Multiple instances may share the same URL if they have a distinct version.
      # 
      # The determination of when to create a new version of a resource (same url, new version) vs. defining a new artifact is up to the author.  Considerations for making this decision are found in [Technical and Business Versions](resource.html#versions). 
      # 
      # In some cases, the resource can no longer be found at the stated url, but the url itself cannot change. Implementations can use the [meta.source](resource.html#meta) element to indicate where the current master source of the resource can be found.
      'url' => {
        'type'=>'uri',
        'path'=>'PlanDefinition.url',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional identifier for the plan definition
      # A formal identifier that is used to identify this plan definition when it is represented in other formats, or referenced in a specification, model, design or an instance.
      # Typically, this is used for identifiers that can go in an HL7 V3 II (instance identifier) data type, and can then identify this plan definition outside of FHIR, where it is not possible to use the logical URI.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'PlanDefinition.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Business version of the plan definition
      # The identifier that is used to identify this version of the plan definition when it is referenced in a specification, model, design or instance. This is an arbitrary value managed by the plan definition author and is not expected to be globally unique. For example, it might be a timestamp (e.g. yyyymmdd) if a managed version is not available. There is also no expectation that versions can be placed in a lexicographical sequence. To provide a version consistent with the Decision Support Service specification, use the format Major.Minor.Revision (e.g. 1.0.0). For more information on versioning knowledge assets, refer to the Decision Support Service specification. Note that a version is required for non-experimental active artifacts.
      # There may be different plan definition instances that have the same identifier but different versions.  The version can be appended to the url in a reference to allow a reference to a particular business version of the plan definition with the format [url]|[version].
      'version' => {
        'type'=>'string',
        'path'=>'PlanDefinition.version',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name for this plan definition (computer friendly)
      # A natural language name identifying the plan definition. This name should be usable as an identifier for the module by machine processing applications such as code generation.
      # The name is not expected to be globally unique. The name should be a simple alphanumeric type name to ensure that it is machine-processing friendly.
      'name' => {
        'type'=>'string',
        'path'=>'PlanDefinition.name',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name for this plan definition (human friendly)
      # A short, descriptive, user-friendly title for the plan definition.
      # This name does not need to be machine-processing friendly and may contain punctuation, white-space, etc.
      'title' => {
        'type'=>'string',
        'path'=>'PlanDefinition.title',
        'min'=>0,
        'max'=>1
      },
      ##
      # Subordinate title of the plan definition
      # An explanatory or alternate title for the plan definition giving additional information about its content.
      'subtitle' => {
        'type'=>'string',
        'path'=>'PlanDefinition.subtitle',
        'min'=>0,
        'max'=>1
      },
      ##
      # order-set | clinical-protocol | eca-rule | workflow-definition
      # A high-level category for the plan definition that distinguishes the kinds of systems that would be interested in the plan definition.
      'type' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/plan-definition-type'=>[ 'order-set', 'clinical-protocol', 'eca-rule', 'workflow-definition' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'PlanDefinition.type',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/plan-definition-type'}
      },
      ##
      # draft | active | retired | unknown
      # The status of this plan definition. Enables tracking the life-cycle of the content.
      # Allows filtering of plan definitions that are appropriate for use versus not.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/publication-status'=>[ 'draft', 'active', 'retired', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'PlanDefinition.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/publication-status'}
      },
      ##
      # For testing purposes, not real usage
      # A Boolean value to indicate that this plan definition is authored for testing purposes (or education/evaluation/marketing) and is not intended to be used for genuine usage.
      # Allows filtering of plan definitions that are appropriate for use versus not.
      'experimental' => {
        'type'=>'boolean',
        'path'=>'PlanDefinition.experimental',
        'min'=>0,
        'max'=>1
      },
      ##
      # Type of individual the plan definition is focused on
      # A code or group definition that describes the intended subject of the plan definition.
      'subjectCodeableConcept' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/resource-types'=>[ 'Patient', 'Practitioner', 'Organization', 'Location', 'Device' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'PlanDefinition.subject[x]',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/subject-type'}
      },
      ##
      # Type of individual the plan definition is focused on
      # A code or group definition that describes the intended subject of the plan definition.
      'subjectReference' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/resource-types'=>[ 'Patient', 'Practitioner', 'Organization', 'Location', 'Device' ]
        },
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Group'],
        'type'=>'Reference',
        'path'=>'PlanDefinition.subject[x]',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/subject-type'}
      },
      ##
      # Date last changed
      # The date  (and optionally time) when the plan definition was published. The date must change when the business version changes and it must change if the status code changes. In addition, it should change when the substantive content of the plan definition changes.
      # Note that this is not the same as the resource last-modified-date, since the resource may be a secondary representation of the plan definition. Additional specific dates may be added as extensions or be found by consulting Provenances associated with past versions of the resource.
      'date' => {
        'type'=>'dateTime',
        'path'=>'PlanDefinition.date',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name of the publisher (organization or individual)
      # The name of the organization or individual that published the plan definition.
      # Usually an organization but may be an individual. The publisher (or steward) of the plan definition is the organization or individual primarily responsible for the maintenance and upkeep of the plan definition. This is not necessarily the same individual or organization that developed and initially authored the content. The publisher is the primary point of contact for questions or issues with the plan definition. This item SHOULD be populated unless the information is available from context.
      'publisher' => {
        'type'=>'string',
        'path'=>'PlanDefinition.publisher',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contact details for the publisher
      # Contact details to assist a user in finding and communicating with the publisher.
      # May be a web site, an email address, a telephone number, etc.
      'contact' => {
        'type'=>'ContactDetail',
        'path'=>'PlanDefinition.contact',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Natural language description of the plan definition
      # A free text natural language description of the plan definition from a consumer's perspective.
      # This description can be used to capture details such as why the plan definition was built, comments about misuse, instructions for clinical use and interpretation, literature references, examples from the paper world, etc. It is not a rendering of the plan definition as conveyed in the 'text' field of the resource itself. This item SHOULD be populated unless the information is available from context (e.g. the language of the plan definition is presumed to be the predominant language in the place the plan definition was created).
      'description' => {
        'type'=>'markdown',
        'path'=>'PlanDefinition.description',
        'min'=>0,
        'max'=>1
      },
      ##
      # The context that the content is intended to support
      # The content was developed with a focus and intent of supporting the contexts that are listed. These contexts may be general categories (gender, age, ...) or may be references to specific programs (insurance plans, studies, ...) and may be used to assist with indexing and searching for appropriate plan definition instances.
      # When multiple useContexts are specified, there is no expectation that all or any of the contexts apply.
      'useContext' => {
        'type'=>'UsageContext',
        'path'=>'PlanDefinition.useContext',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Intended jurisdiction for plan definition (if applicable)
      # A legal or geographic region in which the plan definition is intended to be used.
      # It may be possible for the plan definition to be used in jurisdictions other than those for which it was originally designed or intended.
      'jurisdiction' => {
        'type'=>'CodeableConcept',
        'path'=>'PlanDefinition.jurisdiction',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Why this plan definition is defined
      # Explanation of why this plan definition is needed and why it has been designed as it has.
      # This element does not describe the usage of the plan definition. Instead, it provides traceability of ''why'' the resource is either needed or ''why'' it is defined as it is.  This may be used to point to source materials or specifications that drove the structure of this plan definition.
      'purpose' => {
        'type'=>'markdown',
        'path'=>'PlanDefinition.purpose',
        'min'=>0,
        'max'=>1
      },
      ##
      # Describes the clinical usage of the plan
      # A detailed description of how the plan definition is used from a clinical perspective.
      'usage' => {
        'type'=>'string',
        'path'=>'PlanDefinition.usage',
        'min'=>0,
        'max'=>1
      },
      ##
      # Use and/or publishing restrictions
      # A copyright statement relating to the plan definition and/or its contents. Copyright statements are generally legal restrictions on the use and publishing of the plan definition.
      'copyright' => {
        'type'=>'markdown',
        'path'=>'PlanDefinition.copyright',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the plan definition was approved by publisher
      # The date on which the resource content was approved by the publisher. Approval happens once when the content is officially approved for usage.
      # The 'date' element may be more recent than the approval date because of minor changes or editorial corrections.
      'approvalDate' => {
        'type'=>'date',
        'path'=>'PlanDefinition.approvalDate',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the plan definition was last reviewed
      # The date on which the resource content was last reviewed. Review happens periodically after approval but does not change the original approval date.
      # If specified, this date follows the original approval date.
      'lastReviewDate' => {
        'type'=>'date',
        'path'=>'PlanDefinition.lastReviewDate',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the plan definition is expected to be used
      # The period during which the plan definition content was or is planned to be in active use.
      # The effective period for a plan definition  determines when the content is applicable for usage and is independent of publication and review dates. For example, a measure intended to be used for the year 2016 might be published in 2015.
      'effectivePeriod' => {
        'type'=>'Period',
        'path'=>'PlanDefinition.effectivePeriod',
        'min'=>0,
        'max'=>1
      },
      ##
      # E.g. Education, Treatment, Assessment
      # Descriptive topics related to the content of the plan definition. Topics provide a high-level categorization of the definition that can be useful for filtering and searching.
      'topic' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/definition-topic'=>[ 'treatment', 'education', 'assessment' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'PlanDefinition.topic',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/definition-topic'}
      },
      ##
      # Who authored the content
      # An individiual or organization primarily involved in the creation and maintenance of the content.
      'author' => {
        'type'=>'ContactDetail',
        'path'=>'PlanDefinition.author',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Who edited the content
      # An individual or organization primarily responsible for internal coherence of the content.
      'editor' => {
        'type'=>'ContactDetail',
        'path'=>'PlanDefinition.editor',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Who reviewed the content
      # An individual or organization primarily responsible for review of some aspect of the content.
      'reviewer' => {
        'type'=>'ContactDetail',
        'path'=>'PlanDefinition.reviewer',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Who endorsed the content
      # An individual or organization responsible for officially endorsing the content for use in some setting.
      'endorser' => {
        'type'=>'ContactDetail',
        'path'=>'PlanDefinition.endorser',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional documentation, citations
      # Related artifacts such as additional documentation, justification, or bibliographic references.
      # Each related artifact is either an attachment, or a reference to another resource, but not both.
      'relatedArtifact' => {
        'type'=>'RelatedArtifact',
        'path'=>'PlanDefinition.relatedArtifact',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Logic used by the plan definition
      # A reference to a Library resource containing any formal logic used by the plan definition.
      'library' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Library'],
        'type'=>'canonical',
        'path'=>'PlanDefinition.library',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # What the plan is trying to accomplish
      # Goals that describe what the activities within the plan are intended to achieve. For example, weight loss, restoring an activity of daily living, obtaining herd immunity via immunization, meeting a process improvement objective, etc.
      'goal' => {
        'type'=>'PlanDefinition::Goal',
        'path'=>'PlanDefinition.goal',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Action defined by the plan
      # An action or group of actions to be taken as part of the plan.
      # Note that there is overlap between many of the elements defined here and the ActivityDefinition resource. When an ActivityDefinition is referenced (using the definition element), the overlapping elements in the plan override the content of the referenced ActivityDefinition unless otherwise documented in the specific elements. See the PlanDefinition resource for more detailed information.
      'action' => {
        'type'=>'PlanDefinition::Action',
        'path'=>'PlanDefinition.action',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # What the plan is trying to accomplish
    # Goals that describe what the activities within the plan are intended to achieve. For example, weight loss, restoring an activity of daily living, obtaining herd immunity via immunization, meeting a process improvement objective, etc.
    class Goal < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Goal.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Goal.extension',
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
          'path'=>'Goal.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # E.g. Treatment, dietary, behavioral
        # Indicates a category the goal falls within.
        'category' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/goal-category'=>[ 'dietary', 'safety', 'behavioral', 'nursing', 'physiotherapy' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Goal.category',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/goal-category'}
        },
        ##
        # Code or text describing the goal
        # Human-readable and/or coded description of a specific desired objective of care, such as "control blood pressure" or "negotiate an obstacle course" or "dance with child at wedding".
        # If no code is available, use CodeableConcept.text.
        'description' => {
          'type'=>'CodeableConcept',
          'path'=>'Goal.description',
          'min'=>1,
          'max'=>1
        },
        ##
        # high-priority | medium-priority | low-priority
        # Identifies the expected level of importance associated with reaching/sustaining the defined goal.
        'priority' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/goal-priority'=>[ 'high-priority', 'medium-priority', 'low-priority' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Goal.priority',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/goal-priority'}
        },
        ##
        # When goal pursuit begins
        # The event after which the goal should begin being pursued.
        'start' => {
          'type'=>'CodeableConcept',
          'path'=>'Goal.start',
          'min'=>0,
          'max'=>1
        },
        ##
        # What does the goal address
        # Identifies problems, conditions, issues, or concerns the goal is intended to address.
        'addresses' => {
          'type'=>'CodeableConcept',
          'path'=>'Goal.addresses',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Supporting documentation for the goal
        # Didactic or other informational resources associated with the goal that provide further supporting information about the goal. Information resources can include inline text commentary and links to web resources.
        'documentation' => {
          'type'=>'RelatedArtifact',
          'path'=>'Goal.documentation',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Target outcome for the goal
        # Indicates what should be done and within what timeframe.
        'target' => {
          'type'=>'PlanDefinition::Goal::Target',
          'path'=>'Goal.target',
          'min'=>0,
          'max'=>Float::INFINITY
        }
      }

      ##
      # Target outcome for the goal
      # Indicates what should be done and within what timeframe.
      class Target < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        MULTIPLE_TYPES = {
          'detail[x]' => ['CodeableConcept', 'Quantity', 'Range']
        }
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
          # The parameter whose value is to be tracked, e.g. body weight, blood pressure, or hemoglobin A1c level.
          'measure' => {
            'type'=>'CodeableConcept',
            'path'=>'Target.measure',
            'min'=>0,
            'max'=>1
          },
          ##
          # The target value to be achieved
          # The target value of the measure to be achieved to signify fulfillment of the goal, e.g. 150 pounds or 7.0%. Either the high or low or both values of the range can be specified. When a low value is missing, it indicates that the goal is achieved at any value at or below the high value. Similarly, if the high value is missing, it indicates that the goal is achieved at any value at or above the low value.
          'detailCodeableConcept' => {
            'type'=>'CodeableConcept',
            'path'=>'Target.detail[x]',
            'min'=>0,
            'max'=>1
          },
          ##
          # The target value to be achieved
          # The target value of the measure to be achieved to signify fulfillment of the goal, e.g. 150 pounds or 7.0%. Either the high or low or both values of the range can be specified. When a low value is missing, it indicates that the goal is achieved at any value at or below the high value. Similarly, if the high value is missing, it indicates that the goal is achieved at any value at or above the low value.
          'detailQuantity' => {
            'type'=>'Quantity',
            'path'=>'Target.detail[x]',
            'min'=>0,
            'max'=>1
          },
          ##
          # The target value to be achieved
          # The target value of the measure to be achieved to signify fulfillment of the goal, e.g. 150 pounds or 7.0%. Either the high or low or both values of the range can be specified. When a low value is missing, it indicates that the goal is achieved at any value at or below the high value. Similarly, if the high value is missing, it indicates that the goal is achieved at any value at or above the low value.
          'detailRange' => {
            'type'=>'Range',
            'path'=>'Target.detail[x]',
            'min'=>0,
            'max'=>1
          },
          ##
          # Reach goal within
          # Indicates the timeframe after the start of the goal in which the goal should be met.
          'due' => {
            'type'=>'Duration',
            'path'=>'Target.due',
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
        # The parameter whose value is to be tracked, e.g. body weight, blood pressure, or hemoglobin A1c level.
        attr_accessor :measure                        # 0-1 CodeableConcept
        ##
        # The target value to be achieved
        # The target value of the measure to be achieved to signify fulfillment of the goal, e.g. 150 pounds or 7.0%. Either the high or low or both values of the range can be specified. When a low value is missing, it indicates that the goal is achieved at any value at or below the high value. Similarly, if the high value is missing, it indicates that the goal is achieved at any value at or above the low value.
        attr_accessor :detailCodeableConcept          # 0-1 CodeableConcept
        ##
        # The target value to be achieved
        # The target value of the measure to be achieved to signify fulfillment of the goal, e.g. 150 pounds or 7.0%. Either the high or low or both values of the range can be specified. When a low value is missing, it indicates that the goal is achieved at any value at or below the high value. Similarly, if the high value is missing, it indicates that the goal is achieved at any value at or above the low value.
        attr_accessor :detailQuantity                 # 0-1 Quantity
        ##
        # The target value to be achieved
        # The target value of the measure to be achieved to signify fulfillment of the goal, e.g. 150 pounds or 7.0%. Either the high or low or both values of the range can be specified. When a low value is missing, it indicates that the goal is achieved at any value at or below the high value. Similarly, if the high value is missing, it indicates that the goal is achieved at any value at or above the low value.
        attr_accessor :detailRange                    # 0-1 Range
        ##
        # Reach goal within
        # Indicates the timeframe after the start of the goal in which the goal should be met.
        attr_accessor :due                            # 0-1 Duration
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
      # E.g. Treatment, dietary, behavioral
      # Indicates a category the goal falls within.
      attr_accessor :category                       # 0-1 CodeableConcept
      ##
      # Code or text describing the goal
      # Human-readable and/or coded description of a specific desired objective of care, such as "control blood pressure" or "negotiate an obstacle course" or "dance with child at wedding".
      # If no code is available, use CodeableConcept.text.
      attr_accessor :description                    # 1-1 CodeableConcept
      ##
      # high-priority | medium-priority | low-priority
      # Identifies the expected level of importance associated with reaching/sustaining the defined goal.
      attr_accessor :priority                       # 0-1 CodeableConcept
      ##
      # When goal pursuit begins
      # The event after which the goal should begin being pursued.
      attr_accessor :start                          # 0-1 CodeableConcept
      ##
      # What does the goal address
      # Identifies problems, conditions, issues, or concerns the goal is intended to address.
      attr_accessor :addresses                      # 0-* [ CodeableConcept ]
      ##
      # Supporting documentation for the goal
      # Didactic or other informational resources associated with the goal that provide further supporting information about the goal. Information resources can include inline text commentary and links to web resources.
      attr_accessor :documentation                  # 0-* [ RelatedArtifact ]
      ##
      # Target outcome for the goal
      # Indicates what should be done and within what timeframe.
      attr_accessor :target                         # 0-* [ PlanDefinition::Goal::Target ]
    end

    ##
    # Action defined by the plan
    # An action or group of actions to be taken as part of the plan.
    # Note that there is overlap between many of the elements defined here and the ActivityDefinition resource. When an ActivityDefinition is referenced (using the definition element), the overlapping elements in the plan override the content of the referenced ActivityDefinition unless otherwise documented in the specific elements. See the PlanDefinition resource for more detailed information.
    class Action < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'subject[x]' => ['CodeableConcept', 'Reference'],
        'timing[x]' => ['Age', 'dateTime', 'Duration', 'Period', 'Range', 'Timing'],
        'definition[x]' => ['canonical', 'uri']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Action.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Action.extension',
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
          'path'=>'Action.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # User-visible prefix for the action (e.g. 1. or A.)
        # A user-visible prefix for the action.
        'prefix' => {
          'type'=>'string',
          'path'=>'Action.prefix',
          'min'=>0,
          'max'=>1
        },
        ##
        # User-visible title
        # The title of the action displayed to a user.
        'title' => {
          'type'=>'string',
          'path'=>'Action.title',
          'min'=>0,
          'max'=>1
        },
        ##
        # Brief description of the action
        # A brief description of the action used to provide a summary to display to the user.
        'description' => {
          'type'=>'string',
          'path'=>'Action.description',
          'min'=>0,
          'max'=>1
        },
        ##
        # Static text equivalent of the action, used if the dynamic aspects cannot be interpreted by the receiving system
        # A text equivalent of the action to be performed. This provides a human-interpretable description of the action when the definition is consumed by a system that might not be capable of interpreting it dynamically.
        'textEquivalent' => {
          'type'=>'string',
          'path'=>'Action.textEquivalent',
          'min'=>0,
          'max'=>1
        },
        ##
        # routine | urgent | asap | stat
        # Indicates how quickly the action should be addressed with respect to other actions.
        'priority' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/request-priority'=>[ 'routine', 'urgent', 'asap', 'stat' ]
          },
          'type'=>'code',
          'path'=>'Action.priority',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/request-priority'}
        },
        ##
        # Code representing the meaning of the action or sub-actions
        # A code that provides meaning for the action or action group. For example, a section may have a LOINC code for the section of a documentation template.
        'code' => {
          'type'=>'CodeableConcept',
          'path'=>'Action.code',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Why the action should be performed
        # A description of why this action is necessary or appropriate.
        # This is different than the clinical evidence documentation, it's an actual business description of the reason for performing the action.
        'reason' => {
          'type'=>'CodeableConcept',
          'path'=>'Action.reason',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Supporting documentation for the intended performer of the action
        # Didactic or other informational resources associated with the action that can be provided to the CDS recipient. Information resources can include inline text commentary and links to web resources.
        'documentation' => {
          'type'=>'RelatedArtifact',
          'path'=>'Action.documentation',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # What goals this action supports
        # Identifies goals that this action supports. The reference must be to a goal element defined within this plan definition.
        'goalId' => {
          'type'=>'id',
          'path'=>'Action.goalId',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Type of individual the action is focused on
        # A code or group definition that describes the intended subject of the action and its children, if any.
        # The subject of an action overrides the subject at a parent action or on the root of the PlanDefinition if specified.
        # 
        # In addition, because the subject needs to be resolved during realization, use of subjects in actions (or in the ActivityDefinition referenced by the action) resolves based on the set of subjects supplied in context and by type (i.e. the patient subject would resolve to a resource of type Patient).
        'subjectCodeableConcept' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/resource-types'=>[ 'Patient', 'Practitioner', 'Organization', 'Location', 'Device' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Action.subject[x]',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/subject-type'}
        },
        ##
        # Type of individual the action is focused on
        # A code or group definition that describes the intended subject of the action and its children, if any.
        # The subject of an action overrides the subject at a parent action or on the root of the PlanDefinition if specified.
        # 
        # In addition, because the subject needs to be resolved during realization, use of subjects in actions (or in the ActivityDefinition referenced by the action) resolves based on the set of subjects supplied in context and by type (i.e. the patient subject would resolve to a resource of type Patient).
        'subjectReference' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/resource-types'=>[ 'Patient', 'Practitioner', 'Organization', 'Location', 'Device' ]
          },
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Group'],
          'type'=>'Reference',
          'path'=>'Action.subject[x]',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/subject-type'}
        },
        ##
        # When the action should be triggered
        # A description of when the action should be triggered.
        'trigger' => {
          'type'=>'TriggerDefinition',
          'path'=>'Action.trigger',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Whether or not the action is applicable
        # An expression that describes applicability criteria or start/stop conditions for the action.
        # When multiple conditions of the same kind are present, the effects are combined using AND semantics, so the overall condition is true only if all the conditions are true.
        'condition' => {
          'type'=>'PlanDefinition::Action::Condition',
          'path'=>'Action.condition',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Input data requirements
        # Defines input data requirements for the action.
        'input' => {
          'type'=>'DataRequirement',
          'path'=>'Action.input',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Output data definition
        # Defines the outputs of the action, if any.
        'output' => {
          'type'=>'DataRequirement',
          'path'=>'Action.output',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Relationship to another action
        # A relationship to another action such as "before" or "30-60 minutes after start of".
        # When an action depends on multiple actions, the meaning is that all actions are dependencies, rather than that any of the actions are a dependency.
        'relatedAction' => {
          'type'=>'PlanDefinition::Action::RelatedAction',
          'path'=>'Action.relatedAction',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # When the action should take place
        # An optional value describing when the action should be performed.
        'timingAge' => {
          'type'=>'Age',
          'path'=>'Action.timing[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # When the action should take place
        # An optional value describing when the action should be performed.
        'timingDateTime' => {
          'type'=>'DateTime',
          'path'=>'Action.timing[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # When the action should take place
        # An optional value describing when the action should be performed.
        'timingDuration' => {
          'type'=>'Duration',
          'path'=>'Action.timing[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # When the action should take place
        # An optional value describing when the action should be performed.
        'timingPeriod' => {
          'type'=>'Period',
          'path'=>'Action.timing[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # When the action should take place
        # An optional value describing when the action should be performed.
        'timingRange' => {
          'type'=>'Range',
          'path'=>'Action.timing[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # When the action should take place
        # An optional value describing when the action should be performed.
        'timingTiming' => {
          'type'=>'Timing',
          'path'=>'Action.timing[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Who should participate in the action
        # Indicates who should participate in performing the action described.
        'participant' => {
          'type'=>'PlanDefinition::Action::Participant',
          'path'=>'Action.participant',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # create | update | remove | fire-event
        # The type of action to perform (create, update, remove).
        'type' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/action-type'=>[ 'create', 'update', 'remove', 'fire-event' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Action.type',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/action-type'}
        },
        ##
        # visual-group | logical-group | sentence-group
        # Defines the grouping behavior for the action and its children.
        'groupingBehavior' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/action-grouping-behavior'=>[ 'visual-group', 'logical-group', 'sentence-group' ]
          },
          'type'=>'code',
          'path'=>'Action.groupingBehavior',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/action-grouping-behavior'}
        },
        ##
        # any | all | all-or-none | exactly-one | at-most-one | one-or-more
        # Defines the selection behavior for the action and its children.
        'selectionBehavior' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/action-selection-behavior'=>[ 'any', 'all', 'all-or-none', 'exactly-one', 'at-most-one', 'one-or-more' ]
          },
          'type'=>'code',
          'path'=>'Action.selectionBehavior',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/action-selection-behavior'}
        },
        ##
        # must | could | must-unless-documented
        # Defines the required behavior for the action.
        'requiredBehavior' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/action-required-behavior'=>[ 'must', 'could', 'must-unless-documented' ]
          },
          'type'=>'code',
          'path'=>'Action.requiredBehavior',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/action-required-behavior'}
        },
        ##
        # yes | no
        # Defines whether the action should usually be preselected.
        'precheckBehavior' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/action-precheck-behavior'=>[ 'yes', 'no' ]
          },
          'type'=>'code',
          'path'=>'Action.precheckBehavior',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/action-precheck-behavior'}
        },
        ##
        # single | multiple
        # Defines whether the action can be selected multiple times.
        'cardinalityBehavior' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/action-cardinality-behavior'=>[ 'single', 'multiple' ]
          },
          'type'=>'code',
          'path'=>'Action.cardinalityBehavior',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/action-cardinality-behavior'}
        },
        ##
        # Description of the activity to be performed
        # A reference to an ActivityDefinition that describes the action to be taken in detail, or a PlanDefinition that describes a series of actions to be taken.
        # Note that the definition is optional, and if no definition is specified, a dynamicValue with a root ($this) path can be used to define the entire resource dynamically.
        'definitionCanonical' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ActivityDefinition', 'http://hl7.org/fhir/StructureDefinition/PlanDefinition', 'http://hl7.org/fhir/StructureDefinition/Questionnaire'],
          'type'=>'Canonical',
          'path'=>'Action.definition[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Description of the activity to be performed
        # A reference to an ActivityDefinition that describes the action to be taken in detail, or a PlanDefinition that describes a series of actions to be taken.
        # Note that the definition is optional, and if no definition is specified, a dynamicValue with a root ($this) path can be used to define the entire resource dynamically.
        'definitionUri' => {
          'type'=>'Uri',
          'path'=>'Action.definition[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Transform to apply the template
        # A reference to a StructureMap resource that defines a transform that can be executed to produce the intent resource using the ActivityDefinition instance as the input.
        # Note that when a referenced ActivityDefinition also defines a transform, the transform specified here generally takes precedence. In addition, if both a transform and dynamic values are specific, the dynamic values are applied to the result of the transform.
        'transform' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/StructureMap'],
          'type'=>'canonical',
          'path'=>'Action.transform',
          'min'=>0,
          'max'=>1
        },
        ##
        # Dynamic aspects of the definition
        # Customizations that should be applied to the statically defined resource. For example, if the dosage of a medication must be computed based on the patient's weight, a customization would be used to specify an expression that calculated the weight, and the path on the resource that would contain the result.
        # Dynamic values are applied in the order in which they are defined in the PlanDefinition resource. Note that when dynamic values are also specified by a referenced ActivityDefinition, the dynamicValues from the ActivityDefinition are applied first, followed by the dynamicValues specified here. In addition, if both a transform and dynamic values are specific, the dynamic values are applied to the result of the transform.
        'dynamicValue' => {
          'type'=>'PlanDefinition::Action::DynamicValue',
          'path'=>'Action.dynamicValue',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # A sub-action
        # Sub actions that are contained within the action. The behavior of this action determines the functionality of the sub-actions. For example, a selection behavior of at-most-one indicates that of the sub-actions, at most one may be chosen as part of realizing the action definition.
        'action' => {
          'type'=>'PlanDefinition::Action',
          'path'=>'Action.action',
          'min'=>0,
          'max'=>Float::INFINITY
        }
      }

      ##
      # Whether or not the action is applicable
      # An expression that describes applicability criteria or start/stop conditions for the action.
      # When multiple conditions of the same kind are present, the effects are combined using AND semantics, so the overall condition is true only if all the conditions are true.
      class Condition < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'id',
            'path'=>'Condition.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Condition.extension',
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
            'path'=>'Condition.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # applicability | start | stop
          # The kind of condition.
          # Applicability criteria are used to determine immediate applicability when a plan definition is applied to a given context. Start and stop criteria are carried through application and used to describe enter/exit criteria for an action.
          'kind' => {
            'valid_codes'=>{
              'http://hl7.org/fhir/action-condition-kind'=>[ 'applicability', 'start', 'stop' ]
            },
            'type'=>'code',
            'path'=>'Condition.kind',
            'min'=>1,
            'max'=>1,
            'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/action-condition-kind'}
          },
          ##
          # Boolean-valued expression
          # An expression that returns true or false, indicating whether the condition is satisfied.
          # The expression may be inlined or may be a reference to a named expression within a logic library referenced by the library element.
          'expression' => {
            'type'=>'Expression',
            'path'=>'Condition.expression',
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
        # applicability | start | stop
        # The kind of condition.
        # Applicability criteria are used to determine immediate applicability when a plan definition is applied to a given context. Start and stop criteria are carried through application and used to describe enter/exit criteria for an action.
        attr_accessor :kind                           # 1-1 code
        ##
        # Boolean-valued expression
        # An expression that returns true or false, indicating whether the condition is satisfied.
        # The expression may be inlined or may be a reference to a named expression within a logic library referenced by the library element.
        attr_accessor :expression                     # 0-1 Expression
      end

      ##
      # Relationship to another action
      # A relationship to another action such as "before" or "30-60 minutes after start of".
      # When an action depends on multiple actions, the meaning is that all actions are dependencies, rather than that any of the actions are a dependency.
      class RelatedAction < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        MULTIPLE_TYPES = {
          'offset[x]' => ['Duration', 'Range']
        }
        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'id',
            'path'=>'RelatedAction.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'RelatedAction.extension',
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
            'path'=>'RelatedAction.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # What action is this related to
          # The element id of the related action.
          'actionId' => {
            'type'=>'id',
            'path'=>'RelatedAction.actionId',
            'min'=>1,
            'max'=>1
          },
          ##
          # before-start | before | before-end | concurrent-with-start | concurrent | concurrent-with-end | after-start | after | after-end
          # The relationship of this action to the related action.
          'relationship' => {
            'valid_codes'=>{
              'http://hl7.org/fhir/action-relationship-type'=>[ 'before-start', 'before', 'before-end', 'concurrent-with-start', 'concurrent', 'concurrent-with-end', 'after-start', 'after', 'after-end' ]
            },
            'type'=>'code',
            'path'=>'RelatedAction.relationship',
            'min'=>1,
            'max'=>1,
            'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/action-relationship-type'}
          },
          ##
          # Time offset for the relationship
          # A duration or range of durations to apply to the relationship. For example, 30-60 minutes before.
          'offsetDuration' => {
            'type'=>'Duration',
            'path'=>'RelatedAction.offset[x]',
            'min'=>0,
            'max'=>1
          }
          ##
          # Time offset for the relationship
          # A duration or range of durations to apply to the relationship. For example, 30-60 minutes before.
          'offsetRange' => {
            'type'=>'Range',
            'path'=>'RelatedAction.offset[x]',
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
        # What action is this related to
        # The element id of the related action.
        attr_accessor :actionId                       # 1-1 id
        ##
        # before-start | before | before-end | concurrent-with-start | concurrent | concurrent-with-end | after-start | after | after-end
        # The relationship of this action to the related action.
        attr_accessor :relationship                   # 1-1 code
        ##
        # Time offset for the relationship
        # A duration or range of durations to apply to the relationship. For example, 30-60 minutes before.
        attr_accessor :offsetDuration                 # 0-1 Duration
        ##
        # Time offset for the relationship
        # A duration or range of durations to apply to the relationship. For example, 30-60 minutes before.
        attr_accessor :offsetRange                    # 0-1 Range
      end

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
            'type'=>'id',
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
          # E.g. Nurse, Surgeon, Parent
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
        # patient | practitioner | related-person | device
        # The type of participant in the action.
        attr_accessor :type                           # 1-1 code
        ##
        # E.g. Nurse, Surgeon, Parent
        # The role the participant should play in performing the described action.
        attr_accessor :role                           # 0-1 CodeableConcept
      end

      ##
      # Dynamic aspects of the definition
      # Customizations that should be applied to the statically defined resource. For example, if the dosage of a medication must be computed based on the patient's weight, a customization would be used to specify an expression that calculated the weight, and the path on the resource that would contain the result.
      # Dynamic values are applied in the order in which they are defined in the PlanDefinition resource. Note that when dynamic values are also specified by a referenced ActivityDefinition, the dynamicValues from the ActivityDefinition are applied first, followed by the dynamicValues specified here. In addition, if both a transform and dynamic values are specific, the dynamic values are applied to the result of the transform.
      class DynamicValue < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'id',
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
          # To specify the path to the current action being realized, the %action environment variable is available in this path. For example, to specify the description element of the target action, the path would be %action.description. The path attribute contains a [Simple FHIRPath Subset](fhirpath.html#simple) that allows path traversal, but not calculation.
          'path' => {
            'type'=>'string',
            'path'=>'DynamicValue.path',
            'min'=>0,
            'max'=>1
          },
          ##
          # An expression that provides the dynamic value for the customization
          # An expression specifying the value of the customized element.
          # The expression may be inlined or may be a reference to a named expression within a logic library referenced by the library element.
          'expression' => {
            'type'=>'Expression',
            'path'=>'DynamicValue.expression',
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
        # The path to the element to be set dynamically
        # The path to the element to be customized. This is the path on the resource that will hold the result of the calculation defined by the expression. The specified path SHALL be a FHIRPath resolveable on the specified target type of the ActivityDefinition, and SHALL consist only of identifiers, constant indexers, and a restricted subset of functions. The path is allowed to contain qualifiers (.) to traverse sub-elements, as well as indexers ([x]) to traverse multiple-cardinality sub-elements (see the [Simple FHIRPath Profile](fhirpath.html#simple) for full details).
        # To specify the path to the current action being realized, the %action environment variable is available in this path. For example, to specify the description element of the target action, the path would be %action.description. The path attribute contains a [Simple FHIRPath Subset](fhirpath.html#simple) that allows path traversal, but not calculation.
        attr_accessor :path                           # 0-1 string
        ##
        # An expression that provides the dynamic value for the customization
        # An expression specifying the value of the customized element.
        # The expression may be inlined or may be a reference to a named expression within a logic library referenced by the library element.
        attr_accessor :expression                     # 0-1 Expression
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
      # User-visible prefix for the action (e.g. 1. or A.)
      # A user-visible prefix for the action.
      attr_accessor :prefix                         # 0-1 string
      ##
      # User-visible title
      # The title of the action displayed to a user.
      attr_accessor :title                          # 0-1 string
      ##
      # Brief description of the action
      # A brief description of the action used to provide a summary to display to the user.
      attr_accessor :description                    # 0-1 string
      ##
      # Static text equivalent of the action, used if the dynamic aspects cannot be interpreted by the receiving system
      # A text equivalent of the action to be performed. This provides a human-interpretable description of the action when the definition is consumed by a system that might not be capable of interpreting it dynamically.
      attr_accessor :textEquivalent                 # 0-1 string
      ##
      # routine | urgent | asap | stat
      # Indicates how quickly the action should be addressed with respect to other actions.
      attr_accessor :priority                       # 0-1 code
      ##
      # Code representing the meaning of the action or sub-actions
      # A code that provides meaning for the action or action group. For example, a section may have a LOINC code for the section of a documentation template.
      attr_accessor :code                           # 0-* [ CodeableConcept ]
      ##
      # Why the action should be performed
      # A description of why this action is necessary or appropriate.
      # This is different than the clinical evidence documentation, it's an actual business description of the reason for performing the action.
      attr_accessor :reason                         # 0-* [ CodeableConcept ]
      ##
      # Supporting documentation for the intended performer of the action
      # Didactic or other informational resources associated with the action that can be provided to the CDS recipient. Information resources can include inline text commentary and links to web resources.
      attr_accessor :documentation                  # 0-* [ RelatedArtifact ]
      ##
      # What goals this action supports
      # Identifies goals that this action supports. The reference must be to a goal element defined within this plan definition.
      attr_accessor :goalId                         # 0-* [ id ]
      ##
      # Type of individual the action is focused on
      # A code or group definition that describes the intended subject of the action and its children, if any.
      # The subject of an action overrides the subject at a parent action or on the root of the PlanDefinition if specified.
      # 
      # In addition, because the subject needs to be resolved during realization, use of subjects in actions (or in the ActivityDefinition referenced by the action) resolves based on the set of subjects supplied in context and by type (i.e. the patient subject would resolve to a resource of type Patient).
      attr_accessor :subjectCodeableConcept         # 0-1 CodeableConcept
      ##
      # Type of individual the action is focused on
      # A code or group definition that describes the intended subject of the action and its children, if any.
      # The subject of an action overrides the subject at a parent action or on the root of the PlanDefinition if specified.
      # 
      # In addition, because the subject needs to be resolved during realization, use of subjects in actions (or in the ActivityDefinition referenced by the action) resolves based on the set of subjects supplied in context and by type (i.e. the patient subject would resolve to a resource of type Patient).
      attr_accessor :subjectReference               # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Group)
      ##
      # When the action should be triggered
      # A description of when the action should be triggered.
      attr_accessor :trigger                        # 0-* [ TriggerDefinition ]
      ##
      # Whether or not the action is applicable
      # An expression that describes applicability criteria or start/stop conditions for the action.
      # When multiple conditions of the same kind are present, the effects are combined using AND semantics, so the overall condition is true only if all the conditions are true.
      attr_accessor :condition                      # 0-* [ PlanDefinition::Action::Condition ]
      ##
      # Input data requirements
      # Defines input data requirements for the action.
      attr_accessor :input                          # 0-* [ DataRequirement ]
      ##
      # Output data definition
      # Defines the outputs of the action, if any.
      attr_accessor :output                         # 0-* [ DataRequirement ]
      ##
      # Relationship to another action
      # A relationship to another action such as "before" or "30-60 minutes after start of".
      # When an action depends on multiple actions, the meaning is that all actions are dependencies, rather than that any of the actions are a dependency.
      attr_accessor :relatedAction                  # 0-* [ PlanDefinition::Action::RelatedAction ]
      ##
      # When the action should take place
      # An optional value describing when the action should be performed.
      attr_accessor :timingAge                      # 0-1 Age
      ##
      # When the action should take place
      # An optional value describing when the action should be performed.
      attr_accessor :timingDateTime                 # 0-1 DateTime
      ##
      # When the action should take place
      # An optional value describing when the action should be performed.
      attr_accessor :timingDuration                 # 0-1 Duration
      ##
      # When the action should take place
      # An optional value describing when the action should be performed.
      attr_accessor :timingPeriod                   # 0-1 Period
      ##
      # When the action should take place
      # An optional value describing when the action should be performed.
      attr_accessor :timingRange                    # 0-1 Range
      ##
      # When the action should take place
      # An optional value describing when the action should be performed.
      attr_accessor :timingTiming                   # 0-1 Timing
      ##
      # Who should participate in the action
      # Indicates who should participate in performing the action described.
      attr_accessor :participant                    # 0-* [ PlanDefinition::Action::Participant ]
      ##
      # create | update | remove | fire-event
      # The type of action to perform (create, update, remove).
      attr_accessor :type                           # 0-1 CodeableConcept
      ##
      # visual-group | logical-group | sentence-group
      # Defines the grouping behavior for the action and its children.
      attr_accessor :groupingBehavior               # 0-1 code
      ##
      # any | all | all-or-none | exactly-one | at-most-one | one-or-more
      # Defines the selection behavior for the action and its children.
      attr_accessor :selectionBehavior              # 0-1 code
      ##
      # must | could | must-unless-documented
      # Defines the required behavior for the action.
      attr_accessor :requiredBehavior               # 0-1 code
      ##
      # yes | no
      # Defines whether the action should usually be preselected.
      attr_accessor :precheckBehavior               # 0-1 code
      ##
      # single | multiple
      # Defines whether the action can be selected multiple times.
      attr_accessor :cardinalityBehavior            # 0-1 code
      ##
      # Description of the activity to be performed
      # A reference to an ActivityDefinition that describes the action to be taken in detail, or a PlanDefinition that describes a series of actions to be taken.
      # Note that the definition is optional, and if no definition is specified, a dynamicValue with a root ($this) path can be used to define the entire resource dynamically.
      attr_accessor :definitionCanonical            # 0-1 Canonical(http://hl7.org/fhir/StructureDefinition/ActivityDefinition|http://hl7.org/fhir/StructureDefinition/PlanDefinition|http://hl7.org/fhir/StructureDefinition/Questionnaire)
      ##
      # Description of the activity to be performed
      # A reference to an ActivityDefinition that describes the action to be taken in detail, or a PlanDefinition that describes a series of actions to be taken.
      # Note that the definition is optional, and if no definition is specified, a dynamicValue with a root ($this) path can be used to define the entire resource dynamically.
      attr_accessor :definitionUri                  # 0-1 Uri
      ##
      # Transform to apply the template
      # A reference to a StructureMap resource that defines a transform that can be executed to produce the intent resource using the ActivityDefinition instance as the input.
      # Note that when a referenced ActivityDefinition also defines a transform, the transform specified here generally takes precedence. In addition, if both a transform and dynamic values are specific, the dynamic values are applied to the result of the transform.
      attr_accessor :transform                      # 0-1 canonical(http://hl7.org/fhir/StructureDefinition/StructureMap)
      ##
      # Dynamic aspects of the definition
      # Customizations that should be applied to the statically defined resource. For example, if the dosage of a medication must be computed based on the patient's weight, a customization would be used to specify an expression that calculated the weight, and the path on the resource that would contain the result.
      # Dynamic values are applied in the order in which they are defined in the PlanDefinition resource. Note that when dynamic values are also specified by a referenced ActivityDefinition, the dynamicValues from the ActivityDefinition are applied first, followed by the dynamicValues specified here. In addition, if both a transform and dynamic values are specific, the dynamic values are applied to the result of the transform.
      attr_accessor :dynamicValue                   # 0-* [ PlanDefinition::Action::DynamicValue ]
      ##
      # A sub-action
      # Sub actions that are contained within the action. The behavior of this action determines the functionality of the sub-actions. For example, a selection behavior of at-most-one indicates that of the sub-actions, at most one may be chosen as part of realizing the action definition.
      attr_accessor :action                         # 0-* [ PlanDefinition::Action ]
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
    # Canonical identifier for this plan definition, represented as a URI (globally unique)
    # An absolute URI that is used to identify this plan definition when it is referenced in a specification, model, design or an instance; also called its canonical identifier. This SHOULD be globally unique and SHOULD be a literal address at which at which an authoritative instance of this plan definition is (or will be) published. This URL can be the target of a canonical reference. It SHALL remain the same when the plan definition is stored on different servers.
    # Can be a urn:uuid: or a urn:oid: but real http: addresses are preferred.  Multiple instances may share the same URL if they have a distinct version.
    # 
    # The determination of when to create a new version of a resource (same url, new version) vs. defining a new artifact is up to the author.  Considerations for making this decision are found in [Technical and Business Versions](resource.html#versions). 
    # 
    # In some cases, the resource can no longer be found at the stated url, but the url itself cannot change. Implementations can use the [meta.source](resource.html#meta) element to indicate where the current master source of the resource can be found.
    attr_accessor :url                            # 0-1 uri
    ##
    # Additional identifier for the plan definition
    # A formal identifier that is used to identify this plan definition when it is represented in other formats, or referenced in a specification, model, design or an instance.
    # Typically, this is used for identifiers that can go in an HL7 V3 II (instance identifier) data type, and can then identify this plan definition outside of FHIR, where it is not possible to use the logical URI.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Business version of the plan definition
    # The identifier that is used to identify this version of the plan definition when it is referenced in a specification, model, design or instance. This is an arbitrary value managed by the plan definition author and is not expected to be globally unique. For example, it might be a timestamp (e.g. yyyymmdd) if a managed version is not available. There is also no expectation that versions can be placed in a lexicographical sequence. To provide a version consistent with the Decision Support Service specification, use the format Major.Minor.Revision (e.g. 1.0.0). For more information on versioning knowledge assets, refer to the Decision Support Service specification. Note that a version is required for non-experimental active artifacts.
    # There may be different plan definition instances that have the same identifier but different versions.  The version can be appended to the url in a reference to allow a reference to a particular business version of the plan definition with the format [url]|[version].
    attr_accessor :version                        # 0-1 string
    ##
    # Name for this plan definition (computer friendly)
    # A natural language name identifying the plan definition. This name should be usable as an identifier for the module by machine processing applications such as code generation.
    # The name is not expected to be globally unique. The name should be a simple alphanumeric type name to ensure that it is machine-processing friendly.
    attr_accessor :name                           # 0-1 string
    ##
    # Name for this plan definition (human friendly)
    # A short, descriptive, user-friendly title for the plan definition.
    # This name does not need to be machine-processing friendly and may contain punctuation, white-space, etc.
    attr_accessor :title                          # 0-1 string
    ##
    # Subordinate title of the plan definition
    # An explanatory or alternate title for the plan definition giving additional information about its content.
    attr_accessor :subtitle                       # 0-1 string
    ##
    # order-set | clinical-protocol | eca-rule | workflow-definition
    # A high-level category for the plan definition that distinguishes the kinds of systems that would be interested in the plan definition.
    attr_accessor :type                           # 0-1 CodeableConcept
    ##
    # draft | active | retired | unknown
    # The status of this plan definition. Enables tracking the life-cycle of the content.
    # Allows filtering of plan definitions that are appropriate for use versus not.
    attr_accessor :status                         # 1-1 code
    ##
    # For testing purposes, not real usage
    # A Boolean value to indicate that this plan definition is authored for testing purposes (or education/evaluation/marketing) and is not intended to be used for genuine usage.
    # Allows filtering of plan definitions that are appropriate for use versus not.
    attr_accessor :experimental                   # 0-1 boolean
    ##
    # Type of individual the plan definition is focused on
    # A code or group definition that describes the intended subject of the plan definition.
    attr_accessor :subjectCodeableConcept         # 0-1 CodeableConcept
    ##
    # Type of individual the plan definition is focused on
    # A code or group definition that describes the intended subject of the plan definition.
    attr_accessor :subjectReference               # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Group)
    ##
    # Date last changed
    # The date  (and optionally time) when the plan definition was published. The date must change when the business version changes and it must change if the status code changes. In addition, it should change when the substantive content of the plan definition changes.
    # Note that this is not the same as the resource last-modified-date, since the resource may be a secondary representation of the plan definition. Additional specific dates may be added as extensions or be found by consulting Provenances associated with past versions of the resource.
    attr_accessor :date                           # 0-1 dateTime
    ##
    # Name of the publisher (organization or individual)
    # The name of the organization or individual that published the plan definition.
    # Usually an organization but may be an individual. The publisher (or steward) of the plan definition is the organization or individual primarily responsible for the maintenance and upkeep of the plan definition. This is not necessarily the same individual or organization that developed and initially authored the content. The publisher is the primary point of contact for questions or issues with the plan definition. This item SHOULD be populated unless the information is available from context.
    attr_accessor :publisher                      # 0-1 string
    ##
    # Contact details for the publisher
    # Contact details to assist a user in finding and communicating with the publisher.
    # May be a web site, an email address, a telephone number, etc.
    attr_accessor :contact                        # 0-* [ ContactDetail ]
    ##
    # Natural language description of the plan definition
    # A free text natural language description of the plan definition from a consumer's perspective.
    # This description can be used to capture details such as why the plan definition was built, comments about misuse, instructions for clinical use and interpretation, literature references, examples from the paper world, etc. It is not a rendering of the plan definition as conveyed in the 'text' field of the resource itself. This item SHOULD be populated unless the information is available from context (e.g. the language of the plan definition is presumed to be the predominant language in the place the plan definition was created).
    attr_accessor :description                    # 0-1 markdown
    ##
    # The context that the content is intended to support
    # The content was developed with a focus and intent of supporting the contexts that are listed. These contexts may be general categories (gender, age, ...) or may be references to specific programs (insurance plans, studies, ...) and may be used to assist with indexing and searching for appropriate plan definition instances.
    # When multiple useContexts are specified, there is no expectation that all or any of the contexts apply.
    attr_accessor :useContext                     # 0-* [ UsageContext ]
    ##
    # Intended jurisdiction for plan definition (if applicable)
    # A legal or geographic region in which the plan definition is intended to be used.
    # It may be possible for the plan definition to be used in jurisdictions other than those for which it was originally designed or intended.
    attr_accessor :jurisdiction                   # 0-* [ CodeableConcept ]
    ##
    # Why this plan definition is defined
    # Explanation of why this plan definition is needed and why it has been designed as it has.
    # This element does not describe the usage of the plan definition. Instead, it provides traceability of ''why'' the resource is either needed or ''why'' it is defined as it is.  This may be used to point to source materials or specifications that drove the structure of this plan definition.
    attr_accessor :purpose                        # 0-1 markdown
    ##
    # Describes the clinical usage of the plan
    # A detailed description of how the plan definition is used from a clinical perspective.
    attr_accessor :usage                          # 0-1 string
    ##
    # Use and/or publishing restrictions
    # A copyright statement relating to the plan definition and/or its contents. Copyright statements are generally legal restrictions on the use and publishing of the plan definition.
    attr_accessor :copyright                      # 0-1 markdown
    ##
    # When the plan definition was approved by publisher
    # The date on which the resource content was approved by the publisher. Approval happens once when the content is officially approved for usage.
    # The 'date' element may be more recent than the approval date because of minor changes or editorial corrections.
    attr_accessor :approvalDate                   # 0-1 date
    ##
    # When the plan definition was last reviewed
    # The date on which the resource content was last reviewed. Review happens periodically after approval but does not change the original approval date.
    # If specified, this date follows the original approval date.
    attr_accessor :lastReviewDate                 # 0-1 date
    ##
    # When the plan definition is expected to be used
    # The period during which the plan definition content was or is planned to be in active use.
    # The effective period for a plan definition  determines when the content is applicable for usage and is independent of publication and review dates. For example, a measure intended to be used for the year 2016 might be published in 2015.
    attr_accessor :effectivePeriod                # 0-1 Period
    ##
    # E.g. Education, Treatment, Assessment
    # Descriptive topics related to the content of the plan definition. Topics provide a high-level categorization of the definition that can be useful for filtering and searching.
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
    # Additional documentation, citations
    # Related artifacts such as additional documentation, justification, or bibliographic references.
    # Each related artifact is either an attachment, or a reference to another resource, but not both.
    attr_accessor :relatedArtifact                # 0-* [ RelatedArtifact ]
    ##
    # Logic used by the plan definition
    # A reference to a Library resource containing any formal logic used by the plan definition.
    attr_accessor :library                        # 0-* [ canonical(http://hl7.org/fhir/StructureDefinition/Library) ]
    ##
    # What the plan is trying to accomplish
    # Goals that describe what the activities within the plan are intended to achieve. For example, weight loss, restoring an activity of daily living, obtaining herd immunity via immunization, meeting a process improvement objective, etc.
    attr_accessor :goal                           # 0-* [ PlanDefinition::Goal ]
    ##
    # Action defined by the plan
    # An action or group of actions to be taken as part of the plan.
    # Note that there is overlap between many of the elements defined here and the ActivityDefinition resource. When an ActivityDefinition is referenced (using the definition element), the overlapping elements in the plan override the content of the referenced ActivityDefinition unless otherwise documented in the specific elements. See the PlanDefinition resource for more detailed information.
    attr_accessor :action                         # 0-* [ PlanDefinition::Action ]

    def resourceType
      'PlanDefinition'
    end
  end
end
