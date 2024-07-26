module FHIR

  ##
  # An action that is or was performed on or for a patient. This can be a physical intervention like an operation, or less invasive like long term services, counseling, or hypnotherapy.
  class Procedure < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['based-on', 'category', 'code', 'date', 'encounter', 'identifier', 'instantiates-canonical', 'instantiates-uri', 'location', 'part-of', 'patient', 'performer', 'reason-code', 'reason-reference', 'status', 'subject']
    MULTIPLE_TYPES = {
      'performed[x]' => ['Age', 'dateTime', 'Period', 'Range', 'string']
    }
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'Procedure.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'Procedure.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'Procedure.implicitRules',
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
        'path'=>'Procedure.language',
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
        'path'=>'Procedure.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'Procedure.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Procedure.extension',
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
        'path'=>'Procedure.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # External Identifiers for this procedure
      # Business identifiers assigned to this procedure by the performer or other systems which remain constant as the resource is updated and is propagated from server to server.
      # This is a business identifier, not a resource identifier (see [discussion](resource.html#identifiers)).  It is best practice for the identifier to only appear on a single resource instance, however business practices may occasionally dictate that multiple resource instances with the same identifier can exist - possibly even with different resource types.  For example, multiple Patient and Person resource instances might share the same social insurance number.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'Procedure.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Instantiates FHIR protocol or definition
      # The URL pointing to a FHIR-defined protocol, guideline, order set or other definition that is adhered to in whole or in part by this Procedure.
      'instantiatesCanonical' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/PlanDefinition', 'http://hl7.org/fhir/StructureDefinition/ActivityDefinition', 'http://hl7.org/fhir/StructureDefinition/Measure', 'http://hl7.org/fhir/StructureDefinition/OperationDefinition', 'http://hl7.org/fhir/StructureDefinition/Questionnaire'],
        'type'=>'canonical',
        'path'=>'Procedure.instantiatesCanonical',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Instantiates external protocol or definition
      # The URL pointing to an externally maintained protocol, guideline, order set or other definition that is adhered to in whole or in part by this Procedure.
      # This might be an HTML page, PDF, etc. or could just be a non-resolvable URI identifier.
      'instantiatesUri' => {
        'type'=>'uri',
        'path'=>'Procedure.instantiatesUri',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # A request for this procedure
      # A reference to a resource that contains details of the request for this procedure.
      'basedOn' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/CarePlan', 'http://hl7.org/fhir/StructureDefinition/ServiceRequest'],
        'type'=>'Reference',
        'path'=>'Procedure.basedOn',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Part of referenced event
      # A larger event of which this particular procedure is a component or step.
      # The MedicationAdministration resource has a partOf reference to Procedure, but this is not a circular reference.   For example, the anesthesia MedicationAdministration is part of the surgical Procedure (MedicationAdministration.partOf = Procedure).  For example, the procedure to insert the IV port for an IV medication administration is part of the medication administration (Procedure.partOf = MedicationAdministration).
      'partOf' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Procedure', 'http://hl7.org/fhir/StructureDefinition/Observation', 'http://hl7.org/fhir/StructureDefinition/MedicationAdministration'],
        'type'=>'Reference',
        'path'=>'Procedure.partOf',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # preparation | in-progress | not-done | on-hold | stopped | completed | entered-in-error | unknown
      # A code specifying the state of the procedure. Generally, this will be the in-progress or completed state.
      # The "unknown" code is not to be used to convey other statuses.  The "unknown" code should be used when one of the statuses applies, but the authoring system doesn't know the current state of the procedure.
      # 
      # This element is labeled as a modifier because the status contains codes that mark the resource as not currently valid.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/event-status'=>[ 'preparation', 'in-progress', 'not-done', 'on-hold', 'stopped', 'completed', 'entered-in-error', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'Procedure.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/event-status'}
      },
      ##
      # Reason for current status
      # Captures the reason for the current state of the procedure.
      # This is generally only used for "exception" statuses such as "not-done", "suspended" or "aborted". The reason for performing the event at all is captured in reasonCode, not here.
      'statusReason' => {
        'type'=>'CodeableConcept',
        'path'=>'Procedure.statusReason',
        'min'=>0,
        'max'=>1
      },
      ##
      # Classification of the procedure
      # A code that classifies the procedure for searching, sorting and display purposes (e.g. "Surgical Procedure").
      'category' => {
        'type'=>'CodeableConcept',
        'path'=>'Procedure.category',
        'min'=>0,
        'max'=>1
      },
      ##
      # Identification of the procedure
      # The specific procedure that is performed. Use text if the exact nature of the procedure cannot be coded (e.g. "Laparoscopic Appendectomy").
      'code' => {
        'type'=>'CodeableConcept',
        'path'=>'Procedure.code',
        'min'=>0,
        'max'=>1
      },
      ##
      # Who the procedure was performed on
      # The person, animal or group on which the procedure was performed.
      'subject' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Group'],
        'type'=>'Reference',
        'path'=>'Procedure.subject',
        'min'=>1,
        'max'=>1
      },
      ##
      # Encounter created as part of
      # The Encounter during which this Procedure was created or performed or to which the creation of this record is tightly associated.
      # This will typically be the encounter the event occurred within, but some activities may be initiated prior to or after the official completion of an encounter but still be tied to the context of the encounter.
      'encounter' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Encounter'],
        'type'=>'Reference',
        'path'=>'Procedure.encounter',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the procedure was performed
      # Estimated or actual date, date-time, period, or age when the procedure was performed.  Allows a period to support complex procedures that span more than one date, and also allows for the length of the procedure to be captured.
      # Age is generally used when the patient reports an age at which the procedure was performed. Range is generally used when the patient reports an age range when the procedure was performed, such as sometime between 20-25 years old.  dateTime supports a range of precision due to some procedures being reported as past procedures that might not have millisecond precision while other procedures performed and documented during the encounter might have more precise UTC timestamps with timezone.
      'performedAge' => {
        'type'=>'Age',
        'path'=>'Procedure.performed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the procedure was performed
      # Estimated or actual date, date-time, period, or age when the procedure was performed.  Allows a period to support complex procedures that span more than one date, and also allows for the length of the procedure to be captured.
      # Age is generally used when the patient reports an age at which the procedure was performed. Range is generally used when the patient reports an age range when the procedure was performed, such as sometime between 20-25 years old.  dateTime supports a range of precision due to some procedures being reported as past procedures that might not have millisecond precision while other procedures performed and documented during the encounter might have more precise UTC timestamps with timezone.
      'performedDateTime' => {
        'type'=>'DateTime',
        'path'=>'Procedure.performed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the procedure was performed
      # Estimated or actual date, date-time, period, or age when the procedure was performed.  Allows a period to support complex procedures that span more than one date, and also allows for the length of the procedure to be captured.
      # Age is generally used when the patient reports an age at which the procedure was performed. Range is generally used when the patient reports an age range when the procedure was performed, such as sometime between 20-25 years old.  dateTime supports a range of precision due to some procedures being reported as past procedures that might not have millisecond precision while other procedures performed and documented during the encounter might have more precise UTC timestamps with timezone.
      'performedPeriod' => {
        'type'=>'Period',
        'path'=>'Procedure.performed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the procedure was performed
      # Estimated or actual date, date-time, period, or age when the procedure was performed.  Allows a period to support complex procedures that span more than one date, and also allows for the length of the procedure to be captured.
      # Age is generally used when the patient reports an age at which the procedure was performed. Range is generally used when the patient reports an age range when the procedure was performed, such as sometime between 20-25 years old.  dateTime supports a range of precision due to some procedures being reported as past procedures that might not have millisecond precision while other procedures performed and documented during the encounter might have more precise UTC timestamps with timezone.
      'performedRange' => {
        'type'=>'Range',
        'path'=>'Procedure.performed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the procedure was performed
      # Estimated or actual date, date-time, period, or age when the procedure was performed.  Allows a period to support complex procedures that span more than one date, and also allows for the length of the procedure to be captured.
      # Age is generally used when the patient reports an age at which the procedure was performed. Range is generally used when the patient reports an age range when the procedure was performed, such as sometime between 20-25 years old.  dateTime supports a range of precision due to some procedures being reported as past procedures that might not have millisecond precision while other procedures performed and documented during the encounter might have more precise UTC timestamps with timezone.
      'performedString' => {
        'type'=>'String',
        'path'=>'Procedure.performed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Who recorded the procedure
      # Individual who recorded the record and takes responsibility for its content.
      'recorder' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole'],
        'type'=>'Reference',
        'path'=>'Procedure.recorder',
        'min'=>0,
        'max'=>1
      },
      ##
      # Person who asserts this procedure
      # Individual who is making the procedure statement.
      'asserter' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole'],
        'type'=>'Reference',
        'path'=>'Procedure.asserter',
        'min'=>0,
        'max'=>1
      },
      ##
      # The people who performed the procedure
      # Limited to "real" people rather than equipment.
      'performer' => {
        'type'=>'Procedure::Performer',
        'path'=>'Procedure.performer',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Where the procedure happened
      # The location where the procedure actually happened.  E.g. a newborn at home, a tracheostomy at a restaurant.
      'location' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Location'],
        'type'=>'Reference',
        'path'=>'Procedure.location',
        'min'=>0,
        'max'=>1
      },
      ##
      # Coded reason procedure performed
      # The coded reason why the procedure was performed. This may be a coded entity of some type, or may simply be present as text.
      # Use Procedure.reasonCode when a code sufficiently describes the reason.  Use Procedure.reasonReference when referencing a resource, which allows more information to be conveyed, such as onset date. Procedure.reasonCode and Procedure.reasonReference are not meant to be duplicative.  For a single reason, either Procedure.reasonCode or Procedure.reasonReference can be used.  Procedure.reasonCode may be a summary code, or Procedure.reasonReference may be used to reference a very precise definition of the reason using Condition | Observation | Procedure | DiagnosticReport | DocumentReference.  Both Procedure.reasonCode and Procedure.reasonReference can be used if they are describing different reasons for the procedure.
      'reasonCode' => {
        'type'=>'CodeableConcept',
        'path'=>'Procedure.reasonCode',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The justification that the procedure was performed
      # The justification of why the procedure was performed.
      # It is possible for a procedure to be a reason (such as C-Section) for another procedure (such as an epidural). Other examples include endoscopy for dilatation and biopsy (a combination of diagnostic and therapeutic use). 
      # Use Procedure.reasonCode when a code sufficiently describes the reason.  Use Procedure.reasonReference when referencing a resource, which allows more information to be conveyed, such as onset date. Procedure.reasonCode and Procedure.reasonReference are not meant to be duplicative.  For a single reason, either Procedure.reasonCode or Procedure.reasonReference can be used.  Procedure.reasonCode may be a summary code, or Procedure.reasonReference may be used to reference a very precise definition of the reason using Condition | Observation | Procedure | DiagnosticReport | DocumentReference.  Both Procedure.reasonCode and Procedure.reasonReference can be used if they are describing different reasons for the procedure.
      'reasonReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Condition', 'http://hl7.org/fhir/StructureDefinition/Observation', 'http://hl7.org/fhir/StructureDefinition/Procedure', 'http://hl7.org/fhir/StructureDefinition/DiagnosticReport', 'http://hl7.org/fhir/StructureDefinition/DocumentReference'],
        'type'=>'Reference',
        'path'=>'Procedure.reasonReference',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Target body sites
      # Detailed and structured anatomical location information. Multiple locations are allowed - e.g. multiple punch biopsies of a lesion.
      # If the use case requires attributes from the BodySite resource (e.g. to identify and track separately) then use the standard extension [procedure-targetbodystructure](extension-procedure-targetbodystructure.html).
      'bodySite' => {
        'type'=>'CodeableConcept',
        'path'=>'Procedure.bodySite',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The result of procedure
      # The outcome of the procedure - did it resolve the reasons for the procedure being performed?
      # If outcome contains narrative text only, it can be captured using the CodeableConcept.text.
      'outcome' => {
        'type'=>'CodeableConcept',
        'path'=>'Procedure.outcome',
        'min'=>0,
        'max'=>1
      },
      ##
      # Any report resulting from the procedure
      # This could be a histology result, pathology report, surgical report, etc.
      # There could potentially be multiple reports - e.g. if this was a procedure which took multiple biopsies resulting in a number of anatomical pathology reports.
      'report' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/DiagnosticReport', 'http://hl7.org/fhir/StructureDefinition/DocumentReference', 'http://hl7.org/fhir/StructureDefinition/Composition'],
        'type'=>'Reference',
        'path'=>'Procedure.report',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Complication following the procedure
      # Any complications that occurred during the procedure, or in the immediate post-performance period. These are generally tracked separately from the notes, which will typically describe the procedure itself rather than any 'post procedure' issues.
      # If complications are only expressed by the narrative text, they can be captured using the CodeableConcept.text.
      'complication' => {
        'type'=>'CodeableConcept',
        'path'=>'Procedure.complication',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # A condition that is a result of the procedure
      # Any complications that occurred during the procedure, or in the immediate post-performance period.
      'complicationDetail' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Condition'],
        'type'=>'Reference',
        'path'=>'Procedure.complicationDetail',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Instructions for follow up
      # If the procedure required specific follow up - e.g. removal of sutures. The follow up may be represented as a simple note or could potentially be more complex, in which case the CarePlan resource can be used.
      'followUp' => {
        'type'=>'CodeableConcept',
        'path'=>'Procedure.followUp',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional information about the procedure
      # Any other notes and comments about the procedure.
      'note' => {
        'type'=>'Annotation',
        'path'=>'Procedure.note',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Manipulated, implanted, or removed device
      # A device that is implanted, removed or otherwise manipulated (calibration, battery replacement, fitting a prosthesis, attaching a wound-vac, etc.) as a focal portion of the Procedure.
      'focalDevice' => {
        'type'=>'Procedure::FocalDevice',
        'path'=>'Procedure.focalDevice',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Items used during procedure
      # Identifies medications, devices and any other substance used as part of the procedure.
      # For devices actually implanted or removed, use Procedure.device.
      'usedReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/Medication', 'http://hl7.org/fhir/StructureDefinition/Substance'],
        'type'=>'Reference',
        'path'=>'Procedure.usedReference',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Coded items used during the procedure
      # Identifies coded items that were used as part of the procedure.
      # For devices actually implanted or removed, use Procedure.device.
      'usedCode' => {
        'type'=>'CodeableConcept',
        'path'=>'Procedure.usedCode',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # The people who performed the procedure
    # Limited to "real" people rather than equipment.
    class Performer < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
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
        # Type of performance
        # Distinguishes the type of involvement of the performer in the procedure. For example, surgeon, anaesthetist, endoscopist.
        'function' => {
          'type'=>'CodeableConcept',
          'path'=>'Performer.function',
          'min'=>0,
          'max'=>1
        },
        ##
        # The reference to the practitioner
        # The practitioner who was involved in the procedure.
        'actor' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/Device'],
          'type'=>'Reference',
          'path'=>'Performer.actor',
          'min'=>1,
          'max'=>1
        },
        ##
        # Organization the device or practitioner was acting for
        # The organization the device or practitioner was acting on behalf of.
        'onBehalfOf' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
          'type'=>'Reference',
          'path'=>'Performer.onBehalfOf',
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
      # Type of performance
      # Distinguishes the type of involvement of the performer in the procedure. For example, surgeon, anaesthetist, endoscopist.
      attr_accessor :function                       # 0-1 CodeableConcept
      ##
      # The reference to the practitioner
      # The practitioner who was involved in the procedure.
      attr_accessor :actor                          # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/Device)
      ##
      # Organization the device or practitioner was acting for
      # The organization the device or practitioner was acting on behalf of.
      attr_accessor :onBehalfOf                     # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
    end

    ##
    # Manipulated, implanted, or removed device
    # A device that is implanted, removed or otherwise manipulated (calibration, battery replacement, fitting a prosthesis, attaching a wound-vac, etc.) as a focal portion of the Procedure.
    class FocalDevice < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'FocalDevice.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'FocalDevice.extension',
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
          'path'=>'FocalDevice.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Kind of change to device
        # The kind of change that happened to the device during the procedure.
        'action' => {
          'type'=>'CodeableConcept',
          'path'=>'FocalDevice.action',
          'min'=>0,
          'max'=>1
        },
        ##
        # Device that was changed
        # The device that was manipulated (changed) during the procedure.
        'manipulated' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Device'],
          'type'=>'Reference',
          'path'=>'FocalDevice.manipulated',
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
      # Kind of change to device
      # The kind of change that happened to the device during the procedure.
      attr_accessor :action                         # 0-1 CodeableConcept
      ##
      # Device that was changed
      # The device that was manipulated (changed) during the procedure.
      attr_accessor :manipulated                    # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Device)
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
    # External Identifiers for this procedure
    # Business identifiers assigned to this procedure by the performer or other systems which remain constant as the resource is updated and is propagated from server to server.
    # This is a business identifier, not a resource identifier (see [discussion](resource.html#identifiers)).  It is best practice for the identifier to only appear on a single resource instance, however business practices may occasionally dictate that multiple resource instances with the same identifier can exist - possibly even with different resource types.  For example, multiple Patient and Person resource instances might share the same social insurance number.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Instantiates FHIR protocol or definition
    # The URL pointing to a FHIR-defined protocol, guideline, order set or other definition that is adhered to in whole or in part by this Procedure.
    attr_accessor :instantiatesCanonical          # 0-* [ canonical(http://hl7.org/fhir/StructureDefinition/PlanDefinition|http://hl7.org/fhir/StructureDefinition/ActivityDefinition|http://hl7.org/fhir/StructureDefinition/Measure|http://hl7.org/fhir/StructureDefinition/OperationDefinition|http://hl7.org/fhir/StructureDefinition/Questionnaire) ]
    ##
    # Instantiates external protocol or definition
    # The URL pointing to an externally maintained protocol, guideline, order set or other definition that is adhered to in whole or in part by this Procedure.
    # This might be an HTML page, PDF, etc. or could just be a non-resolvable URI identifier.
    attr_accessor :instantiatesUri                # 0-* [ uri ]
    ##
    # A request for this procedure
    # A reference to a resource that contains details of the request for this procedure.
    attr_accessor :basedOn                        # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/CarePlan|http://hl7.org/fhir/StructureDefinition/ServiceRequest) ]
    ##
    # Part of referenced event
    # A larger event of which this particular procedure is a component or step.
    # The MedicationAdministration resource has a partOf reference to Procedure, but this is not a circular reference.   For example, the anesthesia MedicationAdministration is part of the surgical Procedure (MedicationAdministration.partOf = Procedure).  For example, the procedure to insert the IV port for an IV medication administration is part of the medication administration (Procedure.partOf = MedicationAdministration).
    attr_accessor :partOf                         # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Procedure|http://hl7.org/fhir/StructureDefinition/Observation|http://hl7.org/fhir/StructureDefinition/MedicationAdministration) ]
    ##
    # preparation | in-progress | not-done | on-hold | stopped | completed | entered-in-error | unknown
    # A code specifying the state of the procedure. Generally, this will be the in-progress or completed state.
    # The "unknown" code is not to be used to convey other statuses.  The "unknown" code should be used when one of the statuses applies, but the authoring system doesn't know the current state of the procedure.
    # 
    # This element is labeled as a modifier because the status contains codes that mark the resource as not currently valid.
    attr_accessor :status                         # 1-1 code
    ##
    # Reason for current status
    # Captures the reason for the current state of the procedure.
    # This is generally only used for "exception" statuses such as "not-done", "suspended" or "aborted". The reason for performing the event at all is captured in reasonCode, not here.
    attr_accessor :statusReason                   # 0-1 CodeableConcept
    ##
    # Classification of the procedure
    # A code that classifies the procedure for searching, sorting and display purposes (e.g. "Surgical Procedure").
    attr_accessor :category                       # 0-1 CodeableConcept
    ##
    # Identification of the procedure
    # The specific procedure that is performed. Use text if the exact nature of the procedure cannot be coded (e.g. "Laparoscopic Appendectomy").
    attr_accessor :code                           # 0-1 CodeableConcept
    ##
    # Who the procedure was performed on
    # The person, animal or group on which the procedure was performed.
    attr_accessor :subject                        # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Group)
    ##
    # Encounter created as part of
    # The Encounter during which this Procedure was created or performed or to which the creation of this record is tightly associated.
    # This will typically be the encounter the event occurred within, but some activities may be initiated prior to or after the official completion of an encounter but still be tied to the context of the encounter.
    attr_accessor :encounter                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Encounter)
    ##
    # When the procedure was performed
    # Estimated or actual date, date-time, period, or age when the procedure was performed.  Allows a period to support complex procedures that span more than one date, and also allows for the length of the procedure to be captured.
    # Age is generally used when the patient reports an age at which the procedure was performed. Range is generally used when the patient reports an age range when the procedure was performed, such as sometime between 20-25 years old.  dateTime supports a range of precision due to some procedures being reported as past procedures that might not have millisecond precision while other procedures performed and documented during the encounter might have more precise UTC timestamps with timezone.
    attr_accessor :performedAge                   # 0-1 Age
    ##
    # When the procedure was performed
    # Estimated or actual date, date-time, period, or age when the procedure was performed.  Allows a period to support complex procedures that span more than one date, and also allows for the length of the procedure to be captured.
    # Age is generally used when the patient reports an age at which the procedure was performed. Range is generally used when the patient reports an age range when the procedure was performed, such as sometime between 20-25 years old.  dateTime supports a range of precision due to some procedures being reported as past procedures that might not have millisecond precision while other procedures performed and documented during the encounter might have more precise UTC timestamps with timezone.
    attr_accessor :performedDateTime              # 0-1 DateTime
    ##
    # When the procedure was performed
    # Estimated or actual date, date-time, period, or age when the procedure was performed.  Allows a period to support complex procedures that span more than one date, and also allows for the length of the procedure to be captured.
    # Age is generally used when the patient reports an age at which the procedure was performed. Range is generally used when the patient reports an age range when the procedure was performed, such as sometime between 20-25 years old.  dateTime supports a range of precision due to some procedures being reported as past procedures that might not have millisecond precision while other procedures performed and documented during the encounter might have more precise UTC timestamps with timezone.
    attr_accessor :performedPeriod                # 0-1 Period
    ##
    # When the procedure was performed
    # Estimated or actual date, date-time, period, or age when the procedure was performed.  Allows a period to support complex procedures that span more than one date, and also allows for the length of the procedure to be captured.
    # Age is generally used when the patient reports an age at which the procedure was performed. Range is generally used when the patient reports an age range when the procedure was performed, such as sometime between 20-25 years old.  dateTime supports a range of precision due to some procedures being reported as past procedures that might not have millisecond precision while other procedures performed and documented during the encounter might have more precise UTC timestamps with timezone.
    attr_accessor :performedRange                 # 0-1 Range
    ##
    # When the procedure was performed
    # Estimated or actual date, date-time, period, or age when the procedure was performed.  Allows a period to support complex procedures that span more than one date, and also allows for the length of the procedure to be captured.
    # Age is generally used when the patient reports an age at which the procedure was performed. Range is generally used when the patient reports an age range when the procedure was performed, such as sometime between 20-25 years old.  dateTime supports a range of precision due to some procedures being reported as past procedures that might not have millisecond precision while other procedures performed and documented during the encounter might have more precise UTC timestamps with timezone.
    attr_accessor :performedString                # 0-1 String
    ##
    # Who recorded the procedure
    # Individual who recorded the record and takes responsibility for its content.
    attr_accessor :recorder                       # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole)
    ##
    # Person who asserts this procedure
    # Individual who is making the procedure statement.
    attr_accessor :asserter                       # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole)
    ##
    # The people who performed the procedure
    # Limited to "real" people rather than equipment.
    attr_accessor :performer                      # 0-* [ Procedure::Performer ]
    ##
    # Where the procedure happened
    # The location where the procedure actually happened.  E.g. a newborn at home, a tracheostomy at a restaurant.
    attr_accessor :location                       # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Location)
    ##
    # Coded reason procedure performed
    # The coded reason why the procedure was performed. This may be a coded entity of some type, or may simply be present as text.
    # Use Procedure.reasonCode when a code sufficiently describes the reason.  Use Procedure.reasonReference when referencing a resource, which allows more information to be conveyed, such as onset date. Procedure.reasonCode and Procedure.reasonReference are not meant to be duplicative.  For a single reason, either Procedure.reasonCode or Procedure.reasonReference can be used.  Procedure.reasonCode may be a summary code, or Procedure.reasonReference may be used to reference a very precise definition of the reason using Condition | Observation | Procedure | DiagnosticReport | DocumentReference.  Both Procedure.reasonCode and Procedure.reasonReference can be used if they are describing different reasons for the procedure.
    attr_accessor :reasonCode                     # 0-* [ CodeableConcept ]
    ##
    # The justification that the procedure was performed
    # The justification of why the procedure was performed.
    # It is possible for a procedure to be a reason (such as C-Section) for another procedure (such as an epidural). Other examples include endoscopy for dilatation and biopsy (a combination of diagnostic and therapeutic use). 
    # Use Procedure.reasonCode when a code sufficiently describes the reason.  Use Procedure.reasonReference when referencing a resource, which allows more information to be conveyed, such as onset date. Procedure.reasonCode and Procedure.reasonReference are not meant to be duplicative.  For a single reason, either Procedure.reasonCode or Procedure.reasonReference can be used.  Procedure.reasonCode may be a summary code, or Procedure.reasonReference may be used to reference a very precise definition of the reason using Condition | Observation | Procedure | DiagnosticReport | DocumentReference.  Both Procedure.reasonCode and Procedure.reasonReference can be used if they are describing different reasons for the procedure.
    attr_accessor :reasonReference                # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Condition|http://hl7.org/fhir/StructureDefinition/Observation|http://hl7.org/fhir/StructureDefinition/Procedure|http://hl7.org/fhir/StructureDefinition/DiagnosticReport|http://hl7.org/fhir/StructureDefinition/DocumentReference) ]
    ##
    # Target body sites
    # Detailed and structured anatomical location information. Multiple locations are allowed - e.g. multiple punch biopsies of a lesion.
    # If the use case requires attributes from the BodySite resource (e.g. to identify and track separately) then use the standard extension [procedure-targetbodystructure](extension-procedure-targetbodystructure.html).
    attr_accessor :bodySite                       # 0-* [ CodeableConcept ]
    ##
    # The result of procedure
    # The outcome of the procedure - did it resolve the reasons for the procedure being performed?
    # If outcome contains narrative text only, it can be captured using the CodeableConcept.text.
    attr_accessor :outcome                        # 0-1 CodeableConcept
    ##
    # Any report resulting from the procedure
    # This could be a histology result, pathology report, surgical report, etc.
    # There could potentially be multiple reports - e.g. if this was a procedure which took multiple biopsies resulting in a number of anatomical pathology reports.
    attr_accessor :report                         # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/DiagnosticReport|http://hl7.org/fhir/StructureDefinition/DocumentReference|http://hl7.org/fhir/StructureDefinition/Composition) ]
    ##
    # Complication following the procedure
    # Any complications that occurred during the procedure, or in the immediate post-performance period. These are generally tracked separately from the notes, which will typically describe the procedure itself rather than any 'post procedure' issues.
    # If complications are only expressed by the narrative text, they can be captured using the CodeableConcept.text.
    attr_accessor :complication                   # 0-* [ CodeableConcept ]
    ##
    # A condition that is a result of the procedure
    # Any complications that occurred during the procedure, or in the immediate post-performance period.
    attr_accessor :complicationDetail             # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Condition) ]
    ##
    # Instructions for follow up
    # If the procedure required specific follow up - e.g. removal of sutures. The follow up may be represented as a simple note or could potentially be more complex, in which case the CarePlan resource can be used.
    attr_accessor :followUp                       # 0-* [ CodeableConcept ]
    ##
    # Additional information about the procedure
    # Any other notes and comments about the procedure.
    attr_accessor :note                           # 0-* [ Annotation ]
    ##
    # Manipulated, implanted, or removed device
    # A device that is implanted, removed or otherwise manipulated (calibration, battery replacement, fitting a prosthesis, attaching a wound-vac, etc.) as a focal portion of the Procedure.
    attr_accessor :focalDevice                    # 0-* [ Procedure::FocalDevice ]
    ##
    # Items used during procedure
    # Identifies medications, devices and any other substance used as part of the procedure.
    # For devices actually implanted or removed, use Procedure.device.
    attr_accessor :usedReference                  # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/Medication|http://hl7.org/fhir/StructureDefinition/Substance) ]
    ##
    # Coded items used during the procedure
    # Identifies coded items that were used as part of the procedure.
    # For devices actually implanted or removed, use Procedure.device.
    attr_accessor :usedCode                       # 0-* [ CodeableConcept ]

    def resourceType
      'Procedure'
    end
  end
end
