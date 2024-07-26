module FHIR

  ##
  # A statement of relationships from one set of concepts to one or more other concepts - either concepts in code systems, or data element/data element concepts, or classes in class models.
  class ConceptMap < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['context-quantity', 'context-type-quantity', 'context-type-value', 'context-type', 'context', 'date', 'dependson', 'description', 'identifier', 'jurisdiction', 'name', 'other', 'product', 'publisher', 'source-code', 'source-system', 'source-uri', 'source', 'status', 'target-code', 'target-system', 'target-uri', 'target', 'title', 'url', 'version']
    MULTIPLE_TYPES = {
      'source[x]' => ['canonical', 'uri'],
      'target[x]' => ['canonical', 'uri']
    }
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'ConceptMap.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'ConceptMap.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'ConceptMap.implicitRules',
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
        'path'=>'ConceptMap.language',
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
        'path'=>'ConceptMap.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'ConceptMap.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'ConceptMap.extension',
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
        'path'=>'ConceptMap.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Canonical identifier for this concept map, represented as a URI (globally unique)
      # An absolute URI that is used to identify this concept map when it is referenced in a specification, model, design or an instance; also called its canonical identifier. This SHOULD be globally unique and SHOULD be a literal address at which at which an authoritative instance of this concept map is (or will be) published. This URL can be the target of a canonical reference. It SHALL remain the same when the concept map is stored on different servers.
      # Can be a urn:uuid: or a urn:oid: but real http: addresses are preferred.  Multiple instances may share the same URL if they have a distinct version.
      # 
      # The determination of when to create a new version of a resource (same url, new version) vs. defining a new artifact is up to the author.  Considerations for making this decision are found in [Technical and Business Versions](resource.html#versions). 
      # 
      # In some cases, the resource can no longer be found at the stated url, but the url itself cannot change. Implementations can use the [meta.source](resource.html#meta) element to indicate where the current master source of the resource can be found.
      'url' => {
        'type'=>'uri',
        'path'=>'ConceptMap.url',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional identifier for the concept map
      # A formal identifier that is used to identify this concept map when it is represented in other formats, or referenced in a specification, model, design or an instance.
      # Typically, this is used for identifiers that can go in an HL7 V3 II (instance identifier) data type, and can then identify this concept map outside of FHIR, where it is not possible to use the logical URI.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'ConceptMap.identifier',
        'min'=>0,
        'max'=>1
      },
      ##
      # Business version of the concept map
      # The identifier that is used to identify this version of the concept map when it is referenced in a specification, model, design or instance. This is an arbitrary value managed by the concept map author and is not expected to be globally unique. For example, it might be a timestamp (e.g. yyyymmdd) if a managed version is not available. There is also no expectation that versions can be placed in a lexicographical sequence.
      # There may be different concept map instances that have the same identifier but different versions.  The version can be appended to the url in a reference to allow a reference to a particular business version of the concept map with the format [url]|[version].
      'version' => {
        'type'=>'string',
        'path'=>'ConceptMap.version',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name for this concept map (computer friendly)
      # A natural language name identifying the concept map. This name should be usable as an identifier for the module by machine processing applications such as code generation.
      # The name is not expected to be globally unique. The name should be a simple alphanumeric type name to ensure that it is machine-processing friendly.
      'name' => {
        'type'=>'string',
        'path'=>'ConceptMap.name',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name for this concept map (human friendly)
      # A short, descriptive, user-friendly title for the concept map.
      # This name does not need to be machine-processing friendly and may contain punctuation, white-space, etc.
      'title' => {
        'type'=>'string',
        'path'=>'ConceptMap.title',
        'min'=>0,
        'max'=>1
      },
      ##
      # draft | active | retired | unknown
      # The status of this concept map. Enables tracking the life-cycle of the content.
      # Allows filtering of concept maps that are appropriate for use versus not.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/publication-status'=>[ 'draft', 'active', 'retired', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'ConceptMap.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/publication-status'}
      },
      ##
      # For testing purposes, not real usage
      # A Boolean value to indicate that this concept map is authored for testing purposes (or education/evaluation/marketing) and is not intended to be used for genuine usage.
      # Allows filtering of concept maps that are appropriate for use versus not.
      'experimental' => {
        'type'=>'boolean',
        'path'=>'ConceptMap.experimental',
        'min'=>0,
        'max'=>1
      },
      ##
      # Date last changed
      # The date  (and optionally time) when the concept map was published. The date must change when the business version changes and it must change if the status code changes. In addition, it should change when the substantive content of the concept map changes.
      # Note that this is not the same as the resource last-modified-date, since the resource may be a secondary representation of the concept map. Additional specific dates may be added as extensions or be found by consulting Provenances associated with past versions of the resource.
      'date' => {
        'type'=>'dateTime',
        'path'=>'ConceptMap.date',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name of the publisher (organization or individual)
      # The name of the organization or individual that published the concept map.
      # Usually an organization but may be an individual. The publisher (or steward) of the concept map is the organization or individual primarily responsible for the maintenance and upkeep of the concept map. This is not necessarily the same individual or organization that developed and initially authored the content. The publisher is the primary point of contact for questions or issues with the concept map. This item SHOULD be populated unless the information is available from context.
      'publisher' => {
        'type'=>'string',
        'path'=>'ConceptMap.publisher',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contact details for the publisher
      # Contact details to assist a user in finding and communicating with the publisher.
      # May be a web site, an email address, a telephone number, etc.
      'contact' => {
        'type'=>'ContactDetail',
        'path'=>'ConceptMap.contact',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Natural language description of the concept map
      # A free text natural language description of the concept map from a consumer's perspective.
      # The description is not intended to describe the semantics of the concept map. The description should capture its intended use, which is needed for ensuring integrity for its use in models across future changes.
      'description' => {
        'type'=>'markdown',
        'path'=>'ConceptMap.description',
        'min'=>0,
        'max'=>1
      },
      ##
      # The context that the content is intended to support
      # The content was developed with a focus and intent of supporting the contexts that are listed. These contexts may be general categories (gender, age, ...) or may be references to specific programs (insurance plans, studies, ...) and may be used to assist with indexing and searching for appropriate concept map instances.
      # When multiple useContexts are specified, there is no expectation that all or any of the contexts apply.
      'useContext' => {
        'type'=>'UsageContext',
        'path'=>'ConceptMap.useContext',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Intended jurisdiction for concept map (if applicable)
      # A legal or geographic region in which the concept map is intended to be used.
      # It may be possible for the concept map to be used in jurisdictions other than those for which it was originally designed or intended.
      'jurisdiction' => {
        'type'=>'CodeableConcept',
        'path'=>'ConceptMap.jurisdiction',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Why this concept map is defined
      # Explanation of why this concept map is needed and why it has been designed as it has.
      # This element does not describe the usage of the concept map. Instead, it provides traceability of ''why'' the resource is either needed or ''why'' it is defined as it is.  This may be used to point to source materials or specifications that drove the structure of this concept map.
      'purpose' => {
        'type'=>'markdown',
        'path'=>'ConceptMap.purpose',
        'min'=>0,
        'max'=>1
      },
      ##
      # Use and/or publishing restrictions
      # A copyright statement relating to the concept map and/or its contents. Copyright statements are generally legal restrictions on the use and publishing of the concept map.
      # Frequently the copyright differs between the concept map and codes that are included. The copyright statement should clearly differentiate between these when required.
      'copyright' => {
        'type'=>'markdown',
        'path'=>'ConceptMap.copyright',
        'min'=>0,
        'max'=>1
      },
      ##
      # The source value set that contains the concepts that are being mapped
      # Identifier for the source value set that contains the concepts that are being mapped and provides context for the mappings.
      # Should be a version specific reference. URIs SHOULD be absolute. If there is no source or target value set, there is no specified context for the map (not recommended).  The source value set may select codes from either an explicit (standard or local) or implicit code system.
      'sourceCanonical' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ValueSet'],
        'type'=>'Canonical',
        'path'=>'ConceptMap.source[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # The source value set that contains the concepts that are being mapped
      # Identifier for the source value set that contains the concepts that are being mapped and provides context for the mappings.
      # Should be a version specific reference. URIs SHOULD be absolute. If there is no source or target value set, there is no specified context for the map (not recommended).  The source value set may select codes from either an explicit (standard or local) or implicit code system.
      'sourceUri' => {
        'type'=>'Uri',
        'path'=>'ConceptMap.source[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # The target value set which provides context for the mappings
      # The target value set provides context for the mappings. Note that the mapping is made between concepts, not between value sets, but the value set provides important context about how the concept mapping choices are made.
      # Should be a version specific reference. URIs SHOULD be absolute. If there is no source or target value set, the is no specified context for the map.
      'targetCanonical' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ValueSet'],
        'type'=>'Canonical',
        'path'=>'ConceptMap.target[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # The target value set which provides context for the mappings
      # The target value set provides context for the mappings. Note that the mapping is made between concepts, not between value sets, but the value set provides important context about how the concept mapping choices are made.
      # Should be a version specific reference. URIs SHOULD be absolute. If there is no source or target value set, the is no specified context for the map.
      'targetUri' => {
        'type'=>'Uri',
        'path'=>'ConceptMap.target[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Same source and target systems
      # A group of mappings that all have the same source and target system.
      'group' => {
        'type'=>'ConceptMap::Group',
        'path'=>'ConceptMap.group',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Same source and target systems
    # A group of mappings that all have the same source and target system.
    class Group < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
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
        # Source system where concepts to be mapped are defined
        # An absolute URI that identifies the source system where the concepts to be mapped are defined.
        # This is not needed if the source value set is specified and it contains concepts from only a single system.
        'source' => {
          'type'=>'uri',
          'path'=>'Group.source',
          'min'=>0,
          'max'=>1
        },
        ##
        # Specific version of the  code system
        # The specific version of the code system, as determined by the code system authority.
        # The specification of a particular code system version may be required for code systems which lack concept permanence.
        'sourceVersion' => {
          'type'=>'string',
          'path'=>'Group.sourceVersion',
          'min'=>0,
          'max'=>1
        },
        ##
        # Target system that the concepts are to be mapped to
        # An absolute URI that identifies the target system that the concepts will be mapped to.
        # This is not needed if the target value set is specified and it contains concepts from only a single system. The group target may also be omitted if all of the target element equivalence values are 'unmatched'.
        'target' => {
          'type'=>'uri',
          'path'=>'Group.target',
          'min'=>0,
          'max'=>1
        },
        ##
        # Specific version of the  code system
        # The specific version of the code system, as determined by the code system authority.
        # The specification of a particular code system version may be required for code systems which lack concept permanence.
        'targetVersion' => {
          'type'=>'string',
          'path'=>'Group.targetVersion',
          'min'=>0,
          'max'=>1
        },
        ##
        # Mappings for a concept from the source set
        # Mappings for an individual concept in the source to one or more concepts in the target.
        # Generally, the ideal is that there would only be one mapping for each concept in the source value set, but a given concept may be mapped multiple times with different comments or dependencies.
        'element' => {
          'type'=>'ConceptMap::Group::Element',
          'path'=>'Group.element',
          'min'=>1,
          'max'=>Float::INFINITY
        },
        ##
        # What to do when there is no mapping for the source concept. "Unmapped" does not include codes that are unmatched, and the unmapped element is ignored in a code is specified to have equivalence = unmatched.
        # This only applies if the source code has a system value that matches the system defined for the group.
        'unmapped' => {
          'type'=>'ConceptMap::Group::Unmapped',
          'path'=>'Group.unmapped',
          'min'=>0,
          'max'=>1
        }
      }

      ##
      # Mappings for a concept from the source set
      # Mappings for an individual concept in the source to one or more concepts in the target.
      # Generally, the ideal is that there would only be one mapping for each concept in the source value set, but a given concept may be mapped multiple times with different comments or dependencies.
      class Element < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'id',
            'path'=>'Element.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Element.extension',
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
            'path'=>'Element.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Identifies element being mapped
          # Identity (code or path) or the element/item being mapped.
          'code' => {
            'type'=>'code',
            'path'=>'Element.code',
            'min'=>0,
            'max'=>1
          },
          ##
          # Display for the code
          # The display for the code. The display is only provided to help editors when editing the concept map.
          # The display is ignored when processing the map.
          'display' => {
            'type'=>'string',
            'path'=>'Element.display',
            'min'=>0,
            'max'=>1
          },
          ##
          # Concept in target system for element
          # A concept from the target value set that this concept maps to.
          # Ideally there would only be one map, with equal or equivalent mapping. But multiple maps are allowed for several narrower options, or to assert that other concepts are unmatched.
          'target' => {
            'type'=>'ConceptMap::Group::Element::Target',
            'path'=>'Element.target',
            'min'=>0,
            'max'=>Float::INFINITY
          }
        }

        ##
        # Concept in target system for element
        # A concept from the target value set that this concept maps to.
        # Ideally there would only be one map, with equal or equivalent mapping. But multiple maps are allowed for several narrower options, or to assert that other concepts are unmatched.
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
            # Code that identifies the target element
            # Identity (code or path) or the element/item that the map refers to.
            'code' => {
              'type'=>'code',
              'path'=>'Target.code',
              'min'=>0,
              'max'=>1
            },
            ##
            # Display for the code
            # The display for the code. The display is only provided to help editors when editing the concept map.
            # The display is ignored when processing the map.
            'display' => {
              'type'=>'string',
              'path'=>'Target.display',
              'min'=>0,
              'max'=>1
            },
            ##
            # relatedto | equivalent | equal | wider | subsumes | narrower | specializes | inexact | unmatched | disjoint
            # The equivalence between the source and target concepts (counting for the dependencies and products). The equivalence is read from target to source (e.g. the target is 'wider' than the source).
            # This element is labeled as a modifier because it may indicate that a target does not apply.
            'equivalence' => {
              'valid_codes'=>{
                'http://hl7.org/fhir/concept-map-equivalence'=>[ 'relatedto', 'equivalent', 'equal', 'wider', 'subsumes', 'narrower', 'specializes', 'inexact', 'unmatched', 'disjoint' ]
              },
              'type'=>'code',
              'path'=>'Target.equivalence',
              'min'=>1,
              'max'=>1,
              'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/concept-map-equivalence'}
            },
            ##
            # Description of status/issues in mapping
            # A description of status/issues in mapping that conveys additional information not represented in  the structured data.
            'comment' => {
              'type'=>'string',
              'path'=>'Target.comment',
              'min'=>0,
              'max'=>1
            },
            ##
            # Other elements required for this mapping (from context)
            # A set of additional dependencies for this mapping to hold. This mapping is only applicable if the specified element can be resolved, and it has the specified value.
            'dependsOn' => {
              'type'=>'ConceptMap::Group::Element::Target::DependsOn',
              'path'=>'Target.dependsOn',
              'min'=>0,
              'max'=>Float::INFINITY
            },
            ##
            # Other concepts that this mapping also produces
            # A set of additional outcomes from this mapping to other elements. To properly execute this mapping, the specified element must be mapped to some data element or source that is in context. The mapping may still be useful without a place for the additional data elements, but the equivalence cannot be relied on.
            'product' => {
              'type'=>'ConceptMap::Group::Element::Target::DependsOn',
              'path'=>'Target.product',
              'min'=>0,
              'max'=>Float::INFINITY
            }
          }

          ##
          # Other elements required for this mapping (from context)
          # A set of additional dependencies for this mapping to hold. This mapping is only applicable if the specified element can be resolved, and it has the specified value.
          class DependsOn < FHIR::Model
            include FHIR::Hashable
            include FHIR::Json
            include FHIR::Xml

            METADATA = {
              ##
              # Unique id for inter-element referencing
              # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
              'id' => {
                'type'=>'id',
                'path'=>'DependsOn.id',
                'min'=>0,
                'max'=>1
              },
              ##
              # Additional content defined by implementations
              # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
              # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
              'extension' => {
                'type'=>'Extension',
                'path'=>'DependsOn.extension',
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
                'path'=>'DependsOn.modifierExtension',
                'min'=>0,
                'max'=>Float::INFINITY
              },
              ##
              # Reference to property mapping depends on
              # A reference to an element that holds a coded value that corresponds to a code system property. The idea is that the information model carries an element somewhere that is labeled to correspond with a code system property.
              'property' => {
                'type'=>'uri',
                'path'=>'DependsOn.property',
                'min'=>1,
                'max'=>1
              },
              ##
              # Code System (if necessary)
              # An absolute URI that identifies the code system of the dependency code (if the source/dependency is a value set that crosses code systems).
              'system' => {
                'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/CodeSystem'],
                'type'=>'canonical',
                'path'=>'DependsOn.system',
                'min'=>0,
                'max'=>1
              },
              ##
              # Value of the referenced element
              # Identity (code or path) or the element/item/ValueSet/text that the map depends on / refers to.
              'value' => {
                'type'=>'string',
                'path'=>'DependsOn.value',
                'min'=>1,
                'max'=>1
              },
              ##
              # Display for the code (if value is a code)
              # The display for the code. The display is only provided to help editors when editing the concept map.
              # The display is ignored when processing the map.
              'display' => {
                'type'=>'string',
                'path'=>'DependsOn.display',
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
            # Reference to property mapping depends on
            # A reference to an element that holds a coded value that corresponds to a code system property. The idea is that the information model carries an element somewhere that is labeled to correspond with a code system property.
            attr_accessor :property                       # 1-1 uri
            ##
            # Code System (if necessary)
            # An absolute URI that identifies the code system of the dependency code (if the source/dependency is a value set that crosses code systems).
            attr_accessor :system                         # 0-1 canonical(http://hl7.org/fhir/StructureDefinition/CodeSystem)
            ##
            # Value of the referenced element
            # Identity (code or path) or the element/item/ValueSet/text that the map depends on / refers to.
            attr_accessor :value                          # 1-1 string
            ##
            # Display for the code (if value is a code)
            # The display for the code. The display is only provided to help editors when editing the concept map.
            # The display is ignored when processing the map.
            attr_accessor :display                        # 0-1 string
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
          # Code that identifies the target element
          # Identity (code or path) or the element/item that the map refers to.
          attr_accessor :code                           # 0-1 code
          ##
          # Display for the code
          # The display for the code. The display is only provided to help editors when editing the concept map.
          # The display is ignored when processing the map.
          attr_accessor :display                        # 0-1 string
          ##
          # relatedto | equivalent | equal | wider | subsumes | narrower | specializes | inexact | unmatched | disjoint
          # The equivalence between the source and target concepts (counting for the dependencies and products). The equivalence is read from target to source (e.g. the target is 'wider' than the source).
          # This element is labeled as a modifier because it may indicate that a target does not apply.
          attr_accessor :equivalence                    # 1-1 code
          ##
          # Description of status/issues in mapping
          # A description of status/issues in mapping that conveys additional information not represented in  the structured data.
          attr_accessor :comment                        # 0-1 string
          ##
          # Other elements required for this mapping (from context)
          # A set of additional dependencies for this mapping to hold. This mapping is only applicable if the specified element can be resolved, and it has the specified value.
          attr_accessor :dependsOn                      # 0-* [ ConceptMap::Group::Element::Target::DependsOn ]
          ##
          # Other concepts that this mapping also produces
          # A set of additional outcomes from this mapping to other elements. To properly execute this mapping, the specified element must be mapped to some data element or source that is in context. The mapping may still be useful without a place for the additional data elements, but the equivalence cannot be relied on.
          attr_accessor :product                        # 0-* [ ConceptMap::Group::Element::Target::DependsOn ]
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
        # Identifies element being mapped
        # Identity (code or path) or the element/item being mapped.
        attr_accessor :code                           # 0-1 code
        ##
        # Display for the code
        # The display for the code. The display is only provided to help editors when editing the concept map.
        # The display is ignored when processing the map.
        attr_accessor :display                        # 0-1 string
        ##
        # Concept in target system for element
        # A concept from the target value set that this concept maps to.
        # Ideally there would only be one map, with equal or equivalent mapping. But multiple maps are allowed for several narrower options, or to assert that other concepts are unmatched.
        attr_accessor :target                         # 0-* [ ConceptMap::Group::Element::Target ]
      end

      ##
      # What to do when there is no mapping for the source concept. "Unmapped" does not include codes that are unmatched, and the unmapped element is ignored in a code is specified to have equivalence = unmatched.
      # This only applies if the source code has a system value that matches the system defined for the group.
      class Unmapped < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'id',
            'path'=>'Unmapped.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Unmapped.extension',
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
            'path'=>'Unmapped.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # provided | fixed | other-map
          # Defines which action to take if there is no match for the source concept in the target system designated for the group. One of 3 actions are possible: use the unmapped code (this is useful when doing a mapping between versions, and only a few codes have changed), use a fixed code (a default code), or alternatively, a reference to a different concept map can be provided (by canonical URL).
          'mode' => {
            'valid_codes'=>{
              'http://hl7.org/fhir/conceptmap-unmapped-mode'=>[ 'provided', 'fixed', 'other-map' ]
            },
            'type'=>'code',
            'path'=>'Unmapped.mode',
            'min'=>1,
            'max'=>1,
            'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/conceptmap-unmapped-mode'}
          },
          ##
          # Fixed code when mode = fixed
          # The fixed code to use when the mode = 'fixed'  - all unmapped codes are mapped to a single fixed code.
          'code' => {
            'type'=>'code',
            'path'=>'Unmapped.code',
            'min'=>0,
            'max'=>1
          },
          ##
          # Display for the code
          # The display for the code. The display is only provided to help editors when editing the concept map.
          # The display is ignored when processing the map.
          'display' => {
            'type'=>'string',
            'path'=>'Unmapped.display',
            'min'=>0,
            'max'=>1
          },
          ##
          # canonical reference to an additional ConceptMap to use for mapping if the source concept is unmapped
          # The canonical reference to an additional ConceptMap resource instance to use for mapping if this ConceptMap resource contains no matching mapping for the source concept.
          'url' => {
            'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ConceptMap'],
            'type'=>'canonical',
            'path'=>'Unmapped.url',
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
        # provided | fixed | other-map
        # Defines which action to take if there is no match for the source concept in the target system designated for the group. One of 3 actions are possible: use the unmapped code (this is useful when doing a mapping between versions, and only a few codes have changed), use a fixed code (a default code), or alternatively, a reference to a different concept map can be provided (by canonical URL).
        attr_accessor :mode                           # 1-1 code
        ##
        # Fixed code when mode = fixed
        # The fixed code to use when the mode = 'fixed'  - all unmapped codes are mapped to a single fixed code.
        attr_accessor :code                           # 0-1 code
        ##
        # Display for the code
        # The display for the code. The display is only provided to help editors when editing the concept map.
        # The display is ignored when processing the map.
        attr_accessor :display                        # 0-1 string
        ##
        # canonical reference to an additional ConceptMap to use for mapping if the source concept is unmapped
        # The canonical reference to an additional ConceptMap resource instance to use for mapping if this ConceptMap resource contains no matching mapping for the source concept.
        attr_accessor :url                            # 0-1 canonical(http://hl7.org/fhir/StructureDefinition/ConceptMap)
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
      # Source system where concepts to be mapped are defined
      # An absolute URI that identifies the source system where the concepts to be mapped are defined.
      # This is not needed if the source value set is specified and it contains concepts from only a single system.
      attr_accessor :source                         # 0-1 uri
      ##
      # Specific version of the  code system
      # The specific version of the code system, as determined by the code system authority.
      # The specification of a particular code system version may be required for code systems which lack concept permanence.
      attr_accessor :sourceVersion                  # 0-1 string
      ##
      # Target system that the concepts are to be mapped to
      # An absolute URI that identifies the target system that the concepts will be mapped to.
      # This is not needed if the target value set is specified and it contains concepts from only a single system. The group target may also be omitted if all of the target element equivalence values are 'unmatched'.
      attr_accessor :target                         # 0-1 uri
      ##
      # Specific version of the  code system
      # The specific version of the code system, as determined by the code system authority.
      # The specification of a particular code system version may be required for code systems which lack concept permanence.
      attr_accessor :targetVersion                  # 0-1 string
      ##
      # Mappings for a concept from the source set
      # Mappings for an individual concept in the source to one or more concepts in the target.
      # Generally, the ideal is that there would only be one mapping for each concept in the source value set, but a given concept may be mapped multiple times with different comments or dependencies.
      attr_accessor :element                        # 1-* [ ConceptMap::Group::Element ]
      ##
      # What to do when there is no mapping for the source concept. "Unmapped" does not include codes that are unmatched, and the unmapped element is ignored in a code is specified to have equivalence = unmatched.
      # This only applies if the source code has a system value that matches the system defined for the group.
      attr_accessor :unmapped                       # 0-1 ConceptMap::Group::Unmapped
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
    # Canonical identifier for this concept map, represented as a URI (globally unique)
    # An absolute URI that is used to identify this concept map when it is referenced in a specification, model, design or an instance; also called its canonical identifier. This SHOULD be globally unique and SHOULD be a literal address at which at which an authoritative instance of this concept map is (or will be) published. This URL can be the target of a canonical reference. It SHALL remain the same when the concept map is stored on different servers.
    # Can be a urn:uuid: or a urn:oid: but real http: addresses are preferred.  Multiple instances may share the same URL if they have a distinct version.
    # 
    # The determination of when to create a new version of a resource (same url, new version) vs. defining a new artifact is up to the author.  Considerations for making this decision are found in [Technical and Business Versions](resource.html#versions). 
    # 
    # In some cases, the resource can no longer be found at the stated url, but the url itself cannot change. Implementations can use the [meta.source](resource.html#meta) element to indicate where the current master source of the resource can be found.
    attr_accessor :url                            # 0-1 uri
    ##
    # Additional identifier for the concept map
    # A formal identifier that is used to identify this concept map when it is represented in other formats, or referenced in a specification, model, design or an instance.
    # Typically, this is used for identifiers that can go in an HL7 V3 II (instance identifier) data type, and can then identify this concept map outside of FHIR, where it is not possible to use the logical URI.
    attr_accessor :identifier                     # 0-1 Identifier
    ##
    # Business version of the concept map
    # The identifier that is used to identify this version of the concept map when it is referenced in a specification, model, design or instance. This is an arbitrary value managed by the concept map author and is not expected to be globally unique. For example, it might be a timestamp (e.g. yyyymmdd) if a managed version is not available. There is also no expectation that versions can be placed in a lexicographical sequence.
    # There may be different concept map instances that have the same identifier but different versions.  The version can be appended to the url in a reference to allow a reference to a particular business version of the concept map with the format [url]|[version].
    attr_accessor :version                        # 0-1 string
    ##
    # Name for this concept map (computer friendly)
    # A natural language name identifying the concept map. This name should be usable as an identifier for the module by machine processing applications such as code generation.
    # The name is not expected to be globally unique. The name should be a simple alphanumeric type name to ensure that it is machine-processing friendly.
    attr_accessor :name                           # 0-1 string
    ##
    # Name for this concept map (human friendly)
    # A short, descriptive, user-friendly title for the concept map.
    # This name does not need to be machine-processing friendly and may contain punctuation, white-space, etc.
    attr_accessor :title                          # 0-1 string
    ##
    # draft | active | retired | unknown
    # The status of this concept map. Enables tracking the life-cycle of the content.
    # Allows filtering of concept maps that are appropriate for use versus not.
    attr_accessor :status                         # 1-1 code
    ##
    # For testing purposes, not real usage
    # A Boolean value to indicate that this concept map is authored for testing purposes (or education/evaluation/marketing) and is not intended to be used for genuine usage.
    # Allows filtering of concept maps that are appropriate for use versus not.
    attr_accessor :experimental                   # 0-1 boolean
    ##
    # Date last changed
    # The date  (and optionally time) when the concept map was published. The date must change when the business version changes and it must change if the status code changes. In addition, it should change when the substantive content of the concept map changes.
    # Note that this is not the same as the resource last-modified-date, since the resource may be a secondary representation of the concept map. Additional specific dates may be added as extensions or be found by consulting Provenances associated with past versions of the resource.
    attr_accessor :date                           # 0-1 dateTime
    ##
    # Name of the publisher (organization or individual)
    # The name of the organization or individual that published the concept map.
    # Usually an organization but may be an individual. The publisher (or steward) of the concept map is the organization or individual primarily responsible for the maintenance and upkeep of the concept map. This is not necessarily the same individual or organization that developed and initially authored the content. The publisher is the primary point of contact for questions or issues with the concept map. This item SHOULD be populated unless the information is available from context.
    attr_accessor :publisher                      # 0-1 string
    ##
    # Contact details for the publisher
    # Contact details to assist a user in finding and communicating with the publisher.
    # May be a web site, an email address, a telephone number, etc.
    attr_accessor :contact                        # 0-* [ ContactDetail ]
    ##
    # Natural language description of the concept map
    # A free text natural language description of the concept map from a consumer's perspective.
    # The description is not intended to describe the semantics of the concept map. The description should capture its intended use, which is needed for ensuring integrity for its use in models across future changes.
    attr_accessor :description                    # 0-1 markdown
    ##
    # The context that the content is intended to support
    # The content was developed with a focus and intent of supporting the contexts that are listed. These contexts may be general categories (gender, age, ...) or may be references to specific programs (insurance plans, studies, ...) and may be used to assist with indexing and searching for appropriate concept map instances.
    # When multiple useContexts are specified, there is no expectation that all or any of the contexts apply.
    attr_accessor :useContext                     # 0-* [ UsageContext ]
    ##
    # Intended jurisdiction for concept map (if applicable)
    # A legal or geographic region in which the concept map is intended to be used.
    # It may be possible for the concept map to be used in jurisdictions other than those for which it was originally designed or intended.
    attr_accessor :jurisdiction                   # 0-* [ CodeableConcept ]
    ##
    # Why this concept map is defined
    # Explanation of why this concept map is needed and why it has been designed as it has.
    # This element does not describe the usage of the concept map. Instead, it provides traceability of ''why'' the resource is either needed or ''why'' it is defined as it is.  This may be used to point to source materials or specifications that drove the structure of this concept map.
    attr_accessor :purpose                        # 0-1 markdown
    ##
    # Use and/or publishing restrictions
    # A copyright statement relating to the concept map and/or its contents. Copyright statements are generally legal restrictions on the use and publishing of the concept map.
    # Frequently the copyright differs between the concept map and codes that are included. The copyright statement should clearly differentiate between these when required.
    attr_accessor :copyright                      # 0-1 markdown
    ##
    # The source value set that contains the concepts that are being mapped
    # Identifier for the source value set that contains the concepts that are being mapped and provides context for the mappings.
    # Should be a version specific reference. URIs SHOULD be absolute. If there is no source or target value set, there is no specified context for the map (not recommended).  The source value set may select codes from either an explicit (standard or local) or implicit code system.
    attr_accessor :sourceCanonical                # 0-1 Canonical(http://hl7.org/fhir/StructureDefinition/ValueSet)
    ##
    # The source value set that contains the concepts that are being mapped
    # Identifier for the source value set that contains the concepts that are being mapped and provides context for the mappings.
    # Should be a version specific reference. URIs SHOULD be absolute. If there is no source or target value set, there is no specified context for the map (not recommended).  The source value set may select codes from either an explicit (standard or local) or implicit code system.
    attr_accessor :sourceUri                      # 0-1 Uri
    ##
    # The target value set which provides context for the mappings
    # The target value set provides context for the mappings. Note that the mapping is made between concepts, not between value sets, but the value set provides important context about how the concept mapping choices are made.
    # Should be a version specific reference. URIs SHOULD be absolute. If there is no source or target value set, the is no specified context for the map.
    attr_accessor :targetCanonical                # 0-1 Canonical(http://hl7.org/fhir/StructureDefinition/ValueSet)
    ##
    # The target value set which provides context for the mappings
    # The target value set provides context for the mappings. Note that the mapping is made between concepts, not between value sets, but the value set provides important context about how the concept mapping choices are made.
    # Should be a version specific reference. URIs SHOULD be absolute. If there is no source or target value set, the is no specified context for the map.
    attr_accessor :targetUri                      # 0-1 Uri
    ##
    # Same source and target systems
    # A group of mappings that all have the same source and target system.
    attr_accessor :group                          # 0-* [ ConceptMap::Group ]

    def resourceType
      'ConceptMap'
    end
  end
end
