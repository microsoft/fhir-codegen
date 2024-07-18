module FHIR

  ##
  # Represents a defined collection of entities that may be discussed or acted upon collectively but which are not expected to act collectively, and are not formally or legally recognized; i.e. a collection of entities that isn't an Organization.
  class Group < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['actual', 'characteristic-value', 'characteristic', 'code', 'exclude', 'identifier', 'managing-entity', 'member', 'type', 'value']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'Group.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'Group.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'Group.implicitRules',
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
        'path'=>'Group.language',
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
        'path'=>'Group.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'Group.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Group.extension',
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
        'path'=>'Group.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Unique id
      # A unique business identifier for this group.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'Group.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Whether this group's record is in active use
      # Indicates whether the record for the group is available for use or is merely being retained for historical purposes.
      'active' => {
        'type'=>'boolean',
        'path'=>'Group.active',
        'min'=>0,
        'max'=>1
      },
      ##
      # person | animal | practitioner | device | medication | substance
      # Identifies the broad classification of the kind of resources the group includes.
      # Group members SHALL be of the appropriate resource type (Patient for person or animal; or Practitioner, Device, Medication or Substance for the other types.).
      'type' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/group-type'=>[ 'person', 'animal', 'practitioner', 'device', 'medication', 'substance' ]
        },
        'type'=>'code',
        'path'=>'Group.type',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/group-type'}
      },
      ##
      # Descriptive or actual
      # If true, indicates that the resource refers to a specific group of real individuals.  If false, the group defines a set of intended individuals.
      'actual' => {
        'type'=>'boolean',
        'path'=>'Group.actual',
        'min'=>1,
        'max'=>1
      },
      ##
      # Kind of Group members
      # Provides a specific type of resource the group includes; e.g. "cow", "syringe", etc.
      # This would generally be omitted for Person resources.
      'code' => {
        'type'=>'CodeableConcept',
        'path'=>'Group.code',
        'min'=>0,
        'max'=>1
      },
      ##
      # Label for Group
      # A label assigned to the group for human identification and communication.
      'name' => {
        'type'=>'string',
        'path'=>'Group.name',
        'min'=>0,
        'max'=>1
      },
      ##
      # Number of members
      # A count of the number of resource instances that are part of the group.
      # Note that the quantity may be less than the number of members if some of the members are not active.
      'quantity' => {
        'type'=>'unsignedInt',
        'path'=>'Group.quantity',
        'min'=>0,
        'max'=>1
      },
      ##
      # Entity that is the custodian of the Group's definition
      # Entity responsible for defining and maintaining Group characteristics and/or registered members.
      # This does not strictly align with ownership of a herd or flock, but may suffice to represent that relationship in simple cases. More complex cases will require an extension.
      'managingEntity' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole'],
        'type'=>'Reference',
        'path'=>'Group.managingEntity',
        'min'=>0,
        'max'=>1
      },
      ##
      # Include / Exclude group members by Trait
      # Identifies traits whose presence r absence is shared by members of the group.
      # All the identified characteristics must be true for an entity to a member of the group.
      'characteristic' => {
        'type'=>'Group::Characteristic',
        'path'=>'Group.characteristic',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Who or what is in group
      # Identifies the resource instances that are members of the group.
      'member' => {
        'type'=>'Group::Member',
        'path'=>'Group.member',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Include / Exclude group members by Trait
    # Identifies traits whose presence r absence is shared by members of the group.
    # All the identified characteristics must be true for an entity to a member of the group.
    class Characteristic < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'value[x]' => ['boolean', 'CodeableConcept', 'Quantity', 'Range', 'Reference']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Characteristic.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Characteristic.extension',
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
          'path'=>'Characteristic.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Kind of characteristic
        # A code that identifies the kind of trait being asserted.
        'code' => {
          'type'=>'CodeableConcept',
          'path'=>'Characteristic.code',
          'min'=>1,
          'max'=>1
        },
        ##
        # Value held by characteristic
        # The value of the trait that holds (or does not hold - see 'exclude') for members of the group.
        # For Range, it means members of the group have a value that falls somewhere within the specified range.
        'valueBoolean' => {
          'type'=>'Boolean',
          'path'=>'Characteristic.value[x]',
          'min'=>1,
          'max'=>1
        },
        ##
        # Value held by characteristic
        # The value of the trait that holds (or does not hold - see 'exclude') for members of the group.
        # For Range, it means members of the group have a value that falls somewhere within the specified range.
        'valueCodeableConcept' => {
          'type'=>'CodeableConcept',
          'path'=>'Characteristic.value[x]',
          'min'=>1,
          'max'=>1
        },
        ##
        # Value held by characteristic
        # The value of the trait that holds (or does not hold - see 'exclude') for members of the group.
        # For Range, it means members of the group have a value that falls somewhere within the specified range.
        'valueQuantity' => {
          'type'=>'Quantity',
          'path'=>'Characteristic.value[x]',
          'min'=>1,
          'max'=>1
        },
        ##
        # Value held by characteristic
        # The value of the trait that holds (or does not hold - see 'exclude') for members of the group.
        # For Range, it means members of the group have a value that falls somewhere within the specified range.
        'valueRange' => {
          'type'=>'Range',
          'path'=>'Characteristic.value[x]',
          'min'=>1,
          'max'=>1
        },
        ##
        # Value held by characteristic
        # The value of the trait that holds (or does not hold - see 'exclude') for members of the group.
        # For Range, it means members of the group have a value that falls somewhere within the specified range.
        'valueReference' => {
          'type'=>'Reference',
          'path'=>'Characteristic.value[x]',
          'min'=>1,
          'max'=>1
        },
        ##
        # Group includes or excludes
        # If true, indicates the characteristic is one that is NOT held by members of the group.
        # This is labeled as "Is Modifier" because applications cannot wrongly include excluded members as included or vice versa.
        'exclude' => {
          'type'=>'boolean',
          'path'=>'Characteristic.exclude',
          'min'=>1,
          'max'=>1
        },
        ##
        # Period over which characteristic is tested
        # The period over which the characteristic is tested; e.g. the patient had an operation during the month of June.
        'period' => {
          'type'=>'Period',
          'path'=>'Characteristic.period',
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
      # Kind of characteristic
      # A code that identifies the kind of trait being asserted.
      attr_accessor :code                           # 1-1 CodeableConcept
      ##
      # Value held by characteristic
      # The value of the trait that holds (or does not hold - see 'exclude') for members of the group.
      # For Range, it means members of the group have a value that falls somewhere within the specified range.
      attr_accessor :valueBoolean                   # 1-1 Boolean
      ##
      # Value held by characteristic
      # The value of the trait that holds (or does not hold - see 'exclude') for members of the group.
      # For Range, it means members of the group have a value that falls somewhere within the specified range.
      attr_accessor :valueCodeableConcept           # 1-1 CodeableConcept
      ##
      # Value held by characteristic
      # The value of the trait that holds (or does not hold - see 'exclude') for members of the group.
      # For Range, it means members of the group have a value that falls somewhere within the specified range.
      attr_accessor :valueQuantity                  # 1-1 Quantity
      ##
      # Value held by characteristic
      # The value of the trait that holds (or does not hold - see 'exclude') for members of the group.
      # For Range, it means members of the group have a value that falls somewhere within the specified range.
      attr_accessor :valueRange                     # 1-1 Range
      ##
      # Value held by characteristic
      # The value of the trait that holds (or does not hold - see 'exclude') for members of the group.
      # For Range, it means members of the group have a value that falls somewhere within the specified range.
      attr_accessor :valueReference                 # 1-1 Reference
      ##
      # Group includes or excludes
      # If true, indicates the characteristic is one that is NOT held by members of the group.
      # This is labeled as "Is Modifier" because applications cannot wrongly include excluded members as included or vice versa.
      attr_accessor :exclude                        # 1-1 boolean
      ##
      # Period over which characteristic is tested
      # The period over which the characteristic is tested; e.g. the patient had an operation during the month of June.
      attr_accessor :period                         # 0-1 Period
    end

    ##
    # Who or what is in group
    # Identifies the resource instances that are members of the group.
    class Member < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Member.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Member.extension',
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
          'path'=>'Member.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Reference to the group member
        # A reference to the entity that is a member of the group. Must be consistent with Group.type. If the entity is another group, then the type must be the same.
        'entity' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/Medication', 'http://hl7.org/fhir/StructureDefinition/Substance', 'http://hl7.org/fhir/StructureDefinition/Group'],
          'type'=>'Reference',
          'path'=>'Member.entity',
          'min'=>1,
          'max'=>1
        },
        ##
        # Period member belonged to the group
        # The period that the member was in the group, if known.
        'period' => {
          'type'=>'Period',
          'path'=>'Member.period',
          'min'=>0,
          'max'=>1
        },
        ##
        # If member is no longer in group
        # A flag to indicate that the member is no longer in the group, but previously may have been a member.
        'inactive' => {
          'type'=>'boolean',
          'path'=>'Member.inactive',
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
      # Reference to the group member
      # A reference to the entity that is a member of the group. Must be consistent with Group.type. If the entity is another group, then the type must be the same.
      attr_accessor :entity                         # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/Medication|http://hl7.org/fhir/StructureDefinition/Substance|http://hl7.org/fhir/StructureDefinition/Group)
      ##
      # Period member belonged to the group
      # The period that the member was in the group, if known.
      attr_accessor :period                         # 0-1 Period
      ##
      # If member is no longer in group
      # A flag to indicate that the member is no longer in the group, but previously may have been a member.
      attr_accessor :inactive                       # 0-1 boolean
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
    # Unique id
    # A unique business identifier for this group.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Whether this group's record is in active use
    # Indicates whether the record for the group is available for use or is merely being retained for historical purposes.
    attr_accessor :active                         # 0-1 boolean
    ##
    # person | animal | practitioner | device | medication | substance
    # Identifies the broad classification of the kind of resources the group includes.
    # Group members SHALL be of the appropriate resource type (Patient for person or animal; or Practitioner, Device, Medication or Substance for the other types.).
    attr_accessor :type                           # 1-1 code
    ##
    # Descriptive or actual
    # If true, indicates that the resource refers to a specific group of real individuals.  If false, the group defines a set of intended individuals.
    attr_accessor :actual                         # 1-1 boolean
    ##
    # Kind of Group members
    # Provides a specific type of resource the group includes; e.g. "cow", "syringe", etc.
    # This would generally be omitted for Person resources.
    attr_accessor :code                           # 0-1 CodeableConcept
    ##
    # Label for Group
    # A label assigned to the group for human identification and communication.
    attr_accessor :name                           # 0-1 string
    ##
    # Number of members
    # A count of the number of resource instances that are part of the group.
    # Note that the quantity may be less than the number of members if some of the members are not active.
    attr_accessor :quantity                       # 0-1 unsignedInt
    ##
    # Entity that is the custodian of the Group's definition
    # Entity responsible for defining and maintaining Group characteristics and/or registered members.
    # This does not strictly align with ownership of a herd or flock, but may suffice to represent that relationship in simple cases. More complex cases will require an extension.
    attr_accessor :managingEntity                 # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole)
    ##
    # Include / Exclude group members by Trait
    # Identifies traits whose presence r absence is shared by members of the group.
    # All the identified characteristics must be true for an entity to a member of the group.
    attr_accessor :characteristic                 # 0-* [ Group::Characteristic ]
    ##
    # Who or what is in group
    # Identifies the resource instances that are members of the group.
    attr_accessor :member                         # 0-* [ Group::Member ]

    def resourceType
      'Group'
    end
  end
end
