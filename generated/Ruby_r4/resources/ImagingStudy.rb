module FHIR

  ##
  # Representation of the content produced in a DICOM imaging study. A study comprises a set of series, each of which includes a set of Service-Object Pair Instances (SOP Instances - images or other data) acquired or produced in a common context.  A series is of only one modality (e.g. X-ray, CT, MR, ultrasound), but a study may have multiple series of different modalities.
  class ImagingStudy < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['basedon', 'bodysite', 'dicom-class', 'encounter', 'endpoint', 'identifier', 'instance', 'interpreter', 'modality', 'patient', 'performer', 'reason', 'referrer', 'series', 'started', 'status', 'subject']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'ImagingStudy.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'ImagingStudy.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'ImagingStudy.implicitRules',
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
        'path'=>'ImagingStudy.language',
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
        'path'=>'ImagingStudy.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'ImagingStudy.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'ImagingStudy.extension',
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
        'path'=>'ImagingStudy.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Identifiers for the whole study
      # Identifiers for the ImagingStudy such as DICOM Study Instance UID, and Accession Number.
      # See discussion under [Imaging Study Implementation Notes](imagingstudy.html#notes) for encoding of DICOM Study Instance UID. Accession Number should use ACSN Identifier type.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'ImagingStudy.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # registered | available | cancelled | entered-in-error | unknown
      # The current state of the ImagingStudy.
      # Unknown does not represent "other" - one of the defined statuses must apply.  Unknown is used when the authoring system is not sure what the current status is.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/imagingstudy-status'=>[ 'registered', 'available', 'cancelled', 'entered-in-error', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'ImagingStudy.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/imagingstudy-status'}
      },
      ##
      # All series modality if actual acquisition modalities
      # A list of all the series.modality values that are actual acquisition modalities, i.e. those in the DICOM Context Group 29 (value set OID 1.2.840.10008.6.1.19).
      'modality' => {
        'valid_codes'=>{
          'http://dicom.nema.org/resources/ontology/DCM'=>[ 'OPV', 'DX', 'OPT', 'BMD', 'MG', 'SM', 'US', 'OP', 'IVOCT', 'MR', 'ECG', 'GM', 'IO', 'XA', 'XC', 'VA', 'IVUS', 'CR', 'ES', 'AR', 'CT', 'OSS', 'LEN', 'RG', 'RF', 'KER', 'HD', 'OAM', 'NM', 'OCT', 'BDUS', 'PT', 'EPS', 'PX', 'SRF', 'OPM', 'OPR' ]
        },
        'type'=>'Coding',
        'path'=>'ImagingStudy.modality',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://dicom.nema.org/medical/dicom/current/output/chtml/part16/sect_CID_29.html'}
      },
      ##
      # Who or what is the subject of the study
      # The subject, typically a patient, of the imaging study.
      # QA phantoms can be recorded with a Device; multiple subjects (such as mice) can be recorded with a Group.
      'subject' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/Group'],
        'type'=>'Reference',
        'path'=>'ImagingStudy.subject',
        'min'=>1,
        'max'=>1
      },
      ##
      # Encounter with which this imaging study is associated
      # The healthcare event (e.g. a patient and healthcare provider interaction) during which this ImagingStudy is made.
      # This will typically be the encounter the event occurred within, but some events may be initiated prior to or after the official completion of an encounter but still be tied to the context of the encounter (e.g. pre-admission test).
      'encounter' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Encounter'],
        'type'=>'Reference',
        'path'=>'ImagingStudy.encounter',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the study was started
      # Date and time the study started.
      'started' => {
        'type'=>'dateTime',
        'path'=>'ImagingStudy.started',
        'min'=>0,
        'max'=>1
      },
      ##
      # Request fulfilled
      # A list of the diagnostic requests that resulted in this imaging study being performed.
      'basedOn' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/CarePlan', 'http://hl7.org/fhir/StructureDefinition/ServiceRequest', 'http://hl7.org/fhir/StructureDefinition/Appointment', 'http://hl7.org/fhir/StructureDefinition/AppointmentResponse', 'http://hl7.org/fhir/StructureDefinition/Task'],
        'type'=>'Reference',
        'path'=>'ImagingStudy.basedOn',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Referring physician
      # The requesting/referring physician.
      'referrer' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole'],
        'type'=>'Reference',
        'path'=>'ImagingStudy.referrer',
        'min'=>0,
        'max'=>1
      },
      ##
      # Who interpreted images
      # Who read the study and interpreted the images or other content.
      'interpreter' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole'],
        'type'=>'Reference',
        'path'=>'ImagingStudy.interpreter',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Study access endpoint
      # The network service providing access (e.g., query, view, or retrieval) for the study. See implementation notes for information about using DICOM endpoints. A study-level endpoint applies to each series in the study, unless overridden by a series-level endpoint with the same Endpoint.connectionType.
      # Typical endpoint types include DICOM WADO-RS, which is used to retrieve DICOM instances in native or rendered (e.g., JPG, PNG), formats using a RESTful API; DICOM WADO-URI, which can similarly retrieve native or rendered instances, except using an HTTP query-based approach; DICOM QIDO-RS, which allows RESTful query for DICOM information without retrieving the actual instances; or IHE Invoke Image Display (IID), which provides standard invocation of an imaging web viewer.
      'endpoint' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Endpoint'],
        'type'=>'Reference',
        'path'=>'ImagingStudy.endpoint',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Number of Study Related Series
      # Number of Series in the Study. This value given may be larger than the number of series elements this Resource contains due to resource availability, security, or other factors. This element should be present if any series elements are present.
      'numberOfSeries' => {
        'type'=>'unsignedInt',
        'path'=>'ImagingStudy.numberOfSeries',
        'min'=>0,
        'max'=>1
      },
      ##
      # Number of Study Related Instances
      # Number of SOP Instances in Study. This value given may be larger than the number of instance elements this resource contains due to resource availability, security, or other factors. This element should be present if any instance elements are present.
      'numberOfInstances' => {
        'type'=>'unsignedInt',
        'path'=>'ImagingStudy.numberOfInstances',
        'min'=>0,
        'max'=>1
      },
      ##
      # The performed Procedure reference
      # The procedure which this ImagingStudy was part of.
      'procedureReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Procedure'],
        'type'=>'Reference',
        'path'=>'ImagingStudy.procedureReference',
        'min'=>0,
        'max'=>1
      },
      ##
      # The performed procedure code
      # The code for the performed procedure type.
      'procedureCode' => {
        'type'=>'CodeableConcept',
        'path'=>'ImagingStudy.procedureCode',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Where ImagingStudy occurred
      # The principal physical location where the ImagingStudy was performed.
      'location' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Location'],
        'type'=>'Reference',
        'path'=>'ImagingStudy.location',
        'min'=>0,
        'max'=>1
      },
      ##
      # Why the study was requested
      # Description of clinical condition indicating why the ImagingStudy was requested.
      'reasonCode' => {
        'type'=>'CodeableConcept',
        'path'=>'ImagingStudy.reasonCode',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Why was study performed
      # Indicates another resource whose existence justifies this Study.
      'reasonReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Condition', 'http://hl7.org/fhir/StructureDefinition/Observation', 'http://hl7.org/fhir/StructureDefinition/Media', 'http://hl7.org/fhir/StructureDefinition/DiagnosticReport', 'http://hl7.org/fhir/StructureDefinition/DocumentReference'],
        'type'=>'Reference',
        'path'=>'ImagingStudy.reasonReference',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # User-defined comments
      # Per the recommended DICOM mapping, this element is derived from the Study Description attribute (0008,1030). Observations or findings about the imaging study should be recorded in another resource, e.g. Observation, and not in this element.
      'note' => {
        'type'=>'Annotation',
        'path'=>'ImagingStudy.note',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Institution-generated description
      # The Imaging Manager description of the study. Institution-generated description or classification of the Study (component) performed.
      'description' => {
        'type'=>'string',
        'path'=>'ImagingStudy.description',
        'min'=>0,
        'max'=>1
      },
      ##
      # Each study has one or more series of instances
      # Each study has one or more series of images or other content.
      'series' => {
        'type'=>'ImagingStudy::Series',
        'path'=>'ImagingStudy.series',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Each study has one or more series of instances
    # Each study has one or more series of images or other content.
    class Series < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Series.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Series.extension',
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
          'path'=>'Series.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # DICOM Series Instance UID for the series
        # The DICOM Series Instance UID for the series.
        # See [DICOM PS3.3 C.7.3](http://dicom.nema.org/medical/dicom/current/output/chtml/part03/sect_C.7.3.html).
        'uid' => {
          'type'=>'id',
          'path'=>'Series.uid',
          'min'=>1,
          'max'=>1
        },
        ##
        # Numeric identifier of this series
        # The numeric identifier of this series in the study.
        'number' => {
          'type'=>'unsignedInt',
          'path'=>'Series.number',
          'min'=>0,
          'max'=>1
        },
        ##
        # The modality of the instances in the series
        # The modality of this series sequence.
        'modality' => {
          'valid_codes'=>{
            'http://dicom.nema.org/resources/ontology/DCM'=>[ 'OPV', 'DX', 'OPT', 'BMD', 'MG', 'SM', 'US', 'OP', 'IVOCT', 'MR', 'ECG', 'GM', 'IO', 'XA', 'XC', 'VA', 'IVUS', 'CR', 'ES', 'AR', 'CT', 'OSS', 'LEN', 'RG', 'RF', 'KER', 'HD', 'OAM', 'NM', 'OCT', 'BDUS', 'PT', 'EPS', 'PX', 'SRF', 'OPM', 'OPR' ]
          },
          'type'=>'Coding',
          'path'=>'Series.modality',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'extensible', 'uri'=>'http://dicom.nema.org/medical/dicom/current/output/chtml/part16/sect_CID_29.html'}
        },
        ##
        # A short human readable summary of the series
        # A description of the series.
        'description' => {
          'type'=>'string',
          'path'=>'Series.description',
          'min'=>0,
          'max'=>1
        },
        ##
        # Number of Series Related Instances
        # Number of SOP Instances in the Study. The value given may be larger than the number of instance elements this resource contains due to resource availability, security, or other factors. This element should be present if any instance elements are present.
        'numberOfInstances' => {
          'type'=>'unsignedInt',
          'path'=>'Series.numberOfInstances',
          'min'=>0,
          'max'=>1
        },
        ##
        # Series access endpoint
        # The network service providing access (e.g., query, view, or retrieval) for this series. See implementation notes for information about using DICOM endpoints. A series-level endpoint, if present, has precedence over a study-level endpoint with the same Endpoint.connectionType.
        # Typical endpoint types include DICOM WADO-RS, which is used to retrieve DICOM instances in native or rendered (e.g., JPG, PNG) formats using a RESTful API; DICOM WADO-URI, which can similarly retrieve native or rendered instances, except using an HTTP query-based approach; and DICOM QIDO-RS, which allows RESTful query for DICOM information without retrieving the actual instances.
        'endpoint' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Endpoint'],
          'type'=>'Reference',
          'path'=>'Series.endpoint',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Body part examined
        # The anatomic structures examined. See DICOM Part 16 Annex L (http://dicom.nema.org/medical/dicom/current/output/chtml/part16/chapter_L.html) for DICOM to SNOMED-CT mappings. The bodySite may indicate the laterality of body part imaged; if so, it shall be consistent with any content of ImagingStudy.series.laterality.
        'bodySite' => {
          'type'=>'Coding',
          'path'=>'Series.bodySite',
          'min'=>0,
          'max'=>1
        },
        ##
        # Body part laterality
        # The laterality of the (possibly paired) anatomic structures examined. E.g., the left knee, both lungs, or unpaired abdomen. If present, shall be consistent with any laterality information indicated in ImagingStudy.series.bodySite.
        'laterality' => {
          'type'=>'Coding',
          'path'=>'Series.laterality',
          'min'=>0,
          'max'=>1
        },
        ##
        # Specimen imaged
        # The specimen imaged, e.g., for whole slide imaging of a biopsy.
        'specimen' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Specimen'],
          'type'=>'Reference',
          'path'=>'Series.specimen',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # When the series started
        # The date and time the series was started.
        'started' => {
          'type'=>'dateTime',
          'path'=>'Series.started',
          'min'=>0,
          'max'=>1
        },
        ##
        # Who performed the series
        # Indicates who or what performed the series and how they were involved.
        # If the person who performed the series is not known, their Organization may be recorded. A patient, or related person, may be the performer, e.g. for patient-captured images.
        'performer' => {
          'type'=>'ImagingStudy::Series::Performer',
          'path'=>'Series.performer',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # A single SOP instance from the series
        # A single SOP instance within the series, e.g. an image, or presentation state.
        'instance' => {
          'type'=>'ImagingStudy::Series::Instance',
          'path'=>'Series.instance',
          'min'=>0,
          'max'=>Float::INFINITY
        }
      }

      ##
      # Who performed the series
      # Indicates who or what performed the series and how they were involved.
      # If the person who performed the series is not known, their Organization may be recorded. A patient, or related person, may be the performer, e.g. for patient-captured images.
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
          # Distinguishes the type of involvement of the performer in the series.
          'function' => {
            'valid_codes'=>{
              'http://terminology.hl7.org/CodeSystem/v3-ParticipationType'=>[ 'CON', 'VRF', 'PRF', 'SPRF', 'REF' ]
            },
            'type'=>'CodeableConcept',
            'path'=>'Performer.function',
            'min'=>0,
            'max'=>1,
            'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/series-performer-function'}
          },
          ##
          # Who performed the series
          # Indicates who or what performed the series.
          'actor' => {
            'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/CareTeam', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson'],
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
        # Distinguishes the type of involvement of the performer in the series.
        attr_accessor :function                       # 0-1 CodeableConcept
        ##
        # Who performed the series
        # Indicates who or what performed the series.
        attr_accessor :actor                          # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/CareTeam|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/RelatedPerson)
      end

      ##
      # A single SOP instance from the series
      # A single SOP instance within the series, e.g. an image, or presentation state.
      class Instance < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'id',
            'path'=>'Instance.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Instance.extension',
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
            'path'=>'Instance.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # DICOM SOP Instance UID
          # The DICOM SOP Instance UID for this image or other DICOM content.
          # See  [DICOM PS3.3 C.12.1](http://dicom.nema.org/medical/dicom/current/output/chtml/part03/sect_C.12.html#sect_C.12.1).
          'uid' => {
            'type'=>'id',
            'path'=>'Instance.uid',
            'min'=>1,
            'max'=>1
          },
          ##
          # DICOM class type
          # DICOM instance  type.
          'sopClass' => {
            'type'=>'Coding',
            'path'=>'Instance.sopClass',
            'min'=>1,
            'max'=>1
          },
          ##
          # The number of this instance in the series
          # The number of instance in the series.
          'number' => {
            'type'=>'unsignedInt',
            'path'=>'Instance.number',
            'min'=>0,
            'max'=>1
          },
          ##
          # Description of instance
          # The description of the instance.
          # Particularly for post-acquisition analytic objects, such as SR, presentation states, value mapping, etc.
          'title' => {
            'type'=>'string',
            'path'=>'Instance.title',
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
        # DICOM SOP Instance UID
        # The DICOM SOP Instance UID for this image or other DICOM content.
        # See  [DICOM PS3.3 C.12.1](http://dicom.nema.org/medical/dicom/current/output/chtml/part03/sect_C.12.html#sect_C.12.1).
        attr_accessor :uid                            # 1-1 id
        ##
        # DICOM class type
        # DICOM instance  type.
        attr_accessor :sopClass                       # 1-1 Coding
        ##
        # The number of this instance in the series
        # The number of instance in the series.
        attr_accessor :number                         # 0-1 unsignedInt
        ##
        # Description of instance
        # The description of the instance.
        # Particularly for post-acquisition analytic objects, such as SR, presentation states, value mapping, etc.
        attr_accessor :title                          # 0-1 string
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
      # DICOM Series Instance UID for the series
      # The DICOM Series Instance UID for the series.
      # See [DICOM PS3.3 C.7.3](http://dicom.nema.org/medical/dicom/current/output/chtml/part03/sect_C.7.3.html).
      attr_accessor :uid                            # 1-1 id
      ##
      # Numeric identifier of this series
      # The numeric identifier of this series in the study.
      attr_accessor :number                         # 0-1 unsignedInt
      ##
      # The modality of the instances in the series
      # The modality of this series sequence.
      attr_accessor :modality                       # 1-1 Coding
      ##
      # A short human readable summary of the series
      # A description of the series.
      attr_accessor :description                    # 0-1 string
      ##
      # Number of Series Related Instances
      # Number of SOP Instances in the Study. The value given may be larger than the number of instance elements this resource contains due to resource availability, security, or other factors. This element should be present if any instance elements are present.
      attr_accessor :numberOfInstances              # 0-1 unsignedInt
      ##
      # Series access endpoint
      # The network service providing access (e.g., query, view, or retrieval) for this series. See implementation notes for information about using DICOM endpoints. A series-level endpoint, if present, has precedence over a study-level endpoint with the same Endpoint.connectionType.
      # Typical endpoint types include DICOM WADO-RS, which is used to retrieve DICOM instances in native or rendered (e.g., JPG, PNG) formats using a RESTful API; DICOM WADO-URI, which can similarly retrieve native or rendered instances, except using an HTTP query-based approach; and DICOM QIDO-RS, which allows RESTful query for DICOM information without retrieving the actual instances.
      attr_accessor :endpoint                       # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Endpoint) ]
      ##
      # Body part examined
      # The anatomic structures examined. See DICOM Part 16 Annex L (http://dicom.nema.org/medical/dicom/current/output/chtml/part16/chapter_L.html) for DICOM to SNOMED-CT mappings. The bodySite may indicate the laterality of body part imaged; if so, it shall be consistent with any content of ImagingStudy.series.laterality.
      attr_accessor :bodySite                       # 0-1 Coding
      ##
      # Body part laterality
      # The laterality of the (possibly paired) anatomic structures examined. E.g., the left knee, both lungs, or unpaired abdomen. If present, shall be consistent with any laterality information indicated in ImagingStudy.series.bodySite.
      attr_accessor :laterality                     # 0-1 Coding
      ##
      # Specimen imaged
      # The specimen imaged, e.g., for whole slide imaging of a biopsy.
      attr_accessor :specimen                       # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Specimen) ]
      ##
      # When the series started
      # The date and time the series was started.
      attr_accessor :started                        # 0-1 dateTime
      ##
      # Who performed the series
      # Indicates who or what performed the series and how they were involved.
      # If the person who performed the series is not known, their Organization may be recorded. A patient, or related person, may be the performer, e.g. for patient-captured images.
      attr_accessor :performer                      # 0-* [ ImagingStudy::Series::Performer ]
      ##
      # A single SOP instance from the series
      # A single SOP instance within the series, e.g. an image, or presentation state.
      attr_accessor :instance                       # 0-* [ ImagingStudy::Series::Instance ]
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
    # Identifiers for the whole study
    # Identifiers for the ImagingStudy such as DICOM Study Instance UID, and Accession Number.
    # See discussion under [Imaging Study Implementation Notes](imagingstudy.html#notes) for encoding of DICOM Study Instance UID. Accession Number should use ACSN Identifier type.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # registered | available | cancelled | entered-in-error | unknown
    # The current state of the ImagingStudy.
    # Unknown does not represent "other" - one of the defined statuses must apply.  Unknown is used when the authoring system is not sure what the current status is.
    attr_accessor :status                         # 1-1 code
    ##
    # All series modality if actual acquisition modalities
    # A list of all the series.modality values that are actual acquisition modalities, i.e. those in the DICOM Context Group 29 (value set OID 1.2.840.10008.6.1.19).
    attr_accessor :modality                       # 0-* [ Coding ]
    ##
    # Who or what is the subject of the study
    # The subject, typically a patient, of the imaging study.
    # QA phantoms can be recorded with a Device; multiple subjects (such as mice) can be recorded with a Group.
    attr_accessor :subject                        # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/Group)
    ##
    # Encounter with which this imaging study is associated
    # The healthcare event (e.g. a patient and healthcare provider interaction) during which this ImagingStudy is made.
    # This will typically be the encounter the event occurred within, but some events may be initiated prior to or after the official completion of an encounter but still be tied to the context of the encounter (e.g. pre-admission test).
    attr_accessor :encounter                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Encounter)
    ##
    # When the study was started
    # Date and time the study started.
    attr_accessor :started                        # 0-1 dateTime
    ##
    # Request fulfilled
    # A list of the diagnostic requests that resulted in this imaging study being performed.
    attr_accessor :basedOn                        # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/CarePlan|http://hl7.org/fhir/StructureDefinition/ServiceRequest|http://hl7.org/fhir/StructureDefinition/Appointment|http://hl7.org/fhir/StructureDefinition/AppointmentResponse|http://hl7.org/fhir/StructureDefinition/Task) ]
    ##
    # Referring physician
    # The requesting/referring physician.
    attr_accessor :referrer                       # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole)
    ##
    # Who interpreted images
    # Who read the study and interpreted the images or other content.
    attr_accessor :interpreter                    # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole) ]
    ##
    # Study access endpoint
    # The network service providing access (e.g., query, view, or retrieval) for the study. See implementation notes for information about using DICOM endpoints. A study-level endpoint applies to each series in the study, unless overridden by a series-level endpoint with the same Endpoint.connectionType.
    # Typical endpoint types include DICOM WADO-RS, which is used to retrieve DICOM instances in native or rendered (e.g., JPG, PNG), formats using a RESTful API; DICOM WADO-URI, which can similarly retrieve native or rendered instances, except using an HTTP query-based approach; DICOM QIDO-RS, which allows RESTful query for DICOM information without retrieving the actual instances; or IHE Invoke Image Display (IID), which provides standard invocation of an imaging web viewer.
    attr_accessor :endpoint                       # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Endpoint) ]
    ##
    # Number of Study Related Series
    # Number of Series in the Study. This value given may be larger than the number of series elements this Resource contains due to resource availability, security, or other factors. This element should be present if any series elements are present.
    attr_accessor :numberOfSeries                 # 0-1 unsignedInt
    ##
    # Number of Study Related Instances
    # Number of SOP Instances in Study. This value given may be larger than the number of instance elements this resource contains due to resource availability, security, or other factors. This element should be present if any instance elements are present.
    attr_accessor :numberOfInstances              # 0-1 unsignedInt
    ##
    # The performed Procedure reference
    # The procedure which this ImagingStudy was part of.
    attr_accessor :procedureReference             # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Procedure)
    ##
    # The performed procedure code
    # The code for the performed procedure type.
    attr_accessor :procedureCode                  # 0-* [ CodeableConcept ]
    ##
    # Where ImagingStudy occurred
    # The principal physical location where the ImagingStudy was performed.
    attr_accessor :location                       # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Location)
    ##
    # Why the study was requested
    # Description of clinical condition indicating why the ImagingStudy was requested.
    attr_accessor :reasonCode                     # 0-* [ CodeableConcept ]
    ##
    # Why was study performed
    # Indicates another resource whose existence justifies this Study.
    attr_accessor :reasonReference                # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Condition|http://hl7.org/fhir/StructureDefinition/Observation|http://hl7.org/fhir/StructureDefinition/Media|http://hl7.org/fhir/StructureDefinition/DiagnosticReport|http://hl7.org/fhir/StructureDefinition/DocumentReference) ]
    ##
    # User-defined comments
    # Per the recommended DICOM mapping, this element is derived from the Study Description attribute (0008,1030). Observations or findings about the imaging study should be recorded in another resource, e.g. Observation, and not in this element.
    attr_accessor :note                           # 0-* [ Annotation ]
    ##
    # Institution-generated description
    # The Imaging Manager description of the study. Institution-generated description or classification of the Study (component) performed.
    attr_accessor :description                    # 0-1 string
    ##
    # Each study has one or more series of instances
    # Each study has one or more series of images or other content.
    attr_accessor :series                         # 0-* [ ImagingStudy::Series ]

    def resourceType
      'ImagingStudy'
    end
  end
end
