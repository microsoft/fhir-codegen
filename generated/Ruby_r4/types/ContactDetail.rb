module FHIR

  ##
  # Base StructureDefinition for ContactDetail Type: Specifies contact information for a person or organization.
  # Need to track contact information in the same way across multiple resources.
  class ContactDetail < FHIR::Model
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
        'path'=>'ContactDetail.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'ContactDetail.extension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Name of an individual to contact
      # The name of an individual to contact.
      # If there is no named individual, the telecom information is for the organization as a whole.
      'name' => {
        'type'=>'string',
        'path'=>'ContactDetail.name',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contact details for individual or organization
      # The contact details for the individual (if a name was provided) or the organization.
      'telecom' => {
        'type'=>'ContactPoint',
        'path'=>'ContactDetail.telecom',
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
    # Name of an individual to contact
    # The name of an individual to contact.
    # If there is no named individual, the telecom information is for the organization as a whole.
    attr_accessor :name                           # 0-1 string
    ##
    # Contact details for individual or organization
    # The contact details for the individual (if a name was provided) or the organization.
    attr_accessor :telecom                        # 0-* [ ContactPoint ]
  end
end
