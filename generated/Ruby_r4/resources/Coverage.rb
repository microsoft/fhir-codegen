module FHIR

  ##
  # Financial instrument which may be used to reimburse or pay for health care products and services. Includes both insurance and self-payment.
  # Coverage provides a link between covered parties (patients) and the payors of their healthcare costs (both insurance and self-pay).
  class Coverage < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['beneficiary', 'class-type', 'class-value', 'dependent', 'identifier', 'patient', 'payor', 'policy-holder', 'status', 'subscriber', 'type']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'Coverage.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'Coverage.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'Coverage.implicitRules',
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
        'path'=>'Coverage.language',
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
        'path'=>'Coverage.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'Coverage.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Coverage.extension',
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
        'path'=>'Coverage.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Business Identifier for the coverage
      # A unique identifier assigned to this coverage.
      # The main (and possibly only) identifier for the coverage - often referred to as a Member Id, Certificate number, Personal Health Number or Case ID. May be constructed as the concatenation of the Coverage.SubscriberID and the Coverage.dependant.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'Coverage.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # active | cancelled | draft | entered-in-error
      # The status of the resource instance.
      # This element is labeled as a modifier because the status contains the code entered-in-error that marks the coverage as not currently valid.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/fm-status'=>[ 'active', 'cancelled', 'draft', 'entered-in-error' ]
        },
        'type'=>'code',
        'path'=>'Coverage.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/fm-status'}
      },
      ##
      # Coverage category such as medical or accident
      # The type of coverage: social program, medical plan, accident coverage (workers compensation, auto), group health or payment by an individual or organization.
      'type' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/coverage-selfpay'=>[ 'pay' ],
          'http://terminology.hl7.org/CodeSystem/v3-ActCode'=>[ 'EHCPOL', 'HSAPOL', 'AUTOPOL', 'COL', 'UNINSMOT', 'PUBLICPOL', 'DENTPRG', 'DISEASEPRG', 'CANPRG', 'ENDRENAL', 'HIVAIDS', 'MANDPOL', 'MENTPRG', 'SAFNET', 'SUBPRG', 'SUBSIDIZ', 'SUBSIDMC', 'SUBSUPP', 'WCBPOL', 'DENTAL', 'DISEASE', 'DRUGPOL', 'HIP', 'LTC', 'MCPOL', 'POS', 'HMO', 'PPO', 'MENTPOL', 'SUBPOL', 'VISPOL', 'DIS', 'EWB', 'FLEXP', 'LIFE', 'ANNU', 'TLIFE', 'ULIFE', 'PNC', 'REI', 'SURPL', 'UMBRL', 'CHAR', 'CRIME', 'EAP', 'GOVEMP', 'HIRISK', 'IND', 'MILITARY', 'RETIRE', 'SOCIAL', 'VET' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Coverage.type',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/coverage-type'}
      },
      ##
      # Owner of the policy
      # The party who 'owns' the insurance policy.
      # For example: may be an individual, corporation or the subscriber's employer.
      'policyHolder' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'Coverage.policyHolder',
        'min'=>0,
        'max'=>1
      },
      ##
      # Subscriber to the policy
      # The party who has signed-up for or 'owns' the contractual relationship to the policy or to whom the benefit of the policy for services rendered to them or their family is due.
      # May be self or a parent in the case of dependants.
      'subscriber' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson'],
        'type'=>'Reference',
        'path'=>'Coverage.subscriber',
        'min'=>0,
        'max'=>1
      },
      ##
      # ID assigned to the subscriber
      # The insurer assigned ID for the Subscriber.
      'subscriberId' => {
        'type'=>'string',
        'path'=>'Coverage.subscriberId',
        'min'=>0,
        'max'=>1
      },
      ##
      # Plan beneficiary
      # The party who benefits from the insurance coverage; the patient when products and/or services are provided.
      'beneficiary' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient'],
        'type'=>'Reference',
        'path'=>'Coverage.beneficiary',
        'min'=>1,
        'max'=>1
      },
      ##
      # Dependent number
      # A unique identifier for a dependent under the coverage.
      # Periodically the member number is constructed from the subscriberId and the dependant number.
      'dependent' => {
        'type'=>'string',
        'path'=>'Coverage.dependent',
        'min'=>0,
        'max'=>1
      },
      ##
      # Beneficiary relationship to the subscriber
      # The relationship of beneficiary (patient) to the subscriber.
      # Typically, an individual uses policies which are theirs (relationship='self') before policies owned by others.
      'relationship' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/subscriber-relationship'=>[ 'child', 'parent', 'spouse', 'common', 'other', 'self', 'injured' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Coverage.relationship',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/subscriber-relationship'}
      },
      ##
      # Coverage start and end dates
      # Time period during which the coverage is in force. A missing start date indicates the start date isn't known, a missing end date means the coverage is continuing to be in force.
      'period' => {
        'type'=>'Period',
        'path'=>'Coverage.period',
        'min'=>0,
        'max'=>1
      },
      ##
      # Issuer of the policy
      # The program or plan underwriter or payor including both insurance and non-insurance agreements, such as patient-pay agreements.
      # May provide multiple identifiers such as insurance company identifier or business identifier (BIN number).
      # For selfpay it may provide multiple paying persons and/or organizations.
      'payor' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson'],
        'type'=>'Reference',
        'path'=>'Coverage.payor',
        'min'=>1,
        'max'=>Float::INFINITY
      },
      ##
      # Additional coverage classifications
      # A suite of underwriter specific classifiers.
      # For example may be used to identify a class of coverage or employer group, Policy, Plan.
      'class' => {
        'local_name'=>'local_class'
        'type'=>'Coverage::Class',
        'path'=>'Coverage.class',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Relative order of the coverage
      # The order of applicability of this coverage relative to other coverages which are currently in force. Note, there may be gaps in the numbering and this does not imply primary, secondary etc. as the specific positioning of coverages depends upon the episode of care.
      'order' => {
        'type'=>'positiveInt',
        'path'=>'Coverage.order',
        'min'=>0,
        'max'=>1
      },
      ##
      # Insurer network
      # The insurer-specific identifier for the insurer-defined network of providers to which the beneficiary may seek treatment which will be covered at the 'in-network' rate, otherwise 'out of network' terms and conditions apply.
      'network' => {
        'type'=>'string',
        'path'=>'Coverage.network',
        'min'=>0,
        'max'=>1
      },
      ##
      # Patient payments for services/products
      # A suite of codes indicating the cost category and associated amount which have been detailed in the policy and may have been  included on the health card.
      # For example by knowing the patient visit co-pay, the provider can collect the amount prior to undertaking treatment.
      'costToBeneficiary' => {
        'type'=>'Coverage::CostToBeneficiary',
        'path'=>'Coverage.costToBeneficiary',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Reimbursement to insurer
      # When 'subrogation=true' this insurance instance has been included not for adjudication but to provide insurers with the details to recover costs.
      # Typically, automotive and worker's compensation policies would be flagged with 'subrogation=true' to enable healthcare payors to collect against accident claims.
      'subrogation' => {
        'type'=>'boolean',
        'path'=>'Coverage.subrogation',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contract details
      # The policy(s) which constitute this insurance coverage.
      'contract' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Contract'],
        'type'=>'Reference',
        'path'=>'Coverage.contract',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Additional coverage classifications
    # A suite of underwriter specific classifiers.
    # For example may be used to identify a class of coverage or employer group, Policy, Plan.
    class Class < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Class.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Class.extension',
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
          'path'=>'Class.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Type of class such as 'group' or 'plan'
        # The type of classification for which an insurer-specific class label or number and optional name is provided, for example may be used to identify a class of coverage or employer group, Policy, Plan.
        'type' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/coverage-class'=>[ 'group', 'subgroup', 'plan', 'subplan', 'class', 'subclass', 'sequence', 'rxbin', 'rxpcn', 'rxid', 'rxgroup' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Class.type',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/coverage-class'}
        },
        ##
        # Value associated with the type
        # The alphanumeric string value associated with the insurer issued label.
        # For example, the Group or Plan number.
        'value' => {
          'type'=>'string',
          'path'=>'Class.value',
          'min'=>1,
          'max'=>1
        },
        ##
        # Human readable description of the type and value
        # A short description for the class.
        'name' => {
          'type'=>'string',
          'path'=>'Class.name',
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
      # Type of class such as 'group' or 'plan'
      # The type of classification for which an insurer-specific class label or number and optional name is provided, for example may be used to identify a class of coverage or employer group, Policy, Plan.
      attr_accessor :type                           # 1-1 CodeableConcept
      ##
      # Value associated with the type
      # The alphanumeric string value associated with the insurer issued label.
      # For example, the Group or Plan number.
      attr_accessor :value                          # 1-1 string
      ##
      # Human readable description of the type and value
      # A short description for the class.
      attr_accessor :name                           # 0-1 string
    end

    ##
    # Patient payments for services/products
    # A suite of codes indicating the cost category and associated amount which have been detailed in the policy and may have been  included on the health card.
    # For example by knowing the patient visit co-pay, the provider can collect the amount prior to undertaking treatment.
    class CostToBeneficiary < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'value[x]' => ['Money', 'Quantity']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'CostToBeneficiary.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'CostToBeneficiary.extension',
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
          'path'=>'CostToBeneficiary.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Cost category
        # The category of patient centric costs associated with treatment.
        # For example visit, specialist visits, emergency, inpatient care, etc.
        'type' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/coverage-copay-type'=>[ 'gpvisit', 'spvisit', 'emergency', 'inpthosp', 'televisit', 'urgentcare', 'copaypct', 'copay', 'deductible', 'maxoutofpocket' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'CostToBeneficiary.type',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/coverage-copay-type'}
        },
        ##
        # The amount or percentage due from the beneficiary
        # The amount due from the patient for the cost category.
        # Amount may be expressed as a percentage of the service/product cost or a fixed amount of currency.
        'valueMoney' => {
          'type'=>'Money',
          'path'=>'CostToBeneficiary.value[x]',
          'min'=>1,
          'max'=>1
        },
        ##
        # The amount or percentage due from the beneficiary
        # The amount due from the patient for the cost category.
        # Amount may be expressed as a percentage of the service/product cost or a fixed amount of currency.
        'valueQuantity' => {
          'type'=>'Quantity',
          'path'=>'CostToBeneficiary.value[x]',
          'min'=>1,
          'max'=>1
        },
        ##
        # Exceptions for patient payments
        # A suite of codes indicating exceptions or reductions to patient costs and their effective periods.
        'exception' => {
          'type'=>'Coverage::CostToBeneficiary::Exception',
          'path'=>'CostToBeneficiary.exception',
          'min'=>0,
          'max'=>Float::INFINITY
        }
      }

      ##
      # Exceptions for patient payments
      # A suite of codes indicating exceptions or reductions to patient costs and their effective periods.
      class Exception < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
            'path'=>'Exception.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Exception.extension',
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
            'path'=>'Exception.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Exception category
          # The code for the specific exception.
          'type' => {
            'valid_codes'=>{
              'http://terminology.hl7.org/CodeSystem/ex-coverage-financial-exception'=>[ 'retired', 'foster' ]
            },
            'type'=>'CodeableConcept',
            'path'=>'Exception.type',
            'min'=>1,
            'max'=>1,
            'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/coverage-financial-exception'}
          },
          ##
          # The effective period of the exception
          # The timeframe during when the exception is in force.
          'period' => {
            'type'=>'Period',
            'path'=>'Exception.period',
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
        # Exception category
        # The code for the specific exception.
        attr_accessor :type                           # 1-1 CodeableConcept
        ##
        # The effective period of the exception
        # The timeframe during when the exception is in force.
        attr_accessor :period                         # 0-1 Period
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
      # Cost category
      # The category of patient centric costs associated with treatment.
      # For example visit, specialist visits, emergency, inpatient care, etc.
      attr_accessor :type                           # 0-1 CodeableConcept
      ##
      # The amount or percentage due from the beneficiary
      # The amount due from the patient for the cost category.
      # Amount may be expressed as a percentage of the service/product cost or a fixed amount of currency.
      attr_accessor :valueMoney                     # 1-1 Money
      ##
      # The amount or percentage due from the beneficiary
      # The amount due from the patient for the cost category.
      # Amount may be expressed as a percentage of the service/product cost or a fixed amount of currency.
      attr_accessor :valueQuantity                  # 1-1 Quantity
      ##
      # Exceptions for patient payments
      # A suite of codes indicating exceptions or reductions to patient costs and their effective periods.
      attr_accessor :exception                      # 0-* [ Coverage::CostToBeneficiary::Exception ]
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
    # Business Identifier for the coverage
    # A unique identifier assigned to this coverage.
    # The main (and possibly only) identifier for the coverage - often referred to as a Member Id, Certificate number, Personal Health Number or Case ID. May be constructed as the concatenation of the Coverage.SubscriberID and the Coverage.dependant.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # active | cancelled | draft | entered-in-error
    # The status of the resource instance.
    # This element is labeled as a modifier because the status contains the code entered-in-error that marks the coverage as not currently valid.
    attr_accessor :status                         # 1-1 code
    ##
    # Coverage category such as medical or accident
    # The type of coverage: social program, medical plan, accident coverage (workers compensation, auto), group health or payment by an individual or organization.
    attr_accessor :type                           # 0-1 CodeableConcept
    ##
    # Owner of the policy
    # The party who 'owns' the insurance policy.
    # For example: may be an individual, corporation or the subscriber's employer.
    attr_accessor :policyHolder                   # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Subscriber to the policy
    # The party who has signed-up for or 'owns' the contractual relationship to the policy or to whom the benefit of the policy for services rendered to them or their family is due.
    # May be self or a parent in the case of dependants.
    attr_accessor :subscriber                     # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/RelatedPerson)
    ##
    # ID assigned to the subscriber
    # The insurer assigned ID for the Subscriber.
    attr_accessor :subscriberId                   # 0-1 string
    ##
    # Plan beneficiary
    # The party who benefits from the insurance coverage; the patient when products and/or services are provided.
    attr_accessor :beneficiary                    # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient)
    ##
    # Dependent number
    # A unique identifier for a dependent under the coverage.
    # Periodically the member number is constructed from the subscriberId and the dependant number.
    attr_accessor :dependent                      # 0-1 string
    ##
    # Beneficiary relationship to the subscriber
    # The relationship of beneficiary (patient) to the subscriber.
    # Typically, an individual uses policies which are theirs (relationship='self') before policies owned by others.
    attr_accessor :relationship                   # 0-1 CodeableConcept
    ##
    # Coverage start and end dates
    # Time period during which the coverage is in force. A missing start date indicates the start date isn't known, a missing end date means the coverage is continuing to be in force.
    attr_accessor :period                         # 0-1 Period
    ##
    # Issuer of the policy
    # The program or plan underwriter or payor including both insurance and non-insurance agreements, such as patient-pay agreements.
    # May provide multiple identifiers such as insurance company identifier or business identifier (BIN number).
    # For selfpay it may provide multiple paying persons and/or organizations.
    attr_accessor :payor                          # 1-* [ Reference(http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/RelatedPerson) ]
    ##
    # Additional coverage classifications
    # A suite of underwriter specific classifiers.
    # For example may be used to identify a class of coverage or employer group, Policy, Plan.
    attr_accessor :local_class                    # 0-* [ Coverage::Class ]
    ##
    # Relative order of the coverage
    # The order of applicability of this coverage relative to other coverages which are currently in force. Note, there may be gaps in the numbering and this does not imply primary, secondary etc. as the specific positioning of coverages depends upon the episode of care.
    attr_accessor :order                          # 0-1 positiveInt
    ##
    # Insurer network
    # The insurer-specific identifier for the insurer-defined network of providers to which the beneficiary may seek treatment which will be covered at the 'in-network' rate, otherwise 'out of network' terms and conditions apply.
    attr_accessor :network                        # 0-1 string
    ##
    # Patient payments for services/products
    # A suite of codes indicating the cost category and associated amount which have been detailed in the policy and may have been  included on the health card.
    # For example by knowing the patient visit co-pay, the provider can collect the amount prior to undertaking treatment.
    attr_accessor :costToBeneficiary              # 0-* [ Coverage::CostToBeneficiary ]
    ##
    # Reimbursement to insurer
    # When 'subrogation=true' this insurance instance has been included not for adjudication but to provide insurers with the details to recover costs.
    # Typically, automotive and worker's compensation policies would be flagged with 'subrogation=true' to enable healthcare payors to collect against accident claims.
    attr_accessor :subrogation                    # 0-1 boolean
    ##
    # Contract details
    # The policy(s) which constitute this insurance coverage.
    attr_accessor :contract                       # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Contract) ]

    def resourceType
      'Coverage'
    end
  end
end
