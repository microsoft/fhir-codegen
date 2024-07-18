module FHIR

  ##
  # Base StructureDefinition for Timing Type: Specifies an event that may occur multiple times. Timing schedules are used to record when things are planned, expected or requested to occur. The most common usage is in dosage instructions for medications. They are also used when planning care of various kinds, and may be used for reporting the schedule to which past regular activities were carried out.
  # Need to able to track proposed timing schedules. There are several different ways to do this: one or more specified times, a simple rules like three times a day, or  before/after meals.
  class Timing < FHIR::Model
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
        'path'=>'Timing.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Timing.extension',
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
        'path'=>'Timing.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # When the event occurs
      # Identifies specific times when the event occurs.
      'event' => {
        'type'=>'dateTime',
        'path'=>'Timing.event',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # When the event is to occur
      # A set of rules that describe when the event is scheduled.
      'repeat' => {
        'type'=>'Timing::Repeat',
        'path'=>'Timing.repeat',
        'min'=>0,
        'max'=>1
      },
      ##
      # BID | TID | QID | AM | PM | QD | QOD | +
      # A code for the timing schedule (or just text in code.text). Some codes such as BID are ubiquitous, but many institutions define their own additional codes. If a code is provided, the code is understood to be a complete statement of whatever is specified in the structured timing data, and either the code or the data may be used to interpret the Timing, with the exception that .repeat.bounds still applies over the code (and is not contained in the code).
      # BID etc. are defined as 'at institutionally specified times'. For example, an institution may choose that BID is "always at 7am and 6pm".  If it is inappropriate for this choice to be made, the code BID should not be used. Instead, a distinct organization-specific code should be used in place of the HL7-defined BID code and/or a structured representation should be used (in this case, specifying the two event times).
      'code' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/v3-GTSAbbreviation'=>[ 'BID', 'TID', 'QID', 'AM', 'PM', 'QD', 'QOD', 'Q1H', 'Q2H', 'Q3H', 'Q4H', 'Q6H', 'Q8H', 'BED', 'WK', 'MO' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Timing.code',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/timing-abbreviation'}
      }
    }

    ##
    # When the event is to occur
    # A set of rules that describe when the event is scheduled.
    class Repeat < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'bounds[x]' => ['Duration', 'Period', 'Range']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Repeat.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Repeat.extension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Length/Range of lengths, or (Start and/or end) limits
        # Either a duration for the length of the timing schedule, a range of possible length, or outer bounds for start and/or end limits of the timing schedule.
        'boundsDuration' => {
          'type'=>'Duration',
          'path'=>'Repeat.bounds[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Length/Range of lengths, or (Start and/or end) limits
        # Either a duration for the length of the timing schedule, a range of possible length, or outer bounds for start and/or end limits of the timing schedule.
        'boundsPeriod' => {
          'type'=>'Period',
          'path'=>'Repeat.bounds[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Length/Range of lengths, or (Start and/or end) limits
        # Either a duration for the length of the timing schedule, a range of possible length, or outer bounds for start and/or end limits of the timing schedule.
        'boundsRange' => {
          'type'=>'Range',
          'path'=>'Repeat.bounds[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Number of times to repeat
        # A total count of the desired number of repetitions across the duration of the entire timing specification. If countMax is present, this element indicates the lower bound of the allowed range of count values.
        # If you have both bounds and count, then this should be understood as within the bounds period, until count times happens.
        'count' => {
          'type'=>'positiveInt',
          'path'=>'Repeat.count',
          'min'=>0,
          'max'=>1
        },
        ##
        # Maximum number of times to repeat
        # If present, indicates that the count is a range - so to perform the action between [count] and [countMax] times.
        'countMax' => {
          'type'=>'positiveInt',
          'path'=>'Repeat.countMax',
          'min'=>0,
          'max'=>1
        },
        ##
        # How long when it happens
        # How long this thing happens for when it happens. If durationMax is present, this element indicates the lower bound of the allowed range of the duration.
        # For some events the duration is part of the definition of the event (e.g. IV infusions, where the duration is implicit in the specified quantity and rate). For others, it's part of the timing specification (e.g. exercise).
        'duration' => {
          'type'=>'decimal',
          'path'=>'Repeat.duration',
          'min'=>0,
          'max'=>1
        },
        ##
        # How long when it happens (Max)
        # If present, indicates that the duration is a range - so to perform the action between [duration] and [durationMax] time length.
        # For some events the duration is part of the definition of the event (e.g. IV infusions, where the duration is implicit in the specified quantity and rate). For others, it's part of the timing specification (e.g. exercise).
        'durationMax' => {
          'type'=>'decimal',
          'path'=>'Repeat.durationMax',
          'min'=>0,
          'max'=>1
        },
        ##
        # s | min | h | d | wk | mo | a - unit of time (UCUM)
        # The units of time for the duration, in UCUM units.
        'durationUnit' => {
          'valid_codes'=>{
            'http://unitsofmeasure.org'=>[ 's', 'min', 'h', 'd', 'wk', 'mo', 'a' ]
          },
          'type'=>'code',
          'path'=>'Repeat.durationUnit',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/units-of-time'}
        },
        ##
        # Event occurs frequency times per period
        # The number of times to repeat the action within the specified period. If frequencyMax is present, this element indicates the lower bound of the allowed range of the frequency.
        'frequency' => {
          'type'=>'positiveInt',
          'path'=>'Repeat.frequency',
          'min'=>0,
          'max'=>1
        },
        ##
        # Event occurs up to frequencyMax times per period
        # If present, indicates that the frequency is a range - so to repeat between [frequency] and [frequencyMax] times within the period or period range.
        'frequencyMax' => {
          'type'=>'positiveInt',
          'path'=>'Repeat.frequencyMax',
          'min'=>0,
          'max'=>1
        },
        ##
        # Event occurs frequency times per period
        # Indicates the duration of time over which repetitions are to occur; e.g. to express "3 times per day", 3 would be the frequency and "1 day" would be the period. If periodMax is present, this element indicates the lower bound of the allowed range of the period length.
        'period' => {
          'type'=>'decimal',
          'path'=>'Repeat.period',
          'min'=>0,
          'max'=>1
        },
        ##
        # Upper limit of period (3-4 hours)
        # If present, indicates that the period is a range from [period] to [periodMax], allowing expressing concepts such as "do this once every 3-5 days.
        'periodMax' => {
          'type'=>'decimal',
          'path'=>'Repeat.periodMax',
          'min'=>0,
          'max'=>1
        },
        ##
        # s | min | h | d | wk | mo | a - unit of time (UCUM)
        # The units of time for the period in UCUM units.
        'periodUnit' => {
          'valid_codes'=>{
            'http://unitsofmeasure.org'=>[ 's', 'min', 'h', 'd', 'wk', 'mo', 'a' ]
          },
          'type'=>'code',
          'path'=>'Repeat.periodUnit',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/units-of-time'}
        },
        ##
        # mon | tue | wed | thu | fri | sat | sun
        # If one or more days of week is provided, then the action happens only on the specified day(s).
        # If no days are specified, the action is assumed to happen every day as otherwise specified. The elements frequency and period cannot be used as well as dayOfWeek.
        'dayOfWeek' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/days-of-week'=>[ 'mon', 'tue', 'wed', 'thu', 'fri', 'sat', 'sun' ]
          },
          'type'=>'code',
          'path'=>'Repeat.dayOfWeek',
          'min'=>0,
          'max'=>Float::INFINITY,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/days-of-week'}
        },
        ##
        # Time of day for action
        # Specified time of day for action to take place.
        # When time of day is specified, it is inferred that the action happens every day (as filtered by dayofWeek) on the specified times. The elements when, frequency and period cannot be used as well as timeOfDay.
        'timeOfDay' => {
          'type'=>'time',
          'path'=>'Repeat.timeOfDay',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Code for time period of occurrence
        # An approximate time period during the day, potentially linked to an event of daily living that indicates when the action should occur.
        # When more than one event is listed, the event is tied to the union of the specified events.
        'when' => {
          'local_name'=>'local_when'
          'valid_codes'=>{
            'http://hl7.org/fhir/event-timing'=>[ 'MORN', 'MORN.early', 'MORN.late', 'NOON', 'AFT', 'AFT.early', 'AFT.late', 'EVE', 'EVE.early', 'EVE.late', 'NIGHT', 'PHS' ],
            'http://terminology.hl7.org/CodeSystem/v3-TimingEvent'=>[ 'HS', 'WAKE', 'C', 'CM', 'CD', 'CV', 'AC', 'ACM', 'ACD', 'ACV', 'PC', 'PCM', 'PCD', 'PCV' ]
          },
          'type'=>'code',
          'path'=>'Repeat.when',
          'min'=>0,
          'max'=>Float::INFINITY,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/event-timing'}
        },
        ##
        # Minutes from event (before or after)
        # The number of minutes from the event. If the event code does not indicate whether the minutes is before or after the event, then the offset is assumed to be after the event.
        'offset' => {
          'type'=>'unsignedInt',
          'path'=>'Repeat.offset',
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
      # Length/Range of lengths, or (Start and/or end) limits
      # Either a duration for the length of the timing schedule, a range of possible length, or outer bounds for start and/or end limits of the timing schedule.
      attr_accessor :boundsDuration                 # 0-1 Duration
      ##
      # Length/Range of lengths, or (Start and/or end) limits
      # Either a duration for the length of the timing schedule, a range of possible length, or outer bounds for start and/or end limits of the timing schedule.
      attr_accessor :boundsPeriod                   # 0-1 Period
      ##
      # Length/Range of lengths, or (Start and/or end) limits
      # Either a duration for the length of the timing schedule, a range of possible length, or outer bounds for start and/or end limits of the timing schedule.
      attr_accessor :boundsRange                    # 0-1 Range
      ##
      # Number of times to repeat
      # A total count of the desired number of repetitions across the duration of the entire timing specification. If countMax is present, this element indicates the lower bound of the allowed range of count values.
      # If you have both bounds and count, then this should be understood as within the bounds period, until count times happens.
      attr_accessor :count                          # 0-1 positiveInt
      ##
      # Maximum number of times to repeat
      # If present, indicates that the count is a range - so to perform the action between [count] and [countMax] times.
      attr_accessor :countMax                       # 0-1 positiveInt
      ##
      # How long when it happens
      # How long this thing happens for when it happens. If durationMax is present, this element indicates the lower bound of the allowed range of the duration.
      # For some events the duration is part of the definition of the event (e.g. IV infusions, where the duration is implicit in the specified quantity and rate). For others, it's part of the timing specification (e.g. exercise).
      attr_accessor :duration                       # 0-1 decimal
      ##
      # How long when it happens (Max)
      # If present, indicates that the duration is a range - so to perform the action between [duration] and [durationMax] time length.
      # For some events the duration is part of the definition of the event (e.g. IV infusions, where the duration is implicit in the specified quantity and rate). For others, it's part of the timing specification (e.g. exercise).
      attr_accessor :durationMax                    # 0-1 decimal
      ##
      # s | min | h | d | wk | mo | a - unit of time (UCUM)
      # The units of time for the duration, in UCUM units.
      attr_accessor :durationUnit                   # 0-1 code
      ##
      # Event occurs frequency times per period
      # The number of times to repeat the action within the specified period. If frequencyMax is present, this element indicates the lower bound of the allowed range of the frequency.
      attr_accessor :frequency                      # 0-1 positiveInt
      ##
      # Event occurs up to frequencyMax times per period
      # If present, indicates that the frequency is a range - so to repeat between [frequency] and [frequencyMax] times within the period or period range.
      attr_accessor :frequencyMax                   # 0-1 positiveInt
      ##
      # Event occurs frequency times per period
      # Indicates the duration of time over which repetitions are to occur; e.g. to express "3 times per day", 3 would be the frequency and "1 day" would be the period. If periodMax is present, this element indicates the lower bound of the allowed range of the period length.
      attr_accessor :period                         # 0-1 decimal
      ##
      # Upper limit of period (3-4 hours)
      # If present, indicates that the period is a range from [period] to [periodMax], allowing expressing concepts such as "do this once every 3-5 days.
      attr_accessor :periodMax                      # 0-1 decimal
      ##
      # s | min | h | d | wk | mo | a - unit of time (UCUM)
      # The units of time for the period in UCUM units.
      attr_accessor :periodUnit                     # 0-1 code
      ##
      # mon | tue | wed | thu | fri | sat | sun
      # If one or more days of week is provided, then the action happens only on the specified day(s).
      # If no days are specified, the action is assumed to happen every day as otherwise specified. The elements frequency and period cannot be used as well as dayOfWeek.
      attr_accessor :dayOfWeek                      # 0-* [ code ]
      ##
      # Time of day for action
      # Specified time of day for action to take place.
      # When time of day is specified, it is inferred that the action happens every day (as filtered by dayofWeek) on the specified times. The elements when, frequency and period cannot be used as well as timeOfDay.
      attr_accessor :timeOfDay                      # 0-* [ time ]
      ##
      # Code for time period of occurrence
      # An approximate time period during the day, potentially linked to an event of daily living that indicates when the action should occur.
      # When more than one event is listed, the event is tied to the union of the specified events.
      attr_accessor :local_when                     # 0-* [ code ]
      ##
      # Minutes from event (before or after)
      # The number of minutes from the event. If the event code does not indicate whether the minutes is before or after the event, then the offset is assumed to be after the event.
      attr_accessor :offset                         # 0-1 unsignedInt
    end
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
    # Extensions that cannot be ignored even if unrecognized
    # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
    # 
    # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
    # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
    attr_accessor :modifierExtension              # 0-* [ Extension ]
    ##
    # When the event occurs
    # Identifies specific times when the event occurs.
    attr_accessor :event                          # 0-* [ dateTime ]
    ##
    # When the event is to occur
    # A set of rules that describe when the event is scheduled.
    attr_accessor :repeat                         # 0-1 Timing::Repeat
    ##
    # BID | TID | QID | AM | PM | QD | QOD | +
    # A code for the timing schedule (or just text in code.text). Some codes such as BID are ubiquitous, but many institutions define their own additional codes. If a code is provided, the code is understood to be a complete statement of whatever is specified in the structured timing data, and either the code or the data may be used to interpret the Timing, with the exception that .repeat.bounds still applies over the code (and is not contained in the code).
    # BID etc. are defined as 'at institutionally specified times'. For example, an institution may choose that BID is "always at 7am and 6pm".  If it is inappropriate for this choice to be made, the code BID should not be used. Instead, a distinct organization-specific code should be used in place of the HL7-defined BID code and/or a structured representation should be used (in this case, specifying the two event times).
    attr_accessor :code                           # 0-1 CodeableConcept
  end
end
