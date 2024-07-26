module FHIR

  ##
  # A booking of a healthcare event among patient(s), practitioner(s), related person(s) and/or device(s) for a specific date/time. This may result in one or more Encounter(s).
  class Appointment < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['actor', 'appointment-type', 'based-on', 'date', 'identifier', 'location', 'part-status', 'patient', 'practitioner', 'reason-code', 'reason-reference', 'service-category', 'service-type', 'slot', 'specialty', 'status', 'supporting-info']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'Appointment.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'Appointment.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'Appointment.implicitRules',
        'min'=>0,
        'max'=>1
      },
      ##
      # Language of the resource content
      # The base language in which the resource is written.
      # Language is provided to support indexing and accessibility (typically, services such as text to speech use the language tag). The html language tag in the narrative applies  to the narrative. The language tag on the resource may be used to specify the language of other presentations generated from the data in the resource. Not all the content has to be in the base language. The Resource.language should not be assumed to apply to the narrative automatically. If a language is specified, it should it also be specified on the div element in the html (see rules in HTML5 for information about the relationship between xml:lang and the html lang attribute).
      'language' => {
        'valid_codes'=>{
          'urn:ietf:bcp:47'=>[ 'ar', 'bn', 'cs', 'da', 'de', 'de-AT', 'de-CH', 'de-DE', 'el', 'en', 'en-AU', 'en-CA', 'en-GB', 'en-IN', 'en-NZ', 'en-SG', 'en-US', 'es', 'es-AR', 'es-ES', 'es-UY', 'fi', 'fr', 'fr-BE', 'fr-CH', 'fr-FR', 'fy', 'fy-NL', 'hi', 'hr', 'it', 'it-CH', 'it-IT', 'ja', 'ko', 'nl', 'nl-BE', 'nl-NL', 'no', 'no-NO', 'pa', 'pl', 'pt', 'pt-BR', 'ru', 'ru-RU', 'sr', 'sr-RS', 'sv', 'sv-SE', 'te', 'zh', 'zh-CN', 'zh-HK', 'zh-SG', 'zh-TW' ]
        },
        'type'=>'code',
        'path'=>'Appointment.language',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/languages'}
      },
      ##
      # Text summary of the resource, for human interpretation
      # A human-readable narrative that contains a summary of the resource and can be used to represent the content of the resource to a human. The narrative need not encode all the structured data, but is required to contain sufficient detail to make it "clinically safe" for a human to just read the narrative. Resource definitions may define what content should be represented in the narrative to ensure clinical safety.
      # Contained resources do not have narrative. Resources that are not contained SHOULD have a narrative. In some cases, a resource may only have text with little or no additional discrete data (as long as all minOccurs=1 elements are satisfied).  This may be necessary for data from legacy systems where information is captured as a "text blob" or where text is additionally entered raw or narrated and encoded information is added later.
      'text' => {
        'type'=>'Narrative',
        'path'=>'Appointment.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'Appointment.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Appointment.extension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Extensions that cannot be ignored
      # May be used to represent additional information that is not part of the basic definition of the resource and that modifies the understanding of the element that contains it and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer is allowed to define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
      # 
      # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'modifierExtension' => {
        'type'=>'Extension',
        'path'=>'Appointment.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # External Ids for this item
      # This records identifiers associated with this appointment concern that are defined by business processes and/or used to refer to it when a direct URL reference to the resource itself is not appropriate (e.g. in CDA documents, or in written / printed documentation).
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'Appointment.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # proposed | pending | booked | arrived | fulfilled | cancelled | noshow | entered-in-error | checked-in | waitlist
      # The overall status of the Appointment. Each of the participants has their own participation status which indicates their involvement in the process, however this status indicates the shared status.
      # If the Appointment's status is "cancelled" then all participants are expected to have their calendars released for the appointment period, and as such any Slots that were marked as BUSY can be re-set to FREE.
      # 
      # This element is labeled as a modifier because the status contains the code entered-in-error that mark the Appointment as not currently valid.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/appointmentstatus'=>[ 'proposed', 'pending', 'booked', 'arrived', 'fulfilled', 'cancelled', 'noshow', 'entered-in-error', 'checked-in', 'waitlist' ]
        },
        'type'=>'code',
        'path'=>'Appointment.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/appointmentstatus'}
      },
      ##
      # The coded reason for the appointment being cancelled. This is often used in reporting/billing/futher processing to determine if further actions are required, or specific fees apply.
      'cancelationReason' => {
        'type'=>'CodeableConcept',
        'path'=>'Appointment.cancelationReason',
        'min'=>0,
        'max'=>1
      },
      ##
      # A broad categorization of the service that is to be performed during this appointment.
      'serviceCategory' => {
        'type'=>'CodeableConcept',
        'path'=>'Appointment.serviceCategory',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The specific service that is to be performed during this appointment.
      # For a provider to provider appointment the code "FOLLOWUP" may be appropriate, as this is expected to be discussing some patient that was seen in the past.
      'serviceType' => {
        'type'=>'CodeableConcept',
        'path'=>'Appointment.serviceType',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The specialty of a practitioner that would be required to perform the service requested in this appointment.
      'specialty' => {
        'type'=>'CodeableConcept',
        'path'=>'Appointment.specialty',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The style of appointment or patient that has been booked in the slot (not service type).
      'appointmentType' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/v2-0276'=>[ 'CHECKUP', 'EMERGENCY', 'FOLLOWUP', 'ROUTINE', 'WALKIN' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Appointment.appointmentType',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'preferred', 'uri'=>'http://terminology.hl7.org/ValueSet/v2-0276'}
      },
      ##
      # Coded reason this appointment is scheduled
      # The coded reason that this appointment is being scheduled. This is more clinical than administrative.
      'reasonCode' => {
        'type'=>'CodeableConcept',
        'path'=>'Appointment.reasonCode',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Reason the appointment is to take place (resource)
      # Reason the appointment has been scheduled to take place, as specified using information from another resource. When the patient arrives and the encounter begins it may be used as the admission diagnosis. The indication will typically be a Condition (with other resources referenced in the evidence.detail), or a Procedure.
      'reasonReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Condition', 'http://hl7.org/fhir/StructureDefinition/Procedure', 'http://hl7.org/fhir/StructureDefinition/Observation', 'http://hl7.org/fhir/StructureDefinition/ImmunizationRecommendation'],
        'type'=>'Reference',
        'path'=>'Appointment.reasonReference',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Used to make informed decisions if needing to re-prioritize
      # The priority of the appointment. Can be used to make informed decisions if needing to re-prioritize appointments. (The iCal Standard specifies 0 as undefined, 1 as highest, 9 as lowest priority).
      # Seeking implementer feedback on this property and how interoperable it is.Using an extension to record a CodeableConcept for named values may be tested at a future connectathon.
      'priority' => {
        'type'=>'unsignedInt',
        'path'=>'Appointment.priority',
        'min'=>0,
        'max'=>1
      },
      ##
      # Shown on a subject line in a meeting request, or appointment list
      # The brief description of the appointment as would be shown on a subject line in a meeting request, or appointment list. Detailed or expanded information should be put in the comment field.
      'description' => {
        'type'=>'string',
        'path'=>'Appointment.description',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional information to support the appointment provided when making the appointment.
      'supportingInformation' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'Reference',
        'path'=>'Appointment.supportingInformation',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # When appointment is to take place
      # Date/Time that the appointment is to take place.
      'start' => {
        'type'=>'instant',
        'path'=>'Appointment.start',
        'min'=>0,
        'max'=>1
      },
      ##
      # When appointment is to conclude
      # Date/Time that the appointment is to conclude.
      'end' => {
        'local_name'=>'local_end'
        'type'=>'instant',
        'path'=>'Appointment.end',
        'min'=>0,
        'max'=>1
      },
      ##
      # Can be less than start/end (e.g. estimate)
      # Number of minutes that the appointment is to take. This can be less than the duration between the start and end times.  For example, where the actual time of appointment is only an estimate or if a 30 minute appointment is being requested, but any time would work.  Also, if there is, for example, a planned 15 minute break in the middle of a long appointment, the duration may be 15 minutes less than the difference between the start and end.
      'minutesDuration' => {
        'type'=>'positiveInt',
        'path'=>'Appointment.minutesDuration',
        'min'=>0,
        'max'=>1
      },
      ##
      # The slots that this appointment is filling
      # The slots from the participants' schedules that will be filled by the appointment.
      'slot' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Slot'],
        'type'=>'Reference',
        'path'=>'Appointment.slot',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The date that this appointment was initially created. This could be different to the meta.lastModified value on the initial entry, as this could have been before the resource was created on the FHIR server, and should remain unchanged over the lifespan of the appointment.
      # This property is required for many use cases where the age of an appointment is considered in processing workflows for scheduling and billing of appointments.
      'created' => {
        'type'=>'dateTime',
        'path'=>'Appointment.created',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional comments about the appointment.
      # Additional text to aid in facilitating the appointment. For instance, a comment might be, "patient should proceed immediately to infusion room upon arrival"Where this is a planned appointment and the start/end dates are not set then this field can be used to provide additional guidance on the details of the appointment request, including any restrictions on when to book it.
      'comment' => {
        'type'=>'string',
        'path'=>'Appointment.comment',
        'min'=>0,
        'max'=>1
      },
      ##
      # Detailed information and instructions for the patient
      # While Appointment.comment contains information for internal use, Appointment.patientInstructions is used to capture patient facing information about the Appointment (e.g. please bring your referral or fast from 8pm night before).
      'patientInstruction' => {
        'type'=>'string',
        'path'=>'Appointment.patientInstruction',
        'min'=>0,
        'max'=>1
      },
      ##
      # The service request this appointment is allocated to assess (e.g. incoming referral or procedure request).
      'basedOn' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ServiceRequest'],
        'type'=>'Reference',
        'path'=>'Appointment.basedOn',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Participants involved in appointment
      # List of participants involved in the appointment.
      'participant' => {
        'type'=>'Appointment::Participant',
        'path'=>'Appointment.participant',
        'min'=>1,
        'max'=>Float::INFINITY
      },
      ##
      # Potential date/time interval(s) requested to allocate the appointment within
      # A set of date ranges (potentially including times) that the appointment is preferred to be scheduled within.
      # 
      # The duration (usually in minutes) could also be provided to indicate the length of the appointment to fill and populate the start/end times for the actual allocated time. However, in other situations the duration may be calculated by the scheduling system.
      # This does not introduce a capacity for recurring appointments.
      'requestedPeriod' => {
        'type'=>'Period',
        'path'=>'Appointment.requestedPeriod',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Participants involved in appointment
    # List of participants involved in the appointment.
    class Participant < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Participant.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Participant.extension',
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
          'path'=>'Participant.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Role of participant in the appointment.
        # The role of the participant can be used to declare what the actor will be doing in the scope of this appointment.If the actor is not specified, then it is expected that the actor will be filled in at a later stage of planning.This value SHALL be the same when creating an AppointmentResponse so that they can be matched, and subsequently update the Appointment.
        'type' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/participant-type'=>[ 'translator', 'emergency' ],
            'http://terminology.hl7.org/CodeSystem/v3-ParticipationType'=>[ 'ADM', 'ATND', 'CALLBCK', 'CON', 'DIS', 'ESC', 'REF', 'SPRF', 'PPRF', 'PART' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Participant.type',
          'min'=>0,
          'max'=>Float::INFINITY,
          'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/encounter-participant-type'}
        },
        ##
        # Person, Location/HealthcareService or Device
        # A Person, Location/HealthcareService or Device that is participating in the appointment.
        'actor' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/HealthcareService', 'http://hl7.org/fhir/StructureDefinition/Location'],
          'type'=>'Reference',
          'path'=>'Participant.actor',
          'min'=>0,
          'max'=>1
        },
        ##
        # required | optional | information-only
        # Whether this participant is required to be present at the meeting. This covers a use-case where two doctors need to meet to discuss the results for a specific patient, and the patient is not required to be present.
        'required' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/participantrequired'=>[ 'required', 'optional', 'information-only' ]
          },
          'type'=>'code',
          'path'=>'Participant.required',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/participantrequired'}
        },
        ##
        # accepted | declined | tentative | needs-action
        # Participation status of the actor.
        'status' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/participationstatus'=>[ 'accepted', 'declined', 'tentative', 'needs-action' ]
          },
          'type'=>'code',
          'path'=>'Participant.status',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/participationstatus'}
        },
        ##
        # Participation period of the actor.
        'period' => {
          'type'=>'Period',
          'path'=>'Participant.period',
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
      # Role of participant in the appointment.
      # The role of the participant can be used to declare what the actor will be doing in the scope of this appointment.If the actor is not specified, then it is expected that the actor will be filled in at a later stage of planning.This value SHALL be the same when creating an AppointmentResponse so that they can be matched, and subsequently update the Appointment.
      attr_accessor :type                           # 0-* [ CodeableConcept ]
      ##
      # Person, Location/HealthcareService or Device
      # A Person, Location/HealthcareService or Device that is participating in the appointment.
      attr_accessor :actor                          # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/HealthcareService|http://hl7.org/fhir/StructureDefinition/Location)
      ##
      # required | optional | information-only
      # Whether this participant is required to be present at the meeting. This covers a use-case where two doctors need to meet to discuss the results for a specific patient, and the patient is not required to be present.
      attr_accessor :required                       # 0-1 code
      ##
      # accepted | declined | tentative | needs-action
      # Participation status of the actor.
      attr_accessor :status                         # 1-1 code
      ##
      # Participation period of the actor.
      attr_accessor :period                         # 0-1 Period
    end
    ##
    # Logical id of this artifact
    # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
    # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
    attr_accessor :id                             # 0-1 id
    ##
    # Metadata about the resource
    # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
    attr_accessor :meta                           # 0-1 Meta
    ##
    # A set of rules under which this content was created
    # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
    # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
    attr_accessor :implicitRules                  # 0-1 uri
    ##
    # Language of the resource content
    # The base language in which the resource is written.
    # Language is provided to support indexing and accessibility (typically, services such as text to speech use the language tag). The html language tag in the narrative applies  to the narrative. The language tag on the resource may be used to specify the language of other presentations generated from the data in the resource. Not all the content has to be in the base language. The Resource.language should not be assumed to apply to the narrative automatically. If a language is specified, it should it also be specified on the div element in the html (see rules in HTML5 for information about the relationship between xml:lang and the html lang attribute).
    attr_accessor :language                       # 0-1 code
    ##
    # Text summary of the resource, for human interpretation
    # A human-readable narrative that contains a summary of the resource and can be used to represent the content of the resource to a human. The narrative need not encode all the structured data, but is required to contain sufficient detail to make it "clinically safe" for a human to just read the narrative. Resource definitions may define what content should be represented in the narrative to ensure clinical safety.
    # Contained resources do not have narrative. Resources that are not contained SHOULD have a narrative. In some cases, a resource may only have text with little or no additional discrete data (as long as all minOccurs=1 elements are satisfied).  This may be necessary for data from legacy systems where information is captured as a "text blob" or where text is additionally entered raw or narrated and encoded information is added later.
    attr_accessor :text                           # 0-1 Narrative
    ##
    # Contained, inline Resources
    # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
    # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
    attr_accessor :contained                      # 0-* [ Resource ]
    ##
    # Additional content defined by implementations
    # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
    # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
    attr_accessor :extension                      # 0-* [ Extension ]
    ##
    # Extensions that cannot be ignored
    # May be used to represent additional information that is not part of the basic definition of the resource and that modifies the understanding of the element that contains it and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer is allowed to define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
    # 
    # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
    # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
    attr_accessor :modifierExtension              # 0-* [ Extension ]
    ##
    # External Ids for this item
    # This records identifiers associated with this appointment concern that are defined by business processes and/or used to refer to it when a direct URL reference to the resource itself is not appropriate (e.g. in CDA documents, or in written / printed documentation).
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # proposed | pending | booked | arrived | fulfilled | cancelled | noshow | entered-in-error | checked-in | waitlist
    # The overall status of the Appointment. Each of the participants has their own participation status which indicates their involvement in the process, however this status indicates the shared status.
    # If the Appointment's status is "cancelled" then all participants are expected to have their calendars released for the appointment period, and as such any Slots that were marked as BUSY can be re-set to FREE.
    # 
    # This element is labeled as a modifier because the status contains the code entered-in-error that mark the Appointment as not currently valid.
    attr_accessor :status                         # 1-1 code
    ##
    # The coded reason for the appointment being cancelled. This is often used in reporting/billing/futher processing to determine if further actions are required, or specific fees apply.
    attr_accessor :cancelationReason              # 0-1 CodeableConcept
    ##
    # A broad categorization of the service that is to be performed during this appointment.
    attr_accessor :serviceCategory                # 0-* [ CodeableConcept ]
    ##
    # The specific service that is to be performed during this appointment.
    # For a provider to provider appointment the code "FOLLOWUP" may be appropriate, as this is expected to be discussing some patient that was seen in the past.
    attr_accessor :serviceType                    # 0-* [ CodeableConcept ]
    ##
    # The specialty of a practitioner that would be required to perform the service requested in this appointment.
    attr_accessor :specialty                      # 0-* [ CodeableConcept ]
    ##
    # The style of appointment or patient that has been booked in the slot (not service type).
    attr_accessor :appointmentType                # 0-1 CodeableConcept
    ##
    # Coded reason this appointment is scheduled
    # The coded reason that this appointment is being scheduled. This is more clinical than administrative.
    attr_accessor :reasonCode                     # 0-* [ CodeableConcept ]
    ##
    # Reason the appointment is to take place (resource)
    # Reason the appointment has been scheduled to take place, as specified using information from another resource. When the patient arrives and the encounter begins it may be used as the admission diagnosis. The indication will typically be a Condition (with other resources referenced in the evidence.detail), or a Procedure.
    attr_accessor :reasonReference                # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Condition|http://hl7.org/fhir/StructureDefinition/Procedure|http://hl7.org/fhir/StructureDefinition/Observation|http://hl7.org/fhir/StructureDefinition/ImmunizationRecommendation) ]
    ##
    # Used to make informed decisions if needing to re-prioritize
    # The priority of the appointment. Can be used to make informed decisions if needing to re-prioritize appointments. (The iCal Standard specifies 0 as undefined, 1 as highest, 9 as lowest priority).
    # Seeking implementer feedback on this property and how interoperable it is.Using an extension to record a CodeableConcept for named values may be tested at a future connectathon.
    attr_accessor :priority                       # 0-1 unsignedInt
    ##
    # Shown on a subject line in a meeting request, or appointment list
    # The brief description of the appointment as would be shown on a subject line in a meeting request, or appointment list. Detailed or expanded information should be put in the comment field.
    attr_accessor :description                    # 0-1 string
    ##
    # Additional information to support the appointment provided when making the appointment.
    attr_accessor :supportingInformation          # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Resource) ]
    ##
    # When appointment is to take place
    # Date/Time that the appointment is to take place.
    attr_accessor :start                          # 0-1 instant
    ##
    # When appointment is to conclude
    # Date/Time that the appointment is to conclude.
    attr_accessor :local_end                      # 0-1 instant
    ##
    # Can be less than start/end (e.g. estimate)
    # Number of minutes that the appointment is to take. This can be less than the duration between the start and end times.  For example, where the actual time of appointment is only an estimate or if a 30 minute appointment is being requested, but any time would work.  Also, if there is, for example, a planned 15 minute break in the middle of a long appointment, the duration may be 15 minutes less than the difference between the start and end.
    attr_accessor :minutesDuration                # 0-1 positiveInt
    ##
    # The slots that this appointment is filling
    # The slots from the participants' schedules that will be filled by the appointment.
    attr_accessor :slot                           # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Slot) ]
    ##
    # The date that this appointment was initially created. This could be different to the meta.lastModified value on the initial entry, as this could have been before the resource was created on the FHIR server, and should remain unchanged over the lifespan of the appointment.
    # This property is required for many use cases where the age of an appointment is considered in processing workflows for scheduling and billing of appointments.
    attr_accessor :created                        # 0-1 dateTime
    ##
    # Additional comments about the appointment.
    # Additional text to aid in facilitating the appointment. For instance, a comment might be, "patient should proceed immediately to infusion room upon arrival"Where this is a planned appointment and the start/end dates are not set then this field can be used to provide additional guidance on the details of the appointment request, including any restrictions on when to book it.
    attr_accessor :comment                        # 0-1 string
    ##
    # Detailed information and instructions for the patient
    # While Appointment.comment contains information for internal use, Appointment.patientInstructions is used to capture patient facing information about the Appointment (e.g. please bring your referral or fast from 8pm night before).
    attr_accessor :patientInstruction             # 0-1 string
    ##
    # The service request this appointment is allocated to assess (e.g. incoming referral or procedure request).
    attr_accessor :basedOn                        # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/ServiceRequest) ]
    ##
    # Participants involved in appointment
    # List of participants involved in the appointment.
    attr_accessor :participant                    # 1-* [ Appointment::Participant ]
    ##
    # Potential date/time interval(s) requested to allocate the appointment within
    # A set of date ranges (potentially including times) that the appointment is preferred to be scheduled within.
    # 
    # The duration (usually in minutes) could also be provided to indicate the length of the appointment to fill and populate the start/end times for the actual allocated time. However, in other situations the duration may be calculated by the scheduling system.
    # This does not introduce a capacity for recurring appointments.
    attr_accessor :requestedPeriod                # 0-* [ Period ]

    def resourceType
      'Appointment'
    end
  end
end
