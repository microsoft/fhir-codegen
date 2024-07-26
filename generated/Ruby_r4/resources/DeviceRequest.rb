module FHIR

  ##
  # Represents a request for a patient to employ a medical device. The device may be an implantable device, or an external assistive device, such as a walker.
  class DeviceRequest < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['authored-on', 'based-on', 'code', 'device', 'encounter', 'event-date', 'group-identifier', 'identifier', 'instantiates-canonical', 'instantiates-uri', 'insurance', 'intent', 'patient', 'performer', 'prior-request', 'requester', 'status', 'subject']
    MULTIPLE_TYPES = {
      'code[x]' => ['CodeableConcept', 'Reference'],
      'occurrence[x]' => ['dateTime', 'Period', 'Timing']
    }
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'DeviceRequest.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'DeviceRequest.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'DeviceRequest.implicitRules',
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
        'path'=>'DeviceRequest.language',
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
        'path'=>'DeviceRequest.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'DeviceRequest.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'DeviceRequest.extension',
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
        'path'=>'DeviceRequest.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # External Request identifier
      # Identifiers assigned to this order by the orderer or by the receiver.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'DeviceRequest.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Instantiates FHIR protocol or definition
      # The URL pointing to a FHIR-defined protocol, guideline, orderset or other definition that is adhered to in whole or in part by this DeviceRequest.
      # Note: This is a business identifier, not a resource identifier (see [discussion](resource.html#identifiers)).  It is best practice for the identifier to only appear on a single resource instance, however business practices may occasionally dictate that multiple resource instances with the same identifier can exist - possibly even with different resource types.  For example, multiple Patient and a Person resource instance might share the same social insurance number.
      'instantiatesCanonical' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ActivityDefinition', 'http://hl7.org/fhir/StructureDefinition/PlanDefinition'],
        'type'=>'canonical',
        'path'=>'DeviceRequest.instantiatesCanonical',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Instantiates external protocol or definition
      # The URL pointing to an externally maintained protocol, guideline, orderset or other definition that is adhered to in whole or in part by this DeviceRequest.
      # This might be an HTML page, PDF, etc. or could just be a non-resolvable URI identifier.
      'instantiatesUri' => {
        'type'=>'uri',
        'path'=>'DeviceRequest.instantiatesUri',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # What request fulfills
      # Plan/proposal/order fulfilled by this request.
      'basedOn' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'Reference',
        'path'=>'DeviceRequest.basedOn',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # What request replaces
      # The request takes the place of the referenced completed or terminated request(s).
      'priorRequest' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'Reference',
        'path'=>'DeviceRequest.priorRequest',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Identifier of composite request
      # Composite request this is part of.
      'groupIdentifier' => {
        'type'=>'Identifier',
        'path'=>'DeviceRequest.groupIdentifier',
        'min'=>0,
        'max'=>1
      },
      ##
      # draft | active | on-hold | revoked | completed | entered-in-error | unknown
      # The status of the request.
      # This element is labeled as a modifier because the status contains the codes cancelled and entered-in-error that mark the request as not currently valid.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/request-status'=>[ 'draft', 'active', 'on-hold', 'revoked', 'completed', 'entered-in-error', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'DeviceRequest.status',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/request-status'}
      },
      ##
      # proposal | plan | directive | order | original-order | reflex-order | filler-order | instance-order | option
      # Whether the request is a proposal, plan, an original order or a reflex order.
      'intent' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/request-intent'=>[ 'proposal', 'plan', 'directive', 'order', 'original-order', 'reflex-order', 'filler-order', 'instance-order', 'option' ]
        },
        'type'=>'code',
        'path'=>'DeviceRequest.intent',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/request-intent'}
      },
      ##
      # routine | urgent | asap | stat
      # Indicates how quickly the {{title}} should be addressed with respect to other requests.
      'priority' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/request-priority'=>[ 'routine', 'urgent', 'asap', 'stat' ]
        },
        'type'=>'code',
        'path'=>'DeviceRequest.priority',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/request-priority'}
      },
      ##
      # Device requested
      # The details of the device to be used.
      'codeCodeableConcept' => {
        'type'=>'CodeableConcept',
        'path'=>'DeviceRequest.code[x]',
        'min'=>1,
        'max'=>1
      },
      ##
      # Device requested
      # The details of the device to be used.
      'codeReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Device'],
        'type'=>'Reference',
        'path'=>'DeviceRequest.code[x]',
        'min'=>1,
        'max'=>1
      },
      ##
      # Device details
      # Specific parameters for the ordered item.  For example, the prism value for lenses.
      'parameter' => {
        'type'=>'DeviceRequest::Parameter',
        'path'=>'DeviceRequest.parameter',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Focus of request
      # The patient who will use the device.
      'subject' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Group', 'http://hl7.org/fhir/StructureDefinition/Location', 'http://hl7.org/fhir/StructureDefinition/Device'],
        'type'=>'Reference',
        'path'=>'DeviceRequest.subject',
        'min'=>1,
        'max'=>1
      },
      ##
      # Encounter motivating request
      # An encounter that provides additional context in which this request is made.
      'encounter' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Encounter'],
        'type'=>'Reference',
        'path'=>'DeviceRequest.encounter',
        'min'=>0,
        'max'=>1
      },
      ##
      # Desired time or schedule for use
      # The timing schedule for the use of the device. The Schedule data type allows many different expressions, for example. "Every 8 hours"; "Three times a day"; "1/2 an hour before breakfast for 10 days from 23-Dec 2011:"; "15 Oct 2013, 17 Oct 2013 and 1 Nov 2013".
      'occurrenceDateTime' => {
        'type'=>'DateTime',
        'path'=>'DeviceRequest.occurrence[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Desired time or schedule for use
      # The timing schedule for the use of the device. The Schedule data type allows many different expressions, for example. "Every 8 hours"; "Three times a day"; "1/2 an hour before breakfast for 10 days from 23-Dec 2011:"; "15 Oct 2013, 17 Oct 2013 and 1 Nov 2013".
      'occurrencePeriod' => {
        'type'=>'Period',
        'path'=>'DeviceRequest.occurrence[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Desired time or schedule for use
      # The timing schedule for the use of the device. The Schedule data type allows many different expressions, for example. "Every 8 hours"; "Three times a day"; "1/2 an hour before breakfast for 10 days from 23-Dec 2011:"; "15 Oct 2013, 17 Oct 2013 and 1 Nov 2013".
      'occurrenceTiming' => {
        'type'=>'Timing',
        'path'=>'DeviceRequest.occurrence[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # When recorded
      # When the request transitioned to being actionable.
      'authoredOn' => {
        'type'=>'dateTime',
        'path'=>'DeviceRequest.authoredOn',
        'min'=>0,
        'max'=>1
      },
      ##
      # Who/what is requesting diagnostics
      # The individual who initiated the request and has responsibility for its activation.
      'requester' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'DeviceRequest.requester',
        'min'=>0,
        'max'=>1
      },
      ##
      # Filler role
      # Desired type of performer for doing the diagnostic testing.
      'performerType' => {
        'type'=>'CodeableConcept',
        'path'=>'DeviceRequest.performerType',
        'min'=>0,
        'max'=>1
      },
      ##
      # Requested Filler
      # The desired performer for doing the diagnostic testing.
      'performer' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/CareTeam', 'http://hl7.org/fhir/StructureDefinition/HealthcareService', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson'],
        'type'=>'Reference',
        'path'=>'DeviceRequest.performer',
        'min'=>0,
        'max'=>1
      },
      ##
      # Coded Reason for request
      # Reason or justification for the use of this device.
      'reasonCode' => {
        'type'=>'CodeableConcept',
        'path'=>'DeviceRequest.reasonCode',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Linked Reason for request
      # Reason or justification for the use of this device.
      'reasonReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Condition', 'http://hl7.org/fhir/StructureDefinition/Observation', 'http://hl7.org/fhir/StructureDefinition/DiagnosticReport', 'http://hl7.org/fhir/StructureDefinition/DocumentReference'],
        'type'=>'Reference',
        'path'=>'DeviceRequest.reasonReference',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Associated insurance coverage
      # Insurance plans, coverage extensions, pre-authorizations and/or pre-determinations that may be required for delivering the requested service.
      'insurance' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Coverage', 'http://hl7.org/fhir/StructureDefinition/ClaimResponse'],
        'type'=>'Reference',
        'path'=>'DeviceRequest.insurance',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional clinical information about the patient that may influence the request fulfilment.  For example, this may include where on the subject's body the device will be used (i.e. the target site).
      'supportingInfo' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'Reference',
        'path'=>'DeviceRequest.supportingInfo',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Notes or comments
      # Details about this request that were not represented at all or sufficiently in one of the attributes provided in a class. These may include for example a comment, an instruction, or a note associated with the statement.
      'note' => {
        'type'=>'Annotation',
        'path'=>'DeviceRequest.note',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Request provenance
      # Key events in the history of the request.
      # This might not include provenances for all versions of the request - only those deemed "relevant" or important.This SHALL NOT include the Provenance associated with this current version of the resource.  (If that provenance is deemed to be a "relevant" change, it will need to be added as part of a later update.  Until then, it can be queried directly as the Provenance that points to this version using _revincludeAll Provenances should have some historical version of this Request as their subject.
      'relevantHistory' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Provenance'],
        'type'=>'Reference',
        'path'=>'DeviceRequest.relevantHistory',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Device details
    # Specific parameters for the ordered item.  For example, the prism value for lenses.
    class Parameter < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'value[x]' => ['boolean', 'CodeableConcept', 'Quantity', 'Range']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Parameter.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Parameter.extension',
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
          'path'=>'Parameter.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Device detail
        # A code or string that identifies the device detail being asserted.
        'code' => {
          'type'=>'CodeableConcept',
          'path'=>'Parameter.code',
          'min'=>0,
          'max'=>1
        },
        ##
        # Value of detail
        # The value of the device detail.
        # Range means device should have a value that falls somewhere within the specified range.
        'valueBoolean' => {
          'type'=>'Boolean',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        }
        ##
        # Value of detail
        # The value of the device detail.
        # Range means device should have a value that falls somewhere within the specified range.
        'valueCodeableConcept' => {
          'type'=>'CodeableConcept',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        }
        ##
        # Value of detail
        # The value of the device detail.
        # Range means device should have a value that falls somewhere within the specified range.
        'valueQuantity' => {
          'type'=>'Quantity',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        }
        ##
        # Value of detail
        # The value of the device detail.
        # Range means device should have a value that falls somewhere within the specified range.
        'valueRange' => {
          'type'=>'Range',
          'path'=>'Parameter.value[x]',
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
      # Device detail
      # A code or string that identifies the device detail being asserted.
      attr_accessor :code                           # 0-1 CodeableConcept
      ##
      # Value of detail
      # The value of the device detail.
      # Range means device should have a value that falls somewhere within the specified range.
      attr_accessor :valueBoolean                   # 0-1 Boolean
      ##
      # Value of detail
      # The value of the device detail.
      # Range means device should have a value that falls somewhere within the specified range.
      attr_accessor :valueCodeableConcept           # 0-1 CodeableConcept
      ##
      # Value of detail
      # The value of the device detail.
      # Range means device should have a value that falls somewhere within the specified range.
      attr_accessor :valueQuantity                  # 0-1 Quantity
      ##
      # Value of detail
      # The value of the device detail.
      # Range means device should have a value that falls somewhere within the specified range.
      attr_accessor :valueRange                     # 0-1 Range
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
    # External Request identifier
    # Identifiers assigned to this order by the orderer or by the receiver.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Instantiates FHIR protocol or definition
    # The URL pointing to a FHIR-defined protocol, guideline, orderset or other definition that is adhered to in whole or in part by this DeviceRequest.
    # Note: This is a business identifier, not a resource identifier (see [discussion](resource.html#identifiers)).  It is best practice for the identifier to only appear on a single resource instance, however business practices may occasionally dictate that multiple resource instances with the same identifier can exist - possibly even with different resource types.  For example, multiple Patient and a Person resource instance might share the same social insurance number.
    attr_accessor :instantiatesCanonical          # 0-* [ canonical(http://hl7.org/fhir/StructureDefinition/ActivityDefinition|http://hl7.org/fhir/StructureDefinition/PlanDefinition) ]
    ##
    # Instantiates external protocol or definition
    # The URL pointing to an externally maintained protocol, guideline, orderset or other definition that is adhered to in whole or in part by this DeviceRequest.
    # This might be an HTML page, PDF, etc. or could just be a non-resolvable URI identifier.
    attr_accessor :instantiatesUri                # 0-* [ uri ]
    ##
    # What request fulfills
    # Plan/proposal/order fulfilled by this request.
    attr_accessor :basedOn                        # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Resource) ]
    ##
    # What request replaces
    # The request takes the place of the referenced completed or terminated request(s).
    attr_accessor :priorRequest                   # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Resource) ]
    ##
    # Identifier of composite request
    # Composite request this is part of.
    attr_accessor :groupIdentifier                # 0-1 Identifier
    ##
    # draft | active | on-hold | revoked | completed | entered-in-error | unknown
    # The status of the request.
    # This element is labeled as a modifier because the status contains the codes cancelled and entered-in-error that mark the request as not currently valid.
    attr_accessor :status                         # 0-1 code
    ##
    # proposal | plan | directive | order | original-order | reflex-order | filler-order | instance-order | option
    # Whether the request is a proposal, plan, an original order or a reflex order.
    attr_accessor :intent                         # 1-1 code
    ##
    # routine | urgent | asap | stat
    # Indicates how quickly the {{title}} should be addressed with respect to other requests.
    attr_accessor :priority                       # 0-1 code
    ##
    # Device requested
    # The details of the device to be used.
    attr_accessor :codeCodeableConcept            # 1-1 CodeableConcept
    ##
    # Device requested
    # The details of the device to be used.
    attr_accessor :codeReference                  # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Device)
    ##
    # Device details
    # Specific parameters for the ordered item.  For example, the prism value for lenses.
    attr_accessor :parameter                      # 0-* [ DeviceRequest::Parameter ]
    ##
    # Focus of request
    # The patient who will use the device.
    attr_accessor :subject                        # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Group|http://hl7.org/fhir/StructureDefinition/Location|http://hl7.org/fhir/StructureDefinition/Device)
    ##
    # Encounter motivating request
    # An encounter that provides additional context in which this request is made.
    attr_accessor :encounter                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Encounter)
    ##
    # Desired time or schedule for use
    # The timing schedule for the use of the device. The Schedule data type allows many different expressions, for example. "Every 8 hours"; "Three times a day"; "1/2 an hour before breakfast for 10 days from 23-Dec 2011:"; "15 Oct 2013, 17 Oct 2013 and 1 Nov 2013".
    attr_accessor :occurrenceDateTime             # 0-1 DateTime
    ##
    # Desired time or schedule for use
    # The timing schedule for the use of the device. The Schedule data type allows many different expressions, for example. "Every 8 hours"; "Three times a day"; "1/2 an hour before breakfast for 10 days from 23-Dec 2011:"; "15 Oct 2013, 17 Oct 2013 and 1 Nov 2013".
    attr_accessor :occurrencePeriod               # 0-1 Period
    ##
    # Desired time or schedule for use
    # The timing schedule for the use of the device. The Schedule data type allows many different expressions, for example. "Every 8 hours"; "Three times a day"; "1/2 an hour before breakfast for 10 days from 23-Dec 2011:"; "15 Oct 2013, 17 Oct 2013 and 1 Nov 2013".
    attr_accessor :occurrenceTiming               # 0-1 Timing
    ##
    # When recorded
    # When the request transitioned to being actionable.
    attr_accessor :authoredOn                     # 0-1 dateTime
    ##
    # Who/what is requesting diagnostics
    # The individual who initiated the request and has responsibility for its activation.
    attr_accessor :requester                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Filler role
    # Desired type of performer for doing the diagnostic testing.
    attr_accessor :performerType                  # 0-1 CodeableConcept
    ##
    # Requested Filler
    # The desired performer for doing the diagnostic testing.
    attr_accessor :performer                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/CareTeam|http://hl7.org/fhir/StructureDefinition/HealthcareService|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/RelatedPerson)
    ##
    # Coded Reason for request
    # Reason or justification for the use of this device.
    attr_accessor :reasonCode                     # 0-* [ CodeableConcept ]
    ##
    # Linked Reason for request
    # Reason or justification for the use of this device.
    attr_accessor :reasonReference                # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Condition|http://hl7.org/fhir/StructureDefinition/Observation|http://hl7.org/fhir/StructureDefinition/DiagnosticReport|http://hl7.org/fhir/StructureDefinition/DocumentReference) ]
    ##
    # Associated insurance coverage
    # Insurance plans, coverage extensions, pre-authorizations and/or pre-determinations that may be required for delivering the requested service.
    attr_accessor :insurance                      # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Coverage|http://hl7.org/fhir/StructureDefinition/ClaimResponse) ]
    ##
    # Additional clinical information about the patient that may influence the request fulfilment.  For example, this may include where on the subject's body the device will be used (i.e. the target site).
    attr_accessor :supportingInfo                 # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Resource) ]
    ##
    # Notes or comments
    # Details about this request that were not represented at all or sufficiently in one of the attributes provided in a class. These may include for example a comment, an instruction, or a note associated with the statement.
    attr_accessor :note                           # 0-* [ Annotation ]
    ##
    # Request provenance
    # Key events in the history of the request.
    # This might not include provenances for all versions of the request - only those deemed "relevant" or important.This SHALL NOT include the Provenance associated with this current version of the resource.  (If that provenance is deemed to be a "relevant" change, it will need to be added as part of a later update.  Until then, it can be queried directly as the Provenance that points to this version using _revincludeAll Provenances should have some historical version of this Request as their subject.
    attr_accessor :relevantHistory                # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Provenance) ]

    def resourceType
      'DeviceRequest'
    end
  end
end
