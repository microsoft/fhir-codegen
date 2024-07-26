module FHIR

  ##
  # The CoverageEligibilityRequest provides patient and insurance coverage information to an insurer for them to respond, in the form of an CoverageEligibilityResponse, with information regarding whether the stated coverage is valid and in-force and optionally to provide the insurance details of the policy.
  class CoverageEligibilityRequest < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['created', 'enterer', 'facility', 'identifier', 'patient', 'provider', 'status']
    MULTIPLE_TYPES = {
      'serviced[x]' => ['date', 'Period']
    }
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'CoverageEligibilityRequest.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'CoverageEligibilityRequest.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'CoverageEligibilityRequest.implicitRules',
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
        'path'=>'CoverageEligibilityRequest.language',
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
        'path'=>'CoverageEligibilityRequest.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'CoverageEligibilityRequest.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'CoverageEligibilityRequest.extension',
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
        'path'=>'CoverageEligibilityRequest.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Business Identifier for coverage eligiblity request
      # A unique identifier assigned to this coverage eligiblity request.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'CoverageEligibilityRequest.identifier',
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
        'path'=>'CoverageEligibilityRequest.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/fm-status'}
      },
      ##
      # Desired processing priority
      # When the requestor expects the processor to complete processing.
      'priority' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/processpriority'=>[ 'stat', 'normal', 'deferred' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'CoverageEligibilityRequest.priority',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/process-priority'}
      },
      ##
      # auth-requirements | benefits | discovery | validation
      # Code to specify whether requesting: prior authorization requirements for some service categories or billing codes; benefits for coverages specified or discovered; discovery and return of coverages for the patient; and/or validation that the specified coverage is in-force at the date/period specified or 'now' if not specified.
      'purpose' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/eligibilityrequest-purpose'=>[ 'auth-requirements', 'benefits', 'discovery', 'validation' ]
        },
        'type'=>'code',
        'path'=>'CoverageEligibilityRequest.purpose',
        'min'=>1,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/eligibilityrequest-purpose'}
      },
      ##
      # Intended recipient of products and services
      # The party who is the beneficiary of the supplied coverage and for whom eligibility is sought.
      # 1..1.
      'patient' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient'],
        'type'=>'Reference',
        'path'=>'CoverageEligibilityRequest.patient',
        'min'=>1,
        'max'=>1
      },
      ##
      # Estimated date or dates of service
      # The date or dates when the enclosed suite of services were performed or completed.
      'servicedDate' => {
        'type'=>'Date',
        'path'=>'CoverageEligibilityRequest.serviced[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Estimated date or dates of service
      # The date or dates when the enclosed suite of services were performed or completed.
      'servicedPeriod' => {
        'type'=>'Period',
        'path'=>'CoverageEligibilityRequest.serviced[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Creation date
      # The date when this resource was created.
      'created' => {
        'type'=>'dateTime',
        'path'=>'CoverageEligibilityRequest.created',
        'min'=>1,
        'max'=>1
      },
      ##
      # Author
      # Person who created the request.
      'enterer' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole'],
        'type'=>'Reference',
        'path'=>'CoverageEligibilityRequest.enterer',
        'min'=>0,
        'max'=>1
      },
      ##
      # Party responsible for the request
      # The provider which is responsible for the request.
      # Typically this field would be 1..1 where this party is responsible for the eligibility request but not necessarily professionally responsible for the provision of the individual products and services listed below.
      'provider' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'CoverageEligibilityRequest.provider',
        'min'=>0,
        'max'=>1
      },
      ##
      # Coverage issuer
      # The Insurer who issued the coverage in question and is the recipient of the request.
      'insurer' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'CoverageEligibilityRequest.insurer',
        'min'=>1,
        'max'=>1
      },
      ##
      # Servicing facility
      # Facility where the services are intended to be provided.
      'facility' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Location'],
        'type'=>'Reference',
        'path'=>'CoverageEligibilityRequest.facility',
        'min'=>0,
        'max'=>1
      },
      ##
      # Supporting information
      # Additional information codes regarding exceptions, special considerations, the condition, situation, prior or concurrent issues.
      # Often there are multiple jurisdiction specific valuesets which are required.
      'supportingInfo' => {
        'type'=>'CoverageEligibilityRequest::SupportingInfo',
        'path'=>'CoverageEligibilityRequest.supportingInfo',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Patient insurance information
      # Financial instruments for reimbursement for the health care products and services.
      # All insurance coverages for the patient which may be applicable for reimbursement, of the products and services listed in the claim, are typically provided in the claim to allow insurers to confirm the ordering of the insurance coverages relative to local 'coordination of benefit' rules. One coverage (and only one) with 'focal=true' is to be used in the adjudication of this claim. Coverages appearing before the focal Coverage in the list, and where 'subrogation=false', should provide a reference to the ClaimResponse containing the adjudication results of the prior claim.
      'insurance' => {
        'type'=>'CoverageEligibilityRequest::Insurance',
        'path'=>'CoverageEligibilityRequest.insurance',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Item to be evaluated for eligibiity
      # Service categories or billable services for which benefit details and/or an authorization prior to service delivery may be required by the payor.
      'item' => {
        'type'=>'CoverageEligibilityRequest::Item',
        'path'=>'CoverageEligibilityRequest.item',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Supporting information
    # Additional information codes regarding exceptions, special considerations, the condition, situation, prior or concurrent issues.
    # Often there are multiple jurisdiction specific valuesets which are required.
    class SupportingInfo < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'SupportingInfo.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'SupportingInfo.extension',
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
          'path'=>'SupportingInfo.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Information instance identifier
        # A number to uniquely identify supporting information entries.
        'sequence' => {
          'type'=>'positiveInt',
          'path'=>'SupportingInfo.sequence',
          'min'=>1,
          'max'=>1
        },
        ##
        # Data to be provided
        # Additional data or information such as resources, documents, images etc. including references to the data or the actual inclusion of the data.
        # Could be used to provide references to other resources, document. For example could contain a PDF in an Attachment of the Police Report for an Accident.
        'information' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
          'type'=>'Reference',
          'path'=>'SupportingInfo.information',
          'min'=>1,
          'max'=>1
        },
        ##
        # Applies to all items
        # The supporting materials are applicable for all detail items, product/servce categories and specific billing codes.
        'appliesToAll' => {
          'type'=>'boolean',
          'path'=>'SupportingInfo.appliesToAll',
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
      # Information instance identifier
      # A number to uniquely identify supporting information entries.
      attr_accessor :sequence                       # 1-1 positiveInt
      ##
      # Data to be provided
      # Additional data or information such as resources, documents, images etc. including references to the data or the actual inclusion of the data.
      # Could be used to provide references to other resources, document. For example could contain a PDF in an Attachment of the Police Report for an Accident.
      attr_accessor :information                    # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Resource)
      ##
      # Applies to all items
      # The supporting materials are applicable for all detail items, product/servce categories and specific billing codes.
      attr_accessor :appliesToAll                   # 0-1 boolean
    end

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
        # Applicable coverage
        # A flag to indicate that this Coverage is to be used for evaluation of this request when set to true.
        # A patient may (will) have multiple insurance policies which provide reimburement for healthcare services and products. For example a person may also be covered by their spouse's policy and both appear in the list (and may be from the same insurer). This flag will be set to true for only one of the listed policies and that policy will be used for evaluating this request. Other requests would be created to request evaluation against the other listed policies.
        'focal' => {
          'type'=>'boolean',
          'path'=>'Insurance.focal',
          'min'=>0,
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
      # Applicable coverage
      # A flag to indicate that this Coverage is to be used for evaluation of this request when set to true.
      # A patient may (will) have multiple insurance policies which provide reimburement for healthcare services and products. For example a person may also be covered by their spouse's policy and both appear in the list (and may be from the same insurer). This flag will be set to true for only one of the listed policies and that policy will be used for evaluating this request. Other requests would be created to request evaluation against the other listed policies.
      attr_accessor :focal                          # 0-1 boolean
      ##
      # Insurance information
      # Reference to the insurance card level information contained in the Coverage resource. The coverage issuing insurer will use these details to locate the patient's actual coverage within the insurer's information system.
      attr_accessor :coverage                       # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Coverage)
      ##
      # Additional provider contract number
      # A business agreement number established between the provider and the insurer for special business processing purposes.
      attr_accessor :businessArrangement            # 0-1 string
    end

    ##
    # Item to be evaluated for eligibiity
    # Service categories or billable services for which benefit details and/or an authorization prior to service delivery may be required by the payor.
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
        # Applicable exception or supporting information
        # Exceptions, special conditions and supporting information applicable for this service or product line.
        'supportingInfoSequence' => {
          'type'=>'positiveInt',
          'path'=>'Item.supportingInfoSequence',
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
        # Perfoming practitioner
        # The practitioner who is responsible for the product or service to be rendered to the patient.
        'provider' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole'],
          'type'=>'Reference',
          'path'=>'Item.provider',
          'min'=>0,
          'max'=>1
        },
        ##
        # Count of products or services
        # The number of repetitions of a service or product.
        'quantity' => {
          'type'=>'Quantity',
          'path'=>'Item.quantity',
          'min'=>0,
          'max'=>1
        },
        ##
        # Fee, charge or cost per item
        # The amount charged to the patient by the provider for a single unit.
        'unitPrice' => {
          'type'=>'Money',
          'path'=>'Item.unitPrice',
          'min'=>0,
          'max'=>1
        },
        ##
        # Servicing facility
        # Facility where the services will be provided.
        'facility' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Location', 'http://hl7.org/fhir/StructureDefinition/Organization'],
          'type'=>'Reference',
          'path'=>'Item.facility',
          'min'=>0,
          'max'=>1
        },
        ##
        # Applicable diagnosis
        # Patient diagnosis for which care is sought.
        'diagnosis' => {
          'type'=>'CoverageEligibilityRequest::Item::Diagnosis',
          'path'=>'Item.diagnosis',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Product or service details
        # The plan/proposal/order describing the proposed service in detail.
        'detail' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
          'type'=>'Reference',
          'path'=>'Item.detail',
          'min'=>0,
          'max'=>Float::INFINITY
        }
      }

      ##
      # Applicable diagnosis
      # Patient diagnosis for which care is sought.
      class Diagnosis < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        MULTIPLE_TYPES = {
          'diagnosis[x]' => ['CodeableConcept', 'Reference']
        }
        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'id',
            'path'=>'Diagnosis.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Diagnosis.extension',
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
            'path'=>'Diagnosis.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Nature of illness or problem
          # The nature of illness or problem in a coded form or as a reference to an external defined Condition.
          'diagnosisCodeableConcept' => {
            'valid_codes'=>{
              'http://hl7.org/fhir/sid/icd-10'=>[ '123456', '123457', '987654', '123987', '112233', '997755', '321789' ]
            },
            'type'=>'CodeableConcept',
            'path'=>'Diagnosis.diagnosis[x]',
            'min'=>0,
            'max'=>1,
            'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/icd-10'}
          }
          ##
          # Nature of illness or problem
          # The nature of illness or problem in a coded form or as a reference to an external defined Condition.
          'diagnosisReference' => {
            'valid_codes'=>{
              'http://hl7.org/fhir/sid/icd-10'=>[ '123456', '123457', '987654', '123987', '112233', '997755', '321789' ]
            },
            'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Condition'],
            'type'=>'Reference',
            'path'=>'Diagnosis.diagnosis[x]',
            'min'=>0,
            'max'=>1,
            'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/icd-10'}
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
        # Nature of illness or problem
        # The nature of illness or problem in a coded form or as a reference to an external defined Condition.
        attr_accessor :diagnosisCodeableConcept       # 0-1 CodeableConcept
        ##
        # Nature of illness or problem
        # The nature of illness or problem in a coded form or as a reference to an external defined Condition.
        attr_accessor :diagnosisReference             # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Condition)
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
      # Applicable exception or supporting information
      # Exceptions, special conditions and supporting information applicable for this service or product line.
      attr_accessor :supportingInfoSequence         # 0-* [ positiveInt ]
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
      # Perfoming practitioner
      # The practitioner who is responsible for the product or service to be rendered to the patient.
      attr_accessor :provider                       # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole)
      ##
      # Count of products or services
      # The number of repetitions of a service or product.
      attr_accessor :quantity                       # 0-1 Quantity
      ##
      # Fee, charge or cost per item
      # The amount charged to the patient by the provider for a single unit.
      attr_accessor :unitPrice                      # 0-1 Money
      ##
      # Servicing facility
      # Facility where the services will be provided.
      attr_accessor :facility                       # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Location|http://hl7.org/fhir/StructureDefinition/Organization)
      ##
      # Applicable diagnosis
      # Patient diagnosis for which care is sought.
      attr_accessor :diagnosis                      # 0-* [ CoverageEligibilityRequest::Item::Diagnosis ]
      ##
      # Product or service details
      # The plan/proposal/order describing the proposed service in detail.
      attr_accessor :detail                         # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Resource) ]
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
    # Business Identifier for coverage eligiblity request
    # A unique identifier assigned to this coverage eligiblity request.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # active | cancelled | draft | entered-in-error
    # The status of the resource instance.
    # This element is labeled as a modifier because the status contains codes that mark the resource as not currently valid.
    attr_accessor :status                         # 1-1 code
    ##
    # Desired processing priority
    # When the requestor expects the processor to complete processing.
    attr_accessor :priority                       # 0-1 CodeableConcept
    ##
    # auth-requirements | benefits | discovery | validation
    # Code to specify whether requesting: prior authorization requirements for some service categories or billing codes; benefits for coverages specified or discovered; discovery and return of coverages for the patient; and/or validation that the specified coverage is in-force at the date/period specified or 'now' if not specified.
    attr_accessor :purpose                        # 1-* [ code ]
    ##
    # Intended recipient of products and services
    # The party who is the beneficiary of the supplied coverage and for whom eligibility is sought.
    # 1..1.
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
    # Creation date
    # The date when this resource was created.
    attr_accessor :created                        # 1-1 dateTime
    ##
    # Author
    # Person who created the request.
    attr_accessor :enterer                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole)
    ##
    # Party responsible for the request
    # The provider which is responsible for the request.
    # Typically this field would be 1..1 where this party is responsible for the eligibility request but not necessarily professionally responsible for the provision of the individual products and services listed below.
    attr_accessor :provider                       # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Coverage issuer
    # The Insurer who issued the coverage in question and is the recipient of the request.
    attr_accessor :insurer                        # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Servicing facility
    # Facility where the services are intended to be provided.
    attr_accessor :facility                       # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Location)
    ##
    # Supporting information
    # Additional information codes regarding exceptions, special considerations, the condition, situation, prior or concurrent issues.
    # Often there are multiple jurisdiction specific valuesets which are required.
    attr_accessor :supportingInfo                 # 0-* [ CoverageEligibilityRequest::SupportingInfo ]
    ##
    # Patient insurance information
    # Financial instruments for reimbursement for the health care products and services.
    # All insurance coverages for the patient which may be applicable for reimbursement, of the products and services listed in the claim, are typically provided in the claim to allow insurers to confirm the ordering of the insurance coverages relative to local 'coordination of benefit' rules. One coverage (and only one) with 'focal=true' is to be used in the adjudication of this claim. Coverages appearing before the focal Coverage in the list, and where 'subrogation=false', should provide a reference to the ClaimResponse containing the adjudication results of the prior claim.
    attr_accessor :insurance                      # 0-* [ CoverageEligibilityRequest::Insurance ]
    ##
    # Item to be evaluated for eligibiity
    # Service categories or billable services for which benefit details and/or an authorization prior to service delivery may be required by the payor.
    attr_accessor :item                           # 0-* [ CoverageEligibilityRequest::Item ]

    def resourceType
      'CoverageEligibilityRequest'
    end
  end
end
