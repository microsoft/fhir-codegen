module FHIR

  ##
  # Describes a comparison of an immunization event against published recommendations to determine if the administration is "valid" in relation to those  recommendations.
  class ImmunizationEvaluation < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['date', 'dose-status', 'identifier', 'immunization-event', 'patient', 'status', 'target-disease']
    MULTIPLE_TYPES = {
      'doseNumber[x]' => ['positiveInt', 'string'],
      'seriesDoses[x]' => ['positiveInt', 'string']
    }
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'ImmunizationEvaluation.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'ImmunizationEvaluation.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'ImmunizationEvaluation.implicitRules',
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
        'path'=>'ImmunizationEvaluation.language',
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
        'path'=>'ImmunizationEvaluation.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'ImmunizationEvaluation.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'ImmunizationEvaluation.extension',
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
        'path'=>'ImmunizationEvaluation.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Business identifier
      # A unique identifier assigned to this immunization evaluation record.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'ImmunizationEvaluation.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # completed | entered-in-error
      # Indicates the current status of the evaluation of the vaccination administration event.
      'status' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/medication-admin-status'=>[ 'completed', 'entered-in-error' ]
        },
        'type'=>'code',
        'path'=>'ImmunizationEvaluation.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/immunization-evaluation-status'}
      },
      ##
      # Who this evaluation is for
      # The individual for whom the evaluation is being done.
      'patient' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient'],
        'type'=>'Reference',
        'path'=>'ImmunizationEvaluation.patient',
        'min'=>1,
        'max'=>1
      },
      ##
      # Date evaluation was performed
      # The date the evaluation of the vaccine administration event was performed.
      'date' => {
        'type'=>'dateTime',
        'path'=>'ImmunizationEvaluation.date',
        'min'=>0,
        'max'=>1
      },
      ##
      # Who is responsible for publishing the recommendations
      # Indicates the authority who published the protocol (e.g. ACIP).
      'authority' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'ImmunizationEvaluation.authority',
        'min'=>0,
        'max'=>1
      },
      ##
      # Evaluation target disease
      # The vaccine preventable disease the dose is being evaluated against.
      'targetDisease' => {
        'type'=>'CodeableConcept',
        'path'=>'ImmunizationEvaluation.targetDisease',
        'min'=>1,
        'max'=>1
      },
      ##
      # Immunization being evaluated
      # The vaccine administration event being evaluated.
      'immunizationEvent' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Immunization'],
        'type'=>'Reference',
        'path'=>'ImmunizationEvaluation.immunizationEvent',
        'min'=>1,
        'max'=>1
      },
      ##
      # Status of the dose relative to published recommendations
      # Indicates if the dose is valid or not valid with respect to the published recommendations.
      'doseStatus' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/immunization-evaluation-dose-status'=>[ 'valid', 'notvalid' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'ImmunizationEvaluation.doseStatus',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/immunization-evaluation-dose-status'}
      },
      ##
      # Reason for the dose status
      # Provides an explanation as to why the vaccine administration event is valid or not relative to the published recommendations.
      'doseStatusReason' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/immunization-evaluation-dose-status-reason'=>[ 'advstorage', 'coldchbrk', 'explot', 'outsidesched', 'prodrecall' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'ImmunizationEvaluation.doseStatusReason',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/immunization-evaluation-dose-status-reason'}
      },
      ##
      # Evaluation notes
      # Additional information about the evaluation.
      'description' => {
        'type'=>'string',
        'path'=>'ImmunizationEvaluation.description',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name of vaccine series
      # One possible path to achieve presumed immunity against a disease - within the context of an authority.
      'series' => {
        'type'=>'string',
        'path'=>'ImmunizationEvaluation.series',
        'min'=>0,
        'max'=>1
      },
      ##
      # Dose number within series
      # Nominal position in a series.
      # The use of an integer is preferred if known. A string should only be used in cases where an integer is not available (such as when documenting a recurring booster dose).
      'doseNumberPositiveInt' => {
        'type'=>'PositiveInt',
        'path'=>'ImmunizationEvaluation.doseNumber[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Dose number within series
      # Nominal position in a series.
      # The use of an integer is preferred if known. A string should only be used in cases where an integer is not available (such as when documenting a recurring booster dose).
      'doseNumberString' => {
        'type'=>'String',
        'path'=>'ImmunizationEvaluation.doseNumber[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Recommended number of doses for immunity
      # The recommended number of doses to achieve immunity.
      # The use of an integer is preferred if known. A string should only be used in cases where an integer is not available (such as when documenting a recurring booster dose).
      'seriesDosesPositiveInt' => {
        'type'=>'PositiveInt',
        'path'=>'ImmunizationEvaluation.seriesDoses[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Recommended number of doses for immunity
      # The recommended number of doses to achieve immunity.
      # The use of an integer is preferred if known. A string should only be used in cases where an integer is not available (such as when documenting a recurring booster dose).
      'seriesDosesString' => {
        'type'=>'String',
        'path'=>'ImmunizationEvaluation.seriesDoses[x]',
        'min'=>0,
        'max'=>1
      }
    }
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
    # Business identifier
    # A unique identifier assigned to this immunization evaluation record.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # completed | entered-in-error
    # Indicates the current status of the evaluation of the vaccination administration event.
    attr_accessor :status                         # 1-1 code
    ##
    # Who this evaluation is for
    # The individual for whom the evaluation is being done.
    attr_accessor :patient                        # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient)
    ##
    # Date evaluation was performed
    # The date the evaluation of the vaccine administration event was performed.
    attr_accessor :date                           # 0-1 dateTime
    ##
    # Who is responsible for publishing the recommendations
    # Indicates the authority who published the protocol (e.g. ACIP).
    attr_accessor :authority                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Evaluation target disease
    # The vaccine preventable disease the dose is being evaluated against.
    attr_accessor :targetDisease                  # 1-1 CodeableConcept
    ##
    # Immunization being evaluated
    # The vaccine administration event being evaluated.
    attr_accessor :immunizationEvent              # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Immunization)
    ##
    # Status of the dose relative to published recommendations
    # Indicates if the dose is valid or not valid with respect to the published recommendations.
    attr_accessor :doseStatus                     # 1-1 CodeableConcept
    ##
    # Reason for the dose status
    # Provides an explanation as to why the vaccine administration event is valid or not relative to the published recommendations.
    attr_accessor :doseStatusReason               # 0-* [ CodeableConcept ]
    ##
    # Evaluation notes
    # Additional information about the evaluation.
    attr_accessor :description                    # 0-1 string
    ##
    # Name of vaccine series
    # One possible path to achieve presumed immunity against a disease - within the context of an authority.
    attr_accessor :series                         # 0-1 string
    ##
    # Dose number within series
    # Nominal position in a series.
    # The use of an integer is preferred if known. A string should only be used in cases where an integer is not available (such as when documenting a recurring booster dose).
    attr_accessor :doseNumberPositiveInt          # 0-1 PositiveInt
    ##
    # Dose number within series
    # Nominal position in a series.
    # The use of an integer is preferred if known. A string should only be used in cases where an integer is not available (such as when documenting a recurring booster dose).
    attr_accessor :doseNumberString               # 0-1 String
    ##
    # Recommended number of doses for immunity
    # The recommended number of doses to achieve immunity.
    # The use of an integer is preferred if known. A string should only be used in cases where an integer is not available (such as when documenting a recurring booster dose).
    attr_accessor :seriesDosesPositiveInt         # 0-1 PositiveInt
    ##
    # Recommended number of doses for immunity
    # The recommended number of doses to achieve immunity.
    # The use of an integer is preferred if known. A string should only be used in cases where an integer is not available (such as when documenting a recurring booster dose).
    attr_accessor :seriesDosesString              # 0-1 String

    def resourceType
      'ImmunizationEvaluation'
    end
  end
end
