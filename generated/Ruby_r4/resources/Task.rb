module FHIR

  ##
  # A task to be performed.
  class Task < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['authored-on', 'based-on', 'business-status', 'code', 'encounter', 'focus', 'group-identifier', 'identifier', 'intent', 'modified', 'owner', 'part-of', 'patient', 'performer', 'period', 'priority', 'requester', 'status', 'subject']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'Task.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'Task.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'Task.implicitRules',
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
        'path'=>'Task.language',
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
        'path'=>'Task.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'Task.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Task.extension',
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
        'path'=>'Task.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Task Instance Identifier
      # The business identifier for this task.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'Task.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Formal definition of task
      # The URL pointing to a *FHIR*-defined protocol, guideline, orderset or other definition that is adhered to in whole or in part by this Task.
      'instantiatesCanonical' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ActivityDefinition'],
        'type'=>'canonical',
        'path'=>'Task.instantiatesCanonical',
        'min'=>0,
        'max'=>1
      },
      ##
      # Formal definition of task
      # The URL pointing to an *externally* maintained  protocol, guideline, orderset or other definition that is adhered to in whole or in part by this Task.
      'instantiatesUri' => {
        'type'=>'uri',
        'path'=>'Task.instantiatesUri',
        'min'=>0,
        'max'=>1
      },
      ##
      # Request fulfilled by this task
      # BasedOn refers to a higher-level authorization that triggered the creation of the task.  It references a "request" resource such as a ServiceRequest, MedicationRequest, ServiceRequest, CarePlan, etc. which is distinct from the "request" resource the task is seeking to fulfill.  This latter resource is referenced by FocusOn.  For example, based on a ServiceRequest (= BasedOn), a task is created to fulfill a procedureRequest ( = FocusOn ) to collect a specimen from a patient.
      'basedOn' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'Reference',
        'path'=>'Task.basedOn',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Requisition or grouper id
      # An identifier that links together multiple tasks and other requests that were created in the same context.
      'groupIdentifier' => {
        'type'=>'Identifier',
        'path'=>'Task.groupIdentifier',
        'min'=>0,
        'max'=>1
      },
      ##
      # Composite task
      # Task that this particular task is part of.
      # This should usually be 0..1.
      'partOf' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Task'],
        'type'=>'Reference',
        'path'=>'Task.partOf',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # draft | requested | received | accepted | +
      # The current status of the task.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/task-status'=>[ 'draft', 'requested', 'received', 'accepted', 'rejected', 'ready', 'cancelled', 'in-progress', 'on-hold', 'failed', 'completed', 'entered-in-error' ]
        },
        'type'=>'code',
        'path'=>'Task.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/task-status'}
      },
      ##
      # Reason for current status
      # An explanation as to why this task is held, failed, was refused, etc.
      # This applies to the current status.  Look at the history of the task to see reasons for past statuses.
      'statusReason' => {
        'type'=>'CodeableConcept',
        'path'=>'Task.statusReason',
        'min'=>0,
        'max'=>1
      },
      ##
      # E.g. "Specimen collected", "IV prepped"
      # Contains business-specific nuances of the business state.
      'businessStatus' => {
        'type'=>'CodeableConcept',
        'path'=>'Task.businessStatus',
        'min'=>0,
        'max'=>1
      },
      ##
      # unknown | proposal | plan | order | original-order | reflex-order | filler-order | instance-order | option
      # Indicates the "level" of actionability associated with the Task, i.e. i+R[9]Cs this a proposed task, a planned task, an actionable task, etc.
      # This element is immutable.  Proposed tasks, planned tasks, etc. must be distinct instances.
      # 
      # In most cases, Tasks will have an intent of "order".
      'intent' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/request-intent'=>[ 'proposal', 'plan', 'order', 'original-order', 'reflex-order', 'filler-order', 'instance-order', 'option' ],
          'http://hl7.org/fhir/task-intent'=>[ 'unknown' ]
        },
        'type'=>'code',
        'path'=>'Task.intent',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/task-intent'}
      },
      ##
      # routine | urgent | asap | stat
      # Indicates how quickly the Task should be addressed with respect to other requests.
      'priority' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/request-priority'=>[ 'routine', 'urgent', 'asap', 'stat' ]
        },
        'type'=>'code',
        'path'=>'Task.priority',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/request-priority'}
      },
      ##
      # Task Type
      # A name or code (or both) briefly describing what the task involves.
      # The title (eg "My Tasks", "Outstanding Tasks for Patient X") should go into the code.
      'code' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/CodeSystem/task-code'=>[ 'approve', 'fulfill', 'abort', 'replace', 'change', 'suspend', 'resume' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Task.code',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/task-code'}
      },
      ##
      # Human-readable explanation of task
      # A free-text description of what is to be performed.
      'description' => {
        'type'=>'string',
        'path'=>'Task.description',
        'min'=>0,
        'max'=>1
      },
      ##
      # What task is acting on
      # The request being actioned or the resource being manipulated by this task.
      # If multiple resources need to be manipulated, use sub-tasks.  (This ensures that status can be tracked independently for each referenced resource.).
      'focus' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'Reference',
        'path'=>'Task.focus',
        'min'=>0,
        'max'=>1
      },
      ##
      # Beneficiary of the Task
      # The entity who benefits from the performance of the service specified in the task (e.g., the patient).
      'for' => {
        'local_name'=>'local_for'
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'Reference',
        'path'=>'Task.for',
        'min'=>0,
        'max'=>1
      },
      ##
      # Healthcare event during which this task originated
      # The healthcare event  (e.g. a patient and healthcare provider interaction) during which this task was created.
      'encounter' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Encounter'],
        'type'=>'Reference',
        'path'=>'Task.encounter',
        'min'=>0,
        'max'=>1
      },
      ##
      # Start and end time of execution
      # Identifies the time action was first taken against the task (start) and/or the time final action was taken against the task prior to marking it as completed (end).
      'executionPeriod' => {
        'type'=>'Period',
        'path'=>'Task.executionPeriod',
        'min'=>0,
        'max'=>1
      },
      ##
      # Task Creation Date
      # The date and time this task was created.
      'authoredOn' => {
        'type'=>'dateTime',
        'path'=>'Task.authoredOn',
        'min'=>0,
        'max'=>1
      },
      ##
      # Task Last Modified Date
      # The date and time of last modification to this task.
      'lastModified' => {
        'type'=>'dateTime',
        'path'=>'Task.lastModified',
        'min'=>0,
        'max'=>1
      },
      ##
      # Who is asking for task to be done
      # The creator of the task.
      'requester' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson'],
        'type'=>'Reference',
        'path'=>'Task.requester',
        'min'=>0,
        'max'=>1
      },
      ##
      # Requested performer
      # The kind of participant that should perform the task.
      'performerType' => {
        'type'=>'CodeableConcept',
        'path'=>'Task.performerType',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Responsible individual
      # Individual organization or Device currently responsible for task execution.
      # Tasks may be created with an owner not yet identified.
      'owner' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/CareTeam', 'http://hl7.org/fhir/StructureDefinition/HealthcareService', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson'],
        'type'=>'Reference',
        'path'=>'Task.owner',
        'min'=>0,
        'max'=>1
      },
      ##
      # Where task occurs
      # Principal physical location where the this task is performed.
      'location' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Location'],
        'type'=>'Reference',
        'path'=>'Task.location',
        'min'=>0,
        'max'=>1
      },
      ##
      # Why task is needed
      # A description or code indicating why this task needs to be performed.
      # This should only be included if there is no focus or if it differs from the reason indicated on the focus.
      'reasonCode' => {
        'type'=>'CodeableConcept',
        'path'=>'Task.reasonCode',
        'min'=>0,
        'max'=>1
      },
      ##
      # Why task is needed
      # A resource reference indicating why this task needs to be performed.
      # Tasks might be justified based on an Observation, a Condition, a past or planned procedure, etc.   This should only be included if there is no focus or if it differs from the reason indicated on the focus.    Use the CodeableConcept text element in `Task.reasonCode` if the data is free (uncoded) text.
      'reasonReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'Reference',
        'path'=>'Task.reasonReference',
        'min'=>0,
        'max'=>1
      },
      ##
      # Associated insurance coverage
      # Insurance plans, coverage extensions, pre-authorizations and/or pre-determinations that may be relevant to the Task.
      'insurance' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Coverage', 'http://hl7.org/fhir/StructureDefinition/ClaimResponse'],
        'type'=>'Reference',
        'path'=>'Task.insurance',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Comments made about the task
      # Free-text information captured about the task as it progresses.
      'note' => {
        'type'=>'Annotation',
        'path'=>'Task.note',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Key events in history of the Task
      # Links to Provenance records for past versions of this Task that identify key state transitions or updates that are likely to be relevant to a user looking at the current version of the task.
      # This element does not point to the Provenance associated with the *current* version of the resource - as it would be created after this version existed.  The Provenance for the current version can be retrieved with a _revinclude.
      'relevantHistory' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Provenance'],
        'type'=>'Reference',
        'path'=>'Task.relevantHistory',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Constraints on fulfillment tasks
      # If the Task.focus is a request resource and the task is seeking fulfillment (i.e. is asking for the request to be actioned), this element identifies any limitations on what parts of the referenced request should be actioned.
      'restriction' => {
        'type'=>'Task::Restriction',
        'path'=>'Task.restriction',
        'min'=>0,
        'max'=>1
      },
      ##
      # Information used to perform task
      # Additional information that may be needed in the execution of the task.
      'input' => {
        'type'=>'Task::Input',
        'path'=>'Task.input',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Information produced as part of task
      # Outputs produced by the Task.
      'output' => {
        'type'=>'Task::Output',
        'path'=>'Task.output',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Constraints on fulfillment tasks
    # If the Task.focus is a request resource and the task is seeking fulfillment (i.e. is asking for the request to be actioned), this element identifies any limitations on what parts of the referenced request should be actioned.
    class Restriction < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Restriction.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Restriction.extension',
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
          'path'=>'Restriction.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # How many times to repeat
        # Indicates the number of times the requested action should occur.
        'repetitions' => {
          'type'=>'positiveInt',
          'path'=>'Restriction.repetitions',
          'min'=>0,
          'max'=>1
        },
        ##
        # When fulfillment sought
        # Over what time-period is fulfillment sought.
        # Note that period.high is the due date representing the time by which the task should be completed.
        'period' => {
          'type'=>'Period',
          'path'=>'Restriction.period',
          'min'=>0,
          'max'=>1
        },
        ##
        # For whom is fulfillment sought?
        # For requests that are targeted to more than on potential recipient/target, for whom is fulfillment sought?
        'recipient' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/Group', 'http://hl7.org/fhir/StructureDefinition/Organization'],
          'type'=>'Reference',
          'path'=>'Restriction.recipient',
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
      # How many times to repeat
      # Indicates the number of times the requested action should occur.
      attr_accessor :repetitions                    # 0-1 positiveInt
      ##
      # When fulfillment sought
      # Over what time-period is fulfillment sought.
      # Note that period.high is the due date representing the time by which the task should be completed.
      attr_accessor :period                         # 0-1 Period
      ##
      # For whom is fulfillment sought?
      # For requests that are targeted to more than on potential recipient/target, for whom is fulfillment sought?
      attr_accessor :recipient                      # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/Group|http://hl7.org/fhir/StructureDefinition/Organization) ]
    end

    ##
    # Information used to perform task
    # Additional information that may be needed in the execution of the task.
    class Input < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'value[x]' => ['Address', 'Age', 'Annotation', 'Attachment', 'base64Binary', 'boolean', 'canonical', 'code', 'CodeableConcept', 'Coding', 'ContactDetail', 'ContactPoint', 'Contributor', 'Count', 'DataRequirement', 'date', 'dateTime', 'decimal', 'Distance', 'Dosage', 'Duration', 'Expression', 'HumanName', 'id', 'Identifier', 'instant', 'integer', 'markdown', 'Meta', 'Money', 'oid', 'ParameterDefinition', 'Period', 'positiveInt', 'Quantity', 'Range', 'Ratio', 'Reference', 'RelatedArtifact', 'SampledData', 'Signature', 'string', 'time', 'Timing', 'TriggerDefinition', 'unsignedInt', 'uri', 'url', 'UsageContext', 'uuid']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Input.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Input.extension',
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
          'path'=>'Input.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Label for the input
        # A code or description indicating how the input is intended to be used as part of the task execution.
        # If referencing a BPMN workflow or Protocol, the "system" is the URL for the workflow definition and the code is the "name" of the required input.
        'type' => {
          'type'=>'CodeableConcept',
          'path'=>'Input.type',
          'min'=>1,
          'max'=>1
        },
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueAddress' => {
          'type'=>'Address',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueAge' => {
          'type'=>'Age',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueAnnotation' => {
          'type'=>'Annotation',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueAttachment' => {
          'type'=>'Attachment',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueBase64Binary' => {
          'type'=>'Base64Binary',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueBoolean' => {
          'type'=>'Boolean',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueCanonical' => {
          'type'=>'Canonical',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueCode' => {
          'type'=>'Code',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueCodeableConcept' => {
          'type'=>'CodeableConcept',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueCoding' => {
          'type'=>'Coding',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueContactDetail' => {
          'type'=>'ContactDetail',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueContactPoint' => {
          'type'=>'ContactPoint',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueContributor' => {
          'type'=>'Contributor',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueCount' => {
          'type'=>'Count',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueDataRequirement' => {
          'type'=>'DataRequirement',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueDate' => {
          'type'=>'Date',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueDateTime' => {
          'type'=>'DateTime',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueDecimal' => {
          'type'=>'Decimal',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueDistance' => {
          'type'=>'Distance',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueDosage' => {
          'type'=>'Dosage',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueDuration' => {
          'type'=>'Duration',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueExpression' => {
          'type'=>'Expression',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueHumanName' => {
          'type'=>'HumanName',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueId' => {
          'type'=>'Id',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueIdentifier' => {
          'type'=>'Identifier',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueInstant' => {
          'type'=>'Instant',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueInteger' => {
          'type'=>'Integer',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueMarkdown' => {
          'type'=>'Markdown',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueMeta' => {
          'type'=>'Meta',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueMoney' => {
          'type'=>'Money',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueOid' => {
          'type'=>'Oid',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueParameterDefinition' => {
          'type'=>'ParameterDefinition',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valuePeriod' => {
          'type'=>'Period',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valuePositiveInt' => {
          'type'=>'PositiveInt',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueQuantity' => {
          'type'=>'Quantity',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueRange' => {
          'type'=>'Range',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueRatio' => {
          'type'=>'Ratio',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueReference' => {
          'type'=>'Reference',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueRelatedArtifact' => {
          'type'=>'RelatedArtifact',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueSampledData' => {
          'type'=>'SampledData',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueSignature' => {
          'type'=>'Signature',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueString' => {
          'type'=>'String',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueTime' => {
          'type'=>'Time',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueTiming' => {
          'type'=>'Timing',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueTriggerDefinition' => {
          'type'=>'TriggerDefinition',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueUnsignedInt' => {
          'type'=>'UnsignedInt',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueUri' => {
          'type'=>'Uri',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueUrl' => {
          'type'=>'Url',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueUsageContext' => {
          'type'=>'UsageContext',
          'path'=>'Input.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Content to use in performing the task
        # The value of the input parameter as a basic type.
        'valueUuid' => {
          'type'=>'Uuid',
          'path'=>'Input.value[x]',
          'min'=>1,
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
      # Label for the input
      # A code or description indicating how the input is intended to be used as part of the task execution.
      # If referencing a BPMN workflow or Protocol, the "system" is the URL for the workflow definition and the code is the "name" of the required input.
      attr_accessor :type                           # 1-1 CodeableConcept
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueAddress                   # 1-1 Address
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueAge                       # 1-1 Age
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueAnnotation                # 1-1 Annotation
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueAttachment                # 1-1 Attachment
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueBase64Binary              # 1-1 Base64Binary
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueBoolean                   # 1-1 Boolean
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueCanonical                 # 1-1 Canonical
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueCode                      # 1-1 Code
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueCodeableConcept           # 1-1 CodeableConcept
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueCoding                    # 1-1 Coding
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueContactDetail             # 1-1 ContactDetail
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueContactPoint              # 1-1 ContactPoint
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueContributor               # 1-1 Contributor
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueCount                     # 1-1 Count
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueDataRequirement           # 1-1 DataRequirement
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueDate                      # 1-1 Date
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueDateTime                  # 1-1 DateTime
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueDecimal                   # 1-1 Decimal
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueDistance                  # 1-1 Distance
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueDosage                    # 1-1 Dosage
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueDuration                  # 1-1 Duration
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueExpression                # 1-1 Expression
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueHumanName                 # 1-1 HumanName
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueId                        # 1-1 Id
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueIdentifier                # 1-1 Identifier
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueInstant                   # 1-1 Instant
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueInteger                   # 1-1 Integer
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueMarkdown                  # 1-1 Markdown
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueMeta                      # 1-1 Meta
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueMoney                     # 1-1 Money
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueOid                       # 1-1 Oid
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueParameterDefinition       # 1-1 ParameterDefinition
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valuePeriod                    # 1-1 Period
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valuePositiveInt               # 1-1 PositiveInt
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueQuantity                  # 1-1 Quantity
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueRange                     # 1-1 Range
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueRatio                     # 1-1 Ratio
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueReference                 # 1-1 Reference
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueRelatedArtifact           # 1-1 RelatedArtifact
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueSampledData               # 1-1 SampledData
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueSignature                 # 1-1 Signature
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueString                    # 1-1 String
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueTime                      # 1-1 Time
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueTiming                    # 1-1 Timing
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueTriggerDefinition         # 1-1 TriggerDefinition
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueUnsignedInt               # 1-1 UnsignedInt
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueUri                       # 1-1 Uri
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueUrl                       # 1-1 Url
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueUsageContext              # 1-1 UsageContext
      ##
      # Content to use in performing the task
      # The value of the input parameter as a basic type.
      attr_accessor :valueUuid                      # 1-1 Uuid
    end

    ##
    # Information produced as part of task
    # Outputs produced by the Task.
    class Output < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'value[x]' => ['Address', 'Age', 'Annotation', 'Attachment', 'base64Binary', 'boolean', 'canonical', 'code', 'CodeableConcept', 'Coding', 'ContactDetail', 'ContactPoint', 'Contributor', 'Count', 'DataRequirement', 'date', 'dateTime', 'decimal', 'Distance', 'Dosage', 'Duration', 'Expression', 'HumanName', 'id', 'Identifier', 'instant', 'integer', 'markdown', 'Meta', 'Money', 'oid', 'ParameterDefinition', 'Period', 'positiveInt', 'Quantity', 'Range', 'Ratio', 'Reference', 'RelatedArtifact', 'SampledData', 'Signature', 'string', 'time', 'Timing', 'TriggerDefinition', 'unsignedInt', 'uri', 'url', 'UsageContext', 'uuid']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Output.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Output.extension',
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
          'path'=>'Output.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Label for output
        # The name of the Output parameter.
        'type' => {
          'type'=>'CodeableConcept',
          'path'=>'Output.type',
          'min'=>1,
          'max'=>1
        },
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueAddress' => {
          'type'=>'Address',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueAge' => {
          'type'=>'Age',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueAnnotation' => {
          'type'=>'Annotation',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueAttachment' => {
          'type'=>'Attachment',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueBase64Binary' => {
          'type'=>'Base64Binary',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueBoolean' => {
          'type'=>'Boolean',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueCanonical' => {
          'type'=>'Canonical',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueCode' => {
          'type'=>'Code',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueCodeableConcept' => {
          'type'=>'CodeableConcept',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueCoding' => {
          'type'=>'Coding',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueContactDetail' => {
          'type'=>'ContactDetail',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueContactPoint' => {
          'type'=>'ContactPoint',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueContributor' => {
          'type'=>'Contributor',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueCount' => {
          'type'=>'Count',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueDataRequirement' => {
          'type'=>'DataRequirement',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueDate' => {
          'type'=>'Date',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueDateTime' => {
          'type'=>'DateTime',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueDecimal' => {
          'type'=>'Decimal',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueDistance' => {
          'type'=>'Distance',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueDosage' => {
          'type'=>'Dosage',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueDuration' => {
          'type'=>'Duration',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueExpression' => {
          'type'=>'Expression',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueHumanName' => {
          'type'=>'HumanName',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueId' => {
          'type'=>'Id',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueIdentifier' => {
          'type'=>'Identifier',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueInstant' => {
          'type'=>'Instant',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueInteger' => {
          'type'=>'Integer',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueMarkdown' => {
          'type'=>'Markdown',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueMeta' => {
          'type'=>'Meta',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueMoney' => {
          'type'=>'Money',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueOid' => {
          'type'=>'Oid',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueParameterDefinition' => {
          'type'=>'ParameterDefinition',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valuePeriod' => {
          'type'=>'Period',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valuePositiveInt' => {
          'type'=>'PositiveInt',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueQuantity' => {
          'type'=>'Quantity',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueRange' => {
          'type'=>'Range',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueRatio' => {
          'type'=>'Ratio',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueReference' => {
          'type'=>'Reference',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueRelatedArtifact' => {
          'type'=>'RelatedArtifact',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueSampledData' => {
          'type'=>'SampledData',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueSignature' => {
          'type'=>'Signature',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueString' => {
          'type'=>'String',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueTime' => {
          'type'=>'Time',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueTiming' => {
          'type'=>'Timing',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueTriggerDefinition' => {
          'type'=>'TriggerDefinition',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueUnsignedInt' => {
          'type'=>'UnsignedInt',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueUri' => {
          'type'=>'Uri',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueUrl' => {
          'type'=>'Url',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueUsageContext' => {
          'type'=>'UsageContext',
          'path'=>'Output.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Result of output
        # The value of the Output parameter as a basic type.
        'valueUuid' => {
          'type'=>'Uuid',
          'path'=>'Output.value[x]',
          'min'=>1,
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
      # Label for output
      # The name of the Output parameter.
      attr_accessor :type                           # 1-1 CodeableConcept
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueAddress                   # 1-1 Address
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueAge                       # 1-1 Age
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueAnnotation                # 1-1 Annotation
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueAttachment                # 1-1 Attachment
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueBase64Binary              # 1-1 Base64Binary
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueBoolean                   # 1-1 Boolean
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueCanonical                 # 1-1 Canonical
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueCode                      # 1-1 Code
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueCodeableConcept           # 1-1 CodeableConcept
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueCoding                    # 1-1 Coding
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueContactDetail             # 1-1 ContactDetail
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueContactPoint              # 1-1 ContactPoint
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueContributor               # 1-1 Contributor
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueCount                     # 1-1 Count
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueDataRequirement           # 1-1 DataRequirement
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueDate                      # 1-1 Date
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueDateTime                  # 1-1 DateTime
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueDecimal                   # 1-1 Decimal
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueDistance                  # 1-1 Distance
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueDosage                    # 1-1 Dosage
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueDuration                  # 1-1 Duration
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueExpression                # 1-1 Expression
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueHumanName                 # 1-1 HumanName
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueId                        # 1-1 Id
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueIdentifier                # 1-1 Identifier
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueInstant                   # 1-1 Instant
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueInteger                   # 1-1 Integer
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueMarkdown                  # 1-1 Markdown
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueMeta                      # 1-1 Meta
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueMoney                     # 1-1 Money
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueOid                       # 1-1 Oid
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueParameterDefinition       # 1-1 ParameterDefinition
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valuePeriod                    # 1-1 Period
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valuePositiveInt               # 1-1 PositiveInt
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueQuantity                  # 1-1 Quantity
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueRange                     # 1-1 Range
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueRatio                     # 1-1 Ratio
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueReference                 # 1-1 Reference
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueRelatedArtifact           # 1-1 RelatedArtifact
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueSampledData               # 1-1 SampledData
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueSignature                 # 1-1 Signature
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueString                    # 1-1 String
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueTime                      # 1-1 Time
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueTiming                    # 1-1 Timing
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueTriggerDefinition         # 1-1 TriggerDefinition
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueUnsignedInt               # 1-1 UnsignedInt
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueUri                       # 1-1 Uri
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueUrl                       # 1-1 Url
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueUsageContext              # 1-1 UsageContext
      ##
      # Result of output
      # The value of the Output parameter as a basic type.
      attr_accessor :valueUuid                      # 1-1 Uuid
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
    # Task Instance Identifier
    # The business identifier for this task.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Formal definition of task
    # The URL pointing to a *FHIR*-defined protocol, guideline, orderset or other definition that is adhered to in whole or in part by this Task.
    attr_accessor :instantiatesCanonical          # 0-1 canonical(http://hl7.org/fhir/StructureDefinition/ActivityDefinition)
    ##
    # Formal definition of task
    # The URL pointing to an *externally* maintained  protocol, guideline, orderset or other definition that is adhered to in whole or in part by this Task.
    attr_accessor :instantiatesUri                # 0-1 uri
    ##
    # Request fulfilled by this task
    # BasedOn refers to a higher-level authorization that triggered the creation of the task.  It references a "request" resource such as a ServiceRequest, MedicationRequest, ServiceRequest, CarePlan, etc. which is distinct from the "request" resource the task is seeking to fulfill.  This latter resource is referenced by FocusOn.  For example, based on a ServiceRequest (= BasedOn), a task is created to fulfill a procedureRequest ( = FocusOn ) to collect a specimen from a patient.
    attr_accessor :basedOn                        # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Resource) ]
    ##
    # Requisition or grouper id
    # An identifier that links together multiple tasks and other requests that were created in the same context.
    attr_accessor :groupIdentifier                # 0-1 Identifier
    ##
    # Composite task
    # Task that this particular task is part of.
    # This should usually be 0..1.
    attr_accessor :partOf                         # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Task) ]
    ##
    # draft | requested | received | accepted | +
    # The current status of the task.
    attr_accessor :status                         # 1-1 code
    ##
    # Reason for current status
    # An explanation as to why this task is held, failed, was refused, etc.
    # This applies to the current status.  Look at the history of the task to see reasons for past statuses.
    attr_accessor :statusReason                   # 0-1 CodeableConcept
    ##
    # E.g. "Specimen collected", "IV prepped"
    # Contains business-specific nuances of the business state.
    attr_accessor :businessStatus                 # 0-1 CodeableConcept
    ##
    # unknown | proposal | plan | order | original-order | reflex-order | filler-order | instance-order | option
    # Indicates the "level" of actionability associated with the Task, i.e. i+R[9]Cs this a proposed task, a planned task, an actionable task, etc.
    # This element is immutable.  Proposed tasks, planned tasks, etc. must be distinct instances.
    # 
    # In most cases, Tasks will have an intent of "order".
    attr_accessor :intent                         # 1-1 code
    ##
    # routine | urgent | asap | stat
    # Indicates how quickly the Task should be addressed with respect to other requests.
    attr_accessor :priority                       # 0-1 code
    ##
    # Task Type
    # A name or code (or both) briefly describing what the task involves.
    # The title (eg "My Tasks", "Outstanding Tasks for Patient X") should go into the code.
    attr_accessor :code                           # 0-1 CodeableConcept
    ##
    # Human-readable explanation of task
    # A free-text description of what is to be performed.
    attr_accessor :description                    # 0-1 string
    ##
    # What task is acting on
    # The request being actioned or the resource being manipulated by this task.
    # If multiple resources need to be manipulated, use sub-tasks.  (This ensures that status can be tracked independently for each referenced resource.).
    attr_accessor :focus                          # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Resource)
    ##
    # Beneficiary of the Task
    # The entity who benefits from the performance of the service specified in the task (e.g., the patient).
    attr_accessor :local_for                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Resource)
    ##
    # Healthcare event during which this task originated
    # The healthcare event  (e.g. a patient and healthcare provider interaction) during which this task was created.
    attr_accessor :encounter                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Encounter)
    ##
    # Start and end time of execution
    # Identifies the time action was first taken against the task (start) and/or the time final action was taken against the task prior to marking it as completed (end).
    attr_accessor :executionPeriod                # 0-1 Period
    ##
    # Task Creation Date
    # The date and time this task was created.
    attr_accessor :authoredOn                     # 0-1 dateTime
    ##
    # Task Last Modified Date
    # The date and time of last modification to this task.
    attr_accessor :lastModified                   # 0-1 dateTime
    ##
    # Who is asking for task to be done
    # The creator of the task.
    attr_accessor :requester                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/RelatedPerson)
    ##
    # Requested performer
    # The kind of participant that should perform the task.
    attr_accessor :performerType                  # 0-* [ CodeableConcept ]
    ##
    # Responsible individual
    # Individual organization or Device currently responsible for task execution.
    # Tasks may be created with an owner not yet identified.
    attr_accessor :owner                          # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/CareTeam|http://hl7.org/fhir/StructureDefinition/HealthcareService|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/RelatedPerson)
    ##
    # Where task occurs
    # Principal physical location where the this task is performed.
    attr_accessor :location                       # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Location)
    ##
    # Why task is needed
    # A description or code indicating why this task needs to be performed.
    # This should only be included if there is no focus or if it differs from the reason indicated on the focus.
    attr_accessor :reasonCode                     # 0-1 CodeableConcept
    ##
    # Why task is needed
    # A resource reference indicating why this task needs to be performed.
    # Tasks might be justified based on an Observation, a Condition, a past or planned procedure, etc.   This should only be included if there is no focus or if it differs from the reason indicated on the focus.    Use the CodeableConcept text element in `Task.reasonCode` if the data is free (uncoded) text.
    attr_accessor :reasonReference                # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Resource)
    ##
    # Associated insurance coverage
    # Insurance plans, coverage extensions, pre-authorizations and/or pre-determinations that may be relevant to the Task.
    attr_accessor :insurance                      # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Coverage|http://hl7.org/fhir/StructureDefinition/ClaimResponse) ]
    ##
    # Comments made about the task
    # Free-text information captured about the task as it progresses.
    attr_accessor :note                           # 0-* [ Annotation ]
    ##
    # Key events in history of the Task
    # Links to Provenance records for past versions of this Task that identify key state transitions or updates that are likely to be relevant to a user looking at the current version of the task.
    # This element does not point to the Provenance associated with the *current* version of the resource - as it would be created after this version existed.  The Provenance for the current version can be retrieved with a _revinclude.
    attr_accessor :relevantHistory                # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Provenance) ]
    ##
    # Constraints on fulfillment tasks
    # If the Task.focus is a request resource and the task is seeking fulfillment (i.e. is asking for the request to be actioned), this element identifies any limitations on what parts of the referenced request should be actioned.
    attr_accessor :restriction                    # 0-1 Task::Restriction
    ##
    # Information used to perform task
    # Additional information that may be needed in the execution of the task.
    attr_accessor :input                          # 0-* [ Task::Input ]
    ##
    # Information produced as part of task
    # Outputs produced by the Task.
    attr_accessor :output                         # 0-* [ Task::Output ]

    def resourceType
      'Task'
    end
  end
end
