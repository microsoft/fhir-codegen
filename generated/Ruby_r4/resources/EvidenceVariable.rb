module FHIR

  ##
  # The EvidenceVariable resource describes a "PICO" element that knowledge (evidence, assertion, recommendation) is about.
  # Need to be able to define and reuse the definition of individual elements of a research question.
  class EvidenceVariable < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['composed-of', 'context-quantity', 'context-type-quantity', 'context-type-value', 'context-type', 'context', 'date', 'depends-on', 'derived-from', 'description', 'effective', 'identifier', 'jurisdiction', 'name', 'predecessor', 'publisher', 'status', 'successor', 'title', 'topic', 'url', 'version']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'EvidenceVariable.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'EvidenceVariable.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'EvidenceVariable.implicitRules',
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
        'path'=>'EvidenceVariable.language',
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
        'path'=>'EvidenceVariable.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'EvidenceVariable.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'EvidenceVariable.extension',
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
        'path'=>'EvidenceVariable.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Canonical identifier for this evidence variable, represented as a URI (globally unique)
      # An absolute URI that is used to identify this evidence variable when it is referenced in a specification, model, design or an instance; also called its canonical identifier. This SHOULD be globally unique and SHOULD be a literal address at which at which an authoritative instance of this evidence variable is (or will be) published. This URL can be the target of a canonical reference. It SHALL remain the same when the evidence variable is stored on different servers.
      # Can be a urn:uuid: or a urn:oid: but real http: addresses are preferred.  Multiple instances may share the same URL if they have a distinct version.
      # 
      # The determination of when to create a new version of a resource (same url, new version) vs. defining a new artifact is up to the author.  Considerations for making this decision are found in [Technical and Business Versions](resource.html#versions). 
      # 
      # In some cases, the resource can no longer be found at the stated url, but the url itself cannot change. Implementations can use the [meta.source](resource.html#meta) element to indicate where the current master source of the resource can be found.
      'url' => {
        'type'=>'uri',
        'path'=>'EvidenceVariable.url',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional identifier for the evidence variable
      # A formal identifier that is used to identify this evidence variable when it is represented in other formats, or referenced in a specification, model, design or an instance.
      # Typically, this is used for identifiers that can go in an HL7 V3 II (instance identifier) data type, and can then identify this evidence variable outside of FHIR, where it is not possible to use the logical URI.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'EvidenceVariable.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Business version of the evidence variable
      # The identifier that is used to identify this version of the evidence variable when it is referenced in a specification, model, design or instance. This is an arbitrary value managed by the evidence variable author and is not expected to be globally unique. For example, it might be a timestamp (e.g. yyyymmdd) if a managed version is not available. There is also no expectation that versions can be placed in a lexicographical sequence. To provide a version consistent with the Decision Support Service specification, use the format Major.Minor.Revision (e.g. 1.0.0). For more information on versioning knowledge assets, refer to the Decision Support Service specification. Note that a version is required for non-experimental active artifacts.
      # There may be different evidence variable instances that have the same identifier but different versions.  The version can be appended to the url in a reference to allow a reference to a particular business version of the evidence variable with the format [url]|[version].
      'version' => {
        'type'=>'string',
        'path'=>'EvidenceVariable.version',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name for this evidence variable (computer friendly)
      # A natural language name identifying the evidence variable. This name should be usable as an identifier for the module by machine processing applications such as code generation.
      # The name is not expected to be globally unique. The name should be a simple alphanumeric type name to ensure that it is machine-processing friendly.
      'name' => {
        'type'=>'string',
        'path'=>'EvidenceVariable.name',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name for this evidence variable (human friendly)
      # A short, descriptive, user-friendly title for the evidence variable.
      # This name does not need to be machine-processing friendly and may contain punctuation, white-space, etc.
      'title' => {
        'type'=>'string',
        'path'=>'EvidenceVariable.title',
        'min'=>0,
        'max'=>1
      },
      ##
      # Title for use in informal contexts
      # The short title provides an alternate title for use in informal descriptive contexts where the full, formal title is not necessary.
      'shortTitle' => {
        'type'=>'string',
        'path'=>'EvidenceVariable.shortTitle',
        'min'=>0,
        'max'=>1
      },
      ##
      # Subordinate title of the EvidenceVariable
      # An explanatory or alternate title for the EvidenceVariable giving additional information about its content.
      'subtitle' => {
        'type'=>'string',
        'path'=>'EvidenceVariable.subtitle',
        'min'=>0,
        'max'=>1
      },
      ##
      # draft | active | retired | unknown
      # The status of this evidence variable. Enables tracking the life-cycle of the content.
      # Allows filtering of evidence variables that are appropriate for use versus not.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/publication-status'=>[ 'draft', 'active', 'retired', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'EvidenceVariable.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/publication-status'}
      },
      ##
      # Date last changed
      # The date  (and optionally time) when the evidence variable was published. The date must change when the business version changes and it must change if the status code changes. In addition, it should change when the substantive content of the evidence variable changes.
      # Note that this is not the same as the resource last-modified-date, since the resource may be a secondary representation of the evidence variable. Additional specific dates may be added as extensions or be found by consulting Provenances associated with past versions of the resource.
      'date' => {
        'type'=>'dateTime',
        'path'=>'EvidenceVariable.date',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name of the publisher (organization or individual)
      # The name of the organization or individual that published the evidence variable.
      # Usually an organization but may be an individual. The publisher (or steward) of the evidence variable is the organization or individual primarily responsible for the maintenance and upkeep of the evidence variable. This is not necessarily the same individual or organization that developed and initially authored the content. The publisher is the primary point of contact for questions or issues with the evidence variable. This item SHOULD be populated unless the information is available from context.
      'publisher' => {
        'type'=>'string',
        'path'=>'EvidenceVariable.publisher',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contact details for the publisher
      # Contact details to assist a user in finding and communicating with the publisher.
      # May be a web site, an email address, a telephone number, etc.
      'contact' => {
        'type'=>'ContactDetail',
        'path'=>'EvidenceVariable.contact',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Natural language description of the evidence variable
      # A free text natural language description of the evidence variable from a consumer's perspective.
      # This description can be used to capture details such as why the evidence variable was built, comments about misuse, instructions for clinical use and interpretation, literature references, examples from the paper world, etc. It is not a rendering of the evidence variable as conveyed in the 'text' field of the resource itself. This item SHOULD be populated unless the information is available from context (e.g. the language of the evidence variable is presumed to be the predominant language in the place the evidence variable was created).
      'description' => {
        'type'=>'markdown',
        'path'=>'EvidenceVariable.description',
        'min'=>0,
        'max'=>1
      },
      ##
      # Used for footnotes or explanatory notes
      # A human-readable string to clarify or explain concepts about the resource.
      'note' => {
        'type'=>'Annotation',
        'path'=>'EvidenceVariable.note',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The context that the content is intended to support
      # The content was developed with a focus and intent of supporting the contexts that are listed. These contexts may be general categories (gender, age, ...) or may be references to specific programs (insurance plans, studies, ...) and may be used to assist with indexing and searching for appropriate evidence variable instances.
      # When multiple useContexts are specified, there is no expectation that all or any of the contexts apply.
      'useContext' => {
        'type'=>'UsageContext',
        'path'=>'EvidenceVariable.useContext',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Intended jurisdiction for evidence variable (if applicable)
      # A legal or geographic region in which the evidence variable is intended to be used.
      # It may be possible for the evidence variable to be used in jurisdictions other than those for which it was originally designed or intended.
      'jurisdiction' => {
        'type'=>'CodeableConcept',
        'path'=>'EvidenceVariable.jurisdiction',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Use and/or publishing restrictions
      # A copyright statement relating to the evidence variable and/or its contents. Copyright statements are generally legal restrictions on the use and publishing of the evidence variable.
      'copyright' => {
        'type'=>'markdown',
        'path'=>'EvidenceVariable.copyright',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the evidence variable was approved by publisher
      # The date on which the resource content was approved by the publisher. Approval happens once when the content is officially approved for usage.
      # The 'date' element may be more recent than the approval date because of minor changes or editorial corrections.
      'approvalDate' => {
        'type'=>'date',
        'path'=>'EvidenceVariable.approvalDate',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the evidence variable was last reviewed
      # The date on which the resource content was last reviewed. Review happens periodically after approval but does not change the original approval date.
      # If specified, this date follows the original approval date.
      'lastReviewDate' => {
        'type'=>'date',
        'path'=>'EvidenceVariable.lastReviewDate',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the evidence variable is expected to be used
      # The period during which the evidence variable content was or is planned to be in active use.
      # The effective period for a evidence variable  determines when the content is applicable for usage and is independent of publication and review dates. For example, a measure intended to be used for the year 2016 might be published in 2015.
      'effectivePeriod' => {
        'type'=>'Period',
        'path'=>'EvidenceVariable.effectivePeriod',
        'min'=>0,
        'max'=>1
      },
      ##
      # The category of the EvidenceVariable, such as Education, Treatment, Assessment, etc.
      # Descriptive topics related to the content of the EvidenceVariable. Topics provide a high-level categorization grouping types of EvidenceVariables that can be useful for filtering and searching.
      'topic' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/definition-topic'=>[ 'treatment', 'education', 'assessment' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'EvidenceVariable.topic',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/definition-topic'}
      },
      ##
      # Who authored the content
      # An individiual or organization primarily involved in the creation and maintenance of the content.
      'author' => {
        'type'=>'ContactDetail',
        'path'=>'EvidenceVariable.author',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Who edited the content
      # An individual or organization primarily responsible for internal coherence of the content.
      'editor' => {
        'type'=>'ContactDetail',
        'path'=>'EvidenceVariable.editor',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Who reviewed the content
      # An individual or organization primarily responsible for review of some aspect of the content.
      'reviewer' => {
        'type'=>'ContactDetail',
        'path'=>'EvidenceVariable.reviewer',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Who endorsed the content
      # An individual or organization responsible for officially endorsing the content for use in some setting.
      'endorser' => {
        'type'=>'ContactDetail',
        'path'=>'EvidenceVariable.endorser',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional documentation, citations, etc.
      # Related artifacts such as additional documentation, justification, or bibliographic references.
      # Each related artifact is either an attachment, or a reference to another resource, but not both.
      'relatedArtifact' => {
        'type'=>'RelatedArtifact',
        'path'=>'EvidenceVariable.relatedArtifact',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # dichotomous | continuous | descriptive
      # The type of evidence element, a population, an exposure, or an outcome.
      'type' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/variable-type'=>[ 'dichotomous', 'continuous', 'descriptive' ]
        },
        'type'=>'code',
        'path'=>'EvidenceVariable.type',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/variable-type'}
      },
      ##
      # What defines the members of the evidence element
      # A characteristic that defines the members of the evidence element. Multiple characteristics are applied with "and" semantics.
      # Characteristics can be defined flexibly to accommodate different use cases for membership criteria, ranging from simple codes, all the way to using an expression language to express the criteria.
      'characteristic' => {
        'type'=>'EvidenceVariable::Characteristic',
        'path'=>'EvidenceVariable.characteristic',
        'min'=>1,
        'max'=>Float::INFINITY
      }
    }

    ##
    # What defines the members of the evidence element
    # A characteristic that defines the members of the evidence element. Multiple characteristics are applied with "and" semantics.
    # Characteristics can be defined flexibly to accommodate different use cases for membership criteria, ranging from simple codes, all the way to using an expression language to express the criteria.
    class Characteristic < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'definition[x]' => ['canonical', 'CodeableConcept', 'DataRequirement', 'Expression', 'Reference', 'TriggerDefinition'],
        'participantEffective[x]' => ['dateTime', 'Duration', 'Period', 'Timing']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Characteristic.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Characteristic.extension',
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
          'path'=>'Characteristic.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Natural language description of the characteristic
        # A short, natural language description of the characteristic that could be used to communicate the criteria to an end-user.
        'description' => {
          'type'=>'string',
          'path'=>'Characteristic.description',
          'min'=>0,
          'max'=>1
        },
        ##
        # What code or expression defines members?
        # Define members of the evidence element using Codes (such as condition, medication, or observation), Expressions ( using an expression language such as FHIRPath or CQL) or DataRequirements (such as Diabetes diagnosis onset in the last year).
        'definitionCanonical' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ActivityDefinition'],
          'type'=>'Canonical',
          'path'=>'Characteristic.definition[x]',
          'min'=>1,
          'max'=>1
        },
        ##
        # What code or expression defines members?
        # Define members of the evidence element using Codes (such as condition, medication, or observation), Expressions ( using an expression language such as FHIRPath or CQL) or DataRequirements (such as Diabetes diagnosis onset in the last year).
        'definitionCodeableConcept' => {
          'type'=>'CodeableConcept',
          'path'=>'Characteristic.definition[x]',
          'min'=>1,
          'max'=>1
        },
        ##
        # What code or expression defines members?
        # Define members of the evidence element using Codes (such as condition, medication, or observation), Expressions ( using an expression language such as FHIRPath or CQL) or DataRequirements (such as Diabetes diagnosis onset in the last year).
        'definitionDataRequirement' => {
          'type'=>'DataRequirement',
          'path'=>'Characteristic.definition[x]',
          'min'=>1,
          'max'=>1
        },
        ##
        # What code or expression defines members?
        # Define members of the evidence element using Codes (such as condition, medication, or observation), Expressions ( using an expression language such as FHIRPath or CQL) or DataRequirements (such as Diabetes diagnosis onset in the last year).
        'definitionExpression' => {
          'type'=>'Expression',
          'path'=>'Characteristic.definition[x]',
          'min'=>1,
          'max'=>1
        },
        ##
        # What code or expression defines members?
        # Define members of the evidence element using Codes (such as condition, medication, or observation), Expressions ( using an expression language such as FHIRPath or CQL) or DataRequirements (such as Diabetes diagnosis onset in the last year).
        'definitionReference' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Group'],
          'type'=>'Reference',
          'path'=>'Characteristic.definition[x]',
          'min'=>1,
          'max'=>1
        },
        ##
        # What code or expression defines members?
        # Define members of the evidence element using Codes (such as condition, medication, or observation), Expressions ( using an expression language such as FHIRPath or CQL) or DataRequirements (such as Diabetes diagnosis onset in the last year).
        'definitionTriggerDefinition' => {
          'type'=>'TriggerDefinition',
          'path'=>'Characteristic.definition[x]',
          'min'=>1,
          'max'=>1
        },
        ##
        # What code/value pairs define members?
        # Use UsageContext to define the members of the population, such as Age Ranges, Genders, Settings.
        'usageContext' => {
          'type'=>'UsageContext',
          'path'=>'Characteristic.usageContext',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Whether the characteristic includes or excludes members
        # When true, members with this characteristic are excluded from the element.
        'exclude' => {
          'type'=>'boolean',
          'path'=>'Characteristic.exclude',
          'min'=>0,
          'max'=>1
        },
        ##
        # What time period do participants cover
        # Indicates what effective period the study covers.
        'participantEffectiveDateTime' => {
          'type'=>'DateTime',
          'path'=>'Characteristic.participantEffective[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # What time period do participants cover
        # Indicates what effective period the study covers.
        'participantEffectiveDuration' => {
          'type'=>'Duration',
          'path'=>'Characteristic.participantEffective[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # What time period do participants cover
        # Indicates what effective period the study covers.
        'participantEffectivePeriod' => {
          'type'=>'Period',
          'path'=>'Characteristic.participantEffective[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # What time period do participants cover
        # Indicates what effective period the study covers.
        'participantEffectiveTiming' => {
          'type'=>'Timing',
          'path'=>'Characteristic.participantEffective[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Observation time from study start
        # Indicates duration from the participant's study entry.
        'timeFromStart' => {
          'type'=>'Duration',
          'path'=>'Characteristic.timeFromStart',
          'min'=>0,
          'max'=>1
        },
        ##
        # mean | median | mean-of-mean | mean-of-median | median-of-mean | median-of-median
        # Indicates how elements are aggregated within the study effective period.
        'groupMeasure' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/group-measure'=>[ 'mean', 'median', 'mean-of-mean', 'mean-of-median', 'median-of-mean', 'median-of-median' ]
          },
          'type'=>'code',
          'path'=>'Characteristic.groupMeasure',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/group-measure'}
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
      # Natural language description of the characteristic
      # A short, natural language description of the characteristic that could be used to communicate the criteria to an end-user.
      attr_accessor :description                    # 0-1 string
      ##
      # What code or expression defines members?
      # Define members of the evidence element using Codes (such as condition, medication, or observation), Expressions ( using an expression language such as FHIRPath or CQL) or DataRequirements (such as Diabetes diagnosis onset in the last year).
      attr_accessor :definitionCanonical            # 1-1 Canonical(http://hl7.org/fhir/StructureDefinition/ActivityDefinition)
      ##
      # What code or expression defines members?
      # Define members of the evidence element using Codes (such as condition, medication, or observation), Expressions ( using an expression language such as FHIRPath or CQL) or DataRequirements (such as Diabetes diagnosis onset in the last year).
      attr_accessor :definitionCodeableConcept      # 1-1 CodeableConcept
      ##
      # What code or expression defines members?
      # Define members of the evidence element using Codes (such as condition, medication, or observation), Expressions ( using an expression language such as FHIRPath or CQL) or DataRequirements (such as Diabetes diagnosis onset in the last year).
      attr_accessor :definitionDataRequirement      # 1-1 DataRequirement
      ##
      # What code or expression defines members?
      # Define members of the evidence element using Codes (such as condition, medication, or observation), Expressions ( using an expression language such as FHIRPath or CQL) or DataRequirements (such as Diabetes diagnosis onset in the last year).
      attr_accessor :definitionExpression           # 1-1 Expression
      ##
      # What code or expression defines members?
      # Define members of the evidence element using Codes (such as condition, medication, or observation), Expressions ( using an expression language such as FHIRPath or CQL) or DataRequirements (such as Diabetes diagnosis onset in the last year).
      attr_accessor :definitionReference            # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Group)
      ##
      # What code or expression defines members?
      # Define members of the evidence element using Codes (such as condition, medication, or observation), Expressions ( using an expression language such as FHIRPath or CQL) or DataRequirements (such as Diabetes diagnosis onset in the last year).
      attr_accessor :definitionTriggerDefinition    # 1-1 TriggerDefinition
      ##
      # What code/value pairs define members?
      # Use UsageContext to define the members of the population, such as Age Ranges, Genders, Settings.
      attr_accessor :usageContext                   # 0-* [ UsageContext ]
      ##
      # Whether the characteristic includes or excludes members
      # When true, members with this characteristic are excluded from the element.
      attr_accessor :exclude                        # 0-1 boolean
      ##
      # What time period do participants cover
      # Indicates what effective period the study covers.
      attr_accessor :participantEffectiveDateTime   # 0-1 DateTime
      ##
      # What time period do participants cover
      # Indicates what effective period the study covers.
      attr_accessor :participantEffectiveDuration   # 0-1 Duration
      ##
      # What time period do participants cover
      # Indicates what effective period the study covers.
      attr_accessor :participantEffectivePeriod     # 0-1 Period
      ##
      # What time period do participants cover
      # Indicates what effective period the study covers.
      attr_accessor :participantEffectiveTiming     # 0-1 Timing
      ##
      # Observation time from study start
      # Indicates duration from the participant's study entry.
      attr_accessor :timeFromStart                  # 0-1 Duration
      ##
      # mean | median | mean-of-mean | mean-of-median | median-of-mean | median-of-median
      # Indicates how elements are aggregated within the study effective period.
      attr_accessor :groupMeasure                   # 0-1 code
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
    # Canonical identifier for this evidence variable, represented as a URI (globally unique)
    # An absolute URI that is used to identify this evidence variable when it is referenced in a specification, model, design or an instance; also called its canonical identifier. This SHOULD be globally unique and SHOULD be a literal address at which at which an authoritative instance of this evidence variable is (or will be) published. This URL can be the target of a canonical reference. It SHALL remain the same when the evidence variable is stored on different servers.
    # Can be a urn:uuid: or a urn:oid: but real http: addresses are preferred.  Multiple instances may share the same URL if they have a distinct version.
    # 
    # The determination of when to create a new version of a resource (same url, new version) vs. defining a new artifact is up to the author.  Considerations for making this decision are found in [Technical and Business Versions](resource.html#versions). 
    # 
    # In some cases, the resource can no longer be found at the stated url, but the url itself cannot change. Implementations can use the [meta.source](resource.html#meta) element to indicate where the current master source of the resource can be found.
    attr_accessor :url                            # 0-1 uri
    ##
    # Additional identifier for the evidence variable
    # A formal identifier that is used to identify this evidence variable when it is represented in other formats, or referenced in a specification, model, design or an instance.
    # Typically, this is used for identifiers that can go in an HL7 V3 II (instance identifier) data type, and can then identify this evidence variable outside of FHIR, where it is not possible to use the logical URI.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Business version of the evidence variable
    # The identifier that is used to identify this version of the evidence variable when it is referenced in a specification, model, design or instance. This is an arbitrary value managed by the evidence variable author and is not expected to be globally unique. For example, it might be a timestamp (e.g. yyyymmdd) if a managed version is not available. There is also no expectation that versions can be placed in a lexicographical sequence. To provide a version consistent with the Decision Support Service specification, use the format Major.Minor.Revision (e.g. 1.0.0). For more information on versioning knowledge assets, refer to the Decision Support Service specification. Note that a version is required for non-experimental active artifacts.
    # There may be different evidence variable instances that have the same identifier but different versions.  The version can be appended to the url in a reference to allow a reference to a particular business version of the evidence variable with the format [url]|[version].
    attr_accessor :version                        # 0-1 string
    ##
    # Name for this evidence variable (computer friendly)
    # A natural language name identifying the evidence variable. This name should be usable as an identifier for the module by machine processing applications such as code generation.
    # The name is not expected to be globally unique. The name should be a simple alphanumeric type name to ensure that it is machine-processing friendly.
    attr_accessor :name                           # 0-1 string
    ##
    # Name for this evidence variable (human friendly)
    # A short, descriptive, user-friendly title for the evidence variable.
    # This name does not need to be machine-processing friendly and may contain punctuation, white-space, etc.
    attr_accessor :title                          # 0-1 string
    ##
    # Title for use in informal contexts
    # The short title provides an alternate title for use in informal descriptive contexts where the full, formal title is not necessary.
    attr_accessor :shortTitle                     # 0-1 string
    ##
    # Subordinate title of the EvidenceVariable
    # An explanatory or alternate title for the EvidenceVariable giving additional information about its content.
    attr_accessor :subtitle                       # 0-1 string
    ##
    # draft | active | retired | unknown
    # The status of this evidence variable. Enables tracking the life-cycle of the content.
    # Allows filtering of evidence variables that are appropriate for use versus not.
    attr_accessor :status                         # 1-1 code
    ##
    # Date last changed
    # The date  (and optionally time) when the evidence variable was published. The date must change when the business version changes and it must change if the status code changes. In addition, it should change when the substantive content of the evidence variable changes.
    # Note that this is not the same as the resource last-modified-date, since the resource may be a secondary representation of the evidence variable. Additional specific dates may be added as extensions or be found by consulting Provenances associated with past versions of the resource.
    attr_accessor :date                           # 0-1 dateTime
    ##
    # Name of the publisher (organization or individual)
    # The name of the organization or individual that published the evidence variable.
    # Usually an organization but may be an individual. The publisher (or steward) of the evidence variable is the organization or individual primarily responsible for the maintenance and upkeep of the evidence variable. This is not necessarily the same individual or organization that developed and initially authored the content. The publisher is the primary point of contact for questions or issues with the evidence variable. This item SHOULD be populated unless the information is available from context.
    attr_accessor :publisher                      # 0-1 string
    ##
    # Contact details for the publisher
    # Contact details to assist a user in finding and communicating with the publisher.
    # May be a web site, an email address, a telephone number, etc.
    attr_accessor :contact                        # 0-* [ ContactDetail ]
    ##
    # Natural language description of the evidence variable
    # A free text natural language description of the evidence variable from a consumer's perspective.
    # This description can be used to capture details such as why the evidence variable was built, comments about misuse, instructions for clinical use and interpretation, literature references, examples from the paper world, etc. It is not a rendering of the evidence variable as conveyed in the 'text' field of the resource itself. This item SHOULD be populated unless the information is available from context (e.g. the language of the evidence variable is presumed to be the predominant language in the place the evidence variable was created).
    attr_accessor :description                    # 0-1 markdown
    ##
    # Used for footnotes or explanatory notes
    # A human-readable string to clarify or explain concepts about the resource.
    attr_accessor :note                           # 0-* [ Annotation ]
    ##
    # The context that the content is intended to support
    # The content was developed with a focus and intent of supporting the contexts that are listed. These contexts may be general categories (gender, age, ...) or may be references to specific programs (insurance plans, studies, ...) and may be used to assist with indexing and searching for appropriate evidence variable instances.
    # When multiple useContexts are specified, there is no expectation that all or any of the contexts apply.
    attr_accessor :useContext                     # 0-* [ UsageContext ]
    ##
    # Intended jurisdiction for evidence variable (if applicable)
    # A legal or geographic region in which the evidence variable is intended to be used.
    # It may be possible for the evidence variable to be used in jurisdictions other than those for which it was originally designed or intended.
    attr_accessor :jurisdiction                   # 0-* [ CodeableConcept ]
    ##
    # Use and/or publishing restrictions
    # A copyright statement relating to the evidence variable and/or its contents. Copyright statements are generally legal restrictions on the use and publishing of the evidence variable.
    attr_accessor :copyright                      # 0-1 markdown
    ##
    # When the evidence variable was approved by publisher
    # The date on which the resource content was approved by the publisher. Approval happens once when the content is officially approved for usage.
    # The 'date' element may be more recent than the approval date because of minor changes or editorial corrections.
    attr_accessor :approvalDate                   # 0-1 date
    ##
    # When the evidence variable was last reviewed
    # The date on which the resource content was last reviewed. Review happens periodically after approval but does not change the original approval date.
    # If specified, this date follows the original approval date.
    attr_accessor :lastReviewDate                 # 0-1 date
    ##
    # When the evidence variable is expected to be used
    # The period during which the evidence variable content was or is planned to be in active use.
    # The effective period for a evidence variable  determines when the content is applicable for usage and is independent of publication and review dates. For example, a measure intended to be used for the year 2016 might be published in 2015.
    attr_accessor :effectivePeriod                # 0-1 Period
    ##
    # The category of the EvidenceVariable, such as Education, Treatment, Assessment, etc.
    # Descriptive topics related to the content of the EvidenceVariable. Topics provide a high-level categorization grouping types of EvidenceVariables that can be useful for filtering and searching.
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
    # dichotomous | continuous | descriptive
    # The type of evidence element, a population, an exposure, or an outcome.
    attr_accessor :type                           # 0-1 code
    ##
    # What defines the members of the evidence element
    # A characteristic that defines the members of the evidence element. Multiple characteristics are applied with "and" semantics.
    # Characteristics can be defined flexibly to accommodate different use cases for membership criteria, ranging from simple codes, all the way to using an expression language to express the criteria.
    attr_accessor :characteristic                 # 1-* [ EvidenceVariable::Characteristic ]

    def resourceType
      'EvidenceVariable'
    end
  end
end
