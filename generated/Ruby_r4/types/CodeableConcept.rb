module FHIR

  ##
  # Base StructureDefinition for CodeableConcept Type: A concept that may be defined by a formal reference to a terminology or ontology or may be provided by text.
  # This is a common pattern in healthcare - a concept that may be defined by one or more codes from formal definitions including LOINC and SNOMED CT, and/or defined by the provision of text that captures a human sense of the concept.
  class CodeableConcept < FHIR::Model
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
        'path'=>'CodeableConcept.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'CodeableConcept.extension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Code defined by a terminology system
      # A reference to a code defined by a terminology system.
      # Codes may be defined very casually in enumerations, or code lists, up to very formal definitions such as SNOMED CT - see the HL7 v3 Core Principles for more information.  Ordering of codings is undefined and SHALL NOT be used to infer meaning. Generally, at most only one of the coding values will be labeled as UserSelected = true.
      'coding' => {
        'type'=>'Coding',
        'path'=>'CodeableConcept.coding',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Plain text representation of the concept
      # A human language representation of the concept as seen/selected/uttered by the user who entered the data and/or which represents the intended meaning of the user.
      # Very often the text is the same as a displayName of one of the codings.
      'text' => {
        'type'=>'string',
        'path'=>'CodeableConcept.text',
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
    # Code defined by a terminology system
    # A reference to a code defined by a terminology system.
    # Codes may be defined very casually in enumerations, or code lists, up to very formal definitions such as SNOMED CT - see the HL7 v3 Core Principles for more information.  Ordering of codings is undefined and SHALL NOT be used to infer meaning. Generally, at most only one of the coding values will be labeled as UserSelected = true.
    attr_accessor :coding                         # 0-* [ Coding ]
    ##
    # Plain text representation of the concept
    # A human language representation of the concept as seen/selected/uttered by the user who entered the data and/or which represents the intended meaning of the user.
    # Very often the text is the same as a displayName of one of the codings.
    attr_accessor :text                           # 0-1 string
  end
end
