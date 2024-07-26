module FHIR

  ##
  # A request to convey information; e.g. the CDS system proposes that an alert be sent to a responsible provider, the CDS system proposes that the public health agency be notified about a reportable condition.
  class CommunicationRequest < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['authored', 'based-on', 'category', 'encounter', 'group-identifier', 'identifier', 'medium', 'occurrence', 'patient', 'priority', 'recipient', 'replaces', 'requester', 'sender', 'status', 'subject']
    MULTIPLE_TYPES = {
      'occurrence[x]' => ['dateTime', 'Period']
    }
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'CommunicationRequest.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'CommunicationRequest.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'CommunicationRequest.implicitRules',
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
        'path'=>'CommunicationRequest.language',
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
        'path'=>'CommunicationRequest.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'CommunicationRequest.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'CommunicationRequest.extension',
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
        'path'=>'CommunicationRequest.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Unique identifier
      # Business identifiers assigned to this communication request by the performer or other systems which remain constant as the resource is updated and propagates from server to server.
      # This is a business identifier, not a resource identifier (see [discussion](resource.html#identifiers)).  It is best practice for the identifier to only appear on a single resource instance, however business practices may occasionally dictate that multiple resource instances with the same identifier can exist - possibly even with different resource types.  For example, multiple Patient and a Person resource instance might share the same social insurance number.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'CommunicationRequest.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Fulfills plan or proposal
      # A plan or proposal that is fulfilled in whole or in part by this request.
      'basedOn' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'Reference',
        'path'=>'CommunicationRequest.basedOn',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Request(s) replaced by this request
      # Completed or terminated request(s) whose function is taken by this new request.
      # The replacement could be because the initial request was immediately rejected (due to an issue) or because the previous request was completed, but the need for the action described by the request remains ongoing.
      'replaces' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/CommunicationRequest'],
        'type'=>'Reference',
        'path'=>'CommunicationRequest.replaces',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Composite request this is part of
      # A shared identifier common to all requests that were authorized more or less simultaneously by a single author, representing the identifier of the requisition, prescription or similar form.
      # Requests are linked either by a "basedOn" relationship (i.e. one request is fulfilling another) or by having a common requisition.  Requests that are part of the same requisition are generally treated independently from the perspective of changing their state or maintaining them after initial creation.
      'groupIdentifier' => {
        'type'=>'Identifier',
        'path'=>'CommunicationRequest.groupIdentifier',
        'min'=>0,
        'max'=>1
      },
      ##
      # draft | active | on-hold | revoked | completed | entered-in-error | unknown
      # The status of the proposal or order.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/request-status'=>[ 'draft', 'active', 'on-hold', 'revoked', 'completed', 'entered-in-error', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'CommunicationRequest.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/request-status'}
      },
      ##
      # Reason for current status
      # Captures the reason for the current state of the CommunicationRequest.
      # This is generally only used for "exception" statuses such as "suspended" or "cancelled".  The reason why the CommunicationRequest was created at all is captured in reasonCode, not here.  [distinct reason codes for different statuses can be enforced using invariants if they are universal bindings].
      'statusReason' => {
        'type'=>'CodeableConcept',
        'path'=>'CommunicationRequest.statusReason',
        'min'=>0,
        'max'=>1
      },
      ##
      # Message category
      # The type of message to be sent such as alert, notification, reminder, instruction, etc.
      # There may be multiple axes of categorization and one communication request may serve multiple purposes.
      'category' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/communication-category'=>[ 'alert', 'notification', 'reminder', 'instruction' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'CommunicationRequest.category',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/communication-category'}
      },
      ##
      # routine | urgent | asap | stat
      # Characterizes how quickly the proposed act must be initiated. Includes concepts such as stat, urgent, routine.
      'priority' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/request-priority'=>[ 'routine', 'urgent', 'asap', 'stat' ]
        },
        'type'=>'code',
        'path'=>'CommunicationRequest.priority',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/request-priority'}
      },
      ##
      # True if request is prohibiting action
      # If true indicates that the CommunicationRequest is asking for the specified action to *not* occur.
      # The attributes provided with the request qualify what is not to be done.
      'doNotPerform' => {
        'type'=>'boolean',
        'path'=>'CommunicationRequest.doNotPerform',
        'min'=>0,
        'max'=>1
      },
      ##
      # A channel of communication
      # A channel that was used for this communication (e.g. email, fax).
      'medium' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/v3-ParticipationMode'=>[ 'ELECTRONIC', 'PHYSICAL', 'REMOTE', 'VERBAL', 'DICTATE', 'FACE', 'PHONE', 'VIDEOCONF', 'WRITTEN', 'FAXWRIT', 'HANDWRIT', 'MAILWRIT', 'ONLINEWRIT', 'EMAILWRIT', 'TYPEWRIT' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'CommunicationRequest.medium',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'example', 'uri'=>'http://terminology.hl7.org/ValueSet/v3-ParticipationMode'}
      },
      ##
      # Focus of message
      # The patient or group that is the focus of this communication request.
      'subject' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Group'],
        'type'=>'Reference',
        'path'=>'CommunicationRequest.subject',
        'min'=>0,
        'max'=>1
      },
      ##
      # Resources that pertain to this communication request
      # Other resources that pertain to this communication request and to which this communication request should be associated.
      # Don't use CommunicationRequest.about element when a more specific element exists, such as basedOn, reasonReference, or replaces.
      'about' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'Reference',
        'path'=>'CommunicationRequest.about',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Encounter created as part of
      # The Encounter during which this CommunicationRequest was created or to which the creation of this record is tightly associated.
      # This will typically be the encounter the event occurred within, but some activities may be initiated prior to or after the official completion of an encounter but still be tied to the context of the encounter.
      'encounter' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Encounter'],
        'type'=>'Reference',
        'path'=>'CommunicationRequest.encounter',
        'min'=>0,
        'max'=>1
      },
      ##
      # Message payload
      # Text, attachment(s), or resource(s) to be communicated to the recipient.
      'payload' => {
        'type'=>'CommunicationRequest::Payload',
        'path'=>'CommunicationRequest.payload',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # When scheduled
      # The time when this communication is to occur.
      'occurrenceDateTime' => {
        'type'=>'DateTime',
        'path'=>'CommunicationRequest.occurrence[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # When scheduled
      # The time when this communication is to occur.
      'occurrencePeriod' => {
        'type'=>'Period',
        'path'=>'CommunicationRequest.occurrence[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # When request transitioned to being actionable
      # For draft requests, indicates the date of initial creation.  For requests with other statuses, indicates the date of activation.
      'authoredOn' => {
        'type'=>'dateTime',
        'path'=>'CommunicationRequest.authoredOn',
        'min'=>0,
        'max'=>1
      },
      ##
      # Who/what is requesting service
      # The device, individual, or organization who initiated the request and has responsibility for its activation.
      'requester' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/Device'],
        'type'=>'Reference',
        'path'=>'CommunicationRequest.requester',
        'min'=>0,
        'max'=>1
      },
      ##
      # Message recipient
      # The entity (e.g. person, organization, clinical information system, device, group, or care team) which is the intended target of the communication.
      'recipient' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/Group', 'http://hl7.org/fhir/StructureDefinition/CareTeam', 'http://hl7.org/fhir/StructureDefinition/HealthcareService'],
        'type'=>'Reference',
        'path'=>'CommunicationRequest.recipient',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Message sender
      # The entity (e.g. person, organization, clinical information system, or device) which is to be the source of the communication.
      'sender' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/HealthcareService'],
        'type'=>'Reference',
        'path'=>'CommunicationRequest.sender',
        'min'=>0,
        'max'=>1
      },
      ##
      # Why is communication needed?
      # Describes why the request is being made in coded or textual form.
      # Textual reasons can be captured using reasonCode.text.
      'reasonCode' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/v3-ActReason'=>[ 'ACCREQNA', 'FLRCNV', 'MEDNEC', 'PAT', 'COVSUS', 'DECSD', 'REGERR', 'AGE', 'CRIME', 'DIS', 'EMPLOY', 'FINAN', 'HEALTH', 'MULTI', 'PNC', 'STATUTORY', 'VEHIC', 'WORK', 'OVRER', 'OVRINCOMP', 'OVRPJ', 'OVRPS', 'OVRTPS', 'PurposeOfUse', 'HMARKT', 'HOPERAT', 'CAREMGT', 'DONAT', 'FRAUD', 'GOV', 'HACCRED', 'HCOMPL', 'HDECD', 'HDIRECT', 'HDM', 'HLEGAL', 'HOUTCOMS', 'HPRGRP', 'HQUALIMP', 'HSYSADMIN', 'LABELING', 'METAMGT', 'MEMADMIN', 'MILCDM', 'PATADMIN', 'PATSFTY', 'PERFMSR', 'RECORDMGT', 'SYSDEV', 'HTEST', 'TRAIN', 'HPAYMT', 'CLMATTCH', 'COVAUTH', 'COVERAGE', 'ELIGDTRM', 'ELIGVER', 'ENROLLM', 'MILDCRG', 'REMITADV', 'HRESCH', 'BIORCH', 'CLINTRCH', 'CLINTRCHNPC', 'CLINTRCHPC', 'PRECLINTRCH', 'DSRCH', 'POARCH', 'TRANSRCH', 'PATRQT', 'FAMRQT', 'PWATRNY', 'SUPNWK', 'PUBHLTH', 'DISASTER', 'THREAT', 'TREAT', 'CLINTRL', 'COC', 'ETREAT', 'BTG', 'ERTREAT', 'POPHLTH', 'MARKT', 'OPERAT', 'LEGAL', 'ACCRED', 'COMPL', 'ENADMIN', 'OUTCOMS', 'PRGRPT', 'QUALIMP', 'SYSADMN', 'PAYMT', 'RESCH', 'SRVC', '_ActInvalidReason', 'ADVSTORAGE', 'COLDCHNBRK', 'EXPLOT', 'OUTSIDESCHED', 'PRODRECALL', 'INCCOVPTY', 'INCINVOICE', 'INCPOLICY', 'INCPROV', 'IMMUNE', 'MEDPREC', 'OSTOCK', 'PATOBJ', 'PHILISOP', 'RELIG', 'VACEFF', 'VACSAF', 'FRR01', 'FRR02', 'FRR03', 'FRR04', 'FRR05', 'FRR06', 'RET', 'SCH', 'TRM', 'UNS', 'NPT', 'PPT', 'UPT', 'ALTCHOICE', 'CLARIF', 'DRUGHIGH', 'HOSPADM', 'LABINT', 'NON-AVAIL', 'PREG', 'SALG', 'SDDI', 'SDUPTHER', 'SINTOL', 'SURG', 'WASHOUT', '_ControlActNullificationReasonCode', 'ALTD', 'EIE', 'NORECMTCH', 'INRQSTATE', 'NOMATCH', 'NOPRODMTCH', 'NOSERMTCH', 'NOVERMTCH', 'NOPERM', 'NOUSERPERM', 'NOAGNTPERM', 'NOUSRPERM', 'WRNGVER', 'DISCONT', 'INEFFECT', 'MONIT', 'NOREQ', 'NOTCOVER', 'PREFUS', 'RECALL', 'REPLACE', 'DOSECHG', 'REPLACEFIX', 'UNABLE', 'HOLDDONE', 'HOLDINAP', 'ADMINERROR', 'CLINMOD', '_PharmacySupplyEventAbortReason', 'CONTRA', 'FOABORT', 'FOSUSP', 'NOPICK', 'PATDEC', 'QUANTCHG', 'FLRSTCK', 'LTC', 'OFFICE', 'PHARM', 'PROG', 'ALREADYRX', 'FAMPHYS', 'MODIFY', 'NEEDAPMT', 'NOTAVAIL', 'NOTPAT', 'ONHOLD', 'PRNA', 'STOPMED', 'TOOEARLY', 'IMPROV', 'INTOL', 'NEWSTR', 'NEWTHER', 'CHGDATA', 'FIXDATA', 'MDATA', 'NEWDATA', 'UMDATA', 'ADMREV', 'PATCAR', 'PATREQ', 'PRCREV', 'REGUL', 'RSRCH', 'VALIDATION', '_PharmacySupplyRequestFulfillerRevisionRefusalReasonCode', 'LOCKED', 'UNKWNTARGET', 'BLK', 'DEC', 'FIN', 'MED', 'MTG', 'PHY', '_StatusRevisionRefusalReasonCode', 'FILLED', '_SubstanceAdministrationPermissionRefusalReasonCode', 'PATINELIG', 'PROTUNMET', 'PROVUNAUTH', 'ALGINT', 'COMPCON', 'THERCHAR', 'TRIAL', 'CT', 'FP', 'OS', 'RR', 'ER', 'RQ', 'BONUS', 'CHD', 'DEP', 'ECH', 'EDU', 'EMP', 'ESP', 'FAM', 'IND', 'INVOICE', 'PROA', 'RECOV', 'RETRO', 'SPC', 'SPO', 'TRAN' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'CommunicationRequest.reasonCode',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'example', 'uri'=>'http://terminology.hl7.org/ValueSet/v3-ActReason'}
      },
      ##
      # Why is communication needed?
      # Indicates another resource whose existence justifies this request.
      'reasonReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Condition', 'http://hl7.org/fhir/StructureDefinition/Observation', 'http://hl7.org/fhir/StructureDefinition/DiagnosticReport', 'http://hl7.org/fhir/StructureDefinition/DocumentReference'],
        'type'=>'Reference',
        'path'=>'CommunicationRequest.reasonReference',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Comments made about communication request
      # Comments made about the request by the requester, sender, recipient, subject or other participants.
      'note' => {
        'type'=>'Annotation',
        'path'=>'CommunicationRequest.note',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Message payload
    # Text, attachment(s), or resource(s) to be communicated to the recipient.
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
        # The communicated content (or for multi-part communications, one portion of the communication).
        'contentAttachment' => {
          'type'=>'Attachment',
          'path'=>'Payload.content[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Message part content
        # The communicated content (or for multi-part communications, one portion of the communication).
        'contentReference' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
          'type'=>'Reference',
          'path'=>'Payload.content[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Message part content
        # The communicated content (or for multi-part communications, one portion of the communication).
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
      # The communicated content (or for multi-part communications, one portion of the communication).
      attr_accessor :contentAttachment              # 1-1 Attachment
      ##
      # Message part content
      # The communicated content (or for multi-part communications, one portion of the communication).
      attr_accessor :contentReference               # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Resource)
      ##
      # Message part content
      # The communicated content (or for multi-part communications, one portion of the communication).
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
    # Business identifiers assigned to this communication request by the performer or other systems which remain constant as the resource is updated and propagates from server to server.
    # This is a business identifier, not a resource identifier (see [discussion](resource.html#identifiers)).  It is best practice for the identifier to only appear on a single resource instance, however business practices may occasionally dictate that multiple resource instances with the same identifier can exist - possibly even with different resource types.  For example, multiple Patient and a Person resource instance might share the same social insurance number.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Fulfills plan or proposal
    # A plan or proposal that is fulfilled in whole or in part by this request.
    attr_accessor :basedOn                        # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Resource) ]
    ##
    # Request(s) replaced by this request
    # Completed or terminated request(s) whose function is taken by this new request.
    # The replacement could be because the initial request was immediately rejected (due to an issue) or because the previous request was completed, but the need for the action described by the request remains ongoing.
    attr_accessor :replaces                       # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/CommunicationRequest) ]
    ##
    # Composite request this is part of
    # A shared identifier common to all requests that were authorized more or less simultaneously by a single author, representing the identifier of the requisition, prescription or similar form.
    # Requests are linked either by a "basedOn" relationship (i.e. one request is fulfilling another) or by having a common requisition.  Requests that are part of the same requisition are generally treated independently from the perspective of changing their state or maintaining them after initial creation.
    attr_accessor :groupIdentifier                # 0-1 Identifier
    ##
    # draft | active | on-hold | revoked | completed | entered-in-error | unknown
    # The status of the proposal or order.
    attr_accessor :status                         # 1-1 code
    ##
    # Reason for current status
    # Captures the reason for the current state of the CommunicationRequest.
    # This is generally only used for "exception" statuses such as "suspended" or "cancelled".  The reason why the CommunicationRequest was created at all is captured in reasonCode, not here.  [distinct reason codes for different statuses can be enforced using invariants if they are universal bindings].
    attr_accessor :statusReason                   # 0-1 CodeableConcept
    ##
    # Message category
    # The type of message to be sent such as alert, notification, reminder, instruction, etc.
    # There may be multiple axes of categorization and one communication request may serve multiple purposes.
    attr_accessor :category                       # 0-* [ CodeableConcept ]
    ##
    # routine | urgent | asap | stat
    # Characterizes how quickly the proposed act must be initiated. Includes concepts such as stat, urgent, routine.
    attr_accessor :priority                       # 0-1 code
    ##
    # True if request is prohibiting action
    # If true indicates that the CommunicationRequest is asking for the specified action to *not* occur.
    # The attributes provided with the request qualify what is not to be done.
    attr_accessor :doNotPerform                   # 0-1 boolean
    ##
    # A channel of communication
    # A channel that was used for this communication (e.g. email, fax).
    attr_accessor :medium                         # 0-* [ CodeableConcept ]
    ##
    # Focus of message
    # The patient or group that is the focus of this communication request.
    attr_accessor :subject                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Group)
    ##
    # Resources that pertain to this communication request
    # Other resources that pertain to this communication request and to which this communication request should be associated.
    # Don't use CommunicationRequest.about element when a more specific element exists, such as basedOn, reasonReference, or replaces.
    attr_accessor :about                          # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Resource) ]
    ##
    # Encounter created as part of
    # The Encounter during which this CommunicationRequest was created or to which the creation of this record is tightly associated.
    # This will typically be the encounter the event occurred within, but some activities may be initiated prior to or after the official completion of an encounter but still be tied to the context of the encounter.
    attr_accessor :encounter                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Encounter)
    ##
    # Message payload
    # Text, attachment(s), or resource(s) to be communicated to the recipient.
    attr_accessor :payload                        # 0-* [ CommunicationRequest::Payload ]
    ##
    # When scheduled
    # The time when this communication is to occur.
    attr_accessor :occurrenceDateTime             # 0-1 DateTime
    ##
    # When scheduled
    # The time when this communication is to occur.
    attr_accessor :occurrencePeriod               # 0-1 Period
    ##
    # When request transitioned to being actionable
    # For draft requests, indicates the date of initial creation.  For requests with other statuses, indicates the date of activation.
    attr_accessor :authoredOn                     # 0-1 dateTime
    ##
    # Who/what is requesting service
    # The device, individual, or organization who initiated the request and has responsibility for its activation.
    attr_accessor :requester                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/Device)
    ##
    # Message recipient
    # The entity (e.g. person, organization, clinical information system, device, group, or care team) which is the intended target of the communication.
    attr_accessor :recipient                      # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/Group|http://hl7.org/fhir/StructureDefinition/CareTeam|http://hl7.org/fhir/StructureDefinition/HealthcareService) ]
    ##
    # Message sender
    # The entity (e.g. person, organization, clinical information system, or device) which is to be the source of the communication.
    attr_accessor :sender                         # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/HealthcareService)
    ##
    # Why is communication needed?
    # Describes why the request is being made in coded or textual form.
    # Textual reasons can be captured using reasonCode.text.
    attr_accessor :reasonCode                     # 0-* [ CodeableConcept ]
    ##
    # Why is communication needed?
    # Indicates another resource whose existence justifies this request.
    attr_accessor :reasonReference                # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Condition|http://hl7.org/fhir/StructureDefinition/Observation|http://hl7.org/fhir/StructureDefinition/DiagnosticReport|http://hl7.org/fhir/StructureDefinition/DocumentReference) ]
    ##
    # Comments made about communication request
    # Comments made about the request by the requester, sender, recipient, subject or other participants.
    attr_accessor :note                           # 0-* [ Annotation ]

    def resourceType
      'CommunicationRequest'
    end
  end
end
