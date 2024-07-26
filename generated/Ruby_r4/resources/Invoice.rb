module FHIR

  ##
  # Invoice containing collected ChargeItems from an Account with calculated individual and total price for Billing purpose.
  class Invoice < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['account', 'date', 'identifier', 'issuer', 'participant-role', 'participant', 'patient', 'recipient', 'status', 'subject', 'totalgross', 'totalnet', 'type']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'Invoice.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'Invoice.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'Invoice.implicitRules',
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
        'path'=>'Invoice.language',
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
        'path'=>'Invoice.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'Invoice.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Invoice.extension',
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
        'path'=>'Invoice.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Business Identifier for item
      # Identifier of this Invoice, often used for reference in correspondence about this invoice or for tracking of payments.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'Invoice.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # draft | issued | balanced | cancelled | entered-in-error
      # The current state of the Invoice.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/invoice-status'=>[ 'draft', 'issued', 'balanced', 'cancelled', 'entered-in-error' ]
        },
        'type'=>'code',
        'path'=>'Invoice.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/invoice-status'}
      },
      ##
      # Reason for cancellation of this Invoice
      # In case of Invoice cancellation a reason must be given (entered in error, superseded by corrected invoice etc.).
      # Derived Profiles may choose to add invariants requiring this field to be populated if either priceOverride or factorOverride have been filled.
      'cancelledReason' => {
        'type'=>'string',
        'path'=>'Invoice.cancelledReason',
        'min'=>0,
        'max'=>1
      },
      ##
      # Type of Invoice depending on domain, realm an usage (e.g. internal/external, dental, preliminary).
      'type' => {
        'type'=>'CodeableConcept',
        'path'=>'Invoice.type',
        'min'=>0,
        'max'=>1
      },
      ##
      # Recipient(s) of goods and services
      # The individual or set of individuals receiving the goods and services billed in this invoice.
      'subject' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Group'],
        'type'=>'Reference',
        'path'=>'Invoice.subject',
        'min'=>0,
        'max'=>1
      },
      ##
      # Recipient of this invoice
      # The individual or Organization responsible for balancing of this invoice.
      'recipient' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson'],
        'type'=>'Reference',
        'path'=>'Invoice.recipient',
        'min'=>0,
        'max'=>1
      },
      ##
      # Invoice date / posting date
      # Date/time(s) of when this Invoice was posted.
      # The list of types may be constrained as appropriate for the type of charge item.
      'date' => {
        'type'=>'dateTime',
        'path'=>'Invoice.date',
        'min'=>0,
        'max'=>1
      },
      ##
      # Participant in creation of this Invoice
      # Indicates who or what performed or participated in the charged service.
      'participant' => {
        'type'=>'Invoice::Participant',
        'path'=>'Invoice.participant',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Issuing Organization of Invoice
      # The organizationissuing the Invoice.
      # Practitioners and Devices can be associated with multiple organizations. It has to be made clear, on behalf of which Organization the services have been rendered.
      'issuer' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'Invoice.issuer',
        'min'=>0,
        'max'=>1
      },
      ##
      # Account that is being balanced
      # Account which is supposed to be balanced with this Invoice.
      # Systems posting the ChargeItems might not always be able to determine, which accounts the Items need to be places into. It is up to the potprocessing Financial System to apply internal rules to decide based on the Encounter/EpisodeOfCare/Patient/Coverage context and the type of ChargeItem, which Account is appropriate.
      'account' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Account'],
        'type'=>'Reference',
        'path'=>'Invoice.account',
        'min'=>0,
        'max'=>1
      },
      ##
      # Line items of this Invoice
      # Each line item represents one charge for goods and services rendered. Details such as date, code and amount are found in the referenced ChargeItem resource.
      'lineItem' => {
        'type'=>'Invoice::LineItem',
        'path'=>'Invoice.lineItem',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Components of Invoice total
      # The total amount for the Invoice may be calculated as the sum of the line items with surcharges/deductions that apply in certain conditions.  The priceComponent element can be used to offer transparency to the recipient of the Invoice of how the total price was calculated.
      'totalPriceComponent' => {
        'type'=>'Invoice::LineItem::PriceComponent',
        'path'=>'Invoice.totalPriceComponent',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Net total of this Invoice
      # Invoice total , taxes excluded.
      # There is no reason to carry the price in the instance of a ChargeItem unless circumstances require a manual override. The list prices or are usually defined in a back catalogue of the billing codes  (see ChargeItem.definition). Derived profiles may require a ChargeItem.overrideReason to be provided if either factor or price are manually overridden.
      'totalNet' => {
        'type'=>'Money',
        'path'=>'Invoice.totalNet',
        'min'=>0,
        'max'=>1
      },
      ##
      # Gross total of this Invoice
      # Invoice total, tax included.
      # There is no reason to carry the price in the instance of a ChargeItem unless circumstances require a manual override. The list prices or are usually defined in a back catalogue of the billing codes  (see ChargeItem.definition). Derived profiles may require a ChargeItem.overrideReason to be provided if either factor or price are manually overridden.
      'totalGross' => {
        'type'=>'Money',
        'path'=>'Invoice.totalGross',
        'min'=>0,
        'max'=>1
      },
      ##
      # Payment details such as banking details, period of payment, deductibles, methods of payment.
      # Derived Profiles may chose to add invariants requiring this field to be populated if either priceOverride or factorOverride have been filled.
      'paymentTerms' => {
        'type'=>'markdown',
        'path'=>'Invoice.paymentTerms',
        'min'=>0,
        'max'=>1
      },
      ##
      # Comments made about the invoice by the issuer, subject, or other participants.
      'note' => {
        'type'=>'Annotation',
        'path'=>'Invoice.note',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Participant in creation of this Invoice
    # Indicates who or what performed or participated in the charged service.
    class Participant < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Participant.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Participant.extension',
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
          'path'=>'Participant.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Type of involvement in creation of this Invoice
        # Describes the type of involvement (e.g. transcriptionist, creator etc.). If the invoice has been created automatically, the Participant may be a billing engine or another kind of device.
        'role' => {
          'type'=>'CodeableConcept',
          'path'=>'Participant.role',
          'min'=>0,
          'max'=>1
        },
        ##
        # Individual who was involved
        # The device, practitioner, etc. who performed or participated in the service.
        'actor' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson'],
          'type'=>'Reference',
          'path'=>'Participant.actor',
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
      # Type of involvement in creation of this Invoice
      # Describes the type of involvement (e.g. transcriptionist, creator etc.). If the invoice has been created automatically, the Participant may be a billing engine or another kind of device.
      attr_accessor :role                           # 0-1 CodeableConcept
      ##
      # Individual who was involved
      # The device, practitioner, etc. who performed or participated in the service.
      attr_accessor :actor                          # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/RelatedPerson)
    end

    ##
    # Line items of this Invoice
    # Each line item represents one charge for goods and services rendered. Details such as date, code and amount are found in the referenced ChargeItem resource.
    class LineItem < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'chargeItem[x]' => ['CodeableConcept', 'Reference']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'LineItem.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'LineItem.extension',
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
          'path'=>'LineItem.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Sequence number of line item
        # Sequence in which the items appear on the invoice.
        'sequence' => {
          'type'=>'positiveInt',
          'path'=>'LineItem.sequence',
          'min'=>0,
          'max'=>1
        },
        ##
        # Reference to ChargeItem containing details of this line item or an inline billing code
        # The ChargeItem contains information such as the billing code, date, amount etc. If no further details are required for the lineItem, inline billing codes can be added using the CodeableConcept data type instead of the Reference.
        'chargeItemCodeableConcept' => {
          'type'=>'CodeableConcept',
          'path'=>'LineItem.chargeItem[x]',
          'min'=>1,
          'max'=>1
        },
        ##
        # Reference to ChargeItem containing details of this line item or an inline billing code
        # The ChargeItem contains information such as the billing code, date, amount etc. If no further details are required for the lineItem, inline billing codes can be added using the CodeableConcept data type instead of the Reference.
        'chargeItemReference' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ChargeItem'],
          'type'=>'Reference',
          'path'=>'LineItem.chargeItem[x]',
          'min'=>1,
          'max'=>1
        },
        ##
        # Components of total line item price
        # The price for a ChargeItem may be calculated as a base price with surcharges/deductions that apply in certain conditions. A ChargeItemDefinition resource that defines the prices, factors and conditions that apply to a billing code is currently under development. The priceComponent element can be used to offer transparency to the recipient of the Invoice as to how the prices have been calculated.
        'priceComponent' => {
          'type'=>'Invoice::LineItem::PriceComponent',
          'path'=>'LineItem.priceComponent',
          'min'=>0,
          'max'=>Float::INFINITY
        }
      }

      ##
      # Components of total line item price
      # The price for a ChargeItem may be calculated as a base price with surcharges/deductions that apply in certain conditions. A ChargeItemDefinition resource that defines the prices, factors and conditions that apply to a billing code is currently under development. The priceComponent element can be used to offer transparency to the recipient of the Invoice as to how the prices have been calculated.
      class PriceComponent < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'id',
            'path'=>'PriceComponent.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'PriceComponent.extension',
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
            'path'=>'PriceComponent.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # base | surcharge | deduction | discount | tax | informational
          # This code identifies the type of the component.
          'type' => {
            'valid_codes'=>{
              'http://hl7.org/fhir/invoice-priceComponentType'=>[ 'base', 'surcharge', 'deduction', 'discount', 'tax', 'informational' ]
            },
            'type'=>'code',
            'path'=>'PriceComponent.type',
            'min'=>1,
            'max'=>1,
            'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/invoice-priceComponentType'}
          },
          ##
          # Code identifying the specific component
          # A code that identifies the component. Codes may be used to differentiate between kinds of taxes, surcharges, discounts etc.
          'code' => {
            'type'=>'CodeableConcept',
            'path'=>'PriceComponent.code',
            'min'=>0,
            'max'=>1
          },
          ##
          # Factor used for calculating this component
          # The factor that has been applied on the base price for calculating this component.
          # There is no reason to carry the price in the instance of a ChargeItem unless circumstances require a manual override. The list prices or are usually defined in a back catalogue of the billing codes  (see ChargeItem.definition). Derived profiles may require a ChargeItem.overrideReason to be provided if either factor or price are manually overridden.
          'factor' => {
            'type'=>'decimal',
            'path'=>'PriceComponent.factor',
            'min'=>0,
            'max'=>1
          },
          ##
          # Monetary amount associated with this component
          # The amount calculated for this component.
          # There is no reason to carry the price in the instance of a ChargeItem unless circumstances require a manual override. The list prices or are usually defined in a back catalogue of the billing codes  (see ChargeItem.definition). Derived profiles may require a ChargeItem.overrideReason to be provided if either factor or price are manually overridden.
          'amount' => {
            'type'=>'Money',
            'path'=>'PriceComponent.amount',
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
        # base | surcharge | deduction | discount | tax | informational
        # This code identifies the type of the component.
        attr_accessor :type                           # 1-1 code
        ##
        # Code identifying the specific component
        # A code that identifies the component. Codes may be used to differentiate between kinds of taxes, surcharges, discounts etc.
        attr_accessor :code                           # 0-1 CodeableConcept
        ##
        # Factor used for calculating this component
        # The factor that has been applied on the base price for calculating this component.
        # There is no reason to carry the price in the instance of a ChargeItem unless circumstances require a manual override. The list prices or are usually defined in a back catalogue of the billing codes  (see ChargeItem.definition). Derived profiles may require a ChargeItem.overrideReason to be provided if either factor or price are manually overridden.
        attr_accessor :factor                         # 0-1 decimal
        ##
        # Monetary amount associated with this component
        # The amount calculated for this component.
        # There is no reason to carry the price in the instance of a ChargeItem unless circumstances require a manual override. The list prices or are usually defined in a back catalogue of the billing codes  (see ChargeItem.definition). Derived profiles may require a ChargeItem.overrideReason to be provided if either factor or price are manually overridden.
        attr_accessor :amount                         # 0-1 Money
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
      # Sequence number of line item
      # Sequence in which the items appear on the invoice.
      attr_accessor :sequence                       # 0-1 positiveInt
      ##
      # Reference to ChargeItem containing details of this line item or an inline billing code
      # The ChargeItem contains information such as the billing code, date, amount etc. If no further details are required for the lineItem, inline billing codes can be added using the CodeableConcept data type instead of the Reference.
      attr_accessor :chargeItemCodeableConcept      # 1-1 CodeableConcept
      ##
      # Reference to ChargeItem containing details of this line item or an inline billing code
      # The ChargeItem contains information such as the billing code, date, amount etc. If no further details are required for the lineItem, inline billing codes can be added using the CodeableConcept data type instead of the Reference.
      attr_accessor :chargeItemReference            # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/ChargeItem)
      ##
      # Components of total line item price
      # The price for a ChargeItem may be calculated as a base price with surcharges/deductions that apply in certain conditions. A ChargeItemDefinition resource that defines the prices, factors and conditions that apply to a billing code is currently under development. The priceComponent element can be used to offer transparency to the recipient of the Invoice as to how the prices have been calculated.
      attr_accessor :priceComponent                 # 0-* [ Invoice::LineItem::PriceComponent ]
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
    # Business Identifier for item
    # Identifier of this Invoice, often used for reference in correspondence about this invoice or for tracking of payments.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # draft | issued | balanced | cancelled | entered-in-error
    # The current state of the Invoice.
    attr_accessor :status                         # 1-1 code
    ##
    # Reason for cancellation of this Invoice
    # In case of Invoice cancellation a reason must be given (entered in error, superseded by corrected invoice etc.).
    # Derived Profiles may choose to add invariants requiring this field to be populated if either priceOverride or factorOverride have been filled.
    attr_accessor :cancelledReason                # 0-1 string
    ##
    # Type of Invoice depending on domain, realm an usage (e.g. internal/external, dental, preliminary).
    attr_accessor :type                           # 0-1 CodeableConcept
    ##
    # Recipient(s) of goods and services
    # The individual or set of individuals receiving the goods and services billed in this invoice.
    attr_accessor :subject                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Group)
    ##
    # Recipient of this invoice
    # The individual or Organization responsible for balancing of this invoice.
    attr_accessor :recipient                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/RelatedPerson)
    ##
    # Invoice date / posting date
    # Date/time(s) of when this Invoice was posted.
    # The list of types may be constrained as appropriate for the type of charge item.
    attr_accessor :date                           # 0-1 dateTime
    ##
    # Participant in creation of this Invoice
    # Indicates who or what performed or participated in the charged service.
    attr_accessor :participant                    # 0-* [ Invoice::Participant ]
    ##
    # Issuing Organization of Invoice
    # The organizationissuing the Invoice.
    # Practitioners and Devices can be associated with multiple organizations. It has to be made clear, on behalf of which Organization the services have been rendered.
    attr_accessor :issuer                         # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Account that is being balanced
    # Account which is supposed to be balanced with this Invoice.
    # Systems posting the ChargeItems might not always be able to determine, which accounts the Items need to be places into. It is up to the potprocessing Financial System to apply internal rules to decide based on the Encounter/EpisodeOfCare/Patient/Coverage context and the type of ChargeItem, which Account is appropriate.
    attr_accessor :account                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Account)
    ##
    # Line items of this Invoice
    # Each line item represents one charge for goods and services rendered. Details such as date, code and amount are found in the referenced ChargeItem resource.
    attr_accessor :lineItem                       # 0-* [ Invoice::LineItem ]
    ##
    # Components of Invoice total
    # The total amount for the Invoice may be calculated as the sum of the line items with surcharges/deductions that apply in certain conditions.  The priceComponent element can be used to offer transparency to the recipient of the Invoice of how the total price was calculated.
    attr_accessor :totalPriceComponent            # 0-* [ Invoice::LineItem::PriceComponent ]
    ##
    # Net total of this Invoice
    # Invoice total , taxes excluded.
    # There is no reason to carry the price in the instance of a ChargeItem unless circumstances require a manual override. The list prices or are usually defined in a back catalogue of the billing codes  (see ChargeItem.definition). Derived profiles may require a ChargeItem.overrideReason to be provided if either factor or price are manually overridden.
    attr_accessor :totalNet                       # 0-1 Money
    ##
    # Gross total of this Invoice
    # Invoice total, tax included.
    # There is no reason to carry the price in the instance of a ChargeItem unless circumstances require a manual override. The list prices or are usually defined in a back catalogue of the billing codes  (see ChargeItem.definition). Derived profiles may require a ChargeItem.overrideReason to be provided if either factor or price are manually overridden.
    attr_accessor :totalGross                     # 0-1 Money
    ##
    # Payment details such as banking details, period of payment, deductibles, methods of payment.
    # Derived Profiles may chose to add invariants requiring this field to be populated if either priceOverride or factorOverride have been filled.
    attr_accessor :paymentTerms                   # 0-1 markdown
    ##
    # Comments made about the invoice by the issuer, subject, or other participants.
    attr_accessor :note                           # 0-* [ Annotation ]

    def resourceType
      'Invoice'
    end
  end
end
