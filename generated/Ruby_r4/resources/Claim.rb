module FHIR

  ##
  # A provider issued list of professional services and products which have been provided, or are to be provided, to a patient which is sent to an insurer for reimbursement.
  # The Claim resource is used by providers to exchange services and products rendered to patients or planned to be rendered with insurers for reimbuserment. It is also used by insurers to exchange claims information with statutory reporting and data analytics firms.
  class Claim < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['care-team', 'created', 'detail-udi', 'encounter', 'enterer', 'facility', 'identifier', 'insurer', 'item-udi', 'patient', 'payee', 'priority', 'procedure-udi', 'provider', 'status', 'subdetail-udi', 'use']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'Claim.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'Claim.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'Claim.implicitRules',
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
        'path'=>'Claim.language',
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
        'path'=>'Claim.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'Claim.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Claim.extension',
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
        'path'=>'Claim.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Business Identifier for claim
      # A unique identifier assigned to this claim.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'Claim.identifier',
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
        'path'=>'Claim.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/fm-status'}
      },
      ##
      # Category or discipline
      # The category of claim, e.g. oral, pharmacy, vision, institutional, professional.
      # The majority of jurisdictions use: oral, pharmacy, vision, professional and institutional, or variants on those terms, as the general styles of claims. The valueset is extensible to accommodate other jurisdictional requirements.
      'type' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/claim-type'=>[ 'institutional', 'oral', 'pharmacy', 'professional', 'vision' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Claim.type',
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
        'path'=>'Claim.subType',
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
        'path'=>'Claim.use',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/claim-use'}
      },
      ##
      # The recipient of the products and services
      # The party to whom the professional services and/or products have been supplied or are being considered and for whom actual or forecast reimbursement is sought.
      'patient' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient'],
        'type'=>'Reference',
        'path'=>'Claim.patient',
        'min'=>1,
        'max'=>1
      },
      ##
      # Relevant time frame for the claim
      # The period for which charges are being submitted.
      # Typically this would be today or in the past for a claim, and today or in the future for preauthorizations and predeterminations. Typically line item dates of service should fall within the billing period if one is specified.
      'billablePeriod' => {
        'type'=>'Period',
        'path'=>'Claim.billablePeriod',
        'min'=>0,
        'max'=>1
      },
      ##
      # Resource creation date
      # The date this resource was created.
      # This field is independent of the date of creation of the resource as it may reflect the creation date of a source document prior to digitization. Typically for claims all services must be completed as of this date.
      'created' => {
        'type'=>'dateTime',
        'path'=>'Claim.created',
        'min'=>1,
        'max'=>1
      },
      ##
      # Author of the claim
      # Individual who created the claim, predetermination or preauthorization.
      'enterer' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole'],
        'type'=>'Reference',
        'path'=>'Claim.enterer',
        'min'=>0,
        'max'=>1
      },
      ##
      # Target
      # The Insurer who is target of the request.
      'insurer' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'Claim.insurer',
        'min'=>0,
        'max'=>1
      },
      ##
      # Party responsible for the claim
      # The provider which is responsible for the claim, predetermination or preauthorization.
      # Typically this field would be 1..1 where this party is responsible for the claim but not necessarily professionally responsible for the provision of the individual products and services listed below.
      'provider' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'Claim.provider',
        'min'=>1,
        'max'=>1
      },
      ##
      # Desired processing ugency
      # The provider-required urgency of processing the request. Typical values include: stat, routine deferred.
      # If a claim processor is unable to complete the processing as per the priority then they should generate and error and not process the request.
      'priority' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/processpriority'=>[ 'stat', 'normal', 'deferred' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Claim.priority',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/process-priority'}
      },
      ##
      # For whom to reserve funds
      # A code to indicate whether and for whom funds are to be reserved for future claims.
      # This field is only used for preauthorizations.
      'fundsReserve' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/fundsreserve'=>[ 'patient', 'provider', 'none' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Claim.fundsReserve',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/fundsreserve'}
      },
      ##
      # Prior or corollary claims
      # Other claims which are related to this claim such as prior submissions or claims for related services or for the same event.
      # For example,  for the original treatment and follow-up exams.
      'related' => {
        'type'=>'Claim::Related',
        'path'=>'Claim.related',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Prescription authorizing services and products
      # Prescription to support the dispensing of pharmacy, device or vision products.
      'prescription' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/DeviceRequest', 'http://hl7.org/fhir/StructureDefinition/MedicationRequest', 'http://hl7.org/fhir/StructureDefinition/VisionPrescription'],
        'type'=>'Reference',
        'path'=>'Claim.prescription',
        'min'=>0,
        'max'=>1
      },
      ##
      # Original prescription if superseded by fulfiller
      # Original prescription which has been superseded by this prescription to support the dispensing of pharmacy services, medications or products.
      # For example, a physician may prescribe a medication which the pharmacy determines is contraindicated, or for which the patient has an intolerance, and therefore issues a new prescription for an alternate medication which has the same therapeutic intent. The prescription from the pharmacy becomes the 'prescription' and that from the physician becomes the 'original prescription'.
      'originalPrescription' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/DeviceRequest', 'http://hl7.org/fhir/StructureDefinition/MedicationRequest', 'http://hl7.org/fhir/StructureDefinition/VisionPrescription'],
        'type'=>'Reference',
        'path'=>'Claim.originalPrescription',
        'min'=>0,
        'max'=>1
      },
      ##
      # Recipient of benefits payable
      # The party to be reimbursed for cost of the products and services according to the terms of the policy.
      # Often providers agree to receive the benefits payable to reduce the near-term costs to the patient. The insurer may decline to pay the provider and choose to pay the subscriber instead.
      'payee' => {
        'type'=>'Claim::Payee',
        'path'=>'Claim.payee',
        'min'=>0,
        'max'=>1
      },
      ##
      # Treatment referral
      # A reference to a referral resource.
      # The referral resource which lists the date, practitioner, reason and other supporting information.
      'referral' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ServiceRequest'],
        'type'=>'Reference',
        'path'=>'Claim.referral',
        'min'=>0,
        'max'=>1
      },
      ##
      # Servicing facility
      # Facility where the services were provided.
      'facility' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Location'],
        'type'=>'Reference',
        'path'=>'Claim.facility',
        'min'=>0,
        'max'=>1
      },
      ##
      # Members of the care team
      # The members of the team who provided the products and services.
      'careTeam' => {
        'type'=>'Claim::CareTeam',
        'path'=>'Claim.careTeam',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Supporting information
      # Additional information codes regarding exceptions, special considerations, the condition, situation, prior or concurrent issues.
      # Often there are multiple jurisdiction specific valuesets which are required.
      'supportingInfo' => {
        'type'=>'Claim::SupportingInfo',
        'path'=>'Claim.supportingInfo',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Pertinent diagnosis information
      # Information about diagnoses relevant to the claim items.
      'diagnosis' => {
        'type'=>'Claim::Diagnosis',
        'path'=>'Claim.diagnosis',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Clinical procedures performed
      # Procedures performed on the patient relevant to the billing items with the claim.
      'procedure' => {
        'type'=>'Claim::Procedure',
        'path'=>'Claim.procedure',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Patient insurance information
      # Financial instruments for reimbursement for the health care products and services specified on the claim.
      # All insurance coverages for the patient which may be applicable for reimbursement, of the products and services listed in the claim, are typically provided in the claim to allow insurers to confirm the ordering of the insurance coverages relative to local 'coordination of benefit' rules. One coverage (and only one) with 'focal=true' is to be used in the adjudication of this claim. Coverages appearing before the focal Coverage in the list, and where 'Coverage.subrogation=false', should provide a reference to the ClaimResponse containing the adjudication results of the prior claim.
      'insurance' => {
        'type'=>'Claim::Insurance',
        'path'=>'Claim.insurance',
        'min'=>1,
        'max'=>Float::INFINITY
      },
      ##
      # Details of the event
      # Details of an accident which resulted in injuries which required the products and services listed in the claim.
      'accident' => {
        'type'=>'Claim::Accident',
        'path'=>'Claim.accident',
        'min'=>0,
        'max'=>1
      },
      ##
      # Product or service provided
      # A claim line. Either a simple  product or service or a 'group' of details which can each be a simple items or groups of sub-details.
      'item' => {
        'type'=>'Claim::Item',
        'path'=>'Claim.item',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Total claim cost
      # The total value of the all the items in the claim.
      'total' => {
        'type'=>'Money',
        'path'=>'Claim.total',
        'min'=>0,
        'max'=>1
      }
    }

    ##
    # Prior or corollary claims
    # Other claims which are related to this claim such as prior submissions or claims for related services or for the same event.
    # For example,  for the original treatment and follow-up exams.
    class Related < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Related.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Related.extension',
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
          'path'=>'Related.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Reference to the related claim
        # Reference to a related claim.
        'claim' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Claim'],
          'type'=>'Reference',
          'path'=>'Related.claim',
          'min'=>0,
          'max'=>1
        },
        ##
        # How the reference claim is related
        # A code to convey how the claims are related.
        # For example, prior claim or umbrella.
        'relationship' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/ex-relatedclaimrelationship'=>[ 'prior', 'associated' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Related.relationship',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/related-claim-relationship'}
        },
        ##
        # File or case reference
        # An alternate organizational reference to the case or file to which this particular claim pertains.
        # For example, Property/Casualty insurer claim # or Workers Compensation case # .
        'reference' => {
          'type'=>'Identifier',
          'path'=>'Related.reference',
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
      # Reference to the related claim
      # Reference to a related claim.
      attr_accessor :claim                          # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Claim)
      ##
      # How the reference claim is related
      # A code to convey how the claims are related.
      # For example, prior claim or umbrella.
      attr_accessor :relationship                   # 0-1 CodeableConcept
      ##
      # File or case reference
      # An alternate organizational reference to the case or file to which this particular claim pertains.
      # For example, Property/Casualty insurer claim # or Workers Compensation case # .
      attr_accessor :reference                      # 0-1 Identifier
    end

    ##
    # Recipient of benefits payable
    # The party to be reimbursed for cost of the products and services according to the terms of the policy.
    # Often providers agree to receive the benefits payable to reduce the near-term costs to the patient. The insurer may decline to pay the provider and choose to pay the subscriber instead.
    class Payee < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Payee.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Payee.extension',
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
          'path'=>'Payee.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Category of recipient
        # Type of Party to be reimbursed: subscriber, provider, other.
        'type' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/payeetype'=>[ 'subscriber', 'provider', 'other' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Payee.type',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/payeetype'}
        },
        ##
        # Recipient reference
        # Reference to the individual or organization to whom any payment will be made.
        # Not required if the payee is 'subscriber' or 'provider'.
        'party' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson'],
          'type'=>'Reference',
          'path'=>'Payee.party',
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
      # Category of recipient
      # Type of Party to be reimbursed: subscriber, provider, other.
      attr_accessor :type                           # 1-1 CodeableConcept
      ##
      # Recipient reference
      # Reference to the individual or organization to whom any payment will be made.
      # Not required if the payee is 'subscriber' or 'provider'.
      attr_accessor :party                          # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/RelatedPerson)
    end

    ##
    # Members of the care team
    # The members of the team who provided the products and services.
    class CareTeam < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'CareTeam.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'CareTeam.extension',
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
          'path'=>'CareTeam.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Order of care team
        # A number to uniquely identify care team entries.
        'sequence' => {
          'type'=>'positiveInt',
          'path'=>'CareTeam.sequence',
          'min'=>1,
          'max'=>1
        },
        ##
        # Practitioner or organization
        # Member of the team who provided the product or service.
        'provider' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization'],
          'type'=>'Reference',
          'path'=>'CareTeam.provider',
          'min'=>1,
          'max'=>1
        },
        ##
        # Indicator of the lead practitioner
        # The party who is billing and/or responsible for the claimed products or services.
        # Responsible might not be required when there is only a single provider listed.
        'responsible' => {
          'type'=>'boolean',
          'path'=>'CareTeam.responsible',
          'min'=>0,
          'max'=>1
        },
        ##
        # Function within the team
        # The lead, assisting or supervising practitioner and their discipline if a multidisciplinary team.
        # Role might not be required when there is only a single provider listed.
        'role' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/claimcareteamrole'=>[ 'primary', 'assist', 'supervisor', 'other' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'CareTeam.role',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/claim-careteamrole'}
        },
        ##
        # Practitioner credential or specialization
        # The qualification of the practitioner which is applicable for this service.
        'qualification' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/ex-providerqualification'=>[ '311405', '604215', '604210' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'CareTeam.qualification',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/provider-qualification'}
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
      # Order of care team
      # A number to uniquely identify care team entries.
      attr_accessor :sequence                       # 1-1 positiveInt
      ##
      # Practitioner or organization
      # Member of the team who provided the product or service.
      attr_accessor :provider                       # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization)
      ##
      # Indicator of the lead practitioner
      # The party who is billing and/or responsible for the claimed products or services.
      # Responsible might not be required when there is only a single provider listed.
      attr_accessor :responsible                    # 0-1 boolean
      ##
      # Function within the team
      # The lead, assisting or supervising practitioner and their discipline if a multidisciplinary team.
      # Role might not be required when there is only a single provider listed.
      attr_accessor :role                           # 0-1 CodeableConcept
      ##
      # Practitioner credential or specialization
      # The qualification of the practitioner which is applicable for this service.
      attr_accessor :qualification                  # 0-1 CodeableConcept
    end

    ##
    # Supporting information
    # Additional information codes regarding exceptions, special considerations, the condition, situation, prior or concurrent issues.
    # Often there are multiple jurisdiction specific valuesets which are required.
    class SupportingInfo < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'timing[x]' => ['date', 'Period'],
        'value[x]' => ['Attachment', 'boolean', 'Quantity', 'Reference', 'string']
      }
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
        # Classification of the supplied information
        # The general class of the information supplied: information; exception; accident, employment; onset, etc.
        # This may contain a category for the local bill type codes.
        'category' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/claiminformationcategory'=>[ 'info', 'discharge', 'onset', 'related', 'exception', 'material', 'attachment', 'missingtooth', 'prosthesis', 'other', 'hospitalized', 'employmentimpacted', 'externalcause', 'patientreasonforvisit' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'SupportingInfo.category',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/claim-informationcategory'}
        },
        ##
        # Type of information
        # System and code pertaining to the specific information regarding special conditions relating to the setting, treatment or patient  for which care is sought.
        'code' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/claim-exception'=>[ 'student', 'disabled' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'SupportingInfo.code',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/claim-exception'}
        },
        ##
        # When it occurred
        # The date when or period to which this information refers.
        'timingDate' => {
          'type'=>'Date',
          'path'=>'SupportingInfo.timing[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # When it occurred
        # The date when or period to which this information refers.
        'timingPeriod' => {
          'type'=>'Period',
          'path'=>'SupportingInfo.timing[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Data to be provided
        # Additional data or information such as resources, documents, images etc. including references to the data or the actual inclusion of the data.
        # Could be used to provide references to other resources, document. For example could contain a PDF in an Attachment of the Police Report for an Accident.
        'valueAttachment' => {
          'type'=>'Attachment',
          'path'=>'SupportingInfo.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Data to be provided
        # Additional data or information such as resources, documents, images etc. including references to the data or the actual inclusion of the data.
        # Could be used to provide references to other resources, document. For example could contain a PDF in an Attachment of the Police Report for an Accident.
        'valueBoolean' => {
          'type'=>'Boolean',
          'path'=>'SupportingInfo.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Data to be provided
        # Additional data or information such as resources, documents, images etc. including references to the data or the actual inclusion of the data.
        # Could be used to provide references to other resources, document. For example could contain a PDF in an Attachment of the Police Report for an Accident.
        'valueQuantity' => {
          'type'=>'Quantity',
          'path'=>'SupportingInfo.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Data to be provided
        # Additional data or information such as resources, documents, images etc. including references to the data or the actual inclusion of the data.
        # Could be used to provide references to other resources, document. For example could contain a PDF in an Attachment of the Police Report for an Accident.
        'valueReference' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
          'type'=>'Reference',
          'path'=>'SupportingInfo.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Data to be provided
        # Additional data or information such as resources, documents, images etc. including references to the data or the actual inclusion of the data.
        # Could be used to provide references to other resources, document. For example could contain a PDF in an Attachment of the Police Report for an Accident.
        'valueString' => {
          'type'=>'String',
          'path'=>'SupportingInfo.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Explanation for the information
        # Provides the reason in the situation where a reason code is required in addition to the content.
        # For example: the reason for the additional stay, or why a tooth is  missing.
        'reason' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/missingtoothreason'=>[ 'e', 'c', 'u', 'o' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'SupportingInfo.reason',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/missing-tooth-reason'}
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
      # Classification of the supplied information
      # The general class of the information supplied: information; exception; accident, employment; onset, etc.
      # This may contain a category for the local bill type codes.
      attr_accessor :category                       # 1-1 CodeableConcept
      ##
      # Type of information
      # System and code pertaining to the specific information regarding special conditions relating to the setting, treatment or patient  for which care is sought.
      attr_accessor :code                           # 0-1 CodeableConcept
      ##
      # When it occurred
      # The date when or period to which this information refers.
      attr_accessor :timingDate                     # 0-1 Date
      ##
      # When it occurred
      # The date when or period to which this information refers.
      attr_accessor :timingPeriod                   # 0-1 Period
      ##
      # Data to be provided
      # Additional data or information such as resources, documents, images etc. including references to the data or the actual inclusion of the data.
      # Could be used to provide references to other resources, document. For example could contain a PDF in an Attachment of the Police Report for an Accident.
      attr_accessor :valueAttachment                # 0-1 Attachment
      ##
      # Data to be provided
      # Additional data or information such as resources, documents, images etc. including references to the data or the actual inclusion of the data.
      # Could be used to provide references to other resources, document. For example could contain a PDF in an Attachment of the Police Report for an Accident.
      attr_accessor :valueBoolean                   # 0-1 Boolean
      ##
      # Data to be provided
      # Additional data or information such as resources, documents, images etc. including references to the data or the actual inclusion of the data.
      # Could be used to provide references to other resources, document. For example could contain a PDF in an Attachment of the Police Report for an Accident.
      attr_accessor :valueQuantity                  # 0-1 Quantity
      ##
      # Data to be provided
      # Additional data or information such as resources, documents, images etc. including references to the data or the actual inclusion of the data.
      # Could be used to provide references to other resources, document. For example could contain a PDF in an Attachment of the Police Report for an Accident.
      attr_accessor :valueReference                 # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Resource)
      ##
      # Data to be provided
      # Additional data or information such as resources, documents, images etc. including references to the data or the actual inclusion of the data.
      # Could be used to provide references to other resources, document. For example could contain a PDF in an Attachment of the Police Report for an Accident.
      attr_accessor :valueString                    # 0-1 String
      ##
      # Explanation for the information
      # Provides the reason in the situation where a reason code is required in addition to the content.
      # For example: the reason for the additional stay, or why a tooth is  missing.
      attr_accessor :reason                         # 0-1 CodeableConcept
    end

    ##
    # Pertinent diagnosis information
    # Information about diagnoses relevant to the claim items.
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
        # Diagnosis instance identifier
        # A number to uniquely identify diagnosis entries.
        # Diagnosis are presented in list order to their expected importance: primary, secondary, etc.
        'sequence' => {
          'type'=>'positiveInt',
          'path'=>'Diagnosis.sequence',
          'min'=>1,
          'max'=>1
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
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/icd-10'}
        },
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
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/icd-10'}
        },
        ##
        # Timing or nature of the diagnosis
        # When the condition was observed or the relative ranking.
        # For example: admitting, primary, secondary, discharge.
        'type' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/ex-diagnosistype'=>[ 'admitting', 'clinical', 'differential', 'discharge', 'laboratory', 'nursing', 'prenatal', 'principal', 'radiology', 'remote', 'retrospective', 'self' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Diagnosis.type',
          'min'=>0,
          'max'=>Float::INFINITY,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/ex-diagnosistype'}
        },
        ##
        # Present on admission
        # Indication of whether the diagnosis was present on admission to a facility.
        'onAdmission' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/ex-diagnosis-on-admission'=>[ 'y', 'n', 'u', 'w' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Diagnosis.onAdmission',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/ex-diagnosis-on-admission'}
        },
        ##
        # Package billing code
        # A package billing code or bundle code used to group products and services to a particular health condition (such as heart attack) which is based on a predetermined grouping code system.
        # For example DRG (Diagnosis Related Group) or a bundled billing code. A patient may have a diagnosis of a Myocardial Infarction and a DRG for HeartAttack would be assigned. The Claim item (and possible subsequent claims) would refer to the DRG for those line items that were for services related to the heart attack event.
        'packageCode' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/ex-diagnosisrelatedgroup'=>[ '100', '101', '300', '400' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Diagnosis.packageCode',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/ex-diagnosisrelatedgroup'}
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
      # Diagnosis instance identifier
      # A number to uniquely identify diagnosis entries.
      # Diagnosis are presented in list order to their expected importance: primary, secondary, etc.
      attr_accessor :sequence                       # 1-1 positiveInt
      ##
      # Nature of illness or problem
      # The nature of illness or problem in a coded form or as a reference to an external defined Condition.
      attr_accessor :diagnosisCodeableConcept       # 1-1 CodeableConcept
      ##
      # Nature of illness or problem
      # The nature of illness or problem in a coded form or as a reference to an external defined Condition.
      attr_accessor :diagnosisReference             # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Condition)
      ##
      # Timing or nature of the diagnosis
      # When the condition was observed or the relative ranking.
      # For example: admitting, primary, secondary, discharge.
      attr_accessor :type                           # 0-* [ CodeableConcept ]
      ##
      # Present on admission
      # Indication of whether the diagnosis was present on admission to a facility.
      attr_accessor :onAdmission                    # 0-1 CodeableConcept
      ##
      # Package billing code
      # A package billing code or bundle code used to group products and services to a particular health condition (such as heart attack) which is based on a predetermined grouping code system.
      # For example DRG (Diagnosis Related Group) or a bundled billing code. A patient may have a diagnosis of a Myocardial Infarction and a DRG for HeartAttack would be assigned. The Claim item (and possible subsequent claims) would refer to the DRG for those line items that were for services related to the heart attack event.
      attr_accessor :packageCode                    # 0-1 CodeableConcept
    end

    ##
    # Clinical procedures performed
    # Procedures performed on the patient relevant to the billing items with the claim.
    class Procedure < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'procedure[x]' => ['CodeableConcept', 'Reference']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Procedure.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Procedure.extension',
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
          'path'=>'Procedure.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Procedure instance identifier
        # A number to uniquely identify procedure entries.
        'sequence' => {
          'type'=>'positiveInt',
          'path'=>'Procedure.sequence',
          'min'=>1,
          'max'=>1
        },
        ##
        # Category of Procedure
        # When the condition was observed or the relative ranking.
        # For example: primary, secondary.
        'type' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/ex-procedure-type'=>[ 'primary', 'secondary' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Procedure.type',
          'min'=>0,
          'max'=>Float::INFINITY,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/ex-procedure-type'}
        },
        ##
        # When the procedure was performed
        # Date and optionally time the procedure was performed.
        'date' => {
          'type'=>'dateTime',
          'path'=>'Procedure.date',
          'min'=>0,
          'max'=>1
        },
        ##
        # Specific clinical procedure
        # The code or reference to a Procedure resource which identifies the clinical intervention performed.
        'procedureCodeableConcept' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/sid/ex-icd-10-procedures'=>[ '123001', '123002', '123003' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Procedure.procedure[x]',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/icd-10-procedures'}
        },
        ##
        # Specific clinical procedure
        # The code or reference to a Procedure resource which identifies the clinical intervention performed.
        'procedureReference' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/sid/ex-icd-10-procedures'=>[ '123001', '123002', '123003' ]
          },
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Procedure'],
          'type'=>'Reference',
          'path'=>'Procedure.procedure[x]',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/icd-10-procedures'}
        },
        ##
        # Unique device identifier
        # Unique Device Identifiers associated with this line item.
        'udi' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Device'],
          'type'=>'Reference',
          'path'=>'Procedure.udi',
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
      # Procedure instance identifier
      # A number to uniquely identify procedure entries.
      attr_accessor :sequence                       # 1-1 positiveInt
      ##
      # Category of Procedure
      # When the condition was observed or the relative ranking.
      # For example: primary, secondary.
      attr_accessor :type                           # 0-* [ CodeableConcept ]
      ##
      # When the procedure was performed
      # Date and optionally time the procedure was performed.
      attr_accessor :date                           # 0-1 dateTime
      ##
      # Specific clinical procedure
      # The code or reference to a Procedure resource which identifies the clinical intervention performed.
      attr_accessor :procedureCodeableConcept       # 1-1 CodeableConcept
      ##
      # Specific clinical procedure
      # The code or reference to a Procedure resource which identifies the clinical intervention performed.
      attr_accessor :procedureReference             # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Procedure)
      ##
      # Unique device identifier
      # Unique Device Identifiers associated with this line item.
      attr_accessor :udi                            # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Device) ]
    end

    ##
    # Patient insurance information
    # Financial instruments for reimbursement for the health care products and services specified on the claim.
    # All insurance coverages for the patient which may be applicable for reimbursement, of the products and services listed in the claim, are typically provided in the claim to allow insurers to confirm the ordering of the insurance coverages relative to local 'coordination of benefit' rules. One coverage (and only one) with 'focal=true' is to be used in the adjudication of this claim. Coverages appearing before the focal Coverage in the list, and where 'Coverage.subrogation=false', should provide a reference to the ClaimResponse containing the adjudication results of the prior claim.
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
        # Pre-assigned Claim number
        # The business identifier to be used when the claim is sent for adjudication against this insurance policy.
        # Only required in jurisdictions where insurers, rather than the provider, are required to send claims to  insurers that appear after them in the list. This element is not required when 'subrogation=true'.
        'identifier' => {
          'type'=>'Identifier',
          'path'=>'Insurance.identifier',
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
        },
        ##
        # Prior authorization reference number
        # Reference numbers previously provided by the insurer to the provider to be quoted on subsequent claims containing services or products related to the prior authorization.
        # This value is an alphanumeric string that may be provided over the phone, via text, via paper, or within a ClaimResponse resource and is not a FHIR Identifier.
        'preAuthRef' => {
          'type'=>'string',
          'path'=>'Insurance.preAuthRef',
          'min'=>0,
          'max'=>Float::INFINITY
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
      # Pre-assigned Claim number
      # The business identifier to be used when the claim is sent for adjudication against this insurance policy.
      # Only required in jurisdictions where insurers, rather than the provider, are required to send claims to  insurers that appear after them in the list. This element is not required when 'subrogation=true'.
      attr_accessor :identifier                     # 0-1 Identifier
      ##
      # Insurance information
      # Reference to the insurance card level information contained in the Coverage resource. The coverage issuing insurer will use these details to locate the patient's actual coverage within the insurer's information system.
      attr_accessor :coverage                       # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Coverage)
      ##
      # Additional provider contract number
      # A business agreement number established between the provider and the insurer for special business processing purposes.
      attr_accessor :businessArrangement            # 0-1 string
      ##
      # Prior authorization reference number
      # Reference numbers previously provided by the insurer to the provider to be quoted on subsequent claims containing services or products related to the prior authorization.
      # This value is an alphanumeric string that may be provided over the phone, via text, via paper, or within a ClaimResponse resource and is not a FHIR Identifier.
      attr_accessor :preAuthRef                     # 0-* [ string ]
      ##
      # Adjudication results
      # The result of the adjudication of the line items for the Coverage specified in this insurance.
      # Must not be specified when 'focal=true' for this insurance.
      attr_accessor :claimResponse                  # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/ClaimResponse)
    end

    ##
    # Details of the event
    # Details of an accident which resulted in injuries which required the products and services listed in the claim.
    class Accident < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'location[x]' => ['Address', 'Reference']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Accident.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Accident.extension',
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
          'path'=>'Accident.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # When the incident occurred
        # Date of an accident event  related to the products and services contained in the claim.
        # The date of the accident has to precede the dates of the products and services but within a reasonable timeframe.
        'date' => {
          'type'=>'date',
          'path'=>'Accident.date',
          'min'=>1,
          'max'=>1
        },
        ##
        # The nature of the accident
        # The type or context of the accident event for the purposes of selection of potential insurance coverages and determination of coordination between insurers.
        'type' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/v3-ActCode'=>[ 'MVA', 'SCHOOL', 'SPT', 'WPA' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Accident.type',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'extensible', 'uri'=>'http://terminology.hl7.org/ValueSet/v3-ActIncidentCode'}
        },
        ##
        # Where the event occurred
        # The physical location of the accident event.
        'locationAddress' => {
          'type'=>'Address',
          'path'=>'Accident.location[x]',
          'min'=>0,
          'max'=>1
        }
        ##
        # Where the event occurred
        # The physical location of the accident event.
        'locationReference' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Location'],
          'type'=>'Reference',
          'path'=>'Accident.location[x]',
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
      # When the incident occurred
      # Date of an accident event  related to the products and services contained in the claim.
      # The date of the accident has to precede the dates of the products and services but within a reasonable timeframe.
      attr_accessor :date                           # 1-1 date
      ##
      # The nature of the accident
      # The type or context of the accident event for the purposes of selection of potential insurance coverages and determination of coordination between insurers.
      attr_accessor :type                           # 0-1 CodeableConcept
      ##
      # Where the event occurred
      # The physical location of the accident event.
      attr_accessor :locationAddress                # 0-1 Address
      ##
      # Where the event occurred
      # The physical location of the accident event.
      attr_accessor :locationReference              # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Location)
    end

    ##
    # Product or service provided
    # A claim line. Either a simple  product or service or a 'group' of details which can each be a simple items or groups of sub-details.
    class Item < FHIR::Model
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
        # Item instance identifier
        # A number to uniquely identify item entries.
        'sequence' => {
          'type'=>'positiveInt',
          'path'=>'Item.sequence',
          'min'=>1,
          'max'=>1
        },
        ##
        # Applicable careTeam members
        # CareTeam members related to this service or product.
        'careTeamSequence' => {
          'type'=>'positiveInt',
          'path'=>'Item.careTeamSequence',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Applicable diagnoses
        # Diagnosis applicable for this service or product.
        'diagnosisSequence' => {
          'type'=>'positiveInt',
          'path'=>'Item.diagnosisSequence',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Applicable procedures
        # Procedures applicable for this service or product.
        'procedureSequence' => {
          'type'=>'positiveInt',
          'path'=>'Item.procedureSequence',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Applicable exception and supporting information
        # Exceptions, special conditions and supporting information applicable for this service or product.
        'informationSequence' => {
          'type'=>'positiveInt',
          'path'=>'Item.informationSequence',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Revenue or cost center code
        # The type of revenue or cost center providing the product and/or service.
        'revenue' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/ex-revenue-center'=>[ '0370', '0420', '0421', '0440', '0441', '0450', '0451', '0452', '0010' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Item.revenue',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/ex-revenue-center'}
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
        # When the value is a group code then this item collects a set of related claim details, otherwise this contains the product, service, drug or other billing code for the item.
        # If this is an actual service or product line, i.e. not a Group, then use code to indicate the Professional Service or Product supplied (e.g. CTP, HCPCS, USCLS, ICD10, NCPDP, DIN, RxNorm, ACHI, CCI). If a grouping item then use a group code to indicate the type of thing being grouped e.g. 'glasses' or 'compound'.
        'productOrService' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/ex-USCLS'=>[ '1101', '1102', '1103', '1201', '1205', '2101', '2102', '2141', '2601', '11101', '11102', '11103', '11104', '21211', '21212', '27211', '67211', '99111', '99333', '99555' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Item.productOrService',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/service-uscls'}
        },
        ##
        # Product or service billing modifiers
        # Item typification or modifiers codes to convey additional context for the product or service.
        # For example in Oral whether the treatment is cosmetic or associated with TMJ, or for Medical whether the treatment was outside the clinic or outside of office hours.
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
        # Program the product or service is provided under
        # Identifies the program under which this may be recovered.
        # For example: Neonatal program, child dental program or drug users recovery program.
        'programCode' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/ex-programcode'=>[ 'as', 'hd', 'auscr', 'none' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Item.programCode',
          'min'=>0,
          'max'=>Float::INFINITY,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/ex-program-code'}
        },
        ##
        # Date or dates of service or product delivery
        # The date or dates when the service or product was supplied, performed or completed.
        'servicedDate' => {
          'type'=>'Date',
          'path'=>'Item.serviced[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Date or dates of service or product delivery
        # The date or dates when the service or product was supplied, performed or completed.
        'servicedPeriod' => {
          'type'=>'Period',
          'path'=>'Item.serviced[x]',
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
          'path'=>'Item.location[x]',
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
          'path'=>'Item.location[x]',
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
          'path'=>'Item.location[x]',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/service-place'}
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
        # If the item is not a group then this is the fee for the product or service, otherwise this is the total of the fees for the details of the group.
        'unitPrice' => {
          'type'=>'Money',
          'path'=>'Item.unitPrice',
          'min'=>0,
          'max'=>1
        },
        ##
        # Price scaling factor
        # A real number that represents a multiplier used in determining the overall value of services delivered and/or goods received. The concept of a Factor allows for a discount or surcharge multiplier to be applied to a monetary amount.
        # To show a 10% senior's discount, the value entered is: 0.90 (1.00 - 0.10).
        'factor' => {
          'type'=>'decimal',
          'path'=>'Item.factor',
          'min'=>0,
          'max'=>1
        },
        ##
        # Total item cost
        # The quantity times the unit price for an additional service or product or charge.
        # For example, the formula: quantity * unitPrice * factor  = net. Quantity and factor are assumed to be 1 if not supplied.
        'net' => {
          'type'=>'Money',
          'path'=>'Item.net',
          'min'=>0,
          'max'=>1
        },
        ##
        # Unique device identifier
        # Unique Device Identifiers associated with this line item.
        'udi' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Device'],
          'type'=>'Reference',
          'path'=>'Item.udi',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Anatomical location
        # Physical service site on the patient (limb, tooth, etc.).
        # For example: Providing a tooth code, allows an insurer to identify a provider performing a filling on a tooth that was previously removed.
        'bodySite' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/ex-tooth'=>[ '0', '1', '2', '3', '4', '5', '6', '7', '8', '11', '12', '13', '14', '15', '16', '17', '18', '21', '22', '23', '24', '25', '26', '27', '28', '31', '32', '33', '34', '35', '36', '37', '38', '41', '42', '43', '44', '45', '46', '47', '48' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Item.bodySite',
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
          'path'=>'Item.subSite',
          'min'=>0,
          'max'=>Float::INFINITY,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/surface'}
        },
        ##
        # Encounters related to this billed item
        # The Encounters during which this Claim was created or to which the creation of this record is tightly associated.
        # This will typically be the encounter the event occurred within, but some activities may be initiated prior to or after the official completion of an encounter but still be tied to the context of the encounter.
        'encounter' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Encounter'],
          'type'=>'Reference',
          'path'=>'Item.encounter',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Product or service provided
        # A claim detail line. Either a simple (a product or service) or a 'group' of sub-details which are simple items.
        'detail' => {
          'type'=>'Claim::Item::Detail',
          'path'=>'Item.detail',
          'min'=>0,
          'max'=>Float::INFINITY
        }
      }

      ##
      # Product or service provided
      # A claim detail line. Either a simple (a product or service) or a 'group' of sub-details which are simple items.
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
          # Item instance identifier
          # A number to uniquely identify item entries.
          'sequence' => {
            'type'=>'positiveInt',
            'path'=>'Detail.sequence',
            'min'=>1,
            'max'=>1
          },
          ##
          # Revenue or cost center code
          # The type of revenue or cost center providing the product and/or service.
          'revenue' => {
            'valid_codes'=>{
              'http://terminology.hl7.org/CodeSystem/ex-revenue-center'=>[ '0370', '0420', '0421', '0440', '0441', '0450', '0451', '0452', '0010' ]
            },
            'type'=>'CodeableConcept',
            'path'=>'Detail.revenue',
            'min'=>0,
            'max'=>1,
            'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/ex-revenue-center'}
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
            'path'=>'Detail.category',
            'min'=>0,
            'max'=>1,
            'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/ex-benefitcategory'}
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
          # For example in Oral whether the treatment is cosmetic or associated with TMJ, or for Medical whether the treatment was outside the clinic or out of office hours.
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
          # Program the product or service is provided under
          # Identifies the program under which this may be recovered.
          # For example: Neonatal program, child dental program or drug users recovery program.
          'programCode' => {
            'valid_codes'=>{
              'http://terminology.hl7.org/CodeSystem/ex-programcode'=>[ 'as', 'hd', 'auscr', 'none' ]
            },
            'type'=>'CodeableConcept',
            'path'=>'Detail.programCode',
            'min'=>0,
            'max'=>Float::INFINITY,
            'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/ex-program-code'}
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
          # Unique device identifier
          # Unique Device Identifiers associated with this line item.
          'udi' => {
            'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Device'],
            'type'=>'Reference',
            'path'=>'Detail.udi',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Product or service provided
          # A claim detail line. Either a simple (a product or service) or a 'group' of sub-details which are simple items.
          'subDetail' => {
            'type'=>'Claim::Item::Detail::SubDetail',
            'path'=>'Detail.subDetail',
            'min'=>0,
            'max'=>Float::INFINITY
          }
        }

        ##
        # Product or service provided
        # A claim detail line. Either a simple (a product or service) or a 'group' of sub-details which are simple items.
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
            # Item instance identifier
            # A number to uniquely identify item entries.
            'sequence' => {
              'type'=>'positiveInt',
              'path'=>'SubDetail.sequence',
              'min'=>1,
              'max'=>1
            },
            ##
            # Revenue or cost center code
            # The type of revenue or cost center providing the product and/or service.
            'revenue' => {
              'valid_codes'=>{
                'http://terminology.hl7.org/CodeSystem/ex-revenue-center'=>[ '0370', '0420', '0421', '0440', '0441', '0450', '0451', '0452', '0010' ]
              },
              'type'=>'CodeableConcept',
              'path'=>'SubDetail.revenue',
              'min'=>0,
              'max'=>1,
              'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/ex-revenue-center'}
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
              'path'=>'SubDetail.category',
              'min'=>0,
              'max'=>1,
              'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/ex-benefitcategory'}
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
            # For example in Oral whether the treatment is cosmetic or associated with TMJ, or for Medical whether the treatment was outside the clinic or out of office hours.
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
            # Program the product or service is provided under
            # Identifies the program under which this may be recovered.
            # For example: Neonatal program, child dental program or drug users recovery program.
            'programCode' => {
              'valid_codes'=>{
                'http://terminology.hl7.org/CodeSystem/ex-programcode'=>[ 'as', 'hd', 'auscr', 'none' ]
              },
              'type'=>'CodeableConcept',
              'path'=>'SubDetail.programCode',
              'min'=>0,
              'max'=>Float::INFINITY,
              'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/ex-program-code'}
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
            # Unique device identifier
            # Unique Device Identifiers associated with this line item.
            'udi' => {
              'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Device'],
              'type'=>'Reference',
              'path'=>'SubDetail.udi',
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
          # Item instance identifier
          # A number to uniquely identify item entries.
          attr_accessor :sequence                       # 1-1 positiveInt
          ##
          # Revenue or cost center code
          # The type of revenue or cost center providing the product and/or service.
          attr_accessor :revenue                        # 0-1 CodeableConcept
          ##
          # Benefit classification
          # Code to identify the general type of benefits under which products and services are provided.
          # Examples include Medical Care, Periodontics, Renal Dialysis, Vision Coverage.
          attr_accessor :category                       # 0-1 CodeableConcept
          ##
          # Billing, service, product, or drug code
          # When the value is a group code then this item collects a set of related claim details, otherwise this contains the product, service, drug or other billing code for the item.
          # If this is an actual service or product line, i.e. not a Group, then use code to indicate the Professional Service or Product supplied (e.g. CTP, HCPCS, USCLS, ICD10, NCPDP, DIN, RxNorm, ACHI, CCI). If a grouping item then use a group code to indicate the type of thing being grouped e.g. 'glasses' or 'compound'.
          attr_accessor :productOrService               # 1-1 CodeableConcept
          ##
          # Service/Product billing modifiers
          # Item typification or modifiers codes to convey additional context for the product or service.
          # For example in Oral whether the treatment is cosmetic or associated with TMJ, or for Medical whether the treatment was outside the clinic or out of office hours.
          attr_accessor :modifier                       # 0-* [ CodeableConcept ]
          ##
          # Program the product or service is provided under
          # Identifies the program under which this may be recovered.
          # For example: Neonatal program, child dental program or drug users recovery program.
          attr_accessor :programCode                    # 0-* [ CodeableConcept ]
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
          # Unique device identifier
          # Unique Device Identifiers associated with this line item.
          attr_accessor :udi                            # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Device) ]
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
        # Item instance identifier
        # A number to uniquely identify item entries.
        attr_accessor :sequence                       # 1-1 positiveInt
        ##
        # Revenue or cost center code
        # The type of revenue or cost center providing the product and/or service.
        attr_accessor :revenue                        # 0-1 CodeableConcept
        ##
        # Benefit classification
        # Code to identify the general type of benefits under which products and services are provided.
        # Examples include Medical Care, Periodontics, Renal Dialysis, Vision Coverage.
        attr_accessor :category                       # 0-1 CodeableConcept
        ##
        # Billing, service, product, or drug code
        # When the value is a group code then this item collects a set of related claim details, otherwise this contains the product, service, drug or other billing code for the item.
        # If this is an actual service or product line, i.e. not a Group, then use code to indicate the Professional Service or Product supplied (e.g. CTP, HCPCS, USCLS, ICD10, NCPDP, DIN, RxNorm, ACHI, CCI). If a grouping item then use a group code to indicate the type of thing being grouped e.g. 'glasses' or 'compound'.
        attr_accessor :productOrService               # 1-1 CodeableConcept
        ##
        # Service/Product billing modifiers
        # Item typification or modifiers codes to convey additional context for the product or service.
        # For example in Oral whether the treatment is cosmetic or associated with TMJ, or for Medical whether the treatment was outside the clinic or out of office hours.
        attr_accessor :modifier                       # 0-* [ CodeableConcept ]
        ##
        # Program the product or service is provided under
        # Identifies the program under which this may be recovered.
        # For example: Neonatal program, child dental program or drug users recovery program.
        attr_accessor :programCode                    # 0-* [ CodeableConcept ]
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
        # Unique device identifier
        # Unique Device Identifiers associated with this line item.
        attr_accessor :udi                            # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Device) ]
        ##
        # Product or service provided
        # A claim detail line. Either a simple (a product or service) or a 'group' of sub-details which are simple items.
        attr_accessor :subDetail                      # 0-* [ Claim::Item::Detail::SubDetail ]
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
      # Item instance identifier
      # A number to uniquely identify item entries.
      attr_accessor :sequence                       # 1-1 positiveInt
      ##
      # Applicable careTeam members
      # CareTeam members related to this service or product.
      attr_accessor :careTeamSequence               # 0-* [ positiveInt ]
      ##
      # Applicable diagnoses
      # Diagnosis applicable for this service or product.
      attr_accessor :diagnosisSequence              # 0-* [ positiveInt ]
      ##
      # Applicable procedures
      # Procedures applicable for this service or product.
      attr_accessor :procedureSequence              # 0-* [ positiveInt ]
      ##
      # Applicable exception and supporting information
      # Exceptions, special conditions and supporting information applicable for this service or product.
      attr_accessor :informationSequence            # 0-* [ positiveInt ]
      ##
      # Revenue or cost center code
      # The type of revenue or cost center providing the product and/or service.
      attr_accessor :revenue                        # 0-1 CodeableConcept
      ##
      # Benefit classification
      # Code to identify the general type of benefits under which products and services are provided.
      # Examples include Medical Care, Periodontics, Renal Dialysis, Vision Coverage.
      attr_accessor :category                       # 0-1 CodeableConcept
      ##
      # Billing, service, product, or drug code
      # When the value is a group code then this item collects a set of related claim details, otherwise this contains the product, service, drug or other billing code for the item.
      # If this is an actual service or product line, i.e. not a Group, then use code to indicate the Professional Service or Product supplied (e.g. CTP, HCPCS, USCLS, ICD10, NCPDP, DIN, RxNorm, ACHI, CCI). If a grouping item then use a group code to indicate the type of thing being grouped e.g. 'glasses' or 'compound'.
      attr_accessor :productOrService               # 1-1 CodeableConcept
      ##
      # Product or service billing modifiers
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
      # Unique device identifier
      # Unique Device Identifiers associated with this line item.
      attr_accessor :udi                            # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Device) ]
      ##
      # Anatomical location
      # Physical service site on the patient (limb, tooth, etc.).
      # For example: Providing a tooth code, allows an insurer to identify a provider performing a filling on a tooth that was previously removed.
      attr_accessor :bodySite                       # 0-1 CodeableConcept
      ##
      # Anatomical sub-location
      # A region or surface of the bodySite, e.g. limb region or tooth surface(s).
      attr_accessor :subSite                        # 0-* [ CodeableConcept ]
      ##
      # Encounters related to this billed item
      # The Encounters during which this Claim was created or to which the creation of this record is tightly associated.
      # This will typically be the encounter the event occurred within, but some activities may be initiated prior to or after the official completion of an encounter but still be tied to the context of the encounter.
      attr_accessor :encounter                      # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Encounter) ]
      ##
      # Product or service provided
      # A claim detail line. Either a simple (a product or service) or a 'group' of sub-details which are simple items.
      attr_accessor :detail                         # 0-* [ Claim::Item::Detail ]
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
    # Business Identifier for claim
    # A unique identifier assigned to this claim.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # active | cancelled | draft | entered-in-error
    # The status of the resource instance.
    # This element is labeled as a modifier because the status contains codes that mark the resource as not currently valid.
    attr_accessor :status                         # 1-1 code
    ##
    # Category or discipline
    # The category of claim, e.g. oral, pharmacy, vision, institutional, professional.
    # The majority of jurisdictions use: oral, pharmacy, vision, professional and institutional, or variants on those terms, as the general styles of claims. The valueset is extensible to accommodate other jurisdictional requirements.
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
    # The party to whom the professional services and/or products have been supplied or are being considered and for whom actual or forecast reimbursement is sought.
    attr_accessor :patient                        # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient)
    ##
    # Relevant time frame for the claim
    # The period for which charges are being submitted.
    # Typically this would be today or in the past for a claim, and today or in the future for preauthorizations and predeterminations. Typically line item dates of service should fall within the billing period if one is specified.
    attr_accessor :billablePeriod                 # 0-1 Period
    ##
    # Resource creation date
    # The date this resource was created.
    # This field is independent of the date of creation of the resource as it may reflect the creation date of a source document prior to digitization. Typically for claims all services must be completed as of this date.
    attr_accessor :created                        # 1-1 dateTime
    ##
    # Author of the claim
    # Individual who created the claim, predetermination or preauthorization.
    attr_accessor :enterer                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole)
    ##
    # Target
    # The Insurer who is target of the request.
    attr_accessor :insurer                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Party responsible for the claim
    # The provider which is responsible for the claim, predetermination or preauthorization.
    # Typically this field would be 1..1 where this party is responsible for the claim but not necessarily professionally responsible for the provision of the individual products and services listed below.
    attr_accessor :provider                       # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Desired processing ugency
    # The provider-required urgency of processing the request. Typical values include: stat, routine deferred.
    # If a claim processor is unable to complete the processing as per the priority then they should generate and error and not process the request.
    attr_accessor :priority                       # 1-1 CodeableConcept
    ##
    # For whom to reserve funds
    # A code to indicate whether and for whom funds are to be reserved for future claims.
    # This field is only used for preauthorizations.
    attr_accessor :fundsReserve                   # 0-1 CodeableConcept
    ##
    # Prior or corollary claims
    # Other claims which are related to this claim such as prior submissions or claims for related services or for the same event.
    # For example,  for the original treatment and follow-up exams.
    attr_accessor :related                        # 0-* [ Claim::Related ]
    ##
    # Prescription authorizing services and products
    # Prescription to support the dispensing of pharmacy, device or vision products.
    attr_accessor :prescription                   # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/DeviceRequest|http://hl7.org/fhir/StructureDefinition/MedicationRequest|http://hl7.org/fhir/StructureDefinition/VisionPrescription)
    ##
    # Original prescription if superseded by fulfiller
    # Original prescription which has been superseded by this prescription to support the dispensing of pharmacy services, medications or products.
    # For example, a physician may prescribe a medication which the pharmacy determines is contraindicated, or for which the patient has an intolerance, and therefore issues a new prescription for an alternate medication which has the same therapeutic intent. The prescription from the pharmacy becomes the 'prescription' and that from the physician becomes the 'original prescription'.
    attr_accessor :originalPrescription           # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/DeviceRequest|http://hl7.org/fhir/StructureDefinition/MedicationRequest|http://hl7.org/fhir/StructureDefinition/VisionPrescription)
    ##
    # Recipient of benefits payable
    # The party to be reimbursed for cost of the products and services according to the terms of the policy.
    # Often providers agree to receive the benefits payable to reduce the near-term costs to the patient. The insurer may decline to pay the provider and choose to pay the subscriber instead.
    attr_accessor :payee                          # 0-1 Claim::Payee
    ##
    # Treatment referral
    # A reference to a referral resource.
    # The referral resource which lists the date, practitioner, reason and other supporting information.
    attr_accessor :referral                       # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/ServiceRequest)
    ##
    # Servicing facility
    # Facility where the services were provided.
    attr_accessor :facility                       # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Location)
    ##
    # Members of the care team
    # The members of the team who provided the products and services.
    attr_accessor :careTeam                       # 0-* [ Claim::CareTeam ]
    ##
    # Supporting information
    # Additional information codes regarding exceptions, special considerations, the condition, situation, prior or concurrent issues.
    # Often there are multiple jurisdiction specific valuesets which are required.
    attr_accessor :supportingInfo                 # 0-* [ Claim::SupportingInfo ]
    ##
    # Pertinent diagnosis information
    # Information about diagnoses relevant to the claim items.
    attr_accessor :diagnosis                      # 0-* [ Claim::Diagnosis ]
    ##
    # Clinical procedures performed
    # Procedures performed on the patient relevant to the billing items with the claim.
    attr_accessor :procedure                      # 0-* [ Claim::Procedure ]
    ##
    # Patient insurance information
    # Financial instruments for reimbursement for the health care products and services specified on the claim.
    # All insurance coverages for the patient which may be applicable for reimbursement, of the products and services listed in the claim, are typically provided in the claim to allow insurers to confirm the ordering of the insurance coverages relative to local 'coordination of benefit' rules. One coverage (and only one) with 'focal=true' is to be used in the adjudication of this claim. Coverages appearing before the focal Coverage in the list, and where 'Coverage.subrogation=false', should provide a reference to the ClaimResponse containing the adjudication results of the prior claim.
    attr_accessor :insurance                      # 1-* [ Claim::Insurance ]
    ##
    # Details of the event
    # Details of an accident which resulted in injuries which required the products and services listed in the claim.
    attr_accessor :accident                       # 0-1 Claim::Accident
    ##
    # Product or service provided
    # A claim line. Either a simple  product or service or a 'group' of details which can each be a simple items or groups of sub-details.
    attr_accessor :item                           # 0-* [ Claim::Item ]
    ##
    # Total claim cost
    # The total value of the all the items in the claim.
    attr_accessor :total                          # 0-1 Money

    def resourceType
      'Claim'
    end
  end
end
