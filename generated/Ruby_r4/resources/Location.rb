module FHIR

  ##
  # Details and position information for a physical place where services are provided and resources and participants may be stored, found, contained, or accommodated.
  class Location < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['address-city', 'address-country', 'address-postalcode', 'address-state', 'address-use', 'address', 'endpoint', 'identifier', 'name', 'near', 'operational-status', 'organization', 'partof', 'status', 'type']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'Location.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'Location.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'Location.implicitRules',
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
        'path'=>'Location.language',
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
        'path'=>'Location.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'Location.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Location.extension',
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
        'path'=>'Location.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Unique code or number identifying the location to its users.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'Location.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # active | suspended | inactive
      # The status property covers the general availability of the resource, not the current value which may be covered by the operationStatus, or by a schedule/slots if they are configured for the location.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/location-status'=>[ 'active', 'suspended', 'inactive' ]
        },
        'type'=>'code',
        'path'=>'Location.status',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/location-status'}
      },
      ##
      # The operational status of the location (typically only for a bed/room)
      # The operational status covers operation values most relevant to beds (but can also apply to rooms/units/chairs/etc. such as an isolation unit/dialysis chair). This typically covers concepts such as contamination, housekeeping, and other activities like maintenance.
      'operationalStatus' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/v2-0116'=>[ 'C', 'H', 'I', 'K', 'O', 'U' ]
        },
        'type'=>'Coding',
        'path'=>'Location.operationalStatus',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'preferred', 'uri'=>'http://terminology.hl7.org/ValueSet/v2-0116'}
      },
      ##
      # Name of the location as used by humans. Does not need to be unique.
      # If the name of a location changes, consider putting the old name in the alias column so that it can still be located through searches.
      'name' => {
        'type'=>'string',
        'path'=>'Location.name',
        'min'=>0,
        'max'=>1
      },
      ##
      # A list of alternate names that the location is known as, or was known as, in the past.
      # There are no dates associated with the alias/historic names, as this is not intended to track when names were used, but to assist in searching so that older names can still result in identifying the location.
      'alias' => {
        'local_name'=>'local_alias'
        'type'=>'string',
        'path'=>'Location.alias',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional details about the location that could be displayed as further information to identify the location beyond its name
      # Description of the Location, which helps in finding or referencing the place.
      'description' => {
        'type'=>'string',
        'path'=>'Location.description',
        'min'=>0,
        'max'=>1
      },
      ##
      # instance | kind
      # Indicates whether a resource instance represents a specific location or a class of locations.
      # This is labeled as a modifier because whether or not the location is a class of locations changes how it can be used and understood.
      'mode' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/location-mode'=>[ 'instance', 'kind' ]
        },
        'type'=>'code',
        'path'=>'Location.mode',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/location-mode'}
      },
      ##
      # Type of function performed
      # Indicates the type of function performed at the location.
      'type' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/v3-RoleCode'=>[ 'DX', 'CVDX', 'CATH', 'ECHO', 'GIDX', 'ENDOS', 'RADDX', 'RADO', 'RNEU', 'HOSP', 'CHR', 'GACH', 'MHSP', 'PSYCHF', 'RH', 'RHAT', 'RHII', 'RHMAD', 'RHPI', 'RHPIH', 'RHPIMS', 'RHPIVS', 'RHYAD', 'HU', 'BMTU', 'CCU', 'CHEST', 'EPIL', 'ER', 'ETU', 'HD', 'HLAB', 'INLAB', 'OUTLAB', 'HRAD', 'HUSCS', 'ICU', 'PEDICU', 'PEDNICU', 'INPHARM', 'MBL', 'NCCS', 'NS', 'OUTPHARM', 'PEDU', 'PHU', 'RHU', 'SLEEP', 'NCCF', 'SNF', 'OF', 'ALL', 'AMPUT', 'BMTC', 'BREAST', 'CANC', 'CAPC', 'CARD', 'PEDCARD', 'COAG', 'CRS', 'DERM', 'ENDO', 'PEDE', 'ENT', 'FMC', 'GI', 'PEDGI', 'GIM', 'GYN', 'HEM', 'PEDHEM', 'HTN', 'IEC', 'INFD', 'PEDID', 'INV', 'LYMPH', 'MGEN', 'NEPH', 'PEDNEPH', 'NEUR', 'OB', 'OMS', 'ONCL', 'PEDHO', 'OPH', 'OPTC', 'ORTHO', 'HAND', 'PAINCL', 'PC', 'PEDC', 'PEDRHEUM', 'POD', 'PREV', 'PROCTO', 'PROFF', 'PROS', 'PSI', 'PSY', 'RHEUM', 'SPMED', 'SU', 'PLS', 'URO', 'TR', 'TRAVEL', 'WND', 'RTF', 'PRC', 'SURF', 'DADDR', 'MOBL', 'AMB', 'PHARM', 'ACC', 'COMM', 'CSC', 'PTRES', 'SCHOOL', 'UPC', 'WORK' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Location.type',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://terminology.hl7.org/ValueSet/v3-ServiceDeliveryLocationRoleType'}
      },
      ##
      # Contact details of the location
      # The contact details of communication devices available at the location. This can include phone numbers, fax numbers, mobile numbers, email addresses and web sites.
      'telecom' => {
        'type'=>'ContactPoint',
        'path'=>'Location.telecom',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Physical location.
      # Additional addresses should be recorded using another instance of the Location resource, or via the Organization.
      'address' => {
        'type'=>'Address',
        'path'=>'Location.address',
        'min'=>0,
        'max'=>1
      },
      ##
      # Physical form of the location, e.g. building, room, vehicle, road.
      'physicalType' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/location-physical-type'=>[ 'si', 'bu', 'wi', 'wa', 'lvl', 'co', 'ro', 'bd', 've', 'ho', 'ca', 'rd', 'area', 'jdn' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Location.physicalType',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/location-physical-type'}
      },
      ##
      # The absolute geographic location of the Location, expressed using the WGS84 datum (This is the same co-ordinate system used in KML).
      'position' => {
        'type'=>'Location::Position',
        'path'=>'Location.position',
        'min'=>0,
        'max'=>1
      },
      ##
      # Organization responsible for provisioning and upkeep
      # The organization responsible for the provisioning and upkeep of the location.
      # This can also be used as the part of the organization hierarchy where this location provides services. These services can be defined through the HealthcareService resource.
      'managingOrganization' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'Location.managingOrganization',
        'min'=>0,
        'max'=>1
      },
      ##
      # Another Location this one is physically a part of
      # Another Location of which this Location is physically a part of.
      'partOf' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Location'],
        'type'=>'Reference',
        'path'=>'Location.partOf',
        'min'=>0,
        'max'=>1
      },
      ##
      # What days/times during a week is this location usually open.
      # This type of information is commonly found published in directories and on websites informing customers when the facility is available.
      # 
      # Specific services within the location may have their own hours which could be shorter (or longer) than the locations hours.
      'hoursOfOperation' => {
        'type'=>'Location::HoursOfOperation',
        'path'=>'Location.hoursOfOperation',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Description of availability exceptions
      # A description of when the locations opening ours are different to normal, e.g. public holiday availability. Succinctly describing all possible exceptions to normal site availability as detailed in the opening hours Times.
      'availabilityExceptions' => {
        'type'=>'string',
        'path'=>'Location.availabilityExceptions',
        'min'=>0,
        'max'=>1
      },
      ##
      # Technical endpoints providing access to services operated for the location.
      'endpoint' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Endpoint'],
        'type'=>'Reference',
        'path'=>'Location.endpoint',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # The absolute geographic location of the Location, expressed using the WGS84 datum (This is the same co-ordinate system used in KML).
    class Position < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Position.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Position.extension',
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
          'path'=>'Position.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Longitude with WGS84 datum
        # Longitude. The value domain and the interpretation are the same as for the text of the longitude element in KML (see notes below).
        'longitude' => {
          'type'=>'decimal',
          'path'=>'Position.longitude',
          'min'=>1,
          'max'=>1
        },
        ##
        # Latitude with WGS84 datum
        # Latitude. The value domain and the interpretation are the same as for the text of the latitude element in KML (see notes below).
        'latitude' => {
          'type'=>'decimal',
          'path'=>'Position.latitude',
          'min'=>1,
          'max'=>1
        },
        ##
        # Altitude with WGS84 datum
        # Altitude. The value domain and the interpretation are the same as for the text of the altitude element in KML (see notes below).
        'altitude' => {
          'type'=>'decimal',
          'path'=>'Position.altitude',
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
      # Longitude with WGS84 datum
      # Longitude. The value domain and the interpretation are the same as for the text of the longitude element in KML (see notes below).
      attr_accessor :longitude                      # 1-1 decimal
      ##
      # Latitude with WGS84 datum
      # Latitude. The value domain and the interpretation are the same as for the text of the latitude element in KML (see notes below).
      attr_accessor :latitude                       # 1-1 decimal
      ##
      # Altitude with WGS84 datum
      # Altitude. The value domain and the interpretation are the same as for the text of the altitude element in KML (see notes below).
      attr_accessor :altitude                       # 0-1 decimal
    end

    ##
    # What days/times during a week is this location usually open.
    # This type of information is commonly found published in directories and on websites informing customers when the facility is available.
    # 
    # Specific services within the location may have their own hours which could be shorter (or longer) than the locations hours.
    class HoursOfOperation < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'HoursOfOperation.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'HoursOfOperation.extension',
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
          'path'=>'HoursOfOperation.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # mon | tue | wed | thu | fri | sat | sun
        # Indicates which days of the week are available between the start and end Times.
        'daysOfWeek' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/days-of-week'=>[ 'mon', 'tue', 'wed', 'thu', 'fri', 'sat', 'sun' ]
          },
          'type'=>'code',
          'path'=>'HoursOfOperation.daysOfWeek',
          'min'=>0,
          'max'=>Float::INFINITY,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/days-of-week'}
        },
        ##
        # The Location is open all day.
        'allDay' => {
          'type'=>'boolean',
          'path'=>'HoursOfOperation.allDay',
          'min'=>0,
          'max'=>1
        },
        ##
        # Time that the Location opens.
        'openingTime' => {
          'type'=>'time',
          'path'=>'HoursOfOperation.openingTime',
          'min'=>0,
          'max'=>1
        },
        ##
        # Time that the Location closes.
        'closingTime' => {
          'type'=>'time',
          'path'=>'HoursOfOperation.closingTime',
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
      # mon | tue | wed | thu | fri | sat | sun
      # Indicates which days of the week are available between the start and end Times.
      attr_accessor :daysOfWeek                     # 0-* [ code ]
      ##
      # The Location is open all day.
      attr_accessor :allDay                         # 0-1 boolean
      ##
      # Time that the Location opens.
      attr_accessor :openingTime                    # 0-1 time
      ##
      # Time that the Location closes.
      attr_accessor :closingTime                    # 0-1 time
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
    # Unique code or number identifying the location to its users.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # active | suspended | inactive
    # The status property covers the general availability of the resource, not the current value which may be covered by the operationStatus, or by a schedule/slots if they are configured for the location.
    attr_accessor :status                         # 0-1 code
    ##
    # The operational status of the location (typically only for a bed/room)
    # The operational status covers operation values most relevant to beds (but can also apply to rooms/units/chairs/etc. such as an isolation unit/dialysis chair). This typically covers concepts such as contamination, housekeeping, and other activities like maintenance.
    attr_accessor :operationalStatus              # 0-1 Coding
    ##
    # Name of the location as used by humans. Does not need to be unique.
    # If the name of a location changes, consider putting the old name in the alias column so that it can still be located through searches.
    attr_accessor :name                           # 0-1 string
    ##
    # A list of alternate names that the location is known as, or was known as, in the past.
    # There are no dates associated with the alias/historic names, as this is not intended to track when names were used, but to assist in searching so that older names can still result in identifying the location.
    attr_accessor :local_alias                    # 0-* [ string ]
    ##
    # Additional details about the location that could be displayed as further information to identify the location beyond its name
    # Description of the Location, which helps in finding or referencing the place.
    attr_accessor :description                    # 0-1 string
    ##
    # instance | kind
    # Indicates whether a resource instance represents a specific location or a class of locations.
    # This is labeled as a modifier because whether or not the location is a class of locations changes how it can be used and understood.
    attr_accessor :mode                           # 0-1 code
    ##
    # Type of function performed
    # Indicates the type of function performed at the location.
    attr_accessor :type                           # 0-* [ CodeableConcept ]
    ##
    # Contact details of the location
    # The contact details of communication devices available at the location. This can include phone numbers, fax numbers, mobile numbers, email addresses and web sites.
    attr_accessor :telecom                        # 0-* [ ContactPoint ]
    ##
    # Physical location.
    # Additional addresses should be recorded using another instance of the Location resource, or via the Organization.
    attr_accessor :address                        # 0-1 Address
    ##
    # Physical form of the location, e.g. building, room, vehicle, road.
    attr_accessor :physicalType                   # 0-1 CodeableConcept
    ##
    # The absolute geographic location of the Location, expressed using the WGS84 datum (This is the same co-ordinate system used in KML).
    attr_accessor :position                       # 0-1 Location::Position
    ##
    # Organization responsible for provisioning and upkeep
    # The organization responsible for the provisioning and upkeep of the location.
    # This can also be used as the part of the organization hierarchy where this location provides services. These services can be defined through the HealthcareService resource.
    attr_accessor :managingOrganization           # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Another Location this one is physically a part of
    # Another Location of which this Location is physically a part of.
    attr_accessor :partOf                         # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Location)
    ##
    # What days/times during a week is this location usually open.
    # This type of information is commonly found published in directories and on websites informing customers when the facility is available.
    # 
    # Specific services within the location may have their own hours which could be shorter (or longer) than the locations hours.
    attr_accessor :hoursOfOperation               # 0-* [ Location::HoursOfOperation ]
    ##
    # Description of availability exceptions
    # A description of when the locations opening ours are different to normal, e.g. public holiday availability. Succinctly describing all possible exceptions to normal site availability as detailed in the opening hours Times.
    attr_accessor :availabilityExceptions         # 0-1 string
    ##
    # Technical endpoints providing access to services operated for the location.
    attr_accessor :endpoint                       # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Endpoint) ]

    def resourceType
      'Location'
    end
  end
end
