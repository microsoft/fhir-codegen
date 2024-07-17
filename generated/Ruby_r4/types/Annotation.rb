module FHIR

  ##
  # Base StructureDefinition for Annotation Type: A  text note which also  contains information about who made the statement and when.
  class Annotation < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = []
    MULTIPLE_TYPES = {
      'author[x]' => ['Reference', 'string']
    }
    METADATA = {
      ##
      # Unique id for inter-element referencing
      # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
      'id' => {
        'type'=>'string',
        'path'=>'Annotation.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Annotation.extension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Individual responsible for the annotation
      # The individual responsible for making the annotation.
      # Organization is used when there's no need for specific attribution as to who made the comment.
      'authorReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'Annotation.author[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Individual responsible for the annotation
      # The individual responsible for making the annotation.
      # Organization is used when there's no need for specific attribution as to who made the comment.
      'authorString' => {
        'type'=>'String',
        'path'=>'Annotation.author[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the annotation was made
      # Indicates when this particular annotation was made.
      'time' => {
        'type'=>'dateTime',
        'path'=>'Annotation.time',
        'min'=>0,
        'max'=>1
      },
      ##
      # The annotation  - text content (as markdown)
      # The text of the annotation in markdown format.
      'text' => {
        'type'=>'markdown',
        'path'=>'Annotation.text',
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
    # Individual responsible for the annotation
    # The individual responsible for making the annotation.
    # Organization is used when there's no need for specific attribution as to who made the comment.
    attr_accessor :authorReference                # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Individual responsible for the annotation
    # The individual responsible for making the annotation.
    # Organization is used when there's no need for specific attribution as to who made the comment.
    attr_accessor :authorString                   # 0-1 String
    ##
    # When the annotation was made
    # Indicates when this particular annotation was made.
    attr_accessor :time                           # 0-1 dateTime
    ##
    # The annotation  - text content (as markdown)
    # The text of the annotation in markdown format.
    attr_accessor :text                           # 1-1 markdown
  end
end
