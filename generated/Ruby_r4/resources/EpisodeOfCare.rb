module FHIR

  ##
  # An association between a patient and an organization / healthcare provider(s) during which time encounters may occur. The managing organization assumes a level of responsibility for the patient during this time.
  class EpisodeOfCare < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['care-manager', 'condition', 'date', 'identifier', 'incoming-referral', 'organization', 'patient', 'status', 'type']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'EpisodeOfCare.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'EpisodeOfCare.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'EpisodeOfCare.implicitRules',
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
        'path'=>'EpisodeOfCare.language',
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
        'path'=>'EpisodeOfCare.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'EpisodeOfCare.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'EpisodeOfCare.extension',
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
        'path'=>'EpisodeOfCare.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Business Identifier(s) relevant for this EpisodeOfCare
      # The EpisodeOfCare may be known by different identifiers for different contexts of use, such as when an external agency is tracking the Episode for funding purposes.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'EpisodeOfCare.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # planned | waitlist | active | onhold | finished | cancelled | entered-in-error
      # planned | waitlist | active | onhold | finished | cancelled.
      # This element is labeled as a modifier because the status contains codes that mark the episode as not currently valid.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/episode-of-care-status'=>[ 'planned', 'waitlist', 'active', 'onhold', 'finished', 'cancelled', 'entered-in-error' ]
        },
        'type'=>'code',
        'path'=>'EpisodeOfCare.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/episode-of-care-status'}
      },
      ##
      # Past list of status codes (the current status may be included to cover the start date of the status)
      # The history of statuses that the EpisodeOfCare has been through (without requiring processing the history of the resource).
      'statusHistory' => {
        'type'=>'EpisodeOfCare::StatusHistory',
        'path'=>'EpisodeOfCare.statusHistory',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Type/class  - e.g. specialist referral, disease management
      # A classification of the type of episode of care; e.g. specialist referral, disease management, type of funded care.
      # The type can be very important in processing as this could be used in determining if the EpisodeOfCare is relevant to specific government reporting, or other types of classifications.
      'type' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/episodeofcare-type'=>[ 'hacc', 'pac', 'diab', 'da', 'cacp' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'EpisodeOfCare.type',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/episodeofcare-type'}
      },
      ##
      # The list of diagnosis relevant to this episode of care.
      'diagnosis' => {
        'type'=>'EpisodeOfCare::Diagnosis',
        'path'=>'EpisodeOfCare.diagnosis',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The patient who is the focus of this episode of care.
      'patient' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient'],
        'type'=>'Reference',
        'path'=>'EpisodeOfCare.patient',
        'min'=>1,
        'max'=>1
      },
      ##
      # Organization that assumes care
      # The organization that has assumed the specific responsibilities for the specified duration.
      'managingOrganization' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'EpisodeOfCare.managingOrganization',
        'min'=>0,
        'max'=>1
      },
      ##
      # Interval during responsibility is assumed
      # The interval during which the managing organization assumes the defined responsibility.
      'period' => {
        'type'=>'Period',
        'path'=>'EpisodeOfCare.period',
        'min'=>0,
        'max'=>1
      },
      ##
      # Originating Referral Request(s)
      # Referral Request(s) that are fulfilled by this EpisodeOfCare, incoming referrals.
      'referralRequest' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ServiceRequest'],
        'type'=>'Reference',
        'path'=>'EpisodeOfCare.referralRequest',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Care manager/care coordinator for the patient
      # The practitioner that is the care manager/care coordinator for this patient.
      'careManager' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole'],
        'type'=>'Reference',
        'path'=>'EpisodeOfCare.careManager',
        'min'=>0,
        'max'=>1
      },
      ##
      # Other practitioners facilitating this episode of care
      # The list of practitioners that may be facilitating this episode of care for specific purposes.
      'team' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/CareTeam'],
        'type'=>'Reference',
        'path'=>'EpisodeOfCare.team',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The set of accounts that may be used for billing for this EpisodeOfCare.
      # The billing system may choose to allocate billable items associated with the EpisodeOfCare to different referenced Accounts based on internal business rules.
      'account' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Account'],
        'type'=>'Reference',
        'path'=>'EpisodeOfCare.account',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Past list of status codes (the current status may be included to cover the start date of the status)
    # The history of statuses that the EpisodeOfCare has been through (without requiring processing the history of the resource).
    class StatusHistory < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'StatusHistory.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'StatusHistory.extension',
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
          'path'=>'StatusHistory.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # planned | waitlist | active | onhold | finished | cancelled | entered-in-error
        # planned | waitlist | active | onhold | finished | cancelled.
        'status' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/episode-of-care-status'=>[ 'planned', 'waitlist', 'active', 'onhold', 'finished', 'cancelled', 'entered-in-error' ]
          },
          'type'=>'code',
          'path'=>'StatusHistory.status',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/episode-of-care-status'}
        },
        ##
        # Duration the EpisodeOfCare was in the specified status
        # The period during this EpisodeOfCare that the specific status applied.
        'period' => {
          'type'=>'Period',
          'path'=>'StatusHistory.period',
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
      # planned | waitlist | active | onhold | finished | cancelled | entered-in-error
      # planned | waitlist | active | onhold | finished | cancelled.
      attr_accessor :status                         # 1-1 code
      ##
      # Duration the EpisodeOfCare was in the specified status
      # The period during this EpisodeOfCare that the specific status applied.
      attr_accessor :period                         # 1-1 Period
    end

    ##
    # The list of diagnosis relevant to this episode of care.
    class Diagnosis < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Diagnosis.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Diagnosis.extension',
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
          'path'=>'Diagnosis.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Conditions/problems/diagnoses this episode of care is for
        # A list of conditions/problems/diagnoses that this episode of care is intended to be providing care for.
        'condition' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Condition'],
          'type'=>'Reference',
          'path'=>'Diagnosis.condition',
          'min'=>1,
          'max'=>1
        },
        ##
        # Role that this diagnosis has within the episode of care (e.g. admission, billing, discharge …).
        'role' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/diagnosis-role'=>[ 'AD', 'DD', 'CC', 'CM', 'pre-op', 'post-op', 'billing' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Diagnosis.role',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/diagnosis-role'}
        },
        ##
        # Ranking of the diagnosis (for each role type).
        'rank' => {
          'type'=>'positiveInt',
          'path'=>'Diagnosis.rank',
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
      # Conditions/problems/diagnoses this episode of care is for
      # A list of conditions/problems/diagnoses that this episode of care is intended to be providing care for.
      attr_accessor :condition                      # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Condition)
      ##
      # Role that this diagnosis has within the episode of care (e.g. admission, billing, discharge …).
      attr_accessor :role                           # 0-1 CodeableConcept
      ##
      # Ranking of the diagnosis (for each role type).
      attr_accessor :rank                           # 0-1 positiveInt
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
    # Business Identifier(s) relevant for this EpisodeOfCare
    # The EpisodeOfCare may be known by different identifiers for different contexts of use, such as when an external agency is tracking the Episode for funding purposes.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # planned | waitlist | active | onhold | finished | cancelled | entered-in-error
    # planned | waitlist | active | onhold | finished | cancelled.
    # This element is labeled as a modifier because the status contains codes that mark the episode as not currently valid.
    attr_accessor :status                         # 1-1 code
    ##
    # Past list of status codes (the current status may be included to cover the start date of the status)
    # The history of statuses that the EpisodeOfCare has been through (without requiring processing the history of the resource).
    attr_accessor :statusHistory                  # 0-* [ EpisodeOfCare::StatusHistory ]
    ##
    # Type/class  - e.g. specialist referral, disease management
    # A classification of the type of episode of care; e.g. specialist referral, disease management, type of funded care.
    # The type can be very important in processing as this could be used in determining if the EpisodeOfCare is relevant to specific government reporting, or other types of classifications.
    attr_accessor :type                           # 0-* [ CodeableConcept ]
    ##
    # The list of diagnosis relevant to this episode of care.
    attr_accessor :diagnosis                      # 0-* [ EpisodeOfCare::Diagnosis ]
    ##
    # The patient who is the focus of this episode of care.
    attr_accessor :patient                        # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient)
    ##
    # Organization that assumes care
    # The organization that has assumed the specific responsibilities for the specified duration.
    attr_accessor :managingOrganization           # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Interval during responsibility is assumed
    # The interval during which the managing organization assumes the defined responsibility.
    attr_accessor :period                         # 0-1 Period
    ##
    # Originating Referral Request(s)
    # Referral Request(s) that are fulfilled by this EpisodeOfCare, incoming referrals.
    attr_accessor :referralRequest                # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/ServiceRequest) ]
    ##
    # Care manager/care coordinator for the patient
    # The practitioner that is the care manager/care coordinator for this patient.
    attr_accessor :careManager                    # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole)
    ##
    # Other practitioners facilitating this episode of care
    # The list of practitioners that may be facilitating this episode of care for specific purposes.
    attr_accessor :team                           # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/CareTeam) ]
    ##
    # The set of accounts that may be used for billing for this EpisodeOfCare.
    # The billing system may choose to allocate billable items associated with the EpisodeOfCare to different referenced Accounts based on internal business rules.
    attr_accessor :account                        # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Account) ]

    def resourceType
      'EpisodeOfCare'
    end
  end
end
