module FHIR

  ##
  # Base StructureDefinition for Identifier Type: An identifier - identifies some entity uniquely and unambiguously. Typically this is used for business identifiers.
  # Need to be able to identify things with confidence and be sure that the identification is not subject to misinterpretation.
  class Identifier < FHIR::Model
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
        'path'=>'Identifier.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Identifier.extension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # usual | official | temp | secondary | old (If known)
      # The purpose of this identifier.
      # Applications can assume that an identifier is permanent unless it explicitly says that it is temporary.
      'use' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/identifier-use'=>[ 'usual', 'official', 'temp', 'secondary', 'old' ]
        },
        'type'=>'code',
        'path'=>'Identifier.use',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/identifier-use'}
      },
      ##
      # Description of identifier
      # A coded type for the identifier that can be used to determine which identifier to use for a specific purpose.
      # This element deals only with general categories of identifiers.  It SHOULD not be used for codes that correspond 1..1 with the Identifier.system. Some identifiers may fall into multiple categories due to common usage.   Where the system is known, a type is unnecessary because the type is always part of the system definition. However systems often need to handle identifiers where the system is not known. There is not a 1:1 relationship between type and system, since many different systems have the same type.
      'type' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/v2-0203'=>[ 'DL', 'PPN', 'BRN', 'MR', 'MCN', 'EN', 'TAX', 'NIIP', 'PRN', 'MD', 'DR', 'ACSN', 'UDI', 'SNO', 'SB', 'PLAC', 'FILL', 'JHN' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Identifier.type',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/identifier-type'}
      },
      ##
      # The namespace for the identifier value
      # Establishes the namespace for the value - that is, a URL that describes a set values that are unique.
      # Identifier.system is always case sensitive.
      'system' => {
        'type'=>'uri',
        'path'=>'Identifier.system',
        'min'=>0,
        'max'=>1
      },
      ##
      # The value that is unique
      # The portion of the identifier typically relevant to the user and which is unique within the context of the system.
      # If the value is a full URI, then the system SHALL be urn:ietf:rfc:3986.  The value's primary purpose is computational mapping.  As a result, it may be normalized for comparison purposes (e.g. removing non-significant whitespace, dashes, etc.)  A value formatted for human display can be conveyed using the [Rendered Value extension](extension-rendered-value.html). Identifier.value is to be treated as case sensitive unless knowledge of the Identifier.system allows the processer to be confident that non-case-sensitive processing is safe.
      'value' => {
        'type'=>'string',
        'path'=>'Identifier.value',
        'min'=>0,
        'max'=>1
      },
      ##
      # Time period when id is/was valid for use
      # Time period during which identifier is/was valid for use.
      'period' => {
        'type'=>'Period',
        'path'=>'Identifier.period',
        'min'=>0,
        'max'=>1
      },
      ##
      # Organization that issued id (may be just text)
      # Organization that issued/manages the identifier.
      # The Identifier.assigner may omit the .reference element and only contain a .display element reflecting the name or other textual information about the assigning organization.
      'assigner' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'Identifier.assigner',
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
    # usual | official | temp | secondary | old (If known)
    # The purpose of this identifier.
    # Applications can assume that an identifier is permanent unless it explicitly says that it is temporary.
    attr_accessor :use                            # 0-1 code
    ##
    # Description of identifier
    # A coded type for the identifier that can be used to determine which identifier to use for a specific purpose.
    # This element deals only with general categories of identifiers.  It SHOULD not be used for codes that correspond 1..1 with the Identifier.system. Some identifiers may fall into multiple categories due to common usage.   Where the system is known, a type is unnecessary because the type is always part of the system definition. However systems often need to handle identifiers where the system is not known. There is not a 1:1 relationship between type and system, since many different systems have the same type.
    attr_accessor :type                           # 0-1 CodeableConcept
    ##
    # The namespace for the identifier value
    # Establishes the namespace for the value - that is, a URL that describes a set values that are unique.
    # Identifier.system is always case sensitive.
    attr_accessor :system                         # 0-1 uri
    ##
    # The value that is unique
    # The portion of the identifier typically relevant to the user and which is unique within the context of the system.
    # If the value is a full URI, then the system SHALL be urn:ietf:rfc:3986.  The value's primary purpose is computational mapping.  As a result, it may be normalized for comparison purposes (e.g. removing non-significant whitespace, dashes, etc.)  A value formatted for human display can be conveyed using the [Rendered Value extension](extension-rendered-value.html). Identifier.value is to be treated as case sensitive unless knowledge of the Identifier.system allows the processer to be confident that non-case-sensitive processing is safe.
    attr_accessor :value                          # 0-1 string
    ##
    # Time period when id is/was valid for use
    # Time period during which identifier is/was valid for use.
    attr_accessor :period                         # 0-1 Period
    ##
    # Organization that issued id (may be just text)
    # Organization that issued/manages the identifier.
    # The Identifier.assigner may omit the .reference element and only contain a .display element reflecting the name or other textual information about the assigning organization.
    attr_accessor :assigner                       # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
  end
end
