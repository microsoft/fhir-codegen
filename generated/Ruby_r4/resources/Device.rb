module FHIR

  ##
  # A type of a manufactured item that is used in the provision of healthcare without being substantially changed through that activity. The device may be a medical or non-medical device.
  # Allows institutions to track their devices.
  class Device < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['device-name', 'din', 'identifier', 'location', 'manufacturer', 'model', 'organization', 'patient', 'status', 'type', 'udi-carrier', 'udi-di', 'url']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'Device.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'Device.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'Device.implicitRules',
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
        'path'=>'Device.language',
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
        'path'=>'Device.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'Device.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Device.extension',
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
        'path'=>'Device.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Instance identifier
      # Unique instance identifiers assigned to a device by manufacturers other organizations or owners.
      # The barcode string from a barcode present on a device label or package may identify the instance, include names given to the device in local usage, or may identify the type of device. If the identifier identifies the type of device, Device.type element should be used.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'Device.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The reference to the definition for the device.
      'definition' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/DeviceDefinition'],
        'type'=>'Reference',
        'path'=>'Device.definition',
        'min'=>0,
        'max'=>1
      },
      ##
      # Unique Device Identifier (UDI) Barcode string
      # Unique device identifier (UDI) assigned to device label or package.  Note that the Device may include multiple udiCarriers as it either may include just the udiCarrier for the jurisdiction it is sold, or for multiple jurisdictions it could have been sold.
      # UDI may identify an unique instance of a device, or it may only identify the type of the device.  See [UDI mappings](device-mappings.html#udi) for a complete mapping of UDI parts to Device.
      'udiCarrier' => {
        'type'=>'Device::UdiCarrier',
        'path'=>'Device.udiCarrier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # active | inactive | entered-in-error | unknown
      # Status of the Device availability.
      # This element is labeled as a modifier because the status contains the codes inactive and entered-in-error that mark the device (record)as not currently valid.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/device-status'=>[ 'active', 'inactive', 'entered-in-error', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'Device.status',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/device-status'}
      },
      ##
      # online | paused | standby | offline | not-ready | transduc-discon | hw-discon | off
      # Reason for the dtatus of the Device availability.
      'statusReason' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/device-status-reason'=>[ 'online', 'paused', 'standby', 'offline', 'not-ready', 'transduc-discon', 'hw-discon', 'off' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Device.statusReason',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/device-status-reason'}
      },
      ##
      # The distinct identification string as required by regulation for a human cell, tissue, or cellular and tissue-based product.
      # For example, this applies to devices in the United States regulated under *Code of Federal Regulation 21CFR§1271.290(c)*.
      'distinctIdentifier' => {
        'type'=>'string',
        'path'=>'Device.distinctIdentifier',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name of device manufacturer
      # A name of the manufacturer.
      'manufacturer' => {
        'type'=>'string',
        'path'=>'Device.manufacturer',
        'min'=>0,
        'max'=>1
      },
      ##
      # Date when the device was made
      # The date and time when the device was manufactured.
      'manufactureDate' => {
        'type'=>'dateTime',
        'path'=>'Device.manufactureDate',
        'min'=>0,
        'max'=>1
      },
      ##
      # Date and time of expiry of this device (if applicable)
      # The date and time beyond which this device is no longer valid or should not be used (if applicable).
      'expirationDate' => {
        'type'=>'dateTime',
        'path'=>'Device.expirationDate',
        'min'=>0,
        'max'=>1
      },
      ##
      # Lot number of manufacture
      # Lot number assigned by the manufacturer.
      'lotNumber' => {
        'type'=>'string',
        'path'=>'Device.lotNumber',
        'min'=>0,
        'max'=>1
      },
      ##
      # Serial number assigned by the manufacturer
      # The serial number assigned by the organization when the device was manufactured.
      # Alphanumeric Maximum 20.
      'serialNumber' => {
        'type'=>'string',
        'path'=>'Device.serialNumber',
        'min'=>0,
        'max'=>1
      },
      ##
      # The name of the device as given by the manufacturer
      # This represents the manufacturer's name of the device as provided by the device, from a UDI label, or by a person describing the Device.  This typically would be used when a person provides the name(s) or when the device represents one of the names available from DeviceDefinition.
      'deviceName' => {
        'type'=>'Device::DeviceName',
        'path'=>'Device.deviceName',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The model number for the device.
      'modelNumber' => {
        'type'=>'string',
        'path'=>'Device.modelNumber',
        'min'=>0,
        'max'=>1
      },
      ##
      # The part number of the device.
      # Alphanumeric Maximum 20.
      'partNumber' => {
        'type'=>'string',
        'path'=>'Device.partNumber',
        'min'=>0,
        'max'=>1
      },
      ##
      # The kind or type of device.
      'type' => {
        'type'=>'CodeableConcept',
        'path'=>'Device.type',
        'min'=>0,
        'max'=>1
      },
      ##
      # The capabilities supported on a  device, the standards to which the device conforms for a particular purpose, and used for the communication.
      'specialization' => {
        'type'=>'Device::Specialization',
        'path'=>'Device.specialization',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The actual design of the device or software version running on the device.
      'version' => {
        'type'=>'Device::Version',
        'path'=>'Device.version',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The actual configuration settings of a device as it actually operates, e.g., regulation status, time properties.
      'property' => {
        'type'=>'Device::Property',
        'path'=>'Device.property',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Patient to whom Device is affixed
      # Patient information, If the device is affixed to a person.
      'patient' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient'],
        'type'=>'Reference',
        'path'=>'Device.patient',
        'min'=>0,
        'max'=>1
      },
      ##
      # Organization responsible for device
      # An organization that is responsible for the provision and ongoing maintenance of the device.
      'owner' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'Device.owner',
        'min'=>0,
        'max'=>1
      },
      ##
      # Details for human/organization for support
      # Contact details for an organization or a particular human that is responsible for the device.
      # used for troubleshooting etc.
      'contact' => {
        'type'=>'ContactPoint',
        'path'=>'Device.contact',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Where the device is found
      # The place where the device can be found.
      'location' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Location'],
        'type'=>'Reference',
        'path'=>'Device.location',
        'min'=>0,
        'max'=>1
      },
      ##
      # Network address to contact device
      # A network address on which the device may be contacted directly.
      # If the device is running a FHIR server, the network address should  be the Base URL from which a conformance statement may be retrieved.
      'url' => {
        'type'=>'uri',
        'path'=>'Device.url',
        'min'=>0,
        'max'=>1
      },
      ##
      # Device notes and comments
      # Descriptive information, usage information or implantation information that is not captured in an existing element.
      'note' => {
        'type'=>'Annotation',
        'path'=>'Device.note',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Safety Characteristics of Device
      # Provides additional safety characteristics about a medical device.  For example devices containing latex.
      'safety' => {
        'type'=>'CodeableConcept',
        'path'=>'Device.safety',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The parent device.
      'parent' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Device'],
        'type'=>'Reference',
        'path'=>'Device.parent',
        'min'=>0,
        'max'=>1
      }
    }

    ##
    # Unique Device Identifier (UDI) Barcode string
    # Unique device identifier (UDI) assigned to device label or package.  Note that the Device may include multiple udiCarriers as it either may include just the udiCarrier for the jurisdiction it is sold, or for multiple jurisdictions it could have been sold.
    # UDI may identify an unique instance of a device, or it may only identify the type of the device.  See [UDI mappings](device-mappings.html#udi) for a complete mapping of UDI parts to Device.
    class UdiCarrier < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'UdiCarrier.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'UdiCarrier.extension',
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
          'path'=>'UdiCarrier.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Mandatory fixed portion of UDI
        # The device identifier (DI) is a mandatory, fixed portion of a UDI that identifies the labeler and the specific version or model of a device.
        'deviceIdentifier' => {
          'type'=>'string',
          'path'=>'UdiCarrier.deviceIdentifier',
          'min'=>0,
          'max'=>1
        },
        ##
        # UDI Issuing Organization
        # Organization that is charged with issuing UDIs for devices.  For example, the US FDA issuers include :
        # 1) GS1: 
        # http://hl7.org/fhir/NamingSystem/gs1-di, 
        # 2) HIBCC:
        # http://hl7.org/fhir/NamingSystem/hibcc-dI, 
        # 3) ICCBBA for blood containers:
        # http://hl7.org/fhir/NamingSystem/iccbba-blood-di, 
        # 4) ICCBA for other devices:
        # http://hl7.org/fhir/NamingSystem/iccbba-other-di.
        'issuer' => {
          'type'=>'uri',
          'path'=>'UdiCarrier.issuer',
          'min'=>0,
          'max'=>1
        },
        ##
        # Regional UDI authority
        # The identity of the authoritative source for UDI generation within a  jurisdiction.  All UDIs are globally unique within a single namespace with the appropriate repository uri as the system.  For example,  UDIs of devices managed in the U.S. by the FDA, the value is  http://hl7.org/fhir/NamingSystem/fda-udi.
        'jurisdiction' => {
          'type'=>'uri',
          'path'=>'UdiCarrier.jurisdiction',
          'min'=>0,
          'max'=>1
        },
        ##
        # UDI Machine Readable Barcode String
        # The full UDI carrier of the Automatic Identification and Data Capture (AIDC) technology representation of the barcode string as printed on the packaging of the device - e.g., a barcode or RFID.   Because of limitations on character sets in XML and the need to round-trip JSON data through XML, AIDC Formats *SHALL* be base64 encoded.
        # The AIDC form of UDIs should be scanned or otherwise used for the identification of the device whenever possible to minimize errors in records resulting from manual transcriptions. If separate barcodes for DI and PI are present, concatenate the string with DI first and in order of human readable expression on label.
        'carrierAIDC' => {
          'type'=>'base64Binary',
          'path'=>'UdiCarrier.carrierAIDC',
          'min'=>0,
          'max'=>1
        },
        ##
        # UDI Human Readable Barcode String
        # The full UDI carrier as the human readable form (HRF) representation of the barcode string as printed on the packaging of the device.
        # If separate barcodes for DI and PI are present, concatenate the string with DI first and in order of human readable expression on label.
        'carrierHRF' => {
          'type'=>'string',
          'path'=>'UdiCarrier.carrierHRF',
          'min'=>0,
          'max'=>1
        },
        ##
        # barcode | rfid | manual +
        # A coded entry to indicate how the data was entered.
        'entryType' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/udi-entry-type'=>[ 'barcode', 'rfid', 'manual', 'card', 'self-reported', 'unknown' ]
          },
          'type'=>'code',
          'path'=>'UdiCarrier.entryType',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/udi-entry-type'}
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
      # Mandatory fixed portion of UDI
      # The device identifier (DI) is a mandatory, fixed portion of a UDI that identifies the labeler and the specific version or model of a device.
      attr_accessor :deviceIdentifier               # 0-1 string
      ##
      # UDI Issuing Organization
      # Organization that is charged with issuing UDIs for devices.  For example, the US FDA issuers include :
      # 1) GS1: 
      # http://hl7.org/fhir/NamingSystem/gs1-di, 
      # 2) HIBCC:
      # http://hl7.org/fhir/NamingSystem/hibcc-dI, 
      # 3) ICCBBA for blood containers:
      # http://hl7.org/fhir/NamingSystem/iccbba-blood-di, 
      # 4) ICCBA for other devices:
      # http://hl7.org/fhir/NamingSystem/iccbba-other-di.
      attr_accessor :issuer                         # 0-1 uri
      ##
      # Regional UDI authority
      # The identity of the authoritative source for UDI generation within a  jurisdiction.  All UDIs are globally unique within a single namespace with the appropriate repository uri as the system.  For example,  UDIs of devices managed in the U.S. by the FDA, the value is  http://hl7.org/fhir/NamingSystem/fda-udi.
      attr_accessor :jurisdiction                   # 0-1 uri
      ##
      # UDI Machine Readable Barcode String
      # The full UDI carrier of the Automatic Identification and Data Capture (AIDC) technology representation of the barcode string as printed on the packaging of the device - e.g., a barcode or RFID.   Because of limitations on character sets in XML and the need to round-trip JSON data through XML, AIDC Formats *SHALL* be base64 encoded.
      # The AIDC form of UDIs should be scanned or otherwise used for the identification of the device whenever possible to minimize errors in records resulting from manual transcriptions. If separate barcodes for DI and PI are present, concatenate the string with DI first and in order of human readable expression on label.
      attr_accessor :carrierAIDC                    # 0-1 base64Binary
      ##
      # UDI Human Readable Barcode String
      # The full UDI carrier as the human readable form (HRF) representation of the barcode string as printed on the packaging of the device.
      # If separate barcodes for DI and PI are present, concatenate the string with DI first and in order of human readable expression on label.
      attr_accessor :carrierHRF                     # 0-1 string
      ##
      # barcode | rfid | manual +
      # A coded entry to indicate how the data was entered.
      attr_accessor :entryType                      # 0-1 code
    end

    ##
    # The name of the device as given by the manufacturer
    # This represents the manufacturer's name of the device as provided by the device, from a UDI label, or by a person describing the Device.  This typically would be used when a person provides the name(s) or when the device represents one of the names available from DeviceDefinition.
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
          'type'=>'CodeableConcept',
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
      attr_accessor :systemType                     # 1-1 CodeableConcept
      ##
      # The version of the standard that is used to operate and communicate.
      attr_accessor :version                        # 0-1 string
    end

    ##
    # The actual design of the device or software version running on the device.
    class Version < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Version.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Version.extension',
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
          'path'=>'Version.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # The type of the device version.
        'type' => {
          'type'=>'CodeableConcept',
          'path'=>'Version.type',
          'min'=>0,
          'max'=>1
        },
        ##
        # A single component of the device version.
        'component' => {
          'type'=>'Identifier',
          'path'=>'Version.component',
          'min'=>0,
          'max'=>1
        },
        ##
        # The version text.
        'value' => {
          'type'=>'string',
          'path'=>'Version.value',
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
      # The type of the device version.
      attr_accessor :type                           # 0-1 CodeableConcept
      ##
      # A single component of the device version.
      attr_accessor :component                      # 0-1 Identifier
      ##
      # The version text.
      attr_accessor :value                          # 1-1 string
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
    # Unique instance identifiers assigned to a device by manufacturers other organizations or owners.
    # The barcode string from a barcode present on a device label or package may identify the instance, include names given to the device in local usage, or may identify the type of device. If the identifier identifies the type of device, Device.type element should be used.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # The reference to the definition for the device.
    attr_accessor :definition                     # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/DeviceDefinition)
    ##
    # Unique Device Identifier (UDI) Barcode string
    # Unique device identifier (UDI) assigned to device label or package.  Note that the Device may include multiple udiCarriers as it either may include just the udiCarrier for the jurisdiction it is sold, or for multiple jurisdictions it could have been sold.
    # UDI may identify an unique instance of a device, or it may only identify the type of the device.  See [UDI mappings](device-mappings.html#udi) for a complete mapping of UDI parts to Device.
    attr_accessor :udiCarrier                     # 0-* [ Device::UdiCarrier ]
    ##
    # active | inactive | entered-in-error | unknown
    # Status of the Device availability.
    # This element is labeled as a modifier because the status contains the codes inactive and entered-in-error that mark the device (record)as not currently valid.
    attr_accessor :status                         # 0-1 code
    ##
    # online | paused | standby | offline | not-ready | transduc-discon | hw-discon | off
    # Reason for the dtatus of the Device availability.
    attr_accessor :statusReason                   # 0-* [ CodeableConcept ]
    ##
    # The distinct identification string as required by regulation for a human cell, tissue, or cellular and tissue-based product.
    # For example, this applies to devices in the United States regulated under *Code of Federal Regulation 21CFR§1271.290(c)*.
    attr_accessor :distinctIdentifier             # 0-1 string
    ##
    # Name of device manufacturer
    # A name of the manufacturer.
    attr_accessor :manufacturer                   # 0-1 string
    ##
    # Date when the device was made
    # The date and time when the device was manufactured.
    attr_accessor :manufactureDate                # 0-1 dateTime
    ##
    # Date and time of expiry of this device (if applicable)
    # The date and time beyond which this device is no longer valid or should not be used (if applicable).
    attr_accessor :expirationDate                 # 0-1 dateTime
    ##
    # Lot number of manufacture
    # Lot number assigned by the manufacturer.
    attr_accessor :lotNumber                      # 0-1 string
    ##
    # Serial number assigned by the manufacturer
    # The serial number assigned by the organization when the device was manufactured.
    # Alphanumeric Maximum 20.
    attr_accessor :serialNumber                   # 0-1 string
    ##
    # The name of the device as given by the manufacturer
    # This represents the manufacturer's name of the device as provided by the device, from a UDI label, or by a person describing the Device.  This typically would be used when a person provides the name(s) or when the device represents one of the names available from DeviceDefinition.
    attr_accessor :deviceName                     # 0-* [ Device::DeviceName ]
    ##
    # The model number for the device.
    attr_accessor :modelNumber                    # 0-1 string
    ##
    # The part number of the device.
    # Alphanumeric Maximum 20.
    attr_accessor :partNumber                     # 0-1 string
    ##
    # The kind or type of device.
    attr_accessor :type                           # 0-1 CodeableConcept
    ##
    # The capabilities supported on a  device, the standards to which the device conforms for a particular purpose, and used for the communication.
    attr_accessor :specialization                 # 0-* [ Device::Specialization ]
    ##
    # The actual design of the device or software version running on the device.
    attr_accessor :version                        # 0-* [ Device::Version ]
    ##
    # The actual configuration settings of a device as it actually operates, e.g., regulation status, time properties.
    attr_accessor :property                       # 0-* [ Device::Property ]
    ##
    # Patient to whom Device is affixed
    # Patient information, If the device is affixed to a person.
    attr_accessor :patient                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient)
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
    # Where the device is found
    # The place where the device can be found.
    attr_accessor :location                       # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Location)
    ##
    # Network address to contact device
    # A network address on which the device may be contacted directly.
    # If the device is running a FHIR server, the network address should  be the Base URL from which a conformance statement may be retrieved.
    attr_accessor :url                            # 0-1 uri
    ##
    # Device notes and comments
    # Descriptive information, usage information or implantation information that is not captured in an existing element.
    attr_accessor :note                           # 0-* [ Annotation ]
    ##
    # Safety Characteristics of Device
    # Provides additional safety characteristics about a medical device.  For example devices containing latex.
    attr_accessor :safety                         # 0-* [ CodeableConcept ]
    ##
    # The parent device.
    attr_accessor :parent                         # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Device)

    def resourceType
      'Device'
    end
  end
end
