module FHIR

  ##
  # Catalog entries are wrappers that contextualize items included in a catalog.
  class CatalogEntry < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = []
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'CatalogEntry.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'CatalogEntry.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'CatalogEntry.implicitRules',
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
        'path'=>'CatalogEntry.language',
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
        'path'=>'CatalogEntry.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'CatalogEntry.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'CatalogEntry.extension',
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
        'path'=>'CatalogEntry.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Unique identifier of the catalog item
      # Used in supporting different identifiers for the same product, e.g. manufacturer code and retailer code.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'CatalogEntry.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The type of item - medication, device, service, protocol or other.
      'type' => {
        'type'=>'CodeableConcept',
        'path'=>'CatalogEntry.type',
        'min'=>0,
        'max'=>1
      },
      ##
      # Whether the entry represents an orderable item.
      'orderable' => {
        'type'=>'boolean',
        'path'=>'CatalogEntry.orderable',
        'min'=>1,
        'max'=>1
      },
      ##
      # The item that is being defined
      # The item in a catalog or definition.
      'referencedItem' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Medication', 'http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/HealthcareService', 'http://hl7.org/fhir/StructureDefinition/ActivityDefinition', 'http://hl7.org/fhir/StructureDefinition/PlanDefinition', 'http://hl7.org/fhir/StructureDefinition/SpecimenDefinition', 'http://hl7.org/fhir/StructureDefinition/ObservationDefinition', 'http://hl7.org/fhir/StructureDefinition/Binary'],
        'type'=>'Reference',
        'path'=>'CatalogEntry.referencedItem',
        'min'=>1,
        'max'=>1
      },
      ##
      # Any additional identifier(s) for the catalog item, in the same granularity or concept
      # Used in supporting related concepts, e.g. NDC to RxNorm.
      'additionalIdentifier' => {
        'type'=>'Identifier',
        'path'=>'CatalogEntry.additionalIdentifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Classification (category or class) of the item entry
      # Classes of devices, or ATC for medication.
      'classification' => {
        'type'=>'CodeableConcept',
        'path'=>'CatalogEntry.classification',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # draft | active | retired | unknown
      # Used to support catalog exchange even for unsupported products, e.g. getting list of medications even if not prescribable.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/publication-status'=>[ 'draft', 'active', 'retired', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'CatalogEntry.status',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/publication-status'}
      },
      ##
      # The time period in which this catalog entry is expected to be active.
      'validityPeriod' => {
        'type'=>'Period',
        'path'=>'CatalogEntry.validityPeriod',
        'min'=>0,
        'max'=>1
      },
      ##
      # The date until which this catalog entry is expected to be active.
      'validTo' => {
        'type'=>'dateTime',
        'path'=>'CatalogEntry.validTo',
        'min'=>0,
        'max'=>1
      },
      ##
      # When was this catalog last updated
      # Typically date of issue is different from the beginning of the validity. This can be used to see when an item was last updated.
      # Perhaps not needed - if we use fhir resource metadata.
      'lastUpdated' => {
        'type'=>'dateTime',
        'path'=>'CatalogEntry.lastUpdated',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional characteristics of the catalog entry
      # Used for examplefor Out of Formulary, or any specifics.
      'additionalCharacteristic' => {
        'type'=>'CodeableConcept',
        'path'=>'CatalogEntry.additionalCharacteristic',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional classification of the catalog entry
      # User for example for ATC classification, or.
      'additionalClassification' => {
        'type'=>'CodeableConcept',
        'path'=>'CatalogEntry.additionalClassification',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # An item that this catalog entry is related to
      # Used for example, to point to a substance, or to a device used to administer a medication.
      'relatedEntry' => {
        'type'=>'CatalogEntry::RelatedEntry',
        'path'=>'CatalogEntry.relatedEntry',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # An item that this catalog entry is related to
    # Used for example, to point to a substance, or to a device used to administer a medication.
    class RelatedEntry < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'RelatedEntry.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'RelatedEntry.extension',
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
          'path'=>'RelatedEntry.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # triggers | is-replaced-by
        # The type of relation to the related item: child, parent, packageContent, containerPackage, usedIn, uses, requires, etc.
        'relationtype' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/relation-type'=>[ 'triggers', 'is-replaced-by' ]
          },
          'type'=>'code',
          'path'=>'RelatedEntry.relationtype',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/relation-type'}
        },
        ##
        # The reference to the related item.
        'item' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/CatalogEntry'],
          'type'=>'Reference',
          'path'=>'RelatedEntry.item',
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
      # triggers | is-replaced-by
      # The type of relation to the related item: child, parent, packageContent, containerPackage, usedIn, uses, requires, etc.
      attr_accessor :relationtype                   # 1-1 code
      ##
      # The reference to the related item.
      attr_accessor :item                           # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/CatalogEntry)
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
    # Unique identifier of the catalog item
    # Used in supporting different identifiers for the same product, e.g. manufacturer code and retailer code.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # The type of item - medication, device, service, protocol or other.
    attr_accessor :type                           # 0-1 CodeableConcept
    ##
    # Whether the entry represents an orderable item.
    attr_accessor :orderable                      # 1-1 boolean
    ##
    # The item that is being defined
    # The item in a catalog or definition.
    attr_accessor :referencedItem                 # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Medication|http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/HealthcareService|http://hl7.org/fhir/StructureDefinition/ActivityDefinition|http://hl7.org/fhir/StructureDefinition/PlanDefinition|http://hl7.org/fhir/StructureDefinition/SpecimenDefinition|http://hl7.org/fhir/StructureDefinition/ObservationDefinition|http://hl7.org/fhir/StructureDefinition/Binary)
    ##
    # Any additional identifier(s) for the catalog item, in the same granularity or concept
    # Used in supporting related concepts, e.g. NDC to RxNorm.
    attr_accessor :additionalIdentifier           # 0-* [ Identifier ]
    ##
    # Classification (category or class) of the item entry
    # Classes of devices, or ATC for medication.
    attr_accessor :classification                 # 0-* [ CodeableConcept ]
    ##
    # draft | active | retired | unknown
    # Used to support catalog exchange even for unsupported products, e.g. getting list of medications even if not prescribable.
    attr_accessor :status                         # 0-1 code
    ##
    # The time period in which this catalog entry is expected to be active.
    attr_accessor :validityPeriod                 # 0-1 Period
    ##
    # The date until which this catalog entry is expected to be active.
    attr_accessor :validTo                        # 0-1 dateTime
    ##
    # When was this catalog last updated
    # Typically date of issue is different from the beginning of the validity. This can be used to see when an item was last updated.
    # Perhaps not needed - if we use fhir resource metadata.
    attr_accessor :lastUpdated                    # 0-1 dateTime
    ##
    # Additional characteristics of the catalog entry
    # Used for examplefor Out of Formulary, or any specifics.
    attr_accessor :additionalCharacteristic       # 0-* [ CodeableConcept ]
    ##
    # Additional classification of the catalog entry
    # User for example for ATC classification, or.
    attr_accessor :additionalClassification       # 0-* [ CodeableConcept ]
    ##
    # An item that this catalog entry is related to
    # Used for example, to point to a substance, or to a device used to administer a medication.
    attr_accessor :relatedEntry                   # 0-* [ CatalogEntry::RelatedEntry ]

    def resourceType
      'CatalogEntry'
    end
  end
end
