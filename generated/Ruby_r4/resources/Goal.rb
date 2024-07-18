module FHIR

  ##
  # Describes the intended objective(s) for a patient, group or organization care, for example, weight loss, restoring an activity of daily living, obtaining herd immunity via immunization, meeting a process improvement objective, etc.
  class Goal < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['achievement-status', 'category', 'identifier', 'lifecycle-status', 'patient', 'start-date', 'subject', 'target-date']
    MULTIPLE_TYPES = {
      'start[x]' => ['CodeableConcept', 'date']
    }
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'Goal.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'Goal.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'Goal.implicitRules',
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
        'path'=>'Goal.language',
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
        'path'=>'Goal.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'Goal.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Goal.extension',
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
        'path'=>'Goal.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # External Ids for this goal
      # Business identifiers assigned to this goal by the performer or other systems which remain constant as the resource is updated and propagates from server to server.
      # This is a business identifier, not a resource identifier (see [discussion](resource.html#identifiers)).  It is best practice for the identifier to only appear on a single resource instance, however business practices may occasionally dictate that multiple resource instances with the same identifier can exist - possibly even with different resource types.  For example, multiple Patient and a Person resource instance might share the same social insurance number.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'Goal.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # proposed | planned | accepted | active | on-hold | completed | cancelled | entered-in-error | rejected
      # The state of the goal throughout its lifecycle.
      # This element is labeled as a modifier because the lifecycleStatus contains codes that mark the resource as not currently valid.
      'lifecycleStatus' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/goal-status'=>[ 'proposed', 'planned', 'accepted', 'active', 'on-hold', 'completed', 'cancelled', 'entered-in-error', 'rejected' ]
        },
        'type'=>'code',
        'path'=>'Goal.lifecycleStatus',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/goal-status'}
      },
      ##
      # in-progress | improving | worsening | no-change | achieved | sustaining | not-achieved | no-progress | not-attainable
      # Describes the progression, or lack thereof, towards the goal against the target.
      'achievementStatus' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/goal-achievement'=>[ 'in-progress', 'improving', 'worsening', 'no-change', 'achieved', 'sustaining', 'not-achieved', 'no-progress', 'not-attainable' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Goal.achievementStatus',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/goal-achievement'}
      },
      ##
      # E.g. Treatment, dietary, behavioral, etc.
      # Indicates a category the goal falls within.
      'category' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/goal-category'=>[ 'dietary', 'safety', 'behavioral', 'nursing', 'physiotherapy' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Goal.category',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/goal-category'}
      },
      ##
      # high-priority | medium-priority | low-priority
      # Identifies the mutually agreed level of importance associated with reaching/sustaining the goal.
      # Extensions are available to track priorities as established by each participant (i.e. Priority from the patient's perspective, different practitioners' perspectives, family member's perspectives)The ordinal extension on Coding can be used to convey a numerically comparable ranking to priority.  (Keep in mind that different coding systems may use a "low value=important".
      'priority' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/goal-priority'=>[ 'high-priority', 'medium-priority', 'low-priority' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Goal.priority',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/goal-priority'}
      },
      ##
      # Code or text describing goal
      # Human-readable and/or coded description of a specific desired objective of care, such as "control blood pressure" or "negotiate an obstacle course" or "dance with child at wedding".
      # If no code is available, use CodeableConcept.text.
      'description' => {
        'type'=>'CodeableConcept',
        'path'=>'Goal.description',
        'min'=>1,
        'max'=>1
      },
      ##
      # Who this goal is intended for
      # Identifies the patient, group or organization for whom the goal is being established.
      'subject' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Group', 'http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'Goal.subject',
        'min'=>1,
        'max'=>1
      },
      ##
      # When goal pursuit begins
      # The date or event after which the goal should begin being pursued.
      'startCodeableConcept' => {
        'type'=>'CodeableConcept',
        'path'=>'Goal.start[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # When goal pursuit begins
      # The date or event after which the goal should begin being pursued.
      'startDate' => {
        'type'=>'Date',
        'path'=>'Goal.start[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Target outcome for the goal
      # Indicates what should be done by when.
      # When multiple targets are present for a single goal instance, all targets must be met for the overall goal to be met.
      'target' => {
        'type'=>'Goal::Target',
        'path'=>'Goal.target',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # When goal status took effect
      # Identifies when the current status.  I.e. When initially created, when achieved, when cancelled, etc.
      # To see the date for past statuses, query history.
      'statusDate' => {
        'type'=>'date',
        'path'=>'Goal.statusDate',
        'min'=>0,
        'max'=>1
      },
      ##
      # Reason for current status
      # Captures the reason for the current status.
      # This will typically be captured for statuses such as rejected, on-hold or cancelled, but could be present for others.
      'statusReason' => {
        'type'=>'string',
        'path'=>'Goal.statusReason',
        'min'=>0,
        'max'=>1
      },
      ##
      # Who's responsible for creating Goal?
      # Indicates whose goal this is - patient goal, practitioner goal, etc.
      # This is the individual responsible for establishing the goal, not necessarily who recorded it.  (For that, use the Provenance resource.).
      'expressedBy' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson'],
        'type'=>'Reference',
        'path'=>'Goal.expressedBy',
        'min'=>0,
        'max'=>1
      },
      ##
      # Issues addressed by this goal
      # The identified conditions and other health record elements that are intended to be addressed by the goal.
      'addresses' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Condition', 'http://hl7.org/fhir/StructureDefinition/Observation', 'http://hl7.org/fhir/StructureDefinition/MedicationStatement', 'http://hl7.org/fhir/StructureDefinition/NutritionOrder', 'http://hl7.org/fhir/StructureDefinition/ServiceRequest', 'http://hl7.org/fhir/StructureDefinition/RiskAssessment'],
        'type'=>'Reference',
        'path'=>'Goal.addresses',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Comments about the goal
      # Any comments related to the goal.
      # May be used for progress notes, concerns or other related information that doesn't actually describe the goal itself.
      'note' => {
        'type'=>'Annotation',
        'path'=>'Goal.note',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # What result was achieved regarding the goal?
      # Identifies the change (or lack of change) at the point when the status of the goal is assessed.
      # Note that this should not duplicate the goal status.
      'outcomeCode' => {
        'type'=>'CodeableConcept',
        'path'=>'Goal.outcomeCode',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Observation that resulted from goal
      # Details of what's changed (or not changed).
      # The goal outcome is independent of the outcome of the related activities.  For example, if the Goal is to achieve a target body weight of 150 lb and a care plan activity is defined to diet, then the care plan’s activity outcome could be calories consumed whereas goal outcome is an observation for the actual body weight measured.
      'outcomeReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Observation'],
        'type'=>'Reference',
        'path'=>'Goal.outcomeReference',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Target outcome for the goal
    # Indicates what should be done by when.
    # When multiple targets are present for a single goal instance, all targets must be met for the overall goal to be met.
    class Target < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'detail[x]' => ['boolean', 'CodeableConcept', 'integer', 'Quantity', 'Range', 'Ratio', 'string'],
        'due[x]' => ['date', 'Duration']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Target.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Target.extension',
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
          'path'=>'Target.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # The parameter whose value is being tracked, e.g. body weight, blood pressure, or hemoglobin A1c level.
        'measure' => {
          'type'=>'CodeableConcept',
          'path'=>'Target.measure',
          'min'=>0,
          'max'=>1
        },
        ##
        # The target value to be achieved
        # The target value of the focus to be achieved to signify the fulfillment of the goal, e.g. 150 pounds, 7.0%. Either the high or low or both values of the range can be specified. When a low value is missing, it indicates that the goal is achieved at any focus value at or below the high value. Similarly, if the high value is missing, it indicates that the goal is achieved at any focus value at or above the low value.
        # A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Goal.target.measure defines a coded value.
        'detailBoolean' => {
          'type'=>'Boolean',
          'path'=>'Target.detail[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # The target value to be achieved
        # The target value of the focus to be achieved to signify the fulfillment of the goal, e.g. 150 pounds, 7.0%. Either the high or low or both values of the range can be specified. When a low value is missing, it indicates that the goal is achieved at any focus value at or below the high value. Similarly, if the high value is missing, it indicates that the goal is achieved at any focus value at or above the low value.
        # A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Goal.target.measure defines a coded value.
        'detailCodeableConcept' => {
          'type'=>'CodeableConcept',
          'path'=>'Target.detail[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # The target value to be achieved
        # The target value of the focus to be achieved to signify the fulfillment of the goal, e.g. 150 pounds, 7.0%. Either the high or low or both values of the range can be specified. When a low value is missing, it indicates that the goal is achieved at any focus value at or below the high value. Similarly, if the high value is missing, it indicates that the goal is achieved at any focus value at or above the low value.
        # A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Goal.target.measure defines a coded value.
        'detailInteger' => {
          'type'=>'Integer',
          'path'=>'Target.detail[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # The target value to be achieved
        # The target value of the focus to be achieved to signify the fulfillment of the goal, e.g. 150 pounds, 7.0%. Either the high or low or both values of the range can be specified. When a low value is missing, it indicates that the goal is achieved at any focus value at or below the high value. Similarly, if the high value is missing, it indicates that the goal is achieved at any focus value at or above the low value.
        # A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Goal.target.measure defines a coded value.
        'detailQuantity' => {
          'type'=>'Quantity',
          'path'=>'Target.detail[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # The target value to be achieved
        # The target value of the focus to be achieved to signify the fulfillment of the goal, e.g. 150 pounds, 7.0%. Either the high or low or both values of the range can be specified. When a low value is missing, it indicates that the goal is achieved at any focus value at or below the high value. Similarly, if the high value is missing, it indicates that the goal is achieved at any focus value at or above the low value.
        # A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Goal.target.measure defines a coded value.
        'detailRange' => {
          'type'=>'Range',
          'path'=>'Target.detail[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # The target value to be achieved
        # The target value of the focus to be achieved to signify the fulfillment of the goal, e.g. 150 pounds, 7.0%. Either the high or low or both values of the range can be specified. When a low value is missing, it indicates that the goal is achieved at any focus value at or below the high value. Similarly, if the high value is missing, it indicates that the goal is achieved at any focus value at or above the low value.
        # A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Goal.target.measure defines a coded value.
        'detailRatio' => {
          'type'=>'Ratio',
          'path'=>'Target.detail[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # The target value to be achieved
        # The target value of the focus to be achieved to signify the fulfillment of the goal, e.g. 150 pounds, 7.0%. Either the high or low or both values of the range can be specified. When a low value is missing, it indicates that the goal is achieved at any focus value at or below the high value. Similarly, if the high value is missing, it indicates that the goal is achieved at any focus value at or above the low value.
        # A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Goal.target.measure defines a coded value.
        'detailString' => {
          'type'=>'String',
          'path'=>'Target.detail[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Reach goal on or before
        # Indicates either the date or the duration after start by which the goal should be met.
        'dueDate' => {
          'type'=>'Date',
          'path'=>'Target.due[x]',
          'min'=>0,
          'max'=>1
        }
        ##
        # Reach goal on or before
        # Indicates either the date or the duration after start by which the goal should be met.
        'dueDuration' => {
          'type'=>'Duration',
          'path'=>'Target.due[x]',
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
      # The parameter whose value is being tracked, e.g. body weight, blood pressure, or hemoglobin A1c level.
      attr_accessor :measure                        # 0-1 CodeableConcept
      ##
      # The target value to be achieved
      # The target value of the focus to be achieved to signify the fulfillment of the goal, e.g. 150 pounds, 7.0%. Either the high or low or both values of the range can be specified. When a low value is missing, it indicates that the goal is achieved at any focus value at or below the high value. Similarly, if the high value is missing, it indicates that the goal is achieved at any focus value at or above the low value.
      # A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Goal.target.measure defines a coded value.
      attr_accessor :detailBoolean                  # 0-1 Boolean
      ##
      # The target value to be achieved
      # The target value of the focus to be achieved to signify the fulfillment of the goal, e.g. 150 pounds, 7.0%. Either the high or low or both values of the range can be specified. When a low value is missing, it indicates that the goal is achieved at any focus value at or below the high value. Similarly, if the high value is missing, it indicates that the goal is achieved at any focus value at or above the low value.
      # A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Goal.target.measure defines a coded value.
      attr_accessor :detailCodeableConcept          # 0-1 CodeableConcept
      ##
      # The target value to be achieved
      # The target value of the focus to be achieved to signify the fulfillment of the goal, e.g. 150 pounds, 7.0%. Either the high or low or both values of the range can be specified. When a low value is missing, it indicates that the goal is achieved at any focus value at or below the high value. Similarly, if the high value is missing, it indicates that the goal is achieved at any focus value at or above the low value.
      # A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Goal.target.measure defines a coded value.
      attr_accessor :detailInteger                  # 0-1 Integer
      ##
      # The target value to be achieved
      # The target value of the focus to be achieved to signify the fulfillment of the goal, e.g. 150 pounds, 7.0%. Either the high or low or both values of the range can be specified. When a low value is missing, it indicates that the goal is achieved at any focus value at or below the high value. Similarly, if the high value is missing, it indicates that the goal is achieved at any focus value at or above the low value.
      # A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Goal.target.measure defines a coded value.
      attr_accessor :detailQuantity                 # 0-1 Quantity
      ##
      # The target value to be achieved
      # The target value of the focus to be achieved to signify the fulfillment of the goal, e.g. 150 pounds, 7.0%. Either the high or low or both values of the range can be specified. When a low value is missing, it indicates that the goal is achieved at any focus value at or below the high value. Similarly, if the high value is missing, it indicates that the goal is achieved at any focus value at or above the low value.
      # A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Goal.target.measure defines a coded value.
      attr_accessor :detailRange                    # 0-1 Range
      ##
      # The target value to be achieved
      # The target value of the focus to be achieved to signify the fulfillment of the goal, e.g. 150 pounds, 7.0%. Either the high or low or both values of the range can be specified. When a low value is missing, it indicates that the goal is achieved at any focus value at or below the high value. Similarly, if the high value is missing, it indicates that the goal is achieved at any focus value at or above the low value.
      # A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Goal.target.measure defines a coded value.
      attr_accessor :detailRatio                    # 0-1 Ratio
      ##
      # The target value to be achieved
      # The target value of the focus to be achieved to signify the fulfillment of the goal, e.g. 150 pounds, 7.0%. Either the high or low or both values of the range can be specified. When a low value is missing, it indicates that the goal is achieved at any focus value at or below the high value. Similarly, if the high value is missing, it indicates that the goal is achieved at any focus value at or above the low value.
      # A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Goal.target.measure defines a coded value.
      attr_accessor :detailString                   # 0-1 String
      ##
      # Reach goal on or before
      # Indicates either the date or the duration after start by which the goal should be met.
      attr_accessor :dueDate                        # 0-1 Date
      ##
      # Reach goal on or before
      # Indicates either the date or the duration after start by which the goal should be met.
      attr_accessor :dueDuration                    # 0-1 Duration
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
    # External Ids for this goal
    # Business identifiers assigned to this goal by the performer or other systems which remain constant as the resource is updated and propagates from server to server.
    # This is a business identifier, not a resource identifier (see [discussion](resource.html#identifiers)).  It is best practice for the identifier to only appear on a single resource instance, however business practices may occasionally dictate that multiple resource instances with the same identifier can exist - possibly even with different resource types.  For example, multiple Patient and a Person resource instance might share the same social insurance number.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # proposed | planned | accepted | active | on-hold | completed | cancelled | entered-in-error | rejected
    # The state of the goal throughout its lifecycle.
    # This element is labeled as a modifier because the lifecycleStatus contains codes that mark the resource as not currently valid.
    attr_accessor :lifecycleStatus                # 1-1 code
    ##
    # in-progress | improving | worsening | no-change | achieved | sustaining | not-achieved | no-progress | not-attainable
    # Describes the progression, or lack thereof, towards the goal against the target.
    attr_accessor :achievementStatus              # 0-1 CodeableConcept
    ##
    # E.g. Treatment, dietary, behavioral, etc.
    # Indicates a category the goal falls within.
    attr_accessor :category                       # 0-* [ CodeableConcept ]
    ##
    # high-priority | medium-priority | low-priority
    # Identifies the mutually agreed level of importance associated with reaching/sustaining the goal.
    # Extensions are available to track priorities as established by each participant (i.e. Priority from the patient's perspective, different practitioners' perspectives, family member's perspectives)The ordinal extension on Coding can be used to convey a numerically comparable ranking to priority.  (Keep in mind that different coding systems may use a "low value=important".
    attr_accessor :priority                       # 0-1 CodeableConcept
    ##
    # Code or text describing goal
    # Human-readable and/or coded description of a specific desired objective of care, such as "control blood pressure" or "negotiate an obstacle course" or "dance with child at wedding".
    # If no code is available, use CodeableConcept.text.
    attr_accessor :description                    # 1-1 CodeableConcept
    ##
    # Who this goal is intended for
    # Identifies the patient, group or organization for whom the goal is being established.
    attr_accessor :subject                        # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Group|http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # When goal pursuit begins
    # The date or event after which the goal should begin being pursued.
    attr_accessor :startCodeableConcept           # 0-1 CodeableConcept
    ##
    # When goal pursuit begins
    # The date or event after which the goal should begin being pursued.
    attr_accessor :startDate                      # 0-1 Date
    ##
    # Target outcome for the goal
    # Indicates what should be done by when.
    # When multiple targets are present for a single goal instance, all targets must be met for the overall goal to be met.
    attr_accessor :target                         # 0-* [ Goal::Target ]
    ##
    # When goal status took effect
    # Identifies when the current status.  I.e. When initially created, when achieved, when cancelled, etc.
    # To see the date for past statuses, query history.
    attr_accessor :statusDate                     # 0-1 date
    ##
    # Reason for current status
    # Captures the reason for the current status.
    # This will typically be captured for statuses such as rejected, on-hold or cancelled, but could be present for others.
    attr_accessor :statusReason                   # 0-1 string
    ##
    # Who's responsible for creating Goal?
    # Indicates whose goal this is - patient goal, practitioner goal, etc.
    # This is the individual responsible for establishing the goal, not necessarily who recorded it.  (For that, use the Provenance resource.).
    attr_accessor :expressedBy                    # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/RelatedPerson)
    ##
    # Issues addressed by this goal
    # The identified conditions and other health record elements that are intended to be addressed by the goal.
    attr_accessor :addresses                      # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Condition|http://hl7.org/fhir/StructureDefinition/Observation|http://hl7.org/fhir/StructureDefinition/MedicationStatement|http://hl7.org/fhir/StructureDefinition/NutritionOrder|http://hl7.org/fhir/StructureDefinition/ServiceRequest|http://hl7.org/fhir/StructureDefinition/RiskAssessment) ]
    ##
    # Comments about the goal
    # Any comments related to the goal.
    # May be used for progress notes, concerns or other related information that doesn't actually describe the goal itself.
    attr_accessor :note                           # 0-* [ Annotation ]
    ##
    # What result was achieved regarding the goal?
    # Identifies the change (or lack of change) at the point when the status of the goal is assessed.
    # Note that this should not duplicate the goal status.
    attr_accessor :outcomeCode                    # 0-* [ CodeableConcept ]
    ##
    # Observation that resulted from goal
    # Details of what's changed (or not changed).
    # The goal outcome is independent of the outcome of the related activities.  For example, if the Goal is to achieve a target body weight of 150 lb and a care plan activity is defined to diet, then the care plan’s activity outcome could be calories consumed whereas goal outcome is an observation for the actual body weight measured.
    attr_accessor :outcomeReference               # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Observation) ]

    def resourceType
      'Goal'
    end
  end
end
