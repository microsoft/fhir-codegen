module FHIR

  ##
  # A record of a clinical assessment performed to determine what problem(s) may affect the patient and before planning the treatments or management strategies that are best to manage a patient's condition. Assessments are often 1:1 with a clinical consultation / encounter,  but this varies greatly depending on the clinical workflow. This resource is called "ClinicalImpression" rather than "ClinicalAssessment" to avoid confusion with the recording of assessment tools such as Apgar score.
  class ClinicalImpression < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['assessor', 'date', 'encounter', 'finding-code', 'finding-ref', 'identifier', 'investigation', 'patient', 'previous', 'problem', 'status', 'subject', 'supporting-info']
    MULTIPLE_TYPES = {
      'effective[x]' => ['dateTime', 'Period']
    }
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'ClinicalImpression.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'ClinicalImpression.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'ClinicalImpression.implicitRules',
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
        'path'=>'ClinicalImpression.language',
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
        'path'=>'ClinicalImpression.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'ClinicalImpression.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'ClinicalImpression.extension',
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
        'path'=>'ClinicalImpression.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Business identifiers assigned to this clinical impression by the performer or other systems which remain constant as the resource is updated and propagates from server to server.
      # This is a business identifier, not a resource identifier (see [discussion](resource.html#identifiers)).  It is best practice for the identifier to only appear on a single resource instance, however business practices may occasionally dictate that multiple resource instances with the same identifier can exist - possibly even with different resource types.  For example, multiple Patient and a Person resource instance might share the same social insurance number.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'ClinicalImpression.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # in-progress | completed | entered-in-error
      # Identifies the workflow status of the assessment.
      # This element is labeled as a modifier because the status contains the code entered-in-error that marks the clinical impression as not currently valid.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/event-status'=>[ 'in-progress', 'completed', 'entered-in-error' ]
        },
        'type'=>'code',
        'path'=>'ClinicalImpression.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/clinicalimpression-status'}
      },
      ##
      # Reason for current status
      # Captures the reason for the current state of the ClinicalImpression.
      # This is generally only used for "exception" statuses such as "not-done", "suspended" or "cancelled".
      # 
      # [distinct reason codes for different statuses can be enforced using invariants if they are universal bindings].
      'statusReason' => {
        'type'=>'CodeableConcept',
        'path'=>'ClinicalImpression.statusReason',
        'min'=>0,
        'max'=>1
      },
      ##
      # Kind of assessment performed
      # Categorizes the type of clinical assessment performed.
      # This is present as a place-holder only and may be removed based on feedback/work group opinion.
      'code' => {
        'type'=>'CodeableConcept',
        'path'=>'ClinicalImpression.code',
        'min'=>0,
        'max'=>1
      },
      ##
      # Why/how the assessment was performed
      # A summary of the context and/or cause of the assessment - why / where it was performed, and what patient events/status prompted it.
      'description' => {
        'type'=>'string',
        'path'=>'ClinicalImpression.description',
        'min'=>0,
        'max'=>1
      },
      ##
      # Patient or group assessed
      # The patient or group of individuals assessed as part of this record.
      'subject' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Group'],
        'type'=>'Reference',
        'path'=>'ClinicalImpression.subject',
        'min'=>1,
        'max'=>1
      },
      ##
      # Encounter created as part of
      # The Encounter during which this ClinicalImpression was created or to which the creation of this record is tightly associated.
      # This will typically be the encounter the event occurred within, but some activities may be initiated prior to or after the official completion of an encounter but still be tied to the context of the encounter.
      'encounter' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Encounter'],
        'type'=>'Reference',
        'path'=>'ClinicalImpression.encounter',
        'min'=>0,
        'max'=>1
      },
      ##
      # Time of assessment
      # The point in time or period over which the subject was assessed.
      # This SHOULD be accurate to at least the minute, though some assessments only have a known date.
      'effectiveDateTime' => {
        'type'=>'DateTime',
        'path'=>'ClinicalImpression.effective[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Time of assessment
      # The point in time or period over which the subject was assessed.
      # This SHOULD be accurate to at least the minute, though some assessments only have a known date.
      'effectivePeriod' => {
        'type'=>'Period',
        'path'=>'ClinicalImpression.effective[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the assessment was documented
      # Indicates when the documentation of the assessment was complete.
      'date' => {
        'type'=>'dateTime',
        'path'=>'ClinicalImpression.date',
        'min'=>0,
        'max'=>1
      },
      ##
      # The clinician performing the assessment.
      'assessor' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole'],
        'type'=>'Reference',
        'path'=>'ClinicalImpression.assessor',
        'min'=>0,
        'max'=>1
      },
      ##
      # Reference to last assessment
      # A reference to the last assessment that was conducted on this patient. Assessments are often/usually ongoing in nature; a care provider (practitioner or team) will make new assessments on an ongoing basis as new data arises or the patient's conditions changes.
      # It is always likely that multiple previous assessments exist for a patient. The point of quoting a previous assessment is that this assessment is relative to it (see resolved).
      'previous' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ClinicalImpression'],
        'type'=>'Reference',
        'path'=>'ClinicalImpression.previous',
        'min'=>0,
        'max'=>1
      },
      ##
      # Relevant impressions of patient state
      # A list of the relevant problems/conditions for a patient.
      # e.g. The patient is a pregnant, has congestive heart failure, has an ‎Adenocarcinoma, and is allergic to penicillin.
      'problem' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Condition', 'http://hl7.org/fhir/StructureDefinition/AllergyIntolerance'],
        'type'=>'Reference',
        'path'=>'ClinicalImpression.problem',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # One or more sets of investigations (signs, symptoms, etc.). The actual grouping of investigations varies greatly depending on the type and context of the assessment. These investigations may include data generated during the assessment process, or data previously generated and recorded that is pertinent to the outcomes.
      'investigation' => {
        'type'=>'ClinicalImpression::Investigation',
        'path'=>'ClinicalImpression.investigation',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Clinical Protocol followed
      # Reference to a specific published clinical protocol that was followed during this assessment, and/or that provides evidence in support of the diagnosis.
      'protocol' => {
        'type'=>'uri',
        'path'=>'ClinicalImpression.protocol',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Summary of the assessment
      # A text summary of the investigations and the diagnosis.
      'summary' => {
        'type'=>'string',
        'path'=>'ClinicalImpression.summary',
        'min'=>0,
        'max'=>1
      },
      ##
      # Possible or likely findings and diagnoses
      # Specific findings or diagnoses that were considered likely or relevant to ongoing treatment.
      'finding' => {
        'type'=>'ClinicalImpression::Finding',
        'path'=>'ClinicalImpression.finding',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Estimate of likely outcome.
      'prognosisCodeableConcept' => {
        'type'=>'CodeableConcept',
        'path'=>'ClinicalImpression.prognosisCodeableConcept',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # RiskAssessment expressing likely outcome.
      'prognosisReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/RiskAssessment'],
        'type'=>'Reference',
        'path'=>'ClinicalImpression.prognosisReference',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Information supporting the clinical impression.
      'supportingInfo' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'Reference',
        'path'=>'ClinicalImpression.supportingInfo',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Comments made about the ClinicalImpression
      # Commentary about the impression, typically recorded after the impression itself was made, though supplemental notes by the original author could also appear.
      # Don't use this element for content that should more properly appear as one of the specific elements of the impression.
      'note' => {
        'type'=>'Annotation',
        'path'=>'ClinicalImpression.note',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # One or more sets of investigations (signs, symptoms, etc.). The actual grouping of investigations varies greatly depending on the type and context of the assessment. These investigations may include data generated during the assessment process, or data previously generated and recorded that is pertinent to the outcomes.
    class Investigation < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Investigation.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Investigation.extension',
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
          'path'=>'Investigation.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # A name/code for the set
        # A name/code for the group ("set") of investigations. Typically, this will be something like "signs", "symptoms", "clinical", "diagnostic", but the list is not constrained, and others such groups such as (exposure|family|travel|nutritional) history may be used.
        'code' => {
          'type'=>'CodeableConcept',
          'path'=>'Investigation.code',
          'min'=>1,
          'max'=>1
        },
        ##
        # Record of a specific investigation
        # A record of a specific investigation that was undertaken.
        # Most investigations are observations of one kind or another but some other specific types of data collection resources can also be used.
        'item' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Observation', 'http://hl7.org/fhir/StructureDefinition/QuestionnaireResponse', 'http://hl7.org/fhir/StructureDefinition/FamilyMemberHistory', 'http://hl7.org/fhir/StructureDefinition/DiagnosticReport', 'http://hl7.org/fhir/StructureDefinition/RiskAssessment', 'http://hl7.org/fhir/StructureDefinition/ImagingStudy', 'http://hl7.org/fhir/StructureDefinition/Media'],
          'type'=>'Reference',
          'path'=>'Investigation.item',
          'min'=>0,
          'max'=>Float::INFINITY
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
      # A name/code for the set
      # A name/code for the group ("set") of investigations. Typically, this will be something like "signs", "symptoms", "clinical", "diagnostic", but the list is not constrained, and others such groups such as (exposure|family|travel|nutritional) history may be used.
      attr_accessor :code                           # 1-1 CodeableConcept
      ##
      # Record of a specific investigation
      # A record of a specific investigation that was undertaken.
      # Most investigations are observations of one kind or another but some other specific types of data collection resources can also be used.
      attr_accessor :item                           # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Observation|http://hl7.org/fhir/StructureDefinition/QuestionnaireResponse|http://hl7.org/fhir/StructureDefinition/FamilyMemberHistory|http://hl7.org/fhir/StructureDefinition/DiagnosticReport|http://hl7.org/fhir/StructureDefinition/RiskAssessment|http://hl7.org/fhir/StructureDefinition/ImagingStudy|http://hl7.org/fhir/StructureDefinition/Media) ]
    end

    ##
    # Possible or likely findings and diagnoses
    # Specific findings or diagnoses that were considered likely or relevant to ongoing treatment.
    class Finding < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Finding.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Finding.extension',
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
          'path'=>'Finding.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # What was found
        # Specific text or code for finding or diagnosis, which may include ruled-out or resolved conditions.
        'itemCodeableConcept' => {
          'type'=>'CodeableConcept',
          'path'=>'Finding.itemCodeableConcept',
          'min'=>0,
          'max'=>1
        },
        ##
        # What was found
        # Specific reference for finding or diagnosis, which may include ruled-out or resolved conditions.
        'itemReference' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Condition', 'http://hl7.org/fhir/StructureDefinition/Observation', 'http://hl7.org/fhir/StructureDefinition/Media'],
          'type'=>'Reference',
          'path'=>'Finding.itemReference',
          'min'=>0,
          'max'=>1
        },
        ##
        # Which investigations support finding or diagnosis.
        'basis' => {
          'type'=>'string',
          'path'=>'Finding.basis',
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
      # What was found
      # Specific text or code for finding or diagnosis, which may include ruled-out or resolved conditions.
      attr_accessor :itemCodeableConcept            # 0-1 CodeableConcept
      ##
      # What was found
      # Specific reference for finding or diagnosis, which may include ruled-out or resolved conditions.
      attr_accessor :itemReference                  # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Condition|http://hl7.org/fhir/StructureDefinition/Observation|http://hl7.org/fhir/StructureDefinition/Media)
      ##
      # Which investigations support finding or diagnosis.
      attr_accessor :basis                          # 0-1 string
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
    # Business identifiers assigned to this clinical impression by the performer or other systems which remain constant as the resource is updated and propagates from server to server.
    # This is a business identifier, not a resource identifier (see [discussion](resource.html#identifiers)).  It is best practice for the identifier to only appear on a single resource instance, however business practices may occasionally dictate that multiple resource instances with the same identifier can exist - possibly even with different resource types.  For example, multiple Patient and a Person resource instance might share the same social insurance number.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # in-progress | completed | entered-in-error
    # Identifies the workflow status of the assessment.
    # This element is labeled as a modifier because the status contains the code entered-in-error that marks the clinical impression as not currently valid.
    attr_accessor :status                         # 1-1 code
    ##
    # Reason for current status
    # Captures the reason for the current state of the ClinicalImpression.
    # This is generally only used for "exception" statuses such as "not-done", "suspended" or "cancelled".
    # 
    # [distinct reason codes for different statuses can be enforced using invariants if they are universal bindings].
    attr_accessor :statusReason                   # 0-1 CodeableConcept
    ##
    # Kind of assessment performed
    # Categorizes the type of clinical assessment performed.
    # This is present as a place-holder only and may be removed based on feedback/work group opinion.
    attr_accessor :code                           # 0-1 CodeableConcept
    ##
    # Why/how the assessment was performed
    # A summary of the context and/or cause of the assessment - why / where it was performed, and what patient events/status prompted it.
    attr_accessor :description                    # 0-1 string
    ##
    # Patient or group assessed
    # The patient or group of individuals assessed as part of this record.
    attr_accessor :subject                        # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Group)
    ##
    # Encounter created as part of
    # The Encounter during which this ClinicalImpression was created or to which the creation of this record is tightly associated.
    # This will typically be the encounter the event occurred within, but some activities may be initiated prior to or after the official completion of an encounter but still be tied to the context of the encounter.
    attr_accessor :encounter                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Encounter)
    ##
    # Time of assessment
    # The point in time or period over which the subject was assessed.
    # This SHOULD be accurate to at least the minute, though some assessments only have a known date.
    attr_accessor :effectiveDateTime              # 0-1 DateTime
    ##
    # Time of assessment
    # The point in time or period over which the subject was assessed.
    # This SHOULD be accurate to at least the minute, though some assessments only have a known date.
    attr_accessor :effectivePeriod                # 0-1 Period
    ##
    # When the assessment was documented
    # Indicates when the documentation of the assessment was complete.
    attr_accessor :date                           # 0-1 dateTime
    ##
    # The clinician performing the assessment.
    attr_accessor :assessor                       # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole)
    ##
    # Reference to last assessment
    # A reference to the last assessment that was conducted on this patient. Assessments are often/usually ongoing in nature; a care provider (practitioner or team) will make new assessments on an ongoing basis as new data arises or the patient's conditions changes.
    # It is always likely that multiple previous assessments exist for a patient. The point of quoting a previous assessment is that this assessment is relative to it (see resolved).
    attr_accessor :previous                       # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/ClinicalImpression)
    ##
    # Relevant impressions of patient state
    # A list of the relevant problems/conditions for a patient.
    # e.g. The patient is a pregnant, has congestive heart failure, has an ‎Adenocarcinoma, and is allergic to penicillin.
    attr_accessor :problem                        # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Condition|http://hl7.org/fhir/StructureDefinition/AllergyIntolerance) ]
    ##
    # One or more sets of investigations (signs, symptoms, etc.). The actual grouping of investigations varies greatly depending on the type and context of the assessment. These investigations may include data generated during the assessment process, or data previously generated and recorded that is pertinent to the outcomes.
    attr_accessor :investigation                  # 0-* [ ClinicalImpression::Investigation ]
    ##
    # Clinical Protocol followed
    # Reference to a specific published clinical protocol that was followed during this assessment, and/or that provides evidence in support of the diagnosis.
    attr_accessor :protocol                       # 0-* [ uri ]
    ##
    # Summary of the assessment
    # A text summary of the investigations and the diagnosis.
    attr_accessor :summary                        # 0-1 string
    ##
    # Possible or likely findings and diagnoses
    # Specific findings or diagnoses that were considered likely or relevant to ongoing treatment.
    attr_accessor :finding                        # 0-* [ ClinicalImpression::Finding ]
    ##
    # Estimate of likely outcome.
    attr_accessor :prognosisCodeableConcept       # 0-* [ CodeableConcept ]
    ##
    # RiskAssessment expressing likely outcome.
    attr_accessor :prognosisReference             # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/RiskAssessment) ]
    ##
    # Information supporting the clinical impression.
    attr_accessor :supportingInfo                 # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Resource) ]
    ##
    # Comments made about the ClinicalImpression
    # Commentary about the impression, typically recorded after the impression itself was made, though supplemental notes by the original author could also appear.
    # Don't use this element for content that should more properly appear as one of the specific elements of the impression.
    attr_accessor :note                           # 0-* [ Annotation ]

    def resourceType
      'ClinicalImpression'
    end
  end
end
