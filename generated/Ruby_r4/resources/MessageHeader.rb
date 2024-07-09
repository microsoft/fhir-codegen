module FHIR

  ##
  # The header for a message exchange that is either requesting or responding to an action.  The reference(s) that are the subject of the action as well as other information related to the action are typically transmitted in a bundle in which the MessageHeader resource instance is the first resource in the bundle.
  # Many implementations are not prepared to use REST and need a messaging based infrastructure.
  class MessageHeader < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['author', 'code', 'destination-uri', 'destination', 'enterer', 'event', 'focus', 'receiver', 'response-id', 'responsible', 'sender', 'source-uri', 'source', 'target']
    MULTIPLE_TYPES = {
      'event[x]' => ['Coding', 'uri']
    }
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'MessageHeader.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'MessageHeader.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'MessageHeader.implicitRules',
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
        'path'=>'MessageHeader.language',
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
        'path'=>'MessageHeader.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'MessageHeader.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'MessageHeader.extension',
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
        'path'=>'MessageHeader.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Code for the event this message represents or link to event definition
      # Code that identifies the event this message represents and connects it with its definition. Events defined as part of the FHIR specification have the system value "http://terminology.hl7.org/CodeSystem/message-events".  Alternatively uri to the EventDefinition.
      # The time of the event will be found in the focus resource. The time of the message will be found in [Bundle.timestamp](bundle-definitions.html#Bundle.timestamp).
      'eventCoding' => {
        'valid_codes'=>{
        },
        'type'=>'Coding',
        'path'=>'MessageHeader.event[x]',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/message-events'}
      },
      ##
      # Code for the event this message represents or link to event definition
      # Code that identifies the event this message represents and connects it with its definition. Events defined as part of the FHIR specification have the system value "http://terminology.hl7.org/CodeSystem/message-events".  Alternatively uri to the EventDefinition.
      # The time of the event will be found in the focus resource. The time of the message will be found in [Bundle.timestamp](bundle-definitions.html#Bundle.timestamp).
      'eventUri' => {
        'valid_codes'=>{
        },
        'type'=>'Uri',
        'path'=>'MessageHeader.event[x]',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/message-events'}
      },
      ##
      # Message destination application(s)
      # The destination application which the message is intended for.
      # There SHOULD be at least one destination, but in some circumstances, the source system is unaware of any particular destination system.
      'destination' => {
        'type'=>'MessageHeader::Destination',
        'path'=>'MessageHeader.destination',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Real world sender of the message
      # Identifies the sending system to allow the use of a trust relationship.
      # Use case is for where a (trusted) sending system is responsible for multiple organizations, and therefore cannot differentiate based on source endpoint / authentication alone.
      'sender' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'MessageHeader.sender',
        'min'=>0,
        'max'=>1
      },
      ##
      # The source of the data entry
      # The person or device that performed the data entry leading to this message. When there is more than one candidate, pick the most proximal to the message. Can provide other enterers in extensions.
      # Usually only for the request but can be used in a response.
      'enterer' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole'],
        'type'=>'Reference',
        'path'=>'MessageHeader.enterer',
        'min'=>0,
        'max'=>1
      },
      ##
      # The source of the decision
      # The logical author of the message - the person or device that decided the described event should happen. When there is more than one candidate, pick the most proximal to the MessageHeader. Can provide other authors in extensions.
      # Usually only for the request but can be used in a response.
      'author' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole'],
        'type'=>'Reference',
        'path'=>'MessageHeader.author',
        'min'=>0,
        'max'=>1
      },
      ##
      # Message source application
      # The source application from which this message originated.
      'source' => {
        'type'=>'MessageHeader::Source',
        'path'=>'MessageHeader.source',
        'min'=>1,
        'max'=>1
      },
      ##
      # Final responsibility for event
      # The person or organization that accepts overall responsibility for the contents of the message. The implication is that the message event happened under the policies of the responsible party.
      # Usually only for the request but can be used in a response.
      'responsible' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'MessageHeader.responsible',
        'min'=>0,
        'max'=>1
      },
      ##
      # Cause of event
      # Coded indication of the cause for the event - indicates  a reason for the occurrence of the event that is a focus of this message.
      'reason' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/message-reasons-encounter'=>[ 'admit', 'discharge', 'absent', 'return', 'moved', 'edit' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'MessageHeader.reason',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/message-reason-encounter'}
      },
      ##
      # If this is a reply to prior message
      # Information about the message that this message is a response to.  Only present if this message is a response.
      'response' => {
        'type'=>'MessageHeader::Response',
        'path'=>'MessageHeader.response',
        'min'=>0,
        'max'=>1
      },
      ##
      # The actual content of the message
      # The actual data of the message - a reference to the root/focus class of the event.
      # The data is defined where the transaction type is defined. The transaction data is always included in the bundle that is the full message.  Only the root resource is specified.  The resources it references should be contained in the bundle but are not also listed here.  Multiple repetitions are allowed to cater for merges and other situations with multiple focal targets.
      'focus' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'Reference',
        'path'=>'MessageHeader.focus',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Link to the definition for this message
      # Permanent link to the MessageDefinition for this message.
      'definition' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/MessageDefinition'],
        'type'=>'canonical',
        'path'=>'MessageHeader.definition',
        'min'=>0,
        'max'=>1
      }
    }

    ##
    # Message destination application(s)
    # The destination application which the message is intended for.
    # There SHOULD be at least one destination, but in some circumstances, the source system is unaware of any particular destination system.
    class Destination < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Destination.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Destination.extension',
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
          'path'=>'Destination.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Name of system
        # Human-readable name for the target system.
        'name' => {
          'type'=>'string',
          'path'=>'Destination.name',
          'min'=>0,
          'max'=>1
        },
        ##
        # Particular delivery destination within the destination
        # Identifies the target end system in situations where the initial message transmission is to an intermediary system.
        'target' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Device'],
          'type'=>'Reference',
          'path'=>'Destination.target',
          'min'=>0,
          'max'=>1
        },
        ##
        # Actual destination address or id
        # Indicates where the message should be routed to.
        # The id may be a non-resolvable URI for systems that do not use standard network-based addresses.
        'endpoint' => {
          'type'=>'url',
          'path'=>'Destination.endpoint',
          'min'=>1,
          'max'=>1
        },
        ##
        # Intended "real-world" recipient for the data
        # Allows data conveyed by a message to be addressed to a particular person or department when routing to a specific application isn't sufficient.
        'receiver' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization'],
          'type'=>'Reference',
          'path'=>'Destination.receiver',
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
      # Name of system
      # Human-readable name for the target system.
      attr_accessor :name                           # 0-1 string
      ##
      # Particular delivery destination within the destination
      # Identifies the target end system in situations where the initial message transmission is to an intermediary system.
      attr_accessor :target                         # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Device)
      ##
      # Actual destination address or id
      # Indicates where the message should be routed to.
      # The id may be a non-resolvable URI for systems that do not use standard network-based addresses.
      attr_accessor :endpoint                       # 1-1 url
      ##
      # Intended "real-world" recipient for the data
      # Allows data conveyed by a message to be addressed to a particular person or department when routing to a specific application isn't sufficient.
      attr_accessor :receiver                       # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization)
    end

    ##
    # Message source application
    # The source application from which this message originated.
    class Source < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Source.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Source.extension',
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
          'path'=>'Source.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Name of system
        # Human-readable name for the source system.
        'name' => {
          'type'=>'string',
          'path'=>'Source.name',
          'min'=>0,
          'max'=>1
        },
        ##
        # Name of software running the system
        # May include configuration or other information useful in debugging.
        'software' => {
          'type'=>'string',
          'path'=>'Source.software',
          'min'=>0,
          'max'=>1
        },
        ##
        # Version of software running
        # Can convey versions of multiple systems in situations where a message passes through multiple hands.
        'version' => {
          'type'=>'string',
          'path'=>'Source.version',
          'min'=>0,
          'max'=>1
        },
        ##
        # Human contact for problems
        # An e-mail, phone, website or other contact point to use to resolve issues with message communications.
        'contact' => {
          'type'=>'ContactPoint',
          'path'=>'Source.contact',
          'min'=>0,
          'max'=>1
        },
        ##
        # Actual message source address or id
        # Identifies the routing target to send acknowledgements to.
        # The id may be a non-resolvable URI for systems that do not use standard network-based addresses.
        'endpoint' => {
          'type'=>'url',
          'path'=>'Source.endpoint',
          'min'=>1,
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
      # Name of system
      # Human-readable name for the source system.
      attr_accessor :name                           # 0-1 string
      ##
      # Name of software running the system
      # May include configuration or other information useful in debugging.
      attr_accessor :software                       # 0-1 string
      ##
      # Version of software running
      # Can convey versions of multiple systems in situations where a message passes through multiple hands.
      attr_accessor :version                        # 0-1 string
      ##
      # Human contact for problems
      # An e-mail, phone, website or other contact point to use to resolve issues with message communications.
      attr_accessor :contact                        # 0-1 ContactPoint
      ##
      # Actual message source address or id
      # Identifies the routing target to send acknowledgements to.
      # The id may be a non-resolvable URI for systems that do not use standard network-based addresses.
      attr_accessor :endpoint                       # 1-1 url
    end

    ##
    # If this is a reply to prior message
    # Information about the message that this message is a response to.  Only present if this message is a response.
    class Response < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Response.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Response.extension',
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
          'path'=>'Response.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Id of original message
        # The MessageHeader.id of the message to which this message is a response.
        'identifier' => {
          'type'=>'id',
          'path'=>'Response.identifier',
          'min'=>1,
          'max'=>1
        },
        ##
        # ok | transient-error | fatal-error
        # Code that identifies the type of response to the message - whether it was successful or not, and whether it should be resent or not.
        # This is a generic response to the request message. Specific data for the response will be found in MessageHeader.focus.
        'code' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/response-code'=>[ 'ok', 'transient-error', 'fatal-error' ]
          },
          'type'=>'code',
          'path'=>'Response.code',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/response-code'}
        },
        ##
        # Specific list of hints/warnings/errors
        # Full details of any issues found in the message.
        # This SHALL be contained in the bundle. If any of the issues are errors, the response code SHALL be an error.
        'details' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/OperationOutcome'],
          'type'=>'Reference',
          'path'=>'Response.details',
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
      # Id of original message
      # The MessageHeader.id of the message to which this message is a response.
      attr_accessor :identifier                     # 1-1 id
      ##
      # ok | transient-error | fatal-error
      # Code that identifies the type of response to the message - whether it was successful or not, and whether it should be resent or not.
      # This is a generic response to the request message. Specific data for the response will be found in MessageHeader.focus.
      attr_accessor :code                           # 1-1 code
      ##
      # Specific list of hints/warnings/errors
      # Full details of any issues found in the message.
      # This SHALL be contained in the bundle. If any of the issues are errors, the response code SHALL be an error.
      attr_accessor :details                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/OperationOutcome)
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
    # Code for the event this message represents or link to event definition
    # Code that identifies the event this message represents and connects it with its definition. Events defined as part of the FHIR specification have the system value "http://terminology.hl7.org/CodeSystem/message-events".  Alternatively uri to the EventDefinition.
    # The time of the event will be found in the focus resource. The time of the message will be found in [Bundle.timestamp](bundle-definitions.html#Bundle.timestamp).
    attr_accessor :eventCoding                    # 1-1 Coding
    ##
    # Code for the event this message represents or link to event definition
    # Code that identifies the event this message represents and connects it with its definition. Events defined as part of the FHIR specification have the system value "http://terminology.hl7.org/CodeSystem/message-events".  Alternatively uri to the EventDefinition.
    # The time of the event will be found in the focus resource. The time of the message will be found in [Bundle.timestamp](bundle-definitions.html#Bundle.timestamp).
    attr_accessor :eventUri                       # 1-1 Uri
    ##
    # Message destination application(s)
    # The destination application which the message is intended for.
    # There SHOULD be at least one destination, but in some circumstances, the source system is unaware of any particular destination system.
    attr_accessor :destination                    # 0-* [ MessageHeader::Destination ]
    ##
    # Real world sender of the message
    # Identifies the sending system to allow the use of a trust relationship.
    # Use case is for where a (trusted) sending system is responsible for multiple organizations, and therefore cannot differentiate based on source endpoint / authentication alone.
    attr_accessor :sender                         # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # The source of the data entry
    # The person or device that performed the data entry leading to this message. When there is more than one candidate, pick the most proximal to the message. Can provide other enterers in extensions.
    # Usually only for the request but can be used in a response.
    attr_accessor :enterer                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole)
    ##
    # The source of the decision
    # The logical author of the message - the person or device that decided the described event should happen. When there is more than one candidate, pick the most proximal to the MessageHeader. Can provide other authors in extensions.
    # Usually only for the request but can be used in a response.
    attr_accessor :author                         # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole)
    ##
    # Message source application
    # The source application from which this message originated.
    attr_accessor :source                         # 1-1 MessageHeader::Source
    ##
    # Final responsibility for event
    # The person or organization that accepts overall responsibility for the contents of the message. The implication is that the message event happened under the policies of the responsible party.
    # Usually only for the request but can be used in a response.
    attr_accessor :responsible                    # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Cause of event
    # Coded indication of the cause for the event - indicates  a reason for the occurrence of the event that is a focus of this message.
    attr_accessor :reason                         # 0-1 CodeableConcept
    ##
    # If this is a reply to prior message
    # Information about the message that this message is a response to.  Only present if this message is a response.
    attr_accessor :response                       # 0-1 MessageHeader::Response
    ##
    # The actual content of the message
    # The actual data of the message - a reference to the root/focus class of the event.
    # The data is defined where the transaction type is defined. The transaction data is always included in the bundle that is the full message.  Only the root resource is specified.  The resources it references should be contained in the bundle but are not also listed here.  Multiple repetitions are allowed to cater for merges and other situations with multiple focal targets.
    attr_accessor :focus                          # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Resource) ]
    ##
    # Link to the definition for this message
    # Permanent link to the MessageDefinition for this message.
    attr_accessor :definition                     # 0-1 canonical(http://hl7.org/fhir/StructureDefinition/MessageDefinition)

    def resourceType
      'MessageHeader'
    end
  end
end
