module FHIR

  ##
  # Base StructureDefinition for Duration Type: A length of time.
  class Duration < FHIR::Model
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
        'path'=>'Duration.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Duration.extension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Numerical value (with implicit precision)
      # The value of the measured amount. The value includes an implicit precision in the presentation of the value.
      # The implicit precision in the value should always be honored. Monetary values have their own rules for handling precision (refer to standard accounting text books).
      'value' => {
        'type'=>'decimal',
        'path'=>'Duration.value',
        'min'=>0,
        'max'=>1
      },
      ##
      # < | <= | >= | > - how to understand the value
      # How the value should be understood and represented - whether the actual value is greater or less than the stated value due to measurement issues; e.g. if the comparator is "<" , then the real value is < stated value.
      'comparator' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/quantity-comparator'=>[ '<', '<=', '>=', '>' ]
        },
        'type'=>'code',
        'path'=>'Duration.comparator',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/quantity-comparator'}
      },
      ##
      # Unit representation
      # A human-readable form of the unit.
      'unit' => {
        'type'=>'string',
        'path'=>'Duration.unit',
        'min'=>0,
        'max'=>1
      },
      ##
      # System that defines coded unit form
      # The identification of the system that provides the coded form of the unit.
      'system' => {
        'type'=>'uri',
        'path'=>'Duration.system',
        'min'=>0,
        'max'=>1
      },
      ##
      # Coded form of the unit
      # A computer processable form of the unit in some unit representation system.
      # The preferred system is UCUM, but SNOMED CT can also be used (for customary units) or ISO 4217 for currency.  The context of use may additionally require a code from a particular system.
      'code' => {
        'type'=>'code',
        'path'=>'Duration.code',
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
    # Numerical value (with implicit precision)
    # The value of the measured amount. The value includes an implicit precision in the presentation of the value.
    # The implicit precision in the value should always be honored. Monetary values have their own rules for handling precision (refer to standard accounting text books).
    attr_accessor :value                          # 0-1 decimal
    ##
    # < | <= | >= | > - how to understand the value
    # How the value should be understood and represented - whether the actual value is greater or less than the stated value due to measurement issues; e.g. if the comparator is "<" , then the real value is < stated value.
    attr_accessor :comparator                     # 0-1 code
    ##
    # Unit representation
    # A human-readable form of the unit.
    attr_accessor :unit                           # 0-1 string
    ##
    # System that defines coded unit form
    # The identification of the system that provides the coded form of the unit.
    attr_accessor :system                         # 0-1 uri
    ##
    # Coded form of the unit
    # A computer processable form of the unit in some unit representation system.
    # The preferred system is UCUM, but SNOMED CT can also be used (for customary units) or ISO 4217 for currency.  The context of use may additionally require a code from a particular system.
    attr_accessor :code                           # 0-1 code
  end
end
