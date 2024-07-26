module FHIR

  ##
  # This resource provides the details including amount of a payment and allocates the payment items being paid.
  class PaymentReconciliation < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['created', 'disposition', 'identifier', 'outcome', 'payment-issuer', 'request', 'requestor', 'status']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'PaymentReconciliation.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'PaymentReconciliation.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'PaymentReconciliation.implicitRules',
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
        'path'=>'PaymentReconciliation.language',
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
        'path'=>'PaymentReconciliation.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'PaymentReconciliation.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'PaymentReconciliation.extension',
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
        'path'=>'PaymentReconciliation.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Business Identifier for a payment reconciliation
      # A unique identifier assigned to this payment reconciliation.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'PaymentReconciliation.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # active | cancelled | draft | entered-in-error
      # The status of the resource instance.
      # This element is labeled as a modifier because the status contains codes that mark the resource as not currently valid.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/fm-status'=>[ 'active', 'cancelled', 'draft', 'entered-in-error' ]
        },
        'type'=>'code',
        'path'=>'PaymentReconciliation.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/fm-status'}
      },
      ##
      # Period covered
      # The period of time for which payments have been gathered into this bulk payment for settlement.
      'period' => {
        'type'=>'Period',
        'path'=>'PaymentReconciliation.period',
        'min'=>0,
        'max'=>1
      },
      ##
      # Creation date
      # The date when the resource was created.
      'created' => {
        'type'=>'dateTime',
        'path'=>'PaymentReconciliation.created',
        'min'=>1,
        'max'=>1
      },
      ##
      # Party generating payment
      # The party who generated the payment.
      # This party is also responsible for the reconciliation.
      'paymentIssuer' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'PaymentReconciliation.paymentIssuer',
        'min'=>0,
        'max'=>1
      },
      ##
      # Reference to requesting resource
      # Original request resource reference.
      'request' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Task'],
        'type'=>'Reference',
        'path'=>'PaymentReconciliation.request',
        'min'=>0,
        'max'=>1
      },
      ##
      # Responsible practitioner
      # The practitioner who is responsible for the services rendered to the patient.
      'requestor' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'PaymentReconciliation.requestor',
        'min'=>0,
        'max'=>1
      },
      ##
      # queued | complete | error | partial
      # The outcome of a request for a reconciliation.
      # The resource may be used to indicate that: the request has been held (queued) for processing; that it has been processed and errors found (error); that no errors were found and that some of the adjudication has been undertaken (partial) or that all of the adjudication has been undertaken (complete).
      'outcome' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/remittance-outcome'=>[ 'queued', 'complete', 'error', 'partial' ]
        },
        'type'=>'code',
        'path'=>'PaymentReconciliation.outcome',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/remittance-outcome'}
      },
      ##
      # Disposition message
      # A human readable description of the status of the request for the reconciliation.
      'disposition' => {
        'type'=>'string',
        'path'=>'PaymentReconciliation.disposition',
        'min'=>0,
        'max'=>1
      },
      ##
      # When payment issued
      # The date of payment as indicated on the financial instrument.
      'paymentDate' => {
        'type'=>'date',
        'path'=>'PaymentReconciliation.paymentDate',
        'min'=>1,
        'max'=>1
      },
      ##
      # Total amount of Payment
      # Total payment amount as indicated on the financial instrument.
      'paymentAmount' => {
        'type'=>'Money',
        'path'=>'PaymentReconciliation.paymentAmount',
        'min'=>1,
        'max'=>1
      },
      ##
      # Business identifier for the payment
      # Issuer's unique identifier for the payment instrument.
      # For example: EFT number or check number.
      'paymentIdentifier' => {
        'type'=>'Identifier',
        'path'=>'PaymentReconciliation.paymentIdentifier',
        'min'=>0,
        'max'=>1
      },
      ##
      # Settlement particulars
      # Distribution of the payment amount for a previously acknowledged payable.
      'detail' => {
        'type'=>'PaymentReconciliation::Detail',
        'path'=>'PaymentReconciliation.detail',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Printed form identifier
      # A code for the form to be used for printing the content.
      # May be needed to identify specific jurisdictional forms.
      'formCode' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/forms-codes'=>[ '1', '2' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'PaymentReconciliation.formCode',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/forms'}
      },
      ##
      # Note concerning processing
      # A note that describes or explains the processing in a human readable form.
      'processNote' => {
        'type'=>'PaymentReconciliation::ProcessNote',
        'path'=>'PaymentReconciliation.processNote',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Settlement particulars
    # Distribution of the payment amount for a previously acknowledged payable.
    class Detail < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

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
        # Business identifier of the payment detail
        # Unique identifier for the current payment item for the referenced payable.
        'identifier' => {
          'type'=>'Identifier',
          'path'=>'Detail.identifier',
          'min'=>0,
          'max'=>1
        },
        ##
        # Business identifier of the prior payment detail
        # Unique identifier for the prior payment item for the referenced payable.
        'predecessor' => {
          'type'=>'Identifier',
          'path'=>'Detail.predecessor',
          'min'=>0,
          'max'=>1
        },
        ##
        # Category of payment
        # Code to indicate the nature of the payment.
        # For example: payment, adjustment, funds advance, etc.
        'type' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/payment-type'=>[ 'payment', 'adjustment', 'advance' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Detail.type',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/payment-type'}
        },
        ##
        # Request giving rise to the payment
        # A resource, such as a Claim, the evaluation of which could lead to payment.
        'request' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
          'type'=>'Reference',
          'path'=>'Detail.request',
          'min'=>0,
          'max'=>1
        },
        ##
        # Submitter of the request
        # The party which submitted the claim or financial transaction.
        'submitter' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization'],
          'type'=>'Reference',
          'path'=>'Detail.submitter',
          'min'=>0,
          'max'=>1
        },
        ##
        # Response committing to a payment
        # A resource, such as a ClaimResponse, which contains a commitment to payment.
        'response' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
          'type'=>'Reference',
          'path'=>'Detail.response',
          'min'=>0,
          'max'=>1
        },
        ##
        # Date of commitment to pay
        # The date from the response resource containing a commitment to pay.
        'date' => {
          'type'=>'date',
          'path'=>'Detail.date',
          'min'=>0,
          'max'=>1
        },
        ##
        # Contact for the response
        # A reference to the individual who is responsible for inquiries regarding the response and its payment.
        'responsible' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/PractitionerRole'],
          'type'=>'Reference',
          'path'=>'Detail.responsible',
          'min'=>0,
          'max'=>1
        },
        ##
        # Recipient of the payment
        # The party which is receiving the payment.
        'payee' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization'],
          'type'=>'Reference',
          'path'=>'Detail.payee',
          'min'=>0,
          'max'=>1
        },
        ##
        # Amount allocated to this payable
        # The monetary amount allocated from the total payment to the payable.
        'amount' => {
          'type'=>'Money',
          'path'=>'Detail.amount',
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
      # Business identifier of the payment detail
      # Unique identifier for the current payment item for the referenced payable.
      attr_accessor :identifier                     # 0-1 Identifier
      ##
      # Business identifier of the prior payment detail
      # Unique identifier for the prior payment item for the referenced payable.
      attr_accessor :predecessor                    # 0-1 Identifier
      ##
      # Category of payment
      # Code to indicate the nature of the payment.
      # For example: payment, adjustment, funds advance, etc.
      attr_accessor :type                           # 1-1 CodeableConcept
      ##
      # Request giving rise to the payment
      # A resource, such as a Claim, the evaluation of which could lead to payment.
      attr_accessor :request                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Resource)
      ##
      # Submitter of the request
      # The party which submitted the claim or financial transaction.
      attr_accessor :submitter                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization)
      ##
      # Response committing to a payment
      # A resource, such as a ClaimResponse, which contains a commitment to payment.
      attr_accessor :response                       # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Resource)
      ##
      # Date of commitment to pay
      # The date from the response resource containing a commitment to pay.
      attr_accessor :date                           # 0-1 date
      ##
      # Contact for the response
      # A reference to the individual who is responsible for inquiries regarding the response and its payment.
      attr_accessor :responsible                    # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/PractitionerRole)
      ##
      # Recipient of the payment
      # The party which is receiving the payment.
      attr_accessor :payee                          # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization)
      ##
      # Amount allocated to this payable
      # The monetary amount allocated from the total payment to the payable.
      attr_accessor :amount                         # 0-1 Money
    end

    ##
    # Note concerning processing
    # A note that describes or explains the processing in a human readable form.
    class ProcessNote < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'ProcessNote.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'ProcessNote.extension',
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
          'path'=>'ProcessNote.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # display | print | printoper
        # The business purpose of the note text.
        'type' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/note-type'=>[ 'display', 'print', 'printoper' ]
          },
          'type'=>'code',
          'path'=>'ProcessNote.type',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/note-type'}
        },
        ##
        # Note explanatory text
        # The explanation or description associated with the processing.
        'text' => {
          'type'=>'string',
          'path'=>'ProcessNote.text',
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
      # display | print | printoper
      # The business purpose of the note text.
      attr_accessor :type                           # 0-1 code
      ##
      # Note explanatory text
      # The explanation or description associated with the processing.
      attr_accessor :text                           # 0-1 string
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
    # Business Identifier for a payment reconciliation
    # A unique identifier assigned to this payment reconciliation.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # active | cancelled | draft | entered-in-error
    # The status of the resource instance.
    # This element is labeled as a modifier because the status contains codes that mark the resource as not currently valid.
    attr_accessor :status                         # 1-1 code
    ##
    # Period covered
    # The period of time for which payments have been gathered into this bulk payment for settlement.
    attr_accessor :period                         # 0-1 Period
    ##
    # Creation date
    # The date when the resource was created.
    attr_accessor :created                        # 1-1 dateTime
    ##
    # Party generating payment
    # The party who generated the payment.
    # This party is also responsible for the reconciliation.
    attr_accessor :paymentIssuer                  # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Reference to requesting resource
    # Original request resource reference.
    attr_accessor :request                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Task)
    ##
    # Responsible practitioner
    # The practitioner who is responsible for the services rendered to the patient.
    attr_accessor :requestor                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # queued | complete | error | partial
    # The outcome of a request for a reconciliation.
    # The resource may be used to indicate that: the request has been held (queued) for processing; that it has been processed and errors found (error); that no errors were found and that some of the adjudication has been undertaken (partial) or that all of the adjudication has been undertaken (complete).
    attr_accessor :outcome                        # 0-1 code
    ##
    # Disposition message
    # A human readable description of the status of the request for the reconciliation.
    attr_accessor :disposition                    # 0-1 string
    ##
    # When payment issued
    # The date of payment as indicated on the financial instrument.
    attr_accessor :paymentDate                    # 1-1 date
    ##
    # Total amount of Payment
    # Total payment amount as indicated on the financial instrument.
    attr_accessor :paymentAmount                  # 1-1 Money
    ##
    # Business identifier for the payment
    # Issuer's unique identifier for the payment instrument.
    # For example: EFT number or check number.
    attr_accessor :paymentIdentifier              # 0-1 Identifier
    ##
    # Settlement particulars
    # Distribution of the payment amount for a previously acknowledged payable.
    attr_accessor :detail                         # 0-* [ PaymentReconciliation::Detail ]
    ##
    # Printed form identifier
    # A code for the form to be used for printing the content.
    # May be needed to identify specific jurisdictional forms.
    attr_accessor :formCode                       # 0-1 CodeableConcept
    ##
    # Note concerning processing
    # A note that describes or explains the processing in a human readable form.
    attr_accessor :processNote                    # 0-* [ PaymentReconciliation::ProcessNote ]

    def resourceType
      'PaymentReconciliation'
    end
  end
end
