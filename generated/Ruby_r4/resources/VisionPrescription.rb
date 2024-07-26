module FHIR

  ##
  # An authorization for the provision of glasses and/or contact lenses to a patient.
  class VisionPrescription < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['datewritten', 'encounter', 'identifier', 'patient', 'prescriber', 'status']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'VisionPrescription.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'VisionPrescription.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'VisionPrescription.implicitRules',
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
        'path'=>'VisionPrescription.language',
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
        'path'=>'VisionPrescription.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'VisionPrescription.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'VisionPrescription.extension',
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
        'path'=>'VisionPrescription.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Business Identifier for vision prescription
      # A unique identifier assigned to this vision prescription.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'VisionPrescription.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # active | cancelled | draft | entered-in-error
      # The status of the resource instance.
      # This element is labeled as a modifier because the status contains codes that mark the resource as not currently valid.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/fm-status'=>[ 'active', 'cancelled', 'draft', 'entered-in-error' ]
        },
        'type'=>'code',
        'path'=>'VisionPrescription.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/fm-status'}
      },
      ##
      # Response creation date
      # The date this resource was created.
      'created' => {
        'type'=>'dateTime',
        'path'=>'VisionPrescription.created',
        'min'=>1,
        'max'=>1
      },
      ##
      # Who prescription is for
      # A resource reference to the person to whom the vision prescription applies.
      'patient' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient'],
        'type'=>'Reference',
        'path'=>'VisionPrescription.patient',
        'min'=>1,
        'max'=>1
      },
      ##
      # Created during encounter / admission / stay
      # A reference to a resource that identifies the particular occurrence of contact between patient and health care provider during which the prescription was issued.
      'encounter' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Encounter'],
        'type'=>'Reference',
        'path'=>'VisionPrescription.encounter',
        'min'=>0,
        'max'=>1
      },
      ##
      # When prescription was authorized
      # The date (and perhaps time) when the prescription was written.
      # Jurisdictions determine the valid lifetime of a prescription. Typically vision prescriptions are valid for two years from the date written.
      'dateWritten' => {
        'type'=>'dateTime',
        'path'=>'VisionPrescription.dateWritten',
        'min'=>1,
        'max'=>1
      },
      ##
      # Who authorized the vision prescription
      # The healthcare professional responsible for authorizing the prescription.
      'prescriber' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole'],
        'type'=>'Reference',
        'path'=>'VisionPrescription.prescriber',
        'min'=>1,
        'max'=>1
      },
      ##
      # Vision lens authorization
      # Contain the details of  the individual lens specifications and serves as the authorization for the fullfillment by certified professionals.
      'lensSpecification' => {
        'type'=>'VisionPrescription::LensSpecification',
        'path'=>'VisionPrescription.lensSpecification',
        'min'=>1,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Vision lens authorization
    # Contain the details of  the individual lens specifications and serves as the authorization for the fullfillment by certified professionals.
    class LensSpecification < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'LensSpecification.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'LensSpecification.extension',
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
          'path'=>'LensSpecification.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Product to be supplied
        # Identifies the type of vision correction product which is required for the patient.
        'product' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/ex-visionprescriptionproduct'=>[ 'lens', 'contact' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'LensSpecification.product',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/vision-product'}
        },
        ##
        # right | left
        # The eye for which the lens specification applies.
        # May also appear as OD (oculus dexter) for the right eye and OS (oculus siniter) for the left eye.
        'eye' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/vision-eye-codes'=>[ 'right', 'left' ]
          },
          'type'=>'code',
          'path'=>'LensSpecification.eye',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/vision-eye-codes'}
        },
        ##
        # Power of the lens
        # Lens power measured in dioptres (0.25 units).
        # The value is negative for near-sighted and positive for far sighted.
        # Often insurance will not cover a lens with power between +75 and -75.
        'sphere' => {
          'type'=>'decimal',
          'path'=>'LensSpecification.sphere',
          'min'=>0,
          'max'=>1
        },
        ##
        # Lens power for astigmatism
        # Power adjustment for astigmatism measured in dioptres (0.25 units).
        'cylinder' => {
          'type'=>'decimal',
          'path'=>'LensSpecification.cylinder',
          'min'=>0,
          'max'=>1
        },
        ##
        # Lens meridian which contain no power for astigmatism
        # Adjustment for astigmatism measured in integer degrees.
        # The limits are +180 and -180 degrees.
        'axis' => {
          'type'=>'integer',
          'path'=>'LensSpecification.axis',
          'min'=>0,
          'max'=>1
        },
        ##
        # Eye alignment compensation
        # Allows for adjustment on two axis.
        'prism' => {
          'type'=>'VisionPrescription::LensSpecification::Prism',
          'path'=>'LensSpecification.prism',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Added power for multifocal levels
        # Power adjustment for multifocal lenses measured in dioptres (0.25 units).
        'add' => {
          'type'=>'decimal',
          'path'=>'LensSpecification.add',
          'min'=>0,
          'max'=>1
        },
        ##
        # Contact lens power measured in dioptres (0.25 units).
        'power' => {
          'type'=>'decimal',
          'path'=>'LensSpecification.power',
          'min'=>0,
          'max'=>1
        },
        ##
        # Contact lens back curvature
        # Back curvature measured in millimetres.
        'backCurve' => {
          'type'=>'decimal',
          'path'=>'LensSpecification.backCurve',
          'min'=>0,
          'max'=>1
        },
        ##
        # Contact lens diameter measured in millimetres.
        'diameter' => {
          'type'=>'decimal',
          'path'=>'LensSpecification.diameter',
          'min'=>0,
          'max'=>1
        },
        ##
        # Lens wear duration
        # The recommended maximum wear period for the lens.
        'duration' => {
          'type'=>'Quantity',
          'path'=>'LensSpecification.duration',
          'min'=>0,
          'max'=>1
        },
        ##
        # Color required
        # Special color or pattern.
        'color' => {
          'type'=>'string',
          'path'=>'LensSpecification.color',
          'min'=>0,
          'max'=>1
        },
        ##
        # Brand required
        # Brand recommendations or restrictions.
        'brand' => {
          'type'=>'string',
          'path'=>'LensSpecification.brand',
          'min'=>0,
          'max'=>1
        },
        ##
        # Notes for coatings
        # Notes for special requirements such as coatings and lens materials.
        'note' => {
          'type'=>'Annotation',
          'path'=>'LensSpecification.note',
          'min'=>0,
          'max'=>Float::INFINITY
        }
      }

      ##
      # Eye alignment compensation
      # Allows for adjustment on two axis.
      class Prism < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'id',
            'path'=>'Prism.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Prism.extension',
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
            'path'=>'Prism.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Amount of adjustment
          # Amount of prism to compensate for eye alignment in fractional units.
          'amount' => {
            'type'=>'decimal',
            'path'=>'Prism.amount',
            'min'=>1,
            'max'=>1
          },
          ##
          # up | down | in | out
          # The relative base, or reference lens edge, for the prism.
          'base' => {
            'valid_codes'=>{
              'http://hl7.org/fhir/vision-base-codes'=>[ 'up', 'down', 'in', 'out' ]
            },
            'type'=>'code',
            'path'=>'Prism.base',
            'min'=>1,
            'max'=>1,
            'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/vision-base-codes'}
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
        # Amount of adjustment
        # Amount of prism to compensate for eye alignment in fractional units.
        attr_accessor :amount                         # 1-1 decimal
        ##
        # up | down | in | out
        # The relative base, or reference lens edge, for the prism.
        attr_accessor :base                           # 1-1 code
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
      # Product to be supplied
      # Identifies the type of vision correction product which is required for the patient.
      attr_accessor :product                        # 1-1 CodeableConcept
      ##
      # right | left
      # The eye for which the lens specification applies.
      # May also appear as OD (oculus dexter) for the right eye and OS (oculus siniter) for the left eye.
      attr_accessor :eye                            # 1-1 code
      ##
      # Power of the lens
      # Lens power measured in dioptres (0.25 units).
      # The value is negative for near-sighted and positive for far sighted.
      # Often insurance will not cover a lens with power between +75 and -75.
      attr_accessor :sphere                         # 0-1 decimal
      ##
      # Lens power for astigmatism
      # Power adjustment for astigmatism measured in dioptres (0.25 units).
      attr_accessor :cylinder                       # 0-1 decimal
      ##
      # Lens meridian which contain no power for astigmatism
      # Adjustment for astigmatism measured in integer degrees.
      # The limits are +180 and -180 degrees.
      attr_accessor :axis                           # 0-1 integer
      ##
      # Eye alignment compensation
      # Allows for adjustment on two axis.
      attr_accessor :prism                          # 0-* [ VisionPrescription::LensSpecification::Prism ]
      ##
      # Added power for multifocal levels
      # Power adjustment for multifocal lenses measured in dioptres (0.25 units).
      attr_accessor :add                            # 0-1 decimal
      ##
      # Contact lens power measured in dioptres (0.25 units).
      attr_accessor :power                          # 0-1 decimal
      ##
      # Contact lens back curvature
      # Back curvature measured in millimetres.
      attr_accessor :backCurve                      # 0-1 decimal
      ##
      # Contact lens diameter measured in millimetres.
      attr_accessor :diameter                       # 0-1 decimal
      ##
      # Lens wear duration
      # The recommended maximum wear period for the lens.
      attr_accessor :duration                       # 0-1 Quantity
      ##
      # Color required
      # Special color or pattern.
      attr_accessor :color                          # 0-1 string
      ##
      # Brand required
      # Brand recommendations or restrictions.
      attr_accessor :brand                          # 0-1 string
      ##
      # Notes for coatings
      # Notes for special requirements such as coatings and lens materials.
      attr_accessor :note                           # 0-* [ Annotation ]
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
    # Business Identifier for vision prescription
    # A unique identifier assigned to this vision prescription.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # active | cancelled | draft | entered-in-error
    # The status of the resource instance.
    # This element is labeled as a modifier because the status contains codes that mark the resource as not currently valid.
    attr_accessor :status                         # 1-1 code
    ##
    # Response creation date
    # The date this resource was created.
    attr_accessor :created                        # 1-1 dateTime
    ##
    # Who prescription is for
    # A resource reference to the person to whom the vision prescription applies.
    attr_accessor :patient                        # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient)
    ##
    # Created during encounter / admission / stay
    # A reference to a resource that identifies the particular occurrence of contact between patient and health care provider during which the prescription was issued.
    attr_accessor :encounter                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Encounter)
    ##
    # When prescription was authorized
    # The date (and perhaps time) when the prescription was written.
    # Jurisdictions determine the valid lifetime of a prescription. Typically vision prescriptions are valid for two years from the date written.
    attr_accessor :dateWritten                    # 1-1 dateTime
    ##
    # Who authorized the vision prescription
    # The healthcare professional responsible for authorizing the prescription.
    attr_accessor :prescriber                     # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole)
    ##
    # Vision lens authorization
    # Contain the details of  the individual lens specifications and serves as the authorization for the fullfillment by certified professionals.
    attr_accessor :lensSpecification              # 1-* [ VisionPrescription::LensSpecification ]

    def resourceType
      'VisionPrescription'
    end
  end
end
