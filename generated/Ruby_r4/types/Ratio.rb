module FHIR

  ##
  # Base StructureDefinition for Ratio Type: A relationship of two Quantity values - expressed as a numerator and a denominator.
  # Need to able to capture ratios for some measurements (titers) and some rates (costs).
  class Ratio < FHIR::Model
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
        'path'=>'Ratio.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Ratio.extension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Numerator value
      # The value of the numerator.
      'numerator' => {
        'type'=>'Quantity',
        'path'=>'Ratio.numerator',
        'min'=>0,
        'max'=>1
      },
      ##
      # Denominator value
      # The value of the denominator.
      'denominator' => {
        'type'=>'Quantity',
        'path'=>'Ratio.denominator',
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
    # Numerator value
    # The value of the numerator.
    attr_accessor :numerator                      # 0-1 Quantity
    ##
    # Denominator value
    # The value of the denominator.
    attr_accessor :denominator                    # 0-1 Quantity
  end
end
