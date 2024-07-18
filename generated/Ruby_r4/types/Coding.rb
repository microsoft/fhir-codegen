module FHIR

  ##
  # Base StructureDefinition for Coding Type: A reference to a code defined by a terminology system.
  # References to codes are very common in healthcare models.
  class Coding < FHIR::Model
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
        'path'=>'Coding.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Coding.extension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Identity of the terminology system
      # The identification of the code system that defines the meaning of the symbol in the code.
      # The URI may be an OID (urn:oid:...) or a UUID (urn:uuid:...).  OIDs and UUIDs SHALL be references to the HL7 OID registry. Otherwise, the URI should come from HL7's list of FHIR defined special URIs or it should reference to some definition that establishes the system clearly and unambiguously.
      'system' => {
        'type'=>'uri',
        'path'=>'Coding.system',
        'min'=>0,
        'max'=>1
      },
      ##
      # Version of the system - if relevant
      # The version of the code system which was used when choosing this code. Note that a well-maintained code system does not need the version reported, because the meaning of codes is consistent across versions. However this cannot consistently be assured, and when the meaning is not guaranteed to be consistent, the version SHOULD be exchanged.
      # Where the terminology does not clearly define what string should be used to identify code system versions, the recommendation is to use the date (expressed in FHIR date format) on which that version was officially published as the version date.
      'version' => {
        'type'=>'string',
        'path'=>'Coding.version',
        'min'=>0,
        'max'=>1
      },
      ##
      # Symbol in syntax defined by the system
      # A symbol in syntax defined by the system. The symbol may be a predefined code or an expression in a syntax defined by the coding system (e.g. post-coordination).
      'code' => {
        'type'=>'code',
        'path'=>'Coding.code',
        'min'=>0,
        'max'=>1
      },
      ##
      # Representation defined by the system
      # A representation of the meaning of the code in the system, following the rules of the system.
      'display' => {
        'type'=>'string',
        'path'=>'Coding.display',
        'min'=>0,
        'max'=>1
      },
      ##
      # If this coding was chosen directly by the user
      # Indicates that this coding was chosen by a user directly - e.g. off a pick list of available items (codes or displays).
      # Amongst a set of alternatives, a directly chosen code is the most appropriate starting point for new translations. There is some ambiguity about what exactly 'directly chosen' implies, and trading partner agreement may be needed to clarify the use of this element and its consequences more completely.
      'userSelected' => {
        'type'=>'boolean',
        'path'=>'Coding.userSelected',
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
    # Identity of the terminology system
    # The identification of the code system that defines the meaning of the symbol in the code.
    # The URI may be an OID (urn:oid:...) or a UUID (urn:uuid:...).  OIDs and UUIDs SHALL be references to the HL7 OID registry. Otherwise, the URI should come from HL7's list of FHIR defined special URIs or it should reference to some definition that establishes the system clearly and unambiguously.
    attr_accessor :system                         # 0-1 uri
    ##
    # Version of the system - if relevant
    # The version of the code system which was used when choosing this code. Note that a well-maintained code system does not need the version reported, because the meaning of codes is consistent across versions. However this cannot consistently be assured, and when the meaning is not guaranteed to be consistent, the version SHOULD be exchanged.
    # Where the terminology does not clearly define what string should be used to identify code system versions, the recommendation is to use the date (expressed in FHIR date format) on which that version was officially published as the version date.
    attr_accessor :version                        # 0-1 string
    ##
    # Symbol in syntax defined by the system
    # A symbol in syntax defined by the system. The symbol may be a predefined code or an expression in a syntax defined by the coding system (e.g. post-coordination).
    attr_accessor :code                           # 0-1 code
    ##
    # Representation defined by the system
    # A representation of the meaning of the code in the system, following the rules of the system.
    attr_accessor :display                        # 0-1 string
    ##
    # If this coding was chosen directly by the user
    # Indicates that this coding was chosen by a user directly - e.g. off a pick list of available items (codes or displays).
    # Amongst a set of alternatives, a directly chosen code is the most appropriate starting point for new translations. There is some ambiguity about what exactly 'directly chosen' implies, and trading partner agreement may be needed to clarify the use of this element and its consequences more completely.
    attr_accessor :userSelected                   # 0-1 boolean
  end
end
