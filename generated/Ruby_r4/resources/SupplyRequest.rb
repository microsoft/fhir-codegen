module FHIR

  ##
  # A record of a request for a medication, substance or device used in the healthcare setting.
  class SupplyRequest < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['category', 'date', 'identifier', 'requester', 'status', 'subject', 'supplier']
    MULTIPLE_TYPES = {
      'item[x]' => ['CodeableConcept', 'Reference'],
      'occurrence[x]' => ['dateTime', 'Period', 'Timing']
    }
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'SupplyRequest.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'SupplyRequest.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'SupplyRequest.implicitRules',
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
        'path'=>'SupplyRequest.language',
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
        'path'=>'SupplyRequest.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'SupplyRequest.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'SupplyRequest.extension',
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
        'path'=>'SupplyRequest.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Business Identifier for SupplyRequest
      # Business identifiers assigned to this SupplyRequest by the author and/or other systems. These identifiers remain constant as the resource is updated and propagates from server to server.
      # The identifier.type element is used to distinguish between the identifiers assigned by the requester/placer and the performer/filler.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'SupplyRequest.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # draft | active | suspended +
      # Status of the supply request.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/supplyrequest-status'=>[ 'draft', 'active', 'suspended', 'cancelled', 'completed', 'entered-in-error', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'SupplyRequest.status',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/supplyrequest-status'}
      },
      ##
      # The kind of supply (central, non-stock, etc.)
      # Category of supply, e.g.  central, non-stock, etc. This is used to support work flows associated with the supply process.
      'category' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/supply-kind'=>[ 'central', 'nonstock' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'SupplyRequest.category',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/supplyrequest-kind'}
      },
      ##
      # routine | urgent | asap | stat
      # Indicates how quickly this SupplyRequest should be addressed with respect to other requests.
      'priority' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/request-priority'=>[ 'routine', 'urgent', 'asap', 'stat' ]
        },
        'type'=>'code',
        'path'=>'SupplyRequest.priority',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/request-priority'}
      },
      ##
      # Medication, Substance, or Device requested to be supplied
      # The item that is requested to be supplied. This is either a link to a resource representing the details of the item or a code that identifies the item from a known list.
      # Note that there's a difference between a prescription - an instruction to take a medication, along with a (sometimes) implicit supply, and an explicit request to supply, with no explicit instructions.
      'itemCodeableConcept' => {
        'type'=>'CodeableConcept',
        'path'=>'SupplyRequest.item[x]',
        'min'=>1,
        'max'=>1
      },
      ##
      # Medication, Substance, or Device requested to be supplied
      # The item that is requested to be supplied. This is either a link to a resource representing the details of the item or a code that identifies the item from a known list.
      # Note that there's a difference between a prescription - an instruction to take a medication, along with a (sometimes) implicit supply, and an explicit request to supply, with no explicit instructions.
      'itemReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Medication', 'http://hl7.org/fhir/StructureDefinition/Substance', 'http://hl7.org/fhir/StructureDefinition/Device'],
        'type'=>'Reference',
        'path'=>'SupplyRequest.item[x]',
        'min'=>1,
        'max'=>1
      },
      ##
      # The requested amount of the item indicated
      # The amount that is being ordered of the indicated item.
      'quantity' => {
        'type'=>'Quantity',
        'path'=>'SupplyRequest.quantity',
        'min'=>1,
        'max'=>1
      },
      ##
      # Ordered item details
      # Specific parameters for the ordered item.  For example, the size of the indicated item.
      'parameter' => {
        'type'=>'SupplyRequest::Parameter',
        'path'=>'SupplyRequest.parameter',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # When the request should be fulfilled.
      'occurrenceDateTime' => {
        'type'=>'DateTime',
        'path'=>'SupplyRequest.occurrence[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the request should be fulfilled.
      'occurrencePeriod' => {
        'type'=>'Period',
        'path'=>'SupplyRequest.occurrence[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the request should be fulfilled.
      'occurrenceTiming' => {
        'type'=>'Timing',
        'path'=>'SupplyRequest.occurrence[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the request was made.
      'authoredOn' => {
        'type'=>'dateTime',
        'path'=>'SupplyRequest.authoredOn',
        'min'=>0,
        'max'=>1
      },
      ##
      # Individual making the request
      # The device, practitioner, etc. who initiated the request.
      'requester' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/Device'],
        'type'=>'Reference',
        'path'=>'SupplyRequest.requester',
        'min'=>0,
        'max'=>1
      },
      ##
      # Who is intended to fulfill the request.
      'supplier' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/HealthcareService'],
        'type'=>'Reference',
        'path'=>'SupplyRequest.supplier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The reason why the supply item was requested.
      'reasonCode' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/supplyrequest-reason'=>[ 'patient-care', 'ward-stock' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'SupplyRequest.reasonCode',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/supplyrequest-reason'}
      },
      ##
      # The reason why the supply item was requested.
      'reasonReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Condition', 'http://hl7.org/fhir/StructureDefinition/Observation', 'http://hl7.org/fhir/StructureDefinition/DiagnosticReport', 'http://hl7.org/fhir/StructureDefinition/DocumentReference'],
        'type'=>'Reference',
        'path'=>'SupplyRequest.reasonReference',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The origin of the supply
      # Where the supply is expected to come from.
      'deliverFrom' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/Location'],
        'type'=>'Reference',
        'path'=>'SupplyRequest.deliverFrom',
        'min'=>0,
        'max'=>1
      },
      ##
      # The destination of the supply
      # Where the supply is destined to go.
      'deliverTo' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/Location', 'http://hl7.org/fhir/StructureDefinition/Patient'],
        'type'=>'Reference',
        'path'=>'SupplyRequest.deliverTo',
        'min'=>0,
        'max'=>1
      }
    }

    ##
    # Ordered item details
    # Specific parameters for the ordered item.  For example, the size of the indicated item.
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
          'type'=>'string',
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
        # Item detail
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
      # Item detail
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
    # Business Identifier for SupplyRequest
    # Business identifiers assigned to this SupplyRequest by the author and/or other systems. These identifiers remain constant as the resource is updated and propagates from server to server.
    # The identifier.type element is used to distinguish between the identifiers assigned by the requester/placer and the performer/filler.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # draft | active | suspended +
    # Status of the supply request.
    attr_accessor :status                         # 0-1 code
    ##
    # The kind of supply (central, non-stock, etc.)
    # Category of supply, e.g.  central, non-stock, etc. This is used to support work flows associated with the supply process.
    attr_accessor :category                       # 0-1 CodeableConcept
    ##
    # routine | urgent | asap | stat
    # Indicates how quickly this SupplyRequest should be addressed with respect to other requests.
    attr_accessor :priority                       # 0-1 code
    ##
    # Medication, Substance, or Device requested to be supplied
    # The item that is requested to be supplied. This is either a link to a resource representing the details of the item or a code that identifies the item from a known list.
    # Note that there's a difference between a prescription - an instruction to take a medication, along with a (sometimes) implicit supply, and an explicit request to supply, with no explicit instructions.
    attr_accessor :itemCodeableConcept            # 1-1 CodeableConcept
    ##
    # Medication, Substance, or Device requested to be supplied
    # The item that is requested to be supplied. This is either a link to a resource representing the details of the item or a code that identifies the item from a known list.
    # Note that there's a difference between a prescription - an instruction to take a medication, along with a (sometimes) implicit supply, and an explicit request to supply, with no explicit instructions.
    attr_accessor :itemReference                  # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Medication|http://hl7.org/fhir/StructureDefinition/Substance|http://hl7.org/fhir/StructureDefinition/Device)
    ##
    # The requested amount of the item indicated
    # The amount that is being ordered of the indicated item.
    attr_accessor :quantity                       # 1-1 Quantity
    ##
    # Ordered item details
    # Specific parameters for the ordered item.  For example, the size of the indicated item.
    attr_accessor :parameter                      # 0-* [ SupplyRequest::Parameter ]
    ##
    # When the request should be fulfilled.
    attr_accessor :occurrenceDateTime             # 0-1 DateTime
    ##
    # When the request should be fulfilled.
    attr_accessor :occurrencePeriod               # 0-1 Period
    ##
    # When the request should be fulfilled.
    attr_accessor :occurrenceTiming               # 0-1 Timing
    ##
    # When the request was made.
    attr_accessor :authoredOn                     # 0-1 dateTime
    ##
    # Individual making the request
    # The device, practitioner, etc. who initiated the request.
    attr_accessor :requester                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/Device)
    ##
    # Who is intended to fulfill the request.
    attr_accessor :supplier                       # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/HealthcareService) ]
    ##
    # The reason why the supply item was requested.
    attr_accessor :reasonCode                     # 0-* [ CodeableConcept ]
    ##
    # The reason why the supply item was requested.
    attr_accessor :reasonReference                # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Condition|http://hl7.org/fhir/StructureDefinition/Observation|http://hl7.org/fhir/StructureDefinition/DiagnosticReport|http://hl7.org/fhir/StructureDefinition/DocumentReference) ]
    ##
    # The origin of the supply
    # Where the supply is expected to come from.
    attr_accessor :deliverFrom                    # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/Location)
    ##
    # The destination of the supply
    # Where the supply is destined to go.
    attr_accessor :deliverTo                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/Location|http://hl7.org/fhir/StructureDefinition/Patient)

    def resourceType
      'SupplyRequest'
    end
  end
end
