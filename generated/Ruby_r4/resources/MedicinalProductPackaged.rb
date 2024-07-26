module FHIR

  ##
  # A medicinal product in a container or package.
  class MedicinalProductPackaged < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['identifier', 'subject']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'MedicinalProductPackaged.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'MedicinalProductPackaged.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'MedicinalProductPackaged.implicitRules',
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
        'path'=>'MedicinalProductPackaged.language',
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
        'path'=>'MedicinalProductPackaged.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'MedicinalProductPackaged.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'MedicinalProductPackaged.extension',
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
        'path'=>'MedicinalProductPackaged.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Unique identifier.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'MedicinalProductPackaged.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The product with this is a pack for.
      'subject' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/MedicinalProduct'],
        'type'=>'Reference',
        'path'=>'MedicinalProductPackaged.subject',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Textual description.
      'description' => {
        'type'=>'string',
        'path'=>'MedicinalProductPackaged.description',
        'min'=>0,
        'max'=>1
      },
      ##
      # The legal status of supply of the medicinal product as classified by the regulator.
      'legalStatusOfSupply' => {
        'type'=>'CodeableConcept',
        'path'=>'MedicinalProductPackaged.legalStatusOfSupply',
        'min'=>0,
        'max'=>1
      },
      ##
      # Marketing information.
      'marketingStatus' => {
        'type'=>'MarketingStatus',
        'path'=>'MedicinalProductPackaged.marketingStatus',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Manufacturer of this Package Item.
      'marketingAuthorization' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/MedicinalProductAuthorization'],
        'type'=>'Reference',
        'path'=>'MedicinalProductPackaged.marketingAuthorization',
        'min'=>0,
        'max'=>1
      },
      ##
      # Manufacturer of this Package Item.
      'manufacturer' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'MedicinalProductPackaged.manufacturer',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Batch numbering.
      'batchIdentifier' => {
        'type'=>'MedicinalProductPackaged::BatchIdentifier',
        'path'=>'MedicinalProductPackaged.batchIdentifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # A packaging item, as a contained for medicine, possibly with other packaging items within.
      'packageItem' => {
        'type'=>'MedicinalProductPackaged::PackageItem',
        'path'=>'MedicinalProductPackaged.packageItem',
        'min'=>1,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Batch numbering.
    class BatchIdentifier < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'BatchIdentifier.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'BatchIdentifier.extension',
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
          'path'=>'BatchIdentifier.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # A number appearing on the outer packaging of a specific batch.
        'outerPackaging' => {
          'type'=>'Identifier',
          'path'=>'BatchIdentifier.outerPackaging',
          'min'=>1,
          'max'=>1
        },
        ##
        # A number appearing on the immediate packaging (and not the outer packaging).
        'immediatePackaging' => {
          'type'=>'Identifier',
          'path'=>'BatchIdentifier.immediatePackaging',
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
      # A number appearing on the outer packaging of a specific batch.
      attr_accessor :outerPackaging                 # 1-1 Identifier
      ##
      # A number appearing on the immediate packaging (and not the outer packaging).
      attr_accessor :immediatePackaging             # 0-1 Identifier
    end

    ##
    # A packaging item, as a contained for medicine, possibly with other packaging items within.
    class PackageItem < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'PackageItem.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'PackageItem.extension',
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
          'path'=>'PackageItem.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Including possibly Data Carrier Identifier.
        'identifier' => {
          'type'=>'Identifier',
          'path'=>'PackageItem.identifier',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # The physical type of the container of the medicine.
        'type' => {
          'type'=>'CodeableConcept',
          'path'=>'PackageItem.type',
          'min'=>1,
          'max'=>1
        },
        ##
        # The quantity of this package in the medicinal product, at the current level of packaging. The outermost is always 1.
        'quantity' => {
          'type'=>'Quantity',
          'path'=>'PackageItem.quantity',
          'min'=>1,
          'max'=>1
        },
        ##
        # Material type of the package item.
        'material' => {
          'type'=>'CodeableConcept',
          'path'=>'PackageItem.material',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # A possible alternate material for the packaging.
        'alternateMaterial' => {
          'type'=>'CodeableConcept',
          'path'=>'PackageItem.alternateMaterial',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # A device accompanying a medicinal product.
        'device' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/DeviceDefinition'],
          'type'=>'Reference',
          'path'=>'PackageItem.device',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # The manufactured item as contained in the packaged medicinal product.
        'manufacturedItem' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/MedicinalProductManufactured'],
          'type'=>'Reference',
          'path'=>'PackageItem.manufacturedItem',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Allows containers within containers.
        'packageItem' => {
          'type'=>'MedicinalProductPackaged::PackageItem',
          'path'=>'PackageItem.packageItem',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Dimensions, color etc.
        'physicalCharacteristics' => {
          'type'=>'ProdCharacteristic',
          'path'=>'PackageItem.physicalCharacteristics',
          'min'=>0,
          'max'=>1
        },
        ##
        # Other codeable characteristics.
        'otherCharacteristics' => {
          'type'=>'CodeableConcept',
          'path'=>'PackageItem.otherCharacteristics',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Shelf Life and storage information.
        'shelfLifeStorage' => {
          'type'=>'ProductShelfLife',
          'path'=>'PackageItem.shelfLifeStorage',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Manufacturer of this Package Item.
        'manufacturer' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
          'type'=>'Reference',
          'path'=>'PackageItem.manufacturer',
          'min'=>0,
          'max'=>Float::INFINITY
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
      # Including possibly Data Carrier Identifier.
      attr_accessor :identifier                     # 0-* [ Identifier ]
      ##
      # The physical type of the container of the medicine.
      attr_accessor :type                           # 1-1 CodeableConcept
      ##
      # The quantity of this package in the medicinal product, at the current level of packaging. The outermost is always 1.
      attr_accessor :quantity                       # 1-1 Quantity
      ##
      # Material type of the package item.
      attr_accessor :material                       # 0-* [ CodeableConcept ]
      ##
      # A possible alternate material for the packaging.
      attr_accessor :alternateMaterial              # 0-* [ CodeableConcept ]
      ##
      # A device accompanying a medicinal product.
      attr_accessor :device                         # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/DeviceDefinition) ]
      ##
      # The manufactured item as contained in the packaged medicinal product.
      attr_accessor :manufacturedItem               # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/MedicinalProductManufactured) ]
      ##
      # Allows containers within containers.
      attr_accessor :packageItem                    # 0-* [ MedicinalProductPackaged::PackageItem ]
      ##
      # Dimensions, color etc.
      attr_accessor :physicalCharacteristics        # 0-1 ProdCharacteristic
      ##
      # Other codeable characteristics.
      attr_accessor :otherCharacteristics           # 0-* [ CodeableConcept ]
      ##
      # Shelf Life and storage information.
      attr_accessor :shelfLifeStorage               # 0-* [ ProductShelfLife ]
      ##
      # Manufacturer of this Package Item.
      attr_accessor :manufacturer                   # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Organization) ]
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
    # Unique identifier.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # The product with this is a pack for.
    attr_accessor :subject                        # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/MedicinalProduct) ]
    ##
    # Textual description.
    attr_accessor :description                    # 0-1 string
    ##
    # The legal status of supply of the medicinal product as classified by the regulator.
    attr_accessor :legalStatusOfSupply            # 0-1 CodeableConcept
    ##
    # Marketing information.
    attr_accessor :marketingStatus                # 0-* [ MarketingStatus ]
    ##
    # Manufacturer of this Package Item.
    attr_accessor :marketingAuthorization         # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/MedicinalProductAuthorization)
    ##
    # Manufacturer of this Package Item.
    attr_accessor :manufacturer                   # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Organization) ]
    ##
    # Batch numbering.
    attr_accessor :batchIdentifier                # 0-* [ MedicinalProductPackaged::BatchIdentifier ]
    ##
    # A packaging item, as a contained for medicine, possibly with other packaging items within.
    attr_accessor :packageItem                    # 1-* [ MedicinalProductPackaged::PackageItem ]

    def resourceType
      'MedicinalProductPackaged'
    end
  end
end
