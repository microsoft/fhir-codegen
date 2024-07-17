module FHIR

  ##
  # The resource ChargeItem describes the provision of healthcare provider products for a certain patient, therefore referring not only to the product, but containing in addition details of the provision, like date, time, amounts and participating organizations and persons. Main Usage of the ChargeItem is to enable the billing process and internal cost allocation.
  class ChargeItem < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['account', 'code', 'context', 'entered-date', 'enterer', 'factor-override', 'identifier', 'occurrence', 'patient', 'performer-actor', 'performer-function', 'performing-organization', 'price-override', 'quantity', 'requesting-organization', 'service', 'subject']
    MULTIPLE_TYPES = {
      'occurrence[x]' => ['dateTime', 'Period', 'Timing'],
      'product[x]' => ['CodeableConcept', 'Reference']
    }
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'ChargeItem.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'ChargeItem.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'ChargeItem.implicitRules',
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
        'path'=>'ChargeItem.language',
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
        'path'=>'ChargeItem.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'ChargeItem.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'ChargeItem.extension',
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
        'path'=>'ChargeItem.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Business Identifier for item
      # Identifiers assigned to this event performer or other systems.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'ChargeItem.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Defining information about the code of this charge item
      # References the (external) source of pricing information, rules of application for the code this ChargeItem uses.
      'definitionUri' => {
        'type'=>'uri',
        'path'=>'ChargeItem.definitionUri',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Resource defining the code of this ChargeItem
      # References the source of pricing information, rules of application for the code this ChargeItem uses.
      'definitionCanonical' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ChargeItemDefinition'],
        'type'=>'canonical',
        'path'=>'ChargeItem.definitionCanonical',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # planned | billable | not-billable | aborted | billed | entered-in-error | unknown
      # The current state of the ChargeItem.
      # Unknown does not represent "other" - one of the defined statuses must apply.  Unknown is used when the authoring system is not sure what the current status is.
      # 
      # This element is labeled as a modifier because the status contains the code entered-in-error that marks the charge item as not currently valid.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/chargeitem-status'=>[ 'planned', 'billable', 'not-billable', 'aborted', 'billed', 'entered-in-error', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'ChargeItem.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/chargeitem-status'}
      },
      ##
      # Part of referenced ChargeItem
      # ChargeItems can be grouped to larger ChargeItems covering the whole set.
      'partOf' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ChargeItem'],
        'type'=>'Reference',
        'path'=>'ChargeItem.partOf',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # A code that identifies the charge, like a billing code.
      'code' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/chargeitem-billingcodes'=>[ '1100', '1210', '1320' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'ChargeItem.code',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/chargeitem-billingcodes'}
      },
      ##
      # Individual service was done for/to
      # The individual or set of individuals the action is being or was performed on.
      'subject' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Group'],
        'type'=>'Reference',
        'path'=>'ChargeItem.subject',
        'min'=>1,
        'max'=>1
      },
      ##
      # Encounter / Episode associated with event
      # The encounter or episode of care that establishes the context for this event.
      'context' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Encounter', 'http://hl7.org/fhir/StructureDefinition/EpisodeOfCare'],
        'type'=>'Reference',
        'path'=>'ChargeItem.context',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the charged service was applied
      # Date/time(s) or duration when the charged service was applied.
      # The list of types may be constrained as appropriate for the type of charge item.
      'occurrenceDateTime' => {
        'type'=>'DateTime',
        'path'=>'ChargeItem.occurrence[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the charged service was applied
      # Date/time(s) or duration when the charged service was applied.
      # The list of types may be constrained as appropriate for the type of charge item.
      'occurrencePeriod' => {
        'type'=>'Period',
        'path'=>'ChargeItem.occurrence[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the charged service was applied
      # Date/time(s) or duration when the charged service was applied.
      # The list of types may be constrained as appropriate for the type of charge item.
      'occurrenceTiming' => {
        'type'=>'Timing',
        'path'=>'ChargeItem.occurrence[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Who performed charged service
      # Indicates who or what performed or participated in the charged service.
      'performer' => {
        'type'=>'ChargeItem::Performer',
        'path'=>'ChargeItem.performer',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Organization providing the charged service
      # The organization requesting the service.
      # Practitioners and Devices can be associated with multiple organizations. It has to be made clear, on behalf of which Organization the services have been rendered.
      'performingOrganization' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'ChargeItem.performingOrganization',
        'min'=>0,
        'max'=>1
      },
      ##
      # Organization requesting the charged service
      # The organization performing the service.
      # The rendered Service might not be associated with a Request. This property indicates which Organization requested the services to be rendered. (In many cases, this may just be the Department associated with the Encounter.location).
      'requestingOrganization' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'ChargeItem.requestingOrganization',
        'min'=>0,
        'max'=>1
      },
      ##
      # Organization that has ownership of the (potential, future) revenue
      # The financial cost center permits the tracking of charge attribution.
      # The costCenter could either be given as a reference to an Organization(Role) resource or as the identifier of the cost center determined by Reference.identifier.value and Reference.identifier.system, depending on use case requirements.
      'costCenter' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'ChargeItem.costCenter',
        'min'=>0,
        'max'=>1
      },
      ##
      # Quantity of which the charge item has been serviced.
      # In many cases this may just be a value, if the underlying units are implicit in the definition of the charge item code.
      'quantity' => {
        'type'=>'Quantity',
        'path'=>'ChargeItem.quantity',
        'min'=>0,
        'max'=>1
      },
      ##
      # Anatomical location, if relevant
      # The anatomical location where the related service has been applied.
      # Only used if not implicit in code found in Condition.code. If the use case requires attributes from the BodySite resource (e.g. to identify and track separately) then use the standard extension [bodySite](extension-bodysite.html).  May be a summary code, or a reference to a very precise definition of the location, or both.
      'bodysite' => {
        'type'=>'CodeableConcept',
        'path'=>'ChargeItem.bodysite',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Factor overriding the associated rules
      # Factor overriding the factor determined by the rules associated with the code.
      # There is no reason to carry the factor in the instance of a ChargeItem unless special circumstances require a manual override. The factors are usually defined by a set of rules in a back catalogue of the billing codes  (see ChargeItem.definition). Derived profiles may require a ChargeItem.overrideReason to be provided if either factor or price are manually overridden.
      'factorOverride' => {
        'type'=>'decimal',
        'path'=>'ChargeItem.factorOverride',
        'min'=>0,
        'max'=>1
      },
      ##
      # Price overriding the associated rules
      # Total price of the charge overriding the list price associated with the code.
      # There is no reason to carry the price in the instance of a ChargeItem unless circumstances require a manual override. The list prices or are usually defined in a back catalogue of the billing codes  (see ChargeItem.definition). Derived profiles may require a ChargeItem.overrideReason to be provided if either factor or price are manually overridden.
      'priceOverride' => {
        'type'=>'Money',
        'path'=>'ChargeItem.priceOverride',
        'min'=>0,
        'max'=>1
      },
      ##
      # Reason for overriding the list price/factor
      # If the list price or the rule-based factor associated with the code is overridden, this attribute can capture a text to indicate the  reason for this action.
      # Derived Profiles may choose to add invariants requiring this field to be populated if either priceOverride or factorOverride have been filled.
      'overrideReason' => {
        'type'=>'string',
        'path'=>'ChargeItem.overrideReason',
        'min'=>0,
        'max'=>1
      },
      ##
      # Individual who was entering
      # The device, practitioner, etc. who entered the charge item.
      # The enterer is also the person considered responsible for factor/price overrides if applicable.
      'enterer' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson'],
        'type'=>'Reference',
        'path'=>'ChargeItem.enterer',
        'min'=>0,
        'max'=>1
      },
      ##
      # Date the charge item was entered.
      # The actual date when the service associated with the charge has been rendered is captured in occurrence[x].
      'enteredDate' => {
        'type'=>'dateTime',
        'path'=>'ChargeItem.enteredDate',
        'min'=>0,
        'max'=>1
      },
      ##
      # Why was the charged  service rendered?
      # Describes why the event occurred in coded or textual form.
      # If the application of the charge item requires a reason to be given, it can be captured here. Textual reasons can be captured using reasonCode.text.
      'reason' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/sid/icd-10'=>[ '123456', '123457', '987654', '123987', '112233', '997755', '321789' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'ChargeItem.reason',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/icd-10'}
      },
      ##
      # Which rendered service is being charged?
      # Indicated the rendered service that caused this charge.
      'service' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/DiagnosticReport', 'http://hl7.org/fhir/StructureDefinition/ImagingStudy', 'http://hl7.org/fhir/StructureDefinition/Immunization', 'http://hl7.org/fhir/StructureDefinition/MedicationAdministration', 'http://hl7.org/fhir/StructureDefinition/MedicationDispense', 'http://hl7.org/fhir/StructureDefinition/Observation', 'http://hl7.org/fhir/StructureDefinition/Procedure', 'http://hl7.org/fhir/StructureDefinition/SupplyDelivery'],
        'type'=>'Reference',
        'path'=>'ChargeItem.service',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Product charged
      # Identifies the device, food, drug or other product being charged either by type code or reference to an instance.
      'productCodeableConcept' => {
        'type'=>'CodeableConcept',
        'path'=>'ChargeItem.product[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Product charged
      # Identifies the device, food, drug or other product being charged either by type code or reference to an instance.
      'productReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/Medication', 'http://hl7.org/fhir/StructureDefinition/Substance'],
        'type'=>'Reference',
        'path'=>'ChargeItem.product[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Account to place this charge
      # Account into which this ChargeItems belongs.
      # Systems posting the ChargeItems might not always be able to determine, which accounts the Items need to be places into. It is up to the postprocessing Financial System to apply internal rules to decide based on the Encounter/EpisodeOfCare/Patient/Coverage context and the type of ChargeItem, which Account is appropriate.
      'account' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Account'],
        'type'=>'Reference',
        'path'=>'ChargeItem.account',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Comments made about the ChargeItem
      # Comments made about the event by the performer, subject or other participants.
      'note' => {
        'type'=>'Annotation',
        'path'=>'ChargeItem.note',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Further information supporting this charge.
      'supportingInformation' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'Reference',
        'path'=>'ChargeItem.supportingInformation',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Who performed charged service
    # Indicates who or what performed or participated in the charged service.
    class Performer < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Performer.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Performer.extension',
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
          'path'=>'Performer.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # What type of performance was done
        # Describes the type of performance or participation(e.g. primary surgeon, anesthesiologiest, etc.).
        'function' => {
          'type'=>'CodeableConcept',
          'path'=>'Performer.function',
          'min'=>0,
          'max'=>1
        },
        ##
        # Individual who was performing
        # The device, practitioner, etc. who performed or participated in the service.
        'actor' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/CareTeam', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson'],
          'type'=>'Reference',
          'path'=>'Performer.actor',
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
      # What type of performance was done
      # Describes the type of performance or participation(e.g. primary surgeon, anesthesiologiest, etc.).
      attr_accessor :function                       # 0-1 CodeableConcept
      ##
      # Individual who was performing
      # The device, practitioner, etc. who performed or participated in the service.
      attr_accessor :actor                          # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/CareTeam|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/RelatedPerson)
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
    # Business Identifier for item
    # Identifiers assigned to this event performer or other systems.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Defining information about the code of this charge item
    # References the (external) source of pricing information, rules of application for the code this ChargeItem uses.
    attr_accessor :definitionUri                  # 0-* [ uri ]
    ##
    # Resource defining the code of this ChargeItem
    # References the source of pricing information, rules of application for the code this ChargeItem uses.
    attr_accessor :definitionCanonical            # 0-* [ canonical(http://hl7.org/fhir/StructureDefinition/ChargeItemDefinition) ]
    ##
    # planned | billable | not-billable | aborted | billed | entered-in-error | unknown
    # The current state of the ChargeItem.
    # Unknown does not represent "other" - one of the defined statuses must apply.  Unknown is used when the authoring system is not sure what the current status is.
    # 
    # This element is labeled as a modifier because the status contains the code entered-in-error that marks the charge item as not currently valid.
    attr_accessor :status                         # 1-1 code
    ##
    # Part of referenced ChargeItem
    # ChargeItems can be grouped to larger ChargeItems covering the whole set.
    attr_accessor :partOf                         # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/ChargeItem) ]
    ##
    # A code that identifies the charge, like a billing code.
    attr_accessor :code                           # 1-1 CodeableConcept
    ##
    # Individual service was done for/to
    # The individual or set of individuals the action is being or was performed on.
    attr_accessor :subject                        # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Group)
    ##
    # Encounter / Episode associated with event
    # The encounter or episode of care that establishes the context for this event.
    attr_accessor :context                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Encounter|http://hl7.org/fhir/StructureDefinition/EpisodeOfCare)
    ##
    # When the charged service was applied
    # Date/time(s) or duration when the charged service was applied.
    # The list of types may be constrained as appropriate for the type of charge item.
    attr_accessor :occurrenceDateTime             # 0-1 DateTime
    ##
    # When the charged service was applied
    # Date/time(s) or duration when the charged service was applied.
    # The list of types may be constrained as appropriate for the type of charge item.
    attr_accessor :occurrencePeriod               # 0-1 Period
    ##
    # When the charged service was applied
    # Date/time(s) or duration when the charged service was applied.
    # The list of types may be constrained as appropriate for the type of charge item.
    attr_accessor :occurrenceTiming               # 0-1 Timing
    ##
    # Who performed charged service
    # Indicates who or what performed or participated in the charged service.
    attr_accessor :performer                      # 0-* [ ChargeItem::Performer ]
    ##
    # Organization providing the charged service
    # The organization requesting the service.
    # Practitioners and Devices can be associated with multiple organizations. It has to be made clear, on behalf of which Organization the services have been rendered.
    attr_accessor :performingOrganization         # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Organization requesting the charged service
    # The organization performing the service.
    # The rendered Service might not be associated with a Request. This property indicates which Organization requested the services to be rendered. (In many cases, this may just be the Department associated with the Encounter.location).
    attr_accessor :requestingOrganization         # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Organization that has ownership of the (potential, future) revenue
    # The financial cost center permits the tracking of charge attribution.
    # The costCenter could either be given as a reference to an Organization(Role) resource or as the identifier of the cost center determined by Reference.identifier.value and Reference.identifier.system, depending on use case requirements.
    attr_accessor :costCenter                     # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Quantity of which the charge item has been serviced.
    # In many cases this may just be a value, if the underlying units are implicit in the definition of the charge item code.
    attr_accessor :quantity                       # 0-1 Quantity
    ##
    # Anatomical location, if relevant
    # The anatomical location where the related service has been applied.
    # Only used if not implicit in code found in Condition.code. If the use case requires attributes from the BodySite resource (e.g. to identify and track separately) then use the standard extension [bodySite](extension-bodysite.html).  May be a summary code, or a reference to a very precise definition of the location, or both.
    attr_accessor :bodysite                       # 0-* [ CodeableConcept ]
    ##
    # Factor overriding the associated rules
    # Factor overriding the factor determined by the rules associated with the code.
    # There is no reason to carry the factor in the instance of a ChargeItem unless special circumstances require a manual override. The factors are usually defined by a set of rules in a back catalogue of the billing codes  (see ChargeItem.definition). Derived profiles may require a ChargeItem.overrideReason to be provided if either factor or price are manually overridden.
    attr_accessor :factorOverride                 # 0-1 decimal
    ##
    # Price overriding the associated rules
    # Total price of the charge overriding the list price associated with the code.
    # There is no reason to carry the price in the instance of a ChargeItem unless circumstances require a manual override. The list prices or are usually defined in a back catalogue of the billing codes  (see ChargeItem.definition). Derived profiles may require a ChargeItem.overrideReason to be provided if either factor or price are manually overridden.
    attr_accessor :priceOverride                  # 0-1 Money
    ##
    # Reason for overriding the list price/factor
    # If the list price or the rule-based factor associated with the code is overridden, this attribute can capture a text to indicate the  reason for this action.
    # Derived Profiles may choose to add invariants requiring this field to be populated if either priceOverride or factorOverride have been filled.
    attr_accessor :overrideReason                 # 0-1 string
    ##
    # Individual who was entering
    # The device, practitioner, etc. who entered the charge item.
    # The enterer is also the person considered responsible for factor/price overrides if applicable.
    attr_accessor :enterer                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/RelatedPerson)
    ##
    # Date the charge item was entered.
    # The actual date when the service associated with the charge has been rendered is captured in occurrence[x].
    attr_accessor :enteredDate                    # 0-1 dateTime
    ##
    # Why was the charged  service rendered?
    # Describes why the event occurred in coded or textual form.
    # If the application of the charge item requires a reason to be given, it can be captured here. Textual reasons can be captured using reasonCode.text.
    attr_accessor :reason                         # 0-* [ CodeableConcept ]
    ##
    # Which rendered service is being charged?
    # Indicated the rendered service that caused this charge.
    attr_accessor :service                        # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/DiagnosticReport|http://hl7.org/fhir/StructureDefinition/ImagingStudy|http://hl7.org/fhir/StructureDefinition/Immunization|http://hl7.org/fhir/StructureDefinition/MedicationAdministration|http://hl7.org/fhir/StructureDefinition/MedicationDispense|http://hl7.org/fhir/StructureDefinition/Observation|http://hl7.org/fhir/StructureDefinition/Procedure|http://hl7.org/fhir/StructureDefinition/SupplyDelivery) ]
    ##
    # Product charged
    # Identifies the device, food, drug or other product being charged either by type code or reference to an instance.
    attr_accessor :productCodeableConcept         # 0-1 CodeableConcept
    ##
    # Product charged
    # Identifies the device, food, drug or other product being charged either by type code or reference to an instance.
    attr_accessor :productReference               # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/Medication|http://hl7.org/fhir/StructureDefinition/Substance)
    ##
    # Account to place this charge
    # Account into which this ChargeItems belongs.
    # Systems posting the ChargeItems might not always be able to determine, which accounts the Items need to be places into. It is up to the postprocessing Financial System to apply internal rules to decide based on the Encounter/EpisodeOfCare/Patient/Coverage context and the type of ChargeItem, which Account is appropriate.
    attr_accessor :account                        # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Account) ]
    ##
    # Comments made about the ChargeItem
    # Comments made about the event by the performer, subject or other participants.
    attr_accessor :note                           # 0-* [ Annotation ]
    ##
    # Further information supporting this charge.
    attr_accessor :supportingInformation          # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Resource) ]

    def resourceType
      'ChargeItem'
    end
  end
end
