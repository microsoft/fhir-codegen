module FHIR

  ##
  # A record of a medication that is being consumed by a patient.   A MedicationStatement may indicate that the patient may be taking the medication now or has taken the medication in the past or will be taking the medication in the future.  The source of this information can be the patient, significant other (such as a family member or spouse), or a clinician.  A common scenario where this information is captured is during the history taking process during a patient visit or stay.   The medication information may come from sources such as the patient's memory, from a prescription bottle,  or from a list of medications the patient, clinician or other party maintains. 
  # 
  # The primary difference between a medication statement and a medication administration is that the medication administration has complete administration information and is based on actual administration information from the person who administered the medication.  A medication statement is often, if not always, less specific.  There is no required date/time when the medication was administered, in fact we only know that a source has reported the patient is taking this medication, where details such as time, quantity, or rate or even medication product may be incomplete or missing or less precise.  As stated earlier, the medication statement information may come from the patient's memory, from a prescription bottle or from a list of medications the patient, clinician or other party maintains.  Medication administration is more formal and is not missing detailed information.
  class MedicationStatement < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['category', 'code', 'context', 'effective', 'identifier', 'medication', 'part-of', 'patient', 'source', 'status', 'subject']
    MULTIPLE_TYPES = {
      'medication[x]' => ['CodeableConcept', 'Reference'],
      'effective[x]' => ['dateTime', 'Period']
    }
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'MedicationStatement.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'MedicationStatement.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'MedicationStatement.implicitRules',
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
        'path'=>'MedicationStatement.language',
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
        'path'=>'MedicationStatement.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'MedicationStatement.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'MedicationStatement.extension',
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
        'path'=>'MedicationStatement.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # External identifier
      # Identifiers associated with this Medication Statement that are defined by business processes and/or used to refer to it when a direct URL reference to the resource itself is not appropriate. They are business identifiers assigned to this resource by the performer or other systems and remain constant as the resource is updated and propagates from server to server.
      # This is a business identifier, not a resource identifier.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'MedicationStatement.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Fulfils plan, proposal or order
      # A plan, proposal or order that is fulfilled in whole or in part by this event.
      'basedOn' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/MedicationRequest', 'http://hl7.org/fhir/StructureDefinition/CarePlan', 'http://hl7.org/fhir/StructureDefinition/ServiceRequest'],
        'type'=>'Reference',
        'path'=>'MedicationStatement.basedOn',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Part of referenced event
      # A larger event of which this particular event is a component or step.
      'partOf' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/MedicationAdministration', 'http://hl7.org/fhir/StructureDefinition/MedicationDispense', 'http://hl7.org/fhir/StructureDefinition/MedicationStatement', 'http://hl7.org/fhir/StructureDefinition/Procedure', 'http://hl7.org/fhir/StructureDefinition/Observation'],
        'type'=>'Reference',
        'path'=>'MedicationStatement.partOf',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # active | completed | entered-in-error | intended | stopped | on-hold | unknown | not-taken
      # A code representing the patient or other source's judgment about the state of the medication used that this statement is about.  Generally, this will be active or completed.
      # MedicationStatement is a statement at a point in time.  The status is only representative at the point when it was asserted.  The value set for MedicationStatement.status contains codes that assert the status of the use of the medication by the patient (for example, stopped or on hold) as well as codes that assert the status of the medication statement itself (for example, entered in error).This element is labeled as a modifier because the status contains codes that mark the resource as not currently valid.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/CodeSystem/medication-statement-status'=>[ 'active', 'completed', 'entered-in-error', 'intended', 'stopped', 'on-hold', 'unknown', 'not-taken' ]
        },
        'type'=>'code',
        'path'=>'MedicationStatement.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/medication-statement-status'}
      },
      ##
      # Reason for current status
      # Captures the reason for the current state of the MedicationStatement.
      # This is generally only used for "exception" statuses such as "not-taken", "on-hold", "cancelled" or "entered-in-error". The reason for performing the event at all is captured in reasonCode, not here.
      'statusReason' => {
        'type'=>'CodeableConcept',
        'path'=>'MedicationStatement.statusReason',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Type of medication usage
      # Indicates where the medication is expected to be consumed or administered.
      'category' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/medication-statement-category'=>[ 'inpatient', 'outpatient', 'community', 'patientspecified' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'MedicationStatement.category',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/medication-statement-category'}
      },
      ##
      # What medication was taken
      # Identifies the medication being administered. This is either a link to a resource representing the details of the medication or a simple attribute carrying a code that identifies the medication from a known list of medications.
      # If only a code is specified, then it needs to be a code for a specific product. If more information is required, then the use of the medication resource is recommended.  For example, if you require form or lot number, then you must reference the Medication resource.
      'medicationCodeableConcept' => {
        'type'=>'CodeableConcept',
        'path'=>'MedicationStatement.medication[x]',
        'min'=>1,
        'max'=>1
      },
      ##
      # What medication was taken
      # Identifies the medication being administered. This is either a link to a resource representing the details of the medication or a simple attribute carrying a code that identifies the medication from a known list of medications.
      # If only a code is specified, then it needs to be a code for a specific product. If more information is required, then the use of the medication resource is recommended.  For example, if you require form or lot number, then you must reference the Medication resource.
      'medicationReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Medication'],
        'type'=>'Reference',
        'path'=>'MedicationStatement.medication[x]',
        'min'=>1,
        'max'=>1
      },
      ##
      # Who is/was taking  the medication
      # The person, animal or group who is/was taking the medication.
      'subject' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Group'],
        'type'=>'Reference',
        'path'=>'MedicationStatement.subject',
        'min'=>1,
        'max'=>1
      },
      ##
      # Encounter / Episode associated with MedicationStatement
      # The encounter or episode of care that establishes the context for this MedicationStatement.
      'context' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Encounter', 'http://hl7.org/fhir/StructureDefinition/EpisodeOfCare'],
        'type'=>'Reference',
        'path'=>'MedicationStatement.context',
        'min'=>0,
        'max'=>1
      },
      ##
      # The date/time or interval when the medication is/was/will be taken
      # The interval of time during which it is being asserted that the patient is/was/will be taking the medication (or was not taking, when the MedicationStatement.taken element is No).
      # This attribute reflects the period over which the patient consumed the medication and is expected to be populated on the majority of Medication Statements. If the medication is still being taken at the time the statement is recorded, the "end" date will be omitted.  The date/time attribute supports a variety of dates - year, year/month and exact date.  If something more than this is required, this should be conveyed as text.
      'effectiveDateTime' => {
        'type'=>'DateTime',
        'path'=>'MedicationStatement.effective[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # The date/time or interval when the medication is/was/will be taken
      # The interval of time during which it is being asserted that the patient is/was/will be taking the medication (or was not taking, when the MedicationStatement.taken element is No).
      # This attribute reflects the period over which the patient consumed the medication and is expected to be populated on the majority of Medication Statements. If the medication is still being taken at the time the statement is recorded, the "end" date will be omitted.  The date/time attribute supports a variety of dates - year, year/month and exact date.  If something more than this is required, this should be conveyed as text.
      'effectivePeriod' => {
        'type'=>'Period',
        'path'=>'MedicationStatement.effective[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the statement was asserted?
      # The date when the medication statement was asserted by the information source.
      'dateAsserted' => {
        'type'=>'dateTime',
        'path'=>'MedicationStatement.dateAsserted',
        'min'=>0,
        'max'=>1
      },
      ##
      # Person or organization that provided the information about the taking of this medication
      # The person or organization that provided the information about the taking of this medication. Note: Use derivedFrom when a MedicationStatement is derived from other resources, e.g. Claim or MedicationRequest.
      'informationSource' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'MedicationStatement.informationSource',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional supporting information
      # Allows linking the MedicationStatement to the underlying MedicationRequest, or to other information that supports or is used to derive the MedicationStatement.
      # Likely references would be to MedicationRequest, MedicationDispense, Claim, Observation or QuestionnaireAnswers.  The most common use cases for deriving a MedicationStatement comes from creating a MedicationStatement from a MedicationRequest or from a lab observation or a claim.  it should be noted that the amount of information that is available varies from the type resource that you derive the MedicationStatement from.
      'derivedFrom' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'Reference',
        'path'=>'MedicationStatement.derivedFrom',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Reason for why the medication is being/was taken
      # A reason for why the medication is being/was taken.
      # This could be a diagnosis code. If a full condition record exists or additional detail is needed, use reasonForUseReference.
      'reasonCode' => {
        'type'=>'CodeableConcept',
        'path'=>'MedicationStatement.reasonCode',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Condition or observation that supports why the medication is being/was taken.
      # This is a reference to a condition that is the reason why the medication is being/was taken.  If only a code exists, use reasonForUseCode.
      'reasonReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Condition', 'http://hl7.org/fhir/StructureDefinition/Observation', 'http://hl7.org/fhir/StructureDefinition/DiagnosticReport'],
        'type'=>'Reference',
        'path'=>'MedicationStatement.reasonReference',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Further information about the statement
      # Provides extra information about the medication statement that is not conveyed by the other attributes.
      'note' => {
        'type'=>'Annotation',
        'path'=>'MedicationStatement.note',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Details of how medication is/was taken or should be taken
      # Indicates how the medication is/was or should be taken by the patient.
      # The dates included in the dosage on a Medication Statement reflect the dates for a given dose.  For example, "from November 1, 2016 to November 3, 2016, take one tablet daily and from November 4, 2016 to November 7, 2016, take two tablets daily."  It is expected that this specificity may only be populated where the patient brings in their labeled container or where the Medication Statement is derived from a MedicationRequest.
      'dosage' => {
        'type'=>'Dosage',
        'path'=>'MedicationStatement.dosage',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }
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
    # Identifiers associated with this Medication Statement that are defined by business processes and/or used to refer to it when a direct URL reference to the resource itself is not appropriate. They are business identifiers assigned to this resource by the performer or other systems and remain constant as the resource is updated and propagates from server to server.
    # This is a business identifier, not a resource identifier.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Fulfils plan, proposal or order
    # A plan, proposal or order that is fulfilled in whole or in part by this event.
    attr_accessor :basedOn                        # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/MedicationRequest|http://hl7.org/fhir/StructureDefinition/CarePlan|http://hl7.org/fhir/StructureDefinition/ServiceRequest) ]
    ##
    # Part of referenced event
    # A larger event of which this particular event is a component or step.
    attr_accessor :partOf                         # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/MedicationAdministration|http://hl7.org/fhir/StructureDefinition/MedicationDispense|http://hl7.org/fhir/StructureDefinition/MedicationStatement|http://hl7.org/fhir/StructureDefinition/Procedure|http://hl7.org/fhir/StructureDefinition/Observation) ]
    ##
    # active | completed | entered-in-error | intended | stopped | on-hold | unknown | not-taken
    # A code representing the patient or other source's judgment about the state of the medication used that this statement is about.  Generally, this will be active or completed.
    # MedicationStatement is a statement at a point in time.  The status is only representative at the point when it was asserted.  The value set for MedicationStatement.status contains codes that assert the status of the use of the medication by the patient (for example, stopped or on hold) as well as codes that assert the status of the medication statement itself (for example, entered in error).This element is labeled as a modifier because the status contains codes that mark the resource as not currently valid.
    attr_accessor :status                         # 1-1 code
    ##
    # Reason for current status
    # Captures the reason for the current state of the MedicationStatement.
    # This is generally only used for "exception" statuses such as "not-taken", "on-hold", "cancelled" or "entered-in-error". The reason for performing the event at all is captured in reasonCode, not here.
    attr_accessor :statusReason                   # 0-* [ CodeableConcept ]
    ##
    # Type of medication usage
    # Indicates where the medication is expected to be consumed or administered.
    attr_accessor :category                       # 0-1 CodeableConcept
    ##
    # What medication was taken
    # Identifies the medication being administered. This is either a link to a resource representing the details of the medication or a simple attribute carrying a code that identifies the medication from a known list of medications.
    # If only a code is specified, then it needs to be a code for a specific product. If more information is required, then the use of the medication resource is recommended.  For example, if you require form or lot number, then you must reference the Medication resource.
    attr_accessor :medicationCodeableConcept      # 1-1 CodeableConcept
    ##
    # What medication was taken
    # Identifies the medication being administered. This is either a link to a resource representing the details of the medication or a simple attribute carrying a code that identifies the medication from a known list of medications.
    # If only a code is specified, then it needs to be a code for a specific product. If more information is required, then the use of the medication resource is recommended.  For example, if you require form or lot number, then you must reference the Medication resource.
    attr_accessor :medicationReference            # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Medication)
    ##
    # Who is/was taking  the medication
    # The person, animal or group who is/was taking the medication.
    attr_accessor :subject                        # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Group)
    ##
    # Encounter / Episode associated with MedicationStatement
    # The encounter or episode of care that establishes the context for this MedicationStatement.
    attr_accessor :context                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Encounter|http://hl7.org/fhir/StructureDefinition/EpisodeOfCare)
    ##
    # The date/time or interval when the medication is/was/will be taken
    # The interval of time during which it is being asserted that the patient is/was/will be taking the medication (or was not taking, when the MedicationStatement.taken element is No).
    # This attribute reflects the period over which the patient consumed the medication and is expected to be populated on the majority of Medication Statements. If the medication is still being taken at the time the statement is recorded, the "end" date will be omitted.  The date/time attribute supports a variety of dates - year, year/month and exact date.  If something more than this is required, this should be conveyed as text.
    attr_accessor :effectiveDateTime              # 0-1 DateTime
    ##
    # The date/time or interval when the medication is/was/will be taken
    # The interval of time during which it is being asserted that the patient is/was/will be taking the medication (or was not taking, when the MedicationStatement.taken element is No).
    # This attribute reflects the period over which the patient consumed the medication and is expected to be populated on the majority of Medication Statements. If the medication is still being taken at the time the statement is recorded, the "end" date will be omitted.  The date/time attribute supports a variety of dates - year, year/month and exact date.  If something more than this is required, this should be conveyed as text.
    attr_accessor :effectivePeriod                # 0-1 Period
    ##
    # When the statement was asserted?
    # The date when the medication statement was asserted by the information source.
    attr_accessor :dateAsserted                   # 0-1 dateTime
    ##
    # Person or organization that provided the information about the taking of this medication
    # The person or organization that provided the information about the taking of this medication. Note: Use derivedFrom when a MedicationStatement is derived from other resources, e.g. Claim or MedicationRequest.
    attr_accessor :informationSource              # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Additional supporting information
    # Allows linking the MedicationStatement to the underlying MedicationRequest, or to other information that supports or is used to derive the MedicationStatement.
    # Likely references would be to MedicationRequest, MedicationDispense, Claim, Observation or QuestionnaireAnswers.  The most common use cases for deriving a MedicationStatement comes from creating a MedicationStatement from a MedicationRequest or from a lab observation or a claim.  it should be noted that the amount of information that is available varies from the type resource that you derive the MedicationStatement from.
    attr_accessor :derivedFrom                    # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Resource) ]
    ##
    # Reason for why the medication is being/was taken
    # A reason for why the medication is being/was taken.
    # This could be a diagnosis code. If a full condition record exists or additional detail is needed, use reasonForUseReference.
    attr_accessor :reasonCode                     # 0-* [ CodeableConcept ]
    ##
    # Condition or observation that supports why the medication is being/was taken.
    # This is a reference to a condition that is the reason why the medication is being/was taken.  If only a code exists, use reasonForUseCode.
    attr_accessor :reasonReference                # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Condition|http://hl7.org/fhir/StructureDefinition/Observation|http://hl7.org/fhir/StructureDefinition/DiagnosticReport) ]
    ##
    # Further information about the statement
    # Provides extra information about the medication statement that is not conveyed by the other attributes.
    attr_accessor :note                           # 0-* [ Annotation ]
    ##
    # Details of how medication is/was taken or should be taken
    # Indicates how the medication is/was or should be taken by the patient.
    # The dates included in the dosage on a Medication Statement reflect the dates for a given dose.  For example, "from November 1, 2016 to November 3, 2016, take one tablet daily and from November 4, 2016 to November 7, 2016, take two tablets daily."  It is expected that this specificity may only be populated where the patient brings in their labeled container or where the Medication Statement is derived from a MedicationRequest.
    attr_accessor :dosage                         # 0-* [ Dosage ]

    def resourceType
      'MedicationStatement'
    end
  end
end
