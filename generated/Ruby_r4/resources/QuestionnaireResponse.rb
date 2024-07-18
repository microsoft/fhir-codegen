module FHIR

  ##
  # A structured set of questions and their answers. The questions are ordered and grouped into coherent subsets, corresponding to the structure of the grouping of the questionnaire being responded to.
  # To support structured, hierarchical reporting of data gathered using digital forms and other questionnaires.
  class QuestionnaireResponse < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['author', 'authored', 'based-on', 'encounter', 'identifier', 'item-subject', 'part-of', 'patient', 'questionnaire', 'source', 'status', 'subject']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'QuestionnaireResponse.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'QuestionnaireResponse.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'QuestionnaireResponse.implicitRules',
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
        'path'=>'QuestionnaireResponse.language',
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
        'path'=>'QuestionnaireResponse.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'QuestionnaireResponse.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'QuestionnaireResponse.extension',
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
        'path'=>'QuestionnaireResponse.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Unique id for this set of answers
      # A business identifier assigned to a particular completed (or partially completed) questionnaire.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'QuestionnaireResponse.identifier',
        'min'=>0,
        'max'=>1
      },
      ##
      # Request fulfilled by this QuestionnaireResponse
      # The order, proposal or plan that is fulfilled in whole or in part by this QuestionnaireResponse.  For example, a ServiceRequest seeking an intake assessment or a decision support recommendation to assess for post-partum depression.
      'basedOn' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/CarePlan', 'http://hl7.org/fhir/StructureDefinition/ServiceRequest'],
        'type'=>'Reference',
        'path'=>'QuestionnaireResponse.basedOn',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Part of this action
      # A procedure or observation that this questionnaire was performed as part of the execution of.  For example, the surgery a checklist was executed as part of.
      # Composition of questionnaire responses will be handled by the parent questionnaire having answers that reference the child questionnaire.  For relationships to referrals, and other types of requests, use basedOn.
      'partOf' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Observation', 'http://hl7.org/fhir/StructureDefinition/Procedure'],
        'type'=>'Reference',
        'path'=>'QuestionnaireResponse.partOf',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Form being answered
      # The Questionnaire that defines and organizes the questions for which answers are being provided.
      # If a QuestionnaireResponse references a Questionnaire, then the QuestionnaireResponse structure must be consistent with the Questionnaire (i.e. questions must be organized into the same groups, nested questions must still be nested, etc.).
      'questionnaire' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Questionnaire'],
        'type'=>'canonical',
        'path'=>'QuestionnaireResponse.questionnaire',
        'min'=>0,
        'max'=>1
      },
      ##
      # in-progress | completed | amended | entered-in-error | stopped
      # The position of the questionnaire response within its overall lifecycle.
      # This element is labeled as a modifier because the status contains codes that mark the resource as not currently valid.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/questionnaire-answers-status'=>[ 'in-progress', 'completed', 'amended', 'entered-in-error', 'stopped' ]
        },
        'type'=>'code',
        'path'=>'QuestionnaireResponse.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/questionnaire-answers-status'}
      },
      ##
      # The subject of the questions
      # The subject of the questionnaire response.  This could be a patient, organization, practitioner, device, etc.  This is who/what the answers apply to, but is not necessarily the source of information.
      # If the Questionnaire declared a subjectType, the resource pointed to by this element must be an instance of one of the listed types.
      'subject' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'Reference',
        'path'=>'QuestionnaireResponse.subject',
        'min'=>0,
        'max'=>1
      },
      ##
      # Encounter created as part of
      # The Encounter during which this questionnaire response was created or to which the creation of this record is tightly associated.
      # This will typically be the encounter the event occurred within, but some activities may be initiated prior to or after the official completion of an encounter but still be tied to the context of the encounter. A questionnaire that was initiated during an encounter but not fully completed during the encounter would still generally be associated with the encounter.
      'encounter' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Encounter'],
        'type'=>'Reference',
        'path'=>'QuestionnaireResponse.encounter',
        'min'=>0,
        'max'=>1
      },
      ##
      # Date the answers were gathered
      # The date and/or time that this set of answers were last changed.
      # May be different from the lastUpdateTime of the resource itself, because that reflects when the data was known to the server, not when the data was captured.
      # 
      # This element is optional to allow for systems that might not know the value, however it SHOULD be populated if possible.
      'authored' => {
        'type'=>'dateTime',
        'path'=>'QuestionnaireResponse.authored',
        'min'=>0,
        'max'=>1
      },
      ##
      # Person who received and recorded the answers
      # Person who received the answers to the questions in the QuestionnaireResponse and recorded them in the system.
      # Mapping a subject's answers to multiple choice options and determining what to put in the textual answer is a matter of interpretation.  Authoring by device would indicate that some portion of the questionnaire had been auto-populated.
      'author' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'QuestionnaireResponse.author',
        'min'=>0,
        'max'=>1
      },
      ##
      # The person who answered the questions about the subject.
      # If not specified, no inference can be made about who provided the data.
      'source' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson'],
        'type'=>'Reference',
        'path'=>'QuestionnaireResponse.source',
        'min'=>0,
        'max'=>1
      },
      ##
      # Groups and questions
      # A group or question item from the original questionnaire for which answers are provided.
      # Groups cannot have answers and therefore must nest directly within item. When dealing with questions, nesting must occur within each answer because some questions may have multiple answers (and the nesting occurs for each answer).
      'item' => {
        'type'=>'QuestionnaireResponse::Item',
        'path'=>'QuestionnaireResponse.item',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Groups and questions
    # A group or question item from the original questionnaire for which answers are provided.
    # Groups cannot have answers and therefore must nest directly within item. When dealing with questions, nesting must occur within each answer because some questions may have multiple answers (and the nesting occurs for each answer).
    class Item < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Item.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Item.extension',
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
          'path'=>'Item.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Pointer to specific item from Questionnaire
        # The item from the Questionnaire that corresponds to this item in the QuestionnaireResponse resource.
        'linkId' => {
          'type'=>'string',
          'path'=>'Item.linkId',
          'min'=>1,
          'max'=>1
        },
        ##
        # ElementDefinition - details for the item
        # A reference to an [ElementDefinition](elementdefinition.html) that provides the details for the item.
        # The ElementDefinition must be in a [StructureDefinition](structuredefinition.html#), and must have a fragment identifier that identifies the specific data element by its id (Element.id). E.g. http://hl7.org/fhir/StructureDefinition/Observation#Observation.value[x].
        # 
        # There is no need for this element if the item pointed to by the linkId has a definition listed.
        'definition' => {
          'type'=>'uri',
          'path'=>'Item.definition',
          'min'=>0,
          'max'=>1
        },
        ##
        # Name for group or question text
        # Text that is displayed above the contents of the group or as the text of the question being answered.
        'text' => {
          'type'=>'string',
          'path'=>'Item.text',
          'min'=>0,
          'max'=>1
        },
        ##
        # The response(s) to the question
        # The respondent's answer(s) to the question.
        # The value is nested because we cannot have a repeating structure that has variable type.
        'answer' => {
          'type'=>'QuestionnaireResponse::Item::Answer',
          'path'=>'Item.answer',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Nested questionnaire response items
        # Questions or sub-groups nested beneath a question or group.
        'item' => {
          'type'=>'QuestionnaireResponse::Item',
          'path'=>'Item.item',
          'min'=>0,
          'max'=>Float::INFINITY
        }
      }

      ##
      # The response(s) to the question
      # The respondent's answer(s) to the question.
      # The value is nested because we cannot have a repeating structure that has variable type.
      class Answer < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        MULTIPLE_TYPES = {
          'value[x]' => ['Attachment', 'boolean', 'Coding', 'date', 'dateTime', 'decimal', 'integer', 'Quantity', 'Reference', 'string', 'time', 'uri']
        }
        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
            'path'=>'Answer.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Answer.extension',
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
            'path'=>'Answer.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Single-valued answer to the question
          # The answer (or one of the answers) provided by the respondent to the question.
          # More complex structures (Attachment, Resource and Quantity) will typically be limited to electronic forms that can expose an appropriate user interface to capture the components and enforce the constraints of a complex data type.  Additional complex types can be introduced through extensions. Must match the datatype specified by Questionnaire.item.type in the corresponding Questionnaire.
          'valueAttachment' => {
            'type'=>'Attachment',
            'path'=>'Answer.value[x]',
            'min'=>0,
            'max'=>1
          },
          ##
          # Single-valued answer to the question
          # The answer (or one of the answers) provided by the respondent to the question.
          # More complex structures (Attachment, Resource and Quantity) will typically be limited to electronic forms that can expose an appropriate user interface to capture the components and enforce the constraints of a complex data type.  Additional complex types can be introduced through extensions. Must match the datatype specified by Questionnaire.item.type in the corresponding Questionnaire.
          'valueBoolean' => {
            'type'=>'Boolean',
            'path'=>'Answer.value[x]',
            'min'=>0,
            'max'=>1
          },
          ##
          # Single-valued answer to the question
          # The answer (or one of the answers) provided by the respondent to the question.
          # More complex structures (Attachment, Resource and Quantity) will typically be limited to electronic forms that can expose an appropriate user interface to capture the components and enforce the constraints of a complex data type.  Additional complex types can be introduced through extensions. Must match the datatype specified by Questionnaire.item.type in the corresponding Questionnaire.
          'valueCoding' => {
            'type'=>'Coding',
            'path'=>'Answer.value[x]',
            'min'=>0,
            'max'=>1
          },
          ##
          # Single-valued answer to the question
          # The answer (or one of the answers) provided by the respondent to the question.
          # More complex structures (Attachment, Resource and Quantity) will typically be limited to electronic forms that can expose an appropriate user interface to capture the components and enforce the constraints of a complex data type.  Additional complex types can be introduced through extensions. Must match the datatype specified by Questionnaire.item.type in the corresponding Questionnaire.
          'valueDate' => {
            'type'=>'Date',
            'path'=>'Answer.value[x]',
            'min'=>0,
            'max'=>1
          },
          ##
          # Single-valued answer to the question
          # The answer (or one of the answers) provided by the respondent to the question.
          # More complex structures (Attachment, Resource and Quantity) will typically be limited to electronic forms that can expose an appropriate user interface to capture the components and enforce the constraints of a complex data type.  Additional complex types can be introduced through extensions. Must match the datatype specified by Questionnaire.item.type in the corresponding Questionnaire.
          'valueDateTime' => {
            'type'=>'DateTime',
            'path'=>'Answer.value[x]',
            'min'=>0,
            'max'=>1
          },
          ##
          # Single-valued answer to the question
          # The answer (or one of the answers) provided by the respondent to the question.
          # More complex structures (Attachment, Resource and Quantity) will typically be limited to electronic forms that can expose an appropriate user interface to capture the components and enforce the constraints of a complex data type.  Additional complex types can be introduced through extensions. Must match the datatype specified by Questionnaire.item.type in the corresponding Questionnaire.
          'valueDecimal' => {
            'type'=>'Decimal',
            'path'=>'Answer.value[x]',
            'min'=>0,
            'max'=>1
          },
          ##
          # Single-valued answer to the question
          # The answer (or one of the answers) provided by the respondent to the question.
          # More complex structures (Attachment, Resource and Quantity) will typically be limited to electronic forms that can expose an appropriate user interface to capture the components and enforce the constraints of a complex data type.  Additional complex types can be introduced through extensions. Must match the datatype specified by Questionnaire.item.type in the corresponding Questionnaire.
          'valueInteger' => {
            'type'=>'Integer',
            'path'=>'Answer.value[x]',
            'min'=>0,
            'max'=>1
          },
          ##
          # Single-valued answer to the question
          # The answer (or one of the answers) provided by the respondent to the question.
          # More complex structures (Attachment, Resource and Quantity) will typically be limited to electronic forms that can expose an appropriate user interface to capture the components and enforce the constraints of a complex data type.  Additional complex types can be introduced through extensions. Must match the datatype specified by Questionnaire.item.type in the corresponding Questionnaire.
          'valueQuantity' => {
            'type'=>'Quantity',
            'path'=>'Answer.value[x]',
            'min'=>0,
            'max'=>1
          },
          ##
          # Single-valued answer to the question
          # The answer (or one of the answers) provided by the respondent to the question.
          # More complex structures (Attachment, Resource and Quantity) will typically be limited to electronic forms that can expose an appropriate user interface to capture the components and enforce the constraints of a complex data type.  Additional complex types can be introduced through extensions. Must match the datatype specified by Questionnaire.item.type in the corresponding Questionnaire.
          'valueReference' => {
            'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
            'type'=>'Reference',
            'path'=>'Answer.value[x]',
            'min'=>0,
            'max'=>1
          },
          ##
          # Single-valued answer to the question
          # The answer (or one of the answers) provided by the respondent to the question.
          # More complex structures (Attachment, Resource and Quantity) will typically be limited to electronic forms that can expose an appropriate user interface to capture the components and enforce the constraints of a complex data type.  Additional complex types can be introduced through extensions. Must match the datatype specified by Questionnaire.item.type in the corresponding Questionnaire.
          'valueString' => {
            'type'=>'String',
            'path'=>'Answer.value[x]',
            'min'=>0,
            'max'=>1
          },
          ##
          # Single-valued answer to the question
          # The answer (or one of the answers) provided by the respondent to the question.
          # More complex structures (Attachment, Resource and Quantity) will typically be limited to electronic forms that can expose an appropriate user interface to capture the components and enforce the constraints of a complex data type.  Additional complex types can be introduced through extensions. Must match the datatype specified by Questionnaire.item.type in the corresponding Questionnaire.
          'valueTime' => {
            'type'=>'Time',
            'path'=>'Answer.value[x]',
            'min'=>0,
            'max'=>1
          },
          ##
          # Single-valued answer to the question
          # The answer (or one of the answers) provided by the respondent to the question.
          # More complex structures (Attachment, Resource and Quantity) will typically be limited to electronic forms that can expose an appropriate user interface to capture the components and enforce the constraints of a complex data type.  Additional complex types can be introduced through extensions. Must match the datatype specified by Questionnaire.item.type in the corresponding Questionnaire.
          'valueUri' => {
            'type'=>'Uri',
            'path'=>'Answer.value[x]',
            'min'=>0,
            'max'=>1
          },
          ##
          # Nested groups and questions
          # Nested groups and/or questions found within this particular answer.
          'item' => {
            'type'=>'QuestionnaireResponse::Item',
            'path'=>'Answer.item',
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
        # Single-valued answer to the question
        # The answer (or one of the answers) provided by the respondent to the question.
        # More complex structures (Attachment, Resource and Quantity) will typically be limited to electronic forms that can expose an appropriate user interface to capture the components and enforce the constraints of a complex data type.  Additional complex types can be introduced through extensions. Must match the datatype specified by Questionnaire.item.type in the corresponding Questionnaire.
        attr_accessor :valueAttachment                # 0-1 Attachment
        ##
        # Single-valued answer to the question
        # The answer (or one of the answers) provided by the respondent to the question.
        # More complex structures (Attachment, Resource and Quantity) will typically be limited to electronic forms that can expose an appropriate user interface to capture the components and enforce the constraints of a complex data type.  Additional complex types can be introduced through extensions. Must match the datatype specified by Questionnaire.item.type in the corresponding Questionnaire.
        attr_accessor :valueBoolean                   # 0-1 Boolean
        ##
        # Single-valued answer to the question
        # The answer (or one of the answers) provided by the respondent to the question.
        # More complex structures (Attachment, Resource and Quantity) will typically be limited to electronic forms that can expose an appropriate user interface to capture the components and enforce the constraints of a complex data type.  Additional complex types can be introduced through extensions. Must match the datatype specified by Questionnaire.item.type in the corresponding Questionnaire.
        attr_accessor :valueCoding                    # 0-1 Coding
        ##
        # Single-valued answer to the question
        # The answer (or one of the answers) provided by the respondent to the question.
        # More complex structures (Attachment, Resource and Quantity) will typically be limited to electronic forms that can expose an appropriate user interface to capture the components and enforce the constraints of a complex data type.  Additional complex types can be introduced through extensions. Must match the datatype specified by Questionnaire.item.type in the corresponding Questionnaire.
        attr_accessor :valueDate                      # 0-1 Date
        ##
        # Single-valued answer to the question
        # The answer (or one of the answers) provided by the respondent to the question.
        # More complex structures (Attachment, Resource and Quantity) will typically be limited to electronic forms that can expose an appropriate user interface to capture the components and enforce the constraints of a complex data type.  Additional complex types can be introduced through extensions. Must match the datatype specified by Questionnaire.item.type in the corresponding Questionnaire.
        attr_accessor :valueDateTime                  # 0-1 DateTime
        ##
        # Single-valued answer to the question
        # The answer (or one of the answers) provided by the respondent to the question.
        # More complex structures (Attachment, Resource and Quantity) will typically be limited to electronic forms that can expose an appropriate user interface to capture the components and enforce the constraints of a complex data type.  Additional complex types can be introduced through extensions. Must match the datatype specified by Questionnaire.item.type in the corresponding Questionnaire.
        attr_accessor :valueDecimal                   # 0-1 Decimal
        ##
        # Single-valued answer to the question
        # The answer (or one of the answers) provided by the respondent to the question.
        # More complex structures (Attachment, Resource and Quantity) will typically be limited to electronic forms that can expose an appropriate user interface to capture the components and enforce the constraints of a complex data type.  Additional complex types can be introduced through extensions. Must match the datatype specified by Questionnaire.item.type in the corresponding Questionnaire.
        attr_accessor :valueInteger                   # 0-1 Integer
        ##
        # Single-valued answer to the question
        # The answer (or one of the answers) provided by the respondent to the question.
        # More complex structures (Attachment, Resource and Quantity) will typically be limited to electronic forms that can expose an appropriate user interface to capture the components and enforce the constraints of a complex data type.  Additional complex types can be introduced through extensions. Must match the datatype specified by Questionnaire.item.type in the corresponding Questionnaire.
        attr_accessor :valueQuantity                  # 0-1 Quantity
        ##
        # Single-valued answer to the question
        # The answer (or one of the answers) provided by the respondent to the question.
        # More complex structures (Attachment, Resource and Quantity) will typically be limited to electronic forms that can expose an appropriate user interface to capture the components and enforce the constraints of a complex data type.  Additional complex types can be introduced through extensions. Must match the datatype specified by Questionnaire.item.type in the corresponding Questionnaire.
        attr_accessor :valueReference                 # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Resource)
        ##
        # Single-valued answer to the question
        # The answer (or one of the answers) provided by the respondent to the question.
        # More complex structures (Attachment, Resource and Quantity) will typically be limited to electronic forms that can expose an appropriate user interface to capture the components and enforce the constraints of a complex data type.  Additional complex types can be introduced through extensions. Must match the datatype specified by Questionnaire.item.type in the corresponding Questionnaire.
        attr_accessor :valueString                    # 0-1 String
        ##
        # Single-valued answer to the question
        # The answer (or one of the answers) provided by the respondent to the question.
        # More complex structures (Attachment, Resource and Quantity) will typically be limited to electronic forms that can expose an appropriate user interface to capture the components and enforce the constraints of a complex data type.  Additional complex types can be introduced through extensions. Must match the datatype specified by Questionnaire.item.type in the corresponding Questionnaire.
        attr_accessor :valueTime                      # 0-1 Time
        ##
        # Single-valued answer to the question
        # The answer (or one of the answers) provided by the respondent to the question.
        # More complex structures (Attachment, Resource and Quantity) will typically be limited to electronic forms that can expose an appropriate user interface to capture the components and enforce the constraints of a complex data type.  Additional complex types can be introduced through extensions. Must match the datatype specified by Questionnaire.item.type in the corresponding Questionnaire.
        attr_accessor :valueUri                       # 0-1 Uri
        ##
        # Nested groups and questions
        # Nested groups and/or questions found within this particular answer.
        attr_accessor :item                           # 0-* [ QuestionnaireResponse::Item ]
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
      # Pointer to specific item from Questionnaire
      # The item from the Questionnaire that corresponds to this item in the QuestionnaireResponse resource.
      attr_accessor :linkId                         # 1-1 string
      ##
      # ElementDefinition - details for the item
      # A reference to an [ElementDefinition](elementdefinition.html) that provides the details for the item.
      # The ElementDefinition must be in a [StructureDefinition](structuredefinition.html#), and must have a fragment identifier that identifies the specific data element by its id (Element.id). E.g. http://hl7.org/fhir/StructureDefinition/Observation#Observation.value[x].
      # 
      # There is no need for this element if the item pointed to by the linkId has a definition listed.
      attr_accessor :definition                     # 0-1 uri
      ##
      # Name for group or question text
      # Text that is displayed above the contents of the group or as the text of the question being answered.
      attr_accessor :text                           # 0-1 string
      ##
      # The response(s) to the question
      # The respondent's answer(s) to the question.
      # The value is nested because we cannot have a repeating structure that has variable type.
      attr_accessor :answer                         # 0-* [ QuestionnaireResponse::Item::Answer ]
      ##
      # Nested questionnaire response items
      # Questions or sub-groups nested beneath a question or group.
      attr_accessor :item                           # 0-* [ QuestionnaireResponse::Item ]
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
    # Unique id for this set of answers
    # A business identifier assigned to a particular completed (or partially completed) questionnaire.
    attr_accessor :identifier                     # 0-1 Identifier
    ##
    # Request fulfilled by this QuestionnaireResponse
    # The order, proposal or plan that is fulfilled in whole or in part by this QuestionnaireResponse.  For example, a ServiceRequest seeking an intake assessment or a decision support recommendation to assess for post-partum depression.
    attr_accessor :basedOn                        # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/CarePlan|http://hl7.org/fhir/StructureDefinition/ServiceRequest) ]
    ##
    # Part of this action
    # A procedure or observation that this questionnaire was performed as part of the execution of.  For example, the surgery a checklist was executed as part of.
    # Composition of questionnaire responses will be handled by the parent questionnaire having answers that reference the child questionnaire.  For relationships to referrals, and other types of requests, use basedOn.
    attr_accessor :partOf                         # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Observation|http://hl7.org/fhir/StructureDefinition/Procedure) ]
    ##
    # Form being answered
    # The Questionnaire that defines and organizes the questions for which answers are being provided.
    # If a QuestionnaireResponse references a Questionnaire, then the QuestionnaireResponse structure must be consistent with the Questionnaire (i.e. questions must be organized into the same groups, nested questions must still be nested, etc.).
    attr_accessor :questionnaire                  # 0-1 canonical(http://hl7.org/fhir/StructureDefinition/Questionnaire)
    ##
    # in-progress | completed | amended | entered-in-error | stopped
    # The position of the questionnaire response within its overall lifecycle.
    # This element is labeled as a modifier because the status contains codes that mark the resource as not currently valid.
    attr_accessor :status                         # 1-1 code
    ##
    # The subject of the questions
    # The subject of the questionnaire response.  This could be a patient, organization, practitioner, device, etc.  This is who/what the answers apply to, but is not necessarily the source of information.
    # If the Questionnaire declared a subjectType, the resource pointed to by this element must be an instance of one of the listed types.
    attr_accessor :subject                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Resource)
    ##
    # Encounter created as part of
    # The Encounter during which this questionnaire response was created or to which the creation of this record is tightly associated.
    # This will typically be the encounter the event occurred within, but some activities may be initiated prior to or after the official completion of an encounter but still be tied to the context of the encounter. A questionnaire that was initiated during an encounter but not fully completed during the encounter would still generally be associated with the encounter.
    attr_accessor :encounter                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Encounter)
    ##
    # Date the answers were gathered
    # The date and/or time that this set of answers were last changed.
    # May be different from the lastUpdateTime of the resource itself, because that reflects when the data was known to the server, not when the data was captured.
    # 
    # This element is optional to allow for systems that might not know the value, however it SHOULD be populated if possible.
    attr_accessor :authored                       # 0-1 dateTime
    ##
    # Person who received and recorded the answers
    # Person who received the answers to the questions in the QuestionnaireResponse and recorded them in the system.
    # Mapping a subject's answers to multiple choice options and determining what to put in the textual answer is a matter of interpretation.  Authoring by device would indicate that some portion of the questionnaire had been auto-populated.
    attr_accessor :author                         # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # The person who answered the questions about the subject.
    # If not specified, no inference can be made about who provided the data.
    attr_accessor :source                         # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/RelatedPerson)
    ##
    # Groups and questions
    # A group or question item from the original questionnaire for which answers are provided.
    # Groups cannot have answers and therefore must nest directly within item. When dealing with questions, nesting must occur within each answer because some questions may have multiple answers (and the nesting occurs for each answer).
    attr_accessor :item                           # 0-* [ QuestionnaireResponse::Item ]

    def resourceType
      'QuestionnaireResponse'
    end
  end
end
