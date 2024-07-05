module FHIR

  ##
  # Detailed definition of a medicinal product, typically for uses other than direct patient care (e.g. regulatory use).
  class MedicinalProduct < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['identifier', 'name-language', 'name']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'MedicinalProduct.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'MedicinalProduct.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'MedicinalProduct.implicitRules',
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
        'path'=>'MedicinalProduct.language',
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
        'path'=>'MedicinalProduct.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'MedicinalProduct.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'MedicinalProduct.extension',
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
        'path'=>'MedicinalProduct.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Business identifier for this product. Could be an MPID.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'MedicinalProduct.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Regulatory type, e.g. Investigational or Authorized.
      'type' => {
        'type'=>'CodeableConcept',
        'path'=>'MedicinalProduct.type',
        'min'=>0,
        'max'=>1
      },
      ##
      # If this medicine applies to human or veterinary uses.
      'domain' => {
        'type'=>'Coding',
        'path'=>'MedicinalProduct.domain',
        'min'=>0,
        'max'=>1
      },
      ##
      # The dose form for a single part product, or combined form of a multiple part product.
      'combinedPharmaceuticalDoseForm' => {
        'type'=>'CodeableConcept',
        'path'=>'MedicinalProduct.combinedPharmaceuticalDoseForm',
        'min'=>0,
        'max'=>1
      },
      ##
      # The legal status of supply of the medicinal product as classified by the regulator.
      'legalStatusOfSupply' => {
        'type'=>'CodeableConcept',
        'path'=>'MedicinalProduct.legalStatusOfSupply',
        'min'=>0,
        'max'=>1
      },
      ##
      # Whether the Medicinal Product is subject to additional monitoring for regulatory reasons.
      'additionalMonitoringIndicator' => {
        'type'=>'CodeableConcept',
        'path'=>'MedicinalProduct.additionalMonitoringIndicator',
        'min'=>0,
        'max'=>1
      },
      ##
      # Whether the Medicinal Product is subject to special measures for regulatory reasons.
      'specialMeasures' => {
        'type'=>'string',
        'path'=>'MedicinalProduct.specialMeasures',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # If authorised for use in children.
      'paediatricUseIndicator' => {
        'type'=>'CodeableConcept',
        'path'=>'MedicinalProduct.paediatricUseIndicator',
        'min'=>0,
        'max'=>1
      },
      ##
      # Allows the product to be classified by various systems.
      'productClassification' => {
        'type'=>'CodeableConcept',
        'path'=>'MedicinalProduct.productClassification',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Marketing status of the medicinal product, in contrast to marketing authorizaton.
      'marketingStatus' => {
        'type'=>'MarketingStatus',
        'path'=>'MedicinalProduct.marketingStatus',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Pharmaceutical aspects of product.
      'pharmaceuticalProduct' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/MedicinalProductPharmaceutical'],
        'type'=>'Reference',
        'path'=>'MedicinalProduct.pharmaceuticalProduct',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Package representation for the product.
      'packagedMedicinalProduct' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/MedicinalProductPackaged'],
        'type'=>'Reference',
        'path'=>'MedicinalProduct.packagedMedicinalProduct',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Supporting documentation, typically for regulatory submission.
      'attachedDocument' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/DocumentReference'],
        'type'=>'Reference',
        'path'=>'MedicinalProduct.attachedDocument',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # A master file for to the medicinal product (e.g. Pharmacovigilance System Master File).
      'masterFile' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/DocumentReference'],
        'type'=>'Reference',
        'path'=>'MedicinalProduct.masterFile',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # A product specific contact, person (in a role), or an organization.
      'contact' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole'],
        'type'=>'Reference',
        'path'=>'MedicinalProduct.contact',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Clinical trials or studies that this product is involved in.
      'clinicalTrial' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ResearchStudy'],
        'type'=>'Reference',
        'path'=>'MedicinalProduct.clinicalTrial',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The product's name, including full name and possibly coded parts.
      'name' => {
        'type'=>'MedicinalProduct::Name',
        'path'=>'MedicinalProduct.name',
        'min'=>1,
        'max'=>Float::INFINITY
      },
      ##
      # Reference to another product, e.g. for linking authorised to investigational product.
      'crossReference' => {
        'type'=>'Identifier',
        'path'=>'MedicinalProduct.crossReference',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # An operation applied to the product, for manufacturing or adminsitrative purpose.
      'manufacturingBusinessOperation' => {
        'type'=>'MedicinalProduct::ManufacturingBusinessOperation',
        'path'=>'MedicinalProduct.manufacturingBusinessOperation',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Indicates if the medicinal product has an orphan designation for the treatment of a rare disease.
      'specialDesignation' => {
        'type'=>'MedicinalProduct::SpecialDesignation',
        'path'=>'MedicinalProduct.specialDesignation',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # The product's name, including full name and possibly coded parts.
    class Name < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Name.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Name.extension',
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
          'path'=>'Name.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # The full product name.
        'productName' => {
          'type'=>'string',
          'path'=>'Name.productName',
          'min'=>1,
          'max'=>1
        },
        ##
        # Coding words or phrases of the name.
        'namePart' => {
          'type'=>'MedicinalProduct::Name::NamePart',
          'path'=>'Name.namePart',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Country where the name applies.
        'countryLanguage' => {
          'type'=>'MedicinalProduct::Name::CountryLanguage',
          'path'=>'Name.countryLanguage',
          'min'=>0,
          'max'=>Float::INFINITY
        }
      }

      ##
      # Coding words or phrases of the name.
      class NamePart < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
            'path'=>'NamePart.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'NamePart.extension',
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
            'path'=>'NamePart.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # A fragment of a product name.
          'part' => {
            'type'=>'string',
            'path'=>'NamePart.part',
            'min'=>1,
            'max'=>1
          },
          ##
          # Idenifying type for this part of the name (e.g. strength part).
          'type' => {
            'type'=>'Coding',
            'path'=>'NamePart.type',
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
        # A fragment of a product name.
        attr_accessor :part                           # 1-1 string
        ##
        # Idenifying type for this part of the name (e.g. strength part).
        attr_accessor :type                           # 1-1 Coding
      end

      ##
      # Country where the name applies.
      class CountryLanguage < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
            'path'=>'CountryLanguage.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'CountryLanguage.extension',
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
            'path'=>'CountryLanguage.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Country code for where this name applies.
          'country' => {
            'type'=>'CodeableConcept',
            'path'=>'CountryLanguage.country',
            'min'=>1,
            'max'=>1
          },
          ##
          # Jurisdiction code for where this name applies.
          'jurisdiction' => {
            'type'=>'CodeableConcept',
            'path'=>'CountryLanguage.jurisdiction',
            'min'=>0,
            'max'=>1
          },
          ##
          # Language code for this name.
          'language' => {
            'type'=>'CodeableConcept',
            'path'=>'CountryLanguage.language',
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
        # Country code for where this name applies.
        attr_accessor :country                        # 1-1 CodeableConcept
        ##
        # Jurisdiction code for where this name applies.
        attr_accessor :jurisdiction                   # 0-1 CodeableConcept
        ##
        # Language code for this name.
        attr_accessor :language                       # 1-1 CodeableConcept
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
      # The full product name.
      attr_accessor :productName                    # 1-1 string
      ##
      # Coding words or phrases of the name.
      attr_accessor :namePart                       # 0-* [ MedicinalProduct::Name::NamePart ]
      ##
      # Country where the name applies.
      attr_accessor :countryLanguage                # 0-* [ MedicinalProduct::Name::CountryLanguage ]
    end

    ##
    # An operation applied to the product, for manufacturing or adminsitrative purpose.
    class ManufacturingBusinessOperation < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'ManufacturingBusinessOperation.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'ManufacturingBusinessOperation.extension',
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
          'path'=>'ManufacturingBusinessOperation.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # The type of manufacturing operation.
        'operationType' => {
          'type'=>'CodeableConcept',
          'path'=>'ManufacturingBusinessOperation.operationType',
          'min'=>0,
          'max'=>1
        },
        ##
        # Regulatory authorization reference number.
        'authorisationReferenceNumber' => {
          'type'=>'Identifier',
          'path'=>'ManufacturingBusinessOperation.authorisationReferenceNumber',
          'min'=>0,
          'max'=>1
        },
        ##
        # Regulatory authorization date.
        'effectiveDate' => {
          'type'=>'dateTime',
          'path'=>'ManufacturingBusinessOperation.effectiveDate',
          'min'=>0,
          'max'=>1
        },
        ##
        # To indicate if this proces is commercially confidential.
        'confidentialityIndicator' => {
          'type'=>'CodeableConcept',
          'path'=>'ManufacturingBusinessOperation.confidentialityIndicator',
          'min'=>0,
          'max'=>1
        },
        ##
        # The manufacturer or establishment associated with the process.
        'manufacturer' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
          'type'=>'Reference',
          'path'=>'ManufacturingBusinessOperation.manufacturer',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # A regulator which oversees the operation.
        'regulator' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
          'type'=>'Reference',
          'path'=>'ManufacturingBusinessOperation.regulator',
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
      # The type of manufacturing operation.
      attr_accessor :operationType                  # 0-1 CodeableConcept
      ##
      # Regulatory authorization reference number.
      attr_accessor :authorisationReferenceNumber   # 0-1 Identifier
      ##
      # Regulatory authorization date.
      attr_accessor :effectiveDate                  # 0-1 dateTime
      ##
      # To indicate if this proces is commercially confidential.
      attr_accessor :confidentialityIndicator       # 0-1 CodeableConcept
      ##
      # The manufacturer or establishment associated with the process.
      attr_accessor :manufacturer                   # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Organization) ]
      ##
      # A regulator which oversees the operation.
      attr_accessor :regulator                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
    end

    ##
    # Indicates if the medicinal product has an orphan designation for the treatment of a rare disease.
    class SpecialDesignation < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'indication[x]' => ['CodeableConcept', 'Reference']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'SpecialDesignation.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'SpecialDesignation.extension',
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
          'path'=>'SpecialDesignation.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Identifier for the designation, or procedure number.
        'identifier' => {
          'type'=>'Identifier',
          'path'=>'SpecialDesignation.identifier',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # The type of special designation, e.g. orphan drug, minor use.
        'type' => {
          'type'=>'CodeableConcept',
          'path'=>'SpecialDesignation.type',
          'min'=>0,
          'max'=>1
        },
        ##
        # The intended use of the product, e.g. prevention, treatment.
        'intendedUse' => {
          'type'=>'CodeableConcept',
          'path'=>'SpecialDesignation.intendedUse',
          'min'=>0,
          'max'=>1
        },
        ##
        # Condition for which the medicinal use applies.
        'indicationCodeableConcept' => {
          'type'=>'CodeableConcept',
          'path'=>'SpecialDesignation.indication[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Condition for which the medicinal use applies.
        'indicationReference' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/MedicinalProductIndication'],
          'type'=>'Reference',
          'path'=>'SpecialDesignation.indication[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # For example granted, pending, expired or withdrawn.
        'status' => {
          'type'=>'CodeableConcept',
          'path'=>'SpecialDesignation.status',
          'min'=>0,
          'max'=>1
        },
        ##
        # Date when the designation was granted.
        'date' => {
          'type'=>'dateTime',
          'path'=>'SpecialDesignation.date',
          'min'=>0,
          'max'=>1
        },
        ##
        # Animal species for which this applies.
        'species' => {
          'type'=>'CodeableConcept',
          'path'=>'SpecialDesignation.species',
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
      # Identifier for the designation, or procedure number.
      attr_accessor :identifier                     # 0-* [ Identifier ]
      ##
      # The type of special designation, e.g. orphan drug, minor use.
      attr_accessor :type                           # 0-1 CodeableConcept
      ##
      # The intended use of the product, e.g. prevention, treatment.
      attr_accessor :intendedUse                    # 0-1 CodeableConcept
      ##
      # Condition for which the medicinal use applies.
      attr_accessor :indicationCodeableConcept      # 0-1 CodeableConcept
      ##
      # Condition for which the medicinal use applies.
      attr_accessor :indicationReference            # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/MedicinalProductIndication)
      ##
      # For example granted, pending, expired or withdrawn.
      attr_accessor :status                         # 0-1 CodeableConcept
      ##
      # Date when the designation was granted.
      attr_accessor :date                           # 0-1 dateTime
      ##
      # Animal species for which this applies.
      attr_accessor :species                        # 0-1 CodeableConcept
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
    # Business identifier for this product. Could be an MPID.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Regulatory type, e.g. Investigational or Authorized.
    attr_accessor :type                           # 0-1 CodeableConcept
    ##
    # If this medicine applies to human or veterinary uses.
    attr_accessor :domain                         # 0-1 Coding
    ##
    # The dose form for a single part product, or combined form of a multiple part product.
    attr_accessor :combinedPharmaceuticalDoseForm # 0-1 CodeableConcept
    ##
    # The legal status of supply of the medicinal product as classified by the regulator.
    attr_accessor :legalStatusOfSupply            # 0-1 CodeableConcept
    ##
    # Whether the Medicinal Product is subject to additional monitoring for regulatory reasons.
    attr_accessor :additionalMonitoringIndicator  # 0-1 CodeableConcept
    ##
    # Whether the Medicinal Product is subject to special measures for regulatory reasons.
    attr_accessor :specialMeasures                # 0-* [ string ]
    ##
    # If authorised for use in children.
    attr_accessor :paediatricUseIndicator         # 0-1 CodeableConcept
    ##
    # Allows the product to be classified by various systems.
    attr_accessor :productClassification          # 0-* [ CodeableConcept ]
    ##
    # Marketing status of the medicinal product, in contrast to marketing authorizaton.
    attr_accessor :marketingStatus                # 0-* [ MarketingStatus ]
    ##
    # Pharmaceutical aspects of product.
    attr_accessor :pharmaceuticalProduct          # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/MedicinalProductPharmaceutical) ]
    ##
    # Package representation for the product.
    attr_accessor :packagedMedicinalProduct       # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/MedicinalProductPackaged) ]
    ##
    # Supporting documentation, typically for regulatory submission.
    attr_accessor :attachedDocument               # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/DocumentReference) ]
    ##
    # A master file for to the medicinal product (e.g. Pharmacovigilance System Master File).
    attr_accessor :masterFile                     # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/DocumentReference) ]
    ##
    # A product specific contact, person (in a role), or an organization.
    attr_accessor :contact                        # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/PractitionerRole) ]
    ##
    # Clinical trials or studies that this product is involved in.
    attr_accessor :clinicalTrial                  # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/ResearchStudy) ]
    ##
    # The product's name, including full name and possibly coded parts.
    attr_accessor :name                           # 1-* [ MedicinalProduct::Name ]
    ##
    # Reference to another product, e.g. for linking authorised to investigational product.
    attr_accessor :crossReference                 # 0-* [ Identifier ]
    ##
    # An operation applied to the product, for manufacturing or adminsitrative purpose.
    attr_accessor :manufacturingBusinessOperation # 0-* [ MedicinalProduct::ManufacturingBusinessOperation ]
    ##
    # Indicates if the medicinal product has an orphan designation for the treatment of a rare disease.
    attr_accessor :specialDesignation             # 0-* [ MedicinalProduct::SpecialDesignation ]

    def resourceType
      'MedicinalProduct'
    end
  end
end
