module FHIR

  ##
  # The Care Team includes all the people and organizations who plan to participate in the coordination and delivery of care for a patient.
  class CareTeam < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['category', 'date', 'encounter', 'identifier', 'participant', 'patient', 'status', 'subject']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'CareTeam.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'CareTeam.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'CareTeam.implicitRules',
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
        'path'=>'CareTeam.language',
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
        'path'=>'CareTeam.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'CareTeam.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'CareTeam.extension',
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
        'path'=>'CareTeam.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # External Ids for this team
      # Business identifiers assigned to this care team by the performer or other systems which remain constant as the resource is updated and propagates from server to server.
      # This is a business identifier, not a resource identifier (see [discussion](resource.html#identifiers)).  It is best practice for the identifier to only appear on a single resource instance, however business practices may occasionally dictate that multiple resource instances with the same identifier can exist - possibly even with different resource types.  For example, multiple Patient and a Person resource instance might share the same social insurance number.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'CareTeam.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # proposed | active | suspended | inactive | entered-in-error
      # Indicates the current state of the care team.
      # This element is labeled as a modifier because the status contains the code entered-in-error that marks the care team as not currently valid.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/care-team-status'=>[ 'proposed', 'active', 'suspended', 'inactive', 'entered-in-error' ]
        },
        'type'=>'code',
        'path'=>'CareTeam.status',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/care-team-status'}
      },
      ##
      # Type of team
      # Identifies what kind of team.  This is to support differentiation between multiple co-existing teams, such as care plan team, episode of care team, longitudinal care team.
      # There may be multiple axis of categorization and one team may serve multiple purposes.
      'category' => {
        'type'=>'CodeableConcept',
        'path'=>'CareTeam.category',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Name of the team, such as crisis assessment team
      # A label for human use intended to distinguish like teams.  E.g. the "red" vs. "green" trauma teams.
      # The meaning/purpose of the team is conveyed in CareTeam.category.  This element may also convey semantics of the team (e.g. "Red trauma team"), but its primary purpose is to distinguish between identical teams in a human-friendly way.  ("Team 18735" isn't as friendly.).
      'name' => {
        'type'=>'string',
        'path'=>'CareTeam.name',
        'min'=>0,
        'max'=>1
      },
      ##
      # Who care team is for
      # Identifies the patient or group whose intended care is handled by the team.
      'subject' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Group'],
        'type'=>'Reference',
        'path'=>'CareTeam.subject',
        'min'=>0,
        'max'=>1
      },
      ##
      # Encounter created as part of
      # The Encounter during which this CareTeam was created or to which the creation of this record is tightly associated.
      # This will typically be the encounter the event occurred within, but some activities may be initiated prior to or after the official completion of an encounter but still be tied to the context of the encounter.
      'encounter' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Encounter'],
        'type'=>'Reference',
        'path'=>'CareTeam.encounter',
        'min'=>0,
        'max'=>1
      },
      ##
      # Time period team covers
      # Indicates when the team did (or is intended to) come into effect and end.
      'period' => {
        'type'=>'Period',
        'path'=>'CareTeam.period',
        'min'=>0,
        'max'=>1
      },
      ##
      # Members of the team
      # Identifies all people and organizations who are expected to be involved in the care team.
      'participant' => {
        'type'=>'CareTeam::Participant',
        'path'=>'CareTeam.participant',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Why the care team exists
      # Describes why the care team exists.
      'reasonCode' => {
        'type'=>'CodeableConcept',
        'path'=>'CareTeam.reasonCode',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Why the care team exists
      # Condition(s) that this care team addresses.
      'reasonReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Condition'],
        'type'=>'Reference',
        'path'=>'CareTeam.reasonReference',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Organization responsible for the care team
      # The organization responsible for the care team.
      'managingOrganization' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'CareTeam.managingOrganization',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # A contact detail for the care team (that applies to all members)
      # A central contact detail for the care team (that applies to all members).
      # The ContactPoint.use code of home is not appropriate to use. These contacts are not the contact details of individual care team members.
      'telecom' => {
        'type'=>'ContactPoint',
        'path'=>'CareTeam.telecom',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Comments made about the CareTeam.
      'note' => {
        'type'=>'Annotation',
        'path'=>'CareTeam.note',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Members of the team
    # Identifies all people and organizations who are expected to be involved in the care team.
    class Participant < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Participant.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Participant.extension',
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
          'path'=>'Participant.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Type of involvement
        # Indicates specific responsibility of an individual within the care team, such as "Primary care physician", "Trained social worker counselor", "Caregiver", etc.
        # Roles may sometimes be inferred by type of Practitioner.  These are relationships that hold only within the context of the care team.  General relationships should be handled as properties of the Patient resource directly.
        'role' => {
          'type'=>'CodeableConcept',
          'path'=>'Participant.role',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Who is involved
        # The specific person or organization who is participating/expected to participate in the care team.
        # Patient only needs to be listed if they have a role other than "subject of care".
        # 
        # Member is optional because some participants may be known only by their role, particularly in draft plans.
        'member' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/CareTeam'],
          'type'=>'Reference',
          'path'=>'Participant.member',
          'min'=>0,
          'max'=>1
        },
        ##
        # Organization of the practitioner
        # The organization of the practitioner.
        'onBehalfOf' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
          'type'=>'Reference',
          'path'=>'Participant.onBehalfOf',
          'min'=>0,
          'max'=>1
        },
        ##
        # Time period of participant
        # Indicates when the specific member or organization did (or is intended to) come into effect and end.
        'period' => {
          'type'=>'Period',
          'path'=>'Participant.period',
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
      # Type of involvement
      # Indicates specific responsibility of an individual within the care team, such as "Primary care physician", "Trained social worker counselor", "Caregiver", etc.
      # Roles may sometimes be inferred by type of Practitioner.  These are relationships that hold only within the context of the care team.  General relationships should be handled as properties of the Patient resource directly.
      attr_accessor :role                           # 0-* [ CodeableConcept ]
      ##
      # Who is involved
      # The specific person or organization who is participating/expected to participate in the care team.
      # Patient only needs to be listed if they have a role other than "subject of care".
      # 
      # Member is optional because some participants may be known only by their role, particularly in draft plans.
      attr_accessor :member                         # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/CareTeam)
      ##
      # Organization of the practitioner
      # The organization of the practitioner.
      attr_accessor :onBehalfOf                     # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
      ##
      # Time period of participant
      # Indicates when the specific member or organization did (or is intended to) come into effect and end.
      attr_accessor :period                         # 0-1 Period
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
    # External Ids for this team
    # Business identifiers assigned to this care team by the performer or other systems which remain constant as the resource is updated and propagates from server to server.
    # This is a business identifier, not a resource identifier (see [discussion](resource.html#identifiers)).  It is best practice for the identifier to only appear on a single resource instance, however business practices may occasionally dictate that multiple resource instances with the same identifier can exist - possibly even with different resource types.  For example, multiple Patient and a Person resource instance might share the same social insurance number.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # proposed | active | suspended | inactive | entered-in-error
    # Indicates the current state of the care team.
    # This element is labeled as a modifier because the status contains the code entered-in-error that marks the care team as not currently valid.
    attr_accessor :status                         # 0-1 code
    ##
    # Type of team
    # Identifies what kind of team.  This is to support differentiation between multiple co-existing teams, such as care plan team, episode of care team, longitudinal care team.
    # There may be multiple axis of categorization and one team may serve multiple purposes.
    attr_accessor :category                       # 0-* [ CodeableConcept ]
    ##
    # Name of the team, such as crisis assessment team
    # A label for human use intended to distinguish like teams.  E.g. the "red" vs. "green" trauma teams.
    # The meaning/purpose of the team is conveyed in CareTeam.category.  This element may also convey semantics of the team (e.g. "Red trauma team"), but its primary purpose is to distinguish between identical teams in a human-friendly way.  ("Team 18735" isn't as friendly.).
    attr_accessor :name                           # 0-1 string
    ##
    # Who care team is for
    # Identifies the patient or group whose intended care is handled by the team.
    attr_accessor :subject                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Group)
    ##
    # Encounter created as part of
    # The Encounter during which this CareTeam was created or to which the creation of this record is tightly associated.
    # This will typically be the encounter the event occurred within, but some activities may be initiated prior to or after the official completion of an encounter but still be tied to the context of the encounter.
    attr_accessor :encounter                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Encounter)
    ##
    # Time period team covers
    # Indicates when the team did (or is intended to) come into effect and end.
    attr_accessor :period                         # 0-1 Period
    ##
    # Members of the team
    # Identifies all people and organizations who are expected to be involved in the care team.
    attr_accessor :participant                    # 0-* [ CareTeam::Participant ]
    ##
    # Why the care team exists
    # Describes why the care team exists.
    attr_accessor :reasonCode                     # 0-* [ CodeableConcept ]
    ##
    # Why the care team exists
    # Condition(s) that this care team addresses.
    attr_accessor :reasonReference                # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Condition) ]
    ##
    # Organization responsible for the care team
    # The organization responsible for the care team.
    attr_accessor :managingOrganization           # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Organization) ]
    ##
    # A contact detail for the care team (that applies to all members)
    # A central contact detail for the care team (that applies to all members).
    # The ContactPoint.use code of home is not appropriate to use. These contacts are not the contact details of individual care team members.
    attr_accessor :telecom                        # 0-* [ ContactPoint ]
    ##
    # Comments made about the CareTeam.
    attr_accessor :note                           # 0-* [ Annotation ]

    def resourceType
      'CareTeam'
    end
  end
end
