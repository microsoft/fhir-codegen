module FHIR

  ##
  # A formally or informally recognized grouping of people or organizations formed for the purpose of achieving some form of collective action.  Includes companies, institutions, corporations, departments, community groups, healthcare practice groups, payer/insurer, etc.
  class Organization < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['active', 'address-city', 'address-country', 'address-postalcode', 'address-state', 'address-use', 'address', 'endpoint', 'identifier', 'name', 'partof', 'phonetic', 'type']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'Organization.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'Organization.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'Organization.implicitRules',
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
        'path'=>'Organization.language',
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
        'path'=>'Organization.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'Organization.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Organization.extension',
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
        'path'=>'Organization.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Identifies this organization  across multiple systems
      # Identifier for the organization that is used to identify the organization across multiple disparate systems.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'Organization.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Whether the organization's record is still in active use.
      # This active flag is not intended to be used to mark an organization as temporarily closed or under construction. Instead the Location(s) within the Organization should have the suspended status. If further details of the reason for the suspension are required, then an extension on this element should be used.
      # 
      # This element is labeled as a modifier because it may be used to mark that the resource was created in error.
      'active' => {
        'type'=>'boolean',
        'path'=>'Organization.active',
        'min'=>0,
        'max'=>1
      },
      ##
      # Kind of organization
      # The kind(s) of organization that this is.
      # Organizations can be corporations, wards, sections, clinical teams, government departments, etc. Note that code is generally a classifier of the type of organization; in many applications, codes are used to identity a particular organization (say, ward) as opposed to another of the same type - these are identifiers, not codes
      # 
      # When considering if multiple types are appropriate, you should evaluate if child organizations would be a more appropriate use of the concept, as different types likely are in different sub-areas of the organization. This is most likely to be used where type values have orthogonal values, such as a religious, academic and medical center.
      # 
      # We expect that some jurisdictions will profile this optionality to be a single cardinality.
      'type' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/organization-type'=>[ 'prov', 'dept', 'team', 'govt', 'ins', 'pay', 'edu', 'reli', 'crs', 'cg', 'bus', 'other' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Organization.type',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/organization-type'}
      },
      ##
      # Name used for the organization
      # A name associated with the organization.
      # If the name of an organization changes, consider putting the old name in the alias column so that it can still be located through searches.
      'name' => {
        'type'=>'string',
        'path'=>'Organization.name',
        'min'=>0,
        'max'=>1
      },
      ##
      # A list of alternate names that the organization is known as, or was known as in the past.
      # There are no dates associated with the alias/historic names, as this is not intended to track when names were used, but to assist in searching so that older names can still result in identifying the organization.
      'alias' => {
        'local_name'=>'local_alias'
        'type'=>'string',
        'path'=>'Organization.alias',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # A contact detail for the organization.
      # The use code 'home' is not to be used. Note that these contacts are not the contact details of people who are employed by or represent the organization, but official contacts for the organization itself.
      'telecom' => {
        'type'=>'ContactPoint',
        'path'=>'Organization.telecom',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # An address for the organization.
      # Organization may have multiple addresses with different uses or applicable periods. The use code 'home' is not to be used.
      'address' => {
        'type'=>'Address',
        'path'=>'Organization.address',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The organization of which this organization forms a part.
      'partOf' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'Organization.partOf',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contact for the organization for a certain purpose.
      # Where multiple contacts for the same purpose are provided there is a standard extension that can be used to determine which one is the preferred contact to use.
      'contact' => {
        'type'=>'Organization::Contact',
        'path'=>'Organization.contact',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Technical endpoints providing access to services operated for the organization.
      'endpoint' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Endpoint'],
        'type'=>'Reference',
        'path'=>'Organization.endpoint',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Contact for the organization for a certain purpose.
    # Where multiple contacts for the same purpose are provided there is a standard extension that can be used to determine which one is the preferred contact to use.
    class Contact < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Contact.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Contact.extension',
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
          'path'=>'Contact.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # The type of contact
        # Indicates a purpose for which the contact can be reached.
        'purpose' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/contactentity-type'=>[ 'BILL', 'ADMIN', 'HR', 'PAYOR', 'PATINF', 'PRESS' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Contact.purpose',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/contactentity-type'}
        },
        ##
        # A name associated with the contact.
        'name' => {
          'type'=>'HumanName',
          'path'=>'Contact.name',
          'min'=>0,
          'max'=>1
        },
        ##
        # Contact details (telephone, email, etc.)  for a contact
        # A contact detail (e.g. a telephone number or an email address) by which the party may be contacted.
        'telecom' => {
          'type'=>'ContactPoint',
          'path'=>'Contact.telecom',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Visiting or postal addresses for the contact.
        'address' => {
          'type'=>'Address',
          'path'=>'Contact.address',
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
      # The type of contact
      # Indicates a purpose for which the contact can be reached.
      attr_accessor :purpose                        # 0-1 CodeableConcept
      ##
      # A name associated with the contact.
      attr_accessor :name                           # 0-1 HumanName
      ##
      # Contact details (telephone, email, etc.)  for a contact
      # A contact detail (e.g. a telephone number or an email address) by which the party may be contacted.
      attr_accessor :telecom                        # 0-* [ ContactPoint ]
      ##
      # Visiting or postal addresses for the contact.
      attr_accessor :address                        # 0-1 Address
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
    # Identifies this organization  across multiple systems
    # Identifier for the organization that is used to identify the organization across multiple disparate systems.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Whether the organization's record is still in active use.
    # This active flag is not intended to be used to mark an organization as temporarily closed or under construction. Instead the Location(s) within the Organization should have the suspended status. If further details of the reason for the suspension are required, then an extension on this element should be used.
    # 
    # This element is labeled as a modifier because it may be used to mark that the resource was created in error.
    attr_accessor :active                         # 0-1 boolean
    ##
    # Kind of organization
    # The kind(s) of organization that this is.
    # Organizations can be corporations, wards, sections, clinical teams, government departments, etc. Note that code is generally a classifier of the type of organization; in many applications, codes are used to identity a particular organization (say, ward) as opposed to another of the same type - these are identifiers, not codes
    # 
    # When considering if multiple types are appropriate, you should evaluate if child organizations would be a more appropriate use of the concept, as different types likely are in different sub-areas of the organization. This is most likely to be used where type values have orthogonal values, such as a religious, academic and medical center.
    # 
    # We expect that some jurisdictions will profile this optionality to be a single cardinality.
    attr_accessor :type                           # 0-* [ CodeableConcept ]
    ##
    # Name used for the organization
    # A name associated with the organization.
    # If the name of an organization changes, consider putting the old name in the alias column so that it can still be located through searches.
    attr_accessor :name                           # 0-1 string
    ##
    # A list of alternate names that the organization is known as, or was known as in the past.
    # There are no dates associated with the alias/historic names, as this is not intended to track when names were used, but to assist in searching so that older names can still result in identifying the organization.
    attr_accessor :local_alias                    # 0-* [ string ]
    ##
    # A contact detail for the organization.
    # The use code 'home' is not to be used. Note that these contacts are not the contact details of people who are employed by or represent the organization, but official contacts for the organization itself.
    attr_accessor :telecom                        # 0-* [ ContactPoint ]
    ##
    # An address for the organization.
    # Organization may have multiple addresses with different uses or applicable periods. The use code 'home' is not to be used.
    attr_accessor :address                        # 0-* [ Address ]
    ##
    # The organization of which this organization forms a part.
    attr_accessor :partOf                         # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Contact for the organization for a certain purpose.
    # Where multiple contacts for the same purpose are provided there is a standard extension that can be used to determine which one is the preferred contact to use.
    attr_accessor :contact                        # 0-* [ Organization::Contact ]
    ##
    # Technical endpoints providing access to services operated for the organization.
    attr_accessor :endpoint                       # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Endpoint) ]

    def resourceType
      'Organization'
    end
  end
end
