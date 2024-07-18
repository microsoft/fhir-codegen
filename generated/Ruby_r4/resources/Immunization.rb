module FHIR

  ##
  # Describes the event of a patient being administered a vaccine or a record of an immunization as reported by a patient, a clinician or another party.
  class Immunization < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['date', 'identifier', 'location', 'lot-number', 'manufacturer', 'patient', 'performer', 'reaction-date', 'reaction', 'reason-code', 'reason-reference', 'series', 'status-reason', 'status', 'target-disease', 'vaccine-code']
    MULTIPLE_TYPES = {
      'occurrence[x]' => ['dateTime', 'string']
    }
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'Immunization.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'Immunization.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'Immunization.implicitRules',
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
        'path'=>'Immunization.language',
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
        'path'=>'Immunization.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'Immunization.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Immunization.extension',
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
        'path'=>'Immunization.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Business identifier
      # A unique identifier assigned to this immunization record.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'Immunization.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # completed | entered-in-error | not-done
      # Indicates the current status of the immunization event.
      # Will generally be set to show that the immunization has been completed or not done.  This element is labeled as a modifier because the status contains codes that mark the resource as not currently valid.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/event-status'=>[ 'completed', 'entered-in-error', 'not-done' ]
        },
        'type'=>'code',
        'path'=>'Immunization.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/immunization-status'}
      },
      ##
      # Reason not done
      # Indicates the reason the immunization event was not performed.
      # This is generally only used for the status of "not-done". The reason for performing the immunization event is captured in reasonCode, not here.
      'statusReason' => {
        'type'=>'CodeableConcept',
        'path'=>'Immunization.statusReason',
        'min'=>0,
        'max'=>1
      },
      ##
      # Vaccine product administered
      # Vaccine that was administered or was to be administered.
      'vaccineCode' => {
        'type'=>'CodeableConcept',
        'path'=>'Immunization.vaccineCode',
        'min'=>1,
        'max'=>1
      },
      ##
      # Who was immunized
      # The patient who either received or did not receive the immunization.
      'patient' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient'],
        'type'=>'Reference',
        'path'=>'Immunization.patient',
        'min'=>1,
        'max'=>1
      },
      ##
      # Encounter immunization was part of
      # The visit or admission or other contact between patient and health care provider the immunization was performed as part of.
      'encounter' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Encounter'],
        'type'=>'Reference',
        'path'=>'Immunization.encounter',
        'min'=>0,
        'max'=>1
      },
      ##
      # Vaccine administration date
      # Date vaccine administered or was to be administered.
      # When immunizations are given a specific date and time should always be known.   When immunizations are patient reported, a specific date might not be known.  Although partial dates are allowed, an adult patient might not be able to recall the year a childhood immunization was given. An exact date is always preferable, but the use of the String data type is acceptable when an exact date is not known. A small number of vaccines (e.g. live oral typhoid vaccine) are given as a series of patient self-administered dose over a span of time. In cases like this, often, only the first dose (typically a provider supervised dose) is recorded with the occurrence indicating the date/time of the first dose.
      'occurrenceDateTime' => {
        'type'=>'DateTime',
        'path'=>'Immunization.occurrence[x]',
        'min'=>1,
        'max'=>1
      },
      ##
      # Vaccine administration date
      # Date vaccine administered or was to be administered.
      # When immunizations are given a specific date and time should always be known.   When immunizations are patient reported, a specific date might not be known.  Although partial dates are allowed, an adult patient might not be able to recall the year a childhood immunization was given. An exact date is always preferable, but the use of the String data type is acceptable when an exact date is not known. A small number of vaccines (e.g. live oral typhoid vaccine) are given as a series of patient self-administered dose over a span of time. In cases like this, often, only the first dose (typically a provider supervised dose) is recorded with the occurrence indicating the date/time of the first dose.
      'occurrenceString' => {
        'type'=>'String',
        'path'=>'Immunization.occurrence[x]',
        'min'=>1,
        'max'=>1
      },
      ##
      # When the immunization was first captured in the subject's record
      # The date the occurrence of the immunization was first captured in the record - potentially significantly after the occurrence of the event.
      'recorded' => {
        'type'=>'dateTime',
        'path'=>'Immunization.recorded',
        'min'=>0,
        'max'=>1
      },
      ##
      # Indicates context the data was recorded in
      # An indication that the content of the record is based on information from the person who administered the vaccine. This reflects the context under which the data was originally recorded.
      # Reflects the “reliability” of the content.
      'primarySource' => {
        'type'=>'boolean',
        'path'=>'Immunization.primarySource',
        'min'=>0,
        'max'=>1
      },
      ##
      # Indicates the source of a secondarily reported record
      # The source of the data when the report of the immunization event is not based on information from the person who administered the vaccine.
      # Should not be populated if primarySource = True, not required even if primarySource = False.
      'reportOrigin' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/immunization-origin'=>[ 'provider', 'record', 'recall', 'school' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Immunization.reportOrigin',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/immunization-origin'}
      },
      ##
      # Where immunization occurred
      # The service delivery location where the vaccine administration occurred.
      'location' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Location'],
        'type'=>'Reference',
        'path'=>'Immunization.location',
        'min'=>0,
        'max'=>1
      },
      ##
      # Vaccine manufacturer
      # Name of vaccine manufacturer.
      'manufacturer' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'Immunization.manufacturer',
        'min'=>0,
        'max'=>1
      },
      ##
      # Vaccine lot number
      # Lot number of the  vaccine product.
      'lotNumber' => {
        'type'=>'string',
        'path'=>'Immunization.lotNumber',
        'min'=>0,
        'max'=>1
      },
      ##
      # Vaccine expiration date
      # Date vaccine batch expires.
      'expirationDate' => {
        'type'=>'date',
        'path'=>'Immunization.expirationDate',
        'min'=>0,
        'max'=>1
      },
      ##
      # Body site vaccine  was administered
      # Body site where vaccine was administered.
      'site' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/v3-ActSite'=>[ 'LA', 'RA' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Immunization.site',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/immunization-site'}
      },
      ##
      # How vaccine entered body
      # The path by which the vaccine product is taken into the body.
      'route' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/v3-RouteOfAdministration'=>[ 'IDINJ', 'IM', 'NASINHLC', 'IVINJ', 'PO', 'SQ', 'TRNSDERM' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Immunization.route',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/immunization-route'}
      },
      ##
      # Amount of vaccine administered
      # The quantity of vaccine product that was administered.
      'doseQuantity' => {
        'type'=>'Quantity',
        'path'=>'Immunization.doseQuantity',
        'min'=>0,
        'max'=>1
      },
      ##
      # Who performed event
      # Indicates who performed the immunization event.
      'performer' => {
        'type'=>'Immunization::Performer',
        'path'=>'Immunization.performer',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional immunization notes
      # Extra information about the immunization that is not conveyed by the other attributes.
      'note' => {
        'type'=>'Annotation',
        'path'=>'Immunization.note',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Why immunization occurred
      # Reasons why the vaccine was administered.
      'reasonCode' => {
        'type'=>'CodeableConcept',
        'path'=>'Immunization.reasonCode',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Why immunization occurred
      # Condition, Observation or DiagnosticReport that supports why the immunization was administered.
      'reasonReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Condition', 'http://hl7.org/fhir/StructureDefinition/Observation', 'http://hl7.org/fhir/StructureDefinition/DiagnosticReport'],
        'type'=>'Reference',
        'path'=>'Immunization.reasonReference',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Dose potency
      # Indication if a dose is considered to be subpotent. By default, a dose should be considered to be potent.
      # Typically, the recognition of the dose being sub-potent is retrospective, after the administration (ex. notification of a manufacturer recall after administration). However, in the case of a partial administration (the patient moves unexpectedly and only some of the dose is actually administered), subpotency may be recognized immediately, but it is still important to record the event.
      'isSubpotent' => {
        'type'=>'boolean',
        'path'=>'Immunization.isSubpotent',
        'min'=>0,
        'max'=>1
      },
      ##
      # Reason for being subpotent
      # Reason why a dose is considered to be subpotent.
      'subpotentReason' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/immunization-subpotent-reason'=>[ 'partial', 'coldchainbreak', 'recall' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Immunization.subpotentReason',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/immunization-subpotent-reason'}
      },
      ##
      # Educational material presented to patient
      # Educational material presented to the patient (or guardian) at the time of vaccine administration.
      'education' => {
        'type'=>'Immunization::Education',
        'path'=>'Immunization.education',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Patient eligibility for a vaccination program
      # Indicates a patient's eligibility for a funding program.
      'programEligibility' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/immunization-program-eligibility'=>[ 'ineligible', 'uninsured' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Immunization.programEligibility',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/immunization-program-eligibility'}
      },
      ##
      # Funding source for the vaccine
      # Indicates the source of the vaccine actually administered. This may be different than the patient eligibility (e.g. the patient may be eligible for a publically purchased vaccine but due to inventory issues, vaccine purchased with private funds was actually administered).
      'fundingSource' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/immunization-funding-source'=>[ 'private', 'public' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Immunization.fundingSource',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/immunization-funding-source'}
      },
      ##
      # Details of a reaction that follows immunization
      # Categorical data indicating that an adverse event is associated in time to an immunization.
      # A reaction may be an indication of an allergy or intolerance and, if this is determined to be the case, it should be recorded as a new AllergyIntolerance resource instance as most systems will not query against past Immunization.reaction elements.
      'reaction' => {
        'type'=>'Immunization::Reaction',
        'path'=>'Immunization.reaction',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Protocol followed by the provider
      # The protocol (set of recommendations) being followed by the provider who administered the dose.
      'protocolApplied' => {
        'type'=>'Immunization::ProtocolApplied',
        'path'=>'Immunization.protocolApplied',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Who performed event
    # Indicates who performed the immunization event.
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
        # What type of performance was done
        # Describes the type of performance (e.g. ordering provider, administering provider, etc.).
        'function' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/v2-0443'=>[ 'OP', 'AP' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Performer.function',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/immunization-function'}
        },
        ##
        # Individual or organization who was performing
        # The practitioner or organization who performed the action.
        # When the individual practitioner who performed the action is known, it is best to send.
        'actor' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization'],
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
      # What type of performance was done
      # Describes the type of performance (e.g. ordering provider, administering provider, etc.).
      attr_accessor :function                       # 0-1 CodeableConcept
      ##
      # Individual or organization who was performing
      # The practitioner or organization who performed the action.
      # When the individual practitioner who performed the action is known, it is best to send.
      attr_accessor :actor                          # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization)
    end

    ##
    # Educational material presented to patient
    # Educational material presented to the patient (or guardian) at the time of vaccine administration.
    class Education < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Education.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Education.extension',
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
          'path'=>'Education.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Educational material document identifier
        # Identifier of the material presented to the patient.
        'documentType' => {
          'type'=>'string',
          'path'=>'Education.documentType',
          'min'=>0,
          'max'=>1
        },
        ##
        # Educational material reference pointer
        # Reference pointer to the educational material given to the patient if the information was on line.
        'reference' => {
          'type'=>'uri',
          'path'=>'Education.reference',
          'min'=>0,
          'max'=>1
        },
        ##
        # Educational material publication date
        # Date the educational material was published.
        'publicationDate' => {
          'type'=>'dateTime',
          'path'=>'Education.publicationDate',
          'min'=>0,
          'max'=>1
        },
        ##
        # Educational material presentation date
        # Date the educational material was given to the patient.
        'presentationDate' => {
          'type'=>'dateTime',
          'path'=>'Education.presentationDate',
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
      # Educational material document identifier
      # Identifier of the material presented to the patient.
      attr_accessor :documentType                   # 0-1 string
      ##
      # Educational material reference pointer
      # Reference pointer to the educational material given to the patient if the information was on line.
      attr_accessor :reference                      # 0-1 uri
      ##
      # Educational material publication date
      # Date the educational material was published.
      attr_accessor :publicationDate                # 0-1 dateTime
      ##
      # Educational material presentation date
      # Date the educational material was given to the patient.
      attr_accessor :presentationDate               # 0-1 dateTime
    end

    ##
    # Details of a reaction that follows immunization
    # Categorical data indicating that an adverse event is associated in time to an immunization.
    # A reaction may be an indication of an allergy or intolerance and, if this is determined to be the case, it should be recorded as a new AllergyIntolerance resource instance as most systems will not query against past Immunization.reaction elements.
    class Reaction < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Reaction.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Reaction.extension',
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
          'path'=>'Reaction.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # When reaction started
        # Date of reaction to the immunization.
        'date' => {
          'type'=>'dateTime',
          'path'=>'Reaction.date',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional information on reaction
        # Details of the reaction.
        'detail' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Observation'],
          'type'=>'Reference',
          'path'=>'Reaction.detail',
          'min'=>0,
          'max'=>1
        },
        ##
        # Indicates self-reported reaction
        # Self-reported indicator.
        'reported' => {
          'type'=>'boolean',
          'path'=>'Reaction.reported',
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
      # When reaction started
      # Date of reaction to the immunization.
      attr_accessor :date                           # 0-1 dateTime
      ##
      # Additional information on reaction
      # Details of the reaction.
      attr_accessor :detail                         # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Observation)
      ##
      # Indicates self-reported reaction
      # Self-reported indicator.
      attr_accessor :reported                       # 0-1 boolean
    end

    ##
    # Protocol followed by the provider
    # The protocol (set of recommendations) being followed by the provider who administered the dose.
    class ProtocolApplied < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'doseNumber[x]' => ['positiveInt', 'string'],
        'seriesDoses[x]' => ['positiveInt', 'string']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'ProtocolApplied.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'ProtocolApplied.extension',
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
          'path'=>'ProtocolApplied.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Name of vaccine series
        # One possible path to achieve presumed immunity against a disease - within the context of an authority.
        'series' => {
          'type'=>'string',
          'path'=>'ProtocolApplied.series',
          'min'=>0,
          'max'=>1
        },
        ##
        # Who is responsible for publishing the recommendations
        # Indicates the authority who published the protocol (e.g. ACIP) that is being followed.
        'authority' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
          'type'=>'Reference',
          'path'=>'ProtocolApplied.authority',
          'min'=>0,
          'max'=>1
        },
        ##
        # Vaccine preventatable disease being targetted
        # The vaccine preventable disease the dose is being administered against.
        'targetDisease' => {
          'type'=>'CodeableConcept',
          'path'=>'ProtocolApplied.targetDisease',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Dose number within series
        # Nominal position in a series.
        # The use of an integer is preferred if known. A string should only be used in cases where an integer is not available (such as when documenting a recurring booster dose).
        'doseNumberPositiveInt' => {
          'type'=>'PositiveInt',
          'path'=>'ProtocolApplied.doseNumber[x]',
          'min'=>1,
          'max'=>1
        },
        ##
        # Dose number within series
        # Nominal position in a series.
        # The use of an integer is preferred if known. A string should only be used in cases where an integer is not available (such as when documenting a recurring booster dose).
        'doseNumberString' => {
          'type'=>'String',
          'path'=>'ProtocolApplied.doseNumber[x]',
          'min'=>1,
          'max'=>1
        },
        ##
        # Recommended number of doses for immunity
        # The recommended number of doses to achieve immunity.
        # The use of an integer is preferred if known. A string should only be used in cases where an integer is not available (such as when documenting a recurring booster dose).
        'seriesDosesPositiveInt' => {
          'type'=>'PositiveInt',
          'path'=>'ProtocolApplied.seriesDoses[x]',
          'min'=>0,
          'max'=>1
        }
        ##
        # Recommended number of doses for immunity
        # The recommended number of doses to achieve immunity.
        # The use of an integer is preferred if known. A string should only be used in cases where an integer is not available (such as when documenting a recurring booster dose).
        'seriesDosesString' => {
          'type'=>'String',
          'path'=>'ProtocolApplied.seriesDoses[x]',
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
      # Name of vaccine series
      # One possible path to achieve presumed immunity against a disease - within the context of an authority.
      attr_accessor :series                         # 0-1 string
      ##
      # Who is responsible for publishing the recommendations
      # Indicates the authority who published the protocol (e.g. ACIP) that is being followed.
      attr_accessor :authority                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
      ##
      # Vaccine preventatable disease being targetted
      # The vaccine preventable disease the dose is being administered against.
      attr_accessor :targetDisease                  # 0-* [ CodeableConcept ]
      ##
      # Dose number within series
      # Nominal position in a series.
      # The use of an integer is preferred if known. A string should only be used in cases where an integer is not available (such as when documenting a recurring booster dose).
      attr_accessor :doseNumberPositiveInt          # 1-1 PositiveInt
      ##
      # Dose number within series
      # Nominal position in a series.
      # The use of an integer is preferred if known. A string should only be used in cases where an integer is not available (such as when documenting a recurring booster dose).
      attr_accessor :doseNumberString               # 1-1 String
      ##
      # Recommended number of doses for immunity
      # The recommended number of doses to achieve immunity.
      # The use of an integer is preferred if known. A string should only be used in cases where an integer is not available (such as when documenting a recurring booster dose).
      attr_accessor :seriesDosesPositiveInt         # 0-1 PositiveInt
      ##
      # Recommended number of doses for immunity
      # The recommended number of doses to achieve immunity.
      # The use of an integer is preferred if known. A string should only be used in cases where an integer is not available (such as when documenting a recurring booster dose).
      attr_accessor :seriesDosesString              # 0-1 String
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
    # Business identifier
    # A unique identifier assigned to this immunization record.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # completed | entered-in-error | not-done
    # Indicates the current status of the immunization event.
    # Will generally be set to show that the immunization has been completed or not done.  This element is labeled as a modifier because the status contains codes that mark the resource as not currently valid.
    attr_accessor :status                         # 1-1 code
    ##
    # Reason not done
    # Indicates the reason the immunization event was not performed.
    # This is generally only used for the status of "not-done". The reason for performing the immunization event is captured in reasonCode, not here.
    attr_accessor :statusReason                   # 0-1 CodeableConcept
    ##
    # Vaccine product administered
    # Vaccine that was administered or was to be administered.
    attr_accessor :vaccineCode                    # 1-1 CodeableConcept
    ##
    # Who was immunized
    # The patient who either received or did not receive the immunization.
    attr_accessor :patient                        # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient)
    ##
    # Encounter immunization was part of
    # The visit or admission or other contact between patient and health care provider the immunization was performed as part of.
    attr_accessor :encounter                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Encounter)
    ##
    # Vaccine administration date
    # Date vaccine administered or was to be administered.
    # When immunizations are given a specific date and time should always be known.   When immunizations are patient reported, a specific date might not be known.  Although partial dates are allowed, an adult patient might not be able to recall the year a childhood immunization was given. An exact date is always preferable, but the use of the String data type is acceptable when an exact date is not known. A small number of vaccines (e.g. live oral typhoid vaccine) are given as a series of patient self-administered dose over a span of time. In cases like this, often, only the first dose (typically a provider supervised dose) is recorded with the occurrence indicating the date/time of the first dose.
    attr_accessor :occurrenceDateTime             # 1-1 DateTime
    ##
    # Vaccine administration date
    # Date vaccine administered or was to be administered.
    # When immunizations are given a specific date and time should always be known.   When immunizations are patient reported, a specific date might not be known.  Although partial dates are allowed, an adult patient might not be able to recall the year a childhood immunization was given. An exact date is always preferable, but the use of the String data type is acceptable when an exact date is not known. A small number of vaccines (e.g. live oral typhoid vaccine) are given as a series of patient self-administered dose over a span of time. In cases like this, often, only the first dose (typically a provider supervised dose) is recorded with the occurrence indicating the date/time of the first dose.
    attr_accessor :occurrenceString               # 1-1 String
    ##
    # When the immunization was first captured in the subject's record
    # The date the occurrence of the immunization was first captured in the record - potentially significantly after the occurrence of the event.
    attr_accessor :recorded                       # 0-1 dateTime
    ##
    # Indicates context the data was recorded in
    # An indication that the content of the record is based on information from the person who administered the vaccine. This reflects the context under which the data was originally recorded.
    # Reflects the “reliability” of the content.
    attr_accessor :primarySource                  # 0-1 boolean
    ##
    # Indicates the source of a secondarily reported record
    # The source of the data when the report of the immunization event is not based on information from the person who administered the vaccine.
    # Should not be populated if primarySource = True, not required even if primarySource = False.
    attr_accessor :reportOrigin                   # 0-1 CodeableConcept
    ##
    # Where immunization occurred
    # The service delivery location where the vaccine administration occurred.
    attr_accessor :location                       # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Location)
    ##
    # Vaccine manufacturer
    # Name of vaccine manufacturer.
    attr_accessor :manufacturer                   # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Vaccine lot number
    # Lot number of the  vaccine product.
    attr_accessor :lotNumber                      # 0-1 string
    ##
    # Vaccine expiration date
    # Date vaccine batch expires.
    attr_accessor :expirationDate                 # 0-1 date
    ##
    # Body site vaccine  was administered
    # Body site where vaccine was administered.
    attr_accessor :site                           # 0-1 CodeableConcept
    ##
    # How vaccine entered body
    # The path by which the vaccine product is taken into the body.
    attr_accessor :route                          # 0-1 CodeableConcept
    ##
    # Amount of vaccine administered
    # The quantity of vaccine product that was administered.
    attr_accessor :doseQuantity                   # 0-1 Quantity
    ##
    # Who performed event
    # Indicates who performed the immunization event.
    attr_accessor :performer                      # 0-* [ Immunization::Performer ]
    ##
    # Additional immunization notes
    # Extra information about the immunization that is not conveyed by the other attributes.
    attr_accessor :note                           # 0-* [ Annotation ]
    ##
    # Why immunization occurred
    # Reasons why the vaccine was administered.
    attr_accessor :reasonCode                     # 0-* [ CodeableConcept ]
    ##
    # Why immunization occurred
    # Condition, Observation or DiagnosticReport that supports why the immunization was administered.
    attr_accessor :reasonReference                # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Condition|http://hl7.org/fhir/StructureDefinition/Observation|http://hl7.org/fhir/StructureDefinition/DiagnosticReport) ]
    ##
    # Dose potency
    # Indication if a dose is considered to be subpotent. By default, a dose should be considered to be potent.
    # Typically, the recognition of the dose being sub-potent is retrospective, after the administration (ex. notification of a manufacturer recall after administration). However, in the case of a partial administration (the patient moves unexpectedly and only some of the dose is actually administered), subpotency may be recognized immediately, but it is still important to record the event.
    attr_accessor :isSubpotent                    # 0-1 boolean
    ##
    # Reason for being subpotent
    # Reason why a dose is considered to be subpotent.
    attr_accessor :subpotentReason                # 0-* [ CodeableConcept ]
    ##
    # Educational material presented to patient
    # Educational material presented to the patient (or guardian) at the time of vaccine administration.
    attr_accessor :education                      # 0-* [ Immunization::Education ]
    ##
    # Patient eligibility for a vaccination program
    # Indicates a patient's eligibility for a funding program.
    attr_accessor :programEligibility             # 0-* [ CodeableConcept ]
    ##
    # Funding source for the vaccine
    # Indicates the source of the vaccine actually administered. This may be different than the patient eligibility (e.g. the patient may be eligible for a publically purchased vaccine but due to inventory issues, vaccine purchased with private funds was actually administered).
    attr_accessor :fundingSource                  # 0-1 CodeableConcept
    ##
    # Details of a reaction that follows immunization
    # Categorical data indicating that an adverse event is associated in time to an immunization.
    # A reaction may be an indication of an allergy or intolerance and, if this is determined to be the case, it should be recorded as a new AllergyIntolerance resource instance as most systems will not query against past Immunization.reaction elements.
    attr_accessor :reaction                       # 0-* [ Immunization::Reaction ]
    ##
    # Protocol followed by the provider
    # The protocol (set of recommendations) being followed by the provider who administered the dose.
    attr_accessor :protocolApplied                # 0-* [ Immunization::ProtocolApplied ]

    def resourceType
      'Immunization'
    end
  end
end
