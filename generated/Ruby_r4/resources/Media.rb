module FHIR

  ##
  # A photo, video, or audio recording acquired or used in healthcare. The actual content may be inline or provided by direct reference.
  class Media < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['based-on', 'created', 'device', 'encounter', 'identifier', 'modality', 'operator', 'patient', 'site', 'status', 'subject', 'type', 'view']
    MULTIPLE_TYPES = {
      'created[x]' => ['dateTime', 'Period']
    }
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'Media.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'Media.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'Media.implicitRules',
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
        'path'=>'Media.language',
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
        'path'=>'Media.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'Media.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Media.extension',
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
        'path'=>'Media.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Identifier(s) for the image
      # Identifiers associated with the image - these may include identifiers for the image itself, identifiers for the context of its collection (e.g. series ids) and context ids such as accession numbers or other workflow identifiers.
      # The identifier label and use can be used to determine what kind of identifier it is.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'Media.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Procedure that caused this media to be created
      # A procedure that is fulfilled in whole or in part by the creation of this media.
      'basedOn' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ServiceRequest', 'http://hl7.org/fhir/StructureDefinition/CarePlan'],
        'type'=>'Reference',
        'path'=>'Media.basedOn',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Part of referenced event
      # A larger event of which this particular event is a component or step.
      # Not to be used to link an event to an Encounter - use Media.encounter for that.[The allowed reference resources may be adjusted as appropriate for the event resource].
      'partOf' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'Reference',
        'path'=>'Media.partOf',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # preparation | in-progress | not-done | on-hold | stopped | completed | entered-in-error | unknown
      # The current state of the {{title}}.
      # A nominal state-transition diagram can be found in the [[event.html#statemachine | Event pattern]] documentationUnknown does not represent "other" - one of the defined statuses must apply.  Unknown is used when the authoring system is not sure what the current status is.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/event-status'=>[ 'preparation', 'in-progress', 'not-done', 'on-hold', 'stopped', 'completed', 'entered-in-error', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'Media.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/event-status'}
      },
      ##
      # Classification of media as image, video, or audio
      # A code that classifies whether the media is an image, video or audio recording or some other media category.
      'type' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/media-type'=>[ 'image', 'video', 'audio' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Media.type',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/media-type'}
      },
      ##
      # The type of acquisition equipment/process
      # Details of the type of the media - usually, how it was acquired (what type of device). If images sourced from a DICOM system, are wrapped in a Media resource, then this is the modality.
      'modality' => {
        'type'=>'CodeableConcept',
        'path'=>'Media.modality',
        'min'=>0,
        'max'=>1
      },
      ##
      # Imaging view, e.g. Lateral or Antero-posterior
      # The name of the imaging view e.g. Lateral or Antero-posterior (AP).
      'view' => {
        'type'=>'CodeableConcept',
        'path'=>'Media.view',
        'min'=>0,
        'max'=>1
      },
      ##
      # Who/What this Media is a record of.
      'subject' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Group', 'http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/Specimen', 'http://hl7.org/fhir/StructureDefinition/Location'],
        'type'=>'Reference',
        'path'=>'Media.subject',
        'min'=>0,
        'max'=>1
      },
      ##
      # Encounter associated with media
      # The encounter that establishes the context for this media.
      # This will typically be the encounter the media occurred within.
      'encounter' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Encounter'],
        'type'=>'Reference',
        'path'=>'Media.encounter',
        'min'=>0,
        'max'=>1
      },
      ##
      # When Media was collected
      # The date and time(s) at which the media was collected.
      'createdDateTime' => {
        'type'=>'DateTime',
        'path'=>'Media.created[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # When Media was collected
      # The date and time(s) at which the media was collected.
      'createdPeriod' => {
        'type'=>'Period',
        'path'=>'Media.created[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Date/Time this version was made available
      # The date and time this version of the media was made available to providers, typically after having been reviewed.
      # It may be the same as the [`lastUpdated` ](resource-definitions.html#Meta.lastUpdated) time of the resource itself.  For Observations that do require review and verification for certain updates, it might not be the same as the `lastUpdated` time of the resource itself due to a non-clinically significant update that does not require the new version to be reviewed and verified again.
      'issued' => {
        'type'=>'instant',
        'path'=>'Media.issued',
        'min'=>0,
        'max'=>1
      },
      ##
      # The person who generated the image
      # The person who administered the collection of the image.
      'operator' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/CareTeam', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson'],
        'type'=>'Reference',
        'path'=>'Media.operator',
        'min'=>0,
        'max'=>1
      },
      ##
      # Why was event performed?
      # Describes why the event occurred in coded or textual form.
      # Textual reasons can be captured using reasonCode.text.
      'reasonCode' => {
        'type'=>'CodeableConcept',
        'path'=>'Media.reasonCode',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Observed body part
      # Indicates the site on the subject's body where the observation was made (i.e. the target site).
      # Only used if not implicit in code found in Observation.code.  In many systems, this may be represented as a related observation instead of an inline component.   
      # 
      # If the use case requires BodySite to be handled as a separate resource (e.g. to identify and track separately) then use the standard extension[ bodySite](extension-bodysite.html).
      'bodySite' => {
        'type'=>'CodeableConcept',
        'path'=>'Media.bodySite',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name of the device/manufacturer
      # The name of the device / manufacturer of the device  that was used to make the recording.
      'deviceName' => {
        'type'=>'string',
        'path'=>'Media.deviceName',
        'min'=>0,
        'max'=>1
      },
      ##
      # Observing Device
      # The device used to collect the media.
      # An extension should be used if further typing of the device is needed.  Secondary devices used to support collecting a media can be represented using either extension or through the Observation.related element.
      'device' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/DeviceMetric', 'http://hl7.org/fhir/StructureDefinition/Device'],
        'type'=>'Reference',
        'path'=>'Media.device',
        'min'=>0,
        'max'=>1
      },
      ##
      # Height of the image in pixels (photo/video).
      'height' => {
        'type'=>'positiveInt',
        'path'=>'Media.height',
        'min'=>0,
        'max'=>1
      },
      ##
      # Width of the image in pixels (photo/video).
      'width' => {
        'type'=>'positiveInt',
        'path'=>'Media.width',
        'min'=>0,
        'max'=>1
      },
      ##
      # Number of frames if > 1 (photo)
      # The number of frames in a photo. This is used with a multi-page fax, or an imaging acquisition context that takes multiple slices in a single image, or an animated gif. If there is more than one frame, this SHALL have a value in order to alert interface software that a multi-frame capable rendering widget is required.
      # if the number of frames is not supplied, the value may be unknown. Applications should not assume that there is only one frame unless it is explicitly stated.
      'frames' => {
        'type'=>'positiveInt',
        'path'=>'Media.frames',
        'min'=>0,
        'max'=>1
      },
      ##
      # Length in seconds (audio / video)
      # The duration of the recording in seconds - for audio and video.
      # The duration might differ from occurrencePeriod if recording was paused.
      'duration' => {
        'type'=>'decimal',
        'path'=>'Media.duration',
        'min'=>0,
        'max'=>1
      },
      ##
      # Actual Media - reference or data
      # The actual content of the media - inline or by direct reference to the media source file.
      # Recommended content types: image/jpeg, image/png, image/tiff, video/mpeg, audio/mp4, application/dicom. Application/dicom can contain the transfer syntax as a parameter.  For media that covers a period of time (video/sound), the content.creationTime is the end time. Creation time is used for tracking, organizing versions and searching.
      'content' => {
        'type'=>'Attachment',
        'path'=>'Media.content',
        'min'=>1,
        'max'=>1
      },
      ##
      # Comments made about the media by the performer, subject or other participants.
      # Not to be used for observations, conclusions, etc. Instead use an [Observation](observation.html) based on the Media/ImagingStudy resource.
      'note' => {
        'type'=>'Annotation',
        'path'=>'Media.note',
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
    # Identifier(s) for the image
    # Identifiers associated with the image - these may include identifiers for the image itself, identifiers for the context of its collection (e.g. series ids) and context ids such as accession numbers or other workflow identifiers.
    # The identifier label and use can be used to determine what kind of identifier it is.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Procedure that caused this media to be created
    # A procedure that is fulfilled in whole or in part by the creation of this media.
    attr_accessor :basedOn                        # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/ServiceRequest|http://hl7.org/fhir/StructureDefinition/CarePlan) ]
    ##
    # Part of referenced event
    # A larger event of which this particular event is a component or step.
    # Not to be used to link an event to an Encounter - use Media.encounter for that.[The allowed reference resources may be adjusted as appropriate for the event resource].
    attr_accessor :partOf                         # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Resource) ]
    ##
    # preparation | in-progress | not-done | on-hold | stopped | completed | entered-in-error | unknown
    # The current state of the {{title}}.
    # A nominal state-transition diagram can be found in the [[event.html#statemachine | Event pattern]] documentationUnknown does not represent "other" - one of the defined statuses must apply.  Unknown is used when the authoring system is not sure what the current status is.
    attr_accessor :status                         # 1-1 code
    ##
    # Classification of media as image, video, or audio
    # A code that classifies whether the media is an image, video or audio recording or some other media category.
    attr_accessor :type                           # 0-1 CodeableConcept
    ##
    # The type of acquisition equipment/process
    # Details of the type of the media - usually, how it was acquired (what type of device). If images sourced from a DICOM system, are wrapped in a Media resource, then this is the modality.
    attr_accessor :modality                       # 0-1 CodeableConcept
    ##
    # Imaging view, e.g. Lateral or Antero-posterior
    # The name of the imaging view e.g. Lateral or Antero-posterior (AP).
    attr_accessor :view                           # 0-1 CodeableConcept
    ##
    # Who/What this Media is a record of.
    attr_accessor :subject                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Group|http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/Specimen|http://hl7.org/fhir/StructureDefinition/Location)
    ##
    # Encounter associated with media
    # The encounter that establishes the context for this media.
    # This will typically be the encounter the media occurred within.
    attr_accessor :encounter                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Encounter)
    ##
    # When Media was collected
    # The date and time(s) at which the media was collected.
    attr_accessor :createdDateTime                # 0-1 DateTime
    ##
    # When Media was collected
    # The date and time(s) at which the media was collected.
    attr_accessor :createdPeriod                  # 0-1 Period
    ##
    # Date/Time this version was made available
    # The date and time this version of the media was made available to providers, typically after having been reviewed.
    # It may be the same as the [`lastUpdated` ](resource-definitions.html#Meta.lastUpdated) time of the resource itself.  For Observations that do require review and verification for certain updates, it might not be the same as the `lastUpdated` time of the resource itself due to a non-clinically significant update that does not require the new version to be reviewed and verified again.
    attr_accessor :issued                         # 0-1 instant
    ##
    # The person who generated the image
    # The person who administered the collection of the image.
    attr_accessor :operator                       # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/CareTeam|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/RelatedPerson)
    ##
    # Why was event performed?
    # Describes why the event occurred in coded or textual form.
    # Textual reasons can be captured using reasonCode.text.
    attr_accessor :reasonCode                     # 0-* [ CodeableConcept ]
    ##
    # Observed body part
    # Indicates the site on the subject's body where the observation was made (i.e. the target site).
    # Only used if not implicit in code found in Observation.code.  In many systems, this may be represented as a related observation instead of an inline component.   
    # 
    # If the use case requires BodySite to be handled as a separate resource (e.g. to identify and track separately) then use the standard extension[ bodySite](extension-bodysite.html).
    attr_accessor :bodySite                       # 0-1 CodeableConcept
    ##
    # Name of the device/manufacturer
    # The name of the device / manufacturer of the device  that was used to make the recording.
    attr_accessor :deviceName                     # 0-1 string
    ##
    # Observing Device
    # The device used to collect the media.
    # An extension should be used if further typing of the device is needed.  Secondary devices used to support collecting a media can be represented using either extension or through the Observation.related element.
    attr_accessor :device                         # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/DeviceMetric|http://hl7.org/fhir/StructureDefinition/Device)
    ##
    # Height of the image in pixels (photo/video).
    attr_accessor :height                         # 0-1 positiveInt
    ##
    # Width of the image in pixels (photo/video).
    attr_accessor :width                          # 0-1 positiveInt
    ##
    # Number of frames if > 1 (photo)
    # The number of frames in a photo. This is used with a multi-page fax, or an imaging acquisition context that takes multiple slices in a single image, or an animated gif. If there is more than one frame, this SHALL have a value in order to alert interface software that a multi-frame capable rendering widget is required.
    # if the number of frames is not supplied, the value may be unknown. Applications should not assume that there is only one frame unless it is explicitly stated.
    attr_accessor :frames                         # 0-1 positiveInt
    ##
    # Length in seconds (audio / video)
    # The duration of the recording in seconds - for audio and video.
    # The duration might differ from occurrencePeriod if recording was paused.
    attr_accessor :duration                       # 0-1 decimal
    ##
    # Actual Media - reference or data
    # The actual content of the media - inline or by direct reference to the media source file.
    # Recommended content types: image/jpeg, image/png, image/tiff, video/mpeg, audio/mp4, application/dicom. Application/dicom can contain the transfer syntax as a parameter.  For media that covers a period of time (video/sound), the content.creationTime is the end time. Creation time is used for tracking, organizing versions and searching.
    attr_accessor :content                        # 1-1 Attachment
    ##
    # Comments made about the media by the performer, subject or other participants.
    # Not to be used for observations, conclusions, etc. Instead use an [Observation](observation.html) based on the Media/ImagingStudy resource.
    attr_accessor :note                           # 0-* [ Annotation ]

    def resourceType
      'Media'
    end
  end
end
