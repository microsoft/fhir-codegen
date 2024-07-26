module FHIR

  ##
  # Describes the event of a patient consuming or otherwise being administered a medication.  This may be as simple as swallowing a tablet or it may be a long running infusion.  Related resources tie this event to the authorizing prescription, and the specific encounter between patient and health care practitioner.
  class MedicationAdministration < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['code', 'context', 'device', 'effective-time', 'identifier', 'medication', 'patient', 'performer', 'reason-given', 'reason-not-given', 'request', 'status', 'subject']
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
        'type'=>'id',
        'path'=>'MedicationAdministration.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'MedicationAdministration.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'MedicationAdministration.implicitRules',
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
        'path'=>'MedicationAdministration.language',
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
        'path'=>'MedicationAdministration.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'MedicationAdministration.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'MedicationAdministration.extension',
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
        'path'=>'MedicationAdministration.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # External identifier
      # Identifiers associated with this Medication Administration that are defined by business processes and/or used to refer to it when a direct URL reference to the resource itself is not appropriate. They are business identifiers assigned to this resource by the performer or other systems and remain constant as the resource is updated and propagates from server to server.
      # This is a business identifier, not a resource identifier.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'MedicationAdministration.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Instantiates protocol or definition
      # A protocol, guideline, orderset, or other definition that was adhered to in whole or in part by this event.
      'instantiates' => {
        'type'=>'uri',
        'path'=>'MedicationAdministration.instantiates',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Part of referenced event
      # A larger event of which this particular event is a component or step.
      'partOf' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/MedicationAdministration', 'http://hl7.org/fhir/StructureDefinition/Procedure'],
        'type'=>'Reference',
        'path'=>'MedicationAdministration.partOf',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # in-progress | not-done | on-hold | completed | entered-in-error | stopped | unknown
      # Will generally be set to show that the administration has been completed.  For some long running administrations such as infusions, it is possible for an administration to be started but not completed or it may be paused while some other process is under way.
      # This element is labeled as a modifier because the status contains codes that mark the resource as not currently valid.
      'status' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/medication-admin-status'=>[ 'in-progress', 'not-done', 'on-hold', 'completed', 'entered-in-error', 'stopped', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'MedicationAdministration.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/medication-admin-status'}
      },
      ##
      # Reason administration not performed
      # A code indicating why the administration was not performed.
      'statusReason' => {
        'type'=>'CodeableConcept',
        'path'=>'MedicationAdministration.statusReason',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Type of medication usage
      # Indicates where the medication is expected to be consumed or administered.
      'category' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/medication-admin-category'=>[ 'inpatient', 'outpatient', 'community' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'MedicationAdministration.category',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/medication-admin-category'}
      },
      ##
      # What was administered
      # Identifies the medication that was administered. This is either a link to a resource representing the details of the medication or a simple attribute carrying a code that identifies the medication from a known list of medications.
      # If only a code is specified, then it needs to be a code for a specific product. If more information is required, then the use of the medication resource is recommended.  For example, if you require form or lot number, then you must reference the Medication resource.
      'medicationCodeableConcept' => {
        'type'=>'CodeableConcept',
        'path'=>'MedicationAdministration.medication[x]',
        'min'=>1,
        'max'=>1
      },
      ##
      # What was administered
      # Identifies the medication that was administered. This is either a link to a resource representing the details of the medication or a simple attribute carrying a code that identifies the medication from a known list of medications.
      # If only a code is specified, then it needs to be a code for a specific product. If more information is required, then the use of the medication resource is recommended.  For example, if you require form or lot number, then you must reference the Medication resource.
      'medicationReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Medication'],
        'type'=>'Reference',
        'path'=>'MedicationAdministration.medication[x]',
        'min'=>1,
        'max'=>1
      },
      ##
      # Who received medication
      # The person or animal or group receiving the medication.
      'subject' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Group'],
        'type'=>'Reference',
        'path'=>'MedicationAdministration.subject',
        'min'=>1,
        'max'=>1
      },
      ##
      # Encounter or Episode of Care administered as part of
      # The visit, admission, or other contact between patient and health care provider during which the medication administration was performed.
      'context' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Encounter', 'http://hl7.org/fhir/StructureDefinition/EpisodeOfCare'],
        'type'=>'Reference',
        'path'=>'MedicationAdministration.context',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional information to support administration
      # Additional information (for example, patient height and weight) that supports the administration of the medication.
      'supportingInformation' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'Reference',
        'path'=>'MedicationAdministration.supportingInformation',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Start and end time of administration
      # A specific date/time or interval of time during which the administration took place (or did not take place, when the 'notGiven' attribute is true). For many administrations, such as swallowing a tablet the use of dateTime is more appropriate.
      'effectiveDateTime' => {
        'type'=>'DateTime',
        'path'=>'MedicationAdministration.effective[x]',
        'min'=>1,
        'max'=>1
      },
      ##
      # Start and end time of administration
      # A specific date/time or interval of time during which the administration took place (or did not take place, when the 'notGiven' attribute is true). For many administrations, such as swallowing a tablet the use of dateTime is more appropriate.
      'effectivePeriod' => {
        'type'=>'Period',
        'path'=>'MedicationAdministration.effective[x]',
        'min'=>1,
        'max'=>1
      },
      ##
      # Who performed the medication administration and what they did
      # Indicates who or what performed the medication administration and how they were involved.
      'performer' => {
        'type'=>'MedicationAdministration::Performer',
        'path'=>'MedicationAdministration.performer',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Reason administration performed
      # A code indicating why the medication was given.
      'reasonCode' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/reason-medication-given'=>[ 'a', 'b', 'c' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'MedicationAdministration.reasonCode',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/reason-medication-given-codes'}
      },
      ##
      # Condition or observation that supports why the medication was administered.
      # This is a reference to a condition that is the reason for the medication request.  If only a code exists, use reasonCode.
      'reasonReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Condition', 'http://hl7.org/fhir/StructureDefinition/Observation', 'http://hl7.org/fhir/StructureDefinition/DiagnosticReport'],
        'type'=>'Reference',
        'path'=>'MedicationAdministration.reasonReference',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Request administration performed against
      # The original request, instruction or authority to perform the administration.
      # This is a reference to the MedicationRequest  where the intent is either order or instance-order.  It should not reference MedicationRequests where the intent is any other value.
      'request' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/MedicationRequest'],
        'type'=>'Reference',
        'path'=>'MedicationAdministration.request',
        'min'=>0,
        'max'=>1
      },
      ##
      # Device used to administer
      # The device used in administering the medication to the patient.  For example, a particular infusion pump.
      'device' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Device'],
        'type'=>'Reference',
        'path'=>'MedicationAdministration.device',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Information about the administration
      # Extra information about the medication administration that is not conveyed by the other attributes.
      'note' => {
        'type'=>'Annotation',
        'path'=>'MedicationAdministration.note',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Details of how medication was taken
      # Describes the medication dosage information details e.g. dose, rate, site, route, etc.
      'dosage' => {
        'type'=>'MedicationAdministration::Dosage',
        'path'=>'MedicationAdministration.dosage',
        'min'=>0,
        'max'=>1
      },
      ##
      # A list of events of interest in the lifecycle
      # A summary of the events of interest that have occurred, such as when the administration was verified.
      # This might not include provenances for all versions of the request – only those deemed “relevant” or important. This SHALL NOT include the Provenance associated with this current version of the resource. (If that provenance is deemed to be a “relevant” change, it will need to be added as part of a later update. Until then, it can be queried directly as the Provenance that points to this version using _revinclude All Provenances should have some historical version of this Request as their subject.
      'eventHistory' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Provenance'],
        'type'=>'Reference',
        'path'=>'MedicationAdministration.eventHistory',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Who performed the medication administration and what they did
    # Indicates who or what performed the medication administration and how they were involved.
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
        # Distinguishes the type of involvement of the performer in the medication administration.
        'function' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/med-admin-perform-function'=>[ 'performer', 'verifier', 'witness' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Performer.function',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/med-admin-perform-function'}
        },
        ##
        # Who performed the medication administration
        # Indicates who or what performed the medication administration.
        'actor' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/Device'],
          'type'=>'Reference',
          'path'=>'Performer.actor',
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
      # Type of performance
      # Distinguishes the type of involvement of the performer in the medication administration.
      attr_accessor :function                       # 0-1 CodeableConcept
      ##
      # Who performed the medication administration
      # Indicates who or what performed the medication administration.
      attr_accessor :actor                          # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/Device)
    end

    ##
    # Details of how medication was taken
    # Describes the medication dosage information details e.g. dose, rate, site, route, etc.
    class Dosage < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'rate[x]' => ['Quantity', 'Ratio']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Dosage.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Dosage.extension',
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
          'path'=>'Dosage.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Free text dosage instructions e.g. SIG
        # Free text dosage can be used for cases where the dosage administered is too complex to code. When coded dosage is present, the free text dosage may still be present for display to humans.The dosage instructions should reflect the dosage of the medication that was administered.
        'text' => {
          'type'=>'string',
          'path'=>'Dosage.text',
          'min'=>0,
          'max'=>1
        },
        ##
        # Body site administered to
        # A coded specification of the anatomic site where the medication first entered the body.  For example, "left arm".
        # If the use case requires attributes from the BodySite resource (e.g. to identify and track separately) then use the standard extension [bodySite](extension-bodysite.html).  May be a summary code, or a reference to a very precise definition of the location, or both.
        'site' => {
          'type'=>'CodeableConcept',
          'path'=>'Dosage.site',
          'min'=>0,
          'max'=>1
        },
        ##
        # Path of substance into body
        # A code specifying the route or physiological path of administration of a therapeutic agent into or onto the patient.  For example, topical, intravenous, etc.
        'route' => {
          'type'=>'CodeableConcept',
          'path'=>'Dosage.route',
          'min'=>0,
          'max'=>1
        },
        ##
        # How drug was administered
        # A coded value indicating the method by which the medication is intended to be or was introduced into or on the body.  This attribute will most often NOT be populated.  It is most commonly used for injections.  For example, Slow Push, Deep IV.
        # One of the reasons this attribute is not used often, is that the method is often pre-coordinated with the route and/or form of administration.  This means the codes used in route or form may pre-coordinate the method in the route code or the form code.  The implementation decision about what coding system to use for route or form code will determine how frequently the method code will be populated e.g. if route or form code pre-coordinate method code, then this attribute will not be populated often; if there is no pre-coordination then method code may  be used frequently.
        'method' => {
          'local_name'=>'local_method'
          'type'=>'CodeableConcept',
          'path'=>'Dosage.method',
          'min'=>0,
          'max'=>1
        },
        ##
        # Amount of medication per dose
        # The amount of the medication given at one administration event.   Use this value when the administration is essentially an instantaneous event such as a swallowing a tablet or giving an injection.
        # If the administration is not instantaneous (rate is present), this can be specified to convey the total amount administered over period of time of a single administration.
        'dose' => {
          'type'=>'Quantity',
          'path'=>'Dosage.dose',
          'min'=>0,
          'max'=>1
        },
        ##
        # Dose quantity per unit of time
        # Identifies the speed with which the medication was or will be introduced into the patient.  Typically, the rate for an infusion e.g. 100 ml per 1 hour or 100 ml/hr.  May also be expressed as a rate per unit of time, e.g. 500 ml per 2 hours.  Other examples:  200 mcg/min or 200 mcg/1 minute; 1 liter/8 hours.
        # If the rate changes over time, and you want to capture this in MedicationAdministration, then each change should be captured as a distinct MedicationAdministration, with a specific MedicationAdministration.dosage.rate, and the date time when the rate change occurred. Typically, the MedicationAdministration.dosage.rate element is not used to convey an average rate.
        'rateQuantity' => {
          'type'=>'Quantity',
          'path'=>'Dosage.rate[x]',
          'min'=>0,
          'max'=>1
        }
        ##
        # Dose quantity per unit of time
        # Identifies the speed with which the medication was or will be introduced into the patient.  Typically, the rate for an infusion e.g. 100 ml per 1 hour or 100 ml/hr.  May also be expressed as a rate per unit of time, e.g. 500 ml per 2 hours.  Other examples:  200 mcg/min or 200 mcg/1 minute; 1 liter/8 hours.
        # If the rate changes over time, and you want to capture this in MedicationAdministration, then each change should be captured as a distinct MedicationAdministration, with a specific MedicationAdministration.dosage.rate, and the date time when the rate change occurred. Typically, the MedicationAdministration.dosage.rate element is not used to convey an average rate.
        'rateRatio' => {
          'type'=>'Ratio',
          'path'=>'Dosage.rate[x]',
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
      # Free text dosage instructions e.g. SIG
      # Free text dosage can be used for cases where the dosage administered is too complex to code. When coded dosage is present, the free text dosage may still be present for display to humans.The dosage instructions should reflect the dosage of the medication that was administered.
      attr_accessor :text                           # 0-1 string
      ##
      # Body site administered to
      # A coded specification of the anatomic site where the medication first entered the body.  For example, "left arm".
      # If the use case requires attributes from the BodySite resource (e.g. to identify and track separately) then use the standard extension [bodySite](extension-bodysite.html).  May be a summary code, or a reference to a very precise definition of the location, or both.
      attr_accessor :site                           # 0-1 CodeableConcept
      ##
      # Path of substance into body
      # A code specifying the route or physiological path of administration of a therapeutic agent into or onto the patient.  For example, topical, intravenous, etc.
      attr_accessor :route                          # 0-1 CodeableConcept
      ##
      # How drug was administered
      # A coded value indicating the method by which the medication is intended to be or was introduced into or on the body.  This attribute will most often NOT be populated.  It is most commonly used for injections.  For example, Slow Push, Deep IV.
      # One of the reasons this attribute is not used often, is that the method is often pre-coordinated with the route and/or form of administration.  This means the codes used in route or form may pre-coordinate the method in the route code or the form code.  The implementation decision about what coding system to use for route or form code will determine how frequently the method code will be populated e.g. if route or form code pre-coordinate method code, then this attribute will not be populated often; if there is no pre-coordination then method code may  be used frequently.
      attr_accessor :local_method                   # 0-1 CodeableConcept
      ##
      # Amount of medication per dose
      # The amount of the medication given at one administration event.   Use this value when the administration is essentially an instantaneous event such as a swallowing a tablet or giving an injection.
      # If the administration is not instantaneous (rate is present), this can be specified to convey the total amount administered over period of time of a single administration.
      attr_accessor :dose                           # 0-1 Quantity
      ##
      # Dose quantity per unit of time
      # Identifies the speed with which the medication was or will be introduced into the patient.  Typically, the rate for an infusion e.g. 100 ml per 1 hour or 100 ml/hr.  May also be expressed as a rate per unit of time, e.g. 500 ml per 2 hours.  Other examples:  200 mcg/min or 200 mcg/1 minute; 1 liter/8 hours.
      # If the rate changes over time, and you want to capture this in MedicationAdministration, then each change should be captured as a distinct MedicationAdministration, with a specific MedicationAdministration.dosage.rate, and the date time when the rate change occurred. Typically, the MedicationAdministration.dosage.rate element is not used to convey an average rate.
      attr_accessor :rateQuantity                   # 0-1 Quantity
      ##
      # Dose quantity per unit of time
      # Identifies the speed with which the medication was or will be introduced into the patient.  Typically, the rate for an infusion e.g. 100 ml per 1 hour or 100 ml/hr.  May also be expressed as a rate per unit of time, e.g. 500 ml per 2 hours.  Other examples:  200 mcg/min or 200 mcg/1 minute; 1 liter/8 hours.
      # If the rate changes over time, and you want to capture this in MedicationAdministration, then each change should be captured as a distinct MedicationAdministration, with a specific MedicationAdministration.dosage.rate, and the date time when the rate change occurred. Typically, the MedicationAdministration.dosage.rate element is not used to convey an average rate.
      attr_accessor :rateRatio                      # 0-1 Ratio
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
    # External identifier
    # Identifiers associated with this Medication Administration that are defined by business processes and/or used to refer to it when a direct URL reference to the resource itself is not appropriate. They are business identifiers assigned to this resource by the performer or other systems and remain constant as the resource is updated and propagates from server to server.
    # This is a business identifier, not a resource identifier.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Instantiates protocol or definition
    # A protocol, guideline, orderset, or other definition that was adhered to in whole or in part by this event.
    attr_accessor :instantiates                   # 0-* [ uri ]
    ##
    # Part of referenced event
    # A larger event of which this particular event is a component or step.
    attr_accessor :partOf                         # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/MedicationAdministration|http://hl7.org/fhir/StructureDefinition/Procedure) ]
    ##
    # in-progress | not-done | on-hold | completed | entered-in-error | stopped | unknown
    # Will generally be set to show that the administration has been completed.  For some long running administrations such as infusions, it is possible for an administration to be started but not completed or it may be paused while some other process is under way.
    # This element is labeled as a modifier because the status contains codes that mark the resource as not currently valid.
    attr_accessor :status                         # 1-1 code
    ##
    # Reason administration not performed
    # A code indicating why the administration was not performed.
    attr_accessor :statusReason                   # 0-* [ CodeableConcept ]
    ##
    # Type of medication usage
    # Indicates where the medication is expected to be consumed or administered.
    attr_accessor :category                       # 0-1 CodeableConcept
    ##
    # What was administered
    # Identifies the medication that was administered. This is either a link to a resource representing the details of the medication or a simple attribute carrying a code that identifies the medication from a known list of medications.
    # If only a code is specified, then it needs to be a code for a specific product. If more information is required, then the use of the medication resource is recommended.  For example, if you require form or lot number, then you must reference the Medication resource.
    attr_accessor :medicationCodeableConcept      # 1-1 CodeableConcept
    ##
    # What was administered
    # Identifies the medication that was administered. This is either a link to a resource representing the details of the medication or a simple attribute carrying a code that identifies the medication from a known list of medications.
    # If only a code is specified, then it needs to be a code for a specific product. If more information is required, then the use of the medication resource is recommended.  For example, if you require form or lot number, then you must reference the Medication resource.
    attr_accessor :medicationReference            # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Medication)
    ##
    # Who received medication
    # The person or animal or group receiving the medication.
    attr_accessor :subject                        # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Group)
    ##
    # Encounter or Episode of Care administered as part of
    # The visit, admission, or other contact between patient and health care provider during which the medication administration was performed.
    attr_accessor :context                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Encounter|http://hl7.org/fhir/StructureDefinition/EpisodeOfCare)
    ##
    # Additional information to support administration
    # Additional information (for example, patient height and weight) that supports the administration of the medication.
    attr_accessor :supportingInformation          # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Resource) ]
    ##
    # Start and end time of administration
    # A specific date/time or interval of time during which the administration took place (or did not take place, when the 'notGiven' attribute is true). For many administrations, such as swallowing a tablet the use of dateTime is more appropriate.
    attr_accessor :effectiveDateTime              # 1-1 DateTime
    ##
    # Start and end time of administration
    # A specific date/time or interval of time during which the administration took place (or did not take place, when the 'notGiven' attribute is true). For many administrations, such as swallowing a tablet the use of dateTime is more appropriate.
    attr_accessor :effectivePeriod                # 1-1 Period
    ##
    # Who performed the medication administration and what they did
    # Indicates who or what performed the medication administration and how they were involved.
    attr_accessor :performer                      # 0-* [ MedicationAdministration::Performer ]
    ##
    # Reason administration performed
    # A code indicating why the medication was given.
    attr_accessor :reasonCode                     # 0-* [ CodeableConcept ]
    ##
    # Condition or observation that supports why the medication was administered.
    # This is a reference to a condition that is the reason for the medication request.  If only a code exists, use reasonCode.
    attr_accessor :reasonReference                # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Condition|http://hl7.org/fhir/StructureDefinition/Observation|http://hl7.org/fhir/StructureDefinition/DiagnosticReport) ]
    ##
    # Request administration performed against
    # The original request, instruction or authority to perform the administration.
    # This is a reference to the MedicationRequest  where the intent is either order or instance-order.  It should not reference MedicationRequests where the intent is any other value.
    attr_accessor :request                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/MedicationRequest)
    ##
    # Device used to administer
    # The device used in administering the medication to the patient.  For example, a particular infusion pump.
    attr_accessor :device                         # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Device) ]
    ##
    # Information about the administration
    # Extra information about the medication administration that is not conveyed by the other attributes.
    attr_accessor :note                           # 0-* [ Annotation ]
    ##
    # Details of how medication was taken
    # Describes the medication dosage information details e.g. dose, rate, site, route, etc.
    attr_accessor :dosage                         # 0-1 MedicationAdministration::Dosage
    ##
    # A list of events of interest in the lifecycle
    # A summary of the events of interest that have occurred, such as when the administration was verified.
    # This might not include provenances for all versions of the request – only those deemed “relevant” or important. This SHALL NOT include the Provenance associated with this current version of the resource. (If that provenance is deemed to be a “relevant” change, it will need to be added as part of a later update. Until then, it can be queried directly as the Provenance that points to this version using _revinclude All Provenances should have some historical version of this Request as their subject.
    attr_accessor :eventHistory                   # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Provenance) ]

    def resourceType
      'MedicationAdministration'
    end
  end
end
