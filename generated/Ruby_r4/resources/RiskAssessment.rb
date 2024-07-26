module FHIR

  ##
  # An assessment of the likely outcome(s) for a patient or other subject as well as the likelihood of each outcome.
  class RiskAssessment < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['condition', 'date', 'encounter', 'identifier', 'method', 'patient', 'performer', 'probability', 'risk', 'subject']
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
        'path'=>'RiskAssessment.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'RiskAssessment.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'RiskAssessment.implicitRules',
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
        'path'=>'RiskAssessment.language',
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
        'path'=>'RiskAssessment.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'RiskAssessment.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'RiskAssessment.extension',
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
        'path'=>'RiskAssessment.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Unique identifier for the assessment
      # Business identifier assigned to the risk assessment.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'RiskAssessment.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Request fulfilled by this assessment
      # A reference to the request that is fulfilled by this risk assessment.
      'basedOn' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'Reference',
        'path'=>'RiskAssessment.basedOn',
        'min'=>0,
        'max'=>1
      },
      ##
      # Part of this occurrence
      # A reference to a resource that this risk assessment is part of, such as a Procedure.
      'parent' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'Reference',
        'path'=>'RiskAssessment.parent',
        'min'=>0,
        'max'=>1
      },
      ##
      # registered | preliminary | final | amended +
      # The status of the RiskAssessment, using the same statuses as an Observation.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/observation-status'=>[ 'registered', 'preliminary', 'final', 'amended', 'corrected', 'cancelled', 'entered-in-error', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'RiskAssessment.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/observation-status'}
      },
      ##
      # Evaluation mechanism
      # The algorithm, process or mechanism used to evaluate the risk.
      'method' => {
        'local_name'=>'local_method'
        'type'=>'CodeableConcept',
        'path'=>'RiskAssessment.method',
        'min'=>0,
        'max'=>1
      },
      ##
      # Type of assessment
      # The type of the risk assessment performed.
      'code' => {
        'type'=>'CodeableConcept',
        'path'=>'RiskAssessment.code',
        'min'=>0,
        'max'=>1
      },
      ##
      # Who/what does assessment apply to?
      # The patient or group the risk assessment applies to.
      'subject' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Group'],
        'type'=>'Reference',
        'path'=>'RiskAssessment.subject',
        'min'=>1,
        'max'=>1
      },
      ##
      # Where was assessment performed?
      # The encounter where the assessment was performed.
      'encounter' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Encounter'],
        'type'=>'Reference',
        'path'=>'RiskAssessment.encounter',
        'min'=>0,
        'max'=>1
      },
      ##
      # When was assessment made?
      # The date (and possibly time) the risk assessment was performed.
      'occurrenceDateTime' => {
        'type'=>'DateTime',
        'path'=>'RiskAssessment.occurrence[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # When was assessment made?
      # The date (and possibly time) the risk assessment was performed.
      'occurrencePeriod' => {
        'type'=>'Period',
        'path'=>'RiskAssessment.occurrence[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Condition assessed
      # For assessments or prognosis specific to a particular condition, indicates the condition being assessed.
      'condition' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Condition'],
        'type'=>'Reference',
        'path'=>'RiskAssessment.condition',
        'min'=>0,
        'max'=>1
      },
      ##
      # Who did assessment?
      # The provider or software application that performed the assessment.
      'performer' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Device'],
        'type'=>'Reference',
        'path'=>'RiskAssessment.performer',
        'min'=>0,
        'max'=>1
      },
      ##
      # Why the assessment was necessary?
      # The reason the risk assessment was performed.
      'reasonCode' => {
        'type'=>'CodeableConcept',
        'path'=>'RiskAssessment.reasonCode',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Why the assessment was necessary?
      # Resources supporting the reason the risk assessment was performed.
      'reasonReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Condition', 'http://hl7.org/fhir/StructureDefinition/Observation', 'http://hl7.org/fhir/StructureDefinition/DiagnosticReport', 'http://hl7.org/fhir/StructureDefinition/DocumentReference'],
        'type'=>'Reference',
        'path'=>'RiskAssessment.reasonReference',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Information used in assessment
      # Indicates the source data considered as part of the assessment (for example, FamilyHistory, Observations, Procedures, Conditions, etc.).
      'basis' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'Reference',
        'path'=>'RiskAssessment.basis',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Outcome predicted
      # Describes the expected outcome for the subject.
      # Multiple repetitions can be used to identify the same type of outcome in different timeframes as well as different types of outcomes.
      'prediction' => {
        'type'=>'RiskAssessment::Prediction',
        'path'=>'RiskAssessment.prediction',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # How to reduce risk
      # A description of the steps that might be taken to reduce the identified risk(s).
      'mitigation' => {
        'type'=>'string',
        'path'=>'RiskAssessment.mitigation',
        'min'=>0,
        'max'=>1
      },
      ##
      # Comments on the risk assessment
      # Additional comments about the risk assessment.
      'note' => {
        'type'=>'Annotation',
        'path'=>'RiskAssessment.note',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Outcome predicted
    # Describes the expected outcome for the subject.
    # Multiple repetitions can be used to identify the same type of outcome in different timeframes as well as different types of outcomes.
    class Prediction < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'probability[x]' => ['decimal', 'Range'],
        'when[x]' => ['Period', 'Range']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Prediction.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Prediction.extension',
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
          'path'=>'Prediction.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Possible outcome for the subject
        # One of the potential outcomes for the patient (e.g. remission, death,  a particular condition).
        'outcome' => {
          'type'=>'CodeableConcept',
          'path'=>'Prediction.outcome',
          'min'=>0,
          'max'=>1
        },
        ##
        # Likelihood of specified outcome
        # Indicates how likely the outcome is (in the specified timeframe).
        # If range is used, it represents the lower and upper bounds of certainty; e.g. 40-60%  Decimal values are expressed as percentages as well (max = 100).
        'probabilityDecimal' => {
          'type'=>'Decimal',
          'path'=>'Prediction.probability[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Likelihood of specified outcome
        # Indicates how likely the outcome is (in the specified timeframe).
        # If range is used, it represents the lower and upper bounds of certainty; e.g. 40-60%  Decimal values are expressed as percentages as well (max = 100).
        'probabilityRange' => {
          'type'=>'Range',
          'path'=>'Prediction.probability[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Likelihood of specified outcome as a qualitative value
        # Indicates how likely the outcome is (in the specified timeframe), expressed as a qualitative value (e.g. low, medium, or high).
        'qualitativeRisk' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/risk-probability'=>[ 'negligible', 'low', 'moderate', 'high', 'certain' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Prediction.qualitativeRisk',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/risk-probability'}
        },
        ##
        # Relative likelihood
        # Indicates the risk for this particular subject (with their specific characteristics) divided by the risk of the population in general.  (Numbers greater than 1 = higher risk than the population, numbers less than 1 = lower risk.).
        'relativeRisk' => {
          'type'=>'decimal',
          'path'=>'Prediction.relativeRisk',
          'min'=>0,
          'max'=>1
        },
        ##
        # Timeframe or age range
        # Indicates the period of time or age range of the subject to which the specified probability applies.
        # If not specified, the risk applies "over the subject's lifespan".
        'whenPeriod' => {
          'type'=>'Period',
          'path'=>'Prediction.when[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Timeframe or age range
        # Indicates the period of time or age range of the subject to which the specified probability applies.
        # If not specified, the risk applies "over the subject's lifespan".
        'whenRange' => {
          'type'=>'Range',
          'path'=>'Prediction.when[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Explanation of prediction
        # Additional information explaining the basis for the prediction.
        'rationale' => {
          'type'=>'string',
          'path'=>'Prediction.rationale',
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
      # Possible outcome for the subject
      # One of the potential outcomes for the patient (e.g. remission, death,  a particular condition).
      attr_accessor :outcome                        # 0-1 CodeableConcept
      ##
      # Likelihood of specified outcome
      # Indicates how likely the outcome is (in the specified timeframe).
      # If range is used, it represents the lower and upper bounds of certainty; e.g. 40-60%  Decimal values are expressed as percentages as well (max = 100).
      attr_accessor :probabilityDecimal             # 0-1 Decimal
      ##
      # Likelihood of specified outcome
      # Indicates how likely the outcome is (in the specified timeframe).
      # If range is used, it represents the lower and upper bounds of certainty; e.g. 40-60%  Decimal values are expressed as percentages as well (max = 100).
      attr_accessor :probabilityRange               # 0-1 Range
      ##
      # Likelihood of specified outcome as a qualitative value
      # Indicates how likely the outcome is (in the specified timeframe), expressed as a qualitative value (e.g. low, medium, or high).
      attr_accessor :qualitativeRisk                # 0-1 CodeableConcept
      ##
      # Relative likelihood
      # Indicates the risk for this particular subject (with their specific characteristics) divided by the risk of the population in general.  (Numbers greater than 1 = higher risk than the population, numbers less than 1 = lower risk.).
      attr_accessor :relativeRisk                   # 0-1 decimal
      ##
      # Timeframe or age range
      # Indicates the period of time or age range of the subject to which the specified probability applies.
      # If not specified, the risk applies "over the subject's lifespan".
      attr_accessor :whenPeriod                     # 0-1 Period
      ##
      # Timeframe or age range
      # Indicates the period of time or age range of the subject to which the specified probability applies.
      # If not specified, the risk applies "over the subject's lifespan".
      attr_accessor :whenRange                      # 0-1 Range
      ##
      # Explanation of prediction
      # Additional information explaining the basis for the prediction.
      attr_accessor :rationale                      # 0-1 string
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
    # Unique identifier for the assessment
    # Business identifier assigned to the risk assessment.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Request fulfilled by this assessment
    # A reference to the request that is fulfilled by this risk assessment.
    attr_accessor :basedOn                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Resource)
    ##
    # Part of this occurrence
    # A reference to a resource that this risk assessment is part of, such as a Procedure.
    attr_accessor :parent                         # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Resource)
    ##
    # registered | preliminary | final | amended +
    # The status of the RiskAssessment, using the same statuses as an Observation.
    attr_accessor :status                         # 1-1 code
    ##
    # Evaluation mechanism
    # The algorithm, process or mechanism used to evaluate the risk.
    attr_accessor :local_method                   # 0-1 CodeableConcept
    ##
    # Type of assessment
    # The type of the risk assessment performed.
    attr_accessor :code                           # 0-1 CodeableConcept
    ##
    # Who/what does assessment apply to?
    # The patient or group the risk assessment applies to.
    attr_accessor :subject                        # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Group)
    ##
    # Where was assessment performed?
    # The encounter where the assessment was performed.
    attr_accessor :encounter                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Encounter)
    ##
    # When was assessment made?
    # The date (and possibly time) the risk assessment was performed.
    attr_accessor :occurrenceDateTime             # 0-1 DateTime
    ##
    # When was assessment made?
    # The date (and possibly time) the risk assessment was performed.
    attr_accessor :occurrencePeriod               # 0-1 Period
    ##
    # Condition assessed
    # For assessments or prognosis specific to a particular condition, indicates the condition being assessed.
    attr_accessor :condition                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Condition)
    ##
    # Who did assessment?
    # The provider or software application that performed the assessment.
    attr_accessor :performer                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Device)
    ##
    # Why the assessment was necessary?
    # The reason the risk assessment was performed.
    attr_accessor :reasonCode                     # 0-* [ CodeableConcept ]
    ##
    # Why the assessment was necessary?
    # Resources supporting the reason the risk assessment was performed.
    attr_accessor :reasonReference                # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Condition|http://hl7.org/fhir/StructureDefinition/Observation|http://hl7.org/fhir/StructureDefinition/DiagnosticReport|http://hl7.org/fhir/StructureDefinition/DocumentReference) ]
    ##
    # Information used in assessment
    # Indicates the source data considered as part of the assessment (for example, FamilyHistory, Observations, Procedures, Conditions, etc.).
    attr_accessor :basis                          # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Resource) ]
    ##
    # Outcome predicted
    # Describes the expected outcome for the subject.
    # Multiple repetitions can be used to identify the same type of outcome in different timeframes as well as different types of outcomes.
    attr_accessor :prediction                     # 0-* [ RiskAssessment::Prediction ]
    ##
    # How to reduce risk
    # A description of the steps that might be taken to reduce the identified risk(s).
    attr_accessor :mitigation                     # 0-1 string
    ##
    # Comments on the risk assessment
    # Additional comments about the risk assessment.
    attr_accessor :note                           # 0-* [ Annotation ]

    def resourceType
      'RiskAssessment'
    end
  end
end
