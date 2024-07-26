module FHIR

  ##
  # Base StructureDefinition for Range Type: A set of ordered Quantities defined by a low and high limit.
  # Need to be able to specify ranges of values.
  class Range < FHIR::Model
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
        'path'=>'Range.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Range.extension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Low limit
      # The low limit. The boundary is inclusive.
      # If the low element is missing, the low boundary is not known.
      'low' => {
        'type'=>'Quantity',
        'path'=>'Range.low',
        'min'=>0,
        'max'=>1
      },
      ##
      # High limit
      # The high limit. The boundary is inclusive.
      # If the high element is missing, the high boundary is not known.
      'high' => {
        'type'=>'Quantity',
        'path'=>'Range.high',
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
    # Low limit
    # The low limit. The boundary is inclusive.
    # If the low element is missing, the low boundary is not known.
    attr_accessor :low                            # 0-1 Quantity
    ##
    # High limit
    # The high limit. The boundary is inclusive.
    # If the high element is missing, the high boundary is not known.
    attr_accessor :high                           # 0-1 Quantity
  end
end
