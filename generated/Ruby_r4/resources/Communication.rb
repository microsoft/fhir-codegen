module FHIR

  ##
  # An occurrence of information being transmitted; e.g. an alert that was sent to a responsible provider, a public health agency that was notified about a reportable condition.
  class Communication < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['based-on', 'category', 'encounter', 'identifier', 'instantiates-canonical', 'instantiates-uri', 'medium', 'part-of', 'patient', 'received', 'recipient', 'sender', 'sent', 'status', 'subject']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'Communication.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'Communication.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'Communication.implicitRules',
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
        'path'=>'Communication.language',
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
        'path'=>'Communication.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'Communication.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Communication.extension',
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
        'path'=>'Communication.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Unique identifier
      # Business identifiers assigned to this communication by the performer or other systems which remain constant as the resource is updated and propagates from server to server.
      # This is a business identifier, not a resource identifier (see [discussion](resource.html#identifiers)).  It is best practice for the identifier to only appear on a single resource instance, however business practices may occasionally dictate that multiple resource instances with the same identifier can exist - possibly even with different resource types.  For example, multiple Patient and a Person resource instance might share the same social insurance number.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'Communication.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Instantiates FHIR protocol or definition
      # The URL pointing to a FHIR-defined protocol, guideline, orderset or other definition that is adhered to in whole or in part by this Communication.
      'instantiatesCanonical' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/PlanDefinition', 'http://hl7.org/fhir/StructureDefinition/ActivityDefinition', 'http://hl7.org/fhir/StructureDefinition/Measure', 'http://hl7.org/fhir/StructureDefinition/OperationDefinition', 'http://hl7.org/fhir/StructureDefinition/Questionnaire'],
        'type'=>'canonical',
        'path'=>'Communication.instantiatesCanonical',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Instantiates external protocol or definition
      # The URL pointing to an externally maintained protocol, guideline, orderset or other definition that is adhered to in whole or in part by this Communication.
      # This might be an HTML page, PDF, etc. or could just be a non-resolvable URI identifier.
      'instantiatesUri' => {
        'type'=>'uri',
        'path'=>'Communication.instantiatesUri',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Request fulfilled by this communication
      # An order, proposal or plan fulfilled in whole or in part by this Communication.
      # This must point to some sort of a 'Request' resource, such as CarePlan, CommunicationRequest, ServiceRequest, MedicationRequest, etc.
      'basedOn' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'Reference',
        'path'=>'Communication.basedOn',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Part of this action.
      'partOf' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'Reference',
        'path'=>'Communication.partOf',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Reply to
      # Prior communication that this communication is in response to.
      'inResponseTo' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Communication'],
        'type'=>'Reference',
        'path'=>'Communication.inResponseTo',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # preparation | in-progress | not-done | on-hold | stopped | completed | entered-in-error | unknown
      # The status of the transmission.
      # This element is labeled as a modifier because the status contains the codes aborted and entered-in-error that mark the communication as not currently valid.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/event-status'=>[ 'preparation', 'in-progress', 'not-done', 'on-hold', 'stopped', 'completed', 'entered-in-error', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'Communication.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/event-status'}
      },
      ##
      # Reason for current status
      # Captures the reason for the current state of the Communication.
      # This is generally only used for "exception" statuses such as "not-done", "suspended" or "aborted". The reason for performing the event at all is captured in reasonCode, not here.
      'statusReason' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/communication-not-done-reason'=>[ 'unknown', 'system-error', 'invalid-phone-number', 'recipient-unavailable', 'family-objection', 'patient-objection' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Communication.statusReason',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/communication-not-done-reason'}
      },
      ##
      # Message category
      # The type of message conveyed such as alert, notification, reminder, instruction, etc.
      # There may be multiple axes of categorization and one communication may serve multiple purposes.
      'category' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/communication-category'=>[ 'alert', 'notification', 'reminder', 'instruction' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Communication.category',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/communication-category'}
      },
      ##
      # routine | urgent | asap | stat
      # Characterizes how quickly the planned or in progress communication must be addressed. Includes concepts such as stat, urgent, routine.
      # Used to prioritize workflow (such as which communication to read first) when the communication is planned or in progress.
      'priority' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/request-priority'=>[ 'routine', 'urgent', 'asap', 'stat' ]
        },
        'type'=>'code',
        'path'=>'Communication.priority',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/request-priority'}
      },
      ##
      # A channel of communication
      # A channel that was used for this communication (e.g. email, fax).
      'medium' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/v3-ParticipationMode'=>[ 'ELECTRONIC', 'PHYSICAL', 'REMOTE', 'VERBAL', 'DICTATE', 'FACE', 'PHONE', 'VIDEOCONF', 'WRITTEN', 'FAXWRIT', 'HANDWRIT', 'MAILWRIT', 'ONLINEWRIT', 'EMAILWRIT', 'TYPEWRIT' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Communication.medium',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'example', 'uri'=>'http://terminology.hl7.org/ValueSet/v3-ParticipationMode'}
      },
      ##
      # Focus of message
      # The patient or group that was the focus of this communication.
      'subject' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Group'],
        'type'=>'Reference',
        'path'=>'Communication.subject',
        'min'=>0,
        'max'=>1
      },
      ##
      # Description of the purpose/content, similar to a subject line in an email.
      # Communication.topic.text can be used without any codings.
      'topic' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/communication-topic'=>[ 'prescription-refill-request', 'progress-update', 'report-labs', 'appointment-reminder', 'phone-consult', 'summary-report' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Communication.topic',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/communication-topic'}
      },
      ##
      # Resources that pertain to this communication
      # Other resources that pertain to this communication and to which this communication should be associated.
      # Don't use Communication.about element when a more specific element exists, such as basedOn or reasonReference.
      'about' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'Reference',
        'path'=>'Communication.about',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Encounter created as part of
      # The Encounter during which this Communication was created or to which the creation of this record is tightly associated.
      # This will typically be the encounter the event occurred within, but some activities may be initiated prior to or after the official completion of an encounter but still be tied to the context of the encounter.
      'encounter' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Encounter'],
        'type'=>'Reference',
        'path'=>'Communication.encounter',
        'min'=>0,
        'max'=>1
      },
      ##
      # When sent
      # The time when this communication was sent.
      'sent' => {
        'type'=>'dateTime',
        'path'=>'Communication.sent',
        'min'=>0,
        'max'=>1
      },
      ##
      # When received
      # The time when this communication arrived at the destination.
      'received' => {
        'type'=>'dateTime',
        'path'=>'Communication.received',
        'min'=>0,
        'max'=>1
      },
      ##
      # Message recipient
      # The entity (e.g. person, organization, clinical information system, care team or device) which was the target of the communication. If receipts need to be tracked by an individual, a separate resource instance will need to be created for each recipient.  Multiple recipient communications are intended where either receipts are not tracked (e.g. a mass mail-out) or a receipt is captured in aggregate (all emails confirmed received by a particular time).
      'recipient' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/Group', 'http://hl7.org/fhir/StructureDefinition/CareTeam', 'http://hl7.org/fhir/StructureDefinition/HealthcareService'],
        'type'=>'Reference',
        'path'=>'Communication.recipient',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Message sender
      # The entity (e.g. person, organization, clinical information system, or device) which was the source of the communication.
      'sender' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/HealthcareService'],
        'type'=>'Reference',
        'path'=>'Communication.sender',
        'min'=>0,
        'max'=>1
      },
      ##
      # Indication for message
      # The reason or justification for the communication.
      # Textual reasons can be captured using reasonCode.text.
      'reasonCode' => {
        'type'=>'CodeableConcept',
        'path'=>'Communication.reasonCode',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Why was communication done?
      # Indicates another resource whose existence justifies this communication.
      'reasonReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Condition', 'http://hl7.org/fhir/StructureDefinition/Observation', 'http://hl7.org/fhir/StructureDefinition/DiagnosticReport', 'http://hl7.org/fhir/StructureDefinition/DocumentReference'],
        'type'=>'Reference',
        'path'=>'Communication.reasonReference',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Message payload
      # Text, attachment(s), or resource(s) that was communicated to the recipient.
      'payload' => {
        'type'=>'Communication::Payload',
        'path'=>'Communication.payload',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Comments made about the communication
      # Additional notes or commentary about the communication by the sender, receiver or other interested parties.
      'note' => {
        'type'=>'Annotation',
        'path'=>'Communication.note',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Message payload
    # Text, attachment(s), or resource(s) that was communicated to the recipient.
    class Payload < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'content[x]' => ['Attachment', 'Reference', 'string']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Payload.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Payload.extension',
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
          'path'=>'Payload.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Message part content
        # A communicated content (or for multi-part communications, one portion of the communication).
        'contentAttachment' => {
          'type'=>'Attachment',
          'path'=>'Payload.content[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Message part content
        # A communicated content (or for multi-part communications, one portion of the communication).
        'contentReference' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
          'type'=>'Reference',
          'path'=>'Payload.content[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Message part content
        # A communicated content (or for multi-part communications, one portion of the communication).
        'contentString' => {
          'type'=>'String',
          'path'=>'Payload.content[x]',
          'min'=>1,
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
      # Message part content
      # A communicated content (or for multi-part communications, one portion of the communication).
      attr_accessor :contentAttachment              # 1-1 Attachment
      ##
      # Message part content
      # A communicated content (or for multi-part communications, one portion of the communication).
      attr_accessor :contentReference               # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Resource)
      ##
      # Message part content
      # A communicated content (or for multi-part communications, one portion of the communication).
      attr_accessor :contentString                  # 1-1 String
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
    # Unique identifier
    # Business identifiers assigned to this communication by the performer or other systems which remain constant as the resource is updated and propagates from server to server.
    # This is a business identifier, not a resource identifier (see [discussion](resource.html#identifiers)).  It is best practice for the identifier to only appear on a single resource instance, however business practices may occasionally dictate that multiple resource instances with the same identifier can exist - possibly even with different resource types.  For example, multiple Patient and a Person resource instance might share the same social insurance number.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Instantiates FHIR protocol or definition
    # The URL pointing to a FHIR-defined protocol, guideline, orderset or other definition that is adhered to in whole or in part by this Communication.
    attr_accessor :instantiatesCanonical          # 0-* [ canonical(http://hl7.org/fhir/StructureDefinition/PlanDefinition|http://hl7.org/fhir/StructureDefinition/ActivityDefinition|http://hl7.org/fhir/StructureDefinition/Measure|http://hl7.org/fhir/StructureDefinition/OperationDefinition|http://hl7.org/fhir/StructureDefinition/Questionnaire) ]
    ##
    # Instantiates external protocol or definition
    # The URL pointing to an externally maintained protocol, guideline, orderset or other definition that is adhered to in whole or in part by this Communication.
    # This might be an HTML page, PDF, etc. or could just be a non-resolvable URI identifier.
    attr_accessor :instantiatesUri                # 0-* [ uri ]
    ##
    # Request fulfilled by this communication
    # An order, proposal or plan fulfilled in whole or in part by this Communication.
    # This must point to some sort of a 'Request' resource, such as CarePlan, CommunicationRequest, ServiceRequest, MedicationRequest, etc.
    attr_accessor :basedOn                        # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Resource) ]
    ##
    # Part of this action.
    attr_accessor :partOf                         # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Resource) ]
    ##
    # Reply to
    # Prior communication that this communication is in response to.
    attr_accessor :inResponseTo                   # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Communication) ]
    ##
    # preparation | in-progress | not-done | on-hold | stopped | completed | entered-in-error | unknown
    # The status of the transmission.
    # This element is labeled as a modifier because the status contains the codes aborted and entered-in-error that mark the communication as not currently valid.
    attr_accessor :status                         # 1-1 code
    ##
    # Reason for current status
    # Captures the reason for the current state of the Communication.
    # This is generally only used for "exception" statuses such as "not-done", "suspended" or "aborted". The reason for performing the event at all is captured in reasonCode, not here.
    attr_accessor :statusReason                   # 0-1 CodeableConcept
    ##
    # Message category
    # The type of message conveyed such as alert, notification, reminder, instruction, etc.
    # There may be multiple axes of categorization and one communication may serve multiple purposes.
    attr_accessor :category                       # 0-* [ CodeableConcept ]
    ##
    # routine | urgent | asap | stat
    # Characterizes how quickly the planned or in progress communication must be addressed. Includes concepts such as stat, urgent, routine.
    # Used to prioritize workflow (such as which communication to read first) when the communication is planned or in progress.
    attr_accessor :priority                       # 0-1 code
    ##
    # A channel of communication
    # A channel that was used for this communication (e.g. email, fax).
    attr_accessor :medium                         # 0-* [ CodeableConcept ]
    ##
    # Focus of message
    # The patient or group that was the focus of this communication.
    attr_accessor :subject                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Group)
    ##
    # Description of the purpose/content, similar to a subject line in an email.
    # Communication.topic.text can be used without any codings.
    attr_accessor :topic                          # 0-1 CodeableConcept
    ##
    # Resources that pertain to this communication
    # Other resources that pertain to this communication and to which this communication should be associated.
    # Don't use Communication.about element when a more specific element exists, such as basedOn or reasonReference.
    attr_accessor :about                          # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Resource) ]
    ##
    # Encounter created as part of
    # The Encounter during which this Communication was created or to which the creation of this record is tightly associated.
    # This will typically be the encounter the event occurred within, but some activities may be initiated prior to or after the official completion of an encounter but still be tied to the context of the encounter.
    attr_accessor :encounter                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Encounter)
    ##
    # When sent
    # The time when this communication was sent.
    attr_accessor :sent                           # 0-1 dateTime
    ##
    # When received
    # The time when this communication arrived at the destination.
    attr_accessor :received                       # 0-1 dateTime
    ##
    # Message recipient
    # The entity (e.g. person, organization, clinical information system, care team or device) which was the target of the communication. If receipts need to be tracked by an individual, a separate resource instance will need to be created for each recipient.  Multiple recipient communications are intended where either receipts are not tracked (e.g. a mass mail-out) or a receipt is captured in aggregate (all emails confirmed received by a particular time).
    attr_accessor :recipient                      # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/Group|http://hl7.org/fhir/StructureDefinition/CareTeam|http://hl7.org/fhir/StructureDefinition/HealthcareService) ]
    ##
    # Message sender
    # The entity (e.g. person, organization, clinical information system, or device) which was the source of the communication.
    attr_accessor :sender                         # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/HealthcareService)
    ##
    # Indication for message
    # The reason or justification for the communication.
    # Textual reasons can be captured using reasonCode.text.
    attr_accessor :reasonCode                     # 0-* [ CodeableConcept ]
    ##
    # Why was communication done?
    # Indicates another resource whose existence justifies this communication.
    attr_accessor :reasonReference                # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Condition|http://hl7.org/fhir/StructureDefinition/Observation|http://hl7.org/fhir/StructureDefinition/DiagnosticReport|http://hl7.org/fhir/StructureDefinition/DocumentReference) ]
    ##
    # Message payload
    # Text, attachment(s), or resource(s) that was communicated to the recipient.
    attr_accessor :payload                        # 0-* [ Communication::Payload ]
    ##
    # Comments made about the communication
    # Additional notes or commentary about the communication by the sender, receiver or other interested parties.
    attr_accessor :note                           # 0-* [ Annotation ]

    def resourceType
      'Communication'
    end
  end
end
