module FHIR

  ##
  # Base StructureDefinition for TriggerDefinition Type: A description of a triggering event. Triggering events can be named events, data events, or periodic, as determined by the type element.
  class TriggerDefinition < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = []
    MULTIPLE_TYPES = {
      'timing[x]' => ['date', 'dateTime', 'Reference', 'Timing']
    }
    METADATA = {
      ##
      # Unique id for inter-element referencing
      # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
      'id' => {
        'type'=>'id',
        'path'=>'TriggerDefinition.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'TriggerDefinition.extension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # named-event | periodic | data-changed | data-added | data-modified | data-removed | data-accessed | data-access-ended
      # The type of triggering event.
      'type' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/trigger-type'=>[ 'named-event', 'periodic', 'data-changed', 'data-added', 'data-modified', 'data-removed', 'data-accessed', 'data-access-ended' ]
        },
        'type'=>'code',
        'path'=>'TriggerDefinition.type',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/trigger-type'}
      },
      ##
      # Name or URI that identifies the event
      # A formal name for the event. This may be an absolute URI that identifies the event formally (e.g. from a trigger registry), or a simple relative URI that identifies the event in a local context.
      # An event name can be provided for all event types, but is required for named events. If a name is provided for a type other than named events, it is considered to be a shorthand for the semantics described by the formal description of the event.
      'name' => {
        'type'=>'string',
        'path'=>'TriggerDefinition.name',
        'min'=>0,
        'max'=>1
      },
      ##
      # Timing of the event
      # The timing of the event (if this is a periodic trigger).
      'timingDate' => {
        'type'=>'Date',
        'path'=>'TriggerDefinition.timing[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Timing of the event
      # The timing of the event (if this is a periodic trigger).
      'timingDateTime' => {
        'type'=>'DateTime',
        'path'=>'TriggerDefinition.timing[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Timing of the event
      # The timing of the event (if this is a periodic trigger).
      'timingReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Schedule'],
        'type'=>'Reference',
        'path'=>'TriggerDefinition.timing[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Timing of the event
      # The timing of the event (if this is a periodic trigger).
      'timingTiming' => {
        'type'=>'Timing',
        'path'=>'TriggerDefinition.timing[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Triggering data of the event (multiple = 'and')
      # The triggering data of the event (if this is a data trigger). If more than one data is requirement is specified, then all the data requirements must be true.
      # This element shall be present for any data type trigger.
      'data' => {
        'type'=>'DataRequirement',
        'path'=>'TriggerDefinition.data',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Whether the event triggers (boolean expression)
      # A boolean-valued expression that is evaluated in the context of the container of the trigger definition and returns whether or not the trigger fires.
      # This element can be only be specified for data type triggers and provides additional semantics for the trigger. The context available within the condition is based on the type of data event. For all events, the current resource will be available as context. In addition, for modification events, the previous resource will also be available. The expression may be inlined, or may be a simple absolute URI, which is a reference to a named expression within a logic library referenced by a library element or extension within the containing resource. If the expression is a FHIR Path expression, it evaluates in the context of a resource of one of the type identified in the data requirement, and may also refer to the variable %previous for delta comparisons on events of type data-changed, data-modified, and data-deleted which will always have the same type.
      'condition' => {
        'type'=>'Expression',
        'path'=>'TriggerDefinition.condition',
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
    # named-event | periodic | data-changed | data-added | data-modified | data-removed | data-accessed | data-access-ended
    # The type of triggering event.
    attr_accessor :type                           # 1-1 code
    ##
    # Name or URI that identifies the event
    # A formal name for the event. This may be an absolute URI that identifies the event formally (e.g. from a trigger registry), or a simple relative URI that identifies the event in a local context.
    # An event name can be provided for all event types, but is required for named events. If a name is provided for a type other than named events, it is considered to be a shorthand for the semantics described by the formal description of the event.
    attr_accessor :name                           # 0-1 string
    ##
    # Timing of the event
    # The timing of the event (if this is a periodic trigger).
    attr_accessor :timingDate                     # 0-1 Date
    ##
    # Timing of the event
    # The timing of the event (if this is a periodic trigger).
    attr_accessor :timingDateTime                 # 0-1 DateTime
    ##
    # Timing of the event
    # The timing of the event (if this is a periodic trigger).
    attr_accessor :timingReference                # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Schedule)
    ##
    # Timing of the event
    # The timing of the event (if this is a periodic trigger).
    attr_accessor :timingTiming                   # 0-1 Timing
    ##
    # Triggering data of the event (multiple = 'and')
    # The triggering data of the event (if this is a data trigger). If more than one data is requirement is specified, then all the data requirements must be true.
    # This element shall be present for any data type trigger.
    attr_accessor :data                           # 0-* [ DataRequirement ]
    ##
    # Whether the event triggers (boolean expression)
    # A boolean-valued expression that is evaluated in the context of the container of the trigger definition and returns whether or not the trigger fires.
    # This element can be only be specified for data type triggers and provides additional semantics for the trigger. The context available within the condition is based on the type of data event. For all events, the current resource will be available as context. In addition, for modification events, the previous resource will also be available. The expression may be inlined, or may be a simple absolute URI, which is a reference to a named expression within a logic library referenced by a library element or extension within the containing resource. If the expression is a FHIR Path expression, it evaluates in the context of a resource of one of the type identified in the data requirement, and may also refer to the variable %previous for delta comparisons on events of type data-changed, data-modified, and data-deleted which will always have the same type.
    attr_accessor :condition                      # 0-1 Expression
  end
end
