module FHIR

  ##
  # A record of a device being used by a patient where the record is the result of a report from the patient or another clinician.
  class DeviceUseStatement < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['device', 'identifier', 'patient', 'subject']
    MULTIPLE_TYPES = {
      'timing[x]' => ['dateTime', 'Period', 'Timing']
    }
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'DeviceUseStatement.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'DeviceUseStatement.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'DeviceUseStatement.implicitRules',
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
        'path'=>'DeviceUseStatement.language',
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
        'path'=>'DeviceUseStatement.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'DeviceUseStatement.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'DeviceUseStatement.extension',
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
        'path'=>'DeviceUseStatement.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # External identifier for this record
      # An external identifier for this statement such as an IRI.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'DeviceUseStatement.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Fulfills plan, proposal or order
      # A plan, proposal or order that is fulfilled in whole or in part by this DeviceUseStatement.
      'basedOn' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ServiceRequest'],
        'type'=>'Reference',
        'path'=>'DeviceUseStatement.basedOn',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # active | completed | entered-in-error +
      # A code representing the patient or other source's judgment about the state of the device used that this statement is about.  Generally this will be active or completed.
      # DeviceUseStatment is a statement at a point in time.  The status is only representative at the point when it was asserted.  The value set for contains codes that assert the status of the use  by the patient (for example, stopped or on hold) as well as codes that assert the status of the resource itself (for example, entered in error).This element is labeled as a modifier because the status contains the codes that mark the statement as not currently valid.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/device-statement-status'=>[ 'active', 'completed', 'entered-in-error', 'intended', 'stopped', 'on-hold' ]
        },
        'type'=>'code',
        'path'=>'DeviceUseStatement.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/device-statement-status'}
      },
      ##
      # Patient using device
      # The patient who used the device.
      'subject' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Group'],
        'type'=>'Reference',
        'path'=>'DeviceUseStatement.subject',
        'min'=>1,
        'max'=>1
      },
      ##
      # Supporting information
      # Allows linking the DeviceUseStatement to the underlying Request, or to other information that supports or is used to derive the DeviceUseStatement.
      # The most common use cases for deriving a DeviceUseStatement comes from creating it from a request or from an observation or a claim. it should be noted that the amount of information that is available varies from the type resource that you derive the DeviceUseStatement from.
      'derivedFrom' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ServiceRequest', 'http://hl7.org/fhir/StructureDefinition/Procedure', 'http://hl7.org/fhir/StructureDefinition/Claim', 'http://hl7.org/fhir/StructureDefinition/Observation', 'http://hl7.org/fhir/StructureDefinition/QuestionnaireResponse', 'http://hl7.org/fhir/StructureDefinition/DocumentReference'],
        'type'=>'Reference',
        'path'=>'DeviceUseStatement.derivedFrom',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # How often  the device was used
      # How often the device was used.
      'timingDateTime' => {
        'type'=>'DateTime',
        'path'=>'DeviceUseStatement.timing[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # How often  the device was used
      # How often the device was used.
      'timingPeriod' => {
        'type'=>'Period',
        'path'=>'DeviceUseStatement.timing[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # How often  the device was used
      # How often the device was used.
      'timingTiming' => {
        'type'=>'Timing',
        'path'=>'DeviceUseStatement.timing[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # When statement was recorded
      # The time at which the statement was made/recorded.
      'recordedOn' => {
        'type'=>'dateTime',
        'path'=>'DeviceUseStatement.recordedOn',
        'min'=>0,
        'max'=>1
      },
      ##
      # Who made the statement
      # Who reported the device was being used by the patient.
      'source' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson'],
        'type'=>'Reference',
        'path'=>'DeviceUseStatement.source',
        'min'=>0,
        'max'=>1
      },
      ##
      # Reference to device used
      # The details of the device used.
      'device' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Device'],
        'type'=>'Reference',
        'path'=>'DeviceUseStatement.device',
        'min'=>1,
        'max'=>1
      },
      ##
      # Why device was used
      # Reason or justification for the use of the device.
      'reasonCode' => {
        'type'=>'CodeableConcept',
        'path'=>'DeviceUseStatement.reasonCode',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Why was DeviceUseStatement performed?
      # Indicates another resource whose existence justifies this DeviceUseStatement.
      'reasonReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Condition', 'http://hl7.org/fhir/StructureDefinition/Observation', 'http://hl7.org/fhir/StructureDefinition/DiagnosticReport', 'http://hl7.org/fhir/StructureDefinition/DocumentReference', 'http://hl7.org/fhir/StructureDefinition/Media'],
        'type'=>'Reference',
        'path'=>'DeviceUseStatement.reasonReference',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Target body site
      # Indicates the anotomic location on the subject's body where the device was used ( i.e. the target).
      'bodySite' => {
        'type'=>'CodeableConcept',
        'path'=>'DeviceUseStatement.bodySite',
        'min'=>0,
        'max'=>1
      },
      ##
      # Addition details (comments, instructions)
      # Details about the device statement that were not represented at all or sufficiently in one of the attributes provided in a class. These may include for example a comment, an instruction, or a note associated with the statement.
      'note' => {
        'type'=>'Annotation',
        'path'=>'DeviceUseStatement.note',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }
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
    # External identifier for this record
    # An external identifier for this statement such as an IRI.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Fulfills plan, proposal or order
    # A plan, proposal or order that is fulfilled in whole or in part by this DeviceUseStatement.
    attr_accessor :basedOn                        # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/ServiceRequest) ]
    ##
    # active | completed | entered-in-error +
    # A code representing the patient or other source's judgment about the state of the device used that this statement is about.  Generally this will be active or completed.
    # DeviceUseStatment is a statement at a point in time.  The status is only representative at the point when it was asserted.  The value set for contains codes that assert the status of the use  by the patient (for example, stopped or on hold) as well as codes that assert the status of the resource itself (for example, entered in error).This element is labeled as a modifier because the status contains the codes that mark the statement as not currently valid.
    attr_accessor :status                         # 1-1 code
    ##
    # Patient using device
    # The patient who used the device.
    attr_accessor :subject                        # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Group)
    ##
    # Supporting information
    # Allows linking the DeviceUseStatement to the underlying Request, or to other information that supports or is used to derive the DeviceUseStatement.
    # The most common use cases for deriving a DeviceUseStatement comes from creating it from a request or from an observation or a claim. it should be noted that the amount of information that is available varies from the type resource that you derive the DeviceUseStatement from.
    attr_accessor :derivedFrom                    # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/ServiceRequest|http://hl7.org/fhir/StructureDefinition/Procedure|http://hl7.org/fhir/StructureDefinition/Claim|http://hl7.org/fhir/StructureDefinition/Observation|http://hl7.org/fhir/StructureDefinition/QuestionnaireResponse|http://hl7.org/fhir/StructureDefinition/DocumentReference) ]
    ##
    # How often  the device was used
    # How often the device was used.
    attr_accessor :timingDateTime                 # 0-1 DateTime
    ##
    # How often  the device was used
    # How often the device was used.
    attr_accessor :timingPeriod                   # 0-1 Period
    ##
    # How often  the device was used
    # How often the device was used.
    attr_accessor :timingTiming                   # 0-1 Timing
    ##
    # When statement was recorded
    # The time at which the statement was made/recorded.
    attr_accessor :recordedOn                     # 0-1 dateTime
    ##
    # Who made the statement
    # Who reported the device was being used by the patient.
    attr_accessor :source                         # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/RelatedPerson)
    ##
    # Reference to device used
    # The details of the device used.
    attr_accessor :device                         # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Device)
    ##
    # Why device was used
    # Reason or justification for the use of the device.
    attr_accessor :reasonCode                     # 0-* [ CodeableConcept ]
    ##
    # Why was DeviceUseStatement performed?
    # Indicates another resource whose existence justifies this DeviceUseStatement.
    attr_accessor :reasonReference                # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Condition|http://hl7.org/fhir/StructureDefinition/Observation|http://hl7.org/fhir/StructureDefinition/DiagnosticReport|http://hl7.org/fhir/StructureDefinition/DocumentReference|http://hl7.org/fhir/StructureDefinition/Media) ]
    ##
    # Target body site
    # Indicates the anotomic location on the subject's body where the device was used ( i.e. the target).
    attr_accessor :bodySite                       # 0-1 CodeableConcept
    ##
    # Addition details (comments, instructions)
    # Details about the device statement that were not represented at all or sufficiently in one of the attributes provided in a class. These may include for example a comment, an instruction, or a note associated with the statement.
    attr_accessor :note                           # 0-* [ Annotation ]

    def resourceType
      'DeviceUseStatement'
    end
  end
end
