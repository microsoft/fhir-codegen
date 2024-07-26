module FHIR

  ##
  # Describes the intention of how one or more practitioners intend to deliver care for a particular patient, group or community for a period of time, possibly limited to care for a specific condition or set of conditions.
  class CarePlan < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['activity-code', 'activity-date', 'activity-reference', 'based-on', 'care-team', 'category', 'condition', 'date', 'encounter', 'goal', 'identifier', 'instantiates-canonical', 'instantiates-uri', 'intent', 'part-of', 'patient', 'performer', 'replaces', 'status', 'subject']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'CarePlan.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'CarePlan.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'CarePlan.implicitRules',
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
        'path'=>'CarePlan.language',
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
        'path'=>'CarePlan.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'CarePlan.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'CarePlan.extension',
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
        'path'=>'CarePlan.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # External Ids for this plan
      # Business identifiers assigned to this care plan by the performer or other systems which remain constant as the resource is updated and propagates from server to server.
      # This is a business identifier, not a resource identifier (see [discussion](resource.html#identifiers)).  It is best practice for the identifier to only appear on a single resource instance, however business practices may occasionally dictate that multiple resource instances with the same identifier can exist - possibly even with different resource types.  For example, multiple Patient and a Person resource instance might share the same social insurance number.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'CarePlan.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Instantiates FHIR protocol or definition
      # The URL pointing to a FHIR-defined protocol, guideline, questionnaire or other definition that is adhered to in whole or in part by this CarePlan.
      'instantiatesCanonical' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/PlanDefinition', 'http://hl7.org/fhir/StructureDefinition/Questionnaire', 'http://hl7.org/fhir/StructureDefinition/Measure', 'http://hl7.org/fhir/StructureDefinition/ActivityDefinition', 'http://hl7.org/fhir/StructureDefinition/OperationDefinition'],
        'type'=>'canonical',
        'path'=>'CarePlan.instantiatesCanonical',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Instantiates external protocol or definition
      # The URL pointing to an externally maintained protocol, guideline, questionnaire or other definition that is adhered to in whole or in part by this CarePlan.
      # This might be an HTML page, PDF, etc. or could just be a non-resolvable URI identifier.
      'instantiatesUri' => {
        'type'=>'uri',
        'path'=>'CarePlan.instantiatesUri',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Fulfills CarePlan
      # A care plan that is fulfilled in whole or in part by this care plan.
      'basedOn' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/CarePlan'],
        'type'=>'Reference',
        'path'=>'CarePlan.basedOn',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # CarePlan replaced by this CarePlan
      # Completed or terminated care plan whose function is taken by this new care plan.
      # The replacement could be because the initial care plan was immediately rejected (due to an issue) or because the previous care plan was completed, but the need for the action described by the care plan remains ongoing.
      'replaces' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/CarePlan'],
        'type'=>'Reference',
        'path'=>'CarePlan.replaces',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Part of referenced CarePlan
      # A larger care plan of which this particular care plan is a component or step.
      # Each care plan is an independent request, such that having a care plan be part of another care plan can cause issues with cascading statuses.  As such, this element is still being discussed.
      'partOf' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/CarePlan'],
        'type'=>'Reference',
        'path'=>'CarePlan.partOf',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # draft | active | on-hold | revoked | completed | entered-in-error | unknown
      # Indicates whether the plan is currently being acted upon, represents future intentions or is now a historical record.
      # The unknown code is not to be used to convey other statuses.  The unknown code should be used when one of the statuses applies, but the authoring system doesn't know the current state of the care plan.
      # 
      # This element is labeled as a modifier because the status contains the code entered-in-error that marks the plan as not currently valid.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/request-status'=>[ 'draft', 'active', 'on-hold', 'revoked', 'completed', 'entered-in-error', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'CarePlan.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/request-status'}
      },
      ##
      # proposal | plan | order | option
      # Indicates the level of authority/intentionality associated with the care plan and where the care plan fits into the workflow chain.
      # This element is labeled as a modifier because the intent alters when and how the resource is actually applicable.
      'intent' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/request-intent'=>[ 'proposal', 'plan', 'order', 'option' ]
        },
        'type'=>'code',
        'path'=>'CarePlan.intent',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/care-plan-intent'}
      },
      ##
      # Type of plan
      # Identifies what "kind" of plan this is to support differentiation between multiple co-existing plans; e.g. "Home health", "psychiatric", "asthma", "disease management", "wellness plan", etc.
      # There may be multiple axes of categorization and one plan may serve multiple purposes.  In some cases, this may be redundant with references to CarePlan.concern.
      'category' => {
        'type'=>'CodeableConcept',
        'path'=>'CarePlan.category',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Human-friendly name for the care plan.
      'title' => {
        'type'=>'string',
        'path'=>'CarePlan.title',
        'min'=>0,
        'max'=>1
      },
      ##
      # Summary of nature of plan
      # A description of the scope and nature of the plan.
      'description' => {
        'type'=>'string',
        'path'=>'CarePlan.description',
        'min'=>0,
        'max'=>1
      },
      ##
      # Who the care plan is for
      # Identifies the patient or group whose intended care is described by the plan.
      'subject' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Group'],
        'type'=>'Reference',
        'path'=>'CarePlan.subject',
        'min'=>1,
        'max'=>1
      },
      ##
      # Encounter created as part of
      # The Encounter during which this CarePlan was created or to which the creation of this record is tightly associated.
      # This will typically be the encounter the event occurred within, but some activities may be initiated prior to or after the official completion of an encounter but still be tied to the context of the encounter. CarePlan activities conducted as a result of the care plan may well occur as part of other encounters.
      'encounter' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Encounter'],
        'type'=>'Reference',
        'path'=>'CarePlan.encounter',
        'min'=>0,
        'max'=>1
      },
      ##
      # Time period plan covers
      # Indicates when the plan did (or is intended to) come into effect and end.
      # Any activities scheduled as part of the plan should be constrained to the specified period regardless of whether the activities are planned within a single encounter/episode or across multiple encounters/episodes (e.g. the longitudinal management of a chronic condition).
      'period' => {
        'type'=>'Period',
        'path'=>'CarePlan.period',
        'min'=>0,
        'max'=>1
      },
      ##
      # Date record was first recorded
      # Represents when this particular CarePlan record was created in the system, which is often a system-generated date.
      'created' => {
        'type'=>'dateTime',
        'path'=>'CarePlan.created',
        'min'=>0,
        'max'=>1
      },
      ##
      # Who is the designated responsible party
      # When populated, the author is responsible for the care plan.  The care plan is attributed to the author.
      # The author may also be a contributor.  For example, an organization can be an author, but not listed as a contributor.
      'author' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/CareTeam'],
        'type'=>'Reference',
        'path'=>'CarePlan.author',
        'min'=>0,
        'max'=>1
      },
      ##
      # Who provided the content of the care plan
      # Identifies the individual(s) or organization who provided the contents of the care plan.
      # Collaborative care plans may have multiple contributors.
      'contributor' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/CareTeam'],
        'type'=>'Reference',
        'path'=>'CarePlan.contributor',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Who's involved in plan?
      # Identifies all people and organizations who are expected to be involved in the care envisioned by this plan.
      'careTeam' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/CareTeam'],
        'type'=>'Reference',
        'path'=>'CarePlan.careTeam',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Health issues this plan addresses
      # Identifies the conditions/problems/concerns/diagnoses/etc. whose management and/or mitigation are handled by this plan.
      # When the diagnosis is related to an allergy or intolerance, the Condition and AllergyIntolerance resources can both be used. However, to be actionable for decision support, using Condition alone is not sufficient as the allergy or intolerance condition needs to be represented as an AllergyIntolerance.
      'addresses' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Condition'],
        'type'=>'Reference',
        'path'=>'CarePlan.addresses',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Information considered as part of plan
      # Identifies portions of the patient's record that specifically influenced the formation of the plan.  These might include comorbidities, recent procedures, limitations, recent assessments, etc.
      # Use "concern" to identify specific conditions addressed by the care plan.
      'supportingInfo' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'Reference',
        'path'=>'CarePlan.supportingInfo',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Desired outcome of plan
      # Describes the intended objective(s) of carrying out the care plan.
      # Goal can be achieving a particular change or merely maintaining a current state or even slowing a decline.
      'goal' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Goal'],
        'type'=>'Reference',
        'path'=>'CarePlan.goal',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Action to occur as part of plan
      # Identifies a planned action to occur as part of the plan.  For example, a medication to be used, lab tests to perform, self-monitoring, education, etc.
      'activity' => {
        'type'=>'CarePlan::Activity',
        'path'=>'CarePlan.activity',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Comments about the plan
      # General notes about the care plan not covered elsewhere.
      'note' => {
        'type'=>'Annotation',
        'path'=>'CarePlan.note',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Action to occur as part of plan
    # Identifies a planned action to occur as part of the plan.  For example, a medication to be used, lab tests to perform, self-monitoring, education, etc.
    class Activity < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Activity.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Activity.extension',
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
          'path'=>'Activity.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Results of the activity
        # Identifies the outcome at the point when the status of the activity is assessed.  For example, the outcome of an education activity could be patient understands (or not).
        # Note that this should not duplicate the activity status (e.g. completed or in progress).
        'outcomeCodeableConcept' => {
          'type'=>'CodeableConcept',
          'path'=>'Activity.outcomeCodeableConcept',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Appointment, Encounter, Procedure, etc.
        # Details of the outcome or action resulting from the activity.  The reference to an "event" resource, such as Procedure or Encounter or Observation, is the result/outcome of the activity itself.  The activity can be conveyed using CarePlan.activity.detail OR using the CarePlan.activity.reference (a reference to a “request” resource).
        # The activity outcome is independent of the outcome of the related goal(s).  For example, if the goal is to achieve a target body weight of 150 lbs and an activity is defined to diet, then the activity outcome could be calories consumed whereas the goal outcome is an observation for the actual body weight measured.
        'outcomeReference' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
          'type'=>'Reference',
          'path'=>'Activity.outcomeReference',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Comments about the activity status/progress
        # Notes about the adherence/status/progress of the activity.
        # This element should NOT be used to describe the activity to be performed - that occurs either within the resource pointed to by activity.detail.reference or in activity.detail.description.
        'progress' => {
          'type'=>'Annotation',
          'path'=>'Activity.progress',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Activity details defined in specific resource
        # The details of the proposed activity represented in a specific resource.
        # Standard extension exists ([resource-pertainsToGoal](extension-resource-pertainstogoal.html)) that allows goals to be referenced from any of the referenced resources in CarePlan.activity.reference.  The goal should be visible when the resource referenced by CarePlan.activity.reference is viewed independently from the CarePlan.  Requests that are pointed to by a CarePlan using this element should *not* point to this CarePlan using the "basedOn" element.  i.e. Requests that are part of a CarePlan are not "based on" the CarePlan.
        'reference' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Appointment', 'http://hl7.org/fhir/StructureDefinition/CommunicationRequest', 'http://hl7.org/fhir/StructureDefinition/DeviceRequest', 'http://hl7.org/fhir/StructureDefinition/MedicationRequest', 'http://hl7.org/fhir/StructureDefinition/NutritionOrder', 'http://hl7.org/fhir/StructureDefinition/Task', 'http://hl7.org/fhir/StructureDefinition/ServiceRequest', 'http://hl7.org/fhir/StructureDefinition/VisionPrescription', 'http://hl7.org/fhir/StructureDefinition/RequestGroup'],
          'type'=>'Reference',
          'path'=>'Activity.reference',
          'min'=>0,
          'max'=>1
        },
        ##
        # In-line definition of activity
        # A simple summary of a planned activity suitable for a general care plan system (e.g. form driven) that doesn't know about specific resources such as procedure etc.
        'detail' => {
          'type'=>'CarePlan::Activity::Detail',
          'path'=>'Activity.detail',
          'min'=>0,
          'max'=>1
        }
      }

      ##
      # In-line definition of activity
      # A simple summary of a planned activity suitable for a general care plan system (e.g. form driven) that doesn't know about specific resources such as procedure etc.
      class Detail < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        MULTIPLE_TYPES = {
          'scheduled[x]' => ['Period', 'string', 'Timing'],
          'product[x]' => ['CodeableConcept', 'Reference']
        }
        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'id',
            'path'=>'Detail.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Detail.extension',
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
            'path'=>'Detail.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Appointment | CommunicationRequest | DeviceRequest | MedicationRequest | NutritionOrder | Task | ServiceRequest | VisionPrescription
          # A description of the kind of resource the in-line definition of a care plan activity is representing.  The CarePlan.activity.detail is an in-line definition when a resource is not referenced using CarePlan.activity.reference.  For example, a MedicationRequest, a ServiceRequest, or a CommunicationRequest.
          'kind' => {
            'valid_codes'=>{
              'http://hl7.org/fhir/resource-types'=>[ 'Appointment', 'CommunicationRequest', 'DeviceRequest', 'MedicationRequest', 'NutritionOrder', 'Task', 'ServiceRequest', 'VisionPrescription' ]
            },
            'type'=>'code',
            'path'=>'Detail.kind',
            'min'=>0,
            'max'=>1,
            'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/care-plan-activity-kind'}
          },
          ##
          # Instantiates FHIR protocol or definition
          # The URL pointing to a FHIR-defined protocol, guideline, questionnaire or other definition that is adhered to in whole or in part by this CarePlan activity.
          'instantiatesCanonical' => {
            'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/PlanDefinition', 'http://hl7.org/fhir/StructureDefinition/ActivityDefinition', 'http://hl7.org/fhir/StructureDefinition/Questionnaire', 'http://hl7.org/fhir/StructureDefinition/Measure', 'http://hl7.org/fhir/StructureDefinition/OperationDefinition'],
            'type'=>'canonical',
            'path'=>'Detail.instantiatesCanonical',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Instantiates external protocol or definition
          # The URL pointing to an externally maintained protocol, guideline, questionnaire or other definition that is adhered to in whole or in part by this CarePlan activity.
          # This might be an HTML page, PDF, etc. or could just be a non-resolvable URI identifier.
          'instantiatesUri' => {
            'type'=>'uri',
            'path'=>'Detail.instantiatesUri',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Detail type of activity
          # Detailed description of the type of planned activity; e.g. what lab test, what procedure, what kind of encounter.
          # Tends to be less relevant for activities involving particular products.  Codes should not convey negation - use "prohibited" instead.
          'code' => {
            'type'=>'CodeableConcept',
            'path'=>'Detail.code',
            'min'=>0,
            'max'=>1
          },
          ##
          # Why activity should be done or why activity was prohibited
          # Provides the rationale that drove the inclusion of this particular activity as part of the plan or the reason why the activity was prohibited.
          # This could be a diagnosis code.  If a full condition record exists or additional detail is needed, use reasonCondition instead.
          'reasonCode' => {
            'type'=>'CodeableConcept',
            'path'=>'Detail.reasonCode',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Why activity is needed
          # Indicates another resource, such as the health condition(s), whose existence justifies this request and drove the inclusion of this particular activity as part of the plan.
          # Conditions can be identified at the activity level that are not identified as reasons for the overall plan.
          'reasonReference' => {
            'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Condition', 'http://hl7.org/fhir/StructureDefinition/Observation', 'http://hl7.org/fhir/StructureDefinition/DiagnosticReport', 'http://hl7.org/fhir/StructureDefinition/DocumentReference'],
            'type'=>'Reference',
            'path'=>'Detail.reasonReference',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Goals this activity relates to
          # Internal reference that identifies the goals that this activity is intended to contribute towards meeting.
          'goal' => {
            'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Goal'],
            'type'=>'Reference',
            'path'=>'Detail.goal',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # not-started | scheduled | in-progress | on-hold | completed | cancelled | stopped | unknown | entered-in-error
          # Identifies what progress is being made for the specific activity.
          # Some aspects of status can be inferred based on the resources linked in actionTaken.  Note that "status" is only as current as the plan was most recently updated.  
          # The unknown code is not to be used to convey other statuses.  The unknown code should be used when one of the statuses applies, but the authoring system doesn't know the current state of the activity.
          'status' => {
            'valid_codes'=>{
              'http://hl7.org/fhir/care-plan-activity-status'=>[ 'not-started', 'scheduled', 'in-progress', 'on-hold', 'completed', 'cancelled', 'stopped', 'unknown', 'entered-in-error' ]
            },
            'type'=>'code',
            'path'=>'Detail.status',
            'min'=>1,
            'max'=>1,
            'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/care-plan-activity-status'}
          },
          ##
          # Reason for current status
          # Provides reason why the activity isn't yet started, is on hold, was cancelled, etc.
          # Will generally not be present if status is "complete".  Be sure to prompt to update this (or at least remove the existing value) if the status is changed.
          'statusReason' => {
            'type'=>'CodeableConcept',
            'path'=>'Detail.statusReason',
            'min'=>0,
            'max'=>1
          },
          ##
          # If true, activity is prohibiting action
          # If true, indicates that the described activity is one that must NOT be engaged in when following the plan.  If false, or missing, indicates that the described activity is one that should be engaged in when following the plan.
          # This element is labeled as a modifier because it marks an activity as an activity that is not to be performed.
          'doNotPerform' => {
            'type'=>'boolean',
            'path'=>'Detail.doNotPerform',
            'min'=>0,
            'max'=>1
          },
          ##
          # When activity is to occur
          # The period, timing or frequency upon which the described activity is to occur.
          'scheduledPeriod' => {
            'type'=>'Period',
            'path'=>'Detail.scheduled[x]',
            'min'=>0,
            'max'=>1
          },
          ##
          # When activity is to occur
          # The period, timing or frequency upon which the described activity is to occur.
          'scheduledString' => {
            'type'=>'String',
            'path'=>'Detail.scheduled[x]',
            'min'=>0,
            'max'=>1
          },
          ##
          # When activity is to occur
          # The period, timing or frequency upon which the described activity is to occur.
          'scheduledTiming' => {
            'type'=>'Timing',
            'path'=>'Detail.scheduled[x]',
            'min'=>0,
            'max'=>1
          },
          ##
          # Where it should happen
          # Identifies the facility where the activity will occur; e.g. home, hospital, specific clinic, etc.
          # May reference a specific clinical location or may identify a type of location.
          'location' => {
            'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Location'],
            'type'=>'Reference',
            'path'=>'Detail.location',
            'min'=>0,
            'max'=>1
          },
          ##
          # Who will be responsible?
          # Identifies who's expected to be involved in the activity.
          # A performer MAY also be a participant in the care plan.
          'performer' => {
            'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/CareTeam', 'http://hl7.org/fhir/StructureDefinition/HealthcareService', 'http://hl7.org/fhir/StructureDefinition/Device'],
            'type'=>'Reference',
            'path'=>'Detail.performer',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # What is to be administered/supplied
          # Identifies the food, drug or other product to be consumed or supplied in the activity.
          'productCodeableConcept' => {
            'type'=>'CodeableConcept',
            'path'=>'Detail.product[x]',
            'min'=>0,
            'max'=>1
          },
          ##
          # What is to be administered/supplied
          # Identifies the food, drug or other product to be consumed or supplied in the activity.
          'productReference' => {
            'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Medication', 'http://hl7.org/fhir/StructureDefinition/Substance'],
            'type'=>'Reference',
            'path'=>'Detail.product[x]',
            'min'=>0,
            'max'=>1
          },
          ##
          # How to consume/day?
          # Identifies the quantity expected to be consumed in a given day.
          'dailyAmount' => {
            'type'=>'Quantity',
            'path'=>'Detail.dailyAmount',
            'min'=>0,
            'max'=>1
          },
          ##
          # How much to administer/supply/consume
          # Identifies the quantity expected to be supplied, administered or consumed by the subject.
          'quantity' => {
            'type'=>'Quantity',
            'path'=>'Detail.quantity',
            'min'=>0,
            'max'=>1
          },
          ##
          # Extra info describing activity to perform
          # This provides a textual description of constraints on the intended activity occurrence, including relation to other activities.  It may also include objectives, pre-conditions and end-conditions.  Finally, it may convey specifics about the activity such as body site, method, route, etc.
          'description' => {
            'type'=>'string',
            'path'=>'Detail.description',
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
        # Appointment | CommunicationRequest | DeviceRequest | MedicationRequest | NutritionOrder | Task | ServiceRequest | VisionPrescription
        # A description of the kind of resource the in-line definition of a care plan activity is representing.  The CarePlan.activity.detail is an in-line definition when a resource is not referenced using CarePlan.activity.reference.  For example, a MedicationRequest, a ServiceRequest, or a CommunicationRequest.
        attr_accessor :kind                           # 0-1 code
        ##
        # Instantiates FHIR protocol or definition
        # The URL pointing to a FHIR-defined protocol, guideline, questionnaire or other definition that is adhered to in whole or in part by this CarePlan activity.
        attr_accessor :instantiatesCanonical          # 0-* [ canonical(http://hl7.org/fhir/StructureDefinition/PlanDefinition|http://hl7.org/fhir/StructureDefinition/ActivityDefinition|http://hl7.org/fhir/StructureDefinition/Questionnaire|http://hl7.org/fhir/StructureDefinition/Measure|http://hl7.org/fhir/StructureDefinition/OperationDefinition) ]
        ##
        # Instantiates external protocol or definition
        # The URL pointing to an externally maintained protocol, guideline, questionnaire or other definition that is adhered to in whole or in part by this CarePlan activity.
        # This might be an HTML page, PDF, etc. or could just be a non-resolvable URI identifier.
        attr_accessor :instantiatesUri                # 0-* [ uri ]
        ##
        # Detail type of activity
        # Detailed description of the type of planned activity; e.g. what lab test, what procedure, what kind of encounter.
        # Tends to be less relevant for activities involving particular products.  Codes should not convey negation - use "prohibited" instead.
        attr_accessor :code                           # 0-1 CodeableConcept
        ##
        # Why activity should be done or why activity was prohibited
        # Provides the rationale that drove the inclusion of this particular activity as part of the plan or the reason why the activity was prohibited.
        # This could be a diagnosis code.  If a full condition record exists or additional detail is needed, use reasonCondition instead.
        attr_accessor :reasonCode                     # 0-* [ CodeableConcept ]
        ##
        # Why activity is needed
        # Indicates another resource, such as the health condition(s), whose existence justifies this request and drove the inclusion of this particular activity as part of the plan.
        # Conditions can be identified at the activity level that are not identified as reasons for the overall plan.
        attr_accessor :reasonReference                # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Condition|http://hl7.org/fhir/StructureDefinition/Observation|http://hl7.org/fhir/StructureDefinition/DiagnosticReport|http://hl7.org/fhir/StructureDefinition/DocumentReference) ]
        ##
        # Goals this activity relates to
        # Internal reference that identifies the goals that this activity is intended to contribute towards meeting.
        attr_accessor :goal                           # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Goal) ]
        ##
        # not-started | scheduled | in-progress | on-hold | completed | cancelled | stopped | unknown | entered-in-error
        # Identifies what progress is being made for the specific activity.
        # Some aspects of status can be inferred based on the resources linked in actionTaken.  Note that "status" is only as current as the plan was most recently updated.  
        # The unknown code is not to be used to convey other statuses.  The unknown code should be used when one of the statuses applies, but the authoring system doesn't know the current state of the activity.
        attr_accessor :status                         # 1-1 code
        ##
        # Reason for current status
        # Provides reason why the activity isn't yet started, is on hold, was cancelled, etc.
        # Will generally not be present if status is "complete".  Be sure to prompt to update this (or at least remove the existing value) if the status is changed.
        attr_accessor :statusReason                   # 0-1 CodeableConcept
        ##
        # If true, activity is prohibiting action
        # If true, indicates that the described activity is one that must NOT be engaged in when following the plan.  If false, or missing, indicates that the described activity is one that should be engaged in when following the plan.
        # This element is labeled as a modifier because it marks an activity as an activity that is not to be performed.
        attr_accessor :doNotPerform                   # 0-1 boolean
        ##
        # When activity is to occur
        # The period, timing or frequency upon which the described activity is to occur.
        attr_accessor :scheduledPeriod                # 0-1 Period
        ##
        # When activity is to occur
        # The period, timing or frequency upon which the described activity is to occur.
        attr_accessor :scheduledString                # 0-1 String
        ##
        # When activity is to occur
        # The period, timing or frequency upon which the described activity is to occur.
        attr_accessor :scheduledTiming                # 0-1 Timing
        ##
        # Where it should happen
        # Identifies the facility where the activity will occur; e.g. home, hospital, specific clinic, etc.
        # May reference a specific clinical location or may identify a type of location.
        attr_accessor :location                       # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Location)
        ##
        # Who will be responsible?
        # Identifies who's expected to be involved in the activity.
        # A performer MAY also be a participant in the care plan.
        attr_accessor :performer                      # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/CareTeam|http://hl7.org/fhir/StructureDefinition/HealthcareService|http://hl7.org/fhir/StructureDefinition/Device) ]
        ##
        # What is to be administered/supplied
        # Identifies the food, drug or other product to be consumed or supplied in the activity.
        attr_accessor :productCodeableConcept         # 0-1 CodeableConcept
        ##
        # What is to be administered/supplied
        # Identifies the food, drug or other product to be consumed or supplied in the activity.
        attr_accessor :productReference               # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Medication|http://hl7.org/fhir/StructureDefinition/Substance)
        ##
        # How to consume/day?
        # Identifies the quantity expected to be consumed in a given day.
        attr_accessor :dailyAmount                    # 0-1 Quantity
        ##
        # How much to administer/supply/consume
        # Identifies the quantity expected to be supplied, administered or consumed by the subject.
        attr_accessor :quantity                       # 0-1 Quantity
        ##
        # Extra info describing activity to perform
        # This provides a textual description of constraints on the intended activity occurrence, including relation to other activities.  It may also include objectives, pre-conditions and end-conditions.  Finally, it may convey specifics about the activity such as body site, method, route, etc.
        attr_accessor :description                    # 0-1 string
      end
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
      # Results of the activity
      # Identifies the outcome at the point when the status of the activity is assessed.  For example, the outcome of an education activity could be patient understands (or not).
      # Note that this should not duplicate the activity status (e.g. completed or in progress).
      attr_accessor :outcomeCodeableConcept         # 0-* [ CodeableConcept ]
      ##
      # Appointment, Encounter, Procedure, etc.
      # Details of the outcome or action resulting from the activity.  The reference to an "event" resource, such as Procedure or Encounter or Observation, is the result/outcome of the activity itself.  The activity can be conveyed using CarePlan.activity.detail OR using the CarePlan.activity.reference (a reference to a “request” resource).
      # The activity outcome is independent of the outcome of the related goal(s).  For example, if the goal is to achieve a target body weight of 150 lbs and an activity is defined to diet, then the activity outcome could be calories consumed whereas the goal outcome is an observation for the actual body weight measured.
      attr_accessor :outcomeReference               # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Resource) ]
      ##
      # Comments about the activity status/progress
      # Notes about the adherence/status/progress of the activity.
      # This element should NOT be used to describe the activity to be performed - that occurs either within the resource pointed to by activity.detail.reference or in activity.detail.description.
      attr_accessor :progress                       # 0-* [ Annotation ]
      ##
      # Activity details defined in specific resource
      # The details of the proposed activity represented in a specific resource.
      # Standard extension exists ([resource-pertainsToGoal](extension-resource-pertainstogoal.html)) that allows goals to be referenced from any of the referenced resources in CarePlan.activity.reference.  The goal should be visible when the resource referenced by CarePlan.activity.reference is viewed independently from the CarePlan.  Requests that are pointed to by a CarePlan using this element should *not* point to this CarePlan using the "basedOn" element.  i.e. Requests that are part of a CarePlan are not "based on" the CarePlan.
      attr_accessor :reference                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Appointment|http://hl7.org/fhir/StructureDefinition/CommunicationRequest|http://hl7.org/fhir/StructureDefinition/DeviceRequest|http://hl7.org/fhir/StructureDefinition/MedicationRequest|http://hl7.org/fhir/StructureDefinition/NutritionOrder|http://hl7.org/fhir/StructureDefinition/Task|http://hl7.org/fhir/StructureDefinition/ServiceRequest|http://hl7.org/fhir/StructureDefinition/VisionPrescription|http://hl7.org/fhir/StructureDefinition/RequestGroup)
      ##
      # In-line definition of activity
      # A simple summary of a planned activity suitable for a general care plan system (e.g. form driven) that doesn't know about specific resources such as procedure etc.
      attr_accessor :detail                         # 0-1 CarePlan::Activity::Detail
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
    # External Ids for this plan
    # Business identifiers assigned to this care plan by the performer or other systems which remain constant as the resource is updated and propagates from server to server.
    # This is a business identifier, not a resource identifier (see [discussion](resource.html#identifiers)).  It is best practice for the identifier to only appear on a single resource instance, however business practices may occasionally dictate that multiple resource instances with the same identifier can exist - possibly even with different resource types.  For example, multiple Patient and a Person resource instance might share the same social insurance number.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Instantiates FHIR protocol or definition
    # The URL pointing to a FHIR-defined protocol, guideline, questionnaire or other definition that is adhered to in whole or in part by this CarePlan.
    attr_accessor :instantiatesCanonical          # 0-* [ canonical(http://hl7.org/fhir/StructureDefinition/PlanDefinition|http://hl7.org/fhir/StructureDefinition/Questionnaire|http://hl7.org/fhir/StructureDefinition/Measure|http://hl7.org/fhir/StructureDefinition/ActivityDefinition|http://hl7.org/fhir/StructureDefinition/OperationDefinition) ]
    ##
    # Instantiates external protocol or definition
    # The URL pointing to an externally maintained protocol, guideline, questionnaire or other definition that is adhered to in whole or in part by this CarePlan.
    # This might be an HTML page, PDF, etc. or could just be a non-resolvable URI identifier.
    attr_accessor :instantiatesUri                # 0-* [ uri ]
    ##
    # Fulfills CarePlan
    # A care plan that is fulfilled in whole or in part by this care plan.
    attr_accessor :basedOn                        # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/CarePlan) ]
    ##
    # CarePlan replaced by this CarePlan
    # Completed or terminated care plan whose function is taken by this new care plan.
    # The replacement could be because the initial care plan was immediately rejected (due to an issue) or because the previous care plan was completed, but the need for the action described by the care plan remains ongoing.
    attr_accessor :replaces                       # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/CarePlan) ]
    ##
    # Part of referenced CarePlan
    # A larger care plan of which this particular care plan is a component or step.
    # Each care plan is an independent request, such that having a care plan be part of another care plan can cause issues with cascading statuses.  As such, this element is still being discussed.
    attr_accessor :partOf                         # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/CarePlan) ]
    ##
    # draft | active | on-hold | revoked | completed | entered-in-error | unknown
    # Indicates whether the plan is currently being acted upon, represents future intentions or is now a historical record.
    # The unknown code is not to be used to convey other statuses.  The unknown code should be used when one of the statuses applies, but the authoring system doesn't know the current state of the care plan.
    # 
    # This element is labeled as a modifier because the status contains the code entered-in-error that marks the plan as not currently valid.
    attr_accessor :status                         # 1-1 code
    ##
    # proposal | plan | order | option
    # Indicates the level of authority/intentionality associated with the care plan and where the care plan fits into the workflow chain.
    # This element is labeled as a modifier because the intent alters when and how the resource is actually applicable.
    attr_accessor :intent                         # 1-1 code
    ##
    # Type of plan
    # Identifies what "kind" of plan this is to support differentiation between multiple co-existing plans; e.g. "Home health", "psychiatric", "asthma", "disease management", "wellness plan", etc.
    # There may be multiple axes of categorization and one plan may serve multiple purposes.  In some cases, this may be redundant with references to CarePlan.concern.
    attr_accessor :category                       # 0-* [ CodeableConcept ]
    ##
    # Human-friendly name for the care plan.
    attr_accessor :title                          # 0-1 string
    ##
    # Summary of nature of plan
    # A description of the scope and nature of the plan.
    attr_accessor :description                    # 0-1 string
    ##
    # Who the care plan is for
    # Identifies the patient or group whose intended care is described by the plan.
    attr_accessor :subject                        # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Group)
    ##
    # Encounter created as part of
    # The Encounter during which this CarePlan was created or to which the creation of this record is tightly associated.
    # This will typically be the encounter the event occurred within, but some activities may be initiated prior to or after the official completion of an encounter but still be tied to the context of the encounter. CarePlan activities conducted as a result of the care plan may well occur as part of other encounters.
    attr_accessor :encounter                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Encounter)
    ##
    # Time period plan covers
    # Indicates when the plan did (or is intended to) come into effect and end.
    # Any activities scheduled as part of the plan should be constrained to the specified period regardless of whether the activities are planned within a single encounter/episode or across multiple encounters/episodes (e.g. the longitudinal management of a chronic condition).
    attr_accessor :period                         # 0-1 Period
    ##
    # Date record was first recorded
    # Represents when this particular CarePlan record was created in the system, which is often a system-generated date.
    attr_accessor :created                        # 0-1 dateTime
    ##
    # Who is the designated responsible party
    # When populated, the author is responsible for the care plan.  The care plan is attributed to the author.
    # The author may also be a contributor.  For example, an organization can be an author, but not listed as a contributor.
    attr_accessor :author                         # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/CareTeam)
    ##
    # Who provided the content of the care plan
    # Identifies the individual(s) or organization who provided the contents of the care plan.
    # Collaborative care plans may have multiple contributors.
    attr_accessor :contributor                    # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/CareTeam) ]
    ##
    # Who's involved in plan?
    # Identifies all people and organizations who are expected to be involved in the care envisioned by this plan.
    attr_accessor :careTeam                       # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/CareTeam) ]
    ##
    # Health issues this plan addresses
    # Identifies the conditions/problems/concerns/diagnoses/etc. whose management and/or mitigation are handled by this plan.
    # When the diagnosis is related to an allergy or intolerance, the Condition and AllergyIntolerance resources can both be used. However, to be actionable for decision support, using Condition alone is not sufficient as the allergy or intolerance condition needs to be represented as an AllergyIntolerance.
    attr_accessor :addresses                      # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Condition) ]
    ##
    # Information considered as part of plan
    # Identifies portions of the patient's record that specifically influenced the formation of the plan.  These might include comorbidities, recent procedures, limitations, recent assessments, etc.
    # Use "concern" to identify specific conditions addressed by the care plan.
    attr_accessor :supportingInfo                 # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Resource) ]
    ##
    # Desired outcome of plan
    # Describes the intended objective(s) of carrying out the care plan.
    # Goal can be achieving a particular change or merely maintaining a current state or even slowing a decline.
    attr_accessor :goal                           # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Goal) ]
    ##
    # Action to occur as part of plan
    # Identifies a planned action to occur as part of the plan.  For example, a medication to be used, lab tests to perform, self-monitoring, education, etc.
    attr_accessor :activity                       # 0-* [ CarePlan::Activity ]
    ##
    # Comments about the plan
    # General notes about the care plan not covered elsewhere.
    attr_accessor :note                           # 0-* [ Annotation ]

    def resourceType
      'CarePlan'
    end
  end
end
