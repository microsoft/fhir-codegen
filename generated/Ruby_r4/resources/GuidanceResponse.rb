module FHIR

  ##
  # A guidance response is the formal response to a guidance request, including any output parameters returned by the evaluation, as well as the description of any proposed actions to be taken.
  # The GuidanceResponse resource supports recording the results of decision support interactions, reportability determination for public health, as well as the communication of additional data requirements for subsequent interactions.
  class GuidanceResponse < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['identifier', 'patient', 'request', 'subject']
    MULTIPLE_TYPES = {
      'module[x]' => ['canonical', 'CodeableConcept', 'uri']
    }
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'GuidanceResponse.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'GuidanceResponse.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'GuidanceResponse.implicitRules',
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
        'path'=>'GuidanceResponse.language',
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
        'path'=>'GuidanceResponse.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'GuidanceResponse.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'GuidanceResponse.extension',
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
        'path'=>'GuidanceResponse.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The identifier of the request associated with this response, if any
      # The identifier of the request associated with this response. If an identifier was given as part of the request, it will be reproduced here to enable the requester to more easily identify the response in a multi-request scenario.
      'requestIdentifier' => {
        'type'=>'Identifier',
        'path'=>'GuidanceResponse.requestIdentifier',
        'min'=>0,
        'max'=>1
      },
      ##
      # Business identifier
      # Allows a service to provide  unique, business identifiers for the response.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'GuidanceResponse.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # What guidance was requested
      # An identifier, CodeableConcept or canonical reference to the guidance that was requested.
      'moduleCanonical' => {
        'type'=>'Canonical',
        'path'=>'GuidanceResponse.module[x]',
        'min'=>1,
        'max'=>1
      },
      ##
      # What guidance was requested
      # An identifier, CodeableConcept or canonical reference to the guidance that was requested.
      'moduleCodeableConcept' => {
        'type'=>'CodeableConcept',
        'path'=>'GuidanceResponse.module[x]',
        'min'=>1,
        'max'=>1
      },
      ##
      # What guidance was requested
      # An identifier, CodeableConcept or canonical reference to the guidance that was requested.
      'moduleUri' => {
        'type'=>'Uri',
        'path'=>'GuidanceResponse.module[x]',
        'min'=>1,
        'max'=>1
      },
      ##
      # success | data-requested | data-required | in-progress | failure | entered-in-error
      # The status of the response. If the evaluation is completed successfully, the status will indicate success. However, in order to complete the evaluation, the engine may require more information. In this case, the status will be data-required, and the response will contain a description of the additional required information. If the evaluation completed successfully, but the engine determines that a potentially more accurate response could be provided if more data was available, the status will be data-requested, and the response will contain a description of the additional requested information.
      # This element is labeled as a modifier because the status contains codes that mark the resource as not currently valid.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/guidance-response-status'=>[ 'success', 'data-requested', 'data-required', 'in-progress', 'failure', 'entered-in-error' ]
        },
        'type'=>'code',
        'path'=>'GuidanceResponse.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/guidance-response-status'}
      },
      ##
      # Patient the request was performed for
      # The patient for which the request was processed.
      'subject' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Group'],
        'type'=>'Reference',
        'path'=>'GuidanceResponse.subject',
        'min'=>0,
        'max'=>1
      },
      ##
      # Encounter during which the response was returned
      # The encounter during which this response was created or to which the creation of this record is tightly associated.
      # This will typically be the encounter the event occurred within, but some activities may be initiated prior to or after the official copmletion of an encounter but still be tied to the context of the encounter.
      'encounter' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Encounter'],
        'type'=>'Reference',
        'path'=>'GuidanceResponse.encounter',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the guidance response was processed
      # Indicates when the guidance response was processed.
      'occurrenceDateTime' => {
        'type'=>'dateTime',
        'path'=>'GuidanceResponse.occurrenceDateTime',
        'min'=>0,
        'max'=>1
      },
      ##
      # Device returning the guidance
      # Provides a reference to the device that performed the guidance.
      'performer' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Device'],
        'type'=>'Reference',
        'path'=>'GuidanceResponse.performer',
        'min'=>0,
        'max'=>1
      },
      ##
      # Why guidance is needed
      # Describes the reason for the guidance response in coded or textual form.
      'reasonCode' => {
        'type'=>'CodeableConcept',
        'path'=>'GuidanceResponse.reasonCode',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Why guidance is needed
      # Indicates the reason the request was initiated. This is typically provided as a parameter to the evaluation and echoed by the service, although for some use cases, such as subscription- or event-based scenarios, it may provide an indication of the cause for the response.
      'reasonReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Condition', 'http://hl7.org/fhir/StructureDefinition/Observation', 'http://hl7.org/fhir/StructureDefinition/DiagnosticReport', 'http://hl7.org/fhir/StructureDefinition/DocumentReference'],
        'type'=>'Reference',
        'path'=>'GuidanceResponse.reasonReference',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional notes about the response
      # Provides a mechanism to communicate additional information about the response.
      'note' => {
        'type'=>'Annotation',
        'path'=>'GuidanceResponse.note',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Messages resulting from the evaluation of the artifact or artifacts. As part of evaluating the request, the engine may produce informational or warning messages. These messages will be provided by this element.
      'evaluationMessage' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/OperationOutcome'],
        'type'=>'Reference',
        'path'=>'GuidanceResponse.evaluationMessage',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The output parameters of the evaluation, if any. Many modules will result in the return of specific resources such as procedure or communication requests that are returned as part of the operation result. However, modules may define specific outputs that would be returned as the result of the evaluation, and these would be returned in this element.
      'outputParameters' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Parameters'],
        'type'=>'Reference',
        'path'=>'GuidanceResponse.outputParameters',
        'min'=>0,
        'max'=>1
      },
      ##
      # Proposed actions, if any
      # The actions, if any, produced by the evaluation of the artifact.
      'result' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/CarePlan', 'http://hl7.org/fhir/StructureDefinition/RequestGroup'],
        'type'=>'Reference',
        'path'=>'GuidanceResponse.result',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional required data
      # If the evaluation could not be completed due to lack of information, or additional information would potentially result in a more accurate response, this element will a description of the data required in order to proceed with the evaluation. A subsequent request to the service should include this data.
      'dataRequirement' => {
        'type'=>'DataRequirement',
        'path'=>'GuidanceResponse.dataRequirement',
        'min'=>0,
        'max'=>Float::INFINITY
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
    # The identifier of the request associated with this response, if any
    # The identifier of the request associated with this response. If an identifier was given as part of the request, it will be reproduced here to enable the requester to more easily identify the response in a multi-request scenario.
    attr_accessor :requestIdentifier              # 0-1 Identifier
    ##
    # Business identifier
    # Allows a service to provide  unique, business identifiers for the response.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # What guidance was requested
    # An identifier, CodeableConcept or canonical reference to the guidance that was requested.
    attr_accessor :moduleCanonical                # 1-1 Canonical
    ##
    # What guidance was requested
    # An identifier, CodeableConcept or canonical reference to the guidance that was requested.
    attr_accessor :moduleCodeableConcept          # 1-1 CodeableConcept
    ##
    # What guidance was requested
    # An identifier, CodeableConcept or canonical reference to the guidance that was requested.
    attr_accessor :moduleUri                      # 1-1 Uri
    ##
    # success | data-requested | data-required | in-progress | failure | entered-in-error
    # The status of the response. If the evaluation is completed successfully, the status will indicate success. However, in order to complete the evaluation, the engine may require more information. In this case, the status will be data-required, and the response will contain a description of the additional required information. If the evaluation completed successfully, but the engine determines that a potentially more accurate response could be provided if more data was available, the status will be data-requested, and the response will contain a description of the additional requested information.
    # This element is labeled as a modifier because the status contains codes that mark the resource as not currently valid.
    attr_accessor :status                         # 1-1 code
    ##
    # Patient the request was performed for
    # The patient for which the request was processed.
    attr_accessor :subject                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Group)
    ##
    # Encounter during which the response was returned
    # The encounter during which this response was created or to which the creation of this record is tightly associated.
    # This will typically be the encounter the event occurred within, but some activities may be initiated prior to or after the official copmletion of an encounter but still be tied to the context of the encounter.
    attr_accessor :encounter                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Encounter)
    ##
    # When the guidance response was processed
    # Indicates when the guidance response was processed.
    attr_accessor :occurrenceDateTime             # 0-1 dateTime
    ##
    # Device returning the guidance
    # Provides a reference to the device that performed the guidance.
    attr_accessor :performer                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Device)
    ##
    # Why guidance is needed
    # Describes the reason for the guidance response in coded or textual form.
    attr_accessor :reasonCode                     # 0-* [ CodeableConcept ]
    ##
    # Why guidance is needed
    # Indicates the reason the request was initiated. This is typically provided as a parameter to the evaluation and echoed by the service, although for some use cases, such as subscription- or event-based scenarios, it may provide an indication of the cause for the response.
    attr_accessor :reasonReference                # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Condition|http://hl7.org/fhir/StructureDefinition/Observation|http://hl7.org/fhir/StructureDefinition/DiagnosticReport|http://hl7.org/fhir/StructureDefinition/DocumentReference) ]
    ##
    # Additional notes about the response
    # Provides a mechanism to communicate additional information about the response.
    attr_accessor :note                           # 0-* [ Annotation ]
    ##
    # Messages resulting from the evaluation of the artifact or artifacts. As part of evaluating the request, the engine may produce informational or warning messages. These messages will be provided by this element.
    attr_accessor :evaluationMessage              # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/OperationOutcome) ]
    ##
    # The output parameters of the evaluation, if any. Many modules will result in the return of specific resources such as procedure or communication requests that are returned as part of the operation result. However, modules may define specific outputs that would be returned as the result of the evaluation, and these would be returned in this element.
    attr_accessor :outputParameters               # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Parameters)
    ##
    # Proposed actions, if any
    # The actions, if any, produced by the evaluation of the artifact.
    attr_accessor :result                         # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/CarePlan|http://hl7.org/fhir/StructureDefinition/RequestGroup)
    ##
    # Additional required data
    # If the evaluation could not be completed due to lack of information, or additional information would potentially result in a more accurate response, this element will a description of the data required in order to proceed with the evaluation. A subsequent request to the service should include this data.
    attr_accessor :dataRequirement                # 0-* [ DataRequirement ]

    def resourceType
      'GuidanceResponse'
    end
  end
end
