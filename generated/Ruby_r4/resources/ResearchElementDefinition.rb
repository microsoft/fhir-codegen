module FHIR

  ##
  # The ResearchElementDefinition resource describes a "PICO" element that knowledge (evidence, assertion, recommendation) is about.
  # Need to be able to define and reuse the definition of individual elements of a research question.
  class ResearchElementDefinition < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['composed-of', 'context-quantity', 'context-type-quantity', 'context-type-value', 'context-type', 'context', 'date', 'depends-on', 'derived-from', 'description', 'effective', 'identifier', 'jurisdiction', 'name', 'predecessor', 'publisher', 'status', 'successor', 'title', 'topic', 'url', 'version']
    MULTIPLE_TYPES = {
      'subject[x]' => ['CodeableConcept', 'Reference']
    }
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'ResearchElementDefinition.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'ResearchElementDefinition.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'ResearchElementDefinition.implicitRules',
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
        'path'=>'ResearchElementDefinition.language',
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
        'path'=>'ResearchElementDefinition.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'ResearchElementDefinition.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'ResearchElementDefinition.extension',
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
        'path'=>'ResearchElementDefinition.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Canonical identifier for this research element definition, represented as a URI (globally unique)
      # An absolute URI that is used to identify this research element definition when it is referenced in a specification, model, design or an instance; also called its canonical identifier. This SHOULD be globally unique and SHOULD be a literal address at which at which an authoritative instance of this research element definition is (or will be) published. This URL can be the target of a canonical reference. It SHALL remain the same when the research element definition is stored on different servers.
      # Can be a urn:uuid: or a urn:oid: but real http: addresses are preferred.  Multiple instances may share the same URL if they have a distinct version.
      # 
      # The determination of when to create a new version of a resource (same url, new version) vs. defining a new artifact is up to the author.  Considerations for making this decision are found in [Technical and Business Versions](resource.html#versions). 
      # 
      # In some cases, the resource can no longer be found at the stated url, but the url itself cannot change. Implementations can use the [meta.source](resource.html#meta) element to indicate where the current master source of the resource can be found.
      'url' => {
        'type'=>'uri',
        'path'=>'ResearchElementDefinition.url',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional identifier for the research element definition
      # A formal identifier that is used to identify this research element definition when it is represented in other formats, or referenced in a specification, model, design or an instance.
      # Typically, this is used for identifiers that can go in an HL7 V3 II (instance identifier) data type, and can then identify this research element definition outside of FHIR, where it is not possible to use the logical URI.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'ResearchElementDefinition.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Business version of the research element definition
      # The identifier that is used to identify this version of the research element definition when it is referenced in a specification, model, design or instance. This is an arbitrary value managed by the research element definition author and is not expected to be globally unique. For example, it might be a timestamp (e.g. yyyymmdd) if a managed version is not available. There is also no expectation that versions can be placed in a lexicographical sequence. To provide a version consistent with the Decision Support Service specification, use the format Major.Minor.Revision (e.g. 1.0.0). For more information on versioning knowledge assets, refer to the Decision Support Service specification. Note that a version is required for non-experimental active artifacts.
      # There may be different research element definition instances that have the same identifier but different versions.  The version can be appended to the url in a reference to allow a reference to a particular business version of the research element definition with the format [url]|[version].
      'version' => {
        'type'=>'string',
        'path'=>'ResearchElementDefinition.version',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name for this research element definition (computer friendly)
      # A natural language name identifying the research element definition. This name should be usable as an identifier for the module by machine processing applications such as code generation.
      # The name is not expected to be globally unique. The name should be a simple alphanumeric type name to ensure that it is machine-processing friendly.
      'name' => {
        'type'=>'string',
        'path'=>'ResearchElementDefinition.name',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name for this research element definition (human friendly)
      # A short, descriptive, user-friendly title for the research element definition.
      # This name does not need to be machine-processing friendly and may contain punctuation, white-space, etc.
      'title' => {
        'type'=>'string',
        'path'=>'ResearchElementDefinition.title',
        'min'=>0,
        'max'=>1
      },
      ##
      # Title for use in informal contexts
      # The short title provides an alternate title for use in informal descriptive contexts where the full, formal title is not necessary.
      'shortTitle' => {
        'type'=>'string',
        'path'=>'ResearchElementDefinition.shortTitle',
        'min'=>0,
        'max'=>1
      },
      ##
      # Subordinate title of the ResearchElementDefinition
      # An explanatory or alternate title for the ResearchElementDefinition giving additional information about its content.
      'subtitle' => {
        'type'=>'string',
        'path'=>'ResearchElementDefinition.subtitle',
        'min'=>0,
        'max'=>1
      },
      ##
      # draft | active | retired | unknown
      # The status of this research element definition. Enables tracking the life-cycle of the content.
      # Allows filtering of research element definitions that are appropriate for use versus not.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/publication-status'=>[ 'draft', 'active', 'retired', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'ResearchElementDefinition.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/publication-status'}
      },
      ##
      # For testing purposes, not real usage
      # A Boolean value to indicate that this research element definition is authored for testing purposes (or education/evaluation/marketing) and is not intended to be used for genuine usage.
      # Allows filtering of research element definitions that are appropriate for use versus not.
      'experimental' => {
        'type'=>'boolean',
        'path'=>'ResearchElementDefinition.experimental',
        'min'=>0,
        'max'=>1
      },
      ##
      # E.g. Patient, Practitioner, RelatedPerson, Organization, Location, Device
      # The intended subjects for the ResearchElementDefinition. If this element is not provided, a Patient subject is assumed, but the subject of the ResearchElementDefinition can be anything.
      # The subject of the ResearchElementDefinition is critical in interpreting the criteria definitions, as the logic in the ResearchElementDefinitions is evaluated with respect to a particular subject. This corresponds roughly to the notion of a Compartment in that it limits what content is available based on its relationship to the subject. In CQL, this corresponds to the context declaration.
      'subjectCodeableConcept' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/resource-types'=>[ 'Patient', 'Practitioner', 'Organization', 'Location', 'Device' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'ResearchElementDefinition.subject[x]',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/subject-type'}
      },
      ##
      # E.g. Patient, Practitioner, RelatedPerson, Organization, Location, Device
      # The intended subjects for the ResearchElementDefinition. If this element is not provided, a Patient subject is assumed, but the subject of the ResearchElementDefinition can be anything.
      # The subject of the ResearchElementDefinition is critical in interpreting the criteria definitions, as the logic in the ResearchElementDefinitions is evaluated with respect to a particular subject. This corresponds roughly to the notion of a Compartment in that it limits what content is available based on its relationship to the subject. In CQL, this corresponds to the context declaration.
      'subjectReference' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/resource-types'=>[ 'Patient', 'Practitioner', 'Organization', 'Location', 'Device' ]
        },
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Group'],
        'type'=>'Reference',
        'path'=>'ResearchElementDefinition.subject[x]',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/subject-type'}
      },
      ##
      # Date last changed
      # The date  (and optionally time) when the research element definition was published. The date must change when the business version changes and it must change if the status code changes. In addition, it should change when the substantive content of the research element definition changes.
      # Note that this is not the same as the resource last-modified-date, since the resource may be a secondary representation of the research element definition. Additional specific dates may be added as extensions or be found by consulting Provenances associated with past versions of the resource.
      'date' => {
        'type'=>'dateTime',
        'path'=>'ResearchElementDefinition.date',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name of the publisher (organization or individual)
      # The name of the organization or individual that published the research element definition.
      # Usually an organization but may be an individual. The publisher (or steward) of the research element definition is the organization or individual primarily responsible for the maintenance and upkeep of the research element definition. This is not necessarily the same individual or organization that developed and initially authored the content. The publisher is the primary point of contact for questions or issues with the research element definition. This item SHOULD be populated unless the information is available from context.
      'publisher' => {
        'type'=>'string',
        'path'=>'ResearchElementDefinition.publisher',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contact details for the publisher
      # Contact details to assist a user in finding and communicating with the publisher.
      # May be a web site, an email address, a telephone number, etc.
      'contact' => {
        'type'=>'ContactDetail',
        'path'=>'ResearchElementDefinition.contact',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Natural language description of the research element definition
      # A free text natural language description of the research element definition from a consumer's perspective.
      # This description can be used to capture details such as why the research element definition was built, comments about misuse, instructions for clinical use and interpretation, literature references, examples from the paper world, etc. It is not a rendering of the research element definition as conveyed in the 'text' field of the resource itself. This item SHOULD be populated unless the information is available from context (e.g. the language of the research element definition is presumed to be the predominant language in the place the research element definition was created).
      'description' => {
        'type'=>'markdown',
        'path'=>'ResearchElementDefinition.description',
        'min'=>0,
        'max'=>1
      },
      ##
      # Used for footnotes or explanatory notes
      # A human-readable string to clarify or explain concepts about the resource.
      'comment' => {
        'type'=>'string',
        'path'=>'ResearchElementDefinition.comment',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The context that the content is intended to support
      # The content was developed with a focus and intent of supporting the contexts that are listed. These contexts may be general categories (gender, age, ...) or may be references to specific programs (insurance plans, studies, ...) and may be used to assist with indexing and searching for appropriate research element definition instances.
      # When multiple useContexts are specified, there is no expectation that all or any of the contexts apply.
      'useContext' => {
        'type'=>'UsageContext',
        'path'=>'ResearchElementDefinition.useContext',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Intended jurisdiction for research element definition (if applicable)
      # A legal or geographic region in which the research element definition is intended to be used.
      # It may be possible for the research element definition to be used in jurisdictions other than those for which it was originally designed or intended.
      'jurisdiction' => {
        'type'=>'CodeableConcept',
        'path'=>'ResearchElementDefinition.jurisdiction',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Why this research element definition is defined
      # Explanation of why this research element definition is needed and why it has been designed as it has.
      # This element does not describe the usage of the research element definition. Instead, it provides traceability of ''why'' the resource is either needed or ''why'' it is defined as it is.  This may be used to point to source materials or specifications that drove the structure of this research element definition.
      'purpose' => {
        'type'=>'markdown',
        'path'=>'ResearchElementDefinition.purpose',
        'min'=>0,
        'max'=>1
      },
      ##
      # Describes the clinical usage of the ResearchElementDefinition
      # A detailed description, from a clinical perspective, of how the ResearchElementDefinition is used.
      'usage' => {
        'type'=>'string',
        'path'=>'ResearchElementDefinition.usage',
        'min'=>0,
        'max'=>1
      },
      ##
      # Use and/or publishing restrictions
      # A copyright statement relating to the research element definition and/or its contents. Copyright statements are generally legal restrictions on the use and publishing of the research element definition.
      'copyright' => {
        'type'=>'markdown',
        'path'=>'ResearchElementDefinition.copyright',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the research element definition was approved by publisher
      # The date on which the resource content was approved by the publisher. Approval happens once when the content is officially approved for usage.
      # The 'date' element may be more recent than the approval date because of minor changes or editorial corrections.
      'approvalDate' => {
        'type'=>'date',
        'path'=>'ResearchElementDefinition.approvalDate',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the research element definition was last reviewed
      # The date on which the resource content was last reviewed. Review happens periodically after approval but does not change the original approval date.
      # If specified, this date follows the original approval date.
      'lastReviewDate' => {
        'type'=>'date',
        'path'=>'ResearchElementDefinition.lastReviewDate',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the research element definition is expected to be used
      # The period during which the research element definition content was or is planned to be in active use.
      # The effective period for a research element definition  determines when the content is applicable for usage and is independent of publication and review dates. For example, a measure intended to be used for the year 2016 might be published in 2015.
      'effectivePeriod' => {
        'type'=>'Period',
        'path'=>'ResearchElementDefinition.effectivePeriod',
        'min'=>0,
        'max'=>1
      },
      ##
      # The category of the ResearchElementDefinition, such as Education, Treatment, Assessment, etc.
      # Descriptive topics related to the content of the ResearchElementDefinition. Topics provide a high-level categorization grouping types of ResearchElementDefinitions that can be useful for filtering and searching.
      'topic' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/definition-topic'=>[ 'treatment', 'education', 'assessment' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'ResearchElementDefinition.topic',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/definition-topic'}
      },
      ##
      # Who authored the content
      # An individiual or organization primarily involved in the creation and maintenance of the content.
      'author' => {
        'type'=>'ContactDetail',
        'path'=>'ResearchElementDefinition.author',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Who edited the content
      # An individual or organization primarily responsible for internal coherence of the content.
      'editor' => {
        'type'=>'ContactDetail',
        'path'=>'ResearchElementDefinition.editor',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Who reviewed the content
      # An individual or organization primarily responsible for review of some aspect of the content.
      'reviewer' => {
        'type'=>'ContactDetail',
        'path'=>'ResearchElementDefinition.reviewer',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Who endorsed the content
      # An individual or organization responsible for officially endorsing the content for use in some setting.
      'endorser' => {
        'type'=>'ContactDetail',
        'path'=>'ResearchElementDefinition.endorser',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional documentation, citations, etc.
      # Related artifacts such as additional documentation, justification, or bibliographic references.
      # Each related artifact is either an attachment, or a reference to another resource, but not both.
      'relatedArtifact' => {
        'type'=>'RelatedArtifact',
        'path'=>'ResearchElementDefinition.relatedArtifact',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Logic used by the ResearchElementDefinition
      # A reference to a Library resource containing the formal logic used by the ResearchElementDefinition.
      'library' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Library'],
        'type'=>'canonical',
        'path'=>'ResearchElementDefinition.library',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # population | exposure | outcome
      # The type of research element, a population, an exposure, or an outcome.
      'type' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/research-element-type'=>[ 'population', 'exposure', 'outcome' ]
        },
        'type'=>'code',
        'path'=>'ResearchElementDefinition.type',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/research-element-type'}
      },
      ##
      # dichotomous | continuous | descriptive
      # The type of the outcome (e.g. Dichotomous, Continuous, or Descriptive).
      'variableType' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/variable-type'=>[ 'dichotomous', 'continuous', 'descriptive' ]
        },
        'type'=>'code',
        'path'=>'ResearchElementDefinition.variableType',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/variable-type'}
      },
      ##
      # What defines the members of the research element
      # A characteristic that defines the members of the research element. Multiple characteristics are applied with "and" semantics.
      # Characteristics can be defined flexibly to accommodate different use cases for membership criteria, ranging from simple codes, all the way to using an expression language to express the criteria.
      'characteristic' => {
        'type'=>'ResearchElementDefinition::Characteristic',
        'path'=>'ResearchElementDefinition.characteristic',
        'min'=>1,
        'max'=>Float::INFINITY
      }
    }

    ##
    # What defines the members of the research element
    # A characteristic that defines the members of the research element. Multiple characteristics are applied with "and" semantics.
    # Characteristics can be defined flexibly to accommodate different use cases for membership criteria, ranging from simple codes, all the way to using an expression language to express the criteria.
    class Characteristic < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'definition[x]' => ['canonical', 'CodeableConcept', 'DataRequirement', 'Expression'],
        'studyEffective[x]' => ['dateTime', 'Duration', 'Period', 'Timing'],
        'participantEffective[x]' => ['dateTime', 'Duration', 'Period', 'Timing']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
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
        # What code or expression defines members?
        # Define members of the research element using Codes (such as condition, medication, or observation), Expressions ( using an expression language such as FHIRPath or CQL) or DataRequirements (such as Diabetes diagnosis onset in the last year).
        'definitionCanonical' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ValueSet'],
          'type'=>'Canonical',
          'path'=>'Characteristic.definition[x]',
          'min'=>1,
          'max'=>1
        },
        ##
        # What code or expression defines members?
        # Define members of the research element using Codes (such as condition, medication, or observation), Expressions ( using an expression language such as FHIRPath or CQL) or DataRequirements (such as Diabetes diagnosis onset in the last year).
        'definitionCodeableConcept' => {
          'type'=>'CodeableConcept',
          'path'=>'Characteristic.definition[x]',
          'min'=>1,
          'max'=>1
        },
        ##
        # What code or expression defines members?
        # Define members of the research element using Codes (such as condition, medication, or observation), Expressions ( using an expression language such as FHIRPath or CQL) or DataRequirements (such as Diabetes diagnosis onset in the last year).
        'definitionDataRequirement' => {
          'type'=>'DataRequirement',
          'path'=>'Characteristic.definition[x]',
          'min'=>1,
          'max'=>1
        },
        ##
        # What code or expression defines members?
        # Define members of the research element using Codes (such as condition, medication, or observation), Expressions ( using an expression language such as FHIRPath or CQL) or DataRequirements (such as Diabetes diagnosis onset in the last year).
        'definitionExpression' => {
          'type'=>'Expression',
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
        # What unit is the outcome described in?
        # Specifies the UCUM unit for the outcome.
        'unitOfMeasure' => {
          'type'=>'CodeableConcept',
          'path'=>'Characteristic.unitOfMeasure',
          'min'=>0,
          'max'=>1
        },
        ##
        # What time period does the study cover
        # A narrative description of the time period the study covers.
        'studyEffectiveDescription' => {
          'type'=>'string',
          'path'=>'Characteristic.studyEffectiveDescription',
          'min'=>0,
          'max'=>1
        },
        ##
        # What time period does the study cover
        # Indicates what effective period the study covers.
        'studyEffectiveDateTime' => {
          'type'=>'DateTime',
          'path'=>'Characteristic.studyEffective[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # What time period does the study cover
        # Indicates what effective period the study covers.
        'studyEffectiveDuration' => {
          'type'=>'Duration',
          'path'=>'Characteristic.studyEffective[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # What time period does the study cover
        # Indicates what effective period the study covers.
        'studyEffectivePeriod' => {
          'type'=>'Period',
          'path'=>'Characteristic.studyEffective[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # What time period does the study cover
        # Indicates what effective period the study covers.
        'studyEffectiveTiming' => {
          'type'=>'Timing',
          'path'=>'Characteristic.studyEffective[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Observation time from study start
        # Indicates duration from the study initiation.
        'studyEffectiveTimeFromStart' => {
          'type'=>'Duration',
          'path'=>'Characteristic.studyEffectiveTimeFromStart',
          'min'=>0,
          'max'=>1
        },
        ##
        # mean | median | mean-of-mean | mean-of-median | median-of-mean | median-of-median
        # Indicates how elements are aggregated within the study effective period.
        'studyEffectiveGroupMeasure' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/group-measure'=>[ 'mean', 'median', 'mean-of-mean', 'mean-of-median', 'median-of-mean', 'median-of-median' ]
          },
          'type'=>'code',
          'path'=>'Characteristic.studyEffectiveGroupMeasure',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/group-measure'}
        },
        ##
        # What time period do participants cover
        # A narrative description of the time period the study covers.
        'participantEffectiveDescription' => {
          'type'=>'string',
          'path'=>'Characteristic.participantEffectiveDescription',
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
        'participantEffectiveTimeFromStart' => {
          'type'=>'Duration',
          'path'=>'Characteristic.participantEffectiveTimeFromStart',
          'min'=>0,
          'max'=>1
        },
        ##
        # mean | median | mean-of-mean | mean-of-median | median-of-mean | median-of-median
        # Indicates how elements are aggregated within the study effective period.
        'participantEffectiveGroupMeasure' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/group-measure'=>[ 'mean', 'median', 'mean-of-mean', 'mean-of-median', 'median-of-mean', 'median-of-median' ]
          },
          'type'=>'code',
          'path'=>'Characteristic.participantEffectiveGroupMeasure',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/group-measure'}
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
      # What code or expression defines members?
      # Define members of the research element using Codes (such as condition, medication, or observation), Expressions ( using an expression language such as FHIRPath or CQL) or DataRequirements (such as Diabetes diagnosis onset in the last year).
      attr_accessor :definitionCanonical            # 1-1 Canonical(http://hl7.org/fhir/StructureDefinition/ValueSet)
      ##
      # What code or expression defines members?
      # Define members of the research element using Codes (such as condition, medication, or observation), Expressions ( using an expression language such as FHIRPath or CQL) or DataRequirements (such as Diabetes diagnosis onset in the last year).
      attr_accessor :definitionCodeableConcept      # 1-1 CodeableConcept
      ##
      # What code or expression defines members?
      # Define members of the research element using Codes (such as condition, medication, or observation), Expressions ( using an expression language such as FHIRPath or CQL) or DataRequirements (such as Diabetes diagnosis onset in the last year).
      attr_accessor :definitionDataRequirement      # 1-1 DataRequirement
      ##
      # What code or expression defines members?
      # Define members of the research element using Codes (such as condition, medication, or observation), Expressions ( using an expression language such as FHIRPath or CQL) or DataRequirements (such as Diabetes diagnosis onset in the last year).
      attr_accessor :definitionExpression           # 1-1 Expression
      ##
      # What code/value pairs define members?
      # Use UsageContext to define the members of the population, such as Age Ranges, Genders, Settings.
      attr_accessor :usageContext                   # 0-* [ UsageContext ]
      ##
      # Whether the characteristic includes or excludes members
      # When true, members with this characteristic are excluded from the element.
      attr_accessor :exclude                        # 0-1 boolean
      ##
      # What unit is the outcome described in?
      # Specifies the UCUM unit for the outcome.
      attr_accessor :unitOfMeasure                  # 0-1 CodeableConcept
      ##
      # What time period does the study cover
      # A narrative description of the time period the study covers.
      attr_accessor :studyEffectiveDescription      # 0-1 string
      ##
      # What time period does the study cover
      # Indicates what effective period the study covers.
      attr_accessor :studyEffectiveDateTime         # 0-1 DateTime
      ##
      # What time period does the study cover
      # Indicates what effective period the study covers.
      attr_accessor :studyEffectiveDuration         # 0-1 Duration
      ##
      # What time period does the study cover
      # Indicates what effective period the study covers.
      attr_accessor :studyEffectivePeriod           # 0-1 Period
      ##
      # What time period does the study cover
      # Indicates what effective period the study covers.
      attr_accessor :studyEffectiveTiming           # 0-1 Timing
      ##
      # Observation time from study start
      # Indicates duration from the study initiation.
      attr_accessor :studyEffectiveTimeFromStart    # 0-1 Duration
      ##
      # mean | median | mean-of-mean | mean-of-median | median-of-mean | median-of-median
      # Indicates how elements are aggregated within the study effective period.
      attr_accessor :studyEffectiveGroupMeasure     # 0-1 code
      ##
      # What time period do participants cover
      # A narrative description of the time period the study covers.
      attr_accessor :participantEffectiveDescription # 0-1 string
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
      attr_accessor :participantEffectiveTimeFromStart # 0-1 Duration
      ##
      # mean | median | mean-of-mean | mean-of-median | median-of-mean | median-of-median
      # Indicates how elements are aggregated within the study effective period.
      attr_accessor :participantEffectiveGroupMeasure # 0-1 code
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
    # Canonical identifier for this research element definition, represented as a URI (globally unique)
    # An absolute URI that is used to identify this research element definition when it is referenced in a specification, model, design or an instance; also called its canonical identifier. This SHOULD be globally unique and SHOULD be a literal address at which at which an authoritative instance of this research element definition is (or will be) published. This URL can be the target of a canonical reference. It SHALL remain the same when the research element definition is stored on different servers.
    # Can be a urn:uuid: or a urn:oid: but real http: addresses are preferred.  Multiple instances may share the same URL if they have a distinct version.
    # 
    # The determination of when to create a new version of a resource (same url, new version) vs. defining a new artifact is up to the author.  Considerations for making this decision are found in [Technical and Business Versions](resource.html#versions). 
    # 
    # In some cases, the resource can no longer be found at the stated url, but the url itself cannot change. Implementations can use the [meta.source](resource.html#meta) element to indicate where the current master source of the resource can be found.
    attr_accessor :url                            # 0-1 uri
    ##
    # Additional identifier for the research element definition
    # A formal identifier that is used to identify this research element definition when it is represented in other formats, or referenced in a specification, model, design or an instance.
    # Typically, this is used for identifiers that can go in an HL7 V3 II (instance identifier) data type, and can then identify this research element definition outside of FHIR, where it is not possible to use the logical URI.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Business version of the research element definition
    # The identifier that is used to identify this version of the research element definition when it is referenced in a specification, model, design or instance. This is an arbitrary value managed by the research element definition author and is not expected to be globally unique. For example, it might be a timestamp (e.g. yyyymmdd) if a managed version is not available. There is also no expectation that versions can be placed in a lexicographical sequence. To provide a version consistent with the Decision Support Service specification, use the format Major.Minor.Revision (e.g. 1.0.0). For more information on versioning knowledge assets, refer to the Decision Support Service specification. Note that a version is required for non-experimental active artifacts.
    # There may be different research element definition instances that have the same identifier but different versions.  The version can be appended to the url in a reference to allow a reference to a particular business version of the research element definition with the format [url]|[version].
    attr_accessor :version                        # 0-1 string
    ##
    # Name for this research element definition (computer friendly)
    # A natural language name identifying the research element definition. This name should be usable as an identifier for the module by machine processing applications such as code generation.
    # The name is not expected to be globally unique. The name should be a simple alphanumeric type name to ensure that it is machine-processing friendly.
    attr_accessor :name                           # 0-1 string
    ##
    # Name for this research element definition (human friendly)
    # A short, descriptive, user-friendly title for the research element definition.
    # This name does not need to be machine-processing friendly and may contain punctuation, white-space, etc.
    attr_accessor :title                          # 0-1 string
    ##
    # Title for use in informal contexts
    # The short title provides an alternate title for use in informal descriptive contexts where the full, formal title is not necessary.
    attr_accessor :shortTitle                     # 0-1 string
    ##
    # Subordinate title of the ResearchElementDefinition
    # An explanatory or alternate title for the ResearchElementDefinition giving additional information about its content.
    attr_accessor :subtitle                       # 0-1 string
    ##
    # draft | active | retired | unknown
    # The status of this research element definition. Enables tracking the life-cycle of the content.
    # Allows filtering of research element definitions that are appropriate for use versus not.
    attr_accessor :status                         # 1-1 code
    ##
    # For testing purposes, not real usage
    # A Boolean value to indicate that this research element definition is authored for testing purposes (or education/evaluation/marketing) and is not intended to be used for genuine usage.
    # Allows filtering of research element definitions that are appropriate for use versus not.
    attr_accessor :experimental                   # 0-1 boolean
    ##
    # E.g. Patient, Practitioner, RelatedPerson, Organization, Location, Device
    # The intended subjects for the ResearchElementDefinition. If this element is not provided, a Patient subject is assumed, but the subject of the ResearchElementDefinition can be anything.
    # The subject of the ResearchElementDefinition is critical in interpreting the criteria definitions, as the logic in the ResearchElementDefinitions is evaluated with respect to a particular subject. This corresponds roughly to the notion of a Compartment in that it limits what content is available based on its relationship to the subject. In CQL, this corresponds to the context declaration.
    attr_accessor :subjectCodeableConcept         # 0-1 CodeableConcept
    ##
    # E.g. Patient, Practitioner, RelatedPerson, Organization, Location, Device
    # The intended subjects for the ResearchElementDefinition. If this element is not provided, a Patient subject is assumed, but the subject of the ResearchElementDefinition can be anything.
    # The subject of the ResearchElementDefinition is critical in interpreting the criteria definitions, as the logic in the ResearchElementDefinitions is evaluated with respect to a particular subject. This corresponds roughly to the notion of a Compartment in that it limits what content is available based on its relationship to the subject. In CQL, this corresponds to the context declaration.
    attr_accessor :subjectReference               # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Group)
    ##
    # Date last changed
    # The date  (and optionally time) when the research element definition was published. The date must change when the business version changes and it must change if the status code changes. In addition, it should change when the substantive content of the research element definition changes.
    # Note that this is not the same as the resource last-modified-date, since the resource may be a secondary representation of the research element definition. Additional specific dates may be added as extensions or be found by consulting Provenances associated with past versions of the resource.
    attr_accessor :date                           # 0-1 dateTime
    ##
    # Name of the publisher (organization or individual)
    # The name of the organization or individual that published the research element definition.
    # Usually an organization but may be an individual. The publisher (or steward) of the research element definition is the organization or individual primarily responsible for the maintenance and upkeep of the research element definition. This is not necessarily the same individual or organization that developed and initially authored the content. The publisher is the primary point of contact for questions or issues with the research element definition. This item SHOULD be populated unless the information is available from context.
    attr_accessor :publisher                      # 0-1 string
    ##
    # Contact details for the publisher
    # Contact details to assist a user in finding and communicating with the publisher.
    # May be a web site, an email address, a telephone number, etc.
    attr_accessor :contact                        # 0-* [ ContactDetail ]
    ##
    # Natural language description of the research element definition
    # A free text natural language description of the research element definition from a consumer's perspective.
    # This description can be used to capture details such as why the research element definition was built, comments about misuse, instructions for clinical use and interpretation, literature references, examples from the paper world, etc. It is not a rendering of the research element definition as conveyed in the 'text' field of the resource itself. This item SHOULD be populated unless the information is available from context (e.g. the language of the research element definition is presumed to be the predominant language in the place the research element definition was created).
    attr_accessor :description                    # 0-1 markdown
    ##
    # Used for footnotes or explanatory notes
    # A human-readable string to clarify or explain concepts about the resource.
    attr_accessor :comment                        # 0-* [ string ]
    ##
    # The context that the content is intended to support
    # The content was developed with a focus and intent of supporting the contexts that are listed. These contexts may be general categories (gender, age, ...) or may be references to specific programs (insurance plans, studies, ...) and may be used to assist with indexing and searching for appropriate research element definition instances.
    # When multiple useContexts are specified, there is no expectation that all or any of the contexts apply.
    attr_accessor :useContext                     # 0-* [ UsageContext ]
    ##
    # Intended jurisdiction for research element definition (if applicable)
    # A legal or geographic region in which the research element definition is intended to be used.
    # It may be possible for the research element definition to be used in jurisdictions other than those for which it was originally designed or intended.
    attr_accessor :jurisdiction                   # 0-* [ CodeableConcept ]
    ##
    # Why this research element definition is defined
    # Explanation of why this research element definition is needed and why it has been designed as it has.
    # This element does not describe the usage of the research element definition. Instead, it provides traceability of ''why'' the resource is either needed or ''why'' it is defined as it is.  This may be used to point to source materials or specifications that drove the structure of this research element definition.
    attr_accessor :purpose                        # 0-1 markdown
    ##
    # Describes the clinical usage of the ResearchElementDefinition
    # A detailed description, from a clinical perspective, of how the ResearchElementDefinition is used.
    attr_accessor :usage                          # 0-1 string
    ##
    # Use and/or publishing restrictions
    # A copyright statement relating to the research element definition and/or its contents. Copyright statements are generally legal restrictions on the use and publishing of the research element definition.
    attr_accessor :copyright                      # 0-1 markdown
    ##
    # When the research element definition was approved by publisher
    # The date on which the resource content was approved by the publisher. Approval happens once when the content is officially approved for usage.
    # The 'date' element may be more recent than the approval date because of minor changes or editorial corrections.
    attr_accessor :approvalDate                   # 0-1 date
    ##
    # When the research element definition was last reviewed
    # The date on which the resource content was last reviewed. Review happens periodically after approval but does not change the original approval date.
    # If specified, this date follows the original approval date.
    attr_accessor :lastReviewDate                 # 0-1 date
    ##
    # When the research element definition is expected to be used
    # The period during which the research element definition content was or is planned to be in active use.
    # The effective period for a research element definition  determines when the content is applicable for usage and is independent of publication and review dates. For example, a measure intended to be used for the year 2016 might be published in 2015.
    attr_accessor :effectivePeriod                # 0-1 Period
    ##
    # The category of the ResearchElementDefinition, such as Education, Treatment, Assessment, etc.
    # Descriptive topics related to the content of the ResearchElementDefinition. Topics provide a high-level categorization grouping types of ResearchElementDefinitions that can be useful for filtering and searching.
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
    # Logic used by the ResearchElementDefinition
    # A reference to a Library resource containing the formal logic used by the ResearchElementDefinition.
    attr_accessor :library                        # 0-* [ canonical(http://hl7.org/fhir/StructureDefinition/Library) ]
    ##
    # population | exposure | outcome
    # The type of research element, a population, an exposure, or an outcome.
    attr_accessor :type                           # 1-1 code
    ##
    # dichotomous | continuous | descriptive
    # The type of the outcome (e.g. Dichotomous, Continuous, or Descriptive).
    attr_accessor :variableType                   # 0-1 code
    ##
    # What defines the members of the research element
    # A characteristic that defines the members of the research element. Multiple characteristics are applied with "and" semantics.
    # Characteristics can be defined flexibly to accommodate different use cases for membership criteria, ranging from simple codes, all the way to using an expression language to express the criteria.
    attr_accessor :characteristic                 # 1-* [ ResearchElementDefinition::Characteristic ]

    def resourceType
      'ResearchElementDefinition'
    end
  end
end
