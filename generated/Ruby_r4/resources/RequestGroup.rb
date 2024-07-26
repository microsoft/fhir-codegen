module FHIR

  ##
  # A group of related requests that can be used to capture intended activities that have inter-dependencies such as "give this medication after that one".
  class RequestGroup < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['author', 'authored', 'code', 'encounter', 'group-identifier', 'identifier', 'instantiates-canonical', 'instantiates-uri', 'intent', 'participant', 'patient', 'priority', 'status', 'subject']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'RequestGroup.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'RequestGroup.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'RequestGroup.implicitRules',
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
        'path'=>'RequestGroup.language',
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
        'path'=>'RequestGroup.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'RequestGroup.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'RequestGroup.extension',
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
        'path'=>'RequestGroup.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Business identifier
      # Allows a service to provide a unique, business identifier for the request.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'RequestGroup.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Instantiates FHIR protocol or definition
      # A canonical URL referencing a FHIR-defined protocol, guideline, orderset or other definition that is adhered to in whole or in part by this request.
      'instantiatesCanonical' => {
        'type'=>'canonical',
        'path'=>'RequestGroup.instantiatesCanonical',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Instantiates external protocol or definition
      # A URL referencing an externally defined protocol, guideline, orderset or other definition that is adhered to in whole or in part by this request.
      'instantiatesUri' => {
        'type'=>'uri',
        'path'=>'RequestGroup.instantiatesUri',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Fulfills plan, proposal, or order
      # A plan, proposal or order that is fulfilled in whole or in part by this request.
      'basedOn' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'Reference',
        'path'=>'RequestGroup.basedOn',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Request(s) replaced by this request
      # Completed or terminated request(s) whose function is taken by this new request.
      # The replacement could be because the initial request was immediately rejected (due to an issue) or because the previous request was completed, but the need for the action described by the request remains ongoing.
      'replaces' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'Reference',
        'path'=>'RequestGroup.replaces',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Composite request this is part of
      # A shared identifier common to all requests that were authorized more or less simultaneously by a single author, representing the identifier of the requisition, prescription or similar form.
      # Requests are linked either by a "basedOn" relationship (i.e. one request is fulfilling another) or by having a common requisition.  Requests that are part of the same requisition are generally treated independently from the perspective of changing their state or maintaining them after initial creation.
      'groupIdentifier' => {
        'type'=>'Identifier',
        'path'=>'RequestGroup.groupIdentifier',
        'min'=>0,
        'max'=>1
      },
      ##
      # draft | active | on-hold | revoked | completed | entered-in-error | unknown
      # The current state of the request. For request groups, the status reflects the status of all the requests in the group.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/request-status'=>[ 'draft', 'active', 'on-hold', 'revoked', 'completed', 'entered-in-error', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'RequestGroup.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/request-status'}
      },
      ##
      # proposal | plan | directive | order | original-order | reflex-order | filler-order | instance-order | option
      # Indicates the level of authority/intentionality associated with the request and where the request fits into the workflow chain.
      'intent' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/request-intent'=>[ 'proposal', 'plan', 'directive', 'order', 'original-order', 'reflex-order', 'filler-order', 'instance-order', 'option' ]
        },
        'type'=>'code',
        'path'=>'RequestGroup.intent',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/request-intent'}
      },
      ##
      # routine | urgent | asap | stat
      # Indicates how quickly the request should be addressed with respect to other requests.
      'priority' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/request-priority'=>[ 'routine', 'urgent', 'asap', 'stat' ]
        },
        'type'=>'code',
        'path'=>'RequestGroup.priority',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/request-priority'}
      },
      ##
      # What's being requested/ordered
      # A code that identifies what the overall request group is.
      # This element can be used to provide a code that captures the meaning of the request group as a whole, as opposed to the code of the action element, which captures the meaning of the individual actions within the request group.
      'code' => {
        'type'=>'CodeableConcept',
        'path'=>'RequestGroup.code',
        'min'=>0,
        'max'=>1
      },
      ##
      # Who the request group is about
      # The subject for which the request group was created.
      'subject' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Group'],
        'type'=>'Reference',
        'path'=>'RequestGroup.subject',
        'min'=>0,
        'max'=>1
      },
      ##
      # Created as part of
      # Describes the context of the request group, if any.
      'encounter' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Encounter'],
        'type'=>'Reference',
        'path'=>'RequestGroup.encounter',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the request group was authored
      # Indicates when the request group was created.
      'authoredOn' => {
        'type'=>'dateTime',
        'path'=>'RequestGroup.authoredOn',
        'min'=>0,
        'max'=>1
      },
      ##
      # Device or practitioner that authored the request group
      # Provides a reference to the author of the request group.
      'author' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole'],
        'type'=>'Reference',
        'path'=>'RequestGroup.author',
        'min'=>0,
        'max'=>1
      },
      ##
      # Why the request group is needed
      # Describes the reason for the request group in coded or textual form.
      'reasonCode' => {
        'type'=>'CodeableConcept',
        'path'=>'RequestGroup.reasonCode',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Why the request group is needed
      # Indicates another resource whose existence justifies this request group.
      'reasonReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Condition', 'http://hl7.org/fhir/StructureDefinition/Observation', 'http://hl7.org/fhir/StructureDefinition/DiagnosticReport', 'http://hl7.org/fhir/StructureDefinition/DocumentReference'],
        'type'=>'Reference',
        'path'=>'RequestGroup.reasonReference',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional notes about the response
      # Provides a mechanism to communicate additional information about the response.
      'note' => {
        'type'=>'Annotation',
        'path'=>'RequestGroup.note',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Proposed actions, if any
      # The actions, if any, produced by the evaluation of the artifact.
      'action' => {
        'type'=>'RequestGroup::Action',
        'path'=>'RequestGroup.action',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Proposed actions, if any
    # The actions, if any, produced by the evaluation of the artifact.
    class Action < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'timing[x]' => ['Age', 'dateTime', 'Duration', 'Period', 'Range', 'Timing']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Action.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Action.extension',
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
          'path'=>'Action.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # User-visible prefix for the action (e.g. 1. or A.)
        # A user-visible prefix for the action.
        'prefix' => {
          'type'=>'string',
          'path'=>'Action.prefix',
          'min'=>0,
          'max'=>1
        },
        ##
        # User-visible title
        # The title of the action displayed to a user.
        'title' => {
          'type'=>'string',
          'path'=>'Action.title',
          'min'=>0,
          'max'=>1
        },
        ##
        # Short description of the action
        # A short description of the action used to provide a summary to display to the user.
        'description' => {
          'type'=>'string',
          'path'=>'Action.description',
          'min'=>0,
          'max'=>1
        },
        ##
        # Static text equivalent of the action, used if the dynamic aspects cannot be interpreted by the receiving system
        # A text equivalent of the action to be performed. This provides a human-interpretable description of the action when the definition is consumed by a system that might not be capable of interpreting it dynamically.
        'textEquivalent' => {
          'type'=>'string',
          'path'=>'Action.textEquivalent',
          'min'=>0,
          'max'=>1
        },
        ##
        # routine | urgent | asap | stat
        # Indicates how quickly the action should be addressed with respect to other actions.
        'priority' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/request-priority'=>[ 'routine', 'urgent', 'asap', 'stat' ]
          },
          'type'=>'code',
          'path'=>'Action.priority',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/request-priority'}
        },
        ##
        # Code representing the meaning of the action or sub-actions
        # A code that provides meaning for the action or action group. For example, a section may have a LOINC code for a section of a documentation template.
        'code' => {
          'type'=>'CodeableConcept',
          'path'=>'Action.code',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Supporting documentation for the intended performer of the action
        # Didactic or other informational resources associated with the action that can be provided to the CDS recipient. Information resources can include inline text commentary and links to web resources.
        'documentation' => {
          'type'=>'RelatedArtifact',
          'path'=>'Action.documentation',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Whether or not the action is applicable
        # An expression that describes applicability criteria, or start/stop conditions for the action.
        # When multiple conditions of the same kind are present, the effects are combined using AND semantics, so the overall condition is true only if all of the conditions are true.
        'condition' => {
          'type'=>'RequestGroup::Action::Condition',
          'path'=>'Action.condition',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Relationship to another action
        # A relationship to another action such as "before" or "30-60 minutes after start of".
        'relatedAction' => {
          'type'=>'RequestGroup::Action::RelatedAction',
          'path'=>'Action.relatedAction',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # When the action should take place
        # An optional value describing when the action should be performed.
        'timingAge' => {
          'type'=>'Age',
          'path'=>'Action.timing[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # When the action should take place
        # An optional value describing when the action should be performed.
        'timingDateTime' => {
          'type'=>'DateTime',
          'path'=>'Action.timing[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # When the action should take place
        # An optional value describing when the action should be performed.
        'timingDuration' => {
          'type'=>'Duration',
          'path'=>'Action.timing[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # When the action should take place
        # An optional value describing when the action should be performed.
        'timingPeriod' => {
          'type'=>'Period',
          'path'=>'Action.timing[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # When the action should take place
        # An optional value describing when the action should be performed.
        'timingRange' => {
          'type'=>'Range',
          'path'=>'Action.timing[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # When the action should take place
        # An optional value describing when the action should be performed.
        'timingTiming' => {
          'type'=>'Timing',
          'path'=>'Action.timing[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Who should perform the action
        # The participant that should perform or be responsible for this action.
        'participant' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/Device'],
          'type'=>'Reference',
          'path'=>'Action.participant',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # create | update | remove | fire-event
        # The type of action to perform (create, update, remove).
        'type' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/action-type'=>[ 'create', 'update', 'remove', 'fire-event' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Action.type',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/action-type'}
        },
        ##
        # visual-group | logical-group | sentence-group
        # Defines the grouping behavior for the action and its children.
        'groupingBehavior' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/action-grouping-behavior'=>[ 'visual-group', 'logical-group', 'sentence-group' ]
          },
          'type'=>'code',
          'path'=>'Action.groupingBehavior',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/action-grouping-behavior'}
        },
        ##
        # any | all | all-or-none | exactly-one | at-most-one | one-or-more
        # Defines the selection behavior for the action and its children.
        'selectionBehavior' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/action-selection-behavior'=>[ 'any', 'all', 'all-or-none', 'exactly-one', 'at-most-one', 'one-or-more' ]
          },
          'type'=>'code',
          'path'=>'Action.selectionBehavior',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/action-selection-behavior'}
        },
        ##
        # must | could | must-unless-documented
        # Defines expectations around whether an action is required.
        'requiredBehavior' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/action-required-behavior'=>[ 'must', 'could', 'must-unless-documented' ]
          },
          'type'=>'code',
          'path'=>'Action.requiredBehavior',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/action-required-behavior'}
        },
        ##
        # yes | no
        # Defines whether the action should usually be preselected.
        'precheckBehavior' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/action-precheck-behavior'=>[ 'yes', 'no' ]
          },
          'type'=>'code',
          'path'=>'Action.precheckBehavior',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/action-precheck-behavior'}
        },
        ##
        # single | multiple
        # Defines whether the action can be selected multiple times.
        'cardinalityBehavior' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/action-cardinality-behavior'=>[ 'single', 'multiple' ]
          },
          'type'=>'code',
          'path'=>'Action.cardinalityBehavior',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/action-cardinality-behavior'}
        },
        ##
        # The target of the action
        # The resource that is the target of the action (e.g. CommunicationRequest).
        # The target resource SHALL be a [Request](request.html) resource with a Request.intent set to "option".
        'resource' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
          'type'=>'Reference',
          'path'=>'Action.resource',
          'min'=>0,
          'max'=>1
        },
        ##
        # Sub actions.
        'action' => {
          'type'=>'RequestGroup::Action',
          'path'=>'Action.action',
          'min'=>0,
          'max'=>Float::INFINITY
        }
      }

      ##
      # Whether or not the action is applicable
      # An expression that describes applicability criteria, or start/stop conditions for the action.
      # When multiple conditions of the same kind are present, the effects are combined using AND semantics, so the overall condition is true only if all of the conditions are true.
      class Condition < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

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
          # applicability | start | stop
          # The kind of condition.
          # Applicability criteria are used to determine immediate applicability when a plan definition is applied to a given context. Start and stop criteria are carried through application and used to describe enter/exit criteria for an action.
          'kind' => {
            'valid_codes'=>{
              'http://hl7.org/fhir/action-condition-kind'=>[ 'applicability', 'start', 'stop' ]
            },
            'type'=>'code',
            'path'=>'Condition.kind',
            'min'=>1,
            'max'=>1,
            'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/action-condition-kind'}
          },
          ##
          # Boolean-valued expression
          # An expression that returns true or false, indicating whether or not the condition is satisfied.
          # The expression may be inlined, or may be a reference to a named expression within a logic library referenced by the library element.
          'expression' => {
            'type'=>'Expression',
            'path'=>'Condition.expression',
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
        # applicability | start | stop
        # The kind of condition.
        # Applicability criteria are used to determine immediate applicability when a plan definition is applied to a given context. Start and stop criteria are carried through application and used to describe enter/exit criteria for an action.
        attr_accessor :kind                           # 1-1 code
        ##
        # Boolean-valued expression
        # An expression that returns true or false, indicating whether or not the condition is satisfied.
        # The expression may be inlined, or may be a reference to a named expression within a logic library referenced by the library element.
        attr_accessor :expression                     # 0-1 Expression
      end

      ##
      # Relationship to another action
      # A relationship to another action such as "before" or "30-60 minutes after start of".
      class RelatedAction < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        MULTIPLE_TYPES = {
          'offset[x]' => ['Duration', 'Range']
        }
        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'id',
            'path'=>'RelatedAction.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'RelatedAction.extension',
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
            'path'=>'RelatedAction.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # What action this is related to
          # The element id of the action this is related to.
          'actionId' => {
            'type'=>'id',
            'path'=>'RelatedAction.actionId',
            'min'=>1,
            'max'=>1
          },
          ##
          # before-start | before | before-end | concurrent-with-start | concurrent | concurrent-with-end | after-start | after | after-end
          # The relationship of this action to the related action.
          'relationship' => {
            'valid_codes'=>{
              'http://hl7.org/fhir/action-relationship-type'=>[ 'before-start', 'before', 'before-end', 'concurrent-with-start', 'concurrent', 'concurrent-with-end', 'after-start', 'after', 'after-end' ]
            },
            'type'=>'code',
            'path'=>'RelatedAction.relationship',
            'min'=>1,
            'max'=>1,
            'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/action-relationship-type'}
          },
          ##
          # Time offset for the relationship
          # A duration or range of durations to apply to the relationship. For example, 30-60 minutes before.
          'offsetDuration' => {
            'type'=>'Duration',
            'path'=>'RelatedAction.offset[x]',
            'min'=>0,
            'max'=>1
          }
          ##
          # Time offset for the relationship
          # A duration or range of durations to apply to the relationship. For example, 30-60 minutes before.
          'offsetRange' => {
            'type'=>'Range',
            'path'=>'RelatedAction.offset[x]',
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
        # What action this is related to
        # The element id of the action this is related to.
        attr_accessor :actionId                       # 1-1 id
        ##
        # before-start | before | before-end | concurrent-with-start | concurrent | concurrent-with-end | after-start | after | after-end
        # The relationship of this action to the related action.
        attr_accessor :relationship                   # 1-1 code
        ##
        # Time offset for the relationship
        # A duration or range of durations to apply to the relationship. For example, 30-60 minutes before.
        attr_accessor :offsetDuration                 # 0-1 Duration
        ##
        # Time offset for the relationship
        # A duration or range of durations to apply to the relationship. For example, 30-60 minutes before.
        attr_accessor :offsetRange                    # 0-1 Range
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
      # User-visible prefix for the action (e.g. 1. or A.)
      # A user-visible prefix for the action.
      attr_accessor :prefix                         # 0-1 string
      ##
      # User-visible title
      # The title of the action displayed to a user.
      attr_accessor :title                          # 0-1 string
      ##
      # Short description of the action
      # A short description of the action used to provide a summary to display to the user.
      attr_accessor :description                    # 0-1 string
      ##
      # Static text equivalent of the action, used if the dynamic aspects cannot be interpreted by the receiving system
      # A text equivalent of the action to be performed. This provides a human-interpretable description of the action when the definition is consumed by a system that might not be capable of interpreting it dynamically.
      attr_accessor :textEquivalent                 # 0-1 string
      ##
      # routine | urgent | asap | stat
      # Indicates how quickly the action should be addressed with respect to other actions.
      attr_accessor :priority                       # 0-1 code
      ##
      # Code representing the meaning of the action or sub-actions
      # A code that provides meaning for the action or action group. For example, a section may have a LOINC code for a section of a documentation template.
      attr_accessor :code                           # 0-* [ CodeableConcept ]
      ##
      # Supporting documentation for the intended performer of the action
      # Didactic or other informational resources associated with the action that can be provided to the CDS recipient. Information resources can include inline text commentary and links to web resources.
      attr_accessor :documentation                  # 0-* [ RelatedArtifact ]
      ##
      # Whether or not the action is applicable
      # An expression that describes applicability criteria, or start/stop conditions for the action.
      # When multiple conditions of the same kind are present, the effects are combined using AND semantics, so the overall condition is true only if all of the conditions are true.
      attr_accessor :condition                      # 0-* [ RequestGroup::Action::Condition ]
      ##
      # Relationship to another action
      # A relationship to another action such as "before" or "30-60 minutes after start of".
      attr_accessor :relatedAction                  # 0-* [ RequestGroup::Action::RelatedAction ]
      ##
      # When the action should take place
      # An optional value describing when the action should be performed.
      attr_accessor :timingAge                      # 0-1 Age
      ##
      # When the action should take place
      # An optional value describing when the action should be performed.
      attr_accessor :timingDateTime                 # 0-1 DateTime
      ##
      # When the action should take place
      # An optional value describing when the action should be performed.
      attr_accessor :timingDuration                 # 0-1 Duration
      ##
      # When the action should take place
      # An optional value describing when the action should be performed.
      attr_accessor :timingPeriod                   # 0-1 Period
      ##
      # When the action should take place
      # An optional value describing when the action should be performed.
      attr_accessor :timingRange                    # 0-1 Range
      ##
      # When the action should take place
      # An optional value describing when the action should be performed.
      attr_accessor :timingTiming                   # 0-1 Timing
      ##
      # Who should perform the action
      # The participant that should perform or be responsible for this action.
      attr_accessor :participant                    # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/Device) ]
      ##
      # create | update | remove | fire-event
      # The type of action to perform (create, update, remove).
      attr_accessor :type                           # 0-1 CodeableConcept
      ##
      # visual-group | logical-group | sentence-group
      # Defines the grouping behavior for the action and its children.
      attr_accessor :groupingBehavior               # 0-1 code
      ##
      # any | all | all-or-none | exactly-one | at-most-one | one-or-more
      # Defines the selection behavior for the action and its children.
      attr_accessor :selectionBehavior              # 0-1 code
      ##
      # must | could | must-unless-documented
      # Defines expectations around whether an action is required.
      attr_accessor :requiredBehavior               # 0-1 code
      ##
      # yes | no
      # Defines whether the action should usually be preselected.
      attr_accessor :precheckBehavior               # 0-1 code
      ##
      # single | multiple
      # Defines whether the action can be selected multiple times.
      attr_accessor :cardinalityBehavior            # 0-1 code
      ##
      # The target of the action
      # The resource that is the target of the action (e.g. CommunicationRequest).
      # The target resource SHALL be a [Request](request.html) resource with a Request.intent set to "option".
      attr_accessor :resource                       # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Resource)
      ##
      # Sub actions.
      attr_accessor :action                         # 0-* [ RequestGroup::Action ]
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
    # Business identifier
    # Allows a service to provide a unique, business identifier for the request.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Instantiates FHIR protocol or definition
    # A canonical URL referencing a FHIR-defined protocol, guideline, orderset or other definition that is adhered to in whole or in part by this request.
    attr_accessor :instantiatesCanonical          # 0-* [ canonical ]
    ##
    # Instantiates external protocol or definition
    # A URL referencing an externally defined protocol, guideline, orderset or other definition that is adhered to in whole or in part by this request.
    attr_accessor :instantiatesUri                # 0-* [ uri ]
    ##
    # Fulfills plan, proposal, or order
    # A plan, proposal or order that is fulfilled in whole or in part by this request.
    attr_accessor :basedOn                        # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Resource) ]
    ##
    # Request(s) replaced by this request
    # Completed or terminated request(s) whose function is taken by this new request.
    # The replacement could be because the initial request was immediately rejected (due to an issue) or because the previous request was completed, but the need for the action described by the request remains ongoing.
    attr_accessor :replaces                       # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Resource) ]
    ##
    # Composite request this is part of
    # A shared identifier common to all requests that were authorized more or less simultaneously by a single author, representing the identifier of the requisition, prescription or similar form.
    # Requests are linked either by a "basedOn" relationship (i.e. one request is fulfilling another) or by having a common requisition.  Requests that are part of the same requisition are generally treated independently from the perspective of changing their state or maintaining them after initial creation.
    attr_accessor :groupIdentifier                # 0-1 Identifier
    ##
    # draft | active | on-hold | revoked | completed | entered-in-error | unknown
    # The current state of the request. For request groups, the status reflects the status of all the requests in the group.
    attr_accessor :status                         # 1-1 code
    ##
    # proposal | plan | directive | order | original-order | reflex-order | filler-order | instance-order | option
    # Indicates the level of authority/intentionality associated with the request and where the request fits into the workflow chain.
    attr_accessor :intent                         # 1-1 code
    ##
    # routine | urgent | asap | stat
    # Indicates how quickly the request should be addressed with respect to other requests.
    attr_accessor :priority                       # 0-1 code
    ##
    # What's being requested/ordered
    # A code that identifies what the overall request group is.
    # This element can be used to provide a code that captures the meaning of the request group as a whole, as opposed to the code of the action element, which captures the meaning of the individual actions within the request group.
    attr_accessor :code                           # 0-1 CodeableConcept
    ##
    # Who the request group is about
    # The subject for which the request group was created.
    attr_accessor :subject                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Group)
    ##
    # Created as part of
    # Describes the context of the request group, if any.
    attr_accessor :encounter                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Encounter)
    ##
    # When the request group was authored
    # Indicates when the request group was created.
    attr_accessor :authoredOn                     # 0-1 dateTime
    ##
    # Device or practitioner that authored the request group
    # Provides a reference to the author of the request group.
    attr_accessor :author                         # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole)
    ##
    # Why the request group is needed
    # Describes the reason for the request group in coded or textual form.
    attr_accessor :reasonCode                     # 0-* [ CodeableConcept ]
    ##
    # Why the request group is needed
    # Indicates another resource whose existence justifies this request group.
    attr_accessor :reasonReference                # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Condition|http://hl7.org/fhir/StructureDefinition/Observation|http://hl7.org/fhir/StructureDefinition/DiagnosticReport|http://hl7.org/fhir/StructureDefinition/DocumentReference) ]
    ##
    # Additional notes about the response
    # Provides a mechanism to communicate additional information about the response.
    attr_accessor :note                           # 0-* [ Annotation ]
    ##
    # Proposed actions, if any
    # The actions, if any, produced by the evaluation of the artifact.
    attr_accessor :action                         # 0-* [ RequestGroup::Action ]

    def resourceType
      'RequestGroup'
    end
  end
end
