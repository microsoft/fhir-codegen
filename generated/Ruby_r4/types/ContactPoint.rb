module FHIR

  ##
  # Base StructureDefinition for ContactPoint Type: Details for all kinds of technology mediated contact points for a person or organization, including telephone, email, etc.
  # Need to track phone, fax, mobile, sms numbers, email addresses, twitter tags, etc.
  class ContactPoint < FHIR::Model
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
        'path'=>'ContactPoint.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'ContactPoint.extension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # phone | fax | email | pager | url | sms | other
      # Telecommunications form for contact point - what communications system is required to make use of the contact.
      'system' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/contact-point-system'=>[ 'phone', 'fax', 'email', 'pager', 'url', 'sms', 'other' ]
        },
        'type'=>'code',
        'path'=>'ContactPoint.system',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/contact-point-system'}
      },
      ##
      # The actual contact point details, in a form that is meaningful to the designated communication system (i.e. phone number or email address).
      # Additional text data such as phone extension numbers, or notes about use of the contact are sometimes included in the value.
      'value' => {
        'type'=>'string',
        'path'=>'ContactPoint.value',
        'min'=>0,
        'max'=>1
      },
      ##
      # home | work | temp | old | mobile - purpose of this contact point
      # Identifies the purpose for the contact point.
      # Applications can assume that a contact is current unless it explicitly says that it is temporary or old.
      'use' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/contact-point-use'=>[ 'home', 'work', 'temp', 'old', 'mobile' ]
        },
        'type'=>'code',
        'path'=>'ContactPoint.use',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/contact-point-use'}
      },
      ##
      # Specify preferred order of use (1 = highest)
      # Specifies a preferred order in which to use a set of contacts. ContactPoints with lower rank values are more preferred than those with higher rank values.
      # Note that rank does not necessarily follow the order in which the contacts are represented in the instance.
      'rank' => {
        'type'=>'positiveInt',
        'path'=>'ContactPoint.rank',
        'min'=>0,
        'max'=>1
      },
      ##
      # Time period when the contact point was/is in use.
      'period' => {
        'type'=>'Period',
        'path'=>'ContactPoint.period',
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
    # phone | fax | email | pager | url | sms | other
    # Telecommunications form for contact point - what communications system is required to make use of the contact.
    attr_accessor :system                         # 0-1 code
    ##
    # The actual contact point details, in a form that is meaningful to the designated communication system (i.e. phone number or email address).
    # Additional text data such as phone extension numbers, or notes about use of the contact are sometimes included in the value.
    attr_accessor :value                          # 0-1 string
    ##
    # home | work | temp | old | mobile - purpose of this contact point
    # Identifies the purpose for the contact point.
    # Applications can assume that a contact is current unless it explicitly says that it is temporary or old.
    attr_accessor :use                            # 0-1 code
    ##
    # Specify preferred order of use (1 = highest)
    # Specifies a preferred order in which to use a set of contacts. ContactPoints with lower rank values are more preferred than those with higher rank values.
    # Note that rank does not necessarily follow the order in which the contacts are represented in the instance.
    attr_accessor :rank                           # 0-1 positiveInt
    ##
    # Time period when the contact point was/is in use.
    attr_accessor :period                         # 0-1 Period
  end
end
