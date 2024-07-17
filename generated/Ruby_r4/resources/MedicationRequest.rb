module FHIR

  ##
  # An order or request for both supply of the medication and the instructions for administration of the medication to a patient. The resource is called "MedicationRequest" rather than "MedicationPrescription" or "MedicationOrder" to generalize the use across inpatient and outpatient settings, including care plans, etc., and to harmonize with workflow patterns.
  class MedicationRequest < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['authoredon', 'category', 'code', 'date', 'encounter', 'identifier', 'intended-dispenser', 'intended-performer', 'intended-performertype', 'intent', 'medication', 'patient', 'priority', 'requester', 'status', 'subject']
    MULTIPLE_TYPES = {
      'reported[x]' => ['boolean', 'Reference'],
      'medication[x]' => ['CodeableConcept', 'Reference']
    }
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'MedicationRequest.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'MedicationRequest.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'MedicationRequest.implicitRules',
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
        'path'=>'MedicationRequest.language',
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
        'path'=>'MedicationRequest.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'MedicationRequest.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'MedicationRequest.extension',
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
        'path'=>'MedicationRequest.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # External ids for this request
      # Identifiers associated with this medication request that are defined by business processes and/or used to refer to it when a direct URL reference to the resource itself is not appropriate. They are business identifiers assigned to this resource by the performer or other systems and remain constant as the resource is updated and propagates from server to server.
      # This is a business identifier, not a resource identifier.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'MedicationRequest.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # active | on-hold | cancelled | completed | entered-in-error | stopped | draft | unknown
      # A code specifying the current state of the order.  Generally, this will be active or completed state.
      # This element is labeled as a modifier because the status contains codes that mark the resource as not currently valid.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/CodeSystem/medicationrequest-status'=>[ 'active', 'on-hold', 'cancelled', 'completed', 'entered-in-error', 'stopped', 'draft', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'MedicationRequest.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/medicationrequest-status'}
      },
      ##
      # Reason for current status
      # Captures the reason for the current state of the MedicationRequest.
      # This is generally only used for "exception" statuses such as "suspended" or "cancelled". The reason why the MedicationRequest was created at all is captured in reasonCode, not here.
      'statusReason' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/medicationrequest-status-reason'=>[ 'altchoice', 'clarif', 'drughigh', 'hospadm', 'labint', 'non-avail', 'preg', 'salg', 'sddi', 'sdupther', 'sintol', 'surg', 'washout' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'MedicationRequest.statusReason',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/medicationrequest-status-reason'}
      },
      ##
      # proposal | plan | order | original-order | reflex-order | filler-order | instance-order | option
      # Whether the request is a proposal, plan, or an original order.
      # It is expected that the type of requester will be restricted for different stages of a MedicationRequest.  For example, Proposals can be created by a patient, relatedPerson, Practitioner or Device.  Plans can be created by Practitioners, Patients, RelatedPersons and Devices.  Original orders can be created by a Practitioner only.An instance-order is an instantiation of a request or order and may be used to populate Medication Administration Record.This element is labeled as a modifier because the intent alters when and how the resource is actually applicable.
      'intent' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/CodeSystem/medicationrequest-intent'=>[ 'proposal', 'plan', 'order', 'original-order', 'reflex-order', 'filler-order', 'instance-order', 'option' ]
        },
        'type'=>'code',
        'path'=>'MedicationRequest.intent',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/medicationrequest-intent'}
      },
      ##
      # Type of medication usage
      # Indicates the type of medication request (for example, where the medication is expected to be consumed or administered (i.e. inpatient or outpatient)).
      # The category can be used to include where the medication is expected to be consumed or other types of requests.
      'category' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/medicationrequest-category'=>[ 'inpatient', 'outpatient', 'community', 'discharge' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'MedicationRequest.category',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/medicationrequest-category'}
      },
      ##
      # routine | urgent | asap | stat
      # Indicates how quickly the Medication Request should be addressed with respect to other requests.
      'priority' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/request-priority'=>[ 'routine', 'urgent', 'asap', 'stat' ]
        },
        'type'=>'code',
        'path'=>'MedicationRequest.priority',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/request-priority'}
      },
      ##
      # True if request is prohibiting action
      # If true indicates that the provider is asking for the medication request not to occur.
      # If do not perform is not specified, the request is a positive request e.g. "do perform".
      'doNotPerform' => {
        'type'=>'boolean',
        'path'=>'MedicationRequest.doNotPerform',
        'min'=>0,
        'max'=>1
      },
      ##
      # Reported rather than primary record
      # Indicates if this record was captured as a secondary 'reported' record rather than as an original primary source-of-truth record.  It may also indicate the source of the report.
      'reportedBoolean' => {
        'type'=>'Boolean',
        'path'=>'MedicationRequest.reported[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Reported rather than primary record
      # Indicates if this record was captured as a secondary 'reported' record rather than as an original primary source-of-truth record.  It may also indicate the source of the report.
      'reportedReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'MedicationRequest.reported[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Medication to be taken
      # Identifies the medication being requested. This is a link to a resource that represents the medication which may be the details of the medication or simply an attribute carrying a code that identifies the medication from a known list of medications.
      # If only a code is specified, then it needs to be a code for a specific product. If more information is required, then the use of the Medication resource is recommended.  For example, if you require form or lot number or if the medication is compounded or extemporaneously prepared, then you must reference the Medication resource.
      'medicationCodeableConcept' => {
        'type'=>'CodeableConcept',
        'path'=>'MedicationRequest.medication[x]',
        'min'=>1,
        'max'=>1
      },
      ##
      # Medication to be taken
      # Identifies the medication being requested. This is a link to a resource that represents the medication which may be the details of the medication or simply an attribute carrying a code that identifies the medication from a known list of medications.
      # If only a code is specified, then it needs to be a code for a specific product. If more information is required, then the use of the Medication resource is recommended.  For example, if you require form or lot number or if the medication is compounded or extemporaneously prepared, then you must reference the Medication resource.
      'medicationReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Medication'],
        'type'=>'Reference',
        'path'=>'MedicationRequest.medication[x]',
        'min'=>1,
        'max'=>1
      },
      ##
      # Who or group medication request is for
      # A link to a resource representing the person or set of individuals to whom the medication will be given.
      # The subject on a medication request is mandatory.  For the secondary use case where the actual subject is not provided, there still must be an anonymized subject specified.
      'subject' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Group'],
        'type'=>'Reference',
        'path'=>'MedicationRequest.subject',
        'min'=>1,
        'max'=>1
      },
      ##
      # Encounter created as part of encounter/admission/stay
      # The Encounter during which this [x] was created or to which the creation of this record is tightly associated.
      # This will typically be the encounter the event occurred within, but some activities may be initiated prior to or after the official completion of an encounter but still be tied to the context of the encounter."    If there is a need to link to episodes of care they will be handled with an extension.
      'encounter' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Encounter'],
        'type'=>'Reference',
        'path'=>'MedicationRequest.encounter',
        'min'=>0,
        'max'=>1
      },
      ##
      # Information to support ordering of the medication
      # Include additional information (for example, patient height and weight) that supports the ordering of the medication.
      'supportingInformation' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'Reference',
        'path'=>'MedicationRequest.supportingInformation',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # When request was initially authored
      # The date (and perhaps time) when the prescription was initially written or authored on.
      'authoredOn' => {
        'type'=>'dateTime',
        'path'=>'MedicationRequest.authoredOn',
        'min'=>0,
        'max'=>1
      },
      ##
      # Who/What requested the Request
      # The individual, organization, or device that initiated the request and has responsibility for its activation.
      'requester' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/Device'],
        'type'=>'Reference',
        'path'=>'MedicationRequest.requester',
        'min'=>0,
        'max'=>1
      },
      ##
      # Intended performer of administration
      # The specified desired performer of the medication treatment (e.g. the performer of the medication administration).
      'performer' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/CareTeam'],
        'type'=>'Reference',
        'path'=>'MedicationRequest.performer',
        'min'=>0,
        'max'=>1
      },
      ##
      # Desired kind of performer of the medication administration
      # Indicates the type of performer of the administration of the medication.
      # If specified without indicating a performer, this indicates that the performer must be of the specified type. If specified with a performer then it indicates the requirements of the performer if the designated performer is not available.
      'performerType' => {
        'type'=>'CodeableConcept',
        'path'=>'MedicationRequest.performerType',
        'min'=>0,
        'max'=>1
      },
      ##
      # Person who entered the request
      # The person who entered the order on behalf of another individual for example in the case of a verbal or a telephone order.
      'recorder' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole'],
        'type'=>'Reference',
        'path'=>'MedicationRequest.recorder',
        'min'=>0,
        'max'=>1
      },
      ##
      # Reason or indication for ordering or not ordering the medication
      # The reason or the indication for ordering or not ordering the medication.
      # This could be a diagnosis code. If a full condition record exists or additional detail is needed, use reasonReference.
      'reasonCode' => {
        'type'=>'CodeableConcept',
        'path'=>'MedicationRequest.reasonCode',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Condition or observation that supports why the prescription is being written
      # Condition or observation that supports why the medication was ordered.
      # This is a reference to a condition or observation that is the reason for the medication order.  If only a code exists, use reasonCode.
      'reasonReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Condition', 'http://hl7.org/fhir/StructureDefinition/Observation'],
        'type'=>'Reference',
        'path'=>'MedicationRequest.reasonReference',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Instantiates FHIR protocol or definition
      # The URL pointing to a protocol, guideline, orderset, or other definition that is adhered to in whole or in part by this MedicationRequest.
      'instantiatesCanonical' => {
        'type'=>'canonical',
        'path'=>'MedicationRequest.instantiatesCanonical',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Instantiates external protocol or definition
      # The URL pointing to an externally maintained protocol, guideline, orderset or other definition that is adhered to in whole or in part by this MedicationRequest.
      'instantiatesUri' => {
        'type'=>'uri',
        'path'=>'MedicationRequest.instantiatesUri',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # What request fulfills
      # A plan or request that is fulfilled in whole or in part by this medication request.
      'basedOn' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/CarePlan', 'http://hl7.org/fhir/StructureDefinition/MedicationRequest', 'http://hl7.org/fhir/StructureDefinition/ServiceRequest', 'http://hl7.org/fhir/StructureDefinition/ImmunizationRecommendation'],
        'type'=>'Reference',
        'path'=>'MedicationRequest.basedOn',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Composite request this is part of
      # A shared identifier common to all requests that were authorized more or less simultaneously by a single author, representing the identifier of the requisition or prescription.
      'groupIdentifier' => {
        'type'=>'Identifier',
        'path'=>'MedicationRequest.groupIdentifier',
        'min'=>0,
        'max'=>1
      },
      ##
      # Overall pattern of medication administration
      # The description of the overall patte3rn of the administration of the medication to the patient.
      # This attribute should not be confused with the protocol of the medication.
      'courseOfTherapyType' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/medicationrequest-course-of-therapy'=>[ 'continuous', 'acute', 'seasonal' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'MedicationRequest.courseOfTherapyType',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/medicationrequest-course-of-therapy'}
      },
      ##
      # Associated insurance coverage
      # Insurance plans, coverage extensions, pre-authorizations and/or pre-determinations that may be required for delivering the requested service.
      'insurance' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Coverage', 'http://hl7.org/fhir/StructureDefinition/ClaimResponse'],
        'type'=>'Reference',
        'path'=>'MedicationRequest.insurance',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Information about the prescription
      # Extra information about the prescription that could not be conveyed by the other attributes.
      'note' => {
        'type'=>'Annotation',
        'path'=>'MedicationRequest.note',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # How the medication should be taken
      # Indicates how the medication is to be used by the patient.
      # There are examples where a medication request may include the option of an oral dose or an Intravenous or Intramuscular dose.  For example, "Ondansetron 8mg orally or IV twice a day as needed for nausea" or "Compazine® (prochlorperazine) 5-10mg PO or 25mg PR bid prn nausea or vomiting".  In these cases, two medication requests would be created that could be grouped together.  The decision on which dose and route of administration to use is based on the patient's condition at the time the dose is needed.
      'dosageInstruction' => {
        'type'=>'Dosage',
        'path'=>'MedicationRequest.dosageInstruction',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Medication supply authorization
      # Indicates the specific details for the dispense or medication supply part of a medication request (also known as a Medication Prescription or Medication Order).  Note that this information is not always sent with the order.  There may be in some settings (e.g. hospitals) institutional or system support for completing the dispense details in the pharmacy department.
      'dispenseRequest' => {
        'type'=>'MedicationRequest::DispenseRequest',
        'path'=>'MedicationRequest.dispenseRequest',
        'min'=>0,
        'max'=>1
      },
      ##
      # Any restrictions on medication substitution
      # Indicates whether or not substitution can or should be part of the dispense. In some cases, substitution must happen, in other cases substitution must not happen. This block explains the prescriber's intent. If nothing is specified substitution may be done.
      'substitution' => {
        'type'=>'MedicationRequest::Substitution',
        'path'=>'MedicationRequest.substitution',
        'min'=>0,
        'max'=>1
      },
      ##
      # An order/prescription that is being replaced
      # A link to a resource representing an earlier order related order or prescription.
      'priorPrescription' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/MedicationRequest'],
        'type'=>'Reference',
        'path'=>'MedicationRequest.priorPrescription',
        'min'=>0,
        'max'=>1
      },
      ##
      # Clinical Issue with action
      # Indicates an actual or potential clinical issue with or between one or more active or proposed clinical actions for a patient; e.g. Drug-drug interaction, duplicate therapy, dosage alert etc.
      # This element can include a detected issue that has been identified either by a decision support system or by a clinician and may include information on the steps that were taken to address the issue.
      'detectedIssue' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/DetectedIssue'],
        'type'=>'Reference',
        'path'=>'MedicationRequest.detectedIssue',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # A list of events of interest in the lifecycle
      # Links to Provenance records for past versions of this resource or fulfilling request or event resources that identify key state transitions or updates that are likely to be relevant to a user looking at the current version of the resource.
      # This might not include provenances for all versions of the request – only those deemed “relevant” or important. This SHALL NOT include the provenance associated with this current version of the resource. (If that provenance is deemed to be a “relevant” change, it will need to be added as part of a later update. Until then, it can be queried directly as the provenance that points to this version using _revinclude All Provenances should have some historical version of this Request as their subject.).
      'eventHistory' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Provenance'],
        'type'=>'Reference',
        'path'=>'MedicationRequest.eventHistory',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Medication supply authorization
    # Indicates the specific details for the dispense or medication supply part of a medication request (also known as a Medication Prescription or Medication Order).  Note that this information is not always sent with the order.  There may be in some settings (e.g. hospitals) institutional or system support for completing the dispense details in the pharmacy department.
    class DispenseRequest < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'DispenseRequest.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'DispenseRequest.extension',
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
          'path'=>'DispenseRequest.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # First fill details
        # Indicates the quantity or duration for the first dispense of the medication.
        # If populating this element, either the quantity or the duration must be included.
        'initialFill' => {
          'type'=>'MedicationRequest::DispenseRequest::InitialFill',
          'path'=>'DispenseRequest.initialFill',
          'min'=>0,
          'max'=>1
        },
        ##
        # Minimum period of time between dispenses
        # The minimum period of time that must occur between dispenses of the medication.
        'dispenseInterval' => {
          'type'=>'Duration',
          'path'=>'DispenseRequest.dispenseInterval',
          'min'=>0,
          'max'=>1
        },
        ##
        # Time period supply is authorized for
        # This indicates the validity period of a prescription (stale dating the Prescription).
        # It reflects the prescribers' perspective for the validity of the prescription. Dispenses must not be made against the prescription outside of this period. The lower-bound of the Dispensing Window signifies the earliest date that the prescription can be filled for the first time. If an upper-bound is not specified then the Prescription is open-ended or will default to a stale-date based on regulations.
        'validityPeriod' => {
          'type'=>'Period',
          'path'=>'DispenseRequest.validityPeriod',
          'min'=>0,
          'max'=>1
        },
        ##
        # Number of refills authorized
        # An integer indicating the number of times, in addition to the original dispense, (aka refills or repeats) that the patient can receive the prescribed medication. Usage Notes: This integer does not include the original order dispense. This means that if an order indicates dispense 30 tablets plus "3 repeats", then the order can be dispensed a total of 4 times and the patient can receive a total of 120 tablets.  A prescriber may explicitly say that zero refills are permitted after the initial dispense.
        # If displaying "number of authorized fills", add 1 to this number.
        'numberOfRepeatsAllowed' => {
          'type'=>'unsignedInt',
          'path'=>'DispenseRequest.numberOfRepeatsAllowed',
          'min'=>0,
          'max'=>1
        },
        ##
        # Amount of medication to supply per dispense
        # The amount that is to be dispensed for one fill.
        'quantity' => {
          'type'=>'Quantity',
          'path'=>'DispenseRequest.quantity',
          'min'=>0,
          'max'=>1
        },
        ##
        # Number of days supply per dispense
        # Identifies the period time over which the supplied product is expected to be used, or the length of time the dispense is expected to last.
        # In some situations, this attribute may be used instead of quantity to identify the amount supplied by how long it is expected to last, rather than the physical quantity issued, e.g. 90 days supply of medication (based on an ordered dosage). When possible, it is always better to specify quantity, as this tends to be more precise. expectedSupplyDuration will always be an estimate that can be influenced by external factors.
        'expectedSupplyDuration' => {
          'type'=>'Duration',
          'path'=>'DispenseRequest.expectedSupplyDuration',
          'min'=>0,
          'max'=>1
        },
        ##
        # Intended dispenser
        # Indicates the intended dispensing Organization specified by the prescriber.
        'performer' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
          'type'=>'Reference',
          'path'=>'DispenseRequest.performer',
          'min'=>0,
          'max'=>1
        }
      }

      ##
      # First fill details
      # Indicates the quantity or duration for the first dispense of the medication.
      # If populating this element, either the quantity or the duration must be included.
      class InitialFill < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
            'path'=>'InitialFill.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'InitialFill.extension',
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
            'path'=>'InitialFill.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # First fill quantity
          # The amount or quantity to provide as part of the first dispense.
          'quantity' => {
            'type'=>'Quantity',
            'path'=>'InitialFill.quantity',
            'min'=>0,
            'max'=>1
          },
          ##
          # First fill duration
          # The length of time that the first dispense is expected to last.
          'duration' => {
            'type'=>'Duration',
            'path'=>'InitialFill.duration',
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
        # First fill quantity
        # The amount or quantity to provide as part of the first dispense.
        attr_accessor :quantity                       # 0-1 Quantity
        ##
        # First fill duration
        # The length of time that the first dispense is expected to last.
        attr_accessor :duration                       # 0-1 Duration
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
      # First fill details
      # Indicates the quantity or duration for the first dispense of the medication.
      # If populating this element, either the quantity or the duration must be included.
      attr_accessor :initialFill                    # 0-1 MedicationRequest::DispenseRequest::InitialFill
      ##
      # Minimum period of time between dispenses
      # The minimum period of time that must occur between dispenses of the medication.
      attr_accessor :dispenseInterval               # 0-1 Duration
      ##
      # Time period supply is authorized for
      # This indicates the validity period of a prescription (stale dating the Prescription).
      # It reflects the prescribers' perspective for the validity of the prescription. Dispenses must not be made against the prescription outside of this period. The lower-bound of the Dispensing Window signifies the earliest date that the prescription can be filled for the first time. If an upper-bound is not specified then the Prescription is open-ended or will default to a stale-date based on regulations.
      attr_accessor :validityPeriod                 # 0-1 Period
      ##
      # Number of refills authorized
      # An integer indicating the number of times, in addition to the original dispense, (aka refills or repeats) that the patient can receive the prescribed medication. Usage Notes: This integer does not include the original order dispense. This means that if an order indicates dispense 30 tablets plus "3 repeats", then the order can be dispensed a total of 4 times and the patient can receive a total of 120 tablets.  A prescriber may explicitly say that zero refills are permitted after the initial dispense.
      # If displaying "number of authorized fills", add 1 to this number.
      attr_accessor :numberOfRepeatsAllowed         # 0-1 unsignedInt
      ##
      # Amount of medication to supply per dispense
      # The amount that is to be dispensed for one fill.
      attr_accessor :quantity                       # 0-1 Quantity
      ##
      # Number of days supply per dispense
      # Identifies the period time over which the supplied product is expected to be used, or the length of time the dispense is expected to last.
      # In some situations, this attribute may be used instead of quantity to identify the amount supplied by how long it is expected to last, rather than the physical quantity issued, e.g. 90 days supply of medication (based on an ordered dosage). When possible, it is always better to specify quantity, as this tends to be more precise. expectedSupplyDuration will always be an estimate that can be influenced by external factors.
      attr_accessor :expectedSupplyDuration         # 0-1 Duration
      ##
      # Intended dispenser
      # Indicates the intended dispensing Organization specified by the prescriber.
      attr_accessor :performer                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
    end

    ##
    # Any restrictions on medication substitution
    # Indicates whether or not substitution can or should be part of the dispense. In some cases, substitution must happen, in other cases substitution must not happen. This block explains the prescriber's intent. If nothing is specified substitution may be done.
    class Substitution < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'allowed[x]' => ['boolean', 'CodeableConcept']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Substitution.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Substitution.extension',
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
          'path'=>'Substitution.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Whether substitution is allowed or not
        # True if the prescriber allows a different drug to be dispensed from what was prescribed.
        # This element is labeled as a modifier because whether substitution is allow or not, it cannot be ignored.
        'allowedBoolean' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/v3-substanceAdminSubstitution'=>[ 'E', 'EC', 'BC', 'G', 'TE', 'TB', 'TG', 'F', 'N' ]
          },
          'type'=>'Boolean',
          'path'=>'Substitution.allowed[x]',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://terminology.hl7.org/ValueSet/v3-ActSubstanceAdminSubstitutionCode'}
        },
        ##
        # Whether substitution is allowed or not
        # True if the prescriber allows a different drug to be dispensed from what was prescribed.
        # This element is labeled as a modifier because whether substitution is allow or not, it cannot be ignored.
        'allowedCodeableConcept' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/v3-substanceAdminSubstitution'=>[ 'E', 'EC', 'BC', 'G', 'TE', 'TB', 'TG', 'F', 'N' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Substitution.allowed[x]',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://terminology.hl7.org/ValueSet/v3-ActSubstanceAdminSubstitutionCode'}
        },
        ##
        # Why should (not) substitution be made
        # Indicates the reason for the substitution, or why substitution must or must not be performed.
        'reason' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/v3-ActReason'=>[ 'CT', 'FP', 'OS', 'RR' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Substitution.reason',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://terminology.hl7.org/ValueSet/v3-SubstanceAdminSubstitutionReason'}
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
      # Whether substitution is allowed or not
      # True if the prescriber allows a different drug to be dispensed from what was prescribed.
      # This element is labeled as a modifier because whether substitution is allow or not, it cannot be ignored.
      attr_accessor :allowedBoolean                 # 1-1 Boolean
      ##
      # Whether substitution is allowed or not
      # True if the prescriber allows a different drug to be dispensed from what was prescribed.
      # This element is labeled as a modifier because whether substitution is allow or not, it cannot be ignored.
      attr_accessor :allowedCodeableConcept         # 1-1 CodeableConcept
      ##
      # Why should (not) substitution be made
      # Indicates the reason for the substitution, or why substitution must or must not be performed.
      attr_accessor :reason                         # 0-1 CodeableConcept
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
    # External ids for this request
    # Identifiers associated with this medication request that are defined by business processes and/or used to refer to it when a direct URL reference to the resource itself is not appropriate. They are business identifiers assigned to this resource by the performer or other systems and remain constant as the resource is updated and propagates from server to server.
    # This is a business identifier, not a resource identifier.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # active | on-hold | cancelled | completed | entered-in-error | stopped | draft | unknown
    # A code specifying the current state of the order.  Generally, this will be active or completed state.
    # This element is labeled as a modifier because the status contains codes that mark the resource as not currently valid.
    attr_accessor :status                         # 1-1 code
    ##
    # Reason for current status
    # Captures the reason for the current state of the MedicationRequest.
    # This is generally only used for "exception" statuses such as "suspended" or "cancelled". The reason why the MedicationRequest was created at all is captured in reasonCode, not here.
    attr_accessor :statusReason                   # 0-1 CodeableConcept
    ##
    # proposal | plan | order | original-order | reflex-order | filler-order | instance-order | option
    # Whether the request is a proposal, plan, or an original order.
    # It is expected that the type of requester will be restricted for different stages of a MedicationRequest.  For example, Proposals can be created by a patient, relatedPerson, Practitioner or Device.  Plans can be created by Practitioners, Patients, RelatedPersons and Devices.  Original orders can be created by a Practitioner only.An instance-order is an instantiation of a request or order and may be used to populate Medication Administration Record.This element is labeled as a modifier because the intent alters when and how the resource is actually applicable.
    attr_accessor :intent                         # 1-1 code
    ##
    # Type of medication usage
    # Indicates the type of medication request (for example, where the medication is expected to be consumed or administered (i.e. inpatient or outpatient)).
    # The category can be used to include where the medication is expected to be consumed or other types of requests.
    attr_accessor :category                       # 0-* [ CodeableConcept ]
    ##
    # routine | urgent | asap | stat
    # Indicates how quickly the Medication Request should be addressed with respect to other requests.
    attr_accessor :priority                       # 0-1 code
    ##
    # True if request is prohibiting action
    # If true indicates that the provider is asking for the medication request not to occur.
    # If do not perform is not specified, the request is a positive request e.g. "do perform".
    attr_accessor :doNotPerform                   # 0-1 boolean
    ##
    # Reported rather than primary record
    # Indicates if this record was captured as a secondary 'reported' record rather than as an original primary source-of-truth record.  It may also indicate the source of the report.
    attr_accessor :reportedBoolean                # 0-1 Boolean
    ##
    # Reported rather than primary record
    # Indicates if this record was captured as a secondary 'reported' record rather than as an original primary source-of-truth record.  It may also indicate the source of the report.
    attr_accessor :reportedReference              # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Medication to be taken
    # Identifies the medication being requested. This is a link to a resource that represents the medication which may be the details of the medication or simply an attribute carrying a code that identifies the medication from a known list of medications.
    # If only a code is specified, then it needs to be a code for a specific product. If more information is required, then the use of the Medication resource is recommended.  For example, if you require form or lot number or if the medication is compounded or extemporaneously prepared, then you must reference the Medication resource.
    attr_accessor :medicationCodeableConcept      # 1-1 CodeableConcept
    ##
    # Medication to be taken
    # Identifies the medication being requested. This is a link to a resource that represents the medication which may be the details of the medication or simply an attribute carrying a code that identifies the medication from a known list of medications.
    # If only a code is specified, then it needs to be a code for a specific product. If more information is required, then the use of the Medication resource is recommended.  For example, if you require form or lot number or if the medication is compounded or extemporaneously prepared, then you must reference the Medication resource.
    attr_accessor :medicationReference            # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Medication)
    ##
    # Who or group medication request is for
    # A link to a resource representing the person or set of individuals to whom the medication will be given.
    # The subject on a medication request is mandatory.  For the secondary use case where the actual subject is not provided, there still must be an anonymized subject specified.
    attr_accessor :subject                        # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Group)
    ##
    # Encounter created as part of encounter/admission/stay
    # The Encounter during which this [x] was created or to which the creation of this record is tightly associated.
    # This will typically be the encounter the event occurred within, but some activities may be initiated prior to or after the official completion of an encounter but still be tied to the context of the encounter."    If there is a need to link to episodes of care they will be handled with an extension.
    attr_accessor :encounter                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Encounter)
    ##
    # Information to support ordering of the medication
    # Include additional information (for example, patient height and weight) that supports the ordering of the medication.
    attr_accessor :supportingInformation          # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Resource) ]
    ##
    # When request was initially authored
    # The date (and perhaps time) when the prescription was initially written or authored on.
    attr_accessor :authoredOn                     # 0-1 dateTime
    ##
    # Who/What requested the Request
    # The individual, organization, or device that initiated the request and has responsibility for its activation.
    attr_accessor :requester                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/Device)
    ##
    # Intended performer of administration
    # The specified desired performer of the medication treatment (e.g. the performer of the medication administration).
    attr_accessor :performer                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/CareTeam)
    ##
    # Desired kind of performer of the medication administration
    # Indicates the type of performer of the administration of the medication.
    # If specified without indicating a performer, this indicates that the performer must be of the specified type. If specified with a performer then it indicates the requirements of the performer if the designated performer is not available.
    attr_accessor :performerType                  # 0-1 CodeableConcept
    ##
    # Person who entered the request
    # The person who entered the order on behalf of another individual for example in the case of a verbal or a telephone order.
    attr_accessor :recorder                       # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole)
    ##
    # Reason or indication for ordering or not ordering the medication
    # The reason or the indication for ordering or not ordering the medication.
    # This could be a diagnosis code. If a full condition record exists or additional detail is needed, use reasonReference.
    attr_accessor :reasonCode                     # 0-* [ CodeableConcept ]
    ##
    # Condition or observation that supports why the prescription is being written
    # Condition or observation that supports why the medication was ordered.
    # This is a reference to a condition or observation that is the reason for the medication order.  If only a code exists, use reasonCode.
    attr_accessor :reasonReference                # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Condition|http://hl7.org/fhir/StructureDefinition/Observation) ]
    ##
    # Instantiates FHIR protocol or definition
    # The URL pointing to a protocol, guideline, orderset, or other definition that is adhered to in whole or in part by this MedicationRequest.
    attr_accessor :instantiatesCanonical          # 0-* [ canonical ]
    ##
    # Instantiates external protocol or definition
    # The URL pointing to an externally maintained protocol, guideline, orderset or other definition that is adhered to in whole or in part by this MedicationRequest.
    attr_accessor :instantiatesUri                # 0-* [ uri ]
    ##
    # What request fulfills
    # A plan or request that is fulfilled in whole or in part by this medication request.
    attr_accessor :basedOn                        # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/CarePlan|http://hl7.org/fhir/StructureDefinition/MedicationRequest|http://hl7.org/fhir/StructureDefinition/ServiceRequest|http://hl7.org/fhir/StructureDefinition/ImmunizationRecommendation) ]
    ##
    # Composite request this is part of
    # A shared identifier common to all requests that were authorized more or less simultaneously by a single author, representing the identifier of the requisition or prescription.
    attr_accessor :groupIdentifier                # 0-1 Identifier
    ##
    # Overall pattern of medication administration
    # The description of the overall patte3rn of the administration of the medication to the patient.
    # This attribute should not be confused with the protocol of the medication.
    attr_accessor :courseOfTherapyType            # 0-1 CodeableConcept
    ##
    # Associated insurance coverage
    # Insurance plans, coverage extensions, pre-authorizations and/or pre-determinations that may be required for delivering the requested service.
    attr_accessor :insurance                      # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Coverage|http://hl7.org/fhir/StructureDefinition/ClaimResponse) ]
    ##
    # Information about the prescription
    # Extra information about the prescription that could not be conveyed by the other attributes.
    attr_accessor :note                           # 0-* [ Annotation ]
    ##
    # How the medication should be taken
    # Indicates how the medication is to be used by the patient.
    # There are examples where a medication request may include the option of an oral dose or an Intravenous or Intramuscular dose.  For example, "Ondansetron 8mg orally or IV twice a day as needed for nausea" or "Compazine® (prochlorperazine) 5-10mg PO or 25mg PR bid prn nausea or vomiting".  In these cases, two medication requests would be created that could be grouped together.  The decision on which dose and route of administration to use is based on the patient's condition at the time the dose is needed.
    attr_accessor :dosageInstruction              # 0-* [ Dosage ]
    ##
    # Medication supply authorization
    # Indicates the specific details for the dispense or medication supply part of a medication request (also known as a Medication Prescription or Medication Order).  Note that this information is not always sent with the order.  There may be in some settings (e.g. hospitals) institutional or system support for completing the dispense details in the pharmacy department.
    attr_accessor :dispenseRequest                # 0-1 MedicationRequest::DispenseRequest
    ##
    # Any restrictions on medication substitution
    # Indicates whether or not substitution can or should be part of the dispense. In some cases, substitution must happen, in other cases substitution must not happen. This block explains the prescriber's intent. If nothing is specified substitution may be done.
    attr_accessor :substitution                   # 0-1 MedicationRequest::Substitution
    ##
    # An order/prescription that is being replaced
    # A link to a resource representing an earlier order related order or prescription.
    attr_accessor :priorPrescription              # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/MedicationRequest)
    ##
    # Clinical Issue with action
    # Indicates an actual or potential clinical issue with or between one or more active or proposed clinical actions for a patient; e.g. Drug-drug interaction, duplicate therapy, dosage alert etc.
    # This element can include a detected issue that has been identified either by a decision support system or by a clinician and may include information on the steps that were taken to address the issue.
    attr_accessor :detectedIssue                  # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/DetectedIssue) ]
    ##
    # A list of events of interest in the lifecycle
    # Links to Provenance records for past versions of this resource or fulfilling request or event resources that identify key state transitions or updates that are likely to be relevant to a user looking at the current version of the resource.
    # This might not include provenances for all versions of the request – only those deemed “relevant” or important. This SHALL NOT include the provenance associated with this current version of the resource. (If that provenance is deemed to be a “relevant” change, it will need to be added as part of a later update. Until then, it can be queried directly as the provenance that points to this version using _revinclude All Provenances should have some historical version of this Request as their subject.).
    attr_accessor :eventHistory                   # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Provenance) ]

    def resourceType
      'MedicationRequest'
    end
  end
end
