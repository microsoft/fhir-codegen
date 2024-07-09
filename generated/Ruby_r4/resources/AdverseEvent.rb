module FHIR

  ##
  # Actual or  potential/avoided event causing unintended physical injury resulting from or contributed to by medical care, a research study or other healthcare setting factors that requires additional monitoring, treatment, or hospitalization, or that results in death.
  class AdverseEvent < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['actuality', 'category', 'date', 'event', 'location', 'recorder', 'resultingcondition', 'seriousness', 'severity', 'study', 'subject', 'substance']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'AdverseEvent.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'AdverseEvent.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'AdverseEvent.implicitRules',
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
        'path'=>'AdverseEvent.language',
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
        'path'=>'AdverseEvent.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'AdverseEvent.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'AdverseEvent.extension',
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
        'path'=>'AdverseEvent.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Business identifier for the event
      # Business identifiers assigned to this adverse event by the performer or other systems which remain constant as the resource is updated and propagates from server to server.
      # This is a business identifier, not a resource identifier (see [discussion](resource.html#identifiers)).  It is best practice for the identifier to only appear on a single resource instance, however business practices may occasionally dictate that multiple resource instances with the same identifier can exist - possibly even with different resource types.  For example, multiple Patient and a Person resource instance might share the same social insurance number.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'AdverseEvent.identifier',
        'min'=>0,
        'max'=>1
      },
      ##
      # actual | potential
      # Whether the event actually happened, or just had the potential to. Note that this is independent of whether anyone was affected or harmed or how severely.
      'actuality' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/adverse-event-actuality'=>[ 'actual', 'potential' ]
        },
        'type'=>'code',
        'path'=>'AdverseEvent.actuality',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/adverse-event-actuality'}
      },
      ##
      # product-problem | product-quality | product-use-error | wrong-dose | incorrect-prescribing-information | wrong-technique | wrong-route-of-administration | wrong-rate | wrong-duration | wrong-time | expired-drug | medical-device-use-error | problem-different-manufacturer | unsafe-physical-environment
      # The overall type of event, intended for search and filtering purposes.
      'category' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/adverse-event-category'=>[ 'product-problem', 'product-quality', 'product-use-error', 'wrong-dose', 'incorrect-prescribing-information', 'wrong-technique', 'wrong-route-of-administration', 'wrong-rate', 'wrong-duration', 'wrong-time', 'expired-drug', 'medical-device-use-error', 'problem-different-manufacturer', 'unsafe-physical-environment' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'AdverseEvent.category',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/adverse-event-category'}
      },
      ##
      # Type of the event itself in relation to the subject
      # This element defines the specific type of event that occurred or that was prevented from occurring.
      'event' => {
        'type'=>'CodeableConcept',
        'path'=>'AdverseEvent.event',
        'min'=>0,
        'max'=>1
      },
      ##
      # Subject impacted by event
      # This subject or group impacted by the event.
      # If AdverseEvent.resultingCondition differs among members of the group, then use Patient as the subject.
      'subject' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Group', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson'],
        'type'=>'Reference',
        'path'=>'AdverseEvent.subject',
        'min'=>1,
        'max'=>1
      },
      ##
      # Encounter created as part of
      # The Encounter during which AdverseEvent was created or to which the creation of this record is tightly associated.
      # This will typically be the encounter the event occurred within, but some activities may be initiated prior to or after the official completion of an encounter but still be tied to the context of the encounter.  For example, if a medication administration was considered an adverse event because it resulted in a rash, then the encounter when the medication administration was given is the context.  If the patient reports the AdverseEvent during a second encounter, that second encounter is not the context.
      'encounter' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Encounter'],
        'type'=>'Reference',
        'path'=>'AdverseEvent.encounter',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the event occurred
      # The date (and perhaps time) when the adverse event occurred.
      'date' => {
        'type'=>'dateTime',
        'path'=>'AdverseEvent.date',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the event was detected
      # Estimated or actual date the AdverseEvent began, in the opinion of the reporter.
      'detected' => {
        'type'=>'dateTime',
        'path'=>'AdverseEvent.detected',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the event was recorded
      # The date on which the existence of the AdverseEvent was first recorded.
      # The recordedDate represents the date when this particular AdverseEvent record was created in the system, not the date of the most recent update.  The date of the last record modification can be retrieved from the resource metadata.
      'recordedDate' => {
        'type'=>'dateTime',
        'path'=>'AdverseEvent.recordedDate',
        'min'=>0,
        'max'=>1
      },
      ##
      # Effect on the subject due to this event
      # Includes information about the reaction that occurred as a result of exposure to a substance (for example, a drug or a chemical).
      'resultingCondition' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Condition'],
        'type'=>'Reference',
        'path'=>'AdverseEvent.resultingCondition',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Location where adverse event occurred
      # The information about where the adverse event occurred.
      'location' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Location'],
        'type'=>'Reference',
        'path'=>'AdverseEvent.location',
        'min'=>0,
        'max'=>1
      },
      ##
      # Seriousness of the event
      # Assessment whether this event was of real importance.
      'seriousness' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/adverse-event-seriousness'=>[ 'Non-serious', 'Serious', 'SeriousResultsInDeath', 'SeriousIsLifeThreatening', 'SeriousResultsInHospitalization', 'SeriousResultsInDisability', 'SeriousIsBirthDefect', 'SeriousRequiresPreventImpairment' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'AdverseEvent.seriousness',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/adverse-event-seriousness'}
      },
      ##
      # mild | moderate | severe
      # Describes the severity of the adverse event, in relation to the subject. Contrast to AdverseEvent.seriousness - a severe rash might not be serious, but a mild heart problem is.
      'severity' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/adverse-event-severity'=>[ 'mild', 'moderate', 'severe' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'AdverseEvent.severity',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/adverse-event-severity'}
      },
      ##
      # resolved | recovering | ongoing | resolvedWithSequelae | fatal | unknown
      # Describes the type of outcome from the adverse event.
      'outcome' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/adverse-event-outcome'=>[ 'resolved', 'recovering', 'ongoing', 'resolvedWithSequelae', 'fatal', 'unknown' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'AdverseEvent.outcome',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/adverse-event-outcome'}
      },
      ##
      # Who recorded the adverse event
      # Information on who recorded the adverse event.  May be the patient or a practitioner.
      'recorder' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson'],
        'type'=>'Reference',
        'path'=>'AdverseEvent.recorder',
        'min'=>0,
        'max'=>1
      },
      ##
      # Who  was involved in the adverse event or the potential adverse event
      # Parties that may or should contribute or have contributed information to the adverse event, which can consist of one or more activities.  Such information includes information leading to the decision to perform the activity and how to perform the activity (e.g. consultant), information that the activity itself seeks to reveal (e.g. informant of clinical history), or information about what activity was performed (e.g. informant witness).
      'contributor' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Device'],
        'type'=>'Reference',
        'path'=>'AdverseEvent.contributor',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The suspected agent causing the adverse event
      # Describes the entity that is suspected to have caused the adverse event.
      'suspectEntity' => {
        'type'=>'AdverseEvent::SuspectEntity',
        'path'=>'AdverseEvent.suspectEntity',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # AdverseEvent.subjectMedicalHistory.
      'subjectMedicalHistory' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Condition', 'http://hl7.org/fhir/StructureDefinition/Observation', 'http://hl7.org/fhir/StructureDefinition/AllergyIntolerance', 'http://hl7.org/fhir/StructureDefinition/FamilyMemberHistory', 'http://hl7.org/fhir/StructureDefinition/Immunization', 'http://hl7.org/fhir/StructureDefinition/Procedure', 'http://hl7.org/fhir/StructureDefinition/Media', 'http://hl7.org/fhir/StructureDefinition/DocumentReference'],
        'type'=>'Reference',
        'path'=>'AdverseEvent.subjectMedicalHistory',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # AdverseEvent.referenceDocument.
      'referenceDocument' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/DocumentReference'],
        'type'=>'Reference',
        'path'=>'AdverseEvent.referenceDocument',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # AdverseEvent.study.
      'study' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ResearchStudy'],
        'type'=>'Reference',
        'path'=>'AdverseEvent.study',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # The suspected agent causing the adverse event
    # Describes the entity that is suspected to have caused the adverse event.
    class SuspectEntity < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'SuspectEntity.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'SuspectEntity.extension',
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
          'path'=>'SuspectEntity.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Refers to the specific entity that caused the adverse event
        # Identifies the actual instance of what caused the adverse event.  May be a substance, medication, medication administration, medication statement or a device.
        'instance' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Immunization', 'http://hl7.org/fhir/StructureDefinition/Procedure', 'http://hl7.org/fhir/StructureDefinition/Substance', 'http://hl7.org/fhir/StructureDefinition/Medication', 'http://hl7.org/fhir/StructureDefinition/MedicationAdministration', 'http://hl7.org/fhir/StructureDefinition/MedicationStatement', 'http://hl7.org/fhir/StructureDefinition/Device'],
          'type'=>'Reference',
          'path'=>'SuspectEntity.instance',
          'min'=>1,
          'max'=>1
        },
        ##
        # Information on the possible cause of the event.
        'causality' => {
          'type'=>'AdverseEvent::SuspectEntity::Causality',
          'path'=>'SuspectEntity.causality',
          'min'=>0,
          'max'=>Float::INFINITY
        }
      }

      ##
      # Information on the possible cause of the event.
      class Causality < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
            'path'=>'Causality.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Causality.extension',
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
            'path'=>'Causality.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Assessment of if the entity caused the event.
          'assessment' => {
            'valid_codes'=>{
              'http://terminology.hl7.org/CodeSystem/adverse-event-causality-assess'=>[ 'Certain', 'Probably-Likely', 'Possible', 'Unlikely', 'Conditional-Classified', 'Unassessable-Unclassifiable' ]
            },
            'type'=>'CodeableConcept',
            'path'=>'Causality.assessment',
            'min'=>0,
            'max'=>1,
            'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/adverse-event-causality-assess'}
          },
          ##
          # AdverseEvent.suspectEntity.causalityProductRelatedness.
          'productRelatedness' => {
            'type'=>'string',
            'path'=>'Causality.productRelatedness',
            'min'=>0,
            'max'=>1
          },
          ##
          # AdverseEvent.suspectEntity.causalityAuthor.
          'author' => {
            'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole'],
            'type'=>'Reference',
            'path'=>'Causality.author',
            'min'=>0,
            'max'=>1
          },
          ##
          # ProbabilityScale | Bayesian | Checklist.
          'method' => {
            'local_name'=>'local_method'
            'valid_codes'=>{
              'http://terminology.hl7.org/CodeSystem/adverse-event-causality-method'=>[ 'ProbabilityScale', 'Bayesian', 'Checklist' ]
            },
            'type'=>'CodeableConcept',
            'path'=>'Causality.method',
            'min'=>0,
            'max'=>1,
            'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/adverse-event-causality-method'}
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
        # Assessment of if the entity caused the event.
        attr_accessor :assessment                     # 0-1 CodeableConcept
        ##
        # AdverseEvent.suspectEntity.causalityProductRelatedness.
        attr_accessor :productRelatedness             # 0-1 string
        ##
        # AdverseEvent.suspectEntity.causalityAuthor.
        attr_accessor :author                         # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole)
        ##
        # ProbabilityScale | Bayesian | Checklist.
        attr_accessor :local_method                   # 0-1 CodeableConcept
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
      # Refers to the specific entity that caused the adverse event
      # Identifies the actual instance of what caused the adverse event.  May be a substance, medication, medication administration, medication statement or a device.
      attr_accessor :instance                       # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Immunization|http://hl7.org/fhir/StructureDefinition/Procedure|http://hl7.org/fhir/StructureDefinition/Substance|http://hl7.org/fhir/StructureDefinition/Medication|http://hl7.org/fhir/StructureDefinition/MedicationAdministration|http://hl7.org/fhir/StructureDefinition/MedicationStatement|http://hl7.org/fhir/StructureDefinition/Device)
      ##
      # Information on the possible cause of the event.
      attr_accessor :causality                      # 0-* [ AdverseEvent::SuspectEntity::Causality ]
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
    # Business identifier for the event
    # Business identifiers assigned to this adverse event by the performer or other systems which remain constant as the resource is updated and propagates from server to server.
    # This is a business identifier, not a resource identifier (see [discussion](resource.html#identifiers)).  It is best practice for the identifier to only appear on a single resource instance, however business practices may occasionally dictate that multiple resource instances with the same identifier can exist - possibly even with different resource types.  For example, multiple Patient and a Person resource instance might share the same social insurance number.
    attr_accessor :identifier                     # 0-1 Identifier
    ##
    # actual | potential
    # Whether the event actually happened, or just had the potential to. Note that this is independent of whether anyone was affected or harmed or how severely.
    attr_accessor :actuality                      # 1-1 code
    ##
    # product-problem | product-quality | product-use-error | wrong-dose | incorrect-prescribing-information | wrong-technique | wrong-route-of-administration | wrong-rate | wrong-duration | wrong-time | expired-drug | medical-device-use-error | problem-different-manufacturer | unsafe-physical-environment
    # The overall type of event, intended for search and filtering purposes.
    attr_accessor :category                       # 0-* [ CodeableConcept ]
    ##
    # Type of the event itself in relation to the subject
    # This element defines the specific type of event that occurred or that was prevented from occurring.
    attr_accessor :event                          # 0-1 CodeableConcept
    ##
    # Subject impacted by event
    # This subject or group impacted by the event.
    # If AdverseEvent.resultingCondition differs among members of the group, then use Patient as the subject.
    attr_accessor :subject                        # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Group|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/RelatedPerson)
    ##
    # Encounter created as part of
    # The Encounter during which AdverseEvent was created or to which the creation of this record is tightly associated.
    # This will typically be the encounter the event occurred within, but some activities may be initiated prior to or after the official completion of an encounter but still be tied to the context of the encounter.  For example, if a medication administration was considered an adverse event because it resulted in a rash, then the encounter when the medication administration was given is the context.  If the patient reports the AdverseEvent during a second encounter, that second encounter is not the context.
    attr_accessor :encounter                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Encounter)
    ##
    # When the event occurred
    # The date (and perhaps time) when the adverse event occurred.
    attr_accessor :date                           # 0-1 dateTime
    ##
    # When the event was detected
    # Estimated or actual date the AdverseEvent began, in the opinion of the reporter.
    attr_accessor :detected                       # 0-1 dateTime
    ##
    # When the event was recorded
    # The date on which the existence of the AdverseEvent was first recorded.
    # The recordedDate represents the date when this particular AdverseEvent record was created in the system, not the date of the most recent update.  The date of the last record modification can be retrieved from the resource metadata.
    attr_accessor :recordedDate                   # 0-1 dateTime
    ##
    # Effect on the subject due to this event
    # Includes information about the reaction that occurred as a result of exposure to a substance (for example, a drug or a chemical).
    attr_accessor :resultingCondition             # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Condition) ]
    ##
    # Location where adverse event occurred
    # The information about where the adverse event occurred.
    attr_accessor :location                       # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Location)
    ##
    # Seriousness of the event
    # Assessment whether this event was of real importance.
    attr_accessor :seriousness                    # 0-1 CodeableConcept
    ##
    # mild | moderate | severe
    # Describes the severity of the adverse event, in relation to the subject. Contrast to AdverseEvent.seriousness - a severe rash might not be serious, but a mild heart problem is.
    attr_accessor :severity                       # 0-1 CodeableConcept
    ##
    # resolved | recovering | ongoing | resolvedWithSequelae | fatal | unknown
    # Describes the type of outcome from the adverse event.
    attr_accessor :outcome                        # 0-1 CodeableConcept
    ##
    # Who recorded the adverse event
    # Information on who recorded the adverse event.  May be the patient or a practitioner.
    attr_accessor :recorder                       # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/RelatedPerson)
    ##
    # Who  was involved in the adverse event or the potential adverse event
    # Parties that may or should contribute or have contributed information to the adverse event, which can consist of one or more activities.  Such information includes information leading to the decision to perform the activity and how to perform the activity (e.g. consultant), information that the activity itself seeks to reveal (e.g. informant of clinical history), or information about what activity was performed (e.g. informant witness).
    attr_accessor :contributor                    # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Device) ]
    ##
    # The suspected agent causing the adverse event
    # Describes the entity that is suspected to have caused the adverse event.
    attr_accessor :suspectEntity                  # 0-* [ AdverseEvent::SuspectEntity ]
    ##
    # AdverseEvent.subjectMedicalHistory.
    attr_accessor :subjectMedicalHistory          # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Condition|http://hl7.org/fhir/StructureDefinition/Observation|http://hl7.org/fhir/StructureDefinition/AllergyIntolerance|http://hl7.org/fhir/StructureDefinition/FamilyMemberHistory|http://hl7.org/fhir/StructureDefinition/Immunization|http://hl7.org/fhir/StructureDefinition/Procedure|http://hl7.org/fhir/StructureDefinition/Media|http://hl7.org/fhir/StructureDefinition/DocumentReference) ]
    ##
    # AdverseEvent.referenceDocument.
    attr_accessor :referenceDocument              # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/DocumentReference) ]
    ##
    # AdverseEvent.study.
    attr_accessor :study                          # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/ResearchStudy) ]

    def resourceType
      'AdverseEvent'
    end
  end
end
