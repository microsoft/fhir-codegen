module FHIR

  ##
  # Base StructureDefinition for UsageContext Type: Specifies clinical/business/etc. metadata that can be used to retrieve, index and/or categorize an artifact. This metadata can either be specific to the applicable population (e.g., age category, DRG) or the specific context of care (e.g., venue, care setting, provider of care).
  # Consumers of the resource must be able to determine the intended applicability for the resource. Ideally, this information would be used programmatically to determine when and how it should be incorporated or exposed.
  class UsageContext < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = []
    MULTIPLE_TYPES = {
      'value[x]' => ['CodeableConcept', 'Quantity', 'Range', 'Reference']
    }
    METADATA = {
      ##
      # Unique id for inter-element referencing
      # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
      'id' => {
        'type'=>'string',
        'path'=>'UsageContext.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'UsageContext.extension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Type of context being specified
      # A code that identifies the type of context being specified by this usage context.
      'code' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/usage-context-type'=>[ 'gender', 'age', 'focus', 'user', 'workflow', 'task', 'venue', 'species', 'program' ]
        },
        'type'=>'Coding',
        'path'=>'UsageContext.code',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/usage-context-type'}
      },
      ##
      # Value that defines the context
      # A value that defines the context specified in this context of use. The interpretation of the value is defined by the code.
      'valueCodeableConcept' => {
        'type'=>'CodeableConcept',
        'path'=>'UsageContext.value[x]',
        'min'=>1,
        'max'=>1
      }
      ##
      # Value that defines the context
      # A value that defines the context specified in this context of use. The interpretation of the value is defined by the code.
      'valueQuantity' => {
        'type'=>'Quantity',
        'path'=>'UsageContext.value[x]',
        'min'=>1,
        'max'=>1
      }
      ##
      # Value that defines the context
      # A value that defines the context specified in this context of use. The interpretation of the value is defined by the code.
      'valueRange' => {
        'type'=>'Range',
        'path'=>'UsageContext.value[x]',
        'min'=>1,
        'max'=>1
      }
      ##
      # Value that defines the context
      # A value that defines the context specified in this context of use. The interpretation of the value is defined by the code.
      'valueReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/PlanDefinition', 'http://hl7.org/fhir/StructureDefinition/ResearchStudy', 'http://hl7.org/fhir/StructureDefinition/InsurancePlan', 'http://hl7.org/fhir/StructureDefinition/HealthcareService', 'http://hl7.org/fhir/StructureDefinition/Group', 'http://hl7.org/fhir/StructureDefinition/Location', 'http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'UsageContext.value[x]',
        'min'=>1,
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
    # Type of context being specified
    # A code that identifies the type of context being specified by this usage context.
    attr_accessor :code                           # 1-1 Coding
    ##
    # Value that defines the context
    # A value that defines the context specified in this context of use. The interpretation of the value is defined by the code.
    attr_accessor :valueCodeableConcept           # 1-1 CodeableConcept
    ##
    # Value that defines the context
    # A value that defines the context specified in this context of use. The interpretation of the value is defined by the code.
    attr_accessor :valueQuantity                  # 1-1 Quantity
    ##
    # Value that defines the context
    # A value that defines the context specified in this context of use. The interpretation of the value is defined by the code.
    attr_accessor :valueRange                     # 1-1 Range
    ##
    # Value that defines the context
    # A value that defines the context specified in this context of use. The interpretation of the value is defined by the code.
    attr_accessor :valueReference                 # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/PlanDefinition|http://hl7.org/fhir/StructureDefinition/ResearchStudy|http://hl7.org/fhir/StructureDefinition/InsurancePlan|http://hl7.org/fhir/StructureDefinition/HealthcareService|http://hl7.org/fhir/StructureDefinition/Group|http://hl7.org/fhir/StructureDefinition/Location|http://hl7.org/fhir/StructureDefinition/Organization)
  end
end
