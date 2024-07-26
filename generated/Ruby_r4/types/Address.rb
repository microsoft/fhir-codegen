module FHIR

  ##
  # Base StructureDefinition for Address Type: An address expressed using postal conventions (as opposed to GPS or other location definition formats).  This data type may be used to convey addresses for use in delivering mail as well as for visiting locations which might not be valid for mail delivery.  There are a variety of postal address formats defined around the world.
  # Need to be able to record postal addresses, along with notes about their use.
  class Address < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = []
    METADATA = {
      ##
      # Unique id for inter-element referencing
      # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
      'id' => {
        'type'=>'id',
        'path'=>'Address.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Address.extension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # home | work | temp | old | billing - purpose of this address
      # The purpose of this address.
      # Applications can assume that an address is current unless it explicitly says that it is temporary or old.
      'use' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/address-use'=>[ 'home', 'work', 'temp', 'old', 'billing' ]
        },
        'type'=>'code',
        'path'=>'Address.use',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/address-use'}
      },
      ##
      # postal | physical | both
      # Distinguishes between physical addresses (those you can visit) and mailing addresses (e.g. PO Boxes and care-of addresses). Most addresses are both.
      # The definition of Address states that "address is intended to describe postal addresses, not physical locations". However, many applications track whether an address has a dual purpose of being a location that can be visited as well as being a valid delivery destination, and Postal addresses are often used as proxies for physical locations (also see the [Location](location.html#) resource).
      'type' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/address-type'=>[ 'postal', 'physical', 'both' ]
        },
        'type'=>'code',
        'path'=>'Address.type',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/address-type'}
      },
      ##
      # Text representation of the address
      # Specifies the entire address as it should be displayed e.g. on a postal label. This may be provided instead of or as well as the specific parts.
      # Can provide both a text representation and parts. Applications updating an address SHALL ensure that  when both text and parts are present,  no content is included in the text that isn't found in a part.
      'text' => {
        'type'=>'string',
        'path'=>'Address.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Street name, number, direction & P.O. Box etc.
      # This component contains the house number, apartment number, street name, street direction,  P.O. Box number, delivery hints, and similar address information.
      'line' => {
        'type'=>'string',
        'path'=>'Address.line',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Name of city, town etc.
      # The name of the city, town, suburb, village or other community or delivery center.
      'city' => {
        'type'=>'string',
        'path'=>'Address.city',
        'min'=>0,
        'max'=>1
      },
      ##
      # District name (aka county)
      # The name of the administrative area (county).
      # District is sometimes known as county, but in some regions 'county' is used in place of city (municipality), so county name should be conveyed in city instead.
      'district' => {
        'type'=>'string',
        'path'=>'Address.district',
        'min'=>0,
        'max'=>1
      },
      ##
      # Sub-unit of country (abbreviations ok)
      # Sub-unit of a country with limited sovereignty in a federally organized country. A code may be used if codes are in common use (e.g. US 2 letter state codes).
      'state' => {
        'type'=>'string',
        'path'=>'Address.state',
        'min'=>0,
        'max'=>1
      },
      ##
      # Postal code for area
      # A postal code designating a region defined by the postal service.
      'postalCode' => {
        'type'=>'string',
        'path'=>'Address.postalCode',
        'min'=>0,
        'max'=>1
      },
      ##
      # Country (e.g. can be ISO 3166 2 or 3 letter code)
      # Country - a nation as commonly understood or generally accepted.
      # ISO 3166 3 letter codes can be used in place of a human readable country name.
      'country' => {
        'type'=>'string',
        'path'=>'Address.country',
        'min'=>0,
        'max'=>1
      },
      ##
      # Time period when address was/is in use.
      'period' => {
        'type'=>'Period',
        'path'=>'Address.period',
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
    # home | work | temp | old | billing - purpose of this address
    # The purpose of this address.
    # Applications can assume that an address is current unless it explicitly says that it is temporary or old.
    attr_accessor :use                            # 0-1 code
    ##
    # postal | physical | both
    # Distinguishes between physical addresses (those you can visit) and mailing addresses (e.g. PO Boxes and care-of addresses). Most addresses are both.
    # The definition of Address states that "address is intended to describe postal addresses, not physical locations". However, many applications track whether an address has a dual purpose of being a location that can be visited as well as being a valid delivery destination, and Postal addresses are often used as proxies for physical locations (also see the [Location](location.html#) resource).
    attr_accessor :type                           # 0-1 code
    ##
    # Text representation of the address
    # Specifies the entire address as it should be displayed e.g. on a postal label. This may be provided instead of or as well as the specific parts.
    # Can provide both a text representation and parts. Applications updating an address SHALL ensure that  when both text and parts are present,  no content is included in the text that isn't found in a part.
    attr_accessor :text                           # 0-1 string
    ##
    # Street name, number, direction & P.O. Box etc.
    # This component contains the house number, apartment number, street name, street direction,  P.O. Box number, delivery hints, and similar address information.
    attr_accessor :line                           # 0-* [ string ]
    ##
    # Name of city, town etc.
    # The name of the city, town, suburb, village or other community or delivery center.
    attr_accessor :city                           # 0-1 string
    ##
    # District name (aka county)
    # The name of the administrative area (county).
    # District is sometimes known as county, but in some regions 'county' is used in place of city (municipality), so county name should be conveyed in city instead.
    attr_accessor :district                       # 0-1 string
    ##
    # Sub-unit of country (abbreviations ok)
    # Sub-unit of a country with limited sovereignty in a federally organized country. A code may be used if codes are in common use (e.g. US 2 letter state codes).
    attr_accessor :state                          # 0-1 string
    ##
    # Postal code for area
    # A postal code designating a region defined by the postal service.
    attr_accessor :postalCode                     # 0-1 string
    ##
    # Country (e.g. can be ISO 3166 2 or 3 letter code)
    # Country - a nation as commonly understood or generally accepted.
    # ISO 3166 3 letter codes can be used in place of a human readable country name.
    attr_accessor :country                        # 0-1 string
    ##
    # Time period when address was/is in use.
    attr_accessor :period                         # 0-1 Period
  end
end
