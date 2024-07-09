module FHIR

  ##
  # Indicates that a medication product is to be or has been dispensed for a named person/patient.  This includes a description of the medication product (supply) provided and the instructions for administering the medication.  The medication dispense is the result of a pharmacy system responding to a medication order.
  class MedicationDispense < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['code', 'context', 'destination', 'identifier', 'medication', 'patient', 'performer', 'prescription', 'receiver', 'responsibleparty', 'status', 'subject', 'type', 'whenhandedover', 'whenprepared']
    MULTIPLE_TYPES = {
      'statusReason[x]' => ['CodeableConcept', 'Reference'],
      'medication[x]' => ['CodeableConcept', 'Reference']
    }
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'MedicationDispense.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'MedicationDispense.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'MedicationDispense.implicitRules',
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
        'path'=>'MedicationDispense.language',
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
        'path'=>'MedicationDispense.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'MedicationDispense.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'MedicationDispense.extension',
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
        'path'=>'MedicationDispense.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # External identifier
      # Identifiers associated with this Medication Dispense that are defined by business processes and/or used to refer to it when a direct URL reference to the resource itself is not appropriate. They are business identifiers assigned to this resource by the performer or other systems and remain constant as the resource is updated and propagates from server to server.
      # This is a business identifier, not a resource identifier.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'MedicationDispense.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Event that dispense is part of
      # The procedure that trigger the dispense.
      'partOf' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Procedure'],
        'type'=>'Reference',
        'path'=>'MedicationDispense.partOf',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # preparation | in-progress | cancelled | on-hold | completed | entered-in-error | stopped | declined | unknown
      # A code specifying the state of the set of dispense events.
      # This element is labeled as a modifier because the status contains codes that mark the resource as not currently valid.
      'status' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/medicationdispense-status'=>[ 'preparation', 'in-progress', 'cancelled', 'on-hold', 'completed', 'entered-in-error', 'stopped', 'declined', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'MedicationDispense.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/medicationdispense-status'}
      },
      ##
      # Why a dispense was not performed
      # Indicates the reason why a dispense was not performed.
      'statusReasonCodeableConcept' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/fhir/CodeSystem/medicationdispense-status-reason'=>[ 'frr01', 'frr02', 'frr03', 'frr04', 'frr05', 'frr06', 'altchoice', 'clarif', 'drughigh', 'hospadm', 'labint', 'non-avail', 'preg', 'saig', 'sddi', 'sdupther', 'sintol', 'surg', 'washout', 'outofstock', 'offmarket' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'MedicationDispense.statusReason[x]',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/medicationdispense-status-reason'}
      },
      ##
      # Why a dispense was not performed
      # Indicates the reason why a dispense was not performed.
      'statusReasonReference' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/fhir/CodeSystem/medicationdispense-status-reason'=>[ 'frr01', 'frr02', 'frr03', 'frr04', 'frr05', 'frr06', 'altchoice', 'clarif', 'drughigh', 'hospadm', 'labint', 'non-avail', 'preg', 'saig', 'sddi', 'sdupther', 'sintol', 'surg', 'washout', 'outofstock', 'offmarket' ]
        },
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/DetectedIssue'],
        'type'=>'Reference',
        'path'=>'MedicationDispense.statusReason[x]',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/medicationdispense-status-reason'}
      },
      ##
      # Type of medication dispense
      # Indicates the type of medication dispense (for example, where the medication is expected to be consumed or administered (i.e. inpatient or outpatient)).
      # The category can be used to include where the medication is expected to be consumed or other types of dispenses.  Invariants can be used to bind to different value sets when profiling to bind.
      'category' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/fhir/CodeSystem/medicationdispense-category'=>[ 'inpatient', 'outpatient', 'community', 'discharge' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'MedicationDispense.category',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/medicationdispense-category'}
      },
      ##
      # What medication was supplied
      # Identifies the medication being administered. This is either a link to a resource representing the details of the medication or a simple attribute carrying a code that identifies the medication from a known list of medications.
      # If only a code is specified, then it needs to be a code for a specific product. If more information is required, then the use of the medication resource is recommended.  For example, if you require form or lot number, then you must reference the Medication resource.
      'medicationCodeableConcept' => {
        'type'=>'CodeableConcept',
        'path'=>'MedicationDispense.medication[x]',
        'min'=>1,
        'max'=>1
      },
      ##
      # What medication was supplied
      # Identifies the medication being administered. This is either a link to a resource representing the details of the medication or a simple attribute carrying a code that identifies the medication from a known list of medications.
      # If only a code is specified, then it needs to be a code for a specific product. If more information is required, then the use of the medication resource is recommended.  For example, if you require form or lot number, then you must reference the Medication resource.
      'medicationReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Medication'],
        'type'=>'Reference',
        'path'=>'MedicationDispense.medication[x]',
        'min'=>1,
        'max'=>1
      },
      ##
      # Who the dispense is for
      # A link to a resource representing the person or the group to whom the medication will be given.
      # SubstanceAdministration->subject->Patient.
      'subject' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Group'],
        'type'=>'Reference',
        'path'=>'MedicationDispense.subject',
        'min'=>0,
        'max'=>1
      },
      ##
      # Encounter / Episode associated with event
      # The encounter or episode of care that establishes the context for this event.
      'context' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Encounter', 'http://hl7.org/fhir/StructureDefinition/EpisodeOfCare'],
        'type'=>'Reference',
        'path'=>'MedicationDispense.context',
        'min'=>0,
        'max'=>1
      },
      ##
      # Information that supports the dispensing of the medication
      # Additional information that supports the medication being dispensed.
      'supportingInformation' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'Reference',
        'path'=>'MedicationDispense.supportingInformation',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Who performed event
      # Indicates who or what performed the event.
      'performer' => {
        'type'=>'MedicationDispense::Performer',
        'path'=>'MedicationDispense.performer',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Where the dispense occurred
      # The principal physical location where the dispense was performed.
      'location' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Location'],
        'type'=>'Reference',
        'path'=>'MedicationDispense.location',
        'min'=>0,
        'max'=>1
      },
      ##
      # Medication order that authorizes the dispense
      # Indicates the medication order that is being dispensed against.
      # Maps to basedOn in Event logical model.
      'authorizingPrescription' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/MedicationRequest'],
        'type'=>'Reference',
        'path'=>'MedicationDispense.authorizingPrescription',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Trial fill, partial fill, emergency fill, etc.
      # Indicates the type of dispensing event that is performed. For example, Trial Fill, Completion of Trial, Partial Fill, Emergency Fill, Samples, etc.
      'type' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/v3-ActCode'=>[ 'DF', 'EM', 'SO', 'FF', 'FFC', 'FFP', 'FFSS', 'TF', 'FS', 'MS', 'RF', 'UD', 'RFC', 'RFCS', 'RFF', 'RFFS', 'RFP', 'RFPS', 'RFS', 'TB', 'TBS', 'UDE' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'MedicationDispense.type',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://terminology.hl7.org/ValueSet/v3-ActPharmacySupplyType'}
      },
      ##
      # Amount dispensed
      # The amount of medication that has been dispensed. Includes unit of measure.
      'quantity' => {
        'type'=>'Quantity',
        'path'=>'MedicationDispense.quantity',
        'min'=>0,
        'max'=>1
      },
      ##
      # Amount of medication expressed as a timing amount
      # The amount of medication expressed as a timing amount.
      'daysSupply' => {
        'type'=>'Quantity',
        'path'=>'MedicationDispense.daysSupply',
        'min'=>0,
        'max'=>1
      },
      ##
      # When product was packaged and reviewed
      # The time when the dispensed product was packaged and reviewed.
      'whenPrepared' => {
        'type'=>'dateTime',
        'path'=>'MedicationDispense.whenPrepared',
        'min'=>0,
        'max'=>1
      },
      ##
      # When product was given out
      # The time the dispensed product was provided to the patient or their representative.
      'whenHandedOver' => {
        'type'=>'dateTime',
        'path'=>'MedicationDispense.whenHandedOver',
        'min'=>0,
        'max'=>1
      },
      ##
      # Where the medication was sent
      # Identification of the facility/location where the medication was shipped to, as part of the dispense event.
      'destination' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Location'],
        'type'=>'Reference',
        'path'=>'MedicationDispense.destination',
        'min'=>0,
        'max'=>1
      },
      ##
      # Who collected the medication
      # Identifies the person who picked up the medication.  This will usually be a patient or their caregiver, but some cases exist where it can be a healthcare professional.
      'receiver' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Practitioner'],
        'type'=>'Reference',
        'path'=>'MedicationDispense.receiver',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Information about the dispense
      # Extra information about the dispense that could not be conveyed in the other attributes.
      'note' => {
        'type'=>'Annotation',
        'path'=>'MedicationDispense.note',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # How the medication is to be used by the patient or administered by the caregiver
      # Indicates how the medication is to be used by the patient.
      # When the dose or rate is intended to change over the entire administration period (e.g. Tapering dose prescriptions), multiple instances of dosage instructions will need to be supplied to convey the different doses/rates.The pharmacist reviews the medication order prior to dispense and updates the dosageInstruction based on the actual product being dispensed.
      'dosageInstruction' => {
        'type'=>'Dosage',
        'path'=>'MedicationDispense.dosageInstruction',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Whether a substitution was performed on the dispense
      # Indicates whether or not substitution was made as part of the dispense.  In some cases, substitution will be expected but does not happen, in other cases substitution is not expected but does happen.  This block explains what substitution did or did not happen and why.  If nothing is specified, substitution was not done.
      'substitution' => {
        'type'=>'MedicationDispense::Substitution',
        'path'=>'MedicationDispense.substitution',
        'min'=>0,
        'max'=>1
      },
      ##
      # Clinical issue with action
      # Indicates an actual or potential clinical issue with or between one or more active or proposed clinical actions for a patient; e.g. drug-drug interaction, duplicate therapy, dosage alert etc.
      # This element can include a detected issue that has been identified either by a decision support system or by a clinician and may include information on the steps that were taken to address the issue.
      'detectedIssue' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/DetectedIssue'],
        'type'=>'Reference',
        'path'=>'MedicationDispense.detectedIssue',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # A list of relevant lifecycle events
      # A summary of the events of interest that have occurred, such as when the dispense was verified.
      # This might not include provenances for all versions of the request – only those deemed “relevant” or important. This SHALL NOT include the Provenance associated with this current version of the resource. (If that provenance is deemed to be a “relevant” change, it will need to be added as part of a later update. Until then, it can be queried directly as the Provenance that points to this version using _revinclude All Provenances should have some historical version of this Request as their subject.).
      'eventHistory' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Provenance'],
        'type'=>'Reference',
        'path'=>'MedicationDispense.eventHistory',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Who performed event
    # Indicates who or what performed the event.
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
        # Who performed the dispense and what they did
        # Distinguishes the type of performer in the dispense.  For example, date enterer, packager, final checker.
        'function' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/medicationdispense-performer-function'=>[ 'dataenterer', 'packager', 'checker', 'finalchecker' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Performer.function',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/medicationdispense-performer-function'}
        },
        ##
        # Individual who was performing
        # The device, practitioner, etc. who performed the action.  It should be assumed that the actor is the dispenser of the medication.
        'actor' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson'],
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
      # Who performed the dispense and what they did
      # Distinguishes the type of performer in the dispense.  For example, date enterer, packager, final checker.
      attr_accessor :function                       # 0-1 CodeableConcept
      ##
      # Individual who was performing
      # The device, practitioner, etc. who performed the action.  It should be assumed that the actor is the dispenser of the medication.
      attr_accessor :actor                          # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/RelatedPerson)
    end

    ##
    # Whether a substitution was performed on the dispense
    # Indicates whether or not substitution was made as part of the dispense.  In some cases, substitution will be expected but does not happen, in other cases substitution is not expected but does happen.  This block explains what substitution did or did not happen and why.  If nothing is specified, substitution was not done.
    class Substitution < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

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
        # Whether a substitution was or was not performed on the dispense
        # True if the dispenser dispensed a different drug or product from what was prescribed.
        'wasSubstituted' => {
          'type'=>'boolean',
          'path'=>'Substitution.wasSubstituted',
          'min'=>1,
          'max'=>1
        },
        ##
        # Code signifying whether a different drug was dispensed from what was prescribed
        # A code signifying whether a different drug was dispensed from what was prescribed.
        'type' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/v3-substanceAdminSubstitution'=>[ 'E', 'EC', 'BC', 'G', 'TE', 'TB', 'TG', 'F', 'N' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Substitution.type',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://terminology.hl7.org/ValueSet/v3-ActSubstanceAdminSubstitutionCode'}
        },
        ##
        # Why was substitution made
        # Indicates the reason for the substitution (or lack of substitution) from what was prescribed.
        'reason' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/v3-ActReason'=>[ 'CT', 'FP', 'OS', 'RR' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Substitution.reason',
          'min'=>0,
          'max'=>Float::INFINITY,
          'binding'=>{'strength'=>'example', 'uri'=>'http://terminology.hl7.org/ValueSet/v3-SubstanceAdminSubstitutionReason'}
        },
        ##
        # Who is responsible for the substitution
        # The person or organization that has primary responsibility for the substitution.
        'responsibleParty' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole'],
          'type'=>'Reference',
          'path'=>'Substitution.responsibleParty',
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
      # Whether a substitution was or was not performed on the dispense
      # True if the dispenser dispensed a different drug or product from what was prescribed.
      attr_accessor :wasSubstituted                 # 1-1 boolean
      ##
      # Code signifying whether a different drug was dispensed from what was prescribed
      # A code signifying whether a different drug was dispensed from what was prescribed.
      attr_accessor :type                           # 0-1 CodeableConcept
      ##
      # Why was substitution made
      # Indicates the reason for the substitution (or lack of substitution) from what was prescribed.
      attr_accessor :reason                         # 0-* [ CodeableConcept ]
      ##
      # Who is responsible for the substitution
      # The person or organization that has primary responsibility for the substitution.
      attr_accessor :responsibleParty               # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole) ]
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
    # External identifier
    # Identifiers associated with this Medication Dispense that are defined by business processes and/or used to refer to it when a direct URL reference to the resource itself is not appropriate. They are business identifiers assigned to this resource by the performer or other systems and remain constant as the resource is updated and propagates from server to server.
    # This is a business identifier, not a resource identifier.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Event that dispense is part of
    # The procedure that trigger the dispense.
    attr_accessor :partOf                         # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Procedure) ]
    ##
    # preparation | in-progress | cancelled | on-hold | completed | entered-in-error | stopped | declined | unknown
    # A code specifying the state of the set of dispense events.
    # This element is labeled as a modifier because the status contains codes that mark the resource as not currently valid.
    attr_accessor :status                         # 1-1 code
    ##
    # Why a dispense was not performed
    # Indicates the reason why a dispense was not performed.
    attr_accessor :statusReasonCodeableConcept    # 0-1 CodeableConcept
    ##
    # Why a dispense was not performed
    # Indicates the reason why a dispense was not performed.
    attr_accessor :statusReasonReference          # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/DetectedIssue)
    ##
    # Type of medication dispense
    # Indicates the type of medication dispense (for example, where the medication is expected to be consumed or administered (i.e. inpatient or outpatient)).
    # The category can be used to include where the medication is expected to be consumed or other types of dispenses.  Invariants can be used to bind to different value sets when profiling to bind.
    attr_accessor :category                       # 0-1 CodeableConcept
    ##
    # What medication was supplied
    # Identifies the medication being administered. This is either a link to a resource representing the details of the medication or a simple attribute carrying a code that identifies the medication from a known list of medications.
    # If only a code is specified, then it needs to be a code for a specific product. If more information is required, then the use of the medication resource is recommended.  For example, if you require form or lot number, then you must reference the Medication resource.
    attr_accessor :medicationCodeableConcept      # 1-1 CodeableConcept
    ##
    # What medication was supplied
    # Identifies the medication being administered. This is either a link to a resource representing the details of the medication or a simple attribute carrying a code that identifies the medication from a known list of medications.
    # If only a code is specified, then it needs to be a code for a specific product. If more information is required, then the use of the medication resource is recommended.  For example, if you require form or lot number, then you must reference the Medication resource.
    attr_accessor :medicationReference            # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Medication)
    ##
    # Who the dispense is for
    # A link to a resource representing the person or the group to whom the medication will be given.
    # SubstanceAdministration->subject->Patient.
    attr_accessor :subject                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Group)
    ##
    # Encounter / Episode associated with event
    # The encounter or episode of care that establishes the context for this event.
    attr_accessor :context                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Encounter|http://hl7.org/fhir/StructureDefinition/EpisodeOfCare)
    ##
    # Information that supports the dispensing of the medication
    # Additional information that supports the medication being dispensed.
    attr_accessor :supportingInformation          # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Resource) ]
    ##
    # Who performed event
    # Indicates who or what performed the event.
    attr_accessor :performer                      # 0-* [ MedicationDispense::Performer ]
    ##
    # Where the dispense occurred
    # The principal physical location where the dispense was performed.
    attr_accessor :location                       # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Location)
    ##
    # Medication order that authorizes the dispense
    # Indicates the medication order that is being dispensed against.
    # Maps to basedOn in Event logical model.
    attr_accessor :authorizingPrescription        # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/MedicationRequest) ]
    ##
    # Trial fill, partial fill, emergency fill, etc.
    # Indicates the type of dispensing event that is performed. For example, Trial Fill, Completion of Trial, Partial Fill, Emergency Fill, Samples, etc.
    attr_accessor :type                           # 0-1 CodeableConcept
    ##
    # Amount dispensed
    # The amount of medication that has been dispensed. Includes unit of measure.
    attr_accessor :quantity                       # 0-1 Quantity
    ##
    # Amount of medication expressed as a timing amount
    # The amount of medication expressed as a timing amount.
    attr_accessor :daysSupply                     # 0-1 Quantity
    ##
    # When product was packaged and reviewed
    # The time when the dispensed product was packaged and reviewed.
    attr_accessor :whenPrepared                   # 0-1 dateTime
    ##
    # When product was given out
    # The time the dispensed product was provided to the patient or their representative.
    attr_accessor :whenHandedOver                 # 0-1 dateTime
    ##
    # Where the medication was sent
    # Identification of the facility/location where the medication was shipped to, as part of the dispense event.
    attr_accessor :destination                    # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Location)
    ##
    # Who collected the medication
    # Identifies the person who picked up the medication.  This will usually be a patient or their caregiver, but some cases exist where it can be a healthcare professional.
    attr_accessor :receiver                       # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Practitioner) ]
    ##
    # Information about the dispense
    # Extra information about the dispense that could not be conveyed in the other attributes.
    attr_accessor :note                           # 0-* [ Annotation ]
    ##
    # How the medication is to be used by the patient or administered by the caregiver
    # Indicates how the medication is to be used by the patient.
    # When the dose or rate is intended to change over the entire administration period (e.g. Tapering dose prescriptions), multiple instances of dosage instructions will need to be supplied to convey the different doses/rates.The pharmacist reviews the medication order prior to dispense and updates the dosageInstruction based on the actual product being dispensed.
    attr_accessor :dosageInstruction              # 0-* [ Dosage ]
    ##
    # Whether a substitution was performed on the dispense
    # Indicates whether or not substitution was made as part of the dispense.  In some cases, substitution will be expected but does not happen, in other cases substitution is not expected but does happen.  This block explains what substitution did or did not happen and why.  If nothing is specified, substitution was not done.
    attr_accessor :substitution                   # 0-1 MedicationDispense::Substitution
    ##
    # Clinical issue with action
    # Indicates an actual or potential clinical issue with or between one or more active or proposed clinical actions for a patient; e.g. drug-drug interaction, duplicate therapy, dosage alert etc.
    # This element can include a detected issue that has been identified either by a decision support system or by a clinician and may include information on the steps that were taken to address the issue.
    attr_accessor :detectedIssue                  # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/DetectedIssue) ]
    ##
    # A list of relevant lifecycle events
    # A summary of the events of interest that have occurred, such as when the dispense was verified.
    # This might not include provenances for all versions of the request – only those deemed “relevant” or important. This SHALL NOT include the Provenance associated with this current version of the resource. (If that provenance is deemed to be a “relevant” change, it will need to be added as part of a later update. Until then, it can be queried directly as the Provenance that points to this version using _revinclude All Provenances should have some historical version of this Request as their subject.).
    attr_accessor :eventHistory                   # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Provenance) ]

    def resourceType
      'MedicationDispense'
    end
  end
end
