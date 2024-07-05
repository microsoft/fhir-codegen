module FHIR

  ##
  # This resource provides: the claim details; adjudication details from the processing of a Claim; and optionally account balance information, for informing the subscriber of the benefits provided.
  class ExplanationOfBenefit < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['care-team', 'claim', 'coverage', 'created', 'detail-udi', 'disposition', 'encounter', 'enterer', 'facility', 'identifier', 'item-udi', 'patient', 'payee', 'procedure-udi', 'provider', 'status', 'subdetail-udi']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'ExplanationOfBenefit.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'ExplanationOfBenefit.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'ExplanationOfBenefit.implicitRules',
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
        'path'=>'ExplanationOfBenefit.language',
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
        'path'=>'ExplanationOfBenefit.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'ExplanationOfBenefit.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'ExplanationOfBenefit.extension',
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
        'path'=>'ExplanationOfBenefit.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Business Identifier for the resource
      # A unique identifier assigned to this explanation of benefit.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'ExplanationOfBenefit.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # active | cancelled | draft | entered-in-error
      # The status of the resource instance.
      # This element is labeled as a modifier because the status contains codes that mark the resource as not currently valid.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/explanationofbenefit-status'=>[ 'active', 'cancelled', 'draft', 'entered-in-error' ]
        },
        'type'=>'code',
        'path'=>'ExplanationOfBenefit.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/explanationofbenefit-status'}
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
        'path'=>'ExplanationOfBenefit.type',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/claim-type'}
      },
      ##
      # More granular claim type
      # A finer grained suite of claim type codes which may convey additional information such as Inpatient vs Outpatient and/or a specialty service.
      # This may contain the local bill type codes such as the US UB-04 bill type code.
      'subType' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/ex-claimsubtype'=>[ 'ortho', 'emergency' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'ExplanationOfBenefit.subType',
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
        'path'=>'ExplanationOfBenefit.use',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/claim-use'}
      },
      ##
      # The recipient of the products and services
      # The party to whom the professional services and/or products have been supplied or are being considered and for whom actual for forecast reimbursement is sought.
      'patient' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient'],
        'type'=>'Reference',
        'path'=>'ExplanationOfBenefit.patient',
        'min'=>1,
        'max'=>1
      },
      ##
      # Relevant time frame for the claim
      # The period for which charges are being submitted.
      # Typically this would be today or in the past for a claim, and today or in the future for preauthorizations and prodeterminations. Typically line item dates of service should fall within the billing period if one is specified.
      'billablePeriod' => {
        'type'=>'Period',
        'path'=>'ExplanationOfBenefit.billablePeriod',
        'min'=>0,
        'max'=>1
      },
      ##
      # Response creation date
      # The date this resource was created.
      # This field is independent of the date of creation of the resource as it may reflect the creation date of a source document prior to digitization. Typically for claims all services must be completed as of this date.
      'created' => {
        'type'=>'dateTime',
        'path'=>'ExplanationOfBenefit.created',
        'min'=>1,
        'max'=>1
      },
      ##
      # Author of the claim
      # Individual who created the claim, predetermination or preauthorization.
      'enterer' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole'],
        'type'=>'Reference',
        'path'=>'ExplanationOfBenefit.enterer',
        'min'=>0,
        'max'=>1
      },
      ##
      # Party responsible for reimbursement
      # The party responsible for authorization, adjudication and reimbursement.
      'insurer' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'ExplanationOfBenefit.insurer',
        'min'=>1,
        'max'=>1
      },
      ##
      # Party responsible for the claim
      # The provider which is responsible for the claim, predetermination or preauthorization.
      # Typically this field would be 1..1 where this party is responsible for the claim but not necessarily professionally responsible for the provision of the individual products and services listed below.
      'provider' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'ExplanationOfBenefit.provider',
        'min'=>1,
        'max'=>1
      },
      ##
      # Desired processing urgency
      # The provider-required urgency of processing the request. Typical values include: stat, routine deferred.
      # If a claim processor is unable to complete the processing as per the priority then they should generate and error and not process the request.
      'priority' => {
        'type'=>'CodeableConcept',
        'path'=>'ExplanationOfBenefit.priority',
        'min'=>0,
        'max'=>1
      },
      ##
      # For whom to reserve funds
      # A code to indicate whether and for whom funds are to be reserved for future claims.
      # This field is only used for preauthorizations.
      'fundsReserveRequested' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/fundsreserve'=>[ 'patient', 'provider', 'none' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'ExplanationOfBenefit.fundsReserveRequested',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/fundsreserve'}
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
        'path'=>'ExplanationOfBenefit.fundsReserve',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/fundsreserve'}
      },
      ##
      # Prior or corollary claims
      # Other claims which are related to this claim such as prior submissions or claims for related services or for the same event.
      # For example,  for the original treatment and follow-up exams.
      'related' => {
        'type'=>'ExplanationOfBenefit::Related',
        'path'=>'ExplanationOfBenefit.related',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Prescription authorizing services or products
      # Prescription to support the dispensing of pharmacy, device or vision products.
      'prescription' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/MedicationRequest', 'http://hl7.org/fhir/StructureDefinition/VisionPrescription'],
        'type'=>'Reference',
        'path'=>'ExplanationOfBenefit.prescription',
        'min'=>0,
        'max'=>1
      },
      ##
      # Original prescription if superceded by fulfiller
      # Original prescription which has been superseded by this prescription to support the dispensing of pharmacy services, medications or products.
      # For example, a physician may prescribe a medication which the pharmacy determines is contraindicated, or for which the patient has an intolerance, and therefor issues a new prescription for an alternate medication which has the same therapeutic intent. The prescription from the pharmacy becomes the 'prescription' and that from the physician becomes the 'original prescription'.
      'originalPrescription' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/MedicationRequest'],
        'type'=>'Reference',
        'path'=>'ExplanationOfBenefit.originalPrescription',
        'min'=>0,
        'max'=>1
      },
      ##
      # Recipient of benefits payable
      # The party to be reimbursed for cost of the products and services according to the terms of the policy.
      # Often providers agree to receive the benefits payable to reduce the near-term costs to the patient. The insurer may decline to pay the provider and may choose to pay the subscriber instead.
      'payee' => {
        'type'=>'ExplanationOfBenefit::Payee',
        'path'=>'ExplanationOfBenefit.payee',
        'min'=>0,
        'max'=>1
      },
      ##
      # Treatment Referral
      # A reference to a referral resource.
      # The referral resource which lists the date, practitioner, reason and other supporting information.
      'referral' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ServiceRequest'],
        'type'=>'Reference',
        'path'=>'ExplanationOfBenefit.referral',
        'min'=>0,
        'max'=>1
      },
      ##
      # Servicing Facility
      # Facility where the services were provided.
      'facility' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Location'],
        'type'=>'Reference',
        'path'=>'ExplanationOfBenefit.facility',
        'min'=>0,
        'max'=>1
      },
      ##
      # Claim reference
      # The business identifier for the instance of the adjudication request: claim predetermination or preauthorization.
      'claim' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Claim'],
        'type'=>'Reference',
        'path'=>'ExplanationOfBenefit.claim',
        'min'=>0,
        'max'=>1
      },
      ##
      # Claim response reference
      # The business identifier for the instance of the adjudication response: claim, predetermination or preauthorization response.
      'claimResponse' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ClaimResponse'],
        'type'=>'Reference',
        'path'=>'ExplanationOfBenefit.claimResponse',
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
        'path'=>'ExplanationOfBenefit.outcome',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/remittance-outcome'}
      },
      ##
      # Disposition Message
      # A human readable description of the status of the adjudication.
      'disposition' => {
        'type'=>'string',
        'path'=>'ExplanationOfBenefit.disposition',
        'min'=>0,
        'max'=>1
      },
      ##
      # Preauthorization reference
      # Reference from the Insurer which is used in later communications which refers to this adjudication.
      # This value is only present on preauthorization adjudications.
      'preAuthRef' => {
        'type'=>'string',
        'path'=>'ExplanationOfBenefit.preAuthRef',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Preauthorization in-effect period
      # The timeframe during which the supplied preauthorization reference may be quoted on claims to obtain the adjudication as provided.
      # This value is only present on preauthorization adjudications.
      'preAuthRefPeriod' => {
        'type'=>'Period',
        'path'=>'ExplanationOfBenefit.preAuthRefPeriod',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Care Team members
      # The members of the team who provided the products and services.
      'careTeam' => {
        'type'=>'ExplanationOfBenefit::CareTeam',
        'path'=>'ExplanationOfBenefit.careTeam',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Supporting information
      # Additional information codes regarding exceptions, special considerations, the condition, situation, prior or concurrent issues.
      # Often there are multiple jurisdiction specific valuesets which are required.
      'supportingInfo' => {
        'type'=>'ExplanationOfBenefit::SupportingInfo',
        'path'=>'ExplanationOfBenefit.supportingInfo',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Pertinent diagnosis information
      # Information about diagnoses relevant to the claim items.
      'diagnosis' => {
        'type'=>'ExplanationOfBenefit::Diagnosis',
        'path'=>'ExplanationOfBenefit.diagnosis',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Clinical procedures performed
      # Procedures performed on the patient relevant to the billing items with the claim.
      'procedure' => {
        'type'=>'ExplanationOfBenefit::Procedure',
        'path'=>'ExplanationOfBenefit.procedure',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Precedence (primary, secondary, etc.)
      # This indicates the relative order of a series of EOBs related to different coverages for the same suite of services.
      'precedence' => {
        'type'=>'positiveInt',
        'path'=>'ExplanationOfBenefit.precedence',
        'min'=>0,
        'max'=>1
      },
      ##
      # Patient insurance information
      # Financial instruments for reimbursement for the health care products and services specified on the claim.
      # All insurance coverages for the patient which may be applicable for reimbursement, of the products and services listed in the claim, are typically provided in the claim to allow insurers to confirm the ordering of the insurance coverages relative to local 'coordination of benefit' rules. One coverage (and only one) with 'focal=true' is to be used in the adjudication of this claim. Coverages appearing before the focal Coverage in the list, and where 'Coverage.subrogation=false', should provide a reference to the ClaimResponse containing the adjudication results of the prior claim.
      'insurance' => {
        'type'=>'ExplanationOfBenefit::Insurance',
        'path'=>'ExplanationOfBenefit.insurance',
        'min'=>1,
        'max'=>Float::INFINITY
      },
      ##
      # Details of the event
      # Details of a accident which resulted in injuries which required the products and services listed in the claim.
      'accident' => {
        'type'=>'ExplanationOfBenefit::Accident',
        'path'=>'ExplanationOfBenefit.accident',
        'min'=>0,
        'max'=>1
      },
      ##
      # Product or service provided
      # A claim line. Either a simple (a product or service) or a 'group' of details which can also be a simple items or groups of sub-details.
      'item' => {
        'type'=>'ExplanationOfBenefit::Item',
        'path'=>'ExplanationOfBenefit.item',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Insurer added line items
      # The first-tier service adjudications for payor added product or service lines.
      'addItem' => {
        'type'=>'ExplanationOfBenefit::AddItem',
        'path'=>'ExplanationOfBenefit.addItem',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Header-level adjudication
      # The adjudication results which are presented at the header level rather than at the line-item or add-item levels.
      'adjudication' => {
        'type'=>'ExplanationOfBenefit::Item::Adjudication',
        'path'=>'ExplanationOfBenefit.adjudication',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Adjudication totals
      # Categorized monetary totals for the adjudication.
      # Totals for amounts submitted, co-pays, benefits payable etc.
      'total' => {
        'type'=>'ExplanationOfBenefit::Total',
        'path'=>'ExplanationOfBenefit.total',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Payment Details
      # Payment details for the adjudication of the claim.
      'payment' => {
        'type'=>'ExplanationOfBenefit::Payment',
        'path'=>'ExplanationOfBenefit.payment',
        'min'=>0,
        'max'=>1
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
        'path'=>'ExplanationOfBenefit.formCode',
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
        'path'=>'ExplanationOfBenefit.form',
        'min'=>0,
        'max'=>1
      },
      ##
      # Note concerning adjudication
      # A note that describes or explains adjudication results in a human readable form.
      'processNote' => {
        'type'=>'ExplanationOfBenefit::ProcessNote',
        'path'=>'ExplanationOfBenefit.processNote',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # When the benefits are applicable
      # The term of the benefits documented in this response.
      # Not applicable when use=claim.
      'benefitPeriod' => {
        'type'=>'Period',
        'path'=>'ExplanationOfBenefit.benefitPeriod',
        'min'=>0,
        'max'=>1
      },
      ##
      # Balance by Benefit Category.
      'benefitBalance' => {
        'type'=>'ExplanationOfBenefit::BenefitBalance',
        'path'=>'ExplanationOfBenefit.benefitBalance',
        'min'=>0,
        'max'=>Float::INFINITY
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
          'type'=>'string',
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
        # For example, Property/Casualty insurer claim number or Workers Compensation case number.
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
      # For example, Property/Casualty insurer claim number or Workers Compensation case number.
      attr_accessor :reference                      # 0-1 Identifier
    end

    ##
    # Recipient of benefits payable
    # The party to be reimbursed for cost of the products and services according to the terms of the policy.
    # Often providers agree to receive the benefits payable to reduce the near-term costs to the patient. The insurer may decline to pay the provider and may choose to pay the subscriber instead.
    class Payee < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
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
        # Type of Party to be reimbursed: Subscriber, provider, other.
        'type' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/payeetype'=>[ 'subscriber', 'provider', 'other' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Payee.type',
          'min'=>0,
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
      # Category of recipient
      # Type of Party to be reimbursed: Subscriber, provider, other.
      attr_accessor :type                           # 0-1 CodeableConcept
      ##
      # Recipient reference
      # Reference to the individual or organization to whom any payment will be made.
      # Not required if the payee is 'subscriber' or 'provider'.
      attr_accessor :party                          # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/RelatedPerson)
    end

    ##
    # Care Team members
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
          'type'=>'string',
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
          'type'=>'string',
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
        # This may contain the local bill type codes such as the US UB-04 bill type code.
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
        # Could be used to provide references to other resources, document. For example, could contain a PDF in an Attachment of the Police Report for an Accident.
        'valueAttachment' => {
          'type'=>'Attachment',
          'path'=>'SupportingInfo.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Data to be provided
        # Additional data or information such as resources, documents, images etc. including references to the data or the actual inclusion of the data.
        # Could be used to provide references to other resources, document. For example, could contain a PDF in an Attachment of the Police Report for an Accident.
        'valueBoolean' => {
          'type'=>'Boolean',
          'path'=>'SupportingInfo.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Data to be provided
        # Additional data or information such as resources, documents, images etc. including references to the data or the actual inclusion of the data.
        # Could be used to provide references to other resources, document. For example, could contain a PDF in an Attachment of the Police Report for an Accident.
        'valueQuantity' => {
          'type'=>'Quantity',
          'path'=>'SupportingInfo.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Data to be provided
        # Additional data or information such as resources, documents, images etc. including references to the data or the actual inclusion of the data.
        # Could be used to provide references to other resources, document. For example, could contain a PDF in an Attachment of the Police Report for an Accident.
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
        # Could be used to provide references to other resources, document. For example, could contain a PDF in an Attachment of the Police Report for an Accident.
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
          'type'=>'Coding',
          'path'=>'SupportingInfo.reason',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/missing-tooth-reason'}
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
      # This may contain the local bill type codes such as the US UB-04 bill type code.
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
      # Could be used to provide references to other resources, document. For example, could contain a PDF in an Attachment of the Police Report for an Accident.
      attr_accessor :valueAttachment                # 0-1 Attachment
      ##
      # Data to be provided
      # Additional data or information such as resources, documents, images etc. including references to the data or the actual inclusion of the data.
      # Could be used to provide references to other resources, document. For example, could contain a PDF in an Attachment of the Police Report for an Accident.
      attr_accessor :valueBoolean                   # 0-1 Boolean
      ##
      # Data to be provided
      # Additional data or information such as resources, documents, images etc. including references to the data or the actual inclusion of the data.
      # Could be used to provide references to other resources, document. For example, could contain a PDF in an Attachment of the Police Report for an Accident.
      attr_accessor :valueQuantity                  # 0-1 Quantity
      ##
      # Data to be provided
      # Additional data or information such as resources, documents, images etc. including references to the data or the actual inclusion of the data.
      # Could be used to provide references to other resources, document. For example, could contain a PDF in an Attachment of the Police Report for an Accident.
      attr_accessor :valueReference                 # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Resource)
      ##
      # Data to be provided
      # Additional data or information such as resources, documents, images etc. including references to the data or the actual inclusion of the data.
      # Could be used to provide references to other resources, document. For example, could contain a PDF in an Attachment of the Police Report for an Accident.
      attr_accessor :valueString                    # 0-1 String
      ##
      # Explanation for the information
      # Provides the reason in the situation where a reason code is required in addition to the content.
      # For example: the reason for the additional stay, or why a tooth is  missing.
      attr_accessor :reason                         # 0-1 Coding
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
          'type'=>'string',
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
        # For example, DRG (Diagnosis Related Group) or a bundled billing code. A patient may have a diagnosis of a Myocardio-infarction and a DRG for HeartAttack would assigned. The Claim item (and possible subsequent claims) would refer to the DRG for those line items that were for services related to the heart attack event.
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
      # For example, DRG (Diagnosis Related Group) or a bundled billing code. A patient may have a diagnosis of a Myocardio-infarction and a DRG for HeartAttack would assigned. The Claim item (and possible subsequent claims) would refer to the DRG for those line items that were for services related to the heart attack event.
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
          'type'=>'string',
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
      # Procedure instance identifier
      # A number to uniquely identify procedure entries.
      attr_accessor :sequence                       # 1-1 positiveInt
      ##
      # Category of Procedure
      # When the condition was observed or the relative ranking.
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
        # Coverage to be used for adjudication
        # A flag to indicate that this Coverage is to be used for adjudication of this claim when set to true.
        # A patient may (will) have multiple insurance policies which provide reimbursement for healthcare services and products. For example, a person may also be covered by their spouse's policy and both appear in the list (and may be from the same insurer). This flag will be set to true for only one of the listed policies and that policy will be used for adjudicating this claim. Other claims would be created to request adjudication against the other listed policies.
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
        # Prior authorization reference number
        # Reference numbers previously provided by the insurer to the provider to be quoted on subsequent claims containing services or products related to the prior authorization.
        # This value is an alphanumeric string that may be provided over the phone, via text, via paper, or within a ClaimResponse resource and is not a FHIR Identifier.
        'preAuthRef' => {
          'type'=>'string',
          'path'=>'Insurance.preAuthRef',
          'min'=>0,
          'max'=>Float::INFINITY
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
      # Coverage to be used for adjudication
      # A flag to indicate that this Coverage is to be used for adjudication of this claim when set to true.
      # A patient may (will) have multiple insurance policies which provide reimbursement for healthcare services and products. For example, a person may also be covered by their spouse's policy and both appear in the list (and may be from the same insurer). This flag will be set to true for only one of the listed policies and that policy will be used for adjudicating this claim. Other claims would be created to request adjudication against the other listed policies.
      attr_accessor :focal                          # 1-1 boolean
      ##
      # Insurance information
      # Reference to the insurance card level information contained in the Coverage resource. The coverage issuing insurer will use these details to locate the patient's actual coverage within the insurer's information system.
      attr_accessor :coverage                       # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Coverage)
      ##
      # Prior authorization reference number
      # Reference numbers previously provided by the insurer to the provider to be quoted on subsequent claims containing services or products related to the prior authorization.
      # This value is an alphanumeric string that may be provided over the phone, via text, via paper, or within a ClaimResponse resource and is not a FHIR Identifier.
      attr_accessor :preAuthRef                     # 0-* [ string ]
    end

    ##
    # Details of the event
    # Details of a accident which resulted in injuries which required the products and services listed in the claim.
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
          'type'=>'string',
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
          'min'=>0,
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
      # When the incident occurred
      # Date of an accident event  related to the products and services contained in the claim.
      # The date of the accident has to precede the dates of the products and services but within a reasonable timeframe.
      attr_accessor :date                           # 0-1 date
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
    # A claim line. Either a simple (a product or service) or a 'group' of details which can also be a simple items or groups of sub-details.
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
        # Item instance identifier
        # A number to uniquely identify item entries.
        'sequence' => {
          'type'=>'positiveInt',
          'path'=>'Item.sequence',
          'min'=>1,
          'max'=>1
        },
        ##
        # Applicable care team members
        # Care team members related to this service or product.
        'careTeamSequence' => {
          'type'=>'positiveInt',
          'path'=>'Item.careTeamSequence',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Applicable diagnoses
        # Diagnoses applicable for this service or product.
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
        # For example, in Oral whether the treatment is cosmetic or associated with TMJ, or for Medical whether the treatment was outside the clinic or out of office hours.
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
        # A billed item may include goods or services provided in multiple encounters.
        'encounter' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Encounter'],
          'type'=>'Reference',
          'path'=>'Item.encounter',
          'min'=>0,
          'max'=>Float::INFINITY
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
          'type'=>'ExplanationOfBenefit::Item::Adjudication',
          'path'=>'Item.adjudication',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Additional items
        # Second-tier of goods and services.
        'detail' => {
          'type'=>'ExplanationOfBenefit::Item::Detail',
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
            'type'=>'string',
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
          # A code to indicate the information type of this adjudication record. Information types may include: the value submitted, maximum values or percentages allowed or payable under the plan, amounts that the patient is responsible for in-aggregate or pertaining to this item, amounts paid by other coverages, and the benefit payable for this item.
          # For example, codes indicating: Co-Pay, deductible, eligible, benefit, tax, etc.
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
          # For example, may indicate that the funds for this benefit type have been exhausted.
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
          # For example, amount submitted, eligible amount, co-payment, and benefit payable.
          'amount' => {
            'type'=>'Money',
            'path'=>'Adjudication.amount',
            'min'=>0,
            'max'=>1
          },
          ##
          # Non-monitary value
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
        # Type of adjudication information
        # A code to indicate the information type of this adjudication record. Information types may include: the value submitted, maximum values or percentages allowed or payable under the plan, amounts that the patient is responsible for in-aggregate or pertaining to this item, amounts paid by other coverages, and the benefit payable for this item.
        # For example, codes indicating: Co-Pay, deductible, eligible, benefit, tax, etc.
        attr_accessor :category                       # 1-1 CodeableConcept
        ##
        # Explanation of adjudication outcome
        # A code supporting the understanding of the adjudication result and explaining variance from expected amount.
        # For example, may indicate that the funds for this benefit type have been exhausted.
        attr_accessor :reason                         # 0-1 CodeableConcept
        ##
        # Monetary amount associated with the category.
        # For example, amount submitted, eligible amount, co-payment, and benefit payable.
        attr_accessor :amount                         # 0-1 Money
        ##
        # Non-monitary value
        # A non-monetary value associated with the category. Mutually exclusive to the amount element above.
        # For example: eligible percentage or co-payment percentage.
        attr_accessor :value                          # 0-1 decimal
      end

      ##
      # Additional items
      # Second-tier of goods and services.
      class Detail < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
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
          # Product or service provided
          # A claim detail line. Either a simple (a product or service) or a 'group' of sub-details which are simple items.
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
          # Examples include: Medical Care, Periodontics, Renal Dialysis, Vision Coverage.
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
          # For example, in Oral whether the treatment is cosmetic or associated with TMJ, or for Medical whether the treatment was outside the clinic or out of office hours.
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
            'type'=>'ExplanationOfBenefit::Item::Adjudication',
            'path'=>'Detail.adjudication',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Additional items
          # Third-tier of goods and services.
          'subDetail' => {
            'type'=>'ExplanationOfBenefit::Item::Detail::SubDetail',
            'path'=>'Detail.subDetail',
            'min'=>0,
            'max'=>Float::INFINITY
          }
        }

        ##
        # Additional items
        # Third-tier of goods and services.
        class SubDetail < FHIR::Model
          include FHIR::Hashable
          include FHIR::Json
          include FHIR::Xml

          METADATA = {
            ##
            # Unique id for inter-element referencing
            # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
            'id' => {
              'type'=>'string',
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
            # Product or service provided
            # A claim detail line. Either a simple (a product or service) or a 'group' of sub-details which are simple items.
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
            # For example, in Oral whether the treatment is cosmetic or associated with TMJ, or for Medical whether the treatment was outside the clinic or outside of office hours.
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
              'type'=>'ExplanationOfBenefit::Item::Adjudication',
              'path'=>'SubDetail.adjudication',
              'min'=>0,
              'max'=>Float::INFINITY
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
          # Product or service provided
          # A claim detail line. Either a simple (a product or service) or a 'group' of sub-details which are simple items.
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
          # For example, in Oral whether the treatment is cosmetic or associated with TMJ, or for Medical whether the treatment was outside the clinic or outside of office hours.
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
          # Applicable note numbers
          # The numbers associated with notes below which apply to the adjudication of this item.
          attr_accessor :noteNumber                     # 0-* [ positiveInt ]
          ##
          # Subdetail level adjudication details
          # The adjudication results.
          attr_accessor :adjudication                   # 0-* [ ExplanationOfBenefit::Item::Adjudication ]
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
        # Product or service provided
        # A claim detail line. Either a simple (a product or service) or a 'group' of sub-details which are simple items.
        attr_accessor :sequence                       # 1-1 positiveInt
        ##
        # Revenue or cost center code
        # The type of revenue or cost center providing the product and/or service.
        attr_accessor :revenue                        # 0-1 CodeableConcept
        ##
        # Benefit classification
        # Code to identify the general type of benefits under which products and services are provided.
        # Examples include: Medical Care, Periodontics, Renal Dialysis, Vision Coverage.
        attr_accessor :category                       # 0-1 CodeableConcept
        ##
        # Billing, service, product, or drug code
        # When the value is a group code then this item collects a set of related claim details, otherwise this contains the product, service, drug or other billing code for the item.
        # If this is an actual service or product line, i.e. not a Group, then use code to indicate the Professional Service or Product supplied (e.g. CTP, HCPCS, USCLS, ICD10, NCPDP, DIN, RxNorm, ACHI, CCI). If a grouping item then use a group code to indicate the type of thing being grouped e.g. 'glasses' or 'compound'.
        attr_accessor :productOrService               # 1-1 CodeableConcept
        ##
        # Service/Product billing modifiers
        # Item typification or modifiers codes to convey additional context for the product or service.
        # For example, in Oral whether the treatment is cosmetic or associated with TMJ, or for Medical whether the treatment was outside the clinic or out of office hours.
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
        # Applicable note numbers
        # The numbers associated with notes below which apply to the adjudication of this item.
        attr_accessor :noteNumber                     # 0-* [ positiveInt ]
        ##
        # Detail level adjudication details
        # The adjudication results.
        attr_accessor :adjudication                   # 0-* [ ExplanationOfBenefit::Item::Adjudication ]
        ##
        # Additional items
        # Third-tier of goods and services.
        attr_accessor :subDetail                      # 0-* [ ExplanationOfBenefit::Item::Detail::SubDetail ]
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
      # Item instance identifier
      # A number to uniquely identify item entries.
      attr_accessor :sequence                       # 1-1 positiveInt
      ##
      # Applicable care team members
      # Care team members related to this service or product.
      attr_accessor :careTeamSequence               # 0-* [ positiveInt ]
      ##
      # Applicable diagnoses
      # Diagnoses applicable for this service or product.
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
      # For example, in Oral whether the treatment is cosmetic or associated with TMJ, or for Medical whether the treatment was outside the clinic or out of office hours.
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
      # A billed item may include goods or services provided in multiple encounters.
      attr_accessor :encounter                      # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Encounter) ]
      ##
      # Applicable note numbers
      # The numbers associated with notes below which apply to the adjudication of this item.
      attr_accessor :noteNumber                     # 0-* [ positiveInt ]
      ##
      # Adjudication details
      # If this item is a group then the values here are a summary of the adjudication of the detail items. If this item is a simple product or service then this is the result of the adjudication of this item.
      attr_accessor :adjudication                   # 0-* [ ExplanationOfBenefit::Item::Adjudication ]
      ##
      # Additional items
      # Second-tier of goods and services.
      attr_accessor :detail                         # 0-* [ ExplanationOfBenefit::Item::Detail ]
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
          'type'=>'string',
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
        # The sequence number of the sub-details woithin the details within the claim item which this line is intended to replace.
        'subDetailSequence' => {
          'type'=>'positiveInt',
          'path'=>'AddItem.subDetailSequence',
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
        # For example, in Oral whether the treatment is cosmetic or associated with TMJ, or for Medical whether the treatment was outside the clinic or out of office hours.
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
        # For example, providing a tooth code allows an insurer to identify a provider performing a filling on a tooth that was previously removed.
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
          'type'=>'ExplanationOfBenefit::Item::Adjudication',
          'path'=>'AddItem.adjudication',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Insurer added line items
        # The second-tier service adjudications for payor added services.
        'detail' => {
          'type'=>'ExplanationOfBenefit::AddItem::Detail',
          'path'=>'AddItem.detail',
          'min'=>0,
          'max'=>Float::INFINITY
        }
      }

      ##
      # Insurer added line items
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
            'type'=>'string',
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
          # For example, in Oral whether the treatment is cosmetic or associated with TMJ, or for Medical whether the treatment was outside the clinic or out of office hours.
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
          # Added items adjudication
          # The adjudication results.
          'adjudication' => {
            'type'=>'ExplanationOfBenefit::Item::Adjudication',
            'path'=>'Detail.adjudication',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Insurer added line items
          # The third-tier service adjudications for payor added services.
          'subDetail' => {
            'type'=>'ExplanationOfBenefit::AddItem::Detail::SubDetail',
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
              'type'=>'string',
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
            # For example, in Oral whether the treatment is cosmetic or associated with TMJ, or for Medical whether the treatment was outside the clinic or out of office hours.
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
            # Added items adjudication
            # The adjudication results.
            'adjudication' => {
              'type'=>'ExplanationOfBenefit::Item::Adjudication',
              'path'=>'SubDetail.adjudication',
              'min'=>0,
              'max'=>Float::INFINITY
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
          # Billing, service, product, or drug code
          # When the value is a group code then this item collects a set of related claim details, otherwise this contains the product, service, drug or other billing code for the item.
          # If this is an actual service or product line, i.e. not a Group, then use code to indicate the Professional Service or Product supplied (e.g. CTP, HCPCS, USCLS, ICD10, NCPDP, DIN, RxNorm, ACHI, CCI). If a grouping item then use a group code to indicate the type of thing being grouped e.g. 'glasses' or 'compound'.
          attr_accessor :productOrService               # 1-1 CodeableConcept
          ##
          # Service/Product billing modifiers
          # Item typification or modifiers codes to convey additional context for the product or service.
          # For example, in Oral whether the treatment is cosmetic or associated with TMJ, or for Medical whether the treatment was outside the clinic or out of office hours.
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
          # Added items adjudication
          # The adjudication results.
          attr_accessor :adjudication                   # 0-* [ ExplanationOfBenefit::Item::Adjudication ]
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
        # Billing, service, product, or drug code
        # When the value is a group code then this item collects a set of related claim details, otherwise this contains the product, service, drug or other billing code for the item.
        # If this is an actual service or product line, i.e. not a Group, then use code to indicate the Professional Service or Product supplied (e.g. CTP, HCPCS, USCLS, ICD10, NCPDP, DIN, RxNorm, ACHI, CCI). If a grouping item then use a group code to indicate the type of thing being grouped e.g. 'glasses' or 'compound'.
        attr_accessor :productOrService               # 1-1 CodeableConcept
        ##
        # Service/Product billing modifiers
        # Item typification or modifiers codes to convey additional context for the product or service.
        # For example, in Oral whether the treatment is cosmetic or associated with TMJ, or for Medical whether the treatment was outside the clinic or out of office hours.
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
        # Added items adjudication
        # The adjudication results.
        attr_accessor :adjudication                   # 0-* [ ExplanationOfBenefit::Item::Adjudication ]
        ##
        # Insurer added line items
        # The third-tier service adjudications for payor added services.
        attr_accessor :subDetail                      # 0-* [ ExplanationOfBenefit::AddItem::Detail::SubDetail ]
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
      # Item sequence number
      # Claim items which this service line is intended to replace.
      attr_accessor :itemSequence                   # 0-* [ positiveInt ]
      ##
      # Detail sequence number
      # The sequence number of the details within the claim item which this line is intended to replace.
      attr_accessor :detailSequence                 # 0-* [ positiveInt ]
      ##
      # Subdetail sequence number
      # The sequence number of the sub-details woithin the details within the claim item which this line is intended to replace.
      attr_accessor :subDetailSequence              # 0-* [ positiveInt ]
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
      # For example, in Oral whether the treatment is cosmetic or associated with TMJ, or for Medical whether the treatment was outside the clinic or out of office hours.
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
      # For example, providing a tooth code allows an insurer to identify a provider performing a filling on a tooth that was previously removed.
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
      attr_accessor :adjudication                   # 0-* [ ExplanationOfBenefit::Item::Adjudication ]
      ##
      # Insurer added line items
      # The second-tier service adjudications for payor added services.
      attr_accessor :detail                         # 0-* [ ExplanationOfBenefit::AddItem::Detail ]
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
          'type'=>'string',
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
        # For example, codes indicating: Co-Pay, deductible, eligible, benefit, tax, etc.
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
      # Type of adjudication information
      # A code to indicate the information type of this adjudication record. Information types may include: the value submitted, maximum values or percentages allowed or payable under the plan, amounts that the patient is responsible for in aggregate or pertaining to this item, amounts paid by other coverages, and the benefit payable for this item.
      # For example, codes indicating: Co-Pay, deductible, eligible, benefit, tax, etc.
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
          'type'=>'string',
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
          'min'=>0,
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
        # Explanation for the variance
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
          'min'=>0,
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
      # Partial or complete payment
      # Whether this represents partial or complete payment of the benefits payable.
      attr_accessor :type                           # 0-1 CodeableConcept
      ##
      # Payment adjustment for non-claim issues
      # Total amount of all adjustments to this payment included in this transaction which are not related to this claim's adjudication.
      # Insurers will deduct amounts owing from the provider (adjustment), such as a prior overpayment, from the amount owing to the provider (benefits payable) when payment is made to the provider.
      attr_accessor :adjustment                     # 0-1 Money
      ##
      # Explanation for the variance
      # Reason for the payment adjustment.
      attr_accessor :adjustmentReason               # 0-1 CodeableConcept
      ##
      # Expected date of payment
      # Estimated date the payment will be issued or the actual issue date of payment.
      attr_accessor :date                           # 0-1 date
      ##
      # Payable amount after adjustment
      # Benefits payable less any payment adjustment.
      attr_accessor :amount                         # 0-1 Money
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
          'type'=>'string',
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
          'min'=>0,
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
      attr_accessor :text                           # 0-1 string
      ##
      # Language of the text
      # A code to define the language used in the text of the note.
      # Only required if the language is different from the resource language.
      attr_accessor :language                       # 0-1 CodeableConcept
    end

    ##
    # Balance by Benefit Category.
    class BenefitBalance < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'BenefitBalance.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'BenefitBalance.extension',
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
          'path'=>'BenefitBalance.modifierExtension',
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
          'path'=>'BenefitBalance.category',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/ex-benefitcategory'}
        },
        ##
        # Excluded from the plan
        # True if the indicated class of service is excluded from the plan, missing or False indicates the product or service is included in the coverage.
        'excluded' => {
          'type'=>'boolean',
          'path'=>'BenefitBalance.excluded',
          'min'=>0,
          'max'=>1
        },
        ##
        # Short name for the benefit
        # A short name or tag for the benefit.
        # For example: MED01, or DENT2.
        'name' => {
          'type'=>'string',
          'path'=>'BenefitBalance.name',
          'min'=>0,
          'max'=>1
        },
        ##
        # Description of the benefit or services covered
        # A richer description of the benefit or services covered.
        # For example, 'DENT2 covers 100% of basic, 50% of major but excludes Ortho, Implants and Cosmetic services'.
        'description' => {
          'type'=>'string',
          'path'=>'BenefitBalance.description',
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
          'path'=>'BenefitBalance.network',
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
          'path'=>'BenefitBalance.unit',
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
          'path'=>'BenefitBalance.term',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/benefit-term'}
        },
        ##
        # Benefit Summary
        # Benefits Used to date.
        'financial' => {
          'type'=>'ExplanationOfBenefit::BenefitBalance::Financial',
          'path'=>'BenefitBalance.financial',
          'min'=>0,
          'max'=>Float::INFINITY
        }
      }

      ##
      # Benefit Summary
      # Benefits Used to date.
      class Financial < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        MULTIPLE_TYPES = {
          'allowed[x]' => ['Money', 'string', 'unsignedInt'],
          'used[x]' => ['Money', 'unsignedInt']
        }
        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
            'path'=>'Financial.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Financial.extension',
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
            'path'=>'Financial.modifierExtension',
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
            'path'=>'Financial.type',
            'min'=>1,
            'max'=>1,
            'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/benefit-type'}
          },
          ##
          # Benefits allowed
          # The quantity of the benefit which is permitted under the coverage.
          'allowedMoney' => {
            'type'=>'Money',
            'path'=>'Financial.allowed[x]',
            'min'=>0,
            'max'=>1
          },
          ##
          # Benefits allowed
          # The quantity of the benefit which is permitted under the coverage.
          'allowedString' => {
            'type'=>'String',
            'path'=>'Financial.allowed[x]',
            'min'=>0,
            'max'=>1
          },
          ##
          # Benefits allowed
          # The quantity of the benefit which is permitted under the coverage.
          'allowedUnsignedInt' => {
            'type'=>'UnsignedInt',
            'path'=>'Financial.allowed[x]',
            'min'=>0,
            'max'=>1
          },
          ##
          # Benefits used
          # The quantity of the benefit which have been consumed to date.
          'usedMoney' => {
            'type'=>'Money',
            'path'=>'Financial.used[x]',
            'min'=>0,
            'max'=>1
          }
          ##
          # Benefits used
          # The quantity of the benefit which have been consumed to date.
          'usedUnsignedInt' => {
            'type'=>'UnsignedInt',
            'path'=>'Financial.used[x]',
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
      attr_accessor :category                       # 1-1 CodeableConcept
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
      # For example, 'DENT2 covers 100% of basic, 50% of major but excludes Ortho, Implants and Cosmetic services'.
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
      # Benefits Used to date.
      attr_accessor :financial                      # 0-* [ ExplanationOfBenefit::BenefitBalance::Financial ]
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
    # Business Identifier for the resource
    # A unique identifier assigned to this explanation of benefit.
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
    # This may contain the local bill type codes such as the US UB-04 bill type code.
    attr_accessor :subType                        # 0-1 CodeableConcept
    ##
    # claim | preauthorization | predetermination
    # A code to indicate whether the nature of the request is: to request adjudication of products and services previously rendered; or requesting authorization and adjudication for provision in the future; or requesting the non-binding adjudication of the listed products and services which could be provided in the future.
    attr_accessor :use                            # 1-1 code
    ##
    # The recipient of the products and services
    # The party to whom the professional services and/or products have been supplied or are being considered and for whom actual for forecast reimbursement is sought.
    attr_accessor :patient                        # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient)
    ##
    # Relevant time frame for the claim
    # The period for which charges are being submitted.
    # Typically this would be today or in the past for a claim, and today or in the future for preauthorizations and prodeterminations. Typically line item dates of service should fall within the billing period if one is specified.
    attr_accessor :billablePeriod                 # 0-1 Period
    ##
    # Response creation date
    # The date this resource was created.
    # This field is independent of the date of creation of the resource as it may reflect the creation date of a source document prior to digitization. Typically for claims all services must be completed as of this date.
    attr_accessor :created                        # 1-1 dateTime
    ##
    # Author of the claim
    # Individual who created the claim, predetermination or preauthorization.
    attr_accessor :enterer                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole)
    ##
    # Party responsible for reimbursement
    # The party responsible for authorization, adjudication and reimbursement.
    attr_accessor :insurer                        # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Party responsible for the claim
    # The provider which is responsible for the claim, predetermination or preauthorization.
    # Typically this field would be 1..1 where this party is responsible for the claim but not necessarily professionally responsible for the provision of the individual products and services listed below.
    attr_accessor :provider                       # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Desired processing urgency
    # The provider-required urgency of processing the request. Typical values include: stat, routine deferred.
    # If a claim processor is unable to complete the processing as per the priority then they should generate and error and not process the request.
    attr_accessor :priority                       # 0-1 CodeableConcept
    ##
    # For whom to reserve funds
    # A code to indicate whether and for whom funds are to be reserved for future claims.
    # This field is only used for preauthorizations.
    attr_accessor :fundsReserveRequested          # 0-1 CodeableConcept
    ##
    # Funds reserved status
    # A code, used only on a response to a preauthorization, to indicate whether the benefits payable have been reserved and for whom.
    # Fund would be release by a future claim quoting the preAuthRef of this response. Examples of values include: provider, patient, none.
    attr_accessor :fundsReserve                   # 0-1 CodeableConcept
    ##
    # Prior or corollary claims
    # Other claims which are related to this claim such as prior submissions or claims for related services or for the same event.
    # For example,  for the original treatment and follow-up exams.
    attr_accessor :related                        # 0-* [ ExplanationOfBenefit::Related ]
    ##
    # Prescription authorizing services or products
    # Prescription to support the dispensing of pharmacy, device or vision products.
    attr_accessor :prescription                   # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/MedicationRequest|http://hl7.org/fhir/StructureDefinition/VisionPrescription)
    ##
    # Original prescription if superceded by fulfiller
    # Original prescription which has been superseded by this prescription to support the dispensing of pharmacy services, medications or products.
    # For example, a physician may prescribe a medication which the pharmacy determines is contraindicated, or for which the patient has an intolerance, and therefor issues a new prescription for an alternate medication which has the same therapeutic intent. The prescription from the pharmacy becomes the 'prescription' and that from the physician becomes the 'original prescription'.
    attr_accessor :originalPrescription           # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/MedicationRequest)
    ##
    # Recipient of benefits payable
    # The party to be reimbursed for cost of the products and services according to the terms of the policy.
    # Often providers agree to receive the benefits payable to reduce the near-term costs to the patient. The insurer may decline to pay the provider and may choose to pay the subscriber instead.
    attr_accessor :payee                          # 0-1 ExplanationOfBenefit::Payee
    ##
    # Treatment Referral
    # A reference to a referral resource.
    # The referral resource which lists the date, practitioner, reason and other supporting information.
    attr_accessor :referral                       # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/ServiceRequest)
    ##
    # Servicing Facility
    # Facility where the services were provided.
    attr_accessor :facility                       # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Location)
    ##
    # Claim reference
    # The business identifier for the instance of the adjudication request: claim predetermination or preauthorization.
    attr_accessor :claim                          # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Claim)
    ##
    # Claim response reference
    # The business identifier for the instance of the adjudication response: claim, predetermination or preauthorization response.
    attr_accessor :claimResponse                  # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/ClaimResponse)
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
    attr_accessor :preAuthRef                     # 0-* [ string ]
    ##
    # Preauthorization in-effect period
    # The timeframe during which the supplied preauthorization reference may be quoted on claims to obtain the adjudication as provided.
    # This value is only present on preauthorization adjudications.
    attr_accessor :preAuthRefPeriod               # 0-* [ Period ]
    ##
    # Care Team members
    # The members of the team who provided the products and services.
    attr_accessor :careTeam                       # 0-* [ ExplanationOfBenefit::CareTeam ]
    ##
    # Supporting information
    # Additional information codes regarding exceptions, special considerations, the condition, situation, prior or concurrent issues.
    # Often there are multiple jurisdiction specific valuesets which are required.
    attr_accessor :supportingInfo                 # 0-* [ ExplanationOfBenefit::SupportingInfo ]
    ##
    # Pertinent diagnosis information
    # Information about diagnoses relevant to the claim items.
    attr_accessor :diagnosis                      # 0-* [ ExplanationOfBenefit::Diagnosis ]
    ##
    # Clinical procedures performed
    # Procedures performed on the patient relevant to the billing items with the claim.
    attr_accessor :procedure                      # 0-* [ ExplanationOfBenefit::Procedure ]
    ##
    # Precedence (primary, secondary, etc.)
    # This indicates the relative order of a series of EOBs related to different coverages for the same suite of services.
    attr_accessor :precedence                     # 0-1 positiveInt
    ##
    # Patient insurance information
    # Financial instruments for reimbursement for the health care products and services specified on the claim.
    # All insurance coverages for the patient which may be applicable for reimbursement, of the products and services listed in the claim, are typically provided in the claim to allow insurers to confirm the ordering of the insurance coverages relative to local 'coordination of benefit' rules. One coverage (and only one) with 'focal=true' is to be used in the adjudication of this claim. Coverages appearing before the focal Coverage in the list, and where 'Coverage.subrogation=false', should provide a reference to the ClaimResponse containing the adjudication results of the prior claim.
    attr_accessor :insurance                      # 1-* [ ExplanationOfBenefit::Insurance ]
    ##
    # Details of the event
    # Details of a accident which resulted in injuries which required the products and services listed in the claim.
    attr_accessor :accident                       # 0-1 ExplanationOfBenefit::Accident
    ##
    # Product or service provided
    # A claim line. Either a simple (a product or service) or a 'group' of details which can also be a simple items or groups of sub-details.
    attr_accessor :item                           # 0-* [ ExplanationOfBenefit::Item ]
    ##
    # Insurer added line items
    # The first-tier service adjudications for payor added product or service lines.
    attr_accessor :addItem                        # 0-* [ ExplanationOfBenefit::AddItem ]
    ##
    # Header-level adjudication
    # The adjudication results which are presented at the header level rather than at the line-item or add-item levels.
    attr_accessor :adjudication                   # 0-* [ ExplanationOfBenefit::Item::Adjudication ]
    ##
    # Adjudication totals
    # Categorized monetary totals for the adjudication.
    # Totals for amounts submitted, co-pays, benefits payable etc.
    attr_accessor :total                          # 0-* [ ExplanationOfBenefit::Total ]
    ##
    # Payment Details
    # Payment details for the adjudication of the claim.
    attr_accessor :payment                        # 0-1 ExplanationOfBenefit::Payment
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
    attr_accessor :processNote                    # 0-* [ ExplanationOfBenefit::ProcessNote ]
    ##
    # When the benefits are applicable
    # The term of the benefits documented in this response.
    # Not applicable when use=claim.
    attr_accessor :benefitPeriod                  # 0-1 Period
    ##
    # Balance by Benefit Category.
    attr_accessor :benefitBalance                 # 0-* [ ExplanationOfBenefit::BenefitBalance ]

    def resourceType
      'ExplanationOfBenefit'
    end
  end
end
