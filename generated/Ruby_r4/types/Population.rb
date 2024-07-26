module FHIR

  ##
  # Base StructureDefinition for Population Type: A populatioof people with some set of grouping criteria.
  class Population < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = []
    MULTIPLE_TYPES = {
      'age[x]' => ['CodeableConcept', 'Range']
    }
    METADATA = {
      ##
      # Unique id for inter-element referencing
      # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
      'id' => {
        'type'=>'id',
        'path'=>'Population.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Population.extension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Extensions that cannot be ignored even if unrecognized
      # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
      # 
      # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'modifierExtension' => {
        'type'=>'Extension',
        'path'=>'Population.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The age of the specific population.
      'ageCodeableConcept' => {
        'type'=>'CodeableConcept',
        'path'=>'Population.age[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # The age of the specific population.
      'ageRange' => {
        'type'=>'Range',
        'path'=>'Population.age[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # The gender of the specific population.
      'gender' => {
        'type'=>'CodeableConcept',
        'path'=>'Population.gender',
        'min'=>0,
        'max'=>1
      },
      ##
      # Race of the specific population.
      'race' => {
        'type'=>'CodeableConcept',
        'path'=>'Population.race',
        'min'=>0,
        'max'=>1
      },
      ##
      # The existing physiological conditions of the specific population to which this applies.
      'physiologicalCondition' => {
        'type'=>'CodeableConcept',
        'path'=>'Population.physiologicalCondition',
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
    # Extensions that cannot be ignored even if unrecognized
    # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
    # 
    # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
    # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
    attr_accessor :modifierExtension              # 0-* [ Extension ]
    ##
    # The age of the specific population.
    attr_accessor :ageCodeableConcept             # 0-1 CodeableConcept
    ##
    # The age of the specific population.
    attr_accessor :ageRange                       # 0-1 Range
    ##
    # The gender of the specific population.
    attr_accessor :gender                         # 0-1 CodeableConcept
    ##
    # Race of the specific population.
    attr_accessor :race                           # 0-1 CodeableConcept
    ##
    # The existing physiological conditions of the specific population to which this applies.
    attr_accessor :physiologicalCondition         # 0-1 CodeableConcept
  end
end
