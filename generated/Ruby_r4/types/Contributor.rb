module FHIR

  ##
  # Base StructureDefinition for Contributor Type: A contributor to the content of a knowledge asset, including authors, editors, reviewers, and endorsers.
  # Need to track contributor information in the same way across multiple resources.
  class Contributor < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = []
    METADATA = {
      ##
      # Unique id for inter-element referencing
      # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
      'id' => {
        'type'=>'string',
        'path'=>'Contributor.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Contributor.extension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # author | editor | reviewer | endorser
      # The type of contributor.
      'type' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/contributor-type'=>[ 'author', 'editor', 'reviewer', 'endorser' ]
        },
        'type'=>'code',
        'path'=>'Contributor.type',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/contributor-type'}
      },
      ##
      # Who contributed the content
      # The name of the individual or organization responsible for the contribution.
      'name' => {
        'type'=>'string',
        'path'=>'Contributor.name',
        'min'=>1,
        'max'=>1
      },
      ##
      # Contact details of the contributor
      # Contact details to assist a user in finding and communicating with the contributor.
      'contact' => {
        'type'=>'ContactDetail',
        'path'=>'Contributor.contact',
        'min'=>0,
        'max'=>Float::INFINITY
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
    # author | editor | reviewer | endorser
    # The type of contributor.
    attr_accessor :type                           # 1-1 code
    ##
    # Who contributed the content
    # The name of the individual or organization responsible for the contribution.
    attr_accessor :name                           # 1-1 string
    ##
    # Contact details of the contributor
    # Contact details to assist a user in finding and communicating with the contributor.
    attr_accessor :contact                        # 0-* [ ContactDetail ]
  end
end
