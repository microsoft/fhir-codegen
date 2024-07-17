module FHIR

  ##
  # A Map of relationships between 2 structures that can be used to transform data.
  class StructureMap < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['context-quantity', 'context-type-quantity', 'context-type-value', 'context-type', 'context', 'date', 'description', 'identifier', 'jurisdiction', 'name', 'publisher', 'status', 'title', 'url', 'version']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'StructureMap.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'StructureMap.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'StructureMap.implicitRules',
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
        'path'=>'StructureMap.language',
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
        'path'=>'StructureMap.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'StructureMap.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'StructureMap.extension',
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
        'path'=>'StructureMap.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Canonical identifier for this structure map, represented as a URI (globally unique)
      # An absolute URI that is used to identify this structure map when it is referenced in a specification, model, design or an instance; also called its canonical identifier. This SHOULD be globally unique and SHOULD be a literal address at which at which an authoritative instance of this structure map is (or will be) published. This URL can be the target of a canonical reference. It SHALL remain the same when the structure map is stored on different servers.
      # Can be a urn:uuid: or a urn:oid: but real http: addresses are preferred.  Multiple instances may share the same URL if they have a distinct version.
      # 
      # The determination of when to create a new version of a resource (same url, new version) vs. defining a new artifact is up to the author.  Considerations for making this decision are found in [Technical and Business Versions](resource.html#versions). 
      # 
      # In some cases, the resource can no longer be found at the stated url, but the url itself cannot change. Implementations can use the [meta.source](resource.html#meta) element to indicate where the current master source of the resource can be found.
      'url' => {
        'type'=>'uri',
        'path'=>'StructureMap.url',
        'min'=>1,
        'max'=>1
      },
      ##
      # Additional identifier for the structure map
      # A formal identifier that is used to identify this structure map when it is represented in other formats, or referenced in a specification, model, design or an instance.
      # Typically, this is used for identifiers that can go in an HL7 V3 II (instance identifier) data type, and can then identify this structure map outside of FHIR, where it is not possible to use the logical URI.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'StructureMap.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Business version of the structure map
      # The identifier that is used to identify this version of the structure map when it is referenced in a specification, model, design or instance. This is an arbitrary value managed by the structure map author and is not expected to be globally unique. For example, it might be a timestamp (e.g. yyyymmdd) if a managed version is not available. There is also no expectation that versions can be placed in a lexicographical sequence.
      # There may be different structure map instances that have the same identifier but different versions.  The version can be appended to the url in a reference to allow a reference to a particular business version of the structure map with the format [url]|[version].
      'version' => {
        'type'=>'string',
        'path'=>'StructureMap.version',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name for this structure map (computer friendly)
      # A natural language name identifying the structure map. This name should be usable as an identifier for the module by machine processing applications such as code generation.
      # The name is not expected to be globally unique. The name should be a simple alphanumeric type name to ensure that it is machine-processing friendly.
      'name' => {
        'type'=>'string',
        'path'=>'StructureMap.name',
        'min'=>1,
        'max'=>1
      },
      ##
      # Name for this structure map (human friendly)
      # A short, descriptive, user-friendly title for the structure map.
      # This name does not need to be machine-processing friendly and may contain punctuation, white-space, etc.
      'title' => {
        'type'=>'string',
        'path'=>'StructureMap.title',
        'min'=>0,
        'max'=>1
      },
      ##
      # draft | active | retired | unknown
      # The status of this structure map. Enables tracking the life-cycle of the content.
      # Allows filtering of structure maps that are appropriate for use versus not.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/publication-status'=>[ 'draft', 'active', 'retired', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'StructureMap.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/publication-status'}
      },
      ##
      # For testing purposes, not real usage
      # A Boolean value to indicate that this structure map is authored for testing purposes (or education/evaluation/marketing) and is not intended to be used for genuine usage.
      # Allows filtering of structure maps that are appropriate for use versus not.
      'experimental' => {
        'type'=>'boolean',
        'path'=>'StructureMap.experimental',
        'min'=>0,
        'max'=>1
      },
      ##
      # Date last changed
      # The date  (and optionally time) when the structure map was published. The date must change when the business version changes and it must change if the status code changes. In addition, it should change when the substantive content of the structure map changes.
      # Note that this is not the same as the resource last-modified-date, since the resource may be a secondary representation of the structure map. Additional specific dates may be added as extensions or be found by consulting Provenances associated with past versions of the resource.
      'date' => {
        'type'=>'dateTime',
        'path'=>'StructureMap.date',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name of the publisher (organization or individual)
      # The name of the organization or individual that published the structure map.
      # Usually an organization but may be an individual. The publisher (or steward) of the structure map is the organization or individual primarily responsible for the maintenance and upkeep of the structure map. This is not necessarily the same individual or organization that developed and initially authored the content. The publisher is the primary point of contact for questions or issues with the structure map. This item SHOULD be populated unless the information is available from context.
      'publisher' => {
        'type'=>'string',
        'path'=>'StructureMap.publisher',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contact details for the publisher
      # Contact details to assist a user in finding and communicating with the publisher.
      # May be a web site, an email address, a telephone number, etc.
      'contact' => {
        'type'=>'ContactDetail',
        'path'=>'StructureMap.contact',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Natural language description of the structure map
      # A free text natural language description of the structure map from a consumer's perspective.
      # This description can be used to capture details such as why the structure map was built, comments about misuse, instructions for clinical use and interpretation, literature references, examples from the paper world, etc. It is not a rendering of the structure map as conveyed in the 'text' field of the resource itself. This item SHOULD be populated unless the information is available from context (e.g. the language of the structure map is presumed to be the predominant language in the place the structure map was created).
      'description' => {
        'type'=>'markdown',
        'path'=>'StructureMap.description',
        'min'=>0,
        'max'=>1
      },
      ##
      # The context that the content is intended to support
      # The content was developed with a focus and intent of supporting the contexts that are listed. These contexts may be general categories (gender, age, ...) or may be references to specific programs (insurance plans, studies, ...) and may be used to assist with indexing and searching for appropriate structure map instances.
      # When multiple useContexts are specified, there is no expectation that all or any of the contexts apply.
      'useContext' => {
        'type'=>'UsageContext',
        'path'=>'StructureMap.useContext',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Intended jurisdiction for structure map (if applicable)
      # A legal or geographic region in which the structure map is intended to be used.
      # It may be possible for the structure map to be used in jurisdictions other than those for which it was originally designed or intended.
      'jurisdiction' => {
        'type'=>'CodeableConcept',
        'path'=>'StructureMap.jurisdiction',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Why this structure map is defined
      # Explanation of why this structure map is needed and why it has been designed as it has.
      # This element does not describe the usage of the structure map. Instead, it provides traceability of ''why'' the resource is either needed or ''why'' it is defined as it is.  This may be used to point to source materials or specifications that drove the structure of this structure map.
      'purpose' => {
        'type'=>'markdown',
        'path'=>'StructureMap.purpose',
        'min'=>0,
        'max'=>1
      },
      ##
      # Use and/or publishing restrictions
      # A copyright statement relating to the structure map and/or its contents. Copyright statements are generally legal restrictions on the use and publishing of the structure map.
      'copyright' => {
        'type'=>'markdown',
        'path'=>'StructureMap.copyright',
        'min'=>0,
        'max'=>1
      },
      ##
      # Structure Definition used by this map
      # A structure definition used by this map. The structure definition may describe instances that are converted, or the instances that are produced.
      # It is not necessary for a structure map to identify any dependent structures, though not listing them may restrict its usefulness.
      'structure' => {
        'type'=>'StructureMap::Structure',
        'path'=>'StructureMap.structure',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Other maps used by this map (canonical URLs).
      'import' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/StructureMap'],
        'type'=>'canonical',
        'path'=>'StructureMap.import',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Named sections for reader convenience
      # Organizes the mapping into manageable chunks for human review/ease of maintenance.
      'group' => {
        'type'=>'StructureMap::Group',
        'path'=>'StructureMap.group',
        'min'=>1,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Structure Definition used by this map
    # A structure definition used by this map. The structure definition may describe instances that are converted, or the instances that are produced.
    # It is not necessary for a structure map to identify any dependent structures, though not listing them may restrict its usefulness.
    class Structure < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Structure.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Structure.extension',
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
          'path'=>'Structure.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Canonical reference to structure definition
        # The canonical reference to the structure.
        'url' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/StructureDefinition'],
          'type'=>'canonical',
          'path'=>'Structure.url',
          'min'=>1,
          'max'=>1
        },
        ##
        # source | queried | target | produced
        # How the referenced structure is used in this mapping.
        'mode' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/map-model-mode'=>[ 'source', 'queried', 'target', 'produced' ]
          },
          'type'=>'code',
          'path'=>'Structure.mode',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/map-model-mode'}
        },
        ##
        # Name for type in this map
        # The name used for this type in the map.
        # This is needed if both types have the same name (e.g. version conversion).
        'alias' => {
          'local_name'=>'local_alias'
          'type'=>'string',
          'path'=>'Structure.alias',
          'min'=>0,
          'max'=>1
        },
        ##
        # Documentation on use of structure
        # Documentation that describes how the structure is used in the mapping.
        'documentation' => {
          'type'=>'string',
          'path'=>'Structure.documentation',
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
      # Canonical reference to structure definition
      # The canonical reference to the structure.
      attr_accessor :url                            # 1-1 canonical(http://hl7.org/fhir/StructureDefinition/StructureDefinition)
      ##
      # source | queried | target | produced
      # How the referenced structure is used in this mapping.
      attr_accessor :mode                           # 1-1 code
      ##
      # Name for type in this map
      # The name used for this type in the map.
      # This is needed if both types have the same name (e.g. version conversion).
      attr_accessor :local_alias                    # 0-1 string
      ##
      # Documentation on use of structure
      # Documentation that describes how the structure is used in the mapping.
      attr_accessor :documentation                  # 0-1 string
    end

    ##
    # Named sections for reader convenience
    # Organizes the mapping into manageable chunks for human review/ease of maintenance.
    class Group < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Group.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Group.extension',
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
          'path'=>'Group.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Human-readable label
        # A unique name for the group for the convenience of human readers.
        'name' => {
          'type'=>'id',
          'path'=>'Group.name',
          'min'=>1,
          'max'=>1
        },
        ##
        # Another group that this group adds rules to.
        'extends' => {
          'type'=>'id',
          'path'=>'Group.extends',
          'min'=>0,
          'max'=>1
        },
        ##
        # none | types | type-and-types
        # If this is the default rule set to apply for the source type or this combination of types.
        # Not applicable if the underlying model is untyped. There can only be one default mapping for any particular type combination.
        'typeMode' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/map-group-type-mode'=>[ 'none', 'types', 'type-and-types' ]
          },
          'type'=>'code',
          'path'=>'Group.typeMode',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/map-group-type-mode'}
        },
        ##
        # Additional description/explanation for group
        # Additional supporting documentation that explains the purpose of the group and the types of mappings within it.
        'documentation' => {
          'type'=>'string',
          'path'=>'Group.documentation',
          'min'=>0,
          'max'=>1
        },
        ##
        # Named instance provided when invoking the map
        # A name assigned to an instance of data. The instance must be provided when the mapping is invoked.
        # If no inputs are named, then the entry mappings are type based.
        'input' => {
          'type'=>'StructureMap::Group::Input',
          'path'=>'Group.input',
          'min'=>1,
          'max'=>Float::INFINITY
        },
        ##
        # Transform Rule from source to target.
        'rule' => {
          'type'=>'StructureMap::Group::Rule',
          'path'=>'Group.rule',
          'min'=>1,
          'max'=>Float::INFINITY
        }
      }

      ##
      # Named instance provided when invoking the map
      # A name assigned to an instance of data. The instance must be provided when the mapping is invoked.
      # If no inputs are named, then the entry mappings are type based.
      class Input < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
            'path'=>'Input.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Input.extension',
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
            'path'=>'Input.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Name for this instance of data.
          'name' => {
            'type'=>'id',
            'path'=>'Input.name',
            'min'=>1,
            'max'=>1
          },
          ##
          # Type for this instance of data.
          'type' => {
            'type'=>'string',
            'path'=>'Input.type',
            'min'=>0,
            'max'=>1
          },
          ##
          # source | target
          # Mode for this instance of data.
          'mode' => {
            'valid_codes'=>{
              'http://hl7.org/fhir/map-input-mode'=>[ 'source', 'target' ]
            },
            'type'=>'code',
            'path'=>'Input.mode',
            'min'=>1,
            'max'=>1,
            'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/map-input-mode'}
          },
          ##
          # Documentation for this instance of data.
          'documentation' => {
            'type'=>'string',
            'path'=>'Input.documentation',
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
        # Name for this instance of data.
        attr_accessor :name                           # 1-1 id
        ##
        # Type for this instance of data.
        attr_accessor :type                           # 0-1 string
        ##
        # source | target
        # Mode for this instance of data.
        attr_accessor :mode                           # 1-1 code
        ##
        # Documentation for this instance of data.
        attr_accessor :documentation                  # 0-1 string
      end

      ##
      # Transform Rule from source to target.
      class Rule < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
            'path'=>'Rule.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Rule.extension',
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
            'path'=>'Rule.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Name of the rule for internal references.
          'name' => {
            'type'=>'id',
            'path'=>'Rule.name',
            'min'=>1,
            'max'=>1
          },
          ##
          # Source inputs to the mapping.
          'source' => {
            'type'=>'StructureMap::Group::Rule::Source',
            'path'=>'Rule.source',
            'min'=>1,
            'max'=>Float::INFINITY
          },
          ##
          # Content to create because of this mapping rule.
          'target' => {
            'type'=>'StructureMap::Group::Rule::Target',
            'path'=>'Rule.target',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Rules contained in this rule.
          'rule' => {
            'type'=>'StructureMap::Group::Rule',
            'path'=>'Rule.rule',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Which other rules to apply in the context of this rule.
          'dependent' => {
            'type'=>'StructureMap::Group::Rule::Dependent',
            'path'=>'Rule.dependent',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Documentation for this instance of data.
          'documentation' => {
            'type'=>'string',
            'path'=>'Rule.documentation',
            'min'=>0,
            'max'=>1
          }
        }

        ##
        # Source inputs to the mapping.
        class Source < FHIR::Model
          include FHIR::Hashable
          include FHIR::Json
          include FHIR::Xml

          MULTIPLE_TYPES = {
            'defaultValue[x]' => ['Address', 'Age', 'Annotation', 'Attachment', 'base64Binary', 'boolean', 'canonical', 'code', 'CodeableConcept', 'Coding', 'ContactDetail', 'ContactPoint', 'Contributor', 'Count', 'DataRequirement', 'date', 'dateTime', 'decimal', 'Distance', 'Dosage', 'Duration', 'Expression', 'HumanName', 'id', 'Identifier', 'instant', 'integer', 'markdown', 'Meta', 'Money', 'oid', 'ParameterDefinition', 'Period', 'positiveInt', 'Quantity', 'Range', 'Ratio', 'Reference', 'RelatedArtifact', 'SampledData', 'Signature', 'string', 'time', 'Timing', 'TriggerDefinition', 'unsignedInt', 'uri', 'url', 'UsageContext', 'uuid']
          }
          METADATA = {
            ##
            # Unique id for inter-element referencing
            # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
            'id' => {
              'type'=>'string',
              'path'=>'Source.id',
              'min'=>0,
              'max'=>1
            },
            ##
            # Additional content defined by implementations
            # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
            # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
            'extension' => {
              'type'=>'Extension',
              'path'=>'Source.extension',
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
              'path'=>'Source.modifierExtension',
              'min'=>0,
              'max'=>Float::INFINITY
            },
            ##
            # Type or variable this rule applies to.
            'context' => {
              'type'=>'id',
              'path'=>'Source.context',
              'min'=>1,
              'max'=>1
            },
            ##
            # Specified minimum cardinality for the element. This is optional; if present, it acts an implicit check on the input content.
            'min' => {
              'type'=>'integer',
              'path'=>'Source.min',
              'min'=>0,
              'max'=>1
            },
            ##
            # Specified maximum cardinality (number or *)
            # Specified maximum cardinality for the element - a number or a "*". This is optional; if present, it acts an implicit check on the input content (* just serves as documentation; it's the default value).
            'max' => {
              'type'=>'string',
              'path'=>'Source.max',
              'min'=>0,
              'max'=>1
            },
            ##
            # Rule only applies if source has this type
            # Specified type for the element. This works as a condition on the mapping - use for polymorphic elements.
            'type' => {
              'type'=>'string',
              'path'=>'Source.type',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueAddress' => {
              'type'=>'Address',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueAge' => {
              'type'=>'Age',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueAnnotation' => {
              'type'=>'Annotation',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueAttachment' => {
              'type'=>'Attachment',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueBase64Binary' => {
              'type'=>'Base64Binary',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueBoolean' => {
              'type'=>'Boolean',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueCanonical' => {
              'type'=>'Canonical',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueCode' => {
              'type'=>'Code',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueCodeableConcept' => {
              'type'=>'CodeableConcept',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueCoding' => {
              'type'=>'Coding',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueContactDetail' => {
              'type'=>'ContactDetail',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueContactPoint' => {
              'type'=>'ContactPoint',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueContributor' => {
              'type'=>'Contributor',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueCount' => {
              'type'=>'Count',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueDataRequirement' => {
              'type'=>'DataRequirement',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueDate' => {
              'type'=>'Date',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueDateTime' => {
              'type'=>'DateTime',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueDecimal' => {
              'type'=>'Decimal',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueDistance' => {
              'type'=>'Distance',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueDosage' => {
              'type'=>'Dosage',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueDuration' => {
              'type'=>'Duration',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueExpression' => {
              'type'=>'Expression',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueHumanName' => {
              'type'=>'HumanName',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueId' => {
              'type'=>'Id',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueIdentifier' => {
              'type'=>'Identifier',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueInstant' => {
              'type'=>'Instant',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueInteger' => {
              'type'=>'Integer',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueMarkdown' => {
              'type'=>'Markdown',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueMeta' => {
              'type'=>'Meta',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueMoney' => {
              'type'=>'Money',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueOid' => {
              'type'=>'Oid',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueParameterDefinition' => {
              'type'=>'ParameterDefinition',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValuePeriod' => {
              'type'=>'Period',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValuePositiveInt' => {
              'type'=>'PositiveInt',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueQuantity' => {
              'type'=>'Quantity',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueRange' => {
              'type'=>'Range',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueRatio' => {
              'type'=>'Ratio',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueReference' => {
              'type'=>'Reference',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueRelatedArtifact' => {
              'type'=>'RelatedArtifact',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueSampledData' => {
              'type'=>'SampledData',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueSignature' => {
              'type'=>'Signature',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueString' => {
              'type'=>'String',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueTime' => {
              'type'=>'Time',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueTiming' => {
              'type'=>'Timing',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueTriggerDefinition' => {
              'type'=>'TriggerDefinition',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueUnsignedInt' => {
              'type'=>'UnsignedInt',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueUri' => {
              'type'=>'Uri',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueUrl' => {
              'type'=>'Url',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueUsageContext' => {
              'type'=>'UsageContext',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Default value if no value exists
            # A value to use if there is no existing value in the source object.
            # If there's a default value on an item that can repeat, it will only be used once.
            'defaultValueUuid' => {
              'type'=>'Uuid',
              'path'=>'Source.defaultValue[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Optional field for this source.
            'element' => {
              'type'=>'string',
              'path'=>'Source.element',
              'min'=>0,
              'max'=>1
            },
            ##
            # first | not_first | last | not_last | only_one
            # How to handle the list mode for this element.
            'listMode' => {
              'valid_codes'=>{
                'http://hl7.org/fhir/map-source-list-mode'=>[ 'first', 'not_first', 'last', 'not_last', 'only_one' ]
              },
              'type'=>'code',
              'path'=>'Source.listMode',
              'min'=>0,
              'max'=>1,
              'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/map-source-list-mode'}
            },
            ##
            # Named context for field, if a field is specified.
            'variable' => {
              'type'=>'id',
              'path'=>'Source.variable',
              'min'=>0,
              'max'=>1
            },
            ##
            # FHIRPath expression  - must be true or the rule does not apply.
            'condition' => {
              'type'=>'string',
              'path'=>'Source.condition',
              'min'=>0,
              'max'=>1
            },
            ##
            # FHIRPath expression  - must be true or the mapping engine throws an error instead of completing.
            'check' => {
              'type'=>'string',
              'path'=>'Source.check',
              'min'=>0,
              'max'=>1
            },
            ##
            # Message to put in log if source exists (FHIRPath)
            # A FHIRPath expression which specifies a message to put in the transform log when content matching the source rule is found.
            # This is typically used for recording that something Is not transformed to the target for some reason.
            'logMessage' => {
              'type'=>'string',
              'path'=>'Source.logMessage',
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
          # Type or variable this rule applies to.
          attr_accessor :context                        # 1-1 id
          ##
          # Specified minimum cardinality for the element. This is optional; if present, it acts an implicit check on the input content.
          attr_accessor :min                            # 0-1 integer
          ##
          # Specified maximum cardinality (number or *)
          # Specified maximum cardinality for the element - a number or a "*". This is optional; if present, it acts an implicit check on the input content (* just serves as documentation; it's the default value).
          attr_accessor :max                            # 0-1 string
          ##
          # Rule only applies if source has this type
          # Specified type for the element. This works as a condition on the mapping - use for polymorphic elements.
          attr_accessor :type                           # 0-1 string
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueAddress            # 0-1 Address
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueAge                # 0-1 Age
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueAnnotation         # 0-1 Annotation
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueAttachment         # 0-1 Attachment
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueBase64Binary       # 0-1 Base64Binary
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueBoolean            # 0-1 Boolean
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueCanonical          # 0-1 Canonical
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueCode               # 0-1 Code
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueCodeableConcept    # 0-1 CodeableConcept
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueCoding             # 0-1 Coding
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueContactDetail      # 0-1 ContactDetail
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueContactPoint       # 0-1 ContactPoint
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueContributor        # 0-1 Contributor
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueCount              # 0-1 Count
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueDataRequirement    # 0-1 DataRequirement
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueDate               # 0-1 Date
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueDateTime           # 0-1 DateTime
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueDecimal            # 0-1 Decimal
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueDistance           # 0-1 Distance
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueDosage             # 0-1 Dosage
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueDuration           # 0-1 Duration
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueExpression         # 0-1 Expression
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueHumanName          # 0-1 HumanName
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueId                 # 0-1 Id
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueIdentifier         # 0-1 Identifier
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueInstant            # 0-1 Instant
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueInteger            # 0-1 Integer
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueMarkdown           # 0-1 Markdown
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueMeta               # 0-1 Meta
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueMoney              # 0-1 Money
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueOid                # 0-1 Oid
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueParameterDefinition # 0-1 ParameterDefinition
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValuePeriod             # 0-1 Period
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValuePositiveInt        # 0-1 PositiveInt
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueQuantity           # 0-1 Quantity
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueRange              # 0-1 Range
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueRatio              # 0-1 Ratio
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueReference          # 0-1 Reference
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueRelatedArtifact    # 0-1 RelatedArtifact
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueSampledData        # 0-1 SampledData
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueSignature          # 0-1 Signature
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueString             # 0-1 String
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueTime               # 0-1 Time
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueTiming             # 0-1 Timing
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueTriggerDefinition  # 0-1 TriggerDefinition
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueUnsignedInt        # 0-1 UnsignedInt
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueUri                # 0-1 Uri
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueUrl                # 0-1 Url
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueUsageContext       # 0-1 UsageContext
          ##
          # Default value if no value exists
          # A value to use if there is no existing value in the source object.
          # If there's a default value on an item that can repeat, it will only be used once.
          attr_accessor :defaultValueUuid               # 0-1 Uuid
          ##
          # Optional field for this source.
          attr_accessor :element                        # 0-1 string
          ##
          # first | not_first | last | not_last | only_one
          # How to handle the list mode for this element.
          attr_accessor :listMode                       # 0-1 code
          ##
          # Named context for field, if a field is specified.
          attr_accessor :variable                       # 0-1 id
          ##
          # FHIRPath expression  - must be true or the rule does not apply.
          attr_accessor :condition                      # 0-1 string
          ##
          # FHIRPath expression  - must be true or the mapping engine throws an error instead of completing.
          attr_accessor :check                          # 0-1 string
          ##
          # Message to put in log if source exists (FHIRPath)
          # A FHIRPath expression which specifies a message to put in the transform log when content matching the source rule is found.
          # This is typically used for recording that something Is not transformed to the target for some reason.
          attr_accessor :logMessage                     # 0-1 string
        end

        ##
        # Content to create because of this mapping rule.
        class Target < FHIR::Model
          include FHIR::Hashable
          include FHIR::Json
          include FHIR::Xml

          METADATA = {
            ##
            # Unique id for inter-element referencing
            # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
            'id' => {
              'type'=>'string',
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
            # Type or variable this rule applies to.
            'context' => {
              'type'=>'id',
              'path'=>'Target.context',
              'min'=>0,
              'max'=>1
            },
            ##
            # type | variable
            # How to interpret the context.
            'contextType' => {
              'valid_codes'=>{
                'http://hl7.org/fhir/map-context-type'=>[ 'type', 'variable' ]
              },
              'type'=>'code',
              'path'=>'Target.contextType',
              'min'=>0,
              'max'=>1,
              'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/map-context-type'}
            },
            ##
            # Field to create in the context.
            'element' => {
              'type'=>'string',
              'path'=>'Target.element',
              'min'=>0,
              'max'=>1
            },
            ##
            # Named context for field, if desired, and a field is specified.
            'variable' => {
              'type'=>'id',
              'path'=>'Target.variable',
              'min'=>0,
              'max'=>1
            },
            ##
            # first | share | last | collate
            # If field is a list, how to manage the list.
            'listMode' => {
              'valid_codes'=>{
                'http://hl7.org/fhir/map-target-list-mode'=>[ 'first', 'share', 'last', 'collate' ]
              },
              'type'=>'code',
              'path'=>'Target.listMode',
              'min'=>0,
              'max'=>Float::INFINITY,
              'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/map-target-list-mode'}
            },
            ##
            # Internal rule reference for shared list items.
            'listRuleId' => {
              'type'=>'id',
              'path'=>'Target.listRuleId',
              'min'=>0,
              'max'=>1
            },
            ##
            # create | copy +
            # How the data is copied / created.
            'transform' => {
              'valid_codes'=>{
                'http://hl7.org/fhir/map-transform'=>[ 'create', 'copy', 'truncate', 'escape', 'cast', 'append', 'translate', 'reference', 'dateOp', 'uuid', 'pointer', 'evaluate', 'cc', 'c', 'qty', 'id', 'cp' ]
              },
              'type'=>'code',
              'path'=>'Target.transform',
              'min'=>0,
              'max'=>1,
              'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/map-transform'}
            },
            ##
            # Parameters to the transform.
            'parameter' => {
              'type'=>'StructureMap::Group::Rule::Target::Parameter',
              'path'=>'Target.parameter',
              'min'=>0,
              'max'=>Float::INFINITY
            }
          }

          ##
          # Parameters to the transform.
          class Parameter < FHIR::Model
            include FHIR::Hashable
            include FHIR::Json
            include FHIR::Xml

            MULTIPLE_TYPES = {
              'value[x]' => ['boolean', 'decimal', 'id', 'integer', 'string']
            }
            METADATA = {
              ##
              # Unique id for inter-element referencing
              # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
              'id' => {
                'type'=>'string',
                'path'=>'Parameter.id',
                'min'=>0,
                'max'=>1
              },
              ##
              # Additional content defined by implementations
              # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
              # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
              'extension' => {
                'type'=>'Extension',
                'path'=>'Parameter.extension',
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
                'path'=>'Parameter.modifierExtension',
                'min'=>0,
                'max'=>Float::INFINITY
              },
              ##
              # Parameter value - variable or literal.
              'valueBoolean' => {
                'type'=>'Boolean',
                'path'=>'Parameter.value[x]',
                'min'=>1,
                'max'=>1
              }
              ##
              # Parameter value - variable or literal.
              'valueDecimal' => {
                'type'=>'Decimal',
                'path'=>'Parameter.value[x]',
                'min'=>1,
                'max'=>1
              }
              ##
              # Parameter value - variable or literal.
              'valueId' => {
                'type'=>'Id',
                'path'=>'Parameter.value[x]',
                'min'=>1,
                'max'=>1
              }
              ##
              # Parameter value - variable or literal.
              'valueInteger' => {
                'type'=>'Integer',
                'path'=>'Parameter.value[x]',
                'min'=>1,
                'max'=>1
              }
              ##
              # Parameter value - variable or literal.
              'valueString' => {
                'type'=>'String',
                'path'=>'Parameter.value[x]',
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
            # Parameter value - variable or literal.
            attr_accessor :valueBoolean                   # 1-1 Boolean
            ##
            # Parameter value - variable or literal.
            attr_accessor :valueDecimal                   # 1-1 Decimal
            ##
            # Parameter value - variable or literal.
            attr_accessor :valueId                        # 1-1 Id
            ##
            # Parameter value - variable or literal.
            attr_accessor :valueInteger                   # 1-1 Integer
            ##
            # Parameter value - variable or literal.
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
          # Type or variable this rule applies to.
          attr_accessor :context                        # 0-1 id
          ##
          # type | variable
          # How to interpret the context.
          attr_accessor :contextType                    # 0-1 code
          ##
          # Field to create in the context.
          attr_accessor :element                        # 0-1 string
          ##
          # Named context for field, if desired, and a field is specified.
          attr_accessor :variable                       # 0-1 id
          ##
          # first | share | last | collate
          # If field is a list, how to manage the list.
          attr_accessor :listMode                       # 0-* [ code ]
          ##
          # Internal rule reference for shared list items.
          attr_accessor :listRuleId                     # 0-1 id
          ##
          # create | copy +
          # How the data is copied / created.
          attr_accessor :transform                      # 0-1 code
          ##
          # Parameters to the transform.
          attr_accessor :parameter                      # 0-* [ StructureMap::Group::Rule::Target::Parameter ]
        end

        ##
        # Which other rules to apply in the context of this rule.
        class Dependent < FHIR::Model
          include FHIR::Hashable
          include FHIR::Json
          include FHIR::Xml

          METADATA = {
            ##
            # Unique id for inter-element referencing
            # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
            'id' => {
              'type'=>'string',
              'path'=>'Dependent.id',
              'min'=>0,
              'max'=>1
            },
            ##
            # Additional content defined by implementations
            # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
            # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
            'extension' => {
              'type'=>'Extension',
              'path'=>'Dependent.extension',
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
              'path'=>'Dependent.modifierExtension',
              'min'=>0,
              'max'=>Float::INFINITY
            },
            ##
            # Name of a rule or group to apply.
            'name' => {
              'type'=>'id',
              'path'=>'Dependent.name',
              'min'=>1,
              'max'=>1
            },
            ##
            # Variable to pass to the rule or group.
            'variable' => {
              'type'=>'string',
              'path'=>'Dependent.variable',
              'min'=>1,
              'max'=>Float::INFINITY
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
          # Name of a rule or group to apply.
          attr_accessor :name                           # 1-1 id
          ##
          # Variable to pass to the rule or group.
          attr_accessor :variable                       # 1-* [ string ]
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
        # Name of the rule for internal references.
        attr_accessor :name                           # 1-1 id
        ##
        # Source inputs to the mapping.
        attr_accessor :source                         # 1-* [ StructureMap::Group::Rule::Source ]
        ##
        # Content to create because of this mapping rule.
        attr_accessor :target                         # 0-* [ StructureMap::Group::Rule::Target ]
        ##
        # Rules contained in this rule.
        attr_accessor :rule                           # 0-* [ StructureMap::Group::Rule ]
        ##
        # Which other rules to apply in the context of this rule.
        attr_accessor :dependent                      # 0-* [ StructureMap::Group::Rule::Dependent ]
        ##
        # Documentation for this instance of data.
        attr_accessor :documentation                  # 0-1 string
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
      # Human-readable label
      # A unique name for the group for the convenience of human readers.
      attr_accessor :name                           # 1-1 id
      ##
      # Another group that this group adds rules to.
      attr_accessor :extends                        # 0-1 id
      ##
      # none | types | type-and-types
      # If this is the default rule set to apply for the source type or this combination of types.
      # Not applicable if the underlying model is untyped. There can only be one default mapping for any particular type combination.
      attr_accessor :typeMode                       # 1-1 code
      ##
      # Additional description/explanation for group
      # Additional supporting documentation that explains the purpose of the group and the types of mappings within it.
      attr_accessor :documentation                  # 0-1 string
      ##
      # Named instance provided when invoking the map
      # A name assigned to an instance of data. The instance must be provided when the mapping is invoked.
      # If no inputs are named, then the entry mappings are type based.
      attr_accessor :input                          # 1-* [ StructureMap::Group::Input ]
      ##
      # Transform Rule from source to target.
      attr_accessor :rule                           # 1-* [ StructureMap::Group::Rule ]
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
    # Canonical identifier for this structure map, represented as a URI (globally unique)
    # An absolute URI that is used to identify this structure map when it is referenced in a specification, model, design or an instance; also called its canonical identifier. This SHOULD be globally unique and SHOULD be a literal address at which at which an authoritative instance of this structure map is (or will be) published. This URL can be the target of a canonical reference. It SHALL remain the same when the structure map is stored on different servers.
    # Can be a urn:uuid: or a urn:oid: but real http: addresses are preferred.  Multiple instances may share the same URL if they have a distinct version.
    # 
    # The determination of when to create a new version of a resource (same url, new version) vs. defining a new artifact is up to the author.  Considerations for making this decision are found in [Technical and Business Versions](resource.html#versions). 
    # 
    # In some cases, the resource can no longer be found at the stated url, but the url itself cannot change. Implementations can use the [meta.source](resource.html#meta) element to indicate where the current master source of the resource can be found.
    attr_accessor :url                            # 1-1 uri
    ##
    # Additional identifier for the structure map
    # A formal identifier that is used to identify this structure map when it is represented in other formats, or referenced in a specification, model, design or an instance.
    # Typically, this is used for identifiers that can go in an HL7 V3 II (instance identifier) data type, and can then identify this structure map outside of FHIR, where it is not possible to use the logical URI.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Business version of the structure map
    # The identifier that is used to identify this version of the structure map when it is referenced in a specification, model, design or instance. This is an arbitrary value managed by the structure map author and is not expected to be globally unique. For example, it might be a timestamp (e.g. yyyymmdd) if a managed version is not available. There is also no expectation that versions can be placed in a lexicographical sequence.
    # There may be different structure map instances that have the same identifier but different versions.  The version can be appended to the url in a reference to allow a reference to a particular business version of the structure map with the format [url]|[version].
    attr_accessor :version                        # 0-1 string
    ##
    # Name for this structure map (computer friendly)
    # A natural language name identifying the structure map. This name should be usable as an identifier for the module by machine processing applications such as code generation.
    # The name is not expected to be globally unique. The name should be a simple alphanumeric type name to ensure that it is machine-processing friendly.
    attr_accessor :name                           # 1-1 string
    ##
    # Name for this structure map (human friendly)
    # A short, descriptive, user-friendly title for the structure map.
    # This name does not need to be machine-processing friendly and may contain punctuation, white-space, etc.
    attr_accessor :title                          # 0-1 string
    ##
    # draft | active | retired | unknown
    # The status of this structure map. Enables tracking the life-cycle of the content.
    # Allows filtering of structure maps that are appropriate for use versus not.
    attr_accessor :status                         # 1-1 code
    ##
    # For testing purposes, not real usage
    # A Boolean value to indicate that this structure map is authored for testing purposes (or education/evaluation/marketing) and is not intended to be used for genuine usage.
    # Allows filtering of structure maps that are appropriate for use versus not.
    attr_accessor :experimental                   # 0-1 boolean
    ##
    # Date last changed
    # The date  (and optionally time) when the structure map was published. The date must change when the business version changes and it must change if the status code changes. In addition, it should change when the substantive content of the structure map changes.
    # Note that this is not the same as the resource last-modified-date, since the resource may be a secondary representation of the structure map. Additional specific dates may be added as extensions or be found by consulting Provenances associated with past versions of the resource.
    attr_accessor :date                           # 0-1 dateTime
    ##
    # Name of the publisher (organization or individual)
    # The name of the organization or individual that published the structure map.
    # Usually an organization but may be an individual. The publisher (or steward) of the structure map is the organization or individual primarily responsible for the maintenance and upkeep of the structure map. This is not necessarily the same individual or organization that developed and initially authored the content. The publisher is the primary point of contact for questions or issues with the structure map. This item SHOULD be populated unless the information is available from context.
    attr_accessor :publisher                      # 0-1 string
    ##
    # Contact details for the publisher
    # Contact details to assist a user in finding and communicating with the publisher.
    # May be a web site, an email address, a telephone number, etc.
    attr_accessor :contact                        # 0-* [ ContactDetail ]
    ##
    # Natural language description of the structure map
    # A free text natural language description of the structure map from a consumer's perspective.
    # This description can be used to capture details such as why the structure map was built, comments about misuse, instructions for clinical use and interpretation, literature references, examples from the paper world, etc. It is not a rendering of the structure map as conveyed in the 'text' field of the resource itself. This item SHOULD be populated unless the information is available from context (e.g. the language of the structure map is presumed to be the predominant language in the place the structure map was created).
    attr_accessor :description                    # 0-1 markdown
    ##
    # The context that the content is intended to support
    # The content was developed with a focus and intent of supporting the contexts that are listed. These contexts may be general categories (gender, age, ...) or may be references to specific programs (insurance plans, studies, ...) and may be used to assist with indexing and searching for appropriate structure map instances.
    # When multiple useContexts are specified, there is no expectation that all or any of the contexts apply.
    attr_accessor :useContext                     # 0-* [ UsageContext ]
    ##
    # Intended jurisdiction for structure map (if applicable)
    # A legal or geographic region in which the structure map is intended to be used.
    # It may be possible for the structure map to be used in jurisdictions other than those for which it was originally designed or intended.
    attr_accessor :jurisdiction                   # 0-* [ CodeableConcept ]
    ##
    # Why this structure map is defined
    # Explanation of why this structure map is needed and why it has been designed as it has.
    # This element does not describe the usage of the structure map. Instead, it provides traceability of ''why'' the resource is either needed or ''why'' it is defined as it is.  This may be used to point to source materials or specifications that drove the structure of this structure map.
    attr_accessor :purpose                        # 0-1 markdown
    ##
    # Use and/or publishing restrictions
    # A copyright statement relating to the structure map and/or its contents. Copyright statements are generally legal restrictions on the use and publishing of the structure map.
    attr_accessor :copyright                      # 0-1 markdown
    ##
    # Structure Definition used by this map
    # A structure definition used by this map. The structure definition may describe instances that are converted, or the instances that are produced.
    # It is not necessary for a structure map to identify any dependent structures, though not listing them may restrict its usefulness.
    attr_accessor :structure                      # 0-* [ StructureMap::Structure ]
    ##
    # Other maps used by this map (canonical URLs).
    attr_accessor :import                         # 0-* [ canonical(http://hl7.org/fhir/StructureDefinition/StructureMap) ]
    ##
    # Named sections for reader convenience
    # Organizes the mapping into manageable chunks for human review/ease of maintenance.
    attr_accessor :group                          # 1-* [ StructureMap::Group ]

    def resourceType
      'StructureMap'
    end
  end
end
