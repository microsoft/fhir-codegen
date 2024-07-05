module FHIR

  ##
  # The findings and interpretation of diagnostic  tests performed on patients, groups of patients, devices, and locations, and/or specimens derived from these. The report includes clinical context such as requesting and provider information, and some mix of atomic results, images, textual and coded interpretations, and formatted representation of diagnostic reports.
  # To support reporting for any diagnostic report into a clinical data repository.
  class DiagnosticReport < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['assessed-condition', 'based-on', 'category', 'code', 'conclusion', 'date', 'encounter', 'identifier', 'issued', 'media', 'patient', 'performer', 'result', 'results-interpreter', 'specimen', 'status', 'subject']
    MULTIPLE_TYPES = {
      'effective[x]' => ['dateTime', 'Period']
    }
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'DiagnosticReport.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'DiagnosticReport.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'DiagnosticReport.implicitRules',
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
        'path'=>'DiagnosticReport.language',
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
        'path'=>'DiagnosticReport.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'DiagnosticReport.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'DiagnosticReport.extension',
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
        'path'=>'DiagnosticReport.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Business identifier for report
      # Identifiers assigned to this report by the performer or other systems.
      # Usually assigned by the Information System of the diagnostic service provider (filler id).
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'DiagnosticReport.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # What was requested
      # Details concerning a service requested.
      # Note: Usually there is one test request for each result, however in some circumstances multiple test requests may be represented using a single test result resource. Note that there are also cases where one request leads to multiple reports.
      'basedOn' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/CarePlan', 'http://hl7.org/fhir/StructureDefinition/ImmunizationRecommendation', 'http://hl7.org/fhir/StructureDefinition/MedicationRequest', 'http://hl7.org/fhir/StructureDefinition/NutritionOrder', 'http://hl7.org/fhir/StructureDefinition/ServiceRequest'],
        'type'=>'Reference',
        'path'=>'DiagnosticReport.basedOn',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # registered | partial | preliminary | final +
      # The status of the diagnostic report.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/diagnostic-report-status'=>[ 'registered', 'partial', 'preliminary', 'final', 'amended', 'corrected', 'appended', 'cancelled', 'entered-in-error', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'DiagnosticReport.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/diagnostic-report-status'}
      },
      ##
      # Service category
      # A code that classifies the clinical discipline, department or diagnostic service that created the report (e.g. cardiology, biochemistry, hematology, MRI). This is used for searching, sorting and display purposes.
      # Multiple categories are allowed using various categorization schemes.   The level of granularity is defined by the category concepts in the value set. More fine-grained filtering can be performed using the metadata and/or terminology hierarchy in DiagnosticReport.code.
      'category' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/v2-0074'=>[ 'AU', 'BG', 'BLB', 'CG', 'CH', 'CP', 'CT', 'CTH', 'CUS', 'EC', 'EN', 'GE', 'HM', 'ICU', 'IMM', 'LAB', 'MB', 'MCB', 'MYC', 'NMR', 'NMS', 'NRS', 'OSL', 'OT', 'OTH', 'OUS', 'PF', 'PHR', 'PHY', 'PT', 'RAD', 'RC', 'RT', 'RUS', 'RX', 'SP', 'SR', 'TX', 'VR', 'VUS', 'XRC' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'DiagnosticReport.category',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/diagnostic-service-sections'}
      },
      ##
      # Name/Code for this diagnostic report
      # A code or name that describes this diagnostic report.
      'code' => {
        'type'=>'CodeableConcept',
        'path'=>'DiagnosticReport.code',
        'min'=>1,
        'max'=>1
      },
      ##
      # The subject of the report - usually, but not always, the patient
      # The subject of the report. Usually, but not always, this is a patient. However, diagnostic services also perform analyses on specimens collected from a variety of other sources.
      'subject' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Group', 'http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/Location'],
        'type'=>'Reference',
        'path'=>'DiagnosticReport.subject',
        'min'=>0,
        'max'=>1
      },
      ##
      # Health care event when test ordered
      # The healthcare event  (e.g. a patient and healthcare provider interaction) which this DiagnosticReport is about.
      # This will typically be the encounter the event occurred within, but some events may be initiated prior to or after the official completion of an encounter  but still be tied to the context of the encounter  (e.g. pre-admission laboratory tests).
      'encounter' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Encounter'],
        'type'=>'Reference',
        'path'=>'DiagnosticReport.encounter',
        'min'=>0,
        'max'=>1
      },
      ##
      # Clinically relevant time/time-period for report
      # The time or time-period the observed values are related to. When the subject of the report is a patient, this is usually either the time of the procedure or of specimen collection(s), but very often the source of the date/time is not known, only the date/time itself.
      # If the diagnostic procedure was performed on the patient, this is the time it was performed. If there are specimens, the diagnostically relevant time can be derived from the specimen collection times, but the specimen information is not always available, and the exact relationship between the specimens and the diagnostically relevant time is not always automatic.
      'effectiveDateTime' => {
        'type'=>'DateTime',
        'path'=>'DiagnosticReport.effective[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Clinically relevant time/time-period for report
      # The time or time-period the observed values are related to. When the subject of the report is a patient, this is usually either the time of the procedure or of specimen collection(s), but very often the source of the date/time is not known, only the date/time itself.
      # If the diagnostic procedure was performed on the patient, this is the time it was performed. If there are specimens, the diagnostically relevant time can be derived from the specimen collection times, but the specimen information is not always available, and the exact relationship between the specimens and the diagnostically relevant time is not always automatic.
      'effectivePeriod' => {
        'type'=>'Period',
        'path'=>'DiagnosticReport.effective[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # DateTime this version was made
      # The date and time that this version of the report was made available to providers, typically after the report was reviewed and verified.
      # May be different from the update time of the resource itself, because that is the status of the record (potentially a secondary copy), not the actual release time of the report.
      'issued' => {
        'type'=>'instant',
        'path'=>'DiagnosticReport.issued',
        'min'=>0,
        'max'=>1
      },
      ##
      # Responsible Diagnostic Service
      # The diagnostic service that is responsible for issuing the report.
      # This is not necessarily the source of the atomic data items or the entity that interpreted the results. It is the entity that takes responsibility for the clinical report.
      'performer' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/CareTeam'],
        'type'=>'Reference',
        'path'=>'DiagnosticReport.performer',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Primary result interpreter
      # The practitioner or organization that is responsible for the report's conclusions and interpretations.
      # Might not be the same entity that takes responsibility for the clinical report.
      'resultsInterpreter' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/CareTeam'],
        'type'=>'Reference',
        'path'=>'DiagnosticReport.resultsInterpreter',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Specimens this report is based on
      # Details about the specimens on which this diagnostic report is based.
      # If the specimen is sufficiently specified with a code in the test result name, then this additional data may be redundant. If there are multiple specimens, these may be represented per observation or group.
      'specimen' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Specimen'],
        'type'=>'Reference',
        'path'=>'DiagnosticReport.specimen',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Observations
      # [Observations](observation.html)  that are part of this diagnostic report.
      # Observations can contain observations.
      'result' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Observation'],
        'type'=>'Reference',
        'path'=>'DiagnosticReport.result',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Reference to full details of imaging associated with the diagnostic report
      # One or more links to full details of any imaging performed during the diagnostic investigation. Typically, this is imaging performed by DICOM enabled modalities, but this is not required. A fully enabled PACS viewer can use this information to provide views of the source images.
      # ImagingStudy and the image element are somewhat overlapping - typically, the list of image references in the image element will also be found in one of the imaging study resources. However, each caters to different types of displays for different types of purposes. Neither, either, or both may be provided.
      'imagingStudy' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ImagingStudy'],
        'type'=>'Reference',
        'path'=>'DiagnosticReport.imagingStudy',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Key images associated with this report
      # A list of key images associated with this report. The images are generally created during the diagnostic process, and may be directly of the patient, or of treated specimens (i.e. slides of interest).
      'media' => {
        'type'=>'DiagnosticReport::Media',
        'path'=>'DiagnosticReport.media',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Clinical conclusion (interpretation) of test results
      # Concise and clinically contextualized summary conclusion (interpretation/impression) of the diagnostic report.
      'conclusion' => {
        'type'=>'string',
        'path'=>'DiagnosticReport.conclusion',
        'min'=>0,
        'max'=>1
      },
      ##
      # Codes for the clinical conclusion of test results
      # One or more codes that represent the summary conclusion (interpretation/impression) of the diagnostic report.
      'conclusionCode' => {
        'type'=>'CodeableConcept',
        'path'=>'DiagnosticReport.conclusionCode',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Entire report as issued
      # Rich text representation of the entire result as issued by the diagnostic service. Multiple formats are allowed but they SHALL be semantically equivalent.
      # "application/pdf" is recommended as the most reliable and interoperable in this context.
      'presentedForm' => {
        'type'=>'Attachment',
        'path'=>'DiagnosticReport.presentedForm',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Key images associated with this report
    # A list of key images associated with this report. The images are generally created during the diagnostic process, and may be directly of the patient, or of treated specimens (i.e. slides of interest).
    class Media < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Media.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Media.extension',
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
          'path'=>'Media.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Comment about the image (e.g. explanation)
        # A comment about the image. Typically, this is used to provide an explanation for why the image is included, or to draw the viewer's attention to important features.
        # The comment should be displayed with the image. It would be common for the report to include additional discussion of the image contents in other sections such as the conclusion.
        'comment' => {
          'type'=>'string',
          'path'=>'Media.comment',
          'min'=>0,
          'max'=>1
        },
        ##
        # Reference to the image source.
        'link' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Media'],
          'type'=>'Reference',
          'path'=>'Media.link',
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
      # Comment about the image (e.g. explanation)
      # A comment about the image. Typically, this is used to provide an explanation for why the image is included, or to draw the viewer's attention to important features.
      # The comment should be displayed with the image. It would be common for the report to include additional discussion of the image contents in other sections such as the conclusion.
      attr_accessor :comment                        # 0-1 string
      ##
      # Reference to the image source.
      attr_accessor :link                           # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Media)
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
    # Business identifier for report
    # Identifiers assigned to this report by the performer or other systems.
    # Usually assigned by the Information System of the diagnostic service provider (filler id).
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # What was requested
    # Details concerning a service requested.
    # Note: Usually there is one test request for each result, however in some circumstances multiple test requests may be represented using a single test result resource. Note that there are also cases where one request leads to multiple reports.
    attr_accessor :basedOn                        # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/CarePlan|http://hl7.org/fhir/StructureDefinition/ImmunizationRecommendation|http://hl7.org/fhir/StructureDefinition/MedicationRequest|http://hl7.org/fhir/StructureDefinition/NutritionOrder|http://hl7.org/fhir/StructureDefinition/ServiceRequest) ]
    ##
    # registered | partial | preliminary | final +
    # The status of the diagnostic report.
    attr_accessor :status                         # 1-1 code
    ##
    # Service category
    # A code that classifies the clinical discipline, department or diagnostic service that created the report (e.g. cardiology, biochemistry, hematology, MRI). This is used for searching, sorting and display purposes.
    # Multiple categories are allowed using various categorization schemes.   The level of granularity is defined by the category concepts in the value set. More fine-grained filtering can be performed using the metadata and/or terminology hierarchy in DiagnosticReport.code.
    attr_accessor :category                       # 0-* [ CodeableConcept ]
    ##
    # Name/Code for this diagnostic report
    # A code or name that describes this diagnostic report.
    attr_accessor :code                           # 1-1 CodeableConcept
    ##
    # The subject of the report - usually, but not always, the patient
    # The subject of the report. Usually, but not always, this is a patient. However, diagnostic services also perform analyses on specimens collected from a variety of other sources.
    attr_accessor :subject                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Group|http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/Location)
    ##
    # Health care event when test ordered
    # The healthcare event  (e.g. a patient and healthcare provider interaction) which this DiagnosticReport is about.
    # This will typically be the encounter the event occurred within, but some events may be initiated prior to or after the official completion of an encounter  but still be tied to the context of the encounter  (e.g. pre-admission laboratory tests).
    attr_accessor :encounter                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Encounter)
    ##
    # Clinically relevant time/time-period for report
    # The time or time-period the observed values are related to. When the subject of the report is a patient, this is usually either the time of the procedure or of specimen collection(s), but very often the source of the date/time is not known, only the date/time itself.
    # If the diagnostic procedure was performed on the patient, this is the time it was performed. If there are specimens, the diagnostically relevant time can be derived from the specimen collection times, but the specimen information is not always available, and the exact relationship between the specimens and the diagnostically relevant time is not always automatic.
    attr_accessor :effectiveDateTime              # 0-1 DateTime
    ##
    # Clinically relevant time/time-period for report
    # The time or time-period the observed values are related to. When the subject of the report is a patient, this is usually either the time of the procedure or of specimen collection(s), but very often the source of the date/time is not known, only the date/time itself.
    # If the diagnostic procedure was performed on the patient, this is the time it was performed. If there are specimens, the diagnostically relevant time can be derived from the specimen collection times, but the specimen information is not always available, and the exact relationship between the specimens and the diagnostically relevant time is not always automatic.
    attr_accessor :effectivePeriod                # 0-1 Period
    ##
    # DateTime this version was made
    # The date and time that this version of the report was made available to providers, typically after the report was reviewed and verified.
    # May be different from the update time of the resource itself, because that is the status of the record (potentially a secondary copy), not the actual release time of the report.
    attr_accessor :issued                         # 0-1 instant
    ##
    # Responsible Diagnostic Service
    # The diagnostic service that is responsible for issuing the report.
    # This is not necessarily the source of the atomic data items or the entity that interpreted the results. It is the entity that takes responsibility for the clinical report.
    attr_accessor :performer                      # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/CareTeam) ]
    ##
    # Primary result interpreter
    # The practitioner or organization that is responsible for the report's conclusions and interpretations.
    # Might not be the same entity that takes responsibility for the clinical report.
    attr_accessor :resultsInterpreter             # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/CareTeam) ]
    ##
    # Specimens this report is based on
    # Details about the specimens on which this diagnostic report is based.
    # If the specimen is sufficiently specified with a code in the test result name, then this additional data may be redundant. If there are multiple specimens, these may be represented per observation or group.
    attr_accessor :specimen                       # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Specimen) ]
    ##
    # Observations
    # [Observations](observation.html)  that are part of this diagnostic report.
    # Observations can contain observations.
    attr_accessor :result                         # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Observation) ]
    ##
    # Reference to full details of imaging associated with the diagnostic report
    # One or more links to full details of any imaging performed during the diagnostic investigation. Typically, this is imaging performed by DICOM enabled modalities, but this is not required. A fully enabled PACS viewer can use this information to provide views of the source images.
    # ImagingStudy and the image element are somewhat overlapping - typically, the list of image references in the image element will also be found in one of the imaging study resources. However, each caters to different types of displays for different types of purposes. Neither, either, or both may be provided.
    attr_accessor :imagingStudy                   # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/ImagingStudy) ]
    ##
    # Key images associated with this report
    # A list of key images associated with this report. The images are generally created during the diagnostic process, and may be directly of the patient, or of treated specimens (i.e. slides of interest).
    attr_accessor :media                          # 0-* [ DiagnosticReport::Media ]
    ##
    # Clinical conclusion (interpretation) of test results
    # Concise and clinically contextualized summary conclusion (interpretation/impression) of the diagnostic report.
    attr_accessor :conclusion                     # 0-1 string
    ##
    # Codes for the clinical conclusion of test results
    # One or more codes that represent the summary conclusion (interpretation/impression) of the diagnostic report.
    attr_accessor :conclusionCode                 # 0-* [ CodeableConcept ]
    ##
    # Entire report as issued
    # Rich text representation of the entire result as issued by the diagnostic service. Multiple formats are allowed but they SHALL be semantically equivalent.
    # "application/pdf" is recommended as the most reliable and interoperable in this context.
    attr_accessor :presentedForm                  # 0-* [ Attachment ]

    def resourceType
      'DiagnosticReport'
    end
  end
end
