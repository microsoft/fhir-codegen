module FHIR

  ##
  # A curated namespace that issues unique symbols within that namespace for the identification of concepts, people, devices, etc.  Represents a "System" used within the Identifier and Coding data types.
  class NamingSystem < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['contact', 'context-quantity', 'context-type-quantity', 'context-type-value', 'context-type', 'context', 'date', 'description', 'id-type', 'jurisdiction', 'kind', 'name', 'period', 'publisher', 'responsible', 'status', 'telecom', 'type', 'value']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'NamingSystem.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'NamingSystem.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'NamingSystem.implicitRules',
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
        'path'=>'NamingSystem.language',
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
        'path'=>'NamingSystem.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'NamingSystem.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'NamingSystem.extension',
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
        'path'=>'NamingSystem.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Name for this naming system (computer friendly)
      # A natural language name identifying the naming system. This name should be usable as an identifier for the module by machine processing applications such as code generation.
      # The name is not expected to be globally unique. The name should be a simple alphanumeric type name to ensure that it is machine-processing friendly.The"symbolic name" for an OID would be captured as an extension.
      'name' => {
        'type'=>'string',
        'path'=>'NamingSystem.name',
        'min'=>1,
        'max'=>1
      },
      ##
      # draft | active | retired | unknown
      # The status of this naming system. Enables tracking the life-cycle of the content.
      # Allows filtering of naming systems that are appropriate for use versus not.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/publication-status'=>[ 'draft', 'active', 'retired', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'NamingSystem.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/publication-status'}
      },
      ##
      # codesystem | identifier | root
      # Indicates the purpose for the naming system - what kinds of things does it make unique?
      'kind' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/namingsystem-type'=>[ 'codesystem', 'identifier', 'root' ]
        },
        'type'=>'code',
        'path'=>'NamingSystem.kind',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/namingsystem-type'}
      },
      ##
      # Date last changed
      # The date  (and optionally time) when the naming system was published. The date must change when the business version changes and it must change if the status code changes. In addition, it should change when the substantive content of the naming system changes.
      # Note that this is not the same as the resource last-modified-date, since the resource may be a secondary representation of the naming system. Additional specific dates may be added as extensions or be found by consulting Provenances associated with past versions of the resource.
      'date' => {
        'type'=>'dateTime',
        'path'=>'NamingSystem.date',
        'min'=>1,
        'max'=>1
      },
      ##
      # Name of the publisher (organization or individual)
      # The name of the organization or individual that published the naming system.
      # Usually an organization but may be an individual. The publisher (or steward) of the naming system is the organization or individual primarily responsible for the maintenance and upkeep of the naming system. This is not necessarily the same individual or organization that developed and initially authored the content. The publisher is the primary point of contact for questions or issues with the naming system. This item SHOULD be populated unless the information is available from context.
      'publisher' => {
        'type'=>'string',
        'path'=>'NamingSystem.publisher',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contact details for the publisher
      # Contact details to assist a user in finding and communicating with the publisher.
      # May be a web site, an email address, a telephone number, etc.
      'contact' => {
        'type'=>'ContactDetail',
        'path'=>'NamingSystem.contact',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Who maintains system namespace?
      # The name of the organization that is responsible for issuing identifiers or codes for this namespace and ensuring their non-collision.
      # This is the primary organization.  Responsibility for some aspects of a namespace may be delegated.
      'responsible' => {
        'type'=>'string',
        'path'=>'NamingSystem.responsible',
        'min'=>0,
        'max'=>1
      },
      ##
      # e.g. driver,  provider,  patient, bank etc.
      # Categorizes a naming system for easier search by grouping related naming systems.
      # This will most commonly be used for identifier namespaces, but categories could potentially be useful for code systems and authorities as well.
      'type' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/v2-0203'=>[ 'DL', 'PPN', 'BRN', 'MR', 'MCN', 'EN', 'TAX', 'NIIP', 'PRN', 'MD', 'DR', 'ACSN', 'UDI', 'SNO', 'SB', 'PLAC', 'FILL', 'JHN' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'NamingSystem.type',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/identifier-type'}
      },
      ##
      # Natural language description of the naming system
      # A free text natural language description of the naming system from a consumer's perspective. Details about what the namespace identifies including scope, granularity, version labeling, etc.
      # This description can be used to capture details such as why the naming system was built, comments about misuse, instructions for clinical use and interpretation, literature references, examples from the paper world, etc. It is not a rendering of the naming system as conveyed in the 'text' field of the resource itself. This item SHOULD be populated unless the information is available from context (e.g. the language of the naming system is presumed to be the predominant language in the place the naming system was created).
      'description' => {
        'type'=>'markdown',
        'path'=>'NamingSystem.description',
        'min'=>0,
        'max'=>1
      },
      ##
      # The context that the content is intended to support
      # The content was developed with a focus and intent of supporting the contexts that are listed. These contexts may be general categories (gender, age, ...) or may be references to specific programs (insurance plans, studies, ...) and may be used to assist with indexing and searching for appropriate naming system instances.
      # When multiple useContexts are specified, there is no expectation that all or any of the contexts apply.
      'useContext' => {
        'type'=>'UsageContext',
        'path'=>'NamingSystem.useContext',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Intended jurisdiction for naming system (if applicable)
      # A legal or geographic region in which the naming system is intended to be used.
      # It may be possible for the naming system to be used in jurisdictions other than those for which it was originally designed or intended.
      'jurisdiction' => {
        'type'=>'CodeableConcept',
        'path'=>'NamingSystem.jurisdiction',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # How/where is it used
      # Provides guidance on the use of the namespace, including the handling of formatting characters, use of upper vs. lower case, etc.
      'usage' => {
        'type'=>'string',
        'path'=>'NamingSystem.usage',
        'min'=>0,
        'max'=>1
      },
      ##
      # Unique identifiers used for system
      # Indicates how the system may be identified when referenced in electronic exchange.
      # Multiple identifiers may exist, either due to duplicate registration, regional rules, needs of different communication technologies, etc.
      'uniqueId' => {
        'type'=>'NamingSystem::UniqueId',
        'path'=>'NamingSystem.uniqueId',
        'min'=>1,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Unique identifiers used for system
    # Indicates how the system may be identified when referenced in electronic exchange.
    # Multiple identifiers may exist, either due to duplicate registration, regional rules, needs of different communication technologies, etc.
    class UniqueId < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'UniqueId.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'UniqueId.extension',
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
          'path'=>'UniqueId.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # oid | uuid | uri | other
        # Identifies the unique identifier scheme used for this particular identifier.
        # Different identifier types may be used in different types of communications (OIDs for v3, URIs for FHIR, etc.).  Other includes RUIDs from v3, standard v2 code name strings, etc.
        'type' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/namingsystem-identifier-type'=>[ 'oid', 'uuid', 'uri', 'other' ]
          },
          'type'=>'code',
          'path'=>'UniqueId.type',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/namingsystem-identifier-type'}
        },
        ##
        # The unique identifier
        # The string that should be sent over the wire to identify the code system or identifier system.
        # If the value is a URI intended for use as FHIR system identifier, the URI should not contain "\" or "?" or "," since this makes escaping very difficult.
        'value' => {
          'type'=>'string',
          'path'=>'UniqueId.value',
          'min'=>1,
          'max'=>1
        },
        ##
        # Is this the id that should be used for this type
        # Indicates whether this identifier is the "preferred" identifier of this type.
        'preferred' => {
          'type'=>'boolean',
          'path'=>'UniqueId.preferred',
          'min'=>0,
          'max'=>1
        },
        ##
        # Notes about identifier usage
        # Notes about the past or intended usage of this identifier.
        # e.g. "must be used in Germany" or "was initially published in error with this value".
        'comment' => {
          'type'=>'string',
          'path'=>'UniqueId.comment',
          'min'=>0,
          'max'=>1
        },
        ##
        # When is identifier valid?
        # Identifies the period of time over which this identifier is considered appropriate to refer to the naming system.  Outside of this window, the identifier might be non-deterministic.
        # Within a registry, a given identifier should only be "active" for a single namespace at a time.  (Ideally, an identifier should only ever be associated with a single namespace across all time).
        'period' => {
          'type'=>'Period',
          'path'=>'UniqueId.period',
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
      # oid | uuid | uri | other
      # Identifies the unique identifier scheme used for this particular identifier.
      # Different identifier types may be used in different types of communications (OIDs for v3, URIs for FHIR, etc.).  Other includes RUIDs from v3, standard v2 code name strings, etc.
      attr_accessor :type                           # 1-1 code
      ##
      # The unique identifier
      # The string that should be sent over the wire to identify the code system or identifier system.
      # If the value is a URI intended for use as FHIR system identifier, the URI should not contain "\" or "?" or "," since this makes escaping very difficult.
      attr_accessor :value                          # 1-1 string
      ##
      # Is this the id that should be used for this type
      # Indicates whether this identifier is the "preferred" identifier of this type.
      attr_accessor :preferred                      # 0-1 boolean
      ##
      # Notes about identifier usage
      # Notes about the past or intended usage of this identifier.
      # e.g. "must be used in Germany" or "was initially published in error with this value".
      attr_accessor :comment                        # 0-1 string
      ##
      # When is identifier valid?
      # Identifies the period of time over which this identifier is considered appropriate to refer to the naming system.  Outside of this window, the identifier might be non-deterministic.
      # Within a registry, a given identifier should only be "active" for a single namespace at a time.  (Ideally, an identifier should only ever be associated with a single namespace across all time).
      attr_accessor :period                         # 0-1 Period
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
    # Name for this naming system (computer friendly)
    # A natural language name identifying the naming system. This name should be usable as an identifier for the module by machine processing applications such as code generation.
    # The name is not expected to be globally unique. The name should be a simple alphanumeric type name to ensure that it is machine-processing friendly.The"symbolic name" for an OID would be captured as an extension.
    attr_accessor :name                           # 1-1 string
    ##
    # draft | active | retired | unknown
    # The status of this naming system. Enables tracking the life-cycle of the content.
    # Allows filtering of naming systems that are appropriate for use versus not.
    attr_accessor :status                         # 1-1 code
    ##
    # codesystem | identifier | root
    # Indicates the purpose for the naming system - what kinds of things does it make unique?
    attr_accessor :kind                           # 1-1 code
    ##
    # Date last changed
    # The date  (and optionally time) when the naming system was published. The date must change when the business version changes and it must change if the status code changes. In addition, it should change when the substantive content of the naming system changes.
    # Note that this is not the same as the resource last-modified-date, since the resource may be a secondary representation of the naming system. Additional specific dates may be added as extensions or be found by consulting Provenances associated with past versions of the resource.
    attr_accessor :date                           # 1-1 dateTime
    ##
    # Name of the publisher (organization or individual)
    # The name of the organization or individual that published the naming system.
    # Usually an organization but may be an individual. The publisher (or steward) of the naming system is the organization or individual primarily responsible for the maintenance and upkeep of the naming system. This is not necessarily the same individual or organization that developed and initially authored the content. The publisher is the primary point of contact for questions or issues with the naming system. This item SHOULD be populated unless the information is available from context.
    attr_accessor :publisher                      # 0-1 string
    ##
    # Contact details for the publisher
    # Contact details to assist a user in finding and communicating with the publisher.
    # May be a web site, an email address, a telephone number, etc.
    attr_accessor :contact                        # 0-* [ ContactDetail ]
    ##
    # Who maintains system namespace?
    # The name of the organization that is responsible for issuing identifiers or codes for this namespace and ensuring their non-collision.
    # This is the primary organization.  Responsibility for some aspects of a namespace may be delegated.
    attr_accessor :responsible                    # 0-1 string
    ##
    # e.g. driver,  provider,  patient, bank etc.
    # Categorizes a naming system for easier search by grouping related naming systems.
    # This will most commonly be used for identifier namespaces, but categories could potentially be useful for code systems and authorities as well.
    attr_accessor :type                           # 0-1 CodeableConcept
    ##
    # Natural language description of the naming system
    # A free text natural language description of the naming system from a consumer's perspective. Details about what the namespace identifies including scope, granularity, version labeling, etc.
    # This description can be used to capture details such as why the naming system was built, comments about misuse, instructions for clinical use and interpretation, literature references, examples from the paper world, etc. It is not a rendering of the naming system as conveyed in the 'text' field of the resource itself. This item SHOULD be populated unless the information is available from context (e.g. the language of the naming system is presumed to be the predominant language in the place the naming system was created).
    attr_accessor :description                    # 0-1 markdown
    ##
    # The context that the content is intended to support
    # The content was developed with a focus and intent of supporting the contexts that are listed. These contexts may be general categories (gender, age, ...) or may be references to specific programs (insurance plans, studies, ...) and may be used to assist with indexing and searching for appropriate naming system instances.
    # When multiple useContexts are specified, there is no expectation that all or any of the contexts apply.
    attr_accessor :useContext                     # 0-* [ UsageContext ]
    ##
    # Intended jurisdiction for naming system (if applicable)
    # A legal or geographic region in which the naming system is intended to be used.
    # It may be possible for the naming system to be used in jurisdictions other than those for which it was originally designed or intended.
    attr_accessor :jurisdiction                   # 0-* [ CodeableConcept ]
    ##
    # How/where is it used
    # Provides guidance on the use of the namespace, including the handling of formatting characters, use of upper vs. lower case, etc.
    attr_accessor :usage                          # 0-1 string
    ##
    # Unique identifiers used for system
    # Indicates how the system may be identified when referenced in electronic exchange.
    # Multiple identifiers may exist, either due to duplicate registration, regional rules, needs of different communication technologies, etc.
    attr_accessor :uniqueId                       # 1-* [ NamingSystem::UniqueId ]

    def resourceType
      'NamingSystem'
    end
  end
end
