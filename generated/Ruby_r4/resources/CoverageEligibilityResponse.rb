module FHIR

  ##
  # This resource provides eligibility and plan details from the processing of an CoverageEligibilityRequest resource.
  class CoverageEligibilityResponse < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['created', 'disposition', 'identifier', 'insurer', 'outcome', 'patient', 'request', 'requestor', 'status']
    MULTIPLE_TYPES = {
      'serviced[x]' => ['date', 'Period']
    }
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'CoverageEligibilityResponse.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'CoverageEligibilityResponse.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'CoverageEligibilityResponse.implicitRules',
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
        'path'=>'CoverageEligibilityResponse.language',
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
        'path'=>'CoverageEligibilityResponse.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'CoverageEligibilityResponse.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'CoverageEligibilityResponse.extension',
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
        'path'=>'CoverageEligibilityResponse.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Business Identifier for coverage eligiblity request
      # A unique identifier assigned to this coverage eligiblity request.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'CoverageEligibilityResponse.identifier',
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
        'path'=>'CoverageEligibilityResponse.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/fm-status'}
      },
      ##
      # auth-requirements | benefits | discovery | validation
      # Code to specify whether requesting: prior authorization requirements for some service categories or billing codes; benefits for coverages specified or discovered; discovery and return of coverages for the patient; and/or validation that the specified coverage is in-force at the date/period specified or 'now' if not specified.
      'purpose' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/eligibilityresponse-purpose'=>[ 'auth-requirements', 'benefits', 'discovery', 'validation' ]
        },
        'type'=>'code',
        'path'=>'CoverageEligibilityResponse.purpose',
        'min'=>1,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/eligibilityresponse-purpose'}
      },
      ##
      # Intended recipient of products and services
      # The party who is the beneficiary of the supplied coverage and for whom eligibility is sought.
      'patient' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient'],
        'type'=>'Reference',
        'path'=>'CoverageEligibilityResponse.patient',
        'min'=>1,
        'max'=>1
      },
      ##
      # Estimated date or dates of service
      # The date or dates when the enclosed suite of services were performed or completed.
      'servicedDate' => {
        'type'=>'Date',
        'path'=>'CoverageEligibilityResponse.serviced[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Estimated date or dates of service
      # The date or dates when the enclosed suite of services were performed or completed.
      'servicedPeriod' => {
        'type'=>'Period',
        'path'=>'CoverageEligibilityResponse.serviced[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Response creation date
      # The date this resource was created.
      'created' => {
        'type'=>'dateTime',
        'path'=>'CoverageEligibilityResponse.created',
        'min'=>1,
        'max'=>1
      },
      ##
      # Party responsible for the request
      # The provider which is responsible for the request.
      # Typically this field would be 1..1 where this party is responsible for the claim but not necessarily professionally responsible for the provision of the individual products and services listed below.
      'requestor' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'CoverageEligibilityResponse.requestor',
        'min'=>0,
        'max'=>1
      },
      ##
      # Eligibility request reference
      # Reference to the original request resource.
      'request' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/CoverageEligibilityRequest'],
        'type'=>'Reference',
        'path'=>'CoverageEligibilityResponse.request',
        'min'=>1,
        'max'=>1
      },
      ##
      # queued | complete | error | partial
      # The outcome of the request processing.
      # The resource may be used to indicate that: the request has been held (queued) for processing; that it has been processed and errors found (error); that no errors were found and that some of the adjudication has been undertaken (partial) or that all of the adjudication has been undertaken (complete).
      'outcome' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/remittance-outcome'=>[ 'queued', 'complete', 'error', 'partial' ]
        },
        'type'=>'code',
        'path'=>'CoverageEligibilityResponse.outcome',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/remittance-outcome'}
      },
      ##
      # Disposition Message
      # A human readable description of the status of the adjudication.
      'disposition' => {
        'type'=>'string',
        'path'=>'CoverageEligibilityResponse.disposition',
        'min'=>0,
        'max'=>1
      },
      ##
      # Coverage issuer
      # The Insurer who issued the coverage in question and is the author of the response.
      'insurer' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'CoverageEligibilityResponse.insurer',
        'min'=>1,
        'max'=>1
      },
      ##
      # Patient insurance information
      # Financial instruments for reimbursement for the health care products and services.
      # All insurance coverages for the patient which may be applicable for reimbursement, of the products and services listed in the claim, are typically provided in the claim to allow insurers to confirm the ordering of the insurance coverages relative to local 'coordination of benefit' rules. One coverage (and only one) with 'focal=true' is to be used in the adjudication of this claim. Coverages appearing before the focal Coverage in the list, and where 'subrogation=false', should provide a reference to the ClaimResponse containing the adjudication results of the prior claim.
      'insurance' => {
        'type'=>'CoverageEligibilityResponse::Insurance',
        'path'=>'CoverageEligibilityResponse.insurance',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Preauthorization reference
      # A reference from the Insurer to which these services pertain to be used on further communication and as proof that the request occurred.
      'preAuthRef' => {
        'type'=>'string',
        'path'=>'CoverageEligibilityResponse.preAuthRef',
        'min'=>0,
        'max'=>1
      },
      ##
      # Printed form identifier
      # A code for the form to be used for printing the content.
      # May be needed to identify specific jurisdictional forms.
      'form' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/forms-codes'=>[ '1', '2' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'CoverageEligibilityResponse.form',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/forms'}
      },
      ##
      # Processing errors
      # Errors encountered during the processing of the request.
      'error' => {
        'type'=>'CoverageEligibilityResponse::Error',
        'path'=>'CoverageEligibilityResponse.error',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Patient insurance information
    # Financial instruments for reimbursement for the health care products and services.
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
          'type'=>'string',
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
        # Coverage inforce indicator
        # Flag indicating if the coverage provided is inforce currently if no service date(s) specified or for the whole duration of the service dates.
        'inforce' => {
          'type'=>'boolean',
          'path'=>'Insurance.inforce',
          'min'=>0,
          'max'=>1
        },
        ##
        # When the benefits are applicable
        # The term of the benefits documented in this response.
        'benefitPeriod' => {
          'type'=>'Period',
          'path'=>'Insurance.benefitPeriod',
          'min'=>0,
          'max'=>1
        },
        ##
        # Benefits and authorization details
        # Benefits and optionally current balances, and authorization details by category or service.
        'item' => {
          'type'=>'CoverageEligibilityResponse::Insurance::Item',
          'path'=>'Insurance.item',
          'min'=>0,
          'max'=>Float::INFINITY
        }
      }

      ##
      # Benefits and authorization details
      # Benefits and optionally current balances, and authorization details by category or service.
      class Item < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
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
          # Benefit classification
          # Code to identify the general type of benefits under which products and services are provided.
          # Examples include Medical Care, Periodontics, Renal Dialysis, Vision Coverage.
          'category' => {
            'valid_codes'=>{
              'http://terminology.hl7.org/CodeSystem/ex-benefitcategory'=>[ '1', '2', '3', '4', '5', '14', '23', '24', '25', '26', '27', '28', '30', '35', '36', '37', '49', '55', '56', '61', '62', '63', '69', '76', 'F1', 'F3', 'F4', 'F6' ]
            },
            'type'=>'CodeableConcept',
            'path'=>'Item.category',
            'min'=>0,
            'max'=>1,
            'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/ex-benefitcategory'}
          },
          ##
          # Billing, service, product, or drug code
          # This contains the product, service, drug or other billing code for the item.
          # Code to indicate the Professional Service or Product supplied (e.g. CTP, HCPCS, USCLS, ICD10, NCPDP, DIN, RxNorm, ACHI, CCI).
          'productOrService' => {
            'valid_codes'=>{
              'http://terminology.hl7.org/CodeSystem/ex-USCLS'=>[ '1101', '1102', '1103', '1201', '1205', '2101', '2102', '2141', '2601', '11101', '11102', '11103', '11104', '21211', '21212', '27211', '67211', '99111', '99333', '99555' ]
            },
            'type'=>'CodeableConcept',
            'path'=>'Item.productOrService',
            'min'=>0,
            'max'=>1,
            'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/service-uscls'}
          },
          ##
          # Product or service billing modifiers
          # Item typification or modifiers codes to convey additional context for the product or service.
          # For example in Oral whether the treatment is cosmetic or associated with TMJ, or for Medical whether the treatment was outside the clinic or out of office hours.
          'modifier' => {
            'valid_codes'=>{
              'http://terminology.hl7.org/CodeSystem/modifiers'=>[ 'a', 'b', 'c', 'e', 'rooh', 'x' ]
            },
            'type'=>'CodeableConcept',
            'path'=>'Item.modifier',
            'min'=>0,
            'max'=>Float::INFINITY,
            'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/claim-modifiers'}
          },
          ##
          # Performing practitioner
          # The practitioner who is eligible for the provision of the product or service.
          'provider' => {
            'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole'],
            'type'=>'Reference',
            'path'=>'Item.provider',
            'min'=>0,
            'max'=>1
          },
          ##
          # Excluded from the plan
          # True if the indicated class of service is excluded from the plan, missing or False indicates the product or service is included in the coverage.
          'excluded' => {
            'type'=>'boolean',
            'path'=>'Item.excluded',
            'min'=>0,
            'max'=>1
          },
          ##
          # Short name for the benefit
          # A short name or tag for the benefit.
          # For example: MED01, or DENT2.
          'name' => {
            'type'=>'string',
            'path'=>'Item.name',
            'min'=>0,
            'max'=>1
          },
          ##
          # Description of the benefit or services covered
          # A richer description of the benefit or services covered.
          # For example 'DENT2 covers 100% of basic, 50% of major but excludes Ortho, Implants and Cosmetic services'.
          'description' => {
            'type'=>'string',
            'path'=>'Item.description',
            'min'=>0,
            'max'=>1
          },
          ##
          # In or out of network
          # Is a flag to indicate whether the benefits refer to in-network providers or out-of-network providers.
          'network' => {
            'valid_codes'=>{
              'http://terminology.hl7.org/CodeSystem/benefit-network'=>[ 'in', 'out' ]
            },
            'type'=>'CodeableConcept',
            'path'=>'Item.network',
            'min'=>0,
            'max'=>1,
            'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/benefit-network'}
          },
          ##
          # Individual or family
          # Indicates if the benefits apply to an individual or to the family.
          'unit' => {
            'valid_codes'=>{
              'http://terminology.hl7.org/CodeSystem/benefit-unit'=>[ 'individual', 'family' ]
            },
            'type'=>'CodeableConcept',
            'path'=>'Item.unit',
            'min'=>0,
            'max'=>1,
            'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/benefit-unit'}
          },
          ##
          # Annual or lifetime
          # The term or period of the values such as 'maximum lifetime benefit' or 'maximum annual visits'.
          'term' => {
            'valid_codes'=>{
              'http://terminology.hl7.org/CodeSystem/benefit-term'=>[ 'annual', 'day', 'lifetime' ]
            },
            'type'=>'CodeableConcept',
            'path'=>'Item.term',
            'min'=>0,
            'max'=>1,
            'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/benefit-term'}
          },
          ##
          # Benefit Summary
          # Benefits used to date.
          'benefit' => {
            'type'=>'CoverageEligibilityResponse::Insurance::Item::Benefit',
            'path'=>'Item.benefit',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Authorization required flag
          # A boolean flag indicating whether a preauthorization is required prior to actual service delivery.
          'authorizationRequired' => {
            'type'=>'boolean',
            'path'=>'Item.authorizationRequired',
            'min'=>0,
            'max'=>1
          },
          ##
          # Type of required supporting materials
          # Codes or comments regarding information or actions associated with the preauthorization.
          'authorizationSupporting' => {
            'valid_codes'=>{
              'http://terminology.hl7.org/CodeSystem/coverageeligibilityresponse-ex-auth-support'=>[ 'laborder', 'labreport', 'diagnosticimageorder', 'diagnosticimagereport', 'professionalreport', 'accidentreport', 'model', 'picture' ]
            },
            'type'=>'CodeableConcept',
            'path'=>'Item.authorizationSupporting',
            'min'=>0,
            'max'=>Float::INFINITY,
            'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/coverageeligibilityresponse-ex-auth-support'}
          },
          ##
          # Preauthorization requirements endpoint
          # A web location for obtaining requirements or descriptive information regarding the preauthorization.
          'authorizationUrl' => {
            'type'=>'uri',
            'path'=>'Item.authorizationUrl',
            'min'=>0,
            'max'=>1
          }
        }

        ##
        # Benefit Summary
        # Benefits used to date.
        class Benefit < FHIR::Model
          include FHIR::Hashable
          include FHIR::Json
          include FHIR::Xml

          MULTIPLE_TYPES = {
            'allowed[x]' => ['Money', 'string', 'unsignedInt'],
            'used[x]' => ['Money', 'string', 'unsignedInt']
          }
          METADATA = {
            ##
            # Unique id for inter-element referencing
            # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
            'id' => {
              'type'=>'string',
              'path'=>'Benefit.id',
              'min'=>0,
              'max'=>1
            },
            ##
            # Additional content defined by implementations
            # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
            # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
            'extension' => {
              'type'=>'Extension',
              'path'=>'Benefit.extension',
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
              'path'=>'Benefit.modifierExtension',
              'min'=>0,
              'max'=>Float::INFINITY
            },
            ##
            # Benefit classification
            # Classification of benefit being provided.
            # For example: deductible, visits, benefit amount.
            'type' => {
              'valid_codes'=>{
                'http://terminology.hl7.org/CodeSystem/benefit-type'=>[ 'benefit', 'deductible', 'visit', 'room', 'copay', 'copay-percent', 'copay-maximum', 'vision-exam', 'vision-glasses', 'vision-contacts', 'medical-primarycare', 'pharmacy-dispense' ]
              },
              'type'=>'CodeableConcept',
              'path'=>'Benefit.type',
              'min'=>1,
              'max'=>1,
              'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/benefit-type'}
            },
            ##
            # Benefits allowed
            # The quantity of the benefit which is permitted under the coverage.
            'allowedMoney' => {
              'type'=>'Money',
              'path'=>'Benefit.allowed[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Benefits allowed
            # The quantity of the benefit which is permitted under the coverage.
            'allowedString' => {
              'type'=>'String',
              'path'=>'Benefit.allowed[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Benefits allowed
            # The quantity of the benefit which is permitted under the coverage.
            'allowedUnsignedInt' => {
              'type'=>'UnsignedInt',
              'path'=>'Benefit.allowed[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Benefits used
            # The quantity of the benefit which have been consumed to date.
            'usedMoney' => {
              'type'=>'Money',
              'path'=>'Benefit.used[x]',
              'min'=>0,
              'max'=>1
            }
            ##
            # Benefits used
            # The quantity of the benefit which have been consumed to date.
            'usedString' => {
              'type'=>'String',
              'path'=>'Benefit.used[x]',
              'min'=>0,
              'max'=>1
            }
            ##
            # Benefits used
            # The quantity of the benefit which have been consumed to date.
            'usedUnsignedInt' => {
              'type'=>'UnsignedInt',
              'path'=>'Benefit.used[x]',
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
          # Benefit classification
          # Classification of benefit being provided.
          # For example: deductible, visits, benefit amount.
          attr_accessor :type                           # 1-1 CodeableConcept
          ##
          # Benefits allowed
          # The quantity of the benefit which is permitted under the coverage.
          attr_accessor :allowedMoney                   # 0-1 Money
          ##
          # Benefits allowed
          # The quantity of the benefit which is permitted under the coverage.
          attr_accessor :allowedString                  # 0-1 String
          ##
          # Benefits allowed
          # The quantity of the benefit which is permitted under the coverage.
          attr_accessor :allowedUnsignedInt             # 0-1 UnsignedInt
          ##
          # Benefits used
          # The quantity of the benefit which have been consumed to date.
          attr_accessor :usedMoney                      # 0-1 Money
          ##
          # Benefits used
          # The quantity of the benefit which have been consumed to date.
          attr_accessor :usedString                     # 0-1 String
          ##
          # Benefits used
          # The quantity of the benefit which have been consumed to date.
          attr_accessor :usedUnsignedInt                # 0-1 UnsignedInt
        end
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
        # Benefit classification
        # Code to identify the general type of benefits under which products and services are provided.
        # Examples include Medical Care, Periodontics, Renal Dialysis, Vision Coverage.
        attr_accessor :category                       # 0-1 CodeableConcept
        ##
        # Billing, service, product, or drug code
        # This contains the product, service, drug or other billing code for the item.
        # Code to indicate the Professional Service or Product supplied (e.g. CTP, HCPCS, USCLS, ICD10, NCPDP, DIN, RxNorm, ACHI, CCI).
        attr_accessor :productOrService               # 0-1 CodeableConcept
        ##
        # Product or service billing modifiers
        # Item typification or modifiers codes to convey additional context for the product or service.
        # For example in Oral whether the treatment is cosmetic or associated with TMJ, or for Medical whether the treatment was outside the clinic or out of office hours.
        attr_accessor :modifier                       # 0-* [ CodeableConcept ]
        ##
        # Performing practitioner
        # The practitioner who is eligible for the provision of the product or service.
        attr_accessor :provider                       # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole)
        ##
        # Excluded from the plan
        # True if the indicated class of service is excluded from the plan, missing or False indicates the product or service is included in the coverage.
        attr_accessor :excluded                       # 0-1 boolean
        ##
        # Short name for the benefit
        # A short name or tag for the benefit.
        # For example: MED01, or DENT2.
        attr_accessor :name                           # 0-1 string
        ##
        # Description of the benefit or services covered
        # A richer description of the benefit or services covered.
        # For example 'DENT2 covers 100% of basic, 50% of major but excludes Ortho, Implants and Cosmetic services'.
        attr_accessor :description                    # 0-1 string
        ##
        # In or out of network
        # Is a flag to indicate whether the benefits refer to in-network providers or out-of-network providers.
        attr_accessor :network                        # 0-1 CodeableConcept
        ##
        # Individual or family
        # Indicates if the benefits apply to an individual or to the family.
        attr_accessor :unit                           # 0-1 CodeableConcept
        ##
        # Annual or lifetime
        # The term or period of the values such as 'maximum lifetime benefit' or 'maximum annual visits'.
        attr_accessor :term                           # 0-1 CodeableConcept
        ##
        # Benefit Summary
        # Benefits used to date.
        attr_accessor :benefit                        # 0-* [ CoverageEligibilityResponse::Insurance::Item::Benefit ]
        ##
        # Authorization required flag
        # A boolean flag indicating whether a preauthorization is required prior to actual service delivery.
        attr_accessor :authorizationRequired          # 0-1 boolean
        ##
        # Type of required supporting materials
        # Codes or comments regarding information or actions associated with the preauthorization.
        attr_accessor :authorizationSupporting        # 0-* [ CodeableConcept ]
        ##
        # Preauthorization requirements endpoint
        # A web location for obtaining requirements or descriptive information regarding the preauthorization.
        attr_accessor :authorizationUrl               # 0-1 uri
      end
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
      # Insurance information
      # Reference to the insurance card level information contained in the Coverage resource. The coverage issuing insurer will use these details to locate the patient's actual coverage within the insurer's information system.
      attr_accessor :coverage                       # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Coverage)
      ##
      # Coverage inforce indicator
      # Flag indicating if the coverage provided is inforce currently if no service date(s) specified or for the whole duration of the service dates.
      attr_accessor :inforce                        # 0-1 boolean
      ##
      # When the benefits are applicable
      # The term of the benefits documented in this response.
      attr_accessor :benefitPeriod                  # 0-1 Period
      ##
      # Benefits and authorization details
      # Benefits and optionally current balances, and authorization details by category or service.
      attr_accessor :item                           # 0-* [ CoverageEligibilityResponse::Insurance::Item ]
    end

    ##
    # Processing errors
    # Errors encountered during the processing of the request.
    class Error < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
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
        # Error code detailing processing issues
        # An error code,from a specified code system, which details why the eligibility check could not be performed.
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
      # Error code detailing processing issues
      # An error code,from a specified code system, which details why the eligibility check could not be performed.
      attr_accessor :code                           # 1-1 CodeableConcept
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
    # Business Identifier for coverage eligiblity request
    # A unique identifier assigned to this coverage eligiblity request.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # active | cancelled | draft | entered-in-error
    # The status of the resource instance.
    # This element is labeled as a modifier because the status contains codes that mark the resource as not currently valid.
    attr_accessor :status                         # 1-1 code
    ##
    # auth-requirements | benefits | discovery | validation
    # Code to specify whether requesting: prior authorization requirements for some service categories or billing codes; benefits for coverages specified or discovered; discovery and return of coverages for the patient; and/or validation that the specified coverage is in-force at the date/period specified or 'now' if not specified.
    attr_accessor :purpose                        # 1-* [ code ]
    ##
    # Intended recipient of products and services
    # The party who is the beneficiary of the supplied coverage and for whom eligibility is sought.
    attr_accessor :patient                        # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient)
    ##
    # Estimated date or dates of service
    # The date or dates when the enclosed suite of services were performed or completed.
    attr_accessor :servicedDate                   # 0-1 Date
    ##
    # Estimated date or dates of service
    # The date or dates when the enclosed suite of services were performed or completed.
    attr_accessor :servicedPeriod                 # 0-1 Period
    ##
    # Response creation date
    # The date this resource was created.
    attr_accessor :created                        # 1-1 dateTime
    ##
    # Party responsible for the request
    # The provider which is responsible for the request.
    # Typically this field would be 1..1 where this party is responsible for the claim but not necessarily professionally responsible for the provision of the individual products and services listed below.
    attr_accessor :requestor                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Eligibility request reference
    # Reference to the original request resource.
    attr_accessor :request                        # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/CoverageEligibilityRequest)
    ##
    # queued | complete | error | partial
    # The outcome of the request processing.
    # The resource may be used to indicate that: the request has been held (queued) for processing; that it has been processed and errors found (error); that no errors were found and that some of the adjudication has been undertaken (partial) or that all of the adjudication has been undertaken (complete).
    attr_accessor :outcome                        # 1-1 code
    ##
    # Disposition Message
    # A human readable description of the status of the adjudication.
    attr_accessor :disposition                    # 0-1 string
    ##
    # Coverage issuer
    # The Insurer who issued the coverage in question and is the author of the response.
    attr_accessor :insurer                        # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Patient insurance information
    # Financial instruments for reimbursement for the health care products and services.
    # All insurance coverages for the patient which may be applicable for reimbursement, of the products and services listed in the claim, are typically provided in the claim to allow insurers to confirm the ordering of the insurance coverages relative to local 'coordination of benefit' rules. One coverage (and only one) with 'focal=true' is to be used in the adjudication of this claim. Coverages appearing before the focal Coverage in the list, and where 'subrogation=false', should provide a reference to the ClaimResponse containing the adjudication results of the prior claim.
    attr_accessor :insurance                      # 0-* [ CoverageEligibilityResponse::Insurance ]
    ##
    # Preauthorization reference
    # A reference from the Insurer to which these services pertain to be used on further communication and as proof that the request occurred.
    attr_accessor :preAuthRef                     # 0-1 string
    ##
    # Printed form identifier
    # A code for the form to be used for printing the content.
    # May be needed to identify specific jurisdictional forms.
    attr_accessor :form                           # 0-1 CodeableConcept
    ##
    # Processing errors
    # Errors encountered during the processing of the request.
    attr_accessor :error                          # 0-* [ CoverageEligibilityResponse::Error ]

    def resourceType
      'CoverageEligibilityResponse'
    end
  end
end
