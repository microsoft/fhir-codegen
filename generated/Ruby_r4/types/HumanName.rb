module FHIR

  ##
  # Base StructureDefinition for HumanName Type: A human's name with the ability to identify parts and usage.
  # Need to be able to record names, along with notes about their use.
  class HumanName < FHIR::Model
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
        'path'=>'HumanName.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'HumanName.extension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # usual | official | temp | nickname | anonymous | old | maiden
      # Identifies the purpose for this name.
      # Applications can assume that a name is current unless it explicitly says that it is temporary or old.
      'use' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/name-use'=>[ 'usual', 'official', 'temp', 'nickname', 'anonymous', 'old', 'maiden' ]
        },
        'type'=>'code',
        'path'=>'HumanName.use',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/name-use'}
      },
      ##
      # Text representation of the full name
      # Specifies the entire name as it should be displayed e.g. on an application UI. This may be provided instead of or as well as the specific parts.
      # Can provide both a text representation and parts. Applications updating a name SHALL ensure that when both text and parts are present,  no content is included in the text that isn't found in a part.
      'text' => {
        'type'=>'string',
        'path'=>'HumanName.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Family name (often called 'Surname')
      # The part of a name that links to the genealogy. In some cultures (e.g. Eritrea) the family name of a son is the first name of his father.
      # Family Name may be decomposed into specific parts using extensions (de, nl, es related cultures).
      'family' => {
        'type'=>'string',
        'path'=>'HumanName.family',
        'min'=>0,
        'max'=>1
      },
      ##
      # Given names (not always 'first'). Includes middle names
      # Given name.
      # If only initials are recorded, they may be used in place of the full name parts. Initials may be separated into multiple given names but often aren't due to paractical limitations.  This element is not called "first name" since given names do not always come first.
      'given' => {
        'type'=>'string',
        'path'=>'HumanName.given',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Parts that come before the name
      # Part of the name that is acquired as a title due to academic, legal, employment or nobility status, etc. and that appears at the start of the name.
      'prefix' => {
        'type'=>'string',
        'path'=>'HumanName.prefix',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Parts that come after the name
      # Part of the name that is acquired as a title due to academic, legal, employment or nobility status, etc. and that appears at the end of the name.
      'suffix' => {
        'type'=>'string',
        'path'=>'HumanName.suffix',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Time period when name was/is in use
      # Indicates the period of time when this name was valid for the named person.
      'period' => {
        'type'=>'Period',
        'path'=>'HumanName.period',
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
    # usual | official | temp | nickname | anonymous | old | maiden
    # Identifies the purpose for this name.
    # Applications can assume that a name is current unless it explicitly says that it is temporary or old.
    attr_accessor :use                            # 0-1 code
    ##
    # Text representation of the full name
    # Specifies the entire name as it should be displayed e.g. on an application UI. This may be provided instead of or as well as the specific parts.
    # Can provide both a text representation and parts. Applications updating a name SHALL ensure that when both text and parts are present,  no content is included in the text that isn't found in a part.
    attr_accessor :text                           # 0-1 string
    ##
    # Family name (often called 'Surname')
    # The part of a name that links to the genealogy. In some cultures (e.g. Eritrea) the family name of a son is the first name of his father.
    # Family Name may be decomposed into specific parts using extensions (de, nl, es related cultures).
    attr_accessor :family                         # 0-1 string
    ##
    # Given names (not always 'first'). Includes middle names
    # Given name.
    # If only initials are recorded, they may be used in place of the full name parts. Initials may be separated into multiple given names but often aren't due to paractical limitations.  This element is not called "first name" since given names do not always come first.
    attr_accessor :given                          # 0-* [ string ]
    ##
    # Parts that come before the name
    # Part of the name that is acquired as a title due to academic, legal, employment or nobility status, etc. and that appears at the start of the name.
    attr_accessor :prefix                         # 0-* [ string ]
    ##
    # Parts that come after the name
    # Part of the name that is acquired as a title due to academic, legal, employment or nobility status, etc. and that appears at the end of the name.
    attr_accessor :suffix                         # 0-* [ string ]
    ##
    # Time period when name was/is in use
    # Indicates the period of time when this name was valid for the named person.
    attr_accessor :period                         # 0-1 Period
  end
end
