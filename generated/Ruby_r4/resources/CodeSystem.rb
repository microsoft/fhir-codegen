module FHIR

  ##
  # The CodeSystem resource is used to declare the existence of and describe a code system or code system supplement and its key properties, and optionally define a part or all of its content.
  class CodeSystem < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['code', 'content-mode', 'context-quantity', 'context-type-quantity', 'context-type-value', 'context-type', 'context', 'date', 'description', 'identifier', 'jurisdiction', 'language', 'name', 'publisher', 'status', 'supplements', 'system', 'title', 'url', 'version']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'CodeSystem.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'CodeSystem.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'CodeSystem.implicitRules',
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
        'path'=>'CodeSystem.language',
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
        'path'=>'CodeSystem.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'CodeSystem.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'CodeSystem.extension',
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
        'path'=>'CodeSystem.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Canonical identifier for this code system, represented as a URI (globally unique) (Coding.system)
      # An absolute URI that is used to identify this code system when it is referenced in a specification, model, design or an instance; also called its canonical identifier. This SHOULD be globally unique and SHOULD be a literal address at which at which an authoritative instance of this code system is (or will be) published. This URL can be the target of a canonical reference. It SHALL remain the same when the code system is stored on different servers. This is used in [Coding](datatypes.html#Coding).system.
      # Can be a urn:uuid: or a urn:oid: but real http: addresses are preferred.  Multiple instances may share the same URL if they have a distinct version.
      # 
      # The determination of when to create a new version of a resource (same url, new version) vs. defining a new artifact is up to the author.  Considerations for making this decision are found in [Technical and Business Versions](resource.html#versions). 
      # 
      # In some cases, the resource can no longer be found at the stated url, but the url itself cannot change. Implementations can use the [meta.source](resource.html#meta) element to indicate where the current master source of the resource can be found.
      'url' => {
        'type'=>'uri',
        'path'=>'CodeSystem.url',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional identifier for the code system (business identifier)
      # A formal identifier that is used to identify this code system when it is represented in other formats, or referenced in a specification, model, design or an instance.
      # Typically, this is used for identifiers that can go in an HL7 V3 II (instance identifier) data type, and can then identify this code system outside of FHIR, where it is not possible to use the logical URI.  Note that HL7 defines at least three identifiers for many of its code systems - the FHIR canonical URL, the OID and the V2 Table 0396 mnemonic code.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'CodeSystem.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Business version of the code system (Coding.version)
      # The identifier that is used to identify this version of the code system when it is referenced in a specification, model, design or instance. This is an arbitrary value managed by the code system author and is not expected to be globally unique. For example, it might be a timestamp (e.g. yyyymmdd) if a managed version is not available. There is also no expectation that versions can be placed in a lexicographical sequence. This is used in [Coding](datatypes.html#Coding).version.
      # There may be different code system instances that have the same identifier but different versions.  The version can be appended to the url in a reference to allow a reference to a particular business version of the code system with the format [url]|[version].
      'version' => {
        'type'=>'string',
        'path'=>'CodeSystem.version',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name for this code system (computer friendly)
      # A natural language name identifying the code system. This name should be usable as an identifier for the module by machine processing applications such as code generation.
      # The name is not expected to be globally unique. The name should be a simple alphanumeric type name to ensure that it is machine-processing friendly.
      'name' => {
        'type'=>'string',
        'path'=>'CodeSystem.name',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name for this code system (human friendly)
      # A short, descriptive, user-friendly title for the code system.
      # This name does not need to be machine-processing friendly and may contain punctuation, white-space, etc.
      'title' => {
        'type'=>'string',
        'path'=>'CodeSystem.title',
        'min'=>0,
        'max'=>1
      },
      ##
      # draft | active | retired | unknown
      # The date (and optionally time) when the code system resource was created or revised.
      # Allows filtering of code systems that are appropriate for use versus not.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/publication-status'=>[ 'draft', 'active', 'retired', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'CodeSystem.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/publication-status'}
      },
      ##
      # For testing purposes, not real usage
      # A Boolean value to indicate that this code system is authored for testing purposes (or education/evaluation/marketing) and is not intended to be used for genuine usage.
      # Allows filtering of code systems that are appropriate for use versus not.
      'experimental' => {
        'type'=>'boolean',
        'path'=>'CodeSystem.experimental',
        'min'=>0,
        'max'=>1
      },
      ##
      # Date last changed
      # The date  (and optionally time) when the code system was published. The date must change when the business version changes and it must change if the status code changes. In addition, it should change when the substantive content of the code system changes.
      # Note that this is not the same as the resource last-modified-date, since the resource may be a secondary representation of the code system. Additional specific dates may be added as extensions or be found by consulting Provenances associated with past versions of the resource.
      'date' => {
        'type'=>'dateTime',
        'path'=>'CodeSystem.date',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name of the publisher (organization or individual)
      # The name of the organization or individual that published the code system.
      # Usually an organization but may be an individual. The publisher (or steward) of the code system is the organization or individual primarily responsible for the maintenance and upkeep of the code system. This is not necessarily the same individual or organization that developed and initially authored the content. The publisher is the primary point of contact for questions or issues with the code system. This item SHOULD be populated unless the information is available from context.
      'publisher' => {
        'type'=>'string',
        'path'=>'CodeSystem.publisher',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contact details for the publisher
      # Contact details to assist a user in finding and communicating with the publisher.
      # May be a web site, an email address, a telephone number, etc.
      'contact' => {
        'type'=>'ContactDetail',
        'path'=>'CodeSystem.contact',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Natural language description of the code system
      # A free text natural language description of the code system from a consumer's perspective.
      # This description can be used to capture details such as why the code system was built, comments about misuse, instructions for clinical use and interpretation, literature references, examples from the paper world, etc. It is not a rendering of the code system as conveyed in the 'text' field of the resource itself. This item SHOULD be populated unless the information is available from context (e.g. the language of the code system is presumed to be the predominant language in the place the code system was created).
      'description' => {
        'type'=>'markdown',
        'path'=>'CodeSystem.description',
        'min'=>0,
        'max'=>1
      },
      ##
      # The context that the content is intended to support
      # The content was developed with a focus and intent of supporting the contexts that are listed. These contexts may be general categories (gender, age, ...) or may be references to specific programs (insurance plans, studies, ...) and may be used to assist with indexing and searching for appropriate code system instances.
      # When multiple useContexts are specified, there is no expectation that all or any of the contexts apply.
      'useContext' => {
        'type'=>'UsageContext',
        'path'=>'CodeSystem.useContext',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Intended jurisdiction for code system (if applicable)
      # A legal or geographic region in which the code system is intended to be used.
      # It may be possible for the code system to be used in jurisdictions other than those for which it was originally designed or intended.
      'jurisdiction' => {
        'type'=>'CodeableConcept',
        'path'=>'CodeSystem.jurisdiction',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Why this code system is defined
      # Explanation of why this code system is needed and why it has been designed as it has.
      # This element does not describe the usage of the code system. Instead, it provides traceability of ''why'' the resource is either needed or ''why'' it is defined as it is.  This may be used to point to source materials or specifications that drove the structure of this code system.
      'purpose' => {
        'type'=>'markdown',
        'path'=>'CodeSystem.purpose',
        'min'=>0,
        'max'=>1
      },
      ##
      # Use and/or publishing restrictions
      # A copyright statement relating to the code system and/or its contents. Copyright statements are generally legal restrictions on the use and publishing of the code system.
      # ... Sometimes, the copyright differs between the code system and the codes that are included. The copyright statement should clearly differentiate between these when required.
      'copyright' => {
        'type'=>'markdown',
        'path'=>'CodeSystem.copyright',
        'min'=>0,
        'max'=>1
      },
      ##
      # If code comparison is case sensitive when codes within this system are compared to each other.
      # If this value is missing, then it is not specified whether a code system is case sensitive or not. When the rule is not known, Postel's law should be followed: produce codes with the correct case, and accept codes in any case. This element is primarily provided to support validation software.
      'caseSensitive' => {
        'type'=>'boolean',
        'path'=>'CodeSystem.caseSensitive',
        'min'=>0,
        'max'=>1
      },
      ##
      # Canonical reference to the value set with entire code system
      # Canonical reference to the value set that contains the entire code system.
      # The definition of the value set SHALL include all codes from this code system and only codes from this code system, and it SHALL be immutable.
      'valueSet' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ValueSet'],
        'type'=>'canonical',
        'path'=>'CodeSystem.valueSet',
        'min'=>0,
        'max'=>1
      },
      ##
      # grouped-by | is-a | part-of | classified-with
      # The meaning of the hierarchy of concepts as represented in this resource.
      # Note that other representations might have a different hierarchy or none at all, and represent the information using properties.
      'hierarchyMeaning' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/codesystem-hierarchy-meaning'=>[ 'grouped-by', 'is-a', 'part-of', 'classified-with' ]
        },
        'type'=>'code',
        'path'=>'CodeSystem.hierarchyMeaning',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/codesystem-hierarchy-meaning'}
      },
      ##
      # If code system defines a compositional grammar
      # The code system defines a compositional (post-coordination) grammar.
      # Note that the code system resource does not define what the compositional grammar is, only whether or not there is one.
      'compositional' => {
        'type'=>'boolean',
        'path'=>'CodeSystem.compositional',
        'min'=>0,
        'max'=>1
      },
      ##
      # If definitions are not stable
      # This flag is used to signify that the code system does not commit to concept permanence across versions. If true, a version must be specified when referencing this code system.
      # Best practice is that code systems do not redefine concepts, or that if concepts are redefined, a new code system definition is created. But this is not always possible, so some code systems may be defined as 'versionNeeded'.
      # 
      # Most code systems occasionally refine the displays defined for concepts between versions. Contexts in which the concept display values are validated may require that the version be specified for some code systems irrespective of the value of this property.
      'versionNeeded' => {
        'type'=>'boolean',
        'path'=>'CodeSystem.versionNeeded',
        'min'=>0,
        'max'=>1
      },
      ##
      # not-present | example | fragment | complete | supplement
      # The extent of the content of the code system (the concepts and codes it defines) are represented in this resource instance.
      'content' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/codesystem-content-mode'=>[ 'not-present', 'example', 'fragment', 'complete', 'supplement' ]
        },
        'type'=>'code',
        'path'=>'CodeSystem.content',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/codesystem-content-mode'}
      },
      ##
      # Canonical URL of Code System this adds designations and properties to
      # The canonical URL of the code system that this code system supplement is adding designations and properties to.
      # The most common use of a code system supplement is to add additional language support.
      'supplements' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/CodeSystem'],
        'type'=>'canonical',
        'path'=>'CodeSystem.supplements',
        'min'=>0,
        'max'=>1
      },
      ##
      # Total concepts in the code system
      # The total number of concepts defined by the code system. Where the code system has a compositional grammar, the basis of this count is defined by the system steward.
      # The count of concepts defined in this resource cannot be more than this value but may be less for several reasons - see the content element.
      'count' => {
        'type'=>'unsignedInt',
        'path'=>'CodeSystem.count',
        'min'=>0,
        'max'=>1
      },
      ##
      # Filter that can be used in a value set
      # A filter that can be used in a value set compose statement when selecting concepts using a filter.
      # Note that filters defined in code systems usually require custom code on the part of any terminology engine that will make them available for use in value set filters. For this reason, they are generally only seen in high value published terminologies.
      'filter' => {
        'type'=>'CodeSystem::Filter',
        'path'=>'CodeSystem.filter',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional information supplied about each concept
      # A property defines an additional slot through which additional information can be provided about a concept.
      'property' => {
        'type'=>'CodeSystem::Property',
        'path'=>'CodeSystem.property',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Concepts in the code system
      # Concepts that are in the code system. The concept definitions are inherently hierarchical, but the definitions must be consulted to determine what the meanings of the hierarchical relationships are.
      # If this is empty, it means that the code system resource does not represent the content of the code system.
      'concept' => {
        'type'=>'CodeSystem::Concept',
        'path'=>'CodeSystem.concept',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Filter that can be used in a value set
    # A filter that can be used in a value set compose statement when selecting concepts using a filter.
    # Note that filters defined in code systems usually require custom code on the part of any terminology engine that will make them available for use in value set filters. For this reason, they are generally only seen in high value published terminologies.
    class Filter < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Filter.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Filter.extension',
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
          'path'=>'Filter.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Code that identifies the filter
        # The code that identifies this filter when it is used as a filter in [ValueSet](valueset.html#).compose.include.filter.
        'code' => {
          'type'=>'code',
          'path'=>'Filter.code',
          'min'=>1,
          'max'=>1
        },
        ##
        # How or why the filter is used
        # A description of how or why the filter is used.
        'description' => {
          'type'=>'string',
          'path'=>'Filter.description',
          'min'=>0,
          'max'=>1
        },
        ##
        # = | is-a | descendent-of | is-not-a | regex | in | not-in | generalizes | exists
        # A list of operators that can be used with the filter.
        'operator' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/filter-operator'=>[ '=', 'is-a', 'descendent-of', 'is-not-a', 'regex', 'in', 'not-in', 'generalizes', 'exists' ]
          },
          'type'=>'code',
          'path'=>'Filter.operator',
          'min'=>1,
          'max'=>Float::INFINITY,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/filter-operator'}
        },
        ##
        # What to use for the value
        # A description of what the value for the filter should be.
        'value' => {
          'type'=>'string',
          'path'=>'Filter.value',
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
      # Code that identifies the filter
      # The code that identifies this filter when it is used as a filter in [ValueSet](valueset.html#).compose.include.filter.
      attr_accessor :code                           # 1-1 code
      ##
      # How or why the filter is used
      # A description of how or why the filter is used.
      attr_accessor :description                    # 0-1 string
      ##
      # = | is-a | descendent-of | is-not-a | regex | in | not-in | generalizes | exists
      # A list of operators that can be used with the filter.
      attr_accessor :operator                       # 1-* [ code ]
      ##
      # What to use for the value
      # A description of what the value for the filter should be.
      attr_accessor :value                          # 1-1 string
    end

    ##
    # Additional information supplied about each concept
    # A property defines an additional slot through which additional information can be provided about a concept.
    class Property < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Property.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Property.extension',
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
          'path'=>'Property.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Identifies the property on the concepts, and when referred to in operations
        # A code that is used to identify the property. The code is used internally (in CodeSystem.concept.property.code) and also externally, such as in property filters.
        'code' => {
          'type'=>'code',
          'path'=>'Property.code',
          'min'=>1,
          'max'=>1
        },
        ##
        # Formal identifier for the property
        # Reference to the formal meaning of the property. One possible source of meaning is the [Concept Properties](codesystem-concept-properties.html) code system.
        'uri' => {
          'type'=>'uri',
          'path'=>'Property.uri',
          'min'=>0,
          'max'=>1
        },
        ##
        # Why the property is defined, and/or what it conveys
        # A description of the property- why it is defined, and how its value might be used.
        'description' => {
          'type'=>'string',
          'path'=>'Property.description',
          'min'=>0,
          'max'=>1
        },
        ##
        # code | Coding | string | integer | boolean | dateTime | decimal
        # The type of the property value. Properties of type "code" contain a code defined by the code system (e.g. a reference to another defined concept).
        'type' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/concept-property-type'=>[ 'code', 'Coding', 'string', 'integer', 'boolean', 'dateTime', 'decimal' ]
          },
          'type'=>'code',
          'path'=>'Property.type',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/concept-property-type'}
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
      # Identifies the property on the concepts, and when referred to in operations
      # A code that is used to identify the property. The code is used internally (in CodeSystem.concept.property.code) and also externally, such as in property filters.
      attr_accessor :code                           # 1-1 code
      ##
      # Formal identifier for the property
      # Reference to the formal meaning of the property. One possible source of meaning is the [Concept Properties](codesystem-concept-properties.html) code system.
      attr_accessor :uri                            # 0-1 uri
      ##
      # Why the property is defined, and/or what it conveys
      # A description of the property- why it is defined, and how its value might be used.
      attr_accessor :description                    # 0-1 string
      ##
      # code | Coding | string | integer | boolean | dateTime | decimal
      # The type of the property value. Properties of type "code" contain a code defined by the code system (e.g. a reference to another defined concept).
      attr_accessor :type                           # 1-1 code
    end

    ##
    # Concepts in the code system
    # Concepts that are in the code system. The concept definitions are inherently hierarchical, but the definitions must be consulted to determine what the meanings of the hierarchical relationships are.
    # If this is empty, it means that the code system resource does not represent the content of the code system.
    class Concept < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Concept.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Concept.extension',
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
          'path'=>'Concept.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Code that identifies concept
        # A code - a text symbol - that uniquely identifies the concept within the code system.
        'code' => {
          'type'=>'code',
          'path'=>'Concept.code',
          'min'=>1,
          'max'=>1
        },
        ##
        # Text to display to the user
        # A human readable string that is the recommended default way to present this concept to a user.
        'display' => {
          'type'=>'string',
          'path'=>'Concept.display',
          'min'=>0,
          'max'=>1
        },
        ##
        # Formal definition
        # The formal definition of the concept. The code system resource does not make formal definitions required, because of the prevalence of legacy systems. However, they are highly recommended, as without them there is no formal meaning associated with the concept.
        'definition' => {
          'type'=>'string',
          'path'=>'Concept.definition',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional representations for the concept - other languages, aliases, specialized purposes, used for particular purposes, etc.
        # Concepts have both a ```display``` and an array of ```designation```. The display is equivalent to a special designation with an implied ```designation.use``` of "primary code" and a language equal to the [Resource Language](resource.html#language).
        'designation' => {
          'type'=>'CodeSystem::Concept::Designation',
          'path'=>'Concept.designation',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Property value for the concept
        # A property value for this concept.
        'property' => {
          'type'=>'CodeSystem::Concept::Property',
          'path'=>'Concept.property',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Child Concepts (is-a/contains/categorizes)
        # Defines children of a concept to produce a hierarchy of concepts. The nature of the relationships is variable (is-a/contains/categorizes) - see hierarchyMeaning.
        'concept' => {
          'type'=>'CodeSystem::Concept',
          'path'=>'Concept.concept',
          'min'=>0,
          'max'=>Float::INFINITY
        }
      }

      ##
      # Additional representations for the concept - other languages, aliases, specialized purposes, used for particular purposes, etc.
      # Concepts have both a ```display``` and an array of ```designation```. The display is equivalent to a special designation with an implied ```designation.use``` of "primary code" and a language equal to the [Resource Language](resource.html#language).
      class Designation < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
            'path'=>'Designation.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Designation.extension',
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
            'path'=>'Designation.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Human language of the designation
          # The language this designation is defined for.
          # In the absence of a language, the resource language applies.
          'language' => {
            'valid_codes'=>{
              'urn:ietf:bcp:47'=>[ 'ar', 'bn', 'cs', 'da', 'de', 'de-AT', 'de-CH', 'de-DE', 'el', 'en', 'en-AU', 'en-CA', 'en-GB', 'en-IN', 'en-NZ', 'en-SG', 'en-US', 'es', 'es-AR', 'es-ES', 'es-UY', 'fi', 'fr', 'fr-BE', 'fr-CH', 'fr-FR', 'fy', 'fy-NL', 'hi', 'hr', 'it', 'it-CH', 'it-IT', 'ja', 'ko', 'nl', 'nl-BE', 'nl-NL', 'no', 'no-NO', 'pa', 'pl', 'pt', 'pt-BR', 'ru', 'ru-RU', 'sr', 'sr-RS', 'sv', 'sv-SE', 'te', 'zh', 'zh-CN', 'zh-HK', 'zh-SG', 'zh-TW' ]
            },
            'type'=>'code',
            'path'=>'Designation.language',
            'min'=>0,
            'max'=>1,
            'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/languages'}
          },
          ##
          # Details how this designation would be used
          # A code that details how this designation would be used.
          # If no use is provided, the designation can be assumed to be suitable for general display to a human user.
          'use' => {
            'type'=>'Coding',
            'path'=>'Designation.use',
            'min'=>0,
            'max'=>1
          },
          ##
          # The text value for this designation.
          'value' => {
            'type'=>'string',
            'path'=>'Designation.value',
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
        # Human language of the designation
        # The language this designation is defined for.
        # In the absence of a language, the resource language applies.
        attr_accessor :language                       # 0-1 code
        ##
        # Details how this designation would be used
        # A code that details how this designation would be used.
        # If no use is provided, the designation can be assumed to be suitable for general display to a human user.
        attr_accessor :use                            # 0-1 Coding
        ##
        # The text value for this designation.
        attr_accessor :value                          # 1-1 string
      end

      ##
      # Property value for the concept
      # A property value for this concept.
      class Property < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        MULTIPLE_TYPES = {
          'value[x]' => ['boolean', 'code', 'Coding', 'dateTime', 'decimal', 'integer', 'string']
        }
        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
            'path'=>'Property.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Property.extension',
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
            'path'=>'Property.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Reference to CodeSystem.property.code
          # A code that is a reference to CodeSystem.property.code.
          'code' => {
            'type'=>'code',
            'path'=>'Property.code',
            'min'=>1,
            'max'=>1
          },
          ##
          # Value of the property for this concept
          # The value of this property.
          'valueBoolean' => {
            'type'=>'Boolean',
            'path'=>'Property.value[x]',
            'min'=>1,
            'max'=>1
          }
          ##
          # Value of the property for this concept
          # The value of this property.
          'valueCode' => {
            'type'=>'Code',
            'path'=>'Property.value[x]',
            'min'=>1,
            'max'=>1
          }
          ##
          # Value of the property for this concept
          # The value of this property.
          'valueCoding' => {
            'type'=>'Coding',
            'path'=>'Property.value[x]',
            'min'=>1,
            'max'=>1
          }
          ##
          # Value of the property for this concept
          # The value of this property.
          'valueDateTime' => {
            'type'=>'DateTime',
            'path'=>'Property.value[x]',
            'min'=>1,
            'max'=>1
          }
          ##
          # Value of the property for this concept
          # The value of this property.
          'valueDecimal' => {
            'type'=>'Decimal',
            'path'=>'Property.value[x]',
            'min'=>1,
            'max'=>1
          }
          ##
          # Value of the property for this concept
          # The value of this property.
          'valueInteger' => {
            'type'=>'Integer',
            'path'=>'Property.value[x]',
            'min'=>1,
            'max'=>1
          }
          ##
          # Value of the property for this concept
          # The value of this property.
          'valueString' => {
            'type'=>'String',
            'path'=>'Property.value[x]',
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
        # Reference to CodeSystem.property.code
        # A code that is a reference to CodeSystem.property.code.
        attr_accessor :code                           # 1-1 code
        ##
        # Value of the property for this concept
        # The value of this property.
        attr_accessor :valueBoolean                   # 1-1 Boolean
        ##
        # Value of the property for this concept
        # The value of this property.
        attr_accessor :valueCode                      # 1-1 Code
        ##
        # Value of the property for this concept
        # The value of this property.
        attr_accessor :valueCoding                    # 1-1 Coding
        ##
        # Value of the property for this concept
        # The value of this property.
        attr_accessor :valueDateTime                  # 1-1 DateTime
        ##
        # Value of the property for this concept
        # The value of this property.
        attr_accessor :valueDecimal                   # 1-1 Decimal
        ##
        # Value of the property for this concept
        # The value of this property.
        attr_accessor :valueInteger                   # 1-1 Integer
        ##
        # Value of the property for this concept
        # The value of this property.
        attr_accessor :valueString                    # 1-1 String
      end
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
      # Code that identifies concept
      # A code - a text symbol - that uniquely identifies the concept within the code system.
      attr_accessor :code                           # 1-1 code
      ##
      # Text to display to the user
      # A human readable string that is the recommended default way to present this concept to a user.
      attr_accessor :display                        # 0-1 string
      ##
      # Formal definition
      # The formal definition of the concept. The code system resource does not make formal definitions required, because of the prevalence of legacy systems. However, they are highly recommended, as without them there is no formal meaning associated with the concept.
      attr_accessor :definition                     # 0-1 string
      ##
      # Additional representations for the concept - other languages, aliases, specialized purposes, used for particular purposes, etc.
      # Concepts have both a ```display``` and an array of ```designation```. The display is equivalent to a special designation with an implied ```designation.use``` of "primary code" and a language equal to the [Resource Language](resource.html#language).
      attr_accessor :designation                    # 0-* [ CodeSystem::Concept::Designation ]
      ##
      # Property value for the concept
      # A property value for this concept.
      attr_accessor :property                       # 0-* [ CodeSystem::Concept::Property ]
      ##
      # Child Concepts (is-a/contains/categorizes)
      # Defines children of a concept to produce a hierarchy of concepts. The nature of the relationships is variable (is-a/contains/categorizes) - see hierarchyMeaning.
      attr_accessor :concept                        # 0-* [ CodeSystem::Concept ]
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
    # Canonical identifier for this code system, represented as a URI (globally unique) (Coding.system)
    # An absolute URI that is used to identify this code system when it is referenced in a specification, model, design or an instance; also called its canonical identifier. This SHOULD be globally unique and SHOULD be a literal address at which at which an authoritative instance of this code system is (or will be) published. This URL can be the target of a canonical reference. It SHALL remain the same when the code system is stored on different servers. This is used in [Coding](datatypes.html#Coding).system.
    # Can be a urn:uuid: or a urn:oid: but real http: addresses are preferred.  Multiple instances may share the same URL if they have a distinct version.
    # 
    # The determination of when to create a new version of a resource (same url, new version) vs. defining a new artifact is up to the author.  Considerations for making this decision are found in [Technical and Business Versions](resource.html#versions). 
    # 
    # In some cases, the resource can no longer be found at the stated url, but the url itself cannot change. Implementations can use the [meta.source](resource.html#meta) element to indicate where the current master source of the resource can be found.
    attr_accessor :url                            # 0-1 uri
    ##
    # Additional identifier for the code system (business identifier)
    # A formal identifier that is used to identify this code system when it is represented in other formats, or referenced in a specification, model, design or an instance.
    # Typically, this is used for identifiers that can go in an HL7 V3 II (instance identifier) data type, and can then identify this code system outside of FHIR, where it is not possible to use the logical URI.  Note that HL7 defines at least three identifiers for many of its code systems - the FHIR canonical URL, the OID and the V2 Table 0396 mnemonic code.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Business version of the code system (Coding.version)
    # The identifier that is used to identify this version of the code system when it is referenced in a specification, model, design or instance. This is an arbitrary value managed by the code system author and is not expected to be globally unique. For example, it might be a timestamp (e.g. yyyymmdd) if a managed version is not available. There is also no expectation that versions can be placed in a lexicographical sequence. This is used in [Coding](datatypes.html#Coding).version.
    # There may be different code system instances that have the same identifier but different versions.  The version can be appended to the url in a reference to allow a reference to a particular business version of the code system with the format [url]|[version].
    attr_accessor :version                        # 0-1 string
    ##
    # Name for this code system (computer friendly)
    # A natural language name identifying the code system. This name should be usable as an identifier for the module by machine processing applications such as code generation.
    # The name is not expected to be globally unique. The name should be a simple alphanumeric type name to ensure that it is machine-processing friendly.
    attr_accessor :name                           # 0-1 string
    ##
    # Name for this code system (human friendly)
    # A short, descriptive, user-friendly title for the code system.
    # This name does not need to be machine-processing friendly and may contain punctuation, white-space, etc.
    attr_accessor :title                          # 0-1 string
    ##
    # draft | active | retired | unknown
    # The date (and optionally time) when the code system resource was created or revised.
    # Allows filtering of code systems that are appropriate for use versus not.
    attr_accessor :status                         # 1-1 code
    ##
    # For testing purposes, not real usage
    # A Boolean value to indicate that this code system is authored for testing purposes (or education/evaluation/marketing) and is not intended to be used for genuine usage.
    # Allows filtering of code systems that are appropriate for use versus not.
    attr_accessor :experimental                   # 0-1 boolean
    ##
    # Date last changed
    # The date  (and optionally time) when the code system was published. The date must change when the business version changes and it must change if the status code changes. In addition, it should change when the substantive content of the code system changes.
    # Note that this is not the same as the resource last-modified-date, since the resource may be a secondary representation of the code system. Additional specific dates may be added as extensions or be found by consulting Provenances associated with past versions of the resource.
    attr_accessor :date                           # 0-1 dateTime
    ##
    # Name of the publisher (organization or individual)
    # The name of the organization or individual that published the code system.
    # Usually an organization but may be an individual. The publisher (or steward) of the code system is the organization or individual primarily responsible for the maintenance and upkeep of the code system. This is not necessarily the same individual or organization that developed and initially authored the content. The publisher is the primary point of contact for questions or issues with the code system. This item SHOULD be populated unless the information is available from context.
    attr_accessor :publisher                      # 0-1 string
    ##
    # Contact details for the publisher
    # Contact details to assist a user in finding and communicating with the publisher.
    # May be a web site, an email address, a telephone number, etc.
    attr_accessor :contact                        # 0-* [ ContactDetail ]
    ##
    # Natural language description of the code system
    # A free text natural language description of the code system from a consumer's perspective.
    # This description can be used to capture details such as why the code system was built, comments about misuse, instructions for clinical use and interpretation, literature references, examples from the paper world, etc. It is not a rendering of the code system as conveyed in the 'text' field of the resource itself. This item SHOULD be populated unless the information is available from context (e.g. the language of the code system is presumed to be the predominant language in the place the code system was created).
    attr_accessor :description                    # 0-1 markdown
    ##
    # The context that the content is intended to support
    # The content was developed with a focus and intent of supporting the contexts that are listed. These contexts may be general categories (gender, age, ...) or may be references to specific programs (insurance plans, studies, ...) and may be used to assist with indexing and searching for appropriate code system instances.
    # When multiple useContexts are specified, there is no expectation that all or any of the contexts apply.
    attr_accessor :useContext                     # 0-* [ UsageContext ]
    ##
    # Intended jurisdiction for code system (if applicable)
    # A legal or geographic region in which the code system is intended to be used.
    # It may be possible for the code system to be used in jurisdictions other than those for which it was originally designed or intended.
    attr_accessor :jurisdiction                   # 0-* [ CodeableConcept ]
    ##
    # Why this code system is defined
    # Explanation of why this code system is needed and why it has been designed as it has.
    # This element does not describe the usage of the code system. Instead, it provides traceability of ''why'' the resource is either needed or ''why'' it is defined as it is.  This may be used to point to source materials or specifications that drove the structure of this code system.
    attr_accessor :purpose                        # 0-1 markdown
    ##
    # Use and/or publishing restrictions
    # A copyright statement relating to the code system and/or its contents. Copyright statements are generally legal restrictions on the use and publishing of the code system.
    # ... Sometimes, the copyright differs between the code system and the codes that are included. The copyright statement should clearly differentiate between these when required.
    attr_accessor :copyright                      # 0-1 markdown
    ##
    # If code comparison is case sensitive when codes within this system are compared to each other.
    # If this value is missing, then it is not specified whether a code system is case sensitive or not. When the rule is not known, Postel's law should be followed: produce codes with the correct case, and accept codes in any case. This element is primarily provided to support validation software.
    attr_accessor :caseSensitive                  # 0-1 boolean
    ##
    # Canonical reference to the value set with entire code system
    # Canonical reference to the value set that contains the entire code system.
    # The definition of the value set SHALL include all codes from this code system and only codes from this code system, and it SHALL be immutable.
    attr_accessor :valueSet                       # 0-1 canonical(http://hl7.org/fhir/StructureDefinition/ValueSet)
    ##
    # grouped-by | is-a | part-of | classified-with
    # The meaning of the hierarchy of concepts as represented in this resource.
    # Note that other representations might have a different hierarchy or none at all, and represent the information using properties.
    attr_accessor :hierarchyMeaning               # 0-1 code
    ##
    # If code system defines a compositional grammar
    # The code system defines a compositional (post-coordination) grammar.
    # Note that the code system resource does not define what the compositional grammar is, only whether or not there is one.
    attr_accessor :compositional                  # 0-1 boolean
    ##
    # If definitions are not stable
    # This flag is used to signify that the code system does not commit to concept permanence across versions. If true, a version must be specified when referencing this code system.
    # Best practice is that code systems do not redefine concepts, or that if concepts are redefined, a new code system definition is created. But this is not always possible, so some code systems may be defined as 'versionNeeded'.
    # 
    # Most code systems occasionally refine the displays defined for concepts between versions. Contexts in which the concept display values are validated may require that the version be specified for some code systems irrespective of the value of this property.
    attr_accessor :versionNeeded                  # 0-1 boolean
    ##
    # not-present | example | fragment | complete | supplement
    # The extent of the content of the code system (the concepts and codes it defines) are represented in this resource instance.
    attr_accessor :content                        # 1-1 code
    ##
    # Canonical URL of Code System this adds designations and properties to
    # The canonical URL of the code system that this code system supplement is adding designations and properties to.
    # The most common use of a code system supplement is to add additional language support.
    attr_accessor :supplements                    # 0-1 canonical(http://hl7.org/fhir/StructureDefinition/CodeSystem)
    ##
    # Total concepts in the code system
    # The total number of concepts defined by the code system. Where the code system has a compositional grammar, the basis of this count is defined by the system steward.
    # The count of concepts defined in this resource cannot be more than this value but may be less for several reasons - see the content element.
    attr_accessor :count                          # 0-1 unsignedInt
    ##
    # Filter that can be used in a value set
    # A filter that can be used in a value set compose statement when selecting concepts using a filter.
    # Note that filters defined in code systems usually require custom code on the part of any terminology engine that will make them available for use in value set filters. For this reason, they are generally only seen in high value published terminologies.
    attr_accessor :filter                         # 0-* [ CodeSystem::Filter ]
    ##
    # Additional information supplied about each concept
    # A property defines an additional slot through which additional information can be provided about a concept.
    attr_accessor :property                       # 0-* [ CodeSystem::Property ]
    ##
    # Concepts in the code system
    # Concepts that are in the code system. The concept definitions are inherently hierarchical, but the definitions must be consulted to determine what the meanings of the hierarchical relationships are.
    # If this is empty, it means that the code system resource does not represent the content of the code system.
    attr_accessor :concept                        # 0-* [ CodeSystem::Concept ]

    def resourceType
      'CodeSystem'
    end
  end
end
