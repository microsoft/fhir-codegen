module FHIR

  ##
  # This resource provides the adjudication details from the processing of a Claim resource.
  class ClaimResponse < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['created', 'disposition', 'identifier', 'insurer', 'outcome', 'patient', 'payment-date', 'request', 'requestor', 'status', 'use']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'ClaimResponse.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'ClaimResponse.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'ClaimResponse.implicitRules',
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
        'path'=>'ClaimResponse.language',
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
        'path'=>'ClaimResponse.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'ClaimResponse.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'ClaimResponse.extension',
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
        'path'=>'ClaimResponse.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Business Identifier for a claim response
      # A unique identifier assigned to this claim response.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'ClaimResponse.identifier',
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
        'path'=>'ClaimResponse.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/fm-status'}
      },
      ##
      # More granular claim type
      # A finer grained suite of claim type codes which may convey additional information such as Inpatient vs Outpatient and/or a specialty service.
      # This may contain the local bill type codes, for example the US UB-04 bill type code or the CMS bill type.
      'type' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/claim-type'=>[ 'institutional', 'oral', 'pharmacy', 'professional', 'vision' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'ClaimResponse.type',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/claim-type'}
      },
      ##
      # More granular claim type
      # A finer grained suite of claim type codes which may convey additional information such as Inpatient vs Outpatient and/or a specialty service.
      # This may contain the local bill type codes, for example the US UB-04 bill type code or the CMS bill type.
      'subType' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/ex-claimsubtype'=>[ 'ortho', 'emergency' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'ClaimResponse.subType',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/claim-subtype'}
      },
      ##
      # claim | preauthorization | predetermination
      # A code to indicate whether the nature of the request is: to request adjudication of products and services previously rendered; or requesting authorization and adjudication for provision in the future; or requesting the non-binding adjudication of the listed products and services which could be provided in the future.
      'use' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/claim-use'=>[ 'claim', 'preauthorization', 'predetermination' ]
        },
        'type'=>'code',
        'path'=>'ClaimResponse.use',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/claim-use'}
      },
      ##
      # The recipient of the products and services
      # The party to whom the professional services and/or products have been supplied or are being considered and for whom actual for facast reimbursement is sought.
      'patient' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient'],
        'type'=>'Reference',
        'path'=>'ClaimResponse.patient',
        'min'=>1,
        'max'=>1
      },
      ##
      # Response creation date
      # The date this resource was created.
      'created' => {
        'type'=>'dateTime',
        'path'=>'ClaimResponse.created',
        'min'=>1,
        'max'=>1
      },
      ##
      # Party responsible for reimbursement
      # The party responsible for authorization, adjudication and reimbursement.
      'insurer' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'ClaimResponse.insurer',
        'min'=>1,
        'max'=>1
      },
      ##
      # Party responsible for the claim
      # The provider which is responsible for the claim, predetermination or preauthorization.
      # Typically this field would be 1..1 where this party is responsible for the claim but not necessarily professionally responsible for the provision of the individual products and services listed below.
      'requestor' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'ClaimResponse.requestor',
        'min'=>0,
        'max'=>1
      },
      ##
      # Id of resource triggering adjudication
      # Original request resource reference.
      'request' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Claim'],
        'type'=>'Reference',
        'path'=>'ClaimResponse.request',
        'min'=>0,
        'max'=>1
      },
      ##
      # queued | complete | error | partial
      # The outcome of the claim, predetermination, or preauthorization processing.
      # The resource may be used to indicate that: the request has been held (queued) for processing; that it has been processed and errors found (error); that no errors were found and that some of the adjudication has been undertaken (partial) or that all of the adjudication has been undertaken (complete).
      'outcome' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/remittance-outcome'=>[ 'queued', 'complete', 'error', 'partial' ]
        },
        'type'=>'code',
        'path'=>'ClaimResponse.outcome',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/remittance-outcome'}
      },
      ##
      # Disposition Message
      # A human readable description of the status of the adjudication.
      'disposition' => {
        'type'=>'string',
        'path'=>'ClaimResponse.disposition',
        'min'=>0,
        'max'=>1
      },
      ##
      # Preauthorization reference
      # Reference from the Insurer which is used in later communications which refers to this adjudication.
      # This value is only present on preauthorization adjudications.
      'preAuthRef' => {
        'type'=>'string',
        'path'=>'ClaimResponse.preAuthRef',
        'min'=>0,
        'max'=>1
      },
      ##
      # Preauthorization reference effective period
      # The time frame during which this authorization is effective.
      'preAuthPeriod' => {
        'type'=>'Period',
        'path'=>'ClaimResponse.preAuthPeriod',
        'min'=>0,
        'max'=>1
      },
      ##
      # Party to be paid any benefits payable
      # Type of Party to be reimbursed: subscriber, provider, other.
      'payeeType' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/payeetype'=>[ 'subscriber', 'provider', 'other' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'ClaimResponse.payeeType',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/payeetype'}
      },
      ##
      # Adjudication for claim line items
      # A claim line. Either a simple (a product or service) or a 'group' of details which can also be a simple items or groups of sub-details.
      'item' => {
        'type'=>'ClaimResponse::Item',
        'path'=>'ClaimResponse.item',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Insurer added line items
      # The first-tier service adjudications for payor added product or service lines.
      'addItem' => {
        'type'=>'ClaimResponse::AddItem',
        'path'=>'ClaimResponse.addItem',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Header-level adjudication
      # The adjudication results which are presented at the header level rather than at the line-item or add-item levels.
      'adjudication' => {
        'type'=>'ClaimResponse::Item::Adjudication',
        'path'=>'ClaimResponse.adjudication',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Adjudication totals
      # Categorized monetary totals for the adjudication.
      # Totals for amounts submitted, co-pays, benefits payable etc.
      'total' => {
        'type'=>'ClaimResponse::Total',
        'path'=>'ClaimResponse.total',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Payment Details
      # Payment details for the adjudication of the claim.
      'payment' => {
        'type'=>'ClaimResponse::Payment',
        'path'=>'ClaimResponse.payment',
        'min'=>0,
        'max'=>1
      },
      ##
      # Funds reserved status
      # A code, used only on a response to a preauthorization, to indicate whether the benefits payable have been reserved and for whom.
      # Fund would be release by a future claim quoting the preAuthRef of this response. Examples of values include: provider, patient, none.
      'fundsReserve' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/fundsreserve'=>[ 'patient', 'provider', 'none' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'ClaimResponse.fundsReserve',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/fundsreserve'}
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
        'path'=>'ClaimResponse.formCode',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/forms'}
      },
      ##
      # Printed reference or actual form
      # The actual form, by reference or inclusion, for printing the content or an EOB.
      # Needed to permit insurers to include the actual form.
      'form' => {
        'type'=>'Attachment',
        'path'=>'ClaimResponse.form',
        'min'=>0,
        'max'=>1
      },
      ##
      # Note concerning adjudication
      # A note that describes or explains adjudication results in a human readable form.
      'processNote' => {
        'type'=>'ClaimResponse::ProcessNote',
        'path'=>'ClaimResponse.processNote',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Request for additional information
      # Request for additional supporting or authorizing information.
      # For example: professional reports, documents, images, clinical resources, or accident reports.
      'communicationRequest' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/CommunicationRequest'],
        'type'=>'Reference',
        'path'=>'ClaimResponse.communicationRequest',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Patient insurance information
      # Financial instruments for reimbursement for the health care products and services specified on the claim.
      # All insurance coverages for the patient which may be applicable for reimbursement, of the products and services listed in the claim, are typically provided in the claim to allow insurers to confirm the ordering of the insurance coverages relative to local 'coordination of benefit' rules. One coverage (and only one) with 'focal=true' is to be used in the adjudication of this claim. Coverages appearing before the focal Coverage in the list, and where 'subrogation=false', should provide a reference to the ClaimResponse containing the adjudication results of the prior claim.
      'insurance' => {
        'type'=>'ClaimResponse::Insurance',
        'path'=>'ClaimResponse.insurance',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Processing errors
      # Errors encountered during the processing of the adjudication.
      # If the request contains errors then an error element should be provided and no adjudication related sections (item, addItem, or payment) should be present.
      'error' => {
        'type'=>'ClaimResponse::Error',
        'path'=>'ClaimResponse.error',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Adjudication for claim line items
    # A claim line. Either a simple (a product or service) or a 'group' of details which can also be a simple items or groups of sub-details.
    class Item < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Item.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Item.extension',
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
          'path'=>'Item.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Claim item instance identifier
        # A number to uniquely reference the claim item entries.
        'itemSequence' => {
          'type'=>'positiveInt',
          'path'=>'Item.itemSequence',
          'min'=>1,
          'max'=>1
        },
        ##
        # Applicable note numbers
        # The numbers associated with notes below which apply to the adjudication of this item.
        'noteNumber' => {
          'type'=>'positiveInt',
          'path'=>'Item.noteNumber',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Adjudication details
        # If this item is a group then the values here are a summary of the adjudication of the detail items. If this item is a simple product or service then this is the result of the adjudication of this item.
        'adjudication' => {
          'type'=>'ClaimResponse::Item::Adjudication',
          'path'=>'Item.adjudication',
          'min'=>1,
          'max'=>Float::INFINITY
        },
        ##
        # Adjudication for claim details
        # A claim detail. Either a simple (a product or service) or a 'group' of sub-details which are simple items.
        'detail' => {
          'type'=>'ClaimResponse::Item::Detail',
          'path'=>'Item.detail',
          'min'=>0,
          'max'=>Float::INFINITY
        }
      }

      ##
      # Adjudication details
      # If this item is a group then the values here are a summary of the adjudication of the detail items. If this item is a simple product or service then this is the result of the adjudication of this item.
      class Adjudication < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'id',
            'path'=>'Adjudication.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Adjudication.extension',
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
            'path'=>'Adjudication.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Type of adjudication information
          # A code to indicate the information type of this adjudication record. Information types may include the value submitted, maximum values or percentages allowed or payable under the plan, amounts that: the patient is responsible for in aggregate or pertaining to this item; amounts paid by other coverages; and, the benefit payable for this item.
          # For example codes indicating: Co-Pay, deductible, eligible, benefit, tax, etc.
          'category' => {
            'valid_codes'=>{
              'http://terminology.hl7.org/CodeSystem/adjudication'=>[ 'submitted', 'copay', 'eligible', 'deductible', 'unallocdeduct', 'eligpercent', 'tax', 'benefit' ]
            },
            'type'=>'CodeableConcept',
            'path'=>'Adjudication.category',
            'min'=>1,
            'max'=>1,
            'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/adjudication'}
          },
          ##
          # Explanation of adjudication outcome
          # A code supporting the understanding of the adjudication result and explaining variance from expected amount.
          # For example may indicate that the funds for this benefit type have been exhausted.
          'reason' => {
            'valid_codes'=>{
              'http://terminology.hl7.org/CodeSystem/adjudication-reason'=>[ 'ar001', 'ar002' ]
            },
            'type'=>'CodeableConcept',
            'path'=>'Adjudication.reason',
            'min'=>0,
            'max'=>1,
            'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/adjudication-reason'}
          },
          ##
          # Monetary amount associated with the category.
          # For example: amount submitted, eligible amount, co-payment, and benefit payable.
          'amount' => {
            'type'=>'Money',
            'path'=>'Adjudication.amount',
            'min'=>0,
            'max'=>1
          },
          ##
          # Non-monetary value
          # A non-monetary value associated with the category. Mutually exclusive to the amount element above.
          # For example: eligible percentage or co-payment percentage.
          'value' => {
            'type'=>'decimal',
            'path'=>'Adjudication.value',
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
        # Type of adjudication information
        # A code to indicate the information type of this adjudication record. Information types may include the value submitted, maximum values or percentages allowed or payable under the plan, amounts that: the patient is responsible for in aggregate or pertaining to this item; amounts paid by other coverages; and, the benefit payable for this item.
        # For example codes indicating: Co-Pay, deductible, eligible, benefit, tax, etc.
        attr_accessor :category                       # 1-1 CodeableConcept
        ##
        # Explanation of adjudication outcome
        # A code supporting the understanding of the adjudication result and explaining variance from expected amount.
        # For example may indicate that the funds for this benefit type have been exhausted.
        attr_accessor :reason                         # 0-1 CodeableConcept
        ##
        # Monetary amount associated with the category.
        # For example: amount submitted, eligible amount, co-payment, and benefit payable.
        attr_accessor :amount                         # 0-1 Money
        ##
        # Non-monetary value
        # A non-monetary value associated with the category. Mutually exclusive to the amount element above.
        # For example: eligible percentage or co-payment percentage.
        attr_accessor :value                          # 0-1 decimal
      end

      ##
      # Adjudication for claim details
      # A claim detail. Either a simple (a product or service) or a 'group' of sub-details which are simple items.
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
          # Claim detail instance identifier
          # A number to uniquely reference the claim detail entry.
          'detailSequence' => {
            'type'=>'positiveInt',
            'path'=>'Detail.detailSequence',
            'min'=>1,
            'max'=>1
          },
          ##
          # Applicable note numbers
          # The numbers associated with notes below which apply to the adjudication of this item.
          'noteNumber' => {
            'type'=>'positiveInt',
            'path'=>'Detail.noteNumber',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Detail level adjudication details
          # The adjudication results.
          'adjudication' => {
            'type'=>'ClaimResponse::Item::Adjudication',
            'path'=>'Detail.adjudication',
            'min'=>1,
            'max'=>Float::INFINITY
          },
          ##
          # Adjudication for claim sub-details
          # A sub-detail adjudication of a simple product or service.
          'subDetail' => {
            'type'=>'ClaimResponse::Item::Detail::SubDetail',
            'path'=>'Detail.subDetail',
            'min'=>0,
            'max'=>Float::INFINITY
          }
        }

        ##
        # Adjudication for claim sub-details
        # A sub-detail adjudication of a simple product or service.
        class SubDetail < FHIR::Model
          include FHIR::Hashable
          include FHIR::Json
          include FHIR::Xml

          METADATA = {
            ##
            # Unique id for inter-element referencing
            # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
            'id' => {
              'type'=>'id',
              'path'=>'SubDetail.id',
              'min'=>0,
              'max'=>1
            },
            ##
            # Additional content defined by implementations
            # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
            # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
            'extension' => {
              'type'=>'Extension',
              'path'=>'SubDetail.extension',
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
              'path'=>'SubDetail.modifierExtension',
              'min'=>0,
              'max'=>Float::INFINITY
            },
            ##
            # Claim sub-detail instance identifier
            # A number to uniquely reference the claim sub-detail entry.
            'subDetailSequence' => {
              'type'=>'positiveInt',
              'path'=>'SubDetail.subDetailSequence',
              'min'=>1,
              'max'=>1
            },
            ##
            # Applicable note numbers
            # The numbers associated with notes below which apply to the adjudication of this item.
            'noteNumber' => {
              'type'=>'positiveInt',
              'path'=>'SubDetail.noteNumber',
              'min'=>0,
              'max'=>Float::INFINITY
            },
            ##
            # Subdetail level adjudication details
            # The adjudication results.
            'adjudication' => {
              'type'=>'ClaimResponse::Item::Adjudication',
              'path'=>'SubDetail.adjudication',
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
          # Claim sub-detail instance identifier
          # A number to uniquely reference the claim sub-detail entry.
          attr_accessor :subDetailSequence              # 1-1 positiveInt
          ##
          # Applicable note numbers
          # The numbers associated with notes below which apply to the adjudication of this item.
          attr_accessor :noteNumber                     # 0-* [ positiveInt ]
          ##
          # Subdetail level adjudication details
          # The adjudication results.
          attr_accessor :adjudication                   # 0-* [ ClaimResponse::Item::Adjudication ]
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
        # Claim detail instance identifier
        # A number to uniquely reference the claim detail entry.
        attr_accessor :detailSequence                 # 1-1 positiveInt
        ##
        # Applicable note numbers
        # The numbers associated with notes below which apply to the adjudication of this item.
        attr_accessor :noteNumber                     # 0-* [ positiveInt ]
        ##
        # Detail level adjudication details
        # The adjudication results.
        attr_accessor :adjudication                   # 1-* [ ClaimResponse::Item::Adjudication ]
        ##
        # Adjudication for claim sub-details
        # A sub-detail adjudication of a simple product or service.
        attr_accessor :subDetail                      # 0-* [ ClaimResponse::Item::Detail::SubDetail ]
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
      # Claim item instance identifier
      # A number to uniquely reference the claim item entries.
      attr_accessor :itemSequence                   # 1-1 positiveInt
      ##
      # Applicable note numbers
      # The numbers associated with notes below which apply to the adjudication of this item.
      attr_accessor :noteNumber                     # 0-* [ positiveInt ]
      ##
      # Adjudication details
      # If this item is a group then the values here are a summary of the adjudication of the detail items. If this item is a simple product or service then this is the result of the adjudication of this item.
      attr_accessor :adjudication                   # 1-* [ ClaimResponse::Item::Adjudication ]
      ##
      # Adjudication for claim details
      # A claim detail. Either a simple (a product or service) or a 'group' of sub-details which are simple items.
      attr_accessor :detail                         # 0-* [ ClaimResponse::Item::Detail ]
    end

    ##
    # Insurer added line items
    # The first-tier service adjudications for payor added product or service lines.
    class AddItem < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'serviced[x]' => ['date', 'Period'],
        'location[x]' => ['Address', 'CodeableConcept', 'Reference']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'AddItem.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'AddItem.extension',
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
          'path'=>'AddItem.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Item sequence number
        # Claim items which this service line is intended to replace.
        'itemSequence' => {
          'type'=>'positiveInt',
          'path'=>'AddItem.itemSequence',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Detail sequence number
        # The sequence number of the details within the claim item which this line is intended to replace.
        'detailSequence' => {
          'type'=>'positiveInt',
          'path'=>'AddItem.detailSequence',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Subdetail sequence number
        # The sequence number of the sub-details within the details within the claim item which this line is intended to replace.
        'subdetailSequence' => {
          'type'=>'positiveInt',
          'path'=>'AddItem.subdetailSequence',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Authorized providers
        # The providers who are authorized for the services rendered to the patient.
        'provider' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization'],
          'type'=>'Reference',
          'path'=>'AddItem.provider',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Billing, service, product, or drug code
        # When the value is a group code then this item collects a set of related claim details, otherwise this contains the product, service, drug or other billing code for the item.
        # If this is an actual service or product line, i.e. not a Group, then use code to indicate the Professional Service or Product supplied (e.g. CTP, HCPCS, USCLS, ICD10, NCPDP, DIN, RxNorm, ACHI, CCI). If a grouping item then use a group code to indicate the type of thing being grouped e.g. 'glasses' or 'compound'.
        'productOrService' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/ex-USCLS'=>[ '1101', '1102', '1103', '1201', '1205', '2101', '2102', '2141', '2601', '11101', '11102', '11103', '11104', '21211', '21212', '27211', '67211', '99111', '99333', '99555' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'AddItem.productOrService',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/service-uscls'}
        },
        ##
        # Service/Product billing modifiers
        # Item typification or modifiers codes to convey additional context for the product or service.
        # For example in Oral whether the treatment is cosmetic or associated with TMJ, or for Medical whether the treatment was outside the clinic or outside of office hours.
        'modifier' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/modifiers'=>[ 'a', 'b', 'c', 'e', 'rooh', 'x' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'AddItem.modifier',
          'min'=>0,
          'max'=>Float::INFINITY,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/claim-modifiers'}
        },
        ##
        # Program the product or service is provided under
        # Identifies the program under which this may be recovered.
        # For example: Neonatal program, child dental program or drug users recovery program.
        'programCode' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/ex-programcode'=>[ 'as', 'hd', 'auscr', 'none' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'AddItem.programCode',
          'min'=>0,
          'max'=>Float::INFINITY,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/ex-program-code'}
        },
        ##
        # Date or dates of service or product delivery
        # The date or dates when the service or product was supplied, performed or completed.
        'servicedDate' => {
          'type'=>'Date',
          'path'=>'AddItem.serviced[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Date or dates of service or product delivery
        # The date or dates when the service or product was supplied, performed or completed.
        'servicedPeriod' => {
          'type'=>'Period',
          'path'=>'AddItem.serviced[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Place of service or where product was supplied
        # Where the product or service was provided.
        'locationAddress' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/ex-serviceplace'=>[ '01', '03', '04', '05', '06', '07', '08', '09', '11', '12', '13', '14', '15', '19', '20', '21', '41' ]
          },
          'type'=>'Address',
          'path'=>'AddItem.location[x]',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/service-place'}
        },
        ##
        # Place of service or where product was supplied
        # Where the product or service was provided.
        'locationCodeableConcept' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/ex-serviceplace'=>[ '01', '03', '04', '05', '06', '07', '08', '09', '11', '12', '13', '14', '15', '19', '20', '21', '41' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'AddItem.location[x]',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/service-place'}
        },
        ##
        # Place of service or where product was supplied
        # Where the product or service was provided.
        'locationReference' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/ex-serviceplace'=>[ '01', '03', '04', '05', '06', '07', '08', '09', '11', '12', '13', '14', '15', '19', '20', '21', '41' ]
          },
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Location'],
          'type'=>'Reference',
          'path'=>'AddItem.location[x]',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/service-place'}
        },
        ##
        # Count of products or services
        # The number of repetitions of a service or product.
        'quantity' => {
          'type'=>'Quantity',
          'path'=>'AddItem.quantity',
          'min'=>0,
          'max'=>1
        },
        ##
        # Fee, charge or cost per item
        # If the item is not a group then this is the fee for the product or service, otherwise this is the total of the fees for the details of the group.
        'unitPrice' => {
          'type'=>'Money',
          'path'=>'AddItem.unitPrice',
          'min'=>0,
          'max'=>1
        },
        ##
        # Price scaling factor
        # A real number that represents a multiplier used in determining the overall value of services delivered and/or goods received. The concept of a Factor allows for a discount or surcharge multiplier to be applied to a monetary amount.
        # To show a 10% senior's discount, the value entered is: 0.90 (1.00 - 0.10).
        'factor' => {
          'type'=>'decimal',
          'path'=>'AddItem.factor',
          'min'=>0,
          'max'=>1
        },
        ##
        # Total item cost
        # The quantity times the unit price for an additional service or product or charge.
        # For example, the formula: quantity * unitPrice * factor  = net. Quantity and factor are assumed to be 1 if not supplied.
        'net' => {
          'type'=>'Money',
          'path'=>'AddItem.net',
          'min'=>0,
          'max'=>1
        },
        ##
        # Anatomical location
        # Physical service site on the patient (limb, tooth, etc.).
        # For example: Providing a tooth code allows an insurer to identify a provider performing a filling on a tooth that was previously removed.
        'bodySite' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/ex-tooth'=>[ '0', '1', '2', '3', '4', '5', '6', '7', '8', '11', '12', '13', '14', '15', '16', '17', '18', '21', '22', '23', '24', '25', '26', '27', '28', '31', '32', '33', '34', '35', '36', '37', '38', '41', '42', '43', '44', '45', '46', '47', '48' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'AddItem.bodySite',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/tooth'}
        },
        ##
        # Anatomical sub-location
        # A region or surface of the bodySite, e.g. limb region or tooth surface(s).
        'subSite' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/FDI-surface'=>[ 'M', 'O', 'I', 'D', 'B', 'V', 'L', 'MO', 'DO', 'DI', 'MOD' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'AddItem.subSite',
          'min'=>0,
          'max'=>Float::INFINITY,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/surface'}
        },
        ##
        # Applicable note numbers
        # The numbers associated with notes below which apply to the adjudication of this item.
        'noteNumber' => {
          'type'=>'positiveInt',
          'path'=>'AddItem.noteNumber',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Added items adjudication
        # The adjudication results.
        'adjudication' => {
          'type'=>'ClaimResponse::Item::Adjudication',
          'path'=>'AddItem.adjudication',
          'min'=>1,
          'max'=>Float::INFINITY
        },
        ##
        # Insurer added line details
        # The second-tier service adjudications for payor added services.
        'detail' => {
          'type'=>'ClaimResponse::AddItem::Detail',
          'path'=>'AddItem.detail',
          'min'=>0,
          'max'=>Float::INFINITY
        }
      }

      ##
      # Insurer added line details
      # The second-tier service adjudications for payor added services.
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
          # Billing, service, product, or drug code
          # When the value is a group code then this item collects a set of related claim details, otherwise this contains the product, service, drug or other billing code for the item.
          # If this is an actual service or product line, i.e. not a Group, then use code to indicate the Professional Service or Product supplied (e.g. CTP, HCPCS, USCLS, ICD10, NCPDP, DIN, RxNorm, ACHI, CCI). If a grouping item then use a group code to indicate the type of thing being grouped e.g. 'glasses' or 'compound'.
          'productOrService' => {
            'valid_codes'=>{
              'http://terminology.hl7.org/CodeSystem/ex-USCLS'=>[ '1101', '1102', '1103', '1201', '1205', '2101', '2102', '2141', '2601', '11101', '11102', '11103', '11104', '21211', '21212', '27211', '67211', '99111', '99333', '99555' ]
            },
            'type'=>'CodeableConcept',
            'path'=>'Detail.productOrService',
            'min'=>1,
            'max'=>1,
            'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/service-uscls'}
          },
          ##
          # Service/Product billing modifiers
          # Item typification or modifiers codes to convey additional context for the product or service.
          # For example in Oral whether the treatment is cosmetic or associated with TMJ, or for Medical whether the treatment was outside the clinic or outside of office hours.
          'modifier' => {
            'valid_codes'=>{
              'http://terminology.hl7.org/CodeSystem/modifiers'=>[ 'a', 'b', 'c', 'e', 'rooh', 'x' ]
            },
            'type'=>'CodeableConcept',
            'path'=>'Detail.modifier',
            'min'=>0,
            'max'=>Float::INFINITY,
            'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/claim-modifiers'}
          },
          ##
          # Count of products or services
          # The number of repetitions of a service or product.
          'quantity' => {
            'type'=>'Quantity',
            'path'=>'Detail.quantity',
            'min'=>0,
            'max'=>1
          },
          ##
          # Fee, charge or cost per item
          # If the item is not a group then this is the fee for the product or service, otherwise this is the total of the fees for the details of the group.
          'unitPrice' => {
            'type'=>'Money',
            'path'=>'Detail.unitPrice',
            'min'=>0,
            'max'=>1
          },
          ##
          # Price scaling factor
          # A real number that represents a multiplier used in determining the overall value of services delivered and/or goods received. The concept of a Factor allows for a discount or surcharge multiplier to be applied to a monetary amount.
          # To show a 10% senior's discount, the value entered is: 0.90 (1.00 - 0.10).
          'factor' => {
            'type'=>'decimal',
            'path'=>'Detail.factor',
            'min'=>0,
            'max'=>1
          },
          ##
          # Total item cost
          # The quantity times the unit price for an additional service or product or charge.
          # For example, the formula: quantity * unitPrice * factor  = net. Quantity and factor are assumed to be 1 if not supplied.
          'net' => {
            'type'=>'Money',
            'path'=>'Detail.net',
            'min'=>0,
            'max'=>1
          },
          ##
          # Applicable note numbers
          # The numbers associated with notes below which apply to the adjudication of this item.
          'noteNumber' => {
            'type'=>'positiveInt',
            'path'=>'Detail.noteNumber',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Added items detail adjudication
          # The adjudication results.
          'adjudication' => {
            'type'=>'ClaimResponse::Item::Adjudication',
            'path'=>'Detail.adjudication',
            'min'=>1,
            'max'=>Float::INFINITY
          },
          ##
          # Insurer added line items
          # The third-tier service adjudications for payor added services.
          'subDetail' => {
            'type'=>'ClaimResponse::AddItem::Detail::SubDetail',
            'path'=>'Detail.subDetail',
            'min'=>0,
            'max'=>Float::INFINITY
          }
        }

        ##
        # Insurer added line items
        # The third-tier service adjudications for payor added services.
        class SubDetail < FHIR::Model
          include FHIR::Hashable
          include FHIR::Json
          include FHIR::Xml

          METADATA = {
            ##
            # Unique id for inter-element referencing
            # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
            'id' => {
              'type'=>'id',
              'path'=>'SubDetail.id',
              'min'=>0,
              'max'=>1
            },
            ##
            # Additional content defined by implementations
            # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
            # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
            'extension' => {
              'type'=>'Extension',
              'path'=>'SubDetail.extension',
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
              'path'=>'SubDetail.modifierExtension',
              'min'=>0,
              'max'=>Float::INFINITY
            },
            ##
            # Billing, service, product, or drug code
            # When the value is a group code then this item collects a set of related claim details, otherwise this contains the product, service, drug or other billing code for the item.
            # If this is an actual service or product line, i.e. not a Group, then use code to indicate the Professional Service or Product supplied (e.g. CTP, HCPCS, USCLS, ICD10, NCPDP, DIN, RxNorm, ACHI, CCI). If a grouping item then use a group code to indicate the type of thing being grouped e.g. 'glasses' or 'compound'.
            'productOrService' => {
              'valid_codes'=>{
                'http://terminology.hl7.org/CodeSystem/ex-USCLS'=>[ '1101', '1102', '1103', '1201', '1205', '2101', '2102', '2141', '2601', '11101', '11102', '11103', '11104', '21211', '21212', '27211', '67211', '99111', '99333', '99555' ]
              },
              'type'=>'CodeableConcept',
              'path'=>'SubDetail.productOrService',
              'min'=>1,
              'max'=>1,
              'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/service-uscls'}
            },
            ##
            # Service/Product billing modifiers
            # Item typification or modifiers codes to convey additional context for the product or service.
            # For example in Oral whether the treatment is cosmetic or associated with TMJ, or for Medical whether the treatment was outside the clinic or outside of office hours.
            'modifier' => {
              'valid_codes'=>{
                'http://terminology.hl7.org/CodeSystem/modifiers'=>[ 'a', 'b', 'c', 'e', 'rooh', 'x' ]
              },
              'type'=>'CodeableConcept',
              'path'=>'SubDetail.modifier',
              'min'=>0,
              'max'=>Float::INFINITY,
              'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/claim-modifiers'}
            },
            ##
            # Count of products or services
            # The number of repetitions of a service or product.
            'quantity' => {
              'type'=>'Quantity',
              'path'=>'SubDetail.quantity',
              'min'=>0,
              'max'=>1
            },
            ##
            # Fee, charge or cost per item
            # If the item is not a group then this is the fee for the product or service, otherwise this is the total of the fees for the details of the group.
            'unitPrice' => {
              'type'=>'Money',
              'path'=>'SubDetail.unitPrice',
              'min'=>0,
              'max'=>1
            },
            ##
            # Price scaling factor
            # A real number that represents a multiplier used in determining the overall value of services delivered and/or goods received. The concept of a Factor allows for a discount or surcharge multiplier to be applied to a monetary amount.
            # To show a 10% senior's discount, the value entered is: 0.90 (1.00 - 0.10).
            'factor' => {
              'type'=>'decimal',
              'path'=>'SubDetail.factor',
              'min'=>0,
              'max'=>1
            },
            ##
            # Total item cost
            # The quantity times the unit price for an additional service or product or charge.
            # For example, the formula: quantity * unitPrice * factor  = net. Quantity and factor are assumed to be 1 if not supplied.
            'net' => {
              'type'=>'Money',
              'path'=>'SubDetail.net',
              'min'=>0,
              'max'=>1
            },
            ##
            # Applicable note numbers
            # The numbers associated with notes below which apply to the adjudication of this item.
            'noteNumber' => {
              'type'=>'positiveInt',
              'path'=>'SubDetail.noteNumber',
              'min'=>0,
              'max'=>Float::INFINITY
            },
            ##
            # Added items detail adjudication
            # The adjudication results.
            'adjudication' => {
              'type'=>'ClaimResponse::Item::Adjudication',
              'path'=>'SubDetail.adjudication',
              'min'=>1,
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
          # Billing, service, product, or drug code
          # When the value is a group code then this item collects a set of related claim details, otherwise this contains the product, service, drug or other billing code for the item.
          # If this is an actual service or product line, i.e. not a Group, then use code to indicate the Professional Service or Product supplied (e.g. CTP, HCPCS, USCLS, ICD10, NCPDP, DIN, RxNorm, ACHI, CCI). If a grouping item then use a group code to indicate the type of thing being grouped e.g. 'glasses' or 'compound'.
          attr_accessor :productOrService               # 1-1 CodeableConcept
          ##
          # Service/Product billing modifiers
          # Item typification or modifiers codes to convey additional context for the product or service.
          # For example in Oral whether the treatment is cosmetic or associated with TMJ, or for Medical whether the treatment was outside the clinic or outside of office hours.
          attr_accessor :modifier                       # 0-* [ CodeableConcept ]
          ##
          # Count of products or services
          # The number of repetitions of a service or product.
          attr_accessor :quantity                       # 0-1 Quantity
          ##
          # Fee, charge or cost per item
          # If the item is not a group then this is the fee for the product or service, otherwise this is the total of the fees for the details of the group.
          attr_accessor :unitPrice                      # 0-1 Money
          ##
          # Price scaling factor
          # A real number that represents a multiplier used in determining the overall value of services delivered and/or goods received. The concept of a Factor allows for a discount or surcharge multiplier to be applied to a monetary amount.
          # To show a 10% senior's discount, the value entered is: 0.90 (1.00 - 0.10).
          attr_accessor :factor                         # 0-1 decimal
          ##
          # Total item cost
          # The quantity times the unit price for an additional service or product or charge.
          # For example, the formula: quantity * unitPrice * factor  = net. Quantity and factor are assumed to be 1 if not supplied.
          attr_accessor :net                            # 0-1 Money
          ##
          # Applicable note numbers
          # The numbers associated with notes below which apply to the adjudication of this item.
          attr_accessor :noteNumber                     # 0-* [ positiveInt ]
          ##
          # Added items detail adjudication
          # The adjudication results.
          attr_accessor :adjudication                   # 1-* [ ClaimResponse::Item::Adjudication ]
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
        # Billing, service, product, or drug code
        # When the value is a group code then this item collects a set of related claim details, otherwise this contains the product, service, drug or other billing code for the item.
        # If this is an actual service or product line, i.e. not a Group, then use code to indicate the Professional Service or Product supplied (e.g. CTP, HCPCS, USCLS, ICD10, NCPDP, DIN, RxNorm, ACHI, CCI). If a grouping item then use a group code to indicate the type of thing being grouped e.g. 'glasses' or 'compound'.
        attr_accessor :productOrService               # 1-1 CodeableConcept
        ##
        # Service/Product billing modifiers
        # Item typification or modifiers codes to convey additional context for the product or service.
        # For example in Oral whether the treatment is cosmetic or associated with TMJ, or for Medical whether the treatment was outside the clinic or outside of office hours.
        attr_accessor :modifier                       # 0-* [ CodeableConcept ]
        ##
        # Count of products or services
        # The number of repetitions of a service or product.
        attr_accessor :quantity                       # 0-1 Quantity
        ##
        # Fee, charge or cost per item
        # If the item is not a group then this is the fee for the product or service, otherwise this is the total of the fees for the details of the group.
        attr_accessor :unitPrice                      # 0-1 Money
        ##
        # Price scaling factor
        # A real number that represents a multiplier used in determining the overall value of services delivered and/or goods received. The concept of a Factor allows for a discount or surcharge multiplier to be applied to a monetary amount.
        # To show a 10% senior's discount, the value entered is: 0.90 (1.00 - 0.10).
        attr_accessor :factor                         # 0-1 decimal
        ##
        # Total item cost
        # The quantity times the unit price for an additional service or product or charge.
        # For example, the formula: quantity * unitPrice * factor  = net. Quantity and factor are assumed to be 1 if not supplied.
        attr_accessor :net                            # 0-1 Money
        ##
        # Applicable note numbers
        # The numbers associated with notes below which apply to the adjudication of this item.
        attr_accessor :noteNumber                     # 0-* [ positiveInt ]
        ##
        # Added items detail adjudication
        # The adjudication results.
        attr_accessor :adjudication                   # 1-* [ ClaimResponse::Item::Adjudication ]
        ##
        # Insurer added line items
        # The third-tier service adjudications for payor added services.
        attr_accessor :subDetail                      # 0-* [ ClaimResponse::AddItem::Detail::SubDetail ]
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
      # Item sequence number
      # Claim items which this service line is intended to replace.
      attr_accessor :itemSequence                   # 0-* [ positiveInt ]
      ##
      # Detail sequence number
      # The sequence number of the details within the claim item which this line is intended to replace.
      attr_accessor :detailSequence                 # 0-* [ positiveInt ]
      ##
      # Subdetail sequence number
      # The sequence number of the sub-details within the details within the claim item which this line is intended to replace.
      attr_accessor :subdetailSequence              # 0-* [ positiveInt ]
      ##
      # Authorized providers
      # The providers who are authorized for the services rendered to the patient.
      attr_accessor :provider                       # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization) ]
      ##
      # Billing, service, product, or drug code
      # When the value is a group code then this item collects a set of related claim details, otherwise this contains the product, service, drug or other billing code for the item.
      # If this is an actual service or product line, i.e. not a Group, then use code to indicate the Professional Service or Product supplied (e.g. CTP, HCPCS, USCLS, ICD10, NCPDP, DIN, RxNorm, ACHI, CCI). If a grouping item then use a group code to indicate the type of thing being grouped e.g. 'glasses' or 'compound'.
      attr_accessor :productOrService               # 1-1 CodeableConcept
      ##
      # Service/Product billing modifiers
      # Item typification or modifiers codes to convey additional context for the product or service.
      # For example in Oral whether the treatment is cosmetic or associated with TMJ, or for Medical whether the treatment was outside the clinic or outside of office hours.
      attr_accessor :modifier                       # 0-* [ CodeableConcept ]
      ##
      # Program the product or service is provided under
      # Identifies the program under which this may be recovered.
      # For example: Neonatal program, child dental program or drug users recovery program.
      attr_accessor :programCode                    # 0-* [ CodeableConcept ]
      ##
      # Date or dates of service or product delivery
      # The date or dates when the service or product was supplied, performed or completed.
      attr_accessor :servicedDate                   # 0-1 Date
      ##
      # Date or dates of service or product delivery
      # The date or dates when the service or product was supplied, performed or completed.
      attr_accessor :servicedPeriod                 # 0-1 Period
      ##
      # Place of service or where product was supplied
      # Where the product or service was provided.
      attr_accessor :locationAddress                # 0-1 Address
      ##
      # Place of service or where product was supplied
      # Where the product or service was provided.
      attr_accessor :locationCodeableConcept        # 0-1 CodeableConcept
      ##
      # Place of service or where product was supplied
      # Where the product or service was provided.
      attr_accessor :locationReference              # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Location)
      ##
      # Count of products or services
      # The number of repetitions of a service or product.
      attr_accessor :quantity                       # 0-1 Quantity
      ##
      # Fee, charge or cost per item
      # If the item is not a group then this is the fee for the product or service, otherwise this is the total of the fees for the details of the group.
      attr_accessor :unitPrice                      # 0-1 Money
      ##
      # Price scaling factor
      # A real number that represents a multiplier used in determining the overall value of services delivered and/or goods received. The concept of a Factor allows for a discount or surcharge multiplier to be applied to a monetary amount.
      # To show a 10% senior's discount, the value entered is: 0.90 (1.00 - 0.10).
      attr_accessor :factor                         # 0-1 decimal
      ##
      # Total item cost
      # The quantity times the unit price for an additional service or product or charge.
      # For example, the formula: quantity * unitPrice * factor  = net. Quantity and factor are assumed to be 1 if not supplied.
      attr_accessor :net                            # 0-1 Money
      ##
      # Anatomical location
      # Physical service site on the patient (limb, tooth, etc.).
      # For example: Providing a tooth code allows an insurer to identify a provider performing a filling on a tooth that was previously removed.
      attr_accessor :bodySite                       # 0-1 CodeableConcept
      ##
      # Anatomical sub-location
      # A region or surface of the bodySite, e.g. limb region or tooth surface(s).
      attr_accessor :subSite                        # 0-* [ CodeableConcept ]
      ##
      # Applicable note numbers
      # The numbers associated with notes below which apply to the adjudication of this item.
      attr_accessor :noteNumber                     # 0-* [ positiveInt ]
      ##
      # Added items adjudication
      # The adjudication results.
      attr_accessor :adjudication                   # 1-* [ ClaimResponse::Item::Adjudication ]
      ##
      # Insurer added line details
      # The second-tier service adjudications for payor added services.
      attr_accessor :detail                         # 0-* [ ClaimResponse::AddItem::Detail ]
    end

    ##
    # Adjudication totals
    # Categorized monetary totals for the adjudication.
    # Totals for amounts submitted, co-pays, benefits payable etc.
    class Total < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Total.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Total.extension',
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
          'path'=>'Total.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Type of adjudication information
        # A code to indicate the information type of this adjudication record. Information types may include: the value submitted, maximum values or percentages allowed or payable under the plan, amounts that the patient is responsible for in aggregate or pertaining to this item, amounts paid by other coverages, and the benefit payable for this item.
        # For example codes indicating: Co-Pay, deductible, eligible, benefit, tax, etc.
        'category' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/adjudication'=>[ 'submitted', 'copay', 'eligible', 'deductible', 'unallocdeduct', 'eligpercent', 'tax', 'benefit' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Total.category',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/adjudication'}
        },
        ##
        # Financial total for the category
        # Monetary total amount associated with the category.
        'amount' => {
          'type'=>'Money',
          'path'=>'Total.amount',
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
      # Type of adjudication information
      # A code to indicate the information type of this adjudication record. Information types may include: the value submitted, maximum values or percentages allowed or payable under the plan, amounts that the patient is responsible for in aggregate or pertaining to this item, amounts paid by other coverages, and the benefit payable for this item.
      # For example codes indicating: Co-Pay, deductible, eligible, benefit, tax, etc.
      attr_accessor :category                       # 1-1 CodeableConcept
      ##
      # Financial total for the category
      # Monetary total amount associated with the category.
      attr_accessor :amount                         # 1-1 Money
    end

    ##
    # Payment Details
    # Payment details for the adjudication of the claim.
    class Payment < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Payment.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Payment.extension',
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
          'path'=>'Payment.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Partial or complete payment
        # Whether this represents partial or complete payment of the benefits payable.
        'type' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/ex-paymenttype'=>[ 'complete', 'partial' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Payment.type',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/ex-paymenttype'}
        },
        ##
        # Payment adjustment for non-claim issues
        # Total amount of all adjustments to this payment included in this transaction which are not related to this claim's adjudication.
        # Insurers will deduct amounts owing from the provider (adjustment), such as a prior overpayment, from the amount owing to the provider (benefits payable) when payment is made to the provider.
        'adjustment' => {
          'type'=>'Money',
          'path'=>'Payment.adjustment',
          'min'=>0,
          'max'=>1
        },
        ##
        # Explanation for the adjustment
        # Reason for the payment adjustment.
        'adjustmentReason' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/payment-adjustment-reason'=>[ 'a001', 'a002' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Payment.adjustmentReason',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/payment-adjustment-reason'}
        },
        ##
        # Expected date of payment
        # Estimated date the payment will be issued or the actual issue date of payment.
        'date' => {
          'type'=>'date',
          'path'=>'Payment.date',
          'min'=>0,
          'max'=>1
        },
        ##
        # Payable amount after adjustment
        # Benefits payable less any payment adjustment.
        'amount' => {
          'type'=>'Money',
          'path'=>'Payment.amount',
          'min'=>1,
          'max'=>1
        },
        ##
        # Business identifier for the payment
        # Issuer's unique identifier for the payment instrument.
        # For example: EFT number or check number.
        'identifier' => {
          'type'=>'Identifier',
          'path'=>'Payment.identifier',
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
      # Partial or complete payment
      # Whether this represents partial or complete payment of the benefits payable.
      attr_accessor :type                           # 1-1 CodeableConcept
      ##
      # Payment adjustment for non-claim issues
      # Total amount of all adjustments to this payment included in this transaction which are not related to this claim's adjudication.
      # Insurers will deduct amounts owing from the provider (adjustment), such as a prior overpayment, from the amount owing to the provider (benefits payable) when payment is made to the provider.
      attr_accessor :adjustment                     # 0-1 Money
      ##
      # Explanation for the adjustment
      # Reason for the payment adjustment.
      attr_accessor :adjustmentReason               # 0-1 CodeableConcept
      ##
      # Expected date of payment
      # Estimated date the payment will be issued or the actual issue date of payment.
      attr_accessor :date                           # 0-1 date
      ##
      # Payable amount after adjustment
      # Benefits payable less any payment adjustment.
      attr_accessor :amount                         # 1-1 Money
      ##
      # Business identifier for the payment
      # Issuer's unique identifier for the payment instrument.
      # For example: EFT number or check number.
      attr_accessor :identifier                     # 0-1 Identifier
    end

    ##
    # Note concerning adjudication
    # A note that describes or explains adjudication results in a human readable form.
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
        # Note instance identifier
        # A number to uniquely identify a note entry.
        'number' => {
          'type'=>'positiveInt',
          'path'=>'ProcessNote.number',
          'min'=>0,
          'max'=>1
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
          'min'=>1,
          'max'=>1
        },
        ##
        # Language of the text
        # A code to define the language used in the text of the note.
        # Only required if the language is different from the resource language.
        'language' => {
          'valid_codes'=>{
            'urn:ietf:bcp:47'=>[ 'ar', 'bn', 'cs', 'da', 'de', 'de-AT', 'de-CH', 'de-DE', 'el', 'en', 'en-AU', 'en-CA', 'en-GB', 'en-IN', 'en-NZ', 'en-SG', 'en-US', 'es', 'es-AR', 'es-ES', 'es-UY', 'fi', 'fr', 'fr-BE', 'fr-CH', 'fr-FR', 'fy', 'fy-NL', 'hi', 'hr', 'it', 'it-CH', 'it-IT', 'ja', 'ko', 'nl', 'nl-BE', 'nl-NL', 'no', 'no-NO', 'pa', 'pl', 'pt', 'pt-BR', 'ru', 'ru-RU', 'sr', 'sr-RS', 'sv', 'sv-SE', 'te', 'zh', 'zh-CN', 'zh-HK', 'zh-SG', 'zh-TW' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'ProcessNote.language',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/languages'}
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
      # Note instance identifier
      # A number to uniquely identify a note entry.
      attr_accessor :number                         # 0-1 positiveInt
      ##
      # display | print | printoper
      # The business purpose of the note text.
      attr_accessor :type                           # 0-1 code
      ##
      # Note explanatory text
      # The explanation or description associated with the processing.
      attr_accessor :text                           # 1-1 string
      ##
      # Language of the text
      # A code to define the language used in the text of the note.
      # Only required if the language is different from the resource language.
      attr_accessor :language                       # 0-1 CodeableConcept
    end

    ##
    # Patient insurance information
    # Financial instruments for reimbursement for the health care products and services specified on the claim.
    # All insurance coverages for the patient which may be applicable for reimbursement, of the products and services listed in the claim, are typically provided in the claim to allow insurers to confirm the ordering of the insurance coverages relative to local 'coordination of benefit' rules. One coverage (and only one) with 'focal=true' is to be used in the adjudication of this claim. Coverages appearing before the focal Coverage in the list, and where 'subrogation=false', should provide a reference to the ClaimResponse containing the adjudication results of the prior claim.
    class Insurance < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Insurance.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Insurance.extension',
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
          'path'=>'Insurance.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Insurance instance identifier
        # A number to uniquely identify insurance entries and provide a sequence of coverages to convey coordination of benefit order.
        'sequence' => {
          'type'=>'positiveInt',
          'path'=>'Insurance.sequence',
          'min'=>1,
          'max'=>1
        },
        ##
        # Coverage to be used for adjudication
        # A flag to indicate that this Coverage is to be used for adjudication of this claim when set to true.
        # A patient may (will) have multiple insurance policies which provide reimbursement for healthcare services and products. For example a person may also be covered by their spouse's policy and both appear in the list (and may be from the same insurer). This flag will be set to true for only one of the listed policies and that policy will be used for adjudicating this claim. Other claims would be created to request adjudication against the other listed policies.
        'focal' => {
          'type'=>'boolean',
          'path'=>'Insurance.focal',
          'min'=>1,
          'max'=>1
        },
        ##
        # Insurance information
        # Reference to the insurance card level information contained in the Coverage resource. The coverage issuing insurer will use these details to locate the patient's actual coverage within the insurer's information system.
        'coverage' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Coverage'],
          'type'=>'Reference',
          'path'=>'Insurance.coverage',
          'min'=>1,
          'max'=>1
        },
        ##
        # Additional provider contract number
        # A business agreement number established between the provider and the insurer for special business processing purposes.
        'businessArrangement' => {
          'type'=>'string',
          'path'=>'Insurance.businessArrangement',
          'min'=>0,
          'max'=>1
        },
        ##
        # Adjudication results
        # The result of the adjudication of the line items for the Coverage specified in this insurance.
        # Must not be specified when 'focal=true' for this insurance.
        'claimResponse' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ClaimResponse'],
          'type'=>'Reference',
          'path'=>'Insurance.claimResponse',
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
      # Insurance instance identifier
      # A number to uniquely identify insurance entries and provide a sequence of coverages to convey coordination of benefit order.
      attr_accessor :sequence                       # 1-1 positiveInt
      ##
      # Coverage to be used for adjudication
      # A flag to indicate that this Coverage is to be used for adjudication of this claim when set to true.
      # A patient may (will) have multiple insurance policies which provide reimbursement for healthcare services and products. For example a person may also be covered by their spouse's policy and both appear in the list (and may be from the same insurer). This flag will be set to true for only one of the listed policies and that policy will be used for adjudicating this claim. Other claims would be created to request adjudication against the other listed policies.
      attr_accessor :focal                          # 1-1 boolean
      ##
      # Insurance information
      # Reference to the insurance card level information contained in the Coverage resource. The coverage issuing insurer will use these details to locate the patient's actual coverage within the insurer's information system.
      attr_accessor :coverage                       # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Coverage)
      ##
      # Additional provider contract number
      # A business agreement number established between the provider and the insurer for special business processing purposes.
      attr_accessor :businessArrangement            # 0-1 string
      ##
      # Adjudication results
      # The result of the adjudication of the line items for the Coverage specified in this insurance.
      # Must not be specified when 'focal=true' for this insurance.
      attr_accessor :claimResponse                  # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/ClaimResponse)
    end

    ##
    # Processing errors
    # Errors encountered during the processing of the adjudication.
    # If the request contains errors then an error element should be provided and no adjudication related sections (item, addItem, or payment) should be present.
    class Error < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Error.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Error.extension',
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
          'path'=>'Error.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Item sequence number
        # The sequence number of the line item submitted which contains the error. This value is omitted when the error occurs outside of the item structure.
        'itemSequence' => {
          'type'=>'positiveInt',
          'path'=>'Error.itemSequence',
          'min'=>0,
          'max'=>1
        },
        ##
        # Detail sequence number
        # The sequence number of the detail within the line item submitted which contains the error. This value is omitted when the error occurs outside of the item structure.
        'detailSequence' => {
          'type'=>'positiveInt',
          'path'=>'Error.detailSequence',
          'min'=>0,
          'max'=>1
        },
        ##
        # Subdetail sequence number
        # The sequence number of the sub-detail within the detail within the line item submitted which contains the error. This value is omitted when the error occurs outside of the item structure.
        'subDetailSequence' => {
          'type'=>'positiveInt',
          'path'=>'Error.subDetailSequence',
          'min'=>0,
          'max'=>1
        },
        ##
        # Error code detailing processing issues
        # An error code, from a specified code system, which details why the claim could not be adjudicated.
        'code' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/adjudication-error'=>[ 'a001', 'a002' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Error.code',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/adjudication-error'}
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
      # Item sequence number
      # The sequence number of the line item submitted which contains the error. This value is omitted when the error occurs outside of the item structure.
      attr_accessor :itemSequence                   # 0-1 positiveInt
      ##
      # Detail sequence number
      # The sequence number of the detail within the line item submitted which contains the error. This value is omitted when the error occurs outside of the item structure.
      attr_accessor :detailSequence                 # 0-1 positiveInt
      ##
      # Subdetail sequence number
      # The sequence number of the sub-detail within the detail within the line item submitted which contains the error. This value is omitted when the error occurs outside of the item structure.
      attr_accessor :subDetailSequence              # 0-1 positiveInt
      ##
      # Error code detailing processing issues
      # An error code, from a specified code system, which details why the claim could not be adjudicated.
      attr_accessor :code                           # 1-1 CodeableConcept
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
    # Business Identifier for a claim response
    # A unique identifier assigned to this claim response.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # active | cancelled | draft | entered-in-error
    # The status of the resource instance.
    # This element is labeled as a modifier because the status contains codes that mark the resource as not currently valid.
    attr_accessor :status                         # 1-1 code
    ##
    # More granular claim type
    # A finer grained suite of claim type codes which may convey additional information such as Inpatient vs Outpatient and/or a specialty service.
    # This may contain the local bill type codes, for example the US UB-04 bill type code or the CMS bill type.
    attr_accessor :type                           # 1-1 CodeableConcept
    ##
    # More granular claim type
    # A finer grained suite of claim type codes which may convey additional information such as Inpatient vs Outpatient and/or a specialty service.
    # This may contain the local bill type codes, for example the US UB-04 bill type code or the CMS bill type.
    attr_accessor :subType                        # 0-1 CodeableConcept
    ##
    # claim | preauthorization | predetermination
    # A code to indicate whether the nature of the request is: to request adjudication of products and services previously rendered; or requesting authorization and adjudication for provision in the future; or requesting the non-binding adjudication of the listed products and services which could be provided in the future.
    attr_accessor :use                            # 1-1 code
    ##
    # The recipient of the products and services
    # The party to whom the professional services and/or products have been supplied or are being considered and for whom actual for facast reimbursement is sought.
    attr_accessor :patient                        # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient)
    ##
    # Response creation date
    # The date this resource was created.
    attr_accessor :created                        # 1-1 dateTime
    ##
    # Party responsible for reimbursement
    # The party responsible for authorization, adjudication and reimbursement.
    attr_accessor :insurer                        # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Party responsible for the claim
    # The provider which is responsible for the claim, predetermination or preauthorization.
    # Typically this field would be 1..1 where this party is responsible for the claim but not necessarily professionally responsible for the provision of the individual products and services listed below.
    attr_accessor :requestor                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Id of resource triggering adjudication
    # Original request resource reference.
    attr_accessor :request                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Claim)
    ##
    # queued | complete | error | partial
    # The outcome of the claim, predetermination, or preauthorization processing.
    # The resource may be used to indicate that: the request has been held (queued) for processing; that it has been processed and errors found (error); that no errors were found and that some of the adjudication has been undertaken (partial) or that all of the adjudication has been undertaken (complete).
    attr_accessor :outcome                        # 1-1 code
    ##
    # Disposition Message
    # A human readable description of the status of the adjudication.
    attr_accessor :disposition                    # 0-1 string
    ##
    # Preauthorization reference
    # Reference from the Insurer which is used in later communications which refers to this adjudication.
    # This value is only present on preauthorization adjudications.
    attr_accessor :preAuthRef                     # 0-1 string
    ##
    # Preauthorization reference effective period
    # The time frame during which this authorization is effective.
    attr_accessor :preAuthPeriod                  # 0-1 Period
    ##
    # Party to be paid any benefits payable
    # Type of Party to be reimbursed: subscriber, provider, other.
    attr_accessor :payeeType                      # 0-1 CodeableConcept
    ##
    # Adjudication for claim line items
    # A claim line. Either a simple (a product or service) or a 'group' of details which can also be a simple items or groups of sub-details.
    attr_accessor :item                           # 0-* [ ClaimResponse::Item ]
    ##
    # Insurer added line items
    # The first-tier service adjudications for payor added product or service lines.
    attr_accessor :addItem                        # 0-* [ ClaimResponse::AddItem ]
    ##
    # Header-level adjudication
    # The adjudication results which are presented at the header level rather than at the line-item or add-item levels.
    attr_accessor :adjudication                   # 0-* [ ClaimResponse::Item::Adjudication ]
    ##
    # Adjudication totals
    # Categorized monetary totals for the adjudication.
    # Totals for amounts submitted, co-pays, benefits payable etc.
    attr_accessor :total                          # 0-* [ ClaimResponse::Total ]
    ##
    # Payment Details
    # Payment details for the adjudication of the claim.
    attr_accessor :payment                        # 0-1 ClaimResponse::Payment
    ##
    # Funds reserved status
    # A code, used only on a response to a preauthorization, to indicate whether the benefits payable have been reserved and for whom.
    # Fund would be release by a future claim quoting the preAuthRef of this response. Examples of values include: provider, patient, none.
    attr_accessor :fundsReserve                   # 0-1 CodeableConcept
    ##
    # Printed form identifier
    # A code for the form to be used for printing the content.
    # May be needed to identify specific jurisdictional forms.
    attr_accessor :formCode                       # 0-1 CodeableConcept
    ##
    # Printed reference or actual form
    # The actual form, by reference or inclusion, for printing the content or an EOB.
    # Needed to permit insurers to include the actual form.
    attr_accessor :form                           # 0-1 Attachment
    ##
    # Note concerning adjudication
    # A note that describes or explains adjudication results in a human readable form.
    attr_accessor :processNote                    # 0-* [ ClaimResponse::ProcessNote ]
    ##
    # Request for additional information
    # Request for additional supporting or authorizing information.
    # For example: professional reports, documents, images, clinical resources, or accident reports.
    attr_accessor :communicationRequest           # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/CommunicationRequest) ]
    ##
    # Patient insurance information
    # Financial instruments for reimbursement for the health care products and services specified on the claim.
    # All insurance coverages for the patient which may be applicable for reimbursement, of the products and services listed in the claim, are typically provided in the claim to allow insurers to confirm the ordering of the insurance coverages relative to local 'coordination of benefit' rules. One coverage (and only one) with 'focal=true' is to be used in the adjudication of this claim. Coverages appearing before the focal Coverage in the list, and where 'subrogation=false', should provide a reference to the ClaimResponse containing the adjudication results of the prior claim.
    attr_accessor :insurance                      # 0-* [ ClaimResponse::Insurance ]
    ##
    # Processing errors
    # Errors encountered during the processing of the adjudication.
    # If the request contains errors then an error element should be provided and no adjudication related sections (item, addItem, or payment) should be present.
    attr_accessor :error                          # 0-* [ ClaimResponse::Error ]

    def resourceType
      'ClaimResponse'
    end
  end
end
