module FHIR

  ##
  # Significant health conditions for a person related to the patient relevant in the context of care for the patient.
  class FamilyMemberHistory < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['code', 'date', 'identifier', 'instantiates-canonical', 'instantiates-uri', 'patient', 'relationship', 'sex', 'status']
    MULTIPLE_TYPES = {
      'born[x]' => ['date', 'Period', 'string'],
      'age[x]' => ['Age', 'Range', 'string'],
      'deceased[x]' => ['Age', 'boolean', 'date', 'Range', 'string']
    }
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'FamilyMemberHistory.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'FamilyMemberHistory.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'FamilyMemberHistory.implicitRules',
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
        'path'=>'FamilyMemberHistory.language',
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
        'path'=>'FamilyMemberHistory.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'FamilyMemberHistory.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'FamilyMemberHistory.extension',
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
        'path'=>'FamilyMemberHistory.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # External Id(s) for this record
      # Business identifiers assigned to this family member history by the performer or other systems which remain constant as the resource is updated and propagates from server to server.
      # This is a business identifier, not a resource identifier (see [discussion](resource.html#identifiers)).  It is best practice for the identifier to only appear on a single resource instance, however business practices may occasionally dictate that multiple resource instances with the same identifier can exist - possibly even with different resource types.  For example, multiple Patient and a Person resource instance might share the same social insurance number.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'FamilyMemberHistory.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Instantiates FHIR protocol or definition
      # The URL pointing to a FHIR-defined protocol, guideline, orderset or other definition that is adhered to in whole or in part by this FamilyMemberHistory.
      'instantiatesCanonical' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/PlanDefinition', 'http://hl7.org/fhir/StructureDefinition/Questionnaire', 'http://hl7.org/fhir/StructureDefinition/ActivityDefinition', 'http://hl7.org/fhir/StructureDefinition/Measure', 'http://hl7.org/fhir/StructureDefinition/OperationDefinition'],
        'type'=>'canonical',
        'path'=>'FamilyMemberHistory.instantiatesCanonical',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Instantiates external protocol or definition
      # The URL pointing to an externally maintained protocol, guideline, orderset or other definition that is adhered to in whole or in part by this FamilyMemberHistory.
      # This might be an HTML page, PDF, etc. or could just be a non-resolvable URI identifier.
      'instantiatesUri' => {
        'type'=>'uri',
        'path'=>'FamilyMemberHistory.instantiatesUri',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # partial | completed | entered-in-error | health-unknown
      # A code specifying the status of the record of the family history of a specific family member.
      # This element is labeled as a modifier because the status contains codes that mark the resource as not currently valid.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/history-status'=>[ 'partial', 'completed', 'entered-in-error', 'health-unknown' ]
        },
        'type'=>'code',
        'path'=>'FamilyMemberHistory.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/history-status'}
      },
      ##
      # subject-unknown | withheld | unable-to-obtain | deferred
      # Describes why the family member's history is not available.
      'dataAbsentReason' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/history-absent-reason'=>[ 'subject-unknown', 'withheld', 'unable-to-obtain', 'deferred' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'FamilyMemberHistory.dataAbsentReason',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/history-absent-reason'}
      },
      ##
      # Patient history is about
      # The person who this history concerns.
      'patient' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient'],
        'type'=>'Reference',
        'path'=>'FamilyMemberHistory.patient',
        'min'=>1,
        'max'=>1
      },
      ##
      # When history was recorded or last updated
      # The date (and possibly time) when the family member history was recorded or last updated.
      # This should be captured even if the same as the date on the List aggregating the full family history.
      'date' => {
        'type'=>'dateTime',
        'path'=>'FamilyMemberHistory.date',
        'min'=>0,
        'max'=>1
      },
      ##
      # The family member described
      # This will either be a name or a description; e.g. "Aunt Susan", "my cousin with the red hair".
      'name' => {
        'type'=>'string',
        'path'=>'FamilyMemberHistory.name',
        'min'=>0,
        'max'=>1
      },
      ##
      # Relationship to the subject
      # The type of relationship this person has to the patient (father, mother, brother etc.).
      'relationship' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/v3-RoleCode'=>[ 'FAMMEMB', 'CHILD', 'CHLDADOPT', 'DAUADOPT', 'SONADOPT', 'CHLDFOST', 'DAUFOST', 'SONFOST', 'DAUC', 'DAU', 'STPDAU', 'NCHILD', 'SON', 'SONC', 'STPSON', 'STPCHLD', 'EXT', 'AUNT', 'MAUNT', 'PAUNT', 'COUSN', 'MCOUSN', 'PCOUSN', 'GGRPRN', 'GGRFTH', 'MGGRFTH', 'PGGRFTH', 'GGRMTH', 'MGGRMTH', 'PGGRMTH', 'MGGRPRN', 'PGGRPRN', 'GRNDCHILD', 'GRNDDAU', 'GRNDSON', 'GRPRN', 'GRFTH', 'MGRFTH', 'PGRFTH', 'GRMTH', 'MGRMTH', 'PGRMTH', 'MGRPRN', 'PGRPRN', 'INLAW', 'CHLDINLAW', 'DAUINLAW', 'SONINLAW', 'PRNINLAW', 'FTHINLAW', 'MTHINLAW', 'SIBINLAW', 'BROINLAW', 'SISINLAW', 'NIENEPH', 'NEPHEW', 'NIECE', 'UNCLE', 'MUNCLE', 'PUNCLE', 'PRN', 'ADOPTP', 'ADOPTF', 'ADOPTM', 'FTH', 'FTHFOST', 'NFTH', 'NFTHF', 'STPFTH', 'MTH', 'GESTM', 'MTHFOST', 'NMTH', 'NMTHF', 'STPMTH', 'NPRN', 'PRNFOST', 'STPPRN', 'SIB', 'BRO', 'HBRO', 'NBRO', 'TWINBRO', 'FTWINBRO', 'ITWINBRO', 'STPBRO', 'HSIB', 'HSIS', 'NSIB', 'NSIS', 'TWINSIS', 'FTWINSIS', 'ITWINSIS', 'TWIN', 'FTWIN', 'ITWIN', 'SIS', 'STPSIS', 'STPSIB', 'SIGOTHR', 'DOMPART', 'FMRSPS', 'SPS', 'HUSB', 'WIFE' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'FamilyMemberHistory.relationship',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://terminology.hl7.org/ValueSet/v3-FamilyMember'}
      },
      ##
      # male | female | other | unknown
      # The birth sex of the family member.
      # This element should ideally reflect whether the individual is genetically male or female.  However, as reported information based on the knowledge of the patient or reporting friend/relative, there may be situations where the reported sex might not be totally accurate.  E.g. 'Aunt Sue' might be XY rather than XX.  Questions soliciting this information should be phrased to encourage capture of genetic sex where known.  However, systems performing analysis should also allow for the possibility of imprecision with this element.
      'sex' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/administrative-gender'=>[ 'male', 'female', 'other', 'unknown' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'FamilyMemberHistory.sex',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/administrative-gender'}
      },
      ##
      # (approximate) date of birth
      # The actual or approximate date of birth of the relative.
      'bornDate' => {
        'type'=>'Date',
        'path'=>'FamilyMemberHistory.born[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # (approximate) date of birth
      # The actual or approximate date of birth of the relative.
      'bornPeriod' => {
        'type'=>'Period',
        'path'=>'FamilyMemberHistory.born[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # (approximate) date of birth
      # The actual or approximate date of birth of the relative.
      'bornString' => {
        'type'=>'String',
        'path'=>'FamilyMemberHistory.born[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # (approximate) age
      # The age of the relative at the time the family member history is recorded.
      # use estimatedAge to indicate whether the age is actual or not.
      'ageAge' => {
        'type'=>'Age',
        'path'=>'FamilyMemberHistory.age[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # (approximate) age
      # The age of the relative at the time the family member history is recorded.
      # use estimatedAge to indicate whether the age is actual or not.
      'ageRange' => {
        'type'=>'Range',
        'path'=>'FamilyMemberHistory.age[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # (approximate) age
      # The age of the relative at the time the family member history is recorded.
      # use estimatedAge to indicate whether the age is actual or not.
      'ageString' => {
        'type'=>'String',
        'path'=>'FamilyMemberHistory.age[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Age is estimated?
      # If true, indicates that the age value specified is an estimated value.
      # This element is labeled as a modifier because the fact that age is estimated can/should change the results of any algorithm that calculates based on the specified age.
      'estimatedAge' => {
        'type'=>'boolean',
        'path'=>'FamilyMemberHistory.estimatedAge',
        'min'=>0,
        'max'=>1
      },
      ##
      # Dead? How old/when?
      # Deceased flag or the actual or approximate age of the relative at the time of death for the family member history record.
      'deceasedAge' => {
        'type'=>'Age',
        'path'=>'FamilyMemberHistory.deceased[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Dead? How old/when?
      # Deceased flag or the actual or approximate age of the relative at the time of death for the family member history record.
      'deceasedBoolean' => {
        'type'=>'Boolean',
        'path'=>'FamilyMemberHistory.deceased[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Dead? How old/when?
      # Deceased flag or the actual or approximate age of the relative at the time of death for the family member history record.
      'deceasedDate' => {
        'type'=>'Date',
        'path'=>'FamilyMemberHistory.deceased[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Dead? How old/when?
      # Deceased flag or the actual or approximate age of the relative at the time of death for the family member history record.
      'deceasedRange' => {
        'type'=>'Range',
        'path'=>'FamilyMemberHistory.deceased[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Dead? How old/when?
      # Deceased flag or the actual or approximate age of the relative at the time of death for the family member history record.
      'deceasedString' => {
        'type'=>'String',
        'path'=>'FamilyMemberHistory.deceased[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Why was family member history performed?
      # Describes why the family member history occurred in coded or textual form.
      # Textual reasons can be captured using reasonCode.text.
      'reasonCode' => {
        'type'=>'CodeableConcept',
        'path'=>'FamilyMemberHistory.reasonCode',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Why was family member history performed?
      # Indicates a Condition, Observation, AllergyIntolerance, or QuestionnaireResponse that justifies this family member history event.
      'reasonReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Condition', 'http://hl7.org/fhir/StructureDefinition/Observation', 'http://hl7.org/fhir/StructureDefinition/AllergyIntolerance', 'http://hl7.org/fhir/StructureDefinition/QuestionnaireResponse', 'http://hl7.org/fhir/StructureDefinition/DiagnosticReport', 'http://hl7.org/fhir/StructureDefinition/DocumentReference'],
        'type'=>'Reference',
        'path'=>'FamilyMemberHistory.reasonReference',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # General note about related person
      # This property allows a non condition-specific note to the made about the related person. Ideally, the note would be in the condition property, but this is not always possible.
      'note' => {
        'type'=>'Annotation',
        'path'=>'FamilyMemberHistory.note',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Condition that the related person had
      # The significant Conditions (or condition) that the family member had. This is a repeating section to allow a system to represent more than one condition per resource, though there is nothing stopping multiple resources - one per condition.
      'condition' => {
        'type'=>'FamilyMemberHistory::Condition',
        'path'=>'FamilyMemberHistory.condition',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Condition that the related person had
    # The significant Conditions (or condition) that the family member had. This is a repeating section to allow a system to represent more than one condition per resource, though there is nothing stopping multiple resources - one per condition.
    class Condition < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'onset[x]' => ['Age', 'Period', 'Range', 'string']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Condition.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Condition.extension',
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
          'path'=>'Condition.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Condition suffered by relation
        # The actual condition specified. Could be a coded condition (like MI or Diabetes) or a less specific string like 'cancer' depending on how much is known about the condition and the capabilities of the creating system.
        'code' => {
          'type'=>'CodeableConcept',
          'path'=>'Condition.code',
          'min'=>1,
          'max'=>1
        },
        ##
        # deceased | permanent disability | etc.
        # Indicates what happened following the condition.  If the condition resulted in death, deceased date is captured on the relation.
        'outcome' => {
          'type'=>'CodeableConcept',
          'path'=>'Condition.outcome',
          'min'=>0,
          'max'=>1
        },
        ##
        # Whether the condition contributed to the cause of death
        # This condition contributed to the cause of death of the related person. If contributedToDeath is not populated, then it is unknown.
        'contributedToDeath' => {
          'type'=>'boolean',
          'path'=>'Condition.contributedToDeath',
          'min'=>0,
          'max'=>1
        },
        ##
        # When condition first manifested
        # Either the age of onset, range of approximate age or descriptive string can be recorded.  For conditions with multiple occurrences, this describes the first known occurrence.
        'onsetAge' => {
          'type'=>'Age',
          'path'=>'Condition.onset[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # When condition first manifested
        # Either the age of onset, range of approximate age or descriptive string can be recorded.  For conditions with multiple occurrences, this describes the first known occurrence.
        'onsetPeriod' => {
          'type'=>'Period',
          'path'=>'Condition.onset[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # When condition first manifested
        # Either the age of onset, range of approximate age or descriptive string can be recorded.  For conditions with multiple occurrences, this describes the first known occurrence.
        'onsetRange' => {
          'type'=>'Range',
          'path'=>'Condition.onset[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # When condition first manifested
        # Either the age of onset, range of approximate age or descriptive string can be recorded.  For conditions with multiple occurrences, this describes the first known occurrence.
        'onsetString' => {
          'type'=>'String',
          'path'=>'Condition.onset[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Extra information about condition
        # An area where general notes can be placed about this specific condition.
        'note' => {
          'type'=>'Annotation',
          'path'=>'Condition.note',
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
      # Condition suffered by relation
      # The actual condition specified. Could be a coded condition (like MI or Diabetes) or a less specific string like 'cancer' depending on how much is known about the condition and the capabilities of the creating system.
      attr_accessor :code                           # 1-1 CodeableConcept
      ##
      # deceased | permanent disability | etc.
      # Indicates what happened following the condition.  If the condition resulted in death, deceased date is captured on the relation.
      attr_accessor :outcome                        # 0-1 CodeableConcept
      ##
      # Whether the condition contributed to the cause of death
      # This condition contributed to the cause of death of the related person. If contributedToDeath is not populated, then it is unknown.
      attr_accessor :contributedToDeath             # 0-1 boolean
      ##
      # When condition first manifested
      # Either the age of onset, range of approximate age or descriptive string can be recorded.  For conditions with multiple occurrences, this describes the first known occurrence.
      attr_accessor :onsetAge                       # 0-1 Age
      ##
      # When condition first manifested
      # Either the age of onset, range of approximate age or descriptive string can be recorded.  For conditions with multiple occurrences, this describes the first known occurrence.
      attr_accessor :onsetPeriod                    # 0-1 Period
      ##
      # When condition first manifested
      # Either the age of onset, range of approximate age or descriptive string can be recorded.  For conditions with multiple occurrences, this describes the first known occurrence.
      attr_accessor :onsetRange                     # 0-1 Range
      ##
      # When condition first manifested
      # Either the age of onset, range of approximate age or descriptive string can be recorded.  For conditions with multiple occurrences, this describes the first known occurrence.
      attr_accessor :onsetString                    # 0-1 String
      ##
      # Extra information about condition
      # An area where general notes can be placed about this specific condition.
      attr_accessor :note                           # 0-* [ Annotation ]
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
    # External Id(s) for this record
    # Business identifiers assigned to this family member history by the performer or other systems which remain constant as the resource is updated and propagates from server to server.
    # This is a business identifier, not a resource identifier (see [discussion](resource.html#identifiers)).  It is best practice for the identifier to only appear on a single resource instance, however business practices may occasionally dictate that multiple resource instances with the same identifier can exist - possibly even with different resource types.  For example, multiple Patient and a Person resource instance might share the same social insurance number.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Instantiates FHIR protocol or definition
    # The URL pointing to a FHIR-defined protocol, guideline, orderset or other definition that is adhered to in whole or in part by this FamilyMemberHistory.
    attr_accessor :instantiatesCanonical          # 0-* [ canonical(http://hl7.org/fhir/StructureDefinition/PlanDefinition|http://hl7.org/fhir/StructureDefinition/Questionnaire|http://hl7.org/fhir/StructureDefinition/ActivityDefinition|http://hl7.org/fhir/StructureDefinition/Measure|http://hl7.org/fhir/StructureDefinition/OperationDefinition) ]
    ##
    # Instantiates external protocol or definition
    # The URL pointing to an externally maintained protocol, guideline, orderset or other definition that is adhered to in whole or in part by this FamilyMemberHistory.
    # This might be an HTML page, PDF, etc. or could just be a non-resolvable URI identifier.
    attr_accessor :instantiatesUri                # 0-* [ uri ]
    ##
    # partial | completed | entered-in-error | health-unknown
    # A code specifying the status of the record of the family history of a specific family member.
    # This element is labeled as a modifier because the status contains codes that mark the resource as not currently valid.
    attr_accessor :status                         # 1-1 code
    ##
    # subject-unknown | withheld | unable-to-obtain | deferred
    # Describes why the family member's history is not available.
    attr_accessor :dataAbsentReason               # 0-1 CodeableConcept
    ##
    # Patient history is about
    # The person who this history concerns.
    attr_accessor :patient                        # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient)
    ##
    # When history was recorded or last updated
    # The date (and possibly time) when the family member history was recorded or last updated.
    # This should be captured even if the same as the date on the List aggregating the full family history.
    attr_accessor :date                           # 0-1 dateTime
    ##
    # The family member described
    # This will either be a name or a description; e.g. "Aunt Susan", "my cousin with the red hair".
    attr_accessor :name                           # 0-1 string
    ##
    # Relationship to the subject
    # The type of relationship this person has to the patient (father, mother, brother etc.).
    attr_accessor :relationship                   # 1-1 CodeableConcept
    ##
    # male | female | other | unknown
    # The birth sex of the family member.
    # This element should ideally reflect whether the individual is genetically male or female.  However, as reported information based on the knowledge of the patient or reporting friend/relative, there may be situations where the reported sex might not be totally accurate.  E.g. 'Aunt Sue' might be XY rather than XX.  Questions soliciting this information should be phrased to encourage capture of genetic sex where known.  However, systems performing analysis should also allow for the possibility of imprecision with this element.
    attr_accessor :sex                            # 0-1 CodeableConcept
    ##
    # (approximate) date of birth
    # The actual or approximate date of birth of the relative.
    attr_accessor :bornDate                       # 0-1 Date
    ##
    # (approximate) date of birth
    # The actual or approximate date of birth of the relative.
    attr_accessor :bornPeriod                     # 0-1 Period
    ##
    # (approximate) date of birth
    # The actual or approximate date of birth of the relative.
    attr_accessor :bornString                     # 0-1 String
    ##
    # (approximate) age
    # The age of the relative at the time the family member history is recorded.
    # use estimatedAge to indicate whether the age is actual or not.
    attr_accessor :ageAge                         # 0-1 Age
    ##
    # (approximate) age
    # The age of the relative at the time the family member history is recorded.
    # use estimatedAge to indicate whether the age is actual or not.
    attr_accessor :ageRange                       # 0-1 Range
    ##
    # (approximate) age
    # The age of the relative at the time the family member history is recorded.
    # use estimatedAge to indicate whether the age is actual or not.
    attr_accessor :ageString                      # 0-1 String
    ##
    # Age is estimated?
    # If true, indicates that the age value specified is an estimated value.
    # This element is labeled as a modifier because the fact that age is estimated can/should change the results of any algorithm that calculates based on the specified age.
    attr_accessor :estimatedAge                   # 0-1 boolean
    ##
    # Dead? How old/when?
    # Deceased flag or the actual or approximate age of the relative at the time of death for the family member history record.
    attr_accessor :deceasedAge                    # 0-1 Age
    ##
    # Dead? How old/when?
    # Deceased flag or the actual or approximate age of the relative at the time of death for the family member history record.
    attr_accessor :deceasedBoolean                # 0-1 Boolean
    ##
    # Dead? How old/when?
    # Deceased flag or the actual or approximate age of the relative at the time of death for the family member history record.
    attr_accessor :deceasedDate                   # 0-1 Date
    ##
    # Dead? How old/when?
    # Deceased flag or the actual or approximate age of the relative at the time of death for the family member history record.
    attr_accessor :deceasedRange                  # 0-1 Range
    ##
    # Dead? How old/when?
    # Deceased flag or the actual or approximate age of the relative at the time of death for the family member history record.
    attr_accessor :deceasedString                 # 0-1 String
    ##
    # Why was family member history performed?
    # Describes why the family member history occurred in coded or textual form.
    # Textual reasons can be captured using reasonCode.text.
    attr_accessor :reasonCode                     # 0-* [ CodeableConcept ]
    ##
    # Why was family member history performed?
    # Indicates a Condition, Observation, AllergyIntolerance, or QuestionnaireResponse that justifies this family member history event.
    attr_accessor :reasonReference                # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Condition|http://hl7.org/fhir/StructureDefinition/Observation|http://hl7.org/fhir/StructureDefinition/AllergyIntolerance|http://hl7.org/fhir/StructureDefinition/QuestionnaireResponse|http://hl7.org/fhir/StructureDefinition/DiagnosticReport|http://hl7.org/fhir/StructureDefinition/DocumentReference) ]
    ##
    # General note about related person
    # This property allows a non condition-specific note to the made about the related person. Ideally, the note would be in the condition property, but this is not always possible.
    attr_accessor :note                           # 0-* [ Annotation ]
    ##
    # Condition that the related person had
    # The significant Conditions (or condition) that the family member had. This is a repeating section to allow a system to represent more than one condition per resource, though there is nothing stopping multiple resources - one per condition.
    attr_accessor :condition                      # 0-* [ FamilyMemberHistory::Condition ]

    def resourceType
      'FamilyMemberHistory'
    end
  end
end
