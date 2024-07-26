module FHIR

  ##
  # A person who is directly or indirectly involved in the provisioning of healthcare.
  # Need to track doctors, staff, locums etc. for both healthcare practitioners, funders, etc.
  class Practitioner < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['active', 'address-city', 'address-country', 'address-postalcode', 'address-state', 'address-use', 'address', 'communication', 'email', 'family', 'gender', 'given', 'identifier', 'name', 'phone', 'phonetic', 'telecom']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'Practitioner.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'Practitioner.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'Practitioner.implicitRules',
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
        'path'=>'Practitioner.language',
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
        'path'=>'Practitioner.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'Practitioner.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Practitioner.extension',
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
        'path'=>'Practitioner.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # An identifier for the person as this agent
      # An identifier that applies to this person in this role.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'Practitioner.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Whether this practitioner's record is in active use.
      # If the practitioner is not in use by one organization, then it should mark the period on the PractitonerRole with an end date (even if they are active) as they may be active in another role.
      'active' => {
        'type'=>'boolean',
        'path'=>'Practitioner.active',
        'min'=>0,
        'max'=>1
      },
      ##
      # The name(s) associated with the practitioner.
      # The selection of the use property should ensure that there is a single usual name specified, and others use the nickname (alias), old, or other values as appropriate.  In general, select the value to be used in the ResourceReference.display based on this:1. There is more than 1 name2. Use = usual3. Period is current to the date of the usage4. Use = official5. Other order as decided by internal business rules.
      'name' => {
        'type'=>'HumanName',
        'path'=>'Practitioner.name',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # A contact detail for the practitioner (that apply to all roles)
      # A contact detail for the practitioner, e.g. a telephone number or an email address.
      # Person may have multiple ways to be contacted with different uses or applicable periods.  May need to have options for contacting the person urgently and to help with identification.  These typically will have home numbers, or mobile numbers that are not role specific.
      'telecom' => {
        'type'=>'ContactPoint',
        'path'=>'Practitioner.telecom',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Address(es) of the practitioner that are not role specific (typically home address). Work addresses are not typically entered in this property as they are usually role dependent.
      # The PractitionerRole does not have an address value on it, as it is expected that the location property be used for this purpose (which has an address).
      'address' => {
        'type'=>'Address',
        'path'=>'Practitioner.address',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # male | female | other | unknown
      # Administrative Gender - the gender that the person is considered to have for administration and record keeping purposes.
      'gender' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/administrative-gender'=>[ 'male', 'female', 'other', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'Practitioner.gender',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/administrative-gender'}
      },
      ##
      # The date  on which the practitioner was born
      # The date of birth for the practitioner.
      'birthDate' => {
        'type'=>'date',
        'path'=>'Practitioner.birthDate',
        'min'=>0,
        'max'=>1
      },
      ##
      # Image of the person.
      'photo' => {
        'type'=>'Attachment',
        'path'=>'Practitioner.photo',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Certification, licenses, or training pertaining to the provision of care
      # The official certifications, training, and licenses that authorize or otherwise pertain to the provision of care by the practitioner.  For example, a medical license issued by a medical board authorizing the practitioner to practice medicine within a certian locality.
      'qualification' => {
        'type'=>'Practitioner::Qualification',
        'path'=>'Practitioner.qualification',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # A language the practitioner can use in patient communication.
      # The structure aa-BB with this exact casing is one the most widely used notations for locale. However not all systems code this but instead have it as free text. Hence CodeableConcept instead of code as the data type.
      'communication' => {
        'valid_codes'=>{
          'urn:ietf:bcp:47'=>[ 'ar', 'bn', 'cs', 'da', 'de', 'de-AT', 'de-CH', 'de-DE', 'el', 'en', 'en-AU', 'en-CA', 'en-GB', 'en-IN', 'en-NZ', 'en-SG', 'en-US', 'es', 'es-AR', 'es-ES', 'es-UY', 'fi', 'fr', 'fr-BE', 'fr-CH', 'fr-FR', 'fy', 'fy-NL', 'hi', 'hr', 'it', 'it-CH', 'it-IT', 'ja', 'ko', 'nl', 'nl-BE', 'nl-NL', 'no', 'no-NO', 'pa', 'pl', 'pt', 'pt-BR', 'ru', 'ru-RU', 'sr', 'sr-RS', 'sv', 'sv-SE', 'te', 'zh', 'zh-CN', 'zh-HK', 'zh-SG', 'zh-TW' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Practitioner.communication',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/languages'}
      }
    }

    ##
    # Certification, licenses, or training pertaining to the provision of care
    # The official certifications, training, and licenses that authorize or otherwise pertain to the provision of care by the practitioner.  For example, a medical license issued by a medical board authorizing the practitioner to practice medicine within a certian locality.
    class Qualification < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Qualification.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Qualification.extension',
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
          'path'=>'Qualification.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # An identifier for this qualification for the practitioner
        # An identifier that applies to this person's qualification in this role.
        'identifier' => {
          'type'=>'Identifier',
          'path'=>'Qualification.identifier',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Coded representation of the qualification.
        'code' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/v2-0360|2.7'=>[ 'AA', 'AAS', 'ABA', 'AE', 'AS', 'BA', 'BBA', 'BE', 'BFA', 'BN', 'BS', 'BSL', 'BSN', 'BT', 'CANP', 'CER', 'CMA', 'CNM', 'CNP', 'CNS', 'CPNP', 'CRN', 'CTR', 'DBA', 'DED', 'DIP', 'DO', 'EMT', 'EMTP', 'FPNP', 'HS', 'JD', 'MA', 'MBA', 'MCE', 'MD', 'MDA', 'MDI', 'ME', 'MED', 'MEE', 'MFA', 'MME', 'MS', 'MSL', 'MSN', 'MT', 'MTH', 'NG', 'NP', 'PA', 'PHD', 'PHE', 'PHS', 'PN', 'PharmD', 'RMA', 'RN', 'RPH', 'SEC', 'TS' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Qualification.code',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://terminology.hl7.org/ValueSet/v2-2.7-0360'}
        },
        ##
        # Period during which the qualification is valid.
        'period' => {
          'type'=>'Period',
          'path'=>'Qualification.period',
          'min'=>0,
          'max'=>1
        },
        ##
        # Organization that regulates and issues the qualification.
        'issuer' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
          'type'=>'Reference',
          'path'=>'Qualification.issuer',
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
      # An identifier for this qualification for the practitioner
      # An identifier that applies to this person's qualification in this role.
      attr_accessor :identifier                     # 0-* [ Identifier ]
      ##
      # Coded representation of the qualification.
      attr_accessor :code                           # 1-1 CodeableConcept
      ##
      # Period during which the qualification is valid.
      attr_accessor :period                         # 0-1 Period
      ##
      # Organization that regulates and issues the qualification.
      attr_accessor :issuer                         # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
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
    # An identifier for the person as this agent
    # An identifier that applies to this person in this role.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Whether this practitioner's record is in active use.
    # If the practitioner is not in use by one organization, then it should mark the period on the PractitonerRole with an end date (even if they are active) as they may be active in another role.
    attr_accessor :active                         # 0-1 boolean
    ##
    # The name(s) associated with the practitioner.
    # The selection of the use property should ensure that there is a single usual name specified, and others use the nickname (alias), old, or other values as appropriate.  In general, select the value to be used in the ResourceReference.display based on this:1. There is more than 1 name2. Use = usual3. Period is current to the date of the usage4. Use = official5. Other order as decided by internal business rules.
    attr_accessor :name                           # 0-* [ HumanName ]
    ##
    # A contact detail for the practitioner (that apply to all roles)
    # A contact detail for the practitioner, e.g. a telephone number or an email address.
    # Person may have multiple ways to be contacted with different uses or applicable periods.  May need to have options for contacting the person urgently and to help with identification.  These typically will have home numbers, or mobile numbers that are not role specific.
    attr_accessor :telecom                        # 0-* [ ContactPoint ]
    ##
    # Address(es) of the practitioner that are not role specific (typically home address). Work addresses are not typically entered in this property as they are usually role dependent.
    # The PractitionerRole does not have an address value on it, as it is expected that the location property be used for this purpose (which has an address).
    attr_accessor :address                        # 0-* [ Address ]
    ##
    # male | female | other | unknown
    # Administrative Gender - the gender that the person is considered to have for administration and record keeping purposes.
    attr_accessor :gender                         # 0-1 code
    ##
    # The date  on which the practitioner was born
    # The date of birth for the practitioner.
    attr_accessor :birthDate                      # 0-1 date
    ##
    # Image of the person.
    attr_accessor :photo                          # 0-* [ Attachment ]
    ##
    # Certification, licenses, or training pertaining to the provision of care
    # The official certifications, training, and licenses that authorize or otherwise pertain to the provision of care by the practitioner.  For example, a medical license issued by a medical board authorizing the practitioner to practice medicine within a certian locality.
    attr_accessor :qualification                  # 0-* [ Practitioner::Qualification ]
    ##
    # A language the practitioner can use in patient communication.
    # The structure aa-BB with this exact casing is one the most widely used notations for locale. However not all systems code this but instead have it as free text. Hence CodeableConcept instead of code as the data type.
    attr_accessor :communication                  # 0-* [ CodeableConcept ]

    def resourceType
      'Practitioner'
    end
  end
end
