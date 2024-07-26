module FHIR

  ##
  # A clinical condition, problem, diagnosis, or other event, situation, issue, or clinical concept that has risen to a level of concern.
  class Condition < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['abatement-age', 'abatement-date', 'abatement-string', 'asserter', 'body-site', 'category', 'clinical-status', 'code', 'encounter', 'evidence-detail', 'evidence', 'identifier', 'onset-age', 'onset-date', 'onset-info', 'patient', 'recorded-date', 'severity', 'stage', 'subject', 'verification-status']
    MULTIPLE_TYPES = {
      'onset[x]' => ['Age', 'dateTime', 'Period', 'Range', 'string'],
      'abatement[x]' => ['Age', 'dateTime', 'Period', 'Range', 'string']
    }
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'Condition.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'Condition.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'Condition.implicitRules',
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
        'path'=>'Condition.language',
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
        'path'=>'Condition.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'Condition.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Condition.extension',
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
        'path'=>'Condition.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # External Ids for this condition
      # Business identifiers assigned to this condition by the performer or other systems which remain constant as the resource is updated and propagates from server to server.
      # This is a business identifier, not a resource identifier (see [discussion](resource.html#identifiers)).  It is best practice for the identifier to only appear on a single resource instance, however business practices may occasionally dictate that multiple resource instances with the same identifier can exist - possibly even with different resource types.  For example, multiple Patient and a Person resource instance might share the same social insurance number.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'Condition.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # active | recurrence | relapse | inactive | remission | resolved
      # The clinical status of the condition.
      # The data type is CodeableConcept because clinicalStatus has some clinical judgment involved, such that there might need to be more specificity than the required FHIR value set allows. For example, a SNOMED coding might allow for additional specificity.
      'clinicalStatus' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/condition-clinical'=>[ 'active', 'recurrence', 'relapse', 'inactive', 'remission', 'resolved' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Condition.clinicalStatus',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/condition-clinical'}
      },
      ##
      # unconfirmed | provisional | differential | confirmed | refuted | entered-in-error
      # The verification status to support the clinical status of the condition.
      # verificationStatus is not required.  For example, when a patient has abdominal pain in the ED, there is not likely going to be a verification status.
      # The data type is CodeableConcept because verificationStatus has some clinical judgment involved, such that there might need to be more specificity than the required FHIR value set allows. For example, a SNOMED coding might allow for additional specificity.
      'verificationStatus' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/condition-ver-status'=>[ 'unconfirmed', 'provisional', 'differential', 'confirmed', 'refuted', 'entered-in-error' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Condition.verificationStatus',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/condition-ver-status'}
      },
      ##
      # problem-list-item | encounter-diagnosis
      # A category assigned to the condition.
      # The categorization is often highly contextual and may appear poorly differentiated or not very useful in other contexts.
      'category' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/condition-category'=>[ 'problem-list-item', 'encounter-diagnosis' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Condition.category',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/condition-category'}
      },
      ##
      # Subjective severity of condition
      # A subjective assessment of the severity of the condition as evaluated by the clinician.
      # Coding of the severity with a terminology is preferred, where possible.
      'severity' => {
        'type'=>'CodeableConcept',
        'path'=>'Condition.severity',
        'min'=>0,
        'max'=>1
      },
      ##
      # Identification of the condition, problem or diagnosis.
      'code' => {
        'type'=>'CodeableConcept',
        'path'=>'Condition.code',
        'min'=>0,
        'max'=>1
      },
      ##
      # Anatomical location, if relevant
      # The anatomical location where this condition manifests itself.
      # Only used if not implicit in code found in Condition.code. If the use case requires attributes from the BodySite resource (e.g. to identify and track separately) then use the standard extension [bodySite](extension-bodysite.html).  May be a summary code, or a reference to a very precise definition of the location, or both.
      'bodySite' => {
        'type'=>'CodeableConcept',
        'path'=>'Condition.bodySite',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Who has the condition?
      # Indicates the patient or group who the condition record is associated with.
      'subject' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Group'],
        'type'=>'Reference',
        'path'=>'Condition.subject',
        'min'=>1,
        'max'=>1
      },
      ##
      # Encounter created as part of
      # The Encounter during which this Condition was created or to which the creation of this record is tightly associated.
      # This will typically be the encounter the event occurred within, but some activities may be initiated prior to or after the official completion of an encounter but still be tied to the context of the encounter. This record indicates the encounter this particular record is associated with.  In the case of a "new" diagnosis reflecting ongoing/revised information about the condition, this might be distinct from the first encounter in which the underlying condition was first "known".
      'encounter' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Encounter'],
        'type'=>'Reference',
        'path'=>'Condition.encounter',
        'min'=>0,
        'max'=>1
      },
      ##
      # Estimated or actual date,  date-time, or age
      # Estimated or actual date or date-time  the condition began, in the opinion of the clinician.
      # Age is generally used when the patient reports an age at which the Condition began to occur.
      'onsetAge' => {
        'type'=>'Age',
        'path'=>'Condition.onset[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Estimated or actual date,  date-time, or age
      # Estimated or actual date or date-time  the condition began, in the opinion of the clinician.
      # Age is generally used when the patient reports an age at which the Condition began to occur.
      'onsetDateTime' => {
        'type'=>'DateTime',
        'path'=>'Condition.onset[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Estimated or actual date,  date-time, or age
      # Estimated or actual date or date-time  the condition began, in the opinion of the clinician.
      # Age is generally used when the patient reports an age at which the Condition began to occur.
      'onsetPeriod' => {
        'type'=>'Period',
        'path'=>'Condition.onset[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Estimated or actual date,  date-time, or age
      # Estimated or actual date or date-time  the condition began, in the opinion of the clinician.
      # Age is generally used when the patient reports an age at which the Condition began to occur.
      'onsetRange' => {
        'type'=>'Range',
        'path'=>'Condition.onset[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Estimated or actual date,  date-time, or age
      # Estimated or actual date or date-time  the condition began, in the opinion of the clinician.
      # Age is generally used when the patient reports an age at which the Condition began to occur.
      'onsetString' => {
        'type'=>'String',
        'path'=>'Condition.onset[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # When in resolution/remission
      # The date or estimated date that the condition resolved or went into remission. This is called "abatement" because of the many overloaded connotations associated with "remission" or "resolution" - Conditions are never really resolved, but they can abate.
      # There is no explicit distinction between resolution and remission because in many cases the distinction is not clear. Age is generally used when the patient reports an age at which the Condition abated.  If there is no abatement element, it is unknown whether the condition has resolved or entered remission; applications and users should generally assume that the condition is still valid.  When abatementString exists, it implies the condition is abated.
      'abatementAge' => {
        'type'=>'Age',
        'path'=>'Condition.abatement[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # When in resolution/remission
      # The date or estimated date that the condition resolved or went into remission. This is called "abatement" because of the many overloaded connotations associated with "remission" or "resolution" - Conditions are never really resolved, but they can abate.
      # There is no explicit distinction between resolution and remission because in many cases the distinction is not clear. Age is generally used when the patient reports an age at which the Condition abated.  If there is no abatement element, it is unknown whether the condition has resolved or entered remission; applications and users should generally assume that the condition is still valid.  When abatementString exists, it implies the condition is abated.
      'abatementDateTime' => {
        'type'=>'DateTime',
        'path'=>'Condition.abatement[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # When in resolution/remission
      # The date or estimated date that the condition resolved or went into remission. This is called "abatement" because of the many overloaded connotations associated with "remission" or "resolution" - Conditions are never really resolved, but they can abate.
      # There is no explicit distinction between resolution and remission because in many cases the distinction is not clear. Age is generally used when the patient reports an age at which the Condition abated.  If there is no abatement element, it is unknown whether the condition has resolved or entered remission; applications and users should generally assume that the condition is still valid.  When abatementString exists, it implies the condition is abated.
      'abatementPeriod' => {
        'type'=>'Period',
        'path'=>'Condition.abatement[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # When in resolution/remission
      # The date or estimated date that the condition resolved or went into remission. This is called "abatement" because of the many overloaded connotations associated with "remission" or "resolution" - Conditions are never really resolved, but they can abate.
      # There is no explicit distinction between resolution and remission because in many cases the distinction is not clear. Age is generally used when the patient reports an age at which the Condition abated.  If there is no abatement element, it is unknown whether the condition has resolved or entered remission; applications and users should generally assume that the condition is still valid.  When abatementString exists, it implies the condition is abated.
      'abatementRange' => {
        'type'=>'Range',
        'path'=>'Condition.abatement[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # When in resolution/remission
      # The date or estimated date that the condition resolved or went into remission. This is called "abatement" because of the many overloaded connotations associated with "remission" or "resolution" - Conditions are never really resolved, but they can abate.
      # There is no explicit distinction between resolution and remission because in many cases the distinction is not clear. Age is generally used when the patient reports an age at which the Condition abated.  If there is no abatement element, it is unknown whether the condition has resolved or entered remission; applications and users should generally assume that the condition is still valid.  When abatementString exists, it implies the condition is abated.
      'abatementString' => {
        'type'=>'String',
        'path'=>'Condition.abatement[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Date record was first recorded
      # The recordedDate represents when this particular Condition record was created in the system, which is often a system-generated date.
      'recordedDate' => {
        'type'=>'dateTime',
        'path'=>'Condition.recordedDate',
        'min'=>0,
        'max'=>1
      },
      ##
      # Who recorded the condition
      # Individual who recorded the record and takes responsibility for its content.
      'recorder' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson'],
        'type'=>'Reference',
        'path'=>'Condition.recorder',
        'min'=>0,
        'max'=>1
      },
      ##
      # Person who asserts this condition
      # Individual who is making the condition statement.
      'asserter' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson'],
        'type'=>'Reference',
        'path'=>'Condition.asserter',
        'min'=>0,
        'max'=>1
      },
      ##
      # Stage/grade, usually assessed formally
      # Clinical stage or grade of a condition. May include formal severity assessments.
      'stage' => {
        'type'=>'Condition::Stage',
        'path'=>'Condition.stage',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Supporting evidence / manifestations that are the basis of the Condition's verification status, such as evidence that confirmed or refuted the condition.
      # The evidence may be a simple list of coded symptoms/manifestations, or references to observations or formal assessments, or both.
      'evidence' => {
        'type'=>'Condition::Evidence',
        'path'=>'Condition.evidence',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional information about the Condition. This is a general notes/comments entry  for description of the Condition, its diagnosis and prognosis.
      'note' => {
        'type'=>'Annotation',
        'path'=>'Condition.note',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Stage/grade, usually assessed formally
    # Clinical stage or grade of a condition. May include formal severity assessments.
    class Stage < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Stage.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Stage.extension',
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
          'path'=>'Stage.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Simple summary (disease specific)
        # A simple summary of the stage such as "Stage 3". The determination of the stage is disease-specific.
        'summary' => {
          'type'=>'CodeableConcept',
          'path'=>'Stage.summary',
          'min'=>0,
          'max'=>1
        },
        ##
        # Formal record of assessment
        # Reference to a formal record of the evidence on which the staging assessment is based.
        'assessment' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ClinicalImpression', 'http://hl7.org/fhir/StructureDefinition/DiagnosticReport', 'http://hl7.org/fhir/StructureDefinition/Observation'],
          'type'=>'Reference',
          'path'=>'Stage.assessment',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Kind of staging
        # The kind of staging, such as pathological or clinical staging.
        'type' => {
          'type'=>'CodeableConcept',
          'path'=>'Stage.type',
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
      # Simple summary (disease specific)
      # A simple summary of the stage such as "Stage 3". The determination of the stage is disease-specific.
      attr_accessor :summary                        # 0-1 CodeableConcept
      ##
      # Formal record of assessment
      # Reference to a formal record of the evidence on which the staging assessment is based.
      attr_accessor :assessment                     # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/ClinicalImpression|http://hl7.org/fhir/StructureDefinition/DiagnosticReport|http://hl7.org/fhir/StructureDefinition/Observation) ]
      ##
      # Kind of staging
      # The kind of staging, such as pathological or clinical staging.
      attr_accessor :type                           # 0-1 CodeableConcept
    end

    ##
    # Supporting evidence / manifestations that are the basis of the Condition's verification status, such as evidence that confirmed or refuted the condition.
    # The evidence may be a simple list of coded symptoms/manifestations, or references to observations or formal assessments, or both.
    class Evidence < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Evidence.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Evidence.extension',
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
          'path'=>'Evidence.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Manifestation/symptom
        # A manifestation or symptom that led to the recording of this condition.
        'code' => {
          'type'=>'CodeableConcept',
          'path'=>'Evidence.code',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Supporting information found elsewhere
        # Links to other relevant information, including pathology reports.
        'detail' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
          'type'=>'Reference',
          'path'=>'Evidence.detail',
          'min'=>0,
          'max'=>Float::INFINITY
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
      # Manifestation/symptom
      # A manifestation or symptom that led to the recording of this condition.
      attr_accessor :code                           # 0-* [ CodeableConcept ]
      ##
      # Supporting information found elsewhere
      # Links to other relevant information, including pathology reports.
      attr_accessor :detail                         # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Resource) ]
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
    # External Ids for this condition
    # Business identifiers assigned to this condition by the performer or other systems which remain constant as the resource is updated and propagates from server to server.
    # This is a business identifier, not a resource identifier (see [discussion](resource.html#identifiers)).  It is best practice for the identifier to only appear on a single resource instance, however business practices may occasionally dictate that multiple resource instances with the same identifier can exist - possibly even with different resource types.  For example, multiple Patient and a Person resource instance might share the same social insurance number.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # active | recurrence | relapse | inactive | remission | resolved
    # The clinical status of the condition.
    # The data type is CodeableConcept because clinicalStatus has some clinical judgment involved, such that there might need to be more specificity than the required FHIR value set allows. For example, a SNOMED coding might allow for additional specificity.
    attr_accessor :clinicalStatus                 # 0-1 CodeableConcept
    ##
    # unconfirmed | provisional | differential | confirmed | refuted | entered-in-error
    # The verification status to support the clinical status of the condition.
    # verificationStatus is not required.  For example, when a patient has abdominal pain in the ED, there is not likely going to be a verification status.
    # The data type is CodeableConcept because verificationStatus has some clinical judgment involved, such that there might need to be more specificity than the required FHIR value set allows. For example, a SNOMED coding might allow for additional specificity.
    attr_accessor :verificationStatus             # 0-1 CodeableConcept
    ##
    # problem-list-item | encounter-diagnosis
    # A category assigned to the condition.
    # The categorization is often highly contextual and may appear poorly differentiated or not very useful in other contexts.
    attr_accessor :category                       # 0-* [ CodeableConcept ]
    ##
    # Subjective severity of condition
    # A subjective assessment of the severity of the condition as evaluated by the clinician.
    # Coding of the severity with a terminology is preferred, where possible.
    attr_accessor :severity                       # 0-1 CodeableConcept
    ##
    # Identification of the condition, problem or diagnosis.
    attr_accessor :code                           # 0-1 CodeableConcept
    ##
    # Anatomical location, if relevant
    # The anatomical location where this condition manifests itself.
    # Only used if not implicit in code found in Condition.code. If the use case requires attributes from the BodySite resource (e.g. to identify and track separately) then use the standard extension [bodySite](extension-bodysite.html).  May be a summary code, or a reference to a very precise definition of the location, or both.
    attr_accessor :bodySite                       # 0-* [ CodeableConcept ]
    ##
    # Who has the condition?
    # Indicates the patient or group who the condition record is associated with.
    attr_accessor :subject                        # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Group)
    ##
    # Encounter created as part of
    # The Encounter during which this Condition was created or to which the creation of this record is tightly associated.
    # This will typically be the encounter the event occurred within, but some activities may be initiated prior to or after the official completion of an encounter but still be tied to the context of the encounter. This record indicates the encounter this particular record is associated with.  In the case of a "new" diagnosis reflecting ongoing/revised information about the condition, this might be distinct from the first encounter in which the underlying condition was first "known".
    attr_accessor :encounter                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Encounter)
    ##
    # Estimated or actual date,  date-time, or age
    # Estimated or actual date or date-time  the condition began, in the opinion of the clinician.
    # Age is generally used when the patient reports an age at which the Condition began to occur.
    attr_accessor :onsetAge                       # 0-1 Age
    ##
    # Estimated or actual date,  date-time, or age
    # Estimated or actual date or date-time  the condition began, in the opinion of the clinician.
    # Age is generally used when the patient reports an age at which the Condition began to occur.
    attr_accessor :onsetDateTime                  # 0-1 DateTime
    ##
    # Estimated or actual date,  date-time, or age
    # Estimated or actual date or date-time  the condition began, in the opinion of the clinician.
    # Age is generally used when the patient reports an age at which the Condition began to occur.
    attr_accessor :onsetPeriod                    # 0-1 Period
    ##
    # Estimated or actual date,  date-time, or age
    # Estimated or actual date or date-time  the condition began, in the opinion of the clinician.
    # Age is generally used when the patient reports an age at which the Condition began to occur.
    attr_accessor :onsetRange                     # 0-1 Range
    ##
    # Estimated or actual date,  date-time, or age
    # Estimated or actual date or date-time  the condition began, in the opinion of the clinician.
    # Age is generally used when the patient reports an age at which the Condition began to occur.
    attr_accessor :onsetString                    # 0-1 String
    ##
    # When in resolution/remission
    # The date or estimated date that the condition resolved or went into remission. This is called "abatement" because of the many overloaded connotations associated with "remission" or "resolution" - Conditions are never really resolved, but they can abate.
    # There is no explicit distinction between resolution and remission because in many cases the distinction is not clear. Age is generally used when the patient reports an age at which the Condition abated.  If there is no abatement element, it is unknown whether the condition has resolved or entered remission; applications and users should generally assume that the condition is still valid.  When abatementString exists, it implies the condition is abated.
    attr_accessor :abatementAge                   # 0-1 Age
    ##
    # When in resolution/remission
    # The date or estimated date that the condition resolved or went into remission. This is called "abatement" because of the many overloaded connotations associated with "remission" or "resolution" - Conditions are never really resolved, but they can abate.
    # There is no explicit distinction between resolution and remission because in many cases the distinction is not clear. Age is generally used when the patient reports an age at which the Condition abated.  If there is no abatement element, it is unknown whether the condition has resolved or entered remission; applications and users should generally assume that the condition is still valid.  When abatementString exists, it implies the condition is abated.
    attr_accessor :abatementDateTime              # 0-1 DateTime
    ##
    # When in resolution/remission
    # The date or estimated date that the condition resolved or went into remission. This is called "abatement" because of the many overloaded connotations associated with "remission" or "resolution" - Conditions are never really resolved, but they can abate.
    # There is no explicit distinction between resolution and remission because in many cases the distinction is not clear. Age is generally used when the patient reports an age at which the Condition abated.  If there is no abatement element, it is unknown whether the condition has resolved or entered remission; applications and users should generally assume that the condition is still valid.  When abatementString exists, it implies the condition is abated.
    attr_accessor :abatementPeriod                # 0-1 Period
    ##
    # When in resolution/remission
    # The date or estimated date that the condition resolved or went into remission. This is called "abatement" because of the many overloaded connotations associated with "remission" or "resolution" - Conditions are never really resolved, but they can abate.
    # There is no explicit distinction between resolution and remission because in many cases the distinction is not clear. Age is generally used when the patient reports an age at which the Condition abated.  If there is no abatement element, it is unknown whether the condition has resolved or entered remission; applications and users should generally assume that the condition is still valid.  When abatementString exists, it implies the condition is abated.
    attr_accessor :abatementRange                 # 0-1 Range
    ##
    # When in resolution/remission
    # The date or estimated date that the condition resolved or went into remission. This is called "abatement" because of the many overloaded connotations associated with "remission" or "resolution" - Conditions are never really resolved, but they can abate.
    # There is no explicit distinction between resolution and remission because in many cases the distinction is not clear. Age is generally used when the patient reports an age at which the Condition abated.  If there is no abatement element, it is unknown whether the condition has resolved or entered remission; applications and users should generally assume that the condition is still valid.  When abatementString exists, it implies the condition is abated.
    attr_accessor :abatementString                # 0-1 String
    ##
    # Date record was first recorded
    # The recordedDate represents when this particular Condition record was created in the system, which is often a system-generated date.
    attr_accessor :recordedDate                   # 0-1 dateTime
    ##
    # Who recorded the condition
    # Individual who recorded the record and takes responsibility for its content.
    attr_accessor :recorder                       # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/RelatedPerson)
    ##
    # Person who asserts this condition
    # Individual who is making the condition statement.
    attr_accessor :asserter                       # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/RelatedPerson)
    ##
    # Stage/grade, usually assessed formally
    # Clinical stage or grade of a condition. May include formal severity assessments.
    attr_accessor :stage                          # 0-* [ Condition::Stage ]
    ##
    # Supporting evidence / manifestations that are the basis of the Condition's verification status, such as evidence that confirmed or refuted the condition.
    # The evidence may be a simple list of coded symptoms/manifestations, or references to observations or formal assessments, or both.
    attr_accessor :evidence                       # 0-* [ Condition::Evidence ]
    ##
    # Additional information about the Condition. This is a general notes/comments entry  for description of the Condition, its diagnosis and prognosis.
    attr_accessor :note                           # 0-* [ Annotation ]

    def resourceType
      'Condition'
    end
  end
end
