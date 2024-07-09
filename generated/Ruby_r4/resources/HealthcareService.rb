module FHIR

  ##
  # The details of a healthcare service available at a location.
  class HealthcareService < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['active', 'characteristic', 'coverage-area', 'endpoint', 'identifier', 'location', 'name', 'organization', 'program', 'service-category', 'service-type', 'specialty']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'HealthcareService.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'HealthcareService.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'HealthcareService.implicitRules',
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
        'path'=>'HealthcareService.language',
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
        'path'=>'HealthcareService.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'HealthcareService.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'HealthcareService.extension',
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
        'path'=>'HealthcareService.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # External identifiers for this item.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'HealthcareService.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Whether this HealthcareService record is in active use
      # This flag is used to mark the record to not be used. This is not used when a center is closed for maintenance, or for holidays, the notAvailable period is to be used for this.
      # This element is labeled as a modifier because it may be used to mark that the resource was created in error.
      'active' => {
        'type'=>'boolean',
        'path'=>'HealthcareService.active',
        'min'=>0,
        'max'=>1
      },
      ##
      # Organization that provides this service
      # The organization that provides this healthcare service.
      # This property is recommended to be the same as the Location's managingOrganization, and if not provided should be interpreted as such. If the Location does not have a managing Organization, then this property should be populated.
      'providedBy' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'HealthcareService.providedBy',
        'min'=>0,
        'max'=>1
      },
      ##
      # Broad category of service being performed or delivered
      # Identifies the broad category of service being performed or delivered.
      # Selecting a Service Category then determines the list of relevant service types that can be selected in the primary service type.
      'category' => {
        'type'=>'CodeableConcept',
        'path'=>'HealthcareService.category',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Type of service that may be delivered or performed
      # The specific type of service that may be delivered or performed.
      'type' => {
        'type'=>'CodeableConcept',
        'path'=>'HealthcareService.type',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Specialties handled by the HealthcareService
      # Collection of specialties handled by the service site. This is more of a medical term.
      'specialty' => {
        'type'=>'CodeableConcept',
        'path'=>'HealthcareService.specialty',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Location(s) where service may be provided
      # The location(s) where this healthcare service may be provided.
      'location' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Location'],
        'type'=>'Reference',
        'path'=>'HealthcareService.location',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Description of service as presented to a consumer while searching
      # Further description of the service as it would be presented to a consumer while searching.
      'name' => {
        'type'=>'string',
        'path'=>'HealthcareService.name',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional description and/or any specific issues not covered elsewhere
      # Any additional description of the service and/or any specific issues not covered by the other attributes, which can be displayed as further detail under the serviceName.
      # Would expect that a user would not see this information on a search results, and it would only be available when viewing the complete details of the service.
      'comment' => {
        'type'=>'string',
        'path'=>'HealthcareService.comment',
        'min'=>0,
        'max'=>1
      },
      ##
      # Extra details about the service that can't be placed in the other fields.
      'extraDetails' => {
        'type'=>'markdown',
        'path'=>'HealthcareService.extraDetails',
        'min'=>0,
        'max'=>1
      },
      ##
      # Facilitates quick identification of the service
      # If there is a photo/symbol associated with this HealthcareService, it may be included here to facilitate quick identification of the service in a list.
      'photo' => {
        'type'=>'Attachment',
        'path'=>'HealthcareService.photo',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contacts related to the healthcare service
      # List of contacts related to this specific healthcare service.
      # If this is empty, then refer to the location's contacts.
      'telecom' => {
        'type'=>'ContactPoint',
        'path'=>'HealthcareService.telecom',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Location(s) service is intended for/available to
      # The location(s) that this service is available to (not where the service is provided).
      # The locations referenced by the coverage area can include both specific locations, including areas, and also conceptual domains too (mode = kind), such as a physical area (tri-state area) and some other attribute (covered by Example Care Organization). These types of Locations are often not managed by any specific organization. This could also include generic locations such as "in-home".
      'coverageArea' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Location'],
        'type'=>'Reference',
        'path'=>'HealthcareService.coverageArea',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Conditions under which service is available/offered
      # The code(s) that detail the conditions under which the healthcare service is available/offered.
      # The provision means being commissioned by, contractually obliged or financially sourced. Types of costings that may apply to this healthcare service, such if the service may be available for free, some discounts available, or fees apply.
      'serviceProvisionCode' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/service-provision-conditions'=>[ 'free', 'disc', 'cost' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'HealthcareService.serviceProvisionCode',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/service-provision-conditions'}
      },
      ##
      # Specific eligibility requirements required to use the service
      # Does this service have specific eligibility requirements that need to be met in order to use the service?
      'eligibility' => {
        'type'=>'HealthcareService::Eligibility',
        'path'=>'HealthcareService.eligibility',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Programs that this service is applicable to.
      # Programs are often defined externally to an Organization, commonly by governments; e.g. Home and Community Care Programs, Homeless Program, ….
      'program' => {
        'type'=>'CodeableConcept',
        'path'=>'HealthcareService.program',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Collection of characteristics (attributes).
      # These could be such things as is wheelchair accessible.
      'characteristic' => {
        'type'=>'CodeableConcept',
        'path'=>'HealthcareService.characteristic',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The language that this service is offered in
      # Some services are specifically made available in multiple languages, this property permits a directory to declare the languages this is offered in. Typically this is only provided where a service operates in communities with mixed languages used.
      # When using this property it indicates that the service is available with this language, it is not derived from the practitioners, and not all are required to use this language, just that this language is available while scheduling.
      'communication' => {
        'valid_codes'=>{
          'urn:ietf:bcp:47'=>[ 'ar', 'bn', 'cs', 'da', 'de', 'de-AT', 'de-CH', 'de-DE', 'el', 'en', 'en-AU', 'en-CA', 'en-GB', 'en-IN', 'en-NZ', 'en-SG', 'en-US', 'es', 'es-AR', 'es-ES', 'es-UY', 'fi', 'fr', 'fr-BE', 'fr-CH', 'fr-FR', 'fy', 'fy-NL', 'hi', 'hr', 'it', 'it-CH', 'it-IT', 'ja', 'ko', 'nl', 'nl-BE', 'nl-NL', 'no', 'no-NO', 'pa', 'pl', 'pt', 'pt-BR', 'ru', 'ru-RU', 'sr', 'sr-RS', 'sv', 'sv-SE', 'te', 'zh', 'zh-CN', 'zh-HK', 'zh-SG', 'zh-TW' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'HealthcareService.communication',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/languages'}
      },
      ##
      # Ways that the service accepts referrals, if this is not provided then it is implied that no referral is required.
      'referralMethod' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/service-referral-method'=>[ 'fax', 'phone', 'elec', 'semail', 'mail' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'HealthcareService.referralMethod',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/service-referral-method'}
      },
      ##
      # If an appointment is required for access to this service
      # Indicates whether or not a prospective consumer will require an appointment for a particular service at a site to be provided by the Organization. Indicates if an appointment is required for access to this service.
      'appointmentRequired' => {
        'type'=>'boolean',
        'path'=>'HealthcareService.appointmentRequired',
        'min'=>0,
        'max'=>1
      },
      ##
      # Times the Service Site is available
      # A collection of times that the Service Site is available.
      # More detailed availability information may be provided in associated Schedule/Slot resources.
      'availableTime' => {
        'type'=>'HealthcareService::AvailableTime',
        'path'=>'HealthcareService.availableTime',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Not available during this time due to provided reason
      # The HealthcareService is not available during this period of time due to the provided reason.
      'notAvailable' => {
        'type'=>'HealthcareService::NotAvailable',
        'path'=>'HealthcareService.notAvailable',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Description of availability exceptions
      # A description of site availability exceptions, e.g. public holiday availability. Succinctly describing all possible exceptions to normal site availability as details in the available Times and not available Times.
      'availabilityExceptions' => {
        'type'=>'string',
        'path'=>'HealthcareService.availabilityExceptions',
        'min'=>0,
        'max'=>1
      },
      ##
      # Technical endpoints providing access to electronic services operated for the healthcare service
      # Technical endpoints providing access to services operated for the specific healthcare services defined at this resource.
      'endpoint' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Endpoint'],
        'type'=>'Reference',
        'path'=>'HealthcareService.endpoint',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Specific eligibility requirements required to use the service
    # Does this service have specific eligibility requirements that need to be met in order to use the service?
    class Eligibility < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Eligibility.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Eligibility.extension',
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
          'path'=>'Eligibility.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Coded value for the eligibility.
        'code' => {
          'type'=>'CodeableConcept',
          'path'=>'Eligibility.code',
          'min'=>0,
          'max'=>1
        },
        ##
        # Describes the eligibility conditions for the service.
        # The description of service eligibility should, in general, not exceed one or two paragraphs. It should be sufficient for a prospective consumer to determine if they are likely to be eligible or not. Where eligibility requirements and conditions are complex, it may simply be noted that an eligibility assessment is required. Where eligibility is determined by an outside source, such as an Act of Parliament, this should be noted, preferably with a reference to a commonly available copy of the source document such as a web page.
        'comment' => {
          'type'=>'markdown',
          'path'=>'Eligibility.comment',
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
      # Coded value for the eligibility.
      attr_accessor :code                           # 0-1 CodeableConcept
      ##
      # Describes the eligibility conditions for the service.
      # The description of service eligibility should, in general, not exceed one or two paragraphs. It should be sufficient for a prospective consumer to determine if they are likely to be eligible or not. Where eligibility requirements and conditions are complex, it may simply be noted that an eligibility assessment is required. Where eligibility is determined by an outside source, such as an Act of Parliament, this should be noted, preferably with a reference to a commonly available copy of the source document such as a web page.
      attr_accessor :comment                        # 0-1 markdown
    end

    ##
    # Times the Service Site is available
    # A collection of times that the Service Site is available.
    # More detailed availability information may be provided in associated Schedule/Slot resources.
    class AvailableTime < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'AvailableTime.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'AvailableTime.extension',
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
          'path'=>'AvailableTime.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # mon | tue | wed | thu | fri | sat | sun
        # Indicates which days of the week are available between the start and end Times.
        'daysOfWeek' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/days-of-week'=>[ 'mon', 'tue', 'wed', 'thu', 'fri', 'sat', 'sun' ]
          },
          'type'=>'code',
          'path'=>'AvailableTime.daysOfWeek',
          'min'=>0,
          'max'=>Float::INFINITY,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/days-of-week'}
        },
        ##
        # Always available? e.g. 24 hour service
        # Is this always available? (hence times are irrelevant) e.g. 24 hour service.
        'allDay' => {
          'type'=>'boolean',
          'path'=>'AvailableTime.allDay',
          'min'=>0,
          'max'=>1
        },
        ##
        # Opening time of day (ignored if allDay = true)
        # The opening time of day. Note: If the AllDay flag is set, then this time is ignored.
        # The time zone is expected to be for where this HealthcareService is provided at.
        'availableStartTime' => {
          'type'=>'time',
          'path'=>'AvailableTime.availableStartTime',
          'min'=>0,
          'max'=>1
        },
        ##
        # Closing time of day (ignored if allDay = true)
        # The closing time of day. Note: If the AllDay flag is set, then this time is ignored.
        # The time zone is expected to be for where this HealthcareService is provided at.
        'availableEndTime' => {
          'type'=>'time',
          'path'=>'AvailableTime.availableEndTime',
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
      # mon | tue | wed | thu | fri | sat | sun
      # Indicates which days of the week are available between the start and end Times.
      attr_accessor :daysOfWeek                     # 0-* [ code ]
      ##
      # Always available? e.g. 24 hour service
      # Is this always available? (hence times are irrelevant) e.g. 24 hour service.
      attr_accessor :allDay                         # 0-1 boolean
      ##
      # Opening time of day (ignored if allDay = true)
      # The opening time of day. Note: If the AllDay flag is set, then this time is ignored.
      # The time zone is expected to be for where this HealthcareService is provided at.
      attr_accessor :availableStartTime             # 0-1 time
      ##
      # Closing time of day (ignored if allDay = true)
      # The closing time of day. Note: If the AllDay flag is set, then this time is ignored.
      # The time zone is expected to be for where this HealthcareService is provided at.
      attr_accessor :availableEndTime               # 0-1 time
    end

    ##
    # Not available during this time due to provided reason
    # The HealthcareService is not available during this period of time due to the provided reason.
    class NotAvailable < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'NotAvailable.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'NotAvailable.extension',
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
          'path'=>'NotAvailable.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Reason presented to the user explaining why time not available
        # The reason that can be presented to the user as to why this time is not available.
        'description' => {
          'type'=>'string',
          'path'=>'NotAvailable.description',
          'min'=>1,
          'max'=>1
        },
        ##
        # Service not available from this date
        # Service is not available (seasonally or for a public holiday) from this date.
        'during' => {
          'type'=>'Period',
          'path'=>'NotAvailable.during',
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
      # Reason presented to the user explaining why time not available
      # The reason that can be presented to the user as to why this time is not available.
      attr_accessor :description                    # 1-1 string
      ##
      # Service not available from this date
      # Service is not available (seasonally or for a public holiday) from this date.
      attr_accessor :during                         # 0-1 Period
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
    # External identifiers for this item.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Whether this HealthcareService record is in active use
    # This flag is used to mark the record to not be used. This is not used when a center is closed for maintenance, or for holidays, the notAvailable period is to be used for this.
    # This element is labeled as a modifier because it may be used to mark that the resource was created in error.
    attr_accessor :active                         # 0-1 boolean
    ##
    # Organization that provides this service
    # The organization that provides this healthcare service.
    # This property is recommended to be the same as the Location's managingOrganization, and if not provided should be interpreted as such. If the Location does not have a managing Organization, then this property should be populated.
    attr_accessor :providedBy                     # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Broad category of service being performed or delivered
    # Identifies the broad category of service being performed or delivered.
    # Selecting a Service Category then determines the list of relevant service types that can be selected in the primary service type.
    attr_accessor :category                       # 0-* [ CodeableConcept ]
    ##
    # Type of service that may be delivered or performed
    # The specific type of service that may be delivered or performed.
    attr_accessor :type                           # 0-* [ CodeableConcept ]
    ##
    # Specialties handled by the HealthcareService
    # Collection of specialties handled by the service site. This is more of a medical term.
    attr_accessor :specialty                      # 0-* [ CodeableConcept ]
    ##
    # Location(s) where service may be provided
    # The location(s) where this healthcare service may be provided.
    attr_accessor :location                       # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Location) ]
    ##
    # Description of service as presented to a consumer while searching
    # Further description of the service as it would be presented to a consumer while searching.
    attr_accessor :name                           # 0-1 string
    ##
    # Additional description and/or any specific issues not covered elsewhere
    # Any additional description of the service and/or any specific issues not covered by the other attributes, which can be displayed as further detail under the serviceName.
    # Would expect that a user would not see this information on a search results, and it would only be available when viewing the complete details of the service.
    attr_accessor :comment                        # 0-1 string
    ##
    # Extra details about the service that can't be placed in the other fields.
    attr_accessor :extraDetails                   # 0-1 markdown
    ##
    # Facilitates quick identification of the service
    # If there is a photo/symbol associated with this HealthcareService, it may be included here to facilitate quick identification of the service in a list.
    attr_accessor :photo                          # 0-1 Attachment
    ##
    # Contacts related to the healthcare service
    # List of contacts related to this specific healthcare service.
    # If this is empty, then refer to the location's contacts.
    attr_accessor :telecom                        # 0-* [ ContactPoint ]
    ##
    # Location(s) service is intended for/available to
    # The location(s) that this service is available to (not where the service is provided).
    # The locations referenced by the coverage area can include both specific locations, including areas, and also conceptual domains too (mode = kind), such as a physical area (tri-state area) and some other attribute (covered by Example Care Organization). These types of Locations are often not managed by any specific organization. This could also include generic locations such as "in-home".
    attr_accessor :coverageArea                   # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Location) ]
    ##
    # Conditions under which service is available/offered
    # The code(s) that detail the conditions under which the healthcare service is available/offered.
    # The provision means being commissioned by, contractually obliged or financially sourced. Types of costings that may apply to this healthcare service, such if the service may be available for free, some discounts available, or fees apply.
    attr_accessor :serviceProvisionCode           # 0-* [ CodeableConcept ]
    ##
    # Specific eligibility requirements required to use the service
    # Does this service have specific eligibility requirements that need to be met in order to use the service?
    attr_accessor :eligibility                    # 0-* [ HealthcareService::Eligibility ]
    ##
    # Programs that this service is applicable to.
    # Programs are often defined externally to an Organization, commonly by governments; e.g. Home and Community Care Programs, Homeless Program, ….
    attr_accessor :program                        # 0-* [ CodeableConcept ]
    ##
    # Collection of characteristics (attributes).
    # These could be such things as is wheelchair accessible.
    attr_accessor :characteristic                 # 0-* [ CodeableConcept ]
    ##
    # The language that this service is offered in
    # Some services are specifically made available in multiple languages, this property permits a directory to declare the languages this is offered in. Typically this is only provided where a service operates in communities with mixed languages used.
    # When using this property it indicates that the service is available with this language, it is not derived from the practitioners, and not all are required to use this language, just that this language is available while scheduling.
    attr_accessor :communication                  # 0-* [ CodeableConcept ]
    ##
    # Ways that the service accepts referrals, if this is not provided then it is implied that no referral is required.
    attr_accessor :referralMethod                 # 0-* [ CodeableConcept ]
    ##
    # If an appointment is required for access to this service
    # Indicates whether or not a prospective consumer will require an appointment for a particular service at a site to be provided by the Organization. Indicates if an appointment is required for access to this service.
    attr_accessor :appointmentRequired            # 0-1 boolean
    ##
    # Times the Service Site is available
    # A collection of times that the Service Site is available.
    # More detailed availability information may be provided in associated Schedule/Slot resources.
    attr_accessor :availableTime                  # 0-* [ HealthcareService::AvailableTime ]
    ##
    # Not available during this time due to provided reason
    # The HealthcareService is not available during this period of time due to the provided reason.
    attr_accessor :notAvailable                   # 0-* [ HealthcareService::NotAvailable ]
    ##
    # Description of availability exceptions
    # A description of site availability exceptions, e.g. public holiday availability. Succinctly describing all possible exceptions to normal site availability as details in the available Times and not available Times.
    attr_accessor :availabilityExceptions         # 0-1 string
    ##
    # Technical endpoints providing access to electronic services operated for the healthcare service
    # Technical endpoints providing access to services operated for the specific healthcare services defined at this resource.
    attr_accessor :endpoint                       # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Endpoint) ]

    def resourceType
      'HealthcareService'
    end
  end
end
