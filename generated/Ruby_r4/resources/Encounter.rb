module FHIR

  ##
  # An interaction between a patient and healthcare provider(s) for the purpose of providing healthcare service(s) or assessing the health status of a patient.
  class Encounter < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['account', 'appointment', 'based-on', 'class', 'date', 'diagnosis', 'episode-of-care', 'identifier', 'length', 'location-period', 'location', 'part-of', 'participant-type', 'participant', 'patient', 'practitioner', 'reason-code', 'reason-reference', 'service-provider', 'special-arrangement', 'status', 'subject', 'type']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'Encounter.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'Encounter.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'Encounter.implicitRules',
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
        'path'=>'Encounter.language',
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
        'path'=>'Encounter.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'Encounter.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Encounter.extension',
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
        'path'=>'Encounter.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Identifier(s) by which this encounter is known.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'Encounter.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # planned | arrived | triaged | in-progress | onleave | finished | cancelled +.
      # Note that internal business rules will determine the appropriate transitions that may occur between statuses (and also classes).
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/encounter-status'=>[ 'planned', 'arrived', 'triaged', 'in-progress', 'onleave', 'finished', 'cancelled', 'entered-in-error', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'Encounter.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/encounter-status'}
      },
      ##
      # List of past encounter statuses
      # The status history permits the encounter resource to contain the status history without needing to read through the historical versions of the resource, or even have the server store them.
      # The current status is always found in the current version of the resource, not the status history.
      'statusHistory' => {
        'type'=>'Encounter::StatusHistory',
        'path'=>'Encounter.statusHistory',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Classification of patient encounter
      # Concepts representing classification of patient encounter such as ambulatory (outpatient), inpatient, emergency, home health or others due to local variations.
      'class' => {
        'local_name'=>'local_class'
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/v3-ActCode'=>[ 'AMB', 'EMER', 'FLD', 'HH', 'IMP', 'ACUTE', 'NONAC', 'OBSENC', 'PRENC', 'SS', 'VR' ]
        },
        'type'=>'Coding',
        'path'=>'Encounter.class',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://terminology.hl7.org/ValueSet/v3-ActEncounterCode'}
      },
      ##
      # List of past encounter classes
      # The class history permits the tracking of the encounters transitions without needing to go  through the resource history.  This would be used for a case where an admission starts of as an emergency encounter, then transitions into an inpatient scenario. Doing this and not restarting a new encounter ensures that any lab/diagnostic results can more easily follow the patient and not require re-processing and not get lost or cancelled during a kind of discharge from emergency to inpatient.
      'classHistory' => {
        'type'=>'Encounter::ClassHistory',
        'path'=>'Encounter.classHistory',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Specific type of encounter (e.g. e-mail consultation, surgical day-care, skilled nursing, rehabilitation).
      # Since there are many ways to further classify encounters, this element is 0..*.
      'type' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/encounter-type'=>[ 'ADMS', 'BD/BM-clin', 'CCS60', 'OKI' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Encounter.type',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/encounter-type'}
      },
      ##
      # Specific type of service
      # Broad categorization of the service that is to be provided (e.g. cardiology).
      'serviceType' => {
        'type'=>'CodeableConcept',
        'path'=>'Encounter.serviceType',
        'min'=>0,
        'max'=>1
      },
      ##
      # Indicates the urgency of the encounter.
      'priority' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/v3-ActPriority'=>[ 'A', 'CR', 'CS', 'CSP', 'CSR', 'EL', 'EM', 'P', 'PRN', 'R', 'RR', 'S', 'T', 'UD', 'UR' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Encounter.priority',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://terminology.hl7.org/ValueSet/v3-ActPriority'}
      },
      ##
      # The patient or group present at the encounter.
      # While the encounter is always about the patient, the patient might not actually be known in all contexts of use, and there may be a group of patients that could be anonymous (such as in a group therapy for Alcoholics Anonymous - where the recording of the encounter could be used for billing on the number of people/staff and not important to the context of the specific patients) or alternately in veterinary care a herd of sheep receiving treatment (where the animals are not individually tracked).
      'subject' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Group'],
        'type'=>'Reference',
        'path'=>'Encounter.subject',
        'min'=>0,
        'max'=>1
      },
      ##
      # Episode(s) of care that this encounter should be recorded against
      # Where a specific encounter should be classified as a part of a specific episode(s) of care this field should be used. This association can facilitate grouping of related encounters together for a specific purpose, such as government reporting, issue tracking, association via a common problem.  The association is recorded on the encounter as these are typically created after the episode of care and grouped on entry rather than editing the episode of care to append another encounter to it (the episode of care could span years).
      'episodeOfCare' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/EpisodeOfCare'],
        'type'=>'Reference',
        'path'=>'Encounter.episodeOfCare',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The ServiceRequest that initiated this encounter
      # The request this encounter satisfies (e.g. incoming referral or procedure request).
      'basedOn' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ServiceRequest'],
        'type'=>'Reference',
        'path'=>'Encounter.basedOn',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # List of participants involved in the encounter
      # The list of people responsible for providing the service.
      'participant' => {
        'type'=>'Encounter::Participant',
        'path'=>'Encounter.participant',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The appointment that scheduled this encounter.
      'appointment' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Appointment'],
        'type'=>'Reference',
        'path'=>'Encounter.appointment',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The start and end time of the encounter.
      # If not (yet) known, the end of the Period may be omitted.
      'period' => {
        'type'=>'Period',
        'path'=>'Encounter.period',
        'min'=>0,
        'max'=>1
      },
      ##
      # Quantity of time the encounter lasted (less time absent)
      # Quantity of time the encounter lasted. This excludes the time during leaves of absence.
      # May differ from the time the Encounter.period lasted because of leave of absence.
      'length' => {
        'type'=>'Duration',
        'path'=>'Encounter.length',
        'min'=>0,
        'max'=>1
      },
      ##
      # Coded reason the encounter takes place
      # Reason the encounter takes place, expressed as a code. For admissions, this can be used for a coded admission diagnosis.
      # For systems that need to know which was the primary diagnosis, these will be marked with the standard extension primaryDiagnosis (which is a sequence value rather than a flag, 1 = primary diagnosis).
      'reasonCode' => {
        'type'=>'CodeableConcept',
        'path'=>'Encounter.reasonCode',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Reason the encounter takes place (reference)
      # Reason the encounter takes place, expressed as a code. For admissions, this can be used for a coded admission diagnosis.
      # For systems that need to know which was the primary diagnosis, these will be marked with the standard extension primaryDiagnosis (which is a sequence value rather than a flag, 1 = primary diagnosis).
      'reasonReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Condition', 'http://hl7.org/fhir/StructureDefinition/Procedure', 'http://hl7.org/fhir/StructureDefinition/Observation', 'http://hl7.org/fhir/StructureDefinition/ImmunizationRecommendation'],
        'type'=>'Reference',
        'path'=>'Encounter.reasonReference',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The list of diagnosis relevant to this encounter.
      'diagnosis' => {
        'type'=>'Encounter::Diagnosis',
        'path'=>'Encounter.diagnosis',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The set of accounts that may be used for billing for this Encounter.
      # The billing system may choose to allocate billable items associated with the Encounter to different referenced Accounts based on internal business rules.
      'account' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Account'],
        'type'=>'Reference',
        'path'=>'Encounter.account',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Details about the admission to a healthcare service.
      # An Encounter may cover more than just the inpatient stay. Contexts such as outpatients, community clinics, and aged care facilities are also included.The duration recorded in the period of this encounter covers the entire scope of this hospitalization record.
      'hospitalization' => {
        'type'=>'Encounter::Hospitalization',
        'path'=>'Encounter.hospitalization',
        'min'=>0,
        'max'=>1
      },
      ##
      # List of locations where the patient has been
      # List of locations where  the patient has been during this encounter.
      # Virtual encounters can be recorded in the Encounter by specifying a location reference to a location of type "kind" such as "client's home" and an encounter.class = "virtual".
      'location' => {
        'type'=>'Encounter::Location',
        'path'=>'Encounter.location',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The organization (facility) responsible for this encounter
      # The organization that is primarily responsible for this Encounter's services. This MAY be the same as the organization on the Patient record, however it could be different, such as if the actor performing the services was from an external organization (which may be billed seperately) for an external consultation.  Refer to the example bundle showing an abbreviated set of Encounters for a colonoscopy.
      'serviceProvider' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'Encounter.serviceProvider',
        'min'=>0,
        'max'=>1
      },
      ##
      # Another Encounter this encounter is part of
      # Another Encounter of which this encounter is a part of (administratively or in time).
      # This is also used for associating a child's encounter back to the mother's encounter.Refer to the Notes section in the Patient resource for further details.
      'partOf' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Encounter'],
        'type'=>'Reference',
        'path'=>'Encounter.partOf',
        'min'=>0,
        'max'=>1
      }
    }

    ##
    # List of past encounter statuses
    # The status history permits the encounter resource to contain the status history without needing to read through the historical versions of the resource, or even have the server store them.
    # The current status is always found in the current version of the resource, not the status history.
    class StatusHistory < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'StatusHistory.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'StatusHistory.extension',
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
          'path'=>'StatusHistory.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # planned | arrived | triaged | in-progress | onleave | finished | cancelled +.
        'status' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/encounter-status'=>[ 'planned', 'arrived', 'triaged', 'in-progress', 'onleave', 'finished', 'cancelled', 'entered-in-error', 'unknown' ]
          },
          'type'=>'code',
          'path'=>'StatusHistory.status',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/encounter-status'}
        },
        ##
        # The time that the episode was in the specified status.
        'period' => {
          'type'=>'Period',
          'path'=>'StatusHistory.period',
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
      # Extensions that cannot be ignored even if unrecognized
      # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
      # 
      # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      attr_accessor :modifierExtension              # 0-* [ Extension ]
      ##
      # planned | arrived | triaged | in-progress | onleave | finished | cancelled +.
      attr_accessor :status                         # 1-1 code
      ##
      # The time that the episode was in the specified status.
      attr_accessor :period                         # 1-1 Period
    end

    ##
    # List of past encounter classes
    # The class history permits the tracking of the encounters transitions without needing to go  through the resource history.  This would be used for a case where an admission starts of as an emergency encounter, then transitions into an inpatient scenario. Doing this and not restarting a new encounter ensures that any lab/diagnostic results can more easily follow the patient and not require re-processing and not get lost or cancelled during a kind of discharge from emergency to inpatient.
    class ClassHistory < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'ClassHistory.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'ClassHistory.extension',
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
          'path'=>'ClassHistory.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # inpatient | outpatient | ambulatory | emergency +.
        'class' => {
          'local_name'=>'local_class'
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/v3-ActCode'=>[ 'AMB', 'EMER', 'FLD', 'HH', 'IMP', 'ACUTE', 'NONAC', 'OBSENC', 'PRENC', 'SS', 'VR' ]
          },
          'type'=>'Coding',
          'path'=>'ClassHistory.class',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'extensible', 'uri'=>'http://terminology.hl7.org/ValueSet/v3-ActEncounterCode'}
        },
        ##
        # The time that the episode was in the specified class.
        'period' => {
          'type'=>'Period',
          'path'=>'ClassHistory.period',
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
      # Extensions that cannot be ignored even if unrecognized
      # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
      # 
      # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      attr_accessor :modifierExtension              # 0-* [ Extension ]
      ##
      # inpatient | outpatient | ambulatory | emergency +.
      attr_accessor :local_class                    # 1-1 Coding
      ##
      # The time that the episode was in the specified class.
      attr_accessor :period                         # 1-1 Period
    end

    ##
    # List of participants involved in the encounter
    # The list of people responsible for providing the service.
    class Participant < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
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
        # Role of participant in encounter.
        # The participant type indicates how an individual participates in an encounter. It includes non-practitioner participants, and for practitioners this is to describe the action type in the context of this encounter (e.g. Admitting Dr, Attending Dr, Translator, Consulting Dr). This is different to the practitioner roles which are functional roles, derived from terms of employment, education, licensing, etc.
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
        # Period of time during the encounter that the participant participated
        # The period of time that the specified participant participated in the encounter. These can overlap or be sub-sets of the overall encounter's period.
        'period' => {
          'type'=>'Period',
          'path'=>'Participant.period',
          'min'=>0,
          'max'=>1
        },
        ##
        # Persons involved in the encounter other than the patient.
        'individual' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson'],
          'type'=>'Reference',
          'path'=>'Participant.individual',
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
      # Extensions that cannot be ignored even if unrecognized
      # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
      # 
      # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      attr_accessor :modifierExtension              # 0-* [ Extension ]
      ##
      # Role of participant in encounter.
      # The participant type indicates how an individual participates in an encounter. It includes non-practitioner participants, and for practitioners this is to describe the action type in the context of this encounter (e.g. Admitting Dr, Attending Dr, Translator, Consulting Dr). This is different to the practitioner roles which are functional roles, derived from terms of employment, education, licensing, etc.
      attr_accessor :type                           # 0-* [ CodeableConcept ]
      ##
      # Period of time during the encounter that the participant participated
      # The period of time that the specified participant participated in the encounter. These can overlap or be sub-sets of the overall encounter's period.
      attr_accessor :period                         # 0-1 Period
      ##
      # Persons involved in the encounter other than the patient.
      attr_accessor :individual                     # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/RelatedPerson)
    end

    ##
    # The list of diagnosis relevant to this encounter.
    class Diagnosis < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Diagnosis.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Diagnosis.extension',
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
          'path'=>'Diagnosis.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # The diagnosis or procedure relevant to the encounter
        # Reason the encounter takes place, as specified using information from another resource. For admissions, this is the admission diagnosis. The indication will typically be a Condition (with other resources referenced in the evidence.detail), or a Procedure.
        # For systems that need to know which was the primary diagnosis, these will be marked with the standard extension primaryDiagnosis (which is a sequence value rather than a flag, 1 = primary diagnosis).
        'condition' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Condition', 'http://hl7.org/fhir/StructureDefinition/Procedure'],
          'type'=>'Reference',
          'path'=>'Diagnosis.condition',
          'min'=>1,
          'max'=>1
        },
        ##
        # Role that this diagnosis has within the encounter (e.g. admission, billing, discharge …).
        'use' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/diagnosis-role'=>[ 'AD', 'DD', 'CC', 'CM', 'pre-op', 'post-op', 'billing' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Diagnosis.use',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/diagnosis-role'}
        },
        ##
        # Ranking of the diagnosis (for each role type).
        'rank' => {
          'type'=>'positiveInt',
          'path'=>'Diagnosis.rank',
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
      # Extensions that cannot be ignored even if unrecognized
      # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
      # 
      # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      attr_accessor :modifierExtension              # 0-* [ Extension ]
      ##
      # The diagnosis or procedure relevant to the encounter
      # Reason the encounter takes place, as specified using information from another resource. For admissions, this is the admission diagnosis. The indication will typically be a Condition (with other resources referenced in the evidence.detail), or a Procedure.
      # For systems that need to know which was the primary diagnosis, these will be marked with the standard extension primaryDiagnosis (which is a sequence value rather than a flag, 1 = primary diagnosis).
      attr_accessor :condition                      # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Condition|http://hl7.org/fhir/StructureDefinition/Procedure)
      ##
      # Role that this diagnosis has within the encounter (e.g. admission, billing, discharge …).
      attr_accessor :use                            # 0-1 CodeableConcept
      ##
      # Ranking of the diagnosis (for each role type).
      attr_accessor :rank                           # 0-1 positiveInt
    end

    ##
    # Details about the admission to a healthcare service.
    # An Encounter may cover more than just the inpatient stay. Contexts such as outpatients, community clinics, and aged care facilities are also included.The duration recorded in the period of this encounter covers the entire scope of this hospitalization record.
    class Hospitalization < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Hospitalization.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Hospitalization.extension',
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
          'path'=>'Hospitalization.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Pre-admission identifier.
        'preAdmissionIdentifier' => {
          'type'=>'Identifier',
          'path'=>'Hospitalization.preAdmissionIdentifier',
          'min'=>0,
          'max'=>1
        },
        ##
        # The location/organization from which the patient came before admission.
        'origin' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Location', 'http://hl7.org/fhir/StructureDefinition/Organization'],
          'type'=>'Reference',
          'path'=>'Hospitalization.origin',
          'min'=>0,
          'max'=>1
        },
        ##
        # From where patient was admitted (physician referral, transfer).
        'admitSource' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/admit-source'=>[ 'hosp-trans', 'emd', 'outp', 'born', 'gp', 'mp', 'nursing', 'psych', 'rehab', 'other' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Hospitalization.admitSource',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/encounter-admit-source'}
        },
        ##
        # The type of hospital re-admission that has occurred (if any). If the value is absent, then this is not identified as a readmission
        # Whether this hospitalization is a readmission and why if known.
        'reAdmission' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/v2-0092'=>[ 'R' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Hospitalization.reAdmission',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://terminology.hl7.org/ValueSet/v2-0092'}
        },
        ##
        # Diet preferences reported by the patient.
        # For example, a patient may request both a dairy-free and nut-free diet preference (not mutually exclusive).
        'dietPreference' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/diet'=>[ 'vegetarian', 'dairy-free', 'nut-free', 'gluten-free', 'vegan', 'halal', 'kosher' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Hospitalization.dietPreference',
          'min'=>0,
          'max'=>Float::INFINITY,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/encounter-diet'}
        },
        ##
        # Special courtesies (VIP, board member).
        'specialCourtesy' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/v3-EncounterSpecialCourtesy'=>[ 'EXT', 'NRM', 'PRF', 'STF', 'VIP' ],
            'http://terminology.hl7.org/CodeSystem/v3-NullFlavor'=>[ 'UNK' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Hospitalization.specialCourtesy',
          'min'=>0,
          'max'=>Float::INFINITY,
          'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/encounter-special-courtesy'}
        },
        ##
        # Wheelchair, translator, stretcher, etc.
        # Any special requests that have been made for this hospitalization encounter, such as the provision of specific equipment or other things.
        'specialArrangement' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/encounter-special-arrangements'=>[ 'wheel', 'add-bed', 'int', 'att', 'dog' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Hospitalization.specialArrangement',
          'min'=>0,
          'max'=>Float::INFINITY,
          'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/encounter-special-arrangements'}
        },
        ##
        # Location/organization to which the patient is discharged.
        'destination' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Location', 'http://hl7.org/fhir/StructureDefinition/Organization'],
          'type'=>'Reference',
          'path'=>'Hospitalization.destination',
          'min'=>0,
          'max'=>1
        },
        ##
        # Category or kind of location after discharge.
        'dischargeDisposition' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/discharge-disposition'=>[ 'home', 'alt-home', 'other-hcf', 'hosp', 'long', 'aadvice', 'exp', 'psy', 'rehab', 'snf', 'oth' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Hospitalization.dischargeDisposition',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/encounter-discharge-disposition'}
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
      # Extensions that cannot be ignored even if unrecognized
      # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
      # 
      # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      attr_accessor :modifierExtension              # 0-* [ Extension ]
      ##
      # Pre-admission identifier.
      attr_accessor :preAdmissionIdentifier         # 0-1 Identifier
      ##
      # The location/organization from which the patient came before admission.
      attr_accessor :origin                         # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Location|http://hl7.org/fhir/StructureDefinition/Organization)
      ##
      # From where patient was admitted (physician referral, transfer).
      attr_accessor :admitSource                    # 0-1 CodeableConcept
      ##
      # The type of hospital re-admission that has occurred (if any). If the value is absent, then this is not identified as a readmission
      # Whether this hospitalization is a readmission and why if known.
      attr_accessor :reAdmission                    # 0-1 CodeableConcept
      ##
      # Diet preferences reported by the patient.
      # For example, a patient may request both a dairy-free and nut-free diet preference (not mutually exclusive).
      attr_accessor :dietPreference                 # 0-* [ CodeableConcept ]
      ##
      # Special courtesies (VIP, board member).
      attr_accessor :specialCourtesy                # 0-* [ CodeableConcept ]
      ##
      # Wheelchair, translator, stretcher, etc.
      # Any special requests that have been made for this hospitalization encounter, such as the provision of specific equipment or other things.
      attr_accessor :specialArrangement             # 0-* [ CodeableConcept ]
      ##
      # Location/organization to which the patient is discharged.
      attr_accessor :destination                    # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Location|http://hl7.org/fhir/StructureDefinition/Organization)
      ##
      # Category or kind of location after discharge.
      attr_accessor :dischargeDisposition           # 0-1 CodeableConcept
    end

    ##
    # List of locations where the patient has been
    # List of locations where  the patient has been during this encounter.
    # Virtual encounters can be recorded in the Encounter by specifying a location reference to a location of type "kind" such as "client's home" and an encounter.class = "virtual".
    class Location < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Location.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Location.extension',
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
          'path'=>'Location.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Location the encounter takes place
        # The location where the encounter takes place.
        'location' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Location'],
          'type'=>'Reference',
          'path'=>'Location.location',
          'min'=>1,
          'max'=>1
        },
        ##
        # planned | active | reserved | completed
        # The status of the participants' presence at the specified location during the period specified. If the participant is no longer at the location, then the period will have an end date/time.
        # When the patient is no longer active at a location, then the period end date is entered, and the status may be changed to completed.
        'status' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/encounter-location-status'=>[ 'planned', 'active', 'reserved', 'completed' ]
          },
          'type'=>'code',
          'path'=>'Location.status',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/encounter-location-status'}
        },
        ##
        # The physical type of the location (usually the level in the location hierachy - bed room ward etc.)
        # This will be used to specify the required levels (bed/ward/room/etc.) desired to be recorded to simplify either messaging or query.
        # This information is de-normalized from the Location resource to support the easier understanding of the encounter resource and processing in messaging or query.
        # 
        # There may be many levels in the hierachy, and this may only pic specific levels that are required for a specific usage scenario.
        'physicalType' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/location-physical-type'=>[ 'si', 'bu', 'wi', 'wa', 'lvl', 'co', 'ro', 'bd', 've', 'ho', 'ca', 'rd', 'area', 'jdn' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Location.physicalType',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/location-physical-type'}
        },
        ##
        # Time period during which the patient was present at the location.
        'period' => {
          'type'=>'Period',
          'path'=>'Location.period',
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
      # Extensions that cannot be ignored even if unrecognized
      # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
      # 
      # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      attr_accessor :modifierExtension              # 0-* [ Extension ]
      ##
      # Location the encounter takes place
      # The location where the encounter takes place.
      attr_accessor :location                       # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Location)
      ##
      # planned | active | reserved | completed
      # The status of the participants' presence at the specified location during the period specified. If the participant is no longer at the location, then the period will have an end date/time.
      # When the patient is no longer active at a location, then the period end date is entered, and the status may be changed to completed.
      attr_accessor :status                         # 0-1 code
      ##
      # The physical type of the location (usually the level in the location hierachy - bed room ward etc.)
      # This will be used to specify the required levels (bed/ward/room/etc.) desired to be recorded to simplify either messaging or query.
      # This information is de-normalized from the Location resource to support the easier understanding of the encounter resource and processing in messaging or query.
      # 
      # There may be many levels in the hierachy, and this may only pic specific levels that are required for a specific usage scenario.
      attr_accessor :physicalType                   # 0-1 CodeableConcept
      ##
      # Time period during which the patient was present at the location.
      attr_accessor :period                         # 0-1 Period
    end
    ##
    # Logical id of this artifact
    # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
    # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
    attr_accessor :id                             # 0-1 string
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
    # Identifier(s) by which this encounter is known.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # planned | arrived | triaged | in-progress | onleave | finished | cancelled +.
    # Note that internal business rules will determine the appropriate transitions that may occur between statuses (and also classes).
    attr_accessor :status                         # 1-1 code
    ##
    # List of past encounter statuses
    # The status history permits the encounter resource to contain the status history without needing to read through the historical versions of the resource, or even have the server store them.
    # The current status is always found in the current version of the resource, not the status history.
    attr_accessor :statusHistory                  # 0-* [ Encounter::StatusHistory ]
    ##
    # Classification of patient encounter
    # Concepts representing classification of patient encounter such as ambulatory (outpatient), inpatient, emergency, home health or others due to local variations.
    attr_accessor :local_class                    # 1-1 Coding
    ##
    # List of past encounter classes
    # The class history permits the tracking of the encounters transitions without needing to go  through the resource history.  This would be used for a case where an admission starts of as an emergency encounter, then transitions into an inpatient scenario. Doing this and not restarting a new encounter ensures that any lab/diagnostic results can more easily follow the patient and not require re-processing and not get lost or cancelled during a kind of discharge from emergency to inpatient.
    attr_accessor :classHistory                   # 0-* [ Encounter::ClassHistory ]
    ##
    # Specific type of encounter (e.g. e-mail consultation, surgical day-care, skilled nursing, rehabilitation).
    # Since there are many ways to further classify encounters, this element is 0..*.
    attr_accessor :type                           # 0-* [ CodeableConcept ]
    ##
    # Specific type of service
    # Broad categorization of the service that is to be provided (e.g. cardiology).
    attr_accessor :serviceType                    # 0-1 CodeableConcept
    ##
    # Indicates the urgency of the encounter.
    attr_accessor :priority                       # 0-1 CodeableConcept
    ##
    # The patient or group present at the encounter.
    # While the encounter is always about the patient, the patient might not actually be known in all contexts of use, and there may be a group of patients that could be anonymous (such as in a group therapy for Alcoholics Anonymous - where the recording of the encounter could be used for billing on the number of people/staff and not important to the context of the specific patients) or alternately in veterinary care a herd of sheep receiving treatment (where the animals are not individually tracked).
    attr_accessor :subject                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Group)
    ##
    # Episode(s) of care that this encounter should be recorded against
    # Where a specific encounter should be classified as a part of a specific episode(s) of care this field should be used. This association can facilitate grouping of related encounters together for a specific purpose, such as government reporting, issue tracking, association via a common problem.  The association is recorded on the encounter as these are typically created after the episode of care and grouped on entry rather than editing the episode of care to append another encounter to it (the episode of care could span years).
    attr_accessor :episodeOfCare                  # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/EpisodeOfCare) ]
    ##
    # The ServiceRequest that initiated this encounter
    # The request this encounter satisfies (e.g. incoming referral or procedure request).
    attr_accessor :basedOn                        # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/ServiceRequest) ]
    ##
    # List of participants involved in the encounter
    # The list of people responsible for providing the service.
    attr_accessor :participant                    # 0-* [ Encounter::Participant ]
    ##
    # The appointment that scheduled this encounter.
    attr_accessor :appointment                    # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Appointment) ]
    ##
    # The start and end time of the encounter.
    # If not (yet) known, the end of the Period may be omitted.
    attr_accessor :period                         # 0-1 Period
    ##
    # Quantity of time the encounter lasted (less time absent)
    # Quantity of time the encounter lasted. This excludes the time during leaves of absence.
    # May differ from the time the Encounter.period lasted because of leave of absence.
    attr_accessor :length                         # 0-1 Duration
    ##
    # Coded reason the encounter takes place
    # Reason the encounter takes place, expressed as a code. For admissions, this can be used for a coded admission diagnosis.
    # For systems that need to know which was the primary diagnosis, these will be marked with the standard extension primaryDiagnosis (which is a sequence value rather than a flag, 1 = primary diagnosis).
    attr_accessor :reasonCode                     # 0-* [ CodeableConcept ]
    ##
    # Reason the encounter takes place (reference)
    # Reason the encounter takes place, expressed as a code. For admissions, this can be used for a coded admission diagnosis.
    # For systems that need to know which was the primary diagnosis, these will be marked with the standard extension primaryDiagnosis (which is a sequence value rather than a flag, 1 = primary diagnosis).
    attr_accessor :reasonReference                # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Condition|http://hl7.org/fhir/StructureDefinition/Procedure|http://hl7.org/fhir/StructureDefinition/Observation|http://hl7.org/fhir/StructureDefinition/ImmunizationRecommendation) ]
    ##
    # The list of diagnosis relevant to this encounter.
    attr_accessor :diagnosis                      # 0-* [ Encounter::Diagnosis ]
    ##
    # The set of accounts that may be used for billing for this Encounter.
    # The billing system may choose to allocate billable items associated with the Encounter to different referenced Accounts based on internal business rules.
    attr_accessor :account                        # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Account) ]
    ##
    # Details about the admission to a healthcare service.
    # An Encounter may cover more than just the inpatient stay. Contexts such as outpatients, community clinics, and aged care facilities are also included.The duration recorded in the period of this encounter covers the entire scope of this hospitalization record.
    attr_accessor :hospitalization                # 0-1 Encounter::Hospitalization
    ##
    # List of locations where the patient has been
    # List of locations where  the patient has been during this encounter.
    # Virtual encounters can be recorded in the Encounter by specifying a location reference to a location of type "kind" such as "client's home" and an encounter.class = "virtual".
    attr_accessor :location                       # 0-* [ Encounter::Location ]
    ##
    # The organization (facility) responsible for this encounter
    # The organization that is primarily responsible for this Encounter's services. This MAY be the same as the organization on the Patient record, however it could be different, such as if the actor performing the services was from an external organization (which may be billed seperately) for an external consultation.  Refer to the example bundle showing an abbreviated set of Encounters for a colonoscopy.
    attr_accessor :serviceProvider                # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Another Encounter this encounter is part of
    # Another Encounter of which this encounter is a part of (administratively or in time).
    # This is also used for associating a child's encounter back to the mother's encounter.Refer to the Notes section in the Patient resource for further details.
    attr_accessor :partOf                         # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Encounter)

    def resourceType
      'Encounter'
    end
  end
end
