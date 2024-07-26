module FHIR

  ##
  # The characteristics, operational status and capabilities of a medical-related component of a medical device.
  class DeviceDefinition < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['identifier', 'parent', 'type']
    MULTIPLE_TYPES = {
      'manufacturer[x]' => ['Reference', 'string']
    }
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'DeviceDefinition.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'DeviceDefinition.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'DeviceDefinition.implicitRules',
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
        'path'=>'DeviceDefinition.language',
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
        'path'=>'DeviceDefinition.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'DeviceDefinition.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'DeviceDefinition.extension',
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
        'path'=>'DeviceDefinition.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Instance identifier
      # Unique instance identifiers assigned to a device by the software, manufacturers, other organizations or owners. For example: handle ID.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'DeviceDefinition.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Unique Device Identifier (UDI) Barcode string
      # Unique device identifier (UDI) assigned to device label or package.  Note that the Device may include multiple udiCarriers as it either may include just the udiCarrier for the jurisdiction it is sold, or for multiple jurisdictions it could have been sold.
      'udiDeviceIdentifier' => {
        'type'=>'DeviceDefinition::UdiDeviceIdentifier',
        'path'=>'DeviceDefinition.udiDeviceIdentifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Name of device manufacturer
      # A name of the manufacturer.
      'manufacturerReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'DeviceDefinition.manufacturer[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name of device manufacturer
      # A name of the manufacturer.
      'manufacturerString' => {
        'type'=>'String',
        'path'=>'DeviceDefinition.manufacturer[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # A name given to the device to identify it.
      'deviceName' => {
        'type'=>'DeviceDefinition::DeviceName',
        'path'=>'DeviceDefinition.deviceName',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The model number for the device.
      'modelNumber' => {
        'type'=>'string',
        'path'=>'DeviceDefinition.modelNumber',
        'min'=>0,
        'max'=>1
      },
      ##
      # What kind of device or device system this is.
      'type' => {
        'type'=>'CodeableConcept',
        'path'=>'DeviceDefinition.type',
        'min'=>0,
        'max'=>1
      },
      ##
      # The capabilities supported on a  device, the standards to which the device conforms for a particular purpose, and used for the communication.
      'specialization' => {
        'type'=>'DeviceDefinition::Specialization',
        'path'=>'DeviceDefinition.specialization',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Available versions
      # The available versions of the device, e.g., software versions.
      'version' => {
        'type'=>'string',
        'path'=>'DeviceDefinition.version',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Safety characteristics of the device.
      'safety' => {
        'valid_codes'=>{
          'urn:oid:2.16.840.1.113883.3.26.1.1'=>[ 'C106046', 'C106045', 'C106047', 'C113844', 'C101673', 'C106038' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'DeviceDefinition.safety',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/device-safety'}
      },
      ##
      # Shelf Life and storage information.
      'shelfLifeStorage' => {
        'type'=>'ProductShelfLife',
        'path'=>'DeviceDefinition.shelfLifeStorage',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Dimensions, color etc.
      'physicalCharacteristics' => {
        'type'=>'ProdCharacteristic',
        'path'=>'DeviceDefinition.physicalCharacteristics',
        'min'=>0,
        'max'=>1
      },
      ##
      # Language code for the human-readable text strings produced by the device (all supported).
      'languageCode' => {
        'type'=>'CodeableConcept',
        'path'=>'DeviceDefinition.languageCode',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Device capabilities.
      'capability' => {
        'type'=>'DeviceDefinition::Capability',
        'path'=>'DeviceDefinition.capability',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The actual configuration settings of a device as it actually operates, e.g., regulation status, time properties.
      'property' => {
        'type'=>'DeviceDefinition::Property',
        'path'=>'DeviceDefinition.property',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Organization responsible for device
      # An organization that is responsible for the provision and ongoing maintenance of the device.
      'owner' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'DeviceDefinition.owner',
        'min'=>0,
        'max'=>1
      },
      ##
      # Details for human/organization for support
      # Contact details for an organization or a particular human that is responsible for the device.
      # used for troubleshooting etc.
      'contact' => {
        'type'=>'ContactPoint',
        'path'=>'DeviceDefinition.contact',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Network address to contact device
      # A network address on which the device may be contacted directly.
      # If the device is running a FHIR server, the network address should  be the Base URL from which a conformance statement may be retrieved.
      'url' => {
        'type'=>'uri',
        'path'=>'DeviceDefinition.url',
        'min'=>0,
        'max'=>1
      },
      ##
      # Access to on-line information about the device.
      'onlineInformation' => {
        'type'=>'uri',
        'path'=>'DeviceDefinition.onlineInformation',
        'min'=>0,
        'max'=>1
      },
      ##
      # Device notes and comments
      # Descriptive information, usage information or implantation information that is not captured in an existing element.
      'note' => {
        'type'=>'Annotation',
        'path'=>'DeviceDefinition.note',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The quantity of the device present in the packaging (e.g. the number of devices present in a pack, or the number of devices in the same package of the medicinal product).
      'quantity' => {
        'type'=>'Quantity',
        'path'=>'DeviceDefinition.quantity',
        'min'=>0,
        'max'=>1
      },
      ##
      # The parent device it can be part of.
      'parentDevice' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/DeviceDefinition'],
        'type'=>'Reference',
        'path'=>'DeviceDefinition.parentDevice',
        'min'=>0,
        'max'=>1
      },
      ##
      # A substance used to create the material(s) of which the device is made.
      'material' => {
        'type'=>'DeviceDefinition::Material',
        'path'=>'DeviceDefinition.material',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Unique Device Identifier (UDI) Barcode string
    # Unique device identifier (UDI) assigned to device label or package.  Note that the Device may include multiple udiCarriers as it either may include just the udiCarrier for the jurisdiction it is sold, or for multiple jurisdictions it could have been sold.
    class UdiDeviceIdentifier < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'UdiDeviceIdentifier.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'UdiDeviceIdentifier.extension',
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
          'path'=>'UdiDeviceIdentifier.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # The identifier that is to be associated with every Device that references this DeviceDefintiion for the issuer and jurisdication porvided in the DeviceDefinition.udiDeviceIdentifier.
        'deviceIdentifier' => {
          'type'=>'string',
          'path'=>'UdiDeviceIdentifier.deviceIdentifier',
          'min'=>1,
          'max'=>1
        },
        ##
        # The organization that assigns the identifier algorithm.
        'issuer' => {
          'type'=>'uri',
          'path'=>'UdiDeviceIdentifier.issuer',
          'min'=>1,
          'max'=>1
        },
        ##
        # The jurisdiction to which the deviceIdentifier applies.
        'jurisdiction' => {
          'type'=>'uri',
          'path'=>'UdiDeviceIdentifier.jurisdiction',
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
      # The identifier that is to be associated with every Device that references this DeviceDefintiion for the issuer and jurisdication porvided in the DeviceDefinition.udiDeviceIdentifier.
      attr_accessor :deviceIdentifier               # 1-1 string
      ##
      # The organization that assigns the identifier algorithm.
      attr_accessor :issuer                         # 1-1 uri
      ##
      # The jurisdiction to which the deviceIdentifier applies.
      attr_accessor :jurisdiction                   # 1-1 uri
    end

    ##
    # A name given to the device to identify it.
    class DeviceName < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'DeviceName.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'DeviceName.extension',
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
          'path'=>'DeviceName.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # The name of the device.
        'name' => {
          'type'=>'string',
          'path'=>'DeviceName.name',
          'min'=>1,
          'max'=>1
        },
        ##
        # udi-label-name | user-friendly-name | patient-reported-name | manufacturer-name | model-name | other
        # The type of deviceName.
        # UDILabelName | UserFriendlyName | PatientReportedName | ManufactureDeviceName | ModelName.
        'type' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/device-nametype'=>[ 'udi-label-name', 'user-friendly-name', 'patient-reported-name', 'manufacturer-name', 'model-name', 'other' ]
          },
          'type'=>'code',
          'path'=>'DeviceName.type',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/device-nametype'}
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
      # The name of the device.
      attr_accessor :name                           # 1-1 string
      ##
      # udi-label-name | user-friendly-name | patient-reported-name | manufacturer-name | model-name | other
      # The type of deviceName.
      # UDILabelName | UserFriendlyName | PatientReportedName | ManufactureDeviceName | ModelName.
      attr_accessor :type                           # 1-1 code
    end

    ##
    # The capabilities supported on a  device, the standards to which the device conforms for a particular purpose, and used for the communication.
    class Specialization < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Specialization.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Specialization.extension',
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
          'path'=>'Specialization.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # The standard that is used to operate and communicate.
        'systemType' => {
          'type'=>'string',
          'path'=>'Specialization.systemType',
          'min'=>1,
          'max'=>1
        },
        ##
        # The version of the standard that is used to operate and communicate.
        'version' => {
          'type'=>'string',
          'path'=>'Specialization.version',
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
      # The standard that is used to operate and communicate.
      attr_accessor :systemType                     # 1-1 string
      ##
      # The version of the standard that is used to operate and communicate.
      attr_accessor :version                        # 0-1 string
    end

    ##
    # Device capabilities.
    class Capability < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Capability.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Capability.extension',
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
          'path'=>'Capability.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Type of capability.
        'type' => {
          'type'=>'CodeableConcept',
          'path'=>'Capability.type',
          'min'=>1,
          'max'=>1
        },
        ##
        # Description of capability.
        'description' => {
          'type'=>'CodeableConcept',
          'path'=>'Capability.description',
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
      # Type of capability.
      attr_accessor :type                           # 1-1 CodeableConcept
      ##
      # Description of capability.
      attr_accessor :description                    # 0-* [ CodeableConcept ]
    end

    ##
    # The actual configuration settings of a device as it actually operates, e.g., regulation status, time properties.
    class Property < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
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
        # Code that specifies the property DeviceDefinitionPropetyCode (Extensible).
        'type' => {
          'type'=>'CodeableConcept',
          'path'=>'Property.type',
          'min'=>1,
          'max'=>1
        },
        ##
        # Property value as a quantity.
        'valueQuantity' => {
          'type'=>'Quantity',
          'path'=>'Property.valueQuantity',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Property value as a code, e.g., NTP4 (synced to NTP).
        'valueCode' => {
          'type'=>'CodeableConcept',
          'path'=>'Property.valueCode',
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
      # Code that specifies the property DeviceDefinitionPropetyCode (Extensible).
      attr_accessor :type                           # 1-1 CodeableConcept
      ##
      # Property value as a quantity.
      attr_accessor :valueQuantity                  # 0-* [ Quantity ]
      ##
      # Property value as a code, e.g., NTP4 (synced to NTP).
      attr_accessor :valueCode                      # 0-* [ CodeableConcept ]
    end

    ##
    # A substance used to create the material(s) of which the device is made.
    class Material < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Material.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Material.extension',
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
          'path'=>'Material.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # The substance.
        'substance' => {
          'type'=>'CodeableConcept',
          'path'=>'Material.substance',
          'min'=>1,
          'max'=>1
        },
        ##
        # Indicates an alternative material of the device.
        'alternate' => {
          'type'=>'boolean',
          'path'=>'Material.alternate',
          'min'=>0,
          'max'=>1
        },
        ##
        # Whether the substance is a known or suspected allergen.
        'allergenicIndicator' => {
          'type'=>'boolean',
          'path'=>'Material.allergenicIndicator',
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
      # The substance.
      attr_accessor :substance                      # 1-1 CodeableConcept
      ##
      # Indicates an alternative material of the device.
      attr_accessor :alternate                      # 0-1 boolean
      ##
      # Whether the substance is a known or suspected allergen.
      attr_accessor :allergenicIndicator            # 0-1 boolean
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
    # Instance identifier
    # Unique instance identifiers assigned to a device by the software, manufacturers, other organizations or owners. For example: handle ID.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Unique Device Identifier (UDI) Barcode string
    # Unique device identifier (UDI) assigned to device label or package.  Note that the Device may include multiple udiCarriers as it either may include just the udiCarrier for the jurisdiction it is sold, or for multiple jurisdictions it could have been sold.
    attr_accessor :udiDeviceIdentifier            # 0-* [ DeviceDefinition::UdiDeviceIdentifier ]
    ##
    # Name of device manufacturer
    # A name of the manufacturer.
    attr_accessor :manufacturerReference          # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Name of device manufacturer
    # A name of the manufacturer.
    attr_accessor :manufacturerString             # 0-1 String
    ##
    # A name given to the device to identify it.
    attr_accessor :deviceName                     # 0-* [ DeviceDefinition::DeviceName ]
    ##
    # The model number for the device.
    attr_accessor :modelNumber                    # 0-1 string
    ##
    # What kind of device or device system this is.
    attr_accessor :type                           # 0-1 CodeableConcept
    ##
    # The capabilities supported on a  device, the standards to which the device conforms for a particular purpose, and used for the communication.
    attr_accessor :specialization                 # 0-* [ DeviceDefinition::Specialization ]
    ##
    # Available versions
    # The available versions of the device, e.g., software versions.
    attr_accessor :version                        # 0-* [ string ]
    ##
    # Safety characteristics of the device.
    attr_accessor :safety                         # 0-* [ CodeableConcept ]
    ##
    # Shelf Life and storage information.
    attr_accessor :shelfLifeStorage               # 0-* [ ProductShelfLife ]
    ##
    # Dimensions, color etc.
    attr_accessor :physicalCharacteristics        # 0-1 ProdCharacteristic
    ##
    # Language code for the human-readable text strings produced by the device (all supported).
    attr_accessor :languageCode                   # 0-* [ CodeableConcept ]
    ##
    # Device capabilities.
    attr_accessor :capability                     # 0-* [ DeviceDefinition::Capability ]
    ##
    # The actual configuration settings of a device as it actually operates, e.g., regulation status, time properties.
    attr_accessor :property                       # 0-* [ DeviceDefinition::Property ]
    ##
    # Organization responsible for device
    # An organization that is responsible for the provision and ongoing maintenance of the device.
    attr_accessor :owner                          # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Details for human/organization for support
    # Contact details for an organization or a particular human that is responsible for the device.
    # used for troubleshooting etc.
    attr_accessor :contact                        # 0-* [ ContactPoint ]
    ##
    # Network address to contact device
    # A network address on which the device may be contacted directly.
    # If the device is running a FHIR server, the network address should  be the Base URL from which a conformance statement may be retrieved.
    attr_accessor :url                            # 0-1 uri
    ##
    # Access to on-line information about the device.
    attr_accessor :onlineInformation              # 0-1 uri
    ##
    # Device notes and comments
    # Descriptive information, usage information or implantation information that is not captured in an existing element.
    attr_accessor :note                           # 0-* [ Annotation ]
    ##
    # The quantity of the device present in the packaging (e.g. the number of devices present in a pack, or the number of devices in the same package of the medicinal product).
    attr_accessor :quantity                       # 0-1 Quantity
    ##
    # The parent device it can be part of.
    attr_accessor :parentDevice                   # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/DeviceDefinition)
    ##
    # A substance used to create the material(s) of which the device is made.
    attr_accessor :material                       # 0-* [ DeviceDefinition::Material ]

    def resourceType
      'DeviceDefinition'
    end
  end
end
